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
            forceUpdateState: 0
        };
    },
    //自动更新状态
    componentDidMount: function () {

        this.upadteToForm(this.getdUpdateData());

    },
    componentDidUpdate: function () {
        // this.upadteToForm(this.getdUpdateData());
    },

    getDefaultValue: function () {
        if (this.props.UIType == "reChange") {
            return (this.props.beforeRechangeValue);
        }
        return this.props.initdata;
    },
    updateCopyObj: function (obj) {
        if (!(this.props.copyToClipboard)) {
            return;
        }
        var copyObj = {};
        copyObj[this.props.componentid] = obj;
        this.props.updateCopyObjx(copyObj);
    },
    exend: function () { //use like jq.extend
        // copy reference to target object
        var target = arguments[0] || {},
            i = 1,
            length = arguments.length,
            deep = false,
            options;
        // Handle a deep copy situation
        if (typeof target === "boolean") {
            deep = target;
            target = arguments[1] || {};
            // skip the boolean and the target
            i = 2;
        }
        // Handle case when target is a string or something (possible in deep copy)
        if (typeof target !== "object" && !jQuery.isFunction(target)) target = {};
        // extend jQuery itself if only one argument is passed
        if (length == i) {
            target = this;
            --i;
        }
        for (; i < length; i++)
            // Only deal with non-null/undefined values
            if ((options = arguments[i]) != null)
                // Extend the base object
                for (var name in options) {
                    var src = target[name],
                        copy = options[name];
                    // Prevent never-ending loop
                    if (target === copy) continue;
                    // Recurse if we're merging object values
                    if (deep && copy && typeof copy === "object" && !copy.nodeType) target[name] = this.extend(deep,
                        // Never move original objects, clone them
                        src || (copy.length != null ? [] : {}), copy);
                    // Don't bring in undefined values
                    else if (copy !== undefined) target[name] = copy;
                }
        // Return the modified object
        return target;
    }

};
