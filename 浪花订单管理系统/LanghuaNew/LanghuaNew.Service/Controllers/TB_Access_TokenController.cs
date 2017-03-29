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
    builder.EntitySet<TB_Access_Token>("TB_Access_Token");
    config.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
    public class TB_Access_TokenController : ODataController
    {
        private LanghuaContent db = new LanghuaContent();

        // GET: odata/TB_Access_Token
        [EnableQuery]
        public IQueryable<TB_Access_Token> GetTB_Access_Token()
        {
            return db.TB_Access_Token;
        }

        // GET: odata/TB_Access_Token(5)
        [EnableQuery]
        public SingleResult<TB_Access_Token> GetTB_Access_Token([FromODataUri] int key)
        {
            return SingleResult.Create(db.TB_Access_Token.Where(tB_Access_Token => tB_Access_Token.ID == key));
        }

        // PUT: odata/TB_Access_Token(5)
        public async Task<IHttpActionResult> Put([FromODataUri] int key, Delta<TB_Access_Token> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            TB_Access_Token tB_Access_Token = await db.TB_Access_Token.FindAsync(key);
            if (tB_Access_Token == null)
            {
                return NotFound();
            }

            patch.Put(tB_Access_Token);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TB_Access_TokenExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(tB_Access_Token);
        }

        // POST: odata/TB_Access_Token
        public async Task<IHttpActionResult> Post(TB_Access_Token tB_Access_Token)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.TB_Access_Token.Add(tB_Access_Token);
            await db.SaveChangesAsync();

            return Created(tB_Access_Token);
        }

        // PATCH: odata/TB_Access_Token(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public async Task<IHttpActionResult> Patch([FromODataUri] int key, Delta<TB_Access_Token> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            TB_Access_Token tB_Access_Token = await db.TB_Access_Token.FindAsync(key);
            if (tB_Access_Token == null)
            {
                return NotFound();
            }

            patch.Patch(tB_Access_Token);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TB_Access_TokenExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(tB_Access_Token);
        }

        // DELETE: odata/TB_Access_Token(5)
        public async Task<IHttpActionResult> Delete([FromODataUri] int key)
        {
            TB_Access_Token tB_Access_Token = await db.TB_Access_Token.FindAsync(key);
            if (tB_Access_Token == null)
            {
                return NotFound();
            }

            db.TB_Access_Token.Remove(tB_Access_Token);
            await db.SaveChangesAsync();

            return StatusCode(HttpStatusCode.NoContent);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool TB_Access_TokenExists(int key)
        {
            return db.TB_Access_Token.Count(e => e.ID == key) > 0;
        }
    }
}
