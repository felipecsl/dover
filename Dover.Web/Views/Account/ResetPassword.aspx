<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Dover - Esqueci minha senha
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Reconfigurar senha</h2>

<% using(Html.BeginForm()) { %>
	 <div class="editor-label" style="width: 200px;">
        <label for="Email" class="property-label">Digite seu nome de usuário:</label>
    </div>
    <div class="editor-field">
        <%= Html.TextBox("username") %>
		<%= Html.ValidationMessage("username") %>
    </div>
    <div class="submit-area">
        <input type="submit" value="Enviar nova senha por email" />
    </div>
<% } %>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadArea" runat="server">
</asp:Content>
