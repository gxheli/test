using LanghuaNew.Data;
using Simple.OData.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Commond;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace LanghuaNew
{
    public class CustomeAuthorizeAttribute : System.Web.Mvc.AuthorizeAttribute
    {
        //private LanghuaContent db = new LanghuaContent();
        // 在过程请求授权时调用。
        public override void OnAuthorization(System.Web.Mvc.AuthorizationContext filterContext)
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
            //通过权限验证
            //if (!(filterContext.Result is HttpUnauthorizedResult))
            if (base.AuthorizeCore(filterContext.HttpContext))
            {

                string controllerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
                string actionName = filterContext.ActionDescriptor.ActionName;
                string UserName = filterContext.HttpContext.User.Identity.Name;

                //不管控接口权限
                if (!filterContext.HttpContext.Request.IsAjaxRequest())
                {
                    if ((controllerName != "Users" || actionName != "LogOut") && (controllerName != "NoRight" || actionName != "Index"))
                    {
                        bool bl = false;
                        try
                        {
                            var DefaultResult = HttpHelper.GetActionForOdata("odata/MenuRights?$filter=isDefault eq true&$expand=RoleRights").Result;
                            var DefaultRight = JsonConvert.DeserializeObject<List<MenuRight>>(((JObject)JsonConvert.DeserializeObject(DefaultResult))["value"].ToString());
                            foreach (var MenuRight in DefaultRight)
                            {
                                foreach (var rights in MenuRight.RoleRights)
                                {
                                    if (rights.ControllerName == controllerName && rights.ActionName == actionName)
                                    {
                                        bl = true;
                                        break;
                                    }
                                    if (bl) break;
                                }
                            }
                        }
                        catch { }
                        if (!bl)
                        {
                            string result = HttpHelper.GetActionForOdata("odata/Users?$filter=UserName eq '" + UserName + "'&$expand=UserRole($expand=MenuRights)").Result;
                            try
                            {
                                JObject ja = (JObject)JsonConvert.DeserializeObject(result);
                                string str = ja["value"].ToString();
                                List<User> uesrs = JsonConvert.DeserializeObject<List<User>>(str);
                                User user = uesrs[0];
                                if (user.UserRole != null)
                                {
                                    foreach (var item in user.UserRole.Where(s => s.RoleEnableState == EnableState.Enable))
                                    {
                                        //超级管理员
                                        if (item.RoleID == 1)
                                        {
                                            bl = true;
                                            break;
                                        }
                                        if (item.MenuRights != null)
                                        {
                                            foreach (var MenuRight in item.MenuRights)
                                            {
                                                var MenuResult = JsonConvert.DeserializeObject<MenuRight>(HttpHelper.GetActionForOdata("odata/MenuRights(" + MenuRight.MenuRightID + ")?$expand=RoleRights").Result);
                                                foreach (var rights in MenuResult.RoleRights)
                                                {
                                                    if (rights.ControllerName == controllerName && rights.ActionName == actionName)
                                                    {
                                                        bl = true;
                                                        break;
                                                    }
                                                }
                                                if (bl) break;
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
                                filterContext.Result = new RedirectResult("~/users/Login");
                            }
                        }
                    }
                }
            }
        }
    }
}