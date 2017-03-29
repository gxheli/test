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
using LanghuaNew.Models;
using Newtonsoft.Json;

namespace LanghuaNew.Controllers
{
    public class WeixinMessagesController : Controller
    {
        private ODataClient client = new ODataClient(ConfigurationManager.AppSettings["ServicePath"] + "odata/");
        public async Task<string> GetWeixinMessages(SearchModel search)
        {
            int draw = 1;
            int start = 0;
            int length = 50;
            int status = 0;
            int CountryID = 0;
            string FuzzySearch = string.Empty;
            if (search.length > 0)
            {
                draw = search.draw;
                start = search.start;
                length = search.length;
            }
            if (search.WeixinSearch != null)
            {
                status = search.WeixinSearch.status;
                CountryID = search.WeixinSearch.CountryID;
                FuzzySearch = search.WeixinSearch.FuzzySearch;
            }
            var Result = client.For<WeixinMessage>().Expand(s => s.WeixinCountry);
            var ResultCount = client.For<WeixinMessage>();
            if (status == 1)
            {
                Result = Result.Filter(t => t.WeixinMessageState == EnableState.Enable);
                ResultCount = ResultCount.Filter(t => t.WeixinMessageState == EnableState.Enable);
            }
            else if (status == 2)
            {
                Result = Result.Filter(t => t.WeixinMessageState == EnableState.Disable);
                ResultCount = ResultCount.Filter(t => t.WeixinMessageState == EnableState.Disable);
            }
            if (CountryID > 0)
            {
                Result = Result.Filter(t => t.CountryID == CountryID);
                ResultCount = ResultCount.Filter(t => t.CountryID == CountryID);
            }
            if (!string.IsNullOrEmpty(FuzzySearch))
            {
                Result = Result.Filter(t => t.Message.Contains(FuzzySearch));
                ResultCount = ResultCount.Filter(t => t.Message.Contains(FuzzySearch));
            }
            int count = await ResultCount.Count().FindScalarAsync<int>();
            var weixinMessages = await Result.OrderBy(t => t.CountryID).Skip(start).Top(length).FindEntriesAsync();

            return JsonConvert.SerializeObject(new { draw = draw, recordsFiltered = count, data = weixinMessages, SearchModel = search });
        }
        //启用禁用
        [HttpPost]
        public async Task<string> UpdateDisable(string id, string Operation)
        {
            var failed = (new int[] { 1 }).Select(x => new { name = "", reason = "" }).ToList();
            failed.Clear();
            if (string.IsNullOrEmpty(id))
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "供应商不能为空！" });
            }
            if (string.IsNullOrEmpty(Operation))
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "操作异常！" });
            }
            try
            {
                var WeixinMessageID = id.Split(',');
                if (Operation.Trim() == "0")
                {
                    foreach (var i in WeixinMessageID)
                    {
                        var old = await client.For<WeixinMessage>().Key(int.Parse(i)).FindEntryAsync();
                        old.WeixinMessageState = EnableState.Enable;
                        await client.For<WeixinMessage>().Key(int.Parse(i)).Set(old).UpdateEntryAsync();
                    }
                }
                else if (Operation.Trim() == "1")
                {
                    foreach (var i in WeixinMessageID)
                    {
                        var old = await client.For<WeixinMessage>().Key(int.Parse(i)).FindEntryAsync();
                        old.WeixinMessageState = EnableState.Disable;
                        await client.For<WeixinMessage>().Key(int.Parse(i)).Set(old).UpdateEntryAsync();
                    }
                }
                else if (Operation.Trim().ToLower() == "delete")
                {
                    foreach (var i in WeixinMessageID)
                    {
                        await client.For<WeixinMessage>().Key(int.Parse(i)).DeleteEntryAsync();
                    }
                }
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
        // GET: WeixinMessages
        public async Task<ActionResult> Index()
        {
            ViewBag.Country = await client.For<Country>().Filter(c => c.Suppliers.Any(s => s.SupplierEnableState == EnableState.Enable)).FindEntriesAsync();
            return View();
        }
        // GET: WeixinMessages/Create
        public async Task<ActionResult> Create()
        {
            ViewBag.CountryID = new SelectList(await client.For<Country>().Filter(c => c.Suppliers.Any(s => s.SupplierEnableState == EnableState.Enable)).FindEntriesAsync(), "CountryID", "CountryName");
            return View();
        }

        // POST: WeixinMessages/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "StartTime,EndTime,CountryID,Message,Url")] WeixinMessage weixinMessage)
        {
            if (ModelState.IsValid)
            {
                var message = await client.For<WeixinMessage>()
                    .Filter(w => w.CountryID == weixinMessage.CountryID)
                    .Filter(w => (w.StartTime <= weixinMessage.StartTime && weixinMessage.StartTime <= w.EndTime) || (w.StartTime <= weixinMessage.EndTime && weixinMessage.EndTime <= w.EndTime) || (weixinMessage.StartTime <= w.StartTime && w.StartTime <= weixinMessage.EndTime) || (weixinMessage.StartTime <= w.EndTime && w.EndTime <= weixinMessage.EndTime))
                    .FindEntryAsync();

                if (message != null)
                {
                    ModelState.AddModelError("StartTime", "同一个国家日期范围不能重叠");
                }
                else
                {
                    string userName = User.Identity.Name;
                    User user = await client.For<User>().Filter(u => u.UserName == userName).FindEntryAsync();

                    weixinMessage.OperUserID = user.UserID;
                    weixinMessage.OperUserNickName = user.NickName;
                    weixinMessage.LastEditDate = DateTime.Now;
                    weixinMessage.Url = weixinMessage.Url == null ? "" : ("http://" + weixinMessage.Url.Replace("https://", "").Replace("http://", ""));
                    await client.For<WeixinMessage>().Set(weixinMessage).InsertEntryAsync();
                    return RedirectToAction("Index");
                }
            }

            ViewBag.CountryID = new SelectList(await client.For<Country>().Filter(c => c.Suppliers.Any(s => s.SupplierEnableState == EnableState.Enable)).FindEntriesAsync(), "CountryID", "CountryName", weixinMessage.CountryID);
            return View(weixinMessage);
        }

        // GET: WeixinMessages/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WeixinMessage weixinMessage = await client.For<WeixinMessage>().Key(id).FindEntryAsync();
            if (weixinMessage == null)
            {
                return HttpNotFound();
            }
            ViewBag.CountryID = new SelectList(await client.For<Country>().Filter(c => c.Suppliers.Any(s => s.SupplierEnableState == EnableState.Enable)).FindEntriesAsync(), "CountryID", "CountryName", weixinMessage.CountryID);
            return View(weixinMessage);
        }

        // POST: WeixinMessages/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "WeixinMessageID,StartTime,EndTime,WeixinMessageState,CountryID,Message,Url")] WeixinMessage weixinMessage)
        {
            if (ModelState.IsValid)
            {
                var message = await client.For<WeixinMessage>()
                    .Filter(w => w.CountryID == weixinMessage.CountryID)
                    .Filter(w => w.WeixinMessageID != weixinMessage.WeixinMessageID)
                    .Filter(w => (w.StartTime <= weixinMessage.StartTime && weixinMessage.StartTime <= w.EndTime) || (w.StartTime <= weixinMessage.EndTime && weixinMessage.EndTime <= w.EndTime) || (weixinMessage.StartTime <= w.StartTime && w.StartTime <= weixinMessage.EndTime) || (weixinMessage.StartTime <= w.EndTime && w.EndTime <= weixinMessage.EndTime))
                    .FindEntryAsync();

                if (message != null)
                {
                    ModelState.AddModelError("StartTime", "同一个国家日期范围不能重叠");
                }
                else
                {
                    string userName = User.Identity.Name;
                    User user = await client.For<User>().Filter(u => u.UserName == userName).FindEntryAsync();

                    weixinMessage.OperUserID = user.UserID;
                    weixinMessage.OperUserNickName = user.NickName;
                    weixinMessage.LastEditDate = DateTime.Now;
                    weixinMessage.Url = weixinMessage.Url == null ? "" : ("http://" + weixinMessage.Url.Replace("http://", "").Replace("https://", ""));
                    await client.For<WeixinMessage>().Key(weixinMessage.WeixinMessageID).Set(weixinMessage).UpdateEntryAsync();
                    return RedirectToAction("Index");
                }
            }
            ViewBag.CountryID = new SelectList(await client.For<Country>().Filter(c => c.Suppliers.Any(s => s.SupplierEnableState == EnableState.Enable)).FindEntriesAsync(), "CountryID", "CountryName", weixinMessage.CountryID);
            return View(weixinMessage);
        }

    }
}
