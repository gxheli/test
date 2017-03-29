
jQuery(document).ready(function ($) {

    jQuery('#Date').datepicker();

    $('#orderList').eq(0)
   .on('preXhr.dt', function (e, settings, json) {
       var search = ($('#searchoption').data("search"));
       // 删除插件无必要项目
       delete json.columns;
       delete json.order;
       delete json.search;
       json['BillSearch'] = search;

       $('#reflashTable').find('span').addClass("fa-spin");
   }).on('xhr.dt', function (e, settings, json, xhr) {
       syncState(json.SearchModel.BillSearch);
       $('#reflashTable').find('span').removeClass("fa-spin");
   })

    var newbill =
	jQuery('table#orderList')
        .DataTable({
            ajax: {
                url: "/BillReports/GetList",
                type: 'post',
            },
            ordering: false,
            searching: false,
            serverSide: true,
            initComplete: function (settings, json) {

            },
            drawCallback: function (settings) {
                // 更新页数
                var api = this.api();
                $("#cp.ddone").text(api.page.info().page + 1 + '/' + api.page.info().pages + '页');
                //更新提示数据
            },

            //行操作
            rowId: "BillReportID",
            createdRow: function (row, data, dataIndex) {
                var _this = this.api();
                var thisTable = this;
                jQuery(row).on('click', '.cancel', function () {
                    _this
                        .row(row)
                        .remove()
                        .draw();
                });
                //缓存有用的单行数据
                jQuery(row).data({
                });
            },
            //列操作
            columns: [
                {
                    'data': 'Type',
                },
                {
                    'data': 'StartDate',
                    "createdCell": function (td, cellData, rowData, row, col) {
                        jQuery(td).html('<div>' + rowData.StartDate + '<br/>至<br/>' + rowData.EndDate + '</div>');
                    }
                },
                {
                    'data': 'TotalReceive',
                    "createdCell": function (td, cellData, rowData, row, col) {
                        jQuery(td).html('<div>' + cellData + ' ' + rowData.Currency + '</div>');
                    }
                },
                {
                    'data': 'RealReceive',
                    "createdCell": function (td, cellData, rowData, row, col) {
                        jQuery(td).html('<div><span id="Receive">' + cellData + '</span> <span id="RealCurrency">' + rowData.Currency + '</span></div>');
                    }
                },
                {
                    'data': 'CreateTime',
                },
                {
                    'data': 'PayTime',
                },
                {
                    'data': 'StateValue',
                    "createdCell": function (td, cellData, rowData, row, col) {
                        switch (rowData.State) {
                            case 0:
                                jQuery(td).addClass("background-brown");
                                break;
                            case 1:
                                jQuery(td).addClass("background-yellow");
                                break;
                            case 2:
                                jQuery(td).addClass("background-green");
                                break;
                            case 3:
                                jQuery(td).addClass("background-gray");
                                break;
                        }
                        jQuery(td).html('<div>' + cellData + '</div>');
                    }
                },
                                //操作
                {
                    'data': 'BillReportID',
                    "createdCell": function (td, cellData, rowData, row, col) {
                        var str = '<div><a class="hrefInTable-inline" href="/BillReports/DownFile/' + cellData + '" id="exportproducts">下载明细</a></div>';
                        jQuery(td).html(str);
                    }
                }
            ]
        });

    //clearstate 
    function syncState(obj) {

        var method = {
            status: function (value) {
                $('#state').find(".buttonradio[data-code=" + value + "]")
                    .addClass("active")
                    .siblings(".active").removeClass("active");
            },
            SupplierID: function (value) {
            },
            datetype: function (value) {
                $('#DateType option[value=' + value + ']').prop('selected', 'selected');
            },
            date: function (value) {
                $('#Date').val(value == null ? "" : value);
            },
        }
        if (obj == null) {
            $('#state').find(".buttonradio:first")
               .addClass("active")
               .siblings('.buttonradio').removeClass("active");
            return;
        }
        for (var i in obj) {
            method[i](obj[i])
        }

    }

    //转账类型的筛选
    $("#state").ButtonRadio({
        selected: function (dom, code) {
            if ($('#searchoption').length == 0) {
                $('body').append("<div id='searchoption' class='hidden'></div>");
            }
            $("#searchoption").data({
                search: {
                    status: code
                }
            })
            newbill.draw();
        }
    })
    // 模糊搜索
    jQuery("#fuzzySearch").bind("click", function () {
        if ($('#searchoption').length == 0) {
            $('body').append("<div id='searchoption' class='hidden'></div>");
        }
        var DateType = jQuery("#DateType").val();
        var Date = jQuery("#Date").val();

        $('#searchoption').data("search", {
            status: -1,
            datetype: DateType,
            date: Date
        });
        newbill.draw();
    })
    $('#reflashTable').bind("click", function () {
        newbill.draw()
    });
})
