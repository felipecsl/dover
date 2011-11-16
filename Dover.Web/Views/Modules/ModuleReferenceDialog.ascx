<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>
<%@ Import Namespace="Microsoft.Web.Mvc" %>

<script type="text/javascript">
	var getModulesUrl = '<%= Url.Action("GetAccountModules", "Modules") %>';
</script>

<div id="module-reference-dialog" title="Selecione o módulo">
	<div id="module-reference-form">
		<div class="editor-label"><label for="email">Nome:</label></div>
		<div class="editor-field">
			<select id="module-reference-dropdown">
			</select>
		</div>
	</div>
	<div id="status-msg"></div>
</div>