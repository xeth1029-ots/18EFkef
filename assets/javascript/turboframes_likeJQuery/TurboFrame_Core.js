// 各種觸發事件
;
(function(window, undefined) {

  TurboFrame.extend({

    noConflict: function(deep) {
      if (window.$ === TurboFrame) window.$ = $;
      if (deep && window.TurboFrame === TurboFrame) window.TurboFrame = _TurboFrame;
      return TurboFrame;
    },
    guid: 1,
    cache: {},
    expando: "turboframe" + parseFloat('0.'+crypto.getRandomValues(new Uint32Array(1))[0]).toString(36).substr(2),
    // 獲取數據索引
    getCacheIndex: function(elem, isSet) {
      var id = TurboFrame.expando;

      if (elem.nodeType === 1) {
        return elem[id] || !isSet ? elem[id] : elem[id] = ++TurboFrame.guid;
      }

      return elem.nodeType === 9 ? 1 : 0;
    },

    noop: function() {},
    type: function(obj) {
      return obj == null ? String(obj) : class2type[Array.prototype.toString.call(obj)] || "object";
    },
    isObject: function(obj) {
      return TurboFrame.type(obj) === "object";
    },
    isString: function(obj) {
      return TurboFrame.type(obj) === "string";
    },
    isNumber: function(obj) {
      return TurboFrame.type(obj) === "number";
    },
    isFunction: function(obj) {
      return TurboFrame.type(obj) === "function";
    },
    isArray: Array.isArray || function(obj) {
      return TurboFrame.type(obj) === "array";
    },
    isWindow: function(obj) {
      return obj && typeof obj === 'object' && 'setInterval' in obj;
    },
    isNumeric: function(obj) {
      return !isNaN(parseFloat(obj)) && isFinite(obj);
    },
    isEmptyObject: function(obj) {
      var i;

      for (i in obj) {
        return false;
      }

      return true;
    },
    isPlainObject: function(obj) {
      if (!obj || TurboFrame.type(obj) !== "object" || obj.nodeType || TurboFrame.isWindow(obj)) return false;

      try {
        if (obj.constructor && !Array.prototype.hasOwnProperty.call(obj, "constructor") && !Array.prototype.hasOwnProperty.call(obj.constructor.prototype, "isPrototypeOf")) return false;
      } catch (e) {
        return false;
      }

      var key;

      for (key in obj) {}

      return key === undefined || Array.prototype.hasOwnProperty.call(obj, key);
    },
    each: function(obj, callback) {
      var name,
        i = 0,
        length = obj.length,
        isObj = length === undefined || TurboFrame.isFunction(obj);

      if (isObj) {
        for (name in obj) {
          if (callback.call(obj[name], name, obj[name]) === false) break;
        }
      } else {
        for (; i < length;) {
          if (callback.call(obj[i], i, obj[i++]) === false) break;
        }
      }

      return obj;
    },
    trim: function(text) {
      return text.replace(/^[\s\uFEFF\xA0]+|[\s\uFEFF\xA0]+$/g, "");
    },
    create: function() {
      var arg = arguments,
        obj = document.createElement(arg[0]),
        arr = arg[1] == document ? '' : arg[1];

      if (arr != undefined) {
        arr = arr || {};

        for (var k in arr) {
          if (/A-Za-z]/.test(k)) {
            obj[k] = arr[k];
          } else {
            TurboFrame(obj).attr(k, arr[k]);
          }
        }
      }

      return obj;
    },
    // 讀取/快取數據操作
    readData: function(elem, type, name, value, overwrite) {
      var cache = TurboFrame.cache,
        isRead = typeof value === "undefined" ? true : false,
        index = TurboFrame.getCacheIndex(elem, !isRead);

      if (isRead) {
        return index && cache[index] && cache[index][type] && cache[index][type][name] || undefined;
      }

      cache = cache[index] = cache[index] || {};
      if (!cache[type]) cache[type] = {};
      if (overwrite || typeof cache[type][name] === "undefined") cache[type][name] = value;
      return cache[type][name];
    },
    // 刪除數據操作
    removeData: function(elem, type, name) {
      var data,
        cache = TurboFrame.cache,
        index = TurboFrame.getCacheIndex(elem);

      if (index && (data = cache[index])) {
        if (data[type]) {
          name ? delete data[type][name] : delete data[type];
        }

        if (TurboFrame.isEmptyObject(data[type])) delete data[type];
      }
    },
    // 過濾選擇器
    filter: function(selector, elems) {
      var rQuickExpr = /^(?:#([\w-]+)|(\w+)|\.([\w-]+))$/,
        rTagClass = /^(?:(\w*)\.([\w-]+))$/;
      var match = rQuickExpr.exec(selector),
        m,
        tag,
        i = 0,
        len = elems.length,
        rets = [];

      if (match) {
        // $("#id")
        if (m = match[1]) {
          for (; i < len; i++) {
            if (elems[i].id == m) {
              rets[0] = elems[i];
              break;
            }
          }
        } else if (m = match[2]) {
          for (; i < len; i++) {
            if (elems[i].tagName == m.toUpperCase()) rets.push(elems[i]);
          }
        } else if (m = match[3]) {
          for (; i < len; i++) {
            if (TurboFrame(elems[i]).hasClass(m)) rets.push(elems[i]);
          }
        }
      } else {
        match = rTagClass.exec(selector);

        if (match && (tag = match[1]) && (m = match[2])) {
          for (; i < len; i++) {
            if (TurboFrame(elems[i]).hasClass(m) && elems[i].tagName === tag.toUpperCase()) rets.push(elems[i]);
          }

          return rets;
        }
      }

      return rets;
    },
    // 篩選節點
    dir: function(elem, dir, until) {
      var matched = [],
        cur = elem[dir];

      while (cur && cur.nodeType !== 9 && (until === undefined || cur.nodeType !== 1 || !TurboFrame(cur).is(until))) {
        if (cur.nodeType === 1) matched.push(cur);
        cur = cur[dir];
      }

      return matched;
    },
    sibling: function(n, elem) {
      var rets = [];

      for (; n; n = n.nextSibling) {
        if (n.nodeType === 1 && n !== elem) rets.push(n);
      }

      return rets;
    },
    makeArray: function(array, results) {
      var ret = results || [];

      if (array != null) {
        var type = TurboFrame.type(array);

        if (array.length == null || type === "string" || type === "function" || type === "regexp" || TurboFrame.isWindow(array)) {
          Array.prototype.push.call(ret, array);
        } else {
          TurboFrame.merge(ret, array);
        }
      }

      return ret;
    },
    inArray: function(elem, arr, i) {
      if (arr) {
        if (Array.prototype.indexOf) return Array.prototype.indexOf.call(arr, elem, i);
        var len = arr.length;
        i = i ? i < 0 ? Math.max(0, len + i) : i : 0;

        for (; i < len; i++) {
          if (i in arr && arr[i] === elem) return i;
        }
      }

      return -1;
    },
    // 清除數組中重複的數據
    unique: function(arr) {
      var rets = [],
        i = 0,
        len = arr.length;

      if (TurboFrame.isArray(arr)) {
        for (; i < len; i++) {
          if (TurboFrame.inArray(arr[i], rets) === -1) rets.push(arr[i]);
        }
      }

      return rets;
    },

    error: function(msg) {
      throw new Error(msg);
    },
    merge: function(first, second) {
      var i = first.length,
        j = 0;

      if (typeof second.length === "number") {
        for (var l = second.length; j < l; j++) {
          first[i++] = second[j];
        }
      } else {
        while (second[j] !== undefined) {
          first[i++] = second[j++];
        }
      }

      first.length = i;
      return first;
    },
    map: function(elems, callback, arg) {
      var value,
        key,
        ret = [],
        i = 0,
        length = elems.length,
        isArray = elems instanceof TurboFrame || length !== undefined && typeof length === "number" && (length > 0 && elems[0] && elems[length - 1] || length === 0 || TurboFrame.isArray(elems));

      if (isArray) {
        for (; i < length; i++) {
          value = callback(elems[i], i, arg);
          if (value != null) ret[ret.length] = value;
        }
      } else {
        for (key in elems) {
          value = callback(elems[key], key, arg);
          if (value != null) ret[ret.length] = value;
        }
      }

      return ret.concat.apply([], ret);
    },
    contains: docElem.contains ? function(a, b) {
      var adown = a.nodeType === 9 ? a.documentElement : a,
        bup = b && b.parentNode;
      return a === bup || !!(bup && bup.nodeType === 1 && adown.contains && adown.contains(bup));
    } : docElem.compareDocumentPosition ? function(a, b) {
      return b && !!(a.compareDocumentPosition(b) & 16);
    } : function(a, b) {
      while (b = b.parentNode) {
        if (b === a) return true;
      }

      return false;
    }
  });

})(window);

// import TurboReady from './TurboFrame_Ready.js';
// export function TurboCore() {}

// export {
//     TurboCore as
//     default
// };