document.addEventListener("DOMContentLoaded", function() {

  // Hero Slider
  const heroSlider = new Swiper('.js-swiper-hero', {
    effect: 'coverflow',
    grabCursor: true,
    centeredSlides: true,
    slidesPerView: 'auto',
    coverflow: {
      rotate: 20,
      stretch: 0,
      depth: 200,
      modifier: 1,
      slideShadows: true,
    },
    // loop: true,
    pagination: {
      el: '.js-swiper-hero-pagination',
      clickable: true,
    },
    navigation: {
      nextEl: '.js-swiper-hero-button-next',
      prevEl: '.js-swiper-hero-button-prev',
    },
  });

  // Teacher Slider
  const teacherSlider = new Swiper('.js-swiper-card-grid', {
    autoplay: {
      delay: 8000,
      disableOnInteraction: false,
    },
    pagination: {
      el: '.js-swiper-slides-per-view-auto-pagination',
      clickable: true,
    },
    navigation: {
      nextEl: '.js-swiper-card-grid-button-next',
      prevEl: '.js-swiper-card-grid-button-prev',
    },
    slidesPerView: 1,
    spaceBetween: 30,
    loop: 1,
    breakpoints: {
      480: {
        slidesPerView: 2
      },
      768: {
        slidesPerView: 3
      },
      1024: {
        slidesPerView: 4
      },
    },
    on: {
      'imagesReady': function(swiper) {
        const preloader = swiper.el.querySelector('.js-swiper-preloader')
        //preloader.parentNode.removeChild(preloader)  error?   ²¾°£ by.Senya
      }
    }
  });

});