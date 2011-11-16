<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Com.Dover.Modules.IModule>" %>

<%
if (Model.ModuleType == (int)Com.Dover.Modules.ModuleType.Dynamic) { %>
	<li class="menuitem">
		<span class="withsubmenu"><%= Model.DisplayName%></span>
		<ul>
			<li><a href="<%= Url.RouteUrl("Dynamic_modules_route", new { moduleid = Model.Id, modulename = Model.ModuleName, area = "", action = "List" })%>">Listar <%= Model.DisplayName.ToLower()%></a></li>
			<li><a href="<%= Url.RouteUrl("Dynamic_modules_route", new { moduleid = Model.Id, modulename = Model.ModuleName, area = "", action = "Create" })%>">Inserir novo(a) <%= Model.DisplayName.ToLower().TrimEnd("s".ToCharArray())%></a></li>
		</ul>
	</li>
<%	}
else if (Model.ModuleType == (int)Com.Dover.Modules.ModuleType.SingleEntry) { %>
	<li class="menuitem">
		<span><a href="<%= Url.RouteUrl("Dynamic_modules_route", new { moduleid = Model.Id, modulename = Model.ModuleName, area = "", action = "Edit" })%>"><%= Model.DisplayName %></a></span>
	</li>
<%	}
else { %>
	<li class="menuitem">
		<% Html.RenderAction("MenuItem", new { controller = "CloudTalkModule", area = "CloudTalkModule" }); %>
	</li>
<% } %>