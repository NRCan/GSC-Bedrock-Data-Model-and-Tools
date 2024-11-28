using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Desktop.AddIns;
using ESRI.ArcGIS.Editor;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Maplex;
using ESRI.ArcGIS.ArcMapUI;

namespace GSC_ProjectEditor
{

    public class Button_Environment_AddProjectLayers : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        
        #region Main Variables

        //Features class
        private const string geopntFC = GSC_ProjectEditor.Constants.Database.FGeopoint;
        private const string labelFC = GSC_ProjectEditor.Constants.Database.FLabel;
        private const string geopolyFC = GSC_ProjectEditor.Constants.Database.FGeopoly;
        private const string geolineFC = GSC_ProjectEditor.Constants.Database.FGeoline;
        private const string cgmFC = GSC_ProjectEditor.Constants.Database.FCGMIndex;
        private const string studyAreaFC = GSC_ProjectEditor.Constants.Database.FStudyArea;
        private const string ganfeldStationFC = GSC_ProjectEditor.Constants.Database.gFCStation;

        //Feature dataset
        private const string geoDS = GSC_ProjectEditor.Constants.Database.FDGeo;
        private const string fieldDS = GSC_ProjectEditor.Constants.Database.FDField;

        //Tables
        private const string gEarthmat = GSC_ProjectEditor.Constants.Database.gEarthMath;
        private const string gSample = GSC_ProjectEditor.Constants.Database.gSample;
        private const string gStruc = GSC_ProjectEditor.Constants.Database.gStruc;
        private const string gPhoto = GSC_ProjectEditor.Constants.Database.gPhoto;

        //Topology
        private const string topoFC = GSC_ProjectEditor.Constants.Database.Topology;

        //Fields
        private const string gStationX = GSC_ProjectEditor.Constants.DatabaseFields.FStationEasting;
        private const string gStationY = GSC_ProjectEditor.Constants.DatabaseFields.FStationNorthing;
        private const string gStationID = GSC_ProjectEditor.Constants.DatabaseFields.FStationID;
        private const string gEarthmatID = GSC_ProjectEditor.Constants.DatabaseGanfeldFields.earthmatID;
        private const string gSampleID = GSC_ProjectEditor.Constants.DatabaseGanfeldFields.sampleID;
        private const string gStrucID = GSC_ProjectEditor.Constants.DatabaseGanfeldFields.strucID;
        private const string gPhotoID = GSC_ProjectEditor.Constants.DatabaseGanfeldFields.photoID;
        private const string gStationXDeg = GSC_ProjectEditor.Constants.DatabaseFields.FStationLong;
        private const string gStationYDeg = GSC_ProjectEditor.Constants.DatabaseFields.FStationLat;

        private const string geolineTopo = GSC_ProjectEditor.Constants.Layers.geolineTopology;

        //File extension
        public string lyrExt = ".lyr";
        public string mxd = ".mxd";

        #endregion

        //TODO Find a way to change cursor for a waiting one, because this is not a normal VS control...

        public Button_Environment_AddProjectLayers()
        {
        }

        protected override void OnClick()
        {
            //Set culture before init.
            Utilities.Culture.SetCulture();

            //Get database path
            string databasePath = Dialog.GetFGDBPrompt(ArcMap.Application.hWnd, "Select database source to build layers from: ");

            if (databasePath != null && databasePath != string.Empty)
            {
                //Validate workspace
                List<string> datasetToValidate = new List<string>();
                datasetToValidate.Add(geopntFC);
                datasetToValidate.Add(labelFC);
                datasetToValidate.Add(geopolyFC);
                datasetToValidate.Add(geolineFC);
                datasetToValidate.Add(cgmFC);
                datasetToValidate.Add(studyAreaFC);
                datasetToValidate.Add(ganfeldStationFC);
                datasetToValidate.Add(gSample);
                datasetToValidate.Add(gStruc);
                datasetToValidate.Add(gPhoto);

                #region Start process

                IWorkspace currentWorkspace = GSC_ProjectEditor.Workspace.AccessWorkspace(databasePath);

                #endregion


                if (Workspace.isWorkspaceAProperProjectDatabase(currentWorkspace, datasetToValidate, true, false))
                {
                    List<string> projectTableList = GSC_ProjectEditor.Tables.GetTableNameListFromWorkspace(currentWorkspace);

                    try
                    {

                        #region Manage Field work group

                        //Get current map document
                        IMapDocument currentMapDocument = ArcMap.Document as IMapDocument;

                        //Get a feature class from the stations
                        IFeatureClass stationFC = GSC_ProjectEditor.FeatureClass.OpenFeatureClassFromWorkspace(currentWorkspace, ganfeldStationFC); //Will be used to create the XY events.

                        //Add proper field work group
                        IGroupLayer fieldGroup = AddGrouplayer(currentMapDocument, GSC_ProjectEditor.Properties.Resources.GroupLayerFieldWork, null);

                        //Add sub group inside fieldwork group ORDER => first line code is the highest layer within the code, last line is the one at the bottom.
                        List<string> fieldStrucGroup = GetFieldworkGroupStructure();
                        foreach (string subGroups in fieldStrucGroup)
                        {
                            IGroupLayer fieldSubGroup = AddGrouplayer(currentMapDocument, subGroups, fieldGroup.Name);

                            //Create an empty list to fill with feature layer for subgroup
                            List<Tuple<string, string, string>> fieldLayerList = new List<Tuple<string, string, string>>();

                            #region Add feature layer to this group

                            //Get list of feature layers to add to station if this is the current subgroup
                            if (fieldSubGroup.Name == GSC_ProjectEditor.Properties.Resources.GroupLayerStations)
                            {

                                //Variable
                                string stationAll = "STATIONS_ALL";

                                //Persist loaded layer file
                                GSC_ProjectEditor.FolderAndFiles.WriteResourceToFile(stationAll + lyrExt, "LayerFiles", GSC_ProjectEditor.Constants.NameSpaces.arcCatalog, System.IO.Path.GetTempPath());

                                //Access layer file
                                ILayer stationL = GSC_ProjectEditor.FeatureLayers.GetLyrFileFromComputer(System.IO.Path.Combine(System.IO.Path.GetTempPath(), stationAll) + lyrExt);

                                //Repair data source
                                IFeatureLayer stationFL = stationL as IFeatureLayer;
                                stationFL.FeatureClass = stationFC;

                                //Add layer
                                GSC_ProjectEditor.FeatureLayers.AddLayerToArcMap(stationFL as ILayer, fieldSubGroup.Name, new List<object> { currentMapDocument.Map[0] });

                            }

                            //Get list of feature layers to add to Earthmat if this is the current subgroup
                            if (fieldSubGroup.Name == GSC_ProjectEditor.Properties.Resources.GroupLayerEarthMaterials)
                            {
                                //Get list of earthmat layers
                                fieldLayerList = GetEarthmaEvents();

                            }

                            //Get list of feature layers to add to sample if this is the current subgroup
                            if (fieldSubGroup.Name == GSC_ProjectEditor.Properties.Resources.GroupLayerSamples)
                            {
                                //Get list of sample layers
                                fieldLayerList = GetSampleEvents();

                            }

                            //Get list of feature layers to add to struc if this is the current subgroup
                            if (fieldSubGroup.Name == GSC_ProjectEditor.Properties.Resources.GroupLayerStruc)
                            {
                                //Get list of struc layers
                                fieldLayerList = GetStrucEvents();
                            }

                            //Get list of feature layer to add to photo if this is the current subgroup
                            if (fieldSubGroup.Name == GSC_ProjectEditor.Properties.Resources.GroupLayerPhotos)
                            {
                                //Get list of photo layers
                                fieldLayerList = GetPhotoEvents();
                            }

                            #endregion

                            if (fieldLayerList.Count != 0)
                            {
                                ProcessEventFL(fieldSubGroup, fieldLayerList, currentMapDocument, stationFC, currentWorkspace);
                            }

                        }

                        #endregion

                        #region Manage Project group

                        //Add a project group layer
                        IGroupLayer projectGroup = AddGrouplayer(currentMapDocument, GSC_ProjectEditor.Properties.Resources.GroupLayerProject, null);

                        //Detect if current project is bedrock or surficial and create proper group type layer name
                        string groupeType = "";
                        if (!projectTableList.Contains(GSC_ProjectEditor.Constants.DatabaseGanfeld.gPflow))
                        {
                            groupeType = GSC_ProjectEditor.Properties.Resources.GroupLayerBedrock;

                        }
                        else
                        {
                            groupeType = GSC_ProjectEditor.Properties.Resources.GroupLayerSurficial;
                        }

                        //Add proper project type group
                        IGroupLayer typeGroup = AddGrouplayer(currentMapDocument, groupeType, projectGroup.Name);

                        //Add sub group inside type group ORDER => first line code is the highest layer within the code, last line is the one at the bottom.
                        List<string> projectStrucGroup = GetProjectGroupStructure();
                        foreach (string subGroups in projectStrucGroup)
                        {
                            //Add subgroup to mxd
                            IGroupLayer subGroup = AddGrouplayer(currentMapDocument, subGroups, typeGroup.Name);

                            //Create an empty list to fill with feature layer for subgroup
                            List<string> layerList = new List<string>();

                            #region Add feature layer to this group


                            //Get list of feature layers to add to interpretation if this is the current subgroup
                            if (subGroup.Name == GSC_ProjectEditor.Properties.Resources.GroupLayerInterpretation)
                            {
                                //Process interpretation layers
                                layerList = GetInterpretationFL();
                            }

                            //Get list of feature layers to add to validation group if this is the current group
                            if (subGroup.Name == GSC_ProjectEditor.Properties.Resources.GroupLayerValidation)
                            {
                                //Process interpretation layers
                                List<Tuple<string, string>> topoLayerList = GetValidationFL();

                                //Only use this for topology layers
                                ProcessTopoFL(subGroup, topoLayerList, currentMapDocument, currentWorkspace);
                            }

                            //Get list of feature layers to add to source group if this is the current group
                            if (subGroup.Name == GSC_ProjectEditor.Properties.Resources.GroupLayerSource)
                            {
                                //Process interpretation layers
                                layerList = GetSourceFL();
                            }

                            if (layerList.Count != 0)
                            {
                                ProcessFL(subGroup, layerList, currentMapDocument, currentWorkspace);
                            }


                            #endregion

                        }



                        #endregion

                        #region Update dataframe extent and info

                        IActiveView getActiveView = ArcMap.Document.ActiveView;
                        getActiveView.Refresh();

                        IContentsView tocView = ArcMap.Document.ContentsView[0]; //For table of content
                        tocView.Refresh(null);


                        //Get study area feature extent
                        IFeatureClass saFC = GSC_ProjectEditor.FeatureClass.OpenFeatureClassFromWorkspace(currentWorkspace,studyAreaFC);
                        IEnvelope studyAreaExtent = GSC_ProjectEditor.FeatureClass.GetFeatureClassEnvelope(saFC);

                        //Set extent to current active view
                        currentMapDocument.ActiveView.Extent = studyAreaExtent;

                        //New data frame name
                        currentMapDocument.Map[0].Name = GSC_ProjectEditor.Properties.Settings.Default.PROJECT_DATABASE_NAME;

                        currentMapDocument.Map[0].RecalcFullExtent();
                        currentMapDocument.ActiveView.ContentsChanged();
                        currentMapDocument.ActiveView.Refresh();
                        //It might be impossible to change the spatial ref on the fly if user triggers this on an existing mxd with 
                        //an already existing spatial reference.
                        try
                        {
                            currentMapDocument.ActiveView.FocusMap.SpatialReference = GSC_ProjectEditor.SpatialReferences.GetSpatialRef(saFC);
                        }
                        catch (Exception e)
                        {
                            Messages.ShowGenericErrorMessage(e.StackTrace);
                        }
                        

                        #endregion

                        MessageBox.Show(Properties.Resources.Message_ToolEnds, Properties.Resources.Message_CreateMXD, MessageBoxButtons.OK);
                    }
                    catch (Exception onClickException)
                    {
                        MessageBox.Show(onClickException.StackTrace + "; " + onClickException.Message);
                    }
                }
            }


        }

        protected override void OnUpdate()
        {
        }

        /// <summary>
        /// Will add all the requirement group layers inside the project mxd
        /// </summary>
        /// <param name="inMXD"></param>
        public IGroupLayer AddGrouplayer(IMapDocument inMXD, string groupLayerName, string aboveGroup)
        {
            //Get a group layer
            IGroupLayer newGroup = GSC_ProjectEditor.FeatureLayers.CreateEmptyGroupLayer(groupLayerName);

            //Add the project group
            GSC_ProjectEditor.FeatureLayers.AddLayerToArcMap(newGroup as ILayer, aboveGroup, new List<object> { inMXD.Map[0] });

            return newGroup;
        }

        /// <summary>
        /// Will provide a list of group layer names that will compose a project structure inside PROJECT group
        /// </summary>
        /// <returns></returns>
        public List<string> GetProjectGroupStructure()
        {
            //Variables
            List<string> projectGroupStructure = new List<string>();

            projectGroupStructure.Add(GSC_ProjectEditor.Properties.Resources.GroupLayerVisualization);
            projectGroupStructure.Add(GSC_ProjectEditor.Properties.Resources.GroupLayerValidation);
            projectGroupStructure.Add(GSC_ProjectEditor.Properties.Resources.GroupLayerInterpretation);
            projectGroupStructure.Add(GSC_ProjectEditor.Properties.Resources.GroupLayerSource);

            return projectGroupStructure;

        }

        /// <summary>
        /// Will provide a list of group layer names that will compose a field structure group
        /// </summary>
        /// <returns></returns>
        public List<string> GetFieldworkGroupStructure()
        {
            //Variables
            List<string> projectGroupStructure = new List<string>();

            projectGroupStructure.Add(GSC_ProjectEditor.Properties.Resources.GroupLayerEarthMaterials);
            projectGroupStructure.Add(GSC_ProjectEditor.Properties.Resources.GroupLayerSamples);
            projectGroupStructure.Add(GSC_ProjectEditor.Properties.Resources.GroupLayerStruc);
            projectGroupStructure.Add(GSC_ProjectEditor.Properties.Resources.GroupLayerPhotos);
            projectGroupStructure.Add(GSC_ProjectEditor.Properties.Resources.GroupLayerStations);

            return projectGroupStructure;
        }

        /// <summary>
        /// Will return a list containing a feature class name to add into the project group layer, sub group INTERPRETATION
        /// </summary>
        /// <returns></returns>
        public List<string> GetInterpretationFL()
        {
            //Variables
            List<string> projectFL = new List<string>();

            projectFL.Add(geopntFC);
            projectFL.Add(labelFC);
            projectFL.Add(geolineFC);
            projectFL.Add(geopolyFC);
            projectFL.Add(cgmFC);

            return projectFL;

        }

        /// <summary>
        /// Will return a list containing a feature class name to add into the project group layer, sub group SOURCE
        /// </summary>
        /// <returns></returns>
        public List<string> GetSourceFL()
        {
            //Variables
            List<string> sourceFL = new List<string>();

            sourceFL.Add(studyAreaFC);

            return sourceFL;

        }

        /// <summary>
        /// Will return a list containing a feature class name to add into the project group layer, sub group STATION
        /// </summary>
        /// <returns></returns>
        public List<string> GetStationsFL()
        {
            //Variables
            List<string> stationFL = new List<string>();

            stationFL.Add(ganfeldStationFC);

            return stationFL;

        }

        /// <summary>
        /// Will return a list of tuple with the table to convert into an XY event, the new layer name and the table primary key
        /// </summary>
        /// <returns></returns>
        public List<Tuple<string, string, string>> GetEarthmaEvents()
        {
            //Variables
            List<Tuple<string, string, string>> earthmatXYEvents = new List<Tuple<string, string, string>>();

            earthmatXYEvents.Add(new Tuple<string, string, string>(gEarthmat, GSC_ProjectEditor.Constants.Layers.earthmatDetail, gEarthmatID));
            earthmatXYEvents.Add(new Tuple<string, string, string>(gEarthmat, GSC_ProjectEditor.Constants.Layers.earthmatGroup, gEarthmatID));
            earthmatXYEvents.Add(new Tuple<string, string, string>(gEarthmat, GSC_ProjectEditor.Constants.Layers.earthmatType, gEarthmatID));

            return earthmatXYEvents;
        }

        /// <summary>
        /// Will return a list of tuple with the table to convert into an XY event, the new layer name and the table primary key
        /// </summary>
        /// <returns></returns>
        public List<Tuple<string, string, string>> GetSampleEvents()
        {
            //Variables
            List<Tuple<string, string, string>> sampleXYEvents = new List<Tuple<string, string, string>>();

            sampleXYEvents.Add(new Tuple<string, string, string>(gSample, GSC_ProjectEditor.Constants.Layers.sampleAll, gSampleID));
            sampleXYEvents.Add(new Tuple<string, string, string>(gSample, GSC_ProjectEditor.Constants.Layers.sampleGeochemistry, gSampleID));
            sampleXYEvents.Add(new Tuple<string, string, string>(gSample, GSC_ProjectEditor.Constants.Layers.sampleGeochrone, gSampleID));
            sampleXYEvents.Add(new Tuple<string, string, string>(gSample, GSC_ProjectEditor.Constants.Layers.sampleMineralogy, gSampleID));
            sampleXYEvents.Add(new Tuple<string, string, string>(gSample, GSC_ProjectEditor.Constants.Layers.samplePT, gSampleID));
            sampleXYEvents.Add(new Tuple<string, string, string>(gSample, GSC_ProjectEditor.Constants.Layers.sampleRepLitho, gSampleID));
            sampleXYEvents.Add(new Tuple<string, string, string>(gSample, GSC_ProjectEditor.Constants.Layers.sampleThinSection, gSampleID));
            sampleXYEvents.Add(new Tuple<string, string, string>(gSample, GSC_ProjectEditor.Constants.Layers.sampleOther, gSampleID));

            return sampleXYEvents;
        }

        /// <summary>
        /// Will return a list of tuple with the table to convert into an XY event, the new layer name and the table primary key
        /// </summary>
        /// <returns></returns>
        public List<Tuple<string, string, string>> GetStrucEvents()
        {
            //Variables
            List<Tuple<string, string, string>> strucXYEvents = new List<Tuple<string, string, string>>();

            strucXYEvents.Add(new Tuple<string, string, string>(gStruc, GSC_ProjectEditor.Constants.Layers.strucAll, gStrucID));
            strucXYEvents.Add(new Tuple<string, string, string>(gStruc, GSC_ProjectEditor.Constants.Layers.strucLinearAll, gStrucID));
            strucXYEvents.Add(new Tuple<string, string, string>(gStruc, GSC_ProjectEditor.Constants.Layers.strucPlanDip_0, gStrucID));
            strucXYEvents.Add(new Tuple<string, string, string>(gStruc, GSC_ProjectEditor.Constants.Layers.strucPlanDip_0_90, gStrucID));
            strucXYEvents.Add(new Tuple<string, string, string>(gStruc, GSC_ProjectEditor.Constants.Layers.strucPlanDip_90, gStrucID));

            return strucXYEvents;
        }

        /// <summary>
        /// Will return a list of tuple with the table to convert into an XY event, the new layer name and the table primary key
        /// </summary>
        /// <returns></returns>
        public List<Tuple<string, string, string>> GetPhotoEvents()
        {
            //Variables
            List<Tuple<string, string, string>> photoXYEvents = new List<Tuple<string, string, string>>();

            photoXYEvents.Add(new Tuple<string, string, string>(gPhoto, GSC_ProjectEditor.Constants.Layers.photoAll, gPhotoID));

            return photoXYEvents;
        }

        /// <summary>
        /// Will return a list containing topology object name and a topology layer name, contained inside a tuple.
        /// </summary>
        /// <returns></returns>
        public List<Tuple<string, string>> GetValidationFL()
        {
            //Variables
            List<Tuple<string, string>> valFL = new List<Tuple<string, string>>();

            valFL.Add(new Tuple<string, string>(topoFC, geolineTopo));

            return valFL;
        }

        /// <summary>
        /// From a given group layer, will append desire list of feature layers with alias coming from feature classes.
        /// </summary>
        /// <param name="inGL">The group layer to add layers into</param>
        /// <param name="currentMapDocument"> The mxd document in which to add layers</param>
        /// <param name="layersToAdd">The list of layers to add</param>
        public void ProcessFL(IGroupLayer inGL, List<string> layersToAdd, IMapDocument currentMapDocument, IWorkspace inWorkspace)
        {

            foreach (string featureLayers in layersToAdd)
            {
                //Variables
                bool somethingBroke = false;

                //Reset source path to be sure and assign feature alias to the new layer
                IFeatureLayer currentFL = new FeatureLayerClass();
                IFeatureClass currentFC = GSC_ProjectEditor.FeatureClass.OpenFeatureClassFromWorkspace(inWorkspace, featureLayers);
                currentFL.FeatureClass = currentFC;
                currentFL.Name = currentFC.AliasName; //The alias of the feature class will determine the feature layer name

                #region Layer customization for geoline and geopoly layers
                //Set renderer for geoline
                if (featureLayers == geolineFC)
                {
                    IUniqueValueRenderer geolineRenderer = GSC_ProjectEditor.Symbols.SetGeolineRendererProperties(null);
                    geolineRenderer.FieldCount = 1;//Only one field will determine symbol
                    geolineRenderer.set_Field(0, GSC_ProjectEditor.Constants.DatabaseFields.FGeolineSubtype); //Set the field to sym field 

                    //Add new renderer into the layer
                    IGeoFeatureLayer geolineGeoFeature = currentFL as IGeoFeatureLayer;
                    geolineGeoFeature.Renderer = geolineRenderer as IFeatureRenderer;
                    IFeatureRendererUpdate geolineFeatureRenderer = geolineRenderer as IFeatureRendererUpdate;
                    geolineFeatureRenderer.Update(currentFL);
                }

                //Set renderer for geopolys
                if (featureLayers == geopolyFC)
                {
                    IUniqueValueRenderer geopolyRenderer = GSC_ProjectEditor.Symbols.SetGeopolyRendererProperties(null);
                    geopolyRenderer.FieldCount = 1;//Only one field will determine symbol
                    geopolyRenderer.set_Field(0, GSC_ProjectEditor.Constants.DatabaseFields.FGeopolyLabel); //Set the field to sym field 

                    //Add new renderer into the layer
                    IGeoFeatureLayer geopolyGeoFeature = currentFL as IGeoFeatureLayer;
                    geopolyGeoFeature.Renderer = geopolyRenderer as IFeatureRenderer;
                    IFeatureRendererUpdate geopolyFeatureRenderer = geopolyRenderer as IFeatureRendererUpdate;
                    geopolyFeatureRenderer.Update(currentFL);
                }

                //Set renderer for geopoints
                if (featureLayers == geopntFC)
                {
                    IUniqueValueRenderer geopntRenderer = GSC_ProjectEditor.Symbols.SetGeopointRendererProperties(null);
                    if (geopntRenderer!=null)
                    {
                        geopntRenderer.FieldCount = 1;//Only one field will determine symbol
                        geopntRenderer.set_Field(0, GSC_ProjectEditor.Constants.DatabaseFields.FGeopointType); //Set the field to sym field 

                        //Add new renderer into the layer
                        IGeoFeatureLayer geopntGeoFL = currentFL as IGeoFeatureLayer;
                        geopntGeoFL.Renderer = geopntRenderer as IFeatureRenderer;
                        IFeatureRendererUpdate geopntFeatureRenderer = geopntRenderer as IFeatureRendererUpdate;
                        geopntFeatureRenderer.Update(currentFL); 
                    }
                    else
                    {
                        somethingBroke = true;
                    }

                }

                #endregion

                //Add layer
                if (!somethingBroke)
                {
                    GSC_ProjectEditor.FeatureLayers.AddLayerToArcMap(currentFL as ILayer, inGL.Name, new List<object> { currentMapDocument.Map[0] });
                }
                

            }
        }

        /// <summary>
        /// From a given group layer, will append desire list of topology layers
        /// </summary>
        /// <param name="inGL">The group layer to add layers into</param>
        /// <param name="currentMapDocument"> The mxd document in which to add layers</param>
        /// <param name="layersToAdd">The list of layers to add</param>
        public void ProcessTopoFL(IGroupLayer inGL, List<Tuple<string, string>> layersToAdd, IMapDocument currentMapDocument, IWorkspace inputWorkspace)
        {
            ///TOPOLOGY CAN'T USE FEATURE CLASS ALIAS, BECAUSE IT DOESN'T HAVE ANY

            foreach (Tuple<string, string> featureLayers in layersToAdd)
            {
                try
                {
                    //Find if topologay has been created
                    IWorkspace2 dbWorkspace2 = inputWorkspace as IWorkspace2;
                    bool topoExists = dbWorkspace2.get_NameExists(esriDatasetType.esriDTTopology, featureLayers.Item1);

                    //If the topology exists add it to arc map, else show a warning to user
                    if (topoExists)
                    {
                        //Get a proper feature layer from string
                        ITopologyLayer currentFL = new TopologyLayerClass();
                        ITopology currentTOPO = GSC_ProjectEditor.Topology.OpenTopoLayerLayerFromWorkspace(inputWorkspace, featureLayers.Item1);
                        currentFL.Topology = currentTOPO;
                        ILayer currentLayer = currentFL as ILayer;
                        currentLayer.Name = featureLayers.Item2;

                        //Add layer
                        GSC_ProjectEditor.FeatureLayers.AddLayerToArcMap(currentFL as ILayer, inGL.Name, new List<object> { currentMapDocument.Map[0] }); 
                    }
                    else
                    {
                        MessageBox.Show(Properties.Resources.Warning_MissingTopology, Properties.Resources.Warning_BasicTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }

                }
                catch (Exception processTopoFLException)
                {
                    //In case the topology doesn't exist
                    MessageBox.Show(processTopoFLException.Message + "; " + processTopoFLException.StackTrace);
                }

            }
        }

        /// <summary>
        /// Froma  given group layer, will create and append desire list of XY event layers
        /// </summary>
        /// <param name="inGL">The group layer to add new XY events to</param>
        /// <param name="layersToAdd">The list of new XY events, the input table and the output feature layer name</param>
        /// <param name="currentMapDocument">The mxd document in which the new layers will added</param>
        /// <param name="inputStations">The feature class in which the XY coordinates will be taken from</param>
        public void ProcessEventFL(IGroupLayer inGL, List<Tuple<string, string, string>> layersToAdd, IMapDocument currentMapDocument, IFeatureClass stationFC, IWorkspace inputWorkspace)
        {

            foreach (Tuple<string, string, string> featureLayers in layersToAdd)
            {
                //Prepare parameters
                Tuple<string, string, string> fieldTupleGeographic = new Tuple<string, string, string>(gStationXDeg, gStationYDeg, null);
                Tuple<string, string, string> fieldTupleProjected = new Tuple<string, string, string>(gStationX, gStationY, null);

                Tuple<string, string, string> joinFieldTuple = new Tuple<string, string, string>(gStationID, gStationID, featureLayers.Item3);
                
                //Get current table
                ITable currentTable = GSC_ProjectEditor.Tables.OpenTableFromWorkspace(inputWorkspace, featureLayers.Item1);

                //Get layer of current table
                try 
                {
                    //Get proper coordinate field, based on station FC coordinate system.
                    Tuple<string, string, string> fieldTuple = GetCoordinateTuple(stationFC, fieldTupleGeographic, fieldTupleProjected);

                    IXYEventSource currentEvent = GSC_ProjectEditor.FeatureLayers.CreateXYEvent(currentTable, fieldTuple, joinFieldTuple, stationFC, featureLayers.Item2);

                    //Persist loaded layer file
                    GSC_ProjectEditor.FolderAndFiles.WriteResourceToFile(featureLayers.Item2 + lyrExt, "LayerFiles", GSC_ProjectEditor.Constants.NameSpaces.arcCatalog, System.IO.Path.GetTempPath());

                    //Access layer file
                    ILayer currentL = GSC_ProjectEditor.FeatureLayers.GetLyrFileFromComputer(System.IO.Path.Combine(System.IO.Path.GetTempPath(), featureLayers.Item2) + lyrExt);

                    //Repair data source
                    IFeatureLayer currentFL = currentL as IFeatureLayer;
                    currentFL.FeatureClass = currentEvent as IFeatureClass;

                    //Add layer
                    GSC_ProjectEditor.FeatureLayers.AddLayerToArcMap(currentFL as ILayer, inGL.Name, new List<object> { currentMapDocument.Map[0] });

                }
                catch (Exception eventCreationError)
                {
                    //Show a warning message for possible dupblication station.
                    ///A duplicate station results in a join between station and tables with underscore instead of dots for field names F_STATION_STATIONDID instead of F_STATION.STATIONID
                    ///This problem makes the XY event crash because X field is wrong
                    ///
                    IDataset currentDataset = currentTable as IDataset;
                    string warningTitle = GSC_ProjectEditor.Properties.Resources.ErrorEventCreationTitle;
                    string warningMessage = GSC_ProjectEditor.Properties.Resources.ErrorEventCreation + ": " + currentDataset.BrowseName;
                    MessageBox.Show(warningMessage, warningTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                   
                }

            }
        }

        /// <summary>
        /// Will return a correct tuple of fields depending on entered feature class spatial reference, wheter it's projected or geographic
        /// </summary>
        /// <param name="inFC">The feature class to get the spatial reference from</param>
        /// <param name="geographicTuple">A geographic field tuple</param>
        /// <param name="projectedTuple">A projected field tuple</param>
        /// <returns></returns>
        public Tuple<string, string, string> GetCoordinateTuple(IFeatureClass inFC, Tuple<string, string, string> geographicTuple, Tuple<string, string, string> projectedTuple)
        {
            //Variables
            Tuple<string, string, string> outputTuple = new Tuple<string, string, string>("", "", ""); //Init

            //Get spatial reference
            ISpatialReference inSR = GSC_ProjectEditor.SpatialReferences.GetSpatialRef(inFC);

            //Parse spatial reference
            if (inSR is IGeographicCoordinateSystem)
            {
                outputTuple = geographicTuple;
            }
            else if (inSR is IProjectedCoordinateSystem)
            {
                outputTuple = projectedTuple;
            }

            return outputTuple;
        }

        public string newMXDPath(string currentFullPath, string currentFolderPath, string currentName, string currentExtension)
        {
            //Variable
            string newPath = currentFullPath;
            int counter = 1; //Will be used to iterate through existing mxds

            //Check if current path exist
            while (System.IO.File.Exists(newPath))
            {
                //Calculate a new alpha ID
                string newAlphaID = GSC_ProjectEditor.IDs.CalculateSimplementAlphabeticID(true, counter);

                //Build a new path
                newPath = System.IO.Path.Combine(currentFolderPath, currentName + "_" + newAlphaID) + currentExtension;

                counter = counter + 1;
            }

            return newPath;
        }

    }
}
