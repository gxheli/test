$(document).ready(function () {

    fixed();

    // 
    $('#subAccountList').eq(0)
       .on('preXhr.dt', function (e, settings, json) {
           $('#reflashTable').find('i').addClass("fa-spin");
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
           json['UsersSearch'] = search;

       }).on('xhr.dt', function (e, settings, json, xhr) {
           if (!json) {
               return;
           }
           $('#reflashTable').find('i').removeClass("fa-spin");
           $('#subAccountList thead tr th:eq(0) input').prop("checked", false)

           var searchobj = json.SearchModel.UsersSearch || null;
           console.log(searchobj)
           if (searchobj == null) {
               $('#fuzzyString').val("");
               $('#status').val('-1');
               return
           }
           else if (searchobj['searchType'] == "status") {
               $('#fuzzyString').val("");
           }
           else if (searchobj['searchType'] == "fuzzy") {
               $('#status').val('-1');
               $('#fuzzyString').val(searchobj['FuzzySearch']);
           }
       })


    var subAccountList =
	jQuery('table#subAccountList').DataTable({
	    ajax: {
	        url: "/Users/GetUsers",
	        type: 'post',
	        globalHandler: true,
	        globalMessage: {
	            error: "账号数据请求失败"
	        }

	    },
	    ordering: false,
	    searching: false,
	    serverSide: true,

	    drawCallback: function (settings) {
	        var api = this.api();
	    },
	    //行操作
	    rowId: "SupplierUserID",
	    createdRow: function (row, data, dataIndex) {
	        var _this = this.api();
	        var thisTable = this;
	    },

	    //列操作
	    columns: [
             //左格选择
            {
                'data': null,

                "render": function (data, type, full, meta) {
                    return '<input type="checkbox" class="checkboxes">';
                }
            },
            //用户名
            {
                'data': 'SupplierUserName',
            },

            //昵称
            {
                'data': 'SupplierNickName',

            },

            // 角色
            {
                'data': 'SupplierRoles',
                'render': "[， ].SupplierRoleName"

            },
            //创建时间
            {
                'data': 'CreateTime',

            },
               //最近登录时间
            {
                'data': 'LastLoginTime',
                'render': function (data, type, full, meta) {
                    return ('<div>' + data + '</div><div style="color:#cccccc">' + (full.IP ? full.IP : "") + '</div>');
                }

            },
               //状态
            {
                'data': 'SupplierUserEnableState',
                'render': function (data, type, full, meta) {
                    if (data == 0) {
                        return '启用';
                    }
                    else {
                        return '<span style="color:red">禁用</span>'
                    }
                }
            },

               //微信绑定
            {
                'data': 'WeixinBind',
                'render': function (data, type, full, meta) {
                    if (data == true) {
                        return '<a class="clickToUnbind" href="javascript:;">已绑定</a>';
                    }
                    else {
                        return '';
                    }
                }

            },
            //操作
            {
                'data': 'SupplierUserID',
                'render': function (data, type, full, meta) {
                    return (
                        '<a target="_blank" href="/Users/Edit/' + data + '">修改</a><span> | </span>' +
                        '<a target="_blank" href="/Users/UsersOperation/' + data + '">日志</a><span> | </span>' +
                        '<a target="_blank" class="revisePassword" href="javascript:;">重置密码</a>'
                    )
                }

            },

	    ]
	});

    jQuery('body').on("click", '#reflashTable', function () {
        subAccountList.draw();
    })



    // 表格选择
    $('table.ddtable:first').on('change', '.group-checkable', function (e) {
        $(this).closest('table').find('tbody tr td input.checkboxes').prop('checked', $(this).prop("checked"));
        if ($(this).prop("checked")) {
            $(this).closest('table').find('tbody tr ').addClass("selectedRow");
        }
        else {
            $(this).closest('table').find('tbody tr ').removeClass("selectedRow");
        }
        var length = $('table.ddtable:first').find('tbody tr td input.checkboxes:checked').length
        $('#selectedNumber').text(length);
    })
    $('table.ddtable:first').on('change', 'input.checkboxes', function (e) {
        if ($(this).prop("checked")) {
            $(this).closest('tr').addClass("selectedRow");
        }
        else {
            $(this).closest('tr').removeClass("selectedRow");
        }
        var _this = this;
        var currentPageLength = subAccountList.page.info().end - subAccountList.page.info().start;

        if ($(this).closest("tbody").find("tr td input:checked").length != currentPageLength) {
            $(_this).closest("table.ddtable").find(".group-checkable:first").prop("checked", false);
        }
        else {
            $(_this).closest("table.ddtable").find(".group-checkable:first").prop("checked", true);
        }
        var length = $('table.ddtable:first').find('tbody tr td input.checkboxes:checked').length
        $('#selectedNumber').text(length);

    })



    //模糊搜索 开始
    jQuery("#fuzzySearch").bind("click", function () {
        var fuzzyString = jQuery("#fuzzyString").val().trim();
        $('#searchoption').data("search", {
            'FuzzySearch': fuzzyString,
            'Type': "",
            'searchType': 'fuzzy'
        });
        subAccountList.draw();
    })
    jQuery('#fuzzyString').bind("keydown", function (evt) {
        evt = (evt) ? evt : ((window.event) ? window.event : "") //兼容IE和Firefox获得keyBoardEvent对象  
        var key = evt.keyCode ? evt.keyCode : evt.which; //兼容IE和Firefox获得keyBoardEvent对象的键值  
        if (key == 13) {
            jQuery("#fuzzySearch").trigger("click");
        }

    })
    //模糊搜索 结束

    //状态
    jQuery("#status").bind("change", function () {
        var status = jQuery("#status").val();
        $('#searchoption').data("search", {
            'status': status,
            'Type': "",
            'searchType': 'status'
        });
        subAccountList.draw();
    })




    // 账号流转
    $('#operations').on("click", "a", function () {
        var operation = $(this).data('next-code');
        var numberx = parseInt($('#selectedNumber').text());
        console.log(numberx)
        if (numberx == 0) {
            return
        }
        var arr = []
        $('#subAccountList tr.selectedRow').each(function () {
            arr.push($(this).attr('id'));
        })
        var userID = arr.join(',');
        if (userID.length == 0) {
            return;
        }
        $.ajax({
            url: "/Users/UpdateDisable",
            type: 'post',
            contentType: "application/json; charset=utf-8;",
            data: JSON.stringify({
                UserID: userID,
                Operation: operation
            }),
            dataType: 'json',
            beforeSend: function () {
            },
            success: function (data) {

                if (data.failed.length == 0) {
                    subAccountList.draw(false);

                    return
                }
                subAccountList.draw(false);

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
            }
        })
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


    //微信绑定动作处理
    jQuery("body").on('click', ".clickToUnbind", function () {
        var userid = jQuery(this).closest('tr').attr('id');
        console.log(userid)
        var username = jQuery(this).closest('tr').find("td:eq(1)").text();
        jQuery.LangHua.confirm({
            title: "提示信息",
            tip1: '请确认是否解绑下列用户的微信？',
            tip2: username,
            confirmbutton: '确定',
            cancelbutton: '取消',
            data: null,
            confirm: function () {
                $.ajax({
                    url: "/Users/UnbindWeixin/" + userid,
                    type: 'post',
                    dataType: 'json',
                    success: function (data) {
                        if (data.ErrorCode == 200) {
                            jQuery.LangHua.alert({
                                title: "提示信息",
                                tip1: '提示信息：',
                                tip2: "解绑成功!",
                                button: '确定'
                            })
                            subAccountList.draw(false)
                        }
                        else if (data.ErrorCode == 401) {
                            jQuery.LangHua.alert({
                                title: "提示信息",
                                tip1: '提示信息：',
                                tip2: data.ErrorMessage,
                                button: '确定'
                            })
                        }
                        else {
                            jQuery.LangHua.alert({
                                title: "提示信息",
                                tip1: '提示信息：',
                                tip2: "解绑失败!",
                                button: '确定'
                            })
                        }
                    }
                })
            }
        })
    })


    // 重置密码
    jQuery("body").on('click', ".revisePassword", function () {
        var userid = jQuery(this).closest('tr').attr('id');
        var username = jQuery(this).closest('tr').find("td:eq(1)").text();
        jQuery.LangHua.confirm({
            title: "提示信息",
            tip1: '请确认是否为以下账号重置密码：',
            tip2: username,
            confirmbutton: '确定',
            cancelbutton: '取消',
            data: null,
            confirm: function () {
                $.ajax({
                    url: "/Users/ResetPassWord/" + userid,
                    type: 'post',
                    dataType: 'json',

                    success: function (data) {
                        jQuery.LangHua.alert({
                            title: "提示信息",
                            tip1: '提示信息：',
                            tip2: data.ErrorMessage,
                            button: '确定'
                        })

                    }
                })
            }
        })
    })


    //
});