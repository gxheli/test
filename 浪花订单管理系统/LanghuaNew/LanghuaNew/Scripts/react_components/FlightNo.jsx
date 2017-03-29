var FlightNoRow = React.createClass({

    prehandledata: function (data) {
        if (data == 'emptyrow') {
            var obj = {};
            obj[this.props.data.elements.single.code] = "";
            return (obj);
        }
        else {
            return data;
        }
    },
    prehandleupdatedata: function (data) {
        return data;
    },
    mixins: [MXINS_UPDATE_DATA],
    detectValueState: function (value) {
        var isValueUseAble = 1;
        var isValueNotEmpty = 1;
        var classname = "";
        var searchClass = "";
        var tips = "";
        if (!(value)) {
            classname = "help-inline tips";
            searchClass = "hidden";
            tips = "请填写" + this.props.data.elements.single.title;
            isValueUseAble = 0;
            isValueNotEmpty = 0;
        }
        else {
            if (this.isFlightNO(value)) {
                classname = "help-inline";
                searchClass = "margin-left-10";
                tips = this.props.data.elements.single.tips;
                isValueUseAble = 1;
                isValueNotEmpty = 1;
            } else {
                classname = "help-inline tips";
                searchClass = "hidden";
                tips = "航班号是字母和数字的组合(至少3位)";
                isValueUseAble = 0;
                isValueNotEmpty = 1;
            }

        }
        if (this.props.limit === false || (this.props.data.mandatory == "2")) {//unlimited
            classname = "help-inline";
            tips = this.props.data.elements.single.tips;
        }
        this.setState({
            'classname': classname,
            'tips': tips,
            'searchClass': searchClass
        });
        var stateObj = new Object();
        stateObj[this.props.suborderid] = new Object();
        stateObj[this.props.suborderid][this.props.data.elements.single.code] = new Object();
        stateObj[this.props.suborderid][this.props.data.elements.single.code].isValueUseAble = isValueUseAble;
        stateObj[this.props.suborderid][this.props.data.elements.single.code].isValueNotEmpty = isValueNotEmpty;
        stateObj[this.props.suborderid][this.props.data.elements.single.code].isValueMandatory = this.props.data.mandatory;
        this.props.reciveValueState(stateObj)

    },
    isFlightNO: function (FlightNo) {
        FlightNo = FlightNo.trim();
        var FlightNoReg = /^[a-zA-Z0-9]{3,}$/;
        if (FlightNoReg.test(FlightNo)) {
            if (/^[a-zA-Z]{3,}$/.test(FlightNo)) {
                return false;
            } else if (/^[0-9]{3,}$/.test(FlightNo)) {
                return false;
            }
            else {
                return true;
            }
        }
        else {
            return false;
        }
    },
    getFlightInfo: function (FlightNo) {
        var _this = this;
        axios.get('/Fliter/GetFliterInfo', {
            params: {
                FlightNo: FlightNo,
                timeStamp:(new Date()).valueOf()
            }
        })
        .then(function (response) {
            var tip = "";
            if (response.data.ErrorCode === 200 && response.data.FliterInfo !== null) {
                tips = response.data.FliterInfo.FilterDeparture + '-' + response.data.FliterInfo.FilterArrival
                _this.setState({
                    'tips': tips
                });
            }
        })
        .catch(function (error) {
        });
        // $.ajax({
        //     'url': '/Fliter/GetFliterInfo',
        //     'method': "get",
        //     'data': {
        //         'FlightNo': FlightNo
        //     },
        //     'dataType': 'json',
        //     'success': function (data) {
        //         var tip = "";
        //         if (data.ErrorCode === 200 && data.FliterInfo !== null) {
        //             tips = data.FliterInfo.FilterDeparture + '-' + data.FliterInfo.FilterArrival
        //             _this.setState({
        //                 'tips': tips
        //             });
        //         }
        //     }
        // });
    },
    blurHandler: function (e) {
        var _this = this;
        var FlightNo;
        FlightNo = e.target.value;
        if (FlightNo) {
            this.flightNoHandler(FlightNo);
        }

    },
    goSearch: function (e) {
        var _this = this;
        var departdate = "";
        var departdateX = '';
        var departdateARR;
        var departdatexARR;
        var all = reactx.getALL().refs[_this.props.suborderid].refs;
        var counter = 0;
        for (var j in all) {
            if (counter >= this.props.allLength) {
                break;
            }
            if (all[j].props.type === "Flight_takeofftime_pickuptime" || all[j].props.type === "Flight_takeofftime_arrivaltime") {
                var departdateX = all[j].state.postValue[all[j].props.data.elements.takeofftime.code];
                departdateARR = departdateX.split('&');
                if (departdateARR.length > 0) {
                    departdatexARR = departdateARR[0].split("-");
                    for (var i in departdatexARR) {
                        departdate += "" + departdatexARR[i];
                    }
                }
                break;
            }
            counter++;
        }
        window.open('http://www.variflight.com/flight/fnum/' + _this.state.postValue[_this.props.data.elements.single.code] + '.html?AE71649A58c77&fdate=' + departdate, '_blank');
    },
    flightNoHandler: function (FlightNo) {
        if (this.isFlightNO(FlightNo)) {
            this.getFlightInfo(FlightNo.trim());
        }
    },
    detectValueWhenMounted: function () {
        if (this.getDefaultValue() == "emptyrow") {
            this.setState(
                {
                    classname: 'help-inline',
                    tips: this.props.data.elements.single.tips,
                    searchClass: "hidden"
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
            this.detectValueState(this.state.postValue[this.props.data.elements.single.code]);
        }
    },

    handlerChange: function (e) {
        var obj = {};
        obj[this.props.data.elements.single.code] = e.target.value;

        //回复默认值
        if (this.props.UIType == "reChange") {
            if (!(e.target.value.toString().trim())) {
                var lastValue = this.prehandledata(this.props.beforeRechangeValue);
                obj[this.props.data.elements.single.code] = lastValue[this.props.data.elements.single.code];
            }
        }
        this.detectValueState(obj[this.props.data.elements.single.code]);
        this.setState(
            {
                postValue: obj,

            },
            function () {
                this.upadteToForm(this.getdUpdateData());
                var obj = {};
                obj[this.props.suborderid] = new Object();
                obj[this.props.suborderid][this.props.data.elements.single.code] = this.state.postValue[this.props.data.elements.single.code];
                //更新模板
                this.props.reciveTempValue(obj);
                //更新粘贴板内容
                var copyObj = {};
                copyObj[this.props.data.elements.single.code] = {
                    title: this.props.data.elements.single.title,
                    str: this.state.postValue[this.props.data.elements.single.code]
                }
                this.updateCopyObj(copyObj);
            }
        )
    },
    componentDidMount: function (e) {
        // 可以使用this.getDOMNode()
        // 引用外部库的钩子函数
        // react 运行在服务端时候，该方法 不被调用
        var obj = {};
        obj[this.props.suborderid] = new Object();
        obj[this.props.suborderid][this.props.data.elements.single.code] = this.state.postValue[this.props.data.elements.single.code];
        //更新模板
        this.props.reciveTempValue(obj);
        //检测填写值状态
        this.detectValueWhenMounted();
        //更新粘贴板内容
        var copyObj = {};
        copyObj[this.props.data.elements.single.code] = {
            title: this.props.data.elements.single.title,
            str: this.state.postValue[this.props.data.elements.single.code]
        }
        this.updateCopyObj(copyObj);
        var FlightNoStr = this.state.postValue[this.props.data.elements.single.code];
        if (FlightNoStr) {
            this.flightNoHandler(FlightNoStr);
        }
    },
    render: function () {
        var suborderid = this.props.suborderid;
        var componentid = this.props.componentid;
        var segmentcode = this.props.data.elements.single.code;
        var search = null;
        if (this.props.isForCusClient === false) {
            search = <a className={this.state.searchClass} onClick={this.goSearch} >查询航班</a>;
        }
        if (this.props.UIType == "edit") {
            return (
                <div className="form-group" data-suborderid={suborderid} data-componentid={componentid} data-segmentcode={segmentcode} >
                    <Lable params={{ isMust: this.props.data.mandatory == 1 ? true : false, title: this.props.data.elements.single.title }} ></Lable>
                    <div className="col-md-10">
                        <input type="text" className="form-control input-inline input-medium  input-sm" defaultValue={this.state.postValue[segmentcode]} onBlur={this.blurHandler} onPaste={this.handlerChange} onChange={this.handlerChange} />
                        <span className={this.state.classname}>{this.state.tips}{search}</span>
                    </div>
                </div>
            )
        }
        else if (this.props.UIType == "reChange") {
            var lastValue = this.prehandledata(this.props.beforeRechangeValue);
            var valueChange = 0;

            if (
                lastValue[this.props.data.elements.single.code] == this.state.postValue[this.props.data.elements.single.code] ||
                (this.state.postValue[this.props.data.elements.single.code] == "")
            ) {
                valueChange = 0;
                var valueNoW = '';
            }
            else {
                valueChange = 1;
                var valueNoW = this.state.postValue[this.props.data.elements.single.code];
            }
            var changeObj = new Object();
            changeObj[this.props.suborderid] = new Object();

            changeObj[this.props.suborderid][this.props.data.elements.single.code] = new Object();
            changeObj[this.props.suborderid][this.props.data.elements.single.code].isValueChange = valueChange;
            this.props.reciveValueChange(changeObj);
            return (
                <tbody>
                    <tr>
                        <td>
                            <LableRechange params={{ isMust: this.props.data.mandatory == 1 ? true : false, title: this.props.data.elements.single.title }} ></LableRechange>
                        </td>
                        <td>{this.getDefaultValue()[this.props.data.elements.single.code]}</td>
                        <td>
                            <input type="text" className="form-control input-inline input-medium  input-sm" defaultValue={valueNoW} onPaste={this.handlerChange} onChange={this.handlerChange} />
                            <div><span className={this.state.classname}>{this.state.tips}</span></div>
                        </td>
                    </tr>
                </tbody>
            )
        }
    }
})

