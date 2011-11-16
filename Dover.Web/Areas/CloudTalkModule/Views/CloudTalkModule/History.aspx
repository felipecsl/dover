<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<Com.Dover.Areas.CloudTalkModule.Models.CloudTalkHistoryViewModel>>" %>
<%@ Import Namespace="Microsoft.Web.Mvc" %>
<%@ Import Namespace="Br.Com.Quavio.Tools.Web.Mvc" %>
<%@ Import Namespace="Com.Dover.Helpers" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Dover - Histórico de Atendimento
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<script>
		$(function () {
			drawBreadCrumb(["Home", '<%= ViewContext.RouteData.Values["accountFriendlyName"] as string %>', "Atendimento Online", "Histórico de Atendimento"]);
		});
	</script>
	<% if (Model == null || Model.Count() == 0) { %>
		<br /><br />
		<h2>Nenhum registro encontrado.</h2>
	<% }
	else { %>
	<table class="admin-grid admin-list-grid">
			<thead> 
				<tr class="grid-header">
					<th class="grid-header-cell">Nome</th>
					<th class="grid-header-cell" style="width: 140px;">Data/Hora</th>
					<th class="grid-header-cell">Mensagem</th>
				</tr>
			</thead>
			<tbody>
		<%	foreach (var row in Model) { %>
				<tr class="grid-row">
					<td><%: row.SenderEmail%></td>
					<td><%: row.TimeStamp %></td>
					<td><%: row.Message%></td>
				</tr>
		<%	} %>
		</tbody>
	</table>
<%	} %>
</asp:Content>