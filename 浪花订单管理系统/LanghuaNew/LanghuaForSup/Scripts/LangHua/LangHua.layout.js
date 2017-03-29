var toast = '';
var modalToast = "";
$('body').on('shown.bs.modal', '.modal-animate', function(e) {
    $(this).animateCss("zoomIn");
});
window.onbeforeunload = function(e) {
    if (toast != 'a') {
        modalToast = $.LangHua.loadingToast({
            tip: '正 在 加 载  . .  .   .    .'
        });
    } else {
        toast = '';
    }
}
window.onunload = function(e) {
    modalToast.modal('hide');
    modalToast = null;
}
$('body').on('click', '#exportFieldSave', function() {
    toast = 'a';
});
$('body').on('click', '#exportproducts', function() {
    toast = 'a';
});
$('body').on('click', '.LINKDOWNLOAD', function() {
    toast = 'a';
});
if (jQuery.fn.datepicker) {
    $.fn.datepicker.defaults.autoclose = true;
    $.fn.datepicker.defaults.orientation = '';
    $.fn.datepicker.defaults.format = 'yyyy-mm-dd';
    $.fn.datepicker.defaults.startView = 0;
    $.fn.datepicker.defaults.startDate = '1921-01-01';
    $.fn.datepicker.defaults.todayBtn = 'linked';
    $.fn.datepicker.defaults.clearBtn = true;
    $.fn.datepicker.defaults.language = 'zh-CN';
    $.fn.datepicker.defaults.todayHighlight = true;
    $.fn.datepicker.defaults.zIndexOffset = 300;

    $.fn.datepicker.defaults.templates = {
        leftArrow: '＜',
        rightArrow: '＞'
    }
}

// $.fn.modal.defaults.maxHeight = function(){
//     return $(window).height() - 114; 
// }
jQuery(document).ready(function() {
    var obj = jQuery('#controlleraction').data();
    jQuery('li.' + obj.controller).eq(0).addClass("active");
    jQuery('li.' + obj.controller).find('li.' + obj.action).eq(0).addClass("active");

    jQuery('body').on("click", ' .inputicon', function() {

        jQuery(this).siblings("input:eq(0)").focus();
    })
})



jQuery.ajaxSetup({
    beforeSend: function(xhr) {
        (this.beforeSend);
        var date = new Date().valueOf();
        if (this.type == "GET") {
            if (this.url.indexOf("?") == -1) {
                this.url = this.url + "?timestampFlag=" + date;
            } else {
                this.url = this.url + "&timestampFlag=" + date;
            }
        }
    }
})

$(document).ajaxSuccess(function(event, request, settings, data) {
    if (data.ErrorCode == 600) {
        $.LangHua.alert({
            title: "提示信息",
            tip1: '提示信息：',
            tip2: data.ErrorMessage,
            button: '确定',
            icon: "warning",
            callback: function() {
                window.location.reload();
            }

        })
    }
});
$(function() {
    $("body").on("click", '[data-toggle="tooltip"]', function() {
        $(this).tooltip();
    })

})

// $.event.special.copy.options = {

//     // The default action for the W3C Clipboard API spec (as it stands today) is to
//     // copy the currently selected text [and specificially ignore any pending data]
//     // unless `e.preventDefault();` is called.
//     requirePreventDefault: true,

//     // If HTML has been added to the pending data, this plugin can automatically
//     // convert the HTML into RTF (RichText) for use in non-HTML-capable editors.
//     autoConvertHtmlToRtf: true,

//     // SWF inbound scripting policy: page domains that the SWF should trust.
//     // (single string, or array of strings)
//     //trustedDomains: ZeroClipboard.config("trustedDomains"),

//     // The CSS class name used to mimic the `:hover` pseudo-class
//     hoverClass: "hover",

//     // The CSS class name used to mimic the `:active` pseudo-class
//     activeClass: "active"

// };