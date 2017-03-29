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
    builder.EntitySet<OrderHistory>("OrderHistories");
    builder.EntitySet<Order>("Orders"); 
    config.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
    public class OrderHistoriesController : ODataController
    {
        private LanghuaContent db = new LanghuaContent();

        // GET: odata/OrderHistories
        [EnableQuery]
        public IQueryable<OrderHistory> GetOrderHistories()
        {
            return db.OrderHistories;
        }

        // GET: odata/OrderHistories(5)
        [EnableQuery]
        public SingleResult<OrderHistory> GetOrderHistory([FromODataUri] int key)
        {
            return SingleResult.Create(db.OrderHistories.Where(orderHistory => orderHistory.OrderHistoryID == key));
        }

        // PUT: odata/OrderHistories(5)
        public async Task<IHttpActionResult> Put([FromODataUri] int key, Delta<OrderHistory> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            OrderHistory orderHistory = await db.OrderHistories.FindAsync(key);
            if (orderHistory == null)
            {
                return NotFound();
            }

            patch.Put(orderHistory);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderHistoryExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(orderHistory);
        }

        // POST: odata/OrderHistories
        public async Task<IHttpActionResult> Post(OrderHistory orderHistory)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.OrderHistories.Add(orderHistory);
            await db.SaveChangesAsync();

            return Created(orderHistory);
        }

        // PATCH: odata/OrderHistories(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public async Task<IHttpActionResult> Patch([FromODataUri] int key, Delta<OrderHistory> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            OrderHistory orderHistory = await db.OrderHistories.FindAsync(key);
            if (orderHistory == null)
            {
                return NotFound();
            }

            patch.Patch(orderHistory);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderHistoryExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(orderHistory);
        }

        // DELETE: odata/OrderHistories(5)
        public async Task<IHttpActionResult> Delete([FromODataUri] int key)
        {
            OrderHistory orderHistory = await db.OrderHistories.FindAsync(key);
            if (orderHistory == null)
            {
                return NotFound();
            }

            db.OrderHistories.Remove(orderHistory);
            await db.SaveChangesAsync();

            return StatusCode(HttpStatusCode.NoContent);
        }

        // GET: odata/OrderHistories(5)/order
        [EnableQuery]
        public SingleResult<Order> Getorder([FromODataUri] int key)
        {
            return SingleResult.Create(db.OrderHistories.Where(m => m.OrderHistoryID == key).Select(m => m.order));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool OrderHistoryExists(int key)
        {
            return db.OrderHistories.Count(e => e.OrderHistoryID == key) > 0;
        }
    }
}
