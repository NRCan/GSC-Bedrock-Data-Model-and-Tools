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
    public partial class Form_ProjectMetadata_Roles : Form
    {
        #region Main Variables

        //Person table
        private Dictionary<string, List<string>> personDico { get; set; } //A dictionnary to keep person name and ids
        private const string personFieldID = GSC_ProjectEditor.Constants.DatabaseFields.PersonID;
        private const string personFieldName = GSC_ProjectEditor.Constants.DatabaseFields.PersonAbbr;
        private const string personTable = GSC_ProjectEditor.Constants.Database.TPerson;
        private const string personAbb = GSC_ProjectEditor.Constants.DatabaseFields.PersonAbbr;
        private const string personAlias = GSC_ProjectEditor.Constants.DatabaseFields.PersonAlias;
        private const string personOrg = GSC_ProjectEditor.Constants.DatabaseFields.PersonOrg;

        //Organisation table
        private const string organisationFieldID = GSC_ProjectEditor.Constants.DatabaseFields.OrganisationID;
        private const string orgAbbr = GSC_ProjectEditor.Constants.DatabaseFields.OrganisationAbbr;
        private const string orgTable = GSC_ProjectEditor.Constants.Database.TOrganisation;

        //Participant table
        private const string participantTableName = GSC_ProjectEditor.Constants.Database.TParticipant;
        private const string partPersonID = GSC_ProjectEditor.Constants.DatabaseFields.ParticipantPersonID;
        private const string partMActID = GSC_ProjectEditor.Constants.DatabaseFields.ParticipantMActID;
        private const string partSActID = GSC_ProjectEditor.Constants.DatabaseFields.ParticipantSActID;
        private const string partRole = GSC_ProjectEditor.Constants.DatabaseFields.ParticipantRole;
        private const string partRoleDesc = GSC_ProjectEditor.Constants.DatabaseFields.ParticipantRoleDesc;
        private const string partGeolcode = GSC_ProjectEditor.Constants.DatabaseFields.ParticipantGeolCode;
        private const string partStart = GSC_ProjectEditor.Constants.DatabaseFields.ParticipantStartDate;
        private const string partEnd = GSC_ProjectEditor.Constants.DatabaseFields.ParticipantEndDate;
        private const string partRemarks = GSC_ProjectEditor.Constants.DatabaseFields.ParticipantRemarks;
        private const string partMetaID = GSC_ProjectEditor.Constants.DatabaseFields.ParticipantMetaID;
        private const string partDomain = GSC_ProjectEditor.Constants.DatabaseDomains.participant;
        private const string partID = GSC_ProjectEditor.Constants.DatabaseFields.ParticipantID;

        //Sub Activity table
        private Dictionary<string, List<string>> activityDico { get; set; } //A dictionnary to keep activity name and ids
        private const string activityFieldName = GSC_ProjectEditor.Constants.DatabaseFields.SubActivityName;
        private const string activityFieldId = GSC_ProjectEditor.Constants.DatabaseFields.SubActivityID;
        private const string activityTable = GSC_ProjectEditor.Constants.Database.TSActivity;
        private const string activityMainID = GSC_ProjectEditor.Constants.DatabaseFields.SubActivityMainID;

        //Main activity table
        private const string mActFieldName = GSC_ProjectEditor.Constants.DatabaseFields.MainActivityName;
        private const string mActFieldID = GSC_ProjectEditor.Constants.DatabaseFields.MainActID;
        private const string mActTable = GSC_ProjectEditor.Constants.Database.TMActivity;
        private const string mActProjectID = GSC_ProjectEditor.Constants.DatabaseFields.MainActProjectID;

        //Project table
        private const string projectFieldID = GSC_ProjectEditor.Constants.DatabaseFields.ProjectID;
        private const string projectFieldName = GSC_ProjectEditor.Constants.DatabaseFields.ProjectName;
        private const string projectTable = GSC_ProjectEditor.Constants.Database.TProject;

        //Ganfeld metadata table
        private const string gMetadataTable = GSC_ProjectEditor.Constants.DatabaseGanfeld.gMetadata;
        private const string gMetadataGeologist = GSC_ProjectEditor.Constants.DatabaseGanfeldFields.metadataGeologist;
        private const string gMetadataID = GSC_ProjectEditor.Constants.DatabaseGanfeldFields.metadataID;


        //Other
        private const string roleDomain = GSC_ProjectEditor.Constants.DatabaseDomains.ActivityRole;
        private Dictionary<string, object> inFieldValues = new Dictionary<string, object>(); //Will be used to add info in legend generator table
        private Dictionary<string, string> uniqueIDNameDico = new Dictionary<string, string>();

        public UniqueSubActivityIDs noSubAct = new UniqueSubActivityIDs { subName = Properties.Resources.Message_NoSubAct, subValue = "" };

        #endregion

        #region View Model

        /// <summary>
        /// A class that will keep unique ids and names of participant person, main act and sub act info.
        /// Will be used as main participant combobox datasource
        /// </summary>
        public class UniqueIDs
        {
            public string DisplayName { get; set; } //What is shown in UI
            public string ID { get; set; } //table participant field unique id
            public string geolcode { get; set; } //table participant field
            public string remarks { get; set; } //table participant field
            public string startDate { get; set; } //table participant field
            public string endDate { get; set; } //table participant field

            public UniqueMainActivityIDs PartMainAct {get; set;} //Table participant related information
            public UniqueSubActivityIDs PartSubAct { get; set; } //Table participant related information
            public UniqueMetaIDs PartMeta { get; set; } //Table participant related information
            public UniqueRoleIDs PartRole { get; set; } //Table participant related information
        }

        /// <summary>
        /// A class that will keep unique ids and names of main activities.
        /// Will be used to fill the sub activity combobox, and as main acti. combobox datasource
        /// </summary>
        public class UniqueMainActivityIDs
        {
            public string mainName { get; set; }
            public string mainValue { get; set; }
        }

        /// <summary>
        /// A class that will keep unique ids and names of sub activities. 
        /// Will be used to fill datasource of sub act. combobox
        /// </summary>
        public class UniqueSubActivityIDs
        {
            public string subName { get; set; }
            public string subValue { get; set; }
        }

        /// <summary>
        /// A class that will keep unique ids of metadata from Ganfeld. 
        /// Will be used to fill datasource of metaid combobox.
        /// </summary>
        public class UniqueMetaIDs
        {
            public string personAbbreviatedName { get; set; }
            public string personID { get; set; }
            public string metaID { get; set; }
        }

        public class UniqueRoleIDs
        {
            public string RoleID { get; set; }
            public string RoleDesc { get; set; }
        }

        public Form_ProjectMetadata_Roles()
        {
            //Init culture
            Utilities.Culture.SetCulture();

            InitializeComponent();

            //Initiate some custom controls.
            this.Shown += new EventHandler(FormParticipantRoles_Shown);
        }

        void FormParticipantRoles_Shown(object sender, EventArgs e)
        {
            if (GSC_ProjectEditor.Properties.Settings.Default.dwEnabling)
            {
                try
                {

                    #region Fill in role comboboxes from domain values
                    Dictionary<string, string> getRoleDico = GSC_ProjectEditor.Domains.GetDomDico(roleDomain, "Description");
                    List<UniqueRoleIDs> roleDesc = new List<UniqueRoleIDs>();


                    foreach (KeyValuePair<string, string> roles in getRoleDico)
                    {
                        //Add info to lists
                        UniqueRoleIDs newRole = new UniqueRoleIDs();
                        newRole.RoleID = roles.Value;
                        newRole.RoleDesc = roles.Key;

                        roleDesc.Add(newRole);
                    }
                    this.cbox_SelectPartRole.DataSource = null;
                    this.cbox_SelectPartRole.DataSource = roleDesc;
                    this.cbox_SelectPartRole.DisplayMember = "RoleDesc";
                    this.cbox_SelectPartRole.ValueMember = "RoleID"; 
                    this.cbox_SelectPartRole.SelectedIndex = -1;


                    #endregion

                    #region Activity

                    fillMainActCombobox();

                    #endregion

                    #region Other

                    //Fill textboxes 
                    this.txtbox_PartPersonOrganisation.Text = "";

                    //Manage textbox for organisation and project
                    this.cbox_SelectPartActivity.SelectedIndexChanged += new EventHandler(cbox_SelectActivity_SelectedIndexChanged);

                    #endregion

                    fillParticipantCombobox();

                    fillMetaIDCombobox();

                    clearBoxes();

                }
                catch (Exception dwAddParticipantException)
                {
                    int lineNumber = GSC_ProjectEditor.Exceptions.LineNumber(dwAddParticipantException);
                    MessageBox.Show("dwAddParticipantException(" + lineNumber.ToString() + "):" + dwAddParticipantException.Message);
                }

            }
            else
            {
                this.Close();
            }
        }

        /// <summary>
        /// Will fill the main control of participant combobox
        /// </summary>
        public void fillParticipantCombobox()
        {
            //Variables
            List<UniqueIDs> uniqueIDList = new List<UniqueIDs>();

            //Clear all possible values
            this.cbox_SelectParticipant.DataSource = null;

            //Build a new participant, complex class
            BuildParticipantList(out uniqueIDList);

            //Iterate through dico and build a list
            if (uniqueIDList.Count == 0)
            {
                uniqueIDList.Add(new UniqueIDs { DisplayName = GSC_ProjectEditor.Properties.Resources.Error_NoParticipant, ID = "" });
            }

            this.cbox_SelectParticipant.Sorted = true;
            this.cbox_SelectParticipant.DataSource = uniqueIDList;
            this.cbox_SelectParticipant.DisplayMember = "DisplayName";
            this.cbox_SelectParticipant.ValueMember = "ID";
            this.cbox_SelectParticipant.SelectedIndex = -1;
        }

        /// <summary>
        /// Will fill the main activity combobox of proper values
        /// </summary>
        public void fillMainActCombobox()
        {
            try
            {
                //Variables
                List<UniqueMainActivityIDs> mainActIDList = new List<UniqueMainActivityIDs>();

                //Clear all possible values
                this.cbox_SelectPartActivity.DataSource = null;
                this.cbox_SelectPartActivity.Tag = "";

                //Get a list of person from table
                Dictionary<string, List<string>> getDicoActivity = GSC_ProjectEditor.Tables.GetUniqueFieldValues(mActTable, mActFieldID, null, true, mActFieldName);

                //Iterate through dico and build a list
                foreach (string getPartName in getDicoActivity["Tag"])
                {
                    //Get associated main act id.
                    int currentIndex = getDicoActivity["Tag"].IndexOf(getPartName);

                    //Add new name and id combination to class (will be combobox datasource)
                    mainActIDList.Add(new UniqueMainActivityIDs { mainName = getPartName, mainValue = getDicoActivity["Main"][currentIndex] });
                }

                if (mainActIDList.Count == 0)
                {
                    mainActIDList.Add(new UniqueMainActivityIDs { mainName = Properties.Resources.Message_NoMainActivityInDB, mainValue = "" });
                }

                //this.cbox_SelectPartActivity.Sorted = true;
                this.cbox_SelectPartActivity.DataSource = mainActIDList;
                this.cbox_SelectPartActivity.DisplayMember = "mainName";
                this.cbox_SelectPartActivity.ValueMember = "mainValue";
                this.cbox_SelectPartActivity.SelectedIndex = -1;

                this.cbox_SelectPartActivity.Tag = getDicoActivity;

            }
            catch (Exception fillMainActComboboxExcep)
            {
                int lineNumber = GSC_ProjectEditor.Exceptions.LineNumber(fillMainActComboboxExcep);
                MessageBox.Show("fillMainActComboboxExcep (" + lineNumber.ToString() + "):" + fillMainActComboboxExcep.Message);
            }

        }

        /// <summary>
        /// Will fill the sub activity combobox of proper values
        /// </summary>
        public void fillSubActCombobox()
        {
            //Get current main act information
            UniqueMainActivityIDs currentUniqueIDS = this.cbox_SelectPartActivity.SelectedItem as UniqueMainActivityIDs;
            string currentMActName = currentUniqueIDS.mainName;
            string currentMactIDs = currentUniqueIDS.mainValue;



            if (this.cbox_SelectPartActivity.SelectedIndex != -1 && !currentMActName.Contains(Properties.Resources.Message_NoMainActivityInDB) && currentMactIDs != "")
            {

                //Clear all possible values
                this.cbox_SelectPartSActivity.DataSource = null;
                this.cbox_SelectPartSActivity.Tag = "";

                //Variables
                List<UniqueSubActivityIDs> subActIDList = new List<UniqueSubActivityIDs>();
                subActIDList.Add(noSubAct);

                //Build a query to select only proper sub activities.

                string querySelectSubAct = activityMainID + " = '" + currentMactIDs + "'";

                //Get a list of person from table
                Dictionary<string, List<string>> getDicoActivity = GSC_ProjectEditor.Tables.GetUniqueFieldValues(activityTable, activityFieldId, querySelectSubAct, true, activityFieldName);

                //Iterate through dico and build a list
                foreach (string getActName in getDicoActivity["Tag"])
                {
                    //Get associated main act id.
                    int currentIndex = getDicoActivity["Tag"].IndexOf(getActName);

                    //Add new name and id combination to class (will be combobox datasource)
                    subActIDList.Add(new UniqueSubActivityIDs { subName = getActName, subValue = getDicoActivity["Main"][currentIndex] });
                }

                this.cbox_SelectPartSActivity.Tag = getDicoActivity;
                this.cbox_SelectPartSActivity.DataSource = subActIDList;
                this.cbox_SelectPartSActivity.DisplayMember = "subName";
                this.cbox_SelectPartSActivity.ValueMember = "subValue";
                this.cbox_SelectPartSActivity.SelectedIndex = 0;
            }

        }

        /// <summary>
        /// will fill the combobox of metaID coming from Ganfeld table F_METADATA, if anything is in there.
        /// </summary>
        private void fillMetaIDCombobox()
        {
            //Variables
            List<UniqueMetaIDs> metaDatasource = new List<UniqueMetaIDs>();

            //Clear all possible values and enable combobox
            this.cbox_SelectMetaID.DataSource = null;
            this.cbox_SelectMetaID.Tag = "";
            this.cbox_SelectMetaID.Enabled = true;

            //Get a list of values from F_METADATA
            Dictionary<string, string> metaIDList = GSC_ProjectEditor.Tables.GetUniqueDicoValues(gMetadataTable, gMetadataID, gMetadataGeologist, null);

            //Build datasource for combobox
            if (metaIDList.Count > 0)
            {
                //Iterate through all values
                foreach (KeyValuePair<string, string> meta in metaIDList)
                {
                    //Build cute name
                    string cuteName = meta.Value.ToString() + " (" + meta.Key.ToString() + ")";
                    metaDatasource.Add(new UniqueMetaIDs { personAbbreviatedName = cuteName, metaID = meta.Key });
                }
            }

            if (metaIDList.Count == 0)
            {
                //Disable combobox
                this.cbox_SelectMetaID.Enabled = false;
            }

            //this.cbox_SelectPartActivity.Sorted = true;
            this.cbox_SelectMetaID.DataSource = metaDatasource;
            this.cbox_SelectMetaID.DisplayMember = "geologist";
            this.cbox_SelectMetaID.ValueMember = "metaID";
            this.cbox_SelectMetaID.SelectedIndex = -1;

            this.cbox_SelectMetaID.Tag = metaIDList;
        }

        /// <summary>
        /// Adding a participant to database table, when button is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_AddParticipant_Click(object sender, EventArgs e)
        {

            try
            {
                //Make some validation on necessary fields firt
                if (this.cbox_SelectParticipant.SelectedIndex != -1 && this.cbox_SelectPartActivity.SelectedIndex != -1 && this.cbox_SelectPartRole.SelectedIndex != -1)
                {
                    #region Pre-process

                    //Get full name and full ids from combobox
                    UniqueIDs currentParticipant = this.cbox_SelectParticipant.SelectedItem as UniqueIDs;

                    #endregion

                    //Empty dico
                    inFieldValues.Clear();

                    //Get main
                    UniqueMainActivityIDs selectedMain = this.cbox_SelectPartActivity.SelectedItem as UniqueMainActivityIDs;

                    //Get sub
                    UniqueSubActivityIDs selectSub = this.cbox_SelectPartSActivity.SelectedItem as UniqueSubActivityIDs;

                    //Get role
                    UniqueRoleIDs selectedRole = this.cbox_SelectPartRole.SelectedItem as UniqueRoleIDs;

                    //Get meta
                    UniqueMetaIDs selectedMeta = this.cbox_SelectMetaID.SelectedItem as UniqueMetaIDs;

                    //Add simple interface values
                    inFieldValues[partMActID] = currentParticipant.PartMainAct.mainValue = selectedMain.mainValue;
                    inFieldValues[partGeolcode] = this.txtbox_PartGeolcode.Text; //TODO Put a real calculated geolcode value here
                    inFieldValues[partRemarks] = this.txtbox_PartRemarks.Text;
                    inFieldValues[partRole] = currentParticipant.PartRole.RoleID = selectedRole.RoleID;
                    inFieldValues[partRoleDesc] = this.txtbox_PartRoleDescription.Text;
                    inFieldValues[partStart] = this.timepckr_PartStartDate.Value.Date;
                    inFieldValues[partEnd] = this.timepckr_PartEndDate.Value.Date;
                    inFieldValues[partSActID] = currentParticipant.PartSubAct.subValue = selectSub.subValue;

                    if (selectedMeta != null)
                    {
                        inFieldValues[partMetaID] = currentParticipant.PartMeta.metaID = selectedMeta.metaID;
                    }

                    #region Update P_PARTICIPANT table with user values

                    //Build a query to select current person if exists (else it's "Add new Person")
                    string updateQuery = partID + " = '" + currentParticipant.ID + "'";

                    //Update if needed or add new value
                    if (!currentParticipant.DisplayName.Contains(Properties.Resources.Message_AddNewParticipant))
                    {
                        //Update
                        GSC_ProjectEditor.Tables.UpdateMultipleFieldValue(participantTableName, updateQuery, inFieldValues);

                        //Get new ID and update domain
                        GetNewIDAndUpdateDomain(currentParticipant.ID, false);

                    }
                    else
                    {
                        //Add new person id to list
                        inFieldValues[partPersonID] = currentParticipant.PartMeta.personID;

                        //Get new ID and update domain
                        string newID = GetNewIDAndUpdateDomain(currentParticipant.PartMeta.personID, true);
                        inFieldValues[partID] = newID;

                        //Add new
                        GSC_ProjectEditor.Tables.AddRowWithValues(participantTableName, inFieldValues);
                    }


                    #endregion

                    //Refill main combobox (label)
                    fillParticipantCombobox();

                    //Clear all values from interface
                    clearBoxes();

                    //Keep some settings
                    Properties.Settings.Default.WorkingStartDate = this.timepckr_PartStartDate.Value;
                    Properties.Settings.Default.WorkingEndDate = this.timepckr_PartEndDate.Value;
                    Properties.Settings.Default.refreshParticipant = true;
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
        /// Use to clear all values from dw controls.
        /// </summary>
        public void clearBoxes()
        {

            //Clear comboboxes
            this.cbox_SelectParticipant.SelectedIndex = -1;
            this.cbox_SelectPartActivity.SelectedIndex = -1;
            this.cbox_SelectPartSActivity.SelectedIndex = -1;
            this.cbox_SelectPartRole.SelectedIndex = -1;
            this.cbox_SelectMetaID.SelectedIndex = -1;

            //Clear textboxes
            this.txtbox_PartRoleDescription.Text = "";
            this.txtbox_PartRemarks.Text = "";
            this.txtbox_PartPersonOrganisation.Text = "";
            this.txtbox_PartGeolcode.Text = "";
            this.timepckr_PartStartDate.Value = Properties.Settings.Default.WorkingStartDate;
            this.timepckr_PartEndDate.Value = Properties.Settings.Default.WorkingEndDate;
        }

        /// <summary>
        /// Will erase all controls values from form.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_ClearBoxes_Click(object sender, EventArgs e)
        {
            clearBoxes();
        }

        /// <summary>
        /// Will fill the organisation textbox based on selected participant
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void cbox_SelectParticipant_SelectedIndexChanged(object sender, EventArgs e)
        {

            try
            {
                System.Windows.Forms.ComboBox currentBox = sender as System.Windows.Forms.ComboBox;
                UniqueIDs selectedPart = currentBox.SelectedItem as UniqueIDs;
                if (currentBox.SelectedIndex != -1 && !selectedPart.DisplayName.Contains(GSC_ProjectEditor.Properties.Resources.Error_NoParticipant))
                {
                    #region Pre-process

                    //Current selected value
                    UniqueIDs currentUniqueIDS = currentBox.SelectedItem as UniqueIDs;

                    //Build a query to filter values
                    string getPersonQuery = personAbb + " ='" + currentUniqueIDS.PartMeta.personAbbreviatedName + "'";

                    #region Get participant organization.
                    try
                    {
                        if (currentUniqueIDS.ID!= string.Empty)
                        {

                            //Retrieve a list of person, from alias, in person table
                            List<string> getPersonList = GSC_ProjectEditor.Tables.GetFieldValues(personTable, personOrg, getPersonQuery);

                            //Retrieve organisation abbreviation name
                            string participantOrbAbbr = GSC_ProjectEditor.Tables.GetFieldValues(orgTable, orgAbbr, organisationFieldID + " = '" + getPersonList[0] + "'")[0];

                            this.txtbox_PartPersonOrganisation.Text = participantOrbAbbr; //In theory only one item should be returned.
                            this.txtbox_PartPersonOrganisation.Tag = personOrg;
                        }

                    }
                    catch
                    {
                        MessageBox.Show(Properties.Resources.Error_Organisation);
                    }
                    #endregion

                    //Reset comboboxes
                    this.cbox_SelectMetaID.SelectedIndex = -1;

                    #endregion

                    if (!currentUniqueIDS.DisplayName.Contains(Properties.Resources.Message_AddNewParticipant))
                    {

                        #region Fill Main Activity combobox

                        List<UniqueMainActivityIDs> mainActCbox = this.cbox_SelectPartActivity.DataSource as List<UniqueMainActivityIDs>;
                        foreach (UniqueMainActivityIDs ids in mainActCbox)
                        {
                            if (ids.mainValue == currentUniqueIDS.PartMainAct.mainValue)
                            {
                                this.cbox_SelectPartActivity.SelectedIndex = mainActCbox.IndexOf(ids);
                                break;
                            }
                        }

                        #endregion

                        #region Fill Sub Activity combobox

                        List<UniqueSubActivityIDs> subActCbox = this.cbox_SelectPartSActivity.DataSource as List<UniqueSubActivityIDs>;
                        foreach (UniqueSubActivityIDs ids in subActCbox)
                        {
                            if (ids.subValue == currentUniqueIDS.PartSubAct.subValue)
                            {
                                this.cbox_SelectPartSActivity.SelectedIndex = subActCbox.IndexOf(ids);
                                break;
                            }
                        }

                        #endregion

                        #region Fill Role combobox
                        List<UniqueRoleIDs> roleCbox = this.cbox_SelectPartRole.DataSource as List<UniqueRoleIDs>;
                        foreach (UniqueRoleIDs roles in roleCbox)
                        {
                            if (roles.RoleID == currentUniqueIDS.PartRole.RoleID)
                            {
                                this.cbox_SelectPartRole.SelectedIndex = roleCbox.IndexOf(roles);
                                break;
                            }
                        }

                        #endregion

                        #region Fill Metadata combobox

                        List<UniqueMetaIDs> metaCbox = this.cbox_SelectMetaID.DataSource as List<UniqueMetaIDs>;
                        foreach (UniqueMetaIDs metIDs in metaCbox)
                        {
                            if (metIDs.metaID == currentUniqueIDS.PartMeta.metaID)
                            {
                                this.cbox_SelectMetaID.SelectedIndex = metaCbox.IndexOf(metIDs);
                                break;
                            }
                        }

                        #endregion

                        #region Fill other textboxes

                        this.txtbox_PartRoleDescription.Text = currentUniqueIDS.PartRole.RoleDesc;
                        this.txtbox_PartGeolcode.Text = currentUniqueIDS.geolcode;
                        this.txtbox_PartRemarks.Text = currentUniqueIDS.remarks;
                        try
                        {
                            DateTime getDateTime = Convert.ToDateTime(currentUniqueIDS.startDate);
                            DateTime getEndDateTime = Convert.ToDateTime(currentUniqueIDS.endDate);
                            this.timepckr_PartStartDate.Value = getDateTime.Date;
                            this.timepckr_PartEndDate.Value = getEndDateTime.Date;
                        }
                        catch (Exception)
                        {
                            //Reset calendars
                            this.timepckr_PartEndDate.Value = Properties.Settings.Default.WorkingEndDate;
                            this.timepckr_PartStartDate.Value = Properties.Settings.Default.WorkingStartDate;
                        }


                        #endregion

                    }
                    else
                    {
                        //Reset comboboxes
                        this.cbox_SelectPartRole.SelectedIndex = -1;
                        this.cbox_SelectPartActivity.SelectedIndex = -1;
                        this.cbox_SelectPartSActivity.SelectedIndex = -1;
                        this.cbox_SelectMetaID.SelectedIndex = -1;

                        //Reset textboxes
                        this.txtbox_PartRoleDescription.Text = "";
                        this.txtbox_PartRemarks.Text = "";

                        //Reset date
                        this.timepckr_PartStartDate.Value = Properties.Settings.Default.WorkingStartDate;
                        this.timepckr_PartEndDate.Value = Properties.Settings.Default.WorkingEndDate;
                    }

                }
            }
            catch (Exception cbox_SelectParticipant_SelectedIndexChangedExcept)
            {
                string lineNumber = GSC_ProjectEditor.Exceptions.LineNumber(cbox_SelectParticipant_SelectedIndexChangedExcept).ToString();
                MessageBox.Show("cbox_SelectParticipant_SelectedIndexChangedExcept (" + lineNumber + ") : " + cbox_SelectParticipant_SelectedIndexChangedExcept.Message);
            }

        }

        /// <summary>
        /// For any changes dones in the main activity combobox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbox_SelectPartActivity_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.cbox_SelectPartActivity.SelectedIndex != -1)
            {

                //Current selected value
                UniqueMainActivityIDs currentUniqueIDS = this.cbox_SelectPartActivity.SelectedItem as UniqueMainActivityIDs;
                string getActID = currentUniqueIDS.mainValue;

                //Get project name from project table with projectID
                try
                {
                    fillSubActCombobox();
                }
                catch (Exception activityException)
                {
                    MessageBox.Show(activityException.StackTrace);
                }
            }

        }

        /// <summary>
        /// Fill project and metaid textbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void cbox_SelectActivity_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (this.cbox_SelectPartActivity.SelectedIndex != -1)
            {

                //Current selected value
                UniqueMainActivityIDs currentUniqueIDS = this.cbox_SelectPartActivity.SelectedItem as UniqueMainActivityIDs;
                string getActID = currentUniqueIDS.mainValue;

                //Get project name from project table with projectID
                try
                {

                    fillSubActCombobox();

                    fillMetaIDCombobox();

                }
                catch (Exception activitySelectionException)
                {
                    MessageBox.Show(activitySelectionException.StackTrace);
                }

            }

        }

        /// <summary>
        /// When user clicks on the button, if a participant has been selected, a new item will be added
        /// to the particpant role list with only the participant name, no activities and roles associated to it.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_AddWorkspace_Click(object sender, EventArgs e)
        {

            //Add to list a blank participant
            List<UniqueIDs> currentList = this.cbox_SelectParticipant.DataSource as List<UniqueIDs>;

            //Retrieve info for person
            Dictionary<string, string> personDico = GSC_ProjectEditor.Tables.GetUniqueDicoValues(personTable, personFieldID, personFieldName, null);

            //Show a list of person on-screen for user to choose from
            Dictionary<string, object> personObjDico = new Dictionary<string, object>();
            foreach (KeyValuePair<string, string> ps in personDico)
            {
                personObjDico[ps.Value] = ps.Key;
            }
            string selectedPerson = GSC_ProjectEditor.Form_Generic.ShowGenericComboboxForm(Properties.Resources.Dialog_NewParticipantTitle, Properties.Resources.Dialog_NewParticipantLabel, this.Icon, personObjDico).ToString();

            //Add new selected person to cbobox
            UniqueIDs newParticipant = new UniqueIDs();
            newParticipant.PartMeta = new UniqueMetaIDs();
            newParticipant.PartMainAct = new UniqueMainActivityIDs();
            newParticipant.PartSubAct = new UniqueSubActivityIDs();
            newParticipant.PartRole = new UniqueRoleIDs();
            newParticipant.PartMeta.personID = selectedPerson;
            newParticipant.PartMeta.personAbbreviatedName = personDico[selectedPerson];
            newParticipant.DisplayName = newParticipant.PartMeta.personAbbreviatedName + " (" + Properties.Resources.Message_AddNewParticipant + ")";
            currentList.Add(newParticipant);

            this.cbox_SelectParticipant.DataSource = null;
            this.cbox_SelectParticipant.DataSource = currentList;
            this.cbox_SelectParticipant.DisplayMember = "DisplayName";
            this.cbox_SelectParticipant.ValueMember = "ID";


            this.cbox_SelectParticipant.SelectedIndex = this.cbox_SelectParticipant.Items.IndexOf(newParticipant);


        }

        #endregion

        #region Model

        /// <summary>
        /// Triggered when a new added label event is launched.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void currentActivities_newMainAdded(object sender, EventArgs e)
        {
            fillMainActCombobox();
        }

        /// <summary>
        /// Will calculate a new participant domain desc. and update or add into it the new calcualted value.
        /// </summary>
        /// <param name="domainDescription">The domain code description that will be added</param>
        /// <param name="newRow">True if the update is intented for a new row or for a current row that needs a new id</param>
        /// <returns></returns>
        public string GetNewIDAndUpdateDomain(string id, bool newRow)
        {
            //Calculate domain description
            string currentDescriptionForDomain = GetDisplayName();

            //Calculate new id
            int newID_raw = GSC_ProjectEditor.IDs.CalculateIDFromOBJECTID(participantTableName, null);
            int newID_increment = newID_raw;

            //Since the id is based on object ID, need to increment if it is used for an existing row
            if (newRow)
            {
                //Get a domain count 
                int domCount = GSC_ProjectEditor.IDs.CalculateIDFromDomainCount(partDomain);
                newID_increment = newID_raw + domCount;
                id = id + newID_increment.ToString();

                //Add to domain
                GSC_ProjectEditor.Domains.AddDomainValue(partDomain, id, currentDescriptionForDomain);
            }
            else
            {
                GSC_ProjectEditor.Domains.UpdateDomainDescription(partDomain, id, currentDescriptionForDomain);
            }



            return id;
        }

        /// <summary>
        /// Will calculate a new participant domain desc. and update or add into it the new calcualted value.
        /// </summary>
        /// <param name="domainDescription">The domain code description that will be added</param>
        /// <param name="newRow">True if the update is intented for a new row or for a current row that needs a new id</param>
        /// <returns></returns>
        public string GetNewIDAndUpdateDomainFromWorkspace(IWorkspace inWorkspace, string id, bool newRow)
        {
            //Calculate domain description
            string currentDescriptionForDomain = GetDisplayName();

            //Calculate new id
            int newID_raw = GSC_ProjectEditor.IDs.CalculateIDFromOBJECTID(participantTableName, null);
            int newID_increment = newID_raw;

            //Since the id is based on object ID, need to increment if it is used for an existing row
            if (newRow)
            {
                //Get a domain count 
                int domCount = GSC_ProjectEditor.IDs.CalculateIDFromDomainCount(partDomain);
                newID_increment = newID_raw + domCount;
                id = id + newID_increment.ToString();

                //Add to domain
                GSC_ProjectEditor.Domains.AddDomainValueFromWorkspace(inWorkspace, partDomain, id, currentDescriptionForDomain);
            }
            else
            {
                GSC_ProjectEditor.Domains.UpdateDomainDescriptionFromWorkspace(inWorkspace, partDomain, id, currentDescriptionForDomain);
            }



            return id;
        }

        /// <summary>
        /// From on-screen information, will create the domain description for participants
        /// </summary>
        /// <returns></returns>
        public string GetDisplayName() 
        {
            //Get current person abb
            UniqueIDs selectedParticipant = this.cbox_SelectParticipant.SelectedItem as UniqueIDs;

            //Get current string name for activity
            UniqueMainActivityIDs getSelectedMAID = this.cbox_SelectPartActivity.SelectedItem as UniqueMainActivityIDs;
            string getSelecteMAIDString = getSelectedMAID.mainName;

            //Get current string name for sub activity
            UniqueSubActivityIDs getSelectedSAID = this.cbox_SelectPartSActivity.SelectedItem as UniqueSubActivityIDs;
            string getSelectedSAIDString = getSelectedSAID.subName;

            //Get current string name for role
            UniqueRoleIDs selectedRole = this.cbox_SelectPartRole.SelectedItem as UniqueRoleIDs;
            string getSelectedRole = selectedRole.RoleDesc;

            //Build description
            string currentDescriptionForDomain = string.Empty;
            if (getSelectedSAIDString != Properties.Resources.Message_NoSubAct)
            {
                currentDescriptionForDomain = selectedParticipant.PartMeta.personAbbreviatedName + " (" + getSelecteMAIDString + " - " + getSelectedSAIDString + " - " + getSelectedRole + ")";
            }
            else
            {
                currentDescriptionForDomain = selectedParticipant.PartMeta.personAbbreviatedName + " (" + getSelecteMAIDString + " - " + " - " + getSelectedRole + ")";
            }

            return currentDescriptionForDomain;
        }

        /// <summary>
        /// Triggered when a new organsation event is triggered
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void currentParticipant_newPartAdded(object sender, EventArgs e)
        {
            fillParticipantCombobox();
        }

        /// <summary>
        /// Will build a participant list 
        /// </summary>
        /// <param name="outPartList"></param>
        public void BuildParticipantList(out List<UniqueIDs> outPartList)
        {
            outPartList = new List<UniqueIDs>();

            try
            {

                //Retrieve dico for roles
                Dictionary<string, string> roleDico = GSC_ProjectEditor.Domains.GetDomDico(roleDomain, "Code");

                //Retrieve dico for participant ID
                Dictionary<string, string> partidDico = GSC_ProjectEditor.Domains.GetDomDico(partDomain, "Code");

                //Retrieve info for person
                Dictionary<string, string> personDico = GSC_ProjectEditor.Tables.GetUniqueDicoValues(personTable, personFieldID, personFieldName , null);

                //Retrieve info for main act
                Dictionary<string, string> mainActivityDico = GSC_ProjectEditor.Tables.GetUniqueDicoValues(mActTable, mActFieldID, mActFieldName, null);

                //Retrieve info for sub act
                Dictionary<string, string> subActivityDico = GSC_ProjectEditor.Tables.GetUniqueDicoValues(activityTable, activityFieldId, activityFieldName, null);

                //Get a cursor to search in participant table
                ICursor partCursor = GSC_ProjectEditor.Tables.GetTableCursor("Search", null, participantTableName);

                //Get some field indexes
                int partIDIndex = partCursor.FindField(partID);
                int personIDIndex = partCursor.FindField(partPersonID);
                int mainActIDIndex = partCursor.FindField(partMActID);
                int subActIDIndex = partCursor.FindField(partSActID);
                int roleIndex = partCursor.FindField(partRole);
                int partRoleDescIndex = partCursor.FindField(partRoleDesc);
                int partRemarksIndex = partCursor.FindField(partRemarks);
                int partStartDateIndex = partCursor.FindField(partStart);
                int partEndDateIndex = partCursor.FindField(partEnd);
                int partGeolocodeIndex = partCursor.FindField(partGeolcode);

                //Iterate through table
                IRow partRow = partCursor.NextRow();
                while (partRow != null)
                {
                    //Init
                    UniqueIDs rowClass = new UniqueIDs();

                    //Get some important informations
                    rowClass.ID = partRow.get_Value(partIDIndex).ToString();
                    rowClass.PartMeta = new UniqueMetaIDs();
                    rowClass.PartMeta.personID = partRow.get_Value(personIDIndex).ToString();
                    rowClass.PartMainAct = new UniqueMainActivityIDs();
                    rowClass.PartMainAct.mainValue = partRow.get_Value(mainActIDIndex).ToString();
                    rowClass.PartSubAct = new UniqueSubActivityIDs();
                    rowClass.PartSubAct.subValue = partRow.get_Value(subActIDIndex).ToString();
                    rowClass.PartRole = new UniqueRoleIDs();
                    rowClass.PartRole.RoleID = partRow.get_Value(roleIndex).ToString();
                    rowClass.PartRole.RoleDesc = partRow.get_Value(partRoleDescIndex).ToString();
                    rowClass.remarks = partRow.get_Value(partRemarksIndex).ToString();
                    rowClass.startDate = partRow.get_Value(partStartDateIndex).ToString();
                    rowClass.endDate = partRow.get_Value(partEndDateIndex).ToString();
                    rowClass.geolcode = partRow.get_Value(partGeolocodeIndex).ToString();

                    #region Evaluation main activity informations

                    if (mainActivityDico.ContainsKey(rowClass.PartMainAct.mainValue))
                    {
                        rowClass.PartMainAct.mainName = mainActivityDico[rowClass.PartMainAct.mainValue];
                    }
                    else 
                    {
                        MessageBox.Show("An error occured, Main activity should be filled. Can't be empty...");
                    }
                    #endregion

                    #region Evaluluation sub act informations
                    if (subActivityDico.ContainsKey(rowClass.PartSubAct.subValue))
                    {
                        rowClass.PartSubAct.subName = subActivityDico[rowClass.PartSubAct.subValue];
                    }

                    #endregion

                    #region Evaluation role informations (coming from domain not a table)
                    if (roleDico.ContainsKey(rowClass.PartRole.RoleID))
                    {
                        rowClass.PartRole.RoleDesc = roleDico[rowClass.PartRole.RoleID];
                    }

                    #endregion

                    #region Evaluluation participant id informations (coming from domain not a table)
                    if (partidDico.ContainsKey(rowClass.ID))
                    {
                        rowClass.DisplayName = partidDico[rowClass.ID];
                    }

                    #endregion

                    #region Evaluluation person informations 
                    if (personDico.ContainsKey(rowClass.PartMeta.personID))
                    {
                        rowClass.PartMeta.personAbbreviatedName = personDico[rowClass.PartMeta.personID];
                    }

                    #endregion

                    outPartList.Add(rowClass);
                    partRow = partCursor.NextRow();
                }


            }
            catch (Exception buildPartDicoExcep)
            {
                string lineNumber = GSC_ProjectEditor.Exceptions.LineNumber(buildPartDicoExcep).ToString();
                MessageBox.Show("buildPartDicoExcep (" + lineNumber + "): " + buildPartDicoExcep.Message);
            }
        }

        #endregion



    }
}
