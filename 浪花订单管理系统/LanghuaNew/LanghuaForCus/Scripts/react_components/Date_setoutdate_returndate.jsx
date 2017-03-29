// 出行返回日期控件
var Date_setoutdate_returndateRow = React.createClass({
    prehandledata: function (data) {
        if (data == "emptyrow") {
            var elements = this.props.data.elements;
            var obj = new Object();
            obj[elements.setoutdate.code] = "";
            obj[elements.returndate.code] = "";

            return (obj);
        }
        return data;
    },
    prehandleupdatedata: function (data) {
        return data;
    },
    mixins: [MXINS_UPDATE_DATA],
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
        if (day > getMaxDay(year, month) || day < "01") return false;
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

    detectValueState: function (date1, date2) {
        var isSetoutValueUseAble = 1;
        var isSetoutNotEmpty = 1;
        var isReturnValueUseAble = 1;
        var isReturnNotEmpty = 1;
        var setoutclassname = "";
        var setouttips = "";
        var returnclassname = "";
        var returntips = "";

        var night = parseInt(this.state.Nights);
        var ServiceTypeID = this.props.ServiceTypeID;
        if (
            (date1) && (this.isDate(date1, 'yyyy-MM-dd')) &&
            (date1) && (this.isDate(date2, 'yyyy-MM-dd'))
        ) {

            var dateOut = new Date(date1).valueOf();
            var dateReturn = new Date(date2).valueOf();
            var off = night * 24 * 60 * 60 * 1000;

            if ((dateReturn < dateOut)) {
                isSetoutValueUseAble = 0;
                isReturnValueUseAble = 0;
                isSetoutNotEmpty = 1;
                isReturnNotEmpty = 1;
                returntips = this.props.data.elements.setoutdate.title + "小于" + this.props.data.elements.returndate.title;
                setouttips = this.props.data.elements.setoutdate.title + "大于" + this.props.data.elements.returndate.title;
                returnclassname = "help-inline tips";
                setoutclassname = "help-inline tips";
                //变更变更的更正为单个提示语
                if (this.props.UIType == "reChange") {
                    var lastValue = this.prehandledata(this.props.beforeRechangeValue);
                    if (date1 == lastValue[this.props.data.elements.setoutdate.code]) {
                        setouttips = this.props.data.elements.setoutdate.tips;
                        setoutclassname = "help-inline";
                    }
                    if (date2 == lastValue[this.props.data.elements.returndate.code]) {
                        returntips = this.props.data.elements.returndate.tips;
                        returnclassname = "help-inline";
                    }
                }
            }
            else {
                if ((dateOut != dateReturn - off) && (off != 0) && ServiceTypeID == 4) {
                    isSetoutValueUseAble = 0;
                    isReturnValueUseAble = 0;
                    isSetoutNotEmpty = 1;
                    isReturnNotEmpty = 1;
                    returntips = this.props.data.elements.setoutdate.title + "和" + this.props.data.elements.returndate.title + '应当相差' + night + "晚";
                    setouttips = this.props.data.elements.setoutdate.title + "和" + this.props.data.elements.returndate.title + '应当相差' + night + "晚";
                    returnclassname = "help-inline tips";
                    setoutclassname = "help-inline tips";
                    //变更变更的更正为单个提示语
                    // if(this.props.UIType=="reChange"){
                    //     var lastValue = this.prehandledata(this.props.beforeRechangeValue);

                    //     if((date1 == lastValue[this.props.data.elements.setoutdate.code])){
                    //          this.setState({
                    //             setouttips:this.props.data.elements.setoutdate.tips,
                    //             setoutclassname:"help-inline",
                    //         });
                    //     }
                    //     if(date2 == lastValue[this.props.data.elements.returndate.code]){
                    //         this.setState({
                    //             returntips:this.props.data.elements.returndate.tips,
                    //             returnclassname:"help-inline",
                    //         });
                    //     }
                    // }
                }
                else {
                    isSetoutValueUseAble = 1;
                    isReturnValueUseAble = 1;
                    isSetoutNotEmpty = 1;
                    isReturnNotEmpty = 1;
                    returntips = this.props.data.elements.returndate.tips;
                    setouttips = this.props.data.elements.setoutdate.tips;
                    returnclassname = "help-inline";
                    setoutclassname = "help-inline";
                }
            }
        }
        else {
            //出发日期
            if (!(date1)) {
                setoutclassname = "help-inline tips";
                setouttips = "请选择或填写正确的日期";
                isSetoutValueUseAble = 0;
                isSetoutNotEmpty = 0;
            }
            else if (!(this.isDate(date1, 'yyyy-MM-dd'))) {
                setoutclassname = "help-inline tips";
                setouttips = "请选择或填写正确的日期";
                isSetoutValueUseAble = 0;
                isSetoutNotEmpty = 1;
            }
            else {
                setoutclassname = "help-inline";
                setouttips = this.props.data.elements.setoutdate.tips;
                isSetoutValueUseAble = 1;
                isSetoutNotEmpty = 1;
            }


            // 返回日期
            if (!(date2)) {
                returnclassname = "help-inline tips";
                returntips = "请选择或填写正确的日期";
                isReturnValueUseAble = 0;
                isReturnNotEmpty = 0;
            }
            else if (!(this.isDate(date2, 'yyyy-MM-dd'))) {
                returnclassname = "help-inline tips";
                returntips = "请选择或填写正确的日期";
                isReturnValueUseAble = 0;
                isReturnNotEmpty = 1;
            }
            else {
                returnclassname = "help-inline";
                returntips = this.props.data.elements.setoutdate.tips;
                isReturnValueUseAble = 1;
                isReturnNotEmpty = 1;
            }
        }
        if (this.props.limit === false || (this.props.data.mandatory == "2")) {//unlimited
            setoutclassname = "help-inline";
            setouttips = this.props.data.elements.setoutdate.tips;
            returnclassname = "help-inline";
            returntips = this.props.data.elements.returndate.tips;
        }
        this.setState({
            setoutclassname: setoutclassname,
            setouttips: setouttips,
            returnclassname: returnclassname,
            returntips: returntips
        });


        var stateObj = new Object();
        stateObj[this.props.suborderid] = new Object();

        stateObj[this.props.suborderid][this.props.data.elements.setoutdate.code] = new Object();
        stateObj[this.props.suborderid][this.props.data.elements.setoutdate.code].isValueUseAble = isSetoutValueUseAble;
        stateObj[this.props.suborderid][this.props.data.elements.setoutdate.code].isValueNotEmpty = isSetoutNotEmpty;
        stateObj[this.props.suborderid][this.props.data.elements.setoutdate.code].isValueMandatory = this.props.data.mandatory;

        stateObj[this.props.suborderid][this.props.data.elements.returndate.code] = new Object();
        stateObj[this.props.suborderid][this.props.data.elements.returndate.code].isValueUseAble = isReturnValueUseAble;
        stateObj[this.props.suborderid][this.props.data.elements.returndate.code].isValueNotEmpty = isReturnNotEmpty;
        stateObj[this.props.suborderid][this.props.data.elements.returndate.code].isValueMandatory = this.props.data.mandatory;

        if ((!this.props.data.elements.returndate.visible) || (this.props.ServiceTypeID == 1)) {//提醒的处理
            stateObj[this.props.suborderid][this.props.data.elements.returndate.code].tipsvisible = false;
        }
        this.props.reciveValueState(stateObj)
    },
    detectValueWhenMounted: function () {
        if (this.getDefaultValue() == "emptyrow") {
            this.setState(
                {
                    returnclassname: "help-inline",
                    returntips: this.props.data.elements.setoutdate.tips,
                    setoutclassname: "help-inline",
                    setouttips: this.props.data.elements.setoutdate.tips
                }
            );
            var stateObj = new Object();
            stateObj[this.props.suborderid] = new Object();

            stateObj[this.props.suborderid][this.props.data.elements.setoutdate.code] = new Object();
            stateObj[this.props.suborderid][this.props.data.elements.setoutdate.code].isValueUseAble = 0;
            stateObj[this.props.suborderid][this.props.data.elements.setoutdate.code].isValueNotEmpty = 0;
            stateObj[this.props.suborderid][this.props.data.elements.setoutdate.code].isValueMandatory = this.props.data.mandatory;

            stateObj[this.props.suborderid][this.props.data.elements.returndate.code] = new Object();
            stateObj[this.props.suborderid][this.props.data.elements.returndate.code].isValueUseAble = 0;
            stateObj[this.props.suborderid][this.props.data.elements.returndate.code].isValueNotEmpty = 0;
            stateObj[this.props.suborderid][this.props.data.elements.returndate.code].isValueMandatory = this.props.data.mandatory;
            if ((!this.props.data.elements.returndate.visible) || (this.props.ServiceTypeID == 1)) {//提醒的处理
                stateObj[this.props.suborderid][this.props.data.elements.returndate.code].tipsvisible = false;
            }

            this.props.reciveValueState(stateObj)

        }
        else {
            this.detectValueState(this.state.postValue[this.props.data.elements.setoutdate.code], this.state.postValue[this.props.data.elements.returndate.code]);
        }
    },
    getInitialState: function () {
        return {
            Nights: this.props.Nights
        }
    },
    componentDidMount: function (e) {
        var _this = this;
        {
            $(this.refs.forceUpdate).bind("update", function (e, Nights) {
                _this.setState({
                    Nights: Nights
                }, function () {
                    this.detectValueState(this.state.postValue[this.props.data.elements.setoutdate.code], this.state.postValue[this.props.data.elements.returndate.code]);
                })
            })
        }
        var elements = this.props.data.elements;
        var orivalue = this.state.postValue;
        var objdate = new Object();
        objdate[this.props.suborderid] = new Object();
        for (var s in elements) {
            for (var ss in elements[s]['systemFieldMap']) {
                objdate[this.props.suborderid][elements[s]['systemFieldMap'][ss]['postKey']] = orivalue[elements[s]['code']];
            }
        }
        this.props.recieveSystemFields(objdate);

        // 可以使用this.getDOMNode()
        // 引用外部库的钩子函数
        // react 运行在服务端时候，该方法 不被调用

        this.detectValueWhenMounted();


        var obj = {};
        obj[this.props.suborderid] = new Object();
        obj[this.props.suborderid][this.props.data.elements.setoutdate.code] = this.state.postValue[this.props.data.elements.setoutdate.code];
        obj[this.props.suborderid][this.props.data.elements.returndate.code] = this.state.postValue[this.props.data.elements.returndate.code];
        this.props.reciveTempValue(obj);

        var thisDatePicker = this;
        var ServiceTypeID = thisDatePicker.props.ServiceTypeID;
        var fixedDays = thisDatePicker.props.fixedDays;
        var returndate = thisDatePicker.props.data.elements.returndate;

        var showRetrunDay = true;
        var retrunDate = this.props.data.elements.returndate;
        if (ServiceTypeID == 2) {//对形成类单独处理
            if (fixedDays != 0) {
                showRetrunDay = false;
            }
        }
        else {
            if ((!returndate.visible) || this.props.ServiceTypeID == 1) {
                showRetrunDay = false;
            }
        }
        //更新粘贴板内容
        var copyObj ={};
        copyObj[this.props.data.elements.setoutdate.code]={
            title:this.props.data.elements.setoutdate.title,
            str:this.state.postValue[this.props.data.elements.setoutdate.code]
        }
        if(showRetrunDay){
            copyObj[this.props.data.elements.returndate.code]={
                title:this.props.data.elements.returndate.title,
                str:this.state.postValue[this.props.data.elements.returndate.code]
            }
        }
        this.updateCopyObj(copyObj);

        if (this.props.limit === false) {
            initTravelDateWithoutRule.call(_this);
        }
        else {
            $.ajax({
                url: "/ServiceRules/GetRulesByItemID/" + this.props.serviceItemID,
                type: 'get',
                dataType: "json",
                success: function (ruleArr) {
                    initTravelDateWithRule.call(_this, ruleArr);
                }
            });
        }
        initOthers.call(_this,showRetrunDay);
        function initTravelDateWithRule(ruleArr) {
            jQuery(this.refs.setoutdatepicker).datepicker({
                orientation: "top right",
                startDate: (function () {
                    if (thisDatePicker.props.isForCusClient) {
                        var serverTime = parseInt(document.getElementById("severTimeStamp").innerText);
                        var serverDateObj = new Date(serverTime);
                        if(serverTime!==undefined||serverTime!==null){
                            var clientDateObj = new Date();
                            var clientTime =  clientDateObj.valueOf();
                            if(Math.abs(serverTime-clientTime)>60*60*1000){
                                return serverDateObj;
                            }else{
                                 return '+0d';
                            }
                        }else{
                            return '+0d';
                        }
                    }
                    else{
                        return -Infinity;
                    }
                })(),
                //处理禁止日期
                beforeShowDay: function (date) {
                    var DateState = new Object();
                    DateState.enabled = true;
                    DateState.classes = '';
                    DateState.tooltip = '';

                    for (var j in ruleArr) {
                        var timeStart = ruleArr[j].StartTime.split('T')[0].split("-");
                        var timeEnd = ruleArr[j].EndTime.split('T')[0].split("-");
                        var timestampStart = (new Date(timeStart[0], timeStart[1] - 1, timeStart[2], 0, 0, 0)).valueOf();
                        var timestampEnd = (new Date(timeEnd[0], timeEnd[1] - 1, timeEnd[2], 0, 0, 0)).valueOf();
                        var timestampThisDate = date.valueOf();
                        var allow = ruleArr[j]['RuleUseTypeValue'] == 0 ? true : false;

                        if (timestampThisDate > timestampEnd || timestampThisDate < timestampStart) {
                            continue;
                        }
                        switch (ruleArr[j]['SelectRuleType']) {
                            case 0://案范围
                                {
                                    var timeRangeStart = ruleArr[j].RangeStart.split('T')[0].split("-");
                                    var timeRangeEnd = ruleArr[j].RangeEnd.split('T')[0].split("-");
                                    var timestampRangeStart = (new Date(timeRangeStart[0], timeRangeStart[1] - 1, timeRangeStart[2], 0, 0, 0)).valueOf();
                                    var timestampRangeEnd = (new Date(timeRangeEnd[0], timeRangeEnd[1] - 1, timeRangeEnd[2], 0, 0, 0)).valueOf();
                                    var inRange = false;
                                    if (timestampThisDate <= timestampRangeEnd && timestampThisDate >= timestampRangeStart) {
                                        inRange = true;
                                    }
                                    var enAbledthisrule = (allow == inRange ? true : false);
                                    if (!enAbledthisrule) {
                                        DateState.enabled = false;
                                    }

                                    break;
                                }
                            case 1:
                                {
                                    var weekARR = ruleArr[j]['Week'].split("|");
                                    var inWeek = false;
                                    var thisDay = date.getDay();
                                    for (var d in weekARR) {
                                        if (weekARR[d] == thisDay) {
                                            inWeek = true;
                                            break;
                                        }
                                    }
                                    var enAbledthisrule = (allow == inWeek ? true : false);
                                    if (!enAbledthisrule) {
                                        DateState.enabled = false;
                                    }
                                    break;
                                }
                            case 2:
                                {
                                    allow = (allow == ruleArr[j]['IsDouble'] ? true : false);
                                    var isdouble = false;
                                    if (date.getDate() % 2 == 0) {
                                        isdouble = true;
                                    }
                                    var enAbledthisrule = (isdouble == allow ? true : false);
                                    if (!enAbledthisrule) {
                                        DateState.enabled = false;
                                    }
                                    break;
                                }
                            case 3:
                                {
                                    var dateArr = ruleArr[j]['UseDate'].split("|");
                                    var thisdate = date.getDate();
                                    var inDates = false;
                                    for (var i in dateArr) {
                                        if (dateArr[i] == thisdate) {
                                            inDates = true;
                                            break;
                                        }
                                    }
                                    var enAbledthisrule = (allow == inDates ? true : false);
                                    if (!enAbledthisrule) {
                                        DateState.enabled = false;
                                    }
                                    break;
                                }
                            default:
                                break;
                        }
                    }
                    // if (thisDatePicker.props.isForCusClient) {

                    //     var utcdate = new Date(date.getFullYear(), date.getMonth(), date.getDate());
                    //     var timestampThisDatee = utcdate.valueOf();

                    //     var today = new Date();
                    //     var utctoday = new Date(today.getFullYear(), today.getMonth(), today.getDate());
                    //     var timestampToday = utctoday.valueOf();
                    //     if (timestampThisDatee < timestampToday) {
                    //         DateState.enabled = false;
                    //     }
                    // }
                    if (!(DateState.enabled)) {
                        DateState.classes = 'delete';
                        DateState.tooltip = '';
                    }
                    console.log()
                    return DateState;
                }
            })
        }
        function initTravelDateWithoutRule() {
            jQuery(this.refs.setoutdatepicker).datepicker({
                orientation: "top right",
                startDate: (function () {
                    if (thisDatePicker.props.isForCusClient) {
                        return '+0d';
                    }
                    return -Infinity;
                })()
            });
        }
        function 　initOthers() {
            // var ServiceTypeID = thisDatePicker.props.ServiceTypeID;
            // var fixedDays = thisDatePicker.props.fixedDays;
            // var returndate = thisDatePicker.props.data.elements.returndate;

            // var showRetrunDay = true;
            // var retrunDate = this.props.data.elements.returndate;
            // if (ServiceTypeID == 2) {//对形成类单独处理
            //     if (fixedDays != 0) {
            //         showRetrunDay = false;
            //     }
            // }
            // else {
            //     if ((!returndate.visible) || this.props.ServiceTypeID == 1) {
            //         showRetrunDay = false;
            //     }
            // }
            jQuery(this.refs.setoutdatepicker).on('changeDate', function (ev) {

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


                var lastvalue = this.state.postValue;
                obj[this.props.data.elements.setoutdate.code] = ds;
                // obj= $.extend(true,lastvalue,obj);
                obj[this.props.data.elements.returndate.code] = lastvalue[this.props.data.elements.returndate.code];
                //对于返回日期不可见的情况
                if (!showRetrunDay) {
                    var dateReturn = ds;
                    if (ds != '') {

                        var fixedDays = this.props.fixedDays == 0 ? 0 : parseInt(this.props.fixedDays) - 1;

                        var timestampAdd = fixedDays * 24 * 60 * 60 * 1000;
                        var timestampStart = ev.date.valueOf();
                        var timestampReturn = timestampStart + timestampAdd;


                        var retrunDate = new Date(timestampReturn);
                        var yearr = retrunDate.getFullYear();
                        var monthr = String((parseInt(retrunDate.getMonth()) + 1)).length == 1 ? "0" + String((parseInt(retrunDate.getMonth()) + 1)) : String((parseInt(retrunDate.getMonth()) + 1));
                        var dater = String(parseInt(retrunDate.getDate())).length == 1 ? "0" + String((parseInt(retrunDate.getDate()))) : String((parseInt(retrunDate.getDate())));
                        dateReturn = yearr + '-' + monthr + '-' + dater;


                    }
                    obj[this.props.data.elements.returndate.code] = dateReturn;
                }




                //回复默认值
                if (this.props.UIType == "reChange") {
                    if (obj[this.props.data.elements.setoutdate.code] == '') {
                        var lastValue = this.prehandledata(this.props.beforeRechangeValue);
                        obj[this.props.data.elements.setoutdate.code] = lastValue[this.props.data.elements.setoutdate.code];
                    }
                    if (obj[this.props.data.elements.returndate.code] == '') {
                        var lastValue = this.prehandledata(this.props.beforeRechangeValue);
                        obj[this.props.data.elements.returndate.code] = lastValue[this.props.data.elements.returndate.code];
                    }
                }
                //检测值状况
                this.detectValueState(obj[this.props.data.elements.setoutdate.code], obj[this.props.data.elements.returndate.code]);


                this.setState(
                    {
                        postValue: obj

                    },

                    function () {
                        this.upadteToForm(this.getdUpdateData());

                        //更新系统字段
                        var elements = this.props.data.elements;
                        var orivalue = this.state.postValue;
                        var objdate = new Object();
                        objdate[this.props.suborderid] = new Object();
                        for (var s in elements) {
                            for (var ss in elements[s]['systemFieldMap']) {
                                objdate[this.props.suborderid][elements[s]['systemFieldMap'][ss]['postKey']] = orivalue[elements[s]['code']];
                            }
                        }
                        this.props.recieveSystemFields(objdate);



                        var obj = {};
                        obj[this.props.suborderid] = new Object();
                        obj[this.props.suborderid][this.props.data.elements.setoutdate.code] = this.state.postValue[this.props.data.elements.setoutdate.code];
                        obj[this.props.suborderid][this.props.data.elements.returndate.code] = this.state.postValue[this.props.data.elements.returndate.code];
                        this.props.reciveTempValue(obj);

                        //更新粘贴板内容
                        var copyObj ={};
                        copyObj[this.props.data.elements.setoutdate.code]={
                            title:this.props.data.elements.setoutdate.title,
                            str:this.state.postValue[this.props.data.elements.setoutdate.code]
                        }
                        if(showRetrunDay){
                            copyObj[this.props.data.elements.returndate.code]={
                                title:this.props.data.elements.returndate.title,
                                str:this.state.postValue[this.props.data.elements.returndate.code]
                            }
                        }
                        this.updateCopyObj(copyObj);


                    }
                );
                // this.upadteToForm(this.getdUpdateData());
            }.bind(this))
                .on("hide", function () {
                    $(this.refs.setoutdatepicker).find('input').blur();
                }.bind(this));

            if (showRetrunDay) {
                jQuery(this.refs.returndatepicker).datepicker({
                    orientation: "top right",
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
                    var obj = {};
                    var lastvalue = this.state.postValue;
                    obj[this.props.data.elements.returndate.code] = ds;

                    // obj= $.extend(true,lastvalue,obj);
                    obj[this.props.data.elements.setoutdate.code] = lastvalue[this.props.data.elements.setoutdate.code];






                    //回复默认值
                    if (this.props.UIType == "reChange") {
                        if (obj[this.props.data.elements.setoutdate.code] == '') {
                            var lastValues = this.prehandledata(this.props.beforeRechangeValue);
                            obj[this.props.data.elements.setoutdate.code] = lastValues[this.props.data.elements.setoutdate.code];
                        }
                        if (obj[this.props.data.elements.returndate.code] == '') {
                            var lastValues = this.prehandledata(this.props.beforeRechangeValue);
                            obj[this.props.data.elements.returndate.code] = lastValues[this.props.data.elements.returndate.code];
                        }
                    }

                    //检测值
                    this.detectValueState(obj[this.props.data.elements.setoutdate.code], obj[this.props.data.elements.returndate.code]);

                    this.setState(
                        {
                            postValue: obj

                        },
                        function () {
                            this.upadteToForm(this.getdUpdateData());

                            var elements = this.props.data.elements;
                            var orivalue = this.state.postValue;
                            var objdate = new Object();
                            objdate[this.props.suborderid] = new Object();
                            for (var s in elements) {
                                for (var ss in elements[s]['systemFieldMap']) {
                                    objdate[this.props.suborderid][elements[s]['systemFieldMap'][ss]['postKey']] = orivalue[elements[s]['code']];
                                }
                            }
                            this.props.recieveSystemFields(objdate);


                            var obj = {};
                            obj[this.props.suborderid] = new Object();
                            obj[this.props.suborderid][this.props.data.elements.setoutdate.code] = this.state.postValue[this.props.data.elements.setoutdate.code];
                            obj[this.props.suborderid][this.props.data.elements.returndate.code] = this.state.postValue[this.props.data.elements.returndate.code];
                            this.props.reciveTempValue(obj);

                            //更新粘贴板内容
                            var copyObj ={};
                            copyObj[this.props.data.elements.setoutdate.code]={
                                title:this.props.data.elements.setoutdate.title,
                                str:this.state.postValue[this.props.data.elements.setoutdate.code]
                            }
                            copyObj[this.props.data.elements.returndate.code]={
                                title:this.props.data.elements.returndate.title,
                                str:this.state.postValue[this.props.data.elements.returndate.code]
                            }
                            this.updateCopyObj(copyObj);


                        }
                    );
                    // this.upadteToForm(this.getdUpdateData());
                }.bind(this))
                    .on("hide", function () {
                        $(this.refs.returndatepicker).find('input').blur();
                    }.bind(this));;
            }

        }
    },
    render: function () {
        var data = this.props.data;
        var suborderid = this.props.suborderid;
        var componentid = this.props.componentid;
        var ServiceTypeID = this.props.ServiceTypeID;
        var fixedDays = this.props.fixedDays;
        var setoutdate = this.props.data.elements.setoutdate;
        var returndate = this.props.data.elements.returndate;
        var retrunVisiblity = "form-group";
        var returnVisibiityRe = '';
        if (ServiceTypeID == 2) {//对行程类单独处理
            if (fixedDays != 0) {
                retrunVisiblity = "form-group  hidden";
                returnVisibiityRe = 'hidden';
            }
        }
        else {
            if (!returndate.visible) {
                retrunVisiblity = "form-group  hidden";
                returnVisibiityRe = 'hidden';
            }
            else {
                if (this.props.ServiceTypeID == 1) {
                    retrunVisiblity = "form-group  hidden";
                    returnVisibiityRe = 'hidden';
                }
            }

        }

        if (this.props.UIType == "edit") {
            return (
                <div ref="forceUpdate" className="forceUpdate-setoutreturn">
                    <div className="form-group" data-suborderid={suborderid} data-componentid={componentid} data-segmentcode={setoutdate['code']} >
                        <Lable params={{ isMust: this.props.data.mandatory == 1 ? true : false, title: this.props.data.elements.setoutdate.title }} ></Lable>
                        <div className="col-md-10">
                            <div ref="setoutdatepicker" className="input-group input-medium  input-group-sm ddDatePicker date date-picker  " >
                                <input ref="setoutdate" readOnly type="text" className="form-control input-medium input readOnlyDatePicker" defaultValue={this.state.postValue[setoutdate['code']]} onPaste={this.handlerChange} />
                                <span className="input-group-btn">
                                    <button className="btn default" type="button">
                                        <i className="fa fa-calendar"></i>
                                    </button>
                                </span>
                            </div>

                            <span className={this.state.setoutclassname}>{this.state.setouttips}</span>
                        </div>
                    </div>

                    <div className={retrunVisiblity} data-suborderid={suborderid} data-componentid={componentid} data-segmentcode={returndate['code']} >
                        <Lable params={{ isMust: this.props.data.mandatory == 1 ? true : false, title: this.props.data.elements.returndate.title }} ></Lable>
                        <div className="col-md-10">
                            <div ref="returndatepicker" className="input-group input-medium  input-group-sm ddDatePicker date date-picker  " >
                                <input ref="returndate" readOnly type="text" className="form-control input-medium readOnlyDatePicker" defaultValue={this.state.postValue[returndate['code']]} onPaste={this.handlerChange} />
                                <span className="input-group-btn">
                                    <button className="btn default" type="button">
                                        <i className="fa fa-calendar"></i>
                                    </button>
                                </span>
                            </div>
                            <span className={this.state.returnclassname}>{this.state.returntips}</span>
                        </div>
                    </div>
                </div>
            );
        }
        else if (this.props.UIType == "reChange") {

            var lastValue = this.prehandledata(this.props.beforeRechangeValue);

            var setoutDateChange = 0;
            var returnDateChange = 0;
            var valueSetOutNow = "";
            var valueReturnNow = "";

            if (lastValue == this.state.postValue) {
                setoutDateChange = 0;
                returnDateChange = 0;
                valueSetOutNow = '';
                valueReturnNow = '';
            }
            else {
                if ((lastValue[setoutdate.code] == this.state.postValue[setoutdate.code]) || this.state.postValue[setoutdate.code] == '') {
                    setoutDateChange = 0;
                    valueSetOutNow = '';
                }
                else {
                    setoutDateChange = 1;
                    valueSetOutNow = this.state.postValue[setoutdate.code];
                }
                if ((lastValue[returndate.code] == this.state.postValue[returndate.code]) || this.state.postValue[setoutdate.code] == '') {
                    returnDateChange = 0;
                    valueReturnNow = '';
                }
                else {
                    returnDateChange = 1;
                    valueReturnNow = this.state.postValue[returndate.code];
                }

            }
            var changeObj = new Object();
            changeObj[this.props.suborderid] = new Object();

            changeObj[this.props.suborderid][this.props.data.elements.setoutdate.code] = new Object();
            changeObj[this.props.suborderid][this.props.data.elements.setoutdate.code].isValueChange = setoutDateChange;

            changeObj[this.props.suborderid][this.props.data.elements.returndate.code] = new Object();
            changeObj[this.props.suborderid][this.props.data.elements.returndate.code].isValueChange = returnDateChange;
            this.props.reciveValueChange(changeObj);
            return (
                <tbody ref="forceUpdate" className="forceUpdate-setoutreturn">
                    <tr>
                        <td>
                            <LableRechange params={{ isMust: this.props.data.mandatory == 1 ? true : false, title: this.props.data.elements.setoutdate.title }} ></LableRechange>
                        </td>
                        <td>{this.getDefaultValue()[this.props.data.elements.setoutdate.code]}</td>
                        <td>
                            <div ref="setoutdatepicker" className="input-group input-medium  input-group-sm ddDatePicker date date-picker  " >
                                <input ref="setoutdate" readOnly defaultValue={valueSetOutNow} type="text" className="form-control input-medium input readOnlyDatePicker" onPaste={this.handlerChange} />
                                <span className="input-group-btn">
                                    <button className="btn default" type="button">
                                        <i className="fa fa-calendar"></i>
                                    </button>
                                </span>
                            </div>

                            <div><span className={this.state.setoutclassname}>{this.state.setouttips}</span></div>
                        </td>
                    </tr>

                    <tr className={returnVisibiityRe}>
                        <td>
                            <LableRechange params={{ isMust: this.props.data.mandatory == 1 ? true : false, title: this.props.data.elements.returndate.title }} ></LableRechange>
                        </td>
                        <td>{this.getDefaultValue()[this.props.data.elements.returndate.code]}</td>
                        <td>
                            <div ref="returndatepicker" className="input-group input-medium  input-group-sm ddDatePicker date date-picker  " >
                                <input ref="returndate" readOnly defaultValue={valueReturnNow} type="text" className="form-control input-medium readOnlyDatePicker" onPaste={this.handlerChange} />
                                <span className="input-group-btn">
                                    <button className="btn default" type="button">
                                        <i className="fa fa-calendar"></i>
                                    </button>
                                </span>
                            </div>
                            <div><span className={this.state.returnclassname}>{this.state.returntips}</span></div>
                        </td>
                    </tr>
                </tbody>
            )
        }
    }
})

