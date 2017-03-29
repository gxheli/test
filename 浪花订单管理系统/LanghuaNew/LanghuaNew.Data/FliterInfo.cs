using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanghuaNew.Data
{
    [Table("FliterInfos")]
    public class FliterInfo
    {
        public int ID { get; set; }

        [Index]
        [Required]
        [StringLength(30)]
        public string FliterNum { get; set; }
        [Required]
        [Display(Name = "出发地城市")]
        public string DepartureCity { get; set; }
        [Required]
        [Display(Name = "出发地")]
        public string FilterDeparture { get; set; }
        [Required]
        [Display(Name = "目的地城市")]
        public string ArrivalCity { get; set; }
        [Required]
        [Display(Name = "目的地")]
        public string FilterArrival { get; set; }
        [Display(Name = "抓取时间")]
        public DateTime SpliderTime { get; set; }
    }
}
