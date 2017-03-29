 // 参数格式{ismust:bool,title:string}
 var Lable = React.createClass({
     isMust : function (ismust) {
         if(ismust){
             return(
                 <span className="redspark">*</span>
             )
        }
     },
     getName : function (title) {
         if(!title){
             title = 'undefined';
         }
         return title+'：';
     },
     renderSelectedPeple:function(){
        if(this.props.params.reactType=='personpicker'){
            return(
                <div className="selectnumtips">已选：{this.props.params.selectednum}人</div>
            )
        }
     },
     componentDidMount:function(){
     },
     render : function () {
         return (
              <label className="col-md-2 control-label">
                {this.isMust(this.props.params.isMust)}{this.getName(this.props.params.title)}
                {this.renderSelectedPeple()}
              </label>
         )
    }
 });