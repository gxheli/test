using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanghuaNew.Data
{
    /// <summary>
    /// 货币
    /// </summary>
    public class Currency
    {
        public int CurrencyID { get; set; }
        [Required]
        [Display(Name = "货币名称")]
        public string CurrencyName { get; set; }
        [Required]
        [Display(Name = "货币编码")]
        public string CurrencyNo { get; set; }

        [Display(Name = "兑换方式")]
        public ChangeType CurrencyChangeType { get; set; }
        [Display(Name = "汇率")]
        public float ExchangeRate { get; set; }
        [Display(Name = "状态")]
        public EnableState CurrencyEnableState { get; set; }
    
        public List<SupplierServiceItem> SupplierServiceItems { get; set; }




    }
}
