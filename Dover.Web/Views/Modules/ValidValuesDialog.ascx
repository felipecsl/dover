<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>
<%@ Import Namespace="Microsoft.Web.Mvc" %>

<div id="valid-values-dialog" title="Valores válidos">
	<div class="editor-label"><label for="email">Valor:</label></div>
	<div class="editor-field"><input type="text" name="valid-value" id="valid-value" value="" class="text ui-widget-content ui-corner-all" /></div>
	<br />
	<input type="button" value="Adicionar" id="btn-add-valid-value" />
	<br />
	<br />
	<table cellspacing="1" border="0" style="border: 1px solid #99bbe8;" class="admin-grid admin-list-grid" align="center" id="valid-values-table">
    	<tr class="grid-header">
			<th></th>
			<th>Valor</th>
		</tr>
	 	<tr class="grid-row">
		</tr>
	</table>
</div>