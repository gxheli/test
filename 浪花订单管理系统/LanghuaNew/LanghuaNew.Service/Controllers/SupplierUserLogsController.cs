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
    builder.EntitySet<SupplierUserLog>("SupplierUserLogs");
    config.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
    public class SupplierUserLogsController : ODataController
    {
        private LanghuaContent db = new LanghuaContent();

        // GET: odata/SupplierUserLogs
        [EnableQuery]
        public IQueryable<SupplierUserLog> GetSupplierUserLogs()
        {
            return db.SupplierUserLogs;
        }

        // GET: odata/SupplierUserLogs(5)
        [EnableQuery]
        public SingleResult<SupplierUserLog> GetSupplierUserLog([FromODataUri] int key)
        {
            return SingleResult.Create(db.SupplierUserLogs.Where(supplierUserLog => supplierUserLog.SupplierUserLogID == key));
        }

        // PUT: odata/SupplierUserLogs(5)
        public async Task<IHttpActionResult> Put([FromODataUri] int key, Delta<SupplierUserLog> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            SupplierUserLog supplierUserLog = await db.SupplierUserLogs.FindAsync(key);
            if (supplierUserLog == null)
            {
                return NotFound();
            }

            patch.Put(supplierUserLog);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SupplierUserLogExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(supplierUserLog);
        }

        // POST: odata/SupplierUserLogs
        public async Task<IHttpActionResult> Post(SupplierUserLog supplierUserLog)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.SupplierUserLogs.Add(supplierUserLog);
            await db.SaveChangesAsync();

            return Created(supplierUserLog);
        }

        // PATCH: odata/SupplierUserLogs(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public async Task<IHttpActionResult> Patch([FromODataUri] int key, Delta<SupplierUserLog> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            SupplierUserLog supplierUserLog = await db.SupplierUserLogs.FindAsync(key);
            if (supplierUserLog == null)
            {
                return NotFound();
            }

            patch.Patch(supplierUserLog);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SupplierUserLogExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(supplierUserLog);
        }

        // DELETE: odata/SupplierUserLogs(5)
        public async Task<IHttpActionResult> Delete([FromODataUri] int key)
        {
            SupplierUserLog supplierUserLog = await db.SupplierUserLogs.FindAsync(key);
            if (supplierUserLog == null)
            {
                return NotFound();
            }

            db.SupplierUserLogs.Remove(supplierUserLog);
            await db.SaveChangesAsync();

            return StatusCode(HttpStatusCode.NoContent);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool SupplierUserLogExists(int key)
        {
            return db.SupplierUserLogs.Count(e => e.SupplierUserLogID == key) > 0;
        }
    }
}
