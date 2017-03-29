using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LanghuaForCus
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class LogError : HandleErrorAttribute
    {
        public override void OnException(ExceptionContext filterContext)
        {
            //使用log4net或其他记录错误消息
            log4net.ILog log = log4net.LogManager.GetLogger(filterContext.Controller.GetType());
            log.Error("Error", filterContext.Exception);

            Exception Error = filterContext.Exception;
            string Message = Error.Message;//错误信息
            string Url = HttpContext.Current.Request.RawUrl;//错误发生地址

            filterContext.ExceptionHandled = true;
            //Application["error"] = Message;
            //filterContext.Result = new RedirectResult("~/Error");//跳转至错误提示页面

        }

    }
}