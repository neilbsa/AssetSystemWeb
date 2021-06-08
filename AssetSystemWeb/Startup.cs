using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(AssetSystemWeb.Startup))]
namespace AssetSystemWeb
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
