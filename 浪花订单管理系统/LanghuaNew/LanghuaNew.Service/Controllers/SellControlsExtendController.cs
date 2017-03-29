using Entity;
using LanghuaNew.Data;
using LanghuaNew.Service.App_Code;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace LanghuaNew.Service.Controllers
{
    public class SellControlsExtendController : ApiController
    {
        private LanghuaContent db = new LanghuaContent();
        // GET: api/SellControlsExtend
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/SellControlsExtend/5
        public async Task Get(int id)
        {
            await db.Database.ExecuteSqlCommandAsync("EXEC dbo.SelectSellControl @id="+id);
        }

        // POST: api/SellControlsExtend
        public async Task<HttpResponseMessage> Post([FromBody]string value)
        {
            List<SellControl> sells = JsonConvert.DeserializeObject<List<SellControl>>(value);

            List<SellControl> oldsells = db.SellControls.Include("FirstServiceItem").ToList();
            var AddList = sells.Except(oldsells, p => new { p.SupplierID, p.FirstServiceItem.ServiceItemID }).ToList();
            var ModifList = sells.Except(AddList, p => new { p.SupplierID, p.FirstServiceItem.ServiceItemID }).ToList();
            var DeleteList = oldsells.Except(ModifList, p => new { p.SupplierID, p.FirstServiceItem.ServiceItemID }).ToList();

            try
            {
                foreach (var sell in AddList)
                {
                    int itemid = sell.FirstServiceItem.ServiceItemID;
                    int supplierid = sell.SupplierID;
                    ServiceItem item = db.ServiceItems.Find(itemid);
                    Supplier supplier = db.Suppliers.Find(supplierid);
                    sell.FirstServiceItem = item;
                    sell.MonthNum = 1;
                    sell.SellControlName = "(" + supplier.SupplierNo + ")" + item.cnItemName + item.ServiceCode;
                    db.SellControls.Add(sell);
                }
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("(add)" + ex.Message, System.Text.Encoding.UTF8, "text/plain")
                };
            }
            try
            {
                foreach (var sell in ModifList)
                {
                    int itemid = sell.FirstServiceItem.ServiceItemID;
                    int supplierid = sell.SupplierID;
                    SellControl sc = db.SellControls.Where(s => s.SupplierID == supplierid && s.FirstServiceItem.ServiceItemID == itemid).FirstOrDefault();
                    if (sc != null)
                    {
                        sc.RowNum = sell.RowNum;
                        sc.SellControlClassifyID = sell.SellControlClassifyID;
                        db.Entry(sc).State = EntityState.Modified;
                    }
                }
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("(modif)" + ex.Message, System.Text.Encoding.UTF8, "text/plain")
                };
            }
            try
            {
                foreach (var sell in DeleteList)
                {
                    int itemid = sell.FirstServiceItem.ServiceItemID;
                    int supplierid = sell.SupplierID;
                    SellControl sc = db.SellControls.Where(s => s.SupplierID == supplierid && s.FirstServiceItem.ServiceItemID == itemid).FirstOrDefault();
                    if (sc != null)
                    {
                        db.SellControls.Remove(sc);
                    }
                }
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("(del)" + ex.Message, System.Text.Encoding.UTF8, "text/plain")
                };
            }
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("OK", System.Text.Encoding.UTF8, "text/plain")
            };
        }

        // PUT: api/SellControlsExtend/5
        public async Task<HttpResponseMessage> Put([FromBody]string value)
        {
            SellControl sell = JsonConvert.DeserializeObject<SellControl>(value);
            ServiceItem item = null;
            if (sell.SecondServiceItem != null)
            {
                int id = sell.SecondServiceItem.ServiceItemID;
                item = db.ServiceItems.Find(id);
            }
            try
            {
                SellControl oldsell = db.SellControls.Include("FirstServiceItem").Include("SecondServiceItem").Where(s => s.SellControlID == sell.SellControlID).First();
                oldsell.MonthNum = sell.MonthNum;
                oldsell.SecondServiceItem = item;
                oldsell.SellControlName = sell.SellControlName;
                oldsell.SellControlNum = sell.SellControlNum;
                oldsell.StartDate = sell.StartDate;
                oldsell.IsSurplusNum = sell.IsSurplusNum;
                oldsell.IsDistribution = sell.IsDistribution;
                db.Entry(oldsell).State = EntityState.Modified;
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(ex.Message, System.Text.Encoding.UTF8, "text/plain")
                };
            }
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("OK", System.Text.Encoding.UTF8, "text/plain")
            };
        }

        // DELETE: api/SellControlsExtend/5
        public void Delete(int id)
        {
        }
    }
}
