using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace DudesPlayer.Api.Models
{
    public class ChatHub : Hub
    {
        public async Task Send(string user, string room, string message, bool join)
        {
            if (join)
            {
                await JoinRoom(room).ConfigureAwait(false);
                await Clients.Group(room).SendAsync("Receive", user, " joined to " + room).ConfigureAwait(true);
            }
            else
            {
                await Clients.Group(room).SendAsync("Receive", user, message).ConfigureAwait(true);
            }
        }
        public Task JoinRoom(string roomName)
        {
            return Groups.AddToGroupAsync(Context.ConnectionId, roomName);
        }
        public Task LeaveRoom(string roomName)
        {
            return Groups.RemoveFromGroupAsync(Context.ConnectionId, roomName);
        }
    }
}
