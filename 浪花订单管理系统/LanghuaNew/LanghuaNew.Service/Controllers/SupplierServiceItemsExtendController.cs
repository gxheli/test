using LanghuaNew.Data;
using LanghuaNew.Service.App_Code;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace LanghuaNew.Service.Controllers
{
    public class SupplierServiceItemsExtendController : ApiController
    {
        private LanghuaContent db = new LanghuaContent();
        // GET: api/SupplierServiceItemsExtend
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/SupplierServiceItemsExtend/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/SupplierServiceItemsExtend
        public async Task<HttpResponseMessage> Post([FromBody]string value)
        {
            string Err = "OK";
            try
            {
                SupplierServiceItem item = JsonConvert.DeserializeObject<SupplierServiceItem>(value);
                if (item == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }
                Supplier supplier = db.Suppliers.Find(item.SupplierID);
                SupplierServiceItem oldItem = db.SupplierServiceItems
                    .Include("ItemPriceBySuppliers").Include("ExtraServicePrices")
                    .Where(s => s.ServiceItemID == item.ServiceItemID && s.SupplierID == item.SupplierID).FirstOrDefault();
                SupplierServiceItemChange oldItemChange = db.SupplierServiceItemChanges.Where(s => s.ServiceItemID == item.ServiceItemID && s.SupplierID == item.SupplierID).FirstOrDefault();
                if (oldItem == null || (oldItem.IsChange && oldItemChange == null))//首次填写价格
                {
                    if (oldItem == null)
                    {
                        if (supplier.EnableOnline)
                        {
                            item.IsChange = true;
                        }
                        db.SupplierServiceItems.Add(item);
                    }
                    else
                    {
                        if (oldItem.ItemPriceBySuppliers != null) db.ItemPriceBySuppliers.RemoveRange(oldItem.ItemPriceBySuppliers);
                        if (oldItem.ExtraServicePrices != null) db.ExtraServicePrices.RemoveRange(oldItem.ExtraServicePrices);
                        if (db.SupplierServiceItemChanges.Where(s => s.ServiceItemID == item.ServiceItemID && s.SupplierID == item.SupplierID).FirstOrDefault() != null)
                            db.SupplierServiceItemChanges.Remove(db.SupplierServiceItemChanges.Where(s => s.ServiceItemID == item.ServiceItemID && s.SupplierID == item.SupplierID).FirstOrDefault());
                        oldItem.CurrencyID = item.CurrencyID;
                        oldItem.PayType = item.PayType;
                        oldItem.Remark = item.Remark;
                        oldItem.SelectEffectiveWay = item.SelectEffectiveWay;
                        oldItem.ItemPriceBySuppliers = item.ItemPriceBySuppliers;
                        oldItem.ExtraServicePrices = item.ExtraServicePrices;
                        db.Entry(oldItem).State = EntityState.Modified;
                        Err = oldItem.SupplierServiceItemID.ToString();
                    }
                }
                else if (!supplier.EnableOnline)
                {
                    if (oldItem.ItemPriceBySuppliers != null) db.ItemPriceBySuppliers.RemoveRange(oldItem.ItemPriceBySuppliers);
                    if (oldItem.ExtraServicePrices != null) db.ExtraServicePrices.RemoveRange(oldItem.ExtraServicePrices);
                    if (db.SupplierServiceItemChanges.Where(s => s.ServiceItemID == item.ServiceItemID && s.SupplierID == item.SupplierID).FirstOrDefault() != null)
                        db.SupplierServiceItemChanges.Remove(db.SupplierServiceItemChanges.Where(s => s.ServiceItemID == item.ServiceItemID && s.SupplierID == item.SupplierID).FirstOrDefault());
                    oldItem.IsChange = false;
                    oldItem.CurrencyID = item.CurrencyID;
                    oldItem.PayType = item.PayType;
                    oldItem.Remark = item.Remark;
                    oldItem.SelectEffectiveWay = item.SelectEffectiveWay;
                    oldItem.ItemPriceBySuppliers = item.ItemPriceBySuppliers;
                    oldItem.ExtraServicePrices = item.ExtraServicePrices;
                    db.Entry(oldItem).State = EntityState.Modified;
                }
                else
                {
                    if (db.SupplierServiceItemChanges.Where(s => s.ServiceItemID == item.ServiceItemID && s.SupplierID == item.SupplierID).FirstOrDefault() != null)
                        db.SupplierServiceItemChanges.Remove(db.SupplierServiceItemChanges.Where(s => s.ServiceItemID == item.ServiceItemID && s.SupplierID == item.SupplierID).FirstOrDefault());

                    SupplierServiceItemChange change = new SupplierServiceItemChange();
                    change.SupplierServiceItemID = oldItem.SupplierServiceItemID;
                    change.CurrencyID = item.CurrencyID;
                    change.PayType = item.PayType;
                    change.SelectEffectiveWay = item.SelectEffectiveWay;
                    change.ServiceItemID = item.ServiceItemID;
                    change.SupplierID = item.SupplierID;
                    change.Remark = item.Remark;
                    change.ItemPriceBySuppliers = new List<ItemPriceBySupplierChange>();
                    change.ExtraServicePrices = new List<ExtraServicePriceChange>();
                    foreach (var one in item.ItemPriceBySuppliers)
                    {
                        ItemPriceBySupplierChange price = new ItemPriceBySupplierChange();
                        price.AdultNetPrice = one.AdultNetPrice;
                        price.BobyNetPrice = one.BobyNetPrice;
                        price.ChildNetPrice = one.ChildNetPrice;
                        price.Price = one.Price;
                        price.startTime = one.startTime;
                        price.EndTime = one.EndTime;
                        price.ItemPriceBySupplierID = one.ItemPriceBySupplierID;
                        change.ItemPriceBySuppliers.Add(price);
                    }
                    if (item.ExtraServicePrices != null)
                    {
                        foreach (var one in item.ExtraServicePrices)
                        {
                            ExtraServicePriceChange price = new ExtraServicePriceChange();
                            price.ExtraServiceID = one.ExtraServiceID;
                            price.ServicePrice = one.ServicePrice;
                            price.ExtraServicePriceID = one.ExtraServicePriceID;
                            change.ExtraServicePrices.Add(price);
                        }
                    }
                    db.SupplierServiceItemChanges.Add(change);
                    oldItem.IsChange = true;
                    db.Entry(oldItem).State = EntityState.Modified;
                }
                await db.SaveChangesAsync();
                if (oldItem == null)//首次填写价格，返回ID记录日志
                {
                    Err = item.SupplierServiceItemID.ToString();
                }
            }
            catch (Exception ex)
            {
                Err = ex.Message;
            }
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(Err, System.Text.Encoding.UTF8, "text/plain")
            };
        }

        // PUT: api/SupplierServiceItemsExtend/5
        public async Task<HttpResponseMessage> Put([FromBody]string value)
        {
            try
            {
                int SupplierServiceItemChangeID = int.Parse(value);
                var change = db.SupplierServiceItemChanges.Include("ItemPriceBySuppliers").Include("ExtraServicePrices")
                    .Where(s => s.SupplierServiceItemChangeID == SupplierServiceItemChangeID).First();
                int SupplierServiceItemID = change.SupplierServiceItemID;
                var item = db.SupplierServiceItems.Include("ItemPriceBySuppliers").Include("ExtraServicePrices")
                    .Where(s => s.SupplierServiceItemID == SupplierServiceItemID).First();

                if (item.ItemPriceBySuppliers != null) db.ItemPriceBySuppliers.RemoveRange(item.ItemPriceBySuppliers);
                if (item.ExtraServicePrices != null) db.ExtraServicePrices.RemoveRange(item.ExtraServicePrices);

                item.CurrencyID = change.CurrencyID;
                item.IsChange = false;
                item.PayType = change.PayType;
                item.Remark = change.Remark;
                item.SelectEffectiveWay = change.SelectEffectiveWay;

                item.ItemPriceBySuppliers = new List<ItemPriceBySupplier>();
                item.ExtraServicePrices = new List<ExtraServicePrice>();
                foreach (var one in change.ItemPriceBySuppliers)
                {
                    ItemPriceBySupplier price = new ItemPriceBySupplier();
                    price.AdultNetPrice = one.AdultNetPrice;
                    price.BobyNetPrice = one.BobyNetPrice;
                    price.ChildNetPrice = one.ChildNetPrice;
                    price.Price = one.Price;
                    price.startTime = one.startTime;
                    price.EndTime = one.EndTime;
                    item.ItemPriceBySuppliers.Add(price);
                }
                if (change.ExtraServicePrices != null)
                {
                    foreach (var one in change.ExtraServicePrices)
                    {
                        ExtraServicePrice price = new ExtraServicePrice();
                        price.ExtraServiceID = one.ExtraServiceID;
                        price.ServicePrice = one.ServicePrice;
                        item.ExtraServicePrices.Add(price);
                    }
                }
                db.Entry(item).State = EntityState.Modified;
                db.SupplierServiceItemChanges.Remove(change);
                await db.SaveChangesAsync();
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(SupplierServiceItemID.ToString(), System.Text.Encoding.UTF8, "text/plain")
                };
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(ex.Message, System.Text.Encoding.UTF8, "text/plain")
                };
            }
        }

        // DELETE: api/SupplierServiceItemsExtend/5
        public void Delete(int id)
        {
        }
    }
}
