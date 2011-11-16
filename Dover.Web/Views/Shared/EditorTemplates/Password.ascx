<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Com.Dover.Web.Models.DataTypes.Password>" %>

<%
	string sName = ViewData.TemplateInfo.GetFullHtmlFieldName(null);
	string sId = ViewData.TemplateInfo.GetFullHtmlFieldId(null);
	string sValue = (Model != null) ? Com.Dover.Web.Models.DataTypes.Password.BogusText : String.Empty;
%>

<input type="password" name="<%= sName %>" id="<%= sId %>" value="<%= sValue %>" />