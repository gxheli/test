using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanghuaNew.Data
{
    /// <summary>
    /// 供应商账号
    /// </summary>
    public class SupplierUser
    {
        public int SupplierUserID { get; set; }
        [Display(Name = "供应商")]
        public int SupplierID { get; set; }
        public Supplier OneSupplier { get; set; }
        [Required]
        [Display(Name = "用户名")]
        public string SupplierUserName { get; set; }
        [Required]
        [Display(Name = "昵称")]
        public string SupplierNickName { get; set; }
        [Required]
        [Display(Name = "密码")]
        public string PassWord { get; set; }
        [Display(Name = "创建时间")]
        public DateTimeOffset CreateTime{ get; set; }
        [Display(Name = "最近登录时间")]
        public DateTimeOffset LastLoginTime { get; set; }
        [Display(Name = "登录IP")]
        public string IP { get; set; }
        [Display(Name = "微信OpenID")]
        public string OpenID { get; set; }
        [Display(Name = "状态")]
        public EnableState SupplierUserEnableState { get; set; }
        [Display(Name = "是否主账号")]
        public bool IsMaster { get; set; }
        [Display(Name = "供应商角色")]
        public List<SupplierRole> SupplierRoles { get; set; }
        [Display(Name = "是否开启实时消息")]
        public bool RealTimeMessage { get; set; }
        [Display(Name = "是否开启汇总消息")]
        public bool SummaryMessage { get; set; }
        [Display(Name = "是否开启免打扰时段")]
        public bool Disturb { get; set; }
        [Display(Name = "免打扰开始时段")]
        public string BeginTime { get; set; }
        [Display(Name = "免打扰结束时段")]
        public string EndTime { get; set; }
        [Display(Name = "最后修改密码时间")]
        public DateTimeOffset UpdatePassWordTime { get; set; }
    }
    /// <summary>
    /// 操作日志
    /// </summary>
    public class SupplierUserLog
    {
        public int SupplierUserLogID { get; set; }
        [Display(Name = "处理人")]
        public int OperSupplierUserID { get; set; }
        public string OperSupplierNickName { get; set; }
        [Display(Name = "处理时间")]
        public DateTimeOffset OperTime { get; set; }
        [Display(Name = "备注")]
        public string Remark { get; set; }
        [Display(Name = "操作")]
        public UserOperate Operate { get; set; }
        //用户
        public int? SupplierUserID { get; set; }
        //为了删除查找，预留用户名
        public string SupplierUserName { get; set; }
        public string SupplierNickName { get; set; }
    }
}
