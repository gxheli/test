using System.Web.Mvc;

namespace LanghuaForCus
{
    public class CustomeAuthorizeAttribute : AuthorizeAttribute
    {
        // 在过程请求授权时调用。
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (!base.AuthorizeCore(filterContext.HttpContext))
            {
                string controllerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
                if (controllerName != "langhua")
                {
                    if (filterContext.HttpContext.Request.IsAjaxRequest())
                    {
                        //Ajax 输出错误信息给脚本
                        filterContext.Result = new JsonResult()
                        {
                            Data = new { ErrorCode = 600, ErrorMessage = "检测到您已退出登录，请重新登录" }
                        };
                        return;
                    }
                }
            }
            //通过权限验证
            base.OnAuthorization(filterContext);
        }
    }
}