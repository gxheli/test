; (function ($) {
    if (LangHua) {
        alert("LangHua is defined");
    }
    else {

    }
    var LangHua = new Object();
    //alert
    LangHua.alert = function (options) {
        var defaults = {
            title: "提示信息",
            tip1: '提示信息：',
            tip2: '提示信息：',
            button: '确定',
            icon: "warning",
            data:"",
            indent: true,
            callback:false
        };
       

        var settings = $.extend({}, defaults, options);
        var className="";
        switch (settings.icon) {
            case 'warning':
                className = "fa fa-warning";
                break;
            case 'comment':
                className = "fa fa-commenting";
                break;
           
        
            default:
                className="fa fa fa-warning";
                break;
        }
        switch (settings.indent) {
            case false:
                indentBottom = "bottom no-indent";
                break;
            case true:
                indentBottom = "bottom";
                break;


            default:
                indentBottom = "bottom ";
                break;
        }
        var x = '&times;'
        if (settings.callback) {
            x = "";
        }
        var date = new Date().valueOf();
        var str = [
            '<div class="modal modal-animate" id="alertLH' + date + '"+ tabindex="-1" data-backdrop="static" role="dialog" aria-labelledby="exampleModalLabel">',
                '<div class="modal-dialog " role="document">',
                    '<div class="modal-content">',
                        '<div class="modal-header">',
                            '<button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">'+x+'</span></button>',
                            '<span class="modal-title" id="exampleModalLabel">' + settings.title + '</span>',
                        '</div>',
                        '<div class="modal-body">',
                            '<div class="modalTips">',
                                '<div class="iconx">',
                                '<i class="'+className+'" ></i>',
                                '</div>',
                                '<div class="tips">',
                                    '<div class="top">' + settings.tip1 + '</div>',
                                    '<div class="' + indentBottom + '">' + settings.tip2 + '</div>',
                                '</div>',
                            '</div>',
                        '</div>',
                        '<div class="modal-footer">',
                            '<button id="button'+date+'" type="button" class="btn btn-primary button70" data-toggle="modal" data-dismiss="modal" data-whatever="">' + settings.button + '</button>',
                        '</div>',
                    '</div>',
                '</div>',
            '</div>',
        ].join('');
        jQuery('body').on('shown.bs.modal', "#alertLH" + date, function () {
            var _this = this;
            jQuery("#button" + date).one('click', function () {
                if (settings.callback) {
                    settings.callback(settings.data);
                }
                var animationEnd = 'webkitAnimationEnd mozAnimationEnd MSAnimationEnd oanimationend animationend';
                $(_this).addClass('animate200 fadeOut ').one(animationEnd, function () {
                    $(_this).removeClass('animate200 fadeOut ');
                    jQuery('#miss' + date).trigger('click');
                });
            })
           
            
        })
        jQuery(str).modal();
    }

    LangHua.showResult = function (options) {
        var defaults = {
            title: "提示信息",
            tip1: '提示信息：',
            tip2: '提示信息：',
            content:"",
            confirmbutton: '确定',
            data: null,
            confirm: function () {
                return;
            }
        };
        var date = new Date().valueOf();
        var settings = $.extend({}, defaults, options);
        var str = [
        '<div class="modal modal-animate showResultLH" id="modal' + date + '" tabindex="-1" data-backdrop="static" role="dialog" data-width="500" data-height="200">',
                '<div class="modal-dialog " role="document">',
                    '<div class="modal-content">',
                        '<div class="modal-header">',
                            '<button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true"></span></button>',
                            '<span class="modal-title" id="exampleModalLabel">' + settings.title + '</span>',
                        '</div>',
                        '<div class="modal-body">',
                            settings.content,
                        '</div>',
                        '<div class="modal-footer">',
                            '<button  type="button" class="btn btn-primary button70" data-toggle="modal" id="' + date + '"  >' + settings.confirmbutton + '</button>',
                        '</div>',
                    '</div>',
                '</div>',
            '</div>',
        ].join('');
        jQuery('body').on('shown.bs.modal', "#modal" + date, function () {
            var _this = this;
            jQuery('#' + date).one("click", function () {
                jQuery(this).prop("disabled", true).text("正在处理..");
                settings.confirm.call(this, this, settings.data);
                var animationEnd = 'webkitAnimationEnd mozAnimationEnd MSAnimationEnd oanimationend animationend';
                $(_this).addClass('animate200 fadeOut ').one(animationEnd, function () {
                    $(_this).removeClass('animate200 fadeOut ');
                    jQuery('#miss' + date).trigger('click');
                });
            })
        })

        jQuery(str).modal({
        })

    }

    //confirm
    LangHua.confirm = function (options) {
        var defaults = {
            title: "提示信息",
            tip1: '提示信息：',
            tip2: '提示信息：',
            confirmbutton: '确定',
            cancelbutton: '取消',
            data: null,
            confirm: function () {
                return;
            }

        };
        var date = new Date().valueOf();


        var settings = $.extend({}, defaults, options);
        var str = [
            '<div class="modal modal-animate confirmLH" id="modal' + date + '" tabindex="-1" data-backdrop="static" role="dialog" aria-labelledby="exampleModalLabel">',
                '<div class="modal-dialog " role="document">',
                    '<div class="modal-content">',
                        '<div class="modal-header">',
                            '<button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>',
                            '<span class="modal-title" id="exampleModalLabel">' + settings.title + '</span>',
                        '</div>',
                        '<div class="modal-body">',
                            '<div class="modalTips">',
                                '<div class="icon"></div>',
                                '<div class="tips">',
                                    '<div class="top">' + settings.tip1 + '</div>',
                                    '<div class="bottom">' + settings.tip2 + '</div>',
                                '</div>',
                            '</div>',
                        '</div>',
                        '<div class="modal-footer">',
                            '<button  type="button" class="btn btn-primary button70" data-toggle="modal" id="' + date + '"  >' + settings.confirmbutton + '</button>',
                            '<button  type="button" class="btn btn-default button70" data-toggle="modal" id="miss' + date + '" data-dismiss="modal" data-whatever="">' + settings.cancelbutton + '</button>',
                        '</div>',
                    '</div>',
                '</div>',
            '</div>',
        ].join('');
        jQuery('body').on('shown.bs.modal', "#modal" + date, function () {
            var _this = this;
            jQuery('#' + date).one("click", function () {
                jQuery(this).prop("disabled", true).text("正在处理..");
                settings.confirm.call(this, this, settings.data);
                var animationEnd = 'webkitAnimationEnd mozAnimationEnd MSAnimationEnd oanimationend animationend';
                $(_this).addClass('animate200 fadeOut ').one(animationEnd, function () {
                    $(_this).removeClass('animate200 fadeOut ');
                    jQuery('#miss' + date).trigger('click');
                });
            })
        })

        jQuery(str).modal({
        })
    }
    //toast
    LangHua.toast = function (options) {
        var defaults = {
            tip: "提示信息",


        };

        var settings = $.extend({}, defaults, options);
        var str = [
            '<div class="modal "  tabindex="-1"  role="dialog" aria-labelledby="exampleModalLabel">',
                '<div class="toasting"> ',
                settings.tip,
                '</div>',
            '</div>',
        ].join('');
        var thitoast = jQuery(str).modal({
            attentionAnimation: '',
            backdrop: 'static',
            width: 500,
        });

        return thitoast;
    }
    //blocktoast
    LangHua.blockToast = function (options) {
        var defaults = {
            tip: "提示信息",


        };

        var settings = $.extend({}, defaults, options);
        var str = [
            '<div class="modal "  tabindex="-1"  role="dialog" aria-labelledby="exampleModalLabel">',
                '<div class="toasting"> ',
                settings.tip,
                '</div>',
            '</div>',
        ].join('');
        var thitoast = jQuery(str).modal({
            attentionAnimation: '',
            backdrop: "static",
            width: 500,
        });

        return thitoast;
    }

    //loading
    LangHua.loadingToast = function (options) {
        var defaults = {
            tip: "提示信息",


        };

        var settings = $.extend({}, defaults, options);
        var str = [
            '<div class="modal "  tabindex="-1"  role="dialog" aria-labelledby="exampleModalLabel" style="height:60px !important;text-align:center;padding:10px 0px !important;top:50% !important;background:rgba(45,45,45,1);">',
                '<div style="display:inline-block;width:40px;height:40px;position:relative;vertical-align:middle">',
                    '<div class="progress-langhua medium circles" style="display: block;">',
                        '<span class="circle"></span>',
                        '<span class="circle"></span>',
                        '<span class="circle"></span>',
                        '<span class="circle"></span>',
                        '<span class="circle"></span>',
                        '<span class="circle"></span>',
                    '</div>',
                '</div>',
                 '<span style="display:inline-block;padding-left:20px;line-height:40px;height:40px;vertical-align:middle;font-weight:bold;color:white">',
                        settings.tip,
                    "</span>",
            '</div>',
        ].join('');
        var thitoast = jQuery(str).modal({
            attentionAnimation: '',
            backdrop: "static",
            width: 500,
            height: 60,
        });

        return thitoast;
    }


    jQuery.LangHua = LangHua;





})(jQuery);

$.fn.extend({
    animateCss: function (animationName) {
        var animationEnd = 'webkitAnimationEnd mozAnimationEnd MSAnimationEnd oanimationend animationend';
        $(this).addClass('animate200 ' + animationName).one(animationEnd, function () {
            $(this).removeClass('animate200  ' + animationName);
        });
    }
});

$.fn.extend({
    animateCssTime: function (animationName,timieName) {
        var animationEnd = 'webkitAnimationEnd mozAnimationEnd MSAnimationEnd oanimationend animationend';
        $(this).addClass(timieName +' '+ animationName).one(animationEnd, function () {
            $(this).removeClass(timieName+" "+ animationName);
        });
    }
});
