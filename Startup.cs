using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(BarkBuddyApp.Startup))]
namespace BarkBuddyApp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
