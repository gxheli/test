using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace LanghuaForSup
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                   "Globalization", // 路由名称
                   "{culture}/{controller}/{action}/{id}", // 带有参数的 URL
                   new { culture = "zh-CN", controller = "Home", action = "Index", id = UrlParameter.Optional }, //参数默认值
                   new { culture = "^[a-z]{2}-[A-Z]{2}?$" }    //参数约束
               );


            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
