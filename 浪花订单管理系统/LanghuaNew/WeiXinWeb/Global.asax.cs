using Senparc.Weixin.MP.CommonAPIs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace WeiXinWeb
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            RegisterSenparcWeixin();
        }
        private void RegisterSenparcWeixin()
        {
            AccessTokenContainer.Register(
                System.Configuration.ConfigurationManager.AppSettings["WeixinAppId"],
                System.Configuration.ConfigurationManager.AppSettings["WeixinAppSecret"]);
        }
    }
}
