;
(function(window, undefined) {



  // TurboFrame Extend 
  TurboFrame.extend = TurboFrame.fn.extend = function() {
    var options, name, src, copy, copyIsArray, clone, target = arguments[0] || {},
      i = 1,
      length = arguments.length,
      deep = false;
    if (typeof target === "boolean") {
      deep = target;
      target = arguments[1] || {};
      i = 2;
    }
    if (typeof target !== "object" && !TurboFrame.isFunction(target)) target = {};
    if (length === i) {
      target = this;
      --i;
    }
    for (; i < length; i++) {
      if ((options = arguments[i]) != null) {
        for (name in options) {
          src = target[name];
          copy = options[name];
          if (target === copy) {
            continue;
          }
          if (deep && copy && (TurboFrame.isPlainObject(copy) || (copyIsArray = TurboFrame.isArray(copy)))) {
            if (copyIsArray) {
              copyIsArray = false;
              clone = src && TurboFrame.isArray(src) ? src : [];
            } else {
              clone = src && TurboFrame.isPlainObject(src) ? src : {};
            }
            target[name] = TurboFrame.extend(deep, clone, copy);
          } else if (copy !== undefined) {
            target[name] = copy;
          }
        }
      }
    }
    return target;
  };

})(window.TurboFrame);
// import TurboDefault from './TurboFrame.js';

// export function TurboExtend() {}

// export { TurboExtend as default };