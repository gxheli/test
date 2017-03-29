using LanghuaNew.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Data.Entity;
using LanghuaNew.Service.App_Code;
using WXData;

namespace LanghuaNew.Service.Controllers
{
    public class LHNewsExtendController : ApiController
    {
        private WeiXinContent  db = new WeiXinContent();
        // GET: api/LHNewsExtend
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/UsersExtend/5
        //public string Get(int id)
        //{
        //    return "value";
        //}

        // POST: api/LHNewsExtend
        public async Task<HttpResponseMessage> Post([FromBody]string value)
        {
            try
            {
                List<LHNew> News = JsonConvert.DeserializeObject<List<LHNew>>(value);
                if (News == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }
                db.LHNews.AddRange(News);
                await db.SaveChangesAsync();
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("OK", System.Text.Encoding.UTF8, "text/plain")
                };
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;

            }
            return Request.CreateResponse(HttpStatusCode.NoContent);
        }

        public List<LHNew> GetNewsByKeyWord(string KeyWord)
        {
            var NewsID = db.LHArticles.Where(p => p.Title.Contains(KeyWord)).Distinct().Select(p=>p.LHNewID).ToList();
            var News = db.LHNews.Where(p => NewsID.Contains(p.LHNewID)).Include(p=>p.Articles);
            return News.ToList(); 
        }

       

        // PUT: api/UsersExtend/5


        // DELETE: api/UsersExtend/5
        public void Delete(int id)
        {
        }
    }
}
