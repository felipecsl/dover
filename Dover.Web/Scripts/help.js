$(function () {
	$("#guidesMenu").toggle(function () {
		$("#guides").show();
		return false;
	}, function () {
		$("#guides").hide();
		return false;
	});
});
