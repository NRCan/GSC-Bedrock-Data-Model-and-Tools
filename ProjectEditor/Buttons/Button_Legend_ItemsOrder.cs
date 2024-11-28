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
    public class Button_Legend_ItemsOrder : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        Form_Legend_ItemsOrder newOrderForm;

        public Button_Legend_ItemsOrder()
        {
        }

        protected override void OnClick()
        {
            List<string> restrictedDataset = new List<string>();
            restrictedDataset.Add(Constants.Database.TLegendGene);

            string currentWorkspacePath = Properties.Settings.Default.PROJECT_DATABASE_PATH;
            IWorkspace currentWorkspace = Workspace.AccessWorkspace(currentWorkspacePath);
            bool isProjectValid = Workspace.isWorkspaceAProperProjectDatabase(currentWorkspace, restrictedDataset);

            if (isProjectValid)
            {
                try
                {
                    //Pop the new legend item order
                    if (newOrderForm == null)
                    {
                        newOrderForm = new Form_Legend_ItemsOrder();
                        newOrderForm.Closed += new EventHandler(newOrderForm_Closed);
                        newOrderForm.Show();
                    }
                    else
                    {
                        //Focus
                        if (newOrderForm.WindowState == FormWindowState.Minimized)
                        {
                            newOrderForm.WindowState = FormWindowState.Normal;
                        }
                        newOrderForm.Focus();
                    }

                }

                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }

        }

        void newOrderForm_Closed(object sender, EventArgs e)
        {
            //Reset form object
            newOrderForm = null;
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
