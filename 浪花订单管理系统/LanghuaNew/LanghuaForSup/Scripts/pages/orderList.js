$(document).ready(function() {
    "use strict";

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
        console.log(e);
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
    })
    $("#advancedsearch").on("change", "#stateMulti .unitState", function(e) {
        var checkList = $("#advancedsearch #stateMulti .unitState:checked");
        var arrStateText = [];
        for (var i = 0; i < checkList.length; i++) {
            arrStateText.push(checkList.eq(i).siblings("span:eq(0)").text());
        }
        $("#advancedsearch").find("#status").val(arrStateText.join(",")).attr("title", arrStateText.join(","))
    });
    fixed();
    $('#orderList').eq(0)
        .on('preXhr.dt', function(e, settings, json) {
            $('#reflashTable').find('i').addClass("fa-spin");

            console.log($('#searchoption'))
            var search = ($('#searchoption').data("search"));
            if (search) {
                if (!(search instanceof Object)) {
                    search = JSON.parse(search);
                }
            }
            // 删除插件无必要项目
            delete json.columns;
            delete json.order;
            delete json.search;
            json['OrderBy'] = jQuery("#OrderBy").data("OrderBy")
            json['OrderSearch'] = search;

        }).on('xhr.dt', function(e, settings, json, xhr) {
            $('#reflashTable').find('i').removeClass("fa-spin");
            $('#orderList thead tr th:eq(0) input').prop("checked", false)

            var searchobj = json.SearchModel.OrderSearch || null;
            if (searchobj == null || searchobj['searchType'] == "state" || searchobj['searchType'] == "fuzzy") {
                $('#advancedviewer').trigger('update', [
                    [], searchobj
                ]);
                if (searchobj == null) {
                    $('#allstate a:first').addClass("active").siblings().removeClass("active");
                    return
                } else if (searchobj['searchType'] == "state") {
                    $('#fuzzyString').val("");
                } else if (searchobj['searchType'] == "fuzzy") {
                    $('#allstate a:first').addClass("active").siblings().removeClass("active");
                    $('#fuzzyString').val(searchobj['FuzzySearch']);
                }
                return

            }


            //或是高级搜索



            var temp = {
                status: {
                    text: "状态:",
                },
                OrderSourseID: {
                    text: "来源:"
                },
                isUrgent: {
                    text: '紧急订单'
                },
                ServiceTypeID: {
                    text: '订单类型:',
                },
                SupplierID: {
                    text: "供应商："
                },
                ItemName: {
                    text: '预订项目：'
                },

                isOneself: {
                    text: '本人订单',
                },
                IsChangeTravelDate: {
                    text: '只查变更中的的出行日期',
                },
                TravelDateBegin: {
                    text: "出行："
                },
                ReturnDateBegin: {
                    text: "返回："
                },
                OrderOperDateBegin: {
                    text: "操作："
                },
                OrderCreateDateBegin: {
                    text: "创建："
                },
                OrderSendDateBegin: {
                    text: "下单："
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
                } else if (i == "OrderOperDateBegin") {
                    var arr = [];
                    arr.push({
                        props: 'OrderOperDateBegin',
                        value: searchobj['OrderOperDateBegin'].substr(0, 10),
                        alt: searchobj['OrderOperDateBegin'].substr(0, 10)
                    });
                    arr.push({
                        props: 'OrderOperDateEnd',
                        value: searchobj['OrderOperDateEnd'].substr(0, 10),
                        alt: searchobj['OrderOperDateEnd'].substr(0, 10)
                    });

                    obj.search = JSON.stringify({
                        OrderOperDateBegin: searchobj.OrderOperDateBegin.substr(0, 10),
                        OrderOperDateEnd: searchobj['OrderOperDateEnd'].substr(0, 10),
                    });
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
                    }

                    if (i == "ServiceTypeID") {
                        arr.push({
                            props: i,
                            value: searchobj[i],
                            alt: searchobj['ServiceTypeName']
                        });


                        obj.search = JSON.stringify({
                            ServiceTypeID: searchobj[i],
                            ServiceTypeName: searchobj['ServiceTypeName']
                        })
                    }
                    if (i == "isUrgent") {
                        arr.push({
                            props: i,
                            value: searchobj[i],
                            alt: ""
                        });


                        obj.search = JSON.stringify({
                            isUrgent: searchobj[i],

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


    var orderList =
        jQuery('table#orderList').DataTable({
            ajax: {
                url: "/Orders/GetOrders",
                type: 'post',
            },
            ordering: false,
            searching: false,
            serverSide: true,

            drawCallback: function(settings) {
                var api = this.api();
            },
            initComplete: function(settings, json) {
                var api = this.api();
                $(this).on('click', '.showinfo', function() {
                    var _this = this;
                    $.ajax({
                        url: "/Orders/GetCustomerInfo/" + $(_this).closest("td").attr("uid"),
                        type: 'get',
                        contentType: "application/json; charset=utf-8;",

                        dataType: 'json',
                        beforeSend: function() {},
                        success: function(data) {
                            var usrdata = data.data;


                            var t = [
                                '<div  class="modal modal-animate" tabindex="-1" data-backdrop="static" data-width="500" data-max-height=400>',
                                '<div class="modal-dialog " role="document">',
                                '<div class="modal-content">',
                                '<div class="modal-header">',
                                '<button type="button" class="close" data-dismiss="modal" aria-hidden="true">×</button>',
                                '<h4 class="modal-title">客户资料-' + usrdata.CustomerName + '</h4>',
                                '</div>',
                                '<div class="modal-body">',
                                '<form class="form-horizontal" role="form">',
                                '<disv class="form-body" style="padding-top:0px;padding-bottom:0px">',
                                '<div class="form-group">',
                                '<label class="col-md-3 control-label">淘宝ID：</label>',
                                '<div class="col-md-9">',
                                '<input id="CustomerName" disabled type="text" class="form-control input-inline input-medium" value="' + usrdata.CustomerTBCode + '" placeholder="中文姓名" />', ,
                                '</div>',
                                '</div>',

                                '<div class="form-group">',
                                '<label class="col-md-3 control-label">中文姓名：</label>',
                                '<div class="col-md-9">',
                                '<input id="CustomerName" disabled type="text" class="form-control input-inline input-medium" value="' + usrdata.CustomerName + '" placeholder="中文姓名" />',
                                '<span class="help-inline">  </span>',
                                '</div>',
                                '</div>',
                                '<div class="form-group">',
                                '<label class="col-md-3 control-label">姓名拼音：</label>',
                                '<div class="col-md-9">',
                                '<input id="CustomerEnname" disabled type="text" class="form-control input-inline input-medium" value="' + usrdata.CustomerEnname + '" placeholder="姓名（拼音）" />',
                                '</div>',
                                '</div>',
                                '<div class="form-group">',
                                '<label class="col-md-3 control-label">联系电话：</label>',
                                '<div class="col-md-9">',
                                '<input id="Tel" type="text" disabled class="form-control input-inline input-medium"  value="' + usrdata.Tel + '"  placeholder="" />',
                                '</div>',
                                '</div>',
                                '<div class="form-group">',
                                '<label class="col-md-3 control-label">备用联系电话：</label>',
                                '<div class="col-md-9">',
                                '<input id="BakTel" type="text" disabled class="form-control input-inline input-medium" value="' + (usrdata.BakTel ? usrdata.BakTel : "") + '" placeholder="" />',
                                '</div>',
                                '</div>',
                                '<div class="form-group">',
                                '<label class="col-md-3 control-label"> Email地址：</label>',
                                '<div class="col-md-9">',
                                '<input id="Email" type="text" disabled class="form-control input-inline input-medium" value="' + usrdata.Email + '" placeholder="" />',
                                '</div>',
                                '</div>',
                                '<div class="form-group">',
                                '<label class="col-md-3 control-label"> 微信号：</label>',
                                '<div class="col-md-9">',
                                '<input id="Wechat" type="text" disabled class="form-control input-inline input-medium" value="' + usrdata.Wechat + '"  placeholder="" />',
                                '</div>',
                                '</div>',
                                '</disv>',
                                '</form>',
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
                })
            },
            //行操作
            rowId: "OrderID",
            createdRow: function(row, data, dataIndex) {
                var _this = this.api();
                var thisTable = this;
            },
            //列操作
            columns: [

                //左格选择
                {
                    'data': 'AdultNum',
                    "createdCell": function(td, cellData, rowData, row, col) {
                        jQuery(td).html('<input type="checkbox" class="checkboxes">');
                    }
                },

                //订单号
                {
                    'data': 'OrderNo',
                    "createdCell": function(td, cellData, rowData, row, col) {
                        jQuery(td).html('<div  class="bright"><a  target="_blank" href="/Orders/Details/' + rowData.OrderID + '">' + cellData + '</a></div>');

                    }
                },


                // 姓名
                {
                    'data': 'CustomerName',
                    "createdCell": function(td, cellData, rowData, row, col) {
                        jQuery(td).html('<div><a class="showinfo">' + cellData + '</a></div><div class="mini">' + rowData.CustomerEnname + '</div>');
                        jQuery(td).attr("uid", rowData.CustomerID)
                    }
                },

                //预定项目
                {
                    'data': 'cnItemName',
                    "createdCell": function(td, cellData, rowData, row, col) {
                        jQuery(td).html('<span>' + cellData + '</span><span class="bright"><a href="javascript:;" class="searchcnItemName">  ' + rowData.ServiceCode + '</a></span>');
                    }
                },

                //人数
                {
                    'data': 'AdultNum',
                    "createdCell": function(td, cellData, rowData, row, col) {
                        if (rowData.RoomNum * rowData.NightNum > 0) {
                            jQuery(td).html(rowData.RoomNum + '间 / ' + rowData.NightNum + '晚');
                        } else {
                            jQuery(td).html(cellData + ' / ' + rowData.ChildNum + ' / ' + rowData.INFNum);
                        }
                    }
                },

                //日期
                {
                    'data': 'TravelDate',
                    "createdCell": function(td, cellData, rowData, row, col) {
                        var saletag = '';
                        if (rowData.isUrgent == true) {
                            saletag = '<div class="isUrgent"><span class="spanlabel font11 urgent-service-color">紧急</span></div>';
                        }
                        jQuery(td).html(' <div>' + cellData + '</div>' + saletag);

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
                    "createdCell": function(td, cellData, rowData, row, col) {

                        var tmp = {
                            "3": "#0000ff",
                            "4": "#ff0000",
                            "14": "#ff0000",
                            "15": "#ff0000",
                            "9": "#cc0000",
                            "12": "#cc0000",
                            "5": "#000000",
                            "6": "#000000",
                            "10": "#666666",
                            "11": "#666666",
                            "7": "#ff6600",
                            "8": "#ff6600"
                        }



                        var style = 'style="color:' + tmp[cellData] + '"';
                        var text = rowData.stateName;
                        if (rowData.IsNeedCustomerService == true) {
                            var saletag = '<div class="IsNeedCustomerService"><span class="spanlabel font11 after-sale-service-color">要售后</span></div>';
                        } else
                            var saletag = '';

                        if (rowData.IsPay == true) {
                            var uppaidtag = '<div class="IsPay"><span class="spanlabel font11 need-pay-color">未付完</span></div>';
                        } else
                            var uppaidtag = '';
                        jQuery(td).html(
                            '<div ' + style + '>' + text + '</div>' +
                            saletag + uppaidtag
                        );
                    }
                },

                //团号
                {
                    'data': 'GroupNo'
                },

                //备注
                {
                    'data': 'Remark',
                    "createdCell": function(td, cellData, rowData, row, col) {
                        // jQuery(td).html('');
                    }
                },

                //操作
                {
                    'data': 'OrderID',
                    "createdCell": function(td, cellData, rowData, row, col) {
                        //var a= $('<a></a>');
                        //a.text('日志').attr({
                        //    'href':"/Orders/OrderOperation/"+cellData,
                        //    'target':"_blank"
                        //})
                        //$(td).html(a);
                        var str =
                            '<div class="row">' +
                            '<a id="RemarkShow" href="#Remarksearchx" class="hrefInTable Remarksearch" data-toggle="modal">留言</a>' +
                            '<a class="hrefInTable-inline" target="_blank" href="/Orders/OrderOperation/' + cellData + '">日志</a>' +
                            '</div>';
                        jQuery(td).html(str);
                    }
                }



            ]
        });



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
        var currentPageLength = orderList.page.info().end - orderList.page.info().start;

        if ($(this).closest("tbody").find("tr td input:checked").length != currentPageLength) {
            $(_this).closest("table.ddtable").find(".group-checkable:first").prop("checked", false);
        } else {
            $(_this).closest("table.ddtable").find(".group-checkable:first").prop("checked", true);
        }
        var length = $('table.ddtable:first').find('tbody tr td input.checkboxes:checked').length
        $('#selectedNumber').text(length);

    })


    //状态筛选
    jQuery('#allstate').ButtonRadio({
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
            var searchObj = {};
            searchObj[postkey] = state;
            searchObj['searchType'] = "state";
            if (!("status" in searchObj)) {
                searchObj['status'] = '-1';
            }
            $('#searchoption').data("search", searchObj);
            orderList.draw();

        }
    });
    //模糊搜索 开始
    jQuery("#fuzzySearch").bind("click", function() {
        var fuzzyString = jQuery("#fuzzyString").val().trim();
        $('#searchoption').data("search", {
            'FuzzySearch': fuzzyString,
            'status': "-1",
            'searchType': 'fuzzy'
        });
        orderList.draw();
    })
    jQuery('#fuzzyString').bind("keydown", function(evt) {
            evt = (evt) ? evt : ((window.event) ? window.event : "") //兼容IE和Firefox获得keyBoardEvent对象  
            var key = evt.keyCode ? evt.keyCode : evt.which; //兼容IE和Firefox获得keyBoardEvent对象的键值  
            if (key == 13) {
                jQuery("#fuzzySearch").trigger("click");
            }

        })
        //模糊搜索 结束


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
        };
        var theAjax =
            $.ajax({
                url: "/Orders/UpdateState",
                type: 'post',
                contentType: "application/json; charset=utf-8;",
                data: JSON.stringify({
                    OrderID: OrderID,
                    state: state
                }),
                dataType: 'json',
                beforeSend: function() {
                    $.LangHua.loadingToast({
                        tip: "正在提交请求. . . . . ."
                    })
                },
                success: function(data) {
                    var openModals = $("body").data("modalmanager").getOpenModals();
                    if (openModals) {
                        for (var i in openModals) {
                            console.log(i)
                            $(openModals[i]['$element'][0]).modal("hide");
                        }
                    }
                    if (data.failed.length == 0) {
                        orderList.draw(false);

                        return
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
                        '<div  class="modal modal-animate" tabindex="-1" data-backdrop="static" data-width="500" data-max-height=200>',
                        '<div class="modal-dialog " role="document">',
                        '<div class="modal-content">',
                        '<div class="modal-header">',
                        '<button type="button" class="close" data-dismiss="modal" aria-hidden="true">×</button>',
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
                },
                error: function(jqXHR, textStatus, errorThrown) {
                    var modalManager = $("body").data("modalmanager");
                    if (modalManager) {
                        for (var i in modalManager.stack) {
                            $(modalManager.stack[i]).modal("hide");
                        }
                    }
                    if (errorThrown === "abort") {
                        return
                    }
                    $.LangHua.alert({
                        title: "提示信息",
                        tip1: '请求失败',
                        tip2: '请求失败，请您确认网络访问没有问题',
                        button: '确定',
                        icon: "warning",
                    });
                },
            });
        setTimeout(function() {
            if (theAjax.readyState <= 1) {
                if (theAjax.statusText === "error") {
                    return;
                }
                $.LangHua.confirm({
                    title: "提示信息",
                    tip1: '网络不给力',
                    tip2: '亲，似乎现在网络不给力，请尝试香港服务器 <a target="_blank" href="http://partner.dodotour.com.cn">http://partner.dodotour.com.cn</a>',
                    confirmbutton: '取消网络请求',
                    cancelbutton: '继续',
                    data: null,
                    confirm: function() {
                        theAjax.abort();
                        window.location.reload();
                    }
                });
            }
        }, 15000);
    })


    //排序
    $('body').on('click', '.OrderBy', function() {
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
        orderList.draw();



    })



    //高级搜索条件显示
    jQuery('#advancedviewer').on('update', function(e, data, data2) {
        console.log(data)
        var str = ''
        for (var i in data) {
            if (data[i]['value'].length == 2) {
                var obj = {}
                str += [
                    '<div class="one" data-search=' + data[i]['search'] + '>',
                    '<div class="tip">' + data[i]['text'] + '</div>',
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
                    '<div class="tip">' + data[i]['text'] + '</div>',
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
        orderList.draw();
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
        orderList.draw();
    })



    //高级搜索
    jQuery("body").on('shown.bs.modal', '#advancedsearch', function() {
        jQuery('#launchdaterange, #initdaterange, #returndaterange, #ordersenddaterange').datepicker({});

        jQuery(this).find('#confirmadvancedsearch').one("click", function() {
            var obj = {};
            var textobj = { //对应下拉框
                '#OrderSourseID': "OrderSourseName",
                '#status': 'statusNamae',
                '#SupplierID': "SupplierName",
                '#ServiceTypeID': 'ServiceTypeName'
            }
            jQuery(this).closest('#advancedsearch').find("label input[type=checkbox]:checked:not(.secondary):not(.unitState)").each(function() {
                var which = jQuery(this).data('for').split(',')

                if (which[0] == 'SupplierEnableOnline') {
                    obj.SupplierEnableOnline = $(this).closest('.checkbox').siblings('.form-group').find("input[type=radio]:checked").val();
                    obj.SupplierEnableOnlineName = $(this).closest('.checkbox').siblings('.form-group').find("input[type=radio]:checked").siblings(":eq(0)").text();
                    return;
                }
                if (which[0] == '#status') {
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
                    if (jQuery(this).closest('.form-inline').find(which[i] + ":eq(0)").prop('type') == 'checkbox') {
                        obj[which[i].replace(/[#.]/, '')] = true;
                    } else if (jQuery(this).closest('.form-inline').find(which[i] + ":eq(0)").prop('type') == 'text' ||
                        jQuery(this).closest('.form-inline').find(which[i] + ":eq(0)").prop('type') == 'select-one'
                    ) {
                        obj[which[i].replace(/[#.]/, '')] = jQuery(this).closest('.form-inline').find(which[i] + ":eq(0)").val() || "";
                    } else {
                        obj[which[i].replace(/[#.]/, '')] = "";
                    }
                    jQuery(this).closest('.form-inline').find(".secondary").each(function() {
                        if (jQuery(this).prop("checked")) {
                            obj[jQuery(this).data('for').replace(/[#.]/, '')] = jQuery(this).prop("checked");
                        }

                    })






                    //添加额外中文说明
                    if (which[i] in textobj) {
                        if (jQuery(this).closest('.form-inline').find(which[i] + ":eq(0)").prop('type') == 'select-one') {
                            obj[textobj[which[i]].replace(/[#.]/, '')] = jQuery(this).closest('.form-inline').find(which[i] + ":eq(0)").find('option:selected').text();
                        } else {
                            obj[textobj[which[i]].replace(/[#.]/, '')] = '';
                        }

                    }
                }
            })
            console.log(obj)
            obj['searchType'] = 'advanced'
            $('#searchoption').data("search", obj);
            orderList.draw();
            $('#advancedsearch').modal('hide');

            // status
            // ServiceTypeID
            // isUrgent
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
        orderList.draw();
    })


    function fixed() {
        if ($('#searchoption').length == 0) {
            var div = $("<div></div>");
            div.attr('id', 'searchoption').addClass("hidden");
            $('body').append(div);
        }

        if ($('#OrderBy').length == 0) {
            var div = $("<div></div>");
            div.attr('id', 'OrderBy').addClass("hidden");
            $('body').append(div);
        }
    }

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
                            orderList.draw();
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



    //高级搜索导出订单

    $('body')
        .on("click", "#toExportOrder", { "orderList": orderList }, function(e) {
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

            }

            for (var i in search) {
                varURL += i + '=' + search[i] + "&";
            }

            link.href = varURL;
            document.body.appendChild(link);
            link.click();
            link = null;

            $(this).siblings('button').trigger('click')



        })

    $('#reflashTable').bind("click", function() {
        orderList.draw()
    });
});