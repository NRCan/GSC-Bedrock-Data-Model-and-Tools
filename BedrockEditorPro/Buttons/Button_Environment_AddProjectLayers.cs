using ArcGIS.Core.CIM;
using ArcGIS.Core.Data;
using ArcGIS.Core.Data.Topology;
using ArcGIS.Core.Geometry;
using ArcGIS.Desktop.Catalog;
using ArcGIS.Desktop.Core;
using ArcGIS.Desktop.Editing;
using ArcGIS.Desktop.Extensions;
using ArcGIS.Desktop.Framework;
using ArcGIS.Desktop.Framework.Contracts;
using ArcGIS.Desktop.Framework.Dialogs;
using ArcGIS.Desktop.Framework.Threading.Tasks;
using ArcGIS.Desktop.KnowledgeGraph;
using ArcGIS.Desktop.Layouts;
using ArcGIS.Desktop.Mapping;
using BedrockEditorPro.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace BedrockEditorPro.Buttons
{
	internal class Button_Environment_AddProjectLayers : Button
	{
        private bool _version210Features = false; //Will be used to detect old schema features to add or not

        protected override async void OnClick()
        {

            bool? success = await AddProjectLayers();

            if (success.HasValue)
            {
                if (success.Value)
                {
                    //Show notification sucess
                    FrameworkApplication.AddNotification(new Notification()
                    {
                        Title = Properties.Resources.ButtonEnvironmentAddProjectLayersTitle,
                        Message = Properties.Resources.GenericMessageCompleted,
                        ImageSource = System.Windows.Application.Current.Resources["Success_Toast48"] as ImageSource
                    });
                }
                else
                {
                    MessageBox.Show(Properties.Resources.GenericMessageError, "", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                }
            }


        }

        /// <summary>
        /// Will add the project layers and needed hierarchy to the current map.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        private async Task<bool?> AddProjectLayers()
        {
            bool? isSuccess = null;

            try
            {
                //Get wanted geodatabase to create layers with
                string GeodatabasePath = Dialog.GetFGDBPrompt(Properties.Resources.ButtonEnvironmentAddProjectLayersPromptTitle);

                if (GeodatabasePath != null && GeodatabasePath != string.Empty)
                {
                    isSuccess = await QueuedTask.Run(async Task<bool> () =>
                    {
                        using (Geodatabase currentWorkspace = new Geodatabase(new FileGeodatabaseConnectionPath(new Uri(GeodatabasePath))))
                        {
                            if (currentWorkspace != null)
                            {
                                if (Workspace.FeatureClassExists(currentWorkspace, Constants.Database.FCGMIndex))
                                {
                                    _version210Features = true;
                                }

                                if (Workspace.FeatureDatasetExists(currentWorkspace, Constants.Database.FDGeo) &&
                                    Workspace.FeatureClassExists(currentWorkspace, Constants.Database.FGeopoly) &&
                                    Workspace.FeatureClassExists(currentWorkspace, Constants.Database.FLabel) &&
                                    Workspace.FeatureClassExists(currentWorkspace, Constants.Database.FGeoline) &&
                                    Workspace.FeatureClassExists(currentWorkspace, Constants.Database.FGeopoint) &&
                                    Workspace.FeatureClassExists(currentWorkspace, Constants.Database.FStudyArea))
                                {


                                    //Add project group
                                    GroupLayer projectGroup = LayerFactory.Instance.CreateGroupLayer(MapView.Active.Map, 0, Properties.Resources.GroupLayerProject);

                                    //Add project type (Bedrock vs surficial)
                                    GroupLayer projectTypeGroup = LayerFactory.Instance.CreateGroupLayer(projectGroup, 0, Properties.Resources.GroupLayerBedrock);

                                    //Add sub group inside project type group
                                    //Layer ordering => first line code is the highest layer within the code, last line is the one at the bottom.
                                    List<string> projectSubGroup = GetProjectSubGroupss();
                                    bool renderingFeatureLayerSucess = true;
                                    foreach (string subGroup in projectSubGroup)
                                    {
                                        //Add sub group layer
                                        GroupLayer subGroupLayer = LayerFactory.Instance.CreateGroupLayer(projectTypeGroup, projectSubGroup.IndexOf(subGroup), subGroup);

                                        #region Add feature layers to subgroups
                                        List<string> featureLayers = new List<string>();

                                        //Get list of feature layers to add to interpretation if this is the current subgroup
                                        if (subGroup == Properties.Resources.GroupLayerInterpretation)
                                        {
                                            //Process interpretation layers
                                            featureLayers = GetInterpretationFL();
                                        }

                                        //Get list of feature layers to add to validation group if this is the current group
                                        if (subGroup == Properties.Resources.GroupLayerValidation)
                                        {
                                            //Process interpretation layers
                                            featureLayers = GetValidationFL();
                                        }

                                        //Get list of feature layers to add to source group if this is the current group
                                        if (subGroup == Properties.Resources.GroupLayerSource)
                                        {
                                            //Process interpretation layers
                                            featureLayers = GetSourceFL();
                                        }

                                        renderingFeatureLayerSucess = await ProcessFL(subGroupLayer, featureLayers, currentWorkspace);
                                        if (!renderingFeatureLayerSucess)
                                        {
                                            return renderingFeatureLayerSucess;
                                        }
                                        

                                        #endregion

                                    }

                                    #region Update dataframe extent and info

                                    //Rename data frame and layout
                                    if (currentWorkspace.GetPath().IsFile)
                                    {
                                        //File geodatabase without extension as data frame name
                                        string filename = System.IO.Path.GetFileName(currentWorkspace.GetPath().LocalPath).Split(".")[0];
                                        MapView.Active.Map.SetName(filename);

                                    }

                                    //Zoom in 
                                    using (FeatureClass studyAreaFC = currentWorkspace.OpenDataset<FeatureClass>(Constants.Database.FStudyArea))
                                    {
                                        Envelope studyAreaExtent = studyAreaFC.GetExtent();
                                        if (studyAreaExtent != null)
                                        {
                                            MapView.Active.ZoomTo(studyAreaExtent);
                                        }
                                        
                                    }

                                    #endregion

                                    return renderingFeatureLayerSucess;
                                }
                                else
                                {
                                    new ErrorService($"{nameof(AddProjectLayers)}: Can't find one or many feature classes in selected database.").WriteToFile();
                                    return false;
                                }
                            }
                            else
                            {
                                new ErrorService($"{nameof(AddProjectLayers)}: Geodatabase is null.").WriteToFile();
                                return false;
                            }
                        }
                        ;
                    });
                }

            }
            catch (Exception e)
            {
                new ErrorService(e).WriteToFile();
            }

            return isSuccess;
        }

        /// <summary>
        /// Will provide a list of group layer names that will compose a project structure inside PROJECT group
        /// </summary>
        /// <returns></returns>
        private List<string> GetProjectSubGroupss()
        {
            //Variables
            List<string> projectGroupStructure = new List<string>();

            projectGroupStructure.Add(Properties.Resources.GroupLayerVisualization);
            projectGroupStructure.Add(Properties.Resources.GroupLayerValidation);
            projectGroupStructure.Add(Properties.Resources.GroupLayerInterpretation);
            projectGroupStructure.Add(Properties.Resources.GroupLayerSource);

            return projectGroupStructure;

        }

        /// <summary>
        /// Will return a list containing a feature class name to add into the project group layer, sub group INTERPRETATION
        /// </summary>
        /// <returns></returns>
        private List<string> GetInterpretationFL()
        {
            //Variables
            List<string> projectFL = new List<string>();

            projectFL.Add(Constants.Database.FGeopoint);
            projectFL.Add(Constants.Database.FLabel);
            projectFL.Add(Constants.Database.FGeoline);
            projectFL.Add(Constants.Database.FGeopoly);

            if (_version210Features)
            {
                projectFL.Add(Constants.Database.FCGMIndex);
            }
            

            return projectFL;

        }

        /// <summary>
        /// Will return a list containing a feature class name to add into the project group layer, sub group SOURCE
        /// </summary>
        /// <returns></returns>
        private List<string> GetSourceFL()
        {
            //Variables
            List<string> sourceFL = new List<string>();

            sourceFL.Add(Constants.Database.FStudyArea);

            return sourceFL;

        }

        /// <summary>
        /// Will return a list containing topology object name and a topology layer name, contained inside a tuple.
        /// </summary>
        /// <returns></returns>
        public List<string> GetValidationFL()
        {
            //Variables
            List<string> valFL = new List<string>();

            valFL.Add(Constants.Database.Topology);

            return valFL;
        }

        /// <summary>
        /// From a given group layer, will append desire list of feature layers with alias coming from feature classes.
        /// </summary>
        /// <param name="inGL">The group layer to add layers into</param>
        /// <param name="currentMapDocument"> The mxd document in which to add layers</param>
        /// <param name="layersToAdd">The list of layers to add</param>
        public async Task<bool> ProcessFL(GroupLayer groupLayer, List<string> layersToAdd, Geodatabase inWorkspace)
        {
            //Variable
            bool featureLayerRenderingSucess = true;

            foreach(string featureLayers in layersToAdd)
            {
                try
                {
                    #region Layer customization for geoline and geopoly layers

                    //Set renderer for geopoints
                    if (featureLayers == Constants.Database.FGeopoint)
                    {
                        using (FeatureClass geopointFC = inWorkspace.OpenDataset<FeatureClass>(Constants.Database.FGeopoint))
                        {
                            //Create new layer
                            Layer geopointLayer = LayerFactory.Instance.CreateLayer(geopointFC.GetPath(), groupLayer, 0, Properties.Resources.GroupLayerInterpretationGeopoint);
                            FeatureLayer geopointFeatureLayer = geopointLayer as FeatureLayer;

                            //Set default unique value renderer with a default symbology based on subtype
                            UniqueValueRendererDefinition geopntRenderer = new UniqueValueRendererDefinition()
                            {
                                ValueFields = new List<string>() { Constants.DatabaseFields.FGeopointType },
                                ColorRamp = ColorFactory.Instance.GetColorRamp("Default"),

                            };

                            //Create the feature layer renderer
                            CIMRenderer pntRenderer = geopointFeatureLayer.CreateRenderer(geopntRenderer);

                            // Rotation field setup
                            CIMExpressionInfo expressionInfo = new CIMExpressionInfo { Expression = $"$feature.{Constants.DatabaseFields.FGeopointAzimuth}" }; //Arcade
                            CIMVisualVariableInfo visualRenderer = new CIMVisualVariableInfo()
                            {
                                ValueExpressionInfo = expressionInfo,
                                VisualVariableInfoType = VisualVariableInfoType.Expression
                            };

                            List<CIMVisualVariable> visualVariables = new List<CIMVisualVariable>()
                        {
                            new CIMRotationVisualVariable()
                            {
                                VisualVariableInfoZ = visualRenderer,
                                RotationTypeZ =  SymbolRotationType.Geographic,
                            }
                        };
                            CIMUniqueValueRenderer uniqueValueRenderer = pntRenderer as CIMUniqueValueRenderer;
                            uniqueValueRenderer.VisualVariables = visualVariables.ToArray();

                            //Set the renderer to the geopoint feature layer
                            geopointFeatureLayer.SetRenderer(pntRenderer);

                        }
                    }

                    //Set renderer for labels
                    if (featureLayers == Constants.Database.FLabel)
                    {
                        using (FeatureClass labelFC = inWorkspace.OpenDataset<FeatureClass>(Constants.Database.FLabel))
                        {
                            //Create new layer
                            Layer labelLayer = LayerFactory.Instance.CreateLayer(labelFC.GetPath(), groupLayer, 1, Properties.Resources.GroupLayerInterpretationLabel);
                            FeatureLayer labelFeatureLayer = labelLayer as FeatureLayer;

                            //Set default unique value renderer with a default symbology based on subtype
                            CIMPointSymbol labelSymbol = Symbols.GetDefaultPointSymbol();

                            //Create the feature layer renderer
                            CIMSimpleRenderer labelRenderer = labelFeatureLayer.GetRenderer() as CIMSimpleRenderer;
                            labelRenderer.Symbol = labelSymbol.MakeSymbolReference();

                            //Set the renderer to the geopoint feature layer
                            labelFeatureLayer.SetRenderer(labelRenderer);

                        }
                    }

                    //Set renderer for geoline
                    if (featureLayers == Constants.Database.FGeoline)
                    {

                        using (FeatureClass geolineFC = inWorkspace.OpenDataset<FeatureClass>(Constants.Database.FGeoline))
                        {
                            //Create new layer
                            Layer geolineLayer = LayerFactory.Instance.CreateLayer(geolineFC.GetPath(), groupLayer, 2, Properties.Resources.GroupLayerInterpretationGeoline);
                            FeatureLayer geolineFeatureLayer = geolineLayer as FeatureLayer;

                            //Set default unique value renderer with a default symbology based on subtype
                            UniqueValueRendererDefinition geolineRenderer = new UniqueValueRendererDefinition()
                            {
                                ValueFields = new List<string>() { Constants.DatabaseFields.FGeolineSubtype },
                                ColorRamp = ColorFactory.Instance.GetColorRamp("Default"),

                            };

                            //Create the feature layer renderer
                            CIMRenderer lineRenderer = geolineFeatureLayer.CreateRenderer(geolineRenderer);

                            //Set the renderer to the geopoint feature layer
                            geolineFeatureLayer.SetRenderer(lineRenderer);

                        }
                    }

                    //Set renderer for geopolys
                    if (featureLayers == Constants.Database.FGeopoly)
                    {
                        using (FeatureClass geopolyFC = inWorkspace.OpenDataset<FeatureClass>(Constants.Database.FGeopoly))
                        {
                            //Create new layer
                            Layer geopolyLayer = LayerFactory.Instance.CreateLayer(geopolyFC.GetPath(), groupLayer, 3, Properties.Resources.GroupLayerInterpretationGeopoly);
                            FeatureLayer geopolyFeatureLayer = geopolyLayer as FeatureLayer;

                            //Set default simple renderer
                            CIMPolygonSymbol polySymbol = Symbols.GetDefaultPolygonSymbol();

                            //Create the feature layer renderer
                            CIMSimpleRenderer polyRenderer = geopolyFeatureLayer.GetRenderer() as CIMSimpleRenderer;
                            polyRenderer.Symbol = polySymbol.MakeSymbolReference();

                            //Set the renderer to the geopoly feature layer
                            geopolyFeatureLayer.SetRenderer(polyRenderer);

                        }
                    }

                    //Set renderer for map index
                    if (featureLayers == Constants.Database.FCGMIndex)
                    {
                        using (FeatureClass mapIndexFC = inWorkspace.OpenDataset<FeatureClass>(Constants.Database.FCGMIndex))
                        {
                            //Create new layer
                            Layer mapIndexLayer = LayerFactory.Instance.CreateLayer(mapIndexFC.GetPath(), groupLayer, 4, Properties.Resources.GroupLayerInterpretationCGM);
                            FeatureLayer mapIndexFeatureLayer = mapIndexLayer as FeatureLayer;

                            //Set default simple renderer
                            CIMColor indexColor = CIMColor.CreateRGBColor(217, 128, 38, 100);
                            CIMStroke indexOutline = SymbolFactory.Instance.ConstructStroke(
                                indexColor, 2.0, SimpleLineStyle.Solid);

                            CIMPolygonSymbol indexSymbol = SymbolFactory.Instance.ConstructPolygonSymbol(
                                ColorFactory.Instance.WhiteRGB, SimpleFillStyle.Null, indexOutline);

                            //Create the feature layer renderer
                            CIMSimpleRenderer polyRenderer = mapIndexFeatureLayer.GetRenderer() as CIMSimpleRenderer;
                            polyRenderer.Symbol = indexSymbol.MakeSymbolReference();

                            //Set the renderer to the map index feature layer
                            mapIndexFeatureLayer.SetRenderer(polyRenderer);

                        }
                    }

                    //Set renderer for map sources
                    if (featureLayers == Constants.Database.FStudyArea)
                    {
                        using (FeatureClass studyAreaFC = inWorkspace.OpenDataset<FeatureClass>(Constants.Database.FStudyArea))
                        {
                            //Create new layer
                            Layer mapSourceLayer = LayerFactory.Instance.CreateLayer(studyAreaFC.GetPath(), groupLayer, 0, Properties.Resources.GroupLayerSourceStudyArea);
                            FeatureLayer mapSourceFeatureLayer = mapSourceLayer as FeatureLayer;

                            //Set default simple renderer
                            CIMColor sourceColor = ColorFactory.Instance.BlueRGB;
                            CIMStroke sourceOutline = SymbolFactory.Instance.ConstructStroke(
                                sourceColor, 2.0, SimpleLineStyle.Solid);

                            CIMPolygonSymbol sourceSymbol = SymbolFactory.Instance.ConstructPolygonSymbol(
                                ColorFactory.Instance.WhiteRGB, SimpleFillStyle.Null, sourceOutline);

                            //Create the feature layer renderer
                            CIMSimpleRenderer sourceRenderer = mapSourceFeatureLayer.GetRenderer() as CIMSimpleRenderer;
                            sourceRenderer.Symbol = sourceSymbol.MakeSymbolReference();

                            //Set the renderer to the map index feature layer
                            mapSourceFeatureLayer.SetRenderer(sourceRenderer);

                        }
                    }

                    //Set renderer for validation
                    if (featureLayers == Constants.Database.Topology)
                    {

                        using (Topology topology = inWorkspace.OpenDataset<Topology>(Constants.Database.Topology))
                        {
                            ///No use on changing the default rendering on the topo layers

                            //Init a layer from creation params from topology object (a bit like a definition for a layer)
                            TopologyLayerCreationParams topologyLayerCreationParams = new TopologyLayerCreationParams(topology.GetDataConnection());
                            topologyLayerCreationParams.AddAssociatedLayers = false;
                            topologyLayerCreationParams.Name = Properties.Resources.GroupLayerValidationTopology;

                            //Create the layer with the right typed object
                            TopologyLayer topoLayer = LayerFactory.Instance.CreateLayer<TopologyLayer>(topologyLayerCreationParams, groupLayer);

                        }

                    }
                    #endregion
                }

                catch (Exception layerProcessingException) 
                {
                    new ErrorService(layerProcessingException).WriteToFile();

                    //Special case for topology, it is not mandatory so we can ignore this error
                    if (!layerProcessingException.Message.ToLower().Contains("topology"))
                    {
                        featureLayerRenderingSucess = false;
                    }
                }
            }

            return featureLayerRenderingSucess;
        }
    }
}
