using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanghuaNew.Data
{
    /// <summary>
    /// 供应商
    /// </summary>
    public class Supplier
    {
        public int SupplierID { get; set;}
        [Required]
        [Display(Name = "供应商简称")]
        public string SupplierName { get; set; }
        [Display(Name = "供应商英文名称")]
        public string SupplierEnName { get; set; }
        [Required]
        [Display(Name = "供应商代码")]
        public string SupplierNo { get; set; }
        [Display(Name = "国家")]
        public int CountryID { get; set; }
        public virtual Country SupplierCountry { get; set; }
        [Display(Name = "EMail")]
        public string EMail { get; set; }
        [Display(Name = "联系方式")]
        public string ContactWay { get; set; }
        [Display(Name = "状态")]
        public EnableState SupplierEnableState { get; set; }
        [Display(Name = "是否启用网上供应商")]
        public bool EnableOnline { get; set; }
        //[Display(Name = "系统帐号")]
        //public string SupplierSysName { get; set; }
        //[Display(Name = "系统密码")]
        //public string SupplierPWD { get; set; }
        [Display(Name = "产品")]
        public List<ServiceItem> ServiceItems { get; set; }
        [Display(Name = "订单")]
        public List<ServiceItemHistory> ServiceItemHistorys { get; set; }
        [Display(Name = "角色")]
        public List<SupplierRole> SupplierRoles { get; set; }
        [Display(Name = "账号")]
        public List<SupplierUser> SupplierUsers { get; set; }
    }
}
