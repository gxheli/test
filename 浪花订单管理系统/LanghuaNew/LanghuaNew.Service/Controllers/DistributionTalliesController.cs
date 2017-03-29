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
    builder.EntitySet<DistributionTally>("DistributionTallies");
    builder.EntitySet<ServiceItem>("ServiceItems"); 
    builder.EntitySet<Supplier>("Suppliers"); 
    config.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
    public class DistributionTalliesController : ODataController
    {
        private LanghuaContent db = new LanghuaContent();

        // GET: odata/DistributionTallies
        [EnableQuery]
        public IQueryable<DistributionTally> GetDistributionTallies()
        {
            return db.DistributionTallys;
        }

        // GET: odata/DistributionTallies(5)
        [EnableQuery]
        public SingleResult<DistributionTally> GetDistributionTally([FromODataUri] int key)
        {
            return SingleResult.Create(db.DistributionTallys.Where(distributionTally => distributionTally.DistributionTallyID == key));
        }

        // PUT: odata/DistributionTallies(5)
        public async Task<IHttpActionResult> Put([FromODataUri] int key, Delta<DistributionTally> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            DistributionTally distributionTally = await db.DistributionTallys.FindAsync(key);
            if (distributionTally == null)
            {
                return NotFound();
            }

            patch.Put(distributionTally);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DistributionTallyExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(distributionTally);
        }

        // POST: odata/DistributionTallies
        public async Task<IHttpActionResult> Post(DistributionTally distributionTally)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.DistributionTallys.Add(distributionTally);
            await db.SaveChangesAsync();

            return Created(distributionTally);
        }

        // PATCH: odata/DistributionTallies(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public async Task<IHttpActionResult> Patch([FromODataUri] int key, Delta<DistributionTally> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            DistributionTally distributionTally = await db.DistributionTallys.FindAsync(key);
            if (distributionTally == null)
            {
                return NotFound();
            }

            patch.Patch(distributionTally);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DistributionTallyExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(distributionTally);
        }

        // DELETE: odata/DistributionTallies(5)
        public async Task<IHttpActionResult> Delete([FromODataUri] int key)
        {
            DistributionTally distributionTally = await db.DistributionTallys.FindAsync(key);
            if (distributionTally == null)
            {
                return NotFound();
            }

            db.DistributionTallys.Remove(distributionTally);
            await db.SaveChangesAsync();

            return StatusCode(HttpStatusCode.NoContent);
        }

        // GET: odata/DistributionTallies(5)/serviceItem
        [EnableQuery]
        public SingleResult<ServiceItem> GetserviceItem([FromODataUri] int key)
        {
            return SingleResult.Create(db.DistributionTallys.Where(m => m.DistributionTallyID == key).Select(m => m.serviceItem));
        }

        // GET: odata/DistributionTallies(5)/supplier
        [EnableQuery]
        public SingleResult<Supplier> Getsupplier([FromODataUri] int key)
        {
            return SingleResult.Create(db.DistributionTallys.Where(m => m.DistributionTallyID == key).Select(m => m.supplier));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool DistributionTallyExists(int key)
        {
            return db.DistributionTallys.Count(e => e.DistributionTallyID == key) > 0;
        }
    }
}
