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
    builder.EntitySet<Area>("Areas");
    builder.EntitySet<City>("Cities"); 
    builder.EntitySet<Hotal>("Hotals"); 
    config.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
    public class AreasController : ODataController
    {
        private LanghuaContent db = new LanghuaContent();

        // GET: odata/Areas
        [EnableQuery]
        public IQueryable<Area> GetAreas()
        {
            return db.Areas;
        }

        // GET: odata/Areas(5)
        [EnableQuery]
        public SingleResult<Area> GetArea([FromODataUri] int key)
        {
            return SingleResult.Create(db.Areas.Where(area => area.AreaID == key));
        }

        // PUT: odata/Areas(5)
        public async Task<IHttpActionResult> Put([FromODataUri] int key, Delta<Area> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Area area = await db.Areas.FindAsync(key);
            if (area == null)
            {
                return NotFound();
            }

            patch.Put(area);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AreaExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(area);
        }

        // POST: odata/Areas
        public async Task<IHttpActionResult> Post(Area area)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Areas.Add(area);
            await db.SaveChangesAsync();

            return Created(area);
        }

        // PATCH: odata/Areas(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public async Task<IHttpActionResult> Patch([FromODataUri] int key, Delta<Area> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Area area = await db.Areas.FindAsync(key);
            if (area == null)
            {
                return NotFound();
            }

            patch.Patch(area);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AreaExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(area);
        }

        // DELETE: odata/Areas(5)
        public async Task<IHttpActionResult> Delete([FromODataUri] int key)
        {
            Area area = await db.Areas.FindAsync(key);
            if (area == null)
            {
                return NotFound();
            }

            db.Areas.Remove(area);
            await db.SaveChangesAsync();

            return StatusCode(HttpStatusCode.NoContent);
        }

        // GET: odata/Areas(5)/AreaCity
        [EnableQuery]
        public SingleResult<City> GetAreaCity([FromODataUri] int key)
        {
            return SingleResult.Create(db.Areas.Where(m => m.AreaID == key).Select(m => m.AreaCity));
        }

        // GET: odata/Areas(5)/Hotals
        [EnableQuery]
        public IQueryable<Hotal> GetHotals([FromODataUri] int key)
        {
            return db.Areas.Where(m => m.AreaID == key).SelectMany(m => m.Hotals);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool AreaExists(int key)
        {
            return db.Areas.Count(e => e.AreaID == key) > 0;
        }
    }
}
