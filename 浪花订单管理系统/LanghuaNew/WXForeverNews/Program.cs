using Senparc.Weixin;
using Senparc.Weixin.Entities;
using Senparc.Weixin.MP;
using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin.MP.AdvancedAPIs.TemplateMessage;
using Senparc.Weixin.MP.CommonAPIs;
using Senparc.Weixin.MP.Entities.Menu;
using Senparc.Weixin.MP.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WXData;
using Senparc.Weixin.HttpUtility;
using System.IO;
using Senparc.Weixin.MP.AdvancedAPIs.User;
using Newtonsoft.Json;
using System.Net.Http;
using Commond;

namespace WXForeverNews
{
    class Program
    {
        public static UserInfoJson Info(string accessTokenOrAppId, string openId, Language lang = Language.zh_CN)
        {
            return ApiHandlerWapper.TryCommonApi(accessToken =>
            {
                string url = string.Format("https://api.weixin.qq.com/cgi-bin/user/info?access_token={0}&openid={1}&lang={2}",
                    accessToken.AsUrlData(), openId.AsUrlData(), lang.ToString("g").AsUrlData());
                return Get.GetJson<UserInfoJson>(url);

                //错误时微信会返回错误码等信息，JSON数据包示例如下（该示例为AppID无效错误）:
                //{"errcode":40013,"errmsg":"invalid appid"}

            }, accessTokenOrAppId);
        }
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Hello world");
            }
            finally
            {
                Console.WriteLine("Finally executing");
            }


            //string arg0 = HttpHelper.UrlEncode(EncryptHelper.Encode("wx41a3a7803e7cba777"));
            //string arg1 = HttpHelper.UrlEncode(EncryptHelper.Encode("8227dcf631aa03233037d1af11b42e7e"));

            //HttpResponseMessage Message = HttpHelper.GetActionForWeixin("Token?arg0=" + arg0 + "&arg1=" + arg1).Result;
            //string token = Message.Content.ReadAsStringAsync().Result;
            ////string token = WeiXinHelper.getToken();
            //Console.WriteLine(token);
            Console.WriteLine();
            Console.ReadLine();

            //AccessTokenContainer.Register("wx41a3a7803e7cba77", "8227dcf631aa03233037d1af11b42e7e");
            //var text = AccessTokenContainer.GetAccessToken("wx41a3a7803e7cba77");
            //Console.WriteLine(JsonConvert.SerializeObject(Info("wx41a3a7803e7cba77", "ofSipv2bLYRzqnk26efjlbtT6wyI")));
            ////FileStream steam = new FileStream("E:\\text.txt", FileMode.Append);
            ////StreamWriter sw = new StreamWriter(steam);
            //////开始写入
            ////sw.WriteLine(text);
            //////清空缓冲区
            ////sw.Flush();
            //////关闭流
            ////sw.Close();
            ////steam.Close();
            //Console.WriteLine();
            //Console.ReadLine();

            //var openId = "oWJubwM9YXpLuxuZcKQamVl1njOY";//换成已经关注用户的openId
            //var templateId = "0ufLKLezvScIITS8mh1cvaYBbHAf93x_KzWBLoexHGc";//换成已经在微信后台添加的模板Id
            //string appId = "wxce143aaa1a44a6c9";
            //string appSecret = "b09285d131302f273402a54ae0e006f7";
            // AccessTokenContainer.Register(appId, appSecret);
            //  var accessToken = AccessTokenContainer.GetAccessToken(appId);


            //string ReleaseAppID = "wx2a86ed7c85745eff";
            //string ReleaseappSecret = "465f39dc1cde7000bbd874d14d36631f";

            //AccessTokenContainer.Register(ReleaseAppID, ReleaseappSecret);
            //var ReleaseaccessToken = AccessTokenContainer.GetAccessToken(ReleaseAppID);
            //string _custonPassWord = MD5UtilHelper.GetMD5("123123", null);
            //var test=ApiHandlerWapper.TryCommonApi(accessToken1 =>
            //{
            //    var urlFormat = string.Format("https://api.weixin.qq.com/customservice/kfaccount/add?access_token={0}", accessToken1.AsUrlData());

            //    var data = new
            //    {
            //        kf_account = "xinxin@gh_11a4b88c293f",
            //        nickname = "xinxin"

            //    };

            //    return CommonJsonSend.Send<WxJsonResult>(null, urlFormat, data, CommonJsonSendType.POST, timeOut: Config.TIME_OUT);

            //}, ReleaseaccessToken);


            //  var result = CustomServiceApi.AddCustom(accessToken, "xinxin@gh_11a4b88c293f", "xinxin", _custonPassWord);
            //   var result1 = CustomServiceApi.InviteWorker(accessToken, "xinxin@gh_11a4b88c293f", "xing98218");
            //   var result2= CustomServiceApi.GetCustomBasicInfo(accessToken);
            //var testData = new //TestTemplateData()
            //{
            //    first = new TemplateDataItem("您好，您的团队游订单，我们已为您送签了，请耐心等待签证结果噢。"),
            //    OrderID = new TemplateDataItem("123456789"),
            //    PkgName = new TemplateDataItem("甲米+普吉岛7日跟团游(4钻)·泰神奇 双秀 2晚国5"),
            //    remark = new TemplateDataItem("预计出签日期: 2014年7月7日")
            //};
            //var result = TemplateApi.SendTemplateMessage(accessToken, openId, templateId, "#FF0000", "", testData);
            //Console.WriteLine(result.errcode);
            //Console.ReadLine();
            // WeiXinContent Content = new WeiXinContent();
            //  LHNew news = Content.LHNews.Include("Articles").Where(p => p.LHNewID == 50).FirstOrDefault();





            //AccessTokenContainer.Register("wxce143aaa1a44a6c9", "b09285d131302f273402a54ae0e006f7");
            //   var accessToken = AccessTokenContainer.GetAccessToken("wxce143aaa1a44a6c9");
            //#region 图文素材初始化
            //WeiXinContent Content = new WeiXinContent();
            //int NewsCount = MediaApi.GetMediaCount(ReleaseaccessToken).news_count;
            //Console.WriteLine("数据总条数:" + NewsCount);
            //int i = 0;

            //while (true)
            //{
            //    var result = MediaApi.GetNewsMediaList(ReleaseaccessToken, i, 20);

            //    foreach (var item in result.item)
            //    {
            //        LHNew lhnew = new LHNew();

            //        lhnew.media_id = item.media_id;
            //        //lhnew.update_time = item.update_time;
            //        DateTime start = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            //        DateTime date = start.AddMilliseconds(item.update_time*1000).ToLocalTime();
            //        Console.WriteLine(date);
            //        lhnew.Articles = new List<LHArticle>();
            //        foreach (var contentitem in item.content.news_item)
            //        {
            //            LHArticle art = new LHArticle();
            //            art.Description = contentitem.digest;
            //            art.PicUrl = contentitem.thumb_url;
            //            art.Title = contentitem.title;
            //            art.Url = contentitem.url;
            //            lhnew.Articles.Add(art);
            //        }
            //        Content.LHNews.Add(lhnew);
            //    }
            //    Console.WriteLine("已经读取" + (i + result.item_count) + "条");
            //    if (result.item_count < 20 || i + 20 == NewsCount)
            //    {
            //        break;
            //    }

            //    i = i + 20;

            //}
            //Console.ReadLine();
            //// Content.SaveChanges();
            //#endregion

            //#region 菜单初始化
            //ButtonGroup bg = new ButtonGroup();

            //var FirstSubButton = new SubButton()
            //{
            //    name = "玩转泰国"
            //};
            //FirstSubButton.sub_button.Add(new SingleClickButton()
            //{
            //    key = "News_50",
            //    name = "最美普吉"
            //});
            //FirstSubButton.sub_button.Add(new SingleClickButton()
            //{
            //    key = "News_51",
            //    name = "童话清迈"
            //});
            //FirstSubButton.sub_button.Add(new SingleClickButton()
            //{
            //    key = "News_52",
            //    name = "梦幻苏梅"
            //});
            //FirstSubButton.sub_button.Add(new SingleClickButton()
            //{
            //    key = "News_53",
            //    name = "精华攻略"
            //});
            //FirstSubButton.sub_button.Add(new SingleClickButton()
            //{
            //    key = "News_54",
            //    name = "行程推荐"
            //});

            //bg.button.Add(FirstSubButton);

            //var SecondSubButton = new SubButton()
            //{
            //    name = "实用工具"
            //};
            //SecondSubButton.sub_button.Add(new SingleViewButton()
            //{
            //    url = "http://bbs.16fan.com/forum.php?mod=weather",
            //    name = "天气查询"
            //});
            //SecondSubButton.sub_button.Add(new SingleViewButton()
            //{
            //    url = "http://superrichthai.com/exchange",
            //    name = "泰国当地汇率"
            //});
            //SecondSubButton.sub_button.Add(new SingleViewButton()
            //{
            //    url = "http://bak.dodotour.cn/langhua/checkin.asp",
            //    name = "登记备用号码"
            //});
            //bg.button.Add(SecondSubButton);

            //var ThirdSubButton = new SubButton()
            //{
            //    name = "浪花家的"
            //};
            //ThirdSubButton.sub_button.Add(new SingleClickButton()
            //{
            //    key = "News_2",
            //    name = "正在优惠"
            //});
            //ThirdSubButton.sub_button.Add(new SingleViewButton()
            //{
            //    url = "http://mp.weixin.qq.com/s?biz=MjM5ODU0NzUyMA==&mid=208380985&idx=1&sn=9fe4d3be2cc9aeb7491ecd6747ba0cc7&scene=18",
            //    name = "游艇汇"
            //});
            //ThirdSubButton.sub_button.Add(new SingleClickButton()
            //{
            //    key = "Texts_1",
            //    name = "人工客服"
            //});
            //ThirdSubButton.sub_button.Add(new SingleClickButton()
            //{
            //    key = "Texts_2",
            //    name = "店铺网址"
            //});
            //ThirdSubButton.sub_button.Add(new SingleViewButton()
            //{
            //    url = OAuthApi.GetAuthorizeUrl(appId, "http://226b0c3e.ngrok.natapp.cn/User", "User", OAuthScope.snsapi_base),
            //    name = "账号绑定"
            //});

            //bg.button.Add(ThirdSubButton);
            ////var MenuResult = CommonApi.CreateMenu(accessToken, bg);
            //#endregion
            //Console.WriteLine("插入菜单执行结果："+MenuResult.errcode);
            //var GetMenuResult = CommonApi.GetMenu(accessToken);

            //Console.WriteLine("第一个按钮个数：" + (GetMenuResult.menu.button[0] as SubButton).sub_button.Count);
            //Console.ReadLine();
            //test
        }
    }
}
