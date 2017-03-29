using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.Spatial;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LanghuaNew.Data
{
    /// <summary>
    /// 区域
    /// </summary>
    public class Area
    {
        public int AreaID { get; set; }
        [Required]
        [Display(Name = "区域名称")]
        public string AreaName { get; set; }
        [Required]
        [Display(Name= "区域英文名")]
        public string AreaEnName { get; set; }
       
        [Display(Name = "省市")]
        public int CityID { get; set; }
       
        public virtual City AreaCity { get; set; }
        [Display(Name = "状态")]
        public EnableState AreaEnableState { get; set; }
        [Display(Name = "酒店")]
        public  List<Hotal> Hotals { get; set; }


    }

   
}
