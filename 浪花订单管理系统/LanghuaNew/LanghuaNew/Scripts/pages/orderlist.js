jQuery(document).ready(function() {

    $("body").on("click", "#advancedsearch #status", function(e) {
        if ($(this).data("show") == "yes") {
            $(this).siblings("#stateMulti").css("display", "none");
            $(this).data("show", 'no');
            $(this).blur();
        } else {
            $(this).siblings("#stateMulti").css("display", "inline-block").scrollTop(0);
            $(this).data("show", 'yes');
        }
    });
    $("#advancedsearch").on("click", function(e) {
        if ($(e.target).hasClass("unitState") ||
            $(e.target).hasClass("typeText") ||
            $(e.target).hasClass("unitStateCtn") ||
            $(e.target).hasClass("unitStateCtnx") ||
            $(e.target).attr("id") === "status") {

        } else {
            $("#advancedsearch").find("#stateMulti").css("display", "none");
            $("#advancedsearch").find("#status").data("show", 'no');
            $("#advancedsearch").find("#status").blur();
        }
    });
    $("#advancedsearch").on("change", "#stateMulti .unitState", function(e) {
        var checkList = $("#advancedsearch #stateMulti .unitState:checked");
        var arrStateText = [];
        for (var i = 0; i < checkList.length; i++) {
            arrStateText.push(checkList.eq(i).siblings("span:eq(0)").text());
        }
        $("#advancedsearch").find("#status").val(arrStateText.join(",")).attr("title", arrStateText.join(","))
    })


    var searchPre = ($('#searchoption').data("search"));
    if (searchPre) {
        if (searchPre.FuzzySearchType) {

        } else {
            var FuzzySearchType = $.LangHuaCookie.get("orderListFuzzySearchType");
            if (FuzzySearchType) {
                $(".tabletools:eq(0) #FuzzySearchType").val(FuzzySearchType);
                $(".tabletools:eq(0) #fuzzyString").attr("placeholder", "搜索：" + $(".tabletools:eq(0) #FuzzySearchType option:selected").text());
            }
        }
    } else {
        var FuzzySearchType = $.LangHuaCookie.get("orderListFuzzySearchType");
        if (FuzzySearchType) {
            $(".tabletools:eq(0) #FuzzySearchType").val(FuzzySearchType);
            $(".tabletools:eq(0) #fuzzyString").attr("placeholder", "搜索：" + $(".tabletools:eq(0) #FuzzySearchType option:selected").text());
        }
    }

    // 订单所有状态进行处理
    if ($('#allState').text()) {
        var allState = JSON.parse($('#allState').text());
        for (var i in allState) {
            allState[i] = allState[i].split('|')[0]
        }
        $('#allState').text(JSON.stringify(allState))
    }

    /*对表格进行初始化处理*/
    //表格数据交互控制
    $('#orderList').eq(0)
        .on('preXhr.dt', function(e, settings, json) {
            // 删除插件无必要项目
            delete json.columns;
            delete json.order;
            delete json.search;
            var search = ($('#searchoption').data("search"));
            json['OrderBy'] = jQuery("#OrderBy").data("OrderBy");
            json['OrderSearch'] = search;
            $('#reflashTable').find('.fa').addClass("fa-spin");
        }).on('xhr.dt', function(e, settings, json, xhr) {
            $('#reflashTable').find('.fa').removeClass("fa-spin");
            $('#orderList thead tr th:eq(0) input').prop("checked", false)

            var searchobj = json.SearchModel.OrderSearch || null;
            if (searchobj == null || searchobj['searchType'] == "state" || searchobj['searchType'] == "fuzzy") {
                $('#advancedviewer').trigger('update', [
                    [], searchobj
                ]);
                $('head title:eq(0)').text("订单-订单列表");
                if (searchobj == null) {
                    $('#testb a:first').addClass("active").siblings().removeClass("active");
                    return
                } else if (searchobj['searchType'] == "state") {
                    $('#fuzzyString').val("");
                    $('#FuzzySearchType').val($('#FuzzySearchType').find("option:eq(0)").val());
                } else if (searchobj['searchType'] == "fuzzy") {
                    if (searchobj['FuzzySearch']) {
                        $('head title:eq(0)').text(searchobj['FuzzySearch'] + "-订单-订单列表");
                        $('.tabletools:eq(0) #FuzzySearchType').val(searchobj.FuzzySearchType);
                    }
                    $('#testb a:first').addClass("active").siblings().removeClass("active");
                    $('#fuzzyString').val(searchobj['FuzzySearch']);
                }
                return
            }
            var temp = {
                status: {
                    text: "状态",
                },
                OrderSourseID: {
                    text: "来源"
                },
                SupplierID: {
                    text: "供应商"
                },
                ItemName: {
                    text: '预订项目'
                },

                isOneself: {
                    text: '本人订单',
                },
                CreateName: {
                    text: '创建人'
                },
                isUrgent: {
                    text: '紧急订单',
                },
                IsChangeTravelDate: {
                    text: '只查变更中的出行日期',
                },


                TravelDateBegin: {
                    text: "出行"
                },
                ReturnDateBegin: {
                    text: "返回"
                },
                OrderCreateDateBegin: {
                    text: "创建"
                },
                OrderSendDateBegin: {
                    text: "发送"
                },
                IsPay: {
                    text: "未付完"
                },
                SupplierEnableOnline: {
                    text: "供应商使用本系统"
                },
                IsNeedCustomerService: {
                    text: "要售后"
                }
            }
            var arrs = [];
            for (var i in temp) {
                if (
                    searchobj[i] === null ||
                    searchobj[i] === 0
                ) {
                    continue
                };
                var obj = new Object();
                obj.text = temp[i]['text'];
                delete obj.search;

                if (i == "ReturnDateBegin") {
                    var arr = [];
                    arr.push({
                        props: 'ReturnDateBegin',
                        value: searchobj['ReturnDateBegin'].substr(0, 10),
                        alt: searchobj['ReturnDateBegin'].substr(0, 10)
                    });
                    arr.push({
                        props: 'ReturnDateEnd',
                        value: searchobj['ReturnDateEnd'].substr(0, 10),
                        alt: searchobj['ReturnDateEnd'].substr(0, 10)
                    });

                    obj.search = JSON.stringify({
                        ReturnDateBegin: searchobj['ReturnDateBegin'].substr(0, 10),
                        ReturnDateEnd: searchobj['ReturnDateEnd'].substr(0, 10),
                    })
                } else if (i == 'TravelDateBegin') {
                    var arr = [];
                    arr.push({
                        props: 'TravelDateBegin',
                        value: searchobj['TravelDateBegin'].substr(0, 10),
                        alt: searchobj['TravelDateBegin'].substr(0, 10)

                    });
                    arr.push({
                        props: 'TravelDateEnd',
                        value: searchobj['TravelDateEnd'].substr(0, 10),
                        alt: searchobj['TravelDateEnd'].substr(0, 10)
                    });

                    obj.search = JSON.stringify({
                        TravelDateBegin: searchobj['TravelDateBegin'].substr(0, 10),
                        TravelDateEnd: searchobj['TravelDateEnd'].substr(0, 10),

                    })
                } else if (i == "OrderCreateDateBegin") {
                    var arr = [];
                    arr.push({
                        props: 'OrderCreateDateBegin',
                        value: searchobj['OrderCreateDateBegin'].substr(0, 10),
                        alt: searchobj['OrderCreateDateBegin'].substr(0, 10)
                    });
                    arr.push({
                        props: 'OrderCreateDateEnd',
                        value: searchobj['OrderCreateDateEnd'].substr(0, 10),
                        alt: searchobj['OrderCreateDateEnd'].substr(0, 10)
                    });

                    obj.search = JSON.stringify({
                        OrderCreateDateBegin: searchobj['OrderCreateDateBegin'].substr(0, 10),
                        OrderCreateDateEnd: searchobj['OrderCreateDateEnd'].substr(0, 10),

                    })
                } else if (i == "OrderSendDateBegin") {
                    var arr = [];
                    arr.push({
                        props: 'OrderSendDateBegin',
                        value: searchobj['OrderSendDateBegin'].substr(0, 10),
                        alt: searchobj['OrderSendDateBegin'].substr(0, 10)
                    });
                    arr.push({
                        props: 'OrderSendDateEnd',
                        value: searchobj['OrderSendDateEnd'].substr(0, 10),
                        alt: searchobj['OrderSendDateEnd'].substr(0, 10)
                    });
                    obj.search = JSON.stringify({
                        OrderSendDateBegin: searchobj['OrderSendDateBegin'].substr(0, 10),
                        OrderSendDateEnd: searchobj['OrderSendDateEnd'].substr(0, 10),
                    })
                } else {
                    var arr = [];

                    if (i == "OrderSourseID") {
                        arr.push({
                            props: i,
                            value: searchobj[i],
                            alt: searchobj['OrderSourseName']
                        });


                        obj.search = JSON.stringify({
                            OrderSourseID: searchobj[i],
                            OrderSourseName: searchobj['OrderSourseName']

                        })
                    } else if (i == "SupplierID") {
                        arr.push({
                            props: i,
                            value: searchobj[i],
                            alt: searchobj['SupplierName']
                        });

                        obj.search = JSON.stringify({
                            SupplierID: searchobj[i],
                            SupplierName: searchobj['SupplierName']

                        })
                    } else if (i == "status") {
                        arr.push({
                            props: i,
                            value: searchobj[i],
                            alt: searchobj['statusNamae']
                        });

                        obj.search = JSON.stringify({
                            status: searchobj[i],
                            statusNamae: searchobj['statusNamae']

                        })
                    } else if (i == "ItemName") {
                        arr.push({
                            props: i,
                            value: searchobj[i],
                            alt: searchobj[i]
                        });

                        obj.search = JSON.stringify({
                            ItemName: searchobj[i],

                        })
                    } else if (i == "CreateName") {
                        arr.push({
                            props: i,
                            value: searchobj[i],
                            alt: searchobj[i]
                        });

                        obj.search = JSON.stringify({
                            CreateName: searchobj[i],

                        })
                    } else if (i == "SupplierEnableOnline") {
                        arr.push({
                            props: i,
                            value: searchobj[i],
                            alt: searchobj["SupplierEnableOnlineName"]
                        });

                        obj.search = JSON.stringify({
                            SupplierEnableOnline: searchobj[i],

                        })
                    } else if (i == "isOneself") {
                        arr.push({
                            props: i,
                            value: searchobj[i],
                            alt: ''
                        });

                        obj.search = JSON.stringify({
                            isOneself: searchobj[i],

                        })
                    } else if (i == "IsChangeTravelDate") {
                        arr.push({
                            props: i,
                            value: searchobj[i],
                            alt: ''
                        });

                        obj.search = JSON.stringify({
                            IsChangeTravelDate: searchobj[i],

                        })
                    } else if (i == "isUrgent") {
                        arr.push({
                            props: i,
                            value: searchobj[i],
                            alt: ''
                        });

                        obj.search = JSON.stringify({
                            isUrgent: searchobj[i],

                        })
                    } else if (i == "IsNeedCustomerService") {
                        arr.push({
                            props: i,
                            value: searchobj[i],
                            alt: ""
                        });

                        obj.search = JSON.stringify({
                            IsNeedCustomerService: searchobj[i],

                        })
                    } else if (i == "IsPay") {
                        arr.push({
                            props: i,
                            value: searchobj[i],
                            alt: ""
                        });

                        obj.search = JSON.stringify({
                            IsPay: searchobj[i],

                        })
                    }
                }
                obj.value = arr;
                arrs.push(obj);
            }
            $('#advancedviewer').trigger('update', [arrs, searchobj])

            json = {}
        })

    // 表格非数据逻辑
    var neworders =
        jQuery('table#orderList')
        .DataTable({

            ajax: {
                url: "/Orders/GetOrders",
                type: 'post',
            },
            ordering: false,
            searching: false,
            serverSide: true,

            drawCallback: function(settings) {
                var api = this.api();
                //更新提示数据
                $(this).off("click", '.changeLable');
                $(this).one("click", ".changeLable", function(e) {
                    var thisTable = $(this);
                    var thisTr = thisTable.closest("tr");
                    var Type = thisTable.data('operation');
                    var OrderID = thisTable.closest("tr").attr("id");

                    $.ajax({
                        url: "/Orders/OrderAfterSaleOperation",
                        type: 'post',
                        contentType: "application/json; charset=utf-8;",
                        data: JSON.stringify({
                            Type: Type,
                            OrderID: OrderID
                        }),
                        dataType: 'json',
                        beforeSend: function() {},
                        success: function(data) {
                            update = {
                                1: function() {
                                    thisTr.find('td:eq(7)').find("div:eq(0)").after('<div class="IsNeedCustomerService"><span class="badge-rectangle-default after-sale-service-color">要售后</span></div>');
                                },
                                2: function() {

                                    thisTr.find('td:eq(7)').find("div:eq(0) .IsNeedCustomerService").remove();
                                },

                                3: function() {

                                    thisTr.find('td:eq(7)').find("div:eq(0)").after('<div class="IsPay"><span class="badge-rectangle-default after-sale-service-color">未付完</span></div>');
                                },
                                4: function() {

                                    thisTr.find('td:eq(7)').find("div:eq(0) .IsPay").remove();
                                }

                            }
                            var map = {
                                1: {
                                    label: "IsNeedCustomerService",
                                    value: true
                                },
                                2: {
                                    label: "IsNeedCustomerService",
                                    value: false
                                },
                                3: {
                                    label: "IsPay",
                                    value: true
                                },
                                4: {
                                    label: "IsPay",
                                    value: false
                                },
                                5: {
                                    label: "isUrgent",
                                    value: true
                                },
                                6: {
                                    label: "isUrgent",
                                    value: false
                                },
                            }
                            if (data.ErrorCode == 200) {
                                // update[data.Type]();
                                var dataRow = api.row(thisTr).data();
                                dataRow[map[data.Type]['label']] = map[data.Type]['value']
                                api
                                    .row(thisTr)
                                    .data(dataRow)
                                    .draw('page');
                            }
                        }
                    })
                });
                // $(this).off("copy");
                // // var client = new ZeroClipboard(this);
                // $(this).on("copy", "a.toCopy", function(e) {
                //     //复制链接
                //     e.clipboardData.clearData();
                //     e.clipboardData.setData("text/plain", $('#URL').text() + $(this).data('tborderid'));
                //     e.preventDefault();
                //     jQuery(this).success("复制成功！")
                // });


                // $(this).on("copy", "a.copyEmailAddress", function(e) {
                //     //复制链接
                //     e.clipboardData.clearData();
                //     e.clipboardData.setData("text/plain", $(this).data('emailaddress'));
                //     e.preventDefault();
                //     jQuery(this).success("复制成功！")
                // });
                $(this).off("click", '.copyEmailAddress');
                $(this).off("click", '.toCopy');
                $(this).on("click", "a.copyEmailAddress", function(e) {
                    var copystr = $(this).data('emailaddress');
                    console.log(copystr)
                    $("#globalCopyCatcher.global").trigger('updateCopyboard', [copystr, $(this)]);
                })
                $(this).on("click", "a.toCopy", function(e) {
                    var copystr = $('#URL').text() + $(this).data('tborderid');
                    $("#globalCopyCatcher.global").trigger('updateCopyboard', [copystr, $(this)]);
                })



            },

            //行操作
            rowId: "ServiceItemID",
            createdRow: function(row, data, dataIndex) {
                var _this = this.api();
                var thisTable = this;
                // jQuery(row).on('click', '.cancel', function () {
                //     _this
                // 		.row(row)
                // 		.remove()
                // 		.draw();
                // });
                // //缓存有用的单行数据
                // jQuery(row).data({

                // });

                jQuery(row).attr('id', data.OrderID);

                if (data.state != 6 &&
                    data.state != 11 &&
                    data.state != 13
                ) {
                    if (data.TravelDate) {
                        var arr = data.TravelDate.split('-');

                        var TravelDate = new Date(arr[0], arr[1] - 1, arr[2]);
                        var NowDate = new Date();
                        var dateNow = NowDate.getDate();
                        var yearNow = NowDate.getFullYear();
                        var monthNow = NowDate.getMonth();
                        NowDate = new Date(yearNow, monthNow, dateNow);
                        TravelDate = TravelDate.valueOf();

                        NowDate = NowDate.valueOf();
                        var threeDay = 3 * 24 * 60 * 60 * 1000;
                        if (
                            TravelDate <= (parseInt(NowDate) + parseInt(threeDay))
                        ) {
                            $(row).addClass("yellow");
                        }

                    }
                }




            },

            //列操作
            columns: [

                //左格选择
                {
                    'data': 'AdultNum',
                    "render": function(cellData, type, rowData, meta) {
                        return '<input type="checkbox" class="checkboxes">';
                    }
                },

                //订单号
                {
                    'data': 'OrderNo',
                    "render": function(cellData, type, rowData, meta) {
                        return ('<div  class="bright"><a  target="_blank" href="/Orders/Details/' + rowData.OrderID + '">' + cellData + '</a></div><div class="mini">' + rowData.CreateUserNikeName + '</div>');
                    }
                },

                //淘宝id
                {
                    'data': 'TBID',
                    "render": function(cellData, type, rowData, meta) {
                        return ('<div><a href="javascript:;" class="searchTBID">' + cellData + '</a></div><div class="mini">' + rowData.OrderSourseName + '</div>');
                    }
                },
                // 姓名
                {
                    'data': 'CustomerName',
                    "render": function(cellData, type, rowData, meta) {
                        return ('<div><a target="_blank" href="/Customers/Details/' + rowData.CustomerID + '">' + cellData + '</a></div><div class="mini">' + rowData.Tel + '</div>');
                    }
                },

                //预定项目
                {
                    'data': 'cnItemName',
                    "render": function(cellData, type, rowData, meta) {
                        return ('<span>' + '（' + rowData.SupplierCode + '）' + cellData + '</span><span class="bright"><a href="javascript:;" class="searchcnItemName">  ' + rowData.ServiceCode + '</a></span>');
                    }
                },

                //人数
                {
                    'data': 'AdultNum',
                    "render": function(cellData, type, rowData, meta) {
                        if (rowData.RoomNum * rowData.NightNum > 0) {
                            return (rowData.RoomNum + '间 / ' + rowData.NightNum + '晚');
                        } else {
                            return (cellData + ' / ' + rowData.ChildNum + ' / ' + rowData.INFNum);
                        }
                    }
                },
                //日期
                {
                    'data': 'TravelDate',
                    "render": function(cellData, type, rowData, meta) {
                        var saletag = '';
                        if (rowData.isUrgent == true) {
                            saletag = '<div class="isUrgent"><span class="badge-rectangle-default font11 urgent-service-color">紧急</span></div>';
                        }
                        return (' <div>' + cellData + '</div>' + saletag);


                    },
                    createdCell: function(td, cellData, rowData, row, col) {
                        if (rowData.TravelDate) {
                            var arr = rowData.TravelDate.split('-');

                            var TravelDate = new Date(arr[0], arr[1] - 1, arr[2]);
                            var NowDate = new Date();
                            var dateNow = NowDate.getDate();
                            var yearNow = NowDate.getFullYear();
                            var monthNow = NowDate.getMonth();
                            NowDate = new Date(yearNow, monthNow, dateNow);
                            TravelDate = TravelDate.valueOf();
                            NowDate = NowDate.valueOf();
                            var sixMonth = 6 * 30 * 24 * 60 * 60 * 1000;
                            if (TravelDate > (parseInt(NowDate) + parseInt(sixMonth))) {
                                $(td).css("background", "#FF6600")
                            }

                        }
                    }
                },

                //状态
                {
                    'data': 'state',
                    "render": function(cellData, type, rowData, meta) {
                        var tmp = {
                            0: "#0000ff",
                            1: "#0099ff",
                            2: "#008000",
                            3: "#FF0000",
                            4: "#FF0000",
                            5: "#800080",
                            6: "#000000",
                            7: "#800080",
                            8: "#ff6600",
                            9: "#cc0000",
                            10: "#800080",
                            11: "#66666",
                            12: "#cc0000",
                            13: "#868686",
                            14: "#FF0000",
                            15: "#FF0000"
                        };
                        var style = 'style="color:' + tmp[cellData];;
                        if (cellData == 13) {
                            style += ";text-decoration:line-through;"
                        }
                        style += '"';
                        var text = allState[cellData];
                        if (rowData.IsNeedCustomerService == true) {
                            var saletag = '<div class="IsNeedCustomerService"><span class="badge-rectangle-default font11 after-sale-service-color">要售后</span></div>';
                        } else
                            var saletag = '';

                        if (rowData.IsPay == true) {
                            var uppaidtag = '<div class="IsPay"><span class="badge-rectangle-default font11 need-pay-color">未付完</span></div>';
                        } else
                            var uppaidtag = '';
                        return (
                            '<div ' + style + '>' + text + '</div>' +
                            saletag + uppaidtag
                        );
                    }
                },

                //备注
                {
                    'data': 'Remark',

                },
                //操作
                {
                    'data': 'OrderID',
                    "render": function(cellData, type, rowData, meta) {
                        if (rowData.IsNeedCustomerService == true) {
                            var saletext = '<li><a class="cancelaftersale changeLable"  data-operation=2 href="javascript:;">取消要售后</a></li>';
                        } else
                            var saletext = '<li><a class="getaftersale changeLable"  data-operation=1 href="javascript:;">要售后</a></li>';

                        if (rowData.IsPay == true) {
                            var paidtext = '<li><a class="cancelnotpaid changeLable"  data-operation=4 href="javascript:;">取消未付完</a></li>';
                        } else
                            var paidtext = '<li><a class="setnotpaid changeLable"   data-operation=3 href="javascript:;">未付完</a></li>';
                        if (rowData.isUrgent == true) {
                            var urgenttext = '<li><a class="cancelnotpaid changeLable"  data-operation=6 href="javascript:;">取消紧急订单</a></li>';
                        } else
                            var urgenttext = '<li><a class="setnotpaid changeLable"   data-operation=5 href="javascript:;">紧急订单</a></li>';
                        var AlipayTransfers = encodeURI('/AlipayTransfers/Create?OrderSourseID=' + rowData.OrderSourseID + '&TBID=' + (rowData.TBID).urlSwitch() + '&OrderNo=' + rowData.OrderNo);
                        var str =
                            '<div class="order-menu-in-table">' +
                            '<div class="one-row">' +
                            '<div class="tri"><a target="_blank" href="/Orders/Edit/' + cellData + '" class="">修改</a></div>' +
                            '<div class="tri"><a id="RemarkShow" href="#Remarksearchx" class=" Remarksearch" data-toggle="modal">留言</a></div>' +
                            '<div class="tri"><a  target="_blank" href="/Orders/TBOrderDetail/' + rowData.TBOrderID + '" class=" Remarksearch" data-toggle="modal">查看&nbsp&nbsp&nbsp</a></div>' +
                            '</div>' +
                            '<div class="one-row">' +
                            '<div class="tri"><a data-emailAddress="' + rowData.Email + '" href="javascript:" class="copyEmailAddress ">邮箱</a></div>' +
                            '<div class="tri"><a class="toCopy"  data-TBOrderID =' + rowData.TBOrderID + ' href="javascript:;">链接</a></div>' +
                            '<div class="tri">' +
                            '<div class="dpdContainer down rightBased  ">' +
                            '<a href="#" class="dropdown-toggle " data-toggle="dropdown">' +
                            '更多' +
                            '<b class="caret"></b>' +
                            '</a>' +
                            '<ul class="dropdown-menu " role="menu">' +
                            saletext +
                            paidtext +
                            urgenttext +
                            '<li><a id="Invalid" href="javascript:;">订单作废</a></li>' +
                            '<li><a href="/Customers/Details/' + rowData.CustomerID + '" target="_blank">客户详情</a></li>' +
                            '<li><a href=' + AlipayTransfers + ' " target="_blank">支付宝转账</a></li>' +
                            '<li><a href="/Orders/OrderOperation/' + cellData + '" target="_blank">操作日志</a></li>' +
                            '</ul>' +
                            '</div>' +
                            '</div>' +
                            '</div>' +
                            '</div>';
                        return (str);

                    }
                }
            ]
        });

    // 表格选择
    $('table#orderList').on('change', '.group-checkable', function(e) {
        $(this).closest('table').find('tbody tr td input.checkboxes').prop('checked', $(this).prop("checked"));
        if ($(this).prop("checked")) {
            $(this).closest('table').find('tbody tr ').addClass("selectedRow");
        } else {
            $(this).closest('table').find('tbody tr ').removeClass("selectedRow");
        }
        var length = $('table#orderList').find('tbody tr td input.checkboxes:checked').length;
        $('#selectedNumber').text(length);
    })
    $('table#orderList').on('change', 'input.checkboxes', function(e) {
        if ($(this).prop("checked")) {
            $(this).closest('tr').addClass("selectedRow");
        } else {
            $(this).closest('tr').removeClass("selectedRow");
        }


        var _this = this;
        var currentPageLength = neworders.page.info().end - neworders.page.info().start;

        if ($(this).closest("tbody").find("tr td input:checked").length != currentPageLength) {
            $(_this).closest("table#orderList").find(".group-checkable:first").prop("checked", false);
        } else {
            $(_this).closest("table#orderList").find(".group-checkable:first").prop("checked", true);
        }

        var length = $('table#orderList').find('tbody tr td input.checkboxes:checked').length
        $('#selectedNumber').text(length);

    })

    // 订单流转
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
        var OrderID = arr.join(',');
        if (OrderID.length == 0) {
            return;
        }
        if (parseInt(state) === 5) {
            $("#reasonOrderCancel #typeReason input[name=typeReason]").each(function() {
                $(this).prop("checked", false);
            });
            $("#reasonOrderCancel #more").val("");
            $("#reasonOrderCancel").data("post", {
                OrderID: OrderID,
                state: state,
                Remark: ""
            });
            $("#reasonOrderCancel").modal("show");
            return;
        } else {
            stateCHangeSend(
                neworders, {
                    OrderID: OrderID,
                    state: state,
                    Remark: ""
                });
        }
    });
    $('body').on("click", '#cancelConfirm', function() {
        var reasonOrderCancel = $(this).closest("#reasonOrderCancel");
        var dataPost = reasonOrderCancel.data("post");
        var typeReason = $("#reasonOrderCancel #typeReason input[name=typeReason]:checked:eq(0)");
        if (typeReason.length === 0) {
            $(this).success("请选择原因");
            return;
        }
        var textReason = $.trim(typeReason.siblings(".typeText:eq(0)").text());
        var more = $.trim(reasonOrderCancel.find("#more").val());
        if (more) {
            dataPost.Remark = "取消原因（" + textReason + "：" + more + "）";

        } else {
            dataPost.Remark = "取消原因（" + textReason + "）";
        }
        stateCHangeSend(neworders, dataPost);
    });

    function　 stateCHangeSend(orderList, data) {
        $.ajax({
            url: "/Orders/UpdateState",
            type: 'post',
            contentType: "application/json; charset=utf-8;",
            data: JSON.stringify(data),
            dataType: 'json',
            beforeSend: function() {},
            success: function(data) {
                if ($("body").data("modalmanager")) {
                    var openModals = $("body").data("modalmanager").getOpenModals();
                    if (openModals) {
                        for (var i in openModals) {
                            // if ($(openModals[i]['$element'][0]).attr("id") !== "orderPre") {
                            $(openModals[i]['$element'][0]).modal("hide");
                            // }
                        }
                    }
                }

                if (data.failed.length == 0) {
                    orderList.draw(false);
                    return;
                }
                orderList.draw(false);

                var failed = data.failed;
                var str = '';
                for (var i in failed) {
                    var arr = [
                        '<div style="margin:10px 0px">',
                        '<span style="color:#0099cc">' + failed[i]['OrderNo'] + '：</span>',
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


    // 筛选条件开始
    //  状态栏
    jQuery('#testb').ButtonRadio({
        data: {},
        selected: function(dom, code) {
            var type = $(this).data('type');
            var postkey = $(this).data("postkey");
            if (type == 'select') {
                var state = $(this).val();
            } else if (type = "label") {
                var state = $(this).data('code');

            } else if (type = "state") {
                var state = $(this).data('code');
            }
            var searchObj = {

            };
            searchObj[postkey] = state;
            searchObj['searchType'] = "state";
            if (!("status" in searchObj)) {
                searchObj['status'] = '-1';
            }

            $('#searchoption').data("search", searchObj);
            neworders.draw();

        }
    });

    //模糊搜索
    // 第一项
    jQuery("#fuzzySearch").bind("click", function() {
        var fuzzyString = jQuery("#fuzzyString").val().trim();
        $('#searchoption').data("search", {
            'FuzzySearch': fuzzyString,
            "FuzzySearchType": $(".tabletools:eq(0) #FuzzySearchType").val(),
            'status': "-1",
            'searchType': 'fuzzy'
        });
        neworders.draw();
    });
    jQuery(".tabletools:eq(0) #FuzzySearchType").on("change", function() {
        var text = $(this).find("option:selected").text();
        $(this).siblings("#fuzzyString").attr("placeholder", "搜索：" + text);
        var FuzzySearchType = $(this).val();
        $.LangHuaCookie.set("orderListFuzzySearchType", FuzzySearchType, 24 * 30 * 1, "/Orders");
    });
    jQuery('#fuzzyString').bind("keydown", function(evt) {
        evt = (evt) ? evt : ((window.event) ? window.event : "") //兼容IE和Firefox获得keyBoardEvent对象  
        var key = evt.keyCode ? evt.keyCode : evt.which; //兼容IE和Firefox获得keyBoardEvent对象的键值  
        if (key == 13) {
            jQuery("#fuzzySearch").trigger("click");
        }

    })


    $('#orderList').on('click', '.searchTBID', function() {
        // $('#fuzzyString').val($(this).text())
        // jQuery("#fuzzySearch").trigger("click")

        var tbid = $(this).text();
        var url = '/Orders/Index?search=';
        var search = {
            searchType: "fuzzy",
            FuzzySearch: tbid,
            FuzzySearchType: "TBID"
        }
        window.open(url + JSON.stringify(search).urlSwitch())
    })


    //高级搜索




    jQuery("body").on('shown.bs.modal', '#advancedsearch', function() {
        jQuery('#launchdaterange').datepicker({});
        jQuery('#initdaterange').datepicker({});
        jQuery('#returndaterange').datepicker({});
        jQuery('#ordersenddaterange').datepicker({});


        $(this).find(".fordatepicker").unbind().bind("click", function() {
            $(this).prev('input').trigger("focus")
        })

        jQuery(this).find('#confirmadvancedsearch').one("click", function() {
            var obj = {};
            var textobj = {
                OrderSourseID: "OrderSourseName",
                SupplierID: "SupplierName",
                // SupplierEnableOnline:'SupplierEnableOnlineName'
            }
            jQuery(this).closest('#advancedsearch').find("label input[type=checkbox]:checked:not(.secondary):not(.unitState)").each(function() {
                console.log(this)
                var which = jQuery(this).data('for').split(',')
                if (which[0] == 'isOneself') {
                    obj.isOneself = true;
                    return;
                }
                if (which[0] == 'IsPay') {
                    obj.IsPay = true;
                    return;
                }

                if (which[0] == 'IsNeedCustomerService') {
                    obj.IsNeedCustomerService = true;
                    return;
                }
                if (which[0] == 'isUrgent') {
                    obj.isUrgent = true;
                    return;
                }
                if (which[0] == 'SupplierEnableOnline') {
                    obj.SupplierEnableOnline = jQuery(this).closest('.form-inline').find("input[type=radio]:checked").val();
                    obj.SupplierEnableOnlineName = jQuery(this).closest('.form-inline').find("input[type=radio]:checked").siblings(":eq(0)").text();
                    obj.SupplierEnableOnlineName = jQuery(this).closest('.form-inline').find("input[type=radio]:checked").siblings(":eq(0)").text();
                    return;
                }
                if (which[0] == 'status') {
                    var arrStateText = [];
                    var arrStateCode = [];
                    var stateListChecked = jQuery(this).closest('.form-inline').find("#stateMulti input[type=checkbox]:checked");
                    for (var j = 0; j < stateListChecked.length; j++) {
                        arrStateCode.push((stateListChecked).eq(j).val());
                        arrStateText.push((stateListChecked).eq(j).siblings("span:eq(0)").text());
                    }
                    obj.status = arrStateCode.join(",");
                    obj.statusNamae = arrStateText.join("，");
                    return;
                }

                for (var i in which) {
                    // jQuery(this).closest('div.checkbox').siblings("+div")

                    obj[which[i]] =
                        jQuery(this).closest('.form-inline').find("#" + which[i]).val() || "";
                    if (which[i] == "TravelDateBegin") {
                        if (jQuery(this).closest('.form-inline').find("#" + 'IsChangeTravelDate').prop("checked")) {
                            obj['IsChangeTravelDate'] = true;
                        }

                    }

                    var t = jQuery(this).closest('div.checkbox').siblings().find("#" + which[i]).val();
                    if (which[i] in textobj) {
                        obj[textobj[which[i]]] = jQuery(this).closest('.form-inline').find("#" + which[i]).find('option:selected').text();
                    }
                }
            })
            obj['searchType'] = 'advanced'
            $('#searchoption').data("search", obj);
            neworders.draw();
            $('#advancedsearch').modal('hide');

            // status
            // FuzzySearch
            // OrderSourseID 
            // SupplierID 
            // pItemName 
            // IsPay 
            // IsNeedCustomerService 
            // isOneself 
            // TravelDateBegin 
            // TravelDateEnd 
            // OrderCreateDateBegin 
            // OrderCreateDateEnd 
            // isUrgent
        })
        var _this = this;

        jQuery('#ItemName').unbind().bind("keydown", function() {
            var eventObj = event || e;
            var keyCode = eventObj.keyCode || eventObj.which;
            if (keyCode == 13) {

                jQuery(_this).find('#confirmadvancedsearch').trigger('click');
            }
        })
    })
    $('#orderList').on('click', '.searchcnItemName', function() {
        var ItemName = $(this).text();
        $('#searchoption').data("search", {
            'ItemName': ItemName,
        });
        neworders.draw();
    })



    // 搜索条件展示
    jQuery('#advancedviewer').on('update', function(e, data, data2) {
        var str = ''
        for (var i in data) {
            if (data[i]['value'].length == 2) {
                var obj = {}
                str += [
                    '<div class="one" data-search=' + data[i]['search'] + '>',
                    '<div class="tip">' + data[i]['text'] + '：</div>',
                    '<span class="pair">',
                    '<div class="top">' + data[i]["value"][0]['value'] + '</div>',
                    '<div class="bottom">' + data[i]["value"][1]['value'] + '</div>',
                    '</span>',
                    '<div class="delete">×</div>',
                    '</div>\n'
                ].join('')
            } else {
                var obj = {};

                str += [
                    '<div class="one" data-search=' + data[i]['search'] + '>',
                    '<div class="tip">' + data[i]['text'] + '：</div>',
                    '<div class="single">',
                    data[i]["value"][0]['alt'],
                    '</div>',
                    '<div class="delete">×</div>',
                    '</div>\n',
                ].join('')

            }
        }
        $(this).find(".cirgroup").empty().append(str);

        if (str.length != 0) {
            $(this).removeClass('hidden');
        } else {
            $(this).addClass('hidden');
        }

    }).on('click', '.delete', function() {

        var obj = {}


        $(this).closest(".one").remove();

        $('#advancedviewer .cirgroup:eq(0) .one').each(function() {
            var temp = $(this).data('search');

            if (!(temp instanceof Object)) {
                temp = JSON.parse(temp);
            }
            obj = $.extend(true, {}, obj, temp)
        })


        $('#searchoption').data("search", obj);
        neworders.draw();



    }).on("click", '.clear', function() {
        $(this).siblings(".cirgroup").find(".one").each(function() {
            $(this).remove();
        })


        var obj = {}



        $('#advancedviewer .cirgroup:eq(0) .one').each(function() {
            var temp = $(this).data('search');

            if (!(temp instanceof Object)) {
                temp = JSON.parse(temp);
            }
            obj = $.extend(true, {}, obj, temp)
        })


        $('#searchoption').data("search", obj);
        neworders.draw();

    })






    // 日期排序
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
                OrderBy: 0, //倒叙
                PropertyName: $(this).data('propertyname')
            }
        } else {
            var obj = {
                OrderBy: 1,
                PropertyName: $(this).data('propertyname')
            }
        }
        jQuery("#OrderBy").data('OrderBy', obj)
        neworders.draw();



    })













    //clear state
    function clear(arr) {
        var obj = {
            advancedviewer: clearadvancedviewer,
            testb: cleartestb,
            fuzzyString: clearfuzzyString
        }
        for (var i in arr) {

        }
    }





    //表格工具订单导出

    $('body')
        .on("click", "#toExportOrder", { "orderList": neworders }, function(e) {
            if (e.data.orderList.ajax.json().recordsFiltered > 5000) {
                $.LangHua.alert({
                    "tip1": "导出性能提示",
                    "indent": false,
                    "tip2": "导出数量超过5000条，请设置筛选条件后再导出"
                })
                return;
            } else {
                $($(this).data("target")).modal();

            }
        })
        .on('click', '#exportFieldSave', function(e) {
            var Fields = jQuery(this).closest("#exportField").find(".oneField:checked");
            var arr = new Array();
            Fields.each(function() {
                arr.push($(this).val())
            })
            if (arr.lengt == 0) {
                $(this).success("导出字段未选")
                return;
            }
            var varURL = "/Orders/ExportExcel?exportField=" + (arr.join("%2C")) + "&";
            var link = document.createElement("a");
            var search = $('#searchoption').data("search");
            if (search) {
                if (!(search instanceof Object)) {
                    search = JSON.parse(search)
                } else {}
            } else {
                search = {};
            }
            for (var i in search) {
                varURL += (i + '=' + search[i].toString().urlSwitch() + "&");
            }

            link.href = varURL;
            document.body.appendChild(link);
            link.click();
            delete link;

            $(this).siblings('button').trigger('click')



        })


    //备注
    jQuery("body").on('click', '#RemarkShow', function(e, data) {

            var id = jQuery(this).closest("tr").attr("id");
            var oldReark = jQuery(this).closest("tr").find("td:eq(8)").text();


            $("#Remarksearch").one('shown.bs.modal', function() {
                jQuery("#Remark").val(oldReark)

                jQuery(this).find('#saveRemark').unbind().bind("click", function() {
                    $.ajax({
                        type: 'post',
                        dataType: 'json',
                        contentType: "application/json; charset=utf-8;",
                        data: JSON.stringify({ OrderID: id, Remark: jQuery.trim(jQuery("#Remark").val()) }),
                        url: '/Orders/UpdateRemark',
                        success: function(data) {
                            if (data.ErrorCode == 200) {
                                neworders.draw();
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
        //作废
    jQuery("body").on('click', '#Invalid', function(e, data) {

        var id = jQuery(this).closest("tr").attr("id");
        var no = jQuery(this).closest("tr").find("td:eq(4) span:eq(0)").text() + '<div style="font-size:13px;">' + jQuery(this).closest("tr").find("td:eq(1) a").text() + '</div>';

        jQuery.LangHua.confirm({
            title: "提示信息",
            tip1: '请确认是否作废以下订单：',
            tip2: no,
            confirmbutton: '确定',
            cancelbutton: '取消',
            data: null,
            confirm: function() {
                $.ajax({
                    url: "/Orders/UpdateState",
                    type: 'post',
                    contentType: "application/json; charset=utf-8;",
                    data: JSON.stringify({
                        OrderID: id,
                        state: 7
                    }),
                    dataType: 'json',
                    beforeSend: function() {},
                    success: function(data) {
                        if (data.failed.length == 0) {
                            neworders.draw(false);
                            return
                        }
                        neworders.draw(false);
                        var failed = data.failed;
                        var str = '';
                        for (var i in failed) {
                            var arr = [
                                '<div style="margin:10px 0px">',
                                '<span style="color:#0099cc">' + failed[i]['OrderNo'] + '：</span>',
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
        })
    })
    $('#reflashTable').bind("click", function() {
        neworders.draw()
    });


});