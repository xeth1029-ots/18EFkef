
// Class definition
const JsToggle = function(element, options) {
    ////////////////////////////
    // ** Private variables  ** //
    ////////////////////////////
    var the = this;
    var body = document.getElementsByTagName("BODY")[0];

    if (!element) {
        return;
    }

    // Default Options
    var defaultOptions = {
        saveState: true
    };

    ////////////////////////////
    // ** Private methods  ** //
    ////////////////////////////

    var _construct = function() {
        if ( TuUtil.data(element).has('toggle') === true ) {
            the = TuUtil.data(element).get('toggle');
        } else {
            _init();
        }
    }

    var _init = function() {
        // Variables
        the.options = TuUtil.deepExtend({}, defaultOptions, options);
        the.uid = TuUtil.getUniqueId('toggle');

        // Elements
        the.element = element;

        the.target = document.querySelector(the.element.getAttribute('data-tu-toggle-target')) ? document.querySelector(the.element.getAttribute('data-tu-toggle-target')) : the.element;
        the.state = the.element.hasAttribute('data-tu-toggle-state') ? the.element.getAttribute('data-tu-toggle-state') : '';
        the.attribute = 'data-tu-' + the.element.getAttribute('data-tu-toggle-name');

        // Event Handlers
        _handlers();

        // Bind Instance
        TuUtil.data(the.element).set('toggle', the);
    }

    var _handlers = function() {
        TuUtil.addEvent(the.element, 'click', function(e) {
            e.preventDefault();

            _toggle();
        });
    }

    // Event handlers
    var _toggle = function() {
        // Trigger "after.toggle" event
        TuEventHandler.trigger(the.element, 'tu.toggle.change', the);

        if ( _isEnabled() ) {
            _disable();
        } else {
            _enable();
        }

        // Trigger "before.toggle" event
        TuEventHandler.trigger(the.element, 'tu.toggle.changed', the);

        return the;
    }

    var _enable = function() {
        if ( _isEnabled() === true ) {
            return;
        }

        TuEventHandler.trigger(the.element, 'tu.toggle.enable', the);

        the.target.setAttribute(the.attribute, 'on');

        if (the.state.length > 0) {
            the.element.classList.add(the.state);
        }        

        if ( typeof TuCookie !== 'undefined' && the.options.saveState === true ) {
            TuCookie.set(the.attribute, 'on');
        }

        TuEventHandler.trigger(the.element, 'tu.toggle.enabled', the);

        return the;
    }

    var _disable = function() {
        if ( _isEnabled() === false ) {
            return;
        }

        TuEventHandler.trigger(the.element, 'tu.toggle.disable', the);

        the.target.removeAttribute(the.attribute);

        if (the.state.length > 0) {
            the.element.classList.remove(the.state);
        } 

        if ( typeof TuCookie !== 'undefined' && the.options.saveState === true ) {
            TuCookie.remove(the.attribute);
        }

        TuEventHandler.trigger(the.element, 'tu.toggle.disabled', the);

        return the;
    }

    var _isEnabled = function() {
        return (String(the.target.getAttribute(the.attribute)).toLowerCase() === 'on');
    }

    var _destroy = function() {
        TuUtil.data(the.element).remove('toggle');
    }

    // Construct class
    _construct();

    ///////////////////////
    // ** Public API  ** //
    ///////////////////////

    // Plugin API
    the.toggle = function() {
        return _toggle();
    }

    the.enable = function() {
        return _enable();
    }

    the.disable = function() {
        return _disable();
    }

    the.isEnabled = function() {
        return _isEnabled();
    }

    the.goElement = function() {
        return the.element;
    }

    the.destroy = function() {
        return _destroy();
    }

    // Event API
    the.on = function(name, handler) {
        return TuEventHandler.on(the.element, name, handler);
    }

    the.one = function(name, handler) {
        return TuEventHandler.one(the.element, name, handler);
    }

    the.off = function(name) {
        return TuEventHandler.off(the.element, name);
    }

    the.trigger = function(name, event) {
        return TuEventHandler.trigger(the.element, name, event, the, event);
    }
};

// Static methods
JsToggle.getInstance = function(element) {
    if ( element !== null && TuUtil.data(element).has('toggle') ) {
        return TuUtil.data(element).get('toggle');
    } else {
        return null;
    }
}

// Create instances
JsToggle.createInstances = function(selector = '[data-tu-toggle]') {
    var body = document.getElementsByTagName("BODY")[0];

    // Get instances
    var elements = body.querySelectorAll(selector);

    if ( elements && elements.length > 0 ) {
        for (var i = 0, len = elements.length; i < len; i++) {
            // Initialize instances
            new JsToggle(elements[i]);
        }
    }
}

// Global initialization
JsToggle.init = function() {
    JsToggle.createInstances();
};

// On document ready
if (document.readyState === 'loading') {
   document.addEventListener('DOMContentLoaded', JsToggle.init);
} else {
    JsToggle.init();
}

// // Webpack support
// if (typeof module !== 'undefined' && typeof module.exports !== 'undefined') {
//     module.exports = JsToggle;
// }
