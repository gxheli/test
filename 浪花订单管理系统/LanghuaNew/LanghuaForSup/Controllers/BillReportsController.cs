using Entity;
using LanghuaNew.Data;
using Newtonsoft.Json;
using Simple.OData.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace LanghuaForSup.Controllers
{
    public class BillReportsController : Controller
    {
        private ODataClient client = new ODataClient(ConfigurationManager.AppSettings["ServicePath"] + "odata/");
        // GET: BillReports
        public ActionResult Index()
        {
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
            IBoundClient<BillReport> bill = client.For<BillReport>().OrderByDescending(t => t.CreateTime).Skip(start).Top(length);
            IBoundClient<BillReport> billCount = client.For<BillReport>();

            string userName = User.Identity.Name;
            SupplierUser user = await client.For<SupplierUser>().Filter(u => u.SupplierUserName == userName).FindEntryAsync();
            bill = bill.Filter(t => t.SupplierID == user.SupplierID);
            billCount = billCount.Filter(t => t.SupplierID == user.SupplierID);

            if (search.BillSearch != null)
            {
                if (search.BillSearch.status > -1)
                {
                    int state = search.BillSearch.status;
                    bill = bill.Filter(t => t.State == (CheckState)state);
                    billCount = billCount.Filter(t => t.State == (CheckState)state);
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
                PayTime = t.PayTime < DateTimeOffset.Parse("1901-01-01") ? "" : t.PayTime.ToString("yyyy-MM-dd HH:mm:ss"),
                StartDate = t.StartDate.ToString("yyyy-MM-dd"),
                t.State,
                StateValue = EnumHelper.GetEnumDescription(t.State),
                t.TotalReceive,
                Type = t.Type == EffectiveWay.ByCreateTime ? "下单" : "出行",
                t.RealReceive,
                Currency = string.IsNullOrEmpty(t.Currency) ? "" : t.Currency
            });
            return JsonConvert.SerializeObject(new { draw = draw, recordsFiltered = count, data = data, SearchModel = search });
        }
        //下载明细
        public async Task<FileResult> DownFile(int id)
        {
            List<byte> b3 = new List<byte>();
            string fileName = "";
            try
            {
                string userName = User.Identity.Name;
                SupplierUser user = await client.For<SupplierUser>().Filter(u => u.SupplierUserName == userName).FindEntryAsync();
                BillReport bill = await client.For<BillReport>().Expand(t => t.oneSupplier)
                    .Filter(t => t.BillReportID == id && t.SupplierID == user.SupplierID)
                    .FindEntryAsync();
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
    }
}