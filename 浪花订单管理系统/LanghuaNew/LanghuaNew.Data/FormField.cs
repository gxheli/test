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
    /// 表单字段
    /// </summary>
    public class FormField
    {
        public int FormFieldID { get; set; }
        public string Key { get; set; }
       [Display(Name = "字段中文名")]
        public string FieldName { get; set; }
        [Display(Name = "字段编号")]
        public string FieldNo { get; set; }
        [Display(Name = "字段备注")]
        public string Remark { get; set; }
        [Display(Name = "示例样式")]
        public string ExampleStyle { get; set; }
    }
}
