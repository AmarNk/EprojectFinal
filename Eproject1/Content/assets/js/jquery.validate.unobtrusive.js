/*!
** Unobtrusive validation support library for jQuery and jQuery Validate
** https://github.com/aspnet/jquery-validation-unobtrusive
** Copyright (c) .NET Foundation and contributors
** Licensed under the MIT license
*/

(function(factory) {
    if (typeof define === "function" && define.amd) {
        define("jquery.validate.unobtrusive", ["jquery-validation"], factory);
    } else if (typeof module === "object" && module.exports) {
        module.exports = factory(require("jquery-validation"));
    } else {
        jQuery.validator.unobtrusive = factory(jQuery);
    }
}(function($) {
    var $jQval = $.validator,
        adapters,
        data_validation = "unobtrusiveValidation";

    function setValidationValues(options, ruleName, value) {
        options.rules[ruleName] = value;
        if (options.message) {
            options.messages[ruleName] = options.message;
        }
    }

    function parseAndExtendOptions(element, options, name, attributes) {
        var prefix = "data-val-" + name,
            message = element.getAttribute(prefix),
            paramName,
            paramValue;

        if (message) {
            options.messages[name] = message;
            for (paramName in attributes) {
                paramValue = element.getAttribute(prefix + "-" + paramName);
                if (paramValue) {
                    options.rules[name] = paramValue;
                }
            }
        }
    }

    function attachValidation($element) {
        var validator = $element.parents("form").data(data_validation),
            options = { rules: {}, messages: {} },
            adapters;

        if (!validator) {
            return;
        }

        adapters = $jQval.unobtrusive.adapters;
        $.each(adapters, function() {
            var prefix = "data-val-" + this.name,
                message = $element.attr(prefix),
                paramValues = {};

            if (message) {
                this.params.forEach(function(paramName) {
                    var paramValue = $element.attr(prefix + "-" + paramName);
                    if (paramValue) {
                        paramValues[paramName] = paramValue;
                    }
                });
                this.adapt({
                    element: $element[0],
                    message: message,
                    params: paramValues,
                    rules: options.rules,
                    messages: options.messages
                });
            }
        });

        $jQval.unobtrusive.parseElement($element, options);
    }

    function parse(form) {
        var $form = $(form),
            $elements;

        if (!$form.data(data_validation)) {
            $form.data(data_validation, true);
            $elements = $form.find(":input").not("[type=submit],[type=reset],[type=button],[disabled],[readonly]");
            $elements.each(function() {
                attachValidation($(this));
            });
        }
    }

    adapters = [];
    $.extend($jQval.unobtrusive, {
        adapters: adapters,
        parseElement: function(element, options) {
            var form = $(element).closest("form")[0],
                $element = $(element),
                isGrouped = $element.data("valgroup"),
                adapter = $element.data("val-adapter");

            if (!form || !adapter || $element.is("[readonly]")) {
                return;
            }

            if (isGrouped) {
                options.groups[isGrouped] = $element.attr("name");
            }

            $.each(adapter.params, function(paramName) {
                var paramValue = $element.data("val-" + adapter.name + "-" + paramName);
                if (paramValue !== undefined) {
                    options.rules[adapter.name] = options.rules[adapter.name] || {};
                    options.rules[adapter.name][paramName] = paramValue;
                }
            });

            setValidationValues(options, adapter.name, true);
        },
        parse: parse
    });

    adapters.add = function(name, params, method) {
        if (!method) {
            method = params;
            params = [];
        }
        this.push({ name: name, params: params, adapt: method });
        return this;
    };

    adapters.addBool = function(name, message) {
        return this.add(name, function(options) {
            setValidationValues(options, name, true);
            if (message) {
                options.messages[name] = message;
            }
        });
    };

    adapters.addMinMax = function(name, minOrMax, message, minMax, minMaxMessage) {
        return this.add(name, [minOrMax, minMax], function(options) {
            setValidationValues(options, name, [options.params[minOrMax], options.params[minMax]]);
            if (message) {
                options.messages[name] = message;
            }
            if (minMaxMessage) {
                options.messages[name + "-" + minMax] = minMaxMessage;
            }
        });
    };

    adapters.addSingleVal = function(name, param, message) {
        return this.add(name, [param], function(options) {
            setValidationValues(options, name, options.params[param]);
            if (message) {
                options.messages[name] = message;
            }
        });
    };

    $(function() {
        $jQval.unobtrusive.parse(document);
    });

    return $jQval.unobtrusive;
}));
