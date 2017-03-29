jQuery(document).ready(function($) {
    "use strict";
    var POSTDATA = postData;

    $('a#nowsave').one('click', function b() {
        var _this = $(this);
        var posting = {};

        posting.TBOrderID = $("#TBOrderID").text();

        posting.IsCommit = false;
        posting.customer = {
            "CustomerID": $('#customerid').text(),
            "CustomerTBCode": $('#CustomerTBCode').text().trim(),
            "CustomerName": $('#CustomerName').val(),
            "CustomerEnname": $('#CustomerEnname').val(),
            "Tel": $('#Tel').val(),
            "BakTel": $('#BakTel').val(),
            "Email": $('#Email').val(),
            "Wechat": $('#Wechat').val()

        };
        posting.orders = [];
        for (var i in POSTDATA) {
            var obj = {
                "OrderID": i,
                "ElementsValue": JSON.stringify(POSTDATA[i]),
                "travellers": (function() {
                    var obj = new Object();
                    var arr = new Array();
                    for (var z in personData[i]) {
                        for (var a in personData[i][z]) {
                            for (var aa in personData[i][z][a]) {
                                obj[aa] = personData[i][z][a][aa];
                            }
                        }
                    }
                    for (var c in obj) {
                        arr.push({
                            TravellerID: i
                        });
                    }
                    return arr;
                })()
            };
            obj = $.extend(true, {}, obj, systemFieldsMaps[i]);
            posting.orders.push(obj);
        }
        var toast = '';
        $.ajax({
            url: "/TBOrders/SaveTBOrder",
            type: 'post',
            timeOut: 15000,
            contentType: "application/json; charset=utf-8;",
            data: JSON.stringify(posting),
            dataType: 'json',
            beforeSend: function() {
                toast = $.LangHua.loadingToast({
                    tip: '正在暂存.....'
                });
            },
            success: function(data) {
                toast.modal('hide');
                if (data.ErrorCode == 200) {
                    $.LangHua.alert({
                        title: "提示信息",
                        tip1: '暂存成功',
                        tip2: '',
                        button: '确定'
                    });
                    _this.one('click', b);

                } else if (data.ErrorCode == 401) {
                    jQuery.LangHua.alert({
                        title: "提示信息",
                        tip1: '暂存失败',
                        tip2: data.ErrorMessage,
                        button: '确定',
                    });
                    _this.one('click', b);
                } else {
                    $.LangHua.alert({
                        title: "提示信息",
                        tip1: '暂存失败',
                        tip2: '',
                        button: '确定'
                    });
                    _this.one('click', b);

                }
            },
            complete: function(XHR, TS) {
                toast.modal('hide');
                if (TS !== "success") {
                    _this.one('click', b);
                }
            }
        });
    });


    $('a#confirmsave').one('click', function a() {
        var _this = $(this);
        var thisbutton = $(this);

        var cancel = false;








        ///////////////////


        var warings = [];
        var basewarningOBJ = {};
        basewarningOBJ.list = [];
        basewarningOBJ.cnname = '联系人基本资料';

        var baseInfovalueState = BaseInfoRef.state.valueState;
        for (var x in baseInfovalueState) {
            var oneWaring = {};
            if (baseInfovalueState[x].isValueMandatory == 0) {
                if (((baseInfovalueState[x]['isValueNotEmpty'] == 1) && (baseInfovalueState[x]['isValueUseAble'] == 0))) {
                    oneWaring.title = $('#' + x).closest('.form-group').find('label span:eq(0)').text();
                    oneWaring.warning = '填写值有误';
                    cancel = true;
                    basewarningOBJ.list.push(oneWaring);
                }
            } else {
                if (!((baseInfovalueState[x]['isValueNotEmpty'] == 1) && (baseInfovalueState[x]['isValueUseAble'] == 1))) {
                    oneWaring.title = $('#' + x).closest('.form-group').find('label span:eq(1)').text();
                    oneWaring.warning = '填写值有误';
                    cancel = true;
                    basewarningOBJ.list.push(oneWaring);
                }
            }
        }
        warings.push(basewarningOBJ);
        var ot = valueSate;
        for (var i in valueSate) {
            var breaking = false;
            var oneorderwarningOBJ = {};
            oneorderwarningOBJ.list = [];
            for (var j in valueSate[i]) {
                var oneWaring = {};
                if (valueSate[i][j]['isValueMandatory'] == 2) { //不是必须
                    if (((valueSate[i][j]['isValueNotEmpty'] == 1) && (valueSate[i][j]['isValueUseAble'] == 0))) {
                        if (valueSate[i][j]['tipsvisible'] == false) { //返回日期
                            continue;
                        }
                        cancel = true;
                        breaking = true;
                        oneWaring.title = $('#suborder' + i).find('.form-group[data-segmentcode=' + j + ']  label span:eq(0)').text();
                        oneWaring.warning = '填写值有误';
                        oneWaring.surorderid = i;
                        oneWaring.segmentcode = j;
                        oneorderwarningOBJ.list.push(oneWaring);
                        oneorderwarningOBJ.cnname = $.trim($('#suborder' + i).find('.servicecnname:eq(0) span:eq(0)').text());

                        // break;//如果不需要全部提示
                    }
                } else {
                    if (!((valueSate[i][j]['isValueNotEmpty'] == 1) && (valueSate[i][j]['isValueUseAble'] == 1))) {
                        if (valueSate[i][j]['tipsvisible'] == false) {
                            continue;
                        }
                        cancel = true;
                        breaking = true;
                        oneWaring.title = $('#suborder' + i).find('.form-group[data-segmentcode=' + j + ']  label span:eq(1)').text();
                        if (valueSate[i][j]['isValueNotEmpty'] == 0) {
                            oneWaring.warning = "未填写";
                        } else {
                            oneWaring.warning = '填写值有误';
                        }
                        oneWaring.surorderid = i;
                        oneWaring.segmentcode = j;
                        oneorderwarningOBJ.list.push(oneWaring);
                        oneorderwarningOBJ.cnname = $.trim($('#suborder' + i).find('.servicecnname:eq(0) span:eq(0)').text());
                        // break;//如果不需要全部提示
                    }
                }
            }
            if (breaking) {
                // break;
            }
            warings.push(oneorderwarningOBJ);

        } //bigfor

        if (cancel) {

            var strings = '';
            var count = 6;
            for (var i in warings) {
                if (warings[i].list.length === 0) {
                    continue;
                }
                strings += warings[i].cnname + '<br/><span style="font-size:13px;">';
                for (var j in warings[i].list) {
                    if (count === 0) {
                        break;
                    }
                    count--;
                    strings += warings[i].list[j].title;
                    strings += warings[i].list[j].warning + '<br/>';
                }
                strings += '</span>';
            }
            jQuery.LangHua.alert({
                title: "提示信息",
                tip1: '请检查以下内容：',
                tip2: strings,
                button: '确定',
                icon: "warning",
                maxHeight: 300,
                indent: false,
                callback: false

            });
            thisbutton.one("click", a);
            var top = $('.tips:eq(0)').offset().top - 50;
            $('body').scrollTop(top, 500);
            return;
        }

        if ($('#agreed').prop("checked") === false) {
            $.LangHua.alert({
                "tip1": '温馨提示',
                "tip2": "请仔细阅读《旅游风险须知和安全提示告知书》，阅读后勾选表示同意然后才能提交订单"
            });
            thisbutton.one("click", a);
            return;
        }

        /////
        var detectingPost = {};
        detectingPost.OrderID = [];
        detectingPost.TravelDate = [];
        for (var i in POSTDATA) {
            detectingPost.OrderID.push(i);
            detectingPost.TravelDate.push(systemFieldsMaps[i].TravelDate);
        }
        $.ajax({
            url: "/TBOrders/CheckTravelDate",
            type: 'post',
            contentType: "application/json; charset=utf-8;",
            data: JSON.stringify(detectingPost),
            dataType: 'json',
            success: function(data) {
                if (data.ErrorCode == 200) {
                    finalPost(thisbutton, a);
                } else if (data.ErrorCode == 401) {
                    jQuery.LangHua.confirm({
                        title: "提示信息",
                        tip1: '提示信息：',
                        tip2: data.ErrorMessage,
                        confirmbutton: '确定',
                        cancelbutton: '取消',
                        data: null,
                        cancel: function() {
                            $('a#confirmsave').one('click', a);
                        },
                        confirm: function() {
                            finalPost(thisbutton, a);
                        }
                    })
                } else {
                    jQuery.LangHua.alert({
                        title: "提示信息",
                        tip1: '检测失败',
                        tip2: '检测失败!',
                        button: '确定',
                        callback: function() {
                            cancel = true;
                        }
                    })
                }
            },
            complete: function(XHR, TS) {
                if (TS !== "success") {
                    $('a#confirmsave').one('click', a);
                }
            }
        });

    });

    function finalPost(thisbutton, thisCallback) {
        var posting = {};
        posting.TBOrderID = $("#TBOrderID").text();

        posting.IsCommit = true;
        posting.customer = {
            "CustomerID": $('#customerid').text(),
            "CustomerTBCode": $('#CustomerTBCode').text().trim(),
            "CustomerName": $('#CustomerName').val(),
            "CustomerEnname": $('#CustomerEnname').val(),
            "Tel": $('#Tel').val(),
            "BakTel": $('#BakTel').val(),
            "Email": $('#Email').val(),
            "Wechat": $('#Wechat').val(),

        };
        posting.orders = [];
        for (var i in POSTDATA) {
            var obj = {
                "OrderID": i,
                "ElementsValue": JSON.stringify(POSTDATA[i]),
                "travellers": (function() {
                    var obj = {};
                    var arr = [];
                    for (var z in personData[i]) {
                        for (var a in personData[i][z]) {
                            for (var aa in personData[i][z][a]) {
                                obj[aa] = personData[i][z][a][aa];
                            }
                        }
                    }
                    for (var c in obj) {
                        arr.push({
                            TravellerID: c
                        });
                    }
                    return arr;
                })(),
                'ServiceItemTemplteValue': JSON.stringify(tempValue[i]),
            };
            obj = $.extend(true, {}, obj, systemFieldsMaps[i]);
            posting.orders.push(obj);
        }
        var toast = '';
        $.ajax({
            url: "/TBOrders/SaveTBOrder",
            type: 'post',
            timeOut: 15000,
            contentType: "application/json; charset=utf-8;",
            data: JSON.stringify(posting),
            dataType: 'json',
            beforeSend: function() {
                toast = $.LangHua.loadingToast({
                    tip: '正在保存.....'
                });
            },
            success: function(data) {
                toast.modal('hide');
                if (data.ErrorCode == 200) {
                    window.location.href = "/Orders/Finished";
                } else if (data.ErrorCode == 401) {
                    jQuery.LangHua.alert({
                        title: "提示信息",
                        tip1: '保存失败',
                        tip2: data.ErrorMessage + " 请您先点击暂存按钮暂存",
                        button: '确定',
                    });
                    $('a#confirmsave').one('click', thisCallback)
                } else {
                    $.LangHua.alert({
                        title: "提示信息",
                        tip1: '保存失败',
                        tip2: '请您先点击暂存按钮暂存',
                        button: '确定'
                    });
                    $('a#confirmsave').one('click', thisCallback)
                }
            },
            complete: function(XHR, TS) {
                toast.modal('hide');
                if (TS !== "success") {
                    $('a#confirmsave').one('click', thisCallback);
                    $.LangHua.alert({
                        title: "提示信息",
                        tip1: '保存失败',
                        tip2: '请您先点击暂存按钮暂存',
                        button: '确定'
                    });
                }
            }
        });
    }


    //风险责任处理
    $('#agreed').bind("change", function() {
        var i;
        var TBOrderID = $("#TBOrderID").text();
        if ($(this).prop('checked') === true) {
            $.LangHuaCookie.set('TBORDER' + TBOrderID, 1, 24 * 365 * 3);
            for (i in POSTDATA) {
                $.LangHuaCookie.set('SUBORDER' + i, 1, 24 * 365 * 3, '/');
            }
        } else {
            // $.LangHuaCookie.delete('TBORDER' + TBOrderID, 1);
        }
    });
    var TBOrderID = $("#TBOrderID").text();
    var agreedInCookie = $.LangHuaCookie.get('TBORDER' + TBOrderID);
    if (agreedInCookie) {
        $('#agreed').prop('checked', true);
    }
});