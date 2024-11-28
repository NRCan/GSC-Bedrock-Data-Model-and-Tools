using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.ArcMapUI;

namespace GSC_ProjectEditor
{
    public partial class Form_Legend_CreateTempTableLegendGenerator : Form
    {

        #region Main Variables

        //LEGEND TABLE
        private const string tLegend = GSC_ProjectEditor.Constants.Database.TLegendGene;
        private const string tLegendSymbol = GSC_ProjectEditor.Constants.DatabaseFields.LegendSymbol;
        private const string tLegendItemID = GSC_ProjectEditor.Constants.DatabaseFields.LegendLabelID;
        private const string tLegendName = GSC_ProjectEditor.Constants.DatabaseFields.LegendGISDisplay;
        private const string tLegendSymType = GSC_ProjectEditor.Constants.DatabaseFields.LegendSymType;
        private const string tLegendSymTypeFillValue = GSC_ProjectEditor.Constants.DatabaseDomainsValues.SymTypeFill;
        private const string tLegendSymTypeHeaderValue1 = GSC_ProjectEditor.Constants.DatabaseDomainsValues.SymTypeHeader1;
        private const string tLegendSymTypeHeaderValue2 = GSC_ProjectEditor.Constants.DatabaseDomainsValues.SymTypeHeader2;
        private const string tLegendSymTypeHeaderValue3 = GSC_ProjectEditor.Constants.DatabaseDomainsValues.SymTypeHeader3;
        private const string tLegendSymOrder = GSC_ProjectEditor.Constants.DatabaseFields.LegendOrder;
        private const string tLegendItemType = GSC_ProjectEditor.Constants.DatabaseFields.LegendItemType;
        private const string tLegendItemTypeGeopoint = GSC_ProjectEditor.Constants.DatabaseDomainsValues.legendItemGeopoint;
        private const string tLegendItemTypeGeoline = GSC_ProjectEditor.Constants.DatabaseDomainsValues.legendItemGeoline;
        private const string tLegendItemTypeHeader = GSC_ProjectEditor.Constants.DatabaseDomainsValues.legendItemHeader;
        private const string tLegendItemTypeMapUnit = GSC_ProjectEditor.Constants.DatabaseDomainsValues.legendItemMapUnit;

        //LEGEND TREE TABLE
        private const string tTreeTable = GSC_ProjectEditor.Constants.Database.TLegendTree;
        private const string tTreeTableItemID = GSC_ProjectEditor.Constants.DatabaseFields.LegendTreeItemID;
        private const string tTreeTableDescID = GSC_ProjectEditor.Constants.DatabaseFields.LegendTreeDescID;
        private const string tTreeTableCGMID = GSC_ProjectEditor.Constants.DatabaseFields.LegendTreeCGM;

        //LEGEND DESCRIPTION TABLE
        private const string tLegendDesc = GSC_ProjectEditor.Constants.Database.TLegendDescription;
        private const string tLegendDescID = GSC_ProjectEditor.Constants.DatabaseFields.LegendDescriptionID;
        private const string tLegendDescriptionField = GSC_ProjectEditor.Constants.DatabaseFields.LegendDescription;

        //Feature geoline
        private const string fcGeoline = GSC_ProjectEditor.Constants.Database.FGeoline;
        private const string fcGeolineID = GSC_ProjectEditor.Constants.DatabaseFields.FGeolineID;

        //Feature geopoint
        private const string fcGeopoint = GSC_ProjectEditor.Constants.Database.FGeopoint;
        private const string fcGeopointID = GSC_ProjectEditor.Constants.DatabaseFields.FGeopointID;

        //Feature geopolys
        public const string fcGeopolys = GSC_ProjectEditor.Constants.Database.FGeopoly;
        public const string fcGeopolysID = GSC_ProjectEditor.Constants.DatabaseFields.FGeopolyLabel;

        //Feature CGM_MAPINDEX
        public const string cgmFeature = GSC_ProjectEditor.Constants.Database.FCGMIndex;
        public const string cgmFeatureFieldID = GSC_ProjectEditor.Constants.DatabaseFields.FCGM_ID;

        //Join Field names
        //public string joinGeopolyLabel = geopoly + "." + geopolyLabel;
        //public string joinTLegendSymbol = tLegend + "." + tLegendSymbol;

        //Domain
        public const string booleanDomain = GSC_ProjectEditor.Constants.DatabaseDomains.BoolYesNo;
        public const string booleanDomainTrue = GSC_ProjectEditor.Constants.DatabaseDomainsValues.BoolYes;
        public const string booleanDomainFalse = GSC_ProjectEditor.Constants.DatabaseDomainsValues.BoolNo;

        //Other
        private const string tLegendTemp = GSC_ProjectEditor.Constants.Database.tLegendGeneratorTemp;
        public bool cantDeleteTemp = false;
        public bool cantCopyTemp = false;
        public string MainDico = Constants.ValueKeywords.GetUniqueFieldValuesMain; //Keyword used to get a dictionnary main value list
        public string defaultLegend = Constants.ValueKeywords.FullProjectLegendSuffix;
        public string isInsideField = GSC_ProjectEditor.Constants.DatabaseFields.LegendTempIsVisible;
        public string isInsideFieldAlias = GSC_ProjectEditor.Constants.DatabaseFields.LegendTempIsVisibleAlias;
        public string headerKeyword = GSC_ProjectEditor.Constants.DatabaseDomainsValues.SymTypeHeader1.Substring(0, 1);
        public const string lineType = GSC_ProjectEditor.Constants.DatabaseDomainsValues.SymTypeLine;
        public const string markerType = GSC_ProjectEditor.Constants.DatabaseDomainsValues.SymTypePoint;
        public const string fillType = GSC_ProjectEditor.Constants.DatabaseDomainsValues.SymTypeFill;


        #endregion

        /// <summary>
        /// Initialization
        /// </summary>
        public Form_Legend_CreateTempTableLegendGenerator()
        {
            InitializeComponent();
        }

        private void btn_Create_Click(object sender, EventArgs e)
        {
            //Retrieve some options
            List<string> listedMaps = new List<string>();
            CheckedListBox.CheckedItemCollection checkedList = this.checkedListBox_Maps.CheckedItems;
            foreach (string items in checkedList)
            {
                listedMaps.Add(items.ToString());
            }
            bool allMapUnits = this.checkBox_FullMapUnits.Checked;

            //Process empty check list
            if (listedMaps.Count == 0)
            {
                //Add default
                listedMaps.Add(defaultLegend);
            }

            //Call the creation method
            CreateTempLegend(listedMaps, allMapUnits);

            //Close the form
            this.Close();
        }

        /// <summary>
        /// Will return a table name with a added letter to then end if a table already exists
        /// </summary>
        /// <param name="currentName">The table name to verify and modify if already existing.</param>
        /// <returns></returns>
        public string newTableName(string currentName, IWorkspace outputWorkspace)
        {
            //Variable
            currentName = currentName.Replace(' ', '_'); //Spaces in name could bug valid field name, make it look like they are when spaces are saved in a geodatabase.
            string newName = currentName;
            int counter = 1; //Will be used to iterate through existing mxds


            //Check if current path exist
            while (GSC_ProjectEditor.Workspace.GetNameExists(esriDatasetType.esriDTTable, newName))
            {

                //Calculate a new alpha ID
                string newAlphaID = GSC_ProjectEditor.IDs.CalculateSimplementAlphabeticID(true, counter);

                //Build a new path
                newName = currentName + "_" + newAlphaID;

                counter = counter + 1;
            }

            //Validate new table name
            newName = GSC_ProjectEditor.Workspace.GetValidDatasetName(outputWorkspace, newName);

            return newName;
        }

        /// <summary>
        /// Will create the new temporary legend
        /// </summary>
        public void CreateTempLegend(List<string> mapList, bool fullMapUnits)
        {
            try
            {
                #region Build proper table objects and other

                ITable legendGenerator = GSC_ProjectEditor.Tables.OpenTable(tLegend);
                ITable legendTree = GSC_ProjectEditor.Tables.OpenTable(tTreeTable);
                ITable legendDescription = GSC_ProjectEditor.Tables.OpenTable(tLegendDesc);

                #endregion

                foreach (string maps in mapList)
                {

                    #region Create a subset of the tree table based on selected map

                    //Access a in memory workspace
                    IWorkspace inMemoryWorkspace = GSC_ProjectEditor.Workspace.AccessInMemoryWorkspace();
                    string inMemoryTreeName = tTreeTable + "temp";
                    GSC_ProjectEditor.Tables.CopyTableToWorkspace(inMemoryWorkspace, legendTree as ITable, inMemoryTreeName);
                    ITable inMemoryTree = GSC_ProjectEditor.Tables.OpenTableFromWorkspace(inMemoryWorkspace, inMemoryTreeName);

                    //Delete unwanted maps from table
                    if (maps != defaultLegend)
                    {
                        IQueryFilter filterTree = new QueryFilter();
                        filterTree.WhereClause = tTreeTableCGMID + " <> '" + maps + "'";
                        inMemoryTree.DeleteSearchedRows(filterTree);
                    }


                    #endregion

                    #region Create the first relation with Tree table

                    //Build a relationship factory
                    Type memRelClassFactoryType = Type.GetTypeFromProgID("esriGeodatabase.MemoryRelationshipClassFactory");
                    IMemoryRelationshipClassFactory memRel = Activator.CreateInstance(memRelClassFactoryType) as IMemoryRelationshipClassFactory;

                    //Build a relationship class
                    IRelationshipClass relClass1 = memRel.Open("first", legendGenerator as IObjectClass, tLegendItemID, inMemoryTree as IObjectClass, tTreeTableItemID, "forward", "backward", esriRelCardinality.esriRelCardinalityOneToMany);

                    //Build the relation query table factory 
                    Type relQueryFactoryType = Type.GetTypeFromProgID("esriGeodatabase.RelQueryTableFactory");
                    IRelQueryTableFactory relFactory = Activator.CreateInstance(relQueryFactoryType) as IRelQueryTableFactory;

                    //Build a query for selected map only
                    IQueryFilter filterMap = new QueryFilter();
                    if (maps != defaultLegend)
                    {
                        //Header key character
                        string headerKey = tLegendSymTypeHeaderValue1.First().ToString();

                        //Build the query to filter by maps
                        string filterMapSQL = tTreeTableCGMID + " = '" + maps + "' OR " + tLegendItemType + " <> '" + tLegendItemTypeGeopoint + "' OR " + tLegendItemType + " <> '" + tLegendItemTypeGeoline + "'";

                        //Add some specific to query if user wants all the map units
                        if (fullMapUnits)
                        {
                            filterMapSQL = filterMapSQL + " OR " + tLegendSymType + " = '" + tLegendSymTypeFillValue + "'";
                        }
                        filterMap.WhereClause = filterMapSQL;
                    }
                    else
                    {
                        //Build the query to filter by maps, since it's for all the project, select all.
                        string filterMapSQL = tLegendSymType + " IS NOT NULL";
                        filterMap.WhereClause = filterMapSQL;
                    }

                    //Build the query table
                    IRelQueryTable relTable = relFactory.Open(relClass1, true, filterMap, null, "*", true, true);
                    
                    #endregion

                    #region Create the second relation with legend description table

                    //Build a second relationship clas
                    IRelationshipClass relClass2 = memRel.Open("second", relTable as IObjectClass, inMemoryTreeName + "." + tTreeTableDescID, legendDescription as IObjectClass, tLegendDescID, "forward", "backward", esriRelCardinality.esriRelCardinalityOneToOne);

                    //Build a second query table
                    IRelQueryTable relTable2 = relFactory.Open(relClass2, true, filterMap, null, "*", false, true);

                    #endregion

                    #region Persist new table to in memory workspace, in order to add a new field (can't add field while in edit session)

                    //Get workspace
                    string newTableNameRaw = tLegendTemp + "_" + maps;
                    string tLegendTempValidName = newTableName(newTableNameRaw, inMemoryWorkspace);


                    //Persist new table
                    GSC_ProjectEditor.Tables.CopyTableToWorkspace(inMemoryWorkspace, relTable2 as ITable, tLegendTempValidName);
                    ITable inMemoryLegend = GSC_ProjectEditor.Tables.OpenTableFromWorkspace(inMemoryWorkspace, tLegendTempValidName);

                    #endregion

                    #region Add a new field that will show if items is inside selected map, useful for the full map unit legend for a selected map

                    //Add field, needed to removed the domain from it...
                    AddCustomField(inMemoryLegend);

                    //Make a new copy but this time in the project database to persist the new field
                    GSC_ProjectEditor.Tables.CopyTableToProjectWorkspace(inMemoryLegend, tLegendTempValidName);

                    #endregion

                    #region Get new table

                    ITable newTable = GSC_ProjectEditor.Tables.OpenTable(tLegendTempValidName);
                    IQueryFilter qf = new QueryFilter();

                    
                    #endregion

                    #region Calculate new values for description field, from lines, points and headers

                    string queryNotFillSymbols = tLegendDescriptionField + " = ''";
                    IQueryFilter updateFilter = new QueryFilter();
                    updateFilter.WhereClause = queryNotFillSymbols;

                    ICursor updateCursor = newTable.Update(updateFilter, true);
                    
                    IRow updateRow = updateCursor.NextRow();
                    int descFieldIndex = updateRow.Fields.FindField(tLegendDescriptionField);
                    int nameFieldIndex = updateRow.Fields.FindField(tLegendName);

                    while (updateRow != null)
                    {
                        updateRow.set_Value(descFieldIndex, updateRow.get_Value(nameFieldIndex));
                        updateCursor.UpdateRow(updateRow);
                        updateRow = updateCursor.NextRow();
                        updateCursor.Flush();
                    }

                    //Release the cursor or else some lock could happen.
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(updateCursor);

                    #endregion

                    #region Calculate Is Visible field with a spatial query

                    CalculateIsVisibleField(newTable, maps);

                    #endregion

                    #region Add to Arc Map

                    //Access current map
                    IMap currentMap = Utilities.MapDocumentFeatureLayers.GetCurrentMapObject();

                    //Create a standAlone table out of the result
                    IStandaloneTable standAloneTable = new StandaloneTable();
                    standAloneTable.Table = newTable;

                    //Add stand alone table to current map collection
                    IStandaloneTableCollection currentCollection = currentMap as IStandaloneTableCollection;
                    currentCollection.AddStandaloneTable(standAloneTable);

                    //Update map document
                    IMxDocument currentMapDocument = ArcMap.Document;
                    currentMapDocument.UpdateContents();

                    #endregion

                    #region Delete temp tables from inMemory

                    IDataset inMemoryTreeDataset = inMemoryTree as IDataset;
                    inMemoryTreeDataset.Delete();

                    IDataset inMemoryLegendDataset = inMemoryLegend as IDataset;
                    inMemoryLegendDataset.Delete();

                    #endregion
                }



                MessageBox.Show(GSC_ProjectEditor.Properties.Resources.Message_ToolEnds, GSC_ProjectEditor.Properties.Resources.Message_ToolEndsTitle, MessageBoxButtons.OK);

            }
            catch (Exception Button_Legend_CreateTempTableLegendGeneratorException)
            {

                MessageBox.Show(Button_Legend_CreateTempTableLegendGeneratorException.StackTrace + "; " + Button_Legend_CreateTempTableLegendGeneratorException.Message);
            }



        }

        private void checkBox_AllMaps_CheckedChanged(object sender, EventArgs e)
        {
            //Cast
            CheckBox currentCheckbox = sender as CheckBox;

            if (currentCheckbox.Checked)
            {
                ((ListBox)this.checkedListBox_Maps).DataSource = null;
                this.checkedListBox_Maps.Enabled = false;

            }
            else
            {

                //Fill the checkbox list
                fillMapList();

                this.checkedListBox_Maps.Enabled = true;
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
            List<string> currentMaps = GSC_ProjectEditor.Tables.GetUniqueFieldValues(tTreeTable, tTreeTableCGMID, null, false, null)[MainDico];
            currentMaps.Sort();

            //Build the list
            foreach (string maps in currentMaps)
            {
                //Add map ids and description id to list that will go in checkbox list
                this.checkedListBox_Maps.Items.Add(maps);
            }

        }

        /// <summary>
        /// Will add a new field to specify if legend item is present in selected map
        /// </summary>
        public void AddCustomField(ITable tableToAddField)
        {
            //Get current workspace
            IDataset currentDataset = tableToAddField as IDataset;
            IWorkspace currentWorkspace = currentDataset.Workspace;

            //Create the field object
            IField newField = new FieldClass();
            IFieldEdit2 newFieldEdit = (IFieldEdit2)newField;

            newFieldEdit.Name_2 = isInsideField;
            newFieldEdit.Type_2 = esriFieldType.esriFieldTypeSmallInteger;
            newFieldEdit.IsNullable_2 = true;
            newFieldEdit.AliasName_2 = isInsideFieldAlias;
            newFieldEdit.DefaultValue_2 = booleanDomainTrue;
            newFieldEdit.Editable_2 = true;


            //Add to feature
            tableToAddField.AddField(newField);

        }

        /// <summary>
        /// Will fill the isVisible field with a spatial query on given objects.
        /// </summary>
        /// <param name="inTable">The table in which the isVisible field needs to be updated</param>
        /// <param name="mapID"> The map id as string in which the intersection needs to be done.</param>
        public void CalculateIsVisibleField(ITable inTable, string mapID)
        {
            #region Init.
            //Cast table as dataset
            IDataset inTableDataset = inTable as IDataset;

            //Get the CGM map feature class
            IFeatureClass cgmFC = GSC_ProjectEditor.FeatureClass.OpenFeatureClass(cgmFeature);

            //Get other feature classes
            IFeatureClass geolineFC = GSC_ProjectEditor.FeatureClass.OpenFeatureClass(fcGeoline);
            IFeatureClass geopointFC = GSC_ProjectEditor.FeatureClass.OpenFeatureClass(fcGeopoint);
            IFeatureClass geopolysFC = GSC_ProjectEditor.FeatureClass.OpenFeatureClass(fcGeopolys);

            //Add feature classes to list
            Dictionary<IFeatureClass, string> fcDico = new Dictionary<IFeatureClass, string>();
            fcDico[geolineFC] = fcGeolineID;
            fcDico[geopointFC] = fcGeopointID;
            fcDico[geopolysFC] = fcGeopolysID;

            //Build a list of ids from features that do intersect with the cgm maps
            List<string> idList = new List<string>();

            //Build a dictionary to parse headers that shouldn't appear in a given map
            Dictionary<double, List<string>> headerDico = new Dictionary<double, List<string>>();

            #endregion

            #region Calculate visibility for geometries
            //Build a query filter to intersect only with wanted map
            string queryToFilterMap = cgmFeatureFieldID + " = '" + mapID + "'";

            //Proceed with the intersection
            foreach (KeyValuePair<IFeatureClass, string> featuresToIntersect in fcDico)
            {
                idList.AddRange(GetIntersectionResults(featuresToIntersect.Key, featuresToIntersect.Value, cgmFC, queryToFilterMap));
            }

            //With given id list, calculate isVisible field
            ICursor tableUpdateCursor = GSC_ProjectEditor.Tables.GetTableCursor("Update", null, inTableDataset.Name);
            IRow currentRow = tableUpdateCursor.NextRow();

            //Get some indexes
            int idFieldIndex = tableUpdateCursor.FindField(tLegendItemID);
            int isInsideMapIndex = tableUpdateCursor.FindField(isInsideField);
            int itemTypeIndex = tableUpdateCursor.FindField(tLegendSymType);
            int itemOrderIndex = tableUpdateCursor.FindField(tLegendSymOrder);
            int itemLegendTypeIndex = tableUpdateCursor.FindField(tLegendItemType);

            while (currentRow != null)
            {
                //current row values
                string currentRowID = currentRow.get_Value(idFieldIndex).ToString();
                string currentRowSymType = currentRow.get_Value(itemTypeIndex).ToString();
                double currentRowOrder = Math.Floor(Convert.ToDouble(currentRow.get_Value(itemOrderIndex)));
                string currentRowLegendItemType = currentRow.get_Value(itemLegendTypeIndex).ToString();

                //Add current item type to dico if header

                if (!headerDico.ContainsKey(currentRowOrder))
                {
                    headerDico[currentRowOrder] = new List<string>();
                }

                //Detect if current row id is inside list
                if (idList.Contains(currentRowID))
                {
                    //Write true inside field
                    currentRow.set_Value(isInsideMapIndex, booleanDomainTrue);

                    //Add current row to header dico
                    headerDico[currentRowOrder].Add(currentRowID);
                }
                else
                {
                    //Write false inside field or delete if it's not a header or a fill symbol when the option isn't checked
                    if (!checkBox_AllMaps.Checked)
                    {
                        if (currentRowSymType == lineType || currentRowLegendItemType == tLegendItemTypeGeopoint || (currentRowSymType == fillType && !checkBox_FullMapUnits.Checked))
                        {
                            currentRow.Delete();
                        }
                        else
                        {
                            currentRow.set_Value(isInsideMapIndex, booleanDomainFalse);
                        }

                    }
                    else
                    {
                        currentRow.set_Value(isInsideMapIndex, booleanDomainTrue);
                    }

                }

                //Update
                tableUpdateCursor.UpdateRow(currentRow);

                currentRow = tableUpdateCursor.NextRow();
            }

            //Delete cursor to prevent lock
            System.Runtime.InteropServices.Marshal.ReleaseComObject(tableUpdateCursor);

            #endregion

            #region Calculate visibility for headers and other custom themes

            //Build query to filter cursor
            string headersAndCustomsOnly = tLegendItemType + " <> '" + tLegendItemTypeGeopoint + "' OR " + tLegendItemType + " <> '" + tLegendItemTypeGeoline + "' OR " + tLegendItemType + " <> '" + tLegendItemTypeMapUnit + "'";

            ICursor tableHeaderVisibilityCursor = GSC_ProjectEditor.Tables.GetTableCursor("Update", headersAndCustomsOnly, inTableDataset.Name);
            int legenditemTypeIndex = tableHeaderVisibilityCursor.Fields.FindField(tLegendItemType);
            IRow headerCursorRow = tableHeaderVisibilityCursor.NextRow();
            while (headerCursorRow != null)
            {
                //Retrieve information from current row
                double currentFlooredOrder = Math.Floor(Convert.ToDouble(headerCursorRow.get_Value(itemOrderIndex)));
                string currentItemType = headerCursorRow.get_Value(legenditemTypeIndex).ToString();

                //Update table if order as anything in the dictionary
                if (headerDico.ContainsKey(currentFlooredOrder))
                {
                    if (headerDico[currentFlooredOrder].Count() == 0)
                    {
                        //If user wants all map units keep all headers, else delete row
                        if (this.checkBox_FullMapUnits.Checked)
                        {
                            headerCursorRow.set_Value(isInsideMapIndex, booleanDomainFalse);
                            tableHeaderVisibilityCursor.UpdateRow(headerCursorRow);
                        }
                        else
                        {

                            if (currentItemType == tLegendItemTypeHeader)
                            {
                                headerCursorRow.Delete();
                            }

                        }

                    }
                    else
                    {
                        if (currentItemType == tLegendItemTypeHeader)
                        {
                            headerCursorRow.set_Value(isInsideMapIndex, booleanDomainTrue);
                        }
                        
                        tableHeaderVisibilityCursor.UpdateRow(headerCursorRow);
                    }
                }

                headerCursorRow = tableHeaderVisibilityCursor.NextRow();
            }

            //Delete cursor to prevent lock
            System.Runtime.InteropServices.Marshal.ReleaseComObject(tableHeaderVisibilityCursor);

            #endregion
        }

        /// <summary>
        /// Will proceed with a spatial query to determine if a given feature class intersects with another
        /// </summary>
        /// <returns></returns>
        public List<string> GetIntersectionResults(IFeatureClass fcToIntersectWith, string fcToIntersectWithID, IFeatureClass fcToIntersectFrom, string fcToIntersectFromFilter)
        {
            //Variable
            List<string> intersectingList = new List<string>();

            //Get a cursor with wanted map only
            IDataset fcToIntersectFromDS = fcToIntersectFrom as IDataset;
            IFeatureCursor mapCursor = GSC_ProjectEditor.FeatureClass.GetFeatureCursor("Search", fcToIntersectFromFilter, fcToIntersectFromDS.Name);
            IFeature currentMap = mapCursor.NextFeature();
            while (currentMap != null)
            {
                //Create a spatial query with feature classes
                ISpatialFilter spatialIntersectFilter = new SpatialFilter();
                spatialIntersectFilter.Geometry = currentMap.Shape; //Add current feature polygon geometry to spatialQuery
                spatialIntersectFilter.GeometryField = fcToIntersectWith.ShapeFieldName; //Add shape field name of feature class querying against  (geolines)
                spatialIntersectFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;//A simple intersection query

                //Iterate through result
                IFeatureCursor currentFeatureCursor = fcToIntersectWith.Search((IQueryFilter)spatialIntersectFilter, true);
                IFeature currentFeat = currentFeatureCursor.NextFeature();
                int idField = currentFeatureCursor.FindField(fcToIntersectWithID);
                while (currentFeat != null)
                {
                    //Add current row to list
                    intersectingList.Add(currentFeat.get_Value(idField).ToString());
                    currentFeat = currentFeatureCursor.NextFeature();
                }

                //Flush cursor
                System.Runtime.InteropServices.Marshal.ReleaseComObject(currentFeatureCursor);

                //Next
                currentMap = mapCursor.NextFeature();

            }

            //Flush cursor
            System.Runtime.InteropServices.Marshal.ReleaseComObject(mapCursor);

            return intersectingList.Distinct().ToList();
        }
    }
}
