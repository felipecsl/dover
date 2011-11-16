<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Dover - Ajuda
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<script>
		$(function () {
			drawBreadCrumb(["Home", "Ajuda"]);
		});
	</script>
    <br /><br />
	<h1>Em breve...</h1>
</asp:Content>