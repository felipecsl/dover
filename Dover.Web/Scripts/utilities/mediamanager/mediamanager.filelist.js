(function () {
	function FileList(_targetContainerId) {
		/* Allow selecting multiple files at once */
		this.multiSelect = false;

		/* Target element (file list container element id) */
		this.targetContainerId = "#" + _targetContainerId;

		/* Private array with all the file objects */
		var allFiles = [];

		/* returns the number of files in the list */
		this.count = function () {
			return allFiles.length;
		};

		var that = this;

		// set up Select All/Unselect All button
		$(this.targetContainerId).append(
			$('<p />', {
				'class': "right-align underline select-all",
				id: "btn-select-all",
				text: "Marcar tudo",
				click: function () {
					var thisObj = $(this);

					if (thisObj.text() == "Marcar tudo") {
						thisObj.text("Desmarcar tudo");
						that.selectAll();
					}
					else {
						thisObj.text("Marcar tudo");
						that.unselectAll();
					}
				}
			}).hide()
		);

		/*
		* Adds a file to the file list.
		*/
		this.addFile = function (_fileName, _fileId, _imgPath) {
			$("#galtmpl")
				.tmpl({
					bareFileName: _fileName,
					galleryImg: _imgPath || String.empty
				})
				.appendTo($(this.targetContainerId))
				.attr("id", _fileId);

			var newFile = new File(_fileId, this.targetContainerId, this);

			allFiles.push(newFile);

			if (this.multiSelect) {
				$("#btn-select-all", this.targetContainerId).show();
			}

			return newFile;
		};

		/* 
		* Removes all files and clears the list
		*/
		this.clear = function () {
			allFiles = [];
			$("div.mediagal-item", this.targetContainerId).remove();
			$("#btn-select-all", this.targetContainerId).hide();
		};

		/*
		* Returns the file specified by the provided file id
		* If the id is not found, null is returned.
		*/
		this.getFile = function (_fileId) {
			return JSLINQ(allFiles).First(function (item) { return item.id == _fileId; });
		};

		/*
		* Closes all files in the list, except the file
		* identified by the specified _fileId.
		*/
		this.closeAllFilesExcept = function (_fileId) {
			var files = JSLINQ(allFiles).Where(function (item) { return item.id != _fileId; }).ToArray();

			$.each(files, function (i, val) {
				val.collapse();
				val.unselect();
			});
		};

		/*
		* Returns all files in the list
		*/
		this.getAllFiles = function () {
			return allFiles;
		};

		/*
		* Selects all the files in the list
		*/
		this.selectAll = function () {
			$.each(allFiles, function (i, val) {
				val.select();
			});
		};

		/*
		* Unselects all the files in the list
		*/
		this.unselectAll = function () {
			$.each(allFiles, function (i, val) {
				val.unselect();
			});
		};

		/*
		* Returns an array with the list of selected files. If no
		* files are selected, an empty array is returned
		*/
		this.getSelectedFiles = function () {
			return JSLINQ(allFiles).Where(function (item) { return item.selected; });
		};
	};

	window.FileList = FileList;
})();