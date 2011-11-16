<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Com.Dover.Web.Models.DataTypes.ModuleReference>" %>
<%@ Import Namespace="Microsoft.Web.Mvc" %>

<%	if (Model != null) {
		if (Model.AllItems.Count == 0) { %>
			<br />
			<span>Nenhum registro encontrado.</span>
	<%	}
		else { %>
		<%= Html.DropDownListFor(model => model.Id, Model.AllItems.Select(e => new SelectListItem {
			Text = e.Value,
			Value = e.Id.ToString(),
			Selected = Model.Id == e.Id
		})) %>
	<%	}
	} %>