using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Catalog;
using ESRI.ArcGIS.Geometry;

namespace GSC_ProjectEditor
{
    public class FeatureLayers
    {
        #region GET methods

        /// <summary>
        /// Gets a group layer object from TOC
        /// </summary>
        /// <param name="inputGroupName"></param>
        /// <returns></returns>
        public static IGroupLayer GetGroupLayer(string inputGroupName, IMap currentMap)
        {
            //Create an empty group layer object
            IGroupLayer getGroup = new GroupLayer();

            //Get Group layer objects from GUID
            UID groupUID = new UIDClass();
            groupUID.Value = GSC_ProjectEditor.Constants.GUIDs.UIDGroupLayer;

            //Iterate through group layer and find the one.
            IEnumLayer groupLayers = currentMap.get_Layers(groupUID, true);
            ILayer groupLayer = groupLayers.Next();
            while (groupLayer != null)
            {
                if (groupLayer.Name == inputGroupName)
                {
                    getGroup = groupLayer as IGroupLayer;
                }

                groupLayer = groupLayers.Next();
            }

            return getGroup;
        }

        /// <summary>
        /// Get a copy of a layer file, anywhere on the computer
        /// </summary>
        /// <param name="relativePathToLyr">Reference path to a .lyr file</param>
        /// <returns></returns>
        public static ILayer GetLyrFileFromComputer(string pathToLyr)
        {
            //Create a new GxLayer object
            IGxLayer gxLayer = new GxLayer();

            //Create a new GxFile object from cast
            IGxFile gxFile = gxLayer as IGxFile;

            //Set path
            gxFile.Path = pathToLyr;

            //Return
            return gxLayer.Layer;
        }

        /// <summary>
        /// Get a feature layer object within Arc Map Table of Content, for project geodatabase
        /// </summary>
        /// <param name="featureClassName"></param>
        /// <param name="featureLayerName">Feature class name to retrieve associated feature layer from</param>
        /// <param name="groupLayerName">Group layer in which a feature layer is needed, if null, will parse entire TOC</param>
        /// <param name="getCurrentMap">A map object of current mxd to get feature layer from</param>
        /// <returns></returns>
        public static IFeatureLayer GetFeatureLayer(string featureClassName, string groupLayerName, IMap getCurrentMap)
        {
            //New feature layer to return
            IFeatureLayer wantedFL = new FeatureLayer();

            //Get workspace
            IWorkspace wantedWorkspace = GSC_ProjectEditor.Workspace.AccessWorkspace(GSC_ProjectEditor.Workspace.GetDBPath());

            wantedFL = GetFeatureLayerFromWorkspace(wantedWorkspace, featureClassName, groupLayerName, getCurrentMap);

            return wantedFL;
        }

        /// <summary>
        /// Get a feature layer object within group layer or entire TOC, from a given workspace
        /// </summary>
        /// <param name="flWorkspace">Input feature workspace to get feature class layer from arc map</param>
        /// <param name="featureClassName">Feature class name to retrieve associated feature layer from</param>
        /// <param name="groupLayer">Group layer in which a feature layer is needed, if null, will parse entire TOC</param>
        /// <param name="getCurrentMap">A map object of current mxd to get feature layer from</param>
        /// <returns></returns>
        public static IFeatureLayer GetFeatureLayerFromWorkspace(IWorkspace flWorkspace, string featureClassName, string groupLayerName, IMap getCurrentMap)
        {
            //New feature layer to return
            IFeatureLayer wantedFL = new FeatureLayer();

            //Get a validation keyword
            string valKeyword = GSC_ProjectEditor.Constants.Layers.Keyword;

            try
            {

                //Get feature class
                IFeatureClass wantedFC = GSC_ProjectEditor.FeatureClass.OpenFeatureClassFromWorkspace(flWorkspace, featureClassName);

                //Get feature layer object GUID
                UID flUID = new UIDClass();
                flUID.Value = GSC_ProjectEditor.Constants.GUIDs.UIDFeatureLayer;

                //Get wanted group layer and cast as composite layer
                IGroupLayer groupLayer = GetGroupLayer(groupLayerName, getCurrentMap);
                ICompositeLayer compoLayer = groupLayer as ICompositeLayer;

                //Iterate through Composite layers and find wanted layer
                if (groupLayer != null)
                {
                    for (int iLayer = 0; iLayer < compoLayer.Count; iLayer++)
                    {
                        //Variables
                        bool sourcePathExist = true;

                        //Current sub layer
                        ILayer currentLayer = compoLayer.Layer[iLayer];

                        //Validate if current layer has a known source path
                        try
                        {
                            if (((IFeatureLayer)currentLayer).FeatureClass.Equals(wantedFC))
                            {
                                sourcePathExist = true;
                            }
                        }
                        catch (Exception noSource)
                        {
                            sourcePathExist = false;

                        }

                        //Parse features
                        if (sourcePathExist)
                        {
                            if (((IFeatureLayer)currentLayer).FeatureClass.Equals(wantedFC) && !currentLayer.Name.Contains(valKeyword))
                            {

                                wantedFL = (IFeatureLayer)currentLayer;
                                break;
                            }
                        }
                    }
                }
                else
                {
                    //Iterate trhough TOC and find wanted layer
                    IEnumLayer TOCLayers = getCurrentMap.get_Layers(flUID, true);
                    ILayer currentFeatureLayer = TOCLayers.Next();

                    //Find wanted layer
                    while (currentFeatureLayer != null)
                    {
                        bool sourcePathExist = true;

                        //Validate if current layer has a known source path
                        try
                        {
                            if (((IFeatureLayer)currentFeatureLayer).FeatureClass.Equals(wantedFC))
                            {
                                sourcePathExist = true;
                            }
                        }
                        catch (Exception noSource)
                        {
                            sourcePathExist = false;

                        }

                        //Parse features
                        if (sourcePathExist)
                        {
                            if (((IFeatureLayer)currentFeatureLayer).FeatureClass.Equals(wantedFC) && !currentFeatureLayer.Name.Contains(valKeyword))
                            {

                                wantedFL = (IFeatureLayer)currentFeatureLayer;
                                break;
                            }
                        }

                        //Next
                        currentFeatureLayer = TOCLayers.Next();
                    }
                }

            }

            catch (Exception GetFeatureLayerFromWorkspaceError)
            {
                int lineNumber = Exceptions.LineNumber(GetFeatureLayerFromWorkspaceError);
                MessageBox.Show("GetFeatureLayerFromWorkspaceError ( " + lineNumber.ToString() + "): " + GetFeatureLayerFromWorkspaceError.Message);
            }

            return wantedFL;
        }

        /// <summary>
        /// Will return a feature layer real name
        /// </summary>
        /// <param name="inputFL">The feature layer to retrieve name from</param>
        /// <returns></returns>
        public static string GetFeatureLayerRealName(IFeatureLayer inputFL)
        {
            //Get related feature class from layer
            IFeatureClass inputFC = inputFL.FeatureClass;

            //Get name from method
            return FeatureClass.GetFeatureClassName(inputFC);

        }

        /// <summary>
        /// Will return a list of selected feature from a given string name and an map object
        /// </summary>
        /// <param name="featureLayerName">The feature layer to retrieve selection from, only the name is required</param>
        /// <param name="getCurrentMap">The map object to get the layer from</param>
        /// <returns></returns>
        public static List<IFeature> GetSelectedFeatures(string featureClassName, string featureLayerName, IMap getCurrentMap)
        {
            //Variables
            List<IFeature> featureList = new List<IFeature>();

            

            //Get wanted layer object
            IFeatureLayer getFL = GetFeatureLayer(featureClassName, featureLayerName, getCurrentMap);

            //Create a feature selection object from layer
            IFeatureSelection getFLS = getFL as IFeatureSelection;

            //Get the selection set from layer
            ISelectionSet getSS = getFLS.SelectionSet;

            //Iterate through selection set
            if (getSS!=null)
            {
                ICursor selectionCursor;
                getSS.Search(null, false, out selectionCursor);
                IFeatureCursor featCursor = selectionCursor as IFeatureCursor;
                IFeature currentFeat;
                while ((currentFeat = featCursor.NextFeature()) != null)
                {
                    featureList.Add(currentFeat);
                }
            }


            return featureList;

        }

        /// <summary>
        /// Will return a list of selected feature from a given string name and an map object
        /// </summary>
        /// <param name="featureLayerName">The feature layer to retrieve selection from, only the name is required</param>
        /// <param name="getCurrentMap">The map object to get the layer from</param>
        /// <returns></returns>
        public static List<IFeature> GetSelectedFeaturesFromWorkspace(IWorkspace flWorkspace, string featureClassName, string featureLayerName, IMap getCurrentMap)
        {
            //Variables
            List<IFeature> featureList = new List<IFeature>();

            //Get wanted layer object
            IFeatureLayer getFL = GetFeatureLayerFromWorkspace(flWorkspace, featureClassName, featureLayerName, getCurrentMap);

            //Create a feature selection object from layer
            IFeatureSelection getFLS = getFL as IFeatureSelection;

            //Get the selection set from layer
            ISelectionSet getSS = getFLS.SelectionSet;

            //Iterate through selection set
            ICursor selectionCursor;
            getSS.Search(null, false, out selectionCursor);
            IFeatureCursor featCursor = selectionCursor as IFeatureCursor;
            IFeature currentFeat;
            while ((currentFeat = featCursor.NextFeature()) != null)
            {
                featureList.Add(currentFeat);
            }

            return featureList;

        }

        #endregion

        #region SET methods

        /// <summary>
        /// Changes datasource within a layer within Arc Map Table of Content
        /// </summary>
        /// <param name="featureClassName">Feature class name to set datasource of</param>
        /// <param name="featureDatasetName">Feature dataset that contains feature class to set datasoure from</param>
        /// <param name="layerToResetSource">The laye object that reference the feature class to set datasource from</param>
        /// <param name="arcMapObjects">A list of objects from arc map that can only be retrieved from an addin project: [0]=IMap, [1]=IActiveView, [2]=IContentsView</param>
        /// <returns></returns>
        public static void SetDataSource(string featureClassName, string featureDatasetName, ILayer layerToResetSource, List<object> arcMapObjects)
        {
            //Get current workspace
            string getDatabasePath = GSC_ProjectEditor.Workspace.GetDBPath();
            IWorkspace getWorkspace = GSC_ProjectEditor.Workspace.AccessWorkspace(getDatabasePath);

            //Cast workspace to featureWorkspace
            IFeatureWorkspace getFeatureWorkspace = getWorkspace as IFeatureWorkspace;

            //Get inside feature class
            IFeatureDataset getFeatureDataset = getFeatureWorkspace.OpenFeatureDataset(featureDatasetName);
            IFeatureClassContainer getFeatureClassContainer = getFeatureDataset as IFeatureClassContainer;
            IFeatureClass newFeatureClass = getFeatureClassContainer.get_ClassByName(featureClassName);

            //Get active view
            IMap map = arcMapObjects[0] as IMap;
            IActiveView activeView = arcMapObjects[1] as IActiveView;

            //Get map
            IMapAdmin2 mapAdmin2 = map as IMapAdmin2;

            //Get layer within map
            ILayer layer = map.get_Layer(0); //TODO change this to search for input layer

            //Create feature layer
            IFeatureLayer featureLayer = layer as IFeatureLayer;

            //Get old path
            IFeatureClass oldFeatureClass = featureLayer.FeatureClass;

            //Reset paths
            featureLayer.FeatureClass = newFeatureClass;
            mapAdmin2.FireChangeFeatureClass(oldFeatureClass, newFeatureClass);

            //Refresh view and TOC
            activeView.Refresh(); //For active view
            IContentsView tocView = arcMapObjects[2] as IContentsView; //For table of content
            tocView.Refresh(null);

        }

        #endregion

        #region CREATE methods

        /// <summary>
        /// Create a selection layer from a list of values.
        /// </summary>
        /// <param name="inputFL">Reference to a feature layer object</param>
        /// <param name="inputFCName">Input feature layer related feature class name, for join field name management.</param>
        /// <param name="selectionValueList"> A list of values to select</param>
        /// <param name="valueField">Field name related to list of values</param>
        /// <param name="newLayerName">The output new feature layer name</param>
        /// <returns></returns>
        public static IFeatureLayer CreateFeatureLayerFromSelection(IFeatureLayer inputFL, string inputFCName, List<string> selectionValueList, string valueField, string newLayerName, bool numbers)
        {
            List<string> exceptionList = new List<string>();


            try
            {

                //Create a selection object to extract polygons
                IFeatureSelection featureSelect = inputFL as IFeatureSelection;

                //Make sure nothing is already selected
                featureSelect.Clear();

                //Validate for any join if exists, rename valueField
                //Intented for queryFilter only.
                string newValueField = "";
                bool featHasJoin = Joins.HasJoin(inputFL, valueField);
                if (featHasJoin)
                {
                    //New names for fields
                    newValueField = inputFCName + "." + valueField;
                }
                else
                {
                    newValueField = valueField;

                }

                //Cast in dataset to retrieve workspace and call a method to get a query from list of values.
                IDataset inDataset = inputFL.FeatureClass as IDataset;

                //Create a query filter for selection
                IQueryFilter selectionFilter = new QueryFilter();

                if (numbers)
                {

                    selectionFilter.WhereClause = GSC_ProjectEditor.Queries.BuildQueryFromStringList(newValueField, selectionValueList, "Int", "OR", inDataset.Workspace);
                }
                else
                {
                    selectionFilter.WhereClause = GSC_ProjectEditor.Queries.BuildQueryFromStringList(newValueField, selectionValueList, "String", "OR", inDataset.Workspace);
                }

                try
                {
                    featureSelect.SelectFeatures(selectionFilter, esriSelectionResultEnum.esriSelectionResultAdd, false);
                }
                catch (Exception)
                {

                }
                

                //Create a new feature from selection
                IFeatureLayerDefinition selectionFLD = inputFL as IFeatureLayerDefinition;
                IFeatureLayer newFL = selectionFLD.CreateSelectionLayer(newLayerName, true, null, null);

                //Make sure nothing is selected after process
                featureSelect.Clear();

                return newFL;
            }
            catch (Exception getFeatureLayerSelectException)
            {
                GSC_ProjectEditor.Exceptions.WriteToTextFile(exceptionList.ToArray());
                MessageBox.Show("CreateFeatureLayerFromSelection: " + getFeatureLayerSelectException.StackTrace);
                return null;
            }


        }

        /// <summary>
        /// Create a selection layer from a list of values.
        /// </summary>
        /// <param name="inputFL">Reference to a feature layer object</param>
        /// <param name="inputFCName">Input feature layer related feature class name, for join field name management.</param>
        /// <param name="selectionValueList"> A list of values to select</param>
        /// <param name="valueField">Field name related to list of values</param>
        /// <param name="newLayerName">The output new feature layer name</param>
        /// <returns></returns>
        public static IFeatureLayer CreateFeatureLayerFromSelection2(IFeatureLayer inputFL, string inputFCName, string selectionValue, string valueField, string newLayerName)
        {
            try
            {

                //Create a selection object to extract polygons
                IFeatureSelection featureSelect = inputFL as IFeatureSelection;

                //Make sure nothing is already selected
                featureSelect.Clear();

                //Validate for any join if exists, rename valueField
                //Intented for queryFilter only.
                string newValueField = "";
                bool featHasJoin = Joins.HasJoin(inputFL, valueField);
                if (featHasJoin)
                {
                    //New names for fields
                    newValueField = inputFCName + "." + valueField;
                }
                else
                {
                    newValueField = valueField;

                }

                //Create a query filter for selection
                IQueryFilter selectionFilter = new QueryFilter();

                //Apply where clause inside filter (pass a join field name instead of normal field name)
                selectionFilter.WhereClause = "\"" + newValueField + "\" = " + selectionValue;

                //Apply selection
                featureSelect.SelectFeatures(selectionFilter, esriSelectionResultEnum.esriSelectionResultAdd, true);

                //Create a new feature from selection
                IFeatureLayer selectionFL = featureSelect as IFeatureLayer;
                IFeatureLayerDefinition selectionFLD = selectionFL as IFeatureLayerDefinition;

                IFeatureLayer newFL = selectionFLD.CreateSelectionLayer(newLayerName, false, null, null);

                //Set the def query afterward, or else it won't show up - ticket 6151
                IFeatureLayerDefinition secondDef = newFL as IFeatureLayerDefinition;
                secondDef.DefinitionExpression = "\"" + newValueField + "\" = " + selectionValue;

                //Make sure nothing is selected after process
                featureSelect.Clear();

                return newFL;
            }
            catch (Exception getFeatureLayerSelectException)
            {
                MessageBox.Show("CreateFeatureLayerFromSelection: " + Exceptions.LineNumber(getFeatureLayerSelectException).ToString() + " --> " + getFeatureLayerSelectException.Message);
                return null;
            }


        }

        /// <summary>
        /// Will create an empty group layer, intended to fill it with more feature layers.
        /// </summary>
        /// <param name="groupLayerName">The new group layer name</param>
        /// <returns></returns>
        public static IGroupLayer CreateEmptyGroupLayer(string groupLayerName)
        {
            //Create the new group layer
            IGroupLayer groupLayer = new GroupLayerClass();

            //Assign name
            groupLayer.Name = groupLayerName;

            return groupLayer;

        }

        /// <summary>
        /// Will return a feature layer created from a table and it's associated feature class in which the coordinates will be taken.
        /// </summary>
        /// <param name="inputTableView">The table to convert into a feature layer</param>
        /// <param name="inXField">The X coordinate field inside the table</param>
        /// <param name="inYField">The Y coordinate field inside the table</param>
        /// <param name="inZField">The Z coordinate field inside the table</param>
        /// <param name="inputFC">The feature class in which the spatial reference will be taken</param>
        /// <param name="outputFLName">The new feature layer name to appear in Arc Map TOC</param>
        /// <returns></returns>
        public static IXYEventSource CreateXYEvent(ITable inputTable, Tuple<string, string, string> coordinateFields, Tuple<string, string, string> joinFields, IFeatureClass inputFC, string outputFLName)
        {

            //Variables
            IDataLayer outputXYFL = inputTable as IDataLayer;

            //Do a join with the input FC to get the correct fields from
            ITable joinedTable = Joins.DoJoinTableFC(inputTable, inputFC, joinFields.Item1, joinFields.Item2, joinFields.Item3, outputFLName);

            //Get the table name object from the original table
            IDataset inputDataset = joinedTable as IDataset;
            IName inputTableName = inputDataset.FullName;
            string inputTableNameString = inputDataset.Name;

            //Get the feature name object from the original feature
            IDataset inputFCDataset = inputFC as IDataset;
            string inputFCNameString = inputFCDataset.Name;

            //Find correct field name; could be based with _ or . (usually it's a .)
            string XFieldDot = inputFCNameString + "." + coordinateFields.Item1; //Most frequent
            string YFieldDot = inputFCNameString + "." + coordinateFields.Item2; //Most frequent
            string XField = XFieldDot; //Default value
            string YField = YFieldDot; //Default value


            //Set the X and Y fields  (a join field name needs to be passed)
            IXYEvent2FieldsProperties XYFields = new XYEvent2FieldsPropertiesClass();
            XYFields.XFieldName = XField;
            XYFields.YFieldName = YField;
            if (coordinateFields.Item3 != null)
            {
                XYFields.ZFieldName = inputFCNameString + "." + coordinateFields.Item3;
            }

            //Get projection from the original feature class
            ISpatialReference fcSpatial = SpatialReferences.GetSpatialRef(inputFC);

            //Create the event name object in which all the configuration will go
            IXYEventSourceName eventSourceName = new XYEventSourceNameClass();

            //Assign properties to it
            eventSourceName.EventTableName = inputTableName;
            eventSourceName.EventProperties = XYFields;
            eventSourceName.SpatialReference = fcSpatial;

            //Create the event from above object
            IName eventName = eventSourceName as IName;
            IXYEventSource eventSource2 = eventName.Open() as IXYEventSource;

            return eventSource2;
            
        }

        #endregion

        #region ADD, REMOVE, SELECT

        /// <summary>
        /// Adds a new layer to Arc map
        /// </summary>
        /// <param name="inputLayer">Reference to wanted layer object to add</param>
        /// <param name="inputTargetGroupLayer">If required, specifie a target group layer to add layer to</param>
        /// <param name="arcMapObjects">A list of objects from arc map that can only be retrieved from an addin project: [0]=IMap, [1]=IActiveView, [2]=IContentsView</param>
        public static void AddLayerToArcMap(ILayer inputLayer, string inputTargetGroupLayer, List<object> arcMapObjects)
        {
            //Get current arc map document
            IMap map = arcMapObjects[0] as IMap;

            //Add
            if (inputTargetGroupLayer == null)
            {
                //Will add at the top of the TOC
                map.AddLayer(inputLayer);
            }
            else
            {
                //Get user wanted group layer
                IGroupLayer getTargetGroup = GetGroupLayer(inputTargetGroupLayer, map);

                //Add into it
                try
                {
                    getTargetGroup.Add(inputLayer);
                }
                catch (Exception addLayerException)
                {
                    MessageBox.Show(addLayerException.Message + "; " + addLayerException.StackTrace);
                }

            }

            //Refresh
            if (arcMapObjects.Count >=2)
            {
                IActiveView getActiveView = arcMapObjects[1] as IActiveView;
                getActiveView.Refresh();
            }

            //Refresh view and TOC
            if (arcMapObjects.Count >= 3)
            {
                IContentsView tocView = arcMapObjects[2] as IContentsView; //For table of content
                tocView.Refresh(null);
            }


        }

        /// <summary>
        /// Remove a layer from Arc Map
        /// </summary>
        /// <param name="inputLayerName">Wanted layer name to remove</param>
        /// <param name="getCurrentMap">Current arc map object to remove layer from</param>
        public static void RemoveLayerFromArcMap(string inputLayerName, IMap getCurrentMap)
        {

            //Get feature layer object GUID
            UID removeflUID = new UIDClass();
            removeflUID.Value = GSC_ProjectEditor.Constants.GUIDs.UIDFeatureLayer;

            //Iterate trhough TOC and find wanted layer
            IEnumLayer removeTOCLayers = getCurrentMap.get_Layers(removeflUID, true);
            ILayer removeFeatureLayer = removeTOCLayers.Next();

            //Find wanted layer
            while (removeFeatureLayer != null)
            {

                if (removeFeatureLayer.Name == inputLayerName)
                {
                    getCurrentMap.DeleteLayer(removeFeatureLayer);
                    break;
                }
                removeFeatureLayer = removeTOCLayers.Next();
            }

        }

        /// <summary>
        /// From a given list of codes, will make a selection within feature layer of those codes.
        /// </summary>
        /// <param name="inFL">The feature layer to select features from</param>
        /// <param name="fieldToQuery">The field name to select all the values from</param>
        /// <param name="inList">The list of values to select (OR statement will be used)</param>
        /// <param name="numbers">indicate whether the values or numbers or string caracters</param>
        public static void SelectFeatureLayerFromList(IFeatureLayer inFL, string fieldToQuery, List<string> inList, bool numbers)
        {

            //Create a selection object to extract polygons
            IFeatureSelection featureSelect = inFL as IFeatureSelection;

            //Make sure nothing is already selected
            featureSelect.Clear();

            //Create a query filter for selection
            IQueryFilter selectionFilter = new QueryFilter();

            foreach (string codes in inList)
            {
                int currentIndex = inList.IndexOf(codes);

                //Apply where clause inside filter (pass a join field name instead of normal field name)
                if (numbers)
                {
                    if (currentIndex == 0)
                    {
                        selectionFilter.WhereClause = fieldToQuery + " = " + codes;
                    }
                    else
                    {
                        selectionFilter.WhereClause = selectionFilter.WhereClause + " OR " + fieldToQuery + " = " + codes;
                    }
                    
                }
                else
                {
                    if (currentIndex == 0)
                    {
                        selectionFilter.WhereClause = fieldToQuery + " = '" + codes + "'";
                    }
                    else
                    {
                        selectionFilter.WhereClause = selectionFilter.WhereClause + " OR " + fieldToQuery + " = '" + codes + "'";

                    }

                }
            }

            featureSelect.SelectFeatures(selectionFilter, esriSelectionResultEnum.esriSelectionResultNew, false);

        }

        /// <summary>
        /// Will remove a given string value from a layer symbology that is set to Unique value renderer
        /// </summary>
        /// <param name="inLayerName"></param>
        /// <param name="inValue"></param>
        public static void RemoveSymbolValueFromLayer(string inLayerName, string inGrouplayer, string inValue, IMap getCurrentMap)
        {
            //Get the associated layer from the given map object
            IFeatureLayer inFL = GetFeatureLayer(inLayerName, inGrouplayer, getCurrentMap);

            //Cast to geolayer
            IGeoFeatureLayer inGeoFL = inFL as IGeoFeatureLayer;

            //Get the renderer in it
            IUniqueValueRenderer inUniqueRenderer = inGeoFL.Renderer as UniqueValueRenderer;

            //Find how many symbols are loaded (will validate unique value renderer at the same time)
            int symbolCount = 0;
            int fieldCount = 0;
            bool uniqueRenderer = true; //A joker bool value, this will be used in case arc map says currentUID and wantedUID are the same, when they are not...
            try
            {
                symbolCount = inUniqueRenderer.ValueCount;
                fieldCount = inUniqueRenderer.FieldCount;
            }
            catch
            {
                uniqueRenderer = false;
            }

            //Remove the wanted value
            if (uniqueRenderer)
            {
                try
                {
                    inUniqueRenderer.RemoveValue(inValue);
                }
                catch (Exception)
                {
                }
                
            }

            inGeoFL.Renderer = inUniqueRenderer as IFeatureRenderer;

        }

        #endregion
    }
}
