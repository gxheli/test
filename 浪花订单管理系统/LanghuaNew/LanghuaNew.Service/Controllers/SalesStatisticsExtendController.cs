using Entity;
using LanghuaNew.Data;
using LanghuaNew.Service.App_Code;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace LanghuaNew.Service.Controllers
{
    public class SalesStatisticsExtendController : ApiController
    {
        private LanghuaContent db = new LanghuaContent();
        // GET: api/SalesStatisticsExtend
        public IEnumerable<StatisticsOrderPriceModel> Get(string StartDate, string EndDate)
        {
            SqlParameter[] sqlparms = new SqlParameter[] {
                new SqlParameter("@StartDate", StartDate),
                new SqlParameter("@EndDate", EndDate),
            };
            var result = db.Database.SqlQuery<StatisticsOrderPriceModel>("EXEC dbo.StatisticsOrderPrice @StartDate=@StartDate,@EndDate=@EndDate", sqlparms).ToList();
            return result;
        }
        public IEnumerable<SalesStatisticModel> Post([FromBody]string value)
        {
            ShareSearchModel search = JsonConvert.DeserializeObject<ShareSearchModel>(value);
            int draw = 1;
            int start = 0;
            int length = 50;
            if (search.length > 0)
            {
                draw = search.draw;
                start = search.start;
                length = search.length;
            }
            if (search.SalesStatisticSearch != null)
            {
                DateTimeOffset BeginDate = DateTimeOffset.Parse(search.SalesStatisticSearch.BeginDate).Date;
                DateTimeOffset EndDate = DateTimeOffset.Parse(search.SalesStatisticSearch.EndDate).Date.AddDays(1);
                var salesStatistic = db.SalesStatistics.Include("serviceItem").Include("supplier").Where(s => s.SalesStatisticID == search.SalesStatisticSearch.SalesStatisticID).First();
                IQueryable<SalesStatisticModel> query;
                if (salesStatistic.serviceItem.ServiceTypeID == 4)
                {
                    query = from a in db.Orders
                            join b in db.ServiceItemHistories on a.OrderID equals b.OrderID
                            join c in db.Users on new { CreateUserID = a.CreateUserID } equals new { CreateUserID = c.UserID } into c_join
                            from c in c_join.DefaultIfEmpty()
                            where
                              a.state == OrderState.SencondConfirm &&
                              b.ServiceItemID == salesStatistic.serviceItem.ServiceItemID &&
                              b.SupplierID == salesStatistic.supplier.SupplierID &&
                              a.CreateTime >= BeginDate &&
                              a.CreateTime < EndDate
                            group new { c, b } by new
                            {
                                c.NickName
                            } into g
                            orderby
                              g.Sum(p => p.b.RoomNum * p.b.RightNum) descending
                            select new SalesStatisticModel
                            {
                                NickName = g.Key.NickName,
                                ServiceTypeID = salesStatistic.serviceItem.ServiceTypeID,
                                BeginDate = search.SalesStatisticSearch.BeginDate,
                                EndDate = search.SalesStatisticSearch.EndDate,
                                Num1 = g.Sum(p => p.b.RoomNum),
                                Num2 = g.Sum(p => p.b.RightNum),
                                Num3 = g.Sum(p => p.b.RoomNum * p.b.RightNum)
                            };
                }
                else
                {
                    query = from a in db.Orders
                            join b in db.ServiceItemHistories on a.OrderID equals b.OrderID
                            join c in db.Users on new { CreateUserID = a.CreateUserID } equals new { CreateUserID = c.UserID } into c_join
                            from c in c_join.DefaultIfEmpty()
                            where
                              a.state == OrderState.SencondConfirm &&
                              b.ServiceItemID == salesStatistic.serviceItem.ServiceItemID &&
                              b.SupplierID == salesStatistic.supplier.SupplierID &&
                              a.CreateTime >= BeginDate &&
                              a.CreateTime < EndDate
                            group new { c, b } by new
                            {
                                c.NickName
                            } into g
                            orderby
                              g.Sum(p => p.b.AdultNum) + g.Sum(p => p.b.ChildNum) descending
                            select new SalesStatisticModel
                            {
                                NickName = g.Key.NickName,
                                ServiceTypeID = salesStatistic.serviceItem.ServiceTypeID,
                                BeginDate = search.SalesStatisticSearch.BeginDate,
                                EndDate = search.SalesStatisticSearch.EndDate,
                                Num1 = g.Sum(p => p.b.AdultNum),
                                Num2 = g.Sum(p => p.b.ChildNum),
                                Num3 = g.Sum(p => p.b.AdultNum) + g.Sum(p => p.b.ChildNum)
                            };
                }
                return query;
            }
            return null;
        }
        public async Task<HttpResponseMessage> Put([FromBody]string value)
        {
            try
            {
                List<SalesStatistic> sales = JsonConvert.DeserializeObject<List<SalesStatistic>>(value);

                List<SalesStatistic> oldsales = db.SalesStatistics.ToList();
                var AddList = sales.Except(oldsales, p => new { p.SupplierID, p.ServiceItemID }).ToList();
                var ModifList = sales.Except(AddList, p => new { p.SupplierID, p.ServiceItemID }).ToList();
                var DeleteList = oldsales.Except(ModifList, p => new { p.SupplierID, p.ServiceItemID }).ToList();

                AddList.ForEach(s => db.SalesStatistics.Add(s));
                DeleteList.ForEach(s => db.SalesStatistics.Remove(s));

                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(ex.Message, System.Text.Encoding.UTF8, "text/plain")
                };
            }
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("OK", System.Text.Encoding.UTF8, "text/plain")
            };
        }
    }
}
