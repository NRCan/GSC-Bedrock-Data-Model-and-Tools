using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.DataSourcesFile;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.GeoDatabaseDistributed;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Catalog;
using ESRI.ArcGIS.DataSourcesOleDB;

namespace GSC_ProjectEditor
{
    public class Workspace
    {
        #region GET METHODS

        /// <summary>
        /// Get project database path 
        /// ERROR PREVENT --> Do not use outside functions, crashes arc map at init.
        /// </summary>
        /// <returns></returns>
        public static string GetDBPath()
        {
            //Variables
            string DBPath = null;


            try
            {

                //Build path to database
                DBPath = GSC_ProjectEditor.Properties.Settings.Default.PROJECT_DATABASE_PATH;//getDBPath + "\\" + getDBName + ".gdb";

            }
            catch (Exception getDBPath)
            {
                MessageBox.Show("GetDBPath error: " + getDBPath.Message);
            }

            return DBPath;

        }

        /// <summary>
        /// Will return a workspace from a given feature layer name. Will iterate through arc map table of content to find correct layer.
        /// </summary>
        /// <param name="inFeatureLayerName">The layer name to find</param>
        /// <returns></returns>
        public static IWorkspace GetFeatureLayerWorkspace(string inFeatureLayerName, IMap getCurrentMap, string layerType)
        {
            //New feature layer to return
            IWorkspace wantedWorkspace = null;

            try
            {
                //Get feature layer object GUID
                UID flUID = new UIDClass();
                flUID.Value = GSC_ProjectEditor.Constants.GUIDs.UIDLayer;

                //Iterate trhough TOC and find wanted layer
                IEnumLayer TOCLayers = getCurrentMap.get_Layers(flUID, true);
                ILayer currentFeatureLayer = TOCLayers.Next();


                //Find wanted layer
                while (currentFeatureLayer != null)
                {
                    //Parse features
                    if (layerType == "Table")
                    {

                        ITable flTable = currentFeatureLayer as ITable;
                        Table tableFromFL = flTable as Table;
                        IDataset tableDataset = tableFromFL as IDataset;
                        if (tableDataset != null)
                        {
                            if (tableDataset.Name == inFeatureLayerName)
                            {
                                wantedWorkspace = tableDataset.Workspace;
                                break;
                            }

                        }

                    }
                    else if (layerType == "Feature")
                    {
                        IFeatureDataset inFeatDataset = ((IFeatureLayer)currentFeatureLayer).FeatureClass.FeatureDataset;
                        IFeatureLayer inFL = currentFeatureLayer as IFeatureLayer;
                        if (inFeatDataset != null)
                        {
                            if (inFL.Name == inFeatureLayerName)
                            {
                                wantedWorkspace = inFeatDataset.Workspace;
                                break;
                            }

                        }

                    }

                    //Next
                    currentFeatureLayer = TOCLayers.Next();
                }

            }
            catch (Exception GetFeatureLayerWorkspaceExcept)
            {
                int lineNumber = Exceptions.LineNumber(GetFeatureLayerWorkspaceExcept);
                MessageBox.Show("GetFeatureLayerWorkspaceExcept (" + lineNumber.ToString() + "):" + GetFeatureLayerWorkspaceExcept.Message);
            }

            return wantedWorkspace;

        }

        /// <summary>
        /// Will return a list of dataset that resides inside a workspace, like a file geodatabase
        /// </summary>
        /// <param name="inputWorkspace">The workspace to get a list of dataset from</param>
        /// <returns></returns>
        public static List<IDataset> GetDatasetListFromWorkspace(IWorkspace inputWorkspace)
        {
            //Variables
            List<IDataset> dataList = new List<IDataset>();

            //Get collection of datasets
            IEnumDataset enumData = inputWorkspace.get_Datasets(esriDatasetType.esriDTAny);

            //Fill the list
            IDataset currentData = enumData.Next();
            while (currentData != null)
            {
                dataList.Add(currentData);


                //Check for different inside feature dataset
                if (currentData.Category.Contains("Dataset"))
                {
                    //Cast as feature dataset
                    IFeatureDataset featDataset = currentData as IFeatureDataset;
                    IEnumDataset enumFeatDataset = featDataset.Subsets;

                    IDataset currentFData = enumFeatDataset.Next();
                    while (currentFData != null)
                    {
                        dataList.Add(currentFData);
                        currentFData = enumFeatDataset.Next();
                    }

                }

                currentData = enumData.Next();
            }

            return dataList;
        }

        /// <summary>
        /// Will return a list of dataset browse name that resides inside a workspace, like a file geodatabase
        /// </summary>
        /// <param name="inputWorkspace">The workspace to get a list of dataset from</param>
        /// <returns></returns>
        public static List<string> GetDatasetNameListFromWorkspace(IWorkspace inputWorkspace)
        {
            //Variables
            List<string> dataList = new List<string>();

            //Get collection of datasets
            IEnumDataset enumData = inputWorkspace.get_Datasets(esriDatasetType.esriDTAny);

            //Fill the list
            IDataset currentData = enumData.Next();
            while (currentData != null)
            {
                dataList.Add(currentData.BrowseName);

                //Check for different inside feature dataset
                if (currentData.Category.Contains("Dataset"))
                {
                    //Cast as feature dataset
                    IFeatureDataset featDataset = currentData as IFeatureDataset;

                    if (featDataset != null)
                    {
                        IEnumDataset enumFeatDataset = featDataset.Subsets;

                        IDataset currentFData = enumFeatDataset.Next();
                        while (currentFData != null)
                        {
                            dataList.Add(currentFData.BrowseName);
                            currentFData = enumFeatDataset.Next();
                        }
                    }


                }


                currentData = enumData.Next();
            }

            return dataList;
        }

        /// <summary>
        /// Will return true if a given data type name already exist in project database
        /// </summary>
        /// <param name="dataType"></param>
        /// <param name="inName"></param>
        /// <returns></returns>
        public static bool GetNameExists(esriDatasetType dataType, string inName)
        {
            //Access workspace
            string projectDBPath = GetDBPath();
            IWorkspace projectWork = AccessWorkspace(projectDBPath);

            return GetNameExistsFromWorkspace(projectWork, dataType, inName);

        }

        /// <summary>
        /// Will return true if a given data type name already exist in project database
        /// </summary>
        /// <param name="dataType"></param>
        /// <param name="inName"></param>
        /// <returns></returns>
        public static bool GetNameExistsFromWorkspace(IWorkspace inputWorkspace, esriDatasetType dataType, string inName)
        {
            //Access workspace
            IWorkspace2 work2 = inputWorkspace as IWorkspace2;

            return work2.get_NameExists(dataType, inName);
        }

        /// <summary>
        /// Will validate a dataset name from a given workspace and remove any problemactic characters or keyword.
        /// </summary>
        /// <param name="inputWorkspace">The workspace in which the new dataset will be added.</param>
        /// <param name="inputName">The wanted name for the output dataset name</param>
        /// <returns></returns>
        public static string GetValidDatasetName(IWorkspace inputWorkspace, string inputName)
        {
            //Variable
            string validName = inputName;

            //Validate
            IFieldChecker validChecker = new FieldCheckerClass();
            validChecker.InputWorkspace = inputWorkspace;
            validChecker.ValidateTableName(inputName, out validName);

            return validName;
        }

        #endregion

        #region ACCESS METHODS

        /// <summary>
        /// Create a workspace factory to access a file database
        /// </summary>
        /// <param name="inputWorkspacePath">Reference full path to wanted workspace, usually project main geodatabase</param>
        /// <returns></returns>
        public static dynamic AccessWorkspace_Depreciated(string inputWorkspacePath)
        {
            //Make sure nothing exists
            IWorkspaceFactory iWF = null;
            IWorkspace iW = null;

            //Create workspace factory for file geodatabase
            iWF = new FileGDBWorkspaceFactory();

            try
            {
                if (iWF.IsWorkspace(inputWorkspacePath) == true)
                {
                    return iW = iWF.OpenFromFile(inputWorkspacePath, 0);
                }
                else
                {
                    string error = Properties.Resources.Error_AccessDB;
                    throw new Exception(error);

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return ex;
            }


        }

        /// <summary>
        /// Create a workspace factory to access a file database, personal database or shapefile
        /// </summary>
        /// <param name="inputFeaturePath">Reference full path to wanted workspace, usually project main geodatabase</param>
        /// <returns></returns>
        public static IWorkspace AccessWorkspace(string inputPath)
        {
            //Variables
            IWorkspaceFactory workFactory = null;
            IWorkspace workspc = null;

            try
            {
                if (inputPath.Substring(inputPath.Length - 4, 4).ToLower() == ".gdb")
                {
                    //Activate the singleton, or else System.__ComObject errors could pop.
                    Type t = Type.GetTypeFromProgID("esriDataSourcesGDB.FileGDBWorkspaceFactory");
                    System.Object obj = Activator.CreateInstance(t);
                    workFactory = obj as FileGDBWorkspaceFactory;

                }
                else if (inputPath.Substring(inputPath.Length - 4, 4).ToLower() == ".mdb")
                {
                    //Activate the singleton, or else System.__ComObject errors could pop.
                    Type t = Type.GetTypeFromProgID("esriDataSourcesGDB.AccessWorkspaceFactory");
                    System.Object obj = Activator.CreateInstance(t);
                    workFactory = obj as AccessWorkspaceFactory;
                }
                else if (inputPath.Substring(inputPath.Length - 4, 4).ToLower() == ".shp" || inputPath.Substring(inputPath.Length - 4, 4).ToLower() == ".dbf")
                {
                    //Get workspace path and file name from input
                    inputPath = System.IO.Path.GetDirectoryName(inputPath); //Rest path to directory, for shapefiles only.
                    workFactory = new ShapefileWorkspaceFactory();

                }
                else if (inputPath.Substring(inputPath.Length - 7, 7).ToLower() == ".sqlite")
                {
                    //Activate the singleton, or else System.__ComObject errors could pop.

                    Type t = Type.GetTypeFromProgID("esriDataSourcesGDB.SqlWorkspaceFactory");
                    System.Object obj = Activator.CreateInstance(t);
                    workFactory = obj as SqlWorkspaceFactory;
                }
                workspc = workFactory.OpenFromFile(inputPath, 0);

                //Release workspace factory
                System.Runtime.InteropServices.Marshal.ReleaseComObject(workFactory);

            }
            catch (Exception accessWorkspaceExcept)
            {
                int lineNumber = Exceptions.LineNumber(accessWorkspaceExcept);
                MessageBox.Show("accessWorkspaceExcept (" + lineNumber.ToString() + "):" + accessWorkspaceExcept.Message);
            }

            return workspc;
        }

        /// <summary>
        /// Will return a workspace factory for excel given an input path without the file sheet name 
        /// </summary>
        /// <param name="inputFeaturePath">Reference full path excel sheet</param>
        /// <returns></returns>
        public static IWorkspace AccessExcelWorkspace(string inputPath)
        {
            //Variables
            IWorkspaceFactory workFactory = null;
            IWorkspace workspace = null;

            try
            {
                if (inputPath.Contains(".xls") || inputPath.Contains(".xlsx"))
                {
                    //Activate the singleton, or else System.__ComObject errors could pop.
                    Type t = Type.GetTypeFromProgID("esriDataSourcesOleDB.ExcelWorkspaceFactory");
                    System.Object obj = Activator.CreateInstance(t);
                    workFactory = obj as ExcelWorkspaceFactory;

                }
                workspace = workFactory.OpenFromFile(inputPath, 0);

                //Release workspace factory
                System.Runtime.InteropServices.Marshal.ReleaseComObject(workFactory);

            }
            catch (Exception accessWorkspaceExcept)
            {
                int lineNumber = Exceptions.LineNumber(accessWorkspaceExcept);
                MessageBox.Show("AccessExcelWorkspace (" + lineNumber.ToString() + "):" + accessWorkspaceExcept.Message);
            }

            return workspace;
        }

        /// <summary>
        /// Will return a workspace factory for a given textfile path. 
        /// </summary>
        /// <param name="inputFeaturePath">Reference full path to wanted text file</param>
        /// <returns></returns>
        public static IWorkspace AccessTextfileWorkspace(string inputPath)
        {
            //Variables
            IWorkspaceFactory workFactory = null;
            IWorkspace workspace = null;

            //Get parent folder
            string inputPathParentFolder = System.IO.Path.GetDirectoryName(inputPath);

            try
            {
                if (inputPath.Contains(".txt") || inputPath.Contains(".csv"))
                {
                    //Activate the singleton, or else System.__ComObject errors could pop.
                    Type t = Type.GetTypeFromProgID("esriDataSourcesFile.TextFileWorkspaceFactory");
                    workFactory = (IWorkspaceFactory)Activator.CreateInstance(t);

                }
                workspace = workFactory.OpenFromFile(inputPathParentFolder, 0);

                //Release workspace factory
                System.Runtime.InteropServices.Marshal.ReleaseComObject(workFactory);

            }
            catch (Exception accessWorkspaceExcept)
            {
                int lineNumber = Exceptions.LineNumber(accessWorkspaceExcept);
                MessageBox.Show("AccessTextfileWorkspace (" + lineNumber.ToString() + "):" + accessWorkspaceExcept.Message);
            }

            return workspace;
        }

        /// <summary>
        /// Create a workspace factory to access a file database, personal database or shapefile, used mainly to find if a given feature exists inside database.
        /// </summary>
        /// <param name="inputFeaturePath">Reference full path to wanted workspace, usually project main geodatabase</param>
        /// <returns></returns>
        public static IWorkspace2 AccessWorkspace2(string inputPath)
        {
            //Variables
            IWorkspaceFactory workFactory = null;
            IWorkspace2 workspc = null;

            try
            {
                if (inputPath.Substring(inputPath.Length - 4, 4).ToLower() == ".gdb")
                {
                    //Activate the singleton, or else System.__ComObject errors could pop.
                    Type t = Type.GetTypeFromProgID("esriDataSourcesGDB.FileGDBWorkspaceFactory");
                    System.Object obj = Activator.CreateInstance(t);
                    workFactory = obj as FileGDBWorkspaceFactory;

                }
                else if (inputPath.Substring(inputPath.Length - 4, 4).ToLower() == ".mdb")
                {
                    //Activate the singleton, or else System.__ComObject errors could pop.
                    Type t = Type.GetTypeFromProgID("esriDataSourcesGDB.AccessWorkspaceFactory");
                    System.Object obj = Activator.CreateInstance(t);
                    workFactory = obj as AccessWorkspaceFactory;
                }
                else if (inputPath.Substring(inputPath.Length - 4, 4).ToLower() == ".shp" || inputPath.Substring(inputPath.Length - 4, 4).ToLower() == ".dbf")
                {
                    //Get workspace path and file name from input
                    inputPath = System.IO.Path.GetDirectoryName(inputPath); //Rest path to directory, for shapefiles only.
                    workFactory = new ShapefileWorkspaceFactory();

                }

                workspc = workFactory.OpenFromFile(inputPath, 0) as IWorkspace2;

                //Release workspace factory
                System.Runtime.InteropServices.Marshal.ReleaseComObject(workFactory);

            }
            catch (Exception accessWorkspaceExcept)
            {
                int lineNumber = Exceptions.LineNumber(accessWorkspaceExcept);
                MessageBox.Show("accessWorkspaceExcept (" + lineNumber.ToString() + "):" + accessWorkspaceExcept.Message);
            }

            return workspc;
        }

        /// <summary>
        /// Not Working... was meant to give access to a in_memory workspace.Instead use
        /// </summary>
        /// <param name="getWorkspaceFactory"></param>
        /// <returns></returns>
        public static IWorkspace AccessInMemoryWorkspace()
        {

            //Create an in-memory workspace factory
            IWorkspaceFactory workFact = new InMemoryWorkspaceFactory();

            //Create a new workspace name to init a new workspace
            IWorkspaceName workName = workFact.Create(null, "inMemory", null, 0);
            IName wName = (IName)workName;

            //Open
            IWorkspace workspace = (IWorkspace)wName.Open();

            return workspace;
        }

        #endregion

        #region CREATE METHODS

        /// <summary>
        /// Will create a file geodatabase and return a workspace object.
        /// </summary>
        /// <param name="newPath"> Input new database full path (C:\Folder\Folder\...)</param>
        /// <param name="newName"> Input new database name</param>
        /// <returns></returns>
        public static IWorkspace CreateWorkspace(string parentDirectory, string newName)
        {
            IWorkspace getWorkspace = null;

            //Create a factor
            Type factoryType = Type.GetTypeFromProgID("esriDataSourcesGDB.FileGDBWorkspaceFactory");
            IWorkspaceFactory workFactory = Activator.CreateInstance(factoryType) as IWorkspaceFactory;

            IWorkspaceName workName = workFactory.Create(parentDirectory, newName + ".gdb", null, 0);

            //Cast and return
            IName name = workName as IName;
            getWorkspace = name.Open() as IWorkspace;

            //Release workspace factory
            System.Runtime.InteropServices.Marshal.ReleaseComObject(workFactory);

            return getWorkspace;
        }

        /// <summary>
        /// Will create and return an in_memory workspace to be used in scratchWorkspaces
        /// </summary>
        /// <returns></returns>
        public static IWorkspace CreateInMemoryWorkspace()
        {
            //Create a factory 
            IWorkspaceFactory imWork = new InMemoryWorkspaceFactory();

            //Create a work name
            IWorkspaceName workName = imWork.Create(null, "IMeMineWorkspace", null, 0);
            IName name = workName as IName;

            //Build the workspace
            IWorkspace inMemoryWorkspace = name.Open() as IWorkspace;

            //Release workspace factory
            System.Runtime.InteropServices.Marshal.ReleaseComObject(imWork);

            return inMemoryWorkspace;
        }

        /// <summary>
        /// Will create a new workspace from a given XML resource in current GSC_ProjectEditor project.
        /// </summary>
        /// <param name="inResource">The resource XML file passed as a string</param>
        /// <param name="outFullPath">The output folder path to the new file geodatabase</param>
        /// <param name="outName">The output name of the new geodatabase name</param>
        /// <returns></returns>
        public static IWorkspace CreateWorkspaceFromResource(string resourceXML, string outRootPath, string outName, string extension)
        {

            //Write the XML Workspace document to a local file, or else it can't be used to import into a database
            string tempFilePath = System.IO.Path.GetTempFileName();
            tempFilePath += ".xml";
            using (StreamWriter outfile = new StreamWriter(tempFilePath)) { outfile.Write(resourceXML); }

            //Get new workspace
            IWorkspace newWorkspace = CreateWorkspaceFromXML(tempFilePath, outRootPath, outName, extension);

            return newWorkspace;


        }

        /// <summary>
        /// Will create a new workspace from a given XML workspace file.
        /// </summary>
        /// <param name="XMLFullPath">The XML workspace file path, that will be imported inside new workspace.</param>
        /// <param name="outName">The output name of the new workspace to create.</param>
        /// <param name="outRootPath">The output full path of the new workspace to create.</param>
        /// <returns></returns>
        public static IWorkspace CreateWorkspaceFromXML(string XMLFullPath, string outRootPath, string outName, string extension)
        {
            string newOutputPath = System.IO.Path.Combine(outRootPath, outName + extension);
            bool dbExists = System.IO.Directory.Exists(newOutputPath);

            if (!dbExists)
            {
                //Create new environment database
                IWorkspace newEnvDB = Workspace.CreateWorkspace(outRootPath, outName);

                //Import XML workspace schema
                Workspace.ImportXMLWorkspace(newEnvDB, XMLFullPath);

                return newEnvDB;
            }
            else
            {
                return null;
            }

        }

        /// <summary>
        /// Will create a new workspace for shapefiles, based on the parent folder name.
        /// </summary>
        /// <param name="folderForShapefile">The parent folder path. New shapefiles can be created inside this folder</param>
        /// <returns></returns>
        public static IWorkspace CreateWorkspaceForShapefiles(string folderForShapefile)
        {
            //string environmentDBPath = Properties.Settings.Default.Tools4ProjectFolder + Constants.Environment.envRelPath;
            bool dbExists = System.IO.Directory.Exists(folderForShapefile);

            if (!dbExists)
            {
                //Create the folder
                Directory.CreateDirectory(folderForShapefile);
            }

            //Create a factory 
            IWorkspaceFactory shapeWorkspaceFactory = new ShapefileWorkspaceFactory();

            //Build the workspace
            IWorkspace shapeWorkspace = shapeWorkspaceFactory.OpenFromFile(folderForShapefile, 0);

            //Release workspace factory
            System.Runtime.InteropServices.Marshal.ReleaseComObject(shapeWorkspaceFactory);

            return shapeWorkspace;

        }

        #endregion

        #region IMPORT METHODS

        /// <summary>
        /// Will import any xml workspace into an existing file geodatabase
        /// </summary>
        /// <param name="inputWorkspace">The input database object</param>
        /// <param name="importPath">The input path to XML</param>
        public static IWorkspace ImportXMLWorkspace(IWorkspace inputWorkspace, string importPath)
        {

            //Build proper objects to import xml workspace
            IGdbXmlImport gdbImporter = new GdbImporter();
            IEnumNameMapping enumMapping = null; //Will be used to validate entries
            Boolean conflicts = gdbImporter.GenerateNameMapping(importPath, inputWorkspace, out enumMapping);
            IDataset inputDataset = inputWorkspace as IDataset;
            IName inputName = inputDataset.FullName;

            #region Parse conflicts
            if (conflicts)
            {
                // Iterate through each name mapping.
                INameMapping nameMapping = null;
                enumMapping.Reset();
                while ((nameMapping = enumMapping.Next()) != null)
                {
                    // Resolve the mapping's conflict (if there is one).
                    if (nameMapping.NameConflicts)
                    {
                        nameMapping.TargetName = nameMapping.GetSuggestedName(inputName);
                    }

                    // See if the mapping's children have conflicts.
                    IEnumNameMapping childEnumNameMapping = nameMapping.Children;
                    if (childEnumNameMapping != null)
                    {
                        childEnumNameMapping.Reset();

                        // Iterate through each child mapping.
                        INameMapping childNameMapping = null;
                        while ((childNameMapping = childEnumNameMapping.Next()) != null)
                        {
                            if (childNameMapping.NameConflicts)
                            {
                                childNameMapping.TargetName =
                                    childNameMapping.GetSuggestedName(inputName);
                            }
                        }
                    }
                }
            }
            #endregion

            //Import
            gdbImporter.ImportWorkspace(importPath, enumMapping, inputWorkspace, false);

            return inputWorkspace;

        }

        #endregion

        #region DO METHODS

        /// <summary>
        /// Will compact a given workspace.
        /// </summary>
        /// <param name="inWork">The workspace to compact</param>
        public static void DoCompact(IWorkspace inWork)
        {
            //Cast
            IDatabaseCompact dbCompact = inWork as IDatabaseCompact;

            //Validate if possibe
            if (dbCompact.CanCompact())
            {
                dbCompact.Compact();
            }

        }

        #endregion

        #region DELETE METHODS

        public static void DeleteWorkspace(IWorkspace inWorkToDelete)
        {
            //Cast as dataset
            IDataset datasetToDelete = inWorkToDelete as IDataset;
            
            //Validate if it's possible to delete before doing it 
            if (datasetToDelete.CanDelete())
            {
                datasetToDelete.Delete();  
            }
            else
            {
                MessageBox.Show(GSC_ProjectEditor.Properties.Resources.Error_CantDeleteWorkspace + ": " + inWorkToDelete.PathName);
            }
            
        }

        #endregion

        #region ENABLES/DISABLE

        /// <summary>
        /// Will enable the editor tracking on a given workspace (needs to be file geodatabase)
        /// </summary>
        /// <param name="inWorkspace">The workspace to enable the tracking from</param>
        /// <param name="featureList">A list containing names of all the feature to enable the editor</param>
        /// <param name="createField">The creator field name</param>
        /// <param name="createDateField">The creator init. date field name</param>
        /// <param name="editorField">The editor field name</param>
        /// <param name="editorDateField">The editor editing date field name</param>
        public static void EnabledEditorTrackingFromWorkspace(IWorkspace inWorkspace, List<string> featureList, string createField, string createDateField, string editorField, string editorDateField)
        {
            ////Get a list of all features inside the workspace
            //IEnumDataset enumDatasets = inWorkspace.get_Datasets(esriDatasetType.esriDTFeatureClass);
            //IDataset currentData = enumDatasets.Next();

            ////Iterate through all the features names
            //while (currentData !=null)
            //{

            IFeatureWorkspace featWorkspace = (IFeatureWorkspace)inWorkspace;

            //Get current name if it's in list
            foreach (string features in featureList)
            {
                try
                {
                    //Cast to feature class
                    IFeatureClass currentFC = featWorkspace.OpenFeatureClass(features);

                    //Cast to dataset
                    IDataset currentData = (IDataset)currentFC;

                    //Cast to an object class
                    IObjectClass currentObjectClass = (IObjectClass)currentData;

                    //Cast to schema editor 4
                    IClassSchemaEdit4 schemaEditor4 = (IClassSchemaEdit4)currentObjectClass;

                    //Set the time
                    schemaEditor4.IsTimeInUTC = false;

                    //Access and set the fields
                    if (createField != null)
                    {
                        //Get the index
                        int createFieldIndex = currentFC.FindField(createField);

                        //Get the field object
                        IField createFieldObject = currentFC.Fields.get_Field(createFieldIndex);

                        //Set
                        schemaEditor4.CreatorFieldName = createFieldObject.Name;
                    }

                    if (createDateField != null)
                    {
                        //Get the index
                        int createFieldDateIndex = currentFC.FindField(createDateField);

                        //Get the field object
                        IField createFieldDateObject = currentFC.Fields.get_Field(createFieldDateIndex);

                        //Set
                        schemaEditor4.CreatedAtFieldName = createFieldDateObject.Name;
                    }

                    if (editorField != null)
                    {
                        //Get the index
                        int editorFieldIndex = currentFC.FindField(editorField);

                        //Get the field object
                        IField editorFieldObject = currentFC.Fields.get_Field(editorFieldIndex);

                        //Set
                        schemaEditor4.EditorFieldName = editorFieldObject.Name;
                    }

                    if (editorDateField != null)
                    {
                        //Get the index
                        int editorDateFieldIndex = currentFC.FindField(editorDateField);

                        //Get the field object
                        IField editorDateFieldObject = currentFC.Fields.get_Field(editorDateFieldIndex);

                        //Set
                        schemaEditor4.EditedAtFieldName = editorDateFieldObject.Name;
                    }

                    
                }
                catch (Exception)
                {

                }
            }
        }

        /// <summary>
        /// Will disable editor tracking for a given workspace and list of feature classes
        /// </summary>
        /// <param name="inWorkspace"></param>
        /// <param name="featureList"></param>
        public static void DisableEditorTrackingFromWorkspace(IWorkspace inWorkspace, List<string> featureList)
        {
            //https://desktop.arcgis.com/en/arcobjects/latest/net/webframe.htm#IClassSchemaEdit4_EditorFieldName.htm

            //Access feature workspace
            IFeatureWorkspace featWorkspace = (IFeatureWorkspace)inWorkspace;

            //Iterate through fcs
            foreach (string features in featureList)
            {
                try
                {
                    //Cast to feature class
                    IFeatureClass currentFC = featWorkspace.OpenFeatureClass(features);

                    //Cast to dataset
                    IDataset currentData = (IDataset)currentFC;

                    //Cast to an object class
                    IObjectClass currentObjectClass = (IObjectClass)currentData;

                    //Cast to schema editor 4
                    IClassSchemaEdit4 schemaEditor4 = (IClassSchemaEdit4)currentObjectClass;

                    //Set the time
                    schemaEditor4.IsTimeInUTC = false;

                    //Unset
                    schemaEditor4.CreatorFieldName = string.Empty;
                    schemaEditor4.CreatedAtFieldName = string.Empty;
                    schemaEditor4.EditorFieldName = string.Empty;
                    schemaEditor4.EditedAtFieldName = string.Empty;

                }
                catch (Exception)
                {

                }
            }
        }

        #endregion

        #region EXPORT METHODS

        /// <summary>
        /// Will export a file geodatabase or a given workspace to an XML workspace in a given file path.
        /// </summary>
        /// <param name="outputWorkspace">The workspace to export</param>
        /// <param name="outputXMLPath">The output file path, containing the name and extension.</param>
        public static void ExportXMLWorkspace(IWorkspace outputWorkspace, string outputXMLPath)
        {
            //Build proper objects to export xml workspace
            IGdbXmlExport gdbExporter = new GdbExporter();

            gdbExporter.ExportWorkspace(outputWorkspace, outputXMLPath, true, false, true);

        }

        #endregion

        #region SCHEMA LOCKS

        public static void ListSchemaLocksForObjectClass(IObjectClass objectClass)
        {
            //Get an exclusive schema lock on the dataset.
            ISchemaLock schemaLock = (ISchemaLock)objectClass;

            // Get an enumerator over the current schema locks.
            IEnumSchemaLockInfo enumSchemaLockInfo = null;
            schemaLock.GetCurrentSchemaLocks(out enumSchemaLockInfo);

            // Iterate through the locks.
            ISchemaLockInfo schemaLockInfo = null;
            while ((schemaLockInfo = enumSchemaLockInfo.Next()) != null)
            {
                MessageBox.Show(schemaLockInfo.TableName + "; " + schemaLockInfo.UserName + "; " + schemaLockInfo.SchemaLockType);

            }
        }
        #endregion

        #region VALIDATE

        /// <summary>
        /// From a given workspace, will show an error message if one or many datasets aren't find inside it.
        /// </summary>
        /// <returns></returns>
        public static bool isWorkspaceAProperProjectDatabase(IWorkspace inWorkspace, List<string> datasetToValidate, bool withErrorMessage = true, bool validateWithLastWorkspace = true)
        {
            //Variables
            bool isValid = false;
            List<string> missingDatasets = new List<string>();
            System.Collections.Specialized.StringCollection settingDatasetCollection = new System.Collections.Specialized.StringCollection();

            //Validate if list of names exist in internal setting
            if (!validateWithLastWorkspace)
            {
                settingDatasetCollection.AddRange(GetDatasetNameListFromWorkspace(inWorkspace).ToArray());
                Properties.Settings.Default.DatasetNameList = settingDatasetCollection;
                Properties.Settings.Default.Save();
            }
            else
            {
                if (Properties.Settings.Default.DatasetNameList == null || Properties.Settings.Default.DatasetNameList.Count == 0)
                {
                    settingDatasetCollection.AddRange(GetDatasetNameListFromWorkspace(inWorkspace).ToArray());
                    Properties.Settings.Default.DatasetNameList = settingDatasetCollection;
                    Properties.Settings.Default.Save();
                }
                else
                {
                    settingDatasetCollection = Properties.Settings.Default.DatasetNameList;
                }
            }

            //Validate if user names to validate exists in workspace
            if (datasetToValidate != null || datasetToValidate.Count > 0)
            {
                foreach (string datasets in datasetToValidate)
                {
                    if (!settingDatasetCollection.Contains(datasets))
                    {
                        missingDatasets.Add(datasets);
                    }
                }
            }

            //Manage error messsage
            if (missingDatasets.Count > 0)
            {

                string completeListString = string.Empty;
                foreach (string missingItems in missingDatasets)
                {
                    if (completeListString == string.Empty)
                    {
                        completeListString = missingItems;
                    }
                    else
                    {
                        completeListString = completeListString + "; " + missingItems;
                    }
                    
                }
                if (withErrorMessage)
                {
                    Messages.ShowGenericErrorMessage("Following of mandatary datasets weren't found in current workspace: " + completeListString);
                }
                
            }
            else
            {
                isValid = true;
            }

            return isValid;
        }

        #endregion

    }
}
