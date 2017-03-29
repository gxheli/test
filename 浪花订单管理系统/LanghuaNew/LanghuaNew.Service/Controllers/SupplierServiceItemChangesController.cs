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
    builder.EntitySet<SupplierServiceItemChange>("SupplierServiceItemChanges");
    builder.EntitySet<ExtraServicePriceChange>("ExtraServicePriceChanges"); 
    builder.EntitySet<Currency>("Currencys"); 
    builder.EntitySet<ItemPriceBySupplierChange>("ItemPriceBySupplierChanges"); 
    builder.EntitySet<Supplier>("Suppliers"); 
    builder.EntitySet<ServiceItem>("ServiceItems"); 
    config.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
    public class SupplierServiceItemChangesController : ODataController
    {
        private LanghuaContent db = new LanghuaContent();

        // GET: odata/SupplierServiceItemChanges
        [EnableQuery]
        public IQueryable<SupplierServiceItemChange> GetSupplierServiceItemChanges()
        {
            return db.SupplierServiceItemChanges;
        }

        // GET: odata/SupplierServiceItemChanges(5)
        [EnableQuery]
        public SingleResult<SupplierServiceItemChange> GetSupplierServiceItemChange([FromODataUri] int key)
        {
            return SingleResult.Create(db.SupplierServiceItemChanges.Where(supplierServiceItemChange => supplierServiceItemChange.SupplierServiceItemChangeID == key));
        }

        // PUT: odata/SupplierServiceItemChanges(5)
        public async Task<IHttpActionResult> Put([FromODataUri] int key, Delta<SupplierServiceItemChange> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            SupplierServiceItemChange supplierServiceItemChange = await db.SupplierServiceItemChanges.FindAsync(key);
            if (supplierServiceItemChange == null)
            {
                return NotFound();
            }

            patch.Put(supplierServiceItemChange);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SupplierServiceItemChangeExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(supplierServiceItemChange);
        }

        // POST: odata/SupplierServiceItemChanges
        public async Task<IHttpActionResult> Post(SupplierServiceItemChange supplierServiceItemChange)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.SupplierServiceItemChanges.Add(supplierServiceItemChange);
            await db.SaveChangesAsync();

            return Created(supplierServiceItemChange);
        }

        // PATCH: odata/SupplierServiceItemChanges(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public async Task<IHttpActionResult> Patch([FromODataUri] int key, Delta<SupplierServiceItemChange> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            SupplierServiceItemChange supplierServiceItemChange = await db.SupplierServiceItemChanges.FindAsync(key);
            if (supplierServiceItemChange == null)
            {
                return NotFound();
            }

            patch.Patch(supplierServiceItemChange);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SupplierServiceItemChangeExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(supplierServiceItemChange);
        }

        // DELETE: odata/SupplierServiceItemChanges(5)
        public async Task<IHttpActionResult> Delete([FromODataUri] int key)
        {
            SupplierServiceItemChange supplierServiceItemChange = await db.SupplierServiceItemChanges.FindAsync(key);
            if (supplierServiceItemChange == null)
            {
                return NotFound();
            }

            db.SupplierServiceItemChanges.Remove(supplierServiceItemChange);
            await db.SaveChangesAsync();

            return StatusCode(HttpStatusCode.NoContent);
        }

        // GET: odata/SupplierServiceItemChanges(5)/ExtraServicePrices
        [EnableQuery]
        public IQueryable<ExtraServicePriceChange> GetExtraServicePrices([FromODataUri] int key)
        {
            return db.SupplierServiceItemChanges.Where(m => m.SupplierServiceItemChangeID == key).SelectMany(m => m.ExtraServicePrices);
        }

        // GET: odata/SupplierServiceItemChanges(5)/ItemCurrency
        [EnableQuery]
        public SingleResult<Currency> GetItemCurrency([FromODataUri] int key)
        {
            return SingleResult.Create(db.SupplierServiceItemChanges.Where(m => m.SupplierServiceItemChangeID == key).Select(m => m.ItemCurrency));
        }

        // GET: odata/SupplierServiceItemChanges(5)/ItemPriceBySuppliers
        [EnableQuery]
        public IQueryable<ItemPriceBySupplierChange> GetItemPriceBySuppliers([FromODataUri] int key)
        {
            return db.SupplierServiceItemChanges.Where(m => m.SupplierServiceItemChangeID == key).SelectMany(m => m.ItemPriceBySuppliers);
        }

        // GET: odata/SupplierServiceItemChanges(5)/ItemSupplier
        [EnableQuery]
        public SingleResult<Supplier> GetItemSupplier([FromODataUri] int key)
        {
            return SingleResult.Create(db.SupplierServiceItemChanges.Where(m => m.SupplierServiceItemChangeID == key).Select(m => m.ItemSupplier));
        }

        // GET: odata/SupplierServiceItemChanges(5)/Service
        [EnableQuery]
        public SingleResult<ServiceItem> GetService([FromODataUri] int key)
        {
            return SingleResult.Create(db.SupplierServiceItemChanges.Where(m => m.SupplierServiceItemChangeID == key).Select(m => m.Service));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool SupplierServiceItemChangeExists(int key)
        {
            return db.SupplierServiceItemChanges.Count(e => e.SupplierServiceItemChangeID == key) > 0;
        }
    }
}
