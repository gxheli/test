;
(function($) {
    if (LangHua) {
        alert("LangHua is defined");
    } else {

    }
    var LangHua = new Object();
    LangHua.alert = function(options) {
        var defaults = {
            title: "提示信息",
            tip1: '提示信息：',
            tip2: '提示信息：',
            button: '确定',
            icon: "warning",
            data: "",
            indent: true,
            callback: false
        };
        var settings = $.extend({}, defaults, options);
        var className = "";
        switch (settings.icon) {
            case 'warning':
                className = "fa fa-warning";
                break;
            case 'comment':
                className = "fa fa-commenting";
                break;


            default:
                className = "fa fa fa-warning";
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
            '<div class="modal modal-animate" id="alertLH' + date + '"+ tabindex="-1" data-background="red" data-backdrop="static" role="dialog" aria-labelledby="exampleModalLabel">',
            '<div class="modal-dialog " role="document">',
            '<div class="modal-content">',
            '<div class="modal-header">',
            '<button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">' + x + '</span></button>',
            '<span class="modal-title" id="exampleModalLabel">' + settings.title + '</span>',
            '</div>',
            '<div class="modal-body">',
            '<div class="modalTips">',
            '<div class="iconx">',
            '<i class="' + className + '" ></i>',
            '</div>',
            '<div class="tips">',
            '<div class="top">' + settings.tip1 + '</div>',
            '<div class="' + indentBottom + '">' + settings.tip2 + '</div>',
            '</div>',
            '</div>',
            '</div>',
            '<div class="modal-footer">',
            '<button id="button' + date + '" type="button" class="btn btn-primary button70" data-toggle="modal" data-dismiss="modal" data-whatever="">' + settings.button + '</button>',
            '</div>',
            '</div>',
            '</div>',
            '</div>',
        ].join('');
        jQuery('body').on('shown.bs.modal', "#alertLH" + date, function() {
            var _this = this;
            jQuery("#button" + date).one('click', function() {
                if (settings.callback) {
                    settings.callback(settings.data);
                }
                var animationEnd = 'webkitAnimationEnd mozAnimationEnd MSAnimationEnd oanimationend animationend';
                $(_this).addClass('animate200 fadeOut ').one(animationEnd, function() {
                    $(_this).removeClass('animate200 fadeOut ');
                    jQuery('#miss' + date).trigger('click');
                });
            });


        });
        jQuery(str).modal();
    };

    LangHua.showResult = function(options) {
        var defaults = {
            title: "提示信息",
            tip1: '提示信息：',
            tip2: '提示信息：',
            content: "",
            confirmbutton: '确定',
            data: null,
            confirm: function() {
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
            '<button  type="button" class="btn btn-primary button70" data-dismiss="modal"  data-toggle="modal" id="' + date + '"  >' + settings.confirmbutton + '</button>',
            '</div>',
            '</div>',
            '</div>',
            '</div>',
        ].join('');
        jQuery('body').on('shown.bs.modal', "#modal" + date, function() {
            var _this = this;
            jQuery('#' + date).one("click", function() {
                settings.confirm.call(this, this, settings.data);
                var animationEnd = 'webkitAnimationEnd mozAnimationEnd MSAnimationEnd oanimationend animationend';
                $(_this).addClass('animate200 fadeOut ').one(animationEnd, function() {
                    $(_this).removeClass('animate200 fadeOut ');
                });
            });
        });
        jQuery(str).modal({});

    };
    //confirm
    LangHua.confirm = function(options) {
            var defaults = {
                title: "提示信息",
                tip1: '提示信息：',
                tip2: '提示信息：',
                confirmbutton: '确定',
                cancelbutton: '取消',
                data: null,
                cancel: function() {
                    return;
                },
                confirm: function() {
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
                '<button type="button" class="close" data-dismiss="modal" aria-label="Close" id="close' + date + '"><span aria-hidden="true">&times;</span></button>',
                '<button type="button" class="close hidden" data-dismiss="modal" aria-label="Close" id="closex' + date + '"><span aria-hidden="true">&times;</span></button>',
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
            jQuery('body').on('shown.bs.modal', "#modal" + date, function() {
                var _this = this;
                jQuery('#' + date).one("click", function() {
                    jQuery(this).prop("disabled", true).text("正在处理..");
                    settings.confirm.call(this, this, settings.data);
                    var animationEnd = 'webkitAnimationEnd mozAnimationEnd MSAnimationEnd oanimationend animationend';
                    $(_this).addClass('animate200 fadeOut ').one(animationEnd, function() {
                        $(_this).removeClass('animate200 fadeOut ');
                        jQuery('#closex' + date).trigger('click');
                    });
                });
                jQuery('#miss' + date).one("click", function() {
                    settings.cancel.call(this, this, settings.data);
                });
                jQuery('#close' + date).one("click", function() {
                    settings.cancel.call(this, this, settings.data);
                })
            })

            jQuery(str).modal({})
        }
        //toast
    LangHua.toast = function(options) {
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
    LangHua.blockToast = function(options) {
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
    };

    //loading
    LangHua.loadingToast = function(options) {
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
    };
    LangHua.loadingToastx = function(options) {
        var defaults = {
            tip: "提示信息",
        };

        var settings = $.extend({}, defaults, options);
        var str = [
            '<div class="modal "  tabindex="-1"  role="dialog" aria-labelledby="exampleModalLabel" >',
            '<div class="modal-dialog ">',
            '<div class="loading-simple" >',
            '<div class="containerx" >',
            '<div class="icon-container">',
            '<svg version="1.1" id="图层_1" xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" x="0px" y="0px" width="50px" height="50px" viewBox="0 0 50 50" enable-background="new 0 0 50 50" xml:space="preserve">',
            '<g>',
            '<circle fill="#FFFFFF" cx="25.369" cy="29.052" r="9.271"/>',
            '<path fill="#FFFFFF" d="M46,15.65V0.722l-4.869,3.297l-9.337,5.698v0.001c-2.026-0.667-4.197-1.028-6.449-1.028',
            'c-11.405,0-20.652,9.248-20.652,20.653C4.693,40.751,13.939,50,25.345,50C36.753,50,46,40.751,46,29.344h-4.864',
            'c0,8.721-7.068,15.791-15.791,15.791c-8.718,0-15.787-7.07-15.787-15.791c0-8.719,7.068-15.788,15.787-15.788',
            'c2.499,0,4.863,0.582,6.963,1.616l0,0l4.718-2.858c-0.001-0.002-0.001-0.004-0.007-0.006l4.112-2.497v5.841H46z"/>',
            '</g>',
            '</svg>',
            // '<img src="data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAB4AAAAyCAYAAABLXmvvAAAEkElEQVRYR+2Xe6iURRjGn998GoZdLKIsQiqysizEytJKpCgpI5CUEqKsMIpCCA3CioQyS1KKIsL+EJMuFERQqXRBulCYaQqm2QUloQtB2UUr65snZp1dvrNnz9k94QXiDCzs7rzzPu8873XQAVocIFz1A+835vup/v9SbftmSWv2m49tD7O9EJgiadx+AbZ9g+2HgOOyL8fsU2DbZ2XASU3Bs2+AbR8aY5wNzAIGt4jYvgHbHiDpBEknSjpSqpXc3yR9I+kr4A/bV9meB4zsJUU6A7Z9ToxxGnCZpJOBQVWltktJ2yR9D1zQtBclrZJ0EXBQRz62PSLGOBe4Gij6muS21wGzJW23vRlIjKXV841tz7C9ABjSCtD2LknpNgc3G2V7B/CIpMcz/SnIPgVCr8BlWc4LIcxpoiwBrQBel7RB0s+SEsWHSxoeY5wIXC7pE2AOsKV+3vZoSWsr+sZ2Syfb90uaWwWNMS4LITwMbOqNbtuHAb82y9g+StI1ORjT9ktdgG2nqvJyxdKdwC3A8331bzv5BnCyyvaGenWx/RcwGVjRTsl/2W8At/DrTOCJFrSNjTFOAU6TNMD2thDCG5JeA9ypETVg20Nsfw4ck39/AIyvKrI9KMa4CLgVaBUb7wAzgK2dgNeBpyaHVw5MApZXfF3YfjF3lh712v4CmAB81w68BlyW5eIQwox8269TuQP+rACnvcXtlKX9GONzRVFc1062Bhxj/Bg4N39fVhTF9RXQgbbXAme2U5YN350N/7I3+TrwViAV/7TuAx6sAJ9ieyMwsBPgLDMdWNoJ8A/A0VnwNuDpCvCFkt7vA2gSTS1xYUXHIZLOqxSQPaNPjPFb4NgseCfwWOXQSNvr+9gkUtF5pqLj7FRKK8bvaRIxxk3AiPx9UVEUsyqHBtn+DDipk1vbNjAaWF/RcYWklOspdffEQAZbngt82lgVQri4CmL7HkkNv/dmgO23gNQwGsWkLMsHQgj3ZuDt6ZL1PL5b0vy8sQs4A0iNvbZsD7b9NnB+G9CfgNTwG83EdrC9JrGQdb0aQphcBx5lO7WzWrOPMc4viqK5LQ61vTRPId3wbW8BbgI+bGJroqSVlf+SzJJG6Ysxvglcmq36HRgDbG5SkuRTB5tq+1RJKcVSKqZGsgz4pUk+xcdHwKisNwVxYnNHtTtNyLNRnd7VwCXAzlb0JgolFcDfPdFfluWTIYTbK/t3AY+m312KfVmWS0II0yu+TUGXhrxuzb1dhFcDKt82peS4NAp1A7Z9hO33qqNp9v0dwOp2YBng+BjjghDCtMoF0kCRmkcjl1u1t1QiVwJpdq7Tvtv2syGEJbUHVwt6s8+vtZ3a5tDq2czaK1XDWz5hbA+X9IKkVHG6rDSmStpsOw3xadgbmovP6S3m7R+BG4Fa8WgLnClLz5D0Iki1uz4Pd8J2naU0kaby25g2OwKuUDUmxjgzhHBlHmV7BLf9j6R3gaeALtR2fOMWFA+TND7GmCLzFEnpk5jYmiaPEMK6lI7Axk5o2afP1N4M6AfuxD17Raaf6r1CYydKDhjV/wJH8VbAMq0J9wAAAABJRU5ErkJggg=="/>',
            '</div>',
            '<div class="text-container">',
            settings.tip,
            '<span class="waiting waiting0"></span>',
            '<span class="waiting waiting1"></span>',
            '<span class="waiting waiting2"></span>',
            '<span class="waiting waiting3"></span>',
            '<span class="waiting waiting4"></span>',
            '<span class="waiting waiting5"></span>',
            '</div>',
            '</div>',
            '</div>',
            '</div>',
            '</div>'
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

    if (LangHuaCookie) {
        alert("LangHua is defined");
    }
    var LangHuaCookie = {
        'set': function(name, value, expireObj, path, domain, secure) {
            var cookieStr = encodeURIComponent(name) + '=' + encodeURIComponent(value);
            if (expireObj instanceof Date) {
                cookieStr += ";expires=" + expireObj.toGMTString();
            }
            if (!isNaN(expireObj)) {
                if (expireObj > 0) {
                    var date = new Date();
                    date.setTime(date.valueOf() + parseInt(expireObj) * 3600 * 1000);
                    cookieStr += "; expires=" + date.toGMTString();
                }
            }
            if (path) {
                cookieStr += ";path=" + path;
            }
            if (domain) {
                cookieStr += ";domain=" + domain;
            }
            if (secure) {
                cookieStr += ";secure";
            }
            document.cookie = cookieStr;

        },
        'get': function(name) {
            var cookieName = encodeURIComponent(name);
            var cookieStart = document.cookie.indexOf(cookieName);
            var cookieValue = null;
            if (cookieStart > -1) {
                var cookieEnd = document.cookie.indexOf(';', cookieStart);
                if (cookieEnd == -1) {
                    cookieEnd = document.cookie.length;
                }
                cookieValue = decodeURIComponent(document.cookie.substring(cookieStart, cookieEnd));
                cookieValue = cookieValue.split("=")[1] ? cookieValue.split("=")[1] : null;
            }
            return cookieValue;
        },
        'delete': function(name, path, domain, secure) {
            this.set(name, '', new Date(0), path, domain, secure);
        }
    };
    jQuery.LangHuaCookie = LangHuaCookie;
    if (LangHuaModal) {
        alert("LangHua is defined");
    }
    var LangHuaModal = {
        "closeModals": function (arrExclude) {
            var openModals = $("body").data("modalmanager").getOpenModals();
            var idString = arrExclude.join("#");
            if (openModals) {
                for (var i in openModals) {
                    if (idString.indexOf($(openModals[i]['$element'][0]).attr("id") )=== -1) {
                        $(openModals[i]['$element'][0]).modal("hide");
                    }
                }
            }
        }
    }
    jQuery.LangHuaModal = LangHuaModal;
})(jQuery);

$.fn.extend({
    animateCss: function(animationName) {
        var animationEnd = 'webkitAnimationEnd mozAnimationEnd MSAnimationEnd oanimationend animationend';
        $(this).addClass('animate200 ' + animationName).one(animationEnd, function() {
            $(this).removeClass('animate200  ' + animationName);
        });
    },
    animateCssTime: function(animationName, timieName) {
        var animationEnd = 'webkitAnimationEnd mozAnimationEnd MSAnimationEnd oanimationend animationend';
        $(this).addClass(timieName + ' ' + animationName).one(animationEnd, function() {
            $(this).removeClass(timieName + " " + animationName);
        });
    }
});