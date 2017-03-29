import React from 'react';
import { render } from 'react-dom';
import { Link } from 'react-router';
import DatePicker from 'react-mobile-datepicker';
import { convertDate,modal,setTitle } from './../lib/common';
import axios from 'axios';
//import TopNav from './topNav';
import BtmNav from './btmNav'

import './../sass/addVisitor.scss';

class AddVisitor extends React.Component {
	constructor( props ) {
		super( props );
		this.state = {
			dateFormat: ['YYYY年', 'MM月', 'DD日'],
			time : new Date(),
			isOpen: false,
		}
	}
	componentWillMount(){
		setTitle('新增常用游客');
	}
	componentDidMount(){
		setTimeout(function(){
			document.getElementsByClassName('md-nav')[0].getElementsByTagName('a')[1].className += ' selected';
		},0);
	}
	handleClick(){
		this.setState({ isOpen: true });
	}
	handleCancel(){
		this.setState({ isOpen: false });
	}
	handleSelect( time ){
		this.setState({ time, isOpen: false });
	}
	change(e){
		this.setState({ time, isOpen: false });
	}
	changeSex( e ){

		const sex = document.getElementById('sex');
		if( e.target.value === '男' ){
			sex.value = '女';
		}else{
			sex.value = '男';
		}
	}
	save(){

		const that = this;

		axios.post('/Users/AddTraveller',{
			CustomerID: sessionStorage.getItem('CustomerID'),
			TravellerName: document.getElementById('username').value,
			TravellerEnname: document.getElementById('en_username').value,
			PassportNo:document.getElementById('passport').value,
			Birthday:convertDate(that.state.time,'YYYY-MM-DD'),
			TravellerSex: document.getElementById('sex').value === '男' ? 0 : 1
		}).then(function (response) {

			const data = response.data;
			if( data.ErrorCode === 200 ){
				modal.success('新增成功');
				setTimeout(function(){
					document.getElementById('md-modal-success').classList.remove('show');
					window.location.hash = "#/visitor";
				},1000);
			}else{
				modal.error( data.ErrorMessage );
			}
		})
	}
	cancel(){
		window.location.hash = "#/visitor";
	}
	render(){

		return(
			 <div className="md-addVisitor">
			 	{/*<TopNav title='新增常用游客'/>*/}
			 		<div className="add-form">
			 			<div className="tips-msg">一次输入，永久保存，多次使用，更加方便</div>
			 			<div className="form-item">
			 				<label><i className="mandatory-fields">*</i>中文姓名:</label>
			 				<input type="text" id="username"/>
			 			</div>
			 			<div className="form-item">
			 				<label><i className="mandatory-fields">*</i>姓名拼音:</label>
			 				<input type="text" id="en_username"/>
			 			</div>
			 			<div className="form-item">
			 				<label><i className="mandatory-fields">*</i>护照号码:</label>
			 				<input type="text" id="passport"/>
			 			</div>
			 			<div className="form-item">
			 				<label><i className="mandatory-fields">*</i>生日:</label>
			 				<span className="calendar-box">
			 					<input type="text" readOnly onClick={this.handleClick.bind(this)} value={convertDate(this.state.time,'YYYY-MM-DD')} onChange={this.change.bind(this)} />
			 					<i className="icon-calendar fa fa-calendar"></i>
			 				</span>
			 				<DatePicker  theme="ios" dateFormat={this.state.dateFormat} value={this.state.time} isOpen={this.state.isOpen} 
			 					onSelect={this.handleSelect.bind(this)} onCancel={this.handleCancel.bind(this)} />
			 			</div>
			 			<div className="form-item">
			 				<label>性别:</label>
			 				<input type="text" id="sex" value='男' readOnly onClick={this.changeSex.bind(this)}/>
			 			</div>
			 		</div>
			 		<div className="btm-btn">
			 			<button onClick={this.save.bind(this)}>保存</button>
			 			<button className="cancel" onClick={this.cancel.bind(this)}>取消</button>
			 		</div>
			 	<BtmNav/>
			 </div>
		 )
	}
}

export default AddVisitor;