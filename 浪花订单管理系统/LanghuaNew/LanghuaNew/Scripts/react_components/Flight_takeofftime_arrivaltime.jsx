var Flight_takeofftime_arrivaltimeRow = React.createClass({
    prehandledata: function (data) {
        if (data == "emptyrow") {
            var elements = this.props.data.elements;
            var obj = new Object();
            obj[elements.takeofftime.code] = "&-1&-1";
            obj[elements.arrivaltime.code] = "&-1&-1";

            return (obj);
        }
        return data;
    },
    prehandleupdatedata: function (data) {

        return data;
    },
    componentDidMount: function (e) {
        //判断值的状态合法性
        this.detectValueWhenMounted();


        //更新模板值
        var reciveTempValueLastObj = this.state.postValue;
        var takeoffArr = (reciveTempValueLastObj[this.props.data.elements.takeofftime.code]).toString().split('&');
        var arrivalArr = (reciveTempValueLastObj[this.props.data.elements.arrivaltime.code]).toString().split('&');
        var reciveTempValueLast = new Object();
        reciveTempValueLast[this.props.suborderid] = new Object();
        if (reciveTempValueLastObj[this.props.data.elements.takeofftime.code] == '&-1&-1') {
            reciveTempValueLast[this.props.suborderid][this.props.data.elements.takeofftime.code] = '';
        }
        else {
            reciveTempValueLast[this.props.suborderid][this.props.data.elements.takeofftime.code] = takeoffArr[0] + ' ' + (takeoffArr[1] == '-1' ? '' : takeoffArr[1]) + ':' + (takeoffArr[2] == '-1' ? '' : takeoffArr[2]);
        }
        if (reciveTempValueLastObj[this.props.data.elements.arrivaltime.code] == '&-1&-1') {
            reciveTempValueLast[this.props.suborderid][this.props.data.elements.arrivaltime.code] = '';
        }
        else {
            reciveTempValueLast[this.props.suborderid][this.props.data.elements.arrivaltime.code] = arrivalArr[0] + ' ' + (arrivalArr[1] == '-1' ? '' : arrivalArr[1]) + ':' + (arrivalArr[2] == '-1' ? '' : arrivalArr[2]);
        }
        this.props.reciveTempValue(reciveTempValueLast);

        //更新粘贴板内容
        var copyObj ={};
        copyObj[this.props.data.elements.takeofftime.code]={
            title:this.props.data.elements.takeofftime.title,
            str: reciveTempValueLast[this.props.suborderid][this.props.data.elements.takeofftime.code]
        }
        copyObj[this.props.data.elements.arrivaltime.code]={
            title:this.props.data.elements.arrivaltime.title,
            str: reciveTempValueLast[this.props.suborderid][this.props.data.elements.arrivaltime.code]
        }
        this.updateCopyObj(copyObj);


        //更新系统字段
        var formFields = new Object();
        formFields[this.props.data.elements.takeofftime.code] = takeoffArr[0];
        formFields[this.props.data.elements.arrivaltime.code] = arrivalArr[0];
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
        function initTravelDateWithRule(ruleArr) {
            jQuery(_this.refs.arrivaldatepicker).datepicker({
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
            })

        };
        function 　initTravelDateWithoutRule() {
            jQuery(_this.refs.arrivaldatepicker).datepicker({
                orientation: "top right",
                startDate: (function () {
                    if (thisDatePicker.props.isForCusClient) {
                        return '+0d';
                    }
                    return -Infinity;
                })()
            });
        };
        function 　initOthers() {
            jQuery(_this.refs.takeoffdatepicker).datepicker({
                orientation: "top right",
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

                //到达
                var arrivaldate = $(_this.refs.arrivaldate).val();

                if (arrivaldate) {
                    var ARRarrivaltime = arrivaldate.split('-');
                    var newDate = new Date(parseInt(ARRarrivaltime[0]), parseInt(ARRarrivaltime[1] - 1), parseInt(ARRarrivaltime[2]), 0, 0,0);
                    var year = newDate.getFullYear();
                    var month = String((parseInt(newDate.getMonth()) + 1)).length == 1 ? "0" + String((parseInt(newDate.getMonth()) + 1)) : String((parseInt(newDate.getMonth()) + 1));
                    var date = String(parseInt(newDate.getDate())).length == 1 ? "0" + String((parseInt(newDate.getDate()))) : String((parseInt(newDate.getDate())));
                    arrivaldate = year + '-' + month + '-' + date;
                }
                else {
                    arrivaldate = ''
                }

                var arrivalhour = (_this.refs.arrivalhour.value) == -1 ? "-1" : (function () {
                    if (String(_this.refs.arrivalhour.value).length == 1) {
                        return "0" + _this.refs.arrivalhour.value;
                    }
                    return _this.refs.arrivalhour.value;
                })();
                var arrivalmin = (_this.refs.arrivalmin.value) == -1 ? "-1" : (function () {
                    if (String(_this.refs.arrivalmin.value).length == 1) {
                        return "0" + _this.refs.arrivalmin.value;
                    }
                    return _this.refs.arrivalmin.value;
                })();
                var arrivalfLastValue = arrivaldate + "&" + arrivalhour + "&" + arrivalmin;


                var obj = {};
                obj[_this.props.data.elements.takeofftime.code] = takeoffLastValue;
                obj[_this.props.data.elements.arrivaltime.code] = arrivalfLastValue;


                //回复默认值
                if (_this.props.UIType == "reChange") {
                    if (takeoffLastValue == '&-1&-1') {
                        var lastValue = _this.prehandledata(_this.props.beforeRechangeValue);
                        obj[_this.props.data.elements.takeofftime.code] = lastValue[_this.props.data.elements.takeofftime.code];
                    }
                    if (arrivalfLastValue == '&-1&-1') {
                        var lastValue = _this.prehandledata(_this.props.beforeRechangeValue);
                        obj[_this.props.data.elements.arrivaltime.code] = lastValue[_this.props.data.elements.arrivaltime.code];
                    }
                }
                //检测值
                var takeoffArrs = obj[_this.props.data.elements.takeofftime.code].split("&");
                var arrivalArrs = obj[_this.props.data.elements.arrivaltime.code].split("&");
                _this.detectValueState(
                    takeoffArrs[0],
                    takeoffArrs[1],
                    takeoffArrs[2],
                    arrivalArrs[0],
                    arrivalArrs[1],
                    arrivalArrs[2],
                );
                _this.setState(
                    {
                        postValue: obj
                    },

                    function () {
                        //缓存值
                        _this.upadteToForm(_this.getdUpdateData());

                        //更新模板值
                        var reciveTempValueLastObj = _this.state.postValue;
                        var takeoffArr = (reciveTempValueLastObj[_this.props.data.elements.takeofftime.code]).toString().split('&');
                        var arrivalArr = (reciveTempValueLastObj[_this.props.data.elements.arrivaltime.code]).toString().split('&');
                        var reciveTempValueLast = new Object();
                        reciveTempValueLast[_this.props.suborderid] = new Object();
                        if (reciveTempValueLastObj[_this.props.data.elements.takeofftime.code] == '&-1&-1') {
                            reciveTempValueLast[_this.props.suborderid][_this.props.data.elements.takeofftime.code] = '';
                        }
                        else {
                            reciveTempValueLast[_this.props.suborderid][_this.props.data.elements.takeofftime.code] = takeoffArr[0] + ' ' + (takeoffArr[1] == '-1' ? '' : takeoffArr[1]) + ':' + (takeoffArr[2] == '-1' ? '' : takeoffArr[2]);
                        }
                        if (reciveTempValueLastObj[_this.props.data.elements.arrivaltime.code] == '&-1&-1') {
                            reciveTempValueLast[_this.props.suborderid][_this.props.data.elements.arrivaltime.code] = '';
                        }
                        else {
                            reciveTempValueLast[_this.props.suborderid][_this.props.data.elements.arrivaltime.code] = arrivalArr[0] + ' ' + (arrivalArr[1] == '-1' ? '' : arrivalArr[1]) + ':' + (arrivalArr[2] == '-1' ? '' : arrivalArr[2]);
                        }
                        _this.props.reciveTempValue(reciveTempValueLast);
                         //更新粘贴板内容
                        var copyObj ={};
                        copyObj[_this.props.data.elements.takeofftime.code]={
                            title:_this.props.data.elements.takeofftime.title,
                            str: reciveTempValueLast[_this.props.suborderid][_this.props.data.elements.takeofftime.code]
                        }
                        copyObj[_this.props.data.elements.arrivaltime.code]={
                            title:_this.props.data.elements.arrivaltime.title,
                            str: reciveTempValueLast[_this.props.suborderid][_this.props.data.elements.arrivaltime.code]
                        }
                        _this.updateCopyObj(copyObj);

                    }
                );
            }).on("hide", function () {
                $(_this.refs.takeoffdatepicker).find('input').blur();
            });

            jQuery(_this.refs.arrivaldatepicker).on('changeDate', function (ev) {
                var date = ev.date ? ev.date.valueOf() : '';
                if (date != '') {
                    var newDate = new Date(date);
                    var year = newDate.getFullYear();
                    var month = String((parseInt(newDate.getMonth()) + 1)).length == 1 ? "0" + String((parseInt(newDate.getMonth()) + 1)) : String((parseInt(newDate.getMonth()) + 1));
                    var date = String(parseInt(newDate.getDate())).length == 1 ? "0" + String((parseInt(newDate.getDate()))) : String((parseInt(newDate.getDate())));
                    var arrivaldate = year + '-' + month + '-' + date;
                }
                else {
                    var arrivaldate = ''
                }
                var arrivalhour = (_this.refs.arrivalhour.value) == -1 ? "-1" : (function () {
                    if (String(_this.refs.arrivalhour.value).length == 1) {
                        return "0" + _this.refs.arrivalhour.value;
                    }
                    return _this.refs.arrivalhour.value;
                })();
                var arrivalmin = (_this.refs.arrivalmin.value) == -1 ? "-1" : (function () {
                    if (String(_this.refs.arrivalmin.value).length == 1) {
                        return "0" + _this.refs.arrivalmin.value;
                    }
                    return _this.refs.arrivalmin.value;
                })();

                var arrivalfLastValue = arrivaldate + "&" + arrivalhour + "&" + arrivalmin;



                var takeoffdate = $(_this.refs.takeoffdate).val();
                if (takeoffdate) {
                    var ARRtakeofftime = takeoffdate.split('-');
                    var newDate = new Date(parseInt(ARRtakeofftime[0]), parseInt(ARRtakeofftime[1] - 1), parseInt(ARRtakeofftime[2]), 0, 0,0);
                    var year = newDate.getFullYear();
                    var month = String((parseInt(newDate.getMonth()) + 1)).length == 1 ? "0" + String((parseInt(newDate.getMonth()) + 1)) : String((parseInt(newDate.getMonth()) + 1));
                    var date = String(parseInt(newDate.getDate()) + 1).length == 1 ? "0" + String((parseInt(newDate.getDate()))) : String((parseInt(newDate.getDate())));
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
                obj[_this.props.data.elements.arrivaltime.code] = arrivalfLastValue;

                //回复默认值
                if (_this.props.UIType == "reChange") {
                    if (takeoffLastValue == '&-1&-1') {
                        var lastValue = _this.prehandledata(_this.props.beforeRechangeValue);
                        obj[_this.props.data.elements.takeofftime.code] = lastValue[_this.props.data.elements.takeofftime.code];
                    }
                    if (arrivalfLastValue == '&-1&-1') {
                        var lastValue = _this.prehandledata(_this.props.beforeRechangeValue);
                        obj[_this.props.data.elements.arrivaltime.code] = lastValue[_this.props.data.elements.arrivaltime.code];
                    }
                }
                //检测值
                var takeoffArrs = obj[_this.props.data.elements.takeofftime.code].split("&");
                var arrivalArrs = obj[_this.props.data.elements.arrivaltime.code].split("&");
                _this.detectValueState(
                    takeoffArrs[0],
                    takeoffArrs[1],
                    takeoffArrs[2],
                    arrivalArrs[0],
                    arrivalArrs[1],
                    arrivalArrs[2],
                );
                _this.setState(
                    {
                        postValue: obj
                    },

                    function () {
                        _this.upadteToForm(_this.getdUpdateData());

                        //更新模板值
                        var reciveTempValueLastObj = _this.state.postValue;
                        var takeoffArr = (reciveTempValueLastObj[_this.props.data.elements.takeofftime.code]).toString().split('&');
                        var arrivalArr = (reciveTempValueLastObj[_this.props.data.elements.arrivaltime.code]).toString().split('&');
                        var reciveTempValueLast = new Object();
                        reciveTempValueLast[_this.props.suborderid] = new Object();
                        if (reciveTempValueLastObj[_this.props.data.elements.takeofftime.code] == '&-1&-1') {
                            reciveTempValueLast[_this.props.suborderid][_this.props.data.elements.takeofftime.code] = '';
                        }
                        else {
                            reciveTempValueLast[_this.props.suborderid][_this.props.data.elements.takeofftime.code] = takeoffArr[0] + ' ' + (takeoffArr[1] == '-1' ? '' : takeoffArr[1]) + ':' + (takeoffArr[2] == '-1' ? '' : takeoffArr[2]);
                        }
                        if (reciveTempValueLastObj[_this.props.data.elements.arrivaltime.code] == '&-1&-1') {
                            reciveTempValueLast[_this.props.suborderid][_this.props.data.elements.arrivaltime.code] = '';
                        }
                        else {
                            reciveTempValueLast[_this.props.suborderid][_this.props.data.elements.arrivaltime.code] = arrivalArr[0] + ' ' + (arrivalArr[1] == '-1' ? '' : arrivalArr[1]) + ':' + (arrivalArr[2] == '-1' ? '' : arrivalArr[2]);
                        }
                        _this.props.reciveTempValue(reciveTempValueLast);
                        //更新粘贴板的值
                        var copyObj ={};
                        copyObj[_this.props.data.elements.takeofftime.code]={
                            title:_this.props.data.elements.takeofftime.title,
                            str: reciveTempValueLast[_this.props.suborderid][_this.props.data.elements.takeofftime.code]
                        }
                        copyObj[_this.props.data.elements.arrivaltime.code]={
                            title:_this.props.data.elements.arrivaltime.title,
                            str: reciveTempValueLast[_this.props.suborderid][_this.props.data.elements.arrivaltime.code]
                        }
                        _this.updateCopyObj(copyObj);


                        //更新系统字段
                        var formFields = new Object();
                        formFields[_this.props.data.elements.takeofftime.code] = takeoffArr[0];
                        formFields[_this.props.data.elements.arrivaltime.code] = arrivalArr[0];
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
                $(_this.refs.arrivaldatepicker).find('input').blur();
            });
        };

    },

    mixins: [MXINS_UPDATE_DATA],
    detectValueState: function (takeoffdate, takeoffhour, takeoffmin, arrivaldate, arrivalhour, arrivalmin) {


        var isTakeoffValueUseAble = 1;
        var isTakeoffNotEmpty = 1;
        var isArrivalValueUseAble = 1;
        var isArrivalNotEmpty = 1;
        var takeoffclassname = "";
        var takeofftips = "";
        var arrivalclassname = "";
        var arrivaltips = "";
        if (
            takeoffdate &&
            takeoffhour != -1 &&
            takeoffmin != -1 &&
            arrivaldate &&
            arrivalhour != -1 &&
            arrivalmin != -1
        ) {

            var ARRtakeofftime = takeoffdate.split('-');
            var ARRarrivaltime = arrivaldate.split('-');
            var datetakeofftime = new Date(parseInt(ARRtakeofftime[0]), parseInt(ARRtakeofftime[1] - 1), parseInt(ARRtakeofftime[2]), takeoffhour, takeoffmin);
            var datearrivaltime = new Date(parseInt(ARRarrivaltime[0]), parseInt(ARRarrivaltime[1] - 1), parseInt(ARRarrivaltime[2]), arrivalhour, arrivalmin);

            var takeofftimetimestamp = datetakeofftime.valueOf();
            var arrivaltimestamp = datearrivaltime.valueOf();

            if (arrivaltimestamp < takeofftimetimestamp) {

                isTakeoffValueUseAble = 0;
                isTakeoffNotEmpty = 1;
                isArrivalValueUseAble = 0;
                isArrivalNotEmpty = 1;
                takeoffclassname = "help-inline tips";
                takeofftips = this.props.data.elements.takeofftime.title + "不应该晚于" + this.props.data.elements.arrivaltime.title + "!";
                arrivalclassname = "help-inline tips";
                arrivaltips = this.props.data.elements.arrivaltime.title + "不应该早于" + this.props.data.elements.takeofftime.title + "!";

                //变更变更的更正为单个提示语
                if (this.props.UIType == "reChange") {
                    var lastValue = this.prehandledata(this.props.beforeRechangeValue);
                    if (takeoffdate + '&' + takeoffhour + '&' + takeoffmin == lastValue[this.props.data.elements.takeofftime.code]) {
                        takeoffclassname = "help-inline";
                        takeofftips = this.props.data.elements.takeofftime.tips;
                    }
                    if (arrivaldate + '&' + arrivalhour + '&' + arrivalmin == lastValue[this.props.data.elements.arrivaltime.code]) {

                        arrivalclassname = "help-inline";
                        arrivaltips = this.props.data.elements.arrivaltime.tips;
                    }
                }
            }
            else {
                isTakeoffValueUseAble = 1;
                isTakeoffNotEmpty = 1;
                isArrivalValueUseAble = 1;
                isArrivalNotEmpty = 1;
                takeoffclassname = "help-inline";
                takeofftips = this.props.data.elements.takeofftime.tips;
                arrivalclassname = "help-inline";
                arrivaltips = this.props.data.elements.arrivaltime.tips;
            }
        }
        else {
            if (
                !takeoffdate ||
                takeoffhour == -1 ||
                takeoffmin == -1
            ) {
                takeoffclassname = "help-inline tips";
                takeofftips = "请填写完整的" + this.props.data.elements.takeofftime.title;
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

            //到达
            if (
                !arrivaldate ||
                arrivalhour == -1 ||
                arrivalmin == -1
            ) {
                arrivalclassname = "help-inline tips";
                arrivaltips = "请填写完整的" + this.props.data.elements.arrivaltime.title;
                if (this.props.UIType == "reChange") {
                    var lastValue = this.prehandledata(this.props.beforeRechangeValue);
                    if (arrivaldate + ' ' + arrivalhour + ':' + arrivalmin == lastValue[this.props.data.elements.arrivaltime.code]) {
                        arrivalclassname = "help-inline";
                        arrivaltips = this.props.data.elements.arrivaltime.tips;
                    }
                }
                isArrivalValueUseAble = 0;
                isArrivalNotEmpty = 0;
            }
            else {
                arrivalclassname = "help-inline";
                arrivaltips = this.props.data.elements.arrivaltime.tips;
                isArrivalValueUseAble = 1;
                isArrivalNotEmpty = 1;
            }
        }

        if (this.props.limit === false || (this.props.data.mandatory == "2")) {//unlimited
            takeoffclassname = "help-inline";
            takeofftips = this.props.data.elements.takeofftime.tips;
            arrivalclassname = "help-inline";
            arrivaltips = this.props.data.elements.arrivaltime.tips;
        }
        this.setState({
            'takeoffclassname': takeoffclassname,
            'takeofftips': takeofftips,
            'arrivalclassname': arrivalclassname,
            'arrivaltips': arrivaltips
        });

        var stateObj = new Object();
        stateObj[this.props.suborderid] = new Object();

        stateObj[this.props.suborderid][this.props.data.elements.takeofftime.code] = new Object();
        stateObj[this.props.suborderid][this.props.data.elements.takeofftime.code].isValueUseAble = isTakeoffValueUseAble;
        stateObj[this.props.suborderid][this.props.data.elements.takeofftime.code].isValueNotEmpty = isTakeoffNotEmpty;
        stateObj[this.props.suborderid][this.props.data.elements.takeofftime.code].isValueMandatory = this.props.data.mandatory;

        stateObj[this.props.suborderid][this.props.data.elements.arrivaltime.code] = new Object();
        stateObj[this.props.suborderid][this.props.data.elements.arrivaltime.code].isValueUseAble = isArrivalValueUseAble;
        stateObj[this.props.suborderid][this.props.data.elements.arrivaltime.code].isValueNotEmpty = isArrivalNotEmpty;
        stateObj[this.props.suborderid][this.props.data.elements.arrivaltime.code].isValueMandatory = this.props.data.mandatory;

        this.props.reciveValueState(stateObj);


    },
    detectValueWhenMounted: function () {
        if (this.getDefaultValue() == "emptyrow") {
            this.setState(
                {
                    takeoffclassname: "help-inline",
                    takeofftips: this.props.data.elements.takeofftime.tips,
                    arrivalclassname: "help-inline",
                    arrivaltips: this.props.data.elements.arrivaltime.tips
                }
            );
            var stateObj = new Object();
            stateObj[this.props.suborderid] = new Object();

            stateObj[this.props.suborderid][this.props.data.elements.takeofftime.code] = new Object();
            stateObj[this.props.suborderid][this.props.data.elements.takeofftime.code].isValueUseAble = 0;
            stateObj[this.props.suborderid][this.props.data.elements.takeofftime.code].isValueNotEmpty = 0;
            stateObj[this.props.suborderid][this.props.data.elements.takeofftime.code].isValueMandatory = this.props.data.mandatory;

            stateObj[this.props.suborderid][this.props.data.elements.arrivaltime.code] = new Object();
            stateObj[this.props.suborderid][this.props.data.elements.arrivaltime.code].isValueUseAble = 0;
            stateObj[this.props.suborderid][this.props.data.elements.arrivaltime.code].isValueNotEmpty = 0;
            stateObj[this.props.suborderid][this.props.data.elements.arrivaltime.code].isValueMandatory = this.props.data.mandatory;


            this.props.reciveValueState(stateObj);
        }
        else {
            var takeofftime = this.props.data.elements.takeofftime.code;
            var arrivaltime = this.props.data.elements.arrivaltime.code;

            var lastValueArrT = this.state.postValue[takeofftime].toString().split("&");
            var lastValueDateT = lastValueArrT[0];
            var lastValueHourT = lastValueArrT[1];
            var lastValueMinT = lastValueArrT[2];

            var lastValueArrA = this.state.postValue[arrivaltime].toString().split("&");
            var lastValueDateA = lastValueArrA[0];
            var lastValueHourA = lastValueArrA[1];
            var lastValueMinA = lastValueArrA[2];
            this.detectValueState(
                lastValueDateT,
                lastValueHourT,
                lastValueMinT,
                lastValueDateA,
                lastValueHourA,
                lastValueMinA
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
            <select ref={refname} name="mins" defaultValue={value} className="form-control input-inline input80 input-sm" onChange={this.handlerChange} >
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

        //到达
        var arrivaldate = $(_this.refs.arrivaldate).val();
        if (arrivaldate) {
            var ARRarrivaltime = arrivaldate.split('-');
            var newDateR = new Date(parseInt(ARRarrivaltime[0]), parseInt(ARRarrivaltime[1] - 1), parseInt(ARRarrivaltime[2]), 0, 0,0);
            var year = newDateR.getFullYear();
            var month = String((parseInt(newDateR.getMonth()) + 1)).length == 1 ? "0" + String((parseInt(newDateR.getMonth()) + 1)) : String((parseInt(newDateR.getMonth()) + 1));
            var date = String(parseInt(newDateR.getDate()) + 1).length == 1 ? "0" + String((parseInt(newDateR.getDate()))) : String((parseInt(newDateR.getDate())));
            arrivaldate = year + '-' + month + '-' + date;
        }
        else {
            arrivaldate = ''
        }

        var arrivalhour = (_this.refs.arrivalhour.value) == -1 ? "-1" : (function () {
            if (String(_this.refs.arrivalhour.value).length == 1) {
                return "0" + _this.refs.arrivalhour.value;
            }
            return _this.refs.arrivalhour.value;
        })();
        var arrivalmin = (_this.refs.arrivalmin.value) == -1 ? "-1" : (function () {
            if (String(_this.refs.arrivalmin.value).length == 1) {
                return "0" + _this.refs.arrivalmin.value;
            }
            return _this.refs.arrivalmin.value;
        })();
        var arrivalfLastValue = arrivaldate + "&" + arrivalhour + "&" + arrivalmin;







        var obj = {};
        obj[this.props.data.elements.takeofftime.code] = takeoffLastValue;
        obj[this.props.data.elements.arrivaltime.code] = arrivalfLastValue;


        //回复默认值
        if (_this.props.UIType == "reChange") {
            if (takeoffLastValue == '&-1&-1') {
                var lastValue = this.prehandledata(this.props.beforeRechangeValue);
                obj[this.props.data.elements.takeofftime.code] = lastValue[this.props.data.elements.takeofftime.code];
            }
            if (arrivalfLastValue == '&-1&-1') {
                var lastValue = this.prehandledata(this.props.beforeRechangeValue);
                obj[this.props.data.elements.arrivaltime.code] = lastValue[this.props.data.elements.arrivaltime.code];
            }
        }
        //检测值
        var takeoffArrs = obj[this.props.data.elements.takeofftime.code].split("&");
        var arrivalArrs = obj[this.props.data.elements.arrivaltime.code].split("&");
        this.detectValueState(
            takeoffArrs[0],
            takeoffArrs[1],
            takeoffArrs[2],
            arrivalArrs[0],
            arrivalArrs[1],
            arrivalArrs[2],
        );
        this.setState(
            {
                postValue: obj
            },

            function () {
                this.upadteToForm(this.getdUpdateData());



                var reciveTempValueLastObj = _this.state.postValue;
                var takeoffArr = (reciveTempValueLastObj[_this.props.data.elements.takeofftime.code]).toString().split('&');
                var arrivalArr = (reciveTempValueLastObj[_this.props.data.elements.arrivaltime.code]).toString().split('&');
                var reciveTempValueLast = new Object();
                reciveTempValueLast[_this.props.suborderid] = new Object();
                if (reciveTempValueLastObj[_this.props.data.elements.takeofftime.code] == '&-1&-1') {
                    reciveTempValueLast[_this.props.suborderid][_this.props.data.elements.takeofftime.code] = '';
                }
                else {
                    reciveTempValueLast[_this.props.suborderid][_this.props.data.elements.takeofftime.code] = takeoffArr[0] + ' ' + (takeoffArr[1] == '-1' ? '' : takeoffArr[1]) + ':' + (takeoffArr[2] == '-1' ? '' : takeoffArr[2]);
                }
                if (reciveTempValueLastObj[_this.props.data.elements.arrivaltime.code] == '&-1&-1') {
                    reciveTempValueLast[_this.props.suborderid][_this.props.data.elements.arrivaltime.code] = '';
                }
                else {
                    reciveTempValueLast[_this.props.suborderid][_this.props.data.elements.arrivaltime.code] = arrivalArr[0] + ' ' + (arrivalArr[1] == '-1' ? '' : arrivalArr[1]) + ':' + (arrivalArr[2] == '-1' ? '' : arrivalArr[2]);
                }
                _this.props.reciveTempValue(reciveTempValueLast);
                 //更新粘贴板的值
                var copyObj ={};
                copyObj[_this.props.data.elements.takeofftime.code]={
                    title:_this.props.data.elements.takeofftime.title,
                    str: reciveTempValueLast[_this.props.suborderid][_this.props.data.elements.takeofftime.code]
                }
                copyObj[_this.props.data.elements.arrivaltime.code]={
                    title:_this.props.data.elements.arrivaltime.title,
                    str: reciveTempValueLast[_this.props.suborderid][_this.props.data.elements.arrivaltime.code]
                }
                _this.updateCopyObj(copyObj);

            }
        );

    },
    render: function () {
        var suborderid = this.props.suborderid;
        var componentid = this.props.componentid;
        var takeofftime = this.props.data.elements.takeofftime.code;
        var arrivaltime = this.props.data.elements.arrivaltime.code;



        if (this.props.UIType == "edit") {

            var lastValueArrT = this.state.postValue[takeofftime].toString().split("&");
            var lastValueDateT = lastValueArrT[0];
            var lastValueHourT = parseInt(lastValueArrT[1]);
            var lastValueMinT = parseInt(lastValueArrT[2]);

            var lastValueArrA = this.state.postValue[arrivaltime].toString().split("&");
            var lastValueDateA = lastValueArrA[0];
            var lastValueHourA = parseInt(lastValueArrA[1]);
            var lastValueMinA = parseInt(lastValueArrA[2]);

            return (
                <div>
                    <div className="form-group" data-suborderid={suborderid} data-componentid={componentid} data-segmentcode={this.props.data.elements.takeofftime.code} >
                        <Lable params={{ isMust: this.props.data.mandatory == 1 ? true : false, title: this.props.data.elements.takeofftime.title }} ></Lable>
                        <div className="col-md-10">
                            <div ref="takeoffdatepicker" className="input-group input-medium  input-group-sm ddDatePicker date date-picker  " >
                                <input readOnly defaultValue={lastValueDateT} ref="takeoffdate" type="text" className="form-control input-sm readOnlyDatePicker " placeholder="" onChange={this.handlerChange} />
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

                    <div className="form-group" data-suborderid={suborderid} data-componentid={componentid} data-segmentcode={this.props.data.elements.arrivaltime.code} >
                        <Lable params={{ isMust: this.props.data.mandatory == 1 ? true : false, title: this.props.data.elements.arrivaltime.title }} ></Lable>
                        <div className="col-md-10">
                            <div ref="arrivaldatepicker" className="input-group input-medium  input-group-sm ddDatePicker date date-picker  " >
                                <input readOnly defaultValue={lastValueDateA} ref="arrivaldate" type="text" className="form-control input-sm readOnlyDatePicker" placeholder="" onChange={this.handlerChange} />
                                <span className="input-group-btn">
                                    <button className="btn default" type="button">
                                        <i className="fa fa-calendar"></i>
                                    </button>
                                </span>
                            </div>


                            <div className="verticalBottom" style={{ 'display': "inline-block;height:30px;white-space:nowrap;width:195px;margin-top:5px;margin-right:5px" }}>
                                {this.hoursMap("arrivalhour", lastValueHourA)}
                                {this.minutesMap("arrivalmin", lastValueMinA)}
                            </div>
                            <span className={this.state.arrivalclassname}>{this.state.arrivaltips}</span>
                        </div>
                    </div>
                </div>
            )
        }
        else if (this.props.UIType == "reChange") {
            var lastValue = this.prehandledata(this.props.beforeRechangeValue);

            var takeoffChange = 0;
            var arrivalChange = 0;

            if (lastValue == this.state.postValue) {

                var lastValueDateT = '';
                var lastValueHourT = -1;
                var lastValueMinT = -1;

                var lastValueDateA = '';
                var lastValueHourA = -1;
                var lastValueMinA = -1;

                takeoffChange = 0;
                arrivalChange = 0;

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
                    lastValue[arrivaltime] == this.state.postValue[arrivaltime] ||
                    (this.state.postValue[arrivaltime] == "&-1&-1")
                ) {



                    var lastValueDateA = '';
                    var lastValueHourA = -1;
                    var lastValueMinA = -1;

                    arrivalChange = 0;
                }
                else {
                    var lastValueArrA = this.state.postValue[arrivaltime].toString().split("&");
                    var lastValueDateA = lastValueArrA[0];
                    var lastValueHourA = parseInt(lastValueArrA[1]);
                    var lastValueMinA = parseInt(lastValueArrA[2]);

                    arrivalChange = 1;
                }




            }
            var changeObj = new Object();
            changeObj[this.props.suborderid] = new Object();

            changeObj[this.props.suborderid][this.props.data.elements.takeofftime.code] = new Object();
            changeObj[this.props.suborderid][this.props.data.elements.takeofftime.code].isValueChange = takeoffChange;

            changeObj[this.props.suborderid][this.props.data.elements.arrivaltime.code] = new Object();
            changeObj[this.props.suborderid][this.props.data.elements.arrivaltime.code].isValueChange = arrivalChange;
            this.props.reciveValueChange(changeObj);


            //处理展示原始值
            var takeoffOld = this.getDefaultValue()[this.props.data.elements.takeofftime.code].split("&");
            takeoffOld = takeoffOld[0] + ' ' + takeoffOld[1] + ':' + takeoffOld[2];
            var arrivalOld = this.getDefaultValue()[this.props.data.elements.arrivaltime.code].split("&");
            arrivalOld = arrivalOld[0] + ' ' + arrivalOld[1] + ':' + arrivalOld[2];
            return (
                <tbody>
                    <tr>
                        <td>
                            <LableRechange params={{ isMust: this.props.data.mandatory == 1 ? true : false, title: this.props.data.elements.takeofftime.title }} ></LableRechange>
                        </td>
                        <td>{takeoffOld}</td>
                        <td>
                            <div ref="takeoffdatepicker" className="input-group input-medium  input-group-sm ddDatePicker date date-picker  " >
                                <input readOnly defaultValue={lastValueDateT} ref="takeoffdate" type="text" className="form-control input-sm readOnlyDatePicker " placeholder="" onChange={this.handlerChange} />
                                <span className="input-group-btn">
                                    <button className="btn default" type="button">
                                        <i className="fa fa-calendar"></i>
                                    </button>
                                </span>
                            </div>

                            <div className="verticalBottom" style={{ display: "inline-block;height:30px;white-space:nowrap;width:195px;margin-top:5px" }}>
                                {this.hoursMap("takeoffhour", lastValueHourT)}
                                {this.minutesMap("takeoffmin", lastValueMinT)}


                            </div>
                            <div><span className={this.state.takeoffclassname}>{this.state.takeofftips}</span></div>
                        </td>
                    </tr>
                    <tr>
                        <td>
                              <LableRechange params={{ isMust: this.props.data.mandatory == 1 ? true : false, title: this.props.data.elements.arrivaltime.title }} ></LableRechange>
                        </td>
                        <td>{arrivalOld}</td>
                        <td>
                            <div ref="arrivaldatepicker" className="input-group input-medium  input-group-sm ddDatePicker date date-picker  " >
                                <input readOnly defaultValue={lastValueDateA} ref="arrivaldate" type="text" className="form-control input-sm readOnlyDatePicker " placeholder="" onChange={this.handlerChange} />
                                <span className="input-group-btn">
                                    <button className="btn default" type="button">
                                        <i className="fa fa-calendar"></i>
                                    </button>
                                </span>
                            </div>
                            <div style={{ display: "inline-block;height:30px;white-space:nowrap;width:195px;margin-top:5px" }}>
                                {this.hoursMap("arrivalhour", lastValueHourA)}
                                {this.minutesMap("arrivalmin", lastValueMinA)}
                            </div>
                            <div> <span className={this.state.arrivalclassname}>{this.state.arrivaltips}</span></div>

                        </td>
                    </tr>
                </tbody>
            )
        }

    }
})
