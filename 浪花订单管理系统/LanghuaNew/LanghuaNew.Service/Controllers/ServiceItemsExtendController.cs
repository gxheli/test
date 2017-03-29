using LanghuaNew.Data;
using LanghuaNew.Service.App_Code;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace LanghuaNew.Service.Controllers
{
    public class ServiceItemsExtendController : ApiController
    {
        private LanghuaContent db = new LanghuaContent();
        // GET: api/ServiceItemsExtend
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/ServiceItemsExtend/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/ServiceItemsExtend
        public async Task<HttpResponseMessage> Post([FromBody]string value)
        {
            ServiceItem item = JsonConvert.DeserializeObject<ServiceItem>(value);
            if (!ModelState.IsValid)
            {
                return Request.CreateResponse(HttpStatusCode.NotModified);
            }
            if (item == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }
            var Suppliers = item.ItemSuplier.Select(s => s.SupplierID);
            List<Supplier> NewSupplierList = db.Suppliers.Where(P => Suppliers.Contains(P.SupplierID)).ToList();
            item.ItemSuplier = NewSupplierList;
            db.ServiceItems.Add(item);
            await db.SaveChangesAsync();
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(item.ServiceItemID.ToString(), System.Text.Encoding.UTF8, "text/plain")
            };
        }

        // PUT: api/ServiceItemsExtend/5
        public async Task<IHttpActionResult> Put([FromBody]string value)
        {
            try
            {
                ServiceItem serviceItem = JsonConvert.DeserializeObject<ServiceItem>(value);
                if (!ModelState.IsValid)
                {
                    return StatusCode(HttpStatusCode.NotModified);
                }
                if (serviceItem == null)
                {
                    return NotFound();
                }
                //获取选中供应商
                var Suppliers = serviceItem.ItemSuplier.Select(s => s.SupplierID);
                List<Supplier> NewSupplierList = db.Suppliers.Where(P => Suppliers.Contains(P.SupplierID)).ToList();
                serviceItem.ItemSuplier.Clear();
                //获取额外服务，分为已有和新增
                List<ExtraService> ExtraModif = new List<ExtraService>();
                List<ExtraService> ExtraNew = new List<ExtraService>();
                if (serviceItem.ExtraServices != null)
                {
                    ExtraModif = serviceItem.ExtraServices.Where(s => s.ExtraServiceID != 0).ToList();
                    ExtraNew = serviceItem.ExtraServices.Where(s => s.ExtraServiceID == 0).ToList();
                    serviceItem.ExtraServices.Clear();
                }
                //初始化数据
                db.ServiceItems.Attach(serviceItem);
                var item = db.Entry(serviceItem);
                item.Collection(i => i.ItemSuplier).Load();
                item.Collection(i => i.ExtraServices).Load();
                //处理选中供应商，新增新选中供应商，已有的选中供应商维持不变，删除被移除的供应商
                List<Supplier> OldList = serviceItem.ItemSuplier;
                var AddList = NewSupplierList.Except(OldList, i => i.SupplierID).ToList();
                var ModifList = NewSupplierList.Except(AddList, i => i.SupplierID).ToList();
                var DelList = OldList.Except(ModifList, i => i.SupplierID).ToList();
                DelList.ForEach(i => serviceItem.ItemSuplier.Remove(i));
                AddList.ForEach(i => serviceItem.ItemSuplier.Add(i));
                //foreach (var d in DelList)
                //{//删除供应商价格
                //    var serviceitem = db.SupplierServiceItems.Where(s => s.SupplierID == d.SupplierID & s.ServiceItemID == serviceItem.ServiceItemID).FirstOrDefault();
                //    db.SupplierServiceItems.Remove(serviceitem);
                //}
                //处理额外服务，删除被移除的额外服务
                if (serviceItem.ExtraServices != null)
                {
                    List<ExtraService> ExtraOld = serviceItem.ExtraServices;
                    var ExtraDelList = ExtraOld.Except(ExtraModif, i => i.ExtraServiceID).ToList();
                    ExtraDelList.ForEach(i => db.ExtraServices.Remove(i));
                    //修改保留的额外服务
                    foreach (var Modif in ExtraModif)
                    {
                        ExtraService ext = db.ExtraServices.Find(Modif.ExtraServiceID);
                        ext.MaxNum = Modif.MaxNum;
                        ext.ServiceEnName = Modif.ServiceEnName;
                        ext.ServiceName = Modif.ServiceName;
                        ext.MinNum = Modif.MinNum;
                        ext.ServiceUnit = Modif.ServiceUnit;
                        db.Entry(ext).State = EntityState.Modified;
                    }
                }
                //新增没有ID的
                serviceItem.ExtraServices.AddRange(ExtraNew);
                //保存
                item.State = EntityState.Modified;
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                string err = ex.Message;
            }
            return StatusCode(HttpStatusCode.NoContent);
        }

        // DELETE: api/ServiceItemsExtend/5
        public void Delete(int id)
        {
        }
    }
}
