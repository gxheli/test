using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanghuaNew.Data
{
    /// <summary>
    /// 产品供应商价格库
    /// </summary>
    public class SupplierServiceItem
    {
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

        [Display(Name = "产品基础价格")]
        public  List<ItemPriceBySupplier> ItemPriceBySuppliers { get; set; }
        [Display(Name = "额外服务价格")]
        public List<ExtraServicePrice> ExtraServicePrices { get; set; }
        [Display(Name = "产品价格日志")]
        public List<ItemPriceLog> ItemPriceLogs { get; set; }
        public bool IsChange { get; set; }

        [Display(Name = "卖价")]
        public float SellPrice { get; set; }
        [Display(Name = "儿童卖价")]
        public float ChildSellPrice { get; set; }
    }

    //额外服务价格
    public class ExtraServicePrice
    {
        public int ExtraServicePriceID { get; set; }
        [Display(Name = "服务")]
        public int ExtraServiceID { get; set; }
        public ExtraService Service { get; set; }
        [Display(Name = "单价")]
        public float ServicePrice { get; set; }
        [Display(Name = "卖价")]
        public float ServiceSellPrice { get; set; }
        public int SupplierServiceItemID { get; set; }
        public virtual SupplierServiceItem ServiceItem { get; set; }
    }
}
