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
    builder.EntitySet<CustomerBack>("CustomerBacks");
    builder.EntitySet<Customer>("Customers"); 
    config.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
    public class CustomerBacksController : ODataController
    {
        private LanghuaContent db = new LanghuaContent();

        // GET: odata/CustomerBacks
        [EnableQuery]
        public IQueryable<CustomerBack> GetCustomerBacks()
        {
            return db.CustomerBacks;
        }

        // GET: odata/CustomerBacks(5)
        [EnableQuery]
        public SingleResult<CustomerBack> GetCustomerBack([FromODataUri] int key)
        {
            return SingleResult.Create(db.CustomerBacks.Where(customerBack => customerBack.CustomerBackID == key));
        }

        // PUT: odata/CustomerBacks(5)
        public async Task<IHttpActionResult> Put([FromODataUri] int key, Delta<CustomerBack> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            CustomerBack customerBack = await db.CustomerBacks.FindAsync(key);
            if (customerBack == null)
            {
                return NotFound();
            }

            patch.Put(customerBack);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomerBackExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(customerBack);
        }

        // POST: odata/CustomerBacks
        public async Task<IHttpActionResult> Post(CustomerBack customerBack)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.CustomerBacks.Add(customerBack);
            await db.SaveChangesAsync();

            return Created(customerBack);
        }

        // PATCH: odata/CustomerBacks(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public async Task<IHttpActionResult> Patch([FromODataUri] int key, Delta<CustomerBack> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            CustomerBack customerBack = await db.CustomerBacks.FindAsync(key);
            if (customerBack == null)
            {
                return NotFound();
            }

            patch.Patch(customerBack);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomerBackExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(customerBack);
        }

        // DELETE: odata/CustomerBacks(5)
        public async Task<IHttpActionResult> Delete([FromODataUri] int key)
        {
            CustomerBack customerBack = await db.CustomerBacks.FindAsync(key);
            if (customerBack == null)
            {
                return NotFound();
            }

            db.CustomerBacks.Remove(customerBack);
            await db.SaveChangesAsync();

            return StatusCode(HttpStatusCode.NoContent);
        }

        // GET: odata/CustomerBacks(5)/Customer
        [EnableQuery]
        public SingleResult<Customer> GetCustomer([FromODataUri] int key)
        {
            return SingleResult.Create(db.CustomerBacks.Where(m => m.CustomerBackID == key).Select(m => m.Customer));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool CustomerBackExists(int key)
        {
            return db.CustomerBacks.Count(e => e.CustomerBackID == key) > 0;
        }
    }
}
