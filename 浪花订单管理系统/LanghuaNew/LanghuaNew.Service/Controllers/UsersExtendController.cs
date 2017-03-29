using LanghuaNew.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Data.Entity;
using LanghuaNew.Service.App_Code;

namespace LanghuaNew.Service.Controllers
{
    public class UsersExtendController : ApiController
    {
        private LanghuaContent db = new LanghuaContent();
        // GET: api/UsersExtend
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }
        
        // GET: api/UsersExtend/5
        //public string Get(int id)
        //{
        //    return "value";
        //}

        // POST: api/UsersExtend
        public async Task<HttpResponseMessage> Post([FromBody]string value)
        {
            try
            {
                User user = JsonConvert.DeserializeObject<User>(value);
                if (user == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }
                if (user.UserRole == null)
                {
                    user.UserRole = new List<Role>();
                }
                var RoleIDs = user.UserRole.Select(T => T.RoleID);
                List<Role> NewRoleList = db.Roles.Where(P => RoleIDs.Contains(P.RoleID)).ToList();
                user.UserRole = NewRoleList;
                db.Users.Add(user);
                await db.SaveChangesAsync();
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(user.UserID.ToString(), System.Text.Encoding.UTF8, "text/plain")
                };
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;

            }
            return Request.CreateResponse(HttpStatusCode.NoContent);
        }

        // PUT: api/UsersExtend/5
        public async Task<IHttpActionResult> Put([FromBody]string value)
        {
            try
            {
                User user = JsonConvert.DeserializeObject<User>(value);
                if (user == null)
                {
                    return NotFound();
                }
                if (user.UserRole == null)
                {
                    user.UserRole = new List<Role>();
                }
                var RoleIDs = user.UserRole.Select(T => T.RoleID);
                List<Role> NewRoleList = db.Roles.Where(P => RoleIDs.Contains(P.RoleID)).ToList();
                user.UserRole.Clear();
                db.Users.Attach(user);
                var item = db.Entry(user);
                //关键是这句，需要先把关联表的数据加载过来再删除
                item.Collection(i => i.UserRole).Load();
                List<Role> OldRoleList = user.UserRole;
                var AddRoleList = NewRoleList.Except(OldRoleList, p => p.RoleID).ToList();
                var ModifList = NewRoleList.Except(AddRoleList, p => p.RoleID).ToList();
                var DeleteRoleList = OldRoleList.Except(ModifList, p => p.RoleID).ToList();
                DeleteRoleList.ForEach(P => user.UserRole.Remove(P));
                AddRoleList.ForEach(P => user.UserRole.Add(P));
                item.State = EntityState.Modified;
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
            }
            return StatusCode(HttpStatusCode.NoContent);
        }

        // DELETE: api/UsersExtend/5
        public void Delete(int id)
        {
        }
    }
}
