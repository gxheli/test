using AutoMapper;
using Commond;
using LanghuaNew.Data;
using LanghuaNew.Models;
using Newtonsoft.Json;
using Simple.OData.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace LanghuaNew.Controllers
{
    public class FliterController : Controller
    {
        private ODataClient client = new ODataClient(ConfigurationManager.AppSettings["ServicePath"] + "odata/");
        //航班信息
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
    }
}
