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
        /// <summary>
    /// When button is clicked, dw dockablewindowAddMapUnit is made visible
    /// </summary>
    public class Button_CreateEdit_LabelTemplate : ESRI.ArcGIS.Desktop.AddIns.Button
    {

        /// <summary>
        /// Init.
        /// </summary>
        public Button_CreateEdit_LabelTemplate()
        {
            
        }
        /// <summary>
        /// When button is clicked
        /// </summary>
        protected override void OnClick()
        {
            
            try
            {
                List<string> restrictedDataset = new List<string>();
                restrictedDataset.Add(Constants.Database.FLabel);
                restrictedDataset.Add(Constants.Database.TLegendGene);

                string currentWorkspacePath = Properties.Settings.Default.PROJECT_DATABASE_PATH;
                IWorkspace currentWorkspace = Workspace.AccessWorkspace(currentWorkspacePath);
                bool isProjectValid = Workspace.isWorkspaceAProperProjectDatabase(currentWorkspace, restrictedDataset);

                if (isProjectValid)
                {
                    ArcMap.Application.CurrentTool = null;

                    UID dwLabelUID = new UIDClass();
                    dwLabelUID.Value = ThisAddIn.IDs.Dockablewindow_CreateEdit_LabelTemplate;

                    IDockableWindow labelForm = ArcMap.DockableWindowManager.GetDockableWindow(dwLabelUID);

                    if (labelForm.IsVisible() == false) { labelForm.Show(true); }
                }


            }

            catch (Exception e)
            {
                MessageBox.Show(e.Message);
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
