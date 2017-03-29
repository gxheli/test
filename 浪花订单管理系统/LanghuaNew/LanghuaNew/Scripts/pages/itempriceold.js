jQuery(document).ready(function ($) {

    function isNumber(oNum) {
        if (!oNum) return false;
        var strP = /^\d+(\.\d+)?$/;
        if (!strP.test(oNum)) return false;
        try {
            if (parseFloat(oNum) != oNum) return false;
        }
        catch (ex) {
            return false;
        }
        return true;
    }
    var ex = /^\d+$/;
    var date = /((^((1[8-9]\d{2})|([2-9]\d{3}))(-)(10|12|0?[13578])(-)(3[01]|[12][0-9]|0?[1-9])$)|(^((1[8-9]\d{2})|([2-9]\d{3}))(-)(11|0?[469])(-)(30|[12][0-9]|0?[1-9])$)|(^((1[8-9]\d{2})|([2-9]\d{3}))(-)(0?2)(-)(2[0-8]|1[0-9]|0?[1-9])$)|(^([2468][048]00)(-)(0?2)(-)(29)$)|(^([3579][26]00)(-)(0?2)(-)(29)$)|(^([1][89][0][48])(-)(0?2)(-)(29)$)|(^([2-9][0-9][0][48])(-)(0?2)(-)(29)$)|(^([1][89][2468][048])(-)(0?2)(-)(29)$)|(^([2-9][0-9][2468][048])(-)(0?2)(-)(29)$)|(^([1][89][13579][26])(-)(0?2)(-)(29)$)|(^([2-9][0-9][13579][26])(-)(0?2)(-)(29)$))/
    var PayType = jQuery(".PayType[name=PayType]:checked").val();
    if (PayType == 1) {
        jQuery("#tbItemPriceByPerson").addClass("hidden");
        jQuery("#tbItemPriceByOther").removeClass("hidden");
    }
    else {
        jQuery("#tbItemPriceByOther").addClass("hidden");
        jQuery("#tbItemPriceByPerson").removeClass("hidden");
    }
    jQuery('.startTime').find('input').datepicker({
        autoclose: true,
        orientation: "",
        language: 'zh-CN',
        format: 'yyyy-mm-dd',
        startView: 0,
        startDate: '1921-01-01',
        todayHighlight: true,
        todayBtn: 'linked',
        clearBtn: true,
        title: ""
    });
    jQuery('.EndTime').find('input').datepicker({
        autoclose: true,
        orientation: "",
        language: 'zh-CN',
        format: 'yyyy-mm-dd',
        startView: 0,
        startDate: '1921-01-01',
        todayHighlight: true,
        todayBtn: 'linked',
        clearBtn: true,
        title: ""
    });
    jQuery("#supplier").bind('change', function () {
        var ItemID = jQuery("#ItemID").val()
        var supplierID = jQuery(this).val()
        window.location.href = '/ServiceItems/PriceSetting?ItemID=' + ItemID + '&SupplierID=' + supplierID;
    })

    jQuery("#btnAddPriceByPerson").bind('click', function () {
        var a = jQuery("#tbItemPriceByPerson tr:last input:eq(0)");
        var b = jQuery("#tbItemPriceByPerson tr:last input:eq(1)");
        var c = jQuery("#tbItemPriceByPerson tr:last input:eq(2)");
        var d = jQuery("#tbItemPriceByPerson tr:last input:eq(3)");
        var e = jQuery("#tbItemPriceByPerson tr:last input:eq(4)");

        var check = true;
        if (a.val() == "") {
            a.warning("请填写");
            check = false;
        }
        if (!date.test(a.val())) {
            a.warning("时间格式不正确");
            check = false;
        }
        if (b.val() == "") {
            b.warning("请填写");
            check = false;
        }
        if (!date.test(b.val())) {
            b.warning("时间格式不正确");
            check = false;
        }
        if (c.val() == "") {
            c.warning("请填写");
            check = false;
        }
        if (!isNumber(c.val())) {
            c.warning("只能填写数字");
            check = false;
        }
        if (d.val() == "") {
            d.warning("请填写");
            check = false;
        }
        if (!isNumber(d.val())) {
            d.warning("只能填写数字");
            check = false;
        }
        if (e.val() == "") {
            e.warning("请填写");
            check = false;
        }
        if (!isNumber(e.val())) {
            e.warning("只能填写数字");
            check = false;
        }
        if (a.val() > b.val()) {
            a.warning("开始时间必须小于或等于结束时间");
            b.warning("开始时间必须小于或等于结束时间");
            check = false;
        }
        jQuery("#tbItemPriceByPerson tbody tr.itemRow").each(function () {
            var DateBegin = jQuery(this).find("td input:eq(0)").val();
            var DateEnd = jQuery(this).find("td input:eq(1)").val();
            if (a.val() == DateBegin || a.val() == DateEnd) {
                a.warning("日期不能相同");
                check = false;
            }
            if (b.val() == DateBegin || b.val() == DateEnd) {
                b.warning("日期不能相同");
                check = false;
            }
            if (a.val() > DateBegin & a.val() < DateEnd) {
                a.warning("时间跨度不能重叠");
                check = false;
            }
            if (b.val() > DateBegin & b.val() < DateEnd) {
                b.warning("时间跨度不能重叠");
                check = false;
            }
            if ((DateBegin > a.val() & DateBegin < b.val()) || (DateEnd > a.val() & DateEnd < b.val())) {
                a.warning("时间跨度不能重叠");
                b.warning("时间跨度不能重叠");
                check = false;
            }
        })
        if (!check) return;
        var count = jQuery("#tbItemPriceByPerson tbody tr").length;
        var trHtml = "<tr class='itemRow'><td>" + count + "</td>" +
            "<td class='startTime'><span>" + a.val() + "</span><input class='form-control input-inline hidden' style='width:150px;' value='" + a.val() + "'/></td>" +
            "<td class='EndTime'><span>" + b.val() + "</span><input class='form-control input-inline hidden' style='width:150px;' value='" + b.val() + "'/></td>" +
            "<td class='AdultNetPrice'><span>" + c.val() + "</span><input class='form-control input-inline hidden' style='width:80px;' value='" + c.val() + "'/></td>" +
            "<td class='ChildNetPrice'><span>" + d.val() + "</span><input class='form-control input-inline hidden' style='width:80px;' value='" + d.val() + "'/></td>" +
            "<td class='BobyNetPrice'><span>" + e.val() + "</span><input class='form-control input-inline hidden' style='width:80px;' value='" + e.val() + "'/></td>" +
            "<td><a id='update' class='btn btn-sm btn-default button65'>修改</a> " +
            "<a id='save' class='btn btn-sm btn-primary button65 hidden'>保存</a> " +
            "<a id='delete' class='btn btn-sm btn-default button45'>删除</a></td></tr>";
        jQuery("#tbItemPriceByPerson tr:last").before(trHtml);
        a.val("");
        b.val("");
        c.val("");
        d.val("");
        e.val("");

        jQuery('.startTime').find('input').datepicker({
            autoclose: true,
            orientation: "",
            language: 'cn',
            format: 'yyyy-mm-dd',
            startView: 3,
            startDate: '1921-01-01',
            todayHighlight: true,
            todayBtn: 'linked',
            clearBtn: true,
            title: ""
        });
        jQuery('.EndTime').find('input').datepicker({
            autoclose: true,
            orientation: "",
            language: 'cn',
            format: 'yyyy-mm-dd',
            startView: 3,
            startDate: '1921-01-01',
            todayHighlight: true,
            todayBtn: 'linked',
            clearBtn: true,
            title: ""
        });
    })
    jQuery("#btnAddPriceByOther").bind('click', function () {
        var a = jQuery("#tbItemPriceByOther tr:last input:eq(0)");
        var b = jQuery("#tbItemPriceByOther tr:last input:eq(1)");
        var c = jQuery("#tbItemPriceByOther tr:last input:eq(2)");

        var check = true;
        if (a.val() == "") {
            a.warning("请填写");
            check = false;
        }
        if (!date.test(a.val())) {
            a.warning("时间格式不正确");
            check = false;
        }
        if (b.val() == "") {
            b.warning("请填写");
            check = false;
        }
        if (!date.test(b.val())) {
            b.warning("时间格式不正确");
            check = false;
        }
        if (c.val() == "") {
            c.warning("请填写");
            check = false;
        }
        if (!isNumber(c.val())) {
            c.warning("只能填写数字");
            check = false;
        }
        if (a.val() > b.val()) {
            a.warning("开始时间必须小于或等于结束时间");
            b.warning("开始时间必须小于或等于结束时间");
            check = false;
        }
        jQuery("#tbItemPriceByOther tbody tr.itemRow").each(function () {
            var DateBegin = jQuery(this).find("td input:eq(0)").val();
            var DateEnd = jQuery(this).find("td input:eq(1)").val();
            if (a.val() == DateBegin || a.val() == DateEnd) {
                a.warning("日期不能相同");
                check = false;
            }
            if (b.val() == DateBegin || b.val() == DateEnd) {
                b.warning("日期不能相同");
                check = false;
            }
            if (a.val() > DateBegin & a.val() < DateEnd) {
                a.warning("时间跨度不能重叠");
                check = false;
            }
            if (b.val() > DateBegin & b.val() < DateEnd) {
                b.warning("时间跨度不能重叠");
                check = false;
            }
            if ((DateBegin > a.val() & DateBegin < b.val()) || (DateEnd > a.val() & DateEnd < b.val())) {
                a.warning("时间跨度不能重叠");
                b.warning("时间跨度不能重叠");
                check = false;
            }
        })
        if (!check) return;
        var count = jQuery("#tbItemPriceByOther tbody tr").length;
        var trHtml = "<tr class='itemRow'><td>" + count + "</td>" +
            "<td class='startTime'><span>" + a.val() + "</span><input class='form-control input-inline hidden' style='width:150px;' value='" + a.val() + "'/></td>" +
            "<td class='EndTime'><span>" + b.val() + "</span><input class='form-control input-inline hidden' style='width:150px;' value='" + b.val() + "'/></td>" +
            "<td class='Price'><span>" + c.val() + "</span><input class='form-control input-inline hidden' style='width:80px;' value='" + c.val() + "'/></td>" +
            "<td><a id='update' class='btn btn-sm btn-default button65'>修改</a> " +
            "<a id='save' class='btn btn-sm btn-primary button65 hidden'>保存</a> " +
            "<a id='delete' class='btn btn-sm btn-default button65'>删除</a></td></tr>";
        jQuery("#tbItemPriceByOther tr:last").before(trHtml);
        a.val("");
        b.val("");
        c.val("");
        d.val("");
        e.val("");

        jQuery('.startTime').find('input').datepicker({
            autoclose: true,
            orientation: "",
            language: 'cn',
            format: 'yyyy-mm-dd',
            startView: 3,
            startDate: '1921-01-01',
            todayHighlight: true,
            todayBtn: 'linked',
            clearBtn: true,
            title: ""
        });
        jQuery('.EndTime').find('input').datepicker({
            autoclose: true,
            orientation: "",
            language: 'cn',
            format: 'yyyy-mm-dd',
            startView: 3,
            startDate: '1921-01-01',
            todayHighlight: true,
            todayBtn: 'linked',
            clearBtn: true,
            title: ""
        });
    })

    jQuery(".PayType").bind('change', function () {
        var PayType = jQuery(".PayType[name=PayType]:checked").val();
        if (PayType == 0) {
            jQuery("#tbItemPriceByOther").addClass("hidden");
            jQuery("#tbItemPriceByPerson").removeClass("hidden");
        }
        else {
            jQuery("#tbItemPriceByPerson").addClass("hidden");
            jQuery("#tbItemPriceByOther").removeClass("hidden");
        }
    })

    jQuery("#tbItemPriceByPerson").on('click', "#delete", function () {
        jQuery(this).closest("tr").remove();
    });
    jQuery("#tbItemPriceByPerson").on('click', "#update", function () {
        jQuery(this).closest("tr").find("td input").removeClass("hidden");
        jQuery(this).closest("tr").find("td span").addClass("hidden");
        jQuery(this).closest("tr").find("td #save").removeClass("hidden");
        jQuery(this).closest("tr").find("td #save").addClass("ischeck");
        jQuery(this).closest("tr").find("td #update").addClass("hidden");
    });
    jQuery("#tbItemPriceByPerson").on('click', "#save", function () {
        var check = true;
        jQuery(this).closest("tr").find("td").each(function () {
            if (jQuery(this).find("input").val() == "") {
                jQuery(this).find("input").warning("请填写");
                check = false;
            }
        });
        var a = jQuery(this).closest("tr").find("td input:eq(0)");
        var b = jQuery(this).closest("tr").find("td input:eq(1)");
        var c = jQuery(this).closest("tr").find("td input:eq(2)");

        if (a.val() == "") {
            a.warning("请填写");
            check = false;
        }
        if (!date.test(a.val())) {
            a.warning("时间格式不正确");
            check = false;
        }
        if (b.val() == "") {
            b.warning("请填写");
            check = false;
        }
        if (!date.test(b.val())) {
            b.warning("时间格式不正确");
            check = false;
        }
        if (c.val() == "") {
            c.warning("请填写");
            check = false;
        }
        if (!isNumber(c.val())) {
            c.warning("只能填写数字");
            check = false;
        }
        if (a.val() > b.val()) {
            a.warning("开始时间必须小于或等于结束时间");
            b.warning("开始时间必须小于或等于结束时间");
            check = false;
        }
        jQuery(this).closest("tr").removeClass("itemRow");
        jQuery("#tbItemPriceByPerson tbody tr.itemRow").each(function () {
            var DateBegin = jQuery(this).find("td input:eq(0)").val();
            var DateEnd = jQuery(this).find("td input:eq(1)").val();
            if (a.val() == DateBegin || a.val() == DateEnd) {
                a.warning("日期不能相同");
                check = false;
            }
            if (b.val() == DateBegin || b.val() == DateEnd) {
                b.warning("日期不能相同");
                check = false;
            }
            if (a.val() > DateBegin & a.val() < DateEnd) {
                a.warning("时间跨度不能重叠");
                check = false;
            }
            if (b.val() > DateBegin & b.val() < DateEnd) {
                b.warning("时间跨度不能重叠");
                check = false;
            }
            if ((DateBegin > a.val() & DateBegin < b.val()) || (DateEnd > a.val() & DateEnd < b.val())) {
                a.warning("时间跨度不能重叠");
                b.warning("时间跨度不能重叠");
                check = false;
            }
        })
        jQuery(this).closest("tr").addClass("itemRow");
        if (check) {
            jQuery(this).closest("tr").find("td").each(function () {
                jQuery(this).find("span").text(jQuery(this).find("input").val());
            });
            jQuery(this).closest("tr").find("td input").addClass("hidden");
            jQuery(this).closest("tr").find("td span").removeClass("hidden");
            jQuery(this).closest("tr").find("td #save").addClass("hidden");
            jQuery(this).closest("tr").find("td #save").removeClass("ischeck");
            jQuery(this).closest("tr").find("td #update").removeClass("hidden");
        }
    });
    jQuery("#tbItemPriceByOther").on('click', "#delete", function () {
        jQuery(this).closest("tr").remove();
    });
    jQuery("#tbItemPriceByOther").on('click', "#update", function () {
        jQuery(this).closest("tr").find("td input").removeClass("hidden");
        jQuery(this).closest("tr").find("td span").addClass("hidden");
        jQuery(this).closest("tr").find("td #save").removeClass("hidden");
        jQuery(this).closest("tr").find("td #save").addClass("ischeck");
        jQuery(this).closest("tr").find("td #update").addClass("hidden");
    });
    jQuery("#tbItemPriceByOther").on('click', "#save", function () {
        var check = true;
        jQuery(this).closest("tr").find("td").each(function () {
            if (jQuery(this).find("input").val() == "") {
                jQuery(this).find("input").warning("请填写");
                check = false;
            }
        });
        var a = jQuery(this).closest("tr").find("td input:eq(0)");
        var b = jQuery(this).closest("tr").find("td input:eq(1)");
        var c = jQuery(this).closest("tr").find("td input:eq(2)");

        if (a.val() == "") {
            a.warning("请填写");
            check = false;
        }
        if (!date.test(a.val())) {
            a.warning("时间格式不正确");
            check = false;
        }
        if (b.val() == "") {
            b.warning("请填写");
            check = false;
        }
        if (!date.test(b.val())) {
            b.warning("时间格式不正确");
            check = false;
        }
        if (c.val() == "") {
            c.warning("请填写");
            check = false;
        }
        if (!isNumber(c.val())) {
            c.warning("只能填写数字");
            check = false;
        }
        if (a.val() > b.val()) {
            a.warning("开始时间必须小于或等于结束时间");
            b.warning("开始时间必须小于或等于结束时间");
            check = false;
        }
        jQuery(this).closest("tr").removeClass("itemRow");
        jQuery("#tbItemPriceByOther tbody tr.itemRow").each(function () {
            var DateBegin = jQuery(this).find("td input:eq(0)").val();
            var DateEnd = jQuery(this).find("td input:eq(1)").val();
            if (a.val() == DateBegin || a.val() == DateEnd) {
                a.warning("日期不能相同");
                check = false;
            }
            if (b.val() == DateBegin || b.val() == DateEnd) {
                b.warning("日期不能相同");
                check = false;
            }
            if (a.val() > DateBegin & a.val() < DateEnd) {
                a.warning("时间跨度不能重叠");
                check = false;
            }
            if (b.val() > DateBegin & b.val() < DateEnd) {
                b.warning("时间跨度不能重叠");
                check = false;
            }
            if ((DateBegin > a.val() & DateBegin < b.val()) || (DateEnd > a.val() & DateEnd < b.val())) {
                a.warning("时间跨度不能重叠");
                b.warning("时间跨度不能重叠");
                check = false;
            }
        })
        jQuery(this).closest("tr").addClass("itemRow");
        if (check) {
            jQuery(this).closest("tr").find("td").each(function () {
                jQuery(this).find("span").text(jQuery(this).find("input").val());
            });
            jQuery(this).closest("tr").find("td input").addClass("hidden");
            jQuery(this).closest("tr").find("td span").removeClass("hidden");
            jQuery(this).closest("tr").find("td #save").addClass("hidden");
            jQuery(this).closest("tr").find("td #save").removeClass("ischeck");
            jQuery(this).closest("tr").find("td #update").removeClass("hidden");
        }
    });
    jQuery("#tbExtraService").on('click', "#btnUpdateExPrice", function () {
        jQuery(this).closest("tr").find("td input").removeClass("hidden");
        jQuery(this).closest("tr").find("td span").addClass("hidden");
        jQuery(this).closest("tr").find("td #btnSaveExPrice").removeClass("hidden");
        jQuery(this).closest("tr").find("td #btnSaveExPrice").addClass("ischeck");
        jQuery(this).closest("tr").find("td #btnUpdateExPrice").addClass("hidden");
    });
    jQuery("#tbExtraService").on('click', "#btnSaveExPrice", function () {
        var check = true;
        jQuery(this).closest("tr").find("td").each(function () {
            if (jQuery(this).find("input").val() == "") {
                jQuery(this).find("input").warning("请填写");
                check = false;
            }
        });
        if (check) {
            jQuery(this).closest("tr").find("td").each(function () {
                jQuery(this).find("span").text(jQuery(this).find("input").val());
            });
            jQuery(this).closest("tr").find("td input").addClass("hidden");
            jQuery(this).closest("tr").find("td span").removeClass("hidden");
            jQuery(this).closest("tr").find("td #btnSaveExPrice").addClass("hidden");
            jQuery(this).closest("tr").find("td #btnSaveExPrice").removeClass("ischeck");
            jQuery(this).closest("tr").find("td #btnUpdateExPrice").removeClass("hidden");
        }
    });
    jQuery("#btnSave").one('click', function postData() {
        var _this = this;
        var Result = getPost();
        if (Result.post) {
            if (Result.item.length == 0) {
                return;
            }
            $.ajax({
                type: 'post',
                dataType: 'json',
                contentType: "application/json; charset=utf-8;",
                data: JSON.stringify(Result.item),
                url: '/ServiceItems/SavePriceSetting',
                success: function (data) {
                    if (data.ErrorCode == 200) {
                        window.location.href = "/ServiceItems/Index";
                    }
                    else {
                        if (data.ErrorCode == 401) {
                            $.LangHua.alert({
                                title: "提示信息",
                                tip1: '提交失败',
                                tip2: data.ErrorMessage,
                                button: '确定'
                            })
                        }
                        jQuery(_this).one('click', postData);
                    }
                },
                failed: function () {
                    jQuery(_this).one('click', postData);
                }
            });
        }
        else {
            jQuery(_this).one('click', postData);
        }
    });



    function getPost() {
        var post = true;
        if (jQuery('#tbExtraService tr td .ischeck').length != 0) {
            jQuery('#tbExtraService tr td .ischeck').warning("请先保存再提交");
            post = false;
        }
        var PayType = jQuery(".PayType[name=PayType]:checked").val();
        if (PayType == 1) {
            if (jQuery('#tbItemPriceByOther tr td .ischeck').length != 0) {
                jQuery('#tbItemPriceByOther tr td .ischeck').warning("请先保存再提交");
                post = false;
            }
        }
        else if (PayType == 0) {
            if (jQuery('#tbItemPriceByPerson tr td .ischeck').length != 0) {
                jQuery('#tbItemPriceByPerson tr td .ischeck').warning("请先保存再提交");
                post = false;
            }
        }
        var ItemPriceBySuppliers = [];
        if (PayType == 0) {
            jQuery('#tbItemPriceByPerson tbody tr.itemRow').each(function () {
                var ItemPrice = {
                    "startTime": jQuery(this).find("td.startTime span").text(),
                    "EndTime": jQuery(this).find("td.EndTime span").text(),
                    "AdultNetPrice": jQuery(this).find("td.AdultNetPrice span").text(),
                    "ChildNetPrice": jQuery(this).find("td.ChildNetPrice span").text(),
                    "BobyNetPrice": jQuery(this).find("td.BobyNetPrice span").text(),
                }
                ItemPriceBySuppliers.push(ItemPrice);
            });
            if (ItemPriceBySuppliers.length == 0) {
                jQuery('#tbItemPriceByPerson').focus();
                jQuery('#tbItemPriceByPerson').warning("请填写基础价格");
                post = false;
            }
        }
        else if (PayType == 1) {
            jQuery('#tbItemPriceByOther tbody tr.itemRow').each(function () {
                var ItemPrice = {
                    "startTime": jQuery(this).find("td.startTime span").text(),
                    "EndTime": jQuery(this).find("td.EndTime span").text(),
                    "Price": jQuery(this).find("td.Price span").text(),
                }
                ItemPriceBySuppliers.push(ItemPrice);
            });
            if (ItemPriceBySuppliers.length == 0) {
                jQuery('#tbItemPriceByOther').focus();
                jQuery('#tbItemPriceByOther').warning("请填写基础价格");
                post = false;
            }
        }
        var ExtraServicePrices = [];
        jQuery('#tbExtraService tbody tr.itemRow').each(function () {
            var extraService = {
                "ExtraServiceID": jQuery(this).find("td input#ExtraServiceID").val(),
                "ServicePrice": jQuery(this).find("td.servicePrice span").text(),
            }
            ExtraServicePrices.push(extraService);
        });
        var item = {
            ItemPriceBySuppliers: ItemPriceBySuppliers,
            ExtraServicePrices: ExtraServicePrices,
            ServiceItemID: jQuery.trim(jQuery('#ItemID').val()),
            SupplierID: jQuery('#supplier').val(),
            CurrencyID: (function () {
                var value = jQuery('#currency').val();
                if (!value) {
                    jQuery('#currency').warning("请选择");
                    post = false;
                }
                return value;
            })(),
            PayType: (function () {
                var value = jQuery(".PayType[name=PayType]:checked").val();
                if (!value) {
                    jQuery('.PayTypeList').warning("请选择");
                    post = false;
                }
                return value;
            })(),
            SelectEffectiveWay: (function () {
                var value = jQuery(".SelectEffectiveWay[name=SelectEffectiveWay]:checked").val()
                if (!value) {
                    jQuery('.SelectEffectiveWayList').warning("请选择");
                    post = false;
                }
                return value;
            })()
        };
        return {
            post: post,
            item: item
        }
    }
});