using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanghuaNew.Data
{
    /// <summary>
    /// 业绩统计
    /// </summary>
    public class SalesStatistic
    {
        public int SalesStatisticID { get; set; }
        public int SupplierID { get; set; }
        public Supplier supplier { get; set; }
        public int ServiceItemID { get; set; }
        public ServiceItem serviceItem { get; set; }
    }
}
