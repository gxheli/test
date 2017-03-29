 // 参数格式{ismust:bool,title:string}
 var LableRechange= React.createClass({
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
         return title;
     },
     renderSelectedPeple:function(){
        if(this.props.params.reactType=='personpicker'){
            return(
                <div className="selectnumtips">已选：{this.props.params.selectednum}人</div>
            )
        }
     },
     render : function () {
         return (
              <span>
                {this.isMust(this.props.params.isMust)}{this.getName(this.props.params.title)}
              </span>
         )
    },
 });