using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Optimization;

namespace LanghuaNew
{
    public class VersionBundleTransform : IBundleTransform
    {
        public void Process(BundleContext context, BundleResponse response)
        {
            foreach (var file in response.Files)
            {
                using (FileStream fs = File.OpenRead(HostingEnvironment.MapPath(file.IncludedVirtualPath)))
                {
                    //get hash of file contents
               
                    file.IncludedVirtualPath = string.Concat(file.IncludedVirtualPath, "?v=", System.Configuration.ConfigurationManager.AppSettings["version"]);
                }
            }
        }
    }
}