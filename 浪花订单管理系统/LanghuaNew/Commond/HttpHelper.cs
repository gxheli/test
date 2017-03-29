using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Commond
{
    public class HttpHelper
    {
        public static string BasePath = ConfigurationManager.AppSettings["ServicePath"];
        public static string WeixinPath = ConfigurationManager.AppSettings["WeixinPath"];

        /// <summary>
        /// WEBAPI接口
        /// </summary>
        /// <param name="ActionName"></param>
        /// <param name="PostData"></param>
        /// <returns></returns>
        public static async Task<HttpResponseMessage> PostAction(string ActionName,string PostData)
        {
            string url = BasePath + "api/" + ActionName;
            var handler = new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.GZip };

            using (var http = new HttpClient(handler))
            {
                HttpResponseMessage Message = await http.PostAsync(url, new FormUrlEncodedContent(new Dictionary<string, string>()
                {
                    { "",PostData }
                })).ConfigureAwait(false);
                return Message;
            }

        }
        /// <summary>
        /// 大数据Post WEBAPI接口
        /// </summary>
        /// <param name="ActionName"></param>
        /// <param name="PostData"></param>
        /// <returns></returns>
        public static async Task<HttpResponseMessage> BigDataPostAction(string ActionName, string PostData)
        {
            string url = BasePath + "api/" + ActionName;
            var handler = new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.GZip };

            using (var http = new HttpClient(handler))
            {
                HttpResponseMessage Message = await http.PostAsync(url, new StringContent(PostData, Encoding.UTF8, "application/json")).ConfigureAwait(false);
                return Message;
            }

        }
        /// <summary>
        /// odata接口
        /// </summary>
        /// <param name="ActionUrl"></param>
        /// <returns></returns>
        public static async Task<string> GetActionForOdata(string ActionUrl)
        {
            string url = BasePath + ActionUrl;
            var handler = new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.GZip };

            using (var http = new HttpClient(handler))
            {
                HttpResponseMessage Message = await http.GetAsync(url).ConfigureAwait(false);
                return Message.Content.ReadAsStringAsync().Result;
            }

        }
        /// <summary>
        /// webapi接口
        /// </summary>
        /// <param name="ActionName"></param>
        /// <param name="PostData"></param>
        /// <returns></returns>
        public static async Task<HttpResponseMessage> PutAction(string ActionName, string PostData)
        {
            string url = BasePath + "api/" + ActionName;
            var handler = new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.GZip };

            using (var http = new HttpClient(handler))
            {
                HttpResponseMessage Message = await http.PutAsync(url, new FormUrlEncodedContent(new Dictionary<string, string>()
                {
                    { "",PostData }
                })).ConfigureAwait(false);
                //HttpResponseMessage Message = await http.PutAsync(url, new StringContent(PostData, Encoding.UTF8, "application/json")).ConfigureAwait(false);
                return Message;
            }
          

        }

        /// <summary>
        /// webapi接口
        /// </summary>
        /// <param name="ActionName"></param>
        /// <returns></returns>
        public static async Task<HttpResponseMessage> GetAction(string ActionName)
        {
            string url = BasePath + "api/" + ActionName;
            var handler = new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.GZip };

            using (var http = new HttpClient(handler))
            {
                HttpResponseMessage Message = await http.GetAsync(url).ConfigureAwait(false);
                return Message;
            }

        }
        /// <summary>
        /// 微信webapi接口
        /// </summary>
        /// <param name="ActionName"></param>
        /// <returns></returns>
        public static async Task<HttpResponseMessage> GetActionForWeixin(string ActionName)
        {
            string url = WeixinPath + "api/" + ActionName;
            var handler = new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.GZip };

            using (var http = new HttpClient(handler))
            {
                HttpResponseMessage Message = await http.GetAsync(url).ConfigureAwait(false);
                return Message;
            }

        }
        /// <summary>
        /// 微信webapi接口
        /// </summary>
        /// <param name="ActionName"></param>
        /// <returns></returns>
        public static async Task<HttpResponseMessage> PostActionForWeixin(string ActionName, string PostData = "")
        {
            string url = WeixinPath + "api/" + ActionName;
            var handler = new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.GZip };

            using (var http = new HttpClient(handler))
            {
                HttpResponseMessage Message = await http.PostAsync(url, new FormUrlEncodedContent(new Dictionary<string, string>()
                {
                    { "",PostData }
                })).ConfigureAwait(false);
                return Message;
            }

        }
        /// <summary>
        /// 微信webapi接口
        /// </summary>
        /// <param name="ActionName"></param>
        /// <returns></returns>
        public static async Task<HttpResponseMessage> PutActionForWeixin(string ActionName, string PostData="")
        {
            string url = WeixinPath + "api/" + ActionName;
            var handler = new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.GZip };

            using (var http = new HttpClient(handler))
            {
                HttpResponseMessage Message = await http.PutAsync(url, new FormUrlEncodedContent(new Dictionary<string, string>()
                {
                    { "",PostData }
                })).ConfigureAwait(false);
                return Message;
            }

        }
        /// <summary>
        /// Url转码
        /// </summary>
        /// <param name="parameter">参数</param>
        /// <returns></returns>
        public static string UrlEncode(string parameter)
        {
            //1. % 指定特殊字符 %25
            //2.空格 URL中的空格可以用 + 号或者编码 %20
            //3. / 分隔目录和子目录 %2F
            //4. ? 分隔实际的 URL 和参数 %3F
            //5. + URL 中 + 号表示空格 %2B
            //6. # 表示书签 %23  
            //7. & URL 中指定的参数间的分隔符 %26
            //8. = URL 中指定参数的值 %3D
            return parameter.Trim()
                .Replace("%", "%25")
                .Replace(" ", "%20")
                .Replace("/", "%2F")
                .Replace("?", "%3F")
                .Replace("+", "%2B")
                .Replace("#", "%23")
                .Replace("&", "%26")
                .Replace("=", "%3D");
        }
    }
}
