// Resize
// import TurboFragments from './TurboFrame_Fragments.js';
// export function TurboResize() {}
;
(function(TurboFrame) {


  // resize event
  var resizeHandlers = [];
  var running = false;

  function resize() {
    if (!running) {
      running = true;
      if (window.requestAnimationFrame) {
        window.requestAnimationFrame(TurboFrame(this).runResizeHandlers);
      } else {
        setTimeout(TurboFrame(this).runResizeHandlers, 66);
      }
    }
  }

  // TurboFrame.resize Prototype
  TurboFrame.fn.extend({

    // 需要先調用 TutboFrame.resize_init()
    // TurboFrame.resize_init( fun );
    resize_init: function(elem, settings) {
      var elem = elem || "window";
      if (!resizeHandlers.length) {
        window.addEventListener('resize', TurboFrame.throttle(function(event) {
          console.log(event)
          resize();
        }, 250));
      }
      TurboFrame(elem).resize();
    },

    resize: function() {
      if (typeof(Event) === 'function') {

        window.dispatchEvent(new Event('resize'));
      } else {
        var evt = doc.createEvent('UIEvents');

        evt.initUIEvent('resize', true, false, window, 0);
        window.dispatchEvent(evt);
      }
    },
    // TurboFrame.addResizeHandler( func );
    addResizeHandler: function(callback) {
      if (callback) {
        resizeHandlers.push(callback);
        return true
      }
    },
    // TurboFrame.removeResizeHandler( func );
    removeResizeHandler: function(callback) {
      if (callback) {
        resizeHandlers.pop(callback);
        return true
      }
    },

    runResizeHandlers: function() {
      for (var i = 0; i < resizeHandlers.length; i++) {
        var each = resizeHandlers[i];
        each.call();
      }
      running = false;
    },
  });


})(window.TurboFrame);



// export { TurboResize as default };