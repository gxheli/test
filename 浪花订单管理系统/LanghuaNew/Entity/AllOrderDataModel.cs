using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
    /// <summary>
    /// 整店数据
    /// </summary>
    public class AllOrderDataModel
    {
        /// <summary>
        /// 今日订单数
        /// </summary>
        public int TodayOrderCount { get; set; }
        /// <summary>
        /// 昨日订单数
        /// </summary>
        public int YesterdayOrderCount { get; set; }
        /// <summary>
        /// 待检查
        /// </summary>
        public int OnCheckCount { get; set; }
        /// <summary>
        /// 已核对
        /// </summary>
        public int CheckCount { get; set; }
        /// <summary>
        /// 要售后
        /// </summary>
        public int NeedServiceCount { get; set; }
        /// <summary>
        /// 今日销售额
        /// </summary>
        public float TodaySales { get; set; }
        /// <summary>
        /// 今日利润
        /// </summary>
        public float TodayProfits { get; set; }
        /// <summary>
        /// 本月销售额
        /// </summary>
        public float ThisMonthSales { get; set; }
        /// <summary>
        /// 本月利润
        /// </summary>
        public float ThisMonthProfits { get; set; }
        /// <summary>
        /// 今日出行人数
        /// </summary>
        public int TodayTravelNum { get; set; }
        /// <summary>
        /// 昨日出行人数
        /// </summary>
        public int YesterdayTravelNum { get; set; }
        /// <summary>
        /// 本月服务人数
        /// </summary>
        public int ThisMonthTravelNum { get; set; }
        /// <summary>
        /// 上月服务人数
        /// </summary>
        public int PreMonthTravelNum { get; set; }
        /// <summary>
        /// 微信绑定人数
        /// </summary>
        public int WeixinBindCount { get; set; }
        /// <summary>
        /// 微信绑定率
        /// </summary>
        public string WeixinBindRate { get; set; }
        /// <summary>
        /// 刷新时间点
        /// </summary>
        public string RefreshTime { get; set; }
    }
}
