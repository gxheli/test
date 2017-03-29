using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(LanghuaWapForCus.Startup))]
namespace LanghuaWapForCus
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            //ConfigureAuth(app);
        }
    }
}
