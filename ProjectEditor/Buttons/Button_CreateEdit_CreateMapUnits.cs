using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.DataSourcesFile;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.Desktop.AddIns;
using ESRI.ArcGIS.Editor;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Geoprocessing;

namespace GSC_ProjectEditor
{
    public class Button_CreateEdit_CreateMapUnits : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        //TODO Find a way to change cursor for a waiting one, because this is not a normal VS control...

        #region Main Variables

        //LEGEND TABLE
        private const string tLegend = GSC_ProjectEditor.Constants.Database.TLegendGene;
        private const string tLegendSymbol = GSC_ProjectEditor.Constants.DatabaseFields.LegendSymbol;
        private const string tLegendItemID = GSC_ProjectEditor.Constants.DatabaseFields.LegendLabelID;

        //FEATURE GEO_POLYS
        private const string geopoly = GSC_ProjectEditor.Constants.Database.FGeopoly;
        private const string geopolyLabel = GSC_ProjectEditor.Constants.DatabaseFields.FGeopolyLabel;
        private const string geopolyLayerName = GSC_ProjectEditor.Constants.Layers.geopoly;
        private const string geopolySourceID = GSC_ProjectEditor.Constants.DatabaseFields.SourceID;
        private const string geopolyRemark = GSC_ProjectEditor.Constants.DatabaseFields.FGeopolyRemark;

        //FEATURE P_LABELS
        private const string fcLabel = GSC_ProjectEditor.Constants.Database.FLabel;
        private const string fcLabelID = GSC_ProjectEditor.Constants.DatabaseFields.FLabelID;
        private const string labelLayerName = GSC_ProjectEditor.Constants.Layers.label;

        //Feature geoline
        private const string fcGeoline = GSC_ProjectEditor.Constants.Database.FGeoline;
        private const string fcGeolineSubtype = GSC_ProjectEditor.Constants.DatabaseFields.FGeolineSubtype;
        private const string fcGeolineSubtypeOverprint = GSC_ProjectEditor.Constants.DatabaseSubtypes.FGeolineSubOverprint;
        private const string fcGeolineIsBoundary = GSC_ProjectEditor.Constants.DatabaseFields.FGeolineBoundary;
        private const string fcGeolineIsBoundaryTrue = GSC_ProjectEditor.Constants.DatabaseDomainsValues.BoundYes;

        //Join Field names
        public string joinGeopolyLabel = geopoly + "." + geopolyLabel;
        public string joinTLegendSymbol = tLegend + "." + tLegendSymbol;
        public string joinTLegendDisplayLabel = tLegend + "." + GSC_ProjectEditor.Constants.DatabaseFields.LegendGISDisplay;

        //Database
        private const string FDName = GSC_ProjectEditor.Constants.Database.FDGeo;
        private const string mapUnitDom = GSC_ProjectEditor.Constants.DatabaseDomains.MapUnit;

        //Other
        //private const string interGroupLayer = GSC_ProjectEditor.Constants.Layers.GroupInterpretation;
        private const string dicoMain = "Main"; //Key for unique value dictionnary
        private const string outputTemp = "in_memory";
        private const string outputLabel_NoOverprint = outputTemp + "\\noOver2";
        private const string outputLabel_OverprintOnly = outputTemp +"\\OverOnly";
        private const string tempOverprint = "TOverprint";
        private const string tempOverprintFinal = "TOverprintFinal";
        private const string tempMultipart = "TMultipart";

        #endregion

        public List<string> overprintLevels { get; set; }

        public Button_CreateEdit_CreateMapUnits()
        {
            //Set culture before init.
            Utilities.Culture.SetCulture();
        }

        protected override void OnClick()
        {

            List<string> restrictedDataset = new List<string>();
            restrictedDataset.Add(Constants.Database.FLabel);
            restrictedDataset.Add(Constants.Database.FGeopoly);
            restrictedDataset.Add(Constants.Database.FGeoline);
            restrictedDataset.Add(Constants.Database.TLegendGene);

            string currentWorkspacePath = Properties.Settings.Default.PROJECT_DATABASE_PATH;
            IWorkspace currentWorkspace = Workspace.AccessWorkspace(currentWorkspacePath);
            bool isProjectValid = Workspace.isWorkspaceAProperProjectDatabase(currentWorkspace, restrictedDataset);

            if (isProjectValid)
            {
                #region Second draft code, with arc objects instead of python

                try
                {
                    //Create
                    CreateMapUnits(currentWorkspace);

                    //Update geopoly style
                    updateStyle();

                    GSC_ProjectEditor.Messages.ShowEndOfProcess();

                    //Refresh TOC
                    ArcMap.Document.ActivatedView.ContentsChanged();
                    ArcMap.Document.UpdateContents();
                    ArcMap.Document.ActiveView.Refresh();

                }
                catch (Exception e)
                {
                    MessageBox.Show(e.StackTrace);
                }

                #endregion
            }


        }

        public void CreateMapUnits(IWorkspace inWorkspace)
        {
            //Variables
            bool doesHaveOverprints = false;
            IWorkspace inMemoryWorkspace = GSC_ProjectEditor.Workspace.CreateInMemoryWorkspace();

            //Get some feature classes  and cursor to create new polygons from
            IFeatureClass geopolyFC = GSC_ProjectEditor.FeatureClass.OpenFeatureClassFromWorkspace(inWorkspace, geopoly); //output feature to put polygon into
            IFeatureClass labelFC = GSC_ProjectEditor.FeatureClass.OpenFeatureClassFromWorkspace(inWorkspace, fcLabel); //source of label to categorize polygons with

            //Empty geopoly feature from it's content
            IEditor editor = Utilities.EditSession.GetEditor();
            editor.StartOperation();
            GSC_ProjectEditor.FeatureClass.DeleteFeatures(geopolyFC, null, false);
            editor.StopOperation("");

            //Build a query to select geoline that are not overprints first and also set as a boundary
            string noOverprintQuery = fcGeolineSubtype + " <> " + fcGeolineSubtypeOverprint + " AND " + fcGeolineIsBoundary + " = '" + fcGeolineIsBoundaryTrue + "'";

            //Get a cursor with no overprint query
            if (GSC_ProjectEditor.Tables.GetRowCountFromWorkspace(inWorkspace, fcGeoline, noOverprintQuery) != 0)
            {
                //Create cursor for geoline with query
                IFeatureCursor geolineCursor = GSC_ProjectEditor.FeatureClass.GetFeatureCursorFromWorkspace(inWorkspace, "Search", noOverprintQuery, fcGeoline); //a cursor with the lines to create polygons from

                //Get a feature class of only wanted labels (no overprint), if needed
                IFeatureClass noOverLabelFC = ProcessLabels(labelFC, null, out doesHaveOverprints, inWorkspace);

                //Call a special feature construction object to create the polygons
                GSC_ProjectEditor.FeatureClass.ConvertLineToPolygon(geopolyFC, noOverLabelFC, geolineCursor);

                //Delete temporary features if needed.
                if (doesHaveOverprints)
                {
                    //Output feature class was a temp one not the original layer
                    deleteTempFeatures(noOverLabelFC);
                }

                GSC_ProjectEditor.ObjectManagement.ReleaseObject(geolineCursor);
            }

            //Get a list of overprints
            overprintLevels = GetOverprintLevels(labelFC, inWorkspace);

            //Build a query to select geoline that are only for overprints
            string overprintQuery = fcGeolineIsBoundary + " = '" + fcGeolineIsBoundaryTrue + "'";

            //Get a cursor with new overprint query
            if (overprintLevels.Count != 0)
            {
                //Variable
                IDataset geopolyFCDataset = geopolyFC as IDataset;
                ITable geopolyFCTable = geopolyFCDataset as ITable;
                string dissolvedFCPath = outputTemp + "\\" + tempMultipart;

                //Get the fields from default polygon feature class
                IFields geopolyFields = geopolyFC.Fields;

                //Create a new polygon feature class to hold result
                string newOverprintName = tempOverprintFinal;
                //IFeatureClass overprintFC = GSC_ProjectEditor.FeatureClass.CreateFeatureClass(inMemoryWorkspace, newOverprintName, geopolyFields, esriFeatureType.esriFTSimple, "SHAPE");

                //Iterate through all overprint levels
                overprintLevels.Sort();
                foreach (string levels in overprintLevels)
                {
                    #region For loop
                    //Get a new temp feature for labels
                    IFeatureClass currentOverLabelFC = ProcessLabels(labelFC, levels, out doesHaveOverprints, inWorkspace);
                    IDataset currentOverLabelFCDataset = currentOverLabelFC as IDataset;
                    ITable currentOVerLabelFCTable = currentOverLabelFCDataset as ITable;
                    int labelCount = currentOVerLabelFCTable.RowCount(null);

                    //Call method to create polygons from line with new query if some labels were found
                    if (labelCount != 0)
                    {
                        //New geoline cursor for the overprints only
                        IFeatureCursor geolineCursorOverprint = GSC_ProjectEditor.FeatureClass.GetFeatureCursorFromWorkspace(inWorkspace, "Search", overprintQuery, fcGeoline); //a cursor with the lines to create polygons from

                        //Create a new polygon feature class to hold result
                        //IWorkspace testWorkspace = GSC_ProjectEditor.Workspace.CreateWorkspaceForShapefiles("W:\\Transit\\gab");
                        string overprintName = "T" + levels;
                        IFeatureClass currentLevelOverprintPolygons = GSC_ProjectEditor.FeatureClass.CreateFeatureClass(inMemoryWorkspace, overprintName, geopolyFields, esriFeatureType.esriFTSimple, "SHAPE");

                        //Convert line and labels to polygons
                        GSC_ProjectEditor.FeatureClass.ConvertLineToPolygon(currentLevelOverprintPolygons, currentOverLabelFC, geolineCursorOverprint);

                        //Delete empty polygons
                        IQueryFilter2 emptyPolyFilter = new QueryFilterClass();
                        emptyPolyFilter.WhereClause = geopolyLabel + " =''";
                        IDataset currentPolygonDataset = currentLevelOverprintPolygons as IDataset;
                        ITable currentPolygonTable = currentPolygonDataset as ITable;
                        currentPolygonTable.DeleteSearchedRows(emptyPolyFilter);

                        //COM release
                        GSC_ProjectEditor.ObjectManagement.ReleaseObject(geolineCursorOverprint);

                        //Dissolve overprints if they touch each other, results will be appended to geopoly
                        //DoUnion(currentLevelOverprintPolygons, levels, geopolyLabel, overprintFC);
                        string statFields = geopolySourceID + " LAST;" + GSC_ProjectEditor.Constants.DatabaseFields.ETCreatorID + " LAST; " + GSC_ProjectEditor.Constants.DatabaseFields.ETEditorID + " LAST";
                        GSC_ProjectEditor.GeoProcessing.DissolveFeatureClass(currentLevelOverprintPolygons, dissolvedFCPath, geopolyLabel, statFields);

                        //Delete temp feature class
                        try
                        {
                            GSC_ProjectEditor.FeatureClass.DeleteFeatureClass(currentLevelOverprintPolygons);
                        }
                        catch (Exception)
                        {

                        }

                        ////Explode parts of overprints.
                        //string multipartFCPath = outputTemp + "\\" + tempMultipart;
                        //GSC_ProjectEditor.GeoProcessing.MultiPartToSinglePart(overprintFC, multipartFCPath);

                        //Append all the results
                        IFeatureClass dissolvedFC = GSC_ProjectEditor.FeatureClass.OpenFeatureClassFromInMemory(dissolvedFCPath);
                        Dictionary<string, List<string>> fieldMapping = new Dictionary<string, List<string>>();

                        for (int i = 0; i < dissolvedFC.Fields.FieldCount; i++)
                        {
                            IField iField = dissolvedFC.Fields.get_Field(i);
                            if (iField.Name.Contains(geopolySourceID.Substring(0, 4)))
                            {
                                fieldMapping.Add(geopolySourceID, new List<string>() { iField.Name });
                            }
                            if (iField.Name.Contains(GSC_ProjectEditor.Constants.DatabaseFields.ETCreatorID.Substring(0, 4)))
                            {
                                fieldMapping.Add(GSC_ProjectEditor.Constants.DatabaseFields.ETCreatorID, new List<string>() { iField.Name });
                            }
                            if (iField.Name.Contains(GSC_ProjectEditor.Constants.DatabaseFields.ETEditorID.Substring(0, 4)))
                            {
                                fieldMapping.Add(GSC_ProjectEditor.Constants.DatabaseFields.ETEditorID, new List<string>() { iField.Name });
                            }
                        }
                        fieldMapping.Add(geopolyLabel, new List<string>() { geopolyLabel });
                        GSC_ProjectEditor.GeoProcessing.AppendDataWithFieldMap(dissolvedFC, geopolyFC, fieldMapping);
                        try
                        {
                            //GSC_ProjectEditor.FeatureClass.DeleteFeatureClass(overprintFC);
                            GSC_ProjectEditor.FeatureClass.DeleteFeatureClass(dissolvedFC);
                            GSC_ProjectEditor.FeatureClass.DeleteFeatureClass(currentOverLabelFC);
                        }
                        catch (Exception)
                        {

                        }

                    }


                    #endregion
                }



            }

            //Update GSC_SYMBOL field if exists
            ITable geopolyTable = Tables.OpenTableFromWorkspace(inWorkspace, Constants.Database.FGeopoly);
            int polySymbolField = geopolyTable.FindField(Constants.DatabaseFields.LegendSymbol);
            if (polySymbolField > 0)
            {
                updateSymbolField(Constants.DatabaseFields.LegendSymbol, geopolyTable, inWorkspace);
            }

        }

        /// <summary>
        /// Will calculate GSC_SYMBOL field from P_LEGEND table, if field exits
        /// </summary>
        /// <param name="geopolyFC"></param>
        private void updateSymbolField(string legendSymbolFieldName, ITable polyTable, IWorkspace inWorkspace)
        {
            //Calculate
            string queryForLabels = Constants.DatabaseFields.LegendSymType + " = '" + Constants.DatabaseDomainsValues.SymTypeFill + "'";
            SortedDictionary<string, List<string>> idSymbols = Tables.GetAllDoubleUniqueFieldValuesFromWorkspace(inWorkspace, Constants.Database.TLegendGene, Constants.DatabaseFields.LegendLabelID, Constants.DatabaseFields.LegendSymbol, queryForLabels);

            ICursor geopolysCursor = polyTable.Update(null, false);
            IRow geopolyRow = geopolysCursor.NextRow();
            int geopolySymbolIndex = polyTable.FindField(legendSymbolFieldName);
            int geopolysIdIndex = polyTable.FindField(Constants.DatabaseFields.FGeopolyLabel);
            while (geopolyRow != null)
            {
                if (geopolyRow.Value[geopolysIdIndex] != null && idSymbols.ContainsKey(geopolyRow.Value[geopolysIdIndex].ToString()))
                {
                    geopolyRow.Value[geopolySymbolIndex] = idSymbols[geopolyRow.Value[geopolysIdIndex].ToString()][0];
                    geopolysCursor.UpdateRow(geopolyRow);
                }
                geopolyRow = geopolysCursor.NextRow();
            }

            ObjectManagement.ReleaseObject(geopolysCursor);
        }

        /// <summary>
        /// Will merge together polygon with same labelIDs and add them to a feature class.
        /// </summary>
        /// <param name="geopolyFC"></param>
        /// <param name="levels"></param>
        /// <param name="labelFieldName"></param>
        /// <param name="outputFC"></param>
        private void DoUnion(IFeatureClass geopolyFC, string levels, string labelFieldName, IFeatureClass outputFC)
        {
            //Build geometry bag object that will be used to merge overprints
            IGeometryBag geomBag = new GeometryBagClass();
            IGeometryCollection geomColl = geomBag as IGeometryCollection;

            //Create a spatial filter to select touching same overprints
            ISpatialFilter getIntersectingOP = new SpatialFilterClass();
            getIntersectingOP.WhereClause = labelFieldName + " ='" + levels + "'";
            getIntersectingOP.SpatialRel = esriSpatialRelEnum.esriSpatialRelTouches;

            //Get a search cursor to retrieve touching overprints
            IFeatureCursor geopolySearchCursor = geopolyFC.Search(getIntersectingOP, false);
            IFeature overprintFeature = geopolySearchCursor.NextFeature();
            //string sourceID = string.Empty;
            //int sourceIDFieldIndex = geopolySearchCursor.Fields.FindField(geopolySourceID);
            //string remarks = string.Empty;
            //int remarkFieldIndex = geopolySearchCursor.Fields.FindField(geopolyRemark);
            while (overprintFeature!=null)
            {
                object missing = Type.Missing;
                //Add geometry to bag
                geomColl.AddGeometry(overprintFeature.Shape, ref missing, ref missing);
                //sourceID = overprintFeature.get_Value(sourceIDFieldIndex).ToString();
                //remarks = overprintFeature.get_Value(remarkFieldIndex).ToString();
                overprintFeature = geopolySearchCursor.NextFeature();
                
            }
            GSC_ProjectEditor.ObjectManagement.ReleaseObject(geopolySearchCursor);

            //Merge touching overprints collected in the geometry bag
            ITopologicalOperator unionedPoly = new PolygonClass();
            unionedPoly.ConstructUnion(geomBag as IEnumGeometry);

            //Insert the new polygons in the output feature class
            IFeatureCursor insertNewPoly = outputFC.Insert(true);
            IFeatureBuffer newFeat = outputFC.CreateFeatureBuffer();
            newFeat.Shape = unionedPoly as IGeometry;
            int labelFieldIndex = newFeat.Fields.FindField(labelFieldName);
            newFeat.set_Value(labelFieldIndex, levels);
            //newFeat.set_Value(sourceIDFieldIndex, sourceID);
            //newFeat.set_Value(remarkFieldIndex, remarks);
            insertNewPoly.InsertFeature(newFeat);
            GSC_ProjectEditor.ObjectManagement.ReleaseObject(insertNewPoly);

        }

        /// <summary>
        /// Will return a list of each overprint levels inside label feature.
        /// An empty list indicates no overprint at all.
        /// </summary>
        /// <param name="labelFC">The label feature to look into.</param>
        /// <returns></returns>
        private List<string> GetOverprintLevels(IFeatureClass labelFC, IWorkspace inWorkspace)
        {
            //Variable
            overprintLevels = new List<string>();

            //Get a list
            Dictionary<string, string> domCodes = GSC_ProjectEditor.Domains.GetDomDicoFromWorkspace(inWorkspace, mapUnitDom, "Code");
            SortedDictionary<string, string> domCodesOverprint = new SortedDictionary<string, string>();
            foreach (KeyValuePair<string, string> muCodes in domCodes)
            {
                if (muCodes.Value.Contains(GSC_ProjectEditor.Constants.ValueKeywords.labelOverprint))
                {
                    if (!domCodesOverprint.ContainsKey(muCodes.Value))
                    {
                        domCodesOverprint[muCodes.Value] = muCodes.Key;
                    }
                }
            }

            foreach (KeyValuePair<string, string> sortedOverprints in domCodesOverprint)
            {
                overprintLevels.Add(sortedOverprints.Value);    
            }

            return overprintLevels;
        }

        protected override void OnUpdate()
        {
            //Enabled = ArcMap.Application != null;
        }

        /// <summary>
        /// Update geoline symbols from project style file.
        /// </summary>
        private void updateStyle()
        {
            try
            {
                //Valide style
                string pathToStyle = GSC_ProjectEditor.Symbols.ValidateStyleFile2(ArcMap.Document);
                if (pathToStyle!=string.Empty)
                {
                    ITable geopolyTable = Tables.OpenTable(Constants.Database.FGeopoly);
                    int polySymbolField = geopolyTable.FindField(Constants.DatabaseFields.LegendSymbol);
                    bool existingSymbolField = false;
                    if (polySymbolField > 0)
                    {
                        existingSymbolField = true;
                    }
                    string interGroupLayer = GSC_ProjectEditor.Properties.Resources.GroupLayerInterpretation;

                    //Get geopoly feature layer
                    IFeatureLayer getLayer = Utilities.MapDocumentFeatureLayers.GetFeatureLayerPreProcessing(geopoly, interGroupLayer);

                    //Access other attributes of layer with geofeaturelayer
                    IGeoFeatureLayer getGeoFL = getLayer as IGeoFeatureLayer;

                    //Get table 
                    ITable getTable = GSC_ProjectEditor.Tables.OpenTable(tLegend);

                    //Validate if a join exists, if not add it
                    bool featHasJoin = GSC_ProjectEditor.Joins.HasJoin(getLayer, geopolyLabel);
                    if (featHasJoin == false && existingSymbolField == false)
                    {
                        //Force a join on the layer
                        IDataset currentDataset = getLayer.FeatureClass as IDataset;
                        IWorkspace currentWorkspace = currentDataset.Workspace;
                        GSC_ProjectEditor.Joins.AddJoinsFromExistingRelationship(GSC_ProjectEditor.Constants.Database.rel_tLegend_fGeopoly, currentWorkspace, getLayer);
                    }

                    //Get unique list of labels within feature
                    List<string> uniqueLabelList = GSC_ProjectEditor.Tables.GetUniqueFieldValues(geopoly, geopolyLabel, null, false, null)[dicoMain];

                    //Sort list to get map unit in alphabetical order
                    uniqueLabelList.Sort();

                    //Get dictionnary of label codes and description from the domain itself
                    //Dictionary<string, string> labelDico = GSC_ProjectEditor.Domains.GetDomDico(GSC_ProjectEditor.Constants.DatabaseDomains.MapUnit, "Code");
                    Dictionary<string, string> labelDico = GSC_ProjectEditor.Tables.GetUniqueDicoValues(tLegend, tLegendItemID, GSC_ProjectEditor.Constants.DatabaseFields.LegendGISDisplay, string.Empty);

                    //Verify if layer is already symbolized with wanted style, if not, will create new renderer for it
                    validatePolyRenderer(getGeoFL, pathToStyle, existingSymbolField);

                    //Get symbol field indexes
                    ILayerFields getLayerFields = getLayer as ILayerFields; //Access this object to get index with a join inside feature layer
                    int symIndex = getLayerFields.FindField(joinTLegendSymbol);
                    int geoPolyDisplayIndex = getLayerFields.FindField(joinTLegendDisplayLabel);
                    int geoPolyIndex = getLayerFields.FindField(joinGeopolyLabel);
                    if (existingSymbolField)
                    {
                        symIndex = getLayerFields.FindField(tLegendSymbol);
                        geoPolyDisplayIndex = getLayerFields.FindField(Constants.DatabaseFields.LegendGISDisplay);
                        geoPolyIndex = getLayerFields.FindField(geopolyLabel);
                    }

                    //Loop through feature to match symbol field value with wanted style
                    IFeatureCursor cursorLayer = getGeoFL.SearchDisplayFeatures(null, true);  //Access a special object that will enable search within a join
                    IRow polyRows = null;
                    List<string> processedValues = new List<string>();
                    while ((polyRows = cursorLayer.NextFeature()) != null)
                    {

                        //Get current symbol field value
                        string currentSymValue = polyRows.get_Value(symIndex).ToString();

                        //Get matching legend description 
                        string currentGeopolyID = polyRows.get_Value(geoPolyIndex).ToString();

                        //Only process desired geopoly ids
                        if (uniqueLabelList.Contains(currentGeopolyID))
                        {
                            //Get a proper descriptive label (instead of code)
                            if (labelDico.ContainsKey(currentGeopolyID) && !processedValues.Contains(currentGeopolyID))
                            {
                                string descLabel = labelDico[currentGeopolyID];

                                //Validate if symbol is already loaded within layer (will add it if not there)
                                validatePolyInRenderer(getGeoFL, descLabel, currentSymValue, currentGeopolyID, pathToStyle);

                                processedValues.Add(currentGeopolyID);
                            }

                        }

                    }

                    try
                    {

                        //Release com cursor
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(cursorLayer);
                    }
                    catch (Exception)
                    {

                    }

                    

                }


            }
            catch(Exception MapUnitUpdateStyleError)
            {
                MessageBox.Show("MapUnitUpdateStyleError: " + MapUnitUpdateStyleError.Message + MapUnitUpdateStyleError.StackTrace);
            }


        }

        ///<summary>
        ///This method will help detect if current layer is symbolized correctly.
        ///Will add a custom renderer if nothing exists
        ///</summary>
        /// <param name="layerToAddRenderer">the layer object to verify renderer from</param>
        public void validatePolyRenderer(IGeoFeatureLayer layerToAddRenderer, string polyStylePath, bool existingSymbolField, bool forceRendererRebuild = false)
        {
            try
            {
                //Get the unique renderer style from 
                IUniqueValueRenderer getUniqueRender = layerToAddRenderer.Renderer as UniqueValueRenderer;

                //Get proper interface unique id
                string currentUID = layerToAddRenderer.RendererPropertyPageClassID.Value.ToString();
                string wantedUID = GSC_ProjectEditor.Constants.GUIDs.UIDSymbolUniqueValuesMultipleField;

                //Count entities from current renderer
                int count = 0;
                int fieldCount = 0;
                bool noRenderer = false; //A joker bool value, this will be used in case arc map says currentUID and wantedUID are the same, when they are not...
                try
                {
                    count = getUniqueRender.ValueCount;
                    fieldCount = getUniqueRender.FieldCount;
                }
                catch
                {
                    noRenderer = true;
                }

                //If the renderer is not set on the right interface redo from scratch, or Arc tells that the UID is the right one but the renderer doesn't exists.
                if (wantedUID != currentUID || noRenderer == true || forceRendererRebuild)
                {

                    //Force interface to unique values multiple fields renderer
                    UID pUID = new UIDClass();
                    pUID.Value = wantedUID;
                    layerToAddRenderer.RendererPropertyPageClassID = pUID as UIDClass;

                    //Create a renderer for style
                    getUniqueRender = new UniqueValueRenderer();

                    //Get symbol field indexes
                    int symIndex = layerToAddRenderer.FeatureClass.FindField(joinTLegendSymbol);
                    int geopolyLabelIndex = layerToAddRenderer.FeatureClass.FindField(joinGeopolyLabel);

                    if (existingSymbolField)
                    {
                        symIndex = layerToAddRenderer.FeatureClass.FindField(tLegendSymbol);
                        geopolyLabelIndex = layerToAddRenderer.FeatureClass.FindField(geopolyLabel);
                    }

                    //Set some renderer properties
                    SetRendererProperties(getUniqueRender, polyStylePath, existingSymbolField);

                }
                else
                {

                    //Detect if the renderer is set on the right field
                    if (getUniqueRender.FieldCount != 2)
                    {
                        SetRendererProperties(getUniqueRender, polyStylePath, existingSymbolField);
                    }

                }

                //Persist change
                layerToAddRenderer.Renderer = getUniqueRender as IFeatureRenderer;

                ////Refresh TOC
                //ArcMap.Document.ActivatedView.ContentsChanged();
                //ArcMap.Document.UpdateContents();
                //ArcMap.Document.ActiveView.Refresh();  
                


            }
            catch (Exception validateLineRendererError)
            {
                MessageBox.Show("validateLineRendererError: " + validateLineRendererError.Message);
            }



        }

        /// <summary>
        /// Will validate existance of a geoline id inside an existing unique renderer from a given layer
        /// Will add new symbol if doesn't exist, will skip if exists.
        /// </summary>
        /// <param name="layerToAddSymbol">the layer to add a new symbol to</param>
        /// <param name="descriptiveLabelToAdd">the new symbol name to add into layer</param>
        public void validatePolyInRenderer(IGeoFeatureLayer layerToAddSymbol, string descriptiveLabelToAdd, string symbolCode, string labelCode, string stylePath)
        {
            try
            {
                //Get the unique renderer style from 
                IUniqueValueRenderer getUniqueRender_ = layerToAddSymbol.Renderer as UniqueValueRenderer;

                //Create the new unique value
                string newUniqueValue = symbolCode + GSC_ProjectEditor.Constants.Symbol4Layers.fieldDelimeter + labelCode;

                //Validate existance of geopoly id within symbolized layer
                bool symbolDetected = false;
                bool duplicateLabel = false;
                List<string> duplicateValue = new List<string>();

                //Get number of current symbols within layer
                int idCount = getUniqueRender_.ValueCount;

                //Detect if symbol exists
                for (int idxValue = 0; idxValue < idCount; idxValue++)
                {
                    string value = getUniqueRender_.get_Value(idxValue);
                    string label = getUniqueRender_.get_Label(value);
                    if (value == newUniqueValue)
                    {
                        symbolDetected = true;
                        //break;
                    }
                    if (label == descriptiveLabelToAdd && value != newUniqueValue)
                    {
                        duplicateLabel = true;
                        duplicateValue.Add(value);
                    }
                }

                //If nothing was detected add it
                if (symbolDetected == false)
                {
                    //Get match symbol
                    if (symbolCode != "")
                    {

                        //Create a new empty fill symbol
                        IFillSymbol symx = null;
                        getUniqueRender_.AddValue(newUniqueValue, GSC_ProjectEditor.Constants.Symbol4Layers.geopolyLabelFieldName, symx as ISymbol);
                        getUniqueRender_.Label[newUniqueValue] = descriptiveLabelToAdd; //Apply legend description as label
                        IFillSymbol symy = Utilities.MapDocumentSymbol.GetMatchingPolygonSymbol(symbolCode, stylePath);

                        //Set symbol to the match
                        if (symy != null)
                        {
                            getUniqueRender_.Symbol[newUniqueValue] = symy as ISymbol;  
                        }
                        

                    }

                }
                else
                {
                    //Else refresh it
                    if (!Properties.Settings.Default.KeepCustomSymbols && symbolCode != string.Empty)
                    {
                        IFillSymbol symy = Utilities.MapDocumentSymbol.GetMatchingPolygonSymbol(symbolCode, stylePath);

                        //Set symbol to the match
                        if (symy != null)
                        {
                            getUniqueRender_.Symbol[newUniqueValue] = symy as ISymbol;
                        }
                    }

                }

                //If a duplicate is found, remove it
                if (duplicateLabel && duplicateValue.Count>0)
                {
                    //Set symbol to the match
                    foreach (string values in duplicateValue)
                    {
                        getUniqueRender_.RemoveValue(values);
                    }
                    
                }


            }
            catch (Exception validatePolyInRendererError)
            {
                MessageBox.Show("validatePolyInRendererError:" + validatePolyInRendererError.Message + ": " + validatePolyInRendererError.StackTrace);
            }


        }

        /// <summary>
        /// Will return a feature layer based.
        /// </summary>
        /// <param name="Overprint">Specify if requested layer is for overprint or no overprint. Special queries are made to select all overprints or no overprints within labels.</param>
        /// <returns></returns>
        public IFeatureClass ProcessLabels(IFeatureClass inFC, string opLevel, out bool doesHaveOverprints, IWorkspace inWorkspace)
        {
            //Variables
            List<string> wantedCodes = new List<string>(); //A list of wanted domain codes to get a selection from
            doesHaveOverprints = false; //will be used to break the sequence if there is no overprints labels
            IFeatureClass outputLabels; //The output fc of labels
            string interGroupLayer = GSC_ProjectEditor.Properties.Resources.GroupLayerInterpretation;

            //Cast current FC to FL
            IFeatureLayer inFL = new FeatureLayer();
            inFL.FeatureClass = inFC;

            //Get list of dom codes and description from domain
            Dictionary<string, string> domCodes = GSC_ProjectEditor.Domains.GetDomDicoFromWorkspace(inWorkspace, mapUnitDom, "Code");

            //Parse result based on wanted values
            foreach (KeyValuePair<string, string> kv in domCodes)
            {
                if (kv.Value.Contains(GSC_ProjectEditor.Constants.ValueKeywords.labelOverprint))
                {
                    doesHaveOverprints = true;
                }

                if (opLevel != null)
                {
                    if (kv.Key == opLevel)
                    {
                        wantedCodes.Add(kv.Key);
                    }

                }
                else
                {
                    if (!kv.Value.Contains(GSC_ProjectEditor.Constants.ValueKeywords.labelOverprint))
                    {
                        wantedCodes.Add(kv.Key);
                    }
                }
            }

            //If result doesn't have overprints
            if (doesHaveOverprints)
            {
                
                #region has overprints
                //Get featurelayer from map
                IFeatureLayer currentLabelLayer = Utilities.MapDocumentFeatureLayers.GetFeatureLayerPreProcessing(fcLabel, interGroupLayer);

                //Select proper rows within feature layer
                GSC_ProjectEditor.FeatureLayers.SelectFeatureLayerFromList(currentLabelLayer, fcLabelID, wantedCodes, false);

                //Create a new label feature from selection
                string newFCName = string.Empty;
                if (opLevel != null)
                {
                    newFCName = outputLabel_OverprintOnly;
                }
                else
                {
                    newFCName = outputLabel_NoOverprint;
                }

                //Copy selection to a new feature class
                string outputFCPath = GSC_ProjectEditor.GeoProcessing.CopyFeatures(currentLabelLayer, newFCName);

                //Cast copy as feature class
                outputLabels = GSC_ProjectEditor.FeatureClass.OpenFeatureClassFromInMemory(outputFCPath);

                //Remove any selection from label original feature
                IFeatureSelection currentFeatureSelection = currentLabelLayer as IFeatureSelection;
                currentFeatureSelection.Clear();

                #endregion

            }
            else
            {
                //Return input because there is no overprint found
                outputLabels = inFC;
            }


            return outputLabels;

        }

        /// <summary>
        /// Will delete requested temp feature class
        /// </summary>
        /// <param name="deleteFC">The feature class to delete</param>
        public void deleteTempFeatures(IFeatureClass deleteFC)
        {
            //Make sure the feature class to delete is really a temp one
            IDataset datasetDelete = deleteFC as IDataset;
            IWorkspace fcWorkspace = datasetDelete.Workspace;

            //If the layer comes from in_memory workspace, a proper keyword should be found.
            if (fcWorkspace.PathName.Contains(GSC_ProjectEditor.Constants.ValueKeywords.esriInMemory))
            {
                //Delete
                GSC_ProjectEditor.FeatureClass.DeleteFeatureClass(deleteFC); 
            }


        }

        /// <summary>
        /// Will apply geoline specific properties to input renderer
        /// </summary>
        /// <param name="inRenderer">The renderer to modify</param>
        /// <param name="defaultSymbol">The default symbol for default values</param>
        public void SetRendererProperties(IUniqueValueRenderer inRenderer, string rendererStylePath, bool existingSymbolField)
        {

            IUniqueValueRenderer geolineRenderer = GSC_ProjectEditor.Symbols.SetGeopolyRendererProperties(inRenderer, rendererStylePath);
            if (geolineRenderer !=null)
            {
                inRenderer.FieldCount = 2;//Only one field will determine symbol
                if (existingSymbolField)
                {
                    inRenderer.set_Field(0, tLegendSymbol); //Set the field to sym field 
                    inRenderer.set_Field(1, geopolyLabel); //Set the second field to be geolineID
                }
                else
                {
                    inRenderer.set_Field(0, joinTLegendSymbol); //Set the field to sym field 
                    inRenderer.set_Field(1, joinGeopolyLabel); //Set the second field to be geolineID
                }

            }


        }

        /// <summary>
        /// Will refresh the symbols from the layer and the templates
        /// </summary>
        public void RefreshMapUnitsSymbols()
        {
            //Recreate templates; it'll also trigger layer refresh from within
            updateStyle();

            //Refresh TOC
            ArcMap.Document.ActivatedView.ContentsChanged();
            ArcMap.Document.UpdateContents();
            ArcMap.Document.ActiveView.Refresh();
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
