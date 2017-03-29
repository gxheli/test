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
    builder.EntitySet<Traveller>("Travellers");
    builder.EntitySet<ServiceItemHistory>("ServiceItemHistories"); 
    config.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
    public class TravellersController : ODataController
    {
        private LanghuaContent db = new LanghuaContent();

        // GET: odata/Travellers
        [EnableQuery]
        public IQueryable<Traveller> GetTravellers()
        {
            return db.Travellers;
        }

        // GET: odata/Travellers(5)
        [EnableQuery]
        public SingleResult<Traveller> GetTraveller([FromODataUri] int key)
        {
            return SingleResult.Create(db.Travellers.Where(traveller => traveller.TravellerID == key));
        }

        // PUT: odata/Travellers(5)
        public async Task<IHttpActionResult> Put([FromODataUri] int key, Delta<Traveller> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Traveller traveller = await db.Travellers.FindAsync(key);
            if (traveller == null)
            {
                return NotFound();
            }

            patch.Put(traveller);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TravellerExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(traveller);
        }

        // POST: odata/Travellers
        public async Task<IHttpActionResult> Post(Traveller traveller)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Travellers.Add(traveller);
            await db.SaveChangesAsync();

            return Created(traveller);
        }

        // PATCH: odata/Travellers(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public async Task<IHttpActionResult> Patch([FromODataUri] int key, Delta<Traveller> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Traveller traveller = await db.Travellers.FindAsync(key);
            if (traveller == null)
            {
                return NotFound();
            }

            patch.Patch(traveller);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TravellerExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(traveller);
        }

        // DELETE: odata/Travellers(5)
        public async Task<IHttpActionResult> Delete([FromODataUri] int key)
        {
            Traveller traveller = await db.Travellers.FindAsync(key);
            if (traveller == null)
            {
                return NotFound();
            }

            db.Travellers.Remove(traveller);
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

        private bool TravellerExists(int key)
        {
            return db.Travellers.Count(e => e.TravellerID == key) > 0;
        }
    }
}
