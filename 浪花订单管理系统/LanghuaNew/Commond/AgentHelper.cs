using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Commond
{
    public class AgentHelper
    {
        public static string GetOSNameByUserAgent(string userAgent)
        {
            string osVersion = "Desktop";
            if(userAgent == null)
            {
                return osVersion;
            }
            
            if (userAgent.Contains("iPhone"))
            {
                osVersion = "Mobbile";
            }
            else if (userAgent.Contains("NOKIA"))
            {
                osVersion = "Mobbile";
            }
            else if (userAgent.Contains("Android"))
            {
                osVersion = "Mobbile";
            }
            return osVersion;
        }
        public static string GetBrowserKernelByUserAgent(string userAgent)
        {
            if (userAgent == null)
            {
                return "NOTIE";
            }
            if (userAgent.Contains("Trident"))
            {
                return "IE";
            }
            else
            {
                return "NOTIE";
            }
            
        }

        public static int GetIEVersionByUserAgent(string userAgent)
        {
            if(userAgent!=null && (userAgent.Contains("Trident")||userAgent.Contains("MSIE")))
            {
                string[] list = userAgent.Split(';');
                if (list.Length > 1)
                {
                    if(list[1].Contains("WOW64"))
                    {
                        return 11;
                    }
                    else
                    {
                        var VersionStr = list[1].Replace("MSIE", "").Trim();
                        int i= 13;
                        try
                        {
                            VersionStr = VersionStr.Split('.')[0];
                            i = int.Parse(VersionStr);
                        }
                        catch
                        {
                            i = 14;
                        }
                        return i;
                    }
                }
            }
            return 12;
        }
    }
}
