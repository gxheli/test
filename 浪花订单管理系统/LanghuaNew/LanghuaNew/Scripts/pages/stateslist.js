jQuery(document).ready(function($) {
    jQuery('#traveldate').find('input').datepicker();

    var newproduct = '-1';

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
            json['StateSearch'] = search;

            $('#reflashTable').find('span').addClass("fa-spin");


        }).on('xhr.dt', function(e, settings, json, xhr) {
            $('#orderList thead tr th:eq(0) input').prop("checked", false)
            syncState(json.SearchModel.StateSearch);

            $('#reflashTable').find('span').removeClass("fa-spin");

            $('#selectedNumber').text(0);
        })



    //发货状态的筛选
    $("#SendState").ButtonRadio({
        selected: function(dom, code) {
            if ($('#searchoption').length == 0) {
                $('body').append("<div id='searchoption' class='hidden'></div>");
            }
            var datetype = $('#datetype').val();
            var ordersourse = $('#ordersourse').val();
            var traveldate = $('#traveldate').find('input').val();
            if (!traveldate) {
                $('#traveldate').warning('日期不能为空');
                return;
            }
            $("#searchoption").data({
                search: {
                    DateValue: datetype,
                    OrderSourseID: ordersourse,
                    TravelDate: traveldate,
                    StateValue: code,
                    FuzzySearch: "",
                }
            })
            if (newproduct == "-1") {
                initTable();
            } else {
                newproduct.draw();
            }
        }
    })

    //筛选
    $("#screen").bind('click', function() {
            if ($('#searchoption').length == 0) {
                $('body').append("<div id='searchoption' class='hidden'></div>");
            }
            var datetype = $('#datetype').val();
            var ordersourse = $('#ordersourse').val();
            var traveldate = $('#traveldate').find('input').val();
            if (!traveldate) {
                $('#traveldate').warning('日期不能为空');
                return;
            }
            $("#searchoption").data({
                search: {
                    DateValue: datetype,
                    OrderSourseID: ordersourse,
                    TravelDate: traveldate,
                    StateValue: 1,
                    FuzzySearch: "",
                }
            })
            if (newproduct == "-1") {
                initTable();
            } else {
                newproduct.draw();
            }
        })
        // 模糊搜索
    jQuery("#fuzzySearch").bind("click", function() {

        var fuzzyString = jQuery("#fuzzyString").val().trim();
        if (!fuzzyString.length) {
            $('#fuzzyString').warning('搜索条件不能为空');
            return;
        }
        if ($('#searchoption').length == 0) {
            $('body').append("<div id='searchoption' class='hidden'></div>");
        }
        $('#searchoption').data("search", {
            FuzzySearch: fuzzyString,
        });
        if (newproduct == "-1") {
            initTable();
        } else {
            newproduct.draw();
        }
    })

    //clearstate 
    function syncState(obj) {
        var method = {
            FuzzySearch: function(value) {
                $('#fuzzyString').val(value == null ? "" : value);
                if (value != null && value != "") {
                    $('#SendState').find(".buttonradio").removeClass("active");
                }
            },
            StateValue: function(value) {
                $('#SendState').find(".buttonradio[data-code=" + value + "]")
                    .addClass("active")
                    .siblings(".active").removeClass("active");
            },
            DateValue: function(value) {
                $('#datetype option[value=' + value + ']').prop('selected', 'selected');
            },
            OrderSourseID: function(value) {
                $('#ordersourse option[value=' + value + ']').prop('selected', 'selected');
            },
            TravelDate: function(value) {
                $('#traveldate').val(value);
            },
        }
        if (obj == null) {
            $('#SendState').find(".buttonradio").removeClass("active");
            return;
        }
        for (var i in obj) {
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


    function initTable() {
        newproduct =
            jQuery('table#orderList')
            .DataTable({
                ajax: {
                    url: "/TBOrderStates/GetTBOrderStates",
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
                //列操作
                columns: [
                    //左格选择
                    {
                        'data': 'OrderSourseID',
                        "createdCell": function(td, cellData, rowData, row, col) {
                            jQuery(td).html('<input type="checkbox" class="checkboxes">');
                        }
                    },
                    {
                        'data': 'CustomerTBCode',
                        "createdCell": function(td, cellData, rowData, row, col) {
                            var search = {
                                FuzzySearch: cellData,
                                searchType: "fuzzy",
                            };
                            var searchStr = JSON.stringify(search).urlSwitch();

                            var link = '/Orders/Index?search=' + searchStr;
                            jQuery(td).html("<div><a target='_blank' href='" + link + "' class='searchTBID'>" + cellData + "</a></div>");
                        }
                    },
                    {
                        'data': 'OrderSourseName',
                        "createdCell": function(td, cellData, rowData, row, col) {
                            jQuery(td).html('<div>' + cellData + '</div>');
                        }
                    },
                    {
                        'data': 'mindate',
                        "createdCell": function(td, cellData, rowData, row, col) {
                            jQuery(td).html('<div>' + cellData.substring(0, 10) + '</div>');
                        }
                    },
                    {
                        'data': 'maxdate',
                        "createdCell": function(td, cellData, rowData, row, col) {
                            jQuery(td).html('<div>' + cellData.substring(0, 10) + '</div>');
                        }
                    },
                    {
                        'data': 'SendUserName',
                        "createdCell": function(td, cellData, rowData, row, col) {
                            jQuery(td).html('<div>' + cellData + '</div>');
                        }
                    },
                    {
                        'data': 'sendtime',
                        "createdCell": function(td, cellData, rowData, row, col) {
                            jQuery(td).html(' <div>' + (cellData < "1901-01-01" ? "" : cellData.substring(0, 19).replace('T', ' ')) + '</div>');
                        }
                    },
                    //状态
                    {
                        'data': 'IsSend',
                        "createdCell": function(td, cellData, rowData, row, col) {
                            jQuery(td).html(cellData == 1 ? "已发货" : ('<span style="color:red">' + "未发货" + '</span>'));
                        }
                    },
                    //操作
                    {
                        'data': 'CustomerTBCode',
                        "createdCell": function(td, cellData, rowData, row, col) {
                            var str = "<div class='row'>";
                            if (rowData.IsSend == 0) {
                                str += "<a class='hrefInTable-inline send' href='javascript:;' id='send' data-tbid='" + cellData + "' data-sourseid='" + rowData.OrderSourseID + "'>发货</a>";
                            }
                            str += "<a class='hrefInTable-inline copy' href='javascript:;' id='copy' data-tbid='" + cellData + "'>复制ID</a>";
                            str += "</div>";
                            jQuery(td).html(str);
                        }
                    }
                ]
            });
    }

    $('div').on('copy', '.copy', function(e) {
        var tbid = $(this).data('tbid');
        e.clipboardData.clearData();
        e.clipboardData.setData("text/plain", tbid);
        e.preventDefault();
        $(this).success("复制成功")
    })

    $('div').on('click', '.send', function(e) {
        var tbid = $(this).data('tbid');
        var sourseid = $(this).data('sourseid');
        var arr = [{
            OrderSourseID: sourseid,
            CustomerTBCode: tbid
        }]
        $.ajax({
            url: "/TBOrderStates/UpdateDisable",
            type: 'post',
            contentType: "application/json; charset=utf-8;",
            data: JSON.stringify({ TBID: arr, Operation: "Delivery" }),
            dataType: 'json',
            beforeSend: function() {},
            success: function(data) {
                newproduct.draw(false);
            }
        })
    })

    // 发货
    $('#operations').on("click", "a", function() {
        var number = parseInt($('#selectedNumber').text())
        if (number == 0) {
            jQuery(this).success("请至少选中一条记录");
            return
        }
        var arr = []
        $('#orderList tr.selectedRow').each(function() {
            arr.push({
                CustomerTBCode: $(this).find('a.send').data('tbid'),
                OrderSourseID: $(this).find('a.send').data('sourseid'),
            });
        })
        if (arr.length == 0) {
            console.log(arr)
            return;
        }
        $.ajax({
            url: "/TBOrderStates/UpdateDisable",
            type: 'post',
            contentType: "application/json; charset=utf-8;",
            data: JSON.stringify({ TBID: arr, Operation: "Delivery" }),
            dataType: 'json',
            beforeSend: function() {},
            success: function(data) {
                newproduct.draw(false);
            }
        })
    })
    $('#reflashTable').bind("click", function() {
        newproduct.draw()
    });

})