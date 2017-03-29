jQuery(document).ready(function($) {

    var supplier = new Bloodhound({
        datumTokenizer: function(d) {
            return Bloodhound.tokenizers.whitespace(d.name);
        },
        queryTokenizer: Bloodhound.tokenizers.whitespace,
        limit: 15,
        remote: {
            initialize: false,
            wildcard: '%QUERY',
            url: '/Orders/GetItemsByStr',
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
                        supplyer: country.ItemSupliers,
                        defaultSupplierID: country.DefaultSupplierID,
                        serviceItemID: country.ServiceItemID,
                        ServiceTypeID: country.ServiceTypeID,
                        serviceCode: country.ServiceCode
                    };
                });
            }
        }
    });

    supplier.initialize();
    $('#serviceItem-new')
        .typeahead({
            hint: false,
            highlight: true,
            minLength: 1
        }, {
            name: '',
            displayKey: 'name',
            limit: 30,
            source: supplier,
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
        })
        .bind('typeahead:select', function(ev, suggestion) {

            $(this).data('serviceItemID', suggestion.serviceItemID);
            $(this).data('ServiceTypeID', suggestion.ServiceTypeID);
            $(this).data('changetype', 'byChoose');
            $(this).blur();

            $(this).closest("#order-clone-modal").find('#errorTips').text("");

            var itemServiceTypeOld = $(this).closest("#order-clone-modal").find('#itemServiceType').text();
            if (suggestion.ServiceTypeID != itemServiceTypeOld) {
                $(this).closest(".col-md-10").find('span#errorTips').text('产品类型不同！').removeClass("hidden");
            } else {
                $(this).closest(".col-md-10").find('span#errorTips').text('').addClass("hidden");
            }




            // 供应商更改
            var thisServiceType = $(this).closest('#itemServiceType').text();
            var supplier = suggestion.supplyer;
            var optGroupStr = '';
            for (var i in supplier) {
                optGroupStr +=
                    makeOneOption(supplier[i], suggestion.defaultSupplierID, thisServiceType);
            }
            $('#supplier-new').empty().append(optGroupStr);

            function makeOneOption(obj, defaultSupplierID, thatServiceTypeID) {
                if (obj.SupplierID == defaultSupplierID) {
                    var selected = 'selected="selected"';
                    var altName = obj.SupplierNo + '-' + obj.SupplierName + "(默认)";
                } else {
                    var selected = '';
                    var altName = obj.SupplierNo + '-' + obj.SupplierName;
                }
                var str = "";
                str = '<option suppliercode= "' + obj.SupplierNo + '" value="' + obj.SupplierID + '"    ' + selected + '>' + altName + '</option>';
                return str;
            }
        })
        .bind('typeahead:change', function() {
            var chooseType = $(this).data("changetype");
            if (chooseType == 'byChoose') {
                $(this).data("changetype", 'others');
            } else {
                $(this).closest('#order-clone-modal').find('#supplier-new').empty();
                $(this).closest('#order-clone-modal').find('#serviceItem-new').data({ serviceItemID: "", ServiceTypeID: "" });


                $(this).closest('#order-clone-modal').find('span#errorTips').text("请选择产品").removeClass('hidden');
            }

        }).bind('focus', function() {

            $(this).closest('#order-clone-modal').find('span#errorTips').addClass('hidden');

        })
        .bind('blur', function() {
            var text = $(this).closest('#order-clone-modal').find('span#errorTips').text();
            if (text) {
                $(this).closest('#order-clone-modal').find('span#errorTips').removeClass('hidden');
            }
        })





    $("body").on("show", "#order-clone-modal", function() {
        $(this).find('#chooseCloningItem').show();
        $(this).find('#mapcloningForm').hide();
        $(this).find('#serviceItem-new').typeahead('val', '');
        $(this).find('span#supplier-new').text("");
        $(this).find('#supplier-new').empty();
        $(this).find('#errorTips').text("");
        $(this).find('#clicktoMapFORM').show();
        $(this).find('#clickToClone,#preStep').hide();
        $(this).find('#serviceItem-new').data('serviceItemID', '');


        $(this).find('#chooseCloningItem').show().animateCssTime('slideInDown', 'animate200');
        $(this).find('#tips-fixed').animateCssTime('slideInLeft', 'animate500');


        $('#clicktoMapFORM').one('click', function toMapForm() {
            var thisbutton = $(this);
            var cloningItem = thisbutton.closest("#order-clone-modal").find('#serviceItem-new').data();
            var clonedItemServiceType = thisbutton.closest("#order-clone-modal").find('#itemServiceType').text();

            if (!cloningItem.serviceItemID) {
                thisbutton.one("click", toMapForm);
                thisbutton.closest("#order-clone-modal").find('#errorTips').text("请选择产品！！");
                return;
            }
            if (cloningItem.ServiceTypeID != clonedItemServiceType) {
                thisbutton.one("click", toMapForm);
                thisbutton.closest("#order-clone-modal").find('#errorTips').text("产品类型不同,请选择同类型产品！！");
                return;
            }
            $.ajax({
                url: '/ServiceItems/GetItemByID/' + cloningItem.serviceItemID,
                type: 'get',
                dataType: 'json',
                success: function(data) {
                    thisbutton.closest("#order-clone-modal").find('#chooseCloningItem').hide();
                    var table = thisbutton.closest("#order-clone-modal").find('#mapcloningForm').show().find('table');
                    table.animateCssTime('slideInUp', 'animate200');
                    table.find("tbody:eq(0)").data("newelements", JSON.parse(data.ElementContent).elementList);
                    table.find("#cloningItemName").text("（" + thisbutton.closest("#order-clone-modal").find('#supplier-new option:selected').attr('suppliercode') + "）" + data.cnItemName + data.ServiceCode);
                    var theSame = makeSelect(JSON.parse(data.ElementContent).elementList);
                    thisbutton.closest("#order-clone-modal").find('td.map').empty().append(theSame);

                    thisbutton.hide().siblings('#clickToClone,#preStep').show();
                    thisbutton.trigger('autoChoose', ['#mappingTable']);
                },
                complete: function() {
                    thisbutton.one("click", toMapForm);
                }
            })
        })
    })

    $("body").on("hide", "#order-clone-modal", function() {
        $('#clicktoMapFORM').off('click');
    })

    $("body").on("click", "#preStep", function() {
        $(this).hide().siblings('#clicktoMapFORM').show();
        $(this).siblings('#clickToClone').hide();
        $(this).closest("#order-clone-modal").find('#mapcloningForm').hide()
        $(this).closest("#order-clone-modal").find('#chooseCloningItem').show().animateCssTime('slideInDown', 'animate200');
        $(this).closest("#order-clone-modal").find('#tips-fixed').animateCssTime('slideInLeft', 'animate500');

    })


    $('#clicktoMapFORM').on('autoChoose', function(e, data) {
        $(data).find('tbody tr').each(function() {
            var old = new Object();
            var newList = new Object();
            var matchRateList = new Object();


            old.type = $(this).find('td:eq(0)').data('type');
            old.codelist = new Array();
            var oldElements = JSON.parse($(this).find('td:eq(0)').attr('data-elements'));
            for (var i in oldElements) {
                old.codelist.push(oldElements[i]['code']);
            }
            old.codelist.sort(); //
            var oldstr = old.codelist.join("@@@");
            var oldlen = old.codelist.length;


            $(this).find('td.map select option').each(function(index) {

                var newOne = new Object();
                var newOneInfo = $(this).data();
                newOne.ID = $(this).attr('value');
                newOne.type = newOneInfo.type;
                newOne.codelist = new Array();
                matchRateList[newOne.ID] = -1;
                for (var i in newOneInfo.elements) {
                    newOne.codelist.push(newOneInfo.elements[i].code);
                }
                newOne.codelist.sort();
                newList[$(this).attr('value')] = newOne;

                if (newOne.type == old.type) {
                    matchRateList[newOne.ID]++;
                    var newstr = newOne.codelist.join('@@@');
                    if (newstr.match(oldstr) !== null) {
                        matchRateList[newOne.ID] += 3 * oldlen;
                    } else {
                        for (var j in old.codelist) {
                            if (newstr.match(old.codelist[j]) !== null) {
                                matchRateList[newOne.ID]++;
                            }
                        }

                        for (var j in newOne.codelist) {
                            if (oldstr.match(newOne.codelist[j]) !== null) {
                                matchRateList[newOne.ID]++;
                            }
                        }
                    }

                }
            })


            var maxEachRow = -1;
            var choosed = -1;
            for (var i in matchRateList) {
                if (matchRateList[i] > maxEachRow) {
                    maxEachRow = matchRateList[i];
                    choosed = i;
                }
            }
            $(this).find('td.map select').val(choosed)





        })
    })

    function makeSelect(arr) {
        var anOption = $('<option></option>');
        var aSelect = $('<select></select>').addClass("form-control");
        aSelect.append(
            anOption.clone()
            .text('不复制')
            .attr('value', '-1')
            .data({
                'type': 'anonymous',
                'increaseid': '-1',
                'elements': ""
            })
        );
        for (var i in arr) {
            aSelect.append(
                anOption.clone()
                .text(arr[i].text)
                .attr('value', i)
                .data({
                    'type': arr[i].type,
                    'increaseid': i,
                    'elements': arr[i].elements,
                })
            );
        }
        return aSelect;
    }


    $('#clickToClone').one('click', function toMapForm() {
        var thisbutton = $(this);
        var thisTable = thisbutton.closest("#order-clone-modal").find('#mappingTable');


        var checkUniqe = new Object();
        var cancel = false;
        thisTable.find(".map select").each(function() {
            var id = $.trim($(this).val());
            if (id == -1) {
                return;
            }
            if (id in checkUniqe) {
                cancel = true;
                $(this).Warning({
                    title: "重复选择！",
                    placement: "right"
                });
            } else {
                $(this).WarningDelete();
                var obj = new Object();
                checkUniqe[id] = $(this);
            }
        });
        if (cancel) {
            thisbutton.one('click', toMapForm);
            return;
        }

        // 没有填写过表单
        var hasEdit = thisTable.find('tbody:eq(0)').attr("data-edit");

        var post = new Object();
        post.OrderID = $('#OrderID').text();
        post.ItemID = $("#serviceItem-new").data('serviceItemID');
        post.SupplierID = $("#supplier-new").val();

        if (hasEdit == 'no') {
            //value 为 null;
        } else {
            var valueOBJ = new Object();
            var newElements = thisTable.find('tbody:eq(0)').data("newelements");
            for (var i in newElements) {
                var selfOption = thisTable.find('option:selected[value=' + i + ']');

                if (selfOption.length == 0) { //没有复制
                    valueOBJ[i] = 'emptyrow';
                } else {
                    var olddata = selfOption.closest("tr").find("td:eq()").data();
                    var oldvalue = selfOption.closest("tr").find("td:eq()").attr("data-elementsvalue");
                    if (olddata.type != newElements[i].type) { //类型不同
                        valueOBJ[i] = 'emptyrow';
                        continue;
                    } else {
                        valueOBJ[i] = new Object();
                        for (var j in newElements[i].elements) {
                            if (olddata.elementsvalue) {
                                if (olddata.elementsvalue[olddata.elements[j].code]) {
                                    valueOBJ[i][newElements[i].elements[j].code] = olddata.elementsvalue[olddata.elements[j].code]
                                } else {
                                    valueOBJ[i] = 'emptyrow';
                                    break;
                                }
                            } else {
                                valueOBJ[i] = 'emptyrow';
                                break;
                            }
                            // if (!(olddata.elementsvalue[olddata.elements[j].code])) { //多次复制先不复制，在复制某个字段
                            //     valueOBJ[i] = 'emptyrow';
                            //     break;
                            // } else {
                            //     if (olddata.elementsvalue) {
                            //         valueOBJ[i][newElements[i].elements[j].code] = olddata.elementsvalue[olddata.elements[j].code]
                            //     }
                            // }

                        }
                    }
                }
            }
            post.ElementsValue = JSON.stringify(valueOBJ);
        }
        var loading = '';
        $.ajax({
            url: '/Orders/ChangeProduct',
            type: "post",
            contentType: "application/json;charset=utf-8;",
            data: JSON.stringify(post),
            dataType: 'json',
            beforeSend: function() {
                loading = $.LangHua.loadingToast({
                    tip: '正 在 复 制 . . . . . .'
                })
            },
            success: function(data) {
                loading.modal("hide");
                if (data.ErrorCode == 200) {
                    var tmallOrders = $("#order-clone-modal").data("tmallorders");

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
                                        var tmallOrders = $("#order-clone-modal").data("tmallorders");
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
                        window.location.href = "/Orders/Edit/" + data.OrderID;
                    }, 300);
                } else if (data.ErrorCode == 401) {
                    $.LangHua.alert({
                        title: "提示信息",
                        tip1: '复制结果',
                        tip2: data.ErrorMessage,
                        button: '确定',
                        icon: "warning",
                    });
                    thisbutton.one('click', toMapForm);
                } else {
                    $.LangHua.alert({
                        title: "提示信息",
                        tip1: '复制结果',
                        tip2: '复制失败！',
                        button: '确定',
                        icon: "warning",
                    });
                    thisbutton.one('click', toMapForm);
                }
            },
            error: function() {
                loading.modal("hide");
                thisbutton.one('click', toMapForm);
            }
        })
    })
});