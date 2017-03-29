using LanghuaNew.Data;
using LanghuaNew.Service.App_Code;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace LanghuaNew.Service.Controllers
{
    public class SupplierUsersExtendController : ApiController
    {
        private LanghuaContent db = new LanghuaContent();
        // POST: api/SupplierUsersExtend新建
        public async Task<HttpResponseMessage> Post([FromBody]string value)
        {
            try
            {
                SupplierUser user = JsonConvert.DeserializeObject<SupplierUser>(value);
                if (user == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }
                if (user.SupplierRoles == null)
                {
                    user.SupplierRoles = new List<SupplierRole>();
                }
                var RoleIDs = user.SupplierRoles.Select(T => T.SupplierRoleID);
                List<SupplierRole> NewRoleList = db.SupplierRoles.Where(P => RoleIDs.Contains(P.SupplierRoleID)).ToList();
                user.SupplierRoles = NewRoleList;
                db.SupplierUsers.Add(user);
                CreateRoles(user.SupplierID);
                await db.SaveChangesAsync();
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(user.SupplierUserID.ToString(), System.Text.Encoding.UTF8, "text/plain")
                };
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;

            }
            return Request.CreateResponse(HttpStatusCode.NoContent);
        }
        // PUT: api/SupplierUsersExtend/5修改
        public async Task<IHttpActionResult> Put([FromBody]string value)
        {
            try
            {
                SupplierUser user = JsonConvert.DeserializeObject<SupplierUser>(value);
                if (user == null)
                {
                    return NotFound();
                }
                if (user.SupplierRoles == null)
                {
                    user.SupplierRoles = new List<SupplierRole>();
                }
                var RoleIDs = user.SupplierRoles.Select(T => T.SupplierRoleID);
                List<SupplierRole> NewRoleList = db.SupplierRoles.Where(P => RoleIDs.Contains(P.SupplierRoleID)).ToList();
                user.SupplierRoles.Clear();
                db.SupplierUsers.Attach(user);
                var item = db.Entry(user);
                //关键是这句，需要先把关联表的数据加载过来再删除
                item.Collection(i => i.SupplierRoles).Load();
                List<SupplierRole> OldRoleList = user.SupplierRoles;
                var AddRoleList = NewRoleList.Except(OldRoleList, p => p.SupplierRoleID).ToList();
                var ModifList = NewRoleList.Except(AddRoleList, p => p.SupplierRoleID).ToList();
                var DeleteRoleList = OldRoleList.Except(ModifList, p => p.SupplierRoleID).ToList();
                DeleteRoleList.ForEach(P => user.SupplierRoles.Remove(P));
                AddRoleList.ForEach(P => user.SupplierRoles.Add(P));
                item.State = EntityState.Modified;
                CreateRoles(user.SupplierID);
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
            }
            return StatusCode(HttpStatusCode.NoContent);
        }
        /// <summary>
        /// 赋予默认角色及权限
        /// </summary>
        /// <param name="SupplierID"></param>
        private void CreateRoles(int SupplierID)
        {
            if (db.SupplierRoles.Where(s => s.SupplierID == SupplierID).FirstOrDefault() == null && db.SupplierRoleRights.FirstOrDefault() != null)
            {
                //增加角色\赋予角色权限
                SupplierRole Adminrole = new SupplierRole();
                Adminrole.SupplierRoleName = "管理员";
                Adminrole.SupplierRoleEnName = "Admin";
                Adminrole.SupplierID = SupplierID;
                Adminrole.CreateTime = DateTimeOffset.Now;
                Adminrole.LastEditTime = DateTimeOffset.Now;
                Adminrole.Remark = "可以对子帐号进行管理";
                var AdminidArray = Array.ConvertAll("1,2,3,8,10,11,12,13,14,15,16,18,19,20".Split(','), s => int.Parse(s));
                Adminrole.Rights = new List<SupplierRoleRight>();
                Adminrole.Rights.AddRange(db.SupplierRoleRights.Where(s => AdminidArray.Contains(s.SupplierRoleRightID)));
                db.SupplierRoles.Add(Adminrole);
                SupplierRole oprole = new SupplierRole();
                oprole.SupplierRoleName = "操作员";
                oprole.SupplierRoleEnName = "op";
                oprole.SupplierID = SupplierID;
                oprole.CreateTime = DateTimeOffset.Now;
                oprole.LastEditTime = DateTimeOffset.Now;
                oprole.Remark = "可以对订单进行处理";
                var opidArray = Array.ConvertAll("1,2,3,4,5,6,7,8,14,15,16,18,19,20".Split(','), s => int.Parse(s));
                oprole.Rights = new List<SupplierRoleRight>();
                oprole.Rights.AddRange(db.SupplierRoleRights.Where(s => opidArray.Contains(s.SupplierRoleRightID)));
                db.SupplierRoles.Add(oprole);
                SupplierRole Financialrole = new SupplierRole();
                Financialrole.SupplierRoleName = "财务";
                Financialrole.SupplierRoleEnName = "Financial";
                Financialrole.SupplierID = SupplierID;
                Financialrole.CreateTime = DateTimeOffset.Now;
                Financialrole.LastEditTime = DateTimeOffset.Now;
                Financialrole.Remark = "可以查看对账单";
                var idArray = Array.ConvertAll("1,2,3,8,9,16,17,19,20".Split(','), s => int.Parse(s));
                Financialrole.Rights = new List<SupplierRoleRight>();
                Financialrole.Rights.AddRange(db.SupplierRoleRights.Where(s => idArray.Contains(s.SupplierRoleRightID)));
                db.SupplierRoles.Add(Financialrole);
            }
        }
    }
}
