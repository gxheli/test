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
    builder.EntitySet<Customer>("Customers");
    builder.EntitySet<Order>("Orders"); 
    builder.EntitySet<Traveller>("Travellers"); 
    config.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
    public class CustomersController : ODataController
    {
        private LanghuaContent db = new LanghuaContent();

        // GET: odata/Customers
        [EnableQuery]
        public IQueryable<Customer> GetCustomers()
        {
            return db.Customers;
        }

        // GET: odata/Customers(5)
        [EnableQuery]
        public SingleResult<Customer> GetCustomer([FromODataUri] int key)
        {
            return SingleResult.Create(db.Customers.Where(customer => customer.CustomerID == key));
        }

        // PUT: odata/Customers(5)
        public async Task<IHttpActionResult> Put([FromODataUri] int key, Delta<Customer> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Customer customer = await db.Customers.FindAsync(key);
            if (customer == null)
            {
                return NotFound();
            }

            patch.Put(customer);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomerExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(customer);
        }

        // POST: odata/Customers
        public async Task<IHttpActionResult> Post(Customer customer)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Customers.Add(customer);
            await db.SaveChangesAsync();

            return Created(customer);
        }

        // PATCH: odata/Customers(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public async Task<IHttpActionResult> Patch([FromODataUri] int key, Delta<Customer> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Customer customer = await db.Customers.FindAsync(key);
            if (customer == null)
            {
                return NotFound();
            }

            patch.Patch(customer);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomerExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(customer);
        }

        // DELETE: odata/Customers(5)
        public async Task<IHttpActionResult> Delete([FromODataUri] int key)
        {
            Customer customer = await db.Customers.FindAsync(key);
            if (customer == null)
            {
                return NotFound();
            }

            db.Customers.Remove(customer);
            await db.SaveChangesAsync();

            return StatusCode(HttpStatusCode.NoContent);
        }

        // GET: odata/Customers(5)/Orders
        [EnableQuery]
        public IQueryable<Order> GetOrders([FromODataUri] int key)
        {
            return db.Customers.Where(m => m.CustomerID == key).SelectMany(m => m.Orders);
        }

        // GET: odata/Customers(5)/Travellers
        [EnableQuery]
        public IQueryable<Traveller> GetTravellers([FromODataUri] int key)
        {
            return db.Customers.Where(m => m.CustomerID == key).SelectMany(m => m.Travellers);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool CustomerExists(int key)
        {
            return db.Customers.Count(e => e.CustomerID == key) > 0;
        }
    }
}
