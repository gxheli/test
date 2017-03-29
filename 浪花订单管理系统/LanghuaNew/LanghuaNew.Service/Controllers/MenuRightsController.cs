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
    builder.EntitySet<MenuRight>("MenuRights");
    builder.EntitySet<RoleRight>("RoleRights"); 
    builder.EntitySet<Role>("Roles"); 
    config.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
    public class MenuRightsController : ODataController
    {
        private LanghuaContent db = new LanghuaContent();

        // GET: odata/MenuRights
        [EnableQuery]
        public IQueryable<MenuRight> GetMenuRights()
        {
            return db.MenuRights;
        }

        // GET: odata/MenuRights(5)
        [EnableQuery]
        public SingleResult<MenuRight> GetMenuRight([FromODataUri] int key)
        {
            return SingleResult.Create(db.MenuRights.Where(menuRight => menuRight.MenuRightID == key));
        }

        // PUT: odata/MenuRights(5)
        public async Task<IHttpActionResult> Put([FromODataUri] int key, Delta<MenuRight> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            MenuRight menuRight = await db.MenuRights.FindAsync(key);
            if (menuRight == null)
            {
                return NotFound();
            }

            patch.Put(menuRight);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MenuRightExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(menuRight);
        }

        // POST: odata/MenuRights
        public async Task<IHttpActionResult> Post(MenuRight menuRight)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.MenuRights.Add(menuRight);
            await db.SaveChangesAsync();

            return Created(menuRight);
        }

        // PATCH: odata/MenuRights(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public async Task<IHttpActionResult> Patch([FromODataUri] int key, Delta<MenuRight> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            MenuRight menuRight = await db.MenuRights.FindAsync(key);
            if (menuRight == null)
            {
                return NotFound();
            }

            patch.Patch(menuRight);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MenuRightExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(menuRight);
        }

        // DELETE: odata/MenuRights(5)
        public async Task<IHttpActionResult> Delete([FromODataUri] int key)
        {
            MenuRight menuRight = await db.MenuRights.FindAsync(key);
            if (menuRight == null)
            {
                return NotFound();
            }

            db.MenuRights.Remove(menuRight);
            await db.SaveChangesAsync();

            return StatusCode(HttpStatusCode.NoContent);
        }

        // GET: odata/MenuRights(5)/RoleRights
        [EnableQuery]
        public IQueryable<RoleRight> GetRoleRights([FromODataUri] int key)
        {
            return db.MenuRights.Where(m => m.MenuRightID == key).SelectMany(m => m.RoleRights);
        }

        // GET: odata/MenuRights(5)/Roles
        [EnableQuery]
        public IQueryable<Role> GetRoles([FromODataUri] int key)
        {
            return db.MenuRights.Where(m => m.MenuRightID == key).SelectMany(m => m.Roles);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool MenuRightExists(int key)
        {
            return db.MenuRights.Count(e => e.MenuRightID == key) > 0;
        }
    }
}
