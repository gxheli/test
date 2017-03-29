using AutoMapper;
using Commond;
using Entity;
using LanghuaForSup.Models;
using LanghuaNew.Data;
using Newtonsoft.Json;
using Simple.OData.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace LanghuaForSup.Controllers
{
    public class UsersController : Controller
    {
        private ODataClient client = new ODataClient(ConfigurationManager.AppSettings["ServicePath"] + "odata/");
        // GET: Users
        public ActionResult Index()
        {
            return View();
        }
        // GET: Users/Create
        public async Task<ActionResult> Create()
        {
            string userName = User.Identity.Name;
            SupplierUser user = await client.For<SupplierUser>().Expand(s => s.OneSupplier).Filter(u => u.SupplierUserName == userName).FindEntryAsync();
            IBoundClient<SupplierRole> clientSearch = client.For<SupplierRole>().Filter(s => s.SupplierID == user.SupplierID || s.SupplierRoleID == 1);
            UserViewModel userModel = new UserViewModel();
            userModel.AllRole = new List<SupplierRole>();
            userModel.AllRole.AddRange(await clientSearch.FindEntriesAsync());
            userModel.SupplierRoles = new List<SupplierRole>();
            userModel.OneSupplier = new Supplier();
            userModel.OneSupplier.SupplierNo = user.OneSupplier.SupplierNo;
            return View(userModel);
        }
        //新增用户
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "SupplierUserName,PassWord,SupplierNickName")] SupplierUser user)
        {
            string userName = User.Identity.Name;
            SupplierUser OperUser = await client.For<SupplierUser>().Expand(s => s.OneSupplier).Filter(u => u.SupplierUserName == userName).FindEntryAsync();

            if (ModelState.IsValid)
            {
                if (!string.IsNullOrEmpty(Request.Form["SupplierRoles"]))
                {
                    var SelectRole = new HashSet<string>(Request.Form["SupplierRoles"].Split(','));


                    foreach (var RoleID in SelectRole)
                    {
                        SupplierRole NewRole = new SupplierRole();
                        NewRole.SupplierRoleID = int.Parse(RoleID);
                        user.SupplierRoles = new List<SupplierRole>();
                        user.SupplierRoles.Add(NewRole);
                    }
                }
                user.SupplierUserName = user.SupplierUserName.Trim() + "@" + OperUser.OneSupplier.SupplierNo;

                SupplierUser SameUser = await client.For<SupplierUser>().Filter(p => p.SupplierUserName == user.SupplierUserName).FindEntryAsync();
                //用户名不能重复
                if (SameUser != null)
                {
                    ModelState.AddModelError("SupplierUserName", "用户名不能重复");
                }
                else
                {
                    user.CreateTime = DateTime.Now;
                    user.PassWord = Md5Hash(user.PassWord);
                    user.SupplierID = OperUser.SupplierID;
                    user.RealTimeMessage = true;
                    user.Disturb = true;
                    user.BeginTime = "00:00";
                    user.EndTime = "08:00";
                    HttpResponseMessage Message = await HttpHelper.PostAction("SupplierUsersExtend", JsonConvert.SerializeObject(user));
                    string id = Message.Content.ReadAsStringAsync().Result;
                    try
                    {
                        //操作记录
                        await client.For<SupplierUserLog>().Set(new SupplierUserLog
                        {
                            OperSupplierUserID = OperUser.SupplierUserID,
                            OperSupplierNickName = OperUser.SupplierNickName,
                            OperTime = DateTime.Now,
                            Remark = "",
                            Operate = UserOperate.Add,
                            SupplierUserID = int.Parse(id),
                            SupplierUserName = user.SupplierUserName,
                            SupplierNickName = user.SupplierNickName
                        }).InsertEntryAsync();

                        return RedirectToAction("Index");
                    }
                    catch
                    {

                    }
                }
            }
            var config = new MapperConfiguration(cfg => cfg.CreateMap<SupplierUser, UserViewModel>());
            var mapper = config.CreateMapper();
            IBoundClient<SupplierRole> clientSearch = client.For<SupplierRole>().Filter(s => s.SupplierID == OperUser.SupplierID || s.SupplierRoleID == 1);
            UserViewModel userModel = mapper.Map<UserViewModel>(user);
            userModel.AllRole = new List<SupplierRole>();
            userModel.AllRole.AddRange(await clientSearch.FindEntriesAsync());
            userModel.SupplierRoles = userModel.SupplierRoles == null ? new List<SupplierRole>() : userModel.SupplierRoles;
            userModel.OneSupplier = new Supplier();
            userModel.OneSupplier.SupplierNo = OperUser.OneSupplier.SupplierNo;
            return View(userModel);
        }
        // GET: Users/Create
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            string userName = User.Identity.Name;
            SupplierUser user = await client.For<SupplierUser>().Filter(u => u.SupplierUserName == userName).FindEntryAsync();

            SupplierUser userResult = await client.For<SupplierUser>().Filter(s => s.SupplierUserID == id && s.SupplierID == user.SupplierID && !s.IsMaster).Expand(x => x.SupplierRoles).FindEntryAsync();
            if (userResult == null)
            {
                return HttpNotFound();
            }
            //将linq对象映射到实体对象
            var config = new MapperConfiguration(cfg => cfg.CreateMap<SupplierUser, UserViewModel>());
            var mapper = config.CreateMapper();
            UserViewModel userModel = mapper.Map<UserViewModel>(userResult);
            if (userModel == null)
            {
                return HttpNotFound();
            }
            IBoundClient<SupplierRole> clientSearch = client.For<SupplierRole>().Filter(s => s.SupplierID == user.SupplierID || s.SupplierRoleID == 1);
            userModel.AllRole = new List<SupplierRole>();
            userModel.AllRole.AddRange(await clientSearch.FindEntriesAsync());
            if (userModel.SupplierRoles == null)
            {
                userModel.SupplierRoles = new List<SupplierRole>();
            }
            return View(userModel);
        }
        //修改用户
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "SupplierUserID,SupplierNickName")] SupplierUser user)
        {
            if (!string.IsNullOrEmpty(Request.Form["SupplierRoles"]))
            {
                var SelectRole = new HashSet<string>(Request.Form["SupplierRoles"].Split(','));
                user.SupplierRoles = new List<SupplierRole>();
                foreach (var RoleID in SelectRole)
                {
                    SupplierRole NewRole = new SupplierRole();
                    NewRole.SupplierRoleID = int.Parse(RoleID);
                    user.SupplierRoles.Add(NewRole);
                }
            }
            string userName = User.Identity.Name;
            SupplierUser OperUser = await client.For<SupplierUser>().Filter(u => u.SupplierUserName == userName).FindEntryAsync();
            if (ModelState.IsValidField("SupplierUserID") && ModelState.IsValidField("SupplierNickName"))
            {
                var olduser = await client.For<SupplierUser>().Key(user.SupplierUserID).FindEntryAsync();
                if (olduser.SupplierID != OperUser.SupplierID)
                {
                    return HttpNotFound();
                }
                olduser.SupplierNickName = user.SupplierNickName;
                olduser.SupplierRoles = user.SupplierRoles;
                await HttpHelper.PutAction("SupplierUsersExtend", JsonConvert.SerializeObject(olduser));
                //操作记录
                await client.For<SupplierUserLog>().Set(new SupplierUserLog
                {
                    OperSupplierUserID = OperUser.SupplierUserID,
                    OperSupplierNickName = OperUser.SupplierNickName,
                    OperTime = DateTime.Now,
                    Remark = "",
                    Operate = UserOperate.Edit,
                    SupplierUserID = olduser.SupplierUserID,
                    SupplierUserName = user.SupplierUserName,
                    SupplierNickName = user.SupplierNickName
                }).InsertEntryAsync();

                return RedirectToAction("Index");

            }
            IBoundClient<SupplierRole> clientSearch = client.For<SupplierRole>().Filter(s => s.SupplierID == OperUser.SupplierID || s.SupplierRoleID == 1);
            var config = new MapperConfiguration(cfg => cfg.CreateMap<SupplierUser, UserViewModel>());
            UserViewModel userModel = config.CreateMapper().Map<UserViewModel>(user);
            userModel.AllRole = new List<SupplierRole>();
            userModel.AllRole.AddRange(await clientSearch.FindEntriesAsync());
            if (userModel.SupplierRoles == null)
            {
                userModel.SupplierRoles = new List<SupplierRole>();
            }
            return View(userModel);
        }
        //获取用户列表
        public async Task<string> GetUsers(ShareSearchModel langhua)
        {
            string SupplierUserName = User.Identity.Name;
            SupplierUser user = await client.For<SupplierUser>()
                .Filter(s => s.SupplierUserName == SupplierUserName)
                .FindEntryAsync();

            int draw = 1;
            int start = 0;
            int length = 10;
            if (langhua.length > 0)
            {
                draw = langhua.draw;
                start = langhua.start;
                length = langhua.length;
            }
            int status = 0;
            string FuzzySearch = string.Empty;
            if (langhua.UsersSearch != null)
            {
                status = langhua.UsersSearch.status;
                FuzzySearch = langhua.UsersSearch.FuzzySearch;
            }

            IBoundClient<SupplierUser> Result = client.For<SupplierUser>()
                .Expand(s => s.SupplierRoles)
                .OrderByDescending(s => s.SupplierUserID).Skip(start).Top(length);
            IBoundClient<SupplierUser> ResultCount = client.For<SupplierUser>();

            Result = Result.Filter(s => s.SupplierID == user.SupplierID && !s.IsMaster);
            ResultCount = ResultCount.Filter(s => s.SupplierID == user.SupplierID && !s.IsMaster);

            if (status == 1)
            {
                Result = Result.Filter(s => s.SupplierUserEnableState == EnableState.Enable);
                ResultCount = ResultCount.Filter(s => s.SupplierUserEnableState == EnableState.Enable);
            }
            if (status == 2)
            {
                Result = Result.Filter(s => s.SupplierUserEnableState == EnableState.Disable);
                ResultCount = ResultCount.Filter(s => s.SupplierUserEnableState == EnableState.Disable);
            }
            if (!string.IsNullOrEmpty(FuzzySearch))
            {
                Result = Result.Filter(s => s.SupplierUserName.Contains(FuzzySearch) || s.SupplierNickName.Contains(FuzzySearch));
                ResultCount = ResultCount.Filter(s => s.SupplierUserName.Contains(FuzzySearch) || s.SupplierNickName.Contains(FuzzySearch));
            }
            int count = await ResultCount.Count().FindScalarAsync<int>();
            var list = await Result.FindEntriesAsync();
            var data = list.Select(s => new
            {
                s.SupplierUserID,
                s.SupplierUserName,
                s.SupplierNickName,
                s.SupplierRoles,
                CreateTime = s.CreateTime < DateTimeOffset.Parse("1901-01-01") ? "" : s.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                LastLoginTime = s.LastLoginTime < DateTimeOffset.Parse("1901-01-01") ? "" : s.LastLoginTime.ToString("yyyy-MM-dd HH:mm:ss"),
                s.IP,
                s.SupplierUserEnableState,
                WeixinBind = string.IsNullOrEmpty(s.OpenID) ? false : true,
            });
            return JsonConvert.SerializeObject(new { draw = draw, recordsFiltered = count, data = data, SearchModel = langhua });
        }
        //解绑微信
        [HttpPost]
        public async Task<string> UnbindWeixin(int? id)
        {
            if (id == null || id == 0)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "ID不能为空" });
            }
            try
            {
                var user = await client.For<SupplierUser>().Key(id).FindEntryAsync();
                if (!string.IsNullOrEmpty(user.OpenID))
                {
                    string userName = User.Identity.Name;
                    SupplierUser OperUser = await client.For<SupplierUser>().Filter(u => u.SupplierUserName == userName).FindEntryAsync();
                    if (user.SupplierID != OperUser.SupplierID)
                    {
                        return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "操作异常" });
                    }

                    user.OpenID = null;
                    await client.For<SupplierUser>().Key(id).Set(user).UpdateEntryAsync();

                    //操作记录
                    await client.For<SupplierUserLog>().Set(new SupplierUserLog
                    {
                        OperSupplierUserID = OperUser.SupplierUserID,
                        OperSupplierNickName = OperUser.SupplierNickName,
                        OperTime = DateTime.Now,
                        Remark = "",
                        Operate = UserOperate.UnbindWeixin,
                        SupplierUserID = user.SupplierUserID,
                        SupplierUserName = user.SupplierUserName,
                        SupplierNickName = user.SupplierNickName
                    }).InsertEntryAsync();
                }
            }
            catch(Exception ex)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 400, ErrorMessage = "操作异常！"+ ex .Message});
            }
            return JsonConvert.SerializeObject(new { ErrorCode = 200, ErrorMessage = "OK" });
        }
        //启用禁用
        [HttpPost]
        public async Task<string> UpdateDisable(string UserID, string Operation)
        {
            var failed = (new int[] { 1 }).Select(x => new { name = "", reason = "" }).ToList();
            failed.Clear();
            if (string.IsNullOrEmpty(UserID))
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "ID不能为空！" });
            }
            if (string.IsNullOrEmpty(Operation))
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "操作异常！" });
            }
            try
            {
                var id = UserID.Split(',');
                string userName = User.Identity.Name;
                SupplierUser OperUser = await client.For<SupplierUser>().Filter(u => u.SupplierUserName == userName).FindEntryAsync();
                if (Operation.Trim() == "0")
                {
                    foreach (var i in id)
                    {
                        var oldUser = await client.For<SupplierUser>().Key(int.Parse(i)).FindEntryAsync();
                        if (oldUser.SupplierID != OperUser.SupplierID)
                        {
                            failed.Add(new { name = oldUser.SupplierNickName, reason = "账号异常！" });
                        }
                        else
                        {
                            oldUser.SupplierUserEnableState = EnableState.Enable;
                            await client.For<SupplierUser>().Key(int.Parse(i)).Set(oldUser).UpdateEntryAsync();
                            //操作记录
                            await client.For<SupplierUserLog>().Set(new SupplierUserLog
                            {
                                OperSupplierUserID = OperUser.SupplierUserID,
                                OperSupplierNickName = OperUser.SupplierNickName,
                                OperTime = DateTime.Now,
                                Remark = "",
                                Operate = UserOperate.Enable,
                                SupplierUserID = oldUser.SupplierUserID,
                                SupplierUserName = oldUser.SupplierUserName,
                                SupplierNickName = oldUser.SupplierNickName
                            }).InsertEntryAsync();
                        }
                    }
                }
                else if (Operation.Trim() == "1")
                {
                    foreach (var i in id)
                    {
                        if (OperUser.SupplierUserID == int.Parse(i))
                        {
                            failed.Add(new { name = userName, reason = "谨慎点！别把自己也禁用了呀！" });
                        }
                        else
                        {
                            var oldUser = await client.For<SupplierUser>().Key(int.Parse(i)).FindEntryAsync();
                            if (oldUser.SupplierID != OperUser.SupplierID)
                            {
                                failed.Add(new { name = oldUser.SupplierNickName, reason = "账号异常！" });
                            }
                            else
                            {
                                oldUser.SupplierUserEnableState = EnableState.Disable;
                                await client.For<SupplierUser>().Key(int.Parse(i)).Set(oldUser).UpdateEntryAsync();
                                //操作记录
                                await client.For<SupplierUserLog>().Set(new SupplierUserLog
                                {
                                    OperSupplierUserID = OperUser.SupplierUserID,
                                    OperSupplierNickName = OperUser.SupplierNickName,
                                    OperTime = DateTime.Now,
                                    Remark = "",
                                    Operate = UserOperate.Disable,
                                    SupplierUserID = oldUser.SupplierUserID,
                                    SupplierUserName = oldUser.SupplierUserName,
                                    SupplierNickName = oldUser.SupplierNickName
                                }).InsertEntryAsync();
                            }
                        }
                    }
                }
                else if (Operation.Trim().ToLower() == "delete")
                {
                    foreach (var i in id)
                    {
                        if (OperUser.SupplierUserID == int.Parse(i))
                        {
                            failed.Add(new { name = userName, reason = "谨慎点！别把自己也删了呀！" });
                        }
                        else
                        {
                            var oldUser = await client.For<SupplierUser>().Key(int.Parse(i)).FindEntryAsync();
                            if (oldUser.SupplierID != OperUser.SupplierID)
                            {
                                failed.Add(new { name = oldUser.SupplierNickName, reason = "账号异常！" });
                            }
                            else
                            {
                                await client.For<SupplierUser>().Key(int.Parse(i)).DeleteEntryAsync();
                                //操作记录
                                await client.For<SupplierUserLog>().Set(new SupplierUserLog
                                {
                                    OperSupplierUserID = OperUser.SupplierUserID,
                                    OperSupplierNickName = OperUser.SupplierNickName,
                                    OperTime = DateTime.Now,
                                    Remark = "",
                                    Operate = UserOperate.Delete,
                                    SupplierUserID = oldUser.SupplierUserID,
                                    SupplierUserName = oldUser.SupplierUserName,
                                    SupplierNickName = oldUser.SupplierNickName
                                }).InsertEntryAsync();
                            }
                        }
                    }
                }
                else
                {
                    return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "操作异常！" });
                }
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 400, ErrorMessage = "操作异常：" + ex.Message });
            }
            return JsonConvert.SerializeObject(new { ErrorCode = 200, ErrorMessage = "OK", failed });
        }
        public async Task<ActionResult> UsersOperation(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }
            ViewBag.id = id;
            var oldUser = await client.For<SupplierUser>().Key(id).FindEntryAsync();
            string userName = User.Identity.Name;
            SupplierUser OperUser = await client.For<SupplierUser>().Filter(u => u.SupplierUserName == userName).FindEntryAsync();
            if (oldUser.SupplierID != OperUser.SupplierID)
            {
                return HttpNotFound();
            }
            return View();
        }
        //用户操作日志
        public async Task<string> GetUsersOperation(int? id, int? draw, int start = 0, int length = 50)
        {
            if (id == null)
            {
                return JsonConvert.SerializeObject(new { draw = draw, recordsFiltered = 0, data = "" });
            }
            var oldUser = await client.For<SupplierUser>().Key(id).FindEntryAsync();
            string userName = User.Identity.Name;
            SupplierUser OperUser = await client.For<SupplierUser>().Filter(u => u.SupplierUserName == userName).FindEntryAsync();
            if (oldUser.SupplierID != OperUser.SupplierID)
            {
                return JsonConvert.SerializeObject(new { draw = draw, recordsFiltered = 0, data = "" });
            }
            var Result = await client.For<SupplierUserLog>()
                .Filter(u => u.SupplierUserID == id)
                .OrderByDescending(u => u.SupplierUserLogID)
                .Skip(start).Top(length).FindEntriesAsync();
            var ResultCount = await client.For<SupplierUserLog>()
                .Filter(u => u.SupplierUserID == id)
                .Count().FindScalarAsync<int>();
            var logs = Result.Select(s => new
            {
                OperUserNickName = s.OperSupplierNickName,
                OperTime = s.OperTime.ToString("yyyy-MM-dd HH:mm"),
                Operate = EnumHelper.GetEnumDescription(s.Operate),
                Remark = s.Remark
            });
            return JsonConvert.SerializeObject(new { draw = draw, recordsFiltered = ResultCount, data = logs });
        }
        //重置密码
        [HttpPost]
        public async Task<string> ResetPassWord(int id)
        {
            var user = await client.For<SupplierUser>().Key(id).FindEntryAsync();
            string strNewPW = System.Guid.NewGuid().ToString().Replace("-", "").Substring(0, 6);
            string ResetPassWord = Md5Hash(strNewPW);
            user.PassWord = ResetPassWord;
            user.UpdatePassWordTime = DateTimeOffset.Now;
            await client.For<SupplierUser>().Key(id).Set(user).UpdateEntryAsync();
            string userName = User.Identity.Name;
            SupplierUser OperUser = await client.For<SupplierUser>().Filter(u => u.SupplierUserName == userName).FindEntryAsync();
            //操作记录
            await client.For<SupplierUserLog>().Set(new SupplierUserLog
            {
                OperSupplierUserID = OperUser.SupplierUserID,
                OperSupplierNickName = OperUser.SupplierNickName,
                OperTime = DateTime.Now,
                Remark = "",
                Operate = UserOperate.ResetPassWord,
                SupplierUserID = user.SupplierUserID,
                SupplierUserName = user.SupplierUserName,
                SupplierNickName = user.SupplierNickName
            }).InsertEntryAsync();

            return JsonConvert.SerializeObject(new { ErrorCode = 200, ErrorMessage = "重置成功,新密码为：" + strNewPW });
        }
        //加密
        private static string Md5Hash(string input)
        {
            MD5CryptoServiceProvider md5Hasher = new MD5CryptoServiceProvider();
            byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(input));
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }
    }
}