jQuery(document).ready(function () {

    jQuery("#weixin").bind('click', function () {
        var id = jQuery("#CustomerID").val();
        jQuery.LangHua.confirm({
            title: "提示信息",
            tip1: '请确认是否解绑微信号？',
            tip2: '',
            confirmbutton: '确定',
            cancelbutton: '取消',
            data: null,
            confirm: function () {
                $.ajax({
                    url: "/Customers/UnbindWeixin",
                    type: 'post',
                    contentType: "application/json; charset=utf-8;",
                    data: JSON.stringify({
                        id: id
                    }),
                    dataType: 'json',
                    success: function (data) {
                        if (data.ErrorCode == 200) {
                            window.location.href = "/Customers/Details/" + id;
                        }
                        else {
                            jQuery("#weixin").success(data.ErrorMessage);
                        }
                    }
                })
            }
        })
    })
    jQuery("#btnUpdate").bind('click', function () {
        jQuery("#newCustomerEnname").prop("readonly", false);
        jQuery("#newTel").prop("readonly", false);
        jQuery("#newEmail").prop("readonly", false);
        jQuery("#newCustomerName").prop("readonly", false);
        jQuery("#newBakTel").prop("readonly", false);
        jQuery("#newWechat").prop("readonly", false);
        jQuery("#btnUpdate").addClass('hidden');
        jQuery("#btnUpdateSave").removeClass('hidden')
        jQuery("#btnCancal").removeClass('hidden')
    })
    jQuery("#btnCancal").bind('click', function () {

        jQuery("#newCustomerEnname").val(jQuery("#CustomerEnname").val());
        jQuery("#newTel").val(jQuery("#Tel").val());
        jQuery("#newEmail").val(jQuery("#Email").val());
        jQuery("#newCustomerName").val(jQuery("#CustomerName").val());
        jQuery("#newBakTel").val(jQuery("#BakTel").val());
        jQuery("#newWechat").val(jQuery("#Wechat").val());

        jQuery("#newCustomerEnname").prop("readonly", true);
        jQuery("#newTel").prop("readonly", true);
        jQuery("#newEmail").prop("readonly", true);
        jQuery("#newCustomerName").prop("readonly", true);
        jQuery("#newBakTel").prop("readonly", true);
        jQuery("#newWechat").prop("readonly", true);
        jQuery("#btnUpdate").removeClass('hidden')
        jQuery("#btnUpdateSave").addClass('hidden')
        jQuery("#btnCancal").addClass('hidden')
    })
    jQuery("#btnUpdateSave").bind('click', function () {
        var CustomerID = jQuery("#CustomerID").val();
        var CustomerEnname = jQuery("#newCustomerEnname").val();
        var Tel = jQuery("#newTel").val();
        var Email = jQuery("#newEmail").val();
        var CustomerName = jQuery("#newCustomerName").val();
        var BakTel = jQuery("#newBakTel").val();
        var Wechat = jQuery("#newWechat").val();

        var customer = {
            CustomerID: CustomerID,
            CustomerEnname: CustomerEnname,
            Tel: Tel,
            Email: Email,
            CustomerName: CustomerName,
            BakTel: BakTel,
            Wechat: Wechat,
        }
        $.ajax({
            url: "/Customers/Edit",
            type: 'post',
            contentType: "application/json; charset=utf-8;",
            data: JSON.stringify(customer),
            dataType: 'json',
            success: function (data) {
                if (data.ErrorCode == 200) {
                    jQuery.LangHua.alert({
                        title: "提示信息",
                        tip1: '保存成功',
                        tip2: '',
                        button: '确定',
                        callback: function () {
                            window.location.href = "/Customers/Details/" + CustomerID;
                        }
                    })
                   
                }
                else if (data.ErrorCode == 401) {
                    jQuery.LangHua.alert({
                        title: "提示信息",
                        tip1: data.ErrorMessage,
                        tip2: '',
                        button: '确定'
                    })
                }
                else {
                    jQuery.LangHua.alert({
                        title: "提示信息",
                        tip1: '保存失败',
                        tip2: '',
                        button: '确定'
                    })
                }
            }
        })
    })
    jQuery("#password").bind('click', function () {
        var id = jQuery("#CustomerID").val();
        jQuery.LangHua.confirm({
            title: "提示信息",
            tip1: '请确认是否重置密码？',
            tip2: '',
            confirmbutton: '确定',
            cancelbutton: '取消',
            data: null,
            confirm: function () {
                $.ajax({
                    url: "/Customers/ResetPassWord",
                    type: 'post',
                    contentType: "application/json; charset=utf-8;",
                    data: JSON.stringify({
                        id: id
                    }),
                    dataType: 'json',
                    success: function (data) {
                        jQuery.LangHua.alert({
                            title: "提示信息",
                            tip1: data.ErrorMessage,
                            tip2: '',
                            button: '确定'
                        })
                    }
                })
            }
        })
    })
    jQuery('body').on('click', '.sendmessage', function () {
        var message = jQuery(this).text();
        jQuery("#Remark").val(jQuery("#Remark").val() + ' ' + message);
    })
    jQuery("#btnSave").one('click', function save() {
        var post = true;
        var back = {
            "CustomerID": jQuery("#CustomerID").val(),
            "CustomerBackType": (function () {
                var value = jQuery('#BackType').val()
                if (!value) {
                    jQuery('#BackType').warning("请选择");
                    post = false;
                }
                return value;
            })(),
            "Remark": (function () {
                var value = jQuery('#Remark').val()
                if (!value) {
                    jQuery('#Remark').warning("请填写");
                    post = false;
                }
                return value;
            })(),
        }
        if (post) {
            $.ajax({
                type: 'post',
                dataType: 'json',
                contentType: "application/json; charset=utf-8;",
                data: JSON.stringify({ back: back, isBack: jQuery("#isBack").prop("checked") }),
                url: '/Customers/SaveCustomerBack',
                success: function (data) {
                    if (data.ErrorCode == 200) {
                        window.location.href = "/Customers/Details/" + jQuery("#CustomerID").val();
                    }
                }
            });
        }
        else {
            jQuery("#btnSave").one('click', save);
        }
    })
})