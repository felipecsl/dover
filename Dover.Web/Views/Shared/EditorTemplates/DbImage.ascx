<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Com.Dover.Web.Models.DataTypes.DbImage>" %>
<%@ Import Namespace="Microsoft.Web.Mvc" %>
<%@ Import Namespace="Br.Com.Quavio.Tools.Web.Mvc" %>

<% 
	string namePrefix = ViewData.TemplateInfo.GetFullHtmlFieldName(null);
	string idPrefix = ViewData.TemplateInfo.GetFullHtmlFieldId(null);
	string buttonGuid = Guid.NewGuid().ToString().Substring(0, 5);
%>

<script type="text/javascript">
	$(function () {
	    var imgWrapperElement = null;

		$("#<%= idPrefix %>_ImageDel").click(onClickDelete);

		function onClickDelete() {
		    imgWrapperElement = $(this).parent().remove();
		}

		function onImageSelected(_imgPaths) {
			if(_imgPaths != null && _imgPaths.length > 0) {
				var imgPath = _imgPaths[0];

				var wrapper = $("#<%= idPrefix %>_Wrapper");
				
				if(wrapper.length > 0) {
					wrapper.show();
					$("#<%= idPrefix %>_ImageThumb", wrapper).attr("src", imgPath);
					$("#<%= idPrefix %>_ImagePath", wrapper).val(imgPath);
				}
				else if(imgWrapperElement !== null) {
					imgWrapperElement.appendTo($("#<%= idPrefix %>_container"));
					$("#<%= idPrefix %>_ImageDel").click(onClickDelete);	// re-attach event listener
					$("#<%= idPrefix %>_ImageThumb", imgWrapperElement).attr("src", imgPath);
					$("#<%= idPrefix %>_ImagePath", imgWrapperElement).val(imgPath);
				}
			}

			setTimeout(function() {
				// give some time for the dialog to close itself before hiding the iframe
				$("#<%= namePrefix %>_dialogFrame").hide();
			}, 500);
		};

		$(".btn-send-image_<%= buttonGuid %>").click(function () { 
			$("#<%= namePrefix %>_dialogFrame").attr("style", "display: block");
			frames["<%= namePrefix %>_dialogFrame"].openDialog();

			return false;
		});
		
		$("<iframe></iframe>", {
			src: '<%= Url.Action("MediaManager", "Home") %>',
			id: "<%= idPrefix %>_dialogFrame",
			allowtransparency: true,	// IE only
			name: "<%= namePrefix %>_dialogFrame",
			"class": "modal-dialog-iframe",
			load: function() { frames["<%= namePrefix %>_dialogFrame"].imgSelectedEventListener(onImageSelected); }
		}).appendTo(document.body);

	<% 	if(Model != null && !String.IsNullOrWhiteSpace(Model.AbsolutePath)) {  %>
            $("#<%= idPrefix %>_ImageThumb").attr("src", '<%= Model.AbsolutePath %>');
			$("#<%= idPrefix %>_ImagePath").val('<%= Model.ImagePath %>');
			$("#<%= idPrefix %>_Wrapper").show();
	<% 	} %>
	});
</script>

<div class="dbimage-container" id="<%= idPrefix %>_container">
    <a href="#" class="btn-send-image_<%= buttonGuid %>">Enviar imagem...</a>
    <br />
	<div class="dbimage-wrapper" id="<%= idPrefix %>_Wrapper">
    	<img class="dbimage-thumb" id="<%= idPrefix %>_ImageThumb" alt="Foto em miniatura" />
    	<%= Html.Image("~/Content/Images/Grid/cross.png", new { Class = "cross", id = idPrefix + "_ImageDel" })%>
    	<input type="hidden" name="<%= namePrefix %>.ImagePath" id="<%= idPrefix %>_ImagePath" />
	</div>
</div>