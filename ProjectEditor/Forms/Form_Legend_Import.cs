using ESRI.ArcGIS.Geodatabase;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GSC_ProjectEditor
{
    public partial class Form_Legend_Import : Form
    {
        public List<string> extensionFilter = new List<string>() { ".csv" };

        //Get workspace
        string workPath = string.Empty;
        IWorkspace projectWorkspace = null;


        public Form_Legend_Import()
        {
            //Init culture
            Utilities.Culture.SetCulture();

            InitializeComponent();

            fillLegendTableList();
        }

        /// <summary>
        /// Will fill the map list with current CGM maps from the feature in the database
        /// </summary>
        public void fillLegendTableList()
        {
            workPath = GSC_ProjectEditor.Properties.Settings.Default.PROJECT_DATABASE_PATH;
            projectWorkspace = GSC_ProjectEditor.Workspace.AccessWorkspace(workPath);

            //Reset
            this.combobox_legend_tables.Items.Clear();

            //Get a list of available CGM maps and descriptions ids from tree table
            List<string> currentTables= GSC_ProjectEditor.Tables.GetTableNameListFromWorkspace(projectWorkspace);
            List<string> currentLegendTables = currentTables.Where(x => (x.Contains(Constants.Database.TLegendGene) && !x.Contains(Constants.Database.TLegendTree) && !x.Contains(Constants.Database.TLegendDescription))).ToList();

            //Build the list
            foreach (string legends in currentLegendTables)
            {
                this.combobox_legend_tables.Items.Add(legends);
            }

        }

        /// <summary>
        /// Will close current window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btn_ImportLegend_Click(object sender, EventArgs e)
        {
            //Validate input data type
            bool didValidate = false;

            foreach (string validExtensions in extensionFilter)
            {
                if (this.txtbox_DataPath.Text.Contains(validExtensions))
                {
                    didValidate = true;

                    //Get selected table
                    if (this.combobox_legend_tables.SelectedIndex != -1)
                    {
                        //Get the file name
                        string fileNameOnly = System.IO.Path.GetFileName(this.txtbox_DataPath.Text);

                        //Get the text file workspace factory
                        IWorkspace txtFileWorkspace = GSC_ProjectEditor.Workspace.AccessTextfileWorkspace(this.txtbox_DataPath.Text);
                        ITable inputDataTable = GSC_ProjectEditor.Tables.OpenTableFromWorkspace(txtFileWorkspace, fileNameOnly);

                        //Iterate through input file
                        IQueryFilter searchQueryFilter = new QueryFilterClass();
                        ICursor searchInputCursor = inputDataTable.Search(searchQueryFilter, false);
                        IRow inputRows = null;
                        int idFieldIndex = searchInputCursor.FindField(Constants.DatabaseFields.LegendLabelID);
                        
                        //Access output legend table
                        ITable outputLegend = Tables.OpenTable(this.combobox_legend_tables.SelectedItem.ToString());

                        while ((inputRows = searchInputCursor.NextRow()) != null)
                        {
                            //Manage P_LEGEND versus the legend views
                            if (this.combobox_legend_tables.SelectedItem.ToString() == Constants.Database.TLegendGene)
                            {
                                //--Update description table and labels only

                                //Get extra info
                                int descriptionIDIndex = searchInputCursor.FindField(Constants.DatabaseFields.LegendDescriptionID);
                                int descriptionIndex = searchInputCursor.FindField(Constants.DatabaseFields.LegendDescription);
                                ITable LegendDescription = Tables.OpenTable(Constants.Database.TLegendDescription);
                                int MapUnitIDIndex = searchInputCursor.FindField(Constants.DatabaseFields.LegendMapUnit);
                                int AnnotationIDIndex = searchInputCursor.FindField(Constants.DatabaseFields.LegendAnnotation);
                                int GISDisplayIDIndex = searchInputCursor.FindField(Constants.DatabaseFields.LegendGISDisplay);
                                Dictionary<string, string> mapUnitsDomainValues = Domains.GetDomDico(Constants.DatabaseDomains.MapUnit, "Code");

                                if (descriptionIDIndex == -1)
                                {
                                    //Special case for possible description id field. User might have done some clean-up and ended 
                                    //only keeping DESCRIPTIONID_1 field instead of DESCRIPTIONID
                                    descriptionIDIndex = searchInputCursor.FindField(Constants.DatabaseFields.LegendDescriptionID + "_1");
                                }

                                if (descriptionIDIndex != -1 && descriptionIndex !=-1)
                                {
                                    string currentLegendItemID = inputRows.Value[idFieldIndex].ToString();
                                    string currentDescriptionID = inputRows.Value[descriptionIDIndex].ToString();

                                    if (currentDescriptionID != null && currentDescriptionID != string.Empty)
                                    {
                                        #region DESCRIPTION table update
                                        //Select legend description row that matches input file row
                                        IQueryFilter updateQueryFilter = new QueryFilterClass();
                                        updateQueryFilter.WhereClause = Constants.DatabaseFields.LegendDescriptionID + " = " + currentDescriptionID;

                                        //Iterate
                                        ICursor legendDescUpdateCursor = LegendDescription.Update(updateQueryFilter, true);
                                        IRow legendDescRow = null;
                                        int currentFielIndex = legendDescUpdateCursor.Fields.FindField(Constants.DatabaseFields.LegendDescription);

                                        while ((legendDescRow = legendDescUpdateCursor.NextRow()) != null)
                                        {
                                            //Get current field value and name
                                            object currentFieldValue = inputRows.Value[descriptionIndex];

                                            if (currentFielIndex != -1)
                                            {
                                                //Update value 
                                                legendDescRow.set_Value(currentFielIndex, currentFieldValue);
                                            }

                                            //Persist changes
                                            legendDescUpdateCursor.UpdateRow(legendDescRow);
                                        }

                                        legendDescUpdateCursor.Flush();

                                        //Release
                                        ObjectManagement.ReleaseObject(legendDescUpdateCursor);
                                        #endregion
                                    }



                                    #region LEGEND table update
                                    //Select legend row that matches input file row
                                    IQueryFilter updateLabelQueryFilter = new QueryFilterClass();
                                    updateLabelQueryFilter.WhereClause = Constants.DatabaseFields.LegendLabelID + " = '" + currentLegendItemID + "'";

                                    //Iterate
                                    ICursor legendUpdateCursor = outputLegend.Update(updateLabelQueryFilter, true);
                                    IRow legendRow = null;
                                    while ((legendRow = legendUpdateCursor.NextRow()) != null)
                                    {
                                        //Get current field value and name
                                        object currentMapUnitValue = inputRows.Value[MapUnitIDIndex];
                                        object currentAnnotationValue = inputRows.Value[AnnotationIDIndex];
                                        object currentGISDisplayValue = inputRows.Value[GISDisplayIDIndex];

                                        if (MapUnitIDIndex != -1 && AnnotationIDIndex != -1 && GISDisplayIDIndex != -1)
                                        {
                                            //Update value 
                                            legendRow.set_Value(MapUnitIDIndex, currentMapUnitValue);
                                            legendRow.set_Value(AnnotationIDIndex, currentAnnotationValue);
                                            legendRow.set_Value(GISDisplayIDIndex, currentGISDisplayValue);

                                            #region DOMAIN update for labels
                                            if (mapUnitsDomainValues.ContainsKey(inputRows.Value[idFieldIndex].ToString()))
                                            {
                                                Domains.UpdateDomainDescription(Constants.DatabaseDomains.MapUnit, currentLegendItemID, currentMapUnitValue.ToString());
                                            }
                                            

                                            #endregion
                                        }

                                        //Persist changes
                                        legendUpdateCursor.UpdateRow(legendRow);
                                    }

                                    legendUpdateCursor.Flush();

                                    //Release
                                    ObjectManagement.ReleaseObject(legendUpdateCursor);

                                    #endregion

                                }
                                else
                                {
                                    Messages.ShowGenericErrorMessage(Properties.Resources.Error_MissingKeyFields + " " + 
                                        Constants.DatabaseFields.LegendDescriptionID + ", " + Constants.DatabaseFields.LegendDescription);

                                    //Break process
                                    ObjectManagement.ReleaseObject(searchInputCursor);
                                    ObjectManagement.ReleaseObject(inputDataTable);
                                    ObjectManagement.ReleaseObject(txtFileWorkspace);
                                    this.Close();
                                    break;

                                }

                            }
                            else
                            {
                                //--Replace all rows with values from all fields

                                //Select legend row that matches input file row
                                IQueryFilter updateQueryFilter = new QueryFilterClass();
                                updateQueryFilter.WhereClause = Constants.DatabaseFields.LegendLabelID + " = '" + inputRows.Value[idFieldIndex].ToString() + "'";

                                //Iterate
                                ICursor legendUpdateCursor = outputLegend.Update(updateQueryFilter, true);
                                IRow legendRow = null;

                                while ((legendRow = legendUpdateCursor.NextRow()) != null)
                                {
                                    //Update every fields
                                    for (int i = 0; i < searchInputCursor.Fields.FieldCount; i++)
                                    {
                                        //Get current field value and name
                                        object currentFieldValue = inputRows.Value[i];
                                        string currentFieldName = searchInputCursor.Fields.Field[i].Name;

                                        //Get update field index
                                        int currentFielIndex = legendRow.Fields.FindField(currentFieldName);

                                        if (currentFielIndex != -1)
                                        {
                                            //Update value 
                                            legendRow.set_Value(currentFielIndex, currentFieldValue);
                                        }

                                    }

                                    //Persist changes
                                    legendUpdateCursor.UpdateRow(legendRow);
                                }

                                legendUpdateCursor.Flush();

                                //Release
                                ObjectManagement.ReleaseObject(legendUpdateCursor);

                            }
                        }

                        //Release
                        ObjectManagement.ReleaseObject(searchInputCursor);
                        ObjectManagement.ReleaseObject(inputDataTable);
                        ObjectManagement.ReleaseObject(txtFileWorkspace);

                        
                    }
                    else
                    {
                        Messages.ShowGenericWarning(Properties.Resources.Warning_LegendImport_MissingLegendTable);
                    }
                }

            }

            //If the code gets here, tell user that something was wrong
            if (!didValidate)
            {
                MessageBox.Show(Properties.Resources.Error_WrongFileType, Properties.Resources.Error_GenericTitle, MessageBoxButtons.RetryCancel, MessageBoxIcon.Error);
            }
            else
            {
                Messages.ShowEndOfProcess();

                //Close current form
                this.Close();
            }
        }

        /// <summary>
        /// Will pop a dialog for user to chose an input table to import
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
