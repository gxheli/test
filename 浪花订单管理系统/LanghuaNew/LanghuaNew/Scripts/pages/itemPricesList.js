'use strict';
jQuery(document).ready(function($) {
    fix();
    var tableOrig = $('table#itemPrices').clone();
    var itemPrices = tableInit($('.tabletools').eq(0), tableOrig);
    initTabletools(itemPrices);
    showHistorys(itemPrices);
    exportExcel(itemPrices);
    setSellPrice(itemPrices);

    function tableInit(tabletools, tableOrig) {
        var distribution =
            jQuery('table#itemPrices')
            .on('preXhr.dt', function(e, settings, json) {
                $('#reflashTable').find('.fa').addClass("fa-spin");
                delete json.columns;
                delete json.order;
                delete json.search;
                var search = $('#searchoption').data("search");
                if (search) {
                    if (typeof(search) === "string") {
                        json.ItemPriceSearch = JSON.parse(search);
                    } else {
                        json.ItemPriceSearch = (search);
                    }
                }
            })
            .on('xhr.dt', function(e, settings, json, xhr) {
                $('#reflashTable').find('.fa').removeClass("fa-spin");
            })
            .DataTable({
                ajax: {
                    "url": "/ServiceItems/GetItemPrices",
                    "type": 'post',
                    "dataSrc": "data",
                    "dataFilter": function(a, b) {
                        var timestampNow = (new Date()).valueOf();
                        var dataAll = JSON.parse(a);
                        var i, j, priceListRef, inPriceList, otherPriceList, timeStartArrRef, timeEndArrRef, timeStartRef, timeEndRef;
                        for (i in dataAll.data) {
                            priceListRef = dataAll.data[i].PriceList;
                            inPriceList = [];
                            otherPriceList = [];
                            for (j in priceListRef) {
                                timeStartArrRef = priceListRef[j].StartDate.split("-");
                                timeEndArrRef = priceListRef[j].EndDate.split("-");
                                timeStartRef = (new Date(timeStartArrRef[0], parseInt(timeStartArrRef[1]) - 1, timeStartArrRef[2], 0, 0, 0)).valueOf();
                                timeEndRef = (new Date(timeEndArrRef[0], parseInt(timeEndArrRef[1]) - 1, timeEndArrRef[2], 23, 59, 59, 999)).valueOf();
                                if ((timestampNow >= timeStartRef) && (timestampNow <= timeEndRef)) {
                                    inPriceList.push(priceListRef[j]);
                                } else {
                                    otherPriceList.push(priceListRef[j]);
                                }
                            }
                            //时间先后排序
                            // otherPriceList.sort(function(theNext, theLast) {
                            //     var timeStartArrRef = theNext.StartDate.split("-");
                            //     var timeEndArrRef = theLast.EndDate.split("-");
                            //     var timeStartRef = (new Date(timeStartArrRef[0], parseInt(timeStartArrRef[1]) - 1, timeStartArrRef[2], 0, 0, 0)).valueOf();
                            //     var timeEndRef = (new Date(timeEndArrRef[0], parseInt(timeEndArrRef[1]) - 1, timeEndArrRef[2], 0, 0, 0, 0)).valueOf();
                            //     if (timeStartRef < timeEndRef) {
                            //         return false;
                            //     } else {
                            //         return true;
                            //     }
                            // });
                            dataAll.data[i].inPriceList = inPriceList;
                            dataAll.data[i].otherPriceList = otherPriceList;
                        }
                        return JSON.stringify(dataAll)
                    }
                },
                deferLoading: [0, 0],
                ordering: false,
                searching: false,
                serverSide: true,
                drawCallback: function(settings) {
                    var api = this.api();
                    var thistable = jQuery(this);
                    thistable.find("thead th .allrows").prop("checked", false).trigger("change");
                },
                initComplete: function(settings, json) {
                    var api = this.api();
                    var thistable = jQuery(this);
                    thistable.on('change', ".onerow", function() {
                        var count = thistable.find('.onerow:checked').length;
                        tabletools.find("#selectedNumber").text(count);
                        if ($(this).prop("checked")) {
                            $(this).closest('tr').addClass("selectedRow");
                        } else {
                            $(this).closest('tr').removeClass("selectedRow");
                        }
                        var _this = this;
                        var currentPageLength = api.page.info().end - api.page.info().start;
                        if (thistable.find("tbody tr td input:checked").length != currentPageLength) {
                            thistable.find(".allrows:eq(0)").prop("checked", false);
                        } else {
                            thistable.find(".allrows:eq(0)").prop("checked", true);
                        }
                    });
                    thistable.on('change', ".allrows", function() {
                        $(this).closest('table').find('tbody tr td input.checkboxes').prop('checked', $(this).prop("checked"));
                        if ($(this).prop("checked")) {
                            $(this).closest('table').find('tbody tr ').addClass("selectedRow");
                        } else {
                            $(this).closest('table').find('tbody tr ').removeClass("selectedRow");
                        }
                        var count = thistable.find('tbody tr td input.checkboxes:checked').length;
                        tabletools.eq(0).find("#selectedNumber").text(count);
                    });
                },
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
                    'data': 'ServiceCode',
                }, {
                    'targets': [1],
                    'data': 'cnItemName',
                }, {
                    'targets': [2],
                    'data': 'CityName',
                }, {
                    'targets': [3],
                    'data': 'ServiceItemEnableState',
                    'render': function(cellData, type, rowData, meta) {
                        if (cellData == 0) {
                            return "启用";
                        } else {
                            return "<span style='color:red'>禁用</span>";
                        }

                    }
                }, {
                    'targets': [4],
                    'data': "SupplierNo"
                }, {
                    'targets': [5],
                    'data': null,
                    'render': function(cellData, type, rowData, meta) {
                        if (rowData.inPriceList.length !== 0) {
                            if (rowData.otherPriceList.length === 0) {
                                return '<div>' + rowData.inPriceList[0].StartDate + '</div><div>' + rowData.inPriceList[0].EndDate + '</div>';
                            }
                            return (
                                '<div>' + rowData.inPriceList[0].StartDate + '</div>' +
                                '<div>' + rowData.inPriceList[0].EndDate + '</div>' +
                                '<div><a title="点击展开其它时间范围" class="otherTimeRange any-unselectable">' + '展开' + '</a></div>'
                            );
                        } else {
                            return '<div><a title="点击展开其它时间范围" class="otherTimeRange any-unselectable">' + '展开' + '</a></div>';
                        }
                    }
                }, {
                    'targets': [6],
                    'data': null,
                    'render': function(cellData, type, rowData, meta) {
                        if (rowData.inPriceList.length !== 0) {
                            if (rowData.PayType == 1) {
                                return null;
                            }
                            return (
                                '<div>' + rowData.inPriceList[0].AdultNetPrice + '</div>' +
                                '<div style="font-size:9px;color:#999999">' + rowData.CurrencyNo + '</div>'
                            )
                        } else {
                            return null;
                        }
                    }
                }, {
                    'targets': [7],
                    'data': null,
                    'render': function(cellData, type, rowData, meta) {
                        if (rowData.inPriceList.length !== 0) {
                            if (rowData.PayType == 1) {
                                return null;
                            }
                            return (
                                '<div>' + rowData.inPriceList[0].ChildNetPrice + '</div>' +
                                '<div style="font-size:9px;color:#999999">' + rowData.CurrencyNo + '</div>'
                            )
                        } else {
                            return null;
                        }
                    }
                }, {
                    'targets': [8],
                    'data': null,
                    'render': function(cellData, type, rowData, meta) {
                        if (rowData.inPriceList.length !== 0) {
                            if (rowData.PayType == 1) {
                                return null;
                            }
                            return (
                                '<div>' + rowData.inPriceList[0].BobyNetPrice + '</div>' +
                                '<div style="font-size:9px;color:#999999">' + rowData.CurrencyNo + '</div>'
                            )
                        } else {
                            return null;
                        }
                    }
                }, {
                    'targets': [9],
                    'data': null,
                    'render': function(cellData, type, rowData, meta) {
                        if (rowData.inPriceList.length !== 0) {
                            if (rowData.PayType == 0) {
                                return null;
                            }
                            return (
                                '<div>' + rowData.inPriceList[0].Price + '</div>' +
                                '<div style="font-size:9px;color:#999999">' + rowData.CurrencyNo + '</div>'
                            )
                        } else {
                            return null;
                        }
                    }
                }, {
                    'targets': [10],
                    'data': 'SellPrice',
                    'render': function(cellData, type, rowData, meta) {
                        if (cellData == null) {
                            return null;
                        }
                        return (
                            '<div>' + cellData + '</div>' +
                            '<div style="font-size:9px;color:#999999">RMB</div>'
                        )
                    }
                }, {
                    'targets': [11],
                    'data': null,
                    'render': function(cellData, type, rowData, meta) {
                        var profitX = 0;
                        var priceX = 0;
                        var profixRate = 0;
                        if (rowData.inPriceList.length !== 0) {
                            if (rowData.PayType === 0) {
                                // priceX = parseFloat(rowData.PriceList[0].AdultNetPrice);
                                // if (rowData.CurrencyChangeType === 0) {
                                //     profitX = parseFloat(rowData.PriceList[0].Profit) * parseFloat(rowData.ExchangeRate);
                                // } else {
                                //     profitX = parseFloat(rowData.PriceList[0].Profit) / parseFloat(rowData.ExchangeRate);
                                // }
                                priceX = parseFloat(rowData.SellPrice);
                                profitX = parseFloat(rowData.inPriceList[0].Profit);
                            } else {
                                // priceX = parseFloat(rowData.PriceList[0].Price);
                                // if (rowData.CurrencyChangeType === 0) {
                                //     profitX = parseFloat(rowData.PriceList[0].Profit) * parseFloat(rowData.ExchangeRate);
                                // } else {
                                //     profitX = parseFloat(rowData.PriceList[0].Profit) / parseFloat(rowData.ExchangeRate);
                                // }
                                priceX = parseFloat(rowData.SellPrice);
                                profitX = parseFloat(rowData.inPriceList[0].Profit);

                            }
                            if (priceX == 0) {
                                if (profitX == 0) {
                                    profixRate = 0;
                                } else {
                                    profixRate = -100;
                                }
                                profixRate = '';
                            } else {
                                profixRate = (profitX / priceX * 100).toFixed(0);
                                profixRate += '%';
                            }
                            return ('<div>' + rowData.inPriceList[0].Profit.toFixed(2) + '</div><div>' + profixRate + '</div>');
                        } else {
                            return null;
                        }
                    },
                    "createdCell": function(cell, cellData, rowData, rowIndex, colIndex) {
                        if (rowData.inPriceList.length !== 0) {
                            if (rowData.inPriceList[0].Profit < 0) {
                                $(cell).css({
                                    'background': 'red',
                                    'color': 'white'
                                });
                            }
                        }
                    }
                }, {
                    'targets': [12],
                    'data': 'ChildSellPrice',
                    'render': function(cellData, type, rowData, meta) {
                        if (rowData.PayType === 0) {
                            return (
                                '<div>' + rowData.ChildSellPrice + '</div>' +
                                '<div style="font-size:9px;color:#999999">RMB</div>');
                        } else {
                            return null;
                        }
                    }
                }, {
                    'targets': [13],
                    'data': null,
                    'render': function(cellData, type, rowData, meta) {
                        var profitX = 0;
                        var priceX = 0;
                        var profixRate = 0;
                        if (rowData.inPriceList.length !== 0) {
                            if (rowData.PayType === 0) {
                                // priceX = parseFloat(rowData.PriceList[0].ChildNetPrice);
                                // if (rowData.CurrencyChangeType === 0) {
                                //     profitX = parseFloat(rowData.PriceList[0].ChildProfit) * parseFloat(rowData.ExchangeRate);
                                // } else {
                                //     profitX = parseFloat(rowData.PriceList[0].ChildProfit) / parseFloat(rowData.ExchangeRate);
                                // }
                                priceX = parseFloat(rowData.ChildSellPrice);
                                profitX = parseFloat(rowData.inPriceList[0].ChildProfit)
                            } else {
                                return null;
                            }
                            if (priceX == 0) {
                                if (profitX == 0) {
                                    profixRate = 0;
                                } else {
                                    profixRate = -100;
                                }
                                profixRate = "";
                            } else {
                                profixRate = (profitX / priceX * 100).toFixed(0);
                                profixRate += '%';
                            }
                            return ('<div>' + rowData.inPriceList[0].ChildProfit.toFixed(2) + '</div><div>' + profixRate + '</div>');
                        } else {
                            return null;
                        }
                    },
                    "createdCell": function(cell, cellData, rowData, rowIndex, colIndex) {
                        if (rowData.inPriceList.length !== 0) {
                            if (rowData.inPriceList[0].ChildProfit < 0) {
                                $(cell).css({
                                    'background': 'red',
                                    'color': 'white'
                                });
                            }
                        }
                    }
                }, {
                    'targets': [14],
                    'data': 'ExtraServicePrices',
                    'render': function(cellData, type, rowData, meta) {
                        if (cellData == null) {
                            return null;
                        }
                        return (
                            '<a class="details-control any-unselectable">展开</a>'
                        )
                    }
                }, {
                    'targets': [15],
                    'data': 'IsChange',
                    'render': function(cellData, type, rowData, meta) {
                        if (cellData !== true) {
                            return '确认';
                        } else {
                            return '<a class="showhistorys" style="color:#6600FF !important;">待确认</a>';
                        }
                    }
                }, {
                    'targets': [16],
                    'data': null,
                    'render': function(cellData, type, rowData, meta) {
                        return (
                            '<div class="row">' +
                            '<a class="showhistorys" >详情</a>' + ' | ' +
                            '<a class="" target="_blank" href="/ServiceItems/PriceSetting?ItemID=' + rowData.ServiceItemID + '&SupplierID=' + rowData.SupplierID + '">修改</a>' +
                            '</div>' +
                            '<div class="row">' +
                            '<a class="editSellPrice" >卖价</a>' + ' | ' +
                            '<a target="_blank" href="/ServiceItems/PriceOperation/' + rowData.SupplierServiceItemID + '" >日志</a>' +
                            '</div>'
                        )
                    }
                }]
            });
        jQuery('table#itemPrices').on('click', 'td .details-control', function() {
            var div = $("<div></div>");
            var tr = $(this).closest('tr');
            var row = distribution.row(tr);
            var rowChild = row.child();
            if (rowChild) {
                if (row.child.isShown()) {
                    if (tr.hasClass("extraListShown")) {
                        if (tr.hasClass("otherPriceListShown")) {} else {
                            row.child.hide();
                            tr.removeClass('shown');
                        }
                        $(rowChild).find("#childAll .extraList:eq(0)").addClass("hidden")
                        tr.removeClass('extraListShown');
                        $(this).text("展开");
                    } else {
                        if ($(rowChild).find("#childAll .extraList:eq(0)").length === 0) {
                            var childAll = $(rowChild).find("#childAll");
                            row.child(childAll.append(makeDetail(row.data(), tableOrig, tr.hasClass("even"))));
                        } else {
                            $(rowChild).find("#childAll .extraList:eq(0)").removeClass("hidden")
                        }
                        row.child.show();
                        tr.addClass('extraListShown');
                        $(this).text("收起");
                    }
                } else {
                    if ($(rowChild).find("#childAll .extraList:eq(0)").length === 0) {
                        var childAll = $(rowChild).find("#childAll");
                        row.child(childAll.append(makeDetail(row.data(), tableOrig, tr.hasClass("even"))));
                    } else {
                        $(rowChild).find("#childAll .extraList:eq(0)").removeClass("hidden")
                    }
                    row.child.show();
                    tr.addClass('shown extraListShown');
                    $(this).text("收起");
                }
            } else {
                row.child(div.clone().attr("id", 'childAll').append(makeDetail(row.data(), tableOrig, tr.hasClass("even")))).show();
                tr.addClass('shown extraListShown');
                $(this).text("收起");
            }
        });
        jQuery('table#itemPrices').on('click', 'td .otherTimeRange', function() {
            var div = $("<div></div>");
            var tr = $(this).closest('tr');
            var row = distribution.row(tr);
            var rowChild = row.child();
            if (rowChild) {
                if (row.child.isShown()) {
                    if (tr.hasClass("otherPriceListShown")) {
                        if (tr.hasClass("extraListShown")) {

                        } else {
                            row.child.hide();
                            tr.removeClass('shown');
                        }
                        $(rowChild).find("#childAll .otherPriceList:eq(0)").addClass("hidden")
                        tr.removeClass('otherPriceListShown');
                        $(this).text("展开");
                    } else {
                        if ($(rowChild).find("#childAll .otherPriceList:eq(0)").length === 0) {
                            var childAll = $(rowChild).find("#childAll");
                            row.child(childAll.prepend(makeChildrowsOtherRange(row.data(), tableOrig, tr.hasClass("even"))));
                        } else {
                            $(rowChild).find("#childAll .otherPriceList:eq(0)").removeClass("hidden")
                        }
                        row.child.show();
                        tr.addClass('otherPriceListShown');
                        $(this).text("收起");
                    }
                } else {
                    if ($(rowChild).find("#childAll .otherPriceList:eq(0)").length === 0) {
                        var childAll = $(rowChild).find("#childAll");
                        row.child(childAll.prepend(makeChildrowsOtherRange(row.data(), tableOrig, tr.hasClass("even"))));
                    } else {
                        $(rowChild).find("#childAll .otherPriceList:eq(0)").removeClass("hidden")
                    }
                    row.child.show();
                    tr.addClass('shown otherPriceListShown');
                    $(this).text("收起");

                }
            } else {
                row.child(div.clone().attr("id", 'childAll').prepend(makeChildrowsOtherRange(row.data(), tableOrig, tr.hasClass("even")))).show();
                tr.addClass('shown otherPriceListShown');
                $(this).text("收起");

            }
        });
        return {
            'dataTableRef': distribution,
            'jQueryRef': jQuery('table#itemPrices')
        };
    }

    function makeChildrowsOtherRange(rowData, tableRef, isEven) {
        var divx = $("<div></div>");
        var table = $("<table class='otherPriceList  table-lh  table-lh-bordered-white table-lh-bordered-edge-remove table-lh-td-no table-lh-margin-no'><colgroup></colgroup><tbody></tbody></table>");
        table.css({ "width": "100%", "border-collapse": "collapse" });
        table.find("colgroup:eq(0)").append(tableRef.find("colgroup:eq(0)").html());
        var trtemp, tdtemp;
        var profitRate;
        var i, j, index;
        var onProfit;
        var tr = $("<tr></tr>");
        if (isEven) {
            tr.css("cssText", " background:#f9f9f9 ");
        } else {
            tr.css("cssText", "background:white ");
        }
        var td = $("<td></td>");
        td.css({ "border-top": "0px solid white" });
        index = 0;
        var otherPriceList = rowData.otherPriceList;
        for (i in otherPriceList) {
            trtemp = tr.clone();
            for (j = 0; j < 5; j++) {
                trtemp.append(td.clone());
            }
            trtemp.append(
                td.clone().append(
                    divx.clone().text(otherPriceList[i].StartDate),
                    divx.clone().text(otherPriceList[i].EndDate)
                )
            );
            if (parseInt(rowData.PayType) === 1) {
                trtemp.append(td.clone());
                trtemp.append(td.clone());
                trtemp.append(td.clone());
                trtemp.append(
                    td.clone().append(
                        divx.clone().text(otherPriceList[i].Price),
                        divx.clone().css("color", "#999999").text(rowData.CurrencyNo)
                    )
                );
            } else {
                trtemp.append(
                    td.clone().append(
                        divx.clone().text(otherPriceList[i].AdultNetPrice),
                        divx.clone().css("color", "#999999").text(rowData.CurrencyNo)
                    )
                );
                trtemp.append(
                    td.clone().append(
                        divx.clone().text(otherPriceList[i].ChildNetPrice),
                        divx.clone().css("color", "#999999").text(rowData.CurrencyNo)
                    )
                );
                trtemp.append(
                    td.clone().append(
                        divx.clone().text(otherPriceList[i].BobyNetPrice),
                        divx.clone().css("color", "#999999").text(rowData.CurrencyNo)
                    )
                );
                trtemp.append(td.clone());
            }
            trtemp.append(td.clone().append( //成人卖价或是单价
                divx.clone().text(rowData.SellPrice),
                divx.clone().css("color", "#999999").text("RMB")
            ));
            onProfit = (function() { ///成人利润
                var profitX = 0;
                var priceX = 0;
                var profixRate = 0;
                if (1) {
                    if (rowData.PayType === 0) {

                        priceX = parseFloat(rowData.SellPrice);
                        profitX = parseFloat(otherPriceList[i].Profit);
                    } else {
                        priceX = parseFloat(rowData.SellPrice);
                        profitX = parseFloat(otherPriceList[i].Profit);

                    }
                    if (priceX == 0) {
                        if (profitX == 0) {
                            profixRate = 0;
                        } else {
                            profixRate = -100;
                        }
                        profixRate = '';
                    } else {
                        profixRate = (profitX / priceX * 100).toFixed(0);
                        profixRate += '%';
                    }
                    return ('<div>' + otherPriceList[i].Profit.toFixed(2) + '</div><div>' + profixRate + '</div>');
                } else {
                    return null;
                }
            })();
            tdtemp = td.clone();
            if (parseFloat(otherPriceList[i].Profit) < 0) {
                tdtemp.css({
                    'background': 'red',
                    'color': 'white'
                });
            };
            trtemp.append(tdtemp.append(onProfit)); ///成人利润
            if (rowData.PayType === 0) {
                trtemp.append(td.clone().append(
                    divx.clone().text(rowData.ChildSellPrice),
                    divx.clone().css("color", "#999999").text("RMB")
                ));
            } else {
                trtemp.append(td.clone());
            }
            onProfit = (function() {
                var profitX = 0;
                var priceX = 0;
                var profixRate = 0;
                if (1) {
                    if (rowData.PayType === 0) {
                        priceX = parseFloat(rowData.ChildSellPrice);
                        profitX = parseFloat(otherPriceList[i].ChildProfit)
                    } else {
                        return null;
                    }
                    if (priceX == 0) {
                        if (profitX == 0) {
                            profixRate = 0;
                        } else {
                            profixRate = -100;
                        }
                        profixRate = "";
                    } else {
                        profixRate = (profitX / priceX * 100).toFixed(0);
                        profixRate += '%';
                    }
                    return ('<div>' + otherPriceList[i].ChildProfit.toFixed(2) + '</div><div>' + profixRate + '</div>');
                } else {
                    return null;
                }
            })();
            tdtemp = td.clone();
            if (parseFloat(otherPriceList[i].ChildProfit) < 0) {
                tdtemp.css({
                    'background': 'red',
                    'color': 'white'
                });
            };
            trtemp.append(tdtemp.append(onProfit)); //儿童利润
            for (j = 0; j < 3; j++) {
                trtemp.append(td.clone());
            }
            table.find('tbody').append(trtemp);
        }
        return table;
    }

    function makeDetail(rowData, tableRef, isEven) {
        var divx = $("<div></div>");
        var table = $("<table class='extraList  table-lh  table-lh-bordered-white table-lh-bordered-edge-remove table-lh-td-no table-lh-margin-no'><colgroup></colgroup><tbody></tbody></table>");
        table.css({ "width": "100%", "border-collapse": "collapse" });
        table.find("colgroup:eq(0)").append(tableRef.find("colgroup:eq(0)").html());
        var trtemp, tdtemp;
        var profitRate;
        var i, j, index;
        var tr = $("<tr></tr>");
        if (isEven) {
            tr.css("cssText", " background:#f9f9f9 ");
        } else {
            tr.css("cssText", "background:white ");
        }
        var td = $("<td></td>");
        td.css({ "border-top": "0px solid white" });
        index = 0;
        var ExtraServicePrices = rowData.ExtraServicePrices;
        for (i in ExtraServicePrices) {
            trtemp = tr.clone();
            trtemp.append(td.clone());
            trtemp.append(td.clone().text(ExtraServicePrices[i].ServiceName));
            for (j = 0; j < 7; j++) {
                trtemp.append(td.clone());
            }
            trtemp.append(
                td.clone().append(divx.clone().text(ExtraServicePrices[i].ServicePrice)).append(divx.clone().css("color", "#999999").text(rowData.CurrencyNo))
            );
            trtemp.append(
                td.clone().append(divx.clone().text(ExtraServicePrices[i].ServiceSellPrice), divx.clone().css("color", "#999999").text("RMB"))
            );
            tdtemp = td.clone();
            if (ExtraServicePrices[i].ServiceProfit < 0) {
                tdtemp.css({
                    'background': 'red',
                    'color': 'white'
                });
            }
            if (ExtraServicePrices[i].ServiceSellPrice == 0) {
                if (ExtraServicePrices[i].ServiceProfit == 0) {
                    profitRate = 0 + '%';
                } else {
                    profitRate = -100 + '%';
                }
                profitRate = "";
            } else {
                profitRate = (parseFloat(ExtraServicePrices[i].ServiceProfit) / parseFloat(ExtraServicePrices[i].ServiceSellPrice) * 100).toFixed(0);
                profitRate += '%';
            }
            trtemp.append(
                tdtemp.append(divx.clone().text(ExtraServicePrices[i].ServiceProfit), divx.clone().text(profitRate))
            );
            for (j = 0; j < 5; j++) {
                trtemp.append(td.clone());
            }
            table.find('tbody').append(trtemp);
        }

        return table;
    }

    function fix() {
        if (jQuery("#searchoption").length === 0) {
            var search = jQuery('<div id="searchoption" ></div>').data("search", {
                "SupplierID": "0",
                "FuzzySearch": "",
                "ServiceTypeID": "0",
                'IsChange': false,
            })
            $('body').append(search);
        };


        $('body').on('change', '.price-format', function() {
            var editValue = $(this).val();
            var editValueTemp = $.trim(editValue);
            var afterReplace = editValueTemp.replace(/[^0-9\.]/g, "");
            if (afterReplace.length !== 0) {
                var numberTest = /^[0-9]+(\.[0-9]*)?$/;
                if (numberTest.test(afterReplace)) {
                    var tmp = afterReplace.match('.');
                    if (tmp === null) {
                        $(this).val(afterReplace);
                    } else {
                        var tmpARR = afterReplace.split('.');
                        if (tmpARR.length === 1) {
                            $(this).val(tmpARR[0]);
                        } else {
                            if (tmpARR[1].toString().length > 2) {
                                $(this).val(Number(afterReplace).toFixed(2));
                            } else {
                                if (tmpARR[1].toString().length === 0) {
                                    $(this).val(tmpARR[0]);
                                } else {
                                    $(this).val(afterReplace);
                                }
                            }
                        }
                    }
                } else {
                    $(this).val("");
                }
            } else {
                $(this).val("");
            }
        });
    }



    function initTabletools(Refs) {
        $('.tabletools:eq(0)').find('#search').one("click", function searching(e) {
            var FuzzySearch = $('.tabletools:eq(0)').find("#FuzzySearch").val();
            var cancel = false;
            if (!FuzzySearch) {
                $('.tabletools:eq(0)').find("#FuzzySearch").success("请您选择供应商或者是输入搜索");
                $(this).one("click", searching);
                cancel = true;
            }
            if (cancel === true) {
                $(this).one("click", searching);
                return;
            }
            var search = {
                "SupplierID": 0,
                "FuzzySearch": FuzzySearch,
                "ServiceTypeID": 0
            };
            $('.tabletools:eq(0)').find('#SupplierID').val(0);
            $('.tabletools:eq(0)').find('#CityID').val("0");
            $('.tabletools:eq(0)').find('#ServiceType .buttonradio:eq(0)').addClass('active').siblings().removeClass('active');
            $('.tabletools:eq(0)').find('#ChangeButton .buttonradio').removeClass('active');
            $('#searchoption').data('search', JSON.stringify(search));
            Refs.dataTableRef.draw();
            $(this).one("click", searching);
        });

        $('.tabletools:eq(0)').find('#FuzzySearch').bind("keydown", function searching(evt) {
            evt = (evt) ? evt : ((window.event) ? window.event : "") //兼容IE和Firefox获得keyBoardEvent对象
            var key = evt.keyCode ? evt.keyCode : evt.which; //兼容IE和Firefox获得keyBoardEvent对象的键值
            if (key == 13) {
                $(this).closest("span").siblings("#search").trigger("click");
            }
        });

        $('.tabletools:eq(0)').find('#SupplierID').one("change", function searching(e) {
            var SupplierID = $(this).val();
            var cancel = false;
            if (SupplierID == 0) {
                $(this).success("请您选择供应商或者是输入搜索");
                $(this).one("change", searching);
                cancel = true;
            }
            if (cancel === true) {
                return;
            }
            var search = {
                "SupplierID": SupplierID,
                "FuzzySearch": "",
                "ServiceTypeID": 0
            };
            $('.tabletools:eq(0)').find('#FuzzySearch').val("");
            $('.tabletools:eq(0)').find('#CityID').val("0");
            $('.tabletools:eq(0)').find('#ServiceType .buttonradio:eq(0)').addClass('active').siblings().removeClass('active');
            $('.tabletools:eq(0)').find('#ChangeButton .buttonradio').removeClass('active');

            $('#searchoption').data('search', JSON.stringify(search));
            Refs.dataTableRef.draw();
            $(this).one("change", searching);
        });

        $('.tabletools:eq(0)').find('#CityID').one("change", function searching(e) {
            var CityID = $(this).val();
            var cancel = false;
            if (CityID == 0) {
                $(this).success("请您选择目的地或者是输入搜索");
                $(this).one("change", searching);
                cancel = true;
            }
            if (cancel === true) {
                return;
            }
            var search = {
                "CityID": CityID,
                "FuzzySearch": "",
                "ServiceTypeID": 0
            };
            $('.tabletools:eq(0)').find('#FuzzySearch').val("");
            $('.tabletools:eq(0)').find('#SupplierID').val("0");
            $('.tabletools:eq(0)').find('#ServiceType .buttonradio:eq(0)').addClass('active').siblings().removeClass('active');
            $('.tabletools:eq(0)').find('#ChangeButton .buttonradio').removeClass('active');

            $('#searchoption').data('search', JSON.stringify(search));
            Refs.dataTableRef.draw();
            $(this).one("change", searching);
        });

        $('.tabletools:eq(0)').find('#ServiceType').ButtonRadio({
            data: [],
            befoeSelected: function() {
                var search = $('#searchoption').data("search");
                if (search) {
                    if (typeof(search) === "string") {
                        search = JSON.parse(search);
                    }
                }
                if ((search.SupplierID == 0) && (!search.FuzzySearch)) {
                    $('.tabletools:eq(0)').find('#mainSearch').success('请您先选择供应商或者进行搜索');
                    return false;
                }
                return true;
            },
            selected: function(dom, code) {
                var search = $('#searchoption').data("search");
                if (search) {
                    if (typeof(search) === "string") {
                        search = JSON.parse(search);
                    }
                }
                search.ServiceTypeID = code;
                $('#searchoption').data('search', JSON.stringify(search));
                Refs.dataTableRef.draw();
            }

        });

        $('.tabletools:eq(0)').find('#reflashTable').bind("click", function() {
            var search = $('#searchoption').data("search");
            if (search) {
                if (typeof(search) === "string") {
                    search = JSON.parse(search);
                }
            }
            if ((search.SupplierID == 0) && (!search.FuzzySearch)) {
                $('.tabletools:eq(0)').find('#mainSearch').success('请您先选择供应商或者进行搜索');
                return;
            }
            Refs.dataTableRef.draw();
        });

        $('.tabletools:eq(0)').find('#ChangeButton').ButtonRadio({
            data: [],
            selected: function(dom, code) {

                $('#searchoption').data('search', JSON.stringify({
                    "IsChange": true
                }));
                Refs.dataTableRef.draw();
                $('.tabletools:eq(0)').find('#ServiceType .buttonradio:eq(0)').addClass('active').siblings().removeClass('active');
                $('.tabletools:eq(0)').find('#FuzzySearch').text("");
                $('.tabletools:eq(0)').find('#SupplierID').val(0);

            }

        })


    }

    function showHistorys(tableRef) {
        var prices = $("#historyDisplay #prices").DataTable({
            "serverSide": false,
            "ordering": false,
            "pageLength": 1000,
            "stateSave": false,
            "dom": "<t>",
            "createdRow": function(row, data, dataIndex) {
                if (data.Hilight === true) {
                    $(row).css("background", '#FFFFCC');
                }

            },
            'columnDefs': [{
                'targets': [0],
                'data': null,
                'render': function(cellData, type, rowData, meta) {
                    return (parseInt(meta.row) + 1);
                }
            }, {
                'targets': [1],
                'data': 'startTime',
                'render': function(cellData, type, rowData, meta) {
                    var theTime = cellData ? cellData.toString().split('T')[0] : "";
                    var theChangeTime = rowData.startTimeChange ? rowData.startTimeChange.toString().split('T')[0] : "";
                    if (rowData.isChanging === false) {
                        return theTime;
                    } else if (rowData.isChanging === 'new') {
                        rowData.Hilight = true;
                        return ('<div class="text-price-changing">' + theChangeTime + '</div>');
                    } else {
                        if (theTime == theChangeTime) {
                            return theTime;
                        } else {
                            rowData.Hilight = true;
                            return ('<div class="text-price-changing">' + theChangeTime + '</div>' +
                                '<div class="text-price-deleting">' + theTime + '</div>'
                            )
                        }
                    }
                }

            }, {
                'targets': [2],
                'data': 'EndTime',
                'render': function(cellData, type, rowData, meta) {
                    var theTime = cellData ? cellData.toString().split('T')[0] : "";
                    var theChangeTime = rowData.EndTimeChange ? rowData.EndTimeChange.toString().split('T')[0] : "";
                    if (rowData.isChanging === false) {
                        return theTime;
                    } else if (rowData.isChanging === 'new') {
                        rowData.Hilight = true;
                        return ('<div class="text-price-changing">' + theChangeTime + '</div>');
                    } else {
                        if (theTime == theChangeTime) {
                            return theTime;
                        } else {
                            rowData.Hilight = true;
                            return ('<div class="text-price-changing">' + theChangeTime + '</div>' +
                                '<div class="text-price-deleting">' + theTime + '</div>'
                            )
                        }
                    }
                }
            }, {
                'targets': [3],
                'data': "AdultNetPrice",
                'render': function(cellData, type, rowData, meta) {
                    var cellDataAlt = cellData;
                    // if (rowData.PayType === 0) {
                    //     if (rowData.isChanging === false) {

                    //         return cellDataAlt;
                    //     } else if (rowData.isChanging === 'new') {
                    //         return ('<div class="text-price-changing">' + rowData.AdultNetPriceChange + '</div>');
                    //     } else {
                    //         if (rowData.AdultNetPriceChange == rowData.AdultNetPrice) {
                    //             return cellDataAlt;
                    //         } else {
                    //             return ('<div class="text-price-changing">' + rowData.AdultNetPriceChange + '</div>' +
                    //                 '<div class="text-price-deleting">' + cellDataAlt + '</div>'
                    //             )
                    //         }
                    //     }
                    // } else {
                    //     return null;
                    // }

                    if (rowData.isChanging === false) {
                        if (rowData.PayType === 0) {
                            return cellDataAlt;
                        } else {
                            return null;
                        }
                    } else if (rowData.isChanging === 'new') {
                        if (rowData.PayTypeChange === 0) {
                            rowData.Hilight = true;
                            return ('<div class="text-price-changing">' + rowData.AdultNetPriceChange + '</div>');
                        } else {
                            return null;
                        }

                    } else {
                        if (rowData.PayType !== rowData.PayTypeChange) {
                            if (rowData.PayTypeChange === 1) {
                                return '<div class="text-price-deleting">' + cellDataAlt + '</div>';
                            } else {
                                rowData.Hilight = true;
                                return ('<div class="text-price-changing">' + rowData.AdultNetPriceChange + '</div>');
                            }

                        } else {
                            if (rowData.PayType === 0) {
                                if (rowData.AdultNetPriceChange == rowData.AdultNetPrice) {
                                    return cellDataAlt;
                                } else {
                                    rowData.Hilight = true;
                                    return ('<div class="text-price-changing">' + rowData.AdultNetPriceChange + '</div>' +
                                        '<div class="text-price-deleting">' + cellDataAlt + '</div>'
                                    )
                                }
                            } else {
                                return null;
                            }
                        }
                    }
                }
            }, {
                'targets': [4],
                'data': 'ChildNetPrice',
                'render': function(cellData, type, rowData, meta) {
                    var cellDataAlt = cellData;
                    // if (rowData.PayType === 0) {
                    //     if (rowData.isChanging === false) {
                    //         return cellDataAlt;
                    //     } else if (rowData.isChanging === 'new') {
                    //         return ('<div class="text-price-changing">' + rowData.ChildNetPriceChange + '</div>');
                    //     } else {
                    //         if (rowData.ChildNetPriceChange == rowData.ChildNetPrice) {
                    //             return cellDataAlt;
                    //         } else {
                    //             return ('<div class="text-price-changing">' + rowData.ChildNetPriceChange + '</div>' +
                    //                 '<div class="text-price-deleting">' + cellDataAlt + '</div>'
                    //             )
                    //         }
                    //     }
                    // } else {
                    //     return null;
                    // }

                    if (rowData.isChanging === false) {
                        if (rowData.PayType === 0) {
                            return cellDataAlt;
                        } else {
                            return null;
                        }

                    } else if (rowData.isChanging === 'new') {
                        if (rowData.PayTypeChange === 0) {
                            rowData.Hilight = true;
                            return ('<div class="text-price-changing">' + rowData.ChildNetPriceChange + '</div>');
                        } else {
                            return null;
                        }
                    } else {
                        if (rowData.PayType !== rowData.PayTypeChange) {
                            if (rowData.PayTypeChange === 1) {
                                return '<div class="text-price-deleting">' + cellDataAlt + '</div>';
                            } else {
                                rowData.Hilight = true;
                                return ('<div class="text-price-changing">' + rowData.ChildNetPriceChange + '</div>');
                            }
                        } else {
                            if (rowData.PayType === 0) {
                                if (rowData.ChildNetPriceChange == rowData.ChildNetPrice) {
                                    return cellDataAlt;
                                } else {
                                    rowData.Hilight = true;
                                    return ('<div class="text-price-changing">' + rowData.ChildNetPriceChange + '</div>' +
                                        '<div class="text-price-deleting">' + cellDataAlt + '</div>'
                                    )
                                }
                            } else {
                                return null;
                            }
                        }
                    }
                }
            }, {
                'targets': [5],
                'data': 'BobyNetPrice',
                'render': function(cellData, type, rowData, meta) {
                    var cellDataAlt = cellData;
                    // if (rowData.PayType === 0) {
                    //     if (rowData.isChanging === false) {
                    //         return cellDataAlt;
                    //     } else if (rowData.isChanging === 'new') {
                    //         return ('<div class="text-price-changing">' + rowData.BobyNetPriceChange + '</div>');
                    //     } else {
                    //         if (rowData.BobyNetPriceChange == rowData.BobyNetPrice) {
                    //             return cellDataAlt;
                    //         } else {
                    //             return ('<div class="text-price-changing">' + rowData.BobyNetPriceChange + '</div>' +
                    //                 '<div class="text-price-deleting">' + cellDataAlt + '</div>'
                    //             )
                    //         }
                    //     }
                    // } else {
                    //     return null;
                    // }

                    if (rowData.isChanging === false) {
                        if (rowData.PayType === 0) {
                            return cellDataAlt;
                        } else {
                            return null;
                        }
                    } else if (rowData.isChanging === 'new') {
                        if (rowData.PayTypeChange === 0) {
                            rowData.Hilight = true;
                            return ('<div class="text-price-changing">' + rowData.BobyNetPriceChange + '</div>');
                        } else {
                            return null;
                        }
                    } else {
                        if (rowData.PayType !== rowData.PayTypeChange) {
                            if (rowData.PayTypeChange === 1) {
                                return '<div class="text-price-deleting">' + cellDataAlt + '</div>';
                            } else {
                                rowData.Hilight = true;
                                return ('<div class="text-price-changing">' + rowData.BobyNetPriceChange + '</div>');
                            }
                        } else {
                            if (rowData.PayType === 0) {
                                if (rowData.BobyNetPriceChange == rowData.BobyNetPrice) {
                                    return cellDataAlt;
                                } else {
                                    rowData.Hilight = true;
                                    return ('<div class="text-price-changing">' + rowData.BobyNetPriceChange + '</div>' +
                                        '<div class="text-price-deleting">' + cellDataAlt + '</div>'
                                    )
                                }
                            } else {
                                return null;
                            }
                        }

                    }
                }
            }, {
                'targets': [6],
                'data': 'Price',
                'render': function(cellData, type, rowData, meta) {
                    var cellDataAlt = cellData;
                    // if (rowData.PayType === 1) {
                    //     if (rowData.isChanging === false) {

                    //         return cellDataAlt;
                    //     } else if (rowData.isChanging === 'new') {
                    //         return ('<div class="text-price-changing">' + rowData.PriceChange + '</div>');
                    //     } else {
                    //         if (rowData.PriceChange == rowData.Price) {
                    //             return cellDataAlt;
                    //         } else {
                    //             return ('<div class="text-price-changing">' + rowData.PriceChange + '</div>' +
                    //                 '<div class="text-price-deleting">' + cellDataAlt + '</div>'
                    //             )
                    //         }
                    //     }
                    // } else {
                    //     return null;
                    // }

                    if (rowData.isChanging === false) {
                        if (rowData.PayType === 1) {
                            return cellDataAlt;
                        } else {
                            return null;
                        }
                    } else if (rowData.isChanging === 'new') {
                        if (rowData.PayTypeChange === 1) {
                            rowData.Hilight = true;
                            return ('<div class="text-price-changing">' + rowData.PriceChange + '</div>');
                        } else {
                            return null;
                        }
                    } else {
                        if (rowData.PayType !== rowData.PayTypeChange) {
                            if (rowData.PayTypeChange === 0) {
                                return '<div class="text-price-deleting">' + cellDataAlt + '</div>';
                            } else {
                                rowData.Hilight = true;
                                return ('<div class="text-price-changing">' + rowData.PriceChange + '</div>');
                            }
                        } else {
                            if (rowData.PayType === 1) {
                                if (rowData.PriceChange == rowData.Price) {
                                    return cellDataAlt;
                                } else {
                                    rowData.Hilight = true;
                                    return ('<div class="text-price-changing">' + rowData.PriceChange + '</div>' +
                                        '<div class="text-price-deleting">' + cellDataAlt + '</div>'
                                    )
                                }
                            } else {
                                return null;
                            }
                        }
                    }
                }
            }]
        });
        var extraPrices = $("#historyDisplay #extraPrices").DataTable({
            "serverSide": false,
            "ordering": false,
            "pageLength": 1000,
            "stateSave": false,
            "dom": "<t>",
            'language': {
                "emptyTable": "没有附加项目"
            },
            'createdRow': function(row, data, dataIndex) {
                if (data.Hilight === true) {
                    $(row).css('background', '#FFFFCC');

                }
            },
            'columnDefs': [{
                'targets': [0],
                'data': null,
                'render': function(cellData, type, rowData, meta) {
                    return (parseInt(meta.row) + 1);
                }
            }, {
                'targets': [1],
                'data': 'ServiceName',
            }, {
                'targets': [2],
                'data': "ServicePrice",
                'render': function(cellData, type, rowData, meta) {
                    if (rowData.type === "notChanging") {
                        return (
                            '<div>' + cellData + '</div>'
                        )
                    } else if (rowData === "new") {
                        rowData.Hilight = true;
                        return (
                            '<div class="text-price-changing">' + rowData.ServicePriceChange + '</div>'
                        )
                    } else { //变更中
                        if (rowData.ServicePrice != rowData.ServicePriceChange) {
                            rowData.Hilight = true;
                            return ('<div class="text-price-changing">' + rowData.ServicePriceChange + '</div>' +
                                '<div class="text-price-deleting">' + rowData.ServicePrice + '</div>'
                            );
                        } else {
                            return '<div>' + cellData + '</div>';
                        }

                    }

                }
            }]
        });
        tableRef.jQueryRef.on("click", '.showhistorys', { "prices": prices, "extraPrices": extraPrices, "tableOrig": tableRef.dataTableRef }, function(e) {
            var tr = $(this).closest('tr');
            var row = e.data.tableOrig.row(tr);
            var rowData = row.data();
            $.ajax({
                url: "/ServiceItems/GetPrice",
                type: 'get',
                contentType: "application/json; charset=utf-8;",
                data: {
                    'ItemID': rowData.ServiceItemID,
                    'SupplierID': rowData.SupplierID
                },
                dataType: 'json',
                success: function(data) {
                    var priceExtraPrice =
                        detailPrehandle({
                            'data': data.data
                        });
                    console.log(priceExtraPrice);
                    e.data.prices.rows().remove();
                    e.data.prices.rows.add(sortByDate(priceExtraPrice.priceList));
                    e.data.prices.draw();
                    if (priceExtraPrice.extraPriceList.length === 0) {
                        $('#historyDisplay #extraALL').hide();
                    } else {
                        $('#historyDisplay #extraALL').show();
                    }
                    e.data.extraPrices.rows().remove();
                    e.data.extraPrices.rows.add(priceExtraPrice.extraPriceList);
                    e.data.extraPrices.draw();

                    $("#historyDisplay #whichPrice").text("（" + data.data.baseinfo.SupplierNo + "）" + data.data.baseinfo.cnItemName + data.data.baseinfo.ServiceCode);
                    var others = priceExtraPrice.others;
                    var arr = ['PayType',
                        'SelectEffectiveWay',
                        'CurrencyName',
                    ];
                    var div = $('<div></div>');
                    div.css("line-height", "18px");
                    var fix = "Change";
                    for (var i in arr) {
                        $('#historyDisplay #' + arr[i]).empty();
                        if (arr[i] in others) {
                            if ((arr[i] + fix) in others) {
                                if (others[arr[i]] === others[arr[i] + fix]) {
                                    $('#historyDisplay #' + arr[i]).append(div.clone().text(others[arr[i]]));
                                } else {
                                    $('#historyDisplay #' + arr[i]).append(div.clone().addClass('text-price-changing').text(others[arr[i] + fix]));
                                    $('#historyDisplay #' + arr[i]).append(div.clone().text(others[arr[i]]).addClass("text-price-deleting"));
                                }

                            } else {
                                $('#historyDisplay #' + arr[i]).append(div.clone().text(others[arr[i]]));
                            }
                        } else {
                            if ((arr[i] + fix) in others) {

                                $('#historyDisplay #' + arr[i]).append(div.clone().addClass('text-price-changing').text(others[arr[i] + fix]));

                            } else {

                            }
                        }
                    }
                    var textarea = $('<textarea></textarea>').addClass('form-control');
                    textarea.prop("readonly", true);
                    textarea.css("background", 'white');

                    $('#historyDisplay #Remark .col-md-6:eq(0)').empty();
                    $('#historyDisplay #Remark .col-md-6:eq(1)').empty();
                    if ('Remark' in others) {
                        if ('RemarkChange' in others) {
                            if (others['Remark'] === others['RemarkChange']) {
                                $('#historyDisplay #Remark .col-md-6:eq(0)').append(textarea.clone().val(others.Remark ? others.Remark : ""));
                            } else {
                                $('#historyDisplay #Remark .col-md-6:eq(1)').append(textarea.clone().addClass('text-price-deleting').val(others.Remark ? others.Remark : ""));
                                $('#historyDisplay #Remark .col-md-6:eq(0)').append(textarea.clone().addClass('text-price-changing').val(others.RemarkChange ? others.RemarkChange : ""));
                            }
                        } else {
                            $('#historyDisplay #Remark .col-md-6:eq(0)').append(textarea.clone().val(others.Remark ? others.Remark : ""));
                        }
                    } else {
                        if ('RemarkChange' in others) {
                            $('#historyDisplay #Remark .col-md-6:eq(0)').append(textarea.clone().addClass('text-price-changing').val(others.RemarkChange ? others.RemarkChange : ""));
                        } else {

                        }
                    }
                    $('#historyDisplay').modal('show');

                }
            });
            // var i;
            // var tr = $(this).closest('tr');
            // var row = e.data.tableOrig.row(tr);
            // var CurrencyNo = row.data().CurrencyNo;
            // var history = row.data().PriceList;
            // for (i in history) {
            //     history[i].CurrencyNo = CurrencyNo;
            // }
            // e.data.history.rows().remove();
            // e.data.history.rows.add(history);
            // e.data.history.draw();
            // $('#historyDisplay').modal('show');
        });
    }

    function detailPrehandle(obj) {
        var data = obj.data;
        var price, i;
        var priceList = [];
        var items = data.item;
        var changeitems = data.changeitem;
        if (items !== null) {
            if (items.IsChange === true && changeitems === null) {
                items = data.changeitem;
                changeitems = data.item;
            }
        }

        var itemsObjById = {};
        var changeitemsObjById = {};
        if (items !== null) {
            for (i in items.ItemPriceBySuppliers) {
                itemsObjById[items.ItemPriceBySuppliers[i].ItemPriceBySupplierID] = items.ItemPriceBySuppliers[i];
                itemsObjById[items.ItemPriceBySuppliers[i].ItemPriceBySupplierID].PayType = items.PayType;

            }
        }
        if (changeitems !== null) {
            for (i in changeitems.ItemPriceBySuppliers) {
                if (changeitems.ItemPriceBySuppliers[i].ItemPriceBySupplierID == 0) { //坑s
                    changeitemsObjById['new' + changeitems.ItemPriceBySuppliers[i].ItemPriceBySupplierChangeID] = changeitems.ItemPriceBySuppliers[i];
                    changeitemsObjById['new' + changeitems.ItemPriceBySuppliers[i].ItemPriceBySupplierChangeID].PayType = changeitems.PayType;
                } else {
                    changeitemsObjById[changeitems.ItemPriceBySuppliers[i].ItemPriceBySupplierID] = changeitems.ItemPriceBySuppliers[i];
                    changeitemsObjById[changeitems.ItemPriceBySuppliers[i].ItemPriceBySupplierID].PayType = changeitems.PayType;
                }

            }
        }
        console.log(items);
        console.log(changeitems);
        for (i in changeitemsObjById) {
            if (!(i in itemsObjById)) { //新增
                price = new Price({
                    isChanging: 'new',
                    state: "read",
                    ItemPriceBySupplierID: (function() { //对应第一次
                        if (changeitemsObjById[i].ItemPriceBySupplierChangeID === undefined) {
                            return changeitemsObjById[i].ItemPriceBySupplierID;
                        } else {
                            return 'new' + changeitemsObjById[i].ItemPriceBySupplierChangeID;
                        }
                    })(),
                    AdultNetPrice: null,
                    BobyNetPrice: null,
                    ChildNetPrice: null,
                    Price: null,
                    startTime: null,
                    EndTime: null,

                    AdultNetPriceChange: changeitemsObjById[i].AdultNetPrice,
                    BobyNetPriceChange: changeitemsObjById[i].BobyNetPrice,
                    ChildNetPriceChange: changeitemsObjById[i].ChildNetPrice,
                    PriceChange: changeitemsObjById[i].Price,
                    EndTimeChange: changeitemsObjById[i].EndTime,
                    startTimeChange: changeitemsObjById[i].startTime,
                    PayTypeChange: changeitemsObjById[i].PayType
                });
                priceList.push(price);
            }
        }
        for (i in itemsObjById) {
            if (i in changeitemsObjById) { //去在且在变更中
                price = new Price({
                    isChanging: true,
                    state: "read",
                    ItemPriceBySupplierID: itemsObjById[i].ItemPriceBySupplierID,

                    AdultNetPrice: itemsObjById[i].AdultNetPrice,
                    BobyNetPrice: itemsObjById[i].BobyNetPrice,
                    ChildNetPrice: itemsObjById[i].ChildNetPrice,
                    Price: itemsObjById[i].Price,
                    startTime: itemsObjById[i].startTime,
                    EndTime: itemsObjById[i].EndTime,
                    PayType: itemsObjById[i].PayType,

                    AdultNetPriceChange: (function() {
                        return changeitemsObjById[i].AdultNetPrice;
                    })(),

                    BobyNetPriceChange: (function() {
                        return changeitemsObjById[i].BobyNetPrice;
                    })(),
                    ChildNetPriceChange: (function() {
                        return changeitemsObjById[i].ChildNetPrice;
                    })(),
                    PriceChange: (function() {
                        return changeitemsObjById[i].Price;
                    })(),
                    EndTimeChange: (function() {
                        return changeitemsObjById[i].EndTime;
                    })(),
                    startTimeChange: (function() {
                        return changeitemsObjById[i].startTime;
                    })(),
                    PayTypeChange: changeitemsObjById[i].PayType
                });
                priceList.push(price);

            } else { //存在且未变更
                price = new Price({
                    isChanging: false,
                    state: "read",
                    ItemPriceBySupplierID: itemsObjById[i].ItemPriceBySupplierID,

                    AdultNetPrice: itemsObjById[i].AdultNetPrice,
                    BobyNetPrice: itemsObjById[i].BobyNetPrice,
                    ChildNetPrice: itemsObjById[i].ChildNetPrice,
                    Price: itemsObjById[i].Price,
                    startTime: itemsObjById[i].startTime,
                    EndTime: itemsObjById[i].EndTime,
                    PayType: itemsObjById[i].PayType
                });
                priceList.push(price);
            }
        }

        var extraPricex, countID;
        var extraPriceList = [];
        // var items = data.item;
        // var changeitems = data.changeitem;

        var extraServiceList = {};
        var extraServicePriceExistedList = {};
        var extraServicePriceChangingList = {};
        var extraServicePriceList = {};

        if (data.ExtraServices === null) { //没有额外的项目

        } else { //有额外项目
            for (i in data.ExtraServices) {
                extraServiceList[data.ExtraServices[i].ExtraServiceID] = {
                    ExtraServiceID: data.ExtraServices[i].ExtraServiceID,
                    ServiceName: data.ExtraServices[i].ServiceName,
                    ServiceEnName: data.ExtraServices[i].ServiceEnName,
                }
            }
            if (items !== null) {
                for (i in items.ExtraServicePrices) {
                    extraServicePriceExistedList[items.ExtraServicePrices[i].ExtraServiceID] = {
                        ServicePrice: items.ExtraServicePrices[i].ServicePrice,
                        ExtraServicePriceID: items.ExtraServicePrices[i].ExtraServicePriceID
                    }
                }
            }
            if (changeitems !== null) {
                for (i in changeitems.ExtraServicePrices) {
                    extraServicePriceChangingList[changeitems.ExtraServicePrices[i].ExtraServiceID] = {
                        ServicePriceChange: changeitems.ExtraServicePrices[i].ServicePrice
                    }
                }
            }
            for (i in extraServiceList) {
                countID = 0;
                if (i in extraServicePriceExistedList) {
                    extraServiceList[i].ExtraServicePriceID = extraServicePriceExistedList[i].ExtraServicePriceID;
                    extraServiceList[i].ServicePrice = extraServicePriceExistedList[i].ServicePrice;
                    countID += 1;
                }
                if (i in extraServicePriceChangingList) {
                    extraServiceList[i].ServicePriceChange = extraServicePriceChangingList[i].ServicePriceChange;
                    countID += 2;
                }
                if (countID === 0) {
                    extraServiceList[i].state = 'edit';
                    extraServiceList[i].type = 'new';
                } else if (countID == 1) {
                    extraServiceList[i].state = 'read';
                    extraServiceList[i].type = 'notChanging';
                } else if (countID == 2) {
                    extraServiceList[i].state = 'read';
                    extraServiceList[i].type = 'changing';
                } else if (countID == 3) {
                    extraServiceList[i].state = 'read';
                    extraServiceList[i].type = 'changing';
                }
                extraPricex = new ExtraPrice(extraServiceList[i]);
                extraPriceList.push(extraPricex);
            }
        }

        //其他处理
        var others = {};
        if (items !== null) {
            others.PayType = items.PayType == 0 ? '按游客人头数（例如一日游、门票）' : '按产品数量（例如酒店、包车）';
            others.SelectEffectiveWay = items.SelectEffectiveWay == 0 ? '按下单日期计算' : '按出行日期计算';
            others.CurrencyName = items.ItemCurrency.CurrencyName;
            others.Remark = items.Remark;

        }
        if (changeitems !== null) {
            others.PayTypeChange = changeitems.PayType == 0 ? '按游客人头数（例如一日游、门票）' : '按产品数量（例如酒店、包车）';
            others.SelectEffectiveWayChange = changeitems.SelectEffectiveWay == 0 ? '按下单日期计算' : '按出行日期计算';
            others.CurrencyNameChange = changeitems.ItemCurrency.CurrencyName;
            others.RemarkChange = changeitems.Remark;
        }

        return ({
            priceList: priceList,
            extraPriceList: extraPriceList,
            others: others
        })
    }



    function Price(data) {
        this.state = data.state ? data.state : "read";
        this.isChanging = data.isChanging ? data.isChanging : false;
        this.deleteAble = data.deleteAble ? data.deleteAble : false;

        this.ItemPriceBySupplierID = data.ItemPriceBySupplierID ? data.ItemPriceBySupplierID : null;

        this.AdultNetPrice = data.AdultNetPrice;
        this.BobyNetPrice = data.BobyNetPrice;
        this.ChildNetPrice = data.ChildNetPrice;
        this.Price = data.Price;
        this.EndTime = data.EndTime;
        this.startTime = data.startTime;

        this.AdultNetPriceChange = (data.AdultNetPriceChange !== undefined && data.AdultNetPriceChange !== null) ? data.AdultNetPriceChange : "";
        this.BobyNetPriceChange = (data.BobyNetPriceChange !== undefined && data.BobyNetPriceChange !== null) ? data.BobyNetPriceChange : "";
        this.ChildNetPriceChange = (data.ChildNetPriceChange !== undefined && data.ChildNetPriceChange !== null) ? data.ChildNetPriceChange : "";
        this.PriceChange = (data.PriceChange !== undefined && data.PriceChange !== null) ? data.PriceChange : "";
        this.EndTimeChange = data.EndTimeChange ? data.EndTimeChange : "";
        this.startTimeChange = data.startTimeChange ? data.startTimeChange : "";

        this.PayType = data.PayType == 0 || data.PayType == 1 ? parseInt(data.PayType) : "";
        this.PayTypeChange = data.PayTypeChange == 0 || data.PayTypeChange == 1 ? parseInt(data.PayTypeChange) : "";
    }

    function ExtraPrice(data) {
        this.state = data.state;
        this.type = data.type ? data.type : "notChanging"; //new，changing

        this.ExtraServiceID = data.ExtraServiceID;
        this.ServiceName = data.ServiceName;
        this.ExtraServicePriceID = data.ExtraServicePriceID ? data.ExtraServicePriceID : null;


        this.ServicePrice = (data.ServicePrice !== undefined && data.ServicePrice !== null) ? data.ServicePrice : "";

        this.ServicePriceChange = (data.ServicePriceChange !== undefined && data.ServicePriceChange !== null) ? data.ServicePriceChange : "";

    }

    function exportExcel(tableRef) {
        $('body').on("click", "#exportItemPrices", { "itemPrices": tableRef.dataTableRef }, function(e) {
            var temp;
            if (!(e.data.itemPrices.ajax.json())) {
                return;
            };
            if (e.data.itemPrices.ajax.json().recordsFiltered > 5000) {
                $.LangHua.alert({
                    "tip1": "导出性能提示",
                    "indent": false,
                    "tip2": "导出数量超过5000条，请设置筛选条件后再导出"
                })
                return;
            } else {
                var varURL = "/ServiceItems/PriceExportExcel?";
                var link = document.createElement("a");
                var search = $('#searchoption').data("search") ? $('#searchoption').data("search") : {};
                if (search) {
                    if (!(search instanceof Object)) {
                        search = JSON.parse(search);
                    }
                }
                for (var i in search) {
                    temp = search[i].toString().urlSwitch();
                    varURL += i + '=' + temp + "&";
                }
                link.href = varURL;
                document.body.appendChild(link);
                link.click();
                $(this).siblings('button').trigger('click')
            }
        })
    }

    function setSellPrice(tableRef) {
        var extraSellPrices = $("#sellPriceSet #extraSellPrices").DataTable({
            "serverSide": false,
            "ordering": false,
            "pageLength": 1000,
            "stateSave": false,
            "dom": "<t>",
            'rowId': 'ExtraServicePriceID',
            'createdRow': function(row, data, dataIndex) {
                $(row).addClass("edit");
            },
            'columnDefs': [{
                'targets': [0],
                'data': null,
                'render': function(cellData, type, rowData, meta) {
                    return (parseInt(meta.row) + 1);
                }
            }, {
                'targets': [1],
                'data': 'ServiceName',
            }, {
                'targets': [2],
                'data': "ServiceSellPrice",
                'render': function(cellData, type, rowData, meta) {
                    return (
                        '<input class="form-control displayInlineBlock input100 price-format" value="' + cellData + '">' +
                        '<span class="margin-left-10">RMB</span>'
                    );

                }
            }]
        });
        tableRef.jQueryRef.on('click', '.editSellPrice', { 'mainTable': tableRef, 'extraSellPrices': extraSellPrices }, function(e) {
            var trDom = $(this).closest('tr')[0];
            var rowData = e.data.mainTable.dataTableRef.row(trDom).data();

            //隐藏
            $('#sellPriceSet #payTypeFlag').data("type", rowData.PayType);
            $('#sellPriceSet #SupplierServiceItemID').data("type", rowData.SupplierServiceItemID);
            //主要价格
            if (parseInt(rowData.PayType) == 0) {
                $('#sellPriceSet .priceByPerson').removeClass("hidden");
                $('#sellPriceSet #priceByNumber').addClass("hidden");
                $('#sellPriceSet #AdultSellPrice').val(rowData.SellPrice).trigger("click");
                $('#sellPriceSet #ChildSellPrice').val(rowData.ChildSellPrice).trigger("click");
            } else {
                $('#sellPriceSet .priceByPerson').addClass("hidden");
                $('#sellPriceSet #priceByNumber').removeClass("hidden");
                $('#sellPriceSet #SellPrice').val(rowData.SellPrice).trigger("click");
            }
            ///额外
            var extralist = (rowData.ExtraServicePrices || []);
            if (extralist.length === 0) {
                $('#sellPriceSet #extraALL').hide();
            } else {
                $('#sellPriceSet #extraALL').show();
            }
            e.data.extraSellPrices.rows().remove();
            e.data.extraSellPrices.rows.add(extralist);
            e.data.extraSellPrices.draw();
            $('#sellPriceSet').modal("show");
            if (parseInt(rowData.PayType) == 0) {
                $('#sellPriceSet .priceByPerson input.price-format:eq(0):visible').focus();
            } else {
                $('#sellPriceSet #priceByNumber input.price-format:eq(0)').focus();
            }
        });

        $("#sellPriceSet").on("keypress", '.price-format', function(evt) {
            evt = (evt) ? evt : ((window.event) ? window.event : "") //兼容IE和Firefox获得keyBoardEvent对象
            var key = evt.keyCode ? evt.keyCode : evt.which; //兼容IE和Firefox获得keyBoardEvent对象的键值
            if (key == 13) {
                $('#sellPriceSet #sellPriceConfirm').trigger("click", ['enter', $(this)]);
            }
        })


        $('#sellPriceSet ').one("click", '#sellPriceConfirm', { 'mainTable': tableRef.dataTableRef, 'extraSellPrices': extraSellPrices }, function sellPriceSetting(e, enter, which) {

            var thisbtn = $(this);
            var theModal = $(this).closest('#sellPriceSet');
            var payType = theModal.find('#payTypeFlag').data("type");
            var SupplierServiceItemID = theModal.find('#SupplierServiceItemID').data("type");
            var SellPrice, ChildSellPrice;
            var post = {};
            post.SupplierServiceItemID = SupplierServiceItemID;
            var cancel = false;
            var datax = e.data;
            if (enter !== undefined) {
                which.each(function() {
                    var editValue = $(this).val();
                    var editValueTemp = $.trim(editValue);
                    var afterReplace = editValueTemp.replace(/[^0-9\.]/g, "");
                    if (afterReplace.length !== 0) {
                        var numberTest = /^[0-9]+(\.[0-9]*)?$/;
                        if (numberTest.test(afterReplace)) {
                            var tmp = afterReplace.match('.');
                            if (tmp === null) {
                                $(this).val(afterReplace);
                            } else {
                                var tmpARR = afterReplace.split('.');
                                if (tmpARR.length === 1) {
                                    $(this).val(tmpARR[0]);
                                } else {
                                    if (tmpARR[1].toString().length > 2) {
                                        $(this).val(Number(afterReplace).toFixed(2));
                                    } else {
                                        if (tmpARR[1].toString().length === 0) {
                                            $(this).val(tmpARR[0]);
                                        } else {
                                            $(this).val(afterReplace);
                                        }
                                    }
                                }
                            }
                        } else {
                            $(this).val("");
                        }
                    } else {
                        $(this).val("");
                    }
                });
            }
            if (parseInt(payType) === 0) {
                SellPrice = theModal.find("#AdultSellPrice").val();
                ChildSellPrice = theModal.find("#ChildSellPrice").val();
                if (SellPrice.length === 0) {
                    theModal.find("#AdultSellPrice").Warning({
                        'title': "请您填写有效价格",
                        'placement': "bottom"
                    });
                    cancel = true;
                }
                if (ChildSellPrice.length === 0) {
                    theModal.find("#ChildSellPrice").Warning({
                        'title': "请您填写有效价格",
                        'placement': "bottom"
                    });
                    cancel = true;
                }
                post.SellPrice = SellPrice;
                post.ChildSellPrice = ChildSellPrice;
            } else {
                SellPrice = theModal.find("#SellPrice").val();
                if (SellPrice.length === 0) {
                    theModal.find("#SellPrice").Warning({
                        'title': "请您填写有效价格",
                        'placement': "bottom"
                    });
                    cancel = true;
                }
                post.SellPrice = SellPrice;

            }
            var extralistInputs = theModal.find('#extraSellPrices .price-format');
            var extraSellPriceTemp;
            var ExtraServicePrices = [];
            for (var i = 0; i < extralistInputs.length; i++) {
                extraSellPriceTemp = theModal.find('#extraSellPrices .price-format:eq(' + i + ')').val();
                if (extraSellPriceTemp.length === 0) {
                    theModal.find('#extraSellPrices .price-format:eq(' + i + ')').Warning({
                        'title': "请您填写有效价格"
                    });
                    cancel = true;
                    break;
                }
                ExtraServicePrices.push({
                    'ExtraServicePriceID': theModal.find('#extraSellPrices .price-format:eq(' + i + ')').closest('tr').attr('id'),
                    'ServiceSellPrice': extraSellPriceTemp
                });
            }
            if (cancel == true) {
                $('#sellPriceSet ').one("click", '#sellPriceConfirm', e.data, sellPriceSetting);
                return;
            }
            post.ExtraServicePrices = ExtraServicePrices;
            $.ajax({
                url: "/ServiceItems/SaveSellPrices",
                type: 'post',
                contentType: "application/json; charset=utf-8;",
                data: JSON.stringify(post),
                dataType: 'json',
                beforeSend: function() {
                    $.LangHua.loadingToast({
                        tip: "正在保存卖价. . . . . ."
                    })
                },
                success: function(data) {
                    var openModals = $("body").data("modalmanager").getOpenModals();
                    if (openModals) {
                        for (var i in openModals) {
                            if ($(openModals[i]['$element'][0]).attr("id") !== "sellPriceSet") {
                                $(openModals[i]['$element'][0]).modal("hide");
                            }
                        }
                    }
                    if (data.ErrorCode == 200) {
                        console.log(e)
                        datax.mainTable.draw(false);
                        $('#sellPriceSet').modal("hide");
                    } else {
                        if (data.ErrorCode == 401) {
                            $.LangHua.alert({
                                title: "提示信息",
                                tip1: '保存失败',
                                tip2: data.ErrorMessage,
                                button: '确定'
                            });
                        } else {
                            $.LangHua.alert({
                                title: "提示信息",
                                tip1: '保存失败',
                                tip2: '保存失败',
                                button: '确定'
                            })
                        }
                        jQuery(_this).one('click', postData);
                    }
                },
                "error": function() {
                    $.LangHua.alert({
                        title: "提示信息",
                        tip1: '提交失败',
                        tip2: 'Ajax请求有误',
                        button: '确定'
                    })
                    jQuery(_this).one('click', postData);
                },
                'complete': function() {
                    $('#sellPriceSet ').one("click", '#sellPriceConfirm', datax, sellPriceSetting);
                }
            });
        });
    }

    function sortByDate(arr) {
        var i;
        var timeStampNow = (new Date()).valueOf();
        var timeStampStart, timeStampEnd;
        var arrStart, arrEnd;
        var timeStart, timeEnd;
        var arrSorted = [];
        arr.sort(function(a, b) {

            // var timeStampNow = (new Date()).valueOf();
            var timeStampStartA, arrStartA, timeStampStartB, arrStartB;
            var timeA = a.startTimeChange || a.startTime;
            var timeB = b.startTimeChange || b.startTime;
            arrStartA = timeA.split("T")[0].split("-");
            timeStampStartA = (new Date(arrStartA[0], parseInt(arrStartA[1]) - 1, arrStartA[2], 0, 0, 0)).valueOf();
            arrStartB = timeB.split("T")[0].split("-");
            timeStampStartB = (new Date(arrStartB[0], parseInt(arrStartB[1]) - 1, arrStartB[2], 0, 0, 0)).valueOf();
            if (timeStampStartA < timeStampStartB) {
                return 1;
            } else {
                return -1;
            }
        })
        for (i in arr) {
            timeStart = arr[i].startTimeChange || arr[i].startTime;
            timeEnd = arr[i].EndTimeChange || arr[i].EndTime;
            arrStart = timeStart.split("T")[0].split("-");
            arrEnd = timeEnd.split("T")[0].split("-");

            timeStampStart = (new Date(arrStart[0], parseInt(arrStart[1]) - 1, arrStart[2], 0, 0, 0)).valueOf();
            timeStampEnd = (new Date(arrEnd[0], parseInt(arrEnd[1]) - 1, arrEnd[2], 0, 0, 0)).valueOf() + 1000 * 60 * 60 * 24;
            if (
                (timeStampStart <= timeStampNow) &&
                (timeStampNow <= timeStampEnd)
            ) {
                arrSorted.unshift(arr[i]);
            } else {
                arrSorted.push(arr[i])
            }
        }
        return arrSorted;
    }

});