// DomReady
;
(function(window, undefined) {
  var readyList = [],
    readyBound = false,
    completed = function(event) {
      if (doc.addEventListener || event.type === "load" || doc.readyState === "complete") {
        readyDetach();
        TurboFrame.ready();
      }
    },
    readyDetach = function() {
      if (doc.addEventListener) {
        doc.removeEventListener("DOMContentLoaded", completed, false);
        window.removeEventListener("load", completed, false);
      } else {
        doc.detachEvent("onreadystatechange", completed);
        window.detachEvent("onload", completed);
      }
    },

    readyAttach = function() {
      var top = false;
      readyBound = true;

      if (doc.readyState === "complete") {
        return TurboFrame.ready();
      } else if (doc.addEventListener) {
        doc.addEventListener("DOMContentLoaded", completed, false);
        window.addEventListener("load", completed, false);
      } else if (doc.attachEvent) {
        doc.attachEvent("onreadystatechange", completed);
        window.attachEvent("onload", completed);

        try {
          top = window.frameElement == null && doc.documentElement;
        } catch (e) {}

        if (top && top.doScroll) {
          (function doScrollCheck() {
            if (!TurboFrame.isReady) {
              try {
                top.doScroll("left");
              } catch (e) {
                return setTimeout(doScrollCheck, 50);
              }

              readyDetach();
              TurboFrame.ready();
            }
          })();
        }
      }
    };

  TurboFrame.fn.extend({

    ready: function(fn) {
      if (TurboFrame.isFunction(fn)) {
        if (TurboFrame.isReady) {
          fn.call(doc, TurboFrame);
        } else if (readyList) {
          readyList.push(fn);
          if (!readyBound) readyAttach();
        }
      }

      return this;
    }
  });

  TurboFrame.extend({
    isReady: false,
    ready: function() {
      var i = 0,
        len = readyList.length;

      if (!TurboFrame.isReady) {
        if (!doc.body) return setTimeout(TurboFrame.ready, 13);
        TurboFrame.isReady = true;

        for (; i < len; i++) {
          readyList[i].call(doc, TurboFrame);
        }

        readyList = [];
      }
    }
  });
})(window);
// import TurboExtend from './TurboFrame_Prototype.js';
// export function TurboReady() {
// }

// export { TurboReady as default };