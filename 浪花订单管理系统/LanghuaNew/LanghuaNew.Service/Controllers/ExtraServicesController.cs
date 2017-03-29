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
    builder.EntitySet<ExtraService>("ExtraServices");
    config.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
    public class ExtraServicesController : ODataController
    {
        private LanghuaContent db = new LanghuaContent();

        // GET: odata/ExtraServices
        [EnableQuery]
        public IQueryable<ExtraService> GetExtraServices()
        {
            return db.ExtraServices;
        }

        // GET: odata/ExtraServices(5)
        [EnableQuery]
        public SingleResult<ExtraService> GetExtraService([FromODataUri] int key)
        {
            return SingleResult.Create(db.ExtraServices.Where(extraService => extraService.ExtraServiceID == key));
        }

        // PUT: odata/ExtraServices(5)
        public async Task<IHttpActionResult> Put([FromODataUri] int key, Delta<ExtraService> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            ExtraService extraService = await db.ExtraServices.FindAsync(key);
            if (extraService == null)
            {
                return NotFound();
            }

            patch.Put(extraService);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ExtraServiceExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(extraService);
        }

        // POST: odata/ExtraServices
        public async Task<IHttpActionResult> Post(ExtraService extraService)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.ExtraServices.Add(extraService);
            await db.SaveChangesAsync();

            return Created(extraService);
        }

        // PATCH: odata/ExtraServices(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public async Task<IHttpActionResult> Patch([FromODataUri] int key, Delta<ExtraService> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            ExtraService extraService = await db.ExtraServices.FindAsync(key);
            if (extraService == null)
            {
                return NotFound();
            }

            patch.Patch(extraService);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ExtraServiceExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(extraService);
        }

        // DELETE: odata/ExtraServices(5)
        public async Task<IHttpActionResult> Delete([FromODataUri] int key)
        {
            ExtraService extraService = await db.ExtraServices.FindAsync(key);
            if (extraService == null)
            {
                return NotFound();
            }

            db.ExtraServices.Remove(extraService);
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

        private bool ExtraServiceExists(int key)
        {
            return db.ExtraServices.Count(e => e.ExtraServiceID == key) > 0;
        }
    }
}
