// 修复浏览器不支持trim()方法的问题
if (!String.prototype.trim) {
  String.prototype.trim = function () {
    return this.replace(/^[\s\uFEFF\xA0]+|[\s\uFEFF\xA0]+$/g, '');
  };
}

if( !window.getComputedStyle) {
    window.getComputedStyle = function(e) {return e.currentStyle};
}


if (!String.prototype.urlSwitch) {
    String.prototype.urlSwitch = function () {
        var str ='';
       
        var arr=[
            {
                before:'%',
                after:'%25'
            },
            {
                before:/ /g,
                after:'%20'
            },
            {
                before: /"/g,
                after: '%22'
            },
            {
                before:/#/g,
                after:'%23'
            },
           
            {
                before:/&/g,
                after:'%26'
            },
            {
                before:/\(/g,
                after:'%28'
            },
            {
                before:/\)/g,
                after:'%29'
            },
            {
                before:/\+/g,
                after:'%2B'
            },
            {
                before:/,/g,
                after:'%2C'
            },
            {
                before:/\//g,
                after:'%2F'
            },
            {
                before:/:/g,
                after:'%3A'
            },
            {
                before:/;/g,
                after:'%3B'
            },
            {
                before:/</g,
                after:'%3C'
            },
            {
                before:/=/g,
                after:'%3D'
            },
            {
                before:/>/g,
                after:'%3E'
            },
            {
                before:/\?/g,
                after:'%3F'
            },
            {
                before:/@/g,
                after:'%40'
            },
          
             {
                before:/\\/g,
                after:'%5C'
            },
            {
                before:/\|/g,
                after:'%7C'
            },
        ]
        str = this;
        for(var i in arr){
            str = str.replace(arr[i]['before'], arr[i]['after'])
        }
        return str;
    };
}
//禁止字符
jQuery.fn.forbidChar = function (arr) {
    var objCode = new Object();
    for(var i in arr ){
        objCode[arr[i].charCodeAt(0)]=1;
    }
    jQuery(this).keypress(function (event) {
        var eventObj = event || e;
        var keyCode = eventObj.keyCode || eventObj.which;

        if ((keyCode in objCode)) {
            return false;
        }
        else {
            return true;
        }
    })
    
}

// 限定数字
jQuery.fn.onlyNum = function () {
    jQuery(this).keypress(function (event) {
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
// 限定数字
jQuery.fn.onlyNumWithEmpty = function () {
    jQuery(this).keypress(function (event) {
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
        var value = jQuery.trim(jQuery(this).val());
        if (!!value) {
            if (isNaN(value)) {
                value = "";
            }
            if (value < 0) {
                value = "";
            }
        }
        else {
            value = "";
        }
        jQuery(this).val(value);
    });
}
jQuery.fn.onlyNumWithEmptyR = function () {
    jQuery(this).keypress(function (event) {
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
        var value = jQuery.trim(jQuery(this).val());
        if (!!value) {
            if (isNaN(value)) {
                value = "";
            }
            if (value < 0) {
                value = "";
            }
        }
        else {
            value = "";
        }
        jQuery(this).val(value);
        jQuery(this).trigger("keyup");
    });
}


jQuery.fn.lastNum = function () {
    jQuery(this).keypress(function (event) {
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
        if ((keyCode >= 65 && keyCode <= 90) || (keyCode >= 97 && keyCode <= 122)) {
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



jQuery.fn.onlyCharNum = function () {
    jQuery(this).keypress(function (event) {
        var eventObj = event || e;
        var keyCode = eventObj.keyCode || eventObj.which;
        if ((keyCode >= 48 && keyCode <= 57) || (keyCode >= 65 && keyCode <= 90) || (keyCode >= 97 && keyCode <= 122)) {
            return true;
        }
        else {
            return false;
        }
    })


    jQuery(this).bind("change", function () {

        var value = jQuery.trim(jQuery(this).val());
        var str = '^[a-zA-Z0-9]+$';
        var str = new RegExp(str);
        if (str.test(value)) {
            this.value = value.toUpperCase()
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
        jQuery(this).tooltip('destroy');

    })
}

jQuery.fn.Warning = function (options) {
    var defaults = {
        title: "",
        placement: "top",
        attach:[]
    };
    var settings = $.extend({}, defaults, options);
    jQuery(this).addClass('warning');
    jQuery(this).tooltip({
        title: settings.title,
        placement:settings.placement,
        trigger: "manual"
    }).tooltip("show");

    jQuery(this).one("click", function () {
        $(this).removeClass("warning");
        jQuery(this).tooltip('destroy');
    }).one("focus", function () {
        $(this).removeClass("warning");
        jQuery(this).tooltip('destroy');

    })
    var _this = this;
    for (var i in settings.attach) {
        jQuery(body).one("click", settings.attach[i], function () {
            $(_this).removeClass("warning");
            jQuery(_this).tooltip('destroy');
        })
        jQuery(body).one("focus", settings.attach[i], function () {
            $(_this).removeClass("warning");
            jQuery(_this).tooltip('destroy');
        })
    }
}

jQuery.fn.WarningDelete = function () {
    jQuery(this).tooltip('destroy');
    jQuery(this).removeClass("warning");
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
    var reg = /^[A-Za-z]+$/;
    if (reg.test(str)) {
        return true;
    }
    else {
        return false;
    }
}

function isEmail(str) {
    var myReg =  /^([a-zA-Z0-9_#*~$^`|;:"'/?<>,&\\\(\)={}\[\]\%\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{1,10})+$/;
    if (myReg.test(str)) return true;
    return false;
}

function isWechat(str) {
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




;(function($) {
    jQuery.fn.formWarning = function( options ) {
        var defaults = {
            tips:"填写有误"
        };
        var settings = $.extend( {}, defaults, options );
        jQuery(this).closest(".form-group").find(".help-inline").addClass("tips").text(settings.tips);
        jQuery(this).one("click", function () {
           jQuery(this).closest(".form-group").find(".help-inline").removeClass("tips").text("");
        }).one("focus", function () {
           jQuery(this).closest(".form-group").find(".help-inline").removeClass("tips").text("");
        })
    };
})(jQuery);
    