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
using BedrockEditorPro.Utilities;

namespace BedrockEditorPro.ProWindows
{
    public class Form_Environment_NewGeodatabaseViewModel : PropertyChangedBase
    {
        #region INIT
        private Dialog dialogs = new Dialog();
        private string _xmlFilePath = string.Empty;
        private string _outputGDBPath = string.Empty;

        #endregion

        #region PROPERTIES

        public string XMLFilePath
        {
            get { return _xmlFilePath; }
            set
            {
                SetProperty(ref _xmlFilePath, value, () => XMLFilePath);
            }
        }

        public string OutputGDBPath
        {
            get { return _outputGDBPath; }
            set
            {
                SetProperty(ref _outputGDBPath, value, () => OutputGDBPath);
            }
        }

        #endregion

        #region RELAYS

        private ICommand _openBrowseWindow = null;
        public ICommand OpenBrowseWindow
        {
            get
            {
                if (_openBrowseWindow == null)
                {
                    _openBrowseWindow = new RelayCommand(ShowDialog, () => true);
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

        public void ShowDialog(object commandControl) 
        {
            if (commandControl != null)
            {
                Controls.BrowseButton browseButton = commandControl as Controls.BrowseButton;

                //Make user select a xml file to build the geodatabase
                if (browseButton.Name.Contains("XML"))
                {
                    XMLFilePath = dialogs.GetXMLFilePrompt();
                }

                //Make user select a folder to build the geodatabase
                if (browseButton.Name.Contains("OutputGDB"))
                {
                    OutputGDBPath = dialogs.GetFGDBSavePrompt();
                }
            }
        }
    }
}
