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
    builder.EntitySet<RoleRight>("RoleRights");
    builder.EntitySet<MenuRight>("MenuRights"); 
    config.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
    public class RoleRightsController : ODataController
    {
        private LanghuaContent db = new LanghuaContent();

        // GET: odata/RoleRights
        [EnableQuery]
        public IQueryable<RoleRight> GetRoleRights()
        {
            return db.RoleRights;
        }

        // GET: odata/RoleRights(5)
        [EnableQuery]
        public SingleResult<RoleRight> GetRoleRight([FromODataUri] int key)
        {
            return SingleResult.Create(db.RoleRights.Where(roleRight => roleRight.RoleRightID == key));
        }

        // PUT: odata/RoleRights(5)
        public async Task<IHttpActionResult> Put([FromODataUri] int key, Delta<RoleRight> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            RoleRight roleRight = await db.RoleRights.FindAsync(key);
            if (roleRight == null)
            {
                return NotFound();
            }

            patch.Put(roleRight);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RoleRightExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(roleRight);
        }

        // POST: odata/RoleRights
        public async Task<IHttpActionResult> Post(RoleRight roleRight)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.RoleRights.Add(roleRight);
            await db.SaveChangesAsync();

            return Created(roleRight);
        }

        // PATCH: odata/RoleRights(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public async Task<IHttpActionResult> Patch([FromODataUri] int key, Delta<RoleRight> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            RoleRight roleRight = await db.RoleRights.FindAsync(key);
            if (roleRight == null)
            {
                return NotFound();
            }

            patch.Patch(roleRight);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RoleRightExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(roleRight);
        }

        // DELETE: odata/RoleRights(5)
        public async Task<IHttpActionResult> Delete([FromODataUri] int key)
        {
            RoleRight roleRight = await db.RoleRights.FindAsync(key);
            if (roleRight == null)
            {
                return NotFound();
            }

            db.RoleRights.Remove(roleRight);
            await db.SaveChangesAsync();

            return StatusCode(HttpStatusCode.NoContent);
        }

        // GET: odata/RoleRights(5)/menuRight
        [EnableQuery]
        public SingleResult<MenuRight> GetmenuRight([FromODataUri] int key)
        {
            return SingleResult.Create(db.RoleRights.Where(m => m.RoleRightID == key).Select(m => m.menuRight));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool RoleRightExists(int key)
        {
            return db.RoleRights.Count(e => e.RoleRightID == key) > 0;
        }
    }
}
