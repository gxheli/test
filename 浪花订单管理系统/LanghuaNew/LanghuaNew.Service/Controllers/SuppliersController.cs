﻿using System;
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
    builder.EntitySet<Supplier>("Suppliers");
    builder.EntitySet<ServiceItemHistory>("ServiceItemHistories"); 
    builder.EntitySet<ServiceItem>("ServiceItems"); 
    builder.EntitySet<Country>("Countrys"); 
    builder.EntitySet<SupplierRole>("SupplierRoles"); 
    config.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
    public class SuppliersController : ODataController
    {
        private LanghuaContent db = new LanghuaContent();

        // GET: odata/Suppliers
        [EnableQuery]
        public IQueryable<Supplier> GetSuppliers()
        {
            return db.Suppliers;
        }

        // GET: odata/Suppliers(5)
        [EnableQuery]
        public SingleResult<Supplier> GetSupplier([FromODataUri] int key)
        {
            return SingleResult.Create(db.Suppliers.Where(supplier => supplier.SupplierID == key));
        }

        // PUT: odata/Suppliers(5)
        public async Task<IHttpActionResult> Put([FromODataUri] int key, Delta<Supplier> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Supplier supplier = await db.Suppliers.FindAsync(key);
            if (supplier == null)
            {
                return NotFound();
            }

            patch.Put(supplier);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SupplierExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(supplier);
        }

        // POST: odata/Suppliers
        public async Task<IHttpActionResult> Post(Supplier supplier)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Suppliers.Add(supplier);
            await db.SaveChangesAsync();

            return Created(supplier);
        }

        // PATCH: odata/Suppliers(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public async Task<IHttpActionResult> Patch([FromODataUri] int key, Delta<Supplier> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Supplier supplier = await db.Suppliers.FindAsync(key);
            if (supplier == null)
            {
                return NotFound();
            }

            patch.Patch(supplier);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SupplierExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(supplier);
        }

        // DELETE: odata/Suppliers(5)
        public async Task<IHttpActionResult> Delete([FromODataUri] int key)
        {
            Supplier supplier = await db.Suppliers.FindAsync(key);
            if (supplier == null)
            {
                return NotFound();
            }

            db.Suppliers.Remove(supplier);
            await db.SaveChangesAsync();

            return StatusCode(HttpStatusCode.NoContent);
        }

        // GET: odata/Suppliers(5)/ServiceItemHistorys
        [EnableQuery]
        public IQueryable<ServiceItemHistory> GetServiceItemHistorys([FromODataUri] int key)
        {
            return db.Suppliers.Where(m => m.SupplierID == key).SelectMany(m => m.ServiceItemHistorys);
        }

        // GET: odata/Suppliers(5)/ServiceItems
        [EnableQuery]
        public IQueryable<ServiceItem> GetServiceItems([FromODataUri] int key)
        {
            return db.Suppliers.Where(m => m.SupplierID == key).SelectMany(m => m.ServiceItems);
        }

        // GET: odata/Suppliers(5)/SupplierCountry
        [EnableQuery]
        public SingleResult<Country> GetSupplierCountry([FromODataUri] int key)
        {
            return SingleResult.Create(db.Suppliers.Where(m => m.SupplierID == key).Select(m => m.SupplierCountry));
        }

        // GET: odata/Suppliers(5)/SupplierRoles
        [EnableQuery]
        public IQueryable<SupplierRole> GetSupplierRoles([FromODataUri] int key)
        {
            return db.Suppliers.Where(m => m.SupplierID == key).SelectMany(m => m.SupplierRoles);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool SupplierExists(int key)
        {
            return db.Suppliers.Count(e => e.SupplierID == key) > 0;
        }
    }
}
