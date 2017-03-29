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
using Newtonsoft.Json;
using System.Configuration;
using LanghuaNew.Models;
using System.IO;

namespace LanghuaNew.Controllers
{
    public class HotalsController : Controller
    {
        private ODataClient client = new ODataClient(ConfigurationManager.AppSettings["ServicePath"] + "odata/");
        //酒店信息
        public async Task<string> GetHotal(string Str, int? CityID)
        {
            CityID = CityID == null ? 0 : CityID;
            try
            {
                Str = Str == null ? "" : Str;
                IEnumerable<Hotal> hotals = await client.For<Hotal>()
                    .Expand(u => u.HotalArea)
                    .Filter(u => u.HotalName.Contains(Str))
                    .Filter(u => u.HotalArea.CityID == CityID)
                    .Top(15)
                    .FindEntriesAsync();
                return JsonConvert.SerializeObject(new { ErrorCode = 200, ErrorMessage = "OK", hotals });
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 400, ErrorMessage = ex, hotals = "[]" });
            }
        }
        //区域信息
        public async Task<string> GetArea(int? CityID)
        {
            CityID = CityID == null ? 0 : CityID;
            try
            {
                IEnumerable<Area> area = await client.For<Area>()
                    .Filter(u => u.CityID == CityID)
                    .Filter(u => u.AreaEnableState == EnableState.Enable)
                    .FindEntriesAsync();
                return JsonConvert.SerializeObject(new { ErrorCode = 200, ErrorMessage = "OK", area });
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 400, ErrorMessage = ex, area = "[]" });
            }
        }
        //城市信息
        public async Task<string> GetCity(int? id)
        {
            id = id == null ? 0 : id;
            try
            {
                IEnumerable<City> city = await client.For<City>()
                    .Filter(u => u.CountryID == id)
                    .FindEntriesAsync();
                return JsonConvert.SerializeObject(new { ErrorCode = 200, ErrorMessage = "OK", city });
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 400, ErrorMessage = ex, city = "[]" });
            }
        }
        public async Task<string> GetHotals(SearchModel search)
        {
            int draw = 1;
            int start = 0;
            int length = 50;
            string FuzzySearch = string.Empty;
            if (search.length > 0)
            {
                draw = search.draw;
                start = search.start;
                length = search.length;
            }
            if (search.RuleSearch != null)
            {
                FuzzySearch = search.RuleSearch.FuzzySearch;
            }
            var Result = client.For<Hotal>();
            var ResultCount = client.For<Hotal>();
            if (!string.IsNullOrEmpty(FuzzySearch))
            {
                Result = Result.Filter(t => t.HotalName.Contains(FuzzySearch) || t.HotalPhone.Contains(FuzzySearch) || t.HotalAddress.Contains(FuzzySearch));
                ResultCount = ResultCount.Filter(t => t.HotalName.Contains(FuzzySearch) || t.HotalPhone.Contains(FuzzySearch) || t.HotalAddress.Contains(FuzzySearch));
            }
            int count = await ResultCount.Count().FindScalarAsync<int>();
            var hotals = await Result.OrderByDescending(t => t.HotalID).Skip(start).Top(length).FindEntriesAsync();
            List<Hotal> data = new List<Hotal>();
            foreach (var item in hotals)
            {
                item.HotalArea = await client.For<Area>().Expand(t => t.AreaCity.CityCountry).Key(item.AreaID).FindEntryAsync();
                data.Add(item);
            }
            return JsonConvert.SerializeObject(new { draw = draw, recordsFiltered = count, data = data, SearchModel = search });
        }
        [HttpPost]
        public async Task<string> UpdateDisable(string id, string Operation)
        {
            if (string.IsNullOrEmpty(id))
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "酒店不能为空！" });
            }
            if (string.IsNullOrEmpty(Operation))
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "操作异常！" });
            }
            try
            {
                var HotalID = id.Split(',');
                if (Operation.Trim().ToLower() == "delete")
                {
                    foreach (var i in HotalID)
                    {
                        await client.For<Hotal>().Key(int.Parse(i)).DeleteEntryAsync();
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
        // GET: Hotals
        public ActionResult Index()
        {
            return View();
        }

        // GET: Hotals/Create
        public async Task<ActionResult> Create()
        {
            ViewBag.CountryID =  new SelectList(await client.For<Country>().FindEntriesAsync(), "CountryID", "CountryName");
            return View();
        }

        // POST: Hotals/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "HotalID,HotalName,HotalPhone,HotalAddress,AreaID")] Hotal hotal)
        {
            if (ModelState.IsValid)
            {
                await client.For<Hotal>().Set(hotal).InsertEntryAsync();
                return RedirectToAction("Index");
            }

            ViewBag.CountryID =  new SelectList(await client.For<Country>().FindEntriesAsync(), "CountryID", "CountryName");
            return View(hotal);
        }

        // GET: Hotals/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Hotal hotal = await client.For<Hotal>().Expand(t => t.HotalArea.AreaCity).Key(id).FindEntryAsync();
            if (hotal == null)
            {
                return HttpNotFound();
            }
            ViewBag.CountryID = new SelectList(await client.For<Country>().FindEntriesAsync(), "CountryID", "CountryName", hotal.HotalArea.AreaCity.CountryID);
            ViewBag.CityID = new SelectList(await client.For<City>().Filter(c=>c.CountryID== hotal.HotalArea.AreaCity.CountryID).FindEntriesAsync(), "CityID", "CityName", hotal.HotalArea.CityID);
            ViewBag.AreaID = new SelectList(await client.For<Area>().Filter(a=>a.CityID== hotal.HotalArea.CityID).FindEntriesAsync(), "AreaID", "AreaName", hotal.AreaID);
            return View(hotal);
        }

        // POST: Hotals/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "HotalID,HotalName,HotalPhone,HotalAddress,AreaID")] Hotal hotal)
        {
            if (ModelState.IsValid)
            {
                await client.For<Hotal>().Key(hotal.HotalID).Set(hotal).UpdateEntryAsync();
                return RedirectToAction("Index");
            }
            ViewBag.CountryID = new SelectList(await client.For<Country>().FindEntriesAsync(), "CountryID", "CountryName");
            return View(hotal);
        }
        //列表导出
        public async Task<FileResult> ExportExcel(RuleSearchModel search)
        {
            // 获取列表
            string FuzzySearch = search.FuzzySearch;

            var Result = client.For<Hotal>();
            if (!string.IsNullOrEmpty(FuzzySearch))
            {
                Result = Result.Filter(t => t.HotalName.Contains(FuzzySearch) || t.HotalPhone.Contains(FuzzySearch) || t.HotalAddress.Contains(FuzzySearch));
            }
            var hotals = await Result.OrderByDescending(t => t.HotalID).FindEntriesAsync();
            foreach (var item in hotals)
            {
                item.HotalArea = await client.For<Area>().Expand(t => t.AreaCity.CityCountry).Key(item.AreaID).FindEntryAsync();
            }

            //创建Excel文件的对象
            NPOI.HSSF.UserModel.HSSFWorkbook book = new NPOI.HSSF.UserModel.HSSFWorkbook();
            //添加一个sheet
            NPOI.SS.UserModel.ISheet sheet1 = book.CreateSheet("Sheet1");

            int i = 0;
            //给sheet1添加第一行的头部标题
            NPOI.SS.UserModel.IRow row1 = sheet1.CreateRow(i);

            row1.CreateCell(0).SetCellValue("国家");
            row1.CreateCell(1).SetCellValue("省市");
            row1.CreateCell(2).SetCellValue("区域");
            row1.CreateCell(3).SetCellValue("酒店名称");
            row1.CreateCell(4).SetCellValue("电话");
            row1.CreateCell(5).SetCellValue("地址");

            row1.Height = 450;

            //将数据逐步写入sheet1各个行
            foreach (var item in hotals)
            {
                item.HotalArea = await client.For<Area>().Expand(t => t.AreaCity.CityCountry).Key(item.AreaID).FindEntryAsync();

                i++;
                NPOI.SS.UserModel.IRow rowtemp = sheet1.CreateRow(i);

                rowtemp.CreateCell(0).SetCellValue(item.HotalArea.AreaCity.CityCountry.CountryName);
                rowtemp.CreateCell(1).SetCellValue(item.HotalArea.AreaCity.CityName);
                rowtemp.CreateCell(2).SetCellValue(item.HotalArea.AreaName);
                rowtemp.CreateCell(3).SetCellValue(item.HotalName);
                rowtemp.CreateCell(4).SetCellValue(item.HotalPhone);
                rowtemp.CreateCell(5).SetCellValue(item.HotalAddress);
            }

            // 写入到客户端 
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            book.Write(ms);
            ms.Seek(0, SeekOrigin.Begin);
            return File(ms, "application/vnd.ms-excel", "hotallist.xls");
        }
    }
}
