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
    builder.EntitySet<LHArticle>("LHArticles");
    builder.EntitySet<LHNew>("LHNews"); 
    config.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
    public class LHArticlesController : ODataController
    {
        private WeiXinContent db = new WeiXinContent();

        // GET: odata/LHArticles
        [EnableQuery]
        public IQueryable<LHArticle> GetLHArticles()
        {
            return db.LHArticles;
        }

        // GET: odata/LHArticles(5)
        [EnableQuery]
        public SingleResult<LHArticle> GetLHArticle([FromODataUri] int key)
        {
            return SingleResult.Create(db.LHArticles.Where(lHArticle => lHArticle.LHArticleID == key));
        }

        // PUT: odata/LHArticles(5)
        public async Task<IHttpActionResult> Put([FromODataUri] int key, Delta<LHArticle> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            LHArticle lHArticle = await db.LHArticles.FindAsync(key);
            if (lHArticle == null)
            {
                return NotFound();
            }

            patch.Put(lHArticle);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LHArticleExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(lHArticle);
        }

        // POST: odata/LHArticles
        public async Task<IHttpActionResult> Post(LHArticle lHArticle)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.LHArticles.Add(lHArticle);
            await db.SaveChangesAsync();

            return Created(lHArticle);
        }

        // PATCH: odata/LHArticles(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public async Task<IHttpActionResult> Patch([FromODataUri] int key, Delta<LHArticle> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            LHArticle lHArticle = await db.LHArticles.FindAsync(key);
            if (lHArticle == null)
            {
                return NotFound();
            }

            patch.Patch(lHArticle);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LHArticleExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(lHArticle);
        }

        // DELETE: odata/LHArticles(5)
        public async Task<IHttpActionResult> Delete([FromODataUri] int key)
        {
            LHArticle lHArticle = await db.LHArticles.FindAsync(key);
            if (lHArticle == null)
            {
                return NotFound();
            }

            db.LHArticles.Remove(lHArticle);
            await db.SaveChangesAsync();

            return StatusCode(HttpStatusCode.NoContent);
        }

        // GET: odata/LHArticles(5)/LHNew
        [EnableQuery]
        public SingleResult<LHNew> GetLHNew([FromODataUri] int key)
        {
            return SingleResult.Create(db.LHArticles.Where(m => m.LHArticleID == key).Select(m => m.LHNew));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool LHArticleExists(int key)
        {
            return db.LHArticles.Count(e => e.LHArticleID == key) > 0;
        }
    }
}
