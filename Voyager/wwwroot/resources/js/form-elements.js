/* Initialisers
-------------------------------*/
forms = $('form, .form');
isIE7 = $('html').hasClass('ie7');
isIE8 = $('html').hasClass('ie8');
touchEnabled = Modernizr.touch;		
formPlaceholders = Modernizr.input.placeholder;
boxShadows = Modernizr.boxshadow;


	//////////////////////
	// initalises forms, standardises cross-device behaviours
	//////////////////////
	function initForms(){
		forms.each(function(){
			var $this = $(this),
				allInputs = $this.find('input'),
				allSelects = $this.find('select'),
				placeholderInputs = $this.find('[placeholder]'),
				checkboxInputs = $this.find('input[type="checkbox"]').not('.toggle-checkbox'),
				radioInputs = $this.find('input[type="radio"]'),
				coupledCheckboxes = $this.find('.coupled-checks');
			
			// apply custom iCheck styles to checkboxes
			checkboxInputs.iCheck({
				'checkboxClass': 'styled-checkbox',
				'checkedClass': 'styled-checkbox-checked',
				'insert': '<div class="check"></div>'
			});

				

			// apply custom iCheck styles to radios
			radioInputs.iCheck({
				'radioClass': 'styled-radio',
				'checkedClass': 'styled-radio-checked',
				'insert': '<div class="check"></div>'
			});
			
			// ensure all forms are reset
			if (this.hasClass = "form"){
			}
			else{
			this.reset();
			}

			 // loop over each input
			 allInputs.each(function(){
				var $this = $(this),
					type = $this.attr('type');
				
				if(!boxShadows){
					$this.addClass(type+'-input');	
				}
			 });

			 // loop over each select to build custom select styles
			 allSelects.each(function(){
				
				// not for IE7
				if(!isIE7){
					var $this = $(this),
						wrap = $('<div />', {
							'class':	'styled-select'
						});
						
					// wrap
					$this.wrap(wrap);
	
					// invoke bootstrap-select plugin
					
					$this.selectpicker({
						'showIcon':	true,
						'dropupAuto': false,
						'showSubtext' : true,
						'showContent' : true
					});
					
					if (window.PIE) {$('.form-group .styled-select .btn-group .btn').each(function() {PIE.attach(this);});} 	
				}
			 });

			// if no native support for placeholders
			if(!formPlaceholders){
				// loop through each
				placeholderInputs.each(function(){
					var $this = $(this),
						placeholderClass = 'placeholder',
						thisText = $this.val(),
						placeholderText = $this.attr('placeholder');
	
					// initialisation
					if(thisText !== placeholderText){
						//add placeholder classname and set value to placeholder text
						if(!$this.hasClass('password-input')){
						   $this.addClass(placeholderClass).val(placeholderText);
						   
						   }
						else{
						   //alert($this.attr('type'))
						   $this.wrap( "<div></div>" );
						   $this.addClass('real-pass').val('').parent().addClass('ie8-pass-div').append('<input type="text" class="dummy-pass" value="'+placeholderText+'"/>');
						   
						}
					}
					
					//focus
					$this.not(".real-pass, .dummy-pass").focus(function(){
						if($this.val() == $this.attr('placeholder')){
						   if(!$this.hasClass('password-input')){
							$this.removeClass(placeholderClass).val('');
							}	
						}
					});
					
					//blur
					$this.not(".real-pass, .dummy-pass").blur(function(){
						if($this.val() === '' || $this.val() == $this.attr('placeholder')){
						  if(!$this.hasClass('password-input')){
							//add placeholder classname and set value to placeholder text
							$this.addClass(placeholderClass).val(placeholderText);
							}
						}
					})
					.blur();
				});
			}
			
			// coupled checkboxes
			if(coupledCheckboxes.length){
				coupledCheckboxes.each(function(){
					var checkboxes = $(this).find('input[type=checkbox]');
	
					// loop over each checkbox
					checkboxes.each(function(){
						// identify other checkboxes in this set
						var otherChecks = checkboxes.not($(this));
						
						// tie into iCheck 'change' event
						$(this).on('ifChanged', function(){
							// if this is now selected
							if($(this).attr('checked') == 'checked'){
								otherChecks.iCheck('uncheck');
							}
						});
					});
				});
			}
			
		});
		
		//Customisation Of Onselect, OnChecked
        //$('.specific-time').on('ifChanged', function(){ $('#contactDateDiv').slideToggle();});
        //$('.rooming-same-select').on('ifChanged', function(){$('.rooming-same').toggleClass('select');});
        //$('.room-select').on('ifChanged', function(){$(this).parent().parent().parent().toggleClass('selected');});
        //$('.online').on('ifChecked', function(){$('.payment-mode').show();$('#offline').fadeOut(function(){$('#online').fadeIn()});$('.book-ext-sect li:first-child').removeClass('last');});
        //$('.offline').on('ifChecked', function(){$('.payment-mode').show();$('#online').fadeOut(function(){$('#offline').fadeIn()});$('.book-ext-sect li:first-child').removeClass('last');});
		/** Bokig Summary ***/
        //$('.PrintAll').on('ifChecked', function(){ $(".Print").iCheck('check').iCheck('disable'); $(".print-check-row").addClass('disabled');});
        //$('.EmailAll').on('ifChecked', function(){ $(".Email").iCheck('check').iCheck('disable'); $(".email-check-row").addClass('disabled');});
        //$('.PrintAll').on('ifUnchecked', function(){ $(".Print").iCheck('uncheck').iCheck('enable'); $(".print-check-row").removeClass('disabled');});
        //$('.EmailAll').on('ifUnchecked', function(){ $(".Email").iCheck('uncheck').iCheck('enable'); $(".email-check-row").removeClass('disabled');});
        //$('.CheckAll').on('ifChecked', function(){ $(this).closest('.pax-tbl').find(".Check").iCheck('check').iCheck('disable'); $(this).closest('.pax-tbl').find(".check-row").addClass('disabled');});
        //$('.CheckAll').on('ifUnchecked', function(){$(this).closest('.pax-tbl').find(".Check").iCheck('uncheck').iCheck('enable'); $(this).closest('.pax-tbl').find(".check-row").removeClass('disabled');});
		/***/
        
        $('input[type="radio"]').on('ifChanged', function(){$(this).parents('label').toggleClass('selected');});
	}

/**********Custom Date Picker birthdate********/


initForms();


/* Ajax based Forms Initialisers
-------------------------------*/
AjaxForm = $('.ajaxform');

	//////////////////////
	// initalises forms, standardises cross-device behaviours
	//////////////////////
	function initAjaxForms(){
		AjaxForm.each(function(){
			var $this = $(this),
				allInputs = $this.find('input'),
				allSelects = $this.find('select'),
				placeholderInputs = $this.find('[placeholder]'),
				checkboxInputs = $this.find('input[type="checkbox"]'),
				radioInputs = $this.find('input[type="radio"]'),
				coupledCheckboxes = $this.find('.coupled-checks');
			
			// apply custom iCheck styles to checkboxes
			checkboxInputs.iCheck({
				'checkboxClass': 	'styled-checkbox',
				'checkedClass': 	'styled-checkbox-checked',
				'insert':			'<div class="check"></div>'
			});

			// apply custom iCheck styles to radios
			radioInputs.iCheck({
				'radioClass':		'styled-radio',
				'checkedClass': 	'styled-radio-checked',
				'insert':			'<div class="check"></div>'
			});
			
			// ensure all forms are reset
			if (this.hasClass = "form"){
			}
			else{
			this.reset();
			}

			 // loop over each input
			 allInputs.each(function(){
				var $this = $(this),
					type = $this.attr('type');
				
				if(!boxShadows){
					$this.addClass(type+'-input');	
				}
			 });

			 // loop over each select to build custom select styles
			 allSelects.each(function(){
				
				// not for IE7
				if(!isIE7){
					var $this = $(this),
						wrap = $('<div />', {
							'class':	'styled-select'
						});
						
					// wrap
					$this.wrap(wrap);
	
					// invoke bootstrap-select plugin
					
					$this.selectpicker({
						'showIcon':	true,
						'dropupAuto':	false,
						'showSubtext' : true,
						'showContent' : true
					});
					
					if (window.PIE) {$('.form-group .styled-select .btn-group .btn').each(function() {PIE.attach(this);});} 	
				}
			 });

			// if no native support for placeholders
			if(!formPlaceholders){
				// loop through each
				placeholderInputs.each(function(){
					var $this = $(this),
						placeholderClass = 'placeholder',
						thisText = $this.val(),
						placeholderText = $this.attr('placeholder');
	
					if(thisText !== placeholderText){
						if(!$this.hasClass('password-input')){
						   $this.addClass(placeholderClass).val(placeholderText);
						   
						   }
						else{
						   //alert($this.attr('type'))
						   $this.wrap( "<div></div>" );
						   $this.addClass('real-pass').val('').parent().addClass('ie8-pass-div').append('<input type="text" class="dummy-pass" value="'+placeholderText+'"/>');
						   
						}
					}
					
					//focus
					$this.not('.real-pass, .dummy-pass').focus(function(){
						if($this.val() == $this.attr('placeholder')){
						   if(!$this.hasClass('password-input')){
							$this.removeClass(placeholderClass).val('');
							}
						}
					});
					
					//blur
					$this.not('.real-pass, .dummy-pass').blur(function(){
						if($this.val() === '' || $this.val() == $this.attr('placeholder')){
						  if(!$this.hasClass('password-input')){
							//add placeholder classname and set value to placeholder text
							$this.addClass(placeholderClass).val(placeholderText);
							}
						}
					})
					.blur();
				});
			}
			
			// coupled checkboxes
			if(coupledCheckboxes.length){
				coupledCheckboxes.each(function(){
					var checkboxes = $(this).find('input[type=checkbox]');
	
					// loop over each checkbox
					checkboxes.each(function(){
						// identify other checkboxes in this set
						var otherChecks = checkboxes.not($(this));
						
						// tie into iCheck 'change' event
						$(this).on('ifChanged', function(){
							// if this is now selected
							if($(this).attr('checked') == 'checked'){
								otherChecks.iCheck('uncheck');
							}
						});
					});
				});
				
			
			}
			
		});
		

	}

/*** PAssword placholder issue hack for IE8 ***/
$('.real-pass').hide();
$('.dummy-pass').show();
// On focus of the fake password field
$('.dummy-pass').focus(function(){
    $(this).hide(); //  hide the fake password input text
    $(this).parent().find('.real-pass').show().focus(); // and show the real password input password
});

// On blur of the real pass
$('.real-pass').on('blur', function(){
    if($(this).val() == ''){ // if the value is empty, 
        $(this).hide(); // hide the real password field
        $(this).parent().find('.dummy-pass').show(); // show the fake password
		//alert('dummy-pass hide')
    }
}).blur();

/*** ends ***/	


var initUiInputAppendDate = function(e) {
$('.input-append.date').find('input.span2').datepicker({
	changeMonth:true,
	changeYear:true,
	showOn:"both",
	firstDay:0, // Start with Monday
	dayNamesMin:"Sun Mon Tue Wed Thu Fri Sat".split(" "),
	dateFormat :"dd/mm/yy"
});
}
