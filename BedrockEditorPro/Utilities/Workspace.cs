using ArcGIS.Core.Data;
using ArcGIS.Core.Data.DDL;
using ArcGIS.Core.Data.Topology;
using ArcGIS.Desktop.Core.Geoprocessing;
using ArcGIS.Desktop.Framework.Threading.Tasks;
using ArcGIS.Desktop.Mapping;
using BedrockEditorPro.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Printing;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace BedrockEditorPro.Utilities
{
    public class Workspace
    {
        #region GET METHODS

        /// <summary>
        /// Will return the original database path of a feature layer
        /// </summary>
        /// <param name="inFL"></param>
        /// <returns></returns>
        public static Uri GetWorkspacePathFromFeatureLayer(FeatureLayer inFL)
        {
            Uri outputWorkspaceUri = null;
            FeatureClass fc = inFL.GetFeatureClass();

            if (fc != null)
            {
                Uri fcURI = fc.GetPath();

                if (fcURI != null)
                {
                    string fcPath = fcURI.OriginalString;
                    string outputWorkspacePath = Directory.GetParent(fcPath).FullName;
                    if (outputWorkspacePath != null && outputWorkspacePath != string.Empty)
                    {
                        //Remove any feature dataset name at the end of the workspace path
                        if (outputWorkspacePath.Contains(".gdb") && (outputWorkspacePath.Contains(".gdb\\") || outputWorkspacePath.Contains(".gdb/")))
                        {
                            outputWorkspacePath = outputWorkspacePath.Substring(0, outputWorkspacePath.IndexOf(".gdb") + 4);
                        }

                        outputWorkspaceUri = new Uri(outputWorkspacePath);
                    }
                    
                }
            }
            

            return outputWorkspaceUri;
        }

        #endregion

        #region CREATE METHODS

        /// <summary>
        /// Will create a file geodatabase and return a workspace object.
        /// </summary>
        /// <param name="outputGDBPathName"> Input new database full path (C:\Folder\...\x.gdb)</param>
        /// <returns></returns>
        public static Geodatabase CreateWorkspace(string outputGDBPathName)
        {
            Geodatabase getWorkspace = null;

            try
            {
                // Create a FileGeodatabaseConnectionPath with the name of the file geodatabase you wish to create
                FileGeodatabaseConnectionPath fileGeodatabaseConnectionPath =
                  new FileGeodatabaseConnectionPath(new Uri(outputGDBPathName));

                // Create and use the file geodatabase
                getWorkspace = SchemaBuilder.CreateGeodatabase(fileGeodatabaseConnectionPath);
            }
            catch (Exception e)
            {
                new ErrorService(e).WriteToFile();
            }


            return getWorkspace;
        }

        /// <summary>
        /// Will create and return an in_memory workspace to be used in scratchWorkspaces
        /// </summary>
        /// <returns></returns>
        public static MemoryConnectionProperties CreateInMemoryWorkspace()
        {

            //Create a work name
            MemoryConnectionProperties memoryConnectionProperties = new MemoryConnectionProperties("IMeMineWorkspace");


            return memoryConnectionProperties;
        }

        #endregion

        #region IMPORT METHODS

        /// <summary>
        /// Will import any xml workspace into an existing file geodatabase
        /// </summary>
        /// <param name="inputWorkspace">The input database object</param>
        /// <param name="importPath">The input path to XML</param>
        public static async Task<IGPResult> ImportXMLWorkspace(string inputWorkspacePath, string importPath)
        {

            IEnumerable<string> valueArray = await QueuedTask.Run<IReadOnlyList<string>>(() =>
            {
                
                var valueArray = Geoprocessing.MakeValueArray(inputWorkspacePath, importPath, "DATA");
                return valueArray;
            });

            IGPResult gpResult = await Geoprocessing.ExecuteToolAsync("management.ImportXMLWorkspaceDocument", valueArray, null, CancelableProgressor.None, GPExecuteToolFlags.Default);
            if (gpResult.IsFailed)
            {
                // display error messages if the tool fails, otherwise shows the default messages
                new ErrorService(gpResult).WriteToFile();
            }

            return gpResult;

        }

        #endregion

        #region DO METHODS

        ///// <summary>
        ///// Will compact a given workspace.
        ///// </summary>
        ///// <param name="inWork">The workspace to compact</param>
        //public static void DoCompact(IWorkspace inWork)
        //{
        //    //Cast
        //    IDatabaseCompact dbCompact = inWork as IDatabaseCompact;

        //    //Validate if possibe
        //    if (dbCompact.CanCompact())
        //    {
        //        dbCompact.Compact();
        //    }

        //}

        #endregion

        #region DELETE METHODS

        //public static void DeleteWorkspace(IWorkspace inWorkToDelete)
        //{
        //    //Cast as dataset
        //    IDataset datasetToDelete = inWorkToDelete as IDataset;

        //    //Validate if it's possible to delete before doing it 
        //    if (datasetToDelete.CanDelete())
        //    {
        //        datasetToDelete.Delete();  
        //    }
        //    else
        //    {
        //        MessageBox.Show(GSC_ProjectEditor.Properties.Resources.Error_CantDeleteWorkspace + ": " + inWorkToDelete.PathName);
        //    }

        //}

        #endregion

        #region ENABLES/DISABLE

        ///// <summary>
        ///// Will enable the editor tracking on a given workspace (needs to be file geodatabase)
        ///// </summary>
        ///// <param name="inWorkspace">The workspace to enable the tracking from</param>
        ///// <param name="featureList">A list containing names of all the feature to enable the editor</param>
        ///// <param name="createField">The creator field name</param>
        ///// <param name="createDateField">The creator init. date field name</param>
        ///// <param name="editorField">The editor field name</param>
        ///// <param name="editorDateField">The editor editing date field name</param>
        //public static void EnabledEditorTrackingFromWorkspace(IWorkspace inWorkspace, List<string> featureList, string createField, string createDateField, string editorField, string editorDateField)
        //{
        //    ////Get a list of all features inside the workspace
        //    //IEnumDataset enumDatasets = inWorkspace.get_Datasets(esriDatasetType.esriDTFeatureClass);
        //    //IDataset currentData = enumDatasets.Next();

        //    ////Iterate through all the features names
        //    //while (currentData !=null)
        //    //{

        //    IFeatureWorkspace featWorkspace = (IFeatureWorkspace)inWorkspace;

        //    //Get current name if it's in list
        //    foreach (string features in featureList)
        //    {
        //        try
        //        {
        //            //Cast to feature class
        //            IFeatureClass currentFC = featWorkspace.OpenFeatureClass(features);

        //            //Cast to dataset
        //            IDataset currentData = (IDataset)currentFC;

        //            //Cast to an object class
        //            IObjectClass currentObjectClass = (IObjectClass)currentData;

        //            //Cast to schema editor 4
        //            IClassSchemaEdit4 schemaEditor4 = (IClassSchemaEdit4)currentObjectClass;

        //            //Set the time
        //            schemaEditor4.IsTimeInUTC = false;

        //            //Access and set the fields
        //            if (createField != null)
        //            {
        //                //Get the index
        //                int createFieldIndex = currentFC.FindField(createField);

        //                //Get the field object
        //                IField createFieldObject = currentFC.Fields.get_Field(createFieldIndex);

        //                //Set
        //                schemaEditor4.CreatorFieldName = createFieldObject.Name;
        //            }

        //            if (createDateField != null)
        //            {
        //                //Get the index
        //                int createFieldDateIndex = currentFC.FindField(createDateField);

        //                //Get the field object
        //                IField createFieldDateObject = currentFC.Fields.get_Field(createFieldDateIndex);

        //                //Set
        //                schemaEditor4.CreatedAtFieldName = createFieldDateObject.Name;
        //            }

        //            if (editorField != null)
        //            {
        //                //Get the index
        //                int editorFieldIndex = currentFC.FindField(editorField);

        //                //Get the field object
        //                IField editorFieldObject = currentFC.Fields.get_Field(editorFieldIndex);

        //                //Set
        //                schemaEditor4.EditorFieldName = editorFieldObject.Name;
        //            }

        //            if (editorDateField != null)
        //            {
        //                //Get the index
        //                int editorDateFieldIndex = currentFC.FindField(editorDateField);

        //                //Get the field object
        //                IField editorDateFieldObject = currentFC.Fields.get_Field(editorDateFieldIndex);

        //                //Set
        //                schemaEditor4.EditedAtFieldName = editorDateFieldObject.Name;
        //            }


        //        }
        //        catch (Exception)
        //        {

        //        }
        //    }
        //}

        ///// <summary>
        ///// Will disable editor tracking for a given workspace and list of feature classes
        ///// </summary>
        ///// <param name="inWorkspace"></param>
        ///// <param name="featureList"></param>
        //public static void DisableEditorTrackingFromWorkspace(IWorkspace inWorkspace, List<string> featureList)
        //{
        //    //https://desktop.arcgis.com/en/arcobjects/latest/net/webframe.htm#IClassSchemaEdit4_EditorFieldName.htm

        //    //Access feature workspace
        //    IFeatureWorkspace featWorkspace = (IFeatureWorkspace)inWorkspace;

        //    //Iterate through fcs
        //    foreach (string features in featureList)
        //    {
        //        try
        //        {
        //            //Cast to feature class
        //            IFeatureClass currentFC = featWorkspace.OpenFeatureClass(features);

        //            //Cast to dataset
        //            IDataset currentData = (IDataset)currentFC;

        //            //Cast to an object class
        //            IObjectClass currentObjectClass = (IObjectClass)currentData;

        //            //Cast to schema editor 4
        //            IClassSchemaEdit4 schemaEditor4 = (IClassSchemaEdit4)currentObjectClass;

        //            //Set the time
        //            schemaEditor4.IsTimeInUTC = false;

        //            //Unset
        //            schemaEditor4.CreatorFieldName = string.Empty;
        //            schemaEditor4.CreatedAtFieldName = string.Empty;
        //            schemaEditor4.EditorFieldName = string.Empty;
        //            schemaEditor4.EditedAtFieldName = string.Empty;

        //        }
        //        catch (Exception)
        //        {

        //        }
        //    }
        //}

        #endregion

        #region EXPORT METHODS

        ///// <summary>
        ///// Will export a file geodatabase or a given workspace to an XML workspace in a given file path.
        ///// </summary>
        ///// <param name="outputWorkspace">The workspace to export</param>
        ///// <param name="outputXMLPath">The output file path, containing the name and extension.</param>
        //public static void ExportXMLWorkspace(IWorkspace outputWorkspace, string outputXMLPath)
        //{
        //    //Build proper objects to export xml workspace
        //    IGdbXmlExport gdbExporter = new GdbExporter();

        //    gdbExporter.ExportWorkspace(outputWorkspace, outputXMLPath, true, false, true);

        //}

        #endregion

        #region SCHEMA LOCKS

        //public static void ListSchemaLocksForObjectClass(IObjectClass objectClass)
        //{
        //    //Get an exclusive schema lock on the dataset.
        //    ISchemaLock schemaLock = (ISchemaLock)objectClass;

        //    // Get an enumerator over the current schema locks.
        //    IEnumSchemaLockInfo enumSchemaLockInfo = null;
        //    schemaLock.GetCurrentSchemaLocks(out enumSchemaLockInfo);

        //    // Iterate through the locks.
        //    ISchemaLockInfo schemaLockInfo = null;
        //    while ((schemaLockInfo = enumSchemaLockInfo.Next()) != null)
        //    {
        //        MessageBox.Show(schemaLockInfo.TableName + "; " + schemaLockInfo.UserName + "; " + schemaLockInfo.SchemaLockType);

        //    }
        //}
        #endregion

        #region VALIDATE

        // Must be called within QueuedTask.Run9)
        public static bool TableExists(Geodatabase geodatabase, string tableName)
        {
            try
            {
                TableDefinition tableDefinition = geodatabase.GetDefinition<TableDefinition>(tableName);
                tableDefinition.Dispose();
                return true;
            }
            catch
            {
                // GetDefinition throws an exception if the definition doesn't exist
                return false;
            }
        }

        /// <summary>
        /// Will check for the existance of a feature class inside a geodatabase
        /// </summary>
        /// <param name="geodatabase"></param>
        /// <param name="featureClassName"></param>
        /// <returns></returns>
        public static bool FeatureClassExists(Geodatabase geodatabase, string featureClassName)
        {
            try
            {
                FeatureClassDefinition fcDefinition = geodatabase.GetDefinition<FeatureClassDefinition>(featureClassName);
                fcDefinition.Dispose();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Will check for the existance of a feature dataset inside a geodatabase
        /// </summary>
        /// <param name="geodatabase"></param>
        /// <param name="featureClassName"></param>
        /// <returns></returns>
        public static bool FeatureDatasetExists(Geodatabase geodatabase, string featureDatasetName)
        {
            try
            {
                FeatureDatasetDefinition fdDefinition = geodatabase.GetDefinition<FeatureDatasetDefinition>(featureDatasetName);
                fdDefinition.Dispose();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Will check for the existance of some topology inside a geodatabase
        /// </summary>
        /// <param name="geodatabase"></param>
        /// <param name="featureClassName"></param>
        /// <returns></returns>
        public static bool TopologyExists(Geodatabase geodatabase, string topologyName)
        {
            try
            {
                TopologyDefinition fdDefinition = geodatabase.GetDefinition<TopologyDefinition>(topologyName);
                fdDefinition.Dispose();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion

    }
}
