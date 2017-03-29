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
    /// 订单
    /// </summary>
    public class Order
    {
        public int OrderID { get; set; }
   
        [Display(Name = "订单编号")]
        public string OrderNo { get;set; }
        [Display(Name = "产品")]
        public ServiceItemHistory ServiceItemHistorys { get; set; }
        [Display(Name = "淘宝订单id")]
        public int TBOrderID { get; set; }
        public TBOrder TBOrders{ get; set; }
      
        [Display(Name = "预订客户")]
        public int CustomerID { get; set; }
        public Customer Customers { get; set; }
       
        [Display(Name = "预订时间")]
        public DateTimeOffset OrderDate { get; set; }
      
        [Display(Name = "订单状态")]
        public OrderState state { get; set; }
        [Display(Name = "客户订单状态")]
        public OrderCustomerState CustomerState { get; set; }

        [Display(Name = "是否支付")]
        public bool IsPay { get; set; }
        [Display(Name = "是否已正式")]
        public bool IsConfirm { get; set; }
        [Display(Name = "是否紧急订单")]
        public bool isUrgent { get; set; }
        [Display(Name = "发起人")]
        public int CreateUserID { get; set; }
        [Display(Name = "发起人昵称")]
        public string CreateUserNikeName { get; set; }
        [Display(Name = "订单日志")]
        public List<OrderHistory> OrderHistorys { get; set; }
        [Display(Name = "供应商订单日志")]
        public List<OrderSupplierHistory> OrderSupplierHistorys { get; set; }
        [Display(Name = "生成时间")]
        public DateTimeOffset CreateTime { get; set; }
        [Display(Name = "是否要售后")]
        public bool IsNeedCustomerService { get; set; }
        //[Display(Name = "要售后信息")]
        //public List<CustomerService> CustomerServices { get; set; }
        [Display(Name = "备注")]
        public string Remark { get; set; }
        [Display(Name = "淘宝订单号")]
        public string TBNum { get; set; }
        [Display(Name = "淘宝订单号")]
        public List<TBOrderNo> TBOrderNos { get; set; }

        //联系人信息
        [Display(Name = "中文名")]
        public string CustomerName { get; set; }
        [Display(Name = "拼音")]
        public string CustomerEnname { get; set; }
        [Display(Name = "电话")]
        public string Tel { get; set; }
        [Display(Name = "备用电话")]
        public string BakTel { get; set; }
        [Display(Name = "Email")]
        public string Email { get; set; }
        [Display(Name = "微信号")]
        public string Wechat { get; set; }
    }
    public class TBOrderNo
    {
        public int TBOrderNoID { get; set; }
        [Display(Name = "订单")]
        public int OrderID { get; set; }
        public virtual Order order { get; set; }
        public string No { get; set; }
        public string SubNo { get; set; }
        public float Payment { get; set; }
        public float RefundFee { get; set; }
        public float PaymentSplit { get; set; }
        public float RefundFeeSplit { get; set; }
    }
    /// <summary>
    /// 订单日志
    /// </summary>
    public class OrderHistory
    {
        public int OrderHistoryID { get; set; }
        [Display(Name = "订单")]
        public int OrderID { get; set; }
        public virtual Order order { get; set; }
        [Display(Name = "处理人")]
        public string OperUserID { get; set; }
        public string OperUserNickName { get; set; }
        [Display(Name = "处理时间")]
        public DateTimeOffset OperTime { get; set; }
        [Display(Name = "备注")]
        public string Remark { get; set; }
        [Display(Name = "订单状态")]
        public OrderState State { get; set; }
    }
    /// <summary>
    /// 供应商日志
    /// </summary>
    public class OrderSupplierHistory
    {
        public int OrderSupplierHistoryID { get; set; }
        [Display(Name = "订单")]
        public int OrderID { get; set; }
        public virtual Order order { get; set; }
        [Display(Name = "处理人")]
        public int OperUserID { get; set; }
        public string OperNickName { get; set; }
        [Display(Name = "操作角色")]
        public OrderOperator opera { get; set; }
        [Display(Name = "处理时间")]
        public DateTimeOffset OperTime { get; set; }
        [Display(Name = "备注")]
        public string Remark { get; set; }
        [Display(Name = "订单状态")]
        public OrderState State { get; set; }
    }
}
