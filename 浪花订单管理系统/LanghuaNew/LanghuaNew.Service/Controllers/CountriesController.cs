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
    builder.EntitySet<Country>("Countries");
    builder.EntitySet<City>("Cities"); 
    builder.EntitySet<Supplier>("Suppliers"); 
    config.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
    public class CountriesController : ODataController
    {
        private LanghuaContent db = new LanghuaContent();

        // GET: odata/Countries
        [EnableQuery]
        public IQueryable<Country> GetCountries()
        {
            return db.Countrys;
        }

        // GET: odata/Countries(5)
        [EnableQuery]
        public SingleResult<Country> GetCountry([FromODataUri] int key)
        {
            return SingleResult.Create(db.Countrys.Where(country => country.CountryID == key));
        }

        // PUT: odata/Countries(5)
        public async Task<IHttpActionResult> Put([FromODataUri] int key, Delta<Country> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Country country = await db.Countrys.FindAsync(key);
            if (country == null)
            {
                return NotFound();
            }

            patch.Put(country);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CountryExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(country);
        }

        // POST: odata/Countries
        public async Task<IHttpActionResult> Post(Country country)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Countrys.Add(country);
            await db.SaveChangesAsync();

            return Created(country);
        }

        // PATCH: odata/Countries(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public async Task<IHttpActionResult> Patch([FromODataUri] int key, Delta<Country> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Country country = await db.Countrys.FindAsync(key);
            if (country == null)
            {
                return NotFound();
            }

            patch.Patch(country);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CountryExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(country);
        }

        // DELETE: odata/Countries(5)
        public async Task<IHttpActionResult> Delete([FromODataUri] int key)
        {
            Country country = await db.Countrys.FindAsync(key);
            if (country == null)
            {
                return NotFound();
            }

            db.Countrys.Remove(country);
            await db.SaveChangesAsync();

            return StatusCode(HttpStatusCode.NoContent);
        }

        // GET: odata/Countries(5)/Citys
        [EnableQuery]
        public IQueryable<City> GetCitys([FromODataUri] int key)
        {
            return db.Countrys.Where(m => m.CountryID == key).SelectMany(m => m.Citys);
        }

        // GET: odata/Countries(5)/Suppliers
        [EnableQuery]
        public IQueryable<Supplier> GetSuppliers([FromODataUri] int key)
        {
            return db.Countrys.Where(m => m.CountryID == key).SelectMany(m => m.Suppliers);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool CountryExists(int key)
        {
            return db.Countrys.Count(e => e.CountryID == key) > 0;
        }
    }
}
