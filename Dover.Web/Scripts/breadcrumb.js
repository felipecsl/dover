function drawBreadCrumb(_arrItems) {
	var breadcrumb = $("#breadcrumb");

	try {
		$.each(_arrItems, function (i, val) {
			var canvas = $("<canvas class='bcitem' width='100' height='26'>" + val + "</canvas>").appendTo(breadcrumb);

			if (i < _arrItems.length - 1) {
				drawBreadCrumbItem(canvas[0]);
			}
			else {
				drawBreadCrumbItem(canvas[0], "#f5921f", "#ffa740", "#fff");
			}
		});
	}
	catch (err) {
		// Probably canvas not supported
	}
}

function drawBreadCrumbItem(_elem, _bgColor1, _bgColor2, _txtColor) {
	var txt = $(_elem).text();
	var context = _elem.getContext("2d");
	var angle = 10;
	var bgColor1 = _bgColor1 || "#ccc";
	var bgColor2 = _bgColor2 || "#eee";
	var textColor = _txtColor || "#000";

	context.font = "bold 12px Tahoma";

	var textWidth = context.measureText(txt).width + 30;

	_elem.width = textWidth;

	context.strokeStyle = "#bbb";
	context.lineCap = "round";
	context.lineJoin = "round";
	context.textBaseline = "top";
	context.font = "bold 12px Tahoma";

	context.moveTo(0.5, 0.5);
	context.lineTo(_elem.width - angle, 0.5);
	context.lineTo(_elem.width, _elem.height / 2);
	context.lineTo(_elem.width - angle, _elem.height - 0.5);
	context.lineTo(0.5, _elem.height - 0.5);
	context.lineTo(0.5, 0.5);
	context.stroke();

	var my_gradient = context.createLinearGradient(0, _elem.height, 0, 0);
	my_gradient.addColorStop(0, bgColor1);
	my_gradient.addColorStop(1, bgColor2);
	context.fillStyle = my_gradient;
	context.fill();

	context.fillStyle = textColor;
	context.fillText(txt, 10, 7);
}