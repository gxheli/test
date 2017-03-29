jQuery(document).ready(function() {
    var ex = /^\d+$/;

    jQuery('#launchdaterange').datepicker({
        autoclose: true,
        orientation: "",
        format: 'yyyy-mm-dd',
        startView: 0,
        startDate: '1921-01-01',
        todayHighlight: true,
        todayBtn: 'linked',
        clearBtn: true,
        title: ""
    });
    jQuery('#rulerange').datepicker({
        autoclose: true,
        orientation: "",
        format: 'yyyy-mm-dd',
        startView: 0,
        startDate: '1921-01-01',
        todayHighlight: true,
        todayBtn: 'linked',
        clearBtn: true,
        title: ""
    });

    var remote_cities = new Bloodhound({
        datumTokenizer: function(d) {
            return Bloodhound.tokenizers.whitespace(d.name);
        },
        queryTokenizer: Bloodhound.tokenizers.whitespace,
        limit: 15,
        // 在文本框输入字符时才发起请求
        // 
        // local:dt,
        remote: {
            wildcard: '%QUERY',
            url: '/ServiceItems/GetItemsByStr?Str=%QUERY',
            filter: function(data) {
                return $.map(data.data, function(one) {
                    return {
                        name: one.cnItemName,
                        serviceItemID: one.ServiceItemID,
                        code: one.ServiceCode,
                    };
                });
            }

        }
    });

    remote_cities.initialize();

    $('#ServiceItems').typeahead({
        hint: false,
        highlight: true,
        minLength: 1,
        classNames: {
            menu: "tt-menu tt-menu-top"
        }
    }, {
        name: 'xxx',
        limit: 30,
        source: remote_cities,
        displayKey: "name",
        templates: {
            empty: [
                '<div class="empty-message">',
                '没有找到相关数据',
                '</div>'
            ].join('\n'),
            header: function(data) {
                return ([
                    '<div class="empty-message text-left">',
                    '共搜索到<strong>' + data.suggestions.length + '</strong>个产品',
                    '</div>'
                ].join('\n'));
            },
            pending: [
                '<div class="empty-message">',
                '正在载入数据....',
                '</div>'
            ].join(''),
            suggestion: Handlebars.compile('<div class="text-left">{{name}}{{code}}</div>')
        }
    }).bind('typeahead:select', function(ev, suggestion) {
        $('#warningError').addClass("hidden");
        $('#selectedItem').data("itemid", suggestion['serviceItemID']);
        $('#selectedTips').removeClass("hidden");
        $('#which').text($('#ServiceItems').val());
    }).one("keypress", function(e) {
        $('#warningError').removeClass("hidden");
        $('#selectedTips').addClass("hidden");
        $('#warningError').text("您未选择产品");
    });

    $("#btnAdd").bind('click', function() {
        var ItemID = $('#selectedItem').data("itemid");
        if (!ItemID) {
            $('#ServiceItems').warning("请选择产品");
            return;
        }
        var bl = false;
        $("#tbServiceItem tbody tr.itemRow").each(function() {
            var oldid = jQuery(this).find("td #serviceid").text();
            if (oldid == ItemID) {
                $('#ServiceItems').warning("产品已重复");
                bl = true;
            }
        })
        if (bl) return;
        var count = jQuery("#tbServiceItem tbody tr").length;
        var trHtml = "<tr class='itemRow'><td>" + count + "</td>" +
            "<td>" + $('#ServiceItems').val() + "<span class='hidden' id='serviceid'>" + ItemID + "</td>" +
            "<td><a id='delete' class='btn btn-sm btn-default button70'>删除</a></td></tr>";
        jQuery("#tbServiceItem tr:last").before(trHtml);
        $('#selectedTips').addClass("hidden");
        $('#selectedItem').data("itemid", null);
        $('#ServiceItems').val(null);
        $('#ServiceItems').typeahead('val', '');
    })
    $("#tbServiceItem").on('click', "#delete", function() {
        $(this).closest("tr").remove();
    });



    $("#btnSave").one('click', function postData() {
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
                url: '/ServiceRules/SaveRule',
                success: function(data) {
                    if (data.ErrorCode == 200) {
                        window.location.href = "/ServiceRules/Index";
                    } else {
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
                failed: function() {
                    jQuery(_this).one('click', postData);
                }
            });
        } else {
            jQuery(_this).one('click', postData);
        }
    })

    function getPost() {
        var post = true;
        var RuleServiceItem = [];
        jQuery('#tbServiceItem tbody tr.itemRow').each(function() {
            var Item = {
                "ServiceItemID": jQuery(this).find("td #serviceid").text()
            }
            RuleServiceItem.push(Item);
        });
        if (RuleServiceItem.length == 0) {
            jQuery('#tbServiceItem').focus();
            jQuery('#tbServiceItem').warning("请添加使用产品");
            post = false;
        }

        var rule = {
            RuleServiceItem: RuleServiceItem,
            ServiceRuleID: jQuery.trim(jQuery('#ServiceRuleID').val()),
            UseState: jQuery.trim(jQuery('#UseState').val()),
            SupplierID: jQuery('#supplier').val(),
            StartTime: (function() {
                var value = jQuery('#StartTime').val();
                if (!value) {
                    jQuery('#rulerange').warning("请选择出行范围");
                    post = false;
                }
                return value;
            })(),
            EndTime: (function() {
                var value = jQuery("#EndTime").val();
                if (!value) {
                    jQuery('#rulerange').warning("请选择出行范围");
                    post = false;
                }
                return value;
            })(),
            RuleUseTypeValue: (function() {
                var value = jQuery(".RuleUseTypeValue[name=RuleUseTypeValue]:checked").val()
                if (!value) {
                    jQuery('#RuleUseTypeValue').warning("请选择");
                    post = false;
                }
                return value;
            })(),
            SelectRuleType: (function() {
                var value = jQuery(".SelectRuleType[name=SelectRuleType]:checked").val()
                if (!value) {
                    jQuery('#SelectRuleType').warning("请选择规则类型");
                    post = false;
                }
                return value;
            })(),
            RangeStart: (function() {
                var value = jQuery("#RangeStart").val()
                if (jQuery(".SelectRuleType[name=SelectRuleType]:checked").val() == 0) {
                    if (!value) {
                        jQuery('#launchdaterange').warning("请选择日期");
                        post = false;
                    } else {
                        if (value < jQuery('#StartTime').val() || value > jQuery("#EndTime").val()) {
                            jQuery('#launchdaterange').warning("日期请限制在出行范围之内");
                            post = false;
                        }
                    }
                }
                return value;
            })(),
            RangeEnd: (function() {
                var value = jQuery("#RangeEnd").val()
                if (jQuery(".SelectRuleType[name=SelectRuleType]:checked").val() == 0) {
                    if (!value) {
                        jQuery('#launchdaterange').warning("请选择日期");
                        post = false;
                    } else {
                        if (value < jQuery('#StartTime').val() || value > jQuery("#EndTime").val()) {
                            jQuery('#launchdaterange').warning("日期请限制在出行范围之内");
                            post = false;
                        }
                    }
                }
                return value;
            })(),
            Week: (function() {
                var value = "";
                jQuery(".Week[name=Week]:checked").each(function() {
                    if (value != "") value += "|";
                    value += jQuery(this).val();
                })
                if (jQuery(".SelectRuleType[name=SelectRuleType]:checked").val() == 1) {
                    if (value == "") {
                        jQuery('#Week').warning("请选择");
                        post = false;
                    }
                }
                return value;
            })(),
            IsDouble: (function() {
                var value = jQuery(".IsDouble[name=IsDouble]:checked").val()
                if (jQuery(".SelectRuleType[name=SelectRuleType]:checked").val() == 2) {
                    if (!value) {
                        jQuery('#IsDouble').warning("请选择");
                        post = false;
                    }
                }
                return value;
            })(),
            UseDate: (function() {
                var value = jQuery("#UseDate").val()
                if (jQuery(".SelectRuleType[name=SelectRuleType]:checked").val() == 3) {
                    if (!value) {
                        jQuery('#UseDate').warning("请填写");
                        post = false;
                    }
                    var vv = value.split('|');
                    for (var i in vv) {
                        if (!ex.test(vv[i])) {
                            jQuery('#UseDate').warning("只能填写数字和竖线|");
                            post = false;
                        }
                    }
                }
                return value;
            })(),
            Remark: jQuery.trim(jQuery('#Remark').val()),
        };
        return {
            post: post,
            item: rule
        }
    }
})