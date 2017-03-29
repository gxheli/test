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

namespace LanghuaForSup.Controllers
{
    public class ProfileController : Controller
    {
        private ODataClient client = new ODataClient(ConfigurationManager.AppSettings["ServicePath"] + "odata/");
        public ActionResult Help()
        {
            return View();
        }
        //修改密码
        public async Task<ActionResult> EditPassWord()
        {
            string SupplierUserName = User.Identity.Name;
            SupplierUser user = await client.For<SupplierUser>()
                .Filter(s => s.SupplierUserName == SupplierUserName)
                .Select(s => new { s.SupplierUserName, s.SupplierUserID })
                .FindEntryAsync();

            return View(user);
        }
        //修改密码
        [HttpPost]
        public async Task<string> EditPassWord(int? SupplierUserID, string PassWord, string newPassWord)
        {
            if (SupplierUserID == null || SupplierUserID == 0)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "ID不能为空" });
            }
            if (string.IsNullOrEmpty(PassWord))
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "旧密码不能为空" });
            }
            if (string.IsNullOrEmpty(newPassWord))
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "新密码不能为空" });
            }
            string SupplierUserName = User.Identity.Name;
            SupplierUser user = await client.For<SupplierUser>()
                .Filter(s => s.SupplierUserName == SupplierUserName && s.SupplierUserID == SupplierUserID)
                .FindEntryAsync();
            if (user == null)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "操作异常" });
            }
            if (user.PassWord != Md5Hash(PassWord))
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "旧密码输入错误" });
            }

            user.PassWord = Md5Hash(newPassWord);
            user.UpdatePassWordTime = DateTimeOffset.Now;
            await client.For<SupplierUser>().Key(SupplierUserID).Set(user).UpdateEntryAsync();

            return JsonConvert.SerializeObject(new { ErrorCode = 200, ErrorMessage = "OK" });
        }
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> EditPassWord(int SupplierUserID, string PassWord, string newPassWord, string newPassWord2)
        //{
        //    string SupplierUserName = User.Identity.Name;
        //    SupplierUser user = await client.For<SupplierUser>()
        //        .Filter(s => s.SupplierUserName == SupplierUserName && s.SupplierUserID == SupplierUserID)
        //        .FindEntryAsync();

        //    if (user == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    if (string.IsNullOrEmpty(newPassWord) || newPassWord != newPassWord2)
        //    {
        //        ModelState.AddModelError("", "2次密码输入不一致");
        //    }
        //    else
        //    {
        //        if (string.IsNullOrEmpty(PassWord) || user.PassWord != Md5Hash(PassWord))
        //        {
        //            ModelState.AddModelError("PassWord", "！密码错误");
        //        }
        //        else
        //        {
        //            user.PassWord = Md5Hash(newPassWord);
        //            await client.For<SupplierUser>().Key(SupplierUserID).Set(user).UpdateEntryAsync();
        //            return RedirectToAction("LogOut", "langhua");
        //        }
        //    }

        //    ViewBag.PassWord = PassWord;
        //    ViewBag.newPassWord = newPassWord;
        //    ViewBag.newPassWord2 = newPassWord2;
        //    SupplierUser su = new SupplierUser();
        //    su.SupplierUserID = user.SupplierUserID;
        //    su.SupplierUserName = user.SupplierUserName;
        //    return View(su);
        //}

        //消息配置
        public async Task<ActionResult> MessageSetting()
        {
            string SupplierUserName = User.Identity.Name;
            SupplierUser user = await client.For<SupplierUser>()
                .Filter(s => s.SupplierUserName == SupplierUserName)
                .Select(s => new { s.SupplierUserID, s.SupplierUserName, s.OpenID, s.RealTimeMessage, s.SummaryMessage, s.Disturb, s.BeginTime, s.EndTime })
                .FindEntryAsync();
            ViewBag.ImageUrl = WeiXinHelper.GetImageUrlByID(user.SupplierUserID, systemType.supplier);
            return View(user);
        }

        //消息配置
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> MessageSetting(SupplierUser user)
        {

            string SupplierUserName = User.Identity.Name;
            SupplierUser one = await client.For<SupplierUser>()
                .Filter(s => s.SupplierUserName == SupplierUserName && s.SupplierUserID == user.SupplierUserID)
                .FindEntryAsync();
            ViewBag.ImageUrl = WeiXinHelper.GetImageUrlByID(one.SupplierUserID, systemType.supplier);

            if (one == null)
            {
                return HttpNotFound();
            }

            if (user.Disturb)
            {
                if (string.IsNullOrEmpty(user.BeginTime) || string.IsNullOrEmpty(user.EndTime))
                {
                    ViewBag.Message = "已选中免打扰时段，时间段不能为空";
                    return View(user);
                }
                else
                {
                    var begin = user.BeginTime.Split(':');
                    var end = user.EndTime.Split(':');
                    if (int.Parse(begin[0]) < 0 || int.Parse(begin[0]) >= 24 || int.Parse(end[0]) < 0 || int.Parse(end[0]) >= 24
                        || int.Parse(begin[1]) < 0 || int.Parse(begin[1]) >= 60 || int.Parse(end[1]) < 0 || int.Parse(end[1]) >= 60)
                    {
                        ViewBag.Message = "免打扰时段格式错误";
                        return View(user);
                    }
                    try
                    {
                        if (int.Parse(begin[0]) == int.Parse(end[0]) && int.Parse(begin[1]) == int.Parse(end[1]))
                        {
                            ViewBag.Message = "免打扰时段开始时间不能等于结束时间";
                            return View(user);
                        }
                    }
                    catch
                    {
                        ViewBag.Message = "免打扰时段格式错误";
                        return View(user);
                    }
                }
            }
            else
            {
                user.BeginTime = null;
                user.EndTime = null;
            }

            one.RealTimeMessage = user.RealTimeMessage;
            one.SummaryMessage = user.SummaryMessage;
            one.Disturb = user.Disturb;
            one.BeginTime = user.BeginTime;
            one.EndTime = user.EndTime;
            await client.For<SupplierUser>().Key(one.SupplierUserID).Set(one).UpdateEntryAsync();

            ViewBag.Save = "保存成功";
            return View(one);
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
    }
}
