<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Com.Dover.Web.Models.DataTypes.HtmlText>" %>

<%
	string sPropName = ViewData.TemplateInfo.GetFullHtmlFieldName(null);
	string sPropId = ViewData.TemplateInfo.GetFullHtmlFieldId(null);

	string sanitizedModel = "";
	
	if (Model != null &&
		!String.IsNullOrWhiteSpace(Model.Text)) {

		sanitizedModel = Model.Text
			.Replace("\r", String.Empty)
			.Replace("\n", String.Empty)
			.Replace("\t", String.Empty)
			.Replace("'", "\"");
	}
%>

<script type="text/javascript">
    $(function() {
    	CKEDITOR.config.htmlEncodeOutput = true;        
		
		var editor = CKEDITOR.replace("<%= sPropId %>", { 
			language: "pt-br",
 			filebrowserUploadUrl: "/ckfinder/core/connector/aspx/connector.aspx?command=QuickUpload&type=Files&currentFolder=/<%= Membership.GetUser().UserName %>/",
 			filebrowserImageUploadUrl : "/ckfinder/core/connector/aspx/connector.aspx?command=QuickUpload&type=Images&currentFolder=/<%= Membership.GetUser().UserName %>/"
		});

 		editor.setData('<%= sanitizedModel %>');

        CKFinder.SetupCKEditor(editor, "/ckfinder/");
    });
</script>
<textarea cols="80" id="<%= sPropId %>" name="<%= sPropName %>" rows="10"></textarea>