using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanghuaNew.Data
{
    /// <summary>
    /// 账单报表
    /// </summary>
    public class BillReport
    {
        public int BillReportID { get; set; }
        [Display(Name = "供应商")]
        public int SupplierID { get; set; }
        public virtual Supplier oneSupplier { get; set; }
        [Display(Name = "生效方式-账单类型")]
        public EffectiveWay Type { get; set; }
        [Display(Name = "账单开始日期")]
        public DateTimeOffset StartDate { get; set; }
        [Display(Name = "账单结束日期")]
        public DateTimeOffset EndDate { get; set; }
        [Display(Name = "参考金额")]
        public float TotalReceive { get; set; }
        [Display(Name = "实际金额")]
        public float RealReceive { get; set; }
        [Display(Name = "货币代号")]
        public string Currency { get; set; }
        [Display(Name = "生成时间")]
        public DateTimeOffset CreateTime { get; set; }
        [Display(Name = "付款时间")]
        public DateTimeOffset PayTime { get; set; }
        [Display(Name = "对账状态")]
        public CheckState State { get; set; }
        [Display(Name = "备注")]
        public string Remark { get; set; }
        [Display(Name = "截取的文件流")]
        public string FileStream { get; set; }
        [Display(Name = "文件路径")]
        public string FilePath { get; set; }
    }
}
