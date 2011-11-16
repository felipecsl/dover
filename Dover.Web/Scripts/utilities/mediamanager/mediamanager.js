var saveCallback = function (imgPath) { };
var jcropAPI = null;

$(function () {
	$("img.imggal-thumb").lazyload();

	$("#btn-cancel-crop").hide();

	var isIE7 = $.browser.version.msie && $.browser.version == "7.0";

	if (!isIE7) {
		// IE 7 doesn't handle very well out centering function
		$(window).bind("resize", function () { centerDialog(); });
	}

	$(".modal-dialog").dialog({
		//autoOpen: false,
		modal: true,
		width: 800,
		buttons: {
			Salvar: function () {
				var selIndex = $("#tabs").tabs("option", "selected");
				var imgPaths = []; // array that holds all the selected images to be returned to the calee

				if (selIndex == 1) {
					// Second tab: De uma URL
					saveCallback([$("#image-url").val()]);
					return;
				}

				// First tab: De meu computador
				var listContainer = (selIndex == 0) ? "#fsUploadProgress" : "#mediagal-list";
				var arrSelectedItems = $("div.itm-expanded", listContainer);

				if (arrSelectedItems.length == 0) {
					alert("Você deve escolher pelo menos uma imagem.");
					return false;
				}

				arrSelectedItems.each(function (i) {
					if ($("span.select-imagegal-item", this).text() == "Selecionado") {
						imgPaths.push($("img", this).attr("src"));
					}
				});

				saveCallback(imgPaths);
			},
			Cancelar: function () {
				saveCallback([]);
			}
		},
		close: function (event, ui) {
			saveCallback([]);
		},
		open: function () {

		}
	});

	$("#tabs").tabs();

	$("#btn-cancel-crop a").click(function () {
		$("#btn-edit-image").val("Recortar...");

		clearCropPanel();
	});

	$("#btn-edit-image").click(function () {
		var thisElem = $(this);

		// Show the crop controls
		if (thisElem.val() == "Recortar...") {
			thisElem.val("Salvar");

			$("#image-attributes").hide();

			$("#image-to-crop").removeClass("uploaded-image-preview");
			$("#btn-cancel-crop").show();

			jcropAPI = $.Jcrop("#image-to-crop", {
				boxWidth: 400,
				boxHeight: 250,
				onSelect: function (c) {
					$("#crop-x-axis").val(c.x);
					$("#crop-y-axis").val(c.y);
					$("#crop-width").val(c.w);
					$("#crop-height").val(c.h);
				}
			});
		}

		// Hide the crop controls
		else {
			thisElem.val("Recortar...");

			$.ajax({
				url: cropUrl,
				data: {
					xaxis: $("#crop-x-axis").val(),
					yaxis: $("#crop-y-axis").val(),
					width: $("#crop-width").val(),
					height: $("#crop-height").val(),
					filename: $("#image-to-crop").attr("src")
				},
				dataType: "json",
				type: "POST",
				success: function (data, status, xhr) {
					clearCropPanel();

					var currSrc = $("#image-to-crop").attr("src");
					$("#image-to-crop").attr("src", currSrc.substring(0, currSrc.indexOf("?ts")) + "?ts=" + new Date().getTime());

					$("#image-dimensions").text("Dimensões: " + $("#crop-width").val() + " x " + $("#crop-height").val());
				},
				error: function () {
					alert("Ocorreu um erro ao recortar a sua imagem.");
					clearCropPanel();
				}
			});
		}
	});

	/* 
	* set up AJAX file upload request
	*/
	/*$("#uploadForm").ajaxForm({
	success: function (data) {
	$("#crop-panel").show();

	$("#image-to-crop")
	.attr('src', data.imagePath + '?ts=' + new Date().getTime())
	.show();

	$("#image-filename").text("Nome do arquivo: " + data.fileName);
	$("#image-dimensions").text("Dimensões: " + data.width + " x " + data.height);
	},
	error: function (xhr, textStatus, errorThrown) {
	alert("Ocorreu uma falha ao realizar o envio da imagem. Por favor, tente novamente mais tarde.");
	},
	dataType: 'json',
	type: "POST"
	});*/

	/* 
	* AJAX call that retrieves files in the user media gallery 
	*/
	$("#tabbtn-medialibrary").click(function () {
		if (galleryFileList.count() > 0) {
			return true; // skip reloading if the list is already filled
		}

		$.ajax({
			url: mediaGalleryUrl,
			dataType: 'json',
			success: function (data, textStatus, xhr) {
				if (data == null) {
					alert("Ocorreu uma falha ao obter a lista de arquivos. Por favor, tente novamente mais tarde.");
					return;
				}

				var contentPane = $("div.ajax-loader", "#mediagal-list").remove();

				if (data.length == 0) {
					contentPane.append("No momento, não há arquivos em sua biblioteca de mídia.");
					return;
				}

				$.each(data, function (index, value) {
					if (Utils.isImageFile(value.fileName)) {
						var newFile = galleryFileList.addFile(value.fileName, Utils.random.getId(8), value.imagePath);
						newFile.setCompleted(value);
					}
				});
			},
			error: function (xhr, textStatus, error) {
				alert("Ocorreu uma falha ao obter a lista de arquivos. Por favor, tente novamente mais tarde.");
			}
		});
	});
});

/*
 * External function for specifying a callback to be invoked
 * when the image has been selected by the user
 */
function imgSelectedEventListener(_fnCallback) {
    saveCallback = _fnCallback;
};

/*
 * External function for requesting the overlay dialog to be opened
 * Params: 
 *  _bMultiselect: Tells whether the dialog permits multiple file selection
 */
function openDialog(_bMultiselect) {
	var multiSelect = (typeof (_bMultiselect) === 'boolean') ? _bMultiselect : false;
	
	uploadFileList.multiSelect = multiSelect;
	galleryFileList.multiSelect = multiSelect;

    // reset fields and panels
	$("#crop-panel").hide();
    $("#image-url").val("");
    $("#image-filename").text("");
    $("#image-dimensions").text("");
    $("#image-to-crop").attr("src", "");

    uploadFileList.clear();
    galleryFileList.clear();

    // open the dialog overlay
    $(".modal-dialog").dialog("open");

    var selIndex = $("#tabs").tabs("option", "selected");

    if (selIndex == 2) {
    	// refresh the gallery images
    	$("#tabbtn-medialibrary").click();
    }

    // IE FIX: Give IE some time to show the widget so 
    // that we can center it on the screen
	setTimeout(centerDialog, 100);
};

function centerDialog() {
    var overlayElem = $(".modal-dialog")
	    .dialog("option", "height", $(window).height() - 50)
	    .dialog("widget");

    overlayElem
        .css("left", ($(window).width() - overlayElem.width()) / 2)
        .css("top", ($(window).height() - overlayElem.height()) / 2);
   };

function clearCropPanel() {
	jcropAPI.destroy();
	$("#image-to-crop").addClass("uploaded-image-preview");
	$("#btn-cancel-crop").hide();
	$("#image-attributes").show();
};