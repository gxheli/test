namespace Everything.Models
{
    //查询额外服务
    public class SelectExtraServiceView
    {
        public int ExtraServiceID { get; set; }
        //[Display(Name = "服务中文名称")]
        public string ServiceName { get; set; }
        //[Display(Name = "服务英文名称")]
        public string ServiceEnName { get; set; }
        //[Display(Name = "单位")]
        public string ServiceUnit { get; set; }
        //[Display(Name = "最小值")]
        public int MinNum { get; set; }
        //[Display(Name = "最大值")]
        public int MaxNum { get; set; }
        //[Display(Name = "数目")]
        public int ServiceNum { get; set; }
        //[Display(Name = "单价")]
        public float ServicePrice { get; set; }
    }
}