"use strict";

// Class definition
var JsImageInput = function(element, options) {
    ////////////////////////////
    // ** Private Variables  ** //
    ////////////////////////////
    var that = this;

    if ( typeof element === "undefined" || element === null ) {
        return;
    }

    // Default Options
    var defaultOptions = {
        
    };

    ////////////////////////////
    // ** Private Methods  ** //
    ////////////////////////////

    var _construct = function() {
        if ( JsUtils.data(element).has('image-input') === true ) {
            that = JsUtils.data(element).get('image-input');
        } else {
            _init();
        }
    }

    var _init = function() {
        // Variables
        that.options = JsUtils.deepExtend({}, defaultOptions, options);
        that.uid = JsUtils.getUniqueId('image-input');

        // Elements
        that.element = element;
        that.inputElement = JsUtils.find(element, 'input[type="file"]');
        that.wrapperElement = JsUtils.find(element, '.t-image-input-wrapper');
        that.cancelElement = JsUtils.find(element, '[data-tu-image-input-action="cancel"]');
        that.removeElement = JsUtils.find(element, '[data-tu-image-input-action="remove"]');
        that.hiddenElement = JsUtils.find(element, 'input[type="hidden"]');
        that.src = JsUtils.css(that.wrapperElement, 'backgroundImage');

        // Set initialized
        that.element.setAttribute('data-tu-image-input', 'true');

        // Event Handlers
        _handlers();

        // Bind Instance
        JsUtils.data(that.element).set('image-input', that);
    }

    // Init Event Handlers
    var _handlers = function() {
        JsUtils.addEvent(that.inputElement, 'change', _change);
        JsUtils.addEvent(that.cancelElement, 'click', _cancel);
        JsUtils.addEvent(that.removeElement, 'click', _remove);
    }

    // Event Handlers
    var _change = function(e) {
        e.preventDefault();

        if ( that.inputElement !== null && that.inputElement.files && that.inputElement.files[0] ) {
            // Fire change event
            if ( JsEventHandler.trigger(that.element, 'tu.imageinput.change', that) === false ) {
                return;
            }

            var reader = new FileReader();

            reader.onload = function(e) {
                JsUtils.css(that.wrapperElement, 'background-image', 'url('+ e.target.result +')');
            }

            reader.readAsDataURL(that.inputElement.files[0]);

            JsUtils.addClass(that.element, 't-image-input-changed');
            JsUtils.removeClass(that.element, 't-image-input-empty');

            // Fire removed event
            JsEventHandler.trigger(that.element, 'tu.imageinput.changed', that);
        }
    }

    var _cancel = function(e) {
        e.preventDefault();

        // Fire cancel event
        if ( JsEventHandler.trigger(that.element, 'tu.imageinput.cancel', that) === false ) {
            return;
        }

        JsUtils.removeClass(that.element, 't-image-input-changed');
        JsUtils.removeClass(that.element, 't-image-input-empty');
        JsUtils.css(that.wrapperElement, 'background-image', that.src);
        that.inputElement.value = "";

        if ( that.hiddenElement !== null ) {
            that.hiddenElement.value = "0";
        }

        // Fire canceled event
        JsEventHandler.trigger(that.element, 'tu.imageinput.canceled', that);
    }

    var _remove = function(e) {
        e.preventDefault();

        // Fire remove event
        if ( JsEventHandler.trigger(that.element, 'tu.imageinput.remove', that) === false ) {
            return;
        }

        JsUtils.removeClass(that.element, 't-image-input-changed');
        JsUtils.addClass(that.element, 't-image-input-empty');
        JsUtils.css(that.wrapperElement, 'background-image', "none");
        that.inputElement.value = "";

        if ( that.hiddenElement !== null ) {
            that.hiddenElement.value = "1";
        }

        // Fire removed event
        JsEventHandler.trigger(that.element, 'tu.imageinput.removed', that);
    }

    var _destroy = function() {
        JsUtils.data(that.element).remove('image-input');
    }

    // Construct Class
    _construct();

    ///////////////////////
    // ** Public API  ** //
    ///////////////////////

    // Plugin API
    that.getInputElement = function() {
        return that.inputElement;
    }

    that.goElement = function() {
        return that.element;
    }
    
    that.destroy = function() {
        return _destroy();
    }

    // Event API
    that.on = function(name, handler) {
        return JsEventHandler.on(that.element, name, handler);
    }

    that.one = function(name, handler) {
        return JsEventHandler.one(that.element, name, handler);
    }

    that.off = function(name) {
        return JsEventHandler.off(that.element, name);
    }

    that.trigger = function(name, event) {
        return JsEventHandler.trigger(that.element, name, event, that, event);
    }
};

// Static methods
JsImageInput.getInstance = function(element) {
    if ( element !== null && JsUtils.data(element).has('image-input') ) {
        return JsUtils.data(element).get('image-input');
    } else {
        return null;
    }
}

// Create instances
JsImageInput.createInstances = function(selector = '[data-tu-image-input]') {
    // Initialize Menus
    var elements = document.querySelectorAll(selector);

    if ( elements && elements.length > 0 ) {
        for (var i = 0, len = elements.length; i < len; i++) {
            new JsImageInput(elements[i]);
        }
    }
}

// Global initialization
JsImageInput.init = function() {
    JsImageInput.createInstances();
};

// On document ready
if (document.readyState === 'loading') {
   document.addEventListener('DOMContentLoaded', JsImageInput.init);
} else {
    JsImageInput.init();
}

// Webpack Support
if (typeof module !== 'undefined' && typeof module.exports !== 'undefined') {
    module.exports = JsImageInput;
}
