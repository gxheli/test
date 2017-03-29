using LanghuaNew.Data;
using Newtonsoft.Json;
using Simple.OData.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace LanghuaWapForCus.Controllers
{
    public class ServiceRulesController :Controller
    {
        private ODataClient client = new ODataClient(ConfigurationManager.AppSettings["ServicePath"] + "odata/");

        //根据产品获取规则列表
        public async Task<string> GetRulesByItemID(int id)
        {
            var Result = await client.For<ServiceRule>().Filter(t => t.RuleServiceItem.Any(r => r.ServiceItemID == id) && t.UseState == EnableState.Enable).FindEntriesAsync();
            return JsonConvert.SerializeObject(Result);
        }
    }
}