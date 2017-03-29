using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LanghuaForCus
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            //filters.Add(new AuthorizeAttribute());
            filters.Add(new CustomeAuthorizeAttribute());
            //filters.Add(new HandleErrorAttribute());
            //filters.Add(new CaptchaValidatorAttribute());
            filters.Add(new LogError());
        }
    }
}
