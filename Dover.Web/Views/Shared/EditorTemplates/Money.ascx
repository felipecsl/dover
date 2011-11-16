<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Com.Dover.Web.Models.DataTypes.Money>" %>

<% string fieldId = ViewData.TemplateInfo.GetFullHtmlFieldId(null); %>

<script type="text/javascript">
    $(function () {
        $("#<%= fieldId %>").maskMoney({
            symbol: "R$",
            decimal: ",",
            thousands: ".",
            showSymbol: true
        });
    });
</script>
<input id="<%= fieldId %>" type="text" value="<%= Model %>" name="<%= ViewData.TemplateInfo.GetFullHtmlFieldName(null) %>" />