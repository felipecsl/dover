<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<Com.Dover.Web.Models.DynamicModuleViewModel>>" %>
<%@ Import Namespace="Microsoft.Web.Mvc" %>
<%@ Import Namespace="Br.Com.Quavio.Tools.Web.Mvc" %>
<%@ Import Namespace="Com.Dover.Helpers" %>

<asp:Content ID="Content2" ContentPlaceHolderID="TitleContent" runat="server">
	Dover - Listar <%= ViewData["DisplayName"] %>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
<% 
	string moduleDisplayName = ViewData["DisplayName"] as string;
	string moduleName = ViewData["ModuleName"] as string;
%>
<script type="text/javascript">
	$(function () {
		drawBreadCrumb(["Home", '<%= ViewContext.RouteData.Values["accountFriendlyName"] as string %>', "<%= moduleDisplayName %>", "Lista de <%= moduleDisplayName %>"]);
	});

	function sendSortOrder() {
		var arrIds = [];
		$("tr:not(.grid-header)", "table.admin-list-grid").each(function () {
			arrIds.push($(this).attr("rowid"));
		});
		
		$.ajax({
			type: "POST",
			url: '<%= Url.Action("PersistRowOrder") %>',
			data: { ids: arrIds },
			success: function () { displayFlash("Ordenação salva com sucesso!"); },
			dataType: "json"
		});
	}
</script>
<%
	var routeValues = ViewContext.RouteData.Values;
 %>
    <div class="export-area">
        <a href="<%= Html.GetModuleApiUrl(routeValues["account"] as string, routeValues["modulename"] as string, routeValues["moduleid"] as string, routeValues["id"] as string) %>" target="_blank"><%= Html.Image("~/Content/Images/xml.png", "exportar para XML", new { Class = "export-icon" })%></a>
        <a href="<%= Html.GetModuleApiUrl(routeValues["account"] as string, routeValues["modulename"] as string, routeValues["moduleid"] as string, routeValues["id"] as string, "json") %>" target="_blank"><%= Html.Image("~/Content/Images/json.png", "exportar para JSON", new { Class = "export-icon" })%></a>
        <a href="<%= Html.GetModuleApiUrl(routeValues["account"] as string, routeValues["modulename"] as string, routeValues["moduleid"] as string, routeValues["id"] as string, "csv") %>" target="_blank"><%= Html.Image("~/Content/Images/csv.png", "exportar para CSV", new { Class = "export-icon" })%></a>
    </div>

    <% if (Model == null || Model.Count() == 0) {
		   if (!String.IsNullOrWhiteSpace(moduleDisplayName)) { %>
				<br /><br />
				<h2>Nenhum(a) <%= moduleDisplayName.ToLower().TrimEnd("s".ToCharArray())%> cadastrado(a).</h2>
		<%	}
	   }
	   else { %>
        <table class="admin-grid admin-list-grid sortable draggable">
            <thead> 
                <tr class="grid-header">
                    <% foreach (string s in (IEnumerable<string>)ViewData["Fields"]) { %>
						<th class="grid-header-cell"><%= s%></th>
				    <% } %>
					<th colspan="2">Ações</th>
                </tr>
            </thead>
            <tbody>
        <%  foreach (var row in Model) { %>
            <tr class="grid-row draggable" rowid="<%= row.ID %>">
            <% 	foreach (var field in row.Fields) { %>
	                <td>
						<%= Html.DisplayFor(model => field.Data) %>
	                </td>
            <% 	} %>
				<td class="grid-controls control-edit">
                    <%= Html.ActionImageLink("~/Content/Images/Grid/edit.png", "Dynamic_modules_route", new { action = "Edit", area = "", id = row.ID }, new { title = "Editar" })%>
                </td>
                <td class="grid-controls">
                    <% using (Html.BeginRouteForm("Dynamic_modules_route", new { action = "Delete", area = "" })) { %>
                        <%= Html.Hidden("ID", (int)row.ID)%>
                        <%= Html.SubmitImage("Delete", "~/Content/Images/Grid/delete.png", new { title = "Apagar esta entrada", Class = "confirmable", confirmationmsg = "Tem certeza que deseja apagar esta entrada?" })%>
                    <% } %>
                </td>
            </tr>
        
        <%  }
	   } %>
       </tbody>
    </table>
<%	if (Model != null && Model.Count() > 0) { %>
		<p style="margin-bottom:20px;">Total de <%= Model.Count() %> registros.</p>
<%	} %>
</asp:Content>

