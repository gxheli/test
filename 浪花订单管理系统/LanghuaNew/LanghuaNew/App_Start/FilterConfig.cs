using System.Web;
using System.Web.Mvc;

namespace LanghuaNew
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {

            filters.Add(new CustomeAuthorizeAttribute());
            //filters.Add(new HandleErrorAttribute());
            filters.Add(new ActionLog());
            filters.Add(new LogError());
        }
    }
}
