
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
var Mydata = new Array(
    // 未填写
    new OneCard(
        'MyNotfilledCount',
        new Array('MyNotfilledCount')
    ),
    // 待核对
    new OneCard(
        'MyFilledCount',
        new Array('MyFilledCount')
    ),
    // 为付完
    new OneCard(
        'MyNoPayCount',
        new Array('MyNoPayCount')
    ),
    // 已拒绝
    new OneCard(
        'MySencondFullCount',
        new Array('MySencondFullCount')
    ),
    // 今日订单数
    new OneCard(
        'MyTodayOrderCount',
        new Array('MyTodayOrderCount','MyYesterdayOrderCount')
    ),
    //今日销售额
    new OneCard(
        'MyTodaySales',
        new Array('MyTodaySales','MyThisMonthSales')
    ),
    //今日利润
    new OneCard(
        'MyTodayProfits',
        new Array('MyTodayProfits','MyThisMonthProfits')
    )
);



var ShopData = new Array(
    // 今日订单数
    new OneCard(
        'TodayOrderCount',
        new Array('TodayOrderCount','YesterdayOrderCount')
    ),
    // 待检查
    new OneCard(
        'OnCheckCount',
        new Array('OnCheckCount')
    ),
    //已核对
    new OneCard(
        'CheckCount',
        new Array('CheckCount')
    ),
    //要售后
    new OneCard(
        'NeedServiceCount',
        new Array('NeedServiceCount')
    ),
    //今日出行人数
    new OneCard(
        'TodayTravelNum',
        new Array('TodayTravelNum','YesterdayTravelNum')
    ),
    // 今日利润
    new OneCard(
        'TodayProfits',
        new Array('TodayProfits','ThisMonthProfits')
    ),
    // 今日销售额(元)
    new OneCard(
        'TodaySales',
        new Array('TodaySales','ThisMonthSales')
    ),
    // 本月服务人数
    new OneCard(
        'ThisMonthTravelNum',
        new Array('ThisMonthTravelNum','PreMonthTravelNum')
    ),
    // 微信绑定人数
    new OneCard(
        'WeixinBindCount',
        new Array('WeixinBindCount','WeixinBindRate')
    )

);

var MydataObject = new Object();
var ShopDataObject =new Object();
for(var i in Mydata){
    // Mydata[i].updateVisible();
    MydataObject[Mydata[i].id] = Mydata[i];
}
for(var i in ShopData){
    // ShopData[i].updateVisible();
    ShopDataObject[ShopData[i].id] = ShopData[i];
}

jQuery(document).ready(function () {
    // $('.cardinfo').hide() 

    $('#updateMyData').one("click",function(){
        updateMyData(this);
    }).trigger("click");
    $('#updateShopData').one("click",function(){
        updateShopData(this);
    }).trigger("click");

    $('body').on("show",'#setMyDisplay',function(){
        var theModal = this;
        $.ajax({
            url:'/Home/GetMyWorkTableSetting',
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

    $('body').one('click','#setMyDisplayComfirm',function MY(){
        var theButton = this;
        var obj =new Object();
        $(this).closest('#setMyDisplay').find("input[type=checkbox]").each(function(){
            obj[$(this).attr("id")]=$(this).prop('checked');
        })
        $.ajax({
            url:'/Home/SaveMyWorkTableSetting',
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
                        $('#updateMyData').trigger("click");
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
                        tip1: '组件重复',
                        tip2: '保存失败',
                        button: '确定',
                    });
                }
            },
            complete:function(XRH,TS){
                $('body').one('click','#setMyDisplayComfirm',MY);
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





    
    function updateMyData(theButton){
        $.ajax({
            url:'/Home/GetMyOrderData',
            type:'get',
            dataType:'json',
            beforeSend:function(){
                $(theButton).find('i'). addClass("fa-spin");
            },
            success:function(data){
                if(data.ErrorCode==200){
                    var mydata =    data.data;
                    for(var  i in mydata){
                        $('#myData').find('#'+i).text(parseInt(mydata[i]).toLocaleString());
                        var cardID = $('#myData').find('#'+i).closest(".cardinfo").attr("id");
                        // console.log(cardID)
                        if(mydata[i]!=-1){
                            MydataObject[cardID].content[i] =true;
                        }
                        
                    }
                    for(var i in MydataObject){
                        var isVisible = MydataObject[i].updateVisible();
                        if(isVisible){
                            // $('.cardinfo#'+i).closest('.col-lg-3').show().animateCss('zoomIn','animate500');
                            $('.cardinfo#'+i).closest('.col-lg-3').show().find('.number, .numbertips').animateCssTime('slideInUp','animate200');

                        }
                        else{
                            $('.cardinfo#'+i).closest('.col-lg-3').hide();
                        }
                        //恢复默认 false;
                        for(var j in MydataObject[i].content){
                            MydataObject[i].content[j] =false;
                        }

                    }
                }
               
            },
            complete:function(XHR,TS){
                $(theButton).find('i'). removeClass("fa-spin");
                $(theButton).one("click",function(){
                    updateMyData(this);
                })
                
            }
        })
    }
    function updateShopData(theButton){
        $.ajax({
            url:'/Home/GetAllOrderData',
            dataType:'json',
            type:'get',
            beforeSend:function(){
                $(theButton).find('i').addClass("fa-spin");

            },
            success:function(data){
                if(data.ErrorCode==200){
                    var shopdata =    data.data;
                    for (var i in shopdata) {
                        if (i != "RefreshTime") {
                            $('#shopData').find('#' + i).text(parseInt(shopdata[i]).toLocaleString());
                            var cardID = $('#shopData').find('#' + i).closest(".cardinfo").attr("id");
                            // console.log(cardID)

                            if (shopdata[i] != -1) {
                                ShopDataObject[cardID].content[i] = true;
                            }
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
                    //var date =new Date();
                    //timeSTR = date.getFullYear()+'-'+(parseInt(date.getMonth())+1)+'-'+date.getDate()+' '+date.getHours()+':'+date.getMinutes()+':'+date.getSeconds();
                    //$('#updattime').text("更新时间："+timeSTR);
                    $('#updattime').text("更新时间：" + shopdata.RefreshTime);
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