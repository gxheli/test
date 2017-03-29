using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanghuaNew.Data
{
    /// <summary>
    /// 角色权限表
    /// </summary>
    public class RoleRight
    {
        public int RoleRightID { get; set;}
        [Display(Name = "控制器名")]
        public string ControllerName { get; set; }
        [Display(Name = "动作名")]
        public string ActionName { get; set; }
        [Display(Name = "备注")]
        public string Remark { get; set; }
        public int? MenuRightID { get; set; }
        [Display(Name = "角色权限组")]
        public MenuRight menuRight { get; set; }
    }
    /// <summary>
    /// 角色权限组
    /// </summary>
    public class MenuRight
    {
        public int MenuRightID { get; set; }
        [Display(Name = "权限组名称")]
        public string MenuRightName { get; set; }
        [Display(Name = "备注")]
        public string Remark { get; set; }
        [Display(Name = "是否默认")]
        public bool isDefault { get; set; }
        [Display(Name = "所属模块")]
        public Modular? modular { get; set; }
        [Display(Name = "角色权限")]
        public List<RoleRight> RoleRights { get; set; }
        [Display(Name = "角色")]
        public List<Role> Roles { get; set; }
    }
}
