using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ESRI.ArcGIS.Geodatabase;

namespace GSC_ProjectEditor
{
    /// <summary>
    /// Designer class of the dockable window add-in. It contains user interfaces that
    /// make up the dockable window.
    /// </summary>
    public partial class DockableWindow_Legend_SurroundInformation : UserControl
    {

        #region Main Variables

        // CGMP Feature
        private const string cgmTable = GSC_ProjectEditor.Constants.Database.FCGMIndex;
        private const string cgmRelatedID = GSC_ProjectEditor.Constants.DatabaseFields.FCGM_RelatedID;
        private const string cgmAbbre = GSC_ProjectEditor.Constants.DatabaseFields.FCGM_ID;
        private const string cgmMapName = GSC_ProjectEditor.Constants.DatabaseFields.FCGM_Name;
        private const string cgmMapID = GSC_ProjectEditor.Constants.DatabaseFields.FCGM_ID;
        private const string cgmAbstract = GSC_ProjectEditor.Constants.DatabaseFields.FCGM_Abstract;
        private const string cgmResmue = GSC_ProjectEditor.Constants.DatabaseFields.FCGM_Resume;
        private const string cgmRemarks = GSC_ProjectEditor.Constants.DatabaseFields.FCGM_Remarks;
        private const string cgmDescNote = GSC_ProjectEditor.Constants.DatabaseFields.FCGM_DescNote;

        //Legend tree table 
        private const string legendTreeTable = GSC_ProjectEditor.Constants.Database.TLegendTree;
        private const string legendTreeTableCGMID = GSC_ProjectEditor.Constants.DatabaseFields.LegendTreeCGM;

        //Other
        public string MainDico = "Main"; //Keyword used to get a dictionnary main value list
        public string TagDico = "Tag"; //Keyword used to get a dictionnary tag value list
        private Dictionary<string, object> inFieldValues = new Dictionary<string, object>(); //Will be used to add info in cgm feature

        #endregion

        public DockableWindow_Legend_SurroundInformation(object hook)
        {
            //Set culture before init.
            Utilities.Culture.SetCulture();

            InitializeComponent();
            this.Hook = hook;

            //Manage person list, if enabled is already open before init.
            if (this.Enabled)
            {
                dockablewindowCGM_EnabledChanged(this, null);
            }
            else if (!this.Enabled)
            {
                //Init controls if enabled.
                this.EnabledChanged += new EventHandler(dockablewindowCGM_EnabledChanged);
            }

        }

        public void dockablewindowCGM_EnabledChanged(object sender, EventArgs e)
        {
            //Fill the main combobox with cgm maps elements
            fillCGMCombobox();

        }

        #region Init.
        /// <summary>
        /// Host object of the dockable window
        /// </summary>
        private object Hook
        {
            get;
            set;
        }

        /// <summary>
        /// Implementation class of the dockable window add-in. It is responsible for 
        /// creating and disposing the user interface class of the dockable window.
        /// </summary>
        public class AddinImpl : ESRI.ArcGIS.Desktop.AddIns.DockableWindow
        {
            private DockableWindow_Legend_SurroundInformation m_windowUI;

            public AddinImpl()
            {
            }

            protected override IntPtr OnCreateChild()
            {
                m_windowUI = new DockableWindow_Legend_SurroundInformation(this.Hook);
                return m_windowUI.Handle;
            }

            protected override void Dispose(bool disposing)
            {
                if (m_windowUI != null)
                    m_windowUI.Dispose(disposing);

                base.Dispose(disposing);
            }

        }

        #endregion

        /// <summary>
        /// A class that will keep items names and ids
        /// Will be used as datasource for cgm maps combobox
        /// </summary>
        public class CGMItems
        {
            public string Name { get; set; }
            public string ID { get; set; }
        }

        /// <summary>
        /// Will fill the main combobox containing cgm maps from feature
        /// </summary>
        public void fillCGMCombobox()
        {
            //Reset
            this.cbox_selectCGMap.DataSource = null;
            List<CGMItems> newMapList = new List<CGMItems>();

            //Fill value combobox, with labels from feature
            Dictionary<string,List<string>> mapDico = GSC_ProjectEditor.Tables.GetUniqueFieldValues(cgmTable, cgmAbbre, null, true, cgmRelatedID);
            List<string> mapNameList = mapDico[MainDico];
            List<string> mapRelatedIDList = mapDico[TagDico];

            if (mapNameList.Count != 0)
            {
                foreach (string names in mapNameList)
                {
                    //Get current index
                    int currentIndex = mapNameList.IndexOf(names);

                    //Get related id from other list
                    string relatedID = mapRelatedIDList[currentIndex];

                    //Add new value into list of cgm items
                    newMapList.Add(new CGMItems { Name = names, ID = relatedID });

                }

            }
            else
            {
                //Add items
                newMapList.Add(new CGMItems { Name = Properties.Resources.Warning_NoCGM, ID = "" });

            }

            this.cbox_selectCGMap.SelectedIndex = -1;
            this.cbox_selectCGMap.DataSource = newMapList;
            this.cbox_selectCGMap.DisplayMember = "Name";
            this.cbox_selectCGMap.ValueMember = "ID";
        }

        /// <summary>
        /// Modifies the current selected CGM within feature
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_AddCGM_Click(object sender, EventArgs e)
        {
            //Parse empty textbox that are needed
            if (this.txtbox_MapID.Text != "" && this.txtbox_MapName.Text != "")
            {
                //Cast current combobox item 
                CGMItems currentCGMItem = this.cbox_selectCGMap.SelectedItem as CGMItems;

                #region Build a dictionnary of user values, related to fields within legend table

                //Empty dico
                inFieldValues.Clear();

                //Add simple interface values
                inFieldValues[cgmAbstract] = this.txtbox_MapAbstract.Text;
                inFieldValues[cgmResmue] = this.txtbox_MapResume.Text;
                inFieldValues[cgmRemarks] = this.txtbox_MapRemarks.Text;
                inFieldValues[cgmMapName] = this.txtbox_MapName.Text;
                inFieldValues[cgmMapID] = this.txtbox_MapID.Text;
                inFieldValues[cgmDescNote] = this.txtbox_MapDescNotes.Text;

                #endregion

                #region Update feature

                //Add new row within table
                string upQuery = cgmRelatedID + " = '" + currentCGMItem.ID + "'";
                GSC_ProjectEditor.Tables.UpdateMultipleFieldValue(cgmTable, upQuery, inFieldValues);

                #endregion

                #region Update table (legend tree table)

                //Detect if update is needed
                
                if (this.txtbox_MapID.Text != this.txtbox_MapID.Tag.ToString())
                {
                    MessageBox.Show(this.txtbox_MapID.Text + " != " + this.txtbox_MapID.Tag.ToString());

                    //Build query for update
                    string upTreeQuery = legendTreeTableCGMID + " = '" + this.txtbox_MapID.Tag.ToString() + "'";
                    MessageBox.Show(upTreeQuery);

                    //Change all values within legend tree table to match new id
                    GSC_ProjectEditor.Tables.UpdateFieldValue(legendTreeTable, legendTreeTableCGMID, upTreeQuery, this.txtbox_MapID.Text);
                }

                #endregion

                //Clear all values from interface
                clearBoxes();
            }
            else
            {
                MessageBox.Show(Properties.Resources.Error_CGM);
            }
        }

        /// <summary>
        /// By clicking this button the user will clear interfaces from values
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_ClearCGMBoxes_Click(object sender, EventArgs e)
        {
            clearBoxes();
        }

        /// <summary>
        /// Will clear all textboxes and re-initisalize combobox
        /// </summary>
        public void clearBoxes()
        {
            this.cbox_selectCGMap.SelectedIndex = -1;
            this.txtbox_MapAbstract.Text = "";
            this.txtbox_MapDescNotes.Text = "";
            this.txtbox_MapID.Text = "";
            this.txtbox_MapName.Text = "";
            this.txtbox_MapRemarks.Text = "";
            this.txtbox_MapResume.Text = "";
        }

        /// <summary>
        /// Will update interface with user selected cgm map information from feature table
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbox_selectCGMap_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(this.cbox_selectCGMap.SelectedIndex != -1)
            {
                //Cast current combobox item 
                CGMItems getItem = this.cbox_selectCGMap.SelectedItem as CGMItems;

                //Retrieve proper information from table
                string getQuery = cgmRelatedID + " = '" + getItem.ID + "'";
                ICursor getCursor = GSC_ProjectEditor.Tables.GetTableCursor("Search", getQuery, cgmTable);

                //Get some field indexes
                int cgmMapNameIndex = getCursor.Fields.FindField(cgmMapName);
                int cgmMapIDIndex = getCursor.Fields.FindField(cgmMapID);
                int cgmMapAbstractIndex = getCursor.Fields.FindField(cgmAbstract);
                int cgmMapDescNoteIndex = getCursor.Fields.FindField(cgmDescNote);
                int cgmMapResumeIndex = getCursor.Fields.FindField(cgmResmue);
                int cgmMapRemarkIndex = getCursor.Fields.FindField(cgmRemarks);

                //Iterate trough selection to get wanted information
                IRow getRow = null;
                while ((getRow = getCursor.NextRow()) != null)
                {
                    //Get wanted values from fields
                    string selecteMapName = getRow.get_Value(cgmMapNameIndex).ToString();
                    string selectedMapID = getRow.get_Value(cgmMapIDIndex).ToString();
                    string selectedMapAbstract = getRow.get_Value(cgmMapAbstractIndex).ToString();
                    string selectedMapDescNote = getRow.get_Value(cgmMapDescNoteIndex).ToString();
                    string selectedMapResume = getRow.get_Value(cgmMapResumeIndex).ToString();
                    string selecteMapRemarks = getRow.get_Value(cgmMapRemarkIndex).ToString();

                    //Set values into interfaces
                    this.txtbox_MapAbstract.Text = selectedMapAbstract;
                    this.txtbox_MapDescNotes.Text = selectedMapDescNote;
                    this.txtbox_MapID.Text = selectedMapID;
                    this.txtbox_MapName.Text = selecteMapName;
                    this.txtbox_MapRemarks.Text = selecteMapRemarks;
                    this.txtbox_MapResume.Text = selectedMapResume;

                    //Update textbox tag with current Map ID (needed if user changes it)
                    this.txtbox_MapID.Tag = selectedMapID;

                }

                //Release the cursor or else some lock could happen.
                System.Runtime.InteropServices.Marshal.ReleaseComObject(getCursor);
            }
        }

        /// <summary>
        /// For any double click event on the abstract textbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtbox_MapAbstract_DoubleClick(object sender, EventArgs e)
        {
            //Create a new form to enter more text
            Form_Generic_InputLongTextBox getNewForm = new Form_Generic_InputLongTextBox();

            //Sent current textbox text into new form tag
            getNewForm.Tag = this.txtbox_MapAbstract.Text;

            //Show form
            getNewForm.Show();

            //Get any event coming from the form paste button
            getNewForm.pasteButtonPushed += new Form_Generic_InputLongTextBox.pasteButtonEventHandler(getNewForm_pasteButtonPushedForAbstract);

        }

        /// <summary>
        /// If there is any event that came from the paste button for the abstract textbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void getNewForm_pasteButtonPushedForAbstract(object sender, EventArgs e)
        {
            //Cast incoming object
            TextBox getAbstractLongText = sender as TextBox;

            //Past text into interface
            this.txtbox_MapAbstract.Text = getAbstractLongText.Text;
        }

        /// <summary>
        /// For any double click event on the resume textbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtbox_MapResume_DoubleClick(object sender, EventArgs e)
        {

            //Create a new form to enter more text
            Form_Generic_InputLongTextBox getNewForm = new Form_Generic_InputLongTextBox();

            //Sent current textbox text into new form tag
            getNewForm.Tag = this.txtbox_MapResume.Text;

            //Show form
            getNewForm.Show();

            //Get any event coming from the form paste button
            getNewForm.pasteButtonPushed += new Form_Generic_InputLongTextBox.pasteButtonEventHandler(getNewForm_pasteButtonPushedForResume);

        }

        /// <summary>
        /// If there is any event that came from the paste button for the resume textbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void getNewForm_pasteButtonPushedForResume(object sender, EventArgs e)
        {
            //Cast incoming object
            TextBox getResumeLongText = sender as TextBox;

            //Past text into interface
            this.txtbox_MapResume.Text = getResumeLongText.Text;
        }

        /// <summary>
        /// For any double click event on the resume textbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtbox_MapDescNotes_DoubleClick(object sender, EventArgs e)
        {
            //Create a new form to enter more text
            Form_Generic_InputLongTextBox getNewForm = new Form_Generic_InputLongTextBox();

            //Sent current textbox text into new form tag
            getNewForm.Tag = this.txtbox_MapDescNotes.Text;

            //Show form
            getNewForm.Show();

            //Get any event coming from the form paste button
            getNewForm.pasteButtonPushed += new Form_Generic_InputLongTextBox.pasteButtonEventHandler(getNewForm_pasteButtonPushedForDescNotes);
        }

        /// <summary>
        /// If there is any event that came from the paste button for the descriptive note
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void getNewForm_pasteButtonPushedForDescNotes(object sender, EventArgs e)
        {
            //Cast incoming object
            TextBox getDescLongText = sender as TextBox;

            //Past text into interface
            this.txtbox_MapDescNotes.Text = getDescLongText.Text;
        }

    }
}
