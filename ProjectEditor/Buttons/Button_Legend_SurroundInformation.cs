using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.Editor;
using ESRI.ArcGIS.Geodatabase;

namespace GSC_ProjectEditor
{
    public class Button_Legend_SurroundInformation : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        public Button_Legend_SurroundInformation()
        {
        }

        protected override void OnClick()
        {
            List<string> restrictedDataset = new List<string>();
            restrictedDataset.Add(Constants.Database.TLegendTree);
            restrictedDataset.Add(Constants.Database.FCGMIndex);

            string currentWorkspacePath = Properties.Settings.Default.PROJECT_DATABASE_PATH;
            IWorkspace currentWorkspace = Workspace.AccessWorkspace(currentWorkspacePath);
            bool isProjectValid = Workspace.isWorkspaceAProperProjectDatabase(currentWorkspace, restrictedDataset);

            if (isProjectValid)
            {
                try
                {
                    ArcMap.Application.CurrentTool = null;

                    UID dwCGMUID = new UIDClass();
                    dwCGMUID.Value = ThisAddIn.IDs.DockableWindow_Legend_SurroundInformation;

                    IDockableWindow cgmForm = ArcMap.DockableWindowManager.GetDockableWindow(dwCGMUID);

                    if (cgmForm.IsVisible() == false) { cgmForm.Show(true); }
                }

                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
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
