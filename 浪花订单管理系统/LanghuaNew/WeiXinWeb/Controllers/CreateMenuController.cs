using Commond;
using Newtonsoft.Json;
using Senparc.Weixin.MP;
using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin.MP.CommonAPIs;
using Senparc.Weixin.MP.Entities.Menu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Configuration;
using System.Web.Http;
using WXData;

namespace WeiXinWeb.Controllers
{
    public class CreateMenuController : ApiController
    {
        public static string WeixinAppId = AccessTokenContainer.GetFirstOrDefaultAppId();
        public static string ForCusPath = System.Configuration.ConfigurationManager.AppSettings["ForCusPath"];
        // GET: api/CreateMenu
        public HttpResponseMessage Get(int ReadCount)
        {
            int i = 0;
            //获取素材总数
            int NewsCount = MediaApi.GetMediaCount(WeixinAppId).news_count;
            //素材库中的数据是按创建时间降序排列
            int NeedReadCount = NewsCount - ReadCount;
            List<LHNew> News = new List<LHNew>();
            if (NeedReadCount > 0)
            {
                //分页读取
                while (true)
                {
                    var count = NeedReadCount - i < 20 ? NeedReadCount - i : 20;
                    var result = MediaApi.GetNewsMediaList(WeixinAppId, i, count);
                    foreach (var item in result.item)
                    {
                        LHNew lhnew = new LHNew();
                        lhnew.media_id = item.media_id;
                        //微信接口返回的时间是long类型，算法是1970年1月1日到对应日期的秒数
                        DateTime start = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                        //需要把秒数转换成毫秒
                        lhnew.update_time = start.AddMilliseconds(item.update_time * 1000).ToLocalTime();

                        lhnew.Articles = new List<LHArticle>();
                        foreach (var contentitem in item.content.news_item)
                        {
                            LHArticle art = new LHArticle();
                            art.Description = contentitem.digest;
                            art.PicUrl = contentitem.thumb_url;
                            art.Title = contentitem.title;
                            art.Url = contentitem.url;
                            lhnew.Articles.Add(art);
                        }
                        News.Add(lhnew);
                    }
                    //说明素材库有变动，重新获取
                    if (result.total_count != NewsCount)
                    {
                        News.Clear();
                        i = -20;
                    }
                    i = i + 20;
                    if (i >= NeedReadCount)
                    {
                        break;
                    }
                }
                News.Reverse();
            }
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(News), System.Text.Encoding.UTF8, "text/plain")
            };
        }
        // Post: api/CreateMenu
        public HttpResponseMessage Post([FromBody]string value)
        {
            List<WeiXinMenu> Menus = JsonConvert.DeserializeObject<List<WeiXinMenu>>(value);
            ButtonGroup bg = new ButtonGroup();
            foreach (WeiXinMenu Menu in Menus)
            {
                if (!string.IsNullOrEmpty(Menu.name))
                {
                    var SubButtonObj = new SubButton()
                    {
                        name = Menu.name
                    };
                    foreach (MenuItem item in Menu.Items)
                    {
                        if (!string.IsNullOrEmpty(item.name))
                        {
                            if (item.ItemType == MenuType.ViewMenu)
                            {
                                string InfoUrl = item.value;
                                if (!string.IsNullOrEmpty(item.value) && item.value.Contains(ForCusPath))
                                {
                                    InfoUrl = OAuthApi.GetAuthorizeUrl(WeixinAppId, item.value, "User", OAuthScope.snsapi_base);
                                }
                                SubButtonObj.sub_button.Add(new SingleViewButton()
                                {
                                    url = InfoUrl,
                                    name = item.name
                                });

                            }
                            else
                            {
                                SubButtonObj.sub_button.Add(new SingleClickButton()
                                {
                                    key = item.MenuItemID.ToString(),
                                    name = item.name
                                });

                            }
                        }
                    }

                    bg.button.Add(SubButtonObj);
                }
            }
            var MenuResult = CommonApi.CreateMenu(WeixinAppId, bg);
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject((int)MenuResult.errcode), System.Text.Encoding.UTF8, "text/plain")
            };
        }
    }
}
