using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
    //控位销控表
    public class SellControlModel
    {
        //完整日期 2016-01-01
        public string thisdate { get; set; }
        //简单日期 01-01
        public string date { get; set; }
        //出行人数
        public int TravelNum { get; set; }
        //预扣出行人数
        public int PreTravelNum { get; set; }
        //返回人数
        public int ReturnNum { get; set; }
        //预扣返回人数
        public int PreReturnNum { get; set; }
        //分销数量
        public int DistributionNum { get; set; }
        //颜色
        public SellControlModelState state { get; set; }
        //特殊控位数
        public int ControlNum { get; set; }
        //特殊控位原因
        public string ReMark { get; set; }

    }
    public enum SellControlModelState
    {
        [Description("人数为0")]
        While,
        [Description("人数大于0")]
        Green,
        [Description("确认加上预扣大于0")]
        Yellow,
        [Description("人数超过控位数")]
        Red,
        [Description("规则禁止")]
        Gray,
    }
}
