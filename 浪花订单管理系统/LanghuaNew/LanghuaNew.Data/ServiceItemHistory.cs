using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanghuaNew.Data
{
    /// <summary>
    /// 产品历史库--和订单关联
    /// </summary>

    public class ServiceItemHistory
    {
        [Key, ForeignKey("Order")]
        public int OrderID { get; set; }

        public virtual Order Order { get; set; }
        [Index]
        public int ServiceItemID { get; set; }
        public string ServiceCode { get; set; }
        [Display(Name = "中文名")]
        public string cnItemName { get; set; }
        [Display(Name = "英文名")]
        public string enItemName { get; set; }
        [Display(Name = "供应商")]
        public int SupplierID { get; set; }
        public Supplier orderSupplier { get; set; }
        [Display(Name = "供应商简称")]
        public string SupplierName { get; set; }
        [Display(Name = "供应商编号")]
        public string SupplierCode { get; set; }
        [Display(Name = "成人价格")]
        public float AdultNetPrice { get; set; }
        [Display(Name = "儿童价格")]
        public float ChildNetPrice { get; set; }
        [Display(Name = "婴儿价格")]
        public float BobyNetPrice { get; set; }
        [Display(Name = "单间价格")]
        public float Price { get; set; }

        [Display(Name = "货币ID")]
        public int CurrencyID { get; set; }
        [Display(Name = "货币名称")]
        public string CurrencyName { get; set; }
        [Display(Name = "兑换方式")]
        public ChangeType CurrencyChangeType { get; set; }
        [Display(Name = "汇率")]
        public float ExchangeRate { get; set; }


        [Display(Name = "计费标准")]
        public PricingMethod PayType { get; set; }

        [Display(Name = "表单字段")]
        public string Elements { get; set; }

        [Display(Name = "表单的值")]
        public string ElementsValue { get; set; }
      
      
        [Display(Name = "成人数量")]
        public int AdultNum { get; set; }
        [Display(Name = "儿童数量")]
        public int ChildNum{ get; set; }
        [Display(Name = "婴儿数量")]
        public int INFNum { get; set; }
        [Display(Name = "间数")]
        public int RoomNum { get; set; }
        [Display(Name = "晚数")]
        public int RightNum { get; set; }
        [Display(Name = "出行日期")]
        public DateTimeOffset TravelDate { get; set; }
        [Display(Name = "变更出行日期")]
        public DateTimeOffset ChangeTravelDate { get; set; }
        [Display(Name = "返回日期")]
        public DateTimeOffset ReturnDate { get; set; }
        public DateTimeOffset CreateTime { get; set; }
        //Insurance，TripCompany
        [Display(Name = "出行旅客")]
        public List<OrderTraveller> travellers{ get; set; }
   

        [Display(Name = "团号")]
        public string GroupNo { get; set; }
        [Display(Name = "接人时间")]
        public string ReceiveManTime { get; set; }
        [Display(Name = "交通附加费")]
        public float TrafficSurcharge { get; set; }


        [Display(Name = "交通附加费货币ID")]
        public int TrafficCurrencyID { get; set; }
        [Display(Name = "交通附加费货币名称")]
        public string TrafficCurrencyName { get; set; }
        [Display(Name = "交通附加费兑换方式")]
        public ChangeType TrafficCurrencyChangeType { get; set; }
        [Display(Name = "交通附加费汇率")]
        public float TrafficExchangeRate { get; set; }

        [Display(Name = "总成本")]
        public float TotalCost { get; set; }


        [Display(Name = "额外服务")]
        public virtual List<ExtraServiceHistory> ExtraServiceHistorys { get; set; }

        [Display(Name = "行程公司")]
        public string TravelCompany { get; set; }
        [Display(Name = "保险天数")]
        public int InsuranceDays { get; set; }
        [Display(Name = "产品类别ID")]
        public int ServiceTypeID { get; set; }
        [Display(Name = "产品类别名称")]
        public string ServiceTypeName { get; set; }
        [Display(Name = "模板")]
        public int? ServiceItemTemplteID { get; set; }
        public virtual ServiceItemTemplte ItemTemplte { get; set; }
        [Display(Name = "模板值")]
        public string ServiceItemTemplteValue { get; set; }
        [Display(Name = "行程天数")]
        public int FixedDays { get; set; }

        [Display(Name = "请求变更数据")]
        public string ChangeValue { get; set; }
        [Display(Name = "请求变更模板值")]
        public string ChangeElementsValue { get; set; }
    }

    //额外服务

    public class ExtraServiceHistory
    {

        public int ExtraServiceHistoryID { get; set; }
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
        [Display(Name = "附加资料")]
        public bool IsNeedDetail { get; set; }

        [Display(Name = "服务数量")]
        public int ServiceNum { get; set; }
        [Display(Name = "服务单价")]
        public float ServicePrice { get; set; }

    }
}
