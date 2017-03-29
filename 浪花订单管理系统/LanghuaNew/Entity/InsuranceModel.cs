using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
    public class InsuranceModel
    {
        [Display(Name = "淘宝ID")]
        public string CustomerTBCode { get; set; }

        [Display(Name = "姓名")]
        public string TravellerName { get; set; }

        [Display(Name = "护照编号")]
        public string PassportNo { get; set; }

        [Display(Name = "生日")]
        public DateTimeOffset Birthday { get; set; }

        [Display(Name = "行程天数")]
        public int Insurance_Days { get; set; }

        [Display(Name = "保险开始日期")]
        public DateTimeOffset InsuranceStartDate { get; set; }

        [Display(Name = "产品编号")]
        public int ServiceItemID { get; set; }

        [Display(Name = "产品编码")]
        public string ServiceCode { get; set; }

        [Display(Name = "产品中文名")]
        public string cnItemName { get; set; }
        [Display(Name = "国家编号")]
        public int CountryID { get; set; }
        [Display(Name = "国家名称")]
        public string CountryName { get; set; }
        [Display(Name = "性别")]
        public int TravellerSex { get; set; }
    }

    public class InsuranceListModel
    {

        public int draw { get; set; }
        public int length { get; set; }
        public int start { get; set; }
        public int recordsFiltered { get; set; }
        public InsuranceSearchModel Search { get; set; }
        public List<InsuranceModel> data { get; set; }
    }
    public class InsuranceSearchModel
    {
        public int draw { get; set; }
        public int length { get; set; }
        public int start { get; set; }
        public InsuranceSearch Search { get; set; }
        
    }
    public class InsuranceSearch
    {
        public DateTimeOffset Insurance_Start_Date { get; set; }
        public int Insurance_Days { get; set; }
        public int ServiceItemID { get; set; }
        public string ServiceCode { get; set; }
        public string cnItemName { get; set; }
        public int CountryID { get; set; }
        public string CountryName { get; set; }
        public string InsuranceCode { get; set; }
    }
}
