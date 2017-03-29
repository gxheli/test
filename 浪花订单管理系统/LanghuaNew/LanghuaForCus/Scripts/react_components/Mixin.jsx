    //共用部分
var MXINS_UPDATE_DATA = {
    getdUpdateData: function () {
        var dataUpdate = new Object();
        dataUpdate[this.props.suborderid] = new Object();
        dataUpdate[this.props.suborderid][this.props.componentid] = this.prehandleupdatedata(this.state.postValue);
        return dataUpdate;
    },
    upadteToForm: function (data) {
        this.props.UpdateDataToForm(data);
    },
    //对应默认值
    getInitialState: function () {
        var thisinitdata = this.props.initdata;
        thisinitdata = this.prehandledata(thisinitdata);
        return {
            postValue: thisinitdata,
            forceUpdateMax: 2,
            forceUpdateState:0
        }
    },


    //自动更新状态
    componentDidMount: function () {

        this.upadteToForm(this.getdUpdateData());
        
    },
    componentDidUpdate: function () {
        // this.upadteToForm(this.getdUpdateData());
    },
    
    getDefaultValue:function(){
        if(this.props.UIType=="reChange"){
            return (this.props.beforeRechangeValue);
        }
        return  this.props.initdata;
    },
    updateCopyObj:function(obj){
        if(!(this.props.copyToClipboard)){
            return;
        }
        var copyObj ={};
        copyObj[this.props.componentid]  = obj;
        this.props.updateCopyObjx(copyObj);
    }

}
