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
    builder.EntitySet<OrderSupplierHistory>("OrderSupplierHistories");
    builder.EntitySet<Order>("Orders"); 
    config.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
    public class OrderSupplierHistoriesController : ODataController
    {
        private LanghuaContent db = new LanghuaContent();

        // GET: odata/OrderSupplierHistories
        [EnableQuery]
        public IQueryable<OrderSupplierHistory> GetOrderSupplierHistories()
        {
            return db.OrderSupplierHistories;
        }

        // GET: odata/OrderSupplierHistories(5)
        [EnableQuery]
        public SingleResult<OrderSupplierHistory> GetOrderSupplierHistory([FromODataUri] int key)
        {
            return SingleResult.Create(db.OrderSupplierHistories.Where(orderSupplierHistory => orderSupplierHistory.OrderSupplierHistoryID == key));
        }

        // PUT: odata/OrderSupplierHistories(5)
        public async Task<IHttpActionResult> Put([FromODataUri] int key, Delta<OrderSupplierHistory> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            OrderSupplierHistory orderSupplierHistory = await db.OrderSupplierHistories.FindAsync(key);
            if (orderSupplierHistory == null)
            {
                return NotFound();
            }

            patch.Put(orderSupplierHistory);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderSupplierHistoryExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(orderSupplierHistory);
        }

        // POST: odata/OrderSupplierHistories
        public async Task<IHttpActionResult> Post(OrderSupplierHistory orderSupplierHistory)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.OrderSupplierHistories.Add(orderSupplierHistory);
            await db.SaveChangesAsync();

            return Created(orderSupplierHistory);
        }

        // PATCH: odata/OrderSupplierHistories(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public async Task<IHttpActionResult> Patch([FromODataUri] int key, Delta<OrderSupplierHistory> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            OrderSupplierHistory orderSupplierHistory = await db.OrderSupplierHistories.FindAsync(key);
            if (orderSupplierHistory == null)
            {
                return NotFound();
            }

            patch.Patch(orderSupplierHistory);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderSupplierHistoryExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(orderSupplierHistory);
        }

        // DELETE: odata/OrderSupplierHistories(5)
        public async Task<IHttpActionResult> Delete([FromODataUri] int key)
        {
            OrderSupplierHistory orderSupplierHistory = await db.OrderSupplierHistories.FindAsync(key);
            if (orderSupplierHistory == null)
            {
                return NotFound();
            }

            db.OrderSupplierHistories.Remove(orderSupplierHistory);
            await db.SaveChangesAsync();

            return StatusCode(HttpStatusCode.NoContent);
        }

        // GET: odata/OrderSupplierHistories(5)/order
        [EnableQuery]
        public SingleResult<Order> Getorder([FromODataUri] int key)
        {
            return SingleResult.Create(db.OrderSupplierHistories.Where(m => m.OrderSupplierHistoryID == key).Select(m => m.order));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool OrderSupplierHistoryExists(int key)
        {
            return db.OrderSupplierHistories.Count(e => e.OrderSupplierHistoryID == key) > 0;
        }
    }
}
