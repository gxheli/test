    'use strict';
    jQuery(document).ready(function($) {
        fix();
        var registerCancel = tableInit($('.tabletools').eq(0));
        tabletoolInint(registerCancel);
        var searchResult = search();
        step();
        date();
        create(registerCancel, searchResult);
        todelete(registerCancel);
        edit(registerCancel);
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
        var registerCancel =
            jQuery('table#registerCancel')
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
                    json.CancelRegisterSearch = JSON.parse(search);
                }
            })
            .on('preXhr.dt', function(e, settings, json, xhr) {

            })
            .DataTable({
                ajax: {
                    url: "CancelRegisters/GetCancelRegisters",
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
                rowId: "CancelRegisterID",
                createdRow: function(row, data, dataIndex) {
                    var _this = this.api();
                    $(row).data('cancelregister', data);
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
                        'data': 'StartDate',
                        'render': function(cellData, type, rowData, meta) {
                            if (rowData.StartDate == rowData.EndDate) {
                                return (rowData.StartDate);
                            } else {
                                return (rowData.StartDate + '至' + rowData.EndDate);
                            }
                        }
                    },
                    {
                        'targets': [4],
                        'data': 'Remark'
                    },
                    {
                        'targets': [5],
                        'data': 'CreateTime',
                        'render': function(cellData, type, rowData, meta) {
                            return ('<div>' + cellData + '</div>' + '<div>' + rowData.CreateUserNikeName + '</div>');
                        }
                    },
                    {
                        'targets': [6],
                        'data': null,
                        'render': function (cellData, type, rowData, meta) {
                            if ($('#isSave').val() == 'true') {
                                return (
                                    '<div class="row">' +
                                    '<a class="hrefInTable-inline registerEdit"   >修改</a>' +
                                    '<a class="hrefInTable-inline registerDelete" >删除</a>' +
                                    '<a class="hrefInTable-inline" target="_blank" href="/CancelRegisters/CancelRegisterOperation/' + rowData.CancelRegisterID + '">日志</a>' +
                                    '</div>'
                                );
                            }
                            else {
                                return '';
                            }
                        }
                    }
                ]

            });
        return {
            'dataTableRef': registerCancel,
            'jQueryRef': jQuery('table#registerCancel')
        }
    }

    function　 tabletoolInint(Refs) {

        $('.tabletools:eq(0)').find('#daterange').datepicker({
            inputs: $('#daterange').find('input')
        });
        $('.tabletools:eq(0)').find('#search').one("click", function search(e) {
            var SupplierID = $(this).siblings('#supplier').find("option:selected").val();
            var BeginDate = $(this).siblings('#daterange').find("#BeginDate").val();
            var EndDate = $(this).siblings('#daterange ').find("#EndDate").val();
            var CancelRegisterSearch = {
                "SupplierID": SupplierID,
                "BeginDate": BeginDate,
                "EndDate": EndDate
            };
            $('#searchoption').data('search', JSON.stringify(CancelRegisterSearch));
            Refs.dataTableRef.draw();
            $(this).one('click', search);
        });
        $('.tabletools:eq(0)').find('#deleteall').one("click", { "table": Refs.jQueryRef }, function deleteall(e) {
            var _this = $(this);
            var table = e.data.table;
            var arr = [];
            table.find("tbody tr input:checked").each(function() {
                arr.push($(this).closest("tr").attr("id"));
            });
            var name = '所选的' + arr.length + "个项目";
            var post = {};
            post.CancelRegisterID = arr.join(",");
            $.LangHua.confirm({
                title: "提示信息",
                tip1: '您确定要删除',
                tip2: name,
                confirmbutton: '确定',
                cancelbutton: '取消',
                data: post,
                confirm: function() {
                    $.ajax({
                        url: '/CancelRegisters/Delete',
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

    function search() {
        var thisModal = $('#register-cancel-create');
        var table = thisModal.find('#seachrResult').DataTable({
            "dom": "<t>",
            'stateSave': false,
            'destroy': true,
            'pageLength': 1000,
            'ordering': false,
            'searching': false,
            'serverSide': false,
            'initComplete': function(settings, json) {
                var api = this.api();
                var this_table = jQuery(this);
                this_table.on('change', ".onerow", function() {
                    var count = this_table.find('.onerow:checked').length;
                    this_table.closest('#register-cancel-create').find("#selectedNumber").text(count);
                    if ($(this).prop("checked")) {
                        $(this).closest('tr').addClass("selectedRow");
                    } else {
                        $(this).closest('tr').removeClass("selectedRow");
                    }
                    var _this = this;
                    var currentPageLength = api.page.info().end - api.page.info().start;
                    if (this_table.find("tbody tr td input:checked").length != currentPageLength) {
                        this_table.find(".allrows:eq(0)").prop("checked", false);
                    } else {
                        this_table.find(".allrows:eq(0)").prop("checked", true);
                    }
                });
                this_table.on('change', ".allrows", function() {
                    $(this).closest('table').find('tbody tr td input.checkboxes').prop('checked', $(this).prop("checked"));
                    if ($(this).prop("checked")) {
                        $(this).closest('table').find('tbody tr ').addClass("selectedRow");
                    } else {
                        $(this).closest('table').find('tbody tr ').removeClass("selectedRow");
                    }
                    var count = this_table.find('tbody tr td input.checkboxes:checked').length;
                    this_table.closest('#register-cancel-create').find("#selectedNumber").text(count);
                });
            },
            'rowId': "ServiceItemID",
            'columnDefs': [{
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
                    'data': 'ServiceCode',
                }
            ]
        });
        thisModal.find('#supplierChoose').bind('change', { 'result': table, "jqTableRef": thisModal.find('#seachrResult') }, function(e) {
            e.data.result.clear().draw();
            e.data.jqTableRef.find(".allrows").prop("checked", false).trigger("change");
        });
        thisModal.find('#clickToSearch').bind('click', function() {
            var SupplierID = parseInt(thisModal.find("#supplierChoose").val());
            var Str = thisModal.find('#serviceItemSearch').val().urlSwitch();
            if ((SupplierID !== 0)) {
                getDataOfOneSellControl(thisModal, SupplierID, Str);
            } else {
                thisModal.find('#supplierChoose').formWarning({
                    tips: "请选择供应商"
                });
            }
        });
        thisModal.find('#seachrResult').on("Update", function(e, data) {
            table.rows().remove();
            table.rows.add(data).draw();
            thisModal.find("input.allrows:eq(0)").prop("checked", false);
            thisModal.find("input.allrows:eq(0)").trigger("change");
        });


        function getDataOfOneSellControl(thisModal, SupplierID, Str) {
            $.ajax({
                url: '/CancelRegisters/GetItems?SupplierID=' + SupplierID + "&Str=" + Str,
                type: 'get',
                dataType: "json",
                success: function(data) {
                    if (data.ErrorCode == 200) {
                        if (data.data.length === 0) {
                            $.LangHua.alert({
                                tip1: "搜索结果",
                                tip2: "该供应商能搜索到的产品数为 0！"
                            })
                        }
                        thisModal.find('#seachrResult').trigger("Update", [data.data]);
                    }
                },
            });
        }
        return {
            'result': table
        };
    }

    function step() {
        var thisModal = $('#register-cancel-create');
        thisModal.on('click', '.stepclick', function() {
            var which = $(this).attr('which');
            var selectedNumber = thisModal.find("table#seachrResult tbody tr.selectedRow").length;
            if (selectedNumber !== 0 || (which == 'step1')) {
                thisModal.find('.step:not(.' + which + ')').addClass('hidden');
                thisModal.find('.step.' + which).removeClass('hidden');
            };
        });
    }

    function date() {
        var thisModal = $('#register-cancel-create');
        thisModal.find('#dateRange').datepicker({
            inputs: $('#register-cancel-create #dateRange input')
        });
        var thatModal = $('#register-cancel-edit');
        thatModal.find('#dateRange').datepicker({
            inputs: $('#register-cancel-edit #dateRange input')
        });

    }

    function create(Refs, searchResult) {
        var thisModal = $('#register-cancel-create');
        thisModal.find('#create').one('click', function creating(e) {
            var _this = $(this);
            var thisM = $(this).closest('#register-cancel-create');
            var post = {}
            post.BeginDate = thisM.find("#BeginDate").val();
            post.EndDate = thisM.find("#EndDate").val();
            post.Remark = thisM.find("#Remark").val();
            var postable = true;
            if (!post.BeginDate) {
                thisM.find("#BeginDate, #EndDate").formWarning({
                    tips: "请填写日期"
                });
                postable = false;
            }
            if (!post.EndDate) {
                thisM.find("#BeginDate, #EndDate").formWarning({
                    tips: "请填写日期"
                });
                postable = false;
            }
            if (!post.Remark) {
                thisM.find("#Remark").formWarning({
                    tips: "请填写原因"
                });
                postable = false;
            }
            if (!postable) {
                _this.one('click', creating);
                return;
            }
            post.SupplierID = thisModal.find('#supplierChoose').val();
            post.SupplierID = thisModal.find('#supplierChoose').val();
            var array = [];
            console.log(thisModal.find("table#seachrResult tbody tr.selectedRow"));
            thisModal.find("table#seachrResult tbody tr.selectedRow").each(function() {
                console.log($(this).attr('id'))
                array.push($(this).attr('id'));
            });
            post.ItemID = array.join(",");
            $.ajax({
                url: '/CancelRegisters/Create',
                type: 'post',
                contentType: "application/json; charset=utf-8;",
                data: JSON.stringify(post),
                dataType: 'json',
                success: function(data) {
                    if (data.ErrorCode == 200) {
                        if (data.failed.length === 0) {
                            $.LangHua.alert({
                                tip1: '保存结果',
                                tip2: '保存成功',
                                button: '确定',
                                icon: "warning",
                                callback: function() {
                                    Refs.dataTableRef.draw();
                                    thisM.modal("hide");
                                }
                            });
                        } else {
                            var failed = data.failed;
                            var str = '';
                            for (var i in failed) {
                                var arr = [
                                    '<div style="margin:10px 0px">',
                                    '<span style="color:#0099cc">' + failed[i]['cnItemName'] + '：</span>',
                                    '<span >' + '保存失败' + '，</span>',
                                    '<span style="color:#333" >' + failed[i]['reason'] + '</span>',
                                    '</div>',
                                ].join('\n');
                                str += arr;
                            }
                            $.LangHua.showResult({
                                title: "保存结果",
                                content: str,
                                confirmbutton: '确定',
                                data: null

                            });
                            Refs.dataTableRef.draw();
                        }
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
        });
        $('body').on("shown.bs.modal", '#register-cancel-create', { 'thattable': searchResult.result, 'jqTableRef': thisModal.find('#seachrResult') }, function(e) {
            var selfModal = $(this);
            var thatTable = e.data.thattable;
            var jqTable = e.data.jqTableRef;
            var thisModal = e.data.thisModal;
            selfModal.find(".tips").text("").removeClass("tips");
            selfModal.find('select#supplierChoose').val(0);
            selfModal.find('input#serviceItemSearch').val("");
            selfModal.find('input#BeginDate').val("");
            selfModal.find('input#EndDate').val("");
            selfModal.find('textarea#Remark').val("");
            thatTable.clear().draw();
            jqTable.find(".allrows").prop("checked", false).trigger("change");
            selfModal.find(".stepclick[which=step1]").trigger("click");

        });
        thisModal.on('click', ".label-used-most", function() {
            var forWhich = $(this).closest("div").data("for");
            var oldText = thisModal.find(forWhich).val();
            if (oldText) {
                thisModal.find(forWhich).val(oldText + "；" + $(this).text());
            } else {
                thisModal.find(forWhich).val(oldText + $(this).text());
            }
        });
    }

    function todelete(Refs) {
        var table = $('#registerCancel');
        table.one('click', '.registerDelete', function todeleting(e) {
            var _this = $(this);
            var arr = [];
            arr.push(_this.closest("tr").attr("id"));
            var name = _this.closest("tr").find("td:eq(1)").html();
            var post = {};
            post.CancelRegisterID = arr.join(",");
            $.LangHua.confirm({
                title: "提示信息",
                tip1: '您确定要删除',
                tip2: name,
                confirmbutton: '确定',
                cancelbutton: '取消',
                data: post,
                confirm: function() {
                    $.ajax({
                        url: '/CancelRegisters/Delete',
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
                            Refs.jQueryRef.one('click', '.registerDelete', todeleting);
                        }
                    });
                }
            });
        });

    }

    function edit(Refs) {
        var thisModal = $('#register-cancel-edit');
        var table = $('#registerCancel');
        table.on('click', '.registerEdit', function() {

            var tr = $(this).closest("tr").data('cancelregister');
            var Remark = tr.Remark;
            var StartDate = tr.StartDate;
            var EndDate = tr.EndDate;
            var ServiceCode = tr.ServiceCode;
            var SupplierName = tr.SupplierName;
            var SupplierNo = tr.SupplierNo;
            var CancelRegisterID = tr.CancelRegisterID;

            $('#register-cancel-edit').find(".tips").text("").removeClass("tips");
            $('#register-cancel-edit').find('#Remark').val(Remark);
            $('#register-cancel-edit').find('#EndDate').val(EndDate);
            $('#register-cancel-edit').find('#BeginDate').val(StartDate);
            $('#register-cancel-edit').find('#CancelRegisterID').text(CancelRegisterID);
            $('#register-cancel-edit').find('#selectedItem').text("(" + SupplierNo + ")" + SupplierName + ServiceCode);
            $('#register-cancel-edit').modal({});
        });

        thisModal.find('#edit').one('click', function editing(e) {
            var _this = $(this);
            var thisM = $(this).closest('#register-cancel-edit');
            var post = {}
            post.BeginDate = thisM.find("#BeginDate").val();
            post.EndDate = thisM.find("#EndDate").val();
            post.Remark = thisM.find("#Remark").val();
            var postable = true;
            if ((!post.BeginDate) || (!post.EndDate)) {
                thisM.find("#BeginDate ,#EndDate").formWarning({
                    tips: "请填写完整的取消日期"
                });
                postable = false;
            }
            if (!post.Remark) {
                thisM.find("#Remark").formWarning({
                    tips: "请填写取消原因"
                });
                postable = false;
            }
            if (!postable) {
                _this.one('click', editing);
                return;
            }
            post.CancelRegisterID = thisModal.find('#CancelRegisterID').text();
            $.ajax({
                url: '/CancelRegisters/Edit',
                type: 'post',
                contentType: "application/json; charset=utf-8;",
                data: JSON.stringify(post),
                dataType: 'json',
                success: function(data) {
                    if (data.ErrorCode == 200) {
                        $.LangHua.alert({
                            tip1: '修改结果',
                            tip2: '修改成功',
                            button: '确定',
                            icon: "warning",
                            callback: function() {
                                Refs.dataTableRef.draw(false);
                                thisM.modal("hide");
                            }
                        });
                    } else if (data.ErrorCode == 401) {
                        $.LangHua.alert({
                            tip1: '修改失败',
                            tip2: data.ErrorMessage,
                            button: '确定',
                            icon: "warning"

                        });
                    } else {
                        $.LangHua.alert({
                            tip1: '修改失败',
                            tip2: "修改失败，请重试！",
                            button: '确定',
                            icon: "warning"
                        });
                    }
                },
                error: function() {
                    $.LangHua.alert({
                        tip1: '修改失败',
                        tip2: "修改失败，请重试！",
                        button: '确定',
                        icon: "warning"
                    });
                },
                complete: function() {
                    _this.one('click', editing);
                }
            });

        });
        thisModal.on('click', ".label-used-most", function() {
            var forWhich = $(this).closest("div").data("for");
            var oldText = thisModal.find(forWhich).val();
            if (oldText) {
                thisModal.find(forWhich).val(oldText + "；" + $(this).text());
            } else {
                thisModal.find(forWhich).val(oldText + $(this).text());
            }
        });
    }