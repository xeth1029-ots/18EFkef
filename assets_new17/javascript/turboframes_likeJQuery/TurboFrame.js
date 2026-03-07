/**
 * @ TURBOFRAME JAVASCRIPT LIBRARY
 * @
 * @ Author   BarrY
 * @ Version  1.0.2
 *  @  "_"     內置方法不外用
 */



;
var doc = document || window.document;
var docElem = doc.documentElement;
var class2type = {};
(function(window, undefined) {
  var rootTurboFrame;
  var TurboFrame = window.TurboFrame;
  var $ = window.$;
  var _ClassNameRegex = /^(?:([\w-#\.]+)([\s]?)([\w-#\.\s>]*))$/;

  // 選擇器 $(" SELECTOR ")
  // 核心概念：將 Select 全部丟到一個新組建的 Array
  // 除了 [data] 以外都使用原生 getElement Select .

  var _QuerySelector = function() {
    var __snack =
      /(?:[\*\w\-\\.#]+)+(?:\[(?:[\w\-_][^=]+)=(?:[\'\[\]\w\-_]+)\])*|\*|>/gi,
      __exprClassName = /^(?:[\w\-_]+)?\.([\w\-_]+)/,
      __exprId = /^(?:[\w\-_]+)?#([\w\-_]+)/,
      __exprNodeName = /^([\w\*\-_]+)/,
      __na = [null, null, null];
    var __exprAttr = /\[([\w\-_][^=]+)=([\'\[\]\w\-_]+)\]/;
    var __exprAttrArr = /\[([\w\-_][^=]+)=(([\w\-_]+)\{([\w\-_]+)\})\]/;

    function retSelector(selector, context) {

      context = context || doc;

      var simple = /^[\w\-_#]+$/.test(selector);


      if (!simple && context.querySelectorAll)
        return realArray(context.querySelectorAll(selector));

      if (selector.indexOf(",") > -1) {
        var split = selector.split(/,/g),
          ret = [],
          sIndex = 0,
          len = split.length;

        for (; sIndex < len; ++sIndex) {
          ret = ret.concat(retSelector(split[sIndex], context));
        }

        return uniq(ret);
      }

      var parts = selector.match(__snack),
        part = parts.pop(),
        id = (part.match(__exprId) || __na)[1],
        className = !id && (part.match(__exprClassName) || __na)[1],
        nodeName = !id && (part.match(__exprNodeName) || __na)[1],
        collection;
      var attrs = selector.match(/\[(?:[\w\-_][^=]+)=(?:[\'\[\]\w\-_]+)\]/g);

      if (className && !attrs && !nodeName && context.getElementsByClassName) {
        collection = realArray(context.getElementsByClassName(className));
      } else {
        collection = !id && realArray(context.getElementsByTagName(nodeName || "*"));
        if (className)
          collection = filterByAttr(
            collection,
            "className",
            RegExp("(^|\\s)" + className + "(\\s|$)")
          );

        if (id) {
          var byId = context.getElementById(id);
          return byId ? [byId] : [];
        }

        if (attrs) {
          for (var x = 0; x < attrs.length; x++) {
            var atNode = (attrs[x].match(__exprAttr) || na)[1];
            var atValue = (attrs[x].match(__exprAttr) || na)[2];
            atValue = atValue
              .replace(/\'/g, "")
              .replace(/\-/g, "\\-")
              .replace(/\[/g, "\\[")
              .replace(/\]/g, "\\]");
            collection = filterByAttr(
              collection,
              atNode,
              RegExp("(^" + atValue + "$)")
            );
          }
        }
      }

      return parts[0] && collection[0] ?
        filterParents(parts, collection) :
        collection;
    }

    function realArray(arg) {
      try {
        return Array.prototype.slice.call(arg);
      } catch (event) {
        var ret = [],
          i = 0,
          len = arg.length;

        for (; i < len; ++i) {
          ret[i] = arg[i];
        }

        return ret;
      }
    }

    function filterParents(selectorParts, collection, direct) {
      var parentSelector = selectorParts.pop();

      if (parentSelector === ">") {
        return filterParents(selectorParts, collection, true);
      }

      var ret = [],
        r = -1,
        id = (parentSelector.match(__exprId) || na)[1],
        className = !id && (parentSelector.match(__exprClassName) || na)[1],
        nodeName = !id && (parentSelector.match(__exprNodeName) || na)[1],
        cIndex = -1,
        node,
        parent,
        matches;
      nodeName = nodeName && nodeName.toLowerCase();

      while ((node = collection[++cIndex])) {
        parent = node.parentNode;

        do {
          matches = !nodeName ||
            nodeName === "*" ||
            nodeName === parent.nodeName.toLowerCase();
          matches = matches && (!id || parent.id === id);
          matches =
            matches &&
            (!className ||
              RegExp("(^|\\s)" + className + "(\\s|$)").test(parent.className));

          if (direct || matches) {
            break;
          }
        } while ((parent = parent.parentNode));

        if (matches) ret[++r] = node;
      }

      return selectorParts[0] && ret[0] ?
        filterParents(selectorParts, ret) :
        ret;
    }

    var uniq = function() {
      var uid = +new Date();

      var data = function() {
        var n = 1;
        return function(elem) {
          var cacheIndex = elem[uid],
            nextCacheIndex = n++;

          if (!cacheIndex) {
            elem[uid] = nextCacheIndex;
            return true;
          }

          return false;
        };
      }();

      return function(arr) {
        var length = arr.length,
          ret = [],
          r = -1,
          i = 0,
          item;

        for (; i < length; ++i) {
          item = arr[i];
          if (data(item)) ret[++r] = item;
        }

        uid += 1;
        return ret;
      };
    }();

    function filterByAttr(collection, attr, regex) {
      var i = -1,
        node,
        r = -1,
        ret = [];

      while ((node = collection[++i])) {
        if (regex.test(node.getAttribute(attr))) ret[++r] = node;
      }

      return ret;
    }

    return retSelector;
  }();


  // ** 建構 TurboFrame new TurboFrame {};
  var TurboFrame = function(selector, context) {
    return new TurboFrame.fn.init(selector, context);
  };

  // window.TurboFrame || $ -->> TurboFrame.fn || $.fn  -->>
  // $("selector")
  TurboFrame.fn = TurboFrame.prototype = {
    init: function(selector, context) {
      var _self = this;
      context = context || doc;
      _self.length = 0;
      if (typeof selector === "undefined") {
        return _self;
      } else if (selector.nodeType) {
        _self.context = _self[0] = selector;
        _self.length = 1;
        return _self;
      } else if (selector === "body" && !context && doc.body) {
        _self.context = doc;
        _self[0] = doc.body;
        _self.selector = selector;
        _self.length = 1;
        return _self;
      } else if (typeof selector === "string") {
        selector = TurboFrame.trim(selector);
        // 如果是 Html 片段
        if (selector[0] == "<" && fragmentRE.test(selector)) {
          var dom = TurboFrame.fragmentNode(selector, RegExp.$1, context),
            selector = null;
          return TurboFrame(_self).pushStack(dom);
        } else {
          context && context.nodeType === 1 ?
            (_self.context = context) :
            (context = doc);

          var selSize = _QuerySelector(selector, context),
            dom = [];

          for (var i = 0; i < selSize.length; i++) {
            dom[i] = selSize[i];
          };
          // 父集為多個節點時需要排重
          if (selSize.length > 1 && dom[1]) dom = TurboFrame.unique(dom);
          return TurboFrame(_self).pushStack(dom);
        }
      } else if (TurboFrame.isFunction(selector)) {
        return rootTurboFrame.ready(selector);
      }
      return TurboFrame.makeArray(selector, _self);

      // return _self;
    },
    constructor: TurboFrame,
    selector: "",
    turboframeVer: "1.0.2",
    length: 0,
    size: function() {
      return this.length;
    },
    toArray: function() {
      try {
        return Array.prototype.slice.call(this, 0);
      } catch (e) {
        var arr = [];

        for (var i = 0, len = this.length; i < len; i++) {
          //  arr.push(s[i]);
          arr[i] = this[i];
        }

        return arr;
      }
    },
    get: function(num) {
      return num == null ?
        this.toArray() :
        num < 0 ?
        this[this.length + num] :
        this[num];
    },
    pushStack: function(elems) {
      var obj = TurboFrame(),
        i = 0,
        len = elems.length;

      for (; i < len; i++) {
        obj[i] = elems[i];
      }

      obj.length = len;
      return obj;
    },
    each: function(callback) {
      return TurboFrame.each(this, callback);
    },

    eq: function(i) {
      i = +i;
      return i === -1 ? this.slice(i) : this.slice(i, i + 1);
    },
    find: function(selector) {
      var match,
        i = 0,
        obj,
        len = this.length,
        rets = [];
      match = _ClassNameRegex.exec(selector);

      if (match && match[1]) {
        for (; i < len; i++)
          rets = TurboFrame.merge(rets, _QuerySelector(match[1], this[i]));
      } // 父集為多個節點時需要排重

      if (len > 1 && rets[1]) rets = TurboFrame.unique(rets);
      obj = this.pushStack(rets);
      // 還有後代節點繼續遍歷

      if (obj.length > 0 && match[2] && match[3]) {
        return obj.find(match[3]);
      }

      return obj;
    },
    first: function() {
      return this.eq(0);
    },
    last: function() {
      return this.eq(-1);
    },
    end: function() {
      return this.prevObject || this.constructor(null);
    },
    map: function(callback) {
      return this.pushStack(
        TurboFrame.map(this, function(elem, i) {
          return callback.call(elem, i, elem);
        })
      );
    },
    is: function(selector) {
      var POS = /:(nth|eq|gt|lt|first|last|even|odd)(?:\((\d*)\))?(?=[^\-]|$)/;
      return (!!selector &&
        (typeof selector === "string" ?
          POS.test(selector) ?
          TurboFrame(selector, this.context).index(this[0]) >= 0 :
          TurboFrame.filter(selector, this).length > 0 :
          this.pushStack(selector).length > 0)
      );
    },
    index: function(elem) {
      if (!elem) {
        return this[0] && this[0].parentNode ? this.prevAll().length : -1;
      }

      if (typeof elem === "string") {
        return TurboFrame.inArray(this[0], TurboFrame(elem));
      }

      return TurboFrame.inArray(elem.turboframe ? elem[0] : elem, this);
    },
    // 刪除節點
    remove: function() {
      var len = this.length;

      var removes = function(elem) {
        var parent = elem.parentNode;
        if (parent && parent.nodeType !== 11) parent.removeChild(elem);
      };

      return this.each(function() {
        removes(this);
      });
    },
    slice: function() {
      return this.pushStack(Array.prototype.slice.apply(this, arguments));
    },
    push: Array.prototype.push,
    sort: [].sort,
    splice: [].splice
  };

  TurboFrame.fn.init.prototype = TurboFrame.fn;

  rootTurboFrame = TurboFrame(doc);

  "function" === typeof define
    ?
    define("TurboFrame", [], function() {
      return TurboFrame;
    }) :
    "object" === typeof module && "object" === typeof module.exports ?
    (module.exports = TurboFrame) :
    (window.TurboFrame = window.$ = $ = TurboFrame);

})(window);
// export function TurboDefault() {}

// export { TurboDefault as default };