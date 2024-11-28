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
    public class Button_Load_StudyAreas : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        public Button_Load_StudyAreas()
        {
            //Init culture
            Utilities.Culture.SetCulture();
        }

        protected override void OnClick()
        {
            

            if (Properties.Settings.Default.dwEnabling)
            {

                List<string> restrictedDataset = new List<string>();
                restrictedDataset.Add(Constants.Database.TStudyAreaIndex);
                restrictedDataset.Add(Constants.Database.FStudyArea);
                restrictedDataset.Add(Constants.Database.TSActivity);
                restrictedDataset.Add(Constants.Database.TMActivity);
                restrictedDataset.Add(Constants.Database.TProject);
                restrictedDataset.Add(Constants.Database.FCGMIndex);
                restrictedDataset.Add(Constants.Database.TLegendTree);
                restrictedDataset.Add(Constants.Database.TLegendGene);
                restrictedDataset.Add(Constants.Database.TSource);

                string currentWorkspacePath = Properties.Settings.Default.PROJECT_DATABASE_PATH;
                IWorkspace currentWorkspace = Workspace.AccessWorkspace(currentWorkspacePath);
                bool isProjectValid = Workspace.isWorkspaceAProperProjectDatabase(currentWorkspace, restrictedDataset);

                if (isProjectValid)
                {
                    //Show the first dialog
                    Form_Load_StudyAreas addStudyAreaForm = new Form_Load_StudyAreas();
                    addStudyAreaForm.Show();
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
