<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>
<%@ Import Namespace="Microsoft.Web.Mvc" %>
<%@ Import Namespace="Br.Com.Quavio.Tools.Web.Mvc" %>

<%= Html.Css("~/Content/Site.css?v=" + Com.Dover.DoverApplication.Dover_Build_Minor_Version)%>
<%= Html.Css("~/Content/jquery.Jcrop.css")%>
<%= Html.FavIcon("~/Content/Images/favicon.png")%>

<% 	
if (Request.Url.Host.Contains("dovercms")) { %>
	<link href="https://ajax.googleapis.com/ajax/libs/jqueryui/1.8.5/themes/redmond/jquery-ui.css" rel="stylesheet" type="text/css" />
	<script src="https://ajax.googleapis.com/ajax/libs/jquery/1.4.4/jquery.min.js" type="text/javascript"></script>
	<script src="https://ajax.googleapis.com/ajax/libs/jqueryui/1.8.5/jquery-ui.min.js" type="text/javascript"></script>
	<%= Html.Script("~/ckeditor/lang/_languages.js")%>
	<%= Html.Script("~/ckeditor/ckeditor.js")%>
	<%= Html.Script("~/Scripts/dover-compiled.js?v=" + Com.Dover.DoverApplication.Dover_Build_Minor_Version)%>
	
	<script type="text/javascript">
		// GOOGLE ANALYTICS START
		var _gaq = _gaq || [];
		_gaq.push(['_setAccount', 'UA-17395658-1']);
		_gaq.push(['_trackPageview']);

		(function () {
			var ga = document.createElement('script'); ga.type = 'text/javascript'; ga.async = true;
			ga.src = ('https:' == document.location.protocol ? 'https://ssl' : 'http://www') + '.google-analytics.com/ga.js';
			var s = document.getElementsByTagName('script')[0]; s.parentNode.insertBefore(ga, s);
		})();
		// GOOGLE ANALYTICS END
		// BROWSER UPDATE START
		var $buoop = {};
		$buoop.ol = window.onload;
		$buoop.vs = { i: 8, f: 2, o: 9.63, s: 2, n: 10 };
		window.onload = function () {
			if ($buoop.ol) $buoop.ol();
			var e = document.createElement("script");
			e.setAttribute("type", "text/javascript");
			e.setAttribute("src", '<%= Url.Content("~/Scripts/libs/browserupdate.js") %>');
			document.body.appendChild(e);
		}
		// BROWSER UPDATE END
		// USER VOICE START
		var uservoiceOptions = {
            /* required */
        	key: 'dover',
            host: 'dover.uservoice.com',
            forum: '54188',
            showTab: true,
            /* optional */
            alignment: 'right',
            background_color: '#c3daf9',
            text_color: 'black',
            hover_color: '#d9e5f4',
            lang: 'pt_BR'
        };
        function _loadUserVoice() {
            var s = document.createElement('script');
            s.setAttribute('type', 'text/javascript');
            s.setAttribute('src', ("https:" == document.location.protocol ? "https://" : "http://") + "cdn.uservoice.com/javascripts/widgets/tab.js");
            document.getElementsByTagName('head')[0].appendChild(s);
        }
        _loadSuper = window.onload;
        window.onload = (typeof window.onload != 'function') ? _loadUserVoice : function () { _loadSuper(); _loadUserVoice(); };
        // USER VOICE END
	</script>
<%	
}
else { %>
	<link href="http://localhost/cdncache/css/jquery-ui.css" rel="stylesheet" type="text/css" />
	<script src="http://localhost/cdncache/js/jquery.min.js" type="text/javascript"></script>
	<script src="http://localhost/cdncache/js/jquery-ui.min.js" type="text/javascript"></script>

	<%= Html.Script("~/Scripts/dover.js")%>
	<%= Html.Script("~/Scripts/breadcrumb.js")%>
	<%= Html.Script("~/ckeditor/ckeditor.js")%>
	<%= Html.Script("~/ckeditor/lang/_languages.js")%>
	<%= Html.Script("~/ckfinder/ckfinder.js")%>
	<%= Html.Script("~/Scripts/libs/jquery.menuButton.js")%>
	<%= Html.Script("~/Scripts/libs/jquery.ajaxfileupload.js")%>
	<%= Html.Script("~/Scripts/libs/jquery.cookie.js")%>
	<%= Html.Script("~/Scripts/libs/jquery.maskMoney.js")%>
	<%= Html.Script("~/Scripts/libs/jquery.Jcrop.min.js")%>
	<%= Html.Script("~/Scripts/libs/JSLINQ.js")%>
	<%= Html.Script("~/Scripts/libs/json2.min.js")%>
	<%= Html.Script("~/Scripts/libs/jquery.tablesorter.min.js")%>
	<%= Html.Script("~/Scripts/libs/ui.datepicker-pt-BR.js")%>
	<%= Html.Script("~/Scripts/libs/jquery.lazyload.mini.js")%>
	<%= Html.Script("~/Scripts/libs/jquery.tmpl.min.js")%>
	<%= Html.Script("~/Scripts/libs/jquery.tablednd_0_5.js")%>
	<%= Html.Script("~/Scripts/libs/jquery.flot.min.js")%>
	
	<%= Html.Script("~/Scripts/libs/swfupload/swfupload.min.js")%>
	<%= Html.Script("~/Scripts/libs/swfupload/swfupload.queue.min.js")%>

	<%= Html.Script("~/Scripts/fieldtemplates/imagelist.js")%>
	<%= Html.Script("~/Scripts/utilities/utils.js")%>
	<%= Html.Script("~/Scripts/utilities/date.format.min.js")%>

	<%= Html.Script("~/Scripts/utilities/mediamanager/mediamanager.js")%>
	<%= Html.Script("~/Scripts/utilities/mediamanager/mediamanager.swfupload.js")%>
	<%= Html.Script("~/Scripts/utilities/mediamanager/mediamanager.file.js")%>
	<%= Html.Script("~/Scripts/utilities/mediamanager/mediamanager.filelist.js")%>

	<%= Html.Script("~/Scripts/admin/moduleeditor/moduledetails.js")%>
	<%= Html.Script("~/Scripts/admin/moduleeditor/fieldeditor/fieldmanager.js")%>
	<%= Html.Script("~/Scripts/admin/moduleeditor/fieldeditor/modulereferencedialog.js")%>
	<%= Html.Script("~/Scripts/admin/moduleeditor/fieldeditor/validvaluesdialog.js")%>
<%	
}
if (ViewData["Message"] != null) { %>
	<script type="text/javascript">
		$(function () {
			displayFlash('<%= ViewData["Message"].ToString() %>');
		});
	</script>
<% 	
} %>