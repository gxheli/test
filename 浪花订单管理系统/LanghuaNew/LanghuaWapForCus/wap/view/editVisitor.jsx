import React from 'react';
import { render } from 'react-dom';
import { Link } from 'react-router';
import { getUrlParam,convertDate,modal,setTitle } from './../lib/common';
import DatePicker from 'react-mobile-datepicker';
//import TopNav from './topNav';
import BtmNav from './btmNav'
import axios from 'axios';

import './../sass/editVisitor.scss';


class EditVisitor extends React.Component {
	constructor( props ) {
		super( props );
		this.state = {
			TravellerID:0,
			Birthday:'',
			TravellerEnname:'',
			TravellerName:'',
			PassportNo:'',
			TravellerSex:'',
			TravellerDetail:{
				Height:'',
				ClothesSize:'',
				GlassesNum:'',
				ShoesSize:'',
				Weight:'',
			},
			time : new Date(),
			isOpen: false,
		}
	}
	componentWillMount(){
		setTitle('修改常用游客');
	}
	componentDidMount(){

		setTimeout(function(){
			document.getElementsByClassName('md-nav')[0].getElementsByTagName('a')[1].className += ' selected';
		},0);

		const that = this,
			travellerId = getUrlParam( 'TravellerID' );

		axios.post('/Users/GetUserInfo',{
		}).then(function (response) {

			let data = response.data,
				travellers = null;
			
			if( data.ErrorCode === 200 ){

				for(let i = 0;i<data.UserInfo.Travellers.length;i++ ){
					if( travellerId == data.UserInfo.Travellers[i].TravellerID ){
						travellers = data.UserInfo.Travellers[i];
						break;
					}
				}

				that.setState({
					TravellerID:travellers.TravellerID,
					Birthday:travellers.Birthday,
					TravellerEnname:travellers.TravellerEnname,
					TravellerName:travellers.TravellerName,
					PassportNo:travellers.PassportNo,
					TravellerSex:travellers.TravellerSex ? '女' : '男',
					TravellerDetail:travellers.TravellerDetail,
				})

				if( travellers.TravellerDetail.Height ){
					document.getElementById('elseCheckBox').click();
				}
			}
		})

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
	showElse( e ){
		//debugger;
		const elseBox = document.getElementById('elseBox'),
			target = e.target;

		if( target.tagName === 'INPUT' ){

			setTimeout(function(){
				if( target.checked ){
					elseBox.className = 'ei-box show';
				}else{
					elseBox.className = 'ei-box';
				}
			},10);

		}else{
			let checkbox = target.getElementsByTagName('input')[0];
			if( checkbox.checked ){
				elseBox.className = 'ei-box';
				checkbox.checked = false;
			}else{
				elseBox.className = 'ei-box show';
				checkbox.checked = true;
			}
		}
		

	}
	changeSex( e ){
		if( e.target.value === '男' ){
			this.setState({ TravellerSex:'女' });
		}else{
			this.setState({ TravellerSex:'男' });
		}
	}
	changeInput(){

		this.setState({
			TravellerEnname:document.getElementById('en_username').value,
			TravellerName:document.getElementById('username').value,
			PassportNo:document.getElementById('passport').value,
			TravellerDetail:{
				Height:document.getElementById('height').value,
				ClothesSize:document.getElementById('clothesSize').value,
				GlassesNum:document.getElementById('glassesNum').value,
				ShoesSize:document.getElementById('shoesSize').value,
				Weight:document.getElementById('weight').value,
			},
		});

	}
	save(){

		let that = this;

		axios.post('/Users/EditTraveller',{
			TravellerID: that.state.TravellerID,
			TravellerName:that.state.TravellerName,
			TravellerEnname:that.state.TravellerEnname,
			PassportNo:that.state.PassportNo,
			Birthday:convertDate(that.state.time,'YYYY-MM-DD'),
			TravellerSex: that.state.TravellerSex === '男' ? 0 : 1
		}).then(function (response) {

			let data = response.data;
			if( data.ErrorCode === 200 ){
				

				if( document.getElementById('elseCheckBox').checked ){ 

					const date = +new Date();

					axios.post('/Users/EditTravellerDetail?t='+date,{
						TravellerID: that.state.TravellerID,
						TravellerDetail:{
							Height:document.getElementById('height').value,
							ClothesSize:document.getElementById('clothesSize').value,
							GlassesNum:document.getElementById('glassesNum').value,
							ShoesSize:document.getElementById('shoesSize').value,
							Weight:document.getElementById('weight').value,
						}
					}).then(function (response) {

						modal.success('修改成功');
						setTimeout(function(){
							window.location.hash = "#/visitor";
						},1000);
					})
				}else{

					modal.success('修改成功');
					setTimeout(function(){
						window.location.hash = "#/visitor";
					},1000);

				}
			}else{
				modal.error('修改失败');
			}
		})

		

	}
	render(){

		return(
			 <div className="md-editVisitor">
			 	{/*<TopNav title='修改常用游客'/>*/}
			 	<div className="user-form">
			 		<div className="base-info">
			 			<h3>基本资料<span style={{display:'none'}}><input type="checkbox" />设为本人</span></h3>
			 			<div className="form-item">
			 				<label><i className="mandatory-fields">*</i>中文姓名:</label>
			 				<input type="text" id="username" value={this.state.TravellerName} onChange={this.changeInput.bind(this)}/>
			 			</div>
			 			<div className="form-item">
			 				<label><i className="mandatory-fields">*</i>姓名拼音:</label>
			 				<input type="text" id="en_username" value={this.state.TravellerEnname} onChange={this.changeInput.bind(this)}/>
			 			</div>
			 			<div className="form-item">
			 				<label><i className="mandatory-fields">*</i>护照号码:</label>
			 				<input type="text" id="passport" value={this.state.PassportNo} onChange={this.changeInput.bind(this)}/>
			 			</div>
			 			<div className="form-item">
			 				<label><i className="mandatory-fields">*</i>生日</label>
			 				<span className="calendar-box">
			 					<input type="text" readOnly onClick={this.handleClick.bind(this)} value={convertDate(this.state.time,'YYYY-MM-DD')} onChange={this.change.bind(this)} />
			 					<i className="icon-calendar fa fa-calendar"></i>
			 				</span>
			 				<DatePicker  theme="ios" dateFormat={this.state.dateFormat} value={this.state.time} isOpen={this.state.isOpen} 
			 					onSelect={this.handleSelect.bind(this)} onCancel={this.handleCancel.bind(this)} />
			 			</div>
			 			<div className="form-item">
			 				<label>性别</label>
			 				<input type="text" value={this.state.TravellerSex} readOnly onClick={this.changeSex.bind(this)}/>
			 			</div>
			 		</div>
			 		<div className="else-info">
			 			<h3 onClick={this.showElse.bind(this)} ><input type="checkbox" id="elseCheckBox"/>附加资料</h3>
			 			<div className="ei-box" id="elseBox">
			 				<div className="form-item">
				 				<label><i className="mandatory-fields">*</i>身高:(CM)</label>
				 				<input type="text" id="height" value={this.state.TravellerDetail.Height} onChange={this.changeInput.bind(this)}/>
				 			</div>
				 			<div className="form-item">
				 				<label><i className="mandatory-fields">*</i>体重:(KG)</label>
				 				<input type="text" id="weight" value={this.state.TravellerDetail.Weight} onChange={this.changeInput.bind(this)}/>
				 			</div>
				 			<div className="form-item">
				 				<label><i className="mandatory-fields">*</i>鞋子码数:</label>
				 				<input type="text" id="shoesSize" value={this.state.TravellerDetail.ShoesSize} onChange={this.changeInput.bind(this)}/>
				 			</div>
				 			<div className="form-item">
				 				<label>衣服码数:</label>
				 				<input type="text" id="clothesSize" value={this.state.TravellerDetail.ClothesSize} onChange={this.changeInput.bind(this)}/>
				 			</div>
				 			<div className="form-item">
				 				<label>近视度数:</label>
				 				<input type="text" id="glassesNum" value={this.state.TravellerDetail.GlassesNum} onChange={this.changeInput.bind(this)}/>
				 			</div>
			 			</div>
			 		</div>

			 		<div className="btm-btn">
			 			<button onClick={this.save.bind(this)}>保存</button>
			 		</div>
			 	</div>
			 	<BtmNav/>
			 </div>
		 )
	}
}

export default EditVisitor;