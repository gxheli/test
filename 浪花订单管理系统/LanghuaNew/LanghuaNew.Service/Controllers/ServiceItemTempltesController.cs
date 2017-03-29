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
    builder.EntitySet<ServiceItemTemplte>("ServiceItemTempltes");
    config.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
    public class ServiceItemTempltesController : ODataController
    {
        private LanghuaContent db = new LanghuaContent();

        // GET: odata/ServiceItemTempltes
        [EnableQuery]
        public IQueryable<ServiceItemTemplte> GetServiceItemTempltes()
        {
            return db.ServiceItemTempltes;
        }

        // GET: odata/ServiceItemTempltes(5)
        [EnableQuery]
        public SingleResult<ServiceItemTemplte> GetServiceItemTemplte([FromODataUri] int key)
        {
            return SingleResult.Create(db.ServiceItemTempltes.Where(serviceItemTemplte => serviceItemTemplte.ServiceItemTemplteID == key));
        }

        // PUT: odata/ServiceItemTempltes(5)
        public async Task<IHttpActionResult> Put([FromODataUri] int key, Delta<ServiceItemTemplte> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            ServiceItemTemplte serviceItemTemplte = await db.ServiceItemTempltes.FindAsync(key);
            if (serviceItemTemplte == null)
            {
                return NotFound();
            }

            patch.Put(serviceItemTemplte);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ServiceItemTemplteExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(serviceItemTemplte);
        }

        // POST: odata/ServiceItemTempltes
        public async Task<IHttpActionResult> Post(ServiceItemTemplte serviceItemTemplte)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.ServiceItemTempltes.Add(serviceItemTemplte);
            await db.SaveChangesAsync();

            return Created(serviceItemTemplte);
        }

        // PATCH: odata/ServiceItemTempltes(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public async Task<IHttpActionResult> Patch([FromODataUri] int key, Delta<ServiceItemTemplte> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            ServiceItemTemplte serviceItemTemplte = await db.ServiceItemTempltes.FindAsync(key);
            if (serviceItemTemplte == null)
            {
                return NotFound();
            }

            patch.Patch(serviceItemTemplte);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ServiceItemTemplteExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(serviceItemTemplte);
        }

        // DELETE: odata/ServiceItemTempltes(5)
        public async Task<IHttpActionResult> Delete([FromODataUri] int key)
        {
            ServiceItemTemplte serviceItemTemplte = await db.ServiceItemTempltes.FindAsync(key);
            if (serviceItemTemplte == null)
            {
                return NotFound();
            }

            db.ServiceItemTempltes.Remove(serviceItemTemplte);
            await db.SaveChangesAsync();

            return StatusCode(HttpStatusCode.NoContent);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ServiceItemTemplteExists(int key)
        {
            return db.ServiceItemTempltes.Count(e => e.ServiceItemTemplteID == key) > 0;
        }
    }
}
