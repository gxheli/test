    'use strict';
    jQuery(document).ready(function($) {
        fix();
        var Distribution = tableInit($('.tabletools').eq(0));
        tabletoolInint(Distribution);
        var searchEngineCreate = bloodHound();
        var searchEngineEdit = bloodHound();
        search(searchEngineCreate, $('#distribution-create'));
        search(searchEngineEdit, $('#distribution-edit'));
        date();
        create(Distribution);
        todelete(Distribution);
        edit(Distribution);
    });

    function　 fix() {
        if (jQuery("#OrderBy").length === 0) {
            $('body').append('<div id="OrderBy"></div>');
        }
        if (jQuery("#searchoption").length === 0) {
            $('body').append('<div id="searchoption" ></div>');
        }
    }

    function tableInit(tabletools) {
        var distribution =
            jQuery('table#distribution')
            .on('preXhr.dt', function(e, settings, json) {
                delete json.columns;
                delete json.order;
                delete json.search;
                var OrderBy = jQuery("#OrderBy").data("OrderBy");
                var search = $('#searchoption').data("search");

                if (OrderBy) {
                    json.OrderBy = JSON.parse(OrderBy);
                }
                if (search) {
                    if (typeof(search) === "string") {
                        json.DistributionTallySearch = JSON.parse(search);
                    } else {
                        json.DistributionTallySearch = (search);
                    }
                    $('#TravelDate').val(search.TravelDateBegin);
                }
            })
            .on('preXhr.dt', function(e, settings, json, xhr) {})
            .DataTable({
                ajax: {
                    url: "/DistributionTallies/GetDistributionTallies",
                    type: 'post',
                },
                ordering: false,
                searching: false,
                serverSide: true,
                drawCallback: function(settings) {
                    var api = this.api();
                    var thistable = jQuery(this);
                    thistable.find("thead th .allrows").prop("checked", false).trigger("change");
                },
                initComplete: function(settings, json) {
                    var api = this.api();
                    var thistable = jQuery(this);
                    thistable.on('change', ".onerow", function() {
                        var count = thistable.find('.onerow:checked').length;
                        tabletools.find("#selectedNumber").text(count);
                        if ($(this).prop("checked")) {
                            $(this).closest('tr').addClass("selectedRow");
                        } else {
                            $(this).closest('tr').removeClass("selectedRow");
                        }
                        var _this = this;
                        var currentPageLength = api.page.info().end - api.page.info().start;
                        if (thistable.find("tbody tr td input:checked").length != currentPageLength) {
                            thistable.find(".allrows:eq(0)").prop("checked", false);
                        } else {
                            thistable.find(".allrows:eq(0)").prop("checked", true);
                        }
                    });
                    thistable.on('change', ".allrows", function() {
                        $(this).closest('table').find('tbody tr td input.checkboxes').prop('checked', $(this).prop("checked"));
                        if ($(this).prop("checked")) {
                            $(this).closest('table').find('tbody tr ').addClass("selectedRow");
                        } else {
                            $(this).closest('table').find('tbody tr ').removeClass("selectedRow");
                        }
                        var count = thistable.find('tbody tr td input.checkboxes:checked').length;
                        tabletools.eq(0).find("#selectedNumber").text(count);
                    });
                },
                //行操作
                rowId: "DistributionTallyID",
                createdRow: function(row, data, dataIndex) {
                    if ((data.IsCancel)) {
                        $(row).css("background", '#BCBCBC');
                    }
                    var _this = this.api();
                    $(row).data('distribution', data);
                },
                //列操作
                columnDefs: [{
                        'targets': [0],
                        'data': null,
                        'render': function(cellData, type, rowData, meta) {
                            return (
                                '<input type="checkbox" class="checkboxes onerow">'
                            );
                        }
                    },
                    {
                        'targets': [1],
                        'data': 'cnItemName',
                        'render': function(cellData, type, rowData, meta) {
                            return ('<span>' + cellData + '</span><a>' + rowData.ServiceCode + '</a>');
                        }
                    },
                    {
                        'targets': [2],
                        'data': 'SupplierNo',
                    },
                    {
                        'targets': [3],
                        'data': null,
                        'render': function(cellData, type, rowData, meta) {
                            var str = '';
                            if (rowData.ServiceTypeID == 4) {
                                str += rowData.RoomNum + '间' + ' / ' + rowData.RightNum + '晚';
                            } else {
                                str += rowData.AdultNum + ' / ' + rowData.ChildNum + ' / ' + rowData.INFNum;
                            }
                            return (str);
                        }
                    },
                    {
                        'targets': [4],
                        'data': 'TravelDate'
                    },
                    {
                        'targets': [5],
                        'data': 'GroupNo'
                    },
                    {
                        'targets': [6],
                        'data': 'CreateTime',
                        'render': function(cellData, type, rowData, meta) {
                            return ('<div>' + cellData + '</div>' + '<div>' + rowData.CreateUserNikeName + '</div>');
                        }
                    },
                    {
                        'targets': [7],
                        'data': null,
                        'render': function(cellData, type, rowData, meta) {
                            var str = '';
                            if (rowData.IsCancel) {
                                str += '<a act="NotCancel" class="hrefInTable-inline distributionCancelDelete"   >不取消</a>';
                            } else {
                                str += '<a act ="Cancel" class="hrefInTable-inline distributionCancelDelete"   >取消</a>';
                            }
                            return (
                                '<div class="row">' +
                                str +
                                '<a class="hrefInTable-inline distributionEdit"   >修改</a>' +
                                '<a act="delete" class="hrefInTable-inline distributionCancelDelete" >删除</a>' +
                                '</div>'
                            );
                        }
                    }
                ]
            });
        return {
            'dataTableRef': distribution,
            'jQueryRef': jQuery('table#distribution')
        };
    }

    function　 tabletoolInint(Refs) {
        $('.tabletools:eq(0)').find('#TravelDate').datepicker({});
        $('.tabletools:eq(0)').find('#search').one("click", function search(e) {
            var TravelDate = $(this).siblings('#fuzzyall').find("#TravelDate").val();
            var FuzzySearch = $(this).siblings('#fuzzyall').find('#FuzzySearch').val();
            var DistributionTallySearch = {
                "TravelDateBegin": TravelDate,
                "FuzzySearch": FuzzySearch
            };
            console.log(DistributionTallySearch);
            $('#searchoption').data('search', JSON.stringify(DistributionTallySearch));
            console.log($('#searchoption').data('search'));
            Refs.dataTableRef.draw();
            $(this).one("click", search);
        });
        $('.tabletools:eq(0)').find('#deleteall').one("click", { "table": Refs.jQueryRef }, function deleteall(e) {
            var _this = $(this);
            var table = e.data.table;
            var arr = [];
            table.find("tbody tr input:checked").each(function() {
                arr.push($(this).closest("tr").attr("id"));
            });
            var name = '所选的' + arr.length + "项";
            var post = {};
            post.DistributionTallyID = arr.join(",");
            post.Operation = "delete";
            $.LangHua.confirm({
                title: "提示信息",
                tip1: '您确定要删除',
                tip2: name,
                confirmbutton: '确定',
                cancelbutton: '取消',
                data: post,
                confirm: function() {
                    $.ajax({
                        url: '/DistributionTallies/UpdateDisable',
                        type: 'post',
                        contentType: "application/json; charset=utf-8;",
                        data: JSON.stringify(post),
                        dataType: 'json',
                        success: function(data) {
                            if (data.ErrorCode == 200) {
                                $.LangHua.alert({
                                    tip1: '删除结果',
                                    tip2: '删除成功',
                                    button: '确定',
                                    icon: "warning",
                                    callback: function() {
                                        Refs.dataTableRef.draw();
                                    }
                                });
                            } else if (data.ErrorCode == 401) {
                                $.LangHua.alert({
                                    tip1: '删除失败',
                                    tip2: data.ErrorMessage,
                                    button: '确定',
                                    icon: "warning"
                                });
                            } else {
                                $.LangHua.alert({
                                    tip1: '删除失败',
                                    tip2: "删除失败，请重试！",
                                    button: '确定',
                                    icon: "warning"
                                });
                            }
                        },
                        error: function() {
                            $.LangHua.alert({
                                tip1: '删除失败',
                                tip2: "删除失败，请重试!!",
                                button: '确定',
                                icon: "warning"
                            });
                        },
                        complete: function() {
                            _this.one('click', { "table": Refs.jQueryRef }, deleteall);
                        }
                    });
                }
            });
        });

    }

    function search(searchEngine, which) {

        var thisModal = which;
        thisModal.find('#ItemID').typeahead({
            hint: false,
            highlight: true,
            minLength: 1,
        }, {
            name: 'xxx',
            displayKey: 'name',
            limit: 30,
            source: searchEngine,
            templates: {
                empty: [
                    '<div class="empty-message">',
                    '没有找到相关产品',
                    '</div>'
                ].join('\n'),
                pending: [
                    '<div class="empty-message">',
                    '正在搜索...',
                    '</div>'
                ].join('\n'),
                header: function(data) {
                    return ([
                        '<div class="empty-message">',
                        '共搜索到<strong>' + data.suggestions.length + '</strong>个产品',
                        '</div>'
                    ].join('\n'));
                },
                suggestion: Handlebars.compile('<div id="serviceItemID{{serviceItemID}}">{{name}}{{serviceCode}}</div>')
            }
        }).bind('typeahead:select', function(ev, suggestion) {
            $(this).data('which', suggestion.serviceItemID);
            // 供应商更改
            var supplier = suggestion.supplyer;
            var optGroupStr = '';
            for (var i in supplier) {
                optGroupStr +=
                    makeOneOption(supplier[i], suggestion.defaultSupplierID);
            }
            thisModal.find('#SupplierID').empty().append(optGroupStr).trigger("click");
            if (suggestion.serviceTypeID == 4) {
                $(this).closest(".form-body").find("#RoomNum, #RightNum").show().removeClass('notusing');
            } else {
                $(this).closest(".form-body").find("#RoomNum, #RightNum").hide().addClass("notusing");
            }

            function makeOneOption(obj, defaultSupplierID) {
                var selected = '';
                var altName = '';
                if (obj.SupplierID == defaultSupplierID) {
                    selected = 'selected="selected"';
                    altName = obj.SupplierNo + '-' + obj.SupplierName + "(默认)";
                } else {
                    selected = '';
                    altName = obj.SupplierNo + '-' + obj.SupplierName;
                }
                var str = "";
                str = '<option value="' + obj.SupplierID + '"    ' + selected + '>' + altName + '</option>';
                return str;
            }
        }).bind("keydown", function(evt) {
            evt = (evt) ? evt : ((window.event) ? window.event : "") //兼容IE和Firefox获得keyBoardEvent对象  
            var key = evt.keyCode ? evt.keyCode : evt.which; //兼容IE和Firefox获得keyBoardEvent对象的键值  
            if (key !== 13) {
                if (($(this).data('which'))) {
                    thisModal.find('#SupplierID').empty().text("");
                    $(this).data('which', "");
                    $(this).closest(".form-body").find("#RoomNum, #RightNum").hide().addClass('notusing');

                }
            }
        });





    }



    function date() {
        var thisModal = $('#distribution-create');
        thisModal.find('#TravelDate').datepicker({});
        thisModal.find('#AdultNum').onlyNum();
        thisModal.find('#ChildNum').onlyNum();
        thisModal.find('#INFNum').onlyNum();
        thisModal.find('#RoomNum').onlyNum();
        thisModal.find('#RightNum').onlyNum();
        var thatModal = $('#distribution-edit');
        thatModal.find('#TravelDate').datepicker({});
        thatModal.find('#AdultNum').onlyNum();
        thatModal.find('#ChildNum').onlyNum();
        thatModal.find('#INFNum').onlyNum();
        thatModal.find('#RoomNum').onlyNum();
        thatModal.find('#RightNum').onlyNum();

    }

    function create(Refs) {
        $('#distribution-create').find('#create').one('click', function creating(e) {
            var _this = $(this);
            var thisModal = $(this).closest('#distribution-create');
            var post = {};
            post.ItemID = thisModal.find("#ItemID").data('which');
            post.SupplierID = thisModal.find("#SupplierID").val();
            post.TravelDate = thisModal.find("#TravelDate").val();
            post.GroupNo = thisModal.find("#GroupNo").val();
            post.AdultNum = thisModal.find("#AdultNum").val() ? thisModal.find("#AdultNum").val() : 0;
            post.ChildNum = thisModal.find("#ChildNum").val() ? thisModal.find("#ChildNum").val() : 0;
            post.INFNum = thisModal.find("#INFNum").val() ? thisModal.find("#INFNum").val() : 0;
            post.RoomNum = thisModal.find("#RoomNum").val() ? parseInt(thisModal.find("#RoomNum").val()) : 0;
            post.RightNum = thisModal.find("#RightNum").val() ? parseInt(thisModal.find("#RightNum").val()) : 0;
            var postable = true;
            if (!post.ItemID) {
                thisModal.find("#ItemID").formWarning({
                    tips: "请搜索选择产品"
                });
                postable = false;
            }
            if (!post.SupplierID) {
                thisModal.find("#SupplierID").formWarning({
                    tips: "请选择相应的供应商"
                });
                postable = false;
            }
            if (!post.TravelDate) {
                thisModal.find("#TravelDate").formWarning({
                    tips: "请填写出行日期"
                });
                postable = false;
            }
            if (!post.GroupNo) {
                thisModal.find("#GroupNo").formWarning({
                    tips: "请填写团号"
                });
                postable = false;
            }
            if (parseInt(post.AdultNum) + parseInt(post.ChildNum) + parseInt(post.INFNum) > 0) {} else {
                thisModal.find("#AdultNum,#ChildNum,#INFNum").formWarning({
                    tips: "成人+儿童+婴儿人数应大于0"
                });
                postable = false;
            }
            if (thisModal.find("#RoomNum").hasClass("notusing")) {
                post.RoomNum = 0;
                post.RightNum = 0;
            } else {
                if ((post.RoomNum) * (post.RightNum) <= 0) {
                    thisModal.find("#RoomNum,#RightNum").formWarning({
                        tips: "间数 × 晚数 应大于0"
                    });
                    postable = false;
                }
            }
            if (!postable) {
                _this.one('click', creating);
                return;
            }
            if (postable) {
                $.ajax({
                    url: '/DistributionTallies/Create',
                    type: 'post',
                    contentType: "application/json; charset=utf-8;",
                    data: JSON.stringify(post),
                    dataType: 'json',
                    success: function(data) {
                        if (data.ErrorCode == 200) {
                            $.LangHua.alert({
                                tip1: '保存结果',
                                tip2: '保存成功',
                                button: '确定',
                                icon: "warning",
                                callback: function() {
                                    Refs.dataTableRef.draw();
                                    thisModal.modal("hide");
                                }
                            });
                        } else if (data.ErrorCode == 401) {
                            $.LangHua.alert({
                                tip1: '保存失败',
                                tip2: data.ErrorMessage,
                                button: '确定',
                                icon: "warning"

                            });
                        } else {
                            $.LangHua.alert({
                                tip1: '保存失败',
                                tip2: "保存失败，请重试！",
                                button: '确定',
                                icon: "warning"
                            });
                        }
                    },
                    error: function() {
                        $.LangHua.alert({
                            tip1: '保存失败',
                            tip2: "保存失败，请重试！",
                            button: '确定',
                            icon: "warning"
                        });
                    },
                    complete: function() {
                        _this.one('click', creating);
                    }
                });
            }
        });

        //
        $('body').on("shown.bs.modal", '#distribution-create', function(e) {
            var thisModal = $(this);
            thisModal.find("#ItemID").typeahead('val', "");
            thisModal.find("#ItemID").data('which', "");
            thisModal.find("#TravelDate").datepicker('setDate', "");
            thisModal.find('#SupplierID').val("").empty().text("");
            thisModal.find("#GroupNo").val("");
            thisModal.find("#AdultNum").val("");
            thisModal.find("#ChildNum").val('');
            thisModal.find("#INFNum").val('');
            thisModal.find("#RoomNum").val('');
            thisModal.find("#RightNum").val('');
            thisModal.find("#RoomNum, #RightNum").hide().addClass("notusing");
            thisModal.find('.tips').each(function() {
                $(this).text("").removeClass("tips");
            });
        });
    }

    function todelete(Refs) {
        var table = Refs.jQueryRef;
        table.on('click', '.distributionCancelDelete', function todeleting() {
            var _this = $(this);
            var arr = [];
            arr.push(_this.closest("tr").attr("id"));
            var name = _this.closest("tr").find("td:eq(1)").html();
            var post = {};
            post.DistributionTallyID = arr.join(",");
            post.Operation = _this.attr("act");
            if (post.Operation == 'delete') {
                $.LangHua.confirm({
                    title: "提示信息",
                    tip1: '您确定要删除',
                    tip2: name,
                    confirmbutton: '确定',
                    cancelbutton: '取消',
                    data: post,
                    confirm: function() {
                        canceldelete(post, Refs, false);
                    }
                });
            } else {
                canceldelete(post, Refs, true);
            }

        });

    }

    function canceldelete(post, Refs, isCancel) {
        $.ajax({
            url: '/DistributionTallies/UpdateDisable',
            type: 'post',
            contentType: "application/json; charset=utf-8;",
            data: JSON.stringify(post),
            dataType: 'json',
            success: function(data) {
                var op = ""
                if (isCancel) {
                    op = '更改';
                } else {
                    op = '删除';
                }
                if (data.ErrorCode == 200) {
                    if (isCancel) {
                        Refs.dataTableRef.draw(false);
                    } else {
                        $.LangHua.alert({
                            tip1: '删除结果',
                            tip2: '删除成功',
                            button: '确定',
                            icon: "warning",
                            callback: function() {
                                Refs.dataTableRef.draw(true);
                            }
                        });
                    }
                } else if (data.ErrorCode == 401) {

                    $.LangHua.alert({
                        tip1: op + '失败',
                        tip2: data.ErrorMessage,
                        button: '确定',
                        icon: "warning"
                    });
                } else {
                    $.LangHua.alert({
                        tip1: op + '失败',
                        tip2: op + "失败，请重试！",
                        button: '确定',
                        icon: "warning"
                    });
                }
            },
            error: function() {
                $.LangHua.alert({
                    tip1: op + '失败',
                    tip2: op + "失败，请重试!!",
                    button: '确定',
                    icon: "warning"
                });
            },
        });
    }

    function edit(Refs) {
        var thisModal = $('#distribution-edit');
        var table = Refs.jQueryRef;
        table.on('click', '.distributionEdit', function() {
            var oldData = $(this).closest("tr").data('distribution');
            if (oldData.serviceTypeID == 4) {
                $(this).closest(".form-body").find("#RoomNum, #RightNum").show().removeClass('notusing');
            } else {
                $(this).closest(".form-body").find("#RoomNum, #RightNum").hide().addClass("notusing");
            }
            thisModal.find("#ItemID").one('typeahead:render', function(jq, menu, flag, xx) {
                var int = window.setInterval(function() {
                    if (thisModal.find('.tt-menu #serviceItemID' + oldData.ServiceItemID).length !== 0) {
                        thisModal.find('.tt-menu #serviceItemID' + oldData.ServiceItemID).trigger("click");
                        clearInterval(int);
                    }
                }, 100);
            });
            thisModal.find('.tips').each(function() {
                $(this).text("").removeClass("tips");
            });
            thisModal.find("#DistributionTallyID").text($(this).closest("tr").attr('id'));
            thisModal.find("#ItemID").data('which', oldData.ServiceItemID);
            thisModal.find("#ItemID").typeahead('val', oldData.cnItemName);
            thisModal.find("#SupplierID").val(oldData.SupplierID);
            thisModal.find("#TravelDate").val(oldData.TravelDate);
            thisModal.find("#GroupNo").val(oldData.GroupNo);
            thisModal.find("#AdultNum").val(oldData.AdultNum);
            thisModal.find("#ChildNum").val(oldData.ChildNum);
            thisModal.find("#INFNum").val(oldData.INFNum);
            thisModal.find("#RoomNum").val(oldData.RoomNum);
            thisModal.find("#RightNum").val(oldData.RightNum);
            thisModal.modal({});

        });
        thisModal.on('hide.bs.modal', function() {
            thisModal.find("#ItemID").typeahead('val', '');
        });
        thisModal.find('#edit').one('click', function editing(e) {
            var thisModal = $('#distribution-edit');
            var _this = $(this);
            var post = {};
            post.DistributionTallyID = thisModal.find("#DistributionTallyID").text();
            post.ItemID = thisModal.find("#ItemID").data('which');
            post.SupplierID = thisModal.find("#SupplierID").val();
            post.TravelDate = thisModal.find("#TravelDate").val();
            post.GroupNo = thisModal.find("#GroupNo").val();
            post.AdultNum = thisModal.find("#AdultNum").val() ? thisModal.find("#AdultNum").val() : 0;
            post.ChildNum = thisModal.find("#ChildNum").val() ? thisModal.find("#ChildNum").val() : 0;
            post.INFNum = thisModal.find("#INFNum").val() ? thisModal.find("#INFNum").val() : 0;
            post.RoomNum = thisModal.find("#RoomNum").val() ? parseInt(thisModal.find("#RoomNum").val()) : 0;
            post.RightNum = thisModal.find("#RightNum").val() ? parseInt(thisModal.find("#RightNum").val()) : 0;
            console.log(post)
            var postable = true;
            if (!post.ItemID) {
                thisModal.find("#ItemID").formWarning({
                    tips: "请搜索选择产品"
                });
                postable = false;
            }
            if (!post.SupplierID) {
                thisModal.find("#SupplierID").formWarning({
                    tips: "请选择相应的供应商"
                });
                postable = false;
            }
            if (!post.TravelDate) {
                thisModal.find("#TravelDate").formWarning({
                    tips: "请填写出行日期"
                });
                postable = false;
            }
            if (!post.GroupNo) {
                thisModal.find("#GroupNo").formWarning({
                    tips: "请填写团号"
                });
                postable = false;
            }
            if (parseInt(post.AdultNum) + parseInt(post.ChildNum) + parseInt(post.INFNum) > 0) {


            } else {
                thisModal.find("#AdultNum,#ChildNum,#INFNum").formWarning({
                    tips: "成人+儿童+婴儿人数应大于0"
                });
                postable = false;
            }
            if (thisModal.find("#RoomNum").hasClass("notusing")) {
                post.RoomNum = 0;
                post.RightNum = 0;
            } else {
                if ((post.RoomNum) * (post.RightNum) <= 0) {
                    thisModal.find("#RoomNum,#RightNum").formWarning({
                        tips: "间数 × 晚数 应大于0"
                    });
                    postable = false;
                }
            }
            if (!postable) {
                _this.one('click', editing);
                return;
            }
            if (postable) {
                $.ajax({
                    url: '/DistributionTallies/Edit',
                    type: 'post',
                    contentType: "application/json; charset=utf-8;",
                    data: JSON.stringify(post),
                    dataType: 'json',
                    success: function(data) {
                        if (data.ErrorCode == 200) {
                            $.LangHua.alert({
                                tip1: '保存结果',
                                tip2: '保存成功',
                                button: '确定',
                                icon: "warning",
                                callback: function() {
                                    Refs.dataTableRef.draw();
                                    thisModal.modal("hide");
                                }
                            });
                        } else if (data.ErrorCode == 401) {
                            $.LangHua.alert({
                                tip1: '保存失败',
                                tip2: data.ErrorMessage,
                                button: '确定',
                                icon: "warning"

                            });
                        } else {
                            $.LangHua.alert({
                                tip1: '保存失败',
                                tip2: "保存失败，请重试！",
                                button: '确定',
                                icon: "warning"
                            });
                        }
                    },
                    error: function() {
                        $.LangHua.alert({
                            tip1: '保存失败',
                            tip2: "保存失败，请重试！",
                            button: '确定',
                            icon: "warning"
                        });
                    },
                    complete: function() {
                        _this.one('click', editing);
                    }
                });
            }
        });

    }


    function bloodHound() {
        var searchEngine = new Bloodhound({
            datumTokenizer: function(d) {
                return Bloodhound.tokenizers.whitespace(d.name);
            },
            queryTokenizer: Bloodhound.tokenizers.whitespace,
            limit: 15,
            remote: {
                initialize: false,
                wildcard: '%QUERY',
                url: '/Orders/GetItemsByStr',
                prepare: function (xhr, settings) {
                    settings.dataType = 'json';
                    settings.type = 'POST';
                    settings.data = { Str: xhr };
                    return settings;
                },
                filter: function(data) {
                    return $.map(data.Items, function(one) {
                        return {
                            name: one.cnItemName,
                            enName: one.enItemName,
                            supplyer: one.ItemSupliers,
                            defaultSupplierID: one.DefaultSupplierID,
                            serviceItemID: one.ServiceItemID,
                            serviceCode: one.ServiceCode,
                            serviceTypeID: one.ServiceTypeID
                        };
                    });
                }
            }
        });
        searchEngine.initialize();
        return searchEngine;
    }