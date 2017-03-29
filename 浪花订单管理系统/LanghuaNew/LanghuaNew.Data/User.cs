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
    /// 用户信息
    /// </summary>
   public class User
    {
        public int UserID { get; set; }
        [Required]
        [Display(Name = "用户名")]
        public string UserName{ get; set; }
        [Required]
        [Display(Name = "密码")]
        public string PassWord { get; set; }
        [Required]
        [Display(Name = "昵称")]
        public string NickName { get; set; }
        [Display(Name = "上次登录时间")]
        public DateTimeOffset LateOnLineTime { get; set; }
    
        [Display(Name = "角色")]
        public virtual List<Role> UserRole { get; set; }
        [Display(Name = "状态")]
        public EnableState UserEnableState { get; set; }
        [Display(Name = "生成时间")]
        public DateTimeOffset CreateTime { get; set; }

        [Display(Name = "最后修改密码时间")]
        public DateTimeOffset UpdatePassWordTime { get; set; }

    }
    /// <summary>
    /// 用户操作日志
    /// </summary>
    public class UserLog
    {
        public int UserLogID { get; set; }
        [Display(Name = "处理人")]
        public string OperUserID { get; set; }

        public string OperUserNickName { get; set; }
        [Display(Name = "处理时间")]
        public DateTimeOffset OperTime { get; set; }
        [Display(Name = "备注")]
        public string Remark { get; set; }
        [Display(Name = "操作")]
        public UserOperate Operate { get; set; }
        //用户
        public int UserID { get; set; }

        //为了删除查找，预留用户名
        public string UserName { get; set; }
    }
    /// <summary>
    /// 用户登录日志
    /// </summary>
    public class UserLoginLog
    {
        public int UserLoginLogID { get; set; }
        //用户
        public int UserID { get; set; }

        //用户名
        public string UserName { get; set; }
        [Display(Name = "登录时间")]
        public DateTimeOffset LoginTime { get; set; }
    }
}



