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
using Entity;

namespace LanghuaNew.Controllers
{
    public class DistributionTalliesController : Controller
    {
        private ODataClient client = new ODataClient(ConfigurationManager.AppSettings["ServicePath"] + "odata/");

        // GET: DistributionTallies
        public ActionResult Index(string search)
        {
            ViewBag.search = search;
            return View();
        }
        public async Task<string> GetDistributionTallies(ShareSearchModel share)
        {
            int draw = 1;
            int start = 0;
            int length = 50;
            int SupplierID = 0;
            string ItemName = string.Empty;
            string TravelDate = string.Empty;
            string FuzzySearch = string.Empty;
            if (share.length > 0)
            {
                draw = share.draw;
                start = share.start;
                length = share.length;
            }
            if (share.DistributionTallySearch != null)
            {
                SupplierID = share.DistributionTallySearch.SupplierID;
                ItemName = share.DistributionTallySearch.ItemName;
                TravelDate = share.DistributionTallySearch.TravelDateBegin;
                FuzzySearch = share.DistributionTallySearch.FuzzySearch;
            }
            var Result = client.For<DistributionTally>().Expand(s => s.serviceItem).Expand(s => s.supplier);
            var ResultCount = client.For<DistributionTally>();
            if (SupplierID > 0)
            {
                string filter = "SupplierID eq " + SupplierID;
                Result = Result.Filter(filter);
                ResultCount = ResultCount.Filter(filter);
            }
            if (!string.IsNullOrEmpty(ItemName))
            {//产品，以空格分开
                string[] name = ItemName.Trim().Split(' ');
                string filter = "";
                foreach (var item in name)
                {
                    if (filter != "")
                    {
                        filter += " or ";
                    }
                    filter += "serviceItem/ServiceCode eq '" + item + "'";
                }
                Result = Result.Filter(filter);
                ResultCount = ResultCount.Filter(filter);
            }
            if (!string.IsNullOrEmpty(FuzzySearch))
            {
                string filter = "contains(GroupNo,'" + FuzzySearch.Trim() + "')";
                Result = Result.Filter(filter);
                ResultCount = ResultCount.Filter(filter);
            }
            if (!string.IsNullOrEmpty(TravelDate))
            {
                string filter = "TravelDate eq " + TravelDate.Trim();
                Result = Result.Filter(filter);
                ResultCount = ResultCount.Filter(filter);
            }
            int count = await ResultCount.Count().FindScalarAsync<int>();
            var distributionTallies = await Result.OrderByDescending(s => s.DistributionTallyID).Skip(start).Top(length).FindEntriesAsync();
            var data = distributionTallies.Select(s => new
            {
                s.DistributionTallyID,
                TravelDate = s.TravelDate.ToString("yyyy-MM-dd"),
                s.Remark,
                CreateTime = s.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                s.CreateUserNikeName,
                s.ServiceItemID,
                s.serviceItem.ServiceTypeID,
                s.serviceItem.cnItemName,
                s.serviceItem.ServiceCode,
                s.SupplierID,
                s.supplier.SupplierNo,
                s.supplier.SupplierName,
                s.AdultNum,
                s.ChildNum,
                s.INFNum,
                s.GroupNo,
                s.RoomNum,
                s.RightNum,
                s.IsCancel
            });
            return JsonConvert.SerializeObject(new { draw = draw, recordsFiltered = count, data = data, SearchModel = share });
        }
        public async Task<string> Create(int? SupplierID, int? ItemID, DateTimeOffset TravelDate, string GroupNo, int AdultNum = 0, int ChildNum = 0, int INFNum = 0, int RoomNum = 0, int RightNum = 0)
        {
            if (SupplierID == null || SupplierID == 0)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "供应商不能为空" });
            }
            if (ItemID == null || ItemID == 0)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "产品不能为空" });
            }
            if (TravelDate < DateTimeOffset.Parse("1901-01-01"))
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "出行日期不能为空" });
            }
            if (string.IsNullOrEmpty(GroupNo))
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "团号不能为空" });
            }
            if (AdultNum + ChildNum + INFNum == 0)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "人数需大于0" });
            }
            DateTimeOffset ReturnDate = TravelDate.AddDays(0);
            try
            {
                ServiceItem serviceItem = await client.For<ServiceItem>().Key(ItemID).FindEntryAsync();
                if (serviceItem.ServiceTypeID == 4)
                {
                    if (RoomNum * RightNum == 0)
                    {
                        return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "酒店类型间数晚数需大于0" });
                    }
                    ReturnDate = TravelDate.AddDays(RightNum - 1);
                }
                else
                {
                    RoomNum = 0;
                    RightNum = 0;
                }
            }
            catch
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "找不到该产品" });
            }
            string userName = User.Identity.Name;
            User user = await client.For<User>().Filter(u => u.UserName == userName).FindEntryAsync();
            try
            {
                DistributionTally distributionTally = new DistributionTally();
                distributionTally.CreateTime = DateTimeOffset.Now;
                distributionTally.CreateUserID = user.UserID;
                distributionTally.CreateUserNikeName = user.NickName;
                distributionTally.SupplierID = int.Parse(SupplierID.ToString());
                distributionTally.ServiceItemID = int.Parse(ItemID.ToString());
                distributionTally.TravelDate = TravelDate;
                distributionTally.GroupNo = GroupNo;
                distributionTally.AdultNum = AdultNum;
                distributionTally.ChildNum = ChildNum;
                distributionTally.INFNum = INFNum;
                distributionTally.RoomNum = RoomNum;
                distributionTally.RightNum = RightNum;
                distributionTally.ReturnDate = ReturnDate;
                await client.For<DistributionTally>().Set(distributionTally).InsertEntryAsync();
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 400, ErrorMessage = "添加失败" + ex.Message });
            }
            return JsonConvert.SerializeObject(new { ErrorCode = 200, ErrorMessage = "OK" });
        }
        public async Task<string> Edit(int? DistributionTallyID, int? SupplierID, int? ItemID, DateTimeOffset TravelDate, string GroupNo, int AdultNum = 0, int ChildNum = 0, int INFNum = 0, int RoomNum = 0, int RightNum = 0)
        {
            if (DistributionTallyID == null || DistributionTallyID == 0)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "ID不能为空" });
            }
            if (SupplierID == null || SupplierID == 0)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "供应商不能为空" });
            }
            if (ItemID == null || ItemID == 0)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "产品不能为空" });
            }
            if (TravelDate < DateTimeOffset.Parse("1901-01-01"))
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "出行日期不能为空" });
            }
            if (string.IsNullOrEmpty(GroupNo))
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "团号不能为空" });
            }
            if (AdultNum + ChildNum + INFNum <= 0)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "人数需大于0" });
            }
            DateTimeOffset ReturnDate = TravelDate.AddDays(0);
            try
            {
                ServiceItem serviceItem = await client.For<ServiceItem>().Key(ItemID).FindEntryAsync();
                if (serviceItem.ServiceTypeID == 4)
                {
                    if (RoomNum * RightNum == 0)
                    {
                        return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "酒店类型间数晚数需大于0" });
                    }
                    ReturnDate = TravelDate.AddDays(RightNum - 1);
                }
                else
                {
                    RoomNum = 0;
                    RightNum = 0;
                }
            }
            catch
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "找不到该产品" });
            }
            string userName = User.Identity.Name;
            User user = await client.For<User>().Filter(u => u.UserName == userName).FindEntryAsync();
            try
            {
                DistributionTally distributionTally = await client.For<DistributionTally>().Key(DistributionTallyID).FindEntryAsync();
                distributionTally.SupplierID = int.Parse(SupplierID.ToString());
                distributionTally.ServiceItemID = int.Parse(ItemID.ToString());
                distributionTally.TravelDate = TravelDate;
                distributionTally.GroupNo = GroupNo;
                distributionTally.AdultNum = AdultNum;
                distributionTally.ChildNum = ChildNum;
                distributionTally.INFNum = INFNum;
                distributionTally.RoomNum = RoomNum;
                distributionTally.RightNum = RightNum;
                distributionTally.ReturnDate = ReturnDate;
                await client.For<DistributionTally>().Key(DistributionTallyID).Set(distributionTally).UpdateEntryAsync();
                await client.For<SystemLog>().Set(new
                {
                    Operate = "修改分销统计",
                    OperateTime = DateTime.Now,
                    UserID = user.UserID,
                    UserName = user.NickName,
                    Remark = "Message：" + JsonConvert.SerializeObject(distributionTally)
                }).InsertEntryAsync();
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 400, ErrorMessage = "修改失败" + ex.Message });
            }
            return JsonConvert.SerializeObject(new { ErrorCode = 200, ErrorMessage = "OK" });
        }
        public async Task<string> UpdateDisable(string DistributionTallyID, string Operation)
        {
            if (string.IsNullOrEmpty(DistributionTallyID))
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "ID不能为空" });
            }
            if (string.IsNullOrEmpty(Operation))
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "操作不能为空" });
            }
            try
            {
                if (Operation.Trim() == "Cancel")
                {
                    foreach (var id in DistributionTallyID.Split(','))
                    {
                        DistributionTally distributionTally = await client.For<DistributionTally>().Key(int.Parse(id)).FindEntryAsync();
                        if (!distributionTally.IsCancel)
                        {
                            distributionTally.IsCancel = true;
                            await client.For<DistributionTally>().Key(int.Parse(id)).Set(distributionTally).UpdateEntryAsync();
                        }
                    }
                }
                else if (Operation.Trim() == "NotCancel")
                {
                    foreach (var id in DistributionTallyID.Split(','))
                    {
                        DistributionTally distributionTally = await client.For<DistributionTally>().Key(int.Parse(id)).FindEntryAsync();
                        if (distributionTally.IsCancel)
                        {
                            distributionTally.IsCancel = false;
                            await client.For<DistributionTally>().Key(int.Parse(id)).Set(distributionTally).UpdateEntryAsync();
                        }
                    }
                }
                else if (Operation.Trim().ToLower() == "delete")
                {
                    foreach (var id in DistributionTallyID.Split(','))
                    {
                        await client.For<DistributionTally>().Key(int.Parse(id)).DeleteEntryAsync();
                    }
                }
                else
                {
                    return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "操作状态码有误" });
                }
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 400, ErrorMessage = "操作失败" + ex.Message });
            }
            string userName = User.Identity.Name;
            User user = await client.For<User>().Filter(u => u.UserName == userName).FindEntryAsync();
            await client.For<SystemLog>().Set(new
            {
                Operate = "操作分销统计",
                OperateTime = DateTime.Now,
                UserID = user.UserID,
                UserName = user.NickName,
                Remark = "DistributionTallyID：" + DistributionTallyID + "Operation：" + Operation
            }).InsertEntryAsync();
            return JsonConvert.SerializeObject(new { ErrorCode = 200, ErrorMessage = "OK" });
        }
    }
}
