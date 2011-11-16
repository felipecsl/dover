//$(function() {
//	var sFileName = null,
//		jcropAPI = null,
//		bCropImgVisible = true,
//		sAspectRatio = null,
//		nCurrStep = 0;

//	$("_btnCancel").live('click', function () { $("_modal").dialog("close") });
//	$("_btnFinish").live('click', function () {
//		var imagePath = (nCurrStep > 1)
//			? $("_image-cropped").attr("src")
//			: $("_image-to-crop").attr("src");

//		options.callback(imagePath); 		// notify the listeners
//		overlayObj.dialog("close"); 		// close the overlay
//	});

//	var sMarkup = "<div class='modal' id='{idPrefix}_modal' title='{dialogTitle}'><div class='step-1' id='{idPrefix}_step1'>Escolha um arquivo:<br/><input type='file' name='{idPrefix}_fileInput' size='40' id='{idPrefix}_fileInput' /><br/><br/>ou digite uma Url:<br/><input type='text' value='' id='{idPrefix}_imgUrl' class='text-box single-line' style='width:280px;' /></div><div class='step-2' id='{idPrefix}_step2'><img id='{idPrefix}_image-to-crop' /><input type='hidden' id='{idPrefix}_Xaxis' /><input type='hidden' id='{idPrefix}_Yaxis' /><input type='hidden' id='{idPrefix}_Width' /><input type='hidden' id='{idPrefix}_Height' /></div><div class='step-3' id='{idPrefix}_step3'><img id='{idPrefix}_image-cropped' /></div></div>";

//	sMarkup = sMarkup.replace(/{idPrefix}/g, o.idPrefix);
//	sMarkup = sMarkup.replace(/{step1Title}/g, o.step1Title);
//	sMarkup = sMarkup.replace(/{step2Title}/g, o.step2Title);
//	sMarkup = sMarkup.replace(/{step3Title}/g, o.step3Title);

//	$("body").append(sMarkup);

//	$(".button").button();

//	$("_image-to-crop").load(function () {
//		centerOverlay();
//		setUpJCrop();
//	});

//	$("_modal").dialog({
//		autoOpen: false,
//		modal: true,
//		title: o.step1Title,
//		height: 200,
//		width: 350,
//		buttons: {
//			'Avançar': function () { }
//			Cancelar: function () { },
//		}
//	});

//	$("_btnPrev").live('click', function () {
//		if (nCurrStep == 1) {
//			$("_step1").show();
//			$("_step2").hide();
//			$("_btnFinish").hide();
//			$("_btnNext").text("Avançar");
//			$(this).hide();
//		}
//		else if (nCurrStep == 2) {
//			$("_step2").show();
//			$("_step3").hide();
//			$("_btnNext").text("Recortar").show();
//			$("_btnFinish").show();
//			$(this).show();
//		}

//		nCurrStep--;
//		centerOverlay();
//	});

//	$("_btnNext").live('click', function () {
//		if (nCurrStep === 0) {
//			if ($("_fileInput").val() != "") {
//				// file to upload
//				uploadImageFile();
//			}
//			else if ($("_imgUrl").val() != "") {
//				// simple image url
//				sFileName = $("_imgUrl").val();

//				$('.jcrop-holder').remove();

//				$("_image-to-crop")
//					.attr('src', sFileName + '?' + new Date().getTime())
//					.show();
//			}
//			else {
//				jAlert("Por favor, selecione um arquivo um digite o endereço de uma imagem");
//				return;
//			}

//			$("_btnPrev").show();
//			$("_step1").hide();
//			$("_step2").show();
//			$(this).text("Recortar").show();
//			$("_btnFinish").show();
//		}
//		else if (nCurrStep == 1) {
//			requestCrop();

//			$("_step2").hide();
//			$("_step3").show();
//			$("_btnFinish").show();
//			$(this).text("Avançar").hide();
//		}

//		nCurrStep++;
//		centerOverlay();
//	});

//	return this.each(function () {
//		$(this).click(function (e) {
//			$("_fileInput").val("");
//			$("_imgUrl").val("");
//			$("_modal").dialog("open");
//			$("_btnFinish").hide();
//			$("_btnPrev").hide();
//			$("_btnNext").show();
//			$("_step1").show();
//			$("_step2").hide();
//			$("_step3").hide();

//			nCurrStep = 0;

//			centerOverlay();
//		});
//	});

//	function setUpJCrop() {
//		if (bCropImgVisible) {
//			var imgCrop = $("_image-to-crop");

//			var opts = {
//				boxWidth: 800,
//				boxHeight: 600,
//				onSelect: storeCoords
//			};

//			if (sAspectRatio !== null) {
//				options.aspectRatio = sAspectRatio;
//			}

//			jcropAPI = $.Jcrop(imgCrop, opts);
//		}

//		if (jcropAPI !== null) {
//			jcropAPI.setSelect(getSelection());
//			jcropAPI.focus();
//		}

//		Utils.scale($("_image-cropped"), 800, 600);
//	};

//	// Re-center the overlay on the screen
//	function centerOverlay() {
//		// manually re-center the overlay
//		var overlayElem = $("_modal");

//		overlayElem
//			.css("left", ($(window).width() - overlayElem.width()) / 2)
//		    .css("top", ($(window).height() - overlayElem.height()) / 2);
//	};

//	// Fires ajax file upload process
//	function uploadImageFile() {
//		$.ajaxFileUpload({
//			url: options.uploadUrl,
//			secureuri: false,
//			fileElementId: options.idPrefix + "_fileInput",
//			dataType: 'json',
//			success: function (data, status) {
//				sFileName = data.fileName;

//				$('.jcrop-holder').remove();

//				$("_image-to-crop")
//					.attr('src', data.fileName + '?' + new Date().getTime())
//					.show();
//			},
//			error: function (data, status, e) {
//				jAlert(e.message);
//			}
//		});
//	};

//	function storeCoords(coords) {
//		$("_Xaxis").val(coords.x);
//		$("_Yaxis").val(coords.y);
//		$("_Width").val(coords.w);
//		$("_Height").val(coords.h);
//	};

//	function getSelection() {
//		var dim = jcropAPI.getBounds();
//		var cropWidth = dim[0];
//		var cropHeight = dim[1];

//		return [
//		    Math.round(cropWidth / 3),
//		    Math.round(cropHeight / 3),
//		    Math.round((cropWidth / 3) * 2),
//		    Math.round((cropHeight / 3) * 2)];
//	};

//	function requestCrop() {
//		// request image to be cropped
//		var sXaxis = $("_Xaxis").val();
//		var sYaxis = $("_Yaxis").val();
//		var sWidth = $("_Width").val();
//		var sHeight = $("_Height").val();

//		if (sWidth === "0" &&
//			sHeight === "0") {
//			// zero sized crop - do nothing
//			var imgCropped = $("<img />")
//				.attr("src", $("_image-to-crop").attr("src"))
//				.attr("id", options.idPrefix + "_image-cropped")
//				.load(function () {
//					centerOverlay();
//					Utils.scale(imgCropped, 800, 600);
//				})

//			$("_step3" + " > img").remove();
//			$("_step3").append(imgCropped);
//		}
//		else {
//			$.ajax({
//				url: options.cropUrl,
//				data: {
//					xaxis: sXaxis,
//					yaxis: sYaxis,
//					width: sWidth,
//					height: sHeight,
//					filename: sFileName
//				},
//				success: function (data) {
//					var imgCropped = $("<img />")
//						.attr("src", data.fileName + '?' + new Date().getTime())
//						.attr("id", options.idPrefix + "_image-cropped")
//						.load(function () { centerOverlay(); });

//					$("_step3" + " > img").remove();
//					$("_step3").append(imgCropped);
//				},
//				error: function (xhr, status, e) {
//					jAlert("Ocorreu um erro. Tente novamente mais tarde.");
//				},
//				dataType: "json",
//				type: "POST"
//			});
//		}
//	};
//});