using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.Desktop.AddIns;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Geodatabase;

namespace GSC_ProjectEditor
{
    public class Button_RefreshSymbols : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        //Refresh button is used to disable all controls on launch. This is done by addind onDemand attribute to false in the
        //config file. This forces the current button to init at the same time as arc map launches.
        public Button_RefreshSymbols()
        {
            //Set culture before init.
            Utilities.Culture.SetCulture();

            //Setting default values for dockable windows.
            GSC_ProjectEditor.Properties.Settings.Default.dwEnabling = false;

            //Init as disable by default
            DisableStopButton();

        }


        /// <summary>
        /// Will disable current button
        /// </summary>
        public void DisableStopButton()
        {
            this.Enabled = false;

            Utilities.ToolBarControls.EnableDisableAllControls(false);
        }

        protected override void OnClick()
        {
            ArcMap.Application.CurrentTool = null;

            List<string> restrictedDataset = new List<string>();
            restrictedDataset.Add(Constants.Database.FGeoline);
            restrictedDataset.Add(Constants.Database.FGeopoint);
            restrictedDataset.Add(Constants.Database.FGeopoly);
            restrictedDataset.Add(Constants.Database.FLabel);
            restrictedDataset.Add(Constants.Database.TLegendGene);

            string currentWorkspacePath = Properties.Settings.Default.PROJECT_DATABASE_PATH;
            IWorkspace currentWorkspace = Workspace.AccessWorkspace(currentWorkspacePath);
            bool isProjectValid = Workspace.isWorkspaceAProperProjectDatabase(currentWorkspace, restrictedDataset);

            if (isProjectValid)
            {
                //Pop a form for user to select which symbols he wants to refresh.
                Form_RefreshSymbols getNewForm = new Form_RefreshSymbols();
                getNewForm.Show();

                //Get any event coming from the form paste button
                getNewForm.refreshButtonPushed += new Form_RefreshSymbols.refreshButtonEventHandler(getNewForm_refreshButtonPushed);
            }


        }

        /// <summary>
        /// Will retrieve the proper checklistbox from the form and get it's values to know which feature layer to refresh
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void getNewForm_refreshButtonPushed(object sender, EventArgs e)
        {
            Properties.Settings.Default.RefreshButton = true;
            Properties.Settings.Default.Save();

            //Cast sender as the proper control to retrieve user choice
            CheckedListBox formCheckList = sender as CheckedListBox;

            //Start the chain of events depending on user choice
            if (formCheckList.GetItemCheckState(0) == CheckState.Checked) //For geopoints
            {
                Utilities.ProjectSymbols uSymbolPoints = new Utilities.ProjectSymbols();
                uSymbolPoints.RefreshGeopointSymbols();
            }

            if (formCheckList.GetItemCheckState(1) == CheckState.Checked) //For geolines
            {
                Utilities.ProjectSymbols uLineStyles = new Utilities.ProjectSymbols();
                uLineStyles.RefreshGeolineSymbols();

            }

            if (formCheckList.GetItemCheckState(2) == CheckState.Checked) //For labels and map units
            {
                Utilities.ProjectSymbols uStyleLabel = new Utilities.ProjectSymbols();
                uStyleLabel.CreateLabelTemplate(ArcMap.Application.Document as IMxDocument);

            }

            if (formCheckList.GetItemCheckState(3) == CheckState.Checked) //For labels and map units
            {
                //Call the dockwindow itself and trigger internal method to update style
                string btnCreateID = ThisAddIn.IDs.Button_CreateEdit_CreateMapUnits;
                Button_CreateEdit_CreateMapUnits addinCreatePoly = AddIn.FromID<Button_CreateEdit_CreateMapUnits>(btnCreateID);
                addinCreatePoly.RefreshMapUnitsSymbols();
            }

            GSC_ProjectEditor.Messages.ShowEndOfProcess();

            Properties.Settings.Default.RefreshButton = false;
            Properties.Settings.Default.Save();
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
