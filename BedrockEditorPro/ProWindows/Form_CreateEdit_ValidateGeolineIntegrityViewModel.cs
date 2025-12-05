using ArcGIS.Core.CIM;
using ArcGIS.Core.Data;
using ArcGIS.Core.Data.Exceptions;
using ArcGIS.Core.Geometry;
using ArcGIS.Core.Internal.CIM;
using ArcGIS.Desktop.Core;
using ArcGIS.Desktop.Core.Geoprocessing;
using ArcGIS.Desktop.Editing;
using ArcGIS.Desktop.Framework;
using ArcGIS.Desktop.Framework.Contracts;
using ArcGIS.Desktop.Framework.Threading.Tasks;
using ArcGIS.Desktop.Mapping;
using BedrockEditorPro.Models;
using BedrockEditorPro.Services;
using BedrockEditorPro.Utilities;
using SQLitePCL;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using Field = ArcGIS.Core.Data.Field;
using Geometry = ArcGIS.Core.Geometry.Geometry;
using QueryFilter = ArcGIS.Core.Data.QueryFilter;
using Workspace = BedrockEditorPro.Utilities.Workspace;

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

        private bool _option1Checkbox = true;
        public bool Option1Checkbox
        {
            get { return _option1Checkbox; }
            set
            {
                SetProperty(ref _option1Checkbox, value, () => Option1Checkbox);
            }
        }

        private bool _option2Checkbox = true;
        public bool Option2Checkbox
        {
            get { return _option2Checkbox; }
            set
            {
                SetProperty(ref _option2Checkbox, value, () => Option2Checkbox);
            }
        }

        private bool _option3Checkbox = true;
        public bool Option3Checkbox
        {
            get { return _option3Checkbox; }
            set
            {
                SetProperty(ref _option3Checkbox, value, () => Option3Checkbox);
            }
        }

        private string _projectScale = "50000";
        public string ProjectScale
        {
            get { return _projectScale; }
            set
            {
                SetProperty(ref _projectScale, value, () => ProjectScale);
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
            bool validating = true;

            try
            {
                await QueuedTask.Run(async () =>
                {
                    FeatureLayer validateLayer = ValidateLayers[ValidateSelectedLayerIndex].FLayer;
                    Uri _validateLayerSourceUri = Workspace.GetWorkspacePath(validateLayer);

                    if (_validateSelectedLayerIndex != -1 && _validateLayerSourceUri != null && Directory.Exists(_validateLayerSourceUri.OriginalString))
                    {
                        WaitingCursorVisibility = Visibility.Visible;

                        //Get origin database
                        using (Geodatabase sourceGeodatabase = new Geodatabase(new FileGeodatabaseConnectionPath(_validateLayerSourceUri)))
                        {
                            //Get feature class
                            using (FeatureClass validateLineFC = sourceGeodatabase.OpenDataset<FeatureClass>(Constants.Database.FGeoline))
                            {
                                if (validateLineFC != null)
                                {
                                    //Get the definition class in order to get the shapefield 
                                    FeatureClassDefinition fcDefinition = validateLineFC.GetDefinition();

                                    //Process order
                                    List<Tuple<int, bool>> processOptions = new List<Tuple<int, bool>>()
                                    {
                                        new Tuple<int, bool>(1, _option1Checkbox),
                                        new Tuple<int, bool>(2, _option2Checkbox),
                                        new Tuple<int, bool>(3, _option3Checkbox)
                                    };
                                    string layerPath = validateLineFC.GetPath().ToString();

                                    foreach (Tuple<int, bool> options in processOptions)
                                    {
                                        if (options.Item2 && validating)
                                        {
                                            //Option 1 - remove null and empty geometries
                                            if (options.Item1 == 1)
                                            {
                                                //This process works directly in the input feature class, there is no output
                                                IGPResult repairResult = await Utilities.GeoprocessingBedrock.RepairGeometry(validateLayer);

                                                if (repairResult.IsFailed)
                                                {
                                                    validating = false;

                                                    break;
                                                }
                                            }

                                            //Option 2 - explode parts
                                            if (options.Item1 == 2)
                                            {
  
                                                Tuple<bool, bool> explodeResult = await MultipartToSingleParts(validateLineFC);

                                                if (!explodeResult.Item1)
                                                {
                                                    validating = false;

                                                    break;
                                                }

                                            }

                                            //Option 3 - densify and removes bezier curves
                                            if (options.Item1 == 3 && _projectScale != string.Empty)
                                            {
                                                int projectScaleInt = 0;
                                                int.TryParse(_projectScale, out projectScaleInt);

                                                if (projectScaleInt != 0)
                                                {
                                                    IGPResult densifyResult = await Utilities.GeoprocessingBedrock.Densify(validateLineFC, projectScaleInt);

                                                    if (densifyResult.IsFailed)
                                                    {
                                                        validating = false;

                                                        break;
                                                    }
                                                }

                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        FrameworkApplication.AddNotification(new Notification()
                        {
                            Title = Properties.Resources.FormCreateEditValidateGeolineTitle,
                            Message = Properties.Resources.FormEnvironmentNewGeodatabaseWarningDBExist,
                            ImageSource = System.Windows.Application.Current.Resources["Warning_Toast48"] as ImageSource
                        });

                        validating = false;
                    }
                });

            }
            catch (Exception e)
            {
                new ErrorService(e).WriteToFile();
                WaitingCursorVisibility = Visibility.Collapsed;
                _view.Close();
            }


            //Show notication success
            if (validating)
            {
                //Save edits
                await Project.Current.SaveEditsAsync();

                FrameworkApplication.AddNotification(new Notification()
                {
                    Title = Properties.Resources.FormCreateEditValidateGeolineTitle,
                    Message = Properties.Resources.GenericMessageCompleted,
                    ImageSource = System.Windows.Application.Current.Resources["Success_Toast48"] as ImageSource
                });
            }
            else
            {
                await Project.Current.DiscardEditsAsync();
            }

            //Close window
            WaitingCursorVisibility = Visibility.Collapsed;
            _view.Close();

        }

        #endregion

        /// <summary>
        /// Will explode a multipart feature layer into a single part one, without going
        /// through the geoprocessing, that outputs a new feature class. This will be conducted inside
        /// an edit session to keep result within the feature layer
        /// </summary>
        /// <param name="inLayer"></param>
        /// <returns></returns>
        private async Task<Tuple<bool, bool>> MultipartToSingleParts(FeatureClass inFeatureClass)
        { 
            bool completed = true;
            bool exploded = false;  

            //Exploding
            EditOperation editOp = new EditOperation();
            editOp.Callback(async context =>
            {
                //Iterate through all features
                using (RowCursor fcCursor = inFeatureClass.Search(null, false))
                {
                    while (fcCursor.MoveNext())
                    {
                        using (Row fcRow = fcCursor.Current)
                        {
                            //Get current geometry
                            Feature fcFeature = fcRow as Feature;
                            Geometry fcGeometry = fcFeature.GetShape();

                            //Get the definition class in order to get the shapefield and others if needed
                            FeatureClassDefinition fcDefinition = inFeatureClass.GetDefinition();
                            List<Field> fcFields = fcDefinition.GetFields().ToList();

                            //Get parts if any
                            List<Geometry> singleParts = GeometryMethods.MultipartToSinglePart(fcGeometry).ToList();

                            if (singleParts.Count() > 1)
                            {

                                //Convert all parts into new rows
                                foreach (Geometry part in singleParts)
                                {
                                    //Prepare a buffer to store new geometry
                                    using (RowBuffer rowBuffer = inFeatureClass.CreateRowBuffer())
                                    {
                                        //Update geometry from part
                                        rowBuffer[fcDefinition.GetShapeField()] = part;

                                        //Copy original field values in new row
                                        for (int fieldCount = 0; fieldCount < fcFields.Count; fieldCount++)
                                        {
                                            Field field = fcFields[fieldCount];

                                            if ((field.FieldType != FieldType.Geometry) &&
                                                (field.FieldType != FieldType.OID) && field.IsEditable)
                                            {
                                                rowBuffer[field.Name] = fcFeature.GetOriginalValue(fieldCount);
                                            }
                                        }

                                        //Create row with the buffer
                                        using (Feature feature = inFeatureClass.CreateRow(rowBuffer))
                                        {
                                            context.Invalidate(feature);
                                        }
                                    }
                                }

                                //Delete multipart feature
                                fcRow.Delete();
                                context.Invalidate(fcRow);

                                //Tag that at least something was exploded
                                if (!exploded)
                                {
                                    exploded = true;
                                }
                            }
                        }
                    }
                }

            }, inFeatureClass);

            //execute the new edit
            try
            {
                editOp.Execute();

            }
            catch (Exception e)
            {
                completed = false;
                new ErrorService(e).WriteToFile();
            }

            return new Tuple<bool, bool> (completed, exploded);

        }
    }
}
