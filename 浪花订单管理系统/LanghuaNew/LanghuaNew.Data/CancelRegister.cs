using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanghuaNew.Data
{
    /// <summary>
    /// 取消登记
    /// </summary>
    public class CancelRegister
    {
        public int CancelRegisterID { get; set; }
        public int ServiceItemID { get; set; }
        public ServiceItem serviceItem { get; set; }
        public int SupplierID { get; set; }
        public Supplier supplier { get; set; }
        [Display(Name = "取消开始日期")]
        public DateTimeOffset StartDate { get; set; }
        [Display(Name = "取消结束日期")]
        public DateTimeOffset EndDate { get; set; }
        [Display(Name = "取消原因")]
        public string Remark { get; set; }
        [Display(Name = "登记人")]
        public int CreateUserID { get; set; }
        [Display(Name = "登记人昵称")]
        public string CreateUserNikeName { get; set; }
        [Display(Name = "登记时间")]
        public DateTimeOffset CreateTime { get; set; }
        public List<CancelRegisterLog> cancelRegisterLogs { get; set; }
    }
    /// <summary>
    /// 日志
    /// </summary>
    public class CancelRegisterLog
    {
        public int CancelRegisterLogID { get; set; }
        public virtual CancelRegister cancelRegister { get; set; }
        public int CancelRegisterID { get; set; }
        [Display(Name = "处理人")]
        public int OperUserID { get; set; }
        public string OperUserNickName { get; set; }
        [Display(Name = "处理时间")]
        public DateTimeOffset OperTime { get; set; }
        [Display(Name = "操作")]
        public string Operate { get; set; }
        [Display(Name = "备注")]
        public string Remark { get; set; }
    }
}
