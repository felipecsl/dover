<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>
<%@ Import Namespace="Microsoft.Web.Mvc" %>
<%@ Import Namespace="Br.Com.Quavio.Tools.Web.Mvc" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Dover - Efetuar pagamento
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div class="payment-area">
		<h1>Escolha o seu plano</h1>
		<div class="plans">
			<div class="plan plan-free">
				<h2>Free</h2>
				<div class="plan-price">
					<div class="value"></div>
				</div>
			</div>
			<div class="plan plan-standard">
				<h2>Standard</h2>
				<div class="plan-price price-standard">
					<div class="value"></div>
				</div>
			</div>
			<div class="plan plan-business">
				<h2>Business</h2>
				<div class="plan-price price-business">
					<div class="value"></div>
				</div>
			</div>
			<div class="plan plan-pro">
				<h2>Enterprise</h2>
				<div class="plan-price price-pro">
					<div class="value"></div>
				</div>
			</div>
		</div>
		<div class="plan-description">
			<ul class="plan-description">
				<li>Features</li>
				<li>1 conta</li>
				<li>2 módulos</li>
			</ul>
			<ul class="plan-description">
				<li>Features</li>
				<li>5 contas</li>
				<li>10 módulos</li>
			</ul>
			<ul class="plan-description">
				<li>Features</li>
				<li>10 contas</li>
				<li>30 módulos</li>
			</ul>
			<ul class="plan-description">
				<li>Features</li>
				<li>Contas ilimitadas</li>
				<li>Módulos ilimitados</li>
			</ul>
		</div>
		<p>Aceitamos os principais cartões de crédito</p>
		<%= Html.Image("~/Content/Images/Payment/american-express-curved-32px.png")%>
		<%= Html.Image("~/Content/Images/Payment/mastercard-curved-32px.png")%>
		<%= Html.Image("~/Content/Images/Payment/visa-curved-32px.png")%>
		<%= Html.Image("~/Content/Images/Payment/cirrus-curved-32px.png")%>
		<p>Pagamento seguro via PayPal</p>
		<a href="http://www.paypal.com" target="_blank"><%= Html.Image("~/Content/Images/Payment/paypal-curved-32px.png")%></a>
	</div>

	<%--<form action="https://www.sandbox.paypal.com/cgi-bin/webscr" method="post">
		<input type="hidden" name="cmd" value="_xclick" />
		<input type="hidden" name="business" value="sand_1287601055_biz@gmail.com" />

		<input type="hidden" name="item_name" value="Dover Mensalidade Plano Básico" />
		<input type="hidden" name="amount" value="25.00" /> 
		<input type="hidden" name="return" value="http://189.63.175.37:8796/Account/ThankYou" /> 
		<input type="submit" value="Pagar" />
	</form>--%>

</asp:Content>