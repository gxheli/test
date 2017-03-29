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
    builder.EntitySet<Order>("Orders");
    builder.EntitySet<Customer>("Customers"); 
    builder.EntitySet<OrderHistory>("OrderHistories"); 
    builder.EntitySet<ServiceItemHistory>("ServiceItemHistories"); 
    builder.EntitySet<TBOrder>("TBOrders"); 
    config.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
    public class OrdersController : ODataController
    {
        private LanghuaContent db = new LanghuaContent();

        // GET: odata/Orders
        [EnableQuery]
        public IQueryable<Order> GetOrders()
        {
            return db.Orders;
        }

        // GET: odata/Orders(5)
        [EnableQuery]
        public SingleResult<Order> GetOrder([FromODataUri] int key)
        {
            return SingleResult.Create(db.Orders.Where(order => order.OrderID == key));
        }

        // PUT: odata/Orders(5)
        public async Task<IHttpActionResult> Put([FromODataUri] int key, Delta<Order> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Order order = await db.Orders.FindAsync(key);
            if (order == null)
            {
                return NotFound();
            }

            patch.Put(order);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(order);
        }

        // POST: odata/Orders
        public async Task<IHttpActionResult> Post(Order order)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Orders.Add(order);
            await db.SaveChangesAsync();

            return Created(order);
        }

        // PATCH: odata/Orders(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public async Task<IHttpActionResult> Patch([FromODataUri] int key, Delta<Order> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Order order = await db.Orders.FindAsync(key);
            if (order == null)
            {
                return NotFound();
            }

            patch.Patch(order);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(order);
        }

        // DELETE: odata/Orders(5)
        public async Task<IHttpActionResult> Delete([FromODataUri] int key)
        {
            Order order = await db.Orders.FindAsync(key);
            if (order == null)
            {
                return NotFound();
            }

            db.Orders.Remove(order);
            await db.SaveChangesAsync();

            return StatusCode(HttpStatusCode.NoContent);
        }

        // GET: odata/Orders(5)/Customers
        [EnableQuery]
        public SingleResult<Customer> GetCustomers([FromODataUri] int key)
        {
            return SingleResult.Create(db.Orders.Where(m => m.OrderID == key).Select(m => m.Customers));
        }

        // GET: odata/Orders(5)/OrderHistorys
        [EnableQuery]
        public IQueryable<OrderHistory> GetOrderHistorys([FromODataUri] int key)
        {
            return db.Orders.Where(m => m.OrderID == key).SelectMany(m => m.OrderHistorys);
        }

        // GET: odata/Orders(5)/ServiceItemHistorys
        [EnableQuery]
        public SingleResult<ServiceItemHistory> GetServiceItemHistorys([FromODataUri] int key)
        {
            return SingleResult.Create(db.Orders.Where(m => m.OrderID == key).Select(m => m.ServiceItemHistorys));
        }

        // GET: odata/Orders(5)/TBOrders
        [EnableQuery]
        public SingleResult<TBOrder> GetTBOrders([FromODataUri] int key)
        {
            return SingleResult.Create(db.Orders.Where(m => m.OrderID == key).Select(m => m.TBOrders));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool OrderExists(int key)
        {
            return db.Orders.Count(e => e.OrderID == key) > 0;
        }
    }
}
