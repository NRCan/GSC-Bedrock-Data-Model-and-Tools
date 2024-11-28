using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.Geodatabase;

namespace GSC_ProjectEditor
{
    public class Button_ProjectMetadata_Definition : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        public Button_ProjectMetadata_Definition()
        {
            //Init culture
            Utilities.Culture.SetCulture();
        }

        protected override void OnClick()
        {
            

            if (Properties.Settings.Default.dwEnabling)
            {
                List<string> restrictedDataset = new List<string>();
                restrictedDataset.Add(Constants.Database.TProject);

                string currentWorkspacePath = Properties.Settings.Default.PROJECT_DATABASE_PATH;
                IWorkspace currentWorkspace = Workspace.AccessWorkspace(currentWorkspacePath);
                bool isProjectValid = Workspace.isWorkspaceAProperProjectDatabase(currentWorkspace, restrictedDataset);

                if (isProjectValid)
                {
                    //Show the first dialog
                    Form_ProjectMetadata_Definition addProjectForm = new Form_ProjectMetadata_Definition();
                    addProjectForm.Show();
                }

            }
        }

        protected override void OnUpdate()
        {

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
