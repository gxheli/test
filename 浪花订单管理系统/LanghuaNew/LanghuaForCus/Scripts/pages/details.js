jQuery(document).ready(function ($) {
    var tableDefaultWidth = 0;
    $('.template').find('col').each(function(){
        tableDefaultWidth += parseInt($(this).attr('width'));
        
    });
    tableDefaultWidth  +=  $('.template').find('col').length+1;
    $('.template').css("min-width",tableDefaultWidth+'px');


    var width = $('#detailmain').css('width').split('px')[0];
    var OrderDefaultWidth =  $('#onebookingorder').css("width").split('px')[0];
    var defaultHeight =  $('#onebookingorder').css("height").split('px')[0];

  
    $('#onebookingorder').css("left","50%");
    $('#onebookingorder').css("margin-left",'-'+OrderDefaultWidth/2+"px");
        
    
    if(parseInt(width)<parseInt(OrderDefaultWidth)){
        var scaleRate =width/OrderDefaultWidth;
        
        $('#onebookingorder').css("margin-top",'-'+defaultHeight/2*(1-scaleRate)+"px");
        $('#onebookingorder').css("transform", "scale(" + scaleRate + ")");
        
    }
    $('#onebookingorder').css("opacity",1);

    // $('#onebookingorder').css("left","0px");



    jQuery('#print').bind("click",function(){
       
        jQuery(".noprint").hide();
        while((function(){
             var show = false;
             jQuery(".noprint").each(function(){
                 jQuery(this).css("display")!='none';
                 var show =true
             })
            return show
        })()){

        }
        window.print();
        jQuery(".noprint").show();
        return true;
    })

     //保存功能
    $('#download').bind('click',function(){
        html2canvas(
            document.getElementById("onebookingorder"),
            { 
                letterRendering:true,  
                onrendered: function (canvas) {
                    if (canvas.msToBlob) { //for IE
                        var blob = canvas.msToBlob();
                        window.navigator.msSaveBlob(blob, $('#picturetitle').text()+'.png');
                    }
                    else {
                   
                        var data = canvas.toDataURL("image/png",1);
                    
                        var link = document.createElement("a");
                        link.download =  $('#picturetitle').text()+'.png';
                        link.href = data;
                        document.body.appendChild(link);
                        link.click();

                         document.body.removeChild(link);
                         delete link;
                    }
                }
            }
        )
    })

    $('body').on('shown.bs.modal','#advancedsearch',function(){
        
        $(this).find('#picname').text( $('#picturetitle').text()+'.png')
        $(this).find('#title').val( $('#picturetitle').text())
        $(this).find('#toMail').val( $('#toMailHidden').val());
        $(this).find('#confirmadvancedsearch').text("发送");


    })

    $('body').on('click','#confirmadvancedsearch',function(){

        

        
        
        var t ={};
        t.toMail =$('#advancedsearch').find("#toMail").val().trim();
        t.title =$('#advancedsearch').find("#title").val().trim();
        t.customerName=$('#advancedsearch').find("#customerName").val().trim();
        var cancel = false;
        if(!isEmail(t.toMail)){
            $('#advancedsearch').find("#toMail").warning("");
            cancel = true;
        }
        if((t.title)==''){
            $('#advancedsearch').find("#title").warning("");
            cancel = true;
            
        }
        if(cancel){
           
            return
        }
        var _this = $(this);


        var postmail =false;

         html2canvas(
            document.getElementById("onebookingorder"),
            {
                onrendered: function(canvas) {
                if (canvas.msToBlob) { //for IE
                    var data = canvas.msToBlob();
                   
                }
                else{
                    var data = canvas.toDataURL("image/png",2);
                    
                }

                imgData = data.replace(/^data:image\/(png|jpg|jpeg);base64,/, "");
                jQuery("#showimg").attr("src",data)
                jQuery.ajax({
                    url: '/Orders/UploadPic',
                    contentType: "application/json; charset=utf-8;",
                    data:JSON.stringify({
                        imageData: imgData,
                        imageName: $('#picturetitle').text()+'.png'
                    }),
                    type: 'post',
                    dataType: 'json',
                    beforeSend:function(xhr){
                         _this.text("正在发送图片..")
                        _this.prepend('<i class="fa fa-spinner fa-spin"></i>');
                        
                    },
                    success:function(data){
                        if(data.ErrorCode==200){
                            jQuery.ajax({
                                url: '/Orders/SendMail',
                                contentType: "application/json; charset=utf-8;",
                                data: JSON.stringify({
                                    OrderID:$('#OrderID').val(),
                                    title:t.title, 
                                    toMail:t.toMail,
                                    customerName:t.customerName, 
                                    fileName:$('#picturetitle').text()+'.png',
                                    filePath:data.Pic_Path
                                }),
                                type: 'post',
                                dataType: 'json',
                                success:function(data2){
                                    if(data.errorCode==200){
                                        
                                    }
                                },
                                beforeSend:function(xhr){
                                    _this.text("正在发送邮件..")
                                    _this.prepend('<i class="fa fa-spinner fa-spin"></i>');
                                    
                                },
                                complete:function(XHR, TS){
                                    var response = JSON.parse(XHR.responseText);
                                    if(TS=="success"&&response.statusCode==200){
                                       postmail=true;

                                        setTimeout(function(){
                                            _this.find('i').remove();
                                            // _this.text("发送邮件成功！ ");
                                            _this.success("发送邮件成功！ ");
                                        },500)
                                        setTimeout(function(){
                                            _this.closest('#advancedsearch').modal("hide");
                                        },1000)

                                    }
                                    else{
                                        setTimeout(function(){
                                            _this.find('i').remove();
                                            _this.text("发送邮件失败！ ");
                                        },500)
                                    }
                                }
                                
                            })
                        }
                        else{
                             setTimeout(function(){
                                _this.find('i').remove();
                                _this.text("发送图片失败！");
                            },500)
                        }
                    },
                    
                    complete:function(XHR, TS){
                        if(TS!=="success"){
                             setTimeout(function(){
                                _this.find('i').remove();
                                _this.text("发送图片失败！");
                            },500)

                        }
                        
                    }
                })

                
                
            }
            }
        )
     })




     jQuery.fn.onlyNum = function () {
        jQuery(this).keypress(function (event) {
            var eventObj = event || e;
            var keyCode = eventObj.keyCode || eventObj.which;
            
            if ((keyCode >= 48 && keyCode <= 57)){	
                return true;
            }
            else{
                return false;
            }
        })
        jQuery(this).bind("change",function(){
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

   
    
})
