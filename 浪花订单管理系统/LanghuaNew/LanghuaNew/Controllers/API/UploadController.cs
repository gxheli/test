using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace LanghuaNew.Controllers.API
{
    public class UploadController : ApiController
    {
        public async Task<string> Post()
        {
            // Check whether the POST operation is MultiPart?  
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            // Prepare CustomMultipartFormDataStreamProvider in which our multipart form  
            // data will be loaded.  
            string fileSaveLocation = HttpContext.Current.Server.MapPath("~/App_Data");
            CustomMultipartFormDataStreamProvider provider = new CustomMultipartFormDataStreamProvider(fileSaveLocation);
            List<string> files = new List<string>();

            try
            {
                // Read all contents of multipart message into CustomMultipartFormDataStreamProvider.  
                await Request.Content.ReadAsMultipartAsync(provider);

                foreach (MultipartFileData file in provider.FileData)
                {
                    files.Add(Path.GetFileName(file.LocalFileName));
                }

                // Send OK Response along with saved file names to the client.  
                return JsonConvert.SerializeObject(new { ErrorCode = 200, ErrorMessage = "OK"});
            }
            catch (System.Exception e)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 400, ErrorMessage = e });
            }
        }
    }
    public class CustomMultipartFormDataStreamProvider : MultipartFormDataStreamProvider
    {
        public CustomMultipartFormDataStreamProvider(string path) : base(path) { }

        public override string GetLocalFileName(HttpContentHeaders headers)
        {
            return headers.ContentDisposition.FileName.Replace("\"", string.Empty);
        }
    }
}
