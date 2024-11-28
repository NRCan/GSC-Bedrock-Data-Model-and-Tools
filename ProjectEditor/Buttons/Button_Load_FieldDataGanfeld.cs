using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;
using ESRI.ArcGIS.Geodatabase;

namespace GSC_ProjectEditor
{
    public class Button_Load_FieldDataGanfeld : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        public Button_Load_FieldDataGanfeld()
        {
            //Init culture
            Utilities.Culture.SetCulture();
        }

        protected override void OnClick()
        {

            if (Properties.Settings.Default.dwEnabling)
            {
                List<string> restrictedDataset = new List<string>();
                restrictedDataset.Add(Constants.Database.gFCLinework);
                restrictedDataset.Add(Constants.Database.gFCStation);
                restrictedDataset.Add(Constants.Database.gFCTraverses);
                restrictedDataset.Add(Constants.Database.gEarthMath);
                restrictedDataset.Add(Constants.Database.gMA);
                restrictedDataset.Add(Constants.Database.gMetadata);
                restrictedDataset.Add(Constants.Database.gMineral);
                restrictedDataset.Add(Constants.Database.gSample);
                restrictedDataset.Add(Constants.Database.gPhoto);
                restrictedDataset.Add(Constants.Database.gStruc);

                string currentWorkspacePath = Properties.Settings.Default.PROJECT_DATABASE_PATH;
                IWorkspace currentWorkspace = Workspace.AccessWorkspace(currentWorkspacePath);
                bool isProjectValid = Workspace.isWorkspaceAProperProjectDatabase(currentWorkspace, restrictedDataset);

                if (isProjectValid)
                {
                    Form_Load_FieldDataGanfeld datasourceForm = new Form_Load_FieldDataGanfeld();
                    datasourceForm.Show();
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
