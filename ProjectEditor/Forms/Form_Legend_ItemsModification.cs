using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Web.Services;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.DataSourcesFile;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Desktop.AddIns;
using ESRI.ArcGIS.Editor;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.DisplayUI;
using ESRI.ArcGIS.SystemUI;

namespace GSC_ProjectEditor
{
    public partial class Form_Legend_ItemsModification : Form
    {
        #region Main Variables

        //P_LABELS
        private const string plabels = GSC_ProjectEditor.Constants.Database.FLabel;
        private const string plabelField = GSC_ProjectEditor.Constants.DatabaseFields.FLabelID;

        //DOMAINS
        private const string muPID = GSC_ProjectEditor.Constants.DatabaseDomains.MapUnit;
        private const string domGeolRank = GSC_ProjectEditor.Constants.DatabaseDomains.geolRank;
        private const string symTypeCodeFill = GSC_ProjectEditor.Constants.DatabaseDomainsValues.SymTypeFill;
        private const string symTypeCodeLine = GSC_ProjectEditor.Constants.DatabaseDomainsValues.SymTypeLine;
        private const string symTypeCodePoint = GSC_ProjectEditor.Constants.DatabaseDomainsValues.SymTypePoint;
        private string symTypeCodeHeader = GSC_ProjectEditor.Constants.DatabaseDomainsValues.SymTypeHeader1.Replace("1", "");
        private const string symTypeCodeHeader1 = GSC_ProjectEditor.Constants.DatabaseDomainsValues.SymTypeHeader1;
        private const string symTypeCodeHeader2 = GSC_ProjectEditor.Constants.DatabaseDomainsValues.SymTypeHeader2;
        private const string symTypeCodeHeader3 = GSC_ProjectEditor.Constants.DatabaseDomainsValues.SymTypeHeader3;

        //Legend generator table and fields
        private const string lTable = GSC_ProjectEditor.Constants.Database.TLegendGene;
        private const string lSymbol = GSC_ProjectEditor.Constants.DatabaseFields.LegendSymbol;
        private const string lSymType = GSC_ProjectEditor.Constants.DatabaseFields.LegendSymType;
        private const string lLabel = GSC_ProjectEditor.Constants.DatabaseFields.LegendLabelID;
        private const string lLabelName = GSC_ProjectEditor.Constants.DatabaseFields.LegendLabelName;
        private const string lLabelGISName = GSC_ProjectEditor.Constants.DatabaseFields.LegendGISDisplay;
        //private const string lDescription = GSC_ProjectEditor.Constants.DatabaseFields.LegendDesc;
        private const string lMapUnit = GSC_ProjectEditor.Constants.DatabaseFields.LegendMapUnit;
        private const string lAnnotation = GSC_ProjectEditor.Constants.DatabaseFields.LegendAnnotation;
        private const string lOrder = GSC_ProjectEditor.Constants.DatabaseFields.LegendOrder;
        private const string lLabelID = GSC_ProjectEditor.Constants.DatabaseFields.LegendLabelID;
        private const string lGeolRank = GSC_ProjectEditor.Constants.DatabaseFields.LegendGeolRank;

        //Legend_Treetable
        private const string ltreeTable = GSC_ProjectEditor.Constants.Database.TLegendTree;
        private const string ltreeItemID = GSC_ProjectEditor.Constants.DatabaseFields.LegendTreeItemID;
        private const string ltreeDescID = GSC_ProjectEditor.Constants.DatabaseFields.LegendTreeDescID;
        private const string ltreeCGMID = GSC_ProjectEditor.Constants.DatabaseFields.LegendTreeCGM;

        //Legend_description table
        private const string lDescTable = GSC_ProjectEditor.Constants.Database.TLegendDescription;
        private const string lDescID = GSC_ProjectEditor.Constants.DatabaseFields.LegendDescriptionID;
        private const string lDescDescription = GSC_ProjectEditor.Constants.DatabaseFields.LegendDescription;

        //Geoline symbols
        private const string geolineSymTable = GSC_ProjectEditor.Constants.Database.TGeolineSymbol;
        private const string geolineSymSelectCode = GSC_ProjectEditor.Constants.DatabaseFields.MGeolineSelectCode;
        private const string geolineSymID = GSC_ProjectEditor.Constants.DatabaseFields.MGeolineID;
        private const string geolineSymDesc = GSC_ProjectEditor.Constants.DatabaseFields.MGeolineLegendDescription;

        //Geopoint symbols
        private const string geopointSymTable = GSC_ProjectEditor.Constants.Database.TGeopointSymbol;
        private const string geopointSymSelectCode = GSC_ProjectEditor.Constants.DatabaseFields.TGeopointSelectCode;
        private const string geopointSymID = GSC_ProjectEditor.Constants.DatabaseFields.TGeopointID;
        private const string geopointSymDesc = GSC_ProjectEditor.Constants.DatabaseFields.TGeopointLegendDesc;

        //Geopoint feature
        private const string fGeoline = GSC_ProjectEditor.Constants.Database.FGeoline;
        private const string fGeolineSymbol = GSC_ProjectEditor.Constants.DatabaseFields.FGeolineFGDC;
        private const string fGeolineID = GSC_ProjectEditor.Constants.DatabaseFields.FGeolineID;

        //Geoline feature
        private const string fGeopoint = GSC_ProjectEditor.Constants.Database.FGeopoint;
        private const string fGeopointSymbol = GSC_ProjectEditor.Constants.DatabaseFields.FGeopointFGDC;
        private const string fGeopointID = GSC_ProjectEditor.Constants.DatabaseFields.FGeopointID;

        //Other
        private const string overprintKeyWord = GSC_ProjectEditor.Constants.ValueKeywords.labelOverprint;
        private Dictionary<string, object> inFieldValues = new Dictionary<string, object>(); //Will be used to add info in legend generator table
        private Dictionary<string, object> inFieldValuesLegendTree = new Dictionary<string, object>(); //Will be used to info in legend tree table
        private Dictionary<string, object> inFieldValuesLegendDesc = new Dictionary<string, object>(); //Will be used to add info in legend description table
        private const string symTypeDefaultValue = GSC_ProjectEditor.Constants.FieldDefaults.LegendSymbolType;
        private List<string> cboxLabelFillValues = new List<string>(); //Will be used to fill the main label combobox.
        private List<string> cboxLabelFillValuesTag = new List<string>(); //Will be used to fill the main label combobox tag with some domain codes
        public string MainDico = "Main"; //Keyword used to get a dictionnary main value list
        public string TagDico = "Tag"; //Keyword used to get a dictionnary tag value list
        public string selectedCode = GSC_ProjectEditor.Constants.DatabaseDomainsValues.BoolYes;
        public string selectedCodeNo = GSC_ProjectEditor.Constants.DatabaseDomainsValues.BoolNo;
        public string loaded = GSC_ProjectEditor.Constants.DatabaseDomainsValues.BoolYes;
        public string notLoaded = GSC_ProjectEditor.Constants.DatabaseDomainsValues.BoolNo;
        public string newkeyword = "new"; //Will be used to catch new header items added by user.
        public static Random newRandGenerator = new Random(); //Will be used to add some temporary legend order for new headers.
        public string itemStart = "    "; //Will be used to indent items in the legend item order listbox
        public string cartoPointFieldTheme = GSC_ProjectEditor.Constants.DatabaseDomainsValues.cartoPointThemeField;
        public string fieldItemsProject = "Current Project Field Stations";
        public string fieldItemsLegacy = "Legacy Stations from Cartographic Points";
        public bool init = false;

        #endregion

        #region PROPERTIES
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

        public int OverprintLevel { get; set; }
        #endregion

        public Form_Legend_ItemsModification()
        {
            //Set culture before init.
            Utilities.Culture.SetCulture();

            InitializeComponent();

            //Manage person list, if enabled is already open before init.
            if (this.Enabled)
            {
                FormLegendItems_EnabledChanged(this, null);
            }
            else if (!this.Enabled)
            {
                //Init controls if enabled.
                this.EnabledChanged += new EventHandler(FormLegendItems_EnabledChanged);
            }
        
        }

        #region FILL METHODS
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


        }

        /// <summary>
        /// Will fill the geological rank combobox with values coming from a given domain
        /// </summary>
        private void FillGeologicalRank()
        {
            List<string> geolRankList = GSC_ProjectEditor.Domains.GetDomValueList(domGeolRank);
            this.cbox_GeolRank.DataSource = geolRankList;
            this.cbox_GeolRank.SelectedIndex = -1;
        }

        /// <summary>
        /// Will fill the items combobox from a given selected type 
        /// </summary>
        /// <param name="selectedItemType"></param>
        private void FillItems(LegendItemsType selectedItemType)
        {
            //Reset combobox
            this.cbox_selectItem.DataSource = null;
            List<LegendItems> newItemList = new List<LegendItems>();

            //Fill value combobox with headers from table
            string itemTypeQuery = GSC_ProjectEditor.Constants.DatabaseFields.LegendItemType + " = '" + selectedItemType.ItemTypeID + "'";
            Dictionary<string, string> itemDico = GSC_ProjectEditor.Tables.GetUniqueDicoValues(lTable, lLabelGISName, lLabelID, itemTypeQuery);

            SortedDictionary<string, string> sortedItem = GSC_ProjectEditor.Dictionaries.GetSortedStringDico(itemDico);
            if (sortedItem.Count == 0)
            {
                newItemList.Add(new LegendItems { ItemName = Properties.Resources.Warning_NoLegendItem, ID = string.Empty });
            }
            else
            {
                foreach (KeyValuePair<string, string> heads in sortedItem)
                {
                    newItemList.Add(new LegendItems { ItemName = heads.Key, ID = heads.Value });

                }
            }

            this.cbox_selectItem.SelectedIndex = -1;
            this.cbox_selectItem.DataSource = newItemList;
            this.cbox_selectItem.DisplayMember = "ItemName";
            this.cbox_selectItem.ValueMember = "ID";
        }
        #endregion

        #region ADD/MODIFY/DELETE EVENTS
        /// <summary>
        /// When button is clicked, update legend generator table with new info
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_ModifyItem_Click(object sender, EventArgs e)
        {
            if (this.cbox_selectItem.SelectedIndex != -1)
            {
                //Variables
                LegendItems currentItem = this.cbox_selectItem.SelectedItem as LegendItems;//Cast items from combobox
                LegendItemsType currentItemType = this.cbox_SelectItemType.SelectedItem as LegendItemsType;
                bool didValidate = true;
                string errorMessage = "";
                inFieldValues.Clear();

                //Query
                string updateQuery = lLabelID + " = '" + currentItem.ID + "'";
                string fGeolineQuery = fGeolineID + " = '" + currentItem.ID + "'";
                string fGeopointQuery = fGeopointID + " = '" + currentItem.ID + "'";

                #region Validate parameters based on legend item types
                //Parse item type (labels, geoline, geopoints)
                if (currentItemType.ItemTypeID == GSC_ProjectEditor.Constants.DatabaseDomainsValues.legendItemMapUnit) //If user has selected map units, list all labels
                {
                    //Parse empty textbox that are needed
                    if (this.txtbox_MapUnitLegendSymbol.Text == "" || this.txtbox_MapUnitLegendSymbol.Text == " ")
                    {
                        errorMessage = Properties.Resources.Error_MapUnit;
                        didValidate = false;
                    }

                    //Set symbol type
                    inFieldValues[lSymType] = symTypeCodeFill;

                }
                else if (currentItemType.ItemTypeID == GSC_ProjectEditor.Constants.DatabaseDomainsValues.legendItemGeoline) //If user has selected geoline items
                {
                    //Set symbol type
                    inFieldValues[lSymType] = symTypeCodeLine;
                }
                else if (currentItemType.ItemTypeID == GSC_ProjectEditor.Constants.DatabaseDomainsValues.legendItemGeopoint) //If user has selected geopoints items
                {
                    //Set symbol type
                    inFieldValues[lSymType] = symTypeCodePoint;
                }
                else if (currentItemType.ItemTypeID == GSC_ProjectEditor.Constants.DatabaseDomainsValues.legendItemHeader) //If user has selected headers items
                {
                    //Set symbol type
                    if (radioButton_H1.Checked)
                    {
                        inFieldValues[lSymType] = symTypeCodeHeader1;
                    }
                    else if (radioButton_H2.Checked)
                    {
                        inFieldValues[lSymType] = symTypeCodeHeader2;
                    }
                    else if (radioButton_H3.Checked)
                    {
                        inFieldValues[lSymType] = symTypeCodeHeader3;
                    }


                }
                else if (currentItemType.ItemTypeID == GSC_ProjectEditor.Constants.DatabaseDomainsValues.legendItemField)
                {
                    //Set symbol type
                    inFieldValues[lSymType] = symTypeCodePoint;
                }
                else if (currentItemType.ItemTypeID == GSC_ProjectEditor.Constants.DatabaseDomainsValues.legendItemCartoPoint)
                {
                    //Set symbol type
                    inFieldValues[lSymType] = symTypeCodePoint;
                }
                else
                {
                    //Set symbol type
                    inFieldValues[lSymType] = symTypeCodePoint;
                }
                #endregion

                if (didValidate)
                {
                    #region Update P_LEGEND_GENERATOR table with user new values
                    //Empty dico
                    inFieldValuesLegendTree.Clear();
                    inFieldValuesLegendDesc.Clear();

                    //Check for overprints
                    string newLabelDesc = this.txtBox_NewLabel.Text;
                    if (this.checkBox_Overprint.Checked)
                    {
                        newLabelDesc = newLabelDesc + overprintKeyWord;

                        //Process overprint level
                        if (OverprintLevel > 1)
                        {
                            newLabelDesc = newLabelDesc + OverprintLevel.ToString();
                        }

                    }
                    else
                    {
                        newLabelDesc = newLabelDesc.Split(overprintKeyWord[0])[0]; //Strip it of any overprint related values.
                    }

                    //Add simple interface values
                    inFieldValues[lLabelName] = this.txtbox_MapUnitName.Text;
                    inFieldValues[lLabelGISName] = this.txtBox_ArcGISDisplay.Text;
                    inFieldValues[lSymbol] = this.txtbox_MapUnitLegendSymbol.Text;
                    inFieldValues[GSC_ProjectEditor.Constants.DatabaseFields.LegendItemType] = currentItemType.ItemTypeID;

  
                    //For map units only
                    if (currentItemType.ItemTypeID == GSC_ProjectEditor.Constants.DatabaseDomainsValues.legendItemMapUnit) //If user has selected map units, list all labels
                    {
                        if (this.textBox_mapUnits.Text == string.Empty)
                        {
                            inFieldValues[lMapUnit] = DBNull.Value;
                        }
                        else
                        {
                            inFieldValues[lMapUnit] = this.textBox_mapUnits.Text;
                        }

                        //Detect if map unit label has changed if yes, reset annotation text and do other update
                        if (this.txtBox_NewLabel.Text != currentItem.ItemName)
                        {
                            inFieldValues[lAnnotation] = DBNull.Value;
                            inFieldValues[lLabelGISName] = this.txtBox_NewLabel.Text;
                            //inFieldValues[lLabel] = this.txtBox_NewLabel.Text;
                        }
                        else
                        {
                            inFieldValues[lAnnotation] = this.txtbox_MapUnitAnno.Text;
                        }

                    }


                    if (this.cbox_GeolRank.SelectedIndex != -1)
                    {
                        //Get dico from eon domain
                        Dictionary<string, string> rankDico = GSC_ProjectEditor.Domains.GetDomDico(domGeolRank, "Description");

                        inFieldValues[lGeolRank] = rankDico[this.cbox_GeolRank.SelectedItem.ToString()];
                    }

                    //Update or add a new item in legend_generator table
                    if (this.cbox_selectItem.Tag.ToString() == loaded)
                    {
                        GSC_ProjectEditor.Tables.UpdateMultipleFieldValue(lTable, updateQuery, inFieldValues);
                    }
                    else
                    {
                        //Don't add any new header from this, instead use button within legend display tab.
                        if (this.cbox_selectItem.Text != Properties.Resources.Message_AddNewHeader)
                        {
                            if (currentItem.ID == string.Empty)
                            {
                                currentItem.ID = Guid.NewGuid().ToString();
                            }
                            inFieldValues[lLabel] = currentItem.ID;
                            GSC_ProjectEditor.Tables.AddRowWithValues(lTable, inFieldValues);
                        }

                    }


                    #endregion

                    #region Update MapUnit_PID domain, if necessary
                    if (this.txtBox_ArcGISDisplay.Text != "")
                    {
                        if (inFieldValues[lLabelGISName].ToString() != currentItem.ItemName &&
                            currentItemType.ItemTypeID == GSC_ProjectEditor.Constants.DatabaseDomainsValues.legendItemMapUnit)
                        {

                            //Change value within domain
                            GSC_ProjectEditor.Domains.UpdateDomainDescription(muPID, currentItem.ID, newLabelDesc);

                            //Refresh symbols
                            Utilities.ProjectSymbols uLabelStyle = new Utilities.ProjectSymbols();
                            uLabelStyle.RefreshLabelSymbols();

                            //Refresh map units
                            string btnMapU = ThisAddIn.IDs.Button_CreateEdit_CreateMapUnits;
                            Button_CreateEdit_CreateMapUnits buttonCreateMapUnit = AddIn.FromID<Button_CreateEdit_CreateMapUnits>(btnMapU);
                            buttonCreateMapUnit.RefreshMapUnitsSymbols();
                        }

                    }

                    #endregion

                    #region update feature classes if new symbols were added

                    if (currentItemType.ItemTypeID == GSC_ProjectEditor.Constants.DatabaseDomainsValues.legendItemGeoline) //If user has selecte geoline items
                    {

                        //Update symbol code within feature
                        GSC_ProjectEditor.Tables.UpdateFieldValue(fGeoline, fGeolineSymbol, fGeolineQuery, this.txtbox_MapUnitLegendSymbol.Text);

                        //Refresh symbols
                        Utilities.ProjectSymbols uSymbolPoints = new Utilities.ProjectSymbols();
                        uSymbolPoints.RefreshGeolineSymbols();

                    }
                    else if (currentItemType.ItemTypeID == GSC_ProjectEditor.Constants.DatabaseDomainsValues.legendItemGeopoint) //If user has selected geopoints items
                    {
                        //Update symbol code within feature
                        GSC_ProjectEditor.Tables.UpdateFieldValue(fGeopoint, fGeopointSymbol, fGeopointQuery, this.txtbox_MapUnitLegendSymbol.Text);

                        //Refresh symbols
                        Utilities.ProjectSymbols uSymbolPoints = new Utilities.ProjectSymbols();
                        uSymbolPoints.RefreshGeopointSymbols();

                    }


                    //Clear all values from interface
                    clearBoxes();

                }
                else
                {
                    MessageBox.Show(errorMessage);
                }

                    #endregion
            }
 


        }

        /// <summary>
        /// Will remove a legend item from the project
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_RemoveItem_Click(object sender, EventArgs e)
        {
            //Show a warning before going any further
            if (MessageBox.Show(Properties.Resources.Warning_RemoveItemFromProject, Properties.Resources.Warning_RemoveItemFromProjectTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                //Get current legend item type
                LegendItems currentItem = this.cbox_selectItem.SelectedItem as LegendItems;//Cast items from combobox
                string currentItemID = currentItem.ID;

                //Build the equivalent symbol value from id and symbol code to remove them from layer
                string symValue = this.txtbox_MapUnitLegendSymbol.Text + GSC_ProjectEditor.Constants.Symbol4Layers.fieldDelimeter + currentItemID;

                //Get item name in order to remove it from the templates
                string templateName = currentItem.ItemName;

                #region For Map units (labels)
                if (this.cbox_SelectItemType.SelectedIndex == 0)
                {
                    //Validate if any items are within label and or map units feature class
                    bool itemExists = ValidateExistingItem(plabels, currentItemID, plabelField);

                    if (!itemExists)
                    {
                        //Remove item from legend generator table
                        string queryToDelete = lLabelID + " = '" + currentItemID + "'";
                        GSC_ProjectEditor.Tables.DeleteFieldValue(lTable, queryToDelete);

                        //Remove item from Map unit domain
                        GSC_ProjectEditor.Domains.DeleteDomainValue(muPID, currentItemID);

                        //Remove the value from the layer, if needed
                        GSC_ProjectEditor.FeatureLayers.RemoveSymbolValueFromLayer(plabels, GSC_ProjectEditor.Properties.Resources.GroupLayerInterpretation, currentItemID, Utilities.MapDocumentFeatureLayers.GetCurrentMapObject());

                        //Remove from the template
                        Utilities.Templates.RemoveFromTemplate(templateName);

                    }
                    else
                    {
                        DisplayErrorMessageItemExist();
                    }
                }
                #endregion

                #region For geolines
                else if (this.cbox_SelectItemType.SelectedIndex == 1)
                {
                    //Validate if any items are within geoline feature class
                    bool itemExists = ValidateExistingItem(fGeoline, currentItemID, fGeolineID);

                    if (!itemExists)
                    {
                        //Update SYMBOL_GEOLINE table with required no value in select code field
                        string geolineQuery = lLabelID + " = '" + currentItemID + "'";
                        GSC_ProjectEditor.Tables.DeleteFieldValue(lTable, geolineQuery);

                        //Remove the value from the layer, if needed
                        GSC_ProjectEditor.FeatureLayers.RemoveSymbolValueFromLayer(fGeoline, GSC_ProjectEditor.Properties.Resources.GroupLayerInterpretation, symValue, Utilities.MapDocumentFeatureLayers.GetCurrentMapObject());

                        //Remove the value from the template too
                        Utilities.Templates.RemoveFromTemplate(templateName);

                    }
                    else
                    {
                        DisplayErrorMessageItemExist();
                    }
                }
                #endregion

                #region For Geopoints
                else if (this.cbox_SelectItemType.SelectedIndex == 2)
                {
                    //Validate if any items are within geopoint feature class
                    bool itemExists = ValidateExistingItem(fGeopoint, currentItemID, fGeopointID);

                    if (!itemExists)
                    {
                        //Update SYMBOL_GEOPOINT table with required no value in select code field
                        string geopointQuery = lLabelID + " = '" + currentItemID + "'";
                        GSC_ProjectEditor.Tables.DeleteFieldValue(lTable, geopointQuery);

                        //Remove the value from the layer, if needed
                        GSC_ProjectEditor.FeatureLayers.RemoveSymbolValueFromLayer(fGeopoint, GSC_ProjectEditor.Properties.Resources.GroupLayerInterpretation, symValue, Utilities.MapDocumentFeatureLayers.GetCurrentMapObject());

                        //Remove from the template
                        Utilities.Templates.RemoveFromTemplate(templateName);

                    }
                    else
                    {
                        DisplayErrorMessageItemExist();
                    }
                }
                #endregion

                #region For headers
                else if (this.cbox_SelectItemType.SelectedIndex == 3)
                {
                    ///This particular item doesn't need any validation because it must already exists within legend generator table

                    //Delete the given row from 
                    string queryToDelete = lLabelID + " = '" + currentItemID + "'";
                    GSC_ProjectEditor.Tables.DeleteFieldValue(lTable, queryToDelete);

                }
                #endregion

                //Reset interface 
                clearBoxes();

                //Refresh TOC
                ArcMap.Document.ActivatedView.ContentsChanged();
                ArcMap.Document.UpdateContents();
                ArcMap.Document.ActivatedView.Refresh();
            }
        }

        /// <summary>
        /// When clicked, a new pop up input text box form will ask user for a new theme value to be entered.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_AddItemType_Click(object sender, EventArgs e)
        {
            //Ask user for a new input text
            string title = Properties.Resources.Message_AddThemeTitle;
            string label = Properties.Resources.Message_AddTheme;
            Icon currentIcon = this.Icon;

            string newTheme = GSC_ProjectEditor.Form_Generic.ShowGenericTextboxForm(title, label, currentIcon, string.Empty);
            string projectDBPath = GSC_ProjectEditor.Properties.Settings.Default.PROJECT_DATABASE_PATH;
            IWorkspace projectWorkspace = GSC_ProjectEditor.Workspace.AccessWorkspace(projectDBPath);
            string newThemeCode = GSC_ProjectEditor.Workspace.GetValidDatasetName(projectWorkspace, newTheme);

            //Add to the domain value
            GSC_ProjectEditor.Domains.AddDomainValue(GSC_ProjectEditor.Constants.DatabaseDomains.legendSymbolTheme, newThemeCode, newTheme);

            //Refill combobox
            FillItemType();

            this.cbox_SelectItemType.SelectedIndex = this.cbox_SelectItemType.Items.Count - 1; //Select last item

            cbox_SelectItemType_SelectionChangeCommitted(this.cbox_SelectItemType, e);

        }
        #endregion

        #region OTHER EVENTS
        /// <summary>
        /// Enable event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void FormLegendItems_EnabledChanged(object sender, EventArgs e)
        {
            //Fill controls
            FillGeologicalRank();
            FillItemType();
            clearBoxes();
        }

        /// <summary>
        /// Show  all possible color codes from style file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pct_MapUnitColor_Click(object sender, EventArgs e)
        {
            string pathToStyle = GSC_ProjectEditor.Symbols.ValidateStyleFile2(ArcMap.Document);
            if (pathToStyle != string.Empty)
            {
                if (this.cbox_SelectItemType.SelectedIndex == 0) //If user has selected map units
                {

                    #region Process map units

                    IStyleGalleryStorage styleGal;
                    styleGal = ArcMap.Document.StyleGallery as IStyleGalleryStorage;

                    //Remove all styles except geoline one
                    List<string> userCurrentStyle = GSC_ProjectEditor.Symbols.RemoveAllStylesExceptGiven(styleGal, pathToStyle);

                    //Parse wanted type of symbols
                    ISimpleFillSymbol newFillSymbol = new SimpleFillSymbol();
                    ISymbol wantedSymbolType = newFillSymbol as ISymbol;

                    //Create a symbol selector 
                    ISymbolSelector newDialog = new SymbolSelector();

                    //Call the dialog with chosen symbol as parameter
                    if (newDialog.AddSymbol(wantedSymbolType))
                    {
                        //If user has selected a symbol
                        if (newDialog.SelectSymbol(0))
                        {
                            //Retreive user choice
                            ISymbol userSymbol = newDialog.GetSymbolAt(0);

                            //Find correct line symbol type
                            string symbolTypeName = "";
                            object correctLineSymbol = GSC_ProjectEditor.Symbols.GetPolygonSymbolType(userSymbol, out symbolTypeName);

                            if (correctLineSymbol != null)
                            {
                                this.txtbox_MapUnitLegendSymbol.Text = Utilities.MapDocumentSymbol.GetMatchingPolygonCodeFromSymbol(correctLineSymbol, symbolTypeName, pathToStyle);
                            }
                        }
                    }

                    //Add back all the styles from user into the style gallery storage
                    GSC_ProjectEditor.Symbols.AddStylesToStorage(styleGal, userCurrentStyle);

                    #endregion

                }
                else if (this.cbox_SelectItemType.SelectedIndex == 1) //If user has selecte geoline items, list all geolines
                {

                    #region Process geolines
                    IStyleGalleryStorage styleGal;
                    styleGal = ArcMap.Document.StyleGallery as IStyleGalleryStorage;

                    //Remove all styles except geoline one
                    List<string> userCurrentStyle = GSC_ProjectEditor.Symbols.RemoveAllStylesExceptGiven(styleGal, pathToStyle);

                    //Parse wanted type of symbols
                    ISimpleLineSymbol newFillSymbol = new SimpleLineSymbol();
                    ISymbol wantedSymbolType = newFillSymbol as ISymbol;

                    //Create a symbol selector 
                    ISymbolSelector newDialog = new SymbolSelector();

                    //Call the dialog with chosen symbol as parameter
                    if (newDialog.AddSymbol(wantedSymbolType))
                    {
                        //If user has selected a symbol
                        if (newDialog.SelectSymbol(0))
                        {

                            //Retreive user choice
                            ISymbol userSymbol = newDialog.GetSymbolAt(0);

                            //Find correct line symbol type
                            string symbolTypeName = "";
                            object correctLineSymbol = GSC_ProjectEditor.Symbols.GetLineSymbolType(userSymbol, out symbolTypeName);

                            if (correctLineSymbol != null)
                            {
                                this.txtbox_MapUnitLegendSymbol.Text = Utilities.MapDocumentSymbol.GetMatchingLineCodeFromSymbol(correctLineSymbol, symbolTypeName, pathToStyle);
                            }

                        }
                    }

                    //Add back all the styles from user into the style gallery storage
                    GSC_ProjectEditor.Symbols.AddStylesToStorage(styleGal, userCurrentStyle);

                    #endregion

                }
                else if (this.cbox_SelectItemType.SelectedIndex == 2 || this.cbox_SelectItemType.SelectedIndex >= 4) //If user has selected geopoints items, list all geopoints
                {

                    #region Process geopoints

                    IStyleGalleryStorage styleGal;
                    styleGal = ArcMap.Document.StyleGallery as IStyleGalleryStorage;

                    //Remove all styles except geoline one
                    List<string> userCurrentStyle = GSC_ProjectEditor.Symbols.RemoveAllStylesExceptGiven(styleGal, pathToStyle);


                    //Parse wanted type of symbols
                    ISimpleMarkerSymbol newFillSymbol = new SimpleMarkerSymbol();
                    ISymbol wantedSymbolType = newFillSymbol as ISymbol;

                    //Create a symbol selector 
                    ISymbolSelector newDialog = new SymbolSelector();

                    //Call the dialog with chosen symbol as parameter
                    if (newDialog.AddSymbol(wantedSymbolType))
                    {
                        //If user has selected a symbol
                        if (newDialog.SelectSymbol(0))
                        {

                            //Retreive user choice
                            ISymbol userSymbol = newDialog.GetSymbolAt(0);

                            //Find correct line symbol type
                            string symbolTypeName = "";
                            object correctLineSymbol = GSC_ProjectEditor.Symbols.GetPointSymbolType(userSymbol, out symbolTypeName);

                            if (correctLineSymbol != null)
                            {
                                this.txtbox_MapUnitLegendSymbol.Text = Utilities.MapDocumentSymbol.GetMatchingPointCodeFromSymbol(correctLineSymbol, symbolTypeName, pathToStyle);
                            }

                        }
                    }

                    //Add back all the styles from user into the style gallery storage
                    GSC_ProjectEditor.Symbols.AddStylesToStorage(styleGal, userCurrentStyle);
  
                    #endregion
                }
            }
        }

        /// <summary>
        /// Use to clear all values from dw controls.
        /// </summary>
        private void clearBoxes()
        {
            //Clear textboxes
            this.txtbox_MapUnitAnno.Text = string.Empty;
            this.txtbox_MapUnitLegendSymbol.Text = string.Empty;
            this.txtbox_MapUnitName.Text = string.Empty;
            this.txtBox_NewLabel.Text = string.Empty;
            this.txtBox_ArcGISDisplay.Text = string.Empty;
            this.textBox_mapUnits.Text = string.Empty;

            //Clear comboboxes
            this.cbox_GeolRank.SelectedIndex = -1;
            this.cbox_selectItem.DataSource = null;
            this.cbox_selectItem.Tag = string.Empty;
            this.cbox_SelectItemType.SelectedIndex = -1;
        }

        /// <summary>
        /// Click event to clear all values from dw controls
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_MapUnitClearBoxes_Click(object sender, EventArgs e)
        {
            clearBoxes();
        }

        /// <summary>
        /// This will be triggered when a user changes a label within the combobox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbox_selectItem_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Cast items from combobox
            LegendItems currentSource = this.cbox_selectItem.SelectedItem as LegendItems;

            if (this.cbox_selectItem.SelectedIndex != -1)
            {
                //Variables
                string legendItemID = "";
                init = true;

                #region P_LEGEND_GENERATOR validation
                //Get current selected item id
                legendItemID = currentSource.ID;

                //Check if any items have already been loaded in P_legend_generator
                string legendItemIDQuery = lLabelID + " = '" + legendItemID + "'";
                int itemCount = GSC_ProjectEditor.Tables.GetRowCount(lTable, legendItemIDQuery);

                #endregion

                if (itemCount > 0)
                {
                    #region Item type specific code
                    //Parse item type (labels, geoline, geopoints)
                    if (this.cbox_SelectItemType.SelectedIndex == 0) //If user has selected map units, list all labels
                    {
                        //Fill information based on MapUnit_PID domain
                        this.txtBox_NewLabel.Text = currentSource.ItemName;

                        //Fill overprint checkbox 
                        Dictionary<string, string> mapUnitDomainValues = GSC_ProjectEditor.Domains.GetDomDico(muPID, "Code");
                        if (mapUnitDomainValues.ContainsKey(currentSource.ID))
                        {
                            if (mapUnitDomainValues[currentSource.ID].Contains(overprintKeyWord))
                            {
                                this.checkBox_Overprint.Checked = true;
                            }

                        }
                    }
                    else if (this.cbox_SelectItemType.SelectedIndex == 1) //If user has selecte geoline items, list all geolines
                    {

                    }
                    else if (this.cbox_SelectItemType.SelectedIndex == 2) //If user has selected geopoints items, list all geopoints
                    {
                    }
                    else if (this.cbox_SelectItemType.SelectedIndex == 3) //If user has selected header items, list all headers
                    {

                    }
                    #endregion

                    //Fill the control tag with bool value
                    this.cbox_selectItem.Tag = loaded;

                    #region Fill information based on P_LEGEND_GENERATOR

                    //Build a query and get a proper cursor to read within legend generator table
                    ICursor legendGeneratorCursor = GSC_ProjectEditor.Tables.GetTableCursor("Search", legendItemIDQuery, lTable);

                    //Iterate trough selection to get wanted information
                    IRow legendRow = null;
                    while ((legendRow = legendGeneratorCursor.NextRow()) != null)
                    {
                        //Get some field indexes within legend generator
                        int legendNameIndex = legendRow.Fields.FindField(lLabelName);
                        int legendGISNameIndex = legendRow.Fields.FindField(lLabelGISName);
                        int legendGeolRankIndex = legendRow.Fields.FindField(lGeolRank);
                        int legendOrderIndex = legendRow.Fields.FindField(lOrder);
                        int legendSymbolIndex = legendRow.Fields.FindField(lSymbol);
                        int legendCodedMapUnitIndex = legendRow.Fields.FindField(lMapUnit);
                        int legendAnnotationIndex = legendRow.Fields.FindField(lAnnotation);
                        int legendSymTypeIndex = legendRow.Fields.FindField(lSymType);

                        //Get information related to those fields
                        string legendName = legendRow.get_Value(legendNameIndex).ToString();
                        string legendGISName = legendRow.get_Value(legendGISNameIndex).ToString();
                        string legendGeolRank = legendRow.get_Value(legendGeolRankIndex).ToString();
                        string legendOrder = legendRow.get_Value(legendOrderIndex).ToString();
                        string legendSymbol = legendRow.get_Value(legendSymbolIndex).ToString();
                        string legendMapUnit = legendRow.get_Value(legendCodedMapUnitIndex).ToString();
                        string legendAnno = legendRow.get_Value(legendAnnotationIndex).ToString();
                        string legendSymType = legendRow.get_Value(legendSymTypeIndex).ToString();

                        //Fill all the form with attributes from selected label
                        this.txtbox_MapUnitName.Text = legendName;
                        this.txtbox_MapUnitAnno.Text = legendAnno;
                        this.txtBox_ArcGISDisplay.Text = legendGISName;
                        this.textBox_mapUnits.Text = legendMapUnit;

                        //Manage header types
                        if (legendSymType.Contains(symTypeCodeHeader))
                        {
                            if (legendSymType == symTypeCodeHeader1)
                            {
                                this.radioButton_H1.Checked = true;
                            }
                            else if (legendSymType == symTypeCodeHeader2)
                            {
                                this.radioButton_H2.Checked = true;
                            }
                            else if (legendSymType == symTypeCodeHeader3)
                            {
                                this.radioButton_H3.Checked = true;
                            }
                        }

                        if ((legendSymbol == "" || legendSymbol == DBNull.Value.ToString() || legendSymbol == " ") && this.cbox_SelectItemType.SelectedIndex == 0)
                        {
                            this.txtbox_MapUnitLegendSymbol.Text = "";
                        }
                        else
                        {
                            this.txtbox_MapUnitLegendSymbol.Text = legendSymbol;
                        }


                        try
                        {
                            //Get dico from geol rank domain
                            Dictionary<string, string> selectedRank = GSC_ProjectEditor.Domains.GetDomDico(domGeolRank, "Code");
                            this.cbox_GeolRank.SelectedItem = selectedRank[legendGeolRank];
                        }
                        catch
                        {
                            this.cbox_GeolRank.SelectedIndex = -1;
                        }


                    }

                    #endregion

                    #region Fill information based on P_LEGEND_DESCRIPTION

                    ////Access ids in Legend_TreeTable
                    //string ltreeTableQuery = ltreeItemID + " = '" + legendItemID + "'";
                    //List<string> legendDescriptionIDList = GSC_ProjectEditor.Tables.GetUniqueFieldValues(ltreeTable, ltreeDescID, ltreeTableQuery, false, null)[MainDico];

                    //if (legendDescriptionIDList.Count != 0)
                    //{
                    //    string getDescriptionID = legendDescriptionIDList[0]; //TODO manage multiple ids here instead of only one

                    //    //Access description from legend_description table
                    //    string lDescriptionQuery = lDescID + " = " + getDescriptionID + "";
                    //    List<string> legendDescriptionItemList = GSC_ProjectEditor.Tables.GetUniqueFieldValues(lDescTable, lDescDescription, lDescriptionQuery, false, null)[MainDico];
                    //    string getDescription = legendDescriptionItemList[0]; //TODO manage multiple ids here instead of only one

                    //    //Apply value to textbox within interface
                    //    this.txtbox_Description.Text = getDescription;
                    //}
                    //else
                    //{
                    //    this.txtbox_Description.Text = "";
                    //}

                    #endregion

                }
                else
                {
                    //Fill the control tag with bool value
                    this.cbox_selectItem.Tag = notLoaded;
                }

            }

        }
        /// <summary>
        /// Will return a bool value whether an existing item is within a given table
        /// </summary>
        /// <param name="tableToVerify">The table to search for a value</param>
        /// <returns></returns>
        public bool ValidateExistingItem(string tableToVerify, string valueToVerify, string fieldToVerify)
        {
            //Variables
            bool itemExists = true;

            //Build query
            string query = fieldToVerify + " = '" + valueToVerify + "'";

            //Get a count from given query with given layer
            int rowCount = GSC_ProjectEditor.Tables.GetRowCount(tableToVerify, query);

            //Validate if any values are returned
            if (rowCount == 0)
            {
                itemExists = false;
            }

            return itemExists;
        }

        /// <summary>
        /// Will display to the user an error message in case any feature exists and he's trying to delete them
        /// </summary>
        public void DisplayErrorMessageItemExist()
        {
            MessageBox.Show(Properties.Resources.Error_LegendItemExists, Properties.Resources.Error_LegendItemExistsTitle, MessageBoxButtons.OK, MessageBoxIcon.Stop);
        }
        /// <summary>
        /// Based on user selection, fill the item combobox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbox_SelectItemType_SelectionChangeCommitted(object sender, EventArgs e)
        {
            //Cast current combobox
            System.Windows.Forms.ComboBox itemCbox = sender as System.Windows.Forms.ComboBox;

            #region process different item types

            //If something really changed
            if (itemCbox.SelectedIndex != -1)
            {
                //Cast selected items
                LegendItemsType currentType = this.cbox_SelectItemType.SelectedItem as LegendItemsType;

                //Fill
                FillItems(currentType);

                //If user has selected geopoint show default value for name
                if (currentType.ItemTypeID == GSC_ProjectEditor.Constants.DatabaseDomainsValues.legendItemGeopoint)
                {
                    //Add default or geological name
                    this.txtbox_MapUnitName.Text = GSC_ProjectEditor.Constants.FieldDefaults.NotAvailable;
                }

            }
            #endregion
        }

        /// <summary>
        /// Validate number of characters, to limit with the field 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtbox_MapUnitLegendSymbol_TextChanged(object sender, EventArgs e)
        {
            if (this.txtbox_MapUnitLegendSymbol.Text.Count() > 15)
            {
                GSC_ProjectEditor.Messages.ShowGenericErrorMessage(Properties.Resources.Error_TooManyCharacters + " 15.");
                this.txtbox_MapUnitLegendSymbol.Text = string.Empty;
            }
        }

        /// <summary>
        /// Whenever the overprint is checked get a level of overprint from user
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBox_Overprint_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkBox_Overprint.Checked && init)
            {
                Form_CreateEdit_CreateMapUnits_OverprintHierarchy newOverprintForm = new Form_CreateEdit_CreateMapUnits_OverprintHierarchy();

                //Show the form
                using (newOverprintForm)
                {
                    var formOutput = newOverprintForm.ShowDialog();
                    if (formOutput == DialogResult.OK)
                    {
                        OverprintLevel = newOverprintForm.returnLevel;
                    }

                }

            }
        }

        /// <summary>
        /// Will lock color selector if it's not map unit that user as selected.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbox_SelectItemType_SelectedIndexChanged(object sender, EventArgs e)
        {
            System.Windows.Forms.ComboBox senderBox = sender as System.Windows.Forms.ComboBox;
            if (senderBox.SelectedIndex != -1)
            {
                LegendItemsType selectedItem = senderBox.SelectedItem as LegendItemsType;
                if (selectedItem.ItemTypeID == GSC_ProjectEditor.Constants.DatabaseDomainsValues.legendItemGeoline || selectedItem.ItemTypeID == GSC_ProjectEditor.Constants.DatabaseDomainsValues.legendItemGeopoint || selectedItem.ItemTypeID == GSC_ProjectEditor.Constants.DatabaseDomainsValues.legendItemHeader)
                {
                    this.groupBox_Symbol.Enabled = false;
                }
                else
                {
                    this.groupBox_Symbol.Enabled = true;
                }

                if (selectedItem.ItemTypeID != GSC_ProjectEditor.Constants.DatabaseDomainsValues.legendItemHeader)
                {
                    this.groupBox_header.Enabled = false;
                }
                else
                {
                    this.groupBox_header.Enabled = true;
                }
            }
            else
            {
                this.groupBox_Symbol.Enabled = false;
            }
            

        }
        #endregion


    }
}
