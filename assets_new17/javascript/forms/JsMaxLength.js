    // <div>
    //  <input type="text" data-tu-maxlength="true" spellcheck="false" placeholder="username" maxlength="20" required>
    //   <i class="uis uis-at"></i>
    //   <span class="t-counter">20</span>
    // </div>
  const JsMaxLength = function(element, options) {
    ///////////////////////////////
    // ** Private variables  ** //
    ///////////////////////////////
    var that = this;
    var body = document.getElementsByTagName("BODY")[0];

    if (typeof element === "undefined" || element === null) {
      return;
    }
    var defaultOptions = {
      tooltip: true,
      counter: '.t-counter'
    };

    const _construct = function _construct() {
      if (JsUtils.data(element).has("maxlength") === true) {
        that = JsUtils.data(element).get("maxlength");
      } else {
        _init();
      }
    };

    const _init = function() {
      that.options = JsUtils.deepExtend({}, defaultOptions, options);
      that.element = element;
      that.element.setAttribute('data-tu-maxlength', 'true');
      _handlers();
      JsUtils.data(that.element).set("maxlength", that);
    };

    const _handlers = function() {

      JsUtils.addEvent(that.element, "keyup", function(event) {
        event.preventDefault();
        _keyupHandler()
      });

      JsUtils.addEvent(that.element, "input", function(event) {
        event.preventDefault();
        _keyupHandler()
      });
    };

    const _keyupHandler = function() {

        let input = element.parentElement.children[0];
        let inputMaxLength = input.getAttribute("maxlength");
        let texts = '還可以輸入 ' + (inputMaxLength - input.value.length)+' 字';
        let counter = element.parentElement.querySelector(that.options.counter);
        counter.innerHTML = texts
    }
    
    _construct();
  }

  // Static methods
  JsMaxLength.getInstance = function(element) {
    if (element !== null && KTUtil.data(element).has('maxlength')) {
      return JsUtils.data(element).get('maxlength');
    } else {
      return null;
    }
  }
  // Create
  JsMaxLength.createInstances = function() {
    var selector =
      arguments.length > 0 && arguments[0] !== undefined ?
      arguments[0] :
      '[data-tu-maxlength="true"]';
    var body = document.getElementsByTagName("BODY")[0]; // Initialize Menus
    console.log(this)
    var elements = body.querySelectorAll(selector);
    var maxlength;

    if (elements && elements.length > 0) {
      for (var i = 0, len = elements.length; i < len; i++) {
        maxlength = new JsMaxLength(elements[i]);
      }
    }
  };

  // Global
  JsMaxLength.init = function() {
    JsMaxLength.createInstances();
  };
  
  // On document ready
  if (document.readyState === "loading") {
    document.addEventListener("DOMContentLoaded", JsMaxLength.init);
  } else {
    JsMaxLength.init();
  };
