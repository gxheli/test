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
using LanghuaNew.Models;
using AutoMapper;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Simple.OData.Client;
using System.Configuration;
using Commond;
using System.Net.Http;
using System.Security.Cryptography;
using Newtonsoft.Json.Linq;
using WebGrease.Css.Extensions;
using Entity;
using Pechkin;
using Aspose.Words;
using System.Text.RegularExpressions;
using Top.Api;
using Top.Api.Request;
using Top.Api.Response;

namespace LanghuaNew.Controllers
{
    public class OrdersController : Controller
    {
        public static string WeixinTemplateId = System.Configuration.ConfigurationManager.AppSettings["WeixinTemplateID"];
        public static string ForCusPath = System.Configuration.ConfigurationManager.AppSettings["ForCusPath"];
        private ODataClient client = new ODataClient(ConfigurationManager.AppSettings["ServicePath"] + "odata/");
        // GET: Orders
        public async Task<ActionResult> Index(string search)
        {
            ViewBag.Url = ConfigurationManager.AppSettings["ForCusPath"] + "langhua/client/";
            ViewBag.Supplier = await client.For<Supplier>().OrderBy(s => s.SupplierNo).FindEntriesAsync();
            ViewBag.OrderSourse = await client.For<OrderSourse>()
                .Filter(t => t.OrderSourseEnableState == EnableState.Enable)
                .OrderBy(t => t.ShowNo)
                .FindEntriesAsync();
            Dictionary<string, string> stateLeft = new Dictionary<string, string>();
            Dictionary<string, string> stateRight = new Dictionary<string, string>();
            Dictionary<string, string> stateAll = new Dictionary<string, string>();

            stateLeft.Add(((int)OrderState.Notfilled).ToString(), EnumHelper.GetEnumDescription(OrderState.Notfilled).Substring(0, EnumHelper.GetEnumDescription(OrderState.Notfilled).IndexOf("|")));
            stateLeft.Add(((int)OrderState.Filled).ToString(), EnumHelper.GetEnumDescription(OrderState.Filled).Substring(0, EnumHelper.GetEnumDescription(OrderState.Filled).IndexOf("|")));
            stateLeft.Add(((int)OrderState.Check).ToString(), EnumHelper.GetEnumDescription(OrderState.Check).Substring(0, EnumHelper.GetEnumDescription(OrderState.Check).IndexOf("|")));

            stateRight.Add(((int)OrderState.Send).ToString(), EnumHelper.GetEnumDescription(OrderState.Send).Substring(0, EnumHelper.GetEnumDescription(OrderState.Send).IndexOf("|")));
            stateRight.Add(((int)OrderState.SencondConfirm).ToString(), EnumHelper.GetEnumDescription(OrderState.SencondConfirm).Substring(0, EnumHelper.GetEnumDescription(OrderState.SencondConfirm).IndexOf("|")));
            stateRight.Add(((int)OrderState.SencondFull).ToString(), EnumHelper.GetEnumDescription(OrderState.SencondFull).Substring(0, EnumHelper.GetEnumDescription(OrderState.SencondFull).IndexOf("|")));
            stateRight.Add(((int)OrderState.SencondCancel).ToString(), EnumHelper.GetEnumDescription(OrderState.SencondCancel).Substring(0, EnumHelper.GetEnumDescription(OrderState.SencondCancel).IndexOf("|")));
            stateRight.Add(((int)OrderState.Invalid).ToString(), EnumHelper.GetEnumDescription(OrderState.Invalid).Substring(0, EnumHelper.GetEnumDescription(OrderState.Invalid).IndexOf("|")));
            stateRight.Add(((int)OrderState.SendReceive).ToString(), EnumHelper.GetEnumDescription(OrderState.SendReceive).Substring(0, EnumHelper.GetEnumDescription(OrderState.SendReceive).IndexOf("|")));
            stateRight.Add(((int)OrderState.ChangeReceive).ToString(), EnumHelper.GetEnumDescription(OrderState.ChangeReceive).Substring(0, EnumHelper.GetEnumDescription(OrderState.ChangeReceive).IndexOf("|")));
            stateRight.Add(((int)OrderState.CancelReceive).ToString(), EnumHelper.GetEnumDescription(OrderState.CancelReceive).Substring(0, EnumHelper.GetEnumDescription(OrderState.CancelReceive).IndexOf("|")));
            stateRight.Add(((int)OrderState.Confirm).ToString(), EnumHelper.GetEnumDescription(OrderState.Confirm).Substring(0, EnumHelper.GetEnumDescription(OrderState.Confirm).IndexOf("|")));
            stateRight.Add(((int)OrderState.Cancel).ToString(), EnumHelper.GetEnumDescription(OrderState.Cancel).Substring(0, EnumHelper.GetEnumDescription(OrderState.Cancel).IndexOf("|")));
            stateRight.Add(((int)OrderState.Full).ToString(), EnumHelper.GetEnumDescription(OrderState.Full).Substring(0, EnumHelper.GetEnumDescription(OrderState.Full).IndexOf("|")));
            stateRight.Add(((int)OrderState.RequestCancel).ToString(), EnumHelper.GetEnumDescription(OrderState.RequestCancel).Substring(0, EnumHelper.GetEnumDescription(OrderState.RequestCancel).IndexOf("|")));
            stateRight.Add(((int)OrderState.RequestChange).ToString(), EnumHelper.GetEnumDescription(OrderState.RequestChange).Substring(0, EnumHelper.GetEnumDescription(OrderState.RequestChange).IndexOf("|")));

            stateLeft.ForEach(s => stateAll.Add(s.Key, s.Value));
            stateRight.ForEach(s => stateAll.Add(s.Key, s.Value));

            stateLeft.Add("5,7,10", "待检查");
            stateLeft.Add("3,4,14,15", "处理中");
            stateLeft.Add("9,12", "请求中");
            ViewBag.stateLift = stateLeft;
            ViewBag.stateRight = stateRight;
            ViewBag.stateAll = stateAll;

            Dictionary<int, string> exportField = new Dictionary<int, string>();
            foreach (ExportField item in Enum.GetValues(typeof(ExportField)))
            {

                if (item != ExportField.GroupNo)
                {
                    string state = EnumHelper.GetEnumDescription(item);
                    exportField.Add((int)item, state);
                }
            }
            bool isReceive = false;//接单按钮
            string userName = User.Identity.Name;
            User user = await client.For<User>().Expand("UserRole/MenuRights").Filter(u => u.UserName == userName).FindEntryAsync();
            if (user.UserRole != null)
            {
                foreach (var item in user.UserRole.Where(s => s.RoleEnableState == EnableState.Enable))
                {
                    if (item.RoleID == 1)
                    {
                        isReceive = true;
                        break;
                    }
                    if (item.MenuRights != null)
                    {
                        foreach (var MenuRight in item.MenuRights)
                        {
                            var MenuResult = await client.For<MenuRight>().Expand(s => s.RoleRights).Key(MenuRight.MenuRightID).FindEntryAsync();
                            foreach (var rights in MenuResult.RoleRights)
                            {
                                if (rights.ControllerName == "Orders" && rights.ActionName == "Receive")
                                {
                                    isReceive = true;
                                }
                            }
                        }
                    }
                }
            }
            ViewBag.CreateName = user.NickName;
            Dictionary<int, string> operations = new Dictionary<int, string>();
            foreach (OrderOperations item in Enum.GetValues(typeof(OrderOperations)))
            {
                if (item != OrderOperations.Invalid)//不显示作废按钮
                {
                    if (item != OrderOperations.Receive || (item == OrderOperations.Receive && isReceive))
                    {
                        string state = EnumHelper.GetEnumDescription(item);
                        operations.Add((int)item, state);
                    }
                }
            }
            ViewBag.exportField = exportField;
            ViewBag.operations = operations;
            ViewBag.search = search;
            return View();
        }
        // GET: Orders/OrderFinish?totalCost=200&TBOrderID=1001
        public async Task<ActionResult> OrderFinish(float totalCost, int TBOrderID)
        {
            ViewBag.Url = ConfigurationManager.AppSettings["ForCusPath"] + "langhua/client/" + TBOrderID;
            TBOrder tborder = await client.For<TBOrder>().Expand("Orders/TBOrderNos").Key(TBOrderID).FindEntryAsync();

            ViewBag.Payment = tborder.Orders.Sum(s => s.TBOrderNos == null ? 0 : s.TBOrderNos.Sum(t => t.Payment));
            ViewBag.totalCost = totalCost;
            ViewBag.TBOrderID = TBOrderID;
            ViewBag.TBID = tborder.TBID;
            return View();
        }
        //保存总金额
        [HttpPost]
        [ActionName("OrderFinishPost")]
        public async Task<ActionResult> OrderFinish(float totalCost, float TotalReceive, int TBOrderID)
        {
            try
            {
                TBOrder tborder = await client.For<TBOrder>().Key(TBOrderID).FindEntryAsync();
                tborder.TotalReceive = TotalReceive;

                await client.For<TBOrder>().Key(TBOrderID).Set(tborder).UpdateEntryAsync();
                return RedirectToAction("Index");
            }
            catch
            {
                ViewBag.Url = ConfigurationManager.AppSettings["ForCusPath"] + "langhua/client/" + TBOrderID;
                TBOrder tborder = await client.For<TBOrder>().Expand("Orders/TBOrderNos").Key(TBOrderID).FindEntryAsync();
                ViewBag.Payment = tborder.Orders.Sum(s => s.TBOrderNos == null ? 0 : s.TBOrderNos.Sum(t => t.Payment));
                ViewBag.totalCost = totalCost;
                ViewBag.TBOrderID = TBOrderID;
                ViewBag.TBID = tborder.TBID;
                return View();
            }
        }
        //要售后/未付完
        [HttpPost]
        public async Task<string> OrderAfterSaleOperation(int Type, int OrderID)
        {
            try
            {
                Order order = await client.For<Order>().Key(OrderID).FindEntryAsync();
                string Remark = "";
                switch (Type)
                {
                    case 1:
                        order.IsNeedCustomerService = true;
                        Remark = "要售后";
                        break;
                    case 2:
                        order.IsNeedCustomerService = false;
                        Remark = "取消要售后";
                        break;
                    case 3:
                        order.IsPay = true;
                        Remark = "未付完";
                        break;
                    case 4:
                        order.IsPay = false;
                        Remark = "取消未付完";
                        break;
                    case 5:
                        order.isUrgent = true;
                        Remark = "紧急订单";
                        break;
                    case 6:
                        order.isUrgent = false;
                        Remark = "取消紧急订单";
                        break;
                    default:
                        return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "状态码异常！" });
                }
                string userName = User.Identity.Name;
                User user = await client.For<User>().Filter(u => u.UserName == userName).FindEntryAsync();
                await client.For<Order>().Key(order.OrderID).Set(order).UpdateEntryAsync();
                await client.For<OrderHistory>().Set(new { OrderID = order.OrderID, OperUserID = user.UserID.ToString(), OperUserNickName = user.NickName, OperTime = DateTime.Now, State = order.state, Remark = Remark }).InsertEntryAsync();
                await UpdateCustomerService(order.CustomerID);
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 400, ErrorMessage = ex });
            }
            return JsonConvert.SerializeObject(new { ErrorCode = 200, ErrorMessage = "OK", Type });
        }
        /// <summary>
        /// 更新客户要售后状态
        /// </summary>
        private async Task UpdateCustomerService(int customerid)
        {
            //只要这个客户名下有1个订单标记了【要售后】，那么这个客户就打标【要售后】，否则就取消【要售后】标签
            var order = await client.For<Order>().Filter(o => o.CustomerID == customerid && o.IsNeedCustomerService).Select(o => o.OrderID).FindEntryAsync();
            var customer = await client.For<Customer>().Key(customerid).FindEntryAsync();
            if (order != null && !customer.IsNeedCustomerService)
            {
                customer.IsNeedCustomerService = true;
                await client.For<Customer>().Key(customerid).Set(customer).UpdateEntryAsync();
            }
            else if (order == null && customer.IsNeedCustomerService)
            {
                customer.IsNeedCustomerService = false;
                await client.For<Customer>().Key(customerid).Set(customer).UpdateEntryAsync();
            }
        }
        //订单流转
        [HttpPost]
        public async Task<string> UpdateState(int state, string OrderID, string Remark)
        {
            var successed = (new int[] { 1 }).Select(x => new { OrderNo = "" }).ToList();
            var failed = (new int[] { 1 }).Select(x => new { OrderNo = "", reason = "" }).ToList();
            successed.Clear();
            failed.Clear();

            if (string.IsNullOrEmpty(OrderID))
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "订单不能为空！" });
            }
            try
            {
                var id = OrderID.Split(',');
                foreach (var i in id)
                {
                    #region MyRegion
                    Order order = await client.For<Order>().Expand(t => t.Customers)
                                   .Expand(t => t.OrderHistorys)
                                   .Expand(t => t.ServiceItemHistorys.ExtraServiceHistorys)
                                   .Key(int.Parse(i)).FindEntryAsync();
                    OrderState oldState = order.state;//原订单状态
                    if (oldState == OrderState.Invalid)
                    {
                        failed.Add(new { OrderNo = order.OrderNo, reason = "订单已作废" });
                    }
                    else if (oldState == OrderState.SencondCancel)
                    {
                        failed.Add(new { OrderNo = order.OrderNo, reason = "订单已取消" });
                    }
                    else
                    {
                        string strOldState = EnumHelper.GetEnumDescription(oldState).Substring(0, EnumHelper.GetEnumDescription(oldState).IndexOf("|"));
                        OrderOperations operation = (OrderOperations)state;//操作
                        string strOperation = EnumHelper.GetEnumDescription((OrderOperations)state);
                        //判断该供应商是否为网上供应商

                        Supplier supplier = await client.For<Supplier>()
                            .Expand(s => s.SupplierUsers)
                            .Filter(s => s.SupplierID == order.ServiceItemHistorys.SupplierID && s.SupplierEnableState == EnableState.Enable && s.EnableOnline)
                            .FindEntryAsync();

                        bool bl = false;
                        string reason = strOldState + "状态不能进行" + strOperation + "操作";
                        switch (operation)
                        {
                            //1action核对
                            case OrderOperations.Check:
                                int itemid = order.ServiceItemHistorys.ServiceItemID;
                                ServiceItem serviceItem = await client.For<ServiceItem>().Filter(s => s.ServiceItemID == itemid && s.ExtraServices.Any(e => e.MinNum > 0)).FindEntryAsync();
                                if (serviceItem != null)
                                {
                                    if (order.ServiceItemHistorys.ExtraServiceHistorys == null)
                                    {
                                        reason = "请先选择附加项目";
                                        bl = true;
                                    }
                                }
                                break;
                            //2action发送
                            case OrderOperations.Send:
                                break;
                            //3action接单
                            case OrderOperations.Receive:
                                string userName = User.Identity.Name;
                                User user = await client.For<User>().Expand("UserRole/MenuRights").Filter(u => u.UserName == userName).FindEntryAsync();
                                bool isReceive = false;
                                if (user.UserRole != null)
                                {
                                    foreach (var item in user.UserRole.Where(s => s.RoleEnableState == EnableState.Enable))
                                    {
                                        if (item.RoleID == 1)
                                        {
                                            isReceive = true;
                                            break;
                                        }
                                        if (item.MenuRights != null)
                                        {
                                            foreach (var MenuRight in item.MenuRights)
                                            {
                                                var MenuResult = await client.For<MenuRight>().Expand(s => s.RoleRights).Key(MenuRight.MenuRightID).FindEntryAsync();
                                                foreach (var rights in MenuResult.RoleRights)
                                                {
                                                    if (rights.ControllerName == "Orders" && rights.ActionName == "Receive")
                                                    {
                                                        isReceive = true;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                if (!isReceive)
                                {
                                    return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "对不起，您没有接单的权限！" });
                                }
                                if (supplier != null)
                                {
                                    reason = "检测到该供应商为网上供应商," + reason;
                                    bl = true;
                                }
                                break;
                            //4action确认
                            case OrderOperations.Confirm:
                                if (oldState == OrderState.SendReceive || oldState == OrderState.ChangeReceive)
                                {
                                    if (supplier != null)
                                    {
                                        reason = "检测到该供应商为网上供应商," + reason;
                                        bl = true;
                                    }
                                }
                                if (oldState == OrderState.SencondFull)
                                {
                                    //已拒绝要变为已确认时，需要判断该单历史状态是否有已确认
                                    if (order.OrderHistorys.Where(t => t.State == OrderState.SencondConfirm).Count() == 0)
                                    {
                                        reason = "检测到该订单没有到过已确认状态，" + reason;
                                        bl = true;
                                    }
                                }
                                break;
                            //5action拒绝
                            case OrderOperations.Full:
                                if (oldState == OrderState.SendReceive || oldState == OrderState.ChangeReceive || oldState == OrderState.CancelReceive)
                                {
                                    if (supplier != null)
                                    {
                                        reason = "检测到该供应商为网上供应商," + reason;
                                        bl = true;
                                    }
                                }
                                break;
                            //6action请求取消
                            case OrderOperations.RequestCancel:
                                break;
                            //7action取消
                            case OrderOperations.Cancel:
                                if (oldState == OrderState.CancelReceive)
                                {
                                    if (supplier != null)
                                    {
                                        reason = "检测到该供应商为网上供应商," + reason;
                                        bl = true;
                                    }
                                }
                                break;
                            //8action作废
                            case OrderOperations.Invalid:
                                break;
                            //其他请求为异常
                            default:
                                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "状态异常" });
                        }
                        //获取新状态
                        OrderState newState = OrderStateHelper.GetOrderState(oldState, operation, OrderOperator.inside);
                        //当进行发送操作时，若该供应商为线下供应商，则自动接单
                        if (operation == OrderOperations.Send && newState == OrderState.Send)
                        {
                            if (supplier == null)
                            {
                                newState = OrderStateHelper.GetOrderState(newState, OrderOperations.Receive, OrderOperator.inside);
                            }
                        }
                        string strNewState = EnumHelper.GetEnumDescription(newState).Substring(0, EnumHelper.GetEnumDescription(newState).IndexOf("|"));
                        if (bl || newState == OrderState.nullandvoid)
                        {
                            failed.Add(new { OrderNo = order.OrderNo, reason = reason });
                        }
                        else if (operation == OrderOperations.Send && order.IsPay)
                        {
                            failed.Add(new { OrderNo = order.OrderNo, reason = "该订单标记未付完" });
                        }
                        else if (operation == OrderOperations.RequestCancel && string.IsNullOrEmpty(Remark))
                        {
                            failed.Add(new { OrderNo = order.OrderNo, reason = "取消原因不能为空" });
                        }
                        else
                        {
                            string HistoryRemark = "状态:" + strOldState + "→" + strNewState;
                            if (operation == OrderOperations.RequestCancel && !string.IsNullOrEmpty(Remark))
                            {
                                order.Remark = string.IsNullOrEmpty(order.Remark) ? Remark : order.Remark + "；" + Remark;
                                HistoryRemark += "；" + Remark;
                            }
                            string userName = User.Identity.Name;
                            User user = await client.For<User>().Filter(u => u.UserName == userName).FindEntryAsync();
                            order.state = newState;
                            order.CustomerState = OrderStateHelper.GetOrderCustomerState(newState, oldState);
                            if ((newState == OrderState.SencondFull && (oldState == OrderState.Full || oldState == OrderState.ChangeReceive)) || newState == OrderState.RequestCancel)
                            {
                                order.ServiceItemHistorys.ChangeTravelDate = DateTimeOffset.MinValue;
                                order.ServiceItemHistorys.ChangeValue = null;
                                order.ServiceItemHistorys.ChangeElementsValue = null;
                                await client.For<ServiceItemHistory>().Key(order.OrderID).Set(order.ServiceItemHistorys).UpdateEntryAsync();
                            }
                            if (newState == OrderState.SencondConfirm || newState == OrderState.SencondCancel || newState == OrderState.Invalid)
                            {
                                order.isUrgent = false;
                            }
                            await client.For<Order>().Key(order.OrderID).Set(order).UpdateEntryAsync();
                            await client.For<OrderHistory>().Set(new { OrderID = order.OrderID, OperUserID = user.UserID.ToString(), OperUserNickName = user.NickName, OperTime = DateTime.Now, State = order.state, Remark = HistoryRemark }).InsertEntryAsync();
                            #region 供应商微信推送
                            if (newState == OrderState.Send || newState == OrderState.RequestCancel)
                            {
                                if (supplier != null && supplier.SupplierUsers != null && supplier.SupplierUsers.Count > 0)
                                {
                                    foreach (var item in supplier.SupplierUsers)
                                    {
                                        if (!string.IsNullOrEmpty(item.OpenID) && item.RealTimeMessage)
                                        {
                                            bool Disturb = false;//true为免打扰
                                            if (item.Disturb)
                                            {
                                                try
                                                {
                                                    string BeginTime = item.BeginTime;
                                                    string EndTime = item.EndTime;
                                                    //免打扰开始时间大于结束时间,按跨天算,当前时间大于开始时间或者小于结束时间则开启免打扰，不发实时消息
                                                    if (int.Parse(BeginTime.Split(':')[0]) > int.Parse(EndTime.Split(':')[0])
                                                        || (int.Parse(BeginTime.Split(':')[0]) == int.Parse(EndTime.Split(':')[0]) && int.Parse(BeginTime.Split(':')[1]) > int.Parse(EndTime.Split(':')[1])))
                                                    {
                                                        if (DateTimeOffset.Now.Hour > int.Parse(BeginTime.Split(':')[0])
                                                            || (DateTimeOffset.Now.Hour == int.Parse(BeginTime.Split(':')[0]) && DateTimeOffset.Now.Minute > int.Parse(BeginTime.Split(':')[1]))
                                                            || DateTimeOffset.Now.Hour < int.Parse(EndTime.Split(':')[0])
                                                            || (DateTimeOffset.Now.Hour == int.Parse(EndTime.Split(':')[0]) && DateTimeOffset.Now.Minute < int.Parse(EndTime.Split(':')[1])))
                                                        {
                                                            Disturb = true;//免打扰
                                                        }
                                                    }
                                                    else//免打扰开始时间小于结束时间,按当天算,当前时间大于开始时间并且小于结束时间则开启免打扰，不发实时消息
                                                    {
                                                        if ((DateTimeOffset.Now.Hour > int.Parse(BeginTime.Split(':')[0])
                                                            || (DateTimeOffset.Now.Hour == int.Parse(BeginTime.Split(':')[0]) && DateTimeOffset.Now.Minute > int.Parse(BeginTime.Split(':')[1])))
                                                            && (DateTimeOffset.Now.Hour < int.Parse(EndTime.Split(':')[0])
                                                            || (DateTimeOffset.Now.Hour == int.Parse(EndTime.Split(':')[0]) && DateTimeOffset.Now.Minute < int.Parse(EndTime.Split(':')[1]))))
                                                        {
                                                            Disturb = true;//免打扰
                                                        }
                                                    }

                                                }
                                                catch
                                                {

                                                }
                                            }
                                            if (!Disturb)
                                            {
                                                try
                                                {
                                                    var supplierMessage = WeiXinHelper.SupplierSendMessage(item.OpenID,
                                                    "您好，您有一条订单等待处理。" + " \r\n ",
                                                    EnumHelper.GetEnumDescription(newState).Substring(EnumHelper.GetEnumDescription(newState).IndexOf("|") + 1),
                                                    order.ServiceItemHistorys.cnItemName,
                                                    order.OrderNo,
                                                    " \r\n 请及时登录系统后台进行处理，谢谢！");
                                                    await client.For<SystemLog>().Set(new
                                                    {
                                                        Operate = "供应商微信推送",
                                                        OperateTime = DateTime.Now,
                                                        UserID = item.SupplierUserID,
                                                        UserName = item.SupplierNickName,
                                                        Remark = "OrderID：" + order.OrderID + " Message：" + supplierMessage
                                                    }).InsertEntryAsync();
                                                }
                                                catch (Exception ex)
                                                {
                                                    await client.For<SystemLog>().Set(new
                                                    {
                                                        Operate = "供应商微信推送失败",
                                                        OperateTime = DateTime.Now,
                                                        UserID = item.SupplierUserID,
                                                        UserName = item.SupplierNickName,
                                                        Remark = "OrderID：" + order.OrderID + " Message：" + ex.Message
                                                    }).InsertEntryAsync();
                                                }
                                            }
                                        }
                                    }
                                }
                                await client.For<OrderSupplierHistory>().Set(new OrderSupplierHistory
                                {
                                    opera = OrderOperator.inside,
                                    State = newState,
                                    OrderID = order.OrderID,
                                    OperUserID = user.UserID,
                                    OperTime = DateTimeOffset.Now,
                                    OperNickName = "(浪花朵朵)" + user.NickName,
                                    Remark = "状态:" + EnumHelper.GetEnumDescription(oldState).Substring(EnumHelper.GetEnumDescription(oldState).IndexOf("|") + 1) + "→" + EnumHelper.GetEnumDescription(newState).Substring(EnumHelper.GetEnumDescription(newState).IndexOf("|") + 1)
                                }).InsertEntryAsync();
                            }
                            #endregion
                            if (newState == OrderState.SencondConfirm && (oldState == OrderState.ChangeReceive || oldState == OrderState.Confirm))
                            {
                                //从变更已接或确认待检到已确认时，需要将变更的信息覆盖到订单值中
                                if (!string.IsNullOrEmpty(order.ServiceItemHistorys.ChangeValue))
                                {
                                    await ConfirmChange(order.OrderID);
                                }
                            }
                            //确认和取消时，需要更新发货最早出行日期
                            if (order.state == OrderState.SencondConfirm || order.state == OrderState.SencondCancel)
                            {
                                await SaveTBOrderState(order.TBNum);
                            }
                            //发送微信推送消息
                            if (!string.IsNullOrEmpty(order.Customers.OpenID))
                            {
                                await SendWeixin(order);
                            }
                            successed.Add(new { OrderNo = order.OrderNo });
                        }
                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 400, ErrorMessage = ex, successed, failed });
            }
            return JsonConvert.SerializeObject(new { ErrorCode = 200, ErrorMessage = "OK", successed, failed });
        }
        public async Task ConfirmChange(int OrderID)
        {
            try
            {
                Order order = await client.For<Order>().Expand(t => t.ServiceItemHistorys).Key(OrderID).FindEntryAsync();
                string changevalue = order.ServiceItemHistorys.ChangeValue;
                JObject jo = JObject.Parse(changevalue);
                var systemMap = jo.SelectToken("$..systemMap");
                if (systemMap != null)
                {
                    var TravelDate = systemMap.SelectToken("$..TravelDate") == null ? "" : systemMap.SelectToken("$..TravelDate").ToString();
                    var ReturnDate = systemMap.SelectToken("$..ReturnDate") == null ? "" : systemMap.SelectToken("$..ReturnDate").ToString();
                    order.ServiceItemHistorys.TravelDate = string.IsNullOrEmpty(TravelDate) ? order.ServiceItemHistorys.TravelDate : DateTimeOffset.Parse(TravelDate);
                    order.ServiceItemHistorys.ReturnDate = string.IsNullOrEmpty(ReturnDate) ? order.ServiceItemHistorys.ReturnDate : DateTimeOffset.Parse(ReturnDate);
                }
                var systemFile = jo.SelectToken("$..nameAndNum");
                if (systemFile != null)
                {
                    var AdultNum = systemFile.SelectToken("$..AdultNum") == null ? "" : systemFile.SelectToken("$..AdultNum").ToString();
                    var ChildNum = systemFile.SelectToken("$..ChildNum") == null ? "" : systemFile.SelectToken("$..ChildNum").ToString();
                    var INFNum = systemFile.SelectToken("$..INFNum") == null ? "" : systemFile.SelectToken("$..INFNum").ToString();
                    var RightNum = systemFile.SelectToken("$..RightNum") == null ? "" : systemFile.SelectToken("$..RightNum").ToString();
                    var RoomNum = systemFile.SelectToken("$..RoomNum") == null ? "" : systemFile.SelectToken("$..RoomNum").ToString();
                    var CustomerName = systemFile.SelectToken("$..CustomerName") == null ? "" : systemFile.SelectToken("$..CustomerName").ToString();
                    var CustomerEnname = systemFile.SelectToken("$..CustomerEnname") == null ? "" : systemFile.SelectToken("$..CustomerEnname").ToString();
                    var Tel = systemFile.SelectToken("$..Tel") == null ? "" : systemFile.SelectToken("$..Tel").ToString();
                    var BakTel = systemFile.SelectToken("$..BakTel") == null ? "" : systemFile.SelectToken("$..BakTel").ToString();
                    var Email = systemFile.SelectToken("$..Email") == null ? "" : systemFile.SelectToken("$..Email").ToString();
                    var Wechat = systemFile.SelectToken("$..Wechat") == null ? "" : systemFile.SelectToken("$..Wechat").ToString();

                    order.ServiceItemHistorys.AdultNum = string.IsNullOrEmpty(AdultNum) ? order.ServiceItemHistorys.AdultNum : int.Parse(AdultNum);
                    order.ServiceItemHistorys.ChildNum = string.IsNullOrEmpty(ChildNum) ? order.ServiceItemHistorys.ChildNum : int.Parse(ChildNum);
                    order.ServiceItemHistorys.INFNum = string.IsNullOrEmpty(INFNum) ? order.ServiceItemHistorys.INFNum : int.Parse(INFNum);
                    order.ServiceItemHistorys.RightNum = string.IsNullOrEmpty(RightNum) ? order.ServiceItemHistorys.RightNum : int.Parse(RightNum);
                    order.ServiceItemHistorys.RoomNum = string.IsNullOrEmpty(RoomNum) ? order.ServiceItemHistorys.RoomNum : int.Parse(RoomNum);
                    order.CustomerName = string.IsNullOrEmpty(CustomerName) ? order.CustomerName : CustomerName;
                    order.CustomerEnname = string.IsNullOrEmpty(CustomerEnname) ? order.CustomerEnname : CustomerEnname;
                    order.Tel = string.IsNullOrEmpty(Tel) ? order.Tel : Tel;
                    order.BakTel = string.IsNullOrEmpty(BakTel) ? order.BakTel : BakTel;
                    order.Email = string.IsNullOrEmpty(Email) ? order.Email : Email;
                    order.Wechat = string.IsNullOrEmpty(Wechat) ? order.Wechat : Wechat;
                }
                var formElemnents = jo.SelectToken("$..formElemnents");
                order.ServiceItemHistorys.ElementsValue = JsonConvert.SerializeObject(formElemnents.SelectToken("$.." + OrderID));
                var templatevalue = jo.SelectToken("$..templatevalue");
                order.ServiceItemHistorys.ServiceItemTemplteValue = JsonConvert.SerializeObject(templatevalue);

                var extra = jo.SelectToken("$..extra");
                List<ExtraServiceHistory> exs = new List<ExtraServiceHistory>();
                if (extra != null && extra.Count() > 0)
                {
                    foreach (var item in extra)
                    {
                        ExtraServiceHistory ex = new ExtraServiceHistory();
                        var ExtraServiceID = item.SelectToken("$..ExtraServiceID");
                        var ServiceName = item.SelectToken("$..ServiceName");
                        var ServiceEnName = item.SelectToken("$..ServiceEnName");
                        var ServiceUnit = item.SelectToken("$..ServiceUnit");
                        var MinNum = item.SelectToken("$..MinNum");
                        var MaxNum = item.SelectToken("$..MaxNum");
                        var ServiceNum = item.SelectToken("$..ServiceNum");
                        var ServicePrice = item.SelectToken("$..ServicePrice");
                        ex.ExtraServiceID = ExtraServiceID == null ? 0 : int.Parse(ExtraServiceID.ToString());
                        ex.ServiceName = ServiceName == null ? "" : ServiceName.ToString();
                        ex.ServiceEnName = ServiceEnName == null ? "" : ServiceEnName.ToString();
                        ex.ServiceUnit = ServiceUnit == null ? "" : ServiceUnit.ToString();
                        ex.MinNum = MinNum == null ? 0 : int.Parse(MinNum.ToString());
                        ex.MaxNum = MaxNum == null ? 0 : int.Parse(MaxNum.ToString());
                        ex.ServiceNum = ServiceNum == null ? 0 : int.Parse(ServiceNum.ToString());
                        ex.ServicePrice = ServicePrice == null ? 0 : float.Parse(ServicePrice.ToString());
                        exs.Add(ex);
                    }
                }
                order.ServiceItemHistorys.ExtraServiceHistorys = (exs != null && exs.Count > 0) ? exs : order.ServiceItemHistorys.ExtraServiceHistorys;
                order.ServiceItemHistorys.ChangeTravelDate = DateTimeOffset.MinValue;//将变更值清空
                order.ServiceItemHistorys.ChangeValue = null;//将变更值清空
                order.ServiceItemHistorys.ChangeElementsValue = null;//将变更值清空
                await client.For<Order>().Key(OrderID).Set(order).UpdateEntryAsync();
                await HttpHelper.PostAction("ServiceItemHistoryExtend", JsonConvert.SerializeObject(order.ServiceItemHistorys));
                var persons = jo.SelectToken("$..persons");
                await client.For<OrderTraveller>().Filter(t => t.OrderID == OrderID).DeleteEntriesAsync();
                if (persons != null && persons.Count() > 0)
                {
                    foreach (var item in persons)
                    {
                        var PassportNo = item.SelectToken("$..PassportNo");
                        var TravellerEnname = item.SelectToken("$..TravellerEnname");
                        var TravellerSex = item.SelectToken("$..TravellerSex");
                        var Birthday = item.SelectToken("$..Birthday");
                        var TravellerID = item.SelectToken("$..TravellerID");
                        var TravellerName = item.SelectToken("$..TravellerName");
                        var ClothesSize = item.SelectToken("$..ClothesSize");
                        var GlassesNum = item.SelectToken("$..GlassesNum");
                        var Height = item.SelectToken("$..Height");
                        var ShoesSize = item.SelectToken("$..ShoesSize");
                        var Weight = item.SelectToken("$..Weight");
                        OrderTraveller ordertr = new OrderTraveller();
                        ordertr.Birthday = Birthday == null ? DateTimeOffset.MinValue : DateTimeOffset.Parse(Birthday.ToString());
                        ordertr.CreateTime = DateTimeOffset.Now;
                        ordertr.CustomerID = order.CustomerID;
                        ordertr.TravellerID = TravellerID == null ? 0 : int.Parse(TravellerID.ToString());
                        ordertr.OrderID = order.OrderID;
                        ordertr.PassportNo = PassportNo == null ? "" : PassportNo.ToString();
                        ordertr.TravellerEnname = TravellerEnname == null ? "" : TravellerEnname.ToString();
                        ordertr.TravellerSex = (TravellerSex == null && int.Parse(TravellerSex.ToString()) > 1) ? sex.Male : (sex)int.Parse(TravellerSex.ToString());
                        ordertr.TravellerName = TravellerName == null ? "" : TravellerName.ToString();
                        ordertr.TravellerDetail = new OrderTravellerDetail();
                        ordertr.TravellerDetail.ClothesSize = ClothesSize == null ? "" : ClothesSize.ToString();
                        ordertr.TravellerDetail.GlassesNum = GlassesNum == null ? "" : GlassesNum.ToString();
                        ordertr.TravellerDetail.Height = Height == null ? "" : Height.ToString();
                        ordertr.TravellerDetail.ShoesSize = ShoesSize == null ? "" : ShoesSize.ToString();
                        ordertr.TravellerDetail.Weight = Weight == null ? "" : Weight.ToString();
                        await client.For<OrderTraveller>().Set(ordertr).InsertEntryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                await client.For<SystemLog>().Set(new { Operate = "变更失败", OperateTime = DateTime.Now, UserID = 0, UserName = "", Remark = "ID：" + OrderID + " 异常：" + ex.Message }).InsertEntryAsync();
            }
        }
        /// <summary>
        /// 保存淘宝订单状态
        /// </summary>
        private async Task SaveTBOrderState(string TBNum)
        {
            var orders = await client.For<Order>().Expand(o => o.TBOrders).Expand(o => o.ServiceItemHistorys).Filter(o => o.TBNum == TBNum && o.state != OrderState.Invalid).FindEntriesAsync();
            if (orders != null)
            {
                var order = orders.ToList()[0];
                //客户
                int CustomerID = order.CustomerID;
                //订单来源
                int OrderSourseID = order.TBOrders.OrderSourseID;
                //有确认订单,将发货表新增一笔发货记录或更新最早出行日期
                if (orders.Where(o => o.state == OrderState.SencondConfirm).FirstOrDefault() != null)
                {
                    bool Deliver = true;//自动发货
                    foreach (var item in orders)
                    {
                        int ItemID = item.ServiceItemHistorys.ServiceItemID;
                        if (await client.For<ServiceItem>().Filter(s => s.ServiceItemID == ItemID && !s.IsAutomaticDeliver).Count().FindScalarAsync<int>() > 0)
                        {
                            //如果有不是自动发货的产品
                            Deliver = false;
                        }
                    }
                    if (Deliver)
                    {
                        return;//如果全部是自动发货，则不需记录
                    }
                    //最早出行日期
                    DateTimeOffset MinTravelDate = DateTime.MaxValue;
                    foreach (var item in orders.Where(o => o.state == OrderState.SencondConfirm && o.TBOrders.OrderSourseID == OrderSourseID))
                    {
                        MinTravelDate = item.ServiceItemHistorys.TravelDate < MinTravelDate ? item.ServiceItemHistorys.TravelDate : MinTravelDate;
                    }
                    TBOrderState state = await client.For<TBOrderState>().Filter(o => o.TBNum == TBNum && o.CustomerID == CustomerID && o.OrderSourseID == OrderSourseID).FindEntryAsync();
                    if (state == null)
                    {
                        state = new TBOrderState();
                        state.CustomerID = CustomerID;
                        state.TBNum = TBNum;
                        state.OrderSourseID = OrderSourseID;
                        state.IsSend = false;
                        state.OrderDate = MinTravelDate;
                        await client.For<TBOrderState>().Set(state).InsertEntryAsync();
                    }
                    else
                    {
                        state.OrderDate = MinTravelDate;
                        await client.For<TBOrderState>().Key(state.TBOrderStateID).Set(state).UpdateEntryAsync();
                    }
                }
                else//无确认订单，将发货表未发货的记录清除
                {

                    TBOrderState state = await client.For<TBOrderState>().Filter(o => o.TBNum == TBNum && o.CustomerID == CustomerID && o.OrderSourseID == OrderSourseID && !o.IsSend).FindEntryAsync();
                    if (state != null)
                    {
                        await client.For<TBOrderState>().Key(state.TBOrderStateID).Set(state).DeleteEntryAsync();
                    }
                }
            }
        }
        private async Task SendWeixin(Order order)
        {
            string sendinfo = "";
            switch (order.state)
            {
                case OrderState.Check:
                    sendinfo = "资料已核对完毕，等待客服进行预订操作。";
                    break;
                case OrderState.Send:
                    sendinfo = "订单在预订过程中，一般需要2个工作日，请耐心等待。";
                    break;
                case OrderState.SendReceive:
                    sendinfo = "订单在预订过程中，一般需要2个工作日，请耐心等待。";
                    break;
                case OrderState.SencondFull:
                    sendinfo = "订单因各种原因没有预订、变更、取消成功，请尽快联系客服咨询。";
                    break;
                case OrderState.SencondConfirm:
                    sendinfo = "订单已预订成功，信息被确认，可通过“我的订单”菜单查看确认单。";
                    break;
                case OrderState.SencondCancel:
                    sendinfo = "订单已经被成功取消。";
                    break;
                default:
                    return;
            }

            DateTimeOffset Now = new DateTimeOffset(DateTime.Now.Date);
            var message = await client.For<WeixinMessage>()
                .Filter(w => w.WeixinCountry.Suppliers.Any(s => s.SupplierID == order.ServiceItemHistorys.SupplierID))
                .Filter(w => w.StartTime <= Now && w.EndTime >= Now)
                .Filter(w => w.WeixinMessageState == EnableState.Enable)
                .FindEntryAsync();
            string infoMessage = sendinfo;
            string URL = "";
            string state = "User";
            if (message != null)
            {
                infoMessage += string.IsNullOrEmpty(message.Message) ? "" : (" \r\n \r\n " + message.Message);
                URL = message.Url;
            }
            else
            {
                if (order.state == OrderState.SencondConfirm)
                {
                    URL = ForCusPath + "Weixin/orderdetail";
                    state = order.OrderID.ToString();
                }
                else
                {

                    URL = ForCusPath + "Weixin/myorder/";
                }
            }
            try
            {
                var Message = WeiXinHelper.SendMessage(order.Customers.OpenID,
                "您好,您的订单" + EnumHelper.GetEnumDescription(order.CustomerState) + " \r\n ",
                order.OrderNo,
                order.ServiceItemHistorys.cnItemName,
                " \r\n " + infoMessage,
                URL, state);
                await client.For<SystemLog>().Set(new { Operate = "微信推送", OperateTime = DateTime.Now, UserID = 0, UserName = order.Customers.CustomerTBCode, Remark = "OrderID：" + order.OrderID + " Message：" + Message }).InsertEntryAsync();
            }
            catch (Exception ex)
            {
                await client.For<SystemLog>().Set(new { Operate = "微信推送失败", OperateTime = DateTime.Now, UserID = 0, UserName = order.Customers.CustomerTBCode, Remark = "OrderID：" + order.OrderID + " Message：" + ex.Message }).InsertEntryAsync();
            }
        }
        // GET: Orders/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Order order = await GetOrderByID(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            string html = await GetConfimOrderHtml(id, order);
            ViewBag.html = html;//去掉UTF8的标识
            ViewBag.Currency = await client.For<Currency>().Filter(c => c.CurrencyEnableState == EnableState.Enable).FindEntriesAsync();
            int SupplierID = order.ServiceItemHistorys.SupplierID;
            var supplier = await client.For<Supplier>().Key(SupplierID).FindEntryAsync();
            ViewBag.SupplierEnName = supplier.SupplierEnName;
            ViewBag.EnableOnline = supplier.EnableOnline;
            return View(order);
        }

        public async Task<Order> GetOrderByID(int? id)
        {
            Order order = await client.For<Order>()
             .Expand(u => u.ServiceItemHistorys)
             .Expand(u => u.ServiceItemHistorys.ItemTemplte)
             .Expand(u => u.ServiceItemHistorys.ExtraServiceHistorys)
             .Expand(u => u.Customers)
             .Expand(u => u.OrderHistorys)
             .Expand(u => u.TBOrders)
             .Key(id).FindEntryAsync();
            return order;
        }

        public async Task<string> GetConfimOrderHtml(int? id, Order order)
        {
            string html = "";
            try
            {
                html = order.ServiceItemHistorys.ItemTemplte == null ? "订单未上传模板，请从产品表单维护页面上传模板后点击右上角更新模板按钮" : order.ServiceItemHistorys.ItemTemplte.ServiceItemTemplteHtml;
                //附加项目
                string enExName = "";
                string cnExName = "";
                if (order.ServiceItemHistorys.ExtraServiceHistorys != null)
                {
                    int exnum = 0;
                    foreach (var ex in order.ServiceItemHistorys.ExtraServiceHistorys)
                    {
                        if (ex.ServiceNum > 0)
                        {
                            if (exnum > 0)
                            {
                                enExName += ",";
                                cnExName += ",";
                            }
                            enExName += ex.ServiceEnName + " " + ex.ServiceNum + ex.ServiceUnit;
                            cnExName += ex.ServiceName + "：" + ex.ServiceNum + ex.ServiceUnit;
                            exnum++;
                        }
                    }
                }
                //如果有请求变更,就显示变更的值，鼠标移上去可以看到正式的值
                string Changejson = order.ServiceItemHistorys.ChangeElementsValue;
                if (!string.IsNullOrEmpty(Changejson))
                {
                    JObject jo = JObject.Parse(Changejson);
                    foreach (var item in jo)
                    {
                        string change = "";
                        bool bl = false;
                        switch (item.Key)
                        {
                            case "cnName":
                                bl = true;
                                change = order.CustomerName;
                                break;
                            case "enName":
                                bl = true;
                                change = order.CustomerEnname;
                                break;
                            case "Tel":
                                bl = true;
                                change = order.Tel;
                                break;
                            case "BakTel":
                                bl = true;
                                change = order.BakTel;
                                break;
                            case "Email":
                                bl = true;
                                change = order.Email;
                                break;
                            case "Wechat":
                                bl = true;
                                change = order.Wechat;
                                break;
                            case "Adult":
                                bl = true;
                                change = order.ServiceItemHistorys.AdultNum.ToString();
                                break;
                            case "Child":
                                bl = true;
                                change = order.ServiceItemHistorys.ChildNum.ToString();
                                break;
                            case "Infant":
                                bl = true;
                                change = order.ServiceItemHistorys.INFNum.ToString();
                                break;
                            case "NoOfRoom":
                                bl = true;
                                change = order.ServiceItemHistorys.RoomNum.ToString();
                                break;
                            case "Nights":
                                bl = true;
                                change = order.ServiceItemHistorys.RightNum.ToString();
                                break;
                            case "cnAttachedItem":
                                bl = true;
                                change = cnExName;
                                break;
                            case "enAttachedItem":
                                bl = true;
                                change = enExName;
                                break;
                            case "ServiceDate":
                                bl = true;
                                change = order.ServiceItemHistorys.TravelDate < DateTime.Parse("1901-01-01") ? "" : order.ServiceItemHistorys.TravelDate.ToString("yyyy-MM-dd");
                                break;
                            case "BackDate":
                                bl = true;
                                change = order.ServiceItemHistorys.ReturnDate < DateTime.Parse("1901-01-01") ? "" : order.ServiceItemHistorys.ReturnDate.ToString("yyyy-MM-dd");
                                break;
                        }
                        if (bl && change != item.Value.ToString())
                        {
                            change = "原值：" + change;
                            html = html.Replace("#" + item.Key + "#", "<span class='orderchange' data-change='" + change + "'>" + item.Value.ToString() + "</span>");
                        }
                    }
                }
                html = html.Replace("#TripCompany#", order.ServiceItemHistorys.TravelCompany);
                html = html.Replace("#VoucherNo#", order.ServiceItemHistorys.GroupNo);
                html = html.Replace("#PickupTime#", order.ServiceItemHistorys.ReceiveManTime);
                html = html.Replace("#cnName#", order.CustomerName);
                html = html.Replace("#enName#", order.CustomerEnname);
                html = html.Replace("#Tel#", order.Tel);
                html = html.Replace("#BakTel#", order.BakTel);
                html = html.Replace("#Email#", order.Email);
                html = html.Replace("#Wechat#", order.Wechat);
                html = html.Replace("#TBID#", order.Customers.CustomerTBCode);
                html = html.Replace("#cnProductName#", order.ServiceItemHistorys.cnItemName);
                html = html.Replace("#enProductName#", order.ServiceItemHistorys.enItemName);
                html = html.Replace("#ProductCode#", order.ServiceItemHistorys.ServiceCode);
                Supplier supplier = await client.For<Supplier>().Key(order.ServiceItemHistorys.SupplierID).FindEntryAsync();
                html = html.Replace("#SupplyTel#", supplier.ContactWay);
                html = html.Replace("#SupplierName#", supplier.SupplierName);
                html = html.Replace("#SupplierEnName#", supplier.SupplierEnName);

                html = html.Replace("#ServiceDate#", order.ServiceItemHistorys.TravelDate < DateTime.Parse("1901-01-01") ? "" : order.ServiceItemHistorys.TravelDate.ToString("yyyy-MM-dd"));
                html = html.Replace("#BackDate#", order.ServiceItemHistorys.ReturnDate < DateTime.Parse("1901-01-01") ? "" : order.ServiceItemHistorys.ReturnDate.ToString("yyyy-MM-dd"));
                html = html.Replace("#Adult#", order.ServiceItemHistorys.AdultNum.ToString());
                html = html.Replace("#Child#", order.ServiceItemHistorys.ChildNum.ToString());
                html = html.Replace("#Infant#", order.ServiceItemHistorys.INFNum.ToString());
                html = html.Replace("#Nights#", order.ServiceItemHistorys.RightNum.ToString());
                html = html.Replace("#NoOfRoom#", order.ServiceItemHistorys.RoomNum.ToString());
                html = html.Replace("#Remark#", order.Remark);

                html = html.Replace("#enAttachedItem#", enExName);
                html = html.Replace("#cnAttachedItem#", cnExName);

                html = html.Replace("#LHOrderNo#", order.OrderNo);
                html = html.Replace("#TBOrderNo#", order.TBNum);
                string json = order.ServiceItemHistorys.ServiceItemTemplteValue;
                //遍历模板值进行替换
                if (!string.IsNullOrEmpty(json))
                {
                    JObject jo = JObject.Parse(json);
                    foreach (var item in jo)
                    {

                        var type = item.Value.SelectToken("type");
                        if (type != null && type.ToString() == "PersonPicker")
                        {

                            var list = item.Value.SelectToken("value");
                            string PersonStr = "";//拼音（护照号）
                            string PersonStr_M1 = "";
                            string PersonStr_M2 = "";
                            string PersonStr_M3 = "";
                            string PersonStr_D1 = "";
                            string PersonStr_D2 = "";
                            string PersonStr_D3 = "";
                            string PersonStr_D4 = "";
                            string PersonStr_D5 = "";
                            foreach (var one in list)
                            {

                                PersonStr += one.SelectToken("TravellerEnname").ToString() + "(" + one.SelectToken("PassportNo").ToString() + "), ";
                                PersonStr_M1 += one.SelectToken("TravellerEnname").ToString() + "(" + one.SelectToken("PassportNo").ToString() + "," + DateTime.Parse(one.SelectToken("Birthday").ToString()).ToString("yyyy-MM-dd") + "), ";
                                PersonStr_M2 += one.SelectToken("TravellerName").ToString() + "(" + one.SelectToken("TravellerEnname").ToString() + "," + one.SelectToken("PassportNo").ToString() + "," + DateTime.Parse(one.SelectToken("Birthday").ToString()).ToString("yyyy-MM-dd") + "), ";
                                PersonStr_M3 += one.SelectToken("TravellerEnname").ToString() + "(" + one.SelectToken("PassportNo").ToString() + "," + DateTime.Parse(one.SelectToken("Birthday").ToString()).ToString("yyyy-MM-dd") + "," + (one.SelectToken("TravellerSex").ToString() == "0" ? "M" : "F") + "), ";
                                PersonStr_D1 += one.SelectToken("TravellerEnname").ToString() + ", " + one.SelectToken("Height").ToString() + "cm," + one.SelectToken("Weight").ToString() + "kg," + one.SelectToken("ShoesSize").ToString() + "<br/>";
                                PersonStr_D2 += one.SelectToken("TravellerEnname").ToString() + ", " + one.SelectToken("Height").ToString() + "cm," + one.SelectToken("Weight").ToString() + "kg," + one.SelectToken("ShoesSize").ToString() + "," + (one.SelectToken("ClothesSize").ToString() == "" ? "N/A" : one.SelectToken("ClothesSize").ToString()) + "<br/>";
                                PersonStr_D3 += one.SelectToken("TravellerEnname").ToString() + ", " + one.SelectToken("Height").ToString() + "cm," + one.SelectToken("Weight").ToString() + "kg," + one.SelectToken("ShoesSize").ToString() + "," + (one.SelectToken("ClothesSize").ToString() == "" ? "N/A" : one.SelectToken("ClothesSize").ToString()) + "," + (one.SelectToken("TravellerSex").ToString() == "0" ? "M" : "F") + "<br/>";
                                PersonStr_D4 += one.SelectToken("TravellerEnname").ToString() + ", " + one.SelectToken("Height").ToString() + "cm," + one.SelectToken("Weight").ToString() + "kg," + one.SelectToken("ShoesSize").ToString() + "," + (one.SelectToken("ClothesSize").ToString() == "" ? "N/A" : one.SelectToken("ClothesSize").ToString()) + "," + (one.SelectToken("GlassesNum").ToString() == "" ? "N/A" : one.SelectToken("GlassesNum").ToString()) + "<br/>";
                                PersonStr_D5 += one.SelectToken("TravellerEnname").ToString() + ", " + one.SelectToken("Height").ToString() + "cm," + one.SelectToken("Weight").ToString() + "kg," + one.SelectToken("ShoesSize").ToString() + "," + (one.SelectToken("ClothesSize").ToString() == "" ? "N/A" : one.SelectToken("ClothesSize").ToString()) + "," + (one.SelectToken("GlassesNum").ToString() == "" ? "N/A" : one.SelectToken("GlassesNum").ToString()) + "," + (one.SelectToken("TravellerSex").ToString() == "0" ? "M" : "F") + "<br/>";
                            }
                            //查询请求变更模板值，如果有值且不一致，则显示变更的值，鼠标移上去可以看到正式的值
                            if (!string.IsNullOrEmpty(Changejson))
                            {
                                JObject joChange = JObject.Parse(Changejson);
                                var changetoken = joChange.SelectToken("$.." + item.Key);

                                if (changetoken != null)
                                {

                                    var changelist = changetoken.SelectToken("value");
                                    string changePersonStr = "";//拼音（护照号）
                                    string changePersonStr_M1 = "";
                                    string changePersonStr_M2 = "";
                                    string changePersonStr_M3 = "";
                                    string changePersonStr_D1 = "";
                                    string changePersonStr_D2 = "";
                                    string changePersonStr_D3 = "";
                                    string changePersonStr_D4 = "";
                                    string changePersonStr_D5 = "";
                                    foreach (var one in changelist)
                                    {
                                        changePersonStr += one.SelectToken("TravellerEnname").ToString() + "(" + one.SelectToken("PassportNo").ToString() + "),";
                                        changePersonStr_M1 += one.SelectToken("TravellerEnname").ToString() + "(" + one.SelectToken("PassportNo").ToString() + "," + DateTime.Parse(one.SelectToken("Birthday").ToString()).ToString("yyyy-MM-dd") + "), ";
                                        changePersonStr_M2 += one.SelectToken("TravellerName").ToString() + "(" + one.SelectToken("TravellerEnname").ToString() + "," + one.SelectToken("PassportNo").ToString() + "," + DateTime.Parse(one.SelectToken("Birthday").ToString()).ToString("yyyy-MM-dd") + "), ";
                                        changePersonStr_M3 += one.SelectToken("TravellerName").ToString() + "(" + one.SelectToken("TravellerEnname").ToString() + "," + one.SelectToken("PassportNo").ToString() + "," + DateTime.Parse(one.SelectToken("Birthday").ToString()).ToString("yyyy-MM-dd") + "," + (one.SelectToken("TravellerSex").ToString() == "0" ? "M" : "F") + "), ";
                                        changePersonStr_D1 += one.SelectToken("TravellerEnname").ToString() + "," + one.SelectToken("Height").ToString() + "cm," + one.SelectToken("Weight").ToString() + "kg," + one.SelectToken("ShoesSize").ToString() + "<br/>";
                                        changePersonStr_D2 += one.SelectToken("TravellerEnname").ToString() + "," + one.SelectToken("Height").ToString() + "cm," + one.SelectToken("Weight").ToString() + "kg," + one.SelectToken("ShoesSize").ToString() + "," + (one.SelectToken("ClothesSize").ToString() == "" ? "N/A" : one.SelectToken("ClothesSize").ToString()) + "<br/>";
                                        changePersonStr_D3 += one.SelectToken("TravellerEnname").ToString() + "," + one.SelectToken("Height").ToString() + "cm," + one.SelectToken("Weight").ToString() + "kg," + one.SelectToken("ShoesSize").ToString() + "," + (one.SelectToken("ClothesSize").ToString() == "" ? "N/A" : one.SelectToken("ClothesSize").ToString()) + "," + (one.SelectToken("TravellerSex").ToString() == "0" ? "M" : "F") + "<br/>";
                                        changePersonStr_D4 += one.SelectToken("TravellerEnname").ToString() + "," + one.SelectToken("Height").ToString() + "cm," + one.SelectToken("Weight").ToString() + "kg," + one.SelectToken("ShoesSize").ToString() + "," + (one.SelectToken("ClothesSize").ToString() == "" ? "N/A" : one.SelectToken("ClothesSize").ToString()) + "," + (one.SelectToken("GlassesNum").ToString() == "" ? "N/A" : one.SelectToken("GlassesNum").ToString()) + "<br/>";
                                        changePersonStr_D5 += one.SelectToken("TravellerEnname").ToString() + "," + one.SelectToken("Height").ToString() + "cm," + one.SelectToken("Weight").ToString() + "kg," + one.SelectToken("ShoesSize").ToString() + "," + (one.SelectToken("ClothesSize").ToString() == "" ? "N/A" : one.SelectToken("ClothesSize").ToString()) + "," + (one.SelectToken("GlassesNum").ToString() == "" ? "N/A" : one.SelectToken("GlassesNum").ToString()) + "," + (one.SelectToken("TravellerSex").ToString() == "0" ? "M" : "F") + "<br/>";
                                    }

                                    if (changePersonStr_D5 != PersonStr_D5)
                                    {
                                        html = html.Replace("#" + item.Key + "_D5#", "<span class='orderchange' data-change='" + "原值：" + PersonStr_D5 + "'>" + changePersonStr_D5 + "</span>");
                                    }
                                    if (changePersonStr_D4 != PersonStr_D4)
                                    {
                                        html = html.Replace("#" + item.Key + "_D4#", "<span class='orderchange' data-change='" + "原值：" + PersonStr_D4 + "'>" + changePersonStr_D4 + "</span>");
                                    }
                                    if (changePersonStr_D3 != PersonStr_D3)
                                    {
                                        html = html.Replace("#" + item.Key + "_D3#", "<span class='orderchange' data-change='" + "原值：" + PersonStr_D3 + "'>" + changePersonStr_D3 + "</span>");
                                    }
                                    if (changePersonStr_D2 != PersonStr_D2)
                                    {
                                        html = html.Replace("#" + item.Key + "_D2#", "<span class='orderchange' data-change='" + "原值：" + PersonStr_D2 + "'>" + changePersonStr_D2 + "</span>");
                                    }
                                    if (changePersonStr_D1 != PersonStr_D1)
                                    {
                                        html = html.Replace("#" + item.Key + "_D1#", "<span class='orderchange' data-change='" + "原值：" + PersonStr_D1 + "'>" + changePersonStr_D1 + "</span>");
                                    }
                                    if (changePersonStr_M3 != PersonStr_M3)
                                    {
                                        html = html.Replace("#" + item.Key + "_M3#", "<span class='orderchange' data-change='" + "原值：" + PersonStr_M3 + "'>" + changePersonStr_M3 + "</span>");
                                    }
                                    if (changePersonStr_M2 != PersonStr_M2)
                                    {
                                        html = html.Replace("#" + item.Key + "_M2#", "<span class='orderchange' data-change='" + "原值：" + PersonStr_M2 + "'>" + changePersonStr_M2 + "</span>");
                                    }
                                    if (changePersonStr_M1 != PersonStr_M1)
                                    {
                                        html = html.Replace("#" + item.Key + "_M1#", "<span class='orderchange' data-change='" + "原值：" + PersonStr_M1 + "'>" + changePersonStr_M1 + "</span>");
                                    }
                                    if (changePersonStr != PersonStr)
                                    {
                                        html = html.Replace("#" + item.Key + "#", "<span class='orderchange' data-change='" + "原值：" + PersonStr + "'>" + changePersonStr + "</span>");
                                    }
                                }
                            }
                            html = html.Replace("#" + item.Key + "_D5#", PersonStr_D5);
                            html = html.Replace("#" + item.Key + "_D4#", PersonStr_D4);
                            html = html.Replace("#" + item.Key + "_D3#", PersonStr_D3);
                            html = html.Replace("#" + item.Key + "_D2#", PersonStr_D2);
                            html = html.Replace("#" + item.Key + "_D1#", PersonStr_D1);
                            html = html.Replace("#" + item.Key + "_M3#", PersonStr_M3);
                            html = html.Replace("#" + item.Key + "_M2#", PersonStr_M2);
                            html = html.Replace("#" + item.Key + "_M1#", PersonStr_M1);
                            html = html.Replace("#" + item.Key + "#", PersonStr);
                        }

                        else
                        {
                            //查询请求变更模板值，如果有值且不一致，则显示变更的值，鼠标移上去可以看到正式的值
                            if (!string.IsNullOrEmpty(Changejson))
                            {
                                JObject joChange = JObject.Parse(Changejson);
                                var changetoken = joChange.SelectToken("$.." + item.Key);
                                if (changetoken != null)
                                {
                                    if (changetoken.ToString() != item.Value.ToString())
                                    {
                                        html = html.Replace("#" + item.Key + "#", "<span class='orderchange' data-change='" + "原值：" + item.Value.ToString() + "'>" + changetoken.ToString() + "</span>");
                                    }
                                }
                            }
                            html = html.Replace("#" + item.Key + "#", item.Value.ToString());
                        }
                    }
                }
            }
            catch (IOException e)
            {
                html = e.Message;
            }

            return html.Trim().Replace("﻿", "");
        }

        //发送邮件
        [HttpPost]
        public async Task<string> SendMail(int OrderID, string title, string toMail, string customerName, string fileName, string filePath, string type = "customer")
        {
            Order order;
            try
            {
                order = await client.For<Order>().Key(OrderID).FindEntryAsync();
            }
            catch
            {
                return JsonConvert.SerializeObject(new { result = false, statusCode = 401, message = "找不到该订单！", info = "" });
            }
            if (order == null)
            {
                return JsonConvert.SerializeObject(new { result = false, statusCode = 401, message = "找不到订单！", info = "" });
            }
            if (string.IsNullOrEmpty(toMail))
            {
                return JsonConvert.SerializeObject(new { result = false, statusCode = 401, message = "收件人不能为空！", info = "" });
            }
            if (string.IsNullOrEmpty(title))
            {
                return JsonConvert.SerializeObject(new { result = false, statusCode = 401, message = "标题不能为空！", info = "" });
            }
            if (string.IsNullOrEmpty(customerName))
            {
                return JsonConvert.SerializeObject(new { result = false, statusCode = 401, message = "客户名称不能为空！", info = "" });
            }
            if (string.IsNullOrEmpty(fileName))
            {
                return JsonConvert.SerializeObject(new { result = false, statusCode = 401, message = "附件名称不能为空！", info = "" });
            }
            if (string.IsNullOrEmpty(filePath) || !System.IO.File.Exists(filePath))
            {
                return JsonConvert.SerializeObject(new { result = false, statusCode = 401, message = "发送失败！找不到附件", info = "" });
            }
            string template = "";
            if (type == "supplier")
            {
                template = "bookingmail";
            }
            else if (type == "customer")
            {
                template = "confirmmail";
            }
            else
            {
                return JsonConvert.SerializeObject(new { result = false, statusCode = 401, message = "发送失败！类型错误", info = "" });
            }
            //发送邮件

            string result = await EmailHelper.send(title, toMail, order.CustomerName, fileName, filePath, template);
            string message = "";
            try
            {
                JObject jo = JObject.Parse(result);
                message = jo.SelectToken("$..statusCode").ToString() == "200" ? "成功" : "失败";
            }
            catch
            {
                message = "失败";
            }
            string userName = User.Identity.Name;
            User user = await client.For<User>().Filter(u => u.UserName == userName).FindEntryAsync();
            await client.For<OrderHistory>().Set(new { OrderID = order.OrderID, OperUserID = user.UserID.ToString(), OperUserNickName = user.NickName, OperTime = DateTime.Now, State = order.state, Remark = "发送邮件" + message }).InsertEntryAsync();
            try
            {
                //删除附件
                System.IO.File.Delete(filePath);
            }
            catch { }
            return result;
        }
        //发送邮件前先保存图片，然后作为附件发送
        [HttpPost]
        public string UploadPic(string imageData, string imageName)
        {
            string savePath = System.Web.HttpContext.Current.Server.MapPath("~/App_Data/");
            if (!Directory.Exists(savePath))//判断上传文件夹是否存在，若不存在，则创建
            {
                Directory.CreateDirectory(savePath);//创建文件夹
            }
            string Pic_Path = System.Web.HttpContext.Current.Server.MapPath("~/App_Data/" + imageName);
            using (FileStream fs = new FileStream(Pic_Path, FileMode.Create))
            {
                using (BinaryWriter bw = new BinaryWriter(fs))
                {
                    byte[] data = Convert.FromBase64String(imageData);
                    bw.Write(data);
                    bw.Close();
                }
            }
            return JsonConvert.SerializeObject(new { ErrorCode = 200, ErrorMessage = "OK", Pic_Path });
        }

        //先保存，然后下载
        [HttpPost]
        public FileResult DownFile(string fileData, string fileName)
        {
            string folderPath = "~/data/OrderData/" + DateTime.Now.ToString("yyyyMM") + "/";
            string filePhysicalPath = Server.MapPath(folderPath);
            if (!Directory.Exists(filePhysicalPath))//判断上传文件夹是否存在，若不存在，则创建
            {
                Directory.CreateDirectory(filePhysicalPath);//创建文件夹
            }
            string filePath = System.Web.HttpContext.Current.Server.MapPath(folderPath + fileName);
            using (FileStream fs = new FileStream(filePath, FileMode.Create))
            {
                using (BinaryWriter bw = new BinaryWriter(fs))
                {
                    byte[] data = Convert.FromBase64String(fileData);
                    bw.Write(data);
                    bw.Close();
                }
            }
            return File(new FileStream(filePath, FileMode.Open), "application/octet-stream", Server.UrlEncode(fileName));
        }
        /// <summary>
        /// 打印
        /// </summary>
        /// <param name="id">订单编号</param>
        /// <param name="tipsWidth">宽度</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<string> Print(int? id, string tipsWidth)
        {
            Order order = await GetOrderByID(id);
            string detailHtml = await GetConfimOrderHtml(id, order);

            string htmlPath = string.Empty;
            //使用http的URL
            string url = OfficeHelper.CreateHtml(detailHtml, ref htmlPath);
            url = this.Request.Url.Authority + url;
            if (!string.IsNullOrEmpty(tipsWidth))
            {
                url = url + "?tipsWidth=" + tipsWidth;
            }
            string pdfDirectory = Server.MapPath("~/data/Pdf/");
            OfficeHelper.CreateDirectory(pdfDirectory);
            string pdfPhyName = DateTime.Now.ToString("yyyyMMddhhmmssfffff") + ".pdf";
            string pdfPath = pdfDirectory + pdfPhyName;
            string urlPath = "http://" + this.Request.Url.Authority + "/data/Pdf/" + pdfPhyName;
            OfficeHelper.DelteAgoFiles(pdfPath);
            using (IPechkin pechkin = Factory.Create(new GlobalConfig()))
            {
                ObjectConfig oc = new ObjectConfig();
                oc.SetPrintBackground(true)
                    .SetLoadImages(true)
                    .SetScreenMediaType(true)
                    .SetPageUri(url);
                byte[] pdf = pechkin.Convert(oc);
                System.IO.File.WriteAllBytes(pdfPath, pdf);
                OfficeHelper.DeleteFile(htmlPath);
                return JsonConvert.SerializeObject(new { ErrorCode = 200, ErrorMessage = "OK", UrlPath = urlPath });
            }
        }
        public async Task<FileResult> DownloadPdf(int? id, string fileName, string isNotes, string isCancel, string supplierenname)
        {
            Order order = await GetOrderByID(id);
            string detailHtml = await GetConfimOrderHtml(id, order);

            string htmlPath = string.Empty;
            //使用http的URL
            string url = OfficeHelper.CreateHtml(detailHtml, ref htmlPath);
            url = this.Request.Url.Authority + url;
            url = url + "?isNotes=" + isNotes + "&isCancel=" + isCancel + "&supEnName=" + supplierenname;
            using (IPechkin pechkin = Factory.Create(new GlobalConfig()))
            {
                ObjectConfig oc = new ObjectConfig();
                oc.SetPrintBackground(true)
                    .SetLoadImages(true)
                    .SetScreenMediaType(true)
                    .SetPageUri(url);
                byte[] pdf = pechkin.Convert(oc);
                OfficeHelper.DeleteFile(htmlPath);
                return File(pdf, "application/pdf", Server.UrlDecode(fileName));
            }
        }
        public async Task<FileResult> DownloadWord(int? id, string fileName, string isNotes, string isCancel, string supplierenname)
        {
            Order order = await GetOrderByID(id);
            string detailHtml = await GetConfimOrderHtml(id, order);
            //html文件物理地址
            string htmlPath = string.Empty;
            //使用http的URL
            string url = OfficeHelper.CreateHtml(detailHtml, ref htmlPath);
            url = "http://" + this.Request.Url.Authority + url;
            url = url + "?isNotes=" + isNotes + "&isCancel=" + isCancel + "&supEnName=" + supplierenname;
            string Html = string.Empty;
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            using (StreamReader Reader = new StreamReader(req.GetResponse().GetResponseStream(), Encoding.Default))
            {
                Html = await Reader.ReadToEndAsync();
                if (isNotes == "false")
                {
                    Regex re = new Regex(@"<div\sid=""notes"".*?</div>", RegexOptions.CultureInvariant);
                    string str = re.Match(Html).ToString();
                    Html = Html.Replace(str, "");
                }
                if (isCancel == "false")
                {
                    Regex re = new Regex(@"<div\sid=""cancels"".*?</div>", RegexOptions.CultureInvariant);
                    string str2 = re.Match(Html).ToString();
                    Html = Html.Replace(str2, "");
                }
                if (!string.IsNullOrEmpty(supplierenname))
                {
                    Regex re = new Regex(@"<span\sid=""supEnName"".*?</span>", RegexOptions.CultureInvariant);
                    string str3 = re.Match(Html).ToString();
                    Html = Html.Replace(str3, Server.UrlDecode(supplierenname));
                }
            }
            //word文件路径
            string wordPath = System.Web.HttpContext.Current.Server.MapPath("~/data/PdfHtml/") + Guid.NewGuid().ToString() + ".docx";
            // 写入到服务器端 
            var doc = new Aspose.Words.Document();
            DocumentBuilder builder = new DocumentBuilder(doc);
            Aspose.Words.Font font = builder.Font;
            font.Name = "Microsoft YaHei";
            ParagraphFormat paragraphFormat = builder.ParagraphFormat;
            paragraphFormat.FirstLineIndent = 8;
            paragraphFormat.Alignment = ParagraphAlignment.Justify;
            paragraphFormat.KeepTogether = true;

            builder.InsertHtml(Html.Replace("../..", Server.MapPath("~")), true);

            doc.Save(wordPath);
            // 写入内存,返回给客户端
            using (MemoryStream ms = OfficeHelper.WriteToClient(wordPath))
            {
                OfficeHelper.DeleteFile(htmlPath);
                OfficeHelper.DeleteFile(wordPath);
                return File(ms.ToArray(), "application/ms-word", Server.UrlDecode(fileName));
            }
        }
        // GET: Orders/Create
        public async Task<ActionResult> Create()
        {
            OrderViewModel ov = new OrderViewModel();
            ov.OrderSourse = new List<OrderSourse>();
            ov.OrderSourse = await client.For<OrderSourse>()
                .Filter(t => t.OrderSourseEnableState == EnableState.Enable)
                .OrderBy(t => t.ShowNo)
                .FindEntriesAsync();
            return View(ov);
        }
        //获取产品列表和对应供应商
        [HttpPost]
        public async Task<string> GetItemsByStr(string Str)
        {
            try
            {
                OrderViewModel ov = new OrderViewModel();
                ov.ErrorCode = 200;
                ov.ErrorMessage = "OK";
                var Result = await client.For<ServiceItem>().Expand(t => t.ItemSuplier)
                    .Filter(t => t.ServiceCode.Contains(Str) || t.cnItemName.Contains(Str) || t.enItemName.Contains(Str))
                    .Filter(t => t.ServiceItemEnableState == EnableState.Enable)
                    .Filter(t => t.ElementContent != null)
                    .OrderBy(t => t.ServiceItemID).Top(15)
                    .FindEntriesAsync();
                //产品列表
                List<ServiceItemSupplierView> Items = new List<ServiceItemSupplierView>();
                foreach (ServiceItem r in Result)
                {
                    if (r.ItemSuplier != null)
                    {
                        //供应商列表
                        List<SupplierView> Suppliers = new List<SupplierView>();
                        var ServiceItemViewconfig = new MapperConfiguration(cfg => cfg.CreateMap<ServiceItem, ServiceItemSupplierView>());
                        var ServiceItemViewmapper = ServiceItemViewconfig.CreateMapper();
                        ServiceItemSupplierView Item = ServiceItemViewmapper.Map<ServiceItemSupplierView>(r);
                        foreach (Supplier s in r.ItemSuplier)
                        {
                            if (s.SupplierEnableState == 0)
                            {
                                //只显示已维护价格的供应商
                                SupplierServiceItem supplierServiceItem = await client.For<SupplierServiceItem>()
                                    .Filter(t => t.ServiceItemID == r.ServiceItemID & t.SupplierID == s.SupplierID)
                                    .FindEntryAsync();
                                if (supplierServiceItem != null)
                                {
                                    SupplierServiceItemChange change = await client.For<SupplierServiceItemChange>()
                                        .Filter(t => t.ServiceItemID == r.ServiceItemID & t.SupplierID == s.SupplierID)
                                        .FindEntryAsync();
                                    if (supplierServiceItem.IsChange && change == null)
                                    {
                                        //首次填写价格未确认之前不显示
                                    }
                                    else
                                    {
                                        var SupplierViewconfig = new MapperConfiguration(cfg => cfg.CreateMap<Supplier, SupplierView>());
                                        var SupplierViewmapper = SupplierViewconfig.CreateMapper();
                                        //将Supplier的内容映射到SupplierView
                                        SupplierView Supplier = SupplierViewmapper.Map<SupplierView>(s);
                                        Suppliers.Add(Supplier);
                                    }
                                }
                            }
                        }
                        Item.ItemSupliers = Suppliers.OrderBy(s => s.SupplierNo).ToList();
                        Items.Add(Item);
                    }
                }
                ov.Items = Items;
                return JsonConvert.SerializeObject(new { ov.ErrorCode, ov.ErrorMessage, ov.Items });
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 400, ErrorMessage = ex });
            }
        }
        //获取产品信息、额外服务、供应商价格信息
        [HttpPost]
        public async Task<string> GetItemById(int ItemID, int SupplierID)
        {
            OrderViewModel ov = new OrderViewModel();
            try
            {
                #region 查询数据
                //获取产品信息
                ServiceItem ItemResult = await client.For<ServiceItem>()
                    .Expand(p => p.ExtraServices)
                    .Expand(p => p.ItemServiceType)
                    .Key(ItemID).FindEntryAsync();
                //获取产品供应商价格
                SupplierServiceItem SupplierServiceItemResult = await client.For<SupplierServiceItem>()
                    .Expand(p => p.ItemCurrency)
                    .Expand(p => p.ItemPriceBySuppliers)
                    .Expand(p => p.Service)
                    .Expand(p => p.ExtraServicePrices)
                    .Filter(p => p.ServiceItemID == ItemID & p.SupplierID == SupplierID)
                    .FindEntryAsync();
                //获取供应商信息
                Supplier SupplierResult = await client.For<Supplier>().Key(SupplierID).FindEntryAsync();
                if (ItemResult != null)
                {
                    if (SupplierResult != null)
                    {
                        //获取产品信息，将linq对象映射为实体类
                        ServiceItemView ServiceItem = new ServiceItemView();
                        var ServiceItemViewconfig = new MapperConfiguration(cfg => cfg.CreateMap<ServiceItem, ServiceItemView>());
                        var ServiceItemViewmapper = ServiceItemViewconfig.CreateMapper();
                        ServiceItem = ServiceItemViewmapper.Map<ServiceItemView>(ItemResult);
                        //产品类别
                        ServiceTypeView ServiceType = new ServiceTypeView();
                        ServiceType.ServiceTypeID = ItemResult.ServiceTypeID;
                        ServiceType.ServiceTypeName = ItemResult.ItemServiceType.ServiceTypeName;
                        //供应商
                        SupplierView Supplier = new SupplierView();
                        var SupplierViewconfig = new MapperConfiguration(cfg => cfg.CreateMap<Supplier, SupplierView>());
                        var SupplierViewmapper = SupplierViewconfig.CreateMapper();
                        Supplier = SupplierViewmapper.Map<SupplierView>(SupplierResult);
                        //产品供应商价格库
                        SupplierServiceItemView SupplierServiceItem = new SupplierServiceItemView();
                        //产品供应商对应时间段价格
                        ItemPriceBySupplier ItemPriceBySuppliers = new ItemPriceBySupplier();
                        //额外服务列表
                        List<ExtraServiceView> ExtraServices = new List<ExtraServiceView>();
                        //获取产品供应商价格信息
                        if (SupplierServiceItemResult != null)
                        {
                            //默认兑换方式为人民币对外币。
                            ChangeType ChangeType = ChangeType.FromChina;
                            //默认汇率为1
                            float Rate = 1;
                            var SupplierServiceItemViewconfig = new MapperConfiguration(cfg => cfg.CreateMap<SupplierServiceItem, SupplierServiceItemView>());
                            var SupplierServiceItemViewmapper = SupplierServiceItemViewconfig.CreateMapper();
                            SupplierServiceItem = SupplierServiceItemViewmapper.Map<SupplierServiceItemView>(SupplierServiceItemResult);
                            //获取供应商币别
                            if (SupplierServiceItemResult.ItemCurrency != null)
                            {
                                ChangeType = SupplierServiceItemResult.ItemCurrency.CurrencyChangeType;
                                Rate = SupplierServiceItemResult.ItemCurrency.ExchangeRate;
                            }
                            //获取产品供应商对应时间价格，取当前时间
                            if (SupplierServiceItemResult.ItemPriceBySuppliers != null)
                            {
                                foreach (var r in SupplierServiceItemResult.ItemPriceBySuppliers)
                                {
                                    if (r.startTime < DateTime.Now && r.EndTime.AddDays(1) > DateTime.Now)
                                    {
                                        //根据兑换方式和汇率将价格转换成RMB
                                        if (ChangeType == ChangeType.FromChina)
                                        {
                                            r.AdultNetPrice = r.AdultNetPrice / Rate;
                                            r.BobyNetPrice = r.BobyNetPrice / Rate;
                                            r.ChildNetPrice = r.ChildNetPrice / Rate;
                                            r.Price = r.Price / Rate;
                                        }
                                        else if (ChangeType == ChangeType.ToChina)
                                        {
                                            r.AdultNetPrice = r.AdultNetPrice * Rate;
                                            r.BobyNetPrice = r.BobyNetPrice * Rate;
                                            r.ChildNetPrice = r.ChildNetPrice * Rate;
                                            r.Price = r.Price * Rate;
                                        }
                                        ItemPriceBySuppliers = r;
                                        break;
                                    }
                                }
                            }
                            //额外服务&价格
                            if (ItemResult.ExtraServices != null)
                            {
                                foreach (var r in ItemResult.ExtraServices)
                                {
                                    ExtraServiceView ExtraService = new ExtraServiceView();
                                    ExtraServicePriceView ExtraServicePrice = new ExtraServicePriceView();
                                    var ExtraServiceViewconfig = new MapperConfiguration(cfg => cfg.CreateMap<ExtraService, ExtraServiceView>());
                                    var ExtraServiceViewmapper = ExtraServiceViewconfig.CreateMapper();
                                    ExtraService = ExtraServiceViewmapper.Map<ExtraServiceView>(r);
                                    if (SupplierServiceItemResult.ExtraServicePrices != null)
                                    {
                                        //额外服务价格
                                        foreach (var re in SupplierServiceItemResult.ExtraServicePrices)
                                        {
                                            ExtraServicePrice exPrice = await client.For<ExtraServicePrice>()
                                                .Expand(u => u.Service)
                                                .Key(re.ExtraServicePriceID)
                                                .FindEntryAsync();
                                            if (exPrice.Service != null)
                                            {
                                                if (r.ExtraServiceID == exPrice.Service.ExtraServiceID)
                                                {
                                                    //根据兑换方式和汇率将价格转换成RMB
                                                    if (ChangeType == ChangeType.FromChina)
                                                    {
                                                        ExtraServicePrice.ServicePrice = exPrice.ServicePrice / Rate;
                                                    }
                                                    else if (ChangeType == ChangeType.ToChina)
                                                    {
                                                        ExtraServicePrice.ServicePrice = exPrice.ServicePrice * Rate;
                                                    }
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                    ExtraService.ExtraServicePrices = ExtraServicePrice;
                                    ExtraServices.Add(ExtraService);
                                }
                            }
                        }
                        SupplierServiceItem.ItemPriceBySupplier = ItemPriceBySuppliers;
                        ServiceItem.ItemServiceTypes = ServiceType;
                        ServiceItem.ExtraService = ExtraServices;
                        ServiceItem.ItemSupliers = Supplier;
                        ServiceItem.SupplierServiceItemView = SupplierServiceItem;

                        ov.ErrorCode = 200;
                        ov.ErrorMessage = "OK";
                        ov.Item = ServiceItem;
                    }
                    else
                    {
                        ov.ErrorCode = 401;
                        ov.ErrorMessage = "没有该供应商";
                    }
                }
                else
                {
                    ov.ErrorCode = 401;
                    ov.ErrorMessage = "没有该产品";
                }
                return JsonConvert.SerializeObject(new { ov.ErrorCode, ov.ErrorMessage, ov.Item });
                #endregion
            }
            catch (Exception ex)
            {
                ov.ErrorCode = 400;
                ov.ErrorMessage = ex.Message;
                return JsonConvert.SerializeObject(new { ov.ErrorCode, ov.ErrorMessage });
            }
        }
        //获取产品信息、额外服务、供应商价格信息
        [HttpPost]
        public async Task<string> GetItemByCode(string Code)
        {
            if (string.IsNullOrEmpty(Code))
            {
                return "[" + JsonConvert.SerializeObject(new { ErrorCode = "401", ErrorMessage = "Code不能为空", Code = Code }) + "]";
            }
            string json = "";
            foreach (var one in Code.Split('^'))
            {
                if (json == "")
                {
                    json += "[";
                }
                else
                {
                    json += ",";
                }
                var item = await client.For<ServiceItem>()
                    .Filter(s => s.ServiceCode == one && s.ServiceItemEnableState == EnableState.Enable)
                    .FindEntryAsync();
                if (item == null)
                {
                    json += JsonConvert.SerializeObject(new { ErrorCode = "401", ErrorMessage = "找不到产品编码：" + one, Code = one });
                }
                else
                {
                    try
                    {
                        #region 查询数据
                        //获取产品信息
                        ServiceItem ItemResult = await client.For<ServiceItem>()
                            .Expand(p => p.ExtraServices)
                            .Expand(p => p.ItemServiceType)
                            .Key(item.ServiceItemID).FindEntryAsync();
                        //获取产品供应商价格
                        SupplierServiceItem SupplierServiceItemResult = await client.For<SupplierServiceItem>()
                            .Expand(p => p.ItemCurrency)
                            .Expand(p => p.ItemPriceBySuppliers)
                            .Expand(p => p.Service)
                            .Expand(p => p.ExtraServicePrices)
                            .Filter(p => p.ServiceItemID == item.ServiceItemID & p.SupplierID == item.DefaultSupplierID)
                            .FindEntryAsync();
                        //获取供应商信息
                        Supplier SupplierResult = await client.For<Supplier>().Key(item.DefaultSupplierID).FindEntryAsync();
                        if (ItemResult != null)
                        {
                            if (SupplierResult != null)
                            {
                                //获取产品信息，将linq对象映射为实体类
                                ServiceItemView ServiceItem = new ServiceItemView();
                                var ServiceItemViewconfig = new MapperConfiguration(cfg => cfg.CreateMap<ServiceItem, ServiceItemView>());
                                var ServiceItemViewmapper = ServiceItemViewconfig.CreateMapper();
                                ServiceItem = ServiceItemViewmapper.Map<ServiceItemView>(ItemResult);
                                //产品类别
                                ServiceTypeView ServiceType = new ServiceTypeView();
                                ServiceType.ServiceTypeID = ItemResult.ServiceTypeID;
                                ServiceType.ServiceTypeName = ItemResult.ItemServiceType.ServiceTypeName;
                                //供应商
                                SupplierView Supplier = new SupplierView();
                                var SupplierViewconfig = new MapperConfiguration(cfg => cfg.CreateMap<Supplier, SupplierView>());
                                var SupplierViewmapper = SupplierViewconfig.CreateMapper();
                                Supplier = SupplierViewmapper.Map<SupplierView>(SupplierResult);
                                //产品供应商价格库
                                SupplierServiceItemView SupplierServiceItem = new SupplierServiceItemView();
                                //产品供应商对应时间段价格
                                ItemPriceBySupplier ItemPriceBySuppliers = new ItemPriceBySupplier();
                                //额外服务列表
                                List<ExtraServiceView> ExtraServices = new List<ExtraServiceView>();
                                //获取产品供应商价格信息
                                if (SupplierServiceItemResult != null)
                                {
                                    //默认兑换方式为人民币对外币。
                                    ChangeType ChangeType = ChangeType.FromChina;
                                    //默认汇率为1
                                    float Rate = 1;
                                    var SupplierServiceItemViewconfig = new MapperConfiguration(cfg => cfg.CreateMap<SupplierServiceItem, SupplierServiceItemView>());
                                    var SupplierServiceItemViewmapper = SupplierServiceItemViewconfig.CreateMapper();
                                    SupplierServiceItem = SupplierServiceItemViewmapper.Map<SupplierServiceItemView>(SupplierServiceItemResult);
                                    //获取供应商币别
                                    if (SupplierServiceItemResult.ItemCurrency != null)
                                    {
                                        ChangeType = SupplierServiceItemResult.ItemCurrency.CurrencyChangeType;
                                        Rate = SupplierServiceItemResult.ItemCurrency.ExchangeRate;
                                    }
                                    //获取产品供应商对应时间价格，取当前时间
                                    if (SupplierServiceItemResult.ItemPriceBySuppliers != null)
                                    {
                                        foreach (var r in SupplierServiceItemResult.ItemPriceBySuppliers)
                                        {
                                            if (r.startTime < DateTime.Now && r.EndTime.AddDays(1) > DateTime.Now)
                                            {
                                                //根据兑换方式和汇率将价格转换成RMB
                                                if (ChangeType == ChangeType.FromChina)
                                                {
                                                    r.AdultNetPrice = r.AdultNetPrice / Rate;
                                                    r.BobyNetPrice = r.BobyNetPrice / Rate;
                                                    r.ChildNetPrice = r.ChildNetPrice / Rate;
                                                    r.Price = r.Price / Rate;
                                                }
                                                else if (ChangeType == ChangeType.ToChina)
                                                {
                                                    r.AdultNetPrice = r.AdultNetPrice * Rate;
                                                    r.BobyNetPrice = r.BobyNetPrice * Rate;
                                                    r.ChildNetPrice = r.ChildNetPrice * Rate;
                                                    r.Price = r.Price * Rate;
                                                }
                                                ItemPriceBySuppliers = r;
                                                break;
                                            }
                                        }
                                    }
                                    //额外服务&价格
                                    if (ItemResult.ExtraServices != null)
                                    {
                                        foreach (var r in ItemResult.ExtraServices)
                                        {
                                            ExtraServiceView ExtraService = new ExtraServiceView();
                                            ExtraServicePriceView ExtraServicePrice = new ExtraServicePriceView();
                                            var ExtraServiceViewconfig = new MapperConfiguration(cfg => cfg.CreateMap<ExtraService, ExtraServiceView>());
                                            var ExtraServiceViewmapper = ExtraServiceViewconfig.CreateMapper();
                                            ExtraService = ExtraServiceViewmapper.Map<ExtraServiceView>(r);
                                            if (SupplierServiceItemResult.ExtraServicePrices != null)
                                            {
                                                //额外服务价格
                                                foreach (var re in SupplierServiceItemResult.ExtraServicePrices)
                                                {
                                                    ExtraServicePrice exPrice = await client.For<ExtraServicePrice>()
                                                        .Expand(u => u.Service)
                                                        .Key(re.ExtraServicePriceID)
                                                        .FindEntryAsync();
                                                    if (exPrice.Service != null)
                                                    {
                                                        if (r.ExtraServiceID == exPrice.Service.ExtraServiceID)
                                                        {
                                                            //根据兑换方式和汇率将价格转换成RMB
                                                            if (ChangeType == ChangeType.FromChina)
                                                            {
                                                                ExtraServicePrice.ServicePrice = exPrice.ServicePrice / Rate;
                                                            }
                                                            else if (ChangeType == ChangeType.ToChina)
                                                            {
                                                                ExtraServicePrice.ServicePrice = exPrice.ServicePrice * Rate;
                                                            }
                                                            break;
                                                        }
                                                    }
                                                }
                                            }
                                            ExtraService.ExtraServicePrices = ExtraServicePrice;
                                            ExtraServices.Add(ExtraService);
                                        }
                                    }
                                }
                                SupplierServiceItem.ItemPriceBySupplier = ItemPriceBySuppliers;
                                ServiceItem.ItemServiceTypes = ServiceType;
                                ServiceItem.ExtraService = ExtraServices;
                                ServiceItem.ItemSupliers = Supplier;
                                ServiceItem.SupplierServiceItemView = SupplierServiceItem;

                                json += JsonConvert.SerializeObject(new { ErrorCode = "200", ErrorMessage = "OK", Item = ServiceItem, Code = one });
                            }
                            else
                            {
                                json += JsonConvert.SerializeObject(new { ErrorCode = "401", ErrorMessage = "没有该供应商", Code = one });
                            }
                        }
                        else
                        {
                            json += JsonConvert.SerializeObject(new { ErrorCode = "401", ErrorMessage = "没有该产品", Code = one });
                        }
                        #endregion
                    }
                    catch (Exception ex)
                    {
                        json += JsonConvert.SerializeObject(new { ErrorCode = "400", ErrorMessage = ex.Message, Code = one });
                    }
                }
            }
            if (json == "")
            {
                return "[]";
            }
            else
            {
                json = json + "]";
                return json;
            }
        }
        //获取订单列表
        public async Task<string> GetOrders(ShareSearchModel search)
        {
            int draw = 1;
            int start = 0;
            int length = 10;
            string propertyName = "OrderID";
            int sort = 0;
            string status = string.Empty;
            if (search.length > 0)
            {
                draw = search.draw;
                start = search.start;
                length = search.length;
                if (search.OrderBy != null)
                {
                    propertyName = search.OrderBy.PropertyName == null ? propertyName : search.OrderBy.PropertyName;
                    sort = search.OrderBy.OrderBy;
                }
            }
            IBoundClient<Order> OrderResult = client.For<Order>()
                .Expand(t => t.TBOrders)
                .Expand(t => t.TBOrders.Sourse)
                .Expand(t => t.ServiceItemHistorys)
                .Skip(start).Top(length);
            IBoundClient<Order> OrderCountResult = client.For<Order>().Expand(t => t.Customers);
            if (propertyName == "TravelDate")
            {
                OrderResult = sort == 0 ? OrderResult.OrderByDescending("ServiceItemHistorys/TravelDate") : OrderResult.OrderBy("ServiceItemHistorys/TravelDate");
            }
            else
            {
                OrderResult = sort == 0 ? OrderResult.OrderByDescending(propertyName) : OrderResult.OrderBy(propertyName);
            }
            if (search.OrderSearch != null)
            {
                status = search.OrderSearch.status;
                if (!string.IsNullOrEmpty(status))
                {//状态，以逗号分开
                    string[] state = status.Trim().Split(',');
                    if (state.Count() > 1)
                    {
                        string filter = "";
                        foreach (var item in state)
                        {
                            if (filter != "")
                            {
                                filter += " or ";
                            }
                            filter += "state eq LanghuaNew.Data.OrderState'" + item + "'";
                        }
                        OrderResult = OrderResult.Filter(filter);
                        OrderCountResult = OrderCountResult.Filter(filter);
                    }
                    else
                    {
                        int statuss = string.IsNullOrEmpty(status) ? -1 : int.Parse(status);
                        if (statuss != -1)
                        {
                            string filter = "state eq LanghuaNew.Data.OrderState'" + statuss + "'";
                            OrderResult = OrderResult.Filter(filter);
                            OrderCountResult = OrderCountResult.Filter(filter);
                        }
                    }
                }
                if (!string.IsNullOrEmpty(search.OrderSearch.FuzzySearch))
                {//姓名、电话、订单号、淘宝ID、备注、产品中文名
                    search.OrderSearch.FuzzySearch = search.OrderSearch.FuzzySearch.Trim();
                    string filter = "";
                    switch (search.OrderSearch.FuzzySearchType)
                    {
                        case "TBID":
                            filter = "contains(TBOrders/TBID,'" + search.OrderSearch.FuzzySearch + "')";
                            break;
                        case "CustomerName":
                            filter = "contains(CustomerName,'" + search.OrderSearch.FuzzySearch + "')";
                            break;
                        case "OrderNo":
                            filter = "contains(OrderNo,'" + search.OrderSearch.FuzzySearch + "')";
                            break;
                        case "Remark":
                            filter = "contains(Remark,'" + search.OrderSearch.FuzzySearch + "')";
                            break;
                        case "Tel":
                            filter = "contains(Tel,'" + search.OrderSearch.FuzzySearch + "') or contains(BakTel,'" + search.OrderSearch.FuzzySearch + "')";
                            break;
                        case "cnItemName":
                            filter = "contains(ServiceItemHistorys/cnItemName,'" + search.OrderSearch.FuzzySearch + "')";
                            break;
                        default:
                            filter = "contains(CustomerName,'" + search.OrderSearch.FuzzySearch
                                   + "') or contains(Tel,'" + search.OrderSearch.FuzzySearch
                                   + "') or contains(BakTel,'" + search.OrderSearch.FuzzySearch
                                   + "') or contains(OrderNo,'" + search.OrderSearch.FuzzySearch
                                   + "') or contains(TBOrders/TBID,'" + search.OrderSearch.FuzzySearch
                                   + "') or contains(Remark,'" + search.OrderSearch.FuzzySearch
                                   + "') or contains(ServiceItemHistorys/cnItemName,'" + search.OrderSearch.FuzzySearch + "')";
                            break;
                    }
                    OrderResult = OrderResult.Filter(filter);
                    OrderCountResult = OrderCountResult.Filter(filter);
                }
                if (!string.IsNullOrEmpty(search.OrderSearch.OrderCreateDateBegin))
                {//订单创建时间
                    string filter = "CreateTime ge " + search.OrderSearch.OrderCreateDateBegin.Trim();
                    //filter = "OrderSupplierHistorys/any(x1:x1/opera eq LanghuaNew.Data.OrderOperator'supplier' and x1/OperTime ge " + search.OrderSearch.OrderCreateDateBegin.Trim() + ")";
                    OrderResult = OrderResult.Filter(filter);
                    OrderCountResult = OrderCountResult.Filter(filter);
                }
                if (!string.IsNullOrEmpty(search.OrderSearch.OrderCreateDateEnd))
                {
                    try
                    {
                        DateTime endTime = DateTime.Parse(search.OrderSearch.OrderCreateDateEnd.Trim()).AddDays(1);
                        string filter = "CreateTime lt " + endTime.ToString("yyyy-MM-dd");
                        //filter = "OrderSupplierHistorys/any(x1:x1/opera eq LanghuaNew.Data.OrderOperator'supplier' and x1/OperTime lt " + endTime.ToString("yyyy-MM-dd") + ")";
                        OrderResult = OrderResult.Filter(filter);
                        OrderCountResult = OrderCountResult.Filter(filter);
                    }
                    catch { }
                }

                if (search.OrderSearch.IsChangeTravelDate == "true")
                {//只查变更中的出行日期
                    if (!string.IsNullOrEmpty(search.OrderSearch.TravelDateBegin))
                    {//出行时间
                        string filter = "ServiceItemHistorys/ChangeTravelDate ge " + search.OrderSearch.TravelDateBegin.Trim();
                        OrderResult = OrderResult.Filter(filter);
                        OrderCountResult = OrderCountResult.Filter(filter);
                    }
                    if (!string.IsNullOrEmpty(search.OrderSearch.TravelDateEnd))
                    {
                        try
                        {
                            DateTime endTime = DateTime.Parse(search.OrderSearch.TravelDateEnd.Trim()).AddDays(1);
                            string filter = "ServiceItemHistorys/ChangeTravelDate lt " + endTime.ToString("yyyy-MM-dd");
                            OrderResult = OrderResult.Filter(filter);
                            OrderCountResult = OrderCountResult.Filter(filter);
                        }
                        catch { }
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(search.OrderSearch.TravelDateBegin))
                    {//出行时间
                        string filter = "ServiceItemHistorys/TravelDate ge " + search.OrderSearch.TravelDateBegin.Trim();
                        OrderResult = OrderResult.Filter(filter);
                        OrderCountResult = OrderCountResult.Filter(filter);
                    }
                    if (!string.IsNullOrEmpty(search.OrderSearch.TravelDateEnd))
                    {
                        try
                        {
                            DateTime endTime = DateTime.Parse(search.OrderSearch.TravelDateEnd.Trim()).AddDays(1);
                            string filter = "ServiceItemHistorys/TravelDate lt " + endTime.ToString("yyyy-MM-dd");
                            OrderResult = OrderResult.Filter(filter);
                            OrderCountResult = OrderCountResult.Filter(filter);
                        }
                        catch { }
                    }
                }
                if (!string.IsNullOrEmpty(search.OrderSearch.OrderSendDateBegin))
                {//订单发送时间
                    string filter = "OrderHistorys/any(x1:(x1/Remark eq '状态:已核对→已发送' or x1/Remark eq '状态:已核对→新单已接') and x1/OperTime ge " + search.OrderSearch.OrderSendDateBegin.Trim() + ")";
                    OrderResult = OrderResult.Filter(filter);
                    OrderCountResult = OrderCountResult.Filter(filter);
                }
                if (!string.IsNullOrEmpty(search.OrderSearch.OrderSendDateEnd))
                {
                    try
                    {
                        DateTime endTime = DateTime.Parse(search.OrderSearch.OrderSendDateEnd.Trim()).AddDays(1);
                        string filter = "OrderHistorys/any(x1:(x1/Remark eq '状态:已核对→已发送' or x1/Remark eq '状态:已核对→新单已接') and x1/OperTime lt " + endTime.ToString("yyyy-MM-dd") + ")";
                        OrderResult = OrderResult.Filter(filter);
                        OrderCountResult = OrderCountResult.Filter(filter);
                    }
                    catch { }
                }
                if (!string.IsNullOrEmpty(search.OrderSearch.ReturnDateBegin))
                {//返回时间
                    string filter = "ServiceItemHistorys/ReturnDate ge " + search.OrderSearch.ReturnDateBegin.Trim();
                    OrderResult = OrderResult.Filter(filter);
                    OrderCountResult = OrderCountResult.Filter(filter);
                }
                if (!string.IsNullOrEmpty(search.OrderSearch.ReturnDateEnd))
                {
                    try
                    {
                        DateTime endTime = DateTime.Parse(search.OrderSearch.ReturnDateEnd.Trim()).AddDays(1);
                        string filter = "ServiceItemHistorys/ReturnDate lt " + endTime.ToString("yyyy-MM-dd");
                        OrderResult = OrderResult.Filter(filter);
                        OrderCountResult = OrderCountResult.Filter(filter);
                    }
                    catch { }
                }

                if (search.OrderSearch.OrderSourseID > 0)
                {//订单来源
                    string filter = "TBOrders/OrderSourseID eq " + search.OrderSearch.OrderSourseID;
                    OrderResult = OrderResult.Filter(filter);
                    OrderCountResult = OrderCountResult.Filter(filter);
                }
                if (search.OrderSearch.SupplierID > 0)
                {//供应商
                    string filter = "ServiceItemHistorys/SupplierID eq " + search.OrderSearch.SupplierID;
                    OrderResult = OrderResult.Filter(filter);
                    OrderCountResult = OrderCountResult.Filter(filter);
                }
                if (!string.IsNullOrEmpty(search.OrderSearch.ItemName))
                {//产品，以空格分开
                    string[] name = search.OrderSearch.ItemName.Trim().Split(' ');
                    string filter = "contains(ServiceItemHistorys/cnItemName,'" + search.OrderSearch.ItemName + "') or contains(ServiceItemHistorys/enItemName,'" + search.OrderSearch.ItemName + "')";
                    foreach (var item in name)
                    {
                        filter += " or ServiceItemHistorys/ServiceCode eq '" + item + "'";
                    }
                    OrderResult = OrderResult.Filter(filter);
                    OrderCountResult = OrderCountResult.Filter(filter);
                }
                if (search.OrderSearch.IsNeedCustomerService == "true")
                {//要售后
                    string filter = "IsNeedCustomerService eq true";
                    OrderResult = OrderResult.Filter(filter);
                    OrderCountResult = OrderCountResult.Filter(filter);
                }
                if (search.OrderSearch.IsPay == "true")
                {//未付完
                    string filter = "IsPay eq true";
                    OrderResult = OrderResult.Filter(filter);
                    OrderCountResult = OrderCountResult.Filter(filter);
                }
                if (search.OrderSearch.isUrgent == "true")
                {//紧急订单
                    string filter = "isUrgent eq true";
                    OrderResult = OrderResult.Filter(filter);
                    OrderCountResult = OrderCountResult.Filter(filter);
                }
                if (search.OrderSearch.SupplierEnableOnline == "true")
                {//供应商启用系统
                    string filter = "ServiceItemHistorys/orderSupplier/EnableOnline eq true";
                    OrderResult = OrderResult.Filter(filter);
                    OrderCountResult = OrderCountResult.Filter(filter);
                }
                else if (search.OrderSearch.SupplierEnableOnline == "false")
                {//供应商没有启用系统
                    string filter = "ServiceItemHistorys/orderSupplier/EnableOnline eq false";
                    OrderResult = OrderResult.Filter(filter);
                    OrderCountResult = OrderCountResult.Filter(filter);
                }
                if (!string.IsNullOrEmpty(search.OrderSearch.CreateName))
                {//按订单创建人
                    search.OrderSearch.CreateName = search.OrderSearch.CreateName.Trim();
                    string[] name = search.OrderSearch.CreateName.Trim().Split(' ');
                    string filter = "";
                    foreach (var item in name)
                    {
                        if (!string.IsNullOrEmpty(item))
                        {
                            if (filter != "")
                            {
                                filter += " or ";
                            }
                            filter += "contains(CreateUserNikeName,'" + item + "')";
                        }
                    }
                    OrderResult = OrderResult.Filter(filter);
                    OrderCountResult = OrderCountResult.Filter(filter);
                }
                if (search.OrderSearch.isOneself == "true")
                {//自己创建的订单
                    string UserName = User.Identity.Name;
                    User user = await client.For<User>().Filter(u => u.UserName == UserName).FindEntryAsync();
                    string filter = "CreateUserID eq " + user.UserID;
                    OrderResult = OrderResult.Filter(filter);
                    OrderCountResult = OrderCountResult.Filter(filter);
                }
            }
            int count = await OrderCountResult.Count().FindScalarAsync<int>();
            //var com = await OrderResult.GetCommandTextAsync();
            var items = await OrderResult.Select(s => new
            {
                s.OrderID,
                s.CustomerID,
                s.OrderNo,
                s.TBOrders.TBID,
                s.TBNum,
                s.CustomerName,
                s.CustomerEnname,
                s.ServiceItemHistorys.SupplierCode,
                s.ServiceItemHistorys.ServiceCode,
                s.ServiceItemHistorys.cnItemName,
                s.ServiceItemHistorys.AdultNum,
                s.ServiceItemHistorys.ChildNum,
                s.ServiceItemHistorys.INFNum,
                s.ServiceItemHistorys.ServiceTypeID,
                s.ServiceItemHistorys.RightNum,
                s.ServiceItemHistorys.RoomNum,
                s.CreateTime,
                s.ServiceItemHistorys.TravelDate,
                s.state,
                s.Remark,
                s.TBOrders.OrderSourseID,
                s.TBOrders.Sourse.OrderSourseName,
                s.CreateUserNikeName,
                s.IsNeedCustomerService,
                s.IsPay,
                s.isUrgent,
                s.TBOrderID,
                s.Email,
                s.Tel
            }).FindEntriesAsync();
            var list = items.Select(item => new
            {

                OrderID = item.OrderID,
                CustomerID = item.CustomerID,
                OrderNo = item.OrderNo,
                TBID = item.TBOrders.TBID,
                TBNum = item.TBNum,
                CustomerName = item.CustomerName == null ? "" : item.CustomerName,
                CustomerEnname = item.CustomerEnname == null ? "" : item.CustomerEnname,
                SupplierCode = item.ServiceItemHistorys.SupplierCode,
                ServiceCode = item.ServiceItemHistorys.ServiceCode,
                cnItemName = item.ServiceItemHistorys.cnItemName,
                AdultNum = item.ServiceItemHistorys.AdultNum,
                ChildNum = item.ServiceItemHistorys.ChildNum,
                INFNum = item.ServiceItemHistorys.INFNum,
                NightNum = item.ServiceItemHistorys.ServiceTypeID == 4 ? item.ServiceItemHistorys.RightNum : 0,
                RoomNum = item.ServiceItemHistorys.ServiceTypeID == 4 ? item.ServiceItemHistorys.RoomNum : 0,
                CreateTime = item.CreateTime.ToString("yyyy-MM-dd"),
                TravelDate = (item.ServiceItemHistorys.TravelDate < Convert.ToDateTime("1901-01-02")) ? "" : item.ServiceItemHistorys.TravelDate.ToString("yyyy-MM-dd"),
                state = item.state,
                Remark = item.Remark,
                OrderSourseID = item.TBOrders.OrderSourseID,
                OrderSourseName = item.TBOrders.Sourse.OrderSourseName,
                CreateUserNikeName = item.CreateUserNikeName,
                IsNeedCustomerService = item.IsNeedCustomerService,
                IsPay = item.IsPay,
                isUrgent = item.isUrgent,
                TBOrderID = item.TBOrderID,
                Email = item.Email,
                Tel = item.Tel == null ? "" : item.Tel,
            });
            return JsonConvert.SerializeObject(new { draw = draw, recordsFiltered = count, data = list, SearchModel = search });
        }
        //查看操作日志
        public async Task<ActionResult> OrderOperation(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var orderHistorys = await client.For<OrderHistory>().OrderByDescending(o => o.OperTime).Filter(o => o.OrderID == id).FindEntriesAsync();
            return View(orderHistorys);
        }
        //导出excel
        public async Task<FileResult> ExportExcel(ShareOrderSearchModel search)
        {
            #region 获取列表
            string propertyName = "OrderNo";
            int sort = 0;
            string status = string.Empty;
            status = search.status;

            IBoundClient<Order> OrderResult = client.For<Order>()
                .Expand(t => t.TBOrders)
                .Expand(t => t.TBOrders.Sourse)
                .Expand(t => t.ServiceItemHistorys)
                .Expand(t => t.ServiceItemHistorys.ExtraServiceHistorys)
                .Expand(t => t.Customers);
            if (propertyName == "TravelDate")
            {
                OrderResult = sort == 0 ? OrderResult.OrderByDescending(s => s.ServiceItemHistorys.TravelDate) : OrderResult.OrderBy(s => s.ServiceItemHistorys.TravelDate);
            }
            else
            {
                OrderResult = sort == 0 ? OrderResult.OrderByDescending(propertyName) : OrderResult.OrderBy(propertyName);
            }
            if (!string.IsNullOrEmpty(status))
            {
                string[] state = status.Split(',');
                if (state.Count() > 1)
                {
                    string filter = "";
                    foreach (var item in state)
                    {
                        if (filter != "")
                        {
                            filter += " or ";
                        }
                        filter += "state eq LanghuaNew.Data.OrderState'" + item + "'";
                    }
                    OrderResult = OrderResult.Filter(filter);
                }
                else
                {
                    int statuss = string.IsNullOrEmpty(status) ? -1 : int.Parse(status);
                    if (statuss != -1)
                    {
                        string filter = "state eq LanghuaNew.Data.OrderState'" + statuss + "'";
                        OrderResult = OrderResult.Filter(filter);
                    }
                }
            }
            if (!string.IsNullOrEmpty(search.FuzzySearch))
            {//姓名、电话、订单号、淘宝ID、备注、产品中文名
                string filter = "";
                switch (search.FuzzySearchType)
                {
                    case "TBID":
                        filter = "contains(TBOrders/TBID,'" + search.FuzzySearch + "')";
                        break;
                    case "CustomerName":
                        filter = "contains(CustomerName,'" + search.FuzzySearch + "')";
                        break;
                    case "OrderNo":
                        filter = "contains(OrderNo,'" + search.FuzzySearch + "')";
                        break;
                    case "Remark":
                        filter = "contains(Remark,'" + search.FuzzySearch + "')";
                        break;
                    case "Tel":
                        filter = "contains(Tel,'" + search.FuzzySearch + "') or contains(BakTel,'" + search.FuzzySearch + "')";
                        break;
                    case "cnItemName":
                        filter = "contains(ServiceItemHistorys/cnItemName,'" + search.FuzzySearch + "')";
                        break;
                    default:
                        filter = "contains(CustomerName,'" + search.FuzzySearch
                               + "') or contains(Tel,'" + search.FuzzySearch
                               + "') or contains(BakTel,'" + search.FuzzySearch
                               + "') or contains(OrderNo,'" + search.FuzzySearch
                               + "') or contains(TBOrders/TBID,'" + search.FuzzySearch
                               + "') or contains(Remark,'" + search.FuzzySearch
                               + "') or contains(ServiceItemHistorys/cnItemName,'" + search.FuzzySearch + "')";
                        break;
                }
                OrderResult = OrderResult.Filter(filter);
            }
            if (!string.IsNullOrEmpty(search.OrderCreateDateBegin))
            {
                string filter = "CreateTime ge " + search.OrderCreateDateBegin;
                OrderResult = OrderResult.Filter(filter);
            }
            if (!string.IsNullOrEmpty(search.OrderCreateDateEnd))
            {
                try
                {
                    DateTime endTime = DateTime.Parse(search.OrderCreateDateEnd).AddDays(1);
                    string filter = "CreateTime lt " + endTime.ToString("yyyy-MM-dd");
                    OrderResult = OrderResult.Filter(filter);
                }
                catch { }
            }
            if (search.IsChangeTravelDate == "true")
            {//只查变更中的出行日期
                if (!string.IsNullOrEmpty(search.TravelDateBegin))
                {//出行时间
                    string filter = "ServiceItemHistorys/ChangeTravelDate ge " + search.TravelDateBegin.Trim();
                    OrderResult = OrderResult.Filter(filter);
                }
                if (!string.IsNullOrEmpty(search.TravelDateEnd))
                {
                    try
                    {
                        DateTime endTime = DateTime.Parse(search.TravelDateEnd.Trim()).AddDays(1);
                        string filter = "ServiceItemHistorys/ChangeTravelDate lt " + endTime.ToString("yyyy-MM-dd");
                        OrderResult = OrderResult.Filter(filter);
                    }
                    catch { }
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(search.TravelDateBegin))
                {//出行时间
                    string filter = "ServiceItemHistorys/TravelDate ge " + search.TravelDateBegin.Trim();
                    OrderResult = OrderResult.Filter(filter);
                }
                if (!string.IsNullOrEmpty(search.TravelDateEnd))
                {
                    try
                    {
                        DateTime endTime = DateTime.Parse(search.TravelDateEnd.Trim()).AddDays(1);
                        string filter = "ServiceItemHistorys/TravelDate lt " + endTime.ToString("yyyy-MM-dd");
                        OrderResult = OrderResult.Filter(filter);
                    }
                    catch { }
                }
            }
            if (!string.IsNullOrEmpty(search.OrderSendDateBegin))
            {//订单发送时间
                string filter = "OrderHistorys/any(x1:(x1/Remark eq '状态:已核对→已发送' or x1/Remark eq '状态:已核对→新单已接') and x1/OperTime ge " + search.OrderSendDateBegin.Trim() + ")";
                OrderResult = OrderResult.Filter(filter);
            }
            if (!string.IsNullOrEmpty(search.OrderSendDateEnd))
            {
                try
                {
                    DateTime endTime = DateTime.Parse(search.OrderSendDateEnd.Trim()).AddDays(1);
                    string filter = "OrderHistorys/any(x1:(x1/Remark eq '状态:已核对→已发送' or x1/Remark eq '状态:已核对→新单已接') and x1/OperTime lt " + endTime.ToString("yyyy-MM-dd") + ")";
                    OrderResult = OrderResult.Filter(filter);
                }
                catch { }
            }
            if (!string.IsNullOrEmpty(search.ReturnDateBegin))
            {
                string filter = "ServiceItemHistorys/ReturnDate ge " + search.ReturnDateBegin;
                OrderResult = OrderResult.Filter(filter);
            }
            if (!string.IsNullOrEmpty(search.ReturnDateEnd))
            {
                try
                {
                    DateTime endTime = DateTime.Parse(search.ReturnDateEnd).AddDays(1);
                    string filter = "ServiceItemHistorys/ReturnDate lt " + endTime.ToString("yyyy-MM-dd");
                    OrderResult = OrderResult.Filter(filter);
                }
                catch { }
            }
            if (search.OrderSourseID > 0)
            {
                string filter = "TBOrders/OrderSourseID eq " + search.OrderSourseID;
                OrderResult = OrderResult.Filter(filter);
            }
            if (search.SupplierID > 0)
            {
                string filter = "ServiceItemHistorys/SupplierID eq " + search.SupplierID;
                OrderResult = OrderResult.Filter(filter);
            }
            if (!string.IsNullOrEmpty(search.ItemName))
            {
                //string[] name = search.ItemName.Trim().Split(' ');
                //string filter = "";
                //foreach (var item in name)
                //{
                //    if (filter != "")
                //    {
                //        filter += " or ";
                //    }
                //    filter += "contains(ServiceItemHistorys/cnItemName,'" + item
                //        + "') or contains(ServiceItemHistorys/enItemName,'" + item
                //        + "') or ServiceItemHistorys/ServiceCode eq '" + (item.Contains("#") ? item : ("#" + item))
                //        + "' or ServiceItemHistorys/ServiceCode eq '" + item + "'";
                //}
                //OrderResult = OrderResult.Filter(filter);
                string[] name = search.ItemName.Trim().Split(' ');
                string filter = "contains(ServiceItemHistorys/cnItemName,'" + search.ItemName + "') or contains(ServiceItemHistorys/enItemName,'" + search.ItemName + "')";
                foreach (var item in name)
                {
                    filter += " or ServiceItemHistorys/ServiceCode eq '" + item + "'";
                }
                OrderResult = OrderResult.Filter(filter);
            }
            if (search.IsNeedCustomerService == "true")
            {
                string filter = "IsNeedCustomerService eq true";
                OrderResult = OrderResult.Filter(filter);
            }
            if (search.IsPay == "true")
            {
                string filter = "IsPay eq true";
                OrderResult = OrderResult.Filter(filter);
            }
            if (search.isOneself == "true")
            {
                string UserName = User.Identity.Name;
                User user = await client.For<User>().Filter(u => u.UserName == UserName).FindEntryAsync();

                string filter = "CreateUserID eq " + user.UserID;
                OrderResult = OrderResult.Filter(filter);
            }
            if (search.isUrgent == "true")
            {//紧急订单
                string filter = "isUrgent eq true";
                OrderResult = OrderResult.Filter(filter);
            }
            if (search.SupplierEnableOnline == "true")
            {//供应商启用系统
                string filter = "ServiceItemHistorys/orderSupplier/EnableOnline eq true";
                OrderResult = OrderResult.Filter(filter);
            }
            else if (search.SupplierEnableOnline == "false")
            {//供应商没有启用系统
                string filter = "ServiceItemHistorys/orderSupplier/EnableOnline eq false";
                OrderResult = OrderResult.Filter(filter);
            }
            #endregion
            //创建Excel文件的对象
            NPOI.HSSF.UserModel.HSSFWorkbook book = new NPOI.HSSF.UserModel.HSSFWorkbook();
            //添加一个sheet
            NPOI.SS.UserModel.ISheet sheet1 = book.CreateSheet("Sheet1");
            //search.ExportField = string.IsNullOrEmpty(search.ExportField) ? "0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17" : search.ExportField;
            //导出字段
            if (!string.IsNullOrEmpty(search.ExportField))
            {
                int i = 0;
                //给sheet1添加第一行的头部标题
                NPOI.SS.UserModel.IRow row1 = sheet1.CreateRow(i);

                string[] exportField = search.ExportField.Split(',');
                for (int ex = 0; ex < exportField.Count(); ex++)
                {
                    int FieldValue = int.Parse(exportField[ex]);
                    string cellValue = EnumHelper.GetEnumDescription((ExportField)FieldValue);
                    row1.CreateCell(ex).SetCellValue(cellValue);
                    if ((ExportField)FieldValue == ExportField.ExtraServices)
                    {
                        row1.CreateCell(ex + 1).SetCellValue("数量");
                    }
                    //row1.CreateCell(0).SetCellValue("订单号");
                    //row1.CreateCell(1).SetCellValue("淘宝ID");
                    //row1.CreateCell(2).SetCellValue("中文姓名");
                    //row1.CreateCell(3).SetCellValue("英文姓名");
                    //row1.CreateCell(4).SetCellValue("订单来源");
                    //row1.CreateCell(5).SetCellValue("联系电话");
                    //row1.CreateCell(6).SetCellValue("备用联系电话");
                    //row1.CreateCell(7).SetCellValue("供应商");
                    //row1.CreateCell(8).SetCellValue("预订项目");
                    //row1.CreateCell(9).SetCellValue("产品编码");
                    //row1.CreateCell(10).SetCellValue("成人人数");
                    //row1.CreateCell(11).SetCellValue("儿童人数");
                    //row1.CreateCell(12).SetCellValue("婴儿人数");
                    //row1.CreateCell(13).SetCellValue("出行日期");
                    //row1.CreateCell(14).SetCellValue("订单状态");
                    //row1.CreateCell(15).SetCellValue("备注");
                    //row1.CreateCell(16).SetCellValue("订单创建人");
                    //row1.CreateCell(17).SetCellValue("所有附加项目");
                }
                row1.Height = 450;

                //将数据逐步写入sheet1各个行
                var items = await OrderResult.FindEntriesAsync();
                foreach (var item in items)
                {
                    i++;
                    string IsNeedCustomerService = item.IsNeedCustomerService ? "|要售后" : "";
                    string IsPay = item.IsPay ? "|未付完" : "";
                    NPOI.SS.UserModel.IRow rowtemp = sheet1.CreateRow(i);

                    for (int ex = 0; ex < exportField.Count(); ex++)
                    {
                        int FieldValue = int.Parse(exportField[ex]);
                        switch ((ExportField)FieldValue)
                        {
                            case ExportField.OrderNo:
                                sheet1.SetColumnWidth(ex, 5000);
                                rowtemp.CreateCell(ex).SetCellValue(item.OrderNo == null ? "" : (item.OrderNo.ToString()));
                                break;
                            case ExportField.TBOrderNo:
                                sheet1.SetColumnWidth(ex, 5000);
                                rowtemp.CreateCell(ex).SetCellValue(item.TBNum == null ? "" : (item.TBNum.ToString()));
                                break;
                            case ExportField.TBID:
                                rowtemp.CreateCell(ex).SetCellValue(item.TBOrders.TBID);
                                break;
                            case ExportField.cnName:
                                rowtemp.CreateCell(ex).SetCellValue(item.CustomerName);
                                break;
                            case ExportField.enName:
                                rowtemp.CreateCell(ex).SetCellValue(item.CustomerEnname);
                                break;
                            case ExportField.OrderSourse:
                                rowtemp.CreateCell(ex).SetCellValue(item.TBOrders.Sourse.OrderSourseName);
                                break;
                            case ExportField.Tel:
                                sheet1.SetColumnWidth(ex, 3000);
                                rowtemp.CreateCell(ex).SetCellValue(item.Tel);
                                break;
                            case ExportField.BakTel:
                                sheet1.SetColumnWidth(ex, 3000);
                                rowtemp.CreateCell(ex).SetCellValue(item.BakTel);
                                break;
                            case ExportField.SupplierCode:
                                rowtemp.CreateCell(ex).SetCellValue(item.ServiceItemHistorys.SupplierCode);
                                break;
                            case ExportField.cnItemName:
                                sheet1.SetColumnWidth(ex, 5000);
                                rowtemp.CreateCell(ex).SetCellValue(item.ServiceItemHistorys.cnItemName);
                                break;
                            case ExportField.ServiceCode:
                                rowtemp.CreateCell(ex).SetCellValue(item.ServiceItemHistorys.ServiceCode);
                                break;
                            case ExportField.AdultNum:
                                rowtemp.CreateCell(ex).SetCellValue(item.ServiceItemHistorys.AdultNum);
                                break;
                            case ExportField.ChildNum:
                                rowtemp.CreateCell(ex).SetCellValue(item.ServiceItemHistorys.ChildNum);
                                break;
                            case ExportField.INFNum:
                                rowtemp.CreateCell(ex).SetCellValue(item.ServiceItemHistorys.INFNum);
                                break;
                            case ExportField.RoomNum:
                                rowtemp.CreateCell(ex).SetCellValue(item.ServiceItemHistorys.RoomNum);
                                break;
                            case ExportField.NightNum:
                                rowtemp.CreateCell(ex).SetCellValue(item.ServiceItemHistorys.RightNum);
                                break;
                            case ExportField.TravelDate:
                                sheet1.SetColumnWidth(ex, 3000);
                                rowtemp.CreateCell(ex).SetCellValue((item.ServiceItemHistorys.TravelDate < Convert.ToDateTime("1902-02-02")) ? "" : item.ServiceItemHistorys.TravelDate.ToString("yyyy-MM-dd"));
                                break;
                            case ExportField.OrderState:
                                sheet1.SetColumnWidth(ex, 5000);
                                rowtemp.CreateCell(ex).SetCellValue(EnumHelper.GetEnumDescription(item.state).Substring(0, EnumHelper.GetEnumDescription(item.state).IndexOf("|")) + IsNeedCustomerService + IsPay);
                                break;
                            case ExportField.Remark:
                                rowtemp.CreateCell(ex).SetCellValue(item.Remark == null ? "" : item.Remark.ToString());
                                break;
                            case ExportField.CreateUserNikeName:
                                rowtemp.CreateCell(ex).SetCellValue(item.CreateUserNikeName);
                                break;
                            case ExportField.ExtraServices:
                                if (item.ServiceItemHistorys.ExtraServiceHistorys != null)
                                {
                                    foreach (var extraServiceHistory in item.ServiceItemHistorys.ExtraServiceHistorys.OrderBy(s => s.ServiceName))
                                    {
                                        rowtemp.CreateCell(ex).SetCellValue(extraServiceHistory.ServiceName);
                                        ex++;
                                        rowtemp.CreateCell(ex).SetCellValue(extraServiceHistory.ServiceNum);
                                        ex++;
                                    }
                                }
                                break;
                        }
                    }
                }
            }
            // 写入到客户端 
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            book.Write(ms);
            ms.Seek(0, SeekOrigin.Begin);
            return File(ms, "application/vnd.ms-excel", "orderlist.xls");
        }
        //提交订单
        [HttpPost]
        public async Task<string> CommitOrders(TBOrderView tborderView)
        {
            string TBID = string.IsNullOrEmpty(tborderView.TBID) ? string.Empty : tborderView.TBID.Trim().ToLower();
            if (string.IsNullOrEmpty(TBID))
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "淘宝ID不能为空" });
            }
            if (tborderView.OrderSourseID == 0)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "订单来源不能为空" });
            }
            if (tborderView.Orders == null)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "订单不能为空" });
            }
            TBOrder tborder = new TBOrder();
            DateTime now = DateTime.Now;
            try
            {
                #region 保存订单
                int OrderSourseID = tborderView.OrderSourseID;
                //计算总成本，换算成RMB
                float totalCost = 0;
                List<OrderView> orderViews = tborderView.Orders;
                //先获取订单来源
                OrderSourse orderSourse = await client.For<OrderSourse>().Key(OrderSourseID).FindEntryAsync();
                //获取客户
                Customer customerOld = await client.For<Customer>().Filter(t => t.CustomerTBCode == TBID).FindEntryAsync();
                if (customerOld == null)
                {
                    customerOld = new Customer();
                    customerOld.CustomerTBCode = TBID;
                    customerOld.CreateTime = DateTime.Now;
                    customerOld.CustomerName = "";
                    customerOld.CustomerEnname = "";
                    customerOld.Email = null;
                    customerOld.IsDelete = false;
                    customerOld.Password = "";
                    customerOld.Wechat = "";
                    customerOld = await client.For<Customer>().Set(customerOld).InsertEntryAsync();
                }

                //获取订单详情
                List<Order> orders = new List<Order>();
                //计算额外项目最小值和选择值
                int minNum = 0;
                int SelectNum = 0;
                foreach (var orderView in orderViews)
                {
                    //计算单个订单成本,不转换币别
                    float orderCost = 0;
                    int itemID = orderView.ItemID;
                    int supplierID = orderView.SupplierID;
                    int adultNum = orderView.AdultNum;
                    int childNum = orderView.ChildNum;
                    int infNum = orderView.INFNum;
                    int roomNum = orderView.RoomNum;
                    int rightNum = orderView.RightNum;
                    string TBNum = string.IsNullOrEmpty(orderView.TBNum) ? string.Empty : orderView.TBNum.Trim();
                    if (string.IsNullOrEmpty(TBNum))
                    {
                        return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "淘宝订单号不能为空" });
                    }
                    //获取产品信息
                    ServiceItem item = await client.For<ServiceItem>().Expand(u => u.ExtraServices).Expand(u => u.ItemServiceType).Key(itemID).FindEntryAsync();
                    if (adultNum + childNum + infNum == 0)
                    {
                        return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "人数不能为0" });
                    }
                    if (item.ServiceTypeID == 4 && roomNum * rightNum == 0)
                    {
                        return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "间数和晚数不能为0" });
                    }
                    //获取供应商信息
                    Supplier supplier = await client.For<Supplier>().Key(supplierID).FindEntryAsync();
                    //获取产品供应商价格(可能为null)
                    SupplierServiceItem supplierItem = await client.For<SupplierServiceItem>()
                        .Expand(u => u.ItemPriceBySuppliers)
                        .Expand(u => u.ItemCurrency)
                        .Expand(u => u.ExtraServicePrices)
                        .Filter(u => u.ServiceItemID == itemID && u.SupplierID == supplierID)
                        .FindEntryAsync();
                    //赋默认值,计费方式/兑换方式/汇率/币别/产品价格
                    PricingMethod payType = PricingMethod.ByPerson;
                    ChangeType changeType = ChangeType.FromChina;
                    float rate = 1;
                    string currencyName = "";
                    int currencyID = 0;
                    float adultNetPrice = 0;
                    float childNetPrice = 0;
                    float bobyNetPrice = 0;
                    float price = 0;
                    //从产品供应商价格中将价格读取出来
                    if (supplierItem != null)
                    {
                        payType = supplierItem.PayType;
                        changeType = supplierItem.ItemCurrency.CurrencyChangeType;
                        rate = supplierItem.ItemCurrency.ExchangeRate;
                        currencyName = supplierItem.ItemCurrency.CurrencyName;
                        currencyID = supplierItem.ItemCurrency.CurrencyID;
                        if (supplierItem.ItemPriceBySuppliers != null)
                        {
                            foreach (var r in supplierItem.ItemPriceBySuppliers)
                            {
                                if (r.startTime < DateTime.Now && r.EndTime.AddDays(1) > DateTime.Now)
                                {
                                    adultNetPrice = r.AdultNetPrice;
                                    bobyNetPrice = r.BobyNetPrice;
                                    childNetPrice = r.ChildNetPrice;
                                    price = r.Price;
                                    //计算单个订单成本
                                    orderCost = payType == PricingMethod.ByPerson
                                        ? adultNetPrice * adultNum + childNetPrice * childNum + bobyNetPrice * infNum
                                        : price * roomNum * rightNum;
                                    //按人头与按产品数量2种算法，按兑换方式换算成RMB
                                    if (changeType == ChangeType.FromChina)
                                    {
                                        totalCost += (payType == PricingMethod.ByPerson
                                        ? adultNetPrice * adultNum + childNetPrice * childNum + bobyNetPrice * infNum
                                        : price * roomNum * rightNum) / rate;
                                    }
                                    else if (changeType == ChangeType.ToChina)
                                    {
                                        totalCost += (payType == PricingMethod.ByPerson
                                        ? adultNetPrice * adultNum + childNetPrice * childNum + bobyNetPrice * infNum
                                        : price * roomNum * rightNum) * rate;
                                    }
                                    break;
                                }
                            }
                        }
                    }
                    //保存额外服务及价格extraServiceHistorys
                    List<ExtraServiceHistory> extraServiceHistorys = new List<ExtraServiceHistory>();
                    ExtraServiceHistory extraServiceHistory = new ExtraServiceHistory();
                    if (orderView.ExtraServiceHistorys != null)
                    {
                        foreach (var r in orderView.ExtraServiceHistorys)
                        {
                            ExtraService extraservice = new ExtraService();
                            int serviceID = r.ExtraServiceID;
                            int serviceNum = r.ServiceNum;
                            //获取额外服务信息
                            if (item.ExtraServices != null)
                            {
                                foreach (var e in item.ExtraServices)
                                {
                                    if (e.ExtraServiceID == serviceID)
                                    {
                                        extraservice = e;
                                        break;
                                    }
                                }
                            }
                            //获取额外服务价格
                            float extraServicePrice = 0;
                            if (supplierItem != null)
                            {
                                if (supplierItem.ExtraServicePrices != null)
                                {
                                    foreach (var e in supplierItem.ExtraServicePrices)
                                    {
                                        if (e.ExtraServiceID == serviceID)
                                        {
                                            extraServicePrice = e.ServicePrice;
                                            break;
                                        }

                                    }
                                }
                            }
                            var ExtraServiceViewconfig = new MapperConfiguration(cfg => cfg.CreateMap<ExtraService, ExtraServiceHistory>());
                            var ExtraServiceViewmapper = ExtraServiceViewconfig.CreateMapper();
                            extraServiceHistory = ExtraServiceViewmapper.Map<ExtraServiceHistory>(extraservice);
                            extraServiceHistory.ServiceNum = serviceNum;
                            extraServiceHistory.ServicePrice = extraServicePrice;
                            extraServiceHistorys.Add(extraServiceHistory);
                            //计算单个订单成本
                            orderCost += serviceNum * extraServicePrice;
                            //计算总成本，按兑换方式换算成RMB
                            if (changeType == ChangeType.FromChina)
                            {
                                totalCost += serviceNum * extraServicePrice / rate;
                            }
                            else if (changeType == ChangeType.ToChina)
                            {
                                totalCost += serviceNum * extraServicePrice * rate;
                            }
                            minNum += extraServiceHistory.MinNum;
                            SelectNum += serviceNum;
                        }
                    }

                    //创建产品历史库itemHistory
                    ServiceItemHistory itemHistory = new ServiceItemHistory();
                    itemHistory.ServiceCode = item.ServiceCode;
                    itemHistory.ServiceItemID = item.ServiceItemID;
                    itemHistory.cnItemName = item.cnItemName;
                    itemHistory.enItemName = item.enItemName;
                    itemHistory.SupplierID = supplierID;
                    itemHistory.SupplierCode = supplier.SupplierNo;
                    itemHistory.SupplierName = supplier.SupplierName;
                    itemHistory.ServiceTypeID = item.ItemServiceType.ServiceTypeID;
                    itemHistory.ServiceTypeName = item.ItemServiceType.ServiceTypeName;
                    itemHistory.ServiceItemTemplteID = item.ServiceItemTemplteID;

                    itemHistory.AdultNetPrice = adultNetPrice;
                    itemHistory.ChildNetPrice = childNetPrice;
                    itemHistory.BobyNetPrice = bobyNetPrice;
                    itemHistory.Price = price;
                    itemHistory.TotalCost = orderCost;

                    itemHistory.CurrencyID = currencyID;
                    itemHistory.CurrencyName = currencyName;
                    itemHistory.CurrencyChangeType = changeType;
                    itemHistory.ExchangeRate = rate;

                    itemHistory.PayType = payType;
                    itemHistory.Elements = item.ElementContent;

                    itemHistory.AdultNum = adultNum;
                    itemHistory.ChildNum = childNum;
                    itemHistory.INFNum = infNum;
                    itemHistory.RoomNum = roomNum;
                    itemHistory.RightNum = rightNum;

                    itemHistory.TravelDate = orderView.TravelDate;//Convert.ToDateTime("1900-01-01");
                    itemHistory.ReturnDate = Convert.ToDateTime("1900-01-01");
                    itemHistory.CreateTime = DateTime.Now;
                    itemHistory.GroupNo = "";
                    itemHistory.ReceiveManTime = "";
                    itemHistory.TrafficSurcharge = 0;
                    itemHistory.TravelCompany = item.TravelCompany;
                    itemHistory.InsuranceDays = item.InsuranceDays;

                    itemHistory.TrafficCurrencyID = currencyID;
                    itemHistory.TrafficCurrencyName = currencyName;
                    itemHistory.TrafficCurrencyChangeType = changeType;
                    itemHistory.TrafficExchangeRate = rate;
                    itemHistory.FixedDays = item.FixedDays;

                    itemHistory.ExtraServiceHistorys = extraServiceHistorys;

                    //获取当前用户，创建操作记录orderHistorys
                    string userName = User.Identity.Name;
                    User user = await client.For<User>().Filter(u => u.UserName == userName).FindEntryAsync();
                    List<OrderHistory> orderHistorys = new List<OrderHistory>();
                    OrderHistory orderHistory = new OrderHistory();
                    orderHistory.OperUserID = user.UserID.ToString();
                    orderHistory.OperUserNickName = user.NickName;
                    orderHistory.OperTime = DateTime.Now;
                    orderHistory.State = OrderState.Notfilled;
                    orderHistory.Remark = "新建订单";
                    orderHistorys.Add(orderHistory);

                    //创建订单表order
                    Order order = new Order();
                    order.CustomerID = customerOld.CustomerID;
                    order.ServiceItemHistorys = itemHistory;
                    order.OrderDate = Convert.ToDateTime("1900-01-01");
                    order.state = OrderState.Notfilled;
                    order.IsPay = false;
                    order.CreateTime = now;
                    order.OrderHistorys = orderHistorys;
                    order.CreateUserID = user.UserID;
                    order.CreateUserNikeName = user.NickName;
                    order.TBNum = TBNum;
                    if (customerOld.CustomerName == "" && orderView.Customer != null)
                    {
                        order.CustomerName = orderView.Customer.CustomerName;
                        order.Tel = orderView.Customer.Tel;
                        order.Email = orderView.Customer.Email;
                    }
                    if (orderView.TBOrderNos != null && orderView.TBOrderNos.Count > 0)
                    {
                        order.TBOrderNos = new List<TBOrderNo>();
                        foreach (var no in orderView.TBOrderNos)
                        {
                            TBOrderNo one = new TBOrderNo();
                            one.No = no.No;
                            one.SubNo = no.SubNo;
                            one.Payment = no.Payment;
                            one.PaymentSplit = no.Payment;
                            //如果有退款，就查退款金额
                            if (no.RefundId > 0)
                            {
                                try
                                {
                                    TB_Access_Token tb_Access_Toen = await client.For<TB_Access_Token>().FindEntryAsync();
                                    if (tb_Access_Toen != null)
                                    {
                                        string url = ConfigurationManager.AppSettings["bizInterface_Trades"];
                                        ITopClient Iclient = new DefaultTopClient(url,
                                            ConfigurationManager.AppSettings["client_id"],
                                            ConfigurationManager.AppSettings["client_secret"]);
                                        RefundGetRequest req = new RefundGetRequest();
                                        req.Fields = "refund_fee";
                                        req.RefundId = no.RefundId;
                                        RefundGetResponse rsp = Iclient.Execute(req, tb_Access_Toen.access_token);
                                        one.RefundFee = float.Parse(rsp.Refund.RefundFee);
                                        one.RefundFeeSplit = float.Parse(rsp.Refund.RefundFee);
                                    }
                                }
                                catch { }
                            }
                            order.TBOrderNos.Add(one);
                        }
                    }
                    //将订单放到订单列表里  
                    orders.Add(order);
                }

                if (minNum > 0 && SelectNum == 0)
                {
                    return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "请至少选中一项额外项目" });
                }

                tborder.TBID = TBID;
                tborder.TotalCost = totalCost;
                tborder.TotalReceive = 0;
                tborder.OrderSourseID = orderSourse.OrderSourseID;
                tborder.Orders = orders;

                HttpResponseMessage Message = await HttpHelper.PostAction("TBOrdersExtend", JsonConvert.SerializeObject(tborder));
                string id = Message.Content.ReadAsStringAsync().Result;

                #endregion

                return JsonConvert.SerializeObject(new { ErrorCode = 200, ErrorMessage = "OK", data = new { TBOrderID = id } });
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 400, ErrorMessage = ex.Message });
            }
        }
        //修改成本
        [HttpPost]
        public async Task<string> UpdateTotalCost(int? OrderID, int TotalCost)
        {
            if (OrderID == null)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "修改失败！失败原因：参数不正确" });
            }
            try
            {
                ServiceItemHistory item = await client.For<ServiceItemHistory>().Key(OrderID).FindEntryAsync();
                float OldTotalCost = item.TotalCost;
                //数据有变化才保存
                if (OldTotalCost != TotalCost)
                {
                    item.TotalCost = TotalCost;
                    await client.For<ServiceItemHistory>().Key(OrderID).Set(item).UpdateEntryAsync();

                    Order order = await client.For<Order>().Key(OrderID).Select(s => new { s.OrderID, s.state }).FindEntryAsync();

                    //获取当前用户，创建操作记录orderHistorys
                    string userName = User.Identity.Name;
                    User user = await client.For<User>().Filter(u => u.UserName == userName).FindEntryAsync();
                    await client.For<OrderHistory>().Set(new { OrderID = order.OrderID, OperUserID = user.UserID.ToString(), OperUserNickName = user.NickName, OperTime = DateTime.Now, State = order.state, Remark = "总成本由" + OldTotalCost + "改为" + TotalCost }).InsertEntryAsync();
                }
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 400, ErrorMessage = "修改失败！失败原因：" + ex.Message });
            }
            return JsonConvert.SerializeObject(new { ErrorCode = 200, ErrorMessage = "OK" });
        }
        //修改团号
        [HttpPost]
        public async Task<string> UpdateGroupNo(int? OrderID, string GroupNo, string ReceiveManTime, string Remark, int TrafficSurcharge, int CurrencyID, string CurrencyName)
        {
            if (OrderID == null)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "修改失败！失败原因：参数不正确" });
            }
            if (CurrencyID == 0)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "修改失败！币别不正确" });
            }
            if (string.IsNullOrEmpty(CurrencyName))
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "修改失败！币别名称不能为空" });
            }
            try
            {
                ServiceItemHistory item = await client.For<ServiceItemHistory>().Key(OrderID).FindEntryAsync();
                float OldTrafficSurcharge = item.TrafficSurcharge;
                string OldCurrencyName = item.TrafficCurrencyName;
                string OldGroupNo = item.GroupNo;
                string OldReceiveManTime = item.ReceiveManTime;

                Order order = await client.For<Order>().Key(OrderID).FindEntryAsync();
                string OldRemark = order.Remark;

                string newRemark = "";

                if ((OldGroupNo == null ? "" : OldGroupNo.Trim()) != (GroupNo == null ? "" : GroupNo.Trim()))
                {
                    newRemark += "团号：\"" + OldGroupNo + "\"→\"" + GroupNo + "\" ";
                }
                if ((OldReceiveManTime == null ? "" : OldReceiveManTime.Trim()) != (ReceiveManTime == null ? "" : ReceiveManTime.Trim()))
                {
                    newRemark += "接人时间：\"" + (OldReceiveManTime == null ? "" : OldReceiveManTime.Trim()) + "\"→\"" + (ReceiveManTime == null ? "" : ReceiveManTime.Trim()) + "\" ";
                }
                if (OldTrafficSurcharge != TrafficSurcharge || (OldCurrencyName == null ? "" : OldCurrencyName.Trim()) != (CurrencyName == null ? "" : CurrencyName.Trim()))
                {
                    newRemark += "附加费：\"" + OldTrafficSurcharge.ToString().Trim() + (OldCurrencyName == null ? "" : OldCurrencyName.Trim()) + "\"→\"" + TrafficSurcharge.ToString().Trim() + (CurrencyName == null ? "" : CurrencyName.Trim()) + "\" ";
                }
                string historyRemark = newRemark;
                if ((OldRemark == null ? "" : OldRemark.Trim()) != (Remark == null ? "" : Remark.Trim()))
                {
                    historyRemark += "备注：\"" + (OldRemark == null ? "" : OldRemark.Trim()) + "\"→\"" + (Remark == null ? "" : Remark.Trim()) + "\" ";
                }
                //获取当前用户，创建操作记录orderHistorys
                string userName = User.Identity.Name;
                User user = await client.For<User>().Filter(u => u.UserName == userName).FindEntryAsync();
                //数据有变化才保存
                if (newRemark != "")
                {
                    item.TrafficSurcharge = TrafficSurcharge;
                    item.TrafficCurrencyID = CurrencyID;
                    item.TrafficCurrencyName = CurrencyName;
                    item.GroupNo = GroupNo;
                    item.ReceiveManTime = ReceiveManTime;
                    await client.For<ServiceItemHistory>().Key(OrderID).Set(item).UpdateEntryAsync();

                    await client.For<OrderSupplierHistory>().Set(new { opera = OrderOperator.inside, State = order.state, OrderID = order.OrderID, OperUserID = user.UserID, OperTime = DateTimeOffset.Now, OperNickName = "(浪花朵朵)" + user.NickName, Remark = newRemark }).InsertEntryAsync();
                }
                if (historyRemark != "")
                {
                    order.Remark = Remark;
                    await client.For<Order>().Key(order.OrderID).Set(order).UpdateEntryAsync();
                    await client.For<OrderHistory>().Set(new { OrderID = order.OrderID, OperUserID = user.UserID.ToString(), OperUserNickName = user.NickName, OperTime = DateTime.Now, State = order.state, Remark = historyRemark }).InsertEntryAsync();
                }
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 400, ErrorMessage = "修改失败！失败原因：" + ex.Message });
            }
            return JsonConvert.SerializeObject(new { ErrorCode = 200, ErrorMessage = "OK" });
        }
        //修改备注
        [HttpPost]
        public async Task<string> UpdateRemark(int? OrderID, string Remark)
        {
            if (OrderID == null)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "修改失败！失败原因：参数不正确" });
            }
            try
            {
                Order order = await client.For<Order>().Key(OrderID).FindEntryAsync();
                string OldRemark = order.Remark;
                if (OldRemark != Remark)
                {
                    //获取当前用户，创建操作记录orderHistorys
                    string userName = User.Identity.Name;
                    User user = await client.For<User>().Filter(u => u.UserName == userName).FindEntryAsync();
                    order.Remark = Remark;
                    await client.For<Order>().Key(order.OrderID).Set(order).UpdateEntryAsync();
                    await client.For<OrderHistory>().Set(new { OrderID = order.OrderID, OperUserID = user.UserID.ToString(), OperUserNickName = user.NickName, OperTime = DateTime.Now, State = order.state, Remark = "留言由\"" + OldRemark + "\"改为\"" + Remark + "\"" }).InsertEntryAsync();
                }
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 400, ErrorMessage = "修改失败！失败原因：" + ex.Message });
            }
            return JsonConvert.SerializeObject(new { ErrorCode = 200, ErrorMessage = "OK" });
        }
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = await client.For<Order>()
                .Expand(u => u.ServiceItemHistorys)
                .Expand(u => u.ServiceItemHistorys.ExtraServiceHistorys)
                .Expand(u => u.Customers)
                //.Expand(u => u.Customers.Travellers)
                .Key(id)
                .FindEntryAsync();
            if (string.IsNullOrEmpty(order.CustomerName) && string.IsNullOrEmpty(order.CustomerEnname) && string.IsNullOrEmpty(order.Tel) && string.IsNullOrEmpty(order.BakTel) && string.IsNullOrEmpty(order.Email) && string.IsNullOrEmpty(order.Wechat))
            {
                order.CustomerName = order.Customers.CustomerName;
                order.CustomerEnname = order.Customers.CustomerEnname;
                order.Tel = order.Customers.Tel;
                order.BakTel = order.Customers.BakTel;
                order.Email = order.Customers.Email;
                order.Wechat = order.Customers.Wechat;
            }
            if (order == null)
            {
                return HttpNotFound();
            }
            ViewBag.Area = await client.For<Area>().FindEntriesAsync();
            return View(order);
        }
        //修改订单
        [HttpPost]
        public async Task<string> SaveOrder(Order order)
        {
            if (order.OrderID == 0)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "找不到数据！" });
            }
            if (order.Customers == null)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "联系人信息不能为空！" });
            }

            Order item = await client.For<Order>()
                .Expand(o => o.ServiceItemHistorys).Expand(o => o.Customers)
                .Filter(u => u.OrderID == order.OrderID & u.CustomerID == order.Customers.CustomerID)
                .FindEntryAsync();
            if (item == null)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "错误的客户ID！" });
            }

            item.CustomerName = order.Customers.CustomerName;
            item.CustomerEnname = order.Customers.CustomerEnname;
            item.Tel = order.Customers.Tel;
            item.BakTel = order.Customers.BakTel;
            item.Email = order.Customers.Email;
            item.Wechat = order.Customers.Wechat;
            if (string.IsNullOrEmpty(order.Customers.CustomerName) || string.IsNullOrEmpty(order.Customers.CustomerEnname))
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "姓名不能为空！" });
            }
            if (string.IsNullOrEmpty(order.Customers.Email))
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "邮箱不能为空！" });
            }
            if (string.IsNullOrEmpty(order.Customers.Tel))
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "联系电话不能为空！" });
            }
            if (string.IsNullOrEmpty(order.Customers.Wechat))
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "微信号不能为空！" });
            }

            try
            {
                //保存订单信息
                int OrderID = order.OrderID;
                if (item.state == OrderState.Invalid)
                {
                    return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "本单已作废！" });
                }
                if (item.state > OrderState.Send)
                {
                    return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "供应商接单后不能修改！请发起请求变更！" });
                }
                DateTimeOffset TravelDate = order.ServiceItemHistorys.TravelDate;
                DateTimeOffset ReturnDate = order.ServiceItemHistorys.ReturnDate;
                ServiceItemHistory itemHistory = item.ServiceItemHistorys;
                if (itemHistory.FixedDays > 0)
                {
                    ReturnDate = TravelDate.AddDays(itemHistory.FixedDays - 1);
                }
                if (itemHistory.ServiceTypeID == 2)
                {
                    if (ReturnDate < TravelDate)
                    {
                        return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "行程类项目返回日期不能为空且不能早于出行日期！" });
                    }
                }
                if (itemHistory.ServiceTypeID == 4)
                {
                    if ((ReturnDate.Date - TravelDate.Date).TotalDays != itemHistory.RightNum)
                    {
                        return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "入住" + itemHistory.RightNum + "晚，您选择的日期有误！" });
                    }
                }
                if (await CheckRule(itemHistory.ServiceItemID, TravelDate))
                {
                    return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "出行日期不在规则允许范围内！" });
                }
                //if (itemHistory.ServiceTypeID == 2)
                //{
                //    Order ssorder = await client.For<Order>()
                //    .Filter(s => s.ServiceItemHistorys.ServiceTypeID == 2)//行程
                //    .Filter(s => s.CustomerID == item.CustomerID)
                //    .Filter(s => s.OrderID != order.OrderID)
                //    .Filter(s => s.state != OrderState.Invalid && s.state != OrderState.SencondCancel && s.state != OrderState.CancelReceive && s.state != OrderState.RequestCancel && s.state != OrderState.Cancel && s.state != OrderState.Notfilled)
                //    //.Filter(s => s.CustomerName == item.CustomerName || s.Tel == item.Tel)
                //    .Filter(s => s.ServiceItemHistorys.TravelDate == order.ServiceItemHistorys.TravelDate || s.ServiceItemHistorys.ChangeTravelDate == order.ServiceItemHistorys.TravelDate)
                //    .Select(s => new { s.OrderID, s.CustomerName, s.Tel }).FindEntryAsync();
                //    if (ssorder != null)
                //    {
                //        if (ssorder.CustomerName == item.CustomerName)
                //        {
                //            return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "系统检测到 " + item.CustomerName + " 在 " + order.ServiceItemHistorys.TravelDate.ToString("yyyy-MM-dd") + " 有其它行程安排！请修改出行日期或预订人姓名、电话号码后再保存。" });
                //        }
                //        if (ssorder.Tel == item.Tel)
                //        {
                //            return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "系统检测到 " + item.Tel + "  这个号码出现在 " + order.ServiceItemHistorys.TravelDate.ToString("yyyy-MM-dd") + " 的其它行程中！请修改出行日期或更换电话号码后再保存。" });
                //        }
                //    }
                //}

                itemHistory.ElementsValue = order.ServiceItemHistorys.ElementsValue;
                itemHistory.ServiceItemTemplteValue = order.ServiceItemHistorys.ServiceItemTemplteValue;
                itemHistory.TravelDate = TravelDate;
                itemHistory.ReturnDate = ReturnDate;
                itemHistory.travellers = new List<OrderTraveller>();
                itemHistory.travellers = order.ServiceItemHistorys.travellers == null ? null : order.ServiceItemHistorys.travellers.Distinct().ToList();

                await HttpHelper.PutAction("ServiceItemHistoryExtend", JsonConvert.SerializeObject(itemHistory));
                //获取当前用户，创建操作记录orderHistorys
                string userName = User.Identity.Name;
                User user = await client.For<User>().Filter(u => u.UserName == userName).FindEntryAsync();
                item.state = item.state == OrderState.Notfilled ? OrderState.Filled : item.state;
                item.CustomerState = OrderStateHelper.GetOrderCustomerState(item.state, null);
                await client.For<Order>().Key(order.OrderID).Set(item).UpdateEntryAsync();
                await client.For<OrderHistory>().Set(new { OrderID = order.OrderID, OperUserID = user.UserID.ToString(), OperUserNickName = user.NickName, OperTime = DateTime.Now, State = item.state, Remark = "修改订单" }).InsertEntryAsync();
                //保存客户信息
                Customer oldCustomer = item.Customers;
                if (string.IsNullOrEmpty(oldCustomer.CustomerName))
                {
                    oldCustomer.CustomerName = order.Customers.CustomerName;
                    oldCustomer.CustomerEnname = order.Customers.CustomerEnname.ToUpper();
                    oldCustomer.Tel = order.Customers.Tel;
                    oldCustomer.BakTel = order.Customers.BakTel;
                    oldCustomer.Email = order.Customers.Email;
                    oldCustomer.Wechat = order.Customers.Wechat;
                    oldCustomer.Password = Md5Hash(order.Customers.Tel);
                    await client.For<Customer>().Key(oldCustomer.CustomerID).Set(oldCustomer).UpdateEntryAsync();
                }
                return JsonConvert.SerializeObject(new { ErrorCode = 200, ErrorMessage = "OK" });
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 400, ErrorMessage = "修改失败！失败原因：" + ex.Message });
            }
        }
        //修改附加项目数量
        [HttpPost]
        public async Task<string> UpdateExtraService(int OrderID, List<ExtraServiceHistory> extraServiceHistorys, List<TBOrderNoView> TBOrderNos)
        {
            try
            {
                Order order = await client.For<Order>().Expand(s => s.ServiceItemHistorys).Key(OrderID).FindEntryAsync();

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
                if (minNum > 0 && SelectNum == 0)
                {
                    return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "请至少选中一项额外项目" });
                }
                ServiceItemHistory itemHistory = order.ServiceItemHistorys;
                orderCost += itemHistory.PayType == PricingMethod.ByPerson
                    ? itemHistory.AdultNetPrice * itemHistory.AdultNum + itemHistory.ChildNetPrice * itemHistory.ChildNum + itemHistory.BobyNetPrice * itemHistory.INFNum
                    : itemHistory.Price * itemHistory.RoomNum * itemHistory.RightNum;
                itemHistory.TotalCost = orderCost;
                itemHistory.ExtraServiceHistorys = extraServiceHistorys;
                await HttpHelper.PostAction("ServiceItemHistoryExtend", JsonConvert.SerializeObject(itemHistory));

                //获取当前用户，创建操作记录orderHistorys
                string userName = User.Identity.Name;
                User user = await client.For<User>().Filter(u => u.UserName == userName).FindEntryAsync();
                await client.For<OrderHistory>().Set(new { OrderID = order.OrderID, OperUserID = user.UserID.ToString(), OperUserNickName = user.NickName, OperTime = DateTime.Now, State = order.state, Remark = "修改附加项目数量" }).InsertEntryAsync();
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 400, ErrorMessage = "修改失败！失败原因：" + ex.Message });
            }
            await AddTBOrderNos(OrderID, TBOrderNos);
            return JsonConvert.SerializeObject(new { ErrorCode = 200, ErrorMessage = "OK" });
        }
        //修改项目人数
        [HttpPost]
        public async Task<string> UpdateItemNum(int OrderID, int AdultNum, int ChildNum, int INFNum, int RoomNum, int RightNum, List<TBOrderNoView> TBOrderNos)
        {
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

                Order order = await client.For<Order>().Key(OrderID).FindEntryAsync();

                //获取当前用户，创建操作记录orderHistorys
                string userName = User.Identity.Name;
                User user = await client.For<User>().Filter(u => u.UserName == userName).FindEntryAsync();
                await client.For<OrderHistory>().Set(new { OrderID = order.OrderID, OperUserID = user.UserID.ToString(), OperUserNickName = user.NickName, OperTime = DateTime.Now, State = order.state, Remark = "修改项目人数" }).InsertEntryAsync();
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 400, ErrorMessage = "修改失败！失败原因：" + ex.Message });
            }
            await AddTBOrderNos(OrderID, TBOrderNos);
            return JsonConvert.SerializeObject(new { ErrorCode = 200, ErrorMessage = "OK" });
        }
        //更新模板
        [HttpPost]
        public async Task<string> UpdateTemplte(int? OrderID)
        {
            if (OrderID == null)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "订单不能为空" });
            }
            try
            {
                Order order = await client.For<Order>().Expand(o => o.ServiceItemHistorys).Key(OrderID).FindEntryAsync();
                ServiceItemHistory history = order.ServiceItemHistorys;
                int serviceitemID = history.ServiceItemID;
                int? TemplteID = null;
                try
                {
                    ServiceItem item = await client.For<ServiceItem>().Key(serviceitemID).FindEntryAsync();
                    TemplteID = item.ServiceItemTemplteID;
                }
                catch
                {

                }
                if (TemplteID == null)
                {
                    return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "更新失败，失败原因：找不到要更新的模板" });
                }
                else
                {
                    history.ServiceItemTemplteID = TemplteID;
                    await client.For<ServiceItemHistory>().Key(OrderID).Set(history).UpdateEntryAsync();
                }

                //获取当前用户，创建操作记录orderHistorys
                string userName = User.Identity.Name;
                User user = await client.For<User>().Filter(u => u.UserName == userName).FindEntryAsync();
                await client.For<OrderHistory>().Set(new { OrderID = order.OrderID, OperUserID = user.UserID.ToString(), OperUserNickName = user.NickName, OperTime = DateTime.Now, State = order.state, Remark = "更新模板" }).InsertEntryAsync();
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 400, ErrorMessage = "修改失败！失败原因：" + ex.Message });
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
        //检查出行日期
        [HttpPost]
        public async Task<string> CheckTravelDate(int OrderID, string CustomerName, string Tel, string TravelDate)
        {
            var failed = (new int[] { 1 }).Select(x => new { ErrorCode = 0, ErrorMessage = "" }).ToList();
            failed.Clear();
            List<DateTimeOffset> datelist = new List<DateTimeOffset>();
            try
            {
                datelist.Add(DateTimeOffset.Parse(TravelDate));
            }
            catch
            {
                failed.Add(new { ErrorCode = 400, ErrorMessage = "日期格式错误" });
                return JsonConvert.SerializeObject(failed);
            }
            Order order = await client.For<Order>().Expand(s => s.ServiceItemHistorys).Key(OrderID).FindEntryAsync();
            var orders = await client.For<Order>().Expand(s => s.ServiceItemHistorys)
                .Filter(s => s.CustomerID == order.CustomerID)
                .Filter(s => s.OrderID != OrderID)
                .Filter(s => s.state != OrderState.Invalid && s.state != OrderState.SencondCancel)
                .Select(s => new { s.ServiceItemHistorys.TravelDate, s.ServiceItemHistorys.ChangeTravelDate }).FindEntriesAsync();
            orders.ForEach(s => datelist.Add(s.ServiceItemHistorys.ChangeTravelDate > DateTimeOffset.MinValue ? s.ServiceItemHistorys.ChangeTravelDate : s.ServiceItemHistorys.TravelDate));
            var valid = datelist.Where(s => s > DateTimeOffset.Now.AddMonths(-1));
            if (valid != null && valid.Count() > 0 && (valid.Max() - valid.Min()).Days > 15)
            {
                failed.Add(new { ErrorCode = 401, ErrorMessage = "客人在系统中的最早出行日期 <span style='color:red'>" + valid.Min().ToString("yyyy-MM-dd") + "</span> 与最晚出行日期 <span style='color:red'>" + valid.Max().ToString("yyyy-MM-dd") + "</span> 相隔超过15天，请检查是否有误！确认无误需要保存请点【确认】，否则请点【取消】。" });
            }
            DateTimeOffset date = DateTimeOffset.Parse(TravelDate);
            if (order.ServiceItemHistorys.ServiceTypeID == 2)
            {
                Order ssorder = await client.For<Order>()
                .Filter(s => s.ServiceItemHistorys.ServiceTypeID == 2)//行程
                .Filter(s => s.CustomerID == order.CustomerID)
                .Filter(s => s.OrderID != order.OrderID)
                .Filter(s => s.state != OrderState.Invalid && s.state != OrderState.SencondCancel && s.state != OrderState.CancelReceive && s.state != OrderState.RequestCancel && s.state != OrderState.Cancel && s.state != OrderState.Notfilled)
                .Filter(s => s.ServiceItemHistorys.TravelDate == date || s.ServiceItemHistorys.ChangeTravelDate == date)
                .Select(s => new { s.OrderID, s.CustomerName, s.Tel }).FindEntryAsync();
                if (ssorder != null)
                {
                    if (ssorder.CustomerName == CustomerName)
                    {
                        failed.Add(new { ErrorCode = 401, ErrorMessage = "系统检测到 <span style='color:red'>" + CustomerName + "</span> 在 <span style='color:red'>" + TravelDate + "</span> 有其它行程安排！请确认是否修改出行日期或预订人姓名、电话号码。确认无误需要保存请点【确认】，否则请点【取消】。" });
                    }
                    else if (ssorder.Tel == Tel)
                    {
                        failed.Add(new { ErrorCode = 401, ErrorMessage = "系统检测到 <span style='color:red'>" + Tel + "</span> 这个号码出现在 <span style='color:red'>" + TravelDate + "</span> 的其它行程中！请确认是否修改出行日期或更换电话号码。确认无误需要保存请点【确认】，否则请点【取消】。" });
                    }
                }
                Order ccorder = await client.For<Order>()
                    .Filter(s => s.ServiceItemHistorys.ServiceTypeID == 2)//行程
                    .Filter(s => s.ServiceItemHistorys.ServiceItemID == order.ServiceItemHistorys.ServiceItemID)//同一行程
                    .Filter(s => s.ServiceItemHistorys.SupplierID == order.ServiceItemHistorys.SupplierID)
                    .Filter(s => s.state == OrderState.SencondCancel || s.state == OrderState.CancelReceive || s.state == OrderState.RequestCancel || s.state == OrderState.Cancel)
                    .Filter(s => s.CustomerID == order.CustomerID)
                    .Filter(s => s.ServiceItemHistorys.TravelDate == date || s.ServiceItemHistorys.ChangeTravelDate == date)
                    .Filter(s => s.CustomerName == CustomerName)
                    .Select(s => s.OrderID).FindEntryAsync();
                if (ccorder != null)
                {
                    failed.Add(new { ErrorCode = 401, ErrorMessage = "系统检测到 <span style='color:red'>" + CustomerName + "</span> 在 <span style='color:red'>" + TravelDate + "</span> 的同一行程曾经被取消！请确认是否修改姓名。确认无误需要保存请点【确认】，否则请点【取消】。" });
                }
            }
            return JsonConvert.SerializeObject(failed);
        }
        //保存请求变更
        [HttpPost]
        public async Task<string> SaveOrderChange(int OrderID, string ChangeValue, string ChangeElementsValue, List<TBOrderNoView> TBOrderNos)
        {
            try
            {
                Order order = await client.For<Order>().Expand(o => o.ServiceItemHistorys.ExtraServiceHistorys).Expand(o => o.Customers).Key(OrderID).FindEntryAsync();

                if (order.state < OrderState.SendReceive)
                {
                    return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "在供应商接单前不需要做请求变更，请直接修改" });
                }
                if (order.state == OrderState.Invalid)
                {
                    return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "订单已作废" });
                }
                if (order.state == OrderState.SencondCancel)
                {
                    return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "订单已取消" });
                }
                if (order.state == OrderState.Confirm || order.state == OrderState.Cancel || order.state == OrderState.Full)
                {
                    return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "订单处于待检状态，不能做请求变更" });
                }
                ServiceItemHistory history = order.ServiceItemHistorys;
                if (history.ChangeElementsValue == ChangeElementsValue)
                {
                    return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "请求变更失败！变更字段与前一次一样" });
                }

                string Remark = "";
                //记录变更系统字段
                if (!string.IsNullOrEmpty(ChangeElementsValue))
                {
                    //附加项目
                    string cnExName = "";
                    if (order.ServiceItemHistorys.ExtraServiceHistorys != null)
                    {
                        int exnum = 0;
                        foreach (var ex in order.ServiceItemHistorys.ExtraServiceHistorys)
                        {
                            if (ex.ServiceNum > 0)
                            {
                                if (exnum > 0)
                                {
                                    cnExName += ",";
                                }
                                cnExName += ex.ServiceName + " " + ex.ServiceNum + ex.ServiceUnit;
                                exnum++;
                            }
                        }
                    }
                    JObject jo = JObject.Parse(ChangeElementsValue);
                    JObject joTravel = JObject.Parse(ChangeValue);
                    if (joTravel.SelectToken("$..TravelDate") != null)
                    {
                        var date = DateTimeOffset.Parse(joTravel.SelectToken("$..TravelDate").ToString());
                        if (date != history.TravelDate)
                        {
                            history.ChangeTravelDate = date;
                        }
                        else
                        {
                            history.ChangeTravelDate = DateTimeOffset.MinValue;
                        }
                    }
                    int Nights;//晚数
                    DateTimeOffset TravelDate;//出行/入住日期
                    DateTimeOffset ReturnDate;//返回/退房日期
                    if (jo.SelectToken("$..Nights") != null)
                    {
                        Nights = int.Parse(jo.SelectToken("$..Nights").ToString());
                    }
                    else
                    {
                        Nights = history.RightNum;
                    }
                    if (jo.SelectToken("$..ServiceDate") != null)
                    {
                        TravelDate = DateTimeOffset.Parse(jo.SelectToken("$..ServiceDate").ToString());
                    }
                    else
                    {
                        TravelDate = history.TravelDate;
                    }
                    if (jo.SelectToken("$..BackDate") != null)
                    {
                        ReturnDate = DateTimeOffset.Parse(jo.SelectToken("$..BackDate").ToString());
                    }
                    else
                    {
                        ReturnDate = history.ReturnDate;
                    }
                    //Order ssorder = null;
                    //if (history.ServiceTypeID == 2)
                    //{
                    //    ssorder = await client.For<Order>()
                    //    .Filter(s => s.ServiceItemHistorys.ServiceTypeID == 2)//行程
                    //    .Filter(s => s.CustomerID == order.CustomerID)
                    //    .Filter(s => s.OrderID != order.OrderID)
                    //    .Filter(s => s.state != OrderState.Invalid && s.state != OrderState.SencondCancel && s.state != OrderState.CancelReceive && s.state != OrderState.RequestCancel && s.state != OrderState.Cancel && s.state != OrderState.Notfilled)
                    //    .Filter(s => s.ServiceItemHistorys.TravelDate == order.ServiceItemHistorys.TravelDate || s.ServiceItemHistorys.ChangeTravelDate == order.ServiceItemHistorys.TravelDate)
                    //    .Select(s => new { s.OrderID, s.CustomerName, s.Tel }).FindEntryAsync();
                    //    if (ssorder != null)
                    //    {
                    //        if (jo.SelectToken("$..cnName") != null && ssorder.CustomerName == jo.SelectToken("$..cnName").ToString())
                    //        {
                    //            return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "系统检测到 " + jo.SelectToken("$..cnName").ToString() + " 在 " + order.ServiceItemHistorys.TravelDate.ToString("yyyy-MM-dd") + " 有其它行程安排！请修改出行日期或预订人姓名、电话号码后再保存。" });
                    //        }
                    //        if (jo.SelectToken("$..Tel") != null && ssorder.Tel == jo.SelectToken("$..Tel").ToString())
                    //        {
                    //            return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "系统检测到 " + jo.SelectToken("$..Tel").ToString() + "  这个号码出现在 " + order.ServiceItemHistorys.TravelDate.ToString("yyyy-MM-dd") + " 的其它行程中！请修改出行日期或更换电话号码后再保存。" });
                    //        }
                    //    }
                    //}
                    foreach (var item in jo)
                    {
                        switch (item.Key)
                        {
                            case "cnName":
                                if (order.CustomerName != item.Value.ToString())
                                    Remark += "<div class='oneRechange'>联系人姓名(中文):" + order.CustomerName + "→" + item.Value.ToString() + "</div>";
                                break;
                            case "enName":
                                if (order.CustomerEnname != item.Value.ToString())
                                    Remark += "<div class='oneRechange'>联系人姓名(英文):" + order.CustomerEnname + "→" + item.Value.ToString() + "</div>";
                                break;
                            case "Tel":
                                if (order.Tel != item.Value.ToString())
                                    Remark += "<div class='oneRechange'>联系人电话:" + order.Tel + "→" + item.Value.ToString() + "</div>";
                                break;
                            case "BakTel":
                                if (order.BakTel != item.Value.ToString())
                                    Remark += "<div class='oneRechange'>备用联系人电话:" + order.BakTel + "→" + item.Value.ToString() + "</div>";
                                break;
                            case "Email":
                                if (order.Email != item.Value.ToString())
                                    Remark += "<div class='oneRechange'>联系人邮箱:" + order.Email + "→" + item.Value.ToString() + "</div>";
                                break;
                            case "Wechat":
                                if (order.Wechat != item.Value.ToString())
                                    Remark += "<div class='oneRechange'>联系人微信:" + order.Wechat + "→" + item.Value.ToString() + "</div>";
                                break;
                            case "Adult":
                                if (order.ServiceItemHistorys.AdultNum.ToString() != item.Value.ToString())
                                    Remark += "<div class='oneRechange'>成人:" + order.ServiceItemHistorys.AdultNum.ToString() + "→" + item.Value.ToString() + "</div>";
                                break;
                            case "Child":
                                if (order.ServiceItemHistorys.ChildNum.ToString() != item.Value.ToString())
                                    Remark += "<div class='oneRechange'>儿童:" + order.ServiceItemHistorys.ChildNum.ToString() + "→" + item.Value.ToString() + "</div>";
                                break;
                            case "Infant":
                                if (order.ServiceItemHistorys.INFNum.ToString() != item.Value.ToString())
                                    Remark += "<div class='oneRechange'>婴儿:" + order.ServiceItemHistorys.INFNum.ToString() + "→" + item.Value.ToString() + "</div>";
                                break;
                            case "Nights":
                                if (Nights == 0)
                                {
                                    return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "晚数不能为0！" });
                                }
                                if (order.ServiceItemHistorys.RightNum.ToString() != item.Value.ToString())
                                {
                                    Remark += "<div class='oneRechange'>晚数:" + order.ServiceItemHistorys.RightNum.ToString() + "→" + item.Value.ToString() + "</div>";
                                    if (history.ServiceTypeID == 4)
                                    {
                                        if ((ReturnDate - TravelDate).TotalDays != Nights)
                                        {
                                            return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "入住" + Nights + "晚，您选择的日期有误！" });
                                        }
                                    }
                                }
                                break;
                            case "NoOfRoom":
                                if (int.Parse(item.Value.ToString()) == 0)
                                {
                                    return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "间数不能为0！" });
                                }
                                if (order.ServiceItemHistorys.RoomNum.ToString() != item.Value.ToString())
                                    Remark += "<div class='oneRechange'>间数:" + order.ServiceItemHistorys.RoomNum.ToString() + "→" + item.Value.ToString() + "</div>";
                                break;
                            case "cnAttachedItem":
                                if (cnExName != item.Value.ToString())
                                    Remark += "<div class='oneRechange'>附加项目:" + cnExName + "→" + item.Value.ToString() + "</div>";
                                break;
                            case "ServiceDate":
                                if (await CheckRule(history.ServiceItemID, TravelDate))
                                {
                                    return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "出行日期不在规则允许范围内！" });
                                }
                                if (history.ServiceTypeID == 2)
                                {
                                    if (ReturnDate < TravelDate)
                                    {
                                        return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "行程类项目返回日期不能为空且不能早于出行日期！" });
                                    }
                                }
                                if (history.ServiceTypeID == 4)
                                {
                                    if ((ReturnDate.Date - TravelDate.Date).TotalDays != Nights)
                                    {
                                        return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "入住" + Nights + "晚，您选择的日期有误！" });
                                    }
                                }
                                break;
                            case "BackDate":
                                if (history.ServiceTypeID == 2)
                                {
                                    if (ReturnDate < TravelDate)
                                    {
                                        return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "行程类项目返回日期不能为空且不能早于出行日期！" });
                                    }
                                }
                                if (history.ServiceTypeID == 4)
                                {
                                    if ((ReturnDate.Date - TravelDate.Date).TotalDays != Nights)
                                    {
                                        return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "入住" + Nights + "晚，您选择的日期有误！" });
                                    }
                                }
                                break;
                        }
                    }
                }
                //记录变更表单字段
                if (!string.IsNullOrEmpty(ChangeElementsValue) && !string.IsNullOrEmpty(history.ServiceItemTemplteValue))
                {
                    string ServiceItemTemplteValue = history.ServiceItemTemplteValue;
                    string Elements = history.Elements;
                    JObject jo = JObject.Parse(ChangeElementsValue);
                    JObject joTemplte = JObject.Parse(ServiceItemTemplteValue);
                    JObject joElements = JObject.Parse(Elements);
                    foreach (var item in jo)
                    {

                        var change = joTemplte.SelectToken("$.." + item.Key);
                        if (change != null)
                        {

                            //根据字段代号获取字段名称
                            string title = "";
                            var titles = joElements.SelectTokens("$..elements.*");
                            foreach (var tt in titles)
                            {

                                if (tt.SelectToken("$..code") != null)
                                {

                                    if (item.Key == tt.SelectToken("$..code").ToString())
                                    {

                                        if (tt.SelectToken("$..title") != null)
                                        {

                                            title = tt.SelectToken("$..title").ToString();
                                            break;
                                        }
                                    }
                                }
                            }
                            var type = item.Value.SelectToken("type");
                            if (type != null && type.ToString() == "PersonPicker")
                            {
                                var list = item.Value.SelectToken("value");
                                var changelist = change.SelectToken("value");
                                string PersonStr = "";
                                foreach (var one in list)
                                {
                                    PersonStr += one.SelectToken("TravellerName").ToString() + ",";
                                }
                                string changePersonStr = "";
                                foreach (var one in changelist)
                                {
                                    changePersonStr += one.SelectToken("TravellerName").ToString() + ",";
                                }
                                if (title != "")
                                {
                                    Remark += "<div class='oneRechange'>" + title + ":" + changePersonStr + "→" + PersonStr + "</div>";
                                }
                            }
                            else
                            {
                                if (item.Value.ToString() != change.ToString())
                                {
                                    if (title != "")
                                    {
                                        Remark += "<div class='oneRechange'>" + title + ":" + change.ToString() + "→" + item.Value.ToString() + "</div>";
                                    }
                                }
                            }
                        }
                    }
                }
                if (string.IsNullOrEmpty(history.ChangeElementsValue))
                {
                    if (Remark == "")
                    {
                        return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "请求变更失败！未检测到变更字段" });
                    }
                }
                history.ChangeValue = ChangeValue;
                history.ChangeElementsValue = ChangeElementsValue;
                await client.For<ServiceItemHistory>().Key(OrderID).Set(history).UpdateEntryAsync();

                Supplier supplier = await client.For<Supplier>()
                    .Expand(s => s.SupplierUsers)
                    .Filter(s => s.SupplierID == history.SupplierID && s.SupplierEnableState == EnableState.Enable && s.EnableOnline)
                    .FindEntryAsync();

                string userName = User.Identity.Name;
                User user = await client.For<User>().Filter(u => u.UserName == userName).FindEntryAsync();
                order.state = OrderState.RequestChange;
                order.CustomerState = OrderStateHelper.GetOrderCustomerState(order.state, null);
                await client.For<Order>().Key(order.OrderID).Set(order).UpdateEntryAsync();
                await client.For<OrderHistory>().Set(new { OrderID = order.OrderID, OperUserID = user.UserID.ToString(), OperUserNickName = user.NickName, OperTime = DateTime.Now, State = order.state, Remark = Remark }).InsertEntryAsync();
                await client.For<OrderSupplierHistory>().Set(new { opera = OrderOperator.inside, State = OrderState.RequestChange, OrderID = order.OrderID, OperUserID = user.UserID, OperTime = DateTimeOffset.Now, OperNickName = "(浪花朵朵)" + user.NickName, Remark = Remark }).InsertEntryAsync();

                #region 供应商微信推送
                if (supplier != null && supplier.SupplierUsers != null && supplier.SupplierUsers.Count > 0)
                {
                    foreach (var item in supplier.SupplierUsers)
                    {
                        if (!string.IsNullOrEmpty(item.OpenID) && item.RealTimeMessage)
                        {
                            bool Disturb = false;//true为免打扰
                            if (item.Disturb)
                            {
                                try
                                {
                                    string BeginTime = item.BeginTime;
                                    string EndTime = item.EndTime;
                                    //免打扰开始时间大于结束时间,按跨天算,当前时间大于开始时间或者小于结束时间则开启免打扰，不发实时消息
                                    if (int.Parse(BeginTime.Split(':')[0]) > int.Parse(EndTime.Split(':')[0])
                                        || (int.Parse(BeginTime.Split(':')[0]) == int.Parse(EndTime.Split(':')[0]) && int.Parse(BeginTime.Split(':')[1]) > int.Parse(EndTime.Split(':')[1])))
                                    {
                                        if (DateTimeOffset.Now.Hour > int.Parse(BeginTime.Split(':')[0])
                                            || (DateTimeOffset.Now.Hour == int.Parse(BeginTime.Split(':')[0]) && DateTimeOffset.Now.Minute > int.Parse(BeginTime.Split(':')[1]))
                                            || DateTimeOffset.Now.Hour < int.Parse(EndTime.Split(':')[0])
                                            || (DateTimeOffset.Now.Hour == int.Parse(EndTime.Split(':')[0]) && DateTimeOffset.Now.Minute < int.Parse(EndTime.Split(':')[1])))
                                        {
                                            Disturb = true;//免打扰
                                        }
                                    }
                                    else//免打扰开始时间小于结束时间,按当天算,当前时间大于开始时间并且小于结束时间则开启免打扰，不发实时消息
                                    {
                                        if ((DateTimeOffset.Now.Hour > int.Parse(BeginTime.Split(':')[0])
                                            || (DateTimeOffset.Now.Hour == int.Parse(BeginTime.Split(':')[0]) && DateTimeOffset.Now.Minute > int.Parse(BeginTime.Split(':')[1])))
                                            && (DateTimeOffset.Now.Hour < int.Parse(EndTime.Split(':')[0])
                                            || (DateTimeOffset.Now.Hour == int.Parse(EndTime.Split(':')[0]) && DateTimeOffset.Now.Minute < int.Parse(EndTime.Split(':')[1]))))
                                        {
                                            Disturb = true;//免打扰
                                        }
                                    }

                                }
                                catch
                                {

                                }
                            }
                            if (!Disturb)
                            {
                                try
                                {
                                    var supplierMessage = WeiXinHelper.SupplierSendMessage(item.OpenID,
                                    "您好，您有一条订单等待处理。" + " \r\n ",
                                    "请求变更",
                                    history.cnItemName,
                                    order.OrderNo,
                                    " \r\n 请及时登录系统后台进行处理，谢谢！");
                                    await client.For<SystemLog>().Set(new { Operate = "供应商微信推送", OperateTime = DateTime.Now, UserID = item.SupplierUserID, UserName = item.SupplierNickName, Remark = "OrderID：" + order.OrderID + " Message：" + supplierMessage }).InsertEntryAsync();
                                }
                                catch (Exception ex)
                                {
                                    await client.For<SystemLog>().Set(new { Operate = "供应商微信推送失败", OperateTime = DateTime.Now, UserID = item.SupplierUserID, UserName = item.SupplierNickName, Remark = "OrderID：" + order.OrderID + " Message：" + ex.Message }).InsertEntryAsync();
                                }
                            }
                        }
                    }
                }
                #endregion
                await AddTBOrderNos(OrderID, TBOrderNos);
                return JsonConvert.SerializeObject(new { ErrorCode = 200, ErrorMessage = "OK" });
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 400, ErrorMessage = "请求变更失败！失败原因：" + ex.Message });
            }
        }
        //查询额外项目
        public async Task<string> GetExtraServiceByID(int ItemID, int SupplierID)
        {
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
        /// <summary>
        /// 检查出行日期是否被规则禁止，被禁止则返回true
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
        //换产品生成新订单
        [HttpPost]
        public async Task<string> ChangeProduct(int? OrderID, int? ItemID, int? SupplierID, string ElementsValue, List<TBOrderNoView> TBOrderNos)
        {
            if (OrderID == null)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "订单不能为空" });
            }
            if (ItemID == null)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "产品不能为空" });
            }
            if (SupplierID == null)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "供应商不能为空" });
            }

            string username = User.Identity.Name;
            User user = await client.For<User>().Filter(u => u.UserName == username).FindEntryAsync();

            ServiceItem item = await client.For<ServiceItem>().Key(ItemID).FindEntryAsync();

            Supplier supplier = await client.For<Supplier>().Key(SupplierID).FindEntryAsync();

            SupplierServiceItem itemprice = await client.For<SupplierServiceItem>()
                .Expand(u => u.ItemCurrency)
                .Expand(u => u.ItemPriceBySuppliers)
                .Filter(u => u.ServiceItemID == ItemID && u.SupplierID == SupplierID)
                .FindEntryAsync();

            Order order = await client.For<Order>()
                .Expand(s => s.TBOrders)
                .Expand(s => s.TBOrderNos)
                .Expand(s => s.ServiceItemHistorys.travellers)
                .Key(OrderID).FindEntryAsync();

            float AdultNetPrice = 0;
            float ChildNetPrice = 0;
            float BobyNetPrice = 0;
            float Price = 0;
            if (itemprice != null)
            {
                foreach (var priceby in itemprice.ItemPriceBySuppliers)
                {
                    if (priceby.startTime.Date <= DateTimeOffset.Now.Date && DateTimeOffset.Now.Date <= priceby.EndTime.Date)
                    {
                        AdultNetPrice = priceby.AdultNetPrice;
                        ChildNetPrice = priceby.ChildNetPrice;
                        BobyNetPrice = priceby.BobyNetPrice;
                        Price = priceby.Price;
                        break;
                    }
                }
            }

            if (item.ServiceTypeID != order.ServiceItemHistorys.ServiceTypeID)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "不同产品类型不能更换" });
            }

            order.IsPay = false;
            order.IsNeedCustomerService = false;
            order.Remark = "";
            order.state = OrderState.Notfilled;
            order.CustomerState = OrderCustomerState.Notfilled;
            order.CreateTime = DateTimeOffset.Now;

            //order.CreateUserID = user.UserID;
            //order.CreateUserNikeName = user.NickName;

            order.OrderHistorys = new List<OrderHistory>();
            OrderHistory orderhistory = new OrderHistory();
            orderhistory.OperUserID = user.UserID.ToString();
            orderhistory.OperUserNickName = user.NickName;

            orderhistory.OperTime = DateTimeOffset.Now;
            orderhistory.Remark = "从订单\"" + order.OrderNo + "\"换产品生成";
            orderhistory.State = OrderState.Notfilled;
            order.OrderHistorys.Add(orderhistory);

            order.ServiceItemHistorys.GroupNo = "";
            order.ServiceItemHistorys.ReceiveManTime = "";
            order.ServiceItemHistorys.CreateTime = DateTimeOffset.Now;
            order.ServiceItemHistorys.AdultNetPrice = AdultNetPrice;
            order.ServiceItemHistorys.ChildNetPrice = ChildNetPrice;
            order.ServiceItemHistorys.BobyNetPrice = BobyNetPrice;
            order.ServiceItemHistorys.Price = Price;

            order.ServiceItemHistorys.ServiceItemID = item.ServiceItemID;
            order.ServiceItemHistorys.ServiceCode = item.ServiceCode;
            order.ServiceItemHistorys.cnItemName = item.cnItemName;
            order.ServiceItemHistorys.enItemName = item.enItemName;
            order.ServiceItemHistorys.ServiceItemTemplteID = item.ServiceItemTemplteID;
            order.ServiceItemHistorys.Elements = item.ElementContent;
            order.ServiceItemHistorys.ElementsValue = ElementsValue;
            order.ServiceItemHistorys.TravelCompany = item.TravelCompany;
            order.ServiceItemHistorys.InsuranceDays = item.InsuranceDays;
            order.ServiceItemHistorys.FixedDays = item.FixedDays;

            order.ServiceItemHistorys.SupplierID = supplier.SupplierID;
            order.ServiceItemHistorys.SupplierCode = supplier.SupplierNo;
            order.ServiceItemHistorys.SupplierName = supplier.SupplierName;

            order.ServiceItemHistorys.CurrencyID = itemprice.ItemCurrency.CurrencyID;
            order.ServiceItemHistorys.CurrencyName = itemprice.ItemCurrency.CurrencyName;
            order.ServiceItemHistorys.CurrencyChangeType = itemprice.ItemCurrency.CurrencyChangeType;
            order.ServiceItemHistorys.ExchangeRate = itemprice.ItemCurrency.ExchangeRate;
            order.ServiceItemHistorys.PayType = itemprice.PayType;


            order.TBOrders.TotalCost = 0;
            order.TBOrders.TotalReceive = 0;

            try
            {
                string changevalue = order.ServiceItemHistorys.ChangeValue;
                if (!string.IsNullOrEmpty(changevalue))
                {
                    JObject jo = JObject.Parse(changevalue);
                    var systemMap = jo.SelectToken("$..systemMap");
                    if (systemMap != null)
                    {
                        var TravelDate = systemMap.SelectToken("$..TravelDate") == null ? "" : systemMap.SelectToken("$..TravelDate").ToString();
                        var ReturnDate = systemMap.SelectToken("$..ReturnDate") == null ? "" : systemMap.SelectToken("$..ReturnDate").ToString();
                        order.ServiceItemHistorys.TravelDate = string.IsNullOrEmpty(TravelDate) ? order.ServiceItemHistorys.TravelDate : DateTimeOffset.Parse(TravelDate);
                        order.ServiceItemHistorys.ReturnDate = string.IsNullOrEmpty(ReturnDate) ? order.ServiceItemHistorys.ReturnDate : DateTimeOffset.Parse(ReturnDate);
                    }
                    var systemFile = jo.SelectToken("$..nameAndNum");
                    if (systemFile != null)
                    {
                        var AdultNum = systemFile.SelectToken("$..AdultNum") == null ? "" : systemFile.SelectToken("$..AdultNum").ToString();
                        var ChildNum = systemFile.SelectToken("$..ChildNum") == null ? "" : systemFile.SelectToken("$..ChildNum").ToString();
                        var INFNum = systemFile.SelectToken("$..INFNum") == null ? "" : systemFile.SelectToken("$..INFNum").ToString();
                        var RightNum = systemFile.SelectToken("$..RightNum") == null ? "" : systemFile.SelectToken("$..RightNum").ToString();
                        var RoomNum = systemFile.SelectToken("$..RoomNum") == null ? "" : systemFile.SelectToken("$..RoomNum").ToString();
                        var CustomerName = systemFile.SelectToken("$..CustomerName") == null ? "" : systemFile.SelectToken("$..CustomerName").ToString();
                        var CustomerEnname = systemFile.SelectToken("$..CustomerEnname") == null ? "" : systemFile.SelectToken("$..CustomerEnname").ToString();
                        var Tel = systemFile.SelectToken("$..Tel") == null ? "" : systemFile.SelectToken("$..Tel").ToString();
                        var BakTel = systemFile.SelectToken("$..BakTel") == null ? "" : systemFile.SelectToken("$..BakTel").ToString();
                        var Email = systemFile.SelectToken("$..Email") == null ? "" : systemFile.SelectToken("$..Email").ToString();
                        var Wechat = systemFile.SelectToken("$..Wechat") == null ? "" : systemFile.SelectToken("$..Wechat").ToString();

                        order.ServiceItemHistorys.AdultNum = string.IsNullOrEmpty(AdultNum) ? order.ServiceItemHistorys.AdultNum : int.Parse(AdultNum);
                        order.ServiceItemHistorys.ChildNum = string.IsNullOrEmpty(ChildNum) ? order.ServiceItemHistorys.ChildNum : int.Parse(ChildNum);
                        order.ServiceItemHistorys.INFNum = string.IsNullOrEmpty(INFNum) ? order.ServiceItemHistorys.INFNum : int.Parse(INFNum);
                        order.ServiceItemHistorys.RightNum = string.IsNullOrEmpty(RightNum) ? order.ServiceItemHistorys.RightNum : int.Parse(RightNum);
                        order.ServiceItemHistorys.RoomNum = string.IsNullOrEmpty(RoomNum) ? order.ServiceItemHistorys.RoomNum : int.Parse(RoomNum);
                        order.CustomerName = string.IsNullOrEmpty(CustomerName) ? order.CustomerName : CustomerName;
                        order.CustomerEnname = string.IsNullOrEmpty(CustomerEnname) ? order.CustomerEnname : CustomerEnname;
                        order.Tel = string.IsNullOrEmpty(Tel) ? order.Tel : Tel;
                        order.BakTel = string.IsNullOrEmpty(BakTel) ? order.BakTel : BakTel;
                        order.Email = string.IsNullOrEmpty(Email) ? order.Email : Email;
                        order.Wechat = string.IsNullOrEmpty(Wechat) ? order.Wechat : Wechat;
                    }
                    order.ServiceItemHistorys.ChangeValue = null;//将变更值清空
                    order.ServiceItemHistorys.ChangeElementsValue = null;//将变更值清空
                }
            }
            catch
            {

            }

            order.ServiceItemHistorys.TotalCost = itemprice.PayType == PricingMethod.ByPerson
                ? AdultNetPrice * order.ServiceItemHistorys.AdultNum + ChildNetPrice * order.ServiceItemHistorys.ChildNum + BobyNetPrice * order.ServiceItemHistorys.INFNum
                : Price * order.ServiceItemHistorys.RightNum * order.ServiceItemHistorys.RoomNum;

            HttpResponseMessage Message = await HttpHelper.PostAction("OrdersExtend", JsonConvert.SerializeObject(order));
            try
            {
                int id = int.Parse(Message.Content.ReadAsStringAsync().Result);
                await AddTBOrderNos(id, TBOrderNos);
                return JsonConvert.SerializeObject(new { ErrorCode = 200, ErrorMessage = "OK", OrderID = id });
            }
            catch
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 400, ErrorMessage = Message.Content.ReadAsStringAsync().Result });
            }
        }
        //导出接机送机
        public async Task<ActionResult> execl()
        {
            ViewBag.Supplier = await client.For<Supplier>().OrderBy(s => s.SupplierNo).FindEntriesAsync();
            return View();
        }
        [HttpPost]
        public async Task<FileResult> execl(string start, string end, string SupplierID, string type)
        {
            #region 获取列表
            IBoundClient<Order> OrderResult = client.For<Order>()
                .Expand(t => t.TBOrders)
                .Expand(t => t.ServiceItemHistorys);
            OrderResult = OrderResult.Filter("state ne LanghuaNew.Data.OrderState'13'");
            if (type == "car")
            {
                OrderResult = OrderResult.Filter("ServiceItemHistorys/ServiceTypeID eq 1");
            }
            else
            {
                OrderResult = OrderResult.Filter("ServiceItemHistorys/ServiceTypeID ne 1");
            }
            if (!string.IsNullOrEmpty(SupplierID))
            {
                OrderResult = OrderResult.Filter("ServiceItemHistorys/SupplierID eq " + SupplierID);
            }

            if (!string.IsNullOrEmpty(start))
            {
                try
                {
                    DateTime startTime = DateTime.Parse(start);
                    string filter = "ServiceItemHistorys/TravelDate ge " + startTime.ToString("yyyy-MM-dd");
                    OrderResult = OrderResult.Filter(filter);
                }
                catch { }
            }
            if (!string.IsNullOrEmpty(end))
            {
                try
                {
                    DateTime endTime = DateTime.Parse(end).AddDays(1);
                    string filter = "ServiceItemHistorys/TravelDate lt " + endTime.ToString("yyyy-MM-dd");
                    OrderResult = OrderResult.Filter(filter);
                }
                catch { }
            }
            #endregion

            //创建Excel文件的对象
            NPOI.HSSF.UserModel.HSSFWorkbook book = new NPOI.HSSF.UserModel.HSSFWorkbook();
            //添加一个sheet
            NPOI.SS.UserModel.ISheet sheet1 = book.CreateSheet("Sheet1");
            int i = 0;
            //给sheet1添加第一行的头部标题
            NPOI.SS.UserModel.IRow row1 = sheet1.CreateRow(i);
            row1.CreateCell(0).SetCellValue("供应商");
            row1.CreateCell(1).SetCellValue("订单号");
            row1.CreateCell(2).SetCellValue("姓名");
            row1.CreateCell(3).SetCellValue("预订产品");
            row1.CreateCell(4).SetCellValue("人数");
            row1.CreateCell(5).SetCellValue("日期");
            if (type == "car")
            {
                row1.CreateCell(6).SetCellValue("时间");
                row1.CreateCell(7).SetCellValue("航班号");
                row1.CreateCell(8).SetCellValue("航站楼");
                row1.CreateCell(9).SetCellValue("接人酒店区域");
                row1.CreateCell(10).SetCellValue("接人酒店");
                row1.CreateCell(11).SetCellValue("返回酒店区域");
                row1.CreateCell(12).SetCellValue("返回酒店");
                row1.CreateCell(13).SetCellValue("状态");
            }
            else
            {
                row1.CreateCell(6).SetCellValue("接人酒店区域");
                row1.CreateCell(7).SetCellValue("接人酒店");
                row1.CreateCell(8).SetCellValue("返回酒店区域");
                row1.CreateCell(9).SetCellValue("返回酒店");
                row1.CreateCell(10).SetCellValue("团号");
                row1.CreateCell(11).SetCellValue("接人时间");
                row1.CreateCell(12).SetCellValue("状态");
            }
            row1.Height = 450;
            //将数据逐步写入sheet1各个行
            var items = await OrderResult.FindEntriesAsync();
            foreach (var item in items)
            {
                i++;
                NPOI.SS.UserModel.IRow rowtemp = sheet1.CreateRow(i);
                rowtemp.CreateCell(0).SetCellValue(item.ServiceItemHistorys.SupplierCode + " - " + item.ServiceItemHistorys.SupplierName);
                rowtemp.CreateCell(1).SetCellValue(item.OrderNo);
                rowtemp.CreateCell(2).SetCellValue(item.CustomerName);
                rowtemp.CreateCell(3).SetCellValue(item.ServiceItemHistorys.cnItemName);
                rowtemp.CreateCell(4).SetCellValue(item.ServiceItemHistorys.AdultNum + "+" + item.ServiceItemHistorys.ChildNum + "+" + item.ServiceItemHistorys.INFNum);
                rowtemp.CreateCell(5).SetCellValue(item.ServiceItemHistorys.TravelDate > DateTimeOffset.Parse("1900-01-02") ? item.ServiceItemHistorys.TravelDate.ToString("yyyy-MM-dd") : "");
                var value = item.ServiceItemHistorys.ServiceItemTemplteValue;
                if (!string.IsNullOrEmpty(value))
                {
                    JObject jo = JObject.Parse(value);
                    if (type == "car")
                    {
                        string time = "";
                        if (jo.SelectToken("ServiceTime") != null)
                        {
                            time = jo.SelectToken("ServiceTime").ToString();
                        }
                        if (jo.SelectToken("PickTime") != null)
                        {
                            try
                            {
                                time = jo.SelectToken("PickTime").ToString().Split(' ')[1];
                            }
                            catch { }
                        }
                        if (jo.SelectToken("ArrivalTime") != null)
                        {
                            try
                            {
                                time = jo.SelectToken("ArrivalTime").ToString().ToString().Split(' ')[1];
                            }
                            catch { }
                        }
                        rowtemp.CreateCell(6).SetCellValue(time);
                        rowtemp.CreateCell(7).SetCellValue(jo.SelectToken("FlightNo") == null ? "" : jo.SelectToken("FlightNo").ToString());
                        rowtemp.CreateCell(8).SetCellValue(jo.SelectToken("Terminal") == null ? "" : jo.SelectToken("Terminal").ToString());
                        rowtemp.CreateCell(9).SetCellValue(jo.SelectToken("PickupHotelArea") == null ? "" : jo.SelectToken("PickupHotelArea").ToString());
                        rowtemp.CreateCell(10).SetCellValue(jo.SelectToken("PickupHotelName") == null ? "" : jo.SelectToken("PickupHotelName").ToString());
                        rowtemp.CreateCell(11).SetCellValue(jo.SelectToken("ReturnHotelArea") == null ? "" : jo.SelectToken("ReturnHotelArea").ToString());
                        rowtemp.CreateCell(12).SetCellValue(jo.SelectToken("ReturnHotelName") == null ? "" : jo.SelectToken("ReturnHotelName").ToString());
                    }
                    else
                    {
                        rowtemp.CreateCell(6).SetCellValue(jo.SelectToken("PickupHotelArea") == null ? "" : jo.SelectToken("PickupHotelArea").ToString());
                        rowtemp.CreateCell(7).SetCellValue(jo.SelectToken("PickupHotelName") == null ? "" : jo.SelectToken("PickupHotelName").ToString());
                        rowtemp.CreateCell(8).SetCellValue(jo.SelectToken("ReturnHotelArea") == null ? "" : jo.SelectToken("ReturnHotelArea").ToString());
                        rowtemp.CreateCell(9).SetCellValue(jo.SelectToken("ReturnHotelName") == null ? "" : jo.SelectToken("ReturnHotelName").ToString());
                    }
                }
                if (type == "car")
                {
                    rowtemp.CreateCell(13).SetCellValue(EnumHelper.GetEnumDescription(item.state).Substring(0, EnumHelper.GetEnumDescription(item.state).IndexOf("|")));
                }
                else
                {
                    rowtemp.CreateCell(10).SetCellValue(item.ServiceItemHistorys.GroupNo == null ? "" : item.ServiceItemHistorys.GroupNo);
                    rowtemp.CreateCell(11).SetCellValue(item.ServiceItemHistorys.ReceiveManTime == null ? "" : item.ServiceItemHistorys.ReceiveManTime);
                    rowtemp.CreateCell(12).SetCellValue(EnumHelper.GetEnumDescription(item.state).Substring(0, EnumHelper.GetEnumDescription(item.state).IndexOf("|")));
                }
            }
            // 写入到客户端 
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            book.Write(ms);
            ms.Seek(0, SeekOrigin.Begin);
            return File(ms, "application/vnd.ms-excel", "execllist.xls");
        }
        public async Task<ActionResult> TBOrderDetail(int? id, string Type)
        {
            if (id == null || id == 0)
            {
                return HttpNotFound();
            }
            TBOrder tBOrder = await client.For<TBOrder>().Key(id).FindEntryAsync();
            if (tBOrder == null)
            {
                return HttpNotFound();
            }
            var result = client.For<Order>()
                    .Expand(u => u.ServiceItemHistorys.ExtraServiceHistorys)
                    .Expand(u => u.Customers);
            if (Type == "All")
            {
                result = result.Filter(u => u.TBOrders.TBID == tBOrder.TBID);
            }
            else
            {
                result = result.Filter(u => u.TBOrders.TBOrderID == id);
            }
            var orders = await result.FindEntriesAsync();
            if (orders == null || orders.Count() == 0)
            {
                return HttpNotFound();
            }
            List<Order> items = new List<Order>();
            foreach (var item in orders)
            {
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
            tBOrder.Orders = orders.ToList();
            ViewBag.Type = Type;
            return View(tBOrder);
        }
        public async Task<string> GetTBList(string BuyerNick)
        {
            var StartDate = DateTimeOffset.Parse(DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd"));

            var orders = client.For<Order>().Filter(o => o.TBOrderNos.All(s => s.No == null));
            var aaaaaa = await orders.GetCommandTextAsync();
            try
            {
                if (string.IsNullOrEmpty(BuyerNick))
                {
                    return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "淘宝ID不能为空" });
                }
                TB_Access_Token tb_Access_Toen = null;
                var list = await client.For<TB_Access_Token>().FindEntriesAsync();
                if (list != null && list.Count() > 0)
                {
                    TB_Access_Token item = list.FirstOrDefault();
                    //判断token是否过期
                    var startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1, 0, 0, 0));
                    if (DateTime.Compare(startTime.AddSeconds(item.expire_time / 1000), DateTime.Now.AddDays(1)) > 0)
                    {
                        tb_Access_Toen = item;
                    }
                }
                if (tb_Access_Toen != null)
                {
                    string url = ConfigurationManager.AppSettings["bizInterface_Trades"];
                    ITopClient client = new DefaultTopClient(url,
                        ConfigurationManager.AppSettings["client_id"],
                        ConfigurationManager.AppSettings["client_secret"]);
                    TradesSoldGetRequest req = new TradesSoldGetRequest();
                    req.StartCreated = DateTime.Now.AddMonths(-1);
                    req.EndCreated = DateTime.Now;
                    req.Fields = "tid,pic_path";
                    req.BuyerNick = BuyerNick;
                    req.PageNo = 1L;
                    req.PageSize = 100L;
                    TradesSoldGetResponse rsp = client.Execute(req, tb_Access_Toen.access_token);
                    //新接口
                    //List<TBDetail> DetailList = new List<TBDetail>();
                    //foreach (var item in rsp.Trades)
                    //{
                    //    //遍历Tid获取单笔交易的详细信息
                    //    AlitripTravelTradeQueryRequest reqFull = new AlitripTravelTradeQueryRequest();
                    //    reqFull.OrderId = item.Tid;
                    //    AlitripTravelTradeQueryResponse repFull = client.Execute(reqFull, tb_Access_Toen.access_token);
                    //    if (repFull.FirstResult != null)
                    //    {
                    //        TBDetail detail = new TBDetail();
                    //        var TBDetailconfig = new MapperConfiguration(cfg => cfg.CreateMap<AlitripTravelTradeQueryResponse.TopTripOrderResultDomain, TBDetail>());
                    //        var TBDetailmapper = TBDetailconfig.CreateMapper();
                    //        detail = TBDetailmapper.Map<TBDetail>(repFull.FirstResult);
                    //        detail.PicPath = item.PicPath;
                    //        DetailList.Add(detail);
                    //    }
                    //}
                    //if (DetailList != null && DetailList.Count > 0)
                    //{
                    //    var data = DetailList.Select(s => new
                    //    {
                    //        s.OrderId,
                    //        OrderNo = GetOrderNo(s.OrderId.ToString()),
                    //        s.CreatedTime,
                    //        s.PayInfo,
                    //        s.SellerInfo,
                    //        s.Status,
                    //        s.PromotionDetails,
                    //        s.PicPath,
                    //        Orders = s.SubOrders.Select(a => new
                    //        {
                    //            a.BuyItemInfo,
                    //            a.Contactor,
                    //            a.SubOrderId,
                    //            a.Status,
                    //            a.TotalFee,
                    //            a.Payment,
                    //            a.DiscountFee,
                    //            a.RefundStatus,
                    //            a.RefundId
                    //        })
                    //    });
                    //    return JsonConvert.SerializeObject(new { ErrorCode = 200, ErrorMessage = "OK", BuyerNick, TotalResults = rsp.TotalResults, data });
                    //}
                    //else
                    //{
                    //    return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "未查找到该用户<span style='color:red'>" + BuyerNick + "</span>一个月内的订单" });
                    //}
                    List<AlitripTravelTradeFullinfoGetResponse.TradeDetailDomain> DetailList = new List<AlitripTravelTradeFullinfoGetResponse.TradeDetailDomain>();
                    foreach (var item in rsp.Trades)
                    {
                        //遍历Tid获取单笔交易的详细信息
                        AlitripTravelTradeFullinfoGetRequest reqFull = new AlitripTravelTradeFullinfoGetRequest();
                        reqFull.Tid = item.Tid;
                        AlitripTravelTradeFullinfoGetResponse repFull = client.Execute(reqFull, tb_Access_Toen.access_token);
                        DetailList.Add(repFull.TradeFullinfo);
                    }
                    if (DetailList != null && DetailList.Count > 0)
                    {
                        var data = DetailList.Select(s => new
                        {
                            s.Tid,
                            OrderNo = GetOrderNo(s.Tid.ToString()),
                            s.Created,
                            s.PayTime,
                            s.BuyerAlipayNo,
                            s.BuyerNick,
                            s.SellerFlag,
                            s.SellerMemo,
                            s.Payment,
                            s.Status,
                            s.PromotionDetails,
                            Orders = s.Orders.Select(a => new
                            {
                                a.Title,
                                a.Oid,
                                a.Status,
                                a.Price,
                                a.Num,
                                a.TotalFee,
                                a.Payment,
                                a.DiscountFee,
                                a.PicPath,
                                a.RefundStatus,
                                a.OuterSkuId,
                                a.SkuPropertiesName,
                                a.ItineraryInfo,
                                a.RefundId
                            })
                        });
                        return JsonConvert.SerializeObject(new { ErrorCode = 200, ErrorMessage = "OK", BuyerNick, TotalResults = rsp.TotalResults, data });
                    }
                    else
                    {
                        return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "未查找到该用户<span style='color:red'>" + BuyerNick + "</span>一个月内的订单" });
                    }
                }
                else
                {
                    return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "淘宝授权已过期，请联系系统管理员" });
                }
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 400, ErrorMessage = ex.Message });
            }
        }
        private string GetOrderNo(string TBNum)
        {
            TBNum = TBNum == null ? "" : TBNum.Trim();
            string result = HttpHelper.GetActionForOdata("odata/Orders?$filter=(TBOrderNos/any(x1:x1/No eq '" + TBNum + "') or TBNum eq '" + TBNum + "') and (state ne LanghuaNew.Data.OrderState'Invalid')").Result;
            try
            {
                JObject ja = (JObject)JsonConvert.DeserializeObject(result);
                string str = ja["value"].ToString();
                List<Order> orders = JsonConvert.DeserializeObject<List<Order>>(str);
                Order order = orders[0];
                return order.OrderNo;
            }
            catch
            {
                return "";
            }
        }
        [HttpPost]
        public async Task<string> AddTBOrderNos(int OrderID, List<TBOrderNoView> TBOrderNos)
        {
            if (TBOrderNos != null)
            {
                foreach (var no in TBOrderNos)
                {
                    TBOrderNo oo = await client.For<TBOrderNo>().Filter(s => s.OrderID == OrderID && s.SubNo == no.SubNo).FindEntryAsync();
                    if (oo != null)
                    {
                        continue;
                    }
                    TBOrderNo one = new TBOrderNo();
                    one.OrderID = OrderID;
                    one.No = no.No;
                    one.SubNo = no.SubNo;
                    one.Payment = no.Payment;
                    one.PaymentSplit = no.Payment;
                    //如果有退款，就查退款金额
                    if (no.RefundId > 0)
                    {
                        try
                        {
                            TB_Access_Token tb_Access_Toen = await client.For<TB_Access_Token>().FindEntryAsync();
                            if (tb_Access_Toen != null)
                            {
                                string url = ConfigurationManager.AppSettings["bizInterface_Trades"];
                                ITopClient Iclient = new DefaultTopClient(url,
                                    ConfigurationManager.AppSettings["client_id"],
                                    ConfigurationManager.AppSettings["client_secret"]);
                                RefundGetRequest req = new RefundGetRequest();
                                req.Fields = "refund_fee";
                                req.RefundId = no.RefundId;
                                RefundGetResponse rsp = Iclient.Execute(req, tb_Access_Toen.access_token);
                                one.RefundFee = float.Parse(rsp.Refund.RefundFee);
                                var result = await client.For<TBOrderNo>()
                                    .Filter(s => s.SubNo == no.SubNo)
                                    .Filter(s => s.order.state != OrderState.Invalid && s.order.state != OrderState.SencondCancel)
                                    .FindEntriesAsync();
                                if (result != null && result.Count() > 0)
                                {
                                    int num = result.Count();
                                    num++;
                                    one.RefundFeeSplit = one.RefundFee / num;
                                    foreach (var re in result)
                                    {
                                        re.RefundFeeSplit = one.RefundFeeSplit;
                                        await client.For<TBOrderNo>().Key(re.TBOrderNoID).Set(re).UpdateEntryAsync();
                                    }
                                }
                                else
                                {
                                    one.RefundFeeSplit = float.Parse(rsp.Refund.RefundFee);
                                }
                            }
                        }
                        catch { }
                    }
                    await client.For<TBOrderNo>().Set(one).InsertEntryAsync();
                }
            }
            return JsonConvert.SerializeObject(new { ErrorCode = 200, ErrorMessage = "OK" });
        }
    }

}
