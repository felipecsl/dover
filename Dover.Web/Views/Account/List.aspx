<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<List<KeyValuePair<string, string>>>" %>
<%@ Import Namespace="Microsoft.Web.Mvc" %>
<%@ Import Namespace="Br.Com.Quavio.Tools.Web.Mvc" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Dover - Listar usuários
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h1>Listar usuários</h1>

    <table class="admin-grid admin-list-grid">
		<thead> 
			<tr class="grid-header">
				<th colspan="2">Ações</th>
				<th>Conta</th>
				<th>Nome do usuário</th>
			</tr>
		</thead>
        <tbody>
		<%  foreach (var item in Model) { %>
				<tr class="grid-row">
					<td class="grid-controls control-edit">
						<%= Html.ActionImageLink("~/Content/Images/Grid/edit.png", new { Controller = "Account", Action = "Edit", Id = item.Value })%>
					</td>
					<td class="grid-controls">
						<% using (Html.BeginForm<Com.Dover.Controllers.AccountController>(c => c.Delete(item.Value))) { %>
							<%= Html.Hidden("ID", item.Value)%>
							<%= Html.SubmitImage("Delete", "~/Content/Images/Grid/delete.png", new { title = "Apagar este usuário", Class = "confirmable", confirmationmsg = "Tem certeza que deseja apagar este usuário?" })%>
						<% } %>
					</td>
					<td>
						<%: item.Key %>
					</td>
					<td>
						<%: item.Value %>
					</td>
				</tr>
		<% } %>
		</tbody>
    </table>
</asp:Content>

