
var reactref = '';// 获取整个淘宝订单组件的引用
var postData = {}; //表单值
var personData = {};//已选人
var datePair = {}; //出行返回时间
var tempValue = {};//模板值
var systemFieldsMaps = new Object();
var valueSate = {}; //表单值是否合法
var valueReChange = {};

var valueExtraReChange = [];
var valueExtraOld = [];
var valueExtraReChangeDisplay = '';

var valueNameAndNum = {};
var valueOldnameAndNum = new Object();
const  reactx =
(function(){
    var Reactx;
    var a={};
    a.getALL =function(){
        return Reactx;
    }
    a.setALL =function(ref){
        Reactx =ref;
    }
    return a;
})();




// 映射判别各个组件
var rowsMap = {

    'DatePicker': DatePickerRow,
    'PersonPicker': PersonPickerRow,
    'FlightNO': FlightNoRow,
    'Hotel_area_name_tel': Hotel_name_zone_telRow,
    'Date_setoutdate_returndate': Date_setoutdate_returndateRow,
    'Flight_takeofftime_arrivaltime': Flight_takeofftime_arrivaltimeRow,
    'Flight_takeofftime_pickuptime': Flight_takeofftime_pickuptimeRow,
    'Destination': DestinationRow,
    'Hotelname': HotelNameRow,
    'BusRoute': BusRouteRow,
    'SelectMenu': SelectMenuRow,
    'TEXTinput': TEXTinputRow,
    'TimePicker': TimePickerRow,
    'Hotel_area_name_tel_address': Hotel_name_zone_telRow
};


/*
    对应子单
*/
var SubOrder = React.createClass({


    getInitialState: function () {
        var subOrder = this.props.subOrder;
        var AdultNum = subOrder.ServiceItemHistorys.AdultNum;
        var ChildNum = subOrder.ServiceItemHistorys.ChildNum;
        var INFNum = subOrder.ServiceItemHistorys.INFNum;
        var RoomNum = subOrder.ServiceItemHistorys.RoomNum;
        var RightNum = subOrder.ServiceItemHistorys.RightNum;
        var state = subOrder.state;
        var copyStr = '';
        var copyObj = {};
        //预排序
        var componentList = JSON.parse(this.props.subOrder.ServiceItemHistorys.Elements).elementList;
        var arrAfterSort = new Array();
        for (var i in componentList) {
            arrAfterSort.push({
                orderIndex: componentList[i].orderIndex,
                cID: i
            });
        }
        arrAfterSort.sort(function (a, b) {
            if (parseInt(a.orderIndex) < parseInt(b.orderIndex)) {
                return -1;
            }
            else {
                return 1;
            }
        })

        return {
            AdultNum: AdultNum,
            ChildNum: ChildNum,
            INFNum: INFNum,
            RightNum: RightNum,
            RoomNum: RoomNum,
            ExtraServiceHistorys: subOrder.ServiceItemHistorys.ExtraServiceHistorys,
            state: state,
            copyStr: copyStr,
            copyObj: copyObj,
            sortInfo: arrAfterSort,
            xc: componentList
        };
    },

    //更改按钮人数和额外项目的按钮
    rendersButtons: function (buttons, subOrder, state) {
        var extras = JSON.stringify(subOrder.ServiceItemHistorys.ExtraServiceHistorys);
        var personNum = ({
            'AdultNum': subOrder.ServiceItemHistorys.AdultNum,
            'ChildNum': subOrder.ServiceItemHistorys.ChildNum,
            'INFNum': subOrder.ServiceItemHistorys.INFNum,
        })
        var type = subOrder.ServiceItemHistorys.ServiceTypeID;
        if (type == 4) {
            personNum.RoomNum = subOrder.ServiceItemHistorys.RoomNum;
            personNum.RightNum = subOrder.ServiceItemHistorys.RightNum;
        }
        personNum = JSON.stringify(personNum);

        if (this.props.state == "editable") {
            var disabled = '';
            var revisePersonNumID = "revisePersonNum";
            var reviseExtrasID = "reviseExtras"

        }
        else {
            var disabled = 'disabled';
            var revisePersonNumID = "";
            var reviseExtrasID = ""
        }
        var buttonsList = [];
        if (buttons.copyToClipboard) {//对单利器
            var a =
                <CopyToClipboard key="CopyToClipboardWrapper" text={this.state.copyStr} onCopy={this.onCopy}>
                    <a
                        ref="copyToClipboard"
                        key="buttonCopyToClipboard"
                        id={'copyToClipboard'}
                        className="btn btn-sm btn-default button70"
                        style={{ 'marginRight': '5px' }}
                        role="button"
                        >
                        {"对单利器"}
                    </a>
                </CopyToClipboard>;
            buttonsList.push(a);
        }
        if (buttons.check) {//核对
            var a = "";
            if (this.state.state == 1) {
                a = <a href="javascript:" key="buttonCheck" ref="tocheck" id="tocheck" data-orderid={subOrder.OrderID} style={{ 'marginRight': '5px' }} className="status-green  btn btn-default btn-sm button70" data-next-code="0">核对</a>;
            }
            else {
                a = <a href="javascript:" key="buttonCheck" id="checked" data-orderid={subOrder.OrderID} style={{ 'marginRight': '5px' }} className=" disabled   btn btn-default btn-sm button70" data-next-code="0">核对</a>;
            }
            buttonsList.push(a);
        }
        if (buttons.toEdit) {//跳转编辑页面
            buttonsList.push(
                <a
                    data-state={subOrder.state}
                    key="buttonEdit"
                    target="_blank"
                    href={"/Orders/Edit/" + subOrder.OrderID}
                    className="btn btn-sm btn-default button70"
                    >
                    修改
                 </a>
            )
        }
        if (buttons.extraEdit) {//改项目
            buttonsList.push(
                <a
                    ref="reviseExtras"
                    role="button"
                    key="buttonReviseExtras"
                    disabled={disabled}
                    data-supplierid={subOrder.ServiceItemHistorys.SupplierID}
                    data-serviceitemid={subOrder.ServiceItemHistorys.ServiceItemID}
                    id={reviseExtrasID}
                    data-toggle="modal"
                    data-extras={extras}
                    href="#extrasModal"
                    style={{ 'marginRight': '5px' }}
                    className="btn btn-sm btn-default button70"
                    >
                    改项目
                </a>
            );
        }
        if (buttons.personNumEdit) {//改人数
            buttonsList.push(
                <a
                    ref="revisePersonNum"
                    key="buttonRevisePersonNum"
                    disabled={disabled}
                    id={revisePersonNumID}
                    data-type={1}
                    data-personnum={personNum}
                    href="javascript:;"
                    className="btn btn-sm btn-default button70"
                    >
                    改人数
                </a>
            )
        }
        if (buttons.clone) {//换产品
            buttonsList.push(
                <a
                    ref="xx"
                    id="order-clone"
                    key="buttonClone"
                    data-toggle="modal"
                    data-target="#order-clone-modal"
                    style={{ 'marginLeft': '5px;z-index:40000' }}
                    href="javascript:;"
                    className="btn btn-sm btn-default button70"
                    >
                    换产品
                </a>
            )
        }

        return (
            <span style={{ float: "right", 'lineHeight': '30px' }}>
                {buttonsList}
            </span>
        )
    },
    renderRivseExtraServicePerson: function (isForCusClient, subOrder, state) {
        if (!isForCusClient) {
            if (state === "anyview") {
                var a = "";
                if (this.state.state == 1) {
                    a = <a href="javascript:" ref="tocheck" id="tocheck" data-orderid={subOrder.OrderID} style={{ 'marginRight': '5px' }} className="status-green  btn btn-default btn-sm button70" data-next-code="0">核对</a>;
                }
                else {
                    a = <a href="javascript:" id="checked" data-orderid={subOrder.OrderID} style={{ 'marginRight': '5px' }} className=" disabled   btn btn-default btn-sm button70" data-next-code="0">核对</a>;
                }
                return (

                    <span id="oneOrderOP" style={{ 'float': "right", 'lineHeight': '30px' }}>
                        {a}
                        <a data-state={subOrder.state} target="_blank" href={"/Orders/Edit/" + subOrder.OrderID} className="btn btn-sm btn-default button70">修改</a>
                    </span>
                )
            }
            var extras = JSON.stringify(subOrder.ServiceItemHistorys.ExtraServiceHistorys);
            var personNum = ({
                AdultNum: subOrder.ServiceItemHistorys.AdultNum,
                ChildNum: subOrder.ServiceItemHistorys.ChildNum,
                INFNum: subOrder.ServiceItemHistorys.INFNum,

            })
            var type = subOrder.ServiceItemHistorys.ServiceTypeID
            if (type == 4) {
                personNum.RoomNum = subOrder.ServiceItemHistorys.RoomNum;
                personNum.RightNum = subOrder.ServiceItemHistorys.RightNum;
            }
            personNum = JSON.stringify(personNum);

            if (this.props.state == "editable") {
                var disabled = '';
                var revisePersonNumID = "revisePersonNum";
                var reviseExtrasID = "reviseExtras"

            }
            else {
                var disabled = 'disabled';
                var revisePersonNumID = "";
                var reviseExtrasID = ""
            }

            return (
                <span style={{ float: "right", 'lineHeight': '30px' }}>
                    <a ref="reviseExtras" role="button" disabled={disabled} data-supplierid={subOrder.ServiceItemHistorys.SupplierID} data-serviceitemid={subOrder.ServiceItemHistorys.ServiceItemID} id={reviseExtrasID} data-toggle="modal" data-extras={extras} href="#extrasModal" style={{ 'marginRight': '5px' }} className="btn btn-sm btn-default button70">改项目</a>
                    <a ref="revisePersonNum" disabled={disabled} id={revisePersonNumID} data-type={1} data-personnum={personNum} href="javascript:;" className="btn btn-sm btn-default button70">
                        改人数</a>
                    <a ref="xx" id="order-clone" data-toggle="modal" data-target="#order-clone-modal" style={{ 'marginLeft': '5px;z-index:40000' }} href="javascript:;" className="btn btn-sm btn-default button70">
                        换产品</a>
                </span>
            )
        }
    },
    renderRoom: function () {
        if (this.props.subOrder.ServiceItemHistorys.ServiceTypeID == 4) {
            return <span className="childnum">间数&nbsp;{this.state.RoomNum}&nbsp;&nbsp;</span>
        }
    },
    renderNight: function () {
        if (this.props.subOrder.ServiceItemHistorys.ServiceTypeID == 4) {
            return <span className="childnum">晚数&nbsp;{this.state.RightNum}&nbsp;&nbsp;</span>

        }
    },

    //已选额外项目提示
    renderExtrasInfo: function (ExtraServiceHistorys) {
        if (ExtraServiceHistorys) {
            var totalExtraNum = 0;
            var arr = new Array();
            for (var i in ExtraServiceHistorys) {
                totalExtraNum += ExtraServiceHistorys[i].ServiceNum;
                if (parseInt(ExtraServiceHistorys[i].ServiceNum) !== 0) {
                    arr.push(ExtraServiceHistorys[i].ServiceName + '：' + ExtraServiceHistorys[i].ServiceNum + ExtraServiceHistorys[i].ServiceUnit);
                }

            }
            var text = arr.join('，');
            if (totalExtraNum != 0) {
                return (
                    <div className="langhuatips">
                        【已选项目】
                        <span className="">
                            {text}
                        </span>

                    </div>
                )
            }
        }
    },

    //渲染组件列表
    renderComponentsList: function (componentList, lastValueList, suborderid, basePeopleNum, functions, limit, systemFieldsMap, ServiceItemID, fixedDays, isForCusClient, ServiceTypeID, Nights) {

        var arrComponentsAfterRender = new Array();


        // 调整组件顺序
        var arrComponentsAfterSort = new Array();
        for (var s in componentList) {
            componentList[s]['componentid'] = s;
            arrComponentsAfterSort.push(componentList[s]);
        }
        arrComponentsAfterSort.sort(function (a, b) {
            if (parseInt(a.orderIndex) < parseInt(b.orderIndex)) {
                return -1;
            }
            else {
                return 1;
            }
        });
        var cost = 0;
        for (var i in arrComponentsAfterSort) {

            if (lastValueList == 'initempty') {
                var initdatastart = "emptyrow";
            }
            else {
                if (lastValueList[arrComponentsAfterSort[i]['componentid']]) {
                    var initdatastart = (lastValueList[arrComponentsAfterSort[i]['componentid']]);
                }
                else {
                    var initdatastart = "emptyrow";
                }
            }

            // if(arrComponentsAfterSort[i].type=='DatePicker'){
            //     continue;
            // }
            // if(arrComponentsAfterSort[i].type=="PersonPicker"){
            //     continue;
            // }
            // if(arrComponentsAfterSort[i].type=='FlightNO'){
            //     continue;
            // }
            // if(arrComponentsAfterSort[i].type=='Hotel_area_name_tel'){
            //     continue;
            // }
            // if(arrComponentsAfterSort[i].type=='Date_setoutdate_returndate'){
            //     continue;
            // }

            //  if(arrComponentsAfterSort[i].type=='Flight_takeofftime_arrivaltime'){
            //     continue;
            // }
            //  if(arrComponentsAfterSort[i].type=='Flight_takeofftime_pickuptime'){
            //     continue;
            // }

            //  if(arrComponentsAfterSort[i].type=='Destination'){
            //     continue;
            // }
            // if(arrComponentsAfterSort[i].type=='Hotelname'){
            //     continue;
            // }

            // if(arrComponentsAfterSort[i].type=='BusRoute'){
            //     continue;
            // }

            //  if(arrComponentsAfterSort[i].type=='SelectMenu'){
            //     continue;
            // }

            // if(arrComponentsAfterSort[i].type=='TEXTinput'){
            //     continue;
            // }

            // if(arrComponentsAfterSort[i].type=='TimePicker'){
            //     continue;
            // }
            //  if(arrComponentsAfterSort[i].type=='Hotel_area_name_tel_address'){
            //     continue;
            // }
            if (arrComponentsAfterSort[i].type == 'Date_setoutdate_returndate') {
                if ('ServiceDate' in systemFieldsMap) {
                    cost = 1;
                    continue;
                }
            }
            var id = arrComponentsAfterSort[i]['componentid'];
            arrComponentsAfterRender.push(

                React.createElement(
                    rowsMap[arrComponentsAfterSort[i]["type"]],
                    {
                        key: id,
                        id: id,
                        ref: id,
                        type:arrComponentsAfterSort[i]["type"],
                        allLength:arrComponentsAfterSort.length-cost,

                        orderid: id,
                        postKey: id,

                        componentid: id,
                        suborderid: suborderid,
                        serviceItemID: ServiceItemID,

                        data: arrComponentsAfterSort[i],
                        initdata: initdatastart,

                        UIType: "edit",
                        limit: limit,
                        fixedDays: fixedDays,
                        ServiceTypeID: ServiceTypeID,


                        basePeopleNum: parseInt(this.state.AdultNum) + parseInt(this.state.ChildNum) + parseInt(this.state.INFNum),
                        UpdateDataToForm: functions.recievedata,
                        recievePeson: functions.recievePeson,
                        recieveDatePair: functions.recieveDatePair,
                        reciveTempValue: functions.reciveTempValue,
                        reciveValueState: functions.reciveValueState,
                        recieveSystemFields: functions.recieveSystemFields,
                        isForCusClient: isForCusClient,
                        copyToClipboard: this.props.buttons.copyToClipboard,
                        updateCopyObjx: this.formatCopyObj2Str,
                        Nights: Nights,
                    }
                )
            )

        }
        return arrComponentsAfterRender;
    },
    renderCover: function () {
        // if(this.props.state=="rechangeable"){
        //     return <div className="full-cover"></div>
        // }
        // else {
        //     return null
        // }
        return null;
    },

    render: function () {
        var subOrder = this.props.subOrder;
        var state = this.props.state;

        var areas = this.props.areas;
        var isForCusClient = this.props.isForCusClient;
        var functions = this.props.functions;

        //组件最后一次的值
        if (subOrder.ServiceItemHistorys.ElementsValue == null) {
            var lastValue = 'initempty';
        }
        else {
            var lastValue = JSON.parse(subOrder.ServiceItemHistorys.ElementsValue)
        }
        var basePeopleNum =
            parseInt(this.state.AdultNum) +
            parseInt(this.state.ChildNum) +
            parseInt(this.state.INFNum);

        var Nights = subOrder.ServiceItemHistorys.RightNum;
        var limit = this.props.limit;
        var buttons = this.props.buttons;

        return (
            <div className="panel panel-default oneSuborder" key={"suborder" + subOrder.OrderID} ref={"suborder" + subOrder.OrderID} id={"suborder" + subOrder.OrderID} data-customerid={subOrder.CustomerID}>
                <div className="panel-heading">
                    <span className="servicecnname bold" style={{ 'lineHeight': '30px' }}>{subOrder.ServiceItemHistorys.cnItemName}&nbsp;&nbsp;&nbsp;&nbsp;</span>
                    <span className=" visible-lg-inline-block visible-xs-block visible-md-inline-block visible-sm-inline-block" >
                        <span className="adultnum">成人&nbsp;{this.state.AdultNum}&nbsp;&nbsp;</span>
                        <span className="childnum">儿童&nbsp;{this.state.ChildNum}&nbsp;&nbsp;</span>
                        <span className="">婴儿&nbsp;{this.state.INFNum}&nbsp;&nbsp;</span>
                        {this.renderRoom()}
                        {this.renderNight()}
                    </span>
                    {
                        this.rendersButtons(this.props.buttons, subOrder, state)
                    }
                </div>
                <div className="panel-body form" style={{ 'position': 'relative' }} >
                    {
                        this.renderExtrasInfo(this.state.ExtraServiceHistorys)
                    }
                    {
                        this.renderComponentsList(
                            JSON.parse(subOrder.ServiceItemHistorys.Elements).elementList,
                            lastValue,
                            subOrder.OrderID,
                            basePeopleNum,
                            functions,
                            limit,
                            JSON.parse(subOrder.ServiceItemHistorys.Elements).systemFieldsMap,
                            subOrder.ServiceItemHistorys.ServiceItemID,
                            subOrder.ServiceItemHistorys.FixedDays,
                            isForCusClient,
                            subOrder.ServiceItemHistorys.ServiceTypeID,
                            Nights
                        )
                    }
                    {this.renderCover()}

                </div>
            </div>
        )
    },


    //接受人数和额外项目的更新
    componentDidMount: function () {
        var _thisOrder = this;
        jQuery(_thisOrder.refs.revisePersonNum).bind("update", function (e, data) {
            _thisOrder.setState(data,function(){
                _thisOrder.updatetCopyObj2Str();
            })
        })
        jQuery(_thisOrder.refs.reviseExtras).bind("update", function (e, data) {
            _thisOrder.setState({
                ExtraServiceHistorys: data
            },function(){
                _thisOrder.updatetCopyObj2Str();
            })
        })
        jQuery(_thisOrder.refs.tocheck).bind("update", function (e, state) {
            _thisOrder.setState({
                'state': state
            })
        })
    },
    formatCopyObj2Str: function (obj) {
        var copyObj = this.state.copyObj;
        var cid, code;
        for (var cid in obj) {
            copyObj[cid] = obj[cid];
        }
        this.setState({ 'copyObj': copyObj }, function () {
            this.updatetCopyObj2Str();
        });
    },
    updatetCopyObj2Str: function () {
        var i, j, cid, code;
        var arrMeaningfulExtra = [];
        var copyObj = this.state.copyObj;
        var sortInfo = this.state.sortInfo;
        var extraList = this.state.ExtraServiceHistorys;
        var copyStr = "";
        copyStr += "【" + this.props.subOrder.ServiceItemHistorys.cnItemName + "】的资料请您核对一下哦！\n";
        for (j in extraList) {
            if (extraList[j].ServiceNum != 0) {
                arrMeaningfulExtra.push(extraList[j].ServiceName + "：" + extraList[j].ServiceNum + extraList[j].ServiceUnit);
            }
        }
        var extraStr = arrMeaningfulExtra.join("，");
        if (extraStr) {
            copyStr += "【已选项目】" + extraStr + '\n';
        }
        copyStr += "  成人：" + this.state.AdultNum + "人，儿童：" + this.state.ChildNum + "人，婴儿：" + this.state.INFNum + "人\n";
        for (i in sortInfo) {
            cid = sortInfo[i].cID;
            if (cid in copyObj) {
                for (code in copyObj[cid]) {
                    copyStr += "  " + copyObj[cid][code].title + '：' + copyObj[cid][code].str + '\n';
                }
            }
        }
        this.setState({
            'copyStr': copyStr
        });
    },
    onCopy:function(){
        $(this.refs.copyToClipboard).success("复制成功！")
    }
});


/*
    对应整个淘宝订单
*/
var TBOrders = React.createClass({


    componentDidMount: function () {
        var _this = this;
        $(this.refs.all).bind("forceupdate", function () {
            _this.forceUpdate();
        })
        $('#clientOrderLoading').remove();
    },

    //接收数据更新（表单字段的值）
    receiveDataUpdate: function (data) {
        if (this.props.state == "rechangeable") {
            return;
        }
        // 无法使用扩展
        var temp = postData;
        for (var i in data) {
            if (!(temp instanceof Object)) {
                temp = {};
            }
            for (var j in data[i]) {
                if (!(temp[i] instanceof Object)) {
                    temp[i] = {};
                }
                for (var k in data[i][j]) {
                    if (!(temp[i][j] instanceof Object)) {
                        temp[i][j] = {};
                    }
                    temp[i][j][k] = data[i][j][k];
                }
            }
        }
        postData = temp;
    },

    //接收数据更新（所有的出行旅客）
    recievePeson: function (data) {
        if (this.props.state == "rechangeable") {
            return;
        }
        for (var i in data) {
            if (!(i in personData)) {
                personData[i] = data[i];
                continue;
            }
            else {
                for (var j in data[i]) {
                    personData[i][j] = data[i][j];
                }
            }
        }
    },

    //接收数据更新（出行和返回日期）
    recieveDatePair: function (data) {

        if (this.props.state == "rechangeable") {
            return;
        }
        for (var i in data) {
            datePair[i] = data[i];
        }
    },
    recieveSystemFields: function (data) {
        if (this.props.state == "rechangeable") {
            return;
        }
        for (var i in data) {
            if (!(i in systemFieldsMaps)) {
                systemFieldsMaps[i] = new Object();
            }
            for (var j in data[i]) {
                systemFieldsMaps[i][j] = data[i][j];
            }
        }
    },

    // 接收数据更新（用于订单详情的模板的值）
    reciveTempValue: function (data) {

        if (this.props.state == "rechangeable") {
            return;
        }

        // 无法使用扩展
        for (var i in data) {

            for (var j in data[i]) {
                if (!(tempValue[i] instanceof Object)) {
                    tempValue[i] = {};
                }
                tempValue[i][j] = data[i][j]

            }
        }





        //  for(var i in data){
        //      tempValue[i]= data[i];
        //  }
    },

    // 接收数据更新（用于订单详情的模板的值）
    reciveValueState: function (data) {
        // alert(JSON.stringify(data));
        if (this.props.state == "rechangeable") {
            return;
        }

        for (var i in data) {

            for (var j in data[i]) {
                if (!(valueSate[i] instanceof Object)) {
                    valueSate[i] = {};
                }
                valueSate[i][j] = data[i][j]

            }
        }
    },

    // 判断是否可以提交（尚未启用）
    updateSubmitStatus: function () {

        var postdata = this.state.postData;
        for (var i in postdata) {
            for (var j in postdata[i]) {
                if (postdata[i][j] == this.props.invalidtag) {
                    this.setState({
                        postAble: false,
                    });
                    return;
                }
            }
        }
    },
    renderCover: function () {
        if (this.props.state == "rechangeable") {
            return <div className="full-cover"></div>
        }
        else {
            return null
        }
    },


    //渲染
    render: function () {

        var subOrderList = this.props.subOrderList;
        var isForCusClient = this.props.isForCusClient;
        var functions = {
            recievedata: this.receiveDataUpdate,
            recievePeson: this.recievePeson,
            recieveDatePair: this.recieveDatePair,
            reciveTempValue: this.reciveTempValue,
            reciveValueState: this.reciveValueState,
            recieveSystemFields: this.recieveSystemFields

        }

        var state = this.props.state;
        var limit = this.props.limit;
        var buttons = this.props.buttons;
        return (
            <form ref="all" id="reactall" className="form-horizontal" role="form" style={{ "position": "relative" }}>
                {
                    subOrderList.map(function (subOrder, index) {
                        var suborderid = subOrder.OrderID;
                        return (
                            < SubOrder
                                key={suborderid}// 前三个用于html和react的 引用功能 
                                id={suborderid}
                                ref={suborderid}

                                subOrderId={"suborder" + suborderid}
                                subOrder={subOrder}
                                isForCusClient={isForCusClient}
                                functions={functions}
                                state={state}
                                limit={limit}
                                buttons={buttons}
                                />
                        )
                    })

                }

            </form>
        )
    }

})


//求情变更子单
var RechangeSubOrder = React.createClass({

    getInitialState: function () {
        var subOrder = this.props.subOrder;
        var AdultNum = subOrder.ServiceItemHistorys.AdultNum;
        var ChildNum = subOrder.ServiceItemHistorys.ChildNum;
        var INFNum = subOrder.ServiceItemHistorys.INFNum;
        var AdultNumR = '';
        var ChildNumR = '';
        var INFNumR = '';

        if (subOrder.ServiceItemHistorys.ChangeValue == null) {
            var extras = (subOrder.ServiceItemHistorys.ExtraServiceHistorys);
            var ExtraReChangeDisplay = '';

            //
            var nameAndNum = new Object();
            nameAndNum.AdultNum = AdultNum;
            nameAndNum.ChildNum = ChildNum;
            nameAndNum.INFNum = INFNum;

            AdultNumR = AdultNum;
            ChildNumR = ChildNum;
            INFNumR = INFNum;

            nameAndNum.CustomerName = subOrder.CustomerName;
            nameAndNum.CustomerEnname = subOrder.CustomerEnname;
            nameAndNum.Tel = subOrder.Tel;
            nameAndNum.BakTel = subOrder.BakTel;
            nameAndNum.Email = subOrder.Email;
            nameAndNum.Wechat = subOrder.Wechat;

            if (subOrder.ServiceItemHistorys.ServiceTypeID == 4) {
                nameAndNum.RightNum = subOrder.ServiceItemHistorys.RightNum;
                nameAndNum.RoomNum = subOrder.ServiceItemHistorys.RoomNum;
            }



        }
        else {
            var extras = (JSON.parse(subOrder.ServiceItemHistorys.ChangeValue)['extra']);
            var ExtraReChangeDisplay = (JSON.parse(subOrder.ServiceItemHistorys.ChangeValue)['extraDisplay']);

            var nameAndNum = (JSON.parse(subOrder.ServiceItemHistorys.ChangeValue)['nameAndNum']);

            AdultNumR = nameAndNum.AdultNum;
            ChildNumR = nameAndNum.ChildNum;
            INFNumR = nameAndNum.INFNum;
        }
        var oldnameAndNum = new Object();
        oldnameAndNum.AdultNum = AdultNum;
        oldnameAndNum.ChildNum = ChildNum;
        oldnameAndNum.INFNum = INFNum;
        oldnameAndNum.CustomerName = subOrder.CustomerName;
        oldnameAndNum.CustomerEnname = subOrder.CustomerEnname
        oldnameAndNum.Tel = subOrder.Tel;
        oldnameAndNum.BakTel = subOrder.BakTel;
        oldnameAndNum.Email = subOrder.Email;
        oldnameAndNum.Wechat = subOrder.Wechat;
        if (subOrder.ServiceItemHistorys.ServiceTypeID == 4) {
            oldnameAndNum.RightNum = subOrder.ServiceItemHistorys.RightNum;
            oldnameAndNum.RoomNum = subOrder.ServiceItemHistorys.RoomNum;
        }




        return {
            AdultNum: AdultNum,
            ChildNum: ChildNum,
            INFNum: INFNum,
            AdultNumR: AdultNumR,
            ChildNumR: ChildNumR,
            INFNumR: INFNumR,
            ExtraServiceHistorys: extras,
            valueExtraOld: subOrder.ServiceItemHistorys.ExtraServiceHistorys,
            ExtraReChangeDisplay: ExtraReChangeDisplay,
            nameAndNum: nameAndNum,
            oldnameAndNum: oldnameAndNum
        };
    },
    renderComponentsList: function (componentList, lastValueList, willChangeValueList, suborderid, basePeopleNum, functions, systemFieldsMap, serviceItemID, fixedDays, isForCusClient, ServiceTypeID, Nights) {
        var arrComponentsAfterRender = new Array();
        // 调整组件顺序
        var arrComponentsAfterSort = new Array();
        for (var s in componentList) {
            componentList[s]['componentid'] = s;
            arrComponentsAfterSort.push(componentList[s]);
        }
        arrComponentsAfterSort.sort(function (a, b) {
            if (a.orderIndex < b.orderIndex) {
                return -1;
            }
            else {
                return 1;
            }
        });
        var cost = 0;

        for (var i in arrComponentsAfterSort) {

            if (lastValueList == 'initempty') {
                var initdatastart = "emptyrow";
            }
            else {
                var initdatastart = (lastValueList[arrComponentsAfterSort[i]['componentid']]);
            }

            if (willChangeValueList == 'initempty') {//首次请求变更
                var willChangeValue = initdatastart;
            }
            else {//非首次请求变更
                var willChangeValue = (willChangeValueList['formElemnents'][suborderid][arrComponentsAfterSort[i]['componentid']]);//只考虑了一单的情况故而需要子单的ID
            }







            if (arrComponentsAfterSort[i].type == 'Date_setoutdate_returndate') {
                if ('ServiceDate' in systemFieldsMap) {
                    cost = 1;
                    continue;
                }
            }


            var id = arrComponentsAfterSort[i]['componentid'];

            arrComponentsAfterRender.push(

                React.createElement(
                    rowsMap[arrComponentsAfterSort[i]["type"]],
                    {
                        key: id,
                        id: id,
                        ref: id,
                        type:arrComponentsAfterSort[i]["type"],
                        allLength:arrComponentsAfterSort.length-cost,

                        orderid: id,
                        postKey: id,

                        componentid: id,
                        suborderid: suborderid,
                        serviceItemID: serviceItemID,
                        ServiceTypeID: ServiceTypeID,

                        data: arrComponentsAfterSort[i],
                        initdata: willChangeValue,
                        willChangeValue: willChangeValue,
                        beforeRechangeValue: initdatastart,


                        UIType: 'reChange',
                        fixedDays: fixedDays,

                        basePeopleNum: basePeopleNum,
                        UpdateDataToForm: functions.recievedata,
                        recievePeson: functions.recievePeson,
                        recieveDatePair: functions.recieveDatePair,
                        reciveTempValue: functions.reciveTempValue,
                        reciveValueState: functions.reciveValueState,
                        reciveValueChange: functions.reciveValueChange,
                        recieveSystemFields: functions.recieveSystemFields,
                        isForCusClient: isForCusClient,
                        Nights: Nights
                    }
                )
            )

        }
        return arrComponentsAfterRender;
        // return null;
    },
    renderExtrasInfo: function (ExtraServiceHistorys) {
        if (ExtraServiceHistorys) {
            var totalExtraNum = 0;
            var arr = [];
            console.log(ExtraServiceHistorys)
            for(var i in ExtraServiceHistorys){
                totalExtraNum+=parseInt(ExtraServiceHistorys[i].ServiceNum);
                if(parseInt(ExtraServiceHistorys[i].ServiceNum)!==0){
                    arr.push( ExtraServiceHistorys[i].ServiceName + '：' + ExtraServiceHistorys[i].ServiceNum + ExtraServiceHistorys[i].ServiceUnit)
                }
            }
            var text =arr.join('， ');
            if (totalExtraNum != 0) {
                return (
                    <div className="">
                        <span className="visible-lg-inline-block visible-xs-block visible-md-inline-block visible-sm-inline-block">
                            {text}
                        </span>

                    </div>
                )
            }
        }
    },

    renderRivseExtraService: function (subOrder) {
        var _this = this;
        if (subOrder.ServiceItemHistorys.ChangeValue == null) {
            var extras = JSON.stringify(subOrder.ServiceItemHistorys.ExtraServiceHistorys);
        }
        else {
            var extras = JSON.stringify(JSON.parse(subOrder.ServiceItemHistorys.ChangeValue)['extra']);
        }

        var extraOld = JSON.stringify(subOrder.ServiceItemHistorys.ExtraServiceHistorys)
        return (
            <div>
                <a ref="reviseExtras" id="reviseExtrasRechange" data-serviceitemid={subOrder.ServiceItemHistorys.ServiceItemID} data-supplierid={subOrder.ServiceItemHistorys.SupplierID} data-toggle="modal" data-extra-old={extraOld} data-extras={extras} ref="reviseExtrasR" style={{ 'marginRight': '5px' }} className="btn btn-sm btn-default button70">改项目</a>
                <span>
                    {(function () {
                        if (_this.state.ExtraReChangeDisplay != '') {
                            return _this.state.ExtraReChangeDisplay.changeAltCN;
                        }
                        else {
                            return '';
                        }

                    })()}
                </span>
            </div>
        )
    },
    renderRoom: function (subOrder, nameAndNumx) {
        if (subOrder.ServiceItemHistorys.ServiceTypeID == 4) {
            return (
                <tr>
                    <td>间数</td>
                    <td>{subOrder.ServiceItemHistorys.RoomNum}</td>
                    <td>

                        <input id="RoomNum" ref="RoomNum" type="text" data-for='RoomNum' defaultValue={nameAndNumx['RoomNum']} onBlur={this.changeHandle} className="form-control input-inline input-medium" />
                        <div><span class={this.state.RoomNumclassname}>{this.state.RoomNumtips}</span></div>
                    </td>
                </tr>
            )
        }
    },
    renderNight: function (subOrder, nameAndNumx) {
        if (subOrder.ServiceItemHistorys.ServiceTypeID == 4) {
            return (
                <tr>
                    <td>晚数</td>
                    <td>{subOrder.ServiceItemHistorys.RightNum}</td>
                    <td>

                        <input id="RightNum" ref="RightNum" type="text" data-for='RightNum' defaultValue={nameAndNumx['RightNum']} onBlur={this.changeHandle} className="form-control input-inline input-medium" />
                        <div><span class={this.state.RightNumclassname}>{this.state.RightNumtips}</span></div>

                    </td>
                </tr>
            )
        }
    },

    render: function () {

        var subOrder = this.props.subOrder;

        var isForCusClient = this.props.isForCusClient;
        var functions = this.props.functions;

        //组件最后一次的值
        if (subOrder.ServiceItemHistorys.ElementsValue == null) {
            var lastValue = 'initempty';
        }
        else {
            var lastValue = JSON.parse(subOrder.ServiceItemHistorys.ElementsValue)
        }
        //请求变更暂存值
        if (subOrder.ServiceItemHistorys.ChangeValue == null) {
            var willChangeValue = 'initempty';
        }
        else {
            var willChangeValue = JSON.parse(subOrder.ServiceItemHistorys.ChangeValue)
        }
        var basePeopleNum =
            parseInt(this.state.AdultNumR) +
            parseInt(this.state.ChildNumR) +
            parseInt(this.state.INFNumR);


        var oldnameAndNumx = this.state.oldnameAndNum;
        var nameAndNumx = this.state.nameAndNum;
        for (var i in oldnameAndNumx) {
            if (oldnameAndNumx[i] == nameAndNumx[i]) {
                nameAndNumx[i] = '';
            }
        }

        var Nights = subOrder.ServiceItemHistorys.RightNum;


        return (

            <table className="table reChangeTable table-bordered oneSuborder" data-customerid={subOrder.CustomerID}>
                <thead>
                    <tr>
                        <th style={{ "width": "165px", 'maxWidth': "165px", 'minWidth': '165px' }}>变更项目</th>
                        <th style={{ "width": "165px" }}>原内容</th>
                        <th style={{ "width": "280px" }}>变更后内容</th>
                    </tr>
                </thead>
                <tbody>
                    <tr>
                        <td><span className="redspark">*</span>联系人姓名（中文）</td>
                        <td>{subOrder.CustomerName}</td>
                        <td><input id="CustomerName" ref="CustomerName" type="text" data-for='cnName' defaultValue={nameAndNumx['CustomerName']} onBlur={this.changeHandle} className="form-control input-inline input-medium" /></td>
                    </tr>
                    <tr>
                        <td><span className="redspark">*</span>联系人姓名（英文）</td>
                        <td>{subOrder.CustomerEnname}</td>
                        <td><input id="CustomerEnname" ref="CustomerEnname" type="text" data-for='enName' defaultValue={nameAndNumx['CustomerEnname']} onBlur={this.changeHandle} className="form-control input-inline input-medium" /></td>
                    </tr>

                    <tr>
                        <td><span className="redspark">*</span>联系电话</td>
                        <td>{subOrder.Tel}</td>
                        <td><input id="Tel" ref="Tel" type="text" data-for='Tel' defaultValue={nameAndNumx['Tel']} onBlur={this.changeHandle} className="form-control input-inline input-medium" /></td>
                    </tr>
                    <tr>
                        <td>备用电话</td>
                        <td>{subOrder.BakTel}</td>
                        <td><input id="BakTel" ref="BakTel" type="text" data-for='BakTel' defaultValue={nameAndNumx['BakTel']} onBlur={this.changeHandle} className="form-control input-inline input-medium" /></td>
                    </tr>
                    <tr>
                        <td><span className="redspark">*</span>Email地址</td>
                        <td>{subOrder.Email}</td>
                        <td><input id="Email" ref="Email" type="text" data-for='Email' defaultValue={nameAndNumx['Email']} onBlur={this.changeHandle} className="form-control input-inline input-medium" /></td>
                    </tr>
                    <tr>
                        <td><span className="redspark">*</span>微信号</td>
                        <td>{subOrder.Wechat}</td>
                        <td><input id="Wechat" ref="Wechat" type="text" data-for='Wechat' defaultValue={nameAndNumx['Wechat']} onBlur={this.changeHandle} className="form-control input-inline input-medium" /></td>
                    </tr>

                    <tr>
                        <td>成人</td>
                        <td>{subOrder.ServiceItemHistorys.AdultNum}</td>
                        <td><input id="AdultNum" ref="AdultNum" type="text" data-for='Adult' defaultValue={nameAndNumx['AdultNum']} onBlur={this.changeHandle} className="form-control input-inline input-medium" /></td>
                    </tr>
                    <tr>
                        <td>儿童</td>
                        <td>{subOrder.ServiceItemHistorys.ChildNum}</td>
                        <td><input id="ChildNum" ref="ChildNum" type="text" data-for='Child' defaultValue={nameAndNumx['ChildNum']} onBlur={this.changeHandle} className="form-control input-inline input-medium" /> </td>
                    </tr>
                    <tr>
                        <td>婴儿</td>
                        <td>{subOrder.ServiceItemHistorys.INFNum}</td>
                        <td><input id="INFNum" ref="INFNum" type="text" data-for='Infant' defaultValue={nameAndNumx['INFNum']} onBlur={this.changeHandle} className="form-control input-inline input-medium" /> </td>
                    </tr>
                    {
                        this.renderRoom(subOrder, nameAndNumx)
                    }
                    {
                        this.renderNight(subOrder, nameAndNumx)
                    }
                    <tr>
                        <td>附加项目</td>
                        <td>{this.renderExtrasInfo(subOrder.ServiceItemHistorys.ExtraServiceHistorys)}</td>
                        <td>{this.renderRivseExtraService(subOrder)}</td>
                    </tr>

                </tbody>
                {
                    this.renderComponentsList(
                        JSON.parse(subOrder.ServiceItemHistorys.Elements).elementList,
                        lastValue,
                        willChangeValue,
                        subOrder.OrderID,
                        basePeopleNum,
                        functions,
                        JSON.parse(subOrder.ServiceItemHistorys.Elements).systemFieldsMap,
                        subOrder.ServiceItemHistorys.ServiceItemID,
                        subOrder.ServiceItemHistorys.FixedDays,
                        this.props.isForCusClient,
                        subOrder.ServiceItemHistorys.ServiceTypeID,
                        Nights
                    )
                }
            </table>
        )
    },
    //接受人数和额外项目的更新
    componentDidMount: function () {
        var _thisOrder = this;
        valueExtraReChange = _thisOrder.state.ExtraServiceHistorys;
        valueExtraReChangeDisplay = _thisOrder.state.ExtraReChangeDisplay;
        jQuery(_thisOrder.refs.reviseExtrasR).bind("update", function (e, data, data2) {
            valueExtraReChange = data;
            valueExtraReChangeDisplay = data2;
            _thisOrder.setState({
                ExtraReChangeDisplay: data2
            })
        });

        // $(_thisOrder.refs.CustomerName).onlyChinese();
        // $(_thisOrder.refs.CustomerEnname).onlyCapchar();
        // $(_thisOrder.refs.AdultNum).onlyNumWithEmptyR();
        // $(_thisOrder.refs.ChildNum).onlyNumWithEmptyR();
        // $(_thisOrder.refs.INFNum).onlyNumWithEmptyR();


        valueNameAndNum = _thisOrder.state.nameAndNum;
        valueOldnameAndNum = _thisOrder.state.oldnameAndNum;

        valueExtraOld = _thisOrder.state.valueExtraOld;


    },
    changeHandle: function (e) {
        var id = $(e.target).attr('id');
        var value = $(e.target).val().trim();
        if (id == 'CustomerName') {
            var reg = /^[\u4e00-\u9fa5]+$/;
            if (reg.test(value)) {

            }
            else {
                $(e.target).val("");
            }
        }
        else if (id == 'CustomerEnname') {
            var reg = /^[A-Za-z ]+$/;
            if (reg.test(value)) {
                $(e.target).val(value.toUpperCase());
            }
            else {
                $(e.target).val("");
            }
        }
        else if (id == 'CustomerEnname') {
            var reg = /^[A-Za-z ]+$/;
            if (reg.test(value)) {
                $(e.target).val(value.toUpperCase());
            }
            else {
                $(e.target).val("");
            }
        }
        else if (id == "Tel") {
            var reg = /^[0-9+-]+$/;
            if (reg.test(value)) {
                $(e.target).val(value);
            }
            else {
                $(e.target).val("");
            }
        }
        else if (id == "BakTel") {

        }
        else if (id == 'Wechat') {
            //  var reg = /^[a-zA-Z\d_]{5,}$/;
            //  if (reg.test(value)) {
            //      $(e.target).val(value);
            //  }
            //  else {
            //      $(e.target).val("");
            //  }

            $(e.target).val(value);
        }
        else if (id == 'Email') {
            var reg = /^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$/;
            if (reg.test(value)) {
                $(e.target).val(value);
            }
            else {
                $(e.target).val("");
            }
        }
        else if (id == 'RightNum') {
            var reg = /^[0-9]+$/;
            if (reg.test(value)) {
                $(e.target).val(value);
                $(".forceUpdate-setoutreturn").trigger("update", [parseInt(value)]);
            }
            else {
                $(e.target).val("");

                $(".forceUpdate-setoutreturn").trigger("update", [parseInt(this.state.oldnameAndNum[$(e.target).attr('id')])]);
                //   if(
                //     $('#RoomNum').val()==''||
                //     $('#RoomNum').val()==undefined||
                //     $('#RoomNum').val()==null){
                //      this.setState({
                //         RoomNumclassname:"",
                //         RoomNumtips:""
                //      })
                //  }
                //  else{
                //       this.setState({
                //         RoomNumclassname:"help-inline tips",
                //         RoomNumtips:"请填写大于0晚数"
                //      })
                //  }
            }
        }

        else if (id == 'RoomNum') {
            var reg = /^[0-9]+$/;
            if (reg.test(value)) {
                $(e.target).val(value);
            }
            else {
                $(e.target).val("");
                //  if($('#RightNum').val()==''||$('#RightNum').val()==undefined||$('#RightNum').val()==null){
                //      this.setState({
                //         RightNumclassname:"",
                //         RightNumtips:""
                //      })
                //  }
                //  else{
                //       this.setState({
                //         RightNumclassname:"help-inline tips",
                //         RightNumtips:"请填写大于0间数"
                //      })
                //  }
            }
        }
        else {
            var reg = /^\s+$/;

            var numARR = new Array("AdultNum", 'ChildNum', 'INFNum');
            var thisID = [$(e.target).attr('id')];
            var total = 0;

            for (var i in numARR) {
                if (numARR[i] == thisID) {
                    continue;
                }
                if (!valueNameAndNum[numARR[i]]) {
                    total += (parseInt(valueOldnameAndNum[numARR[i]]));
                } else {
                    total += (parseInt(valueNameAndNum[numARR[i]]));

                }


            }
            if (reg.test(value)) {
                $(e.target).val('');
                total += this.state.oldnameAndNum[$(e.target).attr('id')];
            }
            else {
                if (!isNaN(value)) {
                    var last = value ? parseInt(value) : "";
                    $(e.target).val(last);

                    total += last;
                }
                else {
                    $(e.target).val('');
                    total += this.state.oldnameAndNum[$(e.target).attr('id')];


                }
            }
            $(".forceUpdate-peoplenum").trigger("update", [total]);
        }
        if ($(e.target).val() == '') {
            valueNameAndNum[$(e.target).attr('id')] = this.state.oldnameAndNum[$(e.target).attr('id')];
        }
        else {
            valueNameAndNum[$(e.target).attr('id')] = $(e.target).val();
        }
    }
})



// 请求变更整单
var RechangeOrder = React.createClass({
    //接收数据更新（表单字段的值）
    receiveDataUpdate: function (data) {
        // 无法使用扩展
        var temp = postData;
        for (var i in data) {
            if (!(temp instanceof Object)) {
                temp = {};
            }
            for (var j in data[i]) {
                if (!(temp[i] instanceof Object)) {
                    temp[i] = {};
                }
                for (var k in data[i][j]) {
                    if (!(temp[i][j] instanceof Object)) {
                        temp[i][j] = {};
                    }
                    temp[i][j][k] = data[i][j][k];
                }
            }
        }
        postData = temp;
    },
    //接收数据更新（所有的出行旅客）
    recievePeson: function (data) {
        for (var i in data) {
            if (!(i in personData)) {
                personData[i] = data[i];
                continue;
            }
            else {
                for (var j in data[i]) {
                    personData[i][j] = data[i][j];
                }
            }
        }
    },

    //接收数据更新（出行和返回日期）
    recieveDatePair: function (data) {
        for (var i in data) {
            datePair[i] = data[i];
        }
    },
    recieveSystemFields: function (data) {
        for (var i in data) {
            systemFieldsMaps[i] = data[i];
        }
    },

    // 接收数据更新（用于订单详情的模板的值）
    reciveTempValue: function (data) {
        // 无法使用扩展
        for (var i in data) {

            for (var j in data[i]) {
                if (!(tempValue[i] instanceof Object)) {
                    tempValue[i] = {};
                }
                tempValue[i][j] = data[i][j]

            }
        }
    },

    // 接收数据更新（用于订单详情的模板的值）
    reciveValueState: function (data) {

        for (var i in data) {

            for (var j in data[i]) {
                if (!(valueSate[i] instanceof Object)) {
                    valueSate[i] = {};
                }
                valueSate[i][j] = data[i][j]

            }
        }

    },

    reciveValueChange: function (data) {

        for (var i in data) {

            for (var j in data[i]) {
                if (!(valueReChange[i] instanceof Object)) {
                    valueReChange[i] = {};
                }
                valueReChange[i][j] = data[i][j]

            }
        }

    },

    // 判断是否可以提交（尚未启用）
    updateSubmitStatus: function () {

        var postdata = this.state.postData;
        for (var i in postdata) {
            for (var j in postdata[i]) {
                if (postdata[i][j] == this.props.invalidtag) {
                    this.setState({
                        postAble: false,
                    });
                    return;
                }
            }
        }
    },

    render: function () {
        var subOrder = this.props.oneSuborder;
        var functions = {
            recievedata: this.receiveDataUpdate,
            recievePeson: this.recievePeson,
            recieveDatePair: this.recieveDatePair,
            reciveTempValue: this.reciveTempValue,
            reciveValueState: this.reciveValueState,
            reciveValueChange: this.reciveValueChange,
            recieveSystemFields: this.recieveSystemFields
        }

        var suborderid = subOrder.OrderID;

        return (
            < RechangeSubOrder
                key={suborderid}// 前三个用于html和react的 引用功能 
                id={suborderid}
                ref={suborderid}

                subOrderId={"suborder" + suborderid}
                subOrder={subOrder}
                functions={functions}
                isForCusClient={this.props.isForCusClient}
                />
        )
    }
})



/*
    Orders existed for the refs of the top-level component
*/

var Orders = React.createClass({
    render: function () {
        var unlimited = false;
        var buttonsDefault = {
            'copyToClipboard': false,
            'extraEdit': false,
            'personNumEdit': false,
            'clone': false,
            "check": false,
            "toEdit": false,
        };
        var buttons = this.props.buttons;
        for (var i in buttonsDefault) {
            if (i in buttons) {
                buttonsDefault[i] = buttons[i];
            }
        }
        if ((this.props.unlimited) !== undefined) {
            unlimited = this.props.unlimited;
        }

        function isArray(obj) {
            return Object.prototype.toString.call(obj) === '[object Array]';
        }

        if (this.props.UIType == 'edit') {
            var isForCusClient = true;
            var subOrderList = [];

            //判断是客户端填写还是浪花工作台的修改（后台给的数格式不一致）
            if (!(isArray(this.props.initdata))) {//浪花工作台
                subOrderList.push(this.props.initdata);
                isForCusClient = false;
            }
            else {//客户端填写
                subOrderList = this.props.initdata;
                isForCusClient = true;
            }
            return (
                <TBOrders
                    ref={function (form) {
                        reactx.setALL(form);
                        reactref = form;
                    } }
                    subOrderList={subOrderList}
                    isForCusClient={this.props.isForCusClient}
                    state={this.props.state}
                    limit={!unlimited}
                    buttons={buttonsDefault}
                    />
            )
        }
        else if (this.props.UIType == 'reChange') {
            return (
                <RechangeOrder
                    ref={function (reChange) {
                        reactref = reChange;
                    } }
                    oneSuborder={this.props.initdata}
                    isForCusClient={false}
                    limit={!unlimited}

                    />
            )
        }
        else if (this.props.UIType == 'view') {
            var subOrderList = [];

            //判断是客户端填写还是浪花工作台的修改（后台给的数格式不一致）
            if (!(isArray(this.props.initdata))) {//浪花工作台
                subOrderList.push(this.props.initdata);
            }
            else {//客户端填写
                subOrderList = this.props.initdata;
            }
            return (
                <TBOrders
                    subOrderList={subOrderList}
                    isForCusClient={this.props.isForCusClient}
                    state={this.props.state}
                    limit={!unlimited}
                    />
            )
        }

    }

})





















