using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(KM.AdExpress.Web.Startup))]
namespace KM.AdExpress.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
