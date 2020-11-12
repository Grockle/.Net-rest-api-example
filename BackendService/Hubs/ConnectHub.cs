
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace BackendService.Hubs
{
    public class ConnectHub : Hub
    {
        public override async Task OnConnectedAsync(){
            await Clients.Caller.SendAsync("GetConnectionId",this.Context.ConnectionId);
        }
        
        public Task JoinGroup(string group)
        {
            return Groups.AddToGroupAsync(Context.ConnectionId, group);
        }

        public async Task SendMessage(string sender, string message)
        {
            await Clients.All.SendAsync("SendMessage", sender, message);
        }  

        public Task TriggerFunction(string groupName, string function)
        {
            return Clients.Group(groupName).SendAsync(function);
        }
    }
}
