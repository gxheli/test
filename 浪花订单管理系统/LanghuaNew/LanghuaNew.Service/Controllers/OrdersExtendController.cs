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

namespace LanghuaNew.Service.Controllers
{
    public class OrdersExtendController : ApiController
    {
        private LanghuaContent db = new LanghuaContent();
        // GET: api/OrdersExtend
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/OrdersExtend/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/OrdersExtend
        public async Task<HttpResponseMessage> Post([FromBody]string value)
        {
            Order order = JsonConvert.DeserializeObject<Order>(value);
            if (order == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
                //return NotFound();
            }
            db.Orders.Add(order);
            await db.SaveChangesAsync();

            order.OrderNo = order.CreateTime.ToString("yyyyMMddHHmmss") + (order.OrderID % 10000).ToString("D4");
            db.Orders.Attach(order);
            db.Entry(order).Property(u => u.OrderNo).IsModified = true;
            await db.SaveChangesAsync();
            
            //return Request.CreateResponse(HttpStatusCode.OK, tBOrder.TBOrderID.ToString());
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(order.OrderID.ToString(), System.Text.Encoding.UTF8, "text/plain")
            };
        }

        // PUT: api/OrdersExtend/5
        public async Task<HttpResponseMessage> Put([FromBody]string value)
        {
            string exMessage = "OK";
            //try
            //{
            //    Order order = JsonConvert.DeserializeObject<Order>(value);
            //    order.ServiceItemHistorys = null;
            //    order.Customers = null;
            //    if (order == null)
            //    {
            //        return Request.CreateResponse(HttpStatusCode.NotFound);
            //    }
            //    OrderHistory orderhistory = new OrderHistory();
            //    if (order.OrderHistorys != null)
            //    {
            //        orderhistory = order.OrderHistorys[0];
            //    }
            //    order.OrderHistorys.Clear();
            //    db.Orders.Attach(order);
            //    var item = db.Entry(order);
            //    item.Collection(i => i.OrderHistorys).Load();
            //    order.OrderHistorys.Add(orderhistory);

            //    item.State = EntityState.Modified;
            //    await db.SaveChangesAsync();
            //}
            //catch (Exception ex)
            //{
            //    exMessage = ex.Message;
            //}
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(exMessage, System.Text.Encoding.UTF8, "text/plain")
            };
        }

        // DELETE: api/OrdersExtend/5
        public void Delete(int id)
        {
        }
    }
}
