// <elastic-textarea>
//   <label>
//     Textarea 1
//     <textarea name="textarea-1"></textarea>
//   </label>
// </elastic-textarea>
customElements.define(
"elastic-textarea",
class extends HTMLElement {
  connectedCallback() {
    // console.log([document.querySelectorAll("textarea")][0]);

    [document.querySelectorAll("textarea")][0].forEach(textareaEl => {
      textareaEl.getAttribute("data-min-rows", textareaEl.rows || 2);

      this.update(textareaEl);
    });

    this.addEventListener("input", ({ target }) => {
      if (!(target instanceof HTMLTextAreaElement)) return;

      this.update(target);
    });
  }

  isScrolling(textareaEl) {
    return textareaEl.scrollHeight > textareaEl.clientHeight;
  }

  grow(textareaEl) {
    let previousHeight = textareaEl.clientHeight;
    let rows = this.rows(textareaEl);

    while (this.isScrolling(textareaEl)) {if (window.CP.shouldStopExecution(0)) break;
      rows++;
      textareaEl.rows = rows;

      const newHeight = textareaEl.clientHeight;

      if (newHeight === previousHeight) break;

      previousHeight = newHeight;
    }window.CP.exitedLoop(0);
  }

  shrink(textareaEl) {
    let previousHeight = textareaEl.clientHeight;

    const minRows = parseInt(textareaEl.dataset.minRows);
    let rows = this.rows(textareaEl);

    while (!this.isScrolling(textareaEl) && rows > minRows) {if (window.CP.shouldStopExecution(1)) break;
      rows--;
      textareaEl.rows = Math.max(rows, minRows);

      const newHeight = textareaEl.clientHeight;

      if (newHeight === previousHeight) break;

      if (this.isScrolling(textareaEl)) {
        this.grow(textareaEl);
        break;
      }
    }window.CP.exitedLoop(1);
  }

  update(textareaEl) {
    if (this.isScrolling(textareaEl)) {
      this.grow(textareaEl);
    } else {
      this.shrink(textareaEl);
    }
  }

  rows(textareaEl) {
    return (
      textareaEl.rows ||
      parseInt(textareaEl.getAttribute("data-min-rows", textareaEl.rows)));

  }});