using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace WeiXinWeb.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var qrResult = Senparc.Weixin.MP.AdvancedAPIs.QrCodeApi.Create(WebConfigurationManager.AppSettings["WeixinAppId"], 10000, 150);

            var qrCodeUrl = Senparc.Weixin.MP.AdvancedAPIs.QrCodeApi.GetShowQrCodeUrl(qrResult.ticket);

            ViewData["QrCodeUrl"] = qrCodeUrl;
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}