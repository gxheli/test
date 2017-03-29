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
    builder.EntitySet<SellControl>("SellControls");
    builder.EntitySet<ServiceItem>("ServiceItems"); 
    builder.EntitySet<Supplier>("Suppliers"); 
    config.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
    public class SellControlsController : ODataController
    {
        private LanghuaContent db = new LanghuaContent();

        // GET: odata/SellControls
        [EnableQuery]
        public IQueryable<SellControl> GetSellControls()
        {
            return db.SellControls;
        }

        // GET: odata/SellControls(5)
        [EnableQuery]
        public SingleResult<SellControl> GetSellControl([FromODataUri] int key)
        {
            return SingleResult.Create(db.SellControls.Where(sellControl => sellControl.SellControlID == key));
        }

        // PUT: odata/SellControls(5)
        public async Task<IHttpActionResult> Put([FromODataUri] int key, Delta<SellControl> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            SellControl sellControl = await db.SellControls.FindAsync(key);
            if (sellControl == null)
            {
                return NotFound();
            }

            patch.Put(sellControl);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SellControlExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(sellControl);
        }

        // POST: odata/SellControls
        public async Task<IHttpActionResult> Post(SellControl sellControl)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.SellControls.Add(sellControl);
            await db.SaveChangesAsync();

            return Created(sellControl);
        }

        // PATCH: odata/SellControls(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public async Task<IHttpActionResult> Patch([FromODataUri] int key, Delta<SellControl> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            SellControl sellControl = await db.SellControls.FindAsync(key);
            if (sellControl == null)
            {
                return NotFound();
            }

            patch.Patch(sellControl);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SellControlExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(sellControl);
        }

        // DELETE: odata/SellControls(5)
        public async Task<IHttpActionResult> Delete([FromODataUri] int key)
        {
            SellControl sellControl = await db.SellControls.FindAsync(key);
            if (sellControl == null)
            {
                return NotFound();
            }

            db.SellControls.Remove(sellControl);
            await db.SaveChangesAsync();

            return StatusCode(HttpStatusCode.NoContent);
        }

        // GET: odata/SellControls(5)/FirstServiceItem
        [EnableQuery]
        public SingleResult<ServiceItem> GetFirstServiceItem([FromODataUri] int key)
        {
            return SingleResult.Create(db.SellControls.Where(m => m.SellControlID == key).Select(m => m.FirstServiceItem));
        }

        // GET: odata/SellControls(5)/SecondServiceItem
        [EnableQuery]
        public SingleResult<ServiceItem> GetSecondServiceItem([FromODataUri] int key)
        {
            return SingleResult.Create(db.SellControls.Where(m => m.SellControlID == key).Select(m => m.SecondServiceItem));
        }

        // GET: odata/SellControls(5)/Supplier
        [EnableQuery]
        public SingleResult<Supplier> GetSupplier([FromODataUri] int key)
        {
            return SingleResult.Create(db.SellControls.Where(m => m.SellControlID == key).Select(m => m.Supplier));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool SellControlExists(int key)
        {
            return db.SellControls.Count(e => e.SellControlID == key) > 0;
        }
    }
}
