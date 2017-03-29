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
    builder.EntitySet<SupplierRoleRight>("SupplierRoleRights");
    builder.EntitySet<SupplierRole>("SupplierRoles"); 
    config.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
    public class SupplierRoleRightsController : ODataController
    {
        private LanghuaContent db = new LanghuaContent();

        // GET: odata/SupplierRoleRights
        [EnableQuery]
        public IQueryable<SupplierRoleRight> GetSupplierRoleRights()
        {
            return db.SupplierRoleRights;
        }

        // GET: odata/SupplierRoleRights(5)
        [EnableQuery]
        public SingleResult<SupplierRoleRight> GetSupplierRoleRight([FromODataUri] int key)
        {
            return SingleResult.Create(db.SupplierRoleRights.Where(supplierRoleRight => supplierRoleRight.SupplierRoleRightID == key));
        }

        // PUT: odata/SupplierRoleRights(5)
        public async Task<IHttpActionResult> Put([FromODataUri] int key, Delta<SupplierRoleRight> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            SupplierRoleRight supplierRoleRight = await db.SupplierRoleRights.FindAsync(key);
            if (supplierRoleRight == null)
            {
                return NotFound();
            }

            patch.Put(supplierRoleRight);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SupplierRoleRightExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(supplierRoleRight);
        }

        // POST: odata/SupplierRoleRights
        public async Task<IHttpActionResult> Post(SupplierRoleRight supplierRoleRight)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.SupplierRoleRights.Add(supplierRoleRight);
            await db.SaveChangesAsync();

            return Created(supplierRoleRight);
        }

        // PATCH: odata/SupplierRoleRights(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public async Task<IHttpActionResult> Patch([FromODataUri] int key, Delta<SupplierRoleRight> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            SupplierRoleRight supplierRoleRight = await db.SupplierRoleRights.FindAsync(key);
            if (supplierRoleRight == null)
            {
                return NotFound();
            }

            patch.Patch(supplierRoleRight);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SupplierRoleRightExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(supplierRoleRight);
        }

        // DELETE: odata/SupplierRoleRights(5)
        public async Task<IHttpActionResult> Delete([FromODataUri] int key)
        {
            SupplierRoleRight supplierRoleRight = await db.SupplierRoleRights.FindAsync(key);
            if (supplierRoleRight == null)
            {
                return NotFound();
            }

            db.SupplierRoleRights.Remove(supplierRoleRight);
            await db.SaveChangesAsync();

            return StatusCode(HttpStatusCode.NoContent);
        }

        // GET: odata/SupplierRoleRights(5)/Roles
        [EnableQuery]
        public IQueryable<SupplierRole> GetRoles([FromODataUri] int key)
        {
            return db.SupplierRoleRights.Where(m => m.SupplierRoleRightID == key).SelectMany(m => m.Roles);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool SupplierRoleRightExists(int key)
        {
            return db.SupplierRoleRights.Count(e => e.SupplierRoleRightID == key) > 0;
        }
    }
}
