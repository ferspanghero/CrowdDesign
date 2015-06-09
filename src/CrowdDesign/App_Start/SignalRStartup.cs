using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(CrowdDesign.UI.Web.SignalRStartup))]

namespace CrowdDesign.UI.Web
{
    public class SignalRStartup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
        }
    }
}
