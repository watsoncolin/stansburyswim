﻿using System.Web.Optimization;

namespace FillThePool.Web
{
    public static class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/site").Include(
                        "~/Scripts/lib/jquery-{version}.js",
						"~/Scripts/lib/jquery-ui-{version}.js",
						"~/Scripts/lib/modernizr-*",
                        "~/Scripts/lib/bootstrap.js",
                        "~/Scripts/lib/knockout-{version}.js",
                        "~/Scripts/lib/underscore.js",
						"~/Scripts/lib/H5F.js",
						"~/Scripts/app/scheduleDataService.js",
						"~/Scripts/app/studentDataService.js",
						"~/Scripts/app/userDataService.js",
						"~/Scripts/lib/moment.js",
						"~/Scripts/lib/jquery.lettering-0.6.1.min.js",
						"~/Scripts/lib/bootstrap-timepicker.min.js",
						"~/Scripts/app/validationUtility.js",
						"~/Scripts/site.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/lib/jquery.unobtrusive*",
                        "~/Scripts/lib/jquery.validate*"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                        "~/Content/themes/base/jquery.ui.core.css",
                        "~/Content/themes/base/jquery.ui.resizable.css",
                        "~/Content/themes/base/jquery.ui.selectable.css",
                        "~/Content/themes/base/jquery.ui.accordion.css",
                        "~/Content/themes/base/jquery.ui.autocomplete.css",
                        "~/Content/themes/base/jquery.ui.button.css",
                        "~/Content/themes/base/jquery.ui.dialog.css",
                        "~/Content/themes/base/jquery.ui.slider.css",
                        "~/Content/themes/base/jquery.ui.tabs.css",
                        "~/Content/themes/base/jquery.ui.datepicker.css",
                        "~/Content/themes/base/jquery.ui.progressbar.css",
                        "~/Content/themes/base/jquery.ui.theme.css",
						"~/Content/normalize.css",
                        "~/Content/bootstrap.css",
                        "~/Content/bootstrap-theme.css",
						"~/Content/bootstrap-timepicker.min.css",
						"~/Content/site.css"));
        }
    }
}