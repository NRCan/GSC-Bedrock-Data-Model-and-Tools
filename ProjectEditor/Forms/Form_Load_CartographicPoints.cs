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
    public partial class Form_Load_CartographicPoints : Form
    {
        #region Main Variables

        //Public variables
        public string dataPath { get; set; }
        public bool isGIS { get; set; }
        public string dataExtension { get; set; }
        public List<string> dataFieldList { get; set; } //Alias will be used inside this list
        public ITable inputDataTable { get; set; }
        public ITable inMemoryDataTable { get; set; }
        public ITable outputInformationTable { get; set; }
        public ISpatialReference inputDataSpatialReference { get; set; } //If needed only

        //Extensions
        public string shpExt = ".shp";
        public string gdbExt = ".gdb";
        public string mdbExt = ".mdb";
        public string xlExt = ".xls";
        public string txtExt = ".txt";
        public string csvExt = ".csv";
        public string dbfExt = ".dbf";

        //Legend generator table
        private const string legendTable = GSC_ProjectEditor.Constants.Database.TLegendGene;
        private const string legendTableSymbol = GSC_ProjectEditor.Constants.DatabaseFields.LegendSymbol;
        private const string legendTableID = GSC_ProjectEditor.Constants.DatabaseFields.LegendLabelID;
        private const string legendTableSymType = GSC_ProjectEditor.Constants.DatabaseFields.LegendSymType;
        private const string legendTableName = GSC_ProjectEditor.Constants.DatabaseFields.LegendGISDisplay;
        private const string legendTableSymTypePoint = GSC_ProjectEditor.Constants.DatabaseDomainsValues.SymTypePoint;
        private const string legendTableSymTheme = GSC_ProjectEditor.Constants.DatabaseFields.LegendItemType;

        #endregion

        #region VIEW MODEL
        /// <summary>
        /// Intented to use with the theme combobox
        /// </summary>
        public class ThemeDisplay
        {
            public string themeDisplayName { get; set; }
            public string themeDisplayCode { get; set; }

        }

        /// <summary>
        /// Will be used to build and fill the carto point feature class
        /// </summary>
        public class FromToFieldMapping
        {
            public string FromField { get; set; }
            public string ToField { get; set; }
            public int FromFieldIndex { get; set; }
            public int ToFieldIndex { get; set; }
        }

        /// <summary>
        /// Will be used for the source combobox
        /// </summary>
        public class SourceDisplay
        {
            public string SourceName { get; set; }
            public string SourceCode { get; set; }
        }

        /// <summary>
        /// Will be used to create new geometry from tables
        /// </summary>
        public class CoordinatesFromTable
        {
            public double easting { get; set; }
            public double northing { get; set; }
            public double longitude { get; set; }
            public double latitude { get; set; }
            public double altitude { get; set; }
        }

        public Form_Load_CartographicPoints()
        {
            //Init culture
            Utilities.Culture.SetCulture();

            InitializeComponent();

            //Initiate some custom controls.
            this.Shown += new EventHandler(FormAddCartoPoint_Shown);

        }

        /// <summary>
        /// Whenever the form is shown, fill in some comboboxes and other controls initialization
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void FormAddCartoPoint_Shown(object sender, EventArgs e)
        {
            //Fill some variables
            InitFieldList();

            //Fill some controls
            FillThemeCombobox(string.Empty);
            FillSourceCombobox();
            FillFieldChecklist();
            FillGISTabComboboxes();
            FillCartoInfoTabComboboxes();

            //Manage GIS Tab
            EnableDisableGISTab();
        }

        /// <summary>
        /// Will fill all the comboboxes from the carto info tab with a list of fields coming
        /// from the data entry
        /// </summary>
        private void FillCartoInfoTabComboboxes()
        {
            //Check if a field list exists
            if (dataFieldList == null)
            {
                InitFieldList();
            }

            foreach (string items in dataFieldList)
            {
                this.comboBox_SymbolField.Items.Add(items);
                this.comboBox_AngleField.Items.Add(items);
                this.comboBox_ScaleField.Items.Add(items);
            }
            this.comboBox_SymbolField.SelectedIndex = -1;
            this.comboBox_AngleField.SelectedIndex = -1;
            this.comboBox_ScaleField.SelectedIndex = -1;
        }

        /// <summary>
        /// Will fill all the comboboxes from the GIS tab, if enable, with a list of fields coming 
        /// from the data entry.
        /// </summary>
        private void FillGISTabComboboxes()
        {
            if (!isGIS)
            {
                //Check if a field list exists
                if (dataFieldList == null)
                {
                    InitFieldList();
                }

                foreach (string items in dataFieldList)
                {
                    this.comboBox_XField.Items.Add(items);
                    this.comboBox_YField.Items.Add(items);
                    this.comboBox_ZField.Items.Add(items);
                    this.comboBox_longitude.Items.Add(items);
                    this.comboBox_latitude.Items.Add(items);
                }

                this.comboBox_XField.SelectedIndex = -1;
                this.comboBox_YField.SelectedIndex = -1;
                this.comboBox_ZField.SelectedIndex = -1;
                this.comboBox_longitude.SelectedIndex = -1;
                this.comboBox_latitude.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Will fill the field checklist box inside the Field tab, from the given data entry
        /// </summary>
        private void FillFieldChecklist()
        {
            //Check if a field list exists
            if (dataFieldList == null)
            {
                InitFieldList();
            }

            //If user wants to select certain fields
            if (this.checkBox_KeepAllFields.Checked == false && dataFieldList != null)
            {
                this.checkedListBox_Fields.Items.Clear();
                //Fill in the check list 
                foreach (string fieldNames in dataFieldList)
                {
                    if (fieldNames != string.Empty)
                    {
                        this.checkedListBox_Fields.Items.Add(fieldNames);
                    }
                    
                }
            }   
        }

        /// <summary>
        /// Will retrieve a list of field depending on the data extension type
        /// </summary>
        private void InitFieldList()
        {
            if (dataExtension == gdbExt || dataExtension == mdbExt )
            {
                //Open as table
                inputDataTable = GSC_ProjectEditor.Tables.OpenTableFromStringFaster(dataPath);


                //Fill the field list with field names
                dataFieldList = GSC_ProjectEditor.Tables.GetFieldList(inputDataTable, true);

            }
            else if (dataExtension == dbfExt || dataExtension == shpExt)
            {
                //Get the table name and extension only
                string fileNameOnly = System.IO.Path.GetFileName(dataPath);

                //Get the excel workspace factory
                IWorkspace dbfWorkspace = GSC_ProjectEditor.Workspace.AccessWorkspace(dataPath);
                inputDataTable = GSC_ProjectEditor.Tables.OpenTableFromWorkspace(dbfWorkspace, fileNameOnly);

                //Fill the field list with field names
                dataFieldList = GSC_ProjectEditor.Tables.GetFieldList(inputDataTable, true);

            }
            else if (dataExtension == txtExt || dataExtension == csvExt)
            {
                //Get the sheet name only
                string fileNameOnly = System.IO.Path.GetFileName(dataPath);

                //Get the excel workspace factory
                IWorkspace txtFileWorkspace = GSC_ProjectEditor.Workspace.AccessTextfileWorkspace(dataPath);
                inputDataTable = GSC_ProjectEditor.Tables.OpenTableFromWorkspace(txtFileWorkspace, fileNameOnly);

                //Fill the field list with field names
                dataFieldList = GSC_ProjectEditor.Tables.GetFieldList(inputDataTable, true);

            }
            else if (dataExtension.Contains(xlExt))
            {
                //Get the sheet name
                string[] splitedPath = dataPath.Split('\\');

                //Build path to the file itself without the sheet
                string dataPathFileOnly = string.Empty;
                
                foreach (string parts in splitedPath)
                {
                    if (parts != splitedPath[splitedPath.Length - 1])
                    {
                        if (dataPathFileOnly != string.Empty)
                        {
                            dataPathFileOnly = dataPathFileOnly + "\\" + parts;
                        }
                        else
                        {
                            dataPathFileOnly = parts;
                        }
                        
                    }
		            
                }
                
                //Get the sheet name only
                string fileSheetName = splitedPath[splitedPath.Length - 1];

                //Get the excel workspace factory
                IWorkspace excelWorkspace = GSC_ProjectEditor.Workspace.AccessExcelWorkspace(dataPathFileOnly);
                inputDataTable = GSC_ProjectEditor.Tables.OpenTableFromWorkspace(excelWorkspace, fileSheetName);

                //Fill the field list with field names
                dataFieldList = GSC_ProjectEditor.Tables.GetFieldList(inputDataTable, true);

                dataFieldList.Add(string.Empty);
            }

        }

        /// <summary>
        /// Will disable GIS tab if the data entry already has some geometry information inside it.
        /// </summary>
        private void EnableDisableGISTab()
        {
            if (isGIS)
            {
                ((Control)this.tabPage_GIS).Enabled = false;

                //Reset current select tab page
                this.tabControl_CartoPoint.SelectedTab = this.tabPage_CartoInfo;
            }
        }

        /// <summary>
        /// Will fill the source combobox with values from the project database
        /// </summary>
        private void FillSourceCombobox()
        {
            //Clear old list
            this.comboBox_Source.DataSource = null;

            //Access the domain list of values
            Dictionary<string, string> sourceList = GSC_ProjectEditor.Domains.GetDomDico(GSC_ProjectEditor.Constants.DatabaseDomains.Source, "Code");
            List<SourceDisplay> sourcesToDisplay = new List<SourceDisplay>();
            sourcesToDisplay.Add(new SourceDisplay{SourceCode = string.Empty, SourceName = string.Empty});//Add a default value
            
            //Build combobox
            foreach (KeyValuePair<string, string> sources in sourceList)
            {
                //Add
                sourcesToDisplay.Add(new SourceDisplay { SourceCode = sources.Key, SourceName = sources.Value});
            }
            
            this.comboBox_Source.DataSource = sourcesToDisplay;
            this.comboBox_Source.DisplayMember = "SourceName";
            this.comboBox_Source.ValueMember = "SourceCode";
            this.comboBox_Source.SelectedIndex = -1;
        }

        /// <summary>
        /// Will fill the thematic combobox from values contained in the associated domain.
        /// </summary>
        private void FillThemeCombobox(string themeCodeToSelect)
        {
            //Variables
            int addedThemeIndex = -1;

            //Clear old list
            this.comboBox_Theme.DataSource = null;
       
            //Variables
            List<ThemeDisplay> themes = new List<ThemeDisplay>();

            //Access the domain list of values
            Dictionary<string, string> themeDico = GSC_ProjectEditor.Domains.GetDomDico(GSC_ProjectEditor.Constants.DatabaseDomains.cartoTheme, "Description");
            foreach (KeyValuePair<string, string> themeKeyValues in themeDico)
            {
                themes.Add(new ThemeDisplay { themeDisplayName = themeKeyValues.Key, themeDisplayCode = themeKeyValues.Value });
                if (themeCodeToSelect == themeKeyValues.Value)
                {
                    addedThemeIndex = themes.Count - 1;
                }
            }
            this.comboBox_Theme.Tag = themeDico;
            this.comboBox_Theme.DataSource = themes;
            this.comboBox_Theme.DisplayMember = "themeDisplayName";
            this.comboBox_Theme.ValueMember = "themeDisplayCode";
            this.comboBox_Theme.SelectedIndex = addedThemeIndex;
            
        
        }

        /// <summary>
        /// This event will occur when user is ready to append new data
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_Load_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;

            //Check whether a theme was selected
            if (this.comboBox_Theme.SelectedIndex != -1)
            {
                #region PRE-PROCESSING

                //Get carto point feature class
                IFeatureClass cartoPointFC = GSC_ProjectEditor.FeatureClass.OpenFeatureClass(GSC_ProjectEditor.Constants.Database.FCartoPoint);

                // Ask for projection if needed
                ISpatialReference cartoPointSpatialReference = GSC_ProjectEditor.SpatialReferences.GetSpatialRef(cartoPointFC);
                if (!isGIS && cartoPointSpatialReference is IGeographicCoordinateSystem && this.comboBox_XField.SelectedIndex != -1 && this.comboBox_latitude.SelectedIndex == -1)
                {
                    //In this particular case, the output system is geographic but the user only has projected coordinates in a table.
                    inputDataSpatialReference = GSC_ProjectEditor.Dialog.GetProjectionPrompt(this.Handle.ToInt32());
                }

                //Do an internal copy of the table in order to edit fields 
                IDataset inDataset = inputDataTable as IDataset;
                string inDatasetName = inDataset.Name;
                IWorkspace inMemoryWorkspace = GSC_ProjectEditor.Workspace.CreateInMemoryWorkspace();
                GSC_ProjectEditor.Tables.CopyTableToWorkspace(inMemoryWorkspace, inputDataTable, inDatasetName);
                inMemoryDataTable = GSC_ProjectEditor.Tables.OpenTableFromWorkspace(inMemoryWorkspace, inDatasetName);

                //Release original table
                System.Runtime.InteropServices.Marshal.FinalReleaseComObject(inputDataTable);

                //Add point ID field for relation to CARTO_POINT
                int pointIDIndex = cartoPointFC.FindField(GSC_ProjectEditor.Constants.DatabaseFields.FCartoPointID);
                IField pointIDField = cartoPointFC.Fields.get_Field(pointIDIndex);
                IFieldEdit pointIDFieldEdit = pointIDField as IFieldEdit;
                pointIDFieldEdit.IsNullable_2 = true;
                try
                {
                    inMemoryDataTable.AddField(pointIDField);
                }
                catch (Exception)
                {
                    //Maybe it already contains the ID field.
                }
                
                //Fill in the point id with some new ids
                UpdatePointIDField(inMemoryDataTable);

                #endregion

                #region Update Legend Generator Table
                //Get theme
                ThemeDisplay selectedTheme = this.comboBox_Theme.SelectedItem as ThemeDisplay;

                //Get theme from dico
                Dictionary<string, string> legendThemeDico = GSC_ProjectEditor.Domains.GetDomDico(GSC_ProjectEditor.Constants.DatabaseDomains.legendSymbolTheme, "Code");
                string newCalculatedTheme = GSC_ProjectEditor.Constants.DatabaseDomainsValues.legendItemCartoPoint + "_" + selectedTheme.themeDisplayCode;

                //Variables
                string legendGUID = string.Empty;

                //Add row inside legend table if not already there
                if (!legendThemeDico.ContainsKey(newCalculatedTheme))
                {
                    legendGUID = AddToLegendTable(selectedTheme, newCalculatedTheme);
                }
                else
                {
                    //Get current value for legend item id
                    string themeQuery = legendTableSymTheme + " = '" + newCalculatedTheme + "'";
                    List<string> currentThemeGUIDs = GSC_ProjectEditor.Tables.GetFieldValues(legendTable, legendTableID, themeQuery);
                    try
                    {
                        legendGUID = currentThemeGUIDs[0];
                    }
                    catch (Exception)
                    {
                        legendGUID = Guid.NewGuid().ToString();
                    }
                    
                }

                #endregion

                #region Fill in other fields in CARTOGRAPHIC_POINT feature class

                UpdateCartoPointFC(cartoPointFC, legendGUID);


                #region Create the new table or append existing

                //Remove some fields if required by user
                if (this.checkBox_KeepAllFields.Checked == false)
                {
                    RemovingUnwantedFields();
                }

                //Build new name               
                string selectedThemeCode = selectedTheme.themeDisplayCode.ToUpper();

                string newCartoAttributeTableName = GSC_ProjectEditor.Constants.Database.TExtenAttrb + selectedThemeCode;

                //Detect if table already exists before creating it
                if (!GSC_ProjectEditor.Workspace.GetNameExists(esriDatasetType.esriDTTable, newCartoAttributeTableName))
                {
                    CreateInformationTable(newCartoAttributeTableName);
                }
                else
                {
                    //Get the current table
                    ITable cartoAttributeTable_raw = GSC_ProjectEditor.Tables.OpenTable(newCartoAttributeTableName);

                    //Append
                    GSC_ProjectEditor.GeoProcessing.AppendData(inMemoryDataTable, cartoAttributeTable_raw);
                }

                ObjectManagement.ReleaseObject(inMemoryDataTable);
                ObjectManagement.ReleaseObject(inMemoryWorkspace);

                #endregion

                #endregion

                #region ADD RELATIONSHIP

                //Add relationship to those tables
                string relationName = Constants.Database.rel_prefix_CartoPnt + newCartoAttributeTableName;
                IDataset cartoDataset = cartoPointFC as IDataset;
                IFeatureWorkspace featWorkspace = cartoDataset.Workspace as IFeatureWorkspace;
                IObjectClass originClass = cartoPointFC as IObjectClass;
                ITable cartoAttributeTable = GSC_ProjectEditor.Tables.OpenTable(newCartoAttributeTableName);
                IObjectClass destinationClass = (IObjectClass)cartoAttributeTable;

                IRelationshipClass cartoRelClass = featWorkspace.CreateRelationshipClass(relationName,
                    originClass, destinationClass,
                    GSC_ProjectEditor.Constants.DatabaseFields.FCartoPointID,
                    GSC_ProjectEditor.Constants.DatabaseFields.FCartoPointID,
                    esriRelCardinality.esriRelCardinalityOneToOne,
                    esriRelNotification.esriRelNotificationNone,
                    false, false, null,
                    Constants.DatabaseFields.FCartoPointID,
                    "",
                    Constants.DatabaseFields.FCartoPointID,
                    "");

                ObjectManagement.ReleaseObject(cartoRelClass);
                ObjectManagement.ReleaseObject(destinationClass);
                ObjectManagement.ReleaseObject(originClass);
                ObjectManagement.ReleaseObject(featWorkspace);
                ObjectManagement.ReleaseObject(cartoDataset);
                ObjectManagement.ReleaseObject(cartoAttributeTable);
                ObjectManagement.ReleaseObject(inputDataTable);

                #endregion
                //End message and close
                GSC_ProjectEditor.Messages.ShowEndOfProcess();

                this.Close();
            }
            else
            {
                MessageBox.Show(Properties.Resources.Warning_CartoAttribute, Properties.Resources.Warning_BasicTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            this.Cursor = Cursors.Default;
        }

        /// <summary>
        /// Will add a new row inside legend generator table and will return a calculated GUID for legend item id field 
        /// </summary>
        /// <param name="inTheme"></param>
        /// <param name="newTheme"></param>
        private string AddToLegendTable(ThemeDisplay inTheme, string newTheme)
        {
            string legendItemGUID = Guid.NewGuid().ToString();
            
            //Add new theme inside domain
            GSC_ProjectEditor.Domains.AddDomainValue(GSC_ProjectEditor.Constants.DatabaseDomains.legendSymbolTheme, newTheme, inTheme.themeDisplayName);

            Dictionary<string, object> newRowDico = new Dictionary<string, object>();
            newRowDico[legendTableID] = legendItemGUID;
            newRowDico[legendTableSymbol] = GSC_ProjectEditor.Constants.Styles.InvalidPoint_FGDC;
            newRowDico[legendTableName] = inTheme.themeDisplayName;
            newRowDico[legendTableSymType] = legendTableSymTypePoint;
            newRowDico[legendTableSymTheme] = newTheme;
            GSC_ProjectEditor.Tables.AddRowWithValues(legendTable, newRowDico);  

            return legendItemGUID;
        }

        /// <summary>
        /// Will close the current form when clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_Cancel_Click(object sender, EventArgs e)
        {
            //Close current form
            this.Close();
        }

        /// <summary>
        /// Whenever clicked, it'll pop a text box form in which the user can enter a new theme. Then it'll be aded to
        /// the current combobox and also inside the associated domain in the database
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_AddTheme_Click(object sender, EventArgs e)
        {
            //Ask user for a new input text
            string title = Properties.Resources.Message_AddThemeTitle;
            string label = Properties.Resources.Message_AddTheme;
            Icon currentIcon = this.Icon;

            string newTheme = GSC_ProjectEditor.Form_Generic.ShowGenericTextboxForm(title, label, currentIcon, string.Empty);
            string projectDBPath =  GSC_ProjectEditor.Properties.Settings.Default.PROJECT_WORKSPACE_PATH;
            IWorkspace projectWorkspace = GSC_ProjectEditor.Workspace.AccessWorkspace(projectDBPath);
            string newThemeCode = GSC_ProjectEditor.Workspace.GetValidDatasetName(projectWorkspace, newTheme);

            //Add to the domain value
            GSC_ProjectEditor.Domains.AddDomainValue(GSC_ProjectEditor.Constants.DatabaseDomains.cartoTheme, newThemeCode, newTheme);

            //Refill combobox
            FillThemeCombobox(newThemeCode);
        }

        /// <summary>
        /// Whenever the check box is checked or unchecked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBox_KeepAllFields_CheckedChanged(object sender, EventArgs e)
        {
            //If user wants to active the checklist box
            if (this.checkBox_KeepAllFields.Checked == false)
            {
                //Enable control
                this.checkedListBox_Fields.Enabled = true;

                //Fill the control
                FillFieldChecklist();
            }
            else
            {
                //Disable control
                this.checkedListBox_Fields.Enabled = false;
            }
        }
        #endregion

        #region MODEL

        /// <summary>
        /// Will create the new information table from a given name. 
        /// It will also add the pointID field mandatory for the relation
        /// between the feature class point and this new table.
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="cartoPointTable"></param>
        private void CreateInformationTable(string tableName)
        {
            //Do a copy of the table
            GSC_ProjectEditor.Tables.CopyTableToProjectWorkspace(inMemoryDataTable, tableName);

            //Get the table itself
            outputInformationTable = GSC_ProjectEditor.Tables.OpenTable(tableName);
        }

        /// <summary>
        /// Will removed user's unwanted field from table.
        /// </summary>
        private void RemovingUnwantedFields()
        {
            //Get all items from checklist
            CheckedListBox.CheckedItemCollection fieldCollection = this.checkedListBox_Fields.CheckedItems;

            //Iterate through field and delete unwanted ones
            List<IField> fieldsToDelete = new List<IField>();

            for (int i = 0; i < inMemoryDataTable.Fields.FieldCount; i++)
            {
                //Current field
                IField currentField = inMemoryDataTable.Fields.get_Field(i);
                string currentFieldAlias = currentField.AliasName;

                if (!fieldCollection.Contains(currentFieldAlias) && currentField.Name != GSC_ProjectEditor.Constants.DatabaseFields.FCartoPointID)
                {
                    fieldsToDelete.Add(currentField);
                }
            }

            foreach (IField fields in fieldsToDelete)
            {
                try
                {
                    inMemoryDataTable.DeleteField(fields);
                }
                catch (Exception)
                {

                }

            }
        }

        /// <summary>
        /// Will fill the point id field in input table. 
        /// The first ID will be based on Carto Point feature class count 
        /// </summary>
        /// <param name="inputDataTable"></param>
        private void UpdatePointIDField(ITable inputDataTable)
        {
            //Get a first count of the carto point
            int firstID = GSC_ProjectEditor.IDs.CalculateIDFromCount(GSC_ProjectEditor.Constants.Database.FCartoPoint, null);

            //Get a cursor to update the table
            IQueryFilter queryFilter = new QueryFilter();
            ICursor updateCursor = inputDataTable.Update(queryFilter, true);
            int pointIDFieldIndex = updateCursor.FindField(GSC_ProjectEditor.Constants.DatabaseFields.FCartoPointID);
            IRow updateRow = updateCursor.NextRow();
            while (updateRow != null)
            {
                //Update pointID
                updateRow.set_Value(pointIDFieldIndex, firstID);

                //Update id
                firstID++;
                updateCursor.UpdateRow(updateRow);

                updateRow = updateCursor.NextRow();
            }

            //Release com object
            GSC_ProjectEditor.ObjectManagement.ReleaseObject(updateCursor);
        }

        /// <summary>
        /// Will fill the carto point feature class with required information coming from the table 
        /// </summary>
        private void UpdateCartoPointFC(IFeatureClass cartoFC, string legendItemGUID)
        {
            #region Pre-processing
            //If input table already have geometry, build a dictionary of it
            Dictionary<string, IGeometry> geometryDico = new Dictionary<string, IGeometry>();
            List<string> geometryIDList = new List<string>();
            if (isGIS)
            {
                //IFeatureClass inTableFC = GSC_ProjectEditor.FeatureClass.OpenFeatureClassFromString(dataPath);
                IFeatureClass inTableFC = GSC_ProjectEditor.FeatureClass.OpenFeatureClassFromStringFaster(dataPath);
                ISchemaLock fcLock = (ISchemaLock)inTableFC;
                try
                {
                    //It might already locked for example a feature already in the edited database, so no point on doing it again.
                    fcLock.ChangeSchemaLock(esriSchemaLock.esriExclusiveSchemaLock);
                }
                catch (Exception)
                {

                }

                geometryDico = GSC_ProjectEditor.FeatureClass.GetGeometryDicoFromFC(inTableFC, null, inMemoryDataTable.OIDFieldName);
                fcLock.ChangeSchemaLock(esriSchemaLock.esriSharedSchemaLock);
                geometryIDList = geometryDico.Keys.ToList();

            }

            //Get field to update based on user choice and defaults
            List<FromToFieldMapping> updateFieldList = GetUpdateFieldList(cartoFC);

            ISpatialReference cartoPointSR = GSC_ProjectEditor.SpatialReferences.GetSpatialRef(cartoFC);

            #endregion

            #region Source ID field
            //From and to fields
            FromToFieldMapping sourceConfig = new FromToFieldMapping();
            sourceConfig.FromField = GSC_ProjectEditor.Constants.DatabaseFields.FCartoPointSourceID;

            //From and to indexes
            sourceConfig.FromFieldIndex = cartoFC.Fields.FindField(sourceConfig.FromField);

            #endregion

            #region OID Field
            //From and to fields
            FromToFieldMapping oidConfig = new FromToFieldMapping();
            oidConfig.FromField = inMemoryDataTable.OIDFieldName;
            oidConfig.ToField = GSC_ProjectEditor.Constants.DatabaseFields.ObjectID;

            //From and to indexes
            oidConfig.FromFieldIndex = inMemoryDataTable.FindField(oidConfig.FromField);
            oidConfig.ToFieldIndex = cartoFC.FindField(oidConfig.ToField);

            #endregion

            #region Theme Field
            //From and to fields
            FromToFieldMapping themeConfig = new FromToFieldMapping();
            themeConfig.FromField = GSC_ProjectEditor.Constants.DatabaseFields.FCartoPointTheme;

            //From and to indexes
            themeConfig.FromFieldIndex = cartoFC.Fields.FindField(themeConfig.FromField);

            #endregion

            #region LegendItemID Field
            //From and to fields
            FromToFieldMapping legendItemConfig = new FromToFieldMapping();
            legendItemConfig.FromField = GSC_ProjectEditor.Constants.DatabaseFields.FCartoPointLegendID;

            //From and to indexes
            legendItemConfig.FromFieldIndex = cartoFC.Fields.FindField(legendItemConfig.FromField);

            #endregion

            //Iterate through information table to retrieve wanted information
            IFeatureCursor insertCursor = cartoFC.Insert(true); //Init an insert cursor
            ICursor inTableReadCursor = inMemoryDataTable.Search(null, false);
            IRow inTableReadRow = inTableReadCursor.NextRow();
            while (inTableReadRow!=null)
            {
                CoordinatesFromTable inCoordinates = new CoordinatesFromTable();
                inCoordinates.altitude = double.NaN;
                inCoordinates.easting = double.NaN;
                inCoordinates.northing = double.NaN;
                inCoordinates.longitude = double.NaN;
                inCoordinates.latitude = double.NaN;
                
                //Add a new row inside point feature
                IFeatureBuffer newFeat = cartoFC.CreateFeatureBuffer();
                
                #region Iterate through update field list
                foreach (FromToFieldMapping fieldConfigs in updateFieldList)
                {
                    if (fieldConfigs != null)
                    {

                        //Read original value
                        var oriValue = inTableReadRow.get_Value(fieldConfigs.FromFieldIndex);

                        //Set
                        try
                        {
                            //If string, troncate if needed
                            if (newFeat.Fields.get_Field(fieldConfigs.ToFieldIndex).Type == esriFieldType.esriFieldTypeString)
                            {
                                if (newFeat.Fields.get_Field(fieldConfigs.ToFieldIndex).Length < oriValue.ToString().Length)
                                {
                                    oriValue = oriValue.ToString().Substring(0, newFeat.Fields.get_Field(fieldConfigs.ToFieldIndex).Length - 1);
                                }
                            }

                            #region Keep Coordinates
                            if (fieldConfigs.ToField == GSC_ProjectEditor.Constants.DatabaseFields.FCartoPointEasting)
	                        {
                                inCoordinates.easting = Convert.ToDouble(oriValue);
	                        }

                            if (fieldConfigs.ToField == GSC_ProjectEditor.Constants.DatabaseFields.FCartoPointNorthing)
	                        {
                                inCoordinates.northing = Convert.ToDouble(oriValue);
	                        }
                            if (fieldConfigs.ToField == GSC_ProjectEditor.Constants.DatabaseFields.FCartoPointAltitude)
	                        {
                                inCoordinates.altitude = Convert.ToDouble(oriValue);
	                        }
                            if (fieldConfigs.ToField == GSC_ProjectEditor.Constants.DatabaseFields.FCartoPointLongitude)
	                        {
                                inCoordinates.longitude = Convert.ToDouble(oriValue);
	                        }
                            if (fieldConfigs.ToField == GSC_ProjectEditor.Constants.DatabaseFields.FCartoPointLatitude)
	                        {
                                inCoordinates.latitude = Convert.ToDouble(oriValue);
	                        }

                            #endregion

                            newFeat.set_Value(fieldConfigs.ToFieldIndex, oriValue);
                        }
                        catch (Exception)
                        {
                            newFeat.set_Value(fieldConfigs.ToFieldIndex, DBNull.Value);
                        }
                    }

                }
                #endregion  

                #region Manage source id
                SourceDisplay sourceCode = this.comboBox_Source.SelectedItem as SourceDisplay;
                if (sourceCode != null && sourceCode.SourceName != string.Empty)
                {
                    //Set
                    newFeat.set_Value(sourceConfig.FromFieldIndex, sourceCode.SourceCode);
                }
                #endregion

                #region Manage Theme
                ThemeDisplay themeCode = this.comboBox_Theme.SelectedItem as ThemeDisplay;
                if (themeCode != null && themeCode.themeDisplayCode != string.Empty)
                {
                    //Set
                    newFeat.set_Value(themeConfig.FromFieldIndex, themeCode.themeDisplayCode);
                }
                #endregion

                #region Manage geometry
                if (isGIS)
                {
                    ////Access geometry in pre-build dictionnary
                    int currentOID = Convert.ToInt32(inTableReadRow.get_Value(oidConfig.FromFieldIndex));
                    string originalOID = geometryIDList[currentOID - 1];
                    IGeometry inputGeometry = geometryDico[originalOID];
                    GSC_ProjectEditor.Geometry.MakeZAware(inputGeometry);
                    GSC_ProjectEditor.Geometry.ApplyConstantZ(inputGeometry, 0.0);
                    newFeat.Shape = inputGeometry;

                    //Fill in the coordinate fields
                    newFeat = WriteCoordinatesFromGeometry(newFeat, inputGeometry);

                }
                else 
                {
                    //Create a new point geometry
                    IPoint newPoint = new PointClass();
                    newPoint.SpatialReference = cartoPointSR;
                    GSC_ProjectEditor.Geometry.MakeZAware(newPoint);
                    newPoint.X = 0.0;
                    newPoint.Y = 0.0;
                    newPoint.Z = 0.0;

                    newPoint = WriteCoordinatesFromTable(newFeat, newPoint, inCoordinates);
                    newFeat.Shape = newPoint;
                }


                #endregion

                #region Manage LegendItemID
                if (legendItemGUID != null && legendItemGUID != string.Empty)
                {
                    //Set
                    newFeat.set_Value(legendItemConfig.FromFieldIndex, legendItemGUID);
                }
                #endregion

                insertCursor.InsertFeature(newFeat);
                inTableReadRow = inTableReadCursor.NextRow();
            }

            //Release all com objects
            GSC_ProjectEditor.ObjectManagement.ReleaseObject(insertCursor);
            GSC_ProjectEditor.ObjectManagement.ReleaseObject(inTableReadCursor);
        }

        /// <summary>
        /// Will return a list of mapping for all needed fields to update inside carto point feature class
        /// </summary>
        /// <returns></returns>
        private List<FromToFieldMapping> GetUpdateFieldList(IFeatureClass cartoFC)
        {
            //Variables
            List<FromToFieldMapping> updateFieldList = new List<FromToFieldMapping>();

            //user's choice
            #region Angle Field
            if (this.comboBox_AngleField.SelectedIndex != -1 && this.comboBox_AngleField.SelectedItem.ToString() != string.Empty)
            {
                //From and to fields
                FromToFieldMapping angleConfig = new FromToFieldMapping();
                angleConfig.FromField = this.comboBox_AngleField.SelectedItem as string;
                angleConfig.ToField = GSC_ProjectEditor.Constants.DatabaseFields.FCartoPointAngle;

                //From and to indexes
                angleConfig.FromFieldIndex = inMemoryDataTable.Fields.FindFieldByAliasName(angleConfig.FromField);
                angleConfig.ToFieldIndex = cartoFC.FindField(angleConfig.ToField);

                updateFieldList.Add(angleConfig);

            }
            #endregion

            #region Scale Field
            if (this.comboBox_ScaleField.SelectedIndex != -1 && this.comboBox_ScaleField.SelectedItem.ToString() != string.Empty)
            {
                //From and to fields
                FromToFieldMapping scaleConfig = new FromToFieldMapping();
                scaleConfig.FromField = this.comboBox_ScaleField.SelectedItem as string;
                scaleConfig.ToField = GSC_ProjectEditor.Constants.DatabaseFields.FCartoPointDisplayFrom;

                //From and to indexes
                scaleConfig.FromFieldIndex = inMemoryDataTable.Fields.FindFieldByAliasName(scaleConfig.FromField);
                scaleConfig.ToFieldIndex = cartoFC.FindField(scaleConfig.ToField);

                updateFieldList.Add(scaleConfig);
            }
            #endregion

            #region Symbol Field
            if (this.comboBox_SymbolField.SelectedIndex != -1 && this.comboBox_SymbolField.SelectedItem.ToString() != string.Empty)
            {
                //From and to fields
                FromToFieldMapping symbolConfig = new FromToFieldMapping();
                symbolConfig.FromField = this.comboBox_SymbolField.SelectedItem as string;
                symbolConfig.ToField = GSC_ProjectEditor.Constants.DatabaseFields.FCartoPointSymbol;

                //From and to indexes
                symbolConfig.FromFieldIndex = inMemoryDataTable.Fields.FindFieldByAliasName(symbolConfig.FromField);
                symbolConfig.ToFieldIndex = cartoFC.FindField(symbolConfig.ToField);

                updateFieldList.Add(symbolConfig);
            }
            #endregion

            //defaults

            #region POINT ID Field
            //From and to fields
            FromToFieldMapping pointIDConfig = new FromToFieldMapping();
            pointIDConfig.FromField = GSC_ProjectEditor.Constants.DatabaseFields.FCartoPointID;
            pointIDConfig.ToField = GSC_ProjectEditor.Constants.DatabaseFields.FCartoPointID;

            //From and to indexes
            pointIDConfig.FromFieldIndex = inMemoryDataTable.FindField(pointIDConfig.FromField);
            pointIDConfig.ToFieldIndex = cartoFC.FindField(pointIDConfig.ToField);

            updateFieldList.Add(pointIDConfig);

            #endregion

            #region Easting/Northing/altitude
            if (!isGIS)
            {
                if (this.comboBox_XField.SelectedIndex != -1 && this.comboBox_XField.SelectedItem.ToString() != string.Empty)
                {
                    //From and to fields
                    FromToFieldMapping eastingConfig = new FromToFieldMapping();
                    eastingConfig.FromField = this.comboBox_XField.SelectedItem as string;
                    eastingConfig.ToField = GSC_ProjectEditor.Constants.DatabaseFields.FCartoPointEasting;

                    //From and to indexes
                    eastingConfig.FromFieldIndex = inMemoryDataTable.Fields.FindFieldByAliasName(eastingConfig.FromField);
                    eastingConfig.ToFieldIndex = cartoFC.FindField(eastingConfig.ToField);

                    updateFieldList.Add(eastingConfig);
                }

                if (this.comboBox_YField.SelectedIndex != -1 && this.comboBox_YField.SelectedItem.ToString() != string.Empty)
                {
                    //From and to fields
                    FromToFieldMapping northingConfig = new FromToFieldMapping();
                    northingConfig.FromField = this.comboBox_YField.SelectedItem as string;
                    northingConfig.ToField = GSC_ProjectEditor.Constants.DatabaseFields.FCartoPointNorthing;

                    //From and to indexes
                    northingConfig.FromFieldIndex = inMemoryDataTable.Fields.FindFieldByAliasName(northingConfig.FromField);
                    northingConfig.ToFieldIndex = cartoFC.FindField(northingConfig.ToField);

                    updateFieldList.Add(northingConfig);
                }

                if (this.comboBox_ZField.SelectedIndex != -1 && this.comboBox_ZField.SelectedItem.ToString() != string.Empty)
                {
                    //From and to fields
                    FromToFieldMapping altitudeConfig = new FromToFieldMapping();
                    altitudeConfig.FromField = this.comboBox_ZField.SelectedItem as string;
                    altitudeConfig.ToField = GSC_ProjectEditor.Constants.DatabaseFields.FCartoPointAltitude;

                    //From and to indexes
                    altitudeConfig.FromFieldIndex = inMemoryDataTable.Fields.FindFieldByAliasName(altitudeConfig.FromField);
                    altitudeConfig.ToFieldIndex = cartoFC.FindField(altitudeConfig.ToField);

                    updateFieldList.Add(altitudeConfig);
                }
            }


            #endregion

            #region Longitude/latitude
            if (!isGIS)
            {
                if (this.comboBox_longitude.SelectedIndex != -1 && this.comboBox_longitude.SelectedItem.ToString() != string.Empty)
                {
                    //From and to fields
                    FromToFieldMapping longConfig = new FromToFieldMapping();
                    longConfig.FromField = this.comboBox_longitude.SelectedItem as string;
                    longConfig.ToField = GSC_ProjectEditor.Constants.DatabaseFields.FCartoPointLongitude;

                    //From and to indexes
                    longConfig.FromFieldIndex = inMemoryDataTable.Fields.FindFieldByAliasName(longConfig.FromField);
                    longConfig.ToFieldIndex = cartoFC.FindField(longConfig.ToField);

                    updateFieldList.Add(longConfig);
                }

                if (this.comboBox_latitude.SelectedIndex != -1 && this.comboBox_latitude.SelectedItem.ToString() != string.Empty)
                {
                    //From and to fields
                    FromToFieldMapping latConfig = new FromToFieldMapping();
                    latConfig.FromField = this.comboBox_latitude.SelectedItem as string;
                    latConfig.ToField = GSC_ProjectEditor.Constants.DatabaseFields.FCartoPointLatitude;

                    //From and to indexes
                    latConfig.FromFieldIndex = inMemoryDataTable.Fields.FindFieldByAliasName(latConfig.FromField);
                    latConfig.ToFieldIndex = cartoFC.FindField(latConfig.ToField);

                    updateFieldList.Add(latConfig);
                }

            }

            #endregion

            return updateFieldList;
        }

        /// <summary>
        /// From a given geometry, will write the proper coordinates in the proper field in carto point feature
        /// </summary>
        /// <param name="inGeometry"></param>
        private IFeatureBuffer WriteCoordinatesFromGeometry(IFeatureBuffer cartoPointBuffer,  IGeometry inGeometry)
        {
            #region Pre-Processing
            //Get some indexes
            int eastingIndex = cartoPointBuffer.Fields.FindField(GSC_ProjectEditor.Constants.DatabaseFields.FCartoPointEasting);
            int northingIndex = cartoPointBuffer.Fields.FindField(GSC_ProjectEditor.Constants.DatabaseFields.FCartoPointNorthing);
            int altitudeIndex = cartoPointBuffer.Fields.FindField(GSC_ProjectEditor.Constants.DatabaseFields.FCartoPointAltitude);
            int longIndex = cartoPointBuffer.Fields.FindField(GSC_ProjectEditor.Constants.DatabaseFields.FCartoPointLongitude);
            int latIndex = cartoPointBuffer.Fields.FindField(GSC_ProjectEditor.Constants.DatabaseFields.FCartoPointLatitude);
            int datumZoneIndex = cartoPointBuffer.Fields.FindField(GSC_ProjectEditor.Constants.DatabaseFields.FCartoPointDatumZone);

            //Get original spatial reference system and output one
            ISpatialReference originalSpatialRef = inGeometry.SpatialReference;
            ISpatialReference cartoPointSpatialRef = cartoPointBuffer.Shape.SpatialReference;

            //Cast geometry to point shape
            IPoint pointGeometry = inGeometry as IPoint;

            #endregion

            #region Fill the fields from all case scenario

            if (originalSpatialRef is IGeographicCoordinateSystem && cartoPointSpatialRef is IProjectedCoordinateSystem)
            {
                FillGeographicFields(cartoPointBuffer, pointGeometry);
                IPoint getProjectedPoint = GeographicPointToProjected(pointGeometry, cartoPointSpatialRef);
                FillProjectedFields(cartoPointBuffer, getProjectedPoint);
            }

            if (originalSpatialRef is IGeographicCoordinateSystem && cartoPointSpatialRef is IGeographicCoordinateSystem)
            {
                FillGeographicFields(cartoPointBuffer, pointGeometry);
            }

            if (originalSpatialRef is IProjectedCoordinateSystem && cartoPointSpatialRef is IProjectedCoordinateSystem)
            {

                FillProjectedFields(cartoPointBuffer, pointGeometry);
                IPoint getGeoPoint = ProjectedPointToGeographic(pointGeometry, originalSpatialRef);
                FillGeographicFields(cartoPointBuffer, getGeoPoint);
            }

            if (originalSpatialRef is IProjectedCoordinateSystem && cartoPointSpatialRef is IGeographicCoordinateSystem)
            {
                FillProjectedFields(cartoPointBuffer, pointGeometry);
                IPoint getGeoPoint = ProjectedPointToGeographic(pointGeometry, originalSpatialRef);
                FillGeographicFields(cartoPointBuffer, getGeoPoint);
            }

            cartoPointBuffer.set_Value(altitudeIndex, pointGeometry.Z);

            #endregion

            return cartoPointBuffer;
        }

        /// <summary>
        /// From a given geometry, will write the proper coordinates in the proper field in carto point feature and also build the point object
        /// </summary>
        /// <param name="cartoPointBuffer"></param>
        /// <param name="inEmptyPoint"></param>
        /// <param name="inCoordinates"></param>
        /// <returns></returns>
        private IPoint WriteCoordinatesFromTable(IFeatureBuffer cartoPointBuffer, IPoint inEmptyPoint, CoordinatesFromTable inCoordinates)
        {
            #region Pre-Processing

            //Get original spatial reference system and output one
            ISpatialReference cartoPointSpatialRef = inEmptyPoint.SpatialReference;

            bool enteredCoordinatesAreGeo = false;
            bool enteredCoordinatesAreProj = false;

            #endregion

            #region Fill the fields from all case scenario

            try
            {
                //For geographic input
                if (!Double.IsNaN(inCoordinates.longitude) && !Double.IsNaN(inCoordinates.latitude))
                {
                    inEmptyPoint.X = inCoordinates.longitude;
                    inEmptyPoint.Y = inCoordinates.latitude;
                    enteredCoordinatesAreGeo = true;
                }

                if (!Double.IsNaN(inCoordinates.altitude))
                {
                    inEmptyPoint.Z = inCoordinates.altitude;
                }

                //For projected input
                if (!Double.IsNaN(inCoordinates.easting) && !Double.IsNaN(inCoordinates.northing))
                {
                    inEmptyPoint.X = inCoordinates.easting;
                    inEmptyPoint.Y = inCoordinates.northing;
                    enteredCoordinatesAreProj = true;
                }
            }
            catch (Exception)
            {
                //Default point in case.
                inEmptyPoint.X = 0.0;
                inEmptyPoint.Y = 0.0;
                inEmptyPoint.Z = 0.0;
            }

            #region For In-Out Geographic system
            if (cartoPointSpatialRef is IGeographicCoordinateSystem && enteredCoordinatesAreGeo)
            {
                FillGeographicFields(cartoPointBuffer, inEmptyPoint);
                IPoint getProjectedPoint = GeographicPointToProjected(inEmptyPoint, cartoPointSpatialRef);
                FillProjectedFields(cartoPointBuffer, getProjectedPoint);
            }
            #endregion

            #region For In-Out Geographic-Projected systems that has no projected coordinates of any sort from user
            if (cartoPointSpatialRef is IProjectedCoordinateSystem && enteredCoordinatesAreGeo && !enteredCoordinatesAreProj)
            {
                FillGeographicFields(cartoPointBuffer, inEmptyPoint);

                //Reset coordinate system to be geographic
                IProjectedCoordinateSystem cartoPointProjected = cartoPointSpatialRef as IProjectedCoordinateSystem;
                inEmptyPoint.SpatialReference = cartoPointProjected.GeographicCoordinateSystem;

                IPoint getProjectedPoint = GeographicPointToProjected(inEmptyPoint, cartoPointSpatialRef);
                FillProjectedFields(cartoPointBuffer, getProjectedPoint);

                //Reset geometry to fit output
                inEmptyPoint = getProjectedPoint;
            }
            #endregion

            #region For In-Out Projected System
            if (cartoPointSpatialRef is IProjectedCoordinateSystem && enteredCoordinatesAreProj)
            {
                FillProjectedFields(cartoPointBuffer, inEmptyPoint);
                IPoint getProjectedPoint = ProjectedPointToGeographic(inEmptyPoint, cartoPointSpatialRef);
                FillGeographicFields(cartoPointBuffer, getProjectedPoint);
            }

            #endregion

            #region For In-Out Projected-Geographic systems that has no geographic coordinates of any sort from user
            if (cartoPointSpatialRef is IGeographicCoordinateSystem && enteredCoordinatesAreProj && !enteredCoordinatesAreGeo)
            {
                if (inputDataSpatialReference != null)
                {
                    FillProjectedFields(cartoPointBuffer, inEmptyPoint);

                    //Reset coordinate system to be projected
                    inEmptyPoint.SpatialReference = inputDataSpatialReference;

                    IPoint getProjectedPoint = ProjectedPointToGeographic(inEmptyPoint, inputDataSpatialReference);
                    FillGeographicFields(cartoPointBuffer, getProjectedPoint);

                    //Reset geometry to fit output
                    inEmptyPoint = getProjectedPoint;  
                }

            }
            #endregion

            #endregion

            return inEmptyPoint;
        }

        /// <summary>
        /// Will set the projected coordinates fields inside a given buffer based from a given point
        /// </summary>
        /// <param name="inPrjBuffer">The buffer to write information in</param>
        /// <param name="projectedPoint">The point object to get the coordinate values from</param>
        /// <returns></returns>
        private IFeatureBuffer FillProjectedFields(IFeatureBuffer inPrjBuffer, IPoint projectedPoint)
        {
            try
            {
                //Get some indexes
                int eastingIndex = inPrjBuffer.Fields.FindField(GSC_ProjectEditor.Constants.DatabaseFields.FCartoPointEasting);
                int northingIndex = inPrjBuffer.Fields.FindField(GSC_ProjectEditor.Constants.DatabaseFields.FCartoPointNorthing);
                int datumZoneIndex = inPrjBuffer.Fields.FindField(GSC_ProjectEditor.Constants.DatabaseFields.FCartoPointDatumZone);

                //Set
                inPrjBuffer.set_Value(eastingIndex, projectedPoint.X);
                inPrjBuffer.set_Value(northingIndex, projectedPoint.Y);
            }
            catch (Exception)
            {

            }


            return inPrjBuffer;
        }

        /// <summary>
        /// Will set the geographic coordinates fields inside a given buffer based from a given point object.
        /// </summary>
        /// <param name="inGeoBuffer">The buffer to write information in.</param>
        /// <param name="geographicPoint">The point object to get the coordinates values from.</param>
        /// <returns></returns>
        private IFeatureBuffer FillGeographicFields(IFeatureBuffer inGeoBuffer, IPoint geographicPoint)
        {
            try
            {
                //Get some indexes
                int longIndex = inGeoBuffer.Fields.FindField(GSC_ProjectEditor.Constants.DatabaseFields.FCartoPointLongitude);
                int latIndex = inGeoBuffer.Fields.FindField(GSC_ProjectEditor.Constants.DatabaseFields.FCartoPointLatitude);

                //Set values
                inGeoBuffer.set_Value(longIndex, geographicPoint.X);
                inGeoBuffer.set_Value(latIndex, geographicPoint.Y);
            }
            catch (Exception)
            {

            }


            return inGeoBuffer;
        }

        /// <summary>
        /// Will project a point spatial reference system from geographic to a projected system
        /// </summary>
        /// <param name="inGeoPoint">The point to project</param>
        /// <param name="prjReference">The output wanted project reference</param>
        /// <returns></returns>
        private IPoint GeographicPointToProjected(IPoint inGeoPoint, ISpatialReference prjReference)
        {
            //Project the point to a new copy of it
            IPoint prjPointCopy = GSC_ProjectEditor.ObjectManagement.CopyInputObject(inGeoPoint) as IPoint;
            prjPointCopy.Project(prjReference);

            return prjPointCopy;
        }

        /// <summary>
        /// Will project a point spatial reference system from projected to it's inner geographic system.
        /// </summary>
        /// <param name="inProjectedPoint">The point to project</param>
        /// <param name="prjReference">The original spatial reference</param>
        /// <returns></returns>
        private IPoint ProjectedPointToGeographic(IPoint inProjectedPoint, ISpatialReference prjReference)
        {
            //Get geographic values
            IProjectedCoordinateSystem prjSystem = prjReference as IProjectedCoordinateSystem;
            IGeographicCoordinateSystem geoSystem = prjSystem.GeographicCoordinateSystem;

            //Project the point to a new copy of it
            IPoint geoPointCopy = GSC_ProjectEditor.ObjectManagement.CopyInputObject(inProjectedPoint) as IPoint;
            geoPointCopy.Project(geoSystem);

            return geoPointCopy;
        }

        #endregion
    }
}
