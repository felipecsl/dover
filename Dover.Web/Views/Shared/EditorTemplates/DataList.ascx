<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Com.Dover.Web.Models.DataTypes.DataList>" %>
<%@ Import Namespace="Microsoft.Web.Mvc" %>

<%
	string sName = ViewData.TemplateInfo.GetFullHtmlFieldName(null);
	string sId = ViewData.TemplateInfo.GetFullHtmlFieldId(null);
%>

<script type="text/javascript">
    $(function () {
        var itemCount = 0;

        $("#<%= sId %>-btnAdd").click(function () {

            var itemValue = $("#<%= sId %>-txtItem").val(),
                rowText = "<tr class='{rowClass}'><td>{imgCross}</td><td>{itemValue}{hiddenIndex}{hiddenValue}</td></tr>",
                deleteImgPath = '<%= Url.Content("~/Content/Images/Grid/cross.png") %>',
                imgCross = "<img title='Apagar' class='btn-delete-field' src='" + deleteImgPath + "'/>",
                hiddenElemIndex = "<input type='hidden' name='<%= sName %>.Items.index' value='" + itemCount + "' />",
                hiddenElemValue = "<input type='hidden' name='<%= sName %>.Items[" + itemCount++ + "]' value='" + itemValue + "' />",
                rowClass = ($("#<%= sId %>-table tr").length % 2 == 0) ? "grid-row" : "grid-row-alternate";

            rowText = rowText.replace(/{rowClass}/g, rowClass);
            rowText = rowText.replace(/{imgCross}/g, imgCross);
            rowText = rowText.replace(/{itemValue}/g, itemValue);
            rowText = rowText.replace(/{hiddenIndex}/g, hiddenElemIndex);
            rowText = rowText.replace(/{hiddenValue}/g, hiddenElemValue);

            $("#<%= sId %>-table").append(rowText);
            $("#<%= sId %>-txtItem").val("");
        });

        $(".btn-delete-field").live("click", function () {
            $(this).parent().parent().remove();
        });
    });
</script>

<input type="text" value="" id="<%= sId %>-txtItem" />
<input type="button" value="Adicionar" id="<%= sId %>-btnAdd" class="datalist-btnadd" /> 
<br />
<table cellspacing="1" border="0" style="border: 1px solid #99bbe8;" class="admin-grid admin-list-grid" id="<%= sId %>-table">
<%  if(Model != null) {
        int i = 0;
        foreach(var item in Model.Items) {
            string gIndex = Guid.NewGuid().ToString(); %>
        <tr class="<%= (i++ % 2 == 0) ? "grid-row" : "grid-row-alternate" %>">
            <td>
                <%= Html.Image("~/Content/Images/Grid/cross.png", new { Class = "btn-delete-field" })%>
            </td>
            <td>
                <%: item %>
                <input type="hidden" name="<%= sName %>.Items.index" value="<%= gIndex %>" />
                <input type="hidden" name="<%= sName %>.Items[<%= gIndex %>]" value="<%= item %>" />
            </td>
        </tr>
    <% }
    } %>
</table>