using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;
using ESRI.ArcGIS.Geodatabase;

namespace GSC_ProjectEditor
{
    public class Button_CreateEdit_GeologicalEvents : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        public Button_CreateEdit_GeologicalEvents()
        {
            //Init culture
            Utilities.Culture.SetCulture();
        }

        protected override void OnClick()
        {
            if (Properties.Settings.Default.dwEnabling)
            {
                List<string> restrictedDataset = new List<string>();
                restrictedDataset.Add(Constants.Database.TGeoEvent);
                restrictedDataset.Add(Constants.Database.FLabel);
                restrictedDataset.Add(Constants.Database.TLegendGene);
                restrictedDataset.Add(Constants.Database.TSource);
                restrictedDataset.Add(Constants.Database.FGeoline);

                string currentWorkspacePath = Properties.Settings.Default.PROJECT_DATABASE_PATH;
                IWorkspace currentWorkspace = Workspace.AccessWorkspace(currentWorkspacePath);
                bool isProjectValid = Workspace.isWorkspaceAProperProjectDatabase(currentWorkspace, restrictedDataset);

                if (isProjectValid)
                {
                    //Show the first dialog
                    Form_CreateEdit_GeologicalEvents addGeoEventForm = new Form_CreateEdit_GeologicalEvents();
                    addGeoEventForm.Show();
                }

            }
            else
            {
                MessageBox.Show(Properties.Resources.Error_NotEnable, Properties.Resources.Error_NotEnableTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
