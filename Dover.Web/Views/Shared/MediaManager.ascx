<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<%@ Import Namespace="Microsoft.Web.Mvc" %>
<%@ Import Namespace="Br.Com.Quavio.Tools.Web.Mvc" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
	<title>Dover Media Manager</title>

	<%	var uploadPath = Url.Action("UploadImage", "Home", new { area = "" }); %>
	<%	Html.RenderPartial("MasterHeadIncludes"); %>
	<script>
		var mediaGalleryUrl = '<%= Url.Action("MediaGallery", "Home", new { area = "" }) %>',
            cropUrl = '<%= Url.Action("CropImage", "Home", new { area = "" }) %>',
			authToken = "<% = Request.Cookies[FormsAuthentication.FormsCookieName] == null ? string.Empty : Request.Cookies[FormsAuthentication.FormsCookieName].Value %>",
			swfUploadPath = '<%= Url.Content("~/Scripts/libs/swfupload/swfupload.swf") %>',
			swfUploadButtonPath = '<%= Url.Content("~/Content/Images/swfuploadbutton.png") %>',
			uploadAction = '<%= Url.Action("UploadImage", "Home", new { area = "" }) %>';

		var uploadFileList;
		var galleryFileList;

		$(function () {
			if (typeof (swfu) == "undefined") {
				swfu = new SWFUpload({
					flash_url: swfUploadPath,
					upload_url: uploadAction,
					post_params: { token: authToken },
					file_size_limit: "100 MB",
					file_types: "*.png; *.jpg; *.jpeg; *.gif",
					file_types_description: "Imagens",
					file_upload_limit: 100,
					file_queue_limit: 0,
					custom_settings: {
						progressTarget: "fsUploadProgress",
						cancelButtonId: "btnCancel"
					},
					debug: false,

					// Button settings
					button_image_url: swfUploadButtonPath,
					button_width: "133",
					button_height: "28",
					button_placeholder_id: "spanButtonPlaceHolder",
					button_text: '<span class="theFont">Escolher arquivos</span>',
					button_text_style: ".theFont { font-size: 13px; font-family: Lucida Grande,Lucida Sans,Arial,sans-serif; }",
					button_text_left_padding: 5,
					button_text_top_padding: 5,

					// The event handler functions are defined in handlers.js
					file_queued_handler: fileQueued,
					file_queue_error_handler: fileQueueError,
					file_dialog_complete_handler: fileDialogComplete,
					upload_start_handler: uploadStart,
					upload_progress_handler: uploadProgress,
					upload_error_handler: uploadError,
					upload_success_handler: uploadSuccess,
					upload_complete_handler: uploadComplete,
					queue_complete_handler: queueComplete	// Queue plugin event
				});

				swfu.debug = function (text) {
					if (typeof (console) != "undefined") {
						console.log(text);
					}
				}

				uploadFileList = new FileList(swfu.customSettings.progressTarget);
			}

			galleryFileList = new FileList("mediagal-list");
		});
	</script>

	<style type="text/css">
		html, body { background-color: transparent; }
		.ui-widget-overlay { width: 100% !important; height: 100% !important; }
	</style>
</head>
<body>
	<div class="modal-dialog" title="Adicionar imagem">
		<div class="media-dialog">
			<div id="tabs">
				<ul>
					<li><a href="#tabs-1">De meu computador</a></li>
					<li><a href="#tabs-2">De uma URL</a></li>
					<li><a href="#tabs-3" id="tabbtn-medialibrary">Biblioteca de mídia</a></li>
				</ul>
				<div id="tabs-1">
					<div class="editor-field">
						<form id="uploadForm" action="<%= uploadPath %>" method="post" enctype="multipart/form-data">
							<div class="uploadcontrols">
								<span id="spanButtonPlaceHolder"></span>
								<input id="btnCancel" type="button" value="Cancel All Uploads" onclick="swfu.cancelQueue();" disabled="disabled" style="margin-left: 2px; font-size: 8pt; height: 29px;" />
							</div>
							<p>Tamanho máximo por arquivo: 100MB</p>
                        </form>
					</div>
					<div class="horiz-separator"></div>
					<div id="fsUploadProgress">
					</div>
					
					<div id="crop-panel">
						<div class="fl crop-left">
							<img id="image-to-crop" src="" alt="Prévia da imagem" class="uploaded-image-preview" />
							<input type="button" value="Recortar..." id="btn-edit-image" />
							<span id="btn-cancel-crop"> ou <a href="#" class="underline">Cancelar</a></span>
						</div>
						<div class="fl" id="image-attributes">
							<span id="image-filename" class="image-info"></span><br />
							<span id="image-dimensions" class="image-info"></span>
						</div>
						<input type="hidden" id="crop-x-axis" />
						<input type="hidden" id="crop-y-axis" />
						<input type="hidden" id="crop-width" />
						<input type="hidden" id="crop-height" />
					</div>
					<%--<div id="results-panel">
						<img id="results" />
					</div>--%>
				</div>
				<div id="tabs-2">
					<div class="editor-label">
						<label for="url">URL da Imagem:</label>
					</div>
					<div class="editor-field">
						<input type="text" value="" id="image-url" class="text-box single-line" style="width: 280px;" />
					</div>
					<%--<img id="image-preview" />--%>
				</div>
				<div id="tabs-3" class="mediagal-content">
                    <ul>
                        <li>Todos os tipos | </li>
                        <li class="bold">Imagens</li>
                    </ul>
                    <div class="clear" id="mediagal-list">
						<div class="ajax-loader">
							<%= Html.Image("~/Content/Images/ajax-loader.gif")%>
							Por favor, aguarde... 
						</div>
                    </div>
                </div>
			</div>
		</div>
	</div>
	<script id="galtmpl" type="text/x-jquery-tmpl">
		<div class="mediagal-item">
			<div class="progress-bar"></div>
			<img src="${galleryImg}" class="uploaded-image" />
			<div class="progress-content">
				<span class="progress-text bold">Pendente...</span> ${bareFileName}
				<span class='float-right view-imagegal-item'>Exibir</span>
			</div>
		</div>
	</script>
	<script id="imgDetailsTmpl" type="text/x-jquery-tmpl">
		<div class='imggal-itemexpanded'>
			<img src="${imgPath}" class="imggal-thumb" />
			<div class="infos">
				<span class='image-info'><b>Nome do arquivo:</b> ${fileName}</span><br />
				<span class='image-info'><b>Data de envio:</b> ${uploadDate}</span><br />
				<span class='image-info'><b>Dimensões:</b> ${dimension}</span><br />
				<span class='image-info'><b>Link:</b> ${link}</span>
				<span class='float-right select-imagegal-item'>Selecionar</span>
			</div>
		</div>
	</script>
</body>
</html>