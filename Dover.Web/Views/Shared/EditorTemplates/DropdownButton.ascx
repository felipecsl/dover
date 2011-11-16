<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Com.Dover.Web.Models.DataTypes.DropdownButton>" %>
<%@ Import Namespace="Microsoft.Web.Mvc" %>

<% 	string modelName = ViewData.TemplateInfo.GetFullHtmlFieldName(null);
	
	if (Model != null) { %>
		<%= Html.DropDownListFor(
			model => model.SelectedValue,
			Model.ValidValues.Select(v => new SelectListItem() {
				Value = v.Key,
				Text = v.Value,
				Selected = Model.SelectedValue == v.Value
			}))%>

	<% 	foreach (var vv in Model.ValidValues) {
			string g = Guid.NewGuid().ToString(); %>
			<input type="hidden" name="<%= modelName %>.ValidValues.index" value="<%= g %>" />
			<input type="hidden" name="<%= modelName + ".ValidValues[" + g + "].key" %>" value="<%= vv.Key %>" />
			<input type="hidden" name="<%= modelName + ".ValidValues[" + g + "].value" %>" value="<%= vv.Value %>" />
	<%	}
	} %>