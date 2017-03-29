using LanghuaNew.Data;
using Newtonsoft.Json;
using Simple.OData.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Top.Api.Util;

namespace Everything.Controllers
{
    public class OAuth2Controller : Controller
    {
        private ODataClient client = new ODataClient(ConfigurationManager.AppSettings["ServicePath"] + "odata/");
        public ActionResult TaobaoLogin()
        {
            var url = "{0}?response_type=code" +
            "&client_id={1}&redirect_uri={2}&state=1212&view=tmall";
            var auth_URL = ConfigurationManager.AppSettings["tb_Authorize_URL"];
            var cilentId = HttpUtility.UrlEncode(ConfigurationManager.AppSettings["client_id"]);
            var redirectUri = HttpUtility.UrlEncode(ConfigurationManager.AppSettings["redirect_uri"]);
            //Session["CallBack_URL"] = Request.UrlReferrer.ToString();
            return Redirect(string.Format(url, auth_URL, cilentId, redirectUri));
        }

        public async Task<ActionResult> TaobaoCallBack()
        {
            //error = access_denied & error_description = parent + account + should + authorize +in+web + way + first.& state = 1212
            if (!string.IsNullOrEmpty(Request.QueryString["error"]))
            {
                TempData["Description"] = Request.QueryString["error"] + " " + Request.QueryString["error_description"];
                //return Redirect(ConfigurationManager.AppSettings["default_App_Path"]);
                //Response.Redirect("/Error/Index");
                return RedirectToAction("Index", "Error");
            }
            else
            {
                string str = await GetTokenByCode(Request.QueryString["code"]);
                string callStr = Session["CallBack_URL"] != null ? Session["CallBack_URL"].ToString() : ConfigurationManager.AppSettings["default_App_Path"];
                //if (!string.IsNullOrEmpty(str))
                //{
                //    Session["Session_Key"] = str;
                //}
                return Redirect(callStr);
            }

        }

        public async Task<string> GetTokenByCode(string code)
        {
            string accessToken = string.Empty;
            try
            {
                string url = ConfigurationManager.AppSettings["tb_Token_URL"];
                Dictionary<string, string> props = new Dictionary<string, string>();
                props.Add("grant_type", "authorization_code");
                props.Add("client_id", System.Configuration.ConfigurationManager.AppSettings["client_id"]);
                props.Add("client_secret", System.Configuration.ConfigurationManager.AppSettings["client_secret"]);
                props.Add("code", code);
                props.Add("redirect_uri", Session["CallBack_URL"] != null ? Session["CallBack_URL"].ToString() : ConfigurationManager.AppSettings["default_App_Path"]);
                //props.Add("view", "web");
                try
                {
                    WebUtils webUtils = new WebUtils();
                    string resonseJson = webUtils.DoPost(url, props);
                    TB_Access_Token tb_Access_Token = new TB_Access_Token();
                    tb_Access_Token = JsonConvert.DeserializeAnonymousType(resonseJson, tb_Access_Token);
                    accessToken = tb_Access_Token.access_token;
                    //var item = db.TB_Access_Tokens.FirstOrDefault();
                    var item = await client.For<TB_Access_Token>().FindEntryAsync();
                    if (item != null)
                    {
                        await client.For<TB_Access_Token>().Key(item.ID).DeleteEntryAsync();
                        //db.TB_Access_Tokens.Remove(item);
                    }
                    tb_Access_Token.taobao_user_nick = HttpUtility.UrlDecode(tb_Access_Token.taobao_user_nick);
                    await client.For<TB_Access_Token>().Set(tb_Access_Token).InsertEntryAsync();
                    //db.TB_Access_Tokens.Add(tb_Access_Token);
                    //db.SaveChanges();
                    return accessToken;
                }
                catch (IOException e)
                {
                    // TODO: Add insert logic here
                    return accessToken;
                }
            }
            catch (Exception ex)
            {
                return accessToken;
            }
        }
    }
}