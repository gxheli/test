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
    builder.EntitySet<AlipayTransferLog>("AlipayTransferLogs");
    builder.EntitySet<AlipayTransfer>("AlipayTransfers"); 
    config.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
    public class AlipayTransferLogsController : ODataController
    {
        private LanghuaContent db = new LanghuaContent();

        // GET: odata/AlipayTransferLogs
        [EnableQuery]
        public IQueryable<AlipayTransferLog> GetAlipayTransferLogs()
        {
            return db.AlipayTransferLogs;
        }

        // GET: odata/AlipayTransferLogs(5)
        [EnableQuery]
        public SingleResult<AlipayTransferLog> GetAlipayTransferLog([FromODataUri] int key)
        {
            return SingleResult.Create(db.AlipayTransferLogs.Where(alipayTransferLog => alipayTransferLog.AlipayTransferLogID == key));
        }

        // PUT: odata/AlipayTransferLogs(5)
        public async Task<IHttpActionResult> Put([FromODataUri] int key, Delta<AlipayTransferLog> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            AlipayTransferLog alipayTransferLog = await db.AlipayTransferLogs.FindAsync(key);
            if (alipayTransferLog == null)
            {
                return NotFound();
            }

            patch.Put(alipayTransferLog);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AlipayTransferLogExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(alipayTransferLog);
        }

        // POST: odata/AlipayTransferLogs
        public async Task<IHttpActionResult> Post(AlipayTransferLog alipayTransferLog)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.AlipayTransferLogs.Add(alipayTransferLog);
            await db.SaveChangesAsync();

            return Created(alipayTransferLog);
        }

        // PATCH: odata/AlipayTransferLogs(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public async Task<IHttpActionResult> Patch([FromODataUri] int key, Delta<AlipayTransferLog> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            AlipayTransferLog alipayTransferLog = await db.AlipayTransferLogs.FindAsync(key);
            if (alipayTransferLog == null)
            {
                return NotFound();
            }

            patch.Patch(alipayTransferLog);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AlipayTransferLogExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(alipayTransferLog);
        }

        // DELETE: odata/AlipayTransferLogs(5)
        public async Task<IHttpActionResult> Delete([FromODataUri] int key)
        {
            AlipayTransferLog alipayTransferLog = await db.AlipayTransferLogs.FindAsync(key);
            if (alipayTransferLog == null)
            {
                return NotFound();
            }

            db.AlipayTransferLogs.Remove(alipayTransferLog);
            await db.SaveChangesAsync();

            return StatusCode(HttpStatusCode.NoContent);
        }

        // GET: odata/AlipayTransferLogs(5)/AlipayTransferItem
        [EnableQuery]
        public SingleResult<AlipayTransfer> GetAlipayTransferItem([FromODataUri] int key)
        {
            return SingleResult.Create(db.AlipayTransferLogs.Where(m => m.AlipayTransferLogID == key).Select(m => m.AlipayTransferItem));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool AlipayTransferLogExists(int key)
        {
            return db.AlipayTransferLogs.Count(e => e.AlipayTransferLogID == key) > 0;
        }
    }
}
