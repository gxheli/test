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
    builder.EntitySet<TBOrderState>("TBOrderStates");
    builder.EntitySet<Customer>("Customers"); 
    builder.EntitySet<OrderSourse>("OrderSourses"); 
    config.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
    public class TBOrderStatesController : ODataController
    {
        private LanghuaContent db = new LanghuaContent();

        // GET: odata/TBOrderStates
        [EnableQuery]
        public IQueryable<TBOrderState> GetTBOrderStates()
        {
            return db.TBOrderStates;
        }

        // GET: odata/TBOrderStates(5)
        [EnableQuery]
        public SingleResult<TBOrderState> GetTBOrderState([FromODataUri] int key)
        {
            return SingleResult.Create(db.TBOrderStates.Where(tBOrderState => tBOrderState.TBOrderStateID == key));
        }

        // PUT: odata/TBOrderStates(5)
        public async Task<IHttpActionResult> Put([FromODataUri] int key, Delta<TBOrderState> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            TBOrderState tBOrderState = await db.TBOrderStates.FindAsync(key);
            if (tBOrderState == null)
            {
                return NotFound();
            }

            patch.Put(tBOrderState);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TBOrderStateExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(tBOrderState);
        }

        // POST: odata/TBOrderStates
        public async Task<IHttpActionResult> Post(TBOrderState tBOrderState)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.TBOrderStates.Add(tBOrderState);
            await db.SaveChangesAsync();

            return Created(tBOrderState);
        }

        // PATCH: odata/TBOrderStates(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public async Task<IHttpActionResult> Patch([FromODataUri] int key, Delta<TBOrderState> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            TBOrderState tBOrderState = await db.TBOrderStates.FindAsync(key);
            if (tBOrderState == null)
            {
                return NotFound();
            }

            patch.Patch(tBOrderState);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TBOrderStateExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(tBOrderState);
        }

        // DELETE: odata/TBOrderStates(5)
        public async Task<IHttpActionResult> Delete([FromODataUri] int key)
        {
            TBOrderState tBOrderState = await db.TBOrderStates.FindAsync(key);
            if (tBOrderState == null)
            {
                return NotFound();
            }

            db.TBOrderStates.Remove(tBOrderState);
            await db.SaveChangesAsync();

            return StatusCode(HttpStatusCode.NoContent);
        }

        // GET: odata/TBOrderStates(5)/SendCustomer
        [EnableQuery]
        public SingleResult<Customer> GetSendCustomer([FromODataUri] int key)
        {
            return SingleResult.Create(db.TBOrderStates.Where(m => m.TBOrderStateID == key).Select(m => m.SendCustomer));
        }

        // GET: odata/TBOrderStates(5)/Sourse
        [EnableQuery]
        public SingleResult<OrderSourse> GetSourse([FromODataUri] int key)
        {
            return SingleResult.Create(db.TBOrderStates.Where(m => m.TBOrderStateID == key).Select(m => m.Sourse));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool TBOrderStateExists(int key)
        {
            return db.TBOrderStates.Count(e => e.TBOrderStateID == key) > 0;
        }
    }
}
