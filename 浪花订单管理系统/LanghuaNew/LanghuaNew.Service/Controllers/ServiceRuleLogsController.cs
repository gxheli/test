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
    builder.EntitySet<ServiceRuleLog>("ServiceRuleLogs");
    builder.EntitySet<ServiceRule>("ServiceRules"); 
    config.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
    public class ServiceRuleLogsController : ODataController
    {
        private LanghuaContent db = new LanghuaContent();

        // GET: odata/ServiceRuleLogs
        [EnableQuery]
        public IQueryable<ServiceRuleLog> GetServiceRuleLogs()
        {
            return db.ServiceRuleLogs;
        }

        // GET: odata/ServiceRuleLogs(5)
        [EnableQuery]
        public SingleResult<ServiceRuleLog> GetServiceRuleLog([FromODataUri] int key)
        {
            return SingleResult.Create(db.ServiceRuleLogs.Where(serviceRuleLog => serviceRuleLog.ServiceRuleLogID == key));
        }

        // PUT: odata/ServiceRuleLogs(5)
        public async Task<IHttpActionResult> Put([FromODataUri] int key, Delta<ServiceRuleLog> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            ServiceRuleLog serviceRuleLog = await db.ServiceRuleLogs.FindAsync(key);
            if (serviceRuleLog == null)
            {
                return NotFound();
            }

            patch.Put(serviceRuleLog);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ServiceRuleLogExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(serviceRuleLog);
        }

        // POST: odata/ServiceRuleLogs
        public async Task<IHttpActionResult> Post(ServiceRuleLog serviceRuleLog)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.ServiceRuleLogs.Add(serviceRuleLog);
            await db.SaveChangesAsync();

            return Created(serviceRuleLog);
        }

        // PATCH: odata/ServiceRuleLogs(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public async Task<IHttpActionResult> Patch([FromODataUri] int key, Delta<ServiceRuleLog> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            ServiceRuleLog serviceRuleLog = await db.ServiceRuleLogs.FindAsync(key);
            if (serviceRuleLog == null)
            {
                return NotFound();
            }

            patch.Patch(serviceRuleLog);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ServiceRuleLogExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(serviceRuleLog);
        }

        // DELETE: odata/ServiceRuleLogs(5)
        public async Task<IHttpActionResult> Delete([FromODataUri] int key)
        {
            ServiceRuleLog serviceRuleLog = await db.ServiceRuleLogs.FindAsync(key);
            if (serviceRuleLog == null)
            {
                return NotFound();
            }

            db.ServiceRuleLogs.Remove(serviceRuleLog);
            await db.SaveChangesAsync();

            return StatusCode(HttpStatusCode.NoContent);
        }

        // GET: odata/ServiceRuleLogs(5)/ServiceRuleValue
        [EnableQuery]
        public SingleResult<ServiceRule> GetServiceRuleValue([FromODataUri] int key)
        {
            return SingleResult.Create(db.ServiceRuleLogs.Where(m => m.ServiceRuleLogID == key).Select(m => m.ServiceRuleValue));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ServiceRuleLogExists(int key)
        {
            return db.ServiceRuleLogs.Count(e => e.ServiceRuleLogID == key) > 0;
        }
    }
}
