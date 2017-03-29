using Commond;
using Newtonsoft.Json;
using Senparc.Weixin.Entities;
using Senparc.Weixin.Exceptions;
using Senparc.Weixin.MP;
using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin.MP.AdvancedAPIs.TemplateMessage;
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
    public class SendMessageController : ApiController
    {
        public static string WeixinAppId = AccessTokenContainer.GetFirstOrDefaultAppId();
        public static string ForCusPath = System.Configuration.ConfigurationManager.AppSettings["ForCusPath"];
        public static string WeixinTemplateId = System.Configuration.ConfigurationManager.AppSettings["WeixinTemplateId"];

        // Post: api/SendMessage
        // 发送状态变更微信通知
        public HttpResponseMessage Post(string OpenID, string Title, string OrderNo, string ItemName, string Remark, string Url, string state)
        {
            try
            {
                var wap = ApiHandlerWapper.TryCommonApi(accessToken =>
                 {
                     var SendData = new
                     {
                         first = new TemplateDataItem(Title),
                         OrderID = new TemplateDataItem(OrderNo),
                         PkgName = new TemplateDataItem(ItemName),
                         Remark = new TemplateDataItem(Remark)
                     };
                     if (!string.IsNullOrEmpty(Url) && Url.Contains(ForCusPath))
                     {
                         Url = OAuthApi.GetAuthorizeUrl(WeixinAppId, Url, state, OAuthScope.snsapi_base);
                     }
                     var result = TemplateApi.SendTemplateMessage(WeixinAppId, OpenID, WeixinTemplateId, "#FF0000", Url, SendData);
                     WxJsonResult wx = new WxJsonResult();
                     wx.errcode = result.errcode;
                     return wx;
                 });
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(JsonConvert.SerializeObject((int)wap.errcode), System.Text.Encoding.UTF8, "text/plain")
                };
            }
            catch (ErrorJsonResultException ex)
            {
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(JsonConvert.SerializeObject((int)ex.JsonResult.errcode), System.Text.Encoding.UTF8, "text/plain")
                };
            }
        }
        // Put: api/SendMessage
        // 发送模板通知
        public HttpResponseMessage Put(string OpenID, string Title, string Remark, string Url, string state,string Template, string keyword1, string keyword2="", string keyword3="")
        {
            string TemplateID = System.Configuration.ConfigurationManager.AppSettings[Template];
            try
            {
                var wap = ApiHandlerWapper.TryCommonApi(accessToken =>
                 {
                     var SendData = new
                     {
                         first = new TemplateDataItem(Title),
                         keyword1 = new TemplateDataItem(keyword1),
                         keyword2 = new TemplateDataItem(keyword2),
                         keyword3 = new TemplateDataItem(keyword3),
                         remark = new TemplateDataItem(Remark)
                     };
                     if (!string.IsNullOrEmpty(Url) && Url.Contains(ForCusPath))
                     {
                         Url = OAuthApi.GetAuthorizeUrl(WeixinAppId, Url, state, OAuthScope.snsapi_base);
                     }
                     var result = TemplateApi.SendTemplateMessage(WeixinAppId, OpenID, TemplateID, "#FF0000", Url, SendData);
                     WxJsonResult wx = new WxJsonResult();
                     wx.errcode = result.errcode;
                     return wx;
                 });
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(JsonConvert.SerializeObject((int)wap.errcode), System.Text.Encoding.UTF8, "text/plain")
                };
            }
            catch (ErrorJsonResultException ex)
            {
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(JsonConvert.SerializeObject((int)ex.JsonResult.errcode), System.Text.Encoding.UTF8, "text/plain")
                };
            }
        }
    }
}
