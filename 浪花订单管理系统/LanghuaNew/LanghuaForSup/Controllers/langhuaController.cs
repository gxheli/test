using Commond;
using LanghuaNew.Data;
using Newtonsoft.Json;
using Simple.OData.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace LanghuaForSup.Controllers
{
    public class langhuaController : Controller
    {
        private ODataClient client = new ODataClient(ConfigurationManager.AppSettings["ServicePath"] + "odata/");
        [AllowAnonymous]
        public async Task<ActionResult> Login(string returnUrl)
        {
            SupplierUser loginSupplier = await client.For<SupplierUser>().FindEntryAsync();
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }
        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }
        //用户名密码登录
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> Login([Bind(Include = "SupplierUserName,PassWord")]SupplierUser supplier, string returnUrl)
        {
            if (ModelState.IsValidField("SupplierUserName") && ModelState.IsValidField("PassWord"))
            {
                try
                {
                    string PassHash = Md5Hash(supplier.PassWord);
                    SupplierUser loginSupplier = await client.For<SupplierUser>()
                        .Filter(u => u.SupplierUserName == supplier.SupplierUserName && u.PassWord == PassHash)
                        .Filter(u => u.SupplierUserEnableState == EnableState.Enable)
                        .Filter(u => u.OneSupplier.EnableOnline)
                        .Filter(u => u.OneSupplier.SupplierEnableState == EnableState.Enable)
                        .FindEntryAsync();

                    if (loginSupplier == null)
                    {
                        ModelState.AddModelError("", "用户名或密码错误");
                    }
                    else
                    {
                        //修改密码时间超过90天强制要求改密码
                        if (loginSupplier.UpdatePassWordTime.AddDays(90) < DateTimeOffset.Now)
                        {
                            Session["UserName"] = loginSupplier.SupplierUserName;
                            Session["PassWord"] = loginSupplier.PassWord;
                            return RedirectToAction("UpdatePassWord", "langhua");
                        }
                        else
                        {
                            FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(
                            1,
                            supplier.SupplierUserName,
                            DateTime.Now,
                            DateTime.MaxValue,
                            false,
                            ""
                            );
                            string encryptedTicket = FormsAuthentication.Encrypt(authTicket);
                            HttpCookie authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
                            System.Web.HttpContext.Current.Response.Cookies.Add(authCookie);

                            loginSupplier.LastLoginTime = DateTimeOffset.Now;
                            loginSupplier.IP = Request.UserHostAddress;
                            await client.For<SupplierUser>().Key(loginSupplier.SupplierUserID).Set(loginSupplier).UpdateEntryAsync();
                            await client.For<SupplierUserLog>().Set(new SupplierUserLog
                            {
                                OperSupplierUserID = loginSupplier.SupplierUserID,
                                OperSupplierNickName = loginSupplier.SupplierNickName,
                                OperTime = DateTime.Now,
                                Remark = "登录IP：" + Request.UserHostAddress,
                                Operate = UserOperate.Login,
                                SupplierUserID = loginSupplier.SupplierUserID,
                                SupplierUserName = loginSupplier.SupplierUserName,
                                SupplierNickName = loginSupplier.SupplierNickName
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
                catch
                {
                    //ModelState.AddModelError("", ex.Message);
                }
            }
            return View(supplier);
        }
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
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
        //修改密码
        [AllowAnonymous]
        public ActionResult UpdatePassWord()
        {
            if (Session["UserName"] == null || Session["PassWord"] == null)
            {
                return RedirectToAction("Login", "langhua");
            }
            return View();
        }
        //修改密码
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> UpdatePassWord(string newPassWord, string newPassWord2)
        {
            ViewBag.newPassWord = newPassWord;
            ViewBag.newPassWord2 = newPassWord2;
            if (Session["UserName"] == null || Session["PassWord"] == null)
            {
                return RedirectToAction("Login", "langhua");
            }
            string UserName = Session["UserName"].ToString();
            string PassWord = Session["PassWord"].ToString();
            SupplierUser loginSupplier = await client.For<SupplierUser>()
                .Filter(u => u.SupplierUserName == UserName && u.PassWord == PassWord)
                .Filter(u => u.SupplierUserEnableState == EnableState.Enable)
                .Filter(u => u.OneSupplier.EnableOnline)
                .Filter(u => u.OneSupplier.SupplierEnableState == EnableState.Enable)
                .FindEntryAsync();
            if (loginSupplier == null)
            {
                return RedirectToAction("Login", "langhua");
            }
            if (string.IsNullOrEmpty(newPassWord))
            {
                ModelState.AddModelError("", "请输入新密码");
                return View();
            }
            if (string.IsNullOrEmpty(newPassWord2))
            {
                ModelState.AddModelError("", "请重新输入新密码");
                return View();
            }
            if (newPassWord != newPassWord2)
            {
                ModelState.AddModelError("", "2次密码不一致");
                return View();
            }
            if (Session["PassWord"].ToString() == Md5Hash(newPassWord))
            {
                ModelState.AddModelError("", "新密码与上一次密码相同");
                return View();
            }
            if (newPassWord.Length < 6 || newPassWord.Length > 12)
            {
                ModelState.AddModelError("", "密码必须6-12位数");
                return View();
            }
            FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(
                        1,
                        loginSupplier.SupplierUserName,
                        DateTime.Now,
                        DateTime.MaxValue,
                        false,
                        ""
                        );
            string encryptedTicket = FormsAuthentication.Encrypt(authTicket);
            HttpCookie authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
            System.Web.HttpContext.Current.Response.Cookies.Add(authCookie);

            loginSupplier.LastLoginTime = DateTime.Now;
            loginSupplier.UpdatePassWordTime = DateTime.Now;
            loginSupplier.PassWord = Md5Hash(newPassWord);
            await client.For<SupplierUser>().Key(loginSupplier.SupplierUserID).Set(loginSupplier).UpdateEntryAsync();
            //操作记录
            await client.For<SupplierUserLog>().Set(new SupplierUserLog
            {
                OperSupplierUserID = loginSupplier.SupplierUserID,
                OperSupplierNickName = loginSupplier.SupplierNickName,
                OperTime = DateTime.Now,
                Remark = "登录IP：" + Request.UserHostAddress,
                Operate = UserOperate.Login,
                SupplierUserID = loginSupplier.SupplierUserID,
                SupplierUserName = loginSupplier.SupplierUserName,
                SupplierNickName = loginSupplier.SupplierNickName
            }).InsertEntryAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
