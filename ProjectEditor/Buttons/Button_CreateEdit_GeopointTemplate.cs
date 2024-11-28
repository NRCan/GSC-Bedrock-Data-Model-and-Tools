using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.Editor;
using ESRI.ArcGIS.Geodatabase;

namespace GSC_ProjectEditor
{
    public class Button_CreateEdit_GeopointTemplate : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        public Button_CreateEdit_GeopointTemplate()
        {
        }

        protected override void OnClick()
        {
            //Show selectGeoline dockable window.
            ArcMap.Application.CurrentTool = null;

            List<string> restrictedDataset = new List<string>();
            restrictedDataset.Add(Constants.Database.FGeopoint);
            restrictedDataset.Add(Constants.Database.TGeopointSymbol);
            restrictedDataset.Add(Constants.Database.TLegendGene);

            string currentWorkspacePath = Properties.Settings.Default.PROJECT_DATABASE_PATH;
            IWorkspace currentWorkspace = Workspace.AccessWorkspace(currentWorkspacePath);
            bool isProjectValid = Workspace.isWorkspaceAProperProjectDatabase(currentWorkspace, restrictedDataset);

            if (isProjectValid)
            {
                UID theUid = new UIDClass();
                theUid.Value = ThisAddIn.IDs.DockableWindow_CreateEdit_GeopointTemplate;

                IDockableWindow addGeoPointForm = ArcMap.DockableWindowManager.GetDockableWindow(theUid);
                if (addGeoPointForm.IsVisible() == false) { addGeoPointForm.Show(true); }
            }

        }

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
