using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geoprocessing;
using ESRI.ArcGIS.Geometry;

namespace GSC_ProjectEditor
{
    public class Button_Load_TranslateGanfeldPointStructure : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        #region MAIN VARIABLES


        #endregion

        public Button_Load_TranslateGanfeldPointStructure()
        {

        }

        /// <summary>
        /// Will process struc data append within geo_points, wheter the incoming data is from a Ganfeld table, Ganfeld shapefile or GSC Field app table.
        /// </summary>
        /// <param name="isTable">True if data is stored inside F_STRUC table inside project, false if coming from external shapefile</param>
        protected override void OnClick()
        {
            

            if (Properties.Settings.Default.dwEnabling)
            {
                List<string> restrictedDataset = new List<string>();
                restrictedDataset.Add(Constants.Database.TLegendGene);
                restrictedDataset.Add(Constants.Database.TGeolineSymbol);
                restrictedDataset.Add(Constants.Database.FStation);
                restrictedDataset.Add(Constants.Database.gStruc);
                restrictedDataset.Add(Constants.Database.FGeopoint);

                string currentWorkspacePath = Properties.Settings.Default.PROJECT_DATABASE_PATH;
                IWorkspace currentWorkspace = Workspace.AccessWorkspace(currentWorkspacePath);
                bool isProjectValid = Workspace.isWorkspaceAProperProjectDatabase(currentWorkspace, restrictedDataset);

                if (isProjectValid)
                {
                    //Show proper form
                    Form_Load_TranslateGanfeldPointStructure convertStructure = new Form_Load_TranslateGanfeldPointStructure();
                    convertStructure.Show();
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
