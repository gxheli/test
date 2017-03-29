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
    builder.EntitySet<ServiceRule>("ServiceRules");
    builder.EntitySet<ServiceItem>("ServiceItems"); 
    config.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
    public class ServiceRulesController : ODataController
    {
        private LanghuaContent db = new LanghuaContent();

        // GET: odata/ServiceRules
        [EnableQuery]
        public IQueryable<ServiceRule> GetServiceRules()
        {
            return db.ServiceRules;
        }

        // GET: odata/ServiceRules(5)
        [EnableQuery]
        public SingleResult<ServiceRule> GetServiceRule([FromODataUri] int key)
        {
            return SingleResult.Create(db.ServiceRules.Where(serviceRule => serviceRule.ServiceRuleID == key));
        }

        // PUT: odata/ServiceRules(5)
        public async Task<IHttpActionResult> Put([FromODataUri] int key, Delta<ServiceRule> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            ServiceRule serviceRule = await db.ServiceRules.FindAsync(key);
            if (serviceRule == null)
            {
                return NotFound();
            }

            patch.Put(serviceRule);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ServiceRuleExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(serviceRule);
        }

        // POST: odata/ServiceRules
        public async Task<IHttpActionResult> Post(ServiceRule serviceRule)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.ServiceRules.Add(serviceRule);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (ServiceRuleExists(serviceRule.ServiceRuleID))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return Created(serviceRule);
        }

        // PATCH: odata/ServiceRules(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public async Task<IHttpActionResult> Patch([FromODataUri] int key, Delta<ServiceRule> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            ServiceRule serviceRule = await db.ServiceRules.FindAsync(key);
            if (serviceRule == null)
            {
                return NotFound();
            }

            patch.Patch(serviceRule);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ServiceRuleExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(serviceRule);
        }

        // DELETE: odata/ServiceRules(5)
        public async Task<IHttpActionResult> Delete([FromODataUri] int key)
        {
            ServiceRule serviceRule = await db.ServiceRules.FindAsync(key);
            if (serviceRule == null)
            {
                return NotFound();
            }

            db.ServiceRules.Remove(serviceRule);
            await db.SaveChangesAsync();

            return StatusCode(HttpStatusCode.NoContent);
        }

        // GET: odata/ServiceRules(5)/RuleServiceItem
        [EnableQuery]
        public IQueryable<ServiceItem> GetRuleServiceItem([FromODataUri] int key)
        {
            return db.ServiceRules.Where(m => m.ServiceRuleID == key).SelectMany(m => m.RuleServiceItem);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ServiceRuleExists(int key)
        {
            return db.ServiceRules.Count(e => e.ServiceRuleID == key) > 0;
        }
    }
}
