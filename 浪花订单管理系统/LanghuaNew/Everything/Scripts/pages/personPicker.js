$(document).ready(function() {
    var isClient = false;

    // 选择联系人
    $('body').on('click', '#addpersons', function(topEvent) {
        var forreact = jQuery(this).closest(".addpersons");
        var orderid = $(this).closest('.ddporlet').data('order-item-id');
        var needExtra = $(this).data("needextrainfo"); //1 no 2 yes
        var theSelectedObj = JSON.parse($(this).attr('data-selected'));
        var ageRange = JSON.parse($(this).attr("data-agerange"));
        var oriAgeRange = $(this).attr("data-agerange");
        ageRange.ageMin = ((ageRange.ageMin == -1) ? -Infinity : parseInt(ageRange.ageMin));
        ageRange.ageMax = ((ageRange.ageMax == -1) ? Infinity : parseInt(ageRange.ageMax));
        for (var i in theSelectedObj) {
            var theSelected = theSelectedObj[i];
            break;
        }
        var tmpl = [
            // tabindex is required for focus
            '<div id="personList" class="modal modal-animate" data-backdrop="static" tabindex="-1">',
            '<div class="modal-dialog " role="document">',
            '<div class="modal-content">',
            '<div class="modal-header">',
            '<button type="button" class="close" data-dismiss="modal" aria-hidden="true">×</button>',
            '<h4 class="modal-title">选择联系人</h4>',
            '</div>',
            '<div class="modal-body">',
            '<p style="text-align:center"><a id="toaddaperson" class="btn btn-sm btn-default button70  " data-agerange=' + (oriAgeRange) + ' data-toggle="modalx" data-needextrainfo="' + needExtra + '" data-target="#addoneperson"><i class="fa fa-plus"></i>添加常用游客</a> </p>',
            '<ul class="list-group perosnlist">',
            '正在加载 . . . . . .',
            //li * n
            '</ul>',
            '</div>',
            '<div class="modal-footer">',
            '<div class="visible-xs selectedtips">',
            "已选0个",
            '</div>',
            '<div class="row">',
            '<div class="col-sm-3  col-xs-4">',
            '<a id="toselectedallpersons" class="btn btn-sm btn-default button70  " data-selected=0 style="float:left">全选</a>',
            '</div>',
            '<div class="col-sm-5 hidden-xs selectedtips " style="line-height:30px;">',
            '已选0个',
            '</div>',
            '<div class="col-sm-4 col-xs-8">',
            '<a id="confirmselected" class="btn btn-sm btn-primary button65 ">确定</a>',
            '<a data-dismiss="modal" class="btn btn-sm btn-default button65  ">取消</a>',
            '</div>',
            '</div>',
            '</div>',
            '</div>',
            '</div>',
            '</div>'
        ].join('');
        $('body').one('shown.bs.modal', '#personList', function() {
            var list = jQuery(this).find('.perosnlist').eq(0);
            $.ajax({
                url: '/Travellers/GetTraveller?CustomerID=' + $('#customerid').text().trim() + '&OrderID=' + orderid,
                type: 'get',
                dataType: 'json',
                success: function(data) {
                    if (data.ErrorCode == 200) {
                        var t =
                            data.travellers.map(function(travller, index) {
                                var indexs = '';
                                if (index === 0) {
                                    indexs = 'first';
                                }
                                var detailedited = false;
                                var inAgeRange = false;
                                var checkAble = '';
                                var tips = "";
                                var tipsword = [];
                                var needAgeObj = {};
                                needAgeObj.tips = "";
                                needAgeObj.show = 0;
                                var needExtraObj = {};
                                needExtraObj.tips = "";
                                needExtraObj.show = 0;
                                for (var i in travller.TravellerDetail) {
                                    if (travller.TravellerDetail[i] !== null) {
                                        detailedited = true;
                                    }
                                } {
                                    var arrBirthday = travller.Birthday.split("T")[0].split("-");
                                    var birthday = new Date(arrBirthday[0], arrBirthday[1] - 1, arrBirthday[2], 0, 0, 0);
                                    var today = new Date();
                                    var offYear = today.getFullYear() - birthday.getFullYear();
                                    var monBirthday = birthday.getMonth();
                                    var dateBirthday = birthday.getDate();
                                    var monToday = today.getMonth();
                                    var dateToday = today.getDate();
                                    var havedBirthday = false; {
                                        if (parseInt(monToday) < parseInt(monBirthday)) { //未过
                                        } else if (parseInt(monToday) == parseInt(monBirthday)) {
                                            if (parseInt(dateToday) >= parseInt(dateBirthday)) {
                                                havedBirthday = true;
                                            }
                                        } else { //已过生日
                                            havedBirthday = true;
                                        }
                                    }
                                    var age = offYear;
                                    if (!havedBirthday) {
                                        age -= 1;
                                    }
                                    var style = '';
                                    if (age >= 60) {
                                        style = "color:red;font-weight:bold";
                                    }
                                    if (
                                        (age > ageRange.ageMin) &&
                                        (age < ageRange.ageMax)
                                    ) {
                                        inAgeRange = true;
                                    } else {
                                        checkAble = "disabled";
                                        tips = 'data-toggle="tooltip" ';
                                        needAgeObj.show = 1;
                                        if (ageRange.ageMin == -Infinity) {
                                            needAgeObj.tips = "年龄应小于" + ageRange.ageMax + "岁\n";
                                        } else {
                                            if (ageRange.ageMax == Infinity) {
                                                needAgeObj.tips = "年龄应大于" + ageRange.ageMin + "岁\n";
                                            } else {
                                                needAgeObj.tips = "年龄应大于" + ageRange.ageMin + "岁，\n且小于" + ageRange.ageMax + "岁\n";
                                            }
                                        }
                                    }

                                }
                                var str = '';
                                if (!detailedited) {
                                    if (needExtra == 2) {
                                        checkAble = "disabled";
                                        tips = 'data-toggle="tooltip" ';
                                        needExtraObj.show = 1;
                                        needExtraObj.tips = "需要填写附加资料";
                                    }
                                    str = [
                                        '<div class="col-md-4  revise any-unselectable    text-center margin-top-10 margin-bottom-10">',
                                        '<a >填写附加资料</a>',
                                        '</div>'
                                    ].join('\n');

                                } else {
                                    str = [
                                        '<div class="col-md-4  revise any-unselectable    text-center margin-top-10 margin-bottom-10">',
                                        '<span class="dark">已填写(</span>',
                                        '<a>修改</a>',
                                        '<span class="dark">',
                                        ')',
                                        '</span>',
                                        '</div>'
                                    ].join('\n');
                                }
                                return [

                                    '<li    id="' + travller.TravellerID + '" class="list-group-item one" data-index=' + index + '>',
                                    '<label ' + tips + ' class="select"  needextra="' + needExtraObj.show + '" needage="' + needAgeObj.show + '"  tid="' + travller.TravellerID + '" index="' + indexs + '" >',
                                    '<input ' + checkAble + ' type="checkbox"  />',
                                    '<div style="height:100%;width:100%;position:absolute;top:0px;"></div>',
                                    '</label>',
                                    '<div class="main"   needextra = "' + needExtraObj.tips + '"  needage = "' + needAgeObj.tips + '">',
                                    '<div class="container-fluid">',
                                    '<div class="row">',
                                    '<div class="col-md-8 info">',
                                    '<div class="font14 bold">',
                                    '<span class="cnname">' + travller.TravellerName + '</span> / ',
                                    '<span class="enname">' + travller.TravellerEnname + '</span>',
                                    '<span style="font-weight:normal"  >（<span  class="reviseaperson" data-agerange=' + (oriAgeRange) + '><a>修改</a></span>）</span>',
                                    '</div>',
                                    '<div class="row font12 font-gray">',
                                    '<div class="col-md-8">护照：<span class="passport">' + travller.PassportNo + '</span></div>',
                                    '<div class="col-md-4">年龄：' + '<span class="age"><span style="' + style + '">' + age + '</span></span>' + '</div>',
                                    '</div>',
                                    '</div>',
                                    str,
                                    '</div>',
                                    '</div>',
                                    '</div>',
                                    '</li>',
                                ].join('');
                            }).join('');
                        if (data.travellers.length === 0) {
                            t = '<span class="existno">没有可选游客，请先添加</span>';
                        }
                        list.empty().append(t);
                        list.find('li').each(function() {
                            $(this).data(
                                $.extend({},
                                    data.travellers[$(this).data('index')], { 'TravellerID': data.travellers[$(this).data('index')].TravellerID }
                                )
                            );

                        });
                        $('body').trigger('resize');
                        bindActions();
                    }
                },
                error: function() {}
            });
        });
        var this_listModal =
            $(tmpl).modal();
        $('#personList').data({
            'forreact': forreact
        });

        function bindActions() {
            var timeer = {};
            this_listModal.on('touchstart', '[data-toggle=tooltip]', function() {
                var placement = "top";
                if ($(this).attr("index") == 'first') {
                    placement = "bottom";
                }
                var tid = $(this).attr("tid");
                if (!(tid in timeer)) {
                    timeer[tid] = -1;
                }
                e.preventDefault();
                if ($(this).attr("needage") == 1) {
                    title += $(this).siblings('.main:eq(0)').attr("needage") + "; ";
                }
                if ($(this).attr("needextra") == 1) {
                    title += $(this).siblings('.main:eq(0)').attr("needextra");
                }
                $(this).siblings(".main").tooltip({
                    container: ".one",
                    placement: placement,
                    title: title
                }).tooltip("show");
                var _this = this;
                if (timeer[tid] != -1) {
                    window.clearTimeout(timeer[tid]);
                }
                timeer[tid] = setTimeout(function() {
                    $(_this).siblings(".main").tooltip("destroy");
                    timeer[tid] = -1;
                }, 2000);
            });
            this_listModal.on('click', '[data-toggle=tooltip]', function() {
                var placement = "top";
                if ($(this).attr("index") == 'first') {
                    placement = "bottom";
                }
                var tid = $(this).attr("tid");
                if (!(tid in timeer)) {
                    timeer[tid] = -1;
                }
                var title = "";
                if ($(this).attr("needage") == 1) {
                    title += $(this).siblings('.main:eq(0)').attr("needage") + "; ";
                }
                if ($(this).attr("needextra") == 1) {
                    title += $(this).siblings('.main:eq(0)').attr("needextra");
                }
                $(this).siblings(".main").tooltip({
                    trigger: 'click ',
                    container: ".main",
                    placement: placement,
                    // selector:".perosnlist",
                    title: title
                }).tooltip("show");
                var _this = this;
                if (timeer[tid] != -1) {
                    window.clearTimeout(timeer[tid]);
                }
                timeer[tid] = setTimeout(function() {
                    $(_this).siblings(".main").tooltip("destroy");
                    $(_this).siblings(".main").find('.tooltip').remove();

                    timeer[tid] = -1;
                }, 2000);

            });
            this_listModal.on('change', 'input[type=checkbox]', function() {
                var count = this_listModal.find('input:checked').length;
                this_listModal.find("div.selectedtips").text('已选' + count + '个');
            });
            this_listModal.find('li.one').each(function() {
                var thisID = $(this).attr('id');
                if (thisID in theSelected) {
                    $(this).find('input[type=checkbox]').eq(0).prop('checked', true).trigger("change");
                }
            });
            this_listModal.find('#toselectedallpersons').bind('click', function(e) {
                var _this = $(this);
                if ($(this).data('selected') == 0) {
                    this_listModal.find('ul.perosnlist').eq(0).find('label input[type=checkbox]:not(:disabled)').each(function() {
                        $(this).prop("checked", true);
                        _this.data('selected', 1);
                        this_listModal.find('input[type=checkbox]').eq(0).trigger("change");
                    });
                } else {
                    this_listModal.find('ul.perosnlist').eq(0).find('label input[type=checkbox]:not(:disabled)').each(function() {
                        $(this).prop("checked", false);
                        _this.data('selected', 0);
                        this_listModal.find('input[type=checkbox]').eq(0).trigger("change");

                    });
                }
            });

            this_listModal.find('#confirmselected').bind('click', function(e) {
                var _this = $(this);
                var arr = [];
                this_listModal.find('ul.perosnlist').eq(0).find('li').each(function() {
                    var _thisLi = $(this);
                    if ($(this).find("label input[type=checkbox]").prop('checked')) {
                        var personData = _thisLi.data();
                        var copy = {
                            personID: personData.TravellerID,
                            text: personData.TravellerName
                        };
                        var detail = $.extend(true, {}, personData.TravellerDetail);
                        delete personData.TravellerDetail;
                        var one = $.extend(true, copy, personData, detail);
                        arr.push(one);
                    }
                });
                forreact.trigger('addone', [arr]);
                this_listModal.modal('hide');
            });
        }
    });

    //新添加联系人
    $('body').on('click', '#toaddaperson', function() {
        var listmodal = $(this).closest('#personList');
        var needextrainfo = $(this).data("needextrainfo");
        var oriAgeRange = $(this).attr("data-agerange");
        var ageRange = JSON.parse($(this).attr("data-agerange"));

        ageRange.ageMin = ((ageRange.ageMin == -1) ? -Infinity : parseInt(ageRange.ageMin));
        ageRange.ageMax = ((ageRange.ageMax == -1) ? Infinity : parseInt(ageRange.ageMax));
        var tmp = [
            '<div id="addoneperson" class="modal modal-animate" data-backdrop="static" tabindex="-1">',
            '<div class="modal-dialog " role="document">',
            '<div class="modal-content">',
            '<div class="modal-header">',
            '<button type="button" class="close" data-dismiss="modal" aria-hidden="true">×</button>',
            '<h4 class="modal-title">新增常用游客资料</h4>',
            '</div>',
            '<div class="modal-body">',
            '<form class="form-horizontal" role="form">',
            '<div class="form-body">',
            '<div style="margin-bottom:10px">一次输入，永久保存，多次选择使用，更加方便</div>',
            '<div class="form-group">',
            '<label class="col-md-3 control-label"><span style="color:red;margin-right:5px;">*</span>中文姓名：</label>',
            '<div class="col-md-9">',
            '<input id="cnname" type="text" class="form-control input-inline input-medium" placeholder="张三">',
            '<span class="help-inline"></span>',
            '</div>',
            '</div>',
            '<div class="form-group">',
            '<label class="col-md-3 control-label"><span style="color:red;margin-right:5px;">*</span>姓名拼音：</label>',
            '<div class="col-md-9">',
            '<input id="enname" type="text" class="form-control input-inline input-medium" placeholder="ZHANGSAN">',
            '<span class="help-inline">  </span>',
            '</div>',
            '</div>',
            '<div class="form-group">',
            '<label class="col-md-3 control-label"><span style="color:red;margin-right:5px;">*</span>护照号：</label>',
            '<div class="col-md-9">',
            '<input id="identity" type="text"  class="form-control input-inline input-medium" placeholder="护照号、身份证号均可">',
            '<span class="help-inline">  </span>',
            '</div>',
            '</div>',
            '<div class="form-group">',
            '<label class="col-md-3 control-label"><span style="color:red;margin-right:5px;">*</span>生日：</label>',
            '<div class="col-md-9">',
            '<div class="input-group input-medium  input-group-sm ddDatePicker date date-picker  " >',
            '<input type="text" readonly style="background:white" id ="birthday"class="form-control " >',
            '<span class="input-group-btn">',
            '<button class="btn default" type="button">',
            '<i class="fa fa-calendar"></i>',
            '</button>',
            '</span>',
            '</div>',
            '<span class="help-inline"></span>',
            '</div>',
            '</div>',
            '<div class="form-group">',
            '<label class="col-md-3 control-label">性别：</label>',
            '<div class="col-md-9">',
            '<select  id="sex"  type="text" class="form-control input-inline input-medium" >',
            '<option value=0>男</option>',
            '<option value=1>女</option>',
            '</select>',
            '<span class="help-inline"></span>',
            '</div>',
            '</div>',
            '</div>',
            '</form>',
            '</div>',
            '<div class="modal-footer">',
            '<a id="addanewperson" class="btn btn-sm btn-primary button65">确定</a>\n',
            '<a data-dismiss="modal" class="btn btn-sm btn-default button65">取消</a>',
            '</div>',
            '</div>',
            '</div>',
            '</div>',
        ].join('');
        $('body').one('shown.bs.modal', '#addoneperson', function() {
            var this_modal = $(this);
            // $(this).find('.datepicker').datePicker();
            $(this).find("input").each(function() {
                $(this).val('');
            });
            $(this).find('.ddDatePicker').eq(0).datepicker({
                orientation: "top right",
                endDate: '+0d',
                startView: 2
            });
            var thismodal = $(this);
            $(this).find('#addanewperson').one('click', function addOnePersonInit() {
                var data = {};
                data.CustomerID = $('#customerid').text().trim();
                data.traveller = {};
                data.traveller.CustomerID = $('#customerid').text().trim();
                data.traveller.TravellerName = thismodal.find('input#cnname').val().trim();
                data.traveller.TravellerEnname = thismodal.find('input#enname').val().trim();
                data.traveller.PassportNo = thismodal.find('input#identity').val().trim();
                data.traveller.Birthday = thismodal.find('input#birthday').val().trim();
                data.traveller.TravellerSex = thismodal.find('select#sex').val();
                data.traveller.Birthday = thismodal.find('input#birthday').val().trim();

                var cancel = false;
                if (data.traveller.TravellerName == '') {
                    thismodal.find('input#cnname').formWarning({
                        tips: "请填写您的中文姓名"
                    });
                    cancel = true;
                }
                if (data.traveller.TravellerEnname == '') {
                    thismodal.find('input#enname').formWarning({
                        tips: "请填写您的姓名拼音"
                    });
                    cancel = true;
                }
                if (data.traveller.PassportNo == '') {
                    thismodal.find('input#identity').formWarning({
                        tips: "请填写您的护照号或身份证号"
                    });
                    cancel = true;
                }
                if (data.traveller.Birthday == '') {
                    thismodal.find('input#birthday').formWarning({
                        tips: "请填写您的生日 "
                    });
                    cancel = true;
                }
                if (data.traveller.PassportNo != "") {
                    var sexno;
                    if (data.traveller.PassportNo.length == 18) {
                        sexno = data.traveller.PassportNo.substring(16, 17);
                    } else if (data.traveller.PassportNo.length == 15) {
                        sexno = data.traveller.PassportNo.substring(14, 15);
                    } else {}
                    var tempid = sexno % 2;
                    if (tempid == data.traveller.TravellerSex) {
                        thismodal.find('select#sex').formWarning({
                            tips: "您的性别和身份证号码不匹配"
                        });
                        cancel = true;
                    }
                }
                if (cancel) {
                    thismodal.find('#addanewperson').one('click', addOnePersonInit);
                    return;
                }
                $.ajax({
                    url: '/Travellers/AddTraveller',
                    // data:data,
                    type: 'post',
                    contentType: "application/json; charset=utf-8;",
                    data: JSON.stringify(data.traveller),
                    dataType: 'json',
                    success: function(data) {
                        if (data.ErrorCode == 200) {
                            this_modal.modal('hide');
                            addonetolist(listmodal, data.traveller, needextrainfo);
                            listmodal.find("ul.perosnlist").eq(0).find("li").eq(0)
                                .data(
                                    $.extend({},
                                        data.traveller, { 'TravellerID': data.traveller.TravellerID }
                                    )
                                );
                        }
                        if (data.ErrorCode == 401) {
                            jQuery.LangHua.alert({
                                title: "提示信息",
                                tip1: '提示信息：',
                                tip2: data.ErrorMessage,
                                button: '确定'
                            });
                            thismodal.find('#addanewperson').one('click', addOnePersonInit);
                        }
                    },
                    complete: function(XHR, TS) {
                        if (TS !== "success") {
                            thismodal.find('#addanewperson').one('click', addOnePersonInit);
                        }
                    }

                });
            });
            thismodal.find('#cnname').onlyChinese();
            thismodal.find('#enname').onlyChar();
            thismodal.find('#identity').onlyCharNum();

            function addonetolist($listmodal, data, needextrainfo) {
                var travller = data;
                var disabled = needextrainfo == 2 ? "disabled" : "";
                var needAgeObj = {};
                needAgeObj.tips = "";
                needAgeObj.show = 0;
                var needExtraObj = {};
                needExtraObj.tips = "";
                needExtraObj.show = 0;
                var tips = "";
                if (needextrainfo == 2) {
                    tips = 'data-toggle="tooltip" ';
                    needExtraObj.tips = "需要填写附加资料";
                    needExtraObj.show = 1;
                }



                {
                    var arrBirthday = travller.Birthday.split("T")[0].split("-");
                    var birthday = new Date(arrBirthday[0], arrBirthday[1] - 1, arrBirthday[2], 0, 0, 0);
                    var today = new Date();
                    var offYear = today.getFullYear() - birthday.getFullYear();
                    var monBirthday = birthday.getMonth();
                    var dateBirthday = birthday.getDate();
                    var monToday = today.getMonth();
                    var dateToday = today.getDate();
                    var havedBirthday = false; {
                        if (parseInt(monToday) < parseInt(monBirthday)) { //未过
                        } else if (parseInt(monToday) == parseInt(monBirthday)) {
                            if (parseInt(dateToday) >= parseInt(dateBirthday)) {
                                havedBirthday = true;
                            }
                        } else { //已过生日
                            havedBirthday = true;
                        }
                    }
                    var age = offYear;
                    if (!havedBirthday) {
                        age -= 1;
                    }
                    var style = '';
                    if (age >= 60) {
                        style = "color:red;font-weight:bold";
                    }
                    if (
                        (age > ageRange.ageMin) &&
                        (age < ageRange.ageMax)
                    ) {} else {

                        tips = 'data-toggle="tooltip" ';
                        disabled = 'disabled';

                        needAgeObj.show = 1;
                        if (ageRange.ageMin == -Infinity) {
                            needAgeObj.tips = "年龄应小于" + ageRange.ageMax + "岁\n";
                        } else {
                            if (ageRange.ageMax == Infinity) {
                                needAgeObj.tips = "年龄应大于" + ageRange.ageMin + "岁\n";
                            } else {
                                needAgeObj.tips = "年龄应大于" + ageRange.ageMin + "岁，\n且小于" + ageRange.ageMax + "岁\n";
                            }
                        }
                    }

                }

                var str = ['<li id="' + travller.TravellerID + '" class="list-group-item one" >',
                    '<label class="select" ' + tips + '   tid="' + travller.TravellerID + '" index="first"  needextra="' + needExtraObj.show + '" needage="' + needAgeObj.show + '">',
                    '<input ' + disabled + ' type="checkbox" />',
                    '<div style="height:100%;width:100%;position:absolute;top:0px;"></div>',
                    '</label>',

                    '<div class="main"   needextra="' + needExtraObj.tips + '" needage="' + needAgeObj.tips + '">',
                    '<div class="container-fluid">',
                    '<div class="row">',
                    '<div class="col-md-8">',
                    '<div class="font14 bold">',
                    '<span class="cnname">' + travller.TravellerName + '</span> / ',
                    '<span class="enname">' + travller.TravellerEnname + '</span>',
                    '<span style="font-weight:normal">（<span data-agerange=' + (oriAgeRange) + ' class="reviseaperson"><a>修改</a></span>）</span>',

                    '</div>',
                    '<div class="row font12 font-gray">',
                    '<div class="col-md-8">护照：<span class="passport">' + travller.PassportNo + '</span></div>',
                    '<div class="col-md-4">年龄：' + '<span class="age"><span style="' + style + '">' + age + '</span></span>' + '</div>',
                    '</div>',
                    '</div>',
                    '<div class="col-md-4  revise any-unselectable    text-center margin-top-10 margin-bottom-10">',
                    '<a title="' + tips + '">填写附加资料</a>',
                    '</div>',
                    '</div>',
                    '</div>',
                    '</div>',
                    '</li>',
                ].join('');
                $listmodal.find("ul.perosnlist").eq(0).find('.existno').remove();
                $listmodal.find("ul.perosnlist").eq(0).find('label.select[index=first]:eq(0)').attr("index", "");
                $listmodal.find("ul.perosnlist").eq(0).prepend(str).scrollTop(0);
            }
        });

        $(tmp).modal();
    });



    //附加资料
    $('body').on('click', '.revise', function() {
        var thisperson = $(this).closest('li');

        $('body').one('shown.bs.modal', '#extraInfos', function() {


            var this_modal = $(this);
            var this_extralinfo = $(this);


            this_modal.find('#Height').onlyNumWithEmpty();
            this_modal.find('#Weight').onlyNumWithEmpty();
            this_modal.find('#ShoesSize').onlyNumWithEmpty();
            // this_modal.find('#ClothesSize').onlyNumWithEmpty();
            this_modal.find('#GlassesNum').onlyNumWithEmpty();


            $(this).find("#confirm").bind('click', function() {
                var data = {
                    "TravellerID": lastData.TravellerID,
                    TravellerDetail: {
                        "Height": this_extralinfo.find("#Height").val() || null,
                        "Weight": this_extralinfo.find("#Weight").val() || null,
                        "ShoesSize": this_extralinfo.find("#ShoesSize").val() || null,
                        "ClothesSize": this_extralinfo.find("#ClothesSize").val() || null,
                        "GlassesNum": this_extralinfo.find("#GlassesNum").val() || null
                    }
                };
                var cancel = false;
                if (data.TravellerDetail.Height == null

                ) {
                    this_modal.find('input#Height').formWarning({
                        tips: "请填写您的身高 "
                    });
                    cancel = true;


                }
                if (
                    data.TravellerDetail.Weight == null

                ) {
                    this_modal.find('input#Weight').formWarning({
                        tips: "请填写您的体重"
                    });
                    cancel = true;

                }
                if (
                    data.TravellerDetail.ShoesSize == null
                ) {

                    this_modal.find('input#ShoesSize').formWarning({
                        tips: "请填写您的鞋子码数"
                    });
                    cancel = true;

                }
                if (cancel) {
                    this_modal.closest('.modal-scrollable').scrollTop(0);
                    return;
                }
                $.ajax({
                    url: window.location.origin + '/Travellers/EditTravellerDetail',
                    type: 'post',
                    contentType: "application/json; charset=utf-8;",
                    dataType: 'json',
                    data: JSON.stringify(data),
                    success: function(datacallback) {
                        if (datacallback.ErrorCode == 200) {
                            this_modal.modal('hide');
                            thisperson.data(datacallback.travellerOld);
                            thisperson.find(".revise").replaceWith(
                                [
                                    '<div class="col-md-4  revise any-unselectable    text-center margin-top-10 margin-bottom-10">',
                                    '<span class="dark">已填写(</span>',
                                    '<a>修改</a>',
                                    '<span class="dark">',
                                    ')',
                                    '</span>',
                                    '</div>'
                                ].join('\n')
                            );
                            thisperson.find(thisperson.find(".select").attr("needextra", '0'))
                            thisperson.find(thisperson.find(".main").attr("needextra", ''))
                            if (thisperson.find(".select").attr("needage") == 0) {
                                thisperson.find("input[type=checkbox]").removeAttr("disabled");
                                thisperson.find(".select").attr("data-toggle", '');
                            }
                        }
                    },
                    error: function() {

                    }
                });
            });
        });



        var lastData = thisperson.data();
        var height = lastData.TravellerDetail.Height || '';
        var weight = lastData.TravellerDetail.Weight || '';
        var shoesSize = lastData.TravellerDetail.ShoesSize || '';
        var clothSize = lastData.TravellerDetail.ClothesSize || '';
        var degreeG = lastData.TravellerDetail.GlassesNum || '';
        var TravellerName = lastData.TravellerName || "";

        var tmp = [
            '<div id="extraInfos" class="modal modal-animate" data-backdrop="static" tabindex="-1" >',
            '<div class="modal-dialog " role="document">',
            '<div class="modal-content">',
            '<div class="modal-header">',
            '<button type="button" class="close" data-dismiss="modal" aria-hidden="true">×</button>',
            '<h4 class="modal-title">填写附加资料</h4>',
            '</div>',
            '<div class="modal-body">',
            '<div style="margin-bottom:10px">为了提前准备深潜装备，您需要为<span style="font-weight:bold">' + TravellerName + '</span>填写附加资料</div>',
            '<form class="form-horizontal" role="form">',
            '<div class="form-body">',
            '<div class="form-group">',
            '<label class="col-md-3 control-label"><span style="color:red;margin-right:5px;">*</span>身高</label>',
            '<div class="col-md-9">',
            '<input id="Height"  type="text" class="form-control input-inline input-small input80" value="' + height + '" placeholder="如：175"/>',
            '<span class="help-inline">CM</span>',
            '</div>',
            '</div>',
            '<div class="form-group">',
            '<label class="col-md-3 control-label"><span style="color:red;margin-right:5px;">*</span>体重 </label>',
            '<div class="col-md-9">',
            '<input id="Weight" type="text" class="form-control input-inline input-small input80" value="' + weight + '" placeholder=如：65 />',
            '<span class="help-inline">KG</span>',
            '</div>',
            '</div>',
            '<div class="form-group">',
            '<label class="col-md-3 control-label"><span style="color:red;margin-right:5px;">*</span>鞋子码数</label>',
            '<div class="col-md-9">',
            '<input id="ShoesSize" type="text" class="form-control input-inline input-small input80" value="' + shoesSize + '" placeholder="如：43">',
            '<span class="help-inline">  </span>',
            '</div>',
            '</div>',
            '<div class="form-group">',
            '<label class="col-md-3 control-label"></span>衣服码数</label>',
            '<div class="col-md-9">',
            '<input id="ClothesSize" type="text" class="form-control input-inline input-small input80" value="' + clothSize + '" placeholder="如：XL">',
            '<span class="help-inline">  </span>',
            '</div>',
            '</div>',
            '<div class="form-group">',
            '<label class="col-md-3 control-label"></span>近视度数</label>',
            '<div class="col-md-9">',
            '<input id="GlassesNum" type="text" class="form-control input-inline input-small input80" value="' + degreeG + '" placeholder="如：200">',
            '<span class="help-inline">  </span>',
            '</div>',
            '</div>',

            '</div>',
            '</form>',


            '</div>',
            '<div class="modal-footer">',
            '<a id="confirm" class="btn btn-sm btn-primary button70 ">确定</a>\n',
            '<a data-dismiss="modal" class="btn btn-sm btn-default button70 i>取消</a>',
            '</div>',
            '</div>',
            '</div>',
            '</div>'
        ].join('');
        var modalEditExtra = $(tmp).modal();
        if (isClient === true) {
            if (!($.LangHuaCookie.get("AGREEDFLIGHTDIVING"))) {
                $('#agreeFlightDiving #confirmagree').text("确定（5）").attr('disabled', "disabled");
                $('#agreeFlightDiving #remberMe').prop("checked", false);
                var importantNotice = $('#agreeFlightDiving').modal();
                var count = 4;
                var INTx = setInterval(function() {
                    if (count === -1) {
                        $('#agreeFlightDiving #confirmagree').text("确定").removeAttr("disabled");
                        clearInterval(INTx);
                        return;
                    }
                    $('#agreeFlightDiving #confirmagree').text("确定（" + count + "）");
                    count--;

                }, 1000);
                $("#agreeFlightDiving #cancelagree").one("click", { "modal": modalEditExtra, "int": INTx }, function(e) {
                    e.data.modal.modal("hide");
                    clearInterval(e.data.int);
                });
                $("#agreeFlightDiving #confirmagree").one("click", { "modal": importantNotice, "int": INTx }, function agreed(e) {
                    var _this = $(this);
                    if ($(this).attr("disabled") !== "disabled") {
                        if (_this.closest("#agreeFlightDiving").find("#remberMe").prop("checked")) {
                            $.LangHuaCookie.set("AGREEDFLIGHTDIVING", "1", 24 * 30, "/");
                        }
                        e.data.modal.modal("hide");

                    } else {
                        _this.one("click", e.data, agreed);
                    }
                });
            }
        }

    });


    //修改资料


    $('body').on('click', '.reviseaperson', function() {
        var thisPerson = $(this).closest('li');
        var fullInfo = $(this).closest("li.one").data();
        var forreact = $(this).closest("#personList").data("forreact");
        var ageRange = JSON.parse($(this).attr("data-agerange"));

        ageRange.ageMin = ((ageRange.ageMin == -1) ? -Infinity : parseInt(ageRange.ageMin));
        ageRange.ageMax = ((ageRange.ageMax == -1) ? Infinity : parseInt(ageRange.ageMax));
        var tmp = [
            '<div id="reviseoneperson" class="modal modal-animate" data-backdrop="static" tabindex="-1" >',
            '<div class="modal-dialog " role="document">',
            '<div class="modal-content">',
            '<div class="modal-header">',
            '<button type="button" class="close" data-dismiss="modal" aria-hidden="true">×</button>',
            '<h4 class="modal-title">修改常用游客资料</h4>',
            '</div>',
            '<div class="modal-body">',
            '<form class="form-horizontal" role="form">',
            '<div class="form-body">',

            '<div class="form-group">',
            '<label class="col-md-3 control-label"><span style="color:red;margin-right:5px;">*</span>中文姓名：</label>',
            '<div class="col-md-9">',
            '<input id="cnname" type="text" class="form-control input-inline input-medium" placeholder="张三">',
            '<span class="help-inline">  </span>',
            '</div>',
            '</div>',
            '<div class="form-group">',
            '<label class="col-md-3 control-label"><span style="color:red;margin-right:5px;">*</span>姓名拼音：</label>',
            '<div class="col-md-9">',
            '<input id="enname" type="text" class="form-control input-inline input-medium" placeholder="ZHANGSAN">',
            '<span class="help-inline">  </span>',
            '</div>',
            '</div>',

            '<div class="form-group">',
            '<label class="col-md-3 control-label"><span style="color:red;margin-right:5px;">*</span>护照号：</label>',
            '<div class="col-md-9">',
            '<input id="identity" type="text"  class="form-control input-inline input-medium" placeholder="护照号、身份证号均可">',
            '<span class="help-inline">  </span>',
            '</div>',
            '</div>',
            '<div class="form-group">',
            '<label class="col-md-3 control-label"><span style="color:red;margin-right:5px;">*</span>生日：</label>',
            '<div class="col-md-9">',
            '<div id="ddDatePicker" class="input-group input-medium  input-group-sm ddDatePicker date date-picker  " >',
            '<input type="text" readonly style="background:white"  id ="birthday" class="form-control " >',
            '<span class="input-group-btn">',
            '<button class="btn default" type="button">',
            '<i class="fa fa-calendar"></i>',
            '</button>',
            '</span>',
            '</div>',
            '<span class="help-inline">  </span>',
            '</div>',
            '</div>',
            '<div class="form-group">',
            '<label class="col-md-3 control-label">性别：</label>',
            '<div class="col-md-9">',
            '<select  id="sex"  type="text" class="form-control input-inline input-medium" >',
            '<option value=0>男</option>',
            '<option value=1>女</option>',
            '</select>',
            '<span class="help-inline">  </span>',
            '</div>',
            '</div>',

            '</div>',
            '</form>',
            '</div>',
            '<div class="modal-footer">',
            '<span style="line-height:30px;float:left;padding-left:5px"><a id="deleteOnePerson" class="">删除</a></span>\n',
            '<a id="addanewperson" class="btn btn-sm btn-primary button65">确定</a>\n',
            '<a data-dismiss="modal" class="btn btn-sm btn-default button65">取消</a>',
            '</div>',
            '</div>',
            '</div>',
            '</div>',

        ].join('');
        $('body').one('shown.bs.modal', '#reviseoneperson', function() {
            var thismodal = $(this);
            thismodal.find('input#cnname').val(fullInfo.TravellerName);
            thismodal.find('input#enname').val(fullInfo.TravellerEnname);
            thismodal.find('input#identity').val(fullInfo.PassportNo);
            thismodal.find('input#birthday').val(fullInfo.Birthday.toString().substr(0, 10));
            thismodal.find('select#sex').val(fullInfo.TravellerSex);


            $(this).find('#ddDatePicker').datepicker({
                orientation: "top right",
                endDate: '+0d',
                startView: 2,
                // container:'#reviseoneperson'
            });

            $(this).find('#addanewperson').one('click', function addOnePersonInit() {
                var data = {};
                data.TravellerID = thisPerson.attr('id');
                data.TravellerName = thismodal.find('input#cnname').val().trim();
                data.TravellerEnname = thismodal.find('input#enname').val().trim();
                data.PassportNo = thismodal.find('input#identity').val().trim();
                data.Birthday = thismodal.find('input#birthday').val().trim();
                data.TravellerSex = thismodal.find('select#sex').val();


                var cancel = false;
                if (data.TravellerName == '') {
                    thismodal.find('input#cnname').formWarning({
                        tips: "请填写您的中文姓名"
                    });
                    cancel = true;
                }
                if (data.TravellerEnname == '') {
                    thismodal.find('input#enname').formWarning({
                        tips: "请填写您的姓名拼音"
                    });
                    cancel = true;
                }
                if (data.PassportNo == '') {
                    thismodal.find('input#identity').formWarning({
                        tips: "请填写您的护照号 "
                    });
                    cancel = true;
                }
                if (data.Birthday == '') {
                    thismodal.find('input#birthday').formWarning({
                        tips: "请填写您的生日 "
                    });
                    cancel = true;
                }
                if (data.PassportNo != "") {
                    var sexno;
                    if (data.PassportNo.length == 18) {
                        sexno = data.PassportNo.substring(16, 17);
                    } else if (data.PassportNo.length == 15) {
                        sexno = data.PassportNo.substring(14, 15);
                    } else {}
                    var tempid = sexno % 2;
                    if (tempid == data.TravellerSex) {
                        thismodal.find('select#sex').formWarning({
                            tips: "您的性别和身份证号码不匹配"
                        });
                        cancel = true;
                    }
                }
                if (cancel) {
                    thismodal.find('#addanewperson').one('click', addOnePersonInit);
                    return;

                }
                var tips = '';
                $.ajax({
                    url: '/Travellers/EditTraveller',
                    type: 'post',
                    contentType: "application/json; charset=utf-8;",
                    data: JSON.stringify(data),
                    beforeSend: function() {
                        t = $.LangHua.loadingToast({
                            tip: '正在保存'
                        });
                    },

                    dataType: 'json',
                    success: function(dataR) {
                        if (dataR.ErrorCode == 200) {
                            thismodal.modal('hide');
                            thisPerson.data(dataR.traveller); {
                                thisPerson.find('.cnname:eq(0)').text(dataR.traveller.TravellerName);
                                thisPerson.find('.enname:eq(0)').text(dataR.traveller.TravellerEnname);
                                thisPerson.find('.passport:eq(0)').text(dataR.traveller.PassportNo);


                                {
                                    var arrBirthday = dataR.traveller.Birthday.split("T")[0].split("-");
                                    var birthday = new Date(arrBirthday[0], arrBirthday[1] - 1, arrBirthday[2], 0, 0, 0);
                                    var today = new Date();
                                    var offYear = today.getFullYear() - birthday.getFullYear();
                                    var monBirthday = birthday.getMonth();
                                    var dateBirthday = birthday.getDate();
                                    var monToday = today.getMonth();
                                    var dateToday = today.getDate();
                                    var havedBirthday = false; {
                                        if (parseInt(monToday) < parseInt(monBirthday)) { //未过
                                        } else if (parseInt(monToday) == parseInt(monBirthday)) {
                                            if (parseInt(dateToday) >= parseInt(dateBirthday)) {
                                                havedBirthday = true;
                                            }
                                        } else { //已过生日
                                            havedBirthday = true;
                                        }
                                    }
                                    var age = offYear;
                                    if (!havedBirthday) {
                                        age -= 1;
                                    }
                                    var style = '';
                                    if (age >= 60) {
                                        var style = "color:red;font-weight:bold";

                                    }

                                    var needAgeObj = new Object();
                                    needAgeObj.show = 0;
                                    needAgeObj.tips = '';
                                    var disabled = '';
                                    var tips = "";
                                    if (
                                        (age > ageRange.ageMin) &&
                                        (age < ageRange.ageMax)
                                    ) {} else {

                                        tips = 'data-toggle="tooltip" ';
                                        disabled = 'disabled';

                                        needAgeObj.show = 1;
                                        if (ageRange.ageMin == -Infinity) {
                                            needAgeObj.tips = "年龄应小于" + ageRange.ageMax + "岁\n";
                                        } else {
                                            if (ageRange.ageMax == Infinity) {
                                                needAgeObj.tips = "年龄应大于" + ageRange.ageMin + "岁\n";
                                            } else {
                                                needAgeObj.tips = "年龄应大于" + ageRange.ageMin + "岁，\n且小于" + ageRange.ageMax + "岁\n";
                                            }
                                        }
                                    }

                                }
                                thisPerson.find('.age:eq(0)').empty().append('<span style="' + style + '">' + age + '</span>');
                                thisPerson.find('label:eq(0)').attr('needage', needAgeObj.show);
                                thisPerson.find('.main:eq(0)').attr('needage', needAgeObj.tips);
                                if (disabled == "") {
                                    if (thisPerson.find('label:eq(0)').attr('needextra') == 0) {
                                        thisPerson.find('label:eq(0)').attr('data-toggle', "");
                                        thisPerson.find('label:eq(0) input').removeAttr("disabled");
                                    }
                                } else {
                                    thisPerson.find('label:eq(0)').attr('data-toggle', 'tooltip');
                                    thisPerson.find('label:eq(0) input').prop("disabled", true).prop("checked", false).trigger("change");
                                }
                            }
                        }
                        if (dataR.ErrorCode == 401) {
                            jQuery.LangHua.alert({
                                title: "提示信息",
                                tip1: '提示信息：',
                                tip2: dataR.ErrorMessage,
                                button: '确定'
                            });
                            thismodal.find('#addanewperson').one('click', addOnePersonInit);

                        }
                    },
                    complete: function(XHR, TS) {
                        if (TS !== "success") {
                            thismodal.find('#addanewperson').one('click', addOnePersonInit);
                        }
                        t.modal('hide');
                        t = null;

                    }

                });
            });
            thismodal.find('#cnname').onlyChinese();
            thismodal.find('#enname').onlyChar();
            thismodal.find('#identity').onlyCharNum();
            thismodal.find('#deleteOnePerson').data({
                forreact: forreact,
                fullinfo: fullInfo
            });
        });

        $(tmp).modal();
    });

    //删除
    $('body').on('click', '#deleteOnePerson', function() {
        var thisPerson = $(this).data('fullinfo');
        var forreact = $(this).data('forreact');
        var lastModal = $(this).closest(".modal")
        $.LangHua.confirm({
            title: "提示信息",
            tip1: '删除确认：',
            tip2: '你确认要删除<span style="color:red">' + thisPerson.TravellerEnname + ' / ' + thisPerson.TravellerName + '</span>？<br/>删除后不可恢复，请慎重！',
            confirmbutton: '确定',
            cancelbutton: '取消',
            indent: false,
            confirm: function() {
                $.ajax({
                    url: '/Travellers/DelTraveller/' + thisPerson.TravellerID,
                    type: 'post',
                    contentType: "application/json; charset=utf-8;",
                    beforeSend: function() {
                        t = $.LangHua.loadingToast({
                            tip: '正在删除'
                        });
                    },

                    dataType: 'json',
                    success: function(data) {
                        if (data.ErrorCode == 200) {
                            // $('body').trigger("addapersontolist",listmodal,[data.traveller]);
                            $.LangHua.alert({
                                title: "提示信息",
                                tip1: '删除结果',
                                tip2: '删除<span style="color:red">' + thisPerson.TravellerEnname + '/' + thisPerson.TravellerName + '</span>成功',
                                button: '确定',
                                data: thisPerson,
                                callback: function(thisPerson) {
                                    lastModal.find("a[data-dismiss=modal]:eq(0)").trigger('click');
                                    $('body').find('li#' + thisPerson.TravellerID).remove();


                                    var count = $('body').find('#personList').find('input:checked').length;
                                    $('body').find('#personList div.selectedtips').text('已选' + count + '个');

                                    forreact.trigger('delete', [
                                        [
                                            thisPerson
                                        ]
                                    ]);

                                }

                            });

                        }
                        if (data.ErrorCode == 401) {
                            jQuery.LangHua.alert({
                                title: "提示信息",
                                tip1: '提示信息：',
                                tip2: data.ErrorMessage,
                                button: '确定'
                            });


                        }
                    },
                    complete: function(XHR, TS) {
                        if (TS !== "success") {}
                        t.modal('hide');
                        t = null;
                        thisPerson = null;

                    }

                });
            }

        });
    });
});