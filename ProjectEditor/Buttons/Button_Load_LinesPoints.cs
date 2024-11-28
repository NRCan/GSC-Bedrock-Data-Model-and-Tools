using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using ESRI.ArcGIS.Geodatabase;

namespace GSC_ProjectEditor
{
    public class Button_Load_LinesPoints : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        public Button_Load_LinesPoints()
        {
            //Init culture
            Utilities.Culture.SetCulture();
        }

        protected override void OnClick()
        {
            

            if (Properties.Settings.Default.dwEnabling)
            {

                List<string> restrictedDataset = new List<string>();
                restrictedDataset.Add(Constants.Database.FGeoline);
                restrictedDataset.Add(Constants.Database.FGeopoint);
                restrictedDataset.Add(Constants.Database.gStruc);
                restrictedDataset.Add(Constants.Database.FStation);
                restrictedDataset.Add(Constants.Database.TLegendGene);
                restrictedDataset.Add(Constants.Database.TGeolineSymbol);
                restrictedDataset.Add(Constants.Database.TGeopointSymbol);

                string currentWorkspacePath = Properties.Settings.Default.PROJECT_DATABASE_PATH;
                IWorkspace currentWorkspace = Workspace.AccessWorkspace(currentWorkspacePath);
                bool isProjectValid = Workspace.isWorkspaceAProperProjectDatabase(currentWorkspace, restrictedDataset);

                if (isProjectValid)
                {
                    //Open the new form
                    Form_Load_LinesPoints openForm = new Form_Load_LinesPoints();
                    openForm.Show();
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
