using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
    public class SalesStatisticModel
    {
        public string NickName { get; set; }
        public string BeginDate { get; set; }
        public string EndDate { get; set; }
        public int ServiceTypeID { get; set; }
        public int Num1 { get; set; }
        public int Num2 { get; set; }
        public int Num3 { get; set; }
    }
    public class SalesStatisticListModel
    {
        public int draw { get; set; }
        public int recordsFiltered { get; set; }
        public string UpdateTime { get; set; }
        public ShareSearchModel SearchModel { get; set; }
        public List<SalesStatisticModel> data { get; set; }
    }
    public class StatisticsOrderPriceModel
    {
        public string UserName { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public double Income { get; set; }
        public double Cost { get; set; }
        public double RefundFee { get; set; }
        public double AlipayTransfer { get; set; }
    }
}
