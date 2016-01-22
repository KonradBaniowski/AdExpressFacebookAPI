function GetCookie(name) {
	if (document.cookie){
		var startIndex = document.cookie.indexOf(name);
		if (startIndex != -1) {
			var endIndex = document.cookie.indexOf(';', startIndex);
			if (endIndex == -1) endIndex = document.cookie.length;
			return unescape(document.cookie.substring(startIndex+name.length+1, endIndex));
		}
	}
	return null;
}

function setPermanentCookie(name,value) {
	var expire = new Date ();
	expire.setTime (expire.getTime() + 24 * 60 * 60 * 1000 * 365);
	document.cookie = name + '=' + escape(value) + '; expires=' + expire.toGMTString();
}

function setCookie(name,value,days) {
	var expire = new Date ();
	expire.setTime (expire.getTime() + (24 * 60 * 60 * 1000) * days);
	document.cookie = name + '=' + escape(value) + '; expires=' + expire.toGMTString();
}

