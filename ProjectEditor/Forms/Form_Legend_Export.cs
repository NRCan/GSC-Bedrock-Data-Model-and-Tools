using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;

namespace GSC_ProjectEditor
{
    public partial class Form_Legend_Export : Form
    {

        public Form_Legend_Export()
        {
            //Init culture
            Utilities.Culture.SetCulture();

            InitializeComponent();

            fillMapList();

        }

        /// <summary>
        /// Will fill the map list with current CGM maps from the feature in the database
        /// </summary>
        public void fillMapList()
        {

            //Reset
            this.checkedListBox_Maps.Items.Clear();

            //Get a list of available CGM maps and descriptions ids from tree table
            List<string> currentMaps = GSC_ProjectEditor.Tables.GetUniqueFieldValues(Constants.Database.TLegendTree, Constants.DatabaseFields.LegendTreeCGM, null, false, null)[Constants.ValueKeywords.GetUniqueFieldValuesMain];
            currentMaps.Sort();

            //Build the list
            foreach (string maps in currentMaps)
            {
                //Add map ids and description id to list that will go in checkbox list
                this.checkedListBox_Maps.Items.Add(maps);
            }

            //When done add project legend table view name
            this.checkedListBox_Maps.Items.Add(Constants.ValueKeywords.FullProjectLegendSuffix);

        }

        /// <summary>
        /// Will close the current form when clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            //Close current form
            this.Close();
        }

        private void btn_ExportLegend_Click(object sender, EventArgs e)
        {
            //Variable
            bool foundTableBreaker = false;

            //Retrieve some options
            List<string> listedMaps = new List<string>();
            CheckedListBox.CheckedItemCollection checkedList = this.checkedListBox_Maps.CheckedItems;
            foreach (string items in checkedList)
            {
                listedMaps.Add(items.ToString());
            }

            //Get workspace
            string workPath = GSC_ProjectEditor.Properties.Settings.Default.PROJECT_DATABASE_PATH;
            IWorkspace projectWorkspace = GSC_ProjectEditor.Workspace.AccessWorkspace(workPath);

            //Access project folder
            string projectFolderPath = GSC_ProjectEditor.Properties.Settings.Default.PROJECT_WORKSPACE_PATH;

            //Get all table names
            List<string> tableNames = Tables.GetTableNameListFromWorkspace(projectWorkspace);

            //Treat all maps versus refines maps differently
            foreach (string map in listedMaps)
            {
                //Variables
                foundTableBreaker = true;

                List<string> currentMapTables = tableNames.Where(x => x.Contains(map)).ToList();
                if (currentMapTables.Count > 0)
                {
                    //Take the last one in case there is multiples of them
                    currentMapTables.Sort();
                    currentMapTables.Reverse();
                    ITable mapTable = Tables.OpenTable(currentMapTables[0]);
                    string mapName = currentMapTables[0];

                    //Detect existance of table
                    string outputFullPathName = System.IO.Path.Combine(projectFolderPath, mapName + ".csv");
                    bool doesFileExists = System.IO.File.Exists(outputFullPathName);
                    int iterator = 1;
                    while (doesFileExists)
                    {
                        string outputFullPathNameSuffix = IDs.CalculateSimplementAlphabeticID(true, iterator);
                        mapName = currentMapTables[0] + "_" + outputFullPathNameSuffix;
                        outputFullPathName = System.IO.Path.Combine(projectFolderPath, mapName + ".csv");
                        doesFileExists = System.IO.File.Exists(outputFullPathName);
                        iterator = iterator + 1;
                    }

                    GeoProcessing.ConvertTableToTable(mapTable, ".csv", projectFolderPath, mapName);
                }
                else
                {
                    foundTableBreaker = false;
                }

            }

            if (foundTableBreaker)
            {
                Messages.ShowEndOfProcess(Properties.Resources.Form_Legend_CreateDataBundleCompleted + " " + projectFolderPath);

                //Close current form
                this.Close();
            }
            else
            {
                Messages.ShowGenericWarning(Properties.Resources.Warning_LegendExport_MissingTable);
            }
            


        }

        /// <summary>
        /// Will close the window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Cancel_Click_1(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
