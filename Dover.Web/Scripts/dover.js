var flotObj = null;

$(function () {
	$("table.sortable").tablesorter({
		highlightClass: 'highlight',
		dateFormat: 'uk'
	});

	// menu setup
	$(".menuitem > span.withsubmenu").click(function () {
		$(this).next().slideToggle("fast");
	});

	$(".menuitem > span.withsubmenu").toggle(
		function () {
			if ($(this).next().length > 0) {
				$(this).css("border-bottom", "1px solid #E3E3E3");
			}
		},
		function () {
			if ($(this).next().length > 0) {
				var that = $(this);
				setTimeout(function () { that.css("border-bottom", "none"); }, 300);
			}
		}
	);

	$(".confirmable").click(function () {
		if (!confirm($(this).attr("confirmationmsg"))) {
			return false;
		}
	});

	// List template: Collapse/Expand button behavior
	$("#btnCollapse").click(function () {
		if ($(this).text() == "Recolher campos") {
			$("div.panel-body").hide();
			$(".panel-arrow").addClass("panel-arrow-collapsed");
			$(this).text("Expandir campos");
		}
		else {
			$(this).text("Recolher campos");
			$(".panel-arrow").removeClass("panel-arrow-collapsed");
			$("div.panel-body").show();
		}
	});

	// List template: Drag & drop
	$("table.draggable").tableDnD({
		onDragClass: "grid-row-dragging",
		onDrop: function (table, row) {
			sendSortOrder();
		}
	});

	$(".grid-header-cell").click(function () {
		setTimeout(sendSortOrder, 500);
	});

	// Login page: OpenId fade-in/fade-out
	$("#useopenid").click(function () {
		$("#login-account").fadeOut();

		setTimeout(function () {
			$("#rpxframe").fadeIn();
		}, 1000);
	});

	$("#useregularaccount").click(function () {
		$("#rpxframe").fadeOut();

		setTimeout(function () {
			$("#login-account").fadeIn();
		}, 1000);
	});

	function setWidth() {
		$("div.pagecontent").width($(window).width() - $("#menu-wrapper").width() - 80);
	}

	$(window).resize(function () {
		setWidth();
	});

	setWidth();
});

function displayFlash(_msg) {
	$("#msg-body").text(_msg);
	var wrapper = $("#message-wrapper");
	wrapper.attr("style", "margin-left: " + -(wrapper.width() / 2) + "px;").fadeIn();
	setTimeout(function () { $("#message-wrapper").fadeOut(); }, 5000);
	// scroll to the top of the page so that the user can read the message
	$("html").animate({ scrollTop: 0 });
};

// Analytics ajax response
function onAnalyticsDataReceived(data) {
	flotObj = $.plot($("#analytics-placeholder"), data.plotArray, {
		lines: { show: true },
		points: { show: true },
		xaxis: {
			mode: "time",
			timeformat: "%d/%m/%y",
			minTickSize: [1, "day"]
		}
	});

	$("#analytics-period").text(data.period);
	$("#total-visits").text(data.totalVisits);
};