using ArcGIS.Core.CIM;
using ArcGIS.Core.Data;
using ArcGIS.Desktop.Core;
using ArcGIS.Desktop.Framework;
using ArcGIS.Desktop.Framework.Contracts;
using ArcGIS.Desktop.Framework.Dialogs;
using ArcGIS.Desktop.Framework.Threading.Tasks;
using ArcGIS.Desktop.Internal.Core;
using ArcGIS.Desktop.Mapping;
using BedrockEditorPro.Models;
using BedrockEditorPro.Services;
using BedrockEditorPro.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using static BedrockEditorPro.ProWindows.Form_Load_StudyAreaViewModel;
using MessageBox = ArcGIS.Desktop.Framework.Dialogs.MessageBox;

namespace BedrockEditorPro.ProWindows
{
    public class Form_RefreshSymbolsViewModel: Layers
    {
        #region INIT

        private WorkingEnvironment workingEnvironment = new WorkingEnvironment();
        private Form_RefreshSymbols _view = null;

        private object _lock = new(); //For locking the threads to update obs. collection

        #endregion

        #region PROPERTIES

        //Layer controls
        private ObservableCollection<LayerDisplay> _refreshLayers = new();
        public ObservableCollection<LayerDisplay> RefreshLayers
        {
            get { return _refreshLayers; }
        }
        private int _refreshSelectedLayerIndex = -1;
        public int RefreshSelectedLayerIndex
        {
            get { return _refreshSelectedLayerIndex; }
            set
            {
                SetProperty(ref _refreshSelectedLayerIndex, value, () => _refreshSelectedLayerIndex);
            }
        }

        private Visibility _waitingCursorVisibility = Visibility.Collapsed;
        public Visibility WaitingCursorVisibility
        {
            get { return _waitingCursorVisibility; }
            set
            {
                SetProperty(ref _waitingCursorVisibility, value, () => _waitingCursorVisibility);
            }
        }

        #endregion

        #region RELAYS

        private ICommand _runTool = null;
        public ICommand RunTool
        {
            get
            {
                if (_runTool == null)
                {
                    _runTool = new RelayCommand(() => RefreshLayersSymbols(), () => true);
                }
                return _runTool;
            }
        }

        #endregion

        #region METHODS

        public Form_RefreshSymbolsViewModel(Form_RefreshSymbols view)
        {
            //Init as obs. collection the comboboxes
            BindingOperations.EnableCollectionSynchronization(_refreshLayers, _lock);

            //Set related view
            _view = view;

            //Init some components
            UpdateLayerCombobox();

        }

        /// <summary>
        /// Will fill the layer combobox with all feature layers in the map
        /// Optionall i will pre-select the study area layer if it exists
        /// </summary>
        public async void UpdateLayerCombobox()
        {
            //Init some components
            _refreshLayers.Clear();
            List<esriGeometryType> geomTypes = new List<esriGeometryType>() { esriGeometryType.esriGeometryPolygon, esriGeometryType.esriGeometryPolyline, 
                esriGeometryType.esriGeometryLine, esriGeometryType.esriGeometryPoint };
            Layers layerService = new Layers();
            await layerService.UpdateLayerCombobox(geomTypes, _refreshLayers, nameof(RefreshLayers), _refreshSelectedLayerIndex, nameof(RefreshSelectedLayerIndex));

        }

        /// <summary>
        /// Will iterate through all checked layers and try to refresh their symbols based on the 
        /// custom/default style file used by the tools
        /// </summary>
        public async void RefreshLayersSymbols()
        {
            try
            {

                if (_refreshLayers.Count() > 0 && _refreshLayers.Where(x=>x.IsChecked == true).Count() > 0)
                {
                    WaitingCursorVisibility = Visibility.Visible;

                    //Initiate reading of style file
                    UserConfiguration userConfig = await UserConfigurationService.GetUserConfigurationAsync();

                    await QueuedTask.Run(() =>
                    {
                        if (userConfig != null)
                        {
                            //Get styling
                            StyleProjectItem workingStyle = Symbols.GetStyleItemProject(userConfig.StyleFilePath);

                            //Iterate through all selected layers
                            List<LayerDisplay> layersToRefresh = _refreshLayers.Where(x => x.IsChecked == true).ToList();
                            foreach (LayerDisplay l in layersToRefresh)
                            {
                                RefreshLayerSymbols(l.FLayer, workingStyle);
                                RefreshLayerTemplates(l.FLayer);
                            }
                        }
                        else
                        {
                            throw new Exception("User configuration is null");
                        }
                    });
                }

                //Close window
                WaitingCursorVisibility = Visibility.Collapsed;
                _view.Close();

                //Show notication success
                FrameworkApplication.AddNotification(new Notification()
                {
                    Title = Properties.Resources.FormRefreshSymbolesTitle,
                    Message = Properties.Resources.GenericMessageCompleted,
                    ImageSource = System.Windows.Application.Current.Resources["Success_Toast48"] as ImageSource
                });
            }
            catch (Exception e)
            {
                new ErrorService(e).WriteToFile();
                WaitingCursorVisibility = Visibility.Collapsed;
                _view.Close();
            }
        }

        /// <summary>
        /// Will add some fields to a unique value renderer, 
        /// especially meant for geoline features
        /// </summary>
        /// <param name="fieldDescriptions"></param>
        public UniqueValueRendererDefinition PrepareGeolineLabelRenderer(List<FieldDescription> fieldDescriptions, UniqueValueRendererDefinition uniqueValueRenderer)
        {
            List<FieldDescription> geolineSubDescription = fieldDescriptions.Where(x => x.Name == Constants.DatabaseFields.FGeolineSubtype).ToList();
            if (geolineSubDescription != null && geolineSubDescription.Count() > 0)
            {
                uniqueValueRenderer.ValueFields.Add(geolineSubDescription[0].Name);
            }

            List<FieldDescription> geolineQualDescription = fieldDescriptions.Where(x => x.Name == Constants.DatabaseFields.FGeolineQualif).ToList();
            if (geolineQualDescription != null && geolineQualDescription.Count() > 0)
            {
                uniqueValueRenderer.ValueFields.Add(geolineQualDescription[0].Name);
            }

            List<FieldDescription> geolineConfDescription = fieldDescriptions.Where(x => x.Name == Constants.DatabaseFields.FGeolineConf).ToList();
            if (geolineConfDescription != null && geolineConfDescription.Count() > 0)
            {
                uniqueValueRenderer.ValueFields.Add(geolineConfDescription[0].Name);
            }

            List<FieldDescription> geolineAttDescription = fieldDescriptions.Where(x => x.Name == Constants.DatabaseFields.FGeolineAtt).ToList();
            if (geolineAttDescription != null && geolineAttDescription.Count() > 0)
            {
                uniqueValueRenderer.ValueFields.Add(geolineAttDescription[0].Name);
            }

            List<FieldDescription> geolineGeneDescription = fieldDescriptions.Where(x => x.Name == Constants.DatabaseFields.FGeolineGeneration).ToList();
            if (geolineGeneDescription != null && geolineGeneDescription.Count() > 0)
            {
                uniqueValueRenderer.ValueFields.Add(geolineGeneDescription[0].Name);
            }

            return uniqueValueRenderer;
        }

        /// <summary>
        /// Will return a matchin value dictionary that holds bedrock map unit labels and their associated symbol
        /// This is needed since label feature class doesn't have a symbol field, as opposed to geoline, geopoint and geopoly.
        /// </summary>
        /// <param name="inFeatureLayer"></param>
        public Dictionary<string, string> PrepareLabelColorRenderer(FeatureLayer inFeatureLayer)
        {
            Dictionary<string, string> symbolDico = new Dictionary<string, string>();

            CIMBasicFeatureLayer lFeatureDef = inFeatureLayer.GetDefinition() as CIMBasicFeatureLayer;
            CIMFeatureTable lFeatureTable = lFeatureDef.FeatureTable;

            //Make sure it's only label feature class being processed
            if (lFeatureDef.Description.ToLower() == Constants.Database.FLabel.ToLower())
            {

                //Iterate through values and find their match in the style
                CIMDataConnection dataConnection = lFeatureTable.DataConnection;
                CIMFeatureDatasetDataConnection fdDataConnection = dataConnection as CIMFeatureDatasetDataConnection;
                Uri uri = new Uri(fdDataConnection.WorkspaceConnectionString.Replace("DATABASE=", ""));
                using (Geodatabase sourceGeodatabase = new Geodatabase(new FileGeodatabaseConnectionPath(uri)))
                {
                    try
                    {
                        using (Table purposeTable = sourceGeodatabase.OpenDataset<Table>(Constants.Database.TLegendGene))
                        {
                            //Model version managing
                            List<Field> legendFields = purposeTable.GetDefinition().GetFields().ToList();
                            bool is210Model = false;
                            PLegend pLegend = new PLegend();

                            if (legendFields.Exists(f => f.Name == Constants.DatabaseFields.LegendSymbol))
                            {
                                is210Model = false;
                            }
                            else if (legendFields.Exists(f => f.Name == Constants.DatabaseFields.LegendSymbol_190101))
                            {
                                is210Model = true;
                            }

                            QueryFilter queryFilter = new QueryFilter
                            {
                                SubFields = string.Format("{0}, {1}", Constants.DatabaseFields.LegendLabelID, Constants.DatabaseFields.LegendSymbol),
                                PrefixClause = "DISTINCT",
                                WhereClause = string.Format("{0} IS NOT NULL AND {1} IS NOT NULL AND {2} in ({3})",
                                Constants.DatabaseFields.LegendSymbol, Constants.DatabaseFields.LegendLabelID,
                                Constants.DatabaseFields.LegendItemType, string.Format("'{0}'", string.Join("','", pLegend.UnitElements)))
                            };

                            if (is210Model)
                            {
                                queryFilter = new QueryFilter
                                {
                                    SubFields = string.Format("{0}, {1}", Constants.DatabaseFields.LegendLabelID, Constants.DatabaseFields.LegendSymbol_190101),
                                    PrefixClause = "DISTINCT",
                                    WhereClause = string.Format("{0} IS NOT NULL AND {1} IS NOT NULL AND {2} = '{3}'",
                                                                Constants.DatabaseFields.LegendSymbol_190101, Constants.DatabaseFields.LegendLabelID,
                                                                Constants.DatabaseFields.LegendItemType_190101, Constants.DatabaseDomainsValues.legendItemMapUnit)
                                };
                            }

                            using (RowCursor rc = purposeTable.Search(queryFilter, false))
                            {
                                while (rc.MoveNext())
                                {
                                    using (Row row = rc.Current)
                                    {
                                        //Model 4.0
                                        if (row.FindField(Constants.DatabaseFields.LegendSymbol) > 0)
                                        {
                                            symbolDico[row[Constants.DatabaseFields.LegendLabelID].ToString()] = row[Constants.DatabaseFields.LegendSymbol].ToString();
                                        }

                                        //Model 2.10 TODO remove when first release
                                        if (row.FindField(Constants.DatabaseFields.LegendSymbol_190101) > 0)
                                        {
                                            symbolDico[row[Constants.DatabaseFields.LegendLabelID].ToString()] = row[Constants.DatabaseFields.LegendSymbol_190101].ToString();
                                        }

                                    }
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        new ErrorService(e).WriteToFile();
                    }

                }

            }

            return symbolDico;
        }

        /// <summary>
        /// Will add some fields to a unique value renderer, 
        /// especially meant for geoline features
        /// </summary>
        /// <param name="fieldDescriptions"></param>
        public UniqueValueRendererDefinition PrepareGeopointLabelRenderer(List<FieldDescription> fieldDescriptions, UniqueValueRendererDefinition uniqueValueRenderer)
        {

            List<FieldDescription> geopointTypeDescription = fieldDescriptions.Where(x => x.Name == Constants.DatabaseFields.FGeopointType).ToList();
            if (geopointTypeDescription != null && geopointTypeDescription.Count() > 0)
            {
                uniqueValueRenderer.ValueFields.Add(geopointTypeDescription[0].Name);
            }

            List<FieldDescription> geopointSubDescription = fieldDescriptions.Where(x => x.Name == Constants.DatabaseFields.FGeopointSubset).ToList();
            if (geopointSubDescription != null && geopointSubDescription.Count() > 0)
            {
                uniqueValueRenderer.ValueFields.Add(geopointSubDescription[0].Name);
            }

            List<FieldDescription> geopointAttDescription = fieldDescriptions.Where(x => x.Name == Constants.DatabaseFields.FGeopointStrucAtt).ToList();
            if (geopointAttDescription != null && geopointAttDescription.Count() > 0)
            {
                uniqueValueRenderer.ValueFields.Add(geopointAttDescription[0].Name);
            }

            List<FieldDescription> geopointGeneDescription = fieldDescriptions.Where(x => x.Name == Constants.DatabaseFields.FGeopointStrucGene).ToList();
            if (geopointGeneDescription != null && geopointGeneDescription.Count() > 0)
            {
                uniqueValueRenderer.ValueFields.Add(geopointGeneDescription[0].Name);
            }

            List<FieldDescription> geopointYoungDescription = fieldDescriptions.Where(x => x.Name == Constants.DatabaseFields.FGeopointStrucYoung).ToList();
            if (geopointYoungDescription != null && geopointYoungDescription.Count() > 0)
            {
                uniqueValueRenderer.ValueFields.Add(geopointYoungDescription[0].Name);
            }

            List<FieldDescription> geopointMethodDescription = fieldDescriptions.Where(x => x.Name == Constants.DatabaseFields.FGeopointStrucMethod).ToList();
            if (geopointMethodDescription != null && geopointMethodDescription.Count() > 0)
            {
                uniqueValueRenderer.ValueFields.Add(geopointMethodDescription[0].Name);
            }

            return uniqueValueRenderer;
        }

        /// <summary>
        /// Will refresh the incoming layer symbols base on the custom/default style file used by the tools
        /// </summary>
        /// <param name="inLayer"></param>
        public void RefreshLayerSymbols(FeatureLayer inLayer, StyleProjectItem workingStyle)
        {
            //Make sure the layer has the proper symbol and/or label fields
            List<FieldDescription> flDescriptions = inLayer.GetFieldDescriptions().ToList();
            if (flDescriptions != null && flDescriptions.Count() > 0)
            {
                //Prepare unique value renderer                
                UniqueValueRendererDefinition uniqueValueRenderer = new UniqueValueRendererDefinition()
                {
                    ColorRamp = ColorFactory.Instance.GetColorRamp("Default"),
                    ValueFields = new List<string>()
                    {
                        //Constants.DatabaseFields.LegendSymbol
                    },
                };

                //Model 4.0 - Style1 is needed for proper styling, else default color ramp will be used
                bool symbolFieldDescription = flDescriptions.Exists(x => x.Name == Constants.DatabaseFields.LegendSymbol);
                if (symbolFieldDescription)
                {
                    uniqueValueRenderer.ValueFields.Add(Constants.DatabaseFields.LegendSymbol);
                }

                //Model 2.10 - GSC_SYMBOL is needed for proper styling, else default color ramp will be used
                bool symbolFieldDescription2 = flDescriptions.Exists(x => x.Name == Constants.DatabaseFields.LegendSymbol_190101);
                if (symbolFieldDescription2)
                {
                    uniqueValueRenderer.ValueFields.Add(Constants.DatabaseFields.LegendSymbol_190101);
                }

                //Add label field to unique renderer, if any
                List<FieldDescription> labelFieldDescription = flDescriptions.Where(x => x.Alias == Constants.DatabaseFields.FLabelIDAlias).ToList();
                if (labelFieldDescription != null && labelFieldDescription.Count() > 0)
                {
                    uniqueValueRenderer.ValueFields.Add(labelFieldDescription[0].Name);
                }

                //Prepare label field for geolines
                uniqueValueRenderer = PrepareGeolineLabelRenderer(flDescriptions, uniqueValueRenderer);

                //Prepare label field for geopoints
                uniqueValueRenderer = PrepareGeopointLabelRenderer(flDescriptions, uniqueValueRenderer);

                //Build a list of symbols for labels only (missing symbol field)
                Dictionary<string, string> labelSymbols = PrepareLabelColorRenderer(inLayer);

                //Create a default unique renderer, in case styling with the file doesn't work
                CIMRenderer renderer = inLayer.CreateRenderer(uniqueValueRenderer);

                //Sets the renderer to the feature layer
                inLayer.SetRenderer(renderer);

                //Get geometry type in order to be able to search style file properly
                StyleItemType styleItemType = StyleItemType.Unknown;
                if (inLayer.ShapeType == esriGeometryType.esriGeometryPolygon)
                {
                    styleItemType = StyleItemType.PolygonSymbol;
                }
                else if (inLayer.ShapeType == esriGeometryType.esriGeometryPoint)
                {
                    styleItemType = StyleItemType.PointSymbol;
                }
                else if (inLayer.ShapeType == esriGeometryType.esriGeometryLine || inLayer.ShapeType == esriGeometryType.esriGeometryPolyline)
                {
                    styleItemType = StyleItemType.LineSymbol;
                }

                //Get back the renderer and make a copy
                if (inLayer.GetRenderer() is CIMUniqueValueRenderer cIMUniqueValueRenderer)
                {
                    //Keep list of possible missing symbols
                    List<string> missingSymbols = new List<string>();

                    CIMUniqueValueRenderer cloneRenderer = cIMUniqueValueRenderer.Clone();
                    //Go through all groups (headings)
                    foreach (CIMUniqueValueGroup cimVG in cloneRenderer.Groups)
                    {
                        //Go through all classes (symbols)
                        if (cimVG.Classes != null)
                        {
                            foreach (CIMUniqueValueClass cimVC in cimVG.Classes)
                            {


                                //Go through all field values
                                foreach (CIMUniqueValue cimV in cimVC.Values)
                                {
                                    if (labelSymbols.Count() == 0)
                                    {
                                        //Find symbol in style file from first field value
                                        IList<SymbolStyleItem> symbols = workingStyle.SearchSymbols(styleItemType, cimV.FieldValues[0].ToString());
                                        if (symbols != null && symbols.Count() > 0)
                                        {
                                            SymbolStyleItem currentSymbol = symbols[0];

                                            //Set
                                            CIMSymbolReference cimSR = cimVC.Symbol;
                                            cimSR.Symbol = currentSymbol.Symbol;
                                        }
                                        else
                                        {
                                            missingSymbols.Add(cimV.FieldValues[0].ToString());
                                        }

                                    }
                                    else
                                    {

                                        if (labelSymbols.ContainsKey(cimV.FieldValues[0]))
                                        {
                                            //Get symbol code
                                            string symbolCode = labelSymbols[cimV.FieldValues[0]].ToString();

                                            //Find symbol in style file from first field value
                                            SymbolStyleItem currentSymbol = workingStyle.SearchSymbols(StyleItemType.PolygonSymbol, symbolCode)[0];

                                            CIMPointSymbol currentPntSymbol = Symbols.GetLabelDefaultRenderer(currentSymbol.Symbol.GetColor());

                                            //Set
                                            CIMSymbolReference cimSR = cimVC.Symbol;
                                            cimSR.Symbol = currentPntSymbol;
                                        }
                                    }
                                }

                            }
                        }

                    }

                    //Update layer with new renderer
                    inLayer.SetRenderer(cloneRenderer);

                    //Show warning if missing symbols were found for some reasons.
                    if (missingSymbols.Count() > 0)
                    {
                        MessageBox.Show(String.Format(Properties.Resources.FormRefreshSymbolsMissingCode, string.Join(", ", missingSymbols), inLayer.Name), Properties.Resources.GenericWarningTitle, System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);

                    }
                }
            }
        }

        /// <summary>
        /// Will sync the editing templates with the legend in case symbols were added but never digitized
        /// </summary>
        /// <param name="inLayer"></param>
        public void RefreshLayerTemplates(FeatureLayer inLayer)
        {
            Uri layerURI = Workspace.GetWorkspacePath(inLayer);
            if (layerURI != null)
            {
                using (Geodatabase layerGeodatabase = new Geodatabase(new FileGeodatabaseConnectionPath(layerURI)))
                {
                    using (Table legendTable = layerGeodatabase.OpenDataset<Table>(Constants.Database.TLegendGene))
                    {
                        List<Field> legendFields = legendTable.GetDefinition().GetFields().ToList();
                        bool is210Model = false;
                        PLegend pLegend = new PLegend();
                        if (legendFields.Exists(f=>f.Name == Constants.DatabaseFields.LegendSymbol))
                        {
                            is210Model = false;
                        }
                        else if (legendFields.Exists(f => f.Name == Constants.DatabaseFields.LegendSymbol_190101))
                        {
                            is210Model = true;
                        }

                        //Get list of all templates associated with feature layer
                        CIMFeatureLayer layerDefinition = inLayer.GetDefinition() as CIMFeatureLayer;
                        List<CIMEditingTemplate> templates = layerDefinition.FeatureTemplates?.ToList();

                        if (templates == null)
                        {
                            templates = new List<CIMEditingTemplate>();
                        }

                        if (inLayer.ShapeType == esriGeometryType.esriGeometryLine || inLayer.ShapeType == esriGeometryType.esriGeometryPolyline)
                        {
                            #region Geolines
                            QueryFilter legendFilter = new QueryFilter
                            {
                                SubFields = string.Format("{0}, {1}, {2}", Constants.DatabaseFields.LegendSymbol, Constants.DatabaseFields.LegendLabelID,
                                Constants.DatabaseFields.LegendGISDisplay),
                                WhereClause = string.Format("{0} IS NOT NULL AND {1} IN ({2})",
                                Constants.DatabaseFields.LegendSymbol, Constants.DatabaseFields.LegendItemType, string.Format("'{0}'", string.Join("','", pLegend.LineElements)))
                            };

                            if (is210Model)
                            {
                                legendFilter = new QueryFilter
                                {
                                    SubFields = string.Format("{0}, {1}, {2}", Constants.DatabaseFields.LegendSymbol_190101, Constants.DatabaseFields.LegendLabelID,
                                                                Constants.DatabaseFields.LegendGISDisplay),
                                    WhereClause = string.Format("{0} IS NOT NULL AND {1} = '{2}'",
                                                                Constants.DatabaseFields.LegendSymbol_190101, Constants.DatabaseFields.LegendItemType_190101,
                                                                Constants.DatabaseDomainsValues.legendItemGeoline)
                                };
                            }

                            List<GeoLines> legendGeolines = new List<GeoLines>();

                            using (RowCursor lineCursor = legendTable.Search(legendFilter))
                            {
                                while (lineCursor.MoveNext())
                                {
                                    using (Row lineRow = lineCursor.Current)
                                    {
                                        GeoLines newGeolines = new GeoLines
                                        {
                                            GeolineID = lineRow[Constants.DatabaseFields.LegendLabelID].ToString(),
                                            Name = lineRow[Constants.DatabaseFields.LegendGISDisplay].ToString(),
                                            CreatorID = Properties.Settings.Default.SelectedParticipantCode
                                        };
                                        newGeolines.GeolineType = int.Parse(newGeolines.GetGeolineSubtypeFromID);
                                        newGeolines.Qualifier = newGeolines.GetGeolineQualifierFromID;
                                        newGeolines.Attitude = newGeolines.GetGeolineAttitudeFromID;
                                        newGeolines.Confidence = newGeolines.GetGeolineConfidenceFromID;
                                        newGeolines.Generation = newGeolines.GetGeolineGenerationFromID;

                                        if (is210Model)
                                        {
                                            newGeolines.GSCSymbol = lineRow[Constants.DatabaseFields.LegendSymbol_190101].ToString();
                                        }
                                        else
                                        {
                                            newGeolines.GSCSymbol = lineRow[Constants.DatabaseFields.LegendSymbol].ToString();
                                        }

                                        legendGeolines.Add(newGeolines);

                                    }
                                }
                            }

                            if (legendGeolines.Count() > 0)
                            {

                                //Iterate through all legend geolines and see if a template exists, else add it
                                foreach (GeoLines gl in legendGeolines)
                                {
                                    if (!templates.Exists(x => x.Description == gl.GeolineID))
                                    {
                                        Symbols.CreateLineTemplate(inLayer,gl);
                                    }
                                }
                            }

                            #endregion
                        }
                        else if (inLayer.ShapeType == esriGeometryType.esriGeometryMultipoint || inLayer.ShapeType == esriGeometryType.esriGeometryPoint)
                        {
                            FeatureClass layerClass = inLayer.GetFeatureClass();

                            if (layerClass.GetName().Contains(Constants.Database.FGeopoint))
                            {
                                #region Geopoints
                                QueryFilter legendFilter = new QueryFilter
                                {
                                    SubFields = string.Format("{0}, {1}, {2}", Constants.DatabaseFields.LegendSymbol, Constants.DatabaseFields.LegendLabelID,
                                        Constants.DatabaseFields.LegendGISDisplay),
                                    WhereClause = string.Format("{0} IS NOT NULL AND {1} IN ({2})",
                                        Constants.DatabaseFields.LegendSymbol, Constants.DatabaseFields.LegendItemType, string.Format("'{0}'", string.Join("','", pLegend.MarkerElements)))
                                };

                                if (is210Model)
                                {
                                    legendFilter = new QueryFilter
                                    {
                                        SubFields = string.Format("{0}, {1}, {2}", Constants.DatabaseFields.LegendSymbol_190101, Constants.DatabaseFields.LegendLabelID,
                                                                    Constants.DatabaseFields.LegendGISDisplay),
                                        WhereClause = string.Format("{0} IS NOT NULL AND {1} = '{2}'",
                                                                    Constants.DatabaseFields.LegendSymbol_190101, Constants.DatabaseFields.LegendItemType_190101,
                                                                    Constants.DatabaseDomainsValues.legendItemGeopoint)
                                    };
                                }


                                List<GeoPoints> legendGeopoints = new List<GeoPoints>();

                                using (RowCursor pointCursor = legendTable.Search(legendFilter))
                                {
                                    while (pointCursor.MoveNext())
                                    {
                                        using (Row pointRow = pointCursor.Current)
                                        {

                                            GeoPoints newGeopoint = new GeoPoints
                                            {
                                                GeopointID = pointRow[Constants.DatabaseFields.LegendLabelID].ToString(),
                                                Name = pointRow[Constants.DatabaseFields.LegendGISDisplay].ToString(),
                                                CreatorID = Properties.Settings.Default.SelectedParticipantCode
                                            };
                                            newGeopoint.GeopointType = int.Parse(newGeopoint.GetGeopointSubtypeFromID);
                                            newGeopoint.Subset = newGeopoint.GetGeolpointSubsetFromID;
                                            newGeopoint.Attitude = newGeopoint.GetGeopointAttitudeFromID;
                                            newGeopoint.Generation = newGeopoint.GetGeopointGenerationFromID;
                                            newGeopoint.Younging = newGeopoint.GetGeopointYoungingFromID;
                                            newGeopoint.Method = newGeopoint.GetGeopointMethodFromID;

                                            if (is210Model)
                                            {
                                                newGeopoint.GSCSymbol = pointRow[Constants.DatabaseFields.LegendSymbol_190101].ToString();
                                            }
                                            else
                                            {
                                                newGeopoint.GSCSymbol = pointRow[Constants.DatabaseFields.LegendSymbol].ToString();
                                            }

                                            legendGeopoints.Add(newGeopoint);

                                        }
                                    }
                                }

                                if (legendGeopoints.Count() > 0)
                                {

                                    //Iterate through all legend geolines and see if a template exists, else add it
                                    foreach (GeoPoints gp in legendGeopoints)
                                    {
                                        if (!templates.Exists(x => x.Description == gp.GeopointID))
                                        {
                                            Symbols.CreatePointTemplate(inLayer, gp);
                                        }
                                    }
                                }

                                #endregion
                            }
                            else if (layerClass.GetName().Contains(Constants.Database.FLabel))
                            {
                                #region Labels

                                QueryFilter legendFilter = new QueryFilter
                                {
                                    SubFields = string.Format("{0}, {1}, {2}", Constants.DatabaseFields.LegendSymbol, Constants.DatabaseFields.LegendLabelID,
                                        Constants.DatabaseFields.LegendGISDisplay),
                                     WhereClause = string.Format("{0} IS NOT NULL AND {1} IN ({2})",
                                        Constants.DatabaseFields.LegendSymbol, Constants.DatabaseFields.LegendItemType, string.Format("'{0}'", string.Join("','", pLegend.UnitElements)))
                                };

                                if (is210Model)
                                {
                                    legendFilter = new QueryFilter
                                    {
                                        SubFields = string.Format("{0}, {1}, {2}", Constants.DatabaseFields.LegendSymbol_190101, Constants.DatabaseFields.LegendLabelID,
                                                                    Constants.DatabaseFields.LegendGISDisplay),
                                        WhereClause = string.Format("{0} IS NOT NULL AND {1} = '{2}'",
                                                                    Constants.DatabaseFields.LegendSymbol_190101, Constants.DatabaseFields.LegendItemType_190101,
                                                                    Constants.DatabaseDomainsValues.legendItemMapUnit)
                                    };
                                }


                                List<Labels> legendLabels = new List<Labels>();

                                using (RowCursor pointCursor = legendTable.Search(legendFilter))
                                {
                                    while (pointCursor.MoveNext())
                                    {
                                        using (Row pointRow = pointCursor.Current)
                                        {
                                            if (is210Model)
                                            {
                                                legendLabels.Add(new Labels
                                                {
                                                    GSCSymbol = pointRow[Constants.DatabaseFields.LegendSymbol_190101].ToString(),
                                                    LabelID = pointRow[Constants.DatabaseFields.LegendLabelID].ToString(),
                                                    Name = pointRow[Constants.DatabaseFields.LegendGISDisplay].ToString(),
                                                    CreatorID = Properties.Settings.Default.SelectedParticipantCode
                                                });
                                            }
                                            else
                                            {
                                                legendLabels.Add(new Labels
                                                {
                                                    GSCSymbol = pointRow[Constants.DatabaseFields.LegendSymbol].ToString(),
                                                    LabelID = pointRow[Constants.DatabaseFields.LegendLabelID].ToString(),
                                                    Name = pointRow[Constants.DatabaseFields.LegendGISDisplay].ToString(),
                                                    CreatorID = Properties.Settings.Default.SelectedParticipantCode
                                                });
                                            }
                                        }
                                    }
                                }

                                if (legendLabels.Count() > 0)
                                {

                                    //Iterate through all legend geolines and see if a template exists, else add it
                                    foreach (Labels gp in legendLabels)
                                    {
                                        if (!templates.Exists(x => x.Description == gp.LabelID))
                                        {
                                            Symbols.CreateLabelTemplate(inLayer, gp);
                                        }
                                    }
                                }


                                #endregion
                            }

                        }
                    }
                }
            } 
        }

               
        #endregion
    }


}
