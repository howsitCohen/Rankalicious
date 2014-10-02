using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Rankalicious.Startup))]
namespace Rankalicious
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
