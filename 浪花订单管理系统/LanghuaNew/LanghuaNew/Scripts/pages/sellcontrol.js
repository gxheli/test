jQuery(document).ready(function() {

    jQuery("#SellControl #btnAdd").bind("click", function() {
        if (!jQuery('#ServiceItems').data('which')) {
            jQuery('#ServiceItems').warning("请选中产品");
            return;
        }
        if (jQuery('#SellControl #classify').val() === "unset") {
            jQuery('#SellControl #classify').formWarning({ "tips": "请您选择分类" });
            return;
        }
        var itemid = jQuery('#ServiceItems').data('which');
        var supplierid = jQuery("#supplier").val();
        var name = jQuery('#ServiceItems').data('name');
        var SupplierNo = jQuery("#supplier option:selected").data('supplierno');
        var classID = jQuery('#SellControl #classify').val();
        var classText = jQuery('#SellControl #classify option:selected').text();
        var list = jQuery("#SellControl #itemlist option");
        for (var i = 0; i < list.length; i++) {
            if (itemid == jQuery(list[i]).data('itemid') && supplierid == jQuery(list[i]).data('supplierid')) {
                $.LangHua.alert({
                    title: "提示信息",
                    tip1: '产品已重复：',
                    tip2: "(" + SupplierNo + ")" + name,
                    button: '确定',
                });
                return;
            }
        }
        var allOptions = $('#SellControl #itemlist option');
        var theSameClassOptions = $('#itemlist option[data-classifyid=' + classID + ']');
        if (theSameClassOptions.length === 0) {
            var newOption = document.createElement('option');
            newOption.setAttribute('data-itemid', itemid)
            newOption.setAttribute('data-supplierid', supplierid);
            newOption.setAttribute('data-supplierid', supplierid);
            newOption.setAttribute('data-classifyid', classID);
            newOption.setAttribute('data-classtext', classText);
            var newContent = document.createTextNode("【" + classText + "】" + "(" + SupplierNo + ")" + name);
            newOption.appendChild(newContent);
            jQuery("#itemlist").append(newOption);
        } else {
            var theLastSameClassOption = $('#itemlist option[data-classifyid=' + classID + ']:eq(' + (theSameClassOptions.length - 1) + ')');
            var indexStart = parseInt(allOptions.index(theLastSameClassOption)) + 1;
            var indexEnd = allOptions.length - 1;
            var newOption = document.createElement('option');
            jQuery("#SellControl #itemlist").append(newOption);
            allOptions = $('#SellControl #itemlist option');
            for (i = indexEnd; i >= indexStart; i--) {
                allOptions.eq(i + 1).data('itemid', allOptions.eq(i).data("itemid"));
                allOptions.eq(i + 1).data('supplierid', allOptions.eq(i).data("supplierid"));
                allOptions.eq(i + 1).data('classifyid', allOptions.eq(i).data("classifyid"));
                allOptions.eq(i + 1).data('classtext', allOptions.eq(i).data("classtext"));
                allOptions.eq(i + 1).text(allOptions.eq(i).text());
            }
            allOptions.eq(i + 1).data('itemid', itemid);
            allOptions.eq(i + 1).data('supplierid', supplierid);
            allOptions.eq(i + 1).data('classifyid', classID);
            allOptions.eq(i + 1).data('classtext', classText);
            allOptions.eq(i + 1).text("【" + classText + "】" + "(" + SupplierNo + ")" + name);
        }
        $('#SellControl').find("#ServiceItems").typeahead('val', '');
        $('#SellControl').find("#supplier").empty().text("");
        $('#SellControl').find("#classify").val('unset');
    });

    jQuery("#SellControl #btnSave").bind("click", function() {
        var sells = [];
        var list = jQuery("#SellControl #itemlist option");
        for (i = 0; i < list.length; i++) {
            var sell = {
                SupplierID: jQuery(list[i]).data('supplierid'),
                RowNum: i + 1,
                SellControlClassifyID: jQuery(list[i]).data('classifyid'),
                FirstServiceItem: {
                    ServiceItemID: jQuery(list[i]).data('itemid')
                }
            }
            sells.push(sell);
        }
        if (sells.length == 0) {
            jQuery("#btnSave").success("请先添加控位产品");
            return;
        }
        $.ajax({
            type: 'post',
            dataType: 'json',
            contentType: "application/json; charset=utf-8;",
            data: JSON.stringify(sells),
            url: '/SellControls/SaveSellControl',
            beforeSend: function() {
                $.LangHua.loadingToast({
                    tip: "正 在 保 存 . . . . . ."
                });
            },
            success: function(data) {

                if (data.ErrorCode == 200) {
                    jQuery("#btnSave").success("保存成功");
                    window.location.href = "/SellControls";
                } else if (data.ErrorCode == 401) {
                    $.LangHua.alert({
                        title: "提示信息",
                        tip1: '保存失败',
                        tip2: data.ErrorMessage,
                    })
                } else {
                    $.LangHua.alert({
                        title: "提示信息",
                        tip1: '保存失败',
                        tip2: '保存失败，请您重试！',
                    })
                }
            },
            error: function() {
                var openModals = $("body").data("modalmanager").getOpenModals();
                if (openModals) {
                    for (var i in openModals) {
                        if ($(openModals[i]['$element'][0]).attr("id") !== "SellControl") {
                            $(openModals[i]['$element'][0]).modal("hide");
                        }
                    }
                }
                $.LangHua.alert({
                    title: "提示信息",
                    tip1: '保存失败',
                    tip2: '保存失败，请您检查网络并重试！',
                })
            }
        });
    });
    jQuery("#SellControl #btnPre").bind("click", function() {
        var obj = document.getElementById('itemlist')
        for (var i = 1; i < obj.length; i++) { //最上面的一个不需要移动，所以直接从i=1开始  
            if (obj.options[i].selected) {
                if (!obj.options.item(i - 1).selected) {
                    var selText = obj.options[i].text;
                    var selValue = obj.options[i].value;
                    var itemid = jQuery(obj.options[i]).data('itemid');
                    var supplierid = jQuery(obj.options[i]).data('supplierid');
                    var classID = jQuery(obj.options[i]).data('classifyid');
                    obj.options[i].text = obj.options[i - 1].text;
                    obj.options[i].value = obj.options[i - 1].value;
                    jQuery(obj.options[i]).data('itemid', jQuery(obj.options[i - 1]).data('itemid'));
                    jQuery(obj.options[i]).data('supplierid', jQuery(obj.options[i - 1]).data('supplierid'));
                    jQuery(obj.options[i]).data('classifyid', jQuery(obj.options[i - 1]).data('classifyid'));
                    obj.options[i].selected = false;
                    obj.options[i - 1].text = selText;
                    obj.options[i - 1].value = selValue;
                    jQuery(obj.options[i - 1]).data('itemid', itemid);
                    jQuery(obj.options[i - 1]).data('supplierid', supplierid);
                    jQuery(obj.options[i - 1]).data('classifyid', classID);
                    obj.options[i - 1].selected = true;
                }
            }
        }
    });
    jQuery("#SellControl #btnNext").bind("click", function() {
        var obj = document.getElementById('itemlist')
        for (var i = obj.length - 2; i >= 0; i--) { //向下移动，最后一个不需要处理，所以直接从倒数第二个开始  
            if (obj.options[i].selected) {
                if (!obj.options[i + 1].selected) {
                    var selText = obj.options[i].text;
                    var selValue = obj.options[i].value;
                    var itemid = jQuery(obj.options[i]).data('itemid');
                    var supplierid = jQuery(obj.options[i]).data('supplierid');
                    var classID = jQuery(obj.options[i]).data('classifyid');
                    obj.options[i].text = obj.options[i + 1].text;
                    obj.options[i].value = obj.options[i + 1].value;
                    jQuery(obj.options[i]).data('itemid', jQuery(obj.options[i + 1]).data('itemid'));
                    jQuery(obj.options[i]).data('supplierid', jQuery(obj.options[i + 1]).data('supplierid'));
                    jQuery(obj.options[i]).data('classifyid', jQuery(obj.options[i + 1]).data('classifyid'));
                    obj.options[i].selected = false;
                    obj.options[i + 1].text = selText;
                    obj.options[i + 1].value = selValue;
                    jQuery(obj.options[i + 1]).data('itemid', itemid);
                    jQuery(obj.options[i + 1]).data('supplierid', supplierid);
                    jQuery(obj.options[i + 1]).data('classifyid', classID);
                    obj.options[i + 1].selected = true;
                }
            }
        }
    });
    jQuery("#SellControl #btnDel").bind("click", function() {
        jQuery("#itemlist option:selected").remove();
    });
    jQuery("#SellControl #btnChange").bind("click", function() {
        var itemSeleted = jQuery("#SellControl #itemlist option:selected");
        if (itemSeleted.length === 0) {
            return;
        }
        var itemID = itemSeleted.data("itemid");
        var classList = $('#SellControl #classify').data("classlist");
        var classText = itemSeleted.data("classtext");
        var classIDCurrent = itemSeleted.eq(0).data("classifyid");
        var arrClassName = classList;
        var i, optionHref, arrOption = [];
        var option = $("<option></option>");
        arrOption.push(
            option.clone()
            .text("请选择")
            .attr("id", "unset")
            .val("unset")
        );
        for (i in arrClassName) {
            if (classIDCurrent == arrClassName[i].SellControlClassifyID) {
                continue;
            }
            arrOption.push(
                option.clone()
                .text(arrClassName[i].ClassName)
                .attr("id", arrClassName[i].SellControlClassifyID)
                .val(arrClassName[i].SellControlClassifyID)
            )
        }
        $("#classMoveTo #classNameCurrent").text(classText).data("itemid", itemID);
        $("#classMoveTo #classify").empty().append(arrOption);
        jQuery("#classMoveTo").modal("show");
    });
    $("body").on('show.bs.modal', "#SellControl", function() {
        $('#SellControl').find("#ServiceItems").typeahead('val', '');
        $('#SellControl').find("#supplier").empty().text("");
        $('#SellControl').find("#classify").val('unset');
    });
    typeAhead();

    function typeAhead() {
        var remote_cities = new Bloodhound({
            datumTokenizer: function(d) {
                return Bloodhound.tokenizers.whitespace(d.name);
            },
            queryTokenizer: Bloodhound.tokenizers.whitespace,
            limit: 15,
            // 在文本框输入字符时才发起请求
            // 
            // local:dt,
            remote: {
                wildcard: '%QUERY',
                url: '/Orders/GetItemsByStr',
                prepare: function(xhr, settings) {
                    settings.dataType = 'json';
                    settings.type = 'POST';
                    settings.data = { Str: xhr };
                    return settings;
                },
                filter: function(data) {
                    return $.map(data.Items, function(one) {
                        return {
                            name: one.cnItemName,
                            supplyer: one.ItemSupliers,
                            defaultSupplierID: one.DefaultSupplierID,
                            serviceItemID: one.ServiceItemID,
                            code: one.ServiceCode,
                        };
                    });
                }

            }
        });

        remote_cities.initialize();

        $('#ServiceItems').typeahead({
            hint: false,
            highlight: true,
            minLength: 1,
        }, {
            name: 'xxx',
            displayKey: "name",
            limit: 30,
            source: remote_cities,
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
                suggestion: Handlebars.compile('<div>{{name}}{{code}}</div>')
            }
        }).bind('typeahead:select', function(ev, suggestion) {
            $(this).data('which', suggestion.serviceItemID);
            $(this).data('name', suggestion.name + suggestion.code);

            // 供应商更改
            var supplier = suggestion.supplyer;
            var optGroupStr = '';
            for (var i in supplier) {
                optGroupStr +=
                    makeOneOption(supplier[i], suggestion.defaultSupplierID);
            }
            $('#supplier').empty().append(optGroupStr);

            function makeOneOption(obj, defaultSupplierID) {
                if (obj.SupplierID == defaultSupplierID) {
                    var selected = 'selected="selected"';
                    var altName = obj.SupplierNo + '-' + obj.SupplierName + "(默认)";
                } else {
                    var selected = '';
                    var altName = obj.SupplierNo + '-' + obj.SupplierName;
                }
                var str = "";
                str = '<option value="' + obj.SupplierID + '" ' + selected + '  data-supplierno="' + obj.SupplierNo + '"   >' + altName + '</option>';
                return str;
            }

        })

        $('#SetProduct #SecondItemName').typeahead({
            hint: false,
            highlight: true,
            minLength: 1,
        }, {
            name: 'xxx',
            display: function(data) {
                return data.name //+" "+ data.code;
            },
            limit: 15,
            source: remote_cities,
            templates: {
                empty: [
                    '<div class="empty-message">',
                    '没有找到相关数据',
                    '</div>'
                ].join('\n'),
                header: [
                    '<div class="empty-message">',
                    '搜索到的数据',
                    '</div>'
                ].join('\n'),
                pending: [
                    '<div class="empty-message">',
                    '正在载入数据....',
                    '</div>'
                ].join(''),
                footer: [

                ].join('\n'),
            }
        }).bind('typeahead:select', function(ev, suggestion) {
            $(this).data('which', suggestion.serviceItemID);
            $(this).data('code', suggestion.code);
        });

    }
    classify();

    function classify() {
        $.ajax({
            type: 'get',
            dataType: 'json',
            contentType: "application/json; charset=utf-8;",
            url: '/SellControls/GetSellControlClassities',
            success: function(data) {
                if (data.ErrorCode == 200) {
                    $("#SellControl #classify").empty().trigger("update", [data.data]);
                }
            }
        });
        $("#SellControl #classify").on("update", function(e, data) {
            $(this).data("classlist", data);
            var arrClassName = data;
            var i, optionHref, arrOption = [];
            var option = $("<option></option>");
            arrOption.push(
                option.clone()
                .text("请选择")
                .attr("id", "unset")
                .val("unset")
            );
            for (i in arrClassName) {
                arrOption.push(
                    option.clone()
                    .text(arrClassName[i].ClassName)
                    .attr("id", arrClassName[i].SellControlClassifyID)
                    .val(arrClassName[i].SellControlClassifyID)
                )
            }
            $(this).empty().append(arrOption);
        });

        $("#SellControl #editClasses").on("click", function() {
            var classList = $(this).closest("#SellControl").find("#classify").data("classlist");
            $("#classEdit").data("classlist", classList);
            $("#classEdit").modal("show");
        });
        $('body').on('show.bs.modal', '#classEdit', function() {
            var arrClassName = $(this).data("classlist");
            var i, optionHref, arrOption = [];
            var option = $("<option></option>");
            for (i in arrClassName) {
                arrOption.push(
                    option.clone()
                    .text(arrClassName[i].ClassName)
                    .attr("id", arrClassName[i].SellControlClassifyID)
                )
            };
            $(this).find('#classList').empty().append(arrOption);
            $(this).find('#classNew').val("");
        });
        $("#classEdit").on("click", "#addClass", function() {
            var classNewJq = $(this).closest("#classEdit").find("#classNew");
            var classNew = $.trim(classNewJq.val());

            if (classNew !== "") {
                var length =
                    (function(str, charset) {
                        var total = 0,
                            charCode,
                            i,
                            len;
                        charset = charset ? charset.toLowerCase() : '';
                        if (charset === 'utf-16' || charset === 'utf16') {
                            for (i = 0, len = str.length; i < len; i++) {
                                charCode = str.charCodeAt(i);
                                if (charCode <= 0xffff) {
                                    total += 2;
                                } else {
                                    total += 4;
                                }
                            }
                        } else {
                            for (i = 0, len = str.length; i < len; i++) {
                                charCode = str.charCodeAt(i);
                                if (charCode <= 0x007f) {
                                    total += 1;
                                } else {
                                    total += 2;
                                }
                            }
                        }
                        return total;
                    })(classNew, "utf-8");
                if (length > 10) {
                    classNewJq.formWarning({
                        tips: "显示名称不能超过5个汉字或10个英文字符"
                    });
                    return;
                }
                var option = $("<option></option>");
                option
                    .text(classNew)
                    .attr("id", "new");
                $(this).closest("#classEdit").find('#classList').append(option);
                $(this).closest("#classEdit").find('#classNew').val("");
            } else {
                classNewJq.formWarning({
                    tips: "显示名称不能为空"
                })
            }
        });
        $("#classEdit").on("click", ".btnPre", function() {
            var obj = document.getElementById('classList')
            for (var i = 1; i < obj.length; i++) { //最上面的一个不需要移动，所以直接从i=1开始  
                if (obj.options[i].selected) {
                    if (!obj.options.item(i - 1).selected) {
                        var selText = obj.options[i].text;
                        var itemid = jQuery(obj.options[i]).attr('id');

                        obj.options[i].text = obj.options[i - 1].text;
                        jQuery(obj.options[i]).attr('id', jQuery(obj.options[i - 1]).attr('id'));

                        obj.options[i - 1].text = selText;
                        jQuery(obj.options[i - 1]).attr('id', itemid);

                        obj.options[i].selected = false;
                        obj.options[i - 1].selected = true;
                    }
                }
            }
        });
        $("#classEdit").on("click", ".btnNext", function() {
            var obj = document.getElementById('classList')
            for (var i = obj.length - 2; i >= 0; i--) { //向下移动，最后一个不需要处理，所以直接从倒数第二个开始  
                if (obj.options[i].selected) {
                    if (!obj.options[i + 1].selected) {

                        var selText = obj.options[i].text;
                        var itemid = jQuery(obj.options[i]).attr('id');

                        obj.options[i].text = obj.options[i + 1].text;
                        jQuery(obj.options[i]).attr('id', jQuery(obj.options[i + 1]).attr('id'));

                        obj.options[i + 1].text = selText;
                        jQuery(obj.options[i + 1]).attr('id', itemid);

                        obj.options[i].selected = false;
                        obj.options[i + 1].selected = true;
                    }
                }
            }
        });
        $("#classEdit").on("click", ".btnChange", function() {});
        $("#classEdit").on("click", ".btnDel", function() {
            jQuery("#classEdit #classList option:selected").remove();
        });
        $("#classEdit").on("click", ".btnSave", function() {
            var i;
            var classList = $(this).closest("#classEdit").find("#classList").find("option");
            if (classList.length === 0) {
                $(this).success("至少要有一个分类");
                return;
            }
            var arrClassEdited = [];
            for (i = 0; i < classList.length; i++) {
                arrClassEdited.push({
                    SellControlClassifyID: classList.eq(i).attr("id"),
                    ClassName: classList.eq(i).text(),
                    OrderBy: (i + 1)
                });
            }

            $.ajax({
                type: 'post',
                dataType: 'json',
                contentType: "application/json; charset=utf-8;",
                url: '/SellControls/SetSellControlClassities',
                data: JSON.stringify(arrClassEdited),
                success: function(data) {
                    if (data.ErrorCode == 200) {
                        $("#SellControl #classify").empty().trigger("update", [data.data]);
                        var openModals = $("body").data("modalmanager").getOpenModals();
                        if (openModals) {
                            for (var i in openModals) {
                                if ($(openModals[i]['$element'][0]).attr("id") !== "SellControl") {
                                    $(openModals[i]['$element'][0]).modal("hide");
                                }
                            }
                        }
                    } else if (data.ErrorCode == 201) {

                        $.LangHua.alert({
                            title: "提示信息",
                            tip1: '下列分类中还有产品显示，不能删除！请将产品从分类中移除后重试',
                            tip2: data.ErrorMessage,
                            button: '确定',
                            icon: "warning",
                            indent: true,
                            callback: function() {
                                var openModals = $("body").data("modalmanager").getOpenModals();
                                if (openModals) {
                                    for (var i in openModals) {
                                        if ($(openModals[i]['$element'][0]).attr("id") !== "SellControl") {
                                            $(openModals[i]['$element'][0]).modal("hide");
                                        }
                                    }
                                }
                                $("#SellControl #classify").empty().trigger("update", [data.data]);
                            }
                        })
                    }
                }
            });
        });
        $("#classMoveTo").on("click", ".btnSave", function() {
            if ($("#classMoveTo #classify").val() == "unset") {
                $("#classMoveTo #classify").formWarning({ "tips": "请您选择分类" });
                return;
            }
            var classIDNew = $("#classMoveTo #classify").val();
            var textNew = $("#classMoveTo #classify option:selected").text();
            var itemID = $("#classMoveTo #classNameCurrent").data("itemid");
            $("#SellControl #itemlist option[data-itemid=" + itemID + "]").data("classifyid", classIDNew);
            $("#SellControl #itemlist option[data-itemid=" + itemID + "]").data("classtext", textNew);
            var textOption = $("#SellControl #itemlist option[data-itemid=" + itemID + "]").text();
            textOption = textOption.replace(/【.*?】/, '【' + textNew + '】');
            $("#SellControl #itemlist option[data-itemid=" + itemID + "]").text(textOption);
            jQuery("#classMoveTo").modal("hide");
        });
    }





    jQuery("#ShowSell").on('click', '#SetSell', function() {
        var SellControlID = jQuery(this).data("sellid");
        var ServiceTypeID = jQuery(this).data("typeid");
        var SellControlName = jQuery(this).closest('.oneSellcontrol').find("#oldSellControlName").text();
        var SellControlNum = jQuery(this).closest('.oneSellcontrol').find("#oldSellControlNum").text();
        var MonthNum = jQuery(this).siblings("#oldMonthNum").val();
        var StartDate = jQuery(this).siblings("#oldStartDate").val() == "0001-01-01" ? "" : jQuery(this).siblings("#oldStartDate").val();
        var SecondItemName = jQuery(this).siblings("#oldSecondItemName").val();
        var SecondServiceItemID = jQuery(this).siblings("#oldSecondServiceItemID").val();
        var SecondServiceCode = jQuery(this).siblings("#SecondServiceCode").text();
        var IsSurplusNum = jQuery(this).closest('.oneSellcontrol').find("#oldIsSurplusNum").val();
        var IsDistribution = jQuery(this).closest('.oneSellcontrol').find("#oldIsDistribution").val();

        jQuery("#SetProduct #SellControlID").val(SellControlID);
        jQuery("#SetProduct #SellControlName").val(SellControlName);
        jQuery("#SetProduct #SellControlNum").val(SellControlNum);
        jQuery("#SetProduct #MonthNum").val(MonthNum);
        jQuery("#SetProduct #StartDate").val(StartDate);
        jQuery("#SetProduct #SecondItemName").val(SecondItemName);
        jQuery("#SetProduct #SecondItemName").data('which', SecondServiceItemID);
        jQuery("#SetProduct #SecondItemName").data('code', SecondServiceCode);

        jQuery("#SetProduct #setting-special-button").data('sellid', SellControlID);
        if (ServiceTypeID == 4) {
            jQuery("#numValue").text("间");
        } else {
            jQuery("#numValue").text("人");
        }

        if (IsSurplusNum) {
            jQuery('#IsSurplusNum').prop("checked", true);
        } else {
            jQuery('#IsSurplusNum').prop("checked", false);
        }
        if (IsDistribution) {
            jQuery('#IsDistribution').prop("checked", true);
        } else {
            jQuery('#IsDistribution').prop("checked", false);
        }
        if (SecondServiceItemID) {
            jQuery('#existsProduct').prop("checked", true);
            jQuery('#product').removeClass("hidden");
            jQuery('#productremark').removeClass("hidden");
        } else {
            jQuery('#existsProduct').prop("checked", false);
            jQuery('#product').addClass("hidden");
            jQuery('#productremark').addClass("hidden");
        }
    })



    //设置成功动作
    $('.oneSellcontrol').each(function() {
        $(this).bind("settingSucceed", function(e, data) {
            jQuery(this).find("#oldSellControlName").text(data.SellControlName);
            jQuery(this).find("#oldSellControlNum").text(data.SellControlNum);
            jQuery(this).find("#oldMonthNum").val(data.MonthNum);
            jQuery(this).find("#oldStartDate").val(data.StartDate)
            jQuery(this).find("#oldSecondItemName").val(data.cnItemName);
            jQuery(this).find("#oldSecondServiceItemID").val(data.SecondServiceItem.ServiceItemID);
            jQuery(this).find("#oldIsSurplusNum").val(data.IsSurplusNum);
            jQuery(this).find("#oldIsDistribution").val(data.IsDistribution);
            if (jQuery('#existsProduct:checked').length > 0) {
                jQuery(this).find("#SecondServiceCode").text(data.code);
            } else {
                jQuery(this).find("#SecondServiceCode").text('');
            }
            $(this).find(".reflsahOneSellcontrol:eq(0)").trigger("click");

        })
    })

    jQuery("#btnUpdate").bind('click', function() {
        var _this = this;
        var bl = true;
        if (!jQuery("#SetProduct #SellControlName").val()) {
            jQuery('#SetProduct #SellControlName').warning("请填写");
            bl = false;
        }
        if (!jQuery("#SetProduct #SellControlNum").val()) {
            jQuery('#SetProduct #SellControlNum').warning("请填写");
            bl = false;
        }
        if (!jQuery("#SetProduct #MonthNum").val()) {
            jQuery('#SetProduct #MonthNum').warning("请填写");
            bl = false;
        }
        if (!jQuery("#SetProduct #StartDate").val()) {
            jQuery('#SetProduct #StartDate').warning("请填写");
            bl = false;
        }
        if (jQuery('#existsProduct:checked').length > 0) {
            if (!jQuery("#SetProduct #SecondItemName").data('which')) {
                jQuery('#SetProduct #SecondItemName').warning("请选中产品");
                bl = false;
            }
        }
        if (bl) {
            var sell = {
                SellControlID: jQuery("#SetProduct #SellControlID").val(),
                SellControlName: jQuery("#SetProduct #SellControlName").val(),
                SellControlNum: jQuery("#SetProduct #SellControlNum").val(),
                MonthNum: jQuery("#SetProduct #MonthNum").val(),
                StartDate: jQuery("#SetProduct #StartDate").val(),
                cnItemName: jQuery("#SetProduct #SecondItemName").val(),
                code: jQuery("#SetProduct #SecondItemName").data('code'),
                IsSurplusNum: (function() {
                    if (jQuery('#IsSurplusNum:checked').length > 0) {
                        return true;
                    }
                })(),
                IsDistribution: (function() {
                    if (jQuery('#IsDistribution:checked').length > 0) {
                        return true;
                    }
                })(),
                SecondServiceItem: {
                    ServiceItemID: (function() {
                        if (jQuery('#existsProduct:checked').length > 0) {
                            return jQuery("#SetProduct #SecondItemName").data('which');
                        }
                    })(),
                }
            }
            $.ajax({
                type: 'post',
                dataType: 'json',
                contentType: "application/json; charset=utf-8;",
                data: JSON.stringify(sell),
                url: '/SellControls/SetSellControl',
                success: function(data) {
                    if (data.ErrorCode == 200) {
                        jQuery("#btnUpdate").success("设置成功");
                        $('.oneSellcontrol#' + sell.SellControlID).trigger("settingSucceed", [sell]);
                        // $(_this).siblings('a.btn').trigger("click");
                        $("#SetProduct").modal('hide');
                        if (jQuery('#existsProduct:checked').length <= 0) {
                            $('#SetProduct #SecondItemName').val(null);
                            $('#SetProduct #SecondItemName').typeahead('val', '');
                        }
                    }
                }
            });
        }
    })









    jQuery('#StartDate').datepicker();





    jQuery('#existsProduct').bind('click', function() {
        if (jQuery('#existsProduct:checked').length == 0) {
            jQuery('#product').addClass("hidden")
            jQuery('#productremark').addClass("hidden")
        } else {
            jQuery('#product').removeClass("hidden")
            jQuery('#productremark').removeClass("hidden")
        }
    })
























    // 获取数据和刷新单个
    $('.oneSellcontrol').each(function() {
        var _this = this;
        $(this).on("Update", function(e, data) {
            $(this).addClass("updated");
            if (!$(this).find(".showhide:eq(0)").hasClass("shown")) {
                $(this).find(".showhide:eq(0)").trigger("click");
            }
            var IsSurplusNum = jQuery(_this).find("#oldIsSurplusNum").val();
            var obj = {
                SupplierID: $(_this).find('#SupplierID').text(),
                ItemName: $(_this).find('#FirstServiceCode').text().trim() + ' ' + $(_this).find('#SecondServiceCode').text().trim(),
                SupplierName: $(_this).find('#SupplierName').text()
            };
            var count = 0;
            var str = [
                '<td class="#color#">',
                '<div class="#classdate#"  data-remark ="#remark#" data-controlnum="#controlnum#" >#date#</div>',
                '<div class="selldetail">#detail#</div>',
                '</td>',
            ].join('\n');
            var strs = '<tr>';

            for (var i in data) {
                if (count % 16 == 0) {
                    strs += '</tr><tr>'
                }
                strs += maketd(data[i], $.extend(true, {}, obj));
                count++;
            };
            strs += '</tr>';

            function maketd(onetd, obj) {
                if (onetd.ReMark == "") {
                    var classdate = "date";
                    var remark = "";
                    var controlnum = onetd.ControlNum;
                } else {
                    var classdate = 'date extraSetting';
                    var remark = onetd.ReMark;
                    var controlnum = onetd.ControlNum;
                }
                var colorMap = {
                    0: 'white',
                    1: 'green',
                    2: "yellow",
                    3: "red",
                    4: "gray",
                };
                var URL = '/Orders/Index?search='
                var detail = '';

                if (onetd.ReturnNum == -1) { //单产品 只有去
                    obj['TravelDateBegin'] = onetd.thisdate;
                    obj['TravelDateEnd'] = onetd.thisdate;

                    var search = JSON.stringify(obj);
                    search = search.urlSwitch();
                    var lastUrl = URL + search;

                    var hreflink = '';
                    if (onetd.TravelNum + onetd.PreTravelNum > 0) {
                        hreflink = "<a target='_blank' href='" + lastUrl + "'>";
                    }
                    var Distributionlink = '';
                    if (onetd.DistributionNum > 0) {
                        Distributionlink = "+<a target='_blank' href='/DistributionTallies/Index?search=" + search + "'>" + onetd.DistributionNum + "</a>";
                    }
                    if (IsSurplusNum) {
                        var SurplusNum = controlnum - onetd.TravelNum - onetd.PreTravelNum - onetd.DistributionNum;
                        detail += "<div>" + hreflink + "余：" + SurplusNum + "</a></div>";
                    } else {
                        if (onetd.PreTravelNum == 0) {
                            detail += "<div>" + hreflink + onetd.TravelNum + "</a>" + Distributionlink + "</div>";
                        } else {
                            detail += "<div class='half'>" + hreflink + onetd.TravelNum + "</a></div>" + "<div class='half'>(" + onetd.PreTravelNum + ")" + Distributionlink + "</div>";
                        }
                    }
                    delete(obj['TravelDateBegin']);
                    delete(obj['TravelDateEnd']);
                } else {
                    obj['TravelDateBegin'] = onetd.thisdate;
                    obj['TravelDateEnd'] = onetd.thisdate;
                    var search = JSON.stringify(obj);
                    search = search.urlSwitch();
                    var lastUrl = URL + search;
                    var hreflink = '';
                    if (onetd.TravelNum + onetd.PreTravelNum > 0) {
                        hreflink = "<a target='_blank' href='" + lastUrl + "'>";
                    }
                    var Distributionlink = '';
                    if (onetd.DistributionNum > 0) {
                        Distributionlink = "+<a target='_blank' href='/DistributionTallies/Index?search=" + search + "'>" + onetd.DistributionNum + "</a>";
                    }
                    if (IsSurplusNum) {
                        var SurplusNum = controlnum - onetd.TravelNum - onetd.PreTravelNum - onetd.DistributionNum;
                        detail += "<div class='half'>去：" + hreflink + SurplusNum + "(余)</a></div>";
                    } else {
                        if (onetd.PreTravelNum == 0) {
                            detail += "<div class='half'>去：" + hreflink + onetd.TravelNum + "</a>" + Distributionlink + "</div>";
                        } else {
                            detail += "<div class='half'>去：" + hreflink + onetd.TravelNum + "(" + onetd.PreTravelNum + ")</a>" + Distributionlink + "</div>";
                        }
                    }
                    delete(obj['TravelDateBegin']);
                    delete(obj['TravelDateEnd']);

                    obj['ReturnDateBegin'] = onetd.thisdate;
                    obj['ReturnDateEnd'] = onetd.thisdate;
                    var Returnsearch = JSON.stringify(obj);
                    Returnsearch = Returnsearch.urlSwitch();
                    var ReturnlastUrl = URL + Returnsearch;
                    var Returnhreflink = '';
                    if (onetd.ReturnNum + onetd.PreReturnNum > 0) {
                        Returnhreflink = "<a target='_blank' href='" + ReturnlastUrl + "'>";
                    }
                    if (IsSurplusNum) {
                        var SurplusNum = controlnum - onetd.ReturnNum - onetd.PreReturnNum - onetd.DistributionNum;
                        detail += "<div class='half'>回：" + Returnhreflink + SurplusNum + "(余)</a></div>";
                    } else {
                        if (onetd.PreReturnNum == 0) {
                            detail += "<div class='half'>回：" + Returnhreflink + onetd.ReturnNum + "</a></div>";
                        } else {
                            detail += "<div class='half'>回：" + Returnhreflink + onetd.ReturnNum + "(" + onetd.PreReturnNum + ")</a></div>";
                        }
                    }
                    delete(obj['ReturnDateBegin']);
                    delete(obj['ReturnDateEnd']);
                }
                var td = str;
                td = td.replace("#date#", onetd.date);
                td = td.replace("#detail#", detail);
                td = td.replace("#color#", colorMap[onetd.state]);
                td = td.replace("#classdate#", classdate);
                td = td.replace("#remark#", remark);
                td = td.replace("#controlnum#", controlnum);
                return td;
            }
            $(this).find("tbody").empty().append(strs).find('.selldetail').animateCssTime('slideInUp', 'animate200');
            $(this).removeClass("updating");
        }).on("resultReturn", function() {
            $(this).find('.reflsahOneSellcontrol:eq(0)').button('reset');
        });
        $(this).find(".reflsahOneSellcontrol:eq(0)").bind('click', function() {
            $(this).button('loading');
            getDataOfOneSellControl($(_this).attr('id'));
            $(this).closest(".oneSellcontrol").addClass("updating");
        });
        $(this).find(".showhide:eq(0)").bind('click', function() {
            if (!$(this).hasClass("shown")) {
                if (!$(this).closest(".oneSellcontrol").hasClass("updated")) {
                    if (!$(this).closest(".oneSellcontrol").hasClass("updating")) {
                        $(this).closest(".oneSellcontrol").find(".reflsahOneSellcontrol:eq(0)").trigger("click");
                    }
                }
                $(this).closest(".oneSellcontrol").find(".dataHolder").css("display", "block");
                $(this).addClass("shown");
                $(this).find("i:eq(0)").addClass("rate180");
            } else {
                $(this).closest(".oneSellcontrol").find(".dataHolder").css("display", "none");
                $(this).removeClass("shown");
                $(this).find("i:eq(0)").removeClass("rate180");
            }
        });
    });


    $('#showAll').on("click", function() {
        $('.oneSellcontrol .showhide').each(function() {
            if (!$(this).hasClass("shown")) {
                $(this).trigger("click");
            }
        });
    });
    $('#hideAll').on("click", function() {
        $('.oneSellcontrol .showhide').each(function() {
            if ($(this).hasClass("shown")) {
                $(this).trigger("click");
            }
        });
    });

    function getDataOfOneSellControl(sellid) {
        $.ajax({
            url: '/SellControls/GetSellControl/' + sellid,
            type: 'get',
            dataType: "json",
            success: function(data) {
                if (data.ErrorCode == 200) {
                    $(".oneSellcontrol#" + sellid).trigger("Update", [data.data]);
                }
            },
            complete: function() {
                $(".oneSellcontrol#" + sellid).trigger("resultReturn");
            }
        })
    }





    // 特殊规则
    $('#special-setting-modal #settingCreated ').datepicker({
        inputs: $('.range')
    }).on('show', function(e) {
        e.stopPropagation();
    });
    $('#special-setting-modal #ExtraSettingNum input:eq(0)').onlyNumWithEmpty();


    //创建规则
    $('body').on("click", '.settingSave', function() {
        var thisRule = $(this).closest("tr");


        var post = {
            ExtraSettingID: thisRule.attr('id'),
            SellControlID: ($('#special-setting-modal').data('sellid')),
            StartTime: $.trim(thisRule.find('#StartTime input:eq(0)').val()),
            EndTime: $.trim(thisRule.find('#EndTime input:eq(0)').val()),
            ExtraSettingNum: thisRule.find('#ExtraSettingNum input:eq(0)').val(),
            Remark: $.trim(thisRule.find('#Remark input:eq(0)').val())
        }
        var cancel = false;
        if ((post.StartTime == null || post.StartTime == undefined || post.StartTime == "")) {
            cancel = true;
            thisRule.find('#StartTime input:eq(0)').Warning({
                title: "请选择或填写"
            });
        }
        if ((post.EndTime == null || post.EndTime == undefined || post.EndTime == "")) {
            cancel = true;
            thisRule.find('#EndTime input:eq(0)').Warning({
                title: "请选择或填写"
            });;
        }
        if (post.StartTime && post.EndTime) {
            var timeStartArr = post.StartTime.split("-");
            var timeEndArr = post.EndTime.split("-");
            timeStartStamp = (new Date(timeStartArr[0], timeStartArr[1], timeStartArr[2], 0, 0, 0)).valueOf();
            timeEndStamp = (new Date(timeEndArr[0], timeEndArr[1], timeEndArr[2], 0, 0, 0)).valueOf();
            if (parseInt(timeEndStamp) < parseInt(timeStartStamp)) {
                cancel = true;
                thisRule.find('#EndTime input:eq(0)').Warning({
                    title: '开始日期大于结束日期'
                });
            }
        }
        if (post.Remark == null || post.Remark == undefined || post.Remark == '') {
            cancel = true;
            thisRule.find('#Remark input:eq(0)').Warning({
                title: "请填写"
            });
        }
        if (post.ExtraSettingNum == null || post.ExtraSettingNum == undefined || post.ExtraSettingNum == '') {
            cancel = true;
            thisRule.find('#ExtraSettingNum input:eq(0)').Warning({
                title: '请填写'
            });
        }
        if (cancel) {
            return;
        }



        $.ajax({
            url: '/ExtraSetting/Insert',
            type: 'post',
            contentType: "application/json; charset=utf-8;",
            data: JSON.stringify(post),
            dataType: 'json',
            success: function(data) {
                if (data.ErrorCode != 200) {
                    if (data.ErrorCode != 401) {
                        $.LangHua.alert({
                            title: "提示信息",
                            tip1: '添加失败',
                            tip2: '提示信息：',
                        })
                    } else {
                        $.LangHua.alert({
                            title: "提示信息",
                            tip1: '添加失败',
                            tip2: data.ErrorMessage,
                        })
                    }
                } else {
                    thisRule.find('#StartTime input:eq(0)').val('');
                    thisRule.find('#EndTime input:eq(0)').val('');
                    thisRule.find('#ExtraSettingNum input:eq(0)').val('');
                    thisRule.find('#Remark input:eq(0)').val('');
                    var one = makeMakeTableRule(data.Data)
                    thisRule.before(one);
                    thisRule.closest("tbody ").find("#" + data.Data.ExtraSettingID).datepicker({
                        inputs: $('.rangex' + data.Data.ExtraSettingID),
                        container: ".modal-scrollable",
                    }).on('show', function(e) {
                        e.stopPropagation();
                    });
                    thisRule.closest("tbody ").find('tr#' + data.Data.ExtraSettingID).animateCssTime("slideInUp", 'animate200');
                    thisRule.closest("tbody ").find("tr#" + data.data[i].ExtraSettingID + " td#ExtraSettingNum input:eq(0)").onlyNumWithEmpty();
                    thisRule.find("td.Operation div.alt").success("修改成功！")


                }
            },
            error: function() {
                $.LangHua.alert({
                    title: "提示信息",
                    tip1: '添加失败',
                    tip2: '添加失败',
                })
            }
        });
    })

    //切换状态
    $('body').on('click', '.existRevise', function() {
            $(this).closest("tr").find('td .edit, td .alt').toggleClass("hidden");
            $(this).closest("tr").find('td .edit').each(function() {
                $(this).val($(this).siblings('.alt').text())
            })
        })
        //切换状态
    $('body').on('click', ' .existCancel', function() {
        $(this).closest("tr").find('td .edit, td .alt').toggleClass("hidden");

    })

    //删除
    $('body').on('click', ' .existDelete', function() {
        var thisButton = $(this);
        var thisRule = $(this).closest("tr");
        var post = {
            ExtraSettingID: thisRule.attr('id'),
        }
        $.LangHua.confirm({
            title: "提示信息",
            tip1: '提示信息：',
            tip2: '您确定要删除该条特殊设置吗?',
            confirmbutton: '确定',
            cancelbutton: '取消',
            confirm: function a() {
                $.ajax({
                    url: '/ExtraSetting/Delete',
                    type: 'post',
                    contentType: "application/json; charset=utf-8;",
                    data: JSON.stringify(post),
                    dataType: 'json',
                    success: function(data) {
                        if (data.ErrorCode != 200) {
                            if (data.ErrorCode != 401) {
                                $.LangHua.alert({
                                    title: "提示信息",
                                    tip1: '删除失败',
                                    tip2: '提示信息：',

                                })
                            } else {
                                $.LangHua.alert({
                                    title: "提示信息",
                                    tip1: '删除失败',
                                    tip2: data.ErrorMessage,
                                })
                            }

                        } else {
                            thisRule.remove();
                        }
                    },
                    error: function() {
                        $.LangHua.alert({
                            title: "提示信息",
                            tip1: '删除失败',
                            tip2: '提示信息：',

                        })
                    }
                });

            }
        })

    })

    //保存修改
    $('body').on('click', ' .existSave', function() {
        var thisRule = $(this).closest("tr");
        var post = {
            ExtraSettingID: thisRule.attr('id'),
            SellControlID: ($('#special-setting-modal').data('sellid')),
            StartTime: thisRule.find('#StartTime input:eq(0)').val(),
            EndTime: thisRule.find('#EndTime input:eq(0)').val(),
            ExtraSettingNum: thisRule.find('#ExtraSettingNum input:eq(0)').val(),
            Remark: thisRule.find('#Remark input:eq(0)').val()
        }

        var cancel = false;
        if ((post.StartTime == null || post.StartTime == undefined || post.StartTime == "")) {
            cancel = true;
            thisRule.find('#StartTime input:eq(0)').Warning({
                title: "请选择或填写"
            });
        }
        if ((post.EndTime == null || post.EndTime == undefined || post.EndTime == "")) {
            cancel = true;
            thisRule.find('#EndTime input:eq(0)').Warning({
                title: "请选择或填写"
            });;
        }
        if (post.StartTime && post.EndTime) {
            var timeStartArr = post.StartTime.split("-");
            var timeEndArr = post.EndTime.split("-");
            timeStartStamp = (new Date(timeStartArr[0], timeStartArr[1], timeStartArr[2], 0, 0, 0)).valueOf();
            timeEndStamp = (new Date(timeEndArr[0], timeEndArr[1], timeEndArr[2], 0, 0, 0)).valueOf();
            if (parseInt(timeEndStamp) < parseInt(timeStartStamp)) {
                cancel = true;
                thisRule.find('#EndTime input:eq(0)').Warning({
                    title: '结束日期小于开始日期'
                });
            }
        }
        if (post.Remark == null || post.Remark == undefined || post.Remark == '') {
            cancel = true;
            thisRule.find('#Remark input:eq(0)').Warning({
                title: "请填写"
            });
        }
        if (post.ExtraSettingNum == null || post.ExtraSettingNum == undefined || post.ExtraSettingNum == '') {
            cancel = true;
            thisRule.find('#ExtraSettingNum input:eq(0)').Warning({
                title: '请填写'
            });
        }
        if (cancel) {
            return;
        }


        $.ajax({
            url: '/ExtraSetting/Update',
            type: 'post',
            contentType: "application/json; charset=utf-8;",
            data: JSON.stringify(post),
            dataType: 'json',
            success: function(data) {
                if (data.ErrorCode == 200) {
                    thisRule.find('td.editArea .edit').each(function() {
                        $(this).siblings('.alt').text($(this).val());
                    })
                    thisRule.find('td .edit, td .alt').toggleClass("hidden");
                    thisRule.find("td.Operation div.alt").success("修改成功！")

                } else if (data.ErrorCode == 401) {
                    $.LangHua.alert({
                        title: "提示信息",
                        tip1: '修改失败',
                        tip2: data.ErrorMessage,
                    })
                } else {
                    $.LangHua.alert({
                        title: "提示信息",
                        tip1: '修改失败',
                        tip2: '修改失败！',
                    })
                }

            },
            error: function() {
                $.LangHua.alert({
                    title: "提示信息",
                    tip1: '修改失败',
                    tip2: '修改失败！',
                })
            }

        });
    })


    //获取列表
    $('body').on('click', '#setting-special-button', function() {
        $('#special-setting-modal').data('sellid', $(this).data('sellid'));

    })
    $('body').on('show.bs.modal', '#special-setting-modal', function() {
        var thisModal = $(this);
        thisModal.find('tr.oldRule').remove();
        $.ajax({
            url: '/ExtraSetting/index?SellControlID=' + ($('#special-setting-modal').data('sellid')),
            type: 'get',
            dataType: 'json',
            success: function(data) {
                if (data.ErrorCode == 200) {

                    for (var i in data.data) {
                        var tr = makeMakeTableRule(data.data[i]);
                        thisModal.find("tbody").prepend(tr);
                        thisModal.find("tbody tr#" + data.data[i].ExtraSettingID).datepicker({
                            inputs: $('.rangex' + data.data[i].ExtraSettingID),
                            container: ".modal-scrollable",
                        }).on('show', function(e) {
                            e.stopPropagation();
                        });
                        thisModal.find("tbody tr#" + data.data[i].ExtraSettingID + " td#ExtraSettingNum input:eq(0)").onlyNumWithEmpty();

                    }
                    thisModal.find('table').animateCssTime("slideInUp", 'animate400')
                }
            }
        });
    })

    $('body').on('click', '#searchWidthSpecialSetting', function() {
        var sellid = $('#special-setting-modal').data('sellid');
        $('.oneSellcontrol#' + sellid).find(".reflsahOneSellcontrol:eq(0)").trigger("click");
        $(this).siblings().trigger("click");
        $('body #btnUpdate').siblings(".btn:eq(0)").trigger("click")


    })

    function makeMakeTableRule(oneRule) {
        console.log(oneRule)
        var tr = $('<tr></tr>');
        tr.attr("id", oneRule.ExtraSettingID).addClass("oldRule");
        var td = $('<td></td>');
        td.addClass('td-padding-10');
        var input = $("<input   />");
        input.addClass("form-control edit hidden text-center");
        var div = $("<div></div>");
        var a = $('<a></a>');
        a.addClass('hrefInTable-inline');
        a.attr('href', 'javascript:;');

        var tdStartDay =
            td.clone()
            .attr("id", 'StartTime')
            .addClass(" editArea")
            .append(input.clone()
                .val(oneRule.StartTime.substr(0, 10))
                .addClass("rangex" + oneRule.ExtraSettingID)
            )
            .append(div.clone()
                .addClass('alt')
                .text(oneRule.StartTime.substr(0, 10))
            );

        var tdEndtDay =
            td.clone()
            .attr("id", 'EndTime')
            .addClass(" editArea")
            .append(input.clone()
                .val(oneRule.EndTime.substr(0, 10))
                .addClass("rangex" + oneRule.ExtraSettingID)
            )
            .append(div.clone()
                .addClass('alt')
                .text(oneRule.EndTime.substr(0, 10))
            );
        var tdNum =
            td.clone()
            .attr("id", 'ExtraSettingNum')
            .addClass(" editArea")
            .append(input.clone()
                .val(oneRule.ExtraSettingNum)
            )
            .append(div.clone()
                .addClass('alt')
                .text(oneRule.ExtraSettingNum)
            );
        var tdReason =
            td.clone()
            .attr("id", 'Remark')
            .addClass("StartTime editArea")
            .append(input.clone()
                .val(oneRule.Remark)
            )
            .append(div.clone()
                .addClass('alt')
                .text(oneRule.Remark)
            );


        var divO = div.clone();
        divO.addClass("alt")
        divO.append(a.clone().addClass("existRevise").text('修改'));
        divO.append(a.clone().addClass('existDelete').text('删除'));

        var divOx = div.clone();
        divOx.addClass("edit hidden")
        divOx.append(a.clone().addClass('existSave').text('保存'));
        divOx.append(a.clone().addClass('existCancel').text('取消'));

        var tdOperation =
            td.clone()
            .addClass("Operation")
            .append(divO)
            .append(divOx);

        tr.append(tdStartDay);
        tr.append(tdEndtDay);
        tr.append(tdNum);
        tr.append(tdReason);
        tr.append(tdOperation);

        return tr;
    }



    // 特殊设置显示
    $('#ShowSell')
        .on("mouseenter", '.extraSetting', function() {

            var data = $(this).data();
            var div = $('<div></div>');
            var span = $('<span></span>');

            var controlnum = div.clone()
                .css("line-height", "24px")
                .append(span.clone()
                    .css('font-weight', "bold")
                    .text('控位总数：')
                )
                .append(span.clone()
                    .text(data.controlnum)
                );
            var remark = div.clone()
                .css("line-height", "24px")
                .append(span.clone()
                    .css('font-weight', "bold")
                    .text('原因：')
                )
                .append(span.clone()
                    .text(data.remark)
                );
            var container = div.clone()
                .append(controlnum)
                .append(remark)

            var wrapperTD = $(this);
            var base = $("#hiddenRelative");
            var leftBase = base.offset().left;
            var topBase = base.offset().top;
            var left = wrapperTD.offset().left;
            var top = wrapperTD.offset().top;
            var leftR = left - leftBase + 5;
            var topR = top - topBase + parseInt(wrapperTD.css('height').split('px')[0]) + 5;
            var divOffset = {
                'left': leftR,
                'top': topR,
                'position': 'absolute',
                'z-index': 300,
                'background': 'white',
                'border': '1px solid #797979',
                'box-shadow': '5px 5px 5px rgba(0,0,0,0.35)',
                'padding': '5px 40px 10px 8px'

            };
            container.css(divOffset);
            $('#hiddenRelative').append(container).find(".rechagne").css(divOffset)


        })
        .on('mouseleave', '.extraSetting', function() {
            $('#hiddenRelative').empty();
        });

    function statisticsSellControl() {
        $("body #GetStatisticsSellControl #rangeDate").datepicker({
            'inputs': $(".dateX"),
            'keepEmptyValues': true
        });
        $("body #GetStatisticsSellControl .setdates").on("click", function() {
            var dateStart, dateEnd, dateTemp;
            var now = new Date();
            if ($(this).hasClass("pastDaysThisMonth")) {
                dateStart = new Date(now.getFullYear(), now.getMonth(), 1, 0, 0, 0);
                dateEnd = new Date(now.getFullYear(), now.getMonth(), parseInt(now.getDate()) === 1 ? 1 : now.getDate() - 1, 0, 0, 0);
            } else if ($(this).hasClass("thisMonth")) {
                dateTemp = new Date(parseInt(now.getMonth()) === 11 ? parseInt(now.getFullYear()) + 1 : now.getFullYear(), parseInt(now.getMonth()) === 11 ? 0 : parseInt(now.getMonth()) + 1, 1, 0, 0, 0); //下个月1号
                dateStart = new Date(now.getFullYear(), now.getMonth(), 1, 0, 0, 0);
                dateEnd = new Date(dateTemp.valueOf() - 1);
            } else if ($(this).hasClass("lastMonth")) {
                dateTemp = new Date(now.getFullYear(), now.getMonth(), 1, 0, 0, 0);
                dateEnd = new Date(dateTemp.valueOf() - 1);
                dateStart = new Date(dateEnd.getUTCFullYear(), dateEnd.getMonth(), 1, 0, 0, 0);
            }
            $("body #GetStatisticsSellControl #rangeDate .dateX:eq(0)").datepicker("setDate", dateStart);
            $("body #GetStatisticsSellControl #rangeDate .dateX:eq(1)").datepicker("setDate", dateEnd);
        });

        $("body").on("shown.bs.modal", "#GetStatisticsSellControl", function() {
            $("body #GetStatisticsSellControl .setdates.pastDaysThisMonth:eq(0)").trigger("click");
            $("body #GetStatisticsSellControl #statisticsSellControlResult tbody:eq(0)").empty();
            $("body #GetStatisticsSellControl #itemlistx").val("-1");
        });

        $("body #GetStatisticsSellControl #getStatistics").on("click", function() {
            var id = $("body #GetStatisticsSellControl #itemlistx").val();
            var StartDate = $("body #GetStatisticsSellControl #rangeDate .dateX:eq(0)").val();
            var EndDate = $("body #GetStatisticsSellControl #rangeDate .dateX:eq(1)").val();
            if (id == -1) {
                $("body #GetStatisticsSellControl #itemlistx").formWarning({
                    tips: "请选择"
                });
                return;
            }
            if (!StartDate) {
                $(this).success("请选择开始日期");
                return;
            }
            if (!EndDate) {
                $(this).success("请选择结束日期");
                return;
            }
            $.ajax({
                type: 'GET',
                dataType: 'json',
                contentType: "application/json; charset=utf-8;",
                data: {
                    id: id,
                    StartDate: StartDate,
                    EndDate: EndDate
                },
                url: '/SellControls/GetStatisticsSellControl',
                "beforeSend": function() {
                    $.LangHua.loadingToast({
                        tip: "正 在 获 取 统 计 信 息. . . . . ."
                    });
                },
                success: function(data) {
                    var openModals = $("body").data("modalmanager").getOpenModals();
                    if (openModals) {
                        for (var i in openModals) {
                            if ($(openModals[i]['$element'][0]).attr("id") !== "GetStatisticsSellControl") {
                                $(openModals[i]['$element'][0]).modal("hide");
                            }
                        }
                    }
                    if (data.ErrorCode == 200) {
                        var tr = $("<tr></tr>");
                        var td = $("<td></td>");
                        tr.append(
                            td.clone().text(data.data.ControlNum),
                            td.clone().text(data.data.TravelNum),
                            td.clone().text(data.data.PreTravelNum),
                            td.clone().text(data.data.DistributionNum),
                            td.clone().text(data.data.SurplusNum),
                            td.text(data.data.SellOutRate)
                        );
                        $("body #GetStatisticsSellControl #statisticsSellControlResult tbody:eq(0)").empty().append(tr);
                        $("body").trigger("resize");
                    } else if (data.ErrorCode == 401) {
                        $.LangHua.alert({
                            tip1: "获取统计数据失败",
                            tip2: data.ErrorMessage,
                            button: '确定',
                        })
                    } else {
                        $.LangHua.alert({
                            title: "提示信息",
                            tip1: '拉取数据失败',
                            tip2: "拉取数据失败",
                            button: '确定'
                        })
                    }

                },
                "error": function(jqXHR, textStatus, errorThrown) {
                    var openModals = $("body").data("modalmanager").getOpenModals();
                    if (openModals) {
                        for (var i in openModals) {
                            if ($(openModals[i]['$element'][0]).attr("id") !== "GetStatisticsSellControl") {
                                $(openModals[i]['$element'][0]).modal("hide");
                            }
                        }
                    }
                    $.LangHua.alert({
                        title: "提示信息",
                        tip1: '拉取数据失败',
                        tip2: "拉取数据失败",
                        button: '确定'
                    })
                }
            });
        });
    }

    statisticsSellControl();


})