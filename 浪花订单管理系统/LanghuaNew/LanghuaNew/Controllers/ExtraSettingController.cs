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

namespace LanghuaNew.Controllers
{
    public class ExtraSettingController : Controller
    {
        private ODataClient client = new ODataClient(ConfigurationManager.AppSettings["ServicePath"] + "odata/");
        // GET: ExtraSetting
        public async Task<string> Index(int? sellControlID)
        {
            if (sellControlID == null && sellControlID == 0)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "控位编号不能为空" });
            }
            var list = await client.For<ExtraSetting>().Filter(d => d.SellControlID == sellControlID).FindEntriesAsync();
            return JsonConvert.SerializeObject(new { ErrorCode = 200, ErrorMessage = "OK", data = list });
        }
        [HttpPost]
        public async Task<string> Update(ExtraSetting extraSetting)
        {
            if (extraSetting.ExtraSettingID == 0)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "特殊设置记录不能为空" });
            }
            if (extraSetting.SellControlID == 0)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "控位记录不能为空" });
            }
            if (extraSetting.StartTime < DateTimeOffset.Parse("1901-01-01"))
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "开始时间不能为空" });
            }
            if (extraSetting.EndTime < DateTimeOffset.Parse("1901-01-01"))
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "结束时间不能为空" });
            }
            if (string.IsNullOrEmpty(extraSetting.Remark))
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "原因不能为空" });
            }
            ExtraSetting one = await client.For<ExtraSetting>()
                .Filter(t => t.ExtraSettingID != extraSetting.ExtraSettingID)
                .Filter(t => t.SellControlID == extraSetting.SellControlID)
                .Filter(t => (extraSetting.StartTime <= t.StartTime && t.StartTime <= extraSetting.EndTime) || (extraSetting.StartTime <= t.EndTime && t.EndTime <= extraSetting.EndTime) || (t.StartTime <= extraSetting.StartTime && extraSetting.StartTime <= t.EndTime) || (t.StartTime <= extraSetting.EndTime && extraSetting.EndTime <= t.EndTime))
                .FindEntryAsync();
            if (one != null)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "特殊设置时间跨度不能重叠" });
            }

            await client.For<ExtraSetting>().Key(extraSetting.ExtraSettingID).Set(extraSetting).UpdateEntryAsync();
            string userName = User.Identity.Name;
            User OperUser = await client.For<User>().Filter(u => u.UserName == userName).FindEntryAsync();
            //操作记录
            await client.For<SystemLog>().Set(new { Operate = "更新保存控位特殊设置", OperateTime = DateTime.Now, UserID = OperUser.UserID, UserName = OperUser.NickName, Remark = extraSetting.SellControlID }).InsertEntryAsync();

            return JsonConvert.SerializeObject(new { ErrorCode = 200, ErrorMessage = "OK" });
        }
        [HttpPost]
        public async Task<string> Insert(ExtraSetting extraSetting)
        {
            if (extraSetting.SellControlID == 0)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "控位记录不能为空" });
            }
            if (extraSetting.StartTime < DateTimeOffset.Parse("1901-01-01"))
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "开始时间不能为空" });
            }
            if (extraSetting.EndTime < DateTimeOffset.Parse("1901-01-01"))
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "结束时间不能为空" });
            }
            if (string.IsNullOrEmpty(extraSetting.Remark))
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "原因不能为空" });
            }
            ExtraSetting one = await client.For<ExtraSetting>()
                .Filter(t => t.SellControlID == extraSetting.SellControlID)
                .Filter(t => (extraSetting.StartTime <= t.StartTime && t.StartTime <= extraSetting.EndTime) || (extraSetting.StartTime <= t.EndTime && t.EndTime <= extraSetting.EndTime) || (t.StartTime <= extraSetting.StartTime && extraSetting.StartTime <= t.EndTime) || (t.StartTime <= extraSetting.EndTime && extraSetting.EndTime <= t.EndTime))
                .FindEntryAsync();
            if (one != null)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "特殊设置时间跨度不能重叠" });
            }

            extraSetting = await client.For<ExtraSetting>().Set(extraSetting).InsertEntryAsync();

            string userName = User.Identity.Name;
            User OperUser = await client.For<User>().Filter(u => u.UserName == userName).FindEntryAsync();
            //操作记录
            await client.For<SystemLog>().Set(new { Operate = "新增控位特殊设置", OperateTime = DateTime.Now, UserID = OperUser.UserID, UserName = OperUser.NickName, Remark = extraSetting.SellControlID }).InsertEntryAsync();

            return JsonConvert.SerializeObject(new { ErrorCode = 200, ErrorMessage = "OK", Data = extraSetting });
        }
        [HttpPost]
        public async Task<string> Delete(int? extraSettingID)
        {
            if (extraSettingID == null || extraSettingID <= 0)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "特殊设置记录不能为空" });
            }
            await client.For<ExtraSetting>().Filter(c => c.ExtraSettingID == extraSettingID).DeleteEntryAsync();
            string userName = User.Identity.Name;
            User OperUser = await client.For<User>().Filter(u => u.UserName == userName).FindEntryAsync();
            //操作记录
            await client.For<SystemLog>().Set(new { Operate = "删除控位特殊设置", OperateTime = DateTime.Now, UserID = OperUser.UserID, UserName = OperUser.NickName, Remark = extraSettingID }).InsertEntryAsync();

            return JsonConvert.SerializeObject(new { ErrorCode = 200, ErrorMessage = "OK" });
        }

    }
}