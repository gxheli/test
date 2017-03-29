using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanghuaNew.Data
{
    /// <summary>
    /// 销控表
    /// </summary>
    public class SellControl
    {
        public int SellControlID { get; set; }
        [Display(Name = "销控表名称")]
        public string SellControlName { get; set; }
        [Display(Name = "每日销控位")]
        public int SellControlNum { get; set; }
        [Display(Name = "开始时间")]
        public DateTimeOffset StartDate { get; set; }
        [Display(Name = "月数")]
        public int MonthNum { get; set; }
        [Display(Name = "显示剩余数量")]
        public bool IsSurplusNum { get; set; }
        [Display(Name = "扣减分销数量")]
        public bool IsDistribution { get; set; }
        [Display(Name = "供应商")]
        public int SupplierID { get; set; }
        public Supplier Supplier { get; set; }
        public ServiceItem FirstServiceItem { get; set; }
        public ServiceItem SecondServiceItem { get; set; }
        public List<ExtraSetting> ExtraSettings { get; set; }
        [Display(Name = "排序编号")]
        public int RowNum { get; set; }
        public List<SellControlShow> sellControlShows { get; set; }
        public SellControlClassify sellControlClassify { get; set; }
        public int? SellControlClassifyID { get; set; }
        [Display(Name = "控位最后更新时间")]
        public DateTimeOffset LastUpdateTime { get; set; }
    }
    public class ExtraSetting
    {
        public int ExtraSettingID { get; set; }
        public int SellControlID { get; set; }
        public SellControl OneSellControl { get; set; }
        [Display(Name = "开始时间")]
        public DateTimeOffset StartTime { get; set; }
        [Display(Name = "结束时间")]
        public DateTimeOffset EndTime { get; set; }
        [Display(Name = "每日销控位")]
        public int ExtraSettingNum { get; set; }
        [Display(Name = "原因")]
        public string Remark { get; set; }
    }

    //控位销控表（显示）
    public class SellControlShow
    {
        public int SellControlShowID { get; set; }
        public SellControl sell { get; set; }
        public int SellControlID { get; set; }
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
    //控位销控表分类
    public class SellControlClassify
    {
        public int SellControlClassifyID { get; set; }
        public List<SellControl> sell { get; set; }
        //分类显示名
        public string ClassName { get; set; }
        //排序
        public int OrderBy { get; set; }
    }
}
