jQuery(document).ready(function() {
    init();
});

function init() {
    var saleStatistics =
        $('#saleStatistics').DataTable({
            "ordering": false,
            "initComplete": function(settings, json) {
                jQuery(this).on("updateData", { "settingx": settings }, function(e, arr) {
                    var api = new $.fn.dataTable.Api(e.data.settingx);
                    var sorting = jQuery(this).data(sorting);
                    sorting = jQuery(this).data("sorting");
                    arr.sort(function(one, another) {
                        if (one[sorting.sortBy] > another[sorting.sortBy]) {
                            return sorting.dir;
                        } else {
                            return -(sorting.dir)
                        }
                    });
                    api.rows().remove();
                    api.rows.add(arr);
                    api.draw();
                });
            },
            "columnDefs": [{
                'targets': [0],
                'data': null,
                "render": function(cellData, type, rowData, meta) {
                    var indexInpage = meta.row;
                    var settings = meta.settings;
                    return (settings._iDisplayStart + indexInpage + 1);
                }
            }, {
                'targets': [1],
                'data': null,
                "render": function(cellData, type, rowData, meta) {
                    return rowData.UserName
                }
            }, {
                'targets': [2],
                'data': null,
                "render": function(cellData, type, rowData, meta) {
                    return rowData.StartDate
                }
            }, {
                'targets': [3],
                'data': null,
                "render": function(cellData, type, rowData, meta) {
                    return rowData.EndDate
                }
            }, {
                'targets': [4],
                'data': null,
                "render": function(cellData, type, rowData, meta) {
                    return rowData.Income
                }
            }, {
                'targets': [5],
                'data': null,
                "render": function(cellData, type, rowData, meta) {
                    return rowData.Cost
                }
            }, {
                'targets': [6],
                'data': null,
                "render": function(cellData, type, rowData, meta) {
                    return rowData.RefundFee
                }
            }, {
                'targets': [7],
                'data': null,
                "render": function(cellData, type, rowData, meta) {
                    return rowData.AlipayTransfer
                }
            }, {
                'targets': [8],
                'data': null,
                "render": function(cellData, type, rowData, meta) {
                    return rowData.Profit.toFixed(2)
                },
                "createdCell": function(cell, cellData, rowData, rowIndex, colIndex) {
                    if (rowData.Profit < 0) {
                        $(cell).css({
                            "background": "red",
                            "color": "white"
                        })
                    }
                }
            }, {
                'targets': [9],
                'data': null,
                "render": function(cellData, type, rowData, meta) {
                    return ((rowData.ProfitRate * 100).toFixed(0) + "%")
                },
                "createdCell": function(cell, cellData, rowData, rowIndex, colIndex) {
                    if (rowData.ProfitRate < 0) {
                        $(cell).css({
                            "background": "red",
                            "color": "white"
                        });
                    }
                }

            }]
        });

    $('#saleStatistics').data("sorting", {
        "sortBy": "Profit",
        "dir": -1
    });

    //tools
    $('.tabletools:eq(0)').find('#daterange').datepicker({
        'inputs': $('.tabletools:eq(0) #daterange input')
    });
    $("body .tabletools:eq(0)  #search").on("click", function() {
        getAllProfitData({
            "StartDate": $("body .tabletools:eq(0)  #StartDate").val(),
            "EndDate": $("body .tabletools:eq(0)  #EndDate").val()
        }, makeDataForTable);
    });
    if (!($("body  #daterange #StartDate").val())) {
        var dateStart, dateEnd, dateTemp;
        var now = new Date();
        dateStart = new Date(now.getFullYear(), now.getMonth(), 1, 0, 0, 0);
        dateEnd = new Date(now.getFullYear(), now.getMonth(), parseInt(now.getDate()) === 1 ? 1 : now.getDate() - 1, 0, 0, 0);
        $("body  #daterange #StartDate").datepicker("setDate", dateStart);
        $("body  #daterange #EndDate").datepicker("setDate", dateEnd);
    }
    // $("body .tabletools:eq(0)  #search").trigger("click");
    $('#saleStatistics').on('click', ".orderby", function() {
        if (!$(this).find('.caret:eq(0)').hasClass('status-gray')) {
            $(this).toggleClass('dropup');
        }
        $(this).find('.caret').removeClass('status-gray');

        $('#saleStatistics .orderby').not(this).each(function() {
            $(this).find('.caret').addClass('status-gray');
        });
        var sortBy = $(this).data("for");
        var _this = $(this);
        $('#saleStatistics').data("sorting", {
            "sortBy": sortBy,
            "dir": (function(which) {
                if (which.hasClass("dropup")) {
                    return 1;
                } else {
                    return -1;
                }
            })(_this)
        });
        if ($('#saleStatistics').data("datalast")) {
            $('#saleStatistics').trigger("updateData", [$('#saleStatistics').data("datalast")]);

        } else {
            $("body .tabletools:eq(0)  #search").trigger("click");
        }

    })
}

function makeDataForTable(arr) {
    var i, item;
    for (i in arr) {
        item = arr[i];
        item.Profit = parseFloat(item.Income) - parseFloat(item.Cost) - parseFloat(item.RefundFee) - parseFloat(item.AlipayTransfer);
        if (parseFloat(item.Income) > 0 || parseFloat(item.Income) < 0) {
            item.ProfitRate = item.Profit / parseFloat(item.Income);
        } else {
            item.ProfitRate = 0;
        }
    }
    $('#saleStatistics').trigger("updateData", [arr]);
    $('#saleStatistics').data("datalast", arr);
}


function getAllProfitData(conditons, makeDataForTale) {
    $.ajax({
        "type": "get",
        "data": conditons,
        "beforeSend": function() {
            $.LangHua.loadingToast({
                tip: "正 在 统 计 数 据 . . . . . ."
            })
        },
        "url": "/SalesStatistics/GetStatisticsOrderPrice",
        "dataType": 'json',
        "success": function(data) {
            var openModals = $("body").data("modalmanager").getOpenModals();
            if (openModals) {
                for (var i in openModals) {
                    $(openModals[i]['$element'][0]).modal("hide");
                }
            }
            if (data.ErrorCode == 200) {
                makeDataForTable(data.prices)
            } else if (data.ErrorCode == 401) {
                $.LangHua.alert({
                    title: "提示信息",
                    tip1: '统计利润失败',
                    tip2: data.ErrorMessage,
                    button: '确定'
                });
            } else {

            }
        },
        "error": function() {
            var openModals = $("body").data("modalmanager").getOpenModals();
            if (openModals) {
                for (var i in openModals) {
                    // if ($(openModals[i]['$element'][0]).attr("id") !== "sellPriceSet") {
                    $(openModals[i]['$element'][0]).modal("hide");
                    // }
                }
            }
            $.LangHua.alert({
                title: "提示信息",
                tip1: '统计利润失败',
                tip2: "统计利润失败,请重试",
                button: '确定'
            });

        }
    })
}