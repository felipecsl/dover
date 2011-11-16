<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Com.Dover.Web.Models.AccountViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Dover - Criar nova conta
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Criar nova conta</h2>
	<div class="create-account">
		<form enctype="multipart/form-data" action="" method="post">
			<%: Html.ValidationSummary("Ocorreu uma falha na criação da sua conta. Por favor, corrija os erros e tente novamente.")%>
		
			<h3>Dados da conta</h3>
        
			<div class="editor-label">
				<%: Html.LabelFor(model => model.Name)%>
			</div>
			<div class="editor-field">
				<%: Html.TextBoxFor(model => model.Name)%>
				<%: Html.ValidationMessageFor(model => model.Name)%>
			</div>
            
			<div class="editor-label">
				<%: Html.LabelFor(model => model.Subdomain)%>
			</div>
			<div class="editor-field">
				<%: Html.TextBoxFor(model => model.Subdomain)%>
				<%: Html.ValidationMessageFor(model => model.Subdomain)%>
			</div>
            
			<div class="editor-label">
				<%: Html.LabelFor(model => model.Logo)%>
			</div>
			<div class="editor-field">
				<input type="file" name="Logo" />
				<%: Html.ValidationMessageFor(model => model.Logo)%>
			</div>
		
			<h3>Dados do usuário</h3>

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
				<%= Html.TextBox("email", null, new { autocomplete = "false" })%>
				<%= Html.ValidationMessage("Email")%>
			</div>
			<div class="editor-label">
				<label for="password">Senha:</label>
			</div>
			<div class="editor-field">
				<%= Html.Password("password", null, new { autocomplete = "false" })%>
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
				<input type="submit" value="Salvar" />
			</div>
		</form>
	</div>
</asp:Content>
