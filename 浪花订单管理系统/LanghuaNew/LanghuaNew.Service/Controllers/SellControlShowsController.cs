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
    builder.EntitySet<SellControlShow>("SellControlShows");
    builder.EntitySet<SellControl>("SellControls"); 
    config.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
    public class SellControlShowsController : ODataController
    {
        private LanghuaContent db = new LanghuaContent();

        // GET: odata/SellControlShows
        [EnableQuery]
        public IQueryable<SellControlShow> GetSellControlShows()
        {
            return db.SellControlShows;
        }

        // GET: odata/SellControlShows(5)
        [EnableQuery]
        public SingleResult<SellControlShow> GetSellControlShow([FromODataUri] int key)
        {
            return SingleResult.Create(db.SellControlShows.Where(sellControlShow => sellControlShow.SellControlShowID == key));
        }

        // PUT: odata/SellControlShows(5)
        public async Task<IHttpActionResult> Put([FromODataUri] int key, Delta<SellControlShow> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            SellControlShow sellControlShow = await db.SellControlShows.FindAsync(key);
            if (sellControlShow == null)
            {
                return NotFound();
            }

            patch.Put(sellControlShow);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SellControlShowExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(sellControlShow);
        }

        // POST: odata/SellControlShows
        public async Task<IHttpActionResult> Post(SellControlShow sellControlShow)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.SellControlShows.Add(sellControlShow);
            await db.SaveChangesAsync();

            return Created(sellControlShow);
        }

        // PATCH: odata/SellControlShows(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public async Task<IHttpActionResult> Patch([FromODataUri] int key, Delta<SellControlShow> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            SellControlShow sellControlShow = await db.SellControlShows.FindAsync(key);
            if (sellControlShow == null)
            {
                return NotFound();
            }

            patch.Patch(sellControlShow);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SellControlShowExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(sellControlShow);
        }

        // DELETE: odata/SellControlShows(5)
        public async Task<IHttpActionResult> Delete([FromODataUri] int key)
        {
            SellControlShow sellControlShow = await db.SellControlShows.FindAsync(key);
            if (sellControlShow == null)
            {
                return NotFound();
            }

            db.SellControlShows.Remove(sellControlShow);
            await db.SaveChangesAsync();

            return StatusCode(HttpStatusCode.NoContent);
        }

        // GET: odata/SellControlShows(5)/sell
        [EnableQuery]
        public SingleResult<SellControl> Getsell([FromODataUri] int key)
        {
            return SingleResult.Create(db.SellControlShows.Where(m => m.SellControlShowID == key).Select(m => m.sell));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool SellControlShowExists(int key)
        {
            return db.SellControlShows.Count(e => e.SellControlShowID == key) > 0;
        }
    }
}
