function File(_id, _targetContainerId, _fileList) {
	this.id = _id;
	this.fileElem = $("#" + _id, _targetContainerId);
	this.selected = false;
	this.expanded = false;
	this.ownerList = _fileList;

	var that = this;
	
	/*
	* Sets the file download/upload progress (eg.: 55%)
	*/
	this.setProgress = function (_progress) {
		$(".progress-bar", that.fileElem).css("width", _progress + "%");
	};

	/*
	* Sets the file download/upload status (eg.: Complete or Failed)
	*/
	this.setStatus = function (_status) {
		$("span.progress-text", that.fileElem).text(_status);
	};

	this.setErrorState = function (_status) {
		$("span.view-imagegal-item", that.fileElem).hide();					// hide the expand details button
		$(".progress-bar", that.fileElem).css("width", "100%").fadeOut(); // hide the progress bar
		$("span.progress-text", that.fileElem).text("Erro de envio - " + _status);
	};

	this.setCompleted = function (_fileData) {
		$("span.progress-text", that.fileElem).hide();
		$(".progress-bar", that.fileElem).css("width", "100%").fadeOut();

		// Show the image thumbnail
		$("img.uploaded-image", that.fileElem).attr("src", _fileData.imagePath + '?ts=' + new Date().getTime()).show();

		var tmplData = {
			imgPath: _fileData.imagePath,
			fileName: Utils.getFileName(_fileData.fileName),
			uploadDate: new Date().format("dd/mm/yyyy"),
			dimension: _fileData.width + " x " + _fileData.height,
			link: _fileData.linkUrl
		};
		// Fill the image details
		var template = $("#imgDetailsTmpl").tmpl(tmplData).appendTo($("div.progress-content", that.fileElem));
		// Show the "Exibir" button and attach event listeners
		$("span.view-imagegal-item", this.fileElem).show().toggle(that.expand, that.collapse);
		$("span.select-imagegal-item", this.fileElem).toggle(that.select, that.unselect);
		$("div.progress-content", this.fileElem).click(function () { $("span.select-imagegal-item", this).click(); });
	};

	/*
	* Expand the file panel and show the details
	*/
	this.expand = function (_event) {
		if (that.expanded) {
			return true;
		}

		try {
			// slide down this item's details
			$(this).text("Fechar").next().slideToggle().end();

			that.expanded = true;
		}
		catch (err) {
			console.log(err);
		}
	};

	/* 
	* Collapse the file panel and hide the details
	*/
	this.collapse = function (_event) {
		if (!that.expanded) {
			return true;
		}

		try {
			var viewElem = (_event !== undefined) ? $(this) : $("span.view-imagegal-item", this.fileElem);

			viewElem.text("Exibir").next().slideToggle().end();
			// if we're closing an already selected item, unselct it
			/*var btnSelect = $("span.select-imagegal-item", viewElem.parent());
			if (btnSelect.text() == "Selecionado") {
				btnSelect.click();
			}*/

			that.expanded = false;
		}
		catch (err) {
			console.log(err);
		}
	};

	/*
	* Sets the file to selected state and adds a color highlight
	*/
	this.select = function (_event) {
		if (that.selected) {
			return true;
		}

		try {
			var selectElem = (_event !== undefined) ? $(this) : $("span.select-imagegal-item", this.fileElem);

			var container = selectElem.text("Selecionado")
				.parent().parent().parent().parent()
				.addClass("itm-expanded")
				.animate({ backgroundColor: "#f8e354" }, 1000);

			if (!that.ownerList.multiSelect) {
				that.ownerList.closeAllFilesExcept(that.id);
			}

			that.selected = true;
		}
		catch (err) {
			console.log(err);
		}
	};

	/*
	* Unselects the specified file and removes the color highlight
	*/
	this.unselect = function (_event) {
		if (!that.selected) {
			return true;
		}

		try {
			var selectElem = (_event !== undefined) ? $(this) : $("span.select-imagegal-item", this.fileElem);

			selectElem.text("Selecionar")
				.parent().parent().parent().parent()
				.removeClass("itm-expanded")
				.animate({ backgroundColor: "#fff" }, 1000);

			that.selected = false;
		}
		catch (err) {
			console.log(err);
		}
	};
};