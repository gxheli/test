$(document).ready(function() {
    fix();
    var tableOrig = $('table#productList').clone();
    var Itemlist = tableInit($('.tabletools').eq(0), tableOrig);
    initTabletools(Itemlist);
    showHistorys(Itemlist);
    confirmPrice();

    function　 fix() {
        if (jQuery("#searchoption").length === 0) {
            var search = jQuery('<div id="searchoption" ></div>').data("search", {
                "FuzzySearch": "",
                "ServiceTypeID": "0"
            });
            $('body').append(search);
        }
    }


    function tableInit(tabletools, tableOrig) {
        var distribution =
            jQuery('table#productList')
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
                syncState(json.SearchModel.ItemPriceSearch);
            })
            .DataTable({
                ajax: {
                    "url": "/Product/GetItemPrices",
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
                            // 时间倒叙
                            otherPriceList.sort(function(theNext, theLast) {
                                var timeStartArrRef = theNext.StartDate.split("-");
                                var timeEndArrRef = theLast.EndDate.split("-");
                                var timeStartRef = (new Date(timeStartArrRef[0], parseInt(timeStartArrRef[1]) - 1, timeStartArrRef[2], 0, 0, 0)).valueOf();
                                var timeEndRef = (new Date(timeEndArrRef[0], parseInt(timeEndArrRef[1]) - 1, timeEndArrRef[2], 0, 0, 0, 0)).valueOf();
                                if (timeStartRef < timeEndRef) {
                                    return 1;
                                } else {
                                    return -1;
                                }
                            });
                            dataAll.data[i].inPriceList = inPriceList;
                            dataAll.data[i].otherPriceList = otherPriceList;
                        }
                        return JSON.stringify(dataAll)
                    }
                },
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
                //列操作
                columnDefs: [{
                    'targets': [0],
                    'data': 'ServiceCode',
                }, {
                    'targets': [1],
                    'data': 'cnItemName',
                }, {
                    'targets': [2],
                    'data': 'TravelCompany',
                }, {
                    'targets': [3],
                    'data': 'ServiceTypeName',
                }, {
                    'targets': [4],
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
                    'targets': [5],
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
                    'targets': [6],
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
                    'targets': [7],
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
                    'targets': [8],
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
                    'targets': [9],
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
                    'targets': [10],
                    'data': 'IsChange',
                    'render': function(cellData, type, rowData, meta) {
                        if (cellData !== true) {
                            return '确认';
                        } else {
                            return '<a class="showhistorys" style="color:#6600FF !important;">待确认</a>';
                        }
                    }
                }, {
                    'targets': [11],
                    'data': null,
                    'render': function(cellData, type, rowData, meta) {
                        return (
                            '<div class="row">' +
                            '<a class="showhistorys" >详情</a>' + ' | ' +
                            '<a class="" target="_blank" href="/Product/PriceOperation/' + rowData.SupplierServiceItemID + '">日志</a>' +
                            '</div>'
                        )
                    }
                }]
            });
        jQuery('table#productList').on('click', 'td .details-control', function() {
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
        jQuery('table#productList').on('click', 'td .otherTimeRange', function() {
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
            'jQueryRef': jQuery('table#productList')
        };
    }

    function makeDetail(rowData, tableRef, isEven) {
        var divx = $("<div></div>");
        var table = $("<table class='extraList table-lh  table-lh-bordered-white table-lh-bordered-edge-remove table-lh-td-no table-lh-hover-by-classnanme'><colgroup></colgroup><tbody></tbody></table>");
        table.css({ "width": "100%", "border-collapse": "collapse" });
        table.find("colgroup:eq(0)").append(tableRef.find("colgroup:eq(0)").html());
        var trtemp, tdtemp;
        var i, j, index;
        var tr = $("<tr></tr>");
        if (isEven) {
            tr.css('background', "#f9f9f9");
        } else {}
        var td = $("<td></td>");
        td.css({ "border-top": "0px solid white" });
        var index = 0;
        var ExtraServicePrices = rowData.ExtraServicePrices;
        for (i in ExtraServicePrices) {
            trtemp = tr.clone();
            trtemp.append(td.clone());
            trtemp.append(td.clone().text(ExtraServicePrices[i].ServiceName));
            for (j = 0; j < 6; j++) {
                trtemp.append(td.clone());
            }
            trtemp.append(
                td.clone().append(divx.clone().text(ExtraServicePrices[i].ServicePrice)).append(divx.clone().css("color", "#999999").text(rowData.CurrencyNo))
            );
            trtemp.append(td.clone());
            trtemp.append(td.clone());
            trtemp.append(td.clone());
            table.find('tbody').append(trtemp);

        }
        return table;
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
            for (j = 0; j < 4; j++) {
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
            for (j = 0; j < 3; j++) {
                trtemp.append(td.clone());
            }
            table.find('tbody').append(trtemp);
        }
        return table;
    }

    function initTabletools(Itemlist) {
        jQuery('body').on("click", '#reflashTable', function() {
            Itemlist.dataTableRef.draw();
        });
        //状态筛选
        jQuery('#allstate').ButtonRadio({
            selected: function(dom, code) {
                if ($('#searchoption').length == 0) {
                    $('body').append("<div id='searchoption' class='hidden'></div>");
                }
                $("#searchoption").data({
                    search: {
                        ServiceTypeID: code
                    }
                })
                Itemlist.dataTableRef.draw();
            }
        });

        $('.tabletools:eq(0)').find('#ChangeButton').ButtonRadio({
                data: [],
                selected: function(dom, code) {

                    $('#searchoption').data('search', JSON.stringify({
                        "IsChange": true
                    }));
                    Itemlist.dataTableRef.draw();
                    // $('.tabletools:eq(0)').find('#ServiceType .buttonradio:eq(0)').addClass('active').siblings().removeClass('active');
                    // $('.tabletools:eq(0)').find('#FuzzySearch').text("");
                    // $('.tabletools:eq(0)').find('#SupplierID').val(0);

                }

            })
            //模糊搜索 开始
        jQuery("#fuzzySearch").bind("click", function() {
            var fuzzyString = jQuery("#fuzzyString").val().trim();
            if ($('#searchoption').length == 0) {
                $('body').append("<div id='searchoption' class='hidden'></div>");
            }
            var fuzzyString = jQuery("#fuzzyString").val().trim();
            $('#searchoption').data("search", {
                'FuzzySearch': fuzzyString,
                'ServiceTypeID': ""
            });
            Itemlist.dataTableRef.draw();
        })
        jQuery('#fuzzyString').bind("keydown", function(evt) {
                evt = (evt) ? evt : ((window.event) ? window.event : "");
                var key = evt.keyCode ? evt.keyCode : evt.which;
                if (key == 13) {
                    jQuery("#fuzzySearch").trigger("click");
                }

            })
            //模糊搜索 结束
    }



    // clearstate 
    function syncState(obj) {
        console.log(obj)
        var method = {
            ServiceTypeID: function(value) {
                $('#allstate').find(".buttonradio[data-code=" + value + "]")
                    .addClass("active")
                    .siblings(".active").removeClass("active");
            },
            SupplierID: function(value) {},
            FuzzySearch: function(value) {
                $('#fuzzyString').val(value == null ? "" : value);
            },
            IsChange: function(value) {
                if (value === false) {
                    $('.tabletools:eq(0)').find('#ChangeButton .buttonradio').removeClass("active");
                }
            }
        }
        if (obj == null) {
            $('#allstate').find(".buttonradio:first")
                .addClass("active")
                .siblings('.buttonradio').removeClass("active");
            return;
        }
        for (var i in obj) {
            method[i](obj[i])
        }

    }

    function showHistorys(tableRef) {
        var prices = $("#historyDisplay #prices").DataTable({
            "serverSide": false,
            "ordering": false,
            "pageLength": 1000,
            "stateSave": false,
            "dom": "<t>",
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
            'createdRow': function(row, data, dataIndex) {
                if (data.Hilight === true) {
                    $(row).css('background', '#FFFFCC');

                }
            },
            'language': {
                'zeroRecords': "",
                'emptyTable': '没有附加项目'
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
                url: "/Product/GetPrice",
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
                    $("#historyDisplay #whichPrice").text(data.data.baseinfo.cnItemName + data.data.baseinfo.ServiceCode);
                    var others = priceExtraPrice.others;
                    var arr = ['PayType',
                        'SelectEffectiveWay',
                        'CurrencyName',
                    ];
                    console.log(others);
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
                    if (rowData.IsChange === true) {
                        $('#historyDisplay #changeFlag').text("yes");
                        if (data.data.item !== null && data.data.changeitem === null && data.data.item.IsChange === true) { //对应第一次未确认
                            $('#historyDisplay #SupplierServiceItemID').text(data.data.item.SupplierServiceItemID);
                            $('#historyDisplay #SupplierServiceItemChangeID').text("");
                        } else { //其他
                            $('#historyDisplay #SupplierServiceItemID').text(data.data.item.SupplierServiceItemID);
                            $('#historyDisplay #SupplierServiceItemChangeID').text(data.data.changeitem.SupplierServiceItemChangeID);
                        }

                        $('#historyDisplay #changeConfirmButton').show();
                        $('#historyDisplay #notChangeConfirmButton').hide();
                    } else {
                        $('#historyDisplay #changeFlag').text("no");
                        $('#historyDisplay #SupplierServiceItemID').text('');
                        $('#historyDisplay #SupplierServiceItemChangeID').text('');
                        $('#historyDisplay #changeConfirmButton').hide();
                        $('#historyDisplay #notChangeConfirmButton').show();
                    }
                    $('#historyDisplay').modal('show');

                }
            });
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
            others.SelectEffectiveWay = items.SelectEffectiveWay == 0 ? '按下单日期计算（推荐）' : '按出行日期计算';
            others.CurrencyName = items.ItemCurrency.CurrencyName;
            others.Remark = items.Remark;

        }
        if (changeitems !== null) {
            others.PayTypeChange = changeitems.PayType == 0 ? '按游客人头数（例如一日游、门票）' : '按产品数量（例如酒店、包车）';
            others.SelectEffectiveWayChange = changeitems.SelectEffectiveWay == 0 ? '按下单日期计算（推荐）' : '按出行日期计算';
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


    function confirmPrice() {

        $('body').one("click", '#changeConfirm', function confirming() {
            var SupplierServiceItemID = $.trim($(this).closest(".modal-footer").find("#SupplierServiceItemID").text());
            var SupplierServiceItemChangeID = $.trim($(this).closest(".modal-footer").find("#SupplierServiceItemChangeID").text());
            var post = {
                'SupplierServiceItemID': SupplierServiceItemID,
                'SupplierServiceItemChangeID': SupplierServiceItemChangeID
            }
            $.ajax({
                type: 'post',
                dataType: 'json',
                contentType: "application/json; charset=utf-8;",
                data: JSON.stringify(post),
                url: '/Product/ConfirmPrice',
                beforeSend: function() {
                    toast = $.LangHua.loadingToast({
                        tip: '正在确认价格 . . . . . .'
                    });
                },
                success: function(data) {
                    toast.modal('hide');
                    if (data.ErrorCode == 200) {
                        window.location.reload();

                    } else {
                        if (data.ErrorCode == 401) {
                            $.LangHua.alert({
                                title: "提示信息",
                                tip1: '正在确认价格失败',
                                tip2: data.ErrorMessage,
                                button: '确定'
                            });
                        } else {
                            $.LangHua.alert({
                                title: "提示信息",
                                tip1: '确认价格失败',
                                tip2: "确认价格失败",
                                button: '确定'
                            });
                        }
                        $('body').one("click", '#changeConfirm', confirming);
                    }
                },

                complete: function(XHR, TS) {
                    toast.modal('hide');
                    if (TS !== "success") {
                        $('body').one("click", '#changeConfirm', confirming);
                    }
                }
            })
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