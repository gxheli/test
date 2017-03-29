var DatePickerRow = React.createClass({

    prehandledata: function (data) {
        if (data == 'emptyrow') {
            var obj = {};
            obj[this.props.data.elements.single.code] = {
                value: ""
            };
            return (obj);
        }
        else {
            return data;
        }
    },
    prehandleupdatedata: function (data) {
        return data;
    },
    isDate: function (date, fmt) {
        if (fmt == null) fmt = "yyyyMMdd";
        var yIndex = fmt.indexOf("yyyy");
        if (yIndex == -1) return false;
        var year = date.substring(yIndex, yIndex + 4);
        var mIndex = fmt.indexOf("MM");
        if (mIndex == -1) return false;
        var month = date.substring(mIndex, mIndex + 2);
        var dIndex = fmt.indexOf("dd");
        if (dIndex == -1) return false;
        var day = date.substring(dIndex, dIndex + 2);
        if (!isNumber(year) || year > "2100" || year < "1900") return false;
        if (!isNumber(month) || month > "12" || month < "01") return false;
        if (day > getMaxDay(year, month) || day < "01") {
            return false;
        }
        return true;

        function getMaxDay(year, month) {
            if (month == 4 || month == 6 || month == 9 || month == 11)
                return "30";
            if (month == 2)
                if (year % 4 == 0 && year % 100 != 0 || year % 400 == 0)
                    return "29";
                else
                    return "28";
            return "31";
        };
        function isNumber(s) {
            var regu = "^[0-9]+$";
            var re = new RegExp(regu);
            if (s.search(re) != -1) {
                return true;
            } else {
                return false;
            }
        }
    },
    detectValueState: function (date1) {
        var isValueUseAble = 1;
        var isValueNotEmpty = 1;
        var isValueNotEmpty = 1;
        var classname = "";
        if (!(date1)) {
            classname = "help-inline tips";
            tips = "请选择或填写正确的" + this.props.data.elements.single.title;
            isValueUseAble = 0;
            isValueNotEmpty = 0;
        }
        else if (!(this.isDate(date1, 'yyyy-MM-dd'))) {
            classname = "help-inline tips";
            tips = "请选择或填写正确的" + this.props.data.elements.single.title;
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
                    setoutclassname: "help-inline",
                    setouttips: this.props.data.elements.single.tips
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
        this.detectValueWhenMounted();
        var obj = {};
        obj[this.props.suborderid] = new Object();
        obj[this.props.suborderid][this.props.data.elements.single.code] = this.state.postValue[this.props.data.elements.single.code]['value'];
        this.props.reciveTempValue(obj);

        //更新粘贴板内容
        var copyObj ={};
        copyObj[this.props.data.elements.single.code]={
            title:this.props.data.elements.single.title,
            str:this.state.postValue[this.props.data.elements.single.code]['value']
        }
        this.updateCopyObj(copyObj);

        jQuery(this.refs.datepicker).datepicker({
            autoclose: true,
            orientation: "top right",
            language: 'ZH-CN',
            format: 'yyyy-mm-dd',
            // startDate:'1921-01-01',
            // endDate:"0d",
            todayHighlight: true,
            todayBtn: 'linked',
            clearBtn: true,
        }).on('changeDate', function (ev) {
            var d = ev.date ? ev.date.valueOf() : '';
            if (d != '') {
                var newDate = new Date(d);
                var year = newDate.getFullYear();
                var month = String((parseInt(newDate.getMonth()) + 1)).length == 1 ? "0" + String((parseInt(newDate.getMonth()) + 1)) : String((parseInt(newDate.getMonth()) + 1));
                var date = String(parseInt(newDate.getDate())).length == 1 ? "0" + String((parseInt(newDate.getDate()))) : String((parseInt(newDate.getDate())));
                var ds = year + '-' + month + '-' + date;
            }
            else {
                var ds = ''
            }
            var obj = {}
            obj[this.props.data.elements.single.code] = {
                value: ds
            };

            //回复默认值
            if (this.props.UIType == "reChange") {
                if (ds == "") {
                    var lastValue = this.prehandledata(this.props.beforeRechangeValue);
                    obj[this.props.data.elements.single.code] = lastValue[this.props.data.elements.single.code];
                }
            }
            this.detectValueState(obj[this.props.data.elements.single.code].value);
            this.setState(
                {
                    postValue: obj
                },
                function () {
                    this.upadteToForm(this.getdUpdateData());

                    var obj = {};
                    obj[this.props.suborderid] = new Object();
                    obj[this.props.suborderid][this.props.data.elements.single.code] = this.state.postValue[this.props.data.elements.single.code]['value'];
                    this.props.reciveTempValue(obj);

                    //更新粘贴板内容
                    var copyObj ={};
                    copyObj[this.props.data.elements.single.code]={
                        title:this.props.data.elements.single.title,
                        str:this.state.postValue[this.props.data.elements.single.code]['value']
                    }
                    this.updateCopyObj(copyObj);
                }
            );
            // this.upadteToForm(this.getdUpdateData());
        }.bind(this))
            .on("hide", function () {
                $(this.refs.datepicker).find('input').blur();
            }.bind(this));

    },
    mixins: [MXINS_UPDATE_DATA],
    handlerChange: function (e) {
        // this.setState({
        //     postValue :e.target.value
        // })
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

                        <div ref="datepicker" className="input-group input-medium  input-group-sm ddDatePicker date date-picker  " >
                            <input ref="datepick" readOnly type="text" className="form-control input-medium input  readOnlyDat readOnlyDatePicker" defaultValue={this.state.postValue[segmentcode]['value']} onPaste={this.handlerChange} />
                            <span className="input-group-btn">
                                <button className="btn default" type="button">
                                    <i className="fa fa-calendar"></i>
                                </button>
                            </span>
                        </div>
                        <span className={this.state.setoutclassname}>{this.state.setouttips}</span>
                    </div>
                </div>
            )
        }
        else if (this.props.UIType == "reChange") {


            var lastValue = this.prehandledata(this.props.beforeRechangeValue);
            var valueChange = 0;
            if (
                lastValue[this.props.data.elements.single.code]['value'] == this.state.postValue[this.props.data.elements.single.code]['value'] ||
                (this.state.postValue[this.props.data.elements.single.code] == "")
            ) {
                valueChange = 0;
                var valueNoW = '';
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
                        <td>
                             <LableRechange params={{ isMust: this.props.data.mandatory == 1 ? true : false, title: this.props.data.elements.single.title }} ></LableRechange>
                        </td>
                        <td>{this.getDefaultValue()[this.props.data.elements.single.code]['value']}</td>
                        <td>
                            <div ref="datepicker" className="input-group input-medium  input-group-sm ddDatePicker date date-picker  " >
                                <input ref="datepick" readOnly type="text" className="form-control input-medium input readOnlyDatePicker" defaultValue={valueNoW} onPaste={this.handlerChange} />
                                <span className="input-group-btn">
                                    <button className="btn default" type="button">
                                        <i className="fa fa-calendar"></i>
                                    </button>
                                </span>
                            </div>
                            <div><span className={this.state.setoutclassname}>{this.state.setouttips}</span></div>
                        </td>
                    </tr>
                </tbody>
            )
        }
    }
})
