<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Com.Dover.Web.Models.ModulesViewModel>" %>
<%@ Import Namespace="Microsoft.Web.Mvc" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Dover - Criar novo módulo
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<script>
		$(function () {
			drawBreadCrumb(["Home", '<%= ViewContext.RouteData.Values["accountFriendlyName"] as string %>', "Criar novo módulo"]);
		});
	</script>

    <% using (Html.BeginForm<Com.Dover.Controllers.ModulesController>(c => c.Create(), FormMethod.Post, new { id = "form-elem" }))
	   { %>
			<%= Html.EditorForModel()%>

			<div class="editor-label"><%= Html.LabelFor(model => model.Type) %></div>
			<div class="editor-field"><%= Html.RadioButton("Type", 0)%>Lista</div>
			<div class="editor-field"><%= Html.RadioButton("Type", 2)%>Entrada Única</div>
			
			<h3>Campos deste módulo</h3>
			
			<%= Html.Partial("ListFields", Model) %>

			<div class="submit-area">
		        <input type="submit" value="Salvar" id="btn-submit" />
		    </div>
	<% 	} %>
	<%= Html.Partial("ManageFields", Model.DataTypeList) %>
</asp:Content>
