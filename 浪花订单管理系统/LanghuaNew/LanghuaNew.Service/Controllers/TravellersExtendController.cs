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

namespace LanghuaNew.Service.Controllers
{
    public class TravellersExtendController : ApiController
    {
        private LanghuaContent db = new LanghuaContent();
        // GET: api/TravellersExtend
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/TravellersExtend/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/TravellersExtend
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/TravellersExtend/5
        public async Task<IHttpActionResult> Put([FromBody]string value)
        {
            try
            {
                Traveller traveller = JsonConvert.DeserializeObject<Traveller>(value);
                if (traveller == null)
                {
                    return NotFound();
                }
                db.Entry(traveller).State = EntityState.Modified;
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // DELETE: api/TravellersExtend/5
        public void Delete(int id)
        {
        }
    }
}
