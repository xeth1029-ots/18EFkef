/**
 * Component JsHeader.
 *
 * @author BarrY
 * @version 1.2.0
 *
 */
'use strict';

const JsHeader = function(element, options) {
  ///////////////////////////////
  // **  Private variables  ** //
  ///////////////////////////////
  var that = this;
  var body = document.getElementsByTagName("BODY")[0];

  if (typeof element === "undefined" || element === null) return;

  var defaultOptions = {
    subFixedID: '',
    fixClass: 'is-fix',
    hideClass: 'is-hide',
    showClass: 'is-show',
    showDelay: 100,
    ignorePageElID: ''
  };

  const header = document.getElementById(element);
  const fixClass = options.fixClass;
  const showClass = options.showClass;
  const hideClass = options.hideClass;

  const showDelay = options.showDelay;
  const subFixedID = options.subFixedID;
  const subFixedElem = document.getElementById(subFixedID);

  let previousScrollPosition = 0;
  let headerHeight = header.getBoundingClientRect().height || header.clientHeight;

  ////////////////////////////
  // ** Private methods  ** //
  ////////////////////////////
  const _construct = function _construct() {

    let ignoreEl = document.getElementById(options.ignorePageElID)
    if (ignoreEl) {
      return
    } else if (JsUtils.data(element).has('header')) {
      that = JsUtils.data(element).get('header');
    } else {
      _init();
    }
  };

  const _init = function() {

    that.options = JsUtils.deepExtend(that, defaultOptions, options);

    _handle(options);
    _setupScrollHandler();
  };


  const _scrollThrottle = JsUtils.throttle(function(event) {
    _handle(options);
    // console.log(options);
  }, showDelay)

  const _isScrollingDown = () => {
    let goingDown = false;
    let scrollPosition = window.pageYOffset;
    if (scrollPosition > previousScrollPosition) {
      goingDown = true
    }
    previousScrollPosition = scrollPosition;
    return goingDown;
  };

  const _handle = (options) => {

    let viewportHeight = window.innerHeight;
    let scrollPosition = window.pageYOffset;

    if (_isScrollingDown()) {
      header.classList.add(fixClass);
      _headerHide(hideClass, showClass);
    } else if (_isScrollingDown() === false) {
      header.classList.add(fixClass);
      _headerShow(hideClass, showClass);
    }

    if (scrollPosition === 0) {
      header.classList.remove(fixClass);
      header.classList.remove(hideClass);
      header.classList.remove(showClass);

      if (document.getElementById('sidePanel')) {
        if (!header.classList.contains(hideClass)) {
          document.getElementById('sidePanel').style.transform = "translateY(0)";
        }
      }
      if (subFixedElem) {
        subFixedElem.style.transform = "translateY(0)";
      }
    }
  };

  const _setupScrollHandler = function() {
    window.addEventListener("scroll", _scrollThrottle, false);
    window.addEventListener("resize", _scrollThrottle, false);
  }


  const _headerShow = function(hideClass, showClass) {
    header.classList.remove(hideClass);
    header.classList.add(showClass);

    if (subFixedElem) {
      if (!header.classList.contains(hideClass)) {
        subFixedElem.style.transform = "translateY(" + headerHeight + "px)";
      }
    }

    if (document.getElementById('sidePanel')) {
      if (!header.classList.contains(hideClass)) {
        document.getElementById('sidePanel').style.transform = "translateY(" + headerHeight + "px)";
      }
    }

  }

  const _headerHide = function(hideClass, showClass) {
    header.classList.remove(showClass);
    header.classList.add(hideClass);

    if (document.getElementById('sidePanel')) {
      if (!header.classList.contains(hideClass)) {
        document.getElementById('sidePanel').style.transform = "translateY(0)";
      }
    }

    if (subFixedElem) {
      if (header.classList.contains(hideClass)) {
        subFixedElem.style.transform = "translateY(0)";
      }
    }
  }

  const _destroy = function() {
    window.removeEventListener('scroll', _scrollThrottle);
    JsUtils.data(that.element).remove('header');
  }

  _construct();

}