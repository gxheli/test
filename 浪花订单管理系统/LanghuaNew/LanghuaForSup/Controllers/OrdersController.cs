using Commond;
using Entity;
using LanghuaNew.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Simple.OData.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Pechkin;
using Aspose.Words;
using System.Text;
using System.Text.RegularExpressions;

namespace LanghuaForSup.Controllers
{
    public class OrdersController : Controller
    {
        private ODataClient client = new ODataClient(ConfigurationManager.AppSettings["ServicePath"] + "odata/");
        // GET: Orders
        public async Task<ActionResult> Index(string search)
        {
            //合并状态
            Dictionary<string, string> stateLeft = new Dictionary<string, string>();
            //全部状态
            Dictionary<string, string> stateRight = new Dictionary<string, string>();
            stateLeft.Add(((int)OrderState.Send).ToString(),
                EnumHelper.GetEnumDescription(OrderState.Send).Substring(EnumHelper.GetEnumDescription(OrderState.Send).IndexOf("|") + 1));
            stateLeft.Add("4,14,15", "已接单");
            stateLeft.Add("9,12", "请求中");
            stateLeft.Add("5,6,7,8,10,11", "已处理");

            stateRight.Add(((int)OrderState.Send).ToString(),
                EnumHelper.GetEnumDescription(OrderState.Send).Substring(EnumHelper.GetEnumDescription(OrderState.Send).IndexOf("|") + 1));
            stateRight.Add(((int)OrderState.SendReceive).ToString(),
                EnumHelper.GetEnumDescription(OrderState.SendReceive).Substring(EnumHelper.GetEnumDescription(OrderState.SendReceive).IndexOf("|") + 1));
            stateRight.Add(((int)OrderState.Confirm).ToString() + "," + ((int)OrderState.SencondConfirm).ToString(),
                EnumHelper.GetEnumDescription(OrderState.Confirm).Substring(EnumHelper.GetEnumDescription(OrderState.Confirm).IndexOf("|") + 1));
            stateRight.Add(((int)OrderState.Cancel).ToString() + "," + ((int)OrderState.SencondCancel).ToString(),
                EnumHelper.GetEnumDescription(OrderState.Cancel).Substring(EnumHelper.GetEnumDescription(OrderState.Cancel).IndexOf("|") + 1));
            stateRight.Add(((int)OrderState.Full).ToString() + "," + ((int)OrderState.SencondFull).ToString(),
                EnumHelper.GetEnumDescription(OrderState.Full).Substring(EnumHelper.GetEnumDescription(OrderState.Full).IndexOf("|") + 1));
            stateRight.Add(((int)OrderState.RequestChange).ToString(),
                EnumHelper.GetEnumDescription(OrderState.RequestChange).Substring(EnumHelper.GetEnumDescription(OrderState.RequestChange).IndexOf("|") + 1));
            stateRight.Add(((int)OrderState.ChangeReceive).ToString(),
                EnumHelper.GetEnumDescription(OrderState.ChangeReceive).Substring(EnumHelper.GetEnumDescription(OrderState.ChangeReceive).IndexOf("|") + 1));
            stateRight.Add(((int)OrderState.RequestCancel).ToString(),
                EnumHelper.GetEnumDescription(OrderState.RequestCancel).Substring(EnumHelper.GetEnumDescription(OrderState.RequestCancel).IndexOf("|") + 1));
            stateRight.Add(((int)OrderState.CancelReceive).ToString(),
                EnumHelper.GetEnumDescription(OrderState.CancelReceive).Substring(EnumHelper.GetEnumDescription(OrderState.CancelReceive).IndexOf("|") + 1));

            ViewBag.stateLift = stateLeft;
            ViewBag.stateRight = stateRight;
            ViewBag.stateAll = stateRight;
            //操作按钮
            Dictionary<int, string> operations = new Dictionary<int, string>();
            operations.Add((int)OrderOperations.Receive, EnumHelper.GetEnumDescription(OrderOperations.Receive));
            operations.Add((int)OrderOperations.Confirm, EnumHelper.GetEnumDescription(OrderOperations.Confirm));
            operations.Add((int)OrderOperations.Cancel, EnumHelper.GetEnumDescription(OrderOperations.Cancel));
            operations.Add((int)OrderOperations.Full, EnumHelper.GetEnumDescription(OrderOperations.Full));
            ViewBag.operations = operations;
            //导出字段
            Dictionary<int, string> exportField = new Dictionary<int, string>();
            exportField.Add((int)ExportField.OrderNo, EnumHelper.GetEnumDescription(ExportField.OrderNo));
            exportField.Add((int)ExportField.cnName, EnumHelper.GetEnumDescription(ExportField.cnName));
            exportField.Add((int)ExportField.enName, EnumHelper.GetEnumDescription(ExportField.enName));
            exportField.Add((int)ExportField.Tel, EnumHelper.GetEnumDescription(ExportField.Tel));
            exportField.Add((int)ExportField.BakTel, EnumHelper.GetEnumDescription(ExportField.BakTel));
            exportField.Add((int)ExportField.cnItemName, EnumHelper.GetEnumDescription(ExportField.cnItemName));
            exportField.Add((int)ExportField.ServiceCode, EnumHelper.GetEnumDescription(ExportField.ServiceCode));
            exportField.Add((int)ExportField.AdultNum, EnumHelper.GetEnumDescription(ExportField.AdultNum));
            exportField.Add((int)ExportField.ChildNum, EnumHelper.GetEnumDescription(ExportField.ChildNum));
            exportField.Add((int)ExportField.INFNum, EnumHelper.GetEnumDescription(ExportField.INFNum));
            exportField.Add((int)ExportField.TravelDate, EnumHelper.GetEnumDescription(ExportField.TravelDate));
            exportField.Add((int)ExportField.OrderState, EnumHelper.GetEnumDescription(ExportField.OrderState));
            exportField.Add((int)ExportField.GroupNo, EnumHelper.GetEnumDescription(ExportField.GroupNo));
            exportField.Add((int)ExportField.Remark, EnumHelper.GetEnumDescription(ExportField.Remark));
            exportField.Add((int)ExportField.ExtraServices, EnumHelper.GetEnumDescription(ExportField.ExtraServices));

            ViewBag.exportField = exportField;
            //产品类型
            var type = await client.For<ServiceType>().FindEntriesAsync();
            ViewBag.type = type;
            ViewBag.search = search;
            return View();
        }
        //订单详情
        // GET: Orders/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }
            Order order = await GetOrderByID(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            string userName = User.Identity.Name;
            SupplierUser OperUser = await client.For<SupplierUser>().Expand(s => s.OneSupplier).Filter(u => u.SupplierUserName == userName).FindEntryAsync();
            if (order.ServiceItemHistorys.SupplierID != OperUser.SupplierID)
            {
                return HttpNotFound();
            }
            //html
            ViewBag.html = await GetConfimOrderHtml(id, order);
            //币别
            ViewBag.Currency = await client.For<Currency>().Filter(c => c.CurrencyEnableState == EnableState.Enable).FindEntriesAsync();
            //操作按钮
            Dictionary<int, string> operations = new Dictionary<int, string>();
            operations.Add((int)OrderOperations.Receive, EnumHelper.GetEnumDescription(OrderOperations.Receive));
            operations.Add((int)OrderOperations.Confirm, EnumHelper.GetEnumDescription(OrderOperations.Confirm));
            operations.Add((int)OrderOperations.Cancel, EnumHelper.GetEnumDescription(OrderOperations.Cancel));
            operations.Add((int)OrderOperations.Full, EnumHelper.GetEnumDescription(OrderOperations.Full));
            ViewBag.operations = operations;
            
            ViewBag.SupplierEnName = OperUser.OneSupplier.SupplierEnName;
            return View(order);
        }

        public async Task<Order> GetOrderByID(int? id)
        {
            Order order = await client.For<Order>()
             .Expand(u => u.ServiceItemHistorys)
             .Expand(u => u.ServiceItemHistorys.ItemTemplte)
             .Expand(u => u.ServiceItemHistorys.ExtraServiceHistorys)
             .Expand(u => u.Customers)
             .Expand(u => u.OrderSupplierHistorys)
             .Expand(u => u.TBOrders)
             .Key(id).FindEntryAsync();
            return order;
        }
        //替换模板返回HTML
        public async Task<string> GetConfimOrderHtml(int? id, Order order)
        {
            string html = "";
            try
            {
                html = order.ServiceItemHistorys.ItemTemplte == null ? "找不到确认单，请联系浪花朵朵工作人员" : order.ServiceItemHistorys.ItemTemplte.ServiceItemTemplteHtml;

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
                //（系统字段）如果有请求变更,就显示变更的值，鼠标移上去可以看到正式的值
                string Changejson = order.ServiceItemHistorys.ChangeElementsValue;
                //供应商处于拒绝待检则只显示变更前的内容
                if (order.state != OrderState.Full)
                {
                    if (!string.IsNullOrEmpty(Changejson))
                    {
                        JObject jo = JObject.Parse(Changejson);
                        foreach (var item in jo)
                        {
                            //变更的值
                            string change = "";
                            //有没有变更的值
                            bool bl = false;
                            switch (item.Key)
                            {
                                case "cnName":
                                    change = order.CustomerName;
                                    bl = true;
                                    break;
                                case "enName":
                                    change = order.CustomerEnname;
                                    bl = true;
                                    break;
                                case "Tel":
                                    change = order.Tel;
                                    bl = true;
                                    break;
                                case "BakTel":
                                    change = order.BakTel;
                                    bl = true;
                                    break;
                                case "Email":
                                    change = order.Email;
                                    bl = true;
                                    break;
                                case "Wechat":
                                    change = order.Wechat;
                                    bl = true;
                                    break;
                                case "Adult":
                                    change = order.ServiceItemHistorys.AdultNum.ToString();
                                    bl = true;
                                    break;
                                case "Child":
                                    change = order.ServiceItemHistorys.ChildNum.ToString();
                                    bl = true;
                                    break;
                                case "Infant":
                                    change = order.ServiceItemHistorys.INFNum.ToString();
                                    bl = true;
                                    break;
                                case "NoOfRoom":
                                    change = order.ServiceItemHistorys.RoomNum.ToString();
                                    bl = true;
                                    break;
                                case "Nights":
                                    change = order.ServiceItemHistorys.RightNum.ToString();
                                    bl = true;
                                    break;
                                case "cnAttachedItem":
                                    change = cnExName;
                                    bl = true;
                                    break;
                                case "enAttachedItem":
                                    change = enExName;
                                    bl = true;
                                    break;
                                case "ServiceDate":
                                    change = order.ServiceItemHistorys.TravelDate < DateTime.Parse("1901-01-01") ? "" : order.ServiceItemHistorys.TravelDate.ToString("yyyy-MM-dd");
                                    bl = true;
                                    break;
                                case "BackDate":
                                    change = order.ServiceItemHistorys.ReturnDate < DateTime.Parse("1901-01-01") ? "" : order.ServiceItemHistorys.ReturnDate.ToString("yyyy-MM-dd");
                                    bl = true;
                                    break;
                            }
                            //如果有变更的值,直接替换模板的对应的值
                            if (bl && change != item.Value.ToString())
                            {
                                //供应商处于确认待检则只显示变更后的内容，否则显示变更后内容的同时鼠标移上去可以看到变更前的值
                                if (order.state == OrderState.Confirm)
                                {
                                    html = html.Replace("#" + item.Key + "#", item.Value.ToString());
                                }
                                else
                                {
                                    html = html.Replace("#" + item.Key + "#", "<span class='orderchange' data-change='" + "原值：" + change + "'>" + item.Value.ToString() + "</span>");
                                }
                            }
                        }
                    }
                }
                //（系统字段）继续替换模板值，如果有变更的字段会在上面的遍历里面先一步替换掉了，剩下的都是没有变更的值
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

                string json = order.ServiceItemHistorys.ServiceItemTemplteValue;
                //（表单字段）遍历模板值进行替换
                if (!string.IsNullOrEmpty(json))
                {
                    JObject jo = JObject.Parse(json);
                    foreach (var item in jo)
                    {
                        var type = item.Value.SelectToken("type");
                        //如果字段的type为PersonPicker，则表示该字段为游客的列表
                        if (type != null && type.ToString() == "PersonPicker")
                        {
                            var list = item.Value.SelectToken("value");
                            string PersonStr = "";//拼音（护照号）
                            string PersonStr_M1 = "";//拼音（护照号，生日）
                            string PersonStr_M2 = "";//姓名（拼音，护照号，生日）
                            string PersonStr_M3 = "";//姓名（拼音，护照号，生日，性别）
                            string PersonStr_D1 = "";//拼音（身高，体重，鞋码）
                            string PersonStr_D2 = "";//拼音（身高，体重，鞋码，衣服码数）
                            string PersonStr_D3 = "";//拼音（身高，体重，鞋码，衣服码数，性别）
                            string PersonStr_D4 = "";//拼音（身高，体重，鞋码，衣服码数，眼镜度数）
                            string PersonStr_D5 = "";//拼音（身高，体重，鞋码，衣服码数，眼镜度，性别）
                            foreach (var one in list)
                            {

                                PersonStr += one.SelectToken("TravellerEnname").ToString() + "(" + one.SelectToken("PassportNo").ToString() + "), ";
                                PersonStr_M1 += one.SelectToken("TravellerEnname").ToString() + "(" + one.SelectToken("PassportNo").ToString() + "," + DateTime.Parse(one.SelectToken("Birthday").ToString()).ToString("yyyy-MM-dd") + "), ";
                                PersonStr_M2 += one.SelectToken("TravellerName").ToString() + "(" + one.SelectToken("TravellerEnname").ToString() + "," + one.SelectToken("PassportNo").ToString() + "," + DateTime.Parse(one.SelectToken("Birthday").ToString()).ToString("yyyy-MM-dd") + "), ";
                                PersonStr_M3 += one.SelectToken("TravellerName").ToString() + "(" + one.SelectToken("TravellerEnname").ToString() + "," + one.SelectToken("PassportNo").ToString() + "," + DateTime.Parse(one.SelectToken("Birthday").ToString()).ToString("yyyy-MM-dd") + "," + one.SelectToken("TravellerSex").ToString() == "0" ? "M" : "F" + "), ";
                                PersonStr_D1 += one.SelectToken("TravellerEnname").ToString() + "," + one.SelectToken("Height").ToString() + "cm," + one.SelectToken("Weight").ToString() + "kg," + one.SelectToken("ShoesSize").ToString() + "<br/>";
                                PersonStr_D2 += one.SelectToken("TravellerEnname").ToString() + "," + one.SelectToken("Height").ToString() + "cm," + one.SelectToken("Weight").ToString() + "kg," + one.SelectToken("ShoesSize").ToString() + "," + (one.SelectToken("ClothesSize").ToString() == "" ? "N/A" : one.SelectToken("ClothesSize").ToString()) + "<br/>";
                                PersonStr_D3 += one.SelectToken("TravellerEnname").ToString() + "," + one.SelectToken("Height").ToString() + "cm," + one.SelectToken("Weight").ToString() + "kg," + one.SelectToken("ShoesSize").ToString() + "," + (one.SelectToken("ClothesSize").ToString() == "" ? "N/A" : one.SelectToken("ClothesSize").ToString()) + "," + (one.SelectToken("TravellerSex").ToString() == "0" ? "M" : "F") + "<br/>";
                                PersonStr_D4 += one.SelectToken("TravellerEnname").ToString() + "," + one.SelectToken("Height").ToString() + "cm," + one.SelectToken("Weight").ToString() + "kg," + one.SelectToken("ShoesSize").ToString() + "," + (one.SelectToken("ClothesSize").ToString() == "" ? "N/A" : one.SelectToken("ClothesSize").ToString()) + "," + (one.SelectToken("GlassesNum").ToString() == "" ? "N/A" : one.SelectToken("GlassesNum").ToString()) + "<br/>";
                                PersonStr_D5 += one.SelectToken("TravellerEnname").ToString() + "," + one.SelectToken("Height").ToString() + "cm," + one.SelectToken("Weight").ToString() + "kg," + one.SelectToken("ShoesSize").ToString() + "," + (one.SelectToken("ClothesSize").ToString() == "" ? "N/A" : one.SelectToken("ClothesSize").ToString()) + "," + (one.SelectToken("GlassesNum").ToString() == "" ? "N/A" : one.SelectToken("GlassesNum").ToString()) + "," + (one.SelectToken("TravellerSex").ToString() == "0" ? "M" : "F") + "<br/>";
                            }
                            //供应商处于拒绝待检则不显示变更后的内容
                            if (order.state != OrderState.Full)
                            {
                                //查询请求变更模板值，如果有值且不一致，则显示变更的值，鼠标移上去可以看到正式的值
                                if (!string.IsNullOrEmpty(Changejson))
                                {
                                    JObject joChange = JObject.Parse(Changejson);
                                    var changetoken = joChange.SelectToken("$.." + item.Key);

                                    if (changetoken != null)
                                    {
                                        var changelist = changetoken.SelectToken("value");
                                        string changePersonStr = "";
                                        string changePersonStr_M1 = "";
                                        string changePersonStr_M2 = "";
                                        string changePersonStr_M3 = "";
                                        string changePersonStr_D1 = "";
                                        string changePersonStr_D2 = "";
                                        string changePersonStr_D3 = "";
                                        string changePersonStr_D4 = "";
                                        string changePersonStr_D5 = "";
                                        foreach (var one in changelist)
                                        {
                                            changePersonStr += one.SelectToken("TravellerEnname").ToString() + "(" + one.SelectToken("PassportNo").ToString() + "), ";
                                            changePersonStr_M1 += one.SelectToken("TravellerEnname").ToString() + "(" + one.SelectToken("PassportNo").ToString() + "," + DateTime.Parse(one.SelectToken("Birthday").ToString()).ToString("yyyy-MM-dd") + "), ";
                                            changePersonStr_M2 += one.SelectToken("TravellerName").ToString() + "(" + one.SelectToken("TravellerEnname").ToString() + "," + one.SelectToken("PassportNo").ToString() + "," + DateTime.Parse(one.SelectToken("Birthday").ToString()).ToString("yyyy-MM-dd") + "), ";
                                            changePersonStr_M3 += one.SelectToken("TravellerName").ToString() + "(" + one.SelectToken("TravellerEnname").ToString() + "," + one.SelectToken("PassportNo").ToString() + "," + DateTime.Parse(one.SelectToken("Birthday").ToString()).ToString("yyyy-MM-dd") + "," + one.SelectToken("TravellerSex").ToString() == "0" ? "M" : "F" + "), ";
                                            changePersonStr_D1 += one.SelectToken("TravellerEnname").ToString() + "," + one.SelectToken("Height").ToString() + "cm," + one.SelectToken("Weight").ToString() + "kg," + one.SelectToken("ShoesSize").ToString() + "<br/>";
                                            changePersonStr_D2 += one.SelectToken("TravellerEnname").ToString() + "," + one.SelectToken("Height").ToString() + "cm," + one.SelectToken("Weight").ToString() + "kg," + one.SelectToken("ShoesSize").ToString() + "," + (one.SelectToken("ClothesSize").ToString() == "" ? "N/A" : one.SelectToken("ClothesSize").ToString()) + "<br/>";
                                            changePersonStr_D3 += one.SelectToken("TravellerEnname").ToString() + "," + one.SelectToken("Height").ToString() + "cm," + one.SelectToken("Weight").ToString() + "kg," + one.SelectToken("ShoesSize").ToString() + "," + (one.SelectToken("ClothesSize").ToString() == "" ? "N/A" : one.SelectToken("ClothesSize").ToString()) + "," + (one.SelectToken("TravellerSex").ToString() == "0" ? "M" : "F") + "<br/>";
                                            changePersonStr_D4 += one.SelectToken("TravellerEnname").ToString() + "," + one.SelectToken("Height").ToString() + "cm," + one.SelectToken("Weight").ToString() + "kg," + one.SelectToken("ShoesSize").ToString() + "," + (one.SelectToken("ClothesSize").ToString() == "" ? "N/A" : one.SelectToken("ClothesSize").ToString()) + "," + (one.SelectToken("GlassesNum").ToString() == "" ? "N/A" : one.SelectToken("GlassesNum").ToString()) + "<br/>";
                                            changePersonStr_D5 += one.SelectToken("TravellerEnname").ToString() + "," + one.SelectToken("Height").ToString() + "cm," + one.SelectToken("Weight").ToString() + "kg," + one.SelectToken("ShoesSize").ToString() + "," + (one.SelectToken("ClothesSize").ToString() == "" ? "N/A" : one.SelectToken("ClothesSize").ToString()) + "," + (one.SelectToken("GlassesNum").ToString() == "" ? "N/A" : one.SelectToken("GlassesNum").ToString()) + "," + (one.SelectToken("TravellerSex").ToString() == "0" ? "M" : "F") + "<br/>";
                                        }
                                        //供应商处于确认待检则只显示变更后的内容
                                        if (order.state == OrderState.Confirm)
                                        {
                                            if (changePersonStr_D5 != PersonStr_D5)
                                            {
                                                html = html.Replace("#" + item.Key + "_D5#", changePersonStr_D5);
                                            }
                                            if (changePersonStr_D4 != PersonStr_D4)
                                            {
                                                html = html.Replace("#" + item.Key + "_D4#", changePersonStr_D4);
                                            }
                                            if (changePersonStr_D3 != PersonStr_D3)
                                            {
                                                html = html.Replace("#" + item.Key + "_D3#", changePersonStr_D3);
                                            }
                                            if (changePersonStr_D2 != PersonStr_D2)
                                            {
                                                html = html.Replace("#" + item.Key + "_D2#", changePersonStr_D2);
                                            }
                                            if (changePersonStr_D1 != PersonStr_D1)
                                            {
                                                html = html.Replace("#" + item.Key + "_D1#", changePersonStr_D1);
                                            }
                                            if (changePersonStr_M3 != PersonStr_M3)
                                            {
                                                html = html.Replace("#" + item.Key + "_M3#", changePersonStr_M3);
                                            }
                                            if (changePersonStr_M2 != PersonStr_M2)
                                            {
                                                html = html.Replace("#" + item.Key + "_M2#", changePersonStr_M2);
                                            }
                                            if (changePersonStr_M1 != PersonStr_M1)
                                            {
                                                html = html.Replace("#" + item.Key + "_M1#", changePersonStr_M1);
                                            }
                                            if (changePersonStr != PersonStr)
                                            {
                                                html = html.Replace("#" + item.Key + "#", changePersonStr);
                                            }
                                        }
                                        //否则显示变更的值，鼠标移上去可以看到正式的值
                                        else
                                        {
                                            if (changePersonStr_D5 != PersonStr_D5)
                                            {
                                                html = html.Replace("#" + item.Key + "_D5#", "<span class='orderchange' data-change='" + "原值：" + PersonStr_D5 + "'>" + changePersonStr_D5 + "</span>");
                                            }
                                            if (changePersonStr_D4 != PersonStr_D4)
                                            {
                                                html = html.Replace("#" + item.Key + "_D4#", "<span class='orderchange' data-change='" + "原值：" + PersonStr_D4 + "'>" + changePersonStr_D4 + "</span>");
                                            }
                                            if (changePersonStr_D3 != PersonStr_D3)
                                            {
                                                html = html.Replace("#" + item.Key + "_D3#", "<span class='orderchange' data-change='" + "原值：" + PersonStr_D3 + "'>" + changePersonStr_D3 + "</span>");
                                            }
                                            if (changePersonStr_D2 != PersonStr_D2)
                                            {
                                                html = html.Replace("#" + item.Key + "_D2#", "<span class='orderchange' data-change='" + "原值：" + PersonStr_D2 + "'>" + changePersonStr_D2 + "</span>");
                                            }
                                            if (changePersonStr_D1 != PersonStr_D1)
                                            {
                                                html = html.Replace("#" + item.Key + "_D1#", "<span class='orderchange' data-change='" + "原值：" + PersonStr_D1 + "'>" + changePersonStr_D1 + "</span>");
                                            }
                                            if (changePersonStr_M3 != PersonStr_M3)
                                            {
                                                html = html.Replace("#" + item.Key + "_M3#", "<span class='orderchange' data-change='" + "原值：" + PersonStr_M3 + "'>" + changePersonStr_M3 + "</span>");
                                            }
                                            if (changePersonStr_M2 != PersonStr_M2)
                                            {
                                                html = html.Replace("#" + item.Key + "_M2#", "<span class='orderchange' data-change='" + "原值：" + PersonStr_M2 + "'>" + changePersonStr_M2 + "</span>");
                                            }
                                            if (changePersonStr_M1 != PersonStr_M1)
                                            {
                                                html = html.Replace("#" + item.Key + "_M1#", "<span class='orderchange' data-change='" + "原值：" + PersonStr_M1 + "'>" + changePersonStr_M1 + "</span>");
                                            }
                                            if (changePersonStr != PersonStr)
                                            {
                                                html = html.Replace("#" + item.Key + "#", "<span class='orderchange' data-change='" + "原值：" + PersonStr + "'>" + changePersonStr + "</span>");
                                            }
                                        }
                                    }
                                }
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
                            //供应商处于拒绝待检则只显示变更前的内容
                            if (order.state != OrderState.Full)
                            {
                                //查询请求变更模板值，如果有值且不一致，则显示变更的值，鼠标移上去可以看到正式的值
                                if (!string.IsNullOrEmpty(Changejson))
                                {
                                    JObject joChange = JObject.Parse(Changejson);
                                    var changetoken = joChange.SelectToken("$.." + item.Key);
                                    if (changetoken != null)
                                    {
                                        if (changetoken.ToString() != item.Value.ToString())
                                        {
                                            //供应商处于确认待检则只显示变更后的内容
                                            if (order.state == OrderState.Confirm)
                                            {
                                                html = html.Replace("#" + item.Key + "#", changetoken.ToString());
                                            }
                                            else
                                            {
                                                html = html.Replace("#" + item.Key + "#", "<span class='orderchange' data-change='" + "原值：" + item.Value.ToString() + "'>" + changetoken.ToString() + "</span>");
                                            }
                                        }
                                    }
                                }
                            }
                            html = html.Replace("#" + item.Key + "#", item.Value.ToString());
                        }
                    }
                }
            }
            catch
            {

            }

            return html.Trim().Replace("﻿", "");
        }

        //获取订单列表
        public async Task<string> GetOrders(ShareSearchModel search)
        {
            string SupplierUserName = User.Identity.Name;
            SupplierUser user = await client.For<SupplierUser>()
                .Expand(s => s.OneSupplier)
                .Filter(s => s.SupplierUserName == SupplierUserName).FindEntryAsync();

            int draw = 1;
            int start = 0;
            int length = 10;
            string propertyName = "OrderNo";
            int sort = 0;
            //string status = "3,4,5,6,7,8,9,10,11,12,14,15";
            string status = "";
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

            IBoundClient<Order> OrderResult = client.For<Order>()
                .Expand(t => t.TBOrders)
                .Expand(t => t.TBOrders.Sourse)
                .Expand(t => t.ServiceItemHistorys)
                .Expand(t => t.Customers)
                .Skip(start).Top(length);
            IBoundClient<Order> OrderCountResult = client.For<Order>().Expand(t => t.Customers);
            //排序
            if (propertyName == "TravelDate")
            {
                OrderResult = sort == 0 ? OrderResult.OrderByDescending("ServiceItemHistorys/TravelDate") : OrderResult.OrderBy("ServiceItemHistorys/TravelDate");
            }
            else if (propertyName == "GroupNo")
            {
                OrderResult = sort == 0 ? OrderResult.OrderByDescending("ServiceItemHistorys/GroupNo") : OrderResult.OrderBy("ServiceItemHistorys/GroupNo");
            }
            else
            {
                OrderResult = sort == 0 ? OrderResult.OrderByDescending(propertyName) : OrderResult.OrderBy(propertyName);
            }
            //搜索条件
            OrderResult = OrderResult.Filter("ServiceItemHistorys/SupplierID eq " + user.OneSupplier.SupplierID);
            OrderCountResult = OrderCountResult.Filter("ServiceItemHistorys/SupplierID eq " + user.OneSupplier.SupplierID);
            if (search.OrderSearch != null)
            {
                if (!string.IsNullOrEmpty(search.OrderSearch.status) && search.OrderSearch.status != "-1")
                {
                    status = search.OrderSearch.status;
                }
            }
            //状态，以逗号分开
            string statefilter = "";
            if (status == "")
            {
                statefilter = "state gt LanghuaNew.Data.OrderState'2' and state lt LanghuaNew.Data.OrderState'16' and state ne LanghuaNew.Data.OrderState'13'";
            }
            else
            {
                string[] state = status.Trim().Split(',');
                foreach (var item in state)
                {
                    if (statefilter != "")
                    {
                        statefilter += " or ";
                    }
                    statefilter += "state eq LanghuaNew.Data.OrderState'" + item + "'";
                }
            }
            OrderResult = OrderResult.Filter(statefilter);
            OrderCountResult = OrderCountResult.Filter(statefilter);

            if (search.OrderSearch != null)
            {
                if (!string.IsNullOrEmpty(search.OrderSearch.FuzzySearch))
                {//姓名、电话、订单号、团号
                    search.OrderSearch.FuzzySearch = search.OrderSearch.FuzzySearch.Trim();
                    string filter = "contains(CustomerName,'" + search.OrderSearch.FuzzySearch + "') or contains(CustomerEnname,'" + search.OrderSearch.FuzzySearch + "') or contains(Tel,'" + search.OrderSearch.FuzzySearch + "') or contains(BakTel,'" + search.OrderSearch.FuzzySearch + "') or contains(OrderNo,'" + search.OrderSearch.FuzzySearch + "') or contains(ServiceItemHistorys/GroupNo,'" + search.OrderSearch.FuzzySearch + "')";
                    OrderResult = OrderResult.Filter(filter);
                    OrderCountResult = OrderCountResult.Filter(filter);
                }
                if (!string.IsNullOrEmpty(search.OrderSearch.OrderCreateDateBegin))
                {//订单创建时间
                    string filter = "CreateTime ge " + search.OrderSearch.OrderCreateDateBegin.Trim();
                    OrderResult = OrderResult.Filter(filter);
                    OrderCountResult = OrderCountResult.Filter(filter);
                }
                if (!string.IsNullOrEmpty(search.OrderSearch.OrderCreateDateEnd))
                {
                    try
                    {
                        DateTime endTime = DateTime.Parse(search.OrderSearch.OrderCreateDateEnd.Trim()).AddDays(1);
                        string filter = "CreateTime lt " + endTime.ToString("yyyy-MM-dd");
                        OrderResult = OrderResult.Filter(filter);
                        OrderCountResult = OrderCountResult.Filter(filter);
                    }
                    catch { }
                }
                if (!string.IsNullOrEmpty(search.OrderSearch.OrderSendDateBegin))
                {//订单发送时间
                    string filter = "OrderHistorys/any(x1:(x1/Remark eq '状态:已核对→已发送' or x1/Remark eq '状态:已核对→新单已接') and x1/OperTime ge " + search.OrderSearch.OrderSendDateBegin.Trim() + ")";
                    OrderResult = OrderResult.Filter(filter);
                    OrderCountResult = OrderCountResult.Filter(filter);
                }
                if (!string.IsNullOrEmpty(search.OrderSearch.OrderSendDateEnd))
                {
                    try
                    {
                        DateTime endTime = DateTime.Parse(search.OrderSearch.OrderSendDateEnd.Trim()).AddDays(1);
                        string filter = "OrderHistorys/any(x1:(x1/Remark eq '状态:已核对→已发送' or x1/Remark eq '状态:已核对→新单已接') and x1/OperTime lt " + endTime.ToString("yyyy-MM-dd") + ")";
                        OrderResult = OrderResult.Filter(filter);
                        OrderCountResult = OrderCountResult.Filter(filter);
                    }
                    catch { }
                }
                if (!string.IsNullOrEmpty(search.OrderSearch.OrderOperDateBegin))
                {//订单操作时间（供应商）
                    string filter = "OrderSupplierHistorys/any(x1:x1/opera eq LanghuaNew.Data.OrderOperator'supplier' and x1/OperTime ge " + search.OrderSearch.OrderOperDateBegin.Trim() + ")";
                    OrderResult = OrderResult.Filter(filter);
                    OrderCountResult = OrderCountResult.Filter(filter);
                }
                if (!string.IsNullOrEmpty(search.OrderSearch.OrderOperDateEnd))
                {
                    try
                    {
                        DateTime endTime = DateTime.Parse(search.OrderSearch.OrderOperDateEnd.Trim()).AddDays(1);
                        string filter = "OrderSupplierHistorys/any(x1:x1/opera eq LanghuaNew.Data.OrderOperator'supplier' and x1/OperTime lt " + endTime.ToString("yyyy-MM-dd") + ")";
                        OrderResult = OrderResult.Filter(filter);
                        OrderCountResult = OrderCountResult.Filter(filter);
                    }
                    catch { }
                }
                if (search.OrderSearch.IsChangeTravelDate == "true")
                {//只查变更中的出行日期
                    if (!string.IsNullOrEmpty(search.OrderSearch.TravelDateBegin))
                    {//出行时间
                        string filter = "ServiceItemHistorys/ChangeTravelDate ge " + search.OrderSearch.TravelDateBegin.Trim();
                        OrderResult = OrderResult.Filter(filter);
                        OrderCountResult = OrderCountResult.Filter(filter);
                    }
                    if (!string.IsNullOrEmpty(search.OrderSearch.TravelDateEnd))
                    {
                        try
                        {
                            DateTime endTime = DateTime.Parse(search.OrderSearch.TravelDateEnd.Trim()).AddDays(1);
                            string filter = "ServiceItemHistorys/ChangeTravelDate lt " + endTime.ToString("yyyy-MM-dd");
                            OrderResult = OrderResult.Filter(filter);
                            OrderCountResult = OrderCountResult.Filter(filter);
                        }
                        catch { }
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(search.OrderSearch.TravelDateBegin))
                    {//出行时间
                        string filter = "ServiceItemHistorys/TravelDate ge " + search.OrderSearch.TravelDateBegin.Trim();
                        OrderResult = OrderResult.Filter(filter);
                        OrderCountResult = OrderCountResult.Filter(filter);
                    }
                    if (!string.IsNullOrEmpty(search.OrderSearch.TravelDateEnd))
                    {
                        try
                        {
                            DateTime endTime = DateTime.Parse(search.OrderSearch.TravelDateEnd.Trim()).AddDays(1);
                            string filter = "ServiceItemHistorys/TravelDate lt " + endTime.ToString("yyyy-MM-dd");
                            OrderResult = OrderResult.Filter(filter);
                            OrderCountResult = OrderCountResult.Filter(filter);
                        }
                        catch { }
                    }
                }
                if (!string.IsNullOrEmpty(search.OrderSearch.ReturnDateBegin))
                {//返回时间
                    string filter = "ServiceItemHistorys/ReturnDate ge " + search.OrderSearch.ReturnDateBegin.Trim();
                    OrderResult = OrderResult.Filter(filter);
                    OrderCountResult = OrderCountResult.Filter(filter);
                }
                if (!string.IsNullOrEmpty(search.OrderSearch.ReturnDateEnd))
                {
                    try
                    {
                        DateTime endTime = DateTime.Parse(search.OrderSearch.ReturnDateEnd.Trim()).AddDays(1);
                        string filter = "ServiceItemHistorys/ReturnDate lt " + endTime.ToString("yyyy-MM-dd");
                        OrderResult = OrderResult.Filter(filter);
                        OrderCountResult = OrderCountResult.Filter(filter);
                    }
                    catch { }
                }
                if (search.OrderSearch.isUrgent == "true")
                {//紧急订单
                    string filter = "isUrgent eq true";
                    OrderResult = OrderResult.Filter(filter);
                    OrderCountResult = OrderCountResult.Filter(filter);
                }
                if (!string.IsNullOrEmpty(search.OrderSearch.ServiceTypeID))
                {
                    string filter = "ServiceItemHistorys/ServiceTypeID eq " + search.OrderSearch.ServiceTypeID;
                    OrderResult = OrderResult.Filter(filter);
                }
                if (!string.IsNullOrEmpty(search.OrderSearch.ItemName))
                {//产品，以空格分开
                    string[] name = search.OrderSearch.ItemName.Trim().Split(' ');
                    string filter = "";
                    foreach (var item in name)
                    {
                        if (filter != "")
                        {
                            filter += " or ";
                        }
                        filter += "contains(ServiceItemHistorys/cnItemName,'" + item + "') or contains(ServiceItemHistorys/enItemName,'" + item + "') or ServiceItemHistorys/ServiceCode eq '" + item + "'";
                    }
                    OrderResult = OrderResult.Filter(filter);
                    OrderCountResult = OrderCountResult.Filter(filter);
                }
            }

            int count = await OrderCountResult.Count().FindScalarAsync<int>();
            var items = await OrderResult.Select(s => new
            {
                s.OrderID,
                s.OrderNo,
                s.CustomerName,
                s.CustomerEnname,
                s.ServiceItemHistorys.cnItemName,
                s.ServiceItemHistorys.ServiceCode,
                s.ServiceItemHistorys.AdultNum,
                s.ServiceItemHistorys.ChildNum,
                s.ServiceItemHistorys.INFNum,
                s.ServiceItemHistorys.TravelDate,
                s.ServiceItemHistorys.ChangeTravelDate,
                s.state,
                s.ServiceItemHistorys.GroupNo,
                s.Remark,
                s.CustomerID,
                s.isUrgent
            }).FindEntriesAsync();
            var list = items.Select(s => new
            {
                s.OrderID,
                s.OrderNo,
                CustomerName = s.CustomerName == null ? "" : s.CustomerName,
                CustomerEnname = s.CustomerEnname == null ? "" : s.CustomerEnname,
                s.ServiceItemHistorys.cnItemName,
                s.ServiceItemHistorys.ServiceCode,
                s.ServiceItemHistorys.AdultNum,
                s.ServiceItemHistorys.ChildNum,
                s.ServiceItemHistorys.INFNum,
                TravelDate = (s.state == OrderState.Confirm && s.ServiceItemHistorys.ChangeTravelDate > Convert.ToDateTime("1900-01-01"))
                ? s.ServiceItemHistorys.ChangeTravelDate.ToString("yyyy-MM-dd")
                : (s.ServiceItemHistorys.TravelDate < Convert.ToDateTime("1901-01-02")) ? "" : s.ServiceItemHistorys.TravelDate.ToString("yyyy-MM-dd"),
                s.state,
                stateName = EnumHelper.GetEnumDescription(s.state).Substring(EnumHelper.GetEnumDescription(s.state).IndexOf("|") + 1),
                GroupNo = s.ServiceItemHistorys.GroupNo == null ? "" : s.ServiceItemHistorys.GroupNo,
                Remark = s.Remark == null ? "" : s.Remark,
                s.CustomerID,
                s.isUrgent
            });
            return JsonConvert.SerializeObject(new { draw = draw, recordsFiltered = count, data = list, SearchModel = search });
        }

        [HttpPost]
        public async Task<string> Print(int? id, string tipsWidth)
        {
            Order order = await GetOrderByID(id);
            string detailHtml = await GetConfimOrderHtml(id, order);

            string htmlPath = string.Empty;
            //使用http的URL
            string url = OfficeHelper.CreateHtml(detailHtml, ref htmlPath);
            url = this.Request.Url.Authority + url;
            if (!string.IsNullOrEmpty(tipsWidth))
            {
                url = url + "?tipsWidth=" + tipsWidth;
            }
            string pdfDirectory = Server.MapPath("~/data/Pdf/");
            OfficeHelper.CreateDirectory(pdfDirectory);
            string pdfPhyName = DateTime.Now.ToString("yyyyMMddhhmmssfffff") + ".pdf";
            string pdfPath = pdfDirectory + pdfPhyName;
            string urlPath = "http://" + this.Request.Url.Authority + "/data/Pdf/" + pdfPhyName;
            OfficeHelper.DelteAgoFiles(pdfPath);
            using (IPechkin pechkin = Factory.Create(new GlobalConfig()))
            {
                ObjectConfig oc = new ObjectConfig();
                oc.SetPrintBackground(true)
                    .SetLoadImages(true)
                    .SetScreenMediaType(true)
                    .SetPageUri(url);
                byte[] pdf = pechkin.Convert(oc);
                System.IO.File.WriteAllBytes(pdfPath, pdf);
                OfficeHelper.DeleteFile(htmlPath);
                return JsonConvert.SerializeObject(new { ErrorCode = 200, ErrorMessage = "OK", UrlPath = urlPath });
            }
        }

        public async Task<FileResult> DownloadPdf(int? id, string fileName, string isNotes, string isCancel, string supplierenname)
        {
            Order order = await GetOrderByID(id);
            string detailHtml = await GetConfimOrderHtml(id, order);

            string htmlPath = string.Empty;
            //使用http的URL
            string url = OfficeHelper.CreateHtml(detailHtml, ref htmlPath);
            url = this.Request.Url.Authority + url;
            url = url + "?isNotes=" + isNotes + "&isCancel=" + isCancel + "&supEnName=" + supplierenname;
            using (IPechkin pechkin = Factory.Create(new GlobalConfig()))
            {
                ObjectConfig oc = new ObjectConfig();
                oc.SetPrintBackground(true)
                    .SetLoadImages(true)
                    .SetScreenMediaType(true)
                    .SetPageUri(url);
                byte[] pdf = pechkin.Convert(oc);
                OfficeHelper.DeleteFile(htmlPath);
                return File(pdf, "application/pdf", Server.UrlDecode(fileName));
            }
        }

        public async Task<FileResult> DownloadWord(int? id, string fileName, string isNotes, string isCancel, string supplierenname)
        {
            Order order = await GetOrderByID(id);
            string detailHtml = await GetConfimOrderHtml(id, order);
            //html文件物理地址
            string htmlPath = string.Empty;
            //使用http的URL
            string url = OfficeHelper.CreateHtml(detailHtml, ref htmlPath);
            url = "http://" + this.Request.Url.Authority + url;
            url = url + "?isNotes=" + isNotes + "&isCancel=" + isCancel + "&supEnName=" + supplierenname;
            string Html = string.Empty;
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            using (StreamReader Reader = new StreamReader(req.GetResponse().GetResponseStream(), Encoding.Default))
            {
                Html = await Reader.ReadToEndAsync();
                if (isNotes == "false")
                {
                    Regex re = new Regex(@"<div\sid=""notes"".*?</div>", RegexOptions.CultureInvariant);
                    string str = re.Match(Html).ToString();
                    Html = Html.Replace(str, "");
                }
                if (isCancel == "false")
                {
                    Regex re = new Regex(@"<div\sid=""cancels"".*?</div>", RegexOptions.CultureInvariant);
                    string str2 = re.Match(Html).ToString();
                    Html = Html.Replace(str2, "");
                }
                if (!string.IsNullOrEmpty(supplierenname))
                {
                    Regex re = new Regex(@"<span\sid=""supEnName"".*?</span>", RegexOptions.CultureInvariant);
                    string str3 = re.Match(Html).ToString();
                    Html = Html.Replace(str3, Server.UrlDecode(supplierenname));
                }
            }
            //word文件路径
            string wordPath = System.Web.HttpContext.Current.Server.MapPath("~/data/PdfHtml/") + Guid.NewGuid().ToString() + ".docx";
            // 写入到服务器端 
            var doc = new Aspose.Words.Document();
            DocumentBuilder builder = new DocumentBuilder(doc);
            Aspose.Words.Font font = builder.Font;
            font.Name = "Microsoft YaHei";
            ParagraphFormat paragraphFormat = builder.ParagraphFormat;
            paragraphFormat.FirstLineIndent = 8;
            paragraphFormat.Alignment = ParagraphAlignment.Justify;
            paragraphFormat.KeepTogether = true;

            builder.InsertHtml(Html.Replace("../..", Server.MapPath("~")), true);
            doc.Save(wordPath);
            // 写入内存,返回给客户端
            using (MemoryStream ms = OfficeHelper.WriteToClient(wordPath))
            {
                OfficeHelper.DeleteFile(htmlPath);
                OfficeHelper.DeleteFile(wordPath);
                return File(ms.ToArray(), "application/ms-word", Server.UrlDecode(fileName));
            }
        }

        //导出excel
        public async Task<FileResult> ExportExcel(ShareOrderSearchModel search)
        {
            #region 获取列表
            string SupplierUserName = User.Identity.Name;
            SupplierUser user = await client.For<SupplierUser>()
                .Expand(s => s.OneSupplier)
                .Filter(s => s.SupplierUserName == SupplierUserName).FindEntryAsync();

            string propertyName = "OrderNo";
            int sort = 0;
            string status = "";// "3,4,5,6,7,8,9,10,11,12,14,15";

            IBoundClient<Order> OrderResult = client.For<Order>()
                .Expand(t => t.TBOrders)
                .Expand(t => t.TBOrders.Sourse)
                .Expand(t => t.ServiceItemHistorys)
                .Expand(t => t.ServiceItemHistorys.ExtraServiceHistorys)
                .Expand(t => t.Customers);

            if (propertyName == "TravelDate")
            {
                OrderResult = sort == 0 ? OrderResult.OrderByDescending(s => s.ServiceItemHistorys.TravelDate) : OrderResult.OrderBy(s => s.ServiceItemHistorys.TravelDate);
            }
            else if (propertyName == "GroupNo")
            {
                OrderResult = sort == 0 ? OrderResult.OrderByDescending("ServiceItemHistorys/GroupNo") : OrderResult.OrderBy("ServiceItemHistorys/GroupNo");
            }
            else
            {
                OrderResult = sort == 0 ? OrderResult.OrderByDescending(propertyName) : OrderResult.OrderBy(propertyName);
            }
            OrderResult = OrderResult.Filter("ServiceItemHistorys/SupplierID eq " + user.OneSupplier.SupplierID);
            if (!string.IsNullOrEmpty(search.status) && search.status != "-1")
            {
                status = search.status;
            }
            //状态，以逗号分开
            string statefilter = "";
            if (status == "")
            {
                statefilter = "state gt LanghuaNew.Data.OrderState'2' and state lt LanghuaNew.Data.OrderState'16' and state ne LanghuaNew.Data.OrderState'13'";
            }
            else
            {
                string[] state = status.Trim().Split(',');
                foreach (var item in state)
                {
                    if (statefilter != "")
                    {
                        statefilter += " or ";
                    }
                    statefilter += "state eq LanghuaNew.Data.OrderState'" + item + "'";
                }
            }
            OrderResult = OrderResult.Filter(statefilter);

            if (!string.IsNullOrEmpty(search.FuzzySearch))
            {//姓名、电话、订单号、团号
                string filter = "contains(CustomerName,'" + search.FuzzySearch + "') or contains(CustomerEnname,'" + search.FuzzySearch + "') or contains(Tel,'" + search.FuzzySearch + "') or contains(BakTel,'" + search.FuzzySearch + "') or contains(OrderNo,'" + search.FuzzySearch + "') or contains(ServiceItemHistorys/GroupNo,'" + search.FuzzySearch + "')";
                OrderResult = OrderResult.Filter(filter);
            }
            if (!string.IsNullOrEmpty(search.OrderCreateDateBegin))
            {
                string filter = "CreateTime ge " + search.OrderCreateDateBegin;
                OrderResult = OrderResult.Filter(filter);
            }
            if (!string.IsNullOrEmpty(search.OrderCreateDateEnd))
            {
                try
                {
                    DateTime endTime = DateTime.Parse(search.OrderCreateDateEnd).AddDays(1);
                    string filter = "CreateTime lt " + endTime.ToString("yyyy-MM-dd");
                    OrderResult = OrderResult.Filter(filter);
                }
                catch { }
            }
            if (search.IsChangeTravelDate == "true")
            {//只查变更中的出行日期
                if (!string.IsNullOrEmpty(search.TravelDateBegin))
                {//出行时间
                    string filter = "ServiceItemHistorys/ChangeTravelDate ge " + search.TravelDateBegin.Trim();
                    OrderResult = OrderResult.Filter(filter);
                }
                if (!string.IsNullOrEmpty(search.TravelDateEnd))
                {
                    try
                    {
                        DateTime endTime = DateTime.Parse(search.TravelDateEnd.Trim()).AddDays(1);
                        string filter = "ServiceItemHistorys/ChangeTravelDate lt " + endTime.ToString("yyyy-MM-dd");
                        OrderResult = OrderResult.Filter(filter);
                    }
                    catch { }
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(search.TravelDateBegin))
                {//出行时间
                    string filter = "ServiceItemHistorys/TravelDate ge " + search.TravelDateBegin.Trim();
                    OrderResult = OrderResult.Filter(filter);
                }
                if (!string.IsNullOrEmpty(search.TravelDateEnd))
                {
                    try
                    {
                        DateTime endTime = DateTime.Parse(search.TravelDateEnd.Trim()).AddDays(1);
                        string filter = "ServiceItemHistorys/TravelDate lt " + endTime.ToString("yyyy-MM-dd");
                        OrderResult = OrderResult.Filter(filter);
                    }
                    catch { }
                }
            }
            if (!string.IsNullOrEmpty(search.OrderSendDateBegin))
            {//订单发送时间
                string filter = "OrderHistorys/any(x1:(x1/Remark eq '状态:已核对→已发送' or x1/Remark eq '状态:已核对→新单已接') and x1/OperTime ge " + search.OrderSendDateBegin.Trim() + ")";
                OrderResult = OrderResult.Filter(filter);
            }
            if (!string.IsNullOrEmpty(search.OrderSendDateEnd))
            {
                try
                {
                    DateTime endTime = DateTime.Parse(search.OrderSendDateEnd.Trim()).AddDays(1);
                    string filter = "OrderHistorys/any(x1:(x1/Remark eq '状态:已核对→已发送' or x1/Remark eq '状态:已核对→新单已接') and x1/OperTime lt " + endTime.ToString("yyyy-MM-dd") + ")";
                    OrderResult = OrderResult.Filter(filter);
                }
                catch { }
            }
            if (!string.IsNullOrEmpty(search.ReturnDateBegin))
            {
                string filter = "ServiceItemHistorys/ReturnDate ge " + search.ReturnDateBegin;
                OrderResult = OrderResult.Filter(filter);
            }
            if (!string.IsNullOrEmpty(search.ReturnDateEnd))
            {
                try
                {
                    DateTime endTime = DateTime.Parse(search.ReturnDateEnd).AddDays(1);
                    string filter = "ServiceItemHistorys/ReturnDate lt " + endTime.ToString("yyyy-MM-dd");
                    OrderResult = OrderResult.Filter(filter);
                }
                catch { }
            }
            if (search.isUrgent == "true")
            {//紧急订单
                string filter = "isUrgent eq true";
                OrderResult = OrderResult.Filter(filter);
            }
            if (!string.IsNullOrEmpty(search.ServiceTypeID))
            {
                string filter = "ServiceItemHistorys/ServiceTypeID eq " + search.ServiceTypeID;
                OrderResult = OrderResult.Filter(filter);
            }
            if (!string.IsNullOrEmpty(search.ItemName))
            {
                string[] name = search.ItemName.Split(' ');
                string filter = "";
                foreach (var item in name)
                {
                    if (filter != "")
                    {
                        filter += " or ";
                    }
                    filter += "contains(ServiceItemHistorys/cnItemName,'" + item + "') or contains(ServiceItemHistorys/enItemName,'" + item + "') or ServiceItemHistorys/ServiceCode eq '" + item + "'";
                }
                OrderResult = OrderResult.Filter(filter);
            }
            #endregion

            //创建Excel文件的对象
            NPOI.HSSF.UserModel.HSSFWorkbook book = new NPOI.HSSF.UserModel.HSSFWorkbook();
            //添加一个sheet
            NPOI.SS.UserModel.ISheet sheet1 = book.CreateSheet("Sheet1");
            //search.ExportField = string.IsNullOrEmpty(search.ExportField) ? "0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17" : search.ExportField;
            //导出字段
            if (!string.IsNullOrEmpty(search.ExportField))
            {
                int i = 0;
                //给sheet1添加第一行的头部标题
                NPOI.SS.UserModel.IRow row1 = sheet1.CreateRow(i);

                string[] exportField = search.ExportField.Split(',');
                for (int ex = 0; ex < exportField.Count(); ex++)
                {
                    int FieldValue = int.Parse(exportField[ex]);
                    string cellValue = EnumHelper.GetEnumDescription((ExportField)FieldValue);
                    row1.CreateCell(ex).SetCellValue(cellValue);
                    if ((ExportField)FieldValue == ExportField.ExtraServices)
                    {
                        row1.CreateCell(ex + 1).SetCellValue("数量");
                    }
                }
                row1.Height = 450;

                //将数据逐步写入sheet1各个行
                var items = await OrderResult.FindEntriesAsync();
                foreach (var item in items)
                {
                    i++;
                    NPOI.SS.UserModel.IRow rowtemp = sheet1.CreateRow(i);

                    for (int ex = 0; ex < exportField.Count(); ex++)
                    {
                        int FieldValue = int.Parse(exportField[ex]);
                        switch ((ExportField)FieldValue)
                        {
                            case ExportField.OrderNo:
                                sheet1.SetColumnWidth(ex, 5000);
                                rowtemp.CreateCell(ex).SetCellValue(item.OrderNo == null ? "" : (item.OrderNo.ToString()));
                                break;
                            case ExportField.TBID:
                                rowtemp.CreateCell(ex).SetCellValue(item.TBOrders.TBID);
                                break;
                            case ExportField.cnName:
                                rowtemp.CreateCell(ex).SetCellValue(item.CustomerName);
                                break;
                            case ExportField.enName:
                                rowtemp.CreateCell(ex).SetCellValue(item.CustomerEnname);
                                break;
                            case ExportField.OrderSourse:
                                rowtemp.CreateCell(ex).SetCellValue(item.TBOrders.Sourse.OrderSourseName);
                                break;
                            case ExportField.Tel:
                                sheet1.SetColumnWidth(ex, 3000);
                                rowtemp.CreateCell(ex).SetCellValue(item.Tel);
                                break;
                            case ExportField.BakTel:
                                sheet1.SetColumnWidth(ex, 3000);
                                rowtemp.CreateCell(ex).SetCellValue(item.BakTel);
                                break;
                            case ExportField.SupplierCode:
                                rowtemp.CreateCell(ex).SetCellValue(item.ServiceItemHistorys.SupplierCode);
                                break;
                            case ExportField.cnItemName:
                                sheet1.SetColumnWidth(ex, 5000);
                                rowtemp.CreateCell(ex).SetCellValue(item.ServiceItemHistorys.cnItemName);
                                break;
                            case ExportField.ServiceCode:
                                rowtemp.CreateCell(ex).SetCellValue(item.ServiceItemHistorys.ServiceCode);
                                break;
                            case ExportField.AdultNum:
                                rowtemp.CreateCell(ex).SetCellValue(item.ServiceItemHistorys.AdultNum);
                                break;
                            case ExportField.ChildNum:
                                rowtemp.CreateCell(ex).SetCellValue(item.ServiceItemHistorys.ChildNum);
                                break;
                            case ExportField.INFNum:
                                rowtemp.CreateCell(ex).SetCellValue(item.ServiceItemHistorys.INFNum);
                                break;
                            case ExportField.RoomNum:
                                rowtemp.CreateCell(ex).SetCellValue(item.ServiceItemHistorys.RoomNum);
                                break;
                            case ExportField.NightNum:
                                rowtemp.CreateCell(ex).SetCellValue(item.ServiceItemHistorys.RightNum);
                                break;
                            case ExportField.TravelDate:
                                sheet1.SetColumnWidth(ex, 3000);
                                rowtemp.CreateCell(ex).SetCellValue((item.ServiceItemHistorys.TravelDate < Convert.ToDateTime("1902-02-02")) ? "" : item.ServiceItemHistorys.TravelDate.ToString("yyyy-MM-dd"));
                                break;
                            case ExportField.OrderState:
                                sheet1.SetColumnWidth(ex, 5000);
                                rowtemp.CreateCell(ex).SetCellValue(EnumHelper.GetEnumDescription(item.state).Substring(EnumHelper.GetEnumDescription(item.state).IndexOf("|") + 1));
                                break;
                            case ExportField.GroupNo:
                                sheet1.SetColumnWidth(ex, 5000);
                                rowtemp.CreateCell(ex).SetCellValue(item.ServiceItemHistorys.GroupNo == null ? "" : item.ServiceItemHistorys.GroupNo.ToString());
                                break;
                            case ExportField.Remark:
                                rowtemp.CreateCell(ex).SetCellValue(item.Remark == null ? "" : item.Remark.ToString());
                                break;
                            case ExportField.CreateUserNikeName:
                                rowtemp.CreateCell(ex).SetCellValue(item.CreateUserNikeName);
                                break;
                            case ExportField.ExtraServices:
                                if (item.ServiceItemHistorys.ExtraServiceHistorys != null)
                                {
                                    foreach (var extraServiceHistory in item.ServiceItemHistorys.ExtraServiceHistorys.OrderBy(s => s.ServiceName))
                                    {
                                        rowtemp.CreateCell(ex).SetCellValue(extraServiceHistory.ServiceName);
                                        ex++;
                                        rowtemp.CreateCell(ex).SetCellValue(extraServiceHistory.ServiceNum);
                                        ex++;
                                    }
                                }
                                break;
                        }
                    }
                }
            }
            // 写入到客户端 
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            book.Write(ms);
            ms.Seek(0, SeekOrigin.Begin);
            return File(ms, "application/vnd.ms-excel", "orderlist.xls");
        }
        //订单流转
        [HttpPost]
        public async Task<string> UpdateState(int state, string OrderID)
        {
            System.Threading.Thread.Sleep(1000);
            OrderOperations operation = (OrderOperations)state;//操作
            switch (operation)
            {
                //action接单
                case OrderOperations.Receive:
                    break;
                //action确认
                case OrderOperations.Confirm:
                    break;
                //action拒绝
                case OrderOperations.Full:
                    break;
                //action取消
                case OrderOperations.Cancel:
                    break;
                //其他请求为异常
                default:
                    return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "操作异常！" });
            }

            string SupplierUserName = User.Identity.Name;
            SupplierUser user = await client.For<SupplierUser>()
                .Expand(s => s.OneSupplier)
                .Filter(s => s.SupplierUserName == SupplierUserName).FindEntryAsync();

            var successed = (new int[] { 1 }).Select(x => new { OrderNo = "" }).ToList();
            var failed = (new int[] { 1 }).Select(x => new { OrderNo = "", reason = "" }).ToList();
            successed.Clear();
            failed.Clear();

            if (string.IsNullOrEmpty(OrderID))
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "ID不能为空！" });
            }
            try
            {
                var id = OrderID.Split(',');
                foreach (var i in id)
                {
                    #region MyRegion
                    Order order = await client.For<Order>().Expand(t => t.Customers)
                                   .Expand(t => t.OrderHistorys)
                                   .Expand(t => t.ServiceItemHistorys)
                                   .Key(int.Parse(i)).FindEntryAsync();
                    OrderState oldState = order.state;//原订单状态
                    if (oldState == OrderState.Invalid)
                    {
                        failed.Add(new { OrderNo = order.OrderNo, reason = "订单已作废" });
                    }
                    else if (oldState == OrderState.SencondCancel)
                    {
                        failed.Add(new { OrderNo = order.OrderNo, reason = "订单已取消" });
                    }
                    else
                    {
                        string strOldState = EnumHelper.GetEnumDescription(oldState).Substring(EnumHelper.GetEnumDescription(oldState).IndexOf("|") + 1);
                        string strOperation = EnumHelper.GetEnumDescription(operation);

                        bool bl = false;
                        string reason = strOldState + "状态不能进行" + strOperation + "操作";

                        if (order.ServiceItemHistorys.SupplierID != user.SupplierID)
                        {
                            reason = "订单异常";
                            bl = true;
                        }

                        //获取新状态
                        OrderState newState = OrderStateHelper.GetOrderState(oldState, operation, OrderOperator.supplier);

                        if (bl || newState == OrderState.nullandvoid)
                        {
                            failed.Add(new { OrderNo = order.OrderNo, reason = reason });
                        }
                        else
                        {
                            string strNewState = EnumHelper.GetEnumDescription(newState).Substring(EnumHelper.GetEnumDescription(newState).IndexOf("|") + 1);
                            order.state = newState;
                            order.CustomerState = OrderStateHelper.GetOrderCustomerState(newState, oldState);
                            await client.For<Order>().Key(order.OrderID).Set(order).UpdateEntryAsync();
                            await client.For<OrderHistory>().Set(new OrderHistory
                            {
                                OrderID = order.OrderID,
                                OperUserID = user.SupplierUserID.ToString(),
                                OperUserNickName = "(供应商)" + user.SupplierNickName,
                                OperTime = DateTime.Now,
                                State = newState,
                                Remark = "状态:" + EnumHelper.GetEnumDescription(oldState).Substring(0, EnumHelper.GetEnumDescription(oldState).IndexOf("|")) + "→" + EnumHelper.GetEnumDescription(newState).Substring(0, EnumHelper.GetEnumDescription(newState).IndexOf("|"))
                            }).InsertEntryAsync();
                            await client.For<OrderSupplierHistory>().Set(new OrderSupplierHistory
                            {
                                opera = OrderOperator.supplier,
                                State = newState,
                                OrderID = order.OrderID,
                                OperUserID = user.SupplierUserID,
                                OperTime = DateTimeOffset.Now,
                                OperNickName = user.SupplierNickName,
                                Remark = "状态:" + strOldState + "→" + strNewState
                            }).InsertEntryAsync();
                        }
                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 400, ErrorMessage = ex, successed, failed });
            }
            return JsonConvert.SerializeObject(new { ErrorCode = 200, ErrorMessage = "OK", successed, failed });
        }

        //发送邮件
        [HttpPost]
        public async Task<string> SendMail(int OrderID, string title, string toMail, string customerName, string fileName, string filePath, string type = "supplier")
        {
            Order order;
            try
            {
                order = await client.For<Order>().Expand(s => s.ServiceItemHistorys).Key(OrderID).FindEntryAsync();
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
            //if (string.IsNullOrEmpty(customerName))
            //{
            //    return JsonConvert.SerializeObject(new { result = false, statusCode = 401, message = "客户名称不能为空！", info = "" });
            //}
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
            else
            {
                return JsonConvert.SerializeObject(new { result = false, statusCode = 401, message = "发送失败！类型错误", info = "" });
            }
            string userName = User.Identity.Name;
            SupplierUser user = await client.For<SupplierUser>().Filter(u => u.SupplierUserName == userName).FindEntryAsync();

            if (order.ServiceItemHistorys.SupplierID != user.SupplierID)
            {
                return JsonConvert.SerializeObject(new { result = false, statusCode = 401, message = "发送失败！订单异常", info = "" });
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

            await client.For<OrderHistory>().Set(new OrderHistory
            {
                OrderID = order.OrderID,
                OperUserID = user.SupplierUserID.ToString(),
                OperUserNickName = "(供应商)" + user.SupplierNickName,
                OperTime = DateTime.Now,
                State = order.state,
                Remark = "发送邮件" + message
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

        //查看操作日志
        public async Task<ActionResult> OrderOperation(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            string userName = User.Identity.Name;
            SupplierUser user = await client.For<SupplierUser>().Filter(u => u.SupplierUserName == userName).FindEntryAsync();
            var orderHistorys = await client.For<OrderSupplierHistory>()
                .OrderByDescending(o => o.OperTime)
                .Filter(o => o.OrderID == id)
                .Filter(o => o.order.ServiceItemHistorys.SupplierID == user.SupplierID)
                .FindEntriesAsync();
            return View(orderHistorys);
        }

        //获取客户详细信息
        public async Task<string> GetCustomerInfo(int? id)
        {
            if (id == null)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 400, ErrorMessage = "ID不能为空" });
            }
            var customer = await client.For<Customer>().Key(id).FindEntryAsync();
            var data = new
            {
                customer.BakTel,
                customer.Tel,
                customer.CustomerName,
                customer.CustomerEnname,
                customer.Email,
                customer.Wechat,
                customer.CustomerTBCode
            };
            return JsonConvert.SerializeObject(new { ErrorCode = 200, ErrorMessage = "OK", data });
        }

        //修改团号
        [HttpPost]
        public async Task<string> UpdateGroupNo(int? OrderID, string GroupNo, string ReceiveManTime, string Remark, int TrafficSurcharge, int CurrencyID, string CurrencyName)
        {
            if (OrderID == null)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "ID不能为空" });
            }
            if (CurrencyID == 0)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "CurrencyID不能为空" });
            }
            if (string.IsNullOrEmpty(CurrencyName))
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "CurrencyName不能为空" });
            }
            try
            {
                ServiceItemHistory item = await client.For<ServiceItemHistory>().Key(OrderID).FindEntryAsync();
                float OldTrafficSurcharge = item.TrafficSurcharge;
                string OldCurrencyName = item.TrafficCurrencyName;
                string OldGroupNo = item.GroupNo;
                string OldReceiveManTime = item.ReceiveManTime;

                Order order = await client.For<Order>().Expand(s => s.Customers).Key(OrderID).FindEntryAsync();
                string OldRemark = order.Remark;

                string newRemark = "";
                bool isSendMail = false;
                string PickupTime = "";
                string TransferFee = "";

                if ((OldGroupNo == null ? "" : OldGroupNo.Trim()) != (GroupNo == null ? "" : GroupNo.Trim()))
                {
                    newRemark += "团号：\"" + OldGroupNo + "\"→\"" + GroupNo + "\" ";
                }
                if ((OldReceiveManTime == null ? "" : OldReceiveManTime.Trim()) != (ReceiveManTime == null ? "" : ReceiveManTime.Trim()))
                {
                    newRemark += "接人时间：\"" + (OldReceiveManTime == null ? "" : OldReceiveManTime.Trim()) + "\"→\"" + (ReceiveManTime == null ? "" : ReceiveManTime.Trim()) + "\" ";
                    isSendMail = true;
                    PickupTime = "“" + (OldReceiveManTime == null ? "" : OldReceiveManTime.Trim().Replace("\"", "'")) + "”→“" + (ReceiveManTime == null ? "" : ReceiveManTime.Trim().Replace("\"", "'")) + "”";
                }
                else
                {
                    PickupTime = "“" + (OldReceiveManTime == null ? "" : OldReceiveManTime.Trim().Replace("\"", "'")) + "”";
                }
                if (OldTrafficSurcharge != TrafficSurcharge || (OldCurrencyName == null ? "" : OldCurrencyName.Trim()) != (CurrencyName == null ? "" : CurrencyName.Trim()))
                {
                    newRemark += "附加费：\"" + OldTrafficSurcharge.ToString().Trim() + (OldCurrencyName == null ? "" : OldCurrencyName.Trim()) + "\"→\"" + TrafficSurcharge.ToString().Trim() + (CurrencyName == null ? "" : CurrencyName.Trim()) + "\" ";
                    isSendMail = true;
                    TransferFee = "“" + OldTrafficSurcharge.ToString().Trim() + (OldCurrencyName == null ? "" : OldCurrencyName.Trim()) + "”→“" + TrafficSurcharge.ToString().Trim() + (CurrencyName == null ? "" : CurrencyName.Trim()) + "”";
                }
                else
                {
                    TransferFee = "“" + OldTrafficSurcharge.ToString().Trim() + (OldCurrencyName == null ? "" : OldCurrencyName.Trim()) + "”";
                }
                string historyRemark = newRemark;
                if ((OldRemark == null ? "" : OldRemark.Trim()) != (Remark == null ? "" : Remark.Trim()))
                {
                    historyRemark += "备注：\"" + (OldRemark == null ? "" : OldRemark.Trim()) + "\"→\"" + (Remark == null ? "" : Remark.Trim()) + "\" ";
                }
                //获取当前用户，创建操作记录orderHistorys
                string SupplierUserName = User.Identity.Name;
                SupplierUser user = await client.For<SupplierUser>()
                    .Expand(s => s.OneSupplier)
                    .Filter(s => s.SupplierUserName == SupplierUserName).FindEntryAsync();

                if (item.SupplierID != user.SupplierID)
                {
                    return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "操作异常" });
                }

                //数据有变化才保存
                if (newRemark != "")
                {
                    item.TrafficSurcharge = TrafficSurcharge;
                    item.TrafficCurrencyID = CurrencyID;
                    item.TrafficCurrencyName = CurrencyName;
                    item.GroupNo = GroupNo;
                    item.ReceiveManTime = ReceiveManTime;
                    await client.For<ServiceItemHistory>().Key(OrderID).Set(item).UpdateEntryAsync();

                    await client.For<OrderSupplierHistory>().Set(new OrderSupplierHistory
                    {
                        opera = OrderOperator.supplier,
                        State = order.state,
                        OrderID = order.OrderID,
                        OperUserID = user.SupplierUserID,
                        OperTime = DateTimeOffset.Now,
                        OperNickName = user.SupplierNickName,
                        Remark = newRemark
                    }).InsertEntryAsync();
                }
                if (historyRemark != "")
                {
                    order.Remark = Remark;
                    await client.For<Order>().Key(order.OrderID).Set(order).UpdateEntryAsync();
                    await client.For<OrderHistory>().Set(new OrderHistory
                    {
                        OrderID = order.OrderID,
                        OperUserID = user.SupplierUserID.ToString(),
                        OperUserNickName = "(供应商)" + user.SupplierNickName,
                        OperTime = DateTime.Now,
                        State = order.state,
                        Remark = historyRemark
                    }).InsertEntryAsync();
                }
                if (isSendMail && order.state == OrderState.SencondConfirm)
                {
                    string VoucherName = item.SupplierCode
                            + "-" + order.CustomerName
                            + "-" + order.Customers.CustomerTBCode
                            + "-" + item.cnItemName
                            + item.ServiceCode
                            + (item.TravelDate < DateTimeOffset.Parse("1901-01-01") ? "" : "-" + item.TravelDate.ToString("yyyyMMdd"));
                    var result = await EmailHelper.SupplierAmend(VoucherName, order.OrderNo, PickupTime, TransferFee);
                }
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 400, ErrorMessage = "操作异常：" + ex.Message });
            }
            return JsonConvert.SerializeObject(new { ErrorCode = 200, ErrorMessage = "OK" });
        }
        //修改备注
        [HttpPost]
        public async Task<string> UpdateRemark(int? OrderID, string Remark)
        {
            if (OrderID == null)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "ID不能为空" });
            }
            try
            {
                Order order = await client.For<Order>().Expand(s => s.ServiceItemHistorys).Key(OrderID).FindEntryAsync();
                string OldRemark = order.Remark;
                if (OldRemark != Remark)
                {
                    //获取当前用户，创建操作记录orderHistorys
                    string SupplierUserName = User.Identity.Name;
                    SupplierUser user = await client.For<SupplierUser>()
                        .Expand(s => s.OneSupplier)
                        .Filter(s => s.SupplierUserName == SupplierUserName).FindEntryAsync();

                    if (order.ServiceItemHistorys.SupplierID != user.SupplierID)
                    {
                        return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "操作异常" });
                    }
                    order.Remark = Remark;
                    await client.For<Order>().Key(order.OrderID).Set(order).UpdateEntryAsync();
                    await client.For<OrderHistory>().Set(new OrderHistory
                    {
                        OrderID = order.OrderID,
                        OperUserID = user.SupplierUserID.ToString(),
                        OperUserNickName = "(供应商)" + user.SupplierNickName,
                        OperTime = DateTime.Now,
                        State = order.state,
                        Remark = "留言由\"" + OldRemark + "\"改为\"" + Remark + "\""
                    }).InsertEntryAsync();
                }
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 400, ErrorMessage = "操作异常：" + ex.Message });
            }
            return JsonConvert.SerializeObject(new { ErrorCode = 200, ErrorMessage = "OK" });
        }
    }
}
