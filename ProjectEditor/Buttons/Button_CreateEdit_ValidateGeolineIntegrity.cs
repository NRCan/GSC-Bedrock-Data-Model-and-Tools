using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Editor;

namespace GSC_ProjectEditor
{
    public class Button_CreateEdit_ValidateGeolineIntegrity : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        //TODO Find a way to change cursor for a waiting one, because this is not a normal VS control...

        #region Main Variables

        //Feature geoline
        private const string fcGeoline = GSC_ProjectEditor.Constants.Database.FGeoline;

        //Database
        private const string FDName = GSC_ProjectEditor.Constants.Database.FDGeo;

        #endregion


        public Button_CreateEdit_ValidateGeolineIntegrity()
        {
        }

        protected override void OnClick()
        {
            List<string> restrictedDataset = new List<string>();
            restrictedDataset.Add(Constants.Database.FGeoline);

            string currentWorkspacePath = Properties.Settings.Default.PROJECT_DATABASE_PATH;
            IWorkspace currentWorkspace = Workspace.AccessWorkspace(currentWorkspacePath);
            bool isProjectValid = Workspace.isWorkspaceAProperProjectDatabase(currentWorkspace, restrictedDataset);

            if (isProjectValid)
            {
                try
                {
                    this.Enabled = false;
                    Application.UseWaitCursor = true;
                    Application.DoEvents();
                    System.Windows.Forms.Cursor.Current = Cursors.WaitCursor;

                    //Access geoline feature
                    IFeatureClass geolineFC = GSC_ProjectEditor.FeatureClass.OpenFeatureClass(fcGeoline);

                    Application.UseWaitCursor = true;
                    System.Windows.Forms.Cursor.Current = Cursors.WaitCursor;

                    //Check if geometry is alright with check geometry and repair
                    string repairOutput = GSC_ProjectEditor.Geometry.DeleteNullGeometries(geolineFC);

                    Application.UseWaitCursor = true;
                    System.Windows.Forms.Cursor.Current = Cursors.WaitCursor;

                    System.Runtime.InteropServices.Marshal.ReleaseComObject(geolineFC);

                    Application.UseWaitCursor = true;
                    System.Windows.Forms.Cursor.Current = Cursors.WaitCursor;

                    //Remove output layer
                    GSC_ProjectEditor.FeatureLayers.RemoveLayerFromArcMap(repairOutput, Utilities.MapDocumentFeatureLayers.GetCurrentMapObject());

                    Application.UseWaitCursor = true;
                    System.Windows.Forms.Cursor.Current = Cursors.WaitCursor;

                    IEditor editSession = Utilities.EditSession.GetEditor();

                    //Multipart to single part 
                    GSC_ProjectEditor.Geometry.PolylineMultiPartToSinglePart(geolineFC, editSession);
                    Application.UseWaitCursor = true;
                    System.Windows.Forms.Cursor.Current = Cursors.WaitCursor;

                    //Exploding intersecting lines

                    //Dissolve

                    //Remove empty geometry and null length
                    //GSC_ProjectEditor.Geometry.PolylineRemoveNullLenght_EmptyFeature(geolineFC, editSession);

                    //Check for bezier curves
                    GSC_ProjectEditor.Geometry.PolylineDensify(geolineFC, editSession);
                    Application.UseWaitCursor = true;
                    System.Windows.Forms.Cursor.Current = Cursors.WaitCursor;
                    GSC_ProjectEditor.Messages.ShowEndOfProcess();


                }
                finally
                {
                    this.Enabled = true;
                    Application.UseWaitCursor = false;
                    System.Windows.Forms.Cursor.Current = Cursors.Default;
                }
            }




        }


        /// <summary>
        /// On update
        /// </summary>
        protected override void OnUpdate()
        {
            //Enabled = ArcMap.Application != null;
        }

        /// <summary>
        /// Will enable or disable current control class.
        /// </summary>
        /// <param name="enable"></param>
        public void EnableDisable(bool enable)
        {
            if (enable)
            {
                this.Enabled = true;
            }
            else
            {
                this.Enabled = false;
            }
        }
    }
}
