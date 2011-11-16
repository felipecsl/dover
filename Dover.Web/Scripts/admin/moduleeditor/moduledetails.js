$(function () {
	$("#btn-new-field").click(function (e) {
		e.preventDefault();

		fieldManager.open({
			callback: onDataAvailable
		});
	});

	$(".btn-edit-field").live("click", function () {
		var indexKey = $(this).attr("indexkey");
		var prefix = "input[name='Fields[" + indexKey + "].";

		fieldManager.open({
			callback: onDataAvailable,
			data: {
				key: indexKey,
				displayName: $(prefix + "FieldDisplayName']").val(),
				dataType: $(prefix + "DataType']").val(),
				isRequired: $(prefix + "IsRequired']").val().toLowerCase() === "true",
				showInList: $(prefix + "ShowInListMode']").val().toLowerCase() == "true",
				metadata: $(prefix + "Metadata']").val()
			}
		});
	});

	$(".btn-delete-module-field").live("click", function () {
		var evtTarget = $(this),
			indexKey = evtTarget.attr("indexkey"),
			prefix = "input[name='Fields[" + indexKey + "].",
			rowId = $(prefix + "ID']");

		if (rowId.length == 0) {
			// the field has not been saved yet.
			// Just remove the row from the table
			evtTarget.parent().parent().remove();
		}
		else {
			$.ajax({
				type: "POST",
				url: deleteFielPath,
				data: "id=" + rowId.val(),
				dataType: "json",
				success: function (data, textStatus, xhr) {
					displayFlash(data.msg);

					if (data.result == 0) {
						evtTarget.parent().parent().remove();
					}
				},
				error: function (xhr, textStatus, err) {
					displayFlash("Ocorreu uma falha ao remover o campo.");
				}
			});
		}
	});

	$("#btn-submit").click(function () {
		if ($("#DisplayName").val() === String.empty) {
			$("#DisplayName").parent().append($("<span class='field-validation-error'>Você deve preencher o nome do módulo.</span>"));
			return false;
		}
	});

	if ($("td", "#field-list-table").length == 0) {
		$("#field-list-table").hide();
	}

	function onDataAvailable(_data) {
		var indexElem = $("input[name='Fields.index'][value='" + _data.key + "']"),
			tableCell = null,
			namePrefix = "Fields[" + _data.key + "].",
			hiddenElem = "<input type='hidden' />";

		if (indexElem.length === 0) {	// new field
			// add the entry to the table
			var data = {
				editImgPath: editImgPath,
				deleteImgPath: deleteImgPath,
				fieldName: _data.displayName,
				key: _data.key,
				isReadOnly: namePrefix + "IsReadOnly",
				displayName: {
					name: namePrefix + "FieldDisplayName",
					value: _data.displayName
				},
				dataType: {
					name: namePrefix + "DataType",
					value: _data.dataType
				},
				isRequired: {
					name: namePrefix + "IsRequired",
					value: _data.isRequired
				},
				showInList: {
					name: namePrefix + "ShowInListMode",
					value: _data.showInList
				},
				metadata: {
					name: namePrefix + "Metadata",
					value: _data.metadata !== null ? JSON.stringify(_data.metadata) : ""
				}
			};

			$("#fieldTemplate").tmpl(data).appendTo("#field-list-table > tbody");
		}
		else {	// edited field
			var selectorPrefix = "input[name='Fields[" + _data.key + "].";

			$(selectorPrefix + "FieldDisplayName']").val(_data.displayName);
			$(selectorPrefix + "DataType']").val(_data.dataType);
			$(selectorPrefix + "IsRequired']").val(_data.isRequired);
			$(selectorPrefix + "Metadata']").val(_data.metadata !== null ? JSON.stringify(_data.metadata) : "");

			tableCell = $(selectorPrefix + "ShowInListMode']").val(_data.showInList).parent();

			$(":first-child", tableCell).text(_data.displayName);
		}

		$("#field-list-table").show();
	}
});