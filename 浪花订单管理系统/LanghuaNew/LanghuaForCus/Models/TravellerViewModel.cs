using LanghuaNew.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LanghuaForCus.Models
{
    public class TravellerViewModel
    {
        public int TravellerID { get; set; }
        //[Display(Name = "中文名")]
        public string TravellerName { get; set; }
        //[Display(Name = "拼音")]
        public string TravellerEnname { get; set; }
        //[Display(Name = "护照")]
        public string PassportNo { get; set; }
        //[Display(Name = "生日")]
        public string Birthday { get; set; }
        //[Display(Name = "客户性别")]
        public sex TravellerSex { get; set; }
        //[Display(Name = "创建时间")]
        public string CreateTime { get; set; }
        //[Display(Name = "详细信息")]
        public TravellerDetail TravellerDetail { get; set; }
        public bool IsCheck { get; set; }
    }
}