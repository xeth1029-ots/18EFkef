/**
 * Modal markup helper-wrapper.
 *
 * @author Barry
 * @version 2.0.1
 *
 */
class JsCoreComponentsModal {

  _options;
  _modal;
  _modalEl;
  _footer;

  static _bootstrapModal;

  static _defaultOptions = {
    icon: null,
    style: '',
    titleStyle: null,
    title: 'Information',
    body: '',
    message: '',
    size: null,
    center: false,
    animation: 'zoomIn',
    show: false,
    relatedTarget: undefined,
    scrollable: true,
    destroyOnClose: false,
    defaultButton: true,
    swapButtonOrder: false,
    justifyButtons: null,
    showHeaderClose: true,
    events: {},

    // only applies to instance/constructor modals
    buttons: [],

    // only applies to static modals, and overwrites defaults set by JsCoreComponentsModal.defaultButtonOptions
    okButton: {
      label: 'OK',
      style: 'primary'
    },
    closeButton: {
      label: 'Close',
      style: 'secondary'
    },

    // only applies to .prompt() static modal
    input: {
      type: 'text',
      class: '',
      value: '',
      title: null,
      placeholder: null,
      autocomplete: 'off',
      minlength: null,
      maxlength: null,
      pattern: null,
      required: false,
      sanitizer: false
    },

    // meant to be overridden with user defined function
    sanitizer: JsCoreComponentsModal._sanitizeString
  };


  static _defaultButtonOptions = {
    label: 'Close',
    style: 'secondary',
    class: '',
    outline: false,
    size: null,
    icon: null,
    title: null,
    disabled: false,
    close: true,
    callback: null
  };


  // default options for each static modal type
  static _modalDefaults = {
    alert: {
      title: 'Alert'
    },
    info: {
      style: 'info',
      title: 'Information'
    },
    success: {
      style: 'success',
      title: 'Success'
    },
    warning: {
      style: 'warning',
      title: 'Warning'
    },
    danger: {
      style: 'danger',
      title: 'Error'
    },
    confirm: {
      title: 'Confirm'
    },
    prompt: {
      title: 'Prompt',
      input: {
        id: JsCoreComponentsModal._getUID('JsCoreComponentsModal-input-')
      }
    }
  };


  // generate a unique id
  static _getUID(prefix = 'JsCoreComponentsModal-') {
    return prefix + Date.now() + Math.floor(parseFloat('0.'+crypto.getRandomValues(new Uint32Array(1))[0]) * 10000);
  }


  // more specific type checking than standard typeof
  static _typeof(obj) {
    return (typeof obj === 'object') ? Object.prototype.toString.call(obj).slice(8, -1).toLowerCase() : typeof obj;
  }


  // recursive object merge
  static _deepMerge(target, source) {
    const result = { ...target, ...source };

    Object.keys(result).forEach(key => {
      const tProp = target[key];
      const sProp = source[key];

      if (JsCoreComponentsModal._typeof(tProp) === 'object' && JsCoreComponentsModal._typeof(sProp) === 'object') {
        result[key] = JsCoreComponentsModal._deepMerge(tProp, sProp);
      }
    });

    return result;
  }


  // if string passed in as options argument, convert to an object and use as the body value
  static _checkUserOptions(userOptions) {
    return (typeof userOptions === 'string') ? { body: userOptions } : userOptions;
  }


  // this has to be done on the fly as opposed to when initializing _modalDefaults above, otherwise changes to JsCoreComponentsModal.defaultOptions will not be reflected in the static modals
  static _mergeModalOptions(modalType, userOptions = {}) {
    return JsCoreComponentsModal._deepMerge(
      JsCoreComponentsModal._deepMerge(JsCoreComponentsModal._defaultOptions, JsCoreComponentsModal._modalDefaults[modalType]),
      JsCoreComponentsModal._checkUserOptions(userOptions)
    );
  }


  // default sanitizer function which just returns the string unmodified
  static _sanitizeString(str = '') {
    return str;
  }


  // build custom modal that returns a Promise
  static _buildPromiseModal(options = {}, type = 'alert') {
    options = {
      ...options,
      // defaults that cannot be overridden
      destroyOnClose: true,
      defaultButton: false,
      buttons: []
    };

    return new Promise((resolve, reject) => {
      const box = new JsCoreComponentsModal(options);

      // build button configurations
      const btns = [options.closeButton];

      if (['confirm', 'prompt'].includes(type)) {
        let okCallback = () => resolve();

        if (type === 'prompt' && JsCoreComponentsModal._typeof(options.input) === 'object') {
          let validateInput = false;

          // don't add modal close markup to button if an option is specified that needs to be validated (handled in button callback instead)
          if (options.input.required === true || typeof options.input.minlength === 'number' || (typeof options.input.pattern === 'string' && options.input.pattern.length)) {
            options.okButton.close = false;
            validateInput = true;
          }

          okCallback = () => {
            const inputEl = box.modalEl.querySelector(`#${options.input.id}`);
            const isValid = (validateInput === true) ? inputEl.reportValidity() : true;

            if (isValid) {
              const sanitizer = (typeof box.options.input.sanitizer === 'function') ? box.options.input.sanitizer :
                (box.options.input.sanitizer === true) ? box.options.sanitizer : JsCoreComponentsModal._sanitizeString;

              resolve(sanitizer(inputEl.value));
              box.hide();
            }
          };
        }

        btns.unshift({
          ...options.okButton,
          callback: function(ev, modal) {
            if (typeof options.okButton.callback === 'function') {
              options.okButton.callback.call(this, ev, modal);
            }

            const defaultPrevented = ev.defaultPrevented != null ? ev.defaultPrevented : ev.returnValue === false;

            if (defaultPrevented) {
              return;
            }

            okCallback();
          }
        });
      }

      // add buttons to modal
      const [okBtn, closeBtn] = btns.map(btnOptions => box.addButton(btnOptions));

      // trigger okButton if enter key pressed within input
      if (type === 'prompt' && JsCoreComponentsModal._typeof(options.input) === 'object') {
        box.modalEl.querySelector(`_${options.input.id}`).addEventListener('keyup', (ev) => {
          if (ev.key === 'Enter') {
            okBtn.click();
          }
        });
      }

      // settle the Promise if the modal is closed in a way other than clicking the buttons (click X, click backdrop, press ESC, etc)
      box.addEvent('hidden', () => {
        if (type === 'alert') {
          resolve();
        }
        else if (['confirm', 'prompt'].includes(type) && document.activeElement !== okBtn) {
          reject();
        }
      });

      box.show();
    });
  }
// (config.animation ? config.animation : config['animation'])

  _buildModal() {
    const isDarkStyle = ['primary', 'secondary', 'success', 'danger', 'dark', 'body'].includes(this._options.style);
    const titleStyle = this._options.titleStyle || (isDarkStyle ? 'white' : 'dark');
    const closeButtonStyle = `btn-close ${isDarkStyle ? 'btn-close-white' : ''}`;

    JsCoreComponentsModal.container.insertAdjacentHTML('beforeend', this._options.sanitizer(`
      <div class="modal ${this._options.animation ? this._options.animation : this._options['animation']}" id="${this._options.id}" tabindex="-1" aria-labelledby="${this._options.id}-title" aria-hidden="true">
        <div class="modal-dialog ${this._options.scrollable ? 'modal-dialog-scrollable' : ''} ${this._options.center ? 'modal-dialog-centered' : ''} ${this._options.size ? `modal-${this._options.size}` : ''}">
          <div class="modal-content">
            <div class="modal-header ${this._options.style ? `bg-${this._options.style}` : ''} ${!this._options.title ? 'd-none' : ''}">
              <h3 class="modal-title text-${titleStyle}">
                ${this._options.icon ? `<i class="${this._options.icon} me-3"></i>` : ''}
                <h4 id="${this._options.id}-title">${this._options.title}</h4>
              </h3>
              <button type="button" class="${closeButtonStyle} ${this._options.showHeaderClose === false ? 'd-none' : ''}" data-bs-dismiss="modal" aria-label="Close"></button>
            </div>
            <div class="modal-body">
              ${this._options.body}
            </div>
            <div class="modal-footer ${this._options.justifyButtons ? `d-flex justify-content-${this._options.justifyButtons}` : ''}"></div>
          </div>
        </div>
      </div>
    `.trim()));

    this._modalEl = JsCoreComponentsModal.container.querySelector(`#${this._options.id}`);
    this._footer = this._modalEl.querySelector('.modal-footer');

    // add buttons to modal
    this._addButtons();
  }


  _addButtons() {
    if (!Array.isArray(this._options.buttons)) {
      this._options.buttons = [];
    }

    if (this._options.buttons.length === 0 && this._options.defaultButton === true) {
      // add default button
      this._options.buttons = [JsCoreComponentsModal._defaultButtonOptions];
    }
    else {
      // hide footer when there are no buttons
      this._footer.classList.add('d-none');
    }

    this._options.buttons.forEach(userBtnOptions => this.addButton(userBtnOptions));
  }


  _addEvents() {
    Object.entries(this._options.events).forEach(([type, fn]) => {
      this.addEvent(type, fn);
    });

    if (this._options.destroyOnClose === true) {
      this.addEvent('hidden', () => this.destroy());
    }
  }


  /* public members */

  constructor(userOptions = {}) {
    // make sure there is a reference to bootstrap
    if (!JsCoreComponentsModal._bootstrapModal) {
      // if bootstrapModal property has not been set, try to use global bootstrap object if it exists
      if (typeof bootstrap === 'object') {
        JsCoreComponentsModal._bootstrapModal = bootstrap.Modal;
      }
      else {
        throw new Error('The "JsCoreComponentsModal.bootstrapModal" property is undefined.  If importing Bootstrap as an ES module, you must also manually set this property.  See the JsCoreComponentsModal README/docs for more info.');
      }
    }

    // add bootstrap modal defaults to JsCoreComponentsModal default options
    // this is done here as opposed to on _defaultOptions initialization to avoid hoisting issues when bootstrap is loaded as an ES module
    JsCoreComponentsModal._defaultOptions = {
      ...JsCoreComponentsModal._bootstrapModal.Default,
      ...JsCoreComponentsModal._defaultOptions
    };

    this._options = {
      // JsCoreComponentsModal default options
      ...JsCoreComponentsModal._defaultOptions,
      id: JsCoreComponentsModal._getUID(),
      // user options
      ...JsCoreComponentsModal._checkUserOptions(userOptions)
    };

    // check for required options
    if (typeof this._options.body !== 'string' || !this._options.body.length) {
      if (typeof this._options.message === 'string' && this._options.message.length) {
        this._options.body = this._options.message;
      }
      else {
        throw new Error('The "body" or "message" configuration option is required (string).');
      }
    }

    // generate modal HTML and add to DOM
    this._buildModal();

    // create bootstrap modal from generated HTML
    this._modal = new JsCoreComponentsModal._bootstrapModal(
      this._modalEl,
      (({ backdrop, keyboard, focus }) => ({ backdrop, keyboard, focus }))(this._options)
    );

    // attach events to modal
    this._addEvents();

    if (this._options.show === true) {
      this.show(this._options.relatedTarget);
    }
  }


  get options() {
    return this._options;
  }


  // returns the native bootstrap modal object
  get modal() {
    return this._modal;
  }


  // returns the top-level modal DOM element
  get modalEl() {
    return this._modalEl;
  }


  get buttons() {
    return [...this._footer.querySelectorAll('button')];
  }


  static get bootstrapModal() {
    return JsCoreComponentsModal._bootstrapModal;
  }


  static set bootstrapModal(bootstrapModalRef) {
    JsCoreComponentsModal._bootstrapModal = bootstrapModalRef;
  }


  static get defaultOptions() {
    return JsCoreComponentsModal._defaultOptions;
  }


  static set defaultOptions(userDefaultOptions = {}) {
    JsCoreComponentsModal._defaultOptions = JsCoreComponentsModal._deepMerge(JsCoreComponentsModal._defaultOptions, userDefaultOptions);
  }


  static get defaultButtonOptions() {
    return JsCoreComponentsModal._defaultButtonOptions;
  }


  static set defaultButtonOptions(userDefaultButtonOptions = {}) {
    JsCoreComponentsModal._defaultButtonOptions = { ...JsCoreComponentsModal._defaultButtonOptions, ...userDefaultButtonOptions };
  }


  // set default options for each static modal type
  static setDefaults(modalType, modalOptions = {}) {
    modalType = modalType.trim?.().toLowerCase?.();

    if (modalType === 'error') {
      modalType = 'danger';
    }

    if (!modalType || !['alert', 'info', 'success', 'warning', 'danger', 'confirm', 'prompt'].includes(modalType)) {
      throw new Error('Invalid modal type.');
    }

    modalOptions = JsCoreComponentsModal._typeof(modalOptions) === 'object' ? modalOptions : {};
    JsCoreComponentsModal._modalDefaults[modalType] = JsCoreComponentsModal._deepMerge(JsCoreComponentsModal._modalDefaults[modalType], modalOptions);
  }


  addButton(userBtnOptions = {}, swapOrder = this._options.swapButtonOrder) {
    // show footer if hidden
    this._footer.classList.remove('d-none');

    const appendLocation = swapOrder ? 'afterbegin' : 'beforeend';

    if (typeof userBtnOptions === 'string' && userBtnOptions.length) {
      this._footer.insertAdjacentHTML(appendLocation, this._options.sanitizer(userBtnOptions));
      const buttons = this.buttons;
      return buttons[swapOrder ? 0 : buttons.length - 1];
    }

    const btnOptions = {
      ...JsCoreComponentsModal._defaultButtonOptions,
      id: JsCoreComponentsModal._getUID('JsCoreComponentsModal-btn-'),
      ...userBtnOptions
    };

    this._footer.insertAdjacentHTML(appendLocation, this._options.sanitizer(`
      <button
        type="button"
        class="btn btn-${btnOptions.outline ? 'outline-' : ''}${btnOptions.style} ${btnOptions.class} ${btnOptions.size ? `btn-${btnOptions.size}` : ''}"
        id="${btnOptions.id}"
        ${btnOptions.title ? `title="${btnOptions.title}"` : ''}
        ${btnOptions.close ? 'data-bs-dismiss="modal"' : ''}
        ${btnOptions.disabled ? 'disabled' : ''}
      >
        ${btnOptions.icon ? `<i class="${btnOptions.icon} me-2"></i>` : ''}${btnOptions.label}
      </button>
    `.trim()));

    const btn = this._footer.querySelector(`#${btnOptions.id}`);

    if (btn && typeof btnOptions.callback === 'function') {
      btn.addEventListener('click', (ev) => btnOptions.callback.call(btn, ev, this));
    }

    return btn;
  }


  addEvent(type, callback) {
    if (['show', 'shown', 'hide', 'hidden', 'hidePrevented'].includes(type) && typeof callback === 'function') {
      this._modalEl.addEventListener(`${type}.bs.modal`, (ev) => callback.call(this._modalEl, ev, this));
    }
  }


  destroy() {
    this.dispose();
    this._modalEl.remove();
  }


  // returns the container element that holds all JsCoreComponentsModales, and creates it if it doesn't exist
  static get container() {
    let containerEl = document.querySelector('#JsCoreComponentsModal-container');

    if (!containerEl) {
      const el = document.createElement('div');
      el.id = 'JsCoreComponentsModal-container';
      containerEl = document.body.appendChild(el);
    }

    return containerEl;
  }


  // convenience method for a generic alert JsCoreComponentsModal
  static alert(userOptions = {}) {
    return JsCoreComponentsModal._buildPromiseModal(
      JsCoreComponentsModal._mergeModalOptions('alert', userOptions)
    );
  }


  // convenience method for an info style alert JsCoreComponentsModal
  static info(userOptions = {}) {
    return JsCoreComponentsModal._buildPromiseModal(
      JsCoreComponentsModal._mergeModalOptions('info', userOptions)
    );
  }


  // convenience method for a success style alert JsCoreComponentsModal
  static success(userOptions = {}) {
    return JsCoreComponentsModal._buildPromiseModal(
      JsCoreComponentsModal._mergeModalOptions('success', userOptions)
    );
  }


  // convenience method for a warning style alert JsCoreComponentsModal
  static warning(userOptions = {}) {
    return JsCoreComponentsModal._buildPromiseModal(
      JsCoreComponentsModal._mergeModalOptions('warning', userOptions)
    );
  }


  // convenience method for an danger style alert JsCoreComponentsModal
  static danger(userOptions = {}) {
    return JsCoreComponentsModal._buildPromiseModal(
      JsCoreComponentsModal._mergeModalOptions('danger', userOptions)
    );
  }


  // alternate method name for the danger() modal
  static error(userOptions = {}) {
    return JsCoreComponentsModal.danger(userOptions);
  }


  // convenience method for a confirmation JsCoreComponentsModal
  static confirm(userOptions = {}) {
    return JsCoreComponentsModal._buildPromiseModal(
      JsCoreComponentsModal._mergeModalOptions('confirm', userOptions),
      'confirm'
    );
  }


  // convenience method for a prompt JsCoreComponentsModal
  static prompt(userOptions = {}) {
    const options = JsCoreComponentsModal._mergeModalOptions('prompt', userOptions);

    // if regex passed as pattern, convert to string first
    if (JsCoreComponentsModal._typeof(options.input?.pattern) === 'regexp') {
      options.input.pattern = options.input.pattern.source;
    }

    options.body = `
      ${options.body ? `<p>${options.body}</p>` : ''}
      ${typeof options.input === 'string' ? options.input :
        `<input
          type="${options.input.type}"
          class="form-control ${options.input.class}"
          id="${options.input.id}"
          value="${options.input.value}"
          ${options.input.title ? `title="${options.input.title}"` : ''}
          ${options.input.placeholder ? `placeholder="${options.input.placeholder}"` : ''}
          ${options.input.autocomplete ? `autocomplete="${options.input.autocomplete}"` : ''}
          ${typeof options.input.minlength === 'number' ? `minlength="${options.input.minlength}"` : ''}
          ${typeof options.input.maxlength === 'number' ? `maxlength="${options.input.maxlength}"` : ''}
          ${typeof options.input.pattern === 'string' && options.input.pattern.length ? `pattern="${options.input.pattern}"` : ''}
          ${options.input.required ? 'required' : ''}
        >`
      }
    `.trim();

    return JsCoreComponentsModal._buildPromiseModal(options, 'prompt');
  }


  /* expose native bootstrap modal methods */

  toggle() {
    this._modal.toggle();
  }

  show(relatedTarget = this._options.relatedTarget) {
    console.log(relatedTarget)
    this._modal.show(relatedTarget);
  }

  hide() {
    this._modal.hide();
  }

  handleUpdate() {
    this._modal.handleUpdate();
  }

  dispose() {
    this._modal.dispose();
  }

  static getInstance(modalEl) {
    console.log(modalEl)
    return JsCoreComponentsModal._bootstrapModal.getInstance(modalEl);
  }

  static getOrCreateInstance(modalEl) {
    console.log(modalEl)
    return JsCoreComponentsModal._bootstrapModal.getOrCreateInstance(modalEl);
  }

}
