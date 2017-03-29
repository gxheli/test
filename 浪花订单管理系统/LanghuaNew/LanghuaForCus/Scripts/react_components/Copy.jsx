var Copy = React.createClass({
    getInitialState: function () {
        return {
            copyStr: "测试",
            fromWhich: ""
        }
    },
    componentDidMount: function (e) {
        var _this = this;
        $(_this.refs.globalCopyCatcher).bind("click", function (e, copyStr, fromWhich) {
            console.log('click');
        })
        $(_this.refs.globalCopyCatcher).bind("updateCopyboard", { "p": _this }, function (e, copyStr, fromWhich) {
            e.data.p.setState({
                'copyStr': copyStr,
                'fromWhich': fromWhich
            }, function () {
                $(_this.refs.globalCopyCatcher).click();
            })
        })
    },
    onCopy: function (e) {
        console.log(e)
        var which = this.state.fromWhich;
        if (which != "") {
            which.success("复制成功！");
        }
    },
    render: function (e) {
        return (
            <CopyToClipboard ref="CopyToClipboardWrapper" key="CopyToClipboardWrapper"  text={this.state.copyStr} onCopy={this.onCopy}>
                <button
                    ref="globalCopyCatcher"
                    key="globalCopyCatcher"
                    id="globalCopyCatcher"
                    className="hidden global btn button-default"
                    role="button"
                    >
                    对单利器
                </button>
            </CopyToClipboard>
        )
    }
})
