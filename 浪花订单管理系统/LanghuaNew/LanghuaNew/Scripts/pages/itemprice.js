jQuery(document).ready(function($) {
    // Prices = initPrices();
    // ExtraPrices = initExtraPrices();
    $.ajax({
        url: "/ServiceItems/GetPrice",
        type: 'get',
        contentType: "application/json; charset=utf-8;",
        data: {
            'ItemID': $('#ItemID').val(),
            'SupplierID': $('#supplier').val()
        },
        dataType: 'json',
        success: function(data) {
            var priceTable = initPrices(data.data);
            var extraPriceTable = initExtraPrices(data.data);
            priceCreate(priceTable);
            payType(priceTable);
            priceSave([priceTable, extraPriceTable]);
        }
    });
    jQuery("#supplier").bind('change', function() {
        var ItemID = jQuery("#ItemID").val()
        var supplierID = jQuery(this).val()
        window.location.href = '/ServiceItems/PriceSetting?ItemID=' + ItemID + '&SupplierID=' + supplierID;
    });
    $('body').on('change', '.price-format', function() {
        var editValue = $(this).val();
        editValueTemp = $.trim(editValue);
        var afterReplace = editValueTemp.replace(/[^0-9\.]/g, "");
        if (afterReplace.length !== 0) {
            var numberTest = /^[0-9]+(\.[0-9]*)?$/;
            if (numberTest.test(afterReplace)) {
                var tmp = afterReplace.match('.');
                if (tmp === null) {
                    $(this).val(afterReplace);
                } else {
                    var tmpARR = afterReplace.split('.');
                    if (tmpARR.length === 1) {
                        $(this).val(tmpARR[0]);
                    } else {
                        if (tmpARR[1].toString().length > 2) {
                            $(this).val(Number(afterReplace).toFixed(2));
                        } else {
                            if (tmpARR[1].toString().length === 0) {
                                $(this).val(tmpARR[0]);
                            } else {
                                $(this).val(afterReplace);
                            }
                        }
                    }
                }
            } else {
                $(this).val("");
            }
        } else {
            $(this).val("");
        }
    });
});

function priceCreate(theTable) {
    $('#priceCreate').datepicker({
        inputs: $('#priceCreate .datepickersx')
    });
    $('#priceCreate').on('change', '#startTime', function() {
        $('#priceCreate #EndTime').trigger("click");
    });
    $('#priceCreate').on('change', '#EndTime', function() {
        $('#priceCreate #startTime').trigger("click");

    });
    $('#priceCreate #toCreate').bind('click', { "theTable": theTable }, function(e) {
        if ($('.PayType[name=PayType]:checked').length === 0) {
            $('#PayTypeList .container-warn:eq(0)').Warning({
                'title': "您需要先选择计费标准"
            });
            return;
        }

        var cancel = false;
        var i, timeStartTemp, timeEndTemp, timeStartTempArr, timeEndTempArr, timeStartEdit, timeEndEdit;
        var payType = parseInt(jQuery(".PayType[name=PayType]:checked").val());
        var fix = "";
        var theTable = e.data.theTable.dataTable;
        var trDom = $(this).closest("tr")[0];
        var allRowDatas = theTable.rows().data();
        var rowData = theTable.row(trDom).data();

        if (payType === 1) {
            if (!($(this).closest("tr").find("#Price").val())) {
                $(this).closest("tr").find("#Price").Warning({
                    title: "请填写价格",
                    placement: "top",
                    attach: []
                });
                cancel = true;
            }
        } else {
            if (!($(this).closest("tr").find("#startTime").val())) {
                $(this).closest("tr").find("#startTime").Warning({
                    title: "请您填写开始时间",
                    placement: "top",
                    attach: []
                });
                cancel = true;
            }
            if (!($(this).closest("tr").find("#EndTime").val())) {
                $(this).closest("tr").find("#EndTime").Warning({
                    title: "请您填写结束时间",
                });
                cancel = true;
            }
            if (!($(this).closest("tr").find("#AdultNetPrice").val())) {
                $(this).closest("tr").find("#AdultNetPrice").Warning({
                    title: "请填写成人价格",
                });
                cancel = true;
            }
            if (!($(this).closest("tr").find("#ChildNetPrice").val())) {
                $(this).closest("tr").find("#ChildNetPrice").Warning({
                    title: "请填写儿童价格",
                });
                cancel = true;
            }
            if (!($(this).closest("tr").find("#BobyNetPrice").val())) {
                $(this).closest("tr").find("#BobyNetPrice").Warning({
                    title: "请填写婴儿价格",
                });
                cancel = true;
            }
        }
        if (cancel === true) {
            return;
        }
        timeStartTempArr = $(this).closest("tr").find("#startTime").val().split("-");
        timeEndTempArr = $(this).closest("tr").find("#EndTime").val().split("-");
        timeStartEdit = new Date(timeStartTempArr[0], parseInt(timeStartTempArr[1]) - 1, timeStartTempArr[2], 0, 0, 0).valueOf();
        timeEndEdit = new Date(timeEndTempArr[0], parseInt(timeEndTempArr[1]) - 1, timeEndTempArr[2], 0, 0, 0).valueOf();
        for (i = 0; i < allRowDatas.length; i++) {
            if (allRowDatas[i].state === 'edit') {
                continue;
            }
            if (allRowDatas[i].isChanging === true || allRowDatas[i].isChanging === 'new') {
                fix = 'Change';
            } else {
                fix = "";
            }
            timeStartTempArr = allRowDatas[i]['startTime' + fix].split("T")[0].split("-");
            timeEndTempArr = allRowDatas[i]['EndTime' + fix].split("T")[0].split("-");
            timeStartTemp = new Date(timeStartTempArr[0], parseInt(timeStartTempArr[1]) - 1, timeStartTempArr[2], 0, 0, 0).valueOf();
            timeEndTemp = new Date(timeEndTempArr[0], parseInt(timeEndTempArr[1]) - 1, timeEndTempArr[2], 0, 0, 0).valueOf();
            if (
                ((timeStartEdit >= timeStartTemp) && (timeStartEdit <= timeEndTemp))) {

                $(this).closest("tr").find("#startTime").Warning({
                    title: "与序号 " + (parseInt(i) + 1) + " 时间段重叠"
                });
                cancel = true;
            }
            if (

                ((timeEndEdit >= timeStartTemp) && (timeEndEdit <= timeEndTemp))
            ) {
                $(this).closest("tr").find("#EndTime").Warning({
                    title: "与序号 " + (parseInt(i) + 1) + " 时间段重叠"
                });
                cancel = true;
            }
            if (
                (timeStartEdit < timeStartTemp) && (timeEndTemp < timeEndEdit)
            ) {
                $(this).closest("tr").find("#startTime").Warning({
                    title: "与序号 " + (parseInt(i) + 1) + " 时间段重叠"
                });
                $(this).closest("tr").find("#EndTime").Warning({
                    title: "与序号 " + (parseInt(i) + 1) + " 时间段重叠"
                });
                cancel = true;
            }
        }
        if (cancel === true) {
            return;
        }
        //判断结束
        var priceTemp = {};
        var priceNew;
        $(this).closest("#priceCreate").find(".price-creating").each(function() {
            priceTemp[$(this).attr('id') + "Change"] = $(this).val() ? $(this).val() : "";
            $(this).val("");
        });
        priceTemp.isChanging = 'new';
        priceTemp.deleteAble = true;
        priceNew = new Price(priceTemp);
        theTable.row.add(priceNew);

        var arrAllData = [];
        for (i = 0; i < theTable.data().length; i++) {
            arrAllData.push(theTable.data()[i])
        }
        theTable.rows().remove();
        theTable.rows.add(sortByDate(arrAllData));
        theTable.draw();

    });
    $('#priceCreate #toCancel').bind('click', function() {
        $('#priceCreate').datepicker("clearDates");
        $(this).closest("#priceCreate").find(".price-creating").each(function() {
            $(this).val("");
        });
    })

}

function payType(theTable) {
    $("input:radio[name=PayType]").change(function() {
        var i, rowDataTemp, fix;
        var value = $("input:radio[name=PayType]:checked").val();
        var rows = theTable.dataTable.data();
        if (value == 0) {
            theTable.dataTable.columns([6]).visible(false);
            jQuery("#priceSettings tbody:eq(1) tr td:eq(6)").hide();
            theTable.dataTable.columns([3, 4, 5]).visible(true);
            jQuery("#priceSettings tbody:eq(1) tr td:eq(3)").show();
            jQuery("#priceSettings tbody:eq(1) tr td:eq(4)").show();
            jQuery("#priceSettings tbody:eq(1) tr td:eq(5)").show();
            for (i = 0; i < rows.length; i++) {
                var rowDataTemp = $.extend(true, {}, theTable.dataTable.row(i).data());
                // if (rowDataTemp.isChanging === false) {
                //     fix = "";
                // } else {
                //     fix = "Change";
                // }
                // rowDataTemp['Price' + fix] = "";
                rowDataTemp.state = "edit";
                theTable.dataTable.row(i).data(rowDataTemp);
            }

        } else {
            theTable.dataTable.columns([6]).visible(true);
            jQuery("#priceSettings tbody:eq(1) tr td:eq(6)").show();
            theTable.dataTable.columns([3, 4, 5]).visible(false);
            jQuery("#priceSettings tbody:eq(1) tr td:eq(3)").hide();
            jQuery("#priceSettings tbody:eq(1) tr td:eq(4)").hide();
            jQuery("#priceSettings tbody:eq(1) tr td:eq(5)").hide();
            for (i = 0; i < rows.length; i++) {
                rowDataTemp = $.extend(true, {}, theTable.dataTable.row(i).data());
                // if (rowDataTemp.isChanging === false) {
                //     fix = "";
                // } else {
                //     fix = "Change";
                // }
                // rowDataTemp['AdultNetPrice' + fix] = "";
                // rowDataTemp['ChildNetPrice' + fix] = "";
                // rowDataTemp['BobyNetPrice' + fix] = "";
                rowDataTemp.state = "edit";
                theTable.dataTable.row(i).data(rowDataTemp);
            }
        }
        theTable.dataTable.draw();

    });
    // $("input:radio[name=PayType]:checked").trigger('change');

}

function Price(data) {
    this.state = data.state ? data.state : "read";
    this.isChanging = data.isChanging ? data.isChanging : false;
    this.deleteAble = data.deleteAble ? data.deleteAble : false;

    this.ItemPriceBySupplierID = data.ItemPriceBySupplierID ? data.ItemPriceBySupplierID : null;

    this.AdultNetPrice = data.AdultNetPrice;
    this.BobyNetPrice = data.BobyNetPrice;
    this.ChildNetPrice = data.ChildNetPrice;
    this.Price = data.Price;
    this.EndTime = data.EndTime;
    this.startTime = data.startTime;

    this.AdultNetPriceChange = (data.AdultNetPriceChange !== undefined && data.AdultNetPriceChange !== null) ? data.AdultNetPriceChange : "";
    this.BobyNetPriceChange = (data.BobyNetPriceChange !== undefined && data.BobyNetPriceChange !== null) ? data.BobyNetPriceChange : "";
    this.ChildNetPriceChange = (data.ChildNetPriceChange !== undefined && data.ChildNetPriceChange !== null) ? data.ChildNetPriceChange : "";
    this.PriceChange = (data.PriceChange !== undefined && data.PriceChange !== null) ? data.PriceChange : "";
    this.EndTimeChange = data.EndTimeChange ? data.EndTimeChange : "";
    this.startTimeChange = data.startTimeChange ? data.startTimeChange : "";
    this.PayType = data.PayType == 0 || data.PayType == 1 ? parseInt(data.PayType) : "";
    this.PayTypeChange = data.PayTypeChange == 0 || data.PayTypeChange == 1 ? parseInt(data.PayTypeChange) : "";
}

function ExtraPrice(data) {
    this.state = data.state;
    this.type = data.type ? data.type : "notChanging"; //new，changing

    this.ExtraServiceID = data.ExtraServiceID;
    this.ServiceName = data.ServiceName;
    this.ExtraServicePriceID = data.ExtraServicePriceID ? data.ExtraServicePriceID : null;


    this.ServicePrice = (data.ServicePrice !== undefined && data.ServicePrice !== null) ? data.ServicePrice : "";

    this.ServicePriceChange = (data.ServicePriceChange !== undefined && data.ServicePriceChange !== null) ? data.ServicePriceChange : "";

}

function initPrices(data) {
    var price, i;
    var priceList = [];
    var items = data.item;
    var changeitems = data.changeitem;
    if (items !== null) {
        if (items.IsChange === true && changeitems === null) {
            items = data.changeitem;
            changeitems = data.item;
        }
    }

    var itemsObjById = {};
    var changeitemsObjById = {};
    if (items !== null) {
        for (i in items.ItemPriceBySuppliers) {
            itemsObjById[items.ItemPriceBySuppliers[i].ItemPriceBySupplierID] = items.ItemPriceBySuppliers[i];
            itemsObjById[items.ItemPriceBySuppliers[i].ItemPriceBySupplierID].PayType = items.PayType;

        }
    }
    if (changeitems !== null) {
        for (i in changeitems.ItemPriceBySuppliers) {
            if (changeitems.ItemPriceBySuppliers[i].ItemPriceBySupplierID == 0) { //坑s
                changeitemsObjById['new' + changeitems.ItemPriceBySuppliers[i].ItemPriceBySupplierChangeID] = changeitems.ItemPriceBySuppliers[i];
                changeitemsObjById['new' + changeitems.ItemPriceBySuppliers[i].ItemPriceBySupplierChangeID].PayType = changeitems.PayType;

            } else {
                changeitemsObjById[changeitems.ItemPriceBySuppliers[i].ItemPriceBySupplierID] = changeitems.ItemPriceBySuppliers[i];
                changeitemsObjById[changeitems.ItemPriceBySuppliers[i].ItemPriceBySupplierID].PayType = changeitems.PayType;

            }
        }
    }
    for (i in changeitemsObjById) {
        if (!(i in itemsObjById)) { //新增
            price = new Price({
                isChanging: 'new',
                state: "read",
                ItemPriceBySupplierID: (function() { //对应第一次
                    if (changeitemsObjById[i].ItemPriceBySupplierChangeID === undefined) {
                        return changeitemsObjById[i].ItemPriceBySupplierID;
                    } else {
                        return 'new' + changeitemsObjById[i].ItemPriceBySupplierChangeID;
                    }
                })(),
                // ItemPriceBySupplierID: 'new' + changeitemsObjById[i].ItemPriceBySupplierChangeID,
                AdultNetPrice: null,
                BobyNetPrice: null,
                ChildNetPrice: null,
                Price: null,
                startTime: null,
                EndTime: null,

                AdultNetPriceChange: changeitemsObjById[i].AdultNetPrice,
                BobyNetPriceChange: changeitemsObjById[i].BobyNetPrice,
                ChildNetPriceChange: changeitemsObjById[i].ChildNetPrice,
                PriceChange: changeitemsObjById[i].Price,
                EndTimeChange: changeitemsObjById[i].EndTime,
                startTimeChange: changeitemsObjById[i].startTime,
                PayTypeChange: changeitemsObjById[i].PayType

            });
            priceList.push(price);
        }
    }
    for (i in itemsObjById) {
        if (i in changeitemsObjById) { //去在且在变更中
            price = new Price({
                isChanging: true,
                state: "read",
                ItemPriceBySupplierID: itemsObjById[i].ItemPriceBySupplierID,

                AdultNetPrice: itemsObjById[i].AdultNetPrice,
                BobyNetPrice: itemsObjById[i].BobyNetPrice,
                ChildNetPrice: itemsObjById[i].ChildNetPrice,
                Price: itemsObjById[i].Price,
                startTime: itemsObjById[i].startTime,
                EndTime: itemsObjById[i].EndTime,
                PayType: itemsObjById[i].PayType,


                AdultNetPriceChange: (function() {
                    return changeitemsObjById[i].AdultNetPrice;
                })(),

                BobyNetPriceChange: (function() {
                    return changeitemsObjById[i].BobyNetPrice;
                })(),
                ChildNetPriceChange: (function() {
                    return changeitemsObjById[i].ChildNetPrice;
                })(),
                PriceChange: (function() {
                    return changeitemsObjById[i].Price;
                })(),
                EndTimeChange: (function() {
                    return changeitemsObjById[i].EndTime;
                })(),
                startTimeChange: (function() {
                    return changeitemsObjById[i].startTime;
                })(),
                PayTypeChange: changeitemsObjById[i].PayType

            });
            priceList.push(price);

        } else { //存在且未变更
            price = new Price({
                isChanging: false,
                state: "read",
                ItemPriceBySupplierID: itemsObjById[i].ItemPriceBySupplierID,

                AdultNetPrice: itemsObjById[i].AdultNetPrice,
                BobyNetPrice: itemsObjById[i].BobyNetPrice,
                ChildNetPrice: itemsObjById[i].ChildNetPrice,
                Price: itemsObjById[i].Price,
                startTime: itemsObjById[i].startTime,
                EndTime: itemsObjById[i].EndTime,
                PayType: itemsObjById[i].PayType
            });
            priceList.push(price);
        }
    }

    console.log(priceList)
    priceList = sortByDate(priceList);
    console.log(priceList)





    var tablePrice =
        jQuery("#priceSettings").DataTable({
            dom: "t",
            ordering: false,
            serverSide: false,
            language: {
                zeroRecords: "",
                emptyTable: '没有相应的价格设置'
            },
            data: priceList,
            'rowCallback': function(row, rowData, dataIndex) {
                if (rowData.state === 'edit') {
                    $(row).datepicker("destroy");
                    $(row).datepicker({
                        inputs: $(row).find('.datepickersx')
                    });
                } else {}
            },
            initComplete: function(settings, json) {


                var api = new $.fn.dataTable.Api(settings);


                $(this).on('change', '#startTime', function() {
                    $(this).closest('tr').find("#EndTime").trigger("click");
                });
                $(this).on('change', '#EndTime', function() {
                    $(this).closest('tr').find("#startTime").trigger("click");
                });
                $(this).on("click", '.toEdit', { "theTable": api }, function(e) {
                    var theTable = e.data.theTable;
                    var trDom = $(this).closest("tr")[0];
                    var rowData = theTable.row(trDom).data();
                    rowData.state = "edit";
                    theTable.row(trDom).data(rowData);
                    theTable.draw();
                }).on("click", '.toSaveTemp', { "theTable": api }, function(e) {
                    var cancel = false;
                    var i, timeStartTemp, timeEndTemp, timeStartTempArr, timeEndTempArr, timeStartEdit, timeEndEdit;
                    var payType = parseInt(jQuery(".PayType[name=PayType]:checked").val());
                    var fix = "";
                    var theTable = e.data.theTable;
                    var trDom = $(this).closest("tr")[0];
                    var allRowDatas = theTable.rows().data();
                    var rowData = theTable.row(trDom).data();

                    if (payType === 1) {
                        if (!($(this).closest("tr").find("#Price").val())) {
                            $(this).closest("tr").find("#Price").Warning({
                                title: "请填写价格",
                                placement: "top",
                                attach: []
                            });
                            cancel = true;
                        }
                    } else {
                        if (!($(this).closest("tr").find("#startTime").val())) {
                            $(this).closest("tr").find("#startTime").Warning({
                                title: "请您填写开始时间",
                                placement: "top",
                                attach: []
                            });
                            cancel = true;
                        }
                        if (!($(this).closest("tr").find("#EndTime").val())) {
                            $(this).closest("tr").find("#EndTime").Warning({
                                title: "请您填写结束时间",
                            });
                            cancel = true;
                        }
                        if (!($(this).closest("tr").find("#AdultNetPrice").val())) {
                            $(this).closest("tr").find("#AdultNetPrice").Warning({
                                title: "请填写成人价格",
                            });
                            cancel = true;
                        }
                        if (!($(this).closest("tr").find("#ChildNetPrice").val())) {
                            $(this).closest("tr").find("#ChildNetPrice").Warning({
                                title: "请填写儿童价格",
                            });
                            cancel = true;
                        }
                        if (!($(this).closest("tr").find("#BobyNetPrice").val())) {
                            $(this).closest("tr").find("#BobyNetPrice").Warning({
                                title: "请填写婴儿价格",
                            });
                            cancel = true;
                        }
                    }
                    if (cancel === true) {
                        return;
                    }
                    timeStartTempArr = $(this).closest("tr").find("#startTime").val().split("-");
                    timeEndTempArr = $(this).closest("tr").find("#EndTime").val().split("-");
                    timeStartEdit = new Date(timeStartTempArr[0], parseInt(timeStartTempArr[1]) - 1, timeStartTempArr[2], 0, 0, 0).valueOf();
                    timeEndEdit = new Date(timeEndTempArr[0], parseInt(timeEndTempArr[1]) - 1, timeEndTempArr[2], 0, 0, 0).valueOf();
                    for (i = 0; i < allRowDatas.length; i++) {
                        if (allRowDatas[i].state === 'edit') {
                            continue;
                        }
                        if (allRowDatas[i].isChanging === true || allRowDatas[i].isChanging === 'new') {
                            fix = 'Change';
                        } else {
                            fix = "";
                        }
                        timeStartTempArr = allRowDatas[i]['startTime' + fix].split("T")[0].split("-");
                        timeEndTempArr = allRowDatas[i]['EndTime' + fix].split("T")[0].split("-");
                        timeStartTemp = new Date(timeStartTempArr[0], parseInt(timeStartTempArr[1]) - 1, timeStartTempArr[2], 0, 0, 0).valueOf();
                        timeEndTemp = new Date(timeEndTempArr[0], parseInt(timeEndTempArr[1]) - 1, timeEndTempArr[2], 0, 0, 0).valueOf();

                        if (
                            ((timeStartEdit >= timeStartTemp) && (timeStartEdit <= timeEndTemp))) {
                            $(this).closest("tr").find("#startTime").Warning({
                                title: "与序号 " + (parseInt(i) + 1) + " 时间段重叠"
                            });
                            cancel = true;
                        }
                        if (

                            ((timeEndEdit >= timeStartTemp) && (timeEndEdit <= timeEndTemp))
                        ) {
                            $(this).closest("tr").find("#EndTime").Warning({
                                title: "与序号 " + (parseInt(i) + 1) + " 时间段重叠"
                            });
                            cancel = true;
                        }
                        if (
                            (timeStartEdit < timeStartTemp) && (timeEndTemp < timeEndEdit)
                        ) {
                            $(this).closest("tr").find("#startTime").Warning({
                                title: "与序号 " + (parseInt(i) + 1) + " 时间段重叠"
                            });
                            $(this).closest("tr").find("#EndTime").Warning({
                                title: "与序号 " + (parseInt(i) + 1) + " 时间段重叠"
                            });
                            cancel = true;
                        }
                    }
                    if (cancel === true) {
                        return;
                    }
                    //判断结束
                    rowData.state = "read";
                    $(this).closest("tr").find(".price-editing").each(function() {
                        if (rowData.isChanging == true || rowData.isChanging == 'new') {
                            rowData[$(this).attr('id') + "Change"] = $(this).val();
                        } else {
                            rowData[$(this).attr('id')] = $(this).val();
                        }
                    });
                    theTable.row(trDom).data(rowData);
                    var arrAllData = [];
                    for (i = 0; i < theTable.data().length; i++) {
                        arrAllData.push(theTable.data()[i])
                    }
                    theTable.rows().remove();
                    theTable.rows.add(sortByDate(arrAllData));
                    theTable.draw();
                }).on("click", ".toDelete", { "theTable": api }, function(e) {
                    var trDom = $(this).closest("tr")[0];
                    console.log(trDom)
                    var theTable = e.data.theTable;
                    theTable.row(trDom).remove();
                    theTable.draw();
                })
            },
            columnDefs: [{
                'targets': [0],
                'data': null,
                "render": function(cellData, type, rowData, meta) {
                    var indexInpage = meta.row;
                    var settings = meta.settings;
                    return (parseInt(settings._iDisplayStart) + parseInt(indexInpage) + 1);
                }
            }, {
                'targets': [1],
                'data': 'startTime',
                'render': function(cellData, type, rowData, meta) {
                    var editValue;
                    var theTime = cellData ? cellData.toString().split('T')[0] : "";
                    var theChangeTime = rowData.startTimeChange ? rowData.startTimeChange.toString().split('T')[0] : "";
                    if (rowData.state === "edit") {
                        if (rowData.isChanging === false) {
                            editValue = theTime;
                        } else if (rowData.isChanging === "new") {
                            editValue = theChangeTime;
                        } else {
                            editValue = theTime;
                        }
                        return '<input id="startTime" type="text" value="' + editValue + '" class="form-control input-inline datepickersx  price-editing" style="width:150px;margin-right:0px">'
                    } else {
                        if (rowData.isChanging === false) {
                            return theTime;
                        } else if (rowData.isChanging === 'new') {
                            return ('<div class="text-price-changing">' + theChangeTime + '</div>');
                        } else {
                            if (theTime == theChangeTime) {
                                return theTime;
                            } else {
                                return ('<div class="text-price-changing">' + theChangeTime + '</div>' +
                                    '<div class="text-price-deleting">' + theTime + '</div>'
                                )
                            }
                        }
                    }
                }
            }, {
                'targets': [2],
                'data': 'EndTime',
                'render': function(cellData, type, rowData, meta) {
                    var editValue;
                    var theTime = cellData ? cellData.toString().split('T')[0] : "";
                    var theChangeTime = rowData.EndTimeChange ? rowData.EndTimeChange.toString().split('T')[0] : "";
                    if (rowData.state === "edit") {
                        if (rowData.isChanging === false) {
                            editValue = theTime;
                        } else if (rowData.isChanging === "new") {
                            editValue = theChangeTime;
                        } else {
                            editValue = theTime;
                        }
                        return '<input id="EndTime" type="text" value="' + editValue + '" class="form-control input-inline datepickersx  price-editing" style="width:150px;margin-right:0px">'
                    } else {
                        if (rowData.isChanging === false) {
                            return theTime;
                        } else if (rowData.isChanging === 'new') {
                            return ('<div class="text-price-changing">' + theChangeTime + '</div>');
                        } else {
                            if (theTime == theChangeTime) {
                                return theTime;
                            } else {
                                return ('<div class="text-price-changing">' + theChangeTime + '</div>' +
                                    '<div class="text-price-deleting">' + theTime + '</div>'
                                )
                            }
                        }
                    }
                }
            }, {
                'targets': [3],
                'data': "AdultNetPrice",
                'render': function(cellData, type, rowData, meta) {
                    var editValue;
                    if (rowData.state === "edit") {
                        if (rowData.isChanging !== "new") {
                            editValue = rowData.AdultNetPrice;
                        } else {
                            editValue = rowData.AdultNetPriceChange;
                        }
                        return '<input id="AdultNetPrice" type="text" value="' + editValue + '" class="form-control input-inline price-format price-editing" style="width:100px;margin-right:0px">'
                    } else {
                        if (rowData.isChanging === false) {
                            return cellData;
                        } else if (rowData.isChanging === 'new') {
                            return ('<div class="text-price-changing">' + rowData.AdultNetPriceChange + '</div>');
                        } else {
                            if (rowData.AdultNetPrice == rowData.AdultNetPriceChange) {
                                return cellData;
                            } else {
                                return ('<div class="text-price-changing">' + rowData.AdultNetPriceChange + '</div>' +
                                    '<div class="text-price-deleting">' + cellData + '</div>'
                                )
                            }
                        }
                    }
                }
            }, {
                'targets': [4],
                'data': 'ChildNetPrice',
                'render': function(cellData, type, rowData, meta) {
                    var editValue;
                    if (rowData.state === "edit") {
                        if (rowData.isChanging !== 'new') {
                            editValue = rowData.ChildNetPrice;
                        } else {
                            editValue = rowData.ChildNetPriceChange;
                        }
                        return '<input id="ChildNetPrice" type="text" value="' + editValue + '" class="form-control input-inline price-format price-editing" style="width:100px;margin-right:0px">'
                    } else {
                        if (rowData.isChanging === false) {
                            return cellData;
                        } else if (rowData.isChanging === 'new') {
                            return ('<div class="text-price-changing">' + rowData.ChildNetPriceChange + '</div>');
                        } else {
                            if (rowData.ChildNetPrice == rowData.ChildNetPriceChange) {
                                return cellData;
                            } else {
                                return ('<div class="text-price-changing">' + rowData.ChildNetPriceChange + '</div>' +
                                    '<div class="text-price-deleting">' + cellData + '</div>'
                                )
                            }
                        }
                    }
                }
            }, {
                'targets': [5],
                'data': 'BobyNetPrice',
                'render': function(cellData, type, rowData, meta) {
                    var editValue;
                    if (rowData.state === "edit") {
                        if (rowData.isChanging !== "new") {
                            editValue = rowData.BobyNetPrice;
                        } else {
                            editValue = rowData.BobyNetPriceChange;
                        }
                        return '<input id="BobyNetPrice" type="text" value="' + editValue + '" class="form-control input-inline price-format price-editing" style="width:100px;margin-right:0px">'
                    } else {
                        if (rowData.isChanging === false) {
                            return cellData;
                        } else if (rowData.isChanging === 'new') {
                            return ('<div class="text-price-changing">' + rowData.BobyNetPriceChange + '</div>');
                        } else {
                            if (rowData.BobyNetPrice == rowData.BobyNetPriceChange) {
                                return cellData;
                            } else {
                                return ('<div class="text-price-changing">' + rowData.BobyNetPriceChange + '</div>' +
                                    '<div class="text-price-deleting">' + cellData + '</div>'
                                )
                            }
                        }
                    }
                }
            }, {
                'targets': [6],
                'data': 'Price',
                'render': function(cellData, type, rowData, meta) {
                    var editValue;
                    if (rowData.state === "edit") {
                        if (rowData.isChanging !== "new") {
                            editValue = rowData.Price;
                        } else {
                            editValue = rowData.PriceChange;
                        }
                        return '<input id="Price" type="text" value="' + editValue + '" class="form-control input-inline price-format price-editing" style="width:100px;margin-right:0px">'
                    } else {
                        if (rowData.isChanging === false) {
                            return cellData;
                        } else if (rowData.isChanging === 'new') {
                            return ('<div class="text-price-changing">' + rowData.PriceChange + '</div>');
                        } else {
                            if (rowData.Price == rowData.PriceChange) {
                                return cellData;
                            } else {
                                return ('<div class="text-price-changing">' + rowData.PriceChange + '</div>' +
                                    '<div class="text-price-deleting">' + cellData + '</div>'
                                )
                            }
                        }
                    }
                }

            }, {
                'targets': [7],
                'data': null,
                render: function(cellData, type, rowData, meta) {
                    var deleteButton = "";
                    if (rowData.deleteAble === true) {
                        deleteButton = '<button type="button" class="btn ' + "toDelete" + ' btn-default button65">' + '删除' + '</button>';
                    }

                    var alt = (rowData.state === 'read') ? "修改" : "保存";
                    var classname = (rowData.state === 'read') ? "toEdit" : "toSaveTemp";


                    return ('<button type="button" class="btn ' + classname + ' btn-default button65">' + alt + '</button>\n' + deleteButton);
                }
            }]
        });

    if ($("input:radio[name=PayType]:checked").length > 0) {
        var value = $("input:radio[name=PayType]:checked").val();
        if (value == 0) {
            tablePrice.columns([6]).visible(false);
            jQuery("#priceSettings tbody:eq(1) tr td:eq(6)").hide();
            tablePrice.columns([3, 4, 5]).visible(true);
            jQuery("#priceSettings tbody:eq(1) tr td:eq(3)").show();
            jQuery("#priceSettings tbody:eq(1) tr td:eq(4)").show();
            jQuery("#priceSettings tbody:eq(1) tr td:eq(5)").show();

        } else {
            tablePrice.columns([6]).visible(true);
            jQuery("#priceSettings tbody:eq(1) tr td:eq(6)").show();
            tablePrice.columns([3, 4, 5]).visible(false);
            jQuery("#priceSettings tbody:eq(1) tr td:eq(3)").hide();
            jQuery("#priceSettings tbody:eq(1) tr td:eq(4)").hide();
            jQuery("#priceSettings tbody:eq(1) tr td:eq(5)").hide();

        }
        tablePrice.draw();
    }
    return {
        dataTable: tablePrice,
        jqueryTable: jQuery("#priceSettings")
    }







}

function initExtraPrices(data) {
    var extraPricex, i, countID;
    var extraPriceList = [];
    var items = data.item;
    var changeitems = data.changeitem;
    if (items !== null) {
        if (items.IsChange === true && changeitems === null) {
            items = data.changeitem;
            changeitems = data.item;
        }
    }

    var extraServiceList = {};
    var extraServicePriceExistedList = {};
    var extraServicePriceChangingList = {};
    var extraServicePriceList = {};

    if (data.ExtraServices === null) { //没有额外的项目

    } else { //有额外项目
        for (i in data.ExtraServices) {
            extraServiceList[data.ExtraServices[i].ExtraServiceID] = {
                ExtraServiceID: data.ExtraServices[i].ExtraServiceID,
                ServiceName: data.ExtraServices[i].ServiceName,
                ServiceEnName: data.ExtraServices[i].ServiceEnName,
            }
        }
        if (items !== null) {
            for (i in items.ExtraServicePrices) {
                extraServicePriceExistedList[items.ExtraServicePrices[i].ExtraServiceID] = {
                    ServicePrice: items.ExtraServicePrices[i].ServicePrice,
                    ExtraServicePriceID: items.ExtraServicePrices[i].ExtraServicePriceID
                }
            }
        }
        if (changeitems !== null) {
            for (i in changeitems.ExtraServicePrices) {
                extraServicePriceChangingList[changeitems.ExtraServicePrices[i].ExtraServiceID] = {
                    ServicePriceChange: changeitems.ExtraServicePrices[i].ServicePrice
                }
            }
        }
        for (i in extraServiceList) {
            countID = 0;
            if (i in extraServicePriceExistedList) {
                extraServiceList[i].ExtraServicePriceID = extraServicePriceExistedList[i].ExtraServicePriceID;
                extraServiceList[i].ServicePrice = extraServicePriceExistedList[i].ServicePrice;
                countID += 1;
            }
            if (i in extraServicePriceChangingList) {
                extraServiceList[i].ServicePriceChange = extraServicePriceChangingList[i].ServicePriceChange;
                countID += 2;
            }
            if (countID === 0) {
                extraServiceList[i].state = 'edit';
                extraServiceList[i].type = 'new';
            } else if (countID == 1) {
                extraServiceList[i].state = 'read';
                extraServiceList[i].type = 'notChanging';
            } else if (countID == 2) {
                extraServiceList[i].state = 'read';
                extraServiceList[i].type = 'changing';
            } else if (countID == 3) {
                extraServiceList[i].state = 'read';
                extraServiceList[i].type = 'changing';
            }
            extraPricex = new ExtraPrice(extraServiceList[i]);
            extraPriceList.push(extraPricex);
        }
    }
    console.log(extraPriceList);

    var extraPrice =
        jQuery("#extraPriceSettings").DataTable({
            dom: "t",
            ordering: false,
            serverSide: false,
            language: {
                zeroRecords: "",
                emptyTable: '没有附加项目'
            },
            data: extraPriceList,
            initComplete: function(settings, json) {
                var api = new $.fn.dataTable.Api(settings);
                $(this).on("click", '.toEdit', { "theTable": api }, function(e) {
                    var theTable = e.data.theTable;
                    var trDom = $(this).closest("tr")[0];
                    var rowData = theTable.row(trDom).data();
                    rowData.state = "edit";
                    theTable.row(trDom).data(rowData);
                    theTable.draw();
                }).on("click", '.toSaveTemp', { "theTable": api }, function(e) {
                    var theTable = e.data.theTable;
                    var trDom = $(this).closest("tr")[0];
                    var rowData = theTable.row(trDom).data();
                    rowData.state = "read";
                    console.log($(this).closest('tr').find("#ServicePrice").val())
                    if (!($(this).closest('tr').find("#ServicePrice").val())) {
                        $(this).closest("tr").find("#ServicePrice").Warning({
                            title: "请填写价格",
                        });
                        return;
                    }
                    $(trDom).find(".price-editing").each(function() {
                        if (rowData.type === 'changing') {
                            rowData[$(this).attr('id') + "Change"] = $(this).val();
                        } else {
                            rowData[$(this).attr('id')] = $(this).val();
                        }
                    })
                    theTable.row(trDom).data(rowData);
                    theTable.draw();
                });
            },
            columnDefs: [{
                'targets': [0],
                'data': null,
                "render": function(cellData, type, rowData, meta) {
                    var indexInpage = meta.row;
                    var settings = meta.settings;
                    return (parseInt(settings._iDisplayStart) + parseInt(indexInpage) + 1);
                }
            }, {
                'targets': [1],
                'data': 'ServiceName'
            }, {
                'targets': [2],
                'data': 'ServicePrice',
                'render': function(cellData, type, rowData, meta) {
                    if (rowData.state === 'read') {
                        if (rowData.type === 'changing') {
                            if (rowData.ServicePrice != rowData.ServicePriceChange) {
                                return ('<div class="text-price-changing">' + rowData.ServicePriceChange + '</div>' +
                                    '<div class="text-price-deleting">' + rowData.ServicePrice + '</div>'
                                );
                            } else {
                                return cellData;
                            }

                        } else {
                            return cellData;
                        }

                    } else {

                        var editValue = cellData;
                        if (rowData.type === 'changing') {
                            editValue = rowData.ServicePriceChange;
                        }
                        //null的处理
                        if (editValue === null || editValue === undefined) {
                            editValue = "";
                        }
                        return '<input id="ServicePrice" type="text" value="' + editValue + '" class="form-control input-inline price-format price-editing" style="width:100px;margin-right:0px">'
                    }
                }
            }, {
                'targets': [3],
                'data': null,
                'render': function(cellData, type, rowData, meta) {
                    var alt = (rowData.state === 'read') ? "修改" : "保存";
                    var classname = (rowData.state === 'read') ? "toEdit" : "toSaveTemp";
                    return '<button type="button" class="btn ' + classname + ' btn-default button65">' + alt + '</button>';
                }
            }]
        });

    return ({
        dataTable: extraPrice,
        jqueryTable: jQuery("#extraPriceSettings")
    });


}

function priceSave(tables) {
    $('#priceSave').bind("click", { "tables": tables }, function saving(e) {
        var i, fix, payType, cancel = false;
        var priceTable = e.data.tables[0].dataTable;
        var extraPriceTable = e.data.tables[1].dataTable;
        var prices = priceTable.rows().data();
        var extraPrices = extraPriceTable.rows().data();
        if (prices.length === 0) {
            $(this).success("请您先设置价格！")
            return;
        }
        var ItemPriceBySuppliers = [];
        var ExtraServicePrices = [];
        if (jQuery('#currency').val() == "-1") {
            $("body").scrollTop(0);
            $('#Currency .container-warn:eq(0)').Warning({
                'title': "您需要先选择结算货币"
            });
            return;
        }
        if ($('#PayTypeList .PayType[name=PayType]:checked').length === 0) {
            $("body").scrollTop(0);
            $('#PayTypeList .container-warn:eq(0)').Warning({
                'title': "您需要先选择计费标准"
            });
            return;
        }
        if (jQuery("#SelectEffectiveWayList .SelectEffectiveWay[name=SelectEffectiveWay]:checked").length === 0) {
            $("body").scrollTop(0);
            $('#SelectEffectiveWayList .container-warn:eq(0)').Warning({
                'title': "您需要先选择生效方式"
            });
            return;
        }
        payType = parseInt(jQuery("#PayTypeList .PayType[name=PayType]:checked").val());
        for (i = 0; i < prices.length; i++) {
            if (prices[i].state == "edit") {
                cancel = true;
                break;
            } else {
                if (prices[i].isChanging === 'new' || prices[i].isChanging === true) {
                    fix = 'Change';
                } else {
                    fix = "";
                }
                if (payType === 0) {
                    ItemPriceBySuppliers.push({
                        ItemPriceBySupplierID: prices[i].ItemPriceBySupplierID,
                        AdultNetPrice: prices[i]["AdultNetPrice" + fix],
                        BobyNetPrice: prices[i]["BobyNetPrice" + fix],
                        ChildNetPrice: prices[i]['ChildNetPrice' + fix],
                        EndTime: prices[i]['EndTime' + fix],
                        startTime: prices[i]['startTime' + fix]
                    });
                } else {
                    ItemPriceBySuppliers.push({
                        ItemPriceBySupplierID: prices[i].ItemPriceBySupplierID,
                        Price: prices[i]['Price' + fix],
                        EndTime: prices[i]['EndTime' + fix],
                        startTime: prices[i]['startTime' + fix]
                    });
                }
            }

        }
        if (cancel === true) {
            $('#priceSave').success("产品基础价格需要您的确认")
            return;
        }
        for (i = 0; i < extraPrices.length; i++) {
            if (extraPrices[i].state == "edit") {
                cancel = true;
                break;
            } else {
                if (extraPrices[i].type === 'changing') {
                    fix = "Change";
                } else {
                    fix = "";
                }
                ExtraServicePrices.push({
                    ExtraServiceID: extraPrices[i].ExtraServiceID,
                    ExtraServicePriceID: extraPrices[i].ExtraServicePriceID,
                    ServicePrice: extraPrices[i]["ServicePrice" + fix]
                });
            }
        }
        if (cancel === true) {
            $('#priceSave').success("附加项目价格需要您的确认")

            return;
        }
        var post = {
            ServiceItemID: jQuery.trim(jQuery('#ItemID').val()),
            SupplierID: jQuery('#supplier').val(),
            CurrencyID: jQuery('#currency').val(),
            PayType: jQuery("#PayTypeList .PayType[name=PayType]:checked").val(),
            SelectEffectiveWay: jQuery(".SelectEffectiveWay[name=SelectEffectiveWay]:checked").val(),
            Remark: jQuery('#remark').val(),
            ExtraServicePrices: ExtraServicePrices,
            ItemPriceBySuppliers: ItemPriceBySuppliers
        };
        var toast;
        $.ajax({
            type: 'post',
            dataType: 'json',
            contentType: "application/json; charset=utf-8;",
            data: JSON.stringify(post),
            url: '/ServiceItems/SavePriceSetting',
            beforeSend: function() {
                toast = $.LangHua.loadingToast({
                    tip: '正 在 保 存 价 格 设 置. . . . . .'
                });
            },
            success: function(data) {
                if (data.ErrorCode == 200) {
                    toast.modal('hide');
                    window.location.href = "/ServiceItems/Index";
                } else {
                    toast.modal('hide');

                    if (data.ErrorCode == 401) {
                        $.LangHua.alert({
                            title: "提示信息",
                            tip1: '保存失败',
                            tip2: data.ErrorMessage,
                            button: '确定'
                        });
                    } else {
                        $.LangHua.alert({
                            title: "提示信息",
                            tip1: '保存失败',
                            tip2: '保存失败',
                            button: '确定'
                        })
                    }
                    jQuery(_this).one('click', postData);
                }
            },
            error: function() {
                toast.modal('hide');
                $.LangHua.alert({
                    title: "提示信息",
                    tip1: '提交失败',
                    tip2: '提交失败',
                    button: '确定'
                })
                jQuery(_this).one('click', postData);
            }
        });

    });
}

function sortByDate(arr) {
    var i;
    var timeStampNow = (new Date()).valueOf();
    var timeStampStart, timeStampEnd;
    var arrStart, arrEnd;
    var timeStart, timeEnd;
    var arrSorted = [];
    arr.sort(function(a, b) {

        // var timeStampNow = (new Date()).valueOf();
        var timeStampStartA, arrStartA, timeStampStartB, arrStartB;
        var timeA = a.startTimeChange || a.startTime;
        var timeB = b.startTimeChange || b.startTime;
        arrStartA = timeA.split("T")[0].split("-");
        timeStampStartA = (new Date(arrStartA[0], parseInt(arrStartA[1]) - 1, arrStartA[2], 0, 0, 0)).valueOf();
        arrStartB = timeB.split("T")[0].split("-");
        timeStampStartB = (new Date(arrStartB[0], parseInt(arrStartB[1]) - 1, arrStartB[2], 0, 0, 0)).valueOf();
        if (timeStampStartA < timeStampStartB) {
            return 1;
        } else {
            return -1;
        }
    })
    for (i in arr) {
        timeStart = arr[i].startTimeChange || arr[i].startTime;
        timeEnd = arr[i].EndTimeChange || arr[i].EndTime;
        arrStart = timeStart.split("T")[0].split("-");
        arrEnd = timeEnd.split("T")[0].split("-");

        timeStampStart = (new Date(arrStart[0], parseInt(arrStart[1]) - 1, arrStart[2], 0, 0, 0)).valueOf();
        timeStampEnd = (new Date(arrEnd[0], parseInt(arrEnd[1]) - 1, arrEnd[2], 0, 0, 0)).valueOf() + 1000 * 60 * 60 * 24;
        if (
            (timeStampStart <= timeStampNow) &&
            (timeStampNow <= timeStampEnd)
        ) {
            arrSorted.unshift(arr[i]);
        } else {
            arrSorted.push(arr[i])
        }
    }
    return arrSorted;
}