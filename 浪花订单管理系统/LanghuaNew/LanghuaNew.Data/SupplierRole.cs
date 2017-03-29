using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanghuaNew.Data
{
    /// <summary>
    /// 供应商角色
    /// </summary>
    public class SupplierRole
    {
        public int SupplierRoleID { get; set; }
        [Display(Name = "供应商")]
        public int? SupplierID { get; set; }
        public Supplier oneSupplier { get; set; }
        [Required]
        [Display(Name = "角色名")]
        public string SupplierRoleName { get; set; }
        [Required]
        [Display(Name = "角色英文名")]
        public string SupplierRoleEnName { get; set; }
        [Display(Name = "备注")]
        public string Remark { get; set; }
        [Display(Name = "供应商账号")]
        public List<SupplierUser> SupplierUsers { get; set; }
        [Display(Name = "创建时间")]
        public DateTimeOffset CreateTime { get; set; }
        [Display(Name = "最后修改时间")]
        public DateTimeOffset LastEditTime { get; set; }
        [Display(Name = "角色权限")]
        public List<SupplierRoleRight> Rights { get; set; }
    }
    /// <summary>
    /// 角色权限表
    /// </summary>
    public class SupplierRoleRight
    {
        public int SupplierRoleRightID { get; set; }
        [Display(Name = "控制器名")]
        public string ControllerName { get; set; }
        [Display(Name = "动作名")]
        public string ActionName { get; set; }
        [Display(Name = "备注")]
        public string Remark { get; set; }
        [Display(Name = "角色")]
        public List<SupplierRole> Roles { get; set; }
    }
}
