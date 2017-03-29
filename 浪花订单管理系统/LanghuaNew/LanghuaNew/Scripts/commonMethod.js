// 修复浏览器不支持trim()方法的问题
if (!String.prototype.trim) {
  String.prototype.trim = function () {
    return this.replace(/^[\s\uFEFF\xA0]+|[\s\uFEFF\xA0]+$/g, '');
  };
}

if( !window.getComputedStyle) {
    window.getComputedStyle = function(e) {return e.currentStyle};
}
// 限定数字
jQuery.fn.onlyNum = function () {
    jQuery(this).keypress(function (event) {
        console.log("onlynumkeypress")
        var eventObj = event || e;
        var keyCode = eventObj.keyCode || eventObj.which;

        if ((keyCode >= 48 && keyCode <= 57)) {
            return true;
        }
        else {
            return false;
        }
    })
    jQuery(this).bind("change", function () {
        console.log("onlynumchange");
        var value = jQuery.trim(jQuery(this).val());
        if (!!value) {
            if (isNaN(value)) {
                value = 0;
            }
            if (value < 0) {
                value = 0;
            }
        }
        else {
            value = 0;
        }
        jQuery(this).val(value);
    });
}


jQuery.fn.lastNum = function () {
    jQuery(this).keypress(function (event) {
        console.log("onlynumkeypress")
        var eventObj = event || e;
        
        
        var keyCode = eventObj.keyCode || eventObj.which;
       
        if ((keyCode >= 48 && keyCode <= 57)) {
             var last =String.fromCharCode(keyCode)
             jQuery(this).val(last)             
            return false;
        }
        else {
            return false;
        }
    })
    
    jQuery(this).bind("change", function () {
        console.log("onlynumchange");
        var value = jQuery.trim(jQuery(this).val());
        if (!!value) {
            if (isNaN(value)) {
                value = 0;
            }
            if (value < 0) {
                value = 0;
            }
        }
        else {
            value = 0;
        }
        jQuery(this).val(value);
    });
}


jQuery.fn.onlyCapchar = function () {
    jQuery(this).keypress(function (event) {
        var eventObj = event || e;
        var keyCode = eventObj.keyCode || eventObj.which;
        console.log(keyCode)
        if ((keyCode >= 97 && keyCode <= 122) || (keyCode >= 65 && keyCode <= 90)) {
            return true;
        }
        else {
            return false;
        }
    })


    jQuery(this).bind("change", function () {

        var value = jQuery.trim(jQuery(this).val());
        var str = '^[a-zA-Z]+$';
        var str = new RegExp(str);
        if (str.test(value)) {
            this.value = value.toUpperCase()
        }
        else {
            jQuery(this).val('');
        }

    });
}

jQuery.fn.onlyChar = function () {
    jQuery(this).keypress(function (event) {
        var eventObj = event || e;
        var keyCode = eventObj.keyCode || eventObj.which;
        console.log(keyCode)
        if ((keyCode >= 97 && keyCode <= 122) || (keyCode >= 65 && keyCode <= 90)) {
            return true;
        }
        else {
            return false;
        }
    })


    jQuery(this).bind("change", function () {

        var value = jQuery.trim(jQuery(this).val());
        var str = '^[a-zA-Z]+$';
        var str = new RegExp(str);
        if (str.test(value)) {
           
        }
        else {
            jQuery(this).val('');
        }

    });
}

jQuery.fn.onlyCharNum = function () {
    jQuery(this).keypress(function (event) {
        var eventObj = event || e;
        var keyCode = eventObj.keyCode || eventObj.which;
        console.log(keyCode)
        if (
            (keyCode >= 97 && keyCode <= 122) ||
            (keyCode >= 65 && keyCode <= 90)||
            (keyCode >= 48 && keyCode <= 57)

        ) {
            return true;
        }
        else {
            return false;
        }
    })


    jQuery(this).bind("change", function () {

        var value = jQuery.trim(jQuery(this).val());
        var str = '^[a-zA-Z]+$';
        var str = new RegExp(str);
        if (str.test(value)) {
           
        }
        else {
            jQuery(this).val('');
        }

    });
}



jQuery.fn.onlyChinese = function () {

    jQuery(this).bind("change", function () {
        var _this = this;

        var value = jQuery.trim(jQuery(this).val());
        var str = /^[\u4e00-\u9fa5]+$/;
        if (str.test(value)) {
        }
        else {
            var strx = new RegExp("[^\\u4E00-\\u9FFF]+", "g");;
            console.log(value.replace(strx, ''));
            jQuery(_this).val(value.replace(strx, ''));
        }

    });
}
//warning 
jQuery.fn.warning = function (title) {
    jQuery(this).addClass('warning');
    jQuery(this).tooltip({
        title: title,
        trigger: "manual"
    }).tooltip("show");

    jQuery(this).one("click", function () {
        $(this).removeClass("warning");
        jQuery(this).tooltip('destroy');
    }).one("focus", function () {
        $(this).removeClass("warning");
    })
}
// success

jQuery.fn.success = function (title) {
    $(this).tooltip({
        title:title,
        trigger:"manual",
        animation: true   
    }).tooltip('show');
    var _this = this;
    setTimeout(function() {
        _this.tooltip('destroy');
            
    },1000);
}
//posting
jQuery.fn.buttonposting = function (title) {
   $(this).text(title);
   $(this).prepend('<i class="fa fa-spinner fa-spin"></i>')
}
// posted

jQuery.fn.posttedbutton = function (tipsfirst,tipssecond) {
    var _this =$(this);
    setTimeout(function(){
        _this.find('i').remove();
        _this.text(tipsfirst);
    },500)
    setTimeout(function(){
        _this.text(tipssecond);
    },2000)


  
}


function isChineseChar(str) {
    var reg = /^[\u4e00-\u9fa5]+$/;
    if (reg.test(str)) {
        return true;
    }
    else {
        return false;
    }
}

function isEnglishChar(str) {
    var reg = /^[A-Z]+$/;
    if (reg.test(str)) {
        return true;
    }
    else {
        return false;
    }
}

function isEmail(str) {
    var myReg = /^[-_A-Za-z0-9]+@([_A-Za-z0-9]+\.)+[A-Za-z0-9]{2,3}$/;
    if (myReg.test(str)) return true;
    return false;
}

function isWechat(str) {
    console.log(str)
    var reg = /^[a-zA-Z\d_]{5,}$/;
    if (reg.test(str)) return true;
    return false;
}


;(function($) {
    jQuery.fn.ButtonRadio = function( options ) {
        var defaults = {
            data:[],
            selected:function (dom,code){
            }
        };
        
        var settings = $.extend( {}, defaults, options );
    
        return this.each(function() {
            
            var str =getButtonList(settings.data);
            jQuery(this).append(str).on('click','.buttonradio',function(){
                
                jQuery(this).addClass('active');
                jQuery(this).siblings('select').val(-1);
                jQuery(this).siblings().removeClass('active'); 
                
                settings.selected.call(this,this,jQuery(this).data('code'));
            })
            
            jQuery(this).on('change','select',function(){
                jQuery(this).siblings().removeClass('active');
                if(jQuery(this).val()==-1){
                    jQuery(this).siblings().first().addClass('active');
                }
                settings.selected.call(this,this,jQuery(this).val());
            })
        });
        
        function getButtonList(obj){
            var str = '';
            for(var i in obj.showOutState){
                str += makeOneRadioButton(obj.showOutState[i]);
            }
            for(var i in obj.labelOut){
                str += makeOneRadioButton(obj.labelOut[i]);
            }
            if('selectInState' in obj){
                str += '<select  data-postkey="status" data-type="select" class="input input-sm input-small ">';
                
                for(var i in obj.selectInState){
                    str += makeOneOption(obj.selectInState[i]);
                }
                str += '</select>';
            }
            return str;
        }
        function makeOneRadioButton(obj){
            var postkey = obj['postkey']?obj['postkey']:"status";
            var str ='<a href="javascript:"  data-postkey="'+postkey+'" data-type="'+obj['type']+'" class="buttonradio"  data-code="'+obj.value+'">'+obj.text+'</a>\n';
            return str;
        }
        function makeOneOption(obj){
            var str ='<option  value="'+obj.value+'">'+obj.text+'</option>';
            return str;
        }
    
    };
})(jQuery);
    