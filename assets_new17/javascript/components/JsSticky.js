"use strict";

// Class definition
const JsSticky = function(element, options) {
    ////////////////////////////
    // ** Private Variables  ** //
    ////////////////////////////
    const that = this;
    const body = document.getElementsByTagName("BODY")[0];

    if (typeof element === "undefined" || element === null) {
        return;
    }

    // Default Options
    var defaultOptions = {
        offset: 200,
        releaseOffset: 0,
        addClass: 'isfix',
        reverse: false,
        animation: true,
        animationSpeed: '0.3s',
        animationClass: 'animation-slide-in-down'
    };
    ////////////////////////////
    // ** Private Methods  ** //
    ////////////////////////////

    var _construct = function() {
        if (JsUtils.data(element).has('sticky') === true) {
            that = JsUtils.data(element).get('sticky');
        } else {
            _init();
        }
    }

    var _init = function() {
        that.element = element;
        that.options = JsUtils.deepExtend({}, defaultOptions, options);
        that.uid = JsUtils.getUniqueId('sticky');
        that.name = that.element.getAttribute('data-tu-sticky-name');
        that.attributeName = 'data-tu-sticky-' + that.name;
        that.eventTriggerState = true;
        that.lastScrollTop = 0;
        that.scrollHandler;

        // Set initialized
        that.element.setAttribute('data-tu-sticky', 'true');

        // Event Handlers
        window.addEventListener('scroll', _scroll);

        // Initial Launch
        _scroll();

        // Bind Instance
        JsUtils.data(that.element).set('sticky', that);
    }

    var _scroll = function(e) {
        var offset = _getOption('offset');
        var releaseOffset = _getOption('release-offset');
        var reverse = _getOption('reverse');
        var st;
        var attrName;
        var diff;

        // Exit if false
        if (offset === false) {
            return;
        }

        offset = parseInt(offset);
        releaseOffset = releaseOffset ? parseInt(releaseOffset) : 0;
        st = JsUtils.getScrollTop();
        diff = document.documentElement.scrollHeight - window.innerHeight - JsUtils.getScrollTop();

        if (reverse === true) { // Release on reverse scroll mode
            if (st > offset && (releaseOffset === 0 || releaseOffset < diff)) {
                if (body.hasAttribute(that.attributeName) === false) {
                    _enable();
                    body.setAttribute(that.attributeName, 'on');
                }

                if (that.eventTriggerState === true) {
                    JsEventHandler.trigger(that.element, 'tu.sticky.on', that);
                    JsEventHandler.trigger(that.element, 'tu.sticky.change', that);

                    that.eventTriggerState = false;
                }
            } else { // Back scroll mode
                if (body.hasAttribute(that.attributeName) === true) {
                    _disable();
                    body.removeAttribute(that.attributeName);
                }

                if (that.eventTriggerState === false) {
                    JsEventHandler.trigger(that.element, 'tu.sticky.off', that);
                    JsEventHandler.trigger(that.element, 'tu.sticky.change', that);
                    that.eventTriggerState = true;
                }
            }

            that.lastScrollTop = st;
        } else { // Classic scroll mode
            if (st > offset && (releaseOffset === 0 || releaseOffset < diff)) {
                if (body.hasAttribute(that.attributeName) === false) {
                    _enable();
                    body.setAttribute(that.attributeName, 'on');
                }

                if (that.eventTriggerState === true) {
                    JsEventHandler.trigger(that.element, 'tu.sticky.on', that);
                    JsEventHandler.trigger(that.element, 'tu.sticky.change', that);
                    that.eventTriggerState = false;
                }
            } else { // back scroll mode
                if (body.hasAttribute(that.attributeName) === true) {
                    _disable();
                    body.removeAttribute(that.attributeName);
                }

                if (that.eventTriggerState === false) {
                    JsEventHandler.trigger(that.element, 'tu.sticky.off', that);
                    JsEventHandler.trigger(that.element, 'tu.sticky.change', that);
                    that.eventTriggerState = true;
                }
            }
        }

        if (releaseOffset > 0) {
            if (diff < releaseOffset) {
                that.element.setAttribute('data-tu-sticky-released', 'true');
            } else {
                that.element.removeAttribute('data-tu-sticky-released');
            }
        }
    }

    var _enable = function(update) {
        var top = _getOption('top');
        var left = _getOption('left');
        var right = _getOption('right');
        var width = _getOption('width');
        var zindex = _getOption('zindex');

        if (update !== true && _getOption('animation') === true) {
            JsUtils.css(that.element, 'animationDuration', _getOption('animationSpeed'));
            JsUtils.animateClass(that.element, 'animation ' + _getOption('animationClass'));
        }

        if (zindex !== null) {
            JsUtils.css(that.element, 'z-index', zindex);
            JsUtils.css(that.element, 'position', 'fixed');
            JsUtils.addClass(that.element, 'is-fix');
        }

        if (top !== null) {
            JsUtils.css(that.element, 'top', top);
        }

        if (width !== null) {
            if (width['target']) {
                var targetElement = document.querySelector(width['target']);
                if (targetElement) {
                    width = JsUtils.css(targetElement, 'width');
                }
            }

            JsUtils.css(that.element, 'width', width);
        }

        if (left !== null) {
            if (String(left).toLowerCase() === 'auto') {
                var offsetLeft = JsUtils.offset(that.element).left;

                if (offsetLeft > 0) {
                    JsUtils.css(that.element, 'left', String(offsetLeft) + 'px');
                }
            }
        }
    }

    var _disable = function() {
        JsUtils.css(that.element, 'top', '');
        JsUtils.css(that.element, 'width', '');
        JsUtils.css(that.element, 'left', '');
        JsUtils.css(that.element, 'right', '');
        JsUtils.css(that.element, 'z-index', '');
        JsUtils.css(that.element, 'position', '');
        JsUtils.removeClass(that.element, 'is-fix');
    }

    var _getOption = function(name) {
        if (that.element.hasAttribute('data-tu-sticky-' + name) === true) {
            var attr = that.element.getAttribute('data-tu-sticky-' + name);
            var value = JsUtils.getResponsiveValue(attr);

            if (value !== null && String(value) === 'true') {
                value = true;
            } else if (value !== null && String(value) === 'false') {
                value = false;
            }

            return value;
        } else {
            var optionName = JsUtils.snakeToCamel(name);

            if (that.options[optionName]) {
                return JsUtils.getResponsiveValue(that.options[optionName]);
            } else {
                return null;
            }
        }
    }

    var _destroy = function() {
        window.removeEventListener('scroll', _scroll);
        JsUtils.data(that.element).remove('sticky');
    }

    // Construct Class
    _construct();

    ///////////////////////
    // ** Public API  ** //
    ///////////////////////

    // Methods
    that.update = function() {
        if (body.hasAttribute(that.attributeName) === true) {
            _disable();
            body.removeAttribute(that.attributeName);
            _enable(true);
            body.setAttribute(that.attributeName, 'on');
        }
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
JsSticky.getInstance = function(element) {
    if (element !== null && JsUtils.data(element).has('sticky')) {
        return JsUtils.data(element).get('sticky');
    } else {
        return null;
    }
}

// Create instances
JsSticky.createInstances = function(selector = '[data-tu-sticky="true"]') {
    var body = document.getElementsByTagName("BODY")[0];

    // Initialize Menus
    var elements = body.querySelectorAll(selector);
    var sticky;

    if (elements && elements.length > 0) {
        for (var i = 0, len = elements.length; i < len; i++) {
            sticky = new JsSticky(elements[i]);
        }
    }
}

// Window resize handler
window.addEventListener('resize', function() {
    var timer;
    var body = document.getElementsByTagName("BODY")[0];

    JsUtils.throttleTime(timer, function() {
        // Locate and update Offcanvas instances on window resize
        var elements = body.querySelectorAll('[data-tu-sticky="true"]');

        if (elements && elements.length > 0) {
            for (var i = 0, len = elements.length; i < len; i++) {
                var sticky = JsSticky.getInstance(elements[i]);
                if (sticky) {
                    sticky.update();
                }
            }
        }
    }, 200);
});

// Global initialization
JsSticky.init = function() {
    JsSticky.createInstances();
};

// On document ready
if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', JsSticky.init);
} else {
    JsSticky.init();
}

// Webpack support
if (typeof module !== 'undefined' && typeof module.exports !== 'undefined') {
    module.exports = JsSticky;
}