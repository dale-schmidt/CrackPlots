using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using ForeSight.Web.Domain;
using System.Threading.Tasks;

namespace ForeSight.Web.Classes.Hubs
{
    public class ProjectHub : Hub
    {
        public void UpdateAct(string groupName, Act act)
        {
            Clients.OthersInGroup(groupName).broadcastAct(act);
        }
        public Task JoinGroup(string groupName)
        {
            return Groups.Add(Context.ConnectionId, groupName);
        }
        public Task LeaveGroup(string groupName)
        {
            return Groups.Remove(Context.ConnectionId, groupName);
        }
    }
}