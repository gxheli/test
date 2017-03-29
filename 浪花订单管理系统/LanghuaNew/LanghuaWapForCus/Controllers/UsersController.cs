using Commond;
using Commond.Captcha;
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

namespace LanghuaWapForCus.Controllers
{
    public class UsersController : Controller
    {
        private ODataClient client = new ODataClient(ConfigurationManager.AppSettings["ServicePath"] + "odata/");
        //用户名密码登录
        [HttpPost]
        [AllowAnonymous]
        public async Task<string> Login(string UserName, string PassWord, string Code)
        {
            if (string.IsNullOrEmpty(UserName))
            {
                SetDefaultLoginFaildTimes();
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "用户名不能为空" });
            }
            if (string.IsNullOrEmpty(PassWord))
            {
                SetDefaultLoginFaildTimes();
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "密码不能为空" });
            }
            if (Session["FaildTimes"] != null && int.Parse(Session["FaildTimes"].ToString()) > 3)
            {
                if (Code == null || Session["ValidateCode"] == null || Code.ToLower() != Session["ValidateCode"].ToString().ToLower())
                {
                    SetDefaultLoginFaildTimes();
                    return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "验证码不正确" });
                }
            }
            string PassHash = Md5Hash(PassWord);
            Customer loginCustomer = await client.For<Customer>()
                .Filter(u => u.CustomerTBCode == UserName.Trim() && u.Password == PassHash)
                .FindEntryAsync();
            if (loginCustomer == null)
            {
                SetDefaultLoginFaildTimes();
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "用户名或密码错误" });
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
                await client.For<CustomerLog>().Set(new CustomerLog
                {
                    Operate = "客人登录",
                    CustomerID = loginCustomer.CustomerID,
                    OperID = loginCustomer.CustomerID.ToString(),
                    OperName = loginCustomer.CustomerTBCode,
                    OperTime = DateTimeOffset.Now,
                    Remark = "客户端：" + Request.UserAgent + "<br/>登录IP：" + Request.UserHostAddress
                }).InsertEntryAsync();
            }
            return JsonConvert.SerializeObject(new { ErrorCode = 200, ErrorMessage = "OK" });
        }
        //获取验证码图片
        [AllowAnonymous]
        public ActionResult GetValidateCode()
        {
            if (Session["FaildTimes"] != null && int.Parse(Session["FaildTimes"].ToString()) > 3)
            {
                ValidateCode vCode = new ValidateCode();
                string code = vCode.CreateValidateCode(4);
                Session["ValidateCode"] = code;
                byte[] bytes = vCode.CreateValidateGraphic(code);
                return File(bytes, @"image/jpeg");
            }
            else
            {
                return null;
            }
        }
        //检查输入的验证码是否正确
        [HttpPost]
        [AllowAnonymous]
        public string CheckValidateCode(string str)
        {
            try
            {
                if (string.IsNullOrEmpty(str))
                {
                    return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "验证码不能为空" });
                }
                else
                {
                    if (Session["ValidateCode"] != null && str.ToLower() == Session["ValidateCode"].ToString().ToLower())
                    {
                        return JsonConvert.SerializeObject(new { ErrorCode = 200, ErrorMessage = "OK" });
                    }
                    else
                    {
                        return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "验证码不匹配" });
                    }
                }
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 400, ErrorMessage = ex.Message });
            }
        }
        //获取用户信息
        [HttpPost]
        public async Task<string> GetUserInfo()
        {
            string UserName = User.Identity.Name;
            Customer customer = await client.For<Customer>()
                .Expand(u => u.Travellers)
                .Filter(u => u.CustomerTBCode == UserName).FindEntryAsync();
            if (customer == null)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "登录异常，请重新登录" });
            }
            var UserInfo = new
            {
                customer.CustomerID,
                EncryptCustomerID = EncryptHelper.Encrypt(customer.CustomerID.ToString()),
                customer.BakTel,
                customer.CustomerEnname,
                customer.CustomerName,
                customer.CustomerTBCode,
                customer.Email,
                customer.Tel,
                customer.Wechat,
                customer.Travellers,
                IsWechatBind = string.IsNullOrEmpty(customer.OpenID) ? "false" : "true",
                ImageUrl = string.IsNullOrEmpty(customer.OpenID) ? "" : WeiXinHelper.GetWeixinImage(customer.OpenID),
                RQUrl = string.IsNullOrEmpty(customer.OpenID) ? WeiXinHelper.GetImageUrlByID(customer.CustomerID, systemType.costomer) : ""
            };
            return JsonConvert.SerializeObject(new { ErrorCode = 200, ErrorMessage = "OK", UserInfo = UserInfo });
        }
        //修改用户基本信息
        [HttpPost]
        public async Task<string> EditUserInfo(Customer customer)
        {
            string UserName = User.Identity.Name;
            if (customer.CustomerID == 0)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "客户不能为空" });
            }
            Customer oldcustomer = await client.For<Customer>()
                .Filter(u => u.CustomerTBCode == UserName)
                .Filter(u => u.CustomerID == customer.CustomerID).FindEntryAsync();
            if (oldcustomer == null)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "页面已过期，请重新登录！" });
            }
            if (string.IsNullOrEmpty(customer.CustomerName) || string.IsNullOrEmpty(customer.CustomerEnname))
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "姓名不能为空！" });
            }
            if (string.IsNullOrEmpty(customer.Email))
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "邮箱不能为空！" });
            }
            if (string.IsNullOrEmpty(customer.Tel))
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "联系电话不能为空！" });
            }
            if (string.IsNullOrEmpty(customer.Wechat))
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "微信号不能为空！" });
            }
            string Remark = "";
            if (oldcustomer.CustomerName != customer.CustomerName)
            {
                Remark += "<div>姓名：" + oldcustomer.CustomerName + "→" + customer.CustomerName + "</div>";
            }
            if (oldcustomer.CustomerEnname != customer.CustomerEnname)
            {
                Remark += "<div>拼音：" + oldcustomer.CustomerEnname + "→" + customer.CustomerEnname + "</div>";
            }
            if (oldcustomer.Tel != customer.Tel)
            {
                Remark += "<div>联系电话：" + oldcustomer.Tel + "→" + customer.Tel + "</div>";
            }
            if (oldcustomer.BakTel != customer.BakTel)
            {
                Remark += "<div>备用联系电话：" + oldcustomer.BakTel + "→" + customer.BakTel + "</div>";
            }
            if (oldcustomer.Email != customer.Email)
            {
                Remark += "<div>邮箱：" + oldcustomer.Email + "→" + customer.Email + "</div>";
            }
            if (oldcustomer.Wechat != customer.Wechat)
            {
                Remark += "<div>微信：" + oldcustomer.Wechat + "→" + customer.Wechat + "</div>";
            }
            if (Remark != "")
            {
                oldcustomer.CustomerName = customer.CustomerName;
                oldcustomer.CustomerEnname = customer.CustomerEnname;
                oldcustomer.Tel = customer.Tel;
                oldcustomer.BakTel = customer.BakTel;
                oldcustomer.Email = customer.Email;
                oldcustomer.Wechat = customer.Wechat;
                oldcustomer.Password = string.IsNullOrEmpty(oldcustomer.Password) ? Md5Hash(customer.Tel) : oldcustomer.Password;
                await client.For<Customer>().Key(customer.CustomerID).Set(oldcustomer).UpdateEntryAsync();
                await client.For<CustomerLog>().Set(new CustomerLog
                {
                    Operate = "客人修改基本资料",
                    CustomerID = customer.CustomerID,
                    OperID = customer.CustomerID.ToString(),
                    OperName = oldcustomer.CustomerTBCode,
                    OperTime = DateTimeOffset.Now,
                    Remark = Remark
                }).InsertEntryAsync();
            }
            return JsonConvert.SerializeObject(new { ErrorCode = 200, ErrorMessage = "OK" });
        }
        //修改用户密码
        [HttpPost]
        public async Task<string> EditPassword(int? CustomerID, string oldPassword, string newPassword)
        {
            if (CustomerID == null)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "客户不能为空" });
            }
            string UserName = User.Identity.Name;
            Customer customer = await client.For<Customer>()
                .Filter(u => u.CustomerTBCode == UserName)
                .Filter(u => u.CustomerID == CustomerID).FindEntryAsync();
            if (customer == null)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "页面已过期，请重新登录！" });
            }
            if (string.IsNullOrEmpty(oldPassword))
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "旧密码不能为空！" });
            }
            if (string.IsNullOrEmpty(newPassword))
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "新密码不能为空！" });
            }
            if (oldPassword == newPassword)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "新密码不能与旧密码相同！" });
            }
            string Md5oldPassword = Md5Hash(oldPassword);
            if (customer.Password != Md5oldPassword)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "旧密码不正确！" });
            }
            customer.Password = Md5Hash(newPassword);
            await client.For<Customer>().Key(customer.CustomerID).Set(customer).UpdateEntryAsync();
            await client.For<CustomerLog>().Set(new CustomerLog
            {
                Operate = "客人修改密码",
                CustomerID = customer.CustomerID,
                OperID = customer.CustomerID.ToString(),
                OperName = customer.CustomerTBCode,
                OperTime = DateTimeOffset.Now,
                Remark = ""
            }).InsertEntryAsync();
            return JsonConvert.SerializeObject(new { ErrorCode = 200, ErrorMessage = "OK" });
        }
        //单个旅客信息
        [HttpPost]
        public async Task<string> GetTravellerByID(int id)
        {
            try
            {
                Traveller traveller = await client.For<Traveller>().Key(id).FindEntryAsync();
                return JsonConvert.SerializeObject(new { ErrorCode = 200, ErrorMessage = "OK", traveller });
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 400, ErrorMessage = "查询失败！失败原因：" + ex.Message });
            }
        }
        //新增常用旅客
        [HttpPost]
        public async Task<string> AddTraveller(Traveller traveller)
        {
            if (traveller.CustomerID == 0)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "用户ID不能为空" });
            }
            string UserName = User.Identity.Name;
            Customer customer = await client.For<Customer>()
                .Filter(u => u.CustomerTBCode == UserName)
                .Filter(u => u.CustomerID == traveller.CustomerID).FindEntryAsync();
            if (customer == null)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "页面已过期，请重新登录！" });
            }
            if (string.IsNullOrEmpty(traveller.TravellerName))
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "姓名不能为空" });
            }
            if (string.IsNullOrEmpty(traveller.TravellerEnname))
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "拼音不能为空" });
            }
            if (string.IsNullOrEmpty(traveller.PassportNo))
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "护照号不能为空" });
            }
            try
            {
                Traveller travellerOld = await client.For<Traveller>().Filter(t => t.PassportNo == traveller.PassportNo && t.CustomerID == traveller.CustomerID).FindEntryAsync();
                if (travellerOld != null)
                {
                    return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "护照号已重复" });
                }
                traveller.Birthday = traveller.Birthday < Convert.ToDateTime("1900-01-01") ? Convert.ToDateTime("1900-01-01") : traveller.Birthday;
                traveller.CreateTime = DateTime.Now;
                traveller.TravellerDetail = new TravellerDetail();
                traveller = await client.For<Traveller>().Set(traveller).InsertEntryAsync();
                return JsonConvert.SerializeObject(new { ErrorCode = 200, ErrorMessage = "OK", Traveller = traveller });
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 400, ErrorMessage = "新增失败！失败原因：" + ex.Message });
            }
        }
        //删除常用旅客
        [HttpPost]
        public async Task<string> DelTraveller(int id)
        {
            try
            {
                string UserName = User.Identity.Name;
                var one = await client.For<Traveller>().Expand(s => s.CustomerValue).Key(id).FindEntryAsync();
                if (one.CustomerValue.CustomerTBCode.ToLower() != UserName.ToLower())
                {
                    return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "该常用游客不属于登录用户" });
                }
                await client.For<Traveller>().Key(id).DeleteEntryAsync();
                return JsonConvert.SerializeObject(new { ErrorCode = 200, ErrorMessage = "OK" });
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 400, ErrorMessage = "删除失败！失败原因：" + ex.Message });
            }
        }
        [HttpPost]
        //修改常用旅客
        public async Task<string> EditTraveller(Traveller traveller)
        {
            if (traveller.TravellerID == 0)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "新增失败！失败原因：参数不正确" });
            }
            if (string.IsNullOrEmpty(traveller.TravellerName))
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "姓名不能为空" });
            }
            if (string.IsNullOrEmpty(traveller.TravellerEnname))
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "拼音不能为空" });
            }
            if (string.IsNullOrEmpty(traveller.PassportNo))
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "护照号不能为空" });
            }
            try
            {
                string UserName = User.Identity.Name;
                var travellerOld = await client.For<Traveller>().Expand(s => s.CustomerValue).Key(traveller.TravellerID).FindEntryAsync();
                if (travellerOld.CustomerValue.CustomerTBCode.ToLower() != UserName.ToLower())
                {
                    return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "该常用游客不属于登录用户" });
                }
                Traveller travellerS = await client.For<Traveller>().Filter(t => t.PassportNo == traveller.PassportNo && t.CustomerID == travellerOld.CustomerID && t.TravellerID != traveller.TravellerID).FindEntryAsync();
                if (travellerS != null)
                {
                    return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "护照号已重复" });
                }

                traveller.TravellerDetail = new TravellerDetail();
                traveller.TravellerDetail = travellerOld.TravellerDetail;
                traveller.CustomerID = travellerOld.CustomerID;
                traveller.Birthday = traveller.Birthday < Convert.ToDateTime("1900-01-01") ? Convert.ToDateTime("1900-01-01") : traveller.Birthday;
                traveller.CreateTime = travellerOld.CreateTime;
                await client.For<Traveller>().Key(traveller.TravellerID).Set(traveller).UpdateEntryAsync();
                return JsonConvert.SerializeObject(new { ErrorCode = 200, ErrorMessage = "OK", traveller });
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 400, ErrorMessage = "修改失败！失败原因：" + ex.Message });
            }
        }
        //新增修改常用旅客附加资料
        [HttpPost]
        public async Task<string> EditTravellerDetail(Traveller traveller)
        {
            try
            {
                Traveller travellerOld = await client.For<Traveller>().Expand(s => s.CustomerValue).Key(traveller.TravellerID).FindEntryAsync();
                string UserName = User.Identity.Name;
                if (travellerOld.CustomerValue.CustomerTBCode.ToLower() != UserName.ToLower())
                {
                    return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "该常用游客不属于登录用户" });
                }
                travellerOld.TravellerDetail = traveller.TravellerDetail;
                travellerOld = await client.For<Traveller>().Key(traveller.TravellerID).Set(travellerOld).UpdateEntryAsync();
                return JsonConvert.SerializeObject(new { ErrorCode = 200, ErrorMessage = "OK", travellerOld });
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 400, ErrorMessage = "保存失败！失败原因：" + ex.Message });
            }
        }
        //根据加密的客户ID获取客户姓名
        [HttpPost]
        public async Task<string> GetUserName(string EncryptCustomerID)
        {
            try
            {
                string strCustomerID = EncryptHelper.Decrypt(EncryptCustomerID);
                int id = int.Parse(strCustomerID);
                Customer customer = await client.For<Customer>().Key(id).FindEntryAsync();
                return JsonConvert.SerializeObject(new { ErrorCode = 200, ErrorMessage = "OK", UserName = customer.CustomerName });
            }
            catch
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "参数有误" });
            }
        }
        //邀请朋友新增常用旅客
        [HttpPost]
        [AllowAnonymous]
        public async Task<string> ExtandAddTraveller(string TravellerName, string TravellerEnname, string PassportNo, DateTimeOffset Birthday, sex TravellerSex, string EncryptCustomerID)
        {
            int id;
            try
            {
                string strCustomerID = EncryptHelper.Decrypt(EncryptCustomerID);
                id = int.Parse(strCustomerID);
                Customer customer = await client.For<Customer>().Expand(s => s.Travellers).Key(id).FindEntryAsync();
                if (customer.Travellers.Count > 200)
                {
                    return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "游客添加数已达上限！" });
                }
            }
            catch
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "参数有误" });
            }
            Traveller traveller = new Traveller();
            traveller.TravellerName = TravellerName;
            traveller.TravellerEnname = TravellerEnname;
            traveller.PassportNo = PassportNo;
            traveller.Birthday = Birthday;
            traveller.TravellerSex = TravellerSex;
            traveller.CustomerID = id;
            if (string.IsNullOrEmpty(traveller.TravellerName))
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "姓名不能为空" });
            }
            if (string.IsNullOrEmpty(traveller.TravellerEnname))
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "拼音不能为空" });
            }
            if (string.IsNullOrEmpty(traveller.PassportNo))
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "护照号不能为空" });
            }
            try
            {
                Traveller travellerOld = await client.For<Traveller>().Filter(t => t.PassportNo == PassportNo && t.CustomerID == id).FindEntryAsync();
                if (travellerOld != null)
                {
                    return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "护照号已重复" });
                }
                traveller.Birthday = traveller.Birthday < Convert.ToDateTime("1900-01-01") ? Convert.ToDateTime("1900-01-01") : traveller.Birthday;
                traveller.CreateTime = DateTime.Now;
                traveller.TravellerDetail = new TravellerDetail();
                traveller = await client.For<Traveller>().Set(traveller).InsertEntryAsync();
                return JsonConvert.SerializeObject(new { ErrorCode = 200, ErrorMessage = "OK", Traveller = traveller });
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 400, ErrorMessage = "新增失败！失败原因：" + ex.Message });
            }
        }

        //设置登录失败次数
        private int SetDefaultLoginFaildTimes()
        {
            int ctimes = 0;
            if (Session["FaildTimes"] == null)
            {
                ctimes = 1;
                Session["FaildTimes"] = ctimes;
                Session.Timeout = 10;
            }
            else
            {
                ctimes = int.Parse(Session["FaildTimes"].ToString()) + 1;
                Session["FaildTimes"] = ctimes;
            }
            return ctimes;
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