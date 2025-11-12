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
using ArcGIS.Desktop.Framework.Threading.Tasks;
using ArcGIS.Desktop.KnowledgeGraph;
using ArcGIS.Desktop.Layouts;
using ArcGIS.Desktop.Mapping;
using BedrockEditorPro.Models;
using BedrockEditorPro.Utilities;
using Microsoft.VisualBasic;
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
using Constants = BedrockEditorPro.Utilities.Constants;

namespace BedrockEditorPro.DockPanes
{
    internal class Dock_CreateEdit_LabelTemplateViewModel : DockPane
    {
        #region INIT
        private const string _dockPaneID = "BedrockEditorPro_DockPanes_Dock_CreateEdit_LabelTemplate";
        private object _lock = new(); //For obs. collection
        private WorkingEnvironment workingEnvironment = new WorkingEnvironment();
        private Uri _uriGeodatabase = null; //Selected geoline layer uri
        private int _maxLabelID = 0; //Max label id found in domain
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
        private ObservableCollection<LayerDisplay> _labelLayers = new();
        public ObservableCollection<LayerDisplay> LabelLayers
        {
            get { return _labelLayers; }
        }
        private int _labelSelectedLayerIndex = -1;
        public int LabelSelectedLayerIndex
        {
            get { return _labelSelectedLayerIndex; }
            set
            {
                SetProperty(ref _labelSelectedLayerIndex, value, () => _labelSelectedLayerIndex);

                FillAgePrefixComboboxAsync();
            }
        }

        private ObservableCollection<ComboBoxItem> _labelAgePrefix = new();
        public ObservableCollection<ComboBoxItem> LabelAgePrefix
        {
            get { return _labelAgePrefix; }
        }
        private int _labelAgePrefixSelectedIndex = -1;
        public int LabelAgePrefixSelectedIndex
        {
            get { return _labelAgePrefixSelectedIndex; }
            set
            {
                SetProperty(ref _labelAgePrefixSelectedIndex, value, () => _labelAgePrefixSelectedIndex);
                _labelName = _labelAgePrefix[_labelAgePrefixSelectedIndex].Tooltip + _labelName;
                NotifyPropertyChanged(nameof(LabelName));
            }
        }

        private ObservableCollection<ComboBoxItem> _labelOverprintLevel = new();
        public ObservableCollection<ComboBoxItem> LabelOverprintLevel
        {
            get { return _labelOverprintLevel; }
        }
        private int _labelOverprintLevelSelectedIndex = -1;
        public int LabelOverprintLevelSelectedIndex
        {
            get { return _labelOverprintLevelSelectedIndex; }
            set
            {
                SetProperty(ref _labelOverprintLevelSelectedIndex, value, () => _labelOverprintLevelSelectedIndex);
            }
        }

        private string _labelName = string.Empty;
        public string LabelName
        {
            get { return _labelName; }
            set
            {
                SetProperty(ref _labelName, value, () => LabelName);
            }
        }

        private string _labelSymbol = string.Empty;
        public string LabelSymbol
        {
            get { return _labelSymbol; }
            set
            {
                SetProperty(ref _labelSymbol, value, () => LabelSymbol);
            }
        }

        #endregion

        #region RELAYS

        private ICommand _openSymbolBrowse = null;
        public ICommand OpenSymbolBrowse
        {
            get
            {
                if (_openSymbolBrowse == null)
                {
                    _openSymbolBrowse = new RelayCommand(() => OpenSymbolDialog(), () => true);
                }
                return _openSymbolBrowse;
            }
        }

        private ICommand _runTool = null;
        public ICommand RunTool
        {
            get
            {
                if (_runTool == null)
                {
                    _runTool = new RelayCommand(() => AddLabelTemplate(), () => true);
                }
                return _runTool;
            }
        }

        #endregion


        protected Dock_CreateEdit_LabelTemplateViewModel() 
        {
            //Subscribe to symbol dialog events
            Dialog.colorSymbolReferenceSelected += SelectedSymbolReferenceFromPrompt;
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
        /// Will add a new label template in the legend table and the editing templates
        /// To get a unique integer value for map unit label id, we will check the domain and the legend table for max value
        /// </summary>
        private void AddLabelTemplate()
        {
            if (_labelName != string.Empty)
            {
                try
                {
                    if (_uriGeodatabase != null)
                    {
                        QueuedTask.Run(async () =>
                        {
                            using (Geodatabase sourceGeodatabase = new Geodatabase(new FileGeodatabaseConnectionPath(_uriGeodatabase)))
                            {
                                //Get a label id from map unit domain
                                _maxLabelID = _maxLabelID + 1;
                                SortedList<object, string> muDico = Utilities.Domains.GetDomDicoFromWorkspace(sourceGeodatabase,
                                    Utilities.Constants.DatabaseDomains.MapUnit);
                                if (muDico != null && muDico.Count() > 0)
                                {
                                    foreach (KeyValuePair<object, string> kv in muDico)
                                    {
                                        int keyAsInt = 0;
                                        int.TryParse(kv.Key.ToString(), out keyAsInt);
                                        if (keyAsInt > _maxLabelID)
                                        {
                                            _maxLabelID = keyAsInt + 1; //Add one to possible max value
                                        }
                                    }

                                }

                                //Build label model (will be used for validation and template creation)
                                Labels _labels = new Labels();
                                _labels.LabelID = _maxLabelID.ToString();
                                _labels.GSCSymbol = _labelSymbol;
                                _labels.Name = _labelName;
                                _labels.CreatorID = Properties.Settings.Default.SelectedParticipantCode;
                                if (LabelOverprintLevelSelectedIndex != -1)
                                {
                                    _labels.OverprintLevel = int.Parse(LabelOverprintLevel[LabelOverprintLevelSelectedIndex].Tooltip);
                                }
                                else
                                {
                                    _labels.OverprintLevel = 0;
                                }


                                //Add value to legend table and domain
                                using (Table legendTable = sourceGeodatabase.OpenDataset<Table>(Utilities.Constants.Database.TLegendGene))
                                {
                                    bool labelNameExists = false;

                                    //Query filter for geopoint only
                                    QueryFilter labelFilter = new QueryFilter()
                                    {
                                        SubFields = string.Format("{0}, {1}", Utilities.Constants.DatabaseFields.LegendLabelID, Constants.DatabaseFields.LegendGISDisplay),
                                        WhereClause = string.Format("{0} = '{1}'", Utilities.Constants.DatabaseFields.LegendItemType, Constants.DatabaseDomainsValues.legendItemMapUnit),
                                    };

                                    RowCursor rowCursor = legendTable.Search(labelFilter);
                                    while (rowCursor.MoveNext())
                                    {
                                        Row currentRow = rowCursor.Current;
                                        int rowID = 0;
                                        int.TryParse(currentRow[Constants.DatabaseFields.LegendLabelID].ToString(), out rowID);
                                        if (rowID > _maxLabelID)
                                        {
                                            _maxLabelID = rowID + 1; //Add one to possible max value
                                        }

                                        //Same label already exists
                                        if (currentRow[Constants.DatabaseFields.LegendGISDisplay].ToString() == _labels.Name)
                                        {
                                            labelNameExists = true;
                                        }
                                    }

                                    //Add values to domain
                                    bool domainValueAdded = await Utilities.Domains.AddDomainValue(sourceGeodatabase, Constants.DatabaseDomains.MapUnit,
                                        _labels.LabelID.ToString(), _labels.Name);

                                    if (domainValueAdded && !labelNameExists)
                                    {
                                        //Prepare callback in case something happens
                                        EditOperation editOp = new EditOperation();
                                        editOp.Callback(async context =>
                                        {
                                            //Prepare a buffer to store information before insertion
                                            using (RowBuffer rowBuffer = legendTable.CreateRowBuffer())
                                            {
                                                rowBuffer[Constants.DatabaseFields.LegendLabelID] = _labels.LabelID;
                                                rowBuffer[Constants.DatabaseFields.LegendGISDisplay] = _labels.Name;
                                                rowBuffer[Constants.DatabaseFields.LegendSymbol] = _labels.GSCSymbol;
                                                rowBuffer[Constants.DatabaseFields.LegendItemType] = Constants.DatabaseDomainsValues.legendItemMapUnit;

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
                                        Symbols.CreateLabelTemplate(LabelLayers[LabelSelectedLayerIndex].FLayer, _labels);

                                        //Show notication success
                                        FrameworkApplication.AddNotification(new Notification()
                                        {
                                            Title = Properties.Resources.FormCreateEditLabelTitle,
                                            Message = Properties.Resources.GenericMessageCompleted,
                                            ImageSource = System.Windows.Application.Current.Resources["Success_Toast48"] as ImageSource
                                        });
                                    }
                                    else
                                    {
                                        MessageBox.Show(Properties.Resources.FormCreateEditLabelExists, Properties.Resources.GenericWarningTitle, System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Exclamation);
                                    }
                                }
                            }
                        });

                    }

                }
                catch (Exception ex)
                {
                    new ErrorService(ex).WriteToFile();
                }
            }
            else
            {
                MessageBox.Show(Properties.Resources.FormCreateEditLabelMissingSelection, Properties.Resources.GenericWarningTitle, System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
            }

        }

        /// <summary>
        /// Will open the symbol browse dialog to select a symbol for the label template
        /// </summary>
        public async void OpenSymbolDialog()
        {
            try
            {
                await QueuedTask.Run(() =>
                {
                    Dialog.GetSymbolPrompt();
                });
            }
            catch (Exception ex)
            {
                new ErrorService(ex).WriteToFile();
            }
        }

        /// <summary>
        /// Will fill the layer combobox with valid point layers from current map
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
                            _labelLayers.Clear();
                            foreach (FeatureLayer fl in layerEnum)
                            {

                                if (fl.ShapeType == esriGeometryType.esriGeometryPoint || fl.ShapeType == esriGeometryType.esriGeometryMultipoint)
                                {
                                    //Get some definition to valide field and move with getting first symbol
                                    CIMFeatureLayer cIMFeatureLayer = fl.GetDefinition() as CIMFeatureLayer;
                                    FeatureClass featureClass = fl.GetFeatureClass();

                                    if (cIMFeatureLayer != null && featureClass != null && featureClass.GetName().Contains(Utilities.Constants.Database.FLabel))
                                    {
                                        LayerDisplay layerItem = MakeComboBoxItemWithSymbolIcons(cIMFeatureLayer, fl);
                                        _labelLayers.Add(layerItem);
                                    }
                                }
                            }

                            if (_labelLayers.Count == 1)
                            {
                                LabelSelectedLayerIndex = 0;
                            }

                            NotifyPropertyChanged(nameof(LabelSelectedLayerIndex));
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
        /// Will fill the age prefix combobox
        /// </summary>
        public void FillAgePrefixComboboxAsync()
        {

            if (LabelSelectedLayerIndex != -1)
            {

                QueuedTask.Run(() =>
                {
                    FeatureLayer labelLayer = LabelLayers[LabelSelectedLayerIndex].FLayer;

                    _uriGeodatabase = Workspace.GetWorkspacePathFromFeatureLayer(labelLayer);

                    if (_uriGeodatabase != null && _labelAgePrefix.Count() == 0 && Directory.Exists(_uriGeodatabase.OriginalString))
                    {
                        //Get origin database
                        using (Geodatabase sourceGeodatabase = new Geodatabase(new FileGeodatabaseConnectionPath(_uriGeodatabase)))
                        {
                            //Clean
                            _labelAgePrefixSelectedIndex = -1;
                            NotifyPropertyChanged(nameof(LabelAgePrefixSelectedIndex));
                            _labelAgePrefix.Clear();
                            NotifyPropertyChanged(nameof(LabelAgePrefix));

                            SortedList<object, string> ageDico = Utilities.Domains.GetDomDicoFromWorkspace(sourceGeodatabase,
                                Utilities.Constants.DatabaseDomains.ageDesignator);
                            if (ageDico != null)
                            {
                                foreach (KeyValuePair<object, string> types in ageDico)
                                {
                                    ComboBoxItem boxItem = new ComboBoxItem();
                                    boxItem.Text = types.Value;
                                    boxItem.Tooltip = types.Key.ToString();
                                    _labelAgePrefix.Add(boxItem);
                                    NotifyPropertyChanged(nameof(LabelAgePrefix));
                                }

                                if (_labelAgePrefix.Count() == 1)
                                {
                                    _labelAgePrefixSelectedIndex = 0;
                                    NotifyPropertyChanged(nameof(LabelAgePrefixSelectedIndex));
                                }
                            }
                            else
                            {
                                new ErrorService(Properties.Resources.FormCreateEditLabelNoDomain).WriteToFile();
                            }
                        }
                    }
                });
            }
        }

        #endregion

        #region EVENTS

        /// <summary>
        /// Evend detect when user does select a spatial reference from the prompt dialog.
        /// Will update the textbox in the form.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="sr"></param>
        public void SelectedSymbolReferenceFromPrompt(object sender, string symbolName)
        {
            if (symbolName != null)
            {
                _labelSymbol = symbolName;
                NotifyPropertyChanged(nameof(LabelSymbol));
            }
        }

        protected override void OnShow(bool isVisible)
        {
            base.OnShow(isVisible);

            //Init as obs. collection the comboboxes
            BindingOperations.EnableCollectionSynchronization(_labelLayers, _lock);
            BindingOperations.EnableCollectionSynchronization(_labelAgePrefix, _lock);
            BindingOperations.EnableCollectionSynchronization(_labelOverprintLevel, _lock);

            //Init some components
            UpdateLayerComboboxAsync();


        }

        #endregion

    }

    /// <summary>
    /// Button implementation to show the DockPane.
    /// </summary>
    internal class Dock_CreateEdit_LabelTemplate_ShowButton : Button
    {
        protected override void OnClick()
        {
            Dock_CreateEdit_LabelTemplateViewModel.Show();
        }
    }


}
