<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>
<%@ Import Namespace="Microsoft.Web.Mvc" %>
<%@ Import Namespace="Br.Com.Quavio.Tools.Web.Mvc" %>

<div class="fl" id="menu-wrapper">
<% 
var repo = new Com.Dover.Modules.ModuleRepository();
var moduleList = repo.GetAccountModules();

if (ViewContext.RouteData.Values["account"] != null) {
	if (moduleList.Count() > 0) {
	 %>
	<%	if (moduleList != null) { %>
			<ul class="menu">
		<%	foreach (var m in moduleList) { %>
				<% Html.RenderPartial("MenuItem", m); %>
		<%	} %>
			</ul>			
	<%	}
		else { %>
			<script type="text/javascript"> 
				$(function () {
					displayFlash("Falha ao obter a lista de módulos");
				});
			</script>
		<%	} %>
<%	}

}
else { %>
		<style>
			.pagecontent { margin-left: 0; }
		</style>
	<% 
} %>
</div>