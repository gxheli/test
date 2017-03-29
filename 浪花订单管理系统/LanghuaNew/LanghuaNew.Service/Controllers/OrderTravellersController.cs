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
    builder.EntitySet<OrderTraveller>("OrderTravellers");
    builder.EntitySet<Customer>("Customers"); 
    builder.EntitySet<ServiceItemHistory>("ServiceItemHistories"); 
    config.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
    public class OrderTravellersController : ODataController
    {
        private LanghuaContent db = new LanghuaContent();

        // GET: odata/OrderTravellers
        [EnableQuery]
        public IQueryable<OrderTraveller> GetOrderTravellers()
        {
            return db.OrderTravellers;
        }

        // GET: odata/OrderTravellers(5)
        [EnableQuery]
        public SingleResult<OrderTraveller> GetOrderTraveller([FromODataUri] int key)
        {
            return SingleResult.Create(db.OrderTravellers.Where(orderTraveller => orderTraveller.OrderTravellerID == key));
        }

        // PUT: odata/OrderTravellers(5)
        public async Task<IHttpActionResult> Put([FromODataUri] int key, Delta<OrderTraveller> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            OrderTraveller orderTraveller = await db.OrderTravellers.FindAsync(key);
            if (orderTraveller == null)
            {
                return NotFound();
            }

            patch.Put(orderTraveller);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderTravellerExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(orderTraveller);
        }

        // POST: odata/OrderTravellers
        public async Task<IHttpActionResult> Post(OrderTraveller orderTraveller)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.OrderTravellers.Add(orderTraveller);
            await db.SaveChangesAsync();

            return Created(orderTraveller);
        }

        // PATCH: odata/OrderTravellers(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public async Task<IHttpActionResult> Patch([FromODataUri] int key, Delta<OrderTraveller> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            OrderTraveller orderTraveller = await db.OrderTravellers.FindAsync(key);
            if (orderTraveller == null)
            {
                return NotFound();
            }

            patch.Patch(orderTraveller);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderTravellerExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(orderTraveller);
        }

        // DELETE: odata/OrderTravellers(5)
        public async Task<IHttpActionResult> Delete([FromODataUri] int key)
        {
            OrderTraveller orderTraveller = await db.OrderTravellers.FindAsync(key);
            if (orderTraveller == null)
            {
                return NotFound();
            }

            db.OrderTravellers.Remove(orderTraveller);
            await db.SaveChangesAsync();

            return StatusCode(HttpStatusCode.NoContent);
        }

        // GET: odata/OrderTravellers(5)/CustomerValue
        [EnableQuery]
        public SingleResult<Customer> GetCustomerValue([FromODataUri] int key)
        {
            return SingleResult.Create(db.OrderTravellers.Where(m => m.OrderTravellerID == key).Select(m => m.CustomerValue));
        }

        // GET: odata/OrderTravellers(5)/ServiceItemHistories
        [EnableQuery]
        public SingleResult<ServiceItemHistory> GetServiceItemHistories([FromODataUri] int key)
        {
            return SingleResult.Create(db.OrderTravellers.Where(m => m.OrderTravellerID == key).Select(m => m.ServiceItemHistories));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool OrderTravellerExists(int key)
        {
            return db.OrderTravellers.Count(e => e.OrderTravellerID == key) > 0;
        }
    }
}
