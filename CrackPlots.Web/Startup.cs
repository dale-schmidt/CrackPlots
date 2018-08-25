using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(CrackPlots.Web.Startup))]
namespace CrackPlots.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
