using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Optimization;

namespace TS_WebApp
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {

            bundles.Add(new ScriptBundle("~/bundles/app").Include(
                "~/Scripts/vue.js",
                "~/Scripts/jquery.3.5.1.min.js",
                "~/Scripts/jquery.signalR-2.4.1.js",
                "~/Scripts/signalr/dist/browser/signalr.js",
                "~/Scripts/app/app.js"));
            
            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                "~/Scripts/bootstrap.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                 "~/Content/bootstrap.css",
                 "~/Content/Site.css"));
        }
    }
}
