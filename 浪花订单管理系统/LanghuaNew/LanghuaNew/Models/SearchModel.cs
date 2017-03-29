using LanghuaNew.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LanghuaNew.Models
{
    public class SearchModel
    {
        public int draw { get; set; }
        public int length { get; set; }
        public int start { get; set; }
        public OrderByModel OrderBy { get; set; }
        public ItemSearchModel ItemSearch { get; set; }
        public RuleSearchModel RuleSearch { get; set; }
        public AlipayTransferSearchModel AlipaySearch { get; set; }
        public WeixinSearchModel WeixinSearch { get; set; }
        
    }
    public class OrderByModel
    {
        public string PropertyName { get; set; }
        public int OrderBy { get; set; }
    }
    public class ItemSearchModel
    {
        public int status { get; set; }
        public string FuzzySearch { get; set; }
        public int ServiceTypeID { get; set; }
        public int SupplierID { get; set; }
        public string searchType { get; set; }

    }
    public class RuleSearchModel
    {
        public int status { get; set; }
        public string FuzzySearch { get; set; }
    }
    public class AlipayTransferSearchModel
    {
        public string FuzzySearch { get; set; }
        //[Display(Name = "状态")]
        public int TransferStateValue { get; set; }
        //[Display(Name = "转账类型")]
        public int TransferTypeValue { get; set; }
    }
    public class WeixinSearchModel
    {
        public int status { get; set; }
        public int CountryID { get; set; }
        public string FuzzySearch { get; set; }
    }

}