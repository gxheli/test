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
    builder.EntitySet<AlipayTransfer>("AlipayTransfers");
    builder.EntitySet<AlipayTransferLog>("AlipayTransferLog"); 
    builder.EntitySet<OrderSourse>("OrderSourses"); 
    config.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
    public class AlipayTransfersController : ODataController
    {
        private LanghuaContent db = new LanghuaContent();

        // GET: odata/AlipayTransfers
        [EnableQuery]
        public IQueryable<AlipayTransfer> GetAlipayTransfers()
        {
            return db.AlipayTransfers;
        }

        // GET: odata/AlipayTransfers(5)
        [EnableQuery]
        public SingleResult<AlipayTransfer> GetAlipayTransfer([FromODataUri] int key)
        {
            return SingleResult.Create(db.AlipayTransfers.Where(alipayTransfer => alipayTransfer.AlipayTransferID == key));
        }

        // PUT: odata/AlipayTransfers(5)
        public async Task<IHttpActionResult> Put([FromODataUri] int key, Delta<AlipayTransfer> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            AlipayTransfer alipayTransfer = await db.AlipayTransfers.FindAsync(key);
            if (alipayTransfer == null)
            {
                return NotFound();
            }

            patch.Put(alipayTransfer);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AlipayTransferExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(alipayTransfer);
        }

        // POST: odata/AlipayTransfers
        public async Task<IHttpActionResult> Post(AlipayTransfer alipayTransfer)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.AlipayTransfers.Add(alipayTransfer);
            await db.SaveChangesAsync();

            return Created(alipayTransfer);
        }

        // PATCH: odata/AlipayTransfers(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public async Task<IHttpActionResult> Patch([FromODataUri] int key, Delta<AlipayTransfer> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            AlipayTransfer alipayTransfer = await db.AlipayTransfers.FindAsync(key);
            if (alipayTransfer == null)
            {
                return NotFound();
            }

            patch.Patch(alipayTransfer);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AlipayTransferExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(alipayTransfer);
        }

        // DELETE: odata/AlipayTransfers(5)
        public async Task<IHttpActionResult> Delete([FromODataUri] int key)
        {
            AlipayTransfer alipayTransfer = await db.AlipayTransfers.FindAsync(key);
            if (alipayTransfer == null)
            {
                return NotFound();
            }

            db.AlipayTransfers.Remove(alipayTransfer);
            await db.SaveChangesAsync();

            return StatusCode(HttpStatusCode.NoContent);
        }

        // GET: odata/AlipayTransfers(5)/Logs
        [EnableQuery]
        public IQueryable<AlipayTransferLog> GetLogs([FromODataUri] int key)
        {
            return db.AlipayTransfers.Where(m => m.AlipayTransferID == key).SelectMany(m => m.Logs);
        }

        // GET: odata/AlipayTransfers(5)/Sourse
        [EnableQuery]
        public SingleResult<OrderSourse> GetSourse([FromODataUri] int key)
        {
            return SingleResult.Create(db.AlipayTransfers.Where(m => m.AlipayTransferID == key).Select(m => m.Sourse));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool AlipayTransferExists(int key)
        {
            return db.AlipayTransfers.Count(e => e.AlipayTransferID == key) > 0;
        }
    }
}
