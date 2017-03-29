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
using System.Security.Cryptography;
using System.Text;
using Simple.OData.Client;
using System.Configuration;
using WebGrease.Css.Extensions;
using Newtonsoft.Json;
using LanghuaNew.Models;
using System.IO;
using Entity;
using System.Net.Http;
using Commond;

namespace LanghuaNew.Controllers
{
    public class CustomersController : Controller
    {
        private ODataClient client = new ODataClient(ConfigurationManager.AppSettings["ServicePath"] + "odata/");
        //客户列表
        // GET: Customers
        public async Task<ActionResult> Index()
        {
            bool SetReturnList = false;
            string userName = User.Identity.Name;
            User user = await client.For<User>().Expand("UserRole/MenuRights").Filter(u => u.UserName == userName).FindEntryAsync();
            if (user.UserRole != null)
            {
                foreach (var item in user.UserRole.Where(s => s.RoleEnableState == EnableState.Enable))
                {
                    if (item.RoleID == 1)
                    {
                        SetReturnList = true;
                        break;
                    }
                    if (item.MenuRights != null)
                    {
                        foreach (var MenuRight in item.MenuRights)
                        {
                            var MenuResult = await client.For<MenuRight>().Expand(s => s.RoleRights).Key(MenuRight.MenuRightID).FindEntryAsync();
                            foreach (var rights in MenuResult.RoleRights)
                            {
                                if (rights.ControllerName == "Customers" && rights.ActionName == "SetReturnList")
                                {
                                    SetReturnList = true;
                                }
                            }
                        }
                    }
                }
            }
            ViewBag.SetReturnList = SetReturnList;

            var list = await client.For<CustomerReturnList>().Expand(c => c.ReturnServiceItem).Expand(c => c.ReturnSupplier).FindEntriesAsync();
            return View(list);
        }
        //客户详情
        // GET: Customers/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = await client.For<Customer>().Expand(c => c.CustomerBacks).Key(id).FindEntryAsync();
            if (customer == null)
            {
                return HttpNotFound();
            }
            if (!string.IsNullOrEmpty(customer.OpenID))
            {
                ViewBag.NickName = WeiXinHelper.GetWeixinNickName(customer.OpenID);
            }
            ViewBag.ImageUrl = WeiXinHelper.GetImageUrlByID(customer.CustomerID, systemType.costomer);
            return View(customer);
        }
        //客户操作日志
        // GET: Customers/CustomerOperation/5
        public async Task<ActionResult> CustomerOperation(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var CustomerLogs = await client.For<CustomerLog>().Filter(c => c.CustomerID == id).OrderByDescending(c => c.CustomerLogID).FindEntriesAsync();
            return View(CustomerLogs);
        }
        //配置回访名单
        [HttpPost]
        public async Task<string> SaveReturnList(IEnumerable<CustomerReturnList> list)
        {
            bool SetReturnList = false;
            string userName = User.Identity.Name;
            User user = await client.For<User>().Expand("UserRole/MenuRights").Filter(u => u.UserName == userName).FindEntryAsync();
            if (user.UserRole != null)
            {
                foreach (var item in user.UserRole.Where(s => s.RoleEnableState == EnableState.Enable))
                {
                    if (item.RoleID == 1)
                    {
                        SetReturnList = true;
                        break;
                    }
                    if (item.MenuRights != null)
                    {
                        foreach (var MenuRight in item.MenuRights)
                        {
                            var MenuResult = await client.For<MenuRight>().Expand(s => s.RoleRights).Key(MenuRight.MenuRightID).FindEntryAsync();
                            foreach (var rights in MenuResult.RoleRights)
                            {
                                if (rights.ControllerName == "Customers" && rights.ActionName == "SetReturnList")
                                {
                                    SetReturnList = true;
                                }
                            }
                        }
                    }
                }
            }
            if (!SetReturnList)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "对不起，您没有权限配置回访名单" });
            }
            if (list.Count() > 15)
            {
                //return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "回访产品不能超过15个" });
            }
            var oldlist = await client.For<CustomerReturnList>().FindEntriesAsync();
            var AddList = list.ExceptItem(oldlist, p => new { p.ServiceItemID, p.SupplierID }).ToList();
            var ModifList = list.ExceptItem(AddList, p => new { p.ServiceItemID, p.SupplierID }).ToList();
            var DeleteList = oldlist.ExceptItem(ModifList, p => new { p.ServiceItemID, p.SupplierID }).ToList();

            foreach (var item in AddList)
            {
                await client.For<CustomerReturnList>().Set(item).InsertEntryAsync();
            }
            foreach (var item in DeleteList)
            {
                await client.For<CustomerReturnList>().Key(item.CustomerReturnListID).DeleteEntriesAsync();
            }

            return JsonConvert.SerializeObject(new { ErrorCode = 200, ErrorMessage = "OK" });
        }
        //获取客户列表
        public async Task<string> GetCustomers(ShareSearchModel search)
        {
            HttpResponseMessage Message = await HttpHelper.PostAction("CustomersExtend", JsonConvert.SerializeObject(search));
            string exMessage = Message.Content.ReadAsStringAsync().Result;
            return exMessage;
        }

        //列表导出
        public async Task<FileResult> ExportExcel(ShareCustomerSearchModel search)
        {
            // 获取列表
            HttpResponseMessage Message = await HttpHelper.PutAction("CustomersExtend", JsonConvert.SerializeObject(search));
            string exMessage = Message.Content.ReadAsStringAsync().Result;
            IEnumerable<CustomerModel> customers = JsonConvert.DeserializeObject<IEnumerable<CustomerModel>>(exMessage);

            //创建Excel文件的对象
            NPOI.HSSF.UserModel.HSSFWorkbook book = new NPOI.HSSF.UserModel.HSSFWorkbook();
            //添加一个sheet
            NPOI.SS.UserModel.ISheet sheet1 = book.CreateSheet("Sheet1");

            int i = 0;
            //给sheet1添加第一行的头部标题
            NPOI.SS.UserModel.IRow row1 = sheet1.CreateRow(i);

            row1.CreateCell(0).SetCellValue("淘宝ID");
            row1.CreateCell(1).SetCellValue("姓名(中文)");
            row1.CreateCell(2).SetCellValue("姓名(拼音)");
            row1.CreateCell(3).SetCellValue("联系电话");
            row1.CreateCell(4).SetCellValue("备用联系电话");
            row1.CreateCell(5).SetCellValue("订单数");
            row1.CreateCell(6).SetCellValue("要售后");
            row1.CreateCell(7).SetCellValue("回访");
            row1.CreateCell(8).SetCellValue("登记微信号");
            row1.CreateCell(9).SetCellValue("微信绑定");
            row1.CreateCell(10).SetCellValue("最近备注");

            row1.Height = 450;

            //将数据逐步写入sheet1各个行
            foreach (var item in customers)
            {
                i++;
                NPOI.SS.UserModel.IRow rowtemp = sheet1.CreateRow(i);

                rowtemp.CreateCell(0).SetCellValue(item.CustomerTBCode == null ? "" : item.CustomerTBCode);
                rowtemp.CreateCell(1).SetCellValue(item.CustomerName == null ? "" : item.CustomerName);
                rowtemp.CreateCell(2).SetCellValue(item.CustomerEnname == null ? "" : item.CustomerEnname);
                rowtemp.CreateCell(3).SetCellValue(item.Tel == null ? "" : item.Tel);
                rowtemp.CreateCell(4).SetCellValue(item.BakTel == null ? "" : item.BakTel);
                rowtemp.CreateCell(5).SetCellValue(item.OrderNum);
                rowtemp.CreateCell(6).SetCellValue(item.IsNeedCustomerService ? "要售后" : "");
                rowtemp.CreateCell(7).SetCellValue(item.IsBack ? "已回访" : "");
                rowtemp.CreateCell(8).SetCellValue(item.WeixinNo);
                rowtemp.CreateCell(9).SetCellValue(item.BindWeixin ? "已绑定" : "");
                rowtemp.CreateCell(10).SetCellValue(item.Remark);
            }

            // 写入到客户端 
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            book.Write(ms);
            ms.Seek(0, SeekOrigin.Begin);
            return File(ms, "application/vnd.ms-excel", "customerlist.xls");
        }

        //解绑微信
        [HttpPost]
        public async Task<string> UnbindWeixin(int id)
        {
            var customer = await client.For<Customer>().Key(id).FindEntryAsync();
            customer.OpenID = null;
            await client.For<Customer>().Key(id).Set(customer).UpdateEntryAsync();
            return JsonConvert.SerializeObject(new { ErrorCode = 200, ErrorMessage = "OK" });
        }
        [HttpPost]
        public async Task<string> ResetPassWord(int id)
        {
            var customer = await client.For<Customer>().Key(id).FindEntryAsync();
            string strNewPW = System.Guid.NewGuid().ToString().Replace("-", "").Substring(0, 6);
            string ResetPassWord = Md5Hash(strNewPW);
            customer.Password = ResetPassWord;
            await client.For<Customer>().Key(id).Set(customer).UpdateEntryAsync();

            string userName = User.Identity.Name;
            User OperUser = await client.For<User>().Filter(u => u.UserName == userName).FindEntryAsync();
            //操作记录
            await client.For<SystemLog>().Set(new { Operate = "重置客户密码", OperateTime = DateTime.Now, UserID = OperUser.UserID, UserName = OperUser.NickName, Remark = customer.CustomerName }).InsertEntryAsync();
            await client.For<CustomerLog>().Set(new CustomerLog
            {
                Operate = "重置客户密码",
                CustomerID = customer.CustomerID,
                OperID = OperUser.UserID.ToString(),
                OperName = OperUser.NickName,
                OperTime = DateTimeOffset.Now,
                Remark = ""
            }).InsertEntryAsync();
            return JsonConvert.SerializeObject("重置成功,新密码为：" + strNewPW);
        }
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
        //保存客户跟进记录
        [HttpPost]
        public async Task<string> SaveCustomerBack(CustomerBack back, bool isBack)
        {
            if (back == null)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "跟进记录不能为空" });
            }
            if (back.CustomerID == 0)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "客户不能为空" });
            }
            if (string.IsNullOrEmpty(back.Remark))
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "情况记录不能为空" });
            }
            string userName = User.Identity.Name;
            User user = await client.For<User>().Filter(u => u.UserName == userName).FindEntryAsync();

            back.CreateData = DateTime.Now;
            back.OperateUserID = user.UserID;
            back.OperateUserNickName = user.NickName;
            await client.For<CustomerBack>().Set(back).InsertEntryAsync();

            var customer = await client.For<Customer>().Key(back.CustomerID).FindEntryAsync();
            customer.IsBack = isBack;
            await client.For<Customer>().Key(back.CustomerID).Set(customer).UpdateEntryAsync();
            return JsonConvert.SerializeObject(new { ErrorCode = 200, ErrorMessage = "OK" });
        }
        //修改客户基本资料
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

            string userName = User.Identity.Name;
            User user = await client.For<User>().Filter(u => u.UserName == userName).FindEntryAsync();

            Customer oldcustomer = await client.For<Customer>().Key(customer.CustomerID).FindEntryAsync();
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
                    Operate = "修改基本资料",
                    CustomerID = customer.CustomerID,
                    OperID = user.UserID.ToString(),
                    OperName = user.NickName,
                    OperTime = DateTimeOffset.Now,
                    Remark = Remark
                }).InsertEntryAsync();
            }
            return JsonConvert.SerializeObject(new { ErrorCode = 200, ErrorMessage = "OK" });
        }


    }
}
