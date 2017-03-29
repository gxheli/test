using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanghuaNew.Data
{
    /// <summary>
    /// 客户回访记录
    /// </summary>
    public class CustomerBack
    {
        public int CustomerBackID { get; set; }
        [Display(Name = "客户")]
        public int CustomerID { get; set; }

        public Customer Customer { get; set; }
        [Display(Name = "跟进方式")]
        public BackType CustomerBackType { get; set; }
        [Display(Name = "情况记录")]
        public string Remark { get; set; }
        [Display(Name = "创建时间")]
        public DateTimeOffset CreateData { get; set; }
        public int OperateUserID { get; set; }
        [Display(Name = "跟进人昵称")]
        public string OperateUserNickName { get; set; }

    }
}
