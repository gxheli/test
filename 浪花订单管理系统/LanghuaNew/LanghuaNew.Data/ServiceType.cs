using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanghuaNew.Data
{
    /// <summary>
    /// 产品类别
    /// </summary>
    public class ServiceType
    {
        public int ServiceTypeID { get; set;}
        [Required]
        [Display(Name = "产品类别")]
        public string ServiceTypeName { get; set; }

        public List<ServiceItem> ServiceItems{ get; set; }
    }
}
