using LanghuaNew.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commond
{
    public class OrderStateHelper
    {
        /// <summary>
        /// 订单状态流转
        /// </summary>
        /// <param name="state">订单状态</param>
        /// <param name="operation">操作</param>
        /// <param name="EditBy">操作角色(浪花朵朵or供应商)</param>
        /// <returns>订单状态</returns>
        public static OrderState GetOrderState(OrderState state, OrderOperations operation, OrderOperator EditBy)
        {
            switch (operation)
            {
                //case OrderOperations.Filled://填写
                //    //未填写→待核对
                //    if (state == OrderState.Notfilled && EditBy == OrderOperator.inside) return OrderState.Filled;
                //    break;
                case OrderOperations.Check://核对
                    //待核对→已核对
                    if (state == OrderState.Filled && EditBy == OrderOperator.inside) return OrderState.Check;
                    break;
                case OrderOperations.Send://发送
                    //已核对→已发送
                    if (state == OrderState.Check && EditBy == OrderOperator.inside) return OrderState.Send;
                    break;
                case OrderOperations.Receive://接单
                    //已发送→新单已接
                    if (state == OrderState.Send) return OrderState.SendReceive;
                    //请求取消→取消已接
                    if (state == OrderState.RequestCancel) return OrderState.CancelReceive;
                    //请求变更→变更已接
                    if (state == OrderState.RequestChange) return OrderState.ChangeReceive;
                    break;
                case OrderOperations.Confirm://确认
                    //新单已接→已确认
                    if (state == OrderState.SendReceive && EditBy == OrderOperator.inside) return OrderState.SencondConfirm;
                    //变更已接→已确认
                    if (state == OrderState.ChangeReceive && EditBy == OrderOperator.inside) return OrderState.SencondConfirm;
                    //确认待检→已确认
                    if (state == OrderState.Confirm && EditBy == OrderOperator.inside) return OrderState.SencondConfirm;
                    //已拒绝→已确认
                    if (state == OrderState.SencondFull && EditBy == OrderOperator.inside) return OrderState.SencondConfirm;

                    //新单已接→确认待检(供应商)
                    if (state == OrderState.SendReceive && EditBy == OrderOperator.supplier) return OrderState.Confirm;
                    //变更已接→确认待检(供应商)
                    if (state == OrderState.ChangeReceive && EditBy == OrderOperator.supplier) return OrderState.Confirm;
                    break;
                case OrderOperations.Full://拒绝
                    //新单已接→已拒绝
                    if (state == OrderState.SendReceive && EditBy == OrderOperator.inside) return OrderState.SencondFull;
                    //变更已接→已拒绝
                    if (state == OrderState.ChangeReceive && EditBy == OrderOperator.inside) return OrderState.SencondFull;
                    //取消已接→已拒绝
                    if (state == OrderState.CancelReceive && EditBy == OrderOperator.inside) return OrderState.SencondFull;
                    //拒绝待检→已拒绝
                    if (state == OrderState.Full && EditBy == OrderOperator.inside) return OrderState.SencondFull;

                    //新单已接→拒绝待检(供应商)
                    if (state == OrderState.SendReceive && EditBy == OrderOperator.supplier) return OrderState.Full;
                    //变更已接→拒绝待检(供应商)
                    if (state == OrderState.ChangeReceive && EditBy == OrderOperator.supplier) return OrderState.Full;
                    //取消已接→拒绝待检(供应商)
                    if (state == OrderState.CancelReceive && EditBy == OrderOperator.supplier) return OrderState.Full;
                    break;
                //case OrderOperations.RequestChange://请求变更
                //    if (state > OrderState.Send) return OrderState.RequestChange;
                //    break;
                case OrderOperations.RequestCancel://请求取消 待检状态只有一条路
                    if (state > OrderState.Send && state != OrderState.Cancel && state != OrderState.Full && state != OrderState.Confirm && state != OrderState.RequestCancel)
                        return OrderState.RequestCancel;
                    break;
                case OrderOperations.Cancel://取消
                    //取消已接→已取消
                    if (state == OrderState.CancelReceive && EditBy == OrderOperator.inside) return OrderState.SencondCancel;
                    //取消待检→已取消
                    if (state == OrderState.Cancel && EditBy == OrderOperator.inside) return OrderState.SencondCancel;

                    //取消已接→取消待检(供应商)
                    if (state == OrderState.CancelReceive && EditBy == OrderOperator.supplier) return OrderState.Cancel;
                    break;
                case OrderOperations.Invalid://作废
                    if (state < OrderState.SendReceive) return OrderState.Invalid;
                    break;
            }
            return OrderState.nullandvoid;
        }
        /// <summary>
        /// 根据订单状态获取客户状态
        /// </summary>
        /// <param name="state">订单状态</param>
        /// <param name="oldstate">订单上一个状态，只有在待检状态时需要</param>
        /// <returns>客户状态</returns>
        public static OrderCustomerState GetOrderCustomerState(OrderState state, OrderState? oldstate)
        {
            switch (state)
            {
                case OrderState.Notfilled:
                    return OrderCustomerState.Notfilled;
                case OrderState.Filled:
                    return OrderCustomerState.Filled;
                case OrderState.Check:
                    return OrderCustomerState.Check;

                case OrderState.Send:
                    return OrderCustomerState.Ordering;
                case OrderState.SendReceive:
                    return OrderCustomerState.Ordering;

                case OrderState.RequestCancel:
                    return OrderCustomerState.Canceling;
                case OrderState.CancelReceive:
                    return OrderCustomerState.Canceling;
                case OrderState.Cancel:
                    return OrderCustomerState.Canceling;

                case OrderState.RequestChange:
                    return OrderCustomerState.Changeing;
                case OrderState.ChangeReceive:
                    return OrderCustomerState.Changeing;

                case OrderState.Confirm:
                    if (oldstate == OrderState.ChangeReceive)
                    {
                        return OrderCustomerState.Changeing;
                    }
                    else if (oldstate == OrderState.CancelReceive)
                    {
                        return OrderCustomerState.Canceling;
                    }
                    else
                    {
                        return OrderCustomerState.Ordering;
                    }
                case OrderState.Full:
                    if (oldstate == OrderState.ChangeReceive)
                    {
                        return OrderCustomerState.Changeing;
                    }
                    else if (oldstate == OrderState.CancelReceive)
                    {
                        return OrderCustomerState.Canceling;
                    }
                    else
                    {
                        return OrderCustomerState.Ordering;
                    }

                case OrderState.SencondConfirm:
                    return OrderCustomerState.SencondConfirm;
                case OrderState.SencondFull:
                    return OrderCustomerState.SencondFull;
                case OrderState.SencondCancel:
                    return OrderCustomerState.SencondCancel;
                default:
                    return OrderCustomerState.Notfilled;
            }
        }
    }
}
