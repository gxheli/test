using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanghuaNew.Data
{
    /// <summary>
    /// 产品模板
    /// </summary>
    public class ServiceItemTemplte
    {
        public int ServiceItemTemplteID { get; set; }
        [Display(Name = "模板内容")]
        public string ServiceItemTemplteHtml { get; set; }

    }
}
