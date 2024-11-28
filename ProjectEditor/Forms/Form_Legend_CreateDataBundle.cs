using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Editor;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.GeoDatabaseUI;
using xl = Microsoft.Office.Interop.Excel;

namespace GSC_ProjectEditor
{
    public partial class Form_Legend_CreateDataBundle : Form
    {
        #region Main Variables and declaration

        public const string oidField = GSC_ProjectEditor.Constants.DatabaseFields.ObjectID;
        public string databaseExtension = ".gdb";

        //LEGEND TREE TABLE
        private const string tTreeTable = GSC_ProjectEditor.Constants.Database.TLegendTree;
        private const string tTreeTableItemID = GSC_ProjectEditor.Constants.DatabaseFields.LegendTreeItemID;
        private const string tTreeTableDescID = GSC_ProjectEditor.Constants.DatabaseFields.LegendTreeDescID;
        private const string tTreeTableCGMID = GSC_ProjectEditor.Constants.DatabaseFields.LegendTreeCGM;

        //LEGEND GENERATOR TABLE
        private const string tLegendGen = GSC_ProjectEditor.Constants.Database.TLegendGene;
        private const string tLegendGenItemId = GSC_ProjectEditor.Constants.DatabaseFields.LegendLabelID;

        //LEGEND DESCRIPTION
        private const string tLegendDesc = GSC_ProjectEditor.Constants.Database.TLegendDescription;
        private const string tLegendDescID = GSC_ProjectEditor.Constants.DatabaseFields.LegendDescriptionID;

        //CGM_MAP_INDEX
        private const string fcCGM = GSC_ProjectEditor.Constants.Database.FCGMIndex;
        private const string fcCGMID = GSC_ProjectEditor.Constants.DatabaseFields.FCGM_ID;

        //FEATURE CLASSES
        private const string pointFCName = GSC_ProjectEditor.Constants.Database.FGeopoint;
        private const string lineFCName = GSC_ProjectEditor.Constants.Database.FGeoline;
        private const string polyFCName = GSC_ProjectEditor.Constants.Database.FGeopoly;

        //GROUP LAYERS
        private const string interpretationGLName = GSC_ProjectEditor.Constants.Layers.GroupInterpretation;
        private const string bedrockGLNameCGM = GSC_ProjectEditor.Constants.Layers.CGMGeologyBedrock;

        //Other
        public string MainDico = "Main"; //Keyword used to get a dictionnary main value list
        public string displayPubField = GSC_ProjectEditor.Constants.DatabaseFields.FGeolineDisplayPub;
        public string mxd = ".mxd";
        private const string mapUnitDom = GSC_ProjectEditor.Constants.DatabaseDomains.MapUnit;

        //Triggers
        public delegate void publicationProcessEventHandler(object sender, EventArgs processEvent);
        public event publicationProcessEventHandler publicationHasStarted;
        public event publicationProcessEventHandler publicationHasEnded;

        //output result folders paths
        public string pubFolderPath { get; set; } //Main publication folder
        public string dbFolderPath { get; set; } //Folder that will hold the database

        #endregion

        #region DEV NOTES

        /// <summary>
        /// To properly clip the feature classes,  the only available tool resides in geoprocessing methods. The geometry object having only a clip
        /// from a given enveloppe which is not what is wanted for the current tool. Using a gp methods imply having output features. To properly bypass
        /// this problem, a copy of the database is first done then features are emptied. The clip will result in a feature inside in_memory workspace that will
        /// then be appended inside the copied database features.
        /// 
        /// The other problem with this methods is the fact that we can't convert relation to composite ones and wish for all the tables to be cleaned after the clip.
        /// A manual processing needs to be done on those tables, from a list of existing relations.
        /// </summary>

        #endregion

        #region VIEW MODEL

        public Form_Legend_CreateDataBundle()
        {
            //Set culture before init.
            Utilities.Culture.SetCulture();

            InitializeComponent();
            fillMapList();

            this.publicationHasStarted += new publicationProcessEventHandler(FormCreatePublication_publicationHasStarted);
            this.publicationHasEnded += new publicationProcessEventHandler(FormCreatePublication_publicationHasEnded);

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
        /// This button will open up a dialog for user to select a projection from Arc.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_newProjection_Click(object sender, EventArgs e)
        {
            //Get spatial ref prompt
            ISpatialReference getUserSR = GSC_ProjectEditor.Dialog.GetProjectionPrompt(this.Handle.ToInt32());

            //Fill textbox
            if (getUserSR != null)
            {
                this.txtbox_newProjection.Text = getUserSR.Name;
                this.txtbox_newProjection.Tag = getUserSR; //Add the the spatial reference objec to the tag
            }
        }

        /// <summary>
        /// Will prompt a dialog to chose a gdb.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_AddToCartoElement_Click(object sender, EventArgs e)
        {
            //Prompt dialog to select proper database
            string getDBPath = GSC_ProjectEditor.Dialog.GetFGDBPrompt(GSC_ProjectEditor.ArcMap.Application.hWnd, Properties.Resources.Message_SelectCartoElement);

            if (getDBPath != "")
            {
                this.txtbox_AddToCartoElement.Text = getDBPath;
            }
        }

        private void btn_Create_Click(object sender, EventArgs e)
        {
            //Call event to change cusor icon.
            publicationHasStarted(sender, e);

            //List of exception of that won't be treated
            List<string> noClipExceptionList = new List<string>() { GSC_ProjectEditor.Constants.Database.FStudyArea }; //List of feature classes that are not points, but doesn't need a clip to be performed on.

            #region Retrieve some information



            //Retrieve some options
            List<string> listedMaps = new List<string>();
            CheckedListBox.CheckedItemCollection checkedList = this.checkedListBox_Maps.CheckedItems;
            foreach (string items in checkedList)
            {
                listedMaps.Add(items.ToString());
            }

            //Get workspace
            string workPath = GSC_ProjectEditor.Properties.Settings.Default.PROJECT_DATABASE_PATH;
            IWorkspace projectWorkspace = GSC_ProjectEditor.Workspace.AccessWorkspace(workPath);

            //Access project folder
            string projectFolderPath = GSC_ProjectEditor.Properties.Settings.Default.PROJECT_WORKSPACE_PATH;

            //Build new paths
            pubFolderPath = System.IO.Path.Combine(projectFolderPath, GSC_ProjectEditor.Constants.Folders.publicationFolder);
            dbFolderPath = string.Empty;

            //Build spatially projected database path
            string projectedDBName = "TEMP_PROJECTED_FCS";
            string projectedDBPath = System.IO.Path.Combine(GSC_ProjectEditor.Properties.Settings.Default.PROJECT_WORKSPACE_PATH, projectedDBName + ".gdb");
            if (System.IO.Directory.Exists(projectedDBPath))
            {
                System.IO.Directory.Delete(projectedDBPath, true);
            }

            #endregion

            #region Iterate through map selection and process

            //Treat all maps versus refines maps differently
            foreach (string map in listedMaps)
            {
                #region Create new folders hierarchy for new database



                CreateCGMFolders(pubFolderPath, map);

                #endregion

                #region Manage new output workspace
                if (dbFolderPath != string.Empty)
                {
                    IWorkspace copiedDB = CreateNewDatabase(GSC_ProjectEditor.Constants.ProjectDatabaseType.bedrockDB, dbFolderPath, projectWorkspace);

                    if (copiedDB != null)
                    {
                        //Start an edit session on the workspace
                        IWorkspaceEdit copiedDBEdit = copiedDB as IWorkspaceEdit;

                        #region Select current map from project CGM_MAP_INDEX feature class
                        string mapQuery = fcCGMID + " = '" + map + "'";
                        IQueryFilter mapQueryFilter = new QueryFilter();
                        mapQueryFilter.WhereClause = mapQuery;
                        IFeatureClass cgmFeatureClass = GSC_ProjectEditor.FeatureClass.OpenFeatureClass(fcCGM);
                        IFeatureLayer cgmFeatureLayer = new FeatureLayerClass();
                        cgmFeatureLayer.Name = map;
                        cgmFeatureLayer.FeatureClass = cgmFeatureClass;
                        IFeatureSelection mapSelection = cgmFeatureLayer as IFeatureSelection;
                        mapSelection.Clear();
                        mapSelection.SelectFeatures(mapQueryFilter, esriSelectionResultEnum.esriSelectionResultNew, false);

                        #endregion

                        #region Gather list and dictionnary of information
                        //Get some lists
                        List<IFeatureClass> copiedDBFC = GetFeatureClassList(copiedDB);
                        List<IFeatureClass> projectedDBFC = new List<IFeatureClass>(); //Will be used only user needs to project
                        List<ITable> copiedDBTables = GSC_ProjectEditor.Tables.GetTableListFromWorkspace(copiedDB);
                        List<string> copiedDBNames = new List<string>();
                        List<IFeatureDataset> copiedDBFD = GSC_ProjectEditor.FeatureDataset.GetFeatureDatasetList(copiedDB);
                        List<IFeatureDataset> projectedDBFD = new List<IFeatureDataset>(); //Will be used only when user needs to project
                        List<IFeatureDataset> originalDBFD = GSC_ProjectEditor.FeatureDataset.GetFeatureDatasetList(projectWorkspace);
                        List<IFeatureClass> originalDBFC = GetFeatureClassList(projectWorkspace);
                        List<ITable> originalDBTables = GSC_ProjectEditor.Tables.GetTableListFromWorkspace(projectWorkspace);

                        //Build a dictionary of table and their names
                        Dictionary<string, ITable> dbTableDico = new Dictionary<string, ITable>();
                        foreach (ITable t in copiedDBTables)
                        {
                            //Cast to dataset
                            IDataset tDataset = t as IDataset;
                            dbTableDico[tDataset.Name] = t;
                            copiedDBNames.Add(tDataset.Name);
                        }

                        //Build a dictionary of feature class and their names
                        Dictionary<string, IFeatureClass> dbFCDico = new Dictionary<string, IFeatureClass>();
                        foreach (IFeatureClass fc in copiedDBFC)
                        {
                            //Cast to dataset
                            IDataset fcDataset = fc as IDataset;
                            dbFCDico[fcDataset.Name] = fc;

                            copiedDBNames.Add(fcDataset.Name);
                        }

                        #endregion
           
                        #region Apply Spatial reference to new datasets

                        //Get spatial reference
                        ISpatialReference originalSR = GSC_ProjectEditor.SpatialReferences.GetSpatialRef(originalDBFC[0]);
                        ISpatialReference outputSR = originalSR; //Default

                        //Check for user input spatial ref
                        if (this.txtbox_newProjection.Tag != null)
                        {
                            outputSR = this.txtbox_newProjection.Tag as ISpatialReference;

                        }

                        foreach (IFeatureDataset fds in copiedDBFD)
                        {
                            GSC_ProjectEditor.SpatialReferences.SetSpatialRef(fds, outputSR);
                        }

                        foreach (IFeatureClass fcs in copiedDBFC)
                        {
                            GSC_ProjectEditor.SpatialReferences.SetSpatialRef(fcs, outputSR);
                        }

                        #endregion

                        #region PROJECT Feature Classes if needed

                        if (this.txtbox_newProjection.Tag != null)
                        {
                            //Variable
                            List<IFeatureClass> originalDBFCNoFD = originalDBFC;

                            //Create a new empty database in which projected fcs will be dumped
                            GSC_ProjectEditor.GeoProcessing.CopyAnyDataset(projectWorkspace, projectedDBPath);
                            IWorkspace spatialProjectedWorkspace = GSC_ProjectEditor.Workspace.AccessWorkspace(projectedDBPath);

                            ////Get rid of all relations, their could be some conflict that prevents projecting some layers
                            //List<IRelationshipClass> projectedRel = GSC_ProjectEditor.RelationshipClass.GetListOfRelationFromWorkspace(spatialProjectedWorkspace);
                            //foreach (IRelationshipClass delRel in projectedRel)
                            //{
                            //    GSC_ProjectEditor.GeoProcessing.DeleteDataset(delRel as IDataset);
                            //}

                            //Project feature datasets
                            spatialProjectedDBFD = GSC_ProjectEditor.FeatureDataset.GetFeatureDatasetList(spatialProjectedWorkspace);
                            spatialProjectedDBFC = GSC_ProjectEditor.FeatureClass.GetFeatureClassListAsFeatureClass(spatialProjectedWorkspace, null);
                            string uniqueID = GSC_ProjectEditor.IDs.GetRandomInt(new Random(Guid.NewGuid().GetHashCode())).ToString();
                            foreach (IFeatureDataset spatialProjectedFD in spatialProjectedDBFD)
                            {
                                //Create path to new feature class
                                IDataset spatialProjectedFDDataset = spatialProjectedFD as IDataset;
                                if (spatialProjectedFDDataset.CanRename())
                                {
                                    spatialProjectedFDDataset.Rename(spatialProjectedFDDataset.Name + uniqueID);
                                }

                                //Project
                                string outputProjectedPath = System.IO.Path.Combine(spatialProjectedWorkspace.PathName, spatialProjectedFDDataset.Name.Replace(uniqueID, ""));
                                GSC_ProjectEditor.GeoProcessing.ProjectFeatureClass(spatialProjectedFD, outputProjectedPath, outputSR);

                            }

                            //Project feature classes out of feature datasets
                            //spatialProjectedDBFC = GSC_ProjectEditor.FeatureClass.GetFeatureClassListAsFeatureClass(spatialProjectedWorkspace, null);
                            foreach (IFeatureClass tempFC in spatialProjectedDBFC)
                            {
                                //Create path to new feature class
                                IDataset tempDataset = tempFC as IDataset;

                                //Project only feature classes that are not in a feature dataset
                                if (!tempDataset.Category.Contains("Dataset"))
                                {
                                    //Rename
                                    if (tempDataset.CanRename())
                                    {
                                        tempDataset.Rename(tempDataset.Name + uniqueID);
                                    }

                                    string outputProjectedPath = System.IO.Path.Combine(spatialProjectedWorkspace.PathName, tempDataset.Name.Replace(uniqueID, ""));

                                    //Project
                                    GSC_ProjectEditor.GeoProcessing.ProjectFeatureClass(tempFC, outputProjectedPath, outputSR);
                                }

                            }

                            //Rebuild list of feature class
                            spatialProjectedDBFC = GetFeatureClassList(spatialProjectedWorkspace);

                            //Reset cgm feature layer to select proper map else spatial intersection doesn't work between two different projections
                            cgmFeatureClass = GSC_ProjectEditor.FeatureClass.OpenFeatureClassFromWorkspace(spatialProjectedWorkspace, fcCGM);
                            cgmFeatureLayer = new FeatureLayerClass();
                            cgmFeatureLayer.Name = map;
                            cgmFeatureLayer.FeatureClass = cgmFeatureClass;
                            mapSelection = cgmFeatureLayer as IFeatureSelection;
                            mapSelection.Clear();
                            mapSelection.SelectFeatures(mapQueryFilter, esriSelectionResultEnum.esriSelectionResultNew, false);

                        }

                        #endregion

                        #region EMPTY already filled tables from new database so the load doesn't double everything.
                        GSC_ProjectEditor.Tables.EmptyTable(copiedDB, GSC_ProjectEditor.Constants.Database.TGeolineSymbol);
                        GSC_ProjectEditor.Tables.EmptyTable(copiedDB, GSC_ProjectEditor.Constants.Database.TGeopointSymbol);
                        GSC_ProjectEditor.Tables.EmptyTable(copiedDB, GSC_ProjectEditor.Constants.Database.TOrganisation);

                        #endregion

                        #region COPY EXTRA TABLES (P_LEGEND_GENERATOR, CARTO tables, ...)

                        //Get a list of cartographic tables
                        List<ITable> fullTableList = GSC_ProjectEditor.Tables.GetTableListFromWorkspace(projectWorkspace);
                        foreach (ITable tabl in fullTableList)
                        {
                            //Cast to dataset in order to retrieve name
                            IDataset datas = (IDataset)tabl;
                            if (datas.BrowseName.Contains(GSC_ProjectEditor.Constants.Database.TExtenAttrb))
                            {
                                //Copy them inside new workspace
                                GSC_ProjectEditor.Tables.CopyTableToWorkspace(copiedDB, tabl, datas.BrowseName);
                            }

                            if (
                                (datas.BrowseName.Contains(GSC_ProjectEditor.Constants.Database.tLegendGeneratorTemp) || datas.BrowseName.Contains(GSC_ProjectEditor.Constants.Database.tLegendGeneratorTemp191224)) && datas.BrowseName.Contains(map))
                            {
                                //Copy them inside new workspace
                                GSC_ProjectEditor.Tables.CopyTableToWorkspace(copiedDB, tabl, datas.BrowseName);
                            }


                        }


                        #endregion

                        #region APPEND original to new database copy

                        //Refill names since extra tables have been copied
                        copiedDBNames = GSC_ProjectEditor.Tables.GetTableNameListFromWorkspace(copiedDB);
                        copiedDBNames.AddRange(GSC_ProjectEditor.FeatureClass.GetFeatureClassList(copiedDB, null));


                        //Append tables
                        foreach (ITable originalTables in originalDBTables)
                        {
                            //Cast to dataset
                            IDataset originalDatasetTable = originalTables as IDataset;
                            string originalTableName = originalDatasetTable.Name;

                            if (dbTableDico.ContainsKey(originalTableName))
                            {
                                //Append
                                GSC_ProjectEditor.GeoProcessing.AppendData(originalTables, dbTableDico[originalTableName]);
                            }
                        }


                        //Append features
                        IFeatureCursor mapCursor = cgmFeatureLayer.Search(mapQueryFilter, false);
                        IFeature mapFeat = null;

                        while ((mapFeat = mapCursor.NextFeature()) != null)
                        {
                            if (this.txtbox_newProjection.Tag == null)
                            {
                                foreach (IFeatureClass originalFeatureClasses in originalDBFC)
                                {
                                    IDataset oriFCData = originalFeatureClasses as IDataset;
                                    if (dbFCDico.ContainsKey(oriFCData.Name))
                                    {
                                        //AppendWithSpatialIntersection(originalFeatureClasses, dbFCDico, mapFeat);
                                        GSC_ProjectEditor.GeoProcessing.AppendData(originalFeatureClasses, dbFCDico[oriFCData.Name]); 
                                    }

                                }
                            }
                            else
                            {
                                foreach (IFeatureClass sProjectedFeatureClasses in spatialProjectedDBFC)
                                {
                                    IDataset oriFCData = sProjectedFeatureClasses as IDataset;
                                    if (dbFCDico.ContainsKey(oriFCData.Name))
                                    {
                                        GSC_ProjectEditor.GeoProcessing.AppendData(sProjectedFeatureClasses, dbFCDico[oriFCData.Name]);
                                    }
                                    
                                }
                            }

                        }

                        //Append extra relationships (the ones from cartographic point feature class especially)
                        List<IRelationshipClass> projectedRel = GSC_ProjectEditor.RelationshipClass.GetListOfRelationFromWorkspace(projectWorkspace);
                        foreach (IRelationshipClass copyRel in projectedRel)
                        {
                            IDataset copyRelDataset = copyRel as IDataset;
                            if (copyRelDataset.Name.Contains(Constants.Database.rel_prefix_CartoPnt))
                            {
                                //Recreate a new one
                                IRelationshipClass copiedRelClass = copyRel as IRelationshipClass;
                                IDataset originDataset = copiedRelClass.OriginClass as IDataset;
                                IDataset destinationDataset = copiedRelClass.DestinationClass as IDataset;
                                if (copiedDBNames.Contains(originDataset.Name) && copiedDBNames.Contains(destinationDataset.Name))
                                {
                                    //Counterfeit relation 
                                    RelationshipClass.CreateRelationshipFromExisting(copiedDB, originDataset as IObjectClass, destinationDataset as IObjectClass, copiedRelClass);
                                }



                            }
                        }

                        #endregion

                        #region Update relations to be composite

                        //Build a list of all relationship classes
                        List<IRelationshipClass> copiedDBRelations = GSC_ProjectEditor.RelationshipClass.GetListOfRelationFromWorkspace(copiedDB);
                        foreach (IFeatureDataset fds in originalDBFD)
                        {
                            copiedDBRelations.AddRange(GSC_ProjectEditor.RelationshipClass.GetListOfRelationFromWorkspace(copiedDB, fds));
                        }

                        GSC_ProjectEditor.RelationshipClass.ConvertSimpleToCompositeRelations(copiedDBRelations, true);
                        #endregion

                        #region Manage feature classes clipping and cleaning

                        //Empty all new fcs if geometry exists, else delete whole feature, emptied fcs will be filled with clip results
                        foreach (IFeatureClass fcs in copiedDBFC)
                        {
                            //Process if not exception, some feature classes shouldn't be parsed here.
                            IDataset datasets = fcs as IDataset;

                            if (fcs.FeatureCount(null) > 0)
                            {
                                copiedDBEdit.StartEditing(false);
                                ClipFromMap(projectWorkspace, fcs, cgmFeatureLayer, noClipExceptionList);
                                copiedDBEdit.StopEditing(true);
                            }

                        }

                        #endregion

                        #region ADD GSC_SYMBOL FIELD TO GEOPOLYS

                        //TODO: removed when new unified model is a thing
                        //Add symbol field so the map unit layer in the created mxd doesn't lose it's symbols
                        ITable legendTable = Tables.OpenTableFromWorkspace(copiedDB, Constants.Database.TLegendGene);
                        ITable geopolyTable = Tables.OpenTableFromWorkspace(copiedDB, Constants.Database.FGeopoly);
                        AddSymbolField(copiedDB, legendTable, geopolyTable);

                        #endregion

                        #region FILTER LEGEND TABLES (GENERATOR, TREE, DESCRIPTION)

                        FilterLegendItemsFromMap(copiedDB, map, dbTableDico[tLegendGen], dbTableDico[tTreeTable], dbTableDico[tLegendDesc]);

                        #endregion

                        #region FILTER STUDY AREA INDEX

                        FilterStudyAreaIndex(copiedDB);

                        #endregion

                        #region RECALCULATE SPATIAL EXTENTS
                        //Recalculate spatial extent of all feature classes, since they were clipped the etent
                        //Needs to be refreshed.
                        foreach (IFeatureClass fcs in copiedDBFC)
                        {
                            FeatureClass.UpdateFeatureClassExtent(fcs);
                        }
                        #endregion

                        #region FILTER CGM
                        //Some CGM contours that weren't selected for the data bundle may still appear in the attribute table
                        IFeatureCursor cgmCursor = FeatureClass.GetFeatureCursorFromWorkspace(copiedDB,"Update", null, fcCGM);
                        IFeature cgmFeat = cgmCursor.NextFeature();
                        while (cgmFeat != null)
                        {
                            if (cgmFeat.Shape.IsEmpty)
                            {
                                cgmCursor.DeleteFeature();
                            }
                            
                            cgmFeat = cgmCursor.NextFeature();
                        }
                        ObjectManagement.ReleaseObject(cgmCursor);

                        #endregion

                        #region APPEND DOMAINS (SOURCE, LABELS).

                        AppendDomains(projectWorkspace, copiedDB);

                        #endregion

                        #region Update relations to be simple
                        GSC_ProjectEditor.RelationshipClass.ConvertSimpleToCompositeRelations(copiedDBRelations, false);
                        #endregion

                        #region CREATE MXD

                        //Recreate map units
                        Button_CreateEdit_CreateMapUnits buttonCreateMapUnit = new Button_CreateEdit_CreateMapUnits();
                        buttonCreateMapUnit.CreateMapUnits(copiedDB);

                        IFeatureClass newCGMIndex = GSC_ProjectEditor.FeatureClass.OpenFeatureClassFromWorkspace(copiedDB, GSC_ProjectEditor.Constants.Database.FCGMIndex);

                        CreateAndUpdateNewMXD(map, dbFolderPath, newCGMIndex);

                        #endregion

                        #region DELETE TEMP PROJECTED DATABASE
                        if (System.IO.Directory.Exists(projectedDBPath))
                        {
                            GSC_ProjectEditor.ObjectManagement.ReleaseObject(cgmFeatureClass);
                            GSC_ProjectEditor.ObjectManagement.ReleaseObject(cgmFeatureLayer);
                            GSC_ProjectEditor.ObjectManagement.ReleaseObject(mapSelection);

                            try
                            {
                                System.IO.Directory.Delete(projectedDBPath, true);
                            }
                            catch (Exception)
                            {
                                
                            }
                            
                            
                        }
                       
                        #endregion

                        #region FILL IN CARTO_ELEMENT DATABASE WITH SUBTYPES AND FEATURE CLASSES
                        if (this.txtbox_AddToCartoElement.Text != null && this.txtbox_AddToCartoElement.Text != string.Empty)
	                    {
                            ExportSubtypesToDBPath(dbFCDico, this.txtbox_AddToCartoElement.Text);
	                    }
                        


                        #endregion
                    }

                }
                #endregion

            }

            #endregion

            //Close form
            this.Close();  

            //Call event to change cursor back to default.
            publicationHasEnded(sender, e);

            GSC_ProjectEditor.Messages.ShowEndOfProcess(Properties.Resources.Form_Legend_CreateDataBundleCompleted + " " + pubFolderPath);

        }

        /// <summary>
        /// Starting pub event
        /// Changes cursor for a waiting icon.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void FormCreatePublication_publicationHasStarted(object sender, EventArgs e)
        {
            this.btn_Create.Cursor = Cursors.WaitCursor;
        }

        /// <summary>
        /// Ending pub event
        /// Changes cursor back to default icon.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void FormCreatePublication_publicationHasEnded(object sender, EventArgs e)
        {
            this.btn_Create.Cursor = Cursors.Default;
        }

        #endregion

        #region MODEL

        public IWorkspace CreateNewDatabase(string outName, string outPath, IWorkspace projectWorkspace)
        {
            //Create a new empty container with an xml workspace
            IWorkspace newWorkspace = GSC_ProjectEditor.Workspace.CreateWorkspaceFromResource(GSC_ProjectEditor.Properties.Resources.GSC_BEDROCKGDB_SCHEMA_V2_10.ToString(), outPath, outName, databaseExtension);

            return newWorkspace;
        }

        /// <summary>
        /// Will create a new mxd with already loaded layers from clipped database.
        /// Fieldwork will be added if wanted by user.
        /// Feature class will come in both format, all project data and thematic seperated layers.
        /// </summary>
        /// <param name="outName"></param>
        /// <param name="outPath"></param>
        /// <param name="inCGMap"></param>
        public void CreateAndUpdateNewMXD(string outName, string outPath, IFeatureClass inCGMap)
        {

            //Combine all path and extension
            string mxdTemplatePath = System.IO.Path.Combine(GSC_ProjectEditor.Properties.Settings.Default.PROJECT_WORKSPACE_PATH, Constants.Folders.mxdFolder);
            string mxdResultPathRaw = System.IO.Path.Combine(outPath, outName) + mxd;

            //Create a new empty mxd with proper group layer and layers set up
            IMapDocument newMXD = GSC_ProjectEditor.MXD.OpenMXDFromResource(mxdTemplatePath);

            #region Update dataframe extent and info

            //Get study area feature extent
            IEnvelope studyAreaExtent = GSC_ProjectEditor.FeatureClass.GetFeatureClassEnvelope(inCGMap);

            //Set extent to current active view
            newMXD.ActiveView.Extent = studyAreaExtent;

            //New data frame name
            newMXD.Map[0].Name = outName;

            newMXD.ActiveView.FocusMap.SpatialReference = GSC_ProjectEditor.SpatialReferences.GetSpatialRef(inCGMap);

            #endregion

            #region Fill in the mxd with layers from current one
            //Variables
            IDataset inDataset = inCGMap as IDataset;
            IWorkspace inWorkspace = inDataset.Workspace;
            Dictionary<string, Tuple<string, string>> toCopyLayers = new Dictionary<string, Tuple<string, string>>();
            toCopyLayers[pointFCName] = new Tuple<string, string>(GSC_ProjectEditor.Constants.Layers.CGMGeopoint, GSC_ProjectEditor.Constants.DatabaseFields.FGeopointID);
            toCopyLayers[lineFCName] = new Tuple<string, string>(GSC_ProjectEditor.Constants.Layers.CGMGeoline, GSC_ProjectEditor.Constants.DatabaseFields.FGeolineID);
            toCopyLayers[polyFCName] = new Tuple<string, string>(GSC_ProjectEditor.Constants.Layers.CGMGeopoly, GSC_ProjectEditor.Constants.DatabaseFields.FGeopolyLabel);
            toCopyLayers[fcCGM] = new Tuple<string, string>(GSC_ProjectEditor.Constants.Layers.CGMLimit, GSC_ProjectEditor.Constants.DatabaseFields.FCGM_ID);

            //Build list of arc map addin only objects.
            List<object> arcMapObjects = new List<object>();
            arcMapObjects.Add(newMXD.Map[0]);
            arcMapObjects.Add(newMXD.ActiveView);

            //Rename geology type group layer
            IGroupLayer geologyGL = GSC_ProjectEditor.FeatureLayers.GetGroupLayer(GSC_ProjectEditor.Constants.Layers.CGMGeologyType, newMXD.Map[0]);
            geologyGL.Name = GSC_ProjectEditor.Constants.Layers.CGMGeologyBedrock;

            //Update some feature classes
            foreach (KeyValuePair<string, Tuple<string, string>> items in toCopyLayers)
            {
                List<IFeatureLayer> flToUpdateStyle = new List<IFeatureLayer>();

                //Get the original point layer file
                IFeatureLayer fl = Utilities.MapDocumentFeatureLayers.GetFeatureLayerPreProcessing(items.Key, interpretationGLName);

                //Get a copy
                IFeatureLayer flCopy = GSC_ProjectEditor.ObjectManagement.CopyInputObject(fl) as IFeatureLayer;

                //Change the data source
                IFeatureClass fc = GSC_ProjectEditor.FeatureClass.OpenFeatureClassFromWorkspace(inWorkspace, items.Key);
                flCopy.FeatureClass = fc;

                //For polygons, redo join with legend
                if (items.Key == polyFCName)
                {
                    GSC_ProjectEditor.Joins.RemoveAllJoins(flCopy);
                    updatePolyStyle(flCopy, inWorkspace);
                }

                if (items.Key == lineFCName || items.Key == pointFCName)
                {
                    flToUpdateStyle.AddRange(CreateThemeLayers(flCopy));
                }

                if (items.Key == polyFCName)
                {
                    flToUpdateStyle.AddRange(CreateOverprintThemeLayer(flCopy, items.Value.Item2));
                }

                flToUpdateStyle.Add(flCopy);

                foreach (IFeatureLayer fls in flToUpdateStyle)
                {


                    GSC_ProjectEditor.Symbols.RemoveEmptySymbols(fls as IGeoFeatureLayer, fl as IGeoFeatureLayer);

                    //Add point layer to new mxd
                    if (fls.Name.Contains("-") && fls.Name.Contains(polyFCName) && fls.Name != flCopy.Name)
                    {
                        GSC_ProjectEditor.FeatureLayers.AddLayerToArcMap(fls as ILayer, GSC_ProjectEditor.Constants.Layers.CGMOverprint, arcMapObjects);
                    }
                    else
                    {
                        GSC_ProjectEditor.FeatureLayers.AddLayerToArcMap(fls as ILayer, items.Value.Item1, arcMapObjects);
                    }

                }

            }


            #endregion

            //Save new MXD as 
            GSC_ProjectEditor.MXD.SaveAsMXD(newMXD, mxdResultPathRaw);

            newMXD.Close();
        }


        public void updatePolyStyle(IFeatureLayer ingeopolyFL, IWorkspace inWorkspace)
        {
            string pathToStyle = GSC_ProjectEditor.Symbols.ValidateStyleFile2(ArcMap.Document);

            Button_CreateEdit_CreateMapUnits buttonCreateMapUnit = new Button_CreateEdit_CreateMapUnits();
            
            ITable geopolyTable = Tables.OpenTableFromWorkspace(inWorkspace, Constants.Database.FGeopoly);
            int polySymbolField = geopolyTable.FindField(Constants.DatabaseFields.LegendSymbol);
            bool existingSymbolField = false;
            if (polySymbolField > 0)
            {
                existingSymbolField = true;
            }
            string interGroupLayer = GSC_ProjectEditor.Properties.Resources.GroupLayerInterpretation;

            //Access other attributes of layer with geofeaturelayer
            IGeoFeatureLayer getGeoFL = ingeopolyFL as IGeoFeatureLayer;

            //Get unique list of labels within feature
            List<string> uniqueLabelList = GSC_ProjectEditor.Tables.GetUniqueFieldValuesFromWorkspace(inWorkspace, polyFCName, Constants.DatabaseFields.FGeopolyLabel, null, false, null)["Main"];

            //Sort list to get map unit in alphabetical order
            uniqueLabelList.Sort();

            //Get dictionnary of label codes and description from the domain itself
            Dictionary<string, string> labelDico = GSC_ProjectEditor.Tables.GetUniqueDicoValuesFromWorkspace(inWorkspace, Constants.Database.TLegendGene, Constants.DatabaseFields.LegendLabelID, GSC_ProjectEditor.Constants.DatabaseFields.LegendGISDisplay, string.Empty);

            //Verify if layer is already symbolized with wanted style, if not, will create new renderer for it
            buttonCreateMapUnit.validatePolyRenderer(getGeoFL, pathToStyle, existingSymbolField, true);

            //Loop through feature to match symbol field value with wanted style
            IFeatureCursor cursorLayer = ingeopolyFL.Search(null, true);  //Access a special object that will enable search within a join
            int symIndex = cursorLayer.FindField(Constants.DatabaseFields.LegendSymbol);
            int geoPolyIndex = cursorLayer.FindField(Constants.DatabaseFields.FGeopolyLabel);
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
                        buttonCreateMapUnit.validatePolyInRenderer(getGeoFL, descLabel, currentSymValue, currentGeopolyID, pathToStyle);

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

        /// <summary>
        /// Update geoline symbols from project style file.
        /// </summary>
        public void updateStyle(IFeatureLayer layerToUpdate, IWorkspace inWorkspace, string inFCName, string idField)
        {
            //Get the feature class name and workspace
            IFeatureClass fcToUpdate = layerToUpdate.FeatureClass;

            //Get unique list of geoline ids within geoline
            List<string> uniqueList = GSC_ProjectEditor.Tables.GetUniqueFieldValuesFromWorkspace(inWorkspace, inFCName, idField, null, false, null)["Main"];

            //Access other attributes of layer with geofeaturelayer
            IGeoFeatureLayer geoFL = layerToUpdate as IGeoFeatureLayer;

            //Get the unique renderer style from 
            IUniqueValueRenderer uniqueRender = geoFL.Renderer as UniqueValueRenderer;

            int idCount = 0;

            //Get number of current symbols within layer
            if (uniqueRender != null)
            {
                idCount = uniqueRender.ValueCount;

                //Detect if symbol exists
                for (int idxValueLine = 0; idxValueLine < idCount; idxValueLine++)
                {
                    //Get full value
                    string fullValue = uniqueRender.get_Value(idxValueLine);

                    //Filter value to retrieve geolineID
                    char[] charValue = GSC_ProjectEditor.Constants.Symbol4Layers.fieldDelimeter.ToCharArray();
                    string[] splitedValue = fullValue.Split(charValue);
                    string IDValue = splitedValue[splitedValue.Count() - 1];

                    if (!uniqueList.Contains(IDValue))
                    {
                        //Remove
                        uniqueRender.RemoveValue(fullValue);

                        //Reset count
                        idCount = uniqueRender.ValueCount;
                        idxValueLine = idxValueLine - 1;

                    }

                }

            }

        }

        /// <summary>
        /// Will seperate a given layer into sub layers based on subtypes
        /// </summary>
        /// <param name="inFL"></param>
        /// <returns></returns>
        public List<IFeatureLayer> CreateThemeLayers(IFeatureLayer inFL)
        {
            //Variables
            List<IFeatureLayer> themeLayers = new List<IFeatureLayer>();

            //Get current subtype list from it.
            Dictionary<string, int> subDico = GSC_ProjectEditor.Subtypes.GetSubtypeDicoFromLayer(inFL);

            if (subDico.Count != 0)
            {
                //Iterate through all subtypes to create new layers
                foreach (KeyValuePair<string, int> subKV in subDico)
                {

                    #region Build proper definition query
                    //Prepare a list of value to create selection from
                    List<string> valueList = new List<string>();
                    valueList.Add(subKV.Value.ToString());

                    //Create a new layer name
                    string newLayerName = inFL.Name + " - " + subKV.Key;

                    //Get current subtype field name
                    string subFieldName = GSC_ProjectEditor.Subtypes.GetSubtypeFieldFromFeatureLayer(inFL);

                    //Get a query based on undefined order list
                    IDataset currentFLDataset = inFL.FeatureClass as IDataset;
                    string buildQuery = GSC_ProjectEditor.Queries.BuildQueryFromStringList(subFieldName, new List<string> { subKV.Value.ToString() }, "Int", "OR", currentFLDataset.Workspace);

                    #endregion

                    #region Make a copy of original layer and add it to the proper place in the TOC

                    //Create a copy of original feature layer
                    object copiedFL = GSC_ProjectEditor.ObjectManagement.CopyInputObject(inFL as object);

                    //Rename the layer
                    IFeatureLayer copiedFeatureLayer = copiedFL as IFeatureLayer;
                    copiedFeatureLayer.Name = newLayerName;

                    #endregion

                    #region Add proper definition query within the new layer

                    //Cast the layer definition from the feature layer object
                    IFeatureLayerDefinition newFLD = copiedFeatureLayer as IFeatureLayerDefinition;

                    //Add the query
                    newFLD.DefinitionExpression = buildQuery;

                    #endregion

                    #region Remove empty symbols, Update renderer and add new fl in TOC

                    themeLayers.Add(copiedFeatureLayer);

                    #endregion

                }
            }

            return themeLayers;

        }

        /// <summary>
        /// Will seperate a given layer into sub layer if it contains overprint polygons
        /// </summary>
        /// <param name="inFL"></param>
        /// <param name="idFieldName"></param>
        /// <returns></returns>
        public List<IFeatureLayer> CreateOverprintThemeLayer(IFeatureLayer inFL, string idFieldName)
        {
            //Variable
            List<IFeatureLayer> overprintLayers = new List<IFeatureLayer>();
            //Layers name
            string newOverprintLayerName = inFL.Name + " - " + GSC_ProjectEditor.Constants.Layers.overprintThematic;

            //Get a list of all labels from the map unit domain
            Dictionary<string, string> allMapUnits = GSC_ProjectEditor.Domains.GetDomDico(mapUnitDom, "Description");
            List<string> overprintsMapUnits = new List<string>();

            //Get a smaller list of only the overprints labels
            foreach (KeyValuePair<string, string> kv in allMapUnits)
            {
                if (kv.Key.Contains(GSC_ProjectEditor.Constants.ValueKeywords.labelOverprint))
                {
                    //Add to overprint list
                    overprintsMapUnits.Add(kv.Value);
                }
            }

            if (overprintsMapUnits.Count > 0)
            {
                //Get a query based on undefined order list
                IDataset mapUnitFLDataset = inFL.FeatureClass as IDataset;
                string buildOverQuery = GSC_ProjectEditor.Queries.BuildQueryFromStringList(idFieldName, overprintsMapUnits, "String", "OR", mapUnitFLDataset.Workspace);

                #region Make a copy of original layer and add it to the proper place in the TOC

                //Create a copy of original feature layer
                object copiedFL = GSC_ProjectEditor.ObjectManagement.CopyInputObject(inFL as object);

                //Rename the layer
                IFeatureLayer copiedFeatureLayer = copiedFL as IFeatureLayer;
                copiedFeatureLayer.Name = newOverprintLayerName;

                #endregion

                #region Add proper definition query within the new layer

                //Cast the layer definition from the feature layer object
                IFeatureLayerDefinition newFLD = copiedFeatureLayer as IFeatureLayerDefinition;

                //Add the query
                newFLD.DefinitionExpression = buildOverQuery;

                #endregion

                #region Remove empty symbols, Update renderer and add new fl in TOC

                overprintLayers.Add(copiedFeatureLayer);

                #endregion
            }



            return overprintLayers;

        }

        /// <summary>
        /// Will return a feature class list from a whole workspace and feature datasets
        /// </summary>
        /// <param name="fromWorkspace">The workspace to retrieve the list from</param>
        /// <returns></returns>
        public List<IFeatureClass> GetFeatureClassList(IWorkspace fromWorkspace)
        {
            //Get a list of feature dataset inside workspace
            List<IFeatureDataset> fdList = GSC_ProjectEditor.FeatureDataset.GetFeatureDatasetList(fromWorkspace);

            //Get a list of feature classes inside this database
            List<IFeatureClass> fcList = GSC_ProjectEditor.FeatureClass.GetFeatureClassListAsFeatureClass(fromWorkspace, null); //Init with root feature classes inside database
            foreach (IFeatureDataset fd in fdList)
            {
                fcList.AddRange(GSC_ProjectEditor.FeatureClass.GetFeatureClassListAsFeatureClass(fromWorkspace, fd));
            }

            return fcList;
        }

        /// <summary>
        /// Will clip a given feature from a clip zone into an output feature class from a given map id.
        /// </summary>
        /// <param name="inFeature">The input feature that will be clipped</param>
        /// <param name="outFeature">The output feature that will contain the results</param>
        /// <param name="currentMap">The wanted map id to clip with</param>
        public void ClipFromMap(IWorkspace projectWorkspace, IFeatureClass outputFeature, IFeatureLayer selectedMap, List<string> noClipExceptionList)
        {
            //Get current feature name
            IDataset dataset = outputFeature as IDataset;
            string datasetName = dataset.Name;

            //Get original feature class, that will be clipped, from project workspace
            IFeatureClass oriFC = GSC_ProjectEditor.FeatureClass.OpenFeatureClass(datasetName);

            //Acces map geometry
            IFeatureSelection mapFeatureSelection = selectedMap as IFeatureSelection;
            ISelectionSet mapSelection = mapFeatureSelection.SelectionSet;
            if (mapSelection.Count == 1)
            {
                IEnumIDs mapIDs = mapSelection.IDs;
                int mapID = mapIDs.Next();
                IFeature mapFeature = selectedMap.FeatureClass.GetFeature(mapID);

                //Check geometry and proceed differently from type
                if (!noClipExceptionList.Contains(datasetName))
                {
                    if (outputFeature.ShapeType == esriGeometryType.esriGeometryPolyline || outputFeature.ShapeType == esriGeometryType.esriGeometryLine)
                    {
                        ClipLineFromMap(outputFeature, mapFeature);
                    }
                    else if (outputFeature.ShapeType == esriGeometryType.esriGeometryPolygon)
                    {
                        ClipPolygonFromMap(outputFeature, mapFeature);
                    }
                    else if (outputFeature.ShapeType == esriGeometryType.esriGeometryPoint)
                    {
                        //Delete disjoint (outside intersect) geometries first
                        DeleteDisjointSpatialQueryResults(mapFeature.ShapeCopy, outputFeature);
                    }
                }
                else
                {
                    //Do nothing, the delete disjoint has make the work
                }

            }
        }

        /// <summary>
        /// Will append the clipped results into the published database that was emptied previously
        /// </summary>
        /// <param name="outputFeature">The output feature path</param>
        /// <param name="featureToAppend">The resulting clip feature class</param>
        public void AppendClipResults(string outputFeature, IFeatureClass featureToAppend)
        {
            //Cast clip result as a feature class
            IFeatureClass outputFC = GSC_ProjectEditor.FeatureClass.OpenFeatureClassFromInMemory(outputFeature);

            ////Append
            //GSC_ProjectEditor.GeoProcessing.AppendData(outputFC, featureToAppend);

            //Get collection of current clip result geometries
            IFeatureCursor clipFeatures = featureToAppend.Search(null, true);
            IGeometry[] geomArray = new IGeometry[featureToAppend.FeatureCount(null)];
            int iterator = 0;
            IFeature clipFeat = clipFeatures.NextFeature();
            while (clipFeat != null)
            {
                IGeometry clipGeom = clipFeat.ShapeCopy;
                geomArray[iterator] = clipGeom;
                iterator = iterator + 1;
                clipFeat = clipFeatures.NextFeature();
            }
            //Release cursor
            System.Runtime.InteropServices.Marshal.ReleaseComObject(clipFeatures);
        }

        /// <summary>
        /// Will cut the line at the intersection of the selected map.
        /// </summary>
        /// <param name="outputFeature">The output feature that will the</param>
        /// <param name="selectedMap"></param>
        public void ClipLineFromMap(IFeatureClass outputFeature, IFeature mapFeature)
        {

            //Iterate through lines and intersect them with map
            IFeatureCursor lineFeatureCursor = outputFeature.Update(null, true);
            IFeature lineFeature = lineFeatureCursor.NextFeature();

            //Is current line z aware
            IZAware currentZ = lineFeature.Shape as IZAware;
            bool currentLineZ = currentZ.ZAware;

            while (lineFeature != null)
            {
                //Cast feature to topological operator
                ITopologicalOperator topoOpo = (ITopologicalOperator)lineFeature.ShapeCopy;
                IGeometry intersectResult = topoOpo.Intersect(mapFeature.ShapeCopy, esriGeometryDimension.esriGeometry1Dimension);

               
                if (currentLineZ)
                {
                    GSC_ProjectEditor.Geometry.MakeZAware(intersectResult);
                    lineFeature.Shape = intersectResult;
                }
                else
                {
                    lineFeature.Shape = intersectResult;
                }
                

                //Cast to a line feature
                IPolyline line = intersectResult as IPolyline;

                //Validate if lenght is 0, delete if yes
                if (line.Length <= 0)
                {
                    lineFeatureCursor.DeleteFeature();
                }
                else
                {
                    lineFeatureCursor.UpdateFeature(lineFeature);
                }

                lineFeature = lineFeatureCursor.NextFeature();
            }

            //Release cursor
            System.Runtime.InteropServices.Marshal.ReleaseComObject(lineFeatureCursor);
        }

        /// <summary>
        /// Will cut the polygon at the intersection of the selected map.
        /// </summary>
        /// <param name="outputFeature">The output feature that will the</param>
        /// <param name="selectedMap"></param>
        public void ClipPolygonFromMap(IFeatureClass outputFeature, IFeature mapFeature)
        {
            //Iterate through lines and intersect them with map
            IFeatureCursor polyFeatureCursor = outputFeature.Update(null, true);
            IFeature polyFeature = polyFeatureCursor.NextFeature();
            while (polyFeature != null)
            {
                //Cast feature to topological operator
                ITopologicalOperator6 topoOpo = (ITopologicalOperator6)polyFeature.ShapeCopy;
                IGeometry intersectResult = topoOpo.IntersectEx(mapFeature.ShapeCopy, false, esriGeometryDimension.esriGeometry2Dimension);

                //Check for z
                IZAware polyZAware = polyFeature.Shape as IZAware;
                IZAware intersectZAware = intersectResult as IZAware;
                if (polyZAware.ZAware && !intersectZAware.ZAware)
                {
                    GSC_ProjectEditor.Geometry.MakeZAware(intersectResult);
                }

                polyFeature.Shape = intersectResult;

                //Validate if lenght is 0, delete if yes
                polyFeatureCursor.UpdateFeature(polyFeature);

                polyFeature = polyFeatureCursor.NextFeature();
            }

            //Release cursor
            System.Runtime.InteropServices.Marshal.ReleaseComObject(polyFeatureCursor);

        }

        /// <summary>
        /// From a spatial disjoint query, will delete from input feature class searched rows.
        /// </summary>
        /// <param name="spatialGeom">The geometry that will serve as a intersect to select outside features of</param>
        /// <param name="outShapeFieldName">The shapefield name querying against.</param>
        /// <param name="fcToFilter">The feature class to filter the geometries inside of</param>
        public void DeleteDisjointSpatialQueryResults(IGeometry spatialGeom, IFeatureClass fcToFilter)
        {
            //Perform a spatial query
            ISpatialFilter spatialFilter = new SpatialFilterClass();
            spatialFilter.Geometry = spatialGeom;
            spatialFilter.GeometryField = fcToFilter.ShapeFieldName; //Add shape field name of feature class querying against 
            spatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelRelation; //A simple intersection query, inclusive
            spatialFilter.SpatialRelDescription = GSC_ProjectEditor.Constants.TopologicalRelations.disjointQuery;
        
            //Delete lines that fits query (fastest method)
            ITable inTable = fcToFilter as ITable;
            ICursor inTableCursor = inTable.Search((IQueryFilter)spatialFilter, false);
            IRow currentInTableCursorRow = inTableCursor.NextRow();
            while (currentInTableCursorRow!=null)
            {
                currentInTableCursorRow.Delete();
                currentInTableCursorRow = inTableCursor.NextRow();
            }
            //inTable.DeleteSearchedRows((IQueryFilter)spatialFilter);
            GSC_ProjectEditor.ObjectManagement.ReleaseObject(inTableCursor);
        }

        /// <summary>
        /// Will return a string list of all child tables for station feature class
        /// </summary>
        /// <returns></returns>
        public List<string> GetFieldStationChilds()
        {
            //Variables
            List<string> stationChildList = new List<string>();
            stationChildList.Add(GSC_ProjectEditor.Constants.Database.gEarthMath);
            stationChildList.Add(GSC_ProjectEditor.Constants.Database.gMA);
            stationChildList.Add(GSC_ProjectEditor.Constants.Database.gPhoto);

            //Get earth mat childs and add to list
            stationChildList.AddRange(GetFieldEarthmatChilds());

            return stationChildList;
        }

        /// <summary>
        /// Will return a string list of all child tables of earth mat
        /// </summary>
        /// <returns></returns>
        public List<string> GetFieldEarthmatChilds()
        {
            //Variables
            List<string> earthmatChildList = new List<string>();
            earthmatChildList.Add(GSC_ProjectEditor.Constants.Database.gMineral);
            earthmatChildList.Add(GSC_ProjectEditor.Constants.Database.gStruc);
            earthmatChildList.Add(GSC_ProjectEditor.Constants.Database.gSample);

            return earthmatChildList;
        }

        /// <summary>
        /// Will create the complete folder hierarchy for any CGM publication, like carto wants it.
        /// </summary>
        /// <param name="pubFolderPath">The publication folder path</param>
        /// <param name="cgmMapID">The current publshed map id</param>
        /// <param name="dbFolderPath">The database folder path, to use in other process</param>
        /// <param name="shapefileFolderPath">The shapefile folder path to user in other process</param>
        public void CreateCGMFolders(string pubFolderPath, string cgmMapID)
        {
            //Build parent folders path
            List<string> folderPaths = new List<string>();

            dbFolderPath = System.IO.Path.Combine(pubFolderPath, cgmMapID); // ./Publication/CGMID/

            folderPaths.Add(pubFolderPath);
            folderPaths.Add(dbFolderPath); //../Publication/CGMID

            foreach (string paths in folderPaths)
            {
                System.IO.Directory.CreateDirectory(paths);
            }

        }

        /// <summary>
        /// Will delete the features that were noted as not being published.
        /// </summary>
        /// <param name="inputFeatureClassDico">A dictionary containing the complete list of feature classes.</param>
        public void FilterDisplayPubField(Dictionary<string, IFeatureClass> inputFeatureClassDico)
        {
            //Iterate through all feature classes to find the required field
            foreach (KeyValuePair<string, IFeatureClass> keyValueFeatures in inputFeatureClassDico)
            {
                //Check if the feature contains the required field
                if (keyValueFeatures.Value.FindField(displayPubField) != -1)
                {
                    //Build the filter
                    IQueryFilter displayPubFilter = new QueryFilterClass();
                    displayPubFilter.WhereClause = displayPubField + " = " + GSC_ProjectEditor.Constants.DatabaseDomainsValues.BoolNo;

                    //Cast to table and delete
                    ITable featureClassTable = keyValueFeatures.Value as ITable;
                    featureClassTable.DeleteSearchedRows(displayPubFilter);

                }
            }
        }

        /// <summary>
        /// Will filter the legend generator to keep only what is inside the published map
        /// </summary>
        /// <param name="filteredWorskpace">The workspace in which the filter will happen (published database)</param>
        /// <param name="cgmMap">The map id for selection purposes.</param>
        /// <param name="legendGeneratorTable">The legend generator table itself</param>
        public void FilterLegendItemsFromMap(IWorkspace filteredWorskpace, string cgmMap, ITable legendGeneratorTable, ITable legendTreeTable, ITable legendDescriptionTable)
        {
            //Build a list of legend Item ids from tree table
            string cgmidQuery = tTreeTableCGMID + " = '" + cgmMap + "'";
            List<string> legendItemIds = GSC_ProjectEditor.Tables.GetFieldValuesFromWorkspace(filteredWorskpace, tTreeTable, tTreeTableItemID, cgmidQuery);
            List<string> legendDescIds = GSC_ProjectEditor.Tables.GetFieldValuesFromWorkspace(filteredWorskpace, tTreeTable, tTreeTableDescID, cgmidQuery);

            //Start an edit session for the workspace
            IWorkspaceEdit workspaceEdit = filteredWorskpace as IWorkspaceEdit;
            workspaceEdit.StartEditing(false);

            #region Filter Legend Generator
            ICursor legendCursor = legendGeneratorTable.Update(null, true);
            IRow legendRows = legendCursor.NextRow();
            int legendItemIDIndex = legendCursor.FindField(tLegendGenItemId);
            while (legendRows != null)
            {
                try
                {
                    string currentItemID = legendRows.get_Value(legendItemIDIndex).ToString();

                    if (!legendItemIds.Contains(currentItemID))
                    {
                        legendRows.Delete();

                    }
                }
                catch (Exception)
                {

                }

                legendRows = legendCursor.NextRow();
            }
            //Release the cursor or else some lock could happen.
            System.Runtime.InteropServices.Marshal.ReleaseComObject(legendCursor);
            #endregion

            #region Filter Legend Tree table
            IQueryFilter treeFilter = new QueryFilterClass();
            treeFilter.WhereClause = tTreeTableCGMID + " <> '" + cgmMap + "'";
            legendTreeTable.DeleteSearchedRows(treeFilter);
            #endregion

            #region Filter Legend description
            ICursor descCursor = legendDescriptionTable.Update(null, true);
            IRow descRows = descCursor.NextRow();
            int legendDescIDIndex = descCursor.FindField(tLegendDescID);
            while (descRows != null)
            {
                string currentDesc = descRows.get_Value(legendDescIDIndex).ToString();

                if (!legendDescIds.Contains(currentDesc))
                {
                    descRows.Delete();

                }
                descRows = descCursor.NextRow();
            }
            //Release the cursor or else some lock could happen.
            System.Runtime.InteropServices.Marshal.ReleaseComObject(descCursor);


            #endregion

            //Stop an edit session for the workspace
            workspaceEdit.StopEditing(true);
        }

        /// <summary>
        /// Will append original domains to the new publication, since it's a new database user domains will be missing.
        /// </summary>
        public void AppendDomains(IWorkspace originalWork, IWorkspace outputWork)
        {
            //Append all PID domains

            //Manage source domains
            Dictionary<string, string> sourceDomain = GSC_ProjectEditor.Domains.GetDomDico(GSC_ProjectEditor.Constants.DatabaseDomains.Source, "Code");
            GSC_ProjectEditor.Domains.AddDomainValueDictionaryFromWorkspace(outputWork, GSC_ProjectEditor.Constants.DatabaseDomains.Source, sourceDomain);

            //Manage label domains
            Dictionary<string, string> labelDomain = GSC_ProjectEditor.Domains.GetDomDico(GSC_ProjectEditor.Constants.DatabaseDomains.MapUnit, "Code");
            GSC_ProjectEditor.Domains.AddDomainValueDictionaryFromWorkspace(outputWork, GSC_ProjectEditor.Constants.DatabaseDomains.MapUnit, labelDomain);

            //Manage legend item theme
            Dictionary<string, string> legendThemeDomain = GSC_ProjectEditor.Domains.GetDomDico(GSC_ProjectEditor.Constants.DatabaseDomains.legendSymbolTheme, "Code");
            GSC_ProjectEditor.Domains.AddDomainValueDictionaryFromWorkspace(outputWork, GSC_ProjectEditor.Constants.DatabaseDomains.legendSymbolTheme, legendThemeDomain);

            //Manage participant
            Dictionary<string, string> participantDomain = GSC_ProjectEditor.Domains.GetDomDico(GSC_ProjectEditor.Constants.DatabaseDomains.participant, "Code");
            GSC_ProjectEditor.Domains.AddDomainValueDictionaryFromWorkspace(outputWork, GSC_ProjectEditor.Constants.DatabaseDomains.participant, participantDomain);
        }

        /// <summary>
        /// Will clean study area index from remaining polygons in cgm index map and study area features.
        /// </summary>
        /// <param name="copiedDB">The reference workspace to clean study area index from.</param>
        private void FilterStudyAreaIndex(IWorkspace copiedDB)
        {
            //Get a list of unique id values from P_CGM_INDEX_MAP
            List<string> cgmIDs = GSC_ProjectEditor.Tables.GetFieldValuesFromWorkspace(copiedDB, GSC_ProjectEditor.Constants.Database.FCGMIndex, GSC_ProjectEditor.Constants.DatabaseFields.FCGM_RelatedID, null);

            //Get a list of unique id values from P_STUDY_AREA
            List<string> saIDs = GSC_ProjectEditor.Tables.GetFieldValuesFromWorkspace(copiedDB, GSC_ProjectEditor.Constants.Database.FStudyArea, GSC_ProjectEditor.Constants.DatabaseFields.FStudyAreaRelatedID, null);

            //Combine two list
            saIDs.AddRange(cgmIDs);

            //Iterate through index and remove useless values
            ITable indexTable = GSC_ProjectEditor.Tables.OpenTableFromWorkspace(copiedDB, GSC_ProjectEditor.Constants.Database.TStudyAreaIndex);
            ICursor indexCursor = indexTable.Update(null, false);
            IRow indexRow = indexCursor.NextRow();
            int tableRelatedFieldIndex = indexCursor.FindField(GSC_ProjectEditor.Constants.DatabaseFields.TStudyAreaRowID);
            while (indexRow != null)
            {
                if (!saIDs.Contains(indexRow.get_Value(tableRelatedFieldIndex)))
                {
                    //Delete
                    indexRow.Delete();
                }

                indexRow = indexCursor.NextRow();
            }
            GSC_ProjectEditor.ObjectManagement.ReleaseObject(indexCursor);

        }

        /// <summary>
        /// From a given feature class will append it to output
        /// </summary>
        /// <param name="inFC"></param>
        /// <param name="inFCDico"></param>
        /// <param name="inFeature"></param>
        private void AppendWithSpatialIntersection(IFeatureClass inFC, Dictionary<string, IFeatureClass> inFCDico, IFeature inFeature)
        {
            //Cast to dataset
            IDataset originalDatasetFC = inFC as IDataset;
            string originalFCName = originalDatasetFC.Name;

            if (inFCDico.ContainsKey(originalFCName))
            {
                //Make a selection first
                IFeatureLayer oriLayer = new FeatureLayerClass();
                oriLayer.Name = originalFCName;
                oriLayer.FeatureClass = inFC;
                IFeatureSelection oriSelectFromMap = oriLayer as IFeatureSelection;
                oriSelectFromMap.Clear();

                ISpatialFilter spatialMapFilter = new SpatialFilterClass();
                spatialMapFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
                spatialMapFilter.Geometry = inFeature.Shape;
                spatialMapFilter.GeometryField = oriLayer.FeatureClass.ShapeFieldName;
                oriSelectFromMap.SelectFeatures(spatialMapFilter as IQueryFilter, esriSelectionResultEnum.esriSelectionResultNew, false);

                //Append
                GSC_ProjectEditor.GeoProcessing.AppendData(oriLayer, inFCDico[originalFCName]);


            }
        }

        /// <summary>
        /// Will export feature classes subtypes if any else complete table to output database path
        /// </summary>
        /// <param name="featureClassDictionary">A dictionary containing the features classes and their names</param>
        /// <param name="outputFolder">The folder path that will contain the new shapefiles</param>
        /// <param name="mapName">The map name for the new shapefiles.</param>
        public void ExportSubtypesToDBPath(Dictionary<string, IFeatureClass> featureClassDictionary, string outputPath)
        {
            #region FEATURE CLASSES TO SHP
            foreach (KeyValuePair<string, IFeatureClass> featureClassesKV in featureClassDictionary)
            {
                //Process if not exception, some feature classes shouldn't be parsed here.
                IDataset datasets = featureClassesKV.Value as IDataset;

                //Build a new feature layer with feature class
                IFeatureLayer outputFL = new FeatureLayerClass();
                outputFL.FeatureClass = featureClassesKV.Value;
                outputFL.Name = datasets.Name;

                //Get a list of subtypes if any
                Dictionary<string, int> subtypeDico = GSC_ProjectEditor.Subtypes.GetSubtypeDicoFromLayer(outputFL);

                if (subtypeDico.Count > 0)
                {
                    string subtypeField = GSC_ProjectEditor.Subtypes.GetSubtypeFieldFromFeatureLayer(outputFL);

                    //Iterate through subtypes for new selection and new feature layers
                    foreach (KeyValuePair<string, int> subtypeInfo in subtypeDico)
                    {
                        //New feature layer name
                        string subtypeLayerName = outputFL.Name + "_" + subtypeInfo.Key.ToUpper();

                        //New feature layer
                        IFeatureLayer subtypeLayer = GSC_ProjectEditor.FeatureLayers.CreateFeatureLayerFromSelection2(outputFL, featureClassesKV.Key, subtypeInfo.Value.ToString(), subtypeField, subtypeLayerName);
                        GSC_ProjectEditor.GeoProcessing.CopyFeatures(subtypeLayer, System.IO.Path.Combine(outputPath, subtypeLayerName));
                    }
                }
                else
                {
                    GSC_ProjectEditor.GeoProcessing.CopyFeatures(outputFL, System.IO.Path.Combine(outputPath, outputFL.Name));
                }


            }
            #endregion

        }

        /// <summary>
        /// Will add Legend table symbol field inside geopolys 
        /// </summary>
        /// <param name="legendTable"></param>
        /// <param name="geopolys"></param>
        public void AddSymbolField(IWorkspace copiedWorkspace, ITable legendTable, ITable geopolys)
        {
            //Get symbol field<
            IField legendSymbolField = legendTable.Fields.Field[legendTable.FindField(Constants.DatabaseFields.LegendSymbol)];

            if (legendSymbolField != null)
            {
                //Create the field object
                IField geopolySymbolField = new FieldClass();
                IFieldEdit2 geopolySymbolFieldEdit = (IFieldEdit2)geopolySymbolField;

                geopolySymbolFieldEdit.Name_2 = legendSymbolField.Name;
                geopolySymbolFieldEdit.Type_2 = legendSymbolField.Type;
                geopolySymbolFieldEdit.IsNullable_2 = legendSymbolField.IsNullable;
                geopolySymbolFieldEdit.AliasName_2 = legendSymbolField.AliasName;
                geopolySymbolFieldEdit.DefaultValue_2 = legendSymbolField.DefaultValue;
                geopolySymbolFieldEdit.Editable_2 = legendSymbolField.Editable;
                geopolySymbolFieldEdit.Length_2 = legendSymbolField.Length;

                //Add to feature
                geopolys.AddField(geopolySymbolField);

            }


        }

        #endregion

        public List<IFeatureDataset> sptialProjectedDBFD { get; set; }

        public List<IFeatureClass> spatialProjectedDBFC { get; set; }

        public List<IFeatureDataset> spatialProjectedDBFD { get; set; }
    }
}