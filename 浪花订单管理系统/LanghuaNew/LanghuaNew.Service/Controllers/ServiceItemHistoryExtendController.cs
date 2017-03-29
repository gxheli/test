using LanghuaNew.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using LanghuaNew.Service.App_Code;
using AutoMapper;
using System.Data.SqlClient;
using Entity;

namespace LanghuaNew.Service.Controllers
{
    public class ServiceItemHistoryExtendController : ApiController
    {
        private LanghuaContent db = new LanghuaContent();
        // GET: api/ServiceItemHistoryExtend
        public IEnumerable<StatisticsSales> Get(string Name,string StartDate, string EndDate)
        {
            SqlParameter[] sqlparms = new SqlParameter[] {
                new SqlParameter("@Name", Name),
                new SqlParameter("@StartDate", StartDate),
                new SqlParameter("@EndDate", EndDate),
            };
            var result = db.Database.SqlQuery<StatisticsSales>("EXEC dbo.StatisticsSales @Name=@Name,@StartDate=@StartDate,@EndDate=@EndDate", sqlparms).ToList();
            return result;
        }

        // GET: api/ServiceItemHistoryExtend/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/ServiceItemHistoryExtend
        public async Task<IHttpActionResult> Post([FromBody]string value)
        {
            //修改订单和额外服务
            try
            {
                ServiceItemHistory itemHistory = JsonConvert.DeserializeObject<ServiceItemHistory>(value);
                if (itemHistory == null)
                {
                    return NotFound();
                }
                if (itemHistory.ExtraServiceHistorys == null)
                {
                    itemHistory.ExtraServiceHistorys = new List<ExtraServiceHistory>();
                }
                List<ExtraServiceHistory> NewList = itemHistory.ExtraServiceHistorys.ToList();
                db.ExtraServiceHistories.AddRange(NewList);
                itemHistory.ExtraServiceHistorys.Clear();
                db.ServiceItemHistories.Attach(itemHistory);
                var item = db.Entry(itemHistory);
                item.Collection(i => i.ExtraServiceHistorys).Load();
                List<ExtraServiceHistory> OldList = itemHistory.ExtraServiceHistorys;
                db.ExtraServiceHistories.RemoveRange(OldList);
                itemHistory.ExtraServiceHistorys = new List<ExtraServiceHistory>();
                NewList.ForEach(i => itemHistory.ExtraServiceHistorys.Add(i));
                item.State = EntityState.Modified;
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                log4net.ILog log = log4net.LogManager.GetLogger(typeof(Exception));
                log.Info(ex);
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // PUT: api/ServiceItemHistoryExtend/5
        public async Task<IHttpActionResult> Put([FromBody]string value)
        {
            //修改订单和同行旅客
            try
            {
                ServiceItemHistory itemHistory = JsonConvert.DeserializeObject<ServiceItemHistory>(value);
                if (itemHistory == null)
                {
                    return NotFound();
                }
                if (itemHistory.travellers == null)
                {
                    itemHistory.travellers = new List<OrderTraveller>();
                }
                var id = itemHistory.travellers.Select(t => t.TravellerID);
                List<Traveller> List = db.Travellers.Where(t => id.Contains(t.TravellerID)).ToList();

                
                itemHistory.travellers.Clear();
                db.ServiceItemHistories.Attach(itemHistory);
                var item = db.Entry(itemHistory);
                item.Collection(i => i.travellers).Load();
                List<OrderTraveller> OldTrList = itemHistory.travellers;
                db.OrderTravellers.RemoveRange(OldTrList);
                itemHistory.travellers = new List<OrderTraveller>();                
                item.State = EntityState.Modified;

                foreach (var list in List)
                {
                    OrderTraveller nn = new OrderTraveller();
                    nn.OrderID = itemHistory.OrderID;
                    nn.Birthday = list.Birthday;
                    nn.CreateTime = list.CreateTime;
                    nn.CustomerID = list.CustomerID;
                    nn.PassportNo = list.PassportNo;
                    nn.TravellerEnname = list.TravellerEnname;
                    nn.TravellerID = list.TravellerID;
                    nn.TravellerName = list.TravellerName;
                    nn.TravellerSex = list.TravellerSex;
                    nn.TravellerDetail = new OrderTravellerDetail();
                    nn.TravellerDetail.ClothesSize = list.TravellerDetail.ClothesSize;
                    nn.TravellerDetail.GlassesNum = list.TravellerDetail.GlassesNum;
                    nn.TravellerDetail.Height = list.TravellerDetail.Height;
                    nn.TravellerDetail.ShoesSize = list.TravellerDetail.ShoesSize;
                    nn.TravellerDetail.Weight = list.TravellerDetail.Weight;
                    db.OrderTravellers.Add(nn);
                }
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                log4net.ILog log = log4net.LogManager.GetLogger(typeof(Exception));
                log.Info(ex);
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // DELETE: api/ServiceItemHistoryExtend/5
        public void Delete(int id)
        {
        }
    }
}
