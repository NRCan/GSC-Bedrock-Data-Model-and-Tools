using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Linq;

namespace GSC_ProjectEditor
{
    /// <summary>
    /// Designer class of the dockable window add-in. It contains user interfaces that
    /// make up the dockable window.
    /// </summary>
    public partial class DockableWindow_Legend_ItemDescription : UserControl
    {
        #region Main Variables

        //DOMAINS
        private const string symTypeCodeFill = GSC_ProjectEditor.Constants.DatabaseDomainsValues.SymTypeFill;
        private const string symTypeCodeLine = GSC_ProjectEditor.Constants.DatabaseDomainsValues.SymTypeLine;
        private const string symTypeCodePoint = GSC_ProjectEditor.Constants.DatabaseDomainsValues.SymTypePoint;
        private const string muPID = GSC_ProjectEditor.Constants.DatabaseDomains.MapUnit;

        //P_LABELS
        private const string plabels = GSC_ProjectEditor.Constants.Database.FLabel;
        private const string plabelField = GSC_ProjectEditor.Constants.DatabaseFields.FLabelID;

        //Legend generator table and fields
        private const string lTable = GSC_ProjectEditor.Constants.Database.TLegendGene;
        private const string lSymbol = GSC_ProjectEditor.Constants.DatabaseFields.LegendSymbol;
        private const string lSymType = GSC_ProjectEditor.Constants.DatabaseFields.LegendSymType;
        private const string lLabel = GSC_ProjectEditor.Constants.DatabaseFields.LegendLabelID;
        private const string lLabelName = GSC_ProjectEditor.Constants.DatabaseFields.LegendGISDisplay;
        private const string lLabelID = GSC_ProjectEditor.Constants.DatabaseFields.LegendLabelID;
        private const string lGeolRank = GSC_ProjectEditor.Constants.DatabaseFields.LegendGeolRank;
        private const string lSymTypeGeoline = GSC_ProjectEditor.Constants.DatabaseDomainsValues.SymTypeLine;
        private const string lSymTypeGeopoint = GSC_ProjectEditor.Constants.DatabaseDomainsValues.SymTypePoint;

        //Legend_Treetable
        private const string ltreeTable = GSC_ProjectEditor.Constants.Database.TLegendTree;
        private const string ltreeItemID = GSC_ProjectEditor.Constants.DatabaseFields.LegendTreeItemID;
        private const string ltreeDescID = GSC_ProjectEditor.Constants.DatabaseFields.LegendTreeDescID;
        private const string ltreeCGMID = GSC_ProjectEditor.Constants.DatabaseFields.LegendTreeCGM;

        //Legend_description table
        private const string lDescTable = GSC_ProjectEditor.Constants.Database.TLegendDescription;
        private const string lDescID = GSC_ProjectEditor.Constants.DatabaseFields.LegendDescriptionID;
        private const string lDescDescription = GSC_ProjectEditor.Constants.DatabaseFields.LegendDescription;

        // CGMP Feature
        private const string cgmTable = GSC_ProjectEditor.Constants.Database.FCGMIndex;
        private const string cgmMapID = GSC_ProjectEditor.Constants.DatabaseFields.FCGM_ID;
        private const string cgmMapName = GSC_ProjectEditor.Constants.DatabaseFields.FCGM_Name;

        //Other
        private Dictionary<string, object> inFieldValues = new Dictionary<string, object>(); //Will be used to add info in legend generator table
        private Dictionary<string, object> inFieldValuesLegendTree = new Dictionary<string, object>(); //Will be used to info in legend tree table
        private Dictionary<string, object> inFieldValuesLegendDesc = new Dictionary<string, object>(); //Will be used to add info in legend description table
        private const string symTypeDefaultValue = GSC_ProjectEditor.Constants.FieldDefaults.LegendSymbolType;
        private List<string> cboxLabelFillValues = new List<string>(); //Will be used to fill the main label combobox.
        private List<string> cboxLabelFillValuesTag = new List<string>(); //Will be used to fill the main label combobox tag with some domain codes
        public string MainDico = "Main"; //Keyword used to get a dictionnary main value list
        public string TagDico = "Tag"; //Keyword used to get a dictionnary tag value list
        public string selectedCode = GSC_ProjectEditor.Constants.DatabaseDomainsValues.BoolYes;
        public string loaded = GSC_ProjectEditor.Constants.DatabaseDomainsValues.BoolYes;
        public string notLoaded = GSC_ProjectEditor.Constants.DatabaseDomainsValues.BoolNo;
        public string newkeyword = "new"; //Will be used to catch new header items added by user.
        public static Random newRandGenerator = new Random(); //Will be used to add some temporary legend order for new headers.
        public string itemStart = "    "; //Will be used to indent items in the legend item order listbox

        #endregion

        #region Model View
        public DockableWindow_Legend_ItemDescription(object hook)
        {
            //Set culture before init.
            Utilities.Culture.SetCulture();

            InitializeComponent();
            this.Hook = hook;

            //Manage person list, if enabled is already open before init.
            if (this.Enabled)
            {
                dockablewindowCGMDesc_EnabledChanged(this, null);
            }
            else if (!this.Enabled)
            {
                //Init controls if enabled.
                this.EnabledChanged += new EventHandler(dockablewindowCGMDesc_EnabledChanged);
            }
        }

        #region Addin Init
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
            private DockableWindow_Legend_ItemDescription m_windowUI;

            public AddinImpl()
            {
            }

            protected override IntPtr OnCreateChild()
            {
                m_windowUI = new DockableWindow_Legend_ItemDescription(this.Hook);
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
        /// Will be used as datasource for item combobox
        /// </summary>
        public class LegendItems
        {
            public string ItemName { get; set; }
            public string ID { get; set; }
        }

        /// <summary>
        /// Will be used to fill in the item type combobox with default values from
        /// a related domain.
        /// </summary>
        public class LegendItemsType
        {
            public string ItemTypeName { get; set; }
            public string ItemTypeID { get; set; }
        }

        ///// <summary>
        ///// A class that will keep map ids and associated description ids
        ///// Will be used as datasource for checkboxlist of maps
        ///// </summary>
        //public class MapItems
        //{
        //    public string MapName { get; set; }
        //    public string MapDescID { get; set; }
        //}

        /// <summary>
        /// A class that will keep map ids and associated description ids
        /// Will be used as datasource for checkboxlist of maps
        /// </summary>
        public class ItemDescription
        {
            public string DescFalseID { get; set; }
            public string DescRealID { get; set; }
        }

        /// <summary>
        /// Triggered whenever the dock window becomes enable
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void dockablewindowCGMDesc_EnabledChanged(object sender, EventArgs e)
        {
            FillItemType();
        }

        /// <summary>
        /// Will fill the item type combobox with values coming from a given domain
        /// </summary>
        private void FillItemType()
        {
            //Clear old list
            this.cbox_SelectItemType.DataSource = null;

            //Variables
            List<LegendItemsType> themes = new List<LegendItemsType>();

            //Access the domain list of values
            Dictionary<string, string> themeDico = GSC_ProjectEditor.Domains.GetDomDico(GSC_ProjectEditor.Constants.DatabaseDomains.legendSymbolTheme, "Description");
            foreach (KeyValuePair<string, string> themeKeyValues in themeDico)
            {
                themes.Add(new LegendItemsType { ItemTypeName = themeKeyValues.Key, ItemTypeID = themeKeyValues.Value });

            }
            this.cbox_SelectItemType.DataSource = themes;
            this.cbox_SelectItemType.DisplayMember = "ItemTypeName";
            this.cbox_SelectItemType.ValueMember = "ItemTypeID";
            this.cbox_SelectItemType.SelectedIndex = -1;

            this.cbox_selectLegendItem.DataSource = null; //Force reset of this list else, first items gets selected with map units.
        }

        /// <summary>
        /// Will fill the items combobox from a given selected type 
        /// </summary>
        /// <param name="selectedItemType"></param>
        private void FillItems(LegendItemsType selectedItemType)
        {
            //Reset combobox
            this.cbox_selectLegendItem.DataSource = null;
            List<LegendItems> newItemList = new List<LegendItems>();

            //Fill value combobox with headers from table
            string itemTypeQuery = GSC_ProjectEditor.Constants.DatabaseFields.LegendItemType + " = '" + selectedItemType.ItemTypeID + "'";
            Dictionary<string, string> itemDico = GSC_ProjectEditor.Tables.GetUniqueDicoValues(lTable, lLabelID, lLabelName, itemTypeQuery);
            SortedDictionary<string, string> sortedDico = new SortedDictionary<string, string>();

            foreach (KeyValuePair<string, string> items in itemDico)
            {
                    if (items.Value == string.Empty)
                    {
                        sortedDico.Add(items.Key, items.Key);
                    }
                    else if (sortedDico.ContainsKey(items.Value))
                    {
                        //Try to find a proper key for dictionary
                        int duplicateNumbers = 2;
                        string keySuffix = items.Value + "(" + duplicateNumbers.ToString() + ")";

                        while (sortedDico.ContainsKey(keySuffix))
                        {
                            duplicateNumbers = duplicateNumbers + 1;
                            keySuffix = items.Value + "(" + duplicateNumbers.ToString() + ")";
                        }

                        sortedDico.Add(keySuffix, items.Key);
                    }
                    else
                    {
                        sortedDico.Add(items.Value, items.Key);
                    }
            }

            if (sortedDico.Count == 0)
            {
                newItemList.Add(new LegendItems { ItemName = Properties.Resources.Warning_NoLegendItem, ID = string.Empty });
            }
            else
            {
                foreach (KeyValuePair<string, string> heads in sortedDico)
                {
                    newItemList.Add(new LegendItems { ItemName = heads.Key, ID = heads.Value });
                }
            }

            this.cbox_selectLegendItem.SelectedIndex = -1;
            this.cbox_selectLegendItem.DataSource = newItemList;
            this.cbox_selectLegendItem.DisplayMember = "ItemName";
            this.cbox_selectLegendItem.ValueMember = "ID";
        }

        /// <summary>
        /// When clicked, will empty all control values from interface.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_MapDescClearBoxes_Click(object sender, EventArgs e)
        {
            ClearBoxes();
        }

        /// <summary>
        /// When clicked, will empty all control values from interface.
        /// </summary>
        public void ClearBoxes()
        {
            this.cbox_SelectItemType.SelectedIndex = -1;
            this.cbox_selectLegendItem.DataSource = null;
            this.cbox_selectLegendDesc.DataSource = null;
            this.txtbox_Description.Text = "";
            //this.checkBox_AllMaps.Checked = false;

            ((ListBox)this.checkedListBox_Maps).DataSource = null;

            //Get list from checked maps in listbox
            int itemCount = this.checkedListBox_Maps.Items.Count;
            for (int i = 0; i < itemCount; i++)
            {
                this.checkedListBox_Maps.SetItemChecked(i, true);
            }

        }

        /// <summary>
        /// Based on user selection, fill the item combobox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbox_SelectItemType_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Cast current combobox
            System.Windows.Forms.ComboBox itemCbox = sender as System.Windows.Forms.ComboBox;
            LegendItemsType currentSelectedType = this.cbox_SelectItemType.SelectedItem as LegendItemsType;

            //If something really changed
            if (itemCbox.SelectedIndex != -1)
            {
                FillItems(currentSelectedType);

            }
        }

        /// <summary>
        /// Will add or modify current item description in P_LEGEND_DESCRIPTION table and the treeTable will also be update.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_AddDescription_Click(object sender, EventArgs e)
        {
            //Get legend item id
            LegendItems currentItem = this.cbox_selectLegendItem.SelectedItem as LegendItems;//Cast items from combobox
            string currentItemID = currentItem.ID;

            //Get description id
            ItemDescription currentDescription = this.cbox_selectLegendDesc.SelectedItem as ItemDescription; //Cast items from combobox
            string currentDescID = currentDescription.DescRealID;
            
            #region Update P_LEGEND_DESCRIPTION and LEGEND_TREETABLE with user new values

            //Only process if user has entered a description
            if (this.txtbox_Description.Text != "")
            {
                //Calculate a new id for description if needed
                if (currentDescID == string.Empty)
                {
                    //Get a list of actual ids
                    List<string> currentDescIDS = GSC_ProjectEditor.Tables.GetFieldValues(lDescTable, lDescID, null);

                    //Calculate new id
                    currentDescID = GSC_ProjectEditor.IDs.CalculateDoubleIDFromGUID().ToString();

                    //Validate existance, else createa new one.
                    while (currentDescIDS.Contains(currentDescID))
                    {
                        currentDescID = GSC_ProjectEditor.IDs.CalculateDoubleIDFromGUID().ToString();
                    }
                    
                }

                for (int i = 0; i < this.checkedListBox_Maps.Items.Count; i++)
                {
                    //Current map id and checked state
                    CheckState checkState = this.checkedListBox_Maps.GetItemCheckState(i);
                    string maps = this.checkedListBox_Maps.Items[i].ToString();

                    //Empty dico
                    inFieldValuesLegendTree.Clear();
                    inFieldValuesLegendDesc.Clear();

                    //Access ids in Legend_TreeTable
                    string ltreeTableQuery = ltreeItemID + " = '" + currentItem.ID + "'";
                    SortedDictionary<string, List<string>> legendTreeTable = GSC_ProjectEditor.Tables.GetAllDoubleUniqueFieldValues(ltreeTable, ltreeDescID, ltreeCGMID, ltreeTableQuery);
                    List<string> legendTreeTableDescList = legendTreeTable.Keys.ToList();


                    //Access ids in legend description table
                    List<string> legendDescriptionIDList = GSC_ProjectEditor.Tables.GetUniqueFieldValues(lDescTable, lDescID, null, false, null)[MainDico];

                    //Description table values
                    inFieldValuesLegendDesc[lDescID] = currentDescID;
                    inFieldValuesLegendDesc[lDescDescription] = this.txtbox_Description.Text;

                    //Index table values
                    inFieldValuesLegendTree[ltreeItemID] = currentItem.ID;
                    inFieldValuesLegendTree[ltreeDescID] = currentDescID;
                    inFieldValuesLegendTree[ltreeCGMID] = maps;

                    #region Step 1 - Update description table

                    if (legendDescriptionIDList.Contains(currentDescID))
                    {
                        //Query
                        string updateDescriptionQuery = lDescID + " = " + currentDescID;

                        //Update description table
                        GSC_ProjectEditor.Tables.UpdateMultipleFieldValue(lDescTable, updateDescriptionQuery, inFieldValuesLegendDesc);
                        
                    }
                    else
	                {
                        //Add new rows for current checked map in description table
                        GSC_ProjectEditor.Tables.AddRowWithValues(lDescTable, inFieldValuesLegendDesc);
	                }

                    #endregion

                    #region Step 2 - Update Index (tree) table
                    //Detect if current map is used in an item
                    bool mapIDRelatedToCurrentItem = false;
                    string associatedDescID = string.Empty;

                    foreach (KeyValuePair<string, List<string>> kvDescIDMapID in legendTreeTable)
                    {
                        if (kvDescIDMapID.Value.Contains(maps))
                        {
                            //Set breaker to true and keep value
                            mapIDRelatedToCurrentItem = true;
                            associatedDescID = kvDescIDMapID.Key;
                        }
                    }


                    if (legendTreeTable.Count != 0 && mapIDRelatedToCurrentItem)
                    {
                        //Query
                        string updateDescQuery = ltreeItemID + " = '" + currentItem.ID + "' AND " + ltreeCGMID + " = '" + maps + "'";

                        #region If user wants to add a new description for a selected map, update description id
                        if (associatedDescID != currentDescID && checkState == CheckState.Checked)
                        {
                            //Update desc ID
                            GSC_ProjectEditor.Tables.UpdateMultipleFieldValue(ltreeTable, updateDescQuery, inFieldValuesLegendTree);
                        }

                        #endregion

                        #region If user wants to update a description, but the map isn't selected but was for the past description, nullify
                        if (legendTreeTable.ContainsKey(currentDescID) && checkState == CheckState.Unchecked && associatedDescID == currentDescID)
                        {
                            inFieldValuesLegendTree[ltreeDescID] = DBNull.Value;

                            //Update desc ID
                            GSC_ProjectEditor.Tables.UpdateMultipleFieldValue(ltreeTable, updateDescQuery, inFieldValuesLegendTree);
                        }
                        #endregion


                    }
                    else
                    {
                        if (checkState == CheckState.Checked)
                        {
                            //Add new row
                            GSC_ProjectEditor.Tables.AddRowWithValues(ltreeTable, inFieldValuesLegendTree);
                        }
                    }


                    #endregion


                }

            }
            else
            {
                MessageBox.Show(Properties.Resources.EmptyFields);
            }

            #endregion

            ClearBoxes();
        }

        /// <summary>
        /// Enable or disable checkboxlist control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBox_AllMaps_CheckedChanged(object sender, EventArgs e)
        {
            //Cast
            CheckBox currentCheckbox = sender as CheckBox;

            if (currentCheckbox.Checked)
            {
                ((ListBox)this.checkedListBox_Maps).DataSource = null;
                for (int i = 0; i < this.checkedListBox_Maps.Items.Count; i++)
                {
                    this.checkedListBox_Maps.SetItemChecked(i, true);
                }
            }
            else
            {
                ((ListBox)this.checkedListBox_Maps).DataSource = null;
                for (int i = 0; i < this.checkedListBox_Maps.Items.Count; i++)
                {
                    this.checkedListBox_Maps.SetItemChecked(i, false);
                }
            }
        }

        /// <summary>
        /// Will fill the map list with current CGM maps from the feature in the database
        /// </summary>
        public void fillMapList()
        {

            //Reset
            this.checkedListBox_Maps.Items.Clear();

            //Get a list of available CGM maps and descriptions ids from tree table
            List<string> currentMaps = GSC_ProjectEditor.Tables.GetUniqueFieldValues(ltreeTable, ltreeCGMID, null, false, null)[MainDico];

            //Build the list
            foreach (string maps in currentMaps)
            {
                //Add map ids and description id to list that will go in checkbox list
                this.checkedListBox_Maps.Items.Add(maps);
            }

            if (this.checkBox_AllMaps.Checked)
            {
                ((ListBox)this.checkedListBox_Maps).DataSource = null;
                for (int i = 0; i < this.checkedListBox_Maps.Items.Count; i++)
                {
                    this.checkedListBox_Maps.SetItemChecked(i, true);
                }
            }

        }

        /// <summary>
        /// Will pop another form to enter more text.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void txtbox_Description_DoubleClick(object sender, EventArgs e)
        {
            //Create a new form to enter more text
            Form_Generic_InputLongTextBox getNewForm = new Form_Generic_InputLongTextBox();

            //Sent current textbox text into new form tag
            getNewForm.Tag = this.txtbox_Description.Text;

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
            this.txtbox_Description.Text = getResumeLongText.Text;
        }

        /// <summary>
        /// When an item is selected fill description textbox and checkboxlist
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbox_selectLegendItem_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Cast items from combobox
            LegendItems currentSource = this.cbox_selectLegendItem.SelectedItem as LegendItems;

            //Reset description combobox
            this.cbox_selectLegendDesc.DataSource = null;
            List<ItemDescription> newDescList = new List<ItemDescription>(); //Will be used to fill combobox of items

            if (this.cbox_selectLegendItem.SelectedIndex != -1)
            {
                #region Retrieve items
                //Variables
                string legendItemID = "";

                //Get current selected item id
                legendItemID = currentSource.ID;

                //Add a default value in cbox
                newDescList.Add(new ItemDescription { DescFalseID = Properties.Resources.Message_AddNewDescription, DescRealID = string.Empty });

                //Access ids in Legend_TreeTable
                string ltreeTableQuery = ltreeItemID + " = '" + legendItemID + "'";
                Dictionary<string, List<string>> legendDescriptionIDDico = GSC_ProjectEditor.Tables.GetUniqueFieldValues(ltreeTable, ltreeDescID, ltreeTableQuery, true, ltreeCGMID);
                List<string> legendDescriptionIDList = legendDescriptionIDDico[MainDico]; //Will be used to retrieve current item description
                List<string> legendDescriptionMapIDList = legendDescriptionIDDico[TagDico]; //Will be used to check current map associated with selected item

                if (legendDescriptionIDList.Count != 0)
                {
                    #region Existing descriptions
                    //Iterate through items
                    foreach (string descriptions in legendDescriptionIDList)
                    {
                        if (descriptions != "")
                        {
                            string falseID = descriptions;

                            string ldescTableQuery = lDescID + " = " + descriptions;
                            List<string> getDescription = GSC_ProjectEditor.Tables.GetFieldValues(lDescTable, lDescDescription, ldescTableQuery);

                            if (getDescription.Count != 0)
                            {
                                falseID = getDescription[0]; //TODO manage multiple ids here instead of only one
                            }

                            //Calculate false id
                            if (falseID.Length > 35)
                            {
                                falseID = falseID.Substring(0, 35) + "...";
                            }


                            //Add new value to description combobox
                            newDescList.Add(new ItemDescription { DescFalseID = falseID, DescRealID = descriptions });
                        }


                    }
                    #endregion

                }


                //Add list to control
                this.cbox_selectLegendDesc.DataSource = newDescList;
                this.cbox_selectLegendDesc.DisplayMember = "DescFalseID";
                this.cbox_selectLegendDesc.ValueMember = "DescRealID";
                this.cbox_selectLegendDesc.SelectedIndex = 0;

                //Fill the checkbox list
                fillMapList();

                //Update the description textbox
                updateDescription();

                #endregion
            }
        }

        private void cbox_selectDesc_SelectionChangeCommitted(object sender, EventArgs e)
        {
            //Get description id
            if (this.cbox_selectLegendDesc.Items.Count > 1)
            {
                ItemDescription currentDescription = this.cbox_selectLegendDesc.SelectedItem as ItemDescription; //Cast items from combobox

                if (currentDescription != null && currentDescription.DescRealID != string.Empty)
                {
                    fillMapList();
                    updateDescription();
                }

            }



        }

        #endregion

        #region Model

        /// <summary>
        ///will update map list and description textbox based on selection of an item
        /// </summary>
        public void updateDescription()
        {
            //Cast items from combobox
            LegendItems currentSource = this.cbox_selectLegendItem.SelectedItem as LegendItems;
            ItemDescription currentDescription = this.cbox_selectLegendDesc.SelectedItem as ItemDescription;

            if (this.cbox_selectLegendDesc.SelectedIndex != -1 && this.cbox_selectLegendItem.SelectedIndex != -1)
            {
                //Variables
                string legendItemID = currentSource.ID;
                string legendDescID = currentDescription.DescRealID;

                if (legendDescID != "")
                {
                    #region Fill information based on P_LEGEND_DESCRIPTION

                    //Access ids in Legend_TreeTable
                    string ldescTableQuery = lDescID + " = " + legendDescID;
                    List<string> getDescription = GSC_ProjectEditor.Tables.GetFieldValues(lDescTable, lDescDescription, ldescTableQuery);

                    if (getDescription.Count != 0)
                    {
                        string desc = getDescription[0]; //TODO manage multiple ids here instead of only one

                        //Apply value to textbox within interface
                        this.txtbox_Description.Text = desc;
                    }
                    else
                    {
                        this.txtbox_Description.Text = "";
                    }
                    #endregion

                    #region Fill information based on associated maps with current description

                    //Get infor from tree table
                    string ltreeQuery = lDescID + " = " + legendDescID;
                    List<string> legendDescriptionMapIDList = GSC_ProjectEditor.Tables.GetFieldValues(ltreeTable, ltreeCGMID, ltreeQuery);

                    //Iterate through all elements
                    foreach (string descMaps in legendDescriptionMapIDList)
                    {
                        if (this.checkedListBox_Maps.Items.Contains(descMaps))
                        {
                            //Check map
                            int currentIndex = this.checkedListBox_Maps.Items.IndexOf(descMaps);
                            this.checkedListBox_Maps.SetItemCheckState(currentIndex, CheckState.Checked);

                        }
                    }

                    #endregion

                }
                else
                {
                    //Reset components
                    //this.checkBox_AllMaps.Checked = true;
                    this.txtbox_Description.Text = "";
                }

            }
        }

        #endregion

    }
}
