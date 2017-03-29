$(document).ready(function($) {
    'use strict';
    var orderPre = orderPreInit();
    whatTmallSearchFor(orderPre);
});

function whatTmallSearchFor(orderPre) {
    $('body').on("click", '#reviseExtras', function(e) {
        $("#orderPre").data("forwhich", "reviseExtrasM");
        $("#orderPre").data("tmallorders", null);
    });
    $('body').on("click", '#revisePersonNum', function(e) {
        $("#orderPre").data("forwhich", "revisepeople12");
        $("#revisepeople12").data("tmallorders", null);
    });
    $('body').on("click", '#order-clone', function(e) {
        $("#orderPre").data("forwhich", "order-clone-modal");
        $("#order-clone-modal").data("tmallorders", null);
        $('#order-clone-modal #tmallOrders #orderNolist *:not(#tips)').remove();
    });
    $('body').on("click", '#reChange', function(e) {
        $("#orderPre").data("forwhich", "reChangeModel");
        $("#reChangeModel").data("tmallorders", null);
        $('#reChangeModel #tmallOrders #orderNolist *:not(#tips)').remove();
    });
    $('body').on("click", '#addTmallData', function(e) {
        $("#orderPre #TBIDX").val($('#baseinfo #CustomerTBCode').text());
        orderPre.rows().remove();
        orderPre.draw();
    });
    $('body').on("show.bs.modal", '#orderPre', function(e) {
        console.log("xx")
        if ($("#orderPre #TBIDX").val()) {
            $("#orderPre #checkTBListR").trigger("click");
        }
    });


    $("#orderPre #setOrders").on("click", function setOrders() {
        if ($("#orderPre #selecteNum").text() && parseInt($("#orderPre #selecteNum").text()) === 0) {
            $(this).success("请选择订单");
            return;
        }
        var tmallOrders = $("#" + $(this).closest("#orderPre").data("forwhich")).data("tmallorders");
        console.log(JSON.stringify(tmallOrders))
        tmallOrders = tmallOrders || {
            TBOrderID: {},
            subOrderList: {}
        };
        var subOrderData, subOrderRow, others;
        var i;
        for (i = 0; i < $("#orderPre .selectThisSubOrderPre:checked").length; i++) {
            subOrderRow = $("#orderPre .selectThisSubOrderPre:checked:eq(" + i + ")");
            others = subOrderRow.closest(".order-pre").data("others");
            subOrderData = subOrderRow.closest(".order-pre").data("suborder");
            subOrderData.Tid = others.Tid;
            subOrderData.OrderNo = others.OrderNo;
            if (subOrderData.Tid in tmallOrders.TBOrderID) {
                tmallOrders.TBOrderID[subOrderData.Tid].push = subOrderData.Oid;
            } else {
                tmallOrders.TBOrderID[subOrderData.Tid] = [subOrderData.Oid];
            }
            tmallOrders.subOrderList[subOrderData.Oid] = subOrderData;
        }
        console.log(tmallOrders)
        $("#" + $(this).closest("#orderPre").data("forwhich")).data("tmallorders", tmallOrders);
        $("#" + $(this).closest("#orderPre").data("forwhich")).trigger("updatetmallorders");
        $.LangHuaModal.closeModals(["revisepeople12", "reviseExtrasM", "order-clone-modal", "reChangeModel"]);
    });
    $("body ").on("updatetmallorders", "#reviseExtrasM,#revisepeople12,#order-clone-modal,#reChangeModel", function() {
        tmallOrders = $(this).data("tmallorders");
        var div = $("<div></div>");
        var span = $("<span></span>");
        var one = div.clone().addClass("one");
        var no = span.clone().addClass("no");
        var deletex = span.clone().addClass("delete");
        var arrTBOrderID = [];
        var has = false;
        for (var i in tmallOrders.TBOrderID) {
            has = true;
            arrTBOrderID.push(one.clone().append(
                no.clone().text(i),
                deletex.clone()
            ));
        }
        $(this).find("#tmallOrders #orderNolist *:not(#tips)").remove();
        if (has === true) {
            $(this).find("#tmallOrders #orderNolist #tips").hide();
        } else {
            $(this).find("#tmallOrders #orderNolist #tips").show();
        }
        $(this).find("#tmallOrders #orderNolist").append(arrTBOrderID);
    });
    $("body").on("click", "#tmallOrders #orderNolist .delete", function() {
        tmallOrders = $(this).closest(".modal").data("tmallorders");
        var theTBOrder = $(this).closest(".one");
        var TBOrderID = $.trim(theTBOrder.find(".no:eq(0)").text());
        for (var i in tmallOrders.TBOrderID[TBOrderID]) {
            delete tmallOrders.subOrderList[tmallOrders.TBOrderID[TBOrderID][i]];
        }
        delete tmallOrders.TBOrderID[TBOrderID];
        var has = false;
        for (i in tmallOrders.TBOrderID) {
            has = true;
            break;
        }
        console.log(tmallOrders)
        if (has === true) {
            $(this).closest("#tmallOrders").find("#orderNolist #tips").hide();
        } else {
            $(this).closest("#tmallOrders").find("#orderNolist #tips").show();
        }
        theTBOrder.remove();
    });
}

function orderPreInit() {

    $.fn.dataTable.ext.search.push(
        function(settings, data, dataIndex) {
            if (settings.sTableId !== "order-pre-list") { //仅仅对阅览表格起作用
                return true;
            }
            var statusAndOrderNo = data[0] || "any";
            if (statusAndOrderNo === 'any') {
                return true;
            } else {
                statusAndOrderNo = JSON.parse(statusAndOrderNo);
                var haveNotOrderedOnly = $("#orderPre #haveNotOrderedOnly").prop("checked");
                var enableOnly = $("#orderPre #enableOnly").prop("checked");
                if (
                    (
                        (!enableOnly) ||
                        (
                            (statusAndOrderNo.Status !== "TRADE_CLOSED") &&
                            (statusAndOrderNo.Status !== "TRADE_CLOSED_BY_TAOBAO")
                        )
                    ) &&
                    ((!haveNotOrderedOnly) || (!statusAndOrderNo.OrderNo))

                ) {
                    return true;
                } else {
                    return false;
                }
            }
        }
    );

    var orderPreList = // KEY:row data 
        jQuery("#order-pre-list").DataTable({
            "ordering": false,
            "data": [],
            'ordering': false,
            'searching': true,
            'serverSide': false,
            'initComplete': function(settings, json) {
                var api = this.api();
                var thistable = jQuery(this);
                thistable.on('change', ".selectAllSubOrdersPre", function() {
                    if ($(this).prop("checked")) {
                        $(this).closest('table.subOrdersGroup').find('.selectThisSubOrderPre').prop("checked", true);
                        $(this).closest('table.subOrdersGroup').find('.order-pre').addClass("selected");
                    } else {
                        $(this).closest('table.subOrdersGroup').find('.selectThisSubOrderPre').prop("checked", false);
                        $(this).closest('table.subOrdersGroup').find('.order-pre').removeClass("selected");
                    }
                    $("body").trigger("updateSelectAll");
                    $("#orderPre #selecteNum").text($(this).closest('#orderPre').find('.selectThisSubOrderPre:checked').length);
                });
                thistable.on('change', ".selectThisSubOrderPre", function() {
                    if ($(this).prop("checked")) {
                        $(this).closest('.order-pre').addClass("selected");

                        var NumberChecked = $(this).closest('table.subOrdersGroup').find('.selectThisSubOrderPre:checked').length;
                        var NumberAll = $(this).closest('table.subOrdersGroup').find('.selectThisSubOrderPre').length;
                        if (NumberChecked === NumberAll) {
                            $(this).closest('table.subOrdersGroup').find('.selectAllSubOrdersPre').prop("checked", true);

                        } else {
                            $(this).closest('table.subOrdersGroup').find('.selectAllSubOrdersPre').prop("checked", false);
                        }
                    } else {
                        $(this).closest('table.subOrdersGroup').find('.selectAllSubOrdersPre').prop("checked", false);
                        $(this).closest('.order-pre').removeClass("selected");
                    }
                    $("body").trigger("updateSelectAll");
                    $("#orderPre #selecteNum").text($(this).closest('#orderPre').find('.selectThisSubOrderPre:checked').length);
                });
            },
            "drawCallback": function(settings) {
                $('body').trigger('resize');
                $("#orderPre .selectThisSubOrderPre,#orderPre .selectAllSubOrdersPre,#orderPre #selectAllOrdersPre").each(function() {
                    $(this).prop("checked", false);
                });
                $("#orderPre #selecteNum").text("0");

            },
            "createdRow": function(row, data, dataIndex) {
                var _this = this.api();
                var thisTable = this;
                $(row).addClass("outerRow");

            },
            //行操作
            'columnDefs': [{
                'targets': [0],
                'data': 'Tid',
                'render': function(cellData, type, rowData, meta) {
                    if (type != "filter") {
                        return null;
                    } else {
                        return JSON.stringify({
                            "OrderNo": rowData.OrderNo,
                            "Status": rowData.Status,
                        });
                    }


                },
                "createdCell": function(tdx, cellData, rowData, row, col) {
                    var tablepreorderinnerwapper = $('<div class="table-preorder-inner-wapper"></div>');
                    var col = $("<col></col>");
                    var cogroup = $('<colgroup></colgroup>').append(
                        col.clone().prop('width', 36),
                        col.clone().prop('width', 350),
                        col.clone().prop('width', 220),
                        col.clone().prop('width', 90),
                        col.clone().prop('width', 130),
                        col.clone().prop('width', 105),
                        col.clone().prop('width', 133),
                        col.clone().prop('width', 85)
                    );
                    var flagColor = {
                        "1": "flag-red",
                        "2": "flag-yellow",
                        "3": "flag-green",
                        "4": "flag-blue",
                        "5": "flag-purple"
                    };
                    var tradeStatus = {
                        "TRADE_NO_CREATE_PAY": "没有创建支付宝交易",
                        "WAIT_BUYER_PAY": "等待买家付款",
                        "SELLER_CONSIGNED_PART": "卖家部分发货",
                        "WAIT_SELLER_SEND_GOODS": "等待卖家发货",
                        "WAIT_BUYER_CONFIRM_GOODS": "等待买家确认收货",
                        "TRADE_FINISHED": "交易成功",
                        //  "TRADE_CLOSED": "付款以后用户退款成功，交易自动关闭",
                        "TRADE_CLOSED": "退款成功，交易关闭",
                        //  "TRADE_CLOSED_BY_TAOBAO": "付款以前，卖家或买家主动关闭交易",
                        "TRADE_CLOSED_BY_TAOBAO": "付款以前，交易关闭",
                        "PAY_PENDING": "国际信用卡支付付款确认中",
                        "WAIT_PRE_AUTH_CONFIRM": "0元购合约中"
                    };
                    var refundStatus = {
                        "RefundStatus": "买家已经申请退款，等待卖家同意",
                        "WAIT_BUYER_RETURN_GOODS": "卖家已经同意退款，等待买家退货",
                        "WAIT_SELLER_CONFIRM_GOODS": "买家已经退货，等待卖家确认收货",
                        "SELLER_REFUSE_BUYER": "卖家拒绝退款",
                        "CLOSED": "退款关闭",
                        "SUCCESS": "退款成功"
                    };
                    var i = $("<i class='fa fa-flag'></i>");
                    var a = $("<a></a>");
                    var input = $('<input/>');
                    var span = $("<span></span>");
                    var div = $("<div></div>");
                    var checkbox = $('<input type="checkbox" />');
                    var img = $('<img/>');
                    var td = $('<td></td>');
                    var tr = $("<tr></tr>");
                    var tbody = $("<tbody></tbody>");
                    var table = $("<table></table>").addClass("subOrdersGroup");
                    table.addClass("table-preorder-inner");
                    var head, bottom
                    head = tr.clone().addClass("head").append(
                        td.clone().append(
                            input.clone().prop("type", "checkbox").addClass("selectAllSubOrdersPre")
                        ),
                        td.clone().prop("colspan", 7).append(
                            div.clone().addClass("left").append(
                                span.clone().addClass("one-seg more-space-left-5").text("淘宝订单号：").append(
                                    a.clone().text(rowData.Tid)
                                ),
                                span.clone().addClass("one-seg").text("成交：" + rowData.Created),
                                span.clone().addClass("one-seg").text("付款：" + (rowData.PayTime ? rowData.PayTime : "未付款")),
                                span.clone().addClass("one-seg").text("支付宝：").append(
                                    a.clone().text(rowData.BuyerAlipayNo)
                                )
                            ),
                            div.clone().addClass("right").append(
                                span.clone().addClass("one-seg").text(rowData.OrderNo ? "系统订单号：" : "").append(
                                    rowData.OrderNo ? a.clone().text(rowData.OrderNo) : span.clone().addClass("order-no-match")
                                )
                            )
                        )
                    );
                    bottom = tr.clone().addClass("bottom").append(
                        td.clone().append(
                            (parseInt(rowData.SellerFlag) !== 0) ? i.clone().addClass(flagColor[rowData.SellerFlag]) : null
                        ),
                        td.clone().prop("colspan", 5).append(
                            div.clone().addClass("left").append(
                                span.clone().addClass("one-seg more-space-left-5").text(rowData.SellerMemo ? rowData.SellerMemo : "")
                            )

                        ),
                        td.clone().prop("colspan", 2).append(
                            div.clone().addClass("right").append(
                                span.clone().addClass("one-seg").text("实收：").append(
                                    span.clone().addClass("paid-in").text(rowData.Payment)
                                )
                            )
                        )
                    );
                    var trArr = [];
                    var counter = 0;
                    rowspan = 0;
                    var lastTd, promotionArr, promotionEachCost;
                    for (var i in rowData.Orders) {
                        promotionEachCost = 0
                        promotionArr = [];
                        for (var j in rowData.PromotionDetails) {
                            if (rowData.PromotionDetails[j].Id == rowData.Orders[i].Oid) {
                                promotionArr.push(
                                    div.clone().css('margin-bottom', "3px").append(
                                        span.clone().addClass("Badge Badge-red").text(rowData.PromotionDetails[j].PromotionName)
                                    ));
                                promotionEachCost += parseFloat(rowData.PromotionDetails[j].DiscountFee)
                            }
                        }
                        if (counter === 0) {
                            rowspan = rowData.Orders.length;
                        } else {
                            rowspan = 1;
                        }
                        if (counter === 0) {
                            lastTd = td.clone().prop('rowspan', rowspan).addClass("border-left").text(tradeStatus[rowData.Status] ? tradeStatus[rowData.Status] : "未知");
                        } else {
                            lastTd = null;
                        }
                        trArr.push(tr.clone()
                            .data({
                                'index': i,
                                "fullinfo": rowData,
                                "suborder": rowData.Orders[i],
                                "others": {
                                    "Tid": rowData.Tid,
                                    "OrderNo": rowData.OrderNo
                                },
                                "itineraryinfo": rowData.Orders[i].ItineraryInfo,

                                "code": rowData.Orders[i].OuterSkuId
                            })
                            .addClass("order-pre")
                            .append(
                                td.clone().append(
                                    checkbox.clone().addClass("selectThisSubOrderPre")
                                ), //选择
                                td.clone().addClass("iteminfo").append(div.clone().addClass("iteminfo-ct").append( //详情
                                    div.clone().addClass("img").append(img.clone().prop("src", rowData.Orders[i].PicPath)),
                                    div.clone().addClass("detail").append(
                                        div.clone().addClass("title-long").text(rowData.Orders[i].Title).attr('title', rowData.Orders[i].Title),
                                        div.clone().addClass("extra").text(rowData.Orders[i].SkuPropertiesName).attr('title', rowData.Orders[i].SkuPropertiesName)
                                    )
                                )),
                                td.clone().addClass("text-left").append( //CODE
                                    div.clone().addClass("code-container").text(rowData.Orders[i].OuterSkuId ? rowData.Orders[i].OuterSkuId : "")
                                ),
                                td.clone().addClass("text-right").append( //单价数量
                                    div.clone().text(rowData.Orders[i].Price),
                                    div.clone().text("×" + rowData.Orders[i].Num)
                                ),
                                td.clone().append( //支付状态
                                    div.clone().text(tradeStatus[rowData.Orders[i].Status] ? tradeStatus[rowData.Orders[i].Status] : ""),
                                    div.clone().text(refundStatus[rowData.Orders[i].RefundStatus] ? refundStatus[rowData.Orders[i].RefundStatus] : "")
                                ),
                                td.clone().append( //优惠
                                    div.clone().text("-" + promotionEachCost),
                                    promotionArr
                                ),
                                td.clone().append( //实付金额
                                    div.clone().text(rowData.Orders[i].Payment)
                                ),
                                lastTd
                            ));
                        counter++
                    }
                    $(tdx).empty().append(
                        tablepreorderinnerwapper.append(
                            table.append(
                                cogroup,
                                tbody.append(
                                    head,
                                    trArr,
                                    bottom
                                )
                            )
                        )
                    );

                }
            }]
        });
    // 全选
    $("body").on("change", "#selectAllOrdersPre", function() {
        if ($(this).prop("checked")) {
            $('body #orderPre .selectThisSubOrderPre').each(function() {
                $(this).prop("checked", true);
                $(this).trigger("change");
            });
        } else {
            $('body #orderPre .selectThisSubOrderPre').each(function() {
                $(this).prop("checked", false);
                $(this).trigger("change")
            });
        }
    });
    $("body").on("updateSelectAll", function() {
        if ($('body #orderPre .selectThisSubOrderPre').length === $('body #orderPre .selectThisSubOrderPre:checked').length) {
            $('body #orderPre #selectAllOrdersPre').prop("checked", true);
        } else {
            $('body #orderPre #selectAllOrdersPre').prop("checked", false);
        }
    });
    $("body #orderPre  #flashOrderPre").on('click', { "orderPreList": orderPreList }, function(e) {
        if ($(this).hasClass("getting")) {
            return;
        }
        getTBListByTBID($.trim($("#orderPre  #TBIDX").val()), e.data.orderPreList);
        $(this).addClass("getting");
    });
    $("body #orderPre  #checkTBListR").on('click', { "orderPreList": orderPreList }, function(e) {
        if ($(this).hasClass("getting")) {
            return;
        }
        getTBListByTBID($.trim($("#orderPre  #TBIDX").val()), e.data.orderPreList);
        $(this).addClass("getting");
    });

    $("#haveNotOrderedOnly").on('change', function() {
        orderPreList.draw();
    });
    $("#enableOnly").on('change', function() {
        orderPreList.draw();
    });

    return orderPreList;
}

function getTBListByTBID(TBID, orderPreList) {
    $.ajax({
        'url': "/Orders/GetTBList",
        'type': "post",
        'data': {
            "BuyerNick": TBID
        },
        "dataType": 'json',
        "beforeSend": function() {
            $.LangHua.loadingToast({
                tip: "正 在 拉 取 信 息. . . . . ."
            });
            $("#orderPre  #flashOrderPre i:eq(0)").addClass("fa-spin");

        },
        "success": function(data) {
            $.LangHuaModal.closeModals([
                'reviseExtrasM',
                "revisepeople12",
                "order-clone-modal",
                "reChangeModel",
                "orderPre"
            ]);
            $("#orderPre  #flashOrderPre").removeClass("getting");
            $("#orderPre  #checkTBListR").removeClass("getting");
            $("#orderPre  #flashOrderPre i:eq(0)").removeClass("fa-spin");
            if (data.ErrorCode == 200) {
                orderPreList.rows().remove();
                $("#orderPre #TBIDX").val(TBID);
                $("#orderPre #enableOnly").prop("checked", true);
                $("#orderPre #haveNotOrderedOnly").prop("checked", false);
                $('#orderPre').modal('show');
                orderPreList.rows.add(data.data);

                orderPreList.draw();
            } else {
                if (data.ErrorCode == 401) {
                    $.LangHua.alert({
                        title: "提示信息",
                        tip1: '拉取数据失败',
                        tip2: data.ErrorMessage,
                        button: '确定'
                    })
                } else {
                    $.LangHua.alert({
                        title: "提示信息",
                        tip1: '拉取数据失败',
                        tip2: "拉取数据失败，请重试！",
                        button: '确定'
                    });
                }
            }
        },
        "error": function(jqXHR, textStatus, errorThrown) {
            $.LangHuaModal.closeModals([
                'reviseExtrasM',
                "revisepeople12",
                "order-clone-modal"
            ]);
            $("#orderPre  #flashOrderPre").removeClass("getting");
            $("#orderPre  #checkTBListR").removeClass("getting");
            $("#orderPre  #flashOrderPre i:eq(0)").removeClass("fa-spin");
            $.LangHua.alert({
                title: "提示信息",
                tip1: '拉取数据失败',
                tip2: "拉取数据失败，请检查网络，再重试！",
                button: '确定'
            })
        }
    })
}