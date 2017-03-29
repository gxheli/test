using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanghuaNew.Data
{
    /// <summary>
    /// 订单旅客信息
    /// </summary>
    public class OrderTraveller
    {
        public int OrderTravellerID { get; set; }
        public int TravellerID { get; set; }
        [Display(Name = "中文名")]
        public string TravellerName { get; set; }
        [Display(Name = "拼音")]
        public string TravellerEnname { get; set; }
        [Display(Name = "护照")]
        public string PassportNo { get; set; }
        [Display(Name = "生日")]
        public DateTimeOffset Birthday { get; set; }
        [Display(Name = "客户性别")]
        public sex TravellerSex { get; set; }
        [Display(Name = "创建时间")]
        public DateTimeOffset CreateTime { get; set; }
        [Display(Name = "详细信息")]
        public OrderTravellerDetail TravellerDetail { get; set; }
        public int OrderID { get; set; }
        [Display(Name = "参加的项目")]
        public ServiceItemHistory ServiceItemHistories { get; set; }
        public int CustomerID { get; set; }
        public virtual Customer CustomerValue { get; set; }

    }
    /// <summary>
    /// 订单旅客详细资料
    /// </summary>
    [ComplexType]
    public class OrderTravellerDetail
    {
        [Display(Name = "身高")]
        public string Height { get; set; }
        [Display(Name = "体重")]
        public string Weight { get; set; }
        [Display(Name = "鞋子码数")]
        public string ShoesSize { get; set; }
        [Display(Name = "衣服码数")]
        public string ClothesSize { get; set; }
        [Display(Name = "眼镜度数")]
        public string GlassesNum { get; set; }
    }
}
