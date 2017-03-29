jQuery(document).ready(function($) {
    jQuery("#TransferTypeValue").bind('change', function() {
        if (jQuery(this).val() == 2) {
            if (!jQuery("#TransferNum").val()) {
                jQuery("#TransferNum").val(5)
            }
            if (!jQuery("#Remark").val()) {
                jQuery("#Remark").val("好评返现")
            }
        }
    })

    jQuery("#btnSave").one('click', function postData() {
        var _this = this;
        var Result = getPost();
        if (Result.post) {
            if (Result.item.alipayTransfer.length == 0) {
                return;
            }
            var checkOrderNo = jQuery('input.CheckOrderNo[name=CheckOrderNo]:checked').val();
            if (checkOrderNo == 1) {
                var value = jQuery.trim(jQuery('#OrderNo').val());
                $.ajax({
                    type: 'get',
                    dataType: 'json',
                    contentType: "application/json; charset=utf-8;",
                    url: '/AlipayTransfers/CheckOrderNo?OrderNo=' + value,
                    success: function (data) {
                        if (data.check == "false") {
                            jQuery('#OrderNo').warning("该系统订单号不存在");
                            jQuery(_this).one('click', postData);
                        }
                        else {
                            $.ajax({
                                type: 'post',
                                dataType: 'json',
                                contentType: "application/json; charset=utf-8;",
                                data: JSON.stringify(Result.item),
                                url: '/AlipayTransfers/SaveAlipayTransfer',
                                success: function (data) {
                                    if (data.ErrorCode == 200) {
                                        window.location.href = "/AlipayTransfers";
                                    } else {
                                        if (data.ErrorCode == 401) {
                                            $.LangHua.alert({
                                                title: "提示信息",
                                                tip1: '提交失败',
                                                tip2: data.ErrorMessage,
                                                button: '确定'
                                            })
                                        }
                                        jQuery(_this).one('click', postData);
                                    }
                                },
                                failed: function () {
                                    jQuery(_this).one('click', postData);
                                }
                            });
                        }
                    }
                });
            }
            else {
                $.ajax({
                    type: 'post',
                    dataType: 'json',
                    contentType: "application/json; charset=utf-8;",
                    data: JSON.stringify(Result.item),
                    url: '/AlipayTransfers/SaveAlipayTransfer',
                    success: function (data) {
                        if (data.ErrorCode == 200) {
                            window.location.href = "/AlipayTransfers";
                        } else {
                            if (data.ErrorCode == 401) {
                                $.LangHua.alert({
                                    title: "提示信息",
                                    tip1: '提交失败',
                                    tip2: data.ErrorMessage,
                                    button: '确定'
                                })
                            }
                            jQuery(_this).one('click', postData);
                        }
                    },
                    failed: function () {
                        jQuery(_this).one('click', postData);
                    }
                });
            }
        } else {
            jQuery(_this).one('click', postData);
        }
    })

    function getPost() {
        var post = true;

        var alipayTransfer = {
            AlipayTransferID: jQuery.trim(jQuery('#AlipayTransferID').val()),
            OrderSourseID: (function() {
                var value = jQuery('#OrderSourse input.OrderSourse[name=OrderSourse]:checked').val();
                if (!value) {
                    jQuery('#OrderSourse').warning("请选择");
                    post = false;
                }
                return value;
            })(),
            TBID: (function() {
                var value = jQuery.trim(jQuery('#TBID').val());
                if (!value) {
                    jQuery('#TBID').warning("请填写");
                    post = false;
                }
                return value;
            })(),
            OrderNo: (function() {
                var checkOrderNo = jQuery('input.CheckOrderNo[name=CheckOrderNo]:checked').val();
                var value = jQuery.trim(jQuery('#OrderNo').val());
                if (checkOrderNo == 1) {
                    if (!value) {
                        jQuery('#OrderNo').warning("请填写");
                        post = false;
                    }
                } else {
                    value = "";
                }
                return value;
            })(),
            ReceiveAddress: (function() {
                var value = jQuery.trim(jQuery('#ReceiveAddress').val())
                if (!value) {
                    jQuery('#ReceiveAddress').warning("请填写");
                    post = false;
                }
                return value;
            })(),
            ReceiveName: jQuery.trim(jQuery('#ReceiveName').val()),
            TransferTypeValue: (function() {
                var value = jQuery.trim(jQuery('#TransferTypeValue').val())
                if (value < 0) {
                    jQuery('#TransferTypeValue').warning("请选择");
                    post = false;
                }
                return value;
            })(),
            TransferNum: (function() {
                var value = jQuery.trim(jQuery('#TransferNum').val())
                if (!value) {
                    jQuery('#TransferNum').warning("请填写");
                    post = false;
                }
                return value;
            })(),
            TransferReason: (function() {
                var value = jQuery.trim(jQuery('#Remark').val())
                if (!value) {
                    jQuery('#Remark').warning("请填写");
                    post = false;
                }
                return value;
            })()
        };
        if (window.location.href.indexOf("Create") != -1) {
            var isAdd = true;
        } else {
            var isAdd = false;
        }
        return {
            post: post,
            item: { isAdd: isAdd, alipayTransfer: alipayTransfer }
        };
    }

    jQuery("#OrderNo").on("click", function() {
        $(this).closest(".col-md-10").find("input.CheckOrderNo:eq(0)").prop("checked", true)
    })







})