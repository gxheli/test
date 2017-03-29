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
    builder.EntitySet<CancelRegisterLog>("CancelRegisterLogs");
    builder.EntitySet<CancelRegister>("CancelRegisters"); 
    config.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
    public class CancelRegisterLogsController : ODataController
    {
        private LanghuaContent db = new LanghuaContent();

        // GET: odata/CancelRegisterLogs
        [EnableQuery]
        public IQueryable<CancelRegisterLog> GetCancelRegisterLogs()
        {
            return db.CancelRegisterLogs;
        }

        // GET: odata/CancelRegisterLogs(5)
        [EnableQuery]
        public SingleResult<CancelRegisterLog> GetCancelRegisterLog([FromODataUri] int key)
        {
            return SingleResult.Create(db.CancelRegisterLogs.Where(cancelRegisterLog => cancelRegisterLog.CancelRegisterLogID == key));
        }

        // PUT: odata/CancelRegisterLogs(5)
        public async Task<IHttpActionResult> Put([FromODataUri] int key, Delta<CancelRegisterLog> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            CancelRegisterLog cancelRegisterLog = await db.CancelRegisterLogs.FindAsync(key);
            if (cancelRegisterLog == null)
            {
                return NotFound();
            }

            patch.Put(cancelRegisterLog);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CancelRegisterLogExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(cancelRegisterLog);
        }

        // POST: odata/CancelRegisterLogs
        public async Task<IHttpActionResult> Post(CancelRegisterLog cancelRegisterLog)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.CancelRegisterLogs.Add(cancelRegisterLog);
            await db.SaveChangesAsync();

            return Created(cancelRegisterLog);
        }

        // PATCH: odata/CancelRegisterLogs(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public async Task<IHttpActionResult> Patch([FromODataUri] int key, Delta<CancelRegisterLog> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            CancelRegisterLog cancelRegisterLog = await db.CancelRegisterLogs.FindAsync(key);
            if (cancelRegisterLog == null)
            {
                return NotFound();
            }

            patch.Patch(cancelRegisterLog);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CancelRegisterLogExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(cancelRegisterLog);
        }

        // DELETE: odata/CancelRegisterLogs(5)
        public async Task<IHttpActionResult> Delete([FromODataUri] int key)
        {
            CancelRegisterLog cancelRegisterLog = await db.CancelRegisterLogs.FindAsync(key);
            if (cancelRegisterLog == null)
            {
                return NotFound();
            }

            db.CancelRegisterLogs.Remove(cancelRegisterLog);
            await db.SaveChangesAsync();

            return StatusCode(HttpStatusCode.NoContent);
        }

        // GET: odata/CancelRegisterLogs(5)/cancelRegister
        [EnableQuery]
        public SingleResult<CancelRegister> GetcancelRegister([FromODataUri] int key)
        {
            return SingleResult.Create(db.CancelRegisterLogs.Where(m => m.CancelRegisterLogID == key).Select(m => m.cancelRegister));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool CancelRegisterLogExists(int key)
        {
            return db.CancelRegisterLogs.Count(e => e.CancelRegisterLogID == key) > 0;
        }
    }
}
