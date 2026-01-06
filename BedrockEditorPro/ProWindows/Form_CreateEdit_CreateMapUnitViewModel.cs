using ArcGIS.Core.CIM;
using ArcGIS.Core.Data;
using ArcGIS.Core.Data.Exceptions;
using ArcGIS.Core.Data.LinearReferencing;
using ArcGIS.Core.Geometry;
using ArcGIS.Core.Internal.CIM;
using ArcGIS.Core.Internal.Threading.Tasks;
using ArcGIS.Desktop.Core;
using ArcGIS.Desktop.Editing;
using ArcGIS.Desktop.Framework;
using ArcGIS.Desktop.Framework.Contracts;
using ArcGIS.Desktop.Framework.Threading.Tasks;
using ArcGIS.Desktop.Internal.Mapping;
using ArcGIS.Desktop.Layouts;
using ArcGIS.Desktop.Mapping;
using BedrockEditorPro.Models;
using BedrockEditorPro.Services;
using BedrockEditorPro.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using static BedrockEditorPro.Utilities.Constants;
using static BedrockEditorPro.Utilities.Layers;
using Field = ArcGIS.Core.Data.Field;
using Layers = BedrockEditorPro.Utilities.Layers;
using QueryFilter = ArcGIS.Core.Data.QueryFilter;
using Workspace = BedrockEditorPro.Utilities.Workspace;

namespace BedrockEditorPro.ProWindows
{
    public class Form_CreateEdit_CreateMapUnitViewModel: Layers
    {

        #region INIT

        private WorkingEnvironment workingEnvironment = new WorkingEnvironment();
        private Form_CreateEdit_CreateMapUnit _view = null;
        private Visibility _waitingCursorVisibility = Visibility.Collapsed;

        private object _lock = new(); //For obs. collection
        private bool _doesHaveOverprints = false;

        private Uri _areaLayerSourceUri = null;

        #endregion

        #region PROPERTIES

        //Layer controls
        private ObservableCollection<LayerDisplay> _mapUnitsLayers = new();
        public ObservableCollection<LayerDisplay> MapUnitsLayers
        {
            get { return _mapUnitsLayers; }
        }
        private int _mapUnitsSelectedLayerIndex = -1;
        public int MapUnitsSelectedLayerIndex
        {
            get { return _mapUnitsSelectedLayerIndex; }
            set
            {
                SetProperty(ref _mapUnitsSelectedLayerIndex, value, () => _mapUnitsSelectedLayerIndex);
            }
        }

        public Visibility WaitingCursorVisibility
        {
            get { return _waitingCursorVisibility; }
            set
            {
                SetProperty(ref _waitingCursorVisibility, value, () => WaitingCursorVisibility);
            }
        }


        #endregion

        public Form_CreateEdit_CreateMapUnitViewModel(Form_CreateEdit_CreateMapUnit view)
        {
            //Init as obs. collection the comboboxes
            BindingOperations.EnableCollectionSynchronization(_mapUnitsLayers, _lock);


            //Set related view
            _view = view;

            //Init some components
            UpdateLayerCombobox();

        }

        #region RELAYS

        private ICommand _runTool = null;
        public ICommand RunTool
        {
            get
            {
                if (_runTool == null)
                {
                    _runTool = new RelayCommand(() => CreateMapUnit(), () => true);
                }
                return _runTool;
            }
        }

        #endregion

        #region METHOD
        private async void CreateMapUnit()
        {
            try 
            {
                WaitingCursorVisibility = Visibility.Visible;

                await QueuedTask.Run(async () =>
                {
                    //Get temp workspace for faster processing
                    MemoryConnectionProperties memoryConnection = Utilities.Workspace.CreateInMemoryWorkspace();
                    using (Geodatabase inMemoryWorkspace = new Geodatabase(memoryConnection))
                    {
                        //Get selected layer and continue
                        if (MapUnitsLayers.Count() > 0 && MapUnitsSelectedLayerIndex != -1)
                        {
                            //Get origin database
                            FeatureLayer geopolyFL = MapUnitsLayers[MapUnitsSelectedLayerIndex].FLayer;
                            Uri _muLayerSourceUri = Workspace.GetWorkspacePath(geopolyFL);
                            using (Geodatabase sourceGeodatabase = new Geodatabase(new FileGeodatabaseConnectionPath(_muLayerSourceUri)))
                            {
                                //Empty geopoly feature from it's content
                                using (FeatureClass geopolyFC = sourceGeodatabase.OpenDataset<FeatureClass>(Utilities.Constants.Database.FGeopoly))
                                {

                                    //Prepare callback in case something happens
                                    EditOperation editOp = new EditOperation();
                                    editOp.Callback(async context =>
                                    {
                                        geopolyFC.DeleteRows(new QueryFilter());
                                    }, geopolyFC);

                                    try
                                    {
                                        editOp.Execute();
                                    }
                                    catch (GeodatabaseException gdbEx)
                                    {
                                        new ErrorService(gdbEx).WriteToFile();
                                        WaitingCursorVisibility = Visibility.Collapsed;
                                        _view.Close();

                                        FrameworkApplication.AddNotification(new Notification()
                                        {
                                            Title = Properties.Resources.FormCreateEditCreateMapUnitTitle,
                                            Message = Properties.Resources.GenericMessageError,
                                            ImageSource = System.Windows.Application.Current.Resources["Warning_Toast48"] as ImageSource
                                        });
                                    }

                                    //Open label feature class and check for overprints
                                    if (Utilities.Workspace.FeatureClassExists(sourceGeodatabase, Utilities.Constants.Database.FLabel))
                                    {
                                        using (FeatureClass labelFC = sourceGeodatabase.OpenDataset<FeatureClass>(Utilities.Constants.Database.FLabel))
                                        {

                                            if (Utilities.Workspace.FeatureClassExists(sourceGeodatabase, Utilities.Constants.Database.FGeoline))
                                            {
                                                using (FeatureClass geolineFC = sourceGeodatabase.OpenDataset<FeatureClass>(Utilities.Constants.Database.FGeoline))
                                                {
                                                    //Build a feature layer for geoline without overprints
                                                    string noOverprintQuery = string.Format("{0} <> {1} AND {2} = '{3}'", DatabaseFields.FGeolineSubtype,
                                                        DatabaseSubtypes.FGeolineSubOverprint, DatabaseFields.FGeolineBoundary, DatabaseDomainsValues.BoundYes);
                                                    FeatureLayer geolineFL = GetFeatureLayerFromQuery(geolineFC, noOverprintQuery, Properties.Resources.LayerNamingGeolineNoOverprint);

                                                    //Build a feature layer for labels without overprints
                                                    string noOverprintLabelQuery = string.Empty;
                                                    SortedList<object, string> labelDomain = Utilities.Domains.GetDomDicoFromWorkspace(sourceGeodatabase, DatabaseDomains.MapUnit);
                                                    Dictionary<object, string> overprintlabels = labelDomain.Where(x => x.Value.Contains(ValueKeywords.labelOverprint)).ToDictionary<object, string>();

                                                    if (overprintlabels != null && overprintlabels.Count() > 0)
                                                    {
                                                        List<object> noOverprintLabelCodes = overprintlabels.Keys.ToList();
                                                        noOverprintLabelQuery = BuildOverprintLabelQueryString(noOverprintLabelCodes);
                                                    }

                                                    FeatureLayer labelFL = GetFeatureLayerFromQuery(labelFC, noOverprintLabelQuery, Properties.Resources.LayerNamingLabelNoOverprint);

                                                    //Create polygons from selected lines and labels
                                                    string temporaryGeopolyNoOverprints = string.Format("memory/{0}", Properties.Resources.LayerNameGeopolyNoOverprint);
                                                    await Utilities.GeoprocessingBedrock.FeaturesToPolygon(geolineFL, labelFL, temporaryGeopolyNoOverprints);

                                                    //Append to map unit feature class
                                                    await Utilities.GeoprocessingBedrock.AppendInEmptyTables(temporaryGeopolyNoOverprints, geopolyFC);

                                                    //Parse all levels of overprints, if any
                                                    if (overprintlabels != null && overprintlabels.Count() > 0)
                                                    {
                                                        //Prepare a new geoline feature layer only for overprints
                                                        string overprintGeolineQuery = string.Format("{0} = '{1}'", DatabaseFields.FGeolineBoundary, DatabaseDomainsValues.BoundYes);
                                                        QueryFilter overprintFilter = new QueryFilter()
                                                        {
                                                            WhereClause = overprintGeolineQuery
                                                        };
                                                        geolineFL.Select(overprintFilter, SelectionCombinationMethod.New);

                                                        //Get a dictionary of overprints levels
                                                        Dictionary<int, List<object>> overprintLevels = GetOverprintLevels(overprintlabels);

                                                        foreach (KeyValuePair<int, List<object>> overprints in overprintLevels)
                                                        {
                                                            if (overprints.Value.Count() > 0)
                                                            {
                                                                //Prepare a new feature layer for level X overprint labels
                                                                string levelOverprintQuery = BuildOverprintLabelQueryString(overprints.Value, false);
                                                                QueryFilter levelOverprintFilter = new QueryFilter()
                                                                {
                                                                    WhereClause = levelOverprintQuery
                                                                };
                                                                labelFL.Select(levelOverprintFilter, SelectionCombinationMethod.New);

                                                                //Create polygons from selected lines and labels
                                                                string temporaryLevelOverprints = string.Format("memory/OverprintLevel{0}", overprints.Key.ToString());
                                                                await Utilities.GeoprocessingBedrock.FeaturesToPolygon(geolineFL, labelFL, temporaryLevelOverprints);

                                                                //Force a select on only overprint polygons and not everything else that was created at the same time
                                                                Layer tempLayer = MapView.Active.Map.Layers.Where(x => x.Name == "OverprintLevel" + overprints.Key.ToString()).FirstOrDefault();
                                                                FeatureLayer tempFeatureLayer = tempLayer as FeatureLayer;
                                                                string overprintLevelPolyQuery = string.Format("{0} <> ''", DatabaseFields.FGeopolyLabel);
                                                                QueryFilter polyFilter = new QueryFilter() { WhereClause = overprintLevelPolyQuery };
                                                                tempFeatureLayer.Select(polyFilter, SelectionCombinationMethod.New);

                                                                //Dissolve the overprint polygons (being extracted from all the linework, the output polygons are usually clumped together and forms a single unit)
                                                                string temporaryDissolveOverprints = string.Format("memory/DissolveOverprintLevel{0}", overprints.Key.ToString());

                                                                await Utilities.GeoprocessingBedrock.Dissolve(tempFeatureLayer, temporaryDissolveOverprints, DatabaseFields.FGeopolyLabel);

                                                                //Append to map unit feature class
                                                                await Utilities.GeoprocessingBedrock.AppendInEmptyTables(temporaryDissolveOverprints, geopolyFC);

                                                                //Clean up
                                                                MapView.Active.Map.RemoveLayer(MapView.Active.Map.Layers.Where(x => x.Name == "OverprintLevel" + overprints.Key.ToString()).FirstOrDefault());
                                                                MapView.Active.Map.RemoveLayer(MapView.Active.Map.Layers.Where(x => x.Name == "DissolveOverprintLevel" + overprints.Key.ToString()).FirstOrDefault());
                                                            }
                                                        }
                                                    }

                                                    //Clean up
                                                    try
                                                    {
                                                        MapView.Active.Map.RemoveLayer(geolineFL);
                                                        MapView.Active.Map.RemoveLayer(labelFL);
                                                        MapView.Active.Map.RemoveLayer(MapView.Active.Map.Layers.Where(x => x.Name == Properties.Resources.LayerNameGeopolyNoOverprint).FirstOrDefault());

                                                    }
                                                    catch (Exception e)
                                                    {
                                                        new ErrorService(e).WriteToFile();
                                                    }

                                                    //Update GSC_SYMBOL field if exists because since label feature class doesn't have the symbol
                                                    //they are not passed over the map unit when the conversion from geoline to geopoly is being done
                                                    using (Table legendTable = sourceGeodatabase.OpenDataset<Table>(Utilities.Constants.Database.TLegendGene))
                                                    {
                                                        UpdateSymbolField(geopolyFC,legendTable);

                                                        //Update feature layer symbols
                                                        UserConfiguration userConfig = await UserConfigurationService.GetUserConfigurationAsync();
                                                        StyleProjectItem workingStyle = Symbols.GetStyleItemProject(userConfig.StyleFilePath);
                                                        Form_RefreshSymbolsViewModel refreshVM = new Form_RefreshSymbolsViewModel(null);
                                                        refreshVM.RefreshLayerSymbols(geopolyFL, workingStyle);
                                                    }

                                                    //Save edits
                                                    await Project.Current.SaveEditsAsync();

                                                }
                                            }
                                            else
                                            {
                                                throw new Exception(string.Format(Properties.Resources.ErrorMissingFeatureClass, Utilities.Constants.Database.FGeoline));
                                            }
                                        }
                                    }
                                    else
                                    {
                                        throw new Exception(string.Format(Properties.Resources.ErrorMissingFeatureClass, Utilities.Constants.Database.FLabel));
                                    }
                                }
                            }
                        }
                    }
                });

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
        /// Will fill the layer combobox with all feature layers in the map
        /// Optionall i will pre-select the study area layer if it exists
        /// </summary>
        public async void UpdateLayerCombobox()
        {
            //Init some components
            _mapUnitsLayers.Clear();
            List<esriGeometryType> geomTypes = new List<esriGeometryType>() { esriGeometryType.esriGeometryPolygon};
            Layers layerService = new Layers();

            bool updated = await layerService.UpdateLayerCombobox(geomTypes, _mapUnitsLayers, nameof(MapUnitsLayers), _mapUnitsSelectedLayerIndex, nameof(MapUnitsSelectedLayerIndex));

            if (updated)
            {
                NotifyPropertyChanged(nameof(MapUnitsLayers));

                if (_mapUnitsLayers.Count() == 1)
                {
                    _mapUnitsSelectedLayerIndex = 0;
                    NotifyPropertyChanged(nameof(MapUnitsSelectedLayerIndex));
                }
            }

        }

        /// <summary>
        /// Will create a feature layer from a given query and feature class
        /// </summary>
        /// <param name="inFeatureClass"></param>
        /// <param name="inQuery"></param>
        /// <returns></returns>
        public FeatureLayer GetFeatureLayerFromQuery(FeatureClass inFeatureClass, string inQuery, string featureLayerName = "")
        {
            //Prep the filter
            ArcGIS.Core.Data.QueryFilter queryFilter = new ArcGIS.Core.Data.QueryFilter
            {
                WhereClause = inQuery
            };

            //Create feature layer
            ILayerFactory layerFactory = LayerFactory.Instance;
            FeatureLayerCreationParams layerParams = new FeatureLayerCreationParams(inFeatureClass)
            {
                Name = featureLayerName,
                IsVisible = false
            };
            FeatureLayer featureLayer = layerFactory.CreateLayer<FeatureLayer>(layerParams, MapView.Active.Map);
            featureLayer.Select(queryFilter, SelectionCombinationMethod.New);
            
            return featureLayer;
        }

        /// <summary>
        /// Will return a list of each overprint levels inside label feature.
        /// An empty list indicates no overprint at all.
        /// </summary>
        /// <param name="labelFC">The label feature to look into.</param>
        /// <returns></returns>
        private Dictionary<int, List<object>> GetOverprintLevels(Dictionary<object, string> overprintLabelDictionnary)
        {
            //Variable
            Dictionary<int, List<object>> overprintLevels = new Dictionary<int, List<object>>();

            for (int i = 1; i <= 5; i++)
            {
                overprintLevels[i] = new List<object>();
            }

            //Parse incoming dictionary to detect levels
            foreach (KeyValuePair<object, string> items in overprintLabelDictionnary)
            {
                if (items.Value.EndsWith(ValueKeywords.labelOverprint))
                {
                    overprintLevels[1].Add(items.Key.ToString());
                }
                else if (items.Value.EndsWith("2"))
                {
                    overprintLevels[2].Add(items.Key.ToString());
                }
                else if (items.Value.EndsWith("3"))
                {
                    overprintLevels[3].Add(items.Key.ToString());
                }
                else if (items.Value.EndsWith("4"))
                {
                    overprintLevels[4].Add(items.Key.ToString());
                }
                else if (items.Value.EndsWith("5"))
                {
                    overprintLevels[5].Add(items.Key.ToString());
                }
            }

            return overprintLevels;
        }

        /// <summary>
        /// Will build a string query to select a given set of overprint labels or not
        /// Intended to create feature layers
        /// </summary>
        /// <param name="listOfOverprintCodes"></param>
        /// <param name="notIn">If the code needs to be or not in a list</param>
        /// <returns></returns>
        private string BuildOverprintLabelQueryString(List<object> listOfOverprintCodes, bool notIn = true)
        {
            string buildOverprintQuery = string.Empty;
            string stringedList = string.Empty;
            foreach (object item in listOfOverprintCodes)
            {
                stringedList = stringedList + string.Format("'{0}',", item.ToString());
            }
            stringedList = stringedList.Trim(',');
            buildOverprintQuery = string.Format("{0} NOT IN ({1})", DatabaseFields.FGeopolyLabel, stringedList);
            if (!notIn)
            {
                buildOverprintQuery = string.Format("{0} IN ({1})", DatabaseFields.FGeopolyLabel, stringedList);
            }

            return buildOverprintQuery;
        }

        /// <summary>
        /// Will calculate GSC_SYMBOL field from P_LEGEND table, if field exits
        /// </summary>
        /// <param name="geopolyFC"></param>
        private void UpdateSymbolField(FeatureClass inputPolygonFeature, Table legendTable)
        {
            //Model version managing
            List<Field> legendFields = legendTable.GetDefinition().GetFields().ToList();

            PLegend pLegend = new PLegend();

            if (!legendFields.Exists(f => f.Name == DatabaseFields.LegendSymbol))
            {
                return;
            }


            //Build a dictionary of labelids and their associated symbol code
            Dictionary<string, string> symbolDictionary = new Dictionary<string, string>();
            QueryFilter labelSymbolFilter = new QueryFilter
            {
                SubFields = string.Format("{0}, {1}", DatabaseFields.LegendLabelID, DatabaseFields.LegendSymbol),
                WhereClause = string.Format("{0} IS NOT NULL AND {1} IS NOT NULL AND {2} in ({3})",
                DatabaseFields.LegendSymbol, DatabaseFields.LegendLabelID,
                DatabaseFields.LegendItemType, string.Format("'{0}'", string.Join("','", pLegend.UnitElements)))
            };

            using (RowCursor labelRow = legendTable.Search(labelSymbolFilter, false))
            {
                while (labelRow.MoveNext())
                {
                    using (Row row = labelRow.Current)
                    {
                        symbolDictionary[row[DatabaseFields.LegendLabelID].ToString()] = row[DatabaseFields.LegendSymbol].ToString();
                    }
                }
            }

            //Prepare an edit operation
            EditOperation editOp = new EditOperation();
            editOp.Callback(async context =>
            {
                //For update cursor with only two fields
                QueryFilter geopolyFilter = new QueryFilter()
                {
                    SubFields = String.Format("{0}, {1}", DatabaseFields.FGeopolyLabel, DatabaseFields.FGeopolyFGDC)
                };

                //Update geopoly
                using (UpdateCursor geopolyRow = inputPolygonFeature.CreateUpdateCursor(geopolyFilter, false))
                {
                    while (geopolyRow.MoveNext())
                    {
                        using (Row gRow = geopolyRow.Current)
                        {
                            if (symbolDictionary.ContainsKey(gRow[DatabaseFields.FGeopolyLabel].ToString()))
                            {
                                //Invalidate view before editing
                                context.Invalidate(gRow);

                                gRow[DatabaseFields.FGeopolyFGDC] = symbolDictionary[gRow[DatabaseFields.FGeopolyLabel].ToString()];

                                gRow.Store();

                                //Invalidate again for viewing purposes
                                context.Invalidate(gRow);
                            }
                        }
                    }
                }

            }, inputPolygonFeature);

            try
            {
                editOp.Execute();
            }
            catch (GeodatabaseException gdbEx)
            {
                new ErrorService(gdbEx).WriteToFile();
                WaitingCursorVisibility = Visibility.Collapsed;
                _view.Close();

                FrameworkApplication.AddNotification(new Notification()
                {
                    Title = Properties.Resources.FormCreateEditCreateMapUnitTitle,
                    Message = Properties.Resources.GenericMessageError,
                    ImageSource = System.Windows.Application.Current.Resources["Warning_Toast48"] as ImageSource
                });
            }

        }

        #endregion
    }
}
