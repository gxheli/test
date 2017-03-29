export function getUrlParam( param ){
	const reg = new RegExp(`${param}=([^&]*)(&?)`, 'i'),
		paramValue = location.hash.match(reg);
	return paramValue ? paramValue[1] : '';
}

export function letDivCenter( target ){

    const divHeight = target.offsetHeight,
    	divWidth = target.offsetWidth,
    	scrollTop = document.documentElement.scrollTop || document.body.scrollTop,
    	scrollLeft = document.documentElement.scrollLeft || document.body.scrollLeft,
    	top = ( document.documentElement.clientHeight - divHeight) / 2,
    	left = (document.documentElement.clientWidth - divWidth) / 2;

    target.style.position = 'absolute';
    target.style.top = (scrollTop + top) + 'px';
    target.style.left = (scrollLeft + left) + 'px';
}

export function sleep( time ){
	return new Promise((resolve) => setTimeout(resolve, time));
}

export function convertDate(date, format) {
    let str = format;
    const o = {
        'M+': date.getMonth() + 1,
        'D+': date.getDate(),
        'h+': date.getHours(),
        'm+': date.getMinutes(),
        's+': date.getSeconds(),
    };
    if (/(Y+)/.test(format)) {
        str = str.replace(RegExp.$1,
                (date.getFullYear().toString()).substr(4 - RegExp.$1.length));
    }

    for (const k in o) { 
        if (new RegExp(`(${k})`).test(format)) {
            str = str.replace(RegExp.$1,
                (RegExp.$1.length === 1) ?
                o[k] : (`00${o[k]}`.substr((o[k].toString()).length)));
        }
    }

    return str;
}

export function getCookieValue(name) {
	let cookieValue = null;
	if( document.cookie && document.cookie != '' ){
		const cookies = document.cookie.split(';');
		for(let i = 0; i < cookies.length; i++ ){

			let cookie = cookies[i].trim();
			if( cookie.substring(0, name.length + 1) == (name + '=')) {
				cookieValue = decodeURIComponent(cookie.substring(name.length + 1));
				break;
			}
		}
	}
	return cookieValue;
}


export function delCookie(name) {
	var exp = new Date();
	exp.setTime( exp.getTime() - 60 * 1000 );
	var cval = getCookieValue(name);

	document.cookie = name + "=" + cval + ";path=/;expires=" + exp.toUTCString();
}

//动态修改网页标题，hack微信浏览器
export function setTitle( title ){
	document.title = title;
	let iframe = document.createElement('iframe'); 
	iframe.src="/favicon.ico";
	iframe.style.display = 'none';
	iframe.onload = function(){
		setTimeout(function() {
	        iframe.remove();
	    }, 0)
	}
	document.body.appendChild(iframe);
}

export const loading = {
	show(){
		const loading = document.getElementById('loading');
		if( loading ){
			loading.style.display = 'block';
			return;
		}
		const node = document.createElement('div'),
			html = '<img src="./images/loading.gif"/>';
		node.className = 'md-loading';
		node.id = 'loading';
		node.innerHTML = html;
		document.body.appendChild(node);
	},
	hide(){
		document.getElementById('loading').style.display = 'none';
	}
};

export const modal = {

	success( message ){

		const sucessModal = document.getElementById('md-modal-success');

		if( sucessModal ){
			sucessModal.getElementsByClassName('modal-msg')[0].innerText = message;
			sucessModal.className += ' show';

			setTimeout(function(){
				sucessModal.className = 'modal-box';
			},900);

			return;	
		}

		const node = document.createElement('div'),
			html = `<div class="modal-icon"><i class="fa fa-check-circle"></i></div>
			 		<div class="modal-msg">${message}</div>
			 		<div class="modal-btn" id="success_ok_btn">确定</div>`;

		node.className = 'modal-box';
		node.id = 'md-modal-success';
		node.innerHTML  = html ;
		document.body.appendChild(node);

		const am = document.getElementById('md-modal-success');
		am.className += ' show';


		document.getElementById('success_ok_btn').onclick = function(){
			am.className = 'modal-box';
		}

		setTimeout(function(){
			am.className = 'modal-box';
		},900);
	},
	error( message ){

		const errorModal = document.getElementById('md-modal-error');

		if( errorModal ){
			errorModal.getElementsByClassName('modal-msg')[0].innerText = message;
			errorModal.className += ' show';
			return;	
		}

		const node = document.createElement('div'),
			html = `<div class="modal-icon"><i class="fa fa-times-circle"></i></div>
			 		<div class="modal-msg">${message}</div>
			 		<div class="modal-btn" id="error_ok_btn">确定</div>`;

		node.className = 'modal-box';
		node.id = 'md-modal-error';
		node.innerHTML  = html ;
		document.body.appendChild(node);

		const am = document.getElementById('md-modal-error');
		am.className += ' show';


		document.getElementById('error_ok_btn').onclick = function(){
			am.className = 'modal-box';
		}
		
	},
	alert( message ){

		const alertModal = document.getElementById('md-modal-alert');

		if( alertModal ){
			alertModal.getElementsByClassName('modal-msg')[0].innerText = message;
			alertModal.className += ' show';
			return;	
		}

		const node = document.createElement('div'),
			html = `<div class="modal-icon"><i class="fa fa-exclamation-circle"></i></div>
			 		<div class="modal-msg">${message}</div>
			 		<div class="modal-btn" id="alert_ok_btn">确定</div>`;

		node.className = 'modal-box';
		node.id = 'md-modal-alert';
		node.innerHTML  = html ;
		document.body.appendChild(node);

		const am = document.getElementById('md-modal-alert');
		am.className += ' show';


		document.getElementById('alert_ok_btn').onclick = function(){
			am.className = 'modal-box';
		}

	},
	confirm( message, callback ){
		const confirmModal = document.getElementById('md-modal-confirm');

		if( confirmModal ){
			confirmModal.getElementsByClassName('modal-msg')[0].innerText = message;
			confirmModal.className += ' show';
			return;	
		}

		const node = document.createElement('div'),
			html = `<div class="modal-icon"><i class="fa fa-exclamation-circle"></i></div>
			 		<div class="modal-msg">${message}</div>
			 		<div class="modal-btn-wrap">
			 			<button class="ok" id="confirm_ok_btn">确定</button>
						<button class="cancel" id="confirm_cancel_btn">取消</button>
			 		</div>`;

		node.className = 'modal-box';
		node.id = 'md-modal-confirm';
		node.innerHTML  = html ;
		document.body.appendChild(node);

		const am = document.getElementById('md-modal-confirm');
		am.className += ' show';

		document.getElementById('confirm_ok_btn').onclick = function(){

			if( typeof callback === 'function' ){
				callback();
				am.className = 'modal-box';
				am.remove();
			}else{
				console.error('callback is not a function....');
			}
			
		}
		document.getElementById('confirm_cancel_btn').onclick = function(){
			am.className = 'modal-box';
			am.remove();
		}
	}
}

