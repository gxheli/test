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
using LanghuaNew.Models;
using System.Net.Http;
using Commond;
using WebGrease.Css.Extensions;
using Entity;
using Commond.Caching;

namespace LanghuaNew.Controllers
{
    public class SellControlsController : Controller
    {
        private ODataClient client = new ODataClient(ConfigurationManager.AppSettings["ServicePath"] + "odata/");

        private readonly ICacheManager _cacheManager;

        public SellControlsController()
        {
            this._cacheManager = new MemoryCacheManager();
        }

        // GET: SellControls
        public async Task<ActionResult> Index(int? id)
        {
            bool isSave = false;
            bool isSet = false;
            string userName = User.Identity.Name;
            User user = await client.For<User>().Expand("UserRole/MenuRights").Filter(u => u.UserName == userName).FindEntryAsync();
            if (user.UserRole != null)
            {
                foreach (var item in user.UserRole.Where(s => s.RoleEnableState == EnableState.Enable))
                {
                    if (item.RoleID == 1)
                    {
                        isSave = true;
                        isSet = true;
                        break;
                    }
                    if (item.MenuRights != null)
                    {
                        foreach (var MenuRight in item.MenuRights)
                        {
                            var MenuResult = await client.For<MenuRight>().Expand(s => s.RoleRights).Key(MenuRight.MenuRightID).FindEntryAsync();
                            foreach (var rights in MenuResult.RoleRights)
                            {
                                if (rights.ControllerName == "SellControls" && rights.ActionName == "SaveSellControl")
                                {
                                    isSave = true;
                                }
                                if (rights.ControllerName == "SellControls" && rights.ActionName == "SetSellControl")
                                {
                                    isSet = true;
                                }
                            }
                        }
                    }
                }
            }
            ViewBag.isSave = isSave;
            ViewBag.isSet = isSet;
            ViewBag.CountryID = id;

            var sellControls = await client.For<SellControl>()
                .Expand(s => s.Supplier.SupplierCountry)
                .Expand(s => s.FirstServiceItem)
                .Expand(s => s.SecondServiceItem)
                .Expand(s => s.sellControlClassify)
                .OrderBy(s => new { s.SellControlClassifyID, s.RowNum }).FindEntriesAsync();
            ViewBag.SellControlClassifies = await client.For<SellControlClassify>().OrderBy(p => p.OrderBy).FindEntriesAsync();
            ViewBag.SellControlClassifyID = id;
            return View(sellControls);
        }
        //配置控位产品
        [HttpPost]
        public async Task<string> SaveSellControl(List<SellControl> sell)
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
                                if (rights.ControllerName == "SellControls" && rights.ActionName == "SaveSellControl")
                                {
                                    isSave = true;
                                }
                            }
                        }
                    }
                }
            }
            if (!isSave)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "对不起，您没有权限配置控位产品" });
            }
            //if (sell.Count() > 15)
            //{
            //    return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "控位产品不能超过15个" });
            //}
            HttpResponseMessage Message = await HttpHelper.PostAction("SellControlsExtend", JsonConvert.SerializeObject(sell));
            string exMessage = Message.Content.ReadAsStringAsync().Result;
            if (exMessage != "OK")
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 400, ErrorMessage = exMessage });
            }

            return JsonConvert.SerializeObject(new { ErrorCode = 200, ErrorMessage = "OK" });
        }
        //设置产品
        [HttpPost]
        public async Task<string> SetSellControl(SellControl sell)
        {
            bool isSet = false;
            string userName = User.Identity.Name;
            User user = await client.For<User>().Expand("UserRole/MenuRights").Filter(u => u.UserName == userName).FindEntryAsync();
            if (user.UserRole != null)
            {
                foreach (var item in user.UserRole.Where(s => s.RoleEnableState == EnableState.Enable))
                {
                    if (item.RoleID == 1)
                    {
                        isSet = true;
                        break;
                    }
                    if (item.MenuRights != null)
                    {
                        foreach (var MenuRight in item.MenuRights)
                        {
                            var MenuResult = await client.For<MenuRight>().Expand(s => s.RoleRights).Key(MenuRight.MenuRightID).FindEntryAsync();
                            foreach (var rights in MenuResult.RoleRights)
                            {
                                if (rights.ControllerName == "SellControls" && rights.ActionName == "SetSellControl")
                                {
                                    isSet = true;
                                }
                            }
                        }
                    }
                }
            }
            if (!isSet)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "对不起，您没有权限设置" });
            }
            if (sell.SellControlID == 0)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "id不能为空" });
            }
            HttpResponseMessage Message = await HttpHelper.PutAction("SellControlsExtend", JsonConvert.SerializeObject(sell));
            string exMessage = Message.Content.ReadAsStringAsync().Result;
            if (exMessage != "OK")
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 400, ErrorMessage = exMessage });
            }
            await UpdateData(sell.SellControlID);
            //string cacheKey = string.Format(ConstantConfig.ORDER_SELLCONTROL_GETSELLCONTROL, sell.SellControlID.ToString());
            //_cacheManager.Remove(cacheKey);
            return JsonConvert.SerializeObject(new { ErrorCode = 200, ErrorMessage = "OK" });
        }
        //获取产品分类
        public async Task<string> GetSellControlClassities()
        {
            var data = await client.For<SellControlClassify>().OrderBy(p => p.OrderBy).FindEntriesAsync();
            return JsonConvert.SerializeObject(new { ErrorCode = 200, ErrorMessage = "OK", data });
        }
        //设置产品分类
        public async Task<string> SetSellControlClassities(List<SellControlClassify> list)
        {
            if (list != null && list.Count > 0)
            {
                var oldlist = await client.For<SellControlClassify>().Expand(p => p.sell).FindEntriesAsync();
                var AddList = list.ExceptItem(oldlist, p => p.SellControlClassifyID).ToList();
                var ModifList = list.ExceptItem(AddList, p => p.SellControlClassifyID).ToList();
                var DeleteList = oldlist.ExceptItem(ModifList, p => p.SellControlClassifyID).ToList();
                string Message = "";
                foreach (var item in AddList)
                {
                    await client.For<SellControlClassify>().Set(item).InsertEntryAsync();
                }
                foreach (var item in DeleteList)
                {
                    if (item.sell != null && item.sell.Count > 0)
                    {
                        Message += (Message == "" ? "" : "、") + item.ClassName;
                    }
                    else
                    {
                        await client.For<SellControlClassify>().Key(item.SellControlClassifyID).DeleteEntriesAsync();
                    }
                }
                foreach (var item in ModifList)
                {
                    await client.For<SellControlClassify>().Key(item.SellControlClassifyID).Set(item).UpdateEntryAsync();
                }
                var data = await client.For<SellControlClassify>().OrderBy(p => p.OrderBy).FindEntriesAsync();
                if (Message == "")
                {
                    return JsonConvert.SerializeObject(new { ErrorCode = 200, ErrorMessage = "OK", data });
                }
                else
                {
                    return JsonConvert.SerializeObject(new { ErrorCode = 201, ErrorMessage = Message, data });
                }
            }
            else
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "分类不能为空" });
            }
        }
        //控位销售表
        public async Task<string> GetSellControl(int? id)
        {
            if (id == null)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "id不能为空" });
            }
            //string cacheKey = string.Format(ConstantConfig.ORDER_SELLCONTROL_GETSELLCONTROL, id.ToString());
            //List<SellControlModel> sells;
            //if (!_cacheManager.IsSet(cacheKey))
            //{
            //    sells = await GetData(id);
            //    _cacheManager.Set(cacheKey, sells, 5);
            //}
            //else
            //{
            //    sells = _cacheManager.Get<List<SellControlModel>>(cacheKey);
            //}
            var sell = await client.For<SellControl>().Key(id).FindEntryAsync();
            if (sell.LastUpdateTime < DateTimeOffset.Now.AddMinutes(-6))
            {
                await UpdateData(id);
            }
            var shows = await GetData(id);
            return JsonConvert.SerializeObject(new { ErrorCode = 200, ErrorMessage = "OK", data = shows, LastUpdateTime = sell.LastUpdateTime.ToString("yyyy-MM-dd HH:mm:ss") });
        }

        public async Task<List<SellControlShow>> GetData(int? id)
        {
            //string Message = await HttpHelper.GetActionForOdata("api/SellControlsExtend/" + id);
            //List<SellControlModel> sells = JsonConvert.DeserializeObject<List<SellControlModel>>(Message);
            var shows = await client.For<SellControlShow>().Filter(s => s.SellControlID == id).FindEntriesAsync();
            return shows.ToList();
        }
        public async Task UpdateData(int? id)
        {
            await HttpHelper.GetActionForOdata("api/SellControlsExtend/" + id);
        }
        public async Task<string> GetStatisticsSellControl(int? id, string StartDate, string EndDate)
        {
            try
            {
                var start = DateTime.Parse(StartDate);
                var end = DateTime.Parse(EndDate);
                if (end < start)
                {
                    return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "结束日期不能小于开始日期" });
                }
                if ((end - start).Days > 100)
                {
                    return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "最多只能查询100天的数据，请缩小范围后再试！" });
                }
            }
            catch
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "日期格式不正确" });
            }
            try
            {
                var one = await client.For<SellControl>().Key(id).FindEntryAsync();
                if (one == null)
                {
                    return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "找不到该统计产品，请刷新后再试" });
                }
            }
            catch
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "找不到该统计产品，请刷新后再试" });
            }
            try
            {
                string Message = await HttpHelper.GetActionForOdata("api/SellControlShowsExtend/?id=" + id + "&StartDate=" + StartDate + "&EndDate=" + DateTime.Parse(EndDate).AddDays(1).ToString("yyyy-MM-dd"));
                List<SellControlModel> sells = JsonConvert.DeserializeObject<List<SellControlModel>>(Message);
                var ControlNum = sells.Sum(s => s.ControlNum);
                var TravelNum = sells.Sum(s => s.TravelNum);
                var PreTravelNum = sells.Sum(s => s.PreTravelNum);
                var DistributionNum = sells.Sum(s => s.DistributionNum);
                var SurplusNum = ControlNum - TravelNum - DistributionNum;
                var SellOutRate = "0%";
                if (ControlNum > 0 && ControlNum - SurplusNum > 0)
                {
                    double percent = Convert.ToDouble(ControlNum - SurplusNum) / Convert.ToDouble(ControlNum);
                    SellOutRate = percent.ToString("0%");
                }
                return JsonConvert.SerializeObject(new
                {
                    ErrorCode = 200,
                    ErrorMessage = "OK",
                    data = new { ControlNum, TravelNum, PreTravelNum, DistributionNum, SurplusNum, SellOutRate }
                });
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 400, ErrorMessage = ex.Message });
            }
        }


        //控位销售表
        //public async Task<string> GetSellControl(int? id)
        //{
        //    if (id == null)
        //    {
        //        return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "id不能为空" });
        //    }
        //    //获取设置
        //    SellControl sell = await client.For<SellControl>().Expand(s=>s.FirstServiceItem).Expand(s => s.SecondServiceItem).Key(id).FindEntryAsync();
        //    int SellControlNum = sell.SellControlNum;
        //    DateTimeOffset StartDate = sell.StartDate;
        //    int MonthNum = sell.MonthNum;
        //    int FirstServiceItemID = sell.FirstServiceItem.ServiceItemID;
        //    int SecondServiceItemID = sell.SecondServiceItem == null ? 0 : sell.SecondServiceItem.ServiceItemID;
        //    int SupplierID = sell.SupplierID;
        //    List<ServiceRule> rules = new List<ServiceRule>();
        //    //获取规则
        //    ServiceItem FirstItem = await client.For<ServiceItem>().Expand(s => s.ServiceRules).Key(FirstServiceItemID).FindEntryAsync();
        //    if (FirstItem.ServiceRules != null)
        //    {
        //        rules.AddRange(FirstItem.ServiceRules);
        //    }
        //    if (SecondServiceItemID > 0)
        //    {
        //        ServiceItem SecondItem = await client.For<ServiceItem>().Expand(s => s.ServiceRules).Key(FirstServiceItemID).FindEntryAsync();
        //        if (SecondItem.ServiceRules != null)
        //        {
        //            rules.AddRange(SecondItem.ServiceRules);
        //        }
        //    }

        //    List<SellControlModel> sells = new List<SellControlModel>();

        //    DateTimeOffset EndDate = StartDate.AddMonths(MonthNum);
        //    var timespan = EndDate - StartDate;
        //    if (StartDate > DateTime.Parse("1901-01-01"))
        //    {
        //        //遍历日期
        //        for (int i = 0; i < timespan.Days; i++)
        //        {
        //            var thisdate = StartDate.AddDays(i);
        //            var thisweek = thisdate.DayOfWeek.ToString("d");
        //            string thismonth = thisdate.Month < 10 ? "0" + thisdate.Month.ToString() : thisdate.Month.ToString();
        //            string thisday = thisdate.Day < 10 ? "0" + thisdate.Day.ToString() : thisdate.Day.ToString();

        //            int TravelNum = 0;
        //            int PreTravelNum = 0;
        //            int ReturnNum = 0;
        //            int PreReturnNum = 0;
        //            //主产品出行统计
        //            var items = await client.For<ServiceItemHistory>()
        //                .Expand(s => s.Order).Expand(s => s.Order.OrderHistorys)
        //                .Filter(s => s.SupplierID == SupplierID && s.ServiceItemID == FirstServiceItemID)
        //                .Filter(s => s.TravelDate == thisdate)
        //                .FindEntriesAsync();
        //            foreach (var item in items)
        //            {
        //                if (item.Order.state != OrderState.SencondCancel)
        //                {
        //                    //已确认过的订单算进确认人数
        //                    if (item.Order.OrderHistorys.Where(s => s.State == OrderState.SencondConfirm).FirstOrDefault() != null)
        //                    {
        //                        TravelNum += item.AdultNum + item.ChildNum + item.INFNum;
        //                    }
        //                    else//没有确认过的订单算进预扣人数
        //                    {
        //                        PreTravelNum += item.AdultNum + item.ChildNum + item.INFNum;
        //                    }
        //                }
        //            }

        //            if (SecondServiceItemID > 0)
        //            {
        //                //次产品出行统计
        //                var SecondItems = await client.For<ServiceItemHistory>()
        //                    .Expand(s => s.Order).Expand(s => s.Order.OrderHistorys)
        //                    .Filter(s => s.SupplierID == SupplierID && s.ServiceItemID == SecondServiceItemID)
        //                    .Filter(s => s.TravelDate == thisdate)
        //                    .FindEntriesAsync();
        //                foreach (var item in SecondItems)
        //                {
        //                    if (item.Order.state != OrderState.SencondCancel)
        //                    {
        //                        //已确认过的订单算进确认人数
        //                        if (item.Order.OrderHistorys.Where(s => s.State == OrderState.SencondConfirm).FirstOrDefault() != null)
        //                        {
        //                            TravelNum += item.AdultNum + item.ChildNum + item.INFNum;
        //                        }
        //                        else//没有确认过的订单算进预扣人数
        //                        {
        //                            PreTravelNum += item.AdultNum + item.ChildNum + item.INFNum;
        //                        }
        //                    }
        //                }

        //                //主、次产品返回统计
        //                var ReturnItems = await client.For<ServiceItemHistory>()
        //                    .Expand(s => s.Order).Expand(s => s.Order.OrderHistorys)
        //                    .Filter(s => s.SupplierID == SupplierID)
        //                    .Filter(s => s.ServiceItemID == FirstServiceItemID || s.ServiceItemID == SecondServiceItemID)
        //                    .Filter(s => s.ReturnDate == thisdate)
        //                    .FindEntriesAsync();
        //                foreach (var item in ReturnItems)
        //                {
        //                    if (item.Order.state != OrderState.SencondCancel)
        //                    {
        //                        //已确认过的订单算进确认人数
        //                        if (item.Order.OrderHistorys.Where(s => s.State == OrderState.SencondConfirm).FirstOrDefault() != null)
        //                        {
        //                            ReturnNum += item.AdultNum + item.ChildNum + item.INFNum;
        //                        }
        //                        else//没有确认过的订单算进预扣人数
        //                        {
        //                            PreReturnNum += item.AdultNum + item.ChildNum + item.INFNum;
        //                        }
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                ReturnNum = -1;
        //                PreReturnNum = -1;
        //            }
        //            //根据人数来生成不同颜色
        //            SellControlModelState state = SellControlModelState.While;
        //            if (TravelNum > SellControlNum)
        //            {
        //                state = SellControlModelState.Red;
        //            }
        //            else if (TravelNum + PreTravelNum > SellControlNum)
        //            {
        //                state = SellControlModelState.Yellow;
        //            }
        //            else if (TravelNum > 0)
        //            {
        //                state = SellControlModelState.Green;
        //            }
        //            SellControlModelState Returnstate = SellControlModelState.While;
        //            if (ReturnNum > SellControlNum)
        //            {
        //                Returnstate = SellControlModelState.Red;
        //            }
        //            else if (ReturnNum + PreReturnNum > SellControlNum)
        //            {
        //                Returnstate = SellControlModelState.Yellow;
        //            }
        //            else if (ReturnNum > 0)
        //            {
        //                Returnstate = SellControlModelState.Green;
        //            }
        //            //从出行和返回中取最大值
        //            state = state > Returnstate ? state : Returnstate;
        //            //根据规则来决定今天thisdate是否禁止
        //            foreach (var item in rules)
        //            {
        //                if (thisdate >= item.StartTime && thisdate <= item.EndTime)
        //                {
        //                    bool isAllow = item.RuleUseTypeValue == RuleUseType.Allow ? true : false;
        //                    switch (item.SelectRuleType)
        //                    {
        //                        case RuleType.ByDateRange:
        //                            if ((thisdate > item.RangeStart && thisdate < item.RangeEnd && !isAllow) || ((thisdate < item.RangeStart || thisdate > item.RangeEnd) && isAllow))
        //                            {
        //                                // 禁止在范围内 或者 只允许在范围内 
        //                                state = SellControlModelState.Gray;
        //                            }
        //                            break;
        //                        case RuleType.ByWeek:
        //                            if ((item.Week.Split('|').Contains(thisweek) && !isAllow) || (!item.Week.Split('|').Contains(thisweek) && isAllow))
        //                            {
        //                                // 禁止在星期几 或者 只允许在星期几 
        //                                state = SellControlModelState.Gray;
        //                            }
        //                            break;
        //                        case RuleType.ByDouble:
        //                            if ((((thisdate.Day % 2 == 0 && item.IsDouble) || (thisdate.Day % 2 != 0 && !item.IsDouble)) && !isAllow) || (((thisdate.Day % 2 != 0 && item.IsDouble) || (thisdate.Day % 2 == 0 && !item.IsDouble)) && isAllow))
        //                            {
        //                                // 禁止单、双 或者 只允许单、双 
        //                                state = SellControlModelState.Gray;
        //                            }
        //                            break;
        //                        case RuleType.ByDate:
        //                            if ((item.UseDate.Split('|').Contains(thisdate.Day.ToString()) && !isAllow) || (!item.UseDate.Split('|').Contains(thisdate.Day.ToString()) && isAllow))
        //                            {
        //                                // 禁止几号 或者 只允许几号
        //                                state = SellControlModelState.Gray;
        //                            }
        //                            break;
        //                    }
        //                }
        //            }
        //            SellControlModel sellControlModel = new SellControlModel();
        //            sellControlModel.date = thismonth + "-" + thisday;
        //            sellControlModel.thisdate = thisdate.ToString("yyyy-MM-dd");
        //            sellControlModel.TravelNum = TravelNum;
        //            sellControlModel.PreTravelNum = PreTravelNum;
        //            sellControlModel.ReturnNum = ReturnNum;
        //            sellControlModel.PreReturnNum = PreReturnNum;
        //            sellControlModel.state = state;
        //            sells.Add(sellControlModel);
        //        }
        //    }
        //    return JsonConvert.SerializeObject(new { ErrorCode = 200, ErrorMessage = "OK", data = sells });
        //}
    }
}
