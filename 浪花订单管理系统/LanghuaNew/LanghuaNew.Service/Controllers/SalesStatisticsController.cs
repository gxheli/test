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
    builder.EntitySet<SalesStatistic>("SalesStatistics");
    builder.EntitySet<ServiceItem>("ServiceItems"); 
    builder.EntitySet<Supplier>("Suppliers"); 
    config.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
    public class SalesStatisticsController : ODataController
    {
        private LanghuaContent db = new LanghuaContent();

        // GET: odata/SalesStatistics
        [EnableQuery]
        public IQueryable<SalesStatistic> GetSalesStatistics()
        {
            return db.SalesStatistics;
        }

        // GET: odata/SalesStatistics(5)
        [EnableQuery]
        public SingleResult<SalesStatistic> GetSalesStatistic([FromODataUri] int key)
        {
            return SingleResult.Create(db.SalesStatistics.Where(salesStatistic => salesStatistic.SalesStatisticID == key));
        }

        // PUT: odata/SalesStatistics(5)
        public async Task<IHttpActionResult> Put([FromODataUri] int key, Delta<SalesStatistic> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            SalesStatistic salesStatistic = await db.SalesStatistics.FindAsync(key);
            if (salesStatistic == null)
            {
                return NotFound();
            }

            patch.Put(salesStatistic);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SalesStatisticExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(salesStatistic);
        }

        // POST: odata/SalesStatistics
        public async Task<IHttpActionResult> Post(SalesStatistic salesStatistic)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.SalesStatistics.Add(salesStatistic);
            await db.SaveChangesAsync();

            return Created(salesStatistic);
        }

        // PATCH: odata/SalesStatistics(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public async Task<IHttpActionResult> Patch([FromODataUri] int key, Delta<SalesStatistic> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            SalesStatistic salesStatistic = await db.SalesStatistics.FindAsync(key);
            if (salesStatistic == null)
            {
                return NotFound();
            }

            patch.Patch(salesStatistic);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SalesStatisticExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(salesStatistic);
        }

        // DELETE: odata/SalesStatistics(5)
        public async Task<IHttpActionResult> Delete([FromODataUri] int key)
        {
            SalesStatistic salesStatistic = await db.SalesStatistics.FindAsync(key);
            if (salesStatistic == null)
            {
                return NotFound();
            }

            db.SalesStatistics.Remove(salesStatistic);
            await db.SaveChangesAsync();

            return StatusCode(HttpStatusCode.NoContent);
        }

        // GET: odata/SalesStatistics(5)/serviceItem
        [EnableQuery]
        public SingleResult<ServiceItem> GetserviceItem([FromODataUri] int key)
        {
            return SingleResult.Create(db.SalesStatistics.Where(m => m.SalesStatisticID == key).Select(m => m.serviceItem));
        }

        // GET: odata/SalesStatistics(5)/supplier
        [EnableQuery]
        public SingleResult<Supplier> Getsupplier([FromODataUri] int key)
        {
            return SingleResult.Create(db.SalesStatistics.Where(m => m.SalesStatisticID == key).Select(m => m.supplier));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool SalesStatisticExists(int key)
        {
            return db.SalesStatistics.Count(e => e.SalesStatisticID == key) > 0;
        }
    }
}
