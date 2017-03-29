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
    /// 产品库
    /// </summary>
    public class ServiceItem
   {
        public int ServiceItemID { get; set; }
        [Display(Name = "产品编码")]
        public string ServiceCode { get; set; }
        [Display(Name = "中文名")]
        public string cnItemName { get; set; }
        [Display(Name = "英文名")]
        public string enItemName { get; set; }
        [Display(Name = "状态")]
        public EnableState ServiceItemEnableState { get; set; }
        public DateTimeOffset CreateTime { get; set; }
        //[Display(Name = "表单字段")]
        //public List<Element> Elements { get; set; }
        [Display(Name = "表单字段内容")]
        public string ElementContent { get; set; }

        [Display(Name = "行程公司")]
        public string TravelCompany { get; set; }
        [Display(Name = "保险天数")]
        public int InsuranceDays { get; set; }
        [Display(Name = "产品类别")]
        public int ServiceTypeID { get; set; }
        public virtual ServiceType ItemServiceType { get; set; }
        [Display(Name = "额外服务")]
        public virtual List<ExtraService> ExtraServices { get; set; }
        [Display(Name = "默认供应商ID")]
        public int DefaultSupplierID { get; set; }
        [Display(Name = "供应商")]
        public List<Supplier> ItemSuplier { get; set; }
        [Display(Name = "规则")]
        public List<ServiceRule> ServiceRules { get; set; }
        [Display(Name = "是否固定行程")]
        public bool IsFixedDay { get; set; }
        [Display(Name = "行程天数")]
        public int FixedDays { get; set; }
        [Display(Name = "模板")]
        public int? ServiceItemTemplteID { get; set; }

        public virtual ServiceItemTemplte ItemTemplte { get; set; }
        [InverseProperty("FirstServiceItem")]
        public List<SellControl> FirstSellControl { get; set; }
        [InverseProperty("SecondServiceItem")]
        public List<SellControl> SecondSellControl { get; set; }
        //Insurance，TripCompany

        [Display(Name = "是否自动发货")]
        public bool IsAutomaticDeliver { get; set; }
        [Display(Name = "目的地（省市）")]
        public int? CityID { get; set; }
        public virtual City city { get; set; }

    }

  
    //额外服务
    public class ExtraService
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
        //[Display(Name = "附加资料")]
        //public bool IsNeedDetail { get; set; }

    }
}
