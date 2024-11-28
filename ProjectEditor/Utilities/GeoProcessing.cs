using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ESRI.ArcGIS.DataSourcesFile;
using ESRI.ArcGIS.Geoprocessing;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.DataManagementTools;
using ESRI.ArcGIS.Geoprocessor;
using ESRI.ArcGIS.AnalysisTools;
using ESRI.ArcGIS.ConversionTools;
using ESRI.ArcGIS.Carto;

namespace GSC_ProjectEditor
{
    public class GeoProcessing
    {

        #region COPY METHODS

        /// <summary>
        /// Will copy input features to an output one
        /// </summary>
        /// <param name="inFeature">The input feature layer or feature class object</param>
        /// <param name="outputFeatures">The output path for the new copy</param>
        /// <returns></returns>
        public static string CopyFeatures(object inFeature, string outputFeatures)
        {
            //Variables
            string outputPath = "";

            //Create a copy feature method
            ESRI.ArcGIS.DataManagementTools.CopyFeatures copyFeat = new ESRI.ArcGIS.DataManagementTools.CopyFeatures();
            copyFeat.in_features = inFeature;
            copyFeat.out_feature_class = outputFeatures;

            //Create the geoprocessing object
            Geoprocessor geop = new Geoprocessor();

            //Execute
            try
            {
                IGeoProcessorResult gpResult = geop.Execute(copyFeat as IGPProcess, null) as IGeoProcessorResult;
                IGPValue gpValue = gpResult.GetOutput(0);
                outputPath = gpValue.GetAsText();
            }
            catch (Exception copyFeaturesException)
            {

                MessageBox.Show(copyFeaturesException.StackTrace);
            }

            return outputPath;
        }

        /// <summary>
        /// Will make a copy of an input file geodatabase to an output path.
        /// </summary>
        public static string CopyFGDBOnly(IWorkspace inputFGDB, string outputPath)
        {
            //Variables
            string outputPathResult = "";

            //Create a copy method
            ESRI.ArcGIS.DataManagementTools.Copy copyObj = new ESRI.ArcGIS.DataManagementTools.Copy();
            copyObj.in_data = inputFGDB;
            copyObj.out_data = outputPath;

            //Create the geoprocessing object
            Geoprocessor geop = new Geoprocessor();

            //Execute
            try
            {
                //MessageBox.Show("here");
                IGeoProcessorResult gpResult = geop.Execute(copyObj as IGPProcess, null) as IGeoProcessorResult;
                IGPValue gpValue = gpResult.GetOutput(0);
                outputPathResult = gpValue.GetAsText();
            }
            catch (Exception CopyFGDBException)
            {
                MessageBox.Show(CopyFGDBException.StackTrace);
            }

            return outputPathResult;
        }

        /// <summary>
        /// Will copy an input dataset to an output workspace. Note: this method doesn't use GP processor in order to make the copy and validates any name conflicts between the input and output
        /// </summary>
        /// <param name="dataToCopy">The data to copy</param>
        /// <param name="outputPath">The output workspace</param>
        /// <returns></returns>
        public static void CopyDataset(IDataset dataToCopy, IWorkspace outputWorkspace)
        {

            try
            {
                //Create a name object for new target data
                IDataset targetWorkspaceName = outputWorkspace as IDataset;
                IName targetName = targetWorkspaceName.FullName as IName;

                //Create a name object for the source
                IName dataName = dataToCopy.FullName;

                //Create an enumarator for the source
                IEnumName sourceEnumName = new NamesEnumeratorClass();
                IEnumNameEdit sourceEnumNameEdit = sourceEnumName as IEnumNameEdit;

                //Add the source dataset to the enum class
                sourceEnumNameEdit.Add(dataName);

                //Create a GDBTransfert object
                IGeoDBDataTransfer geoTransfer = new GeoDBDataTransferClass();
                IEnumNameMapping enumMapping = null;

                //Use the data transfer object to create a name mapping enumrator
                bool conflictsFound = geoTransfer.GenerateNameMapping(sourceEnumName, targetName, out enumMapping);
                enumMapping.Reset();

                //Check for possible conflicts
                if (conflictsFound)
                {
                    //Iterate through name mapping
                    INameMapping nameMapping = null;
                    while ((nameMapping = enumMapping.Next()) != null)
                    {
                        //Resolve the mapping's conflict (if any)
                        if (nameMapping.NameConflicts)
                        {
                            nameMapping.TargetName = nameMapping.GetSuggestedName(targetName);
                        }

                        //Check for children conflicts (those teenagers years, oh boy)
                        IEnumNameMapping childEnumMapping = nameMapping.Children;
                        if (childEnumMapping != null)
                        {
                            childEnumMapping.Reset();
                            
                            //Iterate through childs
                            INameMapping childNameMapping = null;

                            while ((childNameMapping = childEnumMapping.Next()) != null)
                            {
                                if (childNameMapping.NameConflicts)
                                {
                                    childNameMapping.GetSuggestedName(targetName);
                                }
                            }
                        }
                    }
                }

                //Find dataset has parents
                if (dataToCopy.Type == esriDatasetType.esriDTFeatureClass)
                {
                    //Retrieve feature dataset and redo target
                    IFeatureClass inFC = dataToCopy as IFeatureClass;
                    if (inFC.FeatureDataset!= null)
                    {
                        IFeatureDataset inFD = inFC.FeatureDataset;
                        IDataset inD = inFD as IDataset;
                        targetName = inD.FullName; 
                    }

                }

                //Start the transfert
                geoTransfer.Transfer(enumMapping, targetName);
            }
            catch (Exception CopyFGDBException)
            {
                //MessageBox.Show(CopyFGDBException.StackTrace + "; " + CopyFGDBException.Message + "; " );
            }

        }

        /// <summary>
        /// Will copy an input dataset to an output workspace path. Note: this method use GP processor Copy method in order to make the copy, and doesn't validate the names for input and outputs
        /// </summary>
        /// <param name="dataToCopy">The dataset to copy</param>
        /// <param name="outputPath">The output path string for the new data (including it's name)</param>
        /// <returns></returns>
        public static string CopyDataSetGP(IDataset dataToCopy, string outputPath)
        {
            //Variables
            string outputPathResult = "";

            //Get the workspace from current dataset (to be used to create an IDataElement)
            IWorkspace inDataWorkspace = dataToCopy.Workspace;

            //Build some option to convert the dataset
            IDEBrowseOptions deBrowseOptions = new DEBrowseOptionsClass
            {
                RetrieveFullProperties = false,
                ExpandType = esriDEExpandType.esriDEExpandChildren,
                RetrieveMetadata = true

            };

            //Create the new object
            IWorkspaceDataElements workElements = inDataWorkspace as IWorkspaceDataElements;
            IDataElement inDataElement = workElements.GetDatasetDataElement(dataToCopy, deBrowseOptions);

            //Rebuild path to the element itself
            string newPath = inDataWorkspace.PathName + "\\" + dataToCopy.BrowseName;
            inDataElement.CatalogPath = newPath;

            //Validate if copied data doesn't already exists
            INameMapping nameMapping = dataToCopy.FullName as INameMapping;

            //Create a copy method
            ESRI.ArcGIS.DataManagementTools.Copy copyObj = new ESRI.ArcGIS.DataManagementTools.Copy();
            copyObj.in_data = inDataElement;
            copyObj.out_data = outputPath;

            //Create the geoprocessing object
            Geoprocessor geop = new Geoprocessor();

            //Execute
            try
            {
                //MessageBox.Show("here");
                IGeoProcessorResult gpResult = geop.Execute(copyObj as IGPProcess, null) as IGeoProcessorResult;
                IGPValue gpValue = gpResult.GetOutput(0);
                outputPathResult = gpValue.GetAsText();
            }
            catch (Exception CopyFGDBException)
            {
                MessageBox.Show(CopyFGDBException.StackTrace + "; " + CopyFGDBException.Message);
            }

            return outputPathResult;

        }

        /// <summary>
        /// Will copy any dataset to an output path
        /// </summary>
        /// <param name="dataToCopy">The dataset to copy</param>
        /// <param name="outputPath">The output path string for the new data (including it's name)</param>
        /// <returns></returns>
        public static string CopyAnyDataset(object inData, string outputPath)
        {
            //Variables
            string outputPathResult = "";

            //Create a copy method
            ESRI.ArcGIS.DataManagementTools.Copy copyObj = new ESRI.ArcGIS.DataManagementTools.Copy();
            copyObj.in_data = inData;
            copyObj.out_data = outputPath;

            //Create the geoprocessing object
            Geoprocessor geop = new Geoprocessor();

            //Execute
            try
            {
                //MessageBox.Show("here");
                IGeoProcessorResult gpResult = geop.Execute(copyObj as IGPProcess, null) as IGeoProcessorResult;
                IGPValue gpValue = gpResult.GetOutput(0);
                outputPathResult = gpValue.GetAsText();
            }
            catch (Exception CopyAnyDataset)
            {
                MessageBox.Show(CopyAnyDataset.StackTrace + "; " + CopyAnyDataset.Message);
            }

            return outputPathResult;

        }

        #endregion

        #region APPEND METHODS

        /// <summary>
        /// Will start an append operation from a manage assembly append method.
        /// </summary>
        /// <param name="inputFC">The input feature class.</param>
        /// <param name="outputFC">The target feature class.</param>
        public static void AppendData(object input, object output)
        {
            AppendDataWithFieldMap(input, output, null);
        }

        /// <summary>
        /// Will start an append operation from a manage assembly append method. Target FC to append is the output
        /// </summary>
        /// <param name="inputFC">The input feature class.</param>
        /// <param name="outputFC">The target feature class.</param>
        /// <param name="InOutFieldNames"> A dictionary of string that contains input field names as key and output field names as value, wanted to be mapped into the appended output table</param>
        public static void AppendDataWithFieldMap(object input, object output, Dictionary<string, List<string>> InOutFieldNames)
        {
            try
            {
                //Get full path of output data
                Table outputTable = output as Table;
                IDataset outDS = outputTable as IDataset;
                string fullPath = outDS.Workspace.PathName + "\\" + outDS.BrowseName;

                //Get full path of input data
                Table inputTable = input as Table;
                //IDataset inDS = inputTable as IDataset;
                //string inFullPath = inDS.Workspace.PathName + "\\" + inDS.BrowseName + inDS.Name;

                //Create the append method
                Append doAppend = new Append();
                doAppend.inputs = input;
                doAppend.target = output;
                doAppend.schema_type = "NO_TEST";

                //Build output array for fields
                IGPUtilities gpUtil = new GPUtilitiesClass();
                IDETable outputDET = gpUtil.MakeDataElement(fullPath, null, null) as IDETable;
                IArray outputArray = new ArrayClass();
                outputArray.Add(outputDET);

                //Build a field mapping object for output table (will map all fields from table)
                IGPFieldMapping getFieldMapping = new GPFieldMappingClass();
                IDataset outputDS = output as IDataset;
                getFieldMapping.Initialize(outputArray, null);

                //Iterate through dictionary, build and manage field maps
                if (InOutFieldNames != null)
                {
                    foreach (KeyValuePair<string, List<string>> kv in InOutFieldNames)
                    {

                        //Find a field map
                        int fieldMapIndex = getFieldMapping.FindFieldMap(kv.Key);
                        IGPFieldMap currentFM = getFieldMapping.GetFieldMap(fieldMapIndex);

                        //Get Field object from input
                        int inFieldIndex = inputTable.Fields.FindField(kv.Value[0]);
                        IField getInField = inputTable.Fields.get_Field(inFieldIndex);

                        //Add selected in field to out field map, with start and stop position
                        int startPosition = 0;
                        int stopPosition = getInField.Length;
                        if (kv.Value.Count==3)
                        {
                            startPosition = Convert.ToInt16(kv.Value[1]);
                            stopPosition = Convert.ToInt16(kv.Value[2]);
                            currentFM.AddInputField(outputDET, getInField, startPosition, stopPosition);
                        }
                        else
                        {
                            currentFM.AddInputField(outputDET, getInField, startPosition, stopPosition);
                        }
                        

                        currentFM.MergeRule = esriGPFieldMapMergeRule.esriGPFieldMapMergeRuleFirst;
                        //Add modified field map to field mapping object
                        getFieldMapping.AddFieldMap(currentFM);

                    }
                }

                doAppend.field_mapping = getFieldMapping;

                //Create the geoprocessing object
                Geoprocessor geop = new Geoprocessor(); //Geoprocess with a small p is for Manage assemblies, the one with capital P is for the other type
                geop.AddOutputsToMap = false;

                //Execute
                try
                {
                    geop.Execute(doAppend as IGPProcess, null);
                }
                catch
                {
                    //Declare output ref
                    object errorType = null;
                    //Show error message
                    MessageBox.Show(geop.GetMessages(ref errorType));
                }
            }
            catch (Exception appendDataWithFieldMapExcept)
            {
                int lineNumber = GSC_ProjectEditor.Exceptions.LineNumber(appendDataWithFieldMapExcept);
                MessageBox.Show("appendDataWithFieldMapException(" + lineNumber.ToString() + "): " + appendDataWithFieldMapExcept.StackTrace);
            }

        }

        #endregion

        #region LAUNCH METHODS

        /// <summary>
        /// Get a fully functional geoprocessing object and invoke requested tool
        /// </summary>
        /// <param name="toolbox">Insert toolbox name, or full path in case it's a custom toolbox</param>
        /// <param name="toolName">Insert desire tool name</param>
        /// <param name="inputParameters">Array of needed parameters, refer to tool or ArcGIS help for a complete list of params.</param>
        /// <param name="showMessages">Shows or not gp messages.</param>
        public static IGeoProcessorResult LaunchGeoprocessingTool(string toolbox, string toolName, IVariantArray inputParameters, bool showMessages)
        {

            //Create the geoprocessor
            IGeoProcessor gp = new GeoProcessor();
            IGeoProcessorResult gpResult;

            //Invoke the requested tool
            try
            {

                //Setting the object to project custom toolbox
                if (toolbox != null)
                {
                    gp.AddToolbox(toolbox);
                }

                gpResult = gp.Execute(toolName, inputParameters, null);
                if (showMessages == true)
                {
                    MessageBox.Show(gpResult.GetMessages(0));
                }

            }
            catch (Exception LaunchGeoprocessingToolException)
            {
                gpResult = null;

                int lineNumber = Exceptions.LineNumber(LaunchGeoprocessingToolException);

                //MessageBox.Show("LaunchGeoprocessingTool (" + lineNumber.ToString() + "): " + LaunchGeoprocessingToolException.Message);
                MessageBox.Show(gp.GetMessages(0));
            }

            return gpResult;
        }

        #endregion

        #region IDENTITY METHODS

        /// <summary>
        /// Will perform an identity analysis.
        /// Used in the QC tools to validate contacts vs ages
        /// </summary>
        /// <param name="inFeature">An input line feature</param>
        /// <param name="identityFeature">An input polygon feature</param>
        /// <param name="outputFeature">The output resulting feature name and path</param>
        /// <param name="outputFeatureResult">The real output</param>
        /// <param name="useInMemoryForOutput">If true, a in_memory workspace will be used for output feature</param>
        /// <returns>Will return path of the output feature</returns>
        public static string IdentityAnalysis(object inFeature, object identityFeature, string outputFeature)
        {
            //Variables
            string outputPath = "";

            //Create the identity method
            Identity doIdentity = new Identity();
            doIdentity.in_features = inFeature;
            doIdentity.out_feature_class = outputFeature;
            doIdentity.identity_features = identityFeature;
            doIdentity.join_attributes = "ONLY_FID";
            doIdentity.relationship = "KEEP_RELATIONSHIPS";

            //Create the geoprocessing object
            Geoprocessor geop = new Geoprocessor(); //Geoprocess with a small p is for Manage assemblies, the one with capital P is for the other type

            //Execute
            try
            {
                IGeoProcessorResult gpResult = geop.Execute(doIdentity as IGPProcess, null) as IGeoProcessorResult;
                IGPValue gpValue = gpResult.GetOutput(0);
                outputPath = gpValue.GetAsText();

            }
            catch
            {
                MessageBox.Show(GSC_ProjectEditor.Properties.Resources.Error_GeoprocessingFail);
            }

            return outputPath;

        }

        #endregion

        #region REPAIR METHODS

        /// <summary>
        /// Will perform a repair geometry operation
        /// </summary>
        /// <param name="inFeature">The feature class to repair geometry</param>
        /// <param name="deleteNull">True if null geometries needs to be deleted</param>
        /// <returns></returns>
        public static string RepairGeometry(object inFeature, bool deleteNull)
        {
            //Variables
            string outputPath = "";

            //Create the identity method
            RepairGeometry doRepair = new RepairGeometry();
            doRepair.in_features = inFeature;
            if (deleteNull)
            {
                doRepair.delete_null = "DELETE_NULL";
            }
            else
            {
                doRepair.delete_null = "KEEP_NULL";
            }
            

            //Create the geoprocessing object
            Geoprocessor geop = new Geoprocessor(); //Geoprocess with a small p is for Manage assemblies, the one with capital P is for the other type

            //Execute
            try
            {
                IGeoProcessorResult gpResult = geop.Execute(doRepair as IGPProcess, null) as IGeoProcessorResult;
                IGPValue gpValue = gpResult.GetOutput(0);
                outputPath = gpValue.GetAsText();

            }
            catch
            {
                MessageBox.Show(GSC_ProjectEditor.Properties.Resources.Error_GeoprocessingFail);
            }

            return outputPath;

        }

        #endregion

        #region MULTI-PART TO SINGLE-PART

        /// <summary>
        /// Will explode an input feature multiple part geometries into singles parts in the output feature class.
        /// </summary>
        /// <param name="inFeature"></param>
        /// <param name="outFeature"></param>
        /// <returns></returns>
        public static string MultiPartToSinglePart(object inFeature, object outFeature)
        {
            //Variables
            string outputPath = "";

            //Create the identity method
            MultipartToSinglepart explode = new MultipartToSinglepart();
            explode.in_features = inFeature;
            explode.out_feature_class = outFeature;

            //Create the geoprocessing object
            Geoprocessor geop = new Geoprocessor(); //Geoprocess with a small p is for Manage assemblies, the one with capital P is for the other type

            //Execute
            try
            {
                IGeoProcessorResult gpResult = geop.Execute(explode as IGPProcess, null) as IGeoProcessorResult;
                IGPValue gpValue = gpResult.GetOutput(0);
                outputPath = gpValue.GetAsText();

            }
            catch
            {
                MessageBox.Show(GSC_ProjectEditor.Properties.Resources.Error_GeoprocessingFail);
            }

            return outputPath;

        }

        #endregion

        #region CLIP METHODS

        /// <summary>
        /// Will clip input feature to an output one from a given feature that will serve as a cutter
        /// </summary>
        /// <param name="inFeature">The input feature layer or feature class object</param>
        /// <param name="outputFeatures">The output path for the new copy</param>
        /// <returns></returns>
        public static string ClipFeature(object inFeature, object clipFeature, string outputFeature)
        {
            //Variables
            string outputPath = "";

            //Create a copy feature method
            ESRI.ArcGIS.AnalysisTools.Clip clipFeat = new ESRI.ArcGIS.AnalysisTools.Clip();
            clipFeat.in_features = inFeature;
            clipFeat.clip_features = clipFeature;
            clipFeat.out_feature_class = outputFeature;
            //Create the geoprocessing object
            Geoprocessor geop = new Geoprocessor();

            //Execute
            try
            {
                IGeoProcessorResult gpResult = geop.Execute(clipFeat as IGPProcess, null) as IGeoProcessorResult;
                IGPValue gpValue = gpResult.GetOutput(0);
                outputPath = gpValue.GetAsText();
            }
            catch (Exception copyFeaturesException)
            {

                MessageBox.Show(copyFeaturesException.StackTrace);
            }

            return outputPath;
        }

        #endregion

        #region CONVERT METHODS

        /// <summary>
        /// Will convert a given feature class into a shapefile inside a given folder path
        /// </summary>
        /// <param name="inFeature">The feature class to convert into a shapefile</param>
        /// <param name="outputFolder">The folder that will contain the exported shapefile</param>
        /// <param name="convertDomainCodeToDescription">True to add new fields that will contain domain and subtype descriptions instead of codes.</param>
        /// <returns></returns>
        public static string ConvertFeatureClassToShapefile(object inFeature, object outputFolder, bool convertDomainCodeToDescription)
        {
            ///DEV NOTES
            ///Using FeatureToShapefile won't transfer domain code to description in the output shapefile
            ///Using FeatureClassToFeatureClass will...

            //Variables
            string outputPath = "";
            IFeatureLayer inFL = inFeature as IFeatureLayer;
            IDataset inDataset = inFL as IDataset;

            //Create an export feature to shapefile method
            ESRI.ArcGIS.ConversionTools.FeatureClassToFeatureClass exportFeat = new ESRI.ArcGIS.ConversionTools.FeatureClassToFeatureClass();
            exportFeat.in_features = inFL;
            exportFeat.out_name = inDataset.Name + ".shp";
            exportFeat.out_path = outputFolder;

            //Create the geoprocessing object
            IGeoProcessor mainGeoprocessor = new GeoProcessorClass();
            mainGeoprocessor.AddOutputsToMap = false;


            //Set the environment of the geoprocessor
            if (convertDomainCodeToDescription)
            {

                //Create a boolean geoprocessing object
                IGPBoolean boolGP = new GPBooleanClass();
                boolGP.Value = true; //Set value
                mainGeoprocessor.SetEnvironmentValue("transferDomains", (IGPValue)boolGP); //Set env.

            }

            //Execute
            try
            {
                //Create the geoprocessor
                Geoprocessor procedureGeoprocessor = new Geoprocessor();

                //Launch
                IGeoProcessorResult gpResult = procedureGeoprocessor.Execute(exportFeat as IGPProcess, null) as IGeoProcessorResult;
                IGPValue gpValue = gpResult.GetOutput(0);
                outputPath = gpValue.GetAsText();
            }
            catch (Exception)
            {

            }

            return outputPath;
        }

        public static string ConvertTableToExcel(ITable inTable, object outputExcelPath)
        {

            //Variables
            string outputPath = "";

            //Create an export feature to shapefile method
            ESRI.ArcGIS.ConversionTools.TableToExcel exportTable = new ESRI.ArcGIS.ConversionTools.TableToExcel();
            exportTable.Input_Table = inTable ;
            exportTable.Output_Excel_File = outputExcelPath;
            exportTable.Use_domain_and_subtype_description = "ALIAS";
            exportTable.Use_field_alias_as_column_header = "DESCRIPTION";

            ////Create the geoprocessing object
            //IGeoProcessor mainGeoprocessor = new GeoProcessorClass();
            //mainGeoprocessor.AddOutputsToMap = false;


            //Execute
            try
            {
                //Create the geoprocessor
                Geoprocessor procedureGeoprocessor = new Geoprocessor();

                //Launch
                IGeoProcessorResult gpResult = procedureGeoprocessor.Execute(exportTable as IGPProcess, null) as IGeoProcessorResult;
                IGPValue gpValue = gpResult.GetOutput(0);
                outputPath = gpValue.GetAsText();
            }
            catch (Exception)
            {

            }

            return outputPath;
        }

        /// <summary>
        /// Will convert a given table into another one with a given format
        /// </summary>
        /// <param name="formatType">Could be of .csv, .txt or .dbf</param>
        public static void ConvertTableToTable(ITable inTable, string formatType, string outputTablePath, string inTableName)
        {
            //Variables
            string fullOutputPath = System.IO.Path.Combine(outputTablePath, inTableName + formatType);

            //Create an export feature to shapefile method
            TableToTable exportTable = new TableToTable();
            exportTable.in_rows = inTable;
            exportTable.out_path = outputTablePath;
            exportTable.out_name = inTableName + formatType;

            //Execute
            try
            {
                //Create the geoprocessor
                Geoprocessor procedureGeoprocessor = new Geoprocessor();

                //Launch
                procedureGeoprocessor.Execute(exportTable as IGPProcess, null);
            }
            catch (Exception)
            {
            }

        }

        #endregion

        #region DISSOLVE 

        /// <summary>
        /// From given parameter will dissolve a feature class based on field.
        /// </summary>
        /// <param name="inFC">The input feature to dissolve</param>
        /// <param name="outFC">The output resulting feature class</param>
        /// <param name="dissolveField">A field name string to dissolve with</param>
        /// <param name="statFields">A string like "FieldName Stat;FieldName2 Stat", "SOURCEID FIRST;CREATORID FIRST"</param>
        public static string DissolveFeatureClass(IFeatureClass inFC, string outFC, string dissolveField, string statFields)
        {
            //Variable
            string outputPath = string.Empty;

            //Create the dissolve object
            Dissolve dissolve = new Dissolve();
            dissolve.dissolve_field = dissolveField;
            dissolve.in_features = inFC;
            dissolve.multi_part = "SINGLE_PART";
            dissolve.statistics_fields = statFields;
            dissolve.out_feature_class = outFC;

            //Create the geoprocessing object
            Geoprocessor geop = new Geoprocessor(); //Geoprocess with a small p is for Manage assemblies, the one with capital P is for the other type
            geop.AddOutputsToMap = false;

            //Execute
            try
            {
                IGeoProcessorResult gpResult = geop.Execute(dissolve as IGPProcess, null) as IGeoProcessorResult;
                IGPValue gpValue = gpResult.GetOutput(0);
                outputPath = gpValue.GetAsText();
            }
            catch (Exception LaunchGeoprocessingToolException)
            {
                //Declare output ref
                object errorType = null;
                //Show error message
                MessageBox.Show(LaunchGeoprocessingToolException.StackTrace + "; " + geop.GetMessages(ref errorType));
            }

            return outputPath;
        }

        #endregion

        #region PROJECT

        /// <summary>
        /// Will project a given feature class to another spatial reference
        /// </summary>
        /// <param name="inDataset"></param>
        /// <param name="outDataset"></param>
        /// <param name="inSpatialRef"></param>
        /// <param name="outSpatialRef"></param>
        public static void ProjectFeatureClass(object inDataset, object outDataset, object outSpatialRef) 
        {
            try
            {
               
                //Create the project method
                Project doProject = new Project();
                //doProject.in_coor_system = inSpatialRef;
                doProject.in_dataset = inDataset;
                doProject.out_coor_system = outSpatialRef;
                doProject.out_dataset = outDataset;

                //Create the geoprocessing object
                Geoprocessor geop = new Geoprocessor(); //Geoprocess with a small p is for Manage assemblies, the one with capital P is for the other type
                geop.AddOutputsToMap = false;

                //Execute
                try
                {
                    geop.Execute(doProject as IGPProcess, null);
                }
                catch
                {
                    //Declare output ref
                    object errorType = null;
                    //Show error message
                    MessageBox.Show(geop.GetMessages(ref errorType));
                }
            }
            catch (Exception projectFeatureClass)
            {
                int lineNumber = GSC_ProjectEditor.Exceptions.LineNumber(projectFeatureClass);
                MessageBox.Show("projectFeatureClass(" + lineNumber.ToString() + "): " + projectFeatureClass.StackTrace);
            }
        }
        #endregion

        #region DELETE
        /// <summary>
        /// Will delete any in dataset from a workspace.
        /// </summary>
        /// <param name="inDataset"></param>
        /// <returns></returns>
        public static string DeleteDataset(IDataset inDataset)
        {
            //Variables
            string outputPathResult = "";

            //Create a copy method
            ESRI.ArcGIS.DataManagementTools.Delete delObj = new ESRI.ArcGIS.DataManagementTools.Delete();
            delObj.in_data = inDataset;

            //Create the geoprocessing object
            Geoprocessor geop = new Geoprocessor();

            //Execute
            try
            {
                //MessageBox.Show("here");
                IGeoProcessorResult gpResult = geop.Execute(delObj as IGPProcess, null) as IGeoProcessorResult;
                IGPValue gpValue = gpResult.GetOutput(0);
                outputPathResult = gpValue.GetAsText();
            }
            catch (Exception CopyAnyDataset)
            {
                MessageBox.Show(CopyAnyDataset.StackTrace + "; " + CopyAnyDataset.Message);
            }

            return outputPathResult;
        }
        #endregion
    }
}
