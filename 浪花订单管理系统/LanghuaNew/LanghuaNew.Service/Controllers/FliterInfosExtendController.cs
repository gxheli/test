using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using LanghuaNew.Data;
using Commond.Splider;
using Entity;
using Newtonsoft.Json;

namespace LanghuaNew.Service.Controllers
{
    public class FliterInfosExtendController : ApiController
    {
        private LanghuaContent db = new LanghuaContent();

        // GET: api/FliterInfosExtend
        public IQueryable<FliterInfo> GetFliterInfos()
        {
            return db.FliterInfos;
        }
        
        public List<FliterInfo> GetFliters(string fliterDeparture, string fliterArrival)
        {
            return db.FliterInfos.Where(d => d.FilterDeparture == fliterDeparture && d.FilterArrival == fliterArrival).ToList();
        }

        // GET: api/FliterInfosExtend/5
        [ResponseType(typeof(FliterInfo))]
        public async Task<IHttpActionResult> GetFliterInfo(int id)
        {
            FliterInfo fliterInfo = await db.FliterInfos.FindAsync(id);
            if (fliterInfo == null)
            {
                return NotFound();
            }

            return Ok(fliterInfo);
        }

        // PUT: api/FliterInfosExtend/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutFliterInfo(int id, FliterInfo fliterInfo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != fliterInfo.ID)
            {
                return BadRequest();
            }

            db.Entry(fliterInfo).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FliterInfoExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/FliterInfosExtend
        //[ResponseType(typeof(FliterInfo))]
        //public async Task<IHttpActionResult> PostFliterInfo(FliterInfo fliterInfo)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    db.FliterInfos.Add(fliterInfo);
        //    await db.SaveChangesAsync();

        //    return CreatedAtRoute("DefaultApi", new { id = fliterInfo.ID }, fliterInfo);
        //}

        [Route("api/FliterInfosExtend")]
        [HttpPost]
        [ResponseType(typeof(FliterInfo))]
        public async Task<HttpResponseMessage> PostFliterInfo([FromBody]string value)
        {
            PostFliterInfoModel pdata= JsonConvert.DeserializeObject<PostFliterInfoModel>(value);
            string departureCity = pdata.FilterDeparture;
            string arrivalCity = pdata.FilterArrival;
            List<FliterInfo> list = pdata.FliterInfos;   
            //抓取后去重的数据
            var splitList = list.Distinct(new FliterInfoComparer());
            //数据库里的原始数据
            List<FliterInfo> databaseList = db.FliterInfos.Where(d => (d.DepartureCity == departureCity && d.ArrivalCity == arrivalCity) ||
             (d.DepartureCity == arrivalCity && d.ArrivalCity == departureCity)).ToList();
            //抓取的数据和原始数据的差异
            var result = databaseList.Union(splitList, new FliterInfoComparer()).Except(databaseList.Intersect(splitList, new FliterInfoComparer()),new FliterInfoComparer());
            
            if(result!=null && result.Count()>0)
            {
                foreach(var item in result)
                {
                    //如果为此航班号在数据中存在，抓取数据不存在，说为修改
                    if(databaseList.Where(t=>t.FliterNum==item.FliterNum).Count()>0 && splitList.Where(t => t.FliterNum == item.FliterNum).Count() == 0)
                    {
                        FliterInfo f=db.FliterInfos.FirstOrDefault(c => c.FliterNum == item.FliterNum);
                        db.FliterInfos.Remove(f);

                    }
                    //如果为此航班号在list1中存在，说为修改
                    if (splitList.Where(t => t.FliterNum == item.FliterNum).Count() > 0 && databaseList.Where(t => t.FliterNum == item.FliterNum).Count() ==0)
                    {
                        FliterInfo f = db.FliterInfos.FirstOrDefault(c => c.FliterNum == item.FliterNum);
                        db.FliterInfos.Add(item);
                    }
                    if (splitList.Where(t => t.FliterNum == item.FliterNum).Count() > 0 && databaseList.Where(t => t.FliterNum == item.FliterNum).Count() > 0)
                    {
                        FliterInfo f = db.FliterInfos.FirstOrDefault(c => c.FliterNum == item.FliterNum);
                        db.FliterInfos.Remove(f);
                        db.FliterInfos.Add(item);
                    }
                }
                try
                {
                    int res = await db.SaveChangesAsync();

                    return new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent("OK", System.Text.Encoding.UTF8, "text/plain")
                    };

                }
                catch (Exception ex)
                {
                    string exMessage = ex.Message;
                    return new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent(exMessage, System.Text.Encoding.UTF8, "text/plain")
                    };
                }
            }
            else
            {
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("NoContent", System.Text.Encoding.UTF8, "text/plain")
                };
            }          
        }

        // DELETE: api/FliterInfosExtend/5
        [ResponseType(typeof(FliterInfo))]
        public async Task<IHttpActionResult> DeleteFliterInfo(int id)
        {
            FliterInfo fliterInfo = await db.FliterInfos.FindAsync(id);
            if (fliterInfo == null)
            {
                return NotFound();
            }

            db.FliterInfos.Remove(fliterInfo);
            await db.SaveChangesAsync();

            return Ok(fliterInfo);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool FliterInfoExists(int id)
        {
            return db.FliterInfos.Count(e => e.ID == id) > 0;
        }
    }
}