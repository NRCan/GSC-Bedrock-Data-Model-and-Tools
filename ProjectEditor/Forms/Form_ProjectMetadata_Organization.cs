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
    public partial class Form_ProjectMetadata_Organization : Form
    {
        #region Main Variables

        //Organisation table
        private const string tableOrg = GSC_ProjectEditor.Constants.Database.TOrganisation;
        private const string tableOrgName = GSC_ProjectEditor.Constants.DatabaseFields.OrganisationName;
        private const string tableOrgAdd = GSC_ProjectEditor.Constants.DatabaseFields.OrganisationAddress;
        private const string tableOrgID = GSC_ProjectEditor.Constants.DatabaseFields.OrganisationID;
        private const string tableOrgAbbr = GSC_ProjectEditor.Constants.DatabaseFields.OrganisationAbbr;
        private const string tableOrgPhone = GSC_ProjectEditor.Constants.DatabaseFields.OrganisationPhone;
        private const string tableOrgEmail = GSC_ProjectEditor.Constants.DatabaseFields.OrganisationEmail;
        private const string tableOrgWeb = GSC_ProjectEditor.Constants.DatabaseFields.OrganisationWeb;

        //Other
        private Dictionary<string, object> inFieldValues = new Dictionary<string, object>(); //Will be used to add info in legend generator table

        //Delegates and events
        public delegate void newOrgEventHandler(object sender, EventArgs e); //A delegate for execution events
        public event newOrgEventHandler newOrgAdded; //This event is triggered when a new main activity has been added within database

        #endregion

        /// <summary>
        /// A class that will keep unique ids and names of organisation 
        /// Will be used as main combobox datasource
        /// </summary>
        public class OrgIDs
        {
            public string Abbr { get; set; }
            public string IDs { get; set; }
        }

        public Form_ProjectMetadata_Organization()
        {
            //Init culture
            Utilities.Culture.SetCulture();

            InitializeComponent();

            //Initiate some custom controls.
            this.Shown += new EventHandler(FormOrganization_Shown);

        }

        void FormOrganization_Shown(object sender, EventArgs e)
        {
            if (GSC_ProjectEditor.Properties.Settings.Default.dwEnabling)
            {

                #region dock window startup and control fill

                //Fill in comboboxes
                fillOrgCombobox();

                #endregion
            }
            else
            {
                this.Close();
            }
        }

                /// <summary>
        /// Will fill the organistion list combobox
        /// </summary>
        private void fillOrgCombobox()
        {
            //Variables
            List<OrgIDs> newIDList = new List<OrgIDs>();
            newIDList.Add(new OrgIDs { Abbr = Properties.Resources.Message_AddNewOrganisation, IDs = string.Empty });


            //Clear all possible values
            this.cbox_selectOrganistion.DataSource = null;
            this.cbox_selectOrganistion.SelectedIndex = -1;
            this.cbox_selectOrganistion.Tag = "";

            //Create a dictionnary of values from domain
            //Dictionary<string, string> OrgDico = GSC_ProjectEditor.Domains.GetDomDico(domOrg, "Description");
            List<Tuple<string, string>> OrgDico = GSC_ProjectEditor.Tables.GetUniqueDoubleFieldValues(tableOrg, new Tuple<string, string>(tableOrgName, tableOrgID), null);

            //Iterate through dico and build a list
            foreach (Tuple<string, string> orgTuple in OrgDico)
            {
                newIDList.Add(new OrgIDs { Abbr = orgTuple.Item1, IDs = orgTuple.Item2 });
            }

            this.cbox_selectOrganistion.DataSource = newIDList;
            this.cbox_selectOrganistion.DisplayMember = "Abbr";
            this.cbox_selectOrganistion.ValueMember = "IDs";
            this.cbox_selectOrganistion.SelectedIndex = 0;
            
        }

        /// <summary>
        /// Will be used to fill in textbox with proper information related to selecte organisation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbox_selectOrganistion_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.cbox_selectOrganistion.SelectedIndex != -1 && this.cbox_selectOrganistion.Items.Count != 0)
                {
                    //Add current selected name
                    OrgIDs currentSelectedItem = this.cbox_selectOrganistion.SelectedItem as OrgIDs;
                    string currentOrgName = currentSelectedItem.Abbr;
                    string currentOrgID = currentSelectedItem.IDs;


                    if (!currentOrgName.Contains(Properties.Resources.FindKeyWord_01))
                    {

                        //Get information from organisation table
                        string getCurrentOrgQuery = tableOrgID + " = '" + currentOrgID + "'";
                        ICursor orgCursor = GSC_ProjectEditor.Tables.GetTableCursor("Search", getCurrentOrgQuery, tableOrg);
                        IRow orgRow = orgCursor.NextRow();

                        //Get some field indexes
                        //int orgNameIndex = orgCursor.FindField(currentOrgName);
                        int orgAddressIndex = orgCursor.FindField(tableOrgAdd);
                        int orgPhoneIndex = orgCursor.FindField(tableOrgPhone);
                        int orgEmailIndex = orgCursor.FindField(tableOrgEmail);
                        int orgWebIndex = orgCursor.FindField(tableOrgWeb);
                        int orgAbbIndex = orgCursor.FindField(tableOrgAbbr);

                        //Iterate through cursor
                        while (orgRow != null)
                        {
                            //fill textbox
                            this.txtbox_OrgName.Text = currentOrgName;
                            this.txtbox_OrgAddress.Text = orgRow.get_Value(orgAddressIndex) as String;
                            this.txtbox_OrgPhone.Text = orgRow.get_Value(orgPhoneIndex) as String;
                            this.txtbox_OrgMail.Text = orgRow.get_Value(orgEmailIndex) as String;
                            this.txtbox_OrgWeb.Text = orgRow.get_Value(orgWebIndex) as String;
                            this.txtbox_OrgAbb.Text = orgRow.get_Value(orgAbbIndex) as String;

                            //Next iter.
                            orgRow = orgCursor.NextRow();
                        }

                        //Release cursor
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(orgCursor);

                    }
                    else
                    {
                        //Clear textboxes
                        clearBoxes();
                    }

                }


            }
            catch (Exception cboxSelecOrgException)
            {
                string lineNumber = GSC_ProjectEditor.Exceptions.LineNumber(cboxSelecOrgException).ToString();
                MessageBox.Show("cboxSelecOrgException (" + lineNumber + "): " + cboxSelecOrgException.Message);
            }

        }

        /// <summary>
        /// Will add a new organisation or modify an existing one, when clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_AddOrg_Click(object sender, EventArgs e)
        {
            try
            {
                //Empty dico
                inFieldValues.Clear();

                //Make some validation on necessary fields firt
                if (this.txtbox_OrgName.Text != "" && this.cbox_selectOrganistion.SelectedIndex!=-1)
                {
                    //Add simple interface values
                    inFieldValues[tableOrgName] = this.txtbox_OrgName.Text;
                    inFieldValues[tableOrgAdd] = this.txtbox_OrgAddress.Text;
                    inFieldValues[tableOrgPhone] = this.txtbox_OrgPhone.Text;
                    inFieldValues[tableOrgEmail] = this.txtbox_OrgMail.Text;
                    inFieldValues[tableOrgWeb] = this.txtbox_OrgWeb.Text;
                    inFieldValues[tableOrgAbbr] = this.txtbox_OrgAbb.Text;

                    #region Update P_ORGANISATION table with user values
                    //Get proper org code from selected item
                    OrgIDs currentItemID = this.cbox_selectOrganistion.SelectedItem as OrgIDs;
                    string currentOrgCode = currentItemID.IDs;
                    string currentOrgAbbr = currentItemID.Abbr;

                    if (currentOrgAbbr != Properties.Resources.FindKeyWord_01)
                    {
                        string updateQuery = tableOrgID + " = '" + currentOrgCode + "'";

                        //Validate if code exists within table
                        List<string> getCodeList = GSC_ProjectEditor.Tables.GetUniqueFieldValues(tableOrg, tableOrgID, updateQuery, false, null)["Main"];

                        //Update if needed or add new value
                        if (getCodeList.Count != 0)
                        {
                            //Update
                            GSC_ProjectEditor.Tables.UpdateMultipleFieldValue(tableOrg, updateQuery, inFieldValues);
                        }
                        else
                        {
                            //Add new row, with existing domain code
                            inFieldValues[tableOrgID] = GSC_ProjectEditor.IDs.CalculateIDFromOBJECTID(tableOrg, null).ToString();
                            GSC_ProjectEditor.Tables.AddRowWithValues(tableOrg, inFieldValues);

                        }

                    }
                    else
                    {

                        //Update table
                        inFieldValues[tableOrgID] = currentOrgCode;
                        GSC_ProjectEditor.Tables.AddRowWithValues(tableOrg, inFieldValues);
                    }

                    #endregion

                    //Clear all values from interface
                    clearBoxes();

                    //Refill main combobox (label)
                    fillOrgCombobox();

                    //Start event 
                    try
                    {
                        newOrgAdded(this.cbox_selectOrganistion.SelectedItem, e);
                    }
                    catch
                    {

                    }
                }
                else
                {
                    MessageBox.Show(Properties.Resources.EmptyFields);
                }


            }
            catch (Exception btn_AddOrg_ClickExcept)
            {
                string lineNumber = GSC_ProjectEditor.Exceptions.LineNumber(btn_AddOrg_ClickExcept).ToString();
                MessageBox.Show("btn_AddOrg_ClickExcept (" + lineNumber + ") : " + btn_AddOrg_ClickExcept.Message);
            }

        }

        /// <summary>
        /// Use to clear all values from dw controls.
        /// </summary>
        private void clearBoxes()
        {
            //Clear textboxes
            this.txtbox_OrgName.Text = "";
            this.txtbox_OrgAddress.Text = "";
            this.txtbox_OrgPhone.Text = "";
            this.txtbox_OrgMail.Text = "";
            this.txtbox_OrgWeb.Text = "";
            this.txtbox_OrgAbb.Text = "";

            //Clear comboboxes
            this.cbox_selectOrganistion.SelectedIndex = 0;
        }

        private void btn_ClearOrgBoxes_Click(object sender, EventArgs e)
        {
            this.clearBoxes();
        }
    }
}
