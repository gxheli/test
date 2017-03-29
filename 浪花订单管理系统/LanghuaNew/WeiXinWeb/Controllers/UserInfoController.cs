using Commond;
using Newtonsoft.Json;
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
    public class UserInfoController : ApiController
    {
        public static string WeixinAppId = AccessTokenContainer.GetFirstOrDefaultAppId();
        // GET: api/UserInfo
        public HttpResponseMessage Get(string OpenID)
        {
            try
            {
                var qrResult = Senparc.Weixin.MP.AdvancedAPIs.UserApi.Info(WeixinAppId, OpenID);
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(qrResult.nickname, System.Text.Encoding.UTF8, "text/plain")
                };
            }
            catch
            {
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("", System.Text.Encoding.UTF8, "text/plain")
                };
            }
        }
        // Post: api/UserInfo
        public HttpResponseMessage Post(string OpenID)
        {
            try
            {
                var qrResult = Senparc.Weixin.MP.AdvancedAPIs.UserApi.Info(WeixinAppId, OpenID);
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(qrResult.headimgurl, System.Text.Encoding.UTF8, "text/plain")
                };
            }
            catch
            {
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("", System.Text.Encoding.UTF8, "text/plain")
                };
            }
        }
    }
}
