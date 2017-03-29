using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanghuaNew.Data
{
    /// <summary>
    /// 产品供应商价格库(变更)
    /// </summary>
    public class SupplierServiceItemChange
    {
        public int SupplierServiceItemChangeID { get; set; }
        public int SupplierServiceItemID { get; set; }
        [Display(Name = "产品")]
        public int ServiceItemID { get; set; }
        public virtual ServiceItem Service { get; set; }
        [Display(Name = "供应商")]
        public int SupplierID { get; set; }
        public virtual Supplier ItemSupplier { get; set; }
        [Display(Name = "结算货币")]
        public int CurrencyID { get; set; }
        public virtual Currency ItemCurrency { get; set; }
        [Display(Name = "计费标准")]
        public PricingMethod PayType { get; set; }
        [Display(Name = "生效方式")]
        public EffectiveWay SelectEffectiveWay { get; set; }
        [Display(Name = "价格说明")]
        public string Remark { get; set; }
        [Display(Name = "产品基础价格(变更)")]
        public List<ItemPriceBySupplierChange> ItemPriceBySuppliers { get; set; }
        [Display(Name = "额外服务价格(变更)")]
        public List<ExtraServicePriceChange> ExtraServicePrices { get; set; }
    }
    /// <summary>
    /// 产品价格-按供应商和时间段(变更)
    /// </summary>
    public class ItemPriceBySupplierChange
    {
        public int ItemPriceBySupplierChangeID { get; set; }
        public int ItemPriceBySupplierID { get; set; }
        [Display(Name = "开始时间")]
        public DateTimeOffset startTime { get; set; }
        [Display(Name = "结束时间")]
        public DateTimeOffset EndTime { get; set; }
        [Display(Name = "成人价格")]
        public float AdultNetPrice { get; set; }
        [Display(Name = "儿童价格")]
        public float ChildNetPrice { get; set; }
        [Display(Name = "婴儿价格")]
        public float BobyNetPrice { get; set; }
        [Display(Name = "单价")]
        public float Price { get; set; }
        public int SupplierServiceItemChangeID { get; set; }
        public virtual SupplierServiceItemChange ServiceItem { get; set; }
    }
    //额外服务价格(变更)
    public class ExtraServicePriceChange
    {
        public int ExtraServicePriceChangeID { get; set; }
        public int ExtraServicePriceID { get; set; }
        [Display(Name = "服务")]
        public int ExtraServiceID { get; set; }
        public ExtraService Service { get; set; }
        [Display(Name = "单价")]
        public float ServicePrice { get; set; }
        public int SupplierServiceItemChangeID { get; set; }
        public virtual SupplierServiceItemChange ServiceItem { get; set; }
    }
}
