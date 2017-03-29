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
    builder.EntitySet<ServiceType>("ServiceTypes");
    builder.EntitySet<ServiceItem>("ServiceItems"); 
    config.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
    public class ServiceTypesController : ODataController
    {
        private LanghuaContent db = new LanghuaContent();

        // GET: odata/ServiceTypes
        [EnableQuery]
        public IQueryable<ServiceType> GetServiceTypes()
        {
            return db.ServiceTypes;
        }

        // GET: odata/ServiceTypes(5)
        [EnableQuery]
        public SingleResult<ServiceType> GetServiceType([FromODataUri] int key)
        {
            return SingleResult.Create(db.ServiceTypes.Where(serviceType => serviceType.ServiceTypeID == key));
        }

        // PUT: odata/ServiceTypes(5)
        public async Task<IHttpActionResult> Put([FromODataUri] int key, Delta<ServiceType> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            ServiceType serviceType = await db.ServiceTypes.FindAsync(key);
            if (serviceType == null)
            {
                return NotFound();
            }

            patch.Put(serviceType);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ServiceTypeExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(serviceType);
        }

        // POST: odata/ServiceTypes
        public async Task<IHttpActionResult> Post(ServiceType serviceType)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.ServiceTypes.Add(serviceType);
            await db.SaveChangesAsync();

            return Created(serviceType);
        }

        // PATCH: odata/ServiceTypes(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public async Task<IHttpActionResult> Patch([FromODataUri] int key, Delta<ServiceType> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            ServiceType serviceType = await db.ServiceTypes.FindAsync(key);
            if (serviceType == null)
            {
                return NotFound();
            }

            patch.Patch(serviceType);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ServiceTypeExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(serviceType);
        }

        // DELETE: odata/ServiceTypes(5)
        public async Task<IHttpActionResult> Delete([FromODataUri] int key)
        {
            ServiceType serviceType = await db.ServiceTypes.FindAsync(key);
            if (serviceType == null)
            {
                return NotFound();
            }

            db.ServiceTypes.Remove(serviceType);
            await db.SaveChangesAsync();

            return StatusCode(HttpStatusCode.NoContent);
        }

        // GET: odata/ServiceTypes(5)/ServiceItems
        [EnableQuery]
        public IQueryable<ServiceItem> GetServiceItems([FromODataUri] int key)
        {
            return db.ServiceTypes.Where(m => m.ServiceTypeID == key).SelectMany(m => m.ServiceItems);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ServiceTypeExists(int key)
        {
            return db.ServiceTypes.Count(e => e.ServiceTypeID == key) > 0;
        }
    }
}
