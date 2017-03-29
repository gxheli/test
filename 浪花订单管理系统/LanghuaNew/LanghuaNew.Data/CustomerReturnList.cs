using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanghuaNew.Data
{
    /// <summary>
    /// 客户回访名单设置
    /// </summary>
    public class CustomerReturnList
    {
        [Display(Name = "ID")]
        public int CustomerReturnListID { get; set; }
        [Display(Name = "回访产品")]
        public int ServiceItemID { get; set; }
        public virtual ServiceItem ReturnServiceItem { get; set; }
        [Display(Name = "回访供应商")]
        public int SupplierID { get; set; }
        public virtual Supplier ReturnSupplier { get; set; }
    }
}
