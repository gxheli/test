using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Commond;
using Simple.OData.Client;
using System.Configuration;
using System.Threading.Tasks;
using WXData;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net;

namespace LanghuaNew.Controllers
{
    public class WeiXinMenuController : Controller
    {
        private ODataClient client = new ODataClient(ConfigurationManager.AppSettings["ServicePath"] + "odata/");
        // GET: WeiXinMenu
        public async Task<ActionResult> Index()
        {
   
            var Menus = await client.For<WeiXinMenu>().Expand(p=>p.Items).FindEntriesAsync();
            List<WeiXinMenu> MenuList = new List<WeiXinMenu>();
            MenuList.AddRange(Menus);
            if (MenuList.Count == 0)
            {
                for(int i = 1; i < 4; i++)
                {
                    WeiXinMenu MenuObj = new WeiXinMenu();
                    MenuObj.name = "菜单" + i;
                    MenuObj.Items = new List<MenuItem>();
                    for (int j = 1; j < 6; j++)
                    {
                        MenuItem Item = new MenuItem();
                        Item.name = "";
                        Item.ItemType = MenuType.TextMenu;
                        Item.Text = "";
                        Item.value = "";
                        MenuObj.Items.Add(Item);
                    }
                    MenuList.Add(MenuObj);      
                }
              
            }
            return View(MenuList);
        }
        //更新素材库
        [HttpGet]
        public async Task<string> UpdateNews()
        {
            //获取已获取素材的数量
            int Count = await client.For<LHNew>().Count().FindScalarAsync<int>();
            var News = WeiXinHelper.GetNews(Count);
        
            //为了避免长度超过限制，分步更新
            int skip = 0;
            int take = 10;
            while (skip<News.Count)
            {
                await HttpHelper.PostAction("LHNewsExtend", JsonConvert.SerializeObject(News.Skip(skip).Take(take).ToList()));
                skip += take;
            }
            return  JsonConvert.SerializeObject(new { ErrorCode = 200, ErrorMessage = "OK"});
        }
        //关键字查询素材
        public async Task<string> GetNewsByKeyWord(string KeyWord)
        {
            var Result = await HttpHelper.GetAction("LHNewsExtend/GetNewsByKeyWord?KeyWord=" + KeyWord);
            return Result.Content.ReadAsStringAsync().Result;
        }
        //根据路径返回图片-反防盗链
        public async Task<FileContentResult> GetImage(string ImagePath)
        {
          
            var handler = new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.GZip };

            using (var http = new HttpClient(handler))
            {
                HttpResponseMessage Message = await http.GetAsync(ImagePath);
                var Result= Message.Content.ReadAsByteArrayAsync().Result;
                return File(Result, Message.Content.Headers.ContentType.ToString());
            }
         
        }
        //更新微信菜单
        [HttpPost]
        public async Task<string> CommitMenu(List<WeiXinMenu> Menus)
        {
           foreach(var Menu in Menus)
            {
                foreach(var item in Menu.Items)
                {
                    item.name = string.IsNullOrEmpty(item.name) ? "" : item.name;
                    item.Text = string.IsNullOrEmpty(item.Text) ? "" : item.Text;
                    item.value = string.IsNullOrEmpty(item.value) ? "" : item.value;
                }
            }
           
            await HttpHelper.PostAction("WeiXinMenusExtend", JsonConvert.SerializeObject(Menus));
            var NewMenus = await client.For<WeiXinMenu>().Expand(p => p.Items).FindEntriesAsync();
            WeiXinHelper.CreateMenu(NewMenus.ToList());
            return JsonConvert.SerializeObject(new { ErrorCode = 200, ErrorMessage = "OK" });
        }
    }
}
