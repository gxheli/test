using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanghuaNew.Data
{
    /// <summary>
    /// 分销统计
    /// </summary>
    public class DistributionTally
    {
        public int DistributionTallyID { get; set; }
        public int ServiceItemID { get; set; }
        public ServiceItem serviceItem { get; set; }
        public int SupplierID { get; set; }
        public Supplier supplier { get; set; }
        [Display(Name = "团号")]
        public string GroupNo { get; set; }
        [Display(Name = "出行日期")]
        public DateTimeOffset TravelDate { get; set; }
        [Display(Name = "返回日期")]
        public DateTimeOffset ReturnDate { get; set; }
        [Display(Name = "成人数量")]
        public int AdultNum { get; set; }
        [Display(Name = "儿童数量")]
        public int ChildNum { get; set; }
        [Display(Name = "婴儿数量")]
        public int INFNum { get; set; }
        [Display(Name = "间数")]
        public int RoomNum { get; set; }
        [Display(Name = "晚数")]
        public int RightNum { get; set; }
        [Display(Name = "备注")]
        public string Remark { get; set; }
        [Display(Name = "取消/不取消")]
        public bool IsCancel { get; set; }
        [Display(Name = "登记人")]
        public int CreateUserID { get; set; }
        [Display(Name = "登记人昵称")]
        public string CreateUserNikeName { get; set; }
        [Display(Name = "登记时间")]
        public DateTimeOffset CreateTime { get; set; }
    }
}
