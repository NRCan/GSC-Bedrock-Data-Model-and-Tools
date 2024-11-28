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
    public partial class Form_ProjectMetadata_Participants : Form
    {
        #region Main Variables
        //Organisation table
        private const string personTable = GSC_ProjectEditor.Constants.Database.TPerson;
        private const string personID = GSC_ProjectEditor.Constants.DatabaseFields.PersonID;
        private const string personFName = GSC_ProjectEditor.Constants.DatabaseFields.PersonFirstName;
        private const string personMName = GSC_ProjectEditor.Constants.DatabaseFields.PersonMiddleName;
        private const string personLName = GSC_ProjectEditor.Constants.DatabaseFields.PersonLastName;
        private const string personAbb = GSC_ProjectEditor.Constants.DatabaseFields.PersonAbbr;
        private const string personPhone = GSC_ProjectEditor.Constants.DatabaseFields.PersonPhone;
        private const string personEmail = GSC_ProjectEditor.Constants.DatabaseFields.PersonEmail;
        private const string personOrg = GSC_ProjectEditor.Constants.DatabaseFields.PersonOrg;
        private const string personAlias = GSC_ProjectEditor.Constants.DatabaseFields.PersonAlias;

        //Organisation table
        private const string organisationTable = GSC_ProjectEditor.Constants.Database.TOrganisation;
        private const string organisationID = GSC_ProjectEditor.Constants.DatabaseFields.OrganisationID;
        private const string organisationAbbr = GSC_ProjectEditor.Constants.DatabaseFields.OrganisationAbbr;

        //Other
        private Dictionary<string, object> inFieldValues = new Dictionary<string, object>(); //Will be used to add info in legend generator table
        public AliasValues currentAliasValues = new AliasValues();

        //Events and delegate
        public delegate void newParticipantEventHandler(object sender, EventArgs e); //A delegate for execution events
        public event newParticipantEventHandler newParticipantAdded; //This event is triggered when a new main activity has been added within database

        #endregion

        /// <summary>
        /// A class to be used to define person list combobox
        /// </summary>
        public class personDisplay
        {
            public string Abbr { get; set; }
            public string ID { get; set; }
        }

        /// <summary>
        /// A class that will keep unique ids and names of organisation 
        /// Will be used as main combobox datasource
        /// </summary>
        public class OrgDisplay
        {
            public string Abbr { get; set; }
            public string IDs { get; set; }
        }

        public class AliasValues
        {
            public string lastName { get; set; }
            public string firstName { get; set; }
            public string middleName { get; set; }
        }

        public Form_ProjectMetadata_Participants()
        {
            //Init culture
            Utilities.Culture.SetCulture();

            InitializeComponent();

            currentAliasValues.firstName = string.Empty;
            currentAliasValues.lastName = string.Empty;
            currentAliasValues.middleName = string.Empty;

            //Initiate some custom controls.
            this.Shown += new EventHandler(FormParticipant_Shown);
        }

        void FormParticipant_Shown(object sender, EventArgs e)
        {
            if (GSC_ProjectEditor.Properties.Settings.Default.dwEnabling)
            {
                #region dock window startup and control fill

                //Fill in comboboxes
                fillPersonCombobox();
                fillOrganisationCombobox();

                #endregion
            }
            else
            {
                this.Close();
            }
        }

        protected virtual void OnAliasBeingConstructed(EventArgs a)
        {
            this.txtbox_PersonAbb.Text = this.currentAliasValues.lastName + this.currentAliasValues.firstName + this.currentAliasValues.middleName;
        }

        /// <summary>
        /// Will fill in the organisation combobox with values from organisation table
        /// </summary>
        public void fillOrganisationCombobox()
        {
            //Variables
            List<OrgDisplay> newIDList = new List<OrgDisplay>();

            //Clear all possible values
            this.cbox_selectPersonOrg.DataSource = null;
            this.cbox_selectPersonOrg.SelectedIndex = this.cbox_selectPersonOrg.Items.Count - 1;
            this.cbox_selectPersonOrg.Tag = "";

            //Create a dictionnary of values from domain
            //Dictionary<string, string> OrgDico = GSC_ProjectEditor.Domains.GetDomDico(domOrg, "Description");
            List<Tuple<string, string>> OrgDico = GSC_ProjectEditor.Tables.GetUniqueDoubleFieldValues(organisationTable, new Tuple<string, string>(organisationAbbr, organisationID), null);

            //Iterate through dico and build a list
            foreach (Tuple<string, string> orgTuple in OrgDico)
            {
                newIDList.Add(new OrgDisplay { Abbr = orgTuple.Item1, IDs = orgTuple.Item2 });
            }

            this.cbox_selectPersonOrg.DataSource = newIDList;
            this.cbox_selectPersonOrg.DisplayMember = "Abbr";
            this.cbox_selectPersonOrg.ValueMember = "IDs";
            this.cbox_selectPersonOrg.SelectedIndex = -1;
            this.cbox_selectPersonOrg.Tag = OrgDico;
        }

        /// <summary>
        /// Will fill in the person combobox with values from the person table and add "New value" value.
        /// </summary>
        public void fillPersonCombobox()
        {

            //Clear all possible values
            this.cbox_selectPerson.DataSource = null;
            this.cbox_selectPerson.SelectedIndex = -1;
            this.cbox_selectPerson.Tag = "";

            //New datasource definition
            List<personDisplay> personList = new List<personDisplay>();

            //Get a list of person from table
            List<Tuple<string, string>> getDicoPerson = GSC_ProjectEditor.Tables.GetUniqueDoubleFieldValues(personTable, new Tuple<string, string>(personID, personAbb), null);

            //Iterate through dico and build a list
            foreach (Tuple<string, string> getPerson in getDicoPerson)
            {
                personList.Add(new personDisplay { Abbr = getPerson.Item2, ID = getPerson.Item1 });

            }

            //Add default value
            personList.Add(new personDisplay { Abbr = Properties.Resources.Message_AddNewPerson, ID = string.Empty });
            this.cbox_selectPerson.Items.Add(Properties.Resources.Message_AddNewPerson);

            //Add datasource
            this.cbox_selectPerson.DataSource = personList;
            this.cbox_selectPerson.DisplayMember = "Abbr";
            this.cbox_selectPerson.ValueMember = "ID";
            this.cbox_selectPerson.SelectedIndex = this.cbox_selectPerson.Items.Count - 1;
            this.cbox_selectPerson.Tag = getDicoPerson;
        }

        /// <summary>
        /// Use to clear all values from dw controls.
        /// </summary>
        public void clearBoxes()
        {
            //Clear textboxes
            this.txtbox_PersonAbb.Text = String.Empty;
            this.txtbox_PersonAlias.Text = String.Empty;
            this.txtbox_PersonEmail.Text = String.Empty;
            this.txtbox_PersonFName.Text = String.Empty;
            this.txtbox_PersonLName.Text = String.Empty;
            this.txtbox_PersonMName.Text = String.Empty;
            this.txtbox_PersonPhone.Text = String.Empty;

            //Clear comboboxes
            this.cbox_selectPerson.SelectedIndex = this.cbox_selectPerson.Items.Count - 1;
            this.cbox_selectPersonOrg.SelectedIndex = -1;
        }

        /// <summary>
        /// Button to clear all controls values from form.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_ClearPersonBoxes_Click(object sender, EventArgs e)
        {
            clearBoxes();
            fillPersonCombobox();
            fillOrganisationCombobox();
        }

        /// <summary>
        /// Will add or modify an existing person.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_AddPerson_Click(object sender, EventArgs e)
        {
            try
            {

                //Make some validation on necessary fields firt
                if (this.txtbox_PersonAbb.Text != "" && this.txtbox_PersonFName.Text != "" && this.txtbox_PersonLName.Text != "" && this.txtbox_PersonAlias.Text != "" && this.cbox_selectPersonOrg.SelectedIndex != -1)
                {
                    if (this.txtbox_PersonAlias.Text.Length <= 8)
                    {
                        //Empty dico
                        inFieldValues.Clear();

                        //Add simple interface values
                        inFieldValues[personAbb] = this.txtbox_PersonAbb.Text;
                        inFieldValues[personEmail] = this.txtbox_PersonEmail.Text;
                        inFieldValues[personFName] = this.txtbox_PersonFName.Text;
                        inFieldValues[personLName] = this.txtbox_PersonLName.Text;
                        inFieldValues[personMName] = this.txtbox_PersonMName.Text;
                        inFieldValues[personPhone] = this.txtbox_PersonPhone.Text;
                        inFieldValues[personAlias] = this.txtbox_PersonAlias.Text;

                        //Get org dom code
                        OrgDisplay currentOrganisation = this.cbox_selectPersonOrg.SelectedItem as OrgDisplay;
                        string currentOrgID = currentOrganisation.IDs;
                        inFieldValues[personOrg] = currentOrgID;

                        #region Update P_PERSON table with user values
                        personDisplay currentPerson = this.cbox_selectPerson.SelectedItem as personDisplay;
                        string currentPersonAbb = currentPerson.Abbr;

                        if (!currentPersonAbb.Contains(Properties.Resources.FindKeyWord_01))
                        {
                            //Build a query to select current person if exists (else it's "Add new Person")
                            string updateQuery = personID + " = " + currentPerson.ID;

                            //Update
                            GSC_ProjectEditor.Tables.UpdateMultipleFieldValue(personTable, updateQuery, inFieldValues);
                        }
                        else
                        {
                            //Add new row, with existing domain code
                            inFieldValues[personID] = GSC_ProjectEditor.IDs.CalculatePersonID(personTable, null);
                            GSC_ProjectEditor.Tables.AddRowWithValues(personTable, inFieldValues);
                        }

                        #endregion

                        //Clear all values from interface
                        clearBoxes();

                        //Refill main combobox (label)
                        fillPersonCombobox();

                        //Start event 
                        try
                        {
                            newParticipantAdded(this.cbox_selectPerson.SelectedItem.ToString(), e);

                        }
                        catch
                        {

                        }
                    }
                    else
                    {
                        MessageBox.Show(Properties.Resources.Error_PersonIDTooLong, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }

                }
                else
                {
                    MessageBox.Show(Properties.Resources.EmptyFields, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

            }
            catch (Exception btn_AddPersonExcept)
            {
                string lineNumber = GSC_ProjectEditor.Exceptions.LineNumber(btn_AddPersonExcept).ToString();
                MessageBox.Show("btn_AddPersonExcept (" + lineNumber + ") : " + btn_AddPersonExcept.Message);
            }
        }

        private void cbox_selectPerson_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.cbox_selectPerson.SelectedIndex != -1)
                {
                    //Get current
                    personDisplay currentPerson = this.cbox_selectPerson.SelectedItem as personDisplay;
                    string currentPersonnAbb = currentPerson.Abbr;
                    string currentPersonID = currentPerson.ID;
                    if (!currentPersonnAbb.Contains(Properties.Resources.FindKeyWord_01))
                    {
                        //Add current selected name
                        this.txtbox_PersonAbb.Text = currentPersonnAbb;

                        //Get information from person table
                        string getCurrentOrgQuery = personID + " = " + currentPersonID;
                        ICursor personCursor = GSC_ProjectEditor.Tables.GetTableCursor("Search", getCurrentOrgQuery, personTable);
                        IRow personRow = personCursor.NextRow();

                        //Get some field indexes
                        //int personIDIndex = personCursor.FindField(personID);
                        int personFNameIndex = personCursor.FindField(personFName);
                        int personMNameIndex = personCursor.FindField(personMName);
                        int personLNameIndex = personCursor.FindField(personLName);
                        int personAbbIndex = personCursor.FindField(personAbb);
                        int personPhoneIndex = personCursor.FindField(personPhone);
                        int personEmailIndex = personCursor.FindField(personEmail);
                        int personOrgIndex = personCursor.FindField(personOrg);
                        int personAliasIndex = personCursor.FindField(personAlias);

                        //Iterate through cursor
                        while (personRow != null)
                        {
                            //fill textbox
                            this.txtbox_PersonAbb.Text = personRow.get_Value(personAbbIndex) as String;
                            this.txtbox_PersonAlias.Text = personRow.get_Value(personAliasIndex) as String;
                            this.txtbox_PersonEmail.Text = personRow.get_Value(personEmailIndex) as String;
                            this.txtbox_PersonFName.Text = personRow.get_Value(personFNameIndex) as String;
                            this.txtbox_PersonLName.Text = personRow.get_Value(personLNameIndex) as String;
                            this.txtbox_PersonMName.Text = personRow.get_Value(personMNameIndex) as String;
                            this.txtbox_PersonPhone.Text = personRow.get_Value(personPhoneIndex) as String;

                            //Select proper value within combobox of organisation
                            string currentOrgID = personRow.get_Value(personOrgIndex) as String;
                            List<string> orgList = GSC_ProjectEditor.Tables.GetFieldValues(organisationTable, organisationID, null);
                            if (orgList.Contains(currentOrgID))
                            {
                                foreach (OrgDisplay item in this.cbox_selectPersonOrg.Items)
                                {
                                    if (item.IDs == currentOrgID)
                                    {
                                        this.cbox_selectPersonOrg.SelectedItem = item;
                                    }
                                }

                            }


                            //Next iter.
                            personRow = personCursor.NextRow();
                        }

                        //Release cursor
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(personCursor);


                    }
                    else
                    {
                        //Clear textboxes
                        this.txtbox_PersonAbb.Text = String.Empty;
                        this.txtbox_PersonAlias.Text = String.Empty;
                        this.txtbox_PersonEmail.Text = String.Empty;
                        this.txtbox_PersonFName.Text = String.Empty;
                        this.txtbox_PersonLName.Text = String.Empty;
                        this.txtbox_PersonMName.Text = String.Empty;
                        this.txtbox_PersonPhone.Text = String.Empty;
                        this.cbox_selectPersonOrg.SelectedIndex = -1;
                    }
                }
                else
                {
                    clearBoxes();
                }

            }
            catch (Exception cboxSelecPersonException)
            {
                string lineNumber = GSC_ProjectEditor.Exceptions.LineNumber(cboxSelecPersonException).ToString();
                MessageBox.Show("cboxSelecPersonException (" + lineNumber + "): " + cboxSelecPersonException.Message);
            }
        }

        /// <summary>
        /// Triggered when a new organsation event is triggered
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void currentPerson_newProjectAdded(object sender, EventArgs e)
        {
            fillOrganisationCombobox();
        }

        /// <summary>
        /// Whenever some text is entered here, update alias class
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtbox_PersonLName_TextChanged(object sender, EventArgs e)
        {
            if (this.txtbox_PersonLName.Text != string.Empty)
            {
                currentAliasValues.lastName = this.txtbox_PersonLName.Text + ", ";

            }
            else
            {
                this.txtbox_PersonAbb.Text = string.Empty;
                currentAliasValues.lastName = string.Empty;
            }

            OnAliasBeingConstructed(new EventArgs());

        }

        /// <summary>
        /// Whenever this textbox gets updated
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtbox_PersonMName_TextChanged(object sender, EventArgs e)
        {
            if (this.txtbox_PersonMName.Text != string.Empty)
            {
                currentAliasValues.middleName = this.txtbox_PersonMName.Text.Substring(0, 1) + ".";

            }
            else
            {
                currentAliasValues.middleName = string.Empty;
            }
            OnAliasBeingConstructed(new EventArgs());
        }

        /// <summary>
        /// Whenever this textbox gets updated
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtbox_PersonFName_TextChanged(object sender, EventArgs e)
        {
            if (this.txtbox_PersonFName.Text != string.Empty)
            {
                currentAliasValues.firstName = this.txtbox_PersonFName.Text.Substring(0, 1) + ".";

            }
            else
            {
                this.txtbox_PersonAbb.Text = string.Empty;
                currentAliasValues.firstName = string.Empty;
            }
            OnAliasBeingConstructed(new EventArgs());

        }

        private void txtbox_PersonAbb_TextChanged(object sender, EventArgs e)
        {
            TextBox abbText = sender as TextBox;

            personDisplay currentPerson = this.cbox_selectPerson.SelectedItem as personDisplay;
            string currentPersonnAbb = currentPerson.Abbr;
            string currentPersonID = currentPerson.ID;

            if (cbox_selectPerson.SelectedIndex == -1 || currentPersonnAbb.Contains(Properties.Resources.FindKeyWord_01))
            {
                abbText.Text = this.currentAliasValues.lastName + this.currentAliasValues.firstName + this.currentAliasValues.middleName;
            }
        }

    }
}
