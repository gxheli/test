using Commond;
using LanghuaNew.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Security;

namespace LanghuaForSup
{
    public class CustomeAuthorizeAttribute : AuthorizeAttribute
    {
        // 在过程请求授权时调用。
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (!base.AuthorizeCore(filterContext.HttpContext))
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
            //通过权限验证
            base.OnAuthorization(filterContext);

            if (base.AuthorizeCore(filterContext.HttpContext))
            {

                string controllerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
                string actionName = filterContext.ActionDescriptor.ActionName;
                string UserName = filterContext.HttpContext.User.Identity.Name;

                //不管控接口权限
                if (!filterContext.HttpContext.Request.IsAjaxRequest())
                {
                    if ((controllerName != "langhua" || actionName != "LogOut") && (controllerName != "NoRight" || actionName != "Index"))
                    {
                        bool bl = false;
                        try
                        {
                            string result = HttpHelper.GetActionForOdata("odata/SupplierUsers?$filter=SupplierUserName eq '" + UserName + "'&$expand=SupplierRoles,SupplierRoles($expand=Rights)").Result;
                            JObject ja = (JObject)JsonConvert.DeserializeObject(result);

                            string str = ja["value"].ToString();
                            List<SupplierUser> uesrs = JsonConvert.DeserializeObject<List<SupplierUser>>(str);
                            SupplierUser user = uesrs[0];
                            if (user.SupplierRoles != null)
                            {
                                foreach (var item in user.SupplierRoles)
                                {
                                    //超级管理员
                                    if (item.SupplierRoleID == 1)
                                    {
                                        bl = true;
                                        break;
                                    }
                                    if (item.Rights != null)
                                    {
                                        foreach (var rights in item.Rights)
                                        {
                                            if (rights.ControllerName == controllerName && rights.ActionName == actionName)
                                            {
                                                bl = true;
                                                break;
                                            }
                                        }
                                        if (bl) break;
                                    }
                                }
                            }
                            if (!bl)
                            {
                                if (filterContext.HttpContext.Request.IsAjaxRequest())
                                {
                                    //Ajax 输出错误信息给脚本
                                    filterContext.Result = new JsonResult()
                                    {
                                        Data = new { ErrorCode = 402, ErrorMessage = "权限不足" }
                                    };
                                }
                                else
                                {
                                    filterContext.Result = new RedirectResult("/NoRight");
                                }
                            }
                        }
                        catch
                        {
                            FormsAuthentication.SignOut();
                            filterContext.Result = new RedirectResult("~/langhua/Login");
                        }
                    }
                }
            }
        }
    }
}