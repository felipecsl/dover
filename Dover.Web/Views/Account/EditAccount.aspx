<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Com.Dover.Web.Models.AccountViewModel>" %>
<%@ Import Namespace="Microsoft.Web.Mvc" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Dover - Editar conta
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <script>
    	$(function () {
    		drawBreadCrumb(["Home", "Editar Conta"]);
    	});
	</script>

    <form enctype="multipart/form-data" action="" method="post">
        <%: Html.ValidationSummary(true) %>
        
		<input id="Id" name="Id" type="hidden" value="<%= Model.Id %>">

        <div class="editor-label">
            <%: Html.LabelFor(model => model.Name)%>
        </div>
        <div class="editor-field">
            <%: Html.TextBoxFor(model => model.Name) %>
            <%: Html.ValidationMessageFor(model => model.Name) %>
        </div>
            
        <div class="editor-label">
            <%: Html.LabelFor(model => model.Subdomain)%>
        </div>
        <div class="editor-field">
            <%: Html.TextBoxFor(model => model.Subdomain) %>
            <%: Html.ValidationMessageFor(model => model.Subdomain) %>
        </div>
            
        <div class="editor-label">
            <%: Html.LabelFor(model => model.Logo)%>
        </div>
        <div class="editor-field">
			<% if(!String.IsNullOrWhiteSpace(Model.Logo))  %>
				<%= Html.Image(Model.Logo, new { Class = "accountlogo" })%>
            <input type="file" name="Logo" />
            <%: Html.ValidationMessageFor(model => model.Logo)%>
        </div>
		<br />
		<p>Usuários desta conta:</p> 
		<ul class="account-users">
	<%	foreach(var usr in Model.Users) { %>
			<li><%= Html.ActionLink(usr, "Edit", "Account", new { id = usr }, null)%></li>
	<%	} %>
		</ul>
		<%= Html.ActionLink("Novo usuário...", "Register", new { controller = "Account" }) %>
            
		<div class="submit-area">
			<input type="submit" value="Salvar" />
		</div>
    </form>

</asp:Content>