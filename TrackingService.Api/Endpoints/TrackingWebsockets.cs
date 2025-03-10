using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using TrackingService.Api.Handlers;
using WebSocketManager = TrackingService.Api.Handlers.WebSocketManager;

namespace TrackingService.Api.Endpoints;

public static class TrackingWebsockets
{
    public static void MapTrackingWebsockets(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/tracking");
        var webSocketManager = new WebSocketManager();

        group.Map("/ws", async context =>
        {
           await webSocketManager.HandleWebSocket(context);
        });
        
       
    }
}




