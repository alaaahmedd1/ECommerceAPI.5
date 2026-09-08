namespace ECommerce.Application.Interfaces.Services;

public interface IChatService
{
    Task SaveMessageAsync(string senderId, string receiverId, string message);
}