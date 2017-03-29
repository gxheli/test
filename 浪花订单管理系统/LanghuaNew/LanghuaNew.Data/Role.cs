using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace LanghuaNew.Data
{
    /// <summary>
    /// 角色
    /// </summary>
    public class Role
    {
        public int RoleID { get; set; }
        [Required]
        [Display(Name = "角色名")]
        public string RoleName { get; set; }
        [Display(Name = "角色英文名")]
        public string RoleEnName { get; set; }
        [Display(Name = "备注")]
        public string RoleRemark { get; set; }
        [Display(Name = "状态")]
        public EnableState RoleEnableState { get; set; }
        [Display(Name = "用户")]
        public List<User> Users { get; set; }
        [Display(Name = "创建时间")]
        public DateTimeOffset CreateTime { get; set; }
        [Display(Name = "角色权限组")]
        public List<MenuRight> MenuRights { get; set; }
    }
}
