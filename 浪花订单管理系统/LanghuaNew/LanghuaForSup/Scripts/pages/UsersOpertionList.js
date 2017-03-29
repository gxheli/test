jQuery(document).ready(function () {

    $('#orderList').eq(0)
       .on('preXhr.dt', function (e, settings, json) {
           var search = ($('#searchoption').data("search"));
           // 删除插件无必要项目
           delete json.columns;
           delete json.order;
           delete json.search;
       })
    
    //默认获取cookie
    var  obj = jQuery('#controlleraction').data();
    var  isCookieUse =isCookieUseAble();
    var thisID = $('#orderList').attr("id");
    var  cookieStamp = obj.controller+obj.action+ (thisID ? thisID:"");
    var length =50;
    if(isCookieUse){
        length = getCookie(cookieStamp)?getCookie(cookieStamp):50;
        setCookie(cookieStamp,length,365);
    }
    $('#orderList').on( 'length.dt', function ( e, settings, len ) {
        setCookie(cookieStamp,len,365);
    } );
    // 默认获取cookie结束

    var newproduct =
	jQuery('table#orderList')
        .DataTable({
            ajax: {
                url: "/Users/GetUsersOperation/" + jQuery("#userid").val(),
                type: 'post',
            },
            ordering: false,
            searching: false,
            serverSide: true,
            pageLength:length,
           
            //行操作
            rowId: "UserLogID",
           
            //列操作
            columns: [
                {
                    'data': 'OperTime',
                    "createdCell": function (td, cellData, rowData, row, col) {
                        jQuery(td).html(cellData);
                    }
                },
                {
                    'data': 'OperUserNickName',
                    "createdCell": function (td, cellData, rowData, row, col) {
                        jQuery(td).html(cellData);
                    }
                },
                {
                    'data': 'Operate',
                    "createdCell": function (td, cellData, rowData, row, col) {
                        jQuery(td).html(cellData);
                    }
                },
                {
                    'data': 'Remark',
                    "createdCell": function (td, cellData, rowData, row, col) {
                        jQuery(td).html(cellData);
                    }
                },
            ]
        });





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