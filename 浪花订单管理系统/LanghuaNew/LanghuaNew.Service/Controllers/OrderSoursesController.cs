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
    builder.EntitySet<OrderSourse>("OrderSourses");
    builder.EntitySet<TBOrder>("TBOrders"); 
    config.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
    public class OrderSoursesController : ODataController
    {
        private LanghuaContent db = new LanghuaContent();

        // GET: odata/OrderSourses
        [EnableQuery]
        public IQueryable<OrderSourse> GetOrderSourses()
        {
            return db.OrderSourses;
        }

        // GET: odata/OrderSourses(5)
        [EnableQuery]
        public SingleResult<OrderSourse> GetOrderSourse([FromODataUri] int key)
        {
            return SingleResult.Create(db.OrderSourses.Where(orderSourse => orderSourse.OrderSourseID == key));
        }

        // PUT: odata/OrderSourses(5)
        public async Task<IHttpActionResult> Put([FromODataUri] int key, Delta<OrderSourse> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            OrderSourse orderSourse = await db.OrderSourses.FindAsync(key);
            if (orderSourse == null)
            {
                return NotFound();
            }

            patch.Put(orderSourse);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderSourseExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(orderSourse);
        }

        // POST: odata/OrderSourses
        public async Task<IHttpActionResult> Post(OrderSourse orderSourse)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.OrderSourses.Add(orderSourse);
            await db.SaveChangesAsync();

            return Created(orderSourse);
        }

        // PATCH: odata/OrderSourses(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public async Task<IHttpActionResult> Patch([FromODataUri] int key, Delta<OrderSourse> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            OrderSourse orderSourse = await db.OrderSourses.FindAsync(key);
            if (orderSourse == null)
            {
                return NotFound();
            }

            patch.Patch(orderSourse);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderSourseExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(orderSourse);
        }

        // DELETE: odata/OrderSourses(5)
        public async Task<IHttpActionResult> Delete([FromODataUri] int key)
        {
            OrderSourse orderSourse = await db.OrderSourses.FindAsync(key);
            if (orderSourse == null)
            {
                return NotFound();
            }

            db.OrderSourses.Remove(orderSourse);
            await db.SaveChangesAsync();

            return StatusCode(HttpStatusCode.NoContent);
        }

        // GET: odata/OrderSourses(5)/TBOrders
        [EnableQuery]
        public IQueryable<TBOrder> GetTBOrders([FromODataUri] int key)
        {
            return db.OrderSourses.Where(m => m.OrderSourseID == key).SelectMany(m => m.TBOrders);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool OrderSourseExists(int key)
        {
            return db.OrderSourses.Count(e => e.OrderSourseID == key) > 0;
        }
    }
}
