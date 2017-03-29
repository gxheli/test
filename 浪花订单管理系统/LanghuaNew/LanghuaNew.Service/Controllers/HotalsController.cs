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
    builder.EntitySet<Hotal>("Hotals");
    builder.EntitySet<Area>("Areas"); 
    config.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
    public class HotalsController : ODataController
    {
        private LanghuaContent db = new LanghuaContent();

        // GET: odata/Hotals
        [EnableQuery]
        public IQueryable<Hotal> GetHotals()
        {
            return db.Hotals;
        }

        // GET: odata/Hotals(5)
        [EnableQuery]
        public SingleResult<Hotal> GetHotal([FromODataUri] int key)
        {
            return SingleResult.Create(db.Hotals.Where(hotal => hotal.HotalID == key));
        }

        // PUT: odata/Hotals(5)
        public async Task<IHttpActionResult> Put([FromODataUri] int key, Delta<Hotal> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Hotal hotal = await db.Hotals.FindAsync(key);
            if (hotal == null)
            {
                return NotFound();
            }

            patch.Put(hotal);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!HotalExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(hotal);
        }

        // POST: odata/Hotals
        public async Task<IHttpActionResult> Post(Hotal hotal)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Hotals.Add(hotal);
            await db.SaveChangesAsync();

            return Created(hotal);
        }

        // PATCH: odata/Hotals(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public async Task<IHttpActionResult> Patch([FromODataUri] int key, Delta<Hotal> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Hotal hotal = await db.Hotals.FindAsync(key);
            if (hotal == null)
            {
                return NotFound();
            }

            patch.Patch(hotal);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!HotalExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(hotal);
        }

        // DELETE: odata/Hotals(5)
        public async Task<IHttpActionResult> Delete([FromODataUri] int key)
        {
            Hotal hotal = await db.Hotals.FindAsync(key);
            if (hotal == null)
            {
                return NotFound();
            }

            db.Hotals.Remove(hotal);
            await db.SaveChangesAsync();

            return StatusCode(HttpStatusCode.NoContent);
        }

        // GET: odata/Hotals(5)/HotalArea
        [EnableQuery]
        public SingleResult<Area> GetHotalArea([FromODataUri] int key)
        {
            return SingleResult.Create(db.Hotals.Where(m => m.HotalID == key).Select(m => m.HotalArea));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool HotalExists(int key)
        {
            return db.Hotals.Count(e => e.HotalID == key) > 0;
        }
    }
}
