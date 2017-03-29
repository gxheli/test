$(document).ready(function () {
    var fuzzyString = document.getElementById("fuzzyString");
    fuzzyString.onkeydown = jump;
    function jump(event) {
        var event = event || window.event;
        if (event.keyCode == 13) {
            $('#fuzzySearch').trigger("click");
        }
    }

    $('#orderList').eq(0)
       .on('preXhr.dt', function (e, settings, json) {
           var search = ($('#searchoption').data("search"));
           // 删除插件无必要项目
           delete json.columns;
           delete json.order;
           delete json.search;
           json['WeixinSearch'] = search;

           $('#reflashTable').find('span').addClass("fa-spin");
       }).on('xhr.dt', function (e, settings, json, xhr) {
           $('#orderList thead tr th:eq(0) input').prop("checked", false)

           $('#reflashTable').find('span').removeClass("fa-spin");

           $('#selectedNumber').text(0);
       })
    var newproduct =
	jQuery('table#orderList')
        .DataTable({
            ajax: {
                url: "/WeixinMessages/GetWeixinMessages",
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
            rowId: "WeixinMessageID",
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
                //左格选择
                {
                    'data': 'WeixinMessageID',
                    "createdCell": function (td, cellData, rowData, row, col) {
                        jQuery(td).html('<input type="checkbox" class="checkboxes">');
                    }
                },
                {
                    'data': 'CountryID',
                    "createdCell": function (td, cellData, rowData, row, col) {
                        jQuery(td).html(rowData.WeixinCountry.CountryName);
                    }
                },
                {
                    'data': 'Message',
                    "createdCell": function (td, cellData, rowData, row, col) {
                        jQuery(td).html(cellData);
                    }
                },
                {
                    'data': 'Url',
                    "createdCell": function (td, cellData, rowData, row, col) {
                        jQuery(td).html(cellData);
                    }
                },
                {
                    'data': 'StartTime',
                    "createdCell": function (td, cellData, rowData, row, col) {
                        jQuery(td).html('<div>' + rowData.StartTime.substring(0, 10) + '</div><div>' + rowData.EndTime.substring(0, 10) + '</div>');
                    }
                },
                {
                    'data': 'LastEditDate',
                    "createdCell": function (td, cellData, rowData, row, col) {
                        jQuery(td).html(cellData.substring(0, 16).replace('T',' '));
                    }
                },
                {
                    'data': 'OperUserNickName',
                    "createdCell": function (td, cellData, rowData, row, col) {
                        jQuery(td).html(cellData);
                    }
                },
                //状态
                {
                    'data': 'WeixinMessageState',
                    "createdCell": function (td, cellData, rowData, row, col) {
                        jQuery(td).html(cellData == 0 ? "启用" : ('<span style="color:red">' + "禁用" + '</span>'));
                    }
                },
                //操作
                {
                    'data': 'WeixinMessageID',
                    "createdCell": function (td, cellData, rowData, row, col) {
                        var str =
                        '<div class="row">' +
                            '<a class="hrefInTable-inline" href="/WeixinMessages/Edit/' + cellData + '">修改</a>' +
                        '</div>';
                        jQuery(td).html(str);
                    }
                }
            ]
        });

    //状态筛选
    $("#status").bind('change', function () {
        if ($('#searchoption').length == 0) {
            $('body').append("<div id='searchoption' class='hidden'></div>");
        }
        var status = $(this).val();
        $("#searchoption").data({
            search: {
                status: status
            }
        })
        newproduct.draw();
    })
    //国家筛选
    $("#country").bind('change', function () {
        if ($('#searchoption').length == 0) {
            $('body').append("<div id='searchoption' class='hidden'></div>");
        }
        var country = $(this).val();
        $("#searchoption").data({
            search: {
                CountryID: country
            }
        })
        newproduct.draw();
    })
    // 模糊搜索
    jQuery("#fuzzySearch").bind("click", function () {
        var fuzzyString = jQuery("#fuzzyString").val().trim();
        if ($('#searchoption').length == 0) {
            $('body').append("<div id='searchoption' class='hidden'></div>");
        }
        $('#searchoption').data("search", {
            FuzzySearch: fuzzyString

        });
        newproduct.draw();
    })
    // 表格选择
    $('table.ddtable:first').on('change', '.group-checkable', function (e) {
        $(this).closest('table').find('tbody tr td input.checkboxes').prop('checked', $(this).prop("checked"));
        if ($(this).prop("checked")) {
            $(this).closest('table').find('tbody tr ').addClass("selectedRow");
        }
        else {
            $(this).closest('table').find('tbody tr ').removeClass("selectedRow");
        }
        var length = $('table.ddtable:first').find('tbody tr td input.checkboxes:checked').length
        $('#selectedNumber').text(length);
    })
    $('table.ddtable:first').on('change', 'input.checkboxes', function (e) {
        if ($(this).prop("checked")) {
            $(this).closest('tr').addClass("selectedRow");
        }
        else {
            $(this).closest('tr').removeClass("selectedRow");
        }
        var _this = this;
        var currentPageLength = newproduct.page.info().end - newproduct.page.info().start;

        if ($(this).closest("tbody").find("tr td input:checked").length != currentPageLength) {
            $(_this).closest("table.ddtable").find(".group-checkable:first").prop("checked", false);
        }
        else {
            $(_this).closest("table.ddtable").find(".group-checkable:first").prop("checked", true);
        }

        var length = $('table.ddtable:first').find('tbody tr td input.checkboxes:checked').length
        $('#selectedNumber').text(length);

    })
    // 状态流转
    $('#operations').on("click", "a", function () {
        var state = $(this).data('next-code');
        var number = parseInt($('#selectedNumber').text())
        if (number == 0) {
            jQuery(this).success("请至少选中一条记录");
            return
        }
        if (state == "delete") {
            jQuery.LangHua.confirm({
                title: "提示信息",
                tip1: '提示信息：',
                tip2: '您确定要删除吗？删除后将不能撤销！',
                confirmbutton: '确定',
                cancelbutton: '取消',
                data: null,
                confirm: function () {
                    updateState(state)
                }
            });
        }
        else {
            updateState(state)
        }
    })
    function updateState(state) {
        var arr = []
        $('#orderList tr.selectedRow').each(function () {
            arr.push($(this).attr('id'));
        })
        var id = arr.join(',');
        if (id.length == 0) {
            return;
        }
        $.ajax({
            url: "/WeixinMessages/UpdateDisable",
            type: 'post',
            contentType: "application/json; charset=utf-8;",
            data: JSON.stringify({
                id: id,
                Operation: state
            }),
            dataType: 'json',
            beforeSend: function () {
            },
            success: function (data) {
                if (data.failed.length == 0) {
                    newproduct.draw(false);

                    return
                }
                newproduct.draw(false);
                var failed = data.failed;
                var str = '';
                for (var i in failed) {
                    var arr = [
                    '<div style="margin:10px 0px">',
                        '<span style="color:#0099cc">' + failed[i]['name'] + '：</span>',
                        '<spanstyle="color:#333" >' + failed[i]['reason'] + '</span>',
                    '</div>',
                    ].join('\n');
                    str += arr;
                }
                var t = [
                        '<div  class="modal modal-animate" tabindex="-1" data-backdrop="static" data-width="500" data-height="200">',
                            '<div class="modal-dialog " role="document">',
                                '<div class="modal-content">',
                                '<div class="modal-header">',
                                    '<button type="button" class="close" data-dismiss="modal" aria-hidden="true"></button>',
                                    '<h4 class="modal-title">操作结果</h4>',
                                '</div>',
                                '<div class="modal-body">',
                                       str,
                                '</div>',
                                '<div class="modal-footer">',
                                    '<a  class="btn btn-sm btn-primary button70" data-dismiss="modal">确定</a>',
                               '</div>',
                               '</div>',
                               '</div>',
                        '</div>',
                ].join("\n");
                $(t).modal();
            }
        })
    }


    $('#reflashTable').bind("click", function () {
        newproduct.draw()
    });






})