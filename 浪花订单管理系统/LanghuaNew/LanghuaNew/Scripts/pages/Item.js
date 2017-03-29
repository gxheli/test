jQuery(document).ready(function($) {
    var ex = /^[0-9]\d*$/;
    var left = jQuery("#supplierLeft option:selected");
    for (i = 0; i < left.length; i++) {
        jQuery("#supplierRight").append(left[i]);
    }
    var right = jQuery("#supplierRight option:selected");
    for (i = 0; i < right.length; i++) {
        right[i].selected = false;
    }
    jQuery("#default").bind('click', function() {
        var all = jQuery("#supplierRight option");
        for (i = 0; i < all.length; i++) {
            all[i].text = all[i].text.replace("(默认)", "");
        }
        var right = jQuery("#supplierRight option:selected");
        for (i = 0; i < right.length; i++) {
            right[i].text += "(默认)";
            break;
        }
    });
    jQuery("#yes").bind('click', function() {
        var right = jQuery("#supplierRight option:selected");
        for (i = 0; i < right.length; i++) {
            right[i].selected = false;
        }
        var left = jQuery("#supplierLeft option:selected");
        for (i = 0; i < left.length; i++) {
            jQuery("#supplierRight").append(left[i]);
        }
    });
    jQuery("#no").bind('click', function() {
        var left = jQuery("#supplierLeft option:selected");
        for (i = 0; i < left.length; i++) {
            left[i].selected = false;
        }
        var right = jQuery("#supplierRight option:selected");
        for (i = 0; i < right.length; i++) {
            right[i].text = right[i].text.replace("(默认)", "");
            jQuery("#supplierLeft").append(right[i]);
        }
    });
    if (jQuery('#ServiceType input.ServiceType[name=ServiceType]:checked').val() == 2) {
        jQuery("#divFixedDays").css("display", "");
    }

    jQuery("#tbExtraService").on('click', "#delete", function() {
        jQuery(this).closest("tr").remove();
    });
    jQuery("#tbExtraService").on('click', "#update", function() {
        jQuery(this).closest("tr").find("td input").removeClass("hidden");
        jQuery(this).closest("tr").find("td span").addClass("hidden");
        jQuery(this).closest("tr").find("td #save").removeClass("hidden");
        jQuery(this).closest("tr").find("td #save").addClass("ischeck");
        jQuery(this).closest("tr").find("td #update").addClass("hidden");
    });
    jQuery("#tbExtraService").on('click', "#save", function() {
        var check = true;
        //jQuery(this).closest("tr").find("td").each(function () {
        //    if (jQuery(this).find("input").val() == "") {
        //        jQuery(this).find("input").warning("请填写");
        //        check = false;
        //    }
        //});
        var a = jQuery(this).closest("tr").find("td input:eq(0)");
        var b = jQuery(this).closest("tr").find("td input:eq(1)");
        var c = jQuery(this).closest("tr").find("td input:eq(2)");
        var d = jQuery(this).closest("tr").find("td input:eq(3)");
        var e = jQuery(this).closest("tr").find("td input:eq(4)");
        var check = true;

        if (a.val() == "") {
            a.warning("请填写");
            check = false;
        } else {
            if (a.val().search(/['"]/) !== -1) {
                a.warning("不要含有英文的单、双引号");
                check = false;
            }
            if (a.val().search(/[\\\/\*\?\|:<>]/) !== -1) {
                a.warning('文件名不能包含下列任何字符：\\ / : * ? " < > |');
                check = false;
            }
        }
        if (b.val() == "") {
            b.warning("请填写");
            check = false;
        } else {
            if (b.val().search(/['"]/) !== -1) {
                b.warning("不要含有英文的单、双引号");
                check = false;
            }
            if (b.val().search(/[\\\/\*\?\|:<>]/) !== -1) {
                b.warning('文件名不能包含下列任何字符：\\ / : * ? " < > |');
                check = false;
            }
        }
        if (c.val() == "") {
            c.warning("请填写");
            check = false;
        } else {
            if (c.val().search(/['"]/) !== -1) {
                c.warning("不要含有英文的单、双引号");
                check = false;
            }
        }
        if (d.val() == "") {
            d.warning("请填写");
            check = false;
        }
        if (!ex.test(d.val())) {
            d.warning("只能填写正整数");
            check = false;
        }
        if (e.val() == "") {
            e.warning("请填写");
            check = false;
        }
        if (!ex.test(e.val())) {
            e.warning("只能填写正整数");
            check = false;
        }
        if (parseInt(d.val()) >= parseInt(e.val())) {
            e.warning("最大值必须大于最小值");
            check = false;
        }
        jQuery(this).closest("tr").removeClass("itemRow");
        jQuery("#tbExtraService tbody tr.itemRow").each(function() {
            var serviceName = jQuery(this).find("td input:eq(0)").val();
            if (a.val().trim() == serviceName.trim()) {
                a.warning("附加项目名称不能相同");
                check = false;
            }
        })
        jQuery(this).closest("tr").addClass("itemRow");

        if (check) {
            jQuery(this).closest("tr").find("td").each(function() {
                jQuery(this).find("span").text(jQuery(this).find("input").val());
            });
            jQuery(this).closest("tr").find("td input").addClass("hidden");
            jQuery(this).closest("tr").find("td span").removeClass("hidden");
            jQuery(this).closest("tr").find("td #save").addClass("hidden");
            jQuery(this).closest("tr").find("td #save").removeClass("ischeck");
            jQuery(this).closest("tr").find("td #update").removeClass("hidden");
        }
    });
    jQuery("#ServiceType").on('click', ".ServiceType", function() {
        if (jQuery(this).val() == 2) {
            jQuery("#divFixedDays").css("display", "");
        } else {
            jQuery("#divFixedDays").css("display", "none");
        }
    });


    jQuery("#CheckCode").bind('click', function() {
        CheckCode();
    });

    function CheckCode() {
        var ServiceCode = jQuery("#ServiceCode").val();
        if (!ServiceCode) {
            jQuery('#ServiceCode').warning("请填写");
            return;
        }
        $.ajax({
            type: 'get',
            dataType: 'json',
            data: { ServiceItemID: jQuery.trim(jQuery('#ServiceItemID').val()), ServiceCode: ServiceCode },
            url: '/ServiceItems/IsExistServiceCode',
            success: function(data) {
                if (data.ErrorCode == 200) {
                    if (jQuery('#ServiceItemID').length == 0)
                        jQuery("#CheckCodeMsg").text("没有检查到有产品使用该编码");
                    else
                        jQuery("#CheckCodeMsg").text("没有检查到有其他产品使用该编码");
                } else {
                    if (jQuery('#ServiceItemID').length == 0) {
                        jQuery("#CheckCodeMsg").text(data.data);
                        jQuery('#ServiceCode').warning("已存在产品使用该编码");
                    } else {
                        jQuery("#CheckCodeMsg").text(data.data);
                        jQuery('#ServiceCode').warning("已存在其他产品使用该编码");
                    }
                }
            }
        })
    }
    jQuery("#btnAddx").bind('click', function() {

        var a = jQuery("#tbExtraService tr:last input:eq(0)");
        var b = jQuery("#tbExtraService tr:last input:eq(1)");
        var c = jQuery("#tbExtraService tr:last input:eq(2)");
        var d = jQuery("#tbExtraService tr:last input:eq(3)");
        var e = jQuery("#tbExtraService tr:last input:eq(4)");
        var check = true;
        if (a.val() == "") {
            a.warning("请填写");
            check = false;
        } else {
            if (a.val().search(/['"]/) !== -1) {
                a.warning("不要含有英文的单、双引号");
                check = false;
            }
        }
        if (b.val() == "") {
            b.warning("请填写");
            check = false;
        } else {
            if (b.val().search(/['"]/) !== -1) {
                b.warning("不要含有英文的单、双引号");
                check = false;
            }
        }
        if (c.val() == "") {
            c.warning("请填写");
            check = false;
        } else {
            if (c.val().search(/['"]/) !== -1) {
                c.warning("不要含有英文的单、双引号");
                check = false;
            }
        }
        if (d.val() == "") {
            d.warning("请填写");
            check = false;
        }
        if (!ex.test(d.val())) {
            d.warning("只能填写正整数");
            check = false;
        }
        if (e.val() == "") {
            e.warning("请填写");
            check = false;
        }
        if (!ex.test(e.val())) {
            e.warning("只能填写正整数");
            check = false;
        }
        if (parseInt(d.val()) >= parseInt(e.val())) {
            e.warning("最大值必须大于最小值");
            check = false;
        }
        jQuery("#tbExtraService tbody tr.itemRow").each(function() {
            var serviceName = jQuery(this).find("td input:eq(0)").val();
            if (a.val().trim() == serviceName.trim()) {
                a.warning("附加项目名称不能相同");
                check = false;
            }
        })
        if (!check) return;
        //var trHtml = "<tr class='itemRow'><td class='serviceName'>" + a.val() + "</td><td class='serviceEnName'>" + b.val() + "</td><td class='serviceUnit'>" + c.val() + "</td><td class='minNum'>" + d.val() + "</td><td class='maxNum'>" + e.val() + "</td><td><a id='update' class='ddbutton  border-rounded'>修改</a><a id='delete' class='ddbutton  border-rounded'>删除</a></td></tr>";
        var trHtml = "<tr class='itemRow'><td class='serviceName'><span>" + a.val() + "</span><input class='form-control input-inline hidden' style='width:150px;' value='" + a.val() + "'/></td>" +
            "<td class='serviceEnName'><span>" + b.val() + "</span><input class='form-control input-inline hidden' style='width:150px;' value='" + b.val() + "'/></td>" +
            "<td class='serviceUnit'><span>" + c.val() + "</span><input class='form-control input-inline hidden' style='width:80px;' value='" + c.val() + "'/></td>" +
            "<td class='minNum'><span>" + d.val() + "</span><input class='form-control input-inline hidden' style='width:80px;' value='" + d.val() + "'/></td>" +
            "<td class='maxNum'><span>" + e.val() + "</span><input class='form-control input-inline hidden' style='width:80px;' value='" + e.val() + "'/></td>" +
            "<td><a id='update' class='btn btn-sm btn-default button65'>修改</a> " +
            "<a id='save' class='btn btn-sm btn-primary button65 hidden'>保存</a> " +
            "<a id='delete' class='btn btn-sm btn-default button45'>删除</a></td></tr>";
        jQuery("#tbExtraService tr:last").before(trHtml);
        a.val("");
        b.val("");
        c.val("");
        d.val("");
        e.val("");

    });
    jQuery("#oneMoreRow").on("click", function() {

        var extraService = {
            "ExtraServiceID": jQuery(this).find("td input.serviceID").val(),
            "ServiceName": jQuery(this).find("td.serviceName span").text(),
            "ServiceEnName": jQuery(this).find("td.serviceEnName span").text(),
            "ServiceUnit": jQuery(this).find("td.serviceUnit span").text(),
            "MinNum": jQuery(this).find("td.minNum span").text(),
            "MaxNum": jQuery(this).find("td.maxNum span").text(),
        }
        var tr = jQuery('<tr class="itemRow"></tr>');
        var td　 = jQuery('<td></td>');
        var input = jQuery("<input/>").addClass("form-control input-inline ").css("margin-right", "0px");
        var span = jQuery("<span></span>").addClass("alt hidden");
        var button = jQuery("<a  class='btn btn-sm'></a>");
        tr.append(
            td.clone().append(input.clone().addClass("input200"), span.clone()).addClass("serviceName"),
            td.clone().append(input.clone().addClass("input200"), span.clone()).addClass("serviceEnName"),
            td.clone().append(input.clone().addClass("input80"), span.clone()).addClass("serviceUnit"),
            td.clone().append(input.clone().addClass("input80"), span.clone()).addClass("minNum"),
            td.clone().append(input.clone().addClass("input80"), span.clone()).addClass("maxNum"),
            td.clone().append(
                button.clone().text("保存").addClass("btn-primary button65 ischeck").attr("id", "save"),
                "\n",
                button.clone().text("修改").addClass("btn-default button65 hidden").attr("id", "update"),
                "\n",
                button.clone().text("删除").addClass("btn-default button45").attr("id", "delete")
            )
        );
        jQuery("#tbExtraService tr:last").before(tr);
        $("body").scrollTop(parseInt($("body").scrollTop()) + 50);
    });

    jQuery("#btnSave").one('click', function postData() {
        var _this = this;
        var Result = getPost();
        if (Result.post) {
            if (Result.item.serviceItem.length == 0) {
                return;
            }
            $.ajax({
                type: 'get',
                dataType: 'json',
                data: { ServiceItemID: jQuery.trim(jQuery('#ServiceItemID').val()), ServiceCode: jQuery("#ServiceCode").val() },
                url: '/ServiceItems/IsExistServiceCode',
                success: function(data) {
                    if (data.ErrorCode == 200) {
                        if (jQuery('#ServiceItemID').length == 0)
                            jQuery("#CheckCodeMsg").text("没有检查到有产品使用该编码");
                        else
                            jQuery("#CheckCodeMsg").text("没有检查到有其他产品使用该编码");
                        $.ajax({
                            type: 'post',
                            dataType: 'json',
                            contentType: "application/json; charset=utf-8;",
                            data: JSON.stringify(Result.item),
                            url: '/ServiceItems/SaveServiceItem',
                            success: function(data) {
                                if (data.ErrorCode == 200) {
                                    //alert("返回ID=" + data.id);
                                    window.location.href = "/ServiceItems/FormSetting/" + data.id;
                                }
                            }
                        });
                    } else {
                        if (jQuery('#ServiceItemID').length == 0) {
                            jQuery("#CheckCodeMsg").text(data.data);
                            jQuery('#ServiceCode').warning("已存在产品使用该编码");
                        } else {
                            jQuery("#CheckCodeMsg").text(data.data);
                            jQuery('#ServiceCode').warning("已存在其他产品使用该编码");
                        }
                        jQuery(_this).one('click', postData);
                    }
                },
                failed: function() {
                    jQuery(_this).one('click', postData);
                }
            })
        } else {
            jQuery(_this).one('click', postData);
        }
    });

    function getPost() {
        var post = true;

        if (jQuery('#tbExtraService tr td .ischeck').length != 0) {
            jQuery('#tbExtraService tr td .ischeck').warning("请先保存再提交");
            post = false;
        }
        var suppliers = [];
        jQuery('#supplierRight option').each(function() {
            var obj = { "SupplierID": jQuery(this).val() };
            suppliers.push(obj);
        });
        if (suppliers.length == 0) {
            jQuery('#supplierLeft').focus();
            jQuery('#supplierRight').warning("请选择供应商");
            post = false;
        }
        var extraServices = [];
        jQuery('#tbExtraService tbody tr.itemRow').each(function() {
            var extraService = {
                "ExtraServiceID": jQuery(this).find("td input.serviceID:eq(0)").val(),
                "ServiceName": jQuery(this).find("td.serviceName span").text(),
                "ServiceEnName": jQuery(this).find("td.serviceEnName span").text(),
                "ServiceUnit": jQuery(this).find("td.serviceUnit span").text(),
                "MinNum": jQuery(this).find("td.minNum span").text(),
                "MaxNum": jQuery(this).find("td.maxNum span").text(),
            }
            extraServices.push(extraService);
        });
        var serviceItem = {
            ServiceItemID: jQuery.trim(jQuery('#ServiceItemID').val()),
            ServiceItemEnableState: jQuery.trim(jQuery('#ServiceItemEnableState').val()),
            CreateTime: jQuery.trim(jQuery('#CreateTime').val()),
            ElementContent: jQuery.trim(jQuery('#ElementContent').val()),
            ServiceItemTemplteID: jQuery.trim(jQuery('#ServiceItemTemplteID').val()),

            ItemSuplier: suppliers,
            ExtraServices: extraServices,
            IsAutomaticDeliver: jQuery.trim(jQuery('.IsAutomaticDeliver[name=IsAutomaticDeliver]:checked').val()),
            CityID: (function() {
                var value = jQuery('#Cities').val();
                if (!value) {
                    jQuery('#Cities').warning("请选择");
                    post = false;
                }
                return value;
            })(),
            ServiceTypeID: (function() {
                var value = jQuery('#ServiceType input.ServiceType[name=ServiceType]:checked').val();
                if (!value) {
                    jQuery('#ServiceType').warning("请选择");
                    post = false;
                }
                return value;
            })(),
            ServiceCode: (function() {
                var value = jQuery.trim(jQuery('#ServiceCode').val());
                if (!value) {
                    jQuery('#ServiceCode').warning("请填写");
                    post = false;
                }
                return value;
            })(),
            cnItemName: (function() {
                var value = jQuery.trim(jQuery('#cnItemName').val())
                if (!value) {
                    jQuery('#cnItemName').warning("请填写");
                    post = false;
                }
                return value;
            })(),
            enItemName: jQuery.trim(jQuery('#enItemName').val()),
            TravelCompany: jQuery.trim(jQuery('#TravelCompany').val()),
            FixedDays: (function() {
                var value = jQuery.trim(jQuery('#FixedDays').val())
                if (jQuery('#ServiceType input.ServiceType[name=ServiceType]:checked').val() == 2) {
                    if (!value) {
                        jQuery('#FixedDays').warning("请填写");
                        post = false;
                    }
                } else {
                    value = 0;
                }
                if (!ex.test(value)) {
                    jQuery('#FixedDays').warning("只能填写整数");
                    post = false;
                }
                return value;
            })(),
            InsuranceDays: (function() {
                var value = jQuery.trim(jQuery('#InsuranceDays').val())
                if (!value) {
                    jQuery('#InsuranceDays').warning("请填写");
                    post = false;
                }
                if (!ex.test(value)) {
                    jQuery('#InsuranceDays').warning("只能填写整数");
                    post = false;
                }
                return value;
            })(),
            DefaultSupplierID: (function() {
                var value = 0;
                jQuery('#supplierRight option').each(function() {
                    var text = jQuery(this).text();

                    if (text.indexOf("(默认)") != -1) {
                        value = jQuery(this).val();
                    }
                });
                if (value == 0) {
                    jQuery('#supplierLeft').focus();
                    jQuery('#supplierRight').warning("请选择默认供应商");
                    post = false;
                }
                return value;
            })()
        };
        if (window.location.href.indexOf("Create") != -1) {
            var isAdd = true;
        } else {
            var isAdd = false;
        }
        return {
            post: post,
            item: { isAdd: isAdd, serviceItem: serviceItem }
        };
    }
});