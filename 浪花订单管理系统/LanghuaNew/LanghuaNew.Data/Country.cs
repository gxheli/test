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
    /// 国家
    /// </summary>
    public class Country
    {
        public int CountryID { get; set; }
        [Required]
        [Display(Name = "国家中文名")]
        public string CountryName { get; set; }
        [Required]
        [Display(Name= "国家英文名")]
        public string CountryEnName { get; set; }
        public List<City> Citys { get; set; }
        [Display(Name = "该国家的供应商")]
        public List<Supplier> Suppliers { get; set; }

    }

   
}
