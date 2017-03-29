using LanghuaNew.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LanghuaNew.Models
{
    public class AlipayTransferViewModel
    {
        public int AlipayTransferID { get; set; }
        [Display(Name = "订单来源")]
        public string OrderSourseName { get; set; }
        [Display(Name = "淘宝ID")]
        public string TBID { get; set; }
        [Display(Name = "淘宝订单号")]
        public string TBNum { get; set; }
        [Display(Name = "收款账号")]
        public string ReceiveAddress { get; set; }
        [Display(Name = "收款人姓名")]
        public string ReceiveName { get; set; }
        [Display(Name = "转账类型")]
        public TransferType TransferTypeValue { get; set; }
        public string TransferTypeName { get; set; }
        [Display(Name = "转账金额")]
        public double TransferNum { get; set; }
        [Display(Name = "转账原因")]
        public string TransferReason { get; set; }
        //[Display(Name = "转账时间")]
        //public string TransferTime { get; set; }
        [Display(Name = "备注")]
        public string Remark { get; set; }
        [Display(Name = "状态")]
        public TransferState TransferStateValue { get; set; }
        public string TransferStateName { get; set; }
    }
    //显示转账列表
    public class AlipayTransferList
    {
        public int draw { get; set; }
        public int recordsFiltered { get; set; }
        public float TransferThisMonth { get; set; }
        public float TransferBeforeMonth { get; set; }
        public float CheckTransfer { get; set; }
        public List<AlipayTransferViewModel> data { get; set; }
        public SearchModel SearchModel { get; set; }
    }
}