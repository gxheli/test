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
    builder.EntitySet<ServiceItem>("ServiceItems");
    builder.EntitySet<City>("Cities"); 
    builder.EntitySet<ExtraService>("ExtraServices"); 
    builder.EntitySet<SellControl>("SellControls"); 
    builder.EntitySet<ServiceType>("ServiceTypes"); 
    builder.EntitySet<Supplier>("Suppliers"); 
    builder.EntitySet<ServiceItemTemplte>("ServiceItemTempltes"); 
    builder.EntitySet<ServiceRule>("ServiceRules"); 
    config.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
    public class ServiceItemsController : ODataController
    {
        private LanghuaContent db = new LanghuaContent();

        // GET: odata/ServiceItems
        [EnableQuery]
        public IQueryable<ServiceItem> GetServiceItems()
        {
            return db.ServiceItems;
        }

        // GET: odata/ServiceItems(5)
        [EnableQuery]
        public SingleResult<ServiceItem> GetServiceItem([FromODataUri] int key)
        {
            return SingleResult.Create(db.ServiceItems.Where(serviceItem => serviceItem.ServiceItemID == key));
        }

        // PUT: odata/ServiceItems(5)
        public IHttpActionResult Put([FromODataUri] int key, Delta<ServiceItem> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            ServiceItem serviceItem = db.ServiceItems.Find(key);
            if (serviceItem == null)
            {
                return NotFound();
            }

            patch.Put(serviceItem);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ServiceItemExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(serviceItem);
        }

        // POST: odata/ServiceItems
        public IHttpActionResult Post(ServiceItem serviceItem)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.ServiceItems.Add(serviceItem);
            db.SaveChanges();

            return Created(serviceItem);
        }

        // PATCH: odata/ServiceItems(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public IHttpActionResult Patch([FromODataUri] int key, Delta<ServiceItem> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            ServiceItem serviceItem = db.ServiceItems.Find(key);
            if (serviceItem == null)
            {
                return NotFound();
            }

            patch.Patch(serviceItem);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ServiceItemExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(serviceItem);
        }

        // DELETE: odata/ServiceItems(5)
        public IHttpActionResult Delete([FromODataUri] int key)
        {
            ServiceItem serviceItem = db.ServiceItems.Find(key);
            if (serviceItem == null)
            {
                return NotFound();
            }

            db.ServiceItems.Remove(serviceItem);
            db.SaveChanges();

            return StatusCode(HttpStatusCode.NoContent);
        }

        // GET: odata/ServiceItems(5)/city
        [EnableQuery]
        public SingleResult<City> Getcity([FromODataUri] int key)
        {
            return SingleResult.Create(db.ServiceItems.Where(m => m.ServiceItemID == key).Select(m => m.city));
        }

        // GET: odata/ServiceItems(5)/ExtraServices
        [EnableQuery]
        public IQueryable<ExtraService> GetExtraServices([FromODataUri] int key)
        {
            return db.ServiceItems.Where(m => m.ServiceItemID == key).SelectMany(m => m.ExtraServices);
        }

        // GET: odata/ServiceItems(5)/FirstSellControl
        [EnableQuery]
        public IQueryable<SellControl> GetFirstSellControl([FromODataUri] int key)
        {
            return db.ServiceItems.Where(m => m.ServiceItemID == key).SelectMany(m => m.FirstSellControl);
        }

        // GET: odata/ServiceItems(5)/ItemServiceType
        [EnableQuery]
        public SingleResult<ServiceType> GetItemServiceType([FromODataUri] int key)
        {
            return SingleResult.Create(db.ServiceItems.Where(m => m.ServiceItemID == key).Select(m => m.ItemServiceType));
        }

        // GET: odata/ServiceItems(5)/ItemSuplier
        [EnableQuery]
        public IQueryable<Supplier> GetItemSuplier([FromODataUri] int key)
        {
            return db.ServiceItems.Where(m => m.ServiceItemID == key).SelectMany(m => m.ItemSuplier);
        }

        // GET: odata/ServiceItems(5)/ItemTemplte
        [EnableQuery]
        public SingleResult<ServiceItemTemplte> GetItemTemplte([FromODataUri] int key)
        {
            return SingleResult.Create(db.ServiceItems.Where(m => m.ServiceItemID == key).Select(m => m.ItemTemplte));
        }

        // GET: odata/ServiceItems(5)/SecondSellControl
        [EnableQuery]
        public IQueryable<SellControl> GetSecondSellControl([FromODataUri] int key)
        {
            return db.ServiceItems.Where(m => m.ServiceItemID == key).SelectMany(m => m.SecondSellControl);
        }

        // GET: odata/ServiceItems(5)/ServiceRules
        [EnableQuery]
        public IQueryable<ServiceRule> GetServiceRules([FromODataUri] int key)
        {
            return db.ServiceItems.Where(m => m.ServiceItemID == key).SelectMany(m => m.ServiceRules);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ServiceItemExists(int key)
        {
            return db.ServiceItems.Count(e => e.ServiceItemID == key) > 0;
        }
    }
}
