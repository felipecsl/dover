<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Com.Dover.Web.Models.DynamicModuleViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
 <% if (Model != null) { %>
		Dover - Inserir <%= Model.DisplayName %>
<%	} %>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<script>
<% 	if (Model != null) { %>
		$(function () {
			drawBreadCrumb(["Home", '<%= ViewContext.RouteData.Values["accountFriendlyName"] as string%>', "<%= Model.DisplayName%>", "Inserir <%= Model.DisplayName%>"]);
		});
<%	}
	if (!ViewData.ModelState.IsValid) { %>
		$(function () {
			displayFlash("Ocorreu um erro ao inserir a entrada. Por favor, corrija e tente novamente.");
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

