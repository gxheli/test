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
    public class WeiXinMenusExtendController : ApiController
    {
        private WeiXinContent  db = new WeiXinContent();
        // GET: api/UsersExtend
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/UsersExtend/5
        //public string Get(int id)
        //{
        //    return "value";
        //}

        // POST: api/WeiXinMenusExtend
        public async Task<HttpResponseMessage> Post([FromBody]string value)
        {
            try
            {
                db.Database.ExecuteSqlCommand("delete MenuItems");
                db.Database.ExecuteSqlCommand("delete WeiXinMenus");
                List<WeiXinMenu> News = JsonConvert.DeserializeObject<List<WeiXinMenu>>(value);
                if (News == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }
                db.WeiXinMenus.AddRange(News);
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

     

        // DELETE: api/UsersExtend/5
        public void Delete(int id)
        {
        }
    }
}
