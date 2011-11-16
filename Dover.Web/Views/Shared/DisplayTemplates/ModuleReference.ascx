<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Com.Dover.Web.Models.DataTypes.ModuleReference>" %>

<%	if (Model != null) { %>
		<%= Model.SelectedItem %>
<%	} %>