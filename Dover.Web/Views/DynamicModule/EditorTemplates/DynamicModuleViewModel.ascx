<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Com.Dover.Web.Models.DynamicModuleViewModel>" %>
<%@ Import Namespace="Microsoft.Web.Mvc" %>

<script>
	$(function () {
		$(".panel-arrow").click(function () {
			var thisElem = $(this);
			$("div.panel-body", thisElem.parent()).toggle();
			thisElem.toggleClass("panel-arrow-collapsed");
		});

		$("div.form-panel h2").click(function () {
			$(this).prev().click();
		});
	});
</script>

<%
if (Model != null) { 
	foreach (var field in Model.Fields) { %>
        <div class="panel form-panel">
			<div class="panel-arrow"></div>
			<h2><%= Html.Label(field.DisplayName)%></h2>
			<div class="panel-body">
				<%= Html.EditorFor(f => field, null, field.PropertyName)%>
				<%= Html.ValidationMessage(field.PropertyName)%>
			</div>
		</div>
<%  }
} %>
