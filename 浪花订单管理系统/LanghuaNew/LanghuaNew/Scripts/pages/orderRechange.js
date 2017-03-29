jQuery(document).ready(function($) {



    $('#confirmRechange').one("click", function reChangeConfirm() {
        //基本表单变化值
        var valueSatePost = valueSate;
        var valueReChangePost = valueReChange;
        //额外项目
        var valueExtraReChangePost = valueExtraReChange;
        var valueExtraOldPost = valueExtraOld;
        var valueExtraReChangeDisplayPost = valueExtraReChangeDisplay;

        //姓名。人数
        var valueNameAndNumPost = valueNameAndNum;
        var valueOldnameAndNumPost = valueOldnameAndNum;

        //
        var tempValuePost = tempValue[$('#OrderID').text()];
        var postDataPost = postData;



        var postAble = true;
        var hasChanged = false;

        //对通用表单的处理

        var formHasChanged = false;
        var formPostAble = true;
        var tempValuePostFinal = new Object();
        for (var i in valueReChangePost) {
            for (var j in valueReChangePost[i]) {
                if (valueReChangePost[i][j]['isValueChange'] == 1) {
                    if (valueSatePost[i][j]['isValueNotEmpty'] == 1) {
                        formHasChanged = true;
                        hasChanged = true;

                        tempValuePostFinal[j] = tempValuePost[j];
                    } else { //空值对于非必填项有效
                        if (valueSatePost[i][j]['isValueMandatory'] == 2) {
                            tempValuePostFinal[j] = tempValuePost[j];
                        }
                    }
                }

                if (valueSatePost[i][j]['isValueUseAble'] == 0) {
                    if (valueSatePost[i][j]['isValueMandatory'] == 1) {
                        formPostAble = false;
                        postAble = false;
                    }
                }
            }
        }

        //对额外的处理
        var extraHasChanged = false;
        // for(var k in valueExtraReChangePost){
        //     if(valueExtraReChangePost[k]['ServiceNum']!=valueExtraOldPost[k]['ServiceNum']){
        //         extraHasChanged = true;
        //         hasChanged = true;
        //     }
        // }

        var extraOldOBJ = new Object();
        var extraOldNotZero = 0;
        var extraNewOBJ = new Object();
        var extraNewNotZero = 0;
        var extraTheSame = 0;
        for (var z in valueExtraReChangePost) {
            extraOldOBJ[valueExtraReChangePost[z].ExtraServiceID] = valueExtraReChangePost[z];
            if (valueExtraReChangePost.ServiceNum != 0) {
                extraOldNotZero++;
            }
        }
        for (var z in valueExtraOldPost) {
            extraNewOBJ[valueExtraOldPost[z].ExtraServiceID] = valueExtraOldPost[z];
            if (valueExtraOldPost.ServiceNum != 0) {
                extraNewNotZero++;
            }
        }
        for (var zz in extraNewOBJ) {
            if (zz in extraOldOBJ) {
                if (extraNewOBJ[zz].ServiceNum == extraOldOBJ[zz].ServiceNum) {
                    extraTheSame++;
                }
            }
        }
        if ((extraOldNotZero == extraTheSame) && (extraTheSame == extraNewNotZero)) {} else {
            extraHasChanged = true;
            hasChanged = true;
        }
        if (extraHasChanged) {
            tempValuePostFinal['cnAttachedItem'] = valueExtraReChangeDisplayPost.changeAltCN;
            tempValuePostFinal['enAttachedItem'] = valueExtraReChangeDisplayPost.changeAltEN;
        }


        //对姓名和人数的处理
        var nameNumHaschanged = false;
        var keyMap = {
            'CustomerName': 'cnName',
            'CustomerEnname': 'enName',
            'AdultNum': 'Adult',
            'ChildNum': 'Child',
            'INFNum': 'Infant',
            'Tel': 'Tel',
            'BakTel': 'BakTel',
            'Email': 'Email',
            'Wechat': 'Wechat',
            'RoomNum': 'NoOfRoom',
            'RightNum': 'Nights'

        }
        for (var z in valueNameAndNumPost) {
            if ((valueNameAndNumPost[z] != valueOldnameAndNumPost[z]) && (valueNameAndNumPost[z])) {
                hasChanged = true;
                nameNumHaschanged = true;
                tempValuePostFinal[keyMap[z]] = valueNameAndNumPost[z];
            }
        }

        if (!(formPostAble)) {
            jQuery.LangHua.alert({
                title: "警告",
                tip1: '提示信息：',
                tip2: '表单更改值含有 非法的值，如不需要，请清除',
                button: '确定'
            })
            $('#confirmRechange').one("click", reChangeConfirm);
            return;
        }
        // if(!hasChanged){
        //      jQuery.LangHua.alert({
        //         title: "提示信息",
        //         tip1: '提示信息：',
        //         tip2: '',
        //         button: '确定'
        //     })
        //     $('#confirmRechange').one("click",reChangeConfirm);
        //     return ;
        // }


        //添加对人数的保存，为了请求变更确认
        {
            var personSelected = personData;
            var personObj = new Object();
            var personArr = new Array();
            for (var z in personSelected[i]) {
                for (var a in personSelected[i][z]) {
                    for (var aa in personSelected[i][z][a]) {
                        personObj[aa] = personSelected[i][z][a][aa];
                    }
                }
            }
            for (var c in personObj) {
                personArr.push(personObj[c])
            }
        }

        //添加对出行和返回日期的的保存，为了请求变更确认
        {
            var systemMap = systemFieldsMaps;

        }

        //添加对模板值的保存，为了请求变更确认
        {
            var templatevalue = tempValue[$('#OrderID').text()];

        }
        //带入新的订单
        var tmallOrders = $("#reChangeModel").data("tmallorders");
        var TBOrderNos = [];
        if (tmallOrders) {
            for (var x in tmallOrders.subOrderList) {
                TBOrderNos.push({
                    "No": tmallOrders.subOrderList[x].Tid,
                    "SubNo": tmallOrders.subOrderList[x].Oid,
                    "Payment": tmallOrders.subOrderList[x].Payment,
                    "RefundId": tmallOrders.subOrderList[x].RefundId
                });
            }
        }
        var OrderID = $('#OrderID').text();
        var ChangeValue = {
            nameAndNum: valueNameAndNum,
            formElemnents: postDataPost,
            extra: valueExtraReChange,
            extraDisplay: valueExtraReChangeDisplay,
            // 人的选择
            persons: personArr,
            //日期的选择
            systemMap: systemMap,
            // 模板值
            templatevalue: templatevalue
        };
        var m = '';
        $.ajax({
            url: '/Orders/SaveOrderChange',
            type: "post",
            contentType: "application/json; charset=utf-8;",

            data: JSON.stringify({
                'OrderID': OrderID,
                'ChangeElementsValue': JSON.stringify(tempValuePostFinal),
                'ChangeValue': JSON.stringify(ChangeValue),
                'TBOrderNos': TBOrderNos
            }),
            dataType: "json",
            beforeSend: function() {
                m = $.LangHua.loadingToast({
                    tip: "正在保存变更....."
                });
            },
            success: function(data) {
                m.modal("hide");
                if (data.ErrorCode == 200) {
                    $.LangHua.alert({
                        title: "提示",
                        tip1: '保存结果',
                        tip2: '保存成功',
                        button: '确定',
                        callback: function() {
                            // window.location.reload();
                            var search = JSON.stringify({
                                "FuzzySearch": $('#CustomerTBCode').text(),
                                "status": -1,
                                "searchType": "fuzzy",
                                "FuzzySearchType": "TBID"
                            });
                            window.location.href = "/Orders/Index?search=" + search;
                        }
                    })

                } else if (data.ErrorCode == 401) {

                    $.LangHua.alert({
                        title: "提示",
                        tip1: '保存结果',
                        tip2: data.ErrorMessage,
                        button: '确定',
                    })
                    $('#confirmRechange').one("click", reChangeConfirm);

                } else {
                    $('#confirmRechange').one("click", reChangeConfirm);
                }

            },
            comoplete: function(XHR, TS) {
                m.modal("hide");
                m = null;
                if (TS !== "success") {
                    $.LangHua.alert({
                        title: "保存结果",
                        tip1: '保存结果',
                        tip2: '保存失败',
                        button: '确定',
                    })
                    $('#confirmRechange').one("click", reChangeConfirm);

                }

            }
        })
    })



    //更改项目
    $('body').on('click', '#reviseExtrasRechange', function() {
        var thisbutton = $(this);

        var extra = thisbutton.data('extras');
        var extraOld = thisbutton.data('extra-old');

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
                    '<div id="reviseExtrasR" class="modal modal-animate" data-backdrop="static" tabindex="-1" data-focus-on="input:first">',
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

                $('body').one('toShowExtras', '#reviseExtrasR', function(e, data) {
                    var whichModal = $(this);

                    $(this).find(".form-body:eq(0)").empty().append(makeSelects(data, extra));

                    // var oldExtra = new Object();//与请求变更前进行对比
                    // var oldNotZeroCount = 0;
                    // for(var z in extraOld){
                    //     oldExtra[extraOld[z].ExtraServiceID] = extraOld[z];
                    // }

                    $(this).find("#saveExtrasEdit").one('click', function a() {
                        var _this = $(this);
                        // 查找纠错
                        var cancel = false;
                        var arr = [];
                        var changeArrCN = new Array();
                        var changeArrEN = new Array();
                        var minmumCount = 0;
                        var selectCorrectCount = 0;

                        whichModal.find('select').each(function() {
                            var obj = $(this).data('fullinfo');
                            obj.ServiceNum = $(this).val();
                            arr.push(obj);
                            minmumCount += parseInt($(this).data('minnum'));
                            if (parseInt($(this).val()) < ($(this).data('minnum'))) {
                                // cancel = true;
                                // $(this).warning('最少选择：'+obj.MinNum+obj.ServiceUnit);
                            } else {
                                selectCorrectCount++;
                                if (parseInt($(this).val()) > 0) {
                                    changeArrCN.push(
                                        obj.ServiceName + '：' + obj.ServiceNum + obj.ServiceUnit
                                    );
                                    changeArrEN.push(
                                        obj.ServiceEnName + ' ' + obj.ServiceNum + obj.ServiceUnit
                                    );
                                }

                            }
                            // //判断与变更前之前的异同
                            // if(obj.ExtraServiceID in oldExtra){
                            //     if(oldExtra[obj.ExtraServiceID].ServiceNum!=$(this).val()){
                            //     }
                            //     else{
                            //         sameCount++
                            //     }
                            // }
                            // else{
                            // }
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

                        var changeAltCN = changeArrCN.join(',');
                        var changeAltEN = changeArrEN.join(',');
                        thisbutton.data('extras', JSON.stringify(arr));
                        thisbutton.trigger("update", [arr, {
                            changeAltCN: changeAltCN,
                            changeAltEN: changeAltEN
                        }]);

                        _this.success("改项目成功");
                        setTimeout(function() {
                            whichModal.modal('hide');
                        }, 800)
                        _this.one('click', a);
                    })

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
                            str += "<select data-fullinfo='" + JSON.stringify(arr[i]) + "'   data-minnum='" + arr[i]['MinNum'] + "'>";
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
                $('body').find("#reviseExtrasR").trigger("toShowExtras", [extralist])

            }
        })


    })



})