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
    public class CountriesExtendController : ApiController
    {
        private LanghuaContent db = new LanghuaContent();
        // GET: api/CountriesExtend
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/CountriesExtend/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/CountriesExtend
        public async Task<HttpResponseMessage> Post([FromBody]string value)
        {
            try
            {
                Country country = JsonConvert.DeserializeObject<Country>(value);
                if (country == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }

                if (country.CountryID > 0)
                {
                    if (country.Citys[0].CityID > 0)
                    {
                        foreach (var area in country.Citys[0].Areas)
                        {
                            area.CityID = country.Citys[0].CityID;
                            db.Areas.Add(area);
                        }
                    }
                    else
                    {
                        country.Citys[0].CountryID = country.CountryID;
                        db.Cities.Add(country.Citys[0]);
                    }
                }
                else
                {
                    db.Countrys.Add(country);
                }
                await db.SaveChangesAsync();

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

        // PUT: api/CountriesExtend/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/CountriesExtend/5
        public void Delete(int id)
        {
        }
    }
}
