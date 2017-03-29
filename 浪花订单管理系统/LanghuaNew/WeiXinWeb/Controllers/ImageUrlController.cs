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
    public class ImageUrlController : ApiController
    {
        public static string WeixinAppId = AccessTokenContainer.GetFirstOrDefaultAppId();
        // GET: api/ImageUrl
        public HttpResponseMessage Get(int ID)
        {
            var qrResult = Senparc.Weixin.MP.AdvancedAPIs.QrCodeApi.Create(WeixinAppId, 10000, ID);
            string qrCodeUrl = Senparc.Weixin.MP.AdvancedAPIs.QrCodeApi.GetShowQrCodeUrl(qrResult.ticket);
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(qrCodeUrl, System.Text.Encoding.UTF8, "text/plain")
            };
        }
    }
}
