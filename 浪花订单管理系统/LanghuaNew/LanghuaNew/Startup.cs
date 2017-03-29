using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;
using Hangfire;
using Hangfire.Dashboard;
using Simple.OData.Client;
using System.Configuration;
using LanghuaNew.Data;
using Commond;
using System.Linq;
using Entity;
using WebGrease.Css.Extensions;
using Commond.Caching;
using Top.Api;
using Top.Api.Request;
using Top.Api.Response;
using System.Collections.Generic;
using Newtonsoft.Json;

[assembly: OwinStartup(typeof(LanghuaNew.Startup))]

namespace LanghuaNew
{
    public class Startup
    {
        public static string WeixinTemplateId = System.Configuration.ConfigurationManager.AppSettings["TravelWeixinTemplateID"];
        public static string ForCusPath = System.Configuration.ConfigurationManager.AppSettings["ForCusPath"];
        private ODataClient client = new ODataClient(ConfigurationManager.AppSettings["ServicePath"] + "odata/");
        public void Configuration(IAppBuilder app)
        {
            GlobalConfiguration.Configuration.UseSqlServerStorage("JobContent");
            app.UseHangfireDashboard();
            app.UseHangfireServer();
            RecurringJob.AddOrUpdate(() => WriteFie(), Cron.Daily(21, 0), TimeZoneInfo.Local);
            RecurringJob.AddOrUpdate(() => SendSupplierWeixin(), Cron.Hourly, TimeZoneInfo.Local);
            RecurringJob.AddOrUpdate(() => CacheAllOrderData(), Cron.Hourly, TimeZoneInfo.Local);
            RecurringJob.AddOrUpdate(() => StatisticsOrderPrice(), Cron.Daily(1, 0), TimeZoneInfo.Local);
            RecurringJob.AddOrUpdate(() => GetSubOrder(), Cron.Daily(0, 1), TimeZoneInfo.Local);
        }
        [AutomaticRetry(Attempts = 7)]
        public async Task WriteFie()
        {
            //获取明天出行订单
            var result = client.For<Order>().Expand("ServiceItemHistorys").Expand("Customers")
                .Filter("state eq LanghuaNew.Data.OrderState'SencondConfirm'")
                .Filter("ServiceItemHistorys/TravelDate ge " + DateTimeOffset.Now.AddDays(1).ToString("yyyy-MM-dd"))
                .Filter("ServiceItemHistorys/TravelDate lt " + DateTimeOffset.Now.AddDays(2).ToString("yyyy-MM-dd"));

            //var Command = result.GetCommandTextAsync();
            var orders = await result.FindEntriesAsync();

            if (orders != null)
            {
                foreach (var order in orders)
                {
                    if (order.Customers.OpenID != null && order.Customers.OpenID != "")
                    {
                        var message = client.For<WeixinMessage>()
                                .Filter("WeixinCountry/Suppliers/any(x1:x1/SupplierID eq " + order.ServiceItemHistorys.SupplierID + ")")
                                .Filter("StartTime le " + DateTime.Now.ToString("yyyy-MM-dd") + " and EndTime ge " + DateTime.Now.ToString("yyyy-MM-dd"))
                                .Filter("WeixinMessageState eq LanghuaNew.Data.EnableState'Enable'");
                        var xxx = message.GetCommandTextAsync();
                        var info = await message.FindEntryAsync();
                        string ReceiveManTime = order.ServiceItemHistorys.ReceiveManTime;
                        string infoMessage = "";
                        string infoURL = "";
                        string state = "User";
                        if (info != null)
                        {
                            infoMessage = info.Message;
                            infoURL = info.Url;
                        }
                        else
                        {
                            infoMessage = "点击查看确认单";
                            infoURL = ForCusPath + "Weixin/orderdetail";
                            state = order.OrderID.ToString();
                        }
                        if (!string.IsNullOrEmpty(ReceiveManTime) && (order.ServiceItemHistorys.ServiceTypeID == 2 || order.ServiceItemHistorys.ServiceTypeID == 3))
                        {
                            infoMessage = "接人时间：" + ReceiveManTime + "，请准时到酒店大堂等候司机" + (string.IsNullOrEmpty(infoMessage) ? "" : (" \r\n \r\n " + infoMessage));
                        }
                        else
                        {
                            infoMessage = " \r\n " + infoMessage;
                        }
                        try
                        {
                            var massge = WeiXinHelper.TravelSendMessage(order.Customers.OpenID, "您好，您有明天出发的行程。" + " \r\n ", order.ServiceItemHistorys.cnItemName, order.ServiceItemHistorys.TravelDate.ToString("yyyy-MM-dd"), infoMessage, infoURL, state);
                            await client.For<CustomerLog>().Set(new { Operate = "出行微信推送", CustomerID = order.CustomerID, OperID = "", OperName = order.OrderNo, OperTime = DateTimeOffset.Now, Remark = massge.ToString() }).InsertEntryAsync();
                        }
                        catch (Exception ex)
                        {
                            await client.For<CustomerLog>().Set(new { Operate = "出行微信推送失败", CustomerID = order.CustomerID, OperID = "", OperName = order.OrderNo, OperTime = DateTimeOffset.Now, Remark = ex.Message }).InsertEntryAsync();
                        }

                        //OrderHistory orderHistoryWeixin = new OrderHistory();
                        //orderHistoryWeixin.OperUserID = "";
                        //orderHistoryWeixin.OperUserNickName = "微信定时提醒";
                        //orderHistoryWeixin.OperTime = DateTime.Now;
                        //orderHistoryWeixin.State = OrderState.SencondConfirm;
                        //orderHistoryWeixin.Remark = "(微信推送)" + err;
                        //order.OrderHistorys = new List<OrderHistory>();
                        //order.OrderHistorys.Add(orderHistoryWeixin);
                        //await HttpHelper.PutAction("OrdersExtend", JsonConvert.SerializeObject(order));
                        //FileStream steam = new FileStream("E:\\log.txt", FileMode.Append);
                        //StreamWriter sw = new StreamWriter(steam);
                        ////开始写入
                        //sw.WriteLine("提醒明天出行旅客,订单ID"+ order.OrderID+",时间" + DateTime.Now);
                        ////清空缓冲区
                        //sw.Flush();
                        ////关闭流
                        //sw.Close();
                        //steam.Close();
                    }
                }
            }
        }

        [AutomaticRetry(Attempts = 3)]
        public async Task SendSupplierWeixin()
        {
            //获取所有线上供应商
            var result = client.For<Supplier>()
                .Expand("SupplierUsers")
                .Filter("SupplierEnableState eq LanghuaNew.Data.EnableState'Enable'")
                .Filter("EnableOnline eq true")
                .Filter("SupplierUsers/any(x1:x1/SummaryMessage eq true and x1/OpenID ne null and x1/OpenID ne '')");
            var Suppliers = await result.FindEntriesAsync();

            foreach (var supplier in Suppliers)
            {
                //先查询该供应商是否需要发送消息
                bool send = false;//true为需要发送消息
                foreach (var user in supplier.SupplierUsers)
                {
                    if (!string.IsNullOrEmpty(user.OpenID) && user.SummaryMessage)
                    {
                        if (user.Disturb)
                        {
                            try
                            {
                                string BeginTime = user.BeginTime;
                                string EndTime = user.EndTime;
                                //免打扰开始时间大于结束时间,按跨天算,当前时间小于开始时间并且大于结束时间则发送消息
                                if (int.Parse(BeginTime.Split(':')[0]) > int.Parse(EndTime.Split(':')[0])
                                    || (int.Parse(BeginTime.Split(':')[0]) == int.Parse(EndTime.Split(':')[0]) && int.Parse(BeginTime.Split(':')[1]) > int.Parse(EndTime.Split(':')[1])))
                                {
                                    if ((DateTimeOffset.Now.Hour < int.Parse(BeginTime.Split(':')[0])
                                        || (DateTimeOffset.Now.Hour == int.Parse(BeginTime.Split(':')[0]) && DateTimeOffset.Now.Minute <= int.Parse(BeginTime.Split(':')[1])))
                                       && (DateTimeOffset.Now.Hour > int.Parse(EndTime.Split(':')[0])
                                        || (DateTimeOffset.Now.Hour == int.Parse(EndTime.Split(':')[0]) && DateTimeOffset.Now.Minute >= int.Parse(EndTime.Split(':')[1]))))
                                    {
                                        send = true;//发送消息
                                        break;
                                    }
                                }
                                else//免打扰开始时间小于结束时间,按当天算,当前时间小于开始时间或者大于结束时间则发送消息
                                {
                                    if (DateTimeOffset.Now.Hour < int.Parse(BeginTime.Split(':')[0])
                                        || (DateTimeOffset.Now.Hour == int.Parse(BeginTime.Split(':')[0]) && DateTimeOffset.Now.Minute <= int.Parse(BeginTime.Split(':')[1]))
                                        || DateTimeOffset.Now.Hour > int.Parse(EndTime.Split(':')[0])
                                        || (DateTimeOffset.Now.Hour == int.Parse(EndTime.Split(':')[0]) && DateTimeOffset.Now.Minute >= int.Parse(EndTime.Split(':')[1])))
                                    {
                                        send = true;//发送消息
                                        break;
                                    }
                                }

                            }
                            catch { }
                        }
                        else
                        {
                            send = true;//发送消息
                            break;
                        }
                    }
                }
                if (send)
                {
                    var orderSend = await client.For<Order>()
                        .Filter("ServiceItemHistorys/SupplierID eq " + supplier.SupplierID)
                        .Filter("state eq LanghuaNew.Data.OrderState'Send'")
                        .Count().FindScalarAsync<int>();
                    var orderRequestChange = await client.For<Order>()
                        .Filter("ServiceItemHistorys/SupplierID eq " + supplier.SupplierID)
                        .Filter("state eq LanghuaNew.Data.OrderState'RequestChange'")
                        .Count().FindScalarAsync<int>();
                    var orderRequestCancel = await client.For<Order>()
                        .Filter("ServiceItemHistorys/SupplierID eq " + supplier.SupplierID)
                        .Filter("state eq LanghuaNew.Data.OrderState'RequestCancel'")
                        .Count().FindScalarAsync<int>();
                    var count = orderSend + orderRequestChange + orderRequestCancel;
                    if (count > 0)
                    {
                        var title = "您好，您有" + count + "条订单等待处理。";
                        var status = "新订单" + orderSend + "条," + "请求变更" + orderRequestChange + "条," + "请求取消" + orderRequestCancel + "条";
                        var content = "请及时登录系统后台进行处理，谢谢！";
                        foreach (var user in supplier.SupplierUsers)
                        {
                            if (!string.IsNullOrEmpty(user.OpenID) && user.SummaryMessage)
                            {
                                bool usersend = false;
                                if (user.Disturb)
                                {
                                    try
                                    {
                                        string BeginTime = user.BeginTime;
                                        string EndTime = user.EndTime;
                                        //免打扰开始时间大于结束时间,按跨天算,当前时间小于开始时间并且大于结束时间则发送消息
                                        if (int.Parse(BeginTime.Split(':')[0]) > int.Parse(EndTime.Split(':')[0])
                                            || (int.Parse(BeginTime.Split(':')[0]) == int.Parse(EndTime.Split(':')[0]) && int.Parse(BeginTime.Split(':')[1]) > int.Parse(EndTime.Split(':')[1])))
                                        {
                                            if ((DateTimeOffset.Now.Hour < int.Parse(BeginTime.Split(':')[0])
                                                || (DateTimeOffset.Now.Hour == int.Parse(BeginTime.Split(':')[0]) && DateTimeOffset.Now.Minute <= int.Parse(BeginTime.Split(':')[1])))
                                               && (DateTimeOffset.Now.Hour > int.Parse(EndTime.Split(':')[0])
                                                || (DateTimeOffset.Now.Hour == int.Parse(EndTime.Split(':')[0]) && DateTimeOffset.Now.Minute >= int.Parse(EndTime.Split(':')[1]))))
                                            {
                                                usersend = true;//发送消息
                                            }
                                        }
                                        else//免打扰开始时间小于结束时间,按当天算,当前时间小于开始时间或者大于结束时间则发送消息
                                        {
                                            if (DateTimeOffset.Now.Hour < int.Parse(BeginTime.Split(':')[0])
                                                || (DateTimeOffset.Now.Hour == int.Parse(BeginTime.Split(':')[0]) && DateTimeOffset.Now.Minute <= int.Parse(BeginTime.Split(':')[1]))
                                                || DateTimeOffset.Now.Hour > int.Parse(EndTime.Split(':')[0])
                                                || (DateTimeOffset.Now.Hour == int.Parse(EndTime.Split(':')[0]) && DateTimeOffset.Now.Minute >= int.Parse(EndTime.Split(':')[1])))
                                            {
                                                usersend = true;//发送消息
                                            }
                                        }
                                    }
                                    catch { }
                                }
                                else
                                {
                                    usersend = true;//发送消息
                                }
                                if (usersend)
                                {
                                    try
                                    {
                                        var supplierMessage = WeiXinHelper.SupplierSendSummaryMessage(user.OpenID, title, status, content, "");
                                        await client.For<SystemLog>().Set(new { Operate = "供应商汇总微信推送", OperateTime = DateTime.Now, UserID = user.SupplierUserID, UserName = user.SupplierNickName, Remark = "供应商：" + supplier.SupplierNo + "-" + supplier.SupplierName + " Message：" + supplierMessage }).InsertEntryAsync();
                                    }
                                    catch (Exception ex)
                                    {
                                        await client.For<SystemLog>().Set(new { Operate = "供应商汇总推送失败", OperateTime = DateTime.Now, UserID = user.SupplierUserID, UserName = user.SupplierNickName, Remark = "供应商：" + supplier.SupplierNo + "-" + supplier.SupplierName + " Message：" + ex.Message }).InsertEntryAsync();
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        [AutomaticRetry(Attempts = 3)]
        public async Task CacheAllOrderData()
        {
            ICacheManager _cacheManager = new MemoryCacheManager();
            string cacheKey = ConstantConfig.HOME_ALLORDERDATA;

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
            _cacheManager.Remove(cacheKey);
            _cacheManager.Set(cacheKey, model, 70);
        }

        /// <summary>
        /// 拉取退款信息
        /// </summary>
        [AutomaticRetry(Attempts = 3)]
        public async Task StatisticsOrderPrice()
        {
            TB_Access_Token tb_Access_Toen = await client.For<TB_Access_Token>().FindEntryAsync();
            if (tb_Access_Toen != null)
            {
                string url = ConfigurationManager.AppSettings["bizInterface_Trades"];
                ITopClient Iclient = new DefaultTopClient(url,
                    ConfigurationManager.AppSettings["client_id"],
                    ConfigurationManager.AppSettings["client_secret"]);

                int i = 1;
                while (true)
                {
                    RefundsReceiveGetRequest req = new RefundsReceiveGetRequest();
                    req.Fields = "refund_id,refund_fee,oid";
                    req.Status = "SUCCESS";
                    req.StartModified = DateTime.Parse(DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd") + " 00:00:00");
                    req.EndModified = DateTime.Parse(DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd") + " 23:59:59");
                    req.PageNo = i;
                    req.PageSize = 100L;
                    req.UseHasNext = true;
                    RefundsReceiveGetResponse rsp = Iclient.Execute(req, tb_Access_Toen.access_token);
                    foreach (var item in rsp.Refunds)
                    {
                        var result = await client.For<TBOrderNo>().Expand("order").Filter("SubNo eq '" + item.Oid + "'").FindEntriesAsync();
                        if (result != null)
                            foreach (var one in result)
                            {
                                try
                                {
                                    float RefundFeeSplit = float.Parse(item.RefundFee);
                                    if (result.Where(s => s.order.state != OrderState.Invalid && s.order.state != OrderState.SencondCancel) != null)
                                    {
                                        var num = result.Where(s => s.order.state != OrderState.Invalid && s.order.state != OrderState.SencondCancel).Count();
                                        RefundFeeSplit = float.Parse(item.RefundFee) / num;
                                    }
                                    one.RefundFee = float.Parse(item.RefundFee);
                                    one.RefundFeeSplit = RefundFeeSplit;
                                    await client.For<TBOrderNo>().Key(one.TBOrderNoID).Set(one).UpdateEntryAsync();
                                }
                                catch { }
                            }
                    }
                    if (rsp.HasNext)
                        i++;
                    else
                        break;
                }
            }
        }
        /// <summary>
        /// 拉取子订单信息
        /// </summary>
        [AutomaticRetry(Attempts = 3)]
        public async Task GetSubOrder()
        {
            var StartDate = DateTimeOffset.Parse(DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd"));
            //var StartDate = DateTimeOffset.Parse("2017-02-01");
            //var EndDate = DateTimeOffset.Parse("2017-03-01");
            var orders = await client.For<Order>()
                .Filter(o => o.TBOrderNos.All(s => s.TBOrderNoID == 0))
                .Filter(o => o.CreateTime >= StartDate)
                //.Filter(o => o.CreateTime <= EndDate)
                .Filter(o => o.TBOrders.OrderSourseID == 2)
                .Select(o => new { o.OrderID, o.TBNum })
                .FindEntriesAsync();

            TB_Access_Token tb_Access_Toen = await client.For<TB_Access_Token>().FindEntryAsync();
            if (tb_Access_Toen != null)
            {
                string url = ConfigurationManager.AppSettings["bizInterface_Trades"];
                ITopClient Iclient = new DefaultTopClient(url,
                    ConfigurationManager.AppSettings["client_id"],
                    ConfigurationManager.AppSettings["client_secret"]);

                foreach (var item in orders)
                {
                    try
                    {
                        //遍历Tid获取单笔交易的详细信息
                        AlitripTravelTradeFullinfoGetRequest reqFull = new AlitripTravelTradeFullinfoGetRequest();
                        reqFull.Tid = long.Parse(item.TBNum);
                        AlitripTravelTradeFullinfoGetResponse repFull = Iclient.Execute(reqFull, tb_Access_Toen.access_token);
                        System.Threading.Thread.Sleep(2000);
                        foreach (var one in repFull.TradeFullinfo.Orders)
                        {
                            try
                            {
                                float RefundFee = 0;
                                if (one.RefundId > 0)
                                {
                                    RefundGetRequest req = new RefundGetRequest();
                                    req.Fields = "refund_fee";
                                    req.RefundId = one.RefundId;
                                    RefundGetResponse rsp = Iclient.Execute(req, tb_Access_Toen.access_token);
                                    RefundFee = float.Parse(rsp.Refund.RefundFee);
                                    RefundFee = float.Parse(rsp.Refund.RefundFee);
                                }
                                await client.For<TBOrderNo>().Set(new TBOrderNo
                                {
                                    OrderID = item.OrderID,
                                    No = item.TBNum,
                                    SubNo = one.Oid.ToString(),
                                    Payment = float.Parse(one.Payment),
                                    PaymentSplit = float.Parse(one.Payment),
                                    RefundFee = RefundFee,
                                    RefundFeeSplit = RefundFee
                                }).InsertEntryAsync();
                            }
                            catch { }
                        }
                    }
                    catch { }
                }
            }
        }
    }
}
