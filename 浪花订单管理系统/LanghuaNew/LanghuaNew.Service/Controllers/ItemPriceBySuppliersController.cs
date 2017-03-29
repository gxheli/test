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
    builder.EntitySet<ItemPriceBySupplier>("ItemPriceBySuppliers");
    builder.EntitySet<SupplierServiceItem>("SupplierServiceItems"); 
    config.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
    public class ItemPriceBySuppliersController : ODataController
    {
        private LanghuaContent db = new LanghuaContent();

        // GET: odata/ItemPriceBySuppliers
        [EnableQuery]
        public IQueryable<ItemPriceBySupplier> GetItemPriceBySuppliers()
        {
            return db.ItemPriceBySuppliers;
        }

        // GET: odata/ItemPriceBySuppliers(5)
        [EnableQuery]
        public SingleResult<ItemPriceBySupplier> GetItemPriceBySupplier([FromODataUri] int key)
        {
            return SingleResult.Create(db.ItemPriceBySuppliers.Where(itemPriceBySupplier => itemPriceBySupplier.ItemPriceBySupplierID == key));
        }

        // PUT: odata/ItemPriceBySuppliers(5)
        public async Task<IHttpActionResult> Put([FromODataUri] int key, Delta<ItemPriceBySupplier> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            ItemPriceBySupplier itemPriceBySupplier = await db.ItemPriceBySuppliers.FindAsync(key);
            if (itemPriceBySupplier == null)
            {
                return NotFound();
            }

            patch.Put(itemPriceBySupplier);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ItemPriceBySupplierExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(itemPriceBySupplier);
        }

        // POST: odata/ItemPriceBySuppliers
        public async Task<IHttpActionResult> Post(ItemPriceBySupplier itemPriceBySupplier)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.ItemPriceBySuppliers.Add(itemPriceBySupplier);
            await db.SaveChangesAsync();

            return Created(itemPriceBySupplier);
        }

        // PATCH: odata/ItemPriceBySuppliers(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public async Task<IHttpActionResult> Patch([FromODataUri] int key, Delta<ItemPriceBySupplier> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            ItemPriceBySupplier itemPriceBySupplier = await db.ItemPriceBySuppliers.FindAsync(key);
            if (itemPriceBySupplier == null)
            {
                return NotFound();
            }

            patch.Patch(itemPriceBySupplier);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ItemPriceBySupplierExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(itemPriceBySupplier);
        }

        // DELETE: odata/ItemPriceBySuppliers(5)
        public async Task<IHttpActionResult> Delete([FromODataUri] int key)
        {
            ItemPriceBySupplier itemPriceBySupplier = await db.ItemPriceBySuppliers.FindAsync(key);
            if (itemPriceBySupplier == null)
            {
                return NotFound();
            }

            db.ItemPriceBySuppliers.Remove(itemPriceBySupplier);
            await db.SaveChangesAsync();

            return StatusCode(HttpStatusCode.NoContent);
        }

        // GET: odata/ItemPriceBySuppliers(5)/ServiceItem
        [EnableQuery]
        public SingleResult<SupplierServiceItem> GetServiceItem([FromODataUri] int key)
        {
            return SingleResult.Create(db.ItemPriceBySuppliers.Where(m => m.ItemPriceBySupplierID == key).Select(m => m.ServiceItem));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ItemPriceBySupplierExists(int key)
        {
            return db.ItemPriceBySuppliers.Count(e => e.ItemPriceBySupplierID == key) > 0;
        }
    }
}
