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
            json['ItemSearch'] = search;

            $('#reflashTable').find('span').addClass("fa-spin");


        }).on('xhr.dt', function(e, settings, json, xhr) {
            $('#orderList thead tr th:eq(0) input').prop("checked", false)
            syncState(json.SearchModel.ItemSearch);

            $('#reflashTable').find('span').removeClass("fa-spin");

            $('#selectedNumber').text(0);
        })
    var newproduct =
        jQuery('table#orderList')
        .DataTable({
            ajax: {
                url: "/ServiceItems/GetItems",
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
            rowId: "ServiceItemID",
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
            columns: [
                //左格选择
                {
                    'data': 'ServiceCode',
                    "createdCell": function(td, cellData, rowData, row, col) {
                        jQuery(td).html('<input type="checkbox" class="checkboxes">');
                    }
                },
                {
                    'data': 'ServiceCode',
                    "createdCell": function(td, cellData, rowData, row, col) {
                        jQuery(td).html('<div  class=" ">' + rowData.ServiceCode + '</div>');
                    }
                },
                {
                    'data': 'cnItemName',
                    "createdCell": function(td, cellData, rowData, row, col) {
                        jQuery(td).html('<div>' + cellData + '</div>');
                    }
                },
                {
                    'data': 'enItemName',
                    "createdCell": function(td, cellData, rowData, row, col) {
                        jQuery(td).html('<div>' + cellData + '</div>');
                    }
                },
                {
                    'data': 'CityName',
                    "createdCell": function(td, cellData, rowData, row, col) {
                        jQuery(td).html('<div>' + cellData + '</div>');
                    }
                },
                //类型
                {
                    'data': 'ServiceTypeName',
                    "createdCell": function(td, cellData, rowData, row, col) {
                        jQuery(td).html('<div>' + cellData + '</div>');
                    }
                },
                //状态
                {
                    'data': 'ServiceItemEnableState',
                    "createdCell": function(td, cellData, rowData, row, col) {
                        jQuery(td).html(cellData == 0 ? "启用" : ('<span style="color:red">' + "禁用" + '</span>'));
                    }
                },

                // 默认供应商
                {
                    'data': 'DefaultSupplier',
                    "createdCell": function(td, cellData, rowData, row, col) {

                        jQuery(td).html('<div>' + cellData + '</div>');
                    }
                },
                // 保险天数
                {
                    'data': 'InsuranceDays',
                    "createdCell": function(td, cellData, rowData, row, col) {
                        jQuery(td).html(' <div>' + cellData + '</div>');
                    }
                },
                //价格
                {
                    'data': 'DefaultSupplierPriceState',
                    "createdCell": function(td, cellData, rowData, row, col) {
                        // 0 已设置
                        // 1 未设置
                        // 2 过期
                        var color = '#999999';
                        if (cellData == 1) {
                            color = 'red';
                        }
                        jQuery(td).html(cellData == 0 ? rowData.DefaultSupplierPriceStateName : ('<span style="color:' + color + '">' + rowData.DefaultSupplierPriceStateName + '</span>'));
                    }
                },
                //表单
                {
                    'data': 'isExistElement',
                    "createdCell": function(td, cellData, rowData, row, col) {
                        jQuery(td).html(cellData == true ? "√" : ('<span style="color:red">' + "未设置" + '</span>'));
                    }
                },
                //操作
                {
                    'data': 'ServiceItemID',
                    "createdCell": function(td, cellData, rowData, row, col) {
                        var search = '{"fuzzySearch":"' + rowData.ServiceCode + '"}';
                        var str =
                            "<div class='row'>" +
                            "<a target='_blank' href='/ServiceItems/Edit/" + cellData + "'>修改</a> | " +
                            "<a target='_blank' href='/ServiceItems/PriceSetting?ItemID=" + cellData + "'>价格</a> | " +
                            "<a target='_blank' href='/ServiceItems/FormSetting/" + cellData + "'>表单</a> | " +
                            "<a target='_blank' href='/ServiceRules/Index?search=" + search.urlSwitch() + "'>规则</a>" +
                            "</div>";
                        jQuery(td).html(str);
                    }
                }
            ]
        });

    //服务类型的筛选
    $("#ServiceType").ButtonRadio({
        selected: function(dom, code) {
            if ($('#searchoption').length == 0) {
                $('body').append("<div id='searchoption' class='hidden'></div>");

            }
            $("#searchoption").data({
                search: {
                    ServiceTypeID: code,
                    searchType: "ServiceTypeID",
                }
            })
            newproduct.draw();
        }
    })

    //状态筛选
    $("#status").bind('change', function() {
        if ($('#searchoption').length == 0) {
            $('body').append("<div id='searchoption' class='hidden'></div>");
        }

        var status = $(this).val();
        var SupplierID = $('#supplier').val();
        $("#searchoption").data({
            search: {
                status: status,
                SupplierID: SupplierID,
                searchType: "status&Supplier",

            }
        })
        newproduct.draw();
    })

    //供应商筛选
    $("#supplier").bind('change', function() {

        if ($('#searchoption').length == 0) {
            $('body').append("<div id='searchoption' class='hidden'></div>");

        }
        var SupplierID = $(this).val();
        var status = $('#status').val();
        $("#searchoption").data({
            search: {
                SupplierID: SupplierID,
                status: status,
                searchType: "status&Supplier",

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
    $('table#orderList').on('change', '.group-checkable', function(e) {
        $(this).closest('table').find('tbody tr td input.checkboxes').prop('checked', $(this).prop("checked"));
        if ($(this).prop("checked")) {
            $(this).closest('table').find('tbody tr ').addClass("selectedRow");
        } else {
            $(this).closest('table').find('tbody tr ').removeClass("selectedRow");
        }
        var length = $('table#orderList').find('tbody tr td input.checkboxes:checked').length
        $('#selectedNumber').text(length);
    })
    $('table#orderList').on('change', 'input.checkboxes', function(e) {
            if ($(this).prop("checked")) {
                $(this).closest('tr').addClass("selectedRow");
            } else {
                $(this).closest('tr').removeClass("selectedRow");
            }
            var _this = this;
            var currentPageLength = newproduct.page.info().end - newproduct.page.info().start;

            if ($(this).closest("tbody").find("tr td input:checked").length != currentPageLength) {
                $(_this).closest("table#orderList").find(".group-checkable:first").prop("checked", false);
            } else {
                $(_this).closest("table#orderList").find(".group-checkable:first").prop("checked", true);
            }

            var length = $('table#orderList').find('tbody tr td input.checkboxes:checked').length
            $('#selectedNumber').text(length);

        })
        // 状态流转
    $('#operations').on("click", "a", function() {
        var state = $(this).data('next-code');
        var number = parseInt($('#ordersCirculation #selectedNumber').text())
        if (number == 0) {
            return
        }
        var arr = []
        $('#orderList tr.selectedRow').each(function() {
            arr.push($(this).attr('id'));
        })
        var ItemID = arr.join(',');
        if (ItemID.length == 0) {
            return;
        }
        var tip = '';
        $.ajax({
            url: "/ServiceItems/UpdateDisable",
            type: 'post',
            contentType: "application/json; charset=utf-8;",
            data: JSON.stringify({
                ItemID: ItemID,
                Operation: state
            }),
            dataType: 'json',
            beforeSend: function() {
                tip = $.LangHua.blockToast({
                    tip: "正在处理..."
                })

            },
            success: function(data) {


                if (data.failed.length == 0) {
                    newproduct.draw(false);

                    return
                }
                newproduct.draw(false);
                var failed = data.failed;
                var str = '';
                var strs = '';
                for (var i in failed) {
                    var arr = [
                        '<div style="margin:10px 0px">',
                        '<span style="color:#0099cc">' + failed[i]['name'] + '：</span>',
                        '<spanstyle="color:#333" >' + failed[i]['reason'] + '</span>',
                        '</div>',
                    ].join('\n');
                    str += arr;

                }

                for (var i in failed) {
                    var arr = [

                        failed[i]['name'] + '：操作失败。 ' + failed[i]['reason'],
                        '\n',
                    ].join('\n');
                    strs += arr;

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
                    '<textarea id="Remark" class="form-control " rows="7" style="width:400px;margin-right:0px; display:inline-block;max-width:400px">',
                    strs,
                    '</textarea>',
                    '</div>',
                    '<div class="modal-footer">',
                    '<a  class="btn btn-sm btn-primary button70" id="" data-dismiss="modal">保存</a>',
                    '<a  class="btn btn-sm btn-default button70" data-dismiss="modal">关闭</a>',
                    '</div>',
                    '</div>',
                    '</div>',
                    '</div>',
                ].join("\n");
                $(t).modal();
            },
            complete: function() {
                tip.modal("hide");
                tip = null;
            }
        })
    })




    //表格工具订单导出
    $('body').on('click', '#exportproducts', function(e) {

        var varURL = "/ServiceItems/ExportExcel?";
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

    $('#reflashTable').bind("click", function() {
        newproduct.draw()
    });

    function statisticsSellControl() {

        // (function() {
        //     var itemSearch = new Bloodhound({
        //         datumTokenizer: function(d) {
        //             return Bloodhound.tokenizers.whitespace(d.name);
        //         },
        //         queryTokenizer: Bloodhound.tokenizers.whitespace,
        //         limit: 15,
        //         // 在文本框输入字符时才发起请求
        //         // 
        //         // local:dt,
        //         remote: {
        //             wildcard: '%QUERY',
        //             url: '/Orders/GetItemsByStr',
        //             prepare: function(xhr, settings) {
        //                 settings.dataType = 'json';
        //                 settings.type = 'POST';
        //                 settings.data = { Str: xhr };
        //                 return settings;
        //             },
        //             filter: function(data) {
        //                 return $.map(data.Items, function(one) {
        //                     return {
        //                         name: one.cnItemName,
        //                         supplyer: one.ItemSupliers,
        //                         defaultSupplierID: one.DefaultSupplierID,
        //                         serviceItemID: one.ServiceItemID,
        //                         code: one.ServiceCode,
        //                     };
        //                 });
        //             }

        //         }
        //     });
        //     itemSearch.initialize();

        //     $('body #guestStatistics #itemlistx').typeahead({
        //         hint: false,
        //         highlight: true,
        //         minLength: 1,
        //     }, {
        //         name: 'xxx',
        //         displayKey: "name",
        //         limit: 30,
        //         source: itemSearch,
        //         templates: {
        //             empty: [
        //                 '<div class="empty-message">',
        //                 '没有找到相关产品',
        //                 '</div>'
        //             ].join('\n'),
        //             pending: [
        //                 '<div class="empty-message">',
        //                 '正在搜索...',
        //                 '</div>'
        //             ].join('\n'),
        //             header: function(data) {
        //                 return ([
        //                     '<div class="empty-message">',
        //                     '共搜索到<strong>' + data.suggestions.length + '</strong>个产品',
        //                     '</div>'
        //                 ].join('\n'));
        //             },
        //             suggestion: Handlebars.compile('<div>{{name}}{{code}}</div>')
        //         }
        //     }).on('typeahead:select', function(ev, suggestion) {
        //         $(this).data('itemid', suggestion.serviceItemID);
        //     }).on("keydown", function(evt) {
        //         evt = (evt) ? evt : ((window.event) ? window.event : "")
        //         var key = evt.keyCode ? evt.keyCode : evt.which;
        //         if (key !== 9 && key !== 13) {
        //             $(this).data("itemid", "");
        //         }
        //     });
        // })();
        $("body #guestStatistics #rangeDate").datepicker({
            'inputs': $(".dateX"),
            'keepEmptyValues': true
        }).on('changeDate', function(e) {
            var startDate = $("body #guestStatistics #rangeDate .dateX:eq(0)").datepicker("getUTCDate");
            var endDate = $("body #guestStatistics #rangeDate .dateX:eq(1)").datepicker("getUTCDate");
            var startDateTimeStamp = startDate.valueOf();
            var endDateTimeStamp = endDate.valueOf();
            if (endDateTimeStamp - startDateTimeStamp > 1000 * 60 * 60 * 24 * 31 * 3) {
                $("body #guestStatistics #rangeDate").success("时间跨度不要超出3个月");
                var today = new Date();
                // $("body #guestStatistics #rangeDate .dateX:eq(0)").datepicker("setDate", today);
                $("body #guestStatistics #rangeDate .dateX:eq(1)").datepicker("setDate", startDate);
            }
        });

        $("body").on("shown.bs.modal", "#guestStatistics", function() {
            var dateStar;
            var now = new Date();
            dateStart = new Date(now.getFullYear(), now.getMonth(), 1, 0, 0, 0);
            $("body #guestStatistics #rangeDate .dateX:eq(0)").datepicker("setDate", dateStart);
            $("body #guestStatistics #rangeDate .dateX:eq(1)").datepicker("setDate", now);
            $("body #guestStatistics #statisticsSellControlResult tbody:eq(0)").empty();
            $("body #guestStatistics #itemlistx").data("itemid", "");
            // $("body #guestStatistics #itemlistx").typeahead("val", "");
            $("body #guestStatistics #itemlistx").val("");
            $("body #guestStatistics #simpleStatistics").hide();
            $("body #guestStatistics #simpleStatistics .personnum:eq(2)").prop("checked", false);
        });

        $("body #guestStatistics #getStatistics").on("click", function() {
            var id = $("body #guestStatistics #itemlistx").data("itemid");
            var name = $.trim($("body #guestStatistics #itemlistx").val());
            var StartDate = $("body #guestStatistics #rangeDate .dateX:eq(0)").val();
            var EndDate = $("body #guestStatistics #rangeDate .dateX:eq(1)").val();
            if (!name) {
                $("body #guestStatistics #itemlistx").formWarning({
                    tips: "请选择"
                });
                return;
            }
            if (!StartDate) {
                $(this).success("请选择开始日期");
                return;
            }
            if (!EndDate) {
                $(this).success("请选择结束日期");
                return;
            }
            $.ajax({
                type: 'GET',
                dataType: 'json',
                contentType: "application/json; charset=utf-8;",
                data: {
                    Name: name.urlSwitch(),
                    StartDate: StartDate,
                    EndDate: EndDate
                },
                url: '/ServiceItems/GetStatisticsSales',
                "beforeSend": function() {
                    $.LangHua.loadingToast({
                        tip: "正 在 获 取 统 计 信 息. . . . . ."
                    });
                },
                success: function(data) {
                    var openModals = $("body").data("modalmanager").getOpenModals();
                    var i;
                    if (openModals) {
                        for (i in openModals) {
                            if ($(openModals[i]['$element'][0]).attr("id") !== "guestStatistics") {
                                $(openModals[i]['$element'][0]).modal("hide");
                            }
                        }
                    }
                    if (data.ErrorCode == 200) {
                        var tr = $("<tr></tr>");
                        var td = $("<td></td>");
                        $("body #guestStatistics #statisticsSellControlResult tbody:eq(0)").empty();
                        if (data.StatisticsSales.length === 0) {
                            $("body #guestStatistics #statisticsSellControlResult tbody:eq(0)").append(
                                tr.append(
                                    td.prop("colspan", 6).text("没有相关数据")
                                )
                            );
                            return;
                        }

                        var arrTr = [];
                        var statistics = {
                            "AdultNum": 0,
                            "ChildNum": 0,
                            "INFNum": 0,
                            "ordersAccount": 0
                        };
                        for (i in data.StatisticsSales) {
                            statistics.AdultNum += parseInt(data.StatisticsSales[i].AdultNum);
                            statistics.ChildNum += parseInt(data.StatisticsSales[i].ChildNum);
                            statistics.INFNum += parseInt(data.StatisticsSales[i].INFNum);
                            statistics.ordersAccount += parseInt(data.StatisticsSales[i].OrderCount);
                            arrTr.push(
                                tr.clone().append(
                                    td.clone().text(data.StatisticsSales[i].ServiceCode),
                                    td.clone().text(data.StatisticsSales[i].ServiceName),
                                    td.clone().text(data.StatisticsSales[i].SupplierNo + "-" + data.StatisticsSales[i].SupplierName),
                                    td.clone().text(data.StatisticsSales[i].AdultNum),
                                    td.clone().text(data.StatisticsSales[i].ChildNum),
                                    td.clone().text(data.StatisticsSales[i].INFNum)
                                )
                            );
                        }
                        $("body #guestStatistics").data("statistics", statistics);

                        $("body #guestStatistics #statisticsSellControlResult tbody:eq(0)").append(arrTr);
                        if (statistics.ordersAccount === 0) {
                            $("body #guestStatistics #simpleStatistics").hide();
                        } else {
                            $("body #guestStatistics #simpleStatistics").show();
                            $("body #guestStatistics #simpleStatistics #adult").text("成人：" + statistics.AdultNum);
                            $("body #guestStatistics #simpleStatistics #child").text("儿童：" + statistics.ChildNum);
                            $("body #guestStatistics #simpleStatistics #inf").text("婴儿：" + statistics.INFNum);
                            // $("body #guestStatistics #simpleStatistics #personSum").text(+statistics.INFNum + "人");
                            $("body #guestStatistics #simpleStatistics #odersSum").text(statistics.ordersAccount + '单');

                            $("body #guestStatistics #simpleStatistics .personnum:eq(0)").prop("checked", true);
                            $("body #guestStatistics #simpleStatistics .personnum:eq(1)").prop("checked", true);
                            $("body #guestStatistics #simpleStatistics .personnum:eq(0)").trigger("change");


                        }
                        $("body").trigger("resize");
                    } else if (data.ErrorCode == 401) {
                        $.LangHua.alert({
                            tip1: "获取统计数据失败",
                            tip2: data.ErrorMessage,
                            button: '确定',
                        });
                    } else {
                        $.LangHua.alert({
                            title: "提示信息",
                            tip1: '拉取数据失败',
                            tip2: "拉取数据失败",
                            button: '确定'
                        })
                    }

                },
                "error": function(jqXHR, textStatus, errorThrown) {
                    var openModals = $("body").data("modalmanager").getOpenModals();
                    if (openModals) {
                        for (var i in openModals) {
                            if ($(openModals[i]['$element'][0]).attr("id") !== "guestStatistics") {
                                $(openModals[i]['$element'][0]).modal("hide");
                            }
                        }
                    }
                    $.LangHua.alert({
                        title: "提示信息",
                        tip1: '拉取数据失败',
                        tip2: "拉取数据失败",
                        button: '确定'
                    })
                }
            });
        });
        $("body #guestStatistics").on("change", ".personnum", function() {
            var statistics = $("body #guestStatistics").data('statistics');
            $("body #guestStatistics #simpleStatistics").show();
            var personnums = $("body #guestStatistics  .personnum:checked");
            var account = 0;
            for (var i = 0; i < personnums.length; i++) {
                if (personnums.eq(i).siblings('.vertical-middle:eq(0)').attr("id") === "adult") {
                    account += statistics.AdultNum;
                } else if (personnums.eq(i).siblings('.vertical-middle:eq(0)').attr("id") === "child") {
                    account += statistics.ChildNum;
                } else if (personnums.eq(i).siblings('.vertical-middle:eq(0)').attr("id") === "inf") {
                    account += statistics.INFNum;
                }
            }
            $("body #guestStatistics #simpleStatistics #personSum").text(account + "人");
        });
    }
    statisticsSellControl();

});