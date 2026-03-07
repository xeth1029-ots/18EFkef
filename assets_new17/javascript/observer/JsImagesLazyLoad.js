// JsImagesLazyLoad

const JsImagesLazyLoad = function(elements, options) {
  //////////////////////////////////////
  // **  Private JsImagesLazyLoad  ** //
  //////////////////////////////////////
  const that = this;
  const body = document.getElementsByTagName("BODY")[0];
  // 不支援 IntersectionObserver 時 改用 scroll 監聽
  if (!("IntersectionObserver" in window) &&
    !("IntersectionObserverEntry" in window) &&
    !("intersectionRatio" in window.IntersectionObserverEntry.prototype)
  ) {
    function OlderImgLazyLoad() {
      var scrollTop =
        document.body.scrollTop || document.documentElement.scrollTop;
      var winHeight = window.innerHeight;
      for (var i = 0; i < lazyImages.length; i++) {
        if (lazyImages[i].offsetTop < scrollTop + winHeight) {
          lazyImages[i].src = lazyImages[i].getAttribute("data-src");
        }
      }
    }
    document.addEventListener("scroll", supports.throttle(function(event) {
      OlderImgLazyLoad()
    }, 250));
  }

  // JsImagePlaceholder("img[data-src]",{
  //   bgColor: '#d7d3dc',
  //   textColor: '#52406b'
  // });

  var lazyImages = document.querySelectorAll(elements);
  
  var imageLoad = function(image) {
    // console.log(image)
    image.setAttribute("src", image.getAttribute("data-src"));
    image.removeAttribute("data-src");
  };

  var observerOptions = {
    root: null,
    rootMargin: '',
    threshold: 0
  };

  function observerCallback(images) {
    images.forEach(function(image) {
      if (image.isIntersecting) {

        imageLoad(image.target);

        // function callback(el) {
        //   // console.dir(el.parentElement.children.item('IMG'));
        //   var _canvas = document.createElement('canvas');
        //   return el.parentElement.children.item('IMG').insertAdjacentElement('afterend', _canvas);
        // }
        // console.log(image.target.clientWidth / 20)
        setTimeout(function() {
          // localBlur(image.target, 20 , [10, 0, (image.target.clientWidth / 100), (image.target.clientHeight / 200)], callback(image.target));
          image.target.classList.remove("t-lazy-load")
        }, 400)

        observer.unobserve(image.target);
      }

    });
  }

  var observer = new IntersectionObserver(observerCallback, observerOptions);

  lazyImages.forEach(function(image) {
    observer.observe(image);
  });
};

// 觀察元素是否進入視窗範圍
var observerAnimations = function observerAnimations(Items) {
  if (!("IntersectionObserver" in window) &&
    !("IntersectionObserverEntry" in window) &&
    !("intersectionRatio" in window.IntersectionObserverEntry.prototype)
  ) {
    return;
  }

  var elementsToLoadIn = document.querySelectorAll(Items);

  var observerOptions = {
    root: null,
    rootMargin: '',
    threshold: 0
  };

  function observerCallback(entries) {
    entries.forEach(function(entry) {
      if (entry.isIntersecting) {
        entry.target.classList.add("an-loaded");
        // 停止觀察
        setTimeout(function() {
          entry.target.classList.remove("an-loadin");
          entry.target.classList.remove("an-loaded");
        }, 200);
        observer.unobserve(entry.target);
      }
    });
  }

  var observer = new IntersectionObserver(observerCallback, observerOptions);

  elementsToLoadIn.forEach(function(el) {
    return observer.observe(el);
  });
};


