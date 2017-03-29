using LanghuaNew.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LanghuaNew.Models
{
    public class ServiceItemViewModel
    {
        public int ServiceItemID { get; set; }
        //[Display(Name = "产品编码")]
        public string ServiceCode { get; set; }
        //[Display(Name = "中文名")]
        public string cnItemName { get; set; }
        //[Display(Name = "英文名")]
        public string enItemName { get; set; }
        //[Display(Name = "状态")]
        public EnableState ServiceItemEnableState { get; set; }
        //[Display(Name = "产品类别")]
        public string ServiceTypeName { get; set; }
        //[Display(Name = "保险天数")]
        public int InsuranceDays { get; set; }
        //[Display(Name = "默认供应商")]
        public string DefaultSupplier { get; set; }
        //[Display(Name = "是否维护表单信息")]
        public bool isExistElement { get; set; }
        //[Display(Name = "默认供应商价格维护状态")]
        public SupplierPriceState DefaultSupplierPriceState { get; set; }
        //[Display(Name = "默认供应商价格维护状态名称")]
        public string DefaultSupplierPriceStateName { get; set; }
        public string CityName { get; set; }
    }
    //显示产品列表
    public class ItemsList
    {
        public int draw { get; set; }
        public int recordsFiltered { get; set; }
        public List<ServiceItemViewModel> data { get; set; }
        public SearchModel SearchModel { get; set; }
    }
}