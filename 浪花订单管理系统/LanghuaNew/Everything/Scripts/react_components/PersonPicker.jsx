var PersonPickerRow = React.createClass({
    prehandledata: function (data) {
        if (data == 'emptyrow') {
            var obj = {};
            obj[this.props.data.elements.single.code] = {};
            return (obj);
        }

        var segmentcode = this.props.data.elements.single.code;
        var obj = {}
        for (var i in data[segmentcode]) {
            obj[data[segmentcode][i]["personID"]] = data[segmentcode][i];
        }
        var object = {};
        object[segmentcode] = obj

        return object;
    },
    prehandleupdatedata: function (data) {

        var segmentcode = this.props.data.elements.single.code;
        var thisdata = [];
        for (var i in data[segmentcode]) {
            thisdata.push(data[segmentcode][i])
        }
        var obj = {};
        obj[segmentcode] = thisdata;

        this.setState({
            selectednum: thisdata.length
        })
        return obj;
    },
    arr2Obj: function (arr) {
        var obj = {};
        for (var i in arr) {
            obj[arr[i].personID] = arr[i];
        }
        return obj;
    },
    makeArray: function (data) {
        var thisdata = [];
        for (var i in data) {
            thisdata.push(data[i])
        }
        return thisdata;
    },
    mixins: [MXINS_UPDATE_DATA],

    detectRechangeState: function (objOldValue, objReChangeValue) {
        var oldInRechange = 0;
        var reChangeTotal = 0;
        var reChangeInOld = 0;
        var oldTotal = 0;
        var i, j;
        var valueChange = 0;
        for (i in objReChangeValue) {
            if (i in objOldValue) {
                oldInRechange++;
            }
            reChangeTotal++;
        }
        for (j in objOldValue) {
            if (j in objReChangeValue) {
                reChangeInOld++;
            }
            oldTotal++;
        }
        if ((oldInRechange == reChangeTotal) && (reChangeTotal == oldTotal)) {
            outLoop:
            for (i in objReChangeValue) {
                for (j in objReChangeValue[i]) {
                    if (j in objOldValue[i]) {
                        if (objOldValue[i][j] != objReChangeValue[i][j]) {
                            valueChange = 1;
                            break outLoop;
                        }
                    }
                }
            }
        }
        else {//数量或id不一致
            valueChange = 1;
        }
        var changeObj = new Object();
        changeObj[this.props.suborderid] = new Object();
        changeObj[this.props.suborderid][this.props.data.elements.single.code] = new Object();
        changeObj[this.props.suborderid][this.props.data.elements.single.code].isValueChange = valueChange;
        this.props.reciveValueChange(changeObj);
    },
    detectValueState: function (value) {
        var isValueUseAble = 1;
        var isValueNotEmpty = 1;
        var classname = "";
        var tips = "";
        var maxPeopleNum = parseInt(this.state.maxPeopleNum);
        if (value.length == 0) {
            classname = "help-inline tips";
            tips = "请选择" + this.props.data.elements.single.title;
            isValueUseAble = 0;
            isValueNotEmpty = 0;
            if (this.props.UIType == "reChange") {
                if (this.props.data.mandatory == 2) {
                    classname = "help-inline";
                    tips = this.props.data.elements.single.tips;
                }
            }
        }
        else if (value.length > maxPeopleNum) {
            classname = "help-inline tips";
            tips = "最多可以选择" + maxPeopleNum + "人";
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
            this.detectValueState(this.makeArray(this.state.postValue[this.props.data.elements.single.code]));
        }
    },

    getInitialState: function () {
        return {
            maxPeopleNum: this.props.basePeopleNum
        }
    },
    componentDidMount: function () {
        var _this = this;
        {
            $(this.refs.forceUpdate).bind("update", function (e, num) {
                _this.setState({
                    maxPeopleNum: num
                }, function () {
                    this.detectValueState(this.makeArray(this.state.postValue[this.props.data.elements.single.code]));
                })
            })
        }

        //   更新一次人数
        var suborderid = this.props.suborderid;
        var componentid = this.props.componentid;
        var obj = {}
        obj[suborderid] = {};
        obj[suborderid][componentid] = this.state.postValue;
        this.props.recievePeson(obj);
        this.detectValueWhenMounted();

        //更新tempvalue
        var obj = {};
        obj[this.props.suborderid] = new Object();
        personSelected = this.state.postValue[this.props.data.elements.single.code];
        var fullInfo = new Object();
        fullInfo.type = 'PersonPicker';
        fullInfo.value = new Array();
        for (var i in personSelected) {
            fullInfo.value.push(personSelected[i]);
        }
        obj[this.props.suborderid][this.props.data.elements.single.code] = fullInfo;
        this.props.reciveTempValue(obj);
        //检测变更
        if (this.props.UIType == "reChange") {
            var oldValue = _this.arr2Obj(this.props.beforeRechangeValue[this.props.data.elements.single.code]);
            var reChange = this.state.postValue[this.props.data.elements.single.code];
            _this.detectRechangeState(oldValue, reChange);
        }

        //更新粘贴板内容
        var copyObj = {};
        var thisCopyStr = "";
        for (var x in personSelected) {
            thisCopyStr += personSelected[x].TravellerName + "，";
        }
        copyObj[this.props.data.elements.single.code] = {
            title: this.props.data.elements.single.title,
            str: thisCopyStr
        }
        this.updateCopyObj(copyObj);


        //绑定事件
        var t = (ReactDOM.findDOMNode(this));
        $(t).bind('addone', function (e, data) {
            var state = _this.state.postValue[_this.props.data.elements.single.code];
            var maxPeopleNum = _this.props.basePeopleNum;

            if (_this.props.UIType == "reChange") {//请求变更，点了选择按钮就表示重新选择人
                if (false) {
                    var changeObj = new Object();
                    changeObj[_this.props.suborderid] = new Object();
                    changeObj[_this.props.suborderid][_this.props.data.elements.single.code] = new Object();
                    changeObj[_this.props.suborderid][_this.props.data.elements.single.code].isValueChange = 1;
                    _this.props.reciveValueChange(changeObj);
                }
                var oldValuex = _this.arr2Obj(_this.props.beforeRechangeValue[_this.props.data.elements.single.code]);
                _this.detectRechangeState(oldValuex, _this.arr2Obj(data));
            }



            var stateobj = {};
            for (var i in data) {
                stateobj[data[i].personID] = data[i];
            }
            var obj = new Object();

            obj[_this.props.data.elements.single.code] = stateobj;

            _this.setState(
                {
                    postValue: obj,

                },
                function () {
                    this.upadteToForm(this.getdUpdateData());

                    this.detectValueState(this.makeArray(this.state.postValue[this.props.data.elements.single.code]));

                    var suborderid = this.props.suborderid;
                    var componentid = this.props.componentid;
                    var obj = {}
                    obj[suborderid] = {};
                    obj[suborderid][componentid] = this.state.postValue;
                    this.props.recievePeson(obj);



                    //更新tempvalue
                    var obj = {};
                    obj[this.props.suborderid] = new Object();
                    personSelected = this.state.postValue[this.props.data.elements.single.code];

                    var fullInfo = new Object();
                    fullInfo.type = 'PersonPicker';
                    fullInfo.value = new Array();
                    for (var i in personSelected) {
                        fullInfo.value.push(personSelected[i]);
                    }
                    obj[this.props.suborderid][this.props.data.elements.single.code] = fullInfo;
                    this.props.reciveTempValue(obj);


                    //更新粘贴板内容
                    var copyObj = {};
                    var thisCopyStr = "";
                    for (var x in personSelected) {
                        thisCopyStr += personSelected[x].TravellerName + "，";
                    }
                    copyObj[this.props.data.elements.single.code] = {
                        title: this.props.data.elements.single.title,
                        str: thisCopyStr
                    }
                    this.updateCopyObj(copyObj);

                }
            )
        }).bind('delete', function (e, arr) {
            var selectedARR = _this.state.postValue[_this.props.data.elements.single.code];
            for (var i in arr) {
                if (arr[i]['TravellerID'] in selectedARR) {
                    delete selectedARR[arr[i]['TravellerID']];
                }
            }
            var arrx = new Array();
            for (var j in selectedARR) {
                arrx.push(selectedARR[j]);
            }


            $(t).trigger("addone", [arrx]);
        })

    },
    click: function (e) {
        var segmentcode = this.props.data.elements.single.code;

        var id = jQuery(e.target).parent().attr('data-index');
        var _state = this.state;
        var t = delete (_state['postValue'][segmentcode][id]);


        this.setState(
            _state,
            function () {
                this.upadteToForm(this.getdUpdateData());

                var suborderid = this.props.suborderid;
                var componentid = this.props.componentid;
                var obj = {}
                obj[suborderid] = {};
                obj[suborderid][componentid] = this.state.postValue;
                this.props.recievePeson(obj);

                this.detectValueState(this.makeArray(this.state.postValue[this.props.data.elements.single.code]));

                //更新tempvalue
                var obj = {};
                obj[this.props.suborderid] = new Object();
                personSelected = this.state.postValue[this.props.data.elements.single.code];

                var fullInfo = new Object();
                fullInfo.type = 'PersonPicker';
                fullInfo.value = new Array();
                for (var i in personSelected) {
                    fullInfo.value.push(personSelected[i]);
                }
                obj[this.props.suborderid][this.props.data.elements.single.code] = fullInfo;
                this.props.reciveTempValue(obj);

                //更新粘贴板内容
                var copyObj = {};
                var thisCopyStr = "";
                for (var x in personSelected) {
                    thisCopyStr += personSelected[x].TravellerName + "，";
                }
                copyObj[this.props.data.elements.single.code] = {
                    title: this.props.data.elements.single.title,
                    str: thisCopyStr
                }
                this.updateCopyObj(copyObj);
            }
        );
    },
    renderSelected: function (arr) {
        var _this = this;

        var r = [];
        for (var i in arr) {
            var birthday = arr[i].Birthday;


            {
                var arrBirthday = birthday.split("T")[0].split("-");
                var birthday = new Date(arrBirthday[0], arrBirthday[1] - 1, arrBirthday[2], 0, 0, 0);
                var today = new Date();
                var offYear = today.getFullYear() - birthday.getFullYear();
                var monBirthday = birthday.getMonth();
                var dateBirthday = birthday.getDate();
                var monToday = today.getMonth();
                var dateToday = today.getDate();
                var havedBirthday = false;
                {
                    if (parseInt(monToday) < parseInt(monBirthday)) {//未过
                    }
                    else if (parseInt(monToday) == parseInt(monBirthday)) {
                        if (parseInt(dateToday) >= parseInt(dateBirthday)) {
                            havedBirthday = true;
                        }
                    }
                    else {//已过生日
                        havedBirthday = true;
                    }
                }
                var age = parseInt(offYear);
                if (!havedBirthday) {
                    age -= 1;
                }
                var classnames = "one";
                var ageText = age;
                var ageClass = "age ";

                if (_this.props.isForCusClient === true) {
                    ageClass +=" hidden";
                }
                else {
                    if (arr[i].TravellerSex == 1) {
                        classnames += " female";
                    }
                    else {
                        classnames += " male";
                    }
                    if (age >= 60) {
                        ageClass += " highlight age-gt-60";
                    } else if (age >= 12 && age < 60) {
                        ageClass += " highlight  adult";
                    } else if (age >= 4 && age < 12) {
                        ageClass += " highlight child";
                    } else {
                        ageClass += " highlight age-lt-4";
                    }
                }
            }
            r.push(
                <span data-index={i} id={arr[i].personID} key={arr[i].personID} className={classnames}>
                    <span className="altname" >{arr[i].text}</span>
                    <span className={ageClass} >{ageText}</span>
                    <span className="delete" title="取消选择" onClick={this.click}>×</span>
                </span>
            )
        }
        return (
            <span className="formselecttelist">
                {r}
            </span>
        )

    },

    render: function () {
        var suborderid = this.props.suborderid;
        var componentid = this.props.componentid;
        var segmentcode = this.props.data.elements.single.code;
        var ageRange = new Object();
        if ("ageMinChecked" in this.props.data) {
            if (this.props.data.ageMinChecked) {
                ageRange.ageMin = this.props.data.ageMin;

            }
            else {
                ageRange.ageMin = -1;
            }
            if (this.props.data.ageMaxChecked) {
                ageRange.ageMax = this.props.data.ageMax;
            }
            else {
                ageRange.ageMax = -1;
            }
        }
        else {
            ageRange.ageMin = -1;
            ageRange.ageMax = -1;
        }
        if (this.props.UIType == "edit") {
            return (

                <div ref="forceUpdate" className="form-group addpersons forceUpdate-peoplenum" data-suborderid={suborderid} data-componentid={componentid} data-segmentcode={segmentcode} >
                    <Lable params={{
                        isMust: this.props.data.mandatory == 1 ? true : false,
                        title: this.props.data.elements.single.title,
                        reactType: 'personpicker',
                        selectednum: this.state.selectednum
                    }}
                        ></Lable>
                    <div className="col-md-10">
                        <span className="formbuttonwrapper">
                            <a id="addpersons"
                                data-needextrainfo={this.props.data.needExtraInfo}
                                data-selected={JSON.stringify(this.state.postValue)}
                                data-agerange={JSON.stringify(ageRange)}
                                className="btn btn-sm btn-primary button70"
                                data-toggle="modal"
                                data-target="#personlist" >
                                选择</a>
                        </span>

                        {
                            this.renderSelected(this.state.postValue[segmentcode])
                        }
                        <span className={this.state.classname}>{this.state.tips}</span>
                    </div>
                </div>
            )
        }
        else if (this.props.UIType == "reChange") {
            var lastValue = this.prehandledata(this.props.beforeRechangeValue)[this.props.data.elements.single.code];
            var postValue = this.prehandledata(this.state.postValue)[this.props.data.elements.single.code];
            var lastInPost = 0;
            var lastTotal = 0;
            var postInLast = 0;
            var postTotal = 0;

            // for(var i in lastValue){
            //     if(i in postValue){
            //         lastInPost++;
            //     }
            //     lastTotal++;
            // }
            // for(var j in postValue){
            //      if(j in lastValue){
            //         postInLast++;
            //     }
            //     postTotal++;
            // }

            // if((lastInPost==lastTotal) &&(lastTotal==postTotal)){
            //     var valueChange = 0;
            // }
            // else{
            //      var valueChange = 1;
            // }
            // var changeObj = new Object();
            // changeObj[this.props.suborderid]=new Object();

            // changeObj[this.props.suborderid][this.props.data.elements.single.code] =  new Object();
            // changeObj[this.props.suborderid][this.props.data.elements.single.code].isValueChange = valueChange;


            // this.props.reciveValueChange(changeObj);
            return (

                <tbody ref="forceUpdate" className="forceUpdate-peoplenum">
                    <tr className="addpersons">
                        <td>
                             <LableRechange params={
                                 {
                                    isMust: this.props.data.mandatory == 1 ? true : false,
                                    title: this.props.data.elements.single.title,
                                }}
                            ></LableRechange>
                        </td>
                        <td>{this.getDefaultValue()[this.props.data.elements.single.code].length}人</td>
                        <td>
                            <span className="formbuttonwrapper">
                                <a id="addpersons" data-agerange={JSON.stringify(ageRange)} data-needextrainfo={this.props.data.needExtraInfo} data-selected={JSON.stringify(this.state.postValue)} className="btn btn-sm btn-primary button70"  > 选择</a>
                            </span>
                            <div><span className={this.state.classname}>{this.state.tips}</span></div>
                        </td>
                    </tr>
                </tbody>
            )
        }

    }
})
