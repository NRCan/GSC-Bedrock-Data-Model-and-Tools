using ActiproSoftware.Windows.Extensions;
using ArcGIS.Core.CIM;
using ArcGIS.Core.Data;
using ArcGIS.Core.Data.Exceptions;
using ArcGIS.Core.Geometry;
using ArcGIS.Desktop.Core;
using ArcGIS.Desktop.Core.Geoprocessing;
using ArcGIS.Desktop.Editing;
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

        private WorkingEnvironment workingEnvironment = new WorkingEnvironment();
        private Form_Load_StudyArea _view = null;
 
        private object _lock = new(); //For obs. collection
        public enum StudyAreaPurposesEnum { Source, Project, Activity, Subactivity }

        private Uri _areaLayerSourceUri = null;

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

        //Models
        private Models.PStudyArea _studyArea = new Models.PStudyArea();
        public Models.PStudyArea StudyArea
        {
            get { return _studyArea; }
            set
            {
                SetProperty(ref _studyArea, value, () => _studyArea);
            }
        }

        //Layer controls
        private ObservableCollection<LayerDisplay> _studyAreaLayers = new();
        public ObservableCollection<LayerDisplay> StudyAreaLayers
        {
            get { return _studyAreaLayers; }
        }
        private int _studyAreaSelectedLayerIndex = -1;
        public int StudyAreaSelectedLayerIndex
        {
            get { return _studyAreaSelectedLayerIndex; }
            set
            {
                SetProperty(ref _studyAreaSelectedLayerIndex, value, () => _studyAreaSelectedLayerIndex);
            }
        }

        //Purpose controls
        private ObservableCollection<StudyAreaPurposeDisplay> _studyAreaPurposes = new(); 
        public ObservableCollection<StudyAreaPurposeDisplay> StudyAreaPurposes
        {
            get { return _studyAreaPurposes; }
        }
        private StudyAreaPurposeDisplay _studyAreaSelectedPurpose = null;
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

        //Purpose value controls
        private ObservableCollection<StudyAreaPurposeValueDisplay> _studyAreaPurposeValues = new();
        public ObservableCollection<StudyAreaPurposeValueDisplay> StudyAreaPurposeValues
        {
            get { return _studyAreaPurposeValues; }
        }
        private StudyAreaPurposeValueDisplay _studyAreaSelectedPurposeValue = null;
        public StudyAreaPurposeValueDisplay StudyAreaSelectedPurposeValue
        {
            get { return _studyAreaSelectedPurposeValue; }
            set
            {
                SetProperty(ref _studyAreaSelectedPurposeValue, value, () => _studyAreaSelectedPurposeValue);
            }
        }
        private int _studyAreaSelectedPurposeValueIndex = -1;
        public int StudyAreaSelectedPurposeValueIndex
        {
            get { return _studyAreaSelectedPurposeValueIndex; }
            set
            {
                SetProperty(ref _studyAreaSelectedPurposeValueIndex, value, () => _studyAreaSelectedPurposeValueIndex);
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

        private Visibility _waitingCursorVisibility = Visibility.Collapsed;
        public Visibility WaitingCursorVisibility
        {
            get { return _waitingCursorVisibility; }
            set
            {
                SetProperty(ref _waitingCursorVisibility, value, () => _waitingCursorVisibility);
            }
        }

        //Metadata controls
        private string _studyAreaName = string.Empty;
        public string StudyAreaName
        {
            get { return _studyAreaName; }
            set
            {
                SetProperty(ref _studyAreaName, value, () => _studyAreaName);
            }
        }

        private string _studyAreaRemark = string.Empty;
        public string StudyAreaRemark
        {
            get { return _studyAreaRemark; }
            set
            {
                SetProperty(ref _studyAreaRemark, value, () => _studyAreaRemark);
            }
        }

        //Option 2 Layer controls
        private ObservableCollection<LayerDisplay> _studyAreaOption2Layers = new();
        public ObservableCollection<LayerDisplay> StudyAreaOption2Layers
        {
            get { return _studyAreaOption2Layers; }
        }
        private int _studyAreaSelectedOption2LayerIndex = -1;
        public int StudyAreaSelectedOption2LayerIndex
        {
            get { return _studyAreaSelectedOption2LayerIndex; }
            set
            {
                SetProperty(ref _studyAreaSelectedOption2LayerIndex, value, () => _studyAreaSelectedOption2LayerIndex);
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
            BindingOperations.EnableCollectionSynchronization(_studyAreaOption2Layers, _lock);

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
                            if (fl.ShapeType == esriGeometryType.esriGeometryPolygon || 
                                fl.ShapeType == esriGeometryType.esriGeometryPolyline || 
                                fl.ShapeType == esriGeometryType.esriGeometryLine)
                            {
                                //Layer layer = fl as Layer;
                                CIMFeatureLayer cIMFeatureLayer = fl.GetDefinition() as CIMFeatureLayer;

                                if (cIMFeatureLayer != null)
                                {
                                    LayerDisplay layerItem = MakeComboBoxItemWithSymbolIcons(cIMFeatureLayer, fl);

                                    //Special case for study area only
                                    if (fl.ShapeType == esriGeometryType.esriGeometryPolygon)
                                    {
                                        _studyAreaLayers.Add(layerItem);

                                        //Validate name for auto-selection
                                        if (layerItem.Name == Constants.Database.FStudyAreaAlias)
                                        {
                                            _studyAreaSelectedLayerIndex = _studyAreaLayers.Count() - 1;
                                        }
                                    }

                                    _studyAreaOption2Layers.Add(layerItem);

                                }
                            }
                        }
                    }
                });

                NotifyPropertyChanged(nameof(StudyAreaSelectedLayerIndex));

            }
            catch (Exception ex)
            {
                new ErrorService(ex).WriteToFile();

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

                    _studyAreaSelectedPurposeValueIndex = -1;
                    NotifyPropertyChanged(nameof(StudyAreaSelectedPurposeValueIndex));
                    _studyAreaPurposeValues.Clear();
                    NotifyPropertyChanged(nameof(StudyAreaPurposeValues));

                    FeatureLayer areaLayer = StudyAreaLayers[StudyAreaSelectedLayerIndex].FLayer;
                    _areaLayerSourceUri = Workspace.GetWorkspacePathFromFeatureLayer(areaLayer);

                    if (_areaLayerSourceUri != null && StudyAreaPurposeValues.Count() == 0 && Directory.Exists(_areaLayerSourceUri.OriginalString))
                    {
                        _warningMessage = string.Empty;
                        NotifyPropertyChanged(nameof(WarningMessage));

                        try
                        {
                            //Get origin database
                            using (Geodatabase sourceGeodatabase = new Geodatabase(new FileGeodatabaseConnectionPath(_areaLayerSourceUri)))
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
                                                //NotifyPropertyChanged(nameof(StudyAreaPurposeValues));

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
                if (_studyAreaSelectedLayerIndex != -1 && _studyAreaSelectedPurpose != null && _studyAreaSelectedPurposeValue != null &&
                    _areaLayerSourceUri != null && Directory.Exists(_areaLayerSourceUri.OriginalString))
                {
                    WaitingCursorVisibility = Visibility.Visible;
                    _warningMessage = string.Empty;
                    NotifyPropertyChanged(nameof(WarningMessage));

                    QueuedTask.Run( async () =>
                    {
                        //Get origin database
                        using (Geodatabase sourceGeodatabase = new Geodatabase(new FileGeodatabaseConnectionPath(_areaLayerSourceUri)))
                        {
                            //Get feature class
                            using (FeatureClass studyAreaFC = sourceGeodatabase.OpenDataset<FeatureClass>(Constants.Database.FStudyArea))
                            {
                                //Get the definition class in order to get the shapefield 
                                FeatureClassDefinition fcDefinition = studyAreaFC.GetDefinition();

                                //Prepare callback in case something happens
                                EditOperation editOp = new EditOperation();
                                editOp.Callback(async context =>
                                {
                                    //Prepare a buffer to store information before insertion
                                    using (RowBuffer rowBuffer = studyAreaFC.CreateRowBuffer())
                                    {
                                        bool validGeometry = false;

                                        //Set geometry from option 2 else create from option 1
                                        if (StudyAreaSelectedOption2LayerIndex != -1)
                                        {
                                            validGeometry = await ImportFeature();
                                            rowBuffer[fcDefinition.GetShapeField()] = StudyArea.Geometry;
                                        }
                                        else
                                        {
                                            //Set geometry
                                            IEnumerable<Coordinate3D> coord = StudyArea.getCoordinatesFromFields;
                                            rowBuffer[fcDefinition.GetShapeField()] = new PolygonBuilderEx(coord).ToGeometry();

                                            validGeometry = true;
                                        }

                                        if (!validGeometry)
                                        {
                                            throw new Exception(Properties.Resources.GenericMessageError);
                                        }

                                        //Fill in the buffer with other field values
                                        foreach (KeyValuePair<string, object> kv in StudyArea.getModelReadyForInsert)
                                        {
                                            rowBuffer[kv.Key] = kv.Value;

                                            if (kv.Key == Constants.DatabaseFields.FStudyAreaRelatedID)
                                            {
                                                rowBuffer[kv.Key] = _studyAreaSelectedPurposeValue.Value;
                                            }
                                        }

                                        //Create row with the buffer
                                        using (Feature feature = studyAreaFC.CreateRow(rowBuffer))
                                        {
                                            context.Invalidate(feature);
                                        }
                                    }
                                }, studyAreaFC);

                                try
                                {
                                    editOp.Execute();
                                }
                                catch (GeodatabaseException gdbEx )
                                {
                                    new ErrorService(gdbEx).WriteToFile();
                                    WaitingCursorVisibility = Visibility.Collapsed;
                                    _view.Close();

                                    FrameworkApplication.AddNotification(new Notification()
                                    {
                                        Title = Properties.Resources.FormLoadStudyAreaTitle,
                                        Message = Properties.Resources.GenericMessageError,
                                        ImageSource = System.Windows.Application.Current.Resources["Warning_Toast48"] as ImageSource
                                    });
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
                new ErrorService(e).WriteToFile();
                WaitingCursorVisibility = Visibility.Collapsed;
                _view.Close();
            }

        }

        /// <summary>
        /// Will import the feature from a selected layer
        /// </summary>
        /// <returns></returns>
        public async Task<bool> ImportFeature()
        {
            bool imported = false;

            try
            {
                if (StudyAreaSelectedOption2LayerIndex != -1)
                {
                    //Get selected feature layer
                    FeatureLayer flImport = StudyAreaOption2Layers[StudyAreaSelectedOption2LayerIndex].FLayer;

                    using (FeatureClass fc = flImport.GetFeatureClass())
                    {
                        //Get some definition (for shape and oid field)
                        FeatureClassDefinition fcDefinition = fc.GetDefinition();

                        //By default, will always process only the first selected object or the first polygon
                        QueryFilter qf = new QueryFilter()
                        {
                            PostfixClause = string.Format("ORDER BY {0} LIMIT 1", fcDefinition.GetObjectIDField())
                        };

                        //Get first selected object if there is any
                        if (flImport.SelectionCount > 0)
                        {
                            //We need to get all selected features from all layers
                            SelectionSet selectionSet = MapView.Active.Map.GetSelection();

                            //Find the needed layer and get the list of selected OIDs
                            List<long> option2LayerSelectionSet = selectionSet.ToDictionary().Where(l => l.Key.Name == flImport.Name).FirstOrDefault().Value;

                            if (option2LayerSelectionSet != null)
                            {
                                qf = new QueryFilter()
                                {
                                    WhereClause = string.Format("{0} = {1}", fcDefinition.GetObjectIDField(), option2LayerSelectionSet.First())
                                };
                            }
                        }

                        //Get geometry extent along the original geometry
                        using (RowCursor rowCursor = fc.Search(qf, false))
                        {
                            while (rowCursor.MoveNext())
                            {
                                using (Row row = rowCursor.Current)
                                {
                                    Feature feat = row as Feature;

                                    //Find geometry type
                                    if (flImport.ShapeType == esriGeometryType.esriGeometryPolygon)
                                    {
                                        Polygon polygon = feat.GetShape() as Polygon;

                                        StudyArea.West = polygon.Extent.XMax;
                                        StudyArea.East = polygon.Extent.XMin;
                                        StudyArea.North = polygon.Extent.YMax;
                                        StudyArea.South = polygon.Extent.YMin;
                                        StudyArea.Geometry = polygon;

                                    }
                                    else if (flImport.ShapeType == esriGeometryType.esriGeometryPolyline)
                                    {
                                        //If a line is selected, we'll need to convert it to a polygon
                                        Polyline polyline = feat.GetShape() as Polyline;

                                        IReadOnlyCollection<Coordinate3D> pointColl = polyline.Copy3DCoordinatesToList();

                                        Polygon polygon = new PolygonBuilderEx(pointColl).ToGeometry();

                                        StudyArea.West = polygon.Extent.XMax;
                                        StudyArea.East = polygon.Extent.XMin;
                                        StudyArea.North = polygon.Extent.YMax;
                                        StudyArea.South = polygon.Extent.YMin;
                                        StudyArea.Geometry = polygon;

                                    }

                                    imported = true;

                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                new ErrorService(ex).WriteToFile();
            }

            return imported;
        }

        #endregion
    }
}
