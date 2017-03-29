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
    builder.EntitySet<ExportOrder>("ExportOrders");
    config.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
    public class ExportOrdersController : ODataController
    {
        private LanghuaContent db = new LanghuaContent();

        // GET: odata/ExportOrders
        [EnableQuery]
        public IQueryable<ExportOrder> GetExportOrders()
        {
            return db.ExportOrders;
        }

        // GET: odata/ExportOrders(5)
        [EnableQuery]
        public SingleResult<ExportOrder> GetExportOrder([FromODataUri] int key)
        {
            return SingleResult.Create(db.ExportOrders.Where(exportOrder => exportOrder.ExportOrderID == key));
        }

        // PUT: odata/ExportOrders(5)
        public async Task<IHttpActionResult> Put([FromODataUri] int key, Delta<ExportOrder> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            ExportOrder exportOrder = await db.ExportOrders.FindAsync(key);
            if (exportOrder == null)
            {
                return NotFound();
            }

            patch.Put(exportOrder);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ExportOrderExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(exportOrder);
        }

        // POST: odata/ExportOrders
        public async Task<IHttpActionResult> Post(ExportOrder exportOrder)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.ExportOrders.Add(exportOrder);
            await db.SaveChangesAsync();

            return Created(exportOrder);
        }

        // PATCH: odata/ExportOrders(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public async Task<IHttpActionResult> Patch([FromODataUri] int key, Delta<ExportOrder> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            ExportOrder exportOrder = await db.ExportOrders.FindAsync(key);
            if (exportOrder == null)
            {
                return NotFound();
            }

            patch.Patch(exportOrder);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ExportOrderExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(exportOrder);
        }

        // DELETE: odata/ExportOrders(5)
        public async Task<IHttpActionResult> Delete([FromODataUri] int key)
        {
            ExportOrder exportOrder = await db.ExportOrders.FindAsync(key);
            if (exportOrder == null)
            {
                return NotFound();
            }

            db.ExportOrders.Remove(exportOrder);
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

        private bool ExportOrderExists(int key)
        {
            return db.ExportOrders.Count(e => e.ExportOrderID == key) > 0;
        }
    }
}
