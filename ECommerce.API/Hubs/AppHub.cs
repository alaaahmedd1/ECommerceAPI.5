using ECommerce.Application.Interfaces.Services;
using ECommerce.Infrastructure.Services;
using Microsoft.AspNetCore.SignalR;

namespace ECommerce.API.Hubs;

public class AppHub : Hub
{
    private readonly IChatService _chatService;

    public AppHub(IChatService chatService)
    {
        _chatService = chatService;
    }

    public override async Task OnConnectedAsync()
    {
        var httpContext = Context.GetHttpContext();
        var userId = httpContext?.Request.Query["userId"].ToString();

        if (!string.IsNullOrEmpty(userId))
        {
            UserConnectionManager.AddConnection(userId, Context.ConnectionId);
            await Clients.Others.SendAsync("UserConnected", userId);
        }

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var httpContext = Context.GetHttpContext();
        var userId = httpContext?.Request.Query["userId"].ToString();

        if (!string.IsNullOrEmpty(userId))
        {
            UserConnectionManager.RemoveConnection(userId);
            await Clients.Others.SendAsync("UserDisconnected", userId);
        }

        await base.OnDisconnectedAsync(exception);
    }

    public async Task SendMessage(string receiverId, string message)
    {
        var senderId = Context.GetHttpContext()?.Request.Query["userId"].ToString() ?? "Unknown";

        await _chatService.SaveMessageAsync(senderId, receiverId, message);

        var targetConnectionId = UserConnectionManager.GetConnection(receiverId);

        if (!string.IsNullOrEmpty(targetConnectionId))
        {
            await Clients.Client(targetConnectionId).SendAsync("ReceiveMessage", senderId, message);
        }
    }
}