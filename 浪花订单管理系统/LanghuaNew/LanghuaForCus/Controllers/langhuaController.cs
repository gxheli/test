using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using LanghuaNew.Data;
using System.Web.Security;
using Simple.OData.Client;
using System.Configuration;
using System.Security.Cryptography;
using System.Text;
using Commond.Captcha;
using Newtonsoft.Json;

namespace LanghuaForCus.Controllers
{
    public class langhuaController : BaseController
    {
        private ODataClient Dbclient = new ODataClient(ConfigurationManager.AppSettings["ServicePath"] + "odata/");
        //通过链接登录
        [AllowAnonymous]
        public ActionResult client(string id)
        {
            try
            {
                int.Parse(id);
            }
            catch
            {
                ModelState.AddModelError("", "链接有误，请检查链接或联系客服。http://my.dodotour.cn/langhua/client/" + id);
                ViewBag.isVaild = true;
            }
            ViewBag.TBOrderID = id;
            return View();
        }
        //用户名密码登录
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }
        //退出
        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            Response.Cookies["loginnum"].Expires = DateTime.Now.AddDays(-1);
            return RedirectToAction("Login");
        }
        //通过链接登录
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> client([Bind(Include = "CustomerTBCode")]Customer customer, string TBOrderID, string Code)
        {
            int counter = 0;
            try
            {
                int.Parse(TBOrderID);
            }
            catch
            {
                //如果TBOrderID不是int类型，则提示错误
                ModelState.AddModelError("", "链接有误，请检查链接或联系客服。http://my.dodotour.cn/langhua/client/" + TBOrderID);
                //ViewBag.isVaild为true时用户名和密码框隐藏
                ViewBag.isVaild = true;
                ViewBag.TBOrderID = TBOrderID;
                return View(customer);
            }
            if (ModelState.IsValidField("CustomerTBCode"))
            {
                try
                {
                    //int TBOrderID = int.Parse(Request.QueryString["TBOrderID"]);
                    int id = int.Parse(TBOrderID);
                    TBOrder tBOrder = await Dbclient.For<TBOrder>()
                        .Filter(u => u.TBOrderID == id && u.TBID == customer.CustomerTBCode.Trim())
                        .FindEntryAsync();
                    if (tBOrder == null)
                    {
                        ModelState.AddModelError("", "淘宝ID输入错误");
                        counter=SetClientLoginFaildTimes();
                    }
                    else if (Code != null && Session["ValidateCode"] != null && Code.ToLower() != Session["ValidateCode"].ToString().ToLower())
                    {
                        ModelState.AddModelError("", "验证码不正确.");
                        counter = SetClientLoginFaildTimes();
                    }
                    else
                    {
                        FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(
                        1,
                        tBOrder.TBID,
                        DateTime.Now,
                        DateTime.MaxValue,
                        false,
                        ""
                        );
                        string encryptedTicket = FormsAuthentication.Encrypt(authTicket);
                        HttpCookie authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
                        System.Web.HttpContext.Current.Response.Cookies.Add(authCookie);
                        //await SaveLoginInfo(tBOrder.TBID, int.Parse(TBOrderID));
                        //Session["loginnum"] = null;
                        Customer cus = await Dbclient.For<Customer>()
                            .Filter(c => c.CustomerTBCode == tBOrder.TBID).FindEntryAsync();
                        await Dbclient.For<CustomerLog>().Set(new CustomerLog
                        {
                            Operate = "客人链接登录：" + TBOrderID,
                            CustomerID = cus.CustomerID,
                            OperID = cus.CustomerID.ToString(),
                            OperName = cus.CustomerTBCode,
                            OperTime = DateTimeOffset.Now,
                            Remark = "客户端：" + Request.UserAgent + "<br/>登录IP：" + Request.UserHostAddress
                        }).InsertEntryAsync();
                        return RedirectToAction("OrderNew", "TBOrders", new { id = int.Parse(TBOrderID) });
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            else
            {
                ModelState.AddModelError("", "淘宝ID不能为空");
                counter = SetClientLoginFaildTimes();
            }
            ViewBag.TBOrderID = TBOrderID;
            ViewBag.Counter2 = counter;
            return View(customer);
        }
        //用户名密码登录
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login([Bind(Include = "CustomerTBCode,Password")]Customer customer, string returnUrl, string Code)
        {
            int counter = 0;
            if (ModelState.IsValidField("CustomerTBCode") && ModelState.IsValidField("Password"))
            {
                try
                {
                    if (string.IsNullOrEmpty(customer.Password))
                    {
                        ModelState.AddModelError("", "密码不能为空");
                        counter=SetDefaultLoginFaildTimes();
                    }
                    else if (Code != null && Session["ValidateCode"] != null && Code.ToLower() != Session["ValidateCode"].ToString().ToLower())
                    {
                        ModelState.AddModelError("", "验证码不正确.");
                        counter = SetDefaultLoginFaildTimes();
                    }
                    else
                    {
                        Customer nullCustomer = await Dbclient.For<Customer>()
                            .Filter(u => u.CustomerTBCode == customer.CustomerTBCode.Trim() && (u.Password == "" || u.Password == null))
                            .FindEntryAsync();
                        if (nullCustomer != null)
                        {
                            ModelState.AddModelError("", "链接有误，请重新跟客服索要链接。");
                        }
                        else
                        {
                            string PassHash = Md5Hash(customer.Password);
                            Customer loginCustomer = await Dbclient.For<Customer>()
                                .Filter(u => u.CustomerTBCode == customer.CustomerTBCode.Trim() && u.Password == PassHash)
                                .FindEntryAsync();
                            if (loginCustomer == null)
                            {
                                ModelState.AddModelError("", "淘宝ID或密码错误");
                                counter = SetDefaultLoginFaildTimes();
                            }
                            else
                            {
                                FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(
                                1,
                                loginCustomer.CustomerTBCode,
                                DateTime.Now,
                                DateTime.MaxValue,
                                false,
                                ""
                                );
                                string encryptedTicket = FormsAuthentication.Encrypt(authTicket);
                                HttpCookie authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
                                System.Web.HttpContext.Current.Response.Cookies.Add(authCookie);
                                //Session["loginnum2"] = null;
                                //await SaveLoginInfo(loginCustomer.CustomerTBCode);
                                await Dbclient.For<CustomerLog>().Set(new CustomerLog
                                {
                                    Operate = "客人登录",
                                    CustomerID = loginCustomer.CustomerID,
                                    OperID = loginCustomer.CustomerID.ToString(),
                                    OperName = loginCustomer.CustomerTBCode,
                                    OperTime = DateTimeOffset.Now,
                                    Remark = "客户端：" + Request.UserAgent + "<br/>登录IP：" + Request.UserHostAddress
                                }).InsertEntryAsync();

                                if (!string.IsNullOrEmpty(returnUrl))
                                {
                                    return RedirectToLocal(returnUrl);
                                }
                                else
                                {
                                    return RedirectToAction("Index", "Home");
                                }
                            }
                        }
                    }
                }
                catch(Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            ViewBag.Counter = counter;
            return View(customer);
        }
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "TBOrders");
            }
        }
        //加密
        private static string Md5Hash(string input)
        {
            MD5CryptoServiceProvider md5Hasher = new MD5CryptoServiceProvider();
            byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(input));
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }
        //获取验证码
        [AllowAnonymous]
        public ActionResult GetValidateCode()
        {
            ValidateCode vCode = new ValidateCode();
            string code = vCode.CreateValidateCode(4);
            Session["ValidateCode"] = code;
            byte[] bytes = vCode.CreateValidateGraphic(code);
            return File(bytes, @"image/jpeg");
        }
        /// <summary>
        /// 检查输入的验证码是否正确
        /// </summary>
        /// <param name="str">验证码参数</param>
        /// <returns>msg</returns>
        [AllowAnonymous]
        public string CheckValidateCode()
        {
            string valiCode = this.Request.QueryString["str"] ?? string.Empty;
            try
            {
                if (string.IsNullOrEmpty(valiCode))
                {
                    return JsonConvert.SerializeObject(new { ErrorCode = 400, ErrorMessage = "验证码不能为空", data = "0" });
                }
                else
                {
                    if (Session["ValidateCode"] != null && valiCode.ToLower() == Session["ValidateCode"].ToString().ToLower())
                    {
                        return JsonConvert.SerializeObject(new { ErrorCode = 200, ErrorMessage = "OK", data = "1" });
                    }
                    else
                    {
                        return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "验证码不匹配", data = "0" });
                    }
                }
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 400, ErrorMessage = ex, data = "0" });
            }
        }

        public int SetDefaultLoginFaildTimes()
        {
            int ctimes = 0;
            if (Session["loginnum2"] == null)
            {
                ctimes = 1;
                Session["loginnum2"] = ctimes;
                Session.Timeout = 10;
            }
            else
            {
                ctimes = Int32.Parse(Session["loginnum2"].ToString()) + 1;
                Session["loginnum2"] = ctimes;
            }
            return ctimes;
        }

        public int SetClientLoginFaildTimes()
        {
            int ctimes = 0;
            if (Session["loginnum"] == null)
            {
                ctimes = 1;
                Session["loginnum"] = ctimes;
                Session.Timeout = 10;
            }
            else
            {
                ctimes= Int32.Parse(Session["loginnum"].ToString()) + 1;
                Session["loginnum"] = ctimes;
            }
            return ctimes;
        }
    }
}
