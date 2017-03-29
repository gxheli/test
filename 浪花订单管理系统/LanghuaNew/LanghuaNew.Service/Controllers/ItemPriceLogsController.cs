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
    builder.EntitySet<ItemPriceLog>("ItemPriceLogs");
    builder.EntitySet<SupplierServiceItem>("SupplierServiceItems"); 
    config.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
    public class ItemPriceLogsController : ODataController
    {
        private LanghuaContent db = new LanghuaContent();

        // GET: odata/ItemPriceLogs
        [EnableQuery]
        public IQueryable<ItemPriceLog> GetItemPriceLogs()
        {
            return db.ItemPriceLogs;
        }

        // GET: odata/ItemPriceLogs(5)
        [EnableQuery]
        public SingleResult<ItemPriceLog> GetItemPriceLog([FromODataUri] int key)
        {
            return SingleResult.Create(db.ItemPriceLogs.Where(itemPriceLog => itemPriceLog.ItemPriceLogID == key));
        }

        // PUT: odata/ItemPriceLogs(5)
        public async Task<IHttpActionResult> Put([FromODataUri] int key, Delta<ItemPriceLog> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            ItemPriceLog itemPriceLog = await db.ItemPriceLogs.FindAsync(key);
            if (itemPriceLog == null)
            {
                return NotFound();
            }

            patch.Put(itemPriceLog);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ItemPriceLogExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(itemPriceLog);
        }

        // POST: odata/ItemPriceLogs
        public async Task<IHttpActionResult> Post(ItemPriceLog itemPriceLog)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.ItemPriceLogs.Add(itemPriceLog);
            await db.SaveChangesAsync();

            return Created(itemPriceLog);
        }

        // PATCH: odata/ItemPriceLogs(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public async Task<IHttpActionResult> Patch([FromODataUri] int key, Delta<ItemPriceLog> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            ItemPriceLog itemPriceLog = await db.ItemPriceLogs.FindAsync(key);
            if (itemPriceLog == null)
            {
                return NotFound();
            }

            patch.Patch(itemPriceLog);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ItemPriceLogExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(itemPriceLog);
        }

        // DELETE: odata/ItemPriceLogs(5)
        public async Task<IHttpActionResult> Delete([FromODataUri] int key)
        {
            ItemPriceLog itemPriceLog = await db.ItemPriceLogs.FindAsync(key);
            if (itemPriceLog == null)
            {
                return NotFound();
            }

            db.ItemPriceLogs.Remove(itemPriceLog);
            await db.SaveChangesAsync();

            return StatusCode(HttpStatusCode.NoContent);
        }

        // GET: odata/ItemPriceLogs(5)/ServiceItem
        [EnableQuery]
        public SingleResult<SupplierServiceItem> GetServiceItem([FromODataUri] int key)
        {
            return SingleResult.Create(db.ItemPriceLogs.Where(m => m.ItemPriceLogID == key).Select(m => m.ServiceItem));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ItemPriceLogExists(int key)
        {
            return db.ItemPriceLogs.Count(e => e.ItemPriceLogID == key) > 0;
        }
    }
}
