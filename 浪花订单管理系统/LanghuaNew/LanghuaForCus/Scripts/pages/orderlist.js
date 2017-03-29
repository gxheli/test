jQuery(document).ready(function(){
    

    // var statusMap = {}
    //     {
    //         id:0,
    //         text:"未处理",
    //         showoutside:0,
    //         type:   
    //     },
    //     {
    //         id:1,
    //         text:"未核对",
    //         showoutside:0,
    //         type:   
    //     },
    //     {
    //         id:2,
    //         text:"已核对",
    //         showoutside:0,
    //         type:    
    //     },
    //     {
    //         id:3,
    //         text:"已发送",
    //         showoutside:0,
    //         type:    
    //     },
    //     {
    //         id:4,
    //         text:"已接单",
    //         showoutside:0,
    //         type:   
    //     },
    //     {
    //         id:5,
    //         text:"已确认",
    //         showoutside:0,
    //         type:  
    //     },
    //     {
    //         id:6,
    //         text:"再次确认",
    //         showoutside:0,
    //         type:  
    //     },
    //     {
    //         id:7,
    //         text:"已满",
    //         showoutside:0,
    //         type:  
    //     },
    //     {
    //         id:8,
    //         text:"已取消",
    //         showoutside:0,
    //         type:  
    //     },
    //     {
    //         id:9,
    //         text:"再次确认取消",
    //         showoutside:0,
    //         type:   
    //     },
    //     {
    //         id:10,
    //         text:"请求修改",
    //         showoutside:0,
    //         type:    
    //     },
    //     {
    //         id:11,
    //         text:"已接单（请求修改）",
    //         showoutside:0,
    //         type:   
    //     },
    //     {
    //         id:12,
    //         text:"拒绝修改",
    //         showoutside:0,
    //         type:  
    //     },
    //     {
    //         id:13,
    //         text:"请求取消",
    //         showoutside:0,
    //         type:    
    //     },
    //     {
    //         id:14,
    //         text:"已接单（请求取消）",
    //         showoutside:0,
    //         type:  
    //     },
    //     {
    //         id:15,
    //         text:"拒绝取消",
    //         showoutside:0,
    //         type:  
    //     },
    // ] 
    
    var buttonRadio ={
        showOutState:[
            {
               value:-1,
               text:"全部",
            },
            {
               value:1,
               text:"state1",
            },
            
            {
               value:2,
               text:"state2",
            }
        ],
        selectInState:[
            {
               value:-1,
               text:"全部",
            },
            {
               value:3,
               text:"state3",
            },
            {
               value:4,
               text:"state4",
            },
           
            
            
        ],
        labelOut:[
            {
               value:1,
               text:"要售后",
            },
            {
               value:1,
               text:"未付完",
            }
        ]
    }
   ;(function($) {
        jQuery.fn.buttonRadio = function( options ) {
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
                str += '<select class="input input-sm input-small ">';
                
                for(var i in obj.selectInState){
                    str += makeOneOption(obj.selectInState[i]);
                }
                str += '</select>';
                
                return str;
            }
            function makeOneRadioButton(obj){
                var str ='<a href="javascript:" class="buttonradio" data-code="'+obj.value+'">'+obj.text+'</a>\n';
                return str;
            }
            function makeOneOption(obj){
                var str ='<option value="'+obj.value+'">'+obj.text+'</option>';
                return str;
            }
        
        };
    })(jQuery);
    jQuery('#testb').buttonRadio({
        data:buttonRadio,
        selected:function(dom,code){
            console.log(dom);
            console.log(code);
        }
    });

    
    /*对表格进行初始化处理*/
    //表格数据交互控制
    $('#orderList').eq(0)
	    .on('preXhr.dt', function ( e, settings, json ) {
            
            
            // 删除插件无必要项目
            delete json.columns;
            delete json.order;
            delete json.search;
            
            console.log(json);
    } ).on('xhr.dt', function ( e, settings, json, xhr ) {
    } )
    
    // 表格非数据逻辑
    var neworders =
	jQuery('table#orderList')	
		.DataTable( {
            
			ordering:false,
            
            searching:false,
            
		    serverSide:true,
            
			ajax: {
                url:window.location.origin+"/Orders/GetOrders",
                type:'post',
            },
            
			dom:
             '<t>i'+
                '<"paddingbottom"'+ 
                '>'+
                '<"pBottom"'+
                    '<"ddpgroup"  <"ddone" l><"#pi.ddone" ><"#cp.ddone" ><"ddone" p>'+
                '>'+
            '>',
            
			initComplete:function(settings, json){
               console.log(json);
			},
            
			drawCallback: function( settings ) {
				// 更新页数
				var api = this.api();
				$("#cp.ddone").text(api.page.info().page+1+'/'+api.page.info().pages+'页');
                //更新提示数据
                
			},
            
            //行操作
			rowId:"ServiceItemID",
			createdRow: function( row, data, dataIndex ) {
				var _this = this.api();
                var thisTable = this;
				jQuery(row).on('click','.cancel',function(){
					_this 
						.row(row)
						.remove()
						.draw();	
				});
                //缓存有用的单行数据
                jQuery(row).data({
			       
                });
			},
            
            //列操作
			columns: [
                
                //左格选择
				{   
					'data':'AdultNum',
					"createdCell": function (td, cellData, rowData, row, col) {
						jQuery(td).html('<input type="checkbox" class="checkboxes">');
					}
				} ,
                
                //订单号
                {
					'data':'OrderNo',
					"createdCell": function (td, cellData, rowData, row, col) {
						jQuery(td).html('<div  class="bright">'+cellData+'</div><div class="mini">'+rowData.CreateUserNikeName+'</div>');
					}
				} ,
                
                //淘宝id
                {
					'data':'TBID',
					"createdCell": function (td, cellData, rowData, row, col) {
						jQuery(td).html('<div>'+cellData+'</div><div class="mini">'+rowData.OrderSourseName+'</div>');
					}
				} ,
                
                // 姓名
                {
					'data':'CustomerName',
					"createdCell": function (td, cellData, rowData, row, col) {
						jQuery(td).html('<div>'+cellData+'</div><div class="mini">'+rowData.CustomerEnname+'</div>');
					}
				} ,
                
               //预定项目
                {
					'data':'cnItemName',
					"createdCell": function (td, cellData, rowData, row, col) {
					    jQuery(td).html('<span>' +'（'+rowData.SupplierCode+'）'+ cellData + '</span><span class="bright">#' + rowData.ServiceCode + '</span>');
					}
				} ,
                
                //人数
                {
					'data':'AdultNum',
					"createdCell": function (td, cellData, rowData, row, col) {
						jQuery(td).html(cellData+' / '+rowData.ChildNum+' / '+rowData.INFNum);
					}
				} ,
                
                //日期
                {
					'data':'CreateTime',
					"createdCell": function (td, cellData, rowData, row, col) {
						jQuery(td).html(' <div>'+cellData+'</div><div class="mini">城内 - 城内</div>');
					}
				} ,
                
                //状态
                {
					'data':'state',
					"createdCell": function (td, cellData, rowData, row, col) {
                        var statusMap = {
                            0:{
                                text:"未处理",
                                color:""
                            },
                            1:{
                                text:"未核对",
                                color:""
                            },
                            2:{
                                text:"已核对",
                                color:"#008000"
                            },
                            3:{
                                text:"已发送",
                                color:"#FF0000"
                            },
                            4:{
                                text:"已接单",
                                color:""
                            },
                            5:{
                                text:"已确认",
                                color:""
                            },
                            6:{
                                text:"再次确认",
                                color:""
                            },
                            7:{
                                text:"已满",
                                color:""
                            },
                            8:{
                                text:"已取消",
                                color:""
                            },
                            9:{
                                text:"再次确认取消",
                                color:""
                            },
                            10:{
                                text:"请求修改",
                                color:"#CC0000"
                            },
                            11:{
                                text:"已接单（请求修改）",
                                color:""
                            },
                            12:{
                                text:"拒绝修改",
                                color:""
                            },
                            13:{
                                text:"请求取消",
                                color:"#999999"
                            },
                            14:{
                                text:"已接单（请求取消）",
                                color:""
                            },
                            15:{
                                text:"拒绝取消",
                                color:""
                            },
                        }
                        var style = 'style="color:'+statusMap[cellData].color+'"';
                        var text = statusMap[cellData].text;
                        if(rowData.IsNeedCustomerService==1){
                            var saletag = '<div><span class="spanlabel after-sale-service-color">要售后</span></div>';
                        }
                        else
                           var saletag = '';
                           
                        if(rowData.IsPay ==1){
                            var uppaidtag= '<li><a class="cancelnotpaid" href="#">取消未付完</a></li>';
                        }
                        else
                           var unpaidtag = '';
						jQuery(td).html('<div '+style+'>'+text+'</div><div >'+saletag+unpaidtag);
					}
				} ,
                
                //备注
                {
					'data':'Remark',
					"createdCell": function (td, cellData, rowData, row, col) {
						// jQuery(td).html('');
					}
				} ,
                
                //操作
                {
					'data':'OrderID',
					"createdCell": function (td, cellData, rowData, row, col) {
                        if(rowData.Aftersale==1){
                            var saletext = '<li><a class="cancelaftersale" href="#">取消售后</a></li>';
                        }
                        else
                           var saletext = '<li><a class="getaftersale" href="#">要售后</a></li>';
                           
                        if(rowData.Notpaid ==1){
                            var paidtext= '<li><a class="cancelnotpaid" href="#">取消未付完</a></li>';
                        }
                        else
                           var paidtext = '<li><a class="setnotpaid" href="#">未付完</a></li>';
                           
                           
                       
                        var str =
                        '<div class="row">'+
                            '<div class="col-xs-6"><a href="javascript:" class="hrefInTable">修改</a></div>'+
                            '<div class="col-xs-6"><a href="javascript:" class="hrefInTable">邮箱</a></div>'+
                        '</div>'+
                        '<div class="row">'+
                            '<div class="col-xs-6"><a href="javascript:" class="hrefInTable">备注</a></div>'+
                            '<div class="col-xs-6">'+
                                '<div class="dpdContainer down rightBased  ">'+
                                    '<a href="#" class="dropdown-toggle hrefInTable" data-toggle="dropdown">'+
                                        '更多'+
                                        '<b class="caret"></b>'+
                                    '</a>'+
                                    '<ul class="dropdown-menu " role="menu">'+
                                        saletext+
                                        paidtext+
                                        '<li><a class="toCopy" href="#">复制链接</a></li>'+
                                        '<li><a href="#">操作日志</a></li>'+
                                    '</ul>'+
                                '</div>'+
                            '</div>'+
                        '</div>';
						jQuery(td).html(str);
                        
					}
				} 
				
				
				
			]
    });
});








