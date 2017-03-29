using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using System.Web.Optimization;
using System.Web.Routing;

namespace LanghuaForCus
{
    public class MvcApplication : System.Web.HttpApplication
    {

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            log4net.Config.XmlConfigurator.Configure();
        }

        protected void Application_Error(object sender,EventArgs e)
        {
            log4net.ILog log = log4net.LogManager.GetLogger("Server_Error");
            string error = string.Empty;
            string userAgentStr = string.Empty;
            string message = string.Empty;
            var lastError = Server.GetLastError();
            if ((new HttpRequestWrapper(Request)).IsAjaxRequest())
            {
                Response.StatusCode = 500;
                return;
            }
            if (lastError != null)
            {
                var httpError = lastError as HttpException;
                if (httpError!=null)
                {
                    var httpCode = httpError.GetHttpCode();
                    if (httpCode > 500)
                    {
                        error = "发生异常页:" + Request.Url.ToString() + " \n\r ";
                        userAgentStr = "userAgent:" + Request.UserAgent.ToString() + " \n\r ";
                        message = "异常信息: " + lastError.GetBaseException() + " \n\r ";
                        log.Error(httpError);
                        Server.ClearError();
                        Response.Redirect("~/Error");
                    }
                    else
                    {
                        //Server.ClearError();
                        return;
                    }
                }
                error = "发生异常页:" + Request.Url.ToString() + " \n\r ";
                userAgentStr = "userAgent:" + Request.UserAgent.ToString() + " \n\r ";
                message = "异常信息: " + lastError.GetBaseException() + " \n\r ";
                log.Error(error + userAgentStr + message + lastError);
                Response.StatusCode = 500;
                Server.ClearError();
                Response.Redirect("~/Error");
            }
        }

    }
}
