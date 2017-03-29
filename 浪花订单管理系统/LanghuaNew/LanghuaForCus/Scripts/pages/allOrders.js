jQuery(document).ready(function($) {
    'use strict';
    $('#status-select').find('.buttonradio:eq(0)').addClass('active').siblings().removeClass('active');
    // 表格获取筛选条件
    $('#orders-all-cus')
        .on('preXhr.dt', function(e, settings, json) {
            // 删除插件无必要项目
            delete json.columns;
            delete json.search;
            if ($('#searchoption').length === 0) {
                json.OrderSearch = {};
                json.OrderSearch.status = -1;
                json.OrderSearch.searchType = 'state';
            } else {
                json.OrderSearch = $('#searchoption').data("search");
            }
            if (json.order.length !== 0) {
                json.OrderBy = {};
                json.OrderBy.OrderBy = (json.order[0].dir == 'asc' ? 1 : 0);
            }
        });
    // 初始化表格
    var ordersAllCu =
        $("#orders-all-cus").DataTable({
            ajax: {
                url: "/Orders/GetAllOrders",
                type: 'post'
            },
            ordering: true,
            searching: false,
            serverSide: true,
            order: [],
            drawCallback: function(settings) {
                var apix = this.api();
            },

            //行操作
            rowId: "ServiceItemID",
            createdRow: function(row, data, dataIndex) {
                var _this = this.api();
                $(row).on('click', '.orderrefused,  .orderchecked', function() {
                    $.LangHua.alert({
                        title: "联系客服",
                        tip1: '您的订单需要处理。',
                        tip2: '请联系旺旺客服帮您处理订单，谢谢！',
                        icon: "comment"
                    });
                });
            },
            //列操作
            columns: [
                //订单号
                {
                    'data': 'OrderNo',
                    'orderable': false,
                    "createdCell": function(td, cellData, rowData, row, col) {
                        var URL = 'javascript:void(0)';
                        var ClassName = "";
                        switch (rowData.CustomerState) {
                            case 0:
                                ClassName = "notFill";
                                URL = "/Orders/Edit/" + rowData.OrderID;
                                break;
                            case 10:
                                ClassName = "refused";
                                break;
                            case 20:
                                ClassName = "onCheck";
                                URL = "/Orders/Edit/" + rowData.OrderID;
                                break;
                            case 30:
                                ClassName = "checked";
                                break;
                            case 40:
                                ClassName = "booking";
                                URL = "/Orders/Details/" + rowData.EncodeOrderID.urlSwitch();
                                break;
                            case 50:
                                ClassName = "rebooking";
                                URL = "/Orders/Details/" + rowData.EncodeOrderID.urlSwitch();
                                break;

                            case 60:
                                ClassName = "canceling";
                                URL = "/Orders/Details/" + rowData.EncodeOrderID.urlSwitch();
                                break;
                            case 70:
                                ClassName = "confirm";
                                URL = "/Orders/Details/" + rowData.EncodeOrderID.urlSwitch();
                                break;
                            default:
                                ClassName = "canceled";
                                URL = "/Orders/Details/" + rowData.EncodeOrderID.urlSwitch();
                                break;
                        }
                        var userAgent = window.navigator.userAgent;
                        var osVersion = "Desktop";
                        if (userAgent.indexOf("iPhone") > -1) {
                            osVersion = "Mobbile";
                        } else if (userAgent.indexOf("NOKIA") > -1) {
                            osVersion = "Mobbile";
                        } else if (userAgent.indexOf("Android") > -1) {
                            osVersion = "Mobbile";
                        }
                        var target = '';
                        if (osVersion == "Desktop") {
                            target = "_blank";
                        }
                        jQuery(td).html('<div ><a class="order' + ClassName + '"  target="' + target + '" href="' + URL + '">' + cellData + '</a></div>');
                    }
                },
                //预定项目
                {
                    'data': 'cnItemName',
                    'orderable': false
                },
                //人数
                {
                    'data': 'AdultNum',
                    'orderable': false,
                    "createdCell": function(td, cellData, rowData, row, col) {
                        if (rowData.RoomNum * rowData.RightNum > 0) {
                            jQuery(td).html(rowData.RoomNum + '间 / ' + rowData.RightNum + '晚');
                        } else {
                            jQuery(td).html(cellData + ' / ' + rowData.ChildNum + ' / ' + rowData.INFNum);
                        }
                    }
                },
                //日期
                {
                    'data': 'TravelDate',
                    'orderSequence': ["desc", "asc"]

                },

                //状态
                {
                    'data': 'stateName',
                    'orderable': false,
                    "createdCell": function(td, cellData, rowData, row, col) {

                        var statusMap = {
                            '0': {
                                'color': "#FFFFCC",
                                'textColor': "#333333"
                            },
                            '10': {
                                'color': "#FFCCFF",
                                'textColor': "#333333"
                            },
                            '20': {
                                'color': "#FFFFCC",
                                'textColor': "#333333"
                            },
                            '30': {
                                'color': "#00FF00",
                                'textColor': "#333333"
                            },
                            '40': {
                                'color': "#66FFCC",
                                'textColor': "#333333"
                            },
                            '50': {
                                'color': "#66FFCC",
                                'textColor': "#333333"
                            },
                            '60': {
                                'color': "#66FFCC",
                                'textColor': "#333333"
                            },
                            '70': {
                                'color': "#2E9DE6",
                                'textColor': "#FFF"
                            },
                            '80': {
                                'color': "#C9C9C9",
                                'textColor': "#333333"
                            }
                        };
                        var color = statusMap[rowData.CustomerState].color;
                        var textColor = statusMap[rowData.CustomerState].textColor;
                        $(td).css('background', color);
                        $(td).css('color', textColor);

                    }
                }
            ]
        });
    $('#status-select').ButtonRadio({
        data: {},
        selected: function(dom, code) {
            var type = $(this).data('type');
            var postkey = $(this).data("postkey");
            var state = $(this).data('code');
            var searchObj = {};
            searchObj[postkey] = state;
            searchObj.searchType = "state";
            if (!("status" in searchObj)) {
                searchObj.status = '-1';
            }
            if ($('#searchoption').length === 0) {
                $('body').append('<div id ="searchoption" class="hidden"><div>');
            }
            $('#searchoption').data("search", searchObj);
            ordersAllCu.draw();
        }
    });
});