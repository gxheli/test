using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
[assembly: log4net.Config.XmlConfigurator(Watch = true)]
namespace LanghuaNew
{
    public class LogError: HandleErrorAttribute
    {
        public override void OnException(ExceptionContext filterContext)
        {
            log4net.ILog log = log4net.LogManager.GetLogger(filterContext.Controller.GetType());
            log.Error("Error",filterContext.Exception);
            base.OnException(filterContext);

        }
    }
}