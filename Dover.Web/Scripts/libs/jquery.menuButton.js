// jQuery Quavio Admin menu button plugin
//
// Version 1.12
//
// Felipe Lima
// 11 Dez 2009
//
//
// Usage:
//		$(elem).menuButton({
//          text: 'My button text',
//          navigateUrl: 'http://wwww.google.com',
//          iconUrl: '../Images/button_icon.png',   // must be 16x16 pixels, path relative to the html document
//          subMenuItems: [
//              { text: 'Sub item 1', navigateUrl: 'http://www.yahoo.com' },
//              { text: 'Sub item 2', navigateUrl: 'http://www.bing.com' },
//              { 
//                  text: 'Sub item 2 > sub item 1', 
//                  subMenuItems: [
//                      { text: 'Listar categorias', navigateUrl: '<%= Url.RouteUrl(new { controller = "ProductsModule", action = "ListCategories" })%>' },
//                      ...
//                      { ... }]
//              },
//              ...
//              { text: 'Sub item N', navigateUrl: 'http://www.flickr.com' } ]
//      });
// 
// History:
//
//      1.12 - Added support to submenu items with subentries and fixed width bug in IE7
//		1.00 - Released (11 December 2009)
//
//
// License:
// 
// Free to use for Quavio projects only.
//

/*
*  Menu button markup:
*  
* <div class="menu-item">
*   <div class="menu-item-left">
*       <div class="menu-item-right">
*           <div class="menu-item-body">
* 	            <div class="menu-item-body-text"><a href="~/" runat="server">{BUTTON_TEXT}</a></div>
*           </div>
*       </div>
*   </div>
* </div>
*
*  Submenu button markup:
*
* <div class="submenu">
*     <ul class="submenu-list">
*         <li>{SUBMENU_ITEM_1}</li>
*         <li>{SUBMENU_ITEM_2}</li>
*           ...
*         <li>{SUBMENU_ITEM_N}</li>
*     </ul>
* </div>
*
*
*/

(function ($) {
	$.fn.menuButton = function (options) {
		var defaults = {
			text: '',
			navigateUrl: '#',
			iconUrl: 'none',
			subMenuItems: []
		};

		var options = $.extend(defaults, options);

		$(document).click(function (evt) {
			// if the user clicks anywhere outside the menu, hide any open submenus
			// and un-press any pressed meu buttons
			var evtTarget = $(evt.target);

			if (evtTarget.closest('div.menu-item').length > 0) {
				return;
			}

			$('div.submenu').hide();
			$('div.menu-item').removeClass('menu-item-pressed');
		});

		return this.each(function () {
			var o = options;
			var obj = $(this);

			obj.addClass('menu-item')
                .append('<div class="menu-item-left"><div class="menu-item-right"><div class="menu-item-body"><div class="menu-item-body-text"></div></div></div></div>')
                .mouseenter(function () {
                	$(this).addClass('menu-item-hover');
                })
                .mouseleave(function () {
                	$(this).removeClass('menu-item-hover');
                });

			var buttonBodyText = $('div.menu-item-body-text', obj);

			if (o.subMenuItems.length > 0) {
				obj.addClass('menu-item-toggle')
                    .click(function () {
                    	// unselect all other submenus  (TODO: Except this one)
                    	$('div.submenu').hide();
                    	$('div.menu-item').removeClass('menu-item-pressed');

                    	var thisElem = $(this);
                    	if (!thisElem.hasClass('menu-item-pressed')) {
                    		$(this).addClass('menu-item-pressed');
                    	}
                    	else {
                    		$(this).removeClass('menu-item-pressed');
                    	}
                    });

				buttonBodyText.text(o.text);
				buttonBodyText.addClass('menu-item-body-dropdown');

				var subMenuElement = buildSubmenu();

				obj.click(function () {
					var isSubmenuHidden = subMenuElement.css('display') == 'none';
					var thisElem = $(this);

					if (isSubmenuHidden) {
						subMenuElement.show();
						subMenuElement.css({
							top: thisElem.offset().top + Utils.pxToInt(thisElem.css('height')),
							left: thisElem.offset().left
						});
					}
					else {
						subMenuElement.hide();
					}
				});

				$('.submenu-list > li', subMenuElement)
                    .click(function (e) {
                    	if ($(':first-child', this).hasClass('subitem-arrow')) {
                    		e.stopPropagation();    // prevent submenu items with a submenu from being hidden on click
                    	}
                    	else {
                    		$(this).parent().parent().hide();
                    	}
                    })
                    .mouseenter(function () {
                    	if ($(':first-child', this).hasClass('subitem-arrow')) {    // does this submenu entry has a submenu?
                    		var subsubWrapper = $('div.submenu', this);

                    		subsubWrapper.css({
                    			top: $(this).position().top,
                    			left: $(this).width()
                    		})
                            .show();
                    	}
                    })
                    .mouseleave(function () {
                    	if ($(':first-child', this).hasClass('subitem-arrow')) {    // does this submenu entry has a submenu?
                    		$('div.submenu', this).hide();
                    	}
                    });
			}
			else {
				buttonBodyText.append('<a href="' + o.navigateUrl + '">' + o.text + '</a>');
			}

			if (o.iconUrl != 'none') {
				var iconWrapper = $('<div class="menu-item-body-icon"></div>');
				iconWrapper.css('background', 'url(' + o.iconUrl + ') no-repeat');
				iconWrapper.css('padding-left', 20);
				$('div.menu-item-body-text', obj).wrap(iconWrapper);
			}
		});

		function buildSubmenu() {
			var o = options;
			var subMenuWrapper = $('<div class="submenu"><ul class="submenu-list"></ul></div>');
			var subMenuList = $('ul.submenu-list', subMenuWrapper);
			var totalItems = o.subMenuItems.length;

			for (var i = 0; i < totalItems; i++) {
				var subItem = o.subMenuItems[i];
				var itemAnchor = $('<a>' + subItem.text + '</a>');

				if (subItem.navigateUrl != undefined) {
					itemAnchor.attr('href', subItem.navigateUrl);
				}

				subMenuList.append(itemAnchor);
				var thisAnchor = itemAnchor.wrap('<li></li>');

				if (subItem.subMenuItems != undefined &&
                    subItem.subMenuItems.length > 0) {
					itemAnchor.addClass("subitem-arrow");

					var subsubWrapper = $('<div class="submenu"><ul class="submenu-list"></ul></div>');
					var subsubMenuList = $('ul.submenu-list', subsubWrapper);
					var totalSubItems = subItem.subMenuItems.length;

					for (var j = 0; j < totalSubItems; j++) {
						var subsubItem = subItem.subMenuItems[j];

						subsubMenuList.append('<li><a href="' + subsubItem.navigateUrl + '">' + subsubItem.text + '</a></li>');
					}

					thisAnchor.parent().append(subsubWrapper);
				}
			}

			$('body').append(subMenuWrapper);

			return subMenuWrapper;
		}
	};
})(jQuery);

jQuery.log = function (message) {
	if (window.console) {
		console.debug(message);
	}
};