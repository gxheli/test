using Commond;
using Entity;
using LanghuaNew.Data;
using Newtonsoft.Json;
using Simple.OData.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace LanghuaForSup.Controllers
{
    public class ProductController : Controller
    {
        private ODataClient client = new ODataClient(ConfigurationManager.AppSettings["ServicePath"] + "odata/");
        // GET: Product
        public async Task<ActionResult> Index()
        {
            string SupplierUserName = User.Identity.Name;
            SupplierUser user = await client.For<SupplierUser>().Filter(s => s.SupplierUserName == SupplierUserName).FindEntryAsync();
            ViewBag.type = await client.For<ServiceType>().FindEntriesAsync();
            //待确认价格数量
            ViewBag.ChangeNum = await client.For<SupplierServiceItem>()
                .Filter(s => s.IsChange && s.SupplierID == user.SupplierID && s.Service.ServiceItemEnableState == EnableState.Enable)
                .Count().FindScalarAsync<int>();
            return View();
        }
        //获取产品&价格列表
        public async Task<string> GetItemPrices(ShareSearchModel share)
        {
            string SupplierUserName = User.Identity.Name;
            SupplierUser user = await client.For<SupplierUser>().Filter(s => s.SupplierUserName == SupplierUserName).FindEntryAsync();
            int draw = 1;
            int start = 0;
            int length = 50;
            if (share.length > 0)
            {
                draw = share.draw;
                start = share.start;
                length = share.length;
            }
            var Result = client.For<SupplierServiceItem>()
                .Expand(s => s.Service.ItemServiceType)
                .Expand(s => s.ItemCurrency)
                .Expand(s => s.ItemPriceBySuppliers)
                .Expand("ExtraServicePrices/Service");
            var ResultCount = client.For<SupplierServiceItem>();
            Result = Result.Filter(s => s.SupplierID == user.SupplierID && s.Service.ServiceItemEnableState == EnableState.Enable);
            ResultCount = ResultCount.Filter(s => s.SupplierID == user.SupplierID && s.Service.ServiceItemEnableState == EnableState.Enable);
            if (share.ItemPriceSearch != null && share.ItemPriceSearch.IsChange)
            {
                Result = Result.Filter(s => s.IsChange);
                ResultCount = ResultCount.Filter(s => s.IsChange);
            }
            if (share.ItemPriceSearch != null && !string.IsNullOrEmpty(share.ItemPriceSearch.FuzzySearch))
            {
                Result = Result.Filter(s => s.Service.cnItemName.Contains(share.ItemPriceSearch.FuzzySearch) || s.Service.enItemName.Contains(share.ItemPriceSearch.FuzzySearch) || s.Service.ServiceCode.Contains(share.ItemPriceSearch.FuzzySearch));
                ResultCount = ResultCount.Filter(s => s.Service.cnItemName.Contains(share.ItemPriceSearch.FuzzySearch) || s.Service.enItemName.Contains(share.ItemPriceSearch.FuzzySearch) || s.Service.ServiceCode.Contains(share.ItemPriceSearch.FuzzySearch));
            }
            if (share.ItemPriceSearch != null && share.ItemPriceSearch.ServiceTypeID > 0)
            {
                Result = Result.Filter(s => s.Service.ServiceTypeID == share.ItemPriceSearch.ServiceTypeID);
                ResultCount = ResultCount.Filter(s => s.Service.ServiceTypeID == share.ItemPriceSearch.ServiceTypeID);
            }
            int count = await ResultCount.Count().FindScalarAsync<int>();
            var list = await Result.OrderByDescending(s => s.Service.ServiceCode).Skip(start).Top(length).FindEntriesAsync();
            var data = list.Select(s => new
            {
                s.IsChange,
                s.PayType,
                s.SupplierServiceItemID,
                s.ServiceItemID,
                s.Service.ServiceCode,
                s.Service.cnItemName,
                TravelCompany = s.Service.TravelCompany == null ? "" : s.Service.TravelCompany,
                s.Service.ItemServiceType.ServiceTypeName,
                s.ItemCurrency.CurrencyNo,
                PriceList = s.ItemPriceBySuppliers == null ? null : s.ItemPriceBySuppliers//.Where(price => price.startTime <= DateTimeOffset.Now.Date && DateTimeOffset.Now.Date <= price.EndTime)
                                .Select(price => new { price.AdultNetPrice, price.BobyNetPrice, price.ChildNetPrice, price.Price, StartDate = price.startTime.ToString("yyyy-MM-dd"), EndDate = price.EndTime.ToString("yyyy-MM-dd") }),
                ExtraServicePrices = s.ExtraServicePrices == null ? null : s.ExtraServicePrices.Select(e => new { e.Service.ServiceName, e.ServicePrice })
            });
            return JsonConvert.SerializeObject(new { draw = draw, recordsFiltered = count, data = data, SearchModel = share });
        }
        //导出excel
        public async Task<FileResult> ExportExcel(int? ServiceTypeID, string FuzzySearch, bool? IsChange)
        {
            string SupplierUserName = User.Identity.Name;
            SupplierUser user = await client.For<SupplierUser>().Filter(s => s.SupplierUserName == SupplierUserName).FindEntryAsync();
            var Result = client.For<SupplierServiceItem>()
                .Expand(s => s.Service.ItemServiceType)
                .Expand(s => s.ItemCurrency)
                .Expand(s => s.ItemPriceBySuppliers)
                .Expand("ExtraServicePrices/Service");
            Result = Result.Filter(s => s.SupplierID == user.SupplierID);
            if (IsChange == true)
            {
                Result = Result.Filter(s => s.IsChange);
            }
            if (!string.IsNullOrEmpty(FuzzySearch))
            {
                Result = Result.Filter(s => s.Service.cnItemName.Contains(FuzzySearch) || s.Service.enItemName.Contains(FuzzySearch) || s.Service.ServiceCode.Contains(FuzzySearch));
            }
            if (ServiceTypeID > 0)
            {
                Result = Result.Filter(s => s.Service.ServiceTypeID == ServiceTypeID);
            }
            var list = await Result.OrderByDescending(s => s.Service.ServiceCode).FindEntriesAsync();

            //创建Excel文件的对象
            NPOI.HSSF.UserModel.HSSFWorkbook book = new NPOI.HSSF.UserModel.HSSFWorkbook();
            //添加一个sheet
            NPOI.SS.UserModel.ISheet sheet1 = book.CreateSheet("Sheet1");

            int i = 0;
            //给sheet1添加第一行的头部标题
            NPOI.SS.UserModel.IRow row1 = sheet1.CreateRow(i);

            row1.CreateCell(0).SetCellValue("编码");
            row1.CreateCell(1).SetCellValue("中文名");
            row1.CreateCell(2).SetCellValue("行程公司");
            row1.CreateCell(3).SetCellValue("类型");
            row1.CreateCell(4).SetCellValue("币别");
            row1.CreateCell(5).SetCellValue("价格开始日期");
            row1.CreateCell(6).SetCellValue("价格结束日期");
            row1.CreateCell(7).SetCellValue("成人");
            row1.CreateCell(8).SetCellValue("儿童");
            row1.CreateCell(9).SetCellValue("婴儿");
            row1.CreateCell(10).SetCellValue("单价");
            row1.CreateCell(11).SetCellValue("价格状态");

            row1.Height = 450;

            //将数据逐步写入sheet1各个行
            foreach (var item in list)
            {
                i++;
                NPOI.SS.UserModel.IRow rowtemp = sheet1.CreateRow(i);

                rowtemp.CreateCell(0).SetCellValue(item.Service.ServiceCode);
                rowtemp.CreateCell(1).SetCellValue(item.Service.cnItemName);
                rowtemp.CreateCell(2).SetCellValue(item.Service.TravelCompany);
                rowtemp.CreateCell(3).SetCellValue(item.Service.ItemServiceType.ServiceTypeName);
                rowtemp.CreateCell(4).SetCellValue(item.ItemCurrency.CurrencyNo);
                if (item.ItemPriceBySuppliers != null && item.ItemPriceBySuppliers.Find(price => price.startTime <= DateTimeOffset.Now.Date && DateTimeOffset.Now.Date <= price.EndTime) != null)
                {
                    var one = item.ItemPriceBySuppliers.Find(price => price.startTime <= DateTimeOffset.Now.Date && DateTimeOffset.Now.Date <= price.EndTime);
                    rowtemp.CreateCell(5).SetCellValue(one.startTime.ToString("yyyy-MM-dd"));
                    rowtemp.CreateCell(6).SetCellValue(one.EndTime.ToString("yyyy-MM-dd"));
                    rowtemp.CreateCell(7).SetCellValue(one.AdultNetPrice);
                    rowtemp.CreateCell(8).SetCellValue(one.ChildNetPrice);
                    rowtemp.CreateCell(9).SetCellValue(one.BobyNetPrice);
                    rowtemp.CreateCell(10).SetCellValue(one.Price);
                }
                rowtemp.CreateCell(11).SetCellValue(item.IsChange ? "待确认" : "已确认");
                if (item.ExtraServicePrices != null && item.ExtraServicePrices.Count > 0)
                {
                    foreach (var Extra in item.ExtraServicePrices)
                    {
                        i++;
                        NPOI.SS.UserModel.IRow rowExtra = sheet1.CreateRow(i);
                        rowExtra.CreateCell(1).SetCellValue(Extra.Service.ServiceName);
                        rowExtra.CreateCell(10).SetCellValue(Extra.ServicePrice);
                    }
                }
            }
            // 写入到客户端 
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            book.Write(ms);
            ms.Seek(0, SeekOrigin.Begin);
            return File(ms, "application/vnd.ms-excel", "pricelist.xls");
        }
        //确认价格
        public async Task<string> ConfirmPrice(int? SupplierServiceItemChangeID, int SupplierServiceItemID)
        {
            if (SupplierServiceItemChangeID > 0)//价格变更确认
            {
                try
                {
                    await client.For<SupplierServiceItemChange>().Key(SupplierServiceItemChangeID).FindEntryAsync();
                }
                catch
                {
                    return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "价格有变，请刷新再试" });
                }
                HttpResponseMessage Message = await HttpHelper.PutAction("SupplierServiceItemsExtend", SupplierServiceItemChangeID.ToString());
                string Err = Message.Content.ReadAsStringAsync().Result;
                try
                {
                    int.Parse(Err);
                }
                catch
                {
                    return JsonConvert.SerializeObject(new { ErrorCode = 400, ErrorMessage = Err });
                }
            }
            else//价格新增确认
            {
                try
                {
                    var one = await client.For<SupplierServiceItem>().Key(SupplierServiceItemID).FindEntryAsync();
                    //如果处于待确认状态
                    if (one.IsChange)
                    {
                        one.IsChange = false;
                        await client.For<SupplierServiceItem>().Key(SupplierServiceItemID).Set(one).UpdateEntryAsync();
                    }
                    else
                    {
                        return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "价格已被确认" });
                    }
                }
                catch(Exception ex)
                {
                    return JsonConvert.SerializeObject(new { ErrorCode = 400, ErrorMessage = "操作异常：" + ex.Message });
                }
            }
            var item = await client.For<SupplierServiceItem>().Expand(s => new { s.Service, s.ItemSupplier, s.ItemPriceLogs })
                        .Key(SupplierServiceItemID).FindEntryAsync();
            string SupplierUserName = User.Identity.Name;
            SupplierUser user = await client.For<SupplierUser>().Filter(s => s.SupplierUserName == SupplierUserName).FindEntryAsync();
            await client.For<ItemPriceLog>().Set(new ItemPriceLog
            {
                Operate = "确认产品价格",
                OperateTime = DateTimeOffset.Now,
                UserID = user.SupplierUserID,
                UserName = user.SupplierNickName,
                Operator = OrderOperator.supplier,
                SupplierServiceItemID = SupplierServiceItemID
            }).InsertEntryAsync();
            string toMail = ConfigurationManager.AppSettings["confirmtomail"];
            string Detail = item.ItemPriceLogs.Where(s => s.Operate != "填写卖价") == null ? ""
                : item.ItemPriceLogs.Where(s => s.Operate != "填写卖价").OrderByDescending(s => s.ItemPriceLogID).First().Remark;
            if (!string.IsNullOrEmpty(toMail))
            {
                foreach (var to in toMail.Split(','))
                {
                    await EmailHelper.PriceChangeConfirm(to, item.Service.cnItemName, item.ItemSupplier.SupplierName, Detail);
                }
            }
            return JsonConvert.SerializeObject(new { ErrorCode = 200, ErrorMessage = "OK" });
        }
        //获取产品价格
        public async Task<string> GetPrice(int ItemID)
        {
            string SupplierUserName = User.Identity.Name;
            SupplierUser user = await client.For<SupplierUser>().Filter(s => s.SupplierUserName == SupplierUserName).FindEntryAsync();
            int SupplierID = user.SupplierID;
            var result = await client.For<ServiceItem>().Expand(s => s.ExtraServices).Key(ItemID).FindEntryAsync();
            SupplierServiceItem item = await client.For<SupplierServiceItem>()
                .Expand(i => i.ItemPriceBySuppliers)
                .Expand(i => i.ExtraServicePrices)
                .Expand(i => i.ItemCurrency)
                .Filter(i => i.ServiceItemID == ItemID & i.SupplierID == SupplierID)
                .Select(s => new
                {
                    s.CurrencyID,
                    s.ExtraServicePrices,
                    s.IsChange,
                    s.ItemCurrency.CurrencyName,
                    s.ItemPriceBySuppliers,
                    s.PayType,
                    s.Remark,
                    s.SelectEffectiveWay,
                    s.SupplierServiceItemID
                }).FindEntryAsync();
            SupplierServiceItemChange changeitem = await client.For<SupplierServiceItemChange>()
                .Expand(i => i.ItemPriceBySuppliers)
                .Expand(i => i.ExtraServicePrices)
                .Expand(i => i.ItemCurrency)
                .Filter(i => i.ServiceItemID == ItemID & i.SupplierID == SupplierID)
                .Select(s => new
                {
                    s.CurrencyID,
                    s.ExtraServicePrices,
                    s.ItemCurrency.CurrencyName,
                    s.ItemCurrency.CurrencyNo,
                    s.ItemPriceBySuppliers,
                    s.PayType,
                    s.Remark,
                    s.SelectEffectiveWay,
                    s.SupplierServiceItemChangeID
                }).FindEntryAsync();
            return JsonConvert.SerializeObject(new { ErrorCode = 200, ErrorMessage = "OK", data = new { item, changeitem, result.ExtraServices, baseinfo = new { result.cnItemName, result.ServiceCode } } });
        }
        public async Task<ActionResult> PriceOperation(int id)
        {
            string SupplierUserName = User.Identity.Name;
            SupplierUser user = await client.For<SupplierUser>().Filter(s => s.SupplierUserName == SupplierUserName).FindEntryAsync();

            return View(await client.For<ItemPriceLog>()
                .Filter(s => s.SupplierServiceItemID == id)
                .Filter(s => s.ServiceItem.SupplierID == user.SupplierID)
                .Filter(s => s.Operate != "填写卖价")
                .OrderByDescending(oo => oo.ItemPriceLogID)
                .FindEntriesAsync());
        }
    }
}
