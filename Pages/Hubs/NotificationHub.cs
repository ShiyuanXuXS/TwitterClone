using Microsoft.AspNetCore.SignalR;


namespace TwitterClone.Pages.Hubs
{
    public class NotificationHub : Hub
    {
        public override Task OnConnectedAsync()
        {
            Clients.Caller.SendAsync("OnConnected", Context.ConnectionId);
            return base.OnConnectedAsync();
        }
    }
}