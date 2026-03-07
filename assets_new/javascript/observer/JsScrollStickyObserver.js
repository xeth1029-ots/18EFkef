
const JsScrollStickyObserver = function(element, options) {
  const that = this;
  const body = document.getElementsByTagName("BODY")[0];
  // element 不在存退出
  if (typeof element === "undefined" || element === null) {
    return;
  }
  element = document.getElementById(element);
  // 不支援 IntersectionObserver 時，改用 getBoundingClientRect 監聽 
  if (!("IntersectionObserver" in window) &&
    !("IntersectionObserverEntry" in window) &&
    !("intersectionRatio" in window.IntersectionObserverEntry.prototype)
  ) {
    const initialCords = element.getBoundingClientRect();
    document.addEventListener('scroll', function() {
      if (window.scrollY >= initialCords.top) {
        element.classList.add(className);
      } else {
        element.classList.remove(className);
      }
    });
  }
  //////////////////////////////
  // **   defaultOptions   ** //
  //////////////////////////////
  ObserverOptions = {
    root: null,
    rootMargin: '', // 0px 0px -200px 0px
    threshold: ''
  } || options.callback;

  defaultOptions = {
    prevElementID: "",
    stopElementID: "",
    fixedClassName: "is-fix",
    stopClassName: "is-absolute",
    callback: {}
  }

  options = JsUtils.deepExtend(that, defaultOptions, options);

  const fixedClassName = options.fixedClassName || defaultOptions.fixedClassName;
  const stopClassName = options.stopClassName || defaultOptions.stopClassName;
  const stopElement = document.getElementById(options.stopElementID);
  const prevElement = document.getElementById(options.prevElementID);
  const listenSelect = prevElement.id;
  const stopSelect = stopElement.id;

  const observer = new IntersectionObserver(function(entries, observer) {

    let elementWidth = prevElement.clientWidth;
    let listenEntry = entries.find(entry => entry.target.id === listenSelect);
    let footerEntry = entries.find(entry => entry.target.id === stopSelect);
    // console.log(listenEntry.boundingClientRect.top)
    // console.log(listenEntry )
    console.log(document.documentElement.scrollTop)
    if (listenEntry && listenEntry.isIntersecting) {
      element.classList.remove(fixedClassName)
      element.style.width = '';
    } else if (footerEntry && footerEntry.isIntersecting) {
      element.classList.remove(fixedClassName)
      element.classList.add(stopClassName)
      // element.style.transform = "translateY(" + headerHeight + "px)";
      element.style.width = elementWidth + 'px'
    } else {
      if (document.documentElement.scrollTop !== 0) {
        element.classList.add(fixedClassName)
        element.classList.remove(stopClassName)
        element.style.width = elementWidth + 'px'
      }

    }

  });

  [prevElement, stopElement].forEach(function(el) {
    return observer.observe(el);
  });

};