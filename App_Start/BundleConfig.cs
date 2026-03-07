using System.Web;
using System.Web.Optimization;

namespace WKEFSERVICE
{
    public class BundleConfig
    {
        // 如需「搭配」的詳細資訊，請瀏覽 http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            // 將 EnableOptimizations 設為 false 以進行偵錯。如需詳細資訊，
            // 請造訪 http://go.microsoft.com/fwlink/?LinkId=301862
            // 使用開發版本的 Modernizr 進行開發並學習。然後，當您
            // 準備好實際執行時，請使用 http://modernizr.com 上的建置工具，只選擇您需要的測試。

            //assets_new17 js
            bundles.Add(new ScriptBundle("~/bundles/assets_new17_js").Include(
                "~/Scripts/jquery-{version}.js", "~/Scripts/jquery-confirm.min.js", "~/Scripts/jquery.blockUI.js",
                "~/Scripts/global.js",
                "~/assets_new/javascript/custom.js",
                "~/assets_new17/javascript/components/categoryPicker/categoryPicker.js",
                "~/assets_new17/javascript/components/JsAlerts.js",
                "~/assets_new17/javascript/components/JsDarkMode.js",
                "~/assets_new17/javascript/components/JsHeader.js",
                "~/assets_new17/javascript/components/JsMenu.js",
                "~/assets_new17/javascript/components/JsTuScrolltop.js",
                "~/assets_new17/javascript/observer/JsBuildNav.js",
                "~/assets_new17/javascript/observer/JsImagesLazyLoad.js",
                "~/assets_new17/javascript/observer/JsObserverAnimations.js",
                "~/assets_new17/javascript/observer/JsScrollStickyObserver.js",
                "~/assets_new17/javascript/turboframes_justJavascript/turboframe_JsEventHandler.js",
                "~/assets_new17/javascript/turboframes_justJavascript/turboframe_JsUtils.js",
                "~/assets_new17/vendor/bootstrap/dist/js/bootstrap.bundle.js",
                "~/assets_new17/vendor/swiper/swiper-bundle.min.js"
            ));

            //assets_new17_css
            bundles.Add(new StyleBundle("~/Content/assets_new17_css").Include(
                "~/assets_new17/vendor/slim-icon/styles.css",
                "~/assets_new17/css/all.css",
                "~/css/wda.css"
                ));


            //assets_new js
            bundles.Add(new ScriptBundle("~/bundles/assets_new_js").Include(
                "~/Scripts/jquery-{version}.js", "~/Scripts/jquery-confirm.min.js", "~/Scripts/jquery.blockUI.js",
                "~/Scripts/global.js",
                "~/assets_new/vendor/@popperjs/core/dist/umd/popper.js",
                "~/assets_new/vendor/bootstrap/dist/js/bootstrap.bundle.min.js",
                "~/assets_new/vendor/swiper/swiper-bundle.min.js", "~/assets_new/javascript/js-template.js",
                "~/assets_new/javascript/expansions.js", "~/assets_new/javascript/polyfills/turboframe_polyfills_bundle.min.js",
                "~/assets_new/javascript/turboframes_justJavascript/turboframe_JsEventHandler.js",
                "~/assets_new/javascript/turboframes_justJavascript/turboframe_JsUtils.js", "~/assets_new/javascript/custom.js",
                "~/assets_new/javascript/components/JsImagePlaceholder.js", "~/assets_new/javascript/components/JsDarkMode.js",
                "~/assets_new/javascript/components/JsTuScrolltop.js", "~/assets_new/javascript/components/categoryPicker/categoryPicker.js",
                "~/assets_new/javascript/observer/JsImagesLazyLoad.js", "~/assets_new/javascript/components/canvasBlur.js",
                "~/assets_new/javascript/observer/JsObserverAnimations.js", "~/assets_new/javascript/observer/JsBuildNav.js",
                "~/assets_new/javascript/observer/JsScrollStickyObserver.js", "~/assets_new/javascript/components/JsHeader.js",
                "~/assets_new/javascript/components/JsMenu.js", "~/assets_new/vendor/fslightbox/fslightbox.js",
                "~/assets_new/javascript/components/JsAlerts.js",
                "~/Scripts/polyfill/pointer_events_polyfill.js", "~/Scripts/polyfill/ofi.min.js", "~/Scripts/global_1.js",
                "~/Scripts/OwlCarousel2-2.3.4/dist/owl.carousel.min.js"
                ));

            //assets_new_css
            bundles.Add(new StyleBundle("~/Content/assets_new_css").Include(
                "~/assets_new/vendor/slim-icon/styles.css",
                "~/assets/vendor/icon-turbotech/style.css",
                "~/assets_new/css/all.css", "~/css/wda.css",
                "~/Scripts/OwlCarousel2-2.3.4/dist/assets/owl.carousel.min.css"
                ));

            //~/Content/backend_assets_css
            bundles.Add(new StyleBundle("~/Content/backend_assets_css").Include(
                "~/assets_new/vendor/slim-icon/styles.css",
                "~/assets_new/css/all.css",
                "~/css/wda.css"
                ));

            //backend_assets_js
            bundles.Add(new ScriptBundle("~/bundles/backend_assets_js").Include(
                "~/Scripts/jquery-{version}.js", "~/Scripts/jquery-confirm.min.js", "~/Scripts/jquery.blockUI.js",
                "~/Scripts/global.js",
                //"~/assets_new/javascript/charts/Jschartist-area.js",   // 原本後台登入F12會跳錯，拿掉這個就不會了，只是不知道會發生什麼事，初步看起來沒影響，再觀察看看 - by.Senya
                //"~/assets_new/javascript/charts/Jschartist-bar.js",    // 原本後台登入F12會跳錯，拿掉這個就不會了，只是不知道會發生什麼事，初步看起來沒影響，再觀察看看 - by.Senya
                //"~/assets_new/javascript/charts/Jschartist-donut.js",  // 原本後台登入F12會跳錯，拿掉這個就不會了，只是不知道會發生什麼事，初步看起來沒影響，再觀察看看 - by.Senya
                //"~/assets_new/javascript/charts/Jschartist-pie.js",    // 原本後台登入F12會跳錯，拿掉這個就不會了，只是不知道會發生什麼事，初步看起來沒影響，再觀察看看 - by.Senya
                "~/assets_new/javascript/components/canvasBlur.js",
                "~/assets_new/javascript/components/categoryPicker/categoryPicker.js",
                "~/assets_new/javascript/components/JsDarkMode.js",
                "~/assets_new/javascript/components/JsImagePlaceholder.js",
                "~/assets_new/javascript/components/JsMenu.js",
                "~/assets_new/javascript/components/JsTuScrolltop.js",
                "~/assets_new/javascript/core.js",
                "~/assets_new/javascript/custom.js",
                "~/assets_new/javascript/forms/JsInputMask.js",
                "~/assets_new/javascript/observer/JsImagesLazyLoad.js",
                "~/assets_new/javascript/turboframes_justJavascript/turboframe_JsEventHandler.js",
                "~/assets_new/javascript/turboframes_justJavascript/turboframe_JsUtils.js",
                "~/assets_new/javascript/vendors/pikaday.js",
                "~/assets_new/vendor/@popperjs/core/dist/umd/popper.js",
                "~/assets_new/vendor/bootstrap/dist/js/bootstrap.bundle.min.js",
                "~/assets_new/vendor/chartist/chartist.min.js"
                ));

            //assets_new_css
            bundles.Add(new StyleBundle("~/Content/assets_new_css").Include(
                "~/assets_new/vendor/slim-icon/styles.css",
                "~/assets/vendor/icon-turbotech/style.css",
                "~/assets_new/css/all.css", "~/css/wda.css",
                "~/Scripts/OwlCarousel2-2.3.4/dist/assets/owl.carousel.min.css"
                ));

            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            #region 前台
            bundles.Add(new StyleBundle("~/Content/owlcarousel").Include(
                   // owl carousel
                   "~/vendor/owl.carousel/dist/assets/owl.carousel.min.css",
                   "~/vendor/owl.carousel/dist/assets/owl.theme.default.min.css"));

            bundles.Add(new StyleBundle("~/Content/FontBootstrap").Include(
                   // bootstrap
                   "~/Content/front/bootstrap.css",
                   // font-awesome
                   "~/Content/fontawesome-5.6.3.css",
                   // jquery blockUI
                   "~/Content/jquery-confirm.min.css"));

            //"~/vendor/jquery/dist/jquery.js",
            bundles.Add(new ScriptBundle("~/bundles/FrontBootstrapScript").Include(
                   // jquery // jquery blockUI
                   "~/Scripts/jquery-{version}.js", "~/Scripts/jquery-confirm.min.js", "~/Scripts/jquery.blockUI.js",
                   // bootstrap
                   "~/Scripts/front/bootstrap.js",
                   // popper
                   "~/Scripts/popper.js"));

            bundles.Add(new StyleBundle("~/Content/FontBootstrapTable").Include(
                   // bootstrap-table
                   "~/Content/bootstrap-table.css"));

            bundles.Add(new ScriptBundle("~/bundles/FrontOwlScript").Include(
                   // bootstrap-table
                   "~/Scripts/bootstrap-table.js",
                   // owl carousel
                   "~/vendor/owl.carousel/dist/owl.carousel.js",
                   // GoTop
                   "~/Scripts/front/gotop.js",
                   // main
                   "~/Scripts/front/main.js"));

            bundles.Add(new ScriptBundle("~/bundles/APISample").Include(
                 "~/Scripts/APISample/AllComponentErrCode.js",
                 "~/Scripts/APISample/CheckAndLoad.js",
                 "~/Scripts/APISample/HCAAPISVIAdapter.js",
                 "~/Scripts/APISample/env.js"));

            bundles.Add(new StyleBundle("~/Content/base").Include(
                "~/Content/front/base.css", new CssRewriteUrlTransform()));

            bundles.Add(new StyleBundle("~/Content/main").Include(
                "~/Content/front/main.css", new CssRewriteUrlTransform()));
            #endregion

            #region 後台

            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                "~/Scripts/jquery-{version}.js", "~/Scripts/jquery-confirm.min.js", "~/Scripts/jquery.blockUI.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                "~/Scripts/bootstrap.js", "~/Scripts/bootstrap-treeview.js", "~/Scripts/respond.js"));

            bundles.Add(new ScriptBundle("~/bundles/globaljs").Include(
                "~/Scripts/global.js", "~/Scripts/print.js"));

            bundles.Add(new StyleBundle("~/Content/jquery").Include(
                "~/Content/jquery-confirm.min.css"));

            bundles.Add(new StyleBundle("~/Content/bootstrap").Include(
                "~/Content/bootstrap.css",
                "~/Content/bootstrap-treeview.css",
                "~/Content/font-awesome-4.7.0.min.css",
                "~/Content/fontawesome-5.6.3.css"));

            //toastr
            bundles.Add(new ScriptBundle("~/Script/toastr").Include(
                "~/Scripts/toastr.min.js"));

            //toastr
            bundles.Add(new ScriptBundle("~/Script/knockout").Include(
                "~/Scripts/knockout-3.4.2.js", "~/Scripts/knockout.mapping.min.js"));

            //toastr
            bundles.Add(new StyleBundle("~/CSS/toastr").Include(
                 "~/Content/toastr.min.css"));

            bundles.Add(new StyleBundle("~/Content/base_back").Include(
                "~/Content/base_back.css", new CssRewriteUrlTransform()));

            #endregion


        }
    }
}
