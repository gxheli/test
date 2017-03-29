using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using LanghuaNew.Data;
using Simple.OData.Client;
using System.Configuration;
using Newtonsoft.Json;
using Entity;
using Commond.Caching;
using Commond;

namespace LanghuaNew.Controllers
{
    public class SalesStatisticsController : Controller
    {
        private ODataClient client = new ODataClient(ConfigurationManager.AppSettings["ServicePath"] + "odata/");
        private readonly ICacheManager _cacheManager;
        public SalesStatisticsController()
        {
            this._cacheManager = new MemoryCacheManager();
        }
        // GET: SalesStatistics
        public async Task<ActionResult> Index()
        {
            bool isSave = false;
            string userName = User.Identity.Name;
            User user = await client.For<User>().Expand("UserRole/MenuRights").Filter(u => u.UserName == userName).FindEntryAsync();
            if (user.UserRole != null)
            {
                foreach (var item in user.UserRole.Where(s => s.RoleEnableState == EnableState.Enable))
                {
                    if (item.RoleID == 1)
                    {
                        isSave = true;
                        break;
                    }
                    if (item.MenuRights != null)
                    {
                        foreach (var MenuRight in item.MenuRights)
                        {
                            var MenuResult = await client.For<MenuRight>().Expand(s => s.RoleRights).Key(MenuRight.MenuRightID).FindEntryAsync();
                            foreach (var rights in MenuResult.RoleRights)
                            {
                                if (rights.ControllerName == "SalesStatistics" && rights.ActionName == "SetSalesStatistics")
                                {
                                    isSave = true;
                                    break;
                                }
                            }
                            if (isSave) break;
                        }
                        if (isSave) break;
                    }
                }
            }
            ViewBag.isSave = isSave;
            var salesStatistics = client.For<SalesStatistic>().Expand(s => s.serviceItem).Expand(s => s.supplier);
            return View(await salesStatistics.FindEntriesAsync());
        }
        public async Task<string> GetSalesStatistics(ShareSearchModel share)
        {
            if (share.SalesStatisticSearch == null)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "查询条件不能为空！" });
            }
            if (share.SalesStatisticSearch.SalesStatisticID == 0)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "产品不能为空！" });
            }
            try
            {
                DateTimeOffset.Parse(share.SalesStatisticSearch.BeginDate);
                DateTimeOffset.Parse(share.SalesStatisticSearch.EndDate);
            }
            catch
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "时间格式不正确！" });
            }
            string cacheKey = string.Format(ConstantConfig.SALESSTATISTICS, share.SalesStatisticSearch.SalesStatisticID, share.SalesStatisticSearch.BeginDate, share.SalesStatisticSearch.EndDate);
            IEnumerable<SalesStatisticModel> sales;
            if (!_cacheManager.IsSet(cacheKey))
            {
                var Message = await HttpHelper.PostAction("SalesStatisticsExtend", JsonConvert.SerializeObject(share));
                sales = JsonConvert.DeserializeObject<IEnumerable<SalesStatisticModel>>(Message.Content.ReadAsStringAsync().Result);
                _cacheManager.Set(cacheKey, sales, 240);
            }
            else
            {
                sales = _cacheManager.Get<IEnumerable<SalesStatisticModel>>(cacheKey);
            }
            int draw = 1;
            int start = 0;
            int length = 50;
            if (share.length > 0)
            {
                draw = share.draw;
                start = share.start;
                length = share.length;
            }
            return JsonConvert.SerializeObject(new { draw = draw, recordsFiltered = sales.Count(), UpdateTime = _cacheManager.GetLastCacheTime(cacheKey), data = sales.Skip(start).Take(length), SearchModel = share });
        }
        //设置统计产品
        [HttpPost]
        public async Task<string> SetSalesStatistics(List<SalesStatistic> sales)
        {
            bool isSave = false;
            string userName = User.Identity.Name;
            User user = await client.For<User>().Expand("UserRole/MenuRights").Filter(u => u.UserName == userName).FindEntryAsync();
            if (user.UserRole != null)
            {
                foreach (var item in user.UserRole.Where(s => s.RoleEnableState == EnableState.Enable))
                {
                    if (item.RoleID == 1)
                    {
                        isSave = true;
                        break;
                    }
                    if (item.MenuRights != null)
                    {
                        foreach (var MenuRight in item.MenuRights)
                        {
                            var MenuResult = await client.For<MenuRight>().Expand(s => s.RoleRights).Key(MenuRight.MenuRightID).FindEntryAsync();
                            foreach (var rights in MenuResult.RoleRights)
                            {
                                if (rights.ControllerName == "SalesStatistics" && rights.ActionName == "SetSalesStatistics")
                                {
                                    isSave = true;
                                    break;
                                }
                            }
                            if (isSave) break;
                        }
                        if (isSave) break;
                    }
                }
            }
            if (!isSave)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "对不起，您没有权限设置统计产品" });
            }
            var Message = await HttpHelper.PutAction("SalesStatisticsExtend", JsonConvert.SerializeObject(sales));
            string exMessage = Message.Content.ReadAsStringAsync().Result;
            if (exMessage != "OK")
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 400, ErrorMessage = exMessage });
            }

            return JsonConvert.SerializeObject(new { ErrorCode = 200, ErrorMessage = "OK" });
        }
        public ActionResult StatisticsOrderPrice()
        {
            return View();
        }
        public async Task<string> GetStatisticsOrderPrice(string StartDate, string EndDate)
        {
            try
            {
                DateTime.Parse(StartDate);
                DateTime.Parse(EndDate);
            }
            catch
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "日期格式错误" });
            }
            string Message = await HttpHelper.GetActionForOdata("api/SalesStatisticsExtend/?StartDate=" + StartDate + "&EndDate=" + EndDate);
            List<StatisticsOrderPriceModel> prices = JsonConvert.DeserializeObject<List<StatisticsOrderPriceModel>>(Message);
            return JsonConvert.SerializeObject(new { ErrorCode = 200, ErrorMessage = "OK", prices });
        }
    }
}
