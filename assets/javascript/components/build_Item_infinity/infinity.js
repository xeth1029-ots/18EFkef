var url =
  "https://cdn.jsdelivr.net/gh/Barry028/TurboFrame@main/src/json/taiwan_zipCode.json";
var fetchByPromise = function(url, method) {
  // 發起請求,返回給調用者一個 Promise 對象
  return new Promise(function(resolve, reject) {
    fetch(url, {
        method: method // GET POST
      })
      .then((response) => response.json())
      .then((responseData) => {
        // 後渠道解析後的數據
        // console.log("responseData= " + JSON.stringify(responseData));
        return resolve(responseData);
      })
      .catch((error) => {
        return reject(error);
      });
  });
};
fetchByPromise(url)
  .then(function(json) {
    return jsonFormatAndBuild(json);
  })
  .catch(function(error) {
    console.log("error", error);
  });

var container = document.getElementById("container");

function jsonFormatAndBuild(json) {
  const data = json;
  const len = data.length; // 61460

  // var gridView = document.querySelector(".gallery_wrapper");
  const trigger = document.querySelector(".scroll-trigger");
  const observerOptions = {
    root: null,
    rootMargin: "48px",
    threshold: 0
  };

  const observer = new IntersectionObserver(entries => {
    // console.log({entries})
    entries.forEach(function(entry) {
      if (entry.isIntersecting) {
        buildItems(data, {
          buildArea: ".gallery_wrapper",
          buildTagName: "figure",
          buildTagClassName: "t-checkbox-list-item",
          loop: 20,
          dataArray: ["Zip5", "City", "Area", "Road", "Scope"],
          html: `<input type="hidden" id="t_checkbox_group_0@(_lens_)@_value" class="hidden-value" />
                <input type="checkbox" id="t_checkbox_group_0@(_lens_)@" />
                <label class="t-checkbox-group" for="t_checkbox_group_0@(_lens_)@">
                    <div class="t-checkbox-group-box">
                      選擇
                    </div>
                    <figcaption></figcaption>
                    <ul class="t-checkbox-group-list">
                      <li>Zip5: @(Zip5)@ </li>
                      <li>City: @(City)@ </li>
                      <li>Area: @(Area)@ </li>
                      <li>Road: @(Road)@ </li>
                      <li>Scope: @(Scope)@</li>
                    </ul>
                  </label>`
        });
      }
    });
  }, observerOptions);

  observer.observe(trigger);

  function buildItems(data, options) {
    const that = this;
    const body = document.getElementsByTagName("BODY")[0];

    that.data = "";
    that.options = {
      buildArea: body,
      buildTagName: "div",
      buildTagClassName: "",
      loop: 0,
      html: "",
      dataArray: ""
    };

    let dataLength = JsUtils.map(options.dataArray, function(item, index, array) {
      item.name = options.dataArray.name;
      return array.length;
    }, options.dataArray);

    // let aaaa = JsUtils.map(data, function(item, index, array) {
    //   var n = [];
    //   console.log(item["嘉義市"]);
    //   console.log(item.City);
    //   var pp =  item["嘉義市"]
    //   // if (item.City = "嘉義市") {
    //     n.push(pp)
    //   // }
    //   console.log(n);
    // }, data);
    console.log(data);

    let lensData = dataLength.length;
    // const dataLens = jsonLength(data[0]);
    const dataArray = options.dataArray;

    // Area: "中正區"
    // City: "臺北市"
    // Road: "八德路１段"
    // Scope: "全"
    // Zip5: "10058"
    // 去重
    let maps = {};
    let newArrObj = [];
    for (let i = 0; i < data.length; i++) {
      let newData = data[i];
                 console.log( newData['Area'])
      if (!maps[newData['City']]) {
           console.log( newData.length)
        newArrObj.push({
          ID: newData['City'],
          Area: newData['Area'],
          // [dataArray[1]]: newData[dataArray[1]],
          DATAS: [{
            ID: newData.ZIPCODE,
            Area: newData['Area'],
            // [dataArraySub[0]]: newData[dataArraySub[0]],
            // [dataArraySub[1]]: newData[dataArraySub[1]]
          }]
        });
        // console.log(newArrObj)
        // maps[newData[dataArray[0]]] = newData;
      }
      // for (let j = 0; j < 24; j++) {
      //   let dataJson = newArrObj[j];
      //   if (dataJson[Area] == newData[Area]) {
      //     dataJson.DATAS.push({
      //       ID: newData.ZIPCODE,
      //      Area: newData[Area],
      //       // [dataArraySub[0]]: newData[dataArraySub[0]],
      //       // [dataArraySub[1]]: newData[dataArraySub[1]]
      //     });
      // console.log(dataJson)
      //     break;
      //   }
      // }
    }

    function stringBind(srting, data) {
      return srting.replace(/@\((\w+)\)@/gi, function(match, key) {
        return typeof data[key] === "undefined" ? "" : data[key];
      });
    }

    function jsonLength(json) {
      return Object.keys(json).length;
    }

    function getUniqueId(prefix) {
      return prefix + Math.floor(parseFloat('0.'+crypto.getRandomValues(new Uint32Array(1))[0]) * new Date().getTime());
    }

    var options = Object.assign({}, that.options, options || {});
    const fragment = document.createDocumentFragment();
    const selector = document.querySelector(options.buildArea);
    for (let i = 0; i < options.loop; i++) {
      const element = document.createElement(options.buildTagName);
      const keys = Object.keys(data[i]);
      const values = Object.values(data[i]);

      let formatObj = function(lensData) {
        const newFormatObj = {
          _lens_: getUniqueId(i) // 產生變數 id (因為一次只載入 20 筆)
        };
        let obj = dataArray.forEach(function(item, i) {
          const newContent = {};
          let name = item;
          for (let i = 0; i < lensData; i++) {
            objN = Object.assign(newFormatObj, {
              [dataArray[i]]: values[i],
            })
          }
          return objN;
        })
        return newFormatObj;
      }
      // console.log( formatObj(lensData))
      inner = options.html;
      var inner = stringBind(inner, formatObj(lensData));
      element.className = options.buildTagClassName;
      element.innerHTML = inner;
      fragment.appendChild(element);
    }
    return selector.append(fragment);
  }
}