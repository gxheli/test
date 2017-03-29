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
    builder.EntitySet<QRcodeStatistic>("QRcodeStatistics");
    config.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
    public class QRcodeStatisticsController : ODataController
    {
        private LanghuaContent db = new LanghuaContent();

        // GET: odata/QRcodeStatistics
        [EnableQuery]
        public IQueryable<QRcodeStatistic> GetQRcodeStatistics()
        {
            return db.QRcodeStatistics;
        }

        // GET: odata/QRcodeStatistics(5)
        [EnableQuery]
        public SingleResult<QRcodeStatistic> GetQRcodeStatistic([FromODataUri] int key)
        {
            return SingleResult.Create(db.QRcodeStatistics.Where(qRcodeStatistic => qRcodeStatistic.QRcodeStatisticID == key));
        }

        // PUT: odata/QRcodeStatistics(5)
        public async Task<IHttpActionResult> Put([FromODataUri] int key, Delta<QRcodeStatistic> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            QRcodeStatistic qRcodeStatistic = await db.QRcodeStatistics.FindAsync(key);
            if (qRcodeStatistic == null)
            {
                return NotFound();
            }

            patch.Put(qRcodeStatistic);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!QRcodeStatisticExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(qRcodeStatistic);
        }

        // POST: odata/QRcodeStatistics
        public async Task<IHttpActionResult> Post(QRcodeStatistic qRcodeStatistic)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.QRcodeStatistics.Add(qRcodeStatistic);
            await db.SaveChangesAsync();

            return Created(qRcodeStatistic);
        }

        // PATCH: odata/QRcodeStatistics(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public async Task<IHttpActionResult> Patch([FromODataUri] int key, Delta<QRcodeStatistic> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            QRcodeStatistic qRcodeStatistic = await db.QRcodeStatistics.FindAsync(key);
            if (qRcodeStatistic == null)
            {
                return NotFound();
            }

            patch.Patch(qRcodeStatistic);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!QRcodeStatisticExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(qRcodeStatistic);
        }

        // DELETE: odata/QRcodeStatistics(5)
        public async Task<IHttpActionResult> Delete([FromODataUri] int key)
        {
            QRcodeStatistic qRcodeStatistic = await db.QRcodeStatistics.FindAsync(key);
            if (qRcodeStatistic == null)
            {
                return NotFound();
            }

            db.QRcodeStatistics.Remove(qRcodeStatistic);
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

        private bool QRcodeStatisticExists(int key)
        {
            return db.QRcodeStatistics.Count(e => e.QRcodeStatisticID == key) > 0;
        }
    }
}
