const JsInputMask = function (element, options) {
  ///////////////////////////////////
  // **  JsInputMask variables  ** //
  ///////////////////////////////////
  const that = this;
  const body = document.getElementsByTagName("BODY")[0];

  if (typeof element === "undefined" || element === null) {
    return;
  }

  const defaultOptions = {
    maskedInputs: document.querySelectorAll("[data-tu-mask]"),
    maskedNumber: "XdDmMyY9",
    maskedLetter: "_"
  };

  const _construct = function () {
    if (JsUtils.data(element).has("mask")) {
      JsUtils.data(element).get("mask");
    } else {
      _init();
    }
  };

  const _init = function () {
    that.options = JsUtils.deepExtend({}, defaultOptions, options);
    that.element = element;
    _setUpMasks(that.options.maskedInputs);
    that.maskedInputs = document.querySelectorAll("[data-tu-mask]");
    _activateinputMask(that.options.maskedInputs);
    JsUtils.data(that.element).set('mask', that);
  };

  const _setUpMasks = function (inputs) {
    let l = inputs.length;

    for (let i = 0; i < l; i++) {
      _createShell(inputs[i]);
    }
  };

  const _createShell = function (input) {
    let text = "",
        placeholder = input.getAttribute("placeholder");
    input.setAttribute("maxlength", placeholder.length);
    input.setAttribute("data-placeholder", placeholder);
    input.removeAttribute("placeholder");
    text = '<span class="shell">' + '<span aria-hidden="true" id="' + input.id + 'Mask"><i></i>' + placeholder + "</span>" + input.outerHTML + "</span>";
    input.outerHTML = text;
  };

  const _setValueOfMask = function (e) {
    let value = e.target.value,
        placeholder = e.target.getAttribute("data-placeholder");
    return "<i>" + value + "</i>" + placeholder.substr(value.length);
  }; // add event listeners


  const _activateinputMask = function (inputs) {
    let l;
    console.log(that.maskedInputs);

    for (let i = 0, l = inputs.length; i < l; i++) {
      if (that.maskedInputs[i].addEventListener) {
        that.maskedInputs[i].addEventListener("keyup", function (e) {
          _handleValueChange(e);
        }, false);
      } else if (that.maskedInputs[i].attachEvent) {
        that.maskedInputs[i].attachEvent("onkeyup", function (e) {
          e.target = e.srcElement;
          _handleValueChange(e);
        });
      }
    }
  };

  const _handleValueChange = function (e) {
    const id = e.target.getAttribute("id");

    switch (e.keyCode) {
      case 20: // caplocks
      case 17: // control
      case 18: // option
      case 16: // shift
      case 37: // arrow keys
      case 38:
      case 39:
      case 40:
      case 9:
        // tab (let blur handle tab)
        return;
    }

    document.getElementById(id).value = _handleCurrentValue(e);
    //owasp document.getElementById(id + "Mask").innerHTML = _setValueOfMask(e);
  };

  const _handleCurrentValue = function (e, value) {
    var isCharsetPresent = e.target.getAttribute("data-charset"),
        placeholder = isCharsetPresent || e.target.getAttribute("data-placeholder"),
        value = e.target.value,
        l = placeholder.length,
        newValue = "",
        i,
        j,
        isInt,
        isLetter,
        strippedValue;
    strippedValue = isCharsetPresent ? value.replace(/\W/g, "") : value.replace(/\D/g, "");

    for (i = 0, j = 0; i < l; i++) {
      let x = isInt = !isNaN(parseInt(strippedValue[j]));
      isLetter = strippedValue[j] ? strippedValue[j].match(/[A-Z]/i) : false;
      matchesNumber = that.options.maskedNumber.indexOf(placeholder[i]) >= 0;
      matchesLetter = that.options.maskedLetter.indexOf(placeholder[i]) >= 0;

      if (matchesNumber && isInt || isCharsetPresent && matchesLetter && isLetter) {
        newValue += strippedValue[j++];
      } else if (!isCharsetPresent && !isInt && matchesNumber || isCharsetPresent && (matchesLetter && !isLetter || matchesNumber && !isInt)) {
        return newValue;
      } else {
        newValue += placeholder[i];
      }

      if (strippedValue[j] == undefined) {
        break;
      }
    }

    if (e.target.getAttribute("data-valid-example")) {
      return _validateProgress(e, newValue);
    }

    return newValue;
  };

  const _validateProgress = function (e, value) {
    var validExample = e.target.getAttribute("data-valid-example"),
        pattern = new RegExp(e.target.getAttribute("pattern")),
        placeholder = e.target.getAttribute("data-placeholder"),
        l = value.length,
        testValue = "";

    if (l == 1 && placeholder.toUpperCase().substr(0, 2) == "MM") {
      if (value > 1 && value < 10) {
        value = "0" + value;
      }

      return value;
    }

    for (i = l; i >= 0; i--) {
      testValue = value + validExample.substr(value.length);

      if (pattern.test(testValue)) {
        return value;
      } else {
        value = value.substr(0, value.length - 1);
      }
    }

    return value;
  };

  _construct();
}; 

// Static
JsInputMask.getInstance = function (element) {
  // console.log(element)
  if (element && JsUtils.data(element).has("mask")) {
    return JsUtils.data(element).get("mask");
  } else {
    return null;
  }
}; 

// Create
JsInputMask.createInstances = function () {
  let selector = arguments.length > 0 && arguments[0] !== undefined ? arguments[0] : '[data-tu-mask="true"]';
  let body = document.getElementsByTagName("BODY")[0];
  let elements = body.querySelectorAll(selector);

  if (elements && elements.length > 0) {
    for (let i = 0, len = elements.length; i < len; i++) {
      new JsInputMask(elements);
    }
  }
}; 

// Global
JsInputMask.init = function () {
  JsInputMask.createInstances();
}; 

// On document ready
if (document.readyState === "loading") {
  document.addEventListener("DOMContentLoaded", JsInputMask.init);
} else {
  JsInputMask.init();
} 


// // Webpack support
// if (typeof module !== "undefined" && typeof module.exports !== "undefined") {
//   module.exports = JsInputMask;
// }