using System.Web;
using System.Web.Optimization;

namespace AssetSystemWeb
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
     
            bundles.Add(new StyleBundle("~/Content/ThemeCss").Include(
               "~/Content/bootstrap.min.css",
                  "~/Content/themes/base/jquery-ui.css",
               "~/Content/bootstrap-select.min.css",
               "~/ThemeCode/metisMenu/metisMenu.min.css",
               "~/ThemeCode/Datatable/datatables-plugins/dataTables.bootstrap.css",
                "~/ThemeCode/Datatable/datatable-responsive/dataTables.responsive.css",
                "~/ThemeCode/sb-css/sb-admin-2.css",
                "~/Content/typeahead.css",
                "~/ThemeCode/font-aw/css/font-awesome.min.css",
                "~/Content/MyStyleSheet.css",
                "~/Content/loadingScreenStyle.css"
                ));


            bundles.Add(new ScriptBundle("~/bundles/ThemeJs").
            Include(
           "~/Scripts/jquery-3.1.1.js",
           "~/Scripts/jquery.validate.min.js",
            "~/Scripts/jquery.validate.unobtrusive.min.js",
             "~/Scripts/CustomScripts/GlobalVariables.js",
            "~/Scripts/bootstrap.js",
            "~/Scripts/bootstrap-select.min.js",
            "~/Scripts/jquery-ui-1.12.1.min.js",
            "~/ThemeCode/metisMenu/metisMenu.min.js",
            "~/ThemeCode/Datatable/js/js/jquery.dataTables.min.js",
            "~/ThemeCode/Datatable/js/js/dataTables.bootstrap.js",
            "~/ThemeCode/Datatable/datatable-responsive/dataTables.responsive.js",
            "~/ThemeCode/js/sb-admin-2.min.js"

            ));

           

            bundles.Add(new StyleBundle("~/Content/ReportCss").Include(
              "~/Content/bootstrap.min.css",
               "~/Content/MyStyleSheetReport.css"
               ));




            bundles.Add(new ScriptBundle("~/bundles/AccessRegistrationView").
               Include(
                         "~/Scripts/CustomScripts/GlobalVariables.js",
                        "~/Scripts/CustomScripts/AccountListChildDetailJs.js"
               ));


            bundles.Add(new ScriptBundle("~/bundles/AccessIndex").
               Include(
                    "~/Scripts/CustomScripts/GlobalVariables.js",
                     "~/Scripts/CustomScripts/UserProfile.js"
               ));






            bundles.Add(new ScriptBundle("~/bundles/UserProfileIndex").
               Include(
            "~/Scripts/jquery-3.1.1.js",
                "~/Scripts/CustomScripts/GlobalVariables.js",
            "~/Scripts/CustomScripts/UserProfile.js"
               ));




            bundles.Add(new ScriptBundle("~/bundles/AssetIndex").
              Include(
           "~/Scripts/jquery-3.1.1.js",
               "~/Scripts/CustomScripts/GlobalVariables.js",
           "~/Scripts/CustomScripts/AssetScripts.js"
              ));




            bundles.Add(new ScriptBundle("~/bundles/ConsignmentIndex").
              Include(
           "~/Scripts/jquery-3.1.1.js",
               "~/Scripts/CustomScripts/GlobalVariables.js",
           "~/Scripts/CustomScripts/ConsignmentScripts.js"
              ));

            bundles.Add(new ScriptBundle("~/bundles/CreateCosignment").
            Include(
         "~/Scripts/jquery-3.1.1.js",
         "~/Scripts/jquery.validate.min.js",
         "~/Scripts/jquery.validate.unobtrusive.min.js",
             "~/Scripts/CustomScripts/GlobalVariables.js",
         "~/Scripts/CustomScripts/ConsignmentScripts.js"
            ));

            //"~/Scripts/jquery-3.1.1.js",

            bundles.Add(new ScriptBundle("~/bundles/AddNewAssetDetail").
               Include(

           "~/Scripts/jquery-3.1.1.js",
               "~/Scripts/CustomScripts/GlobalVariables.js",
              "~/Scripts/CustomScripts/AssetScripts.js",
              "~/Scripts/CustomScripts/AssetDetailRows.js"

               ));



            bundles.Add(new ScriptBundle("~/bundles/AssetItemDetailRows").
        Include(

                    "~/Scripts/CustomScripts/GlobalVariables.js",

       "~/Scripts/CustomScripts/AssetDetailRows.js"

        ));



            bundles.Add(new ScriptBundle("~/bundles/AddNewAssetRows").
              Include(

          "~/Scripts/jquery.validate.min.js",
            "~/Scripts/jquery.validate.unobtrusive.min.js",
                "~/Scripts/CustomScripts/GlobalVariables.js",
             "~/Scripts/CustomScripts/AssetDetailRows.js"

              ));

            bundles.Add(new ScriptBundle("~/bundles/UpgradeAssetBuffer").
               Include(
              "~/Scripts/CustomScripts/GlobalVariables.js",
              "~/Scripts/CustomScripts/AssetDetailRows.js"
               ));



            bundles.Add(new ScriptBundle("~/bundles/UpgradeAssetDetails").
              Include(
                        "~/Scripts/jquery-3.1.1.js",
                        "~/Scripts/jquery.validate.min.js",
                        "~/Scripts/jquery.validate.unobtrusive.min.js",
                        "~/Scripts/CustomScripts/AssetScripts.js",
                         "~/Scripts/CustomScripts/GlobalVariables.js",
                          "~/Scripts/CustomScripts/AssetDetailRows.js"
              ));

            //"~/Scripts/jquery-3.1.1.js",
            //            "~/Scripts/jquery.validate.min.js",
            //            "~/Scripts/jquery.validate.unobtrusive.min.js",


            bundles.Add(new ScriptBundle("~/bundles/AddNewAssetItem").
              Include(
                   "~/Scripts/jquery-3.1.1.js",
               "~/Scripts/jquery-ui-1.12.1.min.js",
                        "~/Scripts/CustomScripts/GlobalVariables.js",

                        "~/Scripts/CustomScripts/AssetScripts.js"
              ));


            bundles.Add(new ScriptBundle("~/bundles/AssetItemBundle").
              Include(
                   "~/Scripts/jquery-3.1.1.js",
               "~/Scripts/jquery-ui-1.12.1.min.js",
               "~/Scripts/jquery.validate.min.js",
                "~/Scripts/jquery.validate.unobtrusive.min.js",
                 "~/Scripts/CustomScripts/GlobalVariables.js",
                 "~/Scripts/CustomScripts/AssetScripts.js"

              ));


            //-----------------------------------------------------------

            //     bundles.Add(new ScriptBundle("~/bundles/AccountBundle").
            //       Include(

            //        "~/Scripts/jquery.validate.min.js",
            //         "~/Scripts/jquery.validate.unobtrusive.min.js",
            //         "~/Scripts/CustomScripts/GlobalVariables.js",
            //      "~/Scripts/CustomScripts/AccountListChildDetailJs.js"

            //       ));








            bundles.Add(new ScriptBundle("~/bundles/ConsigmentBundle").
            Include(

             "~/Scripts/jquery-ui-1.12.1.min.js",

               "~/Scripts/jquery.validate.min.js",
                "~/Scripts/jquery.validate.unobtrusive.min.js",
               "~/Scripts/CustomScripts/GlobalVariables.js",
       "~/Scripts/CustomScripts/ConsignmentAssetDetailRow.js"

            ));

            //     bundles.Add(new ScriptBundle("~/bundles/jQueryUi").
            //  Include(
            //             "~/Scripts/jquery-3.1.1.js",
            //   "~/Scripts/jquery-ui-1.12.1.min.js"


            //  ));
            BundleTable.EnableOptimizations = true;




        }
    }
}
