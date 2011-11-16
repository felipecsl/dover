<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Com.Dover.Web.Models.DataTypes.File>" %>

<% 	if (Model != null)
   	{ %>
	    <%: Model.FilePath %>
<%	} %>