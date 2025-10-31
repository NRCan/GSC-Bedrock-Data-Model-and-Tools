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
using ArcGIS.Desktop.Internal.Mapping;
using ArcGIS.Desktop.KnowledgeGraph;
using ArcGIS.Desktop.Layouts;
using ArcGIS.Desktop.Mapping;
using BedrockEditorPro.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using static BedrockEditorPro.Utilities.Layers;

namespace BedrockEditorPro.DockPanes
{
    internal class Dock_CreateEdit_GeolineTemplateViewModel : DockPane
    {
        #region INIT
        private const string _dockPaneID = "BedrockEditorPro_DockPanes_Dock_CreateEdit_GeolineTemplate";
        private object _lock = new(); //For obs. collection
        private WorkingEnvironment workingEnvironment = new WorkingEnvironment();

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
        #endregion

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
        #endregion

        protected override void OnShow(bool isVisible)
        {
            base.OnShow(isVisible);

            //Init as obs. collection the comboboxes
            BindingOperations.EnableCollectionSynchronization(_geolineLayers, _lock);

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
