using Simple.OData.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Commond.Splider;
using LanghuaNew.Data;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http;
using Commond;
using Entity;

namespace Everything.Controllers
{
    //航班信息
    public class FlightController : Controller
    {
        private ODataClient client = new ODataClient(ConfigurationManager.AppSettings["ServicePath"] + "odata/");
        // GET: Flight
        public ActionResult Index()
        {
            string url = Request.Url.ToString();
            if (!url.Contains("localhost"))
            {
                return HttpNotFound();
            }
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> Search(string departureCity, string arrivalCity)
        {
            if (!string.IsNullOrEmpty(departureCity) && !string.IsNullOrEmpty(arrivalCity))
            {
                var Result = await GetFliters(departureCity, arrivalCity);
                ViewBag.FliterList = Result;
                ViewBag.Message = string.Format("{0}-{1} 查询到共有航班信息{2}条。", departureCity, arrivalCity,Result.Count());
            }
            else
            {
                ViewBag.Message = "出发地/目的地不能为空！";
            }
            ViewBag.DepartureCity = departureCity;
            ViewBag.ArrivalCity = arrivalCity;
            return View("Index");
        }

        [HttpPost]
        public async Task<ActionResult> SearchReturn(string departureCity, string arrivalCity)
        {
            if (!string.IsNullOrEmpty(departureCity) && !string.IsNullOrEmpty(arrivalCity))
            {
                var Result = await GetFliters(arrivalCity, departureCity);
                ViewBag.FliterList = Result;
                ViewBag.Message = string.Format("{0}-{1} 查询到共有航班信息{2}条。", arrivalCity, departureCity,  Result.Count());
            }
            else
            {
                ViewBag.Message = "出发地/目的地不能为空！";
            }
            ViewBag.DepartureCity = arrivalCity;
            ViewBag.ArrivalCity = departureCity;
            return View("Index");
        }

        [HttpPost]
        public async Task<ActionResult> Spider(string departureCity, string arrivalCity)
        {
            if (!string.IsNullOrEmpty(departureCity) && !string.IsNullOrEmpty(arrivalCity))
            {
                ISpider<FliterInfo> qunaer = new QunerFliterSplider();
                string url = string.Format("http://flight.qunar.com/schedule/international/fsearch_list.jsp?departure={0}&arrival={1}", departureCity, arrivalCity);
                qunaer.GetHtmlByUrl(url);
                List<FliterInfo> fliterInfos = qunaer.Capture();
                fliterInfos=SetDEAndAR(fliterInfos, departureCity, arrivalCity);
                if(fliterInfos!=null && fliterInfos.Count>0)
                {
                    string url2 = string.Format("http://flight.qunar.com/schedule/international/fsearch_list.jsp?departure={0}&arrival={1}", arrivalCity, departureCity);
                    qunaer.GetHtmlByUrl(url2);
                    List<FliterInfo> fliterInfos2 = qunaer.Capture();
                    fliterInfos2= SetDEAndAR(fliterInfos2, arrivalCity, departureCity);
                    fliterInfos.AddRange(fliterInfos2);
                    string result=await Save(departureCity, arrivalCity, fliterInfos);
                    if (result == "OK")
                    {
                        ViewBag.FliterList = fliterInfos;
                        ViewBag.Message = string.Format("抓取成功，{0}-{1} 航班，共抓取{2}条数据,包含返程数据,且已记录到数据库。", departureCity, arrivalCity, fliterInfos.Count());
                    }
                    else if(result == "NoContent")
                    {
                        ViewBag.FliterList = fliterInfos;
                        ViewBag.Message = string.Format("抓取成功，{0}-{1} 航班，共抓取{2}条数据,包含返程数据。数据库已存在该记录。", departureCity, arrivalCity, fliterInfos.Count());
                    }
                    else
                    {
                        ViewBag.Message = string.Format("抓取失败，请联系管理员！");
                    }
                }
                else
                {
                    ViewBag.Message = string.Format("抱歉，{0}-{1} 没有直达航班。", departureCity, arrivalCity);
                }
            }
            else
            {
                ViewBag.Message = "出发地/目的地不能为空！";
            }
            ViewBag.DepartureCity = departureCity;
            ViewBag.ArrivalCity = arrivalCity;
            return View("Index");
        }

        private List<FliterInfo> SetDEAndAR(List<FliterInfo> fliterInfos, string departure, string arrival)
        {
            foreach(var item in fliterInfos)
            {
                item.DepartureCity = departure;
                item.ArrivalCity = arrival;
            }
            return fliterInfos;
        }

        public async Task<string> Save(string departure,string arrival, List<FliterInfo> fliterInfos)
        {
            if ((string.IsNullOrEmpty(departure)))
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "出发地/目的地不能为空！" });
            }
            PostFliterInfoModel pdata = new PostFliterInfoModel();
            pdata.FilterDeparture = departure;
            pdata.FilterArrival = arrival;
            pdata.FliterInfos = fliterInfos;
            HttpResponseMessage Message = await HttpHelper.PostAction("FliterInfosExtend", JsonConvert.SerializeObject(pdata));
            string exMessage = Message.Content.ReadAsStringAsync().Result;
            return exMessage;
        }

        public async Task<List<FliterInfo>> GetFliters(string departure,string arrival)
        {
            List<FliterInfo> fliterInfos = new List<FliterInfo>();
            var result=await client.For<FliterInfo>().Filter(t => t.DepartureCity == departure && t.ArrivalCity == arrival).FindEntriesAsync();
            fliterInfos = result.ToList();
            return fliterInfos;
        }
    }
}
