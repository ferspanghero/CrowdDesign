using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace CrowdDesign.UI.Web.Hubs
{
    public class MorphologicalChartHub : Hub
    {
        public void Refresh()
        {
            Clients.All.refresh();
        }
    }
}