using ArcGIS.Core.CIM;
using ArcGIS.Core.Data;
using ArcGIS.Core.Data.Exceptions;
using ArcGIS.Core.Geometry;
using ArcGIS.Desktop.Catalog;
using ArcGIS.Desktop.Core;
using ArcGIS.Desktop.Editing;
using ArcGIS.Desktop.Extensions;
using ArcGIS.Desktop.Framework;
using ArcGIS.Desktop.Framework.Contracts;
using ArcGIS.Desktop.Framework.Dialogs;
using ArcGIS.Desktop.Framework.Threading.Tasks;
using ArcGIS.Desktop.Internal.Mapping;
using ArcGIS.Desktop.KnowledgeGraph;
using ArcGIS.Desktop.Layouts;
using ArcGIS.Desktop.Mapping;
using BedrockEditorPro.Models;
using BedrockEditorPro.Services;
using BedrockEditorPro.Utilities;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using static BedrockEditorPro.ProWindows.Form_Load_StudyAreaViewModel;
using static BedrockEditorPro.Utilities.Layers;
using Constants = BedrockEditorPro.Utilities.Constants;
using LayerDisplay = BedrockEditorPro.Utilities.Layers.LayerDisplay;
using MessageBox = ArcGIS.Desktop.Framework.Dialogs.MessageBox;

namespace BedrockEditorPro.DockPanes
{
    internal class Dock_CreateEdit_GeolineTemplateViewModel : DockPane
    {
        #region INIT
        private const string _dockPaneID = "BedrockEditorPro_DockPanes_Dock_CreateEdit_GeolineTemplate";
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
        private ObservableCollection<LayerDisplay> _geolineLayers = new();
        public ObservableCollection<LayerDisplay> GeolineLayers
        {
            get { return _geolineLayers; }
        }
        private int _geolineSelectedLayerIndex = -1;
        public int GeolineSelectedLayerIndex
        {
            get { return _geolineSelectedLayerIndex; }
            set
            {
                SetProperty(ref _geolineSelectedLayerIndex, value, () => _geolineSelectedLayerIndex);

                FillGeolineType();
            }
        }

        private ObservableCollection<ComboBoxItem> _geolineType = new();
        public ObservableCollection<ComboBoxItem> GeolineType
        {
            get { return _geolineType; }
        }
        private int _geolineTypeSelectedIndex = -1;
        public int GeolineTypeSelectedIndex
        {
            get { return _geolineTypeSelectedIndex; }
            set
            {
                SetProperty(ref _geolineTypeSelectedIndex, value, () => _geolineTypeSelectedIndex);

                FillGeolineQualifier();
                FillConfidence();
                FillAttitude();
                FillGeneration();
            }
        }

        private ObservableCollection<ComboBoxItem> _geolineQualifier = new();
        public ObservableCollection<ComboBoxItem> GeolineQualifier
        {
            get { return _geolineQualifier; }
        }
        private int _geolineQualifierSelectedIndex = -1;
        public int GeolineQualifierSelectedIndex
        {
            get { return _geolineQualifierSelectedIndex; }
            set
            {
                SetProperty(ref _geolineQualifierSelectedIndex, value, () => _geolineQualifierSelectedIndex);
            }
        }

        private ObservableCollection<ComboBoxItem> _geolineConfidence = new();
        public ObservableCollection<ComboBoxItem> GeolineConfidence
        {
            get { return _geolineConfidence; }
        }
        private int _geolineConfidenceSelectedIndex = -1;
        public int GeolineConfidenceSelectedIndex
        {
            get { return _geolineConfidenceSelectedIndex; }
            set
            {
                SetProperty(ref _geolineConfidenceSelectedIndex, value, () => _geolineConfidenceSelectedIndex);
            }
        }

        private ObservableCollection<ComboBoxItem> _geolineAttitude = new();
        public ObservableCollection<ComboBoxItem> GeolineAttitude
        {
            get { return _geolineAttitude; }
        }
        private int _geolineAttitudeSelectedIndex = -1;
        public int GeolineAttitudeSelectedIndex
        {
            get { return _geolineAttitudeSelectedIndex; }
            set
            {
                SetProperty(ref _geolineAttitudeSelectedIndex, value, () => _geolineAttitudeSelectedIndex);
            }
        }

        private ObservableCollection<ComboBoxItem> _geolineGeneration= new();
        public ObservableCollection<ComboBoxItem> GeolineGeneration
        {
            get { return _geolineGeneration; }
        }
        private int _geolineGenerationSelectedIndex = -1;
        public int GeolineGenerationSelectedIndex
        {
            get { return _geolineGenerationSelectedIndex; }
            set
            {
                SetProperty(ref _geolineGenerationSelectedIndex, value, () => _geolineGenerationSelectedIndex);
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
                    _runTool = new RelayCommand(() => AddGeolineTemplate(), () => true);
                }
                return _runTool;
            }
        }

        #endregion

        protected override void OnShow(bool isVisible)
        {
            base.OnShow(isVisible);

            //Init as obs. collection the comboboxes
            BindingOperations.EnableCollectionSynchronization(_geolineLayers, _lock);
            BindingOperations.EnableCollectionSynchronization(_geolineType, _lock);
            BindingOperations.EnableCollectionSynchronization(_geolineQualifier, _lock);
            BindingOperations.EnableCollectionSynchronization(_geolineConfidence, _lock);
            BindingOperations.EnableCollectionSynchronization(_geolineAttitude, _lock);
            BindingOperations.EnableCollectionSynchronization(_geolineGeneration, _lock);

            //Init some components
            UpdateLayerComboboxAsync();
        }

        protected Dock_CreateEdit_GeolineTemplateViewModel() 
        {
            //Init as obs. collection the comboboxes
            //BindingOperations.EnableCollectionSynchronization(_geolineLayers, _lock);

            //Init some components
            //UpdateLayerComboboxAsync();

        }

        #region METHODS
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

        /// <summary>
        /// Will fill the layer combobox with valid geoline layers from current map
        /// </summary>
        /// <returns></returns>
        private async void UpdateLayerComboboxAsync()
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
                            _geolineLayers.Clear();
                            foreach (FeatureLayer fl in layerEnum)
                            {

                                if (fl.ShapeType == esriGeometryType.esriGeometryLine || fl.ShapeType == esriGeometryType.esriGeometryPolyline)
                                {
                                    //Get some definition to valide field and move with getting first symbol
                                    CIMFeatureLayer cIMFeatureLayer = fl.GetDefinition() as CIMFeatureLayer;
                                    FeatureClass featureClass = fl.GetFeatureClass();
                                    featureClass.GetName();

                                    if (cIMFeatureLayer != null && featureClass != null && featureClass.GetName().Contains(Utilities.Constants.Database.FGeoline))
                                    {
                                        LayerDisplay layerItem = MakeComboBoxItemWithSymbolIcons(cIMFeatureLayer, fl);
                                        _geolineLayers.Add(layerItem);
                                    }
                                }
                            }

                            if (_geolineLayers.Count == 1)
                            {
                                _geolineSelectedLayerIndex = 0;
                                FillGeolineType();
                            }

                            NotifyPropertyChanged(nameof(GeolineSelectedLayerIndex));
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
        /// Will fill the geoline type combobox based on selected feature layer
        /// </summary>
        private void FillGeolineType()
        {
            if (GeolineSelectedLayerIndex != -1 && GeolineType != null)
            {

                QueuedTask.Run(() =>
                {

                    _geolineTypeSelectedIndex = -1;
                    NotifyPropertyChanged(nameof(GeolineTypeSelectedIndex));
                    GeolineType.Clear();
                    NotifyPropertyChanged(nameof(GeolineType));

                    GeolineQualifierSelectedIndex = -1;
                    NotifyPropertyChanged(nameof(GeolineQualifierSelectedIndex));
                    GeolineQualifier.Clear();
                    NotifyPropertyChanged(nameof(GeolineQualifier));

                    GeolineConfidenceSelectedIndex = -1;
                    NotifyPropertyChanged(nameof(GeolineConfidenceSelectedIndex));
                    GeolineConfidence.Clear();
                    NotifyPropertyChanged(nameof(GeolineConfidence));

                    GeolineAttitudeSelectedIndex = -1;
                    NotifyPropertyChanged(nameof(GeolineAttitudeSelectedIndex));
                    GeolineAttitude.Clear();
                    NotifyPropertyChanged(nameof(GeolineAttitude));

                    GeolineGenerationSelectedIndex = -1;
                    NotifyPropertyChanged(nameof(GeolineGenerationSelectedIndex));
                    GeolineGeneration.Clear();
                    NotifyPropertyChanged(nameof(GeolineGeneration));

                    FeatureLayer lineLayer = GeolineLayers[GeolineSelectedLayerIndex].FLayer;
                    _uriGeodatabase = Workspace.GetWorkspacePathFromFeatureLayer(lineLayer);

                    if (_uriGeodatabase != null && _geolineType.Count() == 0 && Directory.Exists(_uriGeodatabase.OriginalString))
                    {

                        try
                        {
                            //Get origin database
                            using (Geodatabase sourceGeodatabase = new Geodatabase(new FileGeodatabaseConnectionPath(_uriGeodatabase)))
                            {
                                SortedList<string, string> typeDico = Utilities.Subtypes.GetSubtypeDicoFromWorkspace(sourceGeodatabase, lineLayer.GetFeatureClass().GetName());
                                if (typeDico != null)
                                {
                                    foreach (KeyValuePair<string, string> types in typeDico)
                                    {
                                        ComboBoxItem boxItem = new ComboBoxItem();
                                        boxItem.Text = types.Key;
                                        boxItem.Tooltip = types.Value;
                                        _geolineType.Add(boxItem);
                                        NotifyPropertyChanged(nameof(GeolineType));
                                    }
                                }
                                else
                                {
                                    new ErrorService(Properties.Resources.FormCreateEditGeolineNoSubtypes).WriteToFile();
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
                        new ErrorService(Properties.Resources.FormCreateEditGeolineNoSource).WriteToFile();
                    }

                });

            }
        }

        /// <summary>
        /// Will fill the geoline qualifier combobox based on selected feature layer
        /// </summary>
        private void FillGeolineQualifier()
        {
            if (GeolineTypeSelectedIndex != -1 && _uriGeodatabase != null)
            {

                QueuedTask.Run(() =>
                {
                    FillCombobox(Utilities.Constants.DatabaseFields.FGeolineQualif, _geolineQualifier, nameof(GeolineQualifier),
                        GeolineQualifierSelectedIndex, nameof(GeolineQualifierSelectedIndex));
                });
            }
        }

        /// <summary>
        /// Will fill the geoline confidence combobox based on selected feature layer
        /// </summary>
        private void FillConfidence()
        {
            if (GeolineTypeSelectedIndex != -1 && _uriGeodatabase != null)
            {

                QueuedTask.Run(() =>
                {
                    FillCombobox(Utilities.Constants.DatabaseFields.FGeolineConf, _geolineConfidence, nameof(GeolineConfidence),
                        GeolineConfidenceSelectedIndex, nameof(GeolineConfidenceSelectedIndex));
                });
            }
        }

        /// <summary>
        /// Will fill the geoline attitude combobox based on selected feature layer
        /// </summary>
        private void FillAttitude()
        {
            if (GeolineTypeSelectedIndex != -1 && _uriGeodatabase != null)
            {

                QueuedTask.Run(() =>
                {
                    FillCombobox(Utilities.Constants.DatabaseFields.FGeolineAtt, _geolineAttitude, nameof(GeolineAttitude),
                        GeolineAttitudeSelectedIndex, nameof(GeolineAttitudeSelectedIndex));
                });
            }
        }

        /// <summary>
        /// Will fill the geoline generation combobox based on selected feature layer
        /// </summary>
        private void FillGeneration()
        {
            if (GeolineTypeSelectedIndex != -1 && _uriGeodatabase != null)
            {

                QueuedTask.Run(() =>
                {
                    FillCombobox(Utilities.Constants.DatabaseFields.FGeolineAtt, _geolineGeneration, nameof(GeolineGeneration),
                        GeolineGenerationSelectedIndex, nameof(GeolineGenerationSelectedIndex));
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

                FeatureLayer lineLayer = GeolineLayers[GeolineSelectedLayerIndex].FLayer;

                //Get origin database
                using (Geodatabase sourceGeodatabase = new Geodatabase(new FileGeodatabaseConnectionPath(_uriGeodatabase)))
                {
                    //Clean
                    collectionIndex = -1;
                    NotifyPropertyChanged(nameof(collectionIndexPropertyName));
                    collection.Clear();
                    NotifyPropertyChanged(nameof(collectionPropertyName));

                    SortedList<object, string> qualifDico = Utilities.Domains.GetDomDicoFromSubtype(sourceGeodatabase,
                        lineLayer.GetFeatureClass().GetName(), GeolineType[GeolineTypeSelectedIndex].Tooltip, fieldName);
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
                        new ErrorService(Properties.Resources.FormCreateEditGeolineNoSubtypesDomains).WriteToFile();
                    }

                }

            }
            catch (Exception ex)
            {
                new ErrorService(ex).WriteToFile();
            }
        }

        /// <summary>
        /// Will add the selected geoline in the legend table and create the template for it.
        /// </summary>
        private void AddGeolineTemplate()
        {
            try
            {
                if (GeolineTypeSelectedIndex != -1 && GeolineQualifierSelectedIndex != -1 &&
                    GeolineConfidenceSelectedIndex != -1 && GeolineAttitudeSelectedIndex != -1 &&
                    GeolineGenerationSelectedIndex != -1)
                {
                    if (_uriGeodatabase != null)
                    {
                        QueuedTask.Run(() =>
                        {
                            using (Geodatabase sourceGeodatabase = new Geodatabase(new FileGeodatabaseConnectionPath(_uriGeodatabase)))
                            {
                                //Build geoline model (will be used for validation and template creation)
                                GeoLines _geoline = new GeoLines();
                                _geoline.GeolineID = string.Format("{0}{1}{2}{3}{4}",
                                    GeolineType[GeolineTypeSelectedIndex].Tooltip,
                                    GeolineQualifier[GeolineQualifierSelectedIndex].Tooltip,
                                    GeolineConfidence[GeolineConfidenceSelectedIndex].Tooltip,
                                    GeolineAttitude[GeolineAttitudeSelectedIndex].Tooltip,
                                    GeolineGeneration[GeolineGenerationSelectedIndex].Tooltip);
                                _geoline.GeolineType = int.Parse(GeolineType[GeolineTypeSelectedIndex].Tooltip);
                                _geoline.Qualifier = GeolineQualifier[GeolineQualifierSelectedIndex].Tooltip;
                                _geoline.Confidence = GeolineConfidence[GeolineConfidenceSelectedIndex].Tooltip;
                                _geoline.Attitude = GeolineAttitude[GeolineAttitudeSelectedIndex].Tooltip;
                                _geoline.Generation = GeolineGeneration[GeolineGenerationSelectedIndex].Tooltip;
                                _geoline.CreatorID = Properties.Settings.Default.SelectedParticipantCode;

                                //Set Creator field (by default first participant), else the new template won't work because Geoline 2.10 has CreatorID not nullable
                                if (_geoline.CreatorID == string.Empty)
                                {
                                    SortedList<object, string> firstPart = Domains.GetDomDicoFromWorkspace(sourceGeodatabase, Constants.DatabaseDomains.participant);
                                    if (firstPart != null)
                                    {
                                        _geoline.CreatorID = firstPart.Keys.First().ToString();
                                    }
                                }

                                //Validate if geoline exists within symbol tables
                                QueryFilter symbolTableFilter = new QueryFilter()
                                {
                                    WhereClause = string.Format("{0} = '{1}'", Constants.DatabaseFields.FGeolineID, _geoline.GeolineID)
                                };

                                using (Table symbolTable = sourceGeodatabase.OpenDataset<Table>(Utilities.Constants.Database.TGeolineSymbol))
                                {
                                    RowCursor symCursor = symbolTable.Search(symbolTableFilter);

                                    while (symCursor.MoveNext())
                                    {
                                        Row symRow = symCursor.Current;
                                        _geoline.GSCSymbol = symRow[Utilities.Constants.DatabaseFields.MGeolineFGDC].ToString();
                                        _geoline.Name = symRow[Utilities.Constants.DatabaseFields.MGeolineLegendDescription].ToString();
                                    }
                                }

                                if (_geoline.Name != null && _geoline.Name != string.Empty)
                                {
                                    using (Table legendTable = sourceGeodatabase.OpenDataset<Table>(Utilities.Constants.Database.TLegendGene))
                                    {
                                        bool geolineIDExists = false;


                                        //Query filter for geoline only
                                        QueryFilter geolineFilter = new QueryFilter()
                                        {
                                            WhereClause = string.Format("{0} = '{1}'", Utilities.Constants.DatabaseFields.LegendLabelID, _geoline.GeolineID)
                                        };

                                        RowCursor rowCursor = legendTable.Search(geolineFilter);
                                        while (rowCursor.MoveNext())
                                        {
                                            Row currentRow = rowCursor.Current;

                                            if (currentRow != null)
                                            {
                                                geolineIDExists = true;
                                                break;
                                            }
                                        }

                                        //Insert new record in legend if it's not already there
                                        if (!geolineIDExists)
                                        {
                                            //Prepare callback in case something happens
                                            EditOperation editOp = new EditOperation();
                                            editOp.Callback(async context =>
                                            {
                                                //Prepare a buffer to store information before insertion
                                                using (RowBuffer rowBuffer = legendTable.CreateRowBuffer())
                                                {
                                                    rowBuffer[Constants.DatabaseFields.LegendLabelID] = _geoline.GeolineID;
                                                    rowBuffer[Constants.DatabaseFields.LegendGISDisplay] = _geoline.Name;
                                                    rowBuffer[Constants.DatabaseFields.LegendSymbol] = _geoline.GSCSymbol;
                                                    rowBuffer[Constants.DatabaseFields.LegendItemType] = Constants.DatabaseDomainsValues.legendItemGeoline;

                                                    //Create row with the buffer
                                                    using (Row row = legendTable.CreateRow(rowBuffer))
                                                    {
                                                        context.Invalidate(row);
                                                    }
                                                }
                                            }, legendTable);

                                            editOp.Execute();

                                            //Create and or update template
                                            Symbols.CreateLineTemplate(GeolineLayers[GeolineSelectedLayerIndex].FLayer, _geoline);

                                            //Show notication success
                                            FrameworkApplication.AddNotification(new Notification()
                                            {
                                                Title = Properties.Resources.FormCreateEditGeolineTitle,
                                                Message = Properties.Resources.GenericMessageCompleted,
                                                ImageSource = System.Windows.Application.Current.Resources["Success_Toast48"] as ImageSource
                                            });

                                        }
                                        else
                                        {
                                            MessageBox.Show(Properties.Resources.FormCreateEditGeolineExists, Properties.Resources.GenericWarningTitle, System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Exclamation);
                                        }

                                    }
                                }
                                else 
                                {
                                    MessageBox.Show(Properties.Resources.FormCreateEditGeolineUndefined, Properties.Resources.GenericWarningTitle, System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
                                }
                            }
                        });

                    }
                }
                else
                {
                    MessageBox.Show(Properties.Resources.FormCreateEditGeolineMissingSelection, Properties.Resources.GenericWarningTitle, System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
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
    internal class Dock_CreateEdit_GeolineTemplate_ShowButton : Button
    {
        protected override void OnClick()
        {
            Dock_CreateEdit_GeolineTemplateViewModel.Show();
        }
    }
}
