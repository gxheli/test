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
using Newtonsoft.Json;
using Simple.OData.Client;
using System.Configuration;
using Commond;
using System.Net.Http;

namespace LanghuaWapForCus.Controllers
{
    public class TravellersController : Controller
    {
        private ODataClient client = new ODataClient(ConfigurationManager.AppSettings["ServicePath"] + "odata/");
        //常用旅客信息
        public async Task<string> GetTraveller(int? CustomerID)
        {
            if(CustomerID==null)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 400, ErrorMessage = "查询失败！失败原因：参数不正确"});
            }
            try
            {
                var travellers = await client.For<Traveller>().Filter(t => t.CustomerID == CustomerID).OrderByDescending(t => t.TravellerID).FindEntriesAsync();

                return JsonConvert.SerializeObject(new { ErrorCode = 200, ErrorMessage = "OK", travellers });
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 400, ErrorMessage = "查询失败！失败原因：" + ex.Message });
            }
        }
        //单个旅客信息
        public async Task<string> GetTravellerByID(int id)
        {
            try
            {
                Traveller traveller = await client.For<Traveller>().Key(id).FindEntryAsync();
                return JsonConvert.SerializeObject(new { ErrorCode = 200, ErrorMessage = "OK", traveller });
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 400, ErrorMessage = "查询失败！失败原因：" + ex.Message });
            }
        }
        //新增常用旅客
        [HttpPost]
        public async Task<string> AddTraveller(Traveller traveller)
        {
            if (traveller.CustomerID == 0)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "新增失败！失败原因：参数不正确" });
            }
            if (string.IsNullOrEmpty(traveller.TravellerName))
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "姓名不能为空" });
            }
            if (string.IsNullOrEmpty(traveller.TravellerEnname))
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "拼音不能为空" });
            }
            if (string.IsNullOrEmpty(traveller.PassportNo))
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "护照号不能为空" });
            }
            try
            {
                Traveller travellerOld = await client.For<Traveller>().Filter(t => t.PassportNo == traveller.PassportNo && t.CustomerID == traveller.CustomerID).FindEntryAsync();
                if (travellerOld != null)
                {
                    return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "护照号已重复" });
                }

                traveller.Birthday = traveller.Birthday < Convert.ToDateTime("1900-01-01") ? Convert.ToDateTime("1900-01-01") : traveller.Birthday;
                traveller.CreateTime = DateTime.Now;
                traveller.TravellerDetail = new TravellerDetail();

                traveller = await client.For<Traveller>().Set(traveller).InsertEntryAsync();
                return JsonConvert.SerializeObject(new { ErrorCode = 200, ErrorMessage = "OK", traveller });
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 400, ErrorMessage = "新增失败！失败原因：" + ex.Message });
            }
        }
        //删除常用旅客
        [HttpPost]
        public async Task<string> DelTraveller(int id)
        {
            try
            {
                await client.For<Traveller>().Key(id).DeleteEntryAsync();
                return JsonConvert.SerializeObject(new { ErrorCode = 200, ErrorMessage = "OK" });
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 400, ErrorMessage = "删除失败！失败原因："+ex.Message });
            }
        }
        [HttpPost]
        //修改常用旅客
        public async Task<string> EditTraveller(Traveller traveller)
        {
            if (traveller.TravellerID == 0)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "新增失败！失败原因：参数不正确" });
            }
            if (string.IsNullOrEmpty(traveller.TravellerName))
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "姓名不能为空" });
            }
            if (string.IsNullOrEmpty(traveller.TravellerEnname))
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "拼音不能为空" });
            }
            if (string.IsNullOrEmpty(traveller.PassportNo))
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "护照号不能为空" });
            }
            try
            {
                Traveller travellerOld = await client.For<Traveller>().Key(traveller.TravellerID).FindEntryAsync();

                Traveller travellerS = await client.For<Traveller>().Filter(t => t.PassportNo == traveller.PassportNo && t.CustomerID == travellerOld.CustomerID && t.TravellerID != traveller.TravellerID).FindEntryAsync();
                if (travellerS != null)
                {
                    return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "护照号已重复" });
                }

                traveller.TravellerDetail = new TravellerDetail();
                traveller.TravellerDetail = travellerOld.TravellerDetail;
                traveller.CustomerID = travellerOld.CustomerID;
                traveller.Birthday = traveller.Birthday < Convert.ToDateTime("1900-01-01") ? Convert.ToDateTime("1900-01-01") : traveller.Birthday;
                traveller.CreateTime = travellerOld.CreateTime;
                await client.For<Traveller>().Key(traveller.TravellerID).Set(traveller).UpdateEntryAsync();
                return JsonConvert.SerializeObject(new { ErrorCode = 200, ErrorMessage = "OK", traveller });
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 400, ErrorMessage = "修改失败！失败原因：" + ex.Message });
            }
        }
        //新增修改常用旅客附加资料
        [HttpPost]
        public async Task<string> EditTravellerDetail(Traveller traveller)
        {
            try
            {
                Traveller travellerOld =await client.For<Traveller>().Key(traveller.TravellerID).FindEntryAsync();
                travellerOld.TravellerDetail = traveller.TravellerDetail;
                await client.For<Traveller>().Key(traveller.TravellerID).Set(travellerOld).UpdateEntryAsync();
                return JsonConvert.SerializeObject(new { ErrorCode = 200, ErrorMessage = "OK",travellerOld });
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 400, ErrorMessage = "保存失败！失败原因：" + ex.Message });
            }
        }
    }
}
