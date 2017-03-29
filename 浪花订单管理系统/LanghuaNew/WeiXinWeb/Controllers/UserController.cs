using Senparc.Weixin.MP.AdvancedAPIs;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WeiXinWeb.Controllers
{
    public class UserController : Controller
    {
        // GET: User
        public ActionResult Index(string code, string state)
        {
            if (string.IsNullOrEmpty(code))
            {
                return Content("您拒绝了授权！");
            }

            if (state != "User")
            {
                return Content("验证失败！请从正规途径进入！");
            }
            string appId = ConfigurationManager.AppSettings["WeixinAppId"];
            string secret = ConfigurationManager.AppSettings["WeixinAppSecret"];
            //通过，用code换取access_token
            var result = OAuthApi.GetAccessToken(appId, secret, code);
            ViewBag.OpenID = result.openid;

            return View();
        }
    }
}