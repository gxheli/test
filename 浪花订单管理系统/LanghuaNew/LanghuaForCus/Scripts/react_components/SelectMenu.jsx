var SelectMenuRow = React.createClass({
    prehandledata: function (data) {
        if (data == 'emptyrow') {
            var obj = {};
            obj[this.props.data.elements.single.code] = new Object();
            obj[this.props.data.elements.single.code]['value'] = -1;
            obj[this.props.data.elements.single.code]['text'] = '';
            return (obj);
        }
        else {
            var segmentcode = this.props.data.elements.single.code;
            var defaultValue = parseInt(data[segmentcode].value);
            var valueText = data[segmentcode].text;
            if (defaultValue in this.props.data.list) {
                if (this.props.data.list[defaultValue] != valueText) {
                    data[this.props.data.elements.single.code].value = -1;
                    data[this.props.data.elements.single.code]['text'] = '';
                    
                }
            }
            else {
                 data[this.props.data.elements.single.code].value = -1;
                 data[this.props.data.elements.single.code]['text'] = '';
            }
            return data;
        }
    },
    prehandleupdatedata: function (data) {
        return data;
    },

    detectValueState: function (value) {
        var isValueUseAble = 1;
        var isValueNotEmpty = 1;
        var classname = "";
        var tips = "";
        if (value == -1) {
            classname = "help-inline tips",
                tips = "请填写" + this.props.data.elements.single.title;
            isValueUseAble = 0;
            isValueNotEmpty = 0;
        }
        else {
            classname = "help-inline",
                tips = this.props.data.elements.single.tips
            isValueUseAble = 1;
            isValueNotEmpty = 1;
        }
        if (this.props.limit === false || (this.props.data.mandatory == "2")) {//unlimited
            classname = "help-inline";
            tips = this.props.data.elements.single.tips;
        }
        this.setState({
            'classname': classname,
            'tips': tips
        })
        var stateObj = new Object();
        stateObj[this.props.suborderid] = new Object();
        stateObj[this.props.suborderid][this.props.data.elements.single.code] = new Object();
        stateObj[this.props.suborderid][this.props.data.elements.single.code].isValueUseAble = isValueUseAble;
        stateObj[this.props.suborderid][this.props.data.elements.single.code].isValueNotEmpty = isValueNotEmpty;
        stateObj[this.props.suborderid][this.props.data.elements.single.code].isValueMandatory = this.props.data.mandatory;
        this.props.reciveValueState(stateObj)
    },
    detectValueWhenMounted: function () {
        if (this.getDefaultValue() == "emptyrow") {
            this.setState(
                {
                    classname: 'help-inline',
                    tips: this.props.data.elements.single.tips
                }
            );
            var stateObj = new Object();
            stateObj[this.props.suborderid] = new Object();
            stateObj[this.props.suborderid][this.props.data.elements.single.code] = new Object();
            stateObj[this.props.suborderid][this.props.data.elements.single.code].isValueUseAble = 0;
            stateObj[this.props.suborderid][this.props.data.elements.single.code].isValueNotEmpty = 0;
            stateObj[this.props.suborderid][this.props.data.elements.single.code].isValueMandatory = this.props.data.mandatory;
            this.props.reciveValueState(stateObj);
        }
        else {
            this.detectValueState(this.state.postValue[this.props.data.elements.single.code]['value']);
        }
    },
    componentDidMount: function (e) {
        // 可以使用this.getDOMNode()
        // 引用外部库的钩子函数
        // react 运行在服务端时候，该方法 不被调用
        var obj = {};
        obj[this.props.suborderid] = new Object();
        obj[this.props.suborderid][this.props.data.elements.single.code] = this.state.postValue[this.props.data.elements.single.code]['text'];
        this.props.reciveTempValue(obj);
        //更新粘贴板内容
        var copyObj = {};
        copyObj[this.props.data.elements.single.code] = {
            title: this.props.data.elements.single.title,
            str: this.state.postValue[this.props.data.elements.single.code]['text']
        }
        this.updateCopyObj(copyObj);


        this.detectValueWhenMounted();
    },
    mixins: [MXINS_UPDATE_DATA],
    handlerChange: function (e) {
        var obj = {};
        obj[this.props.data.elements.single.code] = {
            'value': e.target.value,
            'text': $(this.refs.menu).find('option:selected').text() != "请选择" ? $(this.refs.menu).find('option:selected').text() : ""
        }

        //回复默认值
        if (this.props.UIType == "reChange") {
            if (obj[this.props.data.elements.single.code]['value'] == -1) {
                var lastValue = this.prehandledata(this.props.beforeRechangeValue);
                obj[this.props.data.elements.single.code] = lastValue[this.props.data.elements.single.code];
            }
        }
        this.detectValueState(obj[this.props.data.elements.single.code]['value']);
        this.setState(
            {
                postValue: obj,
            },
            function () {
                this.upadteToForm(this.getdUpdateData());

                var obj = {};
                obj[this.props.suborderid] = new Object();
                obj[this.props.suborderid][this.props.data.elements.single.code] = this.state.postValue[this.props.data.elements.single.code]['text'];
                this.props.reciveTempValue(obj);
                //更新粘贴板内容
                var copyObj = {};
                copyObj[this.props.data.elements.single.code] = {
                    title: this.props.data.elements.single.title,
                    str: this.state.postValue[this.props.data.elements.single.code]['text']
                }
                this.updateCopyObj(copyObj);
            }
        )
    },
    renderOptions: function (arrData) {
        var arr = new Array(<option key={-1} value="-1">请选择</option>);
        return (
            arr.concat(
                arrData.map(function (option, index) {
                    return (
                        <option key={index} value={index}>{option}</option>
                    )
                }))
        )
    },
    render: function () {
        var suborderid = this.props.suborderid;
        var componentid = this.props.componentid;
        var segmentcode = this.props.data.elements.single.code;
        if (this.props.UIType == "edit") {
            return (
                <div className="form-group" data-suborderid={suborderid} data-componentid={componentid} data-segmentcode={segmentcode} >
                    <Lable params={{ isMust: this.props.data.mandatory == 1 ? true : false, title: this.props.data.elements.single.title }} ></Lable>
                    <div className="col-md-10">
                        <select type="text" ref="menu" className="form-control input-inline input-medium  input-sm" defaultValue={this.state.postValue[segmentcode]['value']} onPaste={this.handlerChange} onChange={this.handlerChange} >
                            {this.renderOptions(this.props.data.list)}
                        </select>
                        <span className={this.state.classname}>{this.state.tips}</span>
                    </div>
                </div>
            )
        }
        else if (this.props.UIType == "reChange") {
            var lastValue = this.prehandledata(this.props.beforeRechangeValue);
            var valueChange = -1;
            if (
                lastValue[this.props.data.elements.single.code]['value'] == this.state.postValue[this.props.data.elements.single.code]['value'] ||
                (this.state.postValue[this.props.data.elements.single.code]['value'] == -1)
            ) {
                valueChange = 0;
                var valueNoW = -1;
            }
            else {
                valueChange = 1;
                var valueNoW = this.state.postValue[this.props.data.elements.single.code]['value'];

            }
            var changeObj = new Object();
            changeObj[this.props.suborderid] = new Object();

            changeObj[this.props.suborderid][this.props.data.elements.single.code] = new Object();
            changeObj[this.props.suborderid][this.props.data.elements.single.code].isValueChange = valueChange;
            this.props.reciveValueChange(changeObj);
            return (
                <tbody>
                    <tr>
                        <td> <LableRechange params={{ isMust: this.props.data.mandatory == 1 ? true : false, title: this.props.data.elements.single.title }} ></LableRechange></td>
                        <td>{this.getDefaultValue()[this.props.data.elements.single.code]['text']}</td>
                        <td>
                            <select type="text" ref="menu" className="form-control input-inline input-medium  input-sm" defaultValue={valueNoW} onPaste={this.handlerChange} onChange={this.handlerChange} >
                                {this.renderOptions(this.props.data.list)}
                            </select>
                            <div><span className={this.state.classname}>{this.state.tips}</span></div>
                        </td>
                    </tr>
                </tbody>
            )
        }
    }
})
