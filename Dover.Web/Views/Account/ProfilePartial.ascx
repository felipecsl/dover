<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Com.Dover.Web.Models.UserProfileViewModel>" %>
<% using (Html.BeginForm("Profile", "Account")) { %>
    <h3>Informações do usuário</h3>
    
    <%= Html.Hidden("UserGuid", Model.UserId)%>
	
    <div class="editor-label">
        <label for="Email" class="property-label">Email:</label>
    </div>
    <div class="editor-field">
        <%= Html.TextBoxFor(model => model.Email)%>
        <%= Html.ValidationMessageFor(model => model.Email)%>
    </div>
    <div class="editor-label">
        <label for="Senha" class="property-label">Senha atual:</label>
    </div>
    <div class="editor-field">
        <%= Html.Password("CurrentPassword")%>
        <%= Html.ValidationMessage("CurrentPassword")%>
    </div>
    <div class="editor-label">
        <label for="Senha" class="property-label">Nova senha:</label>
    </div>
    <div class="editor-field">
        <%= Html.Password("NewPassword")%>
        <%= Html.ValidationMessage("NewPassword")%>
    </div>
    <div class="editor-label">
        <label for="PasswordConfirmation" class="property-label">Repita a senha:</label>
    </div>
    <div class="editor-field">
        <%= Html.Password("NewPasswordConfirmation")%>
        <%= Html.ValidationMessage("NewPasswordConfirmation")%>
    </div>
<%	if (Roles.IsUserInRole("sysadmin") || Roles.IsUserInRole("administrators")) { %>
	<div class="editor-label" style="width:200px;">
		<input type="checkbox" name="Administrator" value="Administrator" <%= Roles.IsUserInRole(Membership.GetUser(Model.UserId).UserName, "administrators") ? "checked='checked'" : "" %> />
        <label for="PasswordConfirmation" class="property-label">Administrador</label>
    </div>
<%	} %>
    <br />
    <h3>Dados cadastrais</h3>
	<%  foreach (Com.Dover.Profile.ProfileProperty prop in Model.AccountProperties) { %>
            <div class="editor-label">
                <label for="<%= prop.Key %>" class="property-label"><%= prop.Label%>:</label>
            </div>
            <div class="editor-field">
                <% if (prop.DataType == Com.Dover.Profile.ProfilePropertyDataType.String) { %>
                       <%= Html.TextBox(prop.Key, prop.Value, new { style = "width: 200px;" })%>
                <% }
				   else if (prop.DataType == Com.Dover.Profile.ProfilePropertyDataType.Enumeration) { %>
                        <%= Html.DropDownList(prop.Key, new SelectList((IEnumerable)ViewData[prop.Key + "_Options"], "Value", "Text", prop.Value), new { style = "width: 209px;" })%>
                <% } %>
            </div>
    <% } %>
    <div class="submit-area">
        <input type="submit" value="Salvar" />
    </div>
<% } %>