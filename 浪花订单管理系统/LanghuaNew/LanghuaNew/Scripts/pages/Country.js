jQuery(document).ready(function () {

    $("#countryNext").bind("click", function () {

        if (jQuery('#divCountry input.CountryType[name=CountryType]:checked').val() == "selectCountry") {
            if (!jQuery('#Country').val()) {
                jQuery('#Country').warning("请选择");
                return;
            }
            jQuery('#countryText').text(jQuery('#Country option:selected').text());

            $("#divCity input.CityType[name=CityType]").eq(0).removeAttr("disabled");
            $("#divCity input.CityType[name=CityType]").eq(0).attr("checked", "checked");
            $("#divCity input.CityType[name=CityType]").eq(1).removeAttr("checked");
            $("#divCity input.CityType[name=CityType]").eq(0).click();

            var id = jQuery("#Country").val();
            $.ajax({
                type: 'get',
                dataType: 'json',
                url: '/Hotals/GetCity/' + id,
                success: function (data) {
                    var str = "<option value=''>请选择</option>";
                    for (var i in data.city) {
                        str += '<option value="' + data.city[i].CityID + '">' + data.city[i].CityName + '-' + data.city[i].CityEnName + '</option>';
                    }
                    $('#City').empty().append(str);
                }
            });
            $("#divCity").removeClass("hidden");
            $("#divCountry").addClass("hidden");
        }
        else {
            if (!jQuery('#CountryName').val()) {
                jQuery('#CountryName').warning("请填写");
            }
            if (!jQuery('#CountryEnName').val()) {
                jQuery('#CountryEnName').warning("请填写");
            }
            if (!jQuery('#CountryName').val() || !jQuery('#CountryEnName').val()) {
                return;
            }
            $.ajax({
                type: 'post',
                dataType: 'json',
                url: '/Countries/CheckCountry',
                contentType: "application/json; charset=utf-8;",
                data: JSON.stringify({
                    "CountryName": jQuery('#CountryName').val(),
                    "CountryEnName": jQuery('#CountryEnName').val()
                }),
                success: function (data) {
                    if (data.check === true) {
                        jQuery.LangHua.alert({
                            title: "提示信息",
                            tip1: '提示信息：',
                            tip2: '已存在同名国家',
                            button: '确定',
                        })
                        return;
                    }

                    jQuery('#countryText').text(jQuery('#CountryName').val() + "-" + jQuery('#CountryEnName').val());
                    $("#divCity input.CityType[name=CityType]").eq(0).removeAttr("checked");
                    $("#divCity input.CityType[name=CityType]").eq(0).attr("disabled", "true");
                    $("#divCity input.CityType[name=CityType]").eq(1).attr("checked", "checked");
                    $("#divCity input.CityType[name=CityType]").eq(1).click();
                    $("#divCity").removeClass("hidden");
                    $("#divCountry").addClass("hidden");
                }
            });
        }

    })
    $("#cityNext").bind("click", function () {

        if (jQuery('#divCity input.CityType[name=CityType]:checked').val() == "selectCity") {
            if (!jQuery('#City').val()) {
                jQuery('#City').warning("请选择");
                return;
            }
            jQuery('#cityText').text(jQuery('#City option:selected').text());
            jQuery('#countryAreaText').text(jQuery('#countryText').text());

            $("#divCity").addClass("hidden");
            $("#divArea").removeClass("hidden");
        }
        else {
            if (!jQuery('#CityName').val()) {
                jQuery('#CityName').warning("请填写");
            }
            if (!jQuery('#CityEnName').val()) {
                jQuery('#CityEnName').warning("请填写");
            }
            if (!jQuery('#CityName').val() || !jQuery('#CityEnName').val()) {
                return;
            }
            $.ajax({
                type: 'post',
                dataType: 'json',
                url: '/Countries/CheckCity',
                contentType: "application/json; charset=utf-8;",
                data: JSON.stringify({
                    "CountryID": (function () {
                        if (jQuery('#divCountry input.CountryType[name=CountryType]:checked').val() == "selectCountry") {
                            return jQuery('#Country').val();
                        }
                        return 0;
                    })(),
                    "CityName": jQuery('#CityName').val(),
                    "CityEnName": jQuery('#CityEnName').val()
                }),
                success: function (data) {
                    if (data.check === true) {
                        jQuery.LangHua.alert({
                            title: "提示信息",
                            tip1: '提示信息：',
                            tip2: '该国家已存在同名城市',
                            button: '确定',
                        })
                        return;
                    }
                    jQuery('#cityText').text(jQuery('#CityName').val() + "-" + jQuery('#CityEnName').val());
                    jQuery('#countryAreaText').text(jQuery('#countryText').text());

                    $("#divCity").addClass("hidden");
                    $("#divArea").removeClass("hidden");
                }
            });
        }

    })
    $("#cityPre").bind("click", function () {
        $("#divCity").addClass("hidden");
        $("#divCountry").removeClass("hidden");
    })
    $("#areaPre").bind("click", function () {
        $("#divCity").removeClass("hidden");
        $("#divArea").addClass("hidden");
    })

    jQuery("body").on('click', '.areaRow a.add', function () {
        var _this = this;
        var AreaName = jQuery(_this).siblings('#AreaName').val();
        var AreaEnName = jQuery(_this).siblings('#AreaEnName').val();
        var str = [
            '<div class="form-group areaRow">',
                '<label class="col-md-2 control-label">新增区域：</label>',
                '<div class="col-md-10">',
                    '<input placeholder="中文名" id="AreaName" type="text" class="form-control neworder-input-medium input-inline" style="width:120px" value="' + AreaName + '">',
                    '<input placeholder="英文名" id="AreaEnName" type="text" class="form-control neworder-input-medium input-inline" style="width:120px" value="' + AreaEnName + '">',
                    '<a href="javascript:;" class="del">删除</a>',
                '</div>',
            '</div>',
        ].join('\n');
        jQuery(_this).closest('.areaRow').before(str);
        jQuery(_this).siblings('#AreaName').val('');
        jQuery(_this).siblings('#AreaEnName').val('');
    })
    jQuery("body").on('click', '.areaRow a.del', function () {
        jQuery(this).closest('.areaRow').remove();
    })

    jQuery("#btnSave").one('click', function save() {
        var _this = this;
        var post = true;
        var bl = false;
        var objAreaName = new Object();
        var objAreaEnName = new Object();
        $('.areaRow').each(function () {
            var thisRow = this;
            var AreaName = jQuery.trim($(thisRow).find('#AreaName').val());
            var AreaEnName = jQuery.trim($(thisRow).find('#AreaEnName').val());
            if (!AreaName) {
                $(thisRow).find('#AreaName').warning("请填写");
                return;
            }
            if (AreaName in objAreaName) {
                $(thisRow).find('#AreaName').warning("名字重复");
                bl = true;
            }
            else {
                objAreaName[AreaName] = AreaName;
            }
            if (!AreaEnName) {
                $(thisRow).find('#AreaEnName').warning("请填写");
                return;
            }
            if (AreaEnName in objAreaEnName) {
                $(thisRow).find('#AreaEnName').warning("名字重复")
                bl = true;
            }
            else {
                objAreaEnName[AreaEnName] = AreaEnName;
            }
        })
        if (bl) {
            $(_this).one('click', save)
            return;
        }
        var areas = new Array();
        $('.areaRow').each(function () {
            var thisRow = this;

            var area = {
                "AreaName": jQuery.trim($(thisRow).find('#AreaName').val()),
                "AreaEnName": jQuery.trim($(thisRow).find('#AreaEnName').val()),
            }
            areas.push(area);
        })
        var country = {
            "CountryID": (function () {
                if (jQuery('#divCountry input.CountryType[name=CountryType]:checked').val() == "selectCountry") {
                    return jQuery('#Country').val();
                }
                return 0;
            })(),
            "CountryName": jQuery('#CountryName').val(),
            "CountryEnName": jQuery('#CountryEnName').val(),
            "Citys": [{
                "CityID": (function () {
                    if (jQuery('#divCity input.CityType[name=CityType]:checked').val() == "selectCity") {
                        return jQuery('#City').val();
                    }
                    return 0;
                })(),
                "CityName": jQuery('#CityName').val(),
                "CityEnName": jQuery('#CityEnName').val(),
                "Areas": areas
            }]
        }
        if (post) {
            $.ajax({
                type: 'post',
                dataType: 'json',
                contentType: "application/json; charset=utf-8;",
                data: JSON.stringify(country),
                url: '/Countries/Save',
                success: function (data) {
                    if (data.ErrorCode == 200) {
                        jQuery("#btnSave").success("保存成功");
                        window.location.href = "/Countries";
                    }
                    else {
                        if (data.ErrorCode == 401) {
                            $.LangHua.alert({
                                title: "提示信息",
                                tip1: '保存失败',
                                tip2: data.ErrorMessage,
                                button: '确定'
                            })
                        }
                        jQuery(_this).one('click', save);
                    }
                },
                failed: function () {
                    jQuery(_this).one('click', save);
                }
            });
        }
        else {
            $(_this).one('click', save)
        }
    })




})