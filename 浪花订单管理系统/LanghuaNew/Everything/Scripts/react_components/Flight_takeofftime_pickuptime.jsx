



var Flight_takeofftime_pickuptimeRow = React.createClass({

    prehandledata: function (data) {

        if (data == "emptyrow") {
            var elements = this.props.data.elements;
            var obj = new Object();
            obj[elements.takeofftime.code] = "&-1&-1";
            obj[elements.pickuptime.code] = "&-1&-1";

            return (obj);
        }
        return data;
    },
    prehandleupdatedata: function (data) {

        return data;
    },

    componentDidMount: function (e) {
        // 可以使用this.getDOMNode()
        // 引用外部库的钩子函数
        // react 运行在服务端时候，该方法 不被调用

        this.detectValueWhenMounted();

        var reciveTempValueLastObj = this.state.postValue;
        var takeoffArr = (reciveTempValueLastObj[this.props.data.elements.takeofftime.code]).toString().split('&');
        var pickupArr = (reciveTempValueLastObj[this.props.data.elements.pickuptime.code]).toString().split('&');
        var reciveTempValueLast = new Object();
        reciveTempValueLast[this.props.suborderid] = new Object();
        reciveTempValueLast[this.props.suborderid][this.props.data.elements.takeofftime.code] = takeoffArr[0] + ' ' + takeoffArr[1] + ':' + takeoffArr[2];
        reciveTempValueLast[this.props.suborderid][this.props.data.elements.pickuptime.code] = pickupArr[0] + ' ' + pickupArr[1] + ':' + pickupArr[2];
        this.props.reciveTempValue(reciveTempValueLast);

        //更新粘贴板内容
        var copyObj ={};
        copyObj[this.props.data.elements.takeofftime.code]={
            title:this.props.data.elements.takeofftime.title,
            str:reciveTempValueLast[this.props.suborderid][this.props.data.elements.takeofftime.code]
        }
        copyObj[this.props.data.elements.pickuptime.code]={
            title:this.props.data.elements.pickuptime.title,
            str:reciveTempValueLast[this.props.suborderid][this.props.data.elements.pickuptime.code]
        }
        this.updateCopyObj(copyObj);

        //更新系统字段
        var formFields = new Object();
        formFields[this.props.data.elements.takeofftime.code] = takeoffArr[0];
        formFields[this.props.data.elements.pickuptime.code] = pickupArr[0];
        var elements = this.props.data.elements;
        var objdate = new Object();
        objdate[this.props.suborderid] = new Object();
        for (var s in elements) {
            for (var ss in elements[s]['systemFieldMap']) {
                objdate[this.props.suborderid][elements[s]['systemFieldMap'][ss]['postKey']] = formFields[elements[s]['code']];
            }
        }
        this.props.recieveSystemFields(objdate);

        var _this = this;
        var thisDatePicker = this;
        if (this.props.limit === false) {
            initTravelDateWithoutRule();
        }
        else {
            $.ajax({
                url: "/ServiceRules/GetRulesByItemID/" + this.props.serviceItemID,
                type: 'get',
                dataType: "json",
                success: function (ruleArr) {
                    initTravelDateWithRule(ruleArr);
                }
            });
        }
        initOthers();
        function initTravelDateWithoutRule() {
            jQuery(_this.refs.pickupdatepicker).datepicker({
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
                })()
            })
        }
        function initTravelDateWithRule(ruleArr) {
            jQuery(_this.refs.pickupdatepicker).datepicker({
                orientation: "top right",
                startDate: (function () {
                    if (thisDatePicker.props.isForCusClient) {
                        return '+0d';
                    }
                    return '1900-01-01';
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
                        var timestampStart = (new Date(timeStart[0],timeStart[1]-1,timeStart[2],0,0,0)).valueOf();
                        var timestampEnd = (new Date(timeEnd[0],timeEnd[1]-1,timeEnd[2],0,0,0)).valueOf();
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
                                    var timestampRangeStart = (new Date(timeRangeStart[0],timeRangeStart[1]-1,timeRangeStart[2],0,0,0)).valueOf();
                                    var timestampRangeEnd = (new Date(timeRangeEnd[0],timeRangeEnd[1]-1,timeRangeEnd[2],0,0,0)).valueOf();
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

                    return DateState;
                }

            });
        }
        function initOthers() {
            jQuery(_this.refs.takeoffdatepicker).datepicker({
                orientation: "top right"
            }).on('changeDate', function (ev) {
                var date = ev.date ? ev.date.valueOf() : '';

                if (date != '') {
                    var newDate = new Date(date);
                    var year = newDate.getFullYear();
                    var month = String((parseInt(newDate.getMonth()) + 1)).length == 1 ? "0" + String((parseInt(newDate.getMonth()) + 1)) : String((parseInt(newDate.getMonth()) + 1));
                    var date = String(parseInt(newDate.getDate())).length == 1 ? "0" + String((parseInt(newDate.getDate()))) : String((parseInt(newDate.getDate())));
                    var takeoffdate = year + '-' + month + '-' + date;
                }
                else {
                    var takeoffdate = ''
                }
                var takeoffhour = (_this.refs.takeoffhour.value) == -1 ? "-1" : (function () {
                    if (String(_this.refs.takeoffhour.value).length == 1) {
                        return "0" + _this.refs.takeoffhour.value;
                    }
                    return _this.refs.takeoffhour.value;
                })();
                var takeoffmin = (_this.refs.takeoffmin.value) == -1 ? "-1" : (function () {
                    if (String(_this.refs.takeoffmin.value).length == 1) {
                        return "0" + _this.refs.takeoffmin.value;
                    }
                    return _this.refs.takeoffmin.value;
                })();

                var takeoffLastValue = takeoffdate + "&" + takeoffhour + "&" + takeoffmin;

                var pickupdate = $(_this.refs.pickupdate).val();


                if (pickupdate) {
                    var ARRpickuptime = pickupdate.split('-');
                    var newDate = new Date(parseInt(ARRpickuptime[0]), parseInt(ARRpickuptime[1] - 1), parseInt(ARRpickuptime[2]), 0, 0,0);
                    var year = newDate.getFullYear();
                    var month = String((parseInt(newDate.getMonth()) + 1)).length == 1 ? "0" + String((parseInt(newDate.getMonth()) + 1)) : String((parseInt(newDate.getMonth()) + 1));
                    var date = String(parseInt(newDate.getDate()) + 1).length == 1 ? "0" + String((parseInt(newDate.getDate()))) : String((parseInt(newDate.getDate())));
                    pickupdate = year + '-' + month + '-' + date;
                }
                else {
                    pickupdate = ''
                }

                var pickuphour = (_this.refs.pickuphour.value) == -1 ? "-1" : (function () {
                    if (String(_this.refs.pickuphour.value).length == 1) {
                        return "0" + _this.refs.pickuphour.value;
                    }
                    return _this.refs.pickuphour.value;
                })();
                var pickupmin = (_this.refs.pickupmin.value) == -1 ? "-1" : (function () {
                    if (String(_this.refs.pickupmin.value).length == 1) {
                        return "0" + _this.refs.pickupmin.value;
                    }
                    return _this.refs.pickupmin.value;
                })();
                var pickupLastValue = pickupdate + "&" + pickuphour + "&" + pickupmin;





                var obj = {};
                obj[_this.props.data.elements.takeofftime.code] = takeoffLastValue;
                obj[_this.props.data.elements.pickuptime.code] = pickupLastValue;

                //回复默认值
                if (_this.props.UIType == "reChange") {
                    if (takeoffLastValue == '&-1&-1') {
                        var lastValue = _this.prehandledata(_this.props.beforeRechangeValue);
                        obj[_this.props.data.elements.takeofftime.code] = lastValue[_this.props.data.elements.takeofftime.code];
                    }
                    if (pickupLastValue == '&-1&-1') {
                        var lastValue = _this.prehandledata(_this.props.beforeRechangeValue);
                        obj[_this.props.data.elements.pickuptime.code] = lastValue[_this.props.data.elements.pickuptime.code];
                    }
                }
                //检测值
                var takeoffArrs = obj[_this.props.data.elements.takeofftime.code].split("&");
                var pickupArrs = obj[_this.props.data.elements.pickuptime.code].split("&");
                _this.detectValueState(
                    takeoffArrs[0],
                    takeoffArrs[1],
                    takeoffArrs[2],
                    pickupArrs[0],
                    pickupArrs[1],
                    pickupArrs[2]
                );



                _this.setState(
                    {
                        postValue: obj
                    },

                    function () {
                        _this.upadteToForm(_this.getdUpdateData());

                        var reciveTempValueLastObj = _this.state.postValue;
                        var takeoffArr = (reciveTempValueLastObj[_this.props.data.elements.takeofftime.code]).toString().split('&');
                        var pickupArr = (reciveTempValueLastObj[_this.props.data.elements.pickuptime.code]).toString().split('&');
                        var reciveTempValueLast = new Object();
                        reciveTempValueLast[_this.props.suborderid] = new Object();
                        reciveTempValueLast[_this.props.suborderid][_this.props.data.elements.takeofftime.code] = takeoffArr[0] + ' ' + takeoffArr[1] + ':' + takeoffArr[2];
                        reciveTempValueLast[_this.props.suborderid][_this.props.data.elements.pickuptime.code] = pickupArr[0] + ' ' + pickupArr[1] + ':' + pickupArr[2];
                        _this.props.reciveTempValue(reciveTempValueLast);

                        //更新粘贴板内容
                        var copyObj ={};
                        copyObj[_this.props.data.elements.takeofftime.code]={
                            title:_this.props.data.elements.takeofftime.title,
                            str:reciveTempValueLast[_this.props.suborderid][_this.props.data.elements.takeofftime.code]
                        }
                        copyObj[_this.props.data.elements.pickuptime.code]={
                            title:_this.props.data.elements.pickuptime.title,
                            str:reciveTempValueLast[_this.props.suborderid][_this.props.data.elements.pickuptime.code]
                        }
                        _this.updateCopyObj(copyObj);
                    }
                );
            }).on("hide", function () {
                $(_this.refs.takeoffdatepicker).find('input').blur();
            });

            jQuery(_this.refs.pickupdatepicker).on('changeDate', function (ev) {
                var date = ev.date ? ev.date.valueOf() : '';
                if (date != '') {
                    var newDate = new Date(date);
                    var year = newDate.getFullYear();
                    var month = String((parseInt(newDate.getMonth()) + 1)).length == 1 ? "0" + String((parseInt(newDate.getMonth()) + 1)) : String((parseInt(newDate.getMonth()) + 1));
                    var date = String(parseInt(newDate.getDate())).length == 1 ? "0" + String((parseInt(newDate.getDate()))) : String((parseInt(newDate.getDate())));
                    var pickupdate = year + '-' + month + '-' + date;
                }
                else {
                    var pickupdate = ''
                }
                var pickuphour = (_this.refs.pickuphour.value) == -1 ? "-1" : (function () {
                    if (String(_this.refs.pickuphour.value).length == 1) {
                        return "0" + _this.refs.pickuphour.value;
                    }
                    return _this.refs.pickuphour.value;
                })();
                var pickupmin = (_this.refs.pickupmin.value) == -1 ? "-1" : (function () {
                    if (String(_this.refs.pickupmin.value).length == 1) {
                        return "0" + _this.refs.pickupmin.value;
                    }
                    return _this.refs.pickupmin.value;
                })();
                var pickupfLastValue = pickupdate + "&" + pickuphour + "&" + pickupmin;


                var takeoffdate = $(_this.refs.takeoffdate).val();
                if (takeoffdate) {
                    var ARRtakeofftime = takeoffdate.split('-');
                    var newDate = new Date(parseInt(ARRtakeofftime[0]), parseInt(ARRtakeofftime[1] - 1), parseInt(ARRtakeofftime[2]), 0, 0,0);
                    var year = newDate.getFullYear();
                    var month = String((parseInt(newDate.getMonth()) + 1)).length == 1 ? "0" + String((parseInt(newDate.getMonth()) + 1)) : String((parseInt(newDate.getMonth()) + 1));
                    var date = String(parseInt(newDate.getDate())).length == 1 ? "0" + String((parseInt(newDate.getDate()))) : String((parseInt(newDate.getDate())));
                    takeoffdate = year + '-' + month + '-' + date;
                }
                else {
                    takeoffdate = ''
                }
                var takeoffhour = (_this.refs.takeoffhour.value) == -1 ? "-1" : (function () {
                    if (String(_this.refs.takeoffhour.value).length == 1) {
                        return "0" + _this.refs.takeoffhour.value;
                    }
                    return _this.refs.takeoffhour.value;
                })();
                var takeoffmin = (_this.refs.takeoffmin.value) == -1 ? "-1" : (function () {
                    if (String(_this.refs.takeoffmin.value).length == 1) {
                        return "0" + _this.refs.takeoffmin.value;
                    }
                    return _this.refs.takeoffmin.value;
                })();
                var takeoffLastValue = takeoffdate + "&" + takeoffhour + "&" + takeoffmin;


                var obj = {};
                obj[_this.props.data.elements.takeofftime.code] = takeoffLastValue;
                obj[_this.props.data.elements.pickuptime.code] = pickupfLastValue;

                //回复默认值
                if (_this.props.UIType == "reChange") {
                    if (takeoffLastValue == '&-1&-1') {
                        var lastValue = _this.prehandledata(_this.props.beforeRechangeValue);
                        obj[_this.props.data.elements.takeofftime.code] = lastValue[_this.props.data.elements.takeofftime.code];
                    }
                    if (pickupfLastValue == '&-1&-1') {
                        var lastValue = _this.prehandledata(_this.props.beforeRechangeValue);
                        obj[_this.props.data.elements.pickuptime.code] = lastValue[_this.props.data.elements.pickuptime.code];
                    }
                }

                //检测值
                var takeoffArrs = obj[_this.props.data.elements.takeofftime.code].split("&");
                var pickupArrs = obj[_this.props.data.elements.pickuptime.code].split("&");
                _this.detectValueState(
                    takeoffArrs[0],
                    takeoffArrs[1],
                    takeoffArrs[2],
                    pickupArrs[0],
                    pickupArrs[1],
                    pickupArrs[2]
                );

                _this.setState(
                    {
                        postValue: obj
                    },

                    function () {
                        _this.upadteToForm(_this.getdUpdateData());

                        var reciveTempValueLastObj = _this.state.postValue;
                        var takeoffArr = (reciveTempValueLastObj[_this.props.data.elements.takeofftime.code]).toString().split('&');
                        var pickupArr = (reciveTempValueLastObj[_this.props.data.elements.pickuptime.code]).toString().split('&');
                        var reciveTempValueLast = new Object();
                        reciveTempValueLast[_this.props.suborderid] = new Object();
                        reciveTempValueLast[_this.props.suborderid][_this.props.data.elements.takeofftime.code] = takeoffArr[0] + ' ' + takeoffArr[1] + ':' + takeoffArr[2];
                        reciveTempValueLast[_this.props.suborderid][_this.props.data.elements.pickuptime.code] = pickupArr[0] + ' ' + pickupArr[1] + ':' + pickupArr[2];
                        _this.props.reciveTempValue(reciveTempValueLast);

                        //更新粘贴板内容
                        var copyObj ={};
                        copyObj[_this.props.data.elements.takeofftime.code]={
                            title:_this.props.data.elements.takeofftime.title,
                            str:reciveTempValueLast[_this.props.suborderid][_this.props.data.elements.takeofftime.code]
                        }
                        copyObj[_this.props.data.elements.pickuptime.code]={
                            title:_this.props.data.elements.pickuptime.title,
                            str:reciveTempValueLast[_this.props.suborderid][_this.props.data.elements.pickuptime.code]
                        }
                        _this.updateCopyObj(copyObj);


                        //更新系统字段
                        var formFields = new Object();
                        formFields[_this.props.data.elements.takeofftime.code] = takeoffArr[0];
                        formFields[_this.props.data.elements.pickuptime.code] = pickupArr[0];
                        var elements = _this.props.data.elements;
                        var objdate = new Object();
                        objdate[_this.props.suborderid] = new Object();
                        for (var s in elements) {
                            for (var ss in elements[s]['systemFieldMap']) {
                                objdate[_this.props.suborderid][elements[s]['systemFieldMap'][ss]['postKey']] = formFields[elements[s]['code']];
                            }
                        }
                        _this.props.recieveSystemFields(objdate);
                    }
                );
            }).on("hide", function () {
                $(_this.refs.pickupdatepicker).find('input').blur();
            });
        }

    },

    mixins: [MXINS_UPDATE_DATA],
    detectValueState: function (takeoffdate, takeoffhour, takeoffmin, pickupdate, pickuphour, pickupmin) {
        console.log(takeoffdate)
        console.log(takeoffhour)
        console.log(takeoffmin)
        console.log(pickupdate)
        console.log(pickuphour)
        console.log(pickupmin)
        var isTakeoffValueUseAble = 1;
        var isTakeoffNotEmpty = 1;
        var isPickupValueUseAble = 1;
        var isPickupNotEmpty = 1;
        var takeoffclassname = "";
        var takeofftips = "";
        var pickupclassname = "";
        var pickuptips = "";
        if (
            takeoffdate &&
            takeoffhour != -1 &&
            takeoffmin != -1 &&
            pickupdate &&
            pickuphour != -1 &&
            pickupmin != -1
        ) {
            var ARRtakeofftime = takeoffdate.split('-');
            var ARRpickuptime = pickupdate.split('-');
            var datetakeofftime = new Date(parseInt(ARRtakeofftime[0]), parseInt(ARRtakeofftime[1] - 1), parseInt(ARRtakeofftime[2]), takeoffhour, takeoffmin);
            var datepickuptime = new Date(parseInt(ARRpickuptime[0]), parseInt(ARRpickuptime[1] - 1), parseInt(ARRpickuptime[2]), pickuphour, pickupmin);
            var takeofftimestamp = datetakeofftime.valueOf();
            var pickuptimestamp = datepickuptime.valueOf();
            var hourA = parseInt(this.props.data.timeAdvanced);
            var hourAMax = 10;
            if (pickuptimestamp > takeofftimestamp - hourA * 60 * 60 * 1000) {
                takeoffstate = 0;
                pickupstate = 0;
                isTakeoffValueUseAble = 0;
                isTakeoffNotEmpty = 1;
                isPickupValueUseAble = 0;
                isPickupNotEmpty = 1;
                takeoffclassname = "help-inline tips";
                takeofftips = this.props.data.elements.pickuptime.title + "起码提前" + this.props.data.elements.takeofftime.title +">="+ hourA + "个小时！";
                pickupclassname = "help-inline tips";
                pickuptips = this.props.data.elements.pickuptime.title + "起码提前" + this.props.data.elements.takeofftime.title +">="+ hourA + "个小时！";;

                //变更变更的更正为单个提示语
                if (this.props.UIType == "reChange") {
                    var lastValue = this.prehandledata(this.props.beforeRechangeValue);
                    if (takeoffdate + '&' + takeoffhour + '&' + takeoffmin == lastValue[this.props.data.elements.takeofftime.code]) {
                        takeoffclassname = "help-inline";
                        takeofftips = this.props.data.elements.takeofftime.tips;
                    }
                    if (pickupdate + '&' + pickuphour + '&' + pickupmin == lastValue[this.props.data.elements.pickuptime.code]) {
                        pickupclassname = "help-inline";
                        pickuptips = this.props.data.elements.pickuptime.tips;
                    }
                }
            }

            else {
                isTakeoffValueUseAble = 1;
                isTakeoffNotEmpty = 1;
                isPickupValueUseAble = 1;
                isPickupNotEmpty = 1;
                takeoffclassname = "help-inline";
                takeofftips = this.props.data.elements.takeofftime.tips;
                pickupclassname = "help-inline";
                pickuptips = this.props.data.elements.pickuptime.tips;
            }
            if (pickuptimestamp <= takeofftimestamp - hourAMax * 60 * 60 * 1000) {
                takeoffstate = 0;
                pickupstate = 0;
                isTakeoffValueUseAble = 0;
                isTakeoffNotEmpty = 1;
                isPickupValueUseAble = 0;
                isPickupNotEmpty = 1;
                takeoffclassname = "help-inline tips";
                takeofftips = this.props.data.elements.pickuptime.title + "提前" + this.props.data.elements.takeofftime.title +"需要<"+hourAMax + "个小时！";
                pickupclassname = "help-inline tips";
                pickuptips =  this.props.data.elements.pickuptime.title + "提前" + this.props.data.elements.takeofftime.title +"需要<"+hourAMax + "个小时！"; 

                //变更变更的更正为单个提示语
                if (this.props.UIType == "reChange") {
                    var lastValue = this.prehandledata(this.props.beforeRechangeValue);
                    if (takeoffdate + '&' + takeoffhour + '&' + takeoffmin == lastValue[this.props.data.elements.takeofftime.code]) {
                        takeoffclassname = "help-inline";
                        takeofftips = this.props.data.elements.takeofftime.tips;
                    }
                    if (pickupdate + '&' + pickuphour + '&' + pickupmin == lastValue[this.props.data.elements.pickuptime.code]) {
                        pickupclassname = "help-inline";
                        pickuptips = this.props.data.elements.pickuptime.tips;
                    }
                }
            }
        }
        else {
            if (
                !takeoffdate ||
                takeoffhour == -1 ||
                takeoffmin == -1
            ) {
                takeoffclassname = "help-inline tips";
                takeofftips = "请填写完整的"+this.props.data.elements.takeofftime.title;
                //请求变更时tips只显示单个
                if (this.props.UIType == "reChange") {
                    var lastValue = this.prehandledata(this.props.beforeRechangeValue);
                    if (takeoffdate + ' ' + takeoffhour + ':' + takeoffmin == lastValue[this.props.data.elements.takeofftime.code]) {
                        takeoffclassname = "help-inline";
                        takeofftips = this.props.data.elements.takeofftime.tips;
                    }
                }
                isTakeoffValueUseAble = 0;
                isTakeoffNotEmpty = 0;
            }
            else {
                takeoffclassname = "help-inline";
                takeofftips = this.props.data.elements.takeofftime.tips;
                isTakeoffValueUseAble = 1;
                isTakeoffNotEmpty = 1;
            }
            if (
                (!pickupdate) ||
                pickuphour == -1 ||
                pickupmin == -1
            ) {
                pickupclassname = "help-inline tips";
                pickuptips = "请填写完整的"+this.props.data.elements.pickuptime.title;;
                isPickupValueUseAble = 0;
                isPickupNotEmpty = 0;
                //请求变更时tips只显示单个
                if (this.props.UIType == "reChange") {
                    var lastValue = this.prehandledata(this.props.beforeRechangeValue);
                    if (pickupdate + ' ' + pickuphour + ':' + pickupmin == lastValue[this.props.data.elements.pickuptime.code]) {
                        pickupclassname = "help-inline";
                        pickuptips = this.props.data.elements.pickuptime.tips;
                    }
                }
            }
            else {
                pickupclassname = "help-inline";
                pickuptips = this.props.data.elements.pickuptime.tips;
                isPickupValueUseAble = 1;
                isPickupNotEmpty = 1;
            }
        }

        if (this.props.limit === false || (this.props.data.mandatory == "2")) {//unlimited
            takeoffclassname = "help-inline";
            takeofftips = this.props.data.elements.takeofftime.tips;
            pickupclassname = "help-inline";
            pickuptips = this.props.data.elements.pickuptime.tips;
        }
        this.setState({
            'takeoffclassname': takeoffclassname,
            'takeofftips': takeofftips,
            'pickupclassname': pickupclassname,
            'pickuptips': pickuptips
        });

        var stateObj = new Object();
        stateObj[this.props.suborderid] = new Object();

        stateObj[this.props.suborderid][this.props.data.elements.takeofftime.code] = new Object();
        stateObj[this.props.suborderid][this.props.data.elements.takeofftime.code].isValueUseAble = isTakeoffValueUseAble;
        stateObj[this.props.suborderid][this.props.data.elements.takeofftime.code].isValueNotEmpty = isTakeoffNotEmpty;
        stateObj[this.props.suborderid][this.props.data.elements.takeofftime.code].isValueMandatory = this.props.data.mandatory;

        stateObj[this.props.suborderid][this.props.data.elements.pickuptime.code] = new Object();
        stateObj[this.props.suborderid][this.props.data.elements.pickuptime.code].isValueUseAble = isPickupValueUseAble;
        stateObj[this.props.suborderid][this.props.data.elements.pickuptime.code].isValueNotEmpty = isPickupNotEmpty;
        stateObj[this.props.suborderid][this.props.data.elements.pickuptime.code].isValueMandatory = this.props.data.mandatory;

        this.props.reciveValueState(stateObj);


    },
    detectValueWhenMounted: function () {
        if (this.getDefaultValue() == "emptyrow") {
            this.setState(
                {
                    takeoffclassname: "help-inline",
                    takeofftips: this.props.data.elements.takeofftime.tips,
                    pickupclassname: "help-inline",
                    pickuptips: this.props.data.elements.pickuptime.tips
                }
            );
            var stateObj = new Object();
            stateObj[this.props.suborderid] = new Object();
            stateObj[this.props.suborderid][this.props.data.elements.takeofftime.code] = 0;
            stateObj[this.props.suborderid][this.props.data.elements.pickuptime.code] = 0;
            this.props.reciveValueState(stateObj);
        }
        else {
            var takeofftime = this.props.data.elements.takeofftime.code;
            var pickuptime = this.props.data.elements.pickuptime.code;


            var lastValueArrT = this.state.postValue[takeofftime].toString().split("&");
            var lastValueDateT = lastValueArrT[0];
            var lastValueHourT = lastValueArrT[1];
            var lastValueMinT = lastValueArrT[2];

            var lastValueArrP = this.state.postValue[pickuptime].toString().split("&");
            var lastValueDateP = lastValueArrP[0];
            var lastValueHourP = lastValueArrP[1];
            var lastValueMinP = lastValueArrP[2];
            this.detectValueState(
                lastValueDateT,
                lastValueHourT,
                lastValueMinT,
                lastValueDateP,
                lastValueHourP,
                lastValueMinP
            );
        }
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
            <select ref={refname} defaultValue={value} className="form-control input-inline input100 input-sm" onChange={this.handlerChange}>
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
            <select ref={refname} defaultValue={value} name="mins" className="form-control input-inline input80 input-sm" onChange={this.handlerChange} >
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

    handlerChange: function (e) {
        var _this = this;
        var takeoffdate = $(_this.refs.takeoffdate).val();

        if (takeoffdate) {
            var ARRtakeofftime = takeoffdate.split('-');
            var newDate = new Date(parseInt(ARRtakeofftime[0]), parseInt(ARRtakeofftime[1] - 1), parseInt(ARRtakeofftime[2]), 0, 0,0);
            var year = newDate.getFullYear();
            var month = String((parseInt(newDate.getMonth()) + 1)).length == 1 ? "0" + String((parseInt(newDate.getMonth()) + 1)) : String((parseInt(newDate.getMonth()) + 1));
            var date = String(parseInt(newDate.getDate()) + 1).length == 1 ? "0" + String((parseInt(newDate.getDate()))) : String((parseInt(newDate.getDate())));
            var takeoffdate = year + '-' + month + '-' + date;
        }
        else {
            var takeoffdate = ''
        }

        var takeoffhour = (_this.refs.takeoffhour.value) == -1 ? "-1" : (function () {
            if (String(_this.refs.takeoffhour.value).length == 1) {
                return "0" + _this.refs.takeoffhour.value;
            }
            return _this.refs.takeoffhour.value;
        })();
        var takeoffmin = (_this.refs.takeoffmin.value) == -1 ? "-1" : (function () {
            if (String(_this.refs.takeoffmin.value).length == 1) {
                return "0" + _this.refs.takeoffmin.value;
            }
            return _this.refs.takeoffmin.value;
        })();

        var takeoffLastValue = takeoffdate + "&" + takeoffhour + "&" + takeoffmin;




        var pickupdate = $(_this.refs.pickupdate).val();
        if (pickupdate) {
            var ARRpickuptime = pickupdate.split('-');
            var newDate = new Date(parseInt(ARRpickuptime[0]), parseInt(ARRpickuptime[1] - 1), parseInt(ARRpickuptime[2]), 0, 0,0);
            var year = newDate.getFullYear();
            var month = String((parseInt(newDate.getMonth()) + 1)).length == 1 ? "0" + String((parseInt(newDate.getMonth()) + 1)) : String((parseInt(newDate.getMonth()) + 1));
            var date = String(parseInt(newDate.getDate()) + 1).length == 1 ? "0" + String((parseInt(newDate.getDate()))) : String((parseInt(newDate.getDate())));
            var pickupdate = year + '-' + month + '-' + date;
        }
        else {
            var pickupdate = ''
        }
        var pickuphour = (_this.refs.pickuphour.value) == -1 ? "-1" : (function () {
            if (String(_this.refs.pickuphour.value).length == 1) {
                return "0" + _this.refs.pickuphour.value;
            }
            return _this.refs.pickuphour.value;
        })();
        var pickupmin = (_this.refs.pickupmin.value) == -1 ? "-1" : (function () {
            if (String(_this.refs.pickupmin.value).length == 1) {
                return "0" + _this.refs.pickupmin.value;
            }
            return _this.refs.pickupmin.value;
        })();
        var pickupLastValue = pickupdate + "&" + pickuphour + "&" + pickupmin;




        var obj = {};
        obj[this.props.data.elements.takeofftime.code] = takeoffLastValue;
        obj[this.props.data.elements.pickuptime.code] = pickupLastValue;

        //回复默认值

        if (this.props.UIType == "reChange") {
            if (takeoffLastValue == '&-1&-1') {
                var lastValue = this.prehandledata(this.props.beforeRechangeValue);
                obj[this.props.data.elements.takeofftime.code] = lastValue[this.props.data.elements.takeofftime.code];
            };

            if (pickupLastValue == '&-1&-1') {
                var lastValue = this.prehandledata(this.props.beforeRechangeValue);
                obj[this.props.data.elements.pickuptime.code] = lastValue[this.props.data.elements.pickuptime.code];
            }

        }

        //检测值
        var takeoffArrs = obj[_this.props.data.elements.takeofftime.code].split("&");
        var pickupArrs = obj[_this.props.data.elements.pickuptime.code].split("&");
        _this.detectValueState(
            takeoffArrs[0],
            takeoffArrs[1],
            takeoffArrs[2],
            pickupArrs[0],
            pickupArrs[1],
            pickupArrs[2],
        );

        this.setState(
            {
                postValue: obj
            },

            function () {
                this.upadteToForm(this.getdUpdateData());
                var reciveTempValueLastObj = this.state.postValue;
                var takeoffArr = (reciveTempValueLastObj[this.props.data.elements.takeofftime.code]).toString().split('&');
                var pickupArr = (reciveTempValueLastObj[this.props.data.elements.pickuptime.code]).toString().split('&');
                var reciveTempValueLast = new Object();
                reciveTempValueLast[this.props.suborderid] = new Object();
                reciveTempValueLast[this.props.suborderid][this.props.data.elements.takeofftime.code] = takeoffArr[0] + ' ' + takeoffArr[1] + ':' + takeoffArr[2];
                reciveTempValueLast[this.props.suborderid][this.props.data.elements.pickuptime.code] = pickupArr[0] + ' ' + pickupArr[1] + ':' + pickupArr[2];
                this.props.reciveTempValue(reciveTempValueLast);

                //更新粘贴板内容
                var copyObj ={};
                copyObj[this.props.data.elements.takeofftime.code]={
                    title:this.props.data.elements.takeofftime.title,
                    str:reciveTempValueLast[this.props.suborderid][this.props.data.elements.takeofftime.code]
                }
                copyObj[this.props.data.elements.pickuptime.code]={
                    title:this.props.data.elements.pickuptime.title,
                    str:reciveTempValueLast[this.props.suborderid][this.props.data.elements.pickuptime.code]
                }
                this.updateCopyObj(copyObj);
            }
        );

    },
    render: function () {
        var suborderid = this.props.suborderid;
        var componentid = this.props.componentid;
        var takeofftime = this.props.data.elements.takeofftime.code;
        var pickuptime = this.props.data.elements.pickuptime.code;


        var lastValueArrT = this.state.postValue[takeofftime].toString().split("&");
        var lastValueDateT = lastValueArrT[0];
        var lastValueHourT = parseInt(lastValueArrT[1]);
        var lastValueMinT = parseInt(lastValueArrT[2]);

        var lastValueArrP = this.state.postValue[pickuptime].toString().split("&");
        var lastValueDateP = lastValueArrP[0];
        var lastValueHourP = parseInt(lastValueArrP[1]);
        var lastValueMinP = parseInt(lastValueArrP[2]);
        if (this.props.UIType == "edit") {
            return (
                <div>
                    <div className="form-group" data-suborderid={suborderid} data-componentid={componentid} data-segmentcode={this.props.data.elements.takeofftime.code} >
                        <Lable params={{ isMust: this.props.data.mandatory == 1 ? true : false, title: this.props.data.elements.takeofftime.title }} ></Lable>
                        <div className="col-md-10">
                            <div ref="takeoffdatepicker" className="input-group input-medium  input-group-sm ddDatePicker date date-picker  " >
                                <input readOnly defaultValue={lastValueDateT} ref="takeoffdate" type="text" className="form-control input-sm readOnlyDatePicker" placeholder="" onChange={this.handlerChange} />
                                <span className="input-group-btn">
                                    <button className="btn default" type="button">
                                        <i className="fa fa-calendar"></i>
                                    </button>
                                </span>
                            </div>
                            <div className="verticalBottom" style={{ 'display': "inline-block;height:30px;white-space:nowrap;width:195px;margin-top:5px;margin-right:5px" }}>
                                {this.hoursMap("takeoffhour", lastValueHourT)}
                                {this.minutesMap("takeoffmin", lastValueMinT)}

                            </div>
                            <span className={this.state.takeoffclassname}>{this.state.takeofftips}</span>
                        </div>
                    </div>

                    <div className="form-group" data-suborderid={suborderid} data-componentid={componentid} data-segmentcode={this.props.data.elements.pickuptime.code} >
                        <Lable params={{ isMust: this.props.data.mandatory == 1 ? true : false, title: this.props.data.elements.pickuptime.title }} ></Lable>
                        <div className="col-md-10">
                            <div ref="pickupdatepicker" className="input-group input-medium  input-group-sm ddDatePicker date date-picker  " >
                                <input readOnly defaultValue={lastValueDateP} ref="pickupdate" type="text" className="form-control input-sm readOnlyDatePicker " placeholder="" onChange={this.handlerChange} />
                                <span className="input-group-btn">
                                    <button className="btn default" type="button">
                                        <i className="fa fa-calendar"></i>
                                    </button>
                                </span>
                            </div>
                            <div style={{ 'display': "inline-block;height:30px;white-space:nowrap;width:195px;margin-top:5px;margin-right:5px" }}>
                                {this.hoursMap("pickuphour", lastValueHourP)}
                                {this.minutesMap("pickupmin", lastValueMinP)}
                            </div>
                            <span className={this.state.pickupclassname}>{this.state.pickuptips}</span>
                        </div>
                    </div>
                </div>
            )
        }
        else if (this.props.UIType == "reChange") {
            var lastValue = this.prehandledata(this.props.beforeRechangeValue);

            var takeoffChange = 0;
            var pickupChange = 0;

            if (lastValue == this.state.postValue) {

                var lastValueDateT = '';
                var lastValueHourT = -1;
                var lastValueMinT = -1;

                var lastValueDateP = "";
                var lastValueHourP = -1;
                var lastValueMinP = -1;

                takeoffChange = 0;
                pickupChange = 0;

            }
            else {
                if (
                    lastValue[takeofftime] == this.state.postValue[takeofftime] ||
                    (this.state.postValue[takeofftime] == "&-1&-1")
                ) {
                    var lastValueDateT = '';
                    var lastValueHourT = -1;
                    var lastValueMinT = -1;

                    takeoffChange = 0;

                }
                else {



                    var lastValueArrT = this.state.postValue[takeofftime].toString().split("&");
                    var lastValueDateT = lastValueArrT[0];
                    var lastValueHourT = parseInt(lastValueArrT[1]);
                    var lastValueMinT = parseInt(lastValueArrT[2]);

                    takeoffChange = 1;


                }
                if (
                    lastValue[pickuptime] == this.state.postValue[pickuptime] ||
                    (this.state.postValue[pickuptime] == "&-1&-1")
                ) {
                    var lastValueDateP = "";
                    var lastValueHourP = -1;
                    var lastValueMinP = -1;

                    pickupChange = 0;
                }
                else {


                    var lastValueArrP = this.state.postValue[pickuptime].toString().split("&");
                    var lastValueDateP = lastValueArrP[0];
                    var lastValueHourP = parseInt(lastValueArrP[1]);
                    var lastValueMinP = parseInt(lastValueArrP[2]);

                    pickupChange = 1;
                }


                var changeObj = new Object();
                changeObj[this.props.suborderid] = new Object();

                changeObj[this.props.suborderid][this.props.data.elements.takeofftime.code] = new Object();
                changeObj[this.props.suborderid][this.props.data.elements.takeofftime.code].isValueChange = takeoffChange;

                changeObj[this.props.suborderid][this.props.data.elements.pickuptime.code] = new Object();
                changeObj[this.props.suborderid][this.props.data.elements.pickuptime.code].isValueChange = pickupChange;
                this.props.reciveValueChange(changeObj)

            }

            //处理展示原始值

            var takeoffOld = this.getDefaultValue()[this.props.data.elements.takeofftime.code].split("&");
            takeoffOld = takeoffOld[0] + ' ' + takeoffOld[1] + ':' + takeoffOld[2];
            var pickupOld = this.getDefaultValue()[this.props.data.elements.pickuptime.code].split("&");
            pickupOld = pickupOld[0] + ' ' + pickupOld[1] + ':' + pickupOld[2];



            return (
                <tbody>
                    <tr>
                        <td>
                            <LableRechange params={{ isMust: this.props.data.mandatory == 1 ? true : false, title: this.props.data.elements.takeofftime.title }} ></LableRechange>
                        </td>
                        <td>{takeoffOld}</td>
                        <td>
                            <div ref="takeoffdatepicker" className="input-group input-medium  input-group-sm ddDatePicker date date-picker  " >
                                <input readOnly defaultValue={lastValueDateT} ref="takeoffdate" type="text" className="form-control input-sm readOnlyDatePicker" placeholder="" onChange={this.handlerChange} />

                                <span className="input-group-btn">
                                    <button className="btn default" type="button">
                                        <i className="fa fa-calendar"></i>
                                    </button>
                                </span>
                            </div>

                            <div className="verticalBottom" style={{ display: "inline-block;height:30px;white-space:nowrap;width:195px;margin-top:5px;" }}>
                                {this.hoursMap("takeoffhour", lastValueHourT)}
                                {this.minutesMap("takeoffmin", lastValueMinT)}

                            </div>
                            <div><span className={this.state.takeoffclassname}>{this.state.takeofftips}</span></div>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <LableRechange params={{ isMust: this.props.data.mandatory == 1 ? true : false, title: this.props.data.elements.pickuptime.title }} ></LableRechange>
                        </td>
                        <td>{pickupOld}</td>
                        <td>
                            <div ref="pickupdatepicker" className="input-group input-medium  input-group-sm ddDatePicker date date-picker  " >
                                <input readOnly defaultValue={lastValueDateP} ref="pickupdate" type="text" className="form-control input-sm readOnlyDatePicker" placeholder="" onChange={this.handlerChange} />
                                <span className="input-group-btn">
                                    <button className="btn default" type="button">
                                        <i className="fa fa-calendar"></i>
                                    </button>
                                </span>
                            </div>

                            <div style={{ display: "inline-block;height:30px;white-space:nowrap;width:195px;margin-top:5px" }}>
                                {this.hoursMap("pickuphour", lastValueHourP)}
                                {this.minutesMap("pickupmin", lastValueMinP)}
                            </div>
                            <div><span className={this.state.pickupclassname}>{this.state.pickuptips}</span></div>
                        </td>
                    </tr>
                </tbody>
            )
        }
    }
})
