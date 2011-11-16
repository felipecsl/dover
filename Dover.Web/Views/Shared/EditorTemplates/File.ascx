<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Com.Dover.Web.Models.DataTypes.File>" %>
<%@ Import Namespace="Microsoft.Web.Mvc" %>

<% 
	string namePrefix = ViewData.TemplateInfo.GetFullHtmlFieldName(null);
	string idPrefix = ViewData.TemplateInfo.GetFullHtmlFieldId(null);
%>

<script type="text/javascript">
    $(function () {
        $("#<%= idPrefix %>_btnRemove").click(function () {
            $(this).parent().remove();
        });
    });
</script>

<% if (Model != null) { %>
    <br />
    <div>
        <span class="label">Arquivo atual: </span><a href="<%: Model.AbsolutePath %>" target="_blank"><%: Model.FileName%></a>
        <input type="hidden" name="<%= namePrefix %>.FilePath" id="<%= idPrefix %>_FilePath" value="<%= Model.FilePath %>" />
        <%= Html.Image("~/Content/Images/Grid/cross.png", new { alt = "Remover este arquivo", Class = "file-cross", Id = idPrefix + "_btnRemove" })%>
        <br />
    </div>
<% } %>    
<span class="label">Novo arquivo: </span><input type="file" name="<%= namePrefix %>" id="<%= idPrefix %>" size="30" />
