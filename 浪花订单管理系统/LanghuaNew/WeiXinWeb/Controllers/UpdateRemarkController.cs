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
    public class UpdateRemarkController : ApiController
    {
        public static string WeixinAppId = AccessTokenContainer.GetFirstOrDefaultAppId();
        // GET: api/UpdateRemark
        public HttpResponseMessage Get(string OpenID, string Remark)
        {
            var Result = UserApi.UpdateRemark(WeixinAppId, OpenID, Remark);
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject((int)Result.errcode), System.Text.Encoding.UTF8, "text/plain")
            };
        }
    }
}
