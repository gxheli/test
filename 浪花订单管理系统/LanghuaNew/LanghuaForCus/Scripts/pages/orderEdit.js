jQuery(document).ready(function($) {
    'use strict';
    $('a.save-button').each(function() {
        $(this).one('click', function a() {
            var thisbutton = $(this);
            var posting = {};
            var cancel = false;
            var commit = $(this).data('iscommit') == true ? true : false;
            if (commit) {
                var warings = [];
                var basewarningOBJ = [];
                basewarningOBJ.list = [];
                basewarningOBJ.cnname = '联系人基本资料';
                var baseInfovalueState = BaseInfoRef.state.valueState;
                for (var x in baseInfovalueState) {
                    var oneWaring = {};
                    if (baseInfovalueState[x].isValueMandatory == 0) {
                        if (((baseInfovalueState[x].isValueNotEmpty == 1) && (baseInfovalueState[x].isValueUseAble == 0))) {

                            oneWaring.title = $('#' + x).closest('.form-group').find('label span:eq(0)').text();
                            oneWaring.warning = '填写值有误';
                            cancel = true;
                            basewarningOBJ.list.push(oneWaring);
                        }
                    } else {
                        if (!((baseInfovalueState[x].isValueNotEmpty == 1) && (baseInfovalueState[x].isValueUseAble == 1))) {
                            oneWaring.title = $('#' + x).closest('.form-group').find('label span:eq(1)').text();
                            oneWaring.warning = '填写值有误';
                            cancel = true;
                            basewarningOBJ.list.push(oneWaring);
                        }
                    }
                    oneWaring = null;
                }
                warings.push(basewarningOBJ);
                var ot = valueSate;
                for (var i in valueSate) {
                    var breaking = false;
                    var oneorderwarningOBJ = {};
                    oneorderwarningOBJ.list = [];
                    for (var j in valueSate[i]) {
                        var oneWaring = {};
                        if (valueSate[i][j].isValueMandatory == 2) { //不是必须
                            if (((valueSate[i][j].isValueNotEmpty == 1) && (valueSate[i][j].isValueUseAble == 0))) {
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
                        oneWaring = null;
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
                        "tip2": "请仔细阅读《旅游风险须知和安全提示告知书》，阅读后勾选表示同意然后才能提交订单",
                        "indent": false
                    });
                    thisbutton.one("click", a);
                    return;
                }
                //检测
                var detectingPost = {
                    "OrderID": $('#orderid').text(),
                    "TravelDate": systemFieldsMaps[$('#orderid').text()].TravelDate
                };
                $.ajax({
                    url: "/Orders/CheckTravelDate",
                    type: 'post',
                    contentType: "application/json; charset=utf-8;",
                    data: JSON.stringify(detectingPost),
                    dataType: 'json',
                    success: function(data) {
                        if (data.ErrorCode == 200) {
                            finalPost(thisbutton, a, commit);
                        } else if (data.ErrorCode == 401) {
                            jQuery.LangHua.confirm({
                                title: "提示信息",
                                tip1: '提示信息：',
                                tip2: data.ErrorMessage,
                                confirmbutton: '确定',
                                cancelbutton: '取消',
                                data: null,
                                cancel: function() {
                                    thisbutton.one('click', a);
                                },
                                confirm: function() {
                                    finalPost(thisbutton, a, commit);
                                }
                            })
                        } else {
                            jQuery.LangHua.alert({
                                title: "提示信息",
                                tip1: '检测失败',
                                tip2: '检测失败!',
                                button: '确定',
                                callback: function() {
                                    thisbutton.one('click', a);
                                    cancel = true;
                                }
                            })
                        }
                    },
                    complete: function(XHR, TS) {
                        if (TS !== "success") {
                            thisbutton.one('click', a);
                        }
                    }
                });

            } //if commit
            else {
                finalPost(thisbutton, a, commit);
            }
        });
    });


    function finalPost(thisbutton, thisCallback, commit) {
        var posting = {};
        posting.OrderID = $('#orderid').text();
        posting.Customers = {
            "CustomerID": $('#customerid').text(),
            "CustomerTBCode": $('#CustomerTBCode').text().trim(),
            "CustomerName": $('#CustomerName').val(),
            "CustomerEnname": $('#CustomerEnname').val(),
            "Tel": $('#Tel').val(),
            "BakTel": $('#BakTel').val(),
            "Email": $('#Email').val(),
            "Wechat": $('#Wechat').val()
        };
        posting.ServiceItemHistorys = {};

        function getPerson(personInSubOrder) {
            var obj = {};
            var arr = [];
            for (var z in personInSubOrder) {
                for (var a in personInSubOrder[z]) {
                    for (var aa in personInSubOrder[z][a]) {
                        obj[aa] = personInSubOrder[z][a][aa];
                    }
                }
            }
            for (var c in obj) {
                arr.push({
                    TravellerID: c
                });
            }
            return arr;
        }
        for (var i in postData) {
            posting.ServiceItemHistorys = {
                "OrderID": i,
                "ElementsValue": JSON.stringify(postData[i]),
                "travellers": getPerson(personData[i]),
                'ServiceItemTemplteValue': JSON.stringify(tempValue[i]),
            };
            break;
        }

        //posting.ServiceItemHistorys= $.extend(true,{},posting.ServiceItemHistorys,extrasobj,personNum);
        posting.ServiceItemHistorys = $.extend(true, {}, posting.ServiceItemHistorys, systemFieldsMaps[$('#orderid').text()]);
        var postObject = {};
        postObject.order = posting;
        postObject.isCommit = commit;
        var toast = '';
        $.ajax({
            url: "/Orders/SaveOrder",
            type: 'post',
            contentType: "application/json; charset=utf-8;",
            data: JSON.stringify(postObject),
            dataType: 'json',
            beforeSend: function() {
                var tips = commit === true ? "正 在  保 存" : "正 在 暂 存";
                toast = $.LangHua.loadingToast({
                    tip: tips + '. . . . .'
                });
            },
            success: function(data) {
                toast.modal('hide');
                if (data.ErrorCode == 200) {
                    if (commit === true) {
                        window.location.href = "/Orders/Finished";
                    } else {
                        jQuery.LangHua.alert({
                            title: "提示信息",
                            tip1: '暂存结果',
                            tip2: '暂存成功',
                            button: '确定'
                        });
                        thisbutton.one("click", thisCallback);
                    }
                } else if (data.ErrorCode == 401) {
                    var tips = commit === true ? "保 存 失 败,请您先点击暂存按钮暂存" : "暂 存 失 败";
                    jQuery.LangHua.alert({
                        title: "提示信息",
                        tip1: tips,
                        tip2: data.ErrorMessage,
                        button: '确定'
                    });
                    thisbutton.one("click", thisCallback);
                } else {
                    tips = commit === true ? "保存结果" : "暂存结果";
                    var tips2 = commit === true ? "保存失败，请您先点击暂存按钮暂存" : "暂存失败";
                    jQuery.LangHua.alert({
                        title: "提示信息",
                        tip1: tips,
                        tip2: tips2,
                        button: '确定'
                    });
                    thisbutton.one("click", thisCallback);
                }
            },
            complete: function(XHR, TS) {
                toast.modal('hide');
                toast = null;
                if (TS !== "success") {
                    tips = commit === true ? "保存结果" : "暂存结果";
                    var tips2 = commit === true ? "保存失败，请您先点击暂存按钮暂存" : "暂存失败";
                    jQuery.LangHua.alert({
                        title: "提示信息",
                        tip1: tips,
                        tip2: tips2,
                        button: '确定'
                    });
                    thisbutton.one("click", thisCallback);
                }
            }
        });
    }

    //风险责任处理
    $('#agreed').bind("change", function() {
        var suborder = $('#orderid').text();
        if ($(this).prop('checked') === true) {
            $.LangHuaCookie.set('SUBORDER' + suborder, 1, 24 * 365 * 3);
        }
    });
    var suborder = $('#orderid').text();
    var agreedInCookie = $.LangHuaCookie.get('SUBORDER' + suborder);
    if (agreedInCookie) {
        $('#agreed').prop('checked', true);
    }
});