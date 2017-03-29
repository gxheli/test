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
    builder.EntitySet<FliterInfo>("FliterInfos");
    config.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
    public class FliterInfosController : ODataController
    {
        private LanghuaContent db = new LanghuaContent();

        // GET: odata/FliterInfos
        [EnableQuery]
        public IQueryable<FliterInfo> GetFliterInfos()
        {
            return db.FliterInfos;
        }

        // GET: odata/FliterInfos(5)
        [EnableQuery]
        public SingleResult<FliterInfo> GetFliterInfo([FromODataUri] int key)
        {
            return SingleResult.Create(db.FliterInfos.Where(fliterInfo => fliterInfo.ID == key));
        }

        // PUT: odata/FliterInfos(5)
        public async Task<IHttpActionResult> Put([FromODataUri] int key, Delta<FliterInfo> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            FliterInfo fliterInfo = await db.FliterInfos.FindAsync(key);
            if (fliterInfo == null)
            {
                return NotFound();
            }

            patch.Put(fliterInfo);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FliterInfoExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(fliterInfo);
        }

        // POST: odata/FliterInfos
        public async Task<IHttpActionResult> Post(FliterInfo fliterInfo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.FliterInfos.Add(fliterInfo);
            await db.SaveChangesAsync();

            return Created(fliterInfo);
        }

        // PATCH: odata/FliterInfos(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public async Task<IHttpActionResult> Patch([FromODataUri] int key, Delta<FliterInfo> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            FliterInfo fliterInfo = await db.FliterInfos.FindAsync(key);
            if (fliterInfo == null)
            {
                return NotFound();
            }

            patch.Patch(fliterInfo);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FliterInfoExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(fliterInfo);
        }

        // DELETE: odata/FliterInfos(5)
        public async Task<IHttpActionResult> Delete([FromODataUri] int key)
        {
            FliterInfo fliterInfo = await db.FliterInfos.FindAsync(key);
            if (fliterInfo == null)
            {
                return NotFound();
            }

            db.FliterInfos.Remove(fliterInfo);
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

        private bool FliterInfoExists(int key)
        {
            return db.FliterInfos.Count(e => e.ID == key) > 0;
        }
    }
}
