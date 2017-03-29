using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanghuaNew.Data
{
    //临时导入订单，用于生成查漏发货列表
    public class ExportOrder
    {
        public int ExportOrderID { get; set; }
        //唯一id
        public string Guid { get; set; }
        //导入订单号
        public string ExportOrderNo { get; set; }
        //导入淘宝id
        public string ExportTBID { get; set; }
        //导入时间
        public DateTimeOffset CreateTime { get; set; }
    }
}
