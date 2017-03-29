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
    builder.EntitySet<TBOrderNo>("TBOrderNoes");
    builder.EntitySet<Order>("Orders"); 
    config.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
    public class TBOrderNoesController : ODataController
    {
        private LanghuaContent db = new LanghuaContent();

        // GET: odata/TBOrderNoes
        [EnableQuery]
        public IQueryable<TBOrderNo> GetTBOrderNoes()
        {
            return db.TBOrderNoes;
        }

        // GET: odata/TBOrderNoes(5)
        [EnableQuery]
        public SingleResult<TBOrderNo> GetTBOrderNo([FromODataUri] int key)
        {
            return SingleResult.Create(db.TBOrderNoes.Where(tBOrderNo => tBOrderNo.TBOrderNoID == key));
        }

        // PUT: odata/TBOrderNoes(5)
        public async Task<IHttpActionResult> Put([FromODataUri] int key, Delta<TBOrderNo> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            TBOrderNo tBOrderNo = await db.TBOrderNoes.FindAsync(key);
            if (tBOrderNo == null)
            {
                return NotFound();
            }

            patch.Put(tBOrderNo);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TBOrderNoExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(tBOrderNo);
        }

        // POST: odata/TBOrderNoes
        public async Task<IHttpActionResult> Post(TBOrderNo tBOrderNo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.TBOrderNoes.Add(tBOrderNo);
            await db.SaveChangesAsync();

            return Created(tBOrderNo);
        }

        // PATCH: odata/TBOrderNoes(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public async Task<IHttpActionResult> Patch([FromODataUri] int key, Delta<TBOrderNo> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            TBOrderNo tBOrderNo = await db.TBOrderNoes.FindAsync(key);
            if (tBOrderNo == null)
            {
                return NotFound();
            }

            patch.Patch(tBOrderNo);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TBOrderNoExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(tBOrderNo);
        }

        // DELETE: odata/TBOrderNoes(5)
        public async Task<IHttpActionResult> Delete([FromODataUri] int key)
        {
            TBOrderNo tBOrderNo = await db.TBOrderNoes.FindAsync(key);
            if (tBOrderNo == null)
            {
                return NotFound();
            }

            db.TBOrderNoes.Remove(tBOrderNo);
            await db.SaveChangesAsync();

            return StatusCode(HttpStatusCode.NoContent);
        }

        // GET: odata/TBOrderNoes(5)/order
        [EnableQuery]
        public SingleResult<Order> Getorder([FromODataUri] int key)
        {
            return SingleResult.Create(db.TBOrderNoes.Where(m => m.TBOrderNoID == key).Select(m => m.order));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool TBOrderNoExists(int key)
        {
            return db.TBOrderNoes.Count(e => e.TBOrderNoID == key) > 0;
        }
    }
}
