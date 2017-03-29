using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace I18n
{
    public class LangHelper
    {
        public static string GetResourceByKey(string key)
        {
            return I18n.Resources.ResourceManager.GetString(key);
        }

        public static string GetResourceByKeyAndCultrue(string key, string cultureStr)
        {
            CultureInfo culture = new CultureInfo(cultureStr);
            return I18n.Resources.ResourceManager.GetString(key, culture);
        }

        public static string GetTableHeaderByKeyAndCcsClassArray(string[] keyArray,string[] ccsClassArray,string cultureStr)
        {
            string result = string.Empty;
            if ((keyArray!=null && keyArray.Length>0) && (ccsClassArray!=null && ccsClassArray.Length>0) && keyArray.Length == ccsClassArray.Length)
            {
                StringBuilder temp = new StringBuilder();
                for(int i=0;i<=keyArray.Length-1;i++)
                {
                    for(int j=0;j<=ccsClassArray.Length-1;j++)
                    {
                        if(i==j)
                        {
                            if(ccsClassArray[j]==null)
                            {
                                temp = temp.AppendFormat("<td>{0}</td>", GetResourceByKeyAndCultrue(keyArray[i], cultureStr));
                            }
                            else
                            {
                                temp = temp.AppendFormat("<td class='{1}'>{0}</td>", GetResourceByKeyAndCultrue(keyArray[i], cultureStr), ccsClassArray[j]);
                            }
                            result = string.Format("<table><tr>{0}</tr></table>", temp);
                        }
                    }
                }
            }

            return result;
        }
    }
}
