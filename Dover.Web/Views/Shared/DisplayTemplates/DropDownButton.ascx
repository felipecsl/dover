<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Com.Dover.Web.Models.DataTypes.DropdownButton>" %>
<% if(Model != null) { %>
    <%: Model.SelectedValue %>
<% } %>