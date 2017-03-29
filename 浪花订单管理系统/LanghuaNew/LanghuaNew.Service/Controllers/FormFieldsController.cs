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
    builder.EntitySet<FormField>("FormFields");
    config.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
    public class FormFieldsController : ODataController
    {
        private LanghuaContent db = new LanghuaContent();

        // GET: odata/FormFields
        [EnableQuery]
        public IQueryable<FormField> GetFormFields()
        {
            return db.FormFields;
        }

        // GET: odata/FormFields(5)
        [EnableQuery]
        public SingleResult<FormField> GetFormField([FromODataUri] int key)
        {
            return SingleResult.Create(db.FormFields.Where(formField => formField.FormFieldID == key));
        }

        // PUT: odata/FormFields(5)
        public async Task<IHttpActionResult> Put([FromODataUri] int key, Delta<FormField> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            FormField formField = await db.FormFields.FindAsync(key);
            if (formField == null)
            {
                return NotFound();
            }

            patch.Put(formField);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FormFieldExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(formField);
        }

        // POST: odata/FormFields
        public async Task<IHttpActionResult> Post(FormField formField)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.FormFields.Add(formField);
            await db.SaveChangesAsync();

            return Created(formField);
        }

        // PATCH: odata/FormFields(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public async Task<IHttpActionResult> Patch([FromODataUri] int key, Delta<FormField> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            FormField formField = await db.FormFields.FindAsync(key);
            if (formField == null)
            {
                return NotFound();
            }

            patch.Patch(formField);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FormFieldExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(formField);
        }

        // DELETE: odata/FormFields(5)
        public async Task<IHttpActionResult> Delete([FromODataUri] int key)
        {
            FormField formField = await db.FormFields.FindAsync(key);
            if (formField == null)
            {
                return NotFound();
            }

            db.FormFields.Remove(formField);
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

        private bool FormFieldExists(int key)
        {
            return db.FormFields.Count(e => e.FormFieldID == key) > 0;
        }
    }
}
