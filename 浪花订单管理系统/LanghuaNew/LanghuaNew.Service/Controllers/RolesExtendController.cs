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
    public class RolesExtendController : ApiController
    {
        private LanghuaContent db = new LanghuaContent();
        public async Task<IHttpActionResult> Post([FromBody]string value)
        {
            try
            {
                Role role = JsonConvert.DeserializeObject<Role>(value);
                if (role == null)
                {
                    return NotFound();
                }
                if (role.MenuRights == null)
                {
                    role.MenuRights = new List<MenuRight>();
                }
                var IDs = role.MenuRights.Select(T => T.MenuRightID);
                List<MenuRight> NewList = db.MenuRights.Where(P => IDs.Contains(P.MenuRightID)).ToList();
                role.MenuRights = NewList;
                db.Roles.Add(role);
                await db.SaveChangesAsync();
            }
            catch
            {
                
            }
            return StatusCode(HttpStatusCode.NoContent);
        }
        public async Task<IHttpActionResult> Put([FromBody]string value)
        {
            try
            {
                Role role = JsonConvert.DeserializeObject<Role>(value);
                if (role == null)
                {
                    return NotFound();
                }
                if (role.MenuRights == null)
                {
                    role.MenuRights = new List<MenuRight>();
                }
                var IDs = role.MenuRights.Select(T => T.MenuRightID);
                List<MenuRight> NewList = db.MenuRights.Where(P => IDs.Contains(P.MenuRightID)).ToList();
                role.MenuRights.Clear();
                db.Roles.Attach(role);
                var item = db.Entry(role);
                item.Collection(i => i.MenuRights).Load();
                List<MenuRight> OldList = role.MenuRights;
                var AddRoleList = NewList.Except(OldList, p => p.MenuRightID).ToList();
                var ModifList = NewList.Except(AddRoleList, p => p.MenuRightID).ToList();
                var DeleteList = OldList.Except(ModifList, p => p.MenuRightID).ToList();
                DeleteList.ForEach(P => role.MenuRights.Remove(P));
                AddRoleList.ForEach(P => role.MenuRights.Add(P));
                item.State = EntityState.Modified;
                await db.SaveChangesAsync();
            }
            catch
            {

            }
            return StatusCode(HttpStatusCode.NoContent);
        }
    }
}