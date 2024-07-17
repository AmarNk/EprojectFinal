/*!
 * jQuery Validation Plugin v1.19.3
 *
 * https://jqueryvalidation.org/
 *
 * Copyright (c) 2021 Jörn Zaefferer
 * Released under the MIT license
 */
(function( factory ) {
	if ( typeof define === "function" && define.amd ) {
		define( ["jquery"], factory );
	} else if (typeof module === "object" && module.exports) {
		module.exports = factory( require( "jquery" ) );
	} else {
		factory( jQuery );
	}
}(function( $ ) {
	$.extend($.fn, {
		validate: function( options ) {
			if ( !this.length ) {
				if ( options && options.debug && window.console ) {
					console.warn( "Nothing selected, can't validate, returning nothing." );
				}
				return;
			}
			var validator = $.data( this[ 0 ], "validator" );
			if ( validator ) {
				return validator;
			}
			this.attr( "novalidate", "novalidate" );
			validator = new $.validator( options, this[ 0 ] );
			$.data( this[ 0 ], "validator", validator );
			if ( validator.settings.onsubmit ) {
				this.on( "click.validate", ":submit", function( event ) {
					validator.submitButton = event.currentTarget;
					if ( $( this ).hasClass( "cancel" ) ) {
						validator.cancelSubmit = true;
					}
					if ( $( this ).attr( "formnovalidate" ) !== undefined ) {
						validator.cancelSubmit = true;
					}
				});
				this.on( "submit.validate", function( event ) {
					if ( validator.settings.debug ) {
						event.preventDefault();
					}
					function handle() {
						var hidden, result;
						if ( validator.settings.submitHandler ) {
							if ( validator.submitButton ) {
								hidden = $( "<input type='hidden'/>" )
									.attr( "name", validator.submitButton.name )
									.val( $( validator.submitButton ).val() )
									.appendTo( validator.currentForm );
							}
							result = validator.settings.submitHandler.call( validator, validator.currentForm, event );
							if ( validator.submitButton ) {
								hidden.remove();
							}
							if ( result !== undefined ) {
								return result;
							}
							return false;
						}
						return true;
					}
					if ( validator.cancelSubmit ) {
						validator.cancelSubmit = false;
						return handle();
					}
					if ( validator.form() ) {
						if ( validator.pendingRequest ) {
							validator.formSubmitted = true;
							return false;
						}
						return handle();
					} else {
						validator.showErrors();
						return false;
					}
				});
			}
			return validator;
		},
		valid: function() {
			var valid, validator, errorList;
			if ( $( this[ 0 ] ).is( "form" ) ) {
				valid = this.validate().form();
			} else {
				errorList = [];
				valid = true;
				validator = $( this[ 0 ].form ).validate();
				this.each( function() {
					valid = validator.element( this ) && valid;
					if ( !valid ) {
						errorList = errorList.concat( validator.errorList );
					}
				});
				validator.errorList = errorList;
			}
			return valid;
		},
		rules: function( command, argument ) {
			var element = this[ 0 ],
				settings, staticRules, existingRules, data, param, filtered;
			if ( command ) {
				settings = $.data( element.form, "validator" ).settings;
				staticRules = settings.rules;
				existingRules = $.validator.staticRules( element );
				switch ( command ) {
				case "add":
					$.extend( existingRules, $.validator.normalizeRule( argument ) );
					staticRules[ element.name ] = existingRules;
					if ( argument.messages ) {
						settings.messages[ element.name ] = $.extend( settings.messages[ element.name ], argument.messages );
					}
					break;
				case "remove":
					if ( !argument ) {
						delete staticRules[ element.name ];
						return existingRules;
					}
					filtered = {};
					$.each( argument.split( /\s/ ), function( index, method ) {
						filtered[ method ] = existingRules[ method ];
						delete existingRules[ method ];
					});
					return filtered;
				}
			}
			data = $.validator.normalizeRules(
				$.extend(
					{},
					$.validator.classRules( element ),
					$.validator.attributeRules( element ),
					$.validator.dataRules( element ),
					$.validator.staticRules( element )
				), element
			);
			if ( data.required ) {
				param = data.required;
				delete data.required;
				data = $.extend( { required: param }, data );
			}
			if ( data.remote ) {
				param = data.remote;
				delete data.remote;
				data = $.extend( { remote: param }, data );
			}
			return data;
		}
	});
	$.extend( $.expr.pseudos || $.expr[ ":" ], {
		blank: function( a ) {
			return !$.trim( "" + $( a ).val() );
		},
		filled: function( a ) {
			var val = $( a ).val();
			return val !== null && !!$.trim( "" + val );
		},
		unchecked: function( a ) {
			return !$( a ).prop( "checked" );
		}
	});
	$.validator = function( options, form ) {
		this.settings = $.extend( true, {}, $.validator.defaults, options );
		this.currentForm = form;
		this.init();
	};
	$.validator.format = function( source, params ) {
		if ( arguments.length === 1 ) {
			return function() {
				var args = $.makeArray( arguments );
				args.unshift( source );
				return $.validator.format.apply( this, args );
			};
		}
		if ( params === undefined ) {
			return source;
		}
		if ( arguments.length > 2 && params.constructor !== Array  ) {
			params = $.makeArray( arguments ).slice( 1 );
		}
		if ( params.constructor !== Array ) {
			params = [ params ];
		}
		$.each( params, function( i, n ) {
			source = source.replace( new RegExp( "\\{" + i + "\\}", "g" ), function() {
				return n;
			});
		});
		return source;
	};
	$.extend( $.validator, {
		defaults: {
			messages: {},
			groups: {},
			rules: {},
			errorClass: "error",
			validClass: "valid",
			errorElement: "label",
			focusCleanup: false,
			focusInvalid: true,
			errorContainer: $( [] ),
			errorLabelContainer: $( [] ),
			onsubmit: true,
			ignore: ":hidden",
			ignoreTitle: false,
			onfocusin: function( element ) {
				this.lastActive = element;
				if ( this.settings.focusCleanup ) {
					if ( this.settings.unhighlight ) {
						this.settings.unhighlight.call( this, element, this.settings.errorClass, this.settings.validClass );
					}
					this.hideThese( this.errorsFor( element ) );
				}
			},
			onfocusout: function( element ) {
				if ( !this.checkable( element ) && ( element.name in this.submitted || !this.optional( element ) ) ) {
					this.element( element );
				}
			},
			onkeyup: function( element, event ) {
				var excludedKeys = [
					16, 17, 18, 20, 35, 36, 37,
					38, 39, 40, 45, 144, 225
				];
				if ( event.which === 9 && this.elementValue( element ) === "" ) {
					return;
				}
				if ( $.inArray( event.keyCode, excludedKeys ) !== -1 && !event.ctrlKey && !event.altKey && !event.metaKey ) {
					return;
				}
				if ( element.name in this.submitted || element === this.lastElement ) {
					this.element( element );
				}
			},
			onclick: function( element ) {
				if ( element.name in this.submitted ) {
					this.element( element );
				} else if ( element.name in this.invalid ) {
					this.showErrors();
				}
			},
			highlight: function( element, errorClass, validClass ) {
				if ( element.type === "radio" ) {
					this.findByName( element.name ).addClass( errorClass ).removeClass( validClass );
				} else {
					$( element ).addClass( errorClass ).removeClass( validClass );
				}
			},
			unhighlight: function( element, errorClass, validClass ) {
				if ( element.type === "radio" ) {
					this.findByName( element.name ).removeClass( errorClass ).addClass( validClass );
				} else {
					$( element ).removeClass( errorClass ).addClass( validClass );
				}
			}
		},
		setDefaults: function( settings ) {
			$.extend( $.validator.defaults, settings );
		},
		messages: {
			required: "This field is required.",
			remote: "Please fix this field.",
			email: "Please enter a valid email address.",
			url: "Please enter a valid URL.",
			date: "Please enter a valid date.",
			dateISO: "Please enter a valid date (ISO).",
			number: "Please enter a valid number.",
			digits: "Please enter only digits.",
			equalTo: "Please enter the same value again.",
			maxlength: $.validator.format( "Please enter no more than {0} characters." ),
			minlength: $.validator.format( "Please enter at least {0} characters." ),
			rangelength: $.validator.format( "Please enter a value between {0} and {1} characters long." ),
			range: $.validator.format( "Please enter a value between {0} and {1}." ),
			max: $.validator.format( "Please enter a value less than or equal to {0}." ),
			min: $.validator.format( "Please enter a value greater than or equal to {0}." ),
			step: $.validator.format( "Please enter a multiple of {0}." ),
			creditcard: "Please enter a valid credit card number.",
			phoneUS: "Please specify a valid phone number.",
			phoneUK: "Please specify a valid phone number.",
			mobileUK: "Please specify a valid mobile number.",
			strippedminlength: $.validator.format( "Please enter at least {0} characters." ),
			email2: "Please enter a valid email address.",
			url2: "Please enter a valid URL.",
			creditcardtypes: "Please enter a valid credit card number.",
			ipv4: "Please enter a valid IP v4 address.",
			ipv6: "Please enter a valid IP v6 address.",
			require_from_group: $.validator.format( "Please fill at least {0} of these fields." ),
			nifES: "Please enter a valid NIF number.",
			nieES: "Please enter a valid NIE number.",
			cifES: "Please enter a valid CIF number.",
			postalCodeCA: "Please enter a valid postal code.",
			zipcodeUS: "Please enter a valid zipcode.",
			postalcodeIT: "Please enter a valid postal code."
		}
	});
}));
