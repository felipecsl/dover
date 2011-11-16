var Utils = {};

/*
* Return current time value in the HH:MM format
*/
Utils.getTime = function () {
	var d = new Date();
	d.setDate(new Date().getDate());
	return d.toTimeString().substr(0, d.toTimeString().lastIndexOf(" "));
	//var time = d.toTimeString().substr(0, d.toTimeString().lastIndexOf(" "));
	//return (time + d.toLocaleTimeString().substr(d.toLocaleTimeString().lastIndexOf(" ")));
};

Utils.getTimeWithSeconds = function () {
	var d = new Date();
	d.setDate(new Date().getDate());
	return d.toTimeString().substr(0, d.toTimeString().lastIndexOf(" "));
}

Utils.formatMessage = function (_sMessage) {
	var retStr = _sMessage;
	retStr = Utils.sanitizeMsg(retStr);
	retStr += "<br />";

	return retStr;
};

Utils.isValidEmail = function (strEmail) {
	var emailRegEx = /^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$/;
	return strEmail.match(emailRegEx);
}

/*
* Strip html chars from string for safe displaying
*/
Utils.sanitizeMsg = function (sMsg) {
	var trimChars = ["<", ">", "\n"];
	var safeChars = ["&lt;", "&gt;", "<br />"];
	var retString = sMsg;

	for (var i = 0; i < trimChars.length; i++) {
		while (retString.indexOf(trimChars[i]) != -1) {
			retString = retString.replace(trimChars[i], safeChars[i]);
		}
	}

	return retString;
};

/* 
* Extract numeric part from pixels unit string (eg.: "54px" => 54)
*/
Utils.pxToInt = function (sPixels) {
	return parseInt(sPixels.replace('px', ''));
};

/* 
* Scale an image mantaining its original aspect ratio 
*/
Utils.scale = function (obj, w, h) {
	var nw = obj.width(), nh = obj.height();
	if ((nw > w) && w > 0) {
		nw = w;
		nh = (w / obj.width()) * obj.height();
	}
	if ((nh > h) && h > 0) {
		nh = h;
		nw = (h / obj.height()) * obj.width();
	}
	xscale = obj.width() / nw;
	yscale = obj.height() / nh;
	obj.width(nw).height(nh);
};

/*
* Returns whether the provided file full path is an image file or not
*/
Utils.isImageFile = function (filePath) {
	var fileName = Utils.getFileName(filePath);
	var validExtensions = [".jpg", ".jpeg", ".png", ".gif"];
	var bIsImage = false;

	$.each(validExtensions, function (i, ext) {
		if (fileName.endsWith(ext)) {
			bIsImage = true;
			return;
		}
	});

	return bIsImage;
};

/*
* Returns the file name and extension of the specified path string. (Similar to .NET Path.GetFilename)
*/
Utils.getFileName = function (filePath) {
	return filePath.split('\\').pop().split('/').pop();
};

Utils.getFileNameWithoutExtension = function (filePath) {
	var fileName = Utils.getFileName(filePath);
	var extensionIndex = fileName.lastIndexOf('.');

	return fileName.substr(0, fileName.lastIndexOf('.'));
};

String.prototype.endsWith = function (str) {
	var lastIndex = this.lastIndexOf(str);
	return (lastIndex != -1) && (lastIndex + str.length == this.length);
};

var Random = {};

Random.getNumber = function (range) {
	return Math.floor(Math.random() * range);
};

Random.getChar = function () {
	var chars = "0123456789abcdefghijklmnopqurstuvwxyzABCDEFGHIJKLMNOPQURSTUVWXYZ";
	return chars.substr(Random.getNumber(62), 1);
};

Random.getId = function (size) {
	var str = "";
	for (var i = 0; i < size; i++) {
		str += Random.getChar();
	}
	return str;
};

Utils.random = Random;

String.empty = "";

String.prototype.bool = function () {
	return (/^true$/i).test(this);
};