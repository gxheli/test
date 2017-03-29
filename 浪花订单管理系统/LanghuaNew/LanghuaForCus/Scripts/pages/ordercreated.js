jQuery(document).ready(function($){
    $('input.in').onlyNum();
    $('input.in').bind({
        change:a,
    })

    $('a#copy').bind('copy',function(e){
        console.log(e)
        e.clipboardData.clearData();
        e.clipboardData.setData("text/plain", $('#URL').text()+'\n'+ $('#forcopy').text());
        e.preventDefault();
    })
    $('a#save').bind('click',function(){
       var i = false;
       $('input.in').each(function(){
            if(jQuery(this).val()==''){
                jQuery(this).addClass("warning");
                jQuery(this).one('foucs',function () {
                    jQuery(this).removeClass("warning");
                    
                })
                i = true;
                
            }
       })
       if(i){
           return;
       }
       
        
       $('#formSave').submit();
    })
})
function a(){
        if(isNaN($(this).val())){
            if(jQuery(this).attr('id')=='cost'){
                jQuery(this).val(jQuery(this).attr("old"));
            }
             else{
                jQuery(this).val(0);
            }
        }
        var cost =  Number(jQuery('#cost').val());
        var recieve =  Number(jQuery('#recieve').val());
        jQuery('#benefit').text(parseInt(recieve-cost));
        jQuery('#benefitrate').text(parseInt((recieve-cost)/cost*100)+'%');
}

jQuery.fn.onlyNum = function () {
    jQuery(this).keypress(function (event) {
        console.log("onlynumkeypress")
        var eventObj = event || e;
        var keyCode = eventObj.keyCode || eventObj.which;
        
        if ((keyCode >= 48 && keyCode <= 57)||keyCode==46){	
            return true;
        }
        else{
            return false;
        }
    })
    jQuery(this).bind("change",function(){
        console.log("onlynumchange");
        var value=jQuery.trim(jQuery(this).val());
        if(!!value){
            if(isNaN(value)){
                value=0;
            }
            if(value<0){
                value=0;
            }
        }
        else{
            value=0;
        }
        jQuery(this).val(value);
    });
}