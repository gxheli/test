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
    builder.EntitySet<WorkTableDisplayItem>("WorkTableDisplayItems");
    builder.EntitySet<User>("Users"); 
    config.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
    public class WorkTableDisplayItemsController : ODataController
    {
        private LanghuaContent db = new LanghuaContent();

        // GET: odata/WorkTableDisplayItems
        [EnableQuery]
        public IQueryable<WorkTableDisplayItem> GetWorkTableDisplayItems()
        {
            return db.WorkTableDisplayItems;
        }

        // GET: odata/WorkTableDisplayItems(5)
        [EnableQuery]
        public SingleResult<WorkTableDisplayItem> GetWorkTableDisplayItem([FromODataUri] int key)
        {
            return SingleResult.Create(db.WorkTableDisplayItems.Where(workTableDisplayItem => workTableDisplayItem.WorkTableDisplayItemID == key));
        }

        // PUT: odata/WorkTableDisplayItems(5)
        public async Task<IHttpActionResult> Put([FromODataUri] int key, Delta<WorkTableDisplayItem> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            WorkTableDisplayItem workTableDisplayItem = await db.WorkTableDisplayItems.FindAsync(key);
            if (workTableDisplayItem == null)
            {
                return NotFound();
            }

            patch.Put(workTableDisplayItem);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!WorkTableDisplayItemExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(workTableDisplayItem);
        }

        // POST: odata/WorkTableDisplayItems
        public async Task<IHttpActionResult> Post(WorkTableDisplayItem workTableDisplayItem)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.WorkTableDisplayItems.Add(workTableDisplayItem);
            await db.SaveChangesAsync();

            return Created(workTableDisplayItem);
        }

        // PATCH: odata/WorkTableDisplayItems(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public async Task<IHttpActionResult> Patch([FromODataUri] int key, Delta<WorkTableDisplayItem> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            WorkTableDisplayItem workTableDisplayItem = await db.WorkTableDisplayItems.FindAsync(key);
            if (workTableDisplayItem == null)
            {
                return NotFound();
            }

            patch.Patch(workTableDisplayItem);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!WorkTableDisplayItemExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(workTableDisplayItem);
        }

        // DELETE: odata/WorkTableDisplayItems(5)
        public async Task<IHttpActionResult> Delete([FromODataUri] int key)
        {
            WorkTableDisplayItem workTableDisplayItem = await db.WorkTableDisplayItems.FindAsync(key);
            if (workTableDisplayItem == null)
            {
                return NotFound();
            }

            db.WorkTableDisplayItems.Remove(workTableDisplayItem);
            await db.SaveChangesAsync();

            return StatusCode(HttpStatusCode.NoContent);
        }

        // GET: odata/WorkTableDisplayItems(5)/users
        [EnableQuery]
        public SingleResult<User> Getusers([FromODataUri] int key)
        {
            return SingleResult.Create(db.WorkTableDisplayItems.Where(m => m.WorkTableDisplayItemID == key).Select(m => m.users));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool WorkTableDisplayItemExists(int key)
        {
            return db.WorkTableDisplayItems.Count(e => e.WorkTableDisplayItemID == key) > 0;
        }
    }
}
