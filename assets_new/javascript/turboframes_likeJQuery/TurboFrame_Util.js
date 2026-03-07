// // Utils
// import TurboBrowser from './TurboFrame_Browser.js';
// export function TurboUtil() {

// }
// export { TurboUtil as default };
;
(function(TurboFrame) {


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

    // 節點操作
    var _domUntil = /domUntil$/,
        rparentsprev = /^(?:parents|prev(?:domUntil|All))/,
        guaranteedUnique = {
            children: true,
            contents: true,
            next: true,
            prev: true
        };
    var _ClassNameRegex = /^(?:([\w-#\.]+)([\s]?)([\w-#\.\s>]*))$/;

    function sibling(cur, dir) {
        do {
            cur = cur[dir];
        } while (cur && cur.nodeType !== 1);

        return cur;
    }

    // 節點操作，綁定 extend_fn()

    TurboFrame.each({
        // 獲取元素父節點
        parent: function(elem) {
            var parent = elem.parentNode;
            return parent && parent.nodeType !== 11 ? parent : null;
        },
        // 獲取元素匹配的上級節點
        parents: function(elem) {
            return TurboFrame.dir(elem, "parentNode");
        },
        parentsdomUntil: function(elem, i, domUntil) {
            return TurboFrame.dir(elem, "parentNode", domUntil);
        },
        // 返回元素之後第一個兄弟節點
        next: function(elem) {
            return sibling(elem, "nextSibling");
        },
        // 返回元素之前第一個兄弟節點
        prev: function(elem) {
            return sibling(elem, "previousSibling");
        },
        // 返回元素之後所有兄弟節點
        nextAll: function(elem) {
            return TurboFrame.dir(elem, "nextSibling");
        },
        // 返回元素之前所有兄弟節點
        prevAll: function(elem) {
            return TurboFrame.dir(elem, "previousSibling");
        },

        nextdomUntil: function(elem, i, domUntil) {
            return TurboFrame.dir(elem, "nextSibling", domUntil);
        },

        prevdomUntil: function(elem, i, domUntil) {
            return TurboFrame.dir(elem, "previousSibling", domUntil);
        },

        // 返回除自身以外所有兄弟節點
        siblings: function(elem) {
            return TurboFrame.sibling((elem.parentNode || {}).firstChild, elem);
        },

        // 獲取指定子元素
        children: function(elem) {
            return TurboFrame.sibling(elem.firstChild);
        },

    }, extendNode);

    function extendNode(name, fn) {
        TurboFrame.fn[name] = function(domUntil, selector) {
            var ret = TurboFrame.map(this, fn, domUntil);
            if (!_domUntil.test(name)) selector = domUntil;
            if (selector && typeof selector === "string") ret = TurboFrame.filter(selector, ret);
            ret = this.length > 1 && !guaranteedUnique[name] ? TurboFrame.unique(ret) : ret;

            if (this.length > 1 && rparentsprev.test(name)) {
                ret = ret.reverse();
            }

            return this.pushStack(ret);
        };
    }

    // Dom Manipulate
    TurboFrame.fn.extend({
        domManip: function(args, callback) {
            var value = args[0],
                i = 0,
                len = this.length,
                j = 0,
                vlen = 0;

            if (len && value != undefined) {
                if (TurboFrame.type(value) === "string") {
                    nodes = TurboFrame.parseNodes(value);
                    value = TurboFrame.buildFragment(nodes);
                }

                for (; i < len; i++) {
                    if (value instanceof TurboFrame) {
                        vlen = value.length;

                        for (; j < vlen; j++) {
                            callback.call(this[i], value[j]);
                        }
                    } else {
                        var fragment = doc.createDocumentFragment();
                        fragment.appendChild(value);
                        callback.call(this[i], fragment);
                    }
                }
            }

            return this;
        },
        // 在元素裡面內容的末尾插入內容
        append: function() {
            return this.domManip(arguments, function(elem) {
                if (this.nodeType === 1) {
                    this.appendChild(elem);
                }
            });
        },
        // 在元素裡面內容的前面插入內容
        prepend: function() {
            return this.domManip(arguments, function(elem) {
                if (this.nodeType === 1) {
                    this.insertBefore(elem, this.firstChild);
                }
            });
        },
        // 在元素之前插入內容
        before: function() {
            return this.domManip(arguments, function(elem) {
                if (this.parentNode) this.parentNode.insertBefore(elem, this);
            });
        },
        // 在元素之後插入內容
        after: function() {
            return this.domManip(arguments, function(elem) {
                if (this.parentNode) this.parentNode.insertBefore(elem, this.nextSibling);
            });
        },
        replaceWith: function(value) {
            return this.before(value).remove();
        },
        wrapAll: function(html) {
            var num = TurboFrame.expando,
                thisHtml = "",
                retHtml = [];
            this.eq(0).before(html);
            this.each(function() {
                thisHtml = TurboFrame(this).html(true);
                retHtml.push(thisHtml);
                TurboFrame(this).addClass("wrapall" + num);
            });
            this.eq(0).prev().html(retHtml.join(" "));
            TurboFrame(".wrapall" + num).remove();
            return this;
        },
        unwrap: function() {
            return this.parent().each(function() {
                if (!(this.nodeName && this.nodeName.toLowerCase() === "body".toLowerCase())) {
                    TurboFrame(this).replaceWith(TurboFrame(this).html());
                }
            });
        },
        wrap: function(html) {
            var num = TurboFrame.expando;
            this.each(function() {
                var thisHtml = TurboFrame(this).html(true);
                TurboFrame(this).after(html).addClass("wrap" + num);
                TurboFrame(this).next().append(thisHtml);
            });
            TurboFrame(".wrap" + num).remove();
            return this;
        }
    });

    TurboFrame.each(["width", "height"], function(i, name) {
        TurboFrame.fn[name] = function(value) {
            if (value == undefined) {
                return getWidthOrHeight(this, name);
            } else {
                return this.each(function() {
                    TurboFrame(this).css(name, typeof value === "number" ? value + "px" : value);
                });
            }
        };
    });

    // // 將樣式屬性轉為駝峰式
    // function camelCase(name) {
    //   return name.replace(/\-(\w)/g, function(all, letter) {
    //     return letter.toUpperCase();
    //   });
    // }

    // 寬高屬性單位 auto 轉化
    function getWidthOrHeight(elem, name) {
        var ret = "";

        if (TurboFrame.isWindow(elem[0])) {
            ret = elem[0].docElem[TurboFrame.camelCase("client-" + name)] || elem[0].doc.body[TurboFrame.camelCase("client-" + name)];
        } else {
            var padding = name === "width" ? ["left", "right"] : ["top", "bottom"],
                ret = elem[0][TurboFrame.camelCase("offset-" + name)];

            if (ret <= 0 || ret == null) {
                ret = parseFloat(elem[0][TurboFrame.camelCase("client-" + name)]) - parseFloat(TurboFrame(elem).css("padding-" + padding[0])) - parseFloat(TurboFrame(elem).css("padding-" + padding[1]));
            }
        }

        return ret;
    }
})(window.TurboFrame);