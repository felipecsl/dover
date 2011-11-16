(function (window) {
	// Define the global valid values dialog object
	validValuesDialog = window.validValuesDialog = {
		fieldType: "Com.Dover.Web.Models.DataTypes.DropdownButton;Com.Dover.Web.Models.DataTypes.CheckBoxList",
		editorText: "Valores válidos...",
		/* 
		* Returns an array with the current entries in the valid values table 
		*/
		getData: function () {
			var arrValidValues = [];

			$("span", "#valid-values-table").each(function (index) {
				arrValidValues.push($(this).text());
			});

			return arrValidValues;
		},

		setData: function (_metadata) {
			this.clearData();

			if (_metadata !== null &&
				_metadata instanceof Array) {

				$.each(_metadata, function (index, value) {
					validValuesDialog.addValidValue(value);
				});
			}
		},

		/*
		* Clear pre-existing data on the editor dialog
		*/
		clearData: function () {
			$("#valid-values-table tr[class*=grid-row]").remove(); // clear the valid values table
			$("#valid-value").val("");
		},

		/*
		* Adds the specified key and value to the table
		*/
		addValidValue: function (_value) {
			var itemValue = "<span>" + _value + "</span>",
				imgCross = "<img class='btn-delete-field' title='Apagar' src='" + deleteImgPath + "'/>",
				rowClass = "grid-row";

			var rowText = "<tr class='{rowClass}'><td>{imgCross}</td><td>{itemValue}</td></tr>";
			rowText = rowText.replace(/{rowClass}/g, rowClass);
			rowText = rowText.replace(/{imgCross}/g, imgCross);
			rowText = rowText.replace(/{itemValue}/g, itemValue);

			$("#valid-values-table > tbody").append(rowText);

			$("#valid-value").val("");
		},

		open: function () {
			$("#valid-values-dialog").dialog("open");
		}
	};

	$(function () {
		$("#valid-values-dialog").dialog({
			autoOpen: false,
			modal: true,
			buttons: {
				Fechar: function () { $(this).dialog("close"); }
			},
			close: function () { fieldManager.setFieldMetadata(validValuesDialog.getData()); }
		});

		$("#btn-add-valid-value").click(function () {
			validValuesDialog.addValidValue($("#valid-value").val());
		});

		$(".btn-delete-field").live("click", function () {
			$(this).parent().parent().remove();
		});

		fieldManager.registerFieldEditor(validValuesDialog);
	});

})(window);