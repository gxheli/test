import React from 'react';
import { render } from 'react-dom';
import { Link } from 'react-router';

import './../sass/topNav.scss';

class TopNav extends React.Component {
	constructor( props ) {
		super( props );
	}
	goBack(){
		window.history.go(-1);
	}
	render(){

		return(
			<div className="md-topNav">
				<span className="goback" onClick={this.goBack.bind(this)}><i className="fa fa-angle-left"></i>返回</span>
				<span className="title">{this.props.title}</span>
				<span className="more"><i className="fa fa-ellipsis-h"></i></span>
			</div>
		 )
	}
}

export default TopNav;