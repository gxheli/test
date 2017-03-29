var DestinationRow = React.createClass({
    
    prehandledata:function(data){
        if(data=='emptyrow'){
            var obj = {};
            obj[this.props.data.elements.single.code] ="";
            return (obj);
        }
        else{
            return data;  
        }
    },
    prehandleupdatedata:function(data){
        return data;
    },
    
    isFullSpace:function(str){
        var regu = "^[\s]+$";
        var re = new RegExp(regu);
        if(re.test(str)){
            return true;
        }
        return false;
    },
    
  
    mixins :[MXINS_UPDATE_DATA],
    detectValueState:function(value){
        var isValueUseAble = 1;
        var isValueNotEmpty = 1;
        if(!(value)){
            this.setState(
            {
                classname:"help-inline tips",
                tips:"请填写"
            })
            isValueUseAble = 0;
            isValueNotEmpty  =0 ;
        }
        else{
             this.setState(
            {
                classname:"help-inline",
                tips:this.props.data.elements.single.tips
            })
            isValueUseAble = 1;
            isValueNotEmpty = 1;
        }
        var stateObj = new Object();
        stateObj[this.props.suborderid]=new Object();
        stateObj[this.props.suborderid][this.props.data.elements.single.code] = new Object();
        stateObj[this.props.suborderid][this.props.data.elements.single.code].isValueUseAble = isValueUseAble;
        stateObj[this.props.suborderid][this.props.data.elements.single.code].isValueNotEmpty = isValueNotEmpty;
        stateObj[this.props.suborderid][this.props.data.elements.single.code].isValueMandatory = this.props.data.mandatory;
        this.props.reciveValueState(stateObj)
        
    },
    defaultValueWhenMounted :function(){
        if(this.getDefaultValue()=="emptyrow"){
             this.setState(
                {
                    classname :'help-inline',
                    tips:this.props.data.elements.single.tips
                }
            );
            var stateObj = new Object();
            stateObj[this.props.suborderid]=new Object();
            stateObj[this.props.suborderid][this.props.data.elements.single.code] = new Object();
            stateObj[this.props.suborderid][this.props.data.elements.single.code].isValueUseAble = 0;
            stateObj[this.props.suborderid][this.props.data.elements.single.code].isValueNotEmpty = 0;
            stateObj[this.props.suborderid][this.props.data.elements.single.code].isValueMandatory = this.props.data.mandatory;
            this.props.reciveValueState(stateObj)
        }
        else{
            this.detectValueState(this.state.postValue[this.props.data.elements.single.code]);
        }
    },
    handlerChange : function (e){
       
        var obj = {};
        obj[this.props.data.elements.single.code] =e.target.value;

          //回复默认值
        if(this.props.UIType=="reChange"){
             if(!(e.target.value.toString().trim())){
                var lastValue = this.prehandledata(this.props.beforeRechangeValue);
                obj[this.props.data.elements.single.code] = lastValue[this.props.data.elements.single.code];
            }
        }
        this.detectValueState(obj[this.props.data.elements.single.code]);
       
        this.setState(
            {
                postValue :obj,
                    
            },
            function(){
                this.upadteToForm(this.getdUpdateData());


                 var obj = {}
                 obj[this.props.data.elements.single.code] = this.state.postValue[this.props.data.elements.single.code];
                 this.props.reciveTempValue(obj);
            }
        )
    },
      getInitialState:function () {
        return{
            classname:"inline-help",
            tips:this.props.data.elements.single.tips
        }
      

    },
    componentDidMount: function(e) {
        // 可以使用this.getDOMNode()
        // 引用外部库的钩子函数
        // react 运行在服务端时候，该方法 不被调用
        var obj = {}
        obj[this.props.data.elements.single.code] = this.state.postValue[this.props.data.elements.single.code];
        this.props.reciveTempValue(obj);
        
        this.defaultValueWhenMounted();
        

    },
    render : function () {
        var suborderid  = this.props.suborderid ;
        var componentid  = this.props.componentid ;
        var segmentcode  = this.props.data.elements.single.code ;
        
        if(this.props.UIType=="edit"){
            return(
                <div className="form-group"  data-suborderid ={suborderid} data-componentid={componentid} data-segmentcode ={segmentcode} >
                    <Lable params ={{isMust:this.props.data.mandatory==1?true:false,title:this.props.data.elements.single.title}} ></Lable>
                    <div className="col-md-10">
                        <input type="text" className="form-control input-inline input-medium  input-sm"  defaultValue={this.state.postValue[segmentcode]} onPaste = {this.handlerChange} onChange={this.handlerChange} />
                        <span className={this.state.classname}>{this.state.tips}</span>
                    </div>
                </div>
            )
        }
        else if(this.props.UIType=="reChange"){
            var lastValue = this.prehandledata(this.props.beforeRechangeValue);
            var valueChange = 0;
           
            if(
                lastValue[this.props.data.elements.single.code] == this.state.postValue[this.props.data.elements.single.code]||
                (this.state.postValue[this.props.data.elements.single.code]=="")
            ){
                valueChange = 0;
                var valueNoW = '';
            }
            else{
                valueChange = 1;
                var  valueNoW = this.state.postValue[this.props.data.elements.single.code];

            }
               
            var changeObj = new Object();
            changeObj[this.props.suborderid]=new Object();

            changeObj[this.props.suborderid][this.props.data.elements.single.code] =  new Object();
            changeObj[this.props.suborderid][this.props.data.elements.single.code].isValueChange = valueChange;
            
        
            this.props.reciveValueChange(changeObj);
            return(
                <tbody>
                    <tr>
                        <td>{this.props.data.elements.single.title}</td>
                        <td>{this.getDefaultValue()[this.props.data.elements.single.code]}</td>
                        <td>    
                            <input type="text" className="form-control input-inline input-medium  input-sm"  defaultValue={valueNoW} onPaste = {this.handlerChange} onChange={this.handlerChange} />
                            <div> <span className={this.state.classname}>{this.state.tips}</span></div>
                        </td>
                    </tr>
                    
                </tbody>
            )
        }
        
    }
})
