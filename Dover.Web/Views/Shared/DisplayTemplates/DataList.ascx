<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Com.Dover.Web.Models.DataTypes.DataList>" %>

<% if(Model != null) { %>
    <%: Model.ToString() %>
<% } %>