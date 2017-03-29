using System;
using System.Threading.Tasks;
using System.Net;
using System.Web.Mvc;
using LanghuaNew.Data;
using Simple.OData.Client;
using System.Configuration;
using LanghuaNew.Models;
using System.Linq;
using Newtonsoft.Json;
using System.Collections.Generic;
using Commond;

namespace LanghuaNew.Controllers
{
    [Authorize]
    public class RolesController : Controller
    {
        private ODataClient client = new ODataClient(ConfigurationManager.AppSettings["ServicePath"] + "odata/");

        // GET: Roles
        public ActionResult Index()
        {
            return View();
        }
        // GET: Roles/Create
        public async Task<ActionResult> Create()
        {
            var MenuRights = await client.For<MenuRight>().FindEntriesAsync();
            ViewBag.MenuRights = MenuRights;
            return View();
        }
        // GET: Roles/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Role role = await client.For<Role>().Expand(s => s.MenuRights).Key(id).FindEntryAsync();
            if (role == null)
            {
                return HttpNotFound();
            }
            var MenuRights = await client.For<MenuRight>().FindEntriesAsync();
            ViewBag.MenuRights = MenuRights;
            return View(role);
        }
        public async Task<string> GetRoles(SearchModel search)
        {
            int draw = 1;
            int start = 0;
            int length = 50;
            if (search.length > 0)
            {
                draw = search.draw;
                start = search.start;
                length = search.length;
            }
            var Result = client.For<Role>().Expand(s => s.Users);
            var ResultCount = client.For<Role>();
            int count = await ResultCount.Count().FindScalarAsync<int>();
            var Roles = await Result.Skip(start).Top(length).FindEntriesAsync();
            var data = Roles.Select(s => new
            {
                s.RoleID,
                s.RoleName,
                s.RoleRemark,
                s.RoleEnableState,
                Users = s.Users != null ? s.Users.Select(u => u.NickName) : null,
            });
            return JsonConvert.SerializeObject(new { draw = draw, recordsFiltered = count, data = data, SearchModel = search });
        }
        [HttpPost]
        public async Task<string> Save(int? RoleID, string RoleName, string Remark, string RightID)
        {
            if (string.IsNullOrEmpty(RoleName))
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "角色名称不能为空！" });
            }
            if (string.IsNullOrEmpty(RightID))
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "权限选择不能为空！" });
            }
            if (RoleID == null || RoleID == 0)
            {
                if (await client.For<Role>().Filter(s => s.RoleName == RoleName).FindEntryAsync() != null)
                {
                    return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "角色名称不能重复！" });
                }
                Role role = new Role();
                role.RoleName = RoleName;
                role.RoleRemark = Remark;
                role.CreateTime = DateTimeOffset.Now;
                role.MenuRights = new List<MenuRight>();
                foreach (var item in RightID.Split(','))
                {
                    if (!string.IsNullOrEmpty(item))
                    {
                        role.MenuRights.Add(new MenuRight { MenuRightID = int.Parse(item.Trim()) });
                    }
                }
                await HttpHelper.PostAction("RolesExtend", JsonConvert.SerializeObject(role));
            }
            else
            {
                if (RoleID == 1)
                {
                    return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "超级管理员不能修改哦！" });
                }
                if (await client.For<Role>().Filter(s => s.RoleName == RoleName && s.RoleID != RoleID).FindEntryAsync() != null)
                {
                    return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "角色名称不能重复！" });
                }
                Role role = await client.For<Role>().Key(RoleID).FindEntryAsync();
                role.RoleName = RoleName;
                role.RoleRemark = Remark;
                role.MenuRights = new List<MenuRight>();
                foreach (var item in RightID.Split(','))
                {
                    if (!string.IsNullOrEmpty(item))
                    {
                        role.MenuRights.Add(new MenuRight { MenuRightID = int.Parse(item.Trim()) });
                    }
                }
                await HttpHelper.PutAction("RolesExtend", JsonConvert.SerializeObject(role));
            }
            return JsonConvert.SerializeObject(new { ErrorCode = 200, ErrorMessage = "OK" });
        }
        //启用禁用
        [HttpPost]
        public async Task<string> UpdateDisable(string RoleID, string Operation)
        {
            var failed = (new int[] { 1 }).Select(x => new { name = "", reason = "" }).ToList();
            failed.Clear();
            if (string.IsNullOrEmpty(RoleID))
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "角色不能为空！" });
            }
            if (string.IsNullOrEmpty(Operation))
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "操作异常！" });
            }
            try
            {
                var id = RoleID.Split(',');
                if (Operation.Trim() == "0")
                {
                    foreach (var i in id)
                    {
                        var oldRole = await client.For<Role>().Key(int.Parse(i)).FindEntryAsync();
                        if (oldRole.RoleEnableState != EnableState.Enable)
                        {
                            oldRole.RoleEnableState = EnableState.Enable;
                            await client.For<Role>().Key(int.Parse(i)).Set(oldRole).UpdateEntryAsync();
                        }
                    }
                }
                else if (Operation.Trim() == "1")
                {
                    foreach (var i in id)
                    {
                        var oldRole = await client.For<Role>().Key(int.Parse(i)).FindEntryAsync();
                        if (int.Parse(i) == 1)
                        {
                            failed.Add(new { name = oldRole.RoleName, reason = "超级管理员不能禁用哦！" });
                        }
                        else
                        {
                            if (oldRole.RoleEnableState != EnableState.Disable)
                            {
                                oldRole.RoleEnableState = EnableState.Disable;
                                await client.For<Role>().Key(int.Parse(i)).Set(oldRole).UpdateEntryAsync();
                            }
                        }
                    }
                }
                else if (Operation.Trim().ToLower() == "delete")
                {
                    foreach (var i in id)
                    {
                        var oldRole = await client.For<Role>().Expand(s => s.Users).Key(int.Parse(i)).FindEntryAsync();
                        if (int.Parse(i) == 1)
                        {
                            failed.Add(new { name = oldRole.RoleName, reason = "超级管理员不能删除哦！" });
                        }
                        else
                        {
                            if (oldRole.Users != null && oldRole.Users.Count > 0)
                            {
                                failed.Add(new { name = oldRole.RoleName, reason = "删除失败！需清空该角色成员才能删除！" });
                            }
                            else
                            {
                                await client.For<Role>().Key(int.Parse(i)).DeleteEntryAsync();
                            }
                        }
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
    }
}
