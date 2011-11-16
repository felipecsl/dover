<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Com.Dover.Web.Models.DataTypes.CheckBoxList>" %>
<%@ Import Namespace="Microsoft.Web.Mvc" %>

<% 	string namePrefix = ViewData.TemplateInfo.GetFullHtmlFieldName(null);
	if(Model != null) {
		foreach(var item in Model.OrderBy(i => i.Key)) { %>
			<div class="checkbox-item">
				<%= Html.CheckBox(item.Key, item.IsChecked, new { Class = "chkitem chkitem-" + namePrefix })%>
				<label for="<%= namePrefix %>.<%= item.Key %>"><%= item.Value %></label>
			</div>

			<% string entryIndex = Guid.NewGuid().ToString(); %>

			<input type="hidden" name="<%= namePrefix %>.index" value="<%= entryIndex %>" />
			<input type="hidden" name="<%= namePrefix %>[<%= entryIndex %>].Key" value="<%= item.Key %>" />
			<input type="hidden" name="<%= namePrefix %>[<%= entryIndex %>].Value" value="<%= item.Value %>" />
			<input type="hidden" name="<%= namePrefix %>[<%= entryIndex %>].IsChecked" value="<%= item.IsChecked %>" />
	<%	}
	} 
%>
<script>
	$(function () {
		$("input.chkitem-<%= namePrefix %>").change(function () {
			var bChecked = $(this).is(":checked");
			$(this).parent().next().next().next().next().val(bChecked);
		});
	});
</script>