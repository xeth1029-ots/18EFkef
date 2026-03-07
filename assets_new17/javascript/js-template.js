(function() {

    // 布爾類型屬性
    const propsMap = ['disabled', 'value', 'hidden', 'checked', 'selected', 'required', 'open', 'readonly', 'novalidate', 'reversed'];

    // HTML字符反轉義 &lt;  =>  < 
    function escape2Html(str) {
        var arrEntities = {
            'lt': '<',
            'gt': '>',
            'nbsp': ' ',
            'amp': '&',
            'quot': '"'
        };
        return str.replace(/&(lt|gt|nbsp|amp|quot);/ig, function(all, t) {
            return arrEntities[t];
        });
    }

    // parseFor  "(item, index) in items"  =>  {items:[item,index],items:items}
    function parseFor(strFor) {
        // 是否是對象
        const isObject = strFor.includes(' of ');
        const reg = /\s(?:in|of)\s/g;
        // "(item, index) in items" => ["(item, index)","items"]
        const [keys, obj] = strFor.match(reg) ? strFor.split(reg) : ["item", strFor];
        const items = Number(obj) > 0 ? `[${'null,'.repeat(Number(obj) - 1)}null]` : obj;
        // "(item, index)" => ["item","index"]
        const params = keys.split(/[\(|\)|,\s?]/g).filter(Boolean);
        return {
            isArrray: !isObject,
            items,
            params
        }
    }

    // 字符串轉模板字符串
    String.prototype.interpolate = function(params) {
        const names = Object.keys(params);
        const vals = Object.values(params).map(el => typeof el === 'function' ? el() : el);
        const str = this.replace(/\{\{([^\}]+)\}\}/g, '${$1}'); // {{xx}} => ${xx}
        return new Function(...names, `return \`${escape2Html(str)}\`;`)(...vals);
    };

    // 模板引擎 render
    HTMLTemplateElement.prototype.render = function(data, options) {
        const rule = this.getAttribute("rule") || '$';
        if (!this.$fragment) {
            this.$fragment = this.cloneNode(true);
            this.fragment = document.createElement('TEMPLATE');

            // 模板渲染之前 interpolateBefore
            this.interpolateBefore && this.interpolateBefore(this.$fragment, {
                data,
                options,
                rule
            });

            // $for 循環渲染 e.g. <div $for="list"></div>  =>   ${ list.map(function(item,index){ return '<div></div>' }).join('') }
            const repeatEls = this.$fragment.content.querySelectorAll(`[\\${rule}for]`);
            repeatEls.forEach(el => {
                const strFor = el.getAttribute(`${rule}for`);
                const {
                    isArrray,
                    items,
                    params
                } = parseFor(strFor);
                const name = isArrray ? '__index__' : (params[1] || 'name');
                const value = params[0] || (isArrray ? 'item' : 'value');
                const index = (isArrray ? params[1] : params[2]) || 'index';
                el.before('${Object.entries(' + items + ').map(function([' + `${name},${value}],${index}` + '){ return `');
                el.removeAttribute(`${rule}for`);
                el.after('`}).join("")}');
            })

            // $if 條件渲染 e.g. <div $if="if"></div>   =>    ${ if ? '<div></div>' : '' }
            const ifEls = this.$fragment.content.querySelectorAll(`[\\${rule}if]`);
            ifEls.forEach(el => {
                const ifs = el.getAttribute(`${rule}if`);
                el.before('${' + ifs + '?`');
                el.removeAttribute(`${rule}if`);
                el.after('`:`<!--if:' + el.tagName + '-->`}');
            })

            // fragment e.g. <fragment>aa</fragment>   =>  aa
            const fragments = this.$fragment.content.querySelectorAll('fragment,block');
            fragments.forEach(el => {
                el.after(el.innerHTML);
                el.remove();
            })

            // $show e.g. <div $show="show"></div>   =>  <div hidden="${!show}"></div>
            const showEls = this.$fragment.content.querySelectorAll(`[\\${rule}show]`);
            showEls.forEach(el => {
                const shows = el.getAttribute(`${rule}show`);
                el.setAttribute('hidden', '${!' + shows + '}')
                el.removeAttribute(`${rule}show`);
            })

            // $bind:class e.g. <div :class="{cur:show}"></div>   =>  <div class="${show?'cur':''}"></div>
            const classEls = this.$fragment.content.querySelectorAll(`[\\:class],[\\${rule}bind\\:class]`);
            classEls.forEach(el => {
                const classes = el.getAttribute(`:class`) || el.getAttribute(`${rule}bind:class`);
                if (classes.includes('{')) {
                    // object 格式
                    const classList = classes.replace(/\s/g, '').split(/[\{\}:,]/g).filter(Boolean);
                    for (let i = 0; i < classList.length; i += 2) {
                        el.classList.add("${!!" + classList[i + 1] + "?'" + classList[i] + "':''}");
                    }
                    el.removeAttribute(`:class`);
                    el.removeAttribute(`${rule}bind:class`);
                }
            })

            const keyEls = this.$fragment.content.querySelectorAll(`[\\:key],[\\${rule}bind\\:key]`);
            keyEls.forEach(el => {
                const key = el.getAttribute(`:key`) || el.getAttribute(`${rule}bind:key`);
                el.key = key;
                el.setAttribute('v-data-key', '${' + key + '}')
                el.removeAttribute(`:key`);
                el.removeAttribute(`${rule}bind:key`);
            })
        }
        this.fragment.innerHTML = this.$fragment.innerHTML.interpolate(data);

        // 表單 props 特殊處理，false 直接移除
        const propsEls = this.fragment.content.querySelectorAll(`[${propsMap.join('],[')}]`);
        propsEls.forEach(el => {
            propsMap.forEach(props => {
                // 如果這些屬性值是 false ，就直接移除
                if (el.getAttribute(props) === 'false') {
                    el.removeAttribute(props);
                }
            })
        })

        // 模板渲染之後 interpolateAfter
        this.interpolateAfter && this.interpolateAfter(this.fragment, {
            data,
            options,
            rule
        });
        return this.fragment;
    }
})();
(function() {
    // 屬性操作
    const propsMap = ['disabled', 'value', 'hidden', 'checked', 'selected', 'required', 'open', 'readonly', 'novalidate', 'reversed'];

    // html2Node '<div></div>' =>  <div></div>
    function html2Node(html) {
        return html.nodeType ? html : document.createRange().createContextualFragment(html);
    }

    // 比較屬性
    function diffAttr(oldAttributes, newAttributes) {
        const patch = [];
        const oldAttrs = Array.from(oldAttributes);
        const newAttrs = Array.from(newAttributes);
        //  判斷新的屬性和新的屬性的關係
        oldAttrs.forEach(attr => {
            const newAttr = newAttributes[attr.name] || {
                name: attr.name,
                value: false
            };
            if (attr.value !== newAttr.value) {
                patch.push(newAttr);
            }
        })
        // 舊節點沒有新節點的屬性
        newAttrs.forEach(attr => {
            if (!oldAttrs.find(el => el.name == attr.name)) {
                patch.push(attr);
            }
        })
        return patch;
    }

    // 比較子節點
    function diffNodes(oldNodes, newNodes, patches, oldNode) {

        const oldChildren = Array.from(oldNodes);
        const newChildren = Array.from(newNodes);

        const oldkey = oldChildren.map(el => el.nodeType === Node.ELEMENT_NODE ? el.getAttributeNode('v-data-key') : null).filter(Boolean);
        const newkey = newChildren.map(el => el.nodeType === Node.ELEMENT_NODE ? el.getAttributeNode('v-data-key') : null).filter(Boolean);

        // 有 key 的情況，僅限於 for 循環
        if (oldkey.length > 0) {
            oldkey.forEach((keynode, idx) => {
                // 如果新節點沒有舊節點的key，就移除舊節點
                if (!newkey.find(el => el.value === keynode.value)) {
                    oldkey.splice(idx, 1);
                    patches.push({
                        type: 'REMOVE',
                        el: keynode.ownerElement,
                    })
                }
            });
            newkey.forEach(keynode => {
                // 如果舊節點沒有新節點的key，就新增節點
                if (!oldkey.find(el => el.value === keynode.value)) {
                    oldkey.push(keynode);
                }
            });

            const sort = newkey.map(el => el.value);

            // 根據新的順序排序
            oldkey.sort((a, b) => sort.indexOf(a.value) - sort.indexOf(b.value));

            patches.push({
                type: 'SORT',
                newNode: oldkey.map(el => el.ownerElement),
                el: oldNode,
            })

            newkey.forEach((keynode, idx) => {
                // 如果不相等
                const newNode = keynode.ownerElement;
                const oldNode = oldkey[idx].ownerElement;
                if (!oldNode.isEqualNode(newNode)) {
                    walk(oldNode, newNode, patches);
                }
            });

        } else {
            // 比較舊的每一項
            oldChildren.forEach((child, idx) => {
                // 如果不相等
                if (!child.isEqualNode(newChildren[idx])) {
                    walk(child, newChildren[idx], patches);
                }

            });
            // 新增的節點
            newChildren.forEach((child, idx) => {
                if (!oldChildren[idx]) {
                    patches.push({
                        type: 'ADD',
                        newNode: child,
                        el: oldNode,
                    })
                }
            })
        }
    }

    // 比較差異
    function walk(oldNode, newNode, patches) {
        const currentPatch = {};
        if (!newNode) {
            // 沒有新節點就刪除
            currentPatch.type = 'REMOVE';
            currentPatch.el = oldNode;
        } else if (oldNode.nodeType === Node.TEXT_NODE && newNode.nodeType === Node.TEXT_NODE) {
            // 判斷是文本節點
            if (oldNode.textContent.replace(/\s/g, '') !== newNode.textContent.replace(/\s/g, '')) {
                currentPatch.type = 'TEXT';
                currentPatch.el = oldNode;
                currentPatch.text = newNode.textContent;
            }
        } else if (oldNode.nodeType === newNode.nodeType && newNode.nodeType === Node.ELEMENT_NODE) {
            // 比較屬性
            const attrs = diffAttr(oldNode.attributes, newNode.attributes);
            if (attrs.length > 0) {
                currentPatch.type = 'ATTRS';
                currentPatch.el = oldNode;
                currentPatch.attrs = attrs;
            }
            //  遍歷子節點
            diffNodes(oldNode.childNodes, newNode.childNodes, patches, oldNode);
        } else {
            //  節點被替換
            currentPatch.type = 'REPLACE';
            currentPatch.newNode = newNode;
            currentPatch.el = oldNode;
        }
        if (currentPatch.type) {
            patches.push(currentPatch);
        }
    }

    // diff
    function webDiff(container, html) {
        const patches = [];
        const newNode = html2Node(html);
        diffNodes(container.childNodes, newNode.childNodes, patches, container);
        return patches;
    }

    // setDom
    function setDom(container, html) {
        const patches = webDiff(container, html);
        // console.log(patches)
        patches.forEach(item => {
            switch (item.type) {
                case 'REMOVE':
                    item.el.remove();
                    break;
                case 'TEXT':
                    item.el.textContent = item.text;
                    break;
                case 'ATTRS':
                    item.attrs.forEach(attr => {
                        item.el.setAttribute(attr.name, attr.value);
                        if (propsMap.includes(attr.name)) {
                            item.el[attr.name] = attr.value;
                        }
                    })
                    break;
                case 'REPLACE':
                    item.el.replaceWith(item.newNode);
                    break;
                case 'ADD':
                    item.el.appendChild(item.newNode);
                    break;
                case 'SORT':
                    item.el.append(...item.newNode);
                    break;
                default:
                    break;
            }
        })
    }

    // dom對比渲染
    HTMLElement.prototype.html = function(html) {
        // html可以是字符串，也可以是dom
        setDom(this, html);
    }

    // 模板引擎 mount
    HTMLTemplateElement.prototype.mount = function(data, isDiff) {
        if (!this.container) {
            this.container = document.querySelector(`[is="${this.id}"]`);
        }
        if (this.container) {
            if (isDiff) {
                this.container.html(this.render(data).content);
            } else {
                this.container.innerHTML = this.render(data).innerHTML
            }
        } else {
            throw new Error('沒有找到屬性 is 為 ' + this.id + ' 的容器')
        }
    }

})();


// (function () {

//     // 事件
//     const eventList = ['submit', 'click', 'dblclick', 'input', 'change', 'focus', 'blur', 'keydown', 'keyup', 'keypress', 'scroll', 'submit', 'invalid', 'mousedown', 'mousemove', 'mouseup', 'mouseenter', 'mouseleave', 'drag', 'dragstart', 'dragenter', 'dragover', 'dragleave', 'dragend', 'drop'] 

//     // 表單綁定
//     const modelMap = [
//         {
//             selector: 'input:not([type=checkbox]):not([type=radio]),textarea',
//             attrName: 'value',
//             attrValue: (name) => '${' + name + '}',
//             event: 'input',
//             fn: (name) => `this.${name}=$this.value`
//         },
//         {
//             selector: 'select',
//             attrName: 'value',
//             attrValue: (name) => '${' + name + '}',
//             event: 'change',
//             fn: (name) => `this.${name}=$this.value`
//         },
//         {
//             selector: '[type=radio]',
//             attrName: 'checked',
//             attrValue: (name, value) => `\${${name}==${value}}`,
//             event: 'change',
//             fn: (name) => `this.${name}=$this.value`
//         },
//         {
//             selector: '[type=checkbox]',
//             attrName: 'checked',
//             attrValue: (name, value, isArray) => isArray ? `\${${name}.includes(${value})}` : `\${${name}}`,
//             event: 'change',
//             fn: (name, isArray) => isArray ? `function(){ if(this.${name}.includes($this.value)){ this.${name} = this.${name}.filter(el => el!==$this.value) } else { this.${name} = this.${name}.concat($this.value)}}` : `this.${name}=$this.checked`
//         },
//     ]

//     // parseFor  "(item, index) in items"  =>   {items:[item,index],items:items}
//     function parseFor(strFor) {
//         // 是否是對象
//         const isObject = strFor.includes(' of ');
//         const reg = /\s(?:in|of)\s/g;
//         const [keys, obj] = strFor.match(reg) ? strFor.split(reg) : ["item", strFor];
//         const items = Number(obj) > 0 ? `[${'null,'.repeat(Number(obj) - 1)}null]` : obj;
//         const params = keys.split(/[\(|\)|,\s?]/g).filter(Boolean);
//         return { isArrray: !isObject, items, params }
//     }

//     // 模板渲染之前
//     HTMLTemplateElement.prototype.interpolateBefore = function (fragment, { data, options: { computed }, rule }) {
//         // 表單綁定 $model="val"
//         const modelEls = fragment.content.querySelectorAll(`[\\${rule}model]`);
//         modelEls.forEach(el => {
//             let name = el.getAttribute(`${rule}model`);
//             const value = el.getAttribute('value');
//             const reg = /\$\{([^\}]+)\}/g;
//             const attrvalue = reg.test(value) ? value.replace(reg, '$1') : `'${value}'`;
//             // const forEl = el.closest(`[\\${rule}for]`);

//             // if (forEl) {
//             //     const strFor = forEl.getAttribute(`${rule}for`);
//             //     const { isArrray, items, params } = parseFor(strFor);
//             //     const item = params[0] || (isArrray ? 'item' : 'value');
//             //     const index = isArrray ? '__index__' : (params[1] || 'name');
//             //     name = name.replaceAll(item+'.',`${items}[${index}].`);
//             // }

//             modelMap.forEach(item => {
//                 if (el.matches(item.selector)) {
//                     if (el.tagName === 'SELECT' && !data[name]) {
//                         el.insertAdjacentHTML('afterbegin', '<option value="">請選擇</option>');
//                     }
//                     const isArray = Array.isArray(data[name]);
//                     el.setAttribute(item.attrName, item.attrValue(name, attrvalue, isArray));
//                     el.setAttribute('v-on:'+item.event, item.fn(name, isArray));
//                     el.removeAttribute(`${rule}model`);
//                 }
//             })
//         })
//         // on
//         // const onEls = this.$fragment.content.querySelectorAll(`[\\${rule}on],[my-on]`);
//         // onEls.forEach(el => {
//         //     const forEl = el.closest(`[\\${rule}for]`);
//         //     if (forEl) {
//         //         const strFor = forEl.getAttribute(`${rule}for`);
//         //         const { isArrray, params } = parseFor(strFor);
//         //         const name = isArrray ? '__index__' : (params[1] || 'name');
//         //         const index = (isArrray?params[1]:params[2])||'index';
//         //         const attr = el.attributes[`my-on`] || el.attributes[`${rule}on`];
//         //         if (attr) {
//         //             attr.value = attr.value.replace(RegExp(`(${name}|${index})`),'${$1}');
//         //         }
//         //     }
//         // })
//         // computed
//         Object.keys(computed).forEach(el => {
//             Object.defineProperty(data, el, {
//                 writable: false,
//                 enumerable: true,
//                 value: computed[el].bind(data)
//             })
//         })

//     }

//     // 模板渲染之後
//     HTMLTemplateElement.prototype.interpolateAfter = function (fragment, { options: { methods }, rule }) {
//         const app = this.app;
//         // select
//         const selectEls = fragment.content.querySelectorAll(`select`);
//         selectEls.forEach(el => {
//             if (!el.value) {
//                 el.selectedOptions[0].disabled = true;
//             }
//         })

//         // 綁定事件 @click="fn"
//         const eventEls = fragment.content.querySelectorAll(eventList.map( el => `[\\@${el}]`).join(',') + `,[v-on\\:input],[v-on\\:change]`);
//         eventEls.forEach(el => {
//             const attrs = Array.from(el.attributes).filter( attr => eventList.includes(attr.name.split(RegExp(`(?:@|v-on:|${rule}-on)`))[1]) )
//             attrs.forEach(attr => {
//                 if (methods && Object.keys(methods).length && app) {
//                     const name = attr.name.split(RegExp(`(?:@|v-on:|${rule}-on)`))[1];
//                     const events = attr.value;
//                     const fn = function (event) {
//                         if (events.includes('(') || events.includes('this')) {
//                             const names = Object.keys(methods);
//                             const vals = Object.values(methods);
//                             if (events.startsWith('function')) {
//                                 // $this , $event 內置DOM原生對象
//                                 return Function(...names, '$this', '$event', '"use strict";(' + events + ').call(this)').call(app.vm, ...vals, this, event);
//                             }
//                             return Function(...names, '$this', '$event', '"use strict";' + events.replace(/\(/, '.call(this,')).call(app.vm, ...vals, this, event);
//                         } else {
//                             return methods[events].call(app.vm);
//                         }
//                         // console.warn(name+'事件未定義，請在 methods 中添加');
//                     }
//                     if (name === 'input' && el.tagName === 'INPUT') {
//                         el.addEventListener(name, () => {
//                             this.timer && clearTimeout(this.timer);
//                             this.timer = setTimeout(() => {
//                                 fn.call(el)
//                             }, 100)
//                         });
//                     } else {
//                         el.addEventListener(name, fn);
//                     }
//                     //el.removeAttribute(attr.name);
//                 }
//             })
//         })
//     }

//     // 模板引擎 model
//     HTMLElement.prototype.model = function ({ data, created, methods = {}, computed = {}, watch = {} }) {
//         if (!this.html) {
//             throw new Error('請先引入 web-diff.js');
//         }
//         if (!this.template) {
//             this.template = document.querySelector(`template[is="${this.id}"]`);
//         }
//         if (this.template) {
//             if (!this.template.render) {
//                 throw new Error('請先引入 web-template.js');
//             }
//             const app = this;
//             if (!app.vm) {
//                 this.template.app = app;
//                 app.html(app.template.render(data, { methods, watch, computed }).content);
//                 // 數據更新
//                 let timer = null;
//                 const delay = (cb) => {
//                     timer && clearTimeout(timer);
//                     timer = setTimeout(() => {
//                         cb && cb(data);
//                     })
//                 }
//                 // 監聽
//                 const watches = {};

//                 Object.keys(watch).forEach(el => {
//                     watches[el] = data[el];
//                 })


//                 const handler = {
//                     get(target, key) {
//                         if (target[key] == null && methods[key]) {
//                             return methods[key];
//                         }
//                         if (typeof target[key] === 'object' && target[key] !== null) {
//                             return new Proxy(target[key], handler);
//                         }
//                         return Reflect.get(target, key);
//                     },
//                     set(target, key, value) {
//                         if (methods[key]) {
//                             return;
//                         }
//                         if (target[key] !== value) {
//                             delay((data) => {
//                                 if (watches[key] !== data[key] && watch[key]) {
//                                     watch[key](watches[key],data[key]);
//                                     watches[key] = data[key];
//                                 }
//                                 app.html(app.template.render(data, { methods, watch, computed }).content);
//                             })
//                         }
//                         return Reflect.set(target, key, value);
//                     }
//                 }

//                 Object.defineProperty(app, 'vm', {
//                     writable: false,
//                     value: new Proxy(data, handler)
//                 })
//                 // 初始化完成
//                 created && created.call(app.vm);
//             } else {
//                 throw new Error('不能重複調用model');
//             }
//         } else {
//             throw new Error(`沒有找到 is 屬性為 ${this.id} 的模板`);
//         }
//     }
// })()