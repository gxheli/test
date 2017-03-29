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
            json['AlipaySearch'] = search;

            $('#reflashTable').find('span').addClass("fa-spin");
        }).on('xhr.dt', function(e, settings, json, xhr) {
            $('#orderList thead tr th:eq(0) input').prop("checked", false)
            syncState(json.SearchModel.AlipaySearch);
            $('#TransferBeforeMonth').text(json.TransferBeforeMonth);
            $('#TransferThisMonth').text(json.TransferThisMonth);
            $('#CheckTransfer').text(json.CheckTransfer);
            $('#reflashTable').find('span').removeClass("fa-spin");

            $('#selectedNumber').text(0);
        })

    var newAlipayTransfer =
        jQuery('table#orderList')
        .DataTable({
            ajax: {
                url: "/AlipayTransfers/GetAlipayTransfer",
                type: 'post',
            },
            ordering: false,
            searching: false,
            serverSide: true,
            initComplete: function(settings, json) {

            },
            drawCallback: function(settings) {
                // 更新页数
                var api = this.api();
                $("#cp.ddone").text(api.page.info().page + 1 + '/' + api.page.info().pages + '页');
                //更新提示数据
            },

            //行操作
            rowId: "AlipayTransferID",
            createdRow: function(row, data, dataIndex) {
                var _this = this.api();
                var thisTable = this;
                jQuery(row).on('click', '.cancel', function() {
                    _this
                        .row(row)
                        .remove()
                        .draw();
                });
                //缓存有用的单行数据
                jQuery(row).data({});
            },
            //列操作
            columns: [{
                    'data': 'TBID',
                    "createdCell": function(td, cellData, rowData, row, col) {
                        var search = {
                            FuzzySearch: cellData,
                            searchType: "fuzzy",
                        };
                        var searchStr = JSON.stringify(search).urlSwitch();

                        var link = '/Orders/Index?search=' + searchStr;
                        jQuery(td).html("<div><a target='_blank' href='" + link + "' class='searchTBID'>" + cellData + "</a></div><div class='mini'>" + rowData.OrderSourseName + "</div>");
                    }
                },
                {
                    'data': 'ReceiveAddress',
                    "createdCell": function(td, cellData, rowData, row, col) {
                        jQuery(td).html('<div>' + cellData + '</div><div class="mini">' + rowData.ReceiveName + '</div>');
                    }
                },
                {
                    'data': 'TransferNum',
                    "createdCell": function(td, cellData, rowData, row, col) {
                        jQuery(td).html('<div>' + cellData + '元</div>');
                    }
                },
                //类型
                {
                    'data': 'TransferTypeName',
                    "createdCell": function(td, cellData, rowData, row, col) {
                        jQuery(td).html('<div>' + cellData + '</div>');
                    }
                },
                {
                    'data': 'OrderNo',
                    "createdCell": function(td, cellData, rowData, row, col) {
                        jQuery(td).html('<div>' + cellData + '</div>');
                    }
                },
                {
                    'data': 'CreateTime',
                    "createdCell": function(td, cellData, rowData, row, col) {
                        jQuery(td).html('<div>' + cellData + '</div><div>' + rowData.CreateName + '</div>');
                    }
                },
                {
                    'data': 'TransferReason',
                    "createdCell": function(td, cellData, rowData, row, col) {
                        jQuery(td).html('<div>' + cellData + '</div>');
                    }
                },
                {
                    'data': 'Remark',
                    "createdCell": function(td, cellData, rowData, row, col) {
                        jQuery(td).html(' <div>' + cellData + '</div>');
                    }
                },
                {
                    'data': 'TransferStateName',
                    "createdCell": function(td, cellData, rowData, row, col) {
                        switch (rowData.TransferStateValue) {
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
                    'data': 'AlipayTransferID',
                    "createdCell": function(td, cellData, rowData, row, col) {
                        var str = '';
                        if (rowData.TransferStateValue == 0) {
                            str += '<div>';
                            str += '<a class="hrefInTable-inline" href="/AlipayTransfers/Edit/' + cellData + '">修改</a>';
                            if (jQuery("#isCheck").val())
                                str += '<a class="hrefInTable-inline" href="javascript:;" id="Check">核实</a>';
                            if (jQuery("#isDelete").val())
                                str += '<a class="hrefInTable-inline" href="javascript:;" id="IsDelete">作废</a>';
                            str += '</div>';
                        } else if (rowData.TransferStateValue == 1) {
                            str += '<div>';
                            if (jQuery("#isTransfer").val())
                                str += '<a class="hrefInTable-inline" href="javascript:;" id="Transfer">转账</a>';
                            if (jQuery("#isDelete").val())
                                str += '<a class="hrefInTable-inline" href="javascript:;" id="IsDelete">作废</a>';
                            str += '</div>';
                        }
                        str += '<div><a class="hrefInTable-inline" href="javascript:;" id="RemarkShow">备注</a>' +
                            '<a class="hrefInTable-inline" href="/AlipayTransfers/Details/' + cellData + '">查看</a>' +
                            '</div>';
                        jQuery(td).html(str);
                    }
                }
            ]
        });

    //clearstate 
    function syncState(obj) {

        var method = {
            TransferStateValue: function(value) {
                $('#TransferState').find(".buttonradio[data-code=" + value + "]")
                    .addClass("active")
                    .siblings(".active").removeClass("active");
            },
            FuzzySearch: function(value) {
                $('#fuzzyString').val(value == null ? "" : value);
            },
            TransferTypeValue: function(value) {
                $('#TransferType option[value=' + value + ']').prop('selected', 'selected');
            }
        }
        if (obj == null) {
            $('#TransferState').find(".buttonradio:first")
                .addClass("active")
                .siblings('.buttonradio').removeClass("active");
            return;
        }
        for (var i in obj) {
            method[i](obj[i])
        }

    }

    //转账类型的筛选
    $("#TransferState").ButtonRadio({
        selected: function(dom, code) {
            if ($('#searchoption').length == 0) {
                $('body').append("<div id='searchoption' class='hidden'></div>");
            }
            $("#searchoption").data({
                search: {
                    TransferStateValue: code,
                    TransferTypeValue: $(".tabletools:eq(0) #TransferType").val()
                }
            })
            newAlipayTransfer.draw();
        }
    });
    //状态筛选
    $("#TransferType").bind('change', function() {
        if ($('#searchoption').length == 0) {
            $('body').append("<div id='searchoption' class='hidden'></div>");
        }
        var status = $(this).val();
        $("#searchoption").data({
            search: {
                TransferTypeValue: status,
                TransferStateValue: $(".tabletools:eq(0) #TransferState .buttonradio.active").data("code")
            }
        })
        newAlipayTransfer.draw();
    });
    // 模糊搜索
    jQuery("#fuzzySearch").bind("click", function() {
        var fuzzyString = jQuery("#fuzzyString").val().trim();
        if ($('#searchoption').length == 0) {
            $('body').append("<div id='searchoption' class='hidden'></div>");
        }
        $('#searchoption').data("search", {
            FuzzySearch: fuzzyString
        });
        newAlipayTransfer.draw();
    });

    //备注
    jQuery("body").on('click', '#RemarkShow', function(e, data) {

        var id = jQuery(this).closest("tr").attr("id");
        var oldReark = jQuery(this).closest("tr").find("td:eq(7)").text();

        $("#Remarksearch").one('shown.bs.modal', function() {
            jQuery("#Remark").val(oldReark)

            jQuery(this).find('#saveRemark').unbind().bind("click", function() {
                $.ajax({
                    type: 'post',
                    dataType: 'json',
                    contentType: "application/json; charset=utf-8;",
                    data: JSON.stringify({ id: id, Remark: jQuery.trim(jQuery("#Remark").val()) }),
                    url: '/AlipayTransfers/UpdateRemark',
                    success: function(data) {
                        if (data.ErrorCode == 200) {
                            newAlipayTransfer.draw();
                            $('#Remarksearch').modal('hide');
                            jQuery("#Remark").val('');
                        } else {
                            jQuery('#saveRemark').success(data.ErrorMessage);
                        }
                    }
                })
            })
        })

        $("#Remarksearch").modal();
    })
    jQuery("body").on('click', '#Check', function(e, data) {
        var _this = this;
        var id = jQuery(_this).closest("tr").attr("id");
        var state = 1; //Check
        var tbid = jQuery(_this).closest("tr").find("a.searchTBID").text();
        var type = jQuery(this).closest("tr").find("td:eq(3)").text();
        var num = jQuery(this).closest("tr").find("td:eq(2)").text();


        $.ajax({
            type: 'post',
            dataType: 'json',
            contentType: "application/json; charset=utf-8;",
            data: JSON.stringify({ id: id }),
            url: '/AlipayTransfers/SelectCount',
            success: function(data) {
                if (data.ErrorCode == 200) {
                    var tip1 = '请确认是否<span style="color:red">核实</span>以下记录：';
                    var tip2 = tbid + ' ' + type + ' ' + num;
                    if (data.count > 1) {
                        tip1 = '系统检测到90天内同一ID下还有 <span style="color:red">' + (data.count - 1) + '</span> 条转账记录，请确认是否<span style="color:red">核实</span>以下记录？';
                    }
                    jQuery.LangHua.confirm({
                        title: "提示信息",
                        tip1: tip1,
                        tip2: tip2,
                        confirmbutton: '确定',
                        cancelbutton: '取消',
                        data: null,
                        confirm: function() {
                            $.ajax({
                                type: 'post',
                                dataType: 'json',
                                contentType: "application/json; charset=utf-8;",
                                data: JSON.stringify({ id: id, state: state }),
                                url: '/AlipayTransfers/UpdateState',
                                success: function(data) {
                                    if (data.ErrorCode == 200) {
                                        jQuery(_this).success(data.ErrorMessage);
                                        newAlipayTransfer.draw();
                                    } else {
                                        jQuery(_this).success(data.ErrorMessage);
                                    }
                                }
                            })
                        }
                    })
                } else {
                    jQuery(_this).success(data.ErrorMessage);
                }
            }
        })
    })
    jQuery("body").on('click', '#Transfer', function(e, data) {
        var _this = this;
        var id = jQuery(_this).closest("tr").attr("id");
        var state = 2; //Transfer
        var tbid = jQuery(_this).closest("tr").find("a.searchTBID").text();
        var type = jQuery(this).closest("tr").find("td:eq(3)").text();
        var num = jQuery(this).closest("tr").find("td:eq(2)").text();
        jQuery.LangHua.confirm({
            title: "提示信息",
            tip1: '请确认是否<span style="color:red">转账</span>以下记录：',
            tip2: tbid + ' ' + type + ' ' + num,
            confirmbutton: '确定',
            cancelbutton: '取消',
            data: null,
            confirm: function() {
                $.ajax({
                    type: 'post',
                    dataType: 'json',
                    contentType: "application/json; charset=utf-8;",
                    data: JSON.stringify({ id: id, state: state }),
                    url: '/AlipayTransfers/UpdateState',
                    success: function(data) {
                        if (data.ErrorCode == 200) {
                            jQuery(_this).success(data.ErrorMessage);
                            newAlipayTransfer.draw();
                        } else {
                            jQuery(_this).success(data.ErrorMessage);
                        }
                    }
                })
            }
        })
    })
    jQuery("body").on('click', '#IsDelete', function(e, data) {
        var _this = this;
        var id = jQuery(_this).closest("tr").attr("id");
        var state = 3; //IsDelete
        var tbid = jQuery(_this).closest("tr").find("a.searchTBID").text();
        var type = jQuery(this).closest("tr").find("td:eq(3)").text();
        var num = jQuery(this).closest("tr").find("td:eq(2)").text();
        jQuery.LangHua.confirm({
            title: "提示信息",
            tip1: '请确认是否<span style="color:red">作废</span>以下记录：',
            tip2: tbid + ' ' + type + ' ' + num,
            confirmbutton: '确定',
            cancelbutton: '取消',
            data: null,
            confirm: function() {
                $.ajax({
                    type: 'post',
                    dataType: 'json',
                    contentType: "application/json; charset=utf-8;",
                    data: JSON.stringify({ id: id, state: state }),
                    url: '/AlipayTransfers/UpdateState',
                    success: function(data) {
                        if (data.ErrorCode == 200) {
                            jQuery(_this).success(data.ErrorMessage);
                            newAlipayTransfer.draw();
                        } else {
                            jQuery(_this).success(data.ErrorMessage);
                        }
                    }
                })
            }
        })
    })

    $('#reflashTable').bind("click", function() {
        newAlipayTransfer.draw()
    });









})