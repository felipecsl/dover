<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="registerTitle" ContentPlaceHolderID="TitleContent" runat="server">
    Dover - Criar novo usuário
</asp:Content>

<asp:Content ID="registerContent" ContentPlaceHolderID="MainContent" runat="server">
    <h1>Criar uma nova conta de usuário</h1>
    <h4>A senha deve ter um mínimo de  <%=Html.Encode(ViewData["PasswordLength"])%> caracteres.</h4>
    <br />
    <%= Html.ValidationSummary("Ocorreu uma falha na criação do seu usuário. Por favor, corrija os erros e tente novamente.") %>

    <% using (Html.BeginForm()) { %>
        <div class="editor-label">
			<label for="userName">Nome do usuário:</label>
		</div>
		<div class="editor-field">
			<%= Html.TextBox("userName")%>
			<%= Html.ValidationMessage("Usuario")%>
		</div>
		<div class="editor-label">
			<label for="email">E-mail:</label>
		</div>
		<div class="editor-field">
			<%= Html.TextBox("email")%>
			<%= Html.ValidationMessage("Email")%>
		</div>
		<div class="editor-label">
			<label for="password">Senha:</label>
		</div>
		<div class="editor-field">
			<%= Html.Password("password")%>
			<%= Html.ValidationMessage("Senha")%>
		</div>
		<div class="editor-label">
			<label for="confirmPassword">Repita a sua senha:</label>
		</div>
		<div class="editor-field">
			<%= Html.Password("confirmPassword")%>
			<%= Html.ValidationMessage("ConfirmarSenha")%>
		</div>
        <div class="submit-area">
            <input type="submit" value="Registrar" />
        </div>
    <% } %>
</asp:Content>
