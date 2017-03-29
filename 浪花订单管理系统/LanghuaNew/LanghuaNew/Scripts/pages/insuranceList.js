jQuery(document).ready(function() {

    $('#insuranceList').eq(0)
        .on('preXhr.dt', function(e, settings, json) {

            //var search = ($('#searchoption').data("search"));
            // 删除插件无必要项目
            //test
            delete json.columns;
            delete json.order;
            delete json.search;
            if ($('#searchoption').length == 0) {
                $('body').append("<div id='searchoption' class='hidden'></div>");
            }
            var search = $('#searchoption').data("search");
            //console.log(search)
            //if (!search)
            //{
            //    search = new Object();
            //    search.ServiceItemID = -1;
            //    search.ServiceCode = "-1";
            //    search.cnItemName = "全部";
            //    search.Insurance_Start_Date = jQuery("#insuranceStartDate").val();
            //    search.Insurance_Days = $('#travelDs').val();
            //    search.CountryID = $("#Country").val();
            //    search.CountryName = $("#Country  option:selected").text();
            //    search.InsuranceCode = $('#insuranceCode').text();
            //}
            //console.log(search)
            //test
            json['Search'] = search;

            $('#reflashTable').find('span').addClass("fa-spin");
        }).on('xhr.dt', function(e, settings, json, xhr) {
            $('#insuranceList thead tr th:eq(0) input').prop("checked", false)

            //syncState(json.SearchModel.InsuranceSearch);

            $('#insuranceStart').text($('#insuranceStartDate').val());
            $('#travelDays').text($('#travelDs').val());
            if ($('#ServiceItems') != undefined) {
                if ($('#ServiceItems').val() != "" && $('#ServiceItems').data('name') != undefined && $('#ServiceItems').data('code') != "-1") {
                    $("#serviceName").text($('#ServiceItems').data('code'));
                    $('#insuranceCode').text($('#insuranceStartDate').val().replace("-", "").replace("-", "") + "-" + $("#travelDs").val() + "-" + $('#ServiceItems').data('code'));
                } else {
                    $("#serviceName").text("全部符合条件的产品");
                    $('#insuranceCode').text($('#insuranceStartDate').val().replace("-", "").replace("-", "") + "-" + $("#travelDs").val() + "-" + "全部");
                }
            }
            if ($('#ServiceItems').val() != "" && $('#ServiceItems').data('which') != -1) {
                $('#insuranceCode').text($('#insuranceStartDate').val().replace("-", "").replace("-", "") + "-" + $("#travelDs").val() + "-" + $('#ServiceItems').data('code'));
            } else if ($("#Country").val() != "" && $("#Country  option:selected").text() != "" && $("#Country").val() != -1) {
                $('#insuranceCode').text($('#insuranceStartDate').val().replace("-", "").replace("-", "") + "-" + $("#travelDs").val() + "-" + $("#Country  option:selected").text());
            } else {
                $('#insuranceCode').text($('#insuranceStartDate').val().replace("-", "").replace("-", "") + "-" + $("#travelDs").val() + "-" + "全部");
            }

            $('#reflashTable').find('span').removeClass("fa-spin");

        })
    var newproduct =
        jQuery('table#insuranceList')
        .DataTable({
            ajax: {
                url: "/Insurance/GetInsurances",
                type: 'post',
            },
            ordering: false,
            searching: false,
            serverSide: true,
            initComplete: function(settings, json) {},
            //列操作
            columns: [{
                    'data': 'CustomerTBCode',
                    "createdCell": function(td, cellData, rowData, row, col) {
                        var search = {
                            FuzzySearch: cellData,
                            searchType: "fuzzy",
                        };
                        var searchStr = JSON.stringify(search).urlSwitch();

                        var link = '/Orders/Index?search=' + searchStr;
                        jQuery(td).html("<div><a target='_blank' href='" + link + "'>" + cellData + "</a></div>");
                    }
                },
                {
                    'data': 'TravellerName',
                    "createdCell": function(td, cellData, rowData, row, col) {
                        jQuery(td).html('<div>' + cellData + '</div><div class="mini">' + '</div>');
                    }
                },
                {
                    'data': 'PassportNo',
                    "createdCell": function(td, cellData, rowData, row, col) {
                        var reg = /(^\d{15}$)|(\d{18}$)|(^\d{17}(\d|X|x)$)/;
                        var reg2 = /(^[a-zA-Z]\d{8}$)/;
                        if (reg.test(cellData) === true) {
                            jQuery(td).css("background-color", "orange");
                            jQuery(td).html('<div">' + "居民身份证" + '</div>');
                        } else if (reg2.test(cellData) === true) {
                            jQuery(td).html('<div>' + "中国护照" + '</div>');
                        } else {
                            jQuery(td).css("background-color", "orange");
                            jQuery(td).html('<div>' + "其它证件" + '</div>');
                        }
                    }
                },
                {
                    'data': 'PassportNo',
                    "createdCell": function(td, cellData, rowData, row, col) {
                        jQuery(td).html('<div>' + cellData + '</div>');
                    }
                },
                {
                    'data': 'Birthday',
                    "createdCell": function(td, cellData, rowData, row, col) {
                        jQuery(td).html('<div>' + cellData.substring(0, 10) + '</div>');
                    }
                },
                {
                    'data': 'TravellerSex',
                    "createdCell": function(td, cellData, rowData, row, col) {
                        if (cellData == 0 || cellData == "0") {
                            jQuery(td).html('<div><span>男</span></div>');
                        } else {
                            jQuery(td).html('<div><span>女</span></div>');
                        }
                    }
                }
            ]
        });

    // 模糊搜索
    jQuery("#fuzzySearch").bind("click", function() {
        var fuzzyString = jQuery("#fuzzyString").val().trim();
        if ($('#searchoption').length == 0) {
            $('body').append("<div id='searchoption' class='hidden'></div>");
        }
        $('#searchoption').data("search", {
            FuzzySearch: fuzzyString
        });
        newproduct.draw();
    })



    jQuery('#insuranceStartDate').datepicker();
    //产品和国家点击事件
    $("#ServiceItems").bind('click', function() {
        $("#singleProductRadio").checked = true;
        $("#singleProductRadio").click();
        $("#Country").val(-1);
    });
    $("#Country").bind('click', function() {
        $("#contouryProductRadio").checked = true;
        $("#contouryProductRadio").click();
        $('#ServiceItems').val('');
    });
    //单选按钮点击逻辑
    $("#allProductRadio").bind('click', function() {
        $("#Country").val(-1);
        $('#ServiceItems').val('');
        $("#Country").attr("disabled", true);
        $('#ServiceItems').attr("disabled", true);
    });
    $("#singleProductRadio").bind('click', function() {
        $("#Country").val(-1);
        $("#Country").attr("disabled", true);
        $('#ServiceItems').attr("disabled", false);
    })
    $("#contouryProductRadio").bind('click', function() {
        $('#ServiceItems').val('');
        $("#Country").attr("disabled", false);
        $('#ServiceItems').attr("disabled", true);
    })


    // 筛选
    jQuery("#btnSelect").bind('click', function() {
        if (!jQuery("#insuranceStartDate").val()) {
            jQuery("#insuranceStartDate").warning("请选择日期");
        }

        //联动逻辑
        if ($('#ServiceItems') != undefined) {
            if ($('#ServiceItems').val() != "" && $('#ServiceItems').data('which') != undefined && $('#ServiceItems').data('which') != -1) {
                $("#serviceName").text($('#ServiceItems').data('code'));
                $('#insuranceCode').text($('#insuranceStartDate').val().replace("-", "").replace("-", "") + "-" + $("#travelDs").val() + "-" + $('#ServiceItems').data('code'));
            } else {
                $("#serviceName").text("全部符合条件的产品");
                $('#insuranceCode').text($('#insuranceStartDate').val().replace("-", "").replace("-", "") + "-" + $("#travelDs").val() + "-" + "全部");
            }
        }
        if ($('#ServiceItems').val() != "" && $('#ServiceItems').data('which') != undefined && $('#ServiceItems').data('which') != -1) {
            $('#insuranceCode').text($('#insuranceStartDate').val().replace("-", "").replace("-", "") + "-" + $("#travelDs").val() + "-" + $('#ServiceItems').data('code'));
        } else if ($("#Country").val() != "" && $("#Country  option:selected").text() != "" && $("#Country").val() != -1) {
            $('#insuranceCode').text($('#insuranceStartDate').val().replace("-", "").replace("-", "") + "-" + $("#travelDs").val() + "-" + $("#Country  option:selected").text());
        } else {
            $('#insuranceCode').text($('#insuranceStartDate').val().replace("-", "").replace("-", "") + "-" + $("#travelDs").val() + "-" + "全部");
        }
        ///设置查询条件
        var insuranceSearch = new Object();


        insuranceSearch.Insurance_Start_Date = jQuery("#insuranceStartDate").val();
        insuranceSearch.Insurance_Days = $('#travelDs').val();

        if ($("input[name='productsRadios']:checked").val() == "all") {
            insuranceSearch.ServiceItemID = -1;
            insuranceSearch.ServiceCode = "-1";
            insuranceSearch.cnItemName = "全部";
            insuranceSearch.CountryID = -1;
            insuranceSearch.CountryName = "国家";
        }
        if ($("input[name='productsRadios']:checked").val() == "country") {
            insuranceSearch.ServiceItemID = -1;
            insuranceSearch.ServiceCode = "-1";
            insuranceSearch.cnItemName = "全部";
            insuranceSearch.CountryID = $("#Country").val();
            insuranceSearch.CountryName = $("#Country  option:selected").text();
        }
        if ($("input[name='productsRadios']:checked").val() == "single") {
            insuranceSearch.ServiceItemID = $('#ServiceItems').data('which');
            insuranceSearch.ServiceCode = $('#ServiceItems').data('code');
            insuranceSearch.cnItemName = $('#ServiceItems').data('name');
            insuranceSearch.CountryID = -1;
            insuranceSearch.CountryName = "国家";
        }

        $('#searchoption').data("search", insuranceSearch);

        newproduct.draw();
        $("#insurance").modal('hide');
    })

    var remote_cities = new Bloodhound({
        datumTokenizer: function(d) {
            return Bloodhound.tokenizers.whitespace(d.name);
        },
        queryTokenizer: Bloodhound.tokenizers.whitespace,
        limit: 15,
        remote: {
            wildcard: '%QUERY',
            url: '/Orders/GetItemsByStr',
            prepare: function(xhr, settings) {
                settings.dataType = 'json';
                settings.type = 'POST';
                settings.data = { Str: xhr };
                return settings;
            },
            filter: function(data) {
                return $.map(data.Items, function(one) {
                    return {
                        name: one.cnItemName,
                        supplyer: one.ItemSupliers,
                        defaultSupplierID: one.DefaultSupplierID,
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
    }, {
        name: 'xxx',
        display: function(data) {
            return data.name //+" "+ data.code;
        },
        limit: 30,
        source: remote_cities,
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
        $(this).data('which', suggestion.serviceItemID);
        $(this).data('name', suggestion.name);
        $(this).data('code', suggestion.code);
    })

    //表格工具订单导出
    $('body').on('click', '#exportproducts', function(e) {
        var varURL = "/Insurance/ExportExcel?";
        var link = document.createElement("a");
        var search = $('#searchoption').data("search");
        if (search) {
            if (!(search instanceof Object)) {
                search = JSON.parse(search)
            } else {}
        } else {

        }
        for (var i in search) {
            var s = search[i].toString().replace(/\#/g, "%23");
            varURL += i + '=' + s + "&";
        }

        link.href = varURL;
        document.body.appendChild(link);
        link.click();
        delete link;
    })
    $('#reflashTable').bind("click", function() {
        newproduct.draw()
    });
})