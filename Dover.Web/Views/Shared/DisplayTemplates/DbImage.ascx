<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Com.Dover.Web.Models.DataTypes.DbImage>" %>

<% 	if (Model != null)
   	{ %>
	    <%: Model.AbsolutePath %>
<%	} %>