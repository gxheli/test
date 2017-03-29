using LanghuaNew.Data;
using Simple.OData.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace LanghuaQRCode.Controllers
{
    public class HomeController : Controller
    {
        private ODataClient client = new ODataClient(ConfigurationManager.AppSettings["ServicePath"] + "odata/");
        public async Task<ActionResult> Index(string from)
        {
            try
            {
                QRcode code = QRcode.Other;
                if (!string.IsNullOrEmpty(from))
                {
                    switch (from.ToLower())
                    {
                        case "voucher":
                            code = QRcode.Voucher;
                            break;
                        case "email":
                            code = QRcode.Email;
                            break;
                        case "clothes":
                            code = QRcode.Clothes;
                            break;
                        case "trip":
                            code = QRcode.Trip;
                            break;
                    }
                }
                string UserAgent = Request.UserAgent;
                string IP = Request.UserHostAddress;
                DateTimeOffset CreateTime = DateTimeOffset.Now;
                await client.For<QRcodeStatistic>().Set(new
                {
                    Code = code,
                    UserAgent = UserAgent,
                    IP = IP,
                    CreateTime = CreateTime
                }).InsertEntryAsync();
            }
            catch { }
            return View();
        }
    }
}