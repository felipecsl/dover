<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="indexTitle" ContentPlaceHolderID="TitleContent" runat="server">
    Dover - Home Page
</asp:Content>

<asp:Content ID="indexContent" ContentPlaceHolderID="MainContent" runat="server">
<%	if(ViewData["accountlogo"] != null) { %>
	<%= Html.Image(ViewData["accountlogo"].ToString(), new { Class = "float-right acctlogo" }) %>
<%	}  %>
	
<%	var acctName = ViewContext.RouteData.Values["accountFriendlyName"] as string; %>
	<script>
		$(function () {
	<%	if(!String.IsNullOrWhiteSpace(acctName)) { %>
			drawBreadCrumb(["Home", '<%= ViewContext.RouteData.Values["accountFriendlyName"] as string %>']);
	<%	} else { %>
			drawBreadCrumb(["Home"]);
	<%	} %>
		});
	</script>
	<br /><br />
    <h2>Bem Vindo!</h2>

<%  var currUser = Membership.GetUser(); %>
	<div class="panel" style="width:100%;">
		<h2>Visualizações</h2>
		<div class="panel-body" style="padding-right:130px;">
			<div id="analytics-placeholder"></div> 
			<div id="analytics-details">
				<p>Período: <span id="analytics-period"></span></p>
				<p><span id="total-visits"></span> Visitas</p>
			</div>
		</div>
	</div>
	<div class="panel fl changelog">
		<h2>Histórico de alterações</h2>
		<div class="panel-body">
			<ul>
				<li>18/10: Implementada exportação de dados para CSV</li>
				<li>18/10: Implementado o envio de imagens em lote</li>
				<li>14/09: Criado campo para lista de check boxes</li>
				<li>09/09: Criado gráfico de visitas por conta e por módulo</li>
				<li>06/09: Criada funcionalidade para recorte (crop) de imagens</li>
				<li>28/07: Implementada ordenação de entradas de um módulo</li>
				<li>17/07: Possibilidade de seleção de imagens a partir da aba "Biblioteca de Mídia"</li>
				<li>13/07: Criado painel com Status de utilização do sistema</li>
				<li>28/06: Criada funcionalidade 'Esqueceu sua senha?'</li>
				<li>23/06: Implementação de campo do tipo "Dados de outro módulo", que permite a criação de campos de referência entre módulos diferentes</li>
				<li>01/06: Implementação de campo do tipo "Arquivo", que permite envio arquivos de qualquer tipo</li>
				<li>29/05: Adicionados botões para exportação de módulos em formato XML e JSON</li>
				<li>21/05: Implementada ordenação de linhas a partir de clique no cabeçalho da tabela</li>
				<li>20/05: Implementação de campo do tipo "Lista de ítens"</li>
				<li>19/05: Implementação da aba "Galeria de Mídia" na sobretela de seleção de imagens (disponível para campos do tipo "Imagem" e "Lista de imagens")</li>
			</ul>
		</div>
	</div>
	<div class="fl side-panels">
	<%	if (currUser != null) { %>
		<div class="panel">
			<h2>Status de utilização</h2>
			<div class="panel-body">
				<p>Espaço em disco: <%= String.Format("{0:0.##}", ((Com.Dover.Profile.UACUser)currUser).GetDiskSpaceUsage() / Math.Pow(1024, 2))%> MB</p>
			</div>
		</div>
		<div class="panel">
			<h2>Último acesso</h2>
			<div class="panel-body">
				<p><%= currUser.LastActivityDate.ToString("dd \\de MMMM \\de yyyy", new System.Globalization.CultureInfo("pt-BR")) %>.</p>
			</div>
		</div>
	<%	}
		if (Roles.IsUserInRole("sysadmin")) { %>
		<div class="panel">
			<h2>Usuários online</h2>
			<div class="panel-body">
				<p><%= Membership.GetNumberOfUsersOnline() %> usuário(s) online.</p>
			</div>
		</div>
	<%	} %>
	</div>
	<script>
		$(function () {
			setTimeout(function () {
				$.ajax({
					url: '<%= Url.Action("GetAnalyticsData", "Home") %>',
					method: 'GET',
					dataType: 'json',
					success: onAnalyticsDataReceived
				}, 100);
			});
		});
	</script>
</asp:Content>
