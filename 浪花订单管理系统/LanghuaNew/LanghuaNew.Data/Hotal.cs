using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanghuaNew.Data
{
    /// <summary>
    /// 酒店
    /// </summary>
    public class Hotal
    {
        public int  HotalID { get; set; }
        [Display(Name = "酒店名称")]
        public string HotalName { get; set; }
        [Display(Name = "电话")]
        public string HotalPhone { get; set; }
        [Display(Name = "地址")]
        public string HotalAddress { get; set; }
        [Display(Name = "区域")]
        public int AreaID { get; set; }
        public virtual Area HotalArea { get; set; }

    }
}
