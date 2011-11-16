<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Com.Dover.Web.Models.DataTypes.ImageList>" %>
<%@ Import Namespace="Microsoft.Web.Mvc" %>
<%@ Import Namespace="Br.Com.Quavio.Tools.Web.Mvc" %>

<% 
	string namePrefix = ViewData.TemplateInfo.GetFullHtmlFieldName(null);
	string idPrefix = ViewData.TemplateInfo.GetFullHtmlFieldId(null);
	string buttonGuid = Guid.NewGuid().ToString().Substring(0, 5);
%>

<script type="text/javascript">
	(function () {
		var sCrossImgPath = '<%= Url.Content("~/Content/Images/Grid/cross.png") %>',
		sModelName = '<%= namePrefix %>',
		sModelId = '<%= idPrefix %>',
		counter = 0;

		$(function () {
			function onImageSelected(_images) {
				$.each(_images, function (i, val) {
					if (val != null) {
						$("<div />").append($("<img />", {
							src: val,
							Class: "img-to-add"
						})).append($("<img />", {
							src: sCrossImgPath,
							alt: "Remover esta imagem",
							Class: "cross",
							click: function () { $(this).parent().remove(); }
						})).append($("<input />", {
							type: "hidden",
							name: sModelName + ".index",
							value: counter
						})).append($("<input />", {
							type: "hidden",
							id: sModelId + "[" + counter + "]_ImagePath",
							name: sModelName + "[" + counter++ + "].ImagePath",
							value: val
						})).appendTo("#images-to-add_<%= buttonGuid %>");
					}
				});

				setTimeout(function () {
					// give some time for the dialog to close itself before hiding the iframe
					$("#<%= namePrefix %>_dialogFrame").hide();
				}, 500);
			}

			$(".image-list-add_<%= buttonGuid %>").click(function () {
				$("#<%= namePrefix %>_dialogFrame").show();
				frames["<%= namePrefix %>_dialogFrame"].openDialog(true);

				return false;
			});

			$("<iframe></iframe>", {
				src: '<%= Url.Action("MediaManager", "Home") %>',
				id: "<%= idPrefix %>_dialogFrame",
				allowtransparency: true, // IE only
				name: "<%= namePrefix %>_dialogFrame",
				"class": "modal-dialog-iframe",
				load: function () { frames["<%= namePrefix %>_dialogFrame"].imgSelectedEventListener(onImageSelected); }
			}).appendTo(document.body);
		});
	})();
</script>

<div class="image-list-container">
	<div class="image-list-listing" title="Clique e arraste para ordenar">
	<%  if (Model != null) { 
	        int i = 0;
			foreach (Com.Dover.Web.Models.DataTypes.DbImage img in Model.OrderBy(img => img.SortIndex)) {
	            var absoluteImgPath = img.ImagePath;
                if(absoluteImgPath.StartsWith("~/")) {
                    absoluteImgPath = Url.Content(img.ImagePath);
                }  %>
                <div class="stored-img-container">  
					<% string entryIndex = Guid.NewGuid().ToString(); %>
					<%= Html.Image(absoluteImgPath, new { Class = "stored-image" })%>
	                <%= Html.Image("~/Content/Images/Grid/cross.png", new { Class = "cross" }) %>
					<input type="hidden" name="<%= namePrefix %>.index" value="<%= entryIndex %>" />
					<input type="hidden" name="<%= namePrefix %>[<%= entryIndex %>].ImagePath" value="<%= img.ImagePath %>" />
					<input type="hidden" name="<%= namePrefix %>[<%= entryIndex %>].SortIndex" value="<%= i++ %>" />
	            </div>
	    <%  } 
		} %>
	</div>
	<a href="" class="image-list-add_<%= buttonGuid %>">Adicionar nova imagem...</a>
	<br />
	<div id="images-to-add_<%= buttonGuid %>" class="images-to-add"></div>
</div>