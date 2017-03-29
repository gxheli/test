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
using LanghuaNew.Data;

namespace LanghuaNew.Service.Controllers
{
    /*
    The WebApiConfig class may require additional changes to add a route for this controller. Merge these statements into the Register method of the WebApiConfig class as applicable. Note that OData URLs are case sensitive.

    using System.Web.OData.Builder;
    using System.Web.OData.Extensions;
    using LanghuaNew.Data;
    ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
    builder.EntitySet<SellControlClassify>("SellControlClassifies");
    builder.EntitySet<SellControl>("SellControls"); 
    config.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
    public class SellControlClassifiesController : ODataController
    {
        private LanghuaContent db = new LanghuaContent();

        // GET: odata/SellControlClassifies
        [EnableQuery]
        public IQueryable<SellControlClassify> GetSellControlClassifies()
        {
            return db.SellControlClassifies;
        }

        // GET: odata/SellControlClassifies(5)
        [EnableQuery]
        public SingleResult<SellControlClassify> GetSellControlClassify([FromODataUri] int key)
        {
            return SingleResult.Create(db.SellControlClassifies.Where(sellControlClassify => sellControlClassify.SellControlClassifyID == key));
        }

        // PUT: odata/SellControlClassifies(5)
        public async Task<IHttpActionResult> Put([FromODataUri] int key, Delta<SellControlClassify> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            SellControlClassify sellControlClassify = await db.SellControlClassifies.FindAsync(key);
            if (sellControlClassify == null)
            {
                return NotFound();
            }

            patch.Put(sellControlClassify);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SellControlClassifyExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(sellControlClassify);
        }

        // POST: odata/SellControlClassifies
        public async Task<IHttpActionResult> Post(SellControlClassify sellControlClassify)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.SellControlClassifies.Add(sellControlClassify);
            await db.SaveChangesAsync();

            return Created(sellControlClassify);
        }

        // PATCH: odata/SellControlClassifies(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public async Task<IHttpActionResult> Patch([FromODataUri] int key, Delta<SellControlClassify> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            SellControlClassify sellControlClassify = await db.SellControlClassifies.FindAsync(key);
            if (sellControlClassify == null)
            {
                return NotFound();
            }

            patch.Patch(sellControlClassify);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SellControlClassifyExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(sellControlClassify);
        }

        // DELETE: odata/SellControlClassifies(5)
        public async Task<IHttpActionResult> Delete([FromODataUri] int key)
        {
            SellControlClassify sellControlClassify = await db.SellControlClassifies.FindAsync(key);
            if (sellControlClassify == null)
            {
                return NotFound();
            }

            db.SellControlClassifies.Remove(sellControlClassify);
            await db.SaveChangesAsync();

            return StatusCode(HttpStatusCode.NoContent);
        }

        // GET: odata/SellControlClassifies(5)/sell
        [EnableQuery]
        public IQueryable<SellControl> Getsell([FromODataUri] int key)
        {
            return db.SellControlClassifies.Where(m => m.SellControlClassifyID == key).SelectMany(m => m.sell);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool SellControlClassifyExists(int key)
        {
            return db.SellControlClassifies.Count(e => e.SellControlClassifyID == key) > 0;
        }
    }
}
