/**
 * 用於給 Js 中內置的對象進行擴充方法
 */
;
(function() {
  // 擴充函數執行前需要調用
  stringExtend();
  arrayExtend();
  functionExtend();
  // String 對象方法的擴充
  function stringExtend() {
    // str = 'name: @(name), age:@(age)'
    // data = {name : 'xiugang', age : 18}
    /**
     * 實現一個簡單的數據綁定
     * @param str
     * @param data
     * @return {*}
     */
    String.prototype.formateString = function(data) {
        return this.replace(/@\((\w+)\)/g, function(match, key) {
          // 注意這裏找到的值必須返回出去(如果是undefined，就是沒有數據)
          // 注意：判斷一個值的類型是不是undefined，可以通過typeof判斷
          console.log(typeof data[key] === 'undefined');
          return data[key] === 'undefined' ? '' : data[key];
        });

      }
      /**
       * 去掉座標的空格
       * @param str
       * @return {*}
       */
    String.prototype.ltrim = function() {
        return this.replace(/^\s*/g, '');

      }
      /**
       * 去掉右邊的空格
       * @param str
       * @return {*}
       */
    String.prototype.rtrim = function() {
        return this.replace(/\s*$/g, '');
      }
      /**
       * 去掉兩邊的空格
       * @param str
       * @return {*}
       */
    String.prototype.trim = function() {
      return this.replace(/(^\s*)|(\s*$)/g, '');
    }

    // red ===> Red
    /**
     * 將第一個字母小寫，其他字母大寫
     * @param str
     * @return {*}
     */
    String.prototype.camelCase = function() {
        // .*?是非貪婪的匹配，點可以匹配任意字符，星號是前邊的字符有0-n個均匹配，問號是則是0-1；
        // (^\w{1}): 用於匹配第一個首字母
        // (.*)：用於匹配任意個的前面的字符

        // - param 1: 匹配到的字符串
        // - param 2: 匹配的的子字符串
        // - param 3: 匹配的子字符串
        // - param 4: 匹配到的字符串在字符串中的位置
        // - param 5: 原始字符串

        return this.replace(/(^\w{1})(.*)/g, function(match, g1, g2) {
          return g1.toUpperCase() + g2.toLowerCase();
        });
      }
      /**
       * 將一個字符串的下劃線轉換爲中劃線
       * @param str
       * @return {*}
       */
    String.prototype.dashString = function() {
      // 這裏面的this實際上指向的就是我們自己定義的一個變量字符串
      return this.replace(/\_/g, '-');
    }

    /**
     * 檢測一個字符串是不是爲空
     * @return {boolean}
     */
    String.prototype.isEmpty = function() {
        return this.length === 0;

      }
      /**
       * 判斷字符串是不是包含一個字符串
       * @param target
       * @return {boolean}
       */
    String.prototype.contains = function(target) {
        // 只要這個indexOf的下標不是-1的話，就說明包含這個目標字符串，否則的話就是不包含
        // indexOf() 方法可返回某個指定的字符串值在字符串中首次出現的位置，如果沒找到的話，就返回-1
        return this.indexOf(target) !== -1;
      }
      /**
       * 對一個字符串中的特殊字符進行轉義
       * @return {string}
       */
    String.prototype.escapeHTML = function() {

        // 先進行字符串分割， 得到一個數組
        var strArr = this.split('');
        for (var pos = 0, l = strArr.length, tmp; pos < l; pos++) {
          // 拿到數組中的每一個元素
          tmp = strArr[pos];
          // 對字符串中的每一個元素進行判斷， 如果是特殊字符的話就進行處理
          switch (tmp) {
            // pos始終爲1， 表示要替換的項是1項
            case '<':
              replaceArr(strArr, pos, '&lt;');
              break;
            case '>':
              replaceArr(strArr, pos, '&gt;');
              break;
            case '\'':
              replaceArr(strArr, pos, '&#39;');
              break;
            case '\"':
              replaceArr(strArr, pos, '&quot;');
              break;
            case '&':
              replaceArr(strArr, pos, '&amp;');
              break;
            default:
              ;
          }
        }
        // join() 方法用於把數組中的所有元素放入一個字符串。
        return strArr.join('');

        // 專門用於替換掉數組中的元素
        /**x
         * 替換數組中指定的項
         * @param arr
         * @param pos
         * @param item
         * @return {*}
         */
        function replaceArr(arr, pos, item) {
          // Splice： splice主要用來對 JS 中的數組進行操作，包括刪除，添加，替換等，原來的數組會被改變
          // 刪除數據：array.splice(index,num)，返回值爲刪除內容，array爲結果值。index爲起始項，num爲刪除元素的的個數。
          // 插入數據：array.splice(index,0,insertValue)，index要插入的位置，insertValue要插入的項
          // 替換數據：array.splice(index,num,insertValue)，index起始位置，num要被替換的項數，insertValue要替換的值
          return arr.splice(pos, 1, item);
        }

      }
      /**
       * 忽略HTML中的一些內置的特殊字符
       * @return {string}
       */
    String.prototype.escapeHTML = function() {
        return Array.prototype.slice.call(this).join('').replace(/$/g, '&amp')
          .replace(/\</g, '&lt')
          .replace(/\>/g, '&gt')
          .replace(/\'/g, '&#39')
          .replace(/\"/g, '&quot');
      }
      /**
       * 對字符串進行反轉義
       * @return {string}
       */
    String.prototype.unescapeHTML = function() {
        // 由於這裏的 this 實際上拿到的是一個字符串數組, 因此第一步需要先把字符串數組轉換爲一個字符串
        // console.log(typeof this);
        // 1.先把這個僞數組轉換爲數組對象
        var arr = Array.prototype.slice.call(this);
        // 2.把數組中的內容轉換爲字符串
        var res = arr.join('');
        // 查找所有的< > & " ' 字符，並替換掉
        return res.replace(/&lt/g, '<')
          .replace(/&gt/g, '>')
          .replace(/&#39/g, '\'')
          .replace(/&quot/g, '\"')
          .replace(/&amp/g, '')

        // String.fromCharCode() 靜態方法根據指定的 Unicode 編碼中的序號值來返回一個字符串。String.fromCharCode(65,66,67) “ABC”
        .replace(/&#(\d+)/g, function($0, $1) {
          //parseInt() 函數將給定的字符串以指定基數（radix/base）解析成爲整數。就是 你想把string當成radix進制數解析成10進制
          return String.fromCharCode(parseInt($1, 10));
        });
      }
      /**
       * 把一個字符串進行反轉操作
       * @return {string}
       */
    String.prototype.reverse = function() {
      // 1. 先獲得我需要的字符串，然後進行分割處理
      var arr = this.toString().split('');
      // 2. 對我分割後得到的數組元素進行逆序處理
      arr = arr.reverse();
      // 3.把數組中的元素變爲一個字符串
      return arr.join();
      //return (this.toString()).split('').reverse().join();
    }

  }

  // Array 對象方法的擴充
  function arrayExtend() {
    /**
     * 將一個數組元素清空
     * @return {Array}
     */
    Array.prototype.clear = function() {
        this.length = 0;
        return this;
      }
      /**
       * 計算一個數組的長度
       * @return {*}
       */
    Array.prototype.size = function() {
        return this.length;
      }
      /**
       * 返回數組裏面的第一個元素
       * @return {*}
       */
    Array.prototype.first = function() {
        return this[0];
      }
      /**
       * 返回數組的最後一個元素
       * @return {*}
       */
    Array.prototype.last = function() {
      return this[this.length - 1]
    }


    function cacl(arr, callback) {
      // 變量的初始化（治理在使用的時候進行初始化）
      var ret;
      for (var i = 0, len = arr.length; i < len; i++) {
        ret = callback(arr[i], ret);
      }
      return ret;
    }

    /**
     * 對數組的所有元素進行求和
     * @return {*}
     */
    Array.prototype.sum = function() {
        // 1. 一般的方法
        /*var ret = 0;
        for (var i = 0, len = this.length; i < len; i++){
            ret = ret + this[i];
        }
        return ret;*/

        // 2.使用上面的計算類
        /**
         * @param:item 數組的每一項
         * @param:sum 數組求和的結果
         */
        return cacl(this, function(item, sum) {
          // 如果剛開始沒有初始化的話，就直接使用第一項作爲sum（ret）的初始值
          if (typeof sum === 'undefined') {
            return item;
          } else {
            return sum += item;
          }
        })

      }
      /**
       * 找出數組中的最大值
       * @return {*}
       */
    Array.prototype.max = function() {
        // 1. 一般的方式求出最大值
        /*var ret = 0;
        for (var i = 0, len = this.length; i < len; i++){
            if (ret < this[i]){
                ret = this[i];
            }
        }
        return ret;*/

        // 2. 第二種方式
        return cacl(this, function(item, max) {
          if (typeof max === 'undefined') {
            return item;
          } else {
            if (max < item) {
              return item;
            } else {
              return max;
            }
          }
        })
      }
      /**
       * 找出一個數組中的最小值
       * @return {*}
       */
    Array.prototype.min = function() {
      return cacl(this, function(item, min) {
        if (typeof min === 'undefined') {
          return item;
        } else {
          // 只要每一項的值都不比最小值小的話
          if (!(min < item)) {
            return item;
          } else {
            return min;
          }
        }
      })
    }

    /**
     * 求出一個數組中所有元素的平均值
     * @return {*}
     */
    Array.prototype.avg = function() {
      // 1. 先對數組中的元素個數組進行判斷一下，防止計算出現無窮的情況
      if (this.length === 0) {
        return;
      }
      var sum = this.sum();
      return sum / this.length;
      /*return cacl(this, function (item, avg) {
          // 1. 先求和(進入到這個函數裏面， this指向的是window對象，此時window對象是沒有sum方法的，故執行錯誤)
          //var sum = this.sum();
          // 2.求出平均值
          if (typeof avg === 'undefined'){
              return item;
          } else{
              avg = sum / (this.length);
          }
          return avg;
      })*/
    }


    // 去除數組中的重複項
    /*
     * 實現思路： 遍歷原始數組中的每一項元素，讓每次遍歷的這一個元素和後面的每一個元素進行比較
     * 【只要相同的話就直接跳過繼續向下尋找】
     * */
    Array.prototype.unique = function() {
        var a = [],
          len = this.length;
        for (var i = 0; i < len; i++) {
          for (var j = i + 1; j < len; j++) {
            if (this[i] === this[j]) {
              // 如果找到了相鄰的兩個元素是相同的，i直接向後移動一位
              // 然後j開始從i的位置繼續向後尋找元素
              j = ++i;
            }
          }
          a.push(this[i]);
        };
        return a;
      }
      /**
       * 去除數組中的重複項
       * 【實現思路】：先對數組進行排序，然後比較相鄰的元素是否相同
       * @return {Array}
       */
    Array.prototype.unique = function() {
      var tmp = [],
        len = this.length;
      // 1.先對原始的數組進行排序
      this.sort();
      // 2.比較相鄰的元素
      for (var i = 0; i < len; i++) {
        // 只要相鄰的元素相同，就直接跳過
        if (this[i] === this[i + 1]) {
          continue;
        }

        // 由於tmp.length初始的位置一直是0， 添加一個元素之後變爲1，因此下標和長度每次相差1， 實現了實時插入數據的功能
        tmp[tmp.length] = this[i];
      }
      return tmp;
    }

    /**
     * 實現兩個數組的並集，然後去除重複元素
     * @param target
     * @return {*}
     */
    Array.prototype.union = function(target) {
      // concat() 方法用於連接兩個或多個數組。
      // 連接數組之後然後去除數組中的重複項
      return this.concat(target).union();
    }

    /**
     * 求出兩個數組的交集
     * @param target
     * @return {Array|*[]}
     */
    Array.prototype.intersect = function(target) {
      // 1.先去除原始數組和目標數組中的重複元素
      var originArr = this.unique(),
        targetArr = target.unique();
      // filter()的作用是返回某一數組中滿足條件的元素，該方法返回的是一個新的數組
      // 2.開始使用條件過濾
      /**
       * @param element（必選）：當前元素的值
       @param index（可選）： 當前元素的索引
       @param array（可選）：當前元素所屬的數組
       */
      return originArr.filter(function(element, index, array) {
        // filter函數默認會把所有的返回false的元素去掉
        for (var i = 0, len = targetArr.length; i < len; i++) {
          if (element === targetArr[i]) {
            // 只要是返回滿足true的所有條件，基本上都會被過濾掉
            return true;
          }
          //return false;
        }
        // 只有找到相同的元素的時候返回的是true,其他情況都是返回的是false
        return false;
      });

    }

    /**
     * 找出兩個數組中的不同元素
     * @param target
     * @return {Array|*[]}
     */
    Array.prototype.diff = function(target) {
      // 1. 獲取原始數組和目標數組，去除重複項
      var orignArr = this.unique(),
        targetArr = target.unique();
      // 2. 開始使用filter函數過濾條件
      return orignArr.filter(function(element, index, array) {
        for (var i = 0, len = targetArr.length; i < len; i++) {
          // 只要元素相等的話，就全部過濾掉
          if (element === targetArr[i]) {
            return false;
          }
        }
        return true;
      });
    }

    /**
     * 對數組的每一項遍歷的時候設置一個回調函數（沒有返回結果）
     * @param fn
     * @param ctx
     */
    Array.prototype.forEach = function(fn, ctx) {
      var i = 0,
        len = this.length;
      for (; i < len; i++) {
        // element, index, array
        // call 的第一個參數也就是this的指向， 其他參數表示需要傳遞給回調函數的的參數
        fn.call(ctx || null, this[i], i, this);
      }
    }

    /**
     *
     * 對數組的每一項執行回調，返回由回調函數的結果組成的數組
     * @param fn
     * @param ctx
     * @return {Array}
     */
    Array.prototype.map = function(fn, ctx) {
        // 初始化變量
        var ret = [],
          i = 0,
          len = this.length;
        // 遍歷數組的每一項元素， 返回由回調函數的結果組成的數組
        for (; i < len; i++) {
          // 調用回調函數， 返回指向結果
          res = fn.call(ctx || null, this[i], i, this);
          // 將每一項執行的結果放入到一個新的數組裏面
          ret.push(res);
        }
        return ret;
      }
      /**
       * 對數組的每一項執行回調函數， 返回回調函數執行結果爲true的數組集合
       * @param fn
       * @param ctx
       */
    Array.prototype.filter = function(fn, ctx) {
      var ret = [],
        i = 0,
        len = this.length;
      // 遍歷每一項，把執行結果爲true的所有元素集合存起來
      for (; i < len; i++) {
        // 注意這裏的這種運算方式只會返回所有的回調函數返回true的計算結果集
        fn.call(ctx || null, this[i], i, this) && ret.push(this[i]);
      }
      return ret;
    }


    /**
     * 遍歷數組中的每一項元素
     * @param fn
     */
    Array.prototype.each = function(fn) {
      var i = 0,
        len = this.length;
      for (; i < len; i++) {
        fn.call(this[i]);
      }
    }


    /**
     * 對數組的【每一項】執行回調函數，必須每一項回調函數返回true， 就返回true
     * @param fn
     * @param ctx
     */
    Array.prototype.every = function(fn, ctx) {
        var i = 0,
          len = this.length;
        // 遍歷數組中所有的元素， 只要有一個函數回調函數爲false就返回false,只要所有的都是true纔會返回true
        for (; i < len; i++) {
          // 如：a默認是undefined，!a是true，!!a則是false，所以b的值是false，而不再是undefined。這樣寫可以方便後續判斷使用。
          // 所以，!!(a)的作用是將a強制轉換爲布爾型（boolean）。
          // 如果a = null, !!(a) 的結果就是假， 可以直接把一個弱類型強制轉換爲一個新的類型
          // 下面的代碼就是強制將一個函數轉換爲bool的類型
          if (!!fn.call(ctx || null, this[i], i, this) === false)
            return false;

          // 上面的代碼等價於
          /*if (fn.call(ctx || null, this[i], i, this)) {
              return true;
          }*/
        }
        return true;
      }
      /**
       * 對數組中的每一項執行回調函數，只要有一項爲true的話，就是true，否則就是false
       * @param fn
       * @param ctx
       */
    Array.prototype.some = function(fn, ctx) {
      var i = 0,
        len = this.length;
      // 循環遍歷每一項，只要有一項爲true，就是true
      for (; i < len; i++) {
        /*
        * // 強制轉換爲Boolean 用 !!
        var bool = !!"c";
        console.log(typeof bool); // boolean

        // 強制轉換爲Number 用 +
        var num = +"1234";
        console.log(typeof num); // number

        // 強制轉換爲String 用 ""+
        var str = ""+ 1234;
        console.log(typeof str); // string
        * */
        if (!!fn.call(ctx || null, this[i], i, this) === true)
          return true;
      }
      return false;
    }

    /**
     * 從左向右執行回調函數（第二個元素開始）
     * 其中包含了上一次回調的返回值
     * @param callback
     */
    Array.prototype.reduce = function(callback) {
      var i = 0,
        len = this.length,
        callbackRet = this[0]; // 這個變量保存着上一次回到的函數的返回結果， 默認存儲的是第一個元素
      for (; i < len; i++) {
        // this的指向，element， index， 數組對象本身
        // callbackRet 裏面存儲了數組上一次計算的處理結果
        callbackRet = callback.call(null, callbackRet, this[i], i, this);
      }
      return callbackRet;
    }

    /**
     * 從右向左處理每一項元素，倒數第二項開始執行
     * @param callback
     */
    Array.prototype.reduceRight = function(callback) {
      var len = this.length,
        i = this[len - 2],
        callbackRet = this[len - 1]; // 保存着最後一項

      // 從倒數第二項開始向前遍歷數組的每一項
      for (; i >= 0; i--) {
        //this指向， prev， element, index, arr
        callbackRet = callback.call(null, callbackRet, this[i], i, this);
      }
      return callbackRet;
    }


    /**
     * 返回目標值target在數組中第一次出現的位置， 搜索默認會從左向右執行
     * @param target
     * @param start
     */
    Array.prototype.indexOf = function(target, start) {

      /*
      * 其實是一種利用符號進行的類型轉換,轉換成數字類型
      ~~true == 1
      ~~false == 0
      ~~"" == 0
      ~~[] == 0
      ~~undefined ==0
      ~~!undefined == 1
      ~~null == 0
      ~~!null == 1
      * */
      var len = this.length,
        start = ~~start; // 如果start不傳過來，這裏就是undefined，指向後面的就會保存，這裏使用了~~把其他類型強制轉換爲數字類型
      if (start < 0) {
        // 如果指定搜索的起始位置小於0的話， 默認就從0的位置開始向後搜索
        start = 0;
      }
      // 從用戶指定的起始位置開始向後搜索
      for (; start < len; start++) {
        if (this[start] === target) {
          return start;
        }
      }
      // 如果沒找到的話，就返回-1
      return -1;
    }


    /**
     * 返回指定的目標值在數組中最後一次出現的位置
     * @param target
     * @param start
     */
    Array.prototype.lastIndexOf = function(target, start) {
      // 這裏相當於是typeof start ==== 'undefined'
      if (start === void 0) {
        start = this.length;
      } else if (start < 0) {
        start = 0;
      }

      // 開始從數組的最後面向前遍歷
      for (; start >= 0; start--) {
        // 找到目標元素target在數組中最後一次出現的位置（從後向前找）
        if (this[start] === target) {
          return start;
        }
      }
      return -1;
    }

    /**
     * 數組去重方法加強版本
     * 侷限性：只適用於數組中存放的是單一的數據類型，如果是多種數據類型並存的話，就會去重失敗
     * ['ff', 1, '1']
     */
    Array.prototype.enhanceUnique = function() {
      var ret = [],
        tempMap = {},
        i = 0,
        len = this.length,
        temp;

      // 遍歷數組的每一項
      for (; i < len; i++) {
        temp = this[i];
        // 只要這個tempMap中沒有這一項的話，就直接放入到數組中去
        if (tempMap[temp] === void 0) {
          ret.push(temp);
          // {}數據的存儲格式爲{1 : true, 2 : false, 3 : false}
          tempMap[temp] = true;
        }
      }
      return ret;
    }


    /**
     * 刪除數組中的指定元素， 通過arguments僞數組的方式來接受傳遞過來的參數
     * 經過測試，只能刪除數組中重複的多餘的元素
     * @return {Array}
     */
    Array.prototype.without = function() {
      // slice(start, end) 方法可從已有的數組中返回選定的元素。
      // 如果slice()這個函數沒有指定結束的位置的話，默認是會返回數組中的start之後的所有元素
      // 1. 獲取用戶傳過來的參數， 去掉數組中重複的元素
      //var args = [].slice.call(arguments).unique();
      /*
      * Array.prototype.slice.call({
       0:"likeke",
       1:12,
       2:true,
       length:3
      });
      * */
      //1. 由於arguments實際上是一個僞數組，不能直接使用數組裏面的方法
      // 因此先要把arguments轉換爲數組
      var arr = Array.prototype.slice.call(arguments) || [].slice.call(arguments);
      // 2. 把數組中的重複元素去重
      var args = arr.unique(),
        len = this.length,
        aLength = args.length,
        i = 0,
        j = 0;


      // 遍歷原始的數組(由於後面每次刪除掉一個元素之後，這裏的this.length的長度就是已經都改變了， 因此每次在執行完畢之後都要重新計算一下length)
      for (; i < len; i++) {
        for (; j < aLength; j++) {
          if (this[i] === args[j]) {
            // 只要刪除的數組在我的這個裏面，就直接去掉
            // i 爲起始的值，1爲要刪除的項， 也就是刪除i位置的元素
            // splice  返回的是刪除的元素， this內容是已經修改過之後的項
            this.splice(i, 1);

            // 爲了避免刪除數組的元素之後的數組長度的變化，這裏需要重新計算一下數組的新的長度
            // len = this.length;
          }

        }
        // 將j下標復位，以便下一次循環(注意是在每一次j循環完畢之後然後再把j初始化到原始的狀態)
        j = 0;
      }
      return this;
    }


    /**
     * 去掉數組中的目標元素
     */
    Array.prototype.enhanceWithout = function() {
      // 用於去除數組中指定的的多餘的元素
      var ret = [],
        len = this.length,
        args = ([]).slice.call(arguments),
        argsLength = args.length,
        i = 0,
        j = 0;

      for (; i < len; i++) {
        for (; j < argsLength; j++) {
          if (args[j] !== this[i]) {
            ret.push(this[i]);
          }
        }
        // 由於這裏的j使用的是局部變量，因此這裏需要進行處理
        j = 0;
      }
      return ret;
    }


    /**
     * 實現一個數組的扁平化(可以解決數組裏面存放數組的問題)【遞歸處理調用】
     * [[], [], [], [[], [], []]]
     * @return {Array}
     */
    Array.prototype.flatten = function() {
      // 實現一個flatten函數，將一個嵌套多層的數組 array（數組） (嵌套可以是任何層數)轉換爲只有一層的數組
      // 數組中元素僅基本類型的元素或數組，
      var ret = [],
        len = this.length, // 注意當下一次執行遞歸調用之後，這裏的this指向的是tmp
        i = 0,
        tmp;

      for (; i < len; i++) {
        // 注意這裏先取出來數組中的每一項元素
        tmp = this[i];
        // 判斷一下數組裏面存放的還是不是數組類型（數組裏面的每一項）
        if (({}).toString.call(tmp) === '[object Array]' || Object.prototype.toString.call(tmp) === '[object Array]') {
          // 繼續遞歸調用(遞歸調用的時候需要把結果存起來哦)
          // 1. 對當前數組裏面的數組進行扁平化處理, tmp.flatten()得到的就是一個普通的數組類型
          // 2. 由於ret是一個數組類型，使用concat之後可以把兩個數組裏面的元素鏈接起來
          // 下一次執行遞歸的時候上面的this就是指向了這裏的tmp數組
          ret = ret.concat(tmp.flatten())
            //tmp.flatten();
        } else {
          // 如果不是數組類型的話，就直接放入到我的新數組裏面
          ret.push(tmp);
        }
      }
      return ret;
    }


    /**
     * 刪除數組中的指定位置的項
     * @param pos
     * @return {Array}
     */
    Array.prototype.removeAt = function(pos) {
      // 移出數組中指定位置的項
      // slice() 函數調用的執行結果返回的是刪除掉的項， 這個this就是修改之後的項
      this.splice(pos, 1);
      return this;
    }

    /*
    【經驗話語1】
      直接用等號 （==） 判斷時，變量必須要聲明（包括不用var 的隱式聲明），否則出錯。
      不管變量有沒有聲明，都可用typeof 判斷，注意typeof 返回結果爲字符串，所以是與"undefined"做比較。
      所以，判斷類型最好用typeof ，因爲當判斷的變量是在其他js 文件中定義的全局變量時，
      執行此判斷時，定義該變量所在的js 文件可能還未加載完成，用== 判斷就會報錯：is not defined
    【經驗話語2】

    注意slice()和splice（） 這兩者的區別
    * */


    /**
     * 判定一個字符串是否包含另外一個字符串
     * @param target
     * @return {boolean}
     */
    Array.prototype.contains = function(target) {
      // 可以調用自己之前申明好的some方法，數組中只要有一項，就會返回true
      return this.some(function(element, index, self) {
        // 調用 this.some() 方法實際上會返回遍歷數組元素的每一項
        return element === target;
      })
    }

    /**
     * 隨機返回數組中的某一項 (把數組中的任意一項返回)
     * @param n
     * @return {*}
     */
    Array.prototype.random = function(n) {
      //Math.floor():向下取整。Math.floor(1.8) -> 1
      //Math.ceil():向上取整。Math.ceil(1.1) -> 2
      //v = parseFloat('0.'+crypto.getRandomValues(new Uint32Array(1))[0]) * n:會產生一個 0 < v < nv的數
      //v2 = Math.floor(parseFloat('0.'+crypto.getRandomValues(new Uint32Array(1))[0]) * n)：v2爲一個大於等於0，小於n的整數
      var index = (Math.floor(parseFloat('0.'+crypto.getRandomValues(new Uint32Array(1))[0]) * n));
      return this[index] || this[this.length - 1];
    }
  }

  // Function 對象方法的擴充
  function functionExtend(func) {

    Function.prototype.before = function(func) {
      // 一般來說加下劃線的變量爲私有變量，這是常規都比較遵守的一種代碼規範。
      var __self = this; // 私有的屬性用下劃線
      return function() {
        // 重新把我需要傳遞的參數傳遞過去， 如果目標函數返回的是 false, 就是 false
        if (func.apply(this, arguments) === false) {
          return false;
        }
        // 否則就把我的自己的參數傳遞過去
        return __self.apply(this, arguments);
      }
    }


    /**
     * AOP 切面編程的函數擴充
     * @param func
     * @return {Function}
     */
    Function.prototype.after = function(func) {
      var __self = this;
      return function() {
        var ret = __self.apply(this, arguments); // //返回一個函數，相當於一個代理函數，也就是說，這裏包含了原函數和新函數，原函數指的是myFunc，新函數指的是fn
        if (ret === false) {
          return false;
        }
        func.apply(this, arguments);
        return ret;
      }
    }
  }

})();