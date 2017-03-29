using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanghuaNew.Data
{
    /// <summary>
    /// 系统日志
    /// </summary>
    public class SystemLog
    {
        public int SystemLogID { get; set; }
        [Display(Name = "操作")]
        public string Operate { get; set; }
        [Display(Name = "操作时间")]
        public DateTimeOffset OperateTime { get; set; }
        [Display(Name = "操作人")]
        public int UserID { get; set; }

        public string UserName { get; set; }
        [Display(Name = "备注")]
        public string Remark { get; set; }
    }
}
