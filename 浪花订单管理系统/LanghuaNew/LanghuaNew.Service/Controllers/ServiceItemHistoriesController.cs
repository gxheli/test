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
    builder.EntitySet<ServiceItemHistory>("ServiceItemHistories");
    builder.EntitySet<ExtraServiceHistory>("ExtraServiceHistories"); 
    builder.EntitySet<ServiceItemTemplte>("ServiceItemTempltes"); 
    builder.EntitySet<Order>("Orders"); 
    builder.EntitySet<Traveller>("Travellers"); 
    config.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
    public class ServiceItemHistoriesController : ODataController
    {
        private LanghuaContent db = new LanghuaContent();

        // GET: odata/ServiceItemHistories
        [EnableQuery]
        public IQueryable<ServiceItemHistory> GetServiceItemHistories()
        {
            return db.ServiceItemHistories;
        }

        // GET: odata/ServiceItemHistories(5)
        [EnableQuery]
        public SingleResult<ServiceItemHistory> GetServiceItemHistory([FromODataUri] int key)
        {
            return SingleResult.Create(db.ServiceItemHistories.Where(serviceItemHistory => serviceItemHistory.OrderID == key));
        }

        // PUT: odata/ServiceItemHistories(5)
        public async Task<IHttpActionResult> Put([FromODataUri] int key, Delta<ServiceItemHistory> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            ServiceItemHistory serviceItemHistory = await db.ServiceItemHistories.FindAsync(key);
            if (serviceItemHistory == null)
            {
                return NotFound();
            }

            patch.Put(serviceItemHistory);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ServiceItemHistoryExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(serviceItemHistory);
        }

        // POST: odata/ServiceItemHistories
        public async Task<IHttpActionResult> Post(ServiceItemHistory serviceItemHistory)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.ServiceItemHistories.Add(serviceItemHistory);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (ServiceItemHistoryExists(serviceItemHistory.OrderID))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return Created(serviceItemHistory);
        }

        // PATCH: odata/ServiceItemHistories(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public async Task<IHttpActionResult> Patch([FromODataUri] int key, Delta<ServiceItemHistory> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            ServiceItemHistory serviceItemHistory = await db.ServiceItemHistories.FindAsync(key);
            if (serviceItemHistory == null)
            {
                return NotFound();
            }

            patch.Patch(serviceItemHistory);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ServiceItemHistoryExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(serviceItemHistory);
        }

        // DELETE: odata/ServiceItemHistories(5)
        public async Task<IHttpActionResult> Delete([FromODataUri] int key)
        {
            ServiceItemHistory serviceItemHistory = await db.ServiceItemHistories.FindAsync(key);
            if (serviceItemHistory == null)
            {
                return NotFound();
            }

            db.ServiceItemHistories.Remove(serviceItemHistory);
            await db.SaveChangesAsync();

            return StatusCode(HttpStatusCode.NoContent);
        }

        // GET: odata/ServiceItemHistories(5)/ExtraServiceHistorys
        [EnableQuery]
        public IQueryable<ExtraServiceHistory> GetExtraServiceHistorys([FromODataUri] int key)
        {
            return db.ServiceItemHistories.Where(m => m.OrderID == key).SelectMany(m => m.ExtraServiceHistorys);
        }

        // GET: odata/ServiceItemHistories(5)/ItemTemplte
        [EnableQuery]
        public SingleResult<ServiceItemTemplte> GetItemTemplte([FromODataUri] int key)
        {
            return SingleResult.Create(db.ServiceItemHistories.Where(m => m.OrderID == key).Select(m => m.ItemTemplte));
        }

        // GET: odata/ServiceItemHistories(5)/Order
        [EnableQuery]
        public SingleResult<Order> GetOrder([FromODataUri] int key)
        {
            return SingleResult.Create(db.ServiceItemHistories.Where(m => m.OrderID == key).Select(m => m.Order));
        }

        // GET: odata/ServiceItemHistories(5)/travellers
        [EnableQuery]
        public IQueryable<OrderTraveller> Gettravellers([FromODataUri] int key)
        {
            return db.ServiceItemHistories.Where(m => m.OrderID == key).SelectMany(m => m.travellers);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ServiceItemHistoryExists(int key)
        {
            return db.ServiceItemHistories.Count(e => e.OrderID == key) > 0;
        }
    }
}
