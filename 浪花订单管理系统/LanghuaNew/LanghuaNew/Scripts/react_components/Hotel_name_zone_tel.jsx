// 酒店名字-区域控件
var Hotel_name_zone_telRow = React.createClass({
    prehandledata: function (data) {
        if (data == "emptyrow") {
            var elements = this.props.data.elements;
            var obj = new Object();
            obj[elements.area.code] = {
                value: -1,
                hotelAreaName: '',
                AreaEnName: '',
            };
            obj[elements.name.code] = "";
            obj[elements.tel.code] = "";
            if ('address' in elements) {
                obj[elements.address.code] = "";
            }
            return (obj);
        }
        return data;
    },
    prehandleupdatedata: function (data) {
        return data;
    },
    mixins: [MXINS_UPDATE_DATA],
    isPropertyHotelName: function (str) {
        var regu = "^[^\u4e00-\u9fa5]+$";
        var re = new RegExp(regu);
        if (re.test(str)) {
            return true;
        }
        else {
            return false;
        }
    },
    isPropertyTel: function (tel) {
        var regu = "^[0-9-+\(\)（） ]+$";
        var re = new RegExp(regu);
        if (re.test(tel)) {
            return true;
        }
        else {
            return false;
        }
    },
    detectValueState: function (name, tel, zoneid, address) {
        var isNameValueUseAble = 1;
        var isNameValueNotEmpty = 1;
        var isTelValueUseAble = 1;
        var isTelValueNotEmpty = 1;
        var isAreaValueUseAble = 1;
        var isAreaValueNotEmpty = 1;
        var isAddressValueUseAble = 1;
        var isAddressValueNotEmpty = 1;
        var nametips = "";
        var nameclassname = "";
        var teltips = "";
        var telclassname = "";
        var zonetips = "";
        var zoneclassname = "";
        var addresstips = "";
        var addressclassname = "";
        if (!(name)) {
            nameclassname = "help-inline tips";
            nametips = "请填写正确的" + this.props.data.elements.name.title + "不要中文！";
            var isNameValueUseAble = 0;
            var isNameValueNotEmpty = 0;

        }
        else if (!(this.isPropertyHotelName(name))) {
            nameclassname = "help-inline tips";
            nametips = "请填写正确的" + this.props.data.elements.name.title + "不要中文！";
            var isNameValueUseAble = 0;
            var isNameValueNotEmpty = 1;
        }
        else {
            nameclassname = "help-inline";
            nametips = this.props.data.elements.name.tips;
            var isNameValueUseAble = 1;
            var isNameValueNotEmpty = 1;
        }
        if (!(tel)) {
            telclassname = "help-inline tips";
            teltips = "请填写有效的" + this.props.data.elements.tel.title + "!";
            var isTelValueUseAble = 0;
            var isTelValueNotEmpty = 0;

        }
        else if (!(this.isPropertyTel(tel))) {
            telclassname = "help-inline tips";
            teltips = "请填写有效的" + this.props.data.elements.tel.title + "!";
            var isTelValueUseAble = 0;
            var isTelValueNotEmpty = 1;
        }
        else {
            telclassname = "help-inline";
            teltips = this.props.data.elements.tel.tips;
            var isTelValueUseAble = 1;
            var isTelValueNotEmpty = 1;
        }
        if (!(zoneid) || (zoneid == -1)) {
            zoneclassname = "help-inline tips";
            zonetips = "请选择酒店所在区域！";
            var isAreaValueUseAble = 0;
            var isAreaValueNotEmpty = 0;
        }
        else {
            zoneclassname = "help-inline";
            zonetips = this.props.data.elements.area.tips;
            var isAreaValueUseAble = 1;
            var isAreaValueNotEmpty = 1;
        }
        if ('address' in this.props.data.elements) {
            if (!address) {
                addressclassname = "help-inline tips";
                addresstips = "请填写" + this.props.data.elements.address.title;
                var isAddressValueUseAble = 0;
                var isAddressValueNotEmpty = 0;
            }
            else {
                addressclassname = "help-inline";
                addresstips = this.props.data.elements.address.tips;
                var isAddressValueUseAble = 1;
                var isAddressValueNotEmpty = 1;
            }
        }
        if (this.props.limit === false || (this.props.data.mandatory == "2")) {//unlimited
            nametips = this.props.data.elements.name.tips;
            nameclassname = "";
            teltips = this.props.data.elements.tel.tips;
            telclassname = "";
            zonetips = this.props.data.elements.area.tips;
            zoneclassname = "";
            if ('address' in this.props.data.elements) {
                addresstips = this.props.data.elements.address.tips;
                addressclassname = "";
            }
        }
        var classtips = {
            nametips: nametips,
            nameclassname: nameclassname,
            teltips: teltips,
            telclassname: telclassname,
            zonetips: zonetips,
            zoneclassname: zoneclassname,
        }
        if ('address' in this.props.data.elements) {
            classtips.addresstips = addresstips;
            classtips.addressclassname = addressclassname;
        }
        this.setState(classtips);

        var stateObj = new Object();
        stateObj[this.props.suborderid] = new Object();

        stateObj[this.props.suborderid][this.props.data.elements.name.code] = new Object();
        stateObj[this.props.suborderid][this.props.data.elements.name.code].isValueUseAble = isNameValueUseAble;
        stateObj[this.props.suborderid][this.props.data.elements.name.code].isValueNotEmpty = isNameValueNotEmpty;
        stateObj[this.props.suborderid][this.props.data.elements.name.code].isValueMandatory = this.props.data.mandatory;

        stateObj[this.props.suborderid][this.props.data.elements.tel.code] = new Object();
        stateObj[this.props.suborderid][this.props.data.elements.tel.code].isValueUseAble = isTelValueUseAble;
        stateObj[this.props.suborderid][this.props.data.elements.tel.code].isValueNotEmpty = isTelValueNotEmpty;
        stateObj[this.props.suborderid][this.props.data.elements.tel.code].isValueMandatory = this.props.data.mandatory;

        stateObj[this.props.suborderid][this.props.data.elements.area.code] = new Object();
        stateObj[this.props.suborderid][this.props.data.elements.area.code].isValueUseAble = isAreaValueUseAble;
        stateObj[this.props.suborderid][this.props.data.elements.area.code].isValueNotEmpty = isAreaValueNotEmpty;
        stateObj[this.props.suborderid][this.props.data.elements.area.code].isValueMandatory = this.props.data.mandatory;
        if ('address' in this.props.data.elements) {
            stateObj[this.props.suborderid][this.props.data.elements.address.code] = new Object();
            stateObj[this.props.suborderid][this.props.data.elements.address.code].isValueUseAble = isAddressValueUseAble;
            stateObj[this.props.suborderid][this.props.data.elements.address.code].isValueNotEmpty = isAddressValueNotEmpty;
            stateObj[this.props.suborderid][this.props.data.elements.address.code].isValueMandatory = this.props.data.mandatory;
        }
        this.props.reciveValueState(stateObj);
    },
    detectValueWhenMounted: function () {
        if (this.getDefaultValue() == "emptyrow") {
            var _this = this;
            this.setState(
                {
                    nametips: this.props.data.elements.name.tips,
                    nameclassname: "help-inline",
                    teltips: this.props.data.elements.tel.tips,
                    telclassname: "help-inline",
                    zonetips: this.props.data.elements.area.tips,
                    zoneclassname: "help-inline",
                    addresstips: (function () {
                        if ('address' in _this.props.data.elements) {
                            return _this.props.data.elements.address.tips;
                        }
                        else {
                            return "";
                        }
                    })(),
                    addressclassname: "help-inline",
                }
            );

            var stateObj = new Object();
            stateObj[this.props.suborderid] = new Object();

            stateObj[this.props.suborderid][this.props.data.elements.name.code] = new Object();
            stateObj[this.props.suborderid][this.props.data.elements.name.code].isValueUseAble = 0;
            stateObj[this.props.suborderid][this.props.data.elements.name.code].isValueNotEmpty = 0;
            stateObj[this.props.suborderid][this.props.data.elements.name.code].isValueMandatory = this.props.data.mandatory;

            stateObj[this.props.suborderid][this.props.data.elements.tel.code] = new Object();
            stateObj[this.props.suborderid][this.props.data.elements.tel.code].isValueUseAble = 0;
            stateObj[this.props.suborderid][this.props.data.elements.tel.code].isValueNotEmpty = 0;
            stateObj[this.props.suborderid][this.props.data.elements.tel.code].isValueMandatory = this.props.data.mandatory;

            stateObj[this.props.suborderid][this.props.data.elements.area.code] = new Object();
            stateObj[this.props.suborderid][this.props.data.elements.area.code].isValueUseAble = 0;
            stateObj[this.props.suborderid][this.props.data.elements.area.code].isValueNotEmpty = 0;
            stateObj[this.props.suborderid][this.props.data.elements.area.code].isValueMandatory = this.props.data.mandatory;

            if ('address' in this.props.data.elements) {
                stateObj[this.props.suborderid][this.props.data.elements.address.code] = new Object();
                stateObj[this.props.suborderid][this.props.data.elements.address.code].isValueUseAble = 0;
                stateObj[this.props.suborderid][this.props.data.elements.address.code].isValueNotEmpty = 0;
                stateObj[this.props.suborderid][this.props.data.elements.address.code].isValueMandatory = this.props.data.mandatory;
            }
            this.props.reciveValueState(stateObj);
        }
        else {
            var address = '';
            if ('address' in this.props.data.elements) {
                var address = this.state.postValue[this.props.data.elements.address.code]
            }
            this.detectValueState(
                this.state.postValue[this.props.data.elements.name.code],
                this.state.postValue[this.props.data.elements.tel.code],
                this.state.postValue[this.props.data.elements.area.code]['value'],
                address
            );
        }
    },

    getInitialState: function () {

        return {
            areas: [],
        }
    },
    componentDidMount: function () {
        this.detectValueWhenMounted();
        var _this = this;
        {
            var value = _this.state.postValue[_this.props.data.elements.area.code]['value'];
            var text = value == -1 ? "" : _this.state.postValue[_this.props.data.elements.area.code]['AreaEnName'];
            var obj = {};
            obj[_this.props.suborderid] = new Object();
            obj[_this.props.suborderid][_this.props.data.elements.name.code] = _this.state.postValue[_this.props.data.elements.name.code];
            obj[_this.props.suborderid][_this.props.data.elements.tel.code] = _this.state.postValue[_this.props.data.elements.tel.code];

            var elements = _this.props.data.elements;
            if ('address' in elements) {
                obj[_this.props.suborderid][_this.props.data.elements.address.code] = _this.state.postValue[_this.props.data.elements.address.code];
            }

            obj[_this.props.suborderid][_this.props.data.elements.area.code] = text;
            _this.props.reciveTempValue(obj);

            //更新粘贴板内容
            var copyObj = {};
            copyObj[_this.props.data.elements.name.code] = {
                title: _this.props.data.elements.name.title,
                str: _this.state.postValue[_this.props.data.elements.name.code]
            }
            copyObj[_this.props.data.elements.tel.code] = {
                title: _this.props.data.elements.tel.title,
                str: _this.state.postValue[_this.props.data.elements.tel.code]
            }
            copyObj[_this.props.data.elements.area.code] = {
                title: _this.props.data.elements.area.title,
                str: text
            }
            if ('address' in elements) {
                copyObj[_this.props.data.elements.address.code] = {
                    title: _this.props.data.elements.address.title,
                    str: _this.state.postValue[_this.props.data.elements.address.code]
                }
            }
            _this.updateCopyObj(copyObj);
        }

        //异步获取信息
        var cityid = this.props.data.areaID
        $.ajax({
            url: '/Hotals/GetArea',
            type: 'get',
            dataType: 'json',
            data: {
                CityID: cityid
            },
            success: function (data) {
                if (data.ErrorCode == 200) {
                    _this.setState(
                        {
                            areas: data.area
                        },
                        function () {
                            var lastValue = _this.prehandledata(_this.props.beforeRechangeValue);
                            var postValue = _this.prehandledata(_this.state.postValue);
                            if (this.props.UIType == "reChange") {
                                if (lastValue[_this.props.data.elements.area.code].value == postValue[_this.props.data.elements.area.code].value) {
                                    var value = -1;
                                }
                                else {
                                    var value = postValue[_this.props.data.elements.area.code].value;
                                    var matched = false;
                                    for (var i in data.area) {
                                        if (parseInt(data.area[i].AreaID) === parseInt(value)) {
                                            matched = true;
                                            break;
                                        }
                                    }
                                    if (matched === false && (parseInt(value) !== -1)) {
                                        if ('address' in _this.props.data.elements) {
                                            _this.detectValueState(
                                                _this.state.postValue[_this.props.data.elements.name.code],
                                                _this.state.postValue[_this.props.data.elements.tel.code],
                                                -1,
                                                _this.state.postValue[_this.props.data.elements.address.code]
                                            );
                                        } else {
                                            _this.detectValueState(
                                                _this.state.postValue[_this.props.data.elements.name.code],
                                                _this.state.postValue[_this.props.data.elements.tel.code],
                                                -1,
                                            );
                                        }
                                        value = -1;
                                    }
                                }
                            }
                            else {
                                var value = postValue[_this.props.data.elements.area.code].value;
                                var matched = false;
                                for (var i in data.area) {
                                    if (parseInt(data.area[i].AreaID) === parseInt(value)) {
                                        matched = true;
                                        break;
                                    }
                                }
                                if (matched === false && (parseInt(value) !== -1)) {
                                    if ('address' in _this.props.data.elements) {
                                        _this.detectValueState(
                                            _this.state.postValue[_this.props.data.elements.name.code],
                                            _this.state.postValue[_this.props.data.elements.tel.code],
                                            -1,
                                            _this.state.postValue[_this.props.data.elements.address.code]
                                        );
                                    } else {
                                        _this.detectValueState(
                                            _this.state.postValue[_this.props.data.elements.name.code],
                                            _this.state.postValue[_this.props.data.elements.tel.code],
                                            -1,
                                        );
                                    }
                                    value = -1;
                                }
                            }
                            $(_this.refs.area).val(value);
                        }
                    )
                }
            }
        })


        var remote_cities = new Bloodhound({
            datumTokenizer: function (d) {
                return Bloodhound.tokenizers.whitespace(d.HotalName);
            },
            queryTokenizer: Bloodhound.tokenizers.whitespace,
            limit: 15,
            // 在文本框输入字符时才发起请求
            //
            // local:dt,
            remote: {
                wildcard: '%QUERY',
                url: window.location.origin + '/Hotals/GetHotal?CityID=' + cityid + '&Str=%QUERY',
                filter: function (data) {
                    return data.hotals.map( function (country) {
                        return {
                            AreaName: country.HotalName,
                            tel: country.HotalPhone,
                            AreaID: country.AreaID,
                            hotelAreaName: country.HotalArea.AreaName,
                            AreaEnName: country.HotalArea.AreaEnName,
                            address: country.HotalAddress
                        };
                    });
                }
            }
        });
        remote_cities.initialize();
        jQuery((this.refs.name)).typeahead({
            hint: false,
            highlight: true,
            minLength: 1,
            classNames: {
                menu: (function () {
                    var width = $(document).width();
                    if (width < 500) {
                        return 'tt-menu tt-menu-top';
                    }
                    return 'tt-menu';
                })()
            }
        }, {
                name: 'xxx',
                displayKey: 'AreaName',
                limit: 30,
                source: remote_cities,
                templates: {
                    header: function (data) {
                        return ([
                            '<div class="empty-message">',
                            '共搜索到<strong>' + data.suggestions.length + '</strong>个酒店',
                            '</div>'
                        ].join('\n')
                        );
                    }

                }
            }).bind('typeahead:select', function (ev, oneselected) {
                var elements = _this.props.data.elements;
                jQuery(_this.refs.tel).val(oneselected['tel']);

                jQuery(_this.refs.area).val(oneselected.AreaID);
                jQuery(_this.refs.address).val(oneselected.address);
                var obj = new Object();
                obj[elements.area.code] = {
                    'value': oneselected.AreaID,
                    'hotelAreaName': oneselected.hotelAreaName,
                    "AreaEnName": oneselected.AreaEnName
                };
                obj[elements.name.code] = oneselected.AreaName;
                obj[elements.tel.code] = oneselected.tel;


                var elements = _this.props.data.elements;
                var address = '';
                if ('address' in elements) {
                    obj[elements.address.code] = $(_this.refs.address).val();
                    address = $(_this.refs.address).val();
                };

                //回复默认值

                if (_this.props.UIType == "reChange") {
                    var lastValue = _this.prehandledata(_this.props.beforeRechangeValue);
                    if (!obj[elements.name.code]) {
                        obj[_this.props.data.elements.name.code] = lastValue[_this.props.data.elements.name.code];
                    };
                    if (!obj[elements.tel.code]) {
                        obj[_this.props.data.elements.tel.code] = lastValue[_this.props.data.elements.tel.code];
                    }
                    if (obj[elements.area.code]['value'] == -1) {
                        obj[_this.props.data.elements.name.code] = lastValue[_this.props.data.elements.name.code];
                    }
                    if ('address' in elements) {
                        if (!obj[elements.address.code]) {
                            obj[_this.props.data.elements.address.code] = lastValue[_this.props.data.elements.address.code];
                            address = lastValue[_this.props.data.elements.address.code];
                        }
                    };
                }

                _this.detectValueState(
                    obj[elements.name.code],
                    obj[elements.tel.code],
                    obj[elements.area.code]['value'],
                    address
                )

                _this.setState(
                    {
                        postValue: obj
                    },
                    function () {
                        var _this = this;
                        this.upadteToForm(this.getdUpdateData());


                        var id = this.state.postValue[this.props.data.elements.area.code].value;
                        var text = id == -1 ? "" : this.state.postValue[this.props.data.elements.area.code].AreaEnName;

                        var obj = {};
                        obj[this.props.suborderid] = new Object();
                        obj[this.props.suborderid][this.props.data.elements.name.code] = this.state.postValue[this.props.data.elements.name.code];
                        obj[this.props.suborderid][this.props.data.elements.tel.code] = this.state.postValue[this.props.data.elements.tel.code];

                        var elements = this.props.data.elements;
                        if ('address' in elements) {
                            obj[this.props.suborderid][this.props.data.elements.address.code] = this.state.postValue[this.props.data.elements.address.code];
                        }
                        obj[this.props.suborderid][this.props.data.elements.area.code] = text;

                        this.props.reciveTempValue(obj);

                        //更新粘贴板内容
                        var copyObj = {};
                        copyObj[_this.props.data.elements.name.code] = {
                            title: _this.props.data.elements.name.title,
                            str: _this.state.postValue[_this.props.data.elements.name.code]
                        }
                        copyObj[_this.props.data.elements.tel.code] = {
                            title: _this.props.data.elements.tel.title,
                            str: _this.state.postValue[_this.props.data.elements.tel.code]
                        }
                        copyObj[_this.props.data.elements.area.code] = {
                            title: _this.props.data.elements.area.title,
                            str: text
                        }
                        if ('address' in elements) {
                            copyObj[_this.props.data.elements.address.code] = {
                                title: _this.props.data.elements.address.title,
                                str: _this.state.postValue[_this.props.data.elements.address.code]
                            }
                        }
                        _this.updateCopyObj(copyObj);

                    }
                )

            })
    },
    namechange: function (e) {

        var obj = {};
        var value = e.target.value;
        var lastvalue = this.state.postValue;
        obj[this.props.data.elements.name.code] = value;
        obj = $.extend(true, new Object(), lastvalue, obj);



        var address = "";
        var elements = this.props.data.elements;
        if ('address' in elements) {
            address = obj[elements.address.code];
        }



        //回复默认值
        var lastValue = this.prehandledata(this.props.beforeRechangeValue);

        if (this.props.UIType == "reChange") {
            if (!obj[elements.name.code]) {
                obj[this.props.data.elements.name.code] = lastValue[this.props.data.elements.name.code];
            };
            if (!obj[elements.tel.code]) {
                obj[this.props.data.elements.tel.code] = lastValue[this.props.data.elements.tel.code];
            }
            if (obj[elements.area.code]['value'] == -1) {
                obj[this.props.data.elements.name.code] = lastValue[this.props.data.elements.name.code];
            }
            if ('address' in elements) {
                if (!obj[elements.address.code]) {
                    obj[this.props.data.elements.address.code] = lastValue[this.props.data.elements.address.code];
                    address = lastValue[this.props.data.elements.address.code];
                }
            };
        }

        this.detectValueState(
            obj[elements.name.code],
            obj[elements.tel.code],
            obj[elements.area.code]['value'],
            address
        )



        this.setState(
            {
                postValue: obj
            },
            function () {
                var _this = this;
                this.upadteToForm(this.getdUpdateData());


                var id = this.state.postValue[this.props.data.elements.area.code].value;
                var text = id == -1 ? "" : this.state.postValue[this.props.data.elements.area.code].AreaEnName;
                var obj = {};
                obj[this.props.suborderid] = new Object();
                obj[this.props.suborderid][this.props.data.elements.name.code] = this.state.postValue[this.props.data.elements.name.code];
                obj[this.props.suborderid][this.props.data.elements.tel.code] = this.state.postValue[this.props.data.elements.tel.code];

                var elements = this.props.data.elements;
                if ('address' in elements) {
                    obj[this.props.suborderid][this.props.data.elements.address.code] = this.state.postValue[this.props.data.elements.address.code];
                }

                obj[this.props.suborderid][this.props.data.elements.area.code] = text;

                this.props.reciveTempValue(obj);

                //更新粘贴板内容
                var copyObj = {};
                copyObj[_this.props.data.elements.name.code] = {
                    title: _this.props.data.elements.name.title,
                    str: _this.state.postValue[_this.props.data.elements.name.code]
                }
                copyObj[_this.props.data.elements.tel.code] = {
                    title: _this.props.data.elements.tel.title,
                    str: _this.state.postValue[_this.props.data.elements.tel.code]
                }
                copyObj[_this.props.data.elements.area.code] = {
                    title: _this.props.data.elements.area.title,
                    str: text
                }
                if ('address' in elements) {
                    copyObj[_this.props.data.elements.address.code] = {
                        title: _this.props.data.elements.address.title,
                        str: _this.state.postValue[_this.props.data.elements.address.code]
                    }
                }
                _this.updateCopyObj(copyObj);
            }
        )
    },
    telchange: function (e) {
        var obj = {};
        var value = e.target.value;
        var lastvalue = this.state.postValue;
        obj[this.props.data.elements.tel.code] = value;
        obj = $.extend(true, new Object(), lastvalue, obj);


        var address = "";
        var elements = this.props.data.elements;
        if ('address' in elements) {
            address = obj[elements.address.code];
        }



        //回复默认值
        var lastValue = this.prehandledata(this.props.beforeRechangeValue);

        if (this.props.UIType == "reChange") {
            if (!obj[elements.name.code]) {
                obj[this.props.data.elements.name.code] = lastValue[this.props.data.elements.name.code];
            };
            if (!obj[elements.tel.code]) {
                obj[this.props.data.elements.tel.code] = lastValue[this.props.data.elements.tel.code];
            }
            if (obj[elements.area.code]['value'] == -1) {
                obj[this.props.data.elements.name.code] = lastValue[this.props.data.elements.name.code];
            }
            if ('address' in elements) {
                if (!obj[elements.address.code]) {
                    obj[this.props.data.elements.address.code] = lastValue[this.props.data.elements.address.code];
                    address = lastValue[this.props.data.elements.address.code];
                }
            };
        }
        this.detectValueState(
            obj[elements.name.code],
            obj[elements.tel.code],
            obj[elements.area.code]['value'],
            address
        )



        this.setState(
            {
                postValue: obj
            },
            function () {
                var _this = this;
                this.upadteToForm(this.getdUpdateData());

                var id = this.state.postValue[this.props.data.elements.area.code].value;
                var text = (id == -1 ? "" : this.state.postValue[this.props.data.elements.area.code].AreaEnName);
                var obj = {};
                obj[this.props.suborderid] = new Object();
                obj[this.props.suborderid][this.props.data.elements.name.code] = this.state.postValue[this.props.data.elements.name.code];
                obj[this.props.suborderid][this.props.data.elements.tel.code] = this.state.postValue[this.props.data.elements.tel.code];

                var elements = this.props.data.elements;
                if ('address' in elements) {
                    obj[this.props.suborderid][this.props.data.elements.address.code] = this.state.postValue[this.props.data.elements.address.code];
                }

                obj[this.props.suborderid][this.props.data.elements.area.code] = text;

                this.props.reciveTempValue(obj);

                //更新粘贴板内容
                var copyObj = {};
                copyObj[_this.props.data.elements.name.code] = {
                    title: _this.props.data.elements.name.title,
                    str: _this.state.postValue[_this.props.data.elements.name.code]
                }
                copyObj[_this.props.data.elements.tel.code] = {
                    title: _this.props.data.elements.tel.title,
                    str: _this.state.postValue[_this.props.data.elements.tel.code]
                }
                copyObj[_this.props.data.elements.area.code] = {
                    title: _this.props.data.elements.area.title,
                    str: text
                }
                if ('address' in elements) {
                    copyObj[_this.props.data.elements.address.code] = {
                        title: _this.props.data.elements.address.title,
                        str: _this.state.postValue[_this.props.data.elements.address.code]
                    }
                }
                _this.updateCopyObj(copyObj);
            }
        )

    },
    addresschange: function (e) {
        var obj = {};
        var value = e.target.value;
        var lastvalue = this.state.postValue;
        obj[this.props.data.elements.address.code] = value;
        obj = $.extend(true, new Object(), lastvalue, obj);



        var address = "";
        var elements = this.props.data.elements;
        if ('address' in elements) {
            address = obj[elements.address.code];
        }


        //回复默认值
        var lastValue = this.prehandledata(this.props.beforeRechangeValue);

        if (this.props.UIType == "reChange") {
            if (!obj[elements.name.code]) {
                obj[this.props.data.elements.name.code] = lastValue[this.props.data.elements.name.code];
            };
            if (!obj[elements.tel.code]) {
                obj[this.props.data.elements.tel.code] = lastValue[this.props.data.elements.tel.code];
            }
            if (obj[elements.area.code]['value'] == -1) {
                obj[this.props.data.elements.name.code] = lastValue[this.props.data.elements.name.code];
            }
            if ('address' in elements) {
                if (!obj[elements.address.code]) {
                    obj[this.props.data.elements.address.code] = lastValue[this.props.data.elements.address.code];
                    address = lastValue[this.props.data.elements.address.code];
                }
            };
        }
        this.detectValueState(
            obj[elements.name.code],
            obj[elements.tel.code],
            obj[elements.area.code]['value'],
            address
        )



        this.setState(
            {
                postValue: obj
            },
            function () {
                var _this = this;
                this.upadteToForm(this.getdUpdateData());

                var id = this.state.postValue[this.props.data.elements.area.code].value;
                var text = id == -1 ? "" : this.state.postValue[this.props.data.elements.area.code].AreaEnName;
                var obj = {};
                obj[this.props.suborderid] = new Object();
                obj[this.props.suborderid][this.props.data.elements.name.code] = this.state.postValue[this.props.data.elements.name.code];
                obj[this.props.suborderid][this.props.data.elements.tel.code] = this.state.postValue[this.props.data.elements.tel.code];

                obj[this.props.suborderid][this.props.data.elements.address.code] = this.state.postValue[this.props.data.elements.address.code];


                obj[this.props.suborderid][this.props.data.elements.area.code] = text;

                this.props.reciveTempValue(obj);

                //更新粘贴板内容
                var copyObj = {};
                copyObj[_this.props.data.elements.name.code] = {
                    title: _this.props.data.elements.name.title,
                    str: _this.state.postValue[_this.props.data.elements.name.code]
                }
                copyObj[_this.props.data.elements.tel.code] = {
                    title: _this.props.data.elements.tel.title,
                    str: _this.state.postValue[_this.props.data.elements.tel.code]
                }
                copyObj[_this.props.data.elements.area.code] = {
                    title: _this.props.data.elements.area.title,
                    str: text
                }
                copyObj[_this.props.data.elements.address.code] = {
                    title: _this.props.data.elements.address.title,
                    str: _this.state.postValue[_this.props.data.elements.address.code]
                }
                _this.updateCopyObj(copyObj);
            }
        )

    },
    selectchange: function (e) {
        var obj = {};
        var value = e.target.value;
        var alt = $(e.target).find("option:selected").text().split('-')[0];
        var lastvalue = this.state.postValue;
        obj[this.props.data.elements.area.code] = {
            value: value,
            AreaEnName: alt
        };
        obj = $.extend(true, new Object(), lastvalue, obj);


        var address = "";
        var elements = this.props.data.elements;
        if ('address' in elements) {
            address = obj[elements.address.code];
        }


        //回复默认值
        var lastValue = this.prehandledata(this.props.beforeRechangeValue);

        if (this.props.UIType == "reChange") {
            if (!obj[elements.name.code]) {
                obj[this.props.data.elements.name.code] = lastValue[this.props.data.elements.name.code];
            };
            if (!obj[elements.tel.code]) {
                obj[this.props.data.elements.tel.code] = lastValue[this.props.data.elements.tel.code];
            }
            if (obj[elements.area.code]['value'] == -1) {
                obj[this.props.data.elements.area.code] = lastValue[this.props.data.elements.area.code];
            }
            if ('address' in elements) {
                if (!obj[elements.address.code]) {
                    obj[this.props.data.elements.address.code] = lastValue[this.props.data.elements.address.code];
                    address = lastValue[this.props.data.elements.address.code];
                }
            };
        }

        this.detectValueState(
            obj[elements.name.code],
            obj[elements.tel.code],
            obj[elements.area.code]['value'],
            address
        )


        this.setState(
            {
                postValue: obj
            },
            function () {
                var _this = this;
                this.upadteToForm(this.getdUpdateData());


                var id = this.state.postValue[this.props.data.elements.area.code].value;
                var text = (id == -1 ? "" : this.state.postValue[this.props.data.elements.area.code].AreaEnName);

                var obj = {};
                obj[this.props.suborderid] = new Object();
                obj[this.props.suborderid][this.props.data.elements.name.code] = this.state.postValue[this.props.data.elements.name.code];
                obj[this.props.suborderid][this.props.data.elements.tel.code] = this.state.postValue[this.props.data.elements.tel.code];
                var elements = this.props.data.elements;
                if ('address' in elements) {
                    obj[this.props.suborderid][this.props.data.elements.address.code] = this.state.postValue[this.props.data.elements.address.code];
                }
                obj[this.props.suborderid][this.props.data.elements.area.code] = text;

                this.props.reciveTempValue(obj);
                //更新粘贴板内容
                var copyObj = {};
                copyObj[_this.props.data.elements.name.code] = {
                    title: _this.props.data.elements.name.title,
                    str: _this.state.postValue[_this.props.data.elements.name.code]
                }
                copyObj[_this.props.data.elements.tel.code] = {
                    title: _this.props.data.elements.tel.title,
                    str: _this.state.postValue[_this.props.data.elements.tel.code]
                }
                copyObj[_this.props.data.elements.area.code] = {
                    title: _this.props.data.elements.area.title,
                    str: text
                }
                if ('address' in elements) {
                    copyObj[_this.props.data.elements.address.code] = {
                        title: _this.props.data.elements.address.title,
                        str: _this.state.postValue[_this.props.data.elements.address.code]
                    }
                }
                _this.updateCopyObj(copyObj);
            }
        )
    },
    renderselect: function (areas, cityid) {
        var arr = new Array(<option key={-1} value="-1">请选择</option>);
        return (
            arr.concat(
                areas.map(function (area, index) {
                    return (
                        <option key={index} value={area.AreaID}>{area.AreaEnName}-{area.AreaName}</option>
                    )
                }))
        )
    },
    renderAddress: function (reChangeDefault) {
        var suborderid = this.props.suborderid;
        var componentid = this.props.componentid;
        var elements = this.props.data.elements;
        if ('address' in elements) {
            if (this.props.UIType == "edit") {
                return (
                    <div className="form-group" data-suborderid={suborderid} data-componentid={componentid} data-segmentcode={elements.address.code} >
                        <Lable params={{ isMust: this.props.data.mandatory == 1 ? true : false, title: elements.address.title }} ></Lable>
                        <div className="col-md-10">
                            <input type="text" ref="address" className="form-control input-inline input-sm input-medium" defaultValue={this.state.postValue[elements.address.code]} onChange={this.addresschange} placeholder="" />
                            <span className={this.state.addressclassname}>{this.state.addresstips}</span>
                        </div>
                    </div>
                )
            }
            else if (this.props.UIType == "reChange") {
                return (
                    <tr>
                        <td>
                            <LableRechange params={{ isMust: this.props.data.mandatory == 1 ? true : false, title: elements.address.title }} ></LableRechange>
                        </td>
                        <td>{this.getDefaultValue()[this.props.data.elements.address.code]}</td>
                        <td>
                            <input type="text" ref="address" defaultValue={reChangeDefault} className="form-control input-inline input-sm input-medium" onChange={this.addresschange} placeholder="" />
                        </td>
                    </tr>
                )
            }
        }
    },
    render: function () {
        var data = this.props.data;
        var name = this.props.data.elements.name;
        var tel = this.props.data.elements.tel;
        var area = this.props.data.elements.area;
        var cityID = data.areaID;

        var suborderid = this.props.suborderid;
        var componentid = this.props.componentid;
        if (this.props.UIType == "edit") {
            return (
                <div>
                    <div className="form-group" data-suborderid={suborderid} data-componentid={componentid} data-segmentcode={name.code} >
                        <Lable params={{ isMust: data.mandatory == 1 ? true : false, title: name.title }} ></Lable>
                        <div className="col-md-10">

                            <input type="text" ref="name" className="form-control input-inline input-medium  input-sm" defaultValue={this.state.postValue[name.code]} onChange={this.namechange} placeholder="输入酒店名字或搜索" />

                            <span className={this.state.nameclassname}>  {this.state.nametips}</span>

                        </div>
                    </div>

                    <div className="form-group" data-suborderid={suborderid} data-componentid={componentid} data-segmentcode={tel.code} >
                        <Lable params={{ isMust: data.mandatory == 1 ? true : false, title: tel.title }} ></Lable>
                        <div className="col-md-10">
                            <input type="text" ref="tel" className="form-control input-inline input-medium  input-sm" defaultValue={this.state.postValue[tel.code]} onChange={this.telchange} placeholder="输入酒店电话" />
                            <span className={this.state.telclassname}>{this.state.teltips}</span>
                        </div>
                    </div>

                    <div className="form-group" data-suborderid={suborderid} data-componentid={componentid} data-segmentcode={area.code} >
                        <Lable params={{ isMust: data.mandatory == 1 ? true : false, title: area.title }} ></Lable>
                        <div className="col-md-10">
                            <select ref="area" onChange={this.selectchange} className="form-control input-inline input-sm input-medium">
                                {this.renderselect(this.state.areas)}
                            </select>

                            <span className={this.state.zoneclassname}>{this.state.zonetips}</span>
                        </div>
                    </div>
                    {this.renderAddress()}

                </div>
            )
        }
        else if (this.props.UIType == "reChange") {

            var lastValue = this.prehandledata(this.props.beforeRechangeValue);

            var nameChange = 0;
            var valueNameNow = "";
            var telChange = 0;
            var valueTelNow = "";
            var areaChange = 0;
            var valueAreaNow = "";
            var addressChange = 0;
            var valueAddressNow = "";

            if (0) {
                var nameChange = 0;
                var valueNameNow = "";
                var telChange = 0;
                var valueTelNow = "";
                var areaChange = 0;
                var valueAreaNow = -1;
                if ('address' in this.props.data.elements) {
                    var addressChange = 0;
                    var valueAddressNow = "";
                }
            }
            else {
                if ((lastValue[name.code] == this.state.postValue[name.code]) || this.state.postValue[name.code] == '') {
                    nameChange = 0;
                    valueNameNow = "";
                }
                else {
                    nameChange = 1;
                    valueNameNow = this.state.postValue[name.code];
                }
                if ((lastValue[tel.code] == this.state.postValue[tel.code]) || this.state.postValue[tel.code] == '') {
                    telChange = 0;
                    valueTelNow = '';
                }
                else {
                    telChange = 1;
                    valueTelNow = this.state.postValue[tel.code];
                }
                if ((lastValue[area.code]['value'] == this.state.postValue[area.code]['value'])) {
                    areaChange = 0;
                    // valueAreaNow = '-1';
                    //  valueAreaNow = this.state.postValue[area.code]['value'];
                }
                else {
                    areaChange = 1;
                    valueAreaNow = this.state.postValue[area.code]['value'];
                }
                if ('address' in this.props.data.elements) {
                    var address = this.props.data.elements.address;
                    if ((lastValue[address.code] == this.state.postValue[address.code]) || this.state.postValue[address.code] == '') {
                        addressChange = 0;
                        valueAddressNow = '';
                    }
                    else {
                        addressChange = 1;
                        valueAddressNow = this.state.postValue[address.code];
                    }
                }

            }
            var changeObj = new Object();
            changeObj[this.props.suborderid] = new Object();

            changeObj[this.props.suborderid][this.props.data.elements.name.code] = new Object();
            changeObj[this.props.suborderid][this.props.data.elements.name.code].isValueChange = nameChange;

            changeObj[this.props.suborderid][this.props.data.elements.tel.code] = new Object();
            changeObj[this.props.suborderid][this.props.data.elements.tel.code].isValueChange = telChange;

            changeObj[this.props.suborderid][this.props.data.elements.area.code] = new Object();
            changeObj[this.props.suborderid][this.props.data.elements.area.code].isValueChange = areaChange;

            if ('address' in this.props.data.elements) {
                changeObj[this.props.suborderid][this.props.data.elements.address.code] = new Object();
                changeObj[this.props.suborderid][this.props.data.elements.address.code].isValueChange = addressChange;

            }


            this.props.reciveValueChange(changeObj);
            return (
                <tbody>
                    <tr>
                        <td>
                            <LableRechange params={{ isMust: data.mandatory == 1 ? true : false, title: name.title }} ></LableRechange>
                        </td>
                        <td>{this.getDefaultValue()[this.props.data.elements.name.code]}</td>
                        <td>
                            <input type="text" ref="name" className="form-control input-inline input-medium  input-sm" defaultValue={valueNameNow} onChange={this.namechange} placeholder="输入酒店名字或搜索" />
                            <div> <span className={this.state.nameclassname}>  {this.state.nametips}</span></div>
                        </td>
                    </tr>

                    <tr>
                        <td>
                            <LableRechange params={{ isMust: data.mandatory == 1 ? true : false, title: tel.title }} ></LableRechange>
                        </td>
                        <td>{this.getDefaultValue()[this.props.data.elements.tel.code]}</td>
                        <td>
                            <input type="text" ref="tel" className="form-control input-inline input-medium  input-sm" defaultValue={valueTelNow} onChange={this.telchange} placeholder="输入酒店电话" />
                            <div><span className={this.state.telclassname}>{this.state.teltips}</span></div>
                        </td>
                    </tr>

                    <tr>
                        <td>
                            <LableRechange params={{ isMust: data.mandatory == 1 ? true : false, title: area.title }} ></LableRechange>
                        </td>
                        <td>{this.getDefaultValue()[this.props.data.elements.area.code]['hotelAreaName']}</td>
                        <td>
                            <select ref="area" onChange={this.selectchange} className="form-control input-inline input-sm input-medium">
                                {this.renderselect(this.state.areas)}
                            </select>
                            <div><span className={this.state.zoneclassname}>{this.state.zonetips}</span></div>

                        </td>
                    </tr>
                    {this.renderAddress(valueAddressNow)}

                </tbody>
            )
        }

    }
})


