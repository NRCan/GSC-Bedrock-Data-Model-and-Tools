using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.ArcMapUI;


namespace GSC_ProjectEditor
{
    public class Button_Legend_CreateTempTableLegendGenerator : ESRI.ArcGIS.Desktop.AddIns.Button
    {


        public Button_Legend_CreateTempTableLegendGenerator()
        {
            //Set culture before init.
            Utilities.Culture.SetCulture();
        }

        protected override void OnClick()
        {
            List<string> restrictedDataset = new List<string>();
            restrictedDataset.Add(Constants.Database.TLegendTree);
            restrictedDataset.Add(Constants.Database.TLegendGene);
            restrictedDataset.Add(Constants.Database.FGeopoint);
            restrictedDataset.Add(Constants.Database.FGeoline);
            restrictedDataset.Add(Constants.Database.FGeopoly);
            restrictedDataset.Add(Constants.Database.FCGMIndex);
            restrictedDataset.Add(Constants.Database.TLegendDescription);

            string currentWorkspacePath = Properties.Settings.Default.PROJECT_DATABASE_PATH;
            IWorkspace currentWorkspace = Workspace.AccessWorkspace(currentWorkspacePath);
            bool isProjectValid = Workspace.isWorkspaceAProperProjectDatabase(currentWorkspace, restrictedDataset);

            if (isProjectValid)
            {
                //Pop the appropriate form for the options
                Form_Legend_CreateTempTableLegendGenerator getNewForm = new Form_Legend_CreateTempTableLegendGenerator();
                getNewForm.Show();
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
