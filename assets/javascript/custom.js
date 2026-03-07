document.addEventListener("DOMContentLoaded", function() {
  // Darkmode
  const darkmodeBtns = document.getElementById("tuChangeMode");
  const darkmode = new JsDarkMode(darkmodeBtns, {
    dark_linearGradient: "#fcf81c,#d5ef0d,#b7ff4a",
    light_linearGradient: "#97e6ff,#1b449c,#006bfc",
  });

  // Go Top
  const scrEl = document.getElementById("tuScrolltop")
  const scrollTop = new JsTuScrolltop(scrEl, {
    offset: 300,
    speed: 100
  });

  setTimeout(function() {}, 2000)
  // imagesLazyLoad 
  const imagesLazyLoad = JsImagesLazyLoad("img[data-src]");
  
  // JsImagePlaceholder("img.js-placeholder",{
  //   bgColor: '#d7d3dc',
  //   textColor: '#52406b'
  // });


});

window.addEventListener("load", function(event) {

});