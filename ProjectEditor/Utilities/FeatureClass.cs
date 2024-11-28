using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Geoprocessing;
using ESRI.ArcGIS.esriSystem;

namespace GSC_ProjectEditor
{
    public class FeatureClass
    {
        #region OPEN METHODS

        /// <summary>
        /// To open a feature class
        /// </summary>
        /// <param name="inputWorkspace">Reference to a workspace object</param>
        /// <param name="inputFeature">Reference feature class name to open</param>
        /// <returns></returns>
        public static IFeatureClass OpenFeatureClassFromWorkspace(IWorkspace inputWorkspace, string inputFeature)
        {
            //Access workspace
            IFeatureWorkspace getWorkspace = (IFeatureWorkspace)inputWorkspace;

            IFeatureClass getFC = getWorkspace.OpenFeatureClass(inputFeature);

            getWorkspace = null;
            GC.Collect();

            //Return object
            return getFC;
        }

        /// <summary>
        /// Will return a feature class object from a full path reference (.shp and FCs)
        /// </summary>
        /// <param name="inputFeature"> Feature class full reference path</param>
        /// <returns></returns>
        public static IFeatureClass OpenFeatureClassFromString(string inputFeaturePath)
        {
            //Call the GPUtilities function to retrieve feature class (treats shapefiles and features)
            IGPUtilities gpUtil = new GPUtilitiesClass();
            IFeatureClass getInFeature = gpUtil.OpenFeatureClassFromString(inputFeaturePath);

            return getInFeature;
        }

        /// <summary>
        /// Will return a feature class object from a full path reference (.shp and FCs)
        /// </summary>
        /// <param name="inputFeature"> Table full reference path</param>
        /// <returns></returns>
        public static IFeatureClass OpenFeatureClassFromStringFaster(string inputFeaturePath)
        {
            //Get the original table
            ITable inputFeatureTable = GSC_ProjectEditor.Tables.OpenTableFromStringFaster(inputFeaturePath);

            //Cast to dataset in order to retrieve workspace
            IDataset inputDataset = inputFeatureTable as IDataset;
            IWorkspace inputWorkspace = inputDataset.Workspace;

            //Access the table behind the dataset
            IFeatureClass inputFeatureClass = GSC_ProjectEditor.FeatureClass.OpenFeatureClassFromWorkspace(inputWorkspace, inputDataset.Name);

            GSC_ProjectEditor.ObjectManagement.ReleaseObject(inputFeatureTable);

            return inputFeatureClass;
        }

        /// <summary>
        /// Will return a feature class object from a in_memory reference path
        /// </summary>
        /// <param name="inputFeature"> Feature class full reference path</param>
        /// <returns></returns>
        public static IFeatureClass OpenFeatureClassFromInMemory(string inMemoryFeaturePath)
        {
            IGPUtilities util = new GPUtilities();
            IFeatureClass getInFeature = util.OpenFeatureClassFromString(inMemoryFeaturePath);

            return getInFeature;
        }

        /// <summary>
        /// Will return a feature class object, to be used with project database
        /// </summary>
        /// <param name="inputFeature"></param>
        /// <returns></returns>
        public static IFeatureClass OpenFeatureClass(string inputFeature)
        {
            //Get project database path
            string getDBPath = Workspace.GetDBPath();

            //Get workspace 
            IWorkspace getWork = Workspace.AccessWorkspace(getDBPath);

            //Call other method to get the feature class object
            IFeatureClass getFC = OpenFeatureClassFromWorkspace(getWork, inputFeature);

            return getFC;
        }

        #endregion

        #region GET METHODS

        /// <summary>
        /// Will return a cursor object to iterate through a feature class
        /// </summary>
        /// <param name="cursorType">Cursor type: Update, Insert or Search</param>
        /// <param name="query">A query to filter search or update cursors</param>
        /// <param name="featureName">the desire feature class name to get the cursor from</param>
        /// <returns></returns>
        public static IFeatureCursor GetFeatureCursor(string cursorType, string query, string featureName)
        {
            //Get workspace from project
            string workPath = Workspace.GetDBPath();
            IWorkspace getWork = Workspace.AccessWorkspace(workPath) as IWorkspace;

            return GetFeatureCursorFromWorkspace(getWork, cursorType, query, featureName);
        }

        /// <summary>
        /// Will return a cursor object to iterate through a feature class, from a given workspace
        /// </summary>
        /// <param name="cursorType">Cursor type: Update, Insert or Search</param>
        /// <param name="query">A query to filter search or update cursors</param>
        /// <param name="featureName">the desire feature class name to get the cursor from</param>
        /// <returns></returns>
        public static IFeatureCursor GetFeatureCursorFromWorkspace(IWorkspace inWorkspace, string cursorType, string query, string featureName)
        {
            //Main variable
            IFeatureCursor featCursor = null;

            try
            {
                //Get required feature class object
                IFeatureClass feat = FeatureClass.OpenFeatureClassFromWorkspace(inWorkspace, featureName);

                //Build a query filter for update cursor
                IQueryFilter queryFilter = new QueryFilterClass();

                //Update filter with where clause
                if (query != null)
                {
                    queryFilter.WhereClause = query;
                }
                else
                {
                    queryFilter = null;
                }

                //Access inside feature class with a cursor (an update one in case there is a need to recalculate values)
                if (cursorType == "Update")
                {
                    featCursor = feat.Update(queryFilter, true);
                }

                else if (cursorType == "Search")
                {
                    featCursor = feat.Search(queryFilter, true);
                }

                else if (cursorType == "Insert")
                {
                    featCursor = feat.Insert(true);
                }
                else
                {
                    featCursor = feat.Search(queryFilter, true);
                }
            }
            catch (Exception getFeatureCursorExcept)
            {
                int lineNumber = GSC_ProjectEditor.Exceptions.LineNumber(getFeatureCursorExcept);
                MessageBox.Show("getFeatureCursorExcept (" + lineNumber.ToString() + "): " + getFeatureCursorExcept.StackTrace);
            }

            return featCursor;

        }

        /// <summary>
        /// Will return a string list containing all the fields from given table
        /// </summary>
        /// <param name="inputFC">The feature that contains the table</param>
        /// <param name="alias">Speficify true if field name has to be alias instead of real name</param>
        /// <returns></returns>
        public static List<string> GetFieldList(IFeatureClass inputFC, bool alias)
        {
            //Variables
            List<string> fieldList = new List<string>();
            int fieldCount = 0;

            //Get the fields object
            IFields inFields = inputFC.Fields;

            //Iterate through collection
            while (fieldCount < inFields.FieldCount)
            {
                if (alias)
                {
                    fieldList.Add(inFields.Field[fieldCount].AliasName);
                }
                else
                {
                    fieldList.Add(inFields.Field[fieldCount].Name);
                }

                fieldCount++;
            }

            return fieldList;
        }

        /// <summary>
        /// Will return a feature class name
        /// </summary>
        /// <param name="inputFC">The feature class to retrieve name from</param>
        /// <returns></returns>
        public static string GetFeatureClassName(IFeatureClass inputFC)
        {
            IFeatureDataset inputFD = inputFC.FeatureDataset;

            return inputFD.Name;
        }

        /// <summary>
        /// Will return a list of all feature classes inside a given workspace
        /// </summary>
        /// <param name="inputWorkspace">The input workspace to retrieve FC list from</param>
        /// <param name="inputFeatureDataset">Input feature dataset, if needed, to refine search, or else rooted fc will be returned.</param>
        public static List<string> GetFeatureClassList(IWorkspace inputWorkspace, IFeatureDataset inputFeatureDataset)
        {
            //Variables
            List<string> featureList = new List<string>();
            IEnumDataset fcEnum;

            if (inputFeatureDataset != null)
            {
                //Get a list of all feature classes from workspace
                fcEnum = inputFeatureDataset.Subsets;

            }
            else
            {
                //Get a list of all feature classes from workspace
                fcEnum = inputWorkspace.get_Datasets(esriDatasetType.esriDTFeatureClass);
            }

            //Convert enum to list
            IDataset currentDS = fcEnum.Next();
            while (currentDS != null)
            {
                if (currentDS.Type == esriDatasetType.esriDTFeatureClass)
                {
                    featureList.Add(currentDS.Name);  
                }
               
                currentDS = fcEnum.Next();
            }

            return featureList;
        }

        /// <summary>
        /// Will return a list of all feature classes inside a given workspace
        /// </summary>
        /// <param name="inputWorkspace">The input workspace to retrieve FC list from</param>
        /// <param name="inputFeatureDataset">Input feature dataset, if needed, to refine search, or else rooted fc will be returned.</param>
        public static List<IFeatureClass> GetFeatureClassListAsFeatureClass(IWorkspace inputWorkspace, IFeatureDataset inputFeatureDataset)
        {
            //Variables
            List<IFeatureClass> featureList = new List<IFeatureClass>();
            IEnumDataset fcEnum;

            if (inputFeatureDataset != null)
            {
                //Get a list of all feature classes from workspace
                fcEnum = inputFeatureDataset.Subsets;

            }
            else
            {
                //Get a list of all feature classes from workspace
                fcEnum = inputWorkspace.get_Datasets(esriDatasetType.esriDTFeatureClass);
            }

            //Convert enum to list
            IDataset currentDS = fcEnum.Next();
            while (currentDS != null)
            {
                if (currentDS.Type == esriDatasetType.esriDTFeatureClass)
                {
                    featureList.Add(currentDS as IFeatureClass);
                }

                currentDS = fcEnum.Next();
            }

            return featureList;
        }

        /// <summary>
        /// Will return a list of geometries from a feature.
        /// </summary>
        /// <param name="fromFeatureName">The feature to get geometries from</param>
        /// <param name="query">An optional query to filter geometries</param>
        /// <returns></returns>
        public static List<IGeometry> GetGeometryFrom(string fromFeatureName, bool fromOutside, string query)
        {
            //Variables
            List<IGeometry> geometryList = new List<IGeometry>();
            IFeatureClass getInFeature;
            IWorkspace getInWorkspace;

            try
            {
                if (fromOutside)
                {
                    //Get a feature class object from input study area
                    getInFeature = GSC_ProjectEditor.FeatureClass.OpenFeatureClassFromString(fromFeatureName);
                }
                else
                {
                    string getDbPath = Workspace.GetDBPath();
                    getInWorkspace = GSC_ProjectEditor.Workspace.AccessWorkspace(getDbPath) as IWorkspace;
                    getInFeature = OpenFeatureClassFromWorkspace(getInWorkspace, fromFeatureName);
                }

                //Build a query filter for update cursor
                IQueryFilter queryFilter = new QueryFilterClass();

                //Update filter with where clause
                if (query != null)
                {
                    queryFilter.WhereClause = query;
                }
                else
                {
                    queryFilter = null;
                }

                //Get a search cursor
                IFeatureCursor geomCursor = getInFeature.Search(queryFilter, true);
                IFeature geomRow = geomCursor.NextFeature();

                while (geomRow != null)
                {
                    geometryList.Add(geomRow.ShapeCopy);
                    geomRow = geomCursor.NextFeature();
                }

                //Release coms
                System.Runtime.InteropServices.Marshal.ReleaseComObject(geomCursor);
            }
            catch (Exception GetGeometryFromExcept)
            {
                int lineNumber = GSC_ProjectEditor.Exceptions.LineNumber(GetGeometryFromExcept);
                MessageBox.Show("GetGeometryFromExcept (" + lineNumber.ToString() + "): " + GetGeometryFromExcept.Message);
            }

            return geometryList;

        }

        /// <summary>
        /// Will return a dictionary of geometries from a feature. Key will be id and values the geometry
        /// </summary>
        /// <param name="fromFeatureName">The feature to get geometries from</param>
        /// <param name="query">An optional query to filter geometries</param>
        /// <param name="fromOutside">Indicates if data is outside current project geodatbase</param>
        /// <param name="idField">Enter an id field that will serve as key for output dictionary</param>
        /// <returns></returns>
        public static Dictionary<string, IGeometry> GetGeometryDicoFrom(string fromFeatureName, bool fromOutside, string query, string idField)
        {
            //Variables
            Dictionary<string, IGeometry> geometryList = new Dictionary<string, IGeometry>();
            IFeatureClass getInFeature;
            IWorkspace getInWorkspace;

            if (fromOutside)
            {
                //Get a feature class object from input study area
                getInFeature = GSC_ProjectEditor.FeatureClass.OpenFeatureClassFromStringFaster(fromFeatureName);
            }
            else
            {
                string getDbPath = Workspace.GetDBPath();
                getInWorkspace = GSC_ProjectEditor.Workspace.AccessWorkspace(getDbPath) as IWorkspace;
                getInFeature = OpenFeatureClassFromWorkspace(getInWorkspace, fromFeatureName);
            }

            //Build a query filter for update cursor
            IQueryFilter queryFilter = new QueryFilterClass();

            //Update filter with where clause
            if (query != null)
            {
                queryFilter.WhereClause = query;
            }
            else
            {
                queryFilter = null;
            }

            IFeatureCursor geomCursor = getInFeature.Search(queryFilter, true);
            IFeature geomRow = geomCursor.NextFeature();
            int idFieldIndex = geomCursor.FindField(idField);

            while (geomRow != null)
            {
                geometryList[geomRow.get_Value(idFieldIndex).ToString()] = geomRow.ShapeCopy;
                geomRow = geomCursor.NextFeature();
            }

            //Release coms
            System.Runtime.InteropServices.Marshal.ReleaseComObject(geomCursor);


            return geometryList;

        }

        /// <summary>
        /// Will return a dictionary of geometries from a feature. Key will be id and values the geometry
        /// </summary>
        /// <param name="inFC">The feature to get geometries from</param>
        /// <param name="query">An optional query to filter geometries</param>
        /// <param name="fromOutside">Indicates if data is outside current project geodatbase</param>
        /// <param name="idField">Enter an id field that will serve as key for output dictionary</param>
        /// <returns></returns>
        public static Dictionary<string, IGeometry> GetGeometryDicoFromFC(IFeatureClass inFC, string query, string idField)
        {
            //Variables
            Dictionary<string, IGeometry> geometryList = new Dictionary<string, IGeometry>();

            //Build a query filter for update cursor
            IQueryFilter queryFilter = new QueryFilterClass();

            //Update filter with where clause
            if (query != null)
            {
                queryFilter.WhereClause = query;
            }
            else
            {
                queryFilter = null;
            }

            IFeatureCursor geomCursor = inFC.Search(queryFilter, true);
            IFeature geomRow = geomCursor.NextFeature();
            int idFieldIndex = geomCursor.FindField(idField);

            while (geomRow != null)
            {
                geometryList[geomRow.get_Value(idFieldIndex).ToString()] = geomRow.ShapeCopy;
                geomRow = geomCursor.NextFeature();
            }

            //Release coms
            System.Runtime.InteropServices.Marshal.ReleaseComObject(geomCursor);

            return geometryList;
        }


        /// <summary>
        /// Will return a given feature class extent or "enveloppe"
        /// </summary>
        public static IEnvelope GetFeatureClassEnvelope(IFeatureClass inFC)
        {
            //Cast feature class as geodataset
            IGeoDataset inGeoDS = inFC as IGeoDataset;

            //Cast geodataset extent to IEnvelope
            IEnvelope inEnvelope = inGeoDS.Extent as IEnvelope;

            return inEnvelope;
        }

        #endregion

        #region MODIFY METHODS (DEL, APPEND, ADD, CONVERT)

        /// <summary>
        /// Will add a new polygon to an existing polygon feature class from a given set of coordinates, for project database only
        /// </summary>
        /// <param name="featureName">The feature to add the new polygon to</param>
        /// <param name="coordinateList">A list of vertices pairs as double numbers[x,y] for the coordinates [West, North, East, South]</param>
        /// <param name="inFieldAndValues">A dictionnary to contain field names and field values to add while adding a new polygon</param>
        public static void AddOnePolygonFromCoord(string featureName, List<List<double>> coordinateList, Dictionary<string, object> inFieldAndValues)
        {
            //Access workspace
            string getDBPath = Workspace.GetDBPath();
            IWorkspace getWorkspace = Workspace.AccessWorkspace(getDBPath);

            //Create a feature buffer
            IFeatureClass getFeatClass = OpenFeatureClassFromWorkspace(getWorkspace, featureName);

            AddOnePolygonInFCFromCoord(getFeatClass, coordinateList, inFieldAndValues);
        }
         
        /// <summary>
        /// Will add a new polygon to an existing polygon feature class from a given set of coordinates
        /// </summary>
        /// <param name="featureName">The feature to add the new polygon to</param>
        /// <param name="coordinateList">A list of vertices pairs as double numbers[x,y] for the coordinates [West, North, East, South]</param>
        /// <param name="inFieldAndValues">A dictionnary to contain field names and field values to add while adding a new polygon</param>
        public static void AddOnePolygonInFCFromCoord(IFeatureClass inFC, List<List<double>> coordinateList, Dictionary<string, object> inFieldAndValues)
        {
            //Create a polygon object
            IPolygon newPoly = new PolygonClass();

            //Create an array of points
            IPointCollection getPntArray = Geometry.BuildPointArray(coordinateList);

            //Cast array to polygon
            newPoly = getPntArray as IPolygon;

            //Make Z Aware, or else throws an error, because feature is 3D
            Geometry.MakeZAware(newPoly as IGeometry);

            //Close new poly
            newPoly.Close();

            //Create a feature buffer
            IFeatureBuffer getFeatBuffer = inFC.CreateFeatureBuffer();

            //Create the insert cursor
            IFeatureCursor insertCursor = inFC.Insert(true);

            //Add all field values to new feature
            foreach (KeyValuePair<string, object> fieldValues in inFieldAndValues)
            {
                //Get field index
                int fieldIndex = getFeatBuffer.Fields.FindField(fieldValues.Key);

                //Set field value
                getFeatBuffer.set_Value(fieldIndex, fieldValues.Value);
            }

            //Set the new feature shape
            getFeatBuffer.Shape = newPoly;

            //Insert
            insertCursor.InsertFeature(getFeatBuffer);

            //Flush
            insertCursor.Flush();

            //Release coms
            System.Runtime.InteropServices.Marshal.ReleaseComObject(insertCursor);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(getFeatBuffer);

        }

        /// <summary>
        /// Will copy a set of feature geometries of fromFeatureName and copy it into toFeatureName
        /// </summary>
        /// <param name="fromFeatureName">The feature to get the geometry from, might be outside project database, so full path is required</param>
        /// <param name="fromOutside"> A bool value indicating if input from feature is from outside project database</param>
        /// <param name="toFeatureName">The feature to paste the geometry to</param>
        /// <param name="inFieldAnValues">A list of dictionnaries of field and their values (one list item for each new polygons)</param>
        /// <param name="query">Enter a query to filter geometries</param>
        public static bool AppendPolygon(string fromFeatureName, bool fromOutside, string toFeatureName, List<Dictionary<string, object>> inFieldAnValues, string query)
        {
            //Variable
            bool appended = false;

            try
            {
                //Access workspace for feature to append to
                string getDBPath = Workspace.GetDBPath();
                IWorkspace getWorkspace = Workspace.AccessWorkspace(getDBPath);

                //Get a list of geometries
                List<IGeometry> getGeom = GetGeometryFrom(fromFeatureName, fromOutside, query);

                //Create a feature buffer to paste new geom
                IFeatureClass getFeatClass = OpenFeatureClassFromWorkspace(getWorkspace, toFeatureName);
                IFeatureBuffer getFeatBuffer = getFeatClass.CreateFeatureBuffer();

                //Get spatial references from features
                IFeatureClass fromFeature = OpenFeatureClassFromString(fromFeatureName);
                ISpatialReference getRefFromFeature = SpatialReferences.GetSpatialRef(fromFeature);
                ISpatialReference getRefToFeature = SpatialReferences.GetSpatialRef(getFeatClass);

                //Validate match between spatial ref
                if (getRefFromFeature.Name == getRefToFeature.Name)
                {
                    //Create the insert cursor
                    IFeatureCursor insertCursor = getFeatClass.Insert(true);

                    //Iterate through list and append them into toFeatureName
                    foreach (IGeometry geomItem in getGeom)
                    {

                        //Make Z Aware, or else throws an error, because feature is 3D
                        Geometry.MakeZAware(geomItem);

                        //Apply a constant Z
                        Geometry.ApplyConstantZ(geomItem, 0.0);

                        //Get proper list of field values 
                        int geomIndex = getGeom.IndexOf(geomItem);

                        //TRAP ANY POLYGONS NUMBER OVER 1 WITH THIS TRY CATCH
                        try
                        {
                            Dictionary<string, object> getFieldValues = inFieldAnValues[geomIndex];

                            //Add all field values to new feature
                            foreach (KeyValuePair<string, object> fieldValues in getFieldValues)
                            {
                                //Get field index
                                int fieldIndex = getFeatBuffer.Fields.FindField(fieldValues.Key);

                                //Set field value
                                getFeatBuffer.set_Value(fieldIndex, fieldValues.Value);
                            }

                            //Set the new feature shape
                            getFeatBuffer.Shape = geomItem;

                            //Insert
                            insertCursor.InsertFeature(getFeatBuffer);
                        }
                        catch (Exception)
                        {
                            
                        }


                    }

                    //Flush
                    insertCursor.Flush();

                    //Release coms
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(insertCursor);
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(getFeatBuffer);

                    //Feature were appended, no error detected
                    appended = true;
                }
                else
                {
                    MessageBox.Show(Properties.Resources.Error_SpatialRefMismatch);
                }

            }
            catch (Exception AppendPolygonExcept)
            {
                int lineNumber = GSC_ProjectEditor.Exceptions.LineNumber(AppendPolygonExcept);
                MessageBox.Show("AppendPolygonExcept (" + lineNumber.ToString() + "):" + AppendPolygonExcept.Message);
            }

            return appended;

        }

        /// <summary>
        /// Will delete any feature class as input
        /// </summary>
        public static void DeleteFeatureClass(IFeatureClass inputFCToDelete)
        {
            //Cast the input as a dataset
            IDataset inDataset = inputFCToDelete as IDataset;
            inDataset.Delete();
        }

        /// <summary>
        /// From a given linework and some labels, will create polygons.
        /// </summary>
        /// <param name="outputPolygon">The polygon feature to add polygons to.</param>
        /// <param name="inputLabels">The label feature to get a polygon type from</param>
        /// <param name="inputLines">The linework feature to create the polygons from</param>
        public static void ConvertLineToPolygon(IFeatureClass outputPolygon, IFeatureClass inputLabels, IFeatureCursor inputLines)
        {
            try
            {
                //Get default XY tolerance for given feature class
                ISpatialReference lineSR = GSC_ProjectEditor.SpatialReferences.GetSpatialRef(outputPolygon);
                double currenTolerance = GSC_ProjectEditor.SpatialReferences.GetXYTolerance(lineSR);

                //Call a special feature construction object to create the polygons
                IFeatureConstruction buildGeopoly = new FeatureConstructionClass();
                if (inputLabels != null)
                {
                    buildGeopoly.ConstructPolygonsFromFeaturesFromCursor(null, outputPolygon, null, false, false, inputLines, null, currenTolerance, inputLabels);
                }
                else
                {
                    buildGeopoly.ConstructPolygonsFromFeaturesFromCursor(null, outputPolygon, null, false, false, inputLines, null, currenTolerance, null);
                }
            }
            catch (Exception ConvertLineToPolygonException)
            {
                MessageBox.Show(ConvertLineToPolygonException.StackTrace);
            }


            

        }

        /// <summary>
        /// From a given feature class, will delete the features from it. A query can be passed.
        /// </summary>
        /// <param name="inputFeature">The feature class to delete features from</param>
        /// <param name="query">A query if needed, else null</param>
        /// <param name="outSideEditSession">Will this be runned outside an edit session? If Yes the method will execute but at a lower speed.</param>
        public static void DeleteFeatures(IFeatureClass inputFeature, string query, bool outSideEditSession)
        {
            //Manage query if needed
            IQueryFilter queryFilter = new QueryFilter();
            if (query != null)
            {
                queryFilter.WhereClause = query;
            }
            else
            {
                queryFilter = null;
            }

            #region This method is really slow, but can be launch outside editor
            if (outSideEditSession)
            {
                //Get a cursor for given feature class
                IFeatureCursor searchCursor = inputFeature.Update(queryFilter, false);

                IFeature inFeature = null;
                while ((inFeature = searchCursor.NextFeature()) != null)
                {
                    inFeature.Delete();
                }

                //Release cursor
                System.Runtime.InteropServices.Marshal.ReleaseComObject(searchCursor);
            }

            #endregion

            #region Faster method, but needs to be inside editor
            if (!outSideEditSession)
            {
                ITable inTable = inputFeature as ITable;
                inTable.DeleteSearchedRows(queryFilter);
            }


            #endregion


        }

        /// <summary>
        /// Will return true if given feature class name was found inside a given workspace
        /// </summary>
        /// <returns></returns>
        public static bool FindIfFeatureClassExistsInWorkspace(IWorkspace inWork, string featureClassName)
        {
            //Get a lits of all feature class inside workspace
            List<string> featureClassNames = GetFeatureClassList(inWork, null);
            
            return featureClassNames.Contains(featureClassName);
            
        }

        /// <summary>
        /// Will rename a given feature class
        /// </summary>
        /// <param name="inFCToRename">The feature class to rename</param>
        /// <param name="newName">The new name for the feature class</param>
        /// <returns></returns>
        public static bool RenameFeatureClass(IFeatureClass inFCToRename, string newName)
        {
            //Variables
            bool haveBeenRenamed = false;
            
            //cast to dataset
            IDataset inDataset = inFCToRename as IDataset;

            //Validate if rename is available
            if (inDataset.CanRename())
            {
                //Rename
                inDataset.Rename(newName);
            }

            return haveBeenRenamed;

        }

        /// <summary>
        /// Will update a feature class spatial extent to be on part with internal
        /// geometries
        /// </summary>
        /// <param name="inFC">The feature class to update spatial extent from</param>
        public static void UpdateFeatureClassExtent(IFeatureClass inFC)
        {
            //Cast
            IFeatureClassManage fcManager = inFC as IFeatureClassManage;
            fcManager.UpdateExtent();
        }

        #endregion

        #region CREATE METHODS

        /// <summary>
        /// Will create a new feature class, inside a given workspace.
        /// </summary>
        /// <param name="inWork">The workspace to add the new feature class to</param>
        /// <param name="name">The name of the new feature class</param>
        /// <param name="inFields">A field object containing all the required field to add to the new feature lass</param>
        /// <param name="inType">The type of feature class to create</param>
        /// <returns></returns>
        public static IFeatureClass CreateFeatureClass(IWorkspace inWork, string name, IFields inFields, esriFeatureType inType, string shapeFieldName)
        {
            //Cast the workspace to a feature workspace
            IFeatureWorkspace featWorkspace = inWork as IFeatureWorkspace;

            //Create some UIDs
            UID classID = new UIDClass();
            classID.Value = "esriGeoDatabase.Feature";
            
            //Create
            IFeatureClass outFC = featWorkspace.CreateFeatureClass(name, inFields, null, null, inType, shapeFieldName, null);
            
            return outFC;
        }

        /// <summary>
        /// Will create a new feature class, inside a given workspace, with basic default fields
        /// </summary>
        /// <param name="inWork">The workspace to add the new featrure class inside</param>
        /// <param name="name">The name of the new feature class</param>
        /// <param name="inType">The type of feature class to create, usually it'll be simple</param>
        /// <returns></returns>
        public static IFeatureClass CreateFeatureClassWithDefaultFields(IWorkspace inWork, string name, esriFeatureType inType, esriGeometryType inGeometryType, ISpatialReference inSpatialReference)
        {
            //Variable
            string shapeFieldName = "Shape";
            bool shapeFieldExist = false;
            int shapeFieldIndex = 0;
            IFeatureClass outputFC;

            #region Create the field object with default
            //Create a new object class description definition
            IObjectClassDescription objClassDesc = new FeatureClassDescriptionClass();

            //Create field collection with all required ones
            IFields outFields = objClassDesc.RequiredFields;

            //Init the fields object with the default required field for a feature class
            IFieldsEdit outFieldsEdit = outFields as IFieldsEdit;

            //Locate the shape field in order to set up the geometry to point
            for (int i = 0; i < outFields.FieldCount; i++)
            {
                esriFieldType currentFieldType = outFields.get_Field(i).Type;

                if (outFields.get_Field(i).Type == esriFieldType.esriFieldTypeGeometry)
                {
                    //Get name for when creating the feature class
                    shapeFieldName = outFields.get_Field(i).Name;
                    shapeFieldExist = true;
                    shapeFieldIndex = i;
                }

            }

            //Get current field
            IField shapeField = outFields.get_Field(shapeFieldIndex);
            IFieldEdit shapeFieldEdit = shapeField as IFieldEdit;

            if (!shapeFieldExist)
            {
                //Create  a new field definition for the shape field
                shapeField = new FieldClass();
                shapeFieldEdit = shapeField as IFieldEdit;
                shapeFieldEdit.Name_2 = "Shape";
                shapeFieldEdit.Type_2 = esriFieldType.esriFieldTypeGeometry;
            }

            #endregion
            
            #region Add the geometry type inside the fields
            //Create a new geometry definitin and edit it
            IGeometryDef newGeometryDefinition = new GeometryDefClass();
            IGeometryDefEdit newGeomDefEdit = newGeometryDefinition as IGeometryDefEdit;
            newGeomDefEdit.GeometryType_2 = inGeometryType;
            newGeomDefEdit.HasZ_2 = true;
            newGeomDefEdit.SpatialReference_2 = inSpatialReference;//Get default of table of content, since the user doesn't care about it.

            //Edit the shape field to add the new geometry object
            shapeFieldEdit.GeometryDef_2 = newGeomDefEdit;

            //Add the new field to the collection
            if (shapeFieldExist)
            {
                outFieldsEdit.set_Field(shapeFieldIndex, shapeField);
            }
            else
            {
                outFieldsEdit.set_Field(outFields.FieldCount, shapeField);
            }

            #endregion

            #region Create the new feature class

            outputFC = CreateFeatureClass(inWork, name, outFields, inType, shapeFieldName);

            #endregion

            return outputFC;
        }

        #endregion
    }
}
