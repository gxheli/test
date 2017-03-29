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
    builder.EntitySet<CancelRegister>("CancelRegisters");
    builder.EntitySet<ServiceItem>("ServiceItems"); 
    builder.EntitySet<Supplier>("Suppliers"); 
    config.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
    public class CancelRegistersController : ODataController
    {
        private LanghuaContent db = new LanghuaContent();

        // GET: odata/CancelRegisters
        [EnableQuery]
        public IQueryable<CancelRegister> GetCancelRegisters()
        {
            return db.CancelRegisters;
        }

        // GET: odata/CancelRegisters(5)
        [EnableQuery]
        public SingleResult<CancelRegister> GetCancelRegister([FromODataUri] int key)
        {
            return SingleResult.Create(db.CancelRegisters.Where(cancelRegister => cancelRegister.CancelRegisterID == key));
        }

        // PUT: odata/CancelRegisters(5)
        public async Task<IHttpActionResult> Put([FromODataUri] int key, Delta<CancelRegister> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            CancelRegister cancelRegister = await db.CancelRegisters.FindAsync(key);
            if (cancelRegister == null)
            {
                return NotFound();
            }

            patch.Put(cancelRegister);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CancelRegisterExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(cancelRegister);
        }

        // POST: odata/CancelRegisters
        public async Task<IHttpActionResult> Post(CancelRegister cancelRegister)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.CancelRegisters.Add(cancelRegister);
            await db.SaveChangesAsync();

            return Created(cancelRegister);
        }

        // PATCH: odata/CancelRegisters(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public async Task<IHttpActionResult> Patch([FromODataUri] int key, Delta<CancelRegister> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            CancelRegister cancelRegister = await db.CancelRegisters.FindAsync(key);
            if (cancelRegister == null)
            {
                return NotFound();
            }

            patch.Patch(cancelRegister);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CancelRegisterExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(cancelRegister);
        }

        // DELETE: odata/CancelRegisters(5)
        public async Task<IHttpActionResult> Delete([FromODataUri] int key)
        {
            CancelRegister cancelRegister = await db.CancelRegisters.FindAsync(key);
            if (cancelRegister == null)
            {
                return NotFound();
            }

            db.CancelRegisters.Remove(cancelRegister);
            await db.SaveChangesAsync();

            return StatusCode(HttpStatusCode.NoContent);
        }

        // GET: odata/CancelRegisters(5)/serviceItem
        [EnableQuery]
        public SingleResult<ServiceItem> GetserviceItem([FromODataUri] int key)
        {
            return SingleResult.Create(db.CancelRegisters.Where(m => m.CancelRegisterID == key).Select(m => m.serviceItem));
        }

        // GET: odata/CancelRegisters(5)/supplier
        [EnableQuery]
        public SingleResult<Supplier> Getsupplier([FromODataUri] int key)
        {
            return SingleResult.Create(db.CancelRegisters.Where(m => m.CancelRegisterID == key).Select(m => m.supplier));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool CancelRegisterExists(int key)
        {
            return db.CancelRegisters.Count(e => e.CancelRegisterID == key) > 0;
        }
    }
}
