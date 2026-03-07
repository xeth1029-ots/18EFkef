;
(function(TurboFrame) {


  // Fragment  ==>  TurboFrame.()
  TurboFrame.extend({
    // 返回片段節點
    fragmentNode: function(html, name, properties) {

      var dom,
        nodes,
        container,
        args = [],
        properties = properties || {},
        methodAttributes = ['val', 'css', 'html', 'text', 'data', 'width', 'height', 'offset'];
      if (singleTagRE.test(html)) dom = TurboFrame(document.createElement(RegExp.$1));
      console.log(this)
      if (!dom) {
        if (html.replace) html = html.replace(tagExpanderRE, "<$1></$2>");
        if (name === undefined) name = fragmentRE.test(html) && RegExp.$1;
        if (!(name in containers)) name = "*";
        container = containers[name];
        container.innerHTML = "" + html;
        TurboFrame.each(container.childNodes, function(i, child) {
          //(args[i] = child)  =  (args.push(child))
          args[i] = child;
        });
        dom = TurboFrame.each(args, function() {
          container.removeChild(this);
        });
      }

      if (TurboFrame.isPlainObject(properties)) {
        nodes = TurboFrame(dom);
        TurboFrame.each(properties, function(key, value) {
          if (methodAttributes.indexOf(key) > -1) nodes[key](value);
          else nodes.attr(key, value);
        });
      }

      return dom;
    },

    // 解析 Json
    parseJSON: function(data) {
      if (window.JSON && window.JSON.parse) return window.JSON.parse(data);
      if (data === null) return data;

      if (typeof data === "string") {
        data = TurboFrame.trim(data);

        if (data) {
          if (rValidchars.test(data.replace(rValidescape, "@").replace(rValidtokens, "]").replace(rValidbraces, ""))) {
            return new Function("return " + data)();
          }
        }
      }

      TurboFrame.error("Invalid JSON: " + data);
    },

    // 解析字串為 Dom 節點
    parseNodes: function(data) {
      var wrap = document.createElement("div"),
        rets = [],
        cur;
      wrap.innerHTML = data;
      cur = wrap.firstChild;

      while (cur && cur.nodeType !== 9) {
        rets.push(cur);
        cur = cur.nextSibling;
      }

      wrap = null;
      return rets;
    },

    // 返回節點集合
    buildFragment: function(nodes) {
      var frag = document.createDocumentFragment(),
        i = 0,
        len = nodes.length;

      for (; i < len; i++) {
        frag.appendChild(nodes[i]);
      }
      return frag;
    }
  });

})(window.TurboFrame);
// import TurboCore from './TurboFrame_Core.js';
// export function TurboFragments() {}
// export {
//   TurboFragments as
//   default
// };