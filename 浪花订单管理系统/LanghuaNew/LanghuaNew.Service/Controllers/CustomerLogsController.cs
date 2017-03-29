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
    builder.EntitySet<CustomerLog>("CustomerLogs");
    builder.EntitySet<Customer>("Customers"); 
    config.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
    public class CustomerLogsController : ODataController
    {
        private LanghuaContent db = new LanghuaContent();

        // GET: odata/CustomerLogs
        [EnableQuery]
        public IQueryable<CustomerLog> GetCustomerLogs()
        {
            return db.CustomerLogs;
        }

        // GET: odata/CustomerLogs(5)
        [EnableQuery]
        public SingleResult<CustomerLog> GetCustomerLog([FromODataUri] int key)
        {
            return SingleResult.Create(db.CustomerLogs.Where(customerLog => customerLog.CustomerLogID == key));
        }

        // PUT: odata/CustomerLogs(5)
        public async Task<IHttpActionResult> Put([FromODataUri] int key, Delta<CustomerLog> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            CustomerLog customerLog = await db.CustomerLogs.FindAsync(key);
            if (customerLog == null)
            {
                return NotFound();
            }

            patch.Put(customerLog);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomerLogExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(customerLog);
        }

        // POST: odata/CustomerLogs
        public async Task<IHttpActionResult> Post(CustomerLog customerLog)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.CustomerLogs.Add(customerLog);
            await db.SaveChangesAsync();

            return Created(customerLog);
        }

        // PATCH: odata/CustomerLogs(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public async Task<IHttpActionResult> Patch([FromODataUri] int key, Delta<CustomerLog> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            CustomerLog customerLog = await db.CustomerLogs.FindAsync(key);
            if (customerLog == null)
            {
                return NotFound();
            }

            patch.Patch(customerLog);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomerLogExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(customerLog);
        }

        // DELETE: odata/CustomerLogs(5)
        public async Task<IHttpActionResult> Delete([FromODataUri] int key)
        {
            CustomerLog customerLog = await db.CustomerLogs.FindAsync(key);
            if (customerLog == null)
            {
                return NotFound();
            }

            db.CustomerLogs.Remove(customerLog);
            await db.SaveChangesAsync();

            return StatusCode(HttpStatusCode.NoContent);
        }

        // GET: odata/CustomerLogs(5)/customer
        [EnableQuery]
        public SingleResult<Customer> Getcustomer([FromODataUri] int key)
        {
            return SingleResult.Create(db.CustomerLogs.Where(m => m.CustomerLogID == key).Select(m => m.customer));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool CustomerLogExists(int key)
        {
            return db.CustomerLogs.Count(e => e.CustomerLogID == key) > 0;
        }
    }
}
