using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;
using ESRI.ArcGIS.Editor;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geoprocessing;
using ESRI.ArcGIS.GeoprocessingUI;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geometry;


namespace GSC_ProjectEditor
{
    public class Button_Legend_IntersectItemsWithStudyArea : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        #region Main Variables

        //CGM feature
        private const string cgmFeature = GSC_ProjectEditor.Constants.Database.FCGMIndex;
        private const string cgmID = GSC_ProjectEditor.Constants.DatabaseFields.FCGM_ID;
        private const string cgmLayer = GSC_ProjectEditor.Constants.Layers.cgmMapIndex;

        //Geoline feature
        private const string geolineFeature = GSC_ProjectEditor.Constants.Database.FGeoline;
        private const string geolineID = GSC_ProjectEditor.Constants.DatabaseFields.FGeolineID;
        private const string geolineLayer = GSC_ProjectEditor.Constants.Layers.geoline;

        //Geopoint feature
        private const string geopointFeature = GSC_ProjectEditor.Constants.Database.FGeopoint;
        private const string geopointID = GSC_ProjectEditor.Constants.DatabaseFields.FGeopointID;
        private const string geopointLayer = GSC_ProjectEditor.Constants.Layers.geopoint;

        //Label feature
        private const string labelFeature = GSC_ProjectEditor.Constants.Database.FLabel;
        private const string labelID = GSC_ProjectEditor.Constants.DatabaseFields.FLabelID;

        //Map unit feature
        private const string geopolyFeature = GSC_ProjectEditor.Constants.Database.FGeopoly;
        private const string geopolyID = GSC_ProjectEditor.Constants.DatabaseFields.FGeopolyLabel;
        private const string geopolyLayer = GSC_ProjectEditor.Constants.Layers.geopoly;

        //Cartographic point feature
        private const string cartoPointFeature = GSC_ProjectEditor.Constants.Database.FCartoPoint;
        private const string cartoPointFeatureID = GSC_ProjectEditor.Constants.DatabaseFields.FCartoPointID;

        //Legend generator table
        private const string legendGeneTable = GSC_ProjectEditor.Constants.Database.TLegendGene;
        private const string legendGeneID = GSC_ProjectEditor.Constants.DatabaseFields.LegendLabelID;
        private const string legendGeneSymType = GSC_ProjectEditor.Constants.DatabaseFields.LegendSymType;

        //Legend tree table
        private const string legendTreeTable = GSC_ProjectEditor.Constants.Database.TLegendTree;
        private const string legendTreeID = GSC_ProjectEditor.Constants.DatabaseFields.LegendTreeItemID;
        private const string legendTreeCGMID = GSC_ProjectEditor.Constants.DatabaseFields.LegendTreeCGM;
        private const string legendTreeDescID = GSC_ProjectEditor.Constants.DatabaseFields.LegendTreeDescID;

        //Domains
        private const string symbolTypeLine = GSC_ProjectEditor.Constants.DatabaseDomainsValues.SymTypeLine;
        private const string symbolTypeFill = GSC_ProjectEditor.Constants.DatabaseDomainsValues.SymTypeFill;
        private const string symbolTypeMarkerPoint = GSC_ProjectEditor.Constants.DatabaseDomainsValues.SymTypePoint;

        //Other
        //private const string interGroupLayer = GSC_ProjectEditor.Constants.Layers.GroupInterpretation;
        private const string oidField = GSC_ProjectEditor.Constants.DatabaseFields.ObjectID;
        IList<Tuple<string, string>> masterCombinationList = new List<Tuple<string, string>>(); //A list to retain all process legends ids and cgm_map ids, will be used to nullify unmatching items from spatial query.
        bool knownError = false;

        #endregion

        public Button_Legend_IntersectItemsWithStudyArea()
        {

        }

        /// <summary>
        /// By click the button the users wants to do an spatial intersection between CGM maps and legend itemds. The tool will than parsed the result and update
        /// the table LEGEND_GENERATOR and the LEGEND_TREETABLE
        /// </summary>
        protected override void OnClick()
        {
            List<string> restrictedDataset = new List<string>();
            restrictedDataset.Add(Constants.Database.TLegendTree);
            restrictedDataset.Add(Constants.Database.TLegendGene);
            restrictedDataset.Add(Constants.Database.FGeopoint);
            restrictedDataset.Add(Constants.Database.FGeoline);
            restrictedDataset.Add(Constants.Database.FLabel);
            restrictedDataset.Add(Constants.Database.FGeopoly);
            restrictedDataset.Add(Constants.Database.FCGMIndex);
            restrictedDataset.Add(Constants.Database.FCartoPoint);

            string currentWorkspacePath = Properties.Settings.Default.PROJECT_DATABASE_PATH;
            IWorkspace currentWorkspace = Workspace.AccessWorkspace(currentWorkspacePath);
            bool isProjectValid = Workspace.isWorkspaceAProperProjectDatabase(currentWorkspace, restrictedDataset);

            if (isProjectValid)
            {
                //Set culture before init.
                Utilities.Culture.SetCulture();

                try
                {
                    string interGroupLayer = GSC_ProjectEditor.Properties.Resources.GroupLayerInterpretation;

                    #region Variable creation and assignment

                    //Access CGM feature
                    IFeatureLayer cgmFL = Utilities.MapDocumentFeatureLayers.GetFeatureLayerPreProcessing(cgmFeature, interGroupLayer);
                    if (cgmFL.Name == "")
                    {
                        MessageBox.Show(Properties.Resources.Error_MissingFeature + " " + cgmFeature);
                        knownError = true;
                    }
                    //Access geoline feature
                    IFeatureLayer geolinelFL = Utilities.MapDocumentFeatureLayers.GetFeatureLayerPreProcessing(geolineFeature, interGroupLayer);
                    if (geolinelFL.Name == "")
                    {
                        MessageBox.Show(Properties.Resources.Error_MissingFeature + " " + geolineFeature);
                        knownError = true;
                    }
                    //Access geopoint feature
                    IFeatureLayer geopointFL = Utilities.MapDocumentFeatureLayers.GetFeatureLayerPreProcessing(geopointFeature, interGroupLayer);
                    if (geopointFL.Name == "")
                    {
                        MessageBox.Show(Properties.Resources.Error_MissingFeature + " " + geopointFeature);
                        knownError = true;
                    }
                    //Access label feature
                    IFeatureLayer geopolyFL = Utilities.MapDocumentFeatureLayers.GetFeatureLayerPreProcessing(geopolyFeature, interGroupLayer);
                    if (geopolyFL.Name == "")
                    {
                        MessageBox.Show(Properties.Resources.Error_MissingFeature + " " + geopolyFeature);
                        knownError = true;
                    }

                    //Acces carto point feature
                    IFeatureLayer cartoPointFL = Utilities.MapDocumentFeatureLayers.GetFeatureLayerPreProcessing(cartoPointFeature, interGroupLayer);
                    if (cartoPointFL.Name == "")
                    {
                        //Add the layer to arc map since it's not there by default.
                        cartoPointFL = new FeatureLayerClass();
                        cartoPointFL.FeatureClass = GSC_ProjectEditor.FeatureClass.OpenFeatureClass(cartoPointFeature);
                        cartoPointFL.Name = cartoPointFeature;
                        Utilities.MapDocumentFeatureLayers.AddLayerToArcMapPreProcessing(cartoPointFL, interGroupLayer);

                    }

                    //Create filters to query labels and polygons
                    ISpatialFilter spatialQuery = new SpatialFilter(); //Spatial query to filter items within cgm maps
                    IQueryFilter simpleQuery = new QueryFilter(); //Empty filter to use in cgm feature cursor

                    //Reset master combination list
                    masterCombinationList.Clear();

                    #endregion

                    #region Iterate through maps

                    //MAIN PROCESS => Iterate through all maps
                    IFeatureCursor mapCursor = cgmFL.Search(simpleQuery, true);

                    //Get some indexes
                    int cgmMapIDIndex = mapCursor.Fields.FindField(cgmID);
                    int cgmOIDIndex = mapCursor.Fields.FindField(oidField);

                    IFeature mapFeat = mapCursor.NextFeature();

                    while (mapFeat != null)
                    {

                        //Get mapID and OID field value
                        object currentOIDValue = mapFeat.get_Value(cgmOIDIndex);
                        string currentMapID = mapFeat.get_Value(cgmMapIDIndex).ToString();

                        //Variables
                        Dictionary<string, List<string>> spatialQueryResult = new Dictionary<string, List<string>>(); //A list to retrieve all spatial query intersection results
                        List<string> geolineResult = new List<string>(); //A list to contain all intersection result for geolines.
                        List<string> geopointResult = new List<string>(); //A list to contain all intersection result geo geopoints.
                        List<string> geopolyResult = new List<string>(); //A list to contain all intersection result for labels
                        List<string> cartopointResult = new List<string>(); //A list to contain all intersection result for cartogrpahic point.


                        #region iterate through geolines and build resulting lists

                        //Perform a spatial query
                        spatialQuery.Geometry = mapFeat.Shape; //Add current feature polygon geometry to spatialQuery
                        spatialQuery.GeometryField = geolinelFL.FeatureClass.ShapeFieldName; //Add shape field name of feature class querying against  (geolines)
                        spatialQuery.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;//A simple intersection query

                        //Iterate through polygons, with an applied spatial query into it
                        IFeatureCursor geolineCursor = geolinelFL.Search((IQueryFilter)spatialQuery, true);

                        //Get field indexex
                        int geolineIDIndex = geolineCursor.Fields.FindField(geolineID);
                        int geolineOIDIndex = geolineCursor.Fields.FindField(oidField);

                        IFeature geolineFeat = null;

                        while ((geolineFeat = geolineCursor.NextFeature()) != null)
                        {

                            //Get label and OID field value
                            object getGeolineID = geolineFeat.get_Value(geolineIDIndex);
                            object getGeolineOID = geolineFeat.get_Value(geolineOIDIndex);

                            //Add id to geoline resulting list
                            geolineResult.Add(getGeolineID.ToString());

                        }

                        //Add to main dico
                        spatialQueryResult[symbolTypeLine] = geolineResult;

                        //Release cursor
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(geolineCursor);

                        #endregion

                        #region iterate through geopoints and build resulting lists

                        //Perform a spatial query
                        spatialQuery.GeometryField = geopointFL.FeatureClass.ShapeFieldName; //Add shape field name of feature class querying against  (geolines)

                        //Iterate through polygons, with an applied spatial query into it
                        IFeatureCursor geopointCursor = geopointFL.Search((IQueryFilter)spatialQuery, true);

                        //Get field indexex
                        int geopointIDIndex = geopointCursor.Fields.FindField(geopointID);
                        int geopointOIDIndex = geopointCursor.Fields.FindField(oidField);

                        IFeature geopointFeat = null;

                        while ((geopointFeat = geopointCursor.NextFeature()) != null)
                        {

                            //Get label and OID field value
                            object getGeopointID = geopointFeat.get_Value(geopointIDIndex);
                            object getGeopointOID = geopointFeat.get_Value(geopointOIDIndex);

                            //Add id to geoline resulting list
                            geopointResult.Add(getGeopointID.ToString());

                        }

                        //Add to main dico
                        spatialQueryResult[symbolTypeMarkerPoint] = geopointResult;

                        //Release cursor
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(geopointCursor);

                        #endregion

                        #region iterate through geopolys and build resulting lists

                        //Perform a spatial query
                        spatialQuery.GeometryField = geopolyFL.FeatureClass.ShapeFieldName; //Add shape field name of feature class querying against  (geolines)

                        //Iterate through polygons, with an applied spatial query into it
                        IFeatureCursor geopolyCursor = geopolyFL.Search((IQueryFilter)spatialQuery, true);

                        //Get field indexex
                        int geopolyIDIndex = geopolyCursor.Fields.FindField(geopolyID);
                        int geopolyOIDIndex = geopolyCursor.Fields.FindField(oidField);

                        IFeature geopolyFeat = null;

                        while ((geopolyFeat = geopolyCursor.NextFeature()) != null)
                        {

                            //Get label and OID field value
                            object getGeolpolyID = geopolyFeat.get_Value(geopolyIDIndex);
                            object getGeolpolyOID = geopolyFeat.get_Value(geopolyOIDIndex);

                            //Add id to geoline resulting list
                            geopolyResult.Add(getGeolpolyID.ToString());

                        }

                        //Add to main dico
                        spatialQueryResult[symbolTypeFill] = geopolyResult;

                        //Release cursor
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(geopolyCursor);

                        mapFeat = mapCursor.NextFeature();

                        #endregion

                        #region iterate through cartographic points and build resulting lists

                        //TODO Make this code  available when CARTO_POINTS contains a proper field to link to legend table.

                        ////Perform a spatial query
                        //spatialQuery.GeometryField = cartoPointFL.FeatureClass.ShapeFieldName; //Add shape field name of feature class querying against  (geolines)

                        ////Iterate through polygons, with an applied spatial query into it
                        //IFeatureCursor cartopointCursor = cartoPointFL.Search((IQueryFilter)spatialQuery, true);

                        ////Get field indexex
                        //int cartopointIDIndex = cartopointCursor.Fields.FindField(cartoPointFeatureID);
                        //int cartopointOIDIndex = cartopointCursor.Fields.FindField(oidField);

                        //IFeature cartoFeat = null;

                        //while ((cartoFeat = cartopointCursor.NextFeature()) != null)
                        //{

                        //    //Get label and OID field value
                        //    object getCartopointID = cartoFeat.get_Value(cartopointIDIndex);
                        //    object getCartopointOID = cartoFeat.get_Value(cartopointOIDIndex);

                        //    //Add id to geoline resulting list
                        //    cartopointResult.Add(getCartopointID.ToString());

                        //}

                        ////Add to main dico
                        //spatialQueryResult[symbolTypeMarkerPoint] = cartopointResult;

                        ////Release cursor
                        //System.Runtime.InteropServices.Marshal.ReleaseComObject(cartopointCursor);

                        #endregion

                        //Modify all needed tables with new founds values
                        updateLegendGenerator(spatialQueryResult); //P_LEGEND_GENERATOR table
                        updateLegendTree(currentMapID, spatialQueryResult); // LEGEND_TREETABLE

                    }

                    //Release cursor
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(mapCursor);

                    #endregion

                    GSC_ProjectEditor.Messages.ShowEndOfProcess();
                }
                catch (Exception Button_Legend_IntersectItemsWithStudyAreaException)
                {
                    //Show general error message if it's not a known one.
                    if (!knownError)
                    {
                        int lineNumber = GSC_ProjectEditor.Exceptions.LineNumber(Button_Legend_IntersectItemsWithStudyAreaException);
                        MessageBox.Show("Button_Legend_IntersectItemsWithStudyAreaException (" + lineNumber.ToString() + "):" + Button_Legend_IntersectItemsWithStudyAreaException.Message);
                    }
                }
            }



        }

        /// <summary>
        /// Will update P_LEGEND_GENERATOR table
        /// </summary>
        /// <param name="spatialQueryResult">An input dictionary containing spatial query result</param>
        public void updateLegendGenerator(Dictionary<string, List<string>> spatialQueryResult)
        {
            //Variables
            Dictionary<string, List<string>> updateIDs = new Dictionary<string, List<string>>(); //{SymType: IDs}
            Dictionary<string, List<string>> addIDs = new Dictionary<string, List<string>>(); //{SymType: IDs}

            //Get some parses dictionaries
            getUpdateAndAddDictionary(legendGeneTable, legendGeneID, spatialQueryResult, out updateIDs, out addIDs);

            //Update legend generator table
            if(updateIDs.Count != 0)
            {
                ///Does nothing for now...
                ///

                #region would update table
                ////Get an update cursor
                //ICursor upCursor = GSC_ProjectEditor.Tables.GetTableCursor("Update", null, legendGeneTable);

                ////Get some field indexes
                //int idFieldIndex = upCursor.Fields.FindField(legendGeneID);

                ////Start cursor
                //IRow upRow = upCursor.NextRow();
                //while (upRow != null)
                //{

                //}
                #endregion

            }

            //Add new rows to legend generator table
            if (addIDs.Count != 0)
            {

                #region Add new rows
                //Create a row buffer object (a template of all fields of table)
                IRowBuffer inRowBuffer = GSC_ProjectEditor.Tables.GetRowBuffer(legendGeneTable);

                //Start a cursor to insert new row
                ICursor inCursor = GSC_ProjectEditor.Tables.GetTableCursor("Insert", null, legendGeneTable);

                //Get some field indexes
                int idFieldIndex = inCursor.Fields.FindField(legendGeneID);
                int symTypeFieldIndex = inCursor.Fields.FindField(legendGeneSymType);

                //Iterate through dictionnary
                foreach (KeyValuePair<string, List<string>> elements in addIDs)
                {

                    foreach (string ids in elements.Value)
                    {
                        //Set field value
                        inRowBuffer.set_Value(idFieldIndex, ids);
                        inRowBuffer.set_Value(symTypeFieldIndex, elements.Key);

                        //Add the new row
                        inCursor.InsertRow(inRowBuffer);
                    }

                }

                //Release the cursor or else some lock could happen.
                System.Runtime.InteropServices.Marshal.ReleaseComObject(inCursor);

                #endregion
            }


        }

        /// <summary>
        /// Will update LEGEND_TREETABLE table
        /// </summary>
        /// <param name="spatialQueryResult">An input dictionary containing spatial query result</param>
        public void updateLegendTree(string currentMapID, Dictionary<string, List<string>> spatialQueryResult)
        {
            //Variables
            Dictionary<string, List<string>> updateIDs = new Dictionary<string, List<string>>(); //{SymType: IDs}
            Dictionary<string, List<string>> addIDs = new Dictionary<string, List<string>>(); //{SymType: IDs}
            List<string> updateIDList = new List<string>();
            List<string> addIDList = new List<string>();
            Tuple<string, string> fieldTuple = new Tuple<string, string>(legendTreeID, legendTreeCGMID);

            //Get some parses dictionaries
            getUpdateAndAddDictionary4Tree(legendTreeTable, fieldTuple, currentMapID, spatialQueryResult, out updateIDList, out addIDList);

            //Get current set of description ids and legend items
            SortedDictionary<string, List<string>> currentLegendDescriptions = GSC_ProjectEditor.Tables.GetAllDoubleUniqueFieldValues(legendTreeTable, legendTreeID, legendTreeDescID, null);

            //Update legend generator table
            if (updateIDList.Count != 0)
            {
                ///Do nothing for now.
                ///
                #region Update table with new CGM values

                #endregion

            }

            //Add new rows to legend generator table
            if (addIDList.Count != 0)
            {
                #region Add new rows
                //Create a row buffer object (a template of all fields of table)
                IRowBuffer inRowBuffer = GSC_ProjectEditor.Tables.GetRowBuffer(legendTreeTable);

                //Start a cursor to insert new row
                ICursor inCursor = GSC_ProjectEditor.Tables.GetTableCursor("Insert", null, legendTreeTable);

                //Get some field indexes
                int idFieldIndex = inCursor.Fields.FindField(legendTreeID);
                int cgmIdFieldINdex = inCursor.Fields.FindField(legendTreeCGMID);
                int descIdFieldIndex = inCursor.Fields.FindField(legendTreeDescID);

                //Iterate through list
                foreach (string ids in addIDList)
                {
                    //Set field value
                    inRowBuffer.set_Value(idFieldIndex, ids); //Update legend item id field
                    inRowBuffer.set_Value(cgmIdFieldINdex, currentMapID); //Update cgm id field

                    //If needed, update description ID
                    if (currentLegendDescriptions.ContainsKey(ids))
                    {
                        if (currentLegendDescriptions[ids].Count != 0 && currentLegendDescriptions[ids][0] != null && currentLegendDescriptions[ids][0] != string.Empty)
                        {
                            double doubleDescID = Convert.ToDouble(currentLegendDescriptions[ids][0]);
                            inRowBuffer.set_Value(descIdFieldIndex, doubleDescID); //Update description id
                        }
                    }

                    //Add the new row
                    inCursor.InsertRow(inRowBuffer);
                }

                //Release the cursor or else some lock could happen.
                System.Runtime.InteropServices.Marshal.ReleaseComObject(inCursor);

                #endregion
            }

        }

        /// <summary>
        /// Will parses the intersection result dictionnary into two new dictionary, to know which ids needs to be updated or added within a certain table 
        /// </summary>
        /// <param name="spatialQueryResult"></param>
        /// <param name="updateDico"></param>
        /// <param name="addDico"></param>
        public void getUpdateAndAddDictionary4Tree(string inTable, Tuple<string, string> inFields, string currentMapID, Dictionary<string, List<string>> spatialQueryResult, out List<string> updateList, out List<string> addList)
        {
            //Get a list of all current ids in legend generator table
            List<Tuple<string, string>> allIDs = GSC_ProjectEditor.Tables.GetUniqueDoubleFieldValues(inTable, inFields, null);

            //Init other lists
            updateList = new List<string>();
            addList = new List<string>();

            //Iterate through symbol types
            foreach (KeyValuePair<string, List<string>> kv in spatialQueryResult)
            {
                #region Process legend items

                //Process all geoline ids
                foreach (string items in kv.Value)
                {
                    //Build tuple
                    Tuple<string, string> currentItem = new Tuple<string, string>(items, currentMapID);

                    //Find proper process (update or add)
                    if (allIDs.Contains(currentItem) && !updateList.Contains(items))
                    {
                        //Update
                        updateList.Add(items);

                        //Add to master list
                        masterCombinationList.Add(currentItem);
                    }
                    else if (!allIDs.Contains(currentItem) && !addList.Contains(items))
                    {
                        //Add new row
                        addList.Add(items);

                        //Add to master list
                        masterCombinationList.Add(currentItem);

                    }
                }

                #endregion

            }
        }
        
        /// <summary>
        /// Will parses the intersection result dictionnary into two new dictionary, to know which ids needs to be updated or added within a certain table 
        /// </summary>
        /// <param name="spatialQueryResult"></param>
        /// <param name="updateDico"></param>
        /// <param name="addDico"></param>
        public void getUpdateAndAddDictionary(string inTable, string inField, Dictionary<string, List<string>> spatialQueryResult, out Dictionary<string, List<string>> updateDico, out Dictionary<string, List<string>> addDico)
        {
            //Get a list of all current ids in legend generator table
            List<string> allIDs = GSC_ProjectEditor.Tables.GetFieldValues(inTable, inField, null);

            updateDico = new Dictionary<string, List<string>>(); //{SymType: IDs}
            addDico = new Dictionary<string, List<string>>(); //{SymType: IDs}

            //Iterate through symbol types
            foreach (KeyValuePair<string, List<string>> kv in spatialQueryResult)
            {
                List<string> updateList = new List<string>();
                List<string> addList = new List<string>();

                #region Process geoline symboles

                if (kv.Key == symbolTypeLine)
                {

                    //Process all geoline ids
                    foreach (string geolineIDs in kv.Value)
                    {
                        //Find proper process (update or add)
                        if (allIDs.Contains(geolineIDs) && !updateList.Contains(geolineIDs))
                        {
                            //Update
                            updateList.Add(geolineIDs);
                        }
                        else if (!allIDs.Contains(geolineIDs) && !addList.Contains(geolineIDs))
                        {
                            //Add new row
                            addList.Add(geolineIDs);

                        }
                    }

                    //Add lists to dicos
                    if (updateList.Count != 0)
                    {
                        updateDico[symbolTypeLine] = updateList;
                    }

                    if (addList.Count != 0)
                    {
                        addDico[symbolTypeLine] = addList;
                    }

                }

                #endregion

                #region Process geopoints symboles

                if (kv.Key == symbolTypeMarkerPoint)
                {

                    //Process all geoline ids
                    foreach (string geopointIDs in kv.Value)
                    {
                        //Find proper process (update or add)
                        if (allIDs.Contains(geopointIDs) && !updateList.Contains(geopointIDs))
                        {
                            //Update
                            updateList.Add(geopointIDs);
                        }
                        else if (!allIDs.Contains(geopointIDs) && !addList.Contains(geopointIDs))
                        {
                            //Add new row
                            addList.Add(geopointIDs);

                        }
                    }

                    //Add lists to dicos
                    if (updateList.Count != 0)
                    {
                        updateDico[symbolTypeMarkerPoint] = updateList;
                    }

                    if (addList.Count != 0)
                    {
                        addDico[symbolTypeMarkerPoint] = addList;
                    }

                }

                #endregion

                #region Process geopolys symboles

                if (kv.Key == symbolTypeFill)
                {

                    //Process all geoline ids
                    foreach (string geopolysIDs in kv.Value)
                    {
                        //Find proper process (update or add)
                        if (allIDs.Contains(geopolysIDs) && !updateList.Contains(geopolysIDs))
                        {
                            //Update
                            updateList.Add(geopolysIDs);
                        }
                        else if (!allIDs.Contains(geopolysIDs) && !addList.Contains(geopolysIDs))
                        {
                            //Add new row
                            addList.Add(geopolysIDs);

                        }
                    }

                    //Add lists to dicos
                    if (updateList.Count != 0)
                    {
                        updateDico[symbolTypeFill] = updateList;
                    }

                    if (addList.Count != 0)
                    {
                        addDico[symbolTypeFill] = addList;
                    }

                }

                #endregion

            }
        }

        protected override void OnUpdate()
        {
        }

        /// <summary>
        /// Will enable or disable current control class.
        /// </summary>
        /// <param name="enable"></param>
        public void EnableDisable(bool enable)
        {
            if (enable)
            {
                this.Enabled = true;
            }
            else
            {
                this.Enabled = false;
            }
        }
    }
}
