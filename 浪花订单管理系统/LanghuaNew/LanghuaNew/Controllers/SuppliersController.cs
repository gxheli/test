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
using LanghuaNew.Models;
using Newtonsoft.Json;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Commond;

namespace LanghuaNew.Controllers
{
    public class SuppliersController : Controller
    {
        private ODataClient client = new ODataClient(ConfigurationManager.AppSettings["ServicePath"] + "odata/");
        public async Task<string> GetSuppliers(SearchModel search)
        {
            int draw = 1;
            int start = 0;
            int length = 50;
            int status = 0;
            string FuzzySearch = string.Empty;
            if (search.length > 0)
            {
                draw = search.draw;
                start = search.start;
                length = search.length;
            }
            if (search.RuleSearch != null)
            {
                status = search.RuleSearch.status;
                FuzzySearch = search.RuleSearch.FuzzySearch;
            }
            var Result = client.For<Supplier>().Expand(s => s.SupplierCountry).Expand(s => s.SupplierUsers);
            var ResultCount = client.For<Supplier>();
            if (status == 1)
            {
                Result = Result.Filter(t => t.SupplierEnableState == EnableState.Enable);
                ResultCount = ResultCount.Filter(t => t.SupplierEnableState == EnableState.Enable);
            }
            else if (status == 2)
            {
                Result = Result.Filter(t => t.SupplierEnableState == EnableState.Disable);
                ResultCount = ResultCount.Filter(t => t.SupplierEnableState == EnableState.Disable);
            }
            if (!string.IsNullOrEmpty(FuzzySearch))
            {
                Result = Result.Filter(t => t.SupplierNo == FuzzySearch || t.EMail.Contains(FuzzySearch) || t.ContactWay.Contains(FuzzySearch));
                ResultCount = ResultCount.Filter(t => t.SupplierNo == FuzzySearch || t.EMail.Contains(FuzzySearch) || t.ContactWay.Contains(FuzzySearch));
            }
            int count = await ResultCount.Count().FindScalarAsync<int>();
            var suppliers = await Result.OrderBy(t => t.SupplierNo).Skip(start).Top(length).FindEntriesAsync();
            var data = suppliers.Select(s => new
            {
                s.SupplierCountry,
                s.SupplierNo,
                s.SupplierName,
                s.SupplierEnName,
                s.SupplierID,
                s.EMail,
                s.ContactWay,
                s.EnableOnline,
                s.SupplierEnableState,
                SupplierSysName = s.SupplierUsers != null ? s.SupplierUsers.Where(u => u.IsMaster).ToList()[0].SupplierUserName : ""
            });
            return JsonConvert.SerializeObject(new { draw = draw, recordsFiltered = count, data = data, SearchModel = search });
        }
        // GET: Suppliers
        public ActionResult Index()
        {
            return View();
        }
        // GET: Suppliers/Create
        public async Task<ActionResult> Create()
        {
            ViewBag.CountryID = new SelectList(await client.For<Country>().FindEntriesAsync(), "CountryID", "CountryName");
            return View();
        }

        // POST: Suppliers/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        public async Task<ActionResult> Create([Bind(Include = "SupplierID,SupplierName,SupplierEnName,SupplierNo,CountryID,EMail,ContactWay,SupplierEnableState,EnableOnline")] Supplier supplier, string SupplierSysName, string SupplierPWD)
        {
            if (ModelState.IsValid)
            {
                if (supplier.EnableOnline && (string.IsNullOrEmpty(SupplierSysName) || string.IsNullOrEmpty(SupplierPWD)))
                {
                    ModelState.AddModelError("", "供应商使用本系统必须填写用户名和密码");
                }
                else
                {
                    var oneuser = await client.For<SupplierUser>().Filter(s => s.SupplierUserName == SupplierSysName).FindEntryAsync();
                    if (oneuser != null && supplier.EnableOnline)
                    {
                        ModelState.AddModelError("", "供应商账号不能重复");
                    }
                    else
                    {
                        var result = await client.For<Supplier>().Filter(s => s.SupplierNo == supplier.SupplierNo).FindEntryAsync();
                        if (result != null)
                        {
                            ModelState.AddModelError("SupplierNo", "供应商代码已重复");
                        }
                        else
                        {
                            supplier = await client.For<Supplier>().Set(supplier).InsertEntryAsync();
                            if (supplier.EnableOnline)
                            {
                                //增加主账号
                                SupplierUser user = new SupplierUser();
                                user.SupplierUserName = SupplierSysName;
                                user.SupplierNickName = supplier.SupplierNo;
                                user.PassWord = Md5Hash(SupplierPWD);
                                user.SupplierID = supplier.SupplierID;
                                user.CreateTime = DateTimeOffset.Now;
                                user.IsMaster = true;
                                user.RealTimeMessage = true;
                                user.Disturb = true;
                                user.BeginTime = "00:00";
                                user.EndTime = "08:00";
                                user.SupplierRoles = new List<SupplierRole>();
                                user.SupplierRoles.Add(new SupplierRole { SupplierRoleID = 1 });
                                await HttpHelper.PostAction("SupplierUsersExtend", JsonConvert.SerializeObject(user));
                                ////增加角色
                                //SupplierRole role = new SupplierRole();
                                //role.SupplierRoleName = "管理员";
                                //role.SupplierRoleEnName = "Admin";
                                //role.SupplierID = supplier.SupplierID;
                                //role.CreateTime = DateTimeOffset.Now;
                                //role.LastEditTime = DateTimeOffset.Now;
                                //role.Remark = "可以对子帐号进行管理";
                                //await client.For<SupplierRole>().Set(role).InsertEntryAsync();
                                //role.SupplierRoleName = "操作员";
                                //role.SupplierRoleEnName = "op";
                                //role.SupplierID = supplier.SupplierID;
                                //role.CreateTime = DateTimeOffset.Now;
                                //role.LastEditTime = DateTimeOffset.Now;
                                //role.Remark = "可以对订单进行处理";
                                //await client.For<SupplierRole>().Set(role).InsertEntryAsync();
                                //role.SupplierRoleName = "财务";
                                //role.SupplierRoleEnName = "Financial";
                                //role.SupplierID = supplier.SupplierID;
                                //role.CreateTime = DateTimeOffset.Now;
                                //role.LastEditTime = DateTimeOffset.Now;
                                //role.Remark = "可以查看对账单";
                                //await client.For<SupplierRole>().Set(role).InsertEntryAsync();
                            }
                            return RedirectToAction("Index");
                        }
                    }
                }
            }
            ViewBag.CountryID = new SelectList(await client.For<Country>().FindEntriesAsync(), "CountryID", "CountryName", supplier.CountryID);
            return View(supplier);
        }

        // GET: Suppliers/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Supplier supplier = await client.For<Supplier>().Key(id).FindEntryAsync();
            if (supplier == null)
            {
                return HttpNotFound();
            }
            SupplierUser supplieruser = await client.For<SupplierUser>().Filter(s => s.SupplierID == id && s.IsMaster).FindEntryAsync();
            if (supplieruser != null)
            {
                ViewBag.SupplierSysName = supplieruser.SupplierUserName;
            }
            else
            {
                ViewBag.Password = "true";
            }
            bool isSet = false;
            string userName = User.Identity.Name;
            User user = await client.For<User>().Expand("UserRole/MenuRights").Filter(u => u.UserName == userName).FindEntryAsync();
            if (user.UserRole != null)
            {
                foreach (var item in user.UserRole.Where(s => s.RoleEnableState == EnableState.Enable))
                {
                    if (item.RoleID == 1)
                    {
                        isSet = true;
                        break;
                    }
                    if (item.MenuRights != null)
                    {
                        foreach (var MenuRight in item.MenuRights)
                        {
                            var MenuResult = await client.For<MenuRight>().Expand(s => s.RoleRights).Key(MenuRight.MenuRightID).FindEntryAsync();
                            foreach (var rights in MenuResult.RoleRights)
                            {
                                if (rights.ControllerName == "Suppliers" && rights.ActionName == "SetSupplierUser")
                                {
                                    isSet = true;
                                }
                            }
                        }
                    }
                }
            }
            ViewBag.isSet = isSet;
            ViewBag.CountryID = new SelectList(await client.For<Country>().FindEntriesAsync(), "CountryID", "CountryName", supplier.CountryID);
            return View(supplier);
        }

        // POST: Suppliers/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        public async Task<ActionResult> Edit([Bind(Include = "SupplierID,SupplierName,SupplierEnName,SupplierNo,CountryID,EMail,ContactWay,SupplierEnableState,EnableOnline")] Supplier supplier, string SupplierSysName)
        {
            SupplierUser user = await client.For<SupplierUser>().Filter(s => s.SupplierID == supplier.SupplierID && s.IsMaster).FindEntryAsync();
            if (ModelState.IsValid)
            {               
                if (supplier.EnableOnline && user != null && string.IsNullOrEmpty(SupplierSysName))
                {
                    ModelState.AddModelError("Email", "供应商使用本系统必须填写用户名");
                }
                else if (supplier.EnableOnline && user == null && (string.IsNullOrEmpty(SupplierSysName) || string.IsNullOrEmpty(Request["SupplierPWD"])))
                {
                    ModelState.AddModelError("Email", "供应商使用本系统必须填写用户名和密码");
                }
                else
                {
                    var oneuser = await client.For<SupplierUser>()
                        .Filter(s => s.SupplierUserName == SupplierSysName)
                        .Filter(s => !s.IsMaster || s.SupplierID != supplier.SupplierID)
                        .FindEntryAsync();
                    if (oneuser != null && supplier.EnableOnline)
                    {
                        ModelState.AddModelError("Email", "供应商账号不能重复");
                    }
                    else
                    {
                        var result = await client.For<Supplier>()
                            .Filter(s => s.SupplierNo == supplier.SupplierNo)
                            .Filter(s => s.SupplierID != supplier.SupplierID)
                            .FindEntryAsync();
                        if (result != null)
                        {
                            ModelState.AddModelError("SupplierNo", "供应商代码已重复");
                        }
                        else
                        {
                            await client.For<Supplier>().Key(supplier.SupplierID).Set(supplier).UpdateEntryAsync();

                            if (supplier.EnableOnline)
                            {
                                if (user == null)
                                {
                                    //增加主账号
                                    user = new SupplierUser();
                                    user.SupplierUserName = SupplierSysName;
                                    user.SupplierNickName = supplier.SupplierNo;
                                    user.PassWord = Md5Hash(Request["SupplierPWD"]);
                                    user.SupplierID = supplier.SupplierID;
                                    user.CreateTime = DateTimeOffset.Now;
                                    user.IsMaster = true;
                                    user.RealTimeMessage = true;
                                    user.Disturb = true;
                                    user.BeginTime = "00:00";
                                    user.EndTime = "08:00";
                                    user.SupplierRoles = new List<SupplierRole>();
                                    user.SupplierRoles.Add(new SupplierRole { SupplierRoleID = 1 });
                                    await HttpHelper.PostAction("SupplierUsersExtend", JsonConvert.SerializeObject(user));
                                }
                                else
                                {
                                    user.SupplierUserName = SupplierSysName;
                                    await client.For<SupplierUser>().Key(user.SupplierUserID).Set(user).UpdateEntryAsync();
                                }
                                //SupplierRole SupplierRoles = await client.For<SupplierRole>().Filter(u => u.SupplierID == supplier.SupplierID).FindEntryAsync();
                                //if (SupplierRoles == null)
                                //{
                                //    //增加角色
                                //    SupplierRole role = new SupplierRole();
                                //    role.SupplierRoleName = "管理员";
                                //    role.SupplierRoleEnName = "Admin";
                                //    role.SupplierID = supplier.SupplierID;
                                //    role.CreateTime = DateTimeOffset.Now;
                                //    role.LastEditTime = DateTimeOffset.Now;
                                //    role.Remark = "可以对子帐号进行管理";
                                //    role.Rights = new List<SupplierRoleRight>();
                                //    await client.For<SupplierRole>().Set(role).InsertEntryAsync();
                                //    role.SupplierRoleName = "操作员";
                                //    role.SupplierRoleEnName = "op";
                                //    role.SupplierID = supplier.SupplierID;
                                //    role.CreateTime = DateTimeOffset.Now;
                                //    role.LastEditTime = DateTimeOffset.Now;
                                //    role.Remark = "可以对订单进行处理";
                                //    await client.For<SupplierRole>().Set(role).InsertEntryAsync();
                                //    role.SupplierRoleName = "财务";
                                //    role.SupplierRoleEnName = "Financial";
                                //    role.SupplierID = supplier.SupplierID;
                                //    role.CreateTime = DateTimeOffset.Now;
                                //    role.LastEditTime = DateTimeOffset.Now;
                                //    role.Remark = "可以查看对账单";
                                //    await client.For<SupplierRole>().Set(role).InsertEntryAsync();
                                //}
                            }
                            return RedirectToAction("Index");
                        }
                    }
                }
            }
            ViewBag.SupplierSysName = SupplierSysName;            
            if (user == null)
            {
                ViewBag.SupplierPWD = Request["SupplierPWD"];
                ViewBag.Password = "true";
            }
            bool isSet = false;
            string userName = User.Identity.Name;
            User user2 = await client.For<User>().Expand("UserRole/MenuRights").Filter(u => u.UserName == userName).FindEntryAsync();
            if (user2.UserRole != null)
            {
                foreach (var item in user2.UserRole.Where(s => s.RoleEnableState == EnableState.Enable))
                {
                    if (item.RoleID == 1)
                    {
                        isSet = true;
                        break;
                    }
                    if (item.MenuRights != null)
                    {
                        foreach (var MenuRight in item.MenuRights)
                        {
                            var MenuResult = await client.For<MenuRight>().Expand(s => s.RoleRights).Key(MenuRight.MenuRightID).FindEntryAsync();
                            foreach (var rights in MenuResult.RoleRights)
                            {
                                if (rights.ControllerName == "Suppliers" && rights.ActionName == "SetSupplierUser")
                                {
                                    isSet = true;
                                }
                            }
                        }
                    }
                }
            }
            ViewBag.isSet = isSet;
            ViewBag.CountryID = new SelectList(await client.For<Country>().FindEntriesAsync(), "CountryID", "CountryName", supplier.CountryID);
            return View(supplier);
        }
        [HttpPost]
        public async Task<string> ResetPassWord(int id)
        {
            var supplier = await client.For<SupplierUser>().Filter(s => s.SupplierID == id && s.IsMaster).FindEntryAsync();
            string strNewPW = System.Guid.NewGuid().ToString().Replace("-", "").Substring(0, 6);
            string ResetPassWord = Md5Hash(strNewPW);
            supplier.PassWord = ResetPassWord;
            await client.For<SupplierUser>().Key(supplier.SupplierUserID).Set(supplier).UpdateEntryAsync();
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
        //启用禁用
        [HttpPost]
        public async Task<string> UpdateDisable(string SupplierID, string Operation)
        {
            var failed = (new int[] { 1 }).Select(x => new { name = "", reason = "" }).ToList();
            failed.Clear();
            if (string.IsNullOrEmpty(SupplierID))
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "供应商不能为空！" });
            }
            if (string.IsNullOrEmpty(Operation))
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "操作异常！" });
            }
            try
            {
                var id = SupplierID.Split(',');
                if (Operation.Trim() == "0")
                {
                    foreach (var i in id)
                    {
                        var oldSupplier = await client.For<Supplier>().Key(int.Parse(i)).FindEntryAsync();

                        var result = await client.For<Supplier>()
                            .Filter(s => s.SupplierNo == oldSupplier.SupplierNo)
                            .Filter(s => s.SupplierID != oldSupplier.SupplierID)
                            .FindEntryAsync();
                        if (result == null)
                        {
                            if (oldSupplier.SupplierEnableState != EnableState.Enable)
                            {
                                oldSupplier.SupplierEnableState = EnableState.Enable;
                                await client.For<Supplier>().Key(int.Parse(i)).Set(oldSupplier).UpdateEntryAsync();
                            }
                        }
                        else
                        {
                            failed.Add(new { name = oldSupplier.SupplierNo + "-" + oldSupplier.SupplierName, reason = "启用失败！供应商代码已重复" });
                        }
                    }
                }
                else if (Operation.Trim() == "1")
                {
                    foreach (var i in id)
                    {
                        int sid = int.Parse(i);
                        var serviceitem = await client.For<ServiceItem>().Filter(s => s.DefaultSupplierID == sid && s.ServiceItemEnableState == EnableState.Enable).FindEntryAsync();
                        var oldSupplier = await client.For<Supplier>().Key(sid).FindEntryAsync();
                        if (serviceitem != null)
                        {
                            failed.Add(new { name = oldSupplier.SupplierNo + "-" + oldSupplier.SupplierName, reason = "该供应商为默认供应商，请移除后再禁用！" });
                        }
                        else
                        {
                            if (oldSupplier.SupplierEnableState != EnableState.Disable)
                            {
                                oldSupplier.SupplierEnableState = EnableState.Disable;
                                await client.For<Supplier>().Key(int.Parse(i)).Set(oldSupplier).UpdateEntryAsync();
                            }
                        }
                    }
                }
                //else if (Operation.Trim().ToLower() == "delete")
                //{
                //    foreach (var i in id)
                //    {
                //        await client.For<Supplier>().Key(int.Parse(i)).DeleteEntryAsync();
                //    }
                //}
                else
                {
                    return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "不允许进行当前操作！" });
                }
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 400, ErrorMessage = "出错啦！出错原因：" + ex.Message });
            }
            return JsonConvert.SerializeObject(new { ErrorCode = 200, ErrorMessage = "OK", failed });
        }
        //列表导出
        public async Task<FileResult> ExportExcel(RuleSearchModel search)
        {
            // 获取列表
            int status = search.status;
            string FuzzySearch = search.FuzzySearch;

            var Result = client.For<Supplier>().Expand(s => s.SupplierCountry);
            if (status == 1)
            {
                Result = Result.Filter(t => t.SupplierEnableState == EnableState.Enable);
            }
            else if (status == 2)
            {
                Result = Result.Filter(t => t.SupplierEnableState == EnableState.Disable);
            }
            if (!string.IsNullOrEmpty(FuzzySearch))
            {
                Result = Result.Filter(t => t.SupplierNo == FuzzySearch || t.EMail.Contains(FuzzySearch) || t.ContactWay.Contains(FuzzySearch));
            }
            var suppliers = await Result.OrderBy(t => t.SupplierNo).FindEntriesAsync();

            //创建Excel文件的对象
            NPOI.HSSF.UserModel.HSSFWorkbook book = new NPOI.HSSF.UserModel.HSSFWorkbook();
            //添加一个sheet
            NPOI.SS.UserModel.ISheet sheet1 = book.CreateSheet("Sheet1");

            int i = 0;
            //给sheet1添加第一行的头部标题
            NPOI.SS.UserModel.IRow row1 = sheet1.CreateRow(i);

            row1.CreateCell(0).SetCellValue("国家");
            row1.CreateCell(1).SetCellValue("供应商代码");
            row1.CreateCell(2).SetCellValue("供应商简称");
            row1.CreateCell(3).SetCellValue("邮箱");
            row1.CreateCell(4).SetCellValue("联系方式");
            row1.CreateCell(5).SetCellValue("使用本系统");
            row1.CreateCell(6).SetCellValue("供应商用户名");
            row1.CreateCell(7).SetCellValue("状态");

            row1.Height = 450;

            //将数据逐步写入sheet1各个行
            foreach (var item in suppliers)
            {
                i++;
                NPOI.SS.UserModel.IRow rowtemp = sheet1.CreateRow(i);

                rowtemp.CreateCell(0).SetCellValue(item.SupplierCountry.CountryName);
                rowtemp.CreateCell(1).SetCellValue(item.SupplierNo);
                rowtemp.CreateCell(2).SetCellValue(item.SupplierName);
                rowtemp.CreateCell(3).SetCellValue(item.EMail);
                rowtemp.CreateCell(4).SetCellValue(item.ContactWay);
                rowtemp.CreateCell(5).SetCellValue(item.EnableOnline ? "是" : "否");
                //rowtemp.CreateCell(6).SetCellValue(item.SupplierSysName);
                rowtemp.CreateCell(7).SetCellValue(item.SupplierEnableState == EnableState.Enable ? "启用" : "禁用");
            }

            // 写入到客户端 
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            book.Write(ms);
            ms.Seek(0, SeekOrigin.Begin);
            return File(ms, "application/vnd.ms-excel", "supplierlist.xls");
        }
    }
}
