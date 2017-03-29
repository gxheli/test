'use strict';
jQuery(document).ready(function($) {
    fix();
    var saleStatistics = tableInit($('.tabletools').eq(0));
    initTabletools(saleStatistics);
    var searchEngine = bloodHound();
    search(searchEngine, $('#saleStatistics-set'));
    setSaleStatistics($('#saleStatistics-set'), saleStatistics);

    function tableInit(tabletools) {
        var distribution =
            jQuery('table#saleStatistics')
            .on('preXhr.dt', function(e, settings, json) {
                delete json.columns;
                delete json.order;
                delete json.search;
                var search = $('#searchoption').data("search");
                if (search) {
                    if (typeof(search) === "string") {
                        json.SalesStatisticSearch = JSON.parse(search);
                    } else {
                        json.SalesStatisticSearch = (search);
                    }
                }

            })
            .on('xhr.dt', function(e, settings, json, xhr) {
                tabletools.find("#search").button('reset');
            })
            .DataTable({
                ajax: {
                    "url": "/SalesStatistics/GetSalesStatistics",
                    "type": 'post',
                    "dataSrc": "data"
                },
                deferLoading: [0, 0],
                ordering: false,
                searching: false,
                serverSide: true,
                //行操作
                createdRow: function(row, data, dataIndex) {
                    if ((data.IsCancel)) {
                        $(row).css("background", '#BCBCBC');
                    }
                    var _this = this.api();
                    $(row).data('distribution', data);
                },
                //列操作
                columnDefs: [{
                    'targets': [0],
                    'data': null,
                    "render": function(cellData, type, rowData, meta) {
                        var indexInpage = meta.row;
                        var settings = meta.settings;
                        return (settings._iDisplayStart + indexInpage + 1);
                    }
                }, {
                    'targets': [1],
                    'data': 'NickName',
                }, {
                    'targets': [2],
                    'data': 'BeginDate',
                }, {
                    'targets': [3],
                    'data': "EndDate"
                }, {
                    'targets': [4],
                    'data': 'Num1',
                    'render': function(cellData, type, rowData, meta) {
                        // var api = new $.fn.dataTable.Api(meta.settings);;
                        // var lastJson = api.ajax.json();
                        if (rowData.ServiceTypeID == 4) {
                            return cellData + "（间）";
                        }
                        return cellData;

                    }
                }, {
                    'targets': [5],
                    'data': 'Num2',
                    render: function(cellData, type, rowData, meta) {
                        // var api = new $.fn.dataTable.Api(meta.settings);;
                        // var lastJson = api.ajax.json();
                        if (rowData.ServiceTypeID == 4) {
                            return cellData + "（晚）";
                        }
                        return cellData;
                    }
                }, {
                    'targets': [6],
                    'data': 'Num3'
                }]
            });
        return {
            'dataTableRef': distribution,
            'jQueryRef': jQuery('table#saleStatistics')
        };
    }

    function　 fix() {
        if (jQuery("#searchoption").length === 0) {
            var search = jQuery('<div id="searchoption" ></div>').data("search", {
                "BeginDate": "2016-03-4",
                "EndDate": "2016-11-25",
                "SalesStatisticID": "18"
            })
            $('body').append(search);
        }
    }

    function search(searchEngine, which) {
        var thisModal = which;
        thisModal.find('#ItemID').typeahead({
            hint: false,
            highlight: true,
            minLength: 1,
        }, {
            name: 'xxx',
            displayKey: 'name',
            limit: 30,
            source: searchEngine,
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
        }).bind('typeahead:select', function(ev, suggestion) {
            $(this).data('which', suggestion);
            // 供应商更改
            var supplier = suggestion.supplyer;
            var optGroupStr = [];
            for (var i in supplier) {
                optGroupStr.push(
                    makeOneOption(supplier[i], suggestion.defaultSupplierID));
            }
            thisModal.find('#SupplierID').empty().append(optGroupStr);

            function makeOneOption(obj, defaultSupplierID) {
                var altName = '';
                if (obj.SupplierID == defaultSupplierID) {
                    altName = obj.SupplierNo + '-' + obj.SupplierName + "(默认)";
                } else {
                    altName = obj.SupplierNo + '-' + obj.SupplierName;
                }
                var option = $('<option></option>');
                option.prop("value", obj.SupplierID);
                option.text(altName);
                option.data("which", obj);
                return option;
            }
        }).bind("keydown", function(evt) {
            evt = (evt) ? evt : ((window.event) ? window.event : "") //兼容IE和Firefox获得keyBoardEvent对象  
            var key = evt.keyCode ? evt.keyCode : evt.which; //兼容IE和Firefox获得keyBoardEvent对象的键值  
            if (key !== 13) {
                if (($(this).data('which'))) {
                    thisModal.find('#SupplierID').empty().text("");
                    thisModal.find('#SupplierID').data('which', "");
                    $(this).data('which', "");

                }
            }
        });
    }

    function　 setSaleStatistics(modal, Refs) {
        modal.find('#addItems').one("click", function adding() {
            var _this = $(this);
            var itemInfo = modal.find('#ItemID').data("which");
            if (!itemInfo) {
                modal.find('#ItemID').formWarning({
                    tips: "请您搜索选择产品"
                });
                _this.one("click", adding);
                return;
            }
            var ItemID = itemInfo.serviceItemID;
            var tempItemID, index, cancel;
            var optionList = modal.find("#itemlist option");
            cancel = false;
            for (index = 0; index < optionList.length; index++) {
                tempItemID = optionList.eq(index).data('serviceitemid');
                if (tempItemID == ItemID) {
                    $.LangHua.alert({
                        tip2: '已经添加该产品',
                        button: '确定',
                        icon: "warning"
                    })
                    _this.one("click", adding);
                    cancel = true;
                    break;
                }
            }
            if (cancel === true) {
                return;
            }

            var supplierInfo = modal.find('#SupplierID option:selected').data("which");
            var option = $("<option></option>");
            option.data({
                "supplierid": supplierInfo.SupplierID,
                "serviceitemid": itemInfo.serviceItemID
            });
            option.text("(" + supplierInfo.SupplierNo + ")" + itemInfo.name + itemInfo.serviceCode);
            modal.find("#itemlist").append(option);
            modal.find('#ItemID').data("which", "");
            modal.find('#ItemID').typeahead("val", "");
            modal.find('#SupplierID').data('which', "");
            modal.find('#SupplierID').empty().text("");
            _this.one("click", adding);
        });
        modal.find('#btnDel').bind("click", function deleting() {
            var _this = $(this);
            modal.find('#itemlist option:selected').remove();
        });
        modal.find('#btnSave').one("click", function saving() {
            var _this = $(this);
            var arrItems = [];
            modal.find("#itemlist option").each(function() {
                arrItems.push({
                    "SupplierID": $(this).data("supplierid"),
                    "ServiceItemID": $(this).data("serviceitemid")
                });
            });
            if (arrItems.length === 0) {
                _this.one('click', saving);
                return;
            }
            $.ajax({
                url: '/SalesStatistics/SetSalesStatistics',
                type: 'post',
                contentType: "application/json; charset=utf-8;",
                data: JSON.stringify(arrItems),
                dataType: 'json',
                success: function(data) {
                    if (data.ErrorCode == 200) {
                        $.LangHua.alert({
                            tip1: '保存结果',
                            tip2: '保存成功',
                            button: '确定',
                            icon: "warning",
                            callback: function() {
                                window.location.reload();
                            }
                        });
                    } else if (data.ErrorCode == 401) {
                        $.LangHua.alert({
                            tip1: '保存失败',
                            tip2: data.ErrorMessage,
                            button: '确定',
                            icon: "warning"
                        });
                    } else {
                        $.LangHua.alert({
                            tip1: '保存失败',
                            tip2: "保存失败，请重试！",
                            button: '确定',
                            icon: "warning"
                        });
                    }
                },
                error: function() {
                    $.LangHua.alert({
                        tip1: '保存失败',
                        tip2: "保存失败，请重试 ！！",
                        button: '确定',
                        icon: "warning"
                    });
                },
                complete: function() {
                    _this.one('click', saving);
                }
            });
        });

        $('body').on("shown.bs.modal", '#saleStatistics-set', function(e) {
            var thisModal = $(this);
            thisModal.find('#ItemID').data("which", "");
            thisModal.find('#ItemID').typeahead("val", "");
            thisModal.find('#SupplierID').data('which', "");
            thisModal.find('#SupplierID').empty().text("");
            // thisModal.find('#itemlist option').remove();
        });

    }

    function　 initTabletools(Refs) {



        $('.tabletools:eq(0)').find('#daterange').datepicker({
            'inputs': $('.tabletools:eq(0) #daterange input')
        });
        var dateStart, dateEnd, dateTemp;
        var now = new Date();
        dateStart = new Date(now.getFullYear(), now.getMonth(), 1, 0, 0, 0);
        dateEnd = new Date(now.getFullYear(), now.getMonth(), parseInt(now.getDate()) === 1 ? 1 : now.getDate() - 1, 0, 0, 0);
        $("body  .tabletools:eq(0) #BeginDate").datepicker("setDate", dateStart);
        $("body  .tabletools:eq(0) #EndDate").datepicker("setDate", now);






        $('.tabletools:eq(0)').find('#search').one("click", function searching(e) {
            var BeginDate = $('.tabletools:eq(0)').find("#BeginDate").val();
            var EndDate = $('.tabletools:eq(0)').find('#EndDate').val();
            var SalesStatisticID = $('.tabletools:eq(0)').find('#SalesStatisticID').val();
            var cancel = false;
            if (!(BeginDate) || !(EndDate)) {
                $('.tabletools:eq(0)').find('#daterange').success("请您选择日期范围");
                $(this).one("click", searching);
                cancel = true;
            }
            if (SalesStatisticID == 0) {
                $('.tabletools:eq(0)').find('#SalesStatisticID').success("请您选择统计产品");
                $(this).one("click", searching);
                cancel = true;
            }
            if (cancel === true) {
                $(this).one("click", searching);
                return;
            }
            var search = {
                "BeginDate": BeginDate,
                "EndDate": EndDate,
                "SalesStatisticID": SalesStatisticID
            };
            $(this).button('loading');
            $('#searchoption').data('search', JSON.stringify(search));
            Refs.dataTableRef.draw();
            $(this).one("click", searching);
        });
    }
});


function bloodHound() {
    var searchEngine = new Bloodhound({
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
                return $.map(data.Items, function(one) {
                    return {
                        name: one.cnItemName,
                        enName: one.enItemName,
                        supplyer: one.ItemSupliers,
                        defaultSupplierID: one.DefaultSupplierID,
                        serviceItemID: one.ServiceItemID,
                        serviceCode: one.ServiceCode,
                        serviceTypeID: one.ServiceTypeID
                    };
                });
            }
        }
    });
    searchEngine.initialize();
    return searchEngine;
}