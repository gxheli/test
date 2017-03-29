import React from 'react';
import ReactDom from 'react-dom';

import {Router, Route, hashHistory } from 'react-router';

import Login from './../view/login';
import Index from './../view/index';
import User from './../view/user';
import Order from './../view/order';
import OrderDetail from './../view/orderDetail';
import Visitor from './../view/visitor';
import EditVisitor from './../view/editVisitor';
import AddVisitor from './../view/addVisitor';
import ModifyUser from './../view/modifyUser';
import ModifyPwd from './../view/modifyPwd';

import './../css/style.css';
import './../css/font-awesome.css';


(function (doc, win) {
    var docEl = doc.documentElement,
        resizeEvt = 'orientationchange' in window ? 'orientationchange' : 'resize',
        recalc = function () {
            var clientWidth = docEl.clientWidth;
            if (!clientWidth) return;
            if(clientWidth>=640){
                docEl.style.fontSize = '100px';
            }else{
                docEl.style.fontSize = 100 * (clientWidth / 360) + 'px';
            }
        };

    if (!doc.addEventListener) return;
    win.addEventListener(resizeEvt, recalc, false);
    doc.addEventListener('DOMContentLoaded', recalc, false);
})(document, window);

if( document.cookie.indexOf('.ASPXAUTH') === -1 ){
    window.location.hash = "#/login";
}

document.body.addEventListener("click", function(e){
    var rqBox = document.getElementById('rqBox');
    if( rqBox && rqBox.style.display==='block'){
        rqBox.style.display = 'none';
    }

}, false);

const router = (
	<Router history={hashHistory }>
		<Route path="/" component={Order}/>
		<Route path="login" component={Login}/>
		<Route path="user" component={User}/>
        <Route path="order" component={Order}/>
        <Route path="orderDetail" component={OrderDetail}/>
        <Route path="visitor" component={Visitor}/>
        <Route path="editVisitor" component={EditVisitor}/>
        <Route path="addVisitor" component={AddVisitor}/>
        <Route path="modifyUser" component={ModifyUser}/>
        <Route path="modifyPwd" component={ModifyPwd}/>
	</Router>
	);


ReactDom.render(router,document.getElementById('app'));