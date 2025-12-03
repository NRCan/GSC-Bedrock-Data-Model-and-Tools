using ArcGIS.Desktop.Framework;
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

namespace BedrockEditorPro.ProWindows
{
    public class Form_Legend_ItemsModificationViewModel: Layers
    {
        #region INIT

        private WorkingEnvironment workingEnvironment = new WorkingEnvironment();
        private Form_Legend_ItemsModification _view = null;
        private Visibility _waitingCursorVisibility = Visibility.Collapsed;
        private object _lock = new(); //For locking the threads to update obs. collection

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

        #endregion

        #region METHOD

        public Form_Legend_ItemsModificationViewModel(Form_Legend_ItemsModification view)
        {

            //Init as obs. collection the comboboxes
            BindingOperations.EnableCollectionSynchronization(_legendTables, _lock);

            //Set related view
            _view = view;

            //Init some components
            UpdateTableCombobox();

        }

        /// <summary>
        /// Will update the legend table combobox 
        /// </summary>
        public async void UpdateTableCombobox()
        {
            _legendTables.Clear();
            Layers layerService = new Layers();
            bool updated = await layerService.UpdateTableViewCombobox(_legendTables, nameof(LegendTables), _legendSelectedTableIndex, nameof(LegendSelectedTableIndex), Constants.DatabaseFields.LegendSymbol);

        }

        public async void UpdateLegendItem()
        { }

        #endregion

    }
}
