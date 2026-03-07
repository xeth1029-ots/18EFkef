// Browser 相關

;
(function(TurboFrame) {


    // /**
    //  * 返回當前 url
    //  */
    var currentURL = (function currentURL() {
        return window.location.href;
    })();
    var browserName = (function browserName(val) {
        var userAgent = window.navigator.userAgent;

        var isExplorer = function isExplorer(exp) {
            return userAgent.indexOf(exp) > -1;
        };

        (function appendName(name) {
            if (isExplorer('MSIE')) {
                if (userAgent.indexOf('MSIE') > 0 || userAgent.indexOf('Trident/') > 0) {
                    document.documentElement.className += "ie"
                    return 'IE';
                } else if (userAgent.indexOf('Edge/') > 0) {
                    document.documentElement.className += "edge"
                    return 'Edge';
                }

            }



        })();

        if (isExplorer('MSIE')) return 'IE';
        else if (isExplorer('Firefox')) return 'Firefox';
        else if (isExplorer('Chrome')) return 'Chrome';
        else if (isExplorer('Opera')) return 'Opera';
        else if (isExplorer('Safari')) return 'Safari';
    })();


    TurboFrame.extend({
        /**
         * 獲取當前瀏覽器名稱
         */
        browser: browserName,
        /**
         * 獲取 url 參數
         * @ Param {*} name
         * @ Param {*} origin
         */
        url: currentURL,

        /**
         * Checks whether current device is mobile touch.
         * @ returns {boolean}
         */
        isMobileDevice: function() {
            var test = (this.getViewPort().width < this.getBreakpoint('lg') ? true : false);

            if (test === false) {
                // For use within normal web clients
                test = navigator.userAgent.match(/iPad/i) != null;
            }
            // console.log(test);
            return test;
        },

        /**
         * Checks whether current device is desktop.
         * @ returns {boolean}
         */
        isDesktopDevice: function() {
            // console.log(ByUtil.isMobileDevice());
            return this.isMobileDevice() ? false : true;
        },

        /**
         * Gets browser window viewport size. Ref:
         * http://andylangton.co.uk/articles/javascript/get-viewport-size-javascript/
         * @ returns {object}
         */
        getViewPort: function() {
            var e = window,
                a = 'inner';
            if (!('innerWidth' in window)) {
                a = 'client';
                e = doc.documentElement || doc.body;
            }

            return {
                width: e[a + 'Width'],
                height: e[a + 'Height']
            };
        },

        /**
         * Checks whether given device mode is currently activated.
         * @param {string} mode Responsive mode name(e.g: desktop,
         *     desktop-and-tablet, tablet, tablet-and-mobile, mobile)
         * @returns {boolean}
         */
        isBreakpointUp: function(mode) {
            var width = this.getViewPort().width;
            var breakpoint = this.getBreakpoint(mode);

            return (width >= breakpoint);
        },

        isBreakpointDown: function(mode) {
            var width = this.getViewPort().width;
            var breakpoint = this.getBreakpoint(mode);

            return (width < breakpoint);
        },

        getViewportWidth: function() {
            return this.getViewPort().width;
        },
        getUrlParam: function(name) {
            var origin = arguments.length <= 1 || arguments[1] === undefined ? null : arguments[1];

            var url = location.href;
            var temp1 = url.split('?');
            var pram = temp1[1];
            var keyValue = pram.split('&');
            var obj = {};
            for (var i = 0; i < keyValue.length; i++) {
                var item = keyValue[i].split('=');
                var key = item[0];
                var value = item[1];
                obj[key] = value;
            }
            return obj[name];
        },
        /**
         * 修改 url 中的參數
         * @ Param { string } paramName
         * @ Param { string } replaceWith
         */
        replaceParamVal: function(paramName, replaceWith) {
            var oUrl = location.href.toString();
            var re = eval('/(' + paramName + '=)([^&]*)/gi');
            //owasp B43138B7BCCB58D1BFE8BA70FFFF367D7D132A41266DA7CD436B5F693058E2D226BD9878F4B79D841FC74B9E5DE2B6B58C84555605BB4721A9A369609A79F2D077A7F7E2F6F6E1DF5D38A50894CD05D1
            return location.href;
        },

        /**
         * 刪除 url 中指定的參數
         * @ Param { string } name
         */
        funcUrlDel: function(name) {
            var loca = location;
            var baseUrl = loca.origin + loca.pathname + "?";
            var query = loca.search.substr(1);
            if (query.indexOf(name) > -1) {
                var obj = {};
                var arr = query.split("&");
                for (var i = 0; i < arr.length; i++) {
                    arr[i] = arr[i].split("=");
                    obj[arr[i][0]] = arr[i][1];
                }
                delete obj[name];
                var url = baseUrl + JSON.stringify(obj).replace(/[\"\{\}]/g, "").replace(/\:/g, "=").replace(/\,/g, "&");
                return url;
            }
        },
        /**
         * 獲取全部 url 參數,並轉換成 Json 對象
         * @ Param { string } url 
         */

        getUrlAllParams: function(url) {
            var url = url ? url : window.location.href;
            var _pa = url.substring(url.indexOf('?') + 1),
                _arrS = _pa.split('&'),
                _rs = {};
            for (var i = 0, _len = _arrS.length; i < _len; i++) {
                var pos = _arrS[i].indexOf('=');
                if (pos == -1) {
                    continue;
                }
                var name = _arrS[i].substring(0, pos),
                    value = window.decodeURIComponent(_arrS[i].substring(pos + 1));
                _rs[name] = value;
            }
            return _rs;
        }
    });

    // TurboFrame.browser.prototype = TurboFrame.browser;

})(window.TurboFrame);


// import TurboResize from './TurboFrame_Resize.js';
// export function TurboBrowser() {}

// export { TurboBrowser as default };