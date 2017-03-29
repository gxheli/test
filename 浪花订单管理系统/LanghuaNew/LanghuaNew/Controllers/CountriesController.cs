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
using PagedList;
using LanghuaNew.Models;
using Simple.OData.Client;
using System.Configuration;
using Newtonsoft.Json;
using Commond;
using System.Net.Http;
using System.IO;

namespace LanghuaNew.Controllers
{
    [Authorize]
    public class CountriesController : Controller
    {
        private ODataClient client = new ODataClient(ConfigurationManager.AppSettings["ServicePath"] + "odata/");

        // GET: Countries
        public ActionResult Index()
        {
            return View();
        }
        //获取国家区域
        public async Task<string> GetCountries(SearchModel search)
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
            var Result = client.For<Area>().Expand(t=>t.AreaCity.CityCountry);
            var ResultCount = client.For<Area>();
            if (status == 1)
            {
                Result = Result.Filter(t => t.AreaEnableState == EnableState.Enable);
                ResultCount = ResultCount.Filter(t => t.AreaEnableState == EnableState.Enable);
            }
            else if (status == 2)
            {
                Result = Result.Filter(t => t.AreaEnableState == EnableState.Disable);
                ResultCount = ResultCount.Filter(t => t.AreaEnableState == EnableState.Disable);
            }
            if (!string.IsNullOrEmpty(FuzzySearch))
            {
                Result = Result.Filter(t => t.AreaName.Contains(FuzzySearch) || t.AreaEnName.Contains(FuzzySearch) || t.AreaCity.CityName.Contains(FuzzySearch) || t.AreaCity.CityEnName.Contains(FuzzySearch) || t.AreaCity.CityCountry.CountryName.Contains(FuzzySearch) || t.AreaCity.CityCountry.CountryEnName.Contains(FuzzySearch));
                ResultCount = ResultCount.Filter(t => t.AreaName.Contains(FuzzySearch) || t.AreaEnName.Contains(FuzzySearch) || t.AreaCity.CityName.Contains(FuzzySearch) || t.AreaCity.CityEnName.Contains(FuzzySearch) || t.AreaCity.CityCountry.CountryName.Contains(FuzzySearch) || t.AreaCity.CityCountry.CountryEnName.Contains(FuzzySearch));
            }
            int count = await ResultCount.Count().FindScalarAsync<int>();
            var Countries = await Result.OrderByDescending(t => t.CityID).Skip(start).Top(length).FindEntriesAsync();

            return JsonConvert.SerializeObject(new { draw = draw, recordsFiltered = count, data = Countries, SearchModel = search });
        }
        //启用禁用
        [HttpPost]
        public async Task<string> UpdateDisable(string id, string Operation)
        {
            if (string.IsNullOrEmpty(id))
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "区域不能为空！" });
            }
            if (string.IsNullOrEmpty(Operation))
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "操作异常！" });
            }
            try
            {
                var AreaID = id.Split(',');
                if (Operation.Trim() == "0")
                {
                    foreach (var i in AreaID)
                    {
                        var oldItem = await client.For<Area>().Key(int.Parse(i)).FindEntryAsync();
                        oldItem.AreaEnableState = EnableState.Enable;
                        await client.For<Area>().Key(int.Parse(i)).Set(oldItem).UpdateEntryAsync();
                    }
                }
                else if (Operation.Trim() == "1")
                {
                    foreach (var i in AreaID)
                    {
                        var oldItem = await client.For<Area>().Key(int.Parse(i)).FindEntryAsync();
                        oldItem.AreaEnableState = EnableState.Disable;
                        await client.For<Area>().Key(int.Parse(i)).Set(oldItem).UpdateEntryAsync();
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

        // GET: Countries/Create
        public async Task<ActionResult> Create()
        {
            ViewBag.Countries = await client.For<Country>().FindEntriesAsync();
            return View();
        }
        //保存国家区域
        [HttpPost]
        public async Task<string> Save(Country country)
        {
            if (country.CountryID == 0 && (string.IsNullOrEmpty(country.CountryName) || string.IsNullOrEmpty(country.CountryEnName)))
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "国家名称不能为空！" });
            }
            if (country.Citys == null)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "省市不能为空！" });
            }
            if (country.Citys[0].CityID == 0 && (string.IsNullOrEmpty(country.Citys[0].CityName) || string.IsNullOrEmpty(country.Citys[0].CityEnName)))
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "省市名称不能为空！" });
            }
            if (country.Citys[0].Areas == null)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "区域不能为空！" });
            }

            int CityID = country.Citys[0].CityID;
            if (CityID > 0)
            {                
                foreach (var area in country.Citys[0].Areas)
                {
                    Area one = await client.For<Area>().Filter(a => (a.AreaName == area.AreaName || a.AreaEnName == area.AreaEnName) && a.CityID == CityID).FindEntryAsync();
                    if (one != null)
                    {
                        return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = one.AreaEnName + "-" + one.AreaName + "区域已存在！" });
                    }
                }
            }
            try
            {
                HttpResponseMessage Message = await HttpHelper.PostAction("CountriesExtend", JsonConvert.SerializeObject(country));
                string exMessage = Message.Content.ReadAsStringAsync().Result;
                if (exMessage != "OK")
                {
                    return JsonConvert.SerializeObject(new { ErrorCode = 400, ErrorMessage = exMessage });
                }
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 400, ErrorMessage = "出错啦！出错原因：" + ex.Message });
            }
            return JsonConvert.SerializeObject(new { ErrorCode = 200, ErrorMessage = "OK" });
        }
        // GET: Countries/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Area area = await client.For<Area>().Key(id).Expand(t => t.AreaCity).FindEntryAsync();

            if (area == null)
            {
                return HttpNotFound();
            }
            ViewBag.CountryID = new SelectList(await client.For<Country>().FindEntriesAsync(), "CountryID", "CountryName", area.AreaCity.CountryID);
            ViewBag.CityID = new SelectList(await client.For<City>().Filter(c => c.CountryID == area.AreaCity.CountryID).FindEntriesAsync(), "CityID", "CityName", area.CityID);
            return View(area);
        }

        // POST: Countries/Edit/5
        //修改国家区域
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "AreaID,AreaName,AreaEnName,AreaEnableState,CityID")] Area area)
        {
            if (ModelState.IsValid)
            {
                Area one = await client.For<Area>().Filter(a => (a.AreaName == area.AreaName || a.AreaEnName == area.AreaEnName) && a.CityID == area.CityID && a.AreaID != area.AreaID).FindEntryAsync();
                if (one != null)
                {
                    ModelState.AddModelError("", one.AreaEnName + "-" + one.AreaName + "区域已存在！");
                }
                else
                {
                    await client.For<Area>().Key(area.AreaID).Set(area).UpdateEntryAsync();
                    return RedirectToAction("Index");
                }
            }
            ViewBag.CountryID = new SelectList(await client.For<Country>().FindEntriesAsync(), "CountryID", "CountryName");
            ViewBag.CityID = new SelectList(await client.For<City>().FindEntriesAsync(), "CityID", "CityName", area.CityID);
            return View(area);
        }
        //列表导出
        public async Task<FileResult> ExportExcel(RuleSearchModel search)
        {
            // 获取列表
            int status = search.status;
            string FuzzySearch = search.FuzzySearch;
            var Result = client.For<Area>().Expand(t => t.AreaCity.CityCountry);
            if (status == 1)
            {
                Result = Result.Filter(t => t.AreaEnableState == EnableState.Enable);
            }
            else if (status == 2)
            {
                Result = Result.Filter(t => t.AreaEnableState == EnableState.Disable);
            }
            if (!string.IsNullOrEmpty(FuzzySearch))
            {
                Result = Result.Filter(t => t.AreaName.Contains(FuzzySearch) || t.AreaEnName.Contains(FuzzySearch));
            }
            var Countries = await Result.OrderByDescending(t => t.CityID).FindEntriesAsync();

            //创建Excel文件的对象
            NPOI.HSSF.UserModel.HSSFWorkbook book = new NPOI.HSSF.UserModel.HSSFWorkbook();
            //添加一个sheet
            NPOI.SS.UserModel.ISheet sheet1 = book.CreateSheet("Sheet1");

            int i = 0;
            //给sheet1添加第一行的头部标题
            NPOI.SS.UserModel.IRow row1 = sheet1.CreateRow(i);

            row1.CreateCell(0).SetCellValue("国家");
            row1.CreateCell(1).SetCellValue("省市");
            row1.CreateCell(2).SetCellValue("区域中文");
            row1.CreateCell(3).SetCellValue("区域英文");
            row1.CreateCell(4).SetCellValue("状态");

            row1.Height = 450;

            //将数据逐步写入sheet1各个行
            foreach (var item in Countries)
            {
                i++;
                NPOI.SS.UserModel.IRow rowtemp = sheet1.CreateRow(i);

                rowtemp.CreateCell(0).SetCellValue(item.AreaCity.CityCountry.CountryName);
                rowtemp.CreateCell(1).SetCellValue(item.AreaCity.CityName);
                rowtemp.CreateCell(2).SetCellValue(item.AreaName);
                rowtemp.CreateCell(3).SetCellValue(item.AreaEnName);
                rowtemp.CreateCell(4).SetCellValue(item.AreaEnableState == EnableState.Enable ? "启用" : "禁用");
            }

            // 写入到客户端 
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            book.Write(ms);
            ms.Seek(0, SeekOrigin.Begin);
            return File(ms, "application/vnd.ms-excel", "countrylist.xls");
        }

        //检查国家是否存在
        public async Task<string> CheckCountry(Country country)
        {
            Country one = await client.For<Country>().Filter(c => c.CountryName == country.CountryName || c.CountryEnName == country.CountryEnName).FindEntryAsync();
            if (one != null)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 200, ErrorMessage = "OK", check = true });
            }
            else
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 200, ErrorMessage = "OK", check = false });
            }            
        }
        //检查省市是否存在
        public async Task<string> CheckCity(City city)
        {
            if (city.CountryID == 0)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 200, ErrorMessage = "OK", check = false });
            }
            City one = await client.For<City>().Filter(c => c.CountryID == city.CountryID && (c.CityName == city.CityName || c.CityEnName == city.CityEnName)).FindEntryAsync();
            if (one != null)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 200, ErrorMessage = "OK", check = true });
            }
            else
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 200, ErrorMessage = "OK", check = false });
            }
        }
    }
}
