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
using Simple.OData.Client;
using System.Configuration;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;

namespace LanghuaForCus.Controllers
{
    public class CustomersController : BaseController
    {
        private ODataClient client = new ODataClient(ConfigurationManager.AppSettings["ServicePath"] + "odata/");
        //客人修改基本资料
        [HttpPost]
        public async Task<string> Edit(Customer customer)
        {
            if (customer.CustomerID == 0)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "客户不能为空" });
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
            Customer oldcustomer = await client.For<Customer>().Key(customer.CustomerID).FindEntryAsync();
            //记录客户修改记录
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
        //客人修改密码
        [HttpPost]
        public async Task<string> EditPassword(int? CustomerID, string oldPassword, string newPassword)
        {
            if (CustomerID == null)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "客户不能为空" });
            }
            if (string.IsNullOrEmpty(oldPassword))
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "旧密码不能为空！" });
            }
            if (string.IsNullOrEmpty(newPassword))
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "新密码不能为空！" });
            }
            string Md5oldPassword = Md5Hash(oldPassword);

            Customer customer = await client.For<Customer>()
                .Filter(c => c.CustomerID == CustomerID && c.Password == Md5oldPassword)
                .FindEntryAsync();

            if (customer == null)
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
