function fileQueued(file) {
	try {
		uploadFileList.addFile(file.name, file.id);
	}
	catch (ex) {
		this.debug(ex);
	}
}

function fileQueueError(file, errorCode, message) {
	try {
		if (errorCode === SWFUpload.QUEUE_ERROR.QUEUE_LIMIT_EXCEEDED) {
			this.debug("Você tentou enviar arquivos demais.\n" + (message === 0 ? "Você atingiu o limite de uploads." : "Você pode selecionar " + (message > 1 ? "até " + message + " arquivos." : "arquivo.")));
			return;
		}

		var status = "";

		switch (errorCode) {
			case SWFUpload.QUEUE_ERROR.FILE_EXCEEDS_SIZE_LIMIT:
				status = "Arquivo grande demais.";
				this.debug("Error Code: File too big, File name: " + file.name + ", File size: " + file.size + ", Message: " + message);
				break;
			case SWFUpload.QUEUE_ERROR.ZERO_BYTE_FILE:
				status = "Impossível enviar: arquivo vazio.";
				this.debug("Error Code: Zero byte file, File name: " + file.name + ", File size: " + file.size + ", Message: " + message);
				break;
			case SWFUpload.QUEUE_ERROR.INVALID_FILETYPE:
				status = "Tipo de arquivo inválido.";
				this.debug("Error Code: Invalid File Type, File name: " + file.name + ", File size: " + file.size + ", Message: " + message);
				break;
			default:
				if (file !== null) {
					status = "Erro desconhecido";
				}
				this.debug("Error Code: " + errorCode + ", File name: " + file.name + ", File size: " + file.size + ", Message: " + message);
				break;
		}

		uploadFileList.getFile(file.id).setStatus(status);
	} 
	catch (ex) {
		this.debug(ex);
	}
}

function fileDialogComplete(numFilesSelected, numFilesQueued) {
	try {
		if (numFilesSelected > 0) {
			$("#" + this.customSettings.cancelButtonId).attr("disabled", "false");
		}

		/* I want auto start the upload and I can do that here */
		this.startUpload();
	} 
	catch (ex) {
		this.debug(ex);
	}
}

function uploadStart(file) {
	try {
		uploadFileList.getFile(file.id).setStatus("Enviando...");
	}
	catch (ex) {
		this.debug(ex);
	}
	return true;
}

function uploadProgress(file, bytesLoaded, bytesTotal) {
	try {
		var percent = Math.ceil((bytesLoaded / bytesTotal) * 100);

		uploadFileList.getFile(file.id).setProgress(percent);
	} 
	catch (ex) {
		this.debug(ex);
	}
}

function uploadSuccess(file, serverData) {
	try {
		// Parse the server JSON response, which contains the file data
		var fileData = JSON.parse(serverData);

		uploadFileList.getFile(file.id).setCompleted(fileData);
	} 
	catch (ex) {
		this.debug(ex);
	}
}

function uploadError(file, errorCode, message) {
	try {
		var status = "";
		
		switch (errorCode) {
			case SWFUpload.UPLOAD_ERROR.HTTP_ERROR:
				status = "Erro HTTP " + message;
				this.debug("Error Code: HTTP Error, File name: " + file.name + ", Message: " + message);
				break;
			case SWFUpload.UPLOAD_ERROR.UPLOAD_FAILED:
				status = "Falha de upload.";
				this.debug("Error Code: Upload Failed, File name: " + file.name + ", File size: " + file.size + ", Message: " + message);
				break;
			case SWFUpload.UPLOAD_ERROR.IO_ERROR:
				status = "Erro de servidor (IO)";
				this.debug("Error Code: IO Error, File name: " + file.name + ", Message: " + message);
				break;
			case SWFUpload.UPLOAD_ERROR.SECURITY_ERROR:
				status = "Falha de segurança.";
				this.debug("Error Code: Security Error, File name: " + file.name + ", Message: " + message);
				break;
			case SWFUpload.UPLOAD_ERROR.UPLOAD_LIMIT_EXCEEDED:
				status = "Limite de upload excedido.";
				this.debug("Error Code: Upload Limit Exceeded, File name: " + file.name + ", File size: " + file.size + ", Message: " + message);
				break;
			case SWFUpload.UPLOAD_ERROR.FILE_VALIDATION_FAILED:
				status = "Falha de validação. Arquivo ignorado.";
				this.debug("Error Code: File Validation Failed, File name: " + file.name + ", File size: " + file.size + ", Message: " + message);
				break;
			case SWFUpload.UPLOAD_ERROR.FILE_CANCELLED:
				// If there aren't any files left (they were all cancelled) disable the cancel button
				if (this.getStats().files_queued === 0) {
					$("#" + this.customSettings.cancelButtonId).attr("disabled", true);
				}
				status = "Cancelado";
				break;
			case SWFUpload.UPLOAD_ERROR.UPLOAD_STOPPED:
				status = "Interrompido";
				break;
			default:
				status = "Erro desconhecido: " + errorCode;
				this.debug("Error Code: " + errorCode + ", File name: " + file.name + ", File size: " + file.size + ", Message: " + message);
				break;
		}

		uploadFileList.getFile(file.id).setErrorState(status);
	} 
	catch (ex) {
		this.debug(ex);
	}
}

function uploadComplete(file) {
	if (this.getStats().files_queued === 0) {
		$("#" + this.customSettings.cancelButtonId).attr("disabled", "true");
	}
}

// This event comes from the Queue Plugin
function queueComplete(numFilesUploaded) { }