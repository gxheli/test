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
using LanghuaNew.Models;
using WebGrease.Css.Extensions;

namespace LanghuaNew.Controllers
{
    public class AlipayTransfersController : Controller
    {
        private ODataClient client = new ODataClient(ConfigurationManager.AppSettings["ServicePath"] + "odata/");

        // GET: AlipayTransfers
        public async Task<ActionResult> Index()
        {
            bool isCheck = false;//核实按钮权限
            bool isTransfer = false;//转账按钮权限
            bool isDelete = false;//删除按钮权限
            string userName = User.Identity.Name;
            User user = await client.For<User>().Expand("UserRole/MenuRights").Filter(u => u.UserName == userName).FindEntryAsync();
            if (user.UserRole != null)
            {
                foreach (var item in user.UserRole.Where(s => s.RoleEnableState == EnableState.Enable))
                {
                    //超级管理员
                    if (item.RoleID == 1)
                    {
                        isCheck = true;
                        isTransfer = true;
                        isDelete = true;
                        break;
                    }
                    if (item.MenuRights != null)
                    {
                        foreach (var MenuRight in item.MenuRights)
                        {
                            var MenuResult = await client.For<MenuRight>().Expand(s => s.RoleRights).Key(MenuRight.MenuRightID).FindEntryAsync();
                            foreach (var rights in MenuResult.RoleRights)
                            {
                                if (rights.ControllerName == "AlipayTransfers" && rights.ActionName == "CheckAlipayTransfer")
                                {
                                    isCheck = true;
                                }
                                if (rights.ControllerName == "AlipayTransfers" && rights.ActionName == "TransferAlipayTransfer")
                                {
                                    isTransfer = true;
                                }
                                if (rights.ControllerName == "AlipayTransfers" && rights.ActionName == "DeleteAlipayTransfer")
                                {
                                    isDelete = true;
                                }
                            }
                        }
                    }
                }
            }
            ViewBag.isCheck = isCheck;
            ViewBag.isTransfer = isTransfer;
            ViewBag.isDelete = isDelete;
            return View();
        }
        // GET: AlipayTransfers/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AlipayTransfer alipayTransfer = null;
            try
            {
                alipayTransfer = await client.For<AlipayTransfer>().Expand(t => t.Logs).Key(id).FindEntryAsync();
            }
            catch { }
            if (alipayTransfer == null)
            {
                return HttpNotFound();
            }
            ViewBag.OrderSourse = await client.For<OrderSourse>()
                .Filter(t => t.OrderSourseEnableState == EnableState.Enable)
                .OrderBy(t => t.ShowNo)
                .FindEntriesAsync();
            return View(alipayTransfer);
        }
        // GET: AlipayTransfers/Create
        public async Task<ActionResult> Create(int OrderSourseID = 0, string TBID = "", string OrderNo = "")
        {
            ViewBag.OrderSourse = await client.For<OrderSourse>()
                .Filter(t => t.OrderSourseEnableState == EnableState.Enable)
                .OrderBy(t => t.ShowNo)
                .FindEntriesAsync();
            ViewBag.OrderSourseID = OrderSourseID;
            ViewBag.TBID = TBID;
            ViewBag.OrderNo = OrderNo;
            return View();
        }
        // GET: AlipayTransfers/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AlipayTransfer alipayTransfer = null;
            try
            {
                alipayTransfer = await client.For<AlipayTransfer>().Expand(t => t.Logs).Key(id).FindEntryAsync();
            }
            catch { }
            if (alipayTransfer == null)
            {
                return HttpNotFound();
            }
            ViewBag.OrderSourse = await client.For<OrderSourse>()
                .Filter(t => t.OrderSourseEnableState == EnableState.Enable)
                .OrderBy(t => t.ShowNo)
                .FindEntriesAsync();
            return View(alipayTransfer);
        }
        //新增/修改转账记录
        [HttpPost]
        public async Task<string> SaveAlipayTransfer(bool isAdd, AlipayTransfer alipayTransfer)
        {
            if (alipayTransfer == null)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "参数为空！" });
            }
            if (alipayTransfer.OrderSourseID == 0)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "客户来源不能为空！" });
            }
            if (string.IsNullOrEmpty(alipayTransfer.TBID))
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "淘宝ID不能为空！" });
            }
            //if (alipayTransfer.TransferTypeValue == TransferType.DifferenceReturn && string.IsNullOrEmpty(alipayTransfer.TBNum))
            //{
            //    return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "退差价必填订单号！" });
            //}
            if (string.IsNullOrEmpty(alipayTransfer.ReceiveAddress))
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "收款账号不能为空！" });
            }
            if (string.IsNullOrEmpty(alipayTransfer.TransferReason))
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "转账原因不能为空！" });
            }

            if (isAdd)//新增
            {
                alipayTransfer.TransferStateValue = TransferState.New;
                alipayTransfer = await client.For<AlipayTransfer>().Set(alipayTransfer).InsertEntryAsync();

                string userName = User.Identity.Name;
                User user = await client.For<User>().Filter(u => u.UserName == userName).FindEntryAsync();

                AlipayTransferLog log = new AlipayTransferLog();
                log.AlipayTransferID = alipayTransfer.AlipayTransferID;
                log.Operate = TransferOperate.Add;
                log.OperateTime = DateTime.Now;
                log.UserID = user.UserID;
                log.UserName = user.NickName;
                await client.For<AlipayTransferLog>().Set(log).InsertEntryAsync();
            }
            else//修改
            {
                if (alipayTransfer.AlipayTransferID == 0)
                {
                    return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "转账记录不能为空！" });
                }
                AlipayTransfer alipayTransferOld = await client.For<AlipayTransfer>().Expand(s => s.Sourse).Key(alipayTransfer.AlipayTransferID).FindEntryAsync();
                if (alipayTransferOld.TransferStateValue != TransferState.New)
                {
                    return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "核实后不允许修改！" });
                }

                string Remark = "";
                if (alipayTransferOld.OrderSourseID != alipayTransfer.OrderSourseID)
                {
                    var sourse = await client.For<OrderSourse>().Key(alipayTransfer.OrderSourseID).FindEntryAsync();
                    Remark += "<div>客户来源：" + alipayTransferOld.Sourse.OrderSourseName + "→" + sourse.OrderSourseName + "</div>";
                }
                if (alipayTransferOld.ReceiveAddress != alipayTransfer.ReceiveAddress)
                {
                    Remark += "<div>收款账号：" + alipayTransferOld.ReceiveAddress + "→" + alipayTransfer.ReceiveAddress + "</div>";
                }
                if (alipayTransferOld.TBID != alipayTransfer.TBID)
                {
                    Remark += "<div>淘宝ID：" + alipayTransferOld.TBID + "→" + alipayTransfer.TBID + "</div>";
                }
                if (alipayTransferOld.OrderNo != alipayTransfer.OrderNo)
                {
                    Remark += "<div>系统订单号：" + alipayTransferOld.OrderNo + "→" + alipayTransfer.OrderNo + "</div>";
                }
                if (alipayTransferOld.ReceiveName != alipayTransfer.ReceiveName)
                {
                    Remark += "<div>收款人姓名：" + alipayTransferOld.ReceiveName + "→" + alipayTransfer.ReceiveName + "</div>";
                }
                if (alipayTransferOld.TransferTypeValue != alipayTransfer.TransferTypeValue)
                {
                    Remark += "<div>转账类型：" + EnumHelper.GetEnumDescription(alipayTransferOld.TransferTypeValue) + "→" + EnumHelper.GetEnumDescription(alipayTransfer.TransferTypeValue) + "</div>";
                }
                if (alipayTransferOld.TransferNum != alipayTransfer.TransferNum)
                {
                    Remark += "<div>转账金额：" + alipayTransferOld.TransferNum + "元→" + alipayTransfer.TransferNum + "元" + "</div>";
                }
                if (alipayTransferOld.TransferReason != alipayTransfer.TransferReason)
                {
                    Remark += "<div>转账原因：" + alipayTransferOld.TransferReason + "→" + alipayTransfer.TransferReason + "</div>";
                }
                if (Remark == "")
                {
                    return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "什么东西都没有改哦！" });
                }
                alipayTransfer.TransferStateValue = TransferState.New;
                alipayTransfer.Remark = alipayTransferOld.Remark;
                await client.For<AlipayTransfer>().Key(alipayTransfer.AlipayTransferID).Set(alipayTransfer).UpdateEntryAsync();

                string userName = User.Identity.Name;
                User user = await client.For<User>().Filter(u => u.UserName == userName).FindEntryAsync();

                AlipayTransferLog log = new AlipayTransferLog();
                log.AlipayTransferID = alipayTransfer.AlipayTransferID;
                log.Operate = TransferOperate.Edit;
                log.OperateTime = DateTime.Now;
                log.UserID = user.UserID;
                log.UserName = user.NickName;
                log.Remark = Remark;
                await client.For<AlipayTransferLog>().Set(log).InsertEntryAsync();
            }

            return JsonConvert.SerializeObject(new { ErrorCode = 200, ErrorMessage = "OK" });
        }
        //获取转账列表
        public async Task<string> GetAlipayTransfer(SearchModel search)
        {
            int draw = 1;
            int start = 0;
            int length = 10;
            string propertyName = "AlipayTransferID";
            int sort = 0;
            string status = string.Empty;
            if (search.length > 0)
            {
                draw = search.draw;
                start = search.start;
                length = search.length;
            }
            var beforeMouthFirstDay = DateTimeOffset.Parse(DateTime.Now.ToString("yyyy-MM-01")).AddMonths(-1);
            var thisMouthFirstDay = DateTimeOffset.Parse(DateTime.Now.ToString("yyyy-MM-01"));
            var nextMouthFirstDay = DateTimeOffset.Parse(DateTime.Now.ToString("yyyy-MM-01")).AddMonths(1);

            //计算本月已转账
            double TransferThisMonth = 0;
            var alipayTransferThisMonth = await client.For<AlipayTransfer>()
                .Filter(t => t.TransferStateValue == TransferState.Transfer)
                .Filter(t => t.TransferTime >= thisMouthFirstDay && t.TransferTime < nextMouthFirstDay)
                .Select(t => t.TransferNum).FindEntriesAsync();
            alipayTransferThisMonth.ForEach(t => TransferThisMonth += t.TransferNum);
            //计算上月已转账
            double TransferBeforeMonth = 0;
            var alipayTransferBeforeMonth = await client.For<AlipayTransfer>()
                .Filter(t => t.TransferStateValue == TransferState.Transfer)
                .Filter(t => t.TransferTime >= beforeMouthFirstDay && t.TransferTime < thisMouthFirstDay)
                .Select(t => t.TransferNum).FindEntriesAsync();
            alipayTransferBeforeMonth.ForEach(t => TransferBeforeMonth += t.TransferNum);
            //计算待转账金额
            double CheckTransfer = 0;
            var alipayCheckTransfer = await client.For<AlipayTransfer>()
                .Filter(t => t.TransferStateValue == TransferState.Check)
                .Select(t => t.TransferNum).FindEntriesAsync();
            alipayCheckTransfer.ForEach(t => CheckTransfer += t.TransferNum);

            IBoundClient<AlipayTransfer> alipayTransfer = client.For<AlipayTransfer>().Expand(t => t.Logs).Expand(t => t.Sourse).Skip(start).Top(length);
            IBoundClient<AlipayTransfer> alipayTransferCount = client.For<AlipayTransfer>();

            alipayTransfer = sort == 0 ? alipayTransfer.OrderByDescending(propertyName) : alipayTransfer.OrderBy(propertyName);

            if (search.AlipaySearch != null)
            {
                if (search.AlipaySearch.TransferStateValue > 0)
                {
                    int state = search.AlipaySearch.TransferStateValue - 1;
                    alipayTransfer = alipayTransfer.Filter(t => t.TransferStateValue == (TransferState)state);
                    alipayTransferCount = alipayTransferCount.Filter(t => t.TransferStateValue == (TransferState)state);
                }
                if (search.AlipaySearch.TransferTypeValue > 0)
                {
                    int state = search.AlipaySearch.TransferTypeValue - 1;
                    alipayTransfer = alipayTransfer.Filter(t => t.TransferTypeValue == (TransferType)state);
                    alipayTransferCount = alipayTransferCount.Filter(t => t.TransferTypeValue == (TransferType)state);
                }
                if (!string.IsNullOrEmpty(search.AlipaySearch.FuzzySearch))
                {
                    alipayTransfer = alipayTransfer.Filter(t => t.TBID.Contains(search.AlipaySearch.FuzzySearch) || t.ReceiveName.Contains(search.AlipaySearch.FuzzySearch) || t.ReceiveAddress.Contains(search.AlipaySearch.FuzzySearch));
                    alipayTransferCount = alipayTransferCount.Filter(t => t.TBID.Contains(search.AlipaySearch.FuzzySearch) || t.ReceiveName.Contains(search.AlipaySearch.FuzzySearch) || t.ReceiveAddress.Contains(search.AlipaySearch.FuzzySearch));
                }
            }

            int count = await alipayTransferCount.Count().FindScalarAsync<int>();
            var items = await alipayTransfer.FindEntriesAsync();

            var data = items.Select(item => new
            {
                AlipayTransferID = item.AlipayTransferID,
                OrderSourseName = item.Sourse.OrderSourseName,
                ReceiveAddress = item.ReceiveAddress == null ? "" : item.ReceiveAddress,
                ReceiveName = item.ReceiveName == null ? "" : item.ReceiveName,
                Remark = item.Remark == null ? "" : item.Remark,
                TBID = item.TBID,
                TBNum = item.TBNum == null ? "" : item.TBNum,
                OrderNo = item.OrderNo == null ? "" : item.OrderNo,
                TransferNum = item.TransferNum,
                TransferReason = item.TransferReason == null ? "" : item.TransferReason,
                TransferStateValue = item.TransferStateValue,
                TransferStateName = EnumHelper.GetEnumDescription(item.TransferStateValue),
                TransferTypeValue = item.TransferTypeValue,
                TransferTypeName = EnumHelper.GetEnumDescription(item.TransferTypeValue),
                CreateTime = (item.Logs != null && item.Logs.Where(l => l.Operate == TransferOperate.Add).FirstOrDefault() != null)
                                ? item.Logs.Where(l => l.Operate == TransferOperate.Add).FirstOrDefault().OperateTime.ToString("yyyy-MM-dd") : "",
                CreateName = (item.Logs != null && item.Logs.Where(l => l.Operate == TransferOperate.Add).FirstOrDefault() != null)
                                ? item.Logs.Where(l => l.Operate == TransferOperate.Add).FirstOrDefault().UserName : "",
                //m.TransferTime = item.TransferTime.ToString("yyyy-MM-dd"),
            });
            return JsonConvert.SerializeObject(new
            {
                draw = draw,
                recordsFiltered = count,
                TransferBeforeMonth = (float)TransferBeforeMonth,
                TransferThisMonth = (float)TransferThisMonth,
                CheckTransfer = (float)CheckTransfer,
                SearchModel = search,
                data = data
            });
        }
        //检查3个月内是否有相同淘宝ID和相同类型的记录 @AlipayTransferID
        public async Task<string> SelectCount(int? id)
        {
            if (id == null)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "id不能为空！" });
            }
            AlipayTransfer one = await client.For<AlipayTransfer>().Key(id).FindEntryAsync();
            DateTimeOffset month = DateTimeOffset.Now.Date.AddDays(-90);
            var count = await client.For<AlipayTransfer>().Filter(t => t.TBID == one.TBID && t.TransferTypeValue == one.TransferTypeValue)
                .Filter(t => t.TransferStateValue == TransferState.New || t.TransferStateValue == TransferState.Check || (t.TransferStateValue == TransferState.Transfer && t.TransferTime > month))
                .Count().FindScalarAsync<int>();
            return JsonConvert.SerializeObject(new { ErrorCode = 200, ErrorMessage = "OK", count = count, TBID = one.TBID });
        }
        //状态流转
        [HttpPost]
        public async Task<string> UpdateState(int state, int id)
        {
            if (id == 0)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "id不能为空！" });
            }

            string userName = User.Identity.Name;
            User user = await client.For<User>().Expand("UserRole/MenuRights").Filter(u => u.UserName == userName).FindEntryAsync();
            if (user.UserRole != null)
            {
                bool isCheck = false;//核实按钮权限
                bool isTransfer = false;//转账按钮权限
                bool isDelete = false;//删除按钮权限
                foreach (var item in user.UserRole.Where(s => s.RoleEnableState == EnableState.Enable))
                {
                    //超级管理员
                    if (item.RoleID == 1)
                    {
                        isCheck = true;
                        isTransfer = true;
                        isDelete = true;
                        break;
                    }
                    if (item.MenuRights != null)
                    {
                        foreach (var MenuRight in item.MenuRights)
                        {
                            var MenuResult = await client.For<MenuRight>().Expand(s => s.RoleRights).Key(MenuRight.MenuRightID).FindEntryAsync();
                            foreach (var rights in MenuResult.RoleRights)
                            {
                                if (rights.ControllerName == "AlipayTransfers" && rights.ActionName == "CheckAlipayTransfer")
                                {
                                    isCheck = true;
                                }
                                if (rights.ControllerName == "AlipayTransfers" && rights.ActionName == "TransferAlipayTransfer")
                                {
                                    isTransfer = true;
                                }
                                if (rights.ControllerName == "AlipayTransfers" && rights.ActionName == "DeleteAlipayTransfer")
                                {
                                    isDelete = true;
                                }
                            }
                        }
                    }
                }
                AlipayTransfer one = await client.For<AlipayTransfer>().Expand(s => s.Logs).Key(id).FindEntryAsync();
                //创建操作记录
                AlipayTransferLog log = new AlipayTransferLog();

                switch ((TransferState)state)//操作
                {
                    case TransferState.Check:
                        if (isCheck)
                        {
                            if (one.TransferStateValue == TransferState.New)//只有待核实状态才能进行核实操作
                            {
                                var CreateID = (one.Logs != null && one.Logs.Where(l => l.Operate == TransferOperate.Add).FirstOrDefault() != null)
                                    ? one.Logs.Where(l => l.Operate == TransferOperate.Add).FirstOrDefault().UserID : 0;
                                if (CreateID == user.UserID && !user.UserRole.Any(s => s.RoleID == 1))
                                {
                                    return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "操作失败！不能核实自己创建的转账记录" });
                                }
                                else
                                {
                                    one.TransferStateValue = TransferState.Check;
                                    log.Operate = TransferOperate.Check;
                                }
                            }
                            else return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "操作失败！只有待核实状态才能进行核实操作" });
                        }
                        else return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "对不起，您没有核实转账记录的权限！" });
                        break;
                    case TransferState.Transfer:
                        if (isTransfer)
                        {
                            if (one.TransferStateValue == TransferState.Check)//只有待转账状态才能进行转账操作
                            {
                                one.TransferStateValue = TransferState.Transfer;
                                one.TransferTime = DateTime.Now;
                                log.Operate = TransferOperate.Transfer;
                            }
                            else return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "操作失败！只有待转账状态才能进行转账操作" });
                        }
                        else return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "对不起，您没有转账的权限！" });
                        break;
                    case TransferState.IsDelete:
                        if (isDelete)
                        {
                            //只有待核实和待转账状态才能进行作废操作
                            if (one.TransferStateValue == TransferState.New || one.TransferStateValue == TransferState.Check)
                            {
                                one.TransferStateValue = TransferState.IsDelete;
                                log.Operate = TransferOperate.Delete;
                            }
                            else return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "操作失败！只有待核实和待转账状态才能进行作废操作" });
                        }
                        else return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "对不起，您没有作废转账记录的权限！" });
                        break;
                    default:
                        return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "操作异常！" });
                }

                await client.For<AlipayTransfer>().Key(id).Set(one).UpdateEntryAsync();

                log.AlipayTransferID = id;
                log.OperateTime = DateTime.Now;
                log.UserID = user.UserID;
                log.UserName = user.NickName;
                await client.For<AlipayTransferLog>().Set(log).InsertEntryAsync();

                await client.For<SystemLog>().Set(new { Operate = "(支付宝转账)" + EnumHelper.GetEnumDescription(log.Operate), OperateTime = DateTime.Now, UserID = user.UserID, UserName = user.NickName, Remark = id }).InsertEntryAsync();

                return JsonConvert.SerializeObject(new { ErrorCode = 200, ErrorMessage = "OK" });

            }
            else
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "对不起，您没有权限！" });
            }
        }
        //修改备注
        [HttpPost]
        public async Task<string> UpdateRemark(int? id, string Remark)
        {
            if (id == null)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "修改失败！失败原因：参数不正确" });
            }
            try
            {
                AlipayTransfer one = await client.For<AlipayTransfer>().Key(id).FindEntryAsync();
                string OldRemark = one.Remark;
                //有修改才保存
                if (OldRemark != Remark)
                {
                    one.Remark = Remark;
                    await client.For<AlipayTransfer>().Key(id).Set(one).UpdateEntryAsync();

                    //获取当前用户，创建操作记录
                    string userName = User.Identity.Name;
                    User user = await client.For<User>().Filter(u => u.UserName == userName).FindEntryAsync();
                    AlipayTransferLog log = new AlipayTransferLog();
                    log.AlipayTransferID = one.AlipayTransferID;
                    log.Operate = TransferOperate.Remark;
                    log.OperateTime = DateTime.Now;
                    log.Remark = "备注由\"" + OldRemark + "\"改为\"" + Remark + "\"";
                    log.UserID = user.UserID;
                    log.UserName = user.NickName;
                    await client.For<AlipayTransferLog>().Set(log).InsertEntryAsync();

                    await client.For<SystemLog>().Set(new { Operate = "(支付宝转账)" + EnumHelper.GetEnumDescription(log.Operate), OperateTime = DateTime.Now, UserID = user.UserID, UserName = user.NickName, Remark = "备注由\"" + OldRemark + "\"改为\"" + Remark + "\"" }).InsertEntryAsync();
                }
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 400, ErrorMessage = "修改失败！失败原因：" + ex.Message });
            }
            return JsonConvert.SerializeObject(new { ErrorCode = 200, ErrorMessage = "OK" });
        }
        //检查订单号存在吗
        public async Task<string> CheckOrderNo(string OrderNo)
        {
            var one = await client.For<Order>().Filter(s => s.OrderNo == OrderNo).FindEntryAsync();
            if (one == null)
                return JsonConvert.SerializeObject(new { ErrorCode = 200, ErrorMessage = "OK", check = "false" });
            else
                return JsonConvert.SerializeObject(new { ErrorCode = 200, ErrorMessage = "OK", check = "true" });
        }
    }
}
