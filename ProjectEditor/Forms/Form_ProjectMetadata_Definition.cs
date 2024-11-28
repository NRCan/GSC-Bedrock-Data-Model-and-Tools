using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Globalization;
using ESRI.ArcGIS.Geodatabase;

namespace GSC_ProjectEditor
{
    public partial class Form_ProjectMetadata_Definition : Form
    {
        #region Main Variables

        //Project table
        private const string projectFieldID = GSC_ProjectEditor.Constants.DatabaseFields.ProjectID;
        private const string projectFieldName = GSC_ProjectEditor.Constants.DatabaseFields.ProjectName;
        private const string projectTable = GSC_ProjectEditor.Constants.Database.TProject;
        private const string projectNom = GSC_ProjectEditor.Constants.DatabaseFields.ProjectNom;
        private const string projectAbbr = GSC_ProjectEditor.Constants.DatabaseFields.ProjectAbbr;
        private const string projectStart = GSC_ProjectEditor.Constants.DatabaseFields.ProjectStart;
        private const string projectEnd = GSC_ProjectEditor.Constants.DatabaseFields.ProjectEnd;
        private const string projectRemarks = GSC_ProjectEditor.Constants.DatabaseFields.ProjectRemarks;
        private const string projectWebLink = GSC_ProjectEditor.Constants.DatabaseFields.ProjectWebLink;
        private const string projectCode = GSC_ProjectEditor.Constants.DatabaseFields.ProjectCode;
        private projectDisplay currentProject = new projectDisplay();

        //Other
        private Dictionary<string, object> inFieldValues = new Dictionary<string, object>(); //Will be used to add info in legend generator table

        //Delegates and events
        //public delegate void newProjectEventHandler(object sender, EventArgs e); //A delegate for execution events
        //public event newProjectEventHandler newProjectAdded; //This event is triggered when a new main activity has been added within database

        #endregion

        /// <summary>
        /// A class that will be used as datasource inside the project drop down list (combobox)
        /// </summary>
        public class projectDisplay
        {
            public string projectName { get; set; }
            public string projectID { get; set; }
        }

        public Form_ProjectMetadata_Definition()
        {
            //Init culture
            Utilities.Culture.SetCulture();

            InitializeComponent();

            //Initiate some custom controls.
            this.Shown += new EventHandler(FormProjectDefinition_Shown);
        }

        void FormProjectDefinition_Shown(object sender, EventArgs e)
        {
            if (GSC_ProjectEditor.Properties.Settings.Default.dwEnabling)
            {
                try
                {
                    getProject();

                }
                catch (Exception dwAddParticipantException)
                {
                    MessageBox.Show(dwAddParticipantException.Message);
                }
            }
            else
            {
                this.Close();
            }
        }

        /// <summary>
        /// Will set existing project from table or else init with a default empty
        /// </summary>
        public void getProject()
        {

            //Add default value
            currentProject.projectID = string.Empty;
            currentProject.projectName = Properties.Resources.Message_AddNewProject;

            //Get a list of person from table
            List<Tuple<string, string>> getDicoProject = GSC_ProjectEditor.Tables.GetUniqueDoubleFieldValues(projectTable, new Tuple<string, string>(projectFieldID, projectFieldName), null);

            //Iterate through dico and build a list
            foreach (Tuple<string, string> getProject in getDicoProject)
            {
                currentProject.projectName = getProject.Item2;
                currentProject.projectID = getProject.Item1;
            }

            //Get current
            string currentProjectName = currentProject.projectName;
            string currentProjectID = currentProject.projectID;

            if (currentProject.projectID!=string.Empty)
            {

                //Add current selected name
                this.txtbox_ProjectName.Text = currentProjectName;

                //Get information from organisation table
                string getCurrentProjQuery = projectFieldID + " = " + currentProjectID;
                ICursor prjCursor = GSC_ProjectEditor.Tables.GetTableCursor("Search", getCurrentProjQuery, projectTable);
                IRow prjRow = prjCursor.NextRow();

                //Get some field indexes
                int prjIDIndex = prjCursor.FindField(projectFieldID);
                int prjNameIndex = prjCursor.FindField(projectFieldName);
                //int prjNomIndex = prjCursor.FindField(projectNom);
                int prjAbbrIndex = prjCursor.FindField(projectAbbr);
                int prjStartIndex = prjCursor.FindField(projectStart);
                int prjEndIndex = prjCursor.FindField(projectEnd);
                int prjRemarkIndex = prjCursor.FindField(projectRemarks);
                int prjWebIndex = prjCursor.FindField(projectWebLink);
                int prjCodeIndex = prjCursor.FindField(projectCode);

                //Iterate through cursor
                while (prjRow != null)
                {
                    //fill textbox
                    this.txtbox_ProjectCode.Text = prjRow.get_Value(prjCodeIndex) as String;
                    this.txtbox_ProjectName.Text = prjRow.get_Value(prjNameIndex) as String;
                    this.txtbox_ProjectAbbr.Text = prjRow.get_Value(prjAbbrIndex) as String;
                    this.txtbox_ProjectRemarks.Text = prjRow.get_Value(prjRemarkIndex) as String;
                    this.txtbox_ProjectWebLink.Text = prjRow.get_Value(prjWebIndex) as String;

                    //Fill datepicker boxes
                    try
                    {
                        DateTime getStartTime = Convert.ToDateTime(prjRow.get_Value(prjStartIndex).ToString());
                        this.timepckr_ProjectStart.Value = getStartTime.Date;
                    }
                    catch
                    {
                        this.timepckr_ProjectStart.Value = Properties.Settings.Default.WorkingStartDate;
                    }

                    try
                    {
                        DateTime getEndTime = Convert.ToDateTime(prjRow.get_Value(prjEndIndex).ToString());
                        this.timepckr_ProjectEnd.Value = getEndTime.Date;
                    }
                    catch
                    {
                        this.timepckr_ProjectEnd.Value = Properties.Settings.Default.WorkingEndDate;
                    }

                    //Next iter.
                    prjRow = prjCursor.NextRow();
                }

                //Release cursor
                System.Runtime.InteropServices.Marshal.ReleaseComObject(prjCursor);
            }
            else
            {
                clearBoxes();
            }

        }

        /// <summary>
        /// Use to clear all values from dw controls.
        /// </summary>
        public void clearBoxes()
        {
            //Clear textboxes
            this.txtbox_ProjectCode.Text = "";
            this.txtbox_ProjectName.Text = "";
            this.txtbox_ProjectAbbr.Text = "";
            this.txtbox_ProjectRemarks.Text = "";
            this.txtbox_ProjectWebLink.Text = "";

            //Reset time pickers
            this.timepckr_ProjectEnd.Value = Properties.Settings.Default.WorkingEndDate;
            this.timepckr_ProjectStart.Value = Properties.Settings.Default.WorkingStartDate;

        }

        /// <summary>
        /// When clicked, clears all textboxes, reset comboboxes and time pickers
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_ClearProjectBoxes_Click(object sender, EventArgs e)
        {
            clearBoxes();
        }

        /// <summary>
        /// Adds or modifiy a project when clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_AddProject_Click(object sender, EventArgs e)
        {
            try
            {
                //Empty dico
                inFieldValues.Clear();

                //Make some validation on necessary fields first
                if (this.txtbox_ProjectCode.Text != "" && this.txtbox_ProjectName.Text != "")
                {

                    #region get informations
                    //Add simple interface values
                    inFieldValues[projectAbbr] = this.txtbox_ProjectAbbr.Text;
                    inFieldValues[projectStart] = this.timepckr_ProjectStart.Value.Date;
                    inFieldValues[projectEnd] = this.timepckr_ProjectEnd.Value.Date;
                    inFieldValues[projectRemarks] = this.txtbox_ProjectRemarks.Text;
                    inFieldValues[projectWebLink] = this.txtbox_ProjectWebLink.Text;
                    inFieldValues[projectCode] = this.txtbox_ProjectCode.Text;

                    //Parse project name based on current culture
                    CultureInfo currentCulture = Properties.Settings.Default.Culture;

                    if (currentCulture.Name == GSC_ProjectEditor.Constants.Culture.french)
                    {
                        inFieldValues[projectNom] = this.txtbox_ProjectName.Text;
                    }
                    else
                    {
                        inFieldValues[projectFieldName] = this.txtbox_ProjectName.Text;
                    }
                    #endregion

                    #region Update M_PROJECT table with user values
                    string currentProjectName = currentProject.projectName;

                    if (!currentProjectName.Contains(Properties.Resources.FindKeyWord_01))
                    {
                        //Get proper project from ID
                        string currentProjectID = currentProject.projectID;
                        string updateQuery = projectFieldID + " = " + currentProjectID;

                        //Update
                        GSC_ProjectEditor.Tables.UpdateMultipleFieldValue(projectTable, updateQuery, inFieldValues);


                    }
                    else if (currentProjectName.Contains(Properties.Resources.FindKeyWord_01))
                    {
                        //Update table with new row
                        inFieldValues[projectFieldID] = GSC_ProjectEditor.IDs.CalculateProjectID(projectTable, null);
                        GSC_ProjectEditor.Tables.AddRowWithValues(projectTable, inFieldValues);
                    }

                    #endregion



                    //Clear all values from interface
                    clearBoxes();

                    //Refill main combobox (label)
                    getProject();

                    //Keep some settings
                    Properties.Settings.Default.WorkingStartDate = this.timepckr_ProjectStart.Value;
                    Properties.Settings.Default.WorkingEndDate = this.timepckr_ProjectEnd.Value;
                    Properties.Settings.Default.Save();

                }
                else
                {
                    MessageBox.Show(Properties.Resources.EmptyFields);
                }

            }
            catch (Exception btn_AddProj_ClickExcept)
            {
                string lineNumber = GSC_ProjectEditor.Exceptions.LineNumber(btn_AddProj_ClickExcept).ToString();
                MessageBox.Show("btn_AddProj_ClickExcept (" + lineNumber + ") : " + btn_AddProj_ClickExcept.Message);
            }

            this.Close();
        }

    }
}
