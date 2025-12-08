using Microsoft.AspNetCore.SignalR;

namespace DevTasker.Api.Hubs
{
    public class NotificationHub : Hub
    {
        // Called by clients to send a message
        public async Task SendMessage(string user, string message)
        {
            // Broadcast to all connected clients
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        // Example: send notification to a specific user (based on connection groups/ids)
        public async Task SendPrivate(string userId, string message)
        {
            await Clients.User(userId).SendAsync("ReceiveNotification", message);
        }
    }
}
