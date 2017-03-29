using Commond.Caching;
using Entity;
using LanghuaNew.Data;
using Newtonsoft.Json;
using Simple.OData.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebGrease.Css.Extensions;

namespace LanghuaNew.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private ODataClient client = new ODataClient(ConfigurationManager.AppSettings["ServicePath"] + "odata/");
        private readonly ICacheManager _cacheManager;
        public HomeController()
        {
            this._cacheManager = new MemoryCacheManager();
        }
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public async Task<string> GetMyOrderData()
        {
            //我的订单
            string userName = User.Identity.Name;
            User user = await client.For<User>().Filter(u => u.UserName == userName).FindEntryAsync();
            WorkTableDisplayItem work = await client.For<WorkTableDisplayItem>().Filter(w => w.UserID == user.UserID).FindEntryAsync();
            //昨天、今天、明天、上月、本月、下月
            DateTimeOffset Pre = new DateTimeOffset(DateTime.Now.AddDays(-1).Date);
            DateTimeOffset Now = new DateTimeOffset(DateTime.Now.Date);
            DateTimeOffset Next = new DateTimeOffset(DateTime.Now.AddDays(1).Date);
            DateTimeOffset PreMonth = DateTimeOffset.Parse(DateTime.Now.ToString("yyyy-MM-01")).AddMonths(-1);
            DateTimeOffset ThisMonth = DateTimeOffset.Parse(DateTime.Now.ToString("yyyy-MM-01"));
            DateTimeOffset NextMonth = DateTimeOffset.Parse(DateTime.Now.ToString("yyyy-MM-01")).AddMonths(1);
            if (work == null)
            {
                work = new WorkTableDisplayItem();
                work.MyNotfilledCount = true;
                work.MyFilledCount = true;
                work.MyNoPayCount = true;
                work.MySencondFullCount = true;
                work.MyTodayOrderCount = true;
                work.MyTodaySales = true;
                work.MyTodayProfits = true;
            }
            int MyNotfilledCount = -1;
            int MyFilledCount = -1;
            int MyNoPayCount = -1;
            int MySencondFullCount = -1;
            int MyTodayOrderCount = -1;
            int MyYesterdayOrderCount = -1;
            float MyTodaySales = -1;
            float MyTodayTotalCost = -1;
            float MyTodayProfits = -1;
            float MyThisMonthSales = -1;
            float MyThisMonthTotalCost = -1;
            float MyThisMonthProfits = -1;
            if (work.MyNotfilledCount)
                MyNotfilledCount = await client.For<Order>().Filter(t => t.state == OrderState.Notfilled && t.CreateUserID == user.UserID).Count().FindScalarAsync<int>();
            if (work.MyFilledCount)
                MyFilledCount = await client.For<Order>().Filter(t => t.state == OrderState.Filled && t.CreateUserID == user.UserID).Count().FindScalarAsync<int>();
            if (work.MyNoPayCount)
                MyNoPayCount = await client.For<Order>().Filter(t => t.IsPay && t.CreateUserID == user.UserID).Count().FindScalarAsync<int>();
            if (work.MySencondFullCount)
                MySencondFullCount = await client.For<Order>().Filter(t => t.state == OrderState.SencondFull && t.CreateUserID == user.UserID).Count().FindScalarAsync<int>();
            if (work.MyTodayOrderCount)
            {

                MyTodayOrderCount = await client.For<Order>().Filter(t => t.CreateTime >= Now && t.CreateTime < Next && t.CreateUserID == user.UserID && t.state != OrderState.Invalid).Count().FindScalarAsync<int>();
                MyYesterdayOrderCount = await client.For<Order>().Filter(t => t.CreateTime >= Pre && t.CreateTime < Now && t.CreateUserID == user.UserID && t.state != OrderState.Invalid).Count().FindScalarAsync<int>();
            }
            if (work.MyTodaySales || work.MyTodayProfits)
            {
                var MyTodayOrders = await client.For<TBOrder>().Filter(t => t.Orders.Any(o => o.CreateTime >= Now && o.CreateTime < Next && o.CreateUserID == user.UserID && o.state != OrderState.Invalid)).Select(t => new { t.TotalCost, t.TotalReceive }).FindEntriesAsync();
                var MyThisMonthOrders = await client.For<TBOrder>().Filter(t => t.Orders.Any(o => o.CreateTime >= ThisMonth && o.CreateTime < NextMonth && o.CreateUserID == user.UserID && o.state != OrderState.Invalid)).Select(t => new { t.TotalCost, t.TotalReceive }).FindEntriesAsync();

                MyTodaySales = 0;
                MyThisMonthSales = 0;
                MyTodayOrders.ForEach(t => MyTodaySales += t.TotalReceive);
                MyThisMonthOrders.ForEach(t => MyThisMonthSales += t.TotalReceive);
                if (work.MyTodayProfits)
                {
                    MyTodayTotalCost = 0;
                    MyTodayProfits = 0;
                    MyThisMonthTotalCost = 0;
                    MyThisMonthProfits = 0;
                    MyTodayOrders.ForEach(t => MyTodayTotalCost += t.TotalCost);
                    MyTodayProfits = MyTodaySales - MyTodayTotalCost;
                    MyThisMonthOrders.ForEach(t => MyThisMonthTotalCost += t.TotalCost);
                    MyThisMonthProfits = MyThisMonthSales - MyThisMonthTotalCost;
                }
                if (!work.MyTodaySales)
                {
                    MyTodaySales = -1;
                    MyThisMonthSales = -1;
                }
            }

            var data = new
            {
                MyNotfilledCount,
                MyFilledCount,
                MyNoPayCount,
                MySencondFullCount,
                MyTodayOrderCount,
                MyYesterdayOrderCount,
                MyTodaySales,
                MyTodayProfits,
                MyThisMonthSales,
                MyThisMonthProfits
            };
            return JsonConvert.SerializeObject(new { ErrorCode = 200, ErrorMessage = "OK", data = data });
        }
        public async Task<string> GetAllOrderData()
        {
            //整店数据
            AllOrderDataModel all = new AllOrderDataModel();
            string cacheKey = ConstantConfig.HOME_ALLORDERDATA;
            if (!_cacheManager.IsSet(cacheKey))
            {
                //昨天、今天、明天、上月、本月、下月
                DateTimeOffset Pre = new DateTimeOffset(DateTime.Now.AddDays(-1).Date);
                DateTimeOffset Now = new DateTimeOffset(DateTime.Now.Date);
                DateTimeOffset Next = new DateTimeOffset(DateTime.Now.AddDays(1).Date);
                DateTimeOffset PreMonth = DateTimeOffset.Parse(DateTime.Now.ToString("yyyy-MM-01")).AddMonths(-1);
                DateTimeOffset ThisMonth = DateTimeOffset.Parse(DateTime.Now.ToString("yyyy-MM-01"));
                DateTimeOffset NextMonth = DateTimeOffset.Parse(DateTime.Now.ToString("yyyy-MM-01")).AddMonths(1);
                AllOrderDataModel model = new AllOrderDataModel();
                //订单数
                model.TodayOrderCount = await client.For<Order>().Filter(t => t.CreateTime >= Now && t.CreateTime < Next && t.state != OrderState.Invalid).Count().FindScalarAsync<int>();
                model.YesterdayOrderCount = await client.For<Order>().Filter(t => t.CreateTime >= Pre && t.CreateTime < Now && t.state != OrderState.Invalid).Count().FindScalarAsync<int>();
                //待检查
                model.OnCheckCount = await client.For<Order>().Filter(t => t.state == OrderState.Confirm || t.state == OrderState.Full || t.state == OrderState.Cancel).Count().FindScalarAsync<int>();
                //已核对
                model.CheckCount = await client.For<Order>().Filter(t => t.state == OrderState.Check).Count().FindScalarAsync<int>();
                //要售后
                model.NeedServiceCount = await client.For<Order>().Filter(t => t.IsNeedCustomerService).Count().FindScalarAsync<int>();
                //今日销售额/利润
                var TodayTBOrders = await client.For<TBOrder>().Filter(t => t.Orders.Any(o => o.CreateTime >= Now && o.CreateTime < Next && o.state != OrderState.Invalid)).Select(t => new { t.TotalCost, t.TotalReceive }).FindEntriesAsync();
                TodayTBOrders.ForEach(t => model.TodaySales += t.TotalReceive);
                float TodayTotalCost = 0;//今日成本
                TodayTBOrders.ForEach(t => TodayTotalCost += t.TotalCost);
                model.TodayProfits = model.TodaySales - TodayTotalCost;
                //日出行人数
                var TodayOrders = await client.For<Order>().Expand(t => t.ServiceItemHistorys).Filter(t => t.CreateTime >= Now && t.CreateTime < Next && t.state != OrderState.Invalid).Select(t => new { t.ServiceItemHistorys.AdultNum, t.ServiceItemHistorys.ChildNum, t.ServiceItemHistorys.INFNum }).FindEntriesAsync();
                var YesterdayOrders = await client.For<Order>().Expand(t => t.ServiceItemHistorys).Filter(t => t.CreateTime >= Pre && t.CreateTime < Now && t.state != OrderState.Invalid).Select(t => new { t.ServiceItemHistorys.AdultNum, t.ServiceItemHistorys.ChildNum, t.ServiceItemHistorys.INFNum }).FindEntriesAsync();
                TodayOrders.ForEach(t => model.TodayTravelNum += t.ServiceItemHistorys.AdultNum + t.ServiceItemHistorys.ChildNum + t.ServiceItemHistorys.INFNum);
                YesterdayOrders.ForEach(t => model.YesterdayTravelNum += t.ServiceItemHistorys.AdultNum + t.ServiceItemHistorys.ChildNum + t.ServiceItemHistorys.INFNum);
                if (!_cacheManager.IsSet(cacheKey + "-Month"))
                {
                    //本月销售额/利润
                    var ThisMonthTBOrders = await client.For<TBOrder>().Filter(t => t.Orders.Any(o => o.CreateTime >= ThisMonth && o.CreateTime < NextMonth && o.state != OrderState.Invalid)).Select(t => new { t.TotalCost, t.TotalReceive }).FindEntriesAsync();
                    ThisMonthTBOrders.ForEach(t => model.ThisMonthSales += t.TotalReceive);
                    float ThisMonthTotalCost = 0;//本月成本
                    ThisMonthTBOrders.ForEach(t => ThisMonthTotalCost += t.TotalCost);
                    model.ThisMonthProfits = model.ThisMonthSales - ThisMonthTotalCost;
                    //月出行人数
                    var ThisMonthOrders = await client.For<Order>().Expand(t => t.ServiceItemHistorys).Filter(t => t.CreateTime >= ThisMonth && t.CreateTime < NextMonth && t.state != OrderState.Invalid).Select(t => new { t.ServiceItemHistorys.AdultNum, t.ServiceItemHistorys.ChildNum, t.ServiceItemHistorys.INFNum }).FindEntriesAsync();
                    var PreMonthOrders = await client.For<Order>().Expand(t => t.ServiceItemHistorys).Filter(t => t.CreateTime >= PreMonth && t.CreateTime < ThisMonth && t.state != OrderState.Invalid).Select(t => new { t.ServiceItemHistorys.AdultNum, t.ServiceItemHistorys.ChildNum, t.ServiceItemHistorys.INFNum }).FindEntriesAsync();
                    ThisMonthOrders.ForEach(t => model.ThisMonthTravelNum += t.ServiceItemHistorys.AdultNum + t.ServiceItemHistorys.ChildNum + t.ServiceItemHistorys.INFNum);
                    PreMonthOrders.ForEach(t => model.PreMonthTravelNum += t.ServiceItemHistorys.AdultNum + t.ServiceItemHistorys.ChildNum + t.ServiceItemHistorys.INFNum);
                    _cacheManager.Set(cacheKey + "-Month", model, 1440);
                }
                else
                {
                    AllOrderDataModel Month = _cacheManager.Get<AllOrderDataModel>(cacheKey + "-Month");
                    model.ThisMonthSales = Month.ThisMonthSales;
                    model.ThisMonthProfits = Month.ThisMonthProfits;
                    model.ThisMonthTravelNum = Month.ThisMonthTravelNum;
                    model.PreMonthTravelNum = Month.PreMonthTravelNum;
                }
                //微信绑定
                model.WeixinBindCount = await client.For<Customer>().Filter(c => c.OpenID != null && c.OpenID != "").Count().FindScalarAsync<int>();
                int CustomerCount = await client.For<Customer>().Count().FindScalarAsync<int>();
                if (CustomerCount > 0 && model.WeixinBindCount > 0)
                {
                    double percent = Convert.ToDouble(model.WeixinBindCount) / Convert.ToDouble(CustomerCount);
                    model.WeixinBindRate = percent.ToString("0%");
                }
                else
                {
                    model.WeixinBindRate = "0%";
                }
                model.RefreshTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                _cacheManager.Set(cacheKey, model, 70);
                all = JsonConvert.DeserializeObject<AllOrderDataModel>(JsonConvert.SerializeObject(model));
            }
            else
            {
                AllOrderDataModel model = _cacheManager.Get<AllOrderDataModel>(cacheKey);
                all = JsonConvert.DeserializeObject<AllOrderDataModel>(JsonConvert.SerializeObject(model));
            }
            //配置项
            string userName = User.Identity.Name;
            User user = await client.For<User>().Filter(u => u.UserName == userName).FindEntryAsync();
            WorkTableDisplayItem work = await client.For<WorkTableDisplayItem>().Filter(w => w.UserID == user.UserID).FindEntryAsync();
            if (work != null)
            {
                all.TodayOrderCount = work.TodayOrderCount ? all.TodayOrderCount : -1;
                all.YesterdayOrderCount = work.TodayOrderCount ? all.YesterdayOrderCount : -1;
                all.OnCheckCount = work.OnCheckCount ? all.OnCheckCount : -1;
                all.CheckCount = work.CheckCount ? all.CheckCount : -1;
                all.NeedServiceCount = work.NeedServiceCount ? all.NeedServiceCount : -1;
                all.TodaySales = work.TodaySales ? all.TodaySales : -1;
                all.TodayProfits = work.TodayProfits ? all.TodayProfits : -1;
                all.ThisMonthSales = work.TodaySales ? all.ThisMonthSales : -1;
                all.ThisMonthProfits = work.TodayProfits ? all.ThisMonthProfits : -1;
                all.TodayTravelNum = work.TodayTravelNum ? all.TodayTravelNum : -1;
                all.YesterdayTravelNum = work.TodayTravelNum ? all.YesterdayTravelNum : -1;
                all.ThisMonthTravelNum = work.ThisMonthTravelNum ? all.ThisMonthTravelNum : -1;
                all.PreMonthTravelNum = work.ThisMonthTravelNum ? all.PreMonthTravelNum : -1;
                all.WeixinBindCount = work.WeixinBindCount ? all.WeixinBindCount : -1;
                all.WeixinBindRate = work.WeixinBindCount ? all.WeixinBindRate : "-1";
            }
            return JsonConvert.SerializeObject(new { ErrorCode = 200, ErrorMessage = "OK", data = all });
        }
        public async Task<string> GetMyWorkTableSetting()
        {
            string userName = User.Identity.Name;
            User user = await client.For<User>().Filter(u => u.UserName == userName).FindEntryAsync();
            WorkTableDisplayItem oldwork = await client.For<WorkTableDisplayItem>().Filter(w => w.UserID == user.UserID).FindEntryAsync();
            if (oldwork != null)
            {
                var data = new
                {
                    oldwork.MyNotfilledCount,
                    oldwork.MyFilledCount,
                    oldwork.MyNoPayCount,
                    oldwork.MySencondFullCount,
                    oldwork.MyTodayOrderCount,
                    oldwork.MyTodaySales,
                    oldwork.MyTodayProfits,
                };
                return JsonConvert.SerializeObject(new { ErrorCode = 200, ErrorMessage = "OK", data = data });
            }
            else
            {
                var data = new
                {
                    MyNotfilledCount = true,
                    MyFilledCount = true,
                    MyNoPayCount = true,
                    MySencondFullCount = true,
                    MyTodayOrderCount = true,
                    MyTodaySales = true,
                    MyTodayProfits = true,
                };
                return JsonConvert.SerializeObject(new { ErrorCode = 200, ErrorMessage = "OK", data = data });
            }
        }
        public async Task<string> GetAllWorkTableSetting()
        {
            string userName = User.Identity.Name;
            User user = await client.For<User>().Filter(u => u.UserName == userName).FindEntryAsync();
            WorkTableDisplayItem oldwork = await client.For<WorkTableDisplayItem>().Filter(w => w.UserID == user.UserID).FindEntryAsync();
            if (oldwork != null)
            {
                var data = new
                {
                    oldwork.TodayOrderCount,
                    oldwork.OnCheckCount,
                    oldwork.CheckCount,
                    oldwork.NeedServiceCount,
                    oldwork.TodaySales,
                    oldwork.TodayProfits,
                    oldwork.TodayTravelNum,
                    oldwork.ThisMonthTravelNum,
                    oldwork.WeixinBindCount,
                };
                return JsonConvert.SerializeObject(new { ErrorCode = 200, ErrorMessage = "OK", data = data });
            }
            else
            {
                var data = new
                {
                    TodayOrderCount = true,
                    OnCheckCount = true,
                    CheckCount = true,
                    NeedServiceCount = true,
                    TodaySales = true,
                    TodayProfits = true,
                    TodayTravelNum = true,
                    ThisMonthTravelNum = true,
                    WeixinBindCount = true,
                };
                return JsonConvert.SerializeObject(new { ErrorCode = 200, ErrorMessage = "OK", data = data });
            }
        }
        [HttpPost]
        public async Task<string> SaveMyWorkTableSetting(WorkTableDisplayItem work)
        {
            string userName = User.Identity.Name;
            User user = await client.For<User>().Filter(u => u.UserName == userName).FindEntryAsync();
            WorkTableDisplayItem oldwork = await client.For<WorkTableDisplayItem>().Filter(w => w.UserID == user.UserID).FindEntryAsync();
            if (oldwork == null)
            {
                work.UserID = user.UserID;
                work.TodayOrderCount = true;
                work.CheckCount = true;
                work.OnCheckCount = true;
                work.NeedServiceCount = true;
                work.TodayProfits = true;
                work.TodaySales = true;
                work.TodayTravelNum = true;
                work.ThisMonthTravelNum = true;
                work.WeixinBindCount = true;
                await client.For<WorkTableDisplayItem>().Set(work).InsertEntryAsync();
            }
            else
            {
                oldwork.MyNotfilledCount = work.MyNotfilledCount;
                oldwork.MyFilledCount = work.MyFilledCount;
                oldwork.MyNoPayCount = work.MyNoPayCount;
                oldwork.MySencondFullCount = work.MySencondFullCount;
                oldwork.MyTodayOrderCount = work.MyTodayOrderCount;
                oldwork.MyTodaySales = work.MyTodaySales;
                oldwork.MyTodayProfits = work.MyTodayProfits;
                await client.For<WorkTableDisplayItem>().Key(oldwork.WorkTableDisplayItemID).Set(oldwork).UpdateEntryAsync();
            }

            return JsonConvert.SerializeObject(new { ErrorCode = 200, ErrorMessage = "OK" });
        }
        [HttpPost]
        public async Task<string> SaveAllWorkTableSetting(WorkTableDisplayItem work)
        {
            string userName = User.Identity.Name;
            User user = await client.For<User>().Filter(u => u.UserName == userName).FindEntryAsync();
            WorkTableDisplayItem oldwork = await client.For<WorkTableDisplayItem>().Filter(w => w.UserID == user.UserID).FindEntryAsync();
            if (oldwork == null)
            {
                work.UserID = user.UserID;
                work.MyNotfilledCount = true;
                work.MyFilledCount = true;
                work.MyNoPayCount = true;
                work.MySencondFullCount = true;
                work.MyTodayOrderCount = true;
                work.MyTodaySales = true;
                work.MyTodayProfits = true;
                await client.For<WorkTableDisplayItem>().Set(work).InsertEntryAsync();
            }
            else
            {
                oldwork.TodayOrderCount = work.TodayOrderCount;
                oldwork.CheckCount = work.CheckCount;
                oldwork.OnCheckCount = work.OnCheckCount;
                oldwork.NeedServiceCount = work.NeedServiceCount;
                oldwork.TodayProfits = work.TodayProfits;
                oldwork.TodaySales = work.TodaySales;
                oldwork.TodayTravelNum = work.TodayTravelNum;
                oldwork.ThisMonthTravelNum = work.ThisMonthTravelNum;
                oldwork.WeixinBindCount = work.WeixinBindCount;
                await client.For<WorkTableDisplayItem>().Key(oldwork.WorkTableDisplayItemID).Set(oldwork).UpdateEntryAsync();
            }
            return JsonConvert.SerializeObject(new { ErrorCode = 200, ErrorMessage = "OK" });
        }        
    }
}