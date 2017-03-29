using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanghuaNew.Data
{
    /// <summary>
    /// 产品价格-按供应商和时间段
    /// </summary>
    public   class ItemPriceBySupplier
    {
        public int ItemPriceBySupplierID { get; set; }
       
        [Display(Name = "开始时间")]
        public DateTimeOffset startTime { get; set; }
        [Display(Name = "结束时间")]
        public DateTimeOffset EndTime { get; set; }
        [Display(Name = "成人价格")]
        public float AdultNetPrice { get; set; }
        [Display(Name = "儿童价格")]
        public float ChildNetPrice { get; set; }
        [Display(Name = "婴儿价格")]
        public float BobyNetPrice { get; set; }
        [Display(Name = "结算价格")]
        public float Price { get; set; }
        public int  SupplierServiceItemID { get; set; }

        public virtual SupplierServiceItem ServiceItem { get; set; }


    }

   
}
