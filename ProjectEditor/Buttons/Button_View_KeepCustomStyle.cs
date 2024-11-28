using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using ESRI.ArcGIS.Desktop.AddIns;
using ESRI.ArcGIS.Geodatabase;

namespace GSC_ProjectEditor
{
    public class Button_View_KeepCustomStyle : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        public Button_View_KeepCustomStyle()
        {


        }

        protected override void OnClick()
        {
            List<string> restrictedDataset = new List<string>();
            restrictedDataset.Add(Constants.Database.FGeopoly);
            restrictedDataset.Add(Constants.Database.FGeopoint);
            restrictedDataset.Add(Constants.Database.FLabel);
            restrictedDataset.Add(Constants.Database.FGeoline);
            restrictedDataset.Add(Constants.Database.TLegendGene);

            string currentWorkspacePath = Properties.Settings.Default.PROJECT_DATABASE_PATH;
            IWorkspace currentWorkspace = Workspace.AccessWorkspace(currentWorkspacePath);
            bool isProjectValid = Workspace.isWorkspaceAProperProjectDatabase(currentWorkspace, restrictedDataset);

            if (isProjectValid)
            {
                //Keep button checked or unchecked.
                string btnKeepSym = ThisAddIn.IDs.Button_View_KeepCustomStyle;
                Button_View_KeepCustomStyle Button_View_KeepCustomStyle = AddIn.FromID<Button_View_KeepCustomStyle>(btnKeepSym);

                if (Button_View_KeepCustomStyle.Checked)
                {
                    Button_View_KeepCustomStyle.Checked = false;
                    Properties.Settings.Default.KeepCustomSymbols = false;
                    Properties.Settings.Default.Save();
                }
                else
                {
                    Button_View_KeepCustomStyle.Checked = true;
                    Properties.Settings.Default.KeepCustomSymbols = true;
                    Properties.Settings.Default.Save();
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
                LoadLastState();
            }
            else
            {
                this.Enabled = false;
            }
        }

        /// <summary>
        /// Will put back the button to checked stated if last time it
        /// was saved like so
        /// </summary>
        public void LoadLastState()
        {
            if (Properties.Settings.Default.KeepCustomSymbols)
            {
                //Get button object
                string btnKeepSym = ThisAddIn.IDs.Button_View_KeepCustomStyle;
                Button_View_KeepCustomStyle Button_View_KeepCustomStyle = AddIn.FromID<Button_View_KeepCustomStyle>(btnKeepSym);

                Button_View_KeepCustomStyle.Checked = true;

            }

            
        }
    }
}
