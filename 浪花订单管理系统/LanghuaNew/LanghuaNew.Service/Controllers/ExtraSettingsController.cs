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
    builder.EntitySet<ExtraSetting>("ExtraSettings");
    builder.EntitySet<SellControl>("SellControls"); 
    config.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
    public class ExtraSettingsController : ODataController
    {
        private LanghuaContent db = new LanghuaContent();

        // GET: odata/ExtraSettings
        [EnableQuery]
        public IQueryable<ExtraSetting> GetExtraSettings()
        {
            return db.ExtraSettings;
        }

        // GET: odata/ExtraSettings(5)
        [EnableQuery]
        public SingleResult<ExtraSetting> GetExtraSetting([FromODataUri] int key)
        {
            return SingleResult.Create(db.ExtraSettings.Where(extraSetting => extraSetting.ExtraSettingID == key));
        }

        // PUT: odata/ExtraSettings(5)
        public async Task<IHttpActionResult> Put([FromODataUri] int key, Delta<ExtraSetting> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            ExtraSetting extraSetting = await db.ExtraSettings.FindAsync(key);
            if (extraSetting == null)
            {
                return NotFound();
            }

            patch.Put(extraSetting);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ExtraSettingExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(extraSetting);
        }

        // POST: odata/ExtraSettings
        public async Task<IHttpActionResult> Post(ExtraSetting extraSetting)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.ExtraSettings.Add(extraSetting);
            await db.SaveChangesAsync();

            return Created(extraSetting);
        }

        // PATCH: odata/ExtraSettings(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public async Task<IHttpActionResult> Patch([FromODataUri] int key, Delta<ExtraSetting> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            ExtraSetting extraSetting = await db.ExtraSettings.FindAsync(key);
            if (extraSetting == null)
            {
                return NotFound();
            }

            patch.Patch(extraSetting);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ExtraSettingExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(extraSetting);
        }

        // DELETE: odata/ExtraSettings(5)
        public async Task<IHttpActionResult> Delete([FromODataUri] int key)
        {
            ExtraSetting extraSetting = await db.ExtraSettings.FindAsync(key);
            if (extraSetting == null)
            {
                return NotFound();
            }

            db.ExtraSettings.Remove(extraSetting);
            await db.SaveChangesAsync();

            return StatusCode(HttpStatusCode.NoContent);
        }

        // GET: odata/ExtraSettings(5)/OneSellControl
        [EnableQuery]
        public SingleResult<SellControl> GetOneSellControl([FromODataUri] int key)
        {
            return SingleResult.Create(db.ExtraSettings.Where(m => m.ExtraSettingID == key).Select(m => m.OneSellControl));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ExtraSettingExists(int key)
        {
            return db.ExtraSettings.Count(e => e.ExtraSettingID == key) > 0;
        }
    }
}
