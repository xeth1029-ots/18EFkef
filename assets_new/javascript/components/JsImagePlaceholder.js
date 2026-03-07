// JsImagePlaceholder(".js-placeholder",{
//   bgColor: '#d7d3dc',
//   textColor: '#52406b'
// });
"use strict";

const JsImagePlaceholder = function (element, options) {

  if (typeof element === "undefined" || element === null) {
    return;
  } 
  ///////////////////////////////
  // **  Private variables  ** //
  ///////////////////////////////
  const that = this;
  const body = document.getElementsByTagName("BODY")[0];

  if (typeof element === "undefined" || element === null) {
    return;
  }
  
  const defaultOptions = {
    width: '',
    height: '',
    text: '',
    fontFamily: "eurostile",
    fontWeight: "900",
    fontSize: "1.25rem",
    dy: null,
    bgColor: "#ababb3",
    textColor: "rgba(0,0,0,0.48)",
    dataUri: true,
    charset: "UTF-8"
  };

  options = JsUtils.deepExtend({}, defaultOptions, options);

  // console.time();
  const elements = document.querySelectorAll(element);

  // console.timeEnd();
  JsUtils.each(elements, function (event) {
    const width =
      event.getAttribute("data-tu-width") ||
      event.clientWidth ||
      options.width;
    const height =
      event.getAttribute("data-tu-height") ||
      event.clientHeight ||
      options.height;
    const fontSize =
      event.getAttribute("data-tu-font-size") ||
      Math.floor(Math.min(width, height) * 0.175) ||
      options.fontSize ||
      defaultOptions.fontSize;
    const dy =
      event.getAttribute("data-tu-dy") ||
      fontSize * 0.475 ||
      options.dy ||
      defaultOptions.dy;
    const fontFamily =
      event.getAttribute("data-tu-font-family") ||
      options.fontFamily ||
      defaultOptions.fontFamily;
    const fontWeight =
      event.getAttribute("data-tu-font-weight") ||
      options.fontWeight ||
      defaultOptions.fontWeight;
    const bgColor =
      event.getAttribute("data-tu-background") ||
      options.bgColor ||
      defaultOptions.bgColor;
    const textColor =
      event.getAttribute("data-tu-color") ||
      options.textColor ||
      defaultOptions.textColor;
    const text = event.getAttribute("data-tu-text") || options.text  || width + " x " + height;

    const str = `<svg xmlns="http://www.w3.org/2000/svg" width="${width}" height="${height}" viewBox="0 0 ${width} ${height}">
                      <rect fill="${bgColor}" width="${width}" height="${height}"/>
                      <text fill="${textColor}" font-family="${fontFamily}" font-size="${fontSize}" dy="${dy}" font-weight="${fontWeight}" x="50%" y="50%" text-anchor="middle">${text}</text>
                    </svg>`;

    const cleaned = str
      .replace(/[\t\n\r]/gim, "")
      .replace(/\s\s+/g, " ")
      .replace(/'/gim, "\\i");

    if (options.dataUri) {
      const encoded = encodeURIComponent(cleaned)
        .replace(/\(/g, "%28")
        .replace(/\)/g, "%29");

      event.title = "Placeholder " + text;
      event.alt = "Placeholder " + text;
      if (event.tagName === 'IMG') {
        event.src = `data:image/svg+xml;charset=${options.charset},${encoded}`;
      } else {
        event.style.backgroundImage = `url(data:image/svg+xml;charset=${options.charset},${encoded})`;
      }
    }
  });
};

if (typeof module !== "undefined" && typeof module.exports !== "undefined") {
  module.exports = JsImagePlaceholder;
}
