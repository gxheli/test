var HotelNameRow = React.createClass({
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
    isPropertyHotelName:function(str){
        var regu = "^[^\u4e00-\u9fa5]+$";
        var re = new RegExp(regu);
        if (re.test(str)){
            return true;
        }
        else{
            return false;
        }
    },
    componentDidMount: function(e) {
        // 可以使用this.getDOMNode()
        // 引用外部库的钩子函数
        // react 运行在服务端时候，该方法 不被调用
        this.detectValueWhenMounted();

        var obj = {};
        obj[this.props.suborderid] = new Object();
        obj[this.props.suborderid][this.props.data.elements.single.code] = this.state.postValue[this.props.data.elements.single.code];
        this.props.reciveTempValue(obj);

        //更新粘贴板内容
        var copyObj ={};
        copyObj[this.props.data.elements.single.code]={
            title:this.props.data.elements.single.title,
            str:this.state.postValue[this.props.data.elements.single.code]
        }
        this.updateCopyObj(copyObj);

        var _this  = this;
        var cityid = this.props.data.areaID;
        var remote_cities = new Bloodhound({
            datumTokenizer: function (d) {
                return Bloodhound.tokenizers.whitespace(d.HotalName);
            },
            queryTokenizer: Bloodhound.tokenizers.whitespace,
            limit: 15,
            remote: {
                wildcard: '%QUERY',
                url: window.location.origin + '/Hotals/GetHotal?CityID='+cityid+'&Str=%QUERY',
                filter: function (data) {
                    var length = data.hotals.length;
                    return data.hotals.map( function (country) {
                        return {
                            name: country.HotalName,
                            count:length
                        };
                    });
                }

            }
        });

        remote_cities.initialize();

        jQuery((this.refs.hotelName)).typeahead({
            hint: false,
            highlight: true,
            minLength: 1,
            classNames: {
                menu:(function(){
                    var width = document.documentElement.clientWidth;
                    if(width<500){
                        return 'tt-menu tt-menu-top';
                    }
                    return 'tt-menu';
                })()
            }
        },
        {
            name: 'xxx',
            displayKey: 'name',
            limit: 30,
            source: remote_cities,
            templates: {
                header:function(data){
                    return([
                    '<div class="empty-message">',
                        '共搜索到<strong>'+data.suggestions.length+'</strong>个酒店',
                        '</div>'
                    ].join('\n')
                    );
                }
            }
        }).bind('typeahead:select', function (ev, oneselected){
            var obj = {};
            obj[_this.props.data.elements.single.code] =oneselected.name;

            //回复默认值
            if(_this.props.UIType=="reChange"){
                if(!( obj[_this.props.data.elements.single.code].trim())){
                    var lastValue = _this.prehandledata(_this.props.beforeRechangeValue);
                    obj[_this.props.data.elements.single.code] = lastValue[_this.props.data.elements.single.code];
                }
            }
            _this.detectValueState(obj[_this.props.data.elements.single.code]);
            _this.setState(
                {
                    postValue :obj,
                },
                function(){
                    _this.upadteToForm(_this.getdUpdateData());
                    var obj = {};
                    obj[_this.props.suborderid] = new Object();
                    obj[_this.props.suborderid][_this.props.data.elements.single.code] = _this.state.postValue[_this.props.data.elements.single.code];
                    _this.props.reciveTempValue(obj);
                     //更新粘贴板内容
                    var copyObj ={};
                    copyObj[_this.props.data.elements.single.code]={
                        title:_this.props.data.elements.single.title,
                        str:_this.state.postValue[_this.props.data.elements.single.code]
                    }
                    _this.updateCopyObj(copyObj);
                }
            )
        })

    },


    mixins :[MXINS_UPDATE_DATA],

    detectValueState:function(value){
        var isValueUseAble = 1;
        var isValueNotEmpty = 1;
        var classname = "";
        var tips = "";
        if(!(value)||!this.isPropertyHotelName(value)){
            classname="help-inline tips";
            tips="请填写英文全称，不要中文，不要地址";
            isValueUseAble = 0;
            isValueNotEmpty = 0;
        }
        else{
            classname="help-inline";
            tips=this.props.data.elements.single.tips;
            isValueUseAble = 1;
            isValueNotEmpty = 1;
        }
        if(this.props.limit===false|| (this.props.data.mandatory=="2")){//unlimited
            classname="help-inline";
            tips=this.props.data.elements.single.tips;
        }
        this.setState({
            'classname':classname,
            'tips':tips
        });
        var stateObj = new Object();
        stateObj[this.props.suborderid]=new Object();
        stateObj[this.props.suborderid][this.props.data.elements.single.code] = new Object();
        stateObj[this.props.suborderid][this.props.data.elements.single.code].isValueUseAble = isValueUseAble;
        stateObj[this.props.suborderid][this.props.data.elements.single.code].isValueNotEmpty = isValueNotEmpty;
        stateObj[this.props.suborderid][this.props.data.elements.single.code].isValueMandatory = this.props.data.mandatory;
        this.props.reciveValueState(stateObj);
    },
    detectValueWhenMounted :function(){
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
            this.props.reciveValueState(stateObj);
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


                 var obj = {};
                 obj[this.props.suborderid] = new Object();
                 obj[this.props.suborderid][this.props.data.elements.single.code] = this.state.postValue[this.props.data.elements.single.code];
                 this.props.reciveTempValue(obj);

                //更新粘贴板内容
                var copyObj ={};
                copyObj[this.props.data.elements.single.code]={
                    title:this.props.data.elements.single.title,
                    str:this.state.postValue[this.props.data.elements.single.code]
                }
                this.updateCopyObj(copyObj);
            }
        )
    },

    getInitialState:function(){
        return{
            searchvalue: '',
            suggestions: [],
            searchResult:{
            }
        }
    },
    onChangex :function(event,newValue, method){
        var cityid =this.props.data.areaID;
        this.setState({
            searchvalue: newValue.newValue
        });
        var searchResult =  this.state.searchResult;
        if(!newValue.newValue){
            return;
        }
        if(newValue.newValue in searchResult){
            if(searchResult[newValue.newValue].loaded===true){
                // this.state.suggestions = searchResult[newValue].suggestions;
            }else{
                //loading
            }
        }else{// never search before
            var _this =this;
            searchResult[newValue.newValue] ={
                loaded:false,
            }
            axios.get('/Hotals/GetHotal',{
                    params: {
                        CityID: cityid,
                        Str: newValue.newValue,
                    }
                }).then(function (response) {
                    if(response.data.ErrorCode==200){
                        searchResult[newValue.newValue].suggestions =response.data.hotals;
                        searchResult[newValue.newValue].loaded =true;
                        if(_this.state.searchvalue==newValue.newValue){
                            _this.setState({
                                suggestions: _this.getSuggestions(newValue)
                            },function(){
                                _this.forceUpdate();
                            });
                            
                        }
                    }
                }).catch(function (error) {
                });
        }
    },
    onSuggestionsFetchRequested : function(value) {
        this.setState({
            suggestions: this.getSuggestions(value)
        });
    },
    onSuggestionsClearRequested : function(){
        this.setState({
            suggestions: []
        });
    },
    getSuggestionValue :function (suggestion){
        return suggestion.HotalName;
    },

    renderSuggestion :function(suggestion){
        return suggestion.HotalName;
    },
    getSuggestions:function(value){
        var searchvalue =this.state.searchvalue;
        var searchResult = this.state.searchResult;
        if(searchvalue in searchResult&&searchResult[searchvalue].loaded===true){
            return searchResult[searchvalue].suggestions;
        }else{
            return [];
        }

    },

    render : function () {
        var suborderid  = this.props.suborderid ;
        var componentid  = this.props.componentid ;
        var segmentcode  = this.props.data.elements.single.code ;
        var theme ={
              container:                'react-autosuggest__container',
              containerOpen:            'react-autosuggest__container--open',
              input:                    'form-control input-inline input-medium  input-sm',
              inputOpen:                'react-autosuggest__input--open',
              inputFocused:             'react-autosuggest__input--focused',
              suggestionsContainer:     'react-autosuggest__suggestions-container',
              suggestionsContainerOpen: 'react-autosuggest__suggestions-container--open',
              suggestionsList:          'react-autosuggest__suggestions-list',
              suggestion:               'react-autosuggest__suggestion',
              suggestionFirst:          'react-autosuggest__suggestion--first',
              suggestionHighlighted:    'react-autosuggest__suggestion--highlighted',
              sectionContainer:         'react-autosuggest__section-container',
              sectionContainerFirst:    'react-autosuggest__section-container--first',
              sectionTitle:             'react-autosuggest__section-title'
        }
        var inputProps = {
            placeholder: "whatever",
            value:this.state.searchvalue,
            onChange: this.onChangex
        };
        var suggestion=this.state.suggestions;

        if(this.props.UIType=="edit"){
            return(
                
                <div className="form-group"  data-suborderid ={suborderid} data-componentid={componentid} data-segmentcode ={segmentcode} >
                    <Lable params ={{isMust:this.props.data.mandatory==1?true:false,title:this.props.data.elements.single.title}} ></Lable>
                    <div className="col-md-10">
                        <Autosuggest
                            suggestions={suggestion}
                            onSuggestionsFetchRequested={this.onSuggestionsFetchRequested}
                            onSuggestionsClearRequested={this.onSuggestionsClearRequested}
                            getSuggestionValue={this.getSuggestionValue}
                            renderSuggestion={this.renderSuggestion}
                            inputProps={inputProps}
                            theme ={theme}  
                        />
                        <span className={this.state.classname}>{this.state.tips}</span>
                    </div>
                </div>
            )
        }
        else{
            var lastValue = this.prehandledata(this.props.beforeRechangeValue);
            var valueChange = 0;
            if((lastValue[this.props.data.elements.single.code] == this.state.postValue[this.props.data.elements.single.code])||this.state.postValue[this.props.data.elements.single.code]==''){
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
                        <td>
                            <LableRechange params ={{isMust:this.props.data.mandatory==1?true:false,title:this.props.data.elements.single.title}} ></LableRechange>
                        </td>
                        <td>{this.getDefaultValue()[this.props.data.elements.single.code]}</td>
                        <td>
                            <input  ref="hotelName" type="text" style={{"width":"240px","marginRight":"15px !important"}} className="form-control input-inline input-medium  input-sm"  defaultValue={valueNoW} onPaste = {this.handlerChange} onChange={this.handlerChange} />
                            <div><span className={this.state.classname}>{this.state.tips}</span></div>
                        </td>
                    </tr>
                </tbody>
            )
        }
    }
})
