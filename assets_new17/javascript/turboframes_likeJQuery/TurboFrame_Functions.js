// 防抖 ＆ 節流 
;
(function(TurboFrame) {

  // TutboFrame.throttle(function() {  }, 250 ));
  TurboFrame.extend({

    throttle: function(callback, delay) {
      var delay = arguments.length > 1 && arguments[1] !== undefined ? arguments[1] : 250;
      var timer = null;
      var timeStamp = new Date();
      return function() {
        console.log("throttle");
        var _this = this;
        var args = arguments;
        console.log(_this)
        if (new Date() - timeStamp > delay) {
          timeStamp = new Date();

          timer = setTimeout(function() {
            callback.apply(_this, args);
          }, delay);
        }
      };
    },

    debounce: function(callback, delay) {
      var delay = arguments.length > 1 && arguments[1] !== undefined ? arguments[1] : 500;
      var timer;
      return function() {
        var _this = this;
        for (var _len = arguments.length, args = new Array(_len), _key = 0; _key < _len; _key++) {
          args[_key] = arguments[_key];
        }
        if (timer) {
          clearTimeout(timer);
        }
        timer = setTimeout(function() {
          callback.apply(_this, args);
        }, delay);
      };
    },

  });


})(window.TurboFrame);