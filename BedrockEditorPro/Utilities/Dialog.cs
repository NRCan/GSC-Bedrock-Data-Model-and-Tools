using ArcGIS.Desktop.Catalog;
using ArcGIS.Desktop.Core;
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

    }
}
