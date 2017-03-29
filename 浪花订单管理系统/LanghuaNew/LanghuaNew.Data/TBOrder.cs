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
    /// 淘宝订单
    /// </summary>
    public class TBOrder
    {
        [Display(Name = "淘宝订单ID")]
        public int TBOrderID { get; set; }
        [Display(Name = "淘宝ID")]
        public string TBID { get; set; }
        [Display(Name = "订单来源")]
        public int OrderSourseID { get; set; }
        public OrderSourse Sourse { get; set; }
        [Display(Name = "订单")]
        public virtual List<Order> Orders { get; set; }
        [Display(Name = "总成本")]
        public float TotalCost { get; set; }
        [Display(Name = "总金额")]
        public float TotalReceive { get; set; }
    
    }


 
}
