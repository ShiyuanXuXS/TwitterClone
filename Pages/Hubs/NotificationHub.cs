using Microsoft.AspNetCore.SignalR;
using TwitterClone.Models;
using TwitterClone.Data;
using TwitterClone.Models;

namespace TwitterClone.Pages.Hubs
{
    public class NotificationHub : Hub
    {
        // private readonly TwitterCloneDbContext context;
        // public NotificationHub(TwitterCloneDbContext context)
        // {
        //     this.context = context;
        // }
        // public override Task OnConnectedAsync()
        // {
        //     // Clients.Caller.SendAsync("OnConnected", context.HubConnections.ConnectionId);
        //     return base.OnConnectedAsync();
        // }

        // public async void SaveUserConnection(string username)
        // {
        //     var connectionId = Context.ConnectionId;
        //     HubConnection hubConnection = new HubConnection
        //     {
        //         ConnectionId = connectionId,
        //         Username = username
        //     };
        //     context.HubConnections.Add(hubConnection);
        //     await context.SaveChangesAsync();
        // }

        // public override Task OnDisconnectedAsync(System.Exception exception)
        // {
        //     var connectionId = Context.ConnectionId;
        //     var hubConnection = context.HubConnections.FirstOrDefault(hc => hc.ConnectionId == connectionId);
        //     if (hubConnection != null)
        //     {
        //         context.HubConnections.Remove(hubConnection);
        //         context.SaveChangesAsync();
        //     }
        //     return base.OnDisconnectedAsync(exception);
        // }
    }
}