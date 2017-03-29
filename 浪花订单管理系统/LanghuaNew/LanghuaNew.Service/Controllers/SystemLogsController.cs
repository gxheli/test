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
    builder.EntitySet<SystemLog>("SystemLogs");
    config.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
    public class SystemLogsController : ODataController
    {
        private LanghuaContent db = new LanghuaContent();

        // GET: odata/SystemLogs
        [EnableQuery]
        public IQueryable<SystemLog> GetSystemLogs()
        {
            return db.SystemLogs;
        }

        // GET: odata/SystemLogs(5)
        [EnableQuery]
        public SingleResult<SystemLog> GetSystemLog([FromODataUri] int key)
        {
            return SingleResult.Create(db.SystemLogs.Where(systemLog => systemLog.SystemLogID == key));
        }

        // PUT: odata/SystemLogs(5)
        public async Task<IHttpActionResult> Put([FromODataUri] int key, Delta<SystemLog> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            SystemLog systemLog = await db.SystemLogs.FindAsync(key);
            if (systemLog == null)
            {
                return NotFound();
            }

            patch.Put(systemLog);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SystemLogExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(systemLog);
        }

        // POST: odata/SystemLogs
        public async Task<IHttpActionResult> Post(SystemLog systemLog)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.SystemLogs.Add(systemLog);
            await db.SaveChangesAsync();

            return Created(systemLog);
        }

        // PATCH: odata/SystemLogs(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public async Task<IHttpActionResult> Patch([FromODataUri] int key, Delta<SystemLog> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            SystemLog systemLog = await db.SystemLogs.FindAsync(key);
            if (systemLog == null)
            {
                return NotFound();
            }

            patch.Patch(systemLog);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SystemLogExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(systemLog);
        }

        // DELETE: odata/SystemLogs(5)
        public async Task<IHttpActionResult> Delete([FromODataUri] int key)
        {
            SystemLog systemLog = await db.SystemLogs.FindAsync(key);
            if (systemLog == null)
            {
                return NotFound();
            }

            db.SystemLogs.Remove(systemLog);
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

        private bool SystemLogExists(int key)
        {
            return db.SystemLogs.Count(e => e.SystemLogID == key) > 0;
        }
    }
}
