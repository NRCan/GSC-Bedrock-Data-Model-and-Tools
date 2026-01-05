using ArcGIS.Core.CIM;
using ArcGIS.Core.Data;
using ArcGIS.Core.Data.DDL;
using ArcGIS.Core.Geometry;
using ArcGIS.Desktop.Catalog;
using ArcGIS.Desktop.Core;
using ArcGIS.Desktop.Core.Geoprocessing;
using ArcGIS.Desktop.Editing;
using ArcGIS.Desktop.Extensions;
using ArcGIS.Desktop.Framework;
using ArcGIS.Desktop.Framework.Contracts;
using ArcGIS.Desktop.Framework.Dialogs;
using ArcGIS.Desktop.Framework.Threading.Tasks;
using ArcGIS.Desktop.KnowledgeGraph;
using ArcGIS.Desktop.Layouts;
using ArcGIS.Desktop.Mapping;
using BedrockEditorPro.Services;
using BedrockEditorPro.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace BedrockEditorPro.ProWindows
{
    public class Form_Environment_NewGeodatabaseViewModel : PropertyChangedBase
    {
        #region INIT
        private Dialog dialogs = new Dialog();
        private WorkingEnvironment workingEnvironment = new WorkingEnvironment();
        private string _xmlFilePath = string.Empty;
        private string _xmlSymbolsFilePath = string.Empty;
        private string _orgCSVFilePath = string.Empty;
        private string _geolineCSVFilePath = string.Empty;
        private string _geopointCSVFilePath = string.Empty;
        private string _jsonFilePath = string.Empty;
        private string _outputGDBPath = string.Empty;
        private string _outputSRName = string.Empty;
        private SpatialReference _outputSR = null;
        private Visibility _waitingCursorVisibility = Visibility.Collapsed;
        private bool _isXMLChecked = true;
        private bool _isControlEnabled = false;
        Form_Environment_NewGeodatabase _view = null;
        #endregion

        #region PROPERTIES

        public string OrgCSVFilePath
        {
            get { return _orgCSVFilePath; }
            set
            {
                SetProperty(ref _orgCSVFilePath, value, () => OrgCSVFilePath);
            }
        }

        public string GeolineCSVFilePath
        {
            get { return _geolineCSVFilePath; }
            set
            {
                SetProperty(ref _geolineCSVFilePath, value, () => GeolineCSVFilePath);
            }
        }

        public string GeopointCSVFilePath
        {
            get { return _geopointCSVFilePath; }
            set
            {
                SetProperty(ref _geopointCSVFilePath, value, () => GeopointCSVFilePath);
            }
        }

        public string XMLFilePath
        {
            get { return _xmlFilePath; }
            set
            {
                SetProperty(ref _xmlFilePath, value, () => XMLFilePath);
            }
        }

        public string JSONFilePath
        {
            get { return _jsonFilePath; }
            set
            {
                SetProperty(ref _jsonFilePath, value, () => JSONFilePath);
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

        public string OutputSRName
        {
            get { return _outputSRName; }
            set
            {
                SetProperty(ref _outputSRName, value, () => OutputSRName);
            }
        }

        public SpatialReference OutputSR
        {
            get { return _outputSR; }
            set
            {
                SetProperty(ref _outputSR, value, () => OutputSR);
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

        public bool IsXMLChecked
        {
            get { return _isXMLChecked; }
            set
            {
                SetProperty(ref _isXMLChecked, value, () => IsXMLChecked);
                // If XML is checked, disable the XML file path input
                if (value)
                {
                    XMLFilePath = string.Empty; // Clear the path if XML is checked
                    IsControlEnabled = false; // Disable controls
                }
                else
                {
                    IsControlEnabled = true; // Enable controls if XML is unchecked
                }
            }
        }

        public bool IsControlEnabled
        {
            get { return _isControlEnabled; }
            set
            {
                SetProperty(ref _isControlEnabled, value, () => IsControlEnabled);
            }
        }

        private bool _version210Checkbox = false;
        public bool Version210Checkbox
        {
            get { return _version210Checkbox; }
            set
            {
                SetProperty(ref _version210Checkbox, value, () => _version210Checkbox);
            }
        }

        private bool _version300Checkbox = true;
        public bool Version300Checkbox
        {
            get { return _version300Checkbox; }
            set
            {
                SetProperty(ref _version300Checkbox, value, () => _version300Checkbox);
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

        private ICommand _openProjectionBrowse = null;
        public ICommand OpenProjectionBrowse
        {
            get
            {
                if (_openProjectionBrowse == null)
                {
                    _openProjectionBrowse = new RelayCommand(ShowDialog, () => true);
                }
                return _openProjectionBrowse;
            }
        }

        private ICommand _runTool = null;
        public ICommand RunTool
        {
            get
            {
                if (_runTool == null)
                {
                    _runTool = new RelayCommand(() => CreateNewGeodatabase(), () => true);
                }
                return _runTool;
            }
        }
        #endregion

        #region METHODS
        public Form_Environment_NewGeodatabaseViewModel(Form_Environment_NewGeodatabase view)
        {
            Dialog.spatialReferenceSelected += SelectedSpatialReferenceFromPrompt;
            _view = view;
        }

        /// <summary>
        /// Will show the proper browsing dialog to user depending on which command they tapped.
        /// </summary>
        /// <param name="commandControl"></param>
        public void ShowDialog(object commandControl)
        {
            if (commandControl != null)
            {
                Controls.BrowseButton browseButton = commandControl as Controls.BrowseButton;

                //Make user select a folder to build the geodatabase
                if (browseButton != null && browseButton.Name.Contains("OutputGDB"))
                {
                    _outputGDBPath = dialogs.GetFGDBSavePrompt();
                    NotifyPropertyChanged(nameof(OutputGDBPath));
                }
            }
            else
            {
                dialogs.GetProjectionPrompt();
            }
        }

        /// <summary>
        /// Evend detect when user does select a spatial reference from the prompt dialog.
        /// Will update the textbox in the form.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="sr"></param>
        public void SelectedSpatialReferenceFromPrompt(object sender, SpatialReference sr)
        {
            if (sr != null)
            {
                _outputSR = sr;
                _outputSRName = sr.Name;
                NotifyPropertyChanged(nameof(OutputSR));
                NotifyPropertyChanged(nameof(OutputSRName));
            }
        }

        /// <summary>
        /// Will actually run the tool with all incoming information from the interface.
        /// It will create a new file geodatabase with the path and name selected by user, and
        /// with the internal projection set to user selection
        /// </summary>
        public async void CreateNewGeodatabase()
        {
            try
            {
                WaitingCursorVisibility = Visibility.Visible;

                string projectFolder = System.IO.Path.GetDirectoryName(OutputGDBPath);

                //Make sure the new database doesn't exist already
                bool dbExists = System.IO.Directory.Exists(OutputGDBPath);

                if (!dbExists)
                {
                    //Create the database from scratch and get the workspace object
                    using (Geodatabase newDBWorkspace = Utilities.Workspace.CreateWorkspace(OutputGDBPath))
                    {

                        //Manage the schema files
                        ManageSchemaResources();

                        //Manage the spatial reference see issue #5
                        int wkid = 3857; // Default to Web Mercator
                        if (OutputSR != null && OutputSR.Wkid > 0)
                        {
                            wkid = OutputSR.Wkid;
                        }
                        
                        string schemaString = File.ReadAllText(JSONFilePath);
                        string replacedSchemaString = schemaString.Replace("\"wkid\": null", $"\"wkid\": {wkid.ToString()}");
                        File.WriteAllText(JSONFilePath, replacedSchemaString);

                        //Convert schema from json to xml
                        string assetFileName = nameof(Properties.Resources.GSC_BEDROCKGDB_SCHEMA_V4_0);
                        if (_version210Checkbox)
                        {
                            assetFileName = nameof(Properties.Resources.GSC_BEDROCKGDB_SCHEMA_V2_10);
                        }

                        XMLFilePath = Path.Combine(workingEnvironment.WorkingEnvironmentPath, assetFileName + ".xml");
                        if (File.Exists(XMLFilePath))
                        {
                            File.Delete(XMLFilePath); // Delete existing XML file if it exists so it can take the latest spatial reference
                        }
                        await GeoprocessingBedrock.ConvertJSONToXML(JSONFilePath, workingEnvironment.WorkingEnvironmentPath, assetFileName + ".xml");

                        //Import the xml file into the new database
                        if (File.Exists(XMLFilePath))
                        {
                            IGPResult importResult = await Utilities.Workspace.ImportXMLWorkspace(OutputGDBPath, XMLFilePath);

                            if (!importResult.IsFailed)
                            {
                                //Fill tables that had values but have been wiped because of the JSON schema conversion
                                string symGeoline = Path.Combine(OutputGDBPath, Constants.Database.TGeolineSymbol);
                                string symGeopoint = Path.Combine(OutputGDBPath, Constants.Database.TGeopointSymbol);
                                string org = Path.Combine(OutputGDBPath, Constants.Database.TOrganisation);

                                await GeoprocessingBedrock.AppendInEmptyTables(OrgCSVFilePath, org);
                                await GeoprocessingBedrock.AppendInEmptyTables(GeolineCSVFilePath, symGeoline);
                                await GeoprocessingBedrock.AppendInEmptyTables(GeopointCSVFilePath, symGeopoint);

                                //Clean up
                                File.Delete(XMLFilePath);
                                File.Delete(JSONFilePath);
                                File.Delete(GeolineCSVFilePath);
                                File.Delete(GeopointCSVFilePath);
                                File.Delete(OrgCSVFilePath);
                            }

                        }
                    }

                    GC.Collect();



                    //Close window
                    _view.Close();

                    //Show notication success
                    FrameworkApplication.AddNotification(new Notification()
                    {
                        Title = Properties.Resources.FormEnvironmentNewGeodatabaseTitle,
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

                WaitingCursorVisibility = Visibility.Collapsed;

            }
            catch (Exception e)
            {
                new ErrorService(e).WriteToFile();
                WaitingCursorVisibility = Visibility.Collapsed;
                _view.Close();
            }

        }

        /// <summary>
        /// Will make sure to save the embeded schema resources to the working environment folder.
        /// </summary>
        public void ManageSchemaResources()
        {
            //Whole database is in json so feature datasets can have their spatial reference set see issue #5
            string jsonfFileName = nameof(Properties.Resources.GSC_BEDROCKGDB_SCHEMA_V4_0);
            byte[] jsonBytes = Properties.Resources.GSC_BEDROCKGDB_SCHEMA_V4_0;

            if (_version210Checkbox)
            {
                jsonfFileName = nameof(Properties.Resources.GSC_BEDROCKGDB_SCHEMA_V2_10);
                jsonBytes = Properties.Resources.GSC_BEDROCKGDB_SCHEMA_V2_10;
            }

            JSONFilePath = Path.Combine(workingEnvironment.WorkingEnvironmentPath, jsonfFileName + ".json");


            if (File.Exists(JSONFilePath))
            {
                //needs to be deleted to remove previously embeded projection in the text
                File.Delete(JSONFilePath);
            }
            FileService.WriteStreamResource(jsonBytes, JSONFilePath);

            //Geoline symbol table needs to be reloaded, the json schema can't hold filled in tables and since this one has relationship classes
            //to other table we can't simply add them from an XML workspace with only the table in it else we need to rename everything linked
            //tables and feature classes
            GeolineCSVFilePath = Path.Combine(workingEnvironment.WorkingEnvironmentPath, nameof(Properties.Resources.GSC_BEDROCKGDB_SYMBOL_GEOLINES_V2_10) + ".csv");
            if (!File.Exists(GeolineCSVFilePath))
            {
                FileService.WriteStreamResource(Properties.Resources.GSC_BEDROCKGDB_SYMBOL_GEOLINES_V2_10, GeolineCSVFilePath);
            }

            //Geopoint symbol table needs to be reloaded, the json schema can't hold filled in tables and since this one has relationship classes
            //to other table we can't simply add them from an XML workspace with only the table in it else we need to rename everything linked
            //tables and feature classes
            GeopointCSVFilePath = Path.Combine(workingEnvironment.WorkingEnvironmentPath, nameof(Properties.Resources.GSC_BEDROCKGDB_SYMBOL_GEOPOINTS_V2_10) + ".csv");
            if (!File.Exists(GeopointCSVFilePath))
            {
                FileService.WriteStreamResource(Properties.Resources.GSC_BEDROCKGDB_SYMBOL_GEOPOINTS_V2_10, GeopointCSVFilePath);
            }

            //Organization table needs to be reloaded, the json schema can't hold filled in tables and since this one has relationship classes
            //to other table we can't simply add them from an XML workspace with only the table in it else we need to rename everything linked
            //tables and feature classes
            OrgCSVFilePath = Path.Combine(workingEnvironment.WorkingEnvironmentPath, nameof(Properties.Resources.GSC_BEDROCKGDB_P_ORGANIZATION_V2_10) + ".csv");
            if (!File.Exists(OrgCSVFilePath))
            {
                FileService.WriteStreamResource(Properties.Resources.GSC_BEDROCKGDB_P_ORGANIZATION_V2_10, OrgCSVFilePath);
            }

        }



        #endregion
    }
}
