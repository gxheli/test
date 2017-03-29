jQuery(document).ready(function($) {

    // 订单流转
    $('body').on("click", " #tocheck", function() {
        var _this = $(this);
        var state = $(this).data('next-code');
        var arr = [];
        arr.push($(this).attr('data-orderid'));
        var OrderID = arr.join(",");
        var m = '';
        $.ajax({
            url: "/Orders/UpdateState",
            type: 'post',
            contentType: "application/json; charset=utf-8;",
            data: JSON.stringify({
                OrderID: OrderID,
                state: state
            }),
            dataType: 'json',
            beforeSend: function() {
                m = $.LangHua.loadingToast({
                    tip: "正在提交请求  . . ."
                });
            },
            success: function(data) {
                if (data.failed.length == 0) {
                    $.LangHua.alert({
                        'tip1': "核对结果",
                        "tip2": "核对成功！"
                    });
                    _this.trigger("update", [2]);
                    return;
                }
                var failed = data.failed;
                var str = '';
                for (var i in failed) {
                    var arr = [
                        '<div style="margin:10px 0px">',
                        '<span style="color:#0099cc">' + failed[i]['OrderNo'] + '：</span>',
                        '<spanstyle="color:#333" >' + failed[i]['reason'] + '</span>',
                        '</div>',
                    ].join('\n');
                    str += arr;

                }
                var t = [
                    '<div  class="modal modal-animate" tabindex="-1" data-backdrop="static" data-width="500" data-height="200">',
                    '<div class="modal-dialog " role="document">',
                    '<div class="modal-content">',
                    '<div class="modal-header">',
                    '<button type="button" class="close" data-dismiss="modal" aria-hidden="true"></button>',
                    '<h4 class="modal-title">操作结果</h4>',
                    '</div>',
                    '<div class="modal-body">',
                    str,
                    '</div>',
                    '<div class="modal-footer">',
                    '<a  class="btn btn-sm btn-primary button70" data-dismiss="modal">确定</a>',
                    '</div>',
                    '</div>',
                    '</div>',
                    '</div>',
                ].join("\n");
                $(t).modal();
            },
            failed: function() {
                $.LangHua.alert({
                    tip1: "请求失败！",
                    tip2: "请求失败!！稍后请重试"
                });
            },
            complete: function() {
                m.modal("hide");
            }
        });
    });
});