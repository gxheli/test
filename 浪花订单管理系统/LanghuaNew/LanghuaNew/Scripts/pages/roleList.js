    'use strict';
    jQuery(document).ready(function($) {
        // fix();
        var Distribution = tableInit($('.tabletools').eq(0));
        tabletoolInint(Distribution);
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
        var roleList =
            jQuery('table#roleList')
            .on('preXhr.dt', function(e, settings, json) {
                tabletools.find('a .fa-refresh').addClass("fa-spin");
                delete json.columns;
                delete json.order;
                delete json.search;
            })
            .on('preXhr.dt', function(e, settings, json, xhr) {
                tabletools.find('a .fa-refresh').removeClass("fa-spin");
            })
            .DataTable({
                ajax: {
                    url: "/Roles/GetRoles",
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
                    thistable.on('click', ".roleMembers", function() {
                        var role = $(this).closest("tr").data('role');
                        $('#roleDetail #roleName').text("角色：" + role.RoleName);
                        $('#roleDetail #members').empty("");
                        if (role.Users) {
                            $('#roleDetail #roleMemberNumber').text("成员：" + role.Users.length + "人");
                            var merber = $("<div></div>");
                            merber.css({
                                "display": "inline-block",
                                "float": "left",
                                "width": "25%",
                                "padding": "0px 5px",
                                "margin": "5px 0px"
                            });
                            for (var i in role.Users) {
                                $('#roleDetail #members').append(merber.clone().text(role.Users[i]));
                            }
                        } else {
                            $('#roleDetail #roleMemberNumber').text("成员：" + 0 + "人");
                        }
                        $('#roleDetail').modal("show");
                    });
                },
                //行操作
                rowId: "RoleID",
                createdRow: function(row, data, dataIndex) {
                    if ((data.IsCancel)) {
                        $(row).css("background", '#BCBCBC');
                    }
                    var _this = this.api();
                    $(row).data('role', data);
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
                        'data': 'RoleName',

                    },
                    {
                        'targets': [2],
                        'data': 'RoleRemark',
                    },
                    {
                        'targets': [3],
                        'data': "Users",
                        'render': function(cellData, type, rowData, meta) {
                            if (cellData) {
                                return (cellData.length);
                            } else {
                                return (0);
                            }
                        }
                    },
                    {
                        'targets': [4],
                        'data': 'RoleEnableState',
                        'render': function(cellData, type, rowData, meta) {
                            return (cellData == 0 ? "启用" : '<span style="color:red">禁用</span>');
                        }
                    },
                    {
                        'targets': [5],
                        'data': 'RoleID',
                        'render': function(cellData, type, rowData, meta) {
                            return (
                                '<div class="row">' +
                                '<a class="hrefInTable-inline" href="/Roles/Edit/' + cellData + '"  >修改</a>' +
                                '<a act="delete" class="hrefInTable-inline roleMembers" >成员</a>' +
                                '</div>'
                            );
                        }
                    }
                ]
            });
        return {
            'dataTableRef': roleList,
            'jQueryRef': jQuery('table#roleList')
        };
    }

    function　 tabletoolInint(Refs) {
        $('.tabletools:eq(0)').find('#reflashTable').one("click", function update(e) {
            Refs.dataTableRef.draw();
            $(this).one("click", update);
        });
        $('.tabletools:eq(0)').find('#deleteall').bind("click", { "table": Refs.jQueryRef }, function deleteall(e) {
            var _this = $(this);
            var table = e.data.table;
            var arr = [];
            table.find("tbody tr input:checked").each(function() {
                arr.push($(this).closest("tr").attr("id"));
            });
            var name = '所选的' + arr.length + "项";
            var post = {};
            post.RoleID = arr.join(",");
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
                        url: '/Roles/UpdateDisable',
                        type: 'post',
                        contentType: "application/json; charset=utf-8;",
                        data: JSON.stringify(post),
                        dataType: 'json',
                        success: function(data) {
                            if (data.ErrorCode == 200) {
                                if ('failed' in data) {
                                    if (data.failed.length > 0) {
                                        var failed = data.failed;
                                        var str = '';
                                        for (var i in failed) {
                                            var arr = [
                                                '<div style="margin:10px 0px">',
                                                '<span style="color:#0099cc">' + failed[i]['name'] + '：</span>',
                                                '<span style="color:#333" >' + failed[i]['reason'] + '</span>',
                                                '</div>',
                                            ].join('\n');
                                            str += arr;
                                        }
                                        $.LangHua.showResult({
                                            title: "删除结果",
                                            content: str,
                                            confirmbutton: '确定',
                                            data: null
                                        });
                                    }
                                }
                                Refs.dataTableRef.draw();
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
                        complete: function() {}
                    });
                }
            });
        });
        $('.tabletools:eq(0)').find('#enableall').one("click", { "table": Refs.jQueryRef }, function enableall(e) {
            var _this = $(this);
            var table = e.data.table;
            var arr = [];
            table.find("tbody tr input:checked").each(function() {
                arr.push($(this).closest("tr").attr("id"));
            });
            var post = {};
            post.RoleID = arr.join(",");
            post.Operation = _this.attr("operation");

            $.ajax({
                url: '/Roles/UpdateDisable',
                type: 'post',
                contentType: "application/json; charset=utf-8;",
                data: JSON.stringify(post),
                dataType: 'json',
                success: function(data) {
                    if (data.ErrorCode == 200) {
                        if ('failed' in data) {
                            if (data.failed.length > 0) {
                                var failed = data.failed;
                                var str = '';
                                for (var i in failed) {
                                    var arr = [
                                        '<div style="margin:10px 0px">',
                                        '<span style="color:#0099cc">' + failed[i]['name'] + '：</span>',
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
                            } else {
                                Refs.dataTableRef.draw(false);
                            }
                            Refs.dataTableRef.draw();
                        }
                        Refs.dataTableRef.draw(false);
                    } else if (data.ErrorCode == 401) {
                        $.LangHua.alert({
                            tip1: '启用失败',
                            tip2: data.ErrorMessage,
                            button: '确定',
                            icon: "warning"
                        });
                    } else {
                        $.LangHua.alert({
                            tip1: '启用失败',
                            tip2: "启用失败，请重试！",
                            button: '确定',
                            icon: "warning"
                        });
                    }
                },
                error: function() {
                    $.LangHua.alert({
                        tip1: '启用失败',
                        tip2: "启用失败，请重试!!",
                        button: '确定',
                        icon: "warning"
                    });
                },
                complete: function() {
                    _this.one('click', { "table": Refs.jQueryRef }, enableall);
                }
            });
        });
        $('.tabletools:eq(0)').find('#unableall').one("click", { "table": Refs.jQueryRef }, function unableall(e) {
            var _this = $(this);
            var table = e.data.table;
            var arr = [];
            table.find("tbody tr input:checked").each(function() {
                arr.push($(this).closest("tr").attr("id"));
            });
            var post = {};
            post.RoleID = arr.join(",");
            post.Operation = _this.attr("operation");

            $.ajax({
                url: '/Roles/UpdateDisable',
                type: 'post',
                contentType: "application/json; charset=utf-8;",
                data: JSON.stringify(post),
                dataType: 'json',
                success: function(data) {
                    if (data.ErrorCode == 200) {
                        if ('failed' in data) {
                            if (data.failed.length > 0) {
                                var failed = data.failed;
                                var str = '';
                                for (var i in failed) {
                                    var arr = [
                                        '<div style="margin:10px 0px">',
                                        '<span style="color:#0099cc">' + failed[i]['name'] + '：</span>',
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
                            }
                        }
                        Refs.dataTableRef.draw(false);
                    } else if (data.ErrorCode == 401) {
                        $.LangHua.alert({
                            tip1: '禁用失败',
                            tip2: data.ErrorMessage,
                            button: '确定',
                            icon: "warning"
                        });
                    } else {
                        $.LangHua.alert({
                            tip1: '禁用失败',
                            tip2: "禁用失败，请重试！",
                            button: '确定',
                            icon: "warning"
                        });
                    }
                },
                error: function() {
                    $.LangHua.alert({
                        tip1: '禁用失败',
                        tip2: "禁用失败，请重试!!",
                        button: '确定',
                        icon: "warning"
                    });
                },
                complete: function() {
                    _this.one('click', { "table": Refs.jQueryRef }, unableall);
                }
            });

        });

    }