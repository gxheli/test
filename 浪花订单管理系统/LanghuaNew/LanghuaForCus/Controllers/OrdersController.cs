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
using LanghuaForCus.Models;
using Newtonsoft.Json;
using Commond;
using System.Security.Cryptography;
using System.Text;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using System.IO;
using WebGrease.Css.Extensions;

namespace LanghuaForCus.Controllers
{
    public class OrdersController : BaseController
    {
        private ODataClient client = new ODataClient(ConfigurationManager.AppSettings["ServicePath"] + "odata/");
        //订单首页
        // GET: Orders
        public async Task<ActionResult> Index()
        {
            string CustomerTBCode = User.Identity.Name;
            Customer customer = await client.For<Customer>()
                .Filter(u => u.CustomerTBCode == CustomerTBCode).FindEntryAsync();
            //待处理订单，显示未填写、已填写、已核对、已拒绝
            var orders = await client.For<Order>()
                .Expand(t => t.ServiceItemHistorys)
                .Filter(t => t.CustomerID == customer.CustomerID)
                .Filter(t => t.state != OrderState.Invalid)
                .Filter(t => t.CustomerState == OrderCustomerState.Notfilled || t.CustomerState == OrderCustomerState.Filled || t.CustomerState == OrderCustomerState.Check || t.CustomerState == OrderCustomerState.SencondFull)
                .OrderBy(t => t.CustomerState)
                .FindEntriesAsync();
            DateTimeOffset now = DateTimeOffset.Parse(DateTime.Now.ToString("yyyy-MM-dd"));
            //我的行程，只显示出行日期大于今天的预定中、变更中、取消中、已确认
            var ReserveOrders = await client.For<Order>()
                .Expand(t => t.ServiceItemHistorys)
                .Filter(t => t.CustomerID == customer.CustomerID)
                .Filter(t => t.state != OrderState.Invalid)
                .Filter(t => t.ServiceItemHistorys.TravelDate >= now)
                .Filter(t => t.CustomerState == OrderCustomerState.Ordering || t.CustomerState == OrderCustomerState.Changeing || t.CustomerState == OrderCustomerState.Canceling || t.CustomerState == OrderCustomerState.SencondConfirm)
                .OrderBy(t => t.ServiceItemHistorys.TravelDate)
                .FindEntriesAsync();
            ViewBag.ReserveOrders = ReserveOrders;
            return View(orders);
        }
        //获取全部订单
        // GET: Orders
        public ActionResult allOrder()
        {
            //待处理：未填写、待核对、已核对、已拒绝
            //处理中：预定中、变更中、取消中
            //已确认：已确认
            //已取消：已取消
            Dictionary<string, string> status = new Dictionary<string, string>();
            status.Add("-1", "全部");
            status.Add("0,10,20,30", "待处理");
            status.Add("40,50,60", "处理中");
            status.Add("70", "已确认");
            status.Add("80", "已取消");
            ViewBag.status = status;

            return View();
        }
        //订单详情，通过加密的ID，允许不登录访问（主要用于分享）
        // GET: Orders/Details/5
        [AllowAnonymous]
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            int OrderID;
            try
            {
                string strOrderID = EncryptHelper.Decrypt(id);
                OrderID = int.Parse(strOrderID);
            }
            catch(Exception ex)
            {
                return HttpNotFound();
            }
            //string CustomerTBCode = User.Identity.Name;
            //Customer customer = await client.For<Customer>()
            //    .Filter(u => u.CustomerTBCode == CustomerTBCode).FindEntryAsync();
            
            Order order = await client.For<Order>()
                .Expand(u => u.ServiceItemHistorys)
                .Expand(u => u.ServiceItemHistorys.ItemTemplte)
                .Expand(u => u.ServiceItemHistorys.ExtraServiceHistorys)
                .Expand(u => u.Customers)
                .Expand(u => u.TBOrders)
                .Filter(t => t.OrderID == OrderID && t.state != OrderState.Invalid)
                .FindEntryAsync();
            if (order == null)
            {
                return HttpNotFound();
            }
            string html = string.Empty;
            try
            {
                html = order.ServiceItemHistorys.ItemTemplte == null ? string.Empty : order.ServiceItemHistorys.ItemTemplte.ServiceItemTemplteHtml;

                //附加项目
                string enExName = "";
                string cnExName = "";
                if (order.ServiceItemHistorys.ExtraServiceHistorys != null)
                {
                    int exnum = 0;
                    foreach (var ex in order.ServiceItemHistorys.ExtraServiceHistorys)
                    {
                        if (ex.ServiceNum > 0)
                        {
                            if (exnum > 0)
                            {
                                enExName += ",";
                                cnExName += ",";
                            }
                            enExName += ex.ServiceEnName + " " + ex.ServiceNum + ex.ServiceUnit;
                            cnExName += ex.ServiceName + "：" + ex.ServiceNum + ex.ServiceUnit;
                            exnum++;
                        }
                    }
                }
                //替换系统字段
                html = html.Replace("#TripCompany#", order.ServiceItemHistorys.TravelCompany);
                html = html.Replace("#VoucherNo#", order.ServiceItemHistorys.GroupNo);
                html = html.Replace("#PickupTime#", order.ServiceItemHistorys.ReceiveManTime);
                html = html.Replace("#cnName#", order.CustomerName);
                html = html.Replace("#enName#", order.CustomerEnname);
                html = html.Replace("#Tel#", order.Tel);
                html = html.Replace("#BakTel#", order.BakTel);
                html = html.Replace("#Email#", order.Email);
                html = html.Replace("#Wechat#", order.Wechat);
                html = html.Replace("#TBID#", order.Customers.CustomerTBCode);
                html = html.Replace("#cnProductName#", order.ServiceItemHistorys.cnItemName);
                html = html.Replace("#enProductName#", order.ServiceItemHistorys.enItemName);
                html = html.Replace("#ProductCode#", order.ServiceItemHistorys.ServiceCode);
                Supplier supplier = await client.For<Supplier>().Key(order.ServiceItemHistorys.SupplierID).FindEntryAsync();
                html = html.Replace("#SupplyTel#", supplier.ContactWay);
                html = html.Replace("#SupplierName#", supplier.SupplierName);
                html = html.Replace("#SupplierEnName#", supplier.SupplierEnName);

                html = html.Replace("#ServiceDate#", order.ServiceItemHistorys.TravelDate < DateTime.Parse("1901-01-01") ? "" : order.ServiceItemHistorys.TravelDate.ToString("yyyy-MM-dd"));
                html = html.Replace("#BackDate#", order.ServiceItemHistorys.ReturnDate < DateTime.Parse("1901-01-01") ? "" : order.ServiceItemHistorys.ReturnDate.ToString("yyyy-MM-dd"));
                html = html.Replace("#Adult#", order.ServiceItemHistorys.AdultNum.ToString());
                html = html.Replace("#Child#", order.ServiceItemHistorys.ChildNum.ToString());
                html = html.Replace("#Infant#", order.ServiceItemHistorys.INFNum.ToString());
                html = html.Replace("#Nights#", order.ServiceItemHistorys.RightNum.ToString());
                html = html.Replace("#NoOfRoom#", order.ServiceItemHistorys.RoomNum.ToString());
                html = html.Replace("#Remark#", order.Remark);

                html = html.Replace("#enAttachedItem#", enExName);
                html = html.Replace("#cnAttachedItem#", cnExName);
                html = html.Replace("#LHOrderNo#", order.OrderNo);
                html = html.Replace("#TBOrderNo#", order.TBNum);

                string json = order.ServiceItemHistorys.ServiceItemTemplteValue;
                //遍历模板值进行替换
                if (!string.IsNullOrEmpty(json))
                {
                    JObject jo = JObject.Parse(json);
                    foreach (var item in jo)
                    {
                        var type = item.Value.SelectToken("type");
                        if (type != null && type.ToString() == "PersonPicker")
                        {
                            var list = item.Value.SelectToken("value");
                            string PersonStr = "";//拼音（护照号）
                            string PersonStr_M1 = "";
                            string PersonStr_M2 = "";
                            string PersonStr_M3 = "";
                            string PersonStr_D1 = "";
                            string PersonStr_D2 = "";
                            string PersonStr_D3 = "";
                            string PersonStr_D4 = "";
                            string PersonStr_D5 = "";
                            foreach (var one in list)
                            {
                                PersonStr += one.SelectToken("TravellerEnname").ToString() + "(" + one.SelectToken("PassportNo").ToString() + "),";
                                PersonStr_M1 += one.SelectToken("TravellerEnname").ToString() + "(" + one.SelectToken("PassportNo").ToString() + "," + DateTime.Parse(one.SelectToken("Birthday").ToString()).ToString("yyyy-MM-dd") + "),";
                                PersonStr_M2 += one.SelectToken("TravellerName").ToString() + "(" + one.SelectToken("TravellerEnname").ToString() + "," + one.SelectToken("PassportNo").ToString() + "," + DateTime.Parse(one.SelectToken("Birthday").ToString()).ToString("yyyy-MM-dd") + "),";
                                PersonStr_M3 += one.SelectToken("TravellerEnname").ToString() + "(" + one.SelectToken("PassportNo").ToString() + "," + DateTime.Parse(one.SelectToken("Birthday").ToString()).ToString("yyyy-MM-dd") + "," + (one.SelectToken("TravellerSex").ToString() == "0" ? "M" : "F") + "),";
                                PersonStr_D1 += one.SelectToken("TravellerEnname").ToString() + "," + one.SelectToken("Height").ToString() + "cm," + one.SelectToken("Weight").ToString() + "kg," + one.SelectToken("ShoesSize").ToString() + "<br/>";
                                PersonStr_D2 += one.SelectToken("TravellerEnname").ToString() + "," + one.SelectToken("Height").ToString() + "cm," + one.SelectToken("Weight").ToString() + "kg," + one.SelectToken("ShoesSize").ToString() + "," + (one.SelectToken("ClothesSize").ToString() == "" ? "N/A" : one.SelectToken("ClothesSize").ToString()) + "<br/>";
                                PersonStr_D3 += one.SelectToken("TravellerEnname").ToString() + "," + one.SelectToken("Height").ToString() + "cm," + one.SelectToken("Weight").ToString() + "kg," + one.SelectToken("ShoesSize").ToString() + "," + (one.SelectToken("ClothesSize").ToString() == "" ? "N/A" : one.SelectToken("ClothesSize").ToString()) + "," + (one.SelectToken("TravellerSex").ToString() == "0" ? "M" : "F") + "<br/>";
                                PersonStr_D4 += one.SelectToken("TravellerEnname").ToString() + "," + one.SelectToken("Height").ToString() + "cm," + one.SelectToken("Weight").ToString() + "kg," + one.SelectToken("ShoesSize").ToString() + "," + (one.SelectToken("ClothesSize").ToString() == "" ? "N/A" : one.SelectToken("ClothesSize").ToString()) + "," + (one.SelectToken("GlassesNum").ToString() == "" ? "N/A" : one.SelectToken("GlassesNum").ToString()) + "<br/>";
                                PersonStr_D5 += one.SelectToken("TravellerEnname").ToString() + "," + one.SelectToken("Height").ToString() + "cm," + one.SelectToken("Weight").ToString() + "kg," + one.SelectToken("ShoesSize").ToString() + "," + (one.SelectToken("ClothesSize").ToString() == "" ? "N/A" : one.SelectToken("ClothesSize").ToString()) + "," + (one.SelectToken("GlassesNum").ToString() == "" ? "N/A" : one.SelectToken("GlassesNum").ToString()) + "," + (one.SelectToken("TravellerSex").ToString() == "0" ? "M" : "F") + "<br/>";
                            }
                            html = html.Replace("#" + item.Key + "_D5#", PersonStr_D5);
                            html = html.Replace("#" + item.Key + "_D4#", PersonStr_D4);
                            html = html.Replace("#" + item.Key + "_D3#", PersonStr_D3);
                            html = html.Replace("#" + item.Key + "_D2#", PersonStr_D2);
                            html = html.Replace("#" + item.Key + "_D1#", PersonStr_D1);
                            html = html.Replace("#" + item.Key + "_M3#", PersonStr_M3);
                            html = html.Replace("#" + item.Key + "_M2#", PersonStr_M2);
                            html = html.Replace("#" + item.Key + "_M1#", PersonStr_M1);
                            html = html.Replace("#" + item.Key + "#", PersonStr);
                        }
                        else
                        {
                            html = html.Replace("#" + item.Key + "#", item.Value.ToString());
                        }
                    }
                }

                ViewBag.html = html.Trim().Replace("﻿", "");//去掉UTF8的标识
                ViewBag.SupplierEnName = supplier.SupplierEnName;
            }
            catch (IOException e)
            {
                //html = e.Message;
            }
            return View(order);
        }
        //发送邮件
        [HttpPost]
        public async Task<string> SendMail(int OrderID, string title, string toMail, string customerName, string fileName, string filePath, string type = "customer")
        {
            Order order;
            try
            {
                order = await client.For<Order>().Key(OrderID).FindEntryAsync();
            }
            catch
            {
                return JsonConvert.SerializeObject(new { result = false, statusCode = 401, message = "找不到该订单！", info = "" });
            }
            if (order == null)
            {
                return JsonConvert.SerializeObject(new { result = false, statusCode = 401, message = "找不到订单！", info = "" });
            }
            if (string.IsNullOrEmpty(toMail))
            {
                return JsonConvert.SerializeObject(new { result = false, statusCode = 401, message = "收件人不能为空！", info = "" });
            }
            if (string.IsNullOrEmpty(title))
            {
                return JsonConvert.SerializeObject(new { result = false, statusCode = 401, message = "标题不能为空！", info = "" });
            }
            if (string.IsNullOrEmpty(customerName))
            {
                return JsonConvert.SerializeObject(new { result = false, statusCode = 401, message = "客户名称不能为空！", info = "" });
            }
            if (string.IsNullOrEmpty(fileName))
            {
                return JsonConvert.SerializeObject(new { result = false, statusCode = 401, message = "附件名称不能为空！", info = "" });
            }
            if (string.IsNullOrEmpty(filePath) || !System.IO.File.Exists(filePath))
            {
                return JsonConvert.SerializeObject(new { result = false, statusCode = 401, message = "发送失败！找不到附件", info = "" });
            }
            string template = "";
            if (type == "supplier")
            {
                template = "bookingmail";
            }
            else if (type == "customer")
            {
                template = "confirmmail";
            }
            else
            {
                return JsonConvert.SerializeObject(new { result = false, statusCode = 401, message = "发送失败！类型错误", info = "" });
            }
            //发送邮件
            string result = await EmailHelper.send(title, toMail, order.CustomerName, fileName, filePath, template);
            string message = "";
            try
            {
                JObject jo = JObject.Parse(result);
                message = jo.SelectToken("$..statusCode").ToString() == "200" ? "成功" : "失败";
            }
            catch
            {
                message = "失败";
            }
            string CustomerTBCode = User.Identity.Name;
            Customer customer = await client.For<Customer>()
                .Expand(u => u.Travellers)
                .Filter(u => u.CustomerTBCode == CustomerTBCode).FindEntryAsync();

            await client.For<OrderHistory>().Set(new OrderHistory
            {
                OrderID = order.OrderID,
                OperUserID = customer.CustomerID.ToString(),
                OperUserNickName = "(客户)" + customer.CustomerTBCode,
                OperTime = DateTime.Now,
                State = order.state,
                Remark = "(客户)发送邮件" + message + ",Email:" + toMail
            }).InsertEntryAsync();

            //删除附件
            System.IO.File.Delete(filePath);
            return result;
        }
        //发送邮件前先保存图片，然后作为附件发送
        [HttpPost]
        public string UploadPic(string imageData, string imageName)
        {
            string savePath = System.Web.HttpContext.Current.Server.MapPath("~/App_Data/");
            if (!Directory.Exists(savePath))//判断上传文件夹是否存在，若不存在，则创建
            {
                Directory.CreateDirectory(savePath);//创建文件夹
            }
            string Pic_Path = System.Web.HttpContext.Current.Server.MapPath("~/App_Data/" + imageName);
            using (FileStream fs = new FileStream(Pic_Path, FileMode.Create))
            {
                using (BinaryWriter bw = new BinaryWriter(fs))
                {
                    byte[] data = Convert.FromBase64String(imageData);
                    bw.Write(data);
                    bw.Close();
                }
            }
            return JsonConvert.SerializeObject(new { ErrorCode = 200, ErrorMessage = "OK", Pic_Path });
        }
        //先保存图片，然后下载
        [HttpPost]
        public FileResult DownFile(string fileData, string fileName)
        {
            string folderPath = "~/data/OrderData/" + DateTime.Now.ToString("yyyyMM") + "/";
            string filePhysicalPath = Server.MapPath(folderPath);
            if (!Directory.Exists(filePhysicalPath))//判断上传文件夹是否存在，若不存在，则创建
            {
                Directory.CreateDirectory(filePhysicalPath);//创建文件夹
            }
            string filePath = System.Web.HttpContext.Current.Server.MapPath(folderPath + fileName);
            using (FileStream fs = new FileStream(filePath, FileMode.Create))
            {
                using (BinaryWriter bw = new BinaryWriter(fs))
                {
                    byte[] data = Convert.FromBase64String(fileData);
                    bw.Write(data);
                    bw.Close();
                }
            }
            return File(new FileStream(filePath, FileMode.Open), "application/octet-stream", Server.UrlEncode(fileName));
        }
        //订单修改
        // GET: Orders/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            string CustomerTBCode = User.Identity.Name;
            Customer customer = await client.For<Customer>()
                .Filter(u => u.CustomerTBCode == CustomerTBCode).FindEntryAsync();

            Order order = await client.For<Order>()
                .Expand(t => t.ServiceItemHistorys)
                .Expand(u => u.ServiceItemHistorys.ExtraServiceHistorys)
                .Expand(t => t.Customers)
                .Filter(t => t.OrderID == id && t.CustomerID == customer.CustomerID && t.state != OrderState.Invalid)
                .FindEntryAsync();
            if (order == null)
            {
                return HttpNotFound();
            }
            //如果订单里面的联系人为空，则取客户基本信息里面的信息
            if (string.IsNullOrEmpty(order.CustomerName) && string.IsNullOrEmpty(order.CustomerEnname) && string.IsNullOrEmpty(order.Tel) && string.IsNullOrEmpty(order.BakTel) && string.IsNullOrEmpty(order.Email) && string.IsNullOrEmpty(order.Wechat))
            {
                order.CustomerName = order.Customers.CustomerName;
                order.CustomerEnname = order.Customers.CustomerEnname;
                order.Tel = order.Customers.Tel;
                order.BakTel = order.Customers.BakTel;
                order.Email = order.Customers.Email;
                order.Wechat = order.Customers.Wechat;
            }
            return View(order);
        }
        //获取订单列表
        public async Task<string> GetAllOrders(SearchModel search)
        {
            string CustomerTBCode = User.Identity.Name;
            Customer customer = await client.For<Customer>()
                .Expand(u => u.Travellers)
                .Filter(u => u.CustomerTBCode == CustomerTBCode).FindEntryAsync();

            int draw = 1;
            int start = 0;
            int length = 10;
            string propertyName = "CustomerState";
            int sort = 1;
            if (search.length > 0)
            {
                draw = search.draw;
                start = search.start;
                length = search.length;
                if (search.OrderBy != null)
                {
                    propertyName = search.OrderBy.PropertyName == null ? propertyName : search.OrderBy.PropertyName;
                    sort = search.OrderBy.OrderBy;
                }
            }
            var OrderResult = client.For<Order>()
                        .Expand(t => t.ServiceItemHistorys)
                        .Filter("CustomerID eq " + customer.CustomerID)
                        .Filter("state ne LanghuaNew.Data.OrderState'Invalid'")
                        .Skip(start).Top(length);
            var OrderCountResult = client.For<Order>().Filter("CustomerID eq " + customer.CustomerID);

            if (propertyName == "TravelDate")
            {
                OrderResult = sort == 0 ? OrderResult.OrderByDescending("ServiceItemHistorys/TravelDate") : OrderResult.OrderBy("ServiceItemHistorys/TravelDate");
            }
            else
            {
                OrderResult = sort == 0 ? OrderResult.OrderByDescending(propertyName) : OrderResult.OrderBy(propertyName);
            }

            if (search.OrderSearch != null)
            {
                string status = search.OrderSearch.status;
                if (!string.IsNullOrEmpty(status))
                {//状态，以逗号分开
                    string[] state = status.Trim().Split(',');

                    string filter = "";
                    foreach (var item in state)
                    {
                        if (item == "-1")
                        {
                            break;
                        }
                        if (filter != "")
                        {
                            filter += " or ";
                        }
                        filter += "CustomerState eq LanghuaNew.Data.OrderCustomerState'" + item + "'";
                    }
                    if (filter != "")
                    {
                        OrderResult = OrderResult.Filter(filter);
                        OrderCountResult = OrderCountResult.Filter(filter);
                    }
                }
                string NoShowCancel = search.OrderSearch.NoShowCancel;
                if (NoShowCancel == "true")
                {
                    string filter = "CustomerState ne LanghuaNew.Data.OrderCustomerState'SencondCancel'";
                    OrderResult = OrderResult.Filter(filter);
                    OrderCountResult = OrderCountResult.Filter(filter);
                }
            }
            int count = await OrderCountResult.Count().FindScalarAsync<int>();
            var orders = await OrderResult.FindEntriesAsync();
            var data = orders.Select(t => new
            {
                t.OrderID,
                EncodeOrderID = EncryptHelper.Encrypt(t.OrderID.ToString()),
                t.OrderNo,
                t.ServiceItemHistorys.cnItemName,
                t.ServiceItemHistorys.AdultNum,
                t.ServiceItemHistorys.ChildNum,
                t.ServiceItemHistorys.INFNum,
                RightNum = t.ServiceItemHistorys.ServiceTypeID == 4 ? t.ServiceItemHistorys.RightNum : 0,
                t.ServiceItemHistorys.RoomNum,
                TravelDate = t.ServiceItemHistorys.TravelDate > DateTimeOffset.Parse("1900-01-02") ? t.ServiceItemHistorys.TravelDate.ToString("yyyy-MM-dd") : "",
                t.CustomerState,
                stateName = EnumHelper.GetEnumDescription(t.CustomerState),
            });
            return JsonConvert.SerializeObject(new { draw = draw, recordsFiltered = count, data = data, SearchModel = search });

        }
        //检查出行日期，前端在提交订单前请求
        [HttpPost]
        public async Task<string> CheckTravelDate(int OrderID, string TravelDate)
        {
            List<DateTimeOffset> date = new List<DateTimeOffset>();
            try
            {
                date.Add(DateTimeOffset.Parse(TravelDate));
            }
            catch
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "日期格式错误" });
            }
            var order = await client.For<Order>().Key(OrderID).Select(s => s.CustomerID).FindEntryAsync();
            var result = client.For<Order>().Expand(s => s.ServiceItemHistorys)
                .Filter(s => s.CustomerID == order.CustomerID)
                .Filter(s => s.OrderID != OrderID)
                .Filter(s => s.state != OrderState.Invalid && s.state != OrderState.SencondCancel);
            var orders = await result.Select(s => new { s.ServiceItemHistorys.TravelDate, s.ServiceItemHistorys.ChangeTravelDate }).FindEntriesAsync();
            orders.ForEach(s => date.Add(s.ServiceItemHistorys.ChangeTravelDate > DateTimeOffset.MinValue ? s.ServiceItemHistorys.ChangeTravelDate : s.ServiceItemHistorys.TravelDate));
            var valid = date.Where(s => s > DateTimeOffset.Now.AddMonths(-1));
            if (valid != null && valid.Count() > 0 && (valid.Max() - valid.Min()).Days > 15)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "您在系统中的最早出行日期 <span style='color:red'>" + valid.Min().ToString("yyyy-MM-dd") + "</span> 与最晚出行日期 <span style='color:red'>" + valid.Max().ToString("yyyy-MM-dd") + "</span> 相隔超过15天，请检查是否有误！确认无误需要保存请点【确认】，否则请点【取消】。" });
            }
            return JsonConvert.SerializeObject(new { ErrorCode = 200, ErrorMessage = "OK" });
        }
        //保存订单，isCommit为false时代表暂存
        [HttpPost]
        public async Task<string> SaveOrder(Order order, bool isCommit)
        {
            if (order.OrderID == 0)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "找不到数据！" });
            }
            if (order.Customers == null && isCommit) //暂存时联系人信息可以部分提交
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "联系人信息不能为空！" });
            }
            string CustomerTBCode = User.Identity.Name;
            Customer customer = await client.For<Customer>()
                .Expand(u => u.Travellers)
                .Filter(u => u.CustomerTBCode == CustomerTBCode).FindEntryAsync();

            Order item = await client.For<Order>()
                .Expand(o => o.ServiceItemHistorys).Expand(o => o.Customers)
                .Filter(u => u.OrderID == order.OrderID & u.CustomerID == customer.CustomerID)
                .FindEntryAsync();
            if (item == null)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "找不到订单！" });
            }

            item.CustomerName = order.Customers.CustomerName;
            item.CustomerEnname = order.Customers.CustomerEnname;
            item.Tel = order.Customers.Tel;
            item.BakTel = order.Customers.BakTel;
            item.Email = order.Customers.Email;
            item.Wechat = order.Customers.Wechat;
            try
            {
                if (!isCommit && item.state != OrderState.Notfilled && item.state != OrderState.Invalid)
                {
                    return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "已填报的订单不能暂存！" });
                }
                if (isCommit)//正式提交时才验证以下内容
                {
                    if (item.state == OrderState.Invalid)
                    {
                        return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "本单已作废！请返回首页查看最新订单或联系客服！" });
                    }
                    if (item.state != OrderState.Notfilled && item.state != OrderState.Filled)
                    {
                        return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "核对后不能修改！请联系客服！" });
                    }
                    if (string.IsNullOrEmpty(item.CustomerName) || string.IsNullOrEmpty(item.CustomerEnname))
                    {
                        return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "姓名不能为空！" });
                    }
                    if (string.IsNullOrEmpty(item.Email))
                    {
                        return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "邮箱不能为空！" });
                    }
                    if (string.IsNullOrEmpty(item.Tel))
                    {
                        return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "联系电话不能为空！" });
                    }
                    if (string.IsNullOrEmpty(item.Wechat))
                    {
                        return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "微信号不能为空！" });
                    }
                    if (item.ServiceItemHistorys.ServiceTypeID == 2)//行程
                    {
                        Order ssorder = await client.For<Order>()
                        .Filter(s => s.ServiceItemHistorys.ServiceTypeID == 2)//行程
                        .Filter(s => s.CustomerID == item.CustomerID)
                        .Filter(s => s.OrderID != order.OrderID)
                        .Filter(s => s.state != OrderState.Invalid && s.state != OrderState.SencondCancel && s.state != OrderState.CancelReceive && s.state != OrderState.RequestCancel && s.state != OrderState.Cancel && s.state != OrderState.Notfilled)
                        //.Filter(s => s.CustomerName == item.CustomerName || s.Tel == item.Tel)
                        .Filter(s => s.ServiceItemHistorys.TravelDate == order.ServiceItemHistorys.TravelDate || s.ServiceItemHistorys.ChangeTravelDate == order.ServiceItemHistorys.TravelDate)
                        .Select(s => new { s.OrderID, s.CustomerName, s.Tel }).FindEntryAsync();
                        if (ssorder != null)
                        {
                            //名字相同时，提示相同名字同一天不能去2个地方玩
                            if (ssorder.CustomerName == item.CustomerName)
                            {
                                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "系统检测到 " + item.CustomerName + " 在 " + order.ServiceItemHistorys.TravelDate.ToString("yyyy-MM-dd") + " 有其它行程安排！请修改出行日期或预订人姓名、电话号码后再保存，也可先" + (item.state == OrderState.Notfilled ? "暂存信息后" : "") + "咨询客服。" });
                            }
                            //电话号码相同时，提示相同电话号码同一天不能去2个地方玩
                            if (ssorder.Tel == item.Tel)
                            {
                                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "系统检测到 " + item.Tel + "  这个号码出现在 " + order.ServiceItemHistorys.TravelDate.ToString("yyyy-MM-dd") + " 的其它行程中！请修改出行日期或更换电话号码后再保存，也可先" + (item.state == OrderState.Notfilled ? "暂存信息后" : "") + "咨询客服。" });
                            }
                        }
                        Order ccorder = await client.For<Order>()
                            .Filter(s => s.ServiceItemHistorys.ServiceTypeID == 2)//行程
                            .Filter(s => s.ServiceItemHistorys.ServiceItemID == order.ServiceItemHistorys.ServiceItemID)//同一行程
                            .Filter(s => s.ServiceItemHistorys.SupplierID == order.ServiceItemHistorys.SupplierID)
                            .Filter(s => s.state == OrderState.SencondCancel || s.state == OrderState.CancelReceive || s.state == OrderState.RequestCancel || s.state == OrderState.Cancel)
                            .Filter(s => s.CustomerID == order.CustomerID)
                            .Filter(s => s.ServiceItemHistorys.TravelDate == order.ServiceItemHistorys.TravelDate || s.ServiceItemHistorys.ChangeTravelDate == order.ServiceItemHistorys.TravelDate)
                            .Filter(s => s.CustomerName == item.CustomerName)
                            .Select(s => s.OrderID).FindEntryAsync();
                        //名字相同时，同一个行程曾经被取消，则提示
                        if (ccorder != null)
                        {
                            return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "系统检测到 " + item.CustomerName + " 在 " + order.ServiceItemHistorys.TravelDate.ToString("yyyy-MM-dd") + " 的同一行程曾经被取消！请修改姓名后再保存，也可先" + (item.state == OrderState.Notfilled ? "暂存信息后" : "") + "咨询客服。" });
                        }
                    }
                }
                //保存订单信息
                DateTimeOffset TravelDate = order.ServiceItemHistorys.TravelDate;
                DateTimeOffset ReturnDate = order.ServiceItemHistorys.ReturnDate;

                ServiceItemHistory itemHistory = item.ServiceItemHistorys;
                if (itemHistory.FixedDays > 0)//行程天数大于0时，返回日期等于出行日期加上（行程天数-1）
                {
                    ReturnDate = TravelDate.AddDays(itemHistory.FixedDays - 1);
                }
                if (itemHistory.ServiceTypeID == 2 && isCommit)//行程
                {
                    if (ReturnDate < TravelDate)
                    {
                        return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "行程类项目返回日期不能为空且不能早于出行日期！" });
                    }
                }
                if (itemHistory.ServiceTypeID == 4 && isCommit)//酒店类检查入住日期、退房日期和晚数
                {
                    if ((ReturnDate.Date - TravelDate.Date).TotalDays != itemHistory.RightNum)
                    {
                        return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "入住" + itemHistory.RightNum + "晚，您选择的日期有误！" });
                    }
                }
                if (await CheckRule(itemHistory.ServiceItemID, TravelDate) && isCommit)//检查出行日期规则
                {
                    return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "出行日期不在规则允许范围内！" });
                }
                itemHistory.ElementsValue = order.ServiceItemHistorys.ElementsValue;
                itemHistory.ServiceItemTemplteValue = order.ServiceItemHistorys.ServiceItemTemplteValue;
                itemHistory.TravelDate = TravelDate;
                itemHistory.ReturnDate = ReturnDate;
                itemHistory.travellers = new List<OrderTraveller>();
                itemHistory.travellers = order.ServiceItemHistorys.travellers == null ? null : order.ServiceItemHistorys.travellers.Distinct().ToList();

                await HttpHelper.PutAction("ServiceItemHistoryExtend", JsonConvert.SerializeObject(itemHistory));

                item.state = isCommit ? OrderState.Filled : OrderState.Notfilled;
                item.CustomerState = OrderStateHelper.GetOrderCustomerState(item.state, null);
                await client.For<Order>().Key(order.OrderID).Set(item).UpdateEntryAsync();
                await client.For<OrderHistory>().Set(new OrderHistory
                {
                    OrderID = order.OrderID,
                    OperUserID = customer.CustomerID.ToString(),
                    OperUserNickName = "(客户)" + customer.CustomerTBCode,
                    OperTime = DateTime.Now,
                    State = isCommit ? OrderState.Filled : OrderState.Notfilled,
                    Remark = isCommit ? "填写订单" : "暂存订单"
                }).InsertEntryAsync();
            }
            catch (Exception ex)
            {
                await client.For<SystemLog>().Set(new SystemLog
                {
                    Operate = "客人" + (isCommit ? "填写订单" : "暂存订单") + "失败",
                    OperateTime = DateTime.Now,
                    UserID = customer.CustomerID,
                    UserName = customer.CustomerTBCode,
                    Remark = "OrderID：" + order.OrderID + "异常：" + ex.Message + "填报内容：" + JsonConvert.SerializeObject(order)
                }).InsertEntryAsync();
                return JsonConvert.SerializeObject(new { ErrorCode = 400, ErrorMessage = "修改失败！失败原因：" + ex.Message });
            }
            try
            {
                //如果客户基本信息为空，则保存客户信息
                Customer oldCustomer = item.Customers;
                if (string.IsNullOrEmpty(oldCustomer.CustomerName) && isCommit)
                {
                    oldCustomer.CustomerName = order.Customers.CustomerName;
                    oldCustomer.CustomerEnname = order.Customers.CustomerEnname.ToUpper();
                    oldCustomer.Tel = order.Customers.Tel;
                    oldCustomer.BakTel = order.Customers.BakTel;
                    oldCustomer.Email = order.Customers.Email;
                    oldCustomer.Wechat = order.Customers.Wechat;
                    oldCustomer.Password = string.IsNullOrEmpty(oldCustomer.Password) ? Md5Hash(order.Customers.Tel) : oldCustomer.Password;

                    await client.For<Customer>().Key(oldCustomer.CustomerID).Set(oldCustomer).UpdateEntryAsync();
                }
            }
            catch (Exception ex)
            {
                await client.For<SystemLog>().Set(new SystemLog
                {
                    Operate = "客人填写订单时保存基本信息失败",
                    OperateTime = DateTime.Now,
                    UserID = customer.CustomerID,
                    UserName = customer.CustomerTBCode,
                    Remark = "OrderID：" + order.OrderID + "异常：" + ex.Message + "填报内容：" + JsonConvert.SerializeObject(order)
                }).InsertEntryAsync();
            }
            return JsonConvert.SerializeObject(new { ErrorCode = 200, ErrorMessage = "OK" });
        }
        //加密
        private static string Md5Hash(string input)
        {
            MD5CryptoServiceProvider md5Hasher = new MD5CryptoServiceProvider();
            byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(input));
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }
        /// <summary>
        /// 检查出行日期是否被规则禁止，被禁止则返回ture
        /// </summary>
        /// <param name="ItemID"></param>
        /// <param name="dt"></param>
        /// <returns></returns>
        public async Task<bool> CheckRule(int ItemID, DateTimeOffset dt)
        {
            var rules = await client.For<ServiceRule>().Filter(s => s.RuleServiceItem.Any(r => r.ServiceItemID == ItemID) && s.UseState == EnableState.Enable).FindEntriesAsync();
            if (rules == null)
            {
                return false;
            }
            if (rules.Count() == 0)
            {
                return false;
            }
            var thisweek = dt.DayOfWeek.ToString("d");
            foreach (var rule in rules)
            {
                if (rule.StartTime.Date <= dt.Date && dt.Date <= rule.EndTime.Date)
                {
                    bool isAllow = rule.RuleUseTypeValue == RuleUseType.Allow ? true : false;
                    switch (rule.SelectRuleType)
                    {
                        case RuleType.ByDateRange:
                            if ((rule.RangeStart.Date <= dt.Date && dt.Date <= rule.RangeEnd.Date && !isAllow) || ((dt.Date < rule.RangeStart || dt.Date > rule.RangeEnd) && isAllow))
                            {
                                return true;
                            }
                            break;
                        case RuleType.ByWeek:
                            if ((rule.Week.Split('|').Contains(thisweek) && !isAllow) || (!rule.Week.Split('|').Contains(thisweek) && isAllow))
                            {
                                return true;
                            }
                            break;
                        case RuleType.ByDouble:
                            if ((((dt.Day % 2 == 0 && rule.IsDouble) || (dt.Day % 2 != 0 && !rule.IsDouble)) && !isAllow) || (((dt.Day % 2 != 0 && rule.IsDouble) || (dt.Day % 2 == 0 && !rule.IsDouble)) && isAllow))
                            {
                                return true;
                            }
                            break;
                        case RuleType.ByDate:
                            if ((rule.UseDate.Split('|').Contains(dt.Day.ToString()) && !isAllow) || (!rule.UseDate.Split('|').Contains(dt.Day.ToString()) && isAllow))
                            {
                                return true;
                            }
                            break;
                    }
                }
            }
            return false;
        }
        //订单完成页
        public ActionResult Finished()
        {
            return View();
        }
        //老系统订单
        public async Task<ActionResult> OldOrders()
        {
            string TBID = User.Identity.Name;
            var Message = await HttpHelper.GetAction("OldOrders?TBID=" + TBID);
            string result = Message.Content.ReadAsStringAsync().Result;
            DataSet ds = JsonConvert.DeserializeObject<DataSet>(result);
            DataTable dt = ds == null ? null : ds.Tables[0];
            ViewBag.dt = dt;
            return View();
        }

    }
}
