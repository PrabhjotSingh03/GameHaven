using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Gamers_Hub.Startup))]
namespace Gamers_Hub
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
