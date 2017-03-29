$(document).ready(function () {
    $('#btnSave').bind('click', function () {
        var post = true;
        var data = {
            UserID: $('#UserID').val(),
            PassWord: (function () {
                var value = jQuery.trim(jQuery('#PassWord').val());
                if (!value) {
                    jQuery('#PassWord').warning("请填写");
                    post = false;
                }
                return value;
            })(),
            newPassWord: (function () {
                var value = jQuery.trim(jQuery('#newPassWord').val());
                var value2 = jQuery.trim(jQuery('#newPassWord2').val());
                if (!value) {
                    jQuery('#newPassWord').Warning({
                        title: "请填写",
                        attach: ['#newPassWord2']
                    });
                    post = false;
                }
                if (!value2) {
                    jQuery('#newPassWord2').Warning({
                        title: "请填写",
                        attach: ['#newPassWord']
                    });
                    post = false;
                }
                if (value.length < 6 || value.length > 12) {
                    jQuery("#newPassWord").Warning({
                        title: "密码必须6-12位数",
                        attach: ['#newPassWord2']
                });
                    post = false;
                }
                if (value != value2) {
                    jQuery('#newPassWord2').Warning({
                        title: "2次密码输入不一致",
                        attach: ['#newPassWord']
                });
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
                data: JSON.stringify(data),
                url: '/Users/UpdateUserData',
                success: function (data) {
                    if (data.ErrorCode == 200) {
                        jQuery.LangHua.alert({
                            title: "提示信息",
                            tip1: '保存成功',
                            tip2: '请牢记您的密码',
                            button: '确定',
                            callback: function () {
                                window.location.href = "/Users/LogOut";
                            }
                        })
                    }
                    else if (data.ErrorCode == 401) {
                        jQuery.LangHua.alert({
                            title: "提示信息",
                            tip1: '提示信息：',
                            tip2: data.ErrorMessage,
                            button: '确定'
                        })
                    }
                    else {
                        jQuery.LangHua.alert({
                            title: "提示信息",
                            tip1: '保存失败！',
                            tip2: '',
                            button: '确定'
                        })
                    }
                }
            });
        }
    })
})