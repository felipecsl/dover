<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Com.Dover.Web.Models.DataTypes.CheckBoxList>" %>
<% if(Model != null) { %>
    <%: Model.ToString() %>
<% } %>