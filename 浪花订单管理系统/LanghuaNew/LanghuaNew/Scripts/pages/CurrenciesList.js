jQuery(document).ready(function () {
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
           json['RuleSearch'] = search;

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
                url: "/Currencies/GetCurrencies",
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
            rowId: "CurrencyID",
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
                    'data': 'CurrencyID',
                    "createdCell": function (td, cellData, rowData, row, col) {
                        jQuery(td).html('<input type="checkbox" class="checkboxes">');
                    }
                },
                {
                    'data': 'CurrencyName',
                    "createdCell": function (td, cellData, rowData, row, col) {
                        jQuery(td).html(cellData);
                    }
                },
                {
                    'data': 'CurrencyNo',
                    "createdCell": function (td, cellData, rowData, row, col) {
                        jQuery(td).html(cellData);
                    }
                },
                {
                    'data': 'ExchangeRate',
                    "createdCell": function (td, cellData, rowData, row, col) {
                        if (rowData.CurrencyChangeType == 1) {
                            jQuery(td).html(cellData);
                        }
                        else {
                            jQuery(td).html('');
                        }
                    }
                },
                {
                    'data': 'ExchangeRate',
                    "createdCell": function (td, cellData, rowData, row, col) {
                        if (rowData.CurrencyChangeType == 0) {
                            jQuery(td).html(cellData);
                        }
                        else {
                            jQuery(td).html('');
                        }
                    }
                },
                //状态
                {
                    'data': 'CurrencyEnableState',
                    "createdCell": function (td, cellData, rowData, row, col) {
                        jQuery(td).html(cellData == 0 ? "启用" : ('<span style="color:red">' + "禁用" + '</span>'));
                    }
                },
                //操作
                {
                    'data': 'CurrencyID',
                    "createdCell": function (td, cellData, rowData, row, col) {
                        var str =
                        '<div class="row">' +
                            '<a href="/Currencies/Edit/' + cellData + '">修改</a>' +
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
                status: status,
                searchType: "status",
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
        var arr = []
        $('#orderList tr.selectedRow').each(function () {
            arr.push($(this).attr('id'));
        })
        var id = arr.join(',');
        if (id.length == 0) {
            return;
        }
        $.ajax({
            url: "/Currencies/UpdateDisable",
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
                newproduct.draw(false);
            }
        })
    })

    //表格工具订单导出
    $('body').on('click', '#exportproducts', function (e) {

        var varURL = "/Currencies/ExportExcel?";
        var link = document.createElement("a");
        var search = $('#searchoption').data("search");
        if (search) {
            if (!(search instanceof Object)) {
                search = JSON.parse(search)
            }
            else {
            }
        }
        else {

        }

        for (var i in search) {
            varURL += i + '=' + search[i] + "&";
        }
        console.log(varURL)

        link.href = varURL;
        document.body.appendChild(link);
        link.click();
        delete link;
    })

    $('#reflashTable').bind("click", function () {
        newproduct.draw()
    });
})