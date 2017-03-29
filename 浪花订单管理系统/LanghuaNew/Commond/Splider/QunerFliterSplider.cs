using LanghuaNew.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Commond.Splider
{
    public class QunerFliterSplider : ISpider<FliterInfo>
    {
        public string Html { get; set; }

        public string GetHtmlByUrl(string url)
        {
            string HtmlStr = string.Empty;
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            
            using (StreamReader Reader = new StreamReader(req.GetResponse().GetResponseStream(), Encoding.UTF8))
            {
                HtmlStr = Reader.ReadToEnd();

            }
            Html = HtmlStr;
            return HtmlStr;
        }

        public List<FliterInfo> Capture()
        {
            FliterInfo fl = new FliterInfo();
            List<FliterInfo> fi = new List<FliterInfo>();
            string str = string.Empty;
            //一个匹配
            Regex re = new Regex(@"<div\sclass=""result_content""\sid=""rstContent"">.*</div>", RegexOptions.Singleline);
            if (re.IsMatch(Html))
            {
                MatchCollection matchCollection = re.Matches(Html);
                str = matchCollection[0].Value;

                //两个匹配
                Regex re2 = new Regex(@"<ul>.*?</ul>", RegexOptions.Singleline);
                if (re2.IsMatch(str))
                {
                    MatchCollection ulCollection = re2.Matches(str);
                    str = ulCollection[0].Value;
                    fi = GetData(str);
                }
                
            }
           // fi = (List<FliterInfo>)Fliters;
            return fi;
        }

        public List<FliterInfo> GetData(string str)
        {
            List<FliterInfo> data = new List<FliterInfo>();
            //列表匹配
            Regex re3 = new Regex(@"<li.*?</li>", RegexOptions.Singleline);
            if (re3.IsMatch(str))
            {
                MatchCollection matchCollection = re3.Matches(str);
                foreach (Match s in matchCollection)
                {
                    FliterInfo f = new FliterInfo();
                    //取出航班号
                    Regex re4 = new Regex(@"<b>.*?</b>", RegexOptions.Singleline);
                    if (re4.IsMatch(s.Value))
                    {
                        MatchCollection liCollection = re4.Matches(s.Value);
                        f.FliterNum = RemoveHTMLTags(liCollection[0].Value);
                    }
                    //取出起始地址
                    Regex re5 = new Regex(@"<span\sclass =""c3"">.*?</span>", RegexOptions.Singleline|RegexOptions.IgnorePatternWhitespace);
                    if (re5.IsMatch(s.Value))
                    {
                        MatchCollection liCollection = re5.Matches(s.Value);
                        string[] strAarry=liCollection[0].Value.Split(new string[] { "<br />" },StringSplitOptions.RemoveEmptyEntries);
                        //f.DepartureCity = 
                        f.FilterDeparture = RemoveHTMLTags(strAarry[0]);
                        //f.ArrivalCity = 
                        f.FilterArrival = RemoveHTMLTags(strAarry[1]);
                        f.SpliderTime = DateTime.Now;
                        data.Add(f);
                    }                    
                }               
            }
            return data;
        }

        /// <summary>
        /// 除去所有在html元素中标记
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static string RemoveHTMLTags(string html)
        {
            Regex regex = new Regex(@"<[^>]+>|</[^>]+>");
            return regex.Replace(html, "");
        }

    }

}
