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
using WXData;

namespace LanghuaNew.Service.Controllers
{
    /*
    The WebApiConfig class may require additional changes to add a route for this controller. Merge these statements into the Register method of the WebApiConfig class as applicable. Note that OData URLs are case sensitive.

    using System.Web.OData.Builder;
    using System.Web.OData.Extensions;
    using WXData;
    ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
    builder.EntitySet<MenuItem>("MenuItems");
    builder.EntitySet<WeiXinMenu>("WeiXinMenus"); 
    config.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
    public class MenuItemsController : ODataController
    {
        private WeiXinContent db = new WeiXinContent();

        // GET: odata/MenuItems
        [EnableQuery]
        public IQueryable<MenuItem> GetMenuItems()
        {
            return db.MenuItems;
        }

        // GET: odata/MenuItems(5)
        [EnableQuery]
        public SingleResult<MenuItem> GetMenuItem([FromODataUri] int key)
        {
            return SingleResult.Create(db.MenuItems.Where(menuItem => menuItem.MenuItemID == key));
        }

        // PUT: odata/MenuItems(5)
        public async Task<IHttpActionResult> Put([FromODataUri] int key, Delta<MenuItem> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            MenuItem menuItem = await db.MenuItems.FindAsync(key);
            if (menuItem == null)
            {
                return NotFound();
            }

            patch.Put(menuItem);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MenuItemExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(menuItem);
        }

        // POST: odata/MenuItems
        public async Task<IHttpActionResult> Post(MenuItem menuItem)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.MenuItems.Add(menuItem);
            await db.SaveChangesAsync();

            return Created(menuItem);
        }

        // PATCH: odata/MenuItems(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public async Task<IHttpActionResult> Patch([FromODataUri] int key, Delta<MenuItem> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            MenuItem menuItem = await db.MenuItems.FindAsync(key);
            if (menuItem == null)
            {
                return NotFound();
            }

            patch.Patch(menuItem);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MenuItemExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(menuItem);
        }

        // DELETE: odata/MenuItems(5)
        public async Task<IHttpActionResult> Delete([FromODataUri] int key)
        {
            MenuItem menuItem = await db.MenuItems.FindAsync(key);
            if (menuItem == null)
            {
                return NotFound();
            }

            db.MenuItems.Remove(menuItem);
            await db.SaveChangesAsync();

            return StatusCode(HttpStatusCode.NoContent);
        }

        // GET: odata/MenuItems(5)/WeiXinMenuValue
        [EnableQuery]
        public SingleResult<WeiXinMenu> GetWeiXinMenuValue([FromODataUri] int key)
        {
            return SingleResult.Create(db.MenuItems.Where(m => m.MenuItemID == key).Select(m => m.WeiXinMenuValue));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool MenuItemExists(int key)
        {
            return db.MenuItems.Count(e => e.MenuItemID == key) > 0;
        }
    }
}
