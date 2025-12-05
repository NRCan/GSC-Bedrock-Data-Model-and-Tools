using ArcGIS.Core.Data;
using ArcGIS.Desktop.Framework;
using ArcGIS.Desktop.Framework.Threading.Tasks;
using ArcGIS.Desktop.Mapping;
using BedrockEditorPro.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using static BedrockEditorPro.ProWindows.Form_Load_StudyAreaViewModel;

namespace BedrockEditorPro.ProWindows
{
    public class Form_Legend_ItemsModificationViewModel: Layers
    {
        #region INIT

        private WorkingEnvironment workingEnvironment = new WorkingEnvironment();
        private Form_Legend_ItemsModification _view = null;
        private Visibility _waitingCursorVisibility = Visibility.Collapsed;
        private object _lock = new(); //For locking the threads to update obs. collection
        private Uri _legendTableWorkspaceUri = null;
        public List<Button> listOfAllButtons = new List<Button>();
        private SolidColorBrush _selectedButtonBrush = new SolidColorBrush();

        #endregion

        #region PROPERTIES

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
                LoadLegendItems();
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

        #endregion

        #region RELAYS

        private ICommand _runTool = null;
        public ICommand RunTool
        {
            get
            {
                if (_runTool == null)
                {
                    _runTool = new RelayCommand(() => UpdateLegendItem(), () => true);
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

        #endregion

        #region METHOD

        public Form_Legend_ItemsModificationViewModel(Form_Legend_ItemsModification view)
        {

            //Init as obs. collection the comboboxes
            BindingOperations.EnableCollectionSynchronization(_legendTables, _lock);
            BindingOperations.EnableCollectionSynchronization(_legendItems, _lock);

            //Set related view
            _view = view;

            //Set default color for selected buttons
            _selectedButtonBrush.Color = Color.FromRgb(196, 220, 238);

            //Init some components
            UpdateTableCombobox();

        }

        /// <summary>
        /// Will update the legend table combobox with the available tables in the map
        /// </summary>
        public async void UpdateTableCombobox()
        {
            _legendTables.Clear();
            Layers layerService = new Layers();
            bool updated = await layerService.UpdateTableViewCombobox(_legendTables, nameof(LegendTables), _legendSelectedTableIndex, nameof(LegendSelectedTableIndex), Constants.DatabaseFields.LegendSymbol);

        }

        /// <summary>
        /// Will load the legend items from the selected legend table in the combobox
        /// </summary>
        public void LoadLegendItems()
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

                                    using (Table legendTable = legendSTable.GetTable())
                                    {
                                        if (legendTable != null)
                                        {
                                            QueryFilter itemFilter = new QueryFilter 
                                            { 
                                                PrefixClause = "DISTINCT",
                                                SubFields = $"{Constants.DatabaseFields.LegendGISDisplay}, {Constants.DatabaseFields.LegendLabelID}",
                                                WhereClause = string.Format("{0} IS NOT NULL AND {0} NOT LIKE ''", Constants.DatabaseFields.LegendGISDisplay)
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
                                                        _legendItems.Add(rowBox);
                                                    }
                                                }
                                            }
                                        }
                                    }
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

        public async void UpdateLegendItem()
        { }

        /// <summary>
        /// Will sync the color brush of all buttons to show
        /// only one being selected just like a radio button would
        /// </summary>
        /// <param name="incomingButton"></param>
        public async void SyncAllButtons(object incomingButton)
        { 
            Button inButton = incomingButton as Button;

            if (inButton != null && listOfAllButtons.Count() > 0)
            {
                foreach (Button bts in listOfAllButtons)
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

                    bts.Background = _legendButtonBackground;

                    NotifyPropertyChanged(nameof(LegendButtonBackground));
                }


            }
        }

        #endregion

    }
}
