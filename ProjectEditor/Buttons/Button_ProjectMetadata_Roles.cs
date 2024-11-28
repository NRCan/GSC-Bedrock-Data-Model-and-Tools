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
    public class Button_ProjectMetadata_Roles : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        public Button_ProjectMetadata_Roles()
        {
            //Init culture
            Utilities.Culture.SetCulture();
        }

        protected override void OnClick()
        {
            

            if (Properties.Settings.Default.dwEnabling)
            {
                List<string> restrictedDataset = new List<string>();
                restrictedDataset.Add(Constants.Database.TOrganisation);
                restrictedDataset.Add(Constants.Database.TParticipant);
                restrictedDataset.Add(Constants.Database.TPerson);
                restrictedDataset.Add(Constants.Database.TMActivity);
                restrictedDataset.Add(Constants.Database.TProject);
                restrictedDataset.Add(Constants.Database.TSActivity);

                string currentWorkspacePath = Properties.Settings.Default.PROJECT_DATABASE_PATH;
                IWorkspace currentWorkspace = Workspace.AccessWorkspace(currentWorkspacePath);
                bool isProjectValid = Workspace.isWorkspaceAProperProjectDatabase(currentWorkspace, restrictedDataset);

                if (isProjectValid)
                {
                    //Show the first dialog
                    Form_ProjectMetadata_Roles addParticipantRole = new Form_ProjectMetadata_Roles();
                    addParticipantRole.Show();
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
