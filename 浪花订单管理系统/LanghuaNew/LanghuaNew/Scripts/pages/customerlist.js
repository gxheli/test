jQuery(document).ready(function() {

    var fuzzyString = document.getElementById("fuzzyString");
    fuzzyString.onkeydown = jump;

    function jump(event) {
        var event = event || window.event;
        if (event.keyCode == 13) {
            $('#fuzzySearch').trigger("click");
        }
    }
    $('#orderList').eq(0)
        .on('preXhr.dt', function(e, settings, json) {
            var search = ($('#searchoption').data("search"));
            // 删除插件无必要项目
            delete json.columns;
            delete json.order;
            delete json.search;
            json['CustomerSearch'] = search;

            $('#reflashTable').find('span').addClass("fa-spin");
        }).on('xhr.dt', function(e, settings, json, xhr) {
            $('#orderList thead tr th:eq(0) input').prop("checked", false)
            syncState(json.SearchModel.CustomerSearch);
            $('#needservice').text(json.NeedServiceCount);
            $('#back').text(json.BackCount);
            $('#weixinbind').text(json.WeixinBindCount);

            $('#reflashTable').find('span').removeClass("fa-spin");
        })
    var newproduct =
        jQuery('table#orderList')
        .DataTable({
            ajax: {
                url: "/Customers/GetCustomers",
                type: 'post',
            },
            ordering: false,
            searching: false,
            serverSide: true,
            initComplete: function(settings, json) {},
            //列操作
            columns: [{
                    'data': 'CustomerTBCode',
                    "createdCell": function(td, cellData, rowData, row, col) {
                        var search = {
                            FuzzySearch: cellData,
                            searchType: "fuzzy",
                        };
                        var searchStr = JSON.stringify(search).urlSwitch();

                        var link = '/Orders/Index?search=' + searchStr;
                        jQuery(td).html("<div><a target='_blank' href='" + link + "'>" + cellData + "</a></div>");
                    }
                },
                {
                    'data': 'CustomerName',
                    "createdCell": function(td, cellData, rowData, row, col) {
                        jQuery(td).html('<div>' + cellData + '</div><div class="mini">' + rowData.CustomerEnname + '</div>');
                    }
                },
                {
                    'data': 'Tel',
                    "createdCell": function(td, cellData, rowData, row, col) {
                        jQuery(td).html('<div>' + cellData + '</div><div>' + rowData.BakTel + '</div>');
                    }
                },
                {
                    'data': 'WeixinNo',
                    "createdCell": function(td, cellData, rowData, row, col) {
                        jQuery(td).html('<div>' + cellData + '</div>');
                    }
                },
                {
                    'data': 'OrderNum',
                    "createdCell": function (td, cellData, rowData, row, col) {
                        jQuery(td).html('<div>' + cellData + '</div>');
                    }
                },
                {
                    'data': 'IsNeedCustomerService',
                    "createdCell": function(td, cellData, rowData, row, col) {
                        if (cellData) {
                            jQuery(td).html('<div class="IsNeedCustomerService"><span class="badge-rectangle-default after-sale-service-color">要售后</span></div>');
                        } else {
                            jQuery(td).html('');
                        }
                    }
                },
                {
                    'data': 'IsBack',
                    "createdCell": function(td, cellData, rowData, row, col) {
                        var str = '';
                        if (cellData) {
                            str = '已回访';
                        }
                        jQuery(td).html(str);
                    }
                },
                {
                    'data': 'Remark',
                    "createdCell": function(td, cellData, rowData, row, col) {
                        jQuery(td).html(cellData);
                    }
                },
                //操作
                {
                    'data': 'CustomerID',
                    "createdCell": function(td, cellData, rowData, row, col) {
                        var str =
                            '<div class="row">' +
                            '<a class="hrefInTable-inline" target="_blank" href="/Customers/Details/' + cellData + '">详情</a>' +
                            '<a class="hrefInTable-inline" target="_blank" href="/Customers/CustomerOperation/' + cellData + '">日志</a>' +
                            '<a class="hrefInTable-inline" href="javascript:;" id="ResetPassWord" data-customerid="' + cellData + '">重置密码</a>' +
                            '</div>';
                        jQuery(td).html(str);
                    }
                }
            ]
        });
    //类型的筛选
    $("#State").ButtonRadio({
            selected: function(dom, code) {
                if ($('#searchoption').length == 0) {
                    $('body').append("<div id='searchoption' class='hidden'></div>");
                }
                $("#searchoption").data({
                    search: {
                        status: code
                    }
                })
                newproduct.draw();
            }
        })
        // 模糊搜索
    jQuery("#fuzzySearch").bind("click", function() {
            var fuzzyString = jQuery("#fuzzyString").val().trim();
            if ($('#searchoption').length == 0) {
                $('body').append("<div id='searchoption' class='hidden'></div>");
            }
            $('#searchoption').data("search", {
                FuzzySearch: fuzzyString
            });
            newproduct.draw();
        })
        //clearstate 
    function syncState(obj) {

        var method = {
            status: function(value) {
                $('#State').find(".buttonradio[data-code=" + value + "]")
                    .addClass("active")
                    .siblings(".active").removeClass("active");
            },
            FuzzySearch: function(value) {
                $('#fuzzyString').val(value == null ? "" : value);
            },
        }
        if (obj == null) {
            $('#State').find(".buttonradio:first")
                .addClass("active")
                .siblings('.buttonradio').removeClass("active");
            return;
        }
        for (var i in obj) {
            if (i == "searchType" || i == "StartDate" || i == "EndDate" || i == "ReturnList") {
                continue;
            }
            method[i](obj[i])
        }
    }


    jQuery('#daterange').datepicker();

    jQuery("#btnDel").bind("click", function() {
        jQuery("#itemlist option:selected").remove();
    })

    jQuery("#btnSave").bind('click', function() {
        var list = [];
        var itemlist = jQuery("#itemlist option");
        for (i = 0; i < itemlist.length; i++) {
            var li = {
                SupplierID: jQuery(itemlist[i]).data('supplierid'),
                ServiceItemID: jQuery(itemlist[i]).data('itemid')
            }
            list.push(li);
        }
        if (list.length == 0) {
            jQuery("#btnSave").success("请先添加回访产品");
            return;
        }
        $.ajax({
            type: 'post',
            dataType: 'json',
            contentType: "application/json; charset=utf-8;",
            data: JSON.stringify(list),
            url: '/Customers/SaveReturnList',
            success: function(data) {
                if (data.ErrorCode == 200) {
                    jQuery("#btnSave").success("保存成功");
                    jQuery("#btnSelect").trigger('click');
                } else {
                    $.LangHua.alert({
                        title: "提示信息",
                        tip1: '保存失败',
                        tip2: data.ErrorMessage,
                        button: '确定',
                    })
                }
            }
        });
    })
    jQuery("#btnSelect").bind('click', function() {
        if (!jQuery("#StartTime").val()) {
            jQuery("#StartTime").warning("请选择日期");
        }
        if (!jQuery("#EndTime").val()) {
            jQuery("#EndTime").warning("请选择日期");
        }
        if (jQuery("#StartTime").val() && jQuery("#EndTime").val()) {
            if ($('#searchoption').length == 0) {
                $('body').append("<div id='searchoption' class='hidden'></div>");
            }
            $('#searchoption').data("search", {
                StartDate: jQuery("#StartTime").val(),
                EndDate: jQuery("#EndTime").val(),
                ReturnList: true
            });
            newproduct.draw();
            $("#ReturnList").modal('hide');
        }
    })

    jQuery("#btnAdd").bind("click", function() {
        if (!jQuery('#ServiceItems').data('which')) {
            jQuery('#ServiceItems').warning("请选中产品");
            return;
        }
        var itemid = jQuery('#ServiceItems').data('which');
        var supplierid = jQuery("#supplier").val();
        var name = jQuery('#ServiceItems').data('name');
        var SupplierNo = jQuery("#supplier option:selected").data('supplierno');

        var list = jQuery("#itemlist option");
        //if (list.length >= 15) {
        //    $.LangHua.alert({
        //        title: "提示信息",
        //        tip1: '添加失败',
        //        tip2: "回访产品不能超过15个",
        //        button: '确定',
        //    })
        //    return;
        //}
        for (i = 0; i < list.length; i++) {
            if (itemid == jQuery(list[i]).data('itemid') && supplierid == jQuery(list[i]).data('supplierid')) {
                $.LangHua.alert({
                    title: "提示信息",
                    tip1: '产品已重复：',
                    tip2: "(" + SupplierNo + ")" + name,
                    button: '确定',
                })
                return;
            }
        }
        var newOption = document.createElement('option');
        newOption.setAttribute('data-itemid', itemid);
        newOption.setAttribute('data-supplierid', supplierid);
        var newContent = document.createTextNode("(" + SupplierNo + ")" + name);
        newOption.appendChild(newContent);
        jQuery("#itemlist").append(newOption);

    })
    var remote_cities = new Bloodhound({
        datumTokenizer: function(d) {
            return Bloodhound.tokenizers.whitespace(d.name);
        },
        queryTokenizer: Bloodhound.tokenizers.whitespace,
        limit: 15,
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
        displayKey: 'name',
        display: function(data) {
            return data.name //+" "+ data.code;
        },
        limit: 30,
        source: remote_cities,
        templates: {
            empty: [
                '<div class="empty-message">',
                '没有找到相关产品',
                '</div>'
            ].join('\n'),
            header: function(data) {
                return ([
                    '<div class="empty-message">',
                    '共搜索到<strong>' + data.suggestions.length + '</strong>个产品',
                    '</div>'
                ].join('\n'));
            },

            pending: [
                '<div class="empty-message">',
                '正在载入产品数据....',
                '</div>'
            ].join(''),
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

    }).bind("keydown", function(evt) {
        evt = (evt) ? evt : ((window.event) ? window.event : "") //兼容IE和Firefox获得keyBoardEvent对象  
        var key = evt.keyCode ? evt.keyCode : evt.which; //兼容IE和Firefox获得keyBoardEvent对象的键值  
        if (key === 13) {} else if (key !== 9) {
            console.log(key)
            if (($(this).data('which'))) {
                $('#supplier').empty().text("");
                $(this).data('which', "");
                $(this).data('name', "");
            }
        }
    });

    //表格工具订单导出
    $('body').on('click', '#exportproducts', { 'customerList': newproduct }, function(e) {
        if (e.data.customerList.ajax.json().recordsFiltered > 5000) {
            $.LangHua.alert({
                "tip1": "导出性能提示",
                "indent": false,
                "tip2": "导出数量超过5000条，请设置筛选条件后再导出"
            })
            return;
        }
        var varURL = "/Customers/ExportExcel?";
        var link = document.createElement("a");
        var search = $('#searchoption').data("search");
        if (search) {
            if (!(search instanceof Object)) {
                search = JSON.parse(search)
            } else {}
        } else {

        }

        for (var i in search) {
            varURL += i + '=' + search[i] + "&";
        }

        link.href = varURL;
        document.body.appendChild(link);
        link.click();
        delete link;
    })

    //重置
    jQuery("body").on('click', '#ResetPassWord', function(e, data) {

        var id = jQuery(this).data('customerid');
        var TBID = jQuery(this).closest("tr").find("td:eq(0)").text();

        jQuery.LangHua.confirm({
            title: "提示信息",
            tip1: '请确认是否为以下用户重置密码：',
            tip2: TBID,
            confirmbutton: '确定',
            cancelbutton: '取消',
            data: null,
            confirm: function() {
                $.ajax({
                    url: "/Customers/ResetPassWord/" + id,
                    type: 'post',
                    dataType: 'json',
                    success: function(data) {
                        jQuery.LangHua.alert({
                            title: "提示信息",
                            tip1: '提示信息：',
                            tip2: data,
                            button: '确定'
                        })
                    }
                })
            }
        })
    })


    $('#reflashTable').bind("click", function() {
        newproduct.draw()
    });
})