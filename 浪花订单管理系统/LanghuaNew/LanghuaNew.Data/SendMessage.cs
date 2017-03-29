using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace LanghuaNew.Data
{
    /// <summary>
    /// 发货
    /// </summary>
    public class SendMessage
    {
     
        public int SendMessageID { get; set; }
        [Display(Name = "客户")]
        public int CustomerID { get; set; }
        public virtual Customer SendCustomer { get; set; }
        [Display(Name = "是否已发")]
        public bool IsSend { get; set; }
        [Display(Name = "发货人")]
        public int UserID { get; set; }
        public string UserName { get; set; }
    }
}
