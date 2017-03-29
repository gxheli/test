import React from 'react';
import { render } from 'react-dom';
import axios from 'axios';
import classnames from 'classnames';
import { modal } from './../lib/common';

import './../sass/login.scss';

class Login extends React.Component {
	constructor( props ) {
		super( props );
		this.state = {
			loginErrorTime : 0
		}
	}

	componentDidMount(){
		setTimeout(function(){
			document.getElementsByClassName('logo-form')[0].className += ' load';
		},0);
	}
	checkCode( e ){
		const vercode = e.target.value,
			iconCheck = document.getElementsByClassName('icon-check')[0];

		if( vercode.length === 4 ){

			axios.post('/Users/CheckValidateCode',{
				str: vercode,
			}).then(function (response) {

				const data = response.data;
				if( data.ErrorCode === 200 ){
					iconCheck.className += ' show';
				}
			});
		}else{
			iconCheck.classList.remove('show');
		}

	}
	changeCode( e ){
		e.target.src = '/Users/GetValidateCode?t=?'+ +new Date();
	}
	login(){

		let username = document.getElementById('username').value,
			password = document.getElementById('password').value,
			code = document.getElementById('vercode').value,
			that = this;
			
		axios.post('/Users/Login',{
			Code:code,
			UserName: username,
    		PassWord: password,
		}).then(function (response) {
			//debugger;
			const data = response.data;
			if( data.ErrorCode === 200 ){
				window.location.hash = "#/order";
			}else{
				modal.alert( data.ErrorMessage );

				let loginErrorTime = that.state.loginErrorTime;
				loginErrorTime++;

				that.setState({
					loginErrorTime,
				});

			}
		}).catch(function (error) {
			//modal.alert();
		})
	}
	render() {

		const vercodeClassName = classnames('from-item', { vercode : true } );

		 return(
		 	<div className="md-login">
		 		<div className="logo"><img src="./images/dodotour_logo.png"/></div>
		 		<div className="logo-form">
		 			<div className="from-item">
		 				<span className="username">
		 					<i className="fa fa-user"></i>
		 					<input type="text" placeholder="请输入用户名" id="username"/>
		 				</span>
		 			</div>
		 			<div className="from-item">
		 				<span className="password">
		 					<i className="fa fa-lock"></i>
		 					<input type="password" placeholder="请输入密码" id="password"/>
		 				</span>
		 			</div>
		 			<div className={vercodeClassName} style={{display: this.state.loginErrorTime > 2 ? 'inline-block' : 'none' }}>
		 				<input type="text" placeholder="请输入验证码" id="vercode" onChange={this.checkCode.bind(this)}/>
		 				<i className="icon-check fa fa-check-circle"></i>
		 				<img src="/Users/GetValidateCode" onClick={this.changeCode.bind(this)}/>
		 			</div>

		 			<div className="login-btn">
		 				<button onClick={this.login.bind(this)}>登录</button>
		 			</div>
		 		</div>
		 		<div className="copyright">©DoDo Tour International Travel Co., Ltd. All Rights Reserved.</div>
		 	</div>
		 )
	}
}

export default Login;