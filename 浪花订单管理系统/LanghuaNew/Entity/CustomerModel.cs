using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
    public class CustomerModel
    {
        [Display(Name = "ID")]
        public int CustomerID { get; set; }
        [Display(Name = "淘宝ID")]
        public string CustomerTBCode { get; set; }
        [Display(Name="姓名")]
        public string CustomerName { get; set; }
        [Display(Name = "拼音")]
        public string CustomerEnname { get; set; }
        [Display(Name = "电话")]
        public string Tel { get; set; }
        [Display(Name = "备用电话")]
        public string BakTel { get; set; }
        [Display(Name = "登记微信号")]
        public string WeixinNo { get; set; }
        [Display(Name = "绑定微信")]
        public bool BindWeixin { get; set; }
        [Display(Name = "订单数")]
        public int OrderNum { get; set; }
        [Display(Name = "是否要售后")]
        public bool IsNeedCustomerService { get; set; }
        [Display(Name = "是否已回访")]
        public bool IsBack { get; set; }
        [Display(Name = "最近备注")]
        public string Remark { get; set; }
    }
    public class CustomerListModel
    {
        public int draw { get; set; }
        public int recordsFiltered { get; set; }
        public int NeedServiceCount { get; set; }
        public int BackCount { get; set; }
        public int WeixinBindCount { get; set; }
        public ShareSearchModel SearchModel { get; set; }
        public List<CustomerModel> data { get; set; }
    }
}
