using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(WeiXinWeb.Startup))]
namespace WeiXinWeb
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
