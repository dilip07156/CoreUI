/* Initialisers
-------------------------------*/
$(function () {

	/* Global vars */
	/* media queries */
	mq_red = '(min-width: 1300px)';
	mq_orange = '(min-width: 992px) and (max-width: 1299px)';
	mq_purple = '(max-width: 991px)';
	mq_yellow = '(min-width: 768px) and (max-width: 991px)';
	mq_green = '(max-width: 767px)';
	mq_brown = '(max-width: 599px)';
	mq_blue = '(max-width: 479px)';
	/* animation speed */
	anSp = 500;
	anSpFast = 400;

	ww = document.body.clientWidth;

	/* navigation */
	isMobileNav = false;
	mobileNav = '';

	/* forms */
	//forms = $('form');
	header = $('header');
	homeTh = $('.th-img-container');
	bannerImgFit = $('.banner-img-fit');
	horizontalTab = $('#horizontalTab');
	//HeroBanner =$('.f');
	//BookingSelTop = $('.booking-sel-box');
	//AccordionBox = $('.accordion-style');
	ImgFit = $('.img-fit');
	SingleItemCarousel = $('.single-item-carousel');
	TwoItemCarousel = $('.two-item-carousel');
	ThreeItemCarousel = $('.three-item-carousel');
	FourItemCarousel = $('.four-item-carousel');
	monthCarousel = $('.month-carousel');
	CustomField = $('.custom-field');
	UiInputAppendDate = $('.input-append.date').find('input');
	/* AutoComplete =$('.AutoCompleteTags');*/
	AutoComplete = $('#holidayTags,.holidayTags');
	AutoCompleteSelected = $('#holidayTagsSelected,.holidayTagsSelected');
	//ScrollPanel =$('.scrollable-panel');
	UiCustomizeCalender2 = $('#datepicker2');
	UiCustomCalender = $('#custom_datepicker');

	ChekboxRadioGroup = $('.chekbox-radio-group');

	OwlCarousels = $('.owl-carousel');
	SubItinerary = $('.itenary-details');
	accordinCont = $('.accordion-style');
	//FloatingHeights = $('.floating-btns');
	UiCustomizeCalenderMultiCity = $('.datepickerMulticity');
	UiCustomTwoMonthsCalender = $('.CustomTwoMonthsCalendar');
	FixedPkgBar = $('.top-mid-sec');
	FilterSection = $('.fliter-sec');
	ListingDetailContainer = $('.list > .detail-container');
	//PhotoPanel = $('.photos-panel');
	RequiredCheck = $('.required-chk');
	ListingSelectOption = $('.list .select-option');
	AllCheck = $('.all-check');


	checkMQs();
	checkFeatures();

	/**********/

	if (header.length) { initHeader(); }
	if (homeTh.length) { inithomeTh(); }
	if (ImgFit.length) { initImgFit(); }
	if (SingleItemCarousel.length) { initSingleItemCarousel(); }
	if (TwoItemCarousel.length) { initTwoItemCarousel(); }
	if (ThreeItemCarousel.length) { initThreeItemCarousel(); }
	if (FourItemCarousel.length) { initFourItemCarousel(); }
	if (monthCarousel.length) { initmonthCarousel(); }
	if (CustomField.length) { initCustomField(); }
	if (bannerImgFit.length) { initbannerImgFit(); }
	//if(PhotoPanel.length){initPhotoPanel();} 
	if (UiInputAppendDate.length) { initUiInputAppendDate(); }
	if (AutoComplete.length) { initAutoComplete(); }
	if (AutoCompleteSelected.length) { initAutoCompleteSelected(); }
	//if(ScrollPanel.length){initScrollPanel();}
	if (UiCustomizeCalender2.length) { initUiCustomizeCalender2(); }
	if (UiCustomCalender.length) { initUiCustomCalender(); }
	if (OwlCarousels.length) { initOwlCarousels(); }
	if (accordinCont.length) { initAccordinCont(); }
	//if(FloatingHeights.length){initFloatingHeights();}
	if (UiCustomizeCalenderMultiCity.length) { initUiCustomizeCalenderMultiCity(); }
	if (UiCustomTwoMonthsCalender.length) { initUiCustomTwoMonthsCalender(); }
	if (FixedPkgBar.length) { initFixedPkgBar(); }
	if (SubItinerary.length) { initSubItinerary(); }
	if (FilterSection.length) { initFilterSection(); }
	if (ListingDetailContainer.length) { initListingDetailContainer(); }
	if (AllCheck.length) { initAllCheck(); }
	if (RequiredCheck.length) { initRequiredCheck(); }
	if (ListingSelectOption.length) { initListingSelectOption(); }

	adjustMenu();
	adjustMenuHov();
	adjustFooter();
	timePickerInitialize();
	selectInitialize();
	//$('.time24').inputmask('hh:mm', { placeholder: '__:__ _m', alias: 'time24', hourFormat: '24' });
});

$(window).resize(function () {
	waitForFinalEvent(function () {
		sizeOrientationChange();
	}, 100, 'main resize');
	adjustMenu();
	adjustMenuHov();
	adjustFooter();

	initHeader();
});

if (!window.addEventListener) {
	window.attachEvent('orientationchange', sizeOrientationChange);
} else {
	window.addEventListener('orientationchange', sizeOrientationChange, false);
}

/*** prepare menu **/
$(".mobile-nav").append("<nav><ul class='mob-menu nav' id='nav-menu'></ul></nav>");
$('#nav_menu > li').clone().appendTo(".mob-menu");
$('#nav_menu').addClass("hidden-xs");

$(".nav > li > a, .mob-menu > li > a").each(function () {
	if ($(this).next().length > 0) {
		$(this).addClass("parent");
	}
});

$("html.touch .nav > li > a.parent,.mob-menu > li a.parent").attr({ 'href': 'javascript:void();' });
$('.mob-menu > li a.parent').append('<b class="caret"></b>');
$(".mob-menu > li").unbind('mouseenter mouseleave');

/**** Action Pop on Require Checkbox ****/
var initRequiredCheck = function () {

	//    // $('.required-chk').on('ifChecked', function (e) {
	$('.required-chk').on('change', function (e) {
		if ($(this).is(":checked")) {
			var el = $(this),
				el_TrPopSec = el.parents('tr').next('.tr-pop-sec'),
				el_ActionPop = el_TrPopSec.find('.' + el.attr('data-action') + '-popup'),
				el_ServicesTypeCol = el.parents('tr').find('.services-type'),
				el_SqrTabData = el.parents('.sqr-tab-data');

			//Row - Show
			if (el_TrPopSec.css('display') == 'none')
				el_TrPopSec.show();

			//Action Pop - Show
			if (el_ActionPop.css('display') == 'none') {
				el_TrPopSec.find('.action-popup').hide();
				el_ActionPop.show();
			}

			//services Type - Show
			if (el_ServicesTypeCol.find('.' + el.attr('data-action') + '-icon').css('display') == 'none' && el_ServicesTypeCol.length > 0) {
				el_ServicesTypeCol.find('i').hide();
				el_ServicesTypeCol.find('.' + el.attr('data-action') + '-icon').css({ display: 'inline-block' });
			}
			el.parents('tr').children().removeClass('active');
			el.parents('td').addClass('selected active');
			el_SqrTabData.find('.btn-save').removeClass('disabled');
		}
		//}).on('ifUnchecked', function (e) {
		else
			var el = $(this),
				el_TrPopSec = el.parents('tr').next('.tr-pop-sec'),
				el_ActionPop = el_TrPopSec.find('.' + el.attr('data-action') + '-popup'),
				el_ServicesTypeCol = el.parents('tr').find('.services-type'),
				el_SqrTabData = el.parents('.sqr-tab-data');

		//Row - Hide
		if (el_TrPopSec.css('display') == 'block' || el_TrPopSec.css('display') == 'table-row')
			el_TrPopSec.css({ display: 'none' });

		//Action Pop - Hide
		if (el_ActionPop.css('display') == 'block') {
			el_TrPopSec.find('.action-popup').hide();
		}

		el.parents('tr').children().removeClass('active');
		el.parents('td').removeClass('selected');

		//services Type - Hide
		if (el_ServicesTypeCol.length > 0)
			el_ServicesTypeCol.find('i').hide();

		if ($('.required-chk:checked').length == 0)
			el_SqrTabData.find('.btn-save').addClass('disabled');
	});

}

/**** All Check Checkbox ****/
var initAllCheck = function () {
	//old code nt working
	//$('.all-check').on('ifChecked', function (e) {
	//    $('input[name="' + $(this).attr("data-name") + '"]').iCheck('check');
	//}).on('ifUnchecked', function (e) {
	//    $('input[name="' + $(this).attr("data-name") + '"]').iCheck('uncheck');
	//});     
	$('.all-check').on('change', function (e) {
		var dataname = $(this).attr("data-name").replace('_hdr', '');
		if ($(this).is(":checked"))
			$('input[data-name="' + dataname + '"]').prop('checked', true);
		else
			$('input[data-name="' + dataname + '"]').prop('checked', false);

	});
}

/**** init ImgFIt ***/
var initImgFit = function () {
	$(ImgFit).find("figure").find("img").each(function (i, elem) {
		var imgItem = $(elem);
		var div = $("<div />").css({
			background: "url(" + imgItem.attr("src") + ") no-repeat",
			'background-size': "cover",
			'background-position': "center",
			width: "100%",
			height: "100%"
		});

		imgItem.replaceWith(div);
	});
}

//* init Header****//
var initHeader = function () {

	$(".mob-menu > li a.parent").bind('click', function (e) {
		var $thisNav = $(this);
		var thisParentList = $thisNav.parent();
		var thisParent = $thisNav.parent().find('ul');
		thisParent.slideToggle();
		$('.mob-menu > li ul').not(thisParent).slideUp().removeClass('hover');
		$('.mob-menu > li').not(thisParentList).removeClass('hover');
		$(this).parent("li").toggleClass("hover");
	});


}

/**** init hmoe tabs ***/
var inithomeTh = function () {
	$(homeTh).find("img").each(function (i, elem) {
		var img = $(elem);
		var span = $("<div />").css({
			background: "url(" + img.attr("src") + ") no-repeat", 'background-size': "cover",
			width: "100%",
			height: "100%",
			display: "inline-block"
		});
		img.replaceWith(span);
	});
}
/****** initHeroBanner *****/
var initHeroBanner = function () { }

$(window).bind('orientationchange', function () {
	//mainFlexslider.resize();
});

//* adjust menu****//
var adjustMenu = function () {
	var $nav_overlay = $('#nav_overlay');

	if (Modernizr.mq(mq_green)) {
		$(".toggleMenu").css("display", "inline-block");
		if (!$(".toggleMenu").hasClass("active")) {
			$(".mobile-nav").hide();
		} else {
			$(".mobile-nav").show();
		}
		$(".navbar").hide();
	}
	else {
		$(".toggleMenu").css("display", "none");
		$(".navbar").show();
		if (/Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent)) {
			$("#nav_menu  > li > a").bind('touchstart mouseenter');
			$("#nav_menu  > li > a").unbind('click').bind('touchstart mouseenter', function () {
				$("#nav_menu  > li").removeClass('hover');
				$(this).parent().toggleClass('hover');

			});

		}
		else {

			$("#nav_menu > li").removeClass("hover");
			$("#nav_menu > li > a").unbind('click');
			$("#nav_menu > li").unbind('mouseenter mouseleave').bind('mouseenter mouseleave touchstart', function () {
				$(this).toggleClass('hover');

			});
		}
	}
};

$('ul#nav_menu > li:has(ul)').addClass('has-submenu');

var adjustMenuHov = function (e) {

	//alert("hi");
	var $nav_menu = $('#nav_menu');
	var $nav_menu_items = $nav_menu.find('li.has-submenu');
	var $nav_menu_items_link = $nav_menu.find('li.has-submenu > a');
	var $nav_overlay = $('#nav_overlay');

	$nav_menu_items.find('ul').hide();
	//e.preventdefault();
	if (Modernizr.mq(mq_purple)) {
		//$nav_menu_items.find('ul').css('z-index','-1').show();
		$nav_overlay.hide();
		//$('header').removeClass('sec-hove');
	}

	else {

		$nav_menu_items.bind('touchstart mouseenter', function () {

			//alert("hi");
			var $this = $(this);
			$this.addClass('slided selected');
			$this.find('ul').css('z-index', '9999').stop(true, true).slideDown(200, function () {
				$nav_menu_items.not($this).find('ul').hide();
				//$this.removeClass('slided');

				$nav_overlay.stop(true, true).fadeTo(200, 0.3);
				$this.addClass('hovered');
				$('header').addClass('sec-hove');


			});
		}).bind('mouseleave', function () {
			var $this = $(this);
			$this.removeClass('selected').find('ul').css('z-index', '1').slideUp(200, function () {
				$this.removeClass('slided selected');
				$this.removeClass('hovered');
				$nav_overlay.stop(true, true).fadeTo(200, 0, function () {
					$('header').removeClass('sec-hove');
					$nav_overlay.hide();
				});
			});
		})
		$('#nav_menu li.has-submenu ul').css({ "display": "none" });
	}
}

var adjustFooter = function () { }

var ChangeView = function () { }

var initAccordionBox = function (e) { }

/* Functions
-------------------------------*/


/*========= Popup : Starts =========*/
$(document).on('click', '.popup-inline', function (event) {
	var targetDiv = $(this).attr("href");
	event.preventDefault();
	$.magnificPopup.open({
		type: 'inline',
		items: { src: targetDiv },
		fixedContentPos: true,
		fixedBgPos: true,
		overflowY: 'auto',
		closeBtnInside: true,
		preloader: false,
		midClick: true,
		removalDelay: 300,
		mainClass: 'my-mfp-slide-bottom'
	});

});

$(document).on('click', '.error-close-popup', function (event) {
	var errorMsg = $(this).parents('#modelErrorMsg').find('#pErrorMsg').text();
	if (errorMsg === "401 : Session Timeout") {
		var returnURL = window.location.pathname + window.location.search;
		window.location.href = "/Account/Login?returnUrl=" + returnURL;
	}
	$.magnificPopup.close();
});

$(document).on('click', '.close-popup', function (event) {
	// e.preventDefault();
	$.magnificPopup.close();
});


$('.toggle-checkbox').bootstrapSwitch({ state: true });

$('.switch-state').each(function () {
	//$(this).bootstrapSwitch({inverse: true});

	$(this).on('switchChange.bootstrapSwitch', function (event, state) {
		$(this).closest('.view-switch').find('.ttl-view').toggleClass("disabled");
		//$(this).closest('.switch-div').find('.switch').toggleClass('disabled');
	});

	$(this).closest('.view-switch').find('.ttl-view').on('click', function () {
		var type = $(this).data('switch-set')
		$(this).closest('.view-switch').find('.switch-' + type).bootstrapSwitch(type, $(this).data('switch-value'));
	});
});

function ShowMagnificPopup(targetDiv, closeOnBgClick, enableEscapeKey) {
	//opens the popup 
	$.magnificPopup.open({
		type: 'inline',
		items: { src: targetDiv },
		fixedContentPos: true,
		fixedBgPos: true,
		overflowY: 'auto',
		closeBtnInside: true,
		preloader: false,
		midClick: true,
		removalDelay: 500,
		mainClass: 'my-mfp-slide-bottom',
		closeOnBgClick: closeOnBgClick,
		enableEscapeKey: enableEscapeKey
	});
}
/*========= Popup : Ends =========*/



/* Utility
-------------------------------*/
// fired on window orientation or size change
function sizeOrientationChange() {
	checkMQs();
}

// check media query support,
// if supported, set variables
// if not, set 'orange' as default
function checkMQs() {
	// returns 'true' if MQs are supported
	if (Modernizr.mq('only all')) {
		mq_red_check = Modernizr.mq(mq_red);
		mq_orange_check = Modernizr.mq(mq_orange);
		mq_yellow_check = Modernizr.mq(mq_yellow);
		mq_green_check = Modernizr.mq(mq_green);
		mq_blue_check = Modernizr.mq(mq_blue);
		mq_purple_check = Modernizr.mq(mq_purple);
	} else {
		mq_red_check = false;
		mq_orange_check = true;
		mq_yellow_check = false;
		mq_green_check = false;
		mq_blue_check = false;
		mq_purple_check = false;
	}

	// call responsive nav (no init)
	//responsiveNav();
}

//Check device features
function checkFeatures() {
	//touch devices
	touchEnabled = Modernizr.touch;

	if (touchEnabled) {
		//ensures that touch devices are still able to scroll up/down smoothly
		$('html, body').removeClass('no-touch').addClass('touch-mod');
	}

	//placeholder support
	formPlaceholders = Modernizr.input.placeholder;

	//drop shadow support
	boxShadows = Modernizr.boxshadow;

	// IE7 flag (used to disabled tours slider)
	isIE7 = $('html').hasClass('ie7');

	// IE8 flag
	isIE8 = $('html').hasClass('ie8');

	// initialise forms if they exist
	if (forms.length) {
		//initForms();	
	}
}

// waits for final event to avoid multi-firing (ie: using window.resize)
// originally from: http://stackoverflow.com/questions/2854407/javascript-jquery-window-resize-how-to-fire-after-the-resize-is-completed
var waitForFinalEvent = (function () {
	var timers = {};
	return function (callback, ms, uniqueId) {
		if (!uniqueId) {
			uniqueId = "Don't call this twice without a uniqueId";
		}
		if (timers[uniqueId]) {
			clearTimeout(timers[uniqueId]);
		}
		timers[uniqueId] = setTimeout(callback, ms);
	};
})();

function setLocation(url) {
	window.location.href = url
}

/*** EQUAL HEIGHTS ***/
var $window = $(window);
//	

equalheight = function (container) {
	var currentTallest = 0,
		currentRowStart = 0,
		rowDivs = new Array(),
		$elm,
		topPosition = 0;
	$(container).each(function () {

		$elm = $(this);
		$($elm).height('auto')
		topPostion = $elm.position().top;

		if (currentRowStart != topPostion) {
			for (currentDiv = 0; currentDiv < rowDivs.length; currentDiv++) {
				rowDivs[currentDiv].height(currentTallest);
			}
			rowDivs.length = 0; // empty the array
			currentRowStart = topPostion;
			currentTallest = $elm.height();
			rowDivs.push($elm);
		} else {
			rowDivs.push($elm);
			currentTallest = (currentTallest < $elm.height()) ? ($elm.height()) : (currentTallest);
		}
		for (currentDiv = 0; currentDiv < rowDivs.length; currentDiv++) {
			rowDivs[currentDiv].height(currentTallest);
		}
	});
}

$(window).load(function () {
	equalheight('.equal-heights > div');
});

$(window).resize(function () {
	equalheight('.equal-heights > div');
});

/*** ScrollTop ***/
$(".totop").hide();

$(function () {
	$(window).scroll(function () {
		if ($(this).scrollTop() > 300) {
			$('.totop').slideDown();
		}
		else {
			$('.totop').slideUp();
		}
	});

	$('.totop a').click(function (e) {
		e.preventDefault();
		$('body,html').animate({ scrollTop: 0 }, 500);
	});
});


/*** Header Fixed After Scroll ***/
$(window).scroll(function () {
	if ($(this).scrollTop() > 300) {
		$('header').addClass('fixedHeader');
	}
	else {
		$('header').removeClass('fixedHeader');
	}
});
/****/
/*** Flexslider ***/
//if($(".flexslider-class").length){
//$(".flexslider-class").flexslider({ animation: "slide", slideshow: true, animationLoop:true, controlNav:true, pagination:false,directionNav:true, useCSS : false});
//}
/****/
/*** Owl Carosuel ***/
if ($(".owl-class").length) {
	$(".owl-class").owlCarousel({ items: 4, itemsDesktop: [1199, 3], itemsDesktopSmall: [979, 3] });
}
/****/


var initbannerImgFit = function () {
	$(bannerImgFit).find("img").each(function (i, elem) {
		var img = $(elem);
		var imgClass = $(elem).attr("class");
		var imgAlt = $(elem).attr("alt");
		var div = $("<div />").css({
			background: "url(" + img.attr("src") + ") no-repeat",
			'background-size': "cover",
			width: "100%",
			height: "100%"
		}).addClass(imgClass).attr("title", imgAlt);
		img.replaceWith(div);
	});
}

$('.listing-table tr td a.key-info-link').click(function () {

	$(this).toggleClass('open');
	$(this).parent('td').find('.key-info').slideToggle();
	return false;

});

$('.listing-table tr td a.more-info-link').click(function () {

	$(this).toggleClass('open');
	$(this).parent('td').find('.more-info').slideToggle();
	return false;
});


/*
/*=========  Colapsable block starts =========*/
//$(".collapse-link").click(function (e) {
$(document).on("click", ".collapse-link", function (e) {
	e.preventDefault();
	$(this).toggleClass('collapsed ');
	$(this).closest('.collapse-block').children('.collapse-container').slideToggle();
	$(this).find('.link-cont').toggle();
});
/*========= Colapsable block Ends =========*/

/**** Enquiry Popups Starts ****/

$('.check-list').find('input[type=checkbox]').on('ifChecked', function () {
	$("tr").removeClass('enquirySelected');
	$(this).closest("tr").addClass('enquirySelected');
	var value = $('.enquiry-action select#target').val();
	$('.check-list').find('input[type=checkbox]').not(this).iCheck('uncheck');
	enquiryPopups(value);
}).on('ifUnchecked', function () {
	$(this).closest("tr").removeClass('enquirySelected');
	$('.select-option-popup').hide();
});


$('.enquiry-action select#target').change(function () {
	var value = $(this).val();
	$('.enquiry-action').addClass("active");
	enquiryPopups(value);
});

var enquiryPopups = function (value) {
	$('.select-option-popup').hide();
	$('.enquirySelected').next('tr').find('.select-option-popup.' + value).show();

}
//$(document).on("click", ".action-popup .btn-pop-close", function (e) {
$('.action-popup .btn-pop-close').click(function (e) {
	e.preventDefault();

	var el = $(this),
		el_ActionPop = el.parents('.action-popup');

	// el.parents('.tr-pop-sec').prev('tr').find('input[name="' + el_ActionPop.attr('data-sec') + '-type"]').iCheck('uncheck');     

	el.parents('.tr-pop-sec').prev('tr').find('td.active').removeClass('selected');
	el.parents('.tr-pop-sec').prev('tr').find('td.active').find('input[type="checkbox"][data-name = "' + el_ActionPop.attr('data-sec') + '-type"]').prop('checked', false);
	el.parents('.tr-pop-sec').prev('tr').children().removeClass('active');
	el_ActionPop.hide();
	el.parents('.tr-pop-sec').hide();

});
/**** Enquiry Popups Ends ****/


$(document).on("click", ".menu-btn", function (e) {
	//$('.menu-btn').click(function () {
	$('.navigation').slideToggle();
	return false;
});


var initHeader = function () {

	if (Modernizr.mq()) {
		if ($(".inner-banner").length) {
			$(".inner-pg .wrapper").css({ 'padding-top': 0 });
			$("body.booking-flow").css({ 'padding-top': 0 });
		}
	}
	else {
		$(".wrapper").css({ 'padding-top': ($("header").outerHeight()) });
		$("body.booking-flow").css({ 'padding-top': ($("header").outerHeight()) });
	}
}

$(window).scroll(function () {
	5 < $(document).scrollTop() ? $("header").addClass("fixed") : $("header").removeClass("fixed")
});


/**** My Account Module Starts ******/
/*Plugin-secondary navigations*/
/***** active link***/
$(".aside-widget a").each(function () {
	//$(this).addClass('activemenu');
	if (this.href == document.URL) {
		$(this).addClass('active');
		$(this).attr('href', 'javascript:void();');
	}
});

/** Aside Toggle **/
if ($('.aside-widget').length) {
	$('.aside-widget').each(function () {

		var $TargetAside = $(this),
			$BtnAside = $TargetAside.find('.aside-btn'),
			$ContAside = $TargetAside.find('.aside-container'),

			$BtnActiveAside = $ContAside.find('.active').text();


		$BtnAside.find('a').text($BtnActiveAside);
	})
}
else { }
$('.aside-btn a').click(function (event) {
	event.preventDefault();
	$('.left-panel .aside-container').slideToggle();
	$(this).toggleClass("active");
});
/*Plugin-secondary navigations Ends*/


$('.add-row-lnk').click(function () {
	$(this).siblings('.add-another-content').slideDown();
	return false;
});

$('.popup-btn').click(function () {
	$(this).parents('.popup-wrap').find('.popup-theme-01').fadeIn();
	$(this).addClass('active');
	return false;
});

$('.popup-close-btn').click(function () {
	$(this).parents('.popup-wrap').find('.popup-theme-01').fadeOut();
	$(this).parents('.popup-wrap').find('.popup-btn').removeClass('active');
	return false;
});

$('.change-password').click(function () {
	$('.change-password-form').hide();
	$('.popup-thank-you-msg').show();
	return false;
});

$('.add-new-setup').click(function () {
	$(this).attr("disabled", true);
	$('.setup-new-white-label').show();
	return false;
});
$('.add-color-scheme').click(function () {
	//$(this).attr("disabled", true);
	$(this).parents('.form-group-sec').siblings('.color-scheme-table').show();
	$(this).parents('.form-group-sec').parent('li').addClass('no-border');
	$(this).attr("disabled", true);
	return false;
});

$('.edit-table .table td .edit-delete-links .edit').click(function () {
	$(this).parents('.table').addClass('editable-table');
	$(this).parents('.table').find('td').find('.selected-value').hide();
	$(this).parents('.table').find('td').find('.editable-value').show();
	return false;
});

$('.kyc-upload-document .add-more-doc').click(function () {
	$('.kyc-doc-tbl .addition-document-add').show();
	return false;
})

$('.select-white-label').change(function () {
	if ($('.select-white-label').val() == 'default') {
		$(".customised-white-label").hide();
		$(".standard-default-white-label").show();
	}

	if ($('.select-white-label').val() == 'custom') {
		//alert("hie")
		$(".standard-default-white-label").hide();
		$(".customised-white-label").show();
	}
});

/*Gallery View Switcher starts*/
$('.gal-view-switcher').on('click', 'li', function () {
	$('.gal-view-switcher li').removeClass('active');
	$(this).addClass('active');
});

$(".map").click(function () {
	$(".map-panel").show();
	$(".thumb-seven").hide();
});
$(".photos").click(function () {
	$(".map-panel").hide();
	$(".thumb-seven").show();
	if (booleanValue === true) {
		booleanValue = false;
	} else if (booleanValue === false) {
		booleanValue = true;
	}

	owl.data('packagedetails, sync2').reinit({
		singleItem: booleanValue
	});
});
/*Gallery View Switcher ends*/

$('.map-panel > img').click(function () {
	$(this).parent().find('.map-hotel-popup').toggle();
});

/* Owl Carousels Section Starts */
var initOwlCarousels = function () {

	var OwlElement = $('.owl-carousel');
	var owlWrap = $('.photos-panel');
	// checking if the dom element exists
	// check if plugin is loaded
	if (jQuery().owlCarousel) {
		OwlElement.each(function () {
			// PhotoPanel carousel
			if ($(this).parent().hasClass('photos-panel')) {
				var slideCount;
				if ($(this).parent().hasClass('thumb-seven')) { slideCount = 7 }
				else { slideCount = 5 }


				owlWrap.each(function () {
					var sync1 = $(this).find('.photo-gallery'),
						sync2 = $(this).find(".photo-gallery-thumb"),
						status = $(this).find('.owl-status'),
						imgSlider = $(this).find('.owl-carousel'),
						slidesPerPage = slideCount,
						syncedSecondary = true,
						targetOwlItem = status.find('.owlItems'),
						targetCurrentItem = status.find('.currentItem'),
						that = $(this);

					if ($(this).hasClass('owl-singleItem')) {
						$(imgSlider).owlCarousel({
							items: 1,
							slideSpeed: 2000,
							nav: true,
							autoplay: false,
							dots: false,
							loop: false,
							responsiveRefreshRate: 200,
							onInitialized: function (elem) {
								var CurrentCount = elem.item.count - 1;
								$(targetCurrentItem).find(".result").text(CurrentCount);
							}
						}).on('changed.owl.carousel', syncPositionSingle);

					}

					else {
						sync1.owlCarousel({
							items: 1, slideSpeed: 2000, nav: true, autoplay: false,
							dots: false,
							loop: true,
							responsiveRefreshRate: 200,
							onInitialized: function (elem) {
								var CurrentCount = elem.item.count - 1;
								$(targetCurrentItem).find(".result").text(CurrentCount);
							}
						}).on('changed.owl.carousel', syncPosition);
					}

					sync2.on('initialized.owl.carousel', function () {
						sync2.find(".owl-item").eq(0).addClass("current");
					})
						.owlCarousel({
							items: slidesPerPage,
							dots: false,
							nav: false,
							smartSpeed: 200,
							loop: false,
							slideSpeed: 500,
							slideBy: slidesPerPage, //alternatively you can slide by 1, this way the active slide will stick to the first item in the second carousel
							responsiveRefreshRate: 100,
							navText: ['<svg width="100%" height="100%" viewBox="0 0 11 20"><path style="fill:none;stroke-width: 1px;stroke: #000;" d="M9.554,1.001l-8.607,8.607l8.607,8.606"/></svg>', '<svg width="100%" height="100%" viewBox="0 0 11 20" version="1.1"><path style="fill:none;stroke-width: 1px;stroke: #000;" d="M1.054,18.214l8.606,-8.606l-8.606,-8.607"/></svg>']
						}).on('changed.owl.carousel', syncPosition2);;


					function syncPosition(el) {
						//alert('hi');
						//if you set loop to false, you have to restore this next line
						//var current = el.item.index;
						//if you disable loop you have to comment this block
						var count = el.item.count - 1;
						var current = Math.round(el.item.index - (el.item.count / 2) - .5);
						var RemainingCount = (count - current)

						if (RemainingCount >= 0) {
							$(targetCurrentItem).find(".result").text(RemainingCount);
						}
						else {
							//alert("Minus");
							$(targetCurrentItem).find(".result").text(count);
						}

						if (current < 0) {
							current = count;
						}
						if (current > count) {
							current = 0;
						}

						//end block

						sync2
							.find(".owl-item")
							.removeClass("current")
							.eq(current)
							.addClass("current");
						var onscreen = sync2.find('.owl-item.active').length - 1;
						var start = sync2.find('.owl-item.active').first().index();
						var end = sync2.find('.owl-item.active').last().index();

						if (current > end) {
							sync2.data('owl.carousel').to(current, 100, true);
						}
						if (current < start) {
							sync2.data('owl.carousel').to(current - onscreen, 100, true);
						}
					}

					function syncPosition2(el) {
						if (syncedSecondary) {
							var number = el.item.index;
							sync1.data('owl.carousel').to(number, 100, true);
						}
					}

					sync2.on("click", ".owl-item", function (e) {
						e.preventDefault();
						var number = $(this).index();
						sync1.data('owl.carousel').to(number, 300, true);
					});

					function syncPositionSingle(el) {
						//if you set loop to false, you have to restore this next line
						//var current = el.item.index;
						//if you disable loop you have to comment this block
						var count = el.item.count - 1;
						var current = Math.round(el.item.index - (el.item.count / 2) + 1);
						var RemainingCount = (count - current)

						$(targetCurrentItem).find(".result").text(RemainingCount);

						if (current < 0) {
							current = count;
						}
						if (current > count) {
							current = 0;
						}

					}
				})
			}

			// Regular carousel
			else {

                /*if($(this).hasClass('tablist-carousel')){
                    $(this).owlCarousel({autoWidth: true,  slideSpeed : 2000, nav: true,  autoplay: false,dots: false,loop: false,responsiveRefreshRate : 200})
                }*/

				if ($(this).hasClass('tablist-carousel')) {
					$(this).owlCarousel({ autoWidth: true, margin: 5, navText: [,], slideSpeed: 2000, nav: true, autoplay: false, dots: false, loop: false, responsiveRefreshRate: 200 })
				}

				if ($(this).hasClass('testimonials-items')) {
					$(this).owlCarousel({ items: 1, slideSpeed: 2000, nav: false, autoplay: false, dots: true, loop: false, responsiveRefreshRate: 200 })
				}
				else {
					$(this).owlCarousel({ items: 1, slideSpeed: 2000, nav: true, autoplay: false, dots: false, loop: true, responsiveRefreshRate: 200 });
				}



			}


		})
	}
}
/* Owl Carousels Section Ends */

/** required Fields Focus **/
var initCustomField = function () {
	$('.custom-field').each(function () {

		var $this = $(this),
			ReqHolder = $this.find('.holder'),
			thisText = $this.find('input, textarea,.textarea, button').val(),
			thisfield = $this.find("input, textarea,.textarea, button");

		// initialisation
		if (thisText.length) { ReqHolder.fadeOut(); }
		else { ReqHolder.fadeIn(); }

		ReqHolder.click(function () {
			ReqHolder.fadeOut();
			ReqHolder.closest(thisfield).focusin();
		});

		//focus
		thisfield.focus(function () { ReqHolder.fadeOut(); }).focusout(function () {
			if ($this.find('input, textarea, button').val().length) { ReqHolder.fadeOut(); }
			else { ReqHolder.fadeIn(); }
		});



	});

}
/** required Fields Focus **/

/* Page -> Login : Starts */
// Sign IN
$('.btn-sign-in').click(function (e) {
	e.preventDefault();

	var error_count = 0;
	$('.sign-in-sec input[type=text],.sign-in-sec input[type=password]').each(function () {
		if (this.value == "") {
			$(this).parents('.custom-field').addClass('error');
			error_count++;
		} else {
			$(this).parents('.custom-field').removeClass('error');
		}

	});

	if (error_count > 0) {
		$('.sign-in-sec .error-msg').show();
		return false;
	} else {
		$('.sign-in-sec .error-msg').hide();
	}

});

// Text Link -> Show Forgot Passowrd Section
$('.text-forgot-link').click(function (e) {
	e.preventDefault();

	$('.sign-in-sec, .create-ac-link-sec').hide();
	$('.forgot-password-sec').show();

});

// Forgot Passowrd
$('.forgot-password-sec .btn-submit').click(function (e) {
	e.preventDefault();

	var error_count = 0;
	$('.forgot-password-sec input[type=text],.forgot-password-sec input[type=password]').each(function () {
		if (this.value == "") {
			$(this).parents('.custom-field').addClass('error');
			error_count++;
		} else {
			$(this).parents('.custom-field').removeClass('error');
		}

	});

	if (error_count > 0) {
		$('.forgot-password-sec .error-msg').show();
		return false;
	} else {
		$('.forgot-password-sec .error-msg').hide();
		$('.forgot-password-sec').hide();
		$('.message-sec.verification').show();
	}

});

// Reset Passowrd
$('.message-sec.verification .btn-close').click(function (e) {
	e.preventDefault();
	$('.message-sec.verification').hide();
	$('.reset-password-sec').show();
});

// Reset Passowrd
var reset_password_sec = $('.reset-password-sec');
$(reset_password_sec.find('.btn-submit')).click(function (e) {
	e.preventDefault();

	var error_count = 0;
	$('.reset-password-sec input[type=password]').each(function () {
		if (this.value == "") {
			$(this).parents('.custom-field').addClass('error');
			error_count++;
		} else {
			$(this).parents('.custom-field').removeClass('error');
		}

	});

	if (error_count > 0) {
		$(reset_password_sec.find('.error-msg')).show();
		return false;
	} else {
		reset_password_sec.find('.error-msg').hide();
		reset_password_sec.hide();
		$('.message-sec.alert').show();
	}

});
/* Page -> Login : Ends */

$(".radio-list-bar.js-tab  li > *").click(function (e) {
	//e.preventDefault();
	$(".radio-list-bar.js-tab li").removeClass('active');
	$(this).parents('li').addClass("active");
	//var dataLabel = $(this).attr('data-label');
	//$(this).parent('li').parents('.radio-list-bar').siblings('.radio-list-data').hide()
	//$(this).parent('li').parents('.radio-list-bar').siblings(".tab-" + dataLabel + "-sec").show()
});

//======  Tooltip : Starts =======//
$(document).on("click", ".tool-tip > a , .tool-tip > div.info-lnk, .tool-tip > i", function (e) {
	//$(".tool-tip > a , .tool-tip > div.info-lnk, .tool-tip > i").click(function (e) {
	e.preventDefault();
	if ($(this).attr("data-name") != "price" && $(this).attr("data-name") != "foc") {
		$('.tooltip-cont, .share-collapse').hide();
		$(this).parents('li').css({ 'z-index': '2' });
		$(this).parent().find(".tooltip-cont").show();

		var containerPos = $(this).parents('.container').offset().left;
		var containerWdth = $(this).parents('.container').outerWidth();
		var ContPos = $(this).closest('.tool-tip').children('.tooltip-cont').offset().left;
		var ContWdth = $(this).closest('.tool-tip').children('.tooltip-cont').outerWidth();
		var pos = ContPos - containerPos + 30;
		var totWdth = pos + ContWdth;
		var margin = totWdth - containerWdth + 30;
		if (totWdth > containerWdth) { $(this).closest('.tool-tip').children('.tooltip-cont').offset({ left: ContPos - margin }); }
		$(".tool-tip > a, .tool-tip > div.info-lnk, .tool-tip > i, .share-tooltip > a").removeClass("active");
		$(this).addClass("active");
	}
});


$(document).on("click", ".tooltip-cont .btn-close", function (e) {
	//$(".tooltip-cont .btn-close").click(function (e) {
	$(this).parent(".tooltip-cont").hide();
	$(this).parent("form").parent(".tooltip-cont").hide();
	$(".tool-tip > a, .tool-tip > div.info-lnk , .tool-tip > i").removeClass("active");
	e.preventDefault();
});

//$('.tooltip-cont').click(function(e){
//	e.stopPropagation();
//})

$("body").click(function (e) {
	if (e.target.className !== "share-collapse") {
		$(".share-collapse").hide();
		$(".share-tooltip a").removeClass("active");
	}
	//    if(e.target.className !== "tooltip-cont") {		
	//      $(".tooltip-cont").hide(); 
	//      $(".tool-tip a").removeClass("active");
	//    }
	if (!$('.tooltip-cont').is(e.target) && $('.tooltip-cont').has(e.target).length === 0 && $('.tooltip-cont').css('display') === 'block') {
		$(".tooltip-cont").hide();
		$(".tool-tip a").removeClass("active");
	}
});

//======  Tooltip : End =======//

//====== Check availability Start ======//
$('.chk-apply').on('ifUnchecked', function (event) {
	$(this).parents('.chk-apply-cont').find('.mlti-lst').show();
});
$('.chk-apply').on('ifChecked', function (event) {
	$(this).parents('.chk-apply-cont').find('.mlti-lst').hide();
});
//====== Check availability End ======//

//======  Oncheck Popups : Start =======//
$(document).on('ifChecked', 'input[name="pkgd-type"]', function () {
	var el = $(this),
		attr = el.attr('data-pop-name');
	if (typeof attr !== typeof undefined && attr !== false) {
		$.magnificPopup.open({
			items: {
				src: '#' + attr
			},
			type: 'inline',
			fixedContentPos: true,
			fixedBgPos: true,
			overflowY: 'auto',
			closeBtnInside: true,
			preloader: false,
			midClick: true,
			removalDelay: 300,
			mainClass: 'my-mfp-slide-bottom'
		});
	}
});
//======  Oncheck Popups : End =======//	
/***** Itinerary Sub  tabs starts *****/
var initSubItinerary = function () {

	var fixednavbar = $('.itenary-details'),
		DivoOffset = fixednavbar.offset().top - $('header').outerHeight();



	$(".itenary-heading > li a").click(function (event) {
		if ($(".itenary-details .itenary-heading.mobile").length) {
			event.preventDefault();
			$('.itenary-details .itenary-heading li a').removeClass('active');
			$(this).addClass("active");

			var $TargetAside = $('.itenary-details'),
				$BtnAside = $TargetAside.find('.itenary-heading-btn'),
				$ContAside = $TargetAside.find('.itenary-heading'),
				$BtnActiveAside = $ContAside.find('.active').text();
			$BtnActiveAside = $BtnActiveAside;

			$BtnAside.find('a').text($BtnActiveAside);
			var dataLabel = $(this).attr('data-label');
			$('.itenary-section').hide()
			$("#tab-" + dataLabel + "-sect").show()
			//$('.itenary-details .itenary-heading').slideUp();
		}
		else {
			$(this).parents('.itenary-heading').find('li a').removeClass("active");
			$(this).addClass("active");
			var dataLabel = $(this).attr('data-label');
			var itneraryFixPos = $('.itenary-details .itenary-heading.fixedPos');
			if (itneraryFixPos.length)
				$('html, body').animate({ scrollTop: $("#tab-" + dataLabel + "-sect").offset().top - 190 }, 300);
			else
				$('html, body').animate({ scrollTop: $("#tab-" + dataLabel + "-sect").offset().top - 220 }, 300);
		}

	});


	$('.itenary-heading-btn a').click(function (event) {
		event.preventDefault();
		if ($(".itenary-details .itenary-heading.mobile").length) {

			$('.itenary-details .itenary-heading').slideToggle();
			$(this).toggleClass("active");
		}
	});
}
/***** Itinerary Sub  tabs Ends *****/

$(".log-ttl .fa-angle-right").hide();
$(".log-ttl .fa-angle-down").show();
$(".log-ttl").click(function (e) {
	$(".log-table").slideToggle();
	$(this).toggleClass("active");
	$(".log-ttl .fa-angle-down").slideToggle();
	$(".log-ttl .fa-angle-right").slideToggle();
});

//Horizontal Tab
$('#HorizontalTab, #HorizontalTab1').responsiveTabs({
	startCollapsed: 'accordion',
	collapsible: 'accordion'
});
/*plugin-accordin list starts*/
var initAccordinCont = function () {
	$('.accordion-style dt').click(function () {
		$(this).next("dd").slideToggle();
		$(this).toggleClass('active');
	});
}
/*plugin-accordin list ends*/


//=========== Full width containrt - Listing Result Start ===========// 
var initListingDetailContainer = function () {

	$('.list > .details-sec .btn-details').click(function (e) {
		e.preventDefault();
		var el = $(this),
			el_DetailContainer = el.parents('.list').find('.detail-container');

		if (el_DetailContainer.css('display') == 'none') {
			el_DetailContainer.slideDown();
			el.addClass('active');
		}
		else {
			el_DetailContainer.slideUp();
			el.removeClass('active');
		}
	});
}
//=========== Full width containrt -  Listing Result End ===========//
//=========== Listing Result > Select Option Start ===========//
var initListingSelectOption = function () {

	$('input[name="select-trip"]').on('ifChecked', function (e) {

		var el = $(this),
			el_SelectOption = el.parents('.select-option');

		el_SelectOption.find('.styled-select').show();

	}).on('ifUnchecked', function (e) {

		var el = $(this),
			el_SelectOption = el.parents('.select-option'),
			el_DetailsSec = el.parents('.details-sec');

		el_SelectOption.find('.styled-select').hide();
		el_DetailsSec.removeClass('selected yellow-bg');
		el_DetailsSec.removeClass('selected red-bg');

	});

	$('.select-option .form-control').on('change', function () {
		var el = $(this),
			el_DetailsSec = el.parents('.details-sec');

		switch (el.val()) {
			case 'main':
				el_DetailsSec.addClass('selected yellow-bg').removeClass('red-bg');
				$('.sqr-tab-data .btn-continue').removeClass('disabled');
				break;
			case 'alternate':
			case 'additional':
				el_DetailsSec.addClass('selected red-bg').removeClass('yellow-bg');
				$('.sqr-tab-data .btn-continue').removeClass('disabled');
				break;
			default:
				el_DetailsSec.removeClass('selected yellow-bg');
				el_DetailsSec.removeClass('selected red-bg');
				$('.sqr-tab-data .btn-continue').addClass('disabled');
				break;
		}

	});

}
//=========== Listing Result > Select Option End ===========//

var initFilterSection = function () {
	//======= Filter Panel Start =====//	
	$(".fliter-sec > .filter-pannel .filter-inner-list").hide();
	$(".fliter-sec-head .fliter-sec-toggle").click(function () {
		$(this).toggleClass("active");
		$(".fliter-sec > .filter-pannel .filter-inner-list").slideToggle();
		$('.fliter-sec-head .fliter-sec-toggle').text(function (_, text) {
			return text === 'Show all Filters' ? 'Hide all Filters' : 'Show all Filters';
		});
		$(".filter-pannel").toggleClass("active");
	});
	$(".filter-pannel .filter-tl").click(function () {
		$(".filter-inner-list").slideToggle();
		$(".filter-pannel").toggleClass("active");
		$('.fliter-sec-head .fliter-sec-toggle').text(function (_, text) {
			return text === 'Show all Filters' ? 'Hide all Filters' : 'Show all Filters';
		});
	});
	//======= Filter Panel End =====//	
}


$('.progress-bar-cont .steps-progress-bar > li .link ').click(function () {
	$(this).parent('li').find('.progress-bar-dropdown').slideToggle();
});

/*Range Slider :: Starts*/
$(".rangeSlider").slider({ tooltip: 'hide', min: 3000, max: 32000, value: [3000, 32000] });
$(".rangeSlider").on("slide", function (slideEvt) {
	$(".range-1-slider").text(slideEvt.value[0]);
	$(".range-2-slider").text(slideEvt.value[1]);
});
/*Range Slider :: Ends*/


/* Currency :Starts */
$('.sub-menu.currency li a').on('click', function () {
	$('.sub-menu.currency li a').removeClass('fwtb');
	$('.sub-menu.currency li a .curTick').removeClass('fa fa-check');
	$(this).addClass('fwtb');
	$(this).find('.curTick').addClass('fa fa-check');
	var ccode = $(this).attr('data-contryname');
	var currencycode = $(this).attr('data-contrycurrency');
	//var menuhtml="";
	menuhtml = ccode;
	$('.currency-main-menu').html(menuhtml);
});
/* Currency :Ends */

$(function () {
	//====== Add remove functionality Start ======//

	$(".add-remove-js").each(function () {
		var personclone = $(this).find(".clonable-sec"),
			TargetDiv = $(this);

		//** Add Button **/
		$(this).find(".lnk-add").click(function (e) {
			e.preventDefault();
			//alert('HIO');
			var TargetElem = TargetDiv.find('.clonable-sec'),
				num = TargetElem.length,
				CloneElem = TargetElem.not('.clone-sec').clone();

			if (num < 4) {
				//TargetElem.removeClass('ajaxform');
				CloneElem.appendTo(TargetDiv.find('ul')).addClass("clone-sec ajaxform");
				$(this).siblings('.lnk-remove').show();

				//initAjaxForms();

				if (num == 3) {
					$(this).hide();
					$(this).siblings('.lnk-remove').show();
				}
				else {
					$(this).show();
					$(this).siblings('.lnk-remove').show();
				}
			}
			else { }
		});
		//** Remove Button **/
		$(this).find(".lnk-remove").click(function (e) {
			e.preventDefault();

			//alert('BYO');
			var TargetElem = TargetDiv.find('.clonable-sec'),
				TargetElemRemove = TargetDiv.find('.clone-sec:last-child'),
				numRemove = TargetElem.length;

			TargetElemRemove.remove();

			if (numRemove < 4) {
				$(this).siblings('.lnk-add').show();
			}
			else {
				//alert('bye');
			}
		});
	});
	//====== Add remove functionality End ======//

});


/**** Other check box function Start *****/

$('#othr').on('change', function (e) {
	if ($(this).is(":checked")) {
		$('.mlti-lst').show();
		$('.mlti-lst').find("[name=other-type]").prop("checked", false);
		$('.othrs-fld').hide();
		$('.othrs-fld').val('');
	}
	else {
		$('.mlti-lst').hide();
	}
});

$('#othrs-fld').on('change', function (e) {
	if ($(this).is(":checked")) {
		$('.othrs-fld').show();
		$(".mlti-lst").animate({ scrollTop: $(document).height() }, "slow");
	}
	else {
		$('.othrs-fld').hide();
	}
});

//$('#othr').on('ifChecked', function () {
//    $('.mlti-lst').show();
//}).on('ifUnchecked', function () {
//    $('.mlti-lst').hide();
//});

//$('#othrs-fld').on('ifChecked', function () {
//    $('.othrs-fld').show();
//    $(".mlti-lst").animate({ scrollTop: $(document).height() }, "slow");
//}).on('ifUnchecked', function () {
//    $('.othrs-fld').hide();
//});
/**** Other check box function End *****/

/*-- Price And FOC Popup Show/Hide : Start --*/
$(function () {

	//$(document).on('click', '.custom-mob-tbl .tooltip-cont .btn-save', function (e) {
	//    e.preventDefault();
	//    var el = $(this),
	//        elToolTip = el.parents('.tool-tip'),
	//        elToolTipCont = el.parents('.tooltip-cont');
	//    elToolTip.parents('td').addClass('light-yellow-bg');
	//    elToolTip.find('.msg').show();
	//    elToolTip.find('.btn-add').html('Edit');
	//    elToolTipCont.find('.btn-close').trigger('click');
	//});

	$(document).on('click', '.custom-mob-tbl .tooltip-cont .btn-cancel,.tooltip-cont .btn-cancel', function (e) {
		e.preventDefault();
		var el = $(this),
			elToolTipCont = el.parents('.tooltip-cont');
		elToolTipCont.find('.btn-close').trigger('click');
	});

});

function QRFPricesAfterSave(current, result, positionid, productid, positionname) {
	var tourentityaccordian = current.parents("#frmTourEntities");
	if (tourentityaccordian.length > 0) {
		tourentityaccordian.find("#divSuccessError").show();
		if (result == 'success') {
			tourentityaccordian.find("#divSuccessError").addClass("alert alert-success");
			tourentityaccordian.find("#lblmsg").text("Price details saved successfully.");
			tourentityaccordian.find("#stMsg").text("Success!");
		}
		else {
			tourentityaccordian.find("#divSuccessError").addClass("alert alert-danger");
			tourentityaccordian.find("#lblmsg").text("Price details not saved.");
			tourentityaccordian.find("#stMsg").text("Error!");
		}
		if (positionid != "" && positionid != undefined && positionid != null && productid != "" && productid != undefined && productid != null &&
			positionname != "" && positionname != undefined && positionname != null) {
			ShowTourAllowanceToTable(positionname, positionid, productid);
		}
	}
	else if (result == 'success') {
		var elToolTip = current.parents('.tool-tip'),
			elToolTipCont = current.parents('.tooltip-cont');
		elToolTip.parents('td').addClass('light-yellow-bg');
		elToolTip.find('.msgErr').hide();
		elToolTip.find('.msg').show();
		elToolTip.find('.btn-add').html('Edit');
		elToolTipCont.find('.btn-close').trigger('click');
	}
	else {
		var elToolTip = current.parents('.tool-tip'),
			elToolTipCont = current.parents('.tooltip-cont');
		elToolTip.parents('td').addClass('light-yellow-bg');
		elToolTip.find('.msg').hide();
		elToolTip.find('.msgErr').show();
		elToolTip.find('.btn-add').html('Add');
		elToolTipCont.find('.btn-close').trigger('click');
	}
}
/*-- Price And FOC Popup Show/Hide : End --*/


// ======= Child increment Functionlaity Start ========\\

ChildAgeNo = 0;
$('.count-child .count-up').click(function () {
	var ChildNo = $('.txt-child-no').val();
	if (ChildNo >= 2) {
		$(this).addClass('disabled');
		$(this).attr('disabled', 'disabled');
		return false;
	}
	else {
		$('.multi-child').show();
		ChildAgeNo++;
		var divChild = '<div class="col-xs-12 child"><strong>Child ' + ChildAgeNo + ' Age</strong><div class="cust-spinner value"><div class="input-group spinner"><div class="input-group-btn"><button class="btn btn-default count-down" type="button"><i class="fa-custom-minus white"></i></button></div><input type="text" class="form-control frm-pax-control" value="1"><div class="input-group-btn"><button class="btn btn-default count-up" type="button"><i class="fa-custom-plus white"></i></button></div></div></div></div>';
		$('.div-child').append(divChild);
	}
});

$('.count-child .count-down').click(function () {
	var ChildNo = $('.txt-child-no').val();
	$('.count-child .count-up').removeClass('disabled');
	$('.count-child .count-up').removeAttr('disabled');
	if (ChildNo == 0) {
		ChildAgeNo = 0;
	}
	else {
		$('.multi-child').show();
		ChildAgeNo--;
		$('.div-child .child:last-child').remove();
	}
});

// ======= Child increment Functionlaity End ========\\


// ======= Numeric validations Start ========\\

$(document).on('keypress', '.numeric', function (e) {
	// Allow: backspace, delete, tab, escape, enter and . // Allow: Ctrl+A, Command+A // Allow: home, end, left, right, down, up
	if ($.inArray(e.which, [0, 46, 8, 9, 27, 13, 10]) !== -1 || (e.which === 65 && (e.ctrlKey === true || e.metaKey === true))) {
		return;
	}
	if (e.which < 48 || e.which > 57) {
		return false;
	}
});

$(document).on('keypress', '.numericInt', function (e) {
	// Allow: backspace, delete, tab, escape and enter // Allow: Ctrl+A, Command+A // Allow: home, end, left, right, down, up
	if ($.inArray(e.which, [0, 8, 9, 27, 13, 10]) !== -1 || (e.which === 65 && (e.ctrlKey === true || e.metaKey === true))) {
		return;
	}
	if (e.which < 48 || e.which > 57) {
		return false;
	}
});

$(document).on('keypress', '.numericHour', function (e) {
	// Allow: backspace, delete, tab, escape, enter and . // Allow: Ctrl+A, Command+A // Allow: home, end, left, right, down, up
	if ($.inArray(e.which, [0, 46, 8, 9, 27, 13, 10]) !== -1 || (e.which === 65 && (e.ctrlKey === true || e.metaKey === true)))
		return;
	if (e.which < 48 || e.which > 57)
		return false;
	else {
		var selctedVal = this.value.substring(this.selectionStart, this.selectionEnd);
		if (selctedVal != undefined && selctedVal != '')
			$(this).val('');

		var oldVal = $(this).val();
		var finalVal = parseInt(oldVal + e.key);
		if (finalVal < 0 || finalVal > 24)
			return false;
	}
});

//below function allow only numbers and 1 dot
$(document).on('keypress', '.OnlyNumericOneDigit', function (event) {
	$(this).val($(this).val().replace(/[^0-9\.]/g, ''));
	if ((event.which != 46 || $(this).val().indexOf('.') != -1) && (event.which < 48 || event.which > 57) && event.which != 8 && event.which != 0) {
		event.preventDefault();
	}
});

//below function allow numbers and alpha,space,backspace, arrows keys,enter,del
$(document).on('keypress', '.alphanumeric', function (e) {
	var charCode = e.which;
	if (charCode == 8 || charCode == 0) {
		return;
	}
	else {
		var keyChar = String.fromCharCode(charCode);
		return /[a-zA-Z0-9 ]/.test(keyChar);
	}
});


$(document).on('keypress', '.numericWithSpecChar', function (e) {
	// Allow: backspace, delete, tab, escape, enter, parenthesis,plus and hyphen. // Allow: Ctrl+A, Command+A // Allow: home, end, left, right, down, up
	if ($.inArray(e.which, [0, 8, 9, 27, 13, 10, 40, 41, 43, 45]) !== -1 || (e.which === 65 && (e.ctrlKey === true || e.metaKey === true))) {
		return;
	}
	if (e.which < 48 || e.which > 57) {
		return false;
	}
});

// ======= Numeric validations End ========\\

// ======= Autocomplete Functionlaity End ========\\  
function InitializeAutoComplete(actionUrl, thisparam, minlen, param, status = "", appendTo = "") {
	if (actionUrl != undefined && actionUrl != "") {
		var controlId = "#" + $(thisparam).attr('id');
		var hdnId = controlId.replace('UI', '');

		$(controlId).autocomplete({
			source:
			function (request, response) {
				$.getJSON(
					actionUrl,
					param,
					response
				);
			},
			appendTo: appendTo,
			minLength: minlen,
			select: function (e, ui) {
				if (ui.item) {

					$(controlId).val(ui.item.label);
					$(hdnId).val(ui.item.value);
					if (ui.item.type != undefined && ui.item.type != '') $(hdnId + 'Type').val(ui.item.type);
					if (ui.item.typeId != undefined && ui.item.typeId != '') $(hdnId + 'TypeID').val(ui.item.typeId);
					if (ui.item.code != undefined && ui.item.code != '') $(hdnId + 'Code').val(ui.item.code);

					ui.item.placeholder = (ui.item.placeholder == true && ui.item.placeholder != null) ? "true" : "false";
					if ($(hdnId + 'PlaceHolder') != undefined) $(hdnId + 'PlaceHolder').val(ui.item.placeholder);

					$(".ui-menu-item").hide();
					$(".ui-menu").css("border", "none");
					//AutoCompleteChanged(this);
					e.preventDefault();
				} else {//if list null then hide the list 
					$(".ui-menu-item").hide();
					$(".ui-menu").css("border", "none");
				}
			},
			focus: function (e, ui) { //on key down/up ID shows in textbox to handle this focus event used 
				$(controlId).val(ui.item.label);
				$(".ui-widget.ui-widget-content").css("border", "1px solid #c5c5c5");
				e.preventDefault();
			},
			change: function (e, ui) { //if text is not present in list then blank the textbox   
				$(".ui-widget.ui-widget-content").css("border", "none");
				$(".ui-menu-item").hide();
				$(".ui-menu").css("border", "none");
				$(".ui-menu").css("width", "0px");//add for handling width on browsers 

				if (!ui.item) {
					$(this).val('');
					$(hdnId).val('');
				}
				//if ($(this).val().length >= minlen && (e.which == 0 || e.which === 13) && status != '') {
				if ((e.which == 0 || e.which === 13) && status != '') {
					AutoCompleteChanged(this);
				}
			}
		}).unbind('keypress').bind('keypress', function (e) {
			if ($(controlId).val().length >= minlen) {
				$(".ui-widget.ui-widget-content").css("border", "1px solid #c5c5c5");
			}
			else if ($(controlId).val().length < minlen) {
				$(".ui-menu-item").hide();
				$(".ui-menu").css("border", "none");
				$(".ui-autocomplete").css("width", "0px");//add for handling width on browsers
			}
			if (e.which === 13 || e.which === 27 || e.which === 0) {//on enter or escape or tab key hide list
				$(".ui-menu-item").hide();
				$(".ui-menu").css("border", "none");
				$(".ui-autocomplete").css("width", "0px");//add for handling width on browsers
			}
		}).bind('blur', function (e) { //if char in textbox is 1 then hide the list     
			if (e.which === 8 || e.which === 0) {//on backspace and tab key
				if ($(controlId).val().length < minlen) {
					$(".ui-autocomplete").css("width", "0px"); //add for handling width on browsers
					$(".ui-menu-item").hide();
					$(".ui-menu").css("border", "none");
				}
			}
			//$(controlId).trigger('autocompletechange');
			//$(controlId).data("ui-autocomplete")._trigger("change");
			//if ($(controlId).val() == null || $(controlId).val() == '') {
			//    $(hdnId).val('');
			//}
		});
	}
}

// ======= Autocomplete Functionlaity End ========\\


$(document).on('keypress', '.timeFormat', function (e) {
	if ($.inArray(e.which, [0, 58, 8, 9, 27, 13, 10]) !== -1 || (e.which === 65 && (e.ctrlKey === true || e.metaKey === true))) {
		return;
	}
	if (e.which < 48 || e.which > 57) {
		return false;
	}
	else {
		var timeValue = $(this).val();
		if (timeValue.includes(":")) {
			$(this).val(timeValue.substring(0, 2) + ":" + timeValue.substring(3, 5));
			if (timeValue.length > 4) return false;
		}
		else if (timeValue.length == 2) {
			$(this).val(timeValue + ":");
		}
		else if (timeValue.length > 2) {
			$(this).val(timeValue.substring(0, 2) + ":" + timeValue.substring(2, 4));
		}
	}
});

$(document).on('focusout', '.timeFormat', function (e) {
	var timeValue = $(this).val();
	if (timeValue == '') {
		$(this).siblings("span").text("");
		return true;
	}

	var numericReg = /^([01]?[0-9]|2[0-3]):[0-5][0-9]$/;
	if (!numericReg.test(timeValue)) {
		$(this).siblings("span").text("Invalid Time.");
		$(this).val('');
	}
	else {
		$(this).siblings("span").text("");
	}
});

$(document).on('focusout', '.time24', function (e) {
	 
	var timeStr = $(this).val();
	if (timeStr.indexOf('_') != -1) {
		timeStr = timeStr.replace(/_/g, '0');
		$(this).val(timeStr);
	}
	else {
		$(this).siblings("span").text("");
	}
});

$(document).on('keypress', '.time24', function (e) {
	timePickerInitialize();
});

function timePickerInitialize() {
	$('.time24').inputmask('hh:mm', { placeholder: '__:__ _m', alias: 'time24', hourFormat: '24' });
}

function selectInitialize() {
	$(document).on('change', '.select', function (evt) {
		var controlId = '#' + this.id.replace('ID', '');
        $(this.closest('form')).find(controlId).val(evt.target.selectedOptions[0].label).change();
		//$(controlId).val(evt.target.selectedOptions[0].label);
	});
}

$(document).on('change', '.selectName', function (evt) {
	var controlId = this.name.replace('ID', '');
	$('[name="' + controlId + '"]').val(evt.target.selectedOptions[0].label);
});


$('.multiselect').multiselect({
	buttonWidth: '100%',
	includeSelectAllOption: true,
	//onChange: function (option, checked, select) {
	//    alert('Changed option ' + $(option).val() + '.');
	//}
	//templates: {
	//    li: '<li><a><label class="checkbox"></label><input type="text"/></a></li>'
	//}
	//templates: {
	//    li: '<li><a><label style="display:inline;"></label><input type="text" /></a></li>'
	//}
	//optionLabel: function (element) {
	//    return $(element).html().append('<input class="form-control numeric" id="RoomCount" name="RoomCount" value="1" type="text"></input>');
	//}
	//buttonContainer: '<div class="btn-group"><input class="form-control numeric" id="RoomCount" name="RoomCount" value="1" type="text"></div>'
});

$(document).bind("ajaxSend", function () {
	$(".ajaxloader").show();
}).bind("ajaxComplete", function () {
	$(".ajaxloader").hide();
	ShowHideRoleBasedControls('QRF', 'price-foc-sec');
});

$.ajaxSetup({
	beforeSend: function (xhr) {
		xhr.setRequestHeader("IsAjaxRequest", "true");
	}
});

$(document).on('click', '.disabled a', function (evt) {
	return false;
});

$(".showLinkedQRFs").click(function () {
	var QRFID = $("#QRFId").val();
	if (QRFID == 'undefined' || QRFID == null) {
		QRFID = $("#QRFID").val();
	}
	$.ajax({
		type: "GET",
		url: "/Quote/GetLinkedQRFs",
		data: { QRFID: QRFID },
		contentType: "application/json; charset=utf-8",
		dataType: "html",
		success: function (response) {
			$("#QuoteVersionList-popup .popup-in").html(response);
			$("#QuoteVersionList-popup").show();
		},
		error: function (response) {
			alert(response.statusText);
		}
	});
});

//the below function checks date format as dd/MMM/yyyy
function ValidateDate(id) {
	var dtValue = $(id).val();
	var dtRegex = new RegExp("^([0]?[1-9]|[1-2]\\d|3[0-1])/(JAN|FEB|MAR|APR|MAY|JUN|JUL|AUG|SEP|OCT|NOV|DEC)/[1-2]\\d{3}$", 'i');
	return dtRegex.test(dtValue);
}

//the below function checks date format as mm/dd/yyyy
function ValidateDatemmddyyyy(dtValue) {
	var dtRegex = /^(0[1-9]|1[0-2])\/(0[1-9]|1\d|2\d|3[01])\/(19|20)\d{2}$/;
	return dtRegex.test(dtValue);
}

//the below function checks date format as dd/mm/yyyy
function ValidateDateddmmyyyy(dtValue) {
	var dtRegex = /^(0[1-9]|1\d|2\d|3[01])\/(0[1-9]|1[0-2])\/(19|20)\d{2}$/;
	return dtRegex.test(dtValue);
}

function datepickerInitialize() {
	$('.input-append.date').find('input.span3').datepicker({
		changeMonth: true,
		changeYear: true,
		showOn: "both",
		dateFormat: "dd/mm/yy",
		beforeShow: function (el, dp) {
			$(el).parent().append($('#ui-datepicker-div'));
			$('#ui-datepicker-div').hide();
		}
	});
}

var initUiInputAppendDate = function (e) {
	$('.input-append.date').find('input.span2').datepicker({
		changeMonth: true,
		changeYear: true,
		showOn: "both",
		firstDay: 0, // Start with Monday
		dayNamesMin: "Sun Mon Tue Wed Thu Fri Sat".split(" "),
		dateFormat: "dd/M/yy"
	});

	$('.input-append.date').find('input.span3').datepicker({
		changeMonth: true,
		changeYear: true,
		showOn: "both",
		dateFormat: "dd/mm/yy"
	});

	//$('.input-append.date').find('input.monthyearCal').datepicker({
	//    changeMonth: true,
	//    changeYear: true,
	//    showButtonPanel: true,
	//    dateFormat: 'MM yy',
	//    onClose: function (dateText, inst) {
	//        $(this).datepicker('setDate', new Date(inst.selectedYear, inst.selectedMonth, 1));
	//    }
	//});
};

//if date format is not in dd/mm/yyyy then it will clear the textbox
$(document).on("focusout", ".chkdate", function () {
	var newDt = $(this).val();
	if (!ValidateDatemmddyyyy(newDt)) {
		$(this).val('');
		$(this).parent(".input-append").find("span.text-danger").text("");
	}
	return true;
});

//below function allow numbers and alpha,slash,backspace, arrows keys,enter,del
$(document).on('keypress', '.alphanumericdate', function (e) {
	var charCode = e.which;
	if (charCode == 8 || charCode == 0) {
		return;
	}
	else {
		var keyChar = String.fromCharCode(charCode);
		return /[a-zA-Z0-9/]/.test(keyChar);
	}
});

//below function allow alpha,backspace, arrows keys,enter,del
$(document).on('keypress', '.onlyalpha', function (e) {
	var charCode = e.which;
	if (charCode == 8 || charCode == 0) {
		return;
	}
	else {
		var keyChar = String.fromCharCode(charCode);
		return /[a-zA-Z]/.test(keyChar);
	}
});

//below function allow numeric,+,-,space,backspace, arrows keys,enter,del
$(document).on('keypress', '.numericTelephone', function (e) {
	var charCode = e.which;
	if (charCode == 8 || charCode == 0) {
		return;
	}
	else {
		var keyChar = String.fromCharCode(charCode);
		return /[+-0-9 ]/.test(keyChar);
	}
});


function addMinutesToTime(time, minsToAdd) {
	function D(J) { return (J < 10 ? '0' : '') + J; };
	var piece = time.split(':');
	var mins = piece[0] * 60 + +piece[1] + +minsToAdd;

	return D(mins % (24 * 60) / 60 | 0) + ':' + D(mins % 60);
}

function NewGuid() {
	var u = (new Date()).getTime().toString(16) +
		Math.random().toString(16).substring(2) + "0".repeat(16);
	var guid = u.substr(0, 8) + '-' + u.substr(8, 4) + '-4000-8' +
		u.substr(12, 3) + '-' + u.substr(15, 12);
	return guid;
}