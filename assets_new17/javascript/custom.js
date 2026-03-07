  // JsYoutubeThumb API Youtube 截圖
  const JsYoutubeThumb = (function() {

    var video, results;

    return {

      init: function(elems, size) {
        return JsYoutubeThumb.getThumb(elems, size);

      },

      getThumb: function(elems, size, url) {
        var url = document.querySelectorAll(elems)[0].getAttribute('href');

        if (url === null) {
          return ''
        }
        size = (size === null) ? 'big' : size;
        results = url.match('[\\?&]v=([^&#]*)');
        video = (results === null) ? url : results[1];

        if (size === 'small') {
          return JsYoutubeThumb.setThumb('http://img.youtube.com/vi/' + video + '/2.jpg', elems)
        }
        return JsYoutubeThumb.setThumb('http://img.youtube.com/vi/' + video + '/0.jpg', elems)
      },

      setThumb: function(url, elems) {
        return JsUtils.each(document.querySelectorAll(elems), function(event) {
          event.children[0].src = url;
        })
      }
    };
  }());

  // Collects 收藏按鈕
  const CoreCollects = function CoreCollects(elements) {
    const collects = document.querySelectorAll(elements);
    return JsUtils.each(collects, function(element) {
      element.addEventListener('click', function(event) {
        event.preventDefault();
        console.log(event.target)
        console.log(event.target.nodeName)
        if (event.target.nodeName === 'BUTTON' || event.target.nodeName === 'A') {
          if (!event.target.classList.contains('active')) {
            event.target.classList.add('active');
            JsToast('收藏成功！', 'success', 1500);
          } else {
            event.target.classList.remove('active');
            JsToast('已取消收藏', 'error', 1500);
          }
        }
      });
    });
  };

  // Separated 數字三位一撇
  const CoreSeparated = function CoreSeparated(elements) {
    const nums = document.querySelectorAll(elements);
    return JsUtils.each(nums, function(element) {
      let newVal = JsUtils.numOfComma(element.textContent);
      element.innerHTML = newVal;
    });
  };

  // Toast 吐司訊息
  // JsToast('成功訊息～', 'success', 1500);
  // JsToast(); --> 預設執行是載入中 Loading ... 
  const JsToast = function(txt, type, duration) {
    ///////////////////////////////
    // **  Private variables  ** //
    ///////////////////////////////
    const that = this;
    const body = document.getElementsByTagName("BODY")[0];
    let timer;
    // Default Options
    var defaultOptions = {
      txt: "加載中 ...",
      type: "default",
      duration: 3000
    };
    duration = isNaN(duration) ? defaultOptions.duration : duration;
    txt = txt || defaultOptions.txt;
    type = type || defaultOptions.type;

    JsUtils.throttleTime(timer, function() {
      var toast = document.createElement("div");
      toast.id = "toast";
      var toast_id = document.getElementById("toast");

      if (toast_id) toast_id.remove();

      let successIcon =
        '<path d="M22 12A10 10 0 1 1 12 2a10 10 0 0 1 10 10Z" opacity=".4"/>' +
        '<path d="M11 16a1 1 0 0 1-1-1l-3-2a1 1 0 0 1 1-2l3 3 5-5a1 1 0 0 1 1 1l-6 5a1 1 0 0 1 0 1Z"/>';
      let errorIcon =
        '<path d="M15 2H9a3 3 0 0 0-2 1L3 7a3 3 0 0 0-1 2v6a3 3 0 0 0 1 2l4 4a3 3 0 0 0 2 1h6a3 3 0 0 0 2-1l4-4a3 3 0 0 0 1-2V9a3 3 0 0 0-1-2l-4-4a3 3 0 0 0-2-1Z" opacity=".4"/>' +
        '<path d="m13 12 3-3a1 1 0 0 0-1-1l-3 3-3-3a1 1 0 0 0-1 1l3 3-3 3a1 1 0 0 0 0 1 1 1 0 0 0 1 0l3-3 3 3a1 1 0 0 0 1 0 1 1 0 0 0 0-1Z"/>';
      let infoIcon =
        '<path d="M2 13V7a5 5 0 0 1 5-5h10a5 5 0 0 1 5 5v7a5 5 0 0 1-5 5h-1a1 1 0 0 0-1 0l-2 2a1 1 0 0 1-2 0l-2-2H7a5 5 0 0 1-5-5Z" opacity=".4"/>' +
        '<path d="M15 11a1 1 0 0 1 1-1 1 1 0 0 1 1 1 1 1 0 0 1-1 1 1 1 0 0 1-1-1Zm-4 0a1 1 0 0 1 1-1 1 1 0 0 1 1 1 1 1 0 0 1-1 1 1 1 0 0 1-1-1Zm-4 0a1 1 0 0 1 1-1 1 1 0 0 1 1 1 1 1 0 0 1-1 1 1 1 0 0 1-1-1Z"/>';
      let warnIcon =
        '<path d="M11 2a2 2 0 0 1 2 0l2 2a2 2 0 0 0 1 0h2a2 2 0 0 1 2 2v2a2 2 0 0 0 0 1l2 2a2 2 0 0 1 0 2l-2 2a2 2 0 0 0 0 1v2a2 2 0 0 1-2 2h-2a2 2 0 0 0-1 0l-2 2a2 2 0 0 1-2 0l-2-2a2 2 0 0 0-1 0H6a2 2 0 0 1-2-2v-2a2 2 0 0 0 0-1l-2-2a2 2 0 0 1 0-2l2-2a2 2 0 0 0 0-1V6a2 2 0 0 1 2-2h2a2 2 0 0 0 1 0Z" opacity=".4"/>' +
        '<path d="M11 16a1 1 0 0 1 1-1 1 1 0 0 1 1 1 1 1 0 0 1-1 1 1 1 0 0 1-1-1Zm0-3V8a1 1 0 0 1 1-1 1 1 0 0 1 1 1v5a1 1 0 0 1-1 1 1 1 0 0 1-1-1Z"/>';
      var helpIcon =
        '<path d="M17 18h-4l-4 3a1 1 0 0 1-2 0v-3a5 5 0 0 1-5-5V7a5 5 0 0 1 5-5h10a5 5 0 0 1 5 5v6a5 5 0 0 1-5 5Z" opacity=".4"/>' +
        '<path d="M11 14a1 1 0 0 1 1-1 1 1 0 0 1 1 1 1 1 0 0 1-1 1 1 1 0 0 1-1-1Zm0-3a2 2 0 0 1 1-2h1a1 1 0 0 0-1-1 1 1 0 0 0-1 1 1 1 0 0 1-1 0 2 2 0 0 1 2-3 2 2 0 0 1 2 3 2 2 0 0 1-1 1v1a1 1 0 0 1-1 1 1 1 0 0 1-1-1Z"/>';
      let load =
        '<path d="M20 5a15 15 0 1 0 0 30 15 15 0 0 0 0-30zm0 27a12 12 0 1 1 0-24 12 12 0 0 1 0 24z" opacity=".24"/>' +
        '<path d="m26 10 2-3-8-2v3l6 2z">' +
        '  <animateTransform attributeName="transform" attributeType="xml" dur="0.8s" from="0 20 20" repeatCount="indefinite" to="360 20 20" type="rotate"/>' +
        "</path>";
      let text = "<strong>" + txt + "</strong>";

      switch (type) {
        case "success":
          toast.className = "t-toast t-float-toast t-toast--" + type + " toastIn";
          toast.innerHTML =
            '<svg viewBox="0 0 24 24">' +
            '   <g fill="currentColor">' + successIcon + '</g>' +
            '</svg>' +
            text;
          break;

        case "error":
          toast.className = "t-toast t-float-toast t-toast--" + type + " toastIn";
          toast.innerHTML =
            '<svg viewBox="0 0 24 24"><g fill="currentColor">' +
            errorIcon +
            "</g></svg>" +
            text;
          break;

        case "info":
          toast.className = "t-toast t-float-toast t-toast--" + type + " toastIn";
          toast.innerHTML =
            '<svg viewBox="0 0 24 24"><g fill="currentColor">' +
            infoIcon +
            "</g></svg>" +
            text;
          break;

        case "warning":
          toast.className = "t-toast t-float-toast t-toast--" + type + " toastIn";
          toast.innerHTML =
            '<svg viewBox="0 0 24 24"><g fill="currentColor">' +
            warnIcon +
            "</g></svg>" +
            text;
          break;

        case "help":
          toast.className = "t-toast t-float-toast t-toast--" + type + " toastIn";
          toast.innerHTML =
            '<svg viewBox="0 0 24 24"><g fill="currentColor">' +
            helpIcon +
            "</g></svg>" +
            text;
          break;

        default:
          toast.className = "t-toast t-float-toast t-toast--" + type + " toastIn";
          toast.innerHTML =
            '<svg xml:space="preserve" viewBox="0 0 40 40"><g fill="currentColor">' +
            load +
            "</g></svg>" +
            text;
          break;
      }

      document.body.appendChild(toast);

      setTimeout(function() {
        toast.classList.remove("toastIn");
        toast.classList.add("toastOut", "t-visibility-hidden");
      }, duration);

    }, 250);
  }


  document.addEventListener("DOMContentLoaded", function() {
    // Darkmode
    const darkmodeBtns = document.getElementById("tuChangeMode");
    const darkmode = new JsDarkMode(darkmodeBtns, {
      dark_linearGradient: "#fcf81c,#d5ef0d,#b7ff4a",
      light_linearGradient: "#97e6ff,#1b449c,#006bfc",
    });

    // Go Top
    const scrEl = document.getElementById("tuScrolltop")
    const scrollTop = new JsTuScrolltop(scrEl, {
      offset: 300,
      speed: 100
    });


    const tuHeader = new JsHeader('tHeader', {
      // subFixedID: 'tuJobFilter',
      fixClass: 'is-fix',
      hideClass: 'is-hide',
      showClass: 'is-show',
      showDelay: 100,
      ignorePageElID: 'detailHeader'
    });

    const scrollAnimation = new JsObserverAnimations({
      root: null,
      element: '[data-tu-an]',
      enableCallback: false,
      callback: 'data-tu-an',
      class: 'tu-an-inview',
      initClass: 'tu-an-init',
      threshold: 0.1,
      offset: {
        top: -64,
        bottom: -150,
      },
      direction: 'vertical',
      repeat: true,
      mirror: false,
      startEvent: 'DOMContentLoaded',
    })

    // setTimeout(function() {}, 2000)
    const imagesLazyLoad = JsImagesLazyLoad("img[data-src]");

    let tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'))
    let tooltipList = tooltipTriggerList.map(function(tooltipTriggerEl) {
      return new bootstrap.Tooltip(tooltipTriggerEl)
    })

    // console.log(tooltipTriggerList)
  });

  window.addEventListener("load", function(event) {

  });