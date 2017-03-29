jQuery(document).ready(function($) {





    // $('#CustomerEnname').onlyCapchar();
    // $('#CustomerName').onlyChinese();
    // $('#Tel').onlyNumWithEmpty  ();
    //更改人数
    $('body').on('click', '#revisePersonNum', function() {
        var thisbutton = $(this);
        var personNum = thisbutton.data('personnum');
        if (!(personNum instanceof Object)) {
            personNum = JSON.parse(personNum);
        }
        var tmp = [
            '<div id="revisepeople12" class="modal modal-animate" data-backdrop="static" tabindex="-1" data-focus-on="input:first">',
            '<div class="modal-dialog " role="document">',
            '<div class="modal-content">',
            '<div class="modal-header">',
            '<button type="button" class="close" data-dismiss="modal" aria-hidden="true">×</button>',
            '<h4 class="modal-title">改人数</h4>',
            '</div>',
            '<div class="modal-body">',
            '<div class="form-body">',
            '正在加载...',
            '</div>',
            '</form>',
            '</div>',
            '<div class="modal-footer">',
            '<a id="savepersonNum" class="btn-sm btn btn-primary button70">确定</a>',
            '<span></span>',
            '<a data-dismiss="modal" class="btn-sm btn btn-default button70">取消</a>',
            '</div>',
            '</div>',
            '</div>',
            '</div>',

        ].join('\n');
        $('body').one('shown.bs.modal', '#revisepeople12', function() {
            var whichModal = $(this);

            $(this).find(".form-body:eq(0)").empty().append(makelist(personNum));
            $(this).find(".form-body:eq(0)").append(
                '<div  id="tmallOrders" style="padding:5px">' +
                '<div style="margin:5px 0px"><span style="color:red;"> 新增</span>收入淘宝订单号：</div>' +
                '<div id="orderNolist" class="orderNolist" style="min-height:38px;width:268px;margin-right:0px;display:inline-block;position:relative">' +
                '<div id="tips"  style="position:absolute;top:0px;bottom:0px;left:0px;right:0px;color:#999999;line-height:38px;text-align:center">' +
                '订单收入金额增加时请选择新的淘宝订单' +
                '</div>' +
                '</div>' +
                '<a id="addTmallData" data-toggle="modal" href="#orderPre" style="position:absolute;margin-left: 5px;margin-top:10px">选择淘宝订单</a>' +
                '</div>'
            );
            $(this).find("input.numbercute").onlyNum();

            $(this).find("#savepersonNum").one('click', function a() {
                var _this = $(this);
                // 查找纠错
                var cancel = false;
                var obj = {};
                var add = 0;
                whichModal.find('#person3 .person3').each(function() {
                    obj[$(this).data("for")] = $(this).val();
                    add += parseInt($(this).val());
                })
                if (add == 0) {
                    cancel = true;


                    whichModal.find('#person3').warning("总人数不能为 0 ");

                }
                var multi = 1;
                whichModal.find('#roomnight .roomnight').each(function() {
                    obj[$(this).data("for")] = $(this).val();
                    multi *= parseInt($(this).val());
                })
                if (multi == 0) {
                    cancel = true;

                    whichModal.find('#roomnight').warning('间数和晚数相乘不能为 0 ');

                }

                if (cancel) {
                    _this.one('click', a);
                    return;
                }

                var objdeault = {
                    AdultNum: 0,
                    ChildNum: 0,
                    INFNum: 0,
                    RoomNum: 0,
                    RightNum: 0,
                }

                var UpdateItemNum = new Object();
                UpdateItemNum = $.extend(true, objdeault, obj);
                UpdateItemNum.OrderID = $("#OrderID").text();


                $.ajax({
                    url: "/Orders/UpdateItemNum",
                    type: 'post',
                    contentType: "application/json; charset=utf-8;",
                    data: JSON.stringify(UpdateItemNum),
                    dataType: 'json',
                    beforeSend: function() {},
                    success: function(data) {
                        if (data.ErrorCode == 200) {
                            thisbutton.data('personnum', JSON.stringify(obj));
                            thisbutton.trigger("update", [obj]);

                            _this.success("改人数成功");
                            $(".forceUpdate-setoutreturn").trigger("update", [UpdateItemNum.RightNum]);
                            $(".forceUpdate-peoplenum").trigger("update", [parseInt(UpdateItemNum.AdultNum) + parseInt(UpdateItemNum.ChildNum) + parseInt(UpdateItemNum.INFNum)]);

                            var tmallOrders = $("#revisepeople12").data("tmallorders");
                            if (tmallOrders) {
                                var hastmallOrders = false;
                                for (var i in tmallOrders.subOrderList) {
                                    hastmallOrders = true;
                                    break;
                                }
                                if (hastmallOrders === true) {
                                    $.ajax({
                                        url: "/Orders/AddTBOrderNos",
                                        type: 'post',
                                        contentType: "application/json; charset=utf-8;",
                                        data: JSON.stringify({
                                            "OrderID": $("#OrderID").text(),
                                            "TBOrderNos": (function() {
                                                var tmallOrders = $("#revisepeople12").data("tmallorders");
                                                var TBOrderNos = [];
                                                for (var i in tmallOrders.subOrderList) {
                                                    TBOrderNos.push({
                                                        "No": tmallOrders.subOrderList[i].Tid,
                                                        "SubNo": tmallOrders.subOrderList[i].Oid,
                                                        "Payment": tmallOrders.subOrderList[i].Payment,
                                                        "RefundId": tmallOrders.subOrderList[i].RefundId
                                                    });
                                                }
                                                return TBOrderNos;
                                            })()
                                        }),
                                        dataType: 'json'
                                    });
                                }
                            }
                            setTimeout(function() {
                                whichModal.modal('hide');
                            }, 800)
                        } else if (data.ErrorCode == 400) {
                            jQuery.LangHua.alert({
                                title: "提示信息",
                                tip1: '改人数失败',
                                tip2: data.ErrorMessage,
                                button: '确定',
                            })
                        } else {
                            _this.success("改人数失败");
                            _this.one('click', a);
                        }
                    },
                    complete: function(xhr, TS) {
                        if (TS != "success") {
                            _this.one('click', a);
                            _this.success("改人数失败");

                        }
                    }
                })



            })


            function makelist(obj) {
                var person3 = [
                    '<div id="person3">',
                    '<div  class="margin-bottom-10"><span>成人：</span><input type="text" which="adult" value="' + obj.AdultNum + '" data-for="AdultNum" class="numbercute adult    person3 "></div>',
                    '<div class="margin-bottom-10"><span>儿童：</span><input type="text" which="adult" value="' + obj.ChildNum + '" data-for="ChildNum" class="numbercute child    person3 "></div>',
                    '<div class="margin-bottom-10"><span>婴儿：</span><input type="text" which="adult" value="' + obj.INFNum + '" data-for="INFNum" class="numbercute infant    person3 "></div>',
                    '</div>',




                ];
                if ('RoomNum' in obj) {
                    var roomNight = [
                        '<div id="roomnight">',
                        '<div class="margin-bottom-10"><span>间数：</span><input type="text" which="adult" value="' + obj.RoomNum + '" data-for="RoomNum" class="numbercute roomnight rooms "></div>',
                        '<div class="margin-bottom-10"><span>晚数：</span><input type="text" which="adult" value="' + obj.RightNum + '" data-for="RightNum" class="numbercute roomnight nights"></div>',
                        '</div>',
                    ]
                } else {
                    roomNight = [];
                }
                person3 = person3.concat(roomNight);
                var str = person3.join('');

                return str;
            }
        })

        $(tmp).modal();
    })


    //更改项目
    $('body').on('click', '#reviseExtras', function() {
        var thisbutton = $(this);

        var extra = thisbutton.data('extras');
        var supplierID = thisbutton.data('supplierid');
        var serviceItemID = thisbutton.data("serviceitemid");
        if (!(extra instanceof Object)) {
            extra = JSON.parse(extra);
        }

        $.ajax({
            url: "/Orders/GetExtraServiceByID",
            type: 'get',
            // contentType: "application/json; charset=utf-8;",
            data: {
                'SupplierID': supplierID,
                'ItemID': serviceItemID
            },
            dataType: 'json',
            beforeSend: function() {
                var tmp = [
                    '<div id="reviseExtrasM" class="modal modal-animate" data-backdrop="static" tabindex="-1" data-focus-on="input:first">',
                    '<div class="modal-dialog " role="document">',
                    '<div class="modal-content">',
                    '<div class="modal-header">',
                    '<button type="button" class="close" data-dismiss="modal" aria-hidden="true">×</button>',
                    '<h4 class="modal-title">更改项目</h4>',
                    '</div>',
                    '<div class="modal-body">',
                    '<div class="form-body">',
                    '正在获取额外服务项目...',
                    '</div>',
                    '</div>',
                    '<div class="modal-footer">',
                    '<a id="saveExtrasEdit" class="btn  btn-sm btn-primary button70">确定</a>',
                    '<span></span>',
                    '<a data-dismiss="modal" class="btn  btn-sm btn-default button70">取消</a>',
                    '</div>',
                    '</div>',
                    '</div>',
                    '</div>',
                    '</div>'
                ].join('\n');
                $(tmp).modal();

                $('body').one('toShowExtras', '#reviseExtrasM', function(e, data) {
                    var whichModal = $(this);
                    if (data.length === 0) {
                        $(this).find(".form-body:eq(0)").empty().append("没有附加项目")
                    } else {
                        $(this).find(".form-body:eq(0)").empty().append(makeSelects(data, extra));
                        $(this).find(".form-body:eq(0)").append(
                            '<div id="tmallOrders" style="padding:5px">' +
                            '<div style="margin:5px 0px"><span style="color:red;"> 新增</span> 收入淘宝订单号：</div>' +
                            '<div id="orderNolist" class="orderNolist" style="min-height:38px;width:268px;margin-right:0px;display:inline-block;position:relative">' +
                            '<div id="tips"  style="position:absolute;top:0px;bottom:0px;left:0px;right:0px;color:#999999;line-height:38px;text-align:center">' +
                            '订单收入金额增加时请选择新的淘宝订单' +
                            '</div>' +
                            '</div>' +
                            '<a id="addTmallData" data-toggle="modal" href="#orderPre" style="position:absolute;margin-left: 5px;margin-top:10px">选择淘宝订单</a>' +
                            '</div>'
                        );
                    }


                    $(this).find("#saveExtrasEdit").one('click', function a() {
                        var _this = $(this);
                        // 查找纠错
                        var cancel = false;
                        var arr = [];
                        var minmumCount = 0;
                        var selectCorrectCount = 0;

                        whichModal.find('select').each(function() {
                            var obj = $(this).data('fullinfo');
                            obj.ServiceNum = $(this).val();
                            arr.push(obj);
                            minmumCount += parseInt($(this).data('minnum'));
                            if ($(this).val() < $(this).data('minnum')) {
                                // cancel = true;
                                // $(this).warning('最少选择：'+obj.MinNum+obj.ServiceUnit);
                            } else {
                                selectCorrectCount++;
                            }
                        })
                        if (minmumCount == 0) {
                            cancel = false;
                        } else {
                            if (selectCorrectCount == 0) {
                                cancel = true;
                                $(this).closest('.modal-footer').siblings('.modal-body').find('#selectwrapper').warning('最少选择其中一项');
                            } else {
                                cancel = false;
                            }
                        }
                        if (cancel) {
                            _this.one('click', a);
                            return;
                        }
                        var UpdateExtraService = new Object();
                        UpdateExtraService.OrderID = $("#OrderID").text();
                        UpdateExtraService.extraServiceHistorys = arr;
                        if (arr.length === 0) {
                            _this.one('click', a);
                            return;
                        }
                        $.ajax({
                            url: "/Orders/UpdateExtraService",
                            type: 'post',
                            contentType: "application/json; charset=utf-8;",
                            data: JSON.stringify(UpdateExtraService),
                            dataType: 'json',
                            beforeSend: function() {},
                            success: function(data) {
                                if (data.ErrorCode == 200) {
                                    thisbutton.data('extras', JSON.stringify(arr));
                                    thisbutton.trigger("update", [arr]);

                                    _this.success("更改改项目成功");
                                    var tmallOrders = $("#reviseExtrasM").data("tmallorders");
                                    if (tmallOrders) {
                                        var hastmallOrders = false;
                                        for (var i in tmallOrders.subOrderList) {
                                            hastmallOrders = true;
                                            break;
                                        }
                                        if (hastmallOrders === true) {
                                            $.ajax({
                                                url: "/Orders/AddTBOrderNos",
                                                type: 'post',
                                                contentType: "application/json; charset=utf-8;",
                                                data: JSON.stringify({
                                                    "OrderID": $("#OrderID").text(),
                                                    "TBOrderNos": (function() {
                                                        var tmallOrders = $("#reviseExtrasM").data("tmallorders");
                                                        var TBOrderNos = [];
                                                        for (var i in tmallOrders.subOrderList) {
                                                            TBOrderNos.push({
                                                                "No": tmallOrders.subOrderList[i].Tid,
                                                                "SubNo": tmallOrders.subOrderList[i].Oid,
                                                                "Payment": tmallOrders.subOrderList[i].Payment,
                                                                "RefundId": tmallOrders.subOrderList[i].RefundId
                                                            });
                                                        }
                                                        return TBOrderNos;
                                                    })()
                                                }),
                                                dataType: 'json'
                                            });
                                        }
                                    }
                                    setTimeout(function() {
                                        whichModal.modal('hide');
                                    }, 800);
                                } else if (data.ErrorCode == 401) {
                                    jQuery.LangHua.alert({
                                        title: "提示信息",
                                        tip1: '保存失败',
                                        tip2: data.ErrorMessage,
                                        button: '确定',

                                    })
                                    _this.one('click', a);
                                } else {
                                    _this.success("更改项目失败");
                                    _this.one('click', a);
                                }
                            },
                            complete: function(XHR, TS) {
                                if (TS !== "success") {
                                    _this.one('click', a);
                                }
                            }
                        });
                    });

                    function makeSelects(arr, extras) {

                        var oldExtra = new Object();
                        for (var z in extras) {
                            oldExtra[extras[z].ExtraServiceID] = extras[z];
                        }
                        var str = '<span id="selectwrapper" style="display:inline-block;padding:5px">';
                        for (var i in arr) {
                            if (arr[i]['MinNum'] == 0) {
                                var minmum = 1;

                            } else {
                                var minmum = arr[i]['MinNum'];
                            }
                            var value = arr[i]['ServiceNum'];

                            if (arr[i].ExtraServiceID in oldExtra) {
                                value = oldExtra[arr[i].ExtraServiceID].ServiceNum;
                            }
                            str += "<select style='margin:5px 8px 0px 0px' data-fullinfo='" + JSON.stringify(arr[i]) + "'   data-minnum='" + arr[i]['MinNum'] + "'>";
                            str += '<option value="0">' + arr[i]['ServiceName'] + '&nbsp;0' + arr[i]['ServiceUnit'] + '</option>';
                            for (var j = minmum; j <= arr[i]['MaxNum']; j++) {
                                if (j == value) {
                                    var selected = "selected=selected";
                                } else {
                                    var selected = '';
                                }
                                str += '<option  ' + selected + ' value="' + j + '">' + arr[i]['ServiceName'] + "&nbsp" + j + arr[i]['ServiceUnit'] + '</option>';
                            }
                            str += "</select>\n";
                        }
                        str += '</span>'
                        return str;
                    }
                });

            },
            success: function(data) {
                var extralist = data.data;
                $('body').find("#reviseExtrasM").trigger("toShowExtras", [extralist])
            }
        });
    });












    //  保存接口
    $('a#nowsave').one('click', function a() {
        var thisbutton = $(this);
        var posting = {};
        var cancel = false;


        var warings = new Array();
        var basewarningOBJ = new Object();
        basewarningOBJ.list = new Array();
        basewarningOBJ.cnname = '联系人基本资料';

        var baseInfovalueState = BaseInfoRef.state.valueState;
        for (var x in baseInfovalueState) {
            if (baseInfovalueState[x].isValueMandatory == 0) {
                if (((baseInfovalueState[x]['isValueNotEmpty'] == 1) && (baseInfovalueState[x]['isValueUseAble'] == 0))) {
                    var oneWaring = new Object();
                    oneWaring.title = $('#' + x).closest('.form-group').find('label span:eq(0)').text();
                    oneWaring.warning = '填写值有误';
                    cancel = true;
                    basewarningOBJ.list.push(oneWaring);
                }
            } else {
                if (!((baseInfovalueState[x]['isValueNotEmpty'] == 1) && (baseInfovalueState[x]['isValueUseAble'] == 1))) {
                    var oneWaring = new Object();
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
            var oneorderwarningOBJ = new Object();
            oneorderwarningOBJ.list = new Array();
            for (var j in valueSate[i]) {

                if (valueSate[i][j]['isValueMandatory'] == 2) { //不是必须
                    if (((valueSate[i][j]['isValueNotEmpty'] == 1) && (valueSate[i][j]['isValueUseAble'] == 0))) {
                        if (valueSate[i][j]['tipsvisible'] == false) { //返回日期
                            continue;
                        }
                        cancel = true;
                        breaking = true;

                        var oneWaring = new Object();

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
                        var oneWaring = new Object();
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
                if (warings[i].list.length == 0) {
                    continue;
                }
                strings += warings[i].cnname + '<br/><span style="font-size:13px;">';
                for (var j in warings[i].list) {
                    if (count == 0) {
                        break;
                    }
                    count--;
                    strings += warings[i].list[j].title;
                    strings += warings[i].list[j].warning + '<br/>';
                }
                strings += '</span>';
            }
            console.log(strings)

            jQuery.LangHua.alert({
                title: "提示信息",
                tip1: '请检查以下内容：',
                tip2: strings,
                button: '确定',
                icon: "warning",
                maxHeight: 300,
                indent: false,
                callback: false

            })
            thisbutton.one("click", a);
            var top = $('.tips:eq(0)').offset().top - 50;
            $('body').scrollTop(top, 500);

            return;
        }





        var detectingPost = {
            "CustomerName": $('#CustomerName').val(),
            "Tel": $('#Tel').val(),
            "OrderID": $('#OrderID').text(),
            "TravelDate": systemFieldsMaps[$('#OrderID').text()].TravelDate
        };
        $.ajax({
            url: "/Orders/CheckTravelDate",
            type: 'post',
            contentType: "application/json; charset=utf-8;",
            data: JSON.stringify(detectingPost),
            dataType: 'json',
            success: function(data) {
                if (data) {
                    if (data.length == 0) {
                        finalPost(thisbutton, a);
                    } else {
                        showError(data, 0, thisbutton, a);
                    }
                }
            },
            complete: function(XHR, TS) {
                if (TS !== "success") {
                    $('a#nowsave').one('click', a);
                }
            }
        });
    });

    function showError(errorArr, index, thisbutton, callback) {
        var i, isLast;

        if (errorArr[index].ErrorCode == 401) {
            if (errorArr.length - 1 <= index) {
                isLast = true;
            } else {
                isLast = false;
            }
            if (isLast === true) {
                jQuery.LangHua.confirm({
                    title: "提示信息",
                    tip1: '提示信息：',
                    tip2: errorArr[index].ErrorMessage,
                    confirmbutton: '确定',
                    cancelbutton: '取消',
                    data: null,
                    cancel: function() {
                        thisbutton.one('click', callback);
                    },
                    confirm: function() {
                        finalPost(thisbutton, callback);
                    }
                })
            } else {
                jQuery.LangHua.confirm({
                    title: "提示信息",
                    tip1: '提示信息：',
                    tip2: errorArr[index].ErrorMessage,
                    confirmbutton: '确定',
                    cancelbutton: '取消',
                    data: null,
                    cancel: function() {
                        console.log('xxx')
                        thisbutton.one('click', callback);
                    },
                    confirm: function() {
                        console.log('xxxing')
                        showError(errorArr, parseInt(index) + 1, thisbutton, callback)
                    }
                })
            }
        } else {
            jQuery.LangHua.alert({
                title: "提示信息",
                tip1: '检测失败',
                tip2: '检测失败!',
                button: '确定',
                callback: function() {
                    thisbutton.one('click', callback);
                }
            })
        }


    }

    function finalPost(thisbutton, thisCallback) {
        // var urlARR = window.location.href.split('/')
        // posting.OrderID = urlARR[urlARR.length - 1];
        var posting = {};
        posting.OrderID = $('#OrderID').text()

        posting.Customers = {
            "CustomerID": $('#customerid').text(),
            "CustomerTBCode": $('#CustomerTBCode').text().trim(),
            "CustomerName": $('#CustomerName').val(),
            "CustomerEnname": $('#CustomerEnname').val(),
            "Tel": $('#Tel').val(),
            "BakTel": $('#BakTel').val(),
            "Email": $('#Email').val(),
            "Wechat": $('#Wechat').val(),
        }
        posting.ServiceItemHistorys = {};
        for (var i in postData) {
            posting.ServiceItemHistorys = {
                "OrderID": i,
                "ElementsValue": JSON.stringify(postData[i]),
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
                            TravellerID: c
                        })
                    }
                    return arr
                })(),
                'ServiceItemTemplteValue': JSON.stringify(tempValue[i]),
            }
            break;
        };
        posting.ServiceItemHistorys = $.extend(true, {}, posting.ServiceItemHistorys, systemFieldsMaps[$('#OrderID').text()]);
        var toast = '';
        $.ajax({
            url: "/Orders/SaveOrder",
            type: 'post',
            contentType: "application/json; charset=utf-8;",
            data: JSON.stringify(posting),
            dataType: 'json',
            beforeSend: function() {
                thisbutton.buttonposting("正在保存");
                toast = $.LangHua.loadingToast({
                    tip: '正 在 保 存.....'
                });
            },
            success: function(data) {
                toast.modal('hide');
                if (data.ErrorCode == 200) {
                    // thisbutton.success("保存成功");
                    thisbutton.posttedbutton("保存成功", '保存');
                    if (window.location.href.indexOf("Edit") != -1) {
                        window.location.href = '/Orders/Index?' + 'search={"FuzzySearch":"' + $('#CustomerTBCode').text() + '","status":-1,"searchType":"fuzzy","FuzzySearchType":"TBID"}';
                    }
                } else if (data.ErrorCode == 401) {
                    jQuery.LangHua.alert({
                        title: "提示信息",
                        tip1: '提示信息：',
                        tip2: data.ErrorMessage,
                        button: '确定',
                        callback: function() {
                            thisbutton.posttedbutton("保存");
                        }
                    })
                } else {
                    jQuery.LangHua.alert({
                        title: "提示信息",
                        tip1: '保存失败',
                        tip2: '保存失败！！',
                        button: '确定',
                        callback: function() {
                            cancel = true;
                        }
                    })
                }
            },
            complete: function(XHR, TS) {
                toast.modal('hide');
                if (TS !== "success") {
                    thisbutton.posttedbutton("保存失败", '保存');
                }
                $('a#nowsave').one('click', thisCallback)
            }
        });
    }
});