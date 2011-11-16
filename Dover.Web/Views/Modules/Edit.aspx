<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Com.Dover.Web.Models.ModulesViewModel>" %>
<%@ Import Namespace="Microsoft.Web.Mvc" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Dover - Editar módulo
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<script>
		$(function () {
			drawBreadCrumb(["Home", '<%= ViewContext.RouteData.Values["accountFriendlyName"] as string %>', "Editar módulo <%= Model.DisplayName %>"]);
		});
	</script>

    <% using (Html.BeginForm<Com.Dover.Controllers.ModulesController>( c=> c.Edit(null), FormMethod.Post, new { id = "form-elem" }))
	   { %>
			<%= Html.EditorForModel() %>

			<%= Html.Partial("ListFields", Model) %>

			<div class="submit-area">
		        <input type="submit" value="Salvar" />
		    </div>
	<% 	} %>
	<%= Html.Partial("ManageFields", Model.DataTypeList)%>
</asp:Content>

