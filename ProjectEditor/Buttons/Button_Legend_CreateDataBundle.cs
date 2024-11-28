using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;
using ESRI.ArcGIS.Geodatabase;

namespace GSC_ProjectEditor
{
    public class Button_Legend_CreateDataBundle : ESRI.ArcGIS.Desktop.AddIns.Button
    {



        public Button_Legend_CreateDataBundle()
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
            restrictedDataset.Add(Constants.Database.TLegendDescription);
            restrictedDataset.Add(Constants.Database.FGeopoly);
            restrictedDataset.Add(Constants.Database.FCGMIndex);


            string currentWorkspacePath = Properties.Settings.Default.PROJECT_DATABASE_PATH;
            IWorkspace currentWorkspace = Workspace.AccessWorkspace(currentWorkspacePath);
            bool isProjectValid = Workspace.isWorkspaceAProperProjectDatabase(currentWorkspace, restrictedDataset);

            if (isProjectValid)
            {
                try
                {
                    #region Get a list of maps to publish through a form and create if clicked.

                    //Pop the appropriate form for the options
                    Form_Legend_CreateDataBundle getNewForm = new Form_Legend_CreateDataBundle();
                    getNewForm.Show();

                    #endregion
                }
                catch (Exception pubException)
                {
                    MessageBox.Show(pubException.StackTrace);
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
