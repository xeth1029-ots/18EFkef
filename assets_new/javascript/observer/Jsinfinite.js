
// Jsinfinite

const Jsinfinite = function(elem, ObserverOptions, buildItemFun) {
  ///////////////////////////////
  // **  Private variables  ** //
  ///////////////////////////////
  var that = this;
  const trigger = document.querySelector('' + elem + '');

  dObserverOptions = {
    root: null,
    rootMargin: "0px 0px 0px 0px",
    threshold: [0, 1]
  };


  var ObserverOptions = Object.assign({}, that.dObserverOptions, ObserverOptions || {});


  const observer = new IntersectionObserver((entries) => {
    entries.forEach(function(entry) {
      if (entry.isIntersecting) {
        buildItems(buildItemFun);
        CoreCollects('.t-btn-fav');
        CoreSeparated('[data-separated]');
      }
    });
  }, ObserverOptions);

  observer.observe(trigger, ObserverOptions);

  const buildItems = function(options) {
    const that = this;
    const body = document.getElementsByTagName("BODY")[0];
    that.options = {
      buildArea: 'body',
      buildTagName: "div",
      buildTagClassName: "",
      loop: 10,
      html: "",
    };
    const observeriPages = function(items, select) {
      if (!("IntersectionObserver" in window) &&
        !("IntersectionObserverEntry" in window) &&
        !("intersectionRatio" in window.IntersectionObserverEntry.prototype)
      ) {
        return;
      }
      const elements = document.querySelectorAll(items);
      
      const observerOptions = {
        root: null,
        rootMargin: "0px 0px 0px 0px",
        threshold: [0, 1]
      };

      function observerCallback(entries) {
        entries.forEach(function(entry) {
          if (entry.isIntersecting) {
            const s_selects_pages = document.getElementById(select);
            const val = entry.target.getAttribute("data-pages");
            s_selects_pages.value = +val;
            _scroll();
          }
        });
      }

      const observer = new IntersectionObserver(observerCallback, observerOptions);
      elements.forEach(function(el) {
        return observer.observe(el);
      });
    }
    var options = Object.assign({}, that.options, options || {});
    const fragment = document.createDocumentFragment();
    const selector = document.querySelector(options.buildArea);

    for (let i = 0; i < options.loop; i++) {
      const element = document.createElement(options.buildTagName);

      inner = options.html;
      var inner = JsUtils.stringBind(inner);
      element.className = options.buildTagClassName;
      element.innerHTML = inner;
      fragment.appendChild(element);
    }

    const pagination = document.createElement('div');
    const pageNum = ((document.querySelectorAll('.' + options.buildTagClassName).length / options.loop) + 1);
    const pageNumAll = (options.loop);
    const paginationText = '第 ' + pageNum + ' / ' + pageNumAll + ' 頁';

    pagination.setAttribute('data-pages', pageNum);
    pagination.className = "t-paginations";
    pagination.innerHTML = paginationText;
    fragment.appendChild(pagination);
    observeriPages("[data-pages]", "s_selects_pages");

    return selector.append(fragment);
  };

  const _scroll = function() {
    const s_selects_pages = document.getElementById('s_selects_pages');
    s_selects_pages.addEventListener('change', function(e) {
      console.log(e.target.value);
      var scr = document.querySelector('[data-pages="' + e.target.value + '"]');
      JsUtils.scrollTo(scr)
    })
  }



}