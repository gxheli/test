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
using System.Net.Http;
using Commond;
using System.IO;

namespace LanghuaNew.Controllers
{
    public class ServiceRulesController : Controller
    {
        private ODataClient client = new ODataClient(ConfigurationManager.AppSettings["ServicePath"] + "odata/");

        // GET: ServiceRules
        public ActionResult Index(string search)
        {
            ViewBag.search = search;
            return View();
        }
        // GET: ServiceRules/Create
        public ActionResult Create()
        {
            return View();
        }
        // GET: ServiceRules/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ServiceRule serviceRule = await client.For<ServiceRule>().Expand(s => s.RuleServiceItem).Key(id).FindEntryAsync();
            if (serviceRule == null)
            {
                return HttpNotFound();
            }
            return View(serviceRule);
        }
        // GET: ServiceRules/RulesOperation/5
        public async Task<ActionResult> RulesOperation(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var log = await client.For<ServiceRuleLog>().Filter(s => s.ServiceRuleID == id).OrderByDescending(s => s.OperateTime).FindEntriesAsync();
            return View(log);
        }
        //获取规则列表
        public async Task<string> GetRules(SearchModel search)
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
            var Result = client.For<ServiceRule>().Expand(t => t.RuleServiceItem);
            var ResultCount = client.For<ServiceRule>().Expand(t => t.RuleServiceItem);
            if (search.OrderBy != null && !string.IsNullOrEmpty(search.OrderBy.PropertyName))
            {
                if (search.OrderBy.OrderBy==0)
                {
                    Result = Result.OrderBy(search.OrderBy.PropertyName);
                }
                else
                {
                    Result = Result.OrderByDescending(search.OrderBy.PropertyName);
                }
            }
            if (status == 1)
            {
                Result = Result.Filter(t => t.UseState == EnableState.Enable);
                ResultCount = ResultCount.Filter(t => t.UseState == EnableState.Enable);
            }
            else if (status == 2)
            {
                Result = Result.Filter(t => t.UseState == EnableState.Disable);
                ResultCount = ResultCount.Filter(t => t.UseState == EnableState.Disable);
            }
            if (!string.IsNullOrEmpty(FuzzySearch))
            {
                Result = Result.Filter(t => t.RuleServiceItem.Any(r => r.cnItemName.Contains(FuzzySearch) || r.enItemName.Contains(FuzzySearch) || r.ServiceCode.Contains(FuzzySearch)));
                ResultCount = ResultCount.Filter(t => t.RuleServiceItem.Any(r => r.cnItemName.Contains(FuzzySearch) || r.enItemName.Contains(FuzzySearch) || r.ServiceCode.Contains(FuzzySearch)));
            }
            int count = await ResultCount.Count().FindScalarAsync<int>();
            var Rules = await Result.OrderByDescending(t => t.ServiceRuleID).Skip(start).Top(length).FindEntriesAsync();

            return JsonConvert.SerializeObject(new { draw = draw, recordsFiltered = count, data = Rules, SearchModel = search });
        }
        //新增规则
        public async Task<string> SaveRule(ServiceRule rule)
        {
            if (rule.StartTime < DateTime.Parse("1901-01-02"))
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 400, ErrorMessage = "规则启用时间不能为空" });
            }
            if (rule.EndTime < DateTime.Parse("1901-01-02"))
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 400, ErrorMessage = "规则结束时间不能为空" });
            }
            string Operate = "";
            if (rule.ServiceRuleID > 0)
            {
                Operate = "规则修改";
            }
            else
            {
                Operate = "规则新增";
            }
            HttpResponseMessage Message = await HttpHelper.PostAction("ServiceRulesExtend", JsonConvert.SerializeObject(rule));
            string result = Message.Content.ReadAsStringAsync().Result;
            int? id = null;
            try
            {
                id = int.Parse(result);
            }
            catch { }
            if (id != null)
            {
                string userName = User.Identity.Name;
                User user = await client.For<User>().Filter(u => u.UserName == userName).FindEntryAsync();
                //规则日志
                await client.For<ServiceRuleLog>().Set(new { Operate = Operate, OperateTime = DateTime.Now, UserID = user.UserID, UserName = user.NickName, Remark = "", ServiceRuleID = id }).InsertEntryAsync();
                //系统日志
                await client.For<SystemLog>().Set(new { Operate = Operate, OperateTime = DateTime.Now, UserID = user.UserID, UserName = user.NickName, Remark = "" }).InsertEntryAsync();

                return JsonConvert.SerializeObject(new { ErrorCode = 200, ErrorMessage = "OK" });
            }
            else
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 400, ErrorMessage = result, rule });
            }
        }
        //启用禁用
        [HttpPost]
        public async Task<string> UpdateDisable(string RuleID, string Operation)
        {
            var failed = (new int[] { 1 }).Select(x => new { name = "", reason = "" }).ToList();
            failed.Clear();
            if (string.IsNullOrEmpty(RuleID))
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "规则不能为空！" });
            }
            if (string.IsNullOrEmpty(Operation))
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "操作异常！" });
            }
            try
            {
                string userName = User.Identity.Name;
                User user = await client.For<User>().Filter(u => u.UserName == userName).FindEntryAsync();
                var id = RuleID.Split(',');
                if (Operation.Trim() == "0")
                {
                    foreach (var i in id)
                    {
                        var oldItem = await client.For<ServiceRule>().Key(int.Parse(i)).FindEntryAsync();
                        oldItem.UseState = EnableState.Enable;
                        await client.For<ServiceRule>().Key(int.Parse(i)).Set(oldItem).UpdateEntryAsync();

                        //规则日志
                        await client.For<ServiceRuleLog>().Set(new { Operate = "启用规则", OperateTime = DateTime.Now, UserID = user.UserID, UserName = user.NickName, Remark = "", ServiceRuleID = int.Parse(i) }).InsertEntryAsync();
                        //系统日志
                        await client.For<SystemLog>().Set(new { Operate = "启用规则", OperateTime = DateTime.Now, UserID = user.UserID, UserName = user.NickName, Remark = "" }).InsertEntryAsync();

                    }
                }
                else if (Operation.Trim() == "1")
                {
                    foreach (var i in id)
                    {
                        var oldItem = await client.For<ServiceRule>().Key(int.Parse(i)).FindEntryAsync();
                        oldItem.UseState = EnableState.Disable;
                        await client.For<ServiceRule>().Key(int.Parse(i)).Set(oldItem).UpdateEntryAsync();

                        //规则日志
                        await client.For<ServiceRuleLog>().Set(new { Operate = "禁用规则", OperateTime = DateTime.Now, UserID = user.UserID, UserName = user.NickName, Remark = "", ServiceRuleID = int.Parse(i) }).InsertEntryAsync();
                        //系统日志
                        await client.For<SystemLog>().Set(new { Operate = "禁用规则", OperateTime = DateTime.Now, UserID = user.UserID, UserName = user.NickName, Remark = "" }).InsertEntryAsync();
                    }
                }
                else if (Operation.Trim().ToLower() == "delete")
                {
                    foreach (var i in id)
                    {
                        var oldItem = await client.For<ServiceRule>().Key(int.Parse(i)).FindEntryAsync();
                        if (oldItem.UseState == EnableState.Disable)
                        {
                            await client.For<ServiceRule>().Key(int.Parse(i)).DeleteEntryAsync();
                            //系统日志
                            await client.For<SystemLog>().Set(new { Operate = "删除规则", OperateTime = DateTime.Now, UserID = user.UserID, UserName = user.NickName, Remark = "" }).InsertEntryAsync();
                        }
                        else
                        {
                            failed.Add(new { name = "删除失败", reason = "只能删除禁用状态的规则。请先禁用再删除。" });
                        }
                    }
                }
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
            #region 获取列表
            int status = search.status;
            string FuzzySearch = search.FuzzySearch;

            var Result = client.For<ServiceRule>().Expand(t => t.RuleServiceItem);
            if (status == 1)
            {
                Result = Result.Filter(t => t.UseState == EnableState.Enable);
            }
            else if (status == 2)
            {
                Result = Result.Filter(t => t.UseState == EnableState.Disable);
            }
            if (!string.IsNullOrEmpty(FuzzySearch))
            {
                Result = Result.Filter(t => t.RuleServiceItem.Any(r => r.cnItemName.Contains(FuzzySearch) || r.enItemName.Contains(FuzzySearch) || r.ServiceCode.Contains(FuzzySearch)));
            }
            var Rules = await Result.OrderByDescending(t => t.ServiceRuleID).FindEntriesAsync();
            #endregion

            //创建Excel文件的对象
            NPOI.HSSF.UserModel.HSSFWorkbook book = new NPOI.HSSF.UserModel.HSSFWorkbook();
            //添加一个sheet
            NPOI.SS.UserModel.ISheet sheet1 = book.CreateSheet("Sheet1");

            int i = 0;
            //给sheet1添加第一行的头部标题
            NPOI.SS.UserModel.IRow row1 = sheet1.CreateRow(i);

            row1.CreateCell(0).SetCellValue("适用产品");
            row1.CreateCell(1).SetCellValue("出行范围(开始)");
            row1.CreateCell(2).SetCellValue("出行范围(结束)");
            row1.CreateCell(3).SetCellValue("规则内容");
            row1.CreateCell(4).SetCellValue("状态");

            row1.Height = 450;

            //将数据逐步写入sheet1各个行
            foreach (var item in Rules)
            {
                string items = "";
                foreach (var te in item.RuleServiceItem)
                {
                    if (items != "")
                    {
                        items += " | ";
                    }
                    items += te.cnItemName + " " + te.ServiceCode;
                }

                var str = "";
                str += item.RuleUseTypeValue == 0 ? "只允许：" : "禁止：";
                switch (item.SelectRuleType)
                {
                    case RuleType.ByDateRange:
                        str += item.RangeStart.ToString("yyyy-MM-dd") + "~" + item.RangeEnd.ToString("yyyy-MM-dd");
                        break;
                    case RuleType.ByWeek:
                        var weekstr = "";
                        var week = item.Week.Split('|');
                        foreach (var w in week)
                        {
                            if (weekstr != "")
                            {
                                weekstr += " |";
                            }
                            switch (w)
                            {
                                case "1":
                                    weekstr += "星期一";
                                    break;
                                case "2":
                                    weekstr += "星期二";
                                    break;
                                case "3":
                                    weekstr += "星期三";
                                    break;
                                case "4":
                                    weekstr += "星期四";
                                    break;
                                case "5":
                                    weekstr += "星期五";
                                    break;
                                case "6":
                                    weekstr += "星期六";
                                    break;
                                case "0":
                                    weekstr += "星期日";
                                    break;
                            }
                        }
                        str += weekstr;
                        break;
                    case RuleType.ByDouble:
                        str += item.IsDouble ? "双号" : "单号";
                        break;
                    case RuleType.ByDate:
                        str += item.UseDate;
                        break;
                }

                i++;
                NPOI.SS.UserModel.IRow rowtemp = sheet1.CreateRow(i);

                rowtemp.CreateCell(0).SetCellValue(items);
                rowtemp.CreateCell(1).SetCellValue(item.StartTime.ToString("yyyy-MM-dd"));
                rowtemp.CreateCell(2).SetCellValue(item.EndTime.ToString("yyyy-MM-dd"));
                rowtemp.CreateCell(3).SetCellValue(str);
                rowtemp.CreateCell(4).SetCellValue(EnumHelper.GetEnumDescription(item.UseState));
            }

            // 写入到客户端 
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            book.Write(ms);
            ms.Seek(0, SeekOrigin.Begin);
            return File(ms, "application/vnd.ms-excel", "rulelist.xls");
        }

        //根据产品获取规则列表
        public async Task<string> GetRulesByItemID(int id)
        {
            var Result = await client.For<ServiceRule>().Filter(t => t.RuleServiceItem.Any(r => r.ServiceItemID == id) && t.UseState == EnableState.Enable).FindEntriesAsync();
            return JsonConvert.SerializeObject(Result);
        }

        //修改备注
        [HttpPost]
        public async Task<string> UpdateRemark(int? id, string Remark)
        {
            if (id == null)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 400, ErrorMessage = "id不能为空" });
            }
            try
            {
                ServiceRule rule = await client.For<ServiceRule>().Key(id).FindEntryAsync();
                string OldRemark = rule.Remark;
                if (OldRemark != Remark)
                {
                    rule.Remark = Remark;
                    await client.For<ServiceRule>().Key(id).Set(rule).UpdateEntryAsync();

                    //获取当前用户，创建操作记录
                    string userName = User.Identity.Name;
                    User user = await client.For<User>().Filter(u => u.UserName == userName).FindEntryAsync();
                    await client.For<ServiceRuleLog>().Set(new { Operate = "备注", OperateTime = DateTime.Now, UserID = user.UserID, UserName = user.NickName, Remark = "备注由\"" + OldRemark + "\"改为\"" + Remark + "\"", ServiceRuleID = id }).InsertEntryAsync();
                }
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 400, ErrorMessage = "修改失败！失败原因：" + ex.Message });
            }
            return JsonConvert.SerializeObject(new { ErrorCode = 200, ErrorMessage = "OK" });
        }
    }
}
