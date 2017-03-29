using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.Spatial;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LanghuaNew.Data
{
    /// <summary>
    /// 客户
    /// </summary>
    public class Customer
    {
        [Display(Name = "ID")]
        public int CustomerID { get; set; }
        /// <summary>
        /// 淘宝ID,也用于系统登录名
        /// </summary>
        [Required]
        [Display(Name = "淘宝ID")]
        public string CustomerTBCode { get; set; }
        
        [Display(Name="中文名")]
        public string CustomerName { get; set; }
       
        [Display(Name = "拼音")]
        public string CustomerEnname { get; set; }
       
        [Display(Name = "密码")]
        public string Password { get; set; }
        [Display(Name = "电话")]
        public string Tel { get; set; }
        [Display(Name = "备用电话")]
        public string BakTel { get; set; }
        [Display(Name = "Email")]
        public string Email { get; set; }
        [Display(Name = "微信号")]
        public string Wechat { get; set; }
        public DateTimeOffset CreateTime { get; set; }
        [Display(Name = "常用游客信息")]
        public  List<Traveller> Travellers { get; set; }
        public bool IsDelete { get; set; }
        [Display(Name = "订单信息")]
        public List<Order> Orders { get; set; }
        [Display(Name = "是否要售后")]
        public bool IsNeedCustomerService { get; set; }
        [Display(Name = "是否使用过密码")]
        public bool IsUsePassWord { get; set; }
        [Display(Name = "微信OpenID")]
        public string OpenID { get; set; }
        [Display(Name = "是否已回访")]
        public bool IsBack { get; set; }
        [Display(Name = "客户回访记录")]
        public List<CustomerBack> CustomerBacks { get; set; }
        public List<CustomerLog> CustomerLogs { get; set; }
    }
    /// <summary>
    /// 用户操作日志
    /// </summary>
    public class CustomerLog
    {
        public int CustomerLogID { get; set; }
        public int CustomerID { get; set; }
        public Customer customer { get; set; }
        [Display(Name = "处理人")]
        public string OperID { get; set; }
        public string OperName { get; set; }
        [Display(Name = "处理时间")]
        public DateTimeOffset OperTime { get; set; }
        [Display(Name = "备注")]
        public string Remark { get; set; }
        [Display(Name = "操作")]
        public string Operate { get; set; }
    }

}
