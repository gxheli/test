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
    builder.EntitySet<TBOrder>("TBOrders");
    builder.EntitySet<Order>("Orders"); 
    builder.EntitySet<OrderSourse>("OrderSourses"); 
    config.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
    public class TBOrdersController : ODataController
    {
        private LanghuaContent db = new LanghuaContent();

        // GET: odata/TBOrders
        [EnableQuery]
        public IQueryable<TBOrder> GetTBOrders()
        {
            return db.TBOrders;
        }

        // GET: odata/TBOrders(5)
        [EnableQuery]
        public SingleResult<TBOrder> GetTBOrder([FromODataUri] int key)
        {
            return SingleResult.Create(db.TBOrders.Where(tBOrder => tBOrder.TBOrderID == key));
        }

        // PUT: odata/TBOrders(5)
        public async Task<IHttpActionResult> Put([FromODataUri] int key, Delta<TBOrder> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            TBOrder tBOrder = await db.TBOrders.FindAsync(key);
            if (tBOrder == null)
            {
                return NotFound();
            }

            patch.Put(tBOrder);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TBOrderExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(tBOrder);
        }

        // POST: odata/TBOrders
        public async Task<IHttpActionResult> Post(TBOrder tBOrder)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.TBOrders.Add(tBOrder);
            await db.SaveChangesAsync();

            return Created(tBOrder);
        }

        // PATCH: odata/TBOrders(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public async Task<IHttpActionResult> Patch([FromODataUri] int key, Delta<TBOrder> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            TBOrder tBOrder = await db.TBOrders.FindAsync(key);
            if (tBOrder == null)
            {
                return NotFound();
            }

            patch.Patch(tBOrder);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TBOrderExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(tBOrder);
        }

        // DELETE: odata/TBOrders(5)
        public async Task<IHttpActionResult> Delete([FromODataUri] int key)
        {
            TBOrder tBOrder = await db.TBOrders.FindAsync(key);
            if (tBOrder == null)
            {
                return NotFound();
            }

            db.TBOrders.Remove(tBOrder);
            await db.SaveChangesAsync();

            return StatusCode(HttpStatusCode.NoContent);
        }

        // GET: odata/TBOrders(5)/Orders
        [EnableQuery]
        public IQueryable<Order> GetOrders([FromODataUri] int key)
        {
            return db.TBOrders.Where(m => m.TBOrderID == key).SelectMany(m => m.Orders);
        }

        // GET: odata/TBOrders(5)/Sourse
        [EnableQuery]
        public SingleResult<OrderSourse> GetSourse([FromODataUri] int key)
        {
            return SingleResult.Create(db.TBOrders.Where(m => m.TBOrderID == key).Select(m => m.Sourse));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool TBOrderExists(int key)
        {
            return db.TBOrders.Count(e => e.TBOrderID == key) > 0;
        }
    }
}
