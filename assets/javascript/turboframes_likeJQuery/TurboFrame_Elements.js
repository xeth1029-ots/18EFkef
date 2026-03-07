;
(function(TurboFrame) {



    // Elements Handles
    TurboFrame.fn.extend({
        // 對應元素讀取或寫入快取數據
        readData: function(name, value) {
            if (typeof value === "undefined") {
                return TurboFrame.readData(this[0], "readData", name);
            }

            return TurboFrame.each(this, function() {
                TurboFrame.readData(this, "readData", name, value, true);
            });
        },
        // 對應元素刪除快取數據
        removeData: function(name) {
            return TurboFrame.each(this, function() {
                TurboFrame.removeData(this, "readData", name);
            });
        },
        hasClass: function(className) {
            var i = 0,
                len = this.length;

            for (; i < len; i++) {
                var elemnode = this[i].nodeType === 1 && (" " + this[i].className + " ").indexOf(" " + className + " ") >= 0;
                if (elemnode) return true;
            }

            return false;
        },
        // 添加 Class
        addClass: function(className) {
            return TurboFrame.each(this, function() {
                if (this.nodeType === 1 && !TurboFrame(this).hasClass(className)) this.className = TurboFrame.trim(this.className + " " + className + " ");
            });
        },
        // 刪除 Class
        removeClass: function(className) {
            return TurboFrame.each(this, function() {
                if (this.nodeType === 1) this.className = TurboFrame.trim((" " + this.className + " ").replace(" " + className + " ", " "));
            });
        },
        // 添加刪除樣式
        toggleClass: function(className, state) {
            return TurboFrame.each(this, function() {
                if (typeof state !== "boolean") state = !TurboFrame(this).hasClass(className);
                state ? TurboFrame(this).addClass(className) : TurboFrame(this).removeClass(className);
            });
        },

        detach: function(selector) {
            return this.remove(selector);
        },

        css: function(name, value) {
            var cssNumber = {
                    'column-count': 1,
                    'columns': 1,
                    'box-flex': 1,
                    'line-clamp': 1,
                    'font-weight': 1,
                    'opacity': 1,
                    'z-index': 1,
                    'zoom': 1
                },
                toLower = function(str) {
                    return str.replace(/::/g, '/').replace(/([A-Z]+)([A-Z][a-z])/g, '$1_$2').replace(/([a-z\d])([A-Z])/g, '$1_$2').replace(/_/g, '-').toLowerCase();
                },
                addPx = function(name, value) {
                    return typeof value == "number" && !cssNumber[toLower(name)] ? value + "px" : value;
                };

            if (typeof name == "string" && typeof value == "string") {
                return TurboFrame.each(this, function() {
                    this.style[toLower(name)] = addPx(name, value);
                });
            } else if (TurboFrame.isString(name) && typeof value === "undefined") {
                if (this.size() == 0) return null;

                var ele = this[0],
                    TurboFrameComputedStyle = function() {
                        var def = doc.defaultView;
                        return new Function("el", "style", ["style.indexOf('-')>-1 && (style=style.replace(/-(\\w)/g,function(m,a){return a.toUpperCase()}));", "style=='float' && (style='", def ? "cssFloat" : "styleFloat", "');return el.style[style] || ", def ? "window.getComputedStyle(el, null)[style]" : "el.currentStyle[style]", " || null;"].join(""));
                    }();

                return TurboFrameComputedStyle(ele, toLower(name));
            } else {
                return TurboFrame.each(this, function() {
                    for (var x in name) this.style[toLower(x)] = addPx(x, name[x]);
                });
            }
        },
        // 顯示元素
        show: function(value) {
            return TurboFrame.each(this, function() {
                var val = value == undefined ? "block" : value;
                TurboFrame(this).css("display", val);
            });
        },
        // 隱藏元素
        hide: function() {
            return TurboFrame.each(this, function() {
                TurboFrame(this).css("display", "none");
            });
        },
        // 獲取當前元素的坐標
        position: function() {
            //沒有對象
            if (this.size() == 0) return null;
            var ele = this[0],
                width = ele.offsetWidth,
                height = ele.offsetHeight,
                top = ele.offsetTop,
                left = ele.offsetLeft;

            while (ele = ele.offsetParent) {
                top += ele.offsetTop;
                left += ele.offsetLeft;
            }

            return {
                width: width,
                height: height,
                top: top,
                left: left
            };
        },
        //獲取當前 doc 元素的坐標
        offset: function() {
            if (this.size() == 0) return null;
            var elem = this[0],
                box = elem.getBoundingClientRect(),
                doc = elem.ownerDocument,
                body = doc.body,
                docElem = doc.documentElement,
                clientTop = docElem.clientTop || body.clientTop || 0,
                clientLeft = docElem.clientLeft || body.clientLeft || 0,
                top = box.top + (self.pageYOffset || docElem.scrollTop) - clientTop,
                left = box.left + (self.pageXOffset || docElem.scrollLeft) - clientLeft;
            return {
                left: left,
                top: top
            };
        },
        //獲取與設置，自訂屬性
        attr: function(name, value) {
            var ret;

            if (typeof value === "undefined") {
                if (this[0] && this[0].nodeType === 1) {
                    ret = this[0].getAttribute(name);
                } // 屬性不存在，返回undefined


                return ret == null ? undefined : ret;
            } // value設置為null也是刪除屬性


            if (value === null) return this.removeAttr(name); // 判斷是否bool屬性

            if (rBoolean.test(name)) {
                if (value === false) {
                    return this.removeAttr(name);
                } else {
                    value = name;
                }
            }

            return TurboFrame.each(this, function() {
                if (this.nodeType === 1) this.setAttribute(name, value);
            });
        },
        // 刪除元素指定 Attribute 屬性
        removeAttr: function(name) {
            return TurboFrame.each(this, function() {
                if (this.nodeType === 1) {
                    // 如果是布爾屬性，順便把屬性置false
                    if (rBoolean.test(name)) {
                        this[propFix[name] || name] = false;
                    }

                    this.removeAttribute(name);
                }
            });
        },
        data: function(name, value) {
            var attrName = "data-" + name.replace(/([A-Z])/g, "-$1").toLowerCase();
            var data = 1 in arguments ? this.attr(attrName, value) : this.attr(attrName);

            var lizeValue = function(value) {
                try {
                    return value ? value == "true" || (value == "false" ? false : value == "null" ? null : +value + "" == value ? +value : /^[\[\{]/.test(value) ? JSON.parse(value) : value) : value;
                } catch (e) {
                    return value;
                }
            };

            return data !== null ? lizeValue(data) : undefined;
        },
        // 讀取或設置元素 Property 屬性處理
        prop: function(name, value) {
            name = propFix[name] || name;
            if (typeof value === "undefined") return this[0][name];
            return TurboFrame.each(this, function() {
                this[name] = value;
            });
        },
        // 刪除元素指定 Property 屬性
        removeProp: function(name) {
            name = propFix[name] || name;
            return TurboFrame.each(this, function() {
                delete this[name];
            });
        },
        // 獲取所有子節點
        contents: function() {
            var elems,
                i = 0,
                len = this.length,
                rets = [];

            for (; i < len; i++) {
                rets = TurboFrame.merge(rets, this[i].childNodes);
            } // 排重


            rets = TurboFrame.unique(rets);
            return this.pushStack(rets);
        },
        // 讀取設置節點內容
        html: function(value) {
            return typeof value === "undefined" ? this[0] && this[0].nodeType === 1 ? this[0].innerHTML : undefined : typeof value !== "undefined" && value == true ? this[0] && this[0].nodeType === 1 ? this[0].outerHTML : undefined : TurboFrame.each(this, function() {
                this.innerHTML = value;
            });
        },
        // 讀取設置節點文本內容
        text: function(value) {
            var innText = doc.all ? "innerText" : "textContent";
            return typeof value === "undefined" ? this[0] && this[0].nodeType === 1 ? this[0][innText] : undefined : TurboFrame.each(this, function() {
                this[innText] = value;
            });
        },
        // 讀取設置表單元素的值
        val: function(value) {
            if (typeof value === "undefined") {
                return this[0] && this[0].nodeType === 1 && typeof this[0].value !== "undefined" ? this[0].value : undefined;
            }

            // 將 value 轉化為 string
            value = value == null ? "" : value + "";
            return TurboFrame.each(this, function() {
                if (typeof this.value !== "undefined") {
                    this.value = value;
                }
            });
        },
        empty: function() {
            return this.html("");
        }
    });


})(window.TurboFrame);

// import TurboFunctions from './TurboFrame_Functions.js';
// export function TurboElements() {}

// export { TurboElements as default };