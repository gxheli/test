'use strict'
$(document).ready(function() {
    initSelect();
    createEdit();
});

function initSelect() {
    $('#rights').on("change", ".groupRight", function() {
        $(this).closest("tr").find(".unitRight").prop("checked", $(this).prop("checked"));
    }).on("change", ".unitRight", function() {
        var numTotal = $(this).closest("td").find(".unitRight").length;
        var numChecked = $(this).closest("td").find(".unitRight:checked").length;
        $(this).closest("tr").find(".groupRight:eq(0)").prop("checked", (numTotal === numChecked));
    });
    $('#rights tbody tr').each(function() {
        $(this).find('.unitRight:eq(0)').trigger("change");
    });
}

function createEdit() {
    $('#roleSave').one('click', function creating() {
        var thisButton = $(this);
        var RoleName = $('#RoleName').val() ? $('#RoleName').val() : "";
        var Remark = $('#RoleRemark').val() ? $('#RoleRemark').val() : "";
        var arrRightID = [];
        $("#rights .unitRight:checked").each(function() {
            arrRightID.push($(this).val());
        });
        if ((!RoleName)) {
            $('#RoleName').formWarning({
                tips: "请填写角色名称"
            });
            thisButton.one('click', creating);
            return;
        }
        if (arrRightID.length === 0) {
            thisButton.success("请选择至少一项权限")
            thisButton.one('click', creating);
            return;
        }
        var post = {
            "RoleName": RoleName,
            "Remark": Remark,
            "RightID": arrRightID.join(",")
        };
        if ($('input#RoleID').length > 0) {
            post.RoleID = $('input#RoleID').val();
        }
        $.ajax({
            url: '/Roles/Save',
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
                            window.location.href = "/Roles/Index";
                        }
                    });
                } else if (data.ErrorCode == 401) {
                    $.LangHua.alert({
                        tip1: '保存失败',
                        tip2: data.ErrorMessage,
                        button: '确定',
                        icon: "warning"
                    });
                    thisButton.one('click', creating);


                } else {
                    $.LangHua.alert({
                        tip1: '保存失败',
                        tip2: "保存失败，请重试！",
                        button: '确定',
                        icon: "warning"
                    });
                    thisButton.one('click', creating);
                }
            },
            error: function() {
                $.LangHua.alert({
                    tip1: '保存失败',
                    tip2: "保存失败，请重试！！",
                    button: '确定',
                    icon: "warning"
                });
                thisButton.one('click', creating);
            },
            complete: function() {
                return;
            }
        });


    });
}