<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Com.Dover.Web.Models.DataTypes.Password>" %>
<%	if (Model != null) { %>
		<%= Com.Dover.Web.Models.DataTypes.Password.BogusText%>
<%	}
	else { %>
		********
<%	} %>