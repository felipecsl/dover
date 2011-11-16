(function (window) {
	moduleReferenceDialog = window.moduleReferenceDialog = {
		fieldType: "Com.Dover.Web.Models.DataTypes.ModuleReference",
		editorText: "Selecionar o módulo...",

		/* 
		* Returns the selected module id
		*/
		getData: function () {
			return $("#module-reference-dropdown").val();
		},

		setData: function (_metadata) {
			this.clearData(); // perform cleanup

			if (_metadata !== null) {
				$("#module-reference-dropdown").val(_metadata);
			}

			var dropdown = $("#module-reference-dropdown");
			var formPane = $("#module-reference-form").show();
			var statusMg = $("#status-msg").hide().text("");

			function onModulesReceived(data) {
				if (data.status === 1) {
					if (data.modules !== null &&
						data.modules.length > 0) {

						$.each(data.modules, function (index, value) {
							dropdown.append(
								$("<option />", {
									value: value.id,
									text: value.moduleName
								}));
						});
					}
					else {
						formPane.hide();
						statusMg.show().text("Nenhum módulo encontrado para esta conta.");
					}
				}
				else {
					formPane.hide();
					statusMg.show().text("Erro obtendo a lista de módulos para esta conta.");
				}
			}

			$.getJSON(getModulesUrl, onModulesReceived);
		},

		/*
		* Clear pre-existing data on the editor dialog
		*/
		clearData: function () {
			$("option", $("#module-reference-dropdown").val("")).remove(); // clear pre-existing entries
		},

		open: function (_metadata) {
			$("#module-reference-dialog").dialog("open");
		}
	};

	$(function () {
		$("#module-reference-dialog").dialog({
			autoOpen: false,
			modal: true,
			buttons: {
				Salvar: function () { $(this).dialog("close"); }
			},
			close: function () { fieldManager.setFieldMetadata(moduleReferenceDialog.getData()); }
		});

		fieldManager.registerFieldEditor(moduleReferenceDialog);
	});

})(window);