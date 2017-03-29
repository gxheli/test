using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanghuaNew.Data
{
    /// <summary>
    /// 订单来源
    /// </summary>
    public class OrderSourse
    {
        public int OrderSourseID { get; set; }
        [Required]
        [Display(Name = "订单来源")]
        public string OrderSourseName { get; set; }
        [Display(Name = "显示顺序")]
        public int ShowNo { get; set; }
        [Display(Name = "淘宝订单")]
        public List<TBOrder> TBOrders { get; set; }
        [Display(Name = "状态")]
        public EnableState OrderSourseEnableState { get; set; }
        [Display(Name = "支付宝转账")]
        public List<AlipayTransfer> AlipayTransfers { get; set; }
    }
}
