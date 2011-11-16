<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Com.Dover.Web.Models.EditUserViewModel>" %>
<%@ Import Namespace="Microsoft.Web.Mvc" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Dover - Editar Usuário
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <%--<script type="text/javascript">
        var sAddModulesUrl = '<%= Url.RouteUrl(new { controller = "Module", action = "AddModules" }) %>';
        var sRemovesModuleUrl = '<%= Url.RouteUrl(new { controller = "Module", action = "RemoveModules" })%>';
        var sUserGuid = '<%= Url.RequestContext.RouteData.Values["id"] %>';

        $(function() {
            $(".module-list li").toggle(
                function() {
                    $(this).addClass("selected");
                },
                function() {
                    $(this).removeClass("selected");
                }
            );

            $("#btnMoveLeft").click(function() {
                var itemsToMove = $(".list-selected li.selected").detach().click();
                $("#ul-available").append(itemsToMove);
                removeModules(itemsToMove);
            });
            $("#btnMoveRight").click(function() {
                var itemsToMove = $(".list-available li.selected").detach().click();
                $("#ul-selected").append(itemsToMove);
                addModules(itemsToMove);
            });
        });

        function persistModules(
            _modules,
            _sUrl) {
            if (_modules.size() == 0) return;
            
            var objModules = {};
            _modules.each(function(index) {
                objModules['item' + index] = $(this).attr("guid");
            });

            objModules['userid'] = sUserGuid;

            $.ajax({
                type: 'POST',
                url: _sUrl,
                data: objModules,
                success: function (data) { if (data.result < 0) { displayFlash(data.msg); } },
                dataType: "json"
            });
        }

        function removeModules(_modules) {
            persistModules(_modules, sRemovesModuleUrl);
        }

        function addModules(_modules) {
            persistModules(_modules, sAddModulesUrl);
        }
    </script>--%>
    <%--<h3>Módulos</h3>
    <fieldset class="module-list list-available">
        <legend>Disponíveis</legend>
        <div class="module-list-wrapper">
            <ul id="ul-available">
                <% foreach (Com.Dover.Modules.Module m in Model.AllModules)
                   {%>
                        <li guid="<%= m.Id %>"><%= m.DisplayName%></li>
                <% } %>
            </ul>
        </div>
    </fieldset>
    <div class="arrow-controls">
        <%= Html.Image("~/Content/Images/right2.gif", new { id = "btnMoveRight" })%>
        <br />
        <%= Html.Image("~/Content/Images/left2.gif", new { id = "btnMoveLeft" })%>
    </div>
    <fieldset class="module-list list-selected">
        <legend>Selecionados</legend>
        <ul id="ul-selected">
            <% foreach (Com.Dover.Modules.Module m in Model.UserModules)
               {%>
                    <li guid="<%= m.Id %>"><%= m.DisplayName %></li>
            <% } %>
        </ul>
    </fieldset>--%>
    
	<script>
		$(function () {
			drawBreadCrumb(["Home", "Editar Usuário"]);
		});
	</script>

    <% Html.RenderPartial("ProfilePartial", new Com.Dover.Web.Models.UserProfileViewModel() { 
		AccountProperties = Model.UserProfile.Properties, 
		UserId = Model.UserId, 
		Email = Model.Email
	}); %>
    
</asp:Content>

