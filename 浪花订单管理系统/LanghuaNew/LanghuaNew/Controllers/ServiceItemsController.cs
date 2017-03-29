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
using Commond;
using System.IO;
using System.Text;
using System.Net.Http;
using LanghuaNew.Models;
using Microsoft.VisualBasic;
using Entity;

namespace LanghuaNew.Controllers
{
    public class ServiceItemsController : Controller
    {
        private ODataClient client = new ODataClient("http://localhost:250/odata/");

        // GET: ServiceItems
        public async Task<ActionResult> Index()
        {
            ViewBag.ServiceType = await client.For<ServiceType>().FindEntriesAsync();
            ViewBag.Supplier = await client.For<Supplier>().OrderBy(s => s.SupplierNo).Filter(s => s.SupplierEnableState == EnableState.Enable).FindEntriesAsync();
            return View();
        }
        // GET: ServiceItems/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ServiceItem item = await client.For<ServiceItem>()
                .Expand(s => s.ItemSuplier)
                .Expand(s => s.ExtraServices)
                .Expand(s => s.city)
                .Key(id).FindEntryAsync();
            if (item == null)
            {
                return HttpNotFound();
            }
            ViewBag.Supplier = await client.For<Supplier>().OrderBy(oo => oo.SupplierNo).FindEntriesAsync();
            ViewBag.ServiceType = await client.For<ServiceType>().FindEntriesAsync();
            ViewBag.Countries = new SelectList(await client.For<Country>().FindEntriesAsync(), "CountryID", "CountryName", item.city.CountryID);
            ViewBag.Cities = new SelectList(await client.For<City>().Filter(c => c.CountryID == item.city.CountryID).FindEntriesAsync(), "CityID", "CityName", item.CityID);
            return View(item);
        }
        // GET: ServiceItems/Create
        public async Task<ActionResult> Create()
        {
            ViewBag.Supplier = await client.For<Supplier>().Filter(s => s.SupplierEnableState == EnableState.Enable).OrderBy(oo => oo.SupplierNo).FindEntriesAsync();
            ViewBag.ServiceType = await client.For<ServiceType>().FindEntriesAsync();
            ViewBag.Countries = new SelectList(await client.For<Country>().FindEntriesAsync(), "CountryID", "CountryName");
            return View();
        }
        // GET: ServiceItems/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ServiceItem item = await client.For<ServiceItem>()
                .Expand(s => s.ItemSuplier)
                .Expand(s => s.ExtraServices)
                .Expand(s => s.city)
                .Key(id).FindEntryAsync();
            if (item == null)
            {
                return HttpNotFound();
            }
            ViewBag.Supplier = await client.For<Supplier>().OrderBy(oo => oo.SupplierNo).FindEntriesAsync();
            ViewBag.ServiceType = await client.For<ServiceType>().FindEntriesAsync();
            ViewBag.Countries = new SelectList(await client.For<Country>().FindEntriesAsync(), "CountryID", "CountryName", item.city.CountryID);
            ViewBag.Cities = new SelectList(await client.For<City>().Filter(c => c.CountryID == item.city.CountryID).FindEntriesAsync(), "CityID", "CityName", item.CityID);
            return View(item);
        }
        // GET: ServiceItems/PriceSetting/5
        public async Task<ActionResult> PriceSetting(int? ItemID, int? SupplierID)
        {
            if (ItemID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ServiceItem item = await client.For<ServiceItem>()
                .Expand(s => s.ItemSuplier)
                .Expand(s => s.ExtraServices)
                .Key(ItemID).FindEntryAsync();
            if (item == null)
            {
                return HttpNotFound();
            }
            if (SupplierID == null || SupplierID == 0)
            {
                if (item.DefaultSupplierID > 0)
                {
                    SupplierID = item.DefaultSupplierID;
                }
                else
                {
                    try
                    {
                        SupplierID = item.ItemSuplier[0].SupplierID;
                    }
                    catch
                    {
                        SupplierID = 0;
                    }
                }
            }
            ViewBag.SupplierID = SupplierID;
            ViewBag.Currency = await client.For<Currency>().Filter(c => c.CurrencyEnableState == EnableState.Enable).FindEntriesAsync();
            ViewBag.SupplierServiceItem = await client.For<SupplierServiceItem>()
                .Expand(s => s.ItemPriceBySuppliers)
                .Expand(s => s.ExtraServicePrices)
                .Filter(s => s.ServiceItemID == ItemID & s.SupplierID == SupplierID)
                .FindEntryAsync();
            ViewBag.SupplierServiceItemChange = await client.For<SupplierServiceItemChange>()
                .Expand(s => s.ItemPriceBySuppliers)
                .Expand(s => s.ExtraServicePrices)
                .Filter(s => s.ServiceItemID == ItemID & s.SupplierID == SupplierID)
                .FindEntryAsync();
            if (!item.ItemSuplier.Any(s => s.SupplierID == SupplierID))
            {
                ViewBag.inValid = true;
                ViewBag.message = "产品已移除该供应商，请添加该供应商后再维护价格";
                ViewBag.href = "/ServiceItems/Edit/" + ItemID;
            }
            return View(item);
        }
        // GET: ServiceItems/FormSetting/5
        public async Task<ActionResult> FormSetting(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ServiceItem item = await client.For<ServiceItem>().Key(id).FindEntryAsync();
            if (item == null)
            {
                return HttpNotFound();
            }
            ViewBag.Country = await client.For<Country>().Expand(c => c.Citys).FindEntriesAsync();
            ViewBag.FormField = await client.For<FormField>().FindEntriesAsync();
            return View(item);
        }
        //产品编码检查重复
        public async Task<string> IsExistServiceCode(int? ServiceItemID, string ServiceCode)
        {
            if (ServiceItemID == null)
            {
                ServiceItemID = 0;
            }
            ServiceItem item = await client.For<ServiceItem>()
                .Expand(s => s.ItemSuplier)
                .Filter(s => s.ServiceCode == ServiceCode && s.ServiceItemID != ServiceItemID)
                .Filter(s => s.ServiceItemEnableState == EnableState.Enable)
                .FindEntryAsync();
            if (item != null)
            {
                if (ServiceItemID > 0)
                    return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "已存在其他产品使用该编码", data = item.cnItemName + item.ServiceCode });
                else
                    return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "已存在产品使用该编码", data = item.cnItemName + item.ServiceCode });
            }
            return JsonConvert.SerializeObject(new { ErrorCode = 200, ErrorMessage = "OK" });
        }
        //保存产品信息
        [HttpPost]
        public async Task<string> SaveServiceItem(bool isAdd, ServiceItem serviceItem)
        {
            if (serviceItem == null)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "参数为空！" });
            }
            if (serviceItem.ItemSuplier == null)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "供应商不能为空！" });
            }
            if (string.IsNullOrEmpty(serviceItem.ServiceCode))
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "产品编码不能为空！" });
            }
            if (string.IsNullOrEmpty(serviceItem.cnItemName))
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "产品中文名不能为空！" });
            }
            //if (string.IsNullOrEmpty(serviceItem.enItemName))
            //{
            //    return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "产品英文名不能为空！" });
            //}
            if (serviceItem.ServiceItemID == 0 && !isAdd)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "参数有误！" });
            }
            if (serviceItem.CityID == 0)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "目的地不能为空！" });
            }
            ServiceItem item = await client.For<ServiceItem>()
                .Filter(s => s.ServiceCode == serviceItem.ServiceCode.Trim() && s.ServiceItemID != serviceItem.ServiceItemID)
                .Filter(s => s.ServiceItemEnableState == EnableState.Enable)
                .FindEntryAsync();
            if (item != null)
            {
                if (serviceItem.ServiceItemID > 0)
                    return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "已存在其他产品使用该编码" });
                else
                    return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "已存在产品使用该编码" });
            }
            if (serviceItem.FixedDays > 0)
            {
                serviceItem.IsFixedDay = true;
            }
            else
            {
                serviceItem.IsFixedDay = false;
            }
            string id = "";
            if (isAdd)
            {
                serviceItem.CreateTime = DateTime.Now;
                HttpResponseMessage Message = await HttpHelper.PostAction("ServiceItemsExtend", JsonConvert.SerializeObject(serviceItem));
                id = Message.Content.ReadAsStringAsync().Result;
            }
            else
            {
                id = serviceItem.ServiceItemID.ToString();
                await HttpHelper.PutAction("ServiceItemsExtend", JsonConvert.SerializeObject(serviceItem));
                string userName = User.Identity.Name;
                User user = await client.For<User>().Filter(u => u.UserName == userName).FindEntryAsync();
                await client.For<SystemLog>().Set(new
                {
                    Operate = "修改产品",
                    OperateTime = DateTime.Now,
                    UserID = user.UserID,
                    UserName = user.NickName,
                    Remark = "Message：" + JsonConvert.SerializeObject(serviceItem)
                }).InsertEntryAsync();
            }
            return JsonConvert.SerializeObject(new { ErrorCode = 200, ErrorMessage = "OK", id });
        }
        //获取产品列表
        public async Task<string> GetItems(SearchModel search)
        {
            int draw = 1;
            int start = 0;
            int length = 50;
            string propertyName = "ServiceCode";
            int sort = 0;

            IBoundClient<ServiceItem> ItemResult = client.For<ServiceItem>().Expand(u => u.ItemServiceType).Expand(u => u.city);
            IBoundClient<ServiceItem> ItemCountResult = client.For<ServiceItem>();

            if (search.length > 0)
            {
                draw = search.draw;
                start = search.start;
                length = search.length;
                if (search.OrderBy != null)
                {
                    propertyName = string.IsNullOrEmpty(search.OrderBy.PropertyName) ? propertyName : search.OrderBy.PropertyName;
                    sort = search.OrderBy.OrderBy;
                }

                if (search.ItemSearch != null)
                {
                    if (search.ItemSearch.SupplierID > 0)
                    {
                        ItemResult = ItemResult.Filter(t => t.ItemSuplier.Any(i => i.SupplierID == search.ItemSearch.SupplierID));
                        ItemCountResult = ItemCountResult.Filter(t => t.ItemSuplier.Any(i => i.SupplierID == search.ItemSearch.SupplierID));
                    }
                    //启用状态 0：全部 1：启用 2：禁用
                    int status = search.ItemSearch.status;
                    if (status > 0)
                    {
                        status -= 1;
                        ItemResult = ItemResult.Filter(t => t.ServiceItemEnableState == (EnableState)status);
                        ItemCountResult = ItemCountResult.Filter(t => t.ServiceItemEnableState == (EnableState)status);
                    }
                    if (!string.IsNullOrEmpty(search.ItemSearch.FuzzySearch))
                    {
                        ItemResult = ItemResult.Filter(t => t.ServiceCode.Contains(search.ItemSearch.FuzzySearch) || t.cnItemName.Contains(search.ItemSearch.FuzzySearch) || t.enItemName.Contains(search.ItemSearch.FuzzySearch));
                        ItemCountResult = ItemCountResult.Filter(t => t.ServiceCode.Contains(search.ItemSearch.FuzzySearch) || t.cnItemName.Contains(search.ItemSearch.FuzzySearch) || t.enItemName.Contains(search.ItemSearch.FuzzySearch));
                    }
                    if (search.ItemSearch.ServiceTypeID > 0)
                    {
                        ItemResult = ItemResult.Filter(t => t.ServiceTypeID == search.ItemSearch.ServiceTypeID);
                        ItemCountResult = ItemCountResult.Filter(t => t.ServiceTypeID == search.ItemSearch.ServiceTypeID);
                    }
                }
            }
            List<ServiceItemViewModel> list = new List<ServiceItemViewModel>();
            int count = 0;

            count = await ItemCountResult.Count().FindScalarAsync<int>();
            ItemResult = sort == 0 ? ItemResult.OrderByDescending(propertyName) : ItemResult.OrderBy(propertyName);
            var serviceItems = await ItemResult.Skip(start).Top(length).FindEntriesAsync();

            foreach (var item in serviceItems)
            {
                ServiceItemViewModel view = new ServiceItemViewModel();
                view.ServiceItemID = item.ServiceItemID;
                view.enItemName = item.enItemName == null ? "" : item.enItemName;
                view.ServiceCode = item.ServiceCode == null ? "" : item.ServiceCode;
                view.cnItemName = item.cnItemName == null ? "" : item.cnItemName;
                view.ServiceItemEnableState = item.ServiceItemEnableState;
                view.InsuranceDays = item.InsuranceDays;
                view.CityName = item.city == null ? "" : item.city.CityName;
                ServiceType type = await client.For<ServiceType>().Key(item.ServiceTypeID).Select(s => s.ServiceTypeName).FindEntryAsync();
                view.ServiceTypeName = type.ServiceTypeName;
                Supplier supplier = null;
                try
                {
                    supplier = await client.For<Supplier>().Filter(s => s.SupplierID == item.DefaultSupplierID).FindEntryAsync();
                }
                catch { }
                view.DefaultSupplier = supplier == null ? "" : (supplier.SupplierNo + "-" + supplier.SupplierName);

                view.isExistElement = !string.IsNullOrEmpty(item.ElementContent);

                SupplierServiceItem supplierServiceItem = await client.For<SupplierServiceItem>()
                    .Expand(s => s.ItemPriceBySuppliers)
                    .Filter(s => s.SupplierID == item.DefaultSupplierID && s.ServiceItemID == item.ServiceItemID)
                    .FindEntryAsync();
                if (supplierServiceItem == null || supplierServiceItem.ItemPriceBySuppliers == null)
                {
                    view.DefaultSupplierPriceState = SupplierPriceState.NotSeted;
                    view.DefaultSupplierPriceStateName = EnumHelper.GetEnumDescription(SupplierPriceState.NotSeted);
                }
                else
                {
                    if (supplierServiceItem.ItemPriceBySuppliers.Any(i => i.startTime < DateTime.Now && i.EndTime > DateTime.Now.AddDays(-1)))
                    {
                        view.DefaultSupplierPriceState = SupplierPriceState.Seted;
                        view.DefaultSupplierPriceStateName = EnumHelper.GetEnumDescription(SupplierPriceState.Seted);
                    }
                    else
                    {
                        view.DefaultSupplierPriceState = SupplierPriceState.Expired;
                        view.DefaultSupplierPriceStateName = EnumHelper.GetEnumDescription(SupplierPriceState.Expired);
                    }
                }
                list.Add(view);
            }
            ItemsList o = new ItemsList();
            o.draw = draw;
            o.recordsFiltered = count;
            o.data = list;
            o.SearchModel = search;
            return JsonConvert.SerializeObject(o);
        }
        //获取产品价格
        public async Task<string> GetPrice(int ItemID, int SupplierID)
        {
            var result = await client.For<ServiceItem>().Expand(s => s.ExtraServices).Key(ItemID).FindEntryAsync();
            var supplier = await client.For<Supplier>().Key(SupplierID).FindEntryAsync();

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
                .Expand(i => i.Service)
                .Expand(i => i.ItemSupplier)
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
            return JsonConvert.SerializeObject(new { ErrorCode = 200, ErrorMessage = "OK", data = new { item, changeitem, result.ExtraServices, baseinfo = new { result.cnItemName, result.ServiceCode, supplier.SupplierNo } } });
        }
        //保存价格信息
        [HttpPost]
        public async Task<string> SavePriceSetting(SupplierServiceItem item)
        {
            if (item.ServiceItemID == 0)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "产品不能为空！" });
            }
            if (item.SupplierID == 0)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "供应商不能为空！" });
            }
            if (item.CurrencyID == 0)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "币别不能为空！" });
            }
            if (item.ItemPriceBySuppliers == null)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "产品基础价格不能为空！" });
            }
            foreach (var ItemPriceBySupplier in item.ItemPriceBySuppliers)
            {
                if (ItemPriceBySupplier.startTime > ItemPriceBySupplier.EndTime)
                {
                    return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "开始时间不能大于结束时间！" });
                }
                var ssprice = item.ItemPriceBySuppliers.ToList();
                ssprice.Remove(ItemPriceBySupplier);
                var ssone = ssprice.Where(s => (s.startTime <= ItemPriceBySupplier.startTime && ItemPriceBySupplier.startTime <= s.EndTime)
                            || (s.startTime <= ItemPriceBySupplier.EndTime && ItemPriceBySupplier.EndTime <= s.EndTime)
                            || (ItemPriceBySupplier.EndTime <= s.startTime && s.startTime <= ItemPriceBySupplier.EndTime)
                            || (ItemPriceBySupplier.EndTime <= s.EndTime && s.EndTime <= ItemPriceBySupplier.EndTime)
                ).FirstOrDefault();
                if (ssone != null)
                {
                    return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "时间跨度不能重叠！" });
                }
            }

            string userName = User.Identity.Name;
            User user = await client.For<User>().Filter(u => u.UserName == userName).FindEntryAsync();
            Supplier supplier = await client.For<Supplier>().Key(item.SupplierID).FindEntryAsync();
            SupplierServiceItem oldItem = await client.For<SupplierServiceItem>()
                .Expand("ExtraServicePrices/Service").Expand(s => s.ItemPriceBySuppliers).Expand(s => s.ItemCurrency)
                .Filter(u => u.ServiceItemID == item.ServiceItemID && u.SupplierID == item.SupplierID).FindEntryAsync();
            SupplierServiceItemChange oldChange = await client.For<SupplierServiceItemChange>()
                .Expand(s => s.ExtraServicePrices).Expand(s => s.ItemPriceBySuppliers)
                .Filter(u => u.ServiceItemID == item.ServiceItemID && u.SupplierID == item.SupplierID).FindEntryAsync();
            string Remark = "";
            //首次输入价格或修改首次输入未确认的价格
            if (oldItem == null || (oldItem.IsChange && oldChange == null))
            {
                var currency = await client.For<Currency>().Key(item.CurrencyID).FindEntryAsync();
                Remark += "<div>币别：" + currency.CurrencyName + "</div>";
                Remark += "<div>计费标准：" + EnumHelper.GetEnumDescription(item.PayType) + "</div>";
                Remark += "<div>生效方式：" + EnumHelper.GetEnumDescription(item.SelectEffectiveWay) + "</div>";
                Remark += "<div>价格说明：\"" + item.Remark + "\"</div>";
                if (item.ItemPriceBySuppliers != null)
                {
                    foreach (var one in item.ItemPriceBySuppliers)
                    {
                        string priceRemark = item.PayType == PricingMethod.ByPerson
                            ? "'" + one.startTime.ToString("yyyy-MM-dd") + "~" + one.EndTime.ToString("yyyy-MM-dd") + " 成人" + one.AdultNetPrice + "儿童" + one.ChildNetPrice + "婴儿" + one.BobyNetPrice + "'"
                            : "'" + one.startTime.ToString("yyyy-MM-dd") + "~" + one.EndTime.ToString("yyyy-MM-dd") + " 单价" + one.Price + "'";
                        Remark += "<div>基础价格新增：" + priceRemark + "</div>";
                    }
                }
                if (item.ExtraServicePrices != null)
                {
                    foreach (var one in item.ExtraServicePrices)
                    {
                        var extraOne = await client.For<ExtraService>().Key(one.ExtraServiceID).FindEntryAsync();
                        Remark += "<div>" + extraOne.ServiceName + "：" + one.ServicePrice + currency.CurrencyNo + "</div>";
                    }
                }
                if (oldItem != null)
                {//修改首次输入未确认的价格
                    bool bl = true;
                    if (bl && oldItem.CurrencyID != item.CurrencyID)
                    {
                        bl = false;
                    }
                    if (bl && oldItem.PayType != item.PayType)
                    {
                        bl = false;
                    }
                    if (bl && oldItem.SelectEffectiveWay != item.SelectEffectiveWay)
                    {
                        bl = false;
                    }
                    if (bl && oldItem.Remark != item.Remark)
                    {
                        bl = false;
                    }
                    if (bl && item.ItemPriceBySuppliers != null)
                    {
                        foreach (var one in item.ItemPriceBySuppliers)
                        {
                            ItemPriceBySupplier oldOne = oldItem.ItemPriceBySuppliers == null ? null : oldItem.ItemPriceBySuppliers.Where(s => s.ItemPriceBySupplierID == one.ItemPriceBySupplierID).FirstOrDefault();
                            string priceRemark = item.PayType == PricingMethod.ByPerson
                                ? "'" + one.startTime.ToString("yyyy-MM-dd") + "-" + one.EndTime.ToString("yyyy-MM-dd") + " 成人" + one.AdultNetPrice + "儿童" + one.ChildNetPrice + "婴儿" + one.BobyNetPrice + "'"
                                : "'" + one.startTime.ToString("yyyy-MM-dd") + "-" + one.EndTime.ToString("yyyy-MM-dd") + " 单价" + one.Price + "'";
                            if (oldOne != null)
                            {
                                string oldPriceRemark = item.PayType == PricingMethod.ByPerson
                                    ? "'" + oldOne.startTime.ToString("yyyy-MM-dd") + "-" + oldOne.EndTime.ToString("yyyy-MM-dd") + " 成人" + oldOne.AdultNetPrice + "儿童" + oldOne.ChildNetPrice + "婴儿" + oldOne.BobyNetPrice + "'"
                                    : "'" + oldOne.startTime.ToString("yyyy-MM-dd") + "-" + oldOne.EndTime.ToString("yyyy-MM-dd") + " 单价" + oldOne.Price + "'";
                                if (priceRemark != oldPriceRemark)
                                {
                                    bl = false;
                                    break;
                                }
                            }
                            else
                            {
                                bl = false;
                                break;
                            }
                        }
                    }
                    if (bl && item.ExtraServicePrices != null)
                    {
                        foreach (var one in item.ExtraServicePrices)
                        {
                            ExtraServicePrice oldOne = oldItem.ExtraServicePrices == null ? null : oldItem.ExtraServicePrices.Where(s => s.ExtraServiceID == one.ExtraServiceID).FirstOrDefault();
                            if (oldOne != null)
                            {
                                if (oldOne.ServicePrice != one.ServicePrice)
                                {
                                    bl = false;
                                    break;
                                }
                            }
                            else
                            {
                                bl = false;
                                break;
                            }
                        }
                    }
                    if (bl)
                    {
                        return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "价格好像没变哦，先检查一下吧" });
                    }
                }
                HttpResponseMessage Message = await HttpHelper.PostAction("SupplierServiceItemsExtend", JsonConvert.SerializeObject(item));
                string Err = Message.Content.ReadAsStringAsync().Result;
                try
                {
                    int.Parse(Err);
                }
                catch
                {
                    return JsonConvert.SerializeObject(new { ErrorCode = 400, ErrorMessage = Err });
                }
                await client.For<ItemPriceLog>().Set(new ItemPriceLog
                {
                    Operate = "新增产品价格",
                    OperateTime = DateTimeOffset.Now,
                    UserID = user.UserID,
                    UserName = user.NickName,
                    Operator = OrderOperator.inside,
                    SupplierServiceItemID = int.Parse(Err),
                    Remark = Remark
                }).InsertEntryAsync();
            }
            else//其他情况：
            {
                item.SupplierServiceItemID = oldItem.SupplierServiceItemID;
                var currency = await client.For<Currency>().Key(item.CurrencyID).FindEntryAsync();
                if (oldItem.CurrencyID != item.CurrencyID)
                {
                    Remark += "<div>币别：" + oldItem.ItemCurrency.CurrencyName + "→" + currency.CurrencyName + "</div>";
                }
                if (oldItem.PayType != item.PayType)
                {
                    Remark += "<div>计费标准：" + EnumHelper.GetEnumDescription(oldItem.PayType) + "→" + EnumHelper.GetEnumDescription(item.PayType) + "</div>";
                }
                if (oldItem.SelectEffectiveWay != item.SelectEffectiveWay)
                {
                    Remark += "<div>生效方式：" + EnumHelper.GetEnumDescription(oldItem.SelectEffectiveWay) + "→" + EnumHelper.GetEnumDescription(item.SelectEffectiveWay) + "</div>";
                }
                if (oldItem.Remark != item.Remark)
                {
                    Remark += "<div>价格说明：'" + oldItem.Remark + "'→'" + item.Remark + "'</div>";
                }
                if (item.ItemPriceBySuppliers != null)
                {
                    foreach (var one in item.ItemPriceBySuppliers)
                    {
                        var oldOne = oldItem.ItemPriceBySuppliers == null ? null : oldItem.ItemPriceBySuppliers.Where(s => s.ItemPriceBySupplierID == one.ItemPriceBySupplierID).FirstOrDefault();
                        string priceRemark = item.PayType == PricingMethod.ByPerson
                            ? "'" + one.startTime.ToString("yyyy-MM-dd") + "~" + one.EndTime.ToString("yyyy-MM-dd") + " 成人" + one.AdultNetPrice + "儿童" + one.ChildNetPrice + "婴儿" + one.BobyNetPrice + "'"
                            : "'" + one.startTime.ToString("yyyy-MM-dd") + "~" + one.EndTime.ToString("yyyy-MM-dd") + " 单价" + one.Price + "'";
                        if (oldOne != null)
                        {
                            string oldPriceRemark = oldItem.PayType == PricingMethod.ByPerson
                                ? "'" + oldOne.startTime.ToString("yyyy-MM-dd") + "~" + oldOne.EndTime.ToString("yyyy-MM-dd") + " 成人" + oldOne.AdultNetPrice + "儿童" + oldOne.ChildNetPrice + "婴儿" + oldOne.BobyNetPrice + "'"
                                : "'" + oldOne.startTime.ToString("yyyy-MM-dd") + "~" + oldOne.EndTime.ToString("yyyy-MM-dd") + " 单价" + oldOne.Price + "'";
                            if (priceRemark != oldPriceRemark)
                            {
                                Remark += "<div>" + oldPriceRemark + "→" + priceRemark + "</div>";
                            }
                        }
                        else
                        {
                            Remark += "<div>基础价格新增：" + priceRemark + "</div>";
                        }
                    }
                }
                if (item.ExtraServicePrices != null)
                {
                    foreach (var one in item.ExtraServicePrices)
                    {
                        var extraOne = await client.For<ExtraService>().Key(one.ExtraServiceID).FindEntryAsync();
                        var oldOne = oldItem.ExtraServicePrices == null ? null : oldItem.ExtraServicePrices.Where(s => s.ExtraServicePriceID == one.ExtraServicePriceID).FirstOrDefault();
                        if (oldOne != null)
                        {
                            if (oldOne.ServicePrice != one.ServicePrice)
                            {
                                Remark += "<div>" + extraOne.ServiceName + "：" + oldOne.ServicePrice + "→" + one.ServicePrice + "</div>";
                            }
                        }
                        else
                        {
                            Remark += "<div>" + extraOne.ServiceName + "：" + one.ServicePrice + currency.CurrencyNo + "</div>";
                        }
                    }
                }
                if (!supplier.EnableOnline)//线下供应商
                {
                    if (Remark != "")
                    {
                        HttpResponseMessage Message = await HttpHelper.PostAction("SupplierServiceItemsExtend", JsonConvert.SerializeObject(item));
                        string Err = Message.Content.ReadAsStringAsync().Result;
                        if (Err == "OK")
                        {
                            await client.For<ItemPriceLog>().Set(new ItemPriceLog
                            {
                                Operate = "修改产品价格",
                                OperateTime = DateTimeOffset.Now,
                                UserID = user.UserID,
                                UserName = user.NickName,
                                Operator = OrderOperator.inside,
                                SupplierServiceItemID = oldItem.SupplierServiceItemID,
                                Remark = Remark
                            }).InsertEntryAsync();
                        }
                        else
                        {
                            return JsonConvert.SerializeObject(new { ErrorCode = 400, ErrorMessage = Err });
                        }
                    }
                    else
                    {
                        return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "价格好像没变哦，先检查一下吧" });
                    }
                }
                else//线上供应商
                {
                    if (oldChange != null)
                    {
                        bool bl = true;
                        if (bl && oldChange.CurrencyID != item.CurrencyID)
                        {
                            bl = false;
                        }
                        if (bl && oldChange.PayType != item.PayType)
                        {
                            bl = false;
                        }
                        if (bl && oldChange.SelectEffectiveWay != item.SelectEffectiveWay)
                        {
                            bl = false;
                        }
                        if (bl && oldChange.Remark != item.Remark)
                        {
                            bl = false;
                        }
                        if (bl && item.ItemPriceBySuppliers != null)
                        {
                            foreach (var one in item.ItemPriceBySuppliers)
                            {
                                ItemPriceBySupplierChange oldOne = oldChange.ItemPriceBySuppliers == null ? null : oldChange.ItemPriceBySuppliers.Where(s => s.ItemPriceBySupplierID == one.ItemPriceBySupplierID).FirstOrDefault();
                                string priceRemark = item.PayType == PricingMethod.ByPerson
                                    ? "'" + one.startTime.ToString("yyyy-MM-dd") + "-" + one.EndTime.ToString("yyyy-MM-dd") + " 成人" + one.AdultNetPrice + "儿童" + one.ChildNetPrice + "婴儿" + one.BobyNetPrice + "'"
                                    : "'" + one.startTime.ToString("yyyy-MM-dd") + "-" + one.EndTime.ToString("yyyy-MM-dd") + " 单价" + one.Price + "'";
                                if (oldOne != null)
                                {
                                    string oldPriceRemark = item.PayType == PricingMethod.ByPerson
                                        ? "'" + oldOne.startTime.ToString("yyyy-MM-dd") + "-" + oldOne.EndTime.ToString("yyyy-MM-dd") + " 成人" + oldOne.AdultNetPrice + "儿童" + oldOne.ChildNetPrice + "婴儿" + oldOne.BobyNetPrice + "'"
                                        : "'" + oldOne.startTime.ToString("yyyy-MM-dd") + "-" + oldOne.EndTime.ToString("yyyy-MM-dd") + " 单价" + oldOne.Price + "'";
                                    if (priceRemark != oldPriceRemark)
                                    {
                                        bl = false;
                                        break;
                                    }
                                }
                                else
                                {
                                    bl = false;
                                    break;
                                }
                            }
                        }
                        if (bl && item.ExtraServicePrices != null)
                        {
                            foreach (var one in item.ExtraServicePrices)
                            {
                                ExtraServicePriceChange oldOne = oldChange.ExtraServicePrices == null ? null : oldChange.ExtraServicePrices.Where(s => s.ExtraServiceID == one.ExtraServiceID).FirstOrDefault();
                                if (oldOne != null)
                                {
                                    if (oldOne.ServicePrice != one.ServicePrice)
                                    {
                                        bl = false;
                                        break;
                                    }
                                }
                                else
                                {
                                    bl = false;
                                    break;
                                }
                            }
                        }
                        if (bl)
                        {
                            return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "价格好像没变哦，先检查一下吧" });
                        }
                    }
                    else
                    {
                        if (Remark == "")
                        {
                            return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "价格好像没变化哦，先检查一下吧" });
                        }
                    }
                    HttpResponseMessage Message = await HttpHelper.PostAction("SupplierServiceItemsExtend", JsonConvert.SerializeObject(item));
                    string Err = Message.Content.ReadAsStringAsync().Result;
                    if (Err == "OK")
                    {
                        await client.For<ItemPriceLog>().Set(new ItemPriceLog
                        {
                            Operate = "变更产品价格",
                            OperateTime = DateTimeOffset.Now,
                            UserID = user.UserID,
                            UserName = user.NickName,
                            Operator = OrderOperator.inside,
                            SupplierServiceItemID = oldItem.SupplierServiceItemID,
                            Remark = Remark
                        }).InsertEntryAsync();
                    }
                    else
                    {
                        return JsonConvert.SerializeObject(new { ErrorCode = 400, ErrorMessage = Err });
                    }
                }
            }
            if (supplier.EnableOnline && !string.IsNullOrEmpty(supplier.EMail))
            {
                ServiceItem serviceItem = await client.For<ServiceItem>().Key(item.ServiceItemID).FindEntryAsync();
                if (serviceItem.ServiceItemEnableState == EnableState.Enable)
                {
                    string title = "【待确认】" + serviceItem.cnItemName + "价格变更";
                    var result = await EmailHelper.PriceWaitConfirm(title, supplier.EMail, serviceItem.cnItemName, Remark.Replace("\"", "'"));
                }
            }
            return JsonConvert.SerializeObject(new { ErrorCode = 200, ErrorMessage = "OK" });


            //HttpResponseMessage Message = await HttpHelper.PostAction("SupplierServiceItemsExtend", JsonConvert.SerializeObject(item));
            //string Err = Message.Content.ReadAsStringAsync().Result;
            //if (Err == "OK")
            //{
            //    string userName = User.Identity.Name;
            //    User user = await client.For<User>().Filter(u => u.UserName == userName).FindEntryAsync();
            //    await client.For<SystemLog>().Set(new
            //    {
            //        Operate = "修改产品价格",
            //        OperateTime = DateTime.Now,
            //        UserID = user.UserID,
            //        UserName = user.NickName,
            //        Remark = "Message：" + JsonConvert.SerializeObject(item)
            //    }).InsertEntryAsync();
            //    return JsonConvert.SerializeObject(new { ErrorCode = 200, ErrorMessage = "OK" });
            //}
            //else
            //{
            //    return JsonConvert.SerializeObject(new { ErrorCode = 400, ErrorMessage = Err });
            //}
        }
        //保存表单信息
        [HttpPost]
        public async Task<string> SaveFromSetting(int? ItemID, int? TemplteID, string ElementContent)
        {
            if (ItemID == null || ItemID == 0)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "产品不能为空！" });
            }
            try
            {
                ServiceItem oldItem = await client.For<ServiceItem>().Key(ItemID).FindEntryAsync();
                oldItem.ServiceItemTemplteID = TemplteID;
                oldItem.ElementContent = ElementContent;
                await client.For<ServiceItem>().Key(ItemID).Set(oldItem).UpdateEntryAsync();
                string userName = User.Identity.Name;
                User user = await client.For<User>().Filter(u => u.UserName == userName).FindEntryAsync();
                await client.For<SystemLog>().Set(new
                {
                    Operate = "修改产品表单",
                    OperateTime = DateTime.Now,
                    UserID = user.UserID,
                    UserName = user.NickName,
                    Remark = "ItemID：" + ItemID + "TemplteID：" + ItemID + "ElementContent：" + ElementContent
                }).InsertEntryAsync();
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 400, ErrorMessage = "出错啦！错误原因：" + ex.Message });
            }
            return JsonConvert.SerializeObject(new { ErrorCode = 200, ErrorMessage = "OK" });
        }
        //克隆表单信息
        [HttpPost]
        public async Task<string> CloneFromSetting(int? ItemID, int? CloneItemID)
        {
            if (ItemID == null || ItemID == 0)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "产品不能为空！" });
            }
            if (CloneItemID == null || CloneItemID == 0)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "克隆产品不能为空！" });
            }
            try
            {
                ServiceItem CloneItem = await client.For<ServiceItem>().Key(CloneItemID).FindEntryAsync();
                if (string.IsNullOrEmpty(CloneItem.ElementContent))
                {
                    return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "要克隆的产品并没有维护表单信息，请选择其他产品克隆！" });
                }
                ServiceItem Item = await client.For<ServiceItem>().Key(ItemID).FindEntryAsync();
                if (Item.ServiceTypeID != CloneItem.ServiceTypeID)
                {
                    return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "不同类型的产品不能进行克隆！" });
                }

                Item.ServiceItemTemplteID = CloneItem.ServiceItemTemplteID;
                Item.ElementContent = CloneItem.ElementContent;
                await client.For<ServiceItem>().Key(ItemID).Set(Item).UpdateEntryAsync();
                string userName = User.Identity.Name;
                User user = await client.For<User>().Filter(u => u.UserName == userName).FindEntryAsync();
                await client.For<SystemLog>().Set(new
                {
                    Operate = "克隆产品表单",
                    OperateTime = DateTime.Now,
                    UserID = user.UserID,
                    UserName = user.NickName,
                    Remark = "ItemID：" + ItemID + "CloneItemID：" + CloneItemID
                }).InsertEntryAsync();
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 400, ErrorMessage = "出错啦！错误原因：" + ex.Message });
            }
            return JsonConvert.SerializeObject(new { ErrorCode = 200, ErrorMessage = "OK" });
        }
        //启用禁用
        [HttpPost]
        public async Task<string> UpdateDisable(string ItemID, string Operation)
        {
            var failed = (new int[] { 1 }).Select(x => new { name = "", reason = "" }).ToList();
            failed.Clear();
            if (string.IsNullOrEmpty(ItemID))
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "产品不能为空！" });
            }
            if (string.IsNullOrEmpty(Operation))
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "操作异常！" });
            }
            try
            {
                var id = ItemID.Split(',');
                if (Operation.Trim() == "0")
                {
                    foreach (var i in id)
                    {
                        var oldItem = await client.For<ServiceItem>().Key(int.Parse(i)).FindEntryAsync();
                        //启用时需要检查编码有没有重复
                        string code = oldItem.ServiceCode;
                        ServiceItem item = await client.For<ServiceItem>()
                        .Filter(s => s.ServiceCode == code && s.ServiceItemID != oldItem.ServiceItemID)
                        .Filter(s => s.ServiceItemEnableState == EnableState.Enable)
                        .FindEntryAsync();
                        if (item != null)
                        {
                            //编码重复
                            failed.Add(new { name = oldItem.cnItemName + oldItem.ServiceCode, reason = "已存在产品(" + item.cnItemName + item.ServiceCode + ")使用该编码" });
                        }
                        else
                        {
                            int defaultsupplierid = oldItem.DefaultSupplierID;
                            var supplier = await client.For<Supplier>().Filter(s => s.SupplierID == defaultsupplierid && s.SupplierEnableState == EnableState.Enable).FindEntryAsync();
                            if (supplier == null)
                            {
                                //默认供应商被禁用了
                                failed.Add(new { name = oldItem.cnItemName + oldItem.ServiceCode, reason = "默认供应商已被禁用，请启用该供应商或移除该供应商后再启用产品" });
                            }
                            else
                            {
                                oldItem.ServiceItemEnableState = (EnableState)int.Parse(Operation);
                                await client.For<ServiceItem>().Key(int.Parse(i)).Set(oldItem).UpdateEntryAsync();
                            }
                        }
                    }
                }
                else if (Operation.Trim() == "1")
                {
                    foreach (var i in id)
                    {
                        var oldItem = await client.For<ServiceItem>().Key(int.Parse(i)).FindEntryAsync();
                        oldItem.ServiceItemEnableState = (EnableState)int.Parse(Operation);
                        await client.For<ServiceItem>().Key(int.Parse(i)).Set(oldItem).UpdateEntryAsync();
                    }
                }
                //else if (Operation.Trim().ToLower() == "delete")
                //{
                //    foreach (var i in id)
                //    {
                //        await client.For<ServiceItem>().Key(int.Parse(i)).DeleteEntryAsync();
                //    }
                //}
                else
                {
                    return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "不允许进行当前操作！" });
                }
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 400, ErrorMessage = "出错啦！出错原因：" + ex.Message });
            }
            return JsonConvert.SerializeObject(new { ErrorCode = 200, ErrorMessage = "OK", failed });
        }
        //上传模板
        [HttpPost]
        public async Task<string> UploadFile(int? ItemID)
        {
            if (ItemID == null)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "产品不能为空！" });
            }
            HttpPostedFileBase file = Request.Files["file"];
            if (file != null)
            {
                string folderPath = "~/data/ServiceItemData/" + DateTime.Now.ToString("yyyyMM") + "/";
                string savePath = Server.MapPath(folderPath);
                if (!Directory.Exists(savePath))//判断上传文件夹是否存在，若不存在，则创建
                {
                    Directory.CreateDirectory(savePath);//创建文件夹
                }
                string filePath = savePath + DateTime.Now.ToString("yyyyMMddHHmmss_fff") + Path.GetFileName(file.FileName);
                file.SaveAs(filePath);
                //string html = System.IO.File.ReadAllText(filePath, Encoding.Default);
                byte[] byData = System.IO.File.ReadAllBytes(filePath);
                //byte[] byData = new byte[100000];
                //FileStream fileStream = new FileStream(filePath, FileMode.Open);
                //fileStream.Seek(0, SeekOrigin.Begin);
                //fileStream.Read(byData, 0, 100000); //byData传进来的字节数组,用以接受FileStream对象中的数据,第2个参数是字节数组中开始写入数据的位置,它通常是0,表示从数组的开端文件中向数组写数据,最后一个参数规定从文件读多少字符.
                //fileStream.Close();

                ServiceItemTemplte templte = new ServiceItemTemplte();
                templte.ServiceItemTemplteHtml = Encoding.UTF8.GetString(byData).Trim();
                templte = await client.For<ServiceItemTemplte>().Set(templte).InsertEntryAsync();

                ServiceItem item = await client.For<ServiceItem>().Key(ItemID).FindEntryAsync();
                int? ServiceItemTemplteID = item.ServiceItemTemplteID;
                item.ServiceItemTemplteID = templte.ServiceItemTemplteID;
                await client.For<ServiceItem>().Key(ItemID).Set(item).UpdateEntryAsync();

                string userName = User.Identity.Name;
                User user = await client.For<User>().Filter(u => u.UserName == userName).FindEntryAsync();
                await client.For<SystemLog>().Set(new
                {
                    Operate = "上传产品模板",
                    OperateTime = DateTime.Now,
                    UserID = user.UserID,
                    UserName = user.NickName,
                    Remark = "ItemID：" + ItemID + "原ServiceItemTemplteID：" + ServiceItemTemplteID + "ServiceItemTemplteID：" + templte.ServiceItemTemplteID
                }).InsertEntryAsync();

                return JsonConvert.SerializeObject(new { ErrorCode = 200, ErrorMessage = "OK", templte.ServiceItemTemplteID });
            }
            else
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "参数不能为空！" });
            }

        }
        //下载模板
        public async Task<FileResult> DownFile(int? ServiceItemTemplteID, string fileName)
        {
            if (ServiceItemTemplteID == null || ServiceItemTemplteID == 0)
            {
                //
            }
            ServiceItemTemplte templte = await client.For<ServiceItemTemplte>().Key(ServiceItemTemplteID).FindEntryAsync();
            string html = templte.ServiceItemTemplteHtml == null ? "" : templte.ServiceItemTemplteHtml;
            byte[] fileContents = Encoding.UTF8.GetBytes(html);

            return File(fileContents, "application/octet-stream", fileName);
        }
        //获取产品列表
        public async Task<string> GetItemsByStr(string Str)
        {
            Str = Str == null ? "" : Str;
            try
            {
                var Result = await client.For<ServiceItem>()
                    .Filter(t => t.ServiceCode.Contains(Str) || t.cnItemName.Contains(Str) || t.enItemName.Contains(Str))
                    .Filter(t => t.ServiceItemEnableState == 0)
                    //.Filter(t => t.ElementContent != null)
                    .OrderBy(t => t.ServiceItemID).Top(15)
                    .FindEntriesAsync();

                return JsonConvert.SerializeObject(new { ErrorCode = 200, ErrorMessage = "OK", data = Result.Select(r => new { r.ServiceItemID, r.cnItemName, r.enItemName, r.ServiceCode }) });
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 400, ErrorMessage = ex, data = "[]" });
            }
        }
        //获取产品表单字段默认值
        public async Task<string> GetFormField()
        {
            try
            {
                var Result = await client.For<FormField>().FindEntriesAsync();

                return JsonConvert.SerializeObject(new { ErrorCode = 200, ErrorMessage = "OK", data = Result });
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 400, ErrorMessage = ex, data = "[]" });
            }
        }
        // GET: ServiceItems/FormField
        public async Task<ActionResult> FormField()
        {
            return View(await client.For<FormField>().FindEntriesAsync());
        }
        public async Task<FileResult> ExportExcel(ItemSearchModel search)
        {
            #region 获取列表

            IBoundClient<ServiceItem> ItemResult = client.For<ServiceItem>().Expand(u => u.ItemServiceType).Expand(u => u.city);

            ItemResult = ItemResult.OrderByDescending(t => t.CreateTime);

            IEnumerable<ServiceItem> serviceItems = null;

            if (search.SupplierID > 0)
            {
                Supplier supplier = await client.For<Supplier>().Expand("ServiceItems/city").Key(search.SupplierID).FindEntryAsync();
                serviceItems = supplier.ServiceItems.OrderByDescending(t => t.CreateTime);
            }
            //启用状态 0：全部 1：启用 2：禁用
            int status = search.status;
            if (status > 0)
            {
                status -= 1;
                if (serviceItems == null)
                {
                    ItemResult = ItemResult.Filter(t => t.ServiceItemEnableState == (EnableState)status);
                }
                else
                {
                    serviceItems = serviceItems.Where(s => s.ServiceItemEnableState == (EnableState)status);
                }
            }
            if (!string.IsNullOrEmpty(search.FuzzySearch))
            {
                if (serviceItems == null)
                {
                    ItemResult = ItemResult.Filter(t => t.ServiceCode.Contains(search.FuzzySearch) || t.cnItemName.Contains(search.FuzzySearch) || t.enItemName.Contains(search.FuzzySearch));
                }
                else
                {
                    serviceItems = serviceItems.Where(t => t.ServiceCode.Contains(search.FuzzySearch) || t.cnItemName.Contains(search.FuzzySearch) || t.enItemName.Contains(search.FuzzySearch));
                }
            }
            if (search.ServiceTypeID > 0)
            {
                if (serviceItems == null)
                {
                    ItemResult = ItemResult.Filter(t => t.ServiceTypeID == search.ServiceTypeID);
                }
                else
                {
                    serviceItems = serviceItems.Where(t => t.ServiceTypeID == search.ServiceTypeID);
                }
            }
            if (serviceItems == null)
            {
                serviceItems = await ItemResult.FindEntriesAsync();
            }
            #endregion

            //创建Excel文件的对象
            NPOI.HSSF.UserModel.HSSFWorkbook book = new NPOI.HSSF.UserModel.HSSFWorkbook();
            //添加一个sheet
            NPOI.SS.UserModel.ISheet sheet1 = book.CreateSheet("Sheet1");

            int i = 0;
            //给sheet1添加第一行的头部标题
            NPOI.SS.UserModel.IRow row1 = sheet1.CreateRow(i);

            row1.CreateCell(0).SetCellValue("编码");
            row1.CreateCell(1).SetCellValue("中文名");
            row1.CreateCell(2).SetCellValue("英文名");
            row1.CreateCell(3).SetCellValue("目的地");
            row1.CreateCell(4).SetCellValue("类型");
            row1.CreateCell(5).SetCellValue("状态");
            row1.CreateCell(6).SetCellValue("默认供应商");
            row1.CreateCell(7).SetCellValue("保险天数");
            row1.CreateCell(8).SetCellValue("价格");
            row1.CreateCell(9).SetCellValue("表单");

            row1.Height = 450;

            //将数据逐步写入sheet1各个行
            foreach (var item in serviceItems)
            {
                ServiceType type = await client.For<ServiceType>().Key(item.ServiceTypeID).Select(s => s.ServiceTypeName).FindEntryAsync();
                Supplier supplier = null;
                try
                {
                    supplier = await client.For<Supplier>().Filter(s => s.SupplierID == item.DefaultSupplierID).FindEntryAsync();
                }
                catch { }
                SupplierServiceItem supplierServiceItem = await client.For<SupplierServiceItem>()
                    .Expand(s => s.ItemPriceBySuppliers)
                    .Filter(s => s.SupplierID == item.DefaultSupplierID && s.ServiceItemID == item.ServiceItemID)
                    .FindEntryAsync();
                SupplierPriceState DefaultSupplierPriceState = SupplierPriceState.Seted;
                if (supplierServiceItem == null || supplierServiceItem.ItemPriceBySuppliers == null)
                {
                    DefaultSupplierPriceState = SupplierPriceState.NotSeted;
                }
                else
                {
                    if (supplierServiceItem.ItemPriceBySuppliers.Where(p => p.startTime < DateTime.Now && p.EndTime > DateTime.Now.AddDays(-1)).FirstOrDefault() == null)
                    {
                        DefaultSupplierPriceState = SupplierPriceState.Expired;
                    }
                }

                i++;
                NPOI.SS.UserModel.IRow rowtemp = sheet1.CreateRow(i);

                rowtemp.CreateCell(0).SetCellValue(item.ServiceCode);
                rowtemp.CreateCell(1).SetCellValue(item.cnItemName);
                rowtemp.CreateCell(2).SetCellValue(item.enItemName);
                rowtemp.CreateCell(3).SetCellValue(item.city == null ? "" : item.city.CityName);
                rowtemp.CreateCell(4).SetCellValue(type.ServiceTypeName);
                rowtemp.CreateCell(5).SetCellValue(EnumHelper.GetEnumDescription(item.ServiceItemEnableState));
                rowtemp.CreateCell(6).SetCellValue(supplier == null ? "" : (supplier.SupplierNo + "-" + supplier.SupplierName));
                rowtemp.CreateCell(7).SetCellValue(item.InsuranceDays);
                rowtemp.CreateCell(8).SetCellValue(EnumHelper.GetEnumDescription(DefaultSupplierPriceState));
                rowtemp.CreateCell(9).SetCellValue(string.IsNullOrEmpty(item.ElementContent) ? "未设置" : "√");
            }

            // 写入到客户端 
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            book.Write(ms);
            ms.Seek(0, SeekOrigin.Begin);
            return File(ms, "application/vnd.ms-excel", "itemlist.xls");
        }
        //获取产品信息
        public async Task<string> GetItemByID(int? id)
        {
            if (id == null)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "产品ID不能为空" });
            }
            ServiceItem item = await client.For<ServiceItem>().Key(id).FindEntryAsync();
            return JsonConvert.SerializeObject(item);
        }
        // GET: ServiceItems/Edit/5
        public async Task<ActionResult> ItemPrices()
        {
            ViewBag.Supplier = await client.For<Supplier>().OrderBy(oo => oo.SupplierNo).FindEntriesAsync();
            ViewBag.ServiceType = await client.For<ServiceType>().FindEntriesAsync();
            ViewBag.ChangeNum = await client.For<SupplierServiceItem>().Filter(s => s.IsChange).Count().FindScalarAsync<int>();
            ViewBag.Cities = await client.For<City>().FindEntriesAsync();
            return View();
        }
        public async Task<string> GetItemPrices(ShareSearchModel share)
        {
            //if (share.ItemPriceSearch == null || (share.ItemPriceSearch.SupplierID <= 0 && string.IsNullOrEmpty(share.ItemPriceSearch.FuzzySearch) && !share.ItemPriceSearch.IsChange))
            //{
            //    return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "查询条件不能为空" });
            //}
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
                .Expand(s => s.Service.city)
                .Expand(s => s.Service)
                .Expand(s => s.ItemSupplier)
                .Expand(s => s.ItemCurrency)
                .Expand(s => s.ItemPriceBySuppliers)
                .Expand("ExtraServicePrices/Service");
            var ResultCount = client.For<SupplierServiceItem>();
            if (share.ItemPriceSearch.IsChange)
            {
                Result = Result.Filter("IsChange eq true");
                ResultCount = ResultCount.Filter("IsChange eq true");
            }
            if (share.ItemPriceSearch.SupplierID > 0)
            {
                Result = Result.Filter("SupplierID eq " + share.ItemPriceSearch.SupplierID);
                ResultCount = ResultCount.Filter("SupplierID eq " + share.ItemPriceSearch.SupplierID);
            }
            if (share.ItemPriceSearch.CityID > 0)
            {
                Result = Result.Filter("Service/CityID eq " + share.ItemPriceSearch.CityID);
                ResultCount = ResultCount.Filter("Service/CityID eq " + share.ItemPriceSearch.CityID);
            }
            if (!string.IsNullOrEmpty(share.ItemPriceSearch.FuzzySearch))
            {//模糊搜索，以空格分开
                string[] names = share.ItemPriceSearch.FuzzySearch.Trim().Split(' ');
                string filter = "contains(Service/cnItemName,'" + share.ItemPriceSearch.FuzzySearch + "') or contains(Service/enItemName,'" + share.ItemPriceSearch.FuzzySearch + "')";
                foreach (var name in names)
                {
                    if (name.Trim().Length > 0)
                    {
                        filter += " or Service/ServiceCode eq '" + name + "'";
                    }
                }
                Result = Result.Filter(filter);
                ResultCount = ResultCount.Filter(filter);
                //Result = Result.Filter(s => s.Service.cnItemName.Contains(share.ItemPriceSearch.FuzzySearch) || s.Service.enItemName.Contains(share.ItemPriceSearch.FuzzySearch) || s.Service.ServiceCode.Contains(share.ItemPriceSearch.FuzzySearch));
                //ResultCount = ResultCount.Filter(s => s.Service.cnItemName.Contains(share.ItemPriceSearch.FuzzySearch) || s.Service.enItemName.Contains(share.ItemPriceSearch.FuzzySearch) || s.Service.ServiceCode.Contains(share.ItemPriceSearch.FuzzySearch));
            }
            if (share.ItemPriceSearch.ServiceTypeID > 0)
            {
                Result = Result.Filter("Service/ServiceTypeID eq " + share.ItemPriceSearch.ServiceTypeID);
                ResultCount = ResultCount.Filter("Service/ServiceTypeID eq " + share.ItemPriceSearch.ServiceTypeID);
            }
            var temp = await Result.GetCommandTextAsync();
            int count = await ResultCount.Count().FindScalarAsync<int>();
            var list = await Result.OrderByDescending(s => s.SupplierServiceItemID)
                .Skip(start).Top(length)
                .FindEntriesAsync();
            var data = list.Select(s => new
            {
                s.IsChange,
                s.PayType,
                s.SupplierServiceItemID,
                s.ServiceItemID,
                s.SupplierID,
                s.Service.ServiceCode,
                s.Service.cnItemName,
                CityName = s.Service.city == null ? "" : s.Service.city.CityName,
                TravelCompany = s.Service.TravelCompany == null ? "" : s.Service.TravelCompany,
                s.Service.ItemServiceType.ServiceTypeName,
                s.Service.ServiceItemEnableState,
                s.ItemSupplier.SupplierNo,
                s.ItemCurrency.CurrencyNo,
                s.ItemCurrency.CurrencyChangeType,//0外币-人民币，1人民币-外币
                s.ItemCurrency.ExchangeRate,
                s.SellPrice,
                s.ChildSellPrice,
                PriceList = s.ItemPriceBySuppliers == null ? null
                    : s.ItemPriceBySuppliers.OrderByDescending(f => f.startTime)//.Where(price => price.startTime <= DateTimeOffset.Now.Date && DateTimeOffset.Now.Date <= price.EndTime)
                    .Select(price => new
                    {
                        price.AdultNetPrice,
                        price.BobyNetPrice,
                        price.ChildNetPrice,
                        price.Price,
                        StartDate = price.startTime.ToString("yyyy-MM-dd"),
                        EndDate = price.EndTime.ToString("yyyy-MM-dd"),
                        Profit = float.Parse((s.SellPrice - (s.PayType == PricingMethod.ByPerson
                            ? (s.ItemCurrency.CurrencyChangeType == ChangeType.FromChina ? price.AdultNetPrice / s.ItemCurrency.ExchangeRate : price.AdultNetPrice * s.ItemCurrency.ExchangeRate)
                            : (s.ItemCurrency.CurrencyChangeType == ChangeType.FromChina ? price.Price / s.ItemCurrency.ExchangeRate : price.Price * s.ItemCurrency.ExchangeRate))).ToString("F2")),
                        ChildProfit = float.Parse((s.ChildSellPrice - (s.PayType == PricingMethod.ByPerson
                            ? (s.ItemCurrency.CurrencyChangeType == ChangeType.FromChina ? price.ChildNetPrice / s.ItemCurrency.ExchangeRate : price.ChildNetPrice * s.ItemCurrency.ExchangeRate) : 0)).ToString("F2")),
                    }),
                ExtraServicePrices = s.ExtraServicePrices == null ? null : s.ExtraServicePrices.Select(e => new
                {
                    e.ExtraServicePriceID,
                    e.Service.ServiceName,
                    e.ServicePrice,
                    e.ServiceSellPrice,
                    ServiceProfit = float.Parse((e.ServiceSellPrice - (s.ItemCurrency.CurrencyChangeType == ChangeType.FromChina ? e.ServicePrice / s.ItemCurrency.ExchangeRate : e.ServicePrice * s.ItemCurrency.ExchangeRate)).ToString("F2"))
                })
            });
            return JsonConvert.SerializeObject(new { draw = draw, recordsFiltered = count, data = data, SearchModel = share });
        }
        //导出excel
        public async Task<FileResult> PriceExportExcel(int? ServiceTypeID, int? SupplierID, string FuzzySearch, bool? IsChange, int? CityID)
        {
            var Result = client.For<SupplierServiceItem>()
                .Expand(s => s.Service.ItemServiceType)
                .Expand(s => s.Service.city)
                .Expand(s => s.ItemSupplier)
                .Expand(s => s.ItemCurrency)
                .Expand(s => s.ItemPriceBySuppliers)
                .Expand("ExtraServicePrices/Service");
            if (IsChange == true)
            {
                Result = Result.Filter("IsChange eq true");
            }
            if (SupplierID > 0)
            {
                Result = Result.Filter("SupplierID eq " + SupplierID);
            }
            if (!string.IsNullOrEmpty(FuzzySearch))
            {
                //模糊搜索，以空格分开
                string[] names = FuzzySearch.Trim().Split(' ');
                string filter = "contains(Service/cnItemName,'" + FuzzySearch + "') or contains(Service/enItemName,'" + FuzzySearch + "')";
                foreach (var name in names)
                {
                    if (name.Trim().Length > 0)
                    {
                        filter += " or Service/ServiceCode eq '" + name + "'";
                    }
                }
                Result = Result.Filter(filter);
            }
            if (ServiceTypeID > 0)
            {
                Result = Result.Filter("Service/ServiceTypeID eq " + ServiceTypeID);
            }
            if (CityID > 0)
            {
                Result = Result.Filter("Service/CityID eq " + CityID);
            }
            var list = await Result.OrderByDescending(s => s.SupplierServiceItemID).FindEntriesAsync();

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
            row1.CreateCell(3).SetCellValue("目的地");
            row1.CreateCell(4).SetCellValue("类型");
            row1.CreateCell(5).SetCellValue("状态");
            row1.CreateCell(6).SetCellValue("供应商");
            row1.CreateCell(7).SetCellValue("币别");
            row1.CreateCell(8).SetCellValue("价格开始日期");
            row1.CreateCell(9).SetCellValue("价格结束日期");
            row1.CreateCell(10).SetCellValue("成人");
            row1.CreateCell(11).SetCellValue("儿童");
            row1.CreateCell(12).SetCellValue("婴儿");
            row1.CreateCell(13).SetCellValue("单价");
            row1.CreateCell(14).SetCellValue("成人卖价");
            row1.CreateCell(15).SetCellValue("成人利润");
            row1.CreateCell(16).SetCellValue("成人利润率");
            row1.CreateCell(17).SetCellValue("儿童卖价");
            row1.CreateCell(18).SetCellValue("儿童利润");
            row1.CreateCell(19).SetCellValue("儿童利润率");
            row1.CreateCell(20).SetCellValue("价格状态");
            row1.Height = 450;

            //将数据逐步写入sheet1各个行
            foreach (var item in list)
            {
                i++;
                NPOI.SS.UserModel.IRow rowtemp = sheet1.CreateRow(i);

                rowtemp.CreateCell(0).SetCellValue(item.Service.ServiceCode);
                rowtemp.CreateCell(1).SetCellValue(item.Service.cnItemName);
                rowtemp.CreateCell(2).SetCellValue(item.Service.TravelCompany);
                rowtemp.CreateCell(3).SetCellValue(item.Service.city == null ? "" : item.Service.city.CityName);
                rowtemp.CreateCell(4).SetCellValue(item.Service.ItemServiceType.ServiceTypeName);
                rowtemp.CreateCell(5).SetCellValue(EnumHelper.GetEnumDescription(item.Service.ServiceItemEnableState));
                rowtemp.CreateCell(6).SetCellValue(item.ItemSupplier.SupplierNo);
                rowtemp.CreateCell(7).SetCellValue(item.ItemCurrency.CurrencyNo);
                if (item.ItemPriceBySuppliers != null && item.ItemPriceBySuppliers.Find(price => price.startTime <= DateTimeOffset.Now.Date && DateTimeOffset.Now.Date <= price.EndTime) != null)
                {
                    var one = item.ItemPriceBySuppliers.Find(price => price.startTime <= DateTimeOffset.Now.Date && DateTimeOffset.Now.Date <= price.EndTime);
                    rowtemp.CreateCell(8).SetCellValue(one.startTime.ToString("yyyy-MM-dd"));
                    rowtemp.CreateCell(9).SetCellValue(one.EndTime.ToString("yyyy-MM-dd"));
                    rowtemp.CreateCell(10).SetCellValue(one.AdultNetPrice);
                    rowtemp.CreateCell(11).SetCellValue(one.ChildNetPrice);
                    rowtemp.CreateCell(12).SetCellValue(one.BobyNetPrice);
                    rowtemp.CreateCell(13).SetCellValue(one.Price);
                    var Profit = item.SellPrice - (item.PayType == PricingMethod.ByPerson
                            ? (item.ItemCurrency.CurrencyChangeType == ChangeType.FromChina ? one.AdultNetPrice / item.ItemCurrency.ExchangeRate : one.AdultNetPrice * item.ItemCurrency.ExchangeRate)
                            : (item.ItemCurrency.CurrencyChangeType == ChangeType.FromChina ? one.Price / item.ItemCurrency.ExchangeRate : one.Price * item.ItemCurrency.ExchangeRate));
                    rowtemp.CreateCell(15).SetCellValue(Profit);
                    if (item.SellPrice > 0)
                    {
                        double percent = Convert.ToDouble(Profit) / Convert.ToDouble(item.SellPrice);
                        rowtemp.CreateCell(16).SetCellValue(percent.ToString("0%"));
                    }
                    if (item.PayType == PricingMethod.ByPerson)
                    {
                        var ChildProfit = item.ChildSellPrice - (item.ItemCurrency.CurrencyChangeType == ChangeType.FromChina ? one.ChildNetPrice / item.ItemCurrency.ExchangeRate : one.ChildNetPrice * item.ItemCurrency.ExchangeRate);
                        rowtemp.CreateCell(18).SetCellValue(ChildProfit);
                        if (item.ChildSellPrice > 0)
                        {
                            double Childpercent = Convert.ToDouble(ChildProfit) / Convert.ToDouble(item.ChildSellPrice);
                            rowtemp.CreateCell(19).SetCellValue(Childpercent.ToString("0%"));
                        }
                    }
                }
                rowtemp.CreateCell(14).SetCellValue(item.SellPrice);
                rowtemp.CreateCell(17).SetCellValue(item.ChildSellPrice);
                rowtemp.CreateCell(20).SetCellValue(item.IsChange ? "待确认" : "已确认");
                if (item.ExtraServicePrices != null && item.ExtraServicePrices.Count > 0)
                {
                    foreach (var Extra in item.ExtraServicePrices)
                    {
                        i++;
                        NPOI.SS.UserModel.IRow rowExtra = sheet1.CreateRow(i);
                        rowExtra.CreateCell(1).SetCellValue(Extra.Service.ServiceName);
                        rowExtra.CreateCell(13).SetCellValue(Extra.ServicePrice);
                        rowExtra.CreateCell(14).SetCellValue(Extra.ServiceSellPrice);
                        var ServiceProfit = Extra.ServiceSellPrice - (item.ItemCurrency.CurrencyChangeType == ChangeType.FromChina ? Extra.ServicePrice / item.ItemCurrency.ExchangeRate : Extra.ServicePrice * item.ItemCurrency.ExchangeRate);
                        rowExtra.CreateCell(15).SetCellValue(ServiceProfit);
                        if (Extra.ServiceSellPrice > 0)
                        {
                            double Servicepercent = Convert.ToDouble(ServiceProfit) / Convert.ToDouble(Extra.ServiceSellPrice);
                            rowExtra.CreateCell(16).SetCellValue(Servicepercent.ToString("0%"));
                        }
                    }
                }
            }
            // 写入到客户端 
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            book.Write(ms);
            ms.Seek(0, SeekOrigin.Begin);
            return File(ms, "application/vnd.ms-excel", "pricelist.xls");
        }
        public async Task<ActionResult> PriceOperation(int id)
        {
            return View(await client.For<ItemPriceLog>()
                .Filter(s => s.SupplierServiceItemID == id)
                .OrderByDescending(oo => oo.ItemPriceLogID)
                .FindEntriesAsync());
        }
        //填写卖价
        [HttpPost]
        public async Task<string> SaveSellPrices(SupplierServiceItem share)
        {
            try
            {
                string Remark = "";
                var item = await client.For<SupplierServiceItem>().Expand("ExtraServicePrices/Service").Key(share.SupplierServiceItemID).FindEntryAsync();

                if (item.PayType == PricingMethod.ByPerson)
                {
                    if (item.SellPrice != share.SellPrice)
                        Remark += "<div>成人卖价：" + item.SellPrice + "RMB→" + share.SellPrice + "RMB</div>";
                    if (item.ChildSellPrice != share.ChildSellPrice)
                        Remark += "<div>儿童卖价：" + item.ChildSellPrice + "RMB→" + share.ChildSellPrice + "RMB</div>";
                }
                else
                {
                    if (item.SellPrice != share.SellPrice)
                        Remark += "<div>卖价：" + item.SellPrice + "RMB→" + share.SellPrice + "RMB</div>";
                }
                item.SellPrice = share.SellPrice;
                item.ChildSellPrice = share.ChildSellPrice;
                await client.For<SupplierServiceItem>().Key(share.SupplierServiceItemID).Set(item).UpdateEntryAsync();
                if (share.ExtraServicePrices != null)
                {
                    foreach (var one in share.ExtraServicePrices)
                    {
                        var sone = item.ExtraServicePrices.Find(s => s.ExtraServicePriceID == one.ExtraServicePriceID);
                        if (sone != null)
                        {
                            if (sone.ServiceSellPrice != one.ServiceSellPrice)
                            {
                                Remark += "<div>" + sone.Service.ServiceName + "卖价：" + sone.ServiceSellPrice + "RMB→" + one.ServiceSellPrice + "RMB</div>";
                                sone.ServiceSellPrice = one.ServiceSellPrice;
                                await client.For<ExtraServicePrice>().Key(one.ExtraServicePriceID).Set(sone).UpdateEntryAsync();
                            }
                        }
                    }
                }
                if (Remark != "")
                {
                    string userName = User.Identity.Name;
                    User user = await client.For<User>().Filter(u => u.UserName == userName).FindEntryAsync();
                    await client.For<ItemPriceLog>().Set(new ItemPriceLog
                    {
                        Operate = "填写卖价",
                        OperateTime = DateTimeOffset.Now,
                        UserID = user.UserID,
                        UserName = user.NickName,
                        Operator = OrderOperator.inside,
                        SupplierServiceItemID = share.SupplierServiceItemID,
                        Remark = Remark
                    }).InsertEntryAsync();
                }
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 400, ErrorMessage = ex.Message });
            }
            return JsonConvert.SerializeObject(new { ErrorCode = 200, ErrorMessage = "OK" });

        }

        public async Task<string> GetStatisticsSales(string StartDate, string EndDate, string Name)
        {
            if (string.IsNullOrEmpty(Name))
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "产品名字不能为空" });
            }
            try
            {
                DateTime.Parse(StartDate);
                DateTime.Parse(EndDate);
            }
            catch
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "日期格式错误" });
            }
            string Message = await HttpHelper.GetActionForOdata("api/ServiceItemHistoryExtend/?Name=" + Name + "&StartDate=" + StartDate + "&EndDate=" + EndDate);
            List<StatisticsSales> StatisticsSales = JsonConvert.DeserializeObject<List<StatisticsSales>>(Message);
            return JsonConvert.SerializeObject(new { ErrorCode = 200, ErrorMessage = "OK", StatisticsSales });
        }
    }
}
