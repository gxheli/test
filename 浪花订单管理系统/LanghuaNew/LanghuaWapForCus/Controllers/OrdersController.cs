using Commond;
using LanghuaNew.Data;
using LanghuaWapForCus.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Simple.OData.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebGrease.Css.Extensions;

namespace LanghuaWapForCus.Controllers
{
    public class OrdersController : Controller
    {
        private ODataClient client = new ODataClient(ConfigurationManager.AppSettings["ServicePath"] + "odata/");
        //获取待处理订单
        [HttpPost]
        public async Task<string> GetPendingOrders()
        {
            string UserName = User.Identity.Name;
            Customer customer = await client.For<Customer>().Filter(u => u.CustomerTBCode == UserName).FindEntryAsync();
            if (customer == null)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "登录异常，请重新登录" });
            }
            var PendingOrders = await client.For<Order>()
                .Expand(t => t.ServiceItemHistorys)
                .Filter(t => t.CustomerID == customer.CustomerID)
                .Filter(t => t.state != OrderState.Invalid)
                .Filter(t => t.CustomerState == OrderCustomerState.Notfilled || t.CustomerState == OrderCustomerState.Filled || t.CustomerState == OrderCustomerState.Check || t.CustomerState == OrderCustomerState.SencondFull)
                .OrderBy(t => t.CustomerState)
                .Select(t => new
                {
                    t.OrderID,
                    t.OrderNo,
                    t.ServiceItemHistorys.cnItemName,
                    t.ServiceItemHistorys.ServiceTypeID,
                    t.ServiceItemHistorys.AdultNum,
                    t.ServiceItemHistorys.ChildNum,
                    t.ServiceItemHistorys.INFNum,
                    t.ServiceItemHistorys.RightNum,
                    t.ServiceItemHistorys.RoomNum,
                    t.ServiceItemHistorys.TravelDate,
                    t.CustomerState,
                }).FindEntriesAsync();
            var Orders = PendingOrders.Select(t => new
            {
                OrderID = EncryptHelper.Encrypt(t.OrderID.ToString()),
                t.OrderNo,
                t.ServiceItemHistorys.cnItemName,
                t.ServiceItemHistorys.AdultNum,
                t.ServiceItemHistorys.ChildNum,
                t.ServiceItemHistorys.INFNum,
                RightNum = t.ServiceItemHistorys.ServiceTypeID == 4 ? t.ServiceItemHistorys.RightNum : 0,
                RoomNum = t.ServiceItemHistorys.ServiceTypeID == 4 ? t.ServiceItemHistorys.RoomNum : 0,
                TravelDate = t.ServiceItemHistorys.TravelDate > DateTimeOffset.Parse("1900-01-02") ? t.ServiceItemHistorys.TravelDate.ToString("yyyy-MM-dd") : "",
                t.CustomerState,
                stateName = EnumHelper.GetEnumDescription(t.CustomerState),
            });
            return JsonConvert.SerializeObject(new { ErrorCode = 200, ErrorMessage = "OK", Orders = Orders });
        }
        //获取待出行订单
        [HttpPost]
        public async Task<string> GetTripOrders()
        {
            string UserName = User.Identity.Name;
            Customer customer = await client.For<Customer>().Filter(u => u.CustomerTBCode == UserName).FindEntryAsync();
            if (customer == null)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "登录异常，请重新登录" });
            }
            DateTimeOffset now = DateTimeOffset.Parse(DateTime.Now.ToString("yyyy-MM-dd"));
            var TripOrders = await client.For<Order>()
                .Expand(t => t.ServiceItemHistorys)
                .Filter(t => t.CustomerID == customer.CustomerID)
                .Filter(t => t.state != OrderState.Invalid)
                .Filter(t => t.ServiceItemHistorys.TravelDate >= now)
                .Filter(t => t.CustomerState == OrderCustomerState.Ordering || t.CustomerState == OrderCustomerState.Changeing || t.CustomerState == OrderCustomerState.Canceling || t.CustomerState == OrderCustomerState.SencondConfirm)
                .OrderBy(t => t.ServiceItemHistorys.TravelDate)
                .Select(t => new
                {
                    t.OrderID,
                    t.OrderNo,
                    t.ServiceItemHistorys.cnItemName,
                    t.ServiceItemHistorys.ServiceTypeID,
                    t.ServiceItemHistorys.AdultNum,
                    t.ServiceItemHistorys.ChildNum,
                    t.ServiceItemHistorys.INFNum,
                    t.ServiceItemHistorys.RightNum,
                    t.ServiceItemHistorys.RoomNum,
                    t.ServiceItemHistorys.TravelDate,
                    t.CustomerState,
                }).FindEntriesAsync();
            var Orders = TripOrders.Select(t => new
            {
                OrderID = EncryptHelper.Encrypt(t.OrderID.ToString()),
                t.OrderNo,
                t.ServiceItemHistorys.cnItemName,
                t.ServiceItemHistorys.AdultNum,
                t.ServiceItemHistorys.ChildNum,
                t.ServiceItemHistorys.INFNum,
                RightNum = t.ServiceItemHistorys.ServiceTypeID == 4 ? t.ServiceItemHistorys.RightNum : 0,
                RoomNum = t.ServiceItemHistorys.ServiceTypeID == 4 ? t.ServiceItemHistorys.RoomNum : 0,
                TravelDate = t.ServiceItemHistorys.TravelDate > DateTimeOffset.Parse("1900-01-02") ? t.ServiceItemHistorys.TravelDate.ToString("yyyy-MM-dd") : "",
                t.CustomerState,
                stateName = EnumHelper.GetEnumDescription(t.CustomerState),
            });
            return JsonConvert.SerializeObject(new { ErrorCode = 200, ErrorMessage = "OK", Orders = Orders });
        }
        //获取全部订单
        [HttpPost]
        public async Task<string> GetAllOrders(SearchModel search)
        {
            string UserName = User.Identity.Name;
            Customer customer = await client.For<Customer>().Filter(u => u.CustomerTBCode == UserName).FindEntryAsync();
            if (customer == null)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "登录异常，请重新登录" });
            }
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
                        .Skip(start).Top(length)
                        .Select(t => new
                        {
                            t.OrderID,
                            t.OrderNo,
                            t.ServiceItemHistorys.cnItemName,
                            t.ServiceItemHistorys.ServiceTypeID,
                            t.ServiceItemHistorys.AdultNum,
                            t.ServiceItemHistorys.ChildNum,
                            t.ServiceItemHistorys.INFNum,
                            t.ServiceItemHistorys.RightNum,
                            t.ServiceItemHistorys.RoomNum,
                            t.ServiceItemHistorys.TravelDate,
                            t.CustomerState,
                        });
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
            var AllOrders = await OrderResult.FindEntriesAsync();
            var Orders = AllOrders.Select(t => new
            {
                OrderID = EncryptHelper.Encrypt(t.OrderID.ToString()),
                t.OrderNo,
                t.ServiceItemHistorys.cnItemName,
                t.ServiceItemHistorys.AdultNum,
                t.ServiceItemHistorys.ChildNum,
                t.ServiceItemHistorys.INFNum,
                RightNum = t.ServiceItemHistorys.ServiceTypeID == 4 ? t.ServiceItemHistorys.RightNum : 0,
                RoomNum = t.ServiceItemHistorys.ServiceTypeID == 4 ? t.ServiceItemHistorys.RoomNum : 0,
                TravelDate = t.ServiceItemHistorys.TravelDate > DateTimeOffset.Parse("1900-01-02") ? t.ServiceItemHistorys.TravelDate.ToString("yyyy-MM-dd") : "",
                t.CustomerState,
                stateName = EnumHelper.GetEnumDescription(t.CustomerState),
            });
            return JsonConvert.SerializeObject(new { draw = draw, recordsFiltered = count, Orders = Orders, SearchModel = search });

        }
        //获取订单查看详情
        [HttpPost]
        [AllowAnonymous]
        public async Task<string> GetOrderDetail(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "id不能为空" });
            }
            int OrderID;
            try
            {
                string strOrderID = EncryptHelper.Decrypt(id);
                OrderID = int.Parse(strOrderID);
            }
            catch
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "参数有误" });
            }
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
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "订单不存在" });
            }
            Supplier supplier = await client.For<Supplier>().Key(order.ServiceItemHistorys.SupplierID).FindEntryAsync();
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
                html = html.Trim().Replace("﻿", "");//去掉UTF8的标识
            }
            catch { }
            var OrderDetail = new
            {
                Html = html,
                SupplierEnName = supplier.SupplierEnName,
                order.OrderID,
                order.TBOrders.OrderSourseID,
                order.CustomerName,
                order.Email,
                order.Customers.CustomerTBCode,
                order.ServiceItemHistorys.ServiceCode,
                order.ServiceItemHistorys.cnItemName,
                order.ServiceItemHistorys.SupplierCode,
                order.ServiceItemHistorys.ServiceTypeID,
            };
            return JsonConvert.SerializeObject(new { ErrorCode = 200, ErrorMessage = "OK", OrderDetail = OrderDetail });
        }
        //获取订单修改详情
        [HttpPost]
        public async Task<string> GetOrderEdit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "id不能为空" });
            }
            int OrderID;
            try
            {
                string strOrderID = EncryptHelper.Decrypt(id);
                OrderID = int.Parse(strOrderID);
            }
            catch
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "参数有误" });
            }
            string UserName = User.Identity.Name;
            Customer customer = await client.For<Customer>().Filter(u => u.CustomerTBCode == UserName).FindEntryAsync();
            if (customer == null)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "登录异常，请重新登录" });
            }
            Order order = await client.For<Order>()
                .Expand(t => t.ServiceItemHistorys)
                .Expand(u => u.ServiceItemHistorys.ExtraServiceHistorys)
                .Expand(t => t.Customers)
                .Filter(t => t.OrderID == OrderID && t.CustomerID == customer.CustomerID && t.state != OrderState.Invalid)
                .FindEntryAsync();
            if (order == null)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "订单不存在" });
            }
            if (string.IsNullOrEmpty(order.CustomerName) && string.IsNullOrEmpty(order.CustomerEnname) && string.IsNullOrEmpty(order.Tel) && string.IsNullOrEmpty(order.BakTel) && string.IsNullOrEmpty(order.Email) && string.IsNullOrEmpty(order.Wechat))
            {
                order.CustomerName = order.Customers.CustomerName;
                order.CustomerEnname = order.Customers.CustomerEnname;
                order.Tel = order.Customers.Tel;
                order.BakTel = order.Customers.BakTel;
                order.Email = order.Customers.Email;
                order.Wechat = order.Customers.Wechat;
            }
            var OrderDetail = new
            {
                severTimeStamp = Convert.ToInt64((DateTime.Now.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalSeconds * 1000).ToString(),
                order.OrderID,
                order.CustomerName,
                order.Email,
                order.Tel,
                order.CustomerEnname,
                order.CustomerID,
                order.BakTel,
                order.Wechat,
                order.Customers.CustomerTBCode,
                order.state,
                RQUrl = string.IsNullOrEmpty(customer.OpenID) ? WeiXinHelper.GetImageUrlByID(customer.CustomerID, systemType.costomer) : "",
                ServiceItemHistorys = new
                {
                    order.ServiceItemHistorys.ServiceItemID,
                    order.ServiceItemHistorys.ServiceCode,
                    order.ServiceItemHistorys.cnItemName,
                    order.ServiceItemHistorys.SupplierCode,
                    order.ServiceItemHistorys.ServiceTypeID,
                    order.ServiceItemHistorys.Elements,
                    order.ServiceItemHistorys.ElementsValue,
                    order.ServiceItemHistorys.AdultNum,
                    order.ServiceItemHistorys.ChildNum,
                    order.ServiceItemHistorys.INFNum,
                    order.ServiceItemHistorys.RightNum,
                    order.ServiceItemHistorys.RoomNum,
                    ExtraServiceHistorys = order.ServiceItemHistorys.ExtraServiceHistorys
                },
            };
            return JsonConvert.SerializeObject(new { ErrorCode = 200, ErrorMessage = "OK", OrderDetail = OrderDetail });
        }
        //填写订单
        [HttpPost]
        public async Task<string> SaveOrder(Order order, bool isCommit)
        {
            if (order.OrderID == 0)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "订单ID不能为空！" });
            }
            if (order.Customers == null && isCommit)
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
                if (isCommit)
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
                    if (item.ServiceItemHistorys.ServiceTypeID == 2)
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
                            if (ssorder.CustomerName == item.CustomerName)
                            {
                                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "系统检测到 " + item.CustomerName + " 在 " + order.ServiceItemHistorys.TravelDate.ToString("yyyy-MM-dd") + " 有其它行程安排！请修改出行日期或预订人姓名、电话号码后再保存，也可先" + (item.state == OrderState.Notfilled ? "暂存信息后" : "") + "咨询客服。" });
                            }
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
                if (itemHistory.FixedDays > 0)
                {
                    ReturnDate = TravelDate.AddDays(itemHistory.FixedDays - 1);
                }
                if (itemHistory.ServiceTypeID == 2 && isCommit)
                {
                    if (ReturnDate < TravelDate)
                    {
                        return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "行程类项目返回日期不能为空且不能早于出行日期！" });
                    }
                }
                if (itemHistory.ServiceTypeID == 4 && isCommit)
                {
                    if ((ReturnDate.Date - TravelDate.Date).TotalDays != itemHistory.RightNum)
                    {
                        return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "入住" + itemHistory.RightNum + "晚，您选择的日期有误！" });
                    }
                }
                if (await CheckRule(itemHistory.ServiceItemID, TravelDate) && isCommit)
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
                //保存客户信息
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
        //检查出行日期
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

        //根据产品获取出行规则列表
        [HttpPost]
        public async Task<string> GetRulesByItemID(int id)
        {
            var Rules = await client.For<ServiceRule>()
                .Filter(t => t.RuleServiceItem.Any(r => r.ServiceItemID == id) && t.UseState == EnableState.Enable)
                .FindEntriesAsync();
            return JsonConvert.SerializeObject(new { ErrorCode = 200, ErrorMessage = "OK", Rules = Rules });
        }
        //获取航班信息
        [HttpPost]
        public async Task<string> GetFliterInfo(string FlightNo)
        {
            try
            {
                var result = await client.For<FliterInfo>().Filter(t => t.FliterNum == FlightNo).FindEntryAsync();
                return JsonConvert.SerializeObject(new { ErrorCode = 200, ErrorMessage = "OK", FliterInfo = result });
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 400, ErrorMessage = ex.Message, FliterInfo = string.Empty });
            }
        }
        //获取酒店信息
        [HttpPost]
        public async Task<string> GetHotals(int CityID, string Str)
        {
            try
            {
                IEnumerable<Hotal> Hotals = await client.For<Hotal>()
                    .Expand(u => u.HotalArea)
                    .Filter(u => u.HotalArea.CityID == CityID)
                    .Filter(u => u.HotalName.Contains(Str))
                    .Top(15)
                    .FindEntriesAsync();
                return JsonConvert.SerializeObject(new { ErrorCode = 200, ErrorMessage = "OK", Hotals });
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 400, ErrorMessage = ex, Hotals = "[]" });
            }
        }
        //获取区域信息
        [HttpPost]
        public async Task<string> GetAreas(int CityID)
        {
            try
            {
                IEnumerable<Area> Areas = await client.For<Area>()
                    .Filter(u => u.CityID == CityID)
                    .Filter(u => u.AreaEnableState == EnableState.Enable)
                    .FindEntriesAsync();
                return JsonConvert.SerializeObject(new { ErrorCode = 200, ErrorMessage = "OK", Areas });
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 400, ErrorMessage = ex, Areas = "[]" });
            }
        }

        //检查出行日期是否被规则禁止，被禁止则返回ture
        private async Task<bool> CheckRule(int ItemID, DateTimeOffset dt)
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
    }
}