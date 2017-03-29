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
    public class ServiceRulesExtendController : ApiController
    {
        private LanghuaContent db = new LanghuaContent();
        // GET: api/ServiceRulesExtend
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/ServiceRulesExtend/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/ServiceRulesExtend
        public async Task<HttpResponseMessage> Post([FromBody]string value)
        {
            try
            {
                ServiceRule rule = JsonConvert.DeserializeObject<ServiceRule>(value);
                if (!ModelState.IsValid)
                {
                    return new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent("模型没有通过验证", System.Text.Encoding.UTF8, "text/plain")
                    };
                }
                if (rule == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }
                if (rule.RuleServiceItem == null)
                {
                    rule.RuleServiceItem = new List<ServiceItem>();
                }
                var items = rule.RuleServiceItem.Select(s => s.ServiceItemID);
                List<ServiceItem> itemList = db.ServiceItems.Where(s => items.Contains(s.ServiceItemID)).ToList();
                rule.RuleServiceItem.Clear();

                if (rule.ServiceRuleID >0)
                {
                    //修改
                    db.ServiceRules.Attach(rule);
                    var ru = db.Entry(rule);
                    ru.Collection(s => s.RuleServiceItem).Load();
                    var OldItemList = rule.RuleServiceItem;
                    var AddItemList = itemList.Except(OldItemList, p => p.ServiceItemID).ToList();
                    var ModifList = itemList.Except(AddItemList, p => p.ServiceItemID).ToList();
                    var DeleteList = OldItemList.Except(ModifList, p => p.ServiceItemID).ToList();
                    DeleteList.ForEach(P => rule.RuleServiceItem.Remove(P));
                    AddItemList.ForEach(P => rule.RuleServiceItem.Add(P));
                    ru.State = EntityState.Modified;
                }
                else
                {
                    //新增
                    rule.RuleServiceItem.AddRange(itemList);
                    db.ServiceRules.Add(rule);
                }
                await db.SaveChangesAsync();
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(rule.ServiceRuleID.ToString(), System.Text.Encoding.UTF8, "text/plain")
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

        // PUT: api/ServiceRulesExtend/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/ServiceRulesExtend/5
        public void Delete(int id)
        {
        }
    }
}
