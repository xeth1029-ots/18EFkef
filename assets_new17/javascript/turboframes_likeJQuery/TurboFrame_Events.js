//各種觸發事件

;
(function(window, undefined) {

    TurboFrame.each("Boolean Number String Function Array Object".split(" "), function(i, name) {
        class2type["[object " + name + "]"] = name.toLowerCase();
    });

    var rBoolean = /^(?:checked|selected|async|autofocus|autoplay|controls|defer|disabled|hidden|ismap|loop|multiple|open|readonly|required|scoped)$/i,
        propFix = {
            "for": "htmlFor",
            "class": "className",
            "readonly": 'readOnly',
            "maxlength": 'maxLength',
            "cellspacing": 'cellSpacing',
            "rowspan": 'rowSpan',
            "colspan": 'colSpan',
            "tabindex": 'tabIndex',
            "usemap": 'useMap',
            "frameborder": 'frameBorder'
        };

    TurboFrame.each("tabIndex readOnly maxLength cellSpacing cellPadding rowSpan colSpan useMap frameBorder contentEditable".split(" "), function() {
        propFix[this.toLowerCase()] = this;
    });

    var fragmentRE = /^\s*<(\w+|!)[^>]*>/,
        singleTagRE = /^<(\w+)\s*\/?>(?:<\/\1>|)$/,
        tagExpanderRE = /<(?!area|br|col|embed|hr|img|input|link|meta|param)(([\w:]+)[^>]*)\/>/ig,
        table = document.createElement('table'),
        tableRow = document.createElement('tr'),
        containers = {
            'tr': document.createElement('tbody'),
            'tbody': table,
            'thead': table,
            'tfoot': table,
            'td': tableRow,
            'th': tableRow,
            '*': document.createElement('div')
        };
    var handlers = {},
        _jid = 1;
    /* 綁定事件 start */
    TurboFrame.event = {
        add: addEvent,
        remove: removeEvent
    };

    TurboFrame.fn.extend({
        on: function(event, selector, data, callback) {
            var self = this;
            if (event && !TurboFrame.isString(event)) {
                TurboFrame.each(event, function(type, fn) {
                    self.on(type, selector, data, fn);
                });
                return self;
            }
            if (!TurboFrame.isString(selector) && !TurboFrame.isFunction(callback) && callback !== false) callback = data,
                data = selector, selector = undefined;
            if (TurboFrame.isFunction(data) || data === false) callback = data, data = undefined;
            if (callback === false) callback = function() {
                return false;
            };
            return this.each(function() {
                addEvent(this, event, callback, data, selector);
            });
        },
        off: function(event, selector, callback) {
            var self = this;
            if (event && !TurboFrame.isString(event)) {
                TurboFrame.each(event, function(type, fn) {
                    self.off(type, selector, fn);
                });
                return self;
            }
            if (!TurboFrame.isString(selector) && !TurboFrame.isFunction(callback) && callback !== false) callback = selector,
                selector = undefined;
            if (callback === false) callback = function() {
                return false;
            };
            return self.each(function() {
                removeEvent(this, event, callback, selector);
            });
        },
        bind: function(event, func) {
            return this.each(function() {
                addEvent(this, event, func);
            });
        },
        unbind: function(event, func) {
            return this.each(function() {
                removeEvent(this, event, func);
            });
        },
        hover: function(fnOver, fnOut) {
            return TurboFrame.each(this, function() {
                TurboFrame(this).on("mouseover", fnOver);
                TurboFrame(this).on("mouseout", fnOut);
            });
        },
        delegate: function(selector, event, callback) {
            return this.on(event, selector, callback);
        },
        trigger: function(event, data) {
            var type = event,
                specialEvents = {};
            specialEvents.click = specialEvents.mousedown = specialEvents.mouseup = specialEvents.mousemove = "MouseEvents";
            if (typeof type == "string") {
                event = doc.createEvent(specialEvents[type] || "Events");
                event.initEvent(type, true, true);
            } else return;
            event._data = data;
            return this.each(function() {
                if ("dispatchEvent" in this) this.dispatchEvent(event);
            });
        }
    });

    function addEvent(element, events, func, data, selector) {
        var self = this,
            id = jid(element),
            set = handlers[id] || (handlers[id] = []);
        console.log(events)
        TurboFrame.each(events.split(/\s/), function(i, event) {
            var handler = TurboFrame.extend(parse(event), {
                fn: func,
                sel: selector,
                i: set.length
            });
            var proxyfn = handler.proxy = function(e) {
                //處理事件代理
                if (selector) {
                    var $temp = TurboFrame(element).find(selector);
                    var res = [].some.call($temp, function(val) {
                        return val === e.target || TurboFrame.contains(val, e.target);
                    });
                    //不包含
                    if (!res) {
                        return false;
                    }
                }
                e.data = data;
                var result = func.apply(element, e._data == undefined ? [e] : [e].concat(e._data));
                if (result === false) e.preventDefault(), e.stopPropagation();
                return result;
            };
            set.push(handler);
            //if (element.addEventListener) element.addEventListener(handler.e, proxyfn, false);
            if (element.addEventListener) {
                element.addEventListener(handler.e, proxyfn, false);
            } else if (element.attachEvent) {
                element.attachEvent("on" + handler.e, proxyfn);
            } else {
                element["on" + handler.e] = proxyfn;
            }
        });
    }

    function removeEvent(element, events, func, selector) {
        TurboFrame.each((events || "").split(/\s/), function(i, event) {
            TurboFrame.event = parse(event);
            TurboFrame.each(findHandlers(element, event, func, selector), function(f, handler) {
                delete handlers[jid(element)][handler.i];
                //if (element.removeEventListener) element.removeEventListener(handler.e, handler.proxy, false);
                if (element.removeEventListener) {
                    element.removeEventListener(handler.e, handler.proxy, false);
                } else if (element.detachEvent) {
                    element.detachEvent("on" + handler.e, handler.proxy);
                } else {
                    element["on" + handler.e] = null;
                }
            });
        });
    }

    function jid(element) {
        return element._jid || (element._jid = _jid++);
    }

    function parse(event) {
        var parts = ("" + event).split(".");
        return {
            e: parts[0],
            ns: parts.slice(1).sort().join(" ")
        };
    }

    function findHandlers(element, event, func, selector) {
        var self = this,
            id = jid(element);
        event = parse(event);
        return (handlers[jid(element)] || []).filter(function(handler) {
            return handler && (!event.e || handler.e == event.e) && (!func || handler.fn.toString() === func.toString()) && (!selector || handler.sel == selector);
        });
    }

    var addTouchs = TurboFrame.browser === 'webkit' || TurboFrame.browser === 'safari' ? "touchstart touchmove touchend" : "";

    var eventType = ("blur focus focusin focusout load resize scroll unload click dblclick " + "mousedown mouseup mousemove mouseover mouseout mouseenter mouseleave " + "change select submit keydown keypress keyup error paste drop dragstart dragover " + "beforeunload" + addTouchs).split(" ");

    TurboFrame.each(eventType, function(i, event) {
        TurboFrame.fn[event] = function(callback) {
            return callback ? this.bind(event, callback) : this.trigger(event);
        };
    });

})(window);