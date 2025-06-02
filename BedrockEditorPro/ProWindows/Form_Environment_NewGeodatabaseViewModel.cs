using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
using ArcGIS.Desktop.Layouts;
using ArcGIS.Desktop.Mapping;
using ArcGIS.Desktop.KnowledgeGraph;
using System.Windows.Media;
using System.Windows.Data;
using System.Windows.Input;

namespace BedrockEditorPro.ProWindows
{
    public class Form_Environment_NewGeodatabaseViewModel : PropertyChangedBase
    {

        #region RELAYS

        private ICommand _openBrowseWindow = null;
        public ICommand OpenBrowseWindow
        {
            get
            {
                if (_openBrowseWindow == null)
                {
                    _openBrowseWindow = new RelayCommand(() =>
                    {
                        // Implement the logic to open a browse window here
                        MessageBox.Show("Browse button clicked!");
                    });
                }
                return _openBrowseWindow;
            }
        }

        #endregion


        public Form_Environment_NewGeodatabaseViewModel()
        {
            // Initialize any properties or commands here
            // For example, you can set default values or load data
        }
    }
}
