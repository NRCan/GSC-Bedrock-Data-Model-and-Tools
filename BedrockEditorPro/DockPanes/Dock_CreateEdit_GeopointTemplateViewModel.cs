using ArcGIS.Core.CIM;
using ArcGIS.Core.Data;
using ArcGIS.Core.Geometry;
using ArcGIS.Desktop.Catalog;
using ArcGIS.Desktop.Core;
using ArcGIS.Desktop.Editing;
using ArcGIS.Desktop.Extensions;
using ArcGIS.Desktop.Framework;
using ArcGIS.Desktop.Framework.Contracts;
using ArcGIS.Desktop.Framework.Dialogs;
using ArcGIS.Desktop.Framework.Events;
using ArcGIS.Desktop.Framework.Threading.Tasks;
using ArcGIS.Desktop.KnowledgeGraph;
using ArcGIS.Desktop.Layouts;
using ArcGIS.Desktop.Mapping;
using ArcGIS.Desktop.Mapping.Events;
using BedrockEditorPro.Models;
using BedrockEditorPro.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using static BedrockEditorPro.Utilities.Layers;

namespace BedrockEditorPro.DockPanes
{
    internal class Dock_CreateEdit_GeopointTemplateViewModel : DockPane
    {
        #region INIT
        private const string _dockPaneID = "BedrockEditorPro_DockPanes_Dock_CreateEdit_GeopointTemplate";
        private object _lock = new(); //For obs. collection
        private WorkingEnvironment workingEnvironment = new WorkingEnvironment();
        private Uri _uriGeodatabase = null; //Selected geoline layer uri
        #endregion


        #region PROPERTIES

        /// <summary>
        /// Text shown near the top of the DockPane.
        /// </summary>
        private string _heading = "My DockPane";
        public string Heading
        {
            get => _heading;
            set => SetProperty(ref _heading, value);
        }

        //Layer controls
        private ObservableCollection<LayerDisplay> _geopointLayers = new();
        public ObservableCollection<LayerDisplay> GeopointLayers
        {
            get { return _geopointLayers; }
        }
        private int _geopointSelectedLayerIndex = -1;
        public int GeopointSelectedLayerIndex
        {
            get { return _geopointSelectedLayerIndex; }
            set
            {
                SetProperty(ref _geopointSelectedLayerIndex, value, () => _geopointSelectedLayerIndex);

                FillGeopointType();
            }
        }

        private ObservableCollection<ComboBoxItem> _geopointType = new();
        public ObservableCollection<ComboBoxItem> GeopointType
        {
            get { return _geopointType; }
        }
        private int _geopointTypeSelectedIndex = -1;
        public int GeopointTypeSelectedIndex
        {
            get { return _geopointTypeSelectedIndex; }
            set
            {
                SetProperty(ref _geopointTypeSelectedIndex, value, () => _geopointTypeSelectedIndex);

                FillGeopointSubset();
                FillGeopointAttitude();
                FillGeopointGeneration();
                FillGeopointYounging();
                FillGeopointMethod();
            }
        }

        private ObservableCollection<ComboBoxItem> _geopointSubset = new();
        public ObservableCollection<ComboBoxItem> GeopointSubset
        {
            get { return _geopointSubset; }
        }
        private int _geopointSubsetSelectedIndex = -1;
        public int GeopointSubsetSelectedIndex
        {
            get { return _geopointSubsetSelectedIndex; }
            set
            {
                SetProperty(ref _geopointSubsetSelectedIndex, value, () => _geopointSubsetSelectedIndex);
            }
        }

        private ObservableCollection<ComboBoxItem> _geopointAttitude = new();
        public ObservableCollection<ComboBoxItem> GeopointAttitude
        {
            get { return _geopointAttitude; }
        }
        private int _geopointAttitudeSelectedIndex = -1;
        public int GeopointAttitudeSelectedIndex
        {
            get { return _geopointAttitudeSelectedIndex; }
            set
            {
                SetProperty(ref _geopointAttitudeSelectedIndex, value, () => _geopointAttitudeSelectedIndex);
            }
        }

        private ObservableCollection<ComboBoxItem> _geopointGeneration= new();
        public ObservableCollection<ComboBoxItem> GeopointGeneration
        {
            get { return _geopointGeneration; }
        }
        private int _geopointGenerationSelectedIndex = -1;
        public int GeopointGenerationSelectedIndex
        {
            get { return _geopointGenerationSelectedIndex; }
            set
            {
                SetProperty(ref _geopointGenerationSelectedIndex, value, () => _geopointGenerationSelectedIndex);
            }
        }

        private ObservableCollection<ComboBoxItem> _geopointYounging = new();
        public ObservableCollection<ComboBoxItem> GeopointYounging
        {
            get { return _geopointYounging; }
        }
        private int _geopointYoungingSelectedIndex = -1;
        public int GeopointYoungingSelectedIndex
        {
            get { return _geopointYoungingSelectedIndex; }
            set
            {
                SetProperty(ref _geopointYoungingSelectedIndex, value, () => _geopointYoungingSelectedIndex);
            }
        }

        private ObservableCollection<ComboBoxItem> _geopointMethod= new();
        public ObservableCollection<ComboBoxItem> GeopointMethod
        {
            get { return _geopointMethod; }
        }
        private int _geopointMethodSelectedIndex = -1;
        public int GeopointMethodSelectedIndex
        {
            get { return _geopointMethodSelectedIndex; }
            set
            {
                SetProperty(ref _geopointMethodSelectedIndex, value, () => _geopointMethodSelectedIndex);
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
                    _runTool = new RelayCommand(() => AddGeopointTemplate(), () => true);
                }
                return _runTool;
            }
        }

        #endregion

        #region EVENTS
        protected override void OnShow(bool isVisible)
        {
            base.OnShow(isVisible);

            //Subscribe to some events, in order to refil the layer combobox with latest values
            //Unsubscribe first else they accumulate each time the pane is showed
            ArcGIS.Desktop.Mapping.Events.LayersAddedEvent.Unsubscribe(OnLayersAdded);
            ArcGIS.Desktop.Mapping.Events.LayersAddedEvent.Subscribe(OnLayersAdded);
            ArcGIS.Desktop.Framework.Events.ActivePaneChangedEvent.Unsubscribe(OnActivePaneChanged);
            ArcGIS.Desktop.Framework.Events.ActivePaneChangedEvent.Subscribe(OnActivePaneChanged);


            //Init as obs. collection the comboboxes
            BindingOperations.EnableCollectionSynchronization(_geopointLayers, _lock);
            BindingOperations.EnableCollectionSynchronization(_geopointType, _lock);
            BindingOperations.EnableCollectionSynchronization(_geopointSubset, _lock);
            BindingOperations.EnableCollectionSynchronization(_geopointAttitude, _lock);
            BindingOperations.EnableCollectionSynchronization(_geopointGeneration, _lock);
            BindingOperations.EnableCollectionSynchronization(_geopointYounging, _lock);
            BindingOperations.EnableCollectionSynchronization(_geopointMethod, _lock);

            //Init some components
            UpdateLayerComboboxAsync();
        }

        /// <summary>
        /// Make sure to refresh layer list if users changes map panes
        /// </summary>
        /// <param name="args"></param>
        private void OnActivePaneChanged(PaneEventArgs args)
        {
            UpdateLayerComboboxAsync();
        }

        /// <summary>
        /// Make sure to refresh layer list of user adds any new layers
        /// </summary>
        /// <param name="args"></param>
        private void OnLayersAdded(LayerEventsArgs args)
        {
            UpdateLayerComboboxAsync();
        }

        #endregion

        protected Dock_CreateEdit_GeopointTemplateViewModel() { }

        /// <summary>
        /// Show the DockPane.
        /// </summary>
        internal static void Show()
        {
            DockPane pane = FrameworkApplication.DockPaneManager.Find(_dockPaneID);
            if (pane == null)
                return;

            pane.Activate();
        }

        #region METHODS

        /// <summary>
        /// Will fill the layer combobox with valid geopoint layers from current map
        /// </summary>
        /// <returns></returns>
        public async void UpdateLayerComboboxAsync()
        {
            try
            {
                await QueuedTask.Run(() =>
                {
                    if (MapView.Active != null && MapView.Active.Map != null)
                    {
                        List<FeatureLayer> layerEnum = MapView.Active.Map.GetLayersAsFlattenedList().OfType<FeatureLayer>().ToList();
                        if (layerEnum != null)
                        {
                            _geopointLayers.Clear();
                            foreach (FeatureLayer fl in layerEnum)
                            {

                                if (fl.ShapeType == esriGeometryType.esriGeometryPoint || fl.ShapeType == esriGeometryType.esriGeometryMultipoint)
                                {
                                    //Get some definition to valide field and move with getting first symbol
                                    CIMFeatureLayer cIMFeatureLayer = fl.GetDefinition() as CIMFeatureLayer;
                                    FeatureClass featureClass = fl.GetFeatureClass();
                                    featureClass.GetName();

                                    if (cIMFeatureLayer != null && featureClass != null && featureClass.GetName().Contains(Utilities.Constants.Database.FGeopoint))
                                    {
                                        LayerDisplay layerItem = MakeComboBoxItemWithSymbolIcons(cIMFeatureLayer, fl);
                                        _geopointLayers.Add(layerItem);
                                    }
                                }
                            }

                            if (_geopointLayers.Count == 1)
                            {
                                _geopointSelectedLayerIndex = 0;
                                FillGeopointType();
                            }

                            NotifyPropertyChanged(nameof(GeopointSelectedLayerIndex));
                        }
                    }
                });


            }
            catch (Exception ex)
            {
                new ErrorService(ex).WriteToFile();
            }

        }

        /// <summary>
        /// Will fill the geopoint type combobox based on selected feature layer
        /// </summary>
        private void FillGeopointType()
        {
            if (GeopointSelectedLayerIndex != -1 && GeopointType != null)
            {

                QueuedTask.Run(() =>
                {

                    _geopointTypeSelectedIndex = -1;
                    NotifyPropertyChanged(nameof(GeopointTypeSelectedIndex));
                    GeopointType.Clear();
                    NotifyPropertyChanged(nameof(GeopointType));

                    GeopointSubsetSelectedIndex = -1;
                    NotifyPropertyChanged(nameof(GeopointSubsetSelectedIndex));
                    GeopointSubset.Clear();
                    NotifyPropertyChanged(nameof(GeopointSubset));

                    GeopointAttitudeSelectedIndex = -1;
                    NotifyPropertyChanged(nameof(GeopointAttitudeSelectedIndex));
                    GeopointAttitude.Clear();
                    NotifyPropertyChanged(nameof(GeopointAttitude));

                    GeopointGenerationSelectedIndex = -1;
                    NotifyPropertyChanged(nameof(GeopointGenerationSelectedIndex));
                    GeopointGeneration.Clear();
                    NotifyPropertyChanged(nameof(GeopointGeneration));

                    GeopointYoungingSelectedIndex = -1;
                    NotifyPropertyChanged(nameof(GeopointYoungingSelectedIndex));
                    GeopointYounging.Clear();
                    NotifyPropertyChanged(nameof(GeopointYounging));

                    GeopointMethodSelectedIndex = -1;
                    NotifyPropertyChanged(nameof(GeopointMethodSelectedIndex));
                    GeopointMethod.Clear();
                    NotifyPropertyChanged(nameof(GeopointMethod));

                    FeatureLayer pointLayer = GeopointLayers[GeopointSelectedLayerIndex].FLayer;
                    _uriGeodatabase = Workspace.GetWorkspacePathFromFeatureLayer(pointLayer);

                    if (_uriGeodatabase != null && _geopointType.Count() == 0 && Directory.Exists(_uriGeodatabase.OriginalString))
                    {

                        try
                        {
                            //Get origin database
                            using (Geodatabase sourceGeodatabase = new Geodatabase(new FileGeodatabaseConnectionPath(_uriGeodatabase)))
                            {
                                SortedList<string, string> typeDico = Utilities.Subtypes.GetSubtypeDicoFromWorkspace(sourceGeodatabase, pointLayer.GetFeatureClass().GetName());
                                if (typeDico != null)
                                {
                                    foreach (KeyValuePair<string, string> types in typeDico)
                                    {
                                        ComboBoxItem boxItem = new ComboBoxItem();
                                        boxItem.Text = types.Key;
                                        boxItem.Tooltip = types.Value;
                                        _geopointType.Add(boxItem);
                                        NotifyPropertyChanged(nameof(GeopointType));
                                    }
                                }
                                else
                                {
                                    new ErrorService(Properties.Resources.FormCreateEditGeopointNoSubtypes).WriteToFile();
                                }

                            }

                        }
                        catch (Exception ex)
                        {
                            new ErrorService(ex).WriteToFile();
                        }

                    }
                    else
                    {
                        new ErrorService(Properties.Resources.FormCreateEditGeopointNoSource).WriteToFile();
                    }

                });

            }
        }

        /// <summary>
        /// Will fill the geopoint subset combobox based on selected feature layer
        /// </summary>
        private void FillGeopointSubset()
        {
            if (GeopointTypeSelectedIndex != -1 && _uriGeodatabase != null)
            {

                QueuedTask.Run(() =>
                {
                    FillCombobox(Utilities.Constants.DatabaseFields.FGeopointSubset, _geopointSubset, nameof(GeopointSubset),
                        GeopointSubsetSelectedIndex, nameof(GeopointSubsetSelectedIndex));
                });
            }
        }

        /// <summary>
        /// Will fill the geopoint attitude combobox based on selected feature layer
        /// </summary>
        private void FillGeopointAttitude()
        {
            if (GeopointTypeSelectedIndex != -1 && _uriGeodatabase != null)
            {

                QueuedTask.Run(() =>
                {
                    FillCombobox(Utilities.Constants.DatabaseFields.FGeopointStrucAtt, _geopointAttitude, nameof(GeopointAttitude),
                        GeopointAttitudeSelectedIndex, nameof(GeopointAttitudeSelectedIndex));
                });
            }
        }

        /// <summary>
        /// Will fill the geopoint generation combobox based on selected feature layer
        /// </summary>
        private void FillGeopointGeneration()
        {
            if (GeopointTypeSelectedIndex != -1 && _uriGeodatabase != null)
            {

                QueuedTask.Run(() =>
                {
                    FillCombobox(Utilities.Constants.DatabaseFields.FGeopointStrucGene, _geopointGeneration, nameof(GeopointGeneration),
                        GeopointGenerationSelectedIndex, nameof(GeopointGenerationSelectedIndex));
                });
            }
        }

        /// <summary>
        /// Will fill the geopoint younging combobox based on selected feature layer
        /// </summary>
        private void FillGeopointYounging()
        {
            if (GeopointTypeSelectedIndex != -1 && _uriGeodatabase != null)
            {

                QueuedTask.Run(() =>
                {
                    FillCombobox(Utilities.Constants.DatabaseFields.FGeopointStrucYoung, _geopointYounging, nameof(GeopointYounging),
                        GeopointYoungingSelectedIndex, nameof(GeopointYoungingSelectedIndex));
                });
            }
        }

        /// <summary>
        /// Will fill the geopoint method combobox based on selected feature layer
        /// </summary>
        private void FillGeopointMethod()
        {
            if (GeopointTypeSelectedIndex != -1 && _uriGeodatabase != null)
            {

                QueuedTask.Run(() =>
                {
                    FillCombobox(Utilities.Constants.DatabaseFields.FGeopointStrucMethod, _geopointMethod, nameof(GeopointMethod),
                        GeopointMethodSelectedIndex, nameof(GeopointMethodSelectedIndex));
                });
            }
        }

        /// <summary>
        /// Will fill a combobox based on domain assigned to a specific subtype and field
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="collection"></param>
        /// <param name="propertyName"></param>
        private void FillCombobox(string fieldName, ObservableCollection<ComboBoxItem> collection, string collectionPropertyName, int collectionIndex, string collectionIndexPropertyName)
        {
            try
            {

                FeatureLayer lineLayer = GeopointLayers[GeopointSelectedLayerIndex].FLayer;

                //Get origin database
                using (Geodatabase sourceGeodatabase = new Geodatabase(new FileGeodatabaseConnectionPath(_uriGeodatabase)))
                {
                    //Clean
                    collectionIndex = -1;
                    NotifyPropertyChanged(nameof(collectionIndexPropertyName));
                    collection.Clear();
                    NotifyPropertyChanged(nameof(collectionPropertyName));

                    SortedList<object, string> qualifDico = Utilities.Domains.GetDomDicoFromSubtype(sourceGeodatabase,
                        lineLayer.GetFeatureClass().GetName(), GeopointType[GeopointTypeSelectedIndex].Tooltip, fieldName);
                    if (qualifDico != null)
                    {
                        foreach (KeyValuePair<object, string> types in qualifDico)
                        {
                            ComboBoxItem boxItem = new ComboBoxItem();
                            boxItem.Text = types.Value;
                            boxItem.Tooltip = types.Key.ToString();
                            collection.Add(boxItem);
                            NotifyPropertyChanged(collectionPropertyName);
                        }

                        if (collection.Count() == 1)
                        {
                            collectionIndex = 0;
                            NotifyPropertyChanged(collectionIndexPropertyName);
                        }
                    }
                    else
                    {
                        new ErrorService(Properties.Resources.FormCreateEditGeopointNoSubtypesDomains).WriteToFile();
                    }

                }

            }
            catch (Exception ex)
            {
                new ErrorService(ex).WriteToFile();
            }
        }

        /// <summary>
        /// Will add the selected geopoint in the legend table and create the template for it.
        /// </summary>
        private void AddGeopointTemplate()
        {
            try
            {
                if (GeopointTypeSelectedIndex != -1 && GeopointSubsetSelectedIndex != -1 &&
                    GeopointAttitudeSelectedIndex != -1 && GeopointGenerationSelectedIndex != -1 &&
                    GeopointYoungingSelectedIndex != -1 && GeopointMethodSelectedIndex != -1)
                {
                    if (_uriGeodatabase != null)
                    {
                        QueuedTask.Run(async () =>
                        {
                            using (Geodatabase sourceGeodatabase = new Geodatabase(new FileGeodatabaseConnectionPath(_uriGeodatabase)))
                            {
                                //Build geopoint model (will be used for validation and template creation)
                                GeoPoints _geopoint = new GeoPoints();
                                _geopoint.GeopointID = string.Format("{0}{1}{2}{3}{4}{5}",
                                    GeopointType[GeopointTypeSelectedIndex].Tooltip,
                                    GeopointSubset[GeopointSubsetSelectedIndex].Tooltip,
                                    GeopointAttitude[GeopointAttitudeSelectedIndex].Tooltip,
                                    GeopointGeneration[GeopointGenerationSelectedIndex].Tooltip,
                                    GeopointYounging[GeopointYoungingSelectedIndex].Tooltip,
                                    GeopointMethod[GeopointMethodSelectedIndex].Tooltip);
                                _geopoint.GeopointType = int.Parse(GeopointType[GeopointTypeSelectedIndex].Tooltip);
                                _geopoint.Attitude = GeopointAttitude[GeopointAttitudeSelectedIndex].Tooltip;
                                _geopoint.Generation = GeopointGeneration[GeopointGenerationSelectedIndex].Tooltip;
                                _geopoint.Younging = GeopointYounging[GeopointYoungingSelectedIndex].Tooltip;
                                _geopoint.Subset = GeopointSubset[GeopointSubsetSelectedIndex].Tooltip;
                                _geopoint.Method = GeopointMethod[GeopointMethodSelectedIndex].Tooltip;
                                _geopoint.CreatorID = Properties.Settings.Default.SelectedParticipantCode;

                                //Validate if geopoint exists within symbol tables
                                QueryFilter symbolTableFilter = new QueryFilter()
                                {
                                    WhereClause = string.Format("{0} = '{1}'", Constants.DatabaseFields.FGeopointID, _geopoint.GeopointID)
                                };

                                using (Table symbolTable = sourceGeodatabase.OpenDataset<Table>(Utilities.Constants.Database.TGeopointSymbol))
                                {
                                    RowCursor symCursor = symbolTable.Search(symbolTableFilter);

                                    while (symCursor.MoveNext())
                                    {
                                        Row symRow = symCursor.Current;
                                        _geopoint.GSCSymbol = symRow[Utilities.Constants.DatabaseFields.TGeopointFGDC].ToString();
                                        _geopoint.Name = symRow[Utilities.Constants.DatabaseFields.TGeopointLegendDesc].ToString();
                                    }
                                }

                                if (_geopoint.GSCSymbol != null && _geopoint.GSCSymbol != string.Empty)
                                {
                                    using (Table legendTable = sourceGeodatabase.OpenDataset<Table>(Utilities.Constants.Database.TLegendGene))
                                    {
                                        bool geopointIDExists = false;

                                        //Query filter for geopoint only
                                        QueryFilter geopointFilter = new QueryFilter()
                                        {
                                            WhereClause = string.Format("{0} = '{1}'", Utilities.Constants.DatabaseFields.LegendLabelID, _geopoint.GeopointID)
                                        };

                                        RowCursor rowCursor = legendTable.Search(geopointFilter);
                                        while (rowCursor.MoveNext())
                                        {
                                            Row currentRow = rowCursor.Current;

                                            if (currentRow != null)
                                            {
                                                geopointIDExists = true;
                                                break;
                                            }
                                        }

                                        //Insert new record in legend if it's not already there
                                        if (!geopointIDExists)
                                        {
                                            //Prepare callback in case something happens
                                            EditOperation editOp = new EditOperation();
                                            editOp.Callback(async context =>
                                            {
                                                //Prepare a buffer to store information before insertion
                                                using (RowBuffer rowBuffer = legendTable.CreateRowBuffer())
                                                {
                                                    rowBuffer[Constants.DatabaseFields.LegendLabelID] = _geopoint.GeopointID;
                                                    rowBuffer[Constants.DatabaseFields.LegendGISDisplay] = _geopoint.Name;
                                                    rowBuffer[Constants.DatabaseFields.LegendSymbol] = _geopoint.GSCSymbol;
                                                    rowBuffer[Constants.DatabaseFields.LegendItemType] = Constants.DatabaseDomainsValues.legendItemGeopoint;

                                                    //Create row with the buffer
                                                    using (Row row = legendTable.CreateRow(rowBuffer))
                                                    {
                                                        context.Invalidate(row);
                                                    }
                                                }
                                            }, legendTable);

                                            editOp.Execute();

                                            //Save edits
                                            await Project.Current.SaveEditsAsync();

                                            //Create and or update template
                                            Symbols.CreatePointTemplate(GeopointLayers[GeopointSelectedLayerIndex].FLayer, _geopoint);

                                            //Show notication success
                                            FrameworkApplication.AddNotification(new Notification()
                                            {
                                                Title = Properties.Resources.FormCreateEditGeopointTitle,
                                                Message = Properties.Resources.GenericMessageCompleted,
                                                ImageSource = System.Windows.Application.Current.Resources["Success_Toast48"] as ImageSource
                                            });
                                        }
                                        else
                                        {
                                            MessageBox.Show(Properties.Resources.FormCreateEditGeopointExists, Properties.Resources.GenericWarningTitle, System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Exclamation);
                                        }

                                    }
                                }
                                else
                                {
                                    MessageBox.Show(Properties.Resources.FormCreateEditGeopointUndefined, Properties.Resources.GenericWarningTitle, System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
                                }
                            }
                        });

                    }
                }
                else
                {
                    MessageBox.Show(Properties.Resources.FormCreateEditGeopointMissingSelection, Properties.Resources.GenericWarningTitle, System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                new ErrorService(ex).WriteToFile();
            }
        }

        #endregion
    }

    /// <summary>
    /// Button implementation to show the DockPane.
    /// </summary>
    internal class Dock_CreateEdit_GeopointTemplate_ShowButton : Button
    {
        protected override void OnClick()
        {
            Dock_CreateEdit_GeopointTemplateViewModel.Show();
        }
    }
}
