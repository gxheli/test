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
using System.IO;
using System.Text;
using Entity;

namespace LanghuaNew.Controllers
{
    public class BillReportsController : Controller
    {
        private ODataClient client = new ODataClient(ConfigurationManager.AppSettings["ServicePath"] + "odata/");
        // GET: BillReports
        public async Task<ActionResult> Index()
        {
            var suppliers = await client.For<Supplier>().Filter(s => s.SupplierEnableState == EnableState.Enable)
                .OrderBy(s => s.SupplierNo)
                .FindEntriesAsync();
            ViewBag.suppliers = suppliers;
            return View();
        }
        //获取账单报表
        public async Task<string> GetList(ShareSearchModel search)
        {
            int draw = 1;
            int start = 0;
            int length = 10;
            if (search.length > 0)
            {
                draw = search.draw;
                start = search.start;
                length = search.length;
            }
            IBoundClient<BillReport> bill = client.For<BillReport>().Expand(t => t.oneSupplier).OrderByDescending(t => t.CreateTime).Skip(start).Top(length);
            IBoundClient<BillReport> billCount = client.For<BillReport>();

            if (search.BillSearch != null)
            {
                if (search.BillSearch.status > -1)
                {
                    int state = search.BillSearch.status;
                    bill = bill.Filter(t => t.State == (CheckState)state);
                    billCount = billCount.Filter(t => t.State == (CheckState)state);
                }
                if (search.BillSearch.SupplierID > 0)
                {
                    bill = bill.Filter(t => t.SupplierID == search.BillSearch.SupplierID);
                    billCount = billCount.Filter(t => t.SupplierID == search.BillSearch.SupplierID);
                }
                if (search.BillSearch.datetype > 0 && !string.IsNullOrEmpty(search.BillSearch.date))
                {
                    DateTimeOffset dt = DateTimeOffset.Parse(search.BillSearch.date);
                    DateTimeOffset dtNext = dt.AddDays(1);
                    switch (search.BillSearch.datetype)
                    {
                        case 1://创建时间
                            bill = bill.Filter(t => dt <= t.CreateTime && t.CreateTime <= dtNext);
                            billCount = billCount.Filter(t => dt <= t.CreateTime && t.CreateTime <= dtNext);
                            break;
                        case 2://付款时间
                            bill = bill.Filter(t => dt <= t.PayTime && t.PayTime <= dtNext);
                            billCount = billCount.Filter(t => dt <= t.PayTime && t.PayTime <= dtNext);
                            break;
                        case 3://账单开始日期
                            bill = bill.Filter(t => dt <= t.StartDate && t.StartDate <= dtNext);
                            billCount = billCount.Filter(t => dt <= t.StartDate && t.StartDate <= dtNext);
                            break;
                        case 4://账单结束日期
                            bill = bill.Filter(t => dt <= t.EndDate && t.EndDate <= dtNext);
                            billCount = billCount.Filter(t => dt <= t.EndDate && t.EndDate <= dtNext);
                            break;
                        default:
                            break;
                    }
                }
            }

            int count = await billCount.Count().FindScalarAsync<int>();
            var items = await bill.FindEntriesAsync();
            var data = items.Select(t => new
            {
                t.BillReportID,
                CreateTime = t.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                EndDate = t.EndDate.ToString("yyyy-MM-dd"),
                t.oneSupplier.SupplierNo,
                PayTime = t.PayTime < DateTimeOffset.Parse("1901-01-01") ? "" : t.PayTime.ToString("yyyy-MM-dd HH:mm:ss"),
                Remark = string.IsNullOrEmpty(t.Remark) ? "" : t.Remark,
                StartDate = t.StartDate.ToString("yyyy-MM-dd"),
                t.State,
                StateValue = EnumHelper.GetEnumDescription(t.State),
                TotalReceive = t.TotalReceive,
                Type = t.Type == EffectiveWay.ByCreateTime ? "下单" : "出行",
                RealReceive = t.RealReceive,
                Currency = string.IsNullOrEmpty(t.Currency) ? "" : t.Currency
            });
            return JsonConvert.SerializeObject(new { draw = draw, recordsFiltered = count, data = data, SearchModel = search });
        }
        //生成账单报表
        [HttpPost]
        public async Task<string> CreateBill(int SupplierID, int state, string startdate, string enddate)
        {
            if (string.IsNullOrEmpty(startdate) || string.IsNullOrEmpty(enddate))
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "时间不能为空" });
            }
            try
            {
                DateTimeOffset.Parse(startdate);
                DateTimeOffset.Parse(enddate);
            }
            catch
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "时间格式错误" });
            }
            DateTimeOffset date1900 = DateTimeOffset.Parse("1900-01-01");
            var result = client.For<Order>()
                .Expand(t => t.Customers)
                .Expand(t => t.ServiceItemHistorys.ExtraServiceHistorys)
                .Filter(t => t.ServiceItemHistorys.TravelDate > date1900)
                .Filter(t => t.ServiceItemHistorys.SupplierID == SupplierID && t.state != OrderState.Invalid);

            DateTimeOffset start = DateTimeOffset.Parse(startdate).Date;
            DateTimeOffset end = DateTimeOffset.Parse(enddate).AddDays(1).Date;

            if (state == 1)//出行日期核算
            {
                result.Filter(t => start <= t.ServiceItemHistorys.TravelDate && t.ServiceItemHistorys.TravelDate < end);
            }
            else if (state == 2)//下单日期核算
            {
                result.Filter(t => start <= t.CreateTime && t.CreateTime < end);
            }
            else
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "账单类型异常" });
            }

            var orders = await result.FindEntriesAsync();
            if (orders == null || orders.Count() == 0)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "账单记录为0，请扩大范围" });
            }
            int MaxExtraNum = 1;
            foreach (var item in orders)
            {
                if (item.ServiceItemHistorys.ExtraServiceHistorys != null && item.ServiceItemHistorys.ExtraServiceHistorys.Count > MaxExtraNum)
                {
                    MaxExtraNum = item.ServiceItemHistorys.ExtraServiceHistorys.Count;
                }
            }
            //创建Excel文件的对象
            NPOI.HSSF.UserModel.HSSFWorkbook book = new NPOI.HSSF.UserModel.HSSFWorkbook();
            //添加一个sheet
            NPOI.SS.UserModel.ISheet sheet1 = book.CreateSheet("Sheet1");
            int i = 0;
            //给sheet1添加第一行的头部标题
            NPOI.SS.UserModel.IRow row1 = sheet1.CreateRow(i);
            row1.CreateCell(0).SetCellValue("淘宝ID");
            row1.CreateCell(1).SetCellValue("淘宝订单号");
            row1.CreateCell(2).SetCellValue("系统订单号");
            row1.CreateCell(3).SetCellValue("订单状态");
            row1.CreateCell(4).SetCellValue("预定项目类型");
            row1.CreateCell(5).SetCellValue("姓名");
            row1.CreateCell(6).SetCellValue("订单创建日期");
            row1.CreateCell(7).SetCellValue("出行日期");
            row1.CreateCell(8).SetCellValue("预定项目");
            row1.CreateCell(9).SetCellValue("成人人数");
            row1.CreateCell(10).SetCellValue("儿童人数");
            row1.CreateCell(11).SetCellValue("婴儿人数");
            row1.CreateCell(12).SetCellValue("间数");
            row1.CreateCell(13).SetCellValue("晚数");
            row1.CreateCell(14).SetCellValue("成人单价");
            row1.CreateCell(15).SetCellValue("儿童单价");
            row1.CreateCell(16).SetCellValue("婴儿单价");
            row1.CreateCell(17).SetCellValue("按数量计价单价");
            row1.CreateCell(18).SetCellValue("交通附加费");
            row1.CreateCell(19).SetCellValue("交通附加费币别");
            row1.CreateCell(20).SetCellValue("备注信息");
            row1.CreateCell(21).SetCellValue("附加项目名称");
            row1.CreateCell(22).SetCellValue("附加项目单价");
            row1.CreateCell(23).SetCellValue("附加项目数量");
            row1.CreateCell(24 + (MaxExtraNum - 1) * 3).SetCellValue("小计");
            row1.Height = 450;
            sheet1.SetColumnWidth(0, 3000);
            sheet1.SetColumnWidth(1, 5000);
            sheet1.SetColumnWidth(2, 5000);
            sheet1.SetColumnWidth(3, 2000);
            sheet1.SetColumnWidth(4, 3000);
            sheet1.SetColumnWidth(5, 1500);
            sheet1.SetColumnWidth(6, 3000);
            sheet1.SetColumnWidth(7, 2500);
            sheet1.SetColumnWidth(8, 5000);
            sheet1.SetColumnWidth(9, 2000);
            sheet1.SetColumnWidth(10, 2000);
            sheet1.SetColumnWidth(11, 2000);
            sheet1.SetColumnWidth(12, 1100);
            sheet1.SetColumnWidth(13, 1100);
            sheet1.SetColumnWidth(14, 2000);
            sheet1.SetColumnWidth(15, 2000);
            sheet1.SetColumnWidth(16, 2000);
            sheet1.SetColumnWidth(17, 3500);
            sheet1.SetColumnWidth(18, 2500);
            sheet1.SetColumnWidth(19, 3500);
            sheet1.SetColumnWidth(20, 3000);
            sheet1.SetColumnWidth(21, 3000);
            sheet1.SetColumnWidth(22, 3000);
            sheet1.SetColumnWidth(23, 3000);
            sheet1.SetColumnWidth(24 + (MaxExtraNum - 1) * 3, 3000);
            float total = 0;
            string currency = string.Empty;
            foreach (var order in orders)
            {
                float RowTotal = 0;
                int itemid = order.ServiceItemHistorys.ServiceItemID;
                DateTimeOffset date = state == 1 ? order.ServiceItemHistorys.TravelDate : order.CreateTime;
                ItemPriceBySupplier price = await client.For<ItemPriceBySupplier>()
                    .Filter(s => s.ServiceItem.SupplierID == SupplierID && s.ServiceItem.ServiceItemID == itemid)
                    .Filter(s => s.startTime <= date && date <= s.EndTime)
                    .FindEntryAsync();
                var extras = order.ServiceItemHistorys.ExtraServiceHistorys;
                SupplierServiceItem Sitem = await client.For<SupplierServiceItem>()
                    .Expand(s => s.ItemCurrency)
                    .Filter(s => s.SupplierID == SupplierID && s.ServiceItemID == itemid).FindEntryAsync();
                if (Sitem != null)
                {
                    currency = string.IsNullOrEmpty(currency) ? Sitem.ItemCurrency.CurrencyNo : currency;
                }
                if (price != null)
                {
                    RowTotal += order.ServiceItemHistorys.AdultNum * price.AdultNetPrice;
                    RowTotal += order.ServiceItemHistorys.ChildNum * price.ChildNetPrice;
                    RowTotal += order.ServiceItemHistorys.INFNum * price.BobyNetPrice;
                    RowTotal += order.ServiceItemHistorys.RightNum * order.ServiceItemHistorys.RoomNum * price.Price;
                }
                if (extras != null)
                {
                    foreach (var item in extras)
                    {
                        RowTotal += item.ServiceNum * item.ServicePrice;
                    }
                }
                total += RowTotal;
                i++;
                NPOI.SS.UserModel.IRow rowtemp = sheet1.CreateRow(i);
                rowtemp.CreateCell(0).SetCellValue(order.Customers.CustomerTBCode);
                rowtemp.CreateCell(1).SetCellValue(order.TBNum);
                rowtemp.CreateCell(2).SetCellValue(order.OrderNo);
                rowtemp.CreateCell(3).SetCellValue(EnumHelper.GetEnumDescription(order.state).Substring(0, EnumHelper.GetEnumDescription(order.state).IndexOf("|")));
                rowtemp.CreateCell(4).SetCellValue(order.ServiceItemHistorys.ServiceTypeName);
                rowtemp.CreateCell(5).SetCellValue(order.CustomerName);
                rowtemp.CreateCell(6).SetCellValue(order.CreateTime.ToString("yyyy-MM-dd"));
                rowtemp.CreateCell(7).SetCellValue(order.ServiceItemHistorys.TravelDate < DateTimeOffset.Parse("1901-01-01") ? "" : order.ServiceItemHistorys.TravelDate.ToString("yyyy-MM-dd"));
                rowtemp.CreateCell(8).SetCellValue(order.ServiceItemHistorys.cnItemName);
                rowtemp.CreateCell(9).SetCellValue(order.ServiceItemHistorys.AdultNum);
                rowtemp.CreateCell(10).SetCellValue(order.ServiceItemHistorys.ChildNum);
                rowtemp.CreateCell(11).SetCellValue(order.ServiceItemHistorys.INFNum);
                rowtemp.CreateCell(12).SetCellValue(order.ServiceItemHistorys.RoomNum);
                rowtemp.CreateCell(13).SetCellValue(order.ServiceItemHistorys.RightNum);
                rowtemp.CreateCell(14).SetCellValue(price == null ? 0 : price.AdultNetPrice);
                rowtemp.CreateCell(15).SetCellValue(price == null ? 0 : price.ChildNetPrice);
                rowtemp.CreateCell(16).SetCellValue(price == null ? 0 : price.BobyNetPrice);
                rowtemp.CreateCell(17).SetCellValue(price == null ? 0 : price.Price);
                rowtemp.CreateCell(18).SetCellValue(order.ServiceItemHistorys.TrafficSurcharge);
                rowtemp.CreateCell(19).SetCellValue(order.ServiceItemHistorys.TrafficCurrencyName);
                rowtemp.CreateCell(20).SetCellValue(order.Remark == null ? "" : order.Remark);
                if (extras != null && extras.Count > 0)
                {
                    int cellnum = 21;
                    foreach (var extra in extras)
                    {
                        if (cellnum > 23) row1.CreateCell(cellnum).SetCellValue("附加项目名称");
                        sheet1.SetColumnWidth(cellnum, 3000);
                        rowtemp.CreateCell(cellnum).SetCellValue(extra.ServiceName);
                        cellnum++;
                        if (cellnum > 23) row1.CreateCell(cellnum).SetCellValue("附加项目价格");
                        sheet1.SetColumnWidth(cellnum, 3000);
                        rowtemp.CreateCell(cellnum).SetCellValue(extra.ServicePrice);
                        cellnum++;
                        if (cellnum > 23) row1.CreateCell(cellnum).SetCellValue("附加项目数量");
                        sheet1.SetColumnWidth(cellnum, 3000);
                        rowtemp.CreateCell(cellnum).SetCellValue(extra.ServiceNum);
                        cellnum++;
                    }
                }
                rowtemp.CreateCell(24 + (MaxExtraNum - 1) * 3).SetCellValue(RowTotal);
            }
            // 写入到客户端 
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            book.Write(ms);
            ms.Seek(0, SeekOrigin.Begin);
            //return File(ms, "application/vnd.ms-excel", "orderlist.xls");
            string folderPath = "~/data/BillData/" + DateTime.Now.ToString("yyyyMM") + "/";
            string filePhysicalPath = Server.MapPath(folderPath);
            if (!Directory.Exists(filePhysicalPath))//判断上传文件夹是否存在，若不存在，则创建
            {
                Directory.CreateDirectory(filePhysicalPath);//创建文件夹
            }
            string systemPath = folderPath + Guid.NewGuid() + ".dodo";
            string filePath = System.Web.HttpContext.Current.Server.MapPath(systemPath);
            byte[] pre = ms.ToArray().Skip(0).Take(100).ToArray();
            byte[] next = ms.ToArray().Skip(100).Take(ms.ToArray().Length - 100).ToArray();
            using (FileStream fs = new FileStream(filePath, FileMode.Create))
            {
                using (BinaryWriter w = new BinaryWriter(fs))
                {
                    w.Write(next);
                    w.Close();
                }
            }
            BillReport bill = new BillReport();
            bill.CreateTime = DateTimeOffset.Now;
            bill.FileStream = Convert.ToBase64String(pre);
            bill.SupplierID = SupplierID;
            bill.StartDate = start;
            bill.EndDate = end.AddDays(-1);
            bill.State = CheckState.New;
            bill.Type = state == 1 ? EffectiveWay.BySendTime : EffectiveWay.ByCreateTime;
            bill.TotalReceive = total;
            bill.Currency = currency;
            bill.FilePath = systemPath;
            await client.For<BillReport>().Set(bill).InsertEntryAsync();
            //System.Threading.Thread.Sleep(1000);
            return JsonConvert.SerializeObject(new { ErrorCode = 200, ErrorMessage = "OK" });
        }
        //下载明细
        public async Task<FileResult> DownFile(int id)
        {
            List<byte> b3 = new List<byte>();
            string fileName = "";
            try
            {
                BillReport bill = await client.For<BillReport>().Expand(t => t.oneSupplier).Key(id).FindEntryAsync();
                fileName = bill.oneSupplier.SupplierNo + "_" + bill.StartDate.ToString("yyyyMMdd") + "-" + bill.EndDate.ToString("MMdd") + ".xls";
                string filePath = System.Web.HttpContext.Current.Server.MapPath(bill.FilePath);
                string code = bill.FileStream;
                byte[] pre = Convert.FromBase64String(code);
                FileStream fst = new FileStream(filePath, FileMode.Open);
                byte[] buffer = new byte[fst.Length];
                fst.Read(buffer, 0, buffer.Length);
                fst.Seek(0, SeekOrigin.Begin);
                fst.Close();
                b3.AddRange(pre);
                b3.AddRange(buffer);
            }
            catch
            {
                return null;
            }
            return File(b3.ToArray(), "application/vnd.ms-excel", fileName);
        }
        //状态流转
        [HttpPost]
        public async Task<string> UpdateState(int state, int id)
        {
            if (id == 0)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 400, ErrorMessage = "id不能为空！" });
            }
            BillReport bill = await client.For<BillReport>().Key(id).FindEntryAsync();
            var oldstate = bill.State;
            switch ((CheckState)state)
            {
                case CheckState.Check:
                    if (oldstate != CheckState.New)
                    {
                        return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "操作失败！只有待对账状态才能进行对账操作！" });
                    }
                    break;
                case CheckState.Transfer:
                    bill.PayTime = DateTimeOffset.Now;
                    if (oldstate != CheckState.Check)
                    {
                        return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "操作失败！只有待支付状态才能进行支付操作！" });
                    }
                    break;
                case CheckState.IsDelete:
                    if (oldstate != CheckState.New && oldstate != CheckState.Check)
                    {
                        return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "操作失败！只有待对账和待支付状态才能进行作废操作！" });
                    }
                    break;
                default:
                    return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "操作失败！状态异常！" });
            }
            bill.State = (CheckState)state;
            await client.For<BillReport>().Key(id).Set(bill).UpdateEntryAsync();
            return JsonConvert.SerializeObject(new { ErrorCode = 200, ErrorMessage = "OK" });
        }
        //修改备注
        [HttpPost]
        public async Task<string> UpdateRemark(int? id, string Remark, float RealReceive = 0)
        {
            if (id == null)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "ID不能为空" });
            }
            BillReport one = await client.For<BillReport>().Key(id).FindEntryAsync();
            one.RealReceive = RealReceive;
            one.Remark = Remark;
            await client.For<BillReport>().Key(id).Set(one).UpdateEntryAsync();

            return JsonConvert.SerializeObject(new { ErrorCode = 200, ErrorMessage = "OK" });
        }
    }
}
