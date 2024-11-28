using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;

namespace GSC_ProjectEditor
{
    public partial class Form_Load_CartographicPoints_SelectData : Form
    {
        #region Main Variables

        public List<string> extensionFilter = new List<string> (){ ".shp", ".gdb", ".txt", ".xls", ".csv", ".mdb", ".dbf"}; 

        #endregion

        public Form_Load_CartographicPoints_SelectData()
        {
            //Init culture
            Utilities.Culture.SetCulture();

            InitializeComponent();

            this.Shown += new EventHandler(FormAddCartoPointSelectData_Shown);
        }

        void FormAddCartoPointSelectData_Shown(object sender, EventArgs e)
        {
            if (!GSC_ProjectEditor.Properties.Settings.Default.dwEnabling)
            {
                this.Close();
            }

        }

        /// <summary>
        /// Whenever clicked it'll pop a dialog to the user for him to select the wanted data to import.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_ImportData_Click(object sender, EventArgs e)
        {

            //Validate input data type
            bool didValidate = false;
            bool haveGeometry = false;
            foreach (string validExtensions in extensionFilter)
            {
                if (this.txtbox_DataPath.Text.Contains(validExtensions))
                {
                    //Detect type of data, with or without geometry
                    
                    if (validExtensions == ".shp" )
                    {
                        haveGeometry = true;
                    }
                    else if (validExtensions == ".gdb" || validExtensions == ".mdb")
                    {
                        //Detect if it's a table or a feature class
                        haveGeometry = GSC_ProjectEditor.Datasets.GDBDatasetHasGeometry(this.txtbox_DataPath.Text);

                        //If it does have geometry, check for point
                        if (haveGeometry)
                        {
                            IFeatureClass inFeatureClass = GSC_ProjectEditor.FeatureClass.OpenFeatureClassFromStringFaster(this.txtbox_DataPath.Text);
                            if (inFeatureClass.ShapeType != esriGeometryType.esriGeometryPoint)
                            {
                                GSC_ProjectEditor.Messages.ShowGenericErrorMessage(Properties.Resources.Error_WrongGeometry);
                                break;
                            }

                            GSC_ProjectEditor.ObjectManagement.ReleaseObject(inFeatureClass);

                        }
                    }

                    //Show the next form
                    Form_Load_CartographicPoints addCartoPointInfo = new Form_Load_CartographicPoints();
                    addCartoPointInfo.isGIS = haveGeometry;
                    addCartoPointInfo.dataPath = this.txtbox_DataPath.Text;
                    addCartoPointInfo.dataExtension = validExtensions;
                    addCartoPointInfo.Show();

                    //Close current form
                    this.Close();

                    didValidate = true;
                }
            }

            //If the code gets here, tell user that something was wrong
            if (!didValidate && !haveGeometry)
            {
                MessageBox.Show(Properties.Resources.Error_WrongFileType, Properties.Resources.Error_GenericTitle, MessageBoxButtons.RetryCancel, MessageBoxIcon.Error);

            }


        }

        /// <summary>
        /// Will close the current form when clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            //Close current form
            this.Close();
        }

        /// <summary>
        /// Will open a generic browse dialog in which the user can select the data that he wants to load inside
        /// the database
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_BrowseData_Click(object sender, EventArgs e)
        {
            //Open dialog
            string dataPath = GSC_ProjectEditor.Dialog.GetDataPrompt(this.Handle.ToInt32(), Properties.Resources.Message_DataPromptTitle);
            this.txtbox_DataPath.Text = dataPath;
        
        }
    }
}
