using Entity;
using LanghuaNew.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace LanghuaNew.Service.Controllers
{
    public class ExportOrdersExtendController : ApiController
    {
        private LanghuaContent db = new LanghuaContent();
        // GET: api/ExportOrdersExtend
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/ExportOrdersExtend/5
        public string Get(int id)
        {
            return "value";
        }
        // POST: api/ExportOrdersExtend
        public IEnumerable<CheckDeliveryModel> Post()
        {
            string value = Request.Content.ReadAsStringAsync().Result;
            List<ExportOrder> list = JsonConvert.DeserializeObject<List<ExportOrder>>(value);

            db.ExportOrders.AddRange(list);
            db.SaveChanges();

            string guid = list[0].Guid;
            DateTimeOffset mindate = DateTimeOffset.Parse("1901-01-01");

            string sql =
            "  SELECT p.ExportTBID as TBID,p.ExportOrderNo as OrderNo,x.state FROM dbo.ExportOrders p " +
            "  JOIN( " +
            "  SELECT ExportTBID, CASE ISNULL(MIN(b.TBNum), '') WHEN '' THEN '无订单' ELSE '漏发货' END AS state FROM dbo.ExportOrders a " +
            "  LEFT JOIN dbo.TBOrderStates b ON a.ExportOrderNo = b.TBNum " +
            "  JOIN dbo.Customers c ON a.ExportTBID = c.CustomerTBCode " +
            "  JOIN dbo.Orders d ON c.CustomerID = d.CustomerID " +
            "  JOIN dbo.ServiceItemHistories e ON d.OrderID = e.OrderID " +
            "  WHERE a.Guid = '"+ guid + "' AND(b.TBNum is NULL OR b.IsSend = 1) " +
            "  AND e.TravelDate < CONVERT(VARCHAR(10), dateadd(dd,-1,GETDATE()), 120) AND e.TravelDate > '1901-01-01' AND d.state != 13 AND c.IsNeedCustomerService = 0 " +
            "  GROUP BY ExportTBID " +
            "  UNION " +
            "  SELECT DISTINCT ExportTBID, '静默' AS state FROM dbo.ExportOrders a " +
            "  LEFT JOIN dbo.Customers b ON b.CustomerTBCode = ExportTBID " +
            "  WHERE Guid = '" + guid + "' AND b.CustomerID IS NULL " +
            "  ) x ON p.ExportTBID = x.ExportTBID AND Guid = '" + guid + "'";
            var result = db.Database.SqlQuery<CheckDeliveryModel>(sql);
            
            var aa = result.OrderBy(t => t.TBID).GroupBy(t => new { t.TBID, t.state }).ToList();
            List<CheckDeliveryModel> models = new List<CheckDeliveryModel>();
            foreach (var item in aa)
            {
                CheckDeliveryModel model = new CheckDeliveryModel();
                model.TBID = item.Key.TBID;
                model.state = item.Key.state;
                var cc = "";
                item.ToList().ForEach(t => cc += t.OrderNo + " ");
                model.OrderNo = cc;
                models.Add(model);
            }
            var delList = db.ExportOrders.Where(t => t.Guid == guid);
            db.ExportOrders.RemoveRange(delList);
            db.SaveChanges();

            return models;
        }

        //// POST: api/ExportOrdersExtend
        //public IEnumerable<CheckDeliveryModel> Post()
        //{
        //    string value = Request.Content.ReadAsStringAsync().Result;
        //    List<ExportOrder> list = JsonConvert.DeserializeObject<List<ExportOrder>>(value);

        //    db.ExportOrders.AddRange(list);
        //    db.SaveChanges();

        //    string guid = list[0].Guid;
        //    var result = from a in db.ExportOrders
        //                 join d in (
        //                     (from b in db.TBOrderStates
        //                      select new
        //                      {
        //                          b.IsSend,
        //                          b.TBNum,
        //                          b.SendCustomer.CustomerTBCode
        //                      }))
        //                       on new { a.ExportOrderNo, a.ExportTBID }
        //                   equals new { ExportOrderNo = d.TBNum, ExportTBID = d.CustomerTBCode } into d_join
        //                 from d in d_join.DefaultIfEmpty()
        //                 where a.Guid == guid && (d.TBNum == null || d.IsSend)
        //                 select new
        //                 {
        //                     a.ExportTBID,
        //                     IsSend = (bool?)d.IsSend,
        //                     a.ExportOrderNo
        //                 };
        //    var aa = result.GroupBy(t => new { t.ExportTBID, t.IsSend }).ToList();
        //    List<CheckDeliveryModel> models = new List<CheckDeliveryModel>();
        //    foreach (var item in aa)
        //    {
        //        CheckDeliveryModel model = new CheckDeliveryModel();
        //        model.TBID = item.Key.ExportTBID;
        //        //var order = db.Orders
        //        //    .Where(s => s.TBOrders.TBID == model.TBID && s.ServiceItemHistorys.TravelDate > DateTimeOffset.Now.AddMonths(-3))
        //        //    .OrderBy(s => s.ServiceItemHistorys.TravelDate)
        //        //    .Select(s => new { s.ServiceItemHistorys.TravelDate, s.Customers.IsNeedCustomerService }).FirstOrDefault();
        //        //model.minDate = order != null ? order.TravelDate.ToString("yyyy-MM-dd") :"" ;
        //        //model.IsNeedCustomerService = order != null && order.IsNeedCustomerService ? "要售后" : "";
        //        model.state = item.Key.IsSend == null ? "无订单" : (item.Key.IsSend == true ? "漏发货" : "");
        //        var cc = "";
        //        item.ToList().ForEach(t => cc += t.ExportOrderNo + " ");
        //        model.OrderNo = cc;
        //        models.Add(model);
        //    }
        //    var delList = db.ExportOrders.Where(t => t.Guid == guid);
        //    db.ExportOrders.RemoveRange(delList);
        //    db.SaveChanges();

        //    return models;
        //}

        //// POST: api/ExportOrdersExtend
        //public IEnumerable<CheckDeliveryModel> Post()
        //{
        //    string value = Request.Content.ReadAsStringAsync().Result;
        //    List<ExportOrder> list = JsonConvert.DeserializeObject<List<ExportOrder>>(value);

        //    db.ExportOrders.AddRange(list);
        //    db.SaveChanges();

        //    string guid = list[0].Guid;
        //    var result = from a in db.ExportOrders
        //                 join d in (
        //                     (from b in db.TBOrderStates
        //                      select new
        //                      {
        //                          b.IsSend,
        //                          b.TBNum,
        //                          b.SendCustomer.CustomerTBCode
        //                      }))
        //                       on new { a.ExportOrderNo, a.ExportTBID }
        //                   equals new { ExportOrderNo = d.TBNum, ExportTBID = d.CustomerTBCode } into d_join
        //                 from d in d_join.DefaultIfEmpty()
        //                 where a.Guid == guid && (d.TBNum == null || d.IsSend)
        //                 select new
        //                 {
        //                     a.ExportTBID,
        //                     IsSend = (bool?)d.IsSend,
        //                     a.ExportOrderNo
        //                 };
        //    var aa = result.GroupBy(t => new { t.ExportTBID, t.IsSend }).ToList();
        //    List<CheckDeliveryModel> models = new List<CheckDeliveryModel>();
        //    foreach (var item in aa)
        //    {
        //        string state = "";
        //        if (item.Key.IsSend == true)
        //        {
        //            state = "漏发货";
        //        }
        //        else
        //        {
        //            var order = db.Orders
        //                .Where(s => s.TBOrders.TBID == item.Key.ExportTBID)
        //                .Select(s => new { s.OrderID }).FirstOrDefault();
        //            if (order == null)
        //            {
        //                state = "静默";
        //            }
        //            else
        //            {
        //                var next = DateTimeOffset.Now.Date.AddDays(1);
        //                var order2 = db.Orders
        //                    .Where(s => s.TBOrders.TBID == item.Key.ExportTBID)
        //                    .Where(s => s.ServiceItemHistorys.TravelDate < next)
        //                    .Where(s => !s.Customers.IsNeedCustomerService)
        //                    .Select(s => new { s.OrderID }).FirstOrDefault();
        //                if (order2 != null)
        //                {
        //                    state = "无订单";
        //                }
        //            }
        //        }
        //        if (state != "")
        //        {
        //            CheckDeliveryModel model = new CheckDeliveryModel();
        //            model.TBID = item.Key.ExportTBID;
        //            model.state = state;
        //            var cc = "";
        //            item.ToList().ForEach(t => cc += t.ExportOrderNo + " ");
        //            model.OrderNo = cc;
        //            models.Add(model);
        //        }
        //    }
        //    var delList = db.ExportOrders.Where(t => t.Guid == guid);
        //    db.ExportOrders.RemoveRange(delList);
        //    db.SaveChanges();

        //    return models;
        //}
        // PUT: api/ExportOrdersExtend/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/ExportOrdersExtend/5
        public void Delete(int id)
        {
        }
    }
}
