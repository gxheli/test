
jQuery(document).ready(function ($) {

    jQuery('#Date').datepicker();
    jQuery('#launchdaterange').datepicker();

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
       $('#orderList thead tr th:eq(0) input').prop("checked", false)
       syncState(json.SearchModel.BillSearch);
       $('#reflashTable').find('span').removeClass("fa-spin");
       $('#selectedNumber').text(0);
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
                    'data': 'SupplierNo',
                    "createdCell": function (td, cellData, rowData, row, col) {
                        jQuery(td).html(cellData);
                    }
                },
                {
                    'data': 'Type',
                    "createdCell": function (td, cellData, rowData, row, col) {
                        jQuery(td).html(cellData);
                    }
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
                    "createdCell": function (td, cellData, rowData, row, col) {
                        jQuery(td).html('<div>' + cellData + '</div>');
                    }
                },
                {
                    'data': 'PayTime',
                    "createdCell": function (td, cellData, rowData, row, col) {
                        jQuery(td).html('<div>' + cellData + '</div>');
                    }
                },
                {
                    'data': 'Remark',
                    "createdCell": function (td, cellData, rowData, row, col) {
                        jQuery(td).html(' <div>' + cellData + '</div>');
                    }
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
                        var str = '';
                        if (rowData.State == 0) {
                            str += '<div>';
                            str += '<a class="hrefInTable-inline" href="javascript:;" id="Check">对账</a>';
                            str += '<a class="hrefInTable-inline" href="javascript:;" id="Transfer">支付</a>';
                            str += '<a class="hrefInTable-inline" href="javascript:;" id="IsDelete">作废</a>';
                            str += '</div>';
                        }
                        else if (rowData.State == 1) {
                            str += '<div>';
                            str += '<a class="hrefInTable-inline" href="javascript:;" id="Transfer">支付</a>';
                            str += '<a class="hrefInTable-inline" href="javascript:;" id="IsDelete">作废</a>';
                            str += '</div>';
                        }
                        str += '<div><a class="hrefInTable-inline" href="/BillReports/DownFile/' + cellData + '" id="exportproducts">下载明细</a>' +
                            '<a class="hrefInTable-inline" href="javascript:;" id="RemarkShow">实付</a>' +
                            '</div>';
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
                $('#SupplierID option[value=' + value + ']').prop('selected', 'selected');
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
    $("#SupplierID").bind('change', function () {
        if ($('#searchoption').length == 0) {
            $('body').append("<div id='searchoption' class='hidden'></div>");
        }
        var SupplierID = $(this).val();
        $("#searchoption").data({
            search: {
                status: -1,
                SupplierID: SupplierID
            }
        })
        newbill.draw();
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

    //备注
    jQuery("body").on('click', '#RemarkShow', function (e, data) {

        var id = jQuery(this).closest("tr").attr("id");
        var oldReark = jQuery(this).closest("tr").find("td:eq(7)").text();
        var RealReceive = jQuery(this).closest("tr").find("td:eq(4) #Receive").text();
        var Currency = jQuery(this).closest("tr").find("td:eq(4) #RealCurrency").text();
        $("#Remarksearch").one('shown.bs.modal', function () {
            jQuery("#Remark").val(oldReark)
            jQuery("#RealReceive").val(RealReceive)
            jQuery("#Currency").text(Currency)

            jQuery(this).find('#saveRemark').unbind().bind("click", function () {
                $.ajax({
                    type: 'post',
                    dataType: 'json',
                    contentType: "application/json; charset=utf-8;",
                    data: JSON.stringify({ id: id, Remark: jQuery.trim(jQuery("#Remark").val()), RealReceive: jQuery.trim(jQuery("#RealReceive").val()) }),
                    url: '/BillReports/UpdateRemark',
                    success: function (data) {
                        if (data.ErrorCode == 200) {
                            newbill.draw();
                            $('#Remarksearch').modal('hide');
                            jQuery("#Remark").val('');
                        }
                        else {
                            jQuery('#saveRemark').success(data.ErrorMessage);
                        }
                    }
                })
            })
        })

        $("#Remarksearch").modal();
    })
    jQuery("body").on('click', '#Check', function (e, data) {
        var _this = this;
        var id = jQuery(_this).closest("tr").attr("id");
        var state = 1;//Check

        var tip1 = "提示信息：";
        var tip2 = '您确认要进行对账操作吗？';

        jQuery.LangHua.confirm({
            title: "提示信息",
            tip1: tip1,
            tip2: tip2,
            confirmbutton: '确定',
            cancelbutton: '取消',
            data: null,
            confirm: function () {
                $.ajax({
                    type: 'post',
                    dataType: 'json',
                    contentType: "application/json; charset=utf-8;",
                    data: JSON.stringify({ id: id, state: state }),
                    url: '/BillReports/UpdateState',
                    success: function (data) {
                        if (data.ErrorCode == 200) {
                            jQuery(_this).success(data.ErrorMessage);
                            newbill.draw();
                        }
                        else {
                            jQuery(_this).success(data.ErrorMessage);
                        }
                    }
                })
            }
        })

    })
    jQuery("body").on('click', '#Transfer', function (e, data) {
        var _this = this;
        var id = jQuery(_this).closest("tr").attr("id");
        var state = 2;//Transfer
        jQuery.LangHua.confirm({
            title: "提示信息",
            tip1: '提示信息：',
            tip2: '您确认要进行支付操作吗？',
            confirmbutton: '确定',
            cancelbutton: '取消',
            data: null,
            confirm: function () {
                $.ajax({
                    type: 'post',
                    dataType: 'json',
                    contentType: "application/json; charset=utf-8;",
                    data: JSON.stringify({ id: id, state: state }),
                    url: '/BillReports/UpdateState',
                    success: function (data) {
                        if (data.ErrorCode == 200) {
                            jQuery(_this).success(data.ErrorMessage);
                            newbill.draw();
                        }
                        else {
                            jQuery(_this).success(data.ErrorMessage);
                        }
                    }
                })
            }
        })
    })
    jQuery("body").on('click', '#IsDelete', function (e, data) {
        var _this = this;
        var id = jQuery(_this).closest("tr").attr("id");
        var state = 3;//IsDelete
        jQuery.LangHua.confirm({
            title: "提示信息",
            tip1: '提示信息：',
            tip2: '您确认要进行作废操作吗？',
            confirmbutton: '确定',
            cancelbutton: '取消',
            data: null,
            confirm: function () {
                $.ajax({
                    type: 'post',
                    dataType: 'json',
                    contentType: "application/json; charset=utf-8;",
                    data: JSON.stringify({ id: id, state: state }),
                    url: '/BillReports/UpdateState',
                    success: function (data) {
                        if (data.ErrorCode == 200) {
                            jQuery(_this).success(data.ErrorMessage);
                            newbill.draw();
                        }
                        else {
                            jQuery(_this).success(data.ErrorMessage);
                        }
                    }
                })
            }
        })
    })

    $('#reflashTable').bind("click", function () {
        newbill.draw()
    });

    $('#create').bind('click', function () {
        var _this = this;
        var supplier = $('#supplier').val();
        var type = $('.type[type]:checked').val();
        var startdate = $('#StartDate').val();
        var enddate = $('#EndDate').val();
        var bl = false;
        if (supplier == 0) {
            bl = true;
            $('#supplier').warning('请选择');
        }
        if (!startdate) {
            bl = true;
            $('#StartDate').warning('请填写');
        }
        if (!enddate) {
            bl = true;
            $('#EndDate').warning('请填写');
        }
        if (bl) {
            return;
        }
        $.ajax({
            type: 'post',
            dataType: 'json',
            contentType: "application/json; charset=utf-8;",
            data: JSON.stringify({ SupplierID: supplier, state: type, startdate: startdate, enddate: enddate }),
            url: '/BillReports/CreateBill',
            beforeSend: function () {
                toast = $.LangHua.loadingToast({
                    tip: '正在生成账单.....'
                });
            },
            success: function (data) {
                toast.modal('hide');
                if (data.ErrorCode == 200) {
                    jQuery(_this).success(data.ErrorMessage);
                    newbill.draw();
                    $('#CreateBill').modal('hide');
                }
                else {
                    jQuery(_this).success(data.ErrorMessage);
                }
            }
        })
    })
})
