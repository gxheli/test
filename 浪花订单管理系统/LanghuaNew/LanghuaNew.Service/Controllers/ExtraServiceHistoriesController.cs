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
    builder.EntitySet<ExtraServiceHistory>("ExtraServiceHistories");
    config.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
    public class ExtraServiceHistoriesController : ODataController
    {
        private LanghuaContent db = new LanghuaContent();

        // GET: odata/ExtraServiceHistories
        [EnableQuery]
        public IQueryable<ExtraServiceHistory> GetExtraServiceHistories()
        {
            return db.ExtraServiceHistories;
        }

        // GET: odata/ExtraServiceHistories(5)
        [EnableQuery]
        public SingleResult<ExtraServiceHistory> GetExtraServiceHistory([FromODataUri] int key)
        {
            return SingleResult.Create(db.ExtraServiceHistories.Where(extraServiceHistory => extraServiceHistory.ExtraServiceHistoryID == key));
        }

        // PUT: odata/ExtraServiceHistories(5)
        public async Task<IHttpActionResult> Put([FromODataUri] int key, Delta<ExtraServiceHistory> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            ExtraServiceHistory extraServiceHistory = await db.ExtraServiceHistories.FindAsync(key);
            if (extraServiceHistory == null)
            {
                return NotFound();
            }

            patch.Put(extraServiceHistory);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ExtraServiceHistoryExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(extraServiceHistory);
        }

        // POST: odata/ExtraServiceHistories
        public async Task<IHttpActionResult> Post(ExtraServiceHistory extraServiceHistory)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.ExtraServiceHistories.Add(extraServiceHistory);
            await db.SaveChangesAsync();

            return Created(extraServiceHistory);
        }

        // PATCH: odata/ExtraServiceHistories(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public async Task<IHttpActionResult> Patch([FromODataUri] int key, Delta<ExtraServiceHistory> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            ExtraServiceHistory extraServiceHistory = await db.ExtraServiceHistories.FindAsync(key);
            if (extraServiceHistory == null)
            {
                return NotFound();
            }

            patch.Patch(extraServiceHistory);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ExtraServiceHistoryExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(extraServiceHistory);
        }

        // DELETE: odata/ExtraServiceHistories(5)
        public async Task<IHttpActionResult> Delete([FromODataUri] int key)
        {
            ExtraServiceHistory extraServiceHistory = await db.ExtraServiceHistories.FindAsync(key);
            if (extraServiceHistory == null)
            {
                return NotFound();
            }

            db.ExtraServiceHistories.Remove(extraServiceHistory);
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

        private bool ExtraServiceHistoryExists(int key)
        {
            return db.ExtraServiceHistories.Count(e => e.ExtraServiceHistoryID == key) > 0;
        }
    }
}
