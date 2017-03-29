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

namespace LanghuaNew.Controllers
{
    public class CurrenciesController : Controller
    {
        private ODataClient client = new ODataClient(ConfigurationManager.AppSettings["ServicePath"] + "odata/");
        //获取币别
        public async Task<string> GetCurrencies(SearchModel search)
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
            var Result = client.For<Currency>();
            var ResultCount = client.For<Currency>();
            if (status == 1)
            {
                Result = Result.Filter(t => t.CurrencyEnableState == EnableState.Enable);
                ResultCount = ResultCount.Filter(t => t.CurrencyEnableState == EnableState.Enable);
            }
            else if (status == 2)
            {
                Result = Result.Filter(t => t.CurrencyEnableState == EnableState.Disable);
                ResultCount = ResultCount.Filter(t => t.CurrencyEnableState == EnableState.Disable);
            }
            if (!string.IsNullOrEmpty(FuzzySearch))
            {
                Result = Result.Filter(t => t.CurrencyName.Contains(FuzzySearch) || t.CurrencyNo.Contains(FuzzySearch));
                ResultCount = ResultCount.Filter(t => t.CurrencyName.Contains(FuzzySearch) || t.CurrencyNo.Contains(FuzzySearch));
            }
            int count = await ResultCount.Count().FindScalarAsync<int>();
            var currencies = await Result.OrderBy(t => t.CurrencyID).Skip(start).Top(length).FindEntriesAsync();

            return JsonConvert.SerializeObject(new { draw = draw, recordsFiltered = count, data = currencies, SearchModel = search });
        }
        //启用禁用
        [HttpPost]
        public async Task<string> UpdateDisable(string id, string Operation)
        {
            if (string.IsNullOrEmpty(id))
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "货币不能为空！" });
            }
            if (string.IsNullOrEmpty(Operation))
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "操作异常！" });
            }
            try
            {
                var CurrencyID = id.Split(',');
                if (Operation.Trim() == "0")
                {
                    foreach (var i in CurrencyID)
                    {
                        var oldItem = await client.For<Currency>().Key(int.Parse(i)).FindEntryAsync();
                        oldItem.CurrencyEnableState = EnableState.Enable;
                        await client.For<Currency>().Key(int.Parse(i)).Set(oldItem).UpdateEntryAsync();
                    }
                }
                else if (Operation.Trim() == "1")
                {
                    foreach (var i in CurrencyID)
                    {
                        var oldItem = await client.For<Currency>().Key(int.Parse(i)).FindEntryAsync();
                        oldItem.CurrencyEnableState = EnableState.Disable;
                        await client.For<Currency>().Key(int.Parse(i)).Set(oldItem).UpdateEntryAsync();
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
            return JsonConvert.SerializeObject(new { ErrorCode = 200, ErrorMessage = "OK" });
        }
        // GET: Currencies
        public ActionResult Index()
        {
            return View();
        }
        
        // GET: Currencies/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Currencies/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "CurrencyID,CurrencyName,ExchangeRate,CurrencyChangeType,CurrencyEnableState,CurrencyNo")] Currency currency)
        {
            if (ModelState.IsValid)
            {
                await client.For<Currency>().Set(currency).InsertEntryAsync();
                return RedirectToAction("Index");
            }

            return View(currency);
        }

        // GET: Currencies/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Currency currency = await client.For<Currency>().Key(id).FindEntryAsync();
            if (currency == null)
            {
                return HttpNotFound();
            }
            return View(currency);
        }

        // POST: Currencies/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "CurrencyID,CurrencyName,ExchangeRate,CurrencyChangeType,CurrencyEnableState,CurrencyNo")] Currency currency)
        {
            if (ModelState.IsValid)
            {
                await client.For<Currency>().Key(currency.CurrencyID).Set(currency).UpdateEntryAsync();
                return RedirectToAction("Index");
            }
            return View(currency);
        }

        //列表导出
        public async Task<FileResult> ExportExcel(RuleSearchModel search)
        {
            // 获取列表
            int status = search.status;
            string FuzzySearch = search.FuzzySearch;
            var Result = client.For<Currency>();
            if (status == 1)
            {
                Result = Result.Filter(t => t.CurrencyEnableState == EnableState.Enable);
            }
            else if (status == 2)
            {
                Result = Result.Filter(t => t.CurrencyEnableState == EnableState.Disable);
            }
            if (!string.IsNullOrEmpty(FuzzySearch))
            {
                Result = Result.Filter(t => t.CurrencyName.Contains(FuzzySearch) || t.CurrencyNo.Contains(FuzzySearch));
            }
            var currencies = await Result.OrderBy(t => t.CurrencyID).FindEntriesAsync();

            //创建Excel文件的对象
            NPOI.HSSF.UserModel.HSSFWorkbook book = new NPOI.HSSF.UserModel.HSSFWorkbook();
            //添加一个sheet
            NPOI.SS.UserModel.ISheet sheet1 = book.CreateSheet("Sheet1");

            int i = 0;
            //给sheet1添加第一行的头部标题
            NPOI.SS.UserModel.IRow row1 = sheet1.CreateRow(i);

            row1.CreateCell(0).SetCellValue("货币名称");
            row1.CreateCell(1).SetCellValue("货币代码");
            row1.CreateCell(2).SetCellValue("外币-人民币");
            row1.CreateCell(3).SetCellValue("人民币-外币");
            row1.CreateCell(4).SetCellValue("状态");

            row1.Height = 450;

            //将数据逐步写入sheet1各个行
            foreach (var item in currencies)
            {
                i++;
                NPOI.SS.UserModel.IRow rowtemp = sheet1.CreateRow(i);

                rowtemp.CreateCell(0).SetCellValue(item.CurrencyName);
                rowtemp.CreateCell(1).SetCellValue(item.CurrencyNo);
                rowtemp.CreateCell(2).SetCellValue(item.CurrencyChangeType == ChangeType.ToChina ? item.ExchangeRate.ToString() : "");
                rowtemp.CreateCell(3).SetCellValue(item.CurrencyChangeType == ChangeType.FromChina ? item.ExchangeRate.ToString() : "");
                rowtemp.CreateCell(4).SetCellValue(item.CurrencyEnableState == EnableState.Enable ? "启用" : "禁用");
            }

            // 写入到客户端 
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            book.Write(ms);
            ms.Seek(0, SeekOrigin.Begin);
            return File(ms, "application/vnd.ms-excel", "currencylist.xls");
        }
    }
}
