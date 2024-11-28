using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ESRI.ArcGIS.Geodatabase;

namespace GSC_ProjectEditor
{
    public partial class Form_ProjectMetadata_Activities : Form
    {
        #region Main Variables

        //Sub Activity table
        private Dictionary<string, List<string>> activityDico { get; set; } //A dictionnary to keep activity name and ids
        private const string activityFieldName = GSC_ProjectEditor.Constants.DatabaseFields.SubActivityName;
        private const string activityFieldId = GSC_ProjectEditor.Constants.DatabaseFields.SubActivityID;
        private const string activityTable = GSC_ProjectEditor.Constants.Database.TSActivity;
        private const string activityProjectID = GSC_ProjectEditor.Constants.DatabaseFields.SubActivityMainID;
        private const string activityAbbr = GSC_ProjectEditor.Constants.DatabaseFields.SubActivityAbbr;
        private const string activityStart = GSC_ProjectEditor.Constants.DatabaseFields.SubActivityStart;
        private const string activityEnd = GSC_ProjectEditor.Constants.DatabaseFields.SubActivityEnd;
        private const string activityDesc = GSC_ProjectEditor.Constants.DatabaseFields.SubActivityDesc;

        //Main Activity table
        private const string mainActTable = GSC_ProjectEditor.Constants.Database.TMActivity;
        private const string mainActID = GSC_ProjectEditor.Constants.DatabaseFields.MainActID;
        private const string mainActAbbr = GSC_ProjectEditor.Constants.DatabaseFields.MainActAbbr;
        private const string mainActStart = GSC_ProjectEditor.Constants.DatabaseFields.MainActStart;
        private const string mainActEnd = GSC_ProjectEditor.Constants.DatabaseFields.MainActEnd;
        private const string mainActDesc = GSC_ProjectEditor.Constants.DatabaseFields.MainActDesc;
        private const string mainActName = GSC_ProjectEditor.Constants.DatabaseFields.MainActivityName;
        private const string mainActProjectID = GSC_ProjectEditor.Constants.DatabaseFields.MainActProjectID;

        //Project table
        private const string projectFieldID = GSC_ProjectEditor.Constants.DatabaseFields.ProjectID;
        private const string projectFieldName = GSC_ProjectEditor.Constants.DatabaseFields.ProjectName;
        private const string projectTable = GSC_ProjectEditor.Constants.Database.TProject;

        //Other
        public Dictionary<string, string> activityFieldList = new Dictionary<string, string>();
        private Dictionary<string, object> inFieldValues = new Dictionary<string, object>(); //Will be used to add info in legend generator table
        public bool wrongKeyWord = false; //Will be used to validate a keyword for new cgm maps sub activity.

        //Delegates and events
        public delegate void newMainActivityEventHandler(object sender, EventArgs e); //A delegate for execution events
        public event newMainActivityEventHandler newMainActAdded; //This event is triggered when a new main activity has been added within database

        #endregion

        public Form_ProjectMetadata_Activities()
        {
            //Init culture
            Utilities.Culture.SetCulture();

            InitializeComponent();

            //Initiate some custom controls.
            this.Shown += new EventHandler(FormActivity_Shown);
        }

        /// <summary>
        /// Whenever the form is shown, fill in some comboboxes and other controls initialization
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void FormActivity_Shown(object sender, EventArgs e)
        {
            if (GSC_ProjectEditor.Properties.Settings.Default.dwEnabling)
            {
                try
                {

                    //Fill activity field list
                    BuildActivityFieldList();

                    //Fill other combobox
                    fillRelatedCombobox();

                    //Fill main combobox
                    fillActivityCombobox();

                }
                catch (Exception dwAddActException)
                {
                    MessageBox.Show(dwAddActException.Message);
                }
            }
            else
            {
                this.Close();
            }


        }

        /// <summary>
        /// Will fill activity combobox with either main or sub activity table information, based on user request
        /// </summary>
        public void fillActivityCombobox()
        {
            //Clear all possible values
            this.cbox_selectAct.Items.Clear();
            this.cbox_selectAct.Refresh();
            this.cbox_selectAct.Tag = "";
            this.cbox_selectAct.Items.Add(Properties.Resources.Message_AddNewActivity);

            //Variables
            int selectedIndex = 0;
            this.cbox_selectAct.SelectedIndex = selectedIndex;
            //Get a list of person from table
            Dictionary<string, List<string>> getDicoAct = GSC_ProjectEditor.Tables.GetUniqueFieldValues(activityFieldList["Table"], activityFieldList["ID"], null, true, activityFieldList["Name"]);

            //Iterate through dico and build a list
            foreach (string getActName in getDicoAct["Tag"])
            {
                this.cbox_selectAct.Items.Add(getActName);
            }

            this.cbox_selectAct.Tag = getDicoAct;
        }

        /// <summary>
        /// Based on choice, change labels in interface
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radioButton_MainAct_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton getRB = sender as RadioButton;

            if (getRB.Checked == true || this.radioButton_MainAct.Checked == true)
            {
                //Disable CGM radio button
                this.checkBox_CGM.Enabled = false;

                this.label_ActDynamic.Visible = true;
                this.label_ActDynamic.Text = Properties.Resources.Label_Activity_RelatedToProject;
                this.label_MainSubActivity.Text = Properties.Resources.Label_Activity_RelatedToMainAct;

                //Rebuild a new list of fields
                BuildActivityFieldList();

                //Fill other combobox
                fillRelatedCombobox();

                //Fill main combobox
                fillActivityCombobox();


            }
        }

        /// <summary>
        /// Based on choice, change labels in interface
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radioButton_SubAct_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton getRB = sender as RadioButton;

            if (getRB.Checked == true || this.radioButton_SubAct.Checked == true)
            {
                //Disable CGM radio button
                this.checkBox_CGM.Enabled = true;

                this.label_ActDynamic.Visible = true;
                this.label_ActDynamic.Text = Properties.Resources.Label_Activity_RelatedToMainAct;
                this.label_MainSubActivity.Text = Properties.Resources.Label_Activity_SubAct;

                //Rebuild a new list of fields
                BuildActivityFieldList();

                //Fill other combobox
                fillRelatedCombobox();

                //Fill main combobox
                fillActivityCombobox();



            }
        }

        /// <summary>
        /// Will return a dictionnary containing parsed field names, based on user main or sub activity choice.
        /// </summary>
        /// <returns></returns>
        public void BuildActivityFieldList()
        {
            if (this.radioButton_MainAct.Checked == true)
            {

                activityFieldList["Abbreviation"] = mainActAbbr;
                activityFieldList["Desc"] = mainActDesc;
                activityFieldList["End"] = mainActEnd;
                activityFieldList["ID"] = mainActID;
                activityFieldList["Name"] = mainActName;
                activityFieldList["RelatedID"] = mainActProjectID;
                activityFieldList["Start"] = mainActStart;
                activityFieldList["Table"] = mainActTable;
                activityFieldList["RelatedTable"] = projectTable;
                activityFieldList["RelatedIDField"] = projectFieldID;
                activityFieldList["RelatedNameField"] = projectFieldName;

            }

            if (this.radioButton_SubAct.Checked == true)
            {
                activityFieldList["Abbreviation"] = activityAbbr;
                activityFieldList["Desc"] = activityDesc;
                activityFieldList["End"] = activityEnd;
                activityFieldList["ID"] = activityFieldId;
                activityFieldList["Name"] = activityFieldName;
                activityFieldList["RelatedID"] = activityProjectID;
                activityFieldList["Start"] = activityStart;
                activityFieldList["Table"] = activityTable;
                activityFieldList["RelatedTable"] = mainActTable;
                activityFieldList["RelatedIDField"] = mainActID;
                activityFieldList["RelatedNameField"] = mainActName;
            }

        }

        /// <summary>
        /// Will be used to fill the project or main acticity combobox based on user choice of main or sub activity
        /// </summary>
        public void fillRelatedCombobox()
        {
            //Clear all possible values
            this.cbox_ActRelation.Items.Clear();
            this.cbox_selectAct.Refresh();
            this.cbox_ActRelation.Tag = "";

            //Get a list of person from table
            Dictionary<string, List<string>> getDicoActRel = GSC_ProjectEditor.Tables.GetUniqueFieldValues(activityFieldList["RelatedTable"], activityFieldList["RelatedIDField"], null, true, activityFieldList["RelatedNameField"]);

            //Iterate through dico and build a list
            foreach (string getRelName in getDicoActRel["Tag"])
            {
                this.cbox_ActRelation.Items.Add(getRelName);
            }

            this.cbox_ActRelation.Tag = getDicoActRel;

            if (this.cbox_ActRelation.Items.Count == 0)
            {
                this.cbox_ActRelation.Items.Add("No Project/Activity exists in database.");
            }

            this.cbox_ActRelation.SelectedIndex = 0;
        }

        /// <summary>
        /// Updates all existing textboxes and comboboxes if selected value is already in table
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbox_selectAct_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.cbox_selectAct.SelectedIndex != -1 && this.cbox_selectAct.Items.Count != 0 && this.cbox_selectAct.Tag!=null)
                {
                    ManageCGmKeyword();

                    if (!this.cbox_selectAct.SelectedItem.ToString().Contains(Properties.Resources.FindKeyWord_01))
                    {
                        //Manage CGM sub activities check button
                        if (this.cbox_selectAct.SelectedItem.ToString().Contains(Properties.Resources.Keyword_BuildCGM))
                        {
                            this.checkBox_CGM.Checked = true;
                        }
                        else
                        {
                            this.checkBox_CGM.Checked = false;
                        }


                        //Add current selected name
                        this.txtbox_ActName.Text = this.cbox_selectAct.SelectedItem.ToString();

                        //Get information from organisation table
                        Dictionary<string, List<string>> currentActCodeDico = this.cbox_selectAct.Tag as Dictionary<string, List<string>>;
                        int currentItemIndex = currentActCodeDico["Tag"].IndexOf(this.cbox_selectAct.SelectedItem.ToString());
                        string currentActID = currentActCodeDico["Main"][currentItemIndex];
                        string getCurrentActQuery = activityFieldList["ID"] + " = '" + currentActID + "'";
                        ICursor actCursor = GSC_ProjectEditor.Tables.GetTableCursor("Search", getCurrentActQuery, activityFieldList["Table"]);
                        IRow actRow = actCursor.NextRow();

                        //Get some field indexes
                        //int actIDIndex = actCursor.FindField(activityFieldList["ID"]);
                        //int actNameIndex = actCursor.FindField(activityFieldList["Name"]);
                        int actAbbrIndex = actCursor.FindField(activityFieldList["Abbreviation"]);
                        int actStartIndex = actCursor.FindField(activityFieldList["Start"]);
                        int actEndIndex = actCursor.FindField(activityFieldList["End"]);
                        int actDescIndex = actCursor.FindField(activityFieldList["Desc"]);
                        int actRelIndex = actCursor.FindField(activityFieldList["RelatedID"]);

                        //Iterate through cursor
                        while (actRow != null)
                        {
                            #region Fill textboxes
                            //fill textbox
                            this.txtbox_ActAbbr.Text = actRow.get_Value(actAbbrIndex) as String;
                            this.txtbox_ActDescription.Text = actRow.get_Value(actDescIndex) as String;
                            #endregion

                            #region Fill datepickers
                            //Fill datepicker boxes
                            try
                            {
                                DateTime getStartTime = Convert.ToDateTime(actRow.get_Value(actStartIndex).ToString());
                                this.timepckr_ActStart.Value = getStartTime.Date;
                            }
                            catch
                            {
                                this.timepckr_ActStart.Value = Convert.ToDateTime(Properties.Settings.Default.WorkingStartDate);
                            }

                            try
                            {
                                DateTime getEndTime = Convert.ToDateTime(actRow.get_Value(actEndIndex).ToString());
                                this.timepckr_ActEnd.Value = getEndTime.Date;
                            }
                            catch
                            {
                                this.timepckr_ActEnd.Value = Convert.ToDateTime(Properties.Settings.Default.WorkingEndDate);
                            }

                            #endregion

                            #region Fill combobox

                            //Get current activity id
                            string currentRelID = actRow.get_Value(actRelIndex).ToString();

                            //Retrieve a dico from rel activity combobox tag
                            Dictionary<string, List<string>> activityTag = this.cbox_ActRelation.Tag as Dictionary<string, List<string>>;
                            List<string> activityTagList = activityTag["Main"];
                            int activityTagListIndex = activityTagList.IndexOf(currentRelID);

                            if (activityTagListIndex >= 0) //In case project id wasn't in database
                            {
                                string selecteActivityName = activityTag["Tag"][activityTagListIndex];
                                this.cbox_ActRelation.SelectedItem = selecteActivityName; //In theory only one item should be returned.
                            }



                            #endregion

                            //Next iter.
                            actRow = actCursor.NextRow();
                        }

                        //Release cursor
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(actCursor);
                    }
                    else
                    {
                        //Clear textboxes
                        this.txtbox_ActAbbr.Text = "";
                        this.txtbox_ActDescription.Text = "";

                        //Reset time pickers
                        this.timepckr_ActEnd.Value = Convert.ToDateTime(Properties.Settings.Default.WorkingEndDate);
                        this.timepckr_ActStart.Value = Convert.ToDateTime(Properties.Settings.Default.WorkingStartDate);

                        //Reset combobox
                        this.cbox_ActRelation.SelectedIndex = 0;

                    }


                }

            }
            catch (Exception cboxSelectActException)
            {
                string lineNumber = GSC_ProjectEditor.Exceptions.LineNumber(cboxSelectActException).ToString();
                MessageBox.Show("cboxSelectActException (" + lineNumber + "): " + cboxSelectActException.Message);
            }
        }

        /// <summary>
        /// Will add a new activty within tables, based on user choice of main or sub activity
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_AddAct_Click(object sender, EventArgs e)
        {
            try
            {
                //Make some validation on necessary fields first
                if (this.cbox_ActRelation.SelectedIndex != -1 && this.txtbox_ActName.Text != "" && this.timepckr_ActStart.Value.Date.ToString() != "")
                {

                    #region Pre-process and get some information
                    //Empty dico
                    inFieldValues.Clear();

                    //Get related act or project id
                    Dictionary<string, List<string>> relTag = this.cbox_ActRelation.Tag as Dictionary<string, List<string>>;
                    List<string> relTagList = relTag["Tag"];
                    int relTagListIndex = relTagList.IndexOf(this.cbox_ActRelation.SelectedItem.ToString());
                    string selecteRelID = relTag["Main"][relTagListIndex];

                    //Add simple interface values
                    inFieldValues[activityFieldList["Name"]] = this.txtbox_ActName.Text;
                    inFieldValues[activityFieldList["Abbreviation"]] = this.txtbox_ActAbbr.Text;
                    inFieldValues[activityFieldList["Desc"]] = this.txtbox_ActDescription.Text;
                    inFieldValues[activityFieldList["Start"]] = this.timepckr_ActStart.Value.Date;
                    inFieldValues[activityFieldList["End"]] = this.timepckr_ActEnd.Value.Date;
                    inFieldValues[activityFieldList["RelatedID"]] = selecteRelID;

                    #endregion

                    //Manage updates or add new item
                    if (this.cbox_selectAct.SelectedIndex != -1 && !this.cbox_selectAct.SelectedItem.ToString().Contains(Properties.Resources.FindKeyWord_01))
                    {
                        #region Update selected activity
                        //Get act id
                        Dictionary<string, List<string>> actTag = this.cbox_selectAct.Tag as Dictionary<string, List<string>>;
                        List<string> actTagList = actTag["Tag"];
                        int actTagListIndex = actTagList.IndexOf(this.cbox_selectAct.SelectedItem.ToString());
                        string selectedActID = actTag["Main"][actTagListIndex];

                        //Build a query to select current person if exists (else it's "Add new Person")
                        string updateQuery = activityFieldList["ID"] + " = '" + selectedActID + "'";

                        //Validate if code exists within table
                        List<string> getActList = GSC_ProjectEditor.Tables.GetUniqueFieldValues(activityFieldList["Table"], activityFieldList["ID"], updateQuery, false, null)["Main"];

                        //Update if needed or add new value
                        if (getActList.Count != 0)
                        {
                            GSC_ProjectEditor.Tables.UpdateMultipleFieldValue(activityFieldList["Table"], updateQuery, inFieldValues);
                        }

                        #endregion
                    }
                    else
                    {
                        #region Add new row within main or sub activity table
                        //Add new main activity id to list
                        if (this.radioButton_MainAct.Checked == true)
                        {
                            //Build SQL query to filter ID
                            string mainQueryFilter = activityFieldList["RelatedID"] + " = " + selecteRelID;

                            //Calculate an id for new main activity row based on related table number of rows
                            string newID = GSC_ProjectEditor.IDs.CalculateMainActicityID(activityFieldList["Table"], mainQueryFilter, selecteRelID);

                            //Add to list
                            inFieldValues[activityFieldList["ID"]] = newID;

                            //Start event 
                            try
                            {
                                newMainActAdded(this.cbox_selectAct.SelectedItem.ToString(), e);

                            }
                            catch
                            {

                            }

                        }

                        //Add new sub activity id to list
                        if (this.radioButton_SubAct.Checked == true)
                        {
                            if (this.checkBox_CGM.Checked == true)
                            {
                                if (this.txtbox_ActName.Text.Contains(Properties.Resources.Keyword_BuildCGM))
                                {
                                    //Build SQL query to filter ID
                                    string subQueryFilter = activityFieldList["RelatedID"] + " = '" + selecteRelID + "'";

                                    //Build new ID, based on related main activity id
                                    string newID = GSC_ProjectEditor.IDs.CalculateSubActivityID(activityFieldList["Table"], subQueryFilter, selecteRelID);

                                    //Add to list
                                    inFieldValues[activityFieldList["ID"]] = newID;
                                }
                                else
                                {
                                    MessageBox.Show(Properties.Resources.EmptyFields);

                                    wrongKeyWord = true;
                                }

                            }
                            else
                            {
                                //Build SQL query to filter ID
                                string subQueryFilter = activityFieldList["RelatedID"] + " = '" + selecteRelID + "'";

                                //Build new ID, based on related main activity id
                                string newID = GSC_ProjectEditor.IDs.CalculateSubActivityID(activityFieldList["Table"], subQueryFilter, selecteRelID);

                                //Add to list
                                inFieldValues[activityFieldList["ID"]] = newID;
                            }

                        }

                        //Update
                        if (wrongKeyWord == false)
                        {
                            GSC_ProjectEditor.Tables.AddRowWithValues(activityFieldList["Table"], inFieldValues);
                        }

                        #endregion
                    }

                    #region Final processing

                    if (wrongKeyWord == false)
                    {
                        //Clear all values from interface
                        clearBoxes();

                        //Refill main combobox (label)
                        fillActivityCombobox();

                    }


                    #endregion

                    //Keep some settings
                    Properties.Settings.Default.WorkingStartDate = this.timepckr_ActStart.Value;
                    Properties.Settings.Default.WorkingEndDate = this.timepckr_ActEnd.Value;
                    Properties.Settings.Default.Save();
                }

                else
                {
                    MessageBox.Show(Properties.Resources.EmptyFields);
                }
            }
            catch (Exception btn_AddParticipantExcept)
            {
                string lineNumber = GSC_ProjectEditor.Exceptions.LineNumber(btn_AddParticipantExcept).ToString();
                MessageBox.Show("btn_AddParticipantExcept (" + lineNumber + ") : " + btn_AddParticipantExcept.Message);
            }
        }

        /// <summary>
        /// Will erase all values in textboxes and reset cbox and time pickers
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_ClearActBoxes_Click(object sender, EventArgs e)
        {
            clearBoxes();
            fillActivityCombobox();
        }

        /// <summary>
        /// Use to clear all values from dw controls.
        /// </summary>
        private void clearBoxes()
        {

            //Clear comboboxes
            this.cbox_ActRelation.SelectedIndex = 0;
            this.cbox_selectAct.SelectedIndex = this.cbox_selectAct.Items.Count - 1;

            //Reset timepickers
            this.timepckr_ActEnd.Value = Convert.ToDateTime(Properties.Settings.Default.WorkingEndDate);
            this.timepckr_ActStart.Value = Convert.ToDateTime(Properties.Settings.Default.WorkingStartDate);

            //Reset checkbox
            this.checkBox_CGM.Checked = false;

            //Clear textboxes
            this.txtbox_ActName.Text = "";
            this.txtbox_ActDescription.Text = "";
            this.txtbox_ActAbbr.Text = "";
        }

        /// <summary>
        /// Will force a keyword into new activity name
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBox_CGM_CheckedChanged(object sender, EventArgs e)
        {
            ManageCGmKeyword();
        }

        /// <summary>
        /// Intented to manage the keyword for a new cgm map sub activity
        /// </summary>
        public void ManageCGmKeyword()
        {
            //Detect if selected activity is a new one
            if (this.cbox_selectAct.SelectedIndex != -1)
            {
                if (this.cbox_selectAct.SelectedItem.ToString().Contains(Properties.Resources.FindKeyWord_01) && this.checkBox_CGM.Checked)
                {
                    //Add keyword to new activity name
                    this.txtbox_ActName.Text = Properties.Resources.Keyword_BuildCGM;

                }
            }
        }

        /// <summary>
        /// Whenever the enable setting changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBox_CGM_EnabledChanged(object sender, EventArgs e)
        {
            ManageCGmKeyword();
        }

        /// <summary>
        /// Whenever text is changed within combobox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtbox_ActName_TextChanged(object sender, EventArgs e)
        {
            //If user has selected sub-activity, build a cgm map and needed keywords aren't found within textbox
            if (this.radioButton_SubAct.Checked == true && this.checkBox_CGM.Checked == true && this.cbox_selectAct.SelectedItem.ToString().Contains(Properties.Resources.FindKeyWord_01) && !this.txtbox_ActName.Text.Contains(Properties.Resources.Keyword_BuildCGM))
            {
                this.txtbox_ActName.Text = Properties.Resources.Keyword_BuildCGM;
            }


        }

        /// <summary>
        /// Triggered when a new project event is triggered
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void currentMainActivity_newProjectAdded(object sender, EventArgs e)
        {
            fillRelatedCombobox();
        }
    }
}
