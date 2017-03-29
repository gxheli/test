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
    builder.EntitySet<BillReport>("BillReports");
    builder.EntitySet<Supplier>("Suppliers"); 
    config.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
    public class BillReportsController : ODataController
    {
        private LanghuaContent db = new LanghuaContent();

        // GET: odata/BillReports
        [EnableQuery]
        public IQueryable<BillReport> GetBillReports()
        {
            return db.BillReports;
        }

        // GET: odata/BillReports(5)
        [EnableQuery]
        public SingleResult<BillReport> GetBillReport([FromODataUri] int key)
        {
            return SingleResult.Create(db.BillReports.Where(billReport => billReport.BillReportID == key));
        }

        // PUT: odata/BillReports(5)
        public async Task<IHttpActionResult> Put([FromODataUri] int key, Delta<BillReport> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            BillReport billReport = await db.BillReports.FindAsync(key);
            if (billReport == null)
            {
                return NotFound();
            }

            patch.Put(billReport);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BillReportExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(billReport);
        }

        // POST: odata/BillReports
        public async Task<IHttpActionResult> Post(BillReport billReport)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.BillReports.Add(billReport);
            await db.SaveChangesAsync();

            return Created(billReport);
        }

        // PATCH: odata/BillReports(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public async Task<IHttpActionResult> Patch([FromODataUri] int key, Delta<BillReport> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            BillReport billReport = await db.BillReports.FindAsync(key);
            if (billReport == null)
            {
                return NotFound();
            }

            patch.Patch(billReport);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BillReportExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(billReport);
        }

        // DELETE: odata/BillReports(5)
        public async Task<IHttpActionResult> Delete([FromODataUri] int key)
        {
            BillReport billReport = await db.BillReports.FindAsync(key);
            if (billReport == null)
            {
                return NotFound();
            }

            db.BillReports.Remove(billReport);
            await db.SaveChangesAsync();

            return StatusCode(HttpStatusCode.NoContent);
        }

        // GET: odata/BillReports(5)/oneSupplier
        [EnableQuery]
        public SingleResult<Supplier> GetoneSupplier([FromODataUri] int key)
        {
            return SingleResult.Create(db.BillReports.Where(m => m.BillReportID == key).Select(m => m.oneSupplier));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool BillReportExists(int key)
        {
            return db.BillReports.Count(e => e.BillReportID == key) > 0;
        }
    }
}
