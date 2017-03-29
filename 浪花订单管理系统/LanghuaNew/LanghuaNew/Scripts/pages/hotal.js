jQuery(document).ready(function () {
    $('#CountryID').bind('change', function () {
        var id = jQuery("#CountryID").val();
        $.ajax({
            type: 'get',
            dataType: 'json',
            url: '/Hotals/GetCity/' + id,
            success: function (data) {
                var str = "<option value=''>请选择</option>";
                for (var i in data.city) {
                    str += '<option value="' + data.city[i].CityID + '">' + data.city[i].CityName + '</option>';
                }
                $('#CityID').empty().append(str);
                $('#AreaID').empty().append("<option value=''>请选择</option>");
            }
        });
    })    
    $('#CityID').bind('change', function () {
        var id = jQuery("#CityID").val();
        $.ajax({
            type: 'get',
            dataType: 'json',
            url: '/Hotals/GetArea?CityID=' + id,
            success: function (data) {
                var str = "<option value=''>请选择</option>";
                for (var i in data.area) {
                    str += '<option value="' + data.area[i].AreaID + '">' + data.area[i].AreaName + '</option>';
                }
                $('#AreaID').empty().append(str);
            }
        });
    })

    $("form").bind("submit", function (e) {
        e.preventDefault()
        if (validate_form(this)) {
            this.submit();
        }
    })
    function validate_form(thisform) {
        with (thisform) {
            var bl = true;
            if (validate(AreaID, "请选择") == false)
                bl = false;
            if (validate(HotalName, "请填写") == false)
                bl = false;
            if (validate(HotalPhone, "请填写") == false)
                bl = false;
            if (validate(HotalAddress, "请填写") == false)
                bl = false;
            if (bl) {
                $("#btnSave").attr('disabled', true);
            }
            return bl;
        }
    }
    function validate(field, show) {
        with (field) {
            if (!value) {
                $(field).warning(show);
                return false;
            }
            return true;
        }
    }
})