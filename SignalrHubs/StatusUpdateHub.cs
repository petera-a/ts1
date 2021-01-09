using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace TS_WebApp.SignalrHubs
{
    public class StatusUpdateHub : Hub
    {
        public async Task SendDataUpdate(string message)
        {
            await Clients.All.receiveDataUpdate(message);
        }
    }
}