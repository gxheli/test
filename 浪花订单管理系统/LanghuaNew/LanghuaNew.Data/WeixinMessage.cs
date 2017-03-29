using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanghuaNew.Data
{
    /// <summary>
    /// 微信模板信息
    /// </summary>
    public class WeixinMessage
    {
        public int WeixinMessageID { get; set; }
        [Display(Name = "启用时间")]
        public DateTimeOffset StartTime { get; set; }
        [Display(Name = "结束时间")]
        public DateTimeOffset EndTime { get; set; }
        [Display(Name = "状态")]
        public EnableState WeixinMessageState { get; set; }
        [Display(Name = "国家")]
        public int CountryID { get; set; }
        public Country WeixinCountry { get; set; }
        [Display(Name = "消息内容")]
        public string Message { get; set; }
        [Display(Name = "页面链接")]
        public string Url { get; set; }
        [Display(Name = "最后更新时间")]
        public DateTimeOffset LastEditDate { get; set; }
        [Display(Name = "最后更新人")]
        public int OperUserID { get; set; }
        public string OperUserNickName { get; set; }
    }
}
