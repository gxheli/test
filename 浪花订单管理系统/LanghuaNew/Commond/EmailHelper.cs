using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.IO;
using System.Configuration;

namespace Commond
{
    public class EmailHelper
    {
        public static string api_user = ConfigurationManager.AppSettings["api_user"];
        public static string api_key = ConfigurationManager.AppSettings["api_key"];
        public static string api_from = ConfigurationManager.AppSettings["api_from"];
        private static StreamContent createStream(String filePath, String paramKey, String fileName)
        {
            FileStream fs = File.OpenRead(filePath);
            StreamContent streamContent = new StreamContent(fs);
            streamContent.Headers.Add("Content-Type", "application/octet-stream");
            String headerValue = "form-data; name=\"" + paramKey + "\"; filename=\"" + fileName + "\"";
            byte[] bytes1 = Encoding.UTF8.GetBytes(headerValue);
            headerValue = "";
            foreach (byte b1 in bytes1)
            {
                headerValue += (Char)b1;
            }
            streamContent.Headers.Add("Content-Disposition", headerValue);

            return streamContent;
        }
        /// <summary>
        /// 文件名称规则：供应商编码-联系人姓名-联系人淘宝ID-产品中文名称
        /// </summary>
        /// <param name="title"></param>
        /// <param name="toMail"></param>
        /// <param name="customerName"></param>
        /// <param name="fileName"></param>
        /// <param name="template">confirmmail/bookingmail</param>
        public static async Task<string> send(string title, string toMail, string customerName, string fileName, string filePath, string template)
        {
            string xsmtpapi = string.Format("{{\"to\": [\"{0}\"], \"sub\" : {{ \"%Name%\" : [\"{1}\"], \"%FileName%\" : [\"{2}\"]}}}}", toMail, customerName, fileName);
            //String urlSend = "http://api.sendcloud.net/apiv2/mail/send";
            string url = "http://api.sendcloud.net/apiv2/mail/sendtemplate";

            //String api_user = "dodotour_test_TW53LY";
            //String api_key = "zZJRfZqjlt8pOk3O";

            HttpClient client = null;
            HttpResponseMessage response = null;

            try
            {
                client = new HttpClient();

                List<KeyValuePair<string, string>> paramList = new List<KeyValuePair<string, string>>();

                paramList.Add(new KeyValuePair<string, string>("apiUser", api_user));
                paramList.Add(new KeyValuePair<string, string>("apiKey", api_key));
                paramList.Add(new KeyValuePair<string, string>("from", api_from));
                paramList.Add(new KeyValuePair<string, string>("to", toMail));
                // paramList.Add(new KeyValuePair<string, string>("fromName", "浪花朵朵旅行社"));
                paramList.Add(new KeyValuePair<string, string>("xsmtpapi", xsmtpapi));
                paramList.Add(new KeyValuePair<string, string>("subject", title));
                paramList.Add(new KeyValuePair<string, string>("templateInvokeName", template));

                var multipartFormDataContent = new MultipartFormDataContent();
                foreach (var keyValuePair in paramList)
                {
                    multipartFormDataContent.Add(new StringContent(keyValuePair.Value), string.Format("\"{0}\"", keyValuePair.Key));
                }
                //multipartFormDataContent.Add(createStream(filePath, "attachments", fileName));
                using (FileStream fs = File.OpenRead(filePath))
                {
                    using (StreamContent streamContent = new StreamContent(fs))
                    {
                        streamContent.Headers.Add("Content-Type", "application/octet-stream");
                        String headerValue = "form-data; name=\"attachments\"; filename=\"" + fileName + "\"";
                        byte[] bytes1 = Encoding.UTF8.GetBytes(headerValue);
                        headerValue = "";
                        foreach (byte b1 in bytes1)
                        {
                            headerValue += (Char)b1;
                        }
                        streamContent.Headers.Add("Content-Disposition", headerValue);
                        multipartFormDataContent.Add(streamContent);
                        response = await client.PostAsync(url, multipartFormDataContent);
                    }
                }
                return response.Content.ReadAsStringAsync().Result;
            }
            catch
            {
                throw;
            }
            finally
            {
                if (null != client)
                {
                    client.Dispose();
                }
            }
        }
        /// <summary>
        /// 供应商修改信息通知
        /// </summary>
        public static async Task<string> SupplierAmend(string VoucherName, string OrderNo, string PickupTime, string TransferFee)
        {
            string toMail = ConfigurationManager.AppSettings["tomail"];
            if (string.IsNullOrEmpty(toMail))
            {
                return null;
            }
            string xsmtpapi = string.Format("{{\"to\": [\"{0}\"], \"sub\" : {{ \"%VoucherName%\" : [\"{1}\"], \"%OrderNo%\" : [\"{2}\"], \"%PickupTime%\" : [\"{3}\"], \"%TransferFee%\" : [\"{4}\"]}}}}", toMail, VoucherName, OrderNo, PickupTime, TransferFee);
            string url = "http://api.sendcloud.net/apiv2/mail/sendtemplate";
            HttpClient client = null;
            HttpResponseMessage response = null;
            try
            {
                client = new HttpClient();

                List<KeyValuePair<string, string>> paramList = new List<KeyValuePair<string, string>>();

                paramList.Add(new KeyValuePair<string, string>("apiUser", api_user));
                paramList.Add(new KeyValuePair<string, string>("apiKey", api_key));
                paramList.Add(new KeyValuePair<string, string>("from", api_from));
                paramList.Add(new KeyValuePair<string, string>("to", toMail));
                paramList.Add(new KeyValuePair<string, string>("xsmtpapi", xsmtpapi));
                paramList.Add(new KeyValuePair<string, string>("subject", "【供应商修改】" + VoucherName));
                paramList.Add(new KeyValuePair<string, string>("templateInvokeName", "SupplierAmend"));

                var multipartFormDataContent = new MultipartFormDataContent();
                foreach (var keyValuePair in paramList)
                {
                    multipartFormDataContent.Add(new StringContent(keyValuePair.Value), string.Format("\"{0}\"", keyValuePair.Key));
                }
                response = await client.PostAsync(url, multipartFormDataContent);
                return response.Content.ReadAsStringAsync().Result;
            }
            catch
            {
                throw;
            }
            finally
            {
                if (null != client)
                {
                    client.Dispose();
                }
            }
        }
        /// <summary>
        /// 价格变更时发邮件通知供应商
        /// </summary>
        /// <param name="title"></param>
        /// <param name="toMail"></param>
        /// <param name="ProductName"></param>
        /// <param name="ProductDetail"></param>
        /// <returns></returns>
        public static async Task<string> PriceWaitConfirm(string title, string toMail, string ProductName, string ProductDetail)
        {
            string xsmtpapi = string.Format("{{\"to\": [\"{0}\"], \"sub\" : {{ \"%ProductName%\" : [\"{1}\"], \"%ProductDetail%\" : [\"{2}\"]}}}}", toMail, ProductName, ProductDetail);
            //String urlSend = "http://api.sendcloud.net/apiv2/mail/send";
            string url = "http://api.sendcloud.net/apiv2/mail/sendtemplate";

            //String api_user = "dodotour_test_TW53LY";
            //String api_key = "zZJRfZqjlt8pOk3O";

            HttpClient client = null;
            HttpResponseMessage response = null;

            try
            {
                client = new HttpClient();

                List<KeyValuePair<string, string>> paramList = new List<KeyValuePair<string, string>>();

                paramList.Add(new KeyValuePair<string, string>("apiUser", api_user));
                paramList.Add(new KeyValuePair<string, string>("apiKey", api_key));
                paramList.Add(new KeyValuePair<string, string>("from", api_from));
                paramList.Add(new KeyValuePair<string, string>("to", toMail));
                // paramList.Add(new KeyValuePair<string, string>("fromName", "浪花朵朵旅行社"));
                paramList.Add(new KeyValuePair<string, string>("xsmtpapi", xsmtpapi));
                paramList.Add(new KeyValuePair<string, string>("subject", title));
                paramList.Add(new KeyValuePair<string, string>("templateInvokeName", "PriceWaitConfirm"));

                var multipartFormDataContent = new MultipartFormDataContent();
                foreach (var keyValuePair in paramList)
                {
                    multipartFormDataContent.Add(new StringContent(keyValuePair.Value), string.Format("\"{0}\"", keyValuePair.Key));
                }
                response = await client.PostAsync(url, multipartFormDataContent);
                return response.Content.ReadAsStringAsync().Result;

            }
            catch
            {
                throw;
            }
            finally
            {
                if (null != client)
                {
                    client.Dispose();
                }
            }
        }
        /// <summary>
        /// 供应商确认价格时 发邮件给产品
        /// </summary>
        /// <param name="title"></param>
        /// <param name="toMail"></param>
        /// <param name="ProductName"></param>
        /// <param name="ProductDetail"></param>
        /// <returns></returns>
        public static async Task<string> PriceChangeConfirm(string toMail, string ProductName, string Supplier, string Detail)
        {
            //string toMail = ConfigurationManager.AppSettings["confirmtomail"];
            string xsmtpapi = string.Format("{{\"to\": [\"{0}\"], \"sub\" : {{ \"%ProductName%\" : [\"{1}\"], \"%Supplier%\" : [\"{2}\"], \"%Detail%\" : [\"{3}\"]}}}}", toMail, ProductName, Supplier, Detail);
            //String urlSend = "http://api.sendcloud.net/apiv2/mail/send";
            string url = "http://api.sendcloud.net/apiv2/mail/sendtemplate";
            //String api_user = "dodotour_test_TW53LY";
            //String api_key = "zZJRfZqjlt8pOk3O";

            HttpClient client = null;
            HttpResponseMessage response = null;

            try
            {
                client = new HttpClient();

                List<KeyValuePair<string, string>> paramList = new List<KeyValuePair<string, string>>();

                paramList.Add(new KeyValuePair<string, string>("apiUser", api_user));
                paramList.Add(new KeyValuePair<string, string>("apiKey", api_key));
                paramList.Add(new KeyValuePair<string, string>("from", api_from));
                paramList.Add(new KeyValuePair<string, string>("to", toMail));
                paramList.Add(new KeyValuePair<string, string>("fromName", "浪花朵朵旅行"));
                paramList.Add(new KeyValuePair<string, string>("xsmtpapi", xsmtpapi));
                //paramList.Add(new KeyValuePair<string, string>("subject", title));
                paramList.Add(new KeyValuePair<string, string>("templateInvokeName", "PriceChangeConfirm"));

                var multipartFormDataContent = new MultipartFormDataContent();
                foreach (var keyValuePair in paramList)
                {
                    multipartFormDataContent.Add(new StringContent(keyValuePair.Value), string.Format("\"{0}\"", keyValuePair.Key));
                }
                response = await client.PostAsync(url, multipartFormDataContent);
                return response.Content.ReadAsStringAsync().Result;

            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                if (null != client)
                {
                    client.Dispose();
                }
            }
        }
    }
}
