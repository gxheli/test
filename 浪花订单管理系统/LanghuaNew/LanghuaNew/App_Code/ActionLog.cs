using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LanghuaNew
{
    public class ActionLog:ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            log4net.ILog log = log4net.LogManager.GetLogger(filterContext.Controller.GetType());
            log.Info(filterContext.ActionDescriptor.ActionName);
            base.OnActionExecuting(filterContext);
        }

    }
}