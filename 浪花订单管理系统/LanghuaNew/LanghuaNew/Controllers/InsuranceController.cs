using Commond;
using Entity;
using LanghuaNew.Data;
using LanghuaNew.Models;
using Newtonsoft.Json;
using Simple.OData.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebGrease.Css.Extensions;
using System.Web.Http;
using System.IO;
using System.Net.Http.Headers;
using System.Net;
using System.Text.RegularExpressions;
using NPOI.HSSF.Util;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;

namespace LanghuaNew.Controllers
{
    public class InsuranceController : Controller
    {
        private ODataClient client = new ODataClient(ConfigurationManager.AppSettings["ServicePath"] + "odata/");

        // GET: Insurance
        public async Task<ActionResult> Index()
        {
            //string userName = User.Identity.Name;
            //User user = await client.For<User>().Expand(u => u.UserRole).Expand("UserRole/Rights").Filter(u => u.UserName == userName).FindEntryAsync();
            var InsuranceDayList = await client.For<ServiceItemHistory>().Filter(a=>a.InsuranceDays>0 && a.Order.state==OrderState.SencondConfirm).Select(d=>d.InsuranceDays).OrderBy(a=>a.InsuranceDays).FindEntriesAsync();
            var idlG=InsuranceDayList.GroupBy(b => b.InsuranceDays);
            List<int> idl = new List<int>();
            foreach (var a in idlG)
            {
                idl.Add(a.Key);
            }
            ViewBag.InsuranceDayList = idl;

            ViewBag.Countries = await client.For<Country>()
               .FindEntriesAsync();
            //var list = await client.For<ServiceItemHistory>().Expand(c => c.Order).Expand(c => c.travellers).FindEntriesAsync();
            //return View(list);
            return View();
        }

        public async Task<string> GetInsurances(InsuranceSearchModel insuranceSearch)
        {
            InsuranceSearch search;
            if (insuranceSearch.Search == null)
            {
                search = new InsuranceSearch();
                DateTimeOffset defaultTime = new DateTimeOffset(DateTime.Now.AddDays(1));
                search.Insurance_Start_Date = defaultTime.Date;
                search.ServiceItemID = -1;
                search.ServiceCode = "-1";
                search.cnItemName = "全部";
                search.Insurance_Days = 1;
                search.CountryID = -1;
                search.CountryName = "国家";
                search.InsuranceCode = DateTime.Now.AddDays(1).ToString("yyyyMMdd") + "-1-全部";
            }
            else
            {
                search = insuranceSearch.Search;
                if(search.ServiceItemID==0)
                {
                    search.ServiceItemID = -1;
                    search.ServiceCode = "-1";
                    search.cnItemName = "全部";
                }
                if (search.CountryID == 0)
                {
                    search.CountryID = -1;
                    search.CountryName = "国家";
                }
                if(string.IsNullOrEmpty(search.InsuranceCode))
                {
                    search.InsuranceCode = DateTime.Now.AddDays(1).ToString("yyyyMMdd") + "-" + search.Insurance_Days + "-全部";
                }
            }
            var Result = client.For<ServiceItemHistory>().Expand(c => c.Order.Customers).Expand(c => c.travellers).Expand(c => c.orderSupplier).Expand(c => c.orderSupplier.SupplierCountry);
            //var Result = client.For<ServiceItemHistory>().Select(d=>new { d.ServiceItemID,d.TravelDate,d.InsuranceDays,
            //    d.orderSupplier.CountryID, d.orderSupplier.SupplierCountry.CountryName,
            //    d.travellers});
            //DateTimeOffset UTCNow = new DateTimeOffset(search.Search.Insurance_Start_Date);
            //保险开始日期
            if (search.Insurance_Start_Date != null)
            {
                Result.Filter(t => t.TravelDate.Date == search.Insurance_Start_Date.Date);
            }
            //保险天数    
            if (search.Insurance_Days > 0)
            {
                Result.Filter(t => t.InsuranceDays == search.Insurance_Days);
            }
            else
            {
                Result.Filter(t => t.InsuranceDays == 1);
            }
            //国家
            if (search.CountryID != -1)
            {
                Result.Filter(t => t.orderSupplier.SupplierCountry.CountryID == search.CountryID);
            }
            //产品
            if (search.ServiceItemID != -1 && search.ServiceItemID!=0 && search.cnItemName != "全部" && search.cnItemName != "")
            {
                Result.Filter(t => t.ServiceItemID == search.ServiceItemID);
            }
            Result.Filter(t => t.Order.state== OrderState.SencondConfirm);
            int draw = 1;
            int start = 0;
            int length = 50;
            if (insuranceSearch.length > 0)
            {
                draw = insuranceSearch.draw;
                start = insuranceSearch.start;
                length = insuranceSearch.length;
            }

            
            InsuranceListModel list = new InsuranceListModel();
            list.data = new List<InsuranceModel>();
            //构造列表数据
            var ds = await Result.OrderBy(t=>t.TravelDate).FindEntriesAsync();

            if (ds!=null)
            {
                foreach(ServiceItemHistory t in ds)
                {
                    if(t.travellers!=null)
                    {
                        foreach (OrderTraveller tr in t.travellers)
                        {
                            InsuranceModel model = new InsuranceModel();
                            model.CustomerTBCode = t.Order.Customers.CustomerTBCode;
                            model.ServiceItemID = t.ServiceItemID;
                            model.cnItemName = t.cnItemName;
                            model.InsuranceStartDate = t.TravelDate;
                            model.Insurance_Days = t.InsuranceDays;
                            model.CountryID = t.orderSupplier.CountryID;
                            model.CountryName = t.orderSupplier.SupplierCountry.CountryName;
                        
                            model.TravellerName = tr.TravellerName;
                            model.TravellerSex = (int)tr.TravellerSex;
                            model.Birthday = tr.Birthday;
                            model.PassportNo = tr.PassportNo;
                            if (list.data.Where(c => c.CustomerTBCode == model.CustomerTBCode 
                             && c.PassportNo == model.PassportNo 
                             && c.TravellerName == model.TravellerName).Count() == 0)
                            {
                                list.data.Add(model);
                            }
                        }
                    }
                }
            }
            int count = list.data.Count;
            list.data = list.data.Skip(start).Take(length).ToList();
            return JsonConvert.SerializeObject(new InsuranceListModel { draw = draw, recordsFiltered = count, data = list.data, Search = insuranceSearch });
        }

        //列表导出
        public async Task<FileResult> ExportExcel(InsuranceSearch insuranceSearch)
        {
            InsuranceSearch search;
            if (insuranceSearch.Insurance_Days==0)
            {
                search = new InsuranceSearch();
                DateTimeOffset defaultTime = new DateTimeOffset(DateTime.Now.AddDays(1));
                search.Insurance_Start_Date = defaultTime.Date;
                search.ServiceItemID = -1;
                search.ServiceCode = "-1";
                search.cnItemName = "全部";
                search.Insurance_Days = 1;
                search.CountryID = -1;
                search.CountryName = "国家";
                search.InsuranceCode = DateTime.Now.AddDays(1).ToString("yyyyMMdd") + "-1-全部";
            }
            else
            {
                search = insuranceSearch;
                if (search.ServiceItemID == 0)
                {
                    search.ServiceItemID = -1;
                    search.ServiceCode = "-1";
                    search.cnItemName = "全部";
                }
                if (search.CountryID == 0)
                {
                    search.CountryID = -1;
                    search.CountryName = "国家";
                }
                if ((search.CountryID == 0 || search.CountryID==-1) && (search.ServiceItemID == 0 || search.ServiceItemID==-1) && search.Insurance_Start_Date > default(DateTimeOffset))
                {
                     search.InsuranceCode = search.Insurance_Start_Date.ToString("yyyyMMdd") + "-" + search.Insurance_Days + "-全部";
                }
                else
                {
                    if ((search.ServiceItemID == 0 || search.ServiceItemID == -1) && search.Insurance_Start_Date > default(DateTimeOffset))
                    {
                        search.InsuranceCode = search.Insurance_Start_Date.ToString("yyyyMMdd") + "-" + search.Insurance_Days + "-" + search.CountryName;
                    }
                    else
                    {
                        search.InsuranceCode = search.Insurance_Start_Date.ToString("yyyyMMdd") + "-" + search.Insurance_Days +"-" + search.ServiceCode;
                    }
                }
            }
            var Result = client.For<ServiceItemHistory>().Expand(c => c.Order.Customers).Expand(c => c.travellers).Expand(c => c.orderSupplier).Expand(c => c.orderSupplier.SupplierCountry);
            //保险开始日期
            if (search.Insurance_Start_Date != null)
            {
                Result.Filter(t => t.TravelDate.Date == search.Insurance_Start_Date.Date);
            }
            //保险天数    
            if (search.Insurance_Days > 0)
            {
                Result.Filter(t => t.InsuranceDays == search.Insurance_Days);
            }
            else
            {
                Result.Filter(t => t.InsuranceDays == 1);
            }
            //国家
            if (search.CountryID != -1)
            {
                Result.Filter(t => t.orderSupplier.SupplierCountry.CountryID == search.CountryID);
            }
            //产品
            if (search.ServiceItemID != -1 && search.ServiceItemID != 0 && search.cnItemName != "全部" && search.cnItemName != "")
            {
                Result.Filter(t => t.ServiceCode == search.ServiceCode);
            }
            Result.Filter(t => t.Order.state == OrderState.SencondConfirm);
            List<InsuranceModel> list = new List<InsuranceModel>();

            //构造列表数据
            var ds = await Result.OrderBy(t => t.TravelDate).FindEntriesAsync();
            if (ds != null)
            {
                foreach (ServiceItemHistory t in ds)
                {
                    if (t.travellers != null)
                    {

                        foreach (OrderTraveller tr in t.travellers)
                        {
                            InsuranceModel model = new InsuranceModel();

                            model.CustomerTBCode = t.Order.Customers.CustomerTBCode;
                            model.ServiceItemID = t.ServiceItemID;
                            model.cnItemName = t.cnItemName;
                            model.InsuranceStartDate = t.TravelDate;
                            model.Insurance_Days = t.InsuranceDays;
                            model.CountryID = t.orderSupplier.CountryID;
                            model.CountryName = t.orderSupplier.SupplierCountry.CountryName;

                            model.TravellerSex = (int)tr.TravellerSex;
                            model.TravellerName = tr.TravellerName;
                            model.Birthday = tr.Birthday;
                            model.PassportNo = tr.PassportNo;
                            if (list.Where(c => c.CustomerTBCode == model.CustomerTBCode 
                             && c.PassportNo == model.PassportNo 
                             && c.TravellerName == model.TravellerName).Count() == 0)
                            {
                                if (list.Where(d => d.TravellerName == tr.TravellerName && d.PassportNo == tr.PassportNo).Count() > 0)
                                {
                                    var item =list.Where(d => d.TravellerName == tr.TravellerName && d.PassportNo == tr.PassportNo).FirstOrDefault();
                                    list.Remove(item);
                                    list.Add(model);
                                }
                                else
                                {
                                    list.Add(model);
                                }
                            }
                        }
                    }
                }
            }

            //创建Excel文件的对象
            NPOI.HSSF.UserModel.HSSFWorkbook book = new NPOI.HSSF.UserModel.HSSFWorkbook();
            //添加一个sheet
            NPOI.SS.UserModel.ISheet sheet1 = book.CreateSheet("Sheet1");
            //第一列列宽
            sheet1.SetColumnWidth(0, 8 * 256);
            //第二列列宽
            sheet1.SetColumnWidth(1, 12 * 256);
            //第三列列宽
            sheet1.SetColumnWidth(2, 22 * 256);
            //第四列列宽
            sheet1.SetColumnWidth(3, 12 * 256);
            //第五列列宽
            sheet1.SetColumnWidth(4, 6 * 256);
            //第六列列宽
            sheet1.SetColumnWidth(5, 12 * 256);
            //第六列列宽
            sheet1.SetColumnWidth(6, 12 * 256);
            //标题样式
            ICellStyle titleStyle = book.CreateCellStyle();
            //设置单元格的样式：水平对齐居中
            titleStyle.Alignment = HorizontalAlignment.Center;
            titleStyle.VerticalAlignment = VerticalAlignment.Center;
            //新建一个字体样式对象
            IFont font = book.CreateFont();
            //设置字体加粗样式
            font.Boldweight = short.MaxValue;
            //使用SetFont方法将字体样式添加到单元格样式中 
            titleStyle.SetFont(font);

            //表头样式
            ICellStyle headerStyle = book.CreateCellStyle();
            IFont titleFont = book.CreateFont();
            //设置字体加粗样式
            titleFont.Boldweight = short.MaxValue;
            headerStyle.SetFont(titleFont);

            //特殊内容样式
            ICellStyle style = book.CreateCellStyle();
            style.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.Orange.Index;
            style.FillPattern = FillPattern.SolidForeground;

            NPOI.SS.UserModel.IRow headerRow = sheet1.CreateRow(0);
        
            headerRow.CreateCell(0).SetCellValue("被保险人清单");
            headerRow.GetCell(0).CellStyle = titleStyle;
            headerRow.Height = 40 * 20;
            //合并单元格
            sheet1.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(0, 0, 0, 6));

            int i = 1;
            //给sheet1添加第一行的头部
            NPOI.SS.UserModel.IRow row1 = sheet1.CreateRow(i);

            //row1.CreateCell(0).SetCellValue("淘宝ID");
            row1.CreateCell(0).CellStyle = headerStyle;
            row1.GetCell(0).SetCellValue("姓名");
            row1.CreateCell(1).CellStyle = headerStyle;
            row1.GetCell(1).SetCellValue("证件类型");
            row1.CreateCell(2).CellStyle = headerStyle;
            row1.GetCell(2).SetCellValue("证件号码");
            row1.CreateCell(3).CellStyle = headerStyle;
            row1.GetCell(3).SetCellValue("出生日期");
            row1.CreateCell(4).CellStyle = headerStyle;
            row1.GetCell(4).SetCellValue("性别");
            row1.CreateCell(5).CellStyle = headerStyle;
            row1.GetCell(5).SetCellValue("联系电话");
            row1.CreateCell(6).CellStyle = headerStyle;
            row1.GetCell(6).SetCellValue("国籍代码");

            row1.Height = 450;

            //将数据逐步写入sheet1各个行
            foreach (var item in list)
            {
                i++;
                NPOI.SS.UserModel.IRow rowtemp = sheet1.CreateRow(i);
                
                //rowtemp.CreateCell(0).SetCellValue(item.CustomerTBCode == null ? "" : item.CustomerTBCode);
                rowtemp.CreateCell(0).SetCellValue(item.cnItemName == null ? "" : item.TravellerName);
                string t = GetIDCardType(item.PassportNo);
                if (t != "中国护照")
                {
                    rowtemp.CreateCell(1).CellStyle = style;
                    rowtemp.Cells[1].SetCellValue(t);
                }
                else
                {
                    rowtemp.CreateCell(1).SetCellValue(t);
                }
                rowtemp.CreateCell(2).SetCellValue(item.PassportNo == null ? "" : item.PassportNo);
                rowtemp.CreateCell(3).SetCellValue(item.Birthday.ToString("yyyy-MM-dd"));
                rowtemp.CreateCell(4).SetCellValue(item.TravellerSex == 0 ? "男" : "女");
                rowtemp.CreateCell(5).SetCellValue("");
                rowtemp.CreateCell(6).SetCellValue("CHN");
            }

            // 写入到客户端 
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            book.Write(ms);
            ms.Seek(0, SeekOrigin.Begin);

            return File(ms, "application/vnd.ms-excel", search.InsuranceCode + ".xls");
        }

        public string GetIDCardType(string str)
        {
            string result = "其他证件";
            if (!string.IsNullOrEmpty(str))
            {
                if ((Regex.IsMatch(str, @"^(^\d{15}$|^\d{18}$|^\d{17}(\d|X|x))$", RegexOptions.IgnoreCase)))
                {
                    result = "居民身份证";
                }
                if ((Regex.IsMatch(str, @"^((^[a-zA-Z]\d{8}$))", RegexOptions.IgnoreCase)))
                {
                    result = "中国护照";
                }
            }
            return result;
        }
    }
}
