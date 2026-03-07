// import TurboElements from './TurboFrame_Elements.js';
// export function TurboString() {


// }

// export { TurboString as default };
;
(function(TurboFrame) {


  TurboFrame.extend({
    /**
     * [  使用模板來實現一個簡單的數據綁定  ]
     * 實現簡單的數據綁定：@{name）、 @(role)
     * data : var data = { name : 'BarrY', role : '鑽石會員' };
     * str : var str = '歡迎 @(name)，等級： @(role) 光臨本站！';
     * @ Param str   原始的數據格式 
     * @ Param data  需要綁定的數據對象，是一個 json 格式的數據，json = {name : 'BarrY', age : 18}
     * @ var inner = TurboFrame("selector").stringBind( str , data ); 
     *   TutboFrame('[data-li="true"]').html(inner) ;
     **/
    // formateString: function(data) {
    //   return this.replace(/@\((\w+)\)/g, function(match, key) {
    //     // 注意這裡找到的值必須返回出去(如果是undefined，就是沒有數據)
    //     // 注意：判斷一個值的類型是不是undefined，可以通過typeof判斷
    //     console.log(typeof data[key] === 'undefined');
    //     return data[key] === 'undefined' ? '' : data[key];
    //   })
    // },
    stringBind: function(str, data) {
      // 使用後面的值去替換掉前面的值
      // 細節分析：((\w+)) 使用括號匹配的值在 JavaScript 中實際上就是一個 $1, 把這個參數傳給 match
      // (\w+) 第二個括號實際上匹配到的就是一個 $2, 把這個參數傳給 key
      // match: @(name), @(age), @(role)
      // key: name, age, role
      // console.log(str)
      return str.replace(/@\((\w+)\)/g, function(match, key) {
        // 先判斷有沒有匹配到相應的字符串
        // 找到 @() 開始的字符串， 使用數據域中的數據去替換
        // 如果 json 數據 data 裏面麼有找到相應的 data[key] 數據，返回的實際上就是一個空的字符串
        return typeof data[key] === 'undefined' ? '' : data[key];
      });

    },
    /**
     * @ CamelCase 函數的功能就是將形如 background-color 轉化爲駝峯表示法：backgroundColor
     */
    camelCase: function(str) {
      // all: -c , letter: c
      return str.replace(/\-(\w)/g, function(all, letter) {
        // 把所有的字母都轉換爲大寫的狀態
        return letter.toUpperCase();
      });
    },
    /**
     * @ Param str
     * @ Returns {*}
     */
    trimLeft: function(str) {
      return str.replace(/^\s*/g, '');
    },
    /**
     * @ Param str
     * @ Returns {*}
     */
    trimRight: function(str) {
      return str.replace(/\s*$/g, '');
    },

    /**
     * 去掉所有的空格 (兩邊的空格)，可以針對任意格式的字符串
     * 先去掉左邊的空格，然後去掉右邊的空格
     * @ Param str
     * @ Returns {*}
     */
    trim: function(str) {
      // var regx = '/^\s*\s*$/g';
      // return str.replace(regx, '');
      // | 表示或的意思，也就是滿足 | 左邊的也成立，滿足 | 右面的也成立
      // (^\s*) 表示的就是以 0 個空格或者多個空格開頭
      // (\s*$) 的意思就是，以 0 個空格或者多個空格結尾
      // /…/g 是正則表達式的屬性，表示全文匹配，而不是找到一個就停止
      return str.replace(/(^\s*)|(\s*$)/g, "");
      //return this.trimRight(this.trimLeft(str));
    },
    // 判定目標字符串是否位於原字符串的開始之處
    // Target 目標字符串 || Str 父字符串 || Ignorecase 是否忽略大小寫
    startsWith: function(target, str, ignorecase) {
      var start_str = target.substr(0, str.length);
      return ignorecase ? start_str.toLowerCase() === str.toLowerCase() : //
        start_str === str;
    },

    // 判定目標字符串是否位於原字符串的末尾
    // Target 目標字符串 || Str 父字符串 || Ignorecase 是否忽略大小寫
    endWith: function(target, str, ignorecase) {
      var end_str = target.substring(target.length - str.length);
      return ignorecase ? end_str.toLowerCase() === str.toLowerCase() : //
        end_str === str;
    },

    // 將一個字符串重覆自身 N 次
    // Target 目標字符串 || Ignorecase 重覆次數
    repeat: function(target, n) {
      var ev = target;
      var total = "";
      while (n > 0) {
        if (n % 2 == 1) {
          total += ev;
        }
        if (n == 1) {
          break;
        }
        ev += ev;
        // >> 是右移位運算符，相當於將 n 除以 2 取其商,或說開 2 二次方
        n = n >> 1;
      }
      return total;
    },
    // 移除字符串中的 html 標簽。
    // Target 目標字符串 
    stripTags: function(target) {
      return String(target || "").replace(/<[^>]+>/g);
      //[^>] 匹配除>以外的任意字符
    },

    // 移除字符串中所有的 Script 標簽及內容
    // Target 目標字符串 
    stripScripts: function(target) {
      return String(target || "").replace(/<script[^>]*>([\S\s]*?)<\/script>/img); //[\S\s]*? 懶惰匹配任意字符串盡可能少
    },

    // 在字符串兩端添加雙引號，然後內部需要轉義的地方都要轉義，用於接裝 JSON 的鍵名或模析系統中。
    // Target 目標字符串 
    quote: function(target) {
      //需要轉義的非法字符
      var escapeable = /["\\\x00-\x1f\x7f-\x9f]/g;
      var meta = {
        '\b': '\\b',
        '\t': '\\t',
        '\n': '\\n',
        '\f': '\\f',
        '\r': '\\r',
        '"': '\\"',
        '\\': '\\\\'
      };

      if (target.match(escapeable)) {
        return '"' + target.replace(escapeable, function(a) {
          var c = meta[a];
          if (typeof c === 'string') {
            return c;
          }
          return '\\u' + ('0000' + c.charCodeAt(0).toString(16)).slice(-4)
        }) + '"';
      }
      return '"' + target + '"';
    },


  });
})(window.TurboFrame);