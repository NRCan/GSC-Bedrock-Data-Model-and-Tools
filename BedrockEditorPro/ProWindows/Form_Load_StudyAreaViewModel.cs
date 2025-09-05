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
        private System.Windows.Controls.ComboBox _studyAreaLayers = new System.Windows.Controls.ComboBox();
        private Visibility _waitingCursorVisibility = Visibility.Collapsed;
        private Form_Load_StudyArea _view = null;

        #endregion

        #region PROPERTIES

        public System.Windows.Controls.ComboBox StudyAreaLayers
        {
            get { return _studyAreaLayers; }
            set
            {
                SetProperty(ref _studyAreaLayers, value, () => _studyAreaLayers);
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

        public void UpdateLayerCombobox()
        {
            try
            {
                IEnumerable<Layer> layerlist = MapView.Active.Map.GetLayersAsFlattenedList().OfType<FeatureLayer>();
                StudyAreaLayers.Items.Clear();

                QueuedTask.Run(() =>
                {
                    foreach (var layer in layerlist)
                    {
                        CIMFeatureLayer cIMFeatureLayer = layer.GetDefinition() as CIMFeatureLayer;
                        if (cIMFeatureLayer != null)
                        {
                            StudyAreaLayers.Items.Add(MakeComboBoxItem(layer.GetDefinition() as CIMFeatureLayer));
                        }
                    }
                });

                NotifyPropertyChanged(nameof(StudyAreaLayers));

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
        ComboBoxItem MakeComboBoxItem(CIMFeatureLayer cimFeatureLayer)
        {
            string toolTip = $@"Select this feature layer: {cimFeatureLayer.Name}";
            CIMSimpleRenderer cimRenderer = cimFeatureLayer.Renderer as CIMSimpleRenderer;
            if (cimRenderer == null)
            {
                return new ComboBoxItem(cimFeatureLayer.Name, null, toolTip);
            }
            SymbolStyleItem si = new SymbolStyleItem()
            {
                Symbol = cimRenderer.Symbol.Symbol,
                PatchHeight = 16,
                PatchWidth = 16
            };
            BitmapSource bm = si.PreviewImage as BitmapSource;
            bm.Freeze();
            return new ComboBoxItem(cimFeatureLayer.Name, bm, toolTip);
        }

        #endregion
    }
}
