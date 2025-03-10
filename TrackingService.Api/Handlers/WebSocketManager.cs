using System.Net.WebSockets;
using System.Text;

namespace TrackingService.Api.Handlers;

using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

public class WebSocketManager
{
    private static readonly Dictionary<Guid, WebSocket> _employees = new(); // Employee connections
    private static readonly Dictionary<Guid, List<WebSocket>> _userListeners = new(); // EmployeeId -> List of users tracking them

    public async Task HandleWebSocket(HttpContext context)
    {
        if (!context.WebSockets.IsWebSocketRequest)
        {
            context.Response.StatusCode = 400;
            return;
        }
        
        var employeeId = context.Request.Query["employeeId"].ToString();
        var userId = context.Request.Query["userId"].ToString(); // Only provided by users

        // If employeeId is not a valid guid - close connection
        if (!Guid.TryParse(employeeId, out var empGuid))
        {
            context.Response.StatusCode = 400;
            await context.Response.WriteAsync("Invalid employeeId");
            return;
        }
        
        // Users can only connect if the employee is already online
        if (!string.IsNullOrEmpty(userId))
        {
            if (!Guid.TryParse(userId, out var userGuid))
            {
                context.Response.StatusCode = 400;
                await context.Response.WriteAsync("Invalid userId");
                return;
            }

            if (!_employees.ContainsKey(empGuid)) // Employee must be online first
            {
                context.Response.StatusCode = 404;
                await context.Response.WriteAsync("Employee is not connected");
                return;
            }
        }
    
        
        using var webSocket = await context.WebSockets.AcceptWebSocketAsync();

        var isUserConnecting = !string.IsNullOrEmpty(userId);
        
        if (isUserConnecting)
        {
            //  User connection: Add user to the employee's listeners
            if (!_userListeners.ContainsKey(empGuid))
            {
                _userListeners[empGuid] = new List<WebSocket>();
            }
            _userListeners[empGuid].Add(webSocket);
            await WaitForDisconnect(webSocket, empGuid);
        }
        else    
        {
            // Employee connection: Register employee's WebSocket
            _employees[empGuid] = webSocket;
            await ReceiveAndBroadcastLocation(empGuid, webSocket);
        }
    }

    private async Task ReceiveAndBroadcastLocation(Guid employeeId, WebSocket senderSocket)
    {
        var buffer = new byte[1024 * 4];

        while (senderSocket.State == WebSocketState.Open)
        {
            var result = await senderSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            if (result.MessageType == WebSocketMessageType.Close)
            {
                break;
            }

            var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
            await BroadcastToUsers(employeeId, message);
        }

        _employees.Remove(employeeId);
    }

    private async Task BroadcastToUsers(Guid employeeId, string message)
    {
        if (!_userListeners.ContainsKey(employeeId)) return;

        var messageBytes = Encoding.UTF8.GetBytes(message);
        var buffer = new ArraySegment<byte>(messageBytes);

        foreach (var userSocket in _userListeners[employeeId].Where(l => l.State == WebSocketState.Open))
        {
            await userSocket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
        }
    }

    private async Task WaitForDisconnect(WebSocket socket, Guid employeeId)
    {
        var buffer = new byte[1024];
        while (socket.State == WebSocketState.Open)
        {
            var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            if (result.MessageType == WebSocketMessageType.Close)
            {
                break;
            }
        }

        _userListeners[employeeId]?.Remove(socket);
    }
}