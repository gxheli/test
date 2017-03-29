using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace LanghuaForSup
{
    public class I18nFilterAttribute : ActionFilterAttribute
    {
        private const string currentCulture = "culture";
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            string culture = (string)filterContext.RouteData.Values["culture"];

            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo(culture);
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(culture);
            if (filterContext.HttpContext.Response.Cookies[currentCulture] == null)
            {

                HttpCookie cultrueCookie = new HttpCookie(currentCulture, culture);
                cultrueCookie.Expires = DateTime.Now.AddDays(1);
                filterContext.HttpContext.Response.Cookies.Add(cultrueCookie);

            }
            else
            {
                filterContext.HttpContext.Response.Cookies[currentCulture].Value = culture;
                filterContext.HttpContext.Response.Cookies[currentCulture].Expires = DateTime.Now.AddDays(1);
            }
        }
    }
}