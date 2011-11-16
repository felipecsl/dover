$(function () {
	$('img.cross').click(function () {
		$(this).parent().remove();
	});

	var sortableList = $("div.image-list-listing");
	var dictImgIndexes = [];

	sortableList.sortable({
    	placeholder: 'stored-img-placeholder',
    	update: function (event, ui) {
    		sortableList.children().each(function (_ndx) {
    			$(':last-child', this).attr("value", _ndx);
    		});
    	}
    })
    .disableSelection();

	if (sortableList.children().length == 0) {
		sortableList.hide();	// hide the container if empty
	}
});