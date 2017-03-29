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
using System.Net.Http;
using Commond;
using Entity;
using System.IO;
using System.Data.OleDb;
using System.Activities.Statements;
using System.Text.RegularExpressions;
using WebGrease.Css.Extensions;

namespace LanghuaNew.Controllers
{
    public class TBOrderStatesController : Controller
    {
        private ODataClient client = new ODataClient(ConfigurationManager.AppSettings["ServicePath"] + "odata/");

        // GET: TBOrderStates
        public async Task<ActionResult> Index()
        {
            ViewBag.OrderSourse = await client.For<OrderSourse>()
                .Filter(t => t.OrderSourseEnableState == EnableState.Enable)
                .OrderBy(t => t.ShowNo)
                .FindEntriesAsync();
            return View();
        }
        //获取发货列表
        public async Task<string> GetTBOrderStates(ShareSearchModel search)
        {
            HttpResponseMessage Message = await HttpHelper.PostAction("TBOrderStatesExtend", JsonConvert.SerializeObject(search));
            string exMessage = Message.Content.ReadAsStringAsync().Result;
            return exMessage;
        }
        [HttpPost]
        public async Task<string> UpdateDisable(List<TBOrderStateModel> TBID, string Operation)
        {
            if (TBID == null)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "请至少选中一项！" });
            }
            if (string.IsNullOrEmpty(Operation))
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "操作异常！" });
            }
            try
            {
                if (Operation.Trim() == "Delivery")
                {
                    foreach (var item in TBID)
                    {
                        var oldItem = await client.For<TBOrderState>()
                            .Filter(t => t.OrderSourseID == item.OrderSourseID && t.SendCustomer.CustomerTBCode == item.CustomerTBCode)
                            .Filter(t => !t.IsSend)
                            .FindEntriesAsync();
                        if (oldItem != null)
                        {
                            foreach (var old in oldItem)
                            {
                                string userName = User.Identity.Name;
                                User user = await client.For<User>().Filter(u => u.UserName == userName).FindEntryAsync();
                                old.IsSend = true;
                                old.SendTime = DateTimeOffset.Now;
                                old.SendUserID = user.UserID;
                                old.SendUserName = user.NickName;
                                await client.For<TBOrderState>().Key(old.TBOrderStateID).Set(old).UpdateEntryAsync();
                            }
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
            return JsonConvert.SerializeObject(new { ErrorCode = 200, ErrorMessage = "OK" });
        }

        public ActionResult CheckDelivery()
        {
            return View();
        }
        //上传订单
        [HttpPost]
        public async Task<ActionResult> CheckDelivery(HttpPostedFileBase fileBase)
        {
            HttpPostedFileBase file = Request.Files["files"];
            string FileName;
            string savePath;
            if (file == null || file.ContentLength <= 0)
            {
                ViewBag.error = "文件不能为空";
                return View();
            }
            else
            {
                string filename = Path.GetFileName(file.FileName);
                int filesize = file.ContentLength;//获取上传文件的大小单位为字节byte
                string fileEx = System.IO.Path.GetExtension(filename);//获取上传文件的扩展名
                string NoFileName = System.IO.Path.GetFileNameWithoutExtension(filename);//获取无扩展名的文件名
                int Maxsize = 4000 * 1024;//定义上传文件的最大空间大小为4M
                string FileType = ".xls,.xlsx,.csv";//定义上传文件的类型字符串

                FileName = NoFileName + DateTime.Now.ToString("yyyyMMddhhmmss") + fileEx;
                if (!FileType.Contains(fileEx))
                {
                    ViewBag.error = "文件类型不对，只能导入xls\\xlsx\\csv格式的文件";
                    return View();
                }
                if (filesize >= Maxsize)
                {
                    ViewBag.error = "上传文件超过4M，不能上传";
                    return View();
                }
                string path = AppDomain.CurrentDomain.BaseDirectory + "data/excel/";
                if (!Directory.Exists(path))//判断上传文件夹是否存在，若不存在，则创建
                {
                    Directory.CreateDirectory(path);//创建文件夹
                }
                savePath = Path.Combine(path, FileName);
                file.SaveAs(savePath);
            }
            try
            {
                ExcelHelper excel = new ExcelHelper(savePath);
                DataTable dt = excel.ExcelToDataTable("Sheet1", true);
                string num = Guid.NewGuid().ToString();

                List<ExportOrder> list = new List<ExportOrder>();
                if (dt != null && dt.Rows.Count > 0)
                {
                    DateTimeOffset now = DateTimeOffset.Now;
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        //订单号 格式可能为 ="123" 或者 "123"
                        string OrderNo = dt.Rows[i][0].ToString().Replace("=\"", "").Replace("\"", "");
                        //文本类型需要去除双引号
                        string TBID = dt.Rows[i][1].ToString().Replace("\"", "");
                        if (TBID.Trim().Length > 0)
                        {
                            ExportOrder ex = new ExportOrder();
                            ex.CreateTime = now;
                            ex.Guid = num;
                            ex.ExportOrderNo = OrderNo;
                            ex.ExportTBID = TBID;
                            list.Add(ex);
                        }
                    }
                    HttpResponseMessage message = await HttpHelper.BigDataPostAction("ExportOrdersExtend", JsonConvert.SerializeObject(list));
                    List<CheckDeliveryModel> data = JsonConvert.DeserializeObject<List<CheckDeliveryModel>>(message.Content.ReadAsStringAsync().Result);
                    ViewBag.dt = data;
                    ViewBag.success = "恭喜你，导入的订单已全部发货";
                }
                else
                {
                    ViewBag.success = "对不起，数据无法识别或者您导入的订单为空";
                }
            }
            catch (Exception ex)
            {
                ViewBag.success = "对不起，导入订单失败" + ex.Message;
            }
            if (System.IO.File.Exists(savePath))
            {
                System.IO.File.Delete(savePath);
            }
            return View();
        }
    }
}
