(function (window) {
	/** Instantiate global field manager object **/
	fieldManager = window.fieldManager = {
		metadataObject: null,
		fieldEditors: [],

		open: function (_options) {
			callback = _options.callback || null;

			$(".field-validation-error", txtDispName.val("").parent()).remove(); // clear any old field name and validation errors
			cbIsRequired.attr("checked", false);
			cbShowInList.attr("checked", false);

			// editing a pre-existing field
			if (_options.data !== undefined) {
				currFieldKey = _options.data.key;

				txtDispName.val(_options.data.displayName);
				txtDataType.val(_options.data.dataType);
				cbIsRequired.attr("checked", _options.data.isRequired);
				cbShowInList.attr("checked", _options.data.showInList);

				// store the field metadata, if any
				if (_options.data.metadata !== null &&
					_options.data.metadata !== "") {
					fieldManager.metadataObject = JSON.parse(_options.data.metadata);
				}
			}
			else {	// creating a new field
				currFieldKey = fieldCount++;
			}

			this.updateFieldEditorText();

			$("#create-field-overlay").dialog("open");
		},

		registerFieldEditor: function (_data) {
			this.fieldEditors.push(_data);
		},

		getCurrentFieldEditor: function () {
			var dataType = txtDataType.val();
			var bFound = false;

			return JSLINQ(fieldManager.fieldEditors).Last(function (item) {

				// Field editors may register multiple semicolon-separated data types
				$.each(item.fieldType.split(";"), function (i, val) {
					if (val == dataType) {
						bFound = true;
						return;
					}
				});

				return bFound;
			});
		},

		setFieldMetadata: function (_metadata) {
			this.metadataObject = _metadata;
		},

		getFieldMetadata: function () {
			var editor = fieldManager.getCurrentFieldEditor();

			return (editor !== null) ? editor.getData() : null;
		},

		updateFieldEditorText: function () {
			var editor = fieldManager.getCurrentFieldEditor();

			if (editor !== null) {
				$("#btn-additional-data").show().text(editor.editorText);
				editor.clearData();
			}
			else {
				$("#btn-additional-data").hide();
			}
		},

		openEditor: function () {
			// retrieve the field editor associated with the selected data type
			var editor = fieldManager.getCurrentFieldEditor();

			if (editor !== null) {
				if (fieldManager.metadataObject !== null) {
					editor.setData(fieldManager.metadataObject);

					editor.setData(fieldManager.metadataObject);
				}

				editor.open();
			}
		}
	};

	var txtDispName = null,
		txtDataType = null,
		cbIsRequired = null,
		cbShowInList = null,
		callback = function () { },
		fieldCount = 0,
		currFieldKey = -1;

	/** Set up event listeners **/
	$(function () {
		txtDispName = $("#FieldDisplayName");
		txtDataType = $("#DataType");
		cbIsRequired = $("#IsRequired");
		cbShowInList = $("#ShowInListMode");

		txtDataType.change(fieldManager.updateFieldEditorText);

		$("#btn-additional-data").click(function () {
			fieldManager.openEditor();

			return false;
		});

		$("#create-field-overlay").dialog({
			autoOpen: false,
			modal: true,
			width: 400,
			buttons: {
				Salvar: function () {
					if (txtDispName.val() === "") {
						// validation error: display name cannot be blank
						txtDispName.parent()
							.append($("<span class='field-validation-error'></span>").text("Você deve preencher o nome do campo."));

						return false;
					}

					callback({
						key: currFieldKey,
						displayName: txtDispName.val(),
						dataType: txtDataType.val(),
						isRequired: cbIsRequired.attr("checked"),
						showInList: cbShowInList.attr("checked"),
						metadata: fieldManager.metadataObject
					});

					$(this).dialog("close");
				},
				Cancelar: function () {
					$(this).dialog("close");
				}
			},
			close: function () { }
		});
	});
})(window);