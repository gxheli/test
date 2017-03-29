using LanghuaNew.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace LanghuaNew.Service.Controllers
{
    public class TBOrdersExtendController : ApiController
    {
        private LanghuaContent db = new LanghuaContent();
        // GET: api/TBOrdersExtend
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/TBOrdersExtend/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/TBOrdersExtend
        public async Task<HttpResponseMessage> Post([FromBody]string value)
        {
            TBOrder tBOrder = JsonConvert.DeserializeObject<TBOrder>(value);
            if (tBOrder == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
                //return NotFound();
            }
            db.TBOrders.Add(tBOrder);
            await db.SaveChangesAsync();
            tBOrder.Orders.AddRange(db.Orders.Where(o => o.OrderNo == null || o.OrderNo == ""));
            foreach (Order o in tBOrder.Orders)
            {
                o.OrderNo = o.CreateTime.ToString("yyyyMMddHHmmss") + (o.OrderID % 10000).ToString("D4");
                db.Orders.Attach(o);
                db.Entry(o).Property(u => u.OrderNo).IsModified = true;
            }
            await db.SaveChangesAsync();
            //return Request.CreateResponse(HttpStatusCode.OK, tBOrder.TBOrderID.ToString());
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(tBOrder.TBOrderID.ToString(), System.Text.Encoding.UTF8, "text/plain")
            };
        }

        // PUT: api/TBOrdersExtend/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/TBOrdersExtend/5
        public void Delete(int id)
        {
        }
    }
}
