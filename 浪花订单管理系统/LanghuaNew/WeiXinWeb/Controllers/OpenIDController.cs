using Commond;
using Newtonsoft.Json;
using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin.MP.CommonAPIs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Configuration;
using System.Web.Http;

namespace WeiXinWeb.Controllers
{
    public class OpenIDController : ApiController
    {
        public static string WeixinAppId = AccessTokenContainer.GetFirstOrDefaultAppId();
        public static string WeixinAppSecret = System.Configuration.ConfigurationManager.AppSettings["WeixinAppSecret"];
        // GET: api/OpenID
        public HttpResponseMessage Get(string code)
        {
            var result = OAuthApi.GetAccessToken(WeixinAppId, WeixinAppSecret,code);
            string OpenID = result.openid;
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(OpenID, System.Text.Encoding.UTF8, "text/plain")
            };
        }
    }
}
