using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanghuaNew.Data
{
    /// <summary>
    /// 淘宝订单状态
    /// </summary>
    public class TBOrderState
    {
        public int TBOrderStateID { get; set; }
        [Display(Name = "订单来源")]
        public int OrderSourseID { get; set; }
        public OrderSourse Sourse { get; set; }
        [Display(Name = "出行时间")]
        public DateTimeOffset OrderDate { get; set; }
        [Display(Name = "客户")]
        public int CustomerID { get; set; }
        public virtual Customer SendCustomer { get; set; }
        [Display(Name = "淘宝订单号")]
        public string TBNum { get; set; }
        [Display(Name = "是否已发货")]
        public bool IsSend { get; set; }
        [Display(Name = "发货人")]
        public int SendUserID { get; set; }
        public string SendUserName { get; set; }
        [Display(Name = "发货时间")]
        public DateTimeOffset SendTime { get; set; }

    }
}
