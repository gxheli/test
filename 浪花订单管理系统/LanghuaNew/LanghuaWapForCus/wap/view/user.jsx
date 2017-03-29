import React from 'react';
import { render } from 'react-dom';
import { Link } from 'react-router';
import axios from 'axios';
import { delCookie,setTitle } from './../lib/common';

//import TopNav from './topNav';
import BtmNav from './btmNav'

import './../sass/user.scss';


class User extends React.Component {

	constructor( props ) {
		super( props );
		this.state = {
			username : '',
			IsWechatBind:false,
			RQUrl:'',
			ImageUrl:'#',
		}
	}
	componentWillMount(){
		setTitle( '个人中心');
	}
	componentDidMount(){

		var that = this;

		axios.post('/Users/GetUserInfo',{
		}).then(function (response) {
			const data = response.data;
			
			if( data.ErrorCode === 200 ){

				that.setState({
					username:data.UserInfo.CustomerTBCode,
					IsWechatBind:data.UserInfo.IsWechatBind == "true" ? true : false,
					RQUrl:data.UserInfo.RQUrl,
					ImageUrl:data.UserInfo.ImageUrl,
				});	

				sessionStorage.setItem( 'CustomerID', data.UserInfo.CustomerID );
			}
		}).catch(function (error) {
			
		})
	}
	signOut(){
		delCookie( '.ASPXAUTH' );
		window.location.hash = "#/login";
	}
	showRQ(show){
		let rqBox = document.getElementById('rqBox');
		if( show ){
			rqBox.style.display = 'block';
		}else{
			rqBox.style.display = 'none';
		}
	}
	render(){
		return(
			 <div className="md-user">
			 	{/*<TopNav title='个人中心'/>*/}
			 	<Link to="modifyUser">
				 	<div className="user-top">
				 		<img src={this.state.ImageUrl}/>
				 		<span className="username">{this.state.username}</span>
				 	</div>
				</Link>
			 	<div className="user-center">
			 		<div className="list-item" onClick={this.showRQ.bind(this,true)}><i className="left-icon fa fa-wechat"></i><span>微信绑定</span><i className="right-icon fa fa-angle-right"></i><em>{this.state.IsWechatBind ? '已绑定' : '未绑定' }</em></div>
			 		<Link to="modifyPwd"><div className="list-item"><i className="left-icon fa fa-lock"></i><span>修改密码</span><i className="right-icon fa fa-angle-right"></i></div></Link>
			 		<div className="list-item"><i className="left-icon fa fa-question-circle"></i><span>帮助</span><i className="right-icon fa fa-angle-right"></i></div>
			 		<div className="list-item"><i className="left-icon fa fa-phone"></i><span>联系客服</span><i className="right-icon phone"><a href="tel:+075583973088">0755-83973088</a></i></div>
			 	</div>

			 	<div className="user-signOut">
			 		<button onClick={this.signOut.bind(this)}>退出</button>
			 	</div>
			 	<div className="rq-box" id="rqBox" onClick={this.showRQ.bind(this,false)} ><img src={this.state.RQUrl}/></div>
			 	<BtmNav/>
			 </div>
		 )
	}
}

export default User;




