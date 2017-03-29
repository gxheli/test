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

namespace LanghuaForSup.Controllers
{
    public class HomeController : Controller
    {
        private ODataClient client = new ODataClient(ConfigurationManager.AppSettings["ServicePath"] + "odata/");
        public ActionResult Index()
        {
            return View();
        }
        //工作台数据
        public async Task<string> GetOrderData()
        {
            string SupplierUserName = User.Identity.Name;
            SupplierUser user = await client.For<SupplierUser>()
                .Expand(s => s.OneSupplier)
                .Filter(s => s.SupplierUserName == SupplierUserName).FindEntryAsync();

            //昨天、今天、明天、上月、本月、下月
            DateTimeOffset Pre = new DateTimeOffset(DateTime.Now.AddDays(-1).Date);
            DateTimeOffset Now = new DateTimeOffset(DateTime.Now.Date);
            DateTimeOffset Next = new DateTimeOffset(DateTime.Now.AddDays(1).Date);
            DateTimeOffset PreMonth = DateTimeOffset.Parse(DateTime.Now.ToString("yyyy-MM-01")).AddMonths(-1);
            DateTimeOffset ThisMonth = DateTimeOffset.Parse(DateTime.Now.ToString("yyyy-MM-01"));
            DateTimeOffset NextMonth = DateTimeOffset.Parse(DateTime.Now.ToString("yyyy-MM-01")).AddMonths(1);

            int NewOrderCount = -1;
            int ReceiveOrderCount = -1;
            int CancelOrderCount = -1;
            int ChangeOrderCount = -1;
            int TodayOrderCount = -1;
            int YesterdayOrderCount = -1;
            int ThisMonthOrderCount = -1;
            int PreMonthOrderCount = -1;
            //新订单
            NewOrderCount = await client.For<Order>()
                .Filter(t => t.ServiceItemHistorys.SupplierID == user.SupplierID)
                .Filter(t => t.state == OrderState.Send)
                .Count().FindScalarAsync<int>();
            //已接单
            ReceiveOrderCount = await client.For<Order>()
                .Filter(t => t.ServiceItemHistorys.SupplierID == user.SupplierID)
                .Filter(t => t.state == OrderState.SendReceive || t.state == OrderState.ChangeReceive || t.state == OrderState.CancelReceive)
                .Count().FindScalarAsync<int>();
            //请求取消
            CancelOrderCount = await client.For<Order>()
                .Filter(t => t.ServiceItemHistorys.SupplierID == user.SupplierID)
                .Filter(t => t.state == OrderState.RequestCancel)
                .Count().FindScalarAsync<int>();
            //请求变更
            ChangeOrderCount = await client.For<Order>()
                .Filter(t => t.ServiceItemHistorys.SupplierID == user.SupplierID)
                .Filter(t => t.state == OrderState.RequestChange)
                .Count().FindScalarAsync<int>();

            //供应商能看到的全部订单的状态
            string status = "3,4,5,6,7,8,9,10,11,12,14,15";
            //状态，以逗号分开
            string[] state = status.Trim().Split(',');
            string statefilter = "";
            foreach (var item in state)
            {
                if (statefilter != "")
                {
                    statefilter += " or ";
                }
                statefilter += "state eq LanghuaNew.Data.OrderState'" + item + "'";
            }
            string filter = "ServiceItemHistorys/SupplierID eq " + user.SupplierID;

            //今日订单数
            TodayOrderCount = await client.For<Order>()
                .Filter(statefilter)
                .Filter(filter)
                .Filter("CreateTime ge " + Now.ToString("yyyy-MM-dd"))
                .Filter("CreateTime lt " + Next.ToString("yyyy-MM-dd"))
                .Count().FindScalarAsync<int>();
            //昨日订单数
            YesterdayOrderCount = await client.For<Order>()
                .Filter(statefilter)
                .Filter(filter)
                .Filter("CreateTime ge " + Pre.ToString("yyyy-MM-dd"))
                .Filter("CreateTime lt " + Now.ToString("yyyy-MM-dd"))
                .Count().FindScalarAsync<int>();
            //本月订单数
            ThisMonthOrderCount = await client.For<Order>()
                .Filter(statefilter)
                .Filter(filter)
                .Filter("CreateTime ge " + ThisMonth.ToString("yyyy-MM-dd"))
                .Filter("CreateTime lt " + NextMonth.ToString("yyyy-MM-dd"))
                .Count().FindScalarAsync<int>();
            //上月订单数
            PreMonthOrderCount = await client.For<Order>()
                .Filter(statefilter)
                .Filter(filter)
                .Filter("CreateTime ge " + PreMonth.ToString("yyyy-MM-dd"))
                .Filter("CreateTime lt " + ThisMonth.ToString("yyyy-MM-dd"))
                .Count().FindScalarAsync<int>();

            var data = new
            {
                NewOrderCount,
                ReceiveOrderCount,
                CancelOrderCount,
                ChangeOrderCount,
                TodayOrderCount,
                YesterdayOrderCount,
                ThisMonthOrderCount,
                PreMonthOrderCount,
            };
            return JsonConvert.SerializeObject(new { ErrorCode = 200, ErrorMessage = "OK", data = data });
        }
    }
}