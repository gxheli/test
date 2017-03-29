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
using Commond;
using System.Security.Cryptography;
using System.Text;
using System.Net.Http;
using WebGrease.Css.Extensions;
using LanghuaWapForCus.Models;

namespace LanghuaWapForCus.Controllers
{
    public class TBOrdersController : Controller
    {
        private ODataClient client = new ODataClient(ConfigurationManager.AppSettings["ServicePath"] + "odata/");
        //订单批量填报页面
        public async Task<ActionResult> OrderNew(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TBOrder tBOrder = await client.For<TBOrder>()
                .Key(id).FindEntryAsync();

            if (tBOrder == null)
            {
                return HttpNotFound();
            }
            string CustomerTBCode = User.Identity.Name;
            Customer customer = await client.For<Customer>()
                .Filter(u => u.CustomerTBCode == CustomerTBCode).FindEntryAsync();

            IEnumerable<Order> orders = await client.For<Order>()
                .Expand(u => u.ServiceItemHistorys)
                .Expand(u => u.ServiceItemHistorys.ExtraServiceHistorys)
                .Expand(u => u.Customers)
                //.Expand(u => u.Customers.Travellers)
                .Filter(u => u.TBOrders.TBOrderID == id && u.CustomerID == customer.CustomerID)
                .FindEntriesAsync();

            if (orders == null || orders.Count() == 0)
            {
                return RedirectToAction("Index", "Orders");
            }

            List<Order> items = new List<Order>();
            OrderState state = OrderState.Notfilled;
            foreach (var item in orders)
            {
                state = item.state > state ? item.state : state;
                if (string.IsNullOrEmpty(item.CustomerName) && string.IsNullOrEmpty(item.CustomerEnname) && string.IsNullOrEmpty(item.Tel) && string.IsNullOrEmpty(item.BakTel) && string.IsNullOrEmpty(item.Email) && string.IsNullOrEmpty(item.Wechat))
                {
                    item.CustomerName = item.Customers.CustomerName;
                    item.CustomerEnname = item.Customers.CustomerEnname;
                    item.Tel = item.Customers.Tel;
                    item.BakTel = item.Customers.BakTel;
                    item.Email = item.Customers.Email;
                    item.Wechat = item.Customers.Wechat;
                }
                items.Add(item);
            }
            if (state > OrderState.Notfilled)
            {
                return RedirectToAction("Index", "Orders");
            }
            tBOrder.Orders = items.OrderByDescending(s => s.CustomerName).ToList();
            ViewBag.Area = await client.For<Area>().FindEntriesAsync();

            return View(tBOrder);
        }
        //检查出行日期
        [HttpPost]
        public async Task<string> CheckTravelDate(List<int> OrderID, List<string> TravelDate)
        {
            if (OrderID == null || OrderID.Count == 0)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "ID不能为空" });
            }
            List<DateTimeOffset> date = new List<DateTimeOffset>();
            try
            {
                foreach (var item in TravelDate)
                {
                    date.Add(DateTimeOffset.Parse(item));
                }
            }
            catch
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "日期格式错误" });
            }
            var order = await client.For<Order>().Key(OrderID[0]).Select(s => s.CustomerID).FindEntryAsync();
            var result = client.For<Order>().Expand(s => s.ServiceItemHistorys)
                .Filter(s => s.CustomerID == order.CustomerID)
                .Filter(s => s.state != OrderState.Invalid && s.state != OrderState.SencondCancel);
            OrderID.ForEach(o => result = result.Filter(s => s.OrderID != o));
            var orders = await result.Select(s => new { s.ServiceItemHistorys.TravelDate, s.ServiceItemHistorys.ChangeTravelDate }).FindEntriesAsync();
            if (orders != null)
            {
                orders.ForEach(s => date.Add(s.ServiceItemHistorys.ChangeTravelDate > DateTimeOffset.MinValue ? s.ServiceItemHistorys.ChangeTravelDate : s.ServiceItemHistorys.TravelDate));
            }
            var valid = date.Where(s => s > DateTimeOffset.Now.AddMonths(-1));
            if (valid != null && valid.Count() > 0 && (valid.Max() - valid.Min()).Days > 15)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "您在系统中的最早出行日期 <span style='color:red'>" + valid.Min().ToString("yyyy-MM-dd") + "</span> 与最晚出行日期 <span style='color:red'>" + valid.Max().ToString("yyyy-MM-dd") + "</span> 相隔超过15天，请检查是否有误！确认无误需要保存请点【确认】，否则请点【取消】。" });
            }
            return JsonConvert.SerializeObject(new { ErrorCode = 200, ErrorMessage = "OK" });
        }
        //订单填报保存
        [HttpPost]
        public async Task<string> SaveTBOrder(TBOrderViewModel tBOrder)
        {
            if (tBOrder.TBOrderID == 0)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "找不到数据！" });
            }
            Customer customer = tBOrder.customer;
            if (customer == null)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "联系人信息不能为空！" });
            }
            if (customer.CustomerTBCode == null)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "数据异常！请刷新重试！" });
            }
            if (customer.CustomerTBCode.ToUpper() != User.Identity.Name.ToUpper())
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "账号异常！请重新登录！" });
            }
            bool IsCommit = tBOrder.IsCommit;
            List<OrderViewModel> orders = tBOrder.orders;
            if (orders == null)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "订单信息不能为空！" });
            }
            TBOrder tBOrderOld = await client.For<TBOrder>()
                .Filter(u => u.TBOrderID == tBOrder.TBOrderID & u.TBID == customer.CustomerTBCode)
                .Select(u => u.TBOrderID).FindEntryAsync();
            if (tBOrderOld == null)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "错误的淘宝ID！" });
            }
            if (IsCommit)
            {
                if (string.IsNullOrEmpty(customer.CustomerName) || string.IsNullOrEmpty(customer.CustomerEnname))
                {
                    return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "姓名不能为空！" });
                }
                if (string.IsNullOrEmpty(customer.Email))
                {
                    return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "邮箱不能为空！" });
                }
                if (string.IsNullOrEmpty(customer.Tel))
                {
                    return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "联系电话不能为空！" });
                }
                if (string.IsNullOrEmpty(customer.Wechat))
                {
                    return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "微信号不能为空！" });
                }
            }
            try
            {
                List<string> ssdate = new List<string>();
                foreach (var order in orders)
                {
                    Order item = await client.For<Order>().Expand(s => s.ServiceItemHistorys).Key(order.OrderID)
                        .Select(s => new
                        {
                            s.state,
                            s.CustomerID,
                            s.ServiceItemHistorys.ServiceTypeID,
                            s.ServiceItemHistorys.cnItemName,
                            s.ServiceItemHistorys.FixedDays,
                            s.ServiceItemHistorys.ServiceItemID,
                            s.ServiceItemHistorys.RightNum
                        }).FindEntryAsync();
                    //暂存按钮
                    if (!IsCommit && item.state != OrderState.Notfilled && item.state != OrderState.Invalid)
                    {
                        return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "已填报的订单不能暂存！" });
                    }
                    //保存按钮
                    if (IsCommit)
                    {
                        if (item.state == OrderState.Invalid)
                        {
                            return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "您填写的同时有产品被作废，请先暂存信息后咨询客服。" });
                        }
                        if (item.state != OrderState.Notfilled && item.state != OrderState.Filled)
                        {
                            return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "核对后不能修改！请联系客服！" });
                        }
                        DateTimeOffset TravelDate = order.TravelDate;
                        DateTimeOffset ReturnDate = order.ReturnDate;
                        ServiceItemHistory itemHistory = item.ServiceItemHistorys;

                        if (TravelDate.Date < DateTime.Now.Date)
                        {
                            if (itemHistory.ServiceTypeID == 4)
                            {
                                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = itemHistory.cnItemName + "<br/>入住日期不能早于今天！" });
                            }
                            else
                            {
                                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = itemHistory.cnItemName + "<br/>出行日期不能早于今天！" });
                            }
                        }
                        if (itemHistory.FixedDays > 0)
                        {
                            ReturnDate = TravelDate.AddDays(itemHistory.FixedDays - 1);
                        }
                        if (itemHistory.ServiceTypeID == 2)
                        {
                            if (ReturnDate < TravelDate)
                            {
                                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = itemHistory.cnItemName + "<br/>返回日期不能为空且不能早于出行日期！" });
                            }
                        }
                        if (itemHistory.ServiceTypeID == 4)
                        {
                            if ((ReturnDate.Date - TravelDate.Date).TotalDays != itemHistory.RightNum)
                            {
                                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = itemHistory.cnItemName + "<br/>入住" + itemHistory.RightNum + "晚，您选择的日期有误！" });
                            }
                        }
                        if (await CheckRule(itemHistory.ServiceItemID, TravelDate))
                        {
                            return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = itemHistory.cnItemName + "<br/>出行日期不在规则允许范围内！" });
                        }
                        if (itemHistory.ServiceTypeID == 2)
                        {
                            if (ssdate.Contains(TravelDate.ToString("yyyy-MM-dd")))
                            {
                                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "当前订单中 " + TravelDate.ToString("yyyy-MM-dd") + " 有多个行程同一天出行，请修改日期后再保存，或暂存信息后咨询客服。" });
                            }
                            ssdate.Add(TravelDate.ToString("yyyy-MM-dd"));

                            Order ssorder = await client.For<Order>()
                                .Filter(s => s.ServiceItemHistorys.ServiceTypeID == 2)//行程
                                .Filter(s => s.CustomerID == item.CustomerID)
                                .Filter(s => s.OrderID != order.OrderID)
                                .Filter(s => s.state != OrderState.Invalid && s.state != OrderState.SencondCancel && s.state != OrderState.CancelReceive && s.state != OrderState.RequestCancel && s.state != OrderState.Cancel && s.state != OrderState.Notfilled)
                                //.Filter(s => s.CustomerName == customer.CustomerName || s.Tel == customer.Tel)
                                .Filter(s => s.ServiceItemHistorys.TravelDate == TravelDate || s.ServiceItemHistorys.ChangeTravelDate == TravelDate)
                                .Select(s => new { s.OrderID, s.CustomerName, s.Tel }).FindEntryAsync();
                            if (ssorder != null)
                            {
                                if (ssorder.CustomerName == customer.CustomerName)
                                {
                                    return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "系统检测到 " + item.CustomerName + " 在 " + order.TravelDate.ToString("yyyy-MM-dd") + " 有其它行程安排！请修改出行日期或预订人姓名、电话号码后再保存，也可先" + (item.state == OrderState.Notfilled ? "暂存信息后" : "") + "咨询客服。" });
                                }
                                if (ssorder.Tel == customer.Tel)
                                {
                                    return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "系统检测到 " + item.Tel + "  这个号码出现在 " + order.TravelDate.ToString("yyyy-MM-dd") + " 的其它行程中！请修改出行日期或更换电话号码后再保存，也可先" + (item.state == OrderState.Notfilled ? "暂存信息后" : "") + "咨询客服。" });
                                }
                            }
                            Order ccorder = await client.For<Order>()
                                .Filter(s => s.ServiceItemHistorys.ServiceTypeID == 2)//行程
                                .Filter(s => s.ServiceItemHistorys.ServiceItemID == item.ServiceItemHistorys.ServiceItemID)//同一行程
                                .Filter(s => s.ServiceItemHistorys.SupplierID == item.ServiceItemHistorys.SupplierID)
                                .Filter(s => s.state == OrderState.SencondCancel || s.state == OrderState.CancelReceive || s.state == OrderState.RequestCancel || s.state == OrderState.Cancel)
                                .Filter(s => s.CustomerID == item.CustomerID)
                                .Filter(s => s.ServiceItemHistorys.TravelDate == item.ServiceItemHistorys.TravelDate || s.ServiceItemHistorys.ChangeTravelDate == item.ServiceItemHistorys.TravelDate)
                                .Filter(s => s.CustomerName == customer.CustomerName)
                                .Select(s => s.OrderID).FindEntryAsync();
                            if (ccorder != null)
                            {
                                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "系统检测到 " + customer.CustomerName + " 在 " + item.ServiceItemHistorys.TravelDate.ToString("yyyy-MM-dd") + " 的同一行程曾经被取消！请修改姓名后再保存，也可先" + (item.state == OrderState.Notfilled ? "暂存信息后" : "") + "咨询客服。" });
                            }
                        }
                    }
                }
                //保存订单信息
                foreach (var order in orders)
                {
                    Order item = await client.For<Order>().Expand(s => s.ServiceItemHistorys).Key(order.OrderID).FindEntryAsync();
                    //string Elements = order.Elements;
                    string ElementsValue = order.ElementsValue;
                    string ServiceItemTemplteValue = order.ServiceItemTemplteValue;
                    DateTimeOffset TravelDate = order.TravelDate;
                    DateTimeOffset ReturnDate = order.ReturnDate;

                    //获取出行旅客
                    List<OrderTraveller> trs = order.travellers == null ? null : order.travellers.Distinct().ToList();

                    item.CustomerName = customer.CustomerName;
                    item.CustomerEnname = customer.CustomerEnname;
                    item.Tel = customer.Tel;
                    item.BakTel = customer.BakTel;
                    item.Email = customer.Email;
                    item.Wechat = customer.Wechat;

                    //暂存时订单状态不变
                    item.state = IsCommit ? OrderState.Filled : item.state;
                    item.CustomerState = OrderStateHelper.GetOrderCustomerState(item.state, null);

                    ServiceItemHistory itemHistory = item.ServiceItemHistorys;
                    itemHistory.ElementsValue = ElementsValue;
                    itemHistory.ServiceItemTemplteValue = ServiceItemTemplteValue;
                    itemHistory.TravelDate = TravelDate;
                    itemHistory.ReturnDate = ReturnDate;
                    itemHistory.travellers = new List<OrderTraveller>();
                    itemHistory.travellers = trs;
                    await HttpHelper.PutAction("ServiceItemHistoryExtend", JsonConvert.SerializeObject(itemHistory));

                    await client.For<Order>().Key(item.OrderID).Set(item).UpdateEntryAsync();
                    await client.For<OrderHistory>().Set(new OrderHistory
                    {
                        OrderID = item.OrderID,
                        OperUserID = customer.CustomerID.ToString(),
                        OperUserNickName = "(客户)" + customer.CustomerTBCode,
                        OperTime = DateTime.Now,
                        State = item.state,
                        Remark = IsCommit ? "保存订单" : "暂存订单"
                    }).InsertEntryAsync();
                }
            }
            catch (Exception ex)
            {
                await client.For<SystemLog>().Set(new SystemLog
                {
                    Operate = "客人批量" + (IsCommit ? "填写订单" : "暂存订单") + "失败",
                    OperateTime = DateTime.Now,
                    UserID = customer.CustomerID,
                    UserName = customer.CustomerTBCode,
                    Remark = "TBOrderID：" + tBOrder.TBOrderID + "异常：" + ex.Message + "填报内容：" + JsonConvert.SerializeObject(tBOrder)
                }).InsertEntryAsync();
                return JsonConvert.SerializeObject(new { ErrorCode = 400, ErrorMessage = ex.Message });
            }
            try
            {
                //保存联系人信息
                Customer oldCustomer = await client.For<Customer>().Key(customer.CustomerID).FindEntryAsync();
                //如果客户基本信息为空，将维护到客户基本信息
                if (IsCommit && string.IsNullOrEmpty(oldCustomer.CustomerName))
                {
                    oldCustomer.CustomerName = customer.CustomerName;
                    oldCustomer.CustomerEnname = customer.CustomerEnname.ToUpper();
                    oldCustomer.Tel = customer.Tel;
                    oldCustomer.BakTel = customer.BakTel;
                    oldCustomer.Email = customer.Email;
                    oldCustomer.Wechat = customer.Wechat;
                    oldCustomer.Password = (!oldCustomer.IsUsePassWord && IsCommit) ? Md5Hash(customer.Tel) : oldCustomer.Password;

                    await client.For<Customer>().Key(oldCustomer.CustomerID).Set(oldCustomer).UpdateEntryAsync();
                }
            }
            catch (Exception ex)
            {
                await client.For<SystemLog>().Set(new SystemLog
                {
                    Operate = "客人批量填写订单时保存基本信息失败",
                    OperateTime = DateTime.Now,
                    UserID = customer.CustomerID,
                    UserName = customer.CustomerTBCode,
                    Remark = "TBOrderID：" + tBOrder.TBOrderID + "异常：" + ex.Message + "填报内容：" + JsonConvert.SerializeObject(tBOrder)
                }).InsertEntryAsync();
            }
            return JsonConvert.SerializeObject(new { ErrorCode = 200, ErrorMessage = "OK" });
        }
        private static string Md5Hash(string input)
        {
            MD5CryptoServiceProvider md5Hasher = new MD5CryptoServiceProvider();
            byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(input));
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }
        /// <summary>
        /// 检查出行日期是否被规则禁止，被禁止则返回ture
        /// </summary>
        /// <param name="ItemID"></param>
        /// <param name="dt"></param>
        /// <returns></returns>
        public async Task<bool> CheckRule(int ItemID, DateTimeOffset dt)
        {
            var rules = await client.For<ServiceRule>().Filter(s => s.RuleServiceItem.Any(r => r.ServiceItemID == ItemID) && s.UseState == EnableState.Enable).FindEntriesAsync();
            if (rules == null)
            {
                return false;
            }
            if (rules.Count() == 0)
            {
                return false;
            }
            var thisweek = dt.DayOfWeek.ToString("d");
            foreach (var rule in rules)
            {
                if (rule.StartTime.Date <= dt.Date && dt.Date <= rule.EndTime.Date)
                {
                    bool isAllow = rule.RuleUseTypeValue == RuleUseType.Allow ? true : false;
                    switch (rule.SelectRuleType)
                    {
                        case RuleType.ByDateRange:
                            if ((rule.RangeStart.Date <= dt.Date && dt.Date <= rule.RangeEnd.Date && !isAllow) || ((dt.Date < rule.RangeStart || dt.Date > rule.RangeEnd) && isAllow))
                            {
                                return true;
                            }
                            break;
                        case RuleType.ByWeek:
                            if ((rule.Week.Split('|').Contains(thisweek) && !isAllow) || (!rule.Week.Split('|').Contains(thisweek) && isAllow))
                            {
                                return true;
                            }
                            break;
                        case RuleType.ByDouble:
                            if ((((dt.Day % 2 == 0 && rule.IsDouble) || (dt.Day % 2 != 0 && !rule.IsDouble)) && !isAllow) || (((dt.Day % 2 != 0 && rule.IsDouble) || (dt.Day % 2 == 0 && !rule.IsDouble)) && isAllow))
                            {
                                return true;
                            }
                            break;
                        case RuleType.ByDate:
                            if ((rule.UseDate.Split('|').Contains(dt.Day.ToString()) && !isAllow) || (!rule.UseDate.Split('|').Contains(dt.Day.ToString()) && isAllow))
                            {
                                return true;
                            }
                            break;
                    }
                }
            }
            return false;
        }
    }
}
