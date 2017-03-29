using Commond;
using LanghuaNew.Data;
using Simple.OData.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace LanghuaForCus.Controllers
{
    public class WeixinController : BaseController
    {
        private ODataClient client = new ODataClient(ConfigurationManager.AppSettings["ServicePath"] + "odata/");
        // GET: Weixin
        public ActionResult Index()
        {
            return View();
        }
        // GET: Weixin/weixinbind 微信菜单点击微信绑定
        [AllowAnonymous]
        public async Task<ActionResult> weixinbind(string code, string state)
        {
            if (string.IsNullOrEmpty(code))
            {
                return Content("您拒绝了授权！");
            }
            if (state != "User")
            {
                return Content("验证失败！请从正规途径进入！");
            }
            string OpenID = string.Empty;
            //用code换取access_token
            try
            {
                OpenID = WeiXinHelper.GetOpenID(code);
            }
            catch
            {

            }
            if (string.IsNullOrEmpty(OpenID))
            {
                return Content("页面失效，请重试");
            }
            Customer customer = await client.For<Customer>().Filter(t => t.OpenID == OpenID).FindEntryAsync();
            if (customer == null)
            {
                return RedirectToAction("unbound", "Weixin", new { arg0 = EncryptHelper.Encode(OpenID) });
            }
            else
            {
                return RedirectToAction("bound", "Weixin", new { arg0 = EncryptHelper.Encode(OpenID) });
            }
        }
        // GET: Weixin/weixinpay 微信菜单点击微信付款
        [AllowAnonymous]
        public ActionResult weixinpay(string code, string state)
        {
            return View();
        }
        // GET: Weixin/contact 人工客服
        [AllowAnonymous]
        public ActionResult contact(string code, string state)
        {
            return View();
        }
        // GET: Weixin/myorder 微信菜单点击我的订单
        [AllowAnonymous]
        public async Task<ActionResult> myorder(string code, string state)
        {
            if (string.IsNullOrEmpty(code))
            {
                return Content("您拒绝了授权！");
            }
            if (state != "User")
            {
                return Content("验证失败！请从正规途径进入！");
            }
            string OpenID = string.Empty;
            //用code换取access_token
            try
            {
                OpenID = WeiXinHelper.GetOpenID(code);
            }
            catch
            {

            }
            if (string.IsNullOrEmpty(OpenID))
            {
                return Content("页面失效，请重试");
            }
            Customer customer = await client.For<Customer>().Filter(t => t.OpenID == OpenID).FindEntryAsync();
            if (customer == null)
            {
                return RedirectToAction("unbound", "Weixin", new { arg0 = EncryptHelper.Encode(OpenID) });
            }
            else
            {
                login(customer.CustomerTBCode);
                return RedirectToAction("Index", "Orders");
            }
        }
        // GET: Weixin/myorder 微信菜单点击个人中心
        [AllowAnonymous]
        public async Task<ActionResult> myhome(string code, string state)
        {
            if (string.IsNullOrEmpty(code))
            {
                return Content("您拒绝了授权！");
            }
            if (state != "User")
            {
                return Content("验证失败！请从正规途径进入！");
            }
            string OpenID = string.Empty;
            //用code换取access_token
            try
            {
                OpenID = WeiXinHelper.GetOpenID(code);
            }
            catch
            {

            }
            if (string.IsNullOrEmpty(OpenID))
            {
                return Content("页面失效，请重试");
            }
            Customer customer = await client.For<Customer>().Filter(t => t.OpenID == OpenID).FindEntryAsync();
            if (customer == null)
            {
                return RedirectToAction("unbound", "Weixin", new { arg0 = EncryptHelper.Encode(OpenID) });
            }
            else
            {
                login(customer.CustomerTBCode);
                return RedirectToAction("Index", "Home");
            }
        }
        // GET: Weixin/myorder 微信推送点击订单详情
        [AllowAnonymous]
        public async Task<ActionResult> orderdetail(string code, string state)
        {
            if (string.IsNullOrEmpty(code))
            {
                return Content("您拒绝了授权！");
            }
            int orderid = 0;
            try
            {
                orderid = int.Parse(state);
            }
            catch (Exception)
            {

            }
            if (orderid == 0)
            {
                return Content("验证失败！请从正规途径进入！");
            }
            string OpenID = string.Empty;
            //用code换取access_token
            try
            {
                OpenID = WeiXinHelper.GetOpenID(code);
            }
            catch
            {

            }
            if (string.IsNullOrEmpty(OpenID))
            {
                return Content("页面失效，请重试");
            }
            Customer customer = await client.For<Customer>().Filter(t => t.OpenID == OpenID).FindEntryAsync();
            if (customer == null)
            {
                return RedirectToAction("unbound", "Weixin", new { arg0 = EncryptHelper.Encode(OpenID) });
            }
            else
            {
                //login(customer.CustomerTBCode);
                return RedirectToAction("Details", "Orders", new { id = EncryptHelper.Encrypt(orderid.ToString()) });
            }
        }
        //微信已绑定
        [AllowAnonymous]
        public async Task<ActionResult> bound(string arg0)
        {
            if (string.IsNullOrEmpty(arg0))
            {
                return Content("页面失效，请重试");
            }
            string OpenID = EncryptHelper.Decode(arg0);
            Customer customer = await client.For<Customer>().Filter(t => t.OpenID == OpenID).FindEntryAsync();
            if (customer == null)
            {
                return RedirectToAction("unbound", "Weixin", new { arg0 = arg0 });
            }
            ViewBag.arg0 = arg0;
            return View();
        }
        //进入查看我的订单
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> bind(string arg0)
        {
            string OpenID = EncryptHelper.Decode(arg0);
            Customer customer = await client.For<Customer>().Filter(t => t.OpenID == OpenID).FindEntryAsync();
            if (customer == null)
            {
                return RedirectToAction("unbound", "Weixin", new { arg0 = arg0 });
            }
            else
            {
                login(customer.CustomerTBCode);
                return RedirectToAction("Index", "Orders");
            }
        }
        //微信未绑定
        [AllowAnonymous]
        public async Task<ActionResult> unbound(string arg0)
        {
            if (string.IsNullOrEmpty(arg0))
            {
                return Content("页面失效，请重试");
            }
            string OpenID = EncryptHelper.Decode(arg0);
            Customer customer = await client.For<Customer>().Filter(t => t.OpenID == OpenID).FindEntryAsync();
            if (customer != null)
            {
                return RedirectToAction("bound", "Weixin", new { arg0 = arg0 });
            }
            ViewBag.arg0 = arg0;
            return View();
        }
        //绑定微信
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> unbound(string arg0, string TBID, string Tel)
        {
            if (string.IsNullOrEmpty(arg0))
            {
                return Content("页面失效，请重试");
            }
            if (string.IsNullOrEmpty(TBID) || string.IsNullOrEmpty(Tel))
            {
                ViewBag.Message = "淘宝ID、联系电话不能为空!";
            }
            else
            {
                Customer customer = await client.For<Customer>().Filter(t => t.CustomerTBCode == TBID && (t.Tel == Tel || t.BakTel == Tel)).FindEntryAsync();
                if (customer == null)
                {
                    ViewBag.Message = "淘宝ID或联系电话输入有误，请重新输入!";
                }
                else
                {
                    if (customer.OpenID != null && customer.OpenID != "" && customer.OpenID != EncryptHelper.Decode(arg0))
                    {
                        ViewBag.Message = "您的淘宝ID已与其它微信进行绑定，如需绑定新的微信，请用该淘宝ID联系旺旺客服进行解绑后再重新绑定。";
                    }
                    else
                    {
                        customer.OpenID = EncryptHelper.Decode(arg0);
                        await client.For<Customer>().Key(customer.CustomerID).Set(customer).UpdateEntryAsync();
                        //为用户设置备注
                        WeiXinHelper.UpdateUserRemark(customer.OpenID, customer.CustomerTBCode);
                        return RedirectToAction("bound", "Weixin", new { arg0 = arg0 });
                    }
                }
            }

            ViewBag.arg0 = arg0;
            ViewBag.TBID = TBID;
            ViewBag.Tel = Tel;
            return View();
        }
        //登录
        private void login(string CustomerTBCode)
        {
            FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(
            1,
            CustomerTBCode,
            DateTime.Now,
            DateTime.MaxValue,
            false,
            ""
            );
            string encryptedTicket = FormsAuthentication.Encrypt(authTicket);
            HttpCookie authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
            System.Web.HttpContext.Current.Response.Cookies.Add(authCookie);
        }
    }
}
