'use strict';
const JsAlerts = function JsAlerts(config) {
  var type = config.type;
  var title = config.title;
  var message = config.message;
  var config_img = config.img;
  var config_icon = config.icon;
  var img = config_img === undefined ? '' : config_img;
  var config_buttonText = config.buttonText;
  var buttonText = config_buttonText === undefined ? '確定' : config_buttonText;
  var config_confirmText = config.confirmText;
  var confirmText = config_confirmText === undefined ? '確定' : config_confirmText;
  var config_vibrate = config.vibrate;
  var vibrate = config_vibrate === undefined ? [] : config_vibrate;
  var config_playSound = config.playSound;
  var playSound = config_playSound === undefined ? null : config_playSound;
  var config_cancelText = config.cancelText;
  var cancelText = config_cancelText === undefined ? '取消' : config_cancelText;
  var closeStyle = config.closeStyle;

  return new Promise(function(resolve) {
    var existingAlert = document.querySelector('.t-js-alert-wrapper');

    if (existingAlert) {
      existingAlert.remove();
    }

    var body = document.querySelector('body');

    var scripts = document.getElementsByTagName('script');

    var src = '';

    var _iteratorNormalCompletion = true;
    var _didIteratorError = false;
    var _iteratorError = undefined;

    try {
      for (var _iterator = scripts[Symbol.iterator](), _step; !(_iteratorNormalCompletion = (_step = _iterator.next()).done); _iteratorNormalCompletion = true) {
        var script = _step.value;

        if (script.src.includes('JsAlerts.js')) {
          src = script.src.substring(0, script.src.lastIndexOf('/'));
        }
      }
    } catch (err) {
      _didIteratorError = true;
      _iteratorError = err;
    } finally {
      try {
        if (!_iteratorNormalCompletion && _iterator.return) {
          _iterator.return();
        }
      } finally {
        if (_didIteratorError) {
          throw _iteratorError;
        }
      }
    }

    let closeStyleTemplate = "t-js-alert-close";

    if (closeStyle === "circle") {
      closeStyleTemplate = "t-js-alert-close-circle";
    }
    let btnTemplate = `
    <button class="btn t-js-alert-button t-pill btn--${type}">${buttonText}</button>
    `;
    if (type === "question") {
      btnTemplate = `
      <div class="question-buttons">
        <button class="btn t-js-confirm-button t-pill btn--${type}">${confirmText}</button>
        <button class="btn t-js-cancel-button t-pill btn--${type}">${cancelText}</button>
      </div>
      `;
    }

    if (vibrate.length > 0) {
      navigator.vibrate(vibrate);
    }

    if (playSound !== null) {
      var sound = new Audio(playSound);
      sound.play();
    }


    if (type === "success") {
      var alertIcon =
        `<path d="M22 12A10 10 0 1 1 12 2a10 10 0 0 1 10 10Z" opacity=".4"/>
         <path d="M10.6 15.6a.7.7 0 0 1-.5-.2l-2.9-2.9a.8.8 0 0 1 1-1l2.4 2.3 5.1-5.2a.8.8 0 0 1 1 1l-5.6 5.8a.7.7 0 0 1-.5.2Z"/>`
    }
    if (type === "error") {
      var alertIcon =
        `<path d="M14.9 2H9.1a3.5 3.5 0 0 0-2.1.9l-4.1 4A3.5 3.5 0 0 0 2 9.2V15a3.5 3.5 0 0 0 .9 2.1l4 4.1a3.5 3.5 0 0 0 2.2.9h5.8a3.5 3.5 0 0 0 2.1-.9l4.1-4A3.5 3.5 0 0 0 22 15V9.1a3.5 3.5 0 0 0-.9-2.1l-4-4.1a3.5 3.5 0 0 0-2.2-.9Z" opacity=".4"/>
         <path d="m13 12 3-3a.8.8 0 0 0-1-1l-3 3-3-3a.8.8 0 0 0-1 1l3 3-3 3a.8.8 0 0 0 0 1 .7.7 0 0 0 1 0l3-3 3 3a.7.7 0 0 0 1 0 .8.8 0 0 0 0-1Z"/>`
    }
    if (type === "info") {
      var alertIcon =
        '<path d="M2 13V7a5 5 0 0 1 5-5h10a5 5 0 0 1 5 5v7a5 5 0 0 1-5 5h-1a1 1 0 0 0-1 0l-2 2a1 1 0 0 1-2 0l-2-2H7a5 5 0 0 1-5-5Z" opacity=".4"/>' +
        '<path d="M15 11a1 1 0 0 1 1-1 1 1 0 0 1 1 1 1 1 0 0 1-1 1 1 1 0 0 1-1-1Zm-4 0a1 1 0 0 1 1-1 1 1 0 0 1 1 1 1 1 0 0 1-1 1 1 1 0 0 1-1-1Zm-4 0a1 1 0 0 1 1-1 1 1 0 0 1 1 1 1 1 0 0 1-1 1 1 1 0 0 1-1-1Z"/>';
    }
    if (type === "warning") {
      var alertIcon =
        '<path d="M11 2a2 2 0 0 1 2 0l2 2a2 2 0 0 0 1 0h2a2 2 0 0 1 2 2v2a2 2 0 0 0 0 1l2 2a2 2 0 0 1 0 2l-2 2a2 2 0 0 0 0 1v2a2 2 0 0 1-2 2h-2a2 2 0 0 0-1 0l-2 2a2 2 0 0 1-2 0l-2-2a2 2 0 0 0-1 0H6a2 2 0 0 1-2-2v-2a2 2 0 0 0 0-1l-2-2a2 2 0 0 1 0-2l2-2a2 2 0 0 0 0-1V6a2 2 0 0 1 2-2h2a2 2 0 0 0 1 0Z" opacity=".4"/>' +
        '<path d="M11 16a1 1 0 0 1 1-1 1 1 0 0 1 1 1 1 1 0 0 1-1 1 1 1 0 0 1-1-1Zm0-3V8a1 1 0 0 1 1-1 1 1 0 0 1 1 1v5a1 1 0 0 1-1 1 1 1 0 0 1-1-1Z"/>';
    }
    if (type === "question") {
      var alertIcon =
        '<path d="M17 18h-4l-4 3a1 1 0 0 1-2 0v-3a5 5 0 0 1-5-5V7a5 5 0 0 1 5-5h10a5 5 0 0 1 5 5v6a5 5 0 0 1-5 5Z" opacity=".4"/>' +
        '<path d="M11 14a1 1 0 0 1 1-1 1 1 0 0 1 1 1 1 1 0 0 1-1 1 1 1 0 0 1-1-1Zm0-3a2 2 0 0 1 1-2h1a1 1 0 0 0-1-1 1 1 0 0 0-1 1 1 1 0 0 1-1 0 2 2 0 0 1 2-3 2 2 0 0 1 2 3 2 2 0 0 1-1 1v1a1 1 0 0 1-1 1 1 1 0 0 1-1-1Z"/>';
    }
    var template = `
    <div class="t-js-alert-wrapper">
      <div class="t-js-alert-frame">
        <div class="t-js-alert-header t-bg-grad-${type}">
          <span class="${closeStyleTemplate}"></span>
          <svg class="t-js-alert-img" viewBox="0 0 24 24">
            <g fill="currentColor">' ${alertIcon} '</g>' 
          </svg>
        </div>
        <div class="t-js-alert-body">
          <span class="t-js-alert-title">${title}</span>
          <span class="t-js-alert-message">${message}</span>
          ${btnTemplate}
        </div>
      </div>
    </div>
    `;

    body.insertAdjacentHTML('afterend', template);
    body.classList.add("modal-open");
    var alertWrapper = document.querySelector('.t-js-alert-wrapper');
    var alertFrame = document.querySelector('.t-js-alert-frame');
    var alertClose = document.querySelector('.t-js-alert-close');

    if (type === 'question') {
      var confirmButton = document.querySelector('.t-js-confirm-button');
      var cancelButton = document.querySelector('.t-js-cancel-button');

      confirmButton.addEventListener('click', function() {
        alertWrapper.remove();
        body.classList.remove("modal-open");
        resolve('confirm');
      });

      cancelButton.addEventListener('click', function() {
        alertWrapper.remove();
        body.classList.remove("modal-open");
        resolve();
      });
    } else {
      var alertButton = document.querySelector('.t-js-alert-button');

      alertButton.addEventListener('click', function() {
        alertWrapper.remove();
        body.classList.remove("modal-open");
        resolve('ok');
      });
    }

    alertClose.addEventListener('click', function() {
      alertWrapper.remove();
      body.classList.remove("modal-open");
      resolve('close');
    });

    alertFrame.addEventListener('click', function(e) {
      e.stopPropagation();
    });
  });
};