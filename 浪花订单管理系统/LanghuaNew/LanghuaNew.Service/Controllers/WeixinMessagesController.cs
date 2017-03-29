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
    builder.EntitySet<WeixinMessage>("WeixinMessages");
    builder.EntitySet<Country>("Countrys"); 
    config.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
    public class WeixinMessagesController : ODataController
    {
        private LanghuaContent db = new LanghuaContent();

        // GET: odata/WeixinMessages
        [EnableQuery]
        public IQueryable<WeixinMessage> GetWeixinMessages()
        {
            return db.WeixinMessages;
        }

        // GET: odata/WeixinMessages(5)
        [EnableQuery]
        public SingleResult<WeixinMessage> GetWeixinMessage([FromODataUri] int key)
        {
            return SingleResult.Create(db.WeixinMessages.Where(weixinMessage => weixinMessage.WeixinMessageID == key));
        }

        // PUT: odata/WeixinMessages(5)
        public async Task<IHttpActionResult> Put([FromODataUri] int key, Delta<WeixinMessage> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            WeixinMessage weixinMessage = await db.WeixinMessages.FindAsync(key);
            if (weixinMessage == null)
            {
                return NotFound();
            }

            patch.Put(weixinMessage);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!WeixinMessageExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(weixinMessage);
        }

        // POST: odata/WeixinMessages
        public async Task<IHttpActionResult> Post(WeixinMessage weixinMessage)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.WeixinMessages.Add(weixinMessage);
            await db.SaveChangesAsync();

            return Created(weixinMessage);
        }

        // PATCH: odata/WeixinMessages(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public async Task<IHttpActionResult> Patch([FromODataUri] int key, Delta<WeixinMessage> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            WeixinMessage weixinMessage = await db.WeixinMessages.FindAsync(key);
            if (weixinMessage == null)
            {
                return NotFound();
            }

            patch.Patch(weixinMessage);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!WeixinMessageExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(weixinMessage);
        }

        // DELETE: odata/WeixinMessages(5)
        public async Task<IHttpActionResult> Delete([FromODataUri] int key)
        {
            WeixinMessage weixinMessage = await db.WeixinMessages.FindAsync(key);
            if (weixinMessage == null)
            {
                return NotFound();
            }

            db.WeixinMessages.Remove(weixinMessage);
            await db.SaveChangesAsync();

            return StatusCode(HttpStatusCode.NoContent);
        }

        // GET: odata/WeixinMessages(5)/WeixinCountry
        [EnableQuery]
        public SingleResult<Country> GetWeixinCountry([FromODataUri] int key)
        {
            return SingleResult.Create(db.WeixinMessages.Where(m => m.WeixinMessageID == key).Select(m => m.WeixinCountry));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool WeixinMessageExists(int key)
        {
            return db.WeixinMessages.Count(e => e.WeixinMessageID == key) > 0;
        }
    }
}
