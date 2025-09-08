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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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
        private List<ComboBoxItem> _studyAreaLayers = new List<ComboBoxItem>();
        private Visibility _waitingCursorVisibility = Visibility.Collapsed;
        private Form_Load_StudyArea _view = null;
        private string _studyAreaName = "test";
        private int _studyAreaSelectedLayerIndex = 0;
        #endregion

        #region PROPERTIES

        public List<ComboBoxItem> StudyAreaLayers
        {
            get { return _studyAreaLayers; }
            set
            {
                SetProperty(ref _studyAreaLayers, value, () => _studyAreaLayers);
            }
        }

        public int StudyAreaSelectedLayerIndex
        {
            get { return _studyAreaSelectedLayerIndex; }
            set
            {
                SetProperty(ref _studyAreaSelectedLayerIndex, value, () => _studyAreaSelectedLayerIndex);
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

        //private ICommand _runTool = null;
        //public ICommand RunTool
        //{
        //    get
        //    {
        //        if (_runTool == null)
        //        {
        //            _runTool = new RelayCommand(() => LoadStudyArea(), () => true);
        //        }
        //        return _runTool;
        //    }
        //}

        #endregion

        #region METHODS

        public Form_Load_StudyAreaViewModel(Form_Load_StudyArea view)
        {
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
                    List<Layer> layerEnum = MapView.Active.Map.GetLayersAsFlattenedList().OfType<FeatureLayer>().ToList<Layer>();
                    if (layerEnum != null)
                    {
                        foreach (Layer l in layerEnum)
                        {
                            CIMFeatureLayer cIMFeatureLayer = l.GetDefinition() as CIMFeatureLayer;
                            if (cIMFeatureLayer != null)
                            {
                                ComboBoxItem layerItem = MakeComboBoxItemWithSymbolIcons(l.GetDefinition() as CIMFeatureLayer);
                                //_mapLayers.Add(layerItem);
                                _studyAreaLayers.Add(layerItem);

                                //Validate name for auto-selection
                                if (layerItem.Text == Constants.Database.FStudyAreaAlias)
                                {
                                    _studyAreaSelectedLayerIndex = layerEnum.IndexOf(l);
                                    
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
        ComboBoxItem MakeComboBoxItemWithSymbolIcons(CIMFeatureLayer cimFeatureLayer)
        {
            string toolTip = $@"Select this feature layer: {cimFeatureLayer.Name}";
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


            return new ComboBoxItem(cimFeatureLayer.Name, bm, toolTip);
        }

        #endregion
    }
}
