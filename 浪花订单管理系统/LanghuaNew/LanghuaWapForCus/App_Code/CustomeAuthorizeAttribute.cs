using System.Web.Mvc;

namespace LanghuaWapForCus
{
    public class CustomeAuthorizeAttribute : AuthorizeAttribute
    {
        // 在过程请求授权时调用。
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (!base.AuthorizeCore(filterContext.HttpContext))
            {
                string ControllerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
                string ActionName = filterContext.ActionDescriptor.ActionName;
                if (ControllerName == "Users" && ActionName == "ExtandAddTraveller")
                {
                    //已使用AllowAnonymous，但是无效，查不到原因，先在这里跳过检测
                }
                else if (filterContext.HttpContext.Request.IsAjaxRequest())
                {
                    filterContext.Result = new JsonResult()
                    {
                        Data = new { ErrorCode = 600, ErrorMessage = "检测到您已退出登录，请重新登录" }
                    };
                    return;
                }
            }
            //通过权限验证
            base.OnAuthorization(filterContext);
        }
    }
}