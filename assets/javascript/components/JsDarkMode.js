"use strict";
const JsDarkMode = function(element, options) {
    ///////////////////////////////
    // **  Private variables  ** //
    ///////////////////////////////
    const that = this;
    const body = document.getElementsByTagName("BODY")[0];

    if (typeof element === "undefined" || element === null) {
        return;
    }

    const defaultOptions = {
        storageName: "darkMode",
        className: "dark__mode",
        isDarkMode: "",
        gradientID: "data-tu-gradient",
        dark_linearGradient: "#fcf81c,#d5ef0d,#b7ff4a",
        light_linearGradient: "#97e6ff,#1b449c,#006bfc",
    };
    const svgVan = element.querySelector("svg");

    var options = JsUtils.deepExtend({}, defaultOptions, options);

    const darkLinearGradient =
        svgVan.getAttribute("data-tu-dark-gradient") || options.dark_linearGradient || defaultOptions.dark_linearGradient;

    const lightLinearGradient =
        svgVan.getAttribute("data-tu-light-gradient") || options.light_linearGradient || defaultOptions.light_linearGradient;

    const gradID = defaultOptions.gradientID;

    // console.log(light_linearGradient)

    ////////////////////////////
    // ** Private methods  ** //
    ////////////////////////////


    const _construct = function _construct() {
        if (JsUtils.data(element).has("scheme")) {
            JsUtils.data(element).get("scheme");
        } else {
            _init();
        }
    };

    const _init = function _init() {

        const darkMode = localStorage.getItem(options.storageName);
        const isDarkMode = window.matchMedia("(prefers-color-scheme: dark)").matches;
        const linearGradientArrayKey = ['gradientUnits', 'id', "x1", "x2", "y1", "y2"];
        const linearGradientArrayVal = ['userSpaceOnUse', 'data-tu-gradient', "18", "50", "62", "6"];
        const linearGradient = document.createElement("linearGradient");
        for (let i = 0; i < linearGradientArrayKey.length; i++) {
            linearGradient.setAttribute(linearGradientArrayKey[i], linearGradientArrayVal[i])
        }
        const path =
            "M 0 35 c 0 -2 1 -4 3 -4 c 1 0 2 0 3 1 A 19 19 0 0 0 33 6 a 4 4 0 0 1 -1 -3 c 0 -2 2 -4 4 -3 A 32 32 0 1 1 0 35 z";
        const svgInner =
            '<path fill="url(#' + gradID + ')"' + 'fill-rule="evenodd" d="' + path + '" clip-rule="evenodd" />';
        svgVan.appendChild(linearGradient);
        svgVan.innerHTML += svgInner;

        that.uid = JsUtils.getUniqueId("scheme");
        that.element = element;
        that.element.setAttribute("data-tu-scheme", "true");


        darkMode === "enabled" ? openDarkMode() : offDarkMode();
        _handlers();
        JsUtils.data(that.element).set("scheme", that);
    };

    const openDarkMode = function openDarkMode() {
        window.matchMedia("(prefers-color-scheme: dark)");
        svgVan
            .querySelectorAll("#" + gradID + "")
            .forEach(function(linearGradient, i) {
                let colors = darkLinearGradient;
                linearGradient.innerHTML = colors
                    .split(",")
                    .map(
                        (color, idx, colors) =>
                        `<stop offset="${
                (idx * 1) / colors.length
              }" stop-color="${color}"/>`
                    )
                    .join("");
            });
        body.classList.add(options.className);
        localStorage.setItem(options.storageName, "enabled");
    };

    const offDarkMode = function offDarkMode() {
        window.matchMedia("(prefers-color-scheme: light)");
        svgVan
            .querySelectorAll("#" + gradID + "")
            .forEach(function(linearGradient, i) {
                let colors = lightLinearGradient;
                linearGradient.innerHTML = colors
                    .split(",")
                    .map(
                        (color, idx, colors) =>
                        `<stop offset="${
                (idx * 1) / colors.length
              }" stop-color="${color}"/>`
                    )
                    .join("");
            });
        body.classList.remove(options.className);
        localStorage.setItem(options.storageName, "");
    };

    const scheme = window.matchMedia("scheme-dark");

    const observeMediaChange = function observeMediaChange(scheme, callback) {
        let disposeFunc = {};
        if (scheme.addEventListener && scheme.removeEventListener) {
            scheme.addEventListener("change", callback);
            disposeFunc = function() {
                scheme.removeEventListener("change", callback);
            };
        }
        return disposeFunc;
    };

    const _handlers = function _handlers() {
        observeMediaChange(scheme, function(event) {
            event.matches ? openDarkMode() : offDarkMode();
        });
        JsUtils.addEvent(that.element, "click", function(event) {
            let darkMode = localStorage.getItem("darkMode");
            darkMode == "enabled" ? offDarkMode() : openDarkMode();
        });
    };

    const _destroy = function _destroy() {
        JsUtils.data(that.element).remove("scheme");
    };

    _construct();
    ///////////////////////
    // ** Public API  ** //
    ///////////////////////
    that.destroy = function() {
        return _destroy();
    };
};

// Static
JsDarkMode.getInstance = function(element) {
    if (element && JsUtils.data(element).has("scheme")) {
        return JsUtils.data(element).get("scheme");
    } else {
        return null;
    }
};

// Create
JsDarkMode.createInstances = function() {
    let selector =
        arguments.length > 0 && arguments[0] !== undefined ?
        arguments[0] :
        '[data-tu-scheme="true"]';
    let body = document.getElementsByTagName("BODY")[0];

    let elements = body.querySelectorAll(selector);
    let darkMode;

    if (elements && elements.length > 0) {
        for (let i = 0, len = elements.length; i < len; i++) {
            darkMode = new JsDarkMode(elements[i]);
        }
    }
};

// Global
JsDarkMode.init = function() {
    JsDarkMode.createInstances();
};

// On document ready
if (document.readyState === "loading") {
    document.addEventListener("DOMContentLoaded", JsDarkMode.init);
} else {
    JsDarkMode.init();
}

// Webpack support
if (typeof module !== "undefined" && typeof module.exports !== "undefined") {
    module.exports = JsDarkMode;
}