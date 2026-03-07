/**
 * CategoryPicker 兩層彈跳選單
 *
 * @ Author BarrY
 * @ Version 2.0.2
 * @ Update 2022/09/22
 * @ 修正 --- 關閉視窗 BUG
 */

'use strict';

const JsCategoryPicker = function(element, config) {

  ///////////////////////////////
  // **  Private variables  ** //
  ///////////////////////////////
  const that = this;
  const body = document.getElementsByTagName("BODY")[0];

  if (typeof element === "undefined" || element === null) return;

  let defaultOptions = {
    data: config.data,
    dataArray: {},
    dataArraySub: {},
    prefix: config.prefix,
    prefixSub: config.prefixSub,
    selectAll: config.selectAll,
    selectAllPreTxt: config.selectAllPrefix,
    lv1Active: config.lv1Active,
    lv2Active: config.lv2Active,
    elHidden:  'elHidden',
    sl: config.selectedNum,
    id: config.id,
    title: config.title,
    confirm: {},
  };

  element = document.getElementById(element);
  config = JsUtils.deepExtend({}, defaultOptions, config);

  const dataArray = config.dataArray;
  const dataArraySub = config.dataArraySub;
  const selectAll = config.selectAll || true;
  const selectAllPreTxt = config.selectAllPreTxt || '全區';
  const prefix = config.prefix || 'category_';
  const prefixSub = config.prefixSub || 'sub_';
  const lv1Active = config.lv1Active || "category-item--active";
  const lv2Active = config.lv2Active || "list-level-two--focus";
  const data = config.data;
  const pickerID = config.id;
  const title = config.title || "地區類別選單";
  const picker = document.getElementById(pickerID);
  const elHidden = config.elHidden || pickerID + 'Hidden';
  // const picker = document.getElementById(pickerID);
  // console.log(elHidden)
  // console.log(element.id)
  const inputId = element.id;


  ////////////////////////////
  // ** Private methods  ** //
  ////////////////////////////
  const _construct = function() {
    if (JsUtils.data(element).has('category')) {
      that = JsUtils.data(element).get('category');
    } else {
      _init();
    }
  };

  const _init = function() {
    that.uid = JsUtils.getUniqueId('category_picker');
    _loadDate(data);
    _inPutClick(element);

    JsUtils.data(element).set('category-picker', that);
  };

  const _loadDate = function(data) {
    JsUtils.fetch(data, 'GET')
      .then((responseData) => {
        return _build(responseData, pickerID, title);
      })
      .then(() => {
        return _handlers();
      })
      .then(() => {
        return _confirm(inputId)
      })
      .catch((error) => {
        return console.log('資料載入失敗!!');
      });
  };

  // 產生選單
  const _build = function(data, id, title) {

    const dataArrayLen = dataArray.length;
    const dataArraySubLen = dataArraySub.length;

    let maps = {};
    let newArrObj = [];
    for (let i = 0; i < data.length; i++) {
      let newData = data[i];
      if (!maps[newData[dataArray[0]]]) {
        newArrObj.push({
          ID: prefix + newData[dataArray[0]],
          [dataArray[0]]: newData[dataArray[0]],
          [dataArray[1]]: newData[dataArray[1]],
          DATAS: []
        });
        maps[newData[dataArray[0]]] = newData;
      }
      for (let j = 0; j < newArrObj.length; j++) {
        // console.log(i)
        let dataJson = newArrObj[j];
        if (dataJson[dataArray[0]] == newData[dataArray[0]]) {
          dataJson.DATAS.push({
            ID: prefix + prefixSub + (newData.ZIPCODE || i),
            // ID: prefix + prefixSub + i ,
            [dataArray[0]]: newData[dataArray[0]],
            [dataArraySub[0]]: newData[dataArraySub[0]],
            [dataArraySub[1]]: newData[dataArraySub[1]]
          });
          break;
        }
      }
    }
    // console.log(newArrObj)
    const lv1_fragment = document.createDocumentFragment();
    const lv2_fragment = document.createDocumentFragment();

    let lv1_ul = document.createElement('ul');
    lv1_ul.id = prefix + "lv1";
    lv1_ul.className = "list-level-one";

    let lv2_ul = document.createElement('ul');
    lv2_ul.id = prefix + "lv2";
    lv2_ul.className = "list-level-two-cnt";

    for (let k = 0; k < newArrObj.length; k++) {

      const lv1LiLab = newArrObj[k][dataArray[1]];
      const lv1LiId = prefix + "lv1_" + newArrObj[k][dataArray[0]];
      const lv2UlId = prefix + "lv2_" + newArrObj[k][dataArray[0]];

      // console.log(lv2UlId)
      let lv1_li = document.createElement("li");
      lv1_li.id = lv1LiId;
      lv1_li.className = "lv1 category-item";
      lv1_li.setAttribute("data-targets", lv2UlId);
      lv1_li.setAttribute("aria-label", lv1LiLab);

      let lv1_html = '<a href="javascript:;" class="category-item-txt">' + lv1LiLab + '</a>';
      lv1_li.innerHTML = lv1_html;
      lv1_fragment.appendChild(lv1_li);

      let lv2_ul_ul = document.createElement("ul");
      lv2_ul_ul.className = "list-level-two";
      lv2_ul_ul.id = lv2UlId;
      lv2_ul_ul.setAttribute("data-parents", lv1LiId);
      lv2_fragment.appendChild(lv2_ul_ul);

      if (selectAll === true) {
        const lv2LiLabAll = newArrObj[k][dataArray[1]] + selectAllPreTxt;
        const lv2InputId = prefix + prefixSub + newArrObj[k].DATAS[0][dataArray[0]];
        let lv2_liAll = document.createElement("li");
        lv2_liAll.className = "lv2 category-item";
        lv2_liAll.setAttribute("data-parents", lv2UlId);
        var lv2All_html =
          '<input name="' + pickerID + prefix + 'ckeckbox" type="checkbox" id="' + lv2InputId + '_All">' +
          '<label class="t-checkbox-group" tabindex="0" for="' + lv2InputId + '_All">' +
          '' + lv2LiLabAll + '' +
          '</label>';
        lv2_liAll.innerHTML = lv2All_html;
        lv2_ul_ul.appendChild(lv2_liAll);
      }

      for (let m = 0; m < newArrObj[k].DATAS.length; m++) {

        const lv2LiLab = newArrObj[k].DATAS[m][dataArraySub[1]]
        const lv2LiId = newArrObj[k].DATAS[m].ID;

        if (newArrObj[k][dataArray[0]] === newArrObj[k].DATAS[m][dataArray[0]]) {
          let lv2_li = document.createElement("li");
          lv2_li.className = "lv2 category-item";
          lv2_li.setAttribute("data-parents", lv2UlId);
          var lv2_html =
            '<input name="' + pickerID + prefix + 'ckeckbox" type="checkbox" id="' + lv2LiId + '">' +
            '<label class="t-checkbox-group" tabindex="0" for="' + lv2LiId + '">' +
            '' + lv2LiLab + '' +
            '</label>';
          lv2_li.innerHTML = lv2_html;
          lv2_ul_ul.appendChild(lv2_li);
        }
      }
    }

    lv1_ul.append(lv1_fragment);
    lv2_ul.append(lv2_fragment);
    let _blankv1 = document.createElement('div');
    let _blankv2 = document.createElement('div');
    _blankv1.append(lv1_ul);
    _blankv2.append(lv2_ul);
    let lv1_ul_Html = _blankv1.innerHTML;
    let lv2_ul_Html = _blankv2.innerHTML;

    let modal = document.createElement('div');
    modal.id = pickerID;
    modal.className = "category-picker close";
    let html = '';
    html += '   <div class="category-mask"></div>';
    html += '   <div class="category-modal">';
    html += '       <div class="category-modal-cnt">';
    html += '           <div class="category-modal-header">';
    html += '               <h4 class="category-modal-header-txt">' + title + '</h4>';
    html += '               <button id="' + pickerID + prefix + 'close" type="button" class="btn-close" tabindex="0" aria-label="關閉' + title + '" title="關閉' + config.title + '"></button>';
    html += '           </div>';
    html += '           <div id="' + pickerID + 'selectedCnt" class="category-modal-selected">';
    html += '               <span class="selected-txt"> 已選擇 ( <span id="' + pickerID + 'catNums" class="selectedNum">0</span> )</span>';
    html += '               <a class="selected-del-all" aria-label="清空全部標籤" title="清空全部標籤" tabindex="0">清空全部標籤</a>';
    html += '           </div>';
    html += '           <div class="category-modal-body">';
    html += '             <form name="' + pickerID + prefix + '_form" action="" method="get" id="' + pickerID + prefix + '_form">';
    html += '               ' + lv1_ul_Html + ' ';
    html += '               ' + lv2_ul_Html + ' ';
    html += '             </form>';
    html += '           </div>';
    html += '           <div class="category-modal-footer">';
    html += '               <button id="' + pickerID + prefix + 'footer-close" class="btn btn--border--primary category-close-btn" tabindex="0" aria-label="關閉視窗" title="關閉視窗">關閉視窗</button>';
    html += '               <button class="btn btn--primary category-confirm-btn" tabindex="0" aria-label="選擇完畢" title="選擇完畢">選擇完畢</button>';
    html += '           </div>';
    html += '       </div>';
    html += '   </div>';

    modal.innerHTML = html;
    document.getElementsByTagName("BODY")[0].appendChild(modal);
    document.getElementById("" + prefix + "lv1_1").classList.add(lv1Active);
    document.querySelector("[data-parents='" + prefix + "lv1_1']").classList.add(lv2Active);

    let newInput = document.createElement("input");
    newInput.type = 'hidden';
    newInput.id = elHidden;
    newInput.value = '';

    JsUtils.insertBefore(newInput, document.getElementById(inputId))
  };

  // 鍵盤事件
  const _keydown = function() {
    let elementPicker = document.getElementById(pickerID);
    let focusableEls = JsUtils.makeArray(elementPicker.querySelectorAll('a[href]:not([disabled]), button:not([disabled]), textarea:not([disabled]), input[type="text"]:not([disabled]), input[type="radio"]:not([disabled]), input[type="checkbox"]:not([disabled]), select:not([disabled])'));
    let firstFocusableEl = focusableEls[0];
    let lastFocusableEl = focusableEls[focusableEls.length - 1];
    // let currentFocus = null;
    // firstFocusableEl.focus();
    elementPicker.addEventListener('keydown', function(e) {
      let lv1Cnt = elementPicker.getElementsByClassName('list-level-one')[0];
      let lv2Cnt = elementPicker.getElementsByClassName('list-level-two--focus')[0];
      switch (e.keyCode) {
        case 37: // 左
          if (e.target.className === 't-checkbox-group') {
            lv1Cnt.querySelector('.category-item--active .category-item-txt').focus();
            e.preventDefault();
          }
          break;
        case 38: // 上

          break;
        case 39: // 右
          if (e.target.className === 'category-item-txt') {
            lv2Cnt.querySelector('.t-checkbox-group').focus();
            e.preventDefault();
          }
          break;
        case 40: // 下

          break;
        case 27: // ESC
          _close(pickerID)
          break;
        case 13: // ENTER
          if (e.target.className === 't-checkbox-group') {
            e.target.click();
            e.preventDefault();
          }
          break;
        case 32: // 空白
          if (e.target.className === 't-checkbox-group') {
            e.target.click();
            e.preventDefault();
          }
          break;
        case 9: // TAB
          if (e.shiftKey) {
            if (document.activeElement === firstFocusableEl) {
              lastFocusableEl.focus();
              e.preventDefault();
            }
          } else {
            if (document.activeElement === lastFocusableEl) {
              firstFocusableEl.focus();
              e.preventDefault();
            }
          }
          break;
      }
    });
  }


  // 開啟選單
  const _inPutClick = function(element) {

    let _openModal = function() {
      let picker = document.getElementById(pickerID);
      let hidden = document.getElementById(elHidden);
      if (hidden.value !== '') {
        let hiddenVal = hidden.value.split(",");
        for (let i = 0; i < hiddenVal.length; i++) {
          _select(hiddenVal[i])
        }
      }
       console.dir(element)
      _open();
      _keydown();

      setTimeout(() => picker.querySelector('button').focus(), 200);
    };

    JsUtils.addEvent(element, 'click', function(event) {
      event.preventDefault();
      _openModal()
    });

    element.addEventListener('keydown', function(event) {

      switch (event.keyCode) {
        case 13: // ENTER
          if (event.target.id === element.id) {
            _openModal()
            event.preventDefault();
          }
          break;
        case 32: // 空白
          if (event.target.id === element.id) {
            _openModal()
            event.preventDefault();
          }
          break;
      }
    });

  };

  // 確認按鈕
  const _confirm = function(inputId) {
    var picker = document.getElementById(pickerID);
    const confirmBtn = picker.getElementsByClassName('category-confirm-btn')[0];
    const selectedCnt = document.getElementById(pickerID + 'selectedCnt');
    const input = document.getElementById(inputId);
    var inputHidden = document.getElementById(elHidden);

    JsUtils.addEvent(confirmBtn, 'click', function(event) {
      if (input.tagName === 'DIV') {
        input.querySelector('input').classList.add('t-absolute');
        selectedCnt.querySelectorAll('.selected-label').forEach(function(item, index) {
          let copyItem = item.cloneNode(true);
          let copyItemHtml = copyItem.innerHTML;
          input.appendChild(copyItem)
        });
      }
      if (input.tagName === 'INPUT') {
        let newValgroup = [];
        let newParenID = [];
        let newGroupID = [];

        selectedCnt.querySelectorAll('.selected-label').forEach(function(item, index) {
          input.value = '';
          inputHidden.value = '';
          let name = item.getAttribute('data-name');
          let parenID = item.getAttribute('data-paren-id');
          let group = item.getAttribute('data-group');
          newValgroup.push(name);
          newParenID.push(parenID);
          newGroupID.push(group);
          // console.log(newValgroup);
          // console.log(newParenID);
          // console.log(inputHidden);
        });

        inputHidden.value += newParenID;
        input.value += newValgroup;
      }
      _close(pickerID);
    });
  };

  // 事件
  const _handlers = function() {
    const pickerLv1Cnt = document.getElementById(prefix + "lv1");
    const pickerSubCnt = document.getElementById(prefix + "lv2");
    const checkedBoxInps = pickerSubCnt.getElementsByTagName("input");
    const checkedBoxInpsLen = checkedBoxInps.length;
    const selectedCnt = document.getElementById(pickerID + "selectedCnt");

    // Picker Lv1 Cnt Click
    JsUtils.addEvent(pickerLv1Cnt, 'click', function(event) {
      event.preventDefault();
      const targetItem = event.target;
      if (targetItem.tagName === "A") {

        // console.dir(targetItem)
        pickerLv1Cnt.querySelector("." + lv1Active).classList.remove(lv1Active);
        let targetItemID = targetItem.parentElement.getAttribute("id");
        targetItem.parentElement.classList.add(lv1Active);
        pickerSubCnt.querySelector("." + lv2Active).classList.remove(lv2Active);
        pickerSubCnt.querySelector(".list-level-two[data-parents=" + targetItemID + "]").classList.add(lv2Active);
        return this
      }
    });

    // Close Click
    const closeBtn = document.getElementById(pickerID + prefix + 'close');
    JsUtils.addEvent(closeBtn, 'click', function(event) {
      event.preventDefault();
      console.log(pickerID)
      _close(pickerID);
      return this;
    });

    // Close Footer Click
    const closeBtnFooter = document.getElementById(pickerID + prefix + 'footer-close');
    JsUtils.addEvent(closeBtnFooter, 'click', function(event) {
      event.preventDefault();
      _close(pickerID);
      return this
    });

    // inPut Lv2 Cnt Checked
    for (let i = 0; i < checkedBoxInpsLen; i++) {
      JsUtils.addEvent(checkedBoxInps[i], 'change', function(event) {
        const picker = document.getElementById(pickerID);
        const targetItem = event.target;
        // Const lv1TarId = targetItem.parentElement.getAttribute("data-targets");
        const liParId = targetItem.parentElement.getAttribute("data-parents");
        const thisID = targetItem.id;
        const thisText = targetItem.nextElementSibling.textContent;
        const catNums = document.getElementById(pickerID + 'catNums');
        let item =
          '<label class="selected-label" data-group="' + liParId + '" data-paren-id="' + thisID + '" data-name="' + thisText + '" tabindex="0" aria-label="' + thisText + '" >' +
          '  <span>' + thisText + '</span>' +
          '  <a aria-label="close" class="selected-label-close"></a>' +
          '</label>';
        let labs = selectedCnt.getElementsByClassName("selected-label");
        if (picker.querySelectorAll('[data-paren-id=' + thisID + ']').length === 0) {
          selectedCnt.innerHTML += item;
        } else {
          picker.querySelector('[data-paren-id=' + thisID + ']').remove();
        }

        if (picker.querySelectorAll('[data-group=' + liParId + ']').length != 0) {
          picker.querySelector('[data-targets=' + liParId + '] .category-item-txt').classList.add('category-item-txt--has')
        } else {
          picker.querySelector('[data-targets=' + liParId + '] .category-item-txt').classList.remove('category-item-txt--has')
        }

        _delAllClcik();
        _labelClcik();
        _update();

      });
    }

    // .indeterminate = true
    // 上方標籤區域 label click funcs
    // 第二層選單 全選項目設定
    if (selectAll) {
      let pickerSubUl = pickerSubCnt.getElementsByTagName('ul');
      let pickerSubUlLen = pickerSubUl.length;
      let labels = document.getElementsByClassName("selected-label-close")

      // 右邊選單全選設定
      for (let i = 0; i < pickerSubUlLen; i++) {
        JsUtils.addEvent(pickerSubUl[i].firstElementChild, 'change', function(event) {
          event.preventDefault();
          const targetItem = event.target;
          if (targetItem.tagName === "INPUT") {
            _checkAll(targetItem)
            _update()
          }
        });
      }
    }
  };


  const _labelClcik = function() {
    let picker = document.getElementById(pickerID);
    let labels = picker.getElementsByClassName("selected-label-close");
    for (let i = 0; i < labels.length; i++) {
      JsUtils.addEvent(labels[i], 'click', function(event) {
        event.preventDefault();
        const targetItem = event.target;
        let liTems = targetItem.parentElement;
        let pId = liTems.attributes['data-paren-id'].nodeValue;
        let gId = liTems.attributes['data-group'].nodeValue;
        let this_label = event.target.parentElement;
        let lv2_Select = this_label.getAttribute("data-paren-id");
        if (/All/gi.test(pId)) {
          _checkAll(pId);
          _del(this_label, lv2_Select);
          _update();
        } else {
          _update();
          if (picker.querySelectorAll('[data-group=' + gId + ']').length == 1) {
            picker.querySelector('[data-targets=' + gId + '] .category-item-txt').classList.remove('category-item-txt--has')
          }
          return _del(this_label, lv2_Select)
        }
      });
    };
  };

  const _delAllClcik = function() {
    let picker = document.getElementById(pickerID);
    let labels = picker.getElementsByClassName("selected-label-close");
    let delAll = picker.getElementsByClassName("selected-del-all")[0];
    for (let i = 0; i < labels.length; i++) {
      JsUtils.addEvent(delAll, 'click', function(event) {
        let labels = picker.getElementsByClassName("selected-label-close");
        if (labels.length === 0 || labels.length == null) {
          return
        }
        event.preventDefault();
        let liTems = labels[0].parentElement;
        let pId = liTems.attributes['data-paren-id'].nodeValue;
        let gId = liTems.attributes['data-group'].nodeValue;
        let this_label = liTems;
        let lv2_Select = this_label.getAttribute("data-paren-id");
        if (/All/gi.test(pId)) {
          _checkAll(pId);
          _update();
          _del(this_label, lv2_Select);
        } else {
          _update();
          if (picker.querySelectorAll('[data-group=' + gId + ']').length == 1) {
            picker.querySelector('[data-targets=' + gId + '] .category-item-txt').classList.remove('category-item-txt--has')
          }
          return _del(this_label, lv2_Select)
        }
      });
    };
  };

  const _checkAll = function(el) {
    const picker = document.getElementById(pickerID);

    if (typeof el === 'string') {
      var el = picker.querySelector('#' + el);
      var parId = el.parentElement.getAttribute("data-parents");
      var labs = document.querySelectorAll('[data-group="' + parId + '"]');
      el.click()
      _update()
    }
    const thisUl = JsUtils.parents(el, '.list-level-two')[0];
    const thisLi = thisUl.getElementsByTagName('li');
    const thisLen = thisLi.length;
    for (let x = 1; x < thisLen; x++) {
      let checkBox = thisLi[x];
      let allInput = JsUtils.children(checkBox, 'input')[0];
      if (el.type === "checkbox" && el.checked) {
        let parId = el.parentElement.getAttribute("data-parents");
        let labs = document.querySelectorAll('[data-group="' + parId + '"]');
        labs.forEach(function(element, index) {
          let parID = element.getAttribute("data-paren-id");
          if (!/All/gi.test(parID)) {
            element.remove();
          }
        });
        allInput.indeterminate = true;
        allInput.checked = true;
        allInput.disabled = true;
        _update()
      } else {
        allInput.indeterminate = false;
        allInput.checked = false;
        allInput.disabled = false;
        let inputID = allInput.id;
        _update()
      }
    }
  };

  const _update = function() {
    const picker = document.getElementById(pickerID);
    const catNums = document.getElementById(pickerID + 'catNums');
    const selectedCnt = document.getElementById(pickerID + 'selectedCnt');
    let num = picker.querySelectorAll('input[type="checkbox"]:checked').length;;
    catNums.textContent = num;
    for (let i = 0; i < num; i++) {
      let labId = picker.querySelectorAll('input[type="checkbox"]:checked')[i].getAttribute('id');
      if (!picker.querySelectorAll('[data-paren-id="' + labId + '"]')) {
        picker.querySelectorAll('.selected-label')[i].remove();
      }
    }
  };

  const _select = function(id) {
    let picker = document.getElementById(pickerID);
    let selected = document.getElementById(id);
    console.log(selected)
    if (selected.checked === false) {
      selected.click();
    };
  };

  const _open = function() {
    that._show = true;
    let picker = document.getElementById(pickerID);
    body.classList.add("modal-open");
    picker.classList.add("show");
    _keydown();
  };

  const _close = function( ) {
    that._show = false; 
    let picker = document.getElementById(pickerID);
    let hidden = document.getElementById(elHidden);
    console.log(that)
    picker.classList.remove("show");
    body.classList.remove("modal-open");
    element.focus();
  };

  const _del = function(select, label) {
    let picker = document.getElementById(pickerID);
    let labels = picker.querySelectorAll('[data-paren-id="' + label + '"]');
    if (labels.length !== 0 || labels != null) {
      for (let i = 0; i < labels.length; i++) {
        picker.querySelector('#' + label).checked = false;
        labels[i].remove();
      }
    } else {

    }
    _update();
  };

  const _getOption = function(name) {

    if (that.element.hasAttribute('data-tu-category-' + name) === true) {
      let attr = that.element.getAttribute('data-tu-category-' + name);
      let value = JsUtils.getResponsiveValue(attr);

      if (value !== null && String(value) === 'true') {
        value = true;
      } else if (value !== null && String(value) === 'false') {
        value = false;
      }

      return value;
    } else {
      var optionName = JsUtils.snakeToCamel(name);

      if (that.options[optionName]) {
        return JsUtils.getResponsiveValue(that.options[optionName]);
      } else {
        return null;
      }
    }
  };

  const _destroy = function() {
    JsUtils.data(that.element).remove('JsCategoryPicker');
  };

  _construct();

  ///////////////////////
  // ** Public API  ** //
  ///////////////////////

  that.open = function() {
    return _open();
  };

  that.close = function() {
    return _close();
  };

  that.destroy = function() {
    return _destroy();
  };

  // Event API
  that.on = function(name, handler) {
    // console.log(this)
    return JsEventHandler.on(that.element, name, handler);
  }

  that.one = function(name, handler) {
    // console.log(this)
    return JsEventHandler.one(that.element, name, handler);
  }

  that.off = function(name) {
    // console.log(this)
    return JsEventHandler.off(that.element, name);
  }

  that.trigger = function(name, event) {
    // console.log(this)
    return JsEventHandler.trigger(that.element, name, event, the, event);
  }
}

// Static
JsCategoryPicker.getInstance = function(element) {
  if (element && JsUtils.data(element).has('category')) {
    return JsUtils.data(element).get('category');
  } else {
    return null;
  }
};

// Create
JsCategoryPicker.createInstances = function() {
  let selector = arguments.length > 0 && arguments[0] !== undefined ? arguments[0] : '[data-tu-category="true"]';
  let body = document.getElementsByTagName("BODY")[0]; // Initialize Menus

  let elements = body.querySelectorAll(selector);
  let category;

  if (elements && elements.length > 0) {
    for (let i = 0, len = elements.length; i < len; i++) {
      category = new JsCategoryPicker(elements[i]);
    }
  }
};

// Global
JsCategoryPicker.init = function() {
  JsCategoryPicker.createInstances();
};

// On document ready
if (document.readyState === 'loading') {
  document.addEventListener('DOMContentLoaded', JsCategoryPicker.init);
} else {
  JsCategoryPicker.init();
}
// Webpack support
if (typeof module !== 'undefined' && typeof module.exports !== 'undefined') {
  module.exports = JsCategoryPicker;
}