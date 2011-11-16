<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Com.Dover.Web.Models.ModulesViewModel>" %>
<%@ Import Namespace="Microsoft.Web.Mvc" %>
<%@ Import Namespace="Br.Com.Quavio.Tools.Web.Mvc" %>

<script type="text/javascript">
	var fieldList = [];
</script>

<table cellspacing="1" border="0" class="admin-grid admin-list-grid" id="field-list-table">
    <tr class="grid-header">
        <th>Campo</th>
        <th>Ações</th>
    </tr>
<% 	if (Model != null && Model.Fields != null) {
		foreach (var field in Model.Fields) {
			string fieldIndex = Guid.NewGuid().ToString();
			string prefix = "Fields[" + fieldIndex + "]."; 
			%>
	        <tr class="grid-row">
		        <td>
					<span><%= Html.DisplayFor(model => field.FieldDisplayName)%></span>

					<%= Html.Hidden("Fields.index", fieldIndex)%>
					<%= Html.Hidden(prefix + "ID", field.ID)%>
					<%= Html.Hidden(prefix + "FieldDisplayName", field.FieldDisplayName)%>
					<%= Html.Hidden(prefix + "DataType", field.DataType)%>
					<%= Html.Hidden(prefix + "IsRequired", field.IsRequired)%>
					<%= Html.Hidden(prefix + "IsReadOnly", field.IsReadOnly)%>	
					<%= Html.Hidden(prefix + "ShowInListMode", field.ShowInListMode)%>
					<%= Html.Hidden(prefix + "Metadata", field.Metadata)%>
		        </td>
				<td class="grid-controls">
		            <%= Html.Image("~/Content/Images/Grid/edit.png", new { title = "Editar", Class = "btn-edit-field", indexkey = fieldIndex })%>
		            <%= Html.Image("~/Content/Images/Grid/delete.png", new { title = "Apagar", Class = "btn-delete-module-field confirmable", indexkey = fieldIndex, confirmationmsg = "Tem certeza que deseja apagar este campo?\nEsta operação é irreversível!" })%>
		        </td>
		    </tr>
	<%	}
	} %>
</table>
<a href="#" id="btn-new-field">Novo campo...</a>
<script id="fieldTemplate" type="text/x-jquery-tmpl">
	<tr class="grid-row">
		<td>
			<span>${fieldName}</span>
			<input type="hidden" name="Fields.index" value="${key}" />
			<input type="hidden" name="${displayName.name}" value="${displayName.value}" />
			<input type="hidden" name="${dataType.name}" value="${dataType.value}" />
			<input type="hidden" name="${isRequired.name}" value="${isRequired.value}" />
			<input type="hidden" name="${isReadOnly}" value="false" />
			<input type="hidden" name="${showInList.name}" value="${showInList.value}" />
			<input type="hidden" name="${metadata.name}" value="${metadata.value}" />
		</td>
		<td class="grid-controls">
			<img src="${editImgPath}" class="btn-edit-field" alt="Editar" indexkey="${key}" />
			<img src="${deleteImgPath}" class="btn-delete-module-field confirmable" alt="Apagar" indexkey="${key}" confirmationmsg="Tem certeza que deseja apagar este campo?\nEsta operação é irreversível!" />
		</td>
	</tr>
</script>
