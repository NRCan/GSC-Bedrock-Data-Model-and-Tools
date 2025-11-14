using ArcGIS.Core.CIM;
using ArcGIS.Desktop.Framework;
using ArcGIS.Desktop.Framework.Threading.Tasks;
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

namespace BedrockEditorPro.ProWindows
{
    public class Form_CreateEdit_ValidateGeolineIntegrityViewModel : Layers
    {
        #region INIT

        private WorkingEnvironment workingEnvironment = new WorkingEnvironment();
        private Form_CreateEdit_ValidateGeolineIntegrity _view = null;
        private Visibility _waitingCursorVisibility = Visibility.Collapsed;
        private object _lock = new(); //For locking the threads to update obs. collection

        #endregion

        #region PROPERTIES

        //Layer controls
        private ObservableCollection<LayerDisplay> _validateLayers = new();
        public ObservableCollection<LayerDisplay> ValidateLayers
        {
            get { return _validateLayers; }
        }
        private int _validateSelectedLayerIndex = -1;
        public int ValidateSelectedLayerIndex
        {
            get { return _validateSelectedLayerIndex; }
            set
            {
                SetProperty(ref _validateSelectedLayerIndex, value, () => _validateSelectedLayerIndex);
            }
        }

        public Visibility WaitingCursorVisibility
        {
            get { return _waitingCursorVisibility; }
            set
            {
                SetProperty(ref _waitingCursorVisibility, value, () => WaitingCursorVisibility);
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
                    _runTool = new RelayCommand(() => ValidateGeolines(), () => true);
                }
                return _runTool;
            }
        }

        #endregion

        #region METHODS

        public Form_CreateEdit_ValidateGeolineIntegrityViewModel(Form_CreateEdit_ValidateGeolineIntegrity view)
        {
            //Init as obs. collection the comboboxes
            BindingOperations.EnableCollectionSynchronization(_validateLayers, _lock);

            //Set related view
            _view = view;

            //Init some components
            UpdateLayerCombobox();

        }

        /// <summary>
        /// Will fill the layer combobox with all feature layers in the map
        /// Optionall i will pre-select the geoline layer if it exists
        /// </summary>
        public async void UpdateLayerCombobox()
        {
            //Init some components
            _validateLayers.Clear();
            List<esriGeometryType> geomTypes = new List<esriGeometryType>() { esriGeometryType.esriGeometryPolyline, esriGeometryType.esriGeometryLine };
            Layers layerService = new Layers();

            bool updated = await layerService.UpdateLayerCombobox(geomTypes, _validateLayers, nameof(ValidateLayers), _validateSelectedLayerIndex, nameof(ValidateSelectedLayerIndex));

            if (updated)
            {
                NotifyPropertyChanged(nameof(ValidateLayers));

                if (_validateLayers.Count() == 1)
                {
                    _validateSelectedLayerIndex = 0;
                    NotifyPropertyChanged(nameof(ValidateSelectedLayerIndex));
                }
            }

        }

        /// <summary>
        /// Will iterate through all checked layers and try to refresh their symbols based on the 
        /// custom/default style file used by the tools
        /// </summary>
        public async void ValidateGeolines()
        {

        }

        #endregion

    }
}
