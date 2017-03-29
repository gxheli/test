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
            json['RuleSearch'] = search;
            json['OrderBy'] = jQuery("#OrderBy").data("OrderBy");

            $('#reflashTable').find('span').addClass("fa-spin");
        }).on('xhr.dt', function(e, settings, json, xhr) {
            $('#orderList thead tr th:eq(0) input').prop("checked", false)
            syncState(json.SearchModel.RuleSearch);

            $('#reflashTable').find('span').removeClass("fa-spin");

            $('#selectedNumber').text(0);
        })
    var newproduct =
        jQuery('table#orderList')
        .DataTable({
            ajax: {
                url: "/ServiceRules/GetRules",
                type: 'post',
            },
            ordering: false,
            searching: false,
            serverSide: true,
            initComplete: function(settings, json) {},
            drawCallback: function(settings) {
                // 更新页数
                var api = this.api();
                $("#cp.ddone").text(api.page.info().page + 1 + '/' + api.page.info().pages + '页');
                //更新提示数据
            },

            //行操作
            rowId: "ServiceRuleID",
            createdRow: function(row, data, dataIndex) {
                var _this = this.api();
                var thisTable = this;
                jQuery(row).attr('id', data.ServiceRuleID);
            },
            //列操作
            columns: [
                //左格选择
                {
                    'data': 'ServiceRuleID',
                    "createdCell": function(td, cellData, rowData, row, col) {
                        jQuery(td).html('<input type="checkbox" class="checkboxes">');
                    }
                },
                {
                    'data': 'RuleServiceItem',
                    "createdCell": function(td, cellData, rowData, row, col) {
                        if (cellData.length >= 2) {
                            jQuery(td).html(
                                '<div>' + cellData[0].cnItemName + " " + cellData[0].ServiceCode + '</div>' +

                                '<div class="more-list displayInlineBlock" >' +
                                '<a href="javascript:;" class="dropdown-toggle " data-toggle="dropdown">' +
                                '等多个产品' +
                                '</a>' +
                                '<ul class="dropdown-menu " role="menu" aria-labelledby="dLabel">' +
                                (function() {
                                    var str = '';
                                    for (var i = 1; i < cellData.length; i++) {
                                        str += '<li title="' + cellData[i].cnItemName + '&nbsp' + cellData[i].ServiceCode + '">' + cellData[i].cnItemName + '&nbsp' + cellData[i].ServiceCode + '</li>';
                                    }
                                    return str;
                                })() +
                                '</ul>' +

                                '</div>' +
                                '</div>'
                            );
                        } else {
                            jQuery(td).html('<div>' + cellData[0].cnItemName + " " + cellData[0].ServiceCode + '</div>');
                        }


                    }
                },
                {
                    'data': 'StartTime',
                    "createdCell": function(td, cellData, rowData, row, col) {
                        jQuery(td).html('<div>' + rowData.StartTime.substring(0, 10) + '<br/>' + rowData.EndTime.substring(0, 10) + '</div>');
                    }
                },
                {
                    'data': 'SelectRuleType',
                    "createdCell": function(td, cellData, rowData, row, col) {
                        var str = "";
                        str += rowData.RuleUseTypeValue == 0 ? "只允许：" : "禁止：";
                        switch (cellData) {
                            case 0:
                                str += rowData.RangeStart.substring(0, 10) + "~" + rowData.RangeEnd.substring(0, 10)
                                break;
                            case 1:
                                var weekstr = "";
                                var week = rowData.Week.split('|');
                                for (var i in week) {
                                    if (weekstr != "") {
                                        weekstr += " |";
                                    }
                                    switch (week[i]) {
                                        case "1":
                                            weekstr += "星期一";
                                            break;
                                        case "2":
                                            weekstr += "星期二";
                                            break;
                                        case "3":
                                            weekstr += "星期三";
                                            break;
                                        case "4":
                                            weekstr += "星期四";
                                            break;
                                        case "5":
                                            weekstr += "星期五";
                                            break;
                                        case "6":
                                            weekstr += "星期六";
                                            break;
                                        case "0":
                                            weekstr += "星期日";
                                            break;
                                    }
                                }
                                str += weekstr;
                                break;
                            case 2:
                                str += rowData.IsDouble ? "双号" : "单号";
                                break;
                            case 3:
                                str += rowData.UseDate;
                                break;
                        }
                        jQuery(td).html('<div>' + str + '</div>');
                    }
                },
                //状态
                {
                    'data': 'UseState',
                    "createdCell": function(td, cellData, rowData, row, col) {
                        jQuery(td).html(cellData == 0 ? "启用" : ('<span style="color:red">' + "禁用" + '</span>'));
                    }
                },
                //备注
                {
                    'data': 'Remark',
                    "createdCell": function(td, cellData, rowData, row, col) {
                        jQuery(td).html(cellData == null ? "" : cellData);
                    }
                },
                //操作
                {
                    'data': 'ServiceRuleID',
                    "createdCell": function(td, cellData, rowData, row, col) {
                        var str =
                            '<div class="row">' +
                            '<a id="RemarkShow" href="javascript:;" class="hrefInTable-inline" data-toggle="modal">备注</a>' +
                            '<a href="/ServiceRules/Edit/' + cellData + '" class="hrefInTable-inline">修改</a>' +
                            '<a href="/ServiceRules/RulesOperation/' + cellData + '" target="_blank" class="hrefInTable-inline">日志</a>' +
                            '</div>';
                        jQuery(td).html(str);
                    }
                }
            ]
        });

    //状态筛选
    $("#status").bind('change', function() {
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
    jQuery("#fuzzySearch").bind("click", function() {
        var fuzzyString = jQuery("#fuzzyString").val().trim();
        //if (!fuzzyString.length) {
        //    return;
        //}
        if ($('#searchoption').length == 0) {
            $('body').append("<div id='searchoption' class='hidden'></div>");
        }
        $('#searchoption').data("search", {
            FuzzySearch: fuzzyString,
            searchType: "FuzzySearch",

        });
        newproduct.draw();
    })

    //clearstate 
    function syncState(obj) {

        var method = {
            ServiceTypeID: function(value) {
                $('#ServiceType').find(".buttonradio[data-code=" + value + "]")
                    .addClass("active")
                    .siblings(".active").removeClass("active");
            },
            FuzzySearch: function(value) {

                $('#fuzzyString').val(value == null ? "" : value);
            },
            status: function(value) {
                $('#status option[value=' + value + ']').prop('selected', 'selected');
            },
            SupplierID: function(value) {

                $('#supplier option[value=' + value + ']').prop('selected', 'selected');

            }
        }
        if (obj == null) {
            $('#ServiceType').find(".buttonradio:first")
                .addClass("active")
                .siblings('.buttonradio').removeClass("active");
            return;
        }
        for (var i in obj) {
            if (i == "searchType") {
                continue;
            }
            method[i](obj[i])
        }

    }

    // 表格选择
    $('table.ddtable:first').on('change', '.group-checkable', function(e) {
        $(this).closest('table').find('tbody tr td input.checkboxes').prop('checked', $(this).prop("checked"));
        if ($(this).prop("checked")) {
            $(this).closest('table').find('tbody tr ').addClass("selectedRow");
        } else {
            $(this).closest('table').find('tbody tr ').removeClass("selectedRow");
        }
        var length = $('table.ddtable:first').find('tbody tr td input.checkboxes:checked').length
        $('#selectedNumber').text(length);
    })
    $('table.ddtable:first').on('change', 'input.checkboxes', function(e) {
            if ($(this).prop("checked")) {
                $(this).closest('tr').addClass("selectedRow");
            } else {
                $(this).closest('tr').removeClass("selectedRow");
            }
            var _this = this;
            var currentPageLength = newproduct.page.info().end - newproduct.page.info().start;

            if ($(this).closest("tbody").find("tr td input:checked").length != currentPageLength) {
                $(_this).closest("table.ddtable").find(".group-checkable:first").prop("checked", false);
            } else {
                $(_this).closest("table.ddtable").find(".group-checkable:first").prop("checked", true);
            }

            var length = $('table.ddtable:first').find('tbody tr td input.checkboxes:checked').length
            $('#selectedNumber').text(length);

        })
        // 状态流转
    $('#operations').on("click", "a", function() {
        var state = $(this).data('next-code');
        var number = parseInt($('#ordersCirculation #selectedNumber').text())
        if (number == 0) {
            jQuery(this).success("请至少选中一条记录");
            return
        }
        var bl = true;
        var arr = []
        $('#orderList tr.selectedRow').each(function() {
            arr.push($(this).attr('id'));
            if (jQuery(this).closest("tr").find("td:eq(4)").text() == "启用" && state == "delete") {
                bl = false;
                jQuery.LangHua.alert({
                    title: "提示信息",
                    tip1: '只能删除禁用状态的规则。',
                    tip2: '请先禁用再删除。',
                    button: '确定'
                })
            }
        })
        if (!bl) {
            return;
        }
        if (state == "delete") {
            jQuery.LangHua.confirm({
                title: "提示信息",
                tip1: '提示信息：',
                tip2: '您确定要删除吗？删除后将不能撤销！',
                confirmbutton: '确定',
                cancelbutton: '取消',
                data: null,
                confirm: function() {
                    updateState(state, arr)
                }
            });
        } else {
            updateState(state, arr);
        }
    })

    function updateState(state, arr) {
        var RuleID = arr.join(',');
        if (RuleID.length == 0) {
            return;
        }
        $.ajax({
            url: "/ServiceRules/UpdateDisable",
            type: 'post',
            contentType: "application/json; charset=utf-8;",
            data: JSON.stringify({
                RuleID: RuleID,
                Operation: state
            }),
            dataType: 'json',
            beforeSend: function() {},
            success: function(data) {
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
    //表格工具订单导出
    $('body').on('click', '#exportproducts', function(e) {

        var varURL = "/ServiceRules/ExportExcel?";
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

    //备注
    jQuery("body").on('click', '#RemarkShow', function(e, data) {

        var id = jQuery(this).closest("tr").attr("id");
        var oldReark = jQuery(this).closest("tr").find("td:eq(5)").text();

        $("#Remarksearch").one('shown.bs.modal', function() {
            jQuery("#Remark").val(oldReark)

            jQuery(this).find('#saveRemark').unbind().bind("click", function() {
                $.ajax({
                    type: 'post',
                    dataType: 'json',
                    contentType: "application/json; charset=utf-8;",
                    data: JSON.stringify({ id: id, Remark: jQuery.trim(jQuery("#Remark").val()) }),
                    url: '/ServiceRules/UpdateRemark',
                    success: function(data) {
                        if (data.ErrorCode == 200) {
                            newproduct.draw();
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

    $('#reflashTable').bind("click", function() {
        newproduct.draw()
    });


    $('.OrderBy').on('click', function() {

        if (!$(this).find('.caret:eq(0)').hasClass('status-gray')) {
            $(this).toggleClass('dropup');

        }
        $(this).find('.caret').removeClass('status-gray');

        $('.OrderBy').not(this).each(function() {
            $(this).find('.caret').addClass('status-gray');
        })

        if (jQuery("#OrderBy").length == 0) {
            $("body").append('<div id="OrderBy" class="hidden"></div>')
        }


        if (!$(this).hasClass("dropup")) {
            var obj = {
                OrderBy: 1, // 降序
                PropertyName: $(this).data('propertyname')
            }
        } else {
            var obj = {
                OrderBy: 0, // 升序
                PropertyName: $(this).data('propertyname')
            }
        }
        jQuery("#OrderBy").data('OrderBy', obj)
        newproduct.draw();

    })


})