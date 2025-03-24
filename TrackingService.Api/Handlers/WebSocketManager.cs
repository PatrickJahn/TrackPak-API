using System.Net.WebSockets;
using System.Text;

namespace TrackingService.Api.Handlers;

using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

public class WebSocketManager
{
    private static readonly Dictionary<Guid, WebSocket> _employees = new();
    private static readonly Dictionary<Guid, List<WebSocket>> _userListeners = new();

    public async Task HandleWebSocket(HttpContext context)
    {
        if (!context.WebSockets.IsWebSocketRequest)
        {
            context.Response.StatusCode = 400;
            return;
        }

        try
        {
            var employeeId = context.Request.Query["employeeId"].ToString();
            var userId = context.Request.Query["userId"].ToString();

            if (!Guid.TryParse(employeeId, out var empGuid))
                throw new Exception($"Invalid employee id: {employeeId}");

            if (!string.IsNullOrEmpty(userId))
            {
                if (!Guid.TryParse(userId, out var userGuid))
                {
                    context.Response.StatusCode = 400;
                    throw new Exception($"Invalid user id: {userId}");
                }

                if (!_employees.ContainsKey(empGuid)) // Employee must be online first
                {
                    context.Response.StatusCode = 404;
                    throw new Exception($"Employee is not connected");
                }
            }

            using var webSocket = await context.WebSockets.AcceptWebSocketAsync();

            if (!string.IsNullOrEmpty(userId))
            {
                if (!_userListeners.ContainsKey(empGuid))
                    _userListeners[empGuid] = new List<WebSocket>();

                _userListeners[empGuid].Add(webSocket);
                await WaitForDisconnect(webSocket, empGuid);
            }
            else
            {
                _employees[empGuid] = webSocket;
                await ReceiveAndBroadcastLocation(empGuid, webSocket);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"WebSocket Error: {ex.Message}");
            context.Response.StatusCode = 400;
        }
    }

    private async Task ReceiveAndBroadcastLocation(Guid employeeId, WebSocket senderSocket)
    {
        var buffer = new byte[1024 * 4];
        try
        {
            while (senderSocket.State == WebSocketState.Open)
            {
                var result = await senderSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                if (result.MessageType == WebSocketMessageType.Close)
                    break;

                var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                await BroadcastToUsers(employeeId, message);
            }
        }
        catch (WebSocketException ex)
        {
            Console.WriteLine($"WebSocket closed unexpectedly: {ex.Message}");
        }
        finally
        {
            _employees.Remove(employeeId);
            if (senderSocket.State != WebSocketState.Closed)
                await senderSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed by server", CancellationToken.None);
        }
    }

    private async Task BroadcastToUsers(Guid employeeId, string message)
    {
        if (!_userListeners.ContainsKey(employeeId)) return;

        var messageBytes = Encoding.UTF8.GetBytes(message);
        var buffer = new ArraySegment<byte>(messageBytes);

        foreach (var userSocket in _userListeners[employeeId].Where(l => l.State == WebSocketState.Open).ToList())
        {
            try
            {
                await userSocket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
            }
            catch (WebSocketException ex)
            {
                Console.WriteLine($"Failed to send to user: {ex.Message}");
            }
        }
    }

    private async Task WaitForDisconnect(WebSocket socket, Guid employeeId)
    {
        var buffer = new byte[1024];
        try
        {
            while (socket.State == WebSocketState.Open)
            {
                var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                if (result.MessageType == WebSocketMessageType.Close)
                    break;
            }
        }
        catch (WebSocketException ex)
        {
            Console.WriteLine($"User WebSocket closed unexpectedly: {ex.Message}");
        }
        finally
        {
            _userListeners[employeeId]?.Remove(socket);
            if (socket.State != WebSocketState.Closed)
                await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "User disconnected", CancellationToken.None);
        }
    }
}
