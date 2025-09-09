using ActiproSoftware.Windows.Extensions;
using ArcGIS.Core.CIM;
using ArcGIS.Core.Data;
using ArcGIS.Desktop.Core.Geoprocessing;
using ArcGIS.Desktop.Framework;
using ArcGIS.Desktop.Framework.Contracts;
using ArcGIS.Desktop.Framework.Threading.Tasks;
using ArcGIS.Desktop.Internal.Framework.Controls;
using ArcGIS.Desktop.Mapping;
using BedrockEditorPro.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;


namespace BedrockEditorPro.ProWindows
{
    public class Form_Load_StudyAreaViewModel: PropertyChangedBase
    {
        #region INIT
        private Dialog dialogs = new Dialog();
        private WorkingEnvironment workingEnvironment = new WorkingEnvironment();
        private ObservableCollection<LayerDisplay> _studyAreaLayers = new ();
        private ObservableCollection<StudyAreaPurposeDisplay> _studyAreaPurposes = new ();
        private Visibility _waitingCursorVisibility = Visibility.Collapsed;
        private Form_Load_StudyArea _view = null;
        private string _studyAreaName = string.Empty;
        private int _studyAreaSelectedLayerIndex = -1;
        private StudyAreaPurposeDisplay _studyAreaSelectedPurpose = null;
        private ObservableCollection<StudyAreaPurposeValueDisplay> _studyAreaPurposeValues = new();
        private int _studyAreaPurposeValueSelection = -1;
        private string _warningMessage = string.Empty;
        private object _lock = new(); //For obs. collection
        public enum StudyAreaPurposesEnum { Source, Project, Activity, Subactivity }

        #endregion

        #region CLASSES

        //For combobox display

        public class StudyAreaPurposeDisplay
        {
            public string Name { get; set; }
            public StudyAreaPurposesEnum Value { get; set; }
        }

        public class StudyAreaPurposeValueDisplay
        {
            public string Name { get; set; }
            public string Value { get; set; }
        }

        public class LayerDisplay
        { 
            public string Name { get; set; }
            public FeatureLayer FLayer { get; set; }
            public BitmapSource Icon { get; set; }
        }

        #endregion

        #region PROPERTIES

        public ObservableCollection<LayerDisplay> StudyAreaLayers
        {
            get { return _studyAreaLayers; }
        }

        public int StudyAreaSelectedLayerIndex
        {
            get { return _studyAreaSelectedLayerIndex; }
            set
            {
                SetProperty(ref _studyAreaSelectedLayerIndex, value, () => _studyAreaSelectedLayerIndex);
            }
        }

        public ObservableCollection<StudyAreaPurposeDisplay> StudyAreaPurposes
        {
            get { return _studyAreaPurposes; }
        }

        public StudyAreaPurposeDisplay StudyAreaSelectedPurpose
        {
            get { return _studyAreaSelectedPurpose; }
            set
            {
                SetProperty(ref _studyAreaSelectedPurpose, value, () => _studyAreaSelectedPurpose);

                //Will fill the purpose value combobox with values from associated table
                LoadPurposeValues();

            }
        }

        public ObservableCollection<StudyAreaPurposeValueDisplay> StudyAreaPurposeValues
        {
            get { return _studyAreaPurposeValues; }
        }

        public int StudyAreaPurposeValueSelection
        {
            get { return _studyAreaPurposeValueSelection; }
            set
            {
                SetProperty(ref _studyAreaPurposeValueSelection, value, () => _studyAreaPurposeValueSelection);
            }
        }

        public string WarningMessage
        {
            get { return _warningMessage; }
            set
            {
                SetProperty(ref _warningMessage, value, () => _warningMessage);
            }
        }


        public Visibility WaitingCursorVisibility
        {
            get { return _waitingCursorVisibility; }
            set
            {
                SetProperty(ref _waitingCursorVisibility, value, () => _waitingCursorVisibility);
            }
        }

        public string StudyAreaName
        {
            get { return _studyAreaName; }
            set
            {
                SetProperty(ref _studyAreaName, value, () => _studyAreaName);
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
                    _runTool = new RelayCommand(() => InsertStudyArea(), () => true);
                }
                return _runTool;
            }
        }

        #endregion

        #region METHODS

        public Form_Load_StudyAreaViewModel(Form_Load_StudyArea view)
        {
            //Init as obs. collection the comboboxes
            BindingOperations.EnableCollectionSynchronization(_studyAreaLayers, _lock);
            BindingOperations.EnableCollectionSynchronization(_studyAreaPurposes, _lock);
            BindingOperations.EnableCollectionSynchronization(_studyAreaPurposeValues, _lock);

            //Set related view
            _view = view;

            //Init some components
            UpdateLayerCombobox();
            UpdatePurposeCombobox();
            
        }

        /// <summary>
        /// Will fill the layer combobox with all feature layers in the map
        /// Optionall i will pre-select the study area layer if it exists
        /// </summary>
        public async void UpdateLayerCombobox()
        {

            try
            {
                await QueuedTask.Run(() =>
                {
                    List<FeatureLayer> layerEnum = MapView.Active.Map.GetLayersAsFlattenedList().OfType<FeatureLayer>().ToList();
                    if (layerEnum != null)
                    {
                        foreach (FeatureLayer fl in layerEnum)
                        {
                            //Layer layer = fl as Layer;
                            CIMFeatureLayer cIMFeatureLayer = fl.GetDefinition() as CIMFeatureLayer;
                            if (cIMFeatureLayer != null)
                            {
                                LayerDisplay layerItem = MakeComboBoxItemWithSymbolIcons(cIMFeatureLayer, fl);

                                _studyAreaLayers.Add(layerItem);

                                //Validate name for auto-selection
                                if (layerItem.Name == Constants.Database.FStudyAreaAlias)
                                {
                                    _studyAreaSelectedLayerIndex = layerEnum.IndexOf(fl);
                                    
                                }
                            }
                        }
                    }
                });

                NotifyPropertyChanged(nameof(StudyAreaSelectedLayerIndex));

            }
            catch (Exception ex)
            {
                new ErrorToLogFile(ex).WriteToFile();

            }

        }

        /// <summary>
        /// Will create a combobox item with a layer file type along a little bitmap image of its symbols
        /// </summary>
        /// <param name="cimFeatureLayer"></param>
        /// <returns></returns>
        public LayerDisplay MakeComboBoxItemWithSymbolIcons(CIMFeatureLayer cimFeatureLayer, FeatureLayer fl)
        {
            CIMSymbol sym = null;
            SymbolStyleItem si = null;
            BitmapSource bm = null;

            //Check for single renderer first
            CIMSimpleRenderer cimRenderer = cimFeatureLayer.Renderer as CIMSimpleRenderer;
            if (cimRenderer != null)
            {
                sym = cimRenderer.Symbol.Symbol;
            }
            else
            {
                //Get first symbol of first class instead
                CIMUniqueValueRenderer cimURenderer = cimFeatureLayer.Renderer as CIMUniqueValueRenderer;
                if (cimURenderer != null && cimURenderer.Groups.Count() > 0 && cimURenderer.Groups[0].Classes.Count() > 0 &&
                    cimURenderer.Groups[0].Classes[0].Symbol != null)
                {
                    sym = cimURenderer.Groups[0].Classes[0].Symbol.Symbol;
                }
            }

            //Create a bitmap image for the icon in the combobox, if a symbol was detected
            if (sym != null)
            {
                si = new SymbolStyleItem()
                {
                    Symbol = sym,
                    PatchHeight = 15,
                    PatchWidth = 15
                };
                bm = si.PreviewImage as BitmapSource;
                bm.Freeze();
            }

            //Create the combobox item
            LayerDisplay newLayerDisplay = new LayerDisplay
            {
                Name = fl.Name,
                FLayer = fl,
                Icon = bm
            };

            return newLayerDisplay;
        }

        /// <summary>
        /// Will fill the area types combobox with some preset values
        /// </summary>
        public void UpdatePurposeCombobox()
        {
            if (_studyAreaPurposes != null && _studyAreaPurposes.Count() == 0)
            {
                foreach (StudyAreaPurposesEnum value in Enum.GetValues(typeof(StudyAreaPurposesEnum)))
                {
                    _studyAreaPurposes.Add(new StudyAreaPurposeDisplay { Name = value.ToString(), Value = value });
                }
            }
        }

        /// <summary>
        /// Will fill the purpose value cbox from selected value above it
        /// </summary>
        public void LoadPurposeValues()
        {
            
            if (StudyAreaSelectedLayerIndex != -1 && StudyAreaSelectedPurpose != null)
            {

                QueuedTask.Run(() =>
                {
                    
                    _studyAreaPurposeValueSelection = -1;
                    NotifyPropertyChanged(nameof(StudyAreaPurposeValueSelection));
                    _studyAreaPurposeValues.Clear();
                    NotifyPropertyChanged(nameof(StudyAreaPurposeValues));

                    FeatureLayer areaLayer = StudyAreaLayers[StudyAreaSelectedLayerIndex].FLayer;
                    Uri areaLayerSourceUri = Workspace.GetWorkspacePathFromFeatureLayer(areaLayer);

                    if (areaLayerSourceUri != null && StudyAreaPurposeValues.Count() == 0 && Directory.Exists(areaLayerSourceUri.OriginalString))
                    {
                        _warningMessage = string.Empty;
                        NotifyPropertyChanged(nameof(WarningMessage));

                        try
                        {
                            //Get origin database
                            using (Geodatabase sourceGeodatabase = new Geodatabase(new FileGeodatabaseConnectionPath(areaLayerSourceUri)))
                            {
                                StudyAreaPurposesEnum sap = StudyAreaSelectedPurpose.Value;
                                Tuple<string, string, string> subFields = Tuple.Create(Constants.Database.TSource, Constants.DatabaseFields.TSourceAbbr,
                                                                            Constants.DatabaseFields.TSourceID);

                                switch (sap)
                                {
                                    case StudyAreaPurposesEnum.Activity:
                                        subFields = Tuple.Create(Constants.Database.TMActivity, Constants.DatabaseFields.MainActivityName,
                                                                            Constants.DatabaseFields.MainActID);
                                        break;
                                    case StudyAreaPurposesEnum.Project:
                                        subFields = Tuple.Create(Constants.Database.TProject, Constants.DatabaseFields.ProjectName,
                                                                            Constants.DatabaseFields.ProjectID);
                                        break;
                                    case StudyAreaPurposesEnum.Subactivity:
                                        subFields = Tuple.Create(Constants.Database.TSActivity, Constants.DatabaseFields.SubActivityName,
                                                                            Constants.DatabaseFields.SubActivityID);
                                        break;
                                    case StudyAreaPurposesEnum.Source:
                                        subFields = Tuple.Create(Constants.Database.TSource, Constants.DatabaseFields.TSourceAbbr,
                                                                            Constants.DatabaseFields.TSourceID);
                                        break;
                                    default:
                                        subFields = Tuple.Create(Constants.Database.TSource, Constants.DatabaseFields.TSourceAbbr,
                                                                            Constants.DatabaseFields.TSourceID);
                                        break;
                                }

                                using (Table purposeTable = sourceGeodatabase.OpenDataset<Table>(subFields.Item1))
                                {
                                    QueryFilter queryFilter = new QueryFilter
                                    {
                                        SubFields = string.Format("{0}, {1}", subFields.Item2, subFields.Item3)
                                    };

                                    using (RowCursor rc = purposeTable.Search(queryFilter, false))
                                    {
                                        while (rc.MoveNext())
                                        {
                                            using (Row row = rc.Current)
                                            {
                                                StudyAreaPurposeValueDisplay disp = new StudyAreaPurposeValueDisplay
                                                {
                                                    Name = row[subFields.Item2].ToString(),
                                                    Value = row[subFields.Item3].ToString(),
                                                };
                                                _studyAreaPurposeValues.Add(disp);
                                                NotifyPropertyChanged(nameof(StudyAreaPurposeValues));

                                            }
                                        }
                                    }
                                }
                            }

                        }
                        catch (Exception ex)
                        {
                            new ErrorToLogFile(ex).WriteToFile();
                            _warningMessage = ex.Message;
                            NotifyPropertyChanged(nameof(WarningMessage));
                        }

                    }
                    else
                    {
                        _warningMessage = Properties.Resources.GenericMessageErrorWrongDatabase;
                        NotifyPropertyChanged(nameof(WarningMessage));
                    }

                });

            }

        }

        /// <summary>
        /// Will insert a new record in P_STUDY_AREA table
        /// </summary>
        public void InsertStudyArea()
        {
            try
            {
                if (_studyAreaSelectedLayerIndex != -1 && _studyAreaSelectedPurpose != null && _studyAreaPurposeValueSelection != -1)
                {
                    WaitingCursorVisibility = Visibility.Visible;

                    _warningMessage = string.Empty;
                    NotifyPropertyChanged(nameof(WarningMessage));

                    //Close window
                    _view.Close();

                    //Show notication success
                    FrameworkApplication.AddNotification(new Notification()
                    {
                        Title = Properties.Resources.FormLoadStudyAreaTitle,
                        Message = Properties.Resources.GenericMessageCompleted,
                        ImageSource = System.Windows.Application.Current.Resources["Success_Toast48"] as ImageSource
                    });
                }
                else
                {
                    FrameworkApplication.AddNotification(new Notification()
                    {
                        Title = Properties.Resources.FormEnvironmentNewGeodatabaseTitle,
                        Message = Properties.Resources.FormEnvironmentNewGeodatabaseWarningDBExist,
                        ImageSource = System.Windows.Application.Current.Resources["Warning_Toast48"] as ImageSource
                    });
                }
            }
            catch (Exception e)
            {
                new ErrorToLogFile(e).WriteToFile();
                WaitingCursorVisibility = Visibility.Collapsed;
                _view.Close();
            }

        }
        #endregion
    }
}
