using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanghuaNew.Data
{
    /// <summary>
    /// 支付宝转账
    /// </summary>
   public  class AlipayTransfer
    {
        public int AlipayTransferID { get; set;}

        [Display(Name = "订单来源")]
        public int OrderSourseID { get; set; }
        public virtual OrderSourse Sourse { get; set; }
        [Display(Name = "淘宝ID")]
        public string TBID { get; set; }
        [Display(Name = "淘宝订单号")]
        public string TBNum { get; set; }
        [Display(Name = "系统订单号")]
        public string OrderNo { get; set; }
        [Display(Name = "收款账号")]
        public string ReceiveAddress { get; set; }
        [Display(Name = "收款人姓名")]
        public string ReceiveName { get; set; }
        [Display(Name = "转账类型")]
        public TransferType TransferTypeValue { get; set; }
        [Display(Name = "转账金额")]
        public double TransferNum { get; set; }
        [Display(Name = "转账原因")]
        public string TransferReason{ get; set; }
        [Display(Name = "转账时间")]
        public DateTimeOffset TransferTime { get; set; }
        [Display(Name = "备注")]
        public string Remark { get; set; }
        [Display(Name = "状态")]
        public TransferState TransferStateValue { get; set; }
        [Display(Name = "操作日志")]
        public List<AlipayTransferLog> Logs { get; set; }

    }
    /// <summary>
    /// 支付宝转账操作日志
    /// </summary>
    public class AlipayTransferLog
    {
        public int AlipayTransferLogID { get; set; }
        [Display(Name = "支付宝转账")]
        public int AlipayTransferID { get; set; }
        public AlipayTransfer AlipayTransferItem { get; set; }

        public int UserID { get; set; }

        public string UserName { get; set; }
        [Display(Name = "操作类型")]
        public TransferOperate Operate { get; set; }
        [Display(Name = "操作时间")]
        public DateTimeOffset OperateTime { get; set; }
        [Display(Name = "备注")]
        public string Remark { get; set; }

    }
}
