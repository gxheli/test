using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
    public class TBOrderStateModel
    {
        public int OrderSourseID { get; set; }
        public string OrderSourseName { get; set; }
        public string CustomerTBCode { get; set; }
        public DateTimeOffset mindate { get; set; }
        public DateTimeOffset maxdate { get; set; }
        public string SendUserName { get; set; }
        public DateTimeOffset sendtime { get; set; }
        public int IsSend { get; set; }

    }
    public class TBOrderStateListModel
    {
        public int draw { get; set; }
        public int recordsFiltered { get; set; }
        public ShareSearchModel SearchModel { get; set; }
        public List<TBOrderStateModel> data { get; set; }
    }
    public class TBOrderSubNo
    {
        public string SubNo { get; set; }
    }
}
