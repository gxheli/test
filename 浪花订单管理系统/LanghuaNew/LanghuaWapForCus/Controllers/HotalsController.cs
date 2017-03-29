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

namespace LanghuaWapForCus.Controllers
{
    public class HotalsController : Controller
    {
        private ODataClient client = new ODataClient(ConfigurationManager.AppSettings["ServicePath"] + "odata/");
        //酒店信息
        public async Task<string> GetHotal(string Str, int? CityID)
        {
            CityID = CityID == null ? 0 : CityID;
            try
            {
                Str = Str == null ? "" : Str;
                IEnumerable<Hotal> hotals = await client.For<Hotal>()
                    .Expand(u => u.HotalArea)
                    .Filter(u => u.HotalName.Contains(Str))
                    .Filter(u => u.HotalArea.CityID == CityID)
                    .Top(15)
                    .FindEntriesAsync();
                return JsonConvert.SerializeObject(new { ErrorCode = 200, ErrorMessage = "OK", hotals });
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 400, ErrorMessage = ex, hotals = "[]" });
            }
        }
        //区域信息
        public async Task<string> GetArea(int? CityID)
        {
            CityID = CityID == null ? 0 : CityID;
            try
            {
                IEnumerable<Area> area = await client.For<Area>()
                    .Filter(u => u.CityID == CityID)
                    .Filter(u => u.AreaEnableState == EnableState.Enable)
                    .FindEntriesAsync();
                return JsonConvert.SerializeObject(new { ErrorCode = 200, ErrorMessage = "OK", area });
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 400, ErrorMessage = ex, area = "[]" });
            }
        }
    }
}
