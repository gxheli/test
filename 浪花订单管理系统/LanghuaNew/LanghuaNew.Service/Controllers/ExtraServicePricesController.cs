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
    builder.EntitySet<ExtraServicePrice>("ExtraServicePrices");
    builder.EntitySet<ExtraService>("ExtraServices"); 
    config.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
    public class ExtraServicePricesController : ODataController
    {
        private LanghuaContent db = new LanghuaContent();

        // GET: odata/ExtraServicePrices
        [EnableQuery]
        public IQueryable<ExtraServicePrice> GetExtraServicePrices()
        {
            return db.ExtraServicePrices;
        }

        // GET: odata/ExtraServicePrices(5)
        [EnableQuery]
        public SingleResult<ExtraServicePrice> GetExtraServicePrice([FromODataUri] int key)
        {
            return SingleResult.Create(db.ExtraServicePrices.Where(extraServicePrice => extraServicePrice.ExtraServicePriceID == key));
        }

        // PUT: odata/ExtraServicePrices(5)
        public async Task<IHttpActionResult> Put([FromODataUri] int key, Delta<ExtraServicePrice> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            ExtraServicePrice extraServicePrice = await db.ExtraServicePrices.FindAsync(key);
            if (extraServicePrice == null)
            {
                return NotFound();
            }

            patch.Put(extraServicePrice);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ExtraServicePriceExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(extraServicePrice);
        }

        // POST: odata/ExtraServicePrices
        public async Task<IHttpActionResult> Post(ExtraServicePrice extraServicePrice)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.ExtraServicePrices.Add(extraServicePrice);
            await db.SaveChangesAsync();

            return Created(extraServicePrice);
        }

        // PATCH: odata/ExtraServicePrices(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public async Task<IHttpActionResult> Patch([FromODataUri] int key, Delta<ExtraServicePrice> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            ExtraServicePrice extraServicePrice = await db.ExtraServicePrices.FindAsync(key);
            if (extraServicePrice == null)
            {
                return NotFound();
            }

            patch.Patch(extraServicePrice);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ExtraServicePriceExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(extraServicePrice);
        }

        // DELETE: odata/ExtraServicePrices(5)
        public async Task<IHttpActionResult> Delete([FromODataUri] int key)
        {
            ExtraServicePrice extraServicePrice = await db.ExtraServicePrices.FindAsync(key);
            if (extraServicePrice == null)
            {
                return NotFound();
            }

            db.ExtraServicePrices.Remove(extraServicePrice);
            await db.SaveChangesAsync();

            return StatusCode(HttpStatusCode.NoContent);
        }

        // GET: odata/ExtraServicePrices(5)/Service
        [EnableQuery]
        public SingleResult<ExtraService> GetService([FromODataUri] int key)
        {
            return SingleResult.Create(db.ExtraServicePrices.Where(m => m.ExtraServicePriceID == key).Select(m => m.Service));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ExtraServicePriceExists(int key)
        {
            return db.ExtraServicePrices.Count(e => e.ExtraServicePriceID == key) > 0;
        }
    }
}
