<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>

<span class="withsubmenu">Atendimento Online</span>
<ul>
	<li><%= Html.ActionLink("Iniciar Atendimento", "Start", new {  }, new { target = "_blank" })%></li>
	<li><%= Html.ActionLink("Visualizar o histórico", "History", new {  }) %></li>
</ul>