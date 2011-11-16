<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<DateTime?>" %>
<%
	string sName = ViewData.TemplateInfo.GetFullHtmlFieldName(null);
	string sId = ViewData.TemplateInfo.GetFullHtmlFieldId(null);
%>

<script type="text/javascript">
    $(function() {
    	var txtId = '#<%= sId %>';
        $.datepicker.setDefaults($.extend({ showMonthAfterYear: false }, $.datepicker.regional['pt-BR']));
        $(txtId).datepicker();
    });
</script>
<% 
    System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("pt-BR");
    
    if (Model != null)
    {
        DateTime val = (DateTime)Model;
        %>
        
        <input type="text" id="<%= sId %>" name="<%= sName %>" value="<%= val.ToString("d") %>" />
<%  }
    else
    { %>
        <input type="text" id="<%= sId %>" name="<%= sName %>" />
<%  } %>
