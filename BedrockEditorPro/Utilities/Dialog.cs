using ArcGIS.Core.Geometry;
using ArcGIS.Desktop.Catalog;
using ArcGIS.Desktop.Core;
using ArcGIS.Desktop.Framework;
using ArcGIS.Desktop.Internal.Mapping.Table.QueryTable;
using ArcGIS.Desktop.Mapping;
using BedrockEditorPro.Controls;
using BedrockEditorPro.CustomDialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BedrockEditorPro.Utilities
{
    internal class Dialog
    {
        //Source
        //https://github.com/Esri/arcgis-pro-sdk-community-samples/blob/master/Content/OpenItemDialogBrowseFilter/CustomFilters.cs#L41

        //Events
        public static EventHandler<SpatialReference> spatialReferenceSelected; //This event is triggered when a different a spatial reference is selected from sr dialog
        public static EventHandler<string> colorSymbolReferenceSelected; //This event is triggered when a symbol/color value is selected within the symbol dialog

        public Dialog() { }

        /// <summary>
        /// Will prompt a custom XML file dialog, that uses special custom file filter class.
        /// </summary>
        /// <returns></returns>
        public string GetXMLFilePrompt()
        {
            string xmlFilePath = string.Empty;

            //Create new browser filter
            var bpf = new BrowseProjectFilter("xml_file");

            //Add typeID for Polygon feature class
            bpf.AddCanBeTypeId("xml_file");
            bpf.FileExtension = ".xml";

            //Display only folders and GDB in the browse dialog
            bpf.Includes.Add("FolderConnection");
            //Does not display Online places in the browse dialog
            bpf.Excludes.Add("esri_browsePlaces_Online");

            //Display the filter in an Open Item dialog
            OpenItemDialog aNewFilter = new OpenItemDialog
            {
                Title = BedrockEditorPro.Properties.Resources.DialogXMLPromptTitle,
                MultiSelect = false,
                BrowseFilter = bpf,
                Filter = "XML Files (*.xml)|*.xml|All Files (*.*)|*.*"
            };

            bool? ok = aNewFilter.ShowDialog();

            if (ok.HasValue && ok.Value && aNewFilter.Items.Count() > 0)
            {
                xmlFilePath = aNewFilter.Items[0].Path;
            }

            return xmlFilePath;
        }

        /// <summary>
        /// Will prpt a custom FGDB file dialog, that uses special custom file filter class.
        /// </summary>
        /// <returns></returns>
        public string GetFGDBSavePrompt()
        {
            string fgdbPath = string.Empty;

            //Create new browser filter
            var bpf_gdb = new BrowseProjectFilter("fgdb");

            //Add typeID for file geodatabase
            bpf_gdb.AddCanBeTypeId("database_fgdb");

            //Display only folders and GDB in the browse dialog
            bpf_gdb.Includes.Add("FolderConnection");
            bpf_gdb.Includes.Add("FileGeodatabase");
            bpf_gdb.FileExtension = ".gdb";

            //Does not display Online places in the browse dialog
            bpf_gdb.Excludes.Add("esri_browsePlaces_Online");

            //Display the filter in an Open Item dialog
            SaveItemDialog aNewFilter = new SaveItemDialog
            {
                Title = BedrockEditorPro.Properties.Resources.DialogFGDBSavePromptTitle,
                BrowseFilter = bpf_gdb,
                DefaultExt = ".gdb",
                OverwritePrompt = true,
            };

            bool? ok = aNewFilter.ShowDialog();

            if (ok.HasValue && ok.Value && aNewFilter.FilePath != string.Empty)
            {
                fgdbPath = aNewFilter.FilePath;
            }

            return fgdbPath;
        }

        /// <summary>
        /// Will prompt a projection dialog (spatial reference)
        /// </summary>
        /// <returns>Returns a spatial reference object</returns>
        public static void GetProjectionPrompt()
        {
            try
            {
                //Variable
                SpatialReference spatialReference = null;
                CoordSysDialog _coordDialog = null;

                //Create a new dialog instance
                _coordDialog = new CoordSysDialog();
                _coordDialog.Closing += _coordDialog_Closing;
                _coordDialog.Owner = FrameworkApplication.Current.MainWindow;
                _coordDialog.Show();
            }
            catch (Exception e)
            {
                new ErrorService(e).WriteToFile("", false);
            }


        }

        /// <summary>
        /// Event handler for the projection dialog closing event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void _coordDialog_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            CoordSysDialog senderDialog = sender as CoordSysDialog;
            if (senderDialog != null && senderDialog.SpatialReference != null)
            {
                //Send call to refresh other pages
                EventHandler<SpatialReference> spatialReferenceRequest= spatialReferenceSelected;
                if (spatialReferenceRequest != null)
                {
                    spatialReferenceRequest(sender, senderDialog.SpatialReference);
                }

            }
        }

        /// <summary>
        /// Will prpt a custom FGDB file dialog, that uses special custom file filter class.
        /// </summary>
        /// <returns></returns>
        public static string GetFGDBPrompt(string title)
        {
            string fgdbPath = string.Empty;

            //Create new browser filter
            var bpf_gdb = new BrowseProjectFilter("fgdb");

            //Add typeID for file geodatabase
            bpf_gdb.AddCanBeTypeId("database_fgdb");

            //Display only folders and GDB in the browse dialog
            bpf_gdb.Includes.Add("FolderConnection");
            bpf_gdb.Includes.Add("FileGeodatabase");
            bpf_gdb.FileExtension = ".gdb";

            //Does not display Online places in the browse dialog
            bpf_gdb.Excludes.Add("esri_browsePlaces_Online");

            //Display the filter in an Open Item dialog
            OpenItemDialog openDialogItem = new OpenItemDialog
            {
                Title = title,
                MultiSelect = false,
                BrowseFilter = bpf_gdb,
                Filter = "File Geodatabase (*.gdb)|*.gdb|All Files (*.*)|*.*"
            };

            bool? ok = openDialogItem.ShowDialog();

            if (ok.HasValue && ok.Value && openDialogItem.Items.Count() > 0)
            {
                fgdbPath = openDialogItem.Items.First().Path;
            }

            return fgdbPath;
        }

        /// <summary>
        /// Will prompt the symbol style selection dialog from ESRI
        /// </summary>
        /// <returns></returns>
        public static void GetSymbolPrompt(StyleItemType styleItemType)
        {
            //Create a new dialog instance
            SymbolStyleDialog _symbolDialog = new SymbolStyleDialog(styleItemType);
            _symbolDialog.Closing += _symbolDialog_Closing;
            _symbolDialog.ShowDialog();
        }

        /// <summary>
        /// Upon closing send an event stating the selected symbol name
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void _symbolDialog_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SymbolStyleDialog senderDialog = sender as SymbolStyleDialog;
            if (senderDialog != null && senderDialog.SelectedSymbolItem != null)
            {
                //Send call to refresh other pages
                EventHandler<string> colorSymbolReferenceRequest = colorSymbolReferenceSelected;
                if (colorSymbolReferenceRequest != null)
                {
                    colorSymbolReferenceRequest(sender, senderDialog.SelectedSymbolItem.Name);
                }

            }
        }
    }
}
