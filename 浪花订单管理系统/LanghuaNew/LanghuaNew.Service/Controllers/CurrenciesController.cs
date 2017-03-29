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
    builder.EntitySet<Currency>("Currencies");
    builder.EntitySet<SupplierServiceItem>("SupplierServiceItems"); 
    config.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
    public class CurrenciesController : ODataController
    {
        private LanghuaContent db = new LanghuaContent();

        // GET: odata/Currencies
        [EnableQuery]
        public IQueryable<Currency> GetCurrencies()
        {
            return db.Currencys;
        }

        // GET: odata/Currencies(5)
        [EnableQuery]
        public SingleResult<Currency> GetCurrency([FromODataUri] int key)
        {
            return SingleResult.Create(db.Currencys.Where(currency => currency.CurrencyID == key));
        }

        // PUT: odata/Currencies(5)
        public async Task<IHttpActionResult> Put([FromODataUri] int key, Delta<Currency> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Currency currency = await db.Currencys.FindAsync(key);
            if (currency == null)
            {
                return NotFound();
            }

            patch.Put(currency);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CurrencyExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(currency);
        }

        // POST: odata/Currencies
        public async Task<IHttpActionResult> Post(Currency currency)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Currencys.Add(currency);
            await db.SaveChangesAsync();

            return Created(currency);
        }

        // PATCH: odata/Currencies(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public async Task<IHttpActionResult> Patch([FromODataUri] int key, Delta<Currency> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Currency currency = await db.Currencys.FindAsync(key);
            if (currency == null)
            {
                return NotFound();
            }

            patch.Patch(currency);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CurrencyExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(currency);
        }

        // DELETE: odata/Currencies(5)
        public async Task<IHttpActionResult> Delete([FromODataUri] int key)
        {
            Currency currency = await db.Currencys.FindAsync(key);
            if (currency == null)
            {
                return NotFound();
            }

            db.Currencys.Remove(currency);
            await db.SaveChangesAsync();

            return StatusCode(HttpStatusCode.NoContent);
        }

        // GET: odata/Currencies(5)/SupplierServiceItems
        [EnableQuery]
        public IQueryable<SupplierServiceItem> GetSupplierServiceItems([FromODataUri] int key)
        {
            return db.Currencys.Where(m => m.CurrencyID == key).SelectMany(m => m.SupplierServiceItems);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool CurrencyExists(int key)
        {
            return db.Currencys.Count(e => e.CurrencyID == key) > 0;
        }
    }
}
