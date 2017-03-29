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
using System.Web.ModelBinding;
using System.Web.OData;
using System.Web.OData.Query;
using System.Web.OData.Routing;
using WXData;

namespace LanghuaNew.Service.Controllers
{
    /*
    The WebApiConfig class may require additional changes to add a route for this controller. Merge these statements into the Register method of the WebApiConfig class as applicable. Note that OData URLs are case sensitive.

    using System.Web.OData.Builder;
    using System.Web.OData.Extensions;
    using WXData;
    ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
    builder.EntitySet<LHNew>("LHNews");
    builder.EntitySet<LHArticle>("LHArticles"); 
    config.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
    public class LHNewsController : ODataController
    {
        private WeiXinContent db = new WeiXinContent();

        // GET: odata/LHNews
        [EnableQuery]
        public IQueryable<LHNew> GetLHNews()
        {
            return db.LHNews;
        }

        // GET: odata/LHNews(5)
        [EnableQuery]
        public SingleResult<LHNew> GetLHNew([FromODataUri] int key)
        {
            return SingleResult.Create(db.LHNews.Where(lHNew => lHNew.LHNewID == key));
        }

        // PUT: odata/LHNews(5)
        public async Task<IHttpActionResult> Put([FromODataUri] int key, Delta<LHNew> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            LHNew lHNew = await db.LHNews.FindAsync(key);
            if (lHNew == null)
            {
                return NotFound();
            }

            patch.Put(lHNew);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LHNewExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(lHNew);
        }

        // POST: odata/LHNews
        public async Task<IHttpActionResult> Post(LHNew lHNew)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.LHNews.Add(lHNew);
            await db.SaveChangesAsync();

            return Created(lHNew);
        }

        // PATCH: odata/LHNews(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public async Task<IHttpActionResult> Patch([FromODataUri] int key, Delta<LHNew> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            LHNew lHNew = await db.LHNews.FindAsync(key);
            if (lHNew == null)
            {
                return NotFound();
            }

            patch.Patch(lHNew);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LHNewExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(lHNew);
        }

        // DELETE: odata/LHNews(5)
        public async Task<IHttpActionResult> Delete([FromODataUri] int key)
        {
            LHNew lHNew = await db.LHNews.FindAsync(key);
            if (lHNew == null)
            {
                return NotFound();
            }

            db.LHNews.Remove(lHNew);
            await db.SaveChangesAsync();

            return StatusCode(HttpStatusCode.NoContent);
        }

        // GET: odata/LHNews(5)/Articles
        [EnableQuery]
        public IQueryable<LHArticle> GetArticles([FromODataUri] int key)
        {
            return db.LHNews.Where(m => m.LHNewID == key).SelectMany(m => m.Articles);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool LHNewExists(int key)
        {
            return db.LHNews.Count(e => e.LHNewID == key) > 0;
        }
    }
}
