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
    /// 省市
    /// </summary>
    public class City
    {
        public int CityID { get; set; }
        [Required]
        [Display(Name = "省市中文名")]
        public string CityName { get; set; }
        [Required]
        [Display(Name= "省市英文名")]
        public string CityEnName { get; set; }
        [Display(Name = "国家")]
        public int CountryID { get; set; }

        public virtual Country CityCountry { get; set; }
        [Display(Name = "区域")]
        public List<Area> Areas { get; set; }

    }

   
}
