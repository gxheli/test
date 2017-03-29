var TimePickerRow = React.createClass({

    prehandledata: function (data) {
        if (data == 'emptyrow') {
            var obj = {};
            obj[this.props.data.elements.single.code] = "-1" + ":" + "-1";
            return (obj);
        }
        else {
            return data;
        }
    },
    prehandleupdatedata: function (data) {
        return data;
    },


    hoursMap: function (refname, value) {
        var hours0 = [
        ];
        for (var i = 0; i < 6; i++) {
            hours0.push(i);
        };
        var hours1 = [
        ];
        for (var i = 6; i < 11; i++) {
            hours1.push(i);
        };
        var hours2 = [
        ];
        for (var i = 11; i < 13; i++) {
            hours2.push(i);
        };
        var hours3 = [
        ];
        for (var i = 13; i < 19; i++) {
            hours3.push(i);
        };
        var hours4 = [
        ];
        for (var i = 19; i < 24; i++) {
            hours4.push(i);
        };
        return (
            <select ref={refname} defaultValue={parseInt(value)} className="form-control input-inline input100 input-sm" onChange={this.handlerChange}>
                <option value={-1}>选择</option>

                {
                    hours0.map(function (hour, key) {
                        return (
                            <option key={hour} value={hour}>凌晨 {hour}点</option>
                        );
                    })
                }
                {
                    hours1.map(function (hour, key) {
                        return (
                            <option key={hour} value={hour}>上午 {hour}点</option>
                        );
                    })
                }
                {
                    hours2.map(function (hour, key) {
                        return (
                            <option key={hour} value={hour}>中午 {hour}点</option>
                        );
                    })
                }
                {
                    hours3.map(function (hour, key) {
                        return (
                            <option key={hour} value={hour}>下午 {hour}点</option>
                        );
                    })
                }
                {
                    hours4.map(function (hour, key) {
                        return (
                            <option key={hour} value={hour}>晚上 {hour}点</option>
                        );
                    })
                }
            </select>

        );
    },

    minutesMap: function (refname, value) {
        var mins = [
        ];
        for (var i = 0; i < 60; i++) {
            mins.push(i);
        };
        return (
            <select ref={refname} name="mins" defaultValue={parseInt(value)} className="form-control input-inline input80 input-sm" onChange={this.handlerChange} >
                <option value={-1}>选择</option>

                {
                    mins.map(function (unit, key) {
                        return (
                            <option key={unit} value={unit}>{unit}分</option>
                        );
                    })
                }
            </select>
        );
    },

    detectValueState: function (hour, min) {
        var timeSince = (this.props.data.timeSince && this.props.data.timeSinceCheck) ? this.props.data.timeSince : '0:0';
        var timeBefore = (this.props.data.timeBefore && this.props.data.timeBeforeCheck) ? this.props.data.timeBefore : '23:59';

        var obj = {};

        var isValueUseAble = 1;
        var isValueNotEmpty = 1;
        var classname = "";
        var tips = "";
        //选择时间不完整
        if (
            hour == '' ||
            min == ''

        ) {
            classname = "help-inline tips";
            tips = "请选择完整的" + this.props.data.elements.single.title;
            isValueUseAble = 0;
            isValueNotEmpty = 0;
        }
        else {
            var selectTime = parseInt(parseInt(hour) * 60 + parseInt(min));
            var sinceTimeArr = timeSince.toString().split(':');
            var beforeTimeArr = timeBefore.toString().split(':');
            var sinceTime = parseInt(parseInt(sinceTimeArr[0]) * 60 + parseInt(sinceTimeArr[1]));
            var beforeTime = parseInt(parseInt(beforeTimeArr[0]) * 60 + parseInt(beforeTimeArr[1]));

            if (selectTime < sinceTime || selectTime > beforeTime) {
                classname = "help-inline tips";
                tips = '所选值在允许时间之外,合理' + this.props.data.elements.single.title + '范围：从 ' + timeSince + ' 到 ' + timeBefore;
                isValueUseAble = 0;
                isValueNotEmpty = 1;
            }
            else {
                classname = "help-inline";
                tips = this.props.data.elements.single.tips;
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
            });
        };

        var stateObj = new Object();
        stateObj[this.props.suborderid] = new Object();
        stateObj[this.props.suborderid][this.props.data.elements.single.code] = new Object();
        stateObj[this.props.suborderid][this.props.data.elements.single.code].isValueUseAble = isValueUseAble;
        stateObj[this.props.suborderid][this.props.data.elements.single.code].isValueNotEmpty = isValueNotEmpty;
        stateObj[this.props.suborderid][this.props.data.elements.single.code].isValueMandatory = this.props.data.mandatory;
        this.props.reciveValueState(stateObj);
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
            var lastValue = this.state.postValue[this.props.data.elements.single.code].toString().split(':');
            this.detectValueState(lastValue[0], lastValue[1]);
        }
    },

    handlerChange: function (e) {

        var _this = this;
        var hour = (this.refs.servicehour.value) == -1 ? "" : (function () {
            if (String(_this.refs.servicehour.value).length == 1) {
                return "0" + _this.refs.servicehour.value;
            }
            return _this.refs.servicehour.value;
        })();
        var min = (this.refs.servicemin.value) == -1 ? "" : (function () {
            if (String(_this.refs.servicemin.value).length == 1) {
                return "0" + _this.refs.servicemin.value;
            }
            return _this.refs.servicemin.value;
        })();


        var obj = {};
        obj[this.props.data.elements.single.code] = hour + ':' + min;


        //回复默认值
        if (this.props.UIType == "reChange") {
            if (obj[this.props.data.elements.single.code] == ':') {
                var lastValue = this.prehandledata(this.props.beforeRechangeValue);
                obj[this.props.data.elements.single.code] = lastValue[this.props.data.elements.single.code];
            }
        }

        var timeArr = obj[this.props.data.elements.single.code].split(":");
        this.detectValueState(timeArr[0], timeArr[1]);

        //更新粘贴板内容
        var copyObj = {};
        copyObj[this.props.data.elements.single.code] = {
            title: this.props.data.elements.single.title,
            str: obj[this.props.data.elements.single.code]
        }
        this.updateCopyObj(copyObj);
        this.setState(
            {
                postValue: obj,

            },
            function () {
                this.upadteToForm(this.getdUpdateData());


                var obj = {}
                obj[this.props.suborderid] = new Object();
                obj[this.props.suborderid][this.props.data.elements.single.code] = this.state.postValue[this.props.data.elements.single.code];
                this.props.reciveTempValue(obj);

                 
            }
        )
    },


    mixins: [MXINS_UPDATE_DATA],
    getInitialState: function () {
        return {
            classname: "inline-help",
            tips: this.props.data.elements.single.tips
        }

    },
    componentDidMount: function (e) {
        // 可以使用this.getDOMNode()
        // 引用外部库的钩子函数
        // react 运行在服务端时候，该方法 不被调用
        var obj = {};
        obj[this.props.suborderid] = new Object();
        obj[this.props.suborderid][this.props.data.elements.single.code] = this.state.postValue[this.props.data.elements.single.code];
        this.props.reciveTempValue(obj);

        
        //值的合法状态
        this.detectValueWhenMounted();
        //更新粘贴板内容
        var copyObj = {};
        copyObj[this.props.data.elements.single.code] = {
            title: this.props.data.elements.single.title,
            str: this.state.postValue[this.props.data.elements.single.code]=="-1:-1"?"":this.state.postValue[this.props.data.elements.single.code]
        }
        this.updateCopyObj(copyObj);
    },
    render: function () {
        var suborderid = this.props.suborderid;
        var componentid = this.props.componentid;
        var segmentcode = this.props.data.elements.single.code;
        var lastValue = this.state.postValue[this.props.data.elements.single.code].toString().split(':');

        if (this.props.UIType == 'edit') {
            return (
                <div className="form-group" data-suborderid={suborderid} data-componentid={componentid} data-segmentcode={segmentcode} >
                    <Lable params={{ isMust: this.props.data.mandatory == 1 ? true : false, title: this.props.data.elements.single.title }} ></Lable>
                    <div className="col-md-10">
                        {this.hoursMap("servicehour", lastValue[0])}
                        {this.minutesMap("servicemin", lastValue[1])}
                        <span className={this.state.classname}>{this.state.tips}</span>
                    </div>
                </div>
            )
        }
        else if (this.props.UIType == 'reChange') {

            var lastValue = this.prehandledata(this.props.beforeRechangeValue);
            var valueChange = 0;

            if (
                lastValue[this.props.data.elements.single.code] == this.state.postValue[this.props.data.elements.single.code] ||
                (this.state.postValue[this.props.data.elements.single.code] == "-1:-1")
            ) {
                valueChange = 0;
                var valueHourNow = -1;
                var valueMinsNow = -1;
            }
            else {
                valueChange = 1;
                var valuenow = this.state.postValue[this.props.data.elements.single.code].toString().split(':');
                var valueHourNow = valuenow[0];
                var valueMinsNow = valuenow[1];

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
                            {this.hoursMap("servicehour", valueHourNow)}
                            {this.minutesMap("servicemin", valueMinsNow)}
                            <div> <span className={this.state.classname}>{this.state.tips}</span></div>
                        </td>
                    </tr>
                </tbody>
            )

        }

    }
})
