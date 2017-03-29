using System.Web;
using System.Web.Mvc;

namespace LanghuaWapForCus
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new CustomeAuthorizeAttribute());
            //filters.Add(new HandleErrorAttribute());
        }
    }
}
