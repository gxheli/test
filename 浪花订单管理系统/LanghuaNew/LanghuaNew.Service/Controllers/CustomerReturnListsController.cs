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
    builder.EntitySet<CustomerReturnList>("CustomerReturnLists");
    builder.EntitySet<ServiceItem>("ServiceItems"); 
    builder.EntitySet<Supplier>("Suppliers"); 
    config.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
    public class CustomerReturnListsController : ODataController
    {
        private LanghuaContent db = new LanghuaContent();

        // GET: odata/CustomerReturnLists
        [EnableQuery]
        public IQueryable<CustomerReturnList> GetCustomerReturnLists()
        {
            return db.CustomerReturnLists;
        }

        // GET: odata/CustomerReturnLists(5)
        [EnableQuery]
        public SingleResult<CustomerReturnList> GetCustomerReturnList([FromODataUri] int key)
        {
            return SingleResult.Create(db.CustomerReturnLists.Where(customerReturnList => customerReturnList.CustomerReturnListID == key));
        }

        // PUT: odata/CustomerReturnLists(5)
        public async Task<IHttpActionResult> Put([FromODataUri] int key, Delta<CustomerReturnList> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            CustomerReturnList customerReturnList = await db.CustomerReturnLists.FindAsync(key);
            if (customerReturnList == null)
            {
                return NotFound();
            }

            patch.Put(customerReturnList);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomerReturnListExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(customerReturnList);
        }

        // POST: odata/CustomerReturnLists
        public async Task<IHttpActionResult> Post(CustomerReturnList customerReturnList)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.CustomerReturnLists.Add(customerReturnList);
            await db.SaveChangesAsync();

            return Created(customerReturnList);
        }

        // PATCH: odata/CustomerReturnLists(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public async Task<IHttpActionResult> Patch([FromODataUri] int key, Delta<CustomerReturnList> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            CustomerReturnList customerReturnList = await db.CustomerReturnLists.FindAsync(key);
            if (customerReturnList == null)
            {
                return NotFound();
            }

            patch.Patch(customerReturnList);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomerReturnListExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(customerReturnList);
        }

        // DELETE: odata/CustomerReturnLists(5)
        public async Task<IHttpActionResult> Delete([FromODataUri] int key)
        {
            CustomerReturnList customerReturnList = await db.CustomerReturnLists.FindAsync(key);
            if (customerReturnList == null)
            {
                return NotFound();
            }

            db.CustomerReturnLists.Remove(customerReturnList);
            await db.SaveChangesAsync();

            return StatusCode(HttpStatusCode.NoContent);
        }

        // GET: odata/CustomerReturnLists(5)/ReturnServiceItem
        [EnableQuery]
        public SingleResult<ServiceItem> GetReturnServiceItem([FromODataUri] int key)
        {
            return SingleResult.Create(db.CustomerReturnLists.Where(m => m.CustomerReturnListID == key).Select(m => m.ReturnServiceItem));
        }

        // GET: odata/CustomerReturnLists(5)/ReturnSupplier
        [EnableQuery]
        public SingleResult<Supplier> GetReturnSupplier([FromODataUri] int key)
        {
            return SingleResult.Create(db.CustomerReturnLists.Where(m => m.CustomerReturnListID == key).Select(m => m.ReturnSupplier));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool CustomerReturnListExists(int key)
        {
            return db.CustomerReturnLists.Count(e => e.CustomerReturnListID == key) > 0;
        }
    }
}
