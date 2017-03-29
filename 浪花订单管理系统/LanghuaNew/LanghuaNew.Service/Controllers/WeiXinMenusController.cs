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
    builder.EntitySet<WeiXinMenu>("WeiXinMenus");
    builder.EntitySet<MenuItem>("MenuItems"); 
    config.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
    public class WeiXinMenusController : ODataController
    {
        private WeiXinContent db = new WeiXinContent();

        // GET: odata/WeiXinMenus
        [EnableQuery]
        public IQueryable<WeiXinMenu> GetWeiXinMenus()
        {
            return db.WeiXinMenus;
        }

        // GET: odata/WeiXinMenus(5)
        [EnableQuery]
        public SingleResult<WeiXinMenu> GetWeiXinMenu([FromODataUri] int key)
        {
            return SingleResult.Create(db.WeiXinMenus.Where(weiXinMenu => weiXinMenu.WeiXinMenuID == key));
        }

        // PUT: odata/WeiXinMenus(5)
        public async Task<IHttpActionResult> Put([FromODataUri] int key, Delta<WeiXinMenu> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            WeiXinMenu weiXinMenu = await db.WeiXinMenus.FindAsync(key);
            if (weiXinMenu == null)
            {
                return NotFound();
            }

            patch.Put(weiXinMenu);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!WeiXinMenuExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(weiXinMenu);
        }

        // POST: odata/WeiXinMenus
        public async Task<IHttpActionResult> Post(WeiXinMenu weiXinMenu)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.WeiXinMenus.Add(weiXinMenu);
            await db.SaveChangesAsync();

            return Created(weiXinMenu);
        }

        // PATCH: odata/WeiXinMenus(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public async Task<IHttpActionResult> Patch([FromODataUri] int key, Delta<WeiXinMenu> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            WeiXinMenu weiXinMenu = await db.WeiXinMenus.FindAsync(key);
            if (weiXinMenu == null)
            {
                return NotFound();
            }

            patch.Patch(weiXinMenu);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!WeiXinMenuExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(weiXinMenu);
        }

        // DELETE: odata/WeiXinMenus(5)
        public async Task<IHttpActionResult> Delete([FromODataUri] int key)
        {
            WeiXinMenu weiXinMenu = await db.WeiXinMenus.FindAsync(key);
            if (weiXinMenu == null)
            {
                return NotFound();
            }

            db.WeiXinMenus.Remove(weiXinMenu);
            await db.SaveChangesAsync();

            return StatusCode(HttpStatusCode.NoContent);
        }

        // GET: odata/WeiXinMenus(5)/Items
        [EnableQuery]
        public IQueryable<MenuItem> GetItems([FromODataUri] int key)
        {
            return db.WeiXinMenus.Where(m => m.WeiXinMenuID == key).SelectMany(m => m.Items);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool WeiXinMenuExists(int key)
        {
            return db.WeiXinMenus.Count(e => e.WeiXinMenuID == key) > 0;
        }
    }
}
