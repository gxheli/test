jQuery(document).ready(function($) {








    //新添加联系人
    $('body').on('click', '#toaddaperson', function() {
        var listmodal = $(this).closest('#personList');
        var tmp = [
            '<div id="addoneperson" class="modal modal-animate" data-backdrop="static" tabindex="-1" >',
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
            '<input id="identity" type="text"  class="form-control input-inline input-medium" placeholder="护照号">',
            '<span class="help-inline">  </span>',
            '</div>',
            '</div>',
            '<div class="form-group">',
            '<label class="col-md-3 control-label"><span style="color:red;margin-right:5px;">*</span>生日：</label>',
            '<div class="col-md-9">',
            '<div id="datepicker-birthday" class="input-group input-medium  input-group-sm ddDatePicker date date-picker  " >',
            '<input  readonly style="background:white" type="text"  id ="birthday" class="form-control " >',
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
            '<a id="addanewperson" class="btn btn-sm btn-primary button65">确定</a>\n',
            '<a data-dismiss="modal" class="btn btn-sm btn-default button65">取消</a>',
            '</div>',
            '</div>',
            '</div>',
            '</div>',

        ].join('');
        $('body').one('shown.bs.modal', '#addoneperson', function() {
            var this_modal = $(this);

            $(this).find("input").each(function() {
                $(this).val('');
            })
            $(this).find('#datepicker-birthday').datepicker({
                orientation: "top right",
                language: 'zh-CN',
                endDate: '+0d',
                startView: 2,
                container: '.modal-scrollable'
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
                        tips: "请填写您的护照号 "
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
                var tips = '';
                $.ajax({
                    url: window.location.origin + '/Travellers/AddTraveller',
                    // data:data,
                    type: 'post',
                    contentType: "application/json; charset=utf-8;",
                    data: JSON.stringify(data.traveller),
                    beforeSend: function() {
                        t = $.LangHua.loadingToast({
                            tip: '正在保存'
                        })
                    },

                    dataType: 'json',
                    success: function(data) {
                        if (data.ErrorCode == 200) {
                            this_modal.modal('hide');
                            $.LangHua.alert({
                                title: "提示信息",
                                tip1: '<span class="font20">保存成功！</span>',
                                tip2: '如有行程需要用到刚才保存的游客资料，<span style="color:red">请通知客服</span>为您重新选择游客资料，否则刚才的游客资料无法体现在原有订单中。',
                                button: '确定',
                                indent: false,
                                callback: function() {
                                    window.location.reload();
                                }
                            })

                        }
                        if (data.ErrorCode == 401) {
                            jQuery.LangHua.alert({
                                title: "提示信息",
                                tip1: '提示信息：',
                                tip2: data.ErrorMessage,
                                button: '确定'
                            })
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

                })
            })

            thismodal.find('#cnname').onlyChinese();
            thismodal.find('#enname').onlyChar();
            thismodal.find('#identity').onlyCharNum();


        })

        $(tmp).modal()
    })



    //附加资料
    $('body').on('click', '.revise', function() {
        var this_detail = $(this).closest('td');

        $('body').one('shown.bs.modal', '#extraInfos', function() {


            var this_modal = $(this);
            var this_extralinfo = $(this);


            this_modal.find('#Height').onlyNumWithEmpty();
            this_modal.find('#Weight').onlyNumWithEmpty();
            this_modal.find('#ShoesSize').onlyNumWithEmpty();
            // this_modal.find('#ClothesSize').onlyNumWithEmpty();
            this_modal.find('#GlassesNum').onlyNumWithEmpty();


            $(this).find("#confirm").one('click', function c() {
                var data = {
                    "TravellerID": this_detail.closest('tr').attr('id'),
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
                    this_modal.find("#confirm").one('click', c);
                    return;
                }
                $.ajax({
                    url: window.location.origin + '/Travellers/EditTravellerDetail',
                    type: 'post',
                    contentType: "application/json; charset=utf-8;",
                    dataType: 'json',
                    data: JSON.stringify(data),
                    success: function(data) {
                        if (data.ErrorCode == 200) {
                            this_detail.data("detail", data.travellerOld.TravellerDetail);
                            this_modal.modal('hide');
                            $.LangHua.alert({
                                title: "提示信息",
                                tip1: '<span class="font20">保存成功！</span>',
                                tip2: '如有行程需要用到刚才保存的游客资料，<span style="color:red">请通知客服</span>为您重新选择游客资料，否则刚才的游客资料无法体现在原有订单中。',
                                button: '确定',
                                indent: false,
                            })
                        } else if (data.ErrorCode == 401) {
                            $.LangHua.alert({

                            })
                        }
                    },
                    error: function() {
                        this_modal.find("#confirm").one('click', c);
                    }
                })
            })



        })

        var detail = $(this).closest('td').data('detail');


        var height = detail.Height || '';
        var weight = detail.Weight || '';
        var shoesSize = detail.ShoesSize || '';
        var clothSize = detail.ClothesSize || '';
        var degreeG = detail.GlassesNum || '';
        var TravellerName = detail.TravellerName || "";

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
            '<span class="help-inline"></span>',
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
        console.log("sfs")
        if (!($.LangHuaCookie.get("AGREEDFLIGHTDIVING"))) {
            console.log('sfs')
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
    })

    //  删除联系人
    $('body').on('click', '.delete', function() {
        var thisTr = $(this).closest('tr');
        var personId = $(this).closest('tr').attr('id');
        var cnName = $(this).closest('tr').find("td:eq(1)").text();
        var enName = $(this).closest('tr').find("td:eq(2)").text();
        $.LangHua.confirm({

            title: "提示信息",
            tip1: '删除确认：',
            tip2: '你确认要删除<span style="color:red">' + cnName + '</span>吗？',
            confirmbutton: '确定',
            cancelbutton: '取消',
            data: personId,
            confirm: function(thisbutton, personId) {
                $.ajax({
                    url: '/Travellers/DelTraveller/' + personId,
                    type: 'post',
                    contentType: "application/json; charset=utf-8;",
                    beforeSend: function() {
                        t = $.LangHua.loadingToast({
                            tip: '正在删除'
                        })
                    },

                    dataType: 'json',
                    success: function(data) {
                        if (data.ErrorCode == 200) {
                            // $('body').trigger("addapersontolist",listmodal,[data.traveller]);
                            $.LangHua.alert({
                                title: "提示信息",
                                tip1: '删除结果',
                                tip2: '删除<span style="color:red">' + cnName + '</span>成功',
                                button: '确定',
                                callback: function() {
                                    thisTr.remove();
                                }

                            })

                        }
                        if (data.ErrorCode == 401) {
                            jQuery.LangHua.alert({
                                title: "提示信息",
                                tip1: '提示信息：',
                                tip2: data.ErrorMessage,
                                button: '确定'
                            })


                        }
                    },
                    complete: function(XHR, TS) {
                        if (TS !== "success") {

                        }
                        t.modal('hide');
                        t = null;

                    }

                })
            }
        })
    })


    //修改联系人
    $('body').on('click', '.reviseaperson', function() {
        var thisTr = $(this).closest('tr');
        var fullInfo = $(this).data("full-info");
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
            '<input id="identity" type="text"  class="form-control input-inline input-medium" placeholder="护照号">',
            '<span class="help-inline">  </span>',
            '</div>',
            '</div>',
            '<div class="form-group">',
            '<label class="col-md-3 control-label"><span style="color:red;margin-right:5px;">*</span>生日：</label>',
            '<div class="col-md-9">',
            '<div id="ddDatePicker" class="input-group input-medium  input-group-sm ddDatePicker date date-picker  " >',
            '<input type="text" readonly style="background:white" id ="birthday" class="form-control " >',
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
                data.TravellerID = thisTr.attr('id');
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
                        })
                    },

                    dataType: 'json',
                    success: function(data) {
                        if (data.ErrorCode == 200) {
                            thismodal.modal('hide');
                            $.LangHua.alert({
                                title: "提示信息",
                                tip1: '<span class="font20">保存成功！</span>',
                                tip2: '如有行程需要用到刚才保存的游客资料，<span style="color:red">请通知客服</span>为您重新选择游客资料，否则刚才的游客资料无法体现在原有订单中。',
                                button: '确定',
                                indent: false,
                                callback: function() {
                                    window.location.reload();
                                    thismodal.modal('hide')
                                }
                            })

                        }
                        if (data.ErrorCode == 401) {
                            jQuery.LangHua.alert({
                                title: "提示信息",
                                tip1: '提示信息：',
                                tip2: data.ErrorMessage,
                                button: '确定'
                            })
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

                })
            })
            thismodal.find('#cnname').onlyChinese();
            thismodal.find('#enname').onlyChar();
            thismodal.find('#identity').onlyCharNum();
        })

        $(tmp).modal()
    })


    //修改基本信息
    $('body').on('click', '.reviseBasicInfo', function() {
        var thisTr = $(this).closest('tr');
        var fullInfo = $(this).data("full-info");
        var tmp = [
            '<div id="reviseoneperson" class="modal modal-animate" data-backdrop="static" tabindex="-1" >',
            '<div class="modal-dialog " role="document">',
            '<div class="modal-content">',
            '<div class="modal-header">',
            '<button type="button" class="close" data-dismiss="modal" aria-hidden="true">×</button>',
            '<h4 class="modal-title">修改基本信息</h4>',
            '</div>',
            '<div class="modal-body">',
            '<div style="margin-bottom:10px"> 此个人资料会在填写订单信息时帮您默认填写。</div>',
            '<form class="form-horizontal" role="form">',
            '<div class="form-body">',

            '<div class="form-group">',
            '<label class="col-md-4 control-label"><span style="color:red;margin-right:5px;">*</span>中文姓名：</label>',
            '<div class="col-md-8">',
            '<input id="CustomerName" class="form-control input-inline input-medium"  placeholder="张三" type="text">',
            '<span class="help-inline">  </span>',
            '</div>',
            '</div>',
            '<div class="form-group">',
            '<label class="col-md-4 control-label"><span style="color:red;margin-right:5px;">*</span>姓名拼音：</label>',
            '<div class="col-md-8">',
            '<input id="CustomerEnname" class="form-control input-inline input-medium"  placeholder="ZHANGSAN" type="text">',
            '<span class="help-inline">  </span>',
            '</div>',
            '</div>',

            '<div class="form-group">',
            '<label class="col-md-4 control-label"><span style="color:red;margin-right:5px;">*</span>联系电话：</label>',
            '<div class="col-md-8">',
            '<input id="Tel" class="form-control input-inline input-medium"  placeholder="" type="text">',
            '<span class="help-inline">  </span>',
            '</div>',
            '</div>',

            '<div class="form-group">',
            '<label class="col-md-4 control-label">备用联系电话：</label>',
            '<div class="col-md-8">',
            '<input id="BakTel" class="form-control input-inline input-medium"  placeholder="" type="text">',
            '<span class="help-inline">  </span>',
            '</div>',
            '</div>',
            '<div class="form-group">',
            '<label class="col-md-4 control-label"><span style="color:red;margin-right:5px;">*</span>Email地址：</label>',
            '<div class="col-md-8">',
            '<input id="Email" class="form-control input-inline input-medium"  placeholder="" type="text">',
            '<span class="help-inline">  </span>',
            '</div>',
            '</div>',
            '<div class="form-group">',
            '<label class="col-md-4 control-label"><span style="color:red;margin-right:5px;">*</span>微信号：</label>',
            '<div class="col-md-8">',
            '<input id="Wechat" class="form-control input-inline input-medium"  placeholder="" type="text">',
            '<span class="help-inline">  </span>',
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
        $('body').one('shown.bs.modal', '#reviseoneperson', function() {
            var thismodal = $(this);
            console.log(fullInfo)
            thismodal.find('input#CustomerName').val(fullInfo.CustomerName);
            thismodal.find('input#CustomerEnname').val(fullInfo.CustomerEnname);
            thismodal.find('input#Tel').val(fullInfo.Tel);
            thismodal.find('input#BakTel').val(fullInfo.BakTel);
            thismodal.find('input#Email').val(fullInfo.Email);
            thismodal.find('input#Wechat').val(fullInfo.Wechat);




            $(this).find('#addanewperson').one('click', function reviseOnePersonInit() {
                var data = new Object();
                data.CustomerName = thismodal.find('input#CustomerName').val().trim();
                data.CustomerEnname = thismodal.find('input#CustomerEnname').val().trim();
                data.Tel = thismodal.find('input#Tel').val().trim();
                data.BakTel = thismodal.find('input#BakTel').val().trim();
                data.Email = thismodal.find('input#Email').val().trim();
                data.Wechat = thismodal.find('input#Wechat').val().trim();

                var cancel = false;

                // 名字
                if (data.CustomerName) {
                    if (!isChineseChar(data.CustomerName)) {
                        cancel = true;
                        $('#CustomerName').formWarning({
                            tips: "格式不对"
                        });
                    }
                } else {
                    cancel = true;
                    $('#CustomerName').formWarning({
                        tips: "请您填写"
                    });
                }
                // 拼音
                if (data.CustomerEnname) {
                    if (!isEnglishChar(data.CustomerEnname)) {
                        cancel = true;
                        $('#CustomerEnname').formWarning({
                            tips: "格式不对"
                        });
                    }
                } else {
                    cancel = true;
                    $('#CustomerEnname').formWarning({
                        tips: "请您填写"
                    });
                }

                // 电话
                if (data.Tel) {

                } else {
                    cancel = true;
                    $('#Tel').formWarning({
                        tips: "请您填写"
                    });
                }

                // 微信
                if (data.Wechat) {
                    // if(!isWechat(data.Wechat)){
                    //     cancel = true;
                    //     $('#Wechat').warning("格式不对");
                    // }
                } else {
                    cancel = true;
                    $('#Wechat').formWarning({
                        tips: "请您填写"
                    });
                }
                // email
                if (data.Email) {
                    if (!isEmail(data.Email)) {
                        cancel = true;
                        $('#Email').formWarning({
                            tips: "格式不对"
                        });
                    }
                } else {
                    cancel = true;
                    $('#Email').formWarning({
                        tips: "请您填写"
                    });
                }

                if (cancel) {
                    thismodal.find('#addanewperson').one('click', reviseOnePersonInit);
                    return;

                }
                data.CustomerID = $('#customerid').text();
                var tips = '';
                $.ajax({
                    url: '/Customers/Edit',
                    type: 'post',
                    contentType: "application/json; charset=utf-8;",
                    data: JSON.stringify(data),
                    beforeSend: function() {
                        t = $.LangHua.loadingToast({
                            tip: '正在保存修改....'
                        })
                    },

                    dataType: 'json',
                    success: function(data) {
                        if (data.ErrorCode == 200) {
                            thismodal.modal('hide');
                            $.LangHua.alert({
                                title: "提示信息",
                                tip1: '内容保存成功！',
                                indent: false,
                                tip2: '最新信息会在下一次填写订单时默认为您填好。修改前已填写的订单不会发生变化，但请您放心，您的最新资料我们都可以查到。',
                                button: '确定',
                                callback: function() {
                                    window.location.reload();
                                }
                            })

                        }
                        if (data.ErrorCode == 401) {
                            jQuery.LangHua.alert({
                                title: "提示信息",
                                tip1: '提示信息：',
                                tip2: data.ErrorMessage,
                                button: '确定'
                            })
                            thismodal.find('#addanewperson').one('click', reviseOnePersonInit);

                        }
                    },
                    complete: function(XHR, TS) {
                        if (TS !== "success") {
                            thismodal.find('#addanewperson').one('click', reviseOnePersonInit);
                        }
                        t.modal('hide');
                        t = null;
                    }

                })
            })

            thismodal.find('#CustomerEnname').onlyCapchar();
            thismodal.find('#CustomerName').onlyChinese();
            thismodal.find('#Tel').onlyNumWithEmpty();


        })

        $(tmp).modal()
    })

    // 修改密码 
    $('body').on('shown', '#password-revise', function() {
        var thismodal = $(this);
        thismodal.find('input').each(function() {
            $(this).val("");
        })
        thismodal.find('span.tips').removeClass("tips").empty();
    }).one("click", '#passwrod-revise-confirm', function confirmPassWord() {
        var thisbutton = $(this);
        var passwordOld = jQuery.trim(thisbutton.closest('#password-revise').find("#password-old-r").val());
        var passwordNew = jQuery.trim(thisbutton.closest('#password-revise').find("#password-new").val());
        var passwordNewConfirm = jQuery.trim(thisbutton.closest('#password-revise').find("#password-revise-confirm").val());
        if (!passwordOld) {
            thisbutton.closest('#password-revise').find("#password-old-r").formWarning({
                tips: '请您填写旧密码'
            });
            $('body').one("click", '#passwrod-revise-confirm', confirmPassWord);
            return;
        }
        if (!passwordNew) {
            thisbutton.closest('#password-revise').find("#password-new").formWarning({
                tips: "请您填新密码"
            });
            $('body').one("click", '#passwrod-revise-confirm', confirmPassWord);
            return;
        } else {
            if (passwordNew != passwordNewConfirm) {
                thisbutton.closest('#password-revise').find("#password-revise-confirm").formWarning({
                    tips: "两次填写密码不一致"
                });
                $('body').one("click", '#passwrod-revise-confirm', confirmPassWord);

                return
            }
            if (passwordNew == passwordOld) {
                thisbutton.closest('#password-revise').find("#password-new").formWarning({
                    tips: '新密码和旧密码一样'
                });
                $('body').one("click", '#passwrod-revise-confirm', confirmPassWord);

                return
            }
        }
        var postJson = new Object();
        postJson.CustomerID = $.trim($('#customerid').text());
        postJson.oldPassword = passwordOld;
        postJson.newPassword = passwordNew;
        var loading = '';
        $.ajax({
            url: '/Customers/EditPassword',
            type: 'post',
            contentType: "application/json; charset=utf-8;",
            dataType: 'json',
            data: JSON.stringify(postJson),
            beforeSend: function() {
                loading = $.LangHua.loadingToast({
                    tip: '正 在 修  改 . . .  .'
                });
            },
            success: function(data) {
                loading.modal("hide");
                if (data.ErrorCode == 200) {
                    $.LangHua.alert({
                        title: "提示信息",
                        tip1: '修改修改结果',
                        tip2: '修改密码成功！',
                        button: '确定',
                        callback: function() {
                            window.location.reload();
                        }
                    })
                } else if (data.ErrorCode == 401) {
                    $.LangHua.alert({
                        title: "提示信息",
                        tip1: '修改失败',
                        tip2: data.ErrorMessage,
                        button: '确定'
                    });
                    $('body').one("click", '#passwrod-revise-confirm', confirmPassWord);
                } else {
                    $.LangHua.alert({
                        title: "提示信息",
                        tip1: '修改失败',
                        tip2: '请稍后重试',
                        button: '确定'
                    });
                    $('body').one("click", '#passwrod-revise-confirm', confirmPassWord);

                }
            },
            error: function() {
                loading.modal("hide");
                $.LangHua.alert({
                    title: "提示信息",
                    tip1: '修改失败',
                    tip2: '请稍后重试',
                    button: '确定'
                });
                $('body').one("click", '#passwrod-revise-confirm', confirmPassWord);
            }
        })

    })















})