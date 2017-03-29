using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanghuaNew.Data
{
    /// <summary>
    /// 规则
    /// </summary>
    public class ServiceRule
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ServiceRuleID { get; set; }
        [Display(Name = "规则启用时间")]
        public DateTimeOffset StartTime { get; set; }
        [Display(Name = "规则结束时间")]
        public DateTimeOffset EndTime { get; set; }
        [Display(Name = "规则状态")]
        public EnableState UseState { get; set; }
        [Display(Name = "规则使用方式")]
        public RuleUseType RuleUseTypeValue { get; set; }
        [Display(Name = "规则类型")]
        public RuleType SelectRuleType { get; set; }
        [Display(Name = "按日期范围-开始时间")]
        public DateTimeOffset RangeStart { get; set; }
        [Display(Name = "按日期范围-结束时间")]
        public DateTimeOffset RangeEnd { get; set; }
        [Display(Name = "星期选择")]
        public string Week { get; set; }
        [Display(Name = "单双选择")]
        public bool IsDouble { get; set; }
        [Display(Name = "固定日期")]
        public string UseDate { get; set; }
        [Display(Name = "备注")]
        public string Remark { get; set; }

        [Display(Name = "规则适用产品")]
        public List<ServiceItem> RuleServiceItem { get; set; }

        public List<ServiceRuleLog> Logs { get; set; }
    }
    /// <summary>
    /// 规则日志
    /// </summary>
    public class ServiceRuleLog
    {
        public int ServiceRuleLogID { get; set; }
        [Display(Name = "操作")]
        public string Operate { get; set; }
        [Display(Name = "操作时间")]
        public DateTimeOffset OperateTime { get; set; }
        [Display(Name = "操作人")]
        public int UserID { get; set; }

        public string UserName { get; set; }
        [Display(Name = "备注")]
        public string Remark { get; set; }
        public int ServiceRuleID { get; set; }
        public virtual ServiceRule ServiceRuleValue { get; set; }

    }

}
