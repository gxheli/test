using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanghuaNew.Data
{
    /// <summary>
    /// 二维码统计
    /// </summary>
    public class QRcodeStatistic
    {
        public int QRcodeStatisticID { get; set; }
        [Display(Name = "二维码场景")]
        public QRcode Code { get; set; }
        [Display(Name = "IP")]
        public string IP { get; set; }
        public string UserAgent { get; set; }
        public DateTimeOffset CreateTime { get; set; }
    }
}
