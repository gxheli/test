using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
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
    builder.EntitySet<SupplierServiceItem>("SupplierServiceItems");
    builder.EntitySet<ExtraServicePrice>("ExtraServicePrices"); 
    builder.EntitySet<Currency>("Currencys"); 
    builder.EntitySet<ItemPriceBySupplier>("ItemPriceBySuppliers"); 
    builder.EntitySet<ItemPriceLog>("ItemPriceLogs"); 
    builder.EntitySet<Supplier>("Suppliers"); 
    builder.EntitySet<ServiceItem>("ServiceItems"); 
    config.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
    public class SupplierServiceItemsController : ODataController
    {
        private LanghuaContent db = new LanghuaContent();

        // GET: odata/SupplierServiceItems
        [EnableQuery]
        public IQueryable<SupplierServiceItem> GetSupplierServiceItems()
        {
            return db.SupplierServiceItems;
        }

        // GET: odata/SupplierServiceItems(5)
        [EnableQuery]
        public SingleResult<SupplierServiceItem> GetSupplierServiceItem([FromODataUri] int key)
        {
            return SingleResult.Create(db.SupplierServiceItems.Where(supplierServiceItem => supplierServiceItem.SupplierServiceItemID == key));
        }

        // PUT: odata/SupplierServiceItems(5)
        public IHttpActionResult Put([FromODataUri] int key, Delta<SupplierServiceItem> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            SupplierServiceItem supplierServiceItem = db.SupplierServiceItems.Find(key);
            if (supplierServiceItem == null)
            {
                return NotFound();
            }

            patch.Put(supplierServiceItem);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SupplierServiceItemExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(supplierServiceItem);
        }

        // POST: odata/SupplierServiceItems
        public IHttpActionResult Post(SupplierServiceItem supplierServiceItem)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.SupplierServiceItems.Add(supplierServiceItem);
            db.SaveChanges();

            return Created(supplierServiceItem);
        }

        // PATCH: odata/SupplierServiceItems(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public IHttpActionResult Patch([FromODataUri] int key, Delta<SupplierServiceItem> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            SupplierServiceItem supplierServiceItem = db.SupplierServiceItems.Find(key);
            if (supplierServiceItem == null)
            {
                return NotFound();
            }

            patch.Patch(supplierServiceItem);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SupplierServiceItemExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(supplierServiceItem);
        }

        // DELETE: odata/SupplierServiceItems(5)
        public IHttpActionResult Delete([FromODataUri] int key)
        {
            SupplierServiceItem supplierServiceItem = db.SupplierServiceItems.Find(key);
            if (supplierServiceItem == null)
            {
                return NotFound();
            }

            db.SupplierServiceItems.Remove(supplierServiceItem);
            db.SaveChanges();

            return StatusCode(HttpStatusCode.NoContent);
        }

        // GET: odata/SupplierServiceItems(5)/ExtraServicePrices
        [EnableQuery]
        public IQueryable<ExtraServicePrice> GetExtraServicePrices([FromODataUri] int key)
        {
            return db.SupplierServiceItems.Where(m => m.SupplierServiceItemID == key).SelectMany(m => m.ExtraServicePrices);
        }

        // GET: odata/SupplierServiceItems(5)/ItemCurrency
        [EnableQuery]
        public SingleResult<Currency> GetItemCurrency([FromODataUri] int key)
        {
            return SingleResult.Create(db.SupplierServiceItems.Where(m => m.SupplierServiceItemID == key).Select(m => m.ItemCurrency));
        }

        // GET: odata/SupplierServiceItems(5)/ItemPriceBySuppliers
        [EnableQuery]
        public IQueryable<ItemPriceBySupplier> GetItemPriceBySuppliers([FromODataUri] int key)
        {
            return db.SupplierServiceItems.Where(m => m.SupplierServiceItemID == key).SelectMany(m => m.ItemPriceBySuppliers);
        }

        // GET: odata/SupplierServiceItems(5)/ItemPriceLogs
        [EnableQuery]
        public IQueryable<ItemPriceLog> GetItemPriceLogs([FromODataUri] int key)
        {
            return db.SupplierServiceItems.Where(m => m.SupplierServiceItemID == key).SelectMany(m => m.ItemPriceLogs);
        }

        // GET: odata/SupplierServiceItems(5)/ItemSupplier
        [EnableQuery]
        public SingleResult<Supplier> GetItemSupplier([FromODataUri] int key)
        {
            return SingleResult.Create(db.SupplierServiceItems.Where(m => m.SupplierServiceItemID == key).Select(m => m.ItemSupplier));
        }

        // GET: odata/SupplierServiceItems(5)/Service
        [EnableQuery]
        public SingleResult<ServiceItem> GetService([FromODataUri] int key)
        {
            return SingleResult.Create(db.SupplierServiceItems.Where(m => m.SupplierServiceItemID == key).Select(m => m.Service));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool SupplierServiceItemExists(int key)
        {
            return db.SupplierServiceItems.Count(e => e.SupplierServiceItemID == key) > 0;
        }
    }
}
