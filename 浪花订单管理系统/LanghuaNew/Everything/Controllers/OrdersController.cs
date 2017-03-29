using Commond;
using LanghuaNew.Data;
using Newtonsoft.Json;
using Simple.OData.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Everything.Models;
using AutoMapper;

namespace Everything.Controllers
{
    public class OrdersController : Controller
    {
        private ODataClient client = new ODataClient(ConfigurationManager.AppSettings["ServicePath"] + "odata/");
        public ActionResult Edit()
        {
            string url = Request.Url.ToString();
            if (!url.Contains("localhost"))
            {
                return HttpNotFound();
            }
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> Edit(string OrderNo)
        {
            string url = Request.Url.ToString();
            if (!url.Contains("localhost"))
            {
                return HttpNotFound();
            }
            ViewBag.OrderNo = OrderNo;
            var order = await client.For<Order>()
                .Expand(x => x.ServiceItemHistorys.ExtraServiceHistorys)
                .Expand(u => u.Customers)
                .Filter(x => x.OrderNo == OrderNo)
                .FindEntryAsync();
            if (order == null)
            {
                ViewBag.Message = "单号不存在";
            }
            Dictionary<int, string> stateAll = new Dictionary<int, string>();
            foreach (OrderState item in Enum.GetValues(typeof(OrderState)))
            {
                if (item != OrderState.nullandvoid)
                {
                    string state = EnumHelper.GetEnumDescription(item).Substring(0, EnumHelper.GetEnumDescription(item).IndexOf("|"));
                    stateAll.Add((int)item, state);
                }
            }
            ViewBag.stateAll = stateAll;
            return View(order);
        }
        //修改附加项目数量
        [HttpPost]
        public async Task<string> UpdateExtraService(int? OrderID, List<ExtraServiceHistory> extraServiceHistorys)
        {
            string url = Request.Url.ToString();
            if (!url.Contains("localhost"))
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 400, ErrorMessage = "网址异常" });
            }
            if (OrderID == null)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "ID不能为空" });
            }
            try
            {
                //计算单个订单成本,不转换币别
                float orderCost = 0;
                int minNum = 0;
                int SelectNum = 0;
                foreach (var extraService in extraServiceHistorys)
                {
                    orderCost += extraService.ServicePrice * extraService.ServiceNum;
                    minNum += extraService.MinNum;
                    SelectNum += extraService.ServiceNum;
                }
                //if (minNum > 0 && SelectNum == 0)
                //{
                //    return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "请至少选中一项额外项目" });
                //}
                ServiceItemHistory itemHistory = await client.For<ServiceItemHistory>().Key(OrderID).FindEntryAsync();
                orderCost += itemHistory.PayType == PricingMethod.ByPerson
                    ? itemHistory.AdultNetPrice * itemHistory.AdultNum + itemHistory.ChildNetPrice * itemHistory.ChildNum + itemHistory.BobyNetPrice * itemHistory.INFNum
                    : itemHistory.Price * itemHistory.RoomNum * itemHistory.RightNum;
                itemHistory.TotalCost = orderCost;
                itemHistory.ExtraServiceHistorys = extraServiceHistorys;
                await HttpHelper.PostAction("ServiceItemHistoryExtend", JsonConvert.SerializeObject(itemHistory));
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 400, ErrorMessage = "异常：" + ex.Message });
            }
            return JsonConvert.SerializeObject(new { ErrorCode = 200, ErrorMessage = "OK" });
        }
        //修改项目人数
        [HttpPost]
        public async Task<string> UpdateItemNum(int? OrderID, int AdultNum, int ChildNum, int INFNum, int RoomNum, int RightNum)
        {
            string url = Request.Url.ToString();
            if (!url.Contains("localhost"))
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 400, ErrorMessage = "网址异常" });
            }
            if (OrderID == null)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "ID不能为空" });
            }
            try
            {
                ServiceItemHistory itemHistory = await client.For<ServiceItemHistory>().Expand(s => s.ExtraServiceHistorys).Key(OrderID).FindEntryAsync();
                //计算单个订单成本,不转换币别
                float orderCost = 0;
                if (itemHistory.ExtraServiceHistorys != null)
                {
                    foreach (var extraService in itemHistory.ExtraServiceHistorys)
                    {
                        orderCost += extraService.ServicePrice * extraService.ServiceNum;
                    }
                }
                orderCost += itemHistory.PayType == PricingMethod.ByPerson
                    ? itemHistory.AdultNetPrice * AdultNum + itemHistory.ChildNetPrice * ChildNum + itemHistory.BobyNetPrice * INFNum
                    : itemHistory.Price * RoomNum * RightNum;

                itemHistory.TotalCost = orderCost;
                itemHistory.AdultNum = AdultNum;
                itemHistory.ChildNum = ChildNum;
                itemHistory.INFNum = INFNum;
                itemHistory.RoomNum = RoomNum;
                itemHistory.RightNum = RightNum;
                await client.For<ServiceItemHistory>().Key(OrderID).Set(itemHistory).UpdateEntryAsync();
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 400, ErrorMessage = "异常：" + ex.Message });
            }
            return JsonConvert.SerializeObject(new { ErrorCode = 200, ErrorMessage = "OK" });
        }
        //查询额外项目
        public async Task<string> GetExtraServiceByID(int? ItemID, int? SupplierID)
        {
            string url = Request.Url.ToString();
            if (!url.Contains("localhost"))
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 400, ErrorMessage = "网址异常" });
            }
            if (ItemID == null || SupplierID == null)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "ID不能为空" });
            }
            ServiceItem item = await client.For<ServiceItem>().Expand(s => s.ExtraServices).Key(ItemID).FindEntryAsync();
            //额外服务价格
            SupplierServiceItem price = await client.For<SupplierServiceItem>().Expand(s => s.ExtraServicePrices).Filter(s => s.ServiceItemID == ItemID && s.SupplierID == SupplierID).FindEntryAsync();
            List<SelectExtraServiceView> list = new List<SelectExtraServiceView>();
            if (item.ExtraServices != null)
            {
                foreach (var r in item.ExtraServices)
                {
                    //额外服务
                    SelectExtraServiceView ExtraService = new SelectExtraServiceView();
                    var ExtraServiceViewconfig = new MapperConfiguration(cfg => cfg.CreateMap<ExtraService, SelectExtraServiceView>());
                    var ExtraServiceViewmapper = ExtraServiceViewconfig.CreateMapper();
                    ExtraService = ExtraServiceViewmapper.Map<SelectExtraServiceView>(r);
                    if (price != null && price.ExtraServicePrices != null)
                    {
                        //额外服务价格
                        foreach (var re in price.ExtraServicePrices)
                        {
                            if (r.ExtraServiceID == re.ExtraServiceID)
                            {
                                ExtraService.ServicePrice = re.ServicePrice;
                                break;
                            }
                        }
                    }
                    list.Add(ExtraService);
                }
            }
            return JsonConvert.SerializeObject(new { ErrorCode = 200, ErrorMessage = "OK", data = list });
        }
        //修改订单
        [HttpPost]
        public async Task<string> SaveOrder(Order order)
        {
            string url = Request.Url.ToString();
            if (!url.Contains("localhost"))
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 400, ErrorMessage = "网址异常" });
            }
            if (order.OrderID == 0)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "ID不能为空！" });
            }
            DateTimeOffset TravelDate = order.ServiceItemHistorys.TravelDate;
            DateTimeOffset ReturnDate = order.ServiceItemHistorys.ReturnDate;
            string ElementsValue = order.ServiceItemHistorys.ElementsValue;
            string ServiceItemTemplteValue = order.ServiceItemHistorys.ServiceItemTemplteValue;
            List<OrderTraveller> travellers = order.ServiceItemHistorys.travellers == null ? null : order.ServiceItemHistorys.travellers.Distinct().ToList();
            try
            {
                ServiceItemHistory itemHistory = await client.For<ServiceItemHistory>().Key(order.OrderID).FindEntryAsync();
                itemHistory.ElementsValue = ElementsValue;
                itemHistory.ServiceItemTemplteValue = ServiceItemTemplteValue;
                itemHistory.TravelDate = TravelDate;
                itemHistory.ReturnDate = ReturnDate;
                itemHistory.travellers = new List<OrderTraveller>();
                itemHistory.travellers = travellers;
                await HttpHelper.PutAction("ServiceItemHistoryExtend", JsonConvert.SerializeObject(itemHistory));
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 400, ErrorMessage = "异常：" + ex.Message });
            }
            return JsonConvert.SerializeObject(new { ErrorCode = 200, ErrorMessage = "OK" });
        }
        //修改订单状态
        [HttpPost]
        public async Task<string> UpdateState(int? OrderID, OrderState state)
        {
            string url = Request.Url.ToString();
            if (!url.Contains("localhost"))
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 400, ErrorMessage = "网址异常" });
            }
            if (OrderID == null)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "ID不能为空" });
            }
            try
            {
                Order order= await client.For<Order>().Key(OrderID).FindEntryAsync();
                order.state = state;
                await client.For<Order>().Key(OrderID).Set(order).UpdateEntryAsync();
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 400, ErrorMessage = "异常：" + ex.Message });
            }
            return JsonConvert.SerializeObject(new { ErrorCode = 200, ErrorMessage = "OK" });
        }
    }
}