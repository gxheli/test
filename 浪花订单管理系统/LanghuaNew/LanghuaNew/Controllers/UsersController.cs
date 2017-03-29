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
using System.Web.Security;
using System.Security.Cryptography;
using System.Text;
using PagedList;
using AutoMapper;
using LanghuaNew.Models;
using Simple.OData.Client;
using System.Configuration;
using LanghuaNew;
using System.Linq.Expressions;
using Commond;
using Newtonsoft.Json;
using System.IO;
using System.Net.Http;

namespace LanghuaNew.Controllers
{

    public class UsersController : Controller
    {
        //private LanghuaContent db = new LanghuaContent();
        private ODataClient client = new ODataClient(ConfigurationManager.AppSettings["ServicePath"] + "odata/");
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }
        public async Task<string> GetUsers(SearchModel search)
        {
            int draw = 1;
            int start = 0;
            int length = 50;
            int status = 0;
            string FuzzySearch = string.Empty;
            if (search.length > 0)
            {
                draw = search.draw;
                start = search.start;
                length = search.length;
            }
            if (search.RuleSearch != null)
            {
                status = search.RuleSearch.status;
                FuzzySearch = search.RuleSearch.FuzzySearch;
            }
            var Result = client.For<User>().Expand(t => t.UserRole);//.Filter(t => !t.IsDelete);
            var ResultCount = client.For<User>();//.Filter(t => !t.IsDelete);
            if (status == 1)
            {
                Result = Result.Filter(t => t.UserEnableState == EnableState.Enable);
                ResultCount = ResultCount.Filter(t => t.UserEnableState == EnableState.Enable);
            }
            else if (status == 2)
            {
                Result = Result.Filter(t => t.UserEnableState == EnableState.Disable);
                ResultCount = ResultCount.Filter(t => t.UserEnableState == EnableState.Disable);
            }
            if (!string.IsNullOrEmpty(FuzzySearch))
            {
                Result = Result.Filter(t => t.UserName.Contains(FuzzySearch) || t.NickName.Contains(FuzzySearch));
                ResultCount = ResultCount.Filter(t => t.UserName.Contains(FuzzySearch) || t.NickName.Contains(FuzzySearch));
            }
            int count = await ResultCount.Count().FindScalarAsync<int>();
            var users = await Result.OrderBy(t => t.UserID).Skip(start).Top(length).FindEntriesAsync();

            List<UsersModel> usersmodel = new List<UsersModel>();
            foreach (var item in users)
            {
                UsersModel model = new UsersModel();
                model.UserID = item.UserID;
                model.UserName = item.UserName;
                model.NickName = item.NickName;
                model.CreateTime = item.CreateTime < DateTime.Parse("1900-01-02") ? "" : item.CreateTime.ToString("yyyy-MM-dd HH:mm:ss");
                model.LateOnLineTime = item.LateOnLineTime < DateTime.Parse("1900-01-02") ? "" : item.LateOnLineTime.ToString("yyyy-MM-dd HH:mm:ss");
                model.UserRole = "";
                if (item.UserRole != null)
                {
                    item.UserRole.ForEach(t => model.UserRole += t.RoleName + ",");
                }
                model.UserRole = model.UserRole.Length > 0 ? model.UserRole.Substring(0, model.UserRole.Length - 1) : "";
                model.IsDelete = item.UserEnableState == EnableState.Disable ? true : false;
                usersmodel.Add(model);
            }

            UsersList usersList = new UsersList();
            usersList.draw = draw;
            usersList.recordsFiltered = count;
            usersList.data = usersmodel;
            usersList.SearchModel = search;
            return JsonConvert.SerializeObject(usersList);
        }
        //启用禁用
        [HttpPost]
        public async Task<string> UpdateDisable(string UserID, string Operation)
        {
            var failed = (new int[] { 1 }).Select(x => new { name = "", reason = "" }).ToList();
            failed.Clear();
            if (string.IsNullOrEmpty(UserID))
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "用户不能为空！" });
            }
            if (string.IsNullOrEmpty(Operation))
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "操作异常！" });
            }
            try
            {
                var id = UserID.Split(',');
                string userName = User.Identity.Name;
                User OperUser = await client.For<User>().Filter(u => u.UserName == userName).FindEntryAsync();
                if (Operation.Trim() == "0")
                {
                    foreach (var i in id)
                    {
                        var oldUser = await client.For<User>().Key(int.Parse(i)).FindEntryAsync();
                        oldUser.UserEnableState = EnableState.Enable;
                        await client.For<User>().Key(int.Parse(i)).Set(oldUser).UpdateEntryAsync();
                        //操作记录
                        await client.For<UserLog>().Set(new { OperUserID = OperUser.UserID, OperUserNickName = OperUser.NickName, OperTime = DateTime.Now, Remark = "", Operate = UserOperate.Enable, UserID = oldUser.UserID, UserName = oldUser.UserName }).InsertEntryAsync();

                    }
                }
                else if (Operation.Trim() == "1")
                {
                    foreach (var i in id)
                    {
                        if (OperUser.UserID == int.Parse(i))
                        {
                            failed.Add(new { name = userName, reason = "谨慎点！别把自己也禁用了呀！" });
                        }
                        var oldUser = await client.For<User>().Key(int.Parse(i)).FindEntryAsync();
                        oldUser.UserEnableState = EnableState.Disable;
                        await client.For<User>().Key(int.Parse(i)).Set(oldUser).UpdateEntryAsync();
                        //操作记录
                        await client.For<UserLog>().Set(new { OperUserID = OperUser.UserID, OperUserNickName = OperUser.NickName, OperTime = DateTime.Now, Remark = "", Operate = UserOperate.Disable, UserID = oldUser.UserID, UserName = oldUser.UserName }).InsertEntryAsync();

                    }
                }
                else if (Operation.Trim().ToLower() == "delete")
                {
                    foreach (var i in id)
                    {
                        if (OperUser.UserID == int.Parse(i))
                        {
                            failed.Add(new { name = userName, reason = "谨慎点！别把自己也删了呀！" });
                        }
                        var oldUser = await client.For<User>().Key(int.Parse(i)).FindEntryAsync();
                        await client.For<User>().Key(int.Parse(i)).DeleteEntryAsync();
                        //操作记录
                        await client.For<UserLog>().Set(new { OperUserID = OperUser.UserID, OperUserNickName = OperUser.NickName, OperTime = DateTime.Now, Remark = "", Operate = UserOperate.Delete, UserID = oldUser.UserID, UserName = oldUser.UserName }).InsertEntryAsync();

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
        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }

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
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(User model, string returnUrl)
        {
            if (ModelState.IsValidField("UserName") && ModelState.IsValidField("PassWord"))
            {
                string PassHash = Md5Hash(model.PassWord);
                //User loginUser =await db.Users.FirstOrDefaultAsync(p => p.UserName == model.UserName && p.PassWord == PassHash);
                //IBoundClient<User> UsersSearch = client.For<User>();
                //UsersSearch = UsersSearch.Filter(p => p.UserName == model.UserName && p.PassWord == PassHash);
                User loginUser = await client.For<User>().Expand(p => p.UserRole).Filter(p => p.UserName == model.UserName && p.PassWord == PassHash && p.UserEnableState == EnableState.Enable).FindEntryAsync();

                if (loginUser == null)
                {
                    ModelState.AddModelError("", "账号或密码错误");

                }
                else
                {
                    if (loginUser.UpdatePassWordTime.AddDays(90) < DateTimeOffset.Now)
                    {
                        Session["UserName"] = loginUser.UserName;
                        Session["PassWord"] = loginUser.PassWord;
                        return RedirectToAction("UpdatePassWord", "Users");
                    }
                    else
                    {
                        string RoleNames = "";
                        if (loginUser.UserRole != null)
                        {
                            foreach (Role roleItem in loginUser.UserRole)
                            {
                                RoleNames += "," + roleItem.RoleName;
                            }
                        }

                        RoleNames = RoleNames.Trim(',');
                        Session["RoleNames"] = RoleNames;
                        Session["UserName"] = model.UserName;

                        FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(
                        1,
                        model.UserName,
                        DateTime.Now,
                        DateTime.MaxValue,
                        false,
                        ""
                        );
                        string encryptedTicket = FormsAuthentication.Encrypt(authTicket);
                        HttpCookie authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
                        System.Web.HttpContext.Current.Response.Cookies.Add(authCookie);

                        loginUser.LateOnLineTime = DateTime.Now;
                        await client.For<User>().Key(loginUser.UserID).Set(loginUser).UpdateEntryAsync();

                        //操作记录
                        await client.For<UserLog>().Set(new { OperUserID = loginUser.UserID, OperUserNickName = loginUser.NickName, OperTime = DateTime.Now, Remark = "", Operate = UserOperate.Login, UserID = loginUser.UserID, UserName = loginUser.UserName }).InsertEntryAsync();
                        //系统日志
                        await client.For<SystemLog>().Set(new { Operate = "登录", OperateTime = DateTime.Now, UserID = loginUser.UserID, UserName = loginUser.NickName, Remark = "" }).InsertEntryAsync();

                        if (!string.IsNullOrEmpty(returnUrl))
                        {
                            return RedirectToLocal(returnUrl);
                        }
                        else
                        {
                            return RedirectToAction("Index", "Home");
                        }
                    }
                }
            }
            return View(model);
        }
        [AllowAnonymous]
        public ActionResult UpdatePassWord()
        {
            if (Session["UserName"] == null || Session["PassWord"] == null)
            {
                return RedirectToAction("Login", "Users");
            }
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> UpdatePassWord(string newPassWord, string newPassWord2)
        {
            ViewBag.newPassWord = newPassWord;
            ViewBag.newPassWord2 = newPassWord2;
            if (Session["UserName"] == null || Session["PassWord"] == null)
            {
                return RedirectToAction("Login", "Users");
            }
            string UserName = Session["UserName"].ToString();
            string PassWord = Session["PassWord"].ToString();
            User loginUser = await client.For<User>()
                .Filter(p => p.UserName == UserName && p.PassWord == PassWord && p.UserEnableState == EnableState.Enable).FindEntryAsync();
            if (loginUser == null)
            {
                return RedirectToAction("Login", "Users");
            }
            if (string.IsNullOrEmpty(newPassWord))
            {
                ModelState.AddModelError("", "请输入新密码");
                return View();
            }
            if (string.IsNullOrEmpty(newPassWord2))
            {
                ModelState.AddModelError("", "请重新输入新密码");
                return View();
            }
            if (newPassWord != newPassWord2)
            {
                ModelState.AddModelError("", "2次密码不一致");
                return View();
            }
            if (Session["PassWord"].ToString() == Md5Hash(newPassWord))
            {
                ModelState.AddModelError("", "新密码与上一次密码相同");
                return View();
            }
            if (newPassWord.Length < 6 || newPassWord.Length > 12)
            {
                ModelState.AddModelError("", "密码必须6-12位数");
                return View();
            }
            FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(
                        1,
                        loginUser.UserName,
                        DateTime.Now,
                        DateTime.MaxValue,
                        false,
                        ""
                        );
            string encryptedTicket = FormsAuthentication.Encrypt(authTicket);
            HttpCookie authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
            System.Web.HttpContext.Current.Response.Cookies.Add(authCookie);

            loginUser.LateOnLineTime = DateTime.Now;
            loginUser.UpdatePassWordTime = DateTime.Now;
            loginUser.PassWord = Md5Hash(newPassWord);
            await client.For<User>().Key(loginUser.UserID).Set(loginUser).UpdateEntryAsync();
            //操作记录
            await client.For<UserLog>().Set(new { OperUserID = loginUser.UserID, OperUserNickName = loginUser.NickName, OperTime = DateTime.Now, Remark = "", Operate = UserOperate.ResetPassWord, UserID = loginUser.UserID, UserName = loginUser.UserName }).InsertEntryAsync();

            return RedirectToAction("Index", "Home");
        }
        // GET: Users
        public async Task<ActionResult> Index(string currentFilter, string searchString, int page = 1)
        {
            //const int pagesize = 10;
            //if (searchString != null)
            //{
            //    page = 1;
            //}
            //else
            //{
            //    searchString = currentFilter;
            //}
            //ViewBag.CurrentFilter = searchString;
            ////var Users = from s in db.Users select s;
            //IBoundClient<User> clientSearch = client.For<User>();
            //IBoundClient<User> clientSearchCount = client.For<User>();
            //if (!string.IsNullOrEmpty(searchString))
            //{
            //    clientSearch = clientSearch.Filter(p => p.UserName.Contains(searchString) || p.NickName.Contains(searchString));
            //    clientSearchCount = clientSearchCount.Filter(p => p.UserName.Contains(searchString) || p.NickName.Contains(searchString));

            //}
            //int ToltalCount = await clientSearchCount.Count().FindScalarAsync<int>();
            //IPagedList<User> Modes = await clientSearch.OrderBy(p => p.UserID).ToODataPagedListAsync(page, pagesize, ToltalCount);
            //return View(Modes);

            // return View(await Users.OrderBy(p => p.UserID).ToPagedListAsync(page, pagesize));
            return View();
        }

        // GET: Users/Details/5
        public async Task<ActionResult> Details(int? id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            IBoundClient<User> clientSearch = client.For<User>();
            //User user = await db.Users.FindAsync(id);
            User user = await clientSearch.Key(id).FindEntryAsync();
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // GET: Users/Create
        public async Task<ActionResult> Create()
        {
            IBoundClient<Role> clientSearch = client.For<Role>();
            UserViewModel userModel = new UserViewModel();
            userModel.AllRole = new List<Role>();
            userModel.AllRole.AddRange(await clientSearch.FindEntriesAsync());
            userModel.UserRole = new List<Role>();
            return View(userModel);
        }

        // POST: Users/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "UserName,PassWord,NickName")] User user)
        {
            if (ModelState.IsValid)
            {
                User SameUser = await client.For<User>().Filter(p => p.UserName == user.UserName).FindEntryAsync();
                if (!string.IsNullOrEmpty(Request.Form["UserRole"]))
                {
                    var SelectRole = new HashSet<string>(Request.Form["UserRole"].Split(','));

                    if (user.UserRole == null)
                    {
                        user.UserRole = new List<Role>();
                    }
                    foreach (var RoleID in SelectRole)
                    {
                        Role NewRole = new Role();
                        NewRole.RoleID = int.Parse(RoleID);
                        user.UserRole.Add(NewRole);
                    }
                }
                //用户名不能重复
                if (SameUser != null)
                {
                    ModelState.AddModelError("UserName", "用户名不能重复");
                }
                else
                {
                    user.CreateTime = DateTime.Now;
                    user.PassWord = Md5Hash(user.PassWord);
                    HttpResponseMessage Message = await HttpHelper.PostAction("UsersExtend", JsonConvert.SerializeObject(user));
                    string id = Message.Content.ReadAsStringAsync().Result;
                    try
                    {
                        string userName = User.Identity.Name;
                        User OperUser = await client.For<User>().Filter(u => u.UserName == userName).FindEntryAsync();
                        //操作记录
                        await client.For<UserLog>().Set(new { OperUserID = OperUser.UserID, OperUserNickName = OperUser.NickName, OperTime = DateTime.Now, Remark = "", Operate = UserOperate.Add, UserID = int.Parse(id), UserName = user.UserName }).InsertEntryAsync();

                        return RedirectToAction("Index");
                    }
                    catch
                    {

                    }
                }

            }
            var config = new MapperConfiguration(cfg => cfg.CreateMap<User, UserViewModel>());
            var mapper = config.CreateMapper();
            UserViewModel userModel = mapper.Map<UserViewModel>(user);
            userModel.AllRole = new List<Role>();
            userModel.AllRole.AddRange(await client.For<Role>().FindEntriesAsync());
            userModel.UserRole = userModel.UserRole == null ? new List<Role>() : userModel.UserRole;
            return View(userModel);
        }

        // GET: Users/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            }
            IBoundClient<User> clientSearch = client.For<User>();
            User userResult = await clientSearch.Key(id).Expand(x => x.UserRole).FindEntryAsync();
            //将linq对象映射到实体对象
            var config = new MapperConfiguration(cfg => cfg.CreateMap<User, UserViewModel>());
            var mapper = config.CreateMapper();
            UserViewModel userModel = mapper.Map<UserViewModel>(userResult);
            if (userModel == null)
            {
                return HttpNotFound();
            }
            userModel.AllRole = new List<Role>();
            userModel.AllRole.AddRange(await client.For<Role>().FindEntriesAsync());
            if (userModel.UserRole == null)
            {
                userModel.UserRole = new List<Role>();
            }
            return View(userModel);
        }

        // POST: Users/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "UserID,UserName,NickName")] User user)
        {
            if (!string.IsNullOrEmpty(Request.Form["UserRole"]))
            {
                var SelectRole = new HashSet<string>(Request.Form["UserRole"].Split(','));
                user.UserRole = new List<Role>();
                foreach (var RoleID in SelectRole)
                {
                    Role NewRole = new Role();
                    NewRole.RoleID = int.Parse(RoleID);
                    user.UserRole.Add(NewRole);
                }
            }
            if (ModelState.IsValidField("UserID") && ModelState.IsValidField("NickName"))
            {
                var olduser = await client.For<User>().Key(user.UserID).FindEntryAsync();
                olduser.NickName = user.NickName;
                olduser.UserRole = user.UserRole;
                await HttpHelper.PutAction("UsersExtend", JsonConvert.SerializeObject(olduser));
                string userName = User.Identity.Name;
                User OperUser = await client.For<User>().Filter(u => u.UserName == userName).FindEntryAsync();
                //操作记录
                await client.For<UserLog>().Set(new { OperUserID = OperUser.UserID, OperUserNickName = OperUser.NickName, OperTime = DateTime.Now, Remark = "", Operate = UserOperate.Edit, UserID = user.UserID, UserName = user.UserName }).InsertEntryAsync();

                return RedirectToAction("Index");

            }
            var config = new MapperConfiguration(cfg => cfg.CreateMap<User, UserViewModel>());
            UserViewModel userModel = config.CreateMapper().Map<UserViewModel>(user);
            userModel.AllRole = new List<Role>();
            userModel.AllRole.AddRange(await client.For<Role>().FindEntriesAsync());
            if (userModel.UserRole == null)
            {
                userModel.UserRole = new List<Role>();
            }
            return View(userModel);
        }

        // GET: Users/UserData
        public async Task<ActionResult> UserData()
        {
            string userName = User.Identity.Name;
            User user = await client.For<User>().Expand(u => u.UserRole).Filter(u => u.UserName == userName).FindEntryAsync();
            return View(user);
        }
        [HttpPost]
        public async Task<string> UpdateUserData(int? UserID, string PassWord, string newPassWord)
        {
            if (UserID == null || UserID == 0)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "用户不能为空" });
            }
            if (string.IsNullOrEmpty(PassWord))
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "旧密码不能为空" });
            }
            if (string.IsNullOrEmpty(newPassWord))
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "新密码不能为空" });
            }
            if (PassWord.Trim() == newPassWord.Trim())
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "新密码与上一次密码相同" });
            }
            string UserName = User.Identity.Name;
            User user = await client.For<User>()
                .Filter(s => s.UserName == UserName && s.UserID == UserID)
                .FindEntryAsync();
            if (user == null)
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "找不到用户" });
            }
            if (user.PassWord != Md5Hash(PassWord.Trim()))
            {
                return JsonConvert.SerializeObject(new { ErrorCode = 401, ErrorMessage = "旧密码输入错误" });
            }

            user.PassWord = Md5Hash(newPassWord.Trim());
            user.UpdatePassWordTime = DateTimeOffset.Now;
            await client.For<User>().Key(UserID).Set(user).UpdateEntryAsync();

            return JsonConvert.SerializeObject(new { ErrorCode = 200, ErrorMessage = "OK" });
        }
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> UserData([Bind(Include = "UserID,PassWord")] User user)
        //{
        //    if (ModelState.IsValidField("UserID") && ModelState.IsValidField("PassWord"))
        //    {
        //        var OperUser = await client.For<User>().Key(user.UserID).FindEntryAsync();
        //        OperUser.PassWord = Md5Hash(user.PassWord);
        //        OperUser.UpdatePassWordTime = DateTimeOffset.Now;
        //        await client.For<User>().Key(user.UserID).Set(OperUser).UpdateEntryAsync();
        //        //操作记录
        //        await client.For<UserLog>().Set(new { OperUserID = OperUser.UserID, OperUserNickName = OperUser.NickName, OperTime = DateTime.Now, Remark = "", Operate = UserOperate.Edit, UserID = user.UserID, UserName = user.UserName }).InsertEntryAsync();
        //        return RedirectToAction("Index", "Home");
        //    }
        //    User Olduser = await client.For<User>().Expand(u => u.UserRole).Key(user.UserID).FindEntryAsync();
        //    return View(Olduser);
        //}
        [HttpPost]
        public async Task<string> ResetPassWord(int id)
        {
            var user = await client.For<User>().Key(id).FindEntryAsync();
            string strNewPW = System.Guid.NewGuid().ToString().Replace("-", "").Substring(0, 6);
            string ResetPassWord = Md5Hash(strNewPW);
            user.PassWord = ResetPassWord;
            user.UpdatePassWordTime = DateTimeOffset.Now;
            await client.For<User>().Key(id).Set(user).UpdateEntryAsync();
            string userName = User.Identity.Name;
            User OperUser = await client.For<User>().Filter(u => u.UserName == userName).FindEntryAsync();
            //操作记录
            await client.For<UserLog>().Set(new { OperUserID = OperUser.UserID, OperUserNickName = OperUser.NickName, OperTime = DateTime.Now, Remark = "", Operate = UserOperate.ResetPassWord, UserID = user.UserID, UserName = user.UserName }).InsertEntryAsync();

            return JsonConvert.SerializeObject("重置成功,新密码为：" + strNewPW);
        }
        //列表导出
        public async Task<FileResult> ExportExcel(RuleSearchModel search)
        {
            // 获取列表
            int status = search.status;
            string FuzzySearch = search.FuzzySearch;

            var Result = client.For<User>().Expand(t => t.UserRole);
            if (status == 1)
            {
                Result = Result.Filter(t => t.UserEnableState == EnableState.Enable);
            }
            else if (status == 2)
            {
                Result = Result.Filter(t => t.UserEnableState == EnableState.Disable);
            }
            if (!string.IsNullOrEmpty(FuzzySearch))
            {
                Result = Result.Filter(t => t.UserName.Contains(FuzzySearch) || t.NickName.Contains(FuzzySearch));
            }
            var users = await Result.OrderBy(t => t.UserID).FindEntriesAsync();

            //创建Excel文件的对象
            NPOI.HSSF.UserModel.HSSFWorkbook book = new NPOI.HSSF.UserModel.HSSFWorkbook();
            //添加一个sheet
            NPOI.SS.UserModel.ISheet sheet1 = book.CreateSheet("Sheet1");

            int i = 0;
            //给sheet1添加第一行的头部标题
            NPOI.SS.UserModel.IRow row1 = sheet1.CreateRow(i);

            row1.CreateCell(0).SetCellValue("用户名");
            row1.CreateCell(1).SetCellValue("昵称");
            row1.CreateCell(2).SetCellValue("角色");
            row1.CreateCell(3).SetCellValue("创建时间");
            row1.CreateCell(4).SetCellValue("最近登录时间");
            row1.CreateCell(5).SetCellValue("状态");

            row1.Height = 450;

            //将数据逐步写入sheet1各个行
            foreach (var item in users)
            {
                string UserRole = "";
                if (item.UserRole != null)
                {
                    item.UserRole.ForEach(t => UserRole += t.RoleName + ",");
                }
                UserRole = UserRole.Length > 0 ? UserRole.Substring(0, UserRole.Length - 1) : "";

                i++;
                NPOI.SS.UserModel.IRow rowtemp = sheet1.CreateRow(i);

                rowtemp.CreateCell(0).SetCellValue(item.UserName);
                rowtemp.CreateCell(1).SetCellValue(item.NickName);
                rowtemp.CreateCell(2).SetCellValue(UserRole);
                rowtemp.CreateCell(3).SetCellValue(item.CreateTime < DateTime.Parse("1900-01-02") ? "" : item.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"));
                rowtemp.CreateCell(4).SetCellValue(item.LateOnLineTime < DateTime.Parse("1900-01-02") ? "" : item.LateOnLineTime.ToString("yyyy-MM-dd HH:mm:ss"));
                rowtemp.CreateCell(5).SetCellValue(item.UserEnableState == EnableState.Enable ? "启用" : "禁用");
            }

            // 写入到客户端 
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            book.Write(ms);
            ms.Seek(0, SeekOrigin.Begin);
            return File(ms, "application/vnd.ms-excel", "userlist.xls");
        }

        public ActionResult UsersOperation(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }
            ViewBag.id = id;
            return View();
        }
        public async Task<string> GetUsersOperation(int? id, int? draw, int start = 0, int length = 50)
        {
            if (id == null)
            {
                return JsonConvert.SerializeObject(new { draw = draw, recordsFiltered = 0, data = "" });
            }
            var Result = await client.For<UserLog>().Filter(u => u.UserID == id).OrderByDescending(u => u.UserLogID).Skip(start).Top(length).FindEntriesAsync();
            var ResultCount = await client.For<UserLog>().Filter(u => u.UserID == id).Count().FindScalarAsync<int>();
            List<UserLogModel> logs = new List<UserLogModel>();
            foreach (var item in Result)
            {
                UserLogModel log = new UserLogModel();
                log.OperUserNickName = item.OperUserNickName;
                log.OperTime = item.OperTime.ToString("yyyy-MM-dd HH:mm");
                log.Operate = EnumHelper.GetEnumDescription(item.Operate);
                log.Remark = item.Remark;
                logs.Add(log);
            }
            return JsonConvert.SerializeObject(new { draw = draw, recordsFiltered = ResultCount, data = logs });
        }
    }
}
