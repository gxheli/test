jQuery(document).ready(function () {


    var  obj = jQuery('#controlleraction').data();
    var  isCookieUse =isCookieUseAble();
        
    $('.table-Dairy').each(function(){
        var thisID = $(this).attr("id");
        var  cookieStamp = obj.controller+obj.action+ (thisID ? thisID:"");
        var length =50;
        if(isCookieUse){
            length = getCookie(cookieStamp)?getCookie(cookieStamp):50;
            setCookie(cookieStamp,length,365);
        }
        $(this).on( 'length.dt', function ( e, settings, len ) {
            setCookie(cookieStamp,len,365);
          
        } );
        $(this).DataTable({
            ordering: false,
            searching: false,
            pageLength:parseInt(length)
        });
    })

    function getCookie(c_name)
    {
        if (document.cookie.length>0)
        {
            c_start=document.cookie.indexOf(c_name + "=")
            if (c_start!=-1)
            { 
                c_start=c_start + c_name.length+1 
                c_end=document.cookie.indexOf(";",c_start)
                if (c_end==-1) 
                    c_end=document.cookie.length;
                return unescape(document.cookie.substring(c_start,c_end))
            } 
        }
        return ""
    }

    function setCookie(c_name,value,expiredays)
    {
        var exdate=new Date()
        exdate.setDate(exdate.getDate()+expiredays)
        document.cookie=c_name+ "=" +escape(value)+
        ((expiredays==null) ? "" : ";expires="+exdate.toGMTString())
    }
    function clearCookie(name) {  
        setCookie(name, "", -1);  
    }  
    function isCookieUseAble()
    {
        var cookieStamp = 'cookie'+new Date().valueOf();
        setCookie(cookieStamp,cookieStamp,365);
        if(getCookie(cookieStamp)!=""){
            clearCookie(cookieStamp);
            return true;
        }
        return false;
    }
})