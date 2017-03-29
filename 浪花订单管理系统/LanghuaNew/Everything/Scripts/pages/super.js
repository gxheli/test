$(document).ready(function() {
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

                            setTimeout(function() {
                                whichModal.modal('hide');
                            }, 800)
                        } else if (data.ErrorCode == 400) {
                            jQuery.LangHua.alert({
                                title: "提示信息",
                                tip1: '改人数失败',
                                tip2: data.ErrorMessage,
                                button: '确定',
                            });
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
        });

        $(tmp).modal();
    });


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
                    '</form>',
                    '</div>',
                    '<div class="modal-footer">',
                    '<a id="saveExtrasEdit" class="btn  btn-sm btn-primary button70">确定</a>',
                    '<span></span>',
                    '<a data-dismiss="modal" class="btn  btn-sm btn-default button70">取消</a>',
                    '</div>',
                    '</div>',
                    '</div>',
                    '</div>',
                ].join('\n');
                $(tmp).modal();

                $('body').one('toShowExtras', '#reviseExtrasM', function(e, data) {
                    var whichModal = $(this);

                    $(this).find(".form-body:eq(0)").empty().append(makeSelects(data, extra));
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
                        UpdateExtraService.extraServiceHistorys = arr
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

                                    setTimeout(function() {
                                        whichModal.modal('hide');
                                    }, 800)
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
        var ot = valueSate;
        for (var i in valueSate) {
            var breaking = false;
            var oneorderwarningOBJ = new Object();
            oneorderwarningOBJ.list = new Array();
            for (var j in valueSate[i]) {
                if (valueSate[i][j]['isValueMandatory'] == 2) { //不是必须
                } else {
                    if (valueSate[i][j]['isValueNotEmpty'] == 0) {
                        if (valueSate[i][j]['tipsvisible'] == false) {
                            continue;
                        }
                        cancel = true;
                        breaking = true;
                        var oneWaring = new Object();
                        oneWaring.title = $('#suborder' + i).find('.form-group[data-segmentcode=' + j + ']  label span:eq(1)').text();
                        oneWaring.warning = "未填写";
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

        posting.OrderID = $('#OrderID').text();


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
        }
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
                    thisbutton.posttedbutton('保存成功');
                    jQuery.LangHua.alert({
                        title: "提示信息",
                        tip1: '保存结果',
                        tip2: '保存成功！',
                        button: '确定',
                        callback: function() {
                            thisbutton.posttedbutton('提交保存');
                        }
                    })


                } else if (data.ErrorCode == 401) {
                    jQuery.LangHua.alert({
                        title: "提示信息",
                        tip1: '提示信息：',
                        tip2: data.ErrorMessage,
                        button: '确定',
                        callback: function() {
                            thisbutton.posttedbutton("提交保存");
                        }
                    })
                } else {
                    jQuery.LangHua.alert({
                        title: "提示信息",
                        tip1: '改人数失败!!',
                        tip2: data.ErrorMessage,
                        button: '确定',
                        callback: function() {
                            thisbutton.posttedbutton("提交保存", '提交保存');
                        }
                    })
                }
            },
            complete: function(XHR, TS) {
                toast.modal('hide');
                if (TS !== "success") {
                    thisbutton.posttedbutton("保存失败", '提交保存');
                }
                $('a#nowsave').one('click', a);
            }
        })
    })



    $('#changeState').one('click', function change() {
        var _this = $(this);
        var OrderID = $("#OrderID").text();
        var state = $("#allState").val();
        if (state === "-1") {
            $("#allState").success("请选择状态");
            $('#changeState').one('click', change);
            return;
        };
        $.ajax({
            url: "/Orders/UpdateState",
            type: 'post',
            contentType: "application/json; charset=utf-8;",
            data: JSON.stringify({
                OrderID: OrderID,
                state: state
            }),
            dataType: 'json',
            beforeSend: function() {},
            success: function(data) {
                if (data.ErrorCode == 200) {
                    jQuery.LangHua.alert({
                        title: "提示信息",
                        tip1: '保存结果',
                        tip2: '保存成功！',
                        button: '确定'
                    });
                    $("#statePresentation").text($('#allState option:selected').text());
                } else if (data.ErrorCode == 401) {
                    jQuery.LangHua.alert({
                        title: "提示信息",
                        tip1: '保存失败！',
                        tip2: data.ErrorMessage,
                        button: '确定',
                    })
                } else {
                    jQuery.LangHua.alert({
                        title: "提示信息",
                        tip1: '保存失败!!',
                        tip2: data.ErrorMessage,
                        button: '确定',
                    })
                }
            },
            complete: function(XHR, TS) {
                _this.one('click', change);
            }
        })
    });
    //更改状态

});