using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
    public class CheckDeliveryModel
    {
        public string OrderNo { get; set; }
        public string TBID { get; set; }
        public string state { get; set; }
        public string minDate { get; set; }
        public string IsNeedCustomerService { get; set; }
    }
}
