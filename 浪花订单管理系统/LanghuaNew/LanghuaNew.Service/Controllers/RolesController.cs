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
    builder.EntitySet<Role>("Roles");
    builder.EntitySet<MenuRight>("MenuRights"); 
    builder.EntitySet<User>("Users"); 
    config.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
    public class RolesController : ODataController
    {
        private LanghuaContent db = new LanghuaContent();

        // GET: odata/Roles
        [EnableQuery]
        public IQueryable<Role> GetRoles()
        {
            return db.Roles;
        }

        // GET: odata/Roles(5)
        [EnableQuery]
        public SingleResult<Role> GetRole([FromODataUri] int key)
        {
            return SingleResult.Create(db.Roles.Where(role => role.RoleID == key));
        }

        // PUT: odata/Roles(5)
        public async Task<IHttpActionResult> Put([FromODataUri] int key, Delta<Role> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Role role = await db.Roles.FindAsync(key);
            if (role == null)
            {
                return NotFound();
            }

            patch.Put(role);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RoleExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(role);
        }

        // POST: odata/Roles
        public async Task<IHttpActionResult> Post(Role role)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Roles.Add(role);
            await db.SaveChangesAsync();

            return Created(role);
        }

        // PATCH: odata/Roles(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public async Task<IHttpActionResult> Patch([FromODataUri] int key, Delta<Role> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Role role = await db.Roles.FindAsync(key);
            if (role == null)
            {
                return NotFound();
            }

            patch.Patch(role);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RoleExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(role);
        }

        // DELETE: odata/Roles(5)
        public async Task<IHttpActionResult> Delete([FromODataUri] int key)
        {
            Role role = await db.Roles.FindAsync(key);
            if (role == null)
            {
                return NotFound();
            }

            db.Roles.Remove(role);
            await db.SaveChangesAsync();

            return StatusCode(HttpStatusCode.NoContent);
        }

        // GET: odata/Roles(5)/MenuRights
        [EnableQuery]
        public IQueryable<MenuRight> GetMenuRights([FromODataUri] int key)
        {
            return db.Roles.Where(m => m.RoleID == key).SelectMany(m => m.MenuRights);
        }

        // GET: odata/Roles(5)/Users
        [EnableQuery]
        public IQueryable<User> GetUsers([FromODataUri] int key)
        {
            return db.Roles.Where(m => m.RoleID == key).SelectMany(m => m.Users);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool RoleExists(int key)
        {
            return db.Roles.Count(e => e.RoleID == key) > 0;
        }
    }
}
