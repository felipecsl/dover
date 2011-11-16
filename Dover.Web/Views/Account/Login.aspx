<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>
<%@ Import Namespace="Microsoft.Web.Mvc" %>
<%@ Import Namespace="Br.Com.Quavio.Tools.Web.Mvc" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Dover - Login</title>
    <% Html.RenderPartial("MasterHeadIncludes"); %>
	<style type="text/css">
		body { background-color: #E5E5E5; text-align: center; }
	</style>
</head>
<body>
<%	
	string baseUrl = Com.Dover.DoverApplication.Scheme + Com.Dover.DoverApplication.DomainName;
	string openIdTokenUrl = baseUrl + "/" + "OpenId"; 
%>
    <a href="<%= baseUrl %>"><%= Html.Image("~/Content/Images/logo.png", new { Class = "logosmall" })%></a>
<% if(ViewData["accountlogo"] != null) %>
	<%= Html.Image(ViewData["accountlogo"].ToString(), new { Class = "accountlogobig" })%>

	<div class="loginpanel">
		<div id="rpxframe">
			<iframe src="https://dover.rpxnow.com/openid/embed?token_url=<%= openIdTokenUrl %>&language_preference=pt-BR" scrolling="no" frameBorder="no" allowtransparency="true" class="rpxframe"></iframe>
			<span id="useregularaccount" class="useother">Ou utilizar uma conta do Dover.</span>
		</div>
		<div id="login-account">
			<% using (Html.BeginForm()) { %>
				<%= Html.ValidationSummary() %>

				<%= Html.TextBox("username")%>
				<%= Html.Password("password")%>
					
				<%= Html.CheckBox("rememberMe")%> <label for="rememberMe">Lembrar-se neste computador?</label><br />
				<%= Html.ActionLink("Esqueceu sua senha?", "ResetPassword")%><br />
				
				<input type="submit" value="Entrar" />
				
				<span id="useopenid" class="useother">Ou utilizar uma conta OpenID.</span>
			<% } %>
		</div>
	</div>
</body>
</html>