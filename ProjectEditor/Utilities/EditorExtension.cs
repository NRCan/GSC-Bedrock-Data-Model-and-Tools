using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Linq;
using System.Drawing.Text;
using ESRI.ArcGIS.Editor;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Desktop.AddIns;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.EditorExt;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.DataSourcesFile;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.Geoprocessing;
using ESRI.ArcGIS.SystemUI;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Carto;
using System.Drawing;


namespace GSC_ProjectEditor
{
    /// <summary>
    /// EditorExtension1 class implementing custom ESRI Editor Extension functionalities.
    /// </summary>
    public class EditorExtension : ESRI.ArcGIS.Desktop.AddIns.Extension
    {
        
        #region Main Variables

        //GEO_LINE feature
        private const string geolineFeature = GSC_ProjectEditor.Constants.Database.FGeoline;
        private const string geolineType = GSC_ProjectEditor.Constants.DatabaseFields.FGeolineSubtype;
        private const string geolineQualif = GSC_ProjectEditor.Constants.DatabaseFields.FGeolineQualif;
        private const string geolineConf = GSC_ProjectEditor.Constants.DatabaseFields.FGeolineConf;
        private const string geolineAttitude = GSC_ProjectEditor.Constants.DatabaseFields.FGeolineAtt;
        private const string geolineGeneration = GSC_ProjectEditor.Constants.DatabaseFields.FGeolineGeneration;
        private const string geolineID = GSC_ProjectEditor.Constants.DatabaseFields.FGeolineID;
        private const string geolineSymbol = GSC_ProjectEditor.Constants.DatabaseFields.FGeolineFGDC;

        //M_GEOLINE_SYMBOL table
        private const string mGeolineTable = GSC_ProjectEditor.Constants.Database.TGeolineSymbol;
        private const string mGeolineID = GSC_ProjectEditor.Constants.DatabaseFields.MGeolineID;
        private const string mGeolineFGDC = GSC_ProjectEditor.Constants.DatabaseFields.MGeolineFGDC;

        //Group layer
        //private const string interpretationGroupLayerName = GSC_ProjectEditor.Constants.Layers.GroupInterpretation;
        private const string geolineFeatureClass = GSC_ProjectEditor.Constants.Database.FGeoline;
        private const string geolabelFeatureClass = GSC_ProjectEditor.Constants.Database.FLabel;

        #endregion

        /// <summary>
        /// Init.
        /// Get all change events.
        /// </summary>
        public EditorExtension()
        {

            //Listen for a change event
            Events.OnChangeFeature += new IEditEvents_OnChangeFeatureEventHandler(Events_OnChangeFeature);

            //List for any new feature creation
            Events.OnCreateFeature += new IEditEvents_OnCreateFeatureEventHandler(Events_OnCreateFeature);

            //Manage any save edit operation with these two events
            Events.OnStartEditing += new IEditEvents_OnStartEditingEventHandler(Events_OnStartEditing);
            Events2.OnSaveEdits += new IEditEvents2_OnSaveEditsEventHandler(Events2_OnSaveEdits);

            //Manage stop editing
            Events.OnStopEditing += new IEditEvents_OnStopEditingEventHandler(Events_OnStopEditing);
            
        }

        public void Events_OnStopEditing(bool save)
        {
            Utilities.ToolBarControls.EnableDisableAllControls(false);
        }

        public void Events_OnStartEditing()
        {
            //Set some info
            IWorkspace currentEditingWorkspace = Utilities.EditSession.GetEditor().EditWorkspace;
            IDataset currentEditingWorspaceDataset = currentEditingWorkspace as IDataset;
            string outputProjectWorkspaceName = System.IO.Path.Combine(Constants.ESRI.defaultArcGISFolderName, Constants.Namespaces.mainNamespace + " " + ThisAddIn.Version.ToString());
            string outputProjectWorkspaceFolderPath = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), outputProjectWorkspaceName);

            //Validate workspace existance
            if (!System.IO.Directory.Exists(outputProjectWorkspaceFolderPath))
            {
                System.IO.Directory.CreateDirectory(outputProjectWorkspaceFolderPath);
            }

            //Empty dataset name list
            Properties.Settings.Default.DatasetNameList = new System.Collections.Specialized.StringCollection();

            //Save in settings
            GSC_ProjectEditor.Properties.Settings.Default.PROJECT_DATABASE_NAME = currentEditingWorspaceDataset.Name;
            GSC_ProjectEditor.Properties.Settings.Default.PROJECT_DATABASE_PATH = currentEditingWorkspace.PathName;
            GSC_ProjectEditor.Properties.Settings.Default.PROJECT_WORKSPACE_PATH = outputProjectWorkspaceFolderPath;
            GSC_ProjectEditor.Properties.Settings.Default.Save();

            //Enable tools
            Utilities.ToolBarControls.EnableDisableAllControls(true);
            GSC_ProjectEditor.Properties.Settings.Default.dwEnabling = true;

        }

        public void Events2_OnSaveEdits()
        {

        }

        public void Events_OnCreateFeature(IObject obj)
        {

        }


        /// <summary>
        /// Get all change events.
        /// </summary>
        /// <param name="obj">The object that was changed by the user.</param>
        public void Events_OnChangeFeature(ESRI.ArcGIS.Geodatabase.IObject obj)
        {
            #region Project Database Editor manipulations
            //If user is within GSC Editor
            if (GSC_ProjectEditor.Properties.Settings.Default.dwEnabling == true)
            {

                try
                {
                    //GEO_LINE and GEO_POINT features
                    string geolineFeature = GSC_ProjectEditor.Constants.Database.FGeoline;
                    string geopointFeature = GSC_ProjectEditor.Constants.Database.FGeopoint;

                    //Cast object to retrieve other information about feature change

                    IFeature objectFeature = obj as IFeature;
                    if (objectFeature!=null)
                    {
                        Table objectTable = objectFeature.Table as Table;
                        IDataset objectDataset = objectTable as IDataset;

                        //If user has updated geoline
                        if (objectDataset.BrowseName == geolineFeature)
                        {
                            //Call associated method
                            GSC_ProjectEditor.IDs.CalculateGeolineIDFromRow(obj);
                        }
                        else if (objectDataset.BrowseName == geopointFeature)
                        {
                            //Call associated method
                            GSC_ProjectEditor.IDs.CalculateGeopointIDFromRow(obj);
                        }
                    }



                }
                catch(Exception changeEventException)
                {
                    MessageBox.Show(changeEventException.Message);
                }

            }
            #endregion
        }

        /// <summary>
        /// When edit session is started.
        /// </summary>
        protected override void OnStartup()
        {
            IEditor theEditor = ArcMap.Editor;
        }

        /// <summary>
        /// When edit session has ended.
        /// </summary>
        protected override void OnShutdown()
        {
        }

        #region Editor Events

        #region Shortcut properties to the various editor event interfaces
        public IEditEvents_Event Events
        {
            
            get { return ArcMap.Editor as IEditEvents_Event; }
        }
        public IEditEvents2_Event Events2
        {
            get { return ArcMap.Editor as IEditEvents2_Event; }
        }
        public IEditEvents3_Event Events3
        {
            get { return ArcMap.Editor as IEditEvents3_Event; }
        }
        public IEditEvents4_Event Events4
        {
            get { return ArcMap.Editor as IEditEvents4_Event; }
        }

        #endregion

        public void WireEditorEvents()
        {
            Events.OnCurrentTaskChanged += delegate
            {
                if (ArcMap.Editor.CurrentTask != null)
                    System.Diagnostics.Debug.WriteLine(ArcMap.Editor.CurrentTask.Name);
            };
        }

        #endregion

    }

}
