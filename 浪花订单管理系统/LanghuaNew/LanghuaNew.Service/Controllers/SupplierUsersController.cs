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
    builder.EntitySet<SupplierUser>("SupplierUsers");
    builder.EntitySet<Supplier>("Suppliers"); 
    builder.EntitySet<SupplierRole>("SupplierRoles"); 
    config.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
    public class SupplierUsersController : ODataController
    {
        private LanghuaContent db = new LanghuaContent();

        // GET: odata/SupplierUsers
        [EnableQuery]
        public IQueryable<SupplierUser> GetSupplierUsers()
        {
            return db.SupplierUsers;
        }

        // GET: odata/SupplierUsers(5)
        [EnableQuery]
        public SingleResult<SupplierUser> GetSupplierUser([FromODataUri] int key)
        {
            return SingleResult.Create(db.SupplierUsers.Where(supplierUser => supplierUser.SupplierUserID == key));
        }

        // PUT: odata/SupplierUsers(5)
        public async Task<IHttpActionResult> Put([FromODataUri] int key, Delta<SupplierUser> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            SupplierUser supplierUser = await db.SupplierUsers.FindAsync(key);
            if (supplierUser == null)
            {
                return NotFound();
            }

            patch.Put(supplierUser);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SupplierUserExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(supplierUser);
        }

        // POST: odata/SupplierUsers
        public async Task<IHttpActionResult> Post(SupplierUser supplierUser)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.SupplierUsers.Add(supplierUser);
            await db.SaveChangesAsync();

            return Created(supplierUser);
        }

        // PATCH: odata/SupplierUsers(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public async Task<IHttpActionResult> Patch([FromODataUri] int key, Delta<SupplierUser> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            SupplierUser supplierUser = await db.SupplierUsers.FindAsync(key);
            if (supplierUser == null)
            {
                return NotFound();
            }

            patch.Patch(supplierUser);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SupplierUserExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(supplierUser);
        }

        // DELETE: odata/SupplierUsers(5)
        public async Task<IHttpActionResult> Delete([FromODataUri] int key)
        {
            SupplierUser supplierUser = await db.SupplierUsers.FindAsync(key);
            if (supplierUser == null)
            {
                return NotFound();
            }

            db.SupplierUsers.Remove(supplierUser);
            await db.SaveChangesAsync();

            return StatusCode(HttpStatusCode.NoContent);
        }

        // GET: odata/SupplierUsers(5)/OneSupplier
        [EnableQuery]
        public SingleResult<Supplier> GetOneSupplier([FromODataUri] int key)
        {
            return SingleResult.Create(db.SupplierUsers.Where(m => m.SupplierUserID == key).Select(m => m.OneSupplier));
        }

        // GET: odata/SupplierUsers(5)/SupplierRoles
        [EnableQuery]
        public IQueryable<SupplierRole> GetSupplierRoles([FromODataUri] int key)
        {
            return db.SupplierUsers.Where(m => m.SupplierUserID == key).SelectMany(m => m.SupplierRoles);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool SupplierUserExists(int key)
        {
            return db.SupplierUsers.Count(e => e.SupplierUserID == key) > 0;
        }
    }
}
