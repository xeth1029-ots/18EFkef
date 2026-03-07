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
        className: "js_dark__mode",
        isDarkMode: "",
        gradientID: "data-tu-gradient",
        dark_linearGradient: "#fcf81c,#d5ef0d,#b7ff4a",
        light_linearGradient: "#97e6ff,#1b449c,#006bfc",
    };

    var svgVan = document.getElementById('tuChangeMode')

    var options = JsUtils.deepExtend({}, defaultOptions, options);
    // console.log(svgVan)
    const darkLinearGradient =
        svgVan.getAttribute("data-tu-dark-gradient") || options.dark_linearGradient || defaultOptions.dark_linearGradient;

    const lightLinearGradient =
        svgVan.getAttribute("data-tu-light-gradient") || options.light_linearGradient || defaultOptions.light_linearGradient;

    const gradID = defaultOptions.gradientID;

    // console.log(light_linearGradient)

    ////////////////////////////
    // ** Private methods  ** //
    ////////////////////////////

    const moonPath = "M28 0C35 0 42 3 47 8 52 13 55 20 55 28 55 35 52 42 47 47 42 52 35 55 28 55 20 55 13 52 8 47 3 42 0 35 0 28 0 20 3 13 8 8 13 3 20 0 28 0Z";
    const sunPath =  "M28 0C35 0 42 3 47 8 34 10 27 20 27 28 27 35 34 45 47 47 42 52 35 55 28 55 20 55 13 52 8 47 3 42 0 35 0 28 0 20 3 13 8 8 13 3 20 0 28 0Z";


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
        if (isDarkMode) {
            document.documentElement.setAttribute("data-theme", "dark");
            let path = sunPath;
        } else {
            document.documentElement.setAttribute("data-theme", "light");
            let path = moonPath;
        }
        const svg = document.createElementNS('http://www.w3.org/2000/svg', 'svg');
        svg.setAttribute('viewBox', '-2 -2 60 60');
        const iconPath = document.createElementNS('http://www.w3.org/2000/svg', 'path');
        const linearGradientArrayKey = ['gradientUnits', 'id', "x1", "x2", "y1", "y2"];
        const linearGradientArrayVal = ['userSpaceOnUse', 'data-tu-gradient', "18", "50", "62", "6"];
        const linearGradient = document.createElement("linearGradient");
        for (let i = 0; i < linearGradientArrayKey.length; i++) {
          // console.log(linearGradientArrayKey[i])
            linearGradient.setAttribute(linearGradientArrayKey[i] , linearGradientArrayVal[i])
        }
        let linearGradients = linearGradient.outerHTML;

        iconPath.setAttribute('d', moonPath);
        iconPath.setAttribute('fill', 'url(#' + gradID + ')');
        iconPath.setAttribute('stroke', 'white');
        iconPath.setAttribute('stroke-width', '2.5');
        iconPath.setAttribute('stroke-opacity', '0.66');
        iconPath.setAttribute('ill-rule', 'evenodd');

        svg.appendChild(iconPath);
        svg.innerHTML = linearGradients;
        svg.appendChild(iconPath);
        svgVan.appendChild(svg);

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
        svgVan.querySelector('path').setAttribute('d', moonPath)
        body.classList.add(options.className);
        document.documentElement.setAttribute("data-theme", "dark");
        localStorage.setItem(options.storageName, "enabled");
    };

    const offDarkMode = function offDarkMode() {

        window.matchMedia("(prefers-color-scheme: light)");
        svgVan.querySelectorAll("#" + gradID + "").forEach(function(linearGradient, i) {
            let colors = lightLinearGradient;
            linearGradient.innerHTML = colors.split(",").map((color, idx, colors) =>
                `<stop offset="${(idx * 1) / colors.length}" stop-color="${color}"/>`).join("");
        });
        svgVan.querySelector('path').setAttribute('d', sunPath);
        body.classList.remove(options.className);
        document.documentElement.setAttribute("data-theme", "light");
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
    // console.log(elements)
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