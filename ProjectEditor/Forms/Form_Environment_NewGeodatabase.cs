using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Geodatabase;

namespace GSC_ProjectEditor
{
    public partial class Form_Environment_NewGeodatabase : Form
    {
        #region Main Variables
        //Get feature datasets names
        private const string FDField = GSC_ProjectEditor.Constants.Database.FDField;
        private const string FDGeo = GSC_ProjectEditor.Constants.Database.FDGeo;

        //Get other feature names
        private const string FStudyArea = GSC_ProjectEditor.Constants.Database.FStudyArea;
        private const string FCGM = GSC_ProjectEditor.Constants.Database.FCGMIndex;

        private const string geoline = GSC_ProjectEditor.Constants.Database.FGeoline;
        private const string geopoint = GSC_ProjectEditor.Constants.Database.FGeopoint;
        private const string geopolys = GSC_ProjectEditor.Constants.Database.FGeopoly;
        private const string geolabel = GSC_ProjectEditor.Constants.Database.FLabel;
        private const string cartoPoint = GSC_ProjectEditor.Constants.Database.FCartoPoint;

        //Environment table
        private const string envDBPath = GSC_ProjectEditor.Constants.Environment.envCurrentDBPath;
        private const string envDBName = GSC_ProjectEditor.Constants.Environment.envCurrentDBName;
        private const string envFolderPath = GSC_ProjectEditor.Constants.Environment.envFolderPath;

        //Get some field names
        private const string editDate = GSC_ProjectEditor.Constants.DatabaseFields.ETEditDate;
        private const string createDate = GSC_ProjectEditor.Constants.DatabaseFields.ETCreateDate;

        //Other
        Dictionary<string, List<string>> datasetDico = new Dictionary<string, List<string>>(); //A list of all feature data sets

        //Delegates and events
        public delegate void thisIsTheEndEventHandler(); //A delegate for execution events
        public event thisIsTheEndEventHandler processFinished; //This event is triggered when a new main activity has been added within database

        #endregion

        public Form_Environment_NewGeodatabase()
        {
            //Set culture
            Utilities.Culture.SetCulture();

            InitializeComponent();
        }

        /// <summary>
        /// Create the database
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void btn_CreateGDB_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;

            //Make sure all parameters have been entered
            if(this.txtbox_DBName.Text != "" && this.txtbox_Prj.Text != "" && this.txtbox_XML.Text != "")
            {
                CreateGDB(this.txtbox_DBName.Text, this.txtbox_Prj.Tag as ISpatialReference, this.txtbox_XML.Text);
                GSC_ProjectEditor.Messages.ShowEndOfProcess();

            }
            else
            {
                MessageBox.Show(Properties.Resources.EmptyFields);
            }

            this.Cursor = Cursors.Default;

        }

        private void btn_CancelCreateGDB_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// This button will open up a dialog for user to select an XML file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_SelectXML_Click(object sender, EventArgs e)
        {
            //Get custom xml file prompt (only available within this application)
            string getUserXMLPath = Dialog.GetXMLFilePrompt(this.Handle.ToInt32());
            
            //Fill textbox
            this.txtbox_XML.Text = getUserXMLPath;

        }

        /// <summary>
        /// This button will open up a dialog for user to select a projection from Arc.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_SelectPrj_Click(object sender, EventArgs e)
        {
            //Get spatial ref prompt
            ISpatialReference getUserSR = GSC_ProjectEditor.Dialog.GetProjectionPrompt(this.Handle.ToInt32());

            //Fill textbox
            if (getUserSR != null)
            {
                this.txtbox_Prj.Text = getUserSR.Name;
                this.txtbox_Prj.Tag = getUserSR; //Add the the spatial reference objec to the tag
            }

        }

        /// <summary>
        /// Will enable the editor tracking on the project geodatabase.
        /// </summary>
        /// <param name="newWorkspace">the new workspace in which the editor tracking will be enabled</param>
        public static void EnableEditorTracking(IWorkspace newWorkspace)
        {
            //Get a list of all features to enable the editor on
            List<string> etFeatureList = new List<string> { geoline, geopoint, geopolys, geolabel, cartoPoint };

            //Enable the tracking
            GSC_ProjectEditor.Workspace.EnabledEditorTrackingFromWorkspace(newWorkspace, etFeatureList, null, createDate, null, editDate);

        }

        /// <summary>
        /// Will create the new geodatabase
        /// </summary>
        /// <param name="inDBName"></param>
        /// <param name="inProjection"></param>
        /// <param name="inXML"></param>
        public void CreateGDB(string inDBName, ISpatialReference inProjection, string inXML)
        {
            //Make sure the new database doesn't exist already
            string projectFolder = System.IO.Path.GetDirectoryName(inDBName);

            bool dbExists = System.IO.Directory.Exists(inDBName);

            if (!dbExists)
            {
                //Create the database from scratch and get the workspace object
                string nameOnly = inDBName.Replace(projectFolder, "").Replace(".gdb", "");
                IWorkspace newDBWorkspace = GSC_ProjectEditor.Workspace.CreateWorkspace(projectFolder, nameOnly);

                //Import the xml file into the new database
                newDBWorkspace = GSC_ProjectEditor.Workspace.ImportXMLWorkspace(newDBWorkspace, inXML);

                //For all feature classes and datasets define the projection with users request
                datasetDico["FD"] = new List<string> { FDField, FDGeo };
                datasetDico["FC"] = new List<string> { FStudyArea, FCGM, cartoPoint };

                foreach (KeyValuePair<string, List<string>> datasets in datasetDico)
                {
                    if (datasets.Key == "FD")
                    {
                        foreach (string items in datasetDico["FD"])
                        {
                            //Cast feature classes
                            IFeatureDataset getFD = GSC_ProjectEditor.FeatureDataset.OpenFeatureDataSet(newDBWorkspace, items);

                            //Cast features into geodatasetschema to edit projection
                            IGeoDatasetSchemaEdit getDS = getFD as IGeoDatasetSchemaEdit;

                            //Alter the feature with new projection (use this method if no projection is set)
                            getDS.AlterSpatialReference(inProjection);

                            //Release workspace
                            System.Runtime.InteropServices.Marshal.ReleaseComObject(getFD);

                        }

                    }
                    else if (datasets.Key == "FC")
                    {
                        foreach (string items in datasetDico["FC"])
                        {
                            //Cast feature classes
                            IFeatureClass getFC = GSC_ProjectEditor.FeatureClass.OpenFeatureClassFromWorkspace(newDBWorkspace, items);

                            //Cast features into geodatasetschema to edit projection
                            IGeoDatasetSchemaEdit getDS = getFC as IGeoDatasetSchemaEdit;

                            //Alter the feature with new projection (use this method if no projection is set)
                            getDS.AlterSpatialReference(inProjection);

                            //Release workspace
                            System.Runtime.InteropServices.Marshal.ReleaseComObject(getFC);

                        }

                    }
                }

                //Set the editor tracking
                EnableEditorTracking(newDBWorkspace);

                //Release workspace
                System.Runtime.InteropServices.Marshal.ReleaseComObject(newDBWorkspace);

            }
            else
            {
                MessageBox.Show("Database already exists.");
            }


            GC.Collect();

            //Close window
            this.Close();

            //Start a processFinished event
            if (processFinished != null)
            {
                processFinished();
            }
        }

        /// <summary>
        /// Will prompt for a save path for the new file geodatabase.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_select_DBPath_Click(object sender, EventArgs e)
        {
            //Get custom xml file prompt (only available within this application)
            string getUserFGDBPath = Dialog.GetFGDBSavePrompt(this.Handle.ToInt32(), "Select output path for new File Geodatabase");

            //Fill textbox
            this.txtbox_DBName.Text = getUserFGDBPath;
        }
    }
}
