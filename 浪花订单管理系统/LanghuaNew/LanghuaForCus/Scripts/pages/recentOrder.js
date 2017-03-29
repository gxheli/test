jQuery(document).ready(function ($) {
    $('body').on('click', "a.oneorder", function () {
        var state = $(this).closest(".order-tiny").data('state');
        switch (state) {
            case 10:
                $.LangHua.alert({
                    title:"联系客服",
                    tip1:'您的订单需要处理。',
                    tip2:'请联系旺旺客服帮您处理订单，谢谢！',
                    icon:"comment"
                })
                break;
            case 30:
                $.LangHua.alert({
                    title: "联系客服",
                    tip1: '您的订单需要处理。',
                    tip2: '请联系旺旺客服帮您处理订单，谢谢！',
                    icon: "comment"

                })
                
            default:
                break;
        }
    })
    
})
