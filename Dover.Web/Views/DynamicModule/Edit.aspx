<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Com.Dover.Web.Models.DynamicModuleViewModel>" ValidateRequest="False" %>
<%@ Import Namespace="Microsoft.Web.Mvc" %>
<%@ Import Namespace="Br.Com.Quavio.Tools.Web.Mvc" %>
<%@ Import Namespace="Com.Dover.Helpers" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
<% 	if (Model != null) { %>
		Dover - Editar <%= Model.DisplayName%>
<%	} %>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<%
var routeValues = ViewContext.RouteData.Values;
%>
    <div class="export-area">
        <a href="<%= Html.GetModuleApiUrl(routeValues["account"] as string, routeValues["modulename"] as string, routeValues["moduleid"] as string, routeValues["id"] as string) %>" target="_blank"><%= Html.Image("~/Content/Images/xml.png", "exportar para XML", new { Class = "export-icon" })%></a>
        <a href="<%= Html.GetModuleApiUrl(routeValues["account"] as string, routeValues["modulename"] as string, routeValues["moduleid"] as string, routeValues["id"] as string, "json") %>" target="_blank"><%= Html.Image("~/Content/Images/json.png", "exportar para JSON", new { Class = "export-icon" })%></a>
        <a href="<%= Html.GetModuleApiUrl(routeValues["account"] as string, routeValues["modulename"] as string, routeValues["moduleid"] as string, routeValues["id"] as string, "csv") %>" target="_blank"><%= Html.Image("~/Content/Images/csv.png", "exportar para CSV", new { Class = "export-icon" })%></a>
    </div>

	<script>
<% 	if (Model != null) { %>
		$(function () {
			drawBreadCrumb(["Home", '<%= ViewContext.RouteData.Values["accountFriendlyName"] as string %>', "<%= Model.DisplayName %>", "Editar <%= Model.DisplayName %>"]);
		});
<%	} 
	if(!ViewData.ModelState.IsValid) { %>
		$(function () {
			displayFlash("Ocorreu um erro ao salvar a entrada. Por favor, corrija e tente novamente.");
		});
<%	} %>
	</script>

<%	using (Html.BeginRouteForm("Dynamic_modules_route", FormMethod.Post, new { enctype = "multipart/form-data" })) { %>
		
		<a href="#" class="fr orange" id="btnCollapse">Recolher campos</a>
		<div class="clr"></div>
			
		<%= Html.ValidationSummary() %>
		<%= Html.EditorForModel()%>
	        
	<%	if (Model != null) { %>
		<div class="submit-area">
	        <input type="submit" value="Salvar" />
	    </div>
	<%	} %>
<%	} %>
</asp:Content>
