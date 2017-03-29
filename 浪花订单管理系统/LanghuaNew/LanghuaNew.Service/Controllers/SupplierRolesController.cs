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
    builder.EntitySet<SupplierRole>("SupplierRoles");
    builder.EntitySet<Supplier>("Suppliers"); 
    builder.EntitySet<SupplierRoleRight>("SupplierRoleRights"); 
    builder.EntitySet<SupplierUser>("SupplierUsers"); 
    config.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
    public class SupplierRolesController : ODataController
    {
        private LanghuaContent db = new LanghuaContent();

        // GET: odata/SupplierRoles
        [EnableQuery]
        public IQueryable<SupplierRole> GetSupplierRoles()
        {
            return db.SupplierRoles;
        }

        // GET: odata/SupplierRoles(5)
        [EnableQuery]
        public SingleResult<SupplierRole> GetSupplierRole([FromODataUri] int key)
        {
            return SingleResult.Create(db.SupplierRoles.Where(supplierRole => supplierRole.SupplierRoleID == key));
        }

        // PUT: odata/SupplierRoles(5)
        public async Task<IHttpActionResult> Put([FromODataUri] int key, Delta<SupplierRole> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            SupplierRole supplierRole = await db.SupplierRoles.FindAsync(key);
            if (supplierRole == null)
            {
                return NotFound();
            }

            patch.Put(supplierRole);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SupplierRoleExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(supplierRole);
        }

        // POST: odata/SupplierRoles
        public async Task<IHttpActionResult> Post(SupplierRole supplierRole)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.SupplierRoles.Add(supplierRole);
            await db.SaveChangesAsync();

            return Created(supplierRole);
        }

        // PATCH: odata/SupplierRoles(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public async Task<IHttpActionResult> Patch([FromODataUri] int key, Delta<SupplierRole> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            SupplierRole supplierRole = await db.SupplierRoles.FindAsync(key);
            if (supplierRole == null)
            {
                return NotFound();
            }

            patch.Patch(supplierRole);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SupplierRoleExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(supplierRole);
        }

        // DELETE: odata/SupplierRoles(5)
        public async Task<IHttpActionResult> Delete([FromODataUri] int key)
        {
            SupplierRole supplierRole = await db.SupplierRoles.FindAsync(key);
            if (supplierRole == null)
            {
                return NotFound();
            }

            db.SupplierRoles.Remove(supplierRole);
            await db.SaveChangesAsync();

            return StatusCode(HttpStatusCode.NoContent);
        }

        // GET: odata/SupplierRoles(5)/oneSupplier
        [EnableQuery]
        public SingleResult<Supplier> GetoneSupplier([FromODataUri] int key)
        {
            return SingleResult.Create(db.SupplierRoles.Where(m => m.SupplierRoleID == key).Select(m => m.oneSupplier));
        }

        // GET: odata/SupplierRoles(5)/Rights
        [EnableQuery]
        public IQueryable<SupplierRoleRight> GetRights([FromODataUri] int key)
        {
            return db.SupplierRoles.Where(m => m.SupplierRoleID == key).SelectMany(m => m.Rights);
        }

        // GET: odata/SupplierRoles(5)/SupplierUsers
        [EnableQuery]
        public IQueryable<SupplierUser> GetSupplierUsers([FromODataUri] int key)
        {
            return db.SupplierRoles.Where(m => m.SupplierRoleID == key).SelectMany(m => m.SupplierUsers);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool SupplierRoleExists(int key)
        {
            return db.SupplierRoles.Count(e => e.SupplierRoleID == key) > 0;
        }
    }
}
