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
using Entity;

namespace LanghuaNew.Controllers
{
    public class CancelRegistersController : Controller
    {
        private ODataClient client = new ODataClient(ConfigurationManager.AppSettings["ServicePath"] + "odata/");
        //取消登记
        // GET: CancelRegisters
        public async Task<ActionResult> Index()
        {
            bool isSave = false;
            string userName = User.Identity.Name;
            User user = await client.For<User>().Expand("UserRole/MenuRights").Filter(u => u.UserName == userName).FindEntryAsync();
            if (user.UserRole != null)
            {
                foreach (var item in user.UserRole.Where(s => s.RoleEnableState == EnableState.Enable))
                {
                    if (item.RoleID == 1)
                    {
                        isSave = true;
                        break;
                    }
                    if (item.MenuRights != null)
                    {
                        foreach (var MenuRight in item.MenuRights)
                        {
                            var MenuResult = await client.For<MenuRight>().Expand(s => s.RoleRights).Key(MenuRight.MenuRightID).FindEntryAsync();
                            foreach (var rights in MenuResult.RoleRights)
                            {
                                if (rights.ControllerName == "CancelRegisters" && rights.ActionName == "SetCancelRegisters")
                                {
                                    isSave = true;
                                    break;
                                }
                            }
                            if (isSave) break;
                        }
                        if (isSave) break;
                    }
                }
            }
            ViewBag.isSave = isSave;
            ViewBag.Supplier = await client.For<Supplier>().Filter(s => s.SupplierEnableState == EnableState.Enable).OrderBy(s => s.SupplierNo).FindEntriesAsync();
            return View();
        }
        //获取取消登记列表
        public async Task<string> GetCancelRegisters(ShareSearchModel share)
        {
            int draw = 1;
            int start = 0;
            int length = 50;
            int SupplierID = 0;
            string BeginDate = string.Empty;
            string EndDate = string.Empty;
            if (share.length > 0)
            {
                draw = share.draw;
                start = share.start;
                length = share.length;
            }
            if (share.CancelRegisterSearch != null)
            {
                SupplierID = share.CancelRegisterSearch.SupplierID;
                BeginDate = share.CancelRegisterSearch.BeginDate;
                EndDate = share.CancelRegisterSearch.EndDate;
            }
            var Result = client.For<CancelRegister>().Expand(s => s.serviceItem).Expand(s => s.supplier);
            var ResultCount = client.For<CancelRegister>();
            if (SupplierID > 0)
            {
                Result = Result.Filter(s => s.SupplierID == SupplierID);
                ResultCount = ResultCount.Filter(s => s.SupplierID == SupplierID);
            }
            if (!string.IsNullOrEmpty(BeginDate) && !string.IsNullOrEmpty(EndDate))
            {
                try
                {
                    DateTimeOffset begin = DateTimeOffset.Parse(BeginDate);
                    DateTimeOffset end = DateTimeOffset.Parse(EndDate);
                    Result = Result.Filter(s =>
                    (begin <= s.StartDate && s.StartDate <= end) ||
                    (begin <= s.EndDate && s.EndDate <= end) ||
                    (s.StartDate <= begin && begin <= s.EndDate) ||
                    (s.StartDate <= end && end <= s.EndDate)
                    );
                    ResultCount = ResultCount.Filter(s =>
                    (begin <= s.StartDate && s.StartDate <= end) ||
                    (begin <= s.EndDate && s.EndDate <= end) ||
                    (s.StartDate <= begin && begin <= s.EndDate) ||
                    (s.StartDate <= end && end <= s.EndDate)
                    );
                }
                catch { }
            }
            int count = await ResultCount.Count().FindScalarAsync<int>();
            var cancelRegisters = await Result.OrderByDescending(s => s.CancelRegisterID).Skip(start).Top(length).FindEntriesAsync();
            var data = cancelRegisters.Select(s => new
            {
                s.CancelRegisterID,
                StartDate = s.StartDate.ToString("yyyy-MM-dd"),
                EndDate = s.EndDate.ToString("yyyy-MM-dd"),
                s.Remark,
                CreateTime = s.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                s.CreateUserNikeName,
                s.serviceItem.cnItemName,
                s.serviceItem.ServiceCode,
                s.supplier.SupplierNo,
                s.supplier.SupplierName
            });
            return JsonConvert.SerializeObject(new { draw = draw, recordsFiltered = count, data = data, SearchModel = share });
        }
        //根据供应商搜索产品
        public async Task<string> GetItems(int? SupplierID, string Str)
        {
            if (SupplierID == null || SupplierID == 0)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "供应商不能为空" });
            }
            var Result = client.For<ServiceItem>()
                .Filter(s => s.ItemSuplier.Any(ss => ss.SupplierID == SupplierID))
                .Filter(s => s.ServiceItemEnableState == EnableState.Enable);
            if (!string.IsNullOrEmpty(Str))
            {
                Result = Result.Filter(s => s.cnItemName.Contains(Str) || s.ServiceCode.Contains(Str));
            }
            var data = (await Result.FindEntriesAsync()).Select(s => new { s.cnItemName, s.ServiceCode, s.ServiceItemID });
            return JsonConvert.SerializeObject(new { ErrorCode = 200, ErrorMessage = "OK", data });
        }
        //新增取消登记
        public async Task<string> Create(int? SupplierID, string ItemID, DateTimeOffset BeginDate, DateTimeOffset EndDate, string Remark)
        {
            bool isSave = false;
            string userName = User.Identity.Name;
            User user = await client.For<User>().Expand("UserRole/MenuRights").Filter(u => u.UserName == userName).FindEntryAsync();
            if (user.UserRole != null)
            {
                foreach (var item in user.UserRole.Where(s => s.RoleEnableState == EnableState.Enable))
                {
                    if (item.RoleID == 1)
                    {
                        isSave = true;
                        break;
                    }
                    if (item.MenuRights != null)
                    {
                        foreach (var MenuRight in item.MenuRights)
                        {
                            var MenuResult = await client.For<MenuRight>().Expand(s => s.RoleRights).Key(MenuRight.MenuRightID).FindEntryAsync();
                            foreach (var rights in MenuResult.RoleRights)
                            {
                                if (rights.ControllerName == "CancelRegisters" && rights.ActionName == "SetCancelRegisters")
                                {
                                    isSave = true;
                                    break;
                                }
                            }
                            if (isSave) break;
                        }
                        if (isSave) break;
                    }
                }
            }
            if (!isSave) return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "对不起，您没有权限操作！" });
            if (SupplierID == null || SupplierID == 0)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "供应商不能为空" });
            }
            if (string.IsNullOrEmpty(ItemID))
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "产品不能为空" });
            }
            if (BeginDate < DateTimeOffset.Parse("1901-01-01"))
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "开始日期不能为空" });
            }
            if (EndDate < DateTimeOffset.Parse("1901-01-01"))
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "结束日期不能为空" });
            }
            if (string.IsNullOrEmpty(Remark))
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "取消原因不能为空" });
            }
            //string userName = User.Identity.Name;
            //User user = await client.For<User>().Filter(u => u.UserName == userName).FindEntryAsync();
            var failed = (new int[] { 1 }).Select(x => new { cnItemName = "", reason = "" }).ToList();
            failed.Clear();
            var str = ItemID.Split(',');
            foreach (var id in str)
            {
                int ServiceItemID = int.Parse(id);
                CancelRegister one = await client.For<CancelRegister>()
                    .Expand(s => s.serviceItem)
                    //.Expand(s => s.supplier)
                    .Filter(s => s.SupplierID == SupplierID)
                    .Filter(s => s.ServiceItemID == ServiceItemID)
                    .Filter(s => (BeginDate <= s.StartDate && s.StartDate <= EndDate) ||
                                 (BeginDate <= s.EndDate && s.EndDate <= EndDate) ||
                                 (s.StartDate <= BeginDate && BeginDate <= s.EndDate) ||
                                 (s.StartDate <= EndDate && EndDate <= s.EndDate))
                    .FindEntryAsync();
                if (one != null)
                {
                    failed.Add(new { one.serviceItem.cnItemName, reason = "该时间内已有记录,不能重复添加" });
                }
                else
                {
                    try
                    {
                        CancelRegister cancelRegister = new CancelRegister();
                        cancelRegister.CreateTime = DateTimeOffset.Now;
                        cancelRegister.CreateUserID = user.UserID;
                        cancelRegister.CreateUserNikeName = user.NickName;
                        cancelRegister.SupplierID = int.Parse(SupplierID.ToString());
                        cancelRegister.StartDate = BeginDate;
                        cancelRegister.EndDate = EndDate;
                        cancelRegister.Remark = Remark;
                        cancelRegister.ServiceItemID = ServiceItemID;
                        cancelRegister = await client.For<CancelRegister>().Set(cancelRegister).InsertEntryAsync();
                        CancelRegisterLog log = new CancelRegisterLog();
                        log.CancelRegisterID = cancelRegister.CancelRegisterID;
                        log.OperUserID = user.UserID;
                        log.OperUserNickName = user.NickName;
                        log.OperTime = DateTimeOffset.Now;
                        log.Operate = "新增取消登记";
                        await client.For<CancelRegisterLog>().Set(log).InsertEntryAsync();
                    }
                    catch
                    {
                        failed.Add(new { one.serviceItem.cnItemName, reason = "添加失败" });
                    }
                }
            }
            return JsonConvert.SerializeObject(new { ErrorCode = 200, ErrorMessage = "OK", failed });
        }
        //修改取消登记
        public async Task<string> Edit(int? CancelRegisterID, DateTimeOffset BeginDate, DateTimeOffset EndDate, string Remark)
        {
            bool isSave = false;
            string userName = User.Identity.Name;
            User user = await client.For<User>().Expand("UserRole/MenuRights").Filter(u => u.UserName == userName).FindEntryAsync();
            if (user.UserRole != null)
            {
                foreach (var item in user.UserRole.Where(s => s.RoleEnableState == EnableState.Enable))
                {
                    if (item.RoleID == 1)
                    {
                        isSave = true;
                        break;
                    }
                    if (item.MenuRights != null)
                    {
                        foreach (var MenuRight in item.MenuRights)
                        {
                            var MenuResult = await client.For<MenuRight>().Expand(s => s.RoleRights).Key(MenuRight.MenuRightID).FindEntryAsync();
                            foreach (var rights in MenuResult.RoleRights)
                            {
                                if (rights.ControllerName == "CancelRegisters" && rights.ActionName == "SetCancelRegisters")
                                {
                                    isSave = true;
                                    break;
                                }
                            }
                            if (isSave) break;
                        }
                        if (isSave) break;
                    }
                }
            }
            if (!isSave) return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "对不起，您没有权限操作！" });
            if (CancelRegisterID == null || CancelRegisterID == 0)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "ID不能为空" });
            }
            if (BeginDate < DateTimeOffset.Parse("1901-01-01"))
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "开始日期不能为空" });
            }
            if (EndDate < DateTimeOffset.Parse("1901-01-01"))
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "结束日期不能为空" });
            }
            if (string.IsNullOrEmpty(Remark))
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "取消原因不能为空" });
            }
            //string userName = User.Identity.Name;
            //User user = await client.For<User>().Filter(u => u.UserName == userName).FindEntryAsync();
            try
            {
                CancelRegister cancelRegister = await client.For<CancelRegister>().Key(CancelRegisterID).FindEntryAsync();
                CancelRegister one = await client.For<CancelRegister>()
                    .Filter(s => s.CancelRegisterID != CancelRegisterID)
                    .Filter(s => s.SupplierID == cancelRegister.SupplierID)
                    .Filter(s => s.ServiceItemID == cancelRegister.ServiceItemID)
                    .Filter(s => (BeginDate <= s.StartDate && s.StartDate <= EndDate) ||
                                 (BeginDate <= s.EndDate && s.EndDate <= EndDate) ||
                                 (s.StartDate <= BeginDate && BeginDate <= s.EndDate) ||
                                 (s.StartDate <= EndDate && EndDate <= s.EndDate))
                    .FindEntryAsync();
                if (one != null)
                {
                    return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "该时间内已有记录,不能重复添加" });
                }
                string logRemark = "";
                if (cancelRegister.StartDate.ToString("yyyy-MM-dd") != BeginDate.ToString("yyyy-MM-dd"))
                {
                    logRemark += "<div>开始时间：" + cancelRegister.StartDate.ToString("yyyy-MM-dd") + "→" + BeginDate.ToString("yyyy-MM-dd") + "</div>";
                }
                if (cancelRegister.EndDate.ToString("yyyy-MM-dd") != EndDate.ToString("yyyy-MM-dd"))
                {
                    logRemark += "<div>结束时间：" + cancelRegister.EndDate.ToString("yyyy-MM-dd") + "→" + EndDate.ToString("yyyy-MM-dd") + "</div>";
                }
                if (cancelRegister.Remark != Remark)
                {
                    logRemark += "<div>备注：" + cancelRegister.Remark + "→" + Remark + "</div>";
                }
                if (logRemark == "")
                {
                    return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "没有修改任何内容" });
                }
                CancelRegisterLog log = new CancelRegisterLog();
                log.CancelRegisterID = cancelRegister.CancelRegisterID;
                log.OperUserID = user.UserID;
                log.OperUserNickName = user.NickName;
                log.OperTime = DateTimeOffset.Now;
                log.Operate = "修改取消登记";
                log.Remark = logRemark;
                await client.For<CancelRegisterLog>().Set(log).InsertEntryAsync();
                cancelRegister.StartDate = BeginDate;
                cancelRegister.EndDate = EndDate;
                cancelRegister.Remark = Remark;
                await client.For<CancelRegister>().Key(CancelRegisterID).Set(cancelRegister).UpdateEntryAsync();
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 400, ErrorMessage = "修改失败" + ex.Message });
            }
            await client.For<SystemLog>().Set(new
            {
                Operate = "修改取消登记",
                OperateTime = DateTime.Now,
                UserID = user.UserID,
                UserName = user.NickName,
                Remark = "CancelRegisterID：" + CancelRegisterID + "BeginDate：" + BeginDate + "EndDate：" + EndDate + "Remark：" + Remark
            }).InsertEntryAsync();
            return JsonConvert.SerializeObject(new { ErrorCode = 200, ErrorMessage = "OK" });
        }
        //删除取消登记
        public async Task<string> Delete(string CancelRegisterID)
        {
            bool isSave = false;
            string userName = User.Identity.Name;
            User user = await client.For<User>().Expand("UserRole/MenuRights").Filter(u => u.UserName == userName).FindEntryAsync();
            if (user.UserRole != null)
            {
                foreach (var item in user.UserRole.Where(s => s.RoleEnableState == EnableState.Enable))
                {
                    if (item.RoleID == 1)
                    {
                        isSave = true;
                        break;
                    }
                    if (item.MenuRights != null)
                    {
                        foreach (var MenuRight in item.MenuRights)
                        {
                            var MenuResult = await client.For<MenuRight>().Expand(s => s.RoleRights).Key(MenuRight.MenuRightID).FindEntryAsync();
                            foreach (var rights in MenuResult.RoleRights)
                            {
                                if (rights.ControllerName == "CancelRegisters" && rights.ActionName == "SetCancelRegisters")
                                {
                                    isSave = true;
                                    break;
                                }
                            }
                            if (isSave) break;
                        }
                        if (isSave) break;
                    }
                }
            }
            if (!isSave) return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "对不起，您没有权限操作！" });
            if (string.IsNullOrEmpty(CancelRegisterID))
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "ID不能为空" });
            }
            try
            {
                foreach (var id in CancelRegisterID.Split(','))
                {
                    CancelRegister cancelRegister = await client.For<CancelRegister>().Key(int.Parse(id)).FindEntryAsync();
                    await client.For<CancelRegister>().Key(int.Parse(id)).DeleteEntryAsync();
                    await client.For<SystemLog>().Set(new
                    {
                        Operate = "删除取消登记",
                        OperateTime = DateTime.Now,
                        UserID = user.UserID,
                        UserName = user.NickName,
                        Remark = JsonConvert.SerializeObject(cancelRegister)
                    }).InsertEntryAsync();
                }
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 400, ErrorMessage = "删除失败" + ex.Message });
            }
            return JsonConvert.SerializeObject(new { ErrorCode = 200, ErrorMessage = "OK" });
        }
        //取消登记操作日志
        public async Task<ActionResult> CancelRegisterOperation(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var CancelRegisters = await client.For<CancelRegisterLog>().Filter(c => c.CancelRegisterID == id).OrderByDescending(c => c.CancelRegisterLogID).FindEntriesAsync();
            return View(CancelRegisters);
        }
    }
}
