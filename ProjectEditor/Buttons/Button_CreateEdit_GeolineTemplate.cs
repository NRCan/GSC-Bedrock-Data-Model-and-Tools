using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.Editor;
using System.Windows.Forms;
using ESRI.ArcGIS.Geodatabase;

namespace GSC_ProjectEditor
{
    /// <summary>
    /// When button is clicked, dw dockablewindowSelectGeoline is made visible
    /// </summary>
    public class Button_CreateEdit_GeolineTemplate : ESRI.ArcGIS.Desktop.AddIns.Button
    {

        #region Main Variables
        #endregion

        /// <summary>
        /// Init.
        /// </summary>
        public Button_CreateEdit_GeolineTemplate()
        {

        }

        /// <summary>
        /// When button is clicked
        /// </summary>
        protected override void OnClick()
        {

            //Show selectGeoline dockable window.
            ArcMap.Application.CurrentTool = null;

            List<string> restrictedDataset = new List<string>();
            restrictedDataset.Add(Constants.Database.FGeoline);
            restrictedDataset.Add(Constants.Database.TGeolineSymbol);
            restrictedDataset.Add(Constants.Database.TLegendGene);

            string currentWorkspacePath = Properties.Settings.Default.PROJECT_DATABASE_PATH;
            IWorkspace currentWorkspace = Workspace.AccessWorkspace(currentWorkspacePath);
            bool isProjectValid = Workspace.isWorkspaceAProperProjectDatabase(currentWorkspace, restrictedDataset);

            if (isProjectValid)
            {
                UID theUid = new UIDClass();
                theUid.Value = ThisAddIn.IDs.Dockablewindow_CreateEdit_GeolineTemplate;

                IDockableWindow selectLineForm = ArcMap.DockableWindowManager.GetDockableWindow(theUid);
                if (selectLineForm.IsVisible() == false) { selectLineForm.Show(true); }
            }



        }

        /// <summary>
        /// When button is updated. 
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
