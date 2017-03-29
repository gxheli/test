using System;
using System.Collections.Generic;
using System.Text;

namespace Commond.Caching
{
    /// <summary>
    /// 缓存Key配置类
    /// </summary>
    public static class ConstantConfig
    {
        /// <summary>
        /// 订单系统空位销售表控件编号Key
        /// </summary>
        public static readonly string ORDER_SELLCONTROL_GETSELLCONTROL = "Order.SellControl-GetSellControl-{0}";

        /// <summary>
        /// 工作台整店数据
        /// </summary>
        public static readonly string HOME_ALLORDERDATA = "Home-AllOrderData";
        /// <summary>
        /// 业绩统计：id/开始时间/结束时间
        /// </summary>
        public static readonly string SALESSTATISTICS = "SalesStatistics-{0}-{1}-{2}";
    }
}
