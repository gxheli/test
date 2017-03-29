import React from 'react';
import { render } from 'react-dom';
import { Link } from 'react-router';

import './../sass/btmNav.scss';

class BottomNav extends React.Component {
	constructor( props ) {
		super( props );
	}
	componentDidMount(){
		
		if( document.cookie.indexOf('.ASPXAUTH') === -1 ){
		    window.location.hash = "#/login";
		}
	}

	render(){

		return(
			<div className="md-nav">
				<Link to="/order" activeClassName="selected">
					<div className="nav-item item-trip">
						<i className="fa fa-paper-plane"></i>
						<span>行程</span>
					</div>
				</Link>
				<Link to="/visitor" activeClassName="selected" >
					<div className="nav-item item-visitors">
						<i className="fa fa-users"></i>
						<span>常用游客</span>
					</div>
				</Link>
				<Link to="/user" activeClassName="selected" >
					<div className="nav-item item-user">
						<i className="fa fa-user"></i>
						<span>个人中心</span>
					</div>
				</Link>
			</div>
		 )
	}
}

export default BottomNav;