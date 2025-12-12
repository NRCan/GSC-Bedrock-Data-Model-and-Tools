using ActiproSoftware.Windows.Data;
using ArcGIS.Core.Data;
using ArcGIS.Core.Data.Exceptions;
using ArcGIS.Desktop.Core;
using ArcGIS.Desktop.Editing;
using ArcGIS.Desktop.Framework;
using ArcGIS.Desktop.Framework.Threading.Tasks;
using ArcGIS.Desktop.Internal.Mapping.Events;
using ArcGIS.Desktop.Mapping;
using BedrockEditorPro.Comboboxes;
using BedrockEditorPro.Models;
using BedrockEditorPro.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using static BedrockEditorPro.ProWindows.Form_Load_StudyAreaViewModel;

namespace BedrockEditorPro.ProWindows
{
    public class Form_Legend_ItemsModificationViewModel: Layers
    {
        #region INIT

        private WorkingEnvironment workingEnvironment = new WorkingEnvironment();
        private Form_Legend_ItemsModification _view = null;
        private object _lock = new(); //For locking the threads to update obs. collection
        private Uri _legendTableWorkspaceUri = null;
        public List<Button> listOfAllButtons = new List<Button>();
        private SolidColorBrush _selectedButtonBrush = new SolidColorBrush();

        private enum StyleItems {symbol1, symbol2, label1, label2};
        private StyleItems _selectedStyleDialog = StyleItems.symbol1;

        #endregion

        #region PROPERTIES

        //Models
        private Models.PLegend _legend = new Models.PLegend();
        public Models.PLegend Legend
        {
            get { return _legend; }
            set
            {
                SetProperty(ref _legend, value, () => _legend);
            }
        }

        //Layer controls
        private ObservableCollection<LayerDisplay> _legendTables = new();
        public ObservableCollection<LayerDisplay> LegendTables
        {
            get { return _legendTables; }
        }
        private int _legendSelectedTableIndex = -1;
        public int LegendSelectedTableIndex
        {
            get { return _legendSelectedTableIndex; }
            set
            {
                SetProperty(ref _legendSelectedTableIndex, value, () => _legendSelectedTableIndex);

                //Fill legend items combobox with values coming from selected table
                FillComboboxes();
            }
        }

        //Legend items value controls
        private ObservableCollection<CustomCombobox> _legendItems = new();
        public ObservableCollection<CustomCombobox> LegendItems
        {
            get { return _legendItems; }
        }
        private int _legendSelectedItemIndex = -1;
        public int LegendSelectedItemIndex
        {
            get { return _legendSelectedItemIndex; }
            set
            {
                SetProperty(ref _legendSelectedItemIndex, value, () => _legendSelectedItemIndex);

                UpdateUI();
            }
        }

        //Legend items value controls
        private ObservableCollection<CustomCombobox> _geologicalRanks = new();
        public ObservableCollection<CustomCombobox> GeologicalRanks
        {
            get { return _geologicalRanks; }
        }
        private int _geologicalRanksSelectedIndex = -1;
        public int GeologicalRanksSelectedIndex
        {
            get { return _geologicalRanksSelectedIndex; }
            set
            {
                SetProperty(ref _geologicalRanksSelectedIndex, value, () => _geologicalRanksSelectedIndex);

                if (_geologicalRanksSelectedIndex != -1 && _legend != null)
                {
                    _legend.GeolRank = GeologicalRanks[_geologicalRanksSelectedIndex].Value;
                }
            }
        }

        //Legend items value controls
        private ObservableCollection<CustomCombobox> _overprintLevels = new();
        public ObservableCollection<CustomCombobox> OverprintLevels
        {
            get { return _overprintLevels; }
        }
        private int _overprintSelectedLevelIndex = -1;
        public int OverprintSelectedLevelIndex
        {
            get { return _overprintSelectedLevelIndex; }
            set
            {
                SetProperty(ref _overprintSelectedLevelIndex, value, () => _overprintSelectedLevelIndex);

                if (_overprintSelectedLevelIndex != -1 && _legend != null)
                {
                    _legend.Overprint = OverprintLevels[_overprintSelectedLevelIndex].Value;
                }
            }
        }

        //Other controls
        private string _warningMessage = string.Empty;
        public string WarningMessage
        {
            get { return _warningMessage; }
            set
            {
                SetProperty(ref _warningMessage, value, () => _warningMessage);
            }
        }

        private Brush _legendButtonBackground = Brushes.WhiteSmoke;
        public Brush LegendButtonBackground
        {
            get { return _legendButtonBackground; }
            set
            {
                SetProperty(ref _legendButtonBackground, value, () => _legendButtonBackground);
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

        private int _noOfColumns = 0;
        private string _noOfColumnsHint = string.Empty;
        public string NoOfColumnsHint
        {
            get { return _noOfColumnsHint; }
            set
            {
                SetProperty(ref _noOfColumnsHint, value, () => _noOfColumnsHint);
            }
        }

        private string _noOfOrderHint = string.Empty;
        public string NoOfOrderHint
        {
            get { return _noOfOrderHint; }
            set
            {
                SetProperty(ref _noOfOrderHint, value, () => _noOfOrderHint);
            }
        }

        private bool _isGeolineEnabled = false;
        public bool IsGeolineEnabled
        {
            get { return _isGeolineEnabled; }
            set
            {
                SetProperty(ref _isGeolineEnabled, value, () => _isGeolineEnabled);
            }
        }   

        private bool _isMapUnitEnabled = false;
        public bool IsMapUnitEnabled
        {
            get { return _isMapUnitEnabled; }
            set
            {
                SetProperty(ref _isMapUnitEnabled, value, () => _isMapUnitEnabled);
            }
        }

        private bool _isGeopointEnabled = false;
        public bool IsGeopointEnabled
        {
            get { return _isGeopointEnabled; }
            set
            {
                SetProperty(ref _isGeopointEnabled, value, () => _isGeopointEnabled);
            }
        }

        private bool _isOtherElementEnabled = true;
        public bool IsOtherElementEnabled
        {
            get { return _isOtherElementEnabled; }
            set
            {
                SetProperty(ref _isOtherElementEnabled, value, () => _isOtherElementEnabled);
            }
        }

        private BitmapImage _elementOverview = new BitmapImage();
        public BitmapImage ElementOverview
        {
            get { return _elementOverview; }
            set
            {
                SetProperty(ref _elementOverview, value, () => _elementOverview);
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
                    _runTool = new RelayCommand(() => UpdateLegendItemRecord(), () => true);
                }
                return _runTool;
            }
        }

        private ICommand _elementTypeButtonCommand = null;
        public ICommand ElementTypeButtonCommand
        {
            get
            {
                if (_elementTypeButtonCommand == null)
                {
                    _elementTypeButtonCommand = new RelayCommand((param) => SyncAllButtons(param), () => true);
                }
                return _elementTypeButtonCommand;
            }
        }


        private ICommand _openSymbolBrowse = null;
        public ICommand OpenSymbol1Browse
        {
            get
            {
                if (_openSymbolBrowse == null)
                {
                    _openSymbolBrowse = new RelayCommand(() => OpenSymbolDialog(StyleItems.symbol1), () => true);
                }
                return _openSymbolBrowse;
            }
        }

        private ICommand _openSymbol2Browse = null;
        public ICommand OpenSymbol2Browse
        {
            get
            {
                if (_openSymbol2Browse == null)
                {
                    _openSymbol2Browse = new RelayCommand(() => OpenSymbolDialog(StyleItems.symbol2), () => true);
                }
                return _openSymbol2Browse;
            }
        }

        private ICommand _openLabel1Browse = null;
        public ICommand OpenLabel1Browse
        {
            get
            {
                if (_openLabel1Browse == null)
                {
                    _openLabel1Browse = new RelayCommand(() => OpenSymbolDialog(StyleItems.label1), () => true);
                }
                return _openLabel1Browse;
            }
        }

        private ICommand _openLabel2Browse = null;
        public ICommand OpenLabel2Browse
        {
            get
            {
                if (_openLabel2Browse == null)
                {
                    _openLabel2Browse = new RelayCommand(() => OpenSymbolDialog(StyleItems.label2), () => true);
                }
                return _openLabel2Browse;
            }
        }
        #endregion

        #region METHOD

        public Form_Legend_ItemsModificationViewModel(Form_Legend_ItemsModification view)
        {

            //Init as obs. collection the comboboxes
            BindingOperations.EnableCollectionSynchronization(_legendTables, _lock);
            BindingOperations.EnableCollectionSynchronization(_legendItems, _lock);
            BindingOperations.EnableCollectionSynchronization(_geologicalRanks, _lock);
            BindingOperations.EnableCollectionSynchronization(_overprintLevels, _lock);

            //Subscribe to symbol dialog events
            Dialog.colorSymbolReferenceSelected += SelectedSymbolReferenceFromPrompt;

            //Set related view
            _view = view;

            //Set default color for selected buttons
            _selectedButtonBrush.Color = Color.FromRgb(196, 220, 238);

            //Init some components
            FillTableCombobox();


        }

        /// <summary>
        /// Will update the legend table combobox with the available tables in the map
        /// </summary>
        public async void FillTableCombobox()
        {
            _legendTables.Clear();
            Layers layerService = new Layers();
            bool updated = await layerService.UpdateTableViewCombobox(_legendTables, nameof(LegendTables), _legendSelectedTableIndex, nameof(LegendSelectedTableIndex), Constants.DatabaseFields.LegendSymbol);

        }

        /// <summary>
        /// Will load the legend items from the selected legend table in the combobox, along the geological ranks and the overprint levels
        /// </summary>
        public void FillComboboxes()
        {
            
            if (LegendSelectedTableIndex != -1 )
            {
                QueuedTask.Run(() =>
                {
                    //Reset
                    _legendSelectedItemIndex = -1;
                    NotifyPropertyChanged(nameof(LegendSelectedItemIndex));
                    _legendItems.Clear();
                    NotifyPropertyChanged(nameof(LegendItems));

                    StandaloneTable legendSTable = LegendTables[LegendSelectedTableIndex].STable;
                    if (legendSTable != null)
                    {
                        _legendTableWorkspaceUri = Workspace.GetWorkspacePath(legendSTable);

                        if (_legendTableWorkspaceUri != null && _legendItems.Count() == 0 && Directory.Exists(_legendTableWorkspaceUri.OriginalString))
                        {
                            //Clear warnings
                            _warningMessage = string.Empty;
                            NotifyPropertyChanged(nameof(WarningMessage));

                            try
                            {
                                //Access geodatabase and read legend items from table
                                using (Geodatabase sourceGeodatabase = new Geodatabase(new FileGeodatabaseConnectionPath(_legendTableWorkspaceUri)))
                                {
                                    //Legend table records 
                                    using (Table legendTable = legendSTable.GetTable())
                                    {
                                        if (legendTable != null)
                                        {
                                            
                                            QueryFilter itemFilter = new QueryFilter 
                                            { 
                                                PrefixClause = "DISTINCT",
                                                SubFields = $"{Constants.DatabaseFields.LegendGISDisplay}, {Constants.DatabaseFields.LegendLabelID}, " +
                                                $"{Constants.DatabaseFields.LegendColumn}, {Constants.DatabaseFields.LegendItemType}",
                                                WhereClause = string.Format("{0} IS NOT NULL AND {0} NOT LIKE ''", Constants.DatabaseFields.LegendGISDisplay),
                                                PostfixClause = $"ORDER BY {Constants.DatabaseFields.LegendOrder} ASC"
                                            };

                                            using (RowCursor legendCursor = legendTable.Search(itemFilter, false))
                                            {

                                                while (legendCursor.MoveNext())
                                                {
                                                    using (Row legendRow = legendCursor.Current)
                                                    {
                                                        CustomCombobox rowBox = new CustomCombobox();
                                                        rowBox.Name = legendRow[Constants.DatabaseFields.LegendGISDisplay].ToString();
                                                        rowBox.Value = legendRow[Constants.DatabaseFields.LegendLabelID].ToString();
                                                        rowBox.ExtraValue = legendRow[Constants.DatabaseFields.LegendItemType];

                                                        _legendItems.Add(rowBox);

                                                        int currentColumnNo = 0;
                                                        int.TryParse(legendRow[Constants.DatabaseFields.LegendColumn].ToString(), out currentColumnNo);

                                                        if (currentColumnNo > _noOfColumns)
                                                        {
                                                            _noOfColumns = currentColumnNo;
                                                        }
                                                    }
                                                }

                                                //Add an item for new elements to be added
                                                CustomCombobox newElementBox = new CustomCombobox();
                                                newElementBox.Name = Properties.Resources.FormLegendItemsNewElement;
                                                newElementBox.Value = Properties.Resources.FormLegendItemsNewElement;
                                                newElementBox.Value = null;
                                                _legendItems.Insert(0, newElementBox);
                                                _legendSelectedItemIndex = 0;
                                                NotifyPropertyChanged(nameof(LegendSelectedItemIndex));
                                            }

                                            //Update some UI strings
                                            _noOfOrderHint = string.Format(Properties.Resources.FormLegendItemsOrderHint, legendTable.GetCount().ToString());
                                            NotifyPropertyChanged(nameof(NoOfOrderHint));
                                            _noOfColumnsHint = string.Format(Properties.Resources.FormLegendItemsColumnHint, _noOfColumns.ToString());
                                            NotifyPropertyChanged(nameof(NoOfColumnsHint));
                                        }

                                    }

                                    //Geologiral rank
                                    SortedList<object, string> ranks = Utilities.Domains.GetDomDicoFromWorkspace(sourceGeodatabase, Constants.DatabaseDomains.geolRank);
                                    if (ranks!= null && ranks.Count() > 0)
                                    {
                                        foreach (KeyValuePair<object, string> rank in ranks)
                                        {
                                            CustomCombobox rowBox = new CustomCombobox();
                                            rowBox.Name = rank.Value;
                                            rowBox.Value = rank.Key.ToString();
                                            _geologicalRanks.Add(rowBox);
                                        }
                                    }

                                    NotifyPropertyChanged(nameof(GeologicalRanks));

                                    //Overprint levels
                                    SortedList<object, string> overprints = Utilities.Domains.GetDomDicoFromWorkspace(sourceGeodatabase, Constants.DatabaseDomains.legendOverprint);
                                    if (overprints != null && overprints.Count() > 0)
                                    {
                                        foreach (KeyValuePair<object, string> overprint in overprints)
                                        {
                                            CustomCombobox rowBox = new CustomCombobox();
                                            rowBox.Name = overprint.Value;
                                            rowBox.Value = overprint.Key.ToString();
                                            _overprintLevels.Add(rowBox);
                                        }
                                    }

                                    NotifyPropertyChanged(nameof(OverprintLevels));
                                }
                            }
                            catch (Exception ex)
                            {
                                new ErrorService(ex).WriteToFile();
                                _warningMessage = ex.Message;
                                NotifyPropertyChanged(nameof(WarningMessage));
                            }
                        }
                        else
                        {
                            _warningMessage = Properties.Resources.GenericMessageErrorWrongDatabase;
                            NotifyPropertyChanged(nameof(WarningMessage));
                        }

                    }
                });


            }

        }

        /// <summary>
        /// Will either update or insert a new item record in the legend table
        /// </summary>
        public async void UpdateLegendItemRecord()
        {
            try
            {
                if (_legendSelectedTableIndex != -1 && _legendSelectedItemIndex != -1 && Directory.Exists(_legendTableWorkspaceUri.OriginalString))
                {
                    WaitingCursorVisibility = Visibility.Visible;
                    _warningMessage = string.Empty;
                    NotifyPropertyChanged(nameof(WarningMessage));

                    await QueuedTask.Run(async () =>
                    {
                        //Get origin database
                        using (Geodatabase sourceGeodatabase = new Geodatabase(new FileGeodatabaseConnectionPath(_legendTableWorkspaceUri)))
                        {
                            StandaloneTable legendSTable = LegendTables[LegendSelectedTableIndex].STable;
                            if (legendSTable != null)
                            {
                                //Get table
                                using (Table legendTable = legendSTable.GetTable())
                                {

                                    //Prepare callback in case something happens
                                    EditOperation editOp = new EditOperation();
                                    editOp.Callback(async context =>
                                    {
                                        //INSERT NEW RECORD
                                        if (_legendSelectedItemIndex == 0)
                                        {
                                            //Prepare a buffer to store information before insertion
                                            using (RowBuffer rowBuffer = legendTable.CreateRowBuffer())
                                            {

                                                //Fill in the buffer with other field values
                                                foreach (KeyValuePair<string, object> kv in Legend.getModelReadyForInsert)
                                                {
                                                    if (kv.Key == Constants.DatabaseFields.LegendLabelID)
                                                    {
                                                        //Calculate new id
                                                        rowBuffer[kv.Key] = Guid.NewGuid().ToString();
                                                    }
                                                    else
                                                    {
                                                        rowBuffer[kv.Key] = kv.Value;
                                                    }
                                                        
                                                }

                                                //Create row with the buffer
                                                using (Row row = legendTable.CreateRow(rowBuffer))
                                                {
                                                    context.Invalidate(row);
                                                }
                                            }
                                        }

                                        //UPDATE RECORD
                                        if (_legendSelectedItemIndex > 0)
                                        {
                                            //Select the record to be updated
                                            QueryFilter updateFilter = new QueryFilter
                                            {
                                                WhereClause = string.Format("{0} = '{1}'", Constants.DatabaseFields.LegendLabelID, _legendItems[_legendSelectedItemIndex].Value)
                                            };

                                            using (RowCursor updateCursor = legendTable.Search(updateFilter, false))
                                            {
                                                while (updateCursor.MoveNext())
                                                {
                                                    using (Row updateRow = updateCursor.Current)
                                                    {
                                                        //Update fields
                                                        foreach (KeyValuePair<string, object> kv in Legend.getModelReadyForInsert)
                                                        {
                                                            updateRow[kv.Key] = kv.Value;
                                                        }
                                                        //Store changes
                                                        updateRow.Store();
                                                        context.Invalidate(updateRow);
                                                    }
                                                }

                                            }

                                        }

   
                                    }, legendTable);

                                    try
                                    {
                                        editOp.Execute();

                                        //Save edits
                                        await Project.Current.SaveEditsAsync();
                                    }
                                    catch (GeodatabaseException gdbEx)
                                    {
                                        new ErrorService(gdbEx).WriteToFile();
                                        WaitingCursorVisibility = Visibility.Collapsed;
                                        _view.Close();

                                        FrameworkApplication.AddNotification(new Notification()
                                        {
                                            Title = Properties.Resources.FormLegendItemsModificationTitle,
                                            Message = Properties.Resources.GenericMessageError,
                                            ImageSource = System.Windows.Application.Current.Resources["Warning_Toast48"] as ImageSource
                                        });
                                    }
                                }
                            }

                        }

                    });

                    //Close window
                    WaitingCursorVisibility = Visibility.Collapsed;
                    _view.Close();

                    //Save edits
                    Project.Current.SaveEditsAsync();

                    //Show notication success
                    FrameworkApplication.AddNotification(new Notification()
                    {
                        Title = Properties.Resources.FormLegendItemsModificationTitle,
                        Message = Properties.Resources.GenericMessageCompleted,
                        ImageSource = System.Windows.Application.Current.Resources["Success_Toast48"] as ImageSource
                    });
                }
                else
                {
                    FrameworkApplication.AddNotification(new Notification()
                    {
                        Title = Properties.Resources.FormLegendItemsModificationTitle,
                        Message = Properties.Resources.GenericMessageError,
                        ImageSource = System.Windows.Application.Current.Resources["Warning_Toast48"] as ImageSource
                    });
                }
            }
            catch (Exception e)
            {
                new ErrorService(e).WriteToFile();
                WaitingCursorVisibility = Visibility.Collapsed;
                _view.Close();
            }


        }

        /// <summary>
        /// Will sync the color brush of all buttons to show
        /// only one being selected just like a radio button would
        /// </summary>
        /// <param name="incomingButton"></param>
        public async void SyncAllButtons(object incomingButton, string buttonName = "")
        { 
            Button inButton = incomingButton as Button;

            if (listOfAllButtons.Count() > 0)
            {
                foreach (Button bts in listOfAllButtons)
                {
                    if (inButton != null)
                    {
                        if (bts == inButton)
                        {
                            //Reset all buttons background
                            _legendButtonBackground = _selectedButtonBrush;
                        }
                        else
                        {
                            //Reset all buttons background
                            _legendButtonBackground = Brushes.WhiteSmoke;
                        }
                    }

                    if (buttonName != string.Empty)
                    {
                        if (bts.Name == buttonName)
                        {
                            //Reset all buttons background
                            _legendButtonBackground = _selectedButtonBrush;
                        }
                        else
                        {
                            //Reset all buttons background
                            _legendButtonBackground = Brushes.WhiteSmoke;
                        }
                    }

                    bts.Background = _legendButtonBackground;
                    _legend.Element = bts.Name; //Keep name of the element, it's harcoded, I know it shouldn't be

                    NotifyPropertyChanged(nameof(LegendButtonBackground));
                }


            }
        }

        /// <summary>
        /// Will enable/disable group of elements based on legend item selection
        /// </summary>
        public void SyncElements()
        {
            if (_legendSelectedItemIndex != -1 && _legendItems.Count() > 0)
            {
                object elementType = _legendItems[_legendSelectedItemIndex].ExtraValue;

                if (elementType != null)
                {
                    string elementString = elementType as string;
                    if (elementString != string.Empty)
                    {
                        if (_legend.UnitElements.Contains(elementString))
                        {
                            _isMapUnitEnabled = true;
                            _isGeolineEnabled = false;
                            _isGeopointEnabled = false;
                            _isOtherElementEnabled = false;
                        }
                        else if (_legend.LineElements.Contains(elementString))
                        {
                            _isMapUnitEnabled = false;
                            _isGeolineEnabled = true;
                            _isGeopointEnabled = false;
                            _isOtherElementEnabled = false;

                        }
                        else if (_legend.MarkerElements.Contains(elementString))
                        {
                            _isMapUnitEnabled = false;
                            _isGeolineEnabled = false;
                            _isGeopointEnabled = true;
                            _isOtherElementEnabled = false;
                        }
                        else if (_legend.OtherElements.Contains(elementString))
                        {
                            _isMapUnitEnabled = false;
                            _isGeolineEnabled = false;
                            _isGeopointEnabled = false;
                            _isOtherElementEnabled = true;

                        }

                        SyncAllButtons(null, elementString);
                    }
                }
                else
                {
                    _isMapUnitEnabled = false;
                    _isGeolineEnabled = false;
                    _isGeopointEnabled = false;
                    _isOtherElementEnabled = true;

                }

                NotifyPropertyChanged(nameof(IsMapUnitEnabled));
                NotifyPropertyChanged(nameof(IsGeolineEnabled));
                NotifyPropertyChanged(nameof(IsGeopointEnabled));
                NotifyPropertyChanged(nameof(IsOtherElementEnabled));
                NotifyPropertyChanged(nameof(IsOtherElementEnabled));

                
            }
        }

        /// <summary>
        /// Will open the symbol browse dialog to select a symbol for the label template
        /// </summary>
        private async void OpenSymbolDialog(StyleItems styleItem)
        {
            try
            {
                await QueuedTask.Run(() =>
                {
                    _selectedStyleDialog = styleItem;
                    StyleItemType dialogTypeToOpen = StyleItemType.PolygonSymbol;
                    if (_isOtherElementEnabled)
                    {
                        dialogTypeToOpen = StyleItemType.TextSymbol;
                    }
                    else if (_isGeolineEnabled)
                    {
                        dialogTypeToOpen = StyleItemType.LineSymbol;
                    }
                    else if (_isGeopointEnabled)
                    {
                        dialogTypeToOpen = StyleItemType.PointSymbol;
                    }
                    else if (_isMapUnitEnabled)
                    {
                        dialogTypeToOpen = StyleItemType.PolygonSymbol;
                    }


                    switch (styleItem)
                    {
                        case StyleItems.symbol1:
                            Dialog.GetSymbolPrompt(dialogTypeToOpen);
                            break;
                        case StyleItems.symbol2:
                            Dialog.GetSymbolPrompt(dialogTypeToOpen);
                            break;
                        case StyleItems.label1:
                            Dialog.GetSymbolPrompt(StyleItemType.TextSymbol);
                            break;
                        case StyleItems.label2:
                            Dialog.GetSymbolPrompt(StyleItemType.TextSymbol);
                            break;
                        default:
                            break;
                    }
                    
                });
            }
            catch (Exception ex)
            {
                new ErrorService(ex).WriteToFile();
            }
        }

        /// <summary>
        /// Based on a selected legend record, will update the mode and all UI components
        /// </summary>
        public void UpdateUI()
        {
            if (_legendSelectedItemIndex != -1)
            {
                //Start by enabling/disabling some legend element buttons
                SyncElements();

                if (_legendSelectedItemIndex > 0 && _legendTableWorkspaceUri != null)
                {
                    //Update model
                    QueuedTask.Run(async () =>
                    {
                        //Get origin database
                        using (Geodatabase sourceGeodatabase = new Geodatabase(new FileGeodatabaseConnectionPath(_legendTableWorkspaceUri)))
                        {
                            StandaloneTable legendSTable = LegendTables[LegendSelectedTableIndex].STable;
                            if (legendSTable != null)
                            {
                                //Get table
                                using (Table legendTable = legendSTable.GetTable())
                                {
                                    //Select the record to be updated
                                    QueryFilter itemFilter = new QueryFilter
                                    {
                                        WhereClause = string.Format("{0} = '{1}'", Constants.DatabaseFields.LegendLabelID, _legendItems[_legendSelectedItemIndex].Value)
                                    };

                                    using (RowCursor itemCursor = legendTable.Search(itemFilter, false))
                                    {
                                        while (itemCursor.MoveNext())
                                        {
                                            using (Row item = itemCursor.Current)
                                            {
                                                _legend.ItemID = int.Parse(_legendItems[_legendSelectedItemIndex].Value);
                                                _legend.GISDisplay = Convert.ToString(item[Constants.DatabaseFields.LegendGISDisplay]);
                                                _legend.Element = Convert.ToString(item[Constants.DatabaseFields.LegendItemType]);
                                                _legend.Style1 = Convert.ToString(item[Constants.DatabaseFields.LegendSymbol]);
                                                _legend.Style2 = Convert.ToString(item[Constants.DatabaseFields.LegendSymbol2]);
                                                _legend.Label1 = Convert.ToString(item[Constants.DatabaseFields.LegendLabel1]);
                                                _legend.Label1Style = Convert.ToString(item[Constants.DatabaseFields.LegendLabel1Style]);
                                                _legend.Label2 = Convert.ToString(item[Constants.DatabaseFields.LegendLabel2]);
                                                _legend.Label2Style = Convert.ToString(item[Constants.DatabaseFields.LegendLabel2Style]);
                                                _legend.Heading = Convert.ToString(item[Constants.DatabaseFields.LegendHeading]);

                                                int columnNo = 0;
                                                int.TryParse(Convert.ToString(item[Constants.DatabaseFields.LegendColumn]), out columnNo);
                                                _legend.Column = columnNo;

                                                double orderNo = 0;
                                                double.TryParse(Convert.ToString(item[Constants.DatabaseFields.LegendOrder]), out orderNo);
                                                _legend.Order = orderNo;

                                                _legend.Description = Convert.ToString(item[Constants.DatabaseFields.LegendDescription]);
                                                _legend.GeolRank = Convert.ToString(item[Constants.DatabaseFields.LegendGeolRank]);
                                                _legend.Overprint = Convert.ToString(item[Constants.DatabaseFields.LegendOverprint]);
                                                NotifyPropertyChanged(nameof(Legend));

                                                if (_geologicalRanks.Where(r => r.Value == _legend.GeolRank).Count() == 1)
                                                {
                                                    _geologicalRanksSelectedIndex = _geologicalRanks.IndexOf(_geologicalRanks.Where(r => r.Value == _legend.GeolRank).First());
                                                    NotifyPropertyChanged(nameof(GeologicalRanksSelectedIndex));
                                                }

                                                if (OverprintLevels.Where(r => r.Value == _legend.Overprint).Count() == 1)
                                                {
                                                    _overprintSelectedLevelIndex = OverprintLevels.IndexOf(OverprintLevels.Where(r => r.Value == _legend.Overprint).First());
                                                    NotifyPropertyChanged(nameof(OverprintSelectedLevelIndex));
                                                }
                                            }
                                        }

                                    }
                                }
                            }

                        }

                    });


                    //Update UI based on model

                }
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
                switch (_selectedStyleDialog)
                {
                    case StyleItems.symbol1:
                        _legend.Style1 = symbolName;
                        NotifyPropertyChanged(nameof(Legend));
                        break;
                    case StyleItems.symbol2:
                        _legend.Style2 = symbolName;
                        NotifyPropertyChanged(nameof(Legend));
                        break;
                    case StyleItems.label1:
                        _legend.Label1Style = symbolName;
                        NotifyPropertyChanged(nameof(Legend));
                        break;
                    case StyleItems.label2:
                        _legend.Label2Style = symbolName;
                        NotifyPropertyChanged(nameof(Legend));
                        break;
                    default:
                        //Do nothing
                        break;
                }

            }
        }

        #endregion
    }
}
