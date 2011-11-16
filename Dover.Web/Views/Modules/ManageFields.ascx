<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<SelectList>" %>
<%@ Import Namespace="Microsoft.Web.Mvc" %>

<script type="text/javascript">
	var editImgPath = '<%= Url.Content("~/Content/Images/Grid/edit.png") %>',
		deleteImgPath = '<%= Url.Content("~/Content/Images/Grid/delete.png") %>',
		deleteFielPath = '<%= Url.Action("DeleteField") %>';

</script>
<div id="create-field-overlay" title="Adicionar campo">
	<div class="editor-field"><input type="hidden" name="FieldName" id="FieldName" class="text ui-widget-content ui-corner-all" /></div>
	<div class="editor-label"><label for="FieldDisplayName">Nome do campo:</label></div>
	<div class="editor-field"><input type="text" name="FieldDisplayName" id="FieldDisplayName" class="text ui-widget-content ui-corner-all" /></div>
	<div class="editor-label"><label for="DataType">Tipo de dado:</label></div>
	<div class="editor-field"><%= Html.DropDownList("DataType", Model)%> <a href="#" id="btn-additional-data"></a></div>
	<div class="editor-field editor-checkbox">
		<%= Html.CheckBox("IsRequired")%>
		<label for="IsRequired">Obrigatório</label>
	</div>
	<div class="editor-field editor-checkbox">
		<%= Html.CheckBox("ShowInListMode")%>
		<label for="ShowInListMode">Visível na listagem</label>
	</div>
</div>
<% Html.RenderAction("FieldEditor", new { fieldName = "ValidValuesDialog" }); %>
<% Html.RenderAction("FieldEditor", new { fieldName = "ModuleReferenceDialog" }); %>