using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(NEE.Web.Startup))]

namespace NEE.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
