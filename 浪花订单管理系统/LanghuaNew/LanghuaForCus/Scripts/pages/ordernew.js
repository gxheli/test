 jQuery(document).ready(function ($) { 
  
  
    // only numbers
    jQuery.fn.onlyNum = function () {
        jQuery(this).keypress(function (event) {
            console.log("onlynumkeypress")
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

    
    
    // 数据表格开始
    // ////////////
    
    var neworders =
	jQuery('table#itemListNewOrder')	
		.DataTable( {
			ordering:false,
			// stateSave: true,
			
			dom:"t",
			initComplete:function(settings, json){
				// 用于数据加载完成
				var api = this.api();
               jQuery(this).bind("addItems",function(event,data){
                    console.log(data);
                    api.row.add(data).draw();
               });
               jQuery(this).bind("updateAllCost",function(e){
                    //更新代价提示
                    var allCost = 0;
                    jQuery(this).find('tr td span.cost').each(function(){
                        allCost += parseInt(jQuery(this).text());
                    })
                    console.log(allCost);
                    jQuery('.newordercost  .cost').eq(0).text(allCost);
                })
			},
			"drawCallback": function( settings ) {
				// 更新页数
				var api = this.api();
				$("#cp.ddone").text(api.page.info().page+1+'/'+api.page.info().pages+'页');
                //更新提示数据
                
			},
			"rowId":"ServiceItemID",
			"createdRow": function( row, data, dataIndex ) {
				var _this = this.api();
                var thisTable = this;
				jQuery(row).on('click','.cancel',function(){
					_this 
						.row(row)
						.remove()
						.draw();	
                    jQuery(thisTable).trigger('updateAllCost');
                    
				});
                //缓存单行数据
                jQuery(row).data({
			        "rowId"         :   data.ServiceItemID,
                    "SupplierID"    :   data.ItemSupliers.SupplierID,
                    'payType'       :   data.SupplierServiceItemView.PayType,
                    "adult"         :   data.SupplierServiceItemView.ItemPriceBySupplier.AdultNetPrice,
                    "child"         :   data.SupplierServiceItemView.ItemPriceBySupplier.ChildNetPrice,
                    "infant"        :   data.SupplierServiceItemView.ItemPriceBySupplier.BobyNetPrice,
                    "room"          :   data.SupplierServiceItemView.ItemPriceBySupplier.Price,
                    "ServiceTypeID" :   data.ServiceTypeID,
                });
                
                console.log(jQuery(row).data());
                
			    //更新提示数据
                jQuery(row).addClass("itemRow");
                jQuery(row).find('input.baseService').onlyNum();
                jQuery(row).on("change","input.baseService, select.extraService",function(){
                    console.log("Up");
                   //先更新单行提示数据
                    // 按人头计费
                    console.log(jQuery(row).data('payType'));
                    if(jQuery(row).data('ServiceTypeID')!=4){
                        
                        var baseServiceCost= 
                            Number(jQuery(row).find('input.adult').eq(0).val())*Number(jQuery(row).data('adult'))+
                            Number(jQuery(row).find('input.child').eq(0).val())*Number(jQuery(row).data('child'))+
                            Number(jQuery(row).find('input.infant').eq(0).val())*Number(jQuery(row).data('infant'));
                            console.log(baseServiceCost);
                            
                            
                         var extraServiceCost = 0 ;
                         jQuery(row).find('td select.extraService').each(function(){
                             extraServiceCost +=
                                Number(jQuery(this).attr("unitpice"))* Number(jQuery(this).val());
                         })
                         console.log(extraServiceCost);
                         var  costAll =parseInt( baseServiceCost+extraServiceCost) ;
                            
                         jQuery(row).find("td span.cost").text(costAll)
                            
                    }
                    //按产品数量计费,需要计算间数和晚数
                    else if($(row).data('payType')==4){
                        var baseServiceCost= 
                               Number(jQuery(row).find('input.nights').eq(0).val())*Number(jQuery(row).find('input.numbers').eq(0).val())*Number(jQuery(row).data('room'));
                            
                        console.log(baseServiceCost);
                            
                         var extraServiceCost = 0 ;
                         jQuery(row).find('td select.extraService').each(function(){
                             extraServiceCost +=
                                Number(jQuery(this).attr("unitpice"))* Number(jQuery(this).val());
                         })
                         console.log(extraServiceCost);
                         var  costAll =parseInt( baseServiceCost+extraServiceCost) ;
                         
                         
                         jQuery(row).find("td span.cost").text(costAll);
                    }
                    
                   // 然后在更新整单提示数据
                    jQuery(thisTable).trigger('updateAllCost')
                });
                
                
			},
			"columns": [
				{
					'data':'cnItemName',
					"createdCell": function (td, cellData, rowData, row, col) {
						jQuery(td).html('');
					}
				} ,
				{
					'data':'cnItemName',
					"createdCell": function (td, cellData, rowData, row, col) {
					}
				} ,
                // 基础费用
				{
					'data':'cnItemName',
					"createdCell": function (td, cellData, rowData, row, col) {
                      
                         var priceKeys ={
                            "AdultNetPrice":{
                                position:0,
                                className:'adult',
                            },
                            "ChildNetPrice":{
                                position:1,
                                className:'child',
                            },
                            "BobyNetPrice":{
                                position:2,
                                className:'infant',
                            },
                           
                           
							'maxLength' :3
                       }
                           
                       var itemPriceBySupplier = rowData.SupplierServiceItemView.ItemPriceBySupplier;

                       var arr ={}
                       for(var i in priceKeys){
                           if(i=='maxLength'){
                               continue;
                           }
                           if(i in itemPriceBySupplier){
                               arr[priceKeys[i]['position']] = '<input type="text" which="'+priceKeys[i]['className']+'" class="'+priceKeys[i]['className']+' baseService  service roomnumber">';
                           }
                       }
                       
                       
                       var str ='';
                       
                       for(var j=0 ;j<priceKeys.maxLength;j++){
							if(j in arr){
								str+=arr[j];
							}
							else{
							 	str+=
								 '<span style="width:40px;height:30px;display:inline-block;position:relative;vertical-align:top"></span>';
							}
						}
                        
                        if(rowData.ServiceTypeID==4){
                            str +='<input type="text"  which="night" class="roomnumber nights"><input type="text" which="numbers" class="roomnumber numbers">';
                        }
						jQuery(td).html(str);
                        
                                                
						
					}
				} ,
                // 额外费用
				{
					'data':'cnItemName',
					"createdCell": function (td, cellData, rowData, row, col) {
					    var extraServices = rowData.ExtraService;
                        var str = '';
                        for(var k in extraServices){
                            var extraService = extraServices[k];
                            
                            
                            str +=' <select id="ExtraService'+extraService.ExtraServiceID+'" title="最少选'+extraService.MinNum+'项"    min="'+extraService.MinNum+'" unitpice="'+extraService.ExtraServicePrices.ServicePrice+'"  class="extraService service types">';
                            var  altOption ='<option value="0" >'+extraService.ServiceName+'</option>';
                            str += altOption;
                                var max  = extraService.MaxNum;
                                var min  = extraService.MinNum;
                                var unit = extraService.ServiceUnit;
                                str+=makeOneOption(min,max,unit);
                            str+='</select>';
                        }
                        function makeOneOption(min,max,unit){
                            console.log(min,max, unit)
                            var str ='';
                            var MIN = min>=1?min:1;
                            for(var i = MIN ; i<=max;i++){
                                str +='<option value="'+i+'" >'+i+" "+unit+'</option>';
                            }
                            console.log(str);
                            return str;
                        }

                       $(td).html(str);
					}
				} ,
				{
					'data':'cnItemName',
					"createdCell": function (td, cellData, rowData, row, col) {
                            var str = '';
                            str+=
                            '<span class="cost">0</span>'+
                            '<span class="cancel" title="删除该产品">×</span>';
                            jQuery(td).html(str);
					}
				} ,
				{
					'data':'cnItemName',
					"createdCell": function (td, cellData, rowData, row, col) {
							jQuery(td).html('');
					}
				} 
				
				
			]
    });
    
    
    
    
    
    
    
    
    
    
    // 数据表格结束
    
    // 提交表单开始
    
    jQuery('#postSlectItems').bind('click',function(){
       
        
        var canPostResult = getPost();
        console.log(canPostResult);
        if (!canPostResult.post) {
            if(canPostResult.scrollTop){
                jQuery('body').animate({ scrollTop: 0 }, 200);
            }
            
           jQuery('.warning').one('click',function(){
               console.log(this)
               jQuery(this).removeClass("warning");
           })
             
           jQuery('.warning').one('foucs',function(){
               console.log(this)
               jQuery(this).removeClass("warning");
           })
           jQuery('#itemListNewOrder').on('focus','.warning',function(){
               console.log(this)
               jQuery(this).removeClass("warning");
           })
        }
        else {
            if (canPostResult.postObj.Orders.length == 0) {
                return;
            }
            $.ajax({
                type:'post',
                dataType:'json',
                contentType: "application/json; charset=utf-8;",
                data: JSON.stringify(canPostResult.postObj),
                url: window.location.origin+'/Orders/CommitOrders',
                success:function(data){
                   if(data.ErrorCode==200){
                       window.location.href = window.location.origin+'/Orders/OrderFinish?totalCost='+jQuery('.newordercost .cost').eq(0).text() +'&TBOrderID='+ data.data.TBOrderID;

                   }
                }
            })
        }
      
        function getPost(){
           var post = true;
           var scrollTop = false;
           var postObj ={
                    "TBID":(function(){
                        var value = jQuery.trim(jQuery('#TBID').val())
                        if(!value){
                            jQuery('#TBID').addClass('warning');
                            post = false;
                            scrollTop = true;

                                
                        }
                        return  value;
                    })(),
                    "OrderSourseID":(function(){
                        var value = jQuery('#source input.source[name=source]:checked').val();
                        if(!value){
                            jQuery('#source').addClass('warning');
                            scrollTop = true;
                            post = false;
                                
                        }
                        return  value;
                    })(),
                        
                    "TBNum":(function(){
                        var noSpace=new RegExp(" ","g");
                        var text =(jQuery('#TBORDERID').val()).replace(noSpace,"");
                        text = text.split("\n").join();
                        
                        if(!text){
                            jQuery('#TBORDERID').addClass('warning');
                            scrollTop = true;
                            post = false;
                                
                        }
                        return  text;
                    })(),
            }
            
            
            var orders = [];
            
            jQuery('#itemListNewOrder tbody tr.itemRow').each(function () {
                console.log(this)
                 //检查基础收费
                var oneItem = {};
                oneItem.SupplierID= jQuery(this).data('SupplierID');
                oneItem.ItemID =jQuery(this).data('rowId');
                jQuery(this).find('td .baseService').each(function(){
                    var map = {
                        adult:'AdultNum',
                        child:'ChildNum',
                        infant:'INFNum',
                        nights:'RoomNum',
                        numbers:'RightNum'
                    }
                    var thisValue = jQuery.trim(jQuery(this).val());
                    if(!thisValue){
                        post =false;
                        jQuery(this).addClass('warning');
                    }
                    else{
                        oneItem[map[jQuery(this).attr('which')]] = parseInt(jQuery(this).val());
                    }
                })
                
                // 检查额外收费
                 oneItem.ExtraServiceHistorys=[];
                 
                jQuery(this).find('td .extraService').each(function(){
                    var thisValue = Number(jQuery(this).val());
                    var thisMin = Number(jQuery(this).attr('min'));
                    
                    if(thisValue < thisMin){
                        post =false;
                        jQuery(this).addClass('warning');
                    }
                    else{
                        oneItem.ExtraServiceHistorys.push({
                            ExtraServiceID:jQuery(this).attr('id').split('ExtraService')[1],
                            ServiceNum:thisValue
                        })
                    }
                })
                orders.push(oneItem);
            })
            
            postObj.Orders = orders;
            console.log(post);
            return {
                scrollTop:scrollTop,
                post:post,
                postObj:postObj
            };
        }
        
    })
    
    // 提交表单结束
    var remote_cities = new Bloodhound({
        datumTokenizer: function (d) {
            return Bloodhound.tokenizers.whitespace(d.name);
        },
        queryTokenizer: Bloodhound.tokenizers.whitespace,
        limit: 15,
        // 在文本框输入字符时才发起请求
        // 
        // local:dt,
        remote: {
            wildcard: '%QUERY',
            url: window.location.origin + '/Orders/GetItemsByStr?Str=%QUERY',
            filter: function (data) {
                console.log(data);
                return $.map(data.Items, function (country) {
                    return {
                        name: country.cnItemName,
                        supplyer: country.ItemSupliers,
                        defaultSupplierID:country.DefaultSupplierID,
                        serviceItemID:country.ServiceItemID
                    };
                });
            }
           
        }
    });

    remote_cities.initialize();


    $('#typeahead').typeahead({
        hint: true,
        highlight: false,
        minLength: 1,
    },
    {
        name: 'xxx',
        displayKey: 'name',
        limit: 15,
        source: remote_cities,
        templates: {
            empty: [
                '<div class="empty-message">',
                '没有找到相关数据',
                '</div>'
            ].join('\n'),
            header: [
                '<div class="empty-message">',
                '搜索到的数据',
                '</div>'
            ].join('\n'),
            footer: [
                '<div class="empty-message">',
                '结束',
                '</div>'
            ].join('\n'),
            // suggestion: Handlebars.compile('<p><strong>{{CityName}}</strong> - {{idx}}</p>')
        }
    });

    $('#typeahead').bind('typeahead:active', function (ev, suggestion) {
        console.log("active");
        console.log(suggestion);
    });
    $('#typeahead').bind('typeahead:idle', function (ev, suggestion) {
        console.log("idle");
        console.log(suggestion);
    });
    $('#typeahead').bind('typeahead:open', function (ev, suggestion) {
        console.log("open");
        console.log(suggestion);
    });
    $('#typeahead').bind('typeahead:close', function (ev, suggestion) {
        console.log("close");
        console.log(suggestion);
    });

    $('#typeahead').bind('typeahead:change', function (ev, suggestion) {
        console.log("change");
        console.log(suggestion);
    });
    $('#typeahead').bind('typeahead:render', function (ev, suggestion) {
        console.log("render");
        console.log(suggestion);
    });

    $('#typeahead').bind('typeahead:select', function (ev, suggestion) {
        console.log("select");
        console.log(suggestion);
        
        $(this).data('which',suggestion.serviceItemID);
        console.log( $(this).data('which'));
        // 供应商更改
        var supplier = suggestion.supplyer;
        var optGroupStr ='';
        for(var i in supplier){
            optGroupStr+=
                makeOneOption(supplier[i],suggestion.defaultSupplierID);
        }
        $('#suppliers').empty().append(optGroupStr);
        
        function makeOneOption(obj,defaultSupplierID){
            if(obj.SupplierID==defaultSupplierID){
                var selected = 'selected="selected"';
                var altName = obj.SupplierName + "(默认)";
            }
            else{
                var selected = '';
                var altName = obj.SupplierName;

            }
            var str ="";
            str ='<option value="'+obj.SupplierID+'"    '+selected+'>'+altName+'</option>';
            return str;
        }
    });

    $('#typeahead').bind('typeahead:autocomplete', function (ev, suggestion) {
        console.log("autocomplete");
        console.log(suggestion);
    });


    $('#typeahead').bind('typeahead:cursorchange', function (ev, suggestion) {
        console.log("cursorchange");
        console.log(suggestion);
       
    });

    $('#typeahead').bind('typeahead:change', function (ev, suggestion) {
        console.log("change");
        console.log(suggestion);
    });
    
    
    jQuery('#addOneItem').bind("click",function(){
        if(!jQuery('#typeahead').data('which')&&!jQuery('#suppliers').val()){
            return;
        }
        $.ajax({
            url: window.location.origin + '/Orders/GetItemByID?ItemID=' + jQuery('#typeahead').data('which') + '&SupplierID=' +jQuery('#suppliers').val(),
            type:'get',
            dataType:'json',
            success:function(data){
                if(data.ErrorCode!=200){
                    return;
                }
                console.log(data.Item)
                $('#itemListNewOrder').trigger("addItems", [data.Item]);
            }
        }) 
    })


})



