using ArcGIS.Core.CIM;
using ArcGIS.Core.Data;
using ArcGIS.Desktop.Core;
using ArcGIS.Desktop.Framework;
using ArcGIS.Desktop.Framework.Contracts;
using ArcGIS.Desktop.Framework.Threading.Tasks;
using ArcGIS.Desktop.Internal.Core;
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
using System.Windows.Media.Imaging;
using static BedrockEditorPro.ProWindows.Form_Load_StudyAreaViewModel;

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

        private string _warningMessage = string.Empty;
        public string WarningMessage
        {
            get { return _warningMessage; }
            set
            {
                SetProperty(ref _warningMessage, value, () => _warningMessage);
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

        #region mETHODS

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

            try
            {
                await QueuedTask.Run(() =>
                {
                    List<FeatureLayer> layerEnum = MapView.Active.Map.GetLayersAsFlattenedList().OfType<FeatureLayer>().ToList();
                    if (layerEnum != null)
                    {
                        foreach (FeatureLayer fl in layerEnum)
                        {
                            //System.Threading.Thread.Sleep(1);
                            if (fl.ShapeType == esriGeometryType.esriGeometryPolygon ||
                                fl.ShapeType == esriGeometryType.esriGeometryPolyline ||
                                fl.ShapeType == esriGeometryType.esriGeometryLine || 
                                fl.ShapeType == esriGeometryType.esriGeometryPoint)
                            {
                                //Get some definition to valide field and move with getting first symbol
                                CIMFeatureLayer cIMFeatureLayer = fl.GetDefinition() as CIMFeatureLayer;
                                List<FieldDescription> flDescriptions = fl.GetFieldDescriptions().ToList();

                                if (cIMFeatureLayer != null && flDescriptions != null && flDescriptions.Count() > 0)
                                {
                                    //Will need GSC_SYMBOL to work on
                                    bool symbolFieldDescription = flDescriptions.Exists(x => x.Name == Constants.DatabaseFields.LegendSymbol);

                                    if (symbolFieldDescription)
                                    {

                                        LayerDisplay layerItem = MakeComboBoxItemWithSymbolIcons(cIMFeatureLayer, fl);

                                        _refreshLayers.Add(layerItem);
                                    }

                                    
                                }
                            }
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
                    _warningMessage = string.Empty;
                    NotifyPropertyChanged(nameof(WarningMessage));

                    //Initiate reading of style file
                    UserConfiguration userConfig = await UserConfigurationService.GetUserConfigurationAsync();

                    await QueuedTask.Run(() =>
                    {
                        if (userConfig != null)
                        {

                            //Make sure style file is loaded in project, else add it
                            List<StyleProjectItem> styleItems = Project.Current.GetItems<StyleProjectItem>().Where(x => x.Path == userConfig.StyleFilePath).ToList();
                            if (styleItems == null || styleItems.Count() == 0)
                            {
                                Project.Current.AddStyle(userConfig.StyleFilePath);
                            }
                            StyleProjectItem workingStyle = Project.Current.GetItems<StyleProjectItem>().FirstOrDefault(x => x.Path == userConfig.StyleFilePath);

                            //Iterate through all selected layers
                            List<LayerDisplay> layersToRefresh = _refreshLayers.Where(x => x.IsChecked == true).ToList();
                            foreach (LayerDisplay l in layersToRefresh)
                            {
                                //Make sure the layer has the proper symbol field
                                List<FieldDescription> flDescriptions = l.FLayer.GetFieldDescriptions().ToList();

                                if (flDescriptions != null && flDescriptions.Count() > 0)
                                {
                                    //Will need GSC_SYMBOL to work on
                                    bool symbolFieldDescription = flDescriptions.Exists(x => x.Name == Constants.DatabaseFields.LegendSymbol);

                                    if (symbolFieldDescription)
                                    {
                                        //Prepare unique value renderer                
                                        UniqueValueRendererDefinition uniqueValueRenderer = new UniqueValueRendererDefinition()
                                        {
                                            ValueFields = new List<string> { Constants.DatabaseFields.LegendSymbol }, //multiple fields in the array if needed.
                                            ColorRamp = ColorFactory.Instance.GetColorRamp("Default"),
                                        };

                                        CIMRenderer renderer = l.FLayer.CreateRenderer(uniqueValueRenderer);

                                        //Sets the renderer to the feature layer
                                        l.FLayer.SetRenderer(renderer);

                                        //Get geometry type
                                        StyleItemType styleItemType = StyleItemType.Unknown;
                                        if (l.FLayer.ShapeType == esriGeometryType.esriGeometryPolygon)
                                        {
                                            styleItemType = StyleItemType.PolygonSymbol;
                                        }
                                        else if (l.FLayer.ShapeType == esriGeometryType.esriGeometryPoint)
                                        {
                                            styleItemType = StyleItemType.PointSymbol;
                                        }
                                        else if (l.FLayer.ShapeType == esriGeometryType.esriGeometryLine || l.FLayer.ShapeType == esriGeometryType.esriGeometryPolyline)
                                        {
                                            styleItemType = StyleItemType.LineSymbol;
                                        }

                                        if (l.FLayer.GetRenderer() is CIMUniqueValueRenderer cIMUniqueValueRenderer)
                                        {
                                            CIMUniqueValueRenderer cloneRenderer = cIMUniqueValueRenderer.Clone();
                                            foreach (CIMUniqueValueGroup cimVG in cloneRenderer.Groups)
                                            {
                                                foreach (CIMUniqueValueClass cimVC in cimVG.Classes)
                                                {
                                                    foreach (CIMUniqueValue cimV in cimVC.Values)
                                                    {
                                                        //Find symbol in style file
                                                        SymbolStyleItem currentSymbol = workingStyle.SearchSymbols(styleItemType, cimV.FieldValues[0].ToString())[0];

                                                        CIMSymbolReference cimSR = cimVC.Symbol;
                                                        cimSR.Symbol = currentSymbol.Symbol;

                                                        //cimVC.Label = "pouet";
                                                    }

                                                }
                                            }

                                            l.FLayer.SetRenderer(cloneRenderer);
                                        }

                                        //Prepare unique value renderer
                                        //CIMUniqueValueRenderer uniqueRenderer = new CIMUniqueValueRenderer
                                        //{
                                        //    Fields = new string[] { Constants.DatabaseFields.LegendSymbol }
                                        //};

                                        ////Prepare groups
                                        //CIMUniqueValueGroup mainGroup = new CIMUniqueValueGroup { Heading = "Legend Description" };
                                        //CIMUniqueValueGroup otherGroup = new CIMUniqueValueGroup { Heading = "Unmatch items" };

                                        //CIMBasicFeatureLayer lFeatureDef = l.FLayer.GetDefinition() as CIMBasicFeatureLayer;
                                        //CIMFeatureTable lFeatureTable = lFeatureDef.FeatureTable;

                                        ////Iterate through values and find their match in the style
                                        //using (Geodatabase sourceGeodatabase = new Geodatabase(new FileGeodatabaseConnectionPath(new Uri(lFeatureDef.SourceURI))))
                                        //{
                                        //    using (Table purposeTable = sourceGeodatabase.OpenDataset<Table>(lFeatureDef.Name))
                                        //    {
                                        //        QueryFilter queryFilter = new QueryFilter
                                        //        {
                                        //            SubFields = string.Format("{0}", Constants.DatabaseFields.LegendSymbol)
                                        //        };

                                        //        using (RowCursor rc = purposeTable.Search(queryFilter, false))
                                        //        {
                                        //            while (rc.MoveNext())
                                        //            {
                                        //                using (Row row = rc.Current)
                                        //                {


                                        //                }
                                        //            }
                                        //        }
                                        //    }
                                        //}
                                    }


                                }

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

                //Save edits
                //Project.Current.SaveEditsAsync();

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
        #endregion
    }
}
