jQuery(document).ready(function($) {


    neworders = orderTableInit();
    orderPreList = orderPreInit();

    $("input[type=radio][name=source]").on("change", function() {
        if ($.trim($("input[type=radio][name=source]:checked").siblings("span.vertical-middle:eq(0)").text()) === "天猫") {
            $('#checkTBList').removeClass("hidden");
        } else {
            $('#checkTBList').addClass("hidden");
        }
    });

    $("#checkTBList").on("click", function() {
        var value = jQuery.trim(jQuery('#TBID').val())
        if (!value) {
            jQuery('#TBID').formWarning({
                tips: "亲，请填写淘宝ID"
            });
            return;
        }
        $("#orderPre #TBIDX").data("usefor", 'new')
        getTBListByTBID(value, orderPreList);
    });




    orderManInit();
    initOrderRevise();



    // 提交表单开始
    jQuery('#postSlectItems').one('click', function postOrders() {
        var _this = this;
        var canPostResult = getPost();
        if (!canPostResult.post) {
            if (canPostResult.scrollTop) {
                jQuery('body').animate({ scrollTop: 0 }, 200);
            }


            $(_this).one('click', postOrders);
        } else {
            if (canPostResult.postObj.Orders.length == 0) {
                return;
            }
            var toast = '';
            $.ajax({
                type: 'post',
                dataType: 'json',
                contentType: "application/json; charset=utf-8;",
                data: JSON.stringify(canPostResult.postObj),
                url: '/Orders/CommitOrders',
                beforeSend: function() {
                    toast = $.LangHua.loadingToast({
                        tip: '正在提交订单.....'
                    });
                },
                success: function(data) {
                    toast.modal('hide');
                    if (data.ErrorCode == 200) {
                        window.location.href = '/Orders/OrderFinish?totalCost=' + jQuery('.newordercost .cost').eq(0).text() + '&TBOrderID=' + data.data.TBOrderID;

                    } else {
                        if (data.ErrorCode == 401) {
                            $.LangHua.alert({
                                title: "提示信息",
                                tip1: '提交订单失败',
                                tip2: data.ErrorMessage,
                                button: '确定'
                            })
                        } else {
                            $.LangHua.alert({
                                title: "提示信息",
                                tip1: '提交订单失败',
                                tip2: "提交订单失败",
                                button: '确定'
                            })
                        }
                        jQuery(_this).one('click', postOrders);
                    }
                },

                complete: function(XHR, TS) {
                    toast.modal('hide');
                    if (TS !== "success") {
                        _this.one('click', b);

                        jQuery(_this).one('click', postOrders);
                    }
                }



            })
        }

        function getPost() {
            var post = true;
            var scrollTop = false;
            var postObj = {
                "TBID": (function() {
                    var value = jQuery.trim(jQuery('#TBID').val())
                    if (!value) {
                        jQuery('#TBID').formWarning({
                            tips: "亲，请填写淘宝ID"
                        });
                        post = false;
                        scrollTop = true;
                    }
                    return value;
                })(),
                "OrderSourseID": (function() {
                    var value = jQuery('#source input.source[name=source]:checked').val();
                    if (!value) {
                        jQuery('#source').Warning({
                            title: "亲，请选择 "
                        });
                        scrollTop = true;
                        post = false;
                    }
                    return value;
                })(),
            }

            var orders = [];
            var matchCheck = false;
            jQuery('#itemListNewOrder tbody tr.itemRow').each(function() {
                var oneItem = {};
                var neworders = $('table#itemListNewOrder').DataTable();
                var rowData = neworders.row(this).data();
                if ($(this).hasClass("notMatched")) {
                    post = false;
                    matchCheck = true;
                } else {
                    oneItem.SupplierID = rowData.lhData.data.ItemSupliers.SupplierID;
                    oneItem.ItemID = rowData.lhData.data.ServiceItemID;
                }

                //检查基础收费
                jQuery(this).find('td .baseService, td .roomnight').each(function() {
                    var map = {
                        adult: 'AdultNum',
                        child: 'ChildNum',
                        infant: 'INFNum',
                        night: 'RoomNum',
                        numbers: 'RightNum'
                    }
                    var thisValue = jQuery.trim(jQuery(this).val());
                    if (!thisValue) {
                        post = false;
                        oneItem[map[jQuery(this).attr('which')]] = parseInt(jQuery(this).val());
                    } else {
                        oneItem[map[jQuery(this).attr('which')]] = parseInt(jQuery(this).val());
                    }
                })
                var totalpeoplenum = parseInt(oneItem['AdultNum']) + parseInt(oneItem['ChildNum']) + parseInt(oneItem['INFNum']);
                if (totalpeoplenum == 0) {
                    jQuery(this).find(".person3group").Warning({
                        title: "亲，请填写"
                    });
                    post = false;
                }
                var nightroomnumber = oneItem['RoomNum'] * oneItem['RightNum'];
                if (nightroomnumber == 0) {
                    jQuery(this).find(".nightroomgroup").Warning({
                        title: "亲，请填写"
                    });
                    post = false;
                }

                // 检查额外收费
                oneItem.ExtraServiceHistorys = [];
                jQuery(this).find('td .extraTD').each(function() {
                    var _this = this;
                    var minmumCount = 0;
                    var selectCorrectCount = 0;
                    jQuery(this).find('.extraService').each(function() {
                        minmumCount += parseInt($(this).attr('min'));
                        var thisValue = Number(jQuery(this).val());
                        var thisMin = Number(jQuery(this).attr('min'));
                        if (thisValue < thisMin) {
                            // jQuery(this).addClass('warning');
                        } else {
                            if (thisValue > 0) {
                                selectCorrectCount++;
                                oneItem.ExtraServiceHistorys.push({
                                    ExtraServiceID: jQuery(this).attr('id').split('ExtraService')[1],
                                    ServiceNum: thisValue
                                })
                            }
                        }
                    });
                    if (minmumCount == 0) {
                        // post = true;
                    } else {
                        if (selectCorrectCount == 0) {
                            post = false;
                            jQuery(_this).warning('最少选择其中一项');
                        } else {}
                    }
                });
                var i, j;
                //淘宝订单号,多个取第一个
                var TBNum = "";
                for (i in rowData.TBOrderID) {
                    TBNum = i;
                    break;
                };
                oneItem.TBNum = TBNum;
                //利润信息,
                var TBOrderNos = [];
                for (i in rowData.TBOrderID) {
                    if (rowData.TBOrderID[i] && rowData.TBOrderID[i].length !== 0) {
                        for (j in rowData.TBOrderID[i]) {
                            TBOrderNos.push({
                                "No": i,
                                "SubNo": rowData.TBOrderID[i][j],
                                "Payment": rowData.tmallData.subOrderList[rowData.TBOrderID[i][j]].Payment,
                                "RefundId": rowData.tmallData.subOrderList[rowData.TBOrderID[i][j]].RefundId
                            });
                        }

                    }
                }
                oneItem.TBOrderNos = TBOrderNos;
                var Customer = {
                        "CustomerName": "",
                        "Tel": "",
                        "Email": ""
                    }
                    //预填写信息
                if (rowData.tmallData.hasData === true) {
                    for (i in rowData.tmallData.subOrderList) {
                        if (rowData.tmallData.subOrderList[i].ItineraryInfo) {
                            var regChinese = /^[\u4e00-\u9fa5 ]+$/;
                            if (rowData.tmallData.subOrderList[i].ItineraryInfo.TravelContactName && regChinese.test(rowData.tmallData.subOrderList[i].ItineraryInfo.TravelContactName)) {
                                Customer.CustomerName = rowData.tmallData.subOrderList[i].ItineraryInfo.TravelContactName;
                            }
                            var regEmail = /^([a-zA-Z0-9_#*~$^`|;:"'/?<>,&\\\(\)={}\[\]\%\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{1,10})+$/;
                            if (rowData.tmallData.subOrderList[i].ItineraryInfo.TravelContactMail && regEmail.test(rowData.tmallData.subOrderList[i].ItineraryInfo.TravelContactMail)) {
                                Customer.Email = rowData.tmallData.subOrderList[i].ItineraryInfo.TravelContactMail;
                            }
                            if (rowData.tmallData.subOrderList[i].ItineraryInfo.TravelContactMobile) {
                                Customer.Tel = rowData.tmallData.subOrderList[i].ItineraryInfo.TravelContactMobile;
                            }
                        }
                    }
                };
                oneItem.Customer = Customer;
                orders.push(oneItem);
            })
            postObj.Orders = orders;
            if (matchCheck === true) {
                $.LangHua.alert({
                    title: "提示信息",
                    tip1: '温馨提示',
                    tip2: '已选产品中含有<span style="color:red">无法匹配</span>的产品',
                    button: '确定'
                });
            }
            return {
                scrollTop: scrollTop,
                post: post,
                postObj: postObj
            };
        }

    });
    // 提交表单结束


































    $("#haveNotOrderedOnly").on('change', function() {
        orderPreList.draw();
    });
    $("#enableOnly").on('change', function() {
        orderPreList.draw();
    });
    $("#orderPre  #flashOrderPre").on('click', { "orderPreList": orderPreList }, function(e) {
        if ($(this).hasClass("getting")) {
            return;
        }
        getTBListByTBID($.trim($("#orderPre  #TBIDX").val()), e.data.orderPreList);
        $(this).addClass("getting");
    });
    $("#orderPre  #checkTBListR").on('click', { "orderPreList": orderPreList }, function(e) {
        if ($(this).hasClass("getting")) {
            return;
        }
        getTBListByTBID($.trim($("#orderPre  #TBIDX").val()), e.data.orderPreList);
        $(this).addClass("getting")
    });



    $("#orderPre #setOrders").on("click", function setOrders() {
        if ($("#orderPre #selecteNum").text() && parseInt($("#orderPre #selecteNum").text()) === 0) {
            $(this).success("请选择订单")
            return;
        };

        var orderPreListWithCode = [];
        var counterNotMatched;
        var j;
        var matched = false;
        var temp, code, _this, suborder;


        var subOrderData, subOrderRow, others;
        var subOrderList = [];
        for (var i = 0; i < $("#orderPre .selectThisSubOrderPre:checked").length; ++i) {
            subOrderRow = $("#orderPre .selectThisSubOrderPre:checked:eq(" + i + ")");
            others = subOrderRow.closest(".order-pre").data("others");
            subOrderData = subOrderRow.closest(".order-pre").data("suborder");
            subOrderData.Tid = others.Tid;
            subOrderData.OrderNo = others.OrderNo;
            subOrderList.push(subOrderData)
        }


        //重复添加提示
        var existStr = "";
        for (j in subOrderList) {
            if (
                (subOrderList[j].OrderNo === "") ||
                (subOrderList[j].OrderNo === undefined) ||
                (subOrderList[j].OrderNo === null)
            ) {
                continue;
            }
            existStr += "<div>" + "订单号：" + subOrderList[j].OrderNo + "</div>";
        }
        if (existStr !== "") {
            $.LangHua.confirm({
                title: "提示信息",
                tip1: '选择的订单中以下订单<span class="red">已有系统订单</span>，请确认是否继续生成订单？',
                tip2: existStr,
                confirmbutton: '确定',
                cancelbutton: '取消',
                data: null,
                confirm: function() {

                    if ($('#orderPre #TBIDX').data("usefor") == "add") {
                        combineTmallData(subOrderList, $("#orders-edit #data-container").data('rowdata'));

                    } else {
                        getItemByCode(subOrderList);

                    }
                }
            });
        } else {

            if ($('#orderPre #TBIDX').data("usefor") == "add") {
                combineTmallData(subOrderList, $("#orders-edit #data-container").data('rowdata'));

            } else {
                getItemByCode(subOrderList);

            }
        }
    });









    $("#itemListNewOrder").on("click", ".reviseAgain", { "neworders": neworders }, function(e) {
        var tr = $(this).closest(".itemRow")[0];
        var neworders = e.data.neworders;
        var rowData = neworders.row(tr).data();
        var rowindex = neworders.row(tr).index();
        $("#orders-edit #data-container").data('rowdata', $.extend(true, {}, rowData));
        $("#orders-edit #data-container").data('rowindex', rowindex);
        $("#orders-edit").trigger("update");
        $("#orders-edit").modal();

    });

    $("body").on("update", '#orders-edit', function() {
        var thisModal = $(this);
        var rowData = thisModal.find('#data-container').data("rowdata");
        thisModal.find("#typeaheadR").typeahead('val', "");

        var div = $("<div></div>");
        var span = $("<span></span>");
        var one = div.clone().addClass("one");
        var no = span.clone().addClass("no");
        var deletex = span.clone().addClass("delete");
        var arrTBOrderID = [];
        for (var i in rowData.TBOrderID) {
            arrTBOrderID.push(one.clone().append(
                no.clone().text(i),
                deletex.clone()
            ))
        }
        thisModal.find("#orderNolist").empty().append(arrTBOrderID);
        if (rowData.lhData.hasData === true) {
            var ServiceItemID = $.trim(rowData.lhData.data.ServiceItemID);
            var SupplierID = $.trim(rowData.lhData.data.ItemSupliers.SupplierID);
            thisModal.find("#typeaheadR").one('typeahead:render', function(jq, menu, flag, xx) {
                var int = window.setInterval(function() {

                    if (thisModal.find('.tt-menu #serviceItemID' + ServiceItemID).length !== 0) {
                        thisModal.find('.tt-menu #serviceItemID' + ServiceItemID).trigger("click");
                        thisModal.find('#suppliersR').val(SupplierID);
                        clearInterval(int);
                    }
                }, 100);
            });
        }
        thisModal.find("#typeaheadR").typeahead('val', rowData.ServiceCode);
    })
    $('body').on("shown.bs.modal", '#orders-edit', function(e) {});
    $('body').on("hide.bs.modal", '#orders-edit', function(e) {
        var thisModal = $(this);
        thisModal.find("#orderNolist").siblings(".help-inline:eq(0)").text("").removeClass("tips");
        thisModal.find("#typeaheadR").typeahead('val', '');
        thisModal.find("#typeaheadR").trigger("change");
        thisModal.find("#typeaheadR").data('which', "");;
        thisModal.find("#typeaheadR").closest('.form-group').find('#serviceCodeR').text("");
        // thisModal.find("#typeaheadR").trigger("typeahead:render");
        thisModal.find("#suppliersR").empty().text("");
    })

    jQuery('#addOneItemR').on("click", { neworders: neworders }, function(e) {
        var neworders = e.data.neworders;
        var thisModal = $(this).closest("#orders-edit");
        var dataPrepare = thisModal.find('#data-container').data("temp");


        if (thisModal.find("#orderNolist .one").length === 0) {
            thisModal.find("#orderNolist").formWarning({
                tips: "亲，至少一个淘宝订单号"
            });
            return;
        }

        // if (!(TBORDERIDReg.test(TBORDERID))) {
        //     if (TBORDERID.length < 5) {
        //         thisModal.find("#TBORDERIDR").formWarning({
        //             tips: "亲，淘宝订单号起码长度至少5位"
        //         });
        //     } else {
        //         thisModal.find("#TBORDERIDR").formWarning({
        //             tips: "亲，淘宝订单号不应该含有字母和数字之外的字符，中间也不要含有空格"
        //         });
        //     }
        //     return;
        // }
        if (!jQuery('#typeaheadR').data('which') && !jQuery('#suppliersR').val()) {
            $(this).success("亲，选择产品和供应商");
            return;
        }
        $.ajax({
            url: '/Orders/GetItemByID',
            type: 'post',
            dataType: 'json',
            data: {
                ItemID: jQuery('#typeaheadR').data('which'),
                SupplierID: jQuery('#suppliersR').val()
            },
            success: function(data) {
                var openModals = $("body").data("modalmanager").getOpenModals();
                if (openModals) {
                    for (var i in openModals) {
                        if ($(openModals[i]['$element'][0]).attr("id") !== "orderPre") {
                            $(openModals[i]['$element'][0]).modal("hide");
                        }
                    }
                }
                if (data.ErrorCode != 200) {
                    return;
                }
                var again = false;
                var rows = jQuery('table#itemListNewOrder').find('tr.itemRow');
                var rowIndex = thisModal.find('#data-container').data("rowindex");
                for (var i = 0; i < rows.length; i++) {
                    if (rowIndex != i && (jQuery(rows[i]).attr("id")) && jQuery(rows[i]).attr("id") == data['Item']['ServiceItemID']) {
                        again = true;
                        $.LangHua.confirm({
                            title: "提示信息",
                            tip1: '请确认是否需要<span class="red">重复</span>添加下列产品？',
                            tip2: data['Item']['cnItemName'],
                            confirmbutton: '确定',
                            cancelbutton: '取消',
                            data: null,
                            confirm: function() {
                                var rowIndex = thisModal.find('#data-container').data("rowindex");
                                var rowData = thisModal.find('#data-container').data("rowdata");
                                rowData.lhData.data = data.Item;
                                rowData.lhData.hasData = true;
                                rowData.ServiceCode = data.Item.ServiceCode;
                                neworders.row(rowIndex).data(rowData);
                                neworders.draw();
                            }
                        });
                        break;
                    }
                }
                if (rows.length == 0 || !again) {
                    var rowIndex = thisModal.find('#data-container').data("rowindex");
                    var rowData = thisModal.find('#data-container').data("rowdata");
                    rowData.lhData.data = data.Item;
                    rowData.lhData.hasData = true;
                    rowData.ServiceCode = data.Item.ServiceCode;
                    neworders.row(rowIndex).data(rowData);
                    neworders.draw();
                }
            }
        })
    });

    jQuery('#orders-edit').on("click", "a#addTmallData", { "orderPreList": orderPreList }, function(e) {
        e.data.orderPreList.rows().remove();
        $("#orderPre #TBIDX").val("");
        $("#orderPre #TBIDX").data("usefor", "add");
        $("#orderPre #enableOnly").prop("checked", true);
        $("#orderPre #haveNotOrderedOnly").prop("checked", false);
        $('#orderPre').modal('show');
        e.data.orderPreList.draw();

    });



    jQuery('#orders-edit').on("click", "#orderNolist .delete", function(e) {
        var theTBOrder = $(this).closest(".one");
        var TBOrderID = $.trim(theTBOrder.find(".no:eq(0)").text());
        var ordersEdit = theTBOrder.closest("#orders-edit");
        theTBOrder.remove();
        var rowData = ordersEdit.find("#data-container").data('rowdata');
        delete rowData.TBOrderID[TBOrderID];
        delete rowData.tmallData.subOrderList[TBOrderID];
        var i = 0;
        for (var j in rowData.tmallData.subOrderList) {
            i = 1;
            break;
        }
        if (i === 1) {
            rowData.tmallData.hasData = true;
        } else {
            rowData.tmallData.hasData = false
        }
        ordersEdit.find("#data-container").data('rowdata', rowData);
    });



    function getTBListByTBID(TBID, orderPreList) {
        $.ajax({
            'url': "/Orders/GetTBList",
            'type': "post",
            'data': {
                "BuyerNick": TBID
            },
            "dataType": 'json',
            "beforeSend": function() {
                $.LangHua.loadingToast({
                    tip: "正 在 拉 取 信 息. . . . . ."
                });
                $("#orderPre  #flashOrderPre i:eq(0)").addClass("fa-spin");

            },
            "success": function(data) {
                var openModals = $("body").data("modalmanager").getOpenModals();
                if (openModals) {
                    for (var i in openModals) {
                        if (($(openModals[i]['$element'][0]).attr("id") !== "orderPre") &&
                            ($(openModals[i]['$element'][0]).attr("id") !== "orders-edit")) {
                            $(openModals[i]['$element'][0]).modal("hide");
                        }
                    }
                }
                $("#orderPre  #flashOrderPre").removeClass("getting");
                $("#orderPre  #checkTBListR").removeClass("getting");
                $("#orderPre  #flashOrderPre i:eq(0)").removeClass("fa-spin");
                if (data.ErrorCode == 200) {
                    orderPreList.rows().remove();
                    $("#orderPre #TBIDX").val(TBID);
                    $("#orderPre #enableOnly").prop("checked", true);
                    $("#orderPre #haveNotOrderedOnly").prop("checked", false);
                    $('#orderPre').modal('show');
                    orderPreList.rows.add(data.data);

                    orderPreList.draw();
                } else {
                    if (data.ErrorCode == 401) {
                        $.LangHua.alert({
                            title: "提示信息",
                            tip1: '拉取数据失败',
                            tip2: data.ErrorMessage,
                            button: '确定'
                        })
                    } else {
                        $.LangHua.alert({
                            title: "提示信息",
                            tip1: '拉取数据失败',
                            tip2: "拉取数据失败，请重试！",
                            button: '确定'
                        })
                    }
                }
            },
            "error": function(jqXHR, textStatus, errorThrown) {
                var openModals = $("body").data("modalmanager").getOpenModals();
                if (openModals) {
                    for (var i in openModals) {
                        if (($(openModals[i]['$element'][0]).attr("id") !== "orderPre") &&
                            ($(openModals[i]['$element'][0]).attr("id") !== "orders-edit")) {
                            $(openModals[i]['$element'][0]).modal("hide");
                        }
                    }
                }
                $("#orderPre  #flashOrderPre").removeClass("getting");
                $("#orderPre  #checkTBListR").removeClass("getting");
                $("#orderPre  #flashOrderPre i:eq(0)").removeClass("fa-spin");
                $.LangHua.alert({
                    title: "提示信息",
                    tip1: '拉取数据失败',
                    tip2: "拉取数据失败，请检查网络，再重试！",
                    button: '确定'
                })
            }
        })
    }

    function getItemByCode(subOrderList) {
        var codeStr = "";
        var codeObj = {};
        for (var i in subOrderList) {
            if ((subOrderList[i].OuterSkuId)) {
                if (codeStr === "") {
                    codeStr += subOrderList[i].OuterSkuId + "^";
                } else {
                    if (!(subOrderList[i].OuterSkuId in codeObj)) {
                        codeStr += subOrderList[i].OuterSkuId + "^";
                    }
                }
                codeObj[subOrderList[i].OuterSkuId] = "1";
            }
        };
        codeStr = codeStr.replace(/\^$/, "");
        $.ajax({
            'url': "/Orders/GetItemByCode",
            'type': "post",
            'data': {
                "Code": codeStr
            },
            "dataType": 'json',
            "success": function(data) {
                handlerData(subOrderList, data);
            }
        });
    }


    function handlerData(tmallData, lhData) {
        var i, j, one, k, z, objtemp, flagExist, flagExistTid, flagExistInLhData;
        var arrCombination = [];
        for (i in tmallData) {
            if (!(tmallData[i].OuterSkuId)) {
                one = {};
                one.ServiceCode = tmallData[i].OuterSkuId;
                objtemp = {};
                objtemp[tmallData[i].Tid] = [tmallData[i].Oid];
                one.TBOrderID = objtemp;
                one.lhData = {
                    hasData: false,
                    data: {}
                };
                objtemp = {};
                objtemp[tmallData[i].Oid] = tmallData[i];
                one.tmallData = {
                    hasData: true,
                    subOrderList: objtemp
                };
                arrCombination.push(one);
                continue;
            }
            flagExistInLhData = false;
            for (j in lhData) {
                if (tmallData[i].OuterSkuId == lhData[j].Code) {
                    if (lhData[j].ErrorCode != 200) {
                        one = {};
                        one.ServiceCode = tmallData[i].OuterSkuId;
                        objtemp = {};
                        objtemp[tmallData[i].Tid] = [tmallData[i].Oid];
                        one.TBOrderID = objtemp;
                        one.lhData = {
                            hasData: false,
                            data: {}
                        };
                        objtemp = {};
                        objtemp[tmallData[i].Oid] = tmallData[i];
                        one.tmallData = {
                            hasData: true,
                            subOrderList: objtemp
                        };
                        arrCombination.push(one);
                    } else {
                        //考虑合并
                        flagExist = false;
                        for (k in arrCombination) {
                            if (tmallData[i].OuterSkuId == arrCombination[k].ServiceCode) {
                                if (tmallData[i].Tid in arrCombination[k].TBOrderID) {
                                    arrCombination[k].TBOrderID[tmallData[i].Tid].push(tmallData[i].Oid);
                                } else {
                                    flagExistTid = false;
                                    for (z in arrCombination[k].TBOrderID) {
                                        flagExistTid = true
                                        break;
                                    }
                                    if (flagExistTid === true) {
                                        arrCombination[k].TBOrderID[tmallData[i].Tid] = [tmallData[i].Oid];
                                    } else {
                                        objtemp = {};
                                        objtemp[tmallData[i].Tid] = [tmallData[i].Oid];
                                        arrCombination[k].TBOrderID = objtemp;
                                    }

                                }
                                arrCombination[k].tmallData.subOrderList[tmallData[i].Oid] = tmallData[i];
                                flagExist = true;
                                break;
                            }
                        }
                        if (flagExist === false) {
                            one = {};
                            one.ServiceCode = tmallData[i].OuterSkuId;
                            objtemp = {};
                            objtemp[tmallData[i].Tid] = [tmallData[i].Oid];
                            one.TBOrderID = objtemp;
                            one.lhData = {
                                hasData: true,
                                data: lhData[j].Item
                            };
                            objtemp = {};
                            objtemp[tmallData[i].Oid] = tmallData[i];
                            one.tmallData = {
                                hasData: true,
                                subOrderList: objtemp
                            };
                            arrCombination.push(one);
                        }
                    }
                    // flagExistInLhData = true;
                    // break;
                }
            }
            // if (flagExistInLhData === false) {

            // }
        }
        // 总数是10个的上限限制，这个需要滞后
        var rows = jQuery('table#itemListNewOrder').find('tr.itemRow');
        if (rows.length + arrCombination.length > 10) {
            $.LangHua.alert({
                title: "提示信息",
                tip1: '温馨提示',
                tip2: '最多一次添加10个产品',
                button: '确定'
            })
            return;
        }

        //检查重复
        var existStr = "";
        var rows = jQuery('table#itemListNewOrder').find('tr.itemRow');
        for (var j in arrCombination) {
            for (var i = 0; i < rows.length; i++) {
                if (jQuery(rows[i]).attr("id") && (jQuery(rows[i]).attr("id") == arrCombination[j].lhData.data.ServiceItemID)) {
                    existStr += "<div>" + "产品：" + arrCombination[j].lhData.data.cnItemName + "</div>";
                    break;
                }
            }
        }
        if (existStr !== "") {
            $.LangHua.confirm({
                title: "提示信息",
                tip1: '请确认是否需要<span class="red">重复</span>添加下列产品？',
                tip2: existStr,
                confirmbutton: '确定',
                cancelbutton: '取消',
                data: null,
                confirm: function() {
                    var openModals = $("body").data("modalmanager").getOpenModals();
                    if (openModals) {
                        for (var i in openModals) {
                            $(openModals[i]['$element'][0]).modal("hide");
                        }
                    }
                    $('#itemListNewOrder').trigger("addItems", [
                        arrCombination,
                        true
                    ]);
                }
            })
        } else {
            var openModals = $("body").data("modalmanager").getOpenModals();
            if (openModals) {
                for (var i in openModals) {
                    $(openModals[i]['$element'][0]).modal("hide");
                }
            };
            $('#itemListNewOrder').trigger("addItems", [
                arrCombination,
                true
            ]);
        }

    }

    function combineTmallData(arr1, rowData) {
        var i, hasSubOrder;
        var subOrderList = rowData.tmallData.subOrderList;
        var TBOrderID = rowData.TBOrderID;
        for (i in arr1) {
            if (!(arr1[i].Oid in subOrderList)) {
                subOrderList[arr1[i].Oid] = arr1[i];
                if (!(arr1[i].Tid in TBOrderID)) {
                    TBOrderID[arr1[i].Tid] = [arr1[i].Oid];
                } else {
                    TBOrderID[arr1[i].Tid].push(arr1[i].Oid)
                }
            }
        }
        hasSubOrder = false;
        for (i in subOrderList) {
            hasSubOrder = true;
            break;
        }
        if (hasSubOrder === true) {
            rowData.tmallData.hasData = true;
        }
        $("#orders-edit").trigger("update");
        var openModals = $("body").data("modalmanager").getOpenModals();
        if (openModals) {
            for (var i in openModals) {
                if ($(openModals[i]['$element'][0]).attr("id") !== "orders-edit") {
                    $(openModals[i]['$element'][0]).modal("hide");
                }
            }
        }
        $("body").trigger("resize")
    }
});

function searchEngineInit() {
    var remote_cities = new Bloodhound({
        datumTokenizer: function(d) {
            return Bloodhound.tokenizers.whitespace(d.name);
        },
        queryTokenizer: Bloodhound.tokenizers.whitespace,
        limit: 15,
        remote: {
            initialize: false,
            wildcard: '%QUERY',
            url: '/Orders/GetItemsByStr?Str=%QUERY',
            method: "post",
            prepare: function(xhr, settings) {
                settings.dataType = 'json';
                settings.type = 'POST';
                settings.data = { Str: xhr };
                return settings;
            },
            filter: function(data) {
                return $.map(data.Items, function(country) {
                    return {
                        name: country.cnItemName,
                        enName: country.enItemName,
                        supplyer: country.ItemSupliers,
                        defaultSupplierID: country.DefaultSupplierID,
                        serviceItemID: country.ServiceItemID,
                        serviceCode: country.ServiceCode
                    };
                });
            }
        }
    });
    remote_cities.initialize();
    return remote_cities;
}

function orderTableInit() {
    var neworders =
        jQuery('table#itemListNewOrder')
        .DataTable({
            ordering: false,
            stateSave: false,
            language: {
                "emptyTable": "您尚未添加产品",
            },
            dom: "t",
            "initComplete": function(settings, json) {
                // 用于数据加载完成
                var api = this.api();
                jQuery(this).bind("addItems", function(event, data, isFromTmall) {
                    api.rows.add(data);
                    api.draw();
                    if (isFromTmall !== true) {
                        $('#TBORDERID').val('');
                        $('#serviceCode').text('');
                        $('#typeahead').typeahead('val', '');
                        $('#suppliers').empty().text("");
                    }
                });
                jQuery(this).on("updateAllCost", function(e) {
                    //更新代价提示
                    var allCost = 0;
                    jQuery(this).find('tr.itemRow td span.cost').each(function() {
                        allCost += parseInt(jQuery(this).text());
                    });
                    jQuery('.newordercost  .cost').eq(0).text(allCost);
                });
                jQuery(this).on('click', '.cancel', function() {
                    var rowDom = $(this).closest("tr.itemRow");
                    api
                        .row(rowDom)
                        .remove()
                        .draw();
                    jQuery(this).trigger('updateAllCost');

                });
            },
            "createdRow": function(row, data, dataIndex) {},
            "drawCallback": function(settings) {
                var api = this.api();
                jQuery("table#itemListNewOrder").trigger('updateAllCost');
            },
            // "rowId": "lhData.ServiceItemID",
            "rowCallback": function(row, data, dataIndex) {
                jQuery(row).addClass("itemRow");
                var id = "";
                if (data.lhData.hasData === true) {
                    id = data.lhData.data.ServiceItemID;
                }
                jQuery(row).attr("id", id);
                var _this = this.api();
                var thisTable = this;
                //缓存单行数据
                if (data.lhData.hasData === true) {
                    jQuery(row).data({
                        // "rowId": data.lhData.ServiceItemID,
                        // "SupplierID": data.lhData.ItemSupliers.SupplierID,
                        'payType': data.lhData.data.SupplierServiceItemView.PayType,
                        "adult": data.lhData.data.SupplierServiceItemView.ItemPriceBySupplier.AdultNetPrice,
                        "child": data.lhData.data.SupplierServiceItemView.ItemPriceBySupplier.ChildNetPrice,
                        "infant": data.lhData.data.SupplierServiceItemView.ItemPriceBySupplier.BobyNetPrice,
                        "room": data.lhData.data.SupplierServiceItemView.ItemPriceBySupplier.Price,
                        "ServiceTypeID": data.lhData.data.ServiceTypeID
                    });
                }
                //更新提示数据
                jQuery(row).addClass("itemRow");
                jQuery(row).find('input.baseService, input.roomnight').onlyNum();
                jQuery(row).on("change", "input.baseService, input.roomnight, select.extraService", function() {
                    //先更新单行提示数据
                    // 按人头计费
                    if (jQuery(row).data('payType') == 0) { //

                        var baseServiceCost =
                            Number(jQuery(row).find('input.adult').eq(0).val()) * Number(jQuery(row).data('adult')) +
                            Number(jQuery(row).find('input.child').eq(0).val()) * Number(jQuery(row).data('child')) +
                            Number(jQuery(row).find('input.infant').eq(0).val()) * Number(jQuery(row).data('infant'));


                        var extraServiceCost = 0;
                        jQuery(row).find('td select.extraService').each(function() {
                            extraServiceCost +=
                                Number(jQuery(this).attr("unitpice")) * Number(jQuery(this).val());
                        })
                        var costAll = parseInt(baseServiceCost + extraServiceCost);

                        jQuery(row).find("td span.cost").text(costAll)

                    }
                    //按产品数量计费,需要计算间数和晚数
                    else {
                        if (jQuery(row).find('input.nights').length == 0) {
                            var baseServiceCost = 0
                        } else {
                            var baseServiceCost =
                                parseInt(jQuery(row).find('input.nights').eq(0).val()) * parseInt(jQuery(row).find('input.numbers').eq(0).val()) * parseInt(jQuery(row).data('room'));
                        }
                        var extraServiceCost = 0;
                        jQuery(row).find('td select.extraService').each(function() {
                            extraServiceCost +=
                                Number(jQuery(this).attr("unitpice")) * Number(jQuery(this).val());
                        })
                        var costAll = parseInt(baseServiceCost + extraServiceCost);

                        jQuery(row).find("td span.cost").text(costAll);
                    }

                    // 然后在更新整单提示数据
                    jQuery(thisTable).trigger('updateAllCost');
                });
                jQuery(row).find("input.baseService:eq(0)").trigger("change");
                if (data.lhData.hasData !== true) {
                    jQuery(row).addClass("notMatched");
                } else {
                    jQuery(row).removeClass("notMatched");
                }
            },
            "columns": [{
                    'data': null,
                    "render": function(cellData, type, rowData, meta) {
                        if (type != "display") {
                            return null;
                        }
                        return null;
                        // return ('<input class="TBOrderID" type="hidden" value="' + rowData.TBOrderID + '"/>');
                    }
                },
                {
                    'data': null,
                    "render": function(cellData, type, rowData, meta) {
                        if (type != "display") {
                            return null;
                        }
                        if (rowData.lhData.hasData === true) {
                            return (
                                '<a class="reviseAgain">' + '(' + rowData.lhData.data.ItemSupliers.SupplierNo + ')' + rowData.lhData.data.cnItemName + rowData.ServiceCode + '</a>'
                            );
                        } else {
                            var code = rowData.ServiceCode;
                            if (!code) {
                                code = "不含编码"
                            }
                            return (
                                '<a class="reviseAgain">' + '无法匹配：' + '<span style="color:red">' + code + '</span>' + '</a>'
                            );
                        }
                    }
                },
                // 基础费用
                {
                    'data': null,
                    "render": function(cellData, type, rowData, meta) {
                        if (type != "display") {
                            return null;
                        }
                        if (rowData.lhData.hasData !== true) {
                            return ("");
                        }
                        var priceKeys = {
                            "AdultNetPrice": {
                                position: 0,
                                className: 'adult',
                                "classNameTmall": "AdultGuestNum"
                            },
                            "ChildNetPrice": {
                                position: 1,
                                className: 'child',
                                "classNameTmall": "ChildGuestNum"
                            },
                            "BobyNetPrice": {
                                position: 2,
                                className: 'infant',
                                "classNameTmall": ""
                            },


                            'maxLength': 3
                        }

                        var itemPriceBySupplier = rowData.lhData.data.SupplierServiceItemView.ItemPriceBySupplier;

                        var arr = {};
                        var personNumberFromTmall = 0;
                        for (var i in priceKeys) {

                            if (i == 'maxLength') {
                                continue;
                            }
                            personNumberFromTmall = 0;
                            if (rowData.tmallData.hasData === true) {
                                for (var j in rowData.tmallData.subOrderList) {
                                    if (rowData.tmallData.subOrderList[j].ItineraryInfo && rowData.tmallData.subOrderList[j].ItineraryInfo[priceKeys[i].classNameTmall]) {
                                        personNumberFromTmall += parseInt(rowData.tmallData.subOrderList[j].ItineraryInfo[priceKeys[i].classNameTmall]);
                                    }
                                }
                            }
                            if (i in itemPriceBySupplier) {
                                arr[priceKeys[i]['position']] = '<input type="text" which="' + priceKeys[i]['className'] + '" value=' + personNumberFromTmall + ' class="' + priceKeys[i]['className'] + ' baseService  service persons3 roomnumber">';
                            }
                        }


                        var str = '<span class="person3group" style="display:inline-block">';

                        for (var j = 0; j < priceKeys.maxLength; j++) {
                            if (j in arr) {
                                str += arr[j];
                            } else {
                                str +=
                                    '<span style="width:40px;height:30px;display:inline-block;position:relative;vertical-align:top"></span>';
                            }
                        }
                        str += '</span>';
                        if (rowData.lhData.data.ServiceTypeID == 4) { //酒店类
                            str += '<span class="nightroomgroup"  style="display:inline-block"><input type="text"  which="night" value=0 class="roomnumber  roomnight nights"><input type="text" which="numbers" value=0 class="roomnumber roomnight numbers"></span>';
                        } else {
                            str += '<span class="nightroomgroup"  style="display:none"><input type="text"  which="night" value=1 class="roomnumber  roomnight nights"><input type="text" style="display:none" which="numbers" value=1 class="roomnumber roomnight numbers"></span>';
                        }
                        return (str);
                    }
                },
                // 额外费用
                {
                    'data': null,
                    "render": function(cellData, type, rowData, meta) {
                        if (type != "display") {
                            return null;
                        }
                        if (rowData.lhData.hasData !== true) {
                            return ("");
                        }
                        var extraServices = rowData.lhData.data.ExtraService;
                        var str = '<span class="extraTD" > ';
                        for (var k in extraServices) {
                            var extraService = extraServices[k];


                            str += '<select id="ExtraService' + extraService.ExtraServiceID + '" title="最少选' + extraService.MinNum + '项"    min="' + extraService.MinNum + '" unitpice="' + extraService.ExtraServicePrices.ServicePrice + '"  class="extraService service types">';
                            var altOption = '<option value="0" >' + extraService.ServiceName + '</option>';
                            str += altOption;
                            var max = extraService.MaxNum;
                            var min = extraService.MinNum;
                            var unit = extraService.ServiceUnit;
                            str += makeOneOption(min, max, unit);
                            str += '</select>';
                        }
                        str += '</span>';

                        function makeOneOption(min, max, unit) {
                            var str = '';
                            var MIN = min >= 1 ? min : 1;
                            for (var i = MIN; i <= max; i++) {
                                str += '<option value="' + i + '" >' + i + " " + unit + '</option>';
                            }
                            return str;
                        }
                        return (str);
                    }
                },
                {
                    'data': null,
                    "render": function(cellData, type, rowData, meta) {
                        if (type != "display") {
                            return null;
                        }
                        var str = '';
                        str +=
                            '<span class="cost">0</span>' +
                            '<span class="cancel" title="删除该产品">×</span>';
                        return (str);
                    }
                },
                {
                    'data': null,
                    "render": function(cellData, type, rowData, meta) {
                        if (type != "display") {
                            return null;
                        }
                        return ("");
                    }
                }
            ]
        });
    return neworders;
    // 数据表格结束

}

function orderPreInit() {
    // 全选
    $("body").on("change", "#selectAllOrdersPre", function() {
        if ($(this).prop("checked")) {
            $('body #orderPre .selectThisSubOrderPre').each(function() {
                $(this).prop("checked", true);
                $(this).trigger("change")
            });
        } else {
            $('body #orderPre .selectThisSubOrderPre').each(function() {
                $(this).prop("checked", false);
                $(this).trigger("change")
            });
        }
    });
    $("body").on("updateSelectAll", function() {
        if ($('body #orderPre .selectThisSubOrderPre').length === $('body #orderPre .selectThisSubOrderPre:checked').length) {
            $('body #orderPre #selectAllOrdersPre').prop("checked", true);
        } else {
            $('body #orderPre #selectAllOrdersPre').prop("checked", false);
        }
    });

    $.fn.dataTable.ext.search.push(
        function(settings, data, dataIndex) {
            if (settings.sTableId !== "order-pre-list") { //仅仅对阅览表格起作用
                return true;
            }
            var statusAndOrderNo = data[0] || "any";
            if (statusAndOrderNo === 'any') {
                return true;
            } else {
                statusAndOrderNo = JSON.parse(statusAndOrderNo);
                var haveNotOrderedOnly = $("#orderPre #haveNotOrderedOnly").prop("checked");
                var enableOnly = $("#orderPre #enableOnly").prop("checked");
                if (
                    (
                        (!enableOnly) ||
                        (
                            (statusAndOrderNo.Status !== "TRADE_CLOSED") &&
                            (statusAndOrderNo.Status !== "TRADE_CLOSED_BY_TAOBAO")
                        )
                    ) &&
                    ((!haveNotOrderedOnly) || (!statusAndOrderNo.OrderNo))

                ) {
                    return true;
                } else {
                    return false;
                }
            }
        }
    );
    var orderPreList = // KEY:row data 
        jQuery("#order-pre-list").DataTable({
            "ordering": false,
            "data": [],
            'ordering': false,
            'searching': true,
            'serverSide': false,
            'initComplete': function(settings, json) {
                var api = this.api();
                var thistable = jQuery(this);
                thistable.on('change', ".selectAllSubOrdersPre", function() {
                    if ($(this).prop("checked")) {
                        $(this).closest('table.subOrdersGroup').find('.selectThisSubOrderPre').prop("checked", true);
                        $(this).closest('table.subOrdersGroup').find('.order-pre').addClass("selected");
                    } else {
                        $(this).closest('table.subOrdersGroup').find('.selectThisSubOrderPre').prop("checked", false);
                        $(this).closest('table.subOrdersGroup').find('.order-pre').removeClass("selected");
                    }
                    $("body").trigger("updateSelectAll");
                    $("#orderPre #selecteNum").text($(this).closest('#orderPre').find('.selectThisSubOrderPre:checked').length);
                });
                thistable.on('change', ".selectThisSubOrderPre", function() {
                    if ($(this).prop("checked")) {
                        $(this).closest('.order-pre').addClass("selected");

                        var NumberChecked = $(this).closest('table.subOrdersGroup').find('.selectThisSubOrderPre:checked').length;
                        var NumberAll = $(this).closest('table.subOrdersGroup').find('.selectThisSubOrderPre').length;
                        if (NumberChecked === NumberAll) {
                            $(this).closest('table.subOrdersGroup').find('.selectAllSubOrdersPre').prop("checked", true);

                        } else {
                            $(this).closest('table.subOrdersGroup').find('.selectAllSubOrdersPre').prop("checked", false);
                        }
                    } else {
                        $(this).closest('table.subOrdersGroup').find('.selectAllSubOrdersPre').prop("checked", false);
                        $(this).closest('.order-pre').removeClass("selected");
                    }
                    $("body").trigger("updateSelectAll");
                    $("#orderPre #selecteNum").text($(this).closest('#orderPre').find('.selectThisSubOrderPre:checked').length);
                });
            },
            "drawCallback": function(settings) {
                $('body').trigger('resize');
                $("#orderPre .selectThisSubOrderPre,#orderPre .selectAllSubOrdersPre,#orderPre #selectAllOrdersPre").each(function() {
                    $(this).prop("checked", false);
                });
                $("#orderPre #selecteNum").text("0");

            },
            "createdRow": function(row, data, dataIndex) {
                var _this = this.api();
                var thisTable = this;
                $(row).addClass("outerRow");

            },
            //行操作
            'columnDefs': [{
                'targets': [0],
                'data': 'Tid',
                'render': function(cellData, type, rowData, meta) {
                    if (type != "filter") {
                        return null;
                    } else {
                        return JSON.stringify({
                            "OrderNo": rowData.OrderNo,
                            "Status": rowData.Status,
                        });
                    }


                },
                "createdCell": function(tdx, cellData, rowData, row, col) {
                    var tablepreorderinnerwapper = $('<div class="table-preorder-inner-wapper"></div>');
                    var col = $("<col></col>");
                    var cogroup = $('<colgroup></colgroup>').append(
                        col.clone().prop('width', 36),
                        col.clone().prop('width', 350),
                        col.clone().prop('width', 220),
                        col.clone().prop('width', 90),
                        col.clone().prop('width', 130),
                        col.clone().prop('width', 105),
                        col.clone().prop('width', 133),
                        col.clone().prop('width', 85)
                    );
                    var flagColor = {
                        "1": "flag-red",
                        "2": "flag-yellow",
                        "3": "flag-green",
                        "4": "flag-blue",
                        "5": "flag-purple"
                    };
                    var tradeStatus = {
                        "TRADE_NO_CREATE_PAY": "没有创建支付宝交易",
                        "WAIT_BUYER_PAY": "等待买家付款",
                        "SELLER_CONSIGNED_PART": "卖家部分发货",
                        "WAIT_SELLER_SEND_GOODS": "等待卖家发货",
                        "WAIT_BUYER_CONFIRM_GOODS": "等待买家确认收货",
                        "TRADE_FINISHED": "交易成功",
                        //  "TRADE_CLOSED": "付款以后用户退款成功，交易自动关闭",
                        "TRADE_CLOSED": "退款成功，交易关闭",
                        //  "TRADE_CLOSED_BY_TAOBAO": "付款以前，卖家或买家主动关闭交易",
                        "TRADE_CLOSED_BY_TAOBAO": "付款以前，交易关闭",
                        "PAY_PENDING": "国际信用卡支付付款确认中",
                        "WAIT_PRE_AUTH_CONFIRM": "0元购合约中"
                    };
                    var refundStatus = {
                        "RefundStatus": "买家已经申请退款，等待卖家同意",
                        "WAIT_BUYER_RETURN_GOODS": "卖家已经同意退款，等待买家退货",
                        "WAIT_SELLER_CONFIRM_GOODS": "买家已经退货，等待卖家确认收货",
                        "SELLER_REFUSE_BUYER": "卖家拒绝退款",
                        "CLOSED": "退款关闭",
                        "SUCCESS": "退款成功"
                    };
                    var i = $("<i class='fa fa-flag'></i>");
                    var a = $("<a></a>");
                    var input = $('<input/>');
                    var span = $("<span></span>");
                    var div = $("<div></div>");
                    var checkbox = $('<input type="checkbox" />');
                    var img = $('<img/>');
                    var td = $('<td></td>');
                    var tr = $("<tr></tr>");
                    var tbody = $("<tbody></tbody>");
                    var table = $("<table></table>").addClass("subOrdersGroup");
                    table.addClass("table-preorder-inner");
                    var head, bottom
                    head = tr.clone().addClass("head").append(
                        td.clone().append(
                            input.clone().prop("type", "checkbox").addClass("selectAllSubOrdersPre")
                        ),
                        td.clone().prop("colspan", 7).append(
                            div.clone().addClass("left").append(
                                span.clone().addClass("one-seg more-space-left-5").text("淘宝订单号：").append(
                                    a.clone().text(rowData.Tid)
                                ),
                                span.clone().addClass("one-seg").text("成交：" + rowData.Created),
                                span.clone().addClass("one-seg").text("付款：" + (rowData.PayTime ? rowData.PayTime : "未付款")),
                                span.clone().addClass("one-seg").text("支付宝：").append(
                                    a.clone().text(rowData.BuyerAlipayNo)
                                )
                            ),
                            div.clone().addClass("right").append(
                                span.clone().addClass("one-seg").text(rowData.OrderNo ? "系统订单号：" : "").append(
                                    rowData.OrderNo ? a.clone().text(rowData.OrderNo) : span.clone().addClass("order-no-match")
                                )
                            )
                        )
                    );
                    bottom = tr.clone().addClass("bottom").append(
                        td.clone().append(
                            (parseInt(rowData.SellerFlag) !== 0) ? i.clone().addClass(flagColor[rowData.SellerFlag]) : null
                        ),
                        td.clone().prop("colspan", 5).append(
                            div.clone().addClass("left").append(
                                span.clone().addClass("one-seg more-space-left-5").text(rowData.SellerMemo ? rowData.SellerMemo : "")
                            )

                        ),
                        td.clone().prop("colspan", 2).append(
                            div.clone().addClass("right").append(
                                span.clone().addClass("one-seg").text("实收：").append(
                                    span.clone().addClass("paid-in").text(rowData.Payment)
                                )
                            )
                        )
                    );
                    var trArr = [];
                    var counter = 0;
                    rowspan = 0;
                    var lastTd, promotionArr, promotionEachCost;
                    for (var i in rowData.Orders) {
                        promotionEachCost = 0
                        promotionArr = [];
                        for (var j in rowData.PromotionDetails) {
                            if (rowData.PromotionDetails[j].Id == rowData.Orders[i].Oid) {
                                promotionArr.push(
                                    div.clone().css('margin-bottom', "3px").append(
                                        span.clone().addClass("Badge Badge-red").text(rowData.PromotionDetails[j].PromotionName)
                                    ));
                                promotionEachCost += parseFloat(rowData.PromotionDetails[j].DiscountFee)
                            }
                        }
                        if (counter === 0) {
                            rowspan = rowData.Orders.length;
                        } else {
                            rowspan = 1;
                        }
                        if (counter === 0) {
                            lastTd = td.clone().prop('rowspan', rowspan).addClass("border-left").text(tradeStatus[rowData.Status] ? tradeStatus[rowData.Status] : "未知");
                        } else {
                            lastTd = null;
                        }
                        trArr.push(tr.clone()
                            .data({
                                'index': i,
                                "fullinfo": rowData,
                                "suborder": rowData.Orders[i],
                                "others": {
                                    "Tid": rowData.Tid,
                                    "OrderNo": rowData.OrderNo
                                },
                                "itineraryinfo": rowData.Orders[i].ItineraryInfo,

                                "code": rowData.Orders[i].OuterSkuId
                            })
                            .addClass("order-pre")
                            .append(
                                td.clone().append(
                                    checkbox.clone().addClass("selectThisSubOrderPre")
                                ), //选择
                                td.clone().addClass("iteminfo").append(div.clone().addClass("iteminfo-ct").append( //详情
                                    div.clone().addClass("img").append(img.clone().prop("src", rowData.Orders[i].PicPath)),
                                    div.clone().addClass("detail").append(
                                        div.clone().addClass("title-long").text(rowData.Orders[i].Title).attr('title', rowData.Orders[i].Title),
                                        div.clone().addClass("extra").text(rowData.Orders[i].SkuPropertiesName).attr('title', rowData.Orders[i].SkuPropertiesName)
                                    )
                                )),
                                td.clone().addClass("text-left").append( //CODE
                                    div.clone().addClass("code-container").text(rowData.Orders[i].OuterSkuId ? rowData.Orders[i].OuterSkuId : "")
                                ),
                                td.clone().addClass("text-right").append( //单价数量
                                    div.clone().text(rowData.Orders[i].Price),
                                    div.clone().text("×" + rowData.Orders[i].Num)
                                ),
                                td.clone().append( //支付状态
                                    div.clone().text(tradeStatus[rowData.Orders[i].Status] ? tradeStatus[rowData.Orders[i].Status] : ""),
                                    div.clone().text(refundStatus[rowData.Orders[i].RefundStatus] ? refundStatus[rowData.Orders[i].RefundStatus] : "")
                                ),
                                td.clone().append( //优惠
                                    div.clone().text("-" + promotionEachCost),
                                    promotionArr
                                ),
                                td.clone().append( //实付金额
                                    div.clone().text(rowData.Orders[i].Payment)
                                ),
                                lastTd
                            ));
                        counter++
                    }
                    $(tdx).empty().append(
                        tablepreorderinnerwapper.append(
                            table.append(
                                cogroup,
                                tbody.append(
                                    head,
                                    trArr,
                                    bottom
                                )
                            )
                        )
                    );

                }
            }]
        });
    return orderPreList;
}

function init() {

}

function orderManInit() {
    var isSelectbyEnter = false;
    $('#typeahead').typeahead({
        hint: false,
        highlight: true,
        minLength: 1,
    }, {
        name: 'xxx',
        displayKey: 'name',
        limit: 30,
        source: searchEngineInit(),
        templates: {
            empty: [
                '<div class="empty-message">',
                '没有找到相关产品',
                '</div>'
            ].join('\n'),
            pending: [
                '<div class="empty-message">',
                '正在搜索...',
                '</div>'
            ].join('\n'),
            header: function(data) {
                return ([
                    '<div class="empty-message">',
                    '共搜索到<strong>' + data.suggestions.length + '</strong>个产品',
                    '</div>'
                ].join('\n'));
            },
            suggestion: Handlebars.compile('<div>{{name}}{{serviceCode}}</div>')
        }
    });

    //回车
    jQuery('#typeahead').bind("keydown", function(evt) {
        evt = (evt) ? evt : ((window.event) ? window.event : "") //兼容IE和Firefox获得keyBoardEvent对象  
        var key = evt.keyCode ? evt.keyCode : evt.which; //兼容IE和Firefox获得keyBoardEvent对象的键值  
        if (key === 13) {
            if (!isSelectbyEnter) {
                jQuery("#addOneItem").trigger("click");
            } else {
                isSelectbyEnter = false;
            }

        } else if (key !== 9) {
            if (($(this).data('which'))) {
                $('#suppliers').empty().text("");
                $(this).data('which', "");
                $(this).closest('.form-group').find('#serviceCode').text("");
            }
        }
    });
    $('#typeahead').bind('typeahead:select', function(ev, suggestion) {
        isSelectbyEnter = true;
        $(this).data('which', suggestion.serviceItemID);
        var enName = ' - ';
        if (suggestion.enName) {
            enName += suggestion.enName;
        }
        $(this).closest('.form-group').find('#serviceCode').text(suggestion.serviceCode + enName);

        // 供应商更改
        var supplier = suggestion.supplyer;
        var optGroupStr = '';
        for (var i in supplier) {
            optGroupStr +=
                makeOneOption(supplier[i], suggestion.defaultSupplierID);
        }
        $('#suppliers').empty().append(optGroupStr);

        function makeOneOption(obj, defaultSupplierID) {
            if (obj.SupplierID == defaultSupplierID) {
                var selected = 'selected="selected"';
                var altName = obj.SupplierNo + '-' + obj.SupplierName + "(默认)";
            } else {
                var selected = '';
                var altName = obj.SupplierNo + '-' + obj.SupplierName;

            }
            var str = "";
            str = '<option value="' + obj.SupplierID + '"    ' + selected + '>' + altName + '</option>';
            return str;
        }
    }).bind('typeahead:change', function() {

    });

    jQuery('#addOneItem').bind("click", function() {
        var TBORDERID = jQuery.trim(jQuery("#TBORDERID").val());
        var TBORDERIDReg = /^[a-zA-Z0-9]{5,}$/;
        if (!(TBORDERIDReg.test(TBORDERID))) {
            if (TBORDERID.length < 5) {
                jQuery("#TBORDERID").formWarning({
                    tips: "亲，淘宝订单号起码长度至少5位"
                });
            } else {
                jQuery("#TBORDERID").formWarning({
                    tips: "亲，淘宝订单号不应该含有字母和数字之外的字符，中间也不要含有空格"
                });
            }

            return;
        }
        if (!jQuery('#typeahead').data('which') && !jQuery('#suppliers').val()) {
            $(this).success("亲，选择产品和供应商");
            return;
        }
        if (jQuery('table#itemListNewOrder').DataTable().data().count() >= 10) {
            $.LangHua.alert({
                title: "提示信息",
                tip1: '温馨提示',
                tip2: '最多一次添加10个产品',
                button: '确定'
            })
            return;
        }
        $.ajax({
            url: '/Orders/GetItemByID',
            type: 'post',
            dataType: 'json',
            data: {
                ItemID: jQuery('#typeahead').data('which'),
                SupplierID: jQuery('#suppliers').val()
            },
            success: function(data) {
                if (data.ErrorCode != 200) {
                    return;
                }
                var again = false;
                var rows = jQuery('table#itemListNewOrder').find('tr.itemRow');
                for (var i = 0; i < rows.length; i++) {
                    if (jQuery(rows[i]).attr("id") == data['Item']['ServiceItemID']) {
                        again = true;
                        $.LangHua.confirm({
                            title: "提示信息",
                            tip1: '请确认是否需要<span class="red">重复</span>添加下列产品？',
                            tip2: data['Item']['cnItemName'],
                            confirmbutton: '确定',
                            cancelbutton: '取消',
                            data: null,
                            confirm: function() {
                                var objtemp = {};
                                objtemp[$.trim($("#TBORDERID").val())] = [];
                                $('#itemListNewOrder').trigger("addItems", [
                                    [{
                                        "tmallData": { "hasData": false, subOrderList: {} },
                                        "lhData": {
                                            'hasData': true,
                                            "data": data.Item
                                        },
                                        "TBOrderID": objtemp,
                                        "ServiceCode": data.Item.ServiceCode
                                    }]
                                ]);
                            }
                        })
                        break;
                    }
                }
                if (rows.length == 0 || !again) {
                    var objtemp = {};
                    objtemp[$.trim($("#TBORDERID").val())] = [];
                    $('#itemListNewOrder').trigger("addItems", [
                        [{
                            "tmallData": { "hasData": false, subOrderList: {} },
                            "lhData": {
                                'hasData': true,
                                "data": data.Item
                            },
                            "TBOrderID": objtemp,
                            "ServiceCode": data.Item.ServiceCode
                        }]
                    ]);
                }
            }
        })
    });
}

function initOrderRevise() {
    var isSelectbyEnter = false;
    $('#typeaheadR').typeahead({
        hint: false,
        highlight: true,
        minLength: 1,
    }, {
        name: 'xxx',
        displayKey: 'name',
        limit: 30,
        source: searchEngineInit(),
        templates: {
            empty: [
                '<div class="empty-message">',
                '没有找到相关产品',
                '</div>'
            ].join('\n'),
            pending: [
                '<div class="empty-message">',
                '正在搜索...',
                '</div>'
            ].join('\n'),
            header: function(data) {
                return ([
                    '<div class="empty-message">',
                    '共搜索到<strong>' + data.suggestions.length + '</strong>个产品',
                    '</div>'
                ].join('\n'));
            },
            suggestion: Handlebars.compile('<div id="serviceItemID{{serviceItemID}}">{{name}}{{serviceCode}}</div>')
        }
    });

    //回车
    jQuery('#typeaheadR').bind("keydown", function(evt) {
        evt = (evt) ? evt : ((window.event) ? window.event : "") //兼容IE和Firefox获得keyBoardEvent对象  
        var key = evt.keyCode ? evt.keyCode : evt.which; //兼容IE和Firefox获得keyBoardEvent对象的键值  
        if (key === 13) {
            if (!isSelectbyEnter) {
                jQuery("#addOneItemR").trigger("click");
            } else {
                isSelectbyEnter = false;
            }

        } else if (key !== 9) {
            if (($(this).data('which'))) {
                $('#suppliersR').empty().text("");
                $(this).data('which', "");
                $(this).closest('.form-group').find('#serviceCodeR').text("");
            }
        }
        $("body").trigger("resize");

    });
    $('#typeaheadR').bind('typeahead:select', function(ev, suggestion) {
        isSelectbyEnter = true;
        $(this).data('which', suggestion.serviceItemID);
        var enName = ' - ';
        if (suggestion.enName) {
            enName += suggestion.enName;
        }
        $(this).closest('.form-group').find('#serviceCodeR').text(suggestion.serviceCode + enName);

        // 供应商更改
        var supplier = suggestion.supplyer;
        var optGroupStr = '';
        for (var i in supplier) {
            optGroupStr +=
                makeOneOption(supplier[i], suggestion.defaultSupplierID);
        }
        $('#suppliersR').empty().append(optGroupStr);
        $("body").trigger("resize");

        function makeOneOption(obj, defaultSupplierID) {
            if (obj.SupplierID == defaultSupplierID) {
                var selected = '';
                var altName = obj.SupplierNo + '-' + obj.SupplierName + "(默认)";
            } else {
                var selected = '';
                var altName = obj.SupplierNo + '-' + obj.SupplierName;

            }
            var str = "";
            str = '<option value="' + obj.SupplierID + '"    ' + selected + '>' + altName + '</option>';
            return str;
        }
    }).bind('typeahead:change', function() {

    });
}