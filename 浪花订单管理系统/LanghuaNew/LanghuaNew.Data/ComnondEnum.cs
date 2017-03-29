using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/// <summary>
/// 所有的枚举类
/// </summary>
namespace LanghuaNew.Data
{
    /// <summary>
    /// 性别
    /// </summary>
    public enum sex
    {
       [Description("男")]
        Male,
        [Description("女")]
        Female
    }
    /// <summary>
    /// 订单状态（管理员|供应商）
    /// </summary>
    public enum OrderState
    {
        //新增
        [Description("未填写|未填写")]
        Notfilled,
        [Description("待核对|待核对")]
        Filled,
        [Description("已核对|已核对")]
        Check,
        [Description("已发送|新订单")]
        Send,
        [Description("新单已接|新单已接")]
        SendReceive,
        [Description("确认待检|已确认")]
        Confirm,
        [Description("已确认|已确认")]
        SencondConfirm,
        [Description("拒绝待检|已拒绝")]
        Full,
        [Description("已拒绝|已拒绝")]
        SencondFull,
        //取消
        [Description("请求取消|请求取消")]
        RequestCancel,
        [Description("取消待检|已取消")]
        Cancel,
        [Description("已取消|已取消")]
        SencondCancel,
        //变更
        [Description("请求变更|请求变更")]
        RequestChange,
        //作废
        [Description("已作废|已作废")]
        Invalid,
        //状态拆分
        [Description("取消已接|取消已接")]
        CancelReceive,
        [Description("变更已接|变更已接")]
        ChangeReceive,
        [Description("无效状态|无效状态")]
        nullandvoid,
    }
    /// <summary>
    /// 订单操作
    /// </summary>
    public enum OrderOperations
    {
        [Description("核对")]
        Check,
        [Description("发送")]
        Send,
        [Description("确认")]
        Confirm,
        [Description("取消")]
        Cancel,
        [Description("拒绝")]
        Full,
        [Description("请求取消")]
        RequestCancel,
        [Description("接单")]
        Receive,
        [Description("作废")]
        Invalid,
    }
    /// <summary>
    /// 订单操作角色（浪花朵朵or供应商）
    /// </summary>
    public enum OrderOperator
    {
        [Description("浪花朵朵")]
        inside,
        [Description("供应商")]
        supplier,
    }
    /// <summary>
    ///  订单状态（客户）
    /// </summary>
    public enum OrderCustomerState
    {
        [Description("未填写")]
        Notfilled = 0,
        [Description("已拒绝")]
        SencondFull = 10,
        [Description("待核对")]
        Filled = 20,
        [Description("已核对")]
        Check = 30,
        [Description("预定中")]
        Ordering = 40,
        [Description("变更中")]
        Changeing = 50,
        [Description("取消中")]
        Canceling = 60,
        [Description("已确认")]
        SencondConfirm = 70,
        [Description("已取消")]
        SencondCancel = 80,
    }
    /// <summary>
    /// 价格结算方式
    /// </summary>
    public enum PricingMethod
    {
  
        [Description("按游客人头数（例如一日游,门票）")]
        ByPerson,
        [Description("按产品数量（例如酒店,包车）")]
        other
    }
    /// <summary>
    /// 状态
    /// </summary>
    public enum EnableState
    {
        [Description("启用")]
        Enable,
        [Description("禁用")]
        Disable
    }
    /// <summary>
    /// 兑换方式
    /// </summary>
    public enum ChangeType
    {
        [Description("人民币-外币")]
        FromChina,
        [Description("外币-人民币")]
        ToChina
    }
    /// <summary>
    /// 生效方式
    /// </summary>
    public enum EffectiveWay
    {
        [Description("按下单日期")]
        ByCreateTime,
        [Description("按出行日期")]
        BySendTime
    }
    /// <summary>
    /// 规则类型
    /// </summary>
    public enum RuleType
    {
        [Description("日期范围")]
        ByDateRange,
        [Description("星期选择")]
        ByWeek,
        [Description("单双选择")]
        ByDouble,
        [Description("固定日期")]
        ByDate
    }
    //规则使用状态
    public enum RuleUseType
    {
        [Description("允许")]
        Allow,
        [Description("禁止")]
        Prohibit
    }
    /// <summary>
    /// 跟进方式
    /// </summary>
    public enum BackType
    {
        [Description("电话")]
        ByPhone,
        [Description("旺旺")]
        ByWangwang,
        [Description("微信")]
        ByWeixing,
        [Description("QQ")]
        ByQQ,
        [Description("当面")]
        ByMeet,
        [Description("其他")]
        ByOther

    }
    /// <summary>
    /// 订单导出字段
    /// </summary>
    public enum ExportField
    {
        [Description("订单号")]
        OrderNo,
        [Description("淘宝订单号")]
        TBOrderNo,
        [Description("淘宝ID")]
        TBID,
        [Description("中文姓名")]
        cnName,
        [Description("英文姓名")]
        enName,
        [Description("订单来源")]
        OrderSourse,
        [Description("联系电话")]
        Tel,
        [Description("备用联系电话")]
        BakTel,
        [Description("供应商")]
        SupplierCode,
        [Description("预定项目")]
        cnItemName,
        [Description("产品编码")]
        ServiceCode,
        [Description("成人人数")]
        AdultNum,
        [Description("儿童人数")]
        ChildNum,
        [Description("婴儿人数")]
        INFNum,
        [Description("间数")]
        RoomNum,
        [Description("晚数")]
        NightNum,
        [Description("出行日期")]
        TravelDate,
        [Description("订单状态")]
        OrderState,
        [Description("团号")]
        GroupNo,
        [Description("备注")]
        Remark,
        [Description("订单创建人")]
        CreateUserNikeName,
        [Description("所有附加项目")]
        ExtraServices
    }
    /// <summary>
    /// 转账类型
    /// </summary>
    public enum TransferType
    {
        [Description("退差价")]
        DifferenceReturn,
        [Description("售后赔偿")]
        Compensation,
        [Description("好评返现")]
        ReturnMoney
    }
    /// <summary>
    /// 操作类型
    /// </summary>
    public enum TransferOperate
    {
        [Description("新增")]
        Add,
        [Description("修改")]
        Edit,
        [Description("核实")]
        Check,
        [Description("转账")]
        Transfer,
        [Description("备注")]
        Remark,
        [Description("作废")]
        Delete,
    }
    //转账状态
    public enum TransferState
    {
        [Description("待核实")]
        New,
        [Description("待转账")]
        Check,
        [Description("已转账")]
        Transfer,
        [Description("已作废")]
        IsDelete
    }
    //对账状态
    public enum CheckState
    {
        [Description("待对账")]
        New,
        [Description("待支付")]
        Check,
        [Description("已支付")]
        Transfer,
        [Description("已作废")]
        IsDelete
    }
    //供应商价格维护状态
    public enum SupplierPriceState
    {
        [Description("√")]
        Seted,
        [Description("未设置")]
        NotSeted,
        [Description("已过期")]
        Expired
    }
    /// <summary>
    /// 用户操作
    /// </summary>
    public enum UserOperate
    {
        [Description("新增")]
        Add,
        [Description("修改")]
        Edit,
        [Description("删除")]
        Delete,
        [Description("禁用")]
        Disable,
        [Description("启用")]
        Enable,
        [Description("重置密码")]
        ResetPassWord,
        [Description("登录")]
        Login,
        [Description("解绑微信")]
        UnbindWeixin
    }
    /// <summary>
    /// 模块
    /// </summary>
    public enum Modular
    {
        [Description("工作台")]
        WorkTable,
        [Description("订单")]
        Order,
        [Description("客户")]
        Customer,
        [Description("产品")]
        Product,
        [Description("系统")]
        System
    }
    /// <summary>
    /// 二维码使用场景
    /// </summary>
    public enum QRcode
    {
        [Description("其他")]
        Other,
        [Description("确认单")]
        Voucher,
        [Description("电子邮件")]
        Email,
        [Description("衣服")]
        Clothes,
        [Description("旅行中")]
        Trip
    }
    public enum SellControlModelState
    {
        [Description("人数为0")]
        While,
        [Description("人数大于0")]
        Green,
        [Description("确认加上预扣大于0")]
        Yellow,
        [Description("人数超过控位数")]
        Red,
        [Description("规则禁止")]
        Gray,
    }
    public class EnumHelper
    {
        public static string GetEnumDescription(Enum enumValue)
        {
            string str = enumValue.ToString();
            System.Reflection.FieldInfo field = enumValue.GetType().GetField(str);
            object[] objs = field.GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false);
            if (objs == null || objs.Length == 0) return str;
            DescriptionAttribute da = (DescriptionAttribute)objs[0];
            return da.Description;
        }
    }

   
}
