
function OneCard(id,arr){
    this.id = id+"Card";
    this.content = new Object();
    for(var i in arr){
        this.content[arr[i]]= false;
    }    
    this.isVisible = false;
}
OneCard.prototype.updateVisible = function() {
    this.isVisible =false;
    for(var i in this.content){
        if(this.content[i]==true){
            this.isVisible =true;
            break;
        }
    }
    return this.isVisible;
};

var data = {
    "NewOrderCount": 0,
    "ReceiveOrderCount": 1,
    "CancelOrderCount": 0,
    "ChangeOrderCount": 0,
    "TodayOrderCount": 0,
    "YesterdayOrderCount": 0,
    "ThisMonthOrderCount": 1,
    "PreMonthOrderCount": 11
}
var ShopData = new Array(
    // 新订单
    new OneCard(
        'NewOrderCount',
        new Array('NewOrderCount')
    ),
    // 已接单
    new OneCard(
        'ReceiveOrderCount',
        new Array('ReceiveOrderCount')
    ),
    // 请求取消
    new OneCard(
        'CancelOrderCount',
        new Array('CancelOrderCount')
    ),
    // 请求变更
    new OneCard(
        'ChangeOrderCount',
        new Array('ChangeOrderCount')
    ),
    // 今日订单数
    new OneCard(
        'TodayOrderCount',
        new Array('TodayOrderCount','YesterdayOrderCount')
    ),
    //本月订单数
    new OneCard(
        'ThisMonthOrderCount',
        new Array('ThisMonthOrderCount','PreMonthOrderCount')
    )
   
);





var ShopDataObject =new Object();

for(var i in ShopData){
    // ShopData[i].updateVisible();
    ShopDataObject[ShopData[i].id] = ShopData[i];
}

jQuery(document).ready(function () {
    // $('.cardinfo').hide() 

    $('#updateShopData').one("click",function(){
        updateShopData(this);
    }).trigger("click");

    
    $('body').on("show",'#setShopDisplay',function(){
        var theModal = this;
        $.ajax({
            url:'/Home/GetAllWorkTableSetting',
            dataType:'json',
            type:'get',
            success:function(data){
                if(data.ErrorCode==200){
                    for(var i in data.data){
                        $(theModal).find('#'+i).prop("checked",data.data[i]);
                    }
                }
            }
        })
    })

   

    $('body').one('click','#setShopDisplayComfirm',function SHOP(){
        var theButton = this;
        var obj =new Object();
        $(this).closest('#setShopDisplay').find("input[type=checkbox]").each(function(){
            obj[$(this).attr("id")]=$(this).prop('checked');
        })
        $.ajax({
            url:'/Home/SaveAllWorkTableSetting',
            type:'post',
            data:JSON.stringify(obj),
            contentType: "application/json; charset=utf-8;",
            dataType:'json',
            beforeSend:function(){

            },
            success:function(data){
                if(data.ErrorCode==200){
                    $(theButton).success("保存成功！");
                    setTimeout(function(){
                        $('#updateShopData').trigger("click");
                         $(theButton).siblings("a").trigger("click");   
                    },500)
                    
                }
                else if (data.ErrorCode==401){
                    $.LangHua.alert( {
                        title: "提示信息",
                        tip1: '组件重复',
                        tip2: data.ErrorMessage,
                        button: '确定',
                    
                    });
                }
                else{
                     $.LangHua.alert( {
                        title: "提示信息",
                        tip1: '保存失败',
                        tip2: '保存失败',
                        button: '确定',
                    });
                }
            },
            complete:function(XRH,TS){
              
                $('body').one('click','#setShopDisplayComfirm',SHOP);
            }
            
        })
    })





    
 
    function updateShopData(theButton){
        $.ajax({
            url:'/Home/GetOrderData',
            dataType:'json',
            type:'get',
            beforeSend:function(){
                $(theButton).find('i').addClass("fa-spin");

            },
            success:function(data){
                if(data.ErrorCode==200){
                    var shopdata =    data.data;
                    for(var  i in shopdata){
                        $('#shopData').find('#'+i).text(parseInt(shopdata[i]).toLocaleString());
                        var cardID = $('#shopData').find('#'+i).closest(".cardinfo").attr("id");
                        // console.log(cardID)

                        if(shopdata[i]!=-1){
                            ShopDataObject[cardID].content[i] =true;
                        }
                    }
                    for(var i in ShopDataObject){

                        var isVisible = ShopDataObject[i].updateVisible();
                        if(isVisible){
                            // $('.cardinfo#'+i).closest('.col-lg-3').show().animateCss('zoomIn','animate500');
                            $('.cardinfo#'+i).closest('.col-lg-3').show().find('.number, .numbertips').animateCssTime('slideInUp','animate200');
                            

                        }
                        else{
                            $('.cardinfo#'+i).closest('.col-lg-3').hide(200);
                        }
                        //恢复默认 false;
                        for(var j in ShopDataObject[i].content){
                            ShopDataObject[i].content[j] =false;
                        }
                            

                    }
                    var date =new Date();
                    timeSTR = date.getFullYear()+'-'+(parseInt(date.getMonth())+1)+'-'+date.getDate()+' '+date.getHours()+':'+date.getMinutes()+':'+date.getSeconds();
                    $('#updattime').text("更新时间："+timeSTR);
                }
            },
            complete:function(XHR,TS){
                $(theButton).find('i').removeClass("fa-spin");
                $(theButton).one("click",function(){
                    updateShopData(this);
                })
            }
        })
    }





})