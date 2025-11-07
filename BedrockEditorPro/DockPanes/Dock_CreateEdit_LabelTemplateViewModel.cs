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
using ArcGIS.Desktop.KnowledgeGraph;
using ArcGIS.Desktop.Layouts;
using ArcGIS.Desktop.Mapping;
using BedrockEditorPro.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BedrockEditorPro.Utilities.Layers;

namespace BedrockEditorPro.DockPanes
{
    internal class Dock_CreateEdit_LabelTemplateViewModel : DockPane
    {
        #region INIT
        private const string _dockPaneID = "BedrockEditorPro_DockPanes_Dock_CreateEdit_LabelTemplate";
        private object _lock = new(); //For obs. collection
        private WorkingEnvironment workingEnvironment = new WorkingEnvironment();
        private Uri _uriGeodatabase = null; //Selected geoline layer uri
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
        private ObservableCollection<LayerDisplay> _labelLayers = new();
        public ObservableCollection<LayerDisplay> LabelLayers
        {
            get { return _labelLayers; }
        }
        private int _labelSelectedLayerIndex = -1;
        public int LabelSelectedLayerIndex
        {
            get { return _labelSelectedLayerIndex; }
            set
            {
                SetProperty(ref _labelSelectedLayerIndex, value, () => _labelSelectedLayerIndex);
            }
        }

        private ObservableCollection<ComboBoxItem> _labelAgePrefix = new();
        public ObservableCollection<ComboBoxItem> LabelAgePrefix
        {
            get { return _labelAgePrefix; }
        }
        private int _labelAgePrefixSelectedIndex = -1;
        public int LabelAgePrefixSelectedIndex
        {
            get { return _labelAgePrefixSelectedIndex; }
            set
            {
                SetProperty(ref _labelAgePrefixSelectedIndex, value, () => _labelAgePrefixSelectedIndex);
            }
        }

        private ObservableCollection<ComboBoxItem> _labelOverprintLevel = new();
        public ObservableCollection<ComboBoxItem> LabelOverprintLevel
        {
            get { return _labelOverprintLevel; }
        }
        private int _labelOverprintLevelSelectedIndex = -1;
        public int LabelOverprintLevelSelectedIndex
        {
            get { return _labelOverprintLevelSelectedIndex; }
            set
            {
                SetProperty(ref _labelOverprintLevelSelectedIndex, value, () => _labelOverprintLevelSelectedIndex);
            }
        }

        private string _labelName = string.Empty;
        public string LabelName
        {
            get { return _labelName; }
            set
            {
                SetProperty(ref _labelName, value, () => LabelName);
            }
        }

        private string _labelSymbol = string.Empty;
        public string LabelSymbol
        {
            get { return _labelSymbol; }
            set
            {
                SetProperty(ref _labelSymbol, value, () => LabelSymbol);
            }
        }

        #endregion

        #region RELAYS

        #endregion


        protected Dock_CreateEdit_LabelTemplateViewModel() { }

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

    }

    /// <summary>
    /// Button implementation to show the DockPane.
    /// </summary>
    internal class Dock_CreateEdit_LabelTemplate_ShowButton : Button
    {
        protected override void OnClick()
        {
            Dock_CreateEdit_LabelTemplateViewModel.Show();
        }
    }
}
