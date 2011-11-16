<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Dictionary<int, string>>" %>
<%@ Import Namespace="Microsoft.Web.Mvc" %>
<%@ Import Namespace="Br.Com.Quavio.Tools.Web.Mvc" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Dover - Listar contas
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h1>Minhas contas</h1>

    <table class="admin-grid admin-list-grid">
		<thead> 
			<tr class="grid-header">
				<th colspan="2">Ações</th>
				<th>Conta</th>
			</tr>
		</thead>
        <tbody>
		<%  foreach (var item in Model) { %>
				<tr class="grid-row">
					<td class="grid-controls control-edit">
						<%= Html.ActionImageLink("~/Content/Images/Grid/edit.png", new { Controller = "Account", Action = "EditAccount", Id = item.Key })%>
					</td>
					<td class="grid-controls">
						<% using (Html.BeginForm<Com.Dover.Controllers.AccountController>(c => c.DeleteAccount(item.Key))) { %>
							<%= Html.Hidden("ID", (int)item.Key) %>
							<%= Html.SubmitImage("Delete", "~/Content/Images/Grid/delete.png", new { title = "Apagar esta conta", Class = "confirmable", confirmationmsg = "Tem certeza que deseja apagar esta conta?\nNota: Todos os usuários, módulos e dados associados a esta conta serão apagados!" })%>
						<% } %>
					</td>
					<td>
						<%: item.Value %>
					</td>
				</tr>
		<% } %>
		</tbody>
    </table>
</asp:Content>