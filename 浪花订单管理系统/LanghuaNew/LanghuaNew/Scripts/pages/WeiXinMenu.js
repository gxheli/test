//调用选择素材界面的行号
var SelectIndex = 0;
//调用选择素材界面的行之前选中的素材
var SelectLHNewID ="";
jQuery(document).ready(function () {
    //选择素材界面的搜索回车绑定
    var fuzzyString = document.getElementById("KeyWord");
    fuzzyString.onkeydown = jump;
    function jump(event) {
        var event = event || window.event;
        if (event.keyCode == 13) {
            $('#KeyButton').click();
        }
    }
    //选择素材界面的初始化方法
    $(document).on('show.bs.modal', '#myModal', function () {
        var Button = $(event.target);
        SelectLHNewID = Button.attr("SelectLHNewID")
        SelectIndex = Button.parent().attr("index");
        //清空之前的内容
        $('#KeyWord').val("");
        $('#tabelContent').text("");

    })
   
});
//菜单类型变化
function Show(DDL) {
    var DDLObj = $(DDL);
    if (DDLObj.val() == 0) { 
        $('textarea[index=' + DDLObj.attr('index') + ']').hide();
        $('div[index=' + DDLObj.attr('index') + ']').show();
    }
    else {
        $('textarea[index=' + DDLObj.attr('index') + ']').show();
        $('div[index=' + DDLObj.attr('index') + ']').hide();
    }
}
//手动更新素材库
function UpdateNews() {
    $.ajax({
        url: "/WeiXinMenu/UpdateNews",
        type: 'get',
        dataType: 'json',
        beforeSend: function (xhr) {
            $('#tabelContent').text("素材更新中...")

        },
        complete: function (XHR, TS) {
            $('#tabelContent').text("素材更新完毕")
        }

    })
}
//素材库关键字查询
function GetNewsByKeyWord() {
    var keyWord = $('#KeyWord').val();
    if (keyWord != "") {
        $.ajax({
            url: "/WeiXinMenu/GetNewsByKeyWord?keyWord=" + keyWord,
            type: 'get',
            dataType: 'json',
            beforeSend: function (xhr) {
                $('#tabelContent').text("素材搜索中...")

            },
            complete: function (XHR, TS) {
                var response = JSON.parse(XHR.responseText);
                if (response.length > 0) {
                    var text = '<table class="table">';

                    $(response).each(function (i, domEle) {
                        text += '<tr style="height:100px">';
                        //图片
                        var UrlPath = escape(domEle.Articles[0].PicUrl);
                        text += '<td style="width:100px"><img src="WeiXinMenu/GetImage?ImagePath=' + UrlPath + '" style="width:100px;height:100px" /></td>';
                        //素材标题
                        text += '<td style="text-align:left">';
                        var SelectText = "";
                        $(domEle.Articles).each(function (j, jdomEle) {
                            if (j == 0) {
                                SelectText = jdomEle.Title;
                            }
                            text += (j + 1) + jdomEle.Title + '<br/>';
                        });
                        text += '</td>';
                        //选择框
                        if (SelectLHNewID == domEle.LHNewID) {
                            text += ' <td style="text-align:right"><button type="button" id="' + domEle.LHNewID + '"  class="btn btn-default" style="margin-top:40px;min-width:100px" onclick="SelectNew(' + domEle.LHNewID + ',&quot;' + SelectText + '&quot;)">已选择</button></td>'
                        }
                        else {
                            text += ' <td style="text-align:right"><button type="button" id="' + domEle.LHNewID + '"  class="btn btn-primary" style="margin-top:40px;min-width:100px" onclick="SelectNew(' + domEle.LHNewID + ',&quot;' + SelectText + '&quot;)">选择</button></td>'
                        }

                        text += '</tr>'
                    });
                    text += '</table>';
                    $('#tabelContent')[0].innerHTML = text;
                }
            }

        })
    }
   
}
//选择素材
function SelectNew(LHNewID, SelectText) {
    $("div[index=" + SelectIndex + "] span").text(SelectText);
    $("div[index=" + SelectIndex + "] button").attr("SelectLHNewID", LHNewID);
    $('#myModal').modal('hide');
}
//清空行
function ClearRow(Button) {
    var tr = $(Button).parent().parent();
    //删除菜单名称
    tr.find("[name='MenuName']").val("");
    //类型设置为存文本
    tr.find("[name='MenuType']").val("1");
    tr.find("[name='MenuType']").change();
    //清空菜单内容
    tr.find("[name='MenuText']").text("请选择图文素材");
    tr.find("[name='MenuButton']").attr("SelectLHNewID", "")
    tr.find("[name='MenuValue']").val("");
    
}
//保存提交
function ConfirmMenu() {
    var data = new Array();
    $("#loginForm > div[name='divMenuContainer']").each(function (i, domEle) {
        var Menu=new Object();
        Menu.name = $(domEle).find("[name='MenuName']").val();
        Menu.RowNo = i;
        Menu.Items = new Array();
        $(domEle).find("table[name='MenuTable']>tbody>tr").each(function (j, trEle) {
            var MenuItem = new Object();
            var tr = $(trEle);
            MenuItem.name = tr.find("[name='MenuName']").val();
            MenuItem.ItemType = tr.find("[name='MenuType']").val();
            //如果是图文素材
            if (MenuItem.ItemType == 0) {
                MenuItem.Text = tr.find("[name='MenuText']").text();
                MenuItem.value = tr.find("[name='MenuButton']").attr("SelectLHNewID");
                if (MenuItem.value == null || MenuItem.value =="") {
                    $.LangHua.alert({
                        title: "提示信息",
                        tip1: '菜单格式不正确',
                        tip2: "图文素材类型的菜单必须选择图文素材",
                        button: '确定'
                    })
                    return;
                }
            }
            else {
                MenuItem.Text = tr.find("[name='MenuValue']").val();
                MenuItem.value = MenuItem.Text;
            }
            MenuItem.RowNo = j;
            Menu.Items[j] = MenuItem;

        });
        data[i] = Menu;
    });
    $.ajax({
        type: 'post',
        dataType: 'json',
        contentType: "application/json; charset=utf-8;",
        data: JSON.stringify(data),
        url: '/WeiXinMenu/CommitMenu',
        success: function (data) {
            if (data.ErrorCode == 200) {
                $.LangHua.alert({
                    title: "提示信息",
                    tip1: '更新成功',
                    tip2: "",
                    button: '确定'
                })

            }
            else {
                if (data.ErrorCode == 401) {
                    $.LangHua.alert({
                        title: "提示信息",
                        tip1: '更新菜单失败',
                        tip2: data.ErrorMessage,
                        button: '确定'
                    })
                }
             
            }
        },
        failed: function () {
            $.LangHua.alert({
                title: "提示信息",
                tip1: '更新菜单失败',
                tip2: "",
                button: '确定'
            })
        }


    })
   
  
}