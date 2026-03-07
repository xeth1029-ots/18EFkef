/**
 * CategoryPicker 兩層彈跳選單
 *
 * @ Author BarrY
 * @ Version 2.0.2
 * @ Update 2022/09/02
 * @ 修正 --- 關閉視窗 BUG
 */

'use strict';
// 選取能夠 focus 的標籤
var focusableElements = JsUtils.findAll(JsUtils.Quer('html'), 'a ,button ,input ,select');
// console.dir(focusableElements);
// .not('.category-del-all')
// .not('.category-close')
// .not('.category-close-btn')
// .not('.category-confirm-btn');



// $.CategoryPicker.init({
//   el: '#area', // Input ID 點擊啟用
//   elHiddenid: '.areaVal', // Input Hidden
//   id: 'areaPicker', // Picker ID
//   omitNum: 5, // 幾個選項以上 顯示單位 
//   unit: '地區', // 5 個選項以上顯示 已選擇幾個的單位
//   selectedNum: 10, // 可選擇幾個標籤
//   data: cities.info, // 代入縣市資訊
//   selectAll: true, // 二層選單是否要開啟第一個全選 預設 false
//   title: '地區類別選單',
//   confirm: function(res) {
//     // 回傳 Input hidden
//     if (res.length) {
//       for (var k in res) {
//         $('.areaVal').eq(k).val(res[k].value);
//         $('.areaVal').eq(k).attr('name', 'area[' + res[k].uid + ']');
//       }
//     } else {
//       $('.areaVal').val('');
//       $('.areaVal').attr('name', 'area[]');
//     }
//   }
// });


const JsCategoryPicker = function(element, config) {
    ///////////////////////////////
    // **  Private variables  ** //
    ///////////////////////////////
    var that = this;
    var body = document.getElementsByTagName("BODY")[0];

    if (typeof element === "undefined" || element === null) return;
    // var element = document.getElementById(that.element)
    // console.log(this);
    var defaultOptions = {
      data: config.data,
      dataArray: {},
      dataArraySub: {},
      prefix: config.prefix,
      prefixSub: config.prefixSub,
      selectAll: config.selectAll,
      selectAllPreTxt: config.selectAllPrefix,
      lv1Active: config.lv1Active,
      lv2Active: config.lv2Active,
      // el: document.getElementById(config.el),
      elHidden: document.getElementById(config.elHiddenid),
      sl: config.selectedNum,
      oN: config.omitNum,
      id: config.id,
      title: config.title,
      unit: config.unit,
    };

    element = document.getElementById(element);

    var config = Object.assign({}, that.defaultOptions, config || {});

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

    ////////////////////////////
    // ** Private methods  ** //
    ////////////////////////////
    var _construct = function _construct() {
      if (JsUtils.data(element).has('category')) {
        that = JsUtils.data(element).get('category');
      } else {
        _init();
      }
    };

    var _init = function _init() {
      that.uid = JsUtils.getUniqueId('category_picker');
      _loadDate(data);
      JsUtils.data(element).set('category-picker', that);
    };

    var _loadDate = function(data) {
      JsUtils.fetch(data, 'GET')
        .then((responseData) => {
          return _build(responseData, pickerID, title);
        })
        .then(() => {
          return _handlers();
        })
        .catch((error) => {
          return console.log('資料載入失敗!!');
        });
    };

    var _build = function(data, id, title) {

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
          let dataJson = newArrObj[j];
          if (dataJson[dataArray[0]] == newData[dataArray[0]]) {
            dataJson.DATAS.push({
              ID: prefix + prefixSub + newData.ZIPCODE,
              [dataArray[0]]: newData[dataArray[0]],
              [dataArraySub[0]]: newData[dataArraySub[0]],
              [dataArraySub[1]]: newData[dataArraySub[1]]
            });
            break;
          }
        }
      }
      console.log(newArrObj);

      const lv1_fragment = document.createDocumentFragment();
      const lv2_fragment = document.createDocumentFragment();

      let lv1_ul = document.createElement('ul');
      lv1_ul.id = prefix + "lv1";
      lv1_ul.className = "list-level-one";

      let lv2_ul = document.createElement('ul');
      lv2_ul.id = prefix + "lv2";
      lv2_ul.className = "list-level-two-cnt";
      // lv2_ul.setAttribute("data-parents", prefix + "lv2")

      for (let k = 0; k < newArrObj.length; k++) {
        let lv1_li = document.createElement("li");
        lv1_li.id = prefix + "lv1_" + newArrObj[k].CTID;
        lv1_li.className = "lv1 category-item";
        lv1_li.setAttribute("data-targets", prefix + "lv2_" + newArrObj[k].CTID);
        lv1_li.setAttribute("aria-label", newArrObj[k].CTNAME);

        let lv1_html = '<a href="javascript:;" class="category-item-txt">' + newArrObj[k].CTNAME + '</a>';

        lv1_li.innerHTML = lv1_html;
        lv1_fragment.appendChild(lv1_li);

        let lv2_ul_ul = document.createElement("ul");
        lv2_ul_ul.className = "list-level-two";
        lv2_ul_ul.id = prefix + "lv2_" + newArrObj[k].CTID;
        lv2_ul_ul.setAttribute("data-parents", prefix + "lv1_" + newArrObj[k].CTID);
        lv2_fragment.appendChild(lv2_ul_ul);
        if (selectAll === true) {
          let lv2_liAll = document.createElement("li");
          lv2_liAll.className = "lv2 category-item";
          lv2_liAll.setAttribute("data-parents", prefix + "lv2_" + newArrObj[k].CTID);
          var lv2All_html =
            '<input type="checkbox" id="' + prefix + prefixSub + newArrObj[k].DATAS[0].CTID + '_All">' +
            '<label class="t-checkbox-group" tabindex="0" for="' + prefix + prefixSub + newArrObj[k].DATAS[0].CTID + '_All">' +
            '' + newArrObj[k].CTNAME + selectAllPreTxt + '' +
            '</label>';
          lv2_liAll.innerHTML = lv2All_html;
          lv2_ul_ul.appendChild(lv2_liAll);
        }



        for (let m = 0; m < newArrObj[k].DATAS.length; m++) {
          if (newArrObj[k].CTID === newArrObj[k].DATAS[m].CTID) {
            console.log(newArrObj[k].DATAS)
            let lv2_li = document.createElement("li");
            lv2_li.className = "lv2 category-item";
            lv2_li.setAttribute("data-parents", prefix + "lv2_" + newArrObj[k].CTID);
            // let lv2_html = '<a href="javascript:;"  class="category-item-txt">' + newArrObj[k].DATAS[m].ZIPNAME + '</a>';
            var lv2_html =
              '<input type="checkbox" id="' + newArrObj[k].DATAS[m].ID + '">' +
              '<label class="t-checkbox-group" tabindex="0" for="' + newArrObj[k].DATAS[m].ID + '">' +
              '' + newArrObj[k].DATAS[m].ZIPNAME + '' +
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
      html += '               <button id="' + prefix + 'Close" type="button" class="category-close" tabindex="0" aria-label="關閉' + title + '" title="關閉' + config.title + '"></button>';
      html += '           </div>';
      html += '           <div class="category-modal-selected">';
      html += '               <span>已選擇 ( <span class="selectedNum">0</span> )</span>';
      html += '               <a class="category-del-all" aria-label="清空全部標籤" title="清空全部標籤" tabindex="0">清空全部標籤</a>';
      html += '           </div>';
      html += '           <div class="category-picker-selectedbox"></div>';
      html += '           <div class="category-modal-body">';
      html += '               ' + lv1_ul_Html + ' ';
      html += '               ' + lv2_ul_Html + ' ';
      html += '           </div>';
      html += '           <div class="category-modal-footer">';
      html += '               <button class="btn btn--border--primary category-close-btn" tabindex="0" aria-label="關閉視窗" title="關閉視窗">關閉視窗</button>';
      html += '               <button class="btn btn--primary category-confirm-btn" tabindex="0" aria-label="選擇完畢" title="選擇完畢">選擇完畢</button>';
      html += '           </div>';
      html += '       </div>';
      html += '   </div>';


      modal.innerHTML = html;
      document.getElementsByTagName("BODY")[0].appendChild(modal);
      document.getElementById("" + prefix + "lv1_1").classList.add(lv1Active);
      document.querySelector("[data-parents='" + prefix + "lv1_1']").classList.add(lv2Active);


      // var products = [{
      //   name: "Mathematics",
      //   category: "Subject"
      // }, {
      //   name: "Cow",
      //   category: "Animal"
      // }, {
      //   name: "Science",
      //   category: "Subject"
      // }];

      // var groupByCategory = products.reduce(function(group, product) {
      //   var _group$category;

      //   var category = product.name;

      //   group[category] = (_group$category = group[name]) != null ? _group$category : [];
      //   group[category].push(product);
      //   return group;
      // }, {});

      // console.log(groupByCategory);



      var groupByCategory = JsUtils.groupByProps(data, 'CTID');

      console.log(groupByCategory);

    };


    var _handlers = function() {
      const pickerLv1 = document.getElementById(prefix + "lv1");
      const pickerSub = document.getElementById(prefix + "lv2");

      JsUtils.addEvent(pickerLv1, 'click', function(event) {
        event.preventDefault();
        const targetItem = event.target;
        if (targetItem.tagName === "A") {
          // console.dir(lv1Active)
          // console.dir(targetItem)
          pickerLv1.querySelector("." + lv1Active).classList.remove(lv1Active);
          let targetItemID = targetItem.parentElement.getAttribute("id");
          targetItem.parentElement.classList.add(lv1Active);
          pickerSub.querySelector("." + lv2Active).classList.remove(lv2Active);
          pickerSub.querySelector(".list-level-two[data-parents=" + targetItemID + "]").classList.add(lv2Active);
          return this
        }
      });

      JsUtils.addEvent(element, 'click', function(event) {
        event.preventDefault();
        _open(pickerID)
        return this
      });

      var closeBtn = document.getElementById(prefix + 'Close')
      JsUtils.addEvent(closeBtn, 'click', function(event) {
        event.preventDefault();
        _close(closeBtn)
        return this
      });

    };

    var _open = function() {
      // console.log(pickerID)
      let picker = document.getElementById(pickerID);

      picker.classList.add("show");
      // picker.classList.remove("close");
      // JsUtils.fadeIn(picker)
    };

    var _close = function() {
      let picker = document.getElementById(pickerID);

      picker.classList.remove("show");
      // picker.classList.add("close");

      // JsUtils.fadeOut(picker)

    };

    var _getOption = function(name) {
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

    var _destroy = function() {
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
    that.getElement = function() {
      return that.element;
    };

    that.destroy = function() {
      return _destroy();
    };


    // Event API
    that.on = function(name, handler) {
      console.log(this)
      return JsEventHandler.on(the.element, name, handler);
    }

    that.one = function(name, handler) {
      console.log(this)
      return JsEventHandler.one(the.element, name, handler);
    }

    that.off = function(name) {
      console.log(this)
      return JsEventHandler.off(the.element, name);
    }

    that.trigger = function(name, event) {
      console.log(this)
      return JsEventHandler.trigger(the.element, name, event, the, event);
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
      var selector = arguments.length > 0 && arguments[0] !== undefined ? arguments[0] : '[data-tu-category="true"]';
      var body = document.getElementsByTagName("BODY")[0]; // Initialize Menus

      var elements = body.querySelectorAll(selector);
      var category;

      if (elements && elements.length > 0) {
        for (var i = 0, len = elements.length; i < len; i++) {
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



  }
  // return {
  //   init: function((config) {



//       $(window).on('load', function(e) {

//         var picker = $('#' + id);

//         elHidden.each(function() {
//           var val = $(this).val();
//           picker.find('input[value="' + val + '"]').click();

//         });

//         var selected = [];
//         var strSelected = '';
//         var lum = picker.find('.category-picker-selectedbox-item').length;

//         picker.find('.category-picker-selectedbox-item').each(function() {
//           var _this = $(this)
//           var name = _this.data('name');
//           var id = _this.data('id');
//           var uid = _this.data('uid');

//           selected.push({
//             name: name,
//             value: id,
//             uid: uid
//           });
//           strSelected += name + '、';
//         });

//         strSelected = strSelected.substring(0, strSelected.length - 1);

//         if (el.is('input')) {
//           if (lum >= oN) {
//             el.val('已經選擇 ' + lum + ' 個' + unit);
//             // console.log(strSelected.length)
//           } else {
//             el.val(strSelected);
//           }
//         } else {
//           if (strSelected == '') {
//             el.text('');
//             el.val('');
//           } else {
//             el.text(strSelected);
//             el.val(strSelected);
//           }
//         }
//       });

//       // 產生 HTML
//       this._bulid(data, id, title);

//       // slAll == true 開啟第一個是全選
//       if (slAll == true) {
//         this.slAll(id);
//       }
//       // 打開選單
//       var off = true;
//       if (el.is('input')) {
//         this.open(id, el, off, elHidden);
//       }
//       // 寬度 < 576 px
//       this.sizeModal(id);

//       // 第一層 點擊
//       this.clickLv1(id);

//       // 第二層 點擊
//       this.clickLv2(id, data, sl, config);

//       // 刪除所有標籤
//       this.delAllTags(id);

//       // 刪除標籤
//       this.delTags(id);

//       // 關閉選單
//       this.close(id, el, off);

//       // 儲存選單內容
//       this.confirmC(id, el, off, unit, oN, config);

//       // Enter 鍵盤 事件
//       this._keypress(id);


//     },

//     _bulid: function(data, id, title) {
//       var lv_1_parent = '';
//       var lv_2_parent = '';
//       for (var k in data) {

//         // 第 1 層 台北市全區、新北市全區等等...
//         lv_1_parent += '<ul class="list-level-one" data-parent="' + data[k].id + '"></ul>';

//         // 第 2 層 內容
//         for (var i in data[k].children) {
//           lv_2_parent += '<ul class="list-level-two" data-parent="' + data[k].children[i].id + '">';
//           lv_2_parent += '    <h2 class="category-item--title">' + data[k].children[i].name + '</h2>';
//           lv_2_parent += '</ul>';
//         }
//       }

//       var html = '';

//       html += '<div class="category-picker" id="' + id + '" tabindex="-1">';
//       html += '   <div class="category-mask"></div>';
//       html += '   <div class="category-modal" >';
//       html += '       <div class="category-modal-cnt">';
//       html += '           <div class="category-modal-header">';
//       html += '               ' + title + '';
//       html += '               <button type="button" class="category-close" tabindex="0" aria-label="關閉' + title + '" title="關閉' + title + '"></button>';
//       html += '           </div>';
//       html += '           <div class="category-modal-selected">';
//       html += '               <span>已選擇 ( <span class="selectedNum">0</span> )</span>';
//       html += '               <a class="category-del-all" aria-label="清空全部標籤" title="清空全部標籤" tabindex="0">清空全部標籤</a>';
//       html += '           </div>';
//       html += '           <div class="category-picker-selectedbox"></div>';
//       html += '           <div class="category-modal-body">';
//       html += '               ' + lv_1_parent + ' ';
//       html += '               ' + lv_2_parent + ' ';
//       html += '           </div>';
//       html += '           <div class="category-modal-footer">';
//       html += '               <button class="btn btn--border--primary category-close-btn" tabindex="0" aria-label="關閉" title="關閉">關閉</button>';
//       html += '               <button class="btn btn--primary category-confirm-btn" tabindex="0" aria-label="確定" title="確定">確定</button>';
//       html += '           </div>';
//       html += '       </div>';
//       html += '   </div>';
//       html += '</div>';

//       // 新增 選擇器 至頁面中
//       body.append(html);

//       // 宣告
//       var picker = $('#' + id);

//       // 代入 [date.js] 資料 並且產生 HTML
//       var n = 0;

//       for (var k in data) {
//         var lv_1_son = '';

//         for (var i in data[k].children) {
//           n++;

//           lv_1_son += '<li class="lv1 category-item" data-id="' + data[k].children[i].id + '" aria-label="' + data[k].children[i].name + '" title="' + data[k].children[i].name + '" for="' + data[k].children[i].uid + '" tabindex="0">' + data[k].children[i].name + '</li>';

//           var lv_2_son = '';

//           for (var j in data[k].children[i].children) {

//             var value = data[k].children[i].children[j].id;
//             var name = data[k].children[i].children[j].name;
//             var uid = data[k].children[i].children[j].uid;
//             var pid = data[k].children[i].children[j].pid;

//             lv_2_son += '<li class="category-item" tabindex="0">';
//             lv_2_son += '   <input type="checkbox" class="lv2 chk-inp" value="' + value + '" data-pid="' + pid + '" data-uid="' + uid + '" data-name="' + name + '" id="' + uid + '">';
//             lv_2_son += '   <label  class="chk-lab" aria-label="' + name + '" title="' + name + '" for="' + uid + '" >';
//             lv_2_son += '       <i class="box"></i>';
//             lv_2_son += '       <font>' + name + '</font>';
//             lv_2_son += '   </label>';
//             lv_2_son += '</li>';
//           }
//           picker.find('.list-level-two').eq(n - 1).append(lv_2_son);
//         }
//         picker.find('.list-level-one').eq(k).append(lv_1_son);
//       }
//     },

//     _keypress: function(id) {
//       var picker = $('#' + id);
//       // 鍵盤輸入事件 
//       picker.find('.category-item').bind('keypress', function(event) {
//         var code = event.keyCode || event.which;
//         var _this = $(this);
//         if (code == 13) {
//           if (_this.parent('.list-level-two').length) {
//             _this.find('input').click();
//           } else if (_this.parent('.list-level-one').length) {
//             _this.click();
//           }

//         }
//       });
//     },

//     open: function(id, el, off, elHidden) {

//       var picker = $('#' + id);

//       // 點擊 / 輸入時 Input 顯示選單
//       el.on('click keypress', function() {

//         picker.removeClass('close');
//         picker.find('.list-level-one').eq(0).addClass('list-tree-show');

//         if (picker.find('input.lv2').is(':checked')) {
//           //
//         } else {
//           elHidden.each(function() {
//             var val = $(this).val();
//             picker.find('input[value="' + val + '"]').click();

//           });
//         }

//         setTimeout(function() {
//           // 彈窗顯示
//           picker.addClass('show');
//           // 預設第一層 focus
//           picker.find('.lv1.category-item.active').focus();
//         }, 100);

//         focusableElements.attr('tabindex', '-1');
//         body.addClass('category-picker-body');

//       });

//     },

//     close: function(id, el, off) {
//       // 找 Picker ID
//       var picker = $('#' + id);

//       var selected = [];
//       var strSelected = '';


//       // 點選 關閉 按鈕
//       picker.find('.category-close , .category-close-btn').on('click', function() {
//         colseAction();
//       });



//       // 鍵盤 ESC 鍵關閉彈窗
//       picker.on('keydown', function(event) {
//         var code = event.keyCode || event.which;
//         if (event.keyCode === 27) {
//           colseAction();
//         }
//       });


//       function colseAction() {

//         picker.addClass('close');

//         setTimeout(function() {
//           // 彈窗隱藏
//           picker.removeClass('show');
//           // 輸入框焦點
//           el.focus();
//         }, 100);

//         focusableElements.attr('tabindex', '');

//         body.removeClass('category-picker-body');

//         picker.find('.lv2').prop('checked', false);
//         picker.find('.lv2:not(:checked)').prop('disabled', false);
//         picker.find('.selectedNum').text(0);
//         picker.find('.category-picker-selectedbox').empty();

//         off = true;
//       }
//     },

//     confirmC: function(id, el, off, unit, oN, config) {
//       // 找 Picker ID
//       var picker = $('#' + id);

//       // 點擊 確認 按鈕
//       picker.find('.category-confirm-btn').on('click', function() {

//         var selected = [];
//         var strSelected = '';
//         var lum = picker.find('.category-picker-selectedbox-item').length;

//         picker.find('.category-picker-selectedbox-item').each(function() {
//           var _this = $(this)
//           var name = _this.data('name');
//           var id = _this.data('id');
//           var uid = _this.data('uid');

//           selected.push({
//             name: name,
//             value: id,
//             uid: uid
//           });
//           strSelected += name + '、';
//         });

//         strSelected = strSelected.substring(0, strSelected.length - 1);

//         if (el.is('input')) {
//           if (lum >= oN) {
//             el.val('已經選擇 ' + lum + ' 個' + unit);
//             // console.log(strSelected.length)
//           } else {
//             el.val(strSelected);
//           }
//         } else {
//           if (strSelected == '') {
//             el.text('');
//             el.val('');
//           } else {
//             el.text(strSelected);
//             el.val(strSelected);
//           }
//         }

//         config.confirm(selected);

//         off = true;

//         setTimeout(function() {
//           el.focus();
//           picker.removeClass('show')
//           focusableElements.attr('tabindex', '');
//         }, 100);

//         picker.addClass('close');
//         body.removeClass('category-picker-body');
//       });
//     },

//     delAllTags: function(id) {
//       var picker = $('#' + id);

//       // 清除全部標籤按鈕 點擊事件
//       picker.find('.category-del-all').on('click', function() {

//         delAllTagAction()
//       });

//       // 清除全部標籤按鈕 鍵盤事件
//       picker.find('.category-del-all').bind('keypress', function(event) {
//         var code = event.keyCode || event.which;
//         if (code == 13) {
//           event.preventDefault();
//           delAllTagAction();
//         }
//       });

//       function delAllTagAction() {

//         picker.find('.lv2').prop('checked', false);
//         picker.find('.lv2').parent('.category-item').removeClass('active');
//         picker.find('.lv2:not(:checked)').prop('disabled', false);
//         picker.find('.category-picker-selectedbox .category-picker-selectedbox-item').hide(200, function() {
//           $(this).remove();
//         })
//         picker.find('.list-level-one').find('.lv1.category-item').removeClass('selected')
//         picker.find('.selectedNum').text(0);
//       }
//     },

//     delTags: function(id) {
//       var picker = $('#' + id);

//       // 單個標籤 清除 點擊事件
//       picker.on('click', '.category-picker-selectedbox-item-del-this', function() {
//         var _this = $(this);
//         var se_value = _this.parent().data('id');
//         var se_lum = picker.find('.category-picker-selectedbox-item').length;
//         var se_pid = _this.parent().data('pid');
//         var se_uid = _this.parent().data('uid');


//         picker.find('.lv2').each(function() {
//           var __this = $(this);
//           if (__this.val() == se_value) {

//             __this.parent('.category-item').removeClass('active');
//             __this.prop('checked', false);

//             var chk_pid = __this.data('pid');

//             if (chk_pid == se_pid) {
//               var parents = __this.parent().parent().data('parent');
//               var inp_pid = $('input[data-pid="' + chk_pid + '"]').parent('.active').length;

//               if (inp_pid == 0) {
//                 $('[data-id="' + parents + '').removeClass('selected');
//               }
//             }

//             if (__this.parent().parent().first('.category-item').find('.lv2')) {
//               __this.parent('.category-item').nextAll()
//                 .find('.lv2')
//                 .prop("checked", false);
//             }
//           }
//         });

//         _this.parent().hide(200, function() {
//           $(this).remove();
//         });

//         picker.find('.lv2:not(:checked)').prop('disabled', false);
//         picker.find('.selectedNum').text(se_lum - 1);

//       });

//       // 鍵盤清除標籤事件
//       picker.on('keypress', '.category-picker-selectedbox-item', function(event) {
//         var code = event.keyCode || event.which;
//         var _this = $(this);
//         if (code == 13) {

//           event.preventDefault();
//           _this.find('.category-picker-selectedbox-item-del-this').click();

//         }
//       });
//     },

//     sizeModal: function(id) {
//       var picker = $('#' + id);
//       var width = $(window).width();
//       // 選單預設選取第一個分類

//       // 螢幕寬度大於 576 的時候，選單預設
//       if (width >= 576) {
//         picker.find('.list-level-two').eq(0).addClass('list-tree-show');
//         picker.find('.lv1').eq(0).addClass('active');
//       }

//       // 顯示第 2 層分類標題能夠移動到 上一層
//       picker.find('.category-item--title').on('click', function() {
//         var _this = $(this);
//         if (_this.parent().hasClass('list-tree-show')) {
//           _this.parent().removeClass('list-tree-show');
//         }
//       });
//     },

//     clickLv1: function(id) {
//       var picker = $('#' + id);
//       // 選單第一層分類被點擊時
//       picker.find('.lv1').on('click', function() {
//         var _this = $(this);
//         var value = _this.data('id');

//         picker.find('.list-level-two').each(function(index) {
//           if ($(this).data('parent') == value) {
//             picker.find('.list-level-two').removeClass('list-tree-show');
//             picker.find('.list-level-two').eq(index).addClass('list-tree-show')
//           }
//         });

//         picker.find('.lv1').removeClass('active');
//         _this.addClass('active');

//       });
//     },

//     clickLv2: function(id, data, sl, config) {
//       var picker = $('#' + id);
//       // 第二層選項發生變化的時候
//       var max = sl - 1;

//       picker.find('.lv2').on('change', function() {
//         var _this = $(this);
//         var value = _this.val();
//         var name = _this.data('name');
//         var uid = _this.data('uid');
//         var pid = _this.data('pid');
//         var lum = picker.find('.category-picker-selectedbox-item').length;

//         _this.parent('.category-item').toggleClass("active");
//         // 
//         if (_this.prop('checked')) {
//           var item = '<label class="category-picker-selectedbox-item" data-id="' + value + '" data-pid="' + pid + '" data-uid="' + uid + '" data-name="' + name + '" aria-label="' + name + '" title="' + name + '" tabindex="0" >\
//                                 <span>' + name + '</span>\
//                                 <a class="category-picker-selectedbox-item-del-this"></a>\
//                             </label>';
//           // 第二層選項 被 Ckecked 的時候 產生標籤在上方
//           picker.find('.category-picker-selectedbox').append(item);

//           // 數量超過的時候，所有 CheckBox 變成 disabled
//           if (lum == max) {
//             picker.find('.lv2:not(:checked)').prop('disabled', true);
//           }
//           // 如果第二層分類裡面有被 Checked 則第一層的文字變色提示裡面有選取
//           if (picker.find('.lv2:checked').length > 0) {
//             picker.find('.list-level-one.list-tree-show').find('.lv1.category-item.active').addClass('selected');
//           }
//           // 已選取的數字增加
//           picker.find('.selectedNum').text(lum + 1);

//         } else {
//           // 已選取的數字增加
//           picker.find('.category-picker-selectedbox-item').each(function(index) {
//             if ($(this).data('id') == value) {
//               picker.find('.category-picker-selectedbox-item').eq(index).remove()
//             }
//           });
//           // 當第二層的選項 Checked == 0 得時候 父層選單文字顏色移除
//           for (var k in data) {
//             for (var i in data[k].children) {
//               if (picker.find('[data-parent="' + data[k].children[i].id + '"]').find('.lv2:checked').length == 0) {
//                 $('[data-id="' + data[k].children[i].id + '"]').removeClass('selected');
//               }
//             }
//           }

//           picker.find('.lv2:not(:checked)').prop('disabled', false);
//           // 已選取的數字減少
//           picker.find('.selectedNum').text(lum - 1);
//         }
//       });
//     },

//     slAll: function(id) {
//       var picker = $('#' + id);
//       picker.find('ul.list-level-two').each(function() {
//         var _this = $(this);
//         _this.find('.category-item').first().find('.lv2').click(function() {
//           var __this = $(this);
//           if (this.checked) {
//             __this.parent('.category-item').nextAll()
//               .find('.lv2:checked')
//               .click();
//             __this.parent('.category-item').nextAll()
//               .find('.lv2')
//               .prop("checked", true)
//               .prop("disabled", true);
//           } else {
//             __this.parent('.category-item').nextAll()
//               .find('.lv2')
//               .prop("checked", false);
//           }
//         });
//       });
//     },



//   };