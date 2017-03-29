var BaseInfoRef = new Object();
var BaseInfo = React.createClass({
    componentDidMount: function () {
        BaseInfoRef = this;
        if (this.props.buttons && this.props.buttons.copyToClipboard === true) {
            this.updateCopyStr();
        }
        this.detectValueState(this.state, this.state.eidtFlag);
    },
    toHalfWidth: function (str) {
        var result = "";
        for (var i = 0; i < str.length; i++) {
            if (str.charCodeAt(i) == 12288) {
                result += String.fromCharCode(str.charCodeAt(i) - 12256);
                continue;
            }
            if (str.charCodeAt(i) > 65280 && str.charCodeAt(i) < 65375)
                result += String.fromCharCode(str.charCodeAt(i) - 65248);
            else
                result += String.fromCharCode(str.charCodeAt(i));
        }
        return result;
    },
    handleblur: function (e) {
        var obj = new Object();
        var id = e.target.id;
        if (id != "Wechat") {
            var value = this.toHalfWidth(e.target.value.trim());
            obj[e.target.id] = value;
        }
        else {
            var value = (e.target.value.trim());
            obj[e.target.id] = value;
        }
        if (id == "CustomerEnname") {
            obj[e.target.id] = value.toUpperCase()
        }
        this.setState(obj);
    },
    handlechange: function (e) {
        var obj = new Object();
        var id = e.target.id;
        if (id != "Wechat") {
            var value = this.toHalfWidth(e.target.value.trim());
            obj[e.target.id] = e.target.value.trim();
        }
        else {
            var value = (e.target.value.trim());
            obj[e.target.id] = e.target.value.trim();
        }
        if (id == "Wechat") {

            this.detectValueState({
                'Wechat': value
            }, false)
        }
        else if (id == "BakTel") {
            this.detectValueState({
                'BakTel': value
            }, false)
        }
        else if (id == "Tel") {
            this.detectValueState({
                'Tel': value
            }, false)

        }
        else if (id == "Email") {
            this.detectValueState({
                'Email': value
            }, false)
        }
        else if (id == "CustomerName") {
            this.detectValueState({
                'CustomerName': value
            }, false)
        }
        else if (id == "CustomerEnname") {
            this.detectValueState({
                'CustomerEnname': value
            }, false);
        }
        this.setState(obj, function () {
            if (this.props.buttons && this.props.buttons.copyToClipboard === true) {
                this.updateCopyStr();
            }
        });
    },
    isTel: function (str) {
        if (str) {
            return true;
        }
        else {
            return false;
        }
        // var reg = /^[０-９0-9 +-]+$/;
        // if (reg.test(str)) {
        //     console.log("right")
        //     return true;
        // }
        // else {
        //     console.log("wrong")
        //     return false;
        // }
    },
    isChineseChar: function isChineseChar(str) {
        var reg = /^[\u4e00-\u9fa5 ]+$/;
        if (reg.test(str)) {
            return true;
        }
        else {
            return false;
        }
    },
    isEnglishChar: function (str) {

        var reg = /^[ａ-ｚＡ-ＺA-Z a-z]+$/;
        if (reg.test(str)) {
            return true;
        }
        else {
            return false;
        }
    },
    isEmail: function isEmail(str) {

        var myReg = /^([a-zA-Z0-9_#*~$^`|;:"'/?<>,&\\\(\)={}\[\]\%\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{1,10})+$/;
        if (myReg.test(str)) {
            return true;
        }
        else {
            return false;
        }
    },
    getDefaultProps: function () {
        var a = new Object();
        a.CustomerEnnameTip = '请与护照上的姓名拼音保持一致';
        a.CustomerNameTip = '';
        a.EmailTip = '常用邮箱，用来接收确认单或预订信息变更通知';
        a.TelTip = '如有变更请及时告知我们';
        a.BakTelTip = '尽量填写国外当地号码，以便及时联系';
        a.WechatTip = '不是昵称，在“微信-我”界面中，您头像的旁边可以找到';

        a.CustomerEnnameclassname = 'help-inline';
        a.CustomerNameclassname = 'help-inline';
        a.Emailclassname = 'help-inline';
        a.Telclassname = 'help-inline';
        a.BakTelclassname = 'help-inline';
        a.Wechatclassname = 'help-inline';
        return a;
    },
    getInitialState: function () {

        var a = new Object();
        a.eidtFlag = !this.props.editFlag;
        // return this.props.baseinfo;


        a.tip = new Object();
        a.tip.CustomerEnnameTip = this.props.CustomerEnnameTip
        a.tip.CustomerNameTip = this.props.CustomerNameTip;
        a.tip.EmailTip = this.props.EmailTip;
        a.tip.TelTip = this.props.TelTip;
        a.tip.BakTelTip = this.props.BakTelTip;
        a.tip.WechatTip = this.props.WechatTip;

        a.tipclassname = new Object();

        a.tipclassname.CustomerEnnameclassname = this.props.CustomerEnnameclassname
        a.tipclassname.CustomerNameclassname = this.props.CustomerNameclassname;
        a.tipclassname.Emailclassname = this.props.Emailclassname;
        a.tipclassname.Telclassname = this.props.Telclassname;
        a.tipclassname.BakTelclassname = this.props.baseinfo.BakTelclassname;
        a.tipclassname.Wechatclassname = this.props.Wechatclassname;


        a.CustomerEnname = this.props.baseinfo.CustomerEnname ? this.toHalfWidth(this.props.baseinfo.CustomerEnname.toString().trim()) : "";
        a.CustomerName = this.props.baseinfo.CustomerName ? this.toHalfWidth(this.props.baseinfo.CustomerName.toString().trim()) : "";
        a.Email = this.props.baseinfo.Email ? this.toHalfWidth(this.props.baseinfo.Email.toString().trim()) : "";
        a.Tel = this.props.baseinfo.Tel ? this.toHalfWidth(this.props.baseinfo.Tel.toString().trim()) : "";
        a.BakTel = this.props.baseinfo.BakTel ? this.toHalfWidth(this.props.baseinfo.BakTel.toString().trim()) : "";
        a.Wechat = this.props.baseinfo.Wechat ? (this.props.baseinfo.Wechat.toString().trim()) : "";
        a.valueState = new Object();
        a.copyStr = "what the hell";
        return a;

    },
    detectValueState: function (obj, hasNotEdit) {

        var objx = new Object();
        objx = $.extend(true, {}, this.state.valueState);
        var tip = this.state.tip;
        var tipclassname = this.state.tipclassname;
        for (var i in obj) {
            if (
                i == "tip" ||
                i == 'tipclassname' ||
                i == 'eidtFlag'
            ) {
                continue;
            }
            if (i == "Wechat") {
                if (obj[i]) {
                    objx[i] = {
                        'isValueMandatory': 1,
                        'isValueNotEmpty': 1,
                        'isValueUseAble': 1
                    }
                    tip[i + "Tip"] = this.props[i + "Tip"];
                    tipclassname[i + "classname"] = this.props[i + "classname"];

                }
                else {
                    tip[i + "Tip"] = "请填写微信号！";
                    tipclassname[i + "classname"] = 'help-inline tips';
                    objx[i] = {
                        'isValueMandatory': 1,
                        'isValueNotEmpty': 0,
                        'isValueUseAble': 0
                    }
                }
                if (hasNotEdit == true) {
                    tip[i + "Tip"] = this.props[i + "Tip"];
                    tipclassname[i + "classname"] = this.props[i + "classname"];
                }

            }
            else if (i == "BakTel") {
                if (obj[i]) {
                    objx[i] = {
                        'isValueMandatory': 0,
                        'isValueNotEmpty': 1,
                        'isValueUseAble': 1
                    }
                    tip[i + "Tip"] = this.props[i + "Tip"];
                    tipclassname[i + "classname"] = this.props[i + "classname"];
                }
                else {
                    objx[i] = {
                        'isValueMandatory': 0,
                        'isValueNotEmpty': 1,
                        'isValueUseAble': 1
                    }
                    tip[i + "Tip"] = this.props[i + "Tip"];
                    tipclassname[i + "classname"] = 'help-inline ';
                }
                if (hasNotEdit == true) {
                    tip[i + "Tip"] = this.props[i + "Tip"];
                    tipclassname[i + "classname"] = this.props[i + "classname"];
                }
            }
            else if (i == "Tel") {
                if (obj[i]) {
                    if (this.isTel(obj[i])) {
                        objx[i] = {
                            'isValueMandatory': 1,
                            'isValueNotEmpty': 1,
                            'isValueUseAble': 1
                        }
                        tip[i + "Tip"] = this.props[i + "Tip"];
                        tipclassname[i + "classname"] = this.props[i + "classname"];
                    }
                    else {
                        objx[i] = {
                            'isValueMandatory': 1,
                            'isValueNotEmpty': 1,
                            'isValueUseAble': 0
                        }
                        tip[i + "Tip"] = "请检查联系电话";
                        tipclassname[i + "classname"] = 'help-inline tips';
                    }

                }
                else {
                    objx[i] = {
                        'isValueMandatory': 1,
                        'isValueNotEmpty': 0,
                        'isValueUseAble': 0
                    }
                    tip[i + "Tip"] = "请填写联系电话";
                    tipclassname[i + "classname"] = 'help-inline tips';
                }
                if (hasNotEdit == true) {
                    tip[i + "Tip"] = this.props[i + "Tip"];
                    tipclassname[i + "classname"] = this.props[i + "classname"];
                }

            }
            else if (i == "Email") {
                if (obj[i]) {
                    if (this.isEmail(obj[i])) {
                        objx[i] = {
                            'isValueMandatory': 1,
                            'isValueNotEmpty': 1,
                            'isValueUseAble': 1
                        }
                        tip[i + "Tip"] = this.props[i + "Tip"];
                        tipclassname[i + "classname"] = this.props[i + "classname"];
                    }
                    else {
                        tip[i + "Tip"] = "请填写正确的邮箱地址！";
                        tipclassname[i + "classname"] = 'help-inline tips';
                        objx[i] = {
                            'isValueMandatory': 1,
                            'isValueNotEmpty': 1,
                            'isValueUseAble': 0
                        }
                    }

                }
                else {
                    objx[i] = {
                        'isValueMandatory': 1,
                        'isValueNotEmpty': 0,
                        'isValueUseAble': 0
                    }
                    tip[i + "Tip"] = "请填写正确的邮箱地址！";
                    tipclassname[i + "classname"] = 'help-inline tips';
                }
                if (hasNotEdit == true) {
                    tip[i + "Tip"] = this.props[i + "Tip"];
                    tipclassname[i + "classname"] = this.props[i + "classname"];
                }



            }
            else if (i == "CustomerName") {
                if (obj[i]) {
                    if (this.isChineseChar(obj[i])) {
                        objx[i] = {
                            'isValueMandatory': 1,
                            'isValueNotEmpty': 1,
                            'isValueUseAble': 1
                        }
                        tip[i + "Tip"] = this.props[i + "Tip"];
                        tipclassname[i + "classname"] = this.props[i + "classname"];
                    }
                    else {
                        objx[i] = {
                            'isValueMandatory': 1,
                            'isValueNotEmpty': 1,
                            'isValueUseAble': 0
                        }
                        tip[i + "Tip"] = "请检查姓名（中文）";
                        tipclassname[i + "classname"] = 'help-inline tips';
                    }

                }
                else {
                    objx[i] = {
                        'isValueMandatory': 1,
                        'isValueNotEmpty': 0,
                        'isValueUseAble': 0
                    }
                    tip[i + "Tip"] = "请填写正确的姓名（中文）";
                    tipclassname[i + "classname"] = 'help-inline tips';
                }

                if (hasNotEdit == true) {
                    tip[i + "Tip"] = this.props[i + "Tip"];
                    tipclassname[i + "classname"] = this.props[i + "classname"];
                }
            }
            else if (i == "CustomerEnname") {
                if (obj[i]) {
                    if (this.isEnglishChar(obj[i])) {
                        objx[i] = {
                            'isValueMandatory': 1,
                            'isValueNotEmpty': 1,
                            'isValueUseAble': 1
                        }
                        tip[i + "Tip"] = this.props[i + "Tip"];
                        tipclassname[i + "classname"] = this.props[i + "classname"];
                    }
                    else {
                        objx[i] = {
                            'isValueMandatory': 1,
                            'isValueNotEmpty': 1,
                            'isValueUseAble': 0
                        }
                        tip[i + "Tip"] = "请检查姓名（拼音）";
                        tipclassname[i + "classname"] = 'help-inline tips';
                    }

                }
                else {
                    objx[i] = {
                        'isValueMandatory': 1,
                        'isValueNotEmpty': 0,
                        'isValueUseAble': 0
                    }
                    tip[i + "Tip"] = "请填写正确的姓名（拼音）";
                    tipclassname[i + "classname"] = 'help-inline tips';
                }

                if (hasNotEdit == true) {
                    tip[i + "Tip"] = this.props[i + "Tip"];
                    tipclassname[i + "classname"] = this.props[i + "classname"];
                }
            }
        }

        this.setState({
            valueState: objx,
            tip: tip,
            tipclassname: tipclassname
        }, function () {
            console.log(this.state)
        })

    },
    onCopy: function () {
        $(this.refs.copyUserInfoToClipboard).success("复制成功！")
    },
    renderButtons: function () {
        if (this.props.buttons && this.props.buttons.copyToClipboard === true) {
            return (
                <div className="panel-heading">
                    <span className="help-inline" style={{ float: 'right' }}>
                        <CopyToClipboard key="CopyToClipboardWrapper" text={this.state.copyStr} onCopy={this.onCopy}>
                            <a
                                ref="copyUserInfoToClipboard"
                                key="copyUserInfoToClipboard"
                                id={'copyUserInfoToClipboard'}
                                className="btn btn-sm btn-default button70"
                                role="button"
                                >
                                {"复制"}
                            </a>
                        </CopyToClipboard>
                    </span>
                </div>
            )
        } else {
            return null;
        }
    },
    renderQRCode:function(){
        if(this.props.imageUrl&&this.props.imageUrl!==""){
            return (
                <div className="text-center">
                 <div className="flex">
                    <img src={this.props.imageUrl} alt="等待二维码" />
                    <div className="text">微信扫码关注，绑定帐号后</div>
                    <div className="text">随时跟踪订单，查看确认单</div>
                </div>
                </div>
            )
        }else{
            return null;
        }
        
    },
    updateCopyStr: function () {
        var str = "";
        var state = this.state;
        str += "淘宝ID：" + this.props.code + "\n";
        str += "中文姓名：" + state.CustomerName + "\n";
        str += "姓名拼音：" + state.CustomerEnname + "\n";
        str += "联系电：" + state.Tel + "\n";
        str += "备用联系电话：" + state.BakTel + "\n";
        str += "Email地址：" + state.Email + "\n";
        str += "微-信：" + state.Wechat + "\n";
        this.setState({
            'copyStr': str
        })
    },
    render: function () {
        var baseinfo = this.state;
        var code = this.props.code;
        return (
            <div className="panel panel-default"  style={{ position: 'relative' }}>
                {
                    this.renderButtons()
                }
                {
                    this.renderQRCode()
                }
                <div id="baseinfo" className="panel-body form " >
                    <form className="form-horizontal" role="form">
                        <div className="form-body" >
                            <div className="form-group">
                                <label className="col-md-2 control-label">淘宝ID：</label>
                                <span id="CustomerTBCode" className="help-inline" style={{ marginLeft: '5px' }}>{code}</span>
                            </div>

                            <div className="form-group">
                                <label className="col-md-2 control-label"><span className="redspark">*</span>中文姓名：</label>
                                <div className="col-md-10">
                                    <input onBlur={this.handleblur} onChange={this.handlechange} ref="CustomerName" id="CustomerName" type="text" className="form-control input-inline input-sm input200" value={baseinfo.CustomerName} placeholder="张三" />
                                    <span className={baseinfo.tipclassname.CustomerNameclassname}>{baseinfo.tip.CustomerNameTip} </span>

                                </div>
                            </div>
                            <div className="form-group">
                                <label className="col-md-2 control-label"><span className="redspark">*</span>姓名拼音：</label>
                                <div className="col-md-10">
                                    <input onBlur={this.handleblur} onChange={this.handlechange} ref="CustomerEnname" id="CustomerEnname" type="text" className="form-control input-inline input-sm input200" value={baseinfo.CustomerEnname} placeholder="ZHANGSAN" />
                                    <span className={baseinfo.tipclassname.CustomerEnnameclassname}>{baseinfo.tip.CustomerEnnameTip} </span>
                                </div>
                            </div>
                            <div className="form-group">
                                <label className="col-md-2 control-label"><span className="redspark">*</span>联系电话：</label>
                                <div className="col-md-10">
                                    <input onBlur={this.handleblur} onChange={this.handlechange} ref="Tel" id="Tel" type="text" className="form-control input-inline input-sm input200" value={baseinfo.Tel} placeholder="务必填写国外能接通电话" />
                                    <span className={baseinfo.tipclassname.Telclassname}>{baseinfo.tip.TelTip} </span>
                                </div>
                            </div>
                            <div className="form-group">
                                <label className="col-md-2 control-label">备用联系电话：</label>
                                <div className="col-md-10">
                                    <input onBlur={this.handleblur} onChange={this.handlechange} ref="BakTel" id="BakTel" type="text" className="form-control input-inline input-sm input200" value={baseinfo.BakTel} placeholder="" />
                                    <span className={baseinfo.tipclassname.BakTelclassname}>{baseinfo.tip.BakTelTip} </span>
                                </div>
                            </div>
                            <div className="form-group">
                                <label className="col-md-2 control-label"><span className="redspark">*</span> Email地址：</label>
                                <div className="col-md-10">
                                    <input onBlur={this.handleblur} onChange={this.handlechange} ref="Email" id="Email" type="text" className="form-control input-inline input-sm input200" value={baseinfo.Email} placeholder="" />
                                    <span className={baseinfo.tipclassname.Emailclassname}>{baseinfo.tip.EmailTip} </span>
                                </div>
                            </div>
                            <div className="form-group">
                                <label className="col-md-2 control-label"><span className="redspark">*</span> 微信号：</label>
                                <div className="col-md-10">
                                    <input onBlur={this.handleblur} onChange={this.handlechange} ref="Wechat" id="Wechat" type="text" className="form-control input-inline input-sm input200" value={baseinfo.Wechat} placeholder="" />
                                    <span className={baseinfo.tipclassname.Wechatclassname}>{baseinfo.tip.WechatTip} </span>
                                </div>
                            </div>
                        </div>
                    </form>
                </div>
            </div>
        )
    }
})
