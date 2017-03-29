using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LanghuaNew.Data;
using System.ComponentModel.DataAnnotations;

namespace LanghuaNew.Models
{

    public class OrderViewModel
    {
        public int ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public List<ServiceItemSupplierView> Items { get; set; }
        public ServiceItemView Item { get; set; }
        public IEnumerable<OrderSourse> OrderSourse { get; set; }
    }
    // 带出产品和供应商，生成Json时不会受到导航属性的循环影响
    public class ServiceItemSupplierView
    {
        public ServiceItemSupplierView()
        {
            ItemSupliers = new List<SupplierView>();
        }
        public int ServiceItemID { get; set; }
        [Display(Name = "产品编码")]
        public string ServiceCode { get; set; }
        [Display(Name = "中文名")]
        public string cnItemName { get; set; }
        [Display(Name = "英文名")]
        public string enItemName { get; set; }
        [Display(Name = "产品类别")]
        public string ServiceTypeID { get; set; }
        [Display(Name = "默认供应商ID")]
        public string DefaultSupplierID { get; set; }
        [Display(Name = "供应商")]
        public virtual List<SupplierView> ItemSupliers { get; set; }
    }
    // 根据产品ID/供应商ID带出产品详情和价格，生成Json时不会受到导航属性的循环影响
    public class ServiceItemView
    {
        public int ServiceItemID { get; set; }
        [Display(Name = "产品编码")]
        public string ServiceCode { get; set; }
        [Display(Name = "中文名")]
        public string cnItemName { get; set; }
        [Display(Name = "英文名")]
        public string enItemName { get; set; }
        [Display(Name = "产品类别")]
        public string ServiceTypeID { get; set; }
        public ServiceTypeView ItemServiceTypes { get; set; }
        [Display(Name = "供应商")]
        public SupplierView ItemSupliers { get; set; }
        [Display(Name = "额外服务")]
        public List<ExtraServiceView> ExtraService { get; set; }
        [Display(Name = "产品供应商价格")]
        public virtual SupplierServiceItemView SupplierServiceItemView { get; set; }

    }
    public class ServiceTypeView
    {
        public int ServiceTypeID { get; set; }
        [Display(Name = "产品类别")]
        public string ServiceTypeName { get; set; }
    }
    //额外服务
    public class ExtraServiceView
    {
        public int ExtraServiceID { get; set; }
        [Display(Name = "服务中文名称")]
        public string ServiceName { get; set; }
        [Display(Name = "服务英文名称")]
        public string ServiceEnName { get; set; }
        [Display(Name = "单位")]
        public string ServiceUnit { get; set; }
        [Display(Name = "最小值")]
        public int MinNum { get; set; }
        [Display(Name = "最大值")]
        public int MaxNum { get; set; }
        public ExtraServicePriceView ExtraServicePrices { get; set; }
    }
    //额外服务价格
    public class ExtraServicePriceView
    {
        [Display(Name = "单价")]
        public float ServicePrice { get; set; }
    }
    //供应商产品价格
    public class SupplierServiceItemView
    {
        [Display(Name = "计费标准")]
        public PricingMethod PayType { get; set; }
        [Display(Name = "生效方式")]
        public EffectiveWay SelectEffectiveWay { get; set; }
        [Display(Name = "产品基础价格")]
        public ItemPriceBySupplier ItemPriceBySupplier { get; set; }

    }
    //供应商
    public class SupplierView
    {
        public int SupplierID { get; set; }
        [Display(Name = "供应商简称")]
        public string SupplierName { get; set; }
        [Display(Name = "供应商代码")]
        public string SupplierNo { get; set; }
    }
    //用于接收返回值
    public class TBOrderView
    {
        [Display(Name = "淘宝ID")]
        public string TBID { get; set; }
        [Display(Name = "订单来源")]
        public int OrderSourseID { get; set; }
        [Display(Name = "订单")]
        public virtual List<OrderView> Orders { get; set; }
    }
    public class OrderView
    {
        [Display(Name = "产品ID")]
        public int ItemID { get; set; }
        [Display(Name = "供应商ID")]
        public int SupplierID { get; set; }
        [Display(Name = "淘宝订单号")]
        public string TBNum { get; set; }
        [Display(Name = "成人数量")]
        public int AdultNum { get; set; }
        [Display(Name = "儿童数量")]
        public int ChildNum { get; set; }
        [Display(Name = "婴儿数量")]
        public int INFNum { get; set; }
        [Display(Name = "间数")]
        public int RoomNum { get; set; }
        [Display(Name = "晚数")]
        public int RightNum { get; set; }
        [Display(Name = "出行日期")]
        public DateTimeOffset TravelDate { get; set; }
        [Display(Name = "客户信息")]
        public Customer Customer { get; set; }
        public virtual List<ExtraServiceHistoryView> ExtraServiceHistorys { get; set; }
        public virtual List<TBOrderNoView> TBOrderNos { get; set; }
    }
    public class ExtraServiceHistoryView
    {
        [Display(Name = "额外服务ID")]
        public int ExtraServiceID { get; set; }
        [Display(Name = "服务数量")]
        public int ServiceNum { get; set; }
    }
    public class TBOrderNoView
    {
        public string No { get; set; }
        public string SubNo { get; set; }
        public float Payment { get; set; }
        public long? RefundId { get; set; }
    }
    //查询额外服务
    public class SelectExtraServiceView
    {
        public int ExtraServiceID { get; set; }
        [Display(Name = "服务中文名称")]
        public string ServiceName { get; set; }
        [Display(Name = "服务英文名称")]
        public string ServiceEnName { get; set; }
        [Display(Name = "单位")]
        public string ServiceUnit { get; set; }
        [Display(Name = "最小值")]
        public int MinNum { get; set; }
        [Display(Name = "最大值")]
        public int MaxNum { get; set; }
        [Display(Name = "数目")]
        public int ServiceNum { get; set; }
        [Display(Name = "单价")]
        public float ServicePrice { get; set; }
    }
}