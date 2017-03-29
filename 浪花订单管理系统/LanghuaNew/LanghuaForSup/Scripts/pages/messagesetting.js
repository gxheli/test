$(document).ready(function () {
    jQuery("#UnbindWeixin").bind('click', function () {
        var userid = jQuery("#SupplierUserID").val();
        var username = jQuery("#SupplierUserName").val();
        jQuery.LangHua.confirm({
            title: "提示信息",
            tip1: '请确认是否解绑下列用户的微信？',
            tip2: username,
            confirmbutton: '确定',
            cancelbutton: '取消',
            data: null,
            confirm: function () {
                $.ajax({
                    url: "/Users/UnbindWeixin/" + userid,
                    type: 'post',
                    dataType: 'json',
                    success: function (data) {
                        jQuery.LangHua.alert({
                            title: "提示信息",
                            tip1: '提示信息：',
                            tip2: data.ErrorMessage,
                            button: '确定'
                        })
                        if (data.ErrorCode == 200) {
                            jQuery("#weixinbind").text('未绑定');
                        }
                    }
                })
            }
        })
    })

    $('#BeginTime').timepicker({
        defaultTime: '',
        showMeridian: false,
        appendWidgetTo:$('#message')
    })
    $('#EndTime').timepicker({
        defaultTime: '',
        showMeridian: false,
        appendWidgetTo:$('#message')
    })
   
})

