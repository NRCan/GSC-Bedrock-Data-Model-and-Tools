using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using System.Threading;
using ESRI.ArcGIS.Editor;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geoprocessing;
using ESRI.ArcGIS.GeoprocessingUI;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.ArcMap;

namespace GSC_ProjectEditor
{
    /// <summary>
    /// Designer class of the dockable window add-in. It contains user interfaces that
    /// make up the dockable window.
    /// </summary>
    public partial class Dockablewindow_CreateEdit_QualityControl : UserControl
    {

        #region Main Variables

        //GEO_LINES feature and fields
        private const string geolineFeature = GSC_ProjectEditor.Constants.Database.FGeoline;
        private const string geolineIsBoundaryField = GSC_ProjectEditor.Constants.DatabaseFields.FGeolineBoundary;
        private const string geolineConfidenceField = GSC_ProjectEditor.Constants.DatabaseFields.FGeolineConf;
        private const string geolineType = GSC_ProjectEditor.Constants.DatabaseFields.FGeolineSubtype;
        private const string geolineQualif = GSC_ProjectEditor.Constants.DatabaseFields.FGeolineQualif;
        private const string geolineIDField = GSC_ProjectEditor.Constants.DatabaseFields.FGeolineID;
        private const string geolineMovement = GSC_ProjectEditor.Constants.DatabaseFields.FGeolineMovement;
        private const string geolineHangwall = GSC_ProjectEditor.Constants.DatabaseFields.FGeolineHangwall;
        private const string geolineLayerName = GSC_ProjectEditor.Constants.Layers.geoline;

        //GEO_LINES dom values
        private const string geolineIsBoundaryValueYes = GSC_ProjectEditor.Constants.DatabaseDomainsValues.BoundYes;
        private const string geolineIsBoundaryValueNo = GSC_ProjectEditor.Constants.DatabaseDomainsValues.BoundNo;
        private const string geolineConfidenceDefined = GSC_ProjectEditor.Constants.DatabaseDomainsValues.ConfDef;
        private const string geolineConfidenceApprox = GSC_ProjectEditor.Constants.DatabaseDomainsValues.ConfAprox;
        private const string geolineConfidenceInf = GSC_ProjectEditor.Constants.DatabaseDomainsValues.ConfInf;
        private const string geolineConfidenceConcealed = GSC_ProjectEditor.Constants.DatabaseDomainsValues.ConfConcealed;
        private const string geolineConfidenceNotApp = GSC_ProjectEditor.Constants.DatabaseDomainsValues.ConfNotApp;
        private const string geolineContact = GSC_ProjectEditor.Constants.DatabaseDomainsValues.GeolineContacts;
        private const string geolineQualifDefUnconfor = GSC_ProjectEditor.Constants.DatabaseDomainsValues.geolineDefineUnconformable;
        private const string geolineFault = GSC_ProjectEditor.Constants.DatabaseDomainsValues.GeolineFaults;
        private const string geolineFaultNormal = GSC_ProjectEditor.Constants.DatabaseDomainsValues.geolineFaultNormal;
        private const string geolineMovementN = GSC_ProjectEditor.Constants.DatabaseDomainsValues.geolineMovementN;
        private const string geolineMovementE = GSC_ProjectEditor.Constants.DatabaseDomainsValues.geolineMovementE;
        private const string geolineMovementNA = GSC_ProjectEditor.Constants.DatabaseDomainsValues.geolineMovementNA;
        private const string geolineMovementNE = GSC_ProjectEditor.Constants.DatabaseDomainsValues.geolineMovementNE;
        private const string geolineMovementNW = GSC_ProjectEditor.Constants.DatabaseDomainsValues.geolineMovementNW;
        private const string geolineMovementS = GSC_ProjectEditor.Constants.DatabaseDomainsValues.geolineMovementS;
        private const string geolineMovementSE = GSC_ProjectEditor.Constants.DatabaseDomainsValues.geolineMovementSE;
        private const string geolineMovementSW = GSC_ProjectEditor.Constants.DatabaseDomainsValues.geolineMovementSW;
        private const string geolineMovementUndef = GSC_ProjectEditor.Constants.DatabaseDomainsValues.geolineMovementUndef;
        private const string geolineMovementW = GSC_ProjectEditor.Constants.DatabaseDomainsValues.geolineMovementW;
        private const string geolineHgwallN = GSC_ProjectEditor.Constants.DatabaseDomainsValues.geolineHgwallN;
        private const string geolineHgwallE = GSC_ProjectEditor.Constants.DatabaseDomainsValues.geolineHgwallE;
        private const string geolineHgwallNA = GSC_ProjectEditor.Constants.DatabaseDomainsValues.geolineHgwallNA;
        private const string geolineHgwallNE = GSC_ProjectEditor.Constants.DatabaseDomainsValues.geolineHgwallNE;
        private const string geolineHgwallNW = GSC_ProjectEditor.Constants.DatabaseDomainsValues.geolineHgwallNW;
        private const string geolineHgwallS = GSC_ProjectEditor.Constants.DatabaseDomainsValues.geolineHgwallS;
        private const string geolineHgwallSE = GSC_ProjectEditor.Constants.DatabaseDomainsValues.geolineHgwallSE;
        private const string geolineHgwallSW = GSC_ProjectEditor.Constants.DatabaseDomainsValues.geolineHgwallSW;
        private const string geolineHgwallUndef = GSC_ProjectEditor.Constants.DatabaseDomainsValues.geolineHgwallUndef;
        private const string geolineHgwallW = GSC_ProjectEditor.Constants.DatabaseDomainsValues.geolineHgwallW;

        //GEOL_LINES domaines
        private const string geolineMovementDom = GSC_ProjectEditor.Constants.DatabaseDomains.geolMovement;


        //GEO_LINES validation layers names
        private const string geolineBlueLayerName = GSC_ProjectEditor.Constants.Layers.GeolineLayerQC_BlueIsBoundary;
        private const string geolinePinkLayerName = GSC_ProjectEditor.Constants.Layers.GeolineLayerQC_PinkIsBoundary;
        private const string geolineConfDefName = GSC_ProjectEditor.Constants.Layers.GeolineLayerQC_DefinedConfidence;
        private const string geolineAproxName = GSC_ProjectEditor.Constants.Layers.GeolineLayerQC_ApproxConfidence;
        private const string geolineInferedName = GSC_ProjectEditor.Constants.Layers.GeolineLayerQC_InferredConfidence;
        private const string geolineConcealedName = GSC_ProjectEditor.Constants.Layers.GeolineLayerQC_ConcealeadConfidence;
        private const string geolineNotAppName = GSC_ProjectEditor.Constants.Layers.GeolineLayerQC_NotApplicableConfidence;
        private const string geolineMustBeFlip = GSC_ProjectEditor.Constants.Layers.GeolineLayerQC_ContactAgeMustBeFlip;
        private const string geolineNoNeighbours = GSC_ProjectEditor.Constants.Layers.GeolineLayerQC_ContactAgeNoNeighbour;
        private const string geolineMoveFlip = GSC_ProjectEditor.Constants.Layers.GeolineLayerQC_MovementFaultFlip;
        private const string geolineMoveCode01 = GSC_ProjectEditor.Constants.Layers.GeolineLayerQC_MovementFaultCode01;
        private const string geolineMoveCode02 = GSC_ProjectEditor.Constants.Layers.GeolineLayerQC_MovementFaultCode02;
        private const string geolineMoveCode03 = GSC_ProjectEditor.Constants.Layers.GeolineLayerQC_MovementFaultCode03;
        private const string geolineMoveCode04 = GSC_ProjectEditor.Constants.Layers.GeolineLayerQC_MovementFaultCode04;
        private const string geolineMoveCode05 = GSC_ProjectEditor.Constants.Layers.GeolineLayerQC_MovementFaultCode05;
        private const string geolineMoveCode06 = GSC_ProjectEditor.Constants.Layers.GeolineLayerQC_MovementFaultCode06;
        private const string geolineMoveCode07 = GSC_ProjectEditor.Constants.Layers.GeolineLayerQC_MovementFaultCode07;
        private const string geolineMoveCode08 = GSC_ProjectEditor.Constants.Layers.GeolineLayerQC_MovementFaultCode08;
        private const string geolineMoveMissing = GSC_ProjectEditor.Constants.Layers.GeolineLayerQC_MovementFaultMissing;
        private const string geolineMoveSymChange = GSC_ProjectEditor.Constants.Layers.GeolineLayerQC_MovementFaultChangeSym;
        private const string geolineConcealedOverprintFull = GSC_ProjectEditor.Constants.Layers.GeolineLayerQC_ConcealedOverprintFullyInside;
        private const string geolineConcealedOverprintsTouches = GSC_ProjectEditor.Constants.Layers.GeolineLayerQC_ConcealedOverprintTouches;

        //GEO_POLYS feature
        private const string geopolyFeature = GSC_ProjectEditor.Constants.Database.FGeopoly;
        private const string geopolyLabelField = GSC_ProjectEditor.Constants.DatabaseFields.FGeopolyLabel;
        private const string geopolyRedAdjacentValidationLayer = GSC_ProjectEditor.Constants.Layers.MapUnitLayerQC_Red;
        private const string geopolyYellowAdjacentValidationLayer = GSC_ProjectEditor.Constants.Layers.MapUnitLayerQC_Yellow;
        private const string geopolyYellowSmallFeatLayer1 = GSC_ProjectEditor.Constants.Layers.MapUnitLayerQC_YellowSmallFeature1;
        private const string geopolyYellowSmallFeatLayer2 = GSC_ProjectEditor.Constants.Layers.MapUnitLayerQC_YellowSmallFeature2;
        private const string geopolyYellowSmallFeatLayer3 = GSC_ProjectEditor.Constants.Layers.MapUnitLayerQC_YellowSmallFeature3;
        private const string geopolyLayerName = GSC_ProjectEditor.Constants.Layers.geopoly;

        //P_LABELS feature
        private const string plabelFeature = GSC_ProjectEditor.Constants.Database.FLabel;
        private const string plabelLabelField = GSC_ProjectEditor.Constants.DatabaseFields.FLabelID;
        private const string plabelRedLayerName = GSC_ProjectEditor.Constants.Layers.MapUnitLayerQC_RedMultipleLabel;
        private const string plabelRedLabelLayerName = GSC_ProjectEditor.Constants.Layers.labelLayerQC_RedMultipleLabel;
        private const string plabelYellowLayerName = GSC_ProjectEditor.Constants.Layers.MapUnitLayerQC_YellowMultipleLabel;
        private const string plabelLayerName = GSC_ProjectEditor.Constants.Layers.label;
        
        //F_STATION feature
        private const string fieldStationFeature = GSC_ProjectEditor.Constants.Database.FStation;
        private const string fieldStationEasting = GSC_ProjectEditor.Constants.DatabaseFields.FStationEasting;
        private const string fieldStationNorthing = GSC_ProjectEditor.Constants.DatabaseFields.FStationNorthing;
        private const string fieldStationLong = GSC_ProjectEditor.Constants.DatabaseFields.FStationLong;
        private const string fieldStationLat = GSC_ProjectEditor.Constants.DatabaseFields.FStationLat;
        private const string fieldStationID = GSC_ProjectEditor.Constants.DatabaseFields.FStationID;
        private const string fieldStationENLayer = GSC_ProjectEditor.Constants.Layers.FStationLayerQC_BadEastNorth;
        private const string fieldStationLLLayer = GSC_ProjectEditor.Constants.Layers.FStationLayerQC_BadLatLong;
        private const string fieldStationLayerName = GSC_ProjectEditor.Constants.Layers.fStation;

        //P_LEGEND_GENERATOR table
        private const string tLegendGen = GSC_ProjectEditor.Constants.Database.TLegendGene;
        private const string tLegendGenLabel = GSC_ProjectEditor.Constants.DatabaseFields.LegendLabelID;
        private const string tLegendGenOrder = GSC_ProjectEditor.Constants.DatabaseFields.LegendOrder;
        private const string tLegendSymType = GSC_ProjectEditor.Constants.DatabaseFields.LegendSymType;
        private const string tLegendSymTypeGeoline = GSC_ProjectEditor.Constants.DatabaseDomainsValues.SymTypeLine;
        private const string tLegendSymCode = GSC_ProjectEditor.Constants.DatabaseFields.LegendSymbol;

        //Controls
        private List<RadioButton> generalTabControls = new List<RadioButton>();
        private List<RadioButton> bedrockTabControls = new List<RadioButton>();
        private List<RadioButton> surficialTabControls = new List<RadioButton>();

        //Identity fields
        public const string leftPolyOID = "LEFT_GEO_POLYS";
        public const string rightPolyOID = "RIGHT_GEO_POLYS";

        //Other
        //private const string interGroupLayer = GSC_ProjectEditor.Properties.Resources.GroupLayerInterpretation;
        //private const string fieldworkGrouplayer = GSC_ProjectEditor.Properties.Resources.GroupLayerFieldWork;
        public delegate void rbDelegate(); //A delegate to trigger a method from radio buttons tag
        public delegate void rbExecutionEventHandler(object sender, EventArgs e); //A delegate for execution events
        public event rbExecutionEventHandler processHasStarted; //This event is triggered when a new process is started with the execution button
        public event rbExecutionEventHandler processHasEnded; //This event is triggered when a QC process has ended.
        private const string objectidField = GSC_ProjectEditor.Constants.DatabaseFields.ObjectID;
        private const string objectIDFieldGeoPoly = geopolyFeature + "." + objectidField;
        //private const string validationGroupLayerName = GSC_ProjectEditor.Properties.Resources.GroupLayerValidation;
        private const double smallAreaFactor1 = GSC_ProjectEditor.Constants.MathConstants.smallAreaFactorClass1;
        private const double smallAreaFactor2 = GSC_ProjectEditor.Constants.MathConstants.smallAreaFactorClass2;
        private const string inMemoryWorkspace = "in_memory";
        private const string qcNumFieldTypeDouble = "Double";
        private const string qcNumFieldTypeSmallInt = "SmallInt";
        private const string qcNumFieldTypeSingle = "Single";
        private const string qcNumFieldTypeInteger = "Integer";

        #endregion

        /// <summary>
        /// Will be used to fill the report table for QC on statistic of numerical data
        /// </summary>
        public class QCNumDataReport
        {
            public string datasetName { get; set; }
            public string fieldName { get; set; }
            public string fieldType { get; set; }
            public object fieldMin { get; set; }
            public object fieldMax { get; set; }
            public double fieldAvg { get; set; }
            public double fieldStrd { get; set; }
            public int fieldCount { get; set; }
            public List<object> fieldValues { get; set; } //Will be used to calculate stats
        }

        /// <summary>
        /// Init. of dw used for quality control when editing project data.
        /// </summary>
        /// <param name="hook"></param>
        public Dockablewindow_CreateEdit_QualityControl(object hook)
        {
            //Set culture before init.
            Utilities.Culture.SetCulture();

            InitializeComponent();
            this.Hook = hook;

            //If enabled is already open before init.
            if (this.Enabled)
            {
                dockablewindowQualityControl_EnabledChanged(this, null);
            }
            else if (!this.Enabled)
            {
                //Init controls if enabled.
                this.EnabledChanged += new EventHandler(dockablewindowQualityControl_EnabledChanged);
            }

        }

        /// <summary>
        /// Starting QC event
        /// Changes cursor for a waiting icon.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void dockablewindowQualityControl_processHasStarted(object sender, EventArgs e)
        {
            this.btn_ExecuteQC.Cursor = Cursors.WaitCursor;
        }

        /// <summary>
        /// Ending QC event
        /// Changes cursor back to default icon.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void dockablewindowQualityControl_processHasEnded(object sender, EventArgs e)
        {
            this.btn_ExecuteQC.Cursor = Cursors.Default;
        }

        /// <summary>
        /// Enable event, fill radio button lists.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void dockablewindowQualityControl_EnabledChanged(object sender, EventArgs e)
        {

            //Gen tab list
            generalTabControls.Add(this.radioBtn_reportMetadata);
            generalTabControls.Add(this.radioBtn_reportPhoto);
            generalTabControls.Add(this.radioBtn_reportSMS);
            generalTabControls.Add(this.radioBtn_reportGenStat);
            generalTabControls.Add(this.radioBtn_valGeom);
            generalTabControls.Add(this.radioBtn_valBound);
            generalTabControls.Add(this.radioBtn_valConfidence);
            generalTabControls.Add(this.radioBtn_null);
            generalTabControls.Add(this.radioBtn_findDiffLabel);
            generalTabControls.Add(this.radioBtn_findSmall);
            generalTabControls.Add(this.radioBtn_findSamePoly);
            generalTabControls.Add(this.radioBtn_findOutSpatialDomain);
            generalTabControls.Add(this.radioBtn_findOrphanLabel);
            generalTabControls.Add(this.radioBtn_valConcealineOverprints);

            //Bedrock tab list
            bedrockTabControls.Add(this.radioBtn_valFault);
            bedrockTabControls.Add(this.radioBtn_valContactAge);
            
            //Surficial tab list
            surficialTabControls.Add(this.radioBtn_nullGeolLabels);
            surficialTabControls.Add(this.radioBtn_valDrumlin);
            surficialTabControls.Add(this.radioBtn_valFlow);

            this.processHasStarted += new rbExecutionEventHandler(dockablewindowQualityControl_processHasStarted);
            this.processHasEnded += new rbExecutionEventHandler(dockablewindowQualityControl_processHasEnded);
        }

        #region Init.

        /// <summary>
        /// Host object of the dockable window
        /// </summary>
        private object Hook
        {
            get;
            set;
        }

        /// <summary>
        /// Implementation class of the dockable window add-in. It is responsible for 
        /// creating and disposing the user interface class of the dockable window.
        /// </summary>
        public class AddinImpl : ESRI.ArcGIS.Desktop.AddIns.DockableWindow
        {
            private Dockablewindow_CreateEdit_QualityControl m_windowUI;
            /// <summary>
            /// Init.
            /// </summary>
            public AddinImpl()
            {

            }
            /// <summary>
            /// Create the dw
            /// </summary>
            /// <returns></returns>
            protected override IntPtr OnCreateChild()
            {
                m_windowUI = new Dockablewindow_CreateEdit_QualityControl(this.Hook);
                return m_windowUI.Handle;
            }
            /// <summary>
            /// Dispose the dw
            /// </summary>
            /// <param name="disposing"></param>
            protected override void Dispose(bool disposing)
            {
                if (m_windowUI != null)
                    m_windowUI.Dispose(disposing);

                base.Dispose(disposing);
            }

        }

        #endregion

        /// <summary>
        /// Click event to show useful information to user about what will be done when "Execute" button is pressed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Info_Click(object sender, EventArgs e)
        {
            MessageBox.Show(Properties.Resources.Text_ButtonInfo_01);
        }

        /// <summary>
        /// Click event of execute button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_ExecuteQC_Click(object sender, EventArgs e)
        {
            //Call event to change cusor icon.
            processHasStarted(sender, e);

            //Get current tab name
            string tabName = this.tab_QC.SelectedTab.Name;

            //Get radio button selection (depends on tab)
            if (tabName.Contains("General"))
            {
                //Find which radio button is on
                findRadioButton(generalTabControls);

            }
            else if (tabName.Contains("Bedrock"))
            {
                //Find which radio button is on
                findRadioButton(bedrockTabControls);
            }
            else if (tabName.Contains("Surficial"))
            {
                //Find which radio button is on
                findRadioButton(surficialTabControls);
            }

            //Call event to change cursor back to default.
            processHasEnded(sender, e);

            GSC_ProjectEditor.Messages.ShowEndOfProcess();
        }

        #region QC GENERIC

        /// <summary>
        /// This function will find multiple different labels within one polygon.
        /// Map Unit unicity validation.
        /// </summary>
        public void FindDifferentLabelInPoly()
        {

            #region Variable creation and assignment
            //Create resulting list
            List<string> redList = new List<string>(); //Will contain poly oid 
            List<string> redLabelList = new List<string>(); //Will contain label oid

            //Access geoline feature
            IFeatureLayer geopolyFL = Utilities.MapDocumentFeatureLayers.GetFeatureLayerPreProcessing(geopolyFeature, GSC_ProjectEditor.Properties.Resources.GroupLayerInterpretation);

            //Access geopoly feature
            IFeatureLayer plabelFL = Utilities.MapDocumentFeatureLayers.GetFeatureLayerPreProcessing(plabelFeature, GSC_ProjectEditor.Properties.Resources.GroupLayerInterpretation);

            //Create a selection object to extract polygons
            IFeatureSelection geopolySelect = geopolyFL as IFeatureSelection;

            //Create filters to query labels and polygons
            ISpatialFilter spatialQuery = new SpatialFilter();
            IQueryFilter simpleQuery = new QueryFilter();

            #endregion

            #region Remove any existing result layer from TOC.
            Utilities.MapDocumentFeatureLayers.RemoveLayerFromArcMapPreProcessing(plabelRedLayerName);
            Utilities.MapDocumentFeatureLayers.RemoveLayerFromArcMapPreProcessing(plabelRedLabelLayerName);

            #endregion

            #region iterate through geopoly

            //MAIN PROCESS => Iterate through all geolines
            IFeatureCursor polyCursor = geopolyFL.Search(simpleQuery, false);
            IFeature polyFeat = null;

            while ((polyFeat = polyCursor.NextFeature()) != null)
            {
                //Get field indexex
                int fieldObjectIDIndex = polyFeat.Fields.FindField(objectidField);

                //Get label and OID field value
                object currentOIDValue = polyFeat.get_Value(fieldObjectIDIndex);

                //Variables
                List<string> uniqueRedLabelList = new List<string>(); //A list to get unique labels for each polygons
                List<string> uniqueRedLabelOIDList = new List<string>(); //A list to get problematic labels oids
                List<string> uniqueRedOIDList = new List<string>(); //A list to get polygons OID
                int countResult = 0; //A counter to validate number of element in lists

                //Perform a spatial query
                spatialQuery.Geometry = polyFeat.Shape; //Add current feature polygon geometry to spatialQuery
                spatialQuery.GeometryField = plabelFL.FeatureClass.ShapeFieldName; //Add shape field name of feature class querying against  (labels)
                spatialQuery.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;//esriSpatialRelEnum.esriSpatialRelIntersects; //A simple intersection query

                #region iterate through polygons and build resulting lists
                //Iterate through polygons, with an applied spatial query into it
                IFeatureCursor labelCursor = plabelFL.Search((IQueryFilter)spatialQuery, false);
                IFeature labelFeat = null;

                while ((labelFeat = labelCursor.NextFeature()) != null)
                {
                    //Get field indexex
                    int labelIndex = polyFeat.Fields.FindField(plabelLabelField);
                    int labelObjectIDIndex = polyFeat.Fields.FindField(objectidField);

                    //Get label and OID field value
                    object labelValue = labelFeat.get_Value(labelIndex);
                    object OIDValue = labelFeat.get_Value(labelObjectIDIndex);

                    //Parse result
                    if (!uniqueRedLabelList.Contains(labelValue.ToString()) && labelValue.ToString() != "")
                    {
                        //Add new value to list
                        uniqueRedLabelList.Add(labelValue.ToString());
                        uniqueRedLabelOIDList.Add(OIDValue.ToString());
                        //Get current poly OID and add to list
                        if (!uniqueRedOIDList.Contains(currentOIDValue.ToString()) && uniqueRedLabelList.Count > 1)
                        {
                            uniqueRedOIDList.Add(currentOIDValue.ToString());
                        }
                        

                    }

                    countResult = countResult + 1;
                }

                //Release cursor
                System.Runtime.InteropServices.Marshal.ReleaseComObject(labelCursor);

                //Find any result related to multiple different label that has been found
                if (uniqueRedOIDList.Count >= 1)
                {
                    //Add to red list => Meaning the resulting process found that two, or more different labels have been set within the same polygon.
                    redList = redList.Concat(uniqueRedOIDList).ToList();
                    redLabelList = redLabelList.Concat(uniqueRedLabelOIDList).ToList();
                }

                #endregion

            }

            //Release cursor
            System.Runtime.InteropServices.Marshal.ReleaseComObject(polyCursor);

            #endregion

            #region Parse result
            if (redList.Count != 0)
            {

                #region parse label layer

                //Select all problematic points
                IFeatureLayer newFLMultipleLabel = GSC_ProjectEditor.FeatureLayers.CreateFeatureLayerFromSelection(plabelFL, plabelFeature, redLabelList, objectidField, plabelRedLabelLayerName, true);

                //Access other attributes of layer with geofeaturelayer
                IGeoFeatureLayer getGeoFLMultipleLabel = newFLMultipleLabel as IGeoFeatureLayer;

                //Get a renderer
                List<int> redLabelFillColor = new List<int>(new int[] { 255, 0, 0 });
                IFeatureRenderer redLabelRenderer = GSC_ProjectEditor.Symbols.GetPointRenderer(redLabelFillColor, 2.0) as IFeatureRenderer;

                //Set renderer into layer
                getGeoFLMultipleLabel.Renderer = redLabelRenderer;

                //Add new layer to Arc Map
                Utilities.MapDocumentFeatureLayers.AddLayerToArcMapPreProcessing(getGeoFLMultipleLabel as ILayer, GSC_ProjectEditor.Properties.Resources.GroupLayerValidation);

                #endregion


                #region Parse polygon layer

                //Get a selection layer from redlist
                IFeatureLayer newFLMultipleLabelPoly = GSC_ProjectEditor.FeatureLayers.CreateFeatureLayerFromSelection(geopolyFL, geopolyFeature, redList, objectidField, plabelRedLayerName, true);

                //Access other attributes of layer with geofeaturelayer
                IGeoFeatureLayer getGeoFLMultipleLabelPoly = newFLMultipleLabelPoly as IGeoFeatureLayer;

                //Get a renderer
                List<int> redFillColor = new List<int>(new int[] { 255, 190, 190 });
                List<int> redOutlineColor = new List<int>(new int[] { 255, 0, 0 });
                IFeatureRenderer redRenderer = GSC_ProjectEditor.Symbols.GetSimpleFillRenderer(redFillColor, redOutlineColor, 2.0) as IFeatureRenderer;

                //Set renderer into layer
                getGeoFLMultipleLabelPoly.Renderer = redRenderer;

                //Add new layer to Arc Map
                Utilities.MapDocumentFeatureLayers.AddLayerToArcMapPreProcessing(getGeoFLMultipleLabelPoly as ILayer, GSC_ProjectEditor.Properties.Resources.GroupLayerValidation);
                
                #endregion



            }
            else
            {
                MessageBox.Show(Properties.Resources.Warning_QC_NoDiffLabel);
            }

            #endregion
 
        }

        /// <summary>
        /// This function will find same label with adjacent polygons.
        /// Map Unit unicity validation
        /// </summary>
        public void FindSameLabelInAdjacentPoly()
        {
            
            try
            {
                #region Variable creation and assignment
                //Create resulting list
                List<string> redList = new List<string>();

                //Access geopoly feature
                IFeatureLayer geopolyFL = Utilities.MapDocumentFeatureLayers.GetFeatureLayerPreProcessing(geopolyFeature, GSC_ProjectEditor.Properties.Resources.GroupLayerInterpretation);

                //Create a selection object to extract polygons
                IFeatureSelection geopolySelect = geopolyFL as IFeatureSelection;

                //Create filters to query lines and polygons
                ISpatialFilter spatialQuery = new SpatialFilter();
                IQueryFilter simpleQuery = new QueryFilter();

                #endregion

                #region Remove any existing result layer from TOC.

                removeOldValidationLayer(new List<string> { geopolyRedAdjacentValidationLayer, geopolyYellowAdjacentValidationLayer });

                #endregion

                #region iterate through geolines

                //MAIN PROCESS => Iterate through all geolines
                IFeatureCursor lineCursor = geopolyFL.Search(simpleQuery, true);
                IFeature polyFeat = null;

                while ((polyFeat = lineCursor.NextFeature()) != null)
                {
                    //Get field indexex
                    int fieldGeopolyIDIndex = polyFeat.Fields.FindField(geopolyLabelField);
                    int fieldObjectIDIndex = polyFeat.Fields.FindField(objectidField);

                    //Get label and OID field value
                    string currentLabelValue = polyFeat.get_Value(fieldGeopolyIDIndex).ToString();
                    string currentOIDValue = polyFeat.get_Value(fieldObjectIDIndex).ToString();

                    //Variables
                    //List<string> uniqueLabelList = new List<string>(); //A list to get unique labels for each polygons
                    //Dictionary<string, List<string>> uniqueDico = new Dictionary<string, List<string>>();
                    List<string> uniqueOIDList = new List<string>() { currentOIDValue }; //A list to get polygons OID with the one querying against as init value

                    //Perform a spatial query
                    spatialQuery.Geometry = polyFeat.Shape; //Add current feature line geometry to spatialQuery
                    spatialQuery.GeometryField = geopolyFL.FeatureClass.ShapeFieldName; //Add shape field name of feature class querying against  (polygon)
                    spatialQuery.SpatialRel = esriSpatialRelEnum.esriSpatialRelTouches;//esriSpatialRelEnum.esriSpatialRelIntersects; //A simple intersection query

                    #region iterate through polygons and build resulting lists
                    //Iterate through polygons, with an applied spatial query into it
                    IFeatureCursor polyCursor = geopolyFL.Search((IQueryFilter)spatialQuery, true);
                    IFeature polyFeat2 = null;
                    
                    while ((polyFeat2 = polyCursor.NextFeature()) != null)
                    {
                        int fieldLabelIndex = polyFeat2.Fields.FindField(geopolyLabelField);
                        //Get label and OID field value
                        string labelValue = polyFeat2.get_Value(fieldLabelIndex).ToString();
                        string OIDValue = polyFeat2.get_Value(fieldObjectIDIndex).ToString();

                        if (labelValue == currentLabelValue)
                        {
                            if (!redList.Contains(OIDValue))
                            {
                                redList.Add(OIDValue); 
                            }
                            
                        }
                    }

                    //Release cursor
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(polyCursor);

                    #endregion

                }

                //Release cursor
                System.Runtime.InteropServices.Marshal.ReleaseComObject(lineCursor);

                #endregion

                #region Parse result
                if (redList.Count != 0)
                {
                    //Get a selection layer from redlist
                    IFeatureLayer newFL = GSC_ProjectEditor.FeatureLayers.CreateFeatureLayerFromSelection(geopolyFL, geopolyFeature, redList, objectidField, geopolyRedAdjacentValidationLayer, true);

                    //Access other attributes of layer with geofeaturelayer
                    IGeoFeatureLayer getGeoFL = newFL as IGeoFeatureLayer;

                    //Get a renderer
                    List<int> redFillColor = new List<int>(new int[] { 255, 190, 190 });
                    List<int> redOutlineColor = new List<int>(new int[] { 255, 0, 0 });
                    IFeatureRenderer redRenderer = GSC_ProjectEditor.Symbols.GetSimpleFillRenderer(redFillColor, redOutlineColor, 2.0) as IFeatureRenderer;

                    //Set renderer into layer
                    getGeoFL.Renderer = redRenderer;

                    //Add new layer to Arc Map
                    Utilities.MapDocumentFeatureLayers.AddLayerToArcMapPreProcessing(getGeoFL as ILayer, GSC_ProjectEditor.Properties.Resources.GroupLayerValidation);

                }
                else
                {
                    MessageBox.Show(Properties.Resources.Warning_QC_NoSameAdjLabel);
                }

                #endregion

            }
            catch (Exception findSameLabelInAdjacentPolyEx)
            {
                MessageBox.Show(findSameLabelInAdjacentPolyEx.Message + " --> Line Number: " + GSC_ProjectEditor.Exceptions.LineNumber(findSameLabelInAdjacentPolyEx).ToString());
            }

        }

        /// <summary>
        /// This function will find small features, mainly polygons, related to environment predefined scale
        /// </summary>
        public void FindSmallFeatures()
        {
            

            #region Variable creation and assignment
            //Create resulting list
            List<string> yellowListClass1 = new List<string>();
            List<string> yellowListClass2 = new List<string>();
            List<string> yellowListClass3 = new List<string>();

            //Access geoline feature
            IFeatureLayer geopolyFL = Utilities.MapDocumentFeatureLayers.GetFeatureLayerPreProcessing(geopolyFeature, GSC_ProjectEditor.Properties.Resources.GroupLayerInterpretation);

            //Get area field name
            string areaFieldName = geopolyFL.FeatureClass.AreaField.Name;

            //Create a selection object to extract polygons
            IFeatureSelection geopolySelect = geopolyFL as IFeatureSelection;

            //Create filters to query labels and polygons
            IQueryFilter simpleQuery = new QueryFilter();

            #endregion

            #region Get user project scale and calculate minimal area

            //Get user scale
            
            string getScaleString = Form_Generic.ShowGenericTextboxForm("Map Scale", "Input Map Scale for calculation", null, "50000");
            double getScale = 50000;
            Double.TryParse(getScaleString, out getScale);

            //Calculate smallest area
            double getSmallestLine1 = (smallAreaFactor1/2 * getScale)/1000; //Divide by 1000 to get result in meter
            double getSmallestArea1 = Math.PI * getSmallestLine1 * getSmallestLine1; //Circle area formula
            double getSmallestLine2 = (smallAreaFactor2/2 * getScale) / 1000; //Divide by 1000 to get result in meter
            double getSmallestArea2 = Math.PI * getSmallestLine2 * getSmallestLine2; //Circle area formula

            #endregion

            #region Remove any existing result layer from TOC.

            removeOldValidationLayer(new List<string> { geopolyYellowSmallFeatLayer1, geopolyYellowSmallFeatLayer2, geopolyYellowSmallFeatLayer3 });

            #endregion

            #region Iterate through geopoly and parse info

            //MAIN PROCESS => Iterate through all geolines
            IFeatureCursor polyCursor = geopolyFL.Search(simpleQuery, false);

            //Get field indexes
            int fieldObjectIDIndex = polyCursor.FindField(objectidField);
            int areaFieldIndex = polyCursor.FindField(areaFieldName);
            int labelFieldIndex = polyCursor.FindField(geopolyLabelField);
            IFeature polyFeat = null;
            while ((polyFeat = polyCursor.NextFeature()) != null)
            {

                //Get area and OID field value
                object currentOIDValue = polyFeat.get_Value(fieldObjectIDIndex);
                object currentAreaValue = polyFeat.get_Value(areaFieldIndex);
                object currentLabelValue = polyFeat.get_Value(labelFieldIndex);

                //Parse result
                if (Convert.ToInt64(currentAreaValue) <= getSmallestArea1 && Convert.ToInt64(currentAreaValue) > getSmallestArea2)
                {
                    //Add new value to list
                    yellowListClass1.Add(currentOIDValue.ToString());

                    //Check for missing labels
                    if (currentLabelValue.ToString() == "")
                    {
                        yellowListClass3.Add(currentOIDValue.ToString());
                    }
                }
                else if (Convert.ToInt64(currentAreaValue) <= getSmallestArea2)
                {
                    //Add new value to list
                    yellowListClass2.Add(currentOIDValue.ToString());

                    //Check for missing labels
                    if (currentLabelValue.ToString() == "")
                    {
                        yellowListClass3.Add(currentOIDValue.ToString());
                    }
                }

            }

            //Release cursor
            System.Runtime.InteropServices.Marshal.ReleaseComObject(polyCursor);

            #endregion

            #region Parse result
            if (yellowListClass1.Count != 0)
            {

                //Get a selection layer from yellowlist
                IFeatureLayer newYellowFLMultipleLabel = GSC_ProjectEditor.FeatureLayers.CreateFeatureLayerFromSelection(geopolyFL, geopolyFeature, yellowListClass1, objectidField, geopolyYellowSmallFeatLayer1, true);

                //Access other attributes of layer with geofeaturelayer
                IGeoFeatureLayer getYellowGeoFLMultipleLabel = newYellowFLMultipleLabel as IGeoFeatureLayer;

                //Get a renderer
                List<int> fillColor = new List<int>(new int[] { 255, 255, 115 });
                List<int> outlineColor = new List<int>(new int[] { 255, 170, 0 });
                IFeatureRenderer yellowRenderer = GSC_ProjectEditor.Symbols.GetSimpleFillRenderer(fillColor, outlineColor, 2.0) as IFeatureRenderer;

                //Set renderer into layer
                getYellowGeoFLMultipleLabel.Renderer = yellowRenderer;

                //Add new layer to Arc Map
                Utilities.MapDocumentFeatureLayers.AddLayerToArcMapPreProcessing(getYellowGeoFLMultipleLabel as ILayer, GSC_ProjectEditor.Properties.Resources.GroupLayerValidation);

            }
            if (yellowListClass2.Count != 0)
            {

                //Get a selection layer from yellowlist
                IFeatureLayer newYellowFLMultipleLabel2 = GSC_ProjectEditor.FeatureLayers.CreateFeatureLayerFromSelection(geopolyFL, geopolyFeature, yellowListClass2, objectidField, geopolyYellowSmallFeatLayer2, true);

                //Access other attributes of layer with geofeaturelayer
                IGeoFeatureLayer getYellowGeoFLMultipleLabel2 = newYellowFLMultipleLabel2 as IGeoFeatureLayer;

                //Get a renderer
                List<int> fillColor = new List<int>(new int[] { 255, 170, 0 });
                List<int> outlineColor = new List<int>(new int[] { 255, 85, 0 });
                IFeatureRenderer yellowRenderer2 = GSC_ProjectEditor.Symbols.GetSimpleFillRenderer(fillColor, outlineColor, 2.0) as IFeatureRenderer;

                //Set renderer into layer
                getYellowGeoFLMultipleLabel2.Renderer = yellowRenderer2;

                //Add new layer to Arc Map
                Utilities.MapDocumentFeatureLayers.AddLayerToArcMapPreProcessing(getYellowGeoFLMultipleLabel2 as ILayer, GSC_ProjectEditor.Properties.Resources.GroupLayerValidation);

            }
            if (yellowListClass3.Count != 0)
            {

                //Get a selection layer from yellowlist
                IFeatureLayer newYellowFLMultipleLabel3 = GSC_ProjectEditor.FeatureLayers.CreateFeatureLayerFromSelection(geopolyFL, geopolyFeature, yellowListClass3, objectidField, geopolyYellowSmallFeatLayer3, true);

                //Access other attributes of layer with geofeaturelayer
                IGeoFeatureLayer getYellowGeoFLMultipleLabel3 = newYellowFLMultipleLabel3 as IGeoFeatureLayer;

                //Get a renderer
                List<int> fillColor = new List<int>(new int[] { 255, 190, 190 });
                List<int> outlineColor = new List<int>(new int[] { 255, 0, 0 });
                IFeatureRenderer yellowRenderer3 = GSC_ProjectEditor.Symbols.GetSimpleFillRenderer(fillColor, outlineColor, 2.0) as IFeatureRenderer;

                //Set renderer into layer
                getYellowGeoFLMultipleLabel3.Renderer = yellowRenderer3;

                //Add new layer to Arc Map
                Utilities.MapDocumentFeatureLayers.AddLayerToArcMapPreProcessing(getYellowGeoFLMultipleLabel3 as ILayer, GSC_ProjectEditor.Properties.Resources.GroupLayerValidation);

            }

            #endregion
 
        }

        /// <summary>
        /// Will find any polygon (map units) that doesn't have any label associated to it.
        /// Will add a new layer witin Arc Map
        /// </summary>
        public void FindOrhpanPoly()
        {


            #region Variable creation and assignment
            //Create resulting list
            List<string> yellowList = new List<string>();

            //Access geoline feature
            IFeatureLayer geopolyFL = Utilities.MapDocumentFeatureLayers.GetFeatureLayerPreProcessing(geopolyFeature, GSC_ProjectEditor.Properties.Resources.GroupLayerInterpretation);

            //Access geopoly feature
            IFeatureLayer plabelFL = Utilities.MapDocumentFeatureLayers.GetFeatureLayerPreProcessing(plabelFeature, GSC_ProjectEditor.Properties.Resources.GroupLayerInterpretation);

            //Create a selection object to extract polygons
            IFeatureSelection geopolySelect = geopolyFL as IFeatureSelection;

            //Create filters to query labels and polygons
            ISpatialFilter spatialQuery = new SpatialFilter();
            IQueryFilter simpleQuery = new QueryFilter();

            #endregion

            #region Remove any existing result layer from TOC.
            Utilities.MapDocumentFeatureLayers.RemoveLayerFromArcMapPreProcessing(plabelYellowLayerName);

            #endregion

            #region iterate through geolines

            //MAIN PROCESS => Iterate through all geolines
            IFeatureCursor polyCursor = geopolyFL.Search(simpleQuery, false);
            IFeature polyFeat = null;

            while ((polyFeat = polyCursor.NextFeature()) != null)
            {
                //Get field indexex
                int fieldObjectIDIndex = polyFeat.Fields.FindField(objectidField);

                //Get label and OID field value
                object currentOIDValue = polyFeat.get_Value(fieldObjectIDIndex);

                //Variables
                int countResult = 0; //A counter to validate number of element in lists

                //Perform a spatial query
                spatialQuery.Geometry = polyFeat.Shape; //Add current feature polygon geometry to spatialQuery
                spatialQuery.GeometryField = plabelFL.FeatureClass.ShapeFieldName; //Add shape field name of feature class querying against  (labels)
                spatialQuery.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;//esriSpatialRelEnum.esriSpatialRelIntersects; //A simple intersection query

                #region iterate through polygons and build resulting lists
                //Iterate through polygons, with an applied spatial query into it
                IFeatureCursor labelCursor = plabelFL.Search((IQueryFilter)spatialQuery, false);
                IFeature labelFeat = null;

                while ((labelFeat = labelCursor.NextFeature()) != null)
                {
                    countResult = countResult + 1;
                }

                //Release cursor
                System.Runtime.InteropServices.Marshal.ReleaseComObject(labelCursor);

                //Find if current polygon returns nothing, if yes it means there is no label within it.
                if (countResult == 0)
                {
                    //Add to yellow list => Meaning the resulting process found that no label were inside polygon
                    yellowList.Add(currentOIDValue.ToString());

                }

                #endregion

            }

            //Release cursor
            System.Runtime.InteropServices.Marshal.ReleaseComObject(polyCursor);

            #endregion

            #region Parse result
            if (yellowList.Count != 0)
            {

                //Get a selection layer from yellowlist
                IFeatureLayer newYellowFLMultipleLabel = GSC_ProjectEditor.FeatureLayers.CreateFeatureLayerFromSelection(geopolyFL, geopolyFeature, yellowList, objectidField, plabelYellowLayerName, true);

                //Access other attributes of layer with geofeaturelayer
                IGeoFeatureLayer getYellowGeoFLMultipleLabel = newYellowFLMultipleLabel as IGeoFeatureLayer;

                //Get a renderer
                List<int> fillColor = new List<int>(new int[] { 255, 255, 115 });
                List<int> outlineColor = new List<int>(new int[] { 255, 170, 0 });
                IFeatureRenderer yellowRenderer = GSC_ProjectEditor.Symbols.GetSimpleFillRenderer(fillColor, outlineColor, 2.0) as IFeatureRenderer;

                //Set renderer into layer
                getYellowGeoFLMultipleLabel.Renderer = yellowRenderer;

                //Add new layer to Arc Map
                Utilities.MapDocumentFeatureLayers.AddLayerToArcMapPreProcessing(getYellowGeoFLMultipleLabel as ILayer, GSC_ProjectEditor.Properties.Resources.GroupLayerValidation);

            }
            else
            {
                MessageBox.Show(Properties.Resources.Warning_QC_NoLabel);
            }

            #endregion

        }

        /// <summary>
        /// Will highlight all values for isboundary = yes in one layer and = no in another layer
        /// </summary>
        public void FlashIsBoundary()
        {
            try
            {
                #region Variable creation and assignment
                //Access geoline feature
                IFeatureLayer geolineFL = Utilities.MapDocumentFeatureLayers.GetFeatureLayerPreProcessing(geolineFeature, GSC_ProjectEditor.Properties.Resources.GroupLayerInterpretation);

                //Create filters to query labels and polygons
                IQueryFilter simpleQuery = new QueryFilter();

                //Empty list to contain objectIDs
                List<string> blueYesList = new List<string>();
                List<string> pinkNoList = new List<string>();

                #endregion

                #region Remove any existing result layer from TOC.

                removeOldValidationLayer(new List<string> { geolineBlueLayerName, geolinePinkLayerName });

                #endregion

                #region Manage boundaries

                //MAIN PROCESS => Iterate through all geolines
                IFeatureCursor lineCursor = geolineFL.Search(simpleQuery, true);
                IFeature lineFeat = null;

                //Get field indexes
                int fieldObjectIDIndex = lineCursor.Fields.FindField(objectidField);
                int fieldIsBoundIndex = lineCursor.Fields.FindField(geolineIsBoundaryField);


                while ((lineFeat = lineCursor.NextFeature()) != null)
                {
                    //Get field information
                    object currentOIDValue = lineFeat.get_Value(fieldObjectIDIndex);
                    object currentIsBound = lineFeat.get_Value(fieldIsBoundIndex);

                    //Find if current polygon returns nothing, if yes it means there is no label within it.
                    if (currentIsBound.ToString() == geolineIsBoundaryValueYes)
                    {
                        blueYesList.Add(currentOIDValue.ToString());

                    }
                    else if (currentIsBound.ToString() == geolineIsBoundaryValueNo)
                    {
                        pinkNoList.Add(currentOIDValue.ToString());
                    }

                    if (pinkNoList.Count != 0 && blueYesList.Count!=0)
                    {
                        break;
                    }

                }

                //Release cursor
                System.Runtime.InteropServices.Marshal.ReleaseComObject(lineCursor);

                #endregion

                #region Parse No boundaries
                if (pinkNoList.Count != 0)
                {
                    //Create a new feature layer definition for the new selections
                    IFeatureLayerDefinition noBoundaryFLD = geolineFL as IFeatureLayerDefinition;

                    //Get a query based on undefined order list
                    IDataset geolineFLDataset = geolineFL.FeatureClass as IDataset;
                    string buildnoBoundQuery = GSC_ProjectEditor.Queries.BuildQueryFromStringList(geolineIsBoundaryField, new List<string> { geolineIsBoundaryValueNo }, "String", "OR", geolineFLDataset.Workspace);

                    //Create a new definition query layer
                    IFeatureLayer pinkIsBoundaryLayer = noBoundaryFLD.CreateSelectionLayer(geolinePinkLayerName, false, null, null);

                    //Set the def query afterward, or else it won't show up - ticket 6151
                    IFeatureLayerDefinition secondDef = pinkIsBoundaryLayer as IFeatureLayerDefinition;
                    secondDef.DefinitionExpression = buildnoBoundQuery;

                    //Access other attributes of layer with geofeaturelayer
                    IGeoFeatureLayer pinkIsBoundaryGeoFL = pinkIsBoundaryLayer as IGeoFeatureLayer;

                    //Get a renderer
                    List<int> fillColor = new List<int>(new int[] { 255, 0, 197 });
                    IFeatureRenderer pinkRenderer = GSC_ProjectEditor.Symbols.GetSimpleLineRenderer(fillColor, 2.0) as IFeatureRenderer;

                    //Set renderer into layer
                    pinkIsBoundaryGeoFL.Renderer = pinkRenderer;

                    //Add new layer to Arc Map
                    Utilities.MapDocumentFeatureLayers.AddLayerToArcMapPreProcessing(pinkIsBoundaryGeoFL as ILayer, GSC_ProjectEditor.Properties.Resources.GroupLayerValidation);
                }

                #endregion

                #region Manage boundary to yes

                if (blueYesList.Count != 0)
                {
                    //Create a new feature layer definition for the new selections
                    IFeatureLayerDefinition isBoundaryFLD = geolineFL as IFeatureLayerDefinition;

                    //Get a query based on undefined order list
                    IDataset geolineFLDataset = geolineFL.FeatureClass as IDataset;
                    string buildisBoundQuery = GSC_ProjectEditor.Queries.BuildQueryFromStringList(geolineIsBoundaryField, new List<string> { geolineIsBoundaryValueYes }, "String", "OR", geolineFLDataset.Workspace);

                    //Create a new definition query layer
                    IFeatureLayer blueIsBoundaryLayer = isBoundaryFLD.CreateSelectionLayer(geolineBlueLayerName, false, null, null);

                    //Set the def query afterward, or else it won't show up - ticket 6151
                    IFeatureLayerDefinition thirdDef = blueIsBoundaryLayer as IFeatureLayerDefinition;
                    thirdDef.DefinitionExpression = buildisBoundQuery;

                    //Access other attributes of layer with geofeaturelayer
                    IGeoFeatureLayer blueIsBoundaryGeoFL = blueIsBoundaryLayer as IGeoFeatureLayer;

                    //Get a renderer
                    List<int> fillColorBlue = new List<int>(new int[] { 0, 0, 255 });
                    IFeatureRenderer blueRenderer = GSC_ProjectEditor.Symbols.GetSimpleLineRenderer(fillColorBlue, 2.0) as IFeatureRenderer;

                    //Set renderer into layer
                    blueIsBoundaryGeoFL.Renderer = blueRenderer;

                    //Add new layer to Arc Map
                    Utilities.MapDocumentFeatureLayers.AddLayerToArcMapPreProcessing(blueIsBoundaryGeoFL as ILayer, GSC_ProjectEditor.Properties.Resources.GroupLayerValidation);
                }

                #endregion
            }
            catch(Exception flashBoundaryExcept)
            {
                MessageBox.Show(flashBoundaryExcept.StackTrace);
            }


        }

        /// <summary>
        /// Will highlight all values for confidence field.
        /// </summary>
        public void FlashConfidence()
        {

            #region Variable creation and assignment
            //Access geoline feature
            IFeatureLayer geolineFL = Utilities.MapDocumentFeatureLayers.GetFeatureLayerPreProcessing(geolineFeature, GSC_ProjectEditor.Properties.Resources.GroupLayerInterpretation);

            #endregion

            #region Remove any existing result layer from TOC.

            removeOldValidationLayer(new List<string> { geolineAproxName, geolineConcealedName, geolineConfDefName, geolineNotAppName, geolineInferedName });

            #endregion

            //Create a new feature from selection
            IFeatureLayerDefinition selectionFLD = geolineFL as IFeatureLayerDefinition;

            #region Manage defined confidence
            string confDefQuery = "\"" + geolineConfidenceField + "\" = '" + geolineConfidenceDefined + "'";
            IFeatureLayer greenConfLayer = selectionFLD.CreateSelectionLayer(geolineConfDefName, false, null, null);

            //Set the def query afterward, or else it won't show up - ticket 6151
            IFeatureLayerDefinition secondDef = greenConfLayer as IFeatureLayerDefinition;
            secondDef.DefinitionExpression = confDefQuery;

            //Access other attributes of layer with geofeaturelayer
            IGeoFeatureLayer greenConfGeolayer = greenConfLayer as IGeoFeatureLayer;

            //Get a renderer
            List<int> greenFillColor = new List<int>(new int[] { 0, 255, 0 });
            IFeatureRenderer greenRenderer = GSC_ProjectEditor.Symbols.GetSimpleLineRenderer(greenFillColor, 2.0) as IFeatureRenderer;

            //Set renderer into layer
            greenConfGeolayer.Renderer = greenRenderer;

            //Add new layer to Arc Map
            Utilities.MapDocumentFeatureLayers.AddLayerToArcMapPreProcessing(greenConfGeolayer as ILayer, GSC_ProjectEditor.Properties.Resources.GroupLayerValidation);

            #endregion

            #region Manage approximate confidence

            //Create a new feature from selection
            string confApproxQuery = "\"" + geolineConfidenceField + "\" = '" + geolineConfidenceApprox + "'";
            IFeatureLayer yellowConfLayer = selectionFLD.CreateSelectionLayer(geolineAproxName, false, null, null);

            //Set the def query afterward, or else it won't show up - ticket 6151
            IFeatureLayerDefinition thirdDef = yellowConfLayer as IFeatureLayerDefinition;
            thirdDef.DefinitionExpression = confApproxQuery;

            //Access other attributes of layer with geofeaturelayer
            IGeoFeatureLayer yellowConfGeoLayer = yellowConfLayer as IGeoFeatureLayer;

            //Get a renderer
            List<int> yellowFillColor = new List<int>(new int[] { 255, 255, 115 });
            IFeatureRenderer yellowRenderer = GSC_ProjectEditor.Symbols.GetSimpleLineRenderer(yellowFillColor, 2.0) as IFeatureRenderer;

            //Set renderer into layer
            yellowConfGeoLayer.Renderer = yellowRenderer;

            //Add new layer to Arc Map
            Utilities.MapDocumentFeatureLayers.AddLayerToArcMapPreProcessing(yellowConfGeoLayer as ILayer, GSC_ProjectEditor.Properties.Resources.GroupLayerValidation);

            #endregion

            #region Manage Inferred Confidence

            //Create a new feature from selection
            string confInfQuery = "\"" + geolineConfidenceField + "\" = '" + geolineConfidenceInf + "'";
            IFeatureLayer orangeConfLayer = selectionFLD.CreateSelectionLayer(geolineInferedName, false, null, null);

            //Set the def query afterward, or else it won't show up - ticket 6151
            IFeatureLayerDefinition fourthDef = orangeConfLayer as IFeatureLayerDefinition;
            fourthDef.DefinitionExpression = confInfQuery;

            //Access other attributes of layer with geofeaturelayer
            IGeoFeatureLayer orangeConfGeoLayer = orangeConfLayer as IGeoFeatureLayer;

            //Get a renderer
            List<int> orangeFillColor = new List<int>(new int[] { 253, 106, 2 });
            IFeatureRenderer orangeRenderer = GSC_ProjectEditor.Symbols.GetSimpleLineRenderer(orangeFillColor, 2.0) as IFeatureRenderer;

            //Set renderer into layer
            orangeConfGeoLayer.Renderer = orangeRenderer;

            //Add new layer to Arc Map
            Utilities.MapDocumentFeatureLayers.AddLayerToArcMapPreProcessing(orangeConfGeoLayer as ILayer, GSC_ProjectEditor.Properties.Resources.GroupLayerValidation);

            #endregion

            #region Manage Concealed confidence

            //Create a new feature from selection
            string confConcealedQuery = "\"" + geolineConfidenceField + "\" = '" + geolineConfidenceConcealed + "'";
            IFeatureLayer grayConfLayer = selectionFLD.CreateSelectionLayer(geolineConcealedName, false, null, confConcealedQuery);

            //Set the def query afterward, or else it won't show up - ticket 6151
            IFeatureLayerDefinition fifthDef = grayConfLayer as IFeatureLayerDefinition;
            fifthDef.DefinitionExpression = confConcealedQuery;

            //Access other attributes of layer with geofeaturelayer
            IGeoFeatureLayer grayConfGeoLayer = grayConfLayer as IGeoFeatureLayer;

            //Get a renderer
            List<int> grayFillColor = new List<int>(new int[] { 200, 200, 200 });
            IFeatureRenderer grayRenderer = GSC_ProjectEditor.Symbols.GetSimpleLineRenderer(grayFillColor, 2.0) as IFeatureRenderer;

            //Set renderer into layer
            grayConfGeoLayer.Renderer = grayRenderer;

            //Add new layer to Arc Map
            Utilities.MapDocumentFeatureLayers.AddLayerToArcMapPreProcessing(grayConfGeoLayer as ILayer, GSC_ProjectEditor.Properties.Resources.GroupLayerValidation);

            #endregion

            #region Manage Not Applicable confidence

            //Create a new feature from selection
            string confNotAppQuery = "\"" + geolineConfidenceField + "\" = '" + geolineConfidenceNotApp + "'";
            IFeatureLayer redConfLayer = selectionFLD.CreateSelectionLayer(geolineNotAppName, false, null, confNotAppQuery);

            //Set the def query afterward, or else it won't show up - ticket 6151
            IFeatureLayerDefinition sixthDef = redConfLayer as IFeatureLayerDefinition;
            sixthDef.DefinitionExpression = confNotAppQuery;

            //Access other attributes of layer with geofeaturelayer
            IGeoFeatureLayer redConfGeoLayer = redConfLayer as IGeoFeatureLayer;

            //Get a renderer
            List<int> redFillColor = new List<int>(new int[] { 255, 0, 0 });
            IFeatureRenderer redRenderer = GSC_ProjectEditor.Symbols.GetSimpleLineRenderer(redFillColor, 2.0) as IFeatureRenderer;

            //Set renderer into layer
            redConfGeoLayer.Renderer = redRenderer;

            //Add new layer to Arc Map
            Utilities.MapDocumentFeatureLayers.AddLayerToArcMapPreProcessing(redConfGeoLayer as ILayer, GSC_ProjectEditor.Properties.Resources.GroupLayerValidation);

            #endregion

        }

        /// <summary>
        /// Will find any field station xy attributes that isn't the same as the one stored in the real geometry
        /// </summary>
        public void ValidateXYvsGeometry()
        {
            #region Variable creation and assignment
            //Access F_STATION feature
            IFeatureLayer fieldStationLayer = Utilities.MapDocumentFeatureLayers.GetFeatureLayerPreProcessing(fieldStationFeature, GSC_ProjectEditor.Properties.Resources.GroupLayerFieldWork);
            bool checkForStation = false; //Will be used to know if any stations are within feature
            List<string> invalidEastNorthOIDList = new List<string>(); //Will contain any OID for mismatch between easting and northing;
            List<string> invalidLatLongOIDList = new List<string>(); //Will contain any OID for mismatch between latitude and longitude;
            #endregion

            #region Remove any existing result layer from TOC.

            removeOldValidationLayer(new List<string> { fieldStationENLayer, fieldStationLLLayer });

            #endregion

            #region Process XY by accessing geometry
            
            //Iterate with a cursor through stations
            IFeatureCursor stationCursor = GSC_ProjectEditor.FeatureClass.GetFeatureCursor("Search", null, fieldStationFeature);
            IFeature stationRow = stationCursor.NextFeature();
            while ((stationRow = stationCursor.NextFeature()) != null)
            {
                //Checks for existing station
                checkForStation = true;

                //Get current attributes
                int eastingIndex = stationRow.Fields.FindField(fieldStationEasting);
                int northingIndex = stationRow.Fields.FindField(fieldStationNorthing);
                int latIndex = stationRow.Fields.FindField(fieldStationLat);
                int longIndex = stationRow.Fields.FindField(fieldStationLong);
                int OIDIndex = stationRow.Fields.FindField(objectidField);

                double currentEasting = Convert.ToDouble(stationRow.get_Value(eastingIndex));
                double currentNorthing = Convert.ToDouble(stationRow.get_Value(northingIndex));
                double currentLat = Convert.ToDouble(stationRow.get_Value(latIndex));
                double currentLong = Convert.ToDouble(stationRow.get_Value(longIndex));
                string currentOID = stationRow.get_Value(OIDIndex).ToString();

                //Get geometry
                IGeometry stationGeom = stationRow.Shape;
                IGeometry stationGeom2 = stationRow.ShapeCopy;

                //Spatial reference and type
                ISpatialReference3 stationSR = stationGeom.SpatialReference as ISpatialReference3;

                #region Validate easthing / northing
                if (stationSR is IProjectedCoordinateSystem)
                {
                    //Get a point object
                    IPoint pointObject = stationGeom as IPoint;
                    double pointX = pointObject.X;
                    double pointY = pointObject.Y;

                    //Validate X coordinate
                    if (Math.Round(pointX, 2) != Math.Round(currentEasting, 2))
                    {
                        invalidEastNorthOIDList.Add(currentOID);
                    }

                    //Validate Y coordinate
                    if (Math.Round(pointY, 2) != Math.Round(currentNorthing, 2))
                    {
                        invalidEastNorthOIDList.Add(currentOID);
                    }

                }
                #endregion

                #region Project current spatial reference geometry to geographic data
                ISpatialReferenceFactory getSpatFactory = new SpatialReferenceEnvironment();
                IGeographicCoordinateSystem newGeoCoord = null;

                if (stationSR.Name.Contains("NAD") && stationSR.Name.Contains("83"))
                {
                    newGeoCoord = getSpatFactory.CreateGeographicCoordinateSystem((int)esriSRGeoCSType.esriSRGeoCS_NAD1983);
                }
                else if (stationSR.Name.Contains("WGS") && stationSR.Name.Contains("84"))
                {
                    newGeoCoord = getSpatFactory.CreateGeographicCoordinateSystem((int)esriSRGeoCSType.esriSRGeoCS_WGS1984);
                }

                stationGeom2.Project((ISpatialReference)newGeoCoord);

                #endregion

                #region Validate lat/long coordinates
                //Get a point object
                IPoint geographicPointObject = stationGeom2 as IPoint;

                //Project the point
                geographicPointObject.Project((ISpatialReference)newGeoCoord);
                geographicPointObject.SnapToSpatialReference();
                //Get the new coordinates
                double geoPointX = geographicPointObject.X;
                double geoPointY = geographicPointObject.Y;

                //Validate X coordinate
                if (Math.Round(geoPointX, 4) != Math.Round(currentLong, 4))
                {
                    
                    invalidLatLongOIDList.Add(currentOID);
                }

                //Validate Y coordinate
                if (Math.Round(geoPointY, 4) != Math.Round(currentLat, 4))
                {
                    invalidLatLongOIDList.Add(currentOID);
                }
                #endregion


            }

            //Release cursor
            System.Runtime.InteropServices.Marshal.ReleaseComObject(stationCursor);

            #endregion

            #region Parse result

            //If no stations exists
            if (checkForStation == true)
            {
                if (invalidEastNorthOIDList.Count != 0)
                {
                    //Get a selection layer from yellowlist
                    IFeatureLayer fieldStationSelectionLayer = GSC_ProjectEditor.FeatureLayers.CreateFeatureLayerFromSelection(fieldStationLayer, fieldStationFeature, invalidEastNorthOIDList, objectidField, fieldStationENLayer, true);

                    //Access other attributes of layer with geofeaturelayer
                    IGeoFeatureLayer getStationSelectionGeoLayer = fieldStationSelectionLayer as IGeoFeatureLayer;

                    //Get a renderer
                    List<int> fillColor = new List<int>(new int[] { 255, 0, 0 });
                    IFeatureRenderer stationRenderer = GSC_ProjectEditor.Symbols.GetPointRenderer(fillColor, 2.0) as IFeatureRenderer;

                    //Set renderer into layer
                    getStationSelectionGeoLayer.Renderer = stationRenderer;

                    //Add new layer to Arc Map
                    Utilities.MapDocumentFeatureLayers.AddLayerToArcMapPreProcessing(getStationSelectionGeoLayer as ILayer, GSC_ProjectEditor.Properties.Resources.GroupLayerValidation);

                }
                else
                {
                    MessageBox.Show(Properties.Resources.Warning_QC_NoStationXYMismatch);
                }

                if (invalidLatLongOIDList.Count != 0)
                {
                    //Get a selection layer from yellowlist
                    IFeatureLayer fieldStationSelectionLayerLL = GSC_ProjectEditor.FeatureLayers.CreateFeatureLayerFromSelection(fieldStationLayer, fieldStationFeature, invalidLatLongOIDList, objectidField, fieldStationLLLayer, true);

                    //Access other attributes of layer with geofeaturelayer
                    IGeoFeatureLayer getStationSelectionLLGeoLayer = fieldStationSelectionLayerLL as IGeoFeatureLayer;

                    //Get a renderer
                    List<int> fillColor = new List<int>(new int[] { 255, 0, 0 });
                    IFeatureRenderer stationRendererLL = GSC_ProjectEditor.Symbols.GetPointRenderer(fillColor, 2.0) as IFeatureRenderer;

                    //Set renderer into layer
                    getStationSelectionLLGeoLayer.Renderer = stationRendererLL;

                    //Add new layer to Arc Map
                    Utilities.MapDocumentFeatureLayers.AddLayerToArcMapPreProcessing(getStationSelectionLLGeoLayer as ILayer, GSC_ProjectEditor.Properties.Resources.GroupLayerValidation);

                }
                else
                {
                    MessageBox.Show(Properties.Resources.Warning_QC_NoStationXYMismatch);
                }

            }
            else
            {
                MessageBox.Show(Properties.Resources.Warning_QC_NoStations);
            }

            #endregion
        }

        /// <summary>
        /// Will apply a Null value to all empty string attributes
        /// Targets features only
        /// </summary>
        public void ApplyNullValues()
        {
            //Build a list of feature to apply null values in attribute table
            List<string> featureNullList = new List<string>();
            featureNullList.Add(geolineFeature);
            featureNullList.Add(geopolyFeature);
            featureNullList.Add(plabelFeature);
            featureNullList.Add(GSC_ProjectEditor.Constants.Database.gFCStation);
            featureNullList.Add(GSC_ProjectEditor.Constants.Database.gFCLinework);
            featureNullList.Add(GSC_ProjectEditor.Constants.Database.gFCTraverses);
            featureNullList.Add(GSC_ProjectEditor.Constants.Database.FGeopoint);
            featureNullList.Add(GSC_ProjectEditor.Constants.Database.FCartoPoint);
            featureNullList.Add(GSC_ProjectEditor.Constants.Database.FCGMIndex);
            featureNullList.Add(GSC_ProjectEditor.Constants.Database.FStudyArea);

            //Build a list of table to apply null values in attribute table
            List<string> tableNullList = new List<string>();
            tableNullList.Add(GSC_ProjectEditor.Constants.Database.gEarthMath);
            tableNullList.Add(GSC_ProjectEditor.Constants.Database.gMA);
            tableNullList.Add(GSC_ProjectEditor.Constants.Database.gMetadata);
            tableNullList.Add(GSC_ProjectEditor.Constants.Database.gMineral);
            tableNullList.Add(GSC_ProjectEditor.Constants.Database.gPhoto);
            tableNullList.Add(GSC_ProjectEditor.Constants.Database.gSample);
            tableNullList.Add(GSC_ProjectEditor.Constants.Database.gStruc);
            tableNullList.Add(GSC_ProjectEditor.Constants.Database.gSoil);
            tableNullList.Add(GSC_ProjectEditor.Constants.Database.gPFlow);
            tableNullList.Add(GSC_ProjectEditor.Constants.Database.gEnviron);
            tableNullList.Add(GSC_ProjectEditor.Constants.Database.gBiogeo);
            tableNullList.Add(GSC_ProjectEditor.Constants.Database.TGeoEvent);
            tableNullList.Add(GSC_ProjectEditor.Constants.Database.TLegendGene);
            tableNullList.Add(GSC_ProjectEditor.Constants.Database.TLegendDescription);

            //Build dictionnary of process order
            Dictionary<string, List<string>> dicoNullList = new Dictionary<string, List<string>>();
            dicoNullList["Feature"] = featureNullList;
            dicoNullList["Table"] = tableNullList;

            //Process dictionnary based on different keys
            foreach (KeyValuePair<string, List<string>> objectType in dicoNullList)
            {
                if (objectType.Key == "Feature")
                {
                    #region Process all features in list

                    foreach(string getFeatureName in objectType.Value)
                    {
                        try
                        {
                            //Get a feature cursor to update rows from
                            IFeatureCursor getFeatureCursor = GSC_ProjectEditor.FeatureClass.GetFeatureCursor("Update", null, getFeatureName);

                            //Count number of fields, to iterate through them
                            int fieldCounter = getFeatureCursor.Fields.FieldCount;

                            //Get line
                            IFeature getFeat = getFeatureCursor.NextFeature();
                            while (getFeat != null)
                            {
                                for (int i = 0; i < fieldCounter; i++)
                                {
                                    //Get a  nullable string field only
                                    if (getFeatureCursor.Fields.Field[i].Type == esriFieldType.esriFieldTypeString && getFeatureCursor.Fields.Field[i].IsNullable == true)
                                    {
                                        //Trim
                                        string currentValue = getFeat.get_Value(i).ToString();
                                        string trimmedValue = currentValue.Trim();

                                        //Check for empty string or spaces
                                        if (trimmedValue.Length == 0)
                                        {
                                            getFeat.set_Value(i, DBNull.Value); //Use DBNull to get null values within database, string.Empty or null doesn't work.

                                        }
                                    }
                                }

                                getFeatureCursor.UpdateFeature(getFeat);
                                getFeat = getFeatureCursor.NextFeature();
                            }
                            MessageBox.Show(getFeatureName + " was processed.");
                            //Release the cursor or else some lock could happen.
                            System.Runtime.InteropServices.Marshal.ReleaseComObject(getFeatureCursor);
                        }
                        catch (Exception)
                        {

                        }

                    }
                    #endregion
                }

                if(objectType.Key == "Table")
                {
                    #region Process all tables in list
                    foreach(string getTableName in objectType.Value)
                    {
                        try
                        {
                            //Get a feature cursor to update rows from
                            ICursor getTableCursor = GSC_ProjectEditor.Tables.GetTableCursor("Update", null, getTableName);

                            //Count number of fields, to iterate through them
                            int tableFieldCounter = getTableCursor.Fields.FieldCount;

                            //Get line
                            IRow getTableRow = getTableCursor.NextRow();
                            while (getTableRow != null)
                            {
                                for (int i = 0; i < tableFieldCounter; i++)
                                {
                                    //Get a  nullable string field only
                                    if (getTableCursor.Fields.Field[i].Type == esriFieldType.esriFieldTypeString && getTableCursor.Fields.Field[i].IsNullable == true)
                                    {
                                        //Trim
                                        string currentValue = getTableRow.get_Value(i).ToString();
                                        string trimmedValue = currentValue.Trim();

                                        //Check for empty string or spaces
                                        if (trimmedValue.Length == 0)
                                        {
                                            getTableRow.set_Value(i, DBNull.Value); //Use DBNull to get null values within database, string.Empty or null doesn't work.

                                        }
                                    }
                                }

                                getTableCursor.UpdateRow(getTableRow);
                                getTableRow = getTableCursor.NextRow();
                            }

                            MessageBox.Show(getTableName + " was processed.");

                            //Release the cursor or else some lock could happen.
                            System.Runtime.InteropServices.Marshal.ReleaseComObject(getTableCursor);

                            
                        }
                        catch (Exception)
                        {

                        }

                    }
                    #endregion
                }
            }

        }

        /// <summary>
        /// Will create a report on numerical data from all database datasets (features and tables)
        /// </summary>
        public void NumDataStatReport()
        {
            #region Check for report template existance, else create the file from embeded resource
            string workspacePath = GSC_ProjectEditor.Properties.Settings.Default.PROJECT_WORKSPACE_PATH;
            string templateFolderPath = System.IO.Path.Combine(workspacePath, GSC_ProjectEditor.Constants.Folders.mxdFolder);
            string qcNumReportFile = GSC_ProjectEditor.Constants.Templates.qcNumDataReport;
            string reportTemplatePath = System.IO.Path.Combine(templateFolderPath, qcNumReportFile);

            if (!System.IO.File.Exists(reportTemplatePath))
            {
                //Write embeded resource inside template folder of workspaces
                string templateEmbededResourceFolder = GSC_ProjectEditor.Constants.Templates.reportEmbeddedFolder;
                GSC_ProjectEditor.FolderAndFiles.WriteResourceToFile(qcNumReportFile, templateEmbededResourceFolder, GSC_ProjectEditor.Constants.NameSpaces.arcMap, templateFolderPath);
            }
            #endregion

            #region Check for the num data QC table existance, else create it inside working_environment database from embeded resource xml file.
            string workingEnvDBPath = GSC_ProjectEditor.Environments.GetEnvDBPath();
            IWorkspace environmentWorkspace = GSC_ProjectEditor.Workspace.AccessWorkspace(workingEnvDBPath);
            List<string> getEnvironmentData = GSC_ProjectEditor.Workspace.GetDatasetNameListFromWorkspace(environmentWorkspace);
            if (!getEnvironmentData.Contains(GSC_ProjectEditor.Constants.Environment.qcNumData))
            {
                //Write embeded resource to temp file
                string tempFilePath = System.IO.Path.GetTempFileName();
                tempFilePath += ".xml";
                using (StreamWriter outfile = new StreamWriter(tempFilePath)) { outfile.Write(GSC_ProjectEditor.Properties.Resources.GSC_PROJECT_EDITOR_WORKSPACE.ToString()); }

                //Import again working environment schema
                GSC_ProjectEditor.Workspace.ImportXMLWorkspace(environmentWorkspace, tempFilePath);
            }
            #endregion

            //Get the report type wanted by user
            Tuple<esriReportExportType, string> userReportAndExtension = GetUserProjectReportType();

            //Get the project workspace
            IWorkspace projectWorkspace = GSC_ProjectEditor.Workspace.AccessWorkspace(GSC_ProjectEditor.Properties.Settings.Default.PROJECT_DATABASE_PATH);

            //Get a list of relation field keys. Will be used to condition field detection.
            List<string> banFieldList = GSC_ProjectEditor.RelationshipClass.GetListOfRelationFieldKeysFromWorkspace(projectWorkspace);

            #region INIT QC table
            //Get the table
            ITable qcDataNumTable = GSC_ProjectEditor.Tables.OpenTableFromWorkspace(environmentWorkspace, GSC_ProjectEditor.Constants.Environment.qcNumData);

            //Empty feature class of any content
            GSC_ProjectEditor.Tables.EmptyTable(environmentWorkspace, GSC_ProjectEditor.Constants.Environment.qcNumData);

            List<QCNumDataReport> listOfStatistic = new List<QCNumDataReport>();

            #endregion

            #region Iterate through all features

            List<IFeatureClass> projectFCs = GSC_ProjectEditor.FeatureClass.GetFeatureClassListAsFeatureClass(projectWorkspace, null);
            List<IFeatureDataset> projectFDs = GSC_ProjectEditor.FeatureDataset.GetFeatureDatasetList(projectWorkspace);

            Parallel.ForEach(projectFDs, (fds) =>
            {
                projectFCs.AddRange(GSC_ProjectEditor.FeatureClass.GetFeatureClassListAsFeatureClass(projectWorkspace, fds));
            });

            Parallel.ForEach(projectFCs, (fcs) =>
            {
                //Dictionary to hold what will be shown in report
                Dictionary<int, QCNumDataReport> dataReportDictionary = new Dictionary<int, QCNumDataReport>(); //String = Field index

                //Start new numerical data class and fill some information
                IDataset fcDataset = fcs as IDataset;

                //Get a valid field list 
                List<int> validFieldIndexList = GetValidNumericalFields(fcDataset as ITable, banFieldList, true);

                //Iterate through fields of current feature class
                IFeatureCursor currentFCCursor = fcs.Search(null, true);
                IFeature currentFeat = currentFCCursor.NextFeature();

                while (currentFeat != null)
                {

                    //Iterate through valid fields
                    foreach (int fieldIndexes in validFieldIndexList)
                    {
                        QCNumDataReport currentDataReport = new QCNumDataReport();

                        //Get current field
                        IField currentField = currentFeat.Fields.get_Field(fieldIndexes);

                        #region INIT REPORT OBJECT
                        if (dataReportDictionary.ContainsKey(fieldIndexes))
                        {
                            currentDataReport = dataReportDictionary[fieldIndexes];
                        }
                        else
                        {
                            //Set some defaults
                            currentDataReport.datasetName = fcDataset.Name;

                            currentDataReport.fieldName = currentField.AliasName;
                            currentDataReport.fieldValues = new List<object>();
                            if (currentField.Type == esriFieldType.esriFieldTypeDouble)
                            {
                                currentDataReport.fieldType = qcNumFieldTypeDouble;
                                currentDataReport.fieldMin = double.MaxValue;
                                currentDataReport.fieldMax = double.MinValue;
                            }
                            else if (currentField.Type == esriFieldType.esriFieldTypeInteger)
                            {
                                currentDataReport.fieldType = qcNumFieldTypeInteger;
                                currentDataReport.fieldMin = int.MaxValue;
                                currentDataReport.fieldMax = int.MinValue;
                            }
                            else if (currentField.Type == esriFieldType.esriFieldTypeSingle)
                            {
                                currentDataReport.fieldType = qcNumFieldTypeSingle;
                                currentDataReport.fieldMin = Single.MaxValue;
                                currentDataReport.fieldMax = Single.MinValue;
                            }
                            else if (currentField.Type == esriFieldType.esriFieldTypeSmallInteger)
                            {
                                currentDataReport.fieldType = qcNumFieldTypeSmallInt;
                                currentDataReport.fieldMin = int.MaxValue;
                                currentDataReport.fieldMax = int.MinValue;
                            }

                            dataReportDictionary[fieldIndexes] = currentDataReport;
                        }

                        #endregion

                        #region FILL REPORT OBJECT WITH CURRENT VALUES
                        if (currentFeat.get_Value(fieldIndexes) != DBNull.Value && currentFeat.get_Value(fieldIndexes).ToString() != string.Empty)
                        {

                            try
                            {
                                object currentNumValue = currentFeat.get_Value(fieldIndexes);

                                //Add to general list
                                currentDataReport.fieldValues.Add(currentNumValue);

                                //Set min and max values
                                currentDataReport = FindQCNumMinMax(currentDataReport, currentNumValue);

                                dataReportDictionary[fieldIndexes] = currentDataReport;
                            }
                            catch (Exception e)
                            {
                                //MessageBox.Show(e.StackTrace);
                            }



                        }
                        #endregion

                    }

                    currentFeat = currentFCCursor.NextFeature();
                }

                GSC_ProjectEditor.ObjectManagement.ReleaseObject(currentFCCursor);

                foreach (KeyValuePair<int, QCNumDataReport> dr in dataReportDictionary)
                {
                    listOfStatistic.Add(dr.Value);
                }

            });

            #endregion

            #region Iterate through all tables
            List<ITable> projectTables = GSC_ProjectEditor.Tables.GetTableListFromWorkspace(projectWorkspace);

            Parallel.ForEach(projectTables, (tables) =>
            {
                //Dictionary to hold what will be shown in report
                Dictionary<int, QCNumDataReport> dataReportTables = new Dictionary<int, QCNumDataReport>(); //String = Field index

                //Get a valid field list 
                List<int> validTableFieldIndexList = GetValidNumericalFields(tables, banFieldList, false);

                //Start new numerical data class and fill some information
                IDataset tableDataset = tables as IDataset;

                //Iterate through fields of current feature class
                ICursor currentTableCursor = tables.Search(null, true);
                IRow currentRow = currentTableCursor.NextRow();

                while (currentRow != null)
                {

                    //Iterate through valid fields
                    foreach (int fieldIndexes in validTableFieldIndexList)
                    {
                        QCNumDataReport currentDataReport = new QCNumDataReport();

                        //Get current field
                        IField currentField = currentRow.Fields.get_Field(fieldIndexes);

                        #region INIT REPORT OBJECT
                        if (dataReportTables.ContainsKey(fieldIndexes))
                        {
                            currentDataReport = dataReportTables[fieldIndexes];
                        }
                        else
                        {
                            //Set some defaults
                            currentDataReport.datasetName = tableDataset.Name;

                            currentDataReport.fieldName = currentField.Name;
                            currentDataReport.fieldValues = new List<object>();
                            if (currentField.Type == esriFieldType.esriFieldTypeDouble)
                            {
                                currentDataReport.fieldType = qcNumFieldTypeDouble;
                                currentDataReport.fieldMin = double.MaxValue;
                                currentDataReport.fieldMax = double.MinValue;
                            }
                            else if (currentField.Type == esriFieldType.esriFieldTypeInteger)
                            {
                                currentDataReport.fieldType = qcNumFieldTypeInteger;
                                currentDataReport.fieldMin = int.MaxValue;
                                currentDataReport.fieldMax = int.MinValue;
                            }
                            else if (currentField.Type == esriFieldType.esriFieldTypeSingle)
                            {
                                currentDataReport.fieldType = qcNumFieldTypeSingle;
                                currentDataReport.fieldMin = Single.MaxValue;
                                currentDataReport.fieldMax = Single.MinValue;
                            }
                            else if (currentField.Type == esriFieldType.esriFieldTypeSmallInteger)
                            {
                                currentDataReport.fieldType = qcNumFieldTypeSmallInt;
                                currentDataReport.fieldMin = int.MaxValue;
                                currentDataReport.fieldMax = int.MinValue;
                            }

                            dataReportTables[fieldIndexes] = currentDataReport;
                        }

                        #endregion

                        #region FILL REPORT OBJECT WITH CURRENT VALUES
                        if (currentRow.get_Value(fieldIndexes) != DBNull.Value && currentRow.get_Value(fieldIndexes).ToString() != string.Empty)
                        {

                            try
                            {
                                object currentNumValue = currentRow.get_Value(fieldIndexes);

                                //Add to general list
                                currentDataReport.fieldValues.Add(currentNumValue);

                                //Set min and max values
                                currentDataReport = FindQCNumMinMax(currentDataReport, currentNumValue);

                                dataReportTables[fieldIndexes] = currentDataReport;
                            }
                            catch (Exception e)
                            {
                                //MessageBox.Show(e.StackTrace);
                            }



                        }
                        #endregion

                    }

                    currentRow = currentTableCursor.NextRow();
                }

                GSC_ProjectEditor.ObjectManagement.ReleaseObject(currentTableCursor);

                foreach (KeyValuePair<int, QCNumDataReport> drTable in dataReportTables)
                {
                    listOfStatistic.Add(drTable.Value);
                }

            });

            #endregion

            #region Update num table with information
            List<QCNumDataReport> finalReportList = CalculateStats(listOfStatistic);

            ICursor numTableCursor = qcDataNumTable.Insert(true);
            Parallel.ForEach(listOfStatistic, (numDatas) =>
            {
                //Init a new buffer to add value inside
                IRowBuffer newBuffer = qcDataNumTable.CreateRowBuffer();

                //Init fields
                int dsNameFieldIndex = newBuffer.Fields.FindField(GSC_ProjectEditor.Constants.Environment.qcNumData_DSName);
                int fNameFieldIndex = newBuffer.Fields.FindField(GSC_ProjectEditor.Constants.Environment.qcNumData_FName);
                int fTypeFieldIndex = newBuffer.Fields.FindField(GSC_ProjectEditor.Constants.Environment.qcNumData_FType);
                int fMinFieldIndex = newBuffer.Fields.FindField(GSC_ProjectEditor.Constants.Environment.qcNumData_FMin);
                int fMaxFieldIndex = newBuffer.Fields.FindField(GSC_ProjectEditor.Constants.Environment.qcNumData_FMax);
                int fCountFieldIndex = newBuffer.Fields.FindField(GSC_ProjectEditor.Constants.Environment.qcNumData_FCount);
                int fAvgFieldIndex = newBuffer.Fields.FindField(GSC_ProjectEditor.Constants.Environment.qcNumData_FAvg);
                int fStdFieldIndex = newBuffer.Fields.FindField(GSC_ProjectEditor.Constants.Environment.qcNumData_FStrd);

                newBuffer.set_Value(dsNameFieldIndex, numDatas.datasetName);
                newBuffer.set_Value(fNameFieldIndex, numDatas.fieldName);
                newBuffer.set_Value(fTypeFieldIndex, numDatas.fieldType);
                newBuffer.set_Value(fMinFieldIndex, numDatas.fieldMin);
                newBuffer.set_Value(fMaxFieldIndex, numDatas.fieldMax);
                newBuffer.set_Value(fCountFieldIndex, numDatas.fieldValues.Count);
                newBuffer.set_Value(fAvgFieldIndex, numDatas.fieldAvg);
                newBuffer.set_Value(fStdFieldIndex, numDatas.fieldStrd);

                //Insert
                numTableCursor.InsertRow(newBuffer);
            });

            //release
            GSC_ProjectEditor.ObjectManagement.ReleaseObject(numTableCursor);
            #endregion

            #region BUILD REPORT
            string outputReportFile = System.IO.Path.Combine(workspacePath, GSC_ProjectEditor.Constants.Environment.qcNumData + userReportAndExtension.Item2);
            GSC_ProjectEditor.Reports.GenerateReport(qcDataNumTable, outputReportFile, userReportAndExtension.Item1, reportTemplatePath);
            #endregion

        }

        /// <summary>
        /// Will highlight lines underneath a selected feature layer from table of content.
        /// </summary>
        public void FindConcealedUnderOverprints()
        {
            try
            {
                #region Variable creation and assignment

                //Access some feature layers
                IFeatureLayer geolineFL = Utilities.MapDocumentFeatureLayers.GetFeatureLayerPreProcessing(geolineFeature, GSC_ProjectEditor.Properties.Resources.GroupLayerInterpretation);
                ILayer currentSelectedLayer = ArcMap.Document.SelectedLayer;
                IFeatureLayer geopolyFL = (IFeatureLayer)currentSelectedLayer;

                //Make sure user as selected a proper layer
                if (geopolyFL != null && geopolyFL.FeatureClass.ShapeType == esriGeometryType.esriGeometryPolygon)
                {
                    //Create filters for cursor
                    IQueryFilter overlayQuery = new QueryFilter();

                    //overlayQuery.WhereClause = overlayUnitsQuery;
                    ISpatialFilter spatialQuery = new SpatialFilter();

                    //Empty list to contain objectIDs
                    List<string> greenFullIntersectList = new List<string>();
                    List<string> orangeTouchesList = new List<string>();

                    //Other
                    bool overlayBreaker = false; //Will be used to find if any overlay unit exists and show a warning

                    #endregion

                    #region Remove any existing result layer from TOC.

                    removeOldValidationLayer(new List<string> { geolineConcealedOverprintFull, geolineConcealedOverprintsTouches });

                    #endregion

                    #region Manage boundaries
                    //MAIN PROCESS => Iterate through all geopolys
                    IFeatureCursor polyCursor = geopolyFL.Search(overlayQuery, false);
                    IFeature polyFeat = null;

                    while ((polyFeat = polyCursor.NextFeature()) != null)
                    {
                        overlayBreaker = true; //Found some overlays

                        //Get field indexex
                        int geolineIndex = polyFeat.Fields.FindField(Constants.DatabaseFields.FGeolineID);
                        int geolineObjectIDIndex = polyFeat.Fields.FindField(objectidField);

                        spatialQuery.Geometry = polyFeat.Shape; //Add current feature polygon geometry to spatialQuery
                        spatialQuery.GeometryField = geolineFL.FeatureClass.ShapeFieldName; //Add shape field name of feature class querying against  (geolines)
                        spatialQuery.SpatialRel = esriSpatialRelEnum.esriSpatialRelContains;

                        #region iterate through geolines that are completely within current polygon
                        //Iterate through geolines, with an applied spatial query into it
                        IFeatureCursor geolineCursor = geolineFL.Search((IQueryFilter)spatialQuery, false);
                        IFeature geolineFeat = null;

                        while ((geolineFeat = geolineCursor.NextFeature()) != null)
                        {
                            //Get label and OID field value
                            object geolineValue = geolineFeat.get_Value(geolineObjectIDIndex);

                            //Parse result
                            if (geolineValue != null && !greenFullIntersectList.Contains(geolineValue.ToString()) && geolineValue.ToString() != "")
                            {
                                //Add new value to list
                                greenFullIntersectList.Add(geolineValue.ToString());
                            }

                        }

                        //Release cursor
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(geolineCursor);

                        #endregion

                        #region iterate through geolines that crosses current poly
                        spatialQuery.SpatialRel = esriSpatialRelEnum.esriSpatialRelCrosses;

                        //Iterate through geolines, with an applied spatial query into it
                        IFeatureCursor geolineOrangeCursor = geolineFL.Search((IQueryFilter)spatialQuery, false);
                        IFeature geolineOrangeFeat = null;

                        while ((geolineOrangeFeat = geolineOrangeCursor.NextFeature()) != null)
                        {
                            object geolineOrangeValue = geolineOrangeFeat.get_Value(geolineObjectIDIndex);

                            //Parse result
                            if (geolineOrangeValue != null && !orangeTouchesList.Contains(geolineOrangeValue.ToString()) && geolineOrangeValue.ToString() != "")
                            {
                                //Add new value to list
                                orangeTouchesList.Add(geolineOrangeValue.ToString());
                            }

                        }

                        //Release cursor
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(geolineOrangeCursor);

                        #endregion
                    }

                    //Release poly cursor
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(polyCursor);

                    #endregion

                    if (overlayBreaker)
                    {
                        #region Parse No boundaries
                        if (greenFullIntersectList.Count != 0)
                        {
                            //Select all detected lines
                            IFeatureLayer withinGeolines = GSC_ProjectEditor.FeatureLayers.CreateFeatureLayerFromSelection(geolineFL, Constants.Database.FGeoline, greenFullIntersectList, objectidField, geolineConcealedOverprintFull, true);

                            //Access other attributes of layer with geofeaturelayer
                            IGeoFeatureLayer getGeoWithinGeolines = withinGeolines as IGeoFeatureLayer;

                            //Get a renderer
                            List<int> greenLabelFillColor = new List<int>(new int[] { 0, 255, 0 });
                            IFeatureRenderer greenRenderer = GSC_ProjectEditor.Symbols.GetSimpleLineRenderer(greenLabelFillColor, 2.0) as IFeatureRenderer;

                            //Set renderer into layer
                            getGeoWithinGeolines.Renderer = greenRenderer;

                            //Add new layer to Arc Map
                            Utilities.MapDocumentFeatureLayers.AddLayerToArcMapPreProcessing(getGeoWithinGeolines as ILayer, GSC_ProjectEditor.Properties.Resources.GroupLayerValidation);
                        }
                        else
                        {
                            GSC_ProjectEditor.Messages.ShowGenericWarning(Properties.Resources.Warning_QC_ConcealedLinesEmptyWithinLayer);
                        }

                        #endregion

                        #region Manage result

                        if (orangeTouchesList.Count != 0)
                        {
                            //Select all dtected lines
                            IFeatureLayer crossesGeolines = GSC_ProjectEditor.FeatureLayers.CreateFeatureLayerFromSelection(geolineFL, Constants.Database.FGeoline, orangeTouchesList, objectidField, geolineConcealedOverprintsTouches, true);

                            //Access other attributes of layer with geofeaturelayer
                            IGeoFeatureLayer getGeoCrossesGeolines = crossesGeolines as IGeoFeatureLayer;

                            //Get a renderer
                            List<int> orangeLabelFillColor = new List<int>(new int[] { 253, 106, 2 });
                            IFeatureRenderer orangeRenderer = GSC_ProjectEditor.Symbols.GetSimpleLineRenderer(orangeLabelFillColor, 2.0) as IFeatureRenderer;

                            //Set renderer into layer
                            getGeoCrossesGeolines.Renderer = orangeRenderer;

                            //Add new layer to Arc Map
                            Utilities.MapDocumentFeatureLayers.AddLayerToArcMapPreProcessing(getGeoCrossesGeolines as ILayer, GSC_ProjectEditor.Properties.Resources.GroupLayerValidation);
                        }
                        else
                        {
                            GSC_ProjectEditor.Messages.ShowGenericWarning(Properties.Resources.Warning_QC_ConcealedLinesEmptyTouchLayer);
                        }


                        #endregion
                    }
                    else
                    {
                        GSC_ProjectEditor.Messages.ShowGenericErrorMessage(Properties.Resources.Error_QC_ConcealedLinesEmptySelectedLayer);
                    }
                }
                else
                {
                    GSC_ProjectEditor.Messages.ShowGenericErrorMessage(Properties.Resources.Error_QC_ConcealedLinesNoSelectedLayer);
                }



            }
            catch (Exception flashBoundaryExcept)
            {
                MessageBox.Show(flashBoundaryExcept.StackTrace);
            }
        }

        #endregion

        #region QC BEDROCK

        /// <summary>
        /// Will highlight all oriented geolines that should be revised based on map unit age.
        /// </summary>
        public void ValidateLineVSAge()
        {
            try
            {
                #region Variable creation and assignment

                //Access geoline feature
                IFeatureLayer geolineFL = Utilities.MapDocumentFeatureLayers.GetFeatureLayerPreProcessing(geolineFeature, GSC_ProjectEditor.Properties.Resources.GroupLayerInterpretation);
                IFeatureLayer geopolyFL = Utilities.MapDocumentFeatureLayers.GetFeatureLayerPreProcessing(geopolyFeature, GSC_ProjectEditor.Properties.Resources.GroupLayerInterpretation);
                IFeatureClass geolineFC = GSC_ProjectEditor.FeatureClass.OpenFeatureClass(geolineFeature);

                //Temp names
                string step1 = inMemoryWorkspace + "\\step1";

                //Tuple(name, field type, isNullable, AliasName, default value, editable, length)
                Tuple<string, esriFieldType, bool, string, object, bool, object> rightFieldOrder = new Tuple<string, esriFieldType, bool, string, object, bool, object>("rightOrder", esriFieldType.esriFieldTypeDouble, true, "rightOrder", null, true, null);
                Tuple<string, esriFieldType, bool, string, object, bool, object> leftFieldOrder = new Tuple<string, esriFieldType, bool, string, object, bool, object>("leftOrder", esriFieldType.esriFieldTypeDouble, true, "leftOrder", null, true, null);

                #endregion

                #region Remove any existing result layer from TOC.

                removeOldValidationLayer(new List<string>{geolineMustBeFlip, geolineNoNeighbours});

                #endregion

                #region Manage specific geolines

                string specQuery = geolineType + " = " + geolineContact + " AND " + geolineQualif + " = '" + geolineQualifDefUnconfor + "'";
                IQueryFilter specFilter = new QueryFilterClass();
                specFilter.WhereClause = specQuery;
                IFeatureSelection geolineFS = geolineFL as IFeatureSelection;
                geolineFS.SelectFeatures(specFilter, esriSelectionResultEnum.esriSelectionResultNew, false);

                #endregion

                #region Main process
                if (geolineFS.SelectionSet.Count != 0)
                {
                    #region Identity analysis

                    //Call geoprocessing identity analysis method
                    string outputPath = GSC_ProjectEditor.GeoProcessing.IdentityAnalysis(geolineFL, geopolyFL, step1);

                    //Cast output as feature
                    IFeatureClass step1FC = GSC_ProjectEditor.FeatureClass.OpenFeatureClassFromInMemory(outputPath);

                    #endregion

                    #region Adding new fields

                    //Adding new fields to it
                    GSC_ProjectEditor.Tables.AddField(rightFieldOrder, step1FC as ITable);
                    GSC_ProjectEditor.Tables.AddField(leftFieldOrder, step1FC as ITable);

                    #endregion

                    #region Manage join between polygons and legend table

                    //Validate if a join already exists on geopoly
                    bool hasJoin = GSC_ProjectEditor.Joins.HasJoin(geopolyFL, geopolyLabelField);

                    //Add a join between geopoly and legend generator
                    if (!hasJoin)
                    {
                        //Force a join on the layer
                        IDataset currentDataset = geopolyFL.FeatureClass as IDataset;
                        IWorkspace currentWorkspace = currentDataset.Workspace;
                        GSC_ProjectEditor.Joins.AddJoinsFromExistingRelationship(GSC_ProjectEditor.Constants.Database.rel_tLegend_fGeopoly, currentWorkspace, geopolyFL);
                    }

                    #endregion

                    #region Get a list of undefined polygon order and must be flip geoline feature from proper left and right polygon order

                    List<string> undefinedOrderList = new List<string>();
                    Dictionary<string, Dictionary<string, string>> mustBeFlipRaw = new Dictionary<string, Dictionary<string, string>>();

                    parseGeolineLeftRight(step1FC, rightFieldOrder.Item1, leftFieldOrder.Item1, geopolyFL, out undefinedOrderList, out mustBeFlipRaw);

                    #endregion

                    #region Parse mustBeFlipRaw dictionary to get real geoline that must be flipped

                    List<string> mustBeFlipped =  parseMustBeFlipDictionary(mustBeFlipRaw);

                    #endregion

                    //Create a new feature layer definition for the new selections
                    IFeatureLayerDefinition selectionFLD = geolineFL as IFeatureLayerDefinition;

                    #region Manage undefined order polygons underneath geolines

                    //Get a query based on undefined order list
                    IDataset geolineFLDataset = geolineFL.FeatureClass as IDataset;
                    string buildUndefListQuery = GSC_ProjectEditor.Queries.BuildQueryFromStringList(objectidField, undefinedOrderList, "Int", "OR", geolineFLDataset.Workspace);

                    if (undefinedOrderList.Count !=0)
                    {
                        IFeatureLayer yellowUndefOrder = selectionFLD.CreateSelectionLayer(geolineNoNeighbours, false, null, null);

                        //Set the def query afterward, or else it won't show up - ticket 6151
                        IFeatureLayerDefinition secondDef = yellowUndefOrder as IFeatureLayerDefinition;
                        secondDef.DefinitionExpression = buildUndefListQuery;

                        //Access other attributes of layer with geofeaturelayer
                        IGeoFeatureLayer yellowUndefOrderGeoLayer = yellowUndefOrder as IGeoFeatureLayer;

                        //Get a renderer
                        List<int> yellowFillColor = new List<int>(new int[] { 255, 255, 115 });
                        IFeatureRenderer yellowRenderer = GSC_ProjectEditor.Symbols.GetSimpleLineRenderer(yellowFillColor, 2.0) as IFeatureRenderer;

                        //Set renderer into layer
                        yellowUndefOrderGeoLayer.Renderer = yellowRenderer;

                        //Add new layer to Arc Map
                        Utilities.MapDocumentFeatureLayers.AddLayerToArcMapPreProcessing(yellowUndefOrderGeoLayer as ILayer, GSC_ProjectEditor.Properties.Resources.GroupLayerValidation);
                    }

                    #endregion

                    #region Manage must be flipped geolines

                    //Get a query based on undefined order list
                    string buildMustBeFlipQuery = GSC_ProjectEditor.Queries.BuildQueryFromStringList(objectidField, mustBeFlipped, "Int", "OR", geolineFLDataset.Workspace);

                    if (mustBeFlipped.Count != 0)
                    {
                        IFeatureLayer redFlipped = selectionFLD.CreateSelectionLayer(geolineMustBeFlip, false, null, null);

                        //Set the def query afterward, or else it won't show up - ticket 6151
                        IFeatureLayerDefinition thirdDef = redFlipped as IFeatureLayerDefinition;
                        thirdDef.DefinitionExpression = buildMustBeFlipQuery;

                        //Access other attributes of layer with geofeaturelayer
                        IGeoFeatureLayer redFlippedGeoLayer = redFlipped as IGeoFeatureLayer;

                        //Get a renderer
                        List<int> redFillColor = new List<int>(new int[] { 255, 0, 0 });
                        IFeatureRenderer redRenderer = GSC_ProjectEditor.Symbols.GetSimpleLineRenderer(redFillColor, 2.0) as IFeatureRenderer;

                        //Set renderer into layer
                        redFlippedGeoLayer.Renderer = redRenderer;

                        //Add new layer to Arc Map
                        Utilities.MapDocumentFeatureLayers.AddLayerToArcMapPreProcessing(redFlippedGeoLayer as ILayer, GSC_ProjectEditor.Properties.Resources.GroupLayerValidation);
                    }

                    #endregion

                    #region Remove output

                    GSC_ProjectEditor.FeatureClass.DeleteFeatureClass(step1FC);

                    #endregion

                    if (undefinedOrderList.Count != 0 && mustBeFlipped.Count != 0)
                    {
                        MessageBox.Show(Properties.Resources.Warning_QC_GenericNoProblem);
                    }

                }
                else
                {
                    MessageBox.Show(Properties.Resources.Error_NoSpecificGeolines);
                }
                #endregion


            }
            catch (Exception validateLineVSAgeException)
            {
                int lineNumber = GSC_ProjectEditor.Exceptions.LineNumber(validateLineVSAgeException);
                MessageBox.Show("validateLineVSAgeException (" + lineNumber.ToString() + "): " + validateLineVSAgeException.Message);
            }
        }

        /// <summary>
        /// Will highlight any incoherence in the fault line movement.
        /// </summary>
        public void ValidateFaultMovement()
        {
            try
            {
                #region Variable creation and assignment

                //Access geoline feature
                IFeatureLayer geolineFL = Utilities.MapDocumentFeatureLayers.GetFeatureLayerPreProcessing(geolineFeature, GSC_ProjectEditor.Properties.Resources.GroupLayerInterpretation);

                //Create a new feature layer definition for the new selections
                IFeatureLayerDefinition selectionFLD = geolineFL as IFeatureLayerDefinition;

                //Dictionary to keep geolineIds and symbols
                Dictionary<string, string> geolineSymbols = new Dictionary<string, string>();

                //Output list of ids
                Dictionary<string, List<string>> hangwallList = new Dictionary<string, List<string>>();
                List<string> changeSymbolsList = new List<string>();
                List<string> undefMovement = new List<string>();
                List<string> toValidateMovement = new List<string>();
                hangwallList[geolineHgwallN] = new List<string>();
                hangwallList[geolineHgwallNE] = new List<string>();
                hangwallList[geolineHgwallE] = new List<string>();
                hangwallList[geolineHgwallSE] = new List<string>();
                hangwallList[geolineHgwallS] = new List<string>();
                hangwallList[geolineHgwallSW] = new List<string>();
                hangwallList[geolineHgwallW] = new List<string>();
                hangwallList[geolineHgwallNW] = new List<string>();
                hangwallList[geolineMoveFlip] = new List<string>();

                //Get a list of domain and codes for movement
                Dictionary<string, string> movementDico = GSC_ProjectEditor.Domains.GetDomDico(geolineMovementDom, "Code");

                //A filter for search cursors
                IQueryFilter simpleQuery = new QueryFilter();

                #endregion

                #region Remove any existing result layer from TOC.

                List<string> removeLayerList = new List<string> { geolineMoveCode01, geolineMoveCode02, 
                    geolineMoveCode03, geolineMoveCode04, geolineMoveCode05, 
                    geolineMoveCode06, geolineMoveCode07, geolineMoveCode08, 
                    geolineMoveFlip, geolineMoveMissing, geolineMoveSymChange };

                removeOldValidationLayer(removeLayerList);

                #endregion

                #region Manage specific geolines

                string specQuery = geolineType + " = " + geolineFault + " AND " + geolineQualif + " = '" + geolineFaultNormal + "'";
                IQueryFilter specFilter = new QueryFilterClass();
                specFilter.WhereClause = specQuery;
                IFeatureSelection geolineFS = geolineFL as IFeatureSelection;
                geolineFS.SelectFeatures(specFilter, esriSelectionResultEnum.esriSelectionResultNew, false);

                #endregion

                #region Main process
                if (geolineFS.SelectionSet.Count != 0)
                {
                    #region Get list of all geoline symbols
                    string queryGeoline = tLegendSymType + " = '" + tLegendSymTypeGeoline + "'";
                    Dictionary<string, string> idsAndSymbols = GSC_ProjectEditor.Tables.GetUniqueDicoValues(tLegendGen, tLegendGenLabel, tLegendSymCode, queryGeoline);

                    #endregion

                    #region Build list of associated symbols

                    //Get a cursor
                    ICursor geolineCursorMove;
                    geolineFS.SelectionSet.Search(simpleQuery, true, out geolineCursorMove);

                    //Cast cursor as IFeatureCursor
                    IFeatureCursor moveCursor = geolineCursorMove as IFeatureCursor;

                    //Get some field indexes
                    int geolineIDIndex = moveCursor.Fields.FindField(geolineIDField);
                    int geolineMovementIndex = moveCursor.Fields.FindField(geolineMovement);
                    int geolineOIDIndex = moveCursor.Fields.FindField(objectidField);
                    int geolineHangwallIndex = moveCursor.Fields.FindField(geolineHangwall);

                    //Iterate
                    IFeature nextFeature = moveCursor.NextFeature();
                    while (nextFeature != null)
                    {
                        //Get current geolineid
                        string currentID = nextFeature.get_Value(geolineIDIndex).ToString();
                        string currentOID = nextFeature.get_Value(geolineOIDIndex).ToString();
                        string currentHangwall = nextFeature.get_Value(geolineHangwallIndex).ToString();

                        //Add new values within dico
                        if (idsAndSymbols.ContainsKey(currentID))
                        {
                            //Add to dico
                            geolineSymbols[currentID] = idsAndSymbols[currentID];

                            //Parse geolines that has a movement and needs to change symbols
                            string currentSymbol = idsAndSymbols[currentID];

                            string currentMovement = nextFeature.get_Value(geolineMovementIndex).ToString();

                            if (currentMovement != geolineMovementNA && currentMovement != geolineMovementUndef)
                            {
                                if (!currentSymbol.Contains("02.02"))
                                {
                                    //Add id to list
                                    if (!changeSymbolsList.Contains(currentOID))
                                    {
                                        changeSymbolsList.Add(currentOID);
                                    }

                                }
                                else
                                {
                                    //Find if current hangwall matches bearing calculation
                                    parseHangwallBearing(nextFeature.Shape, currentHangwall, currentOID, hangwallList, out hangwallList);

                                }

                            }

                            //Parse geolines that has no apparent movement, but a good symbols
                            if (currentMovement == geolineMovementNA || currentMovement == geolineMovementUndef)
                            {
                                if (currentSymbol.Contains("02.02"))
                                {

                                    //Add id to list
                                    if (!undefMovement.Contains(currentOID))
                                    {
                                        undefMovement.Add(currentOID);
                                    }
                                }
                            }
                        }

                        nextFeature = moveCursor.NextFeature();
                    }

                    //Release com cursor
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(moveCursor);

                    #endregion

                    #region Manage wrong hangwall code vs real bearing of the line.

                    foreach (KeyValuePair<string, List<string>> kv in hangwallList)
                    {

                        if (kv.Value.Count != 0)
                        {

                            //Get a query based on undefined order list
                            IDataset geolineFLDataset = geolineFL.FeatureClass as IDataset;
                            string buildWrongHgwallQuery = GSC_ProjectEditor.Queries.BuildQueryFromStringList(objectidField, kv.Value, "Int", "OR", geolineFLDataset.Workspace);
                            //MessageBox.Show(buildWrongSymListQuery);

                            //Get validation layer name
                            string validationLayerName = parseOutputLayerNames4Hangwalls(kv.Key, movementDico);
                            IFeatureLayer redWrongSym = selectionFLD.CreateSelectionLayer(validationLayerName, false, null, null);

                            //Set the def query afterward, or else it won't show up - ticket 6151
                            IFeatureLayerDefinition thirdDef = redWrongSym as IFeatureLayerDefinition;
                            thirdDef.DefinitionExpression = buildWrongHgwallQuery;

                            //Access other attributes of layer with geofeaturelayer
                            IGeoFeatureLayer redWrongSymGeoLayer = redWrongSym as IGeoFeatureLayer;

                            //Get a renderer
                            List<int> redFillColor = new List<int>(new int[] { 255, 0, 0 });
                            IFeatureRenderer redRenderer = GSC_ProjectEditor.Symbols.GetSimpleLineRenderer(redFillColor, 2.0) as IFeatureRenderer;

                            //Set renderer into layer
                            redWrongSymGeoLayer.Renderer = redRenderer;

                            //Add new layer to Arc Map
                            Utilities.MapDocumentFeatureLayers.AddLayerToArcMapPreProcessing(redWrongSymGeoLayer as ILayer, GSC_ProjectEditor.Properties.Resources.GroupLayerValidation);
                        }
                    }


                    #endregion

                    #region Manage wrong symbol for normal faults

                    if (changeSymbolsList.Count != 0)
                    {
                        //Get a query based on undefined order list
                        IDataset geolineFLDataset = geolineFL.FeatureClass as IDataset;
                        string buildWrongSymListQuery = GSC_ProjectEditor.Queries.BuildQueryFromStringList(objectidField, changeSymbolsList, "Int", "OR", geolineFLDataset.Workspace);
                        //MessageBox.Show(buildWrongSymListQuery);
                        IFeatureLayer yellowWrongSym = selectionFLD.CreateSelectionLayer(geolineMoveSymChange, false, null, null);

                        //Set the def query afterward, or else it won't show up - ticket 6151
                        IFeatureLayerDefinition fourthDef = yellowWrongSym as IFeatureLayerDefinition;
                        fourthDef.DefinitionExpression = buildWrongSymListQuery;

                        //Access other attributes of layer with geofeaturelayer
                        IGeoFeatureLayer yellowWrongSymGeoLayer = yellowWrongSym as IGeoFeatureLayer;

                        //Get a renderer
                        List<int> yellowFillColor = new List<int>(new int[] { 255, 255, 115 });
                        IFeatureRenderer yellowRenderer = GSC_ProjectEditor.Symbols.GetSimpleLineRenderer(yellowFillColor, 2.0) as IFeatureRenderer;

                        //Set renderer into layer
                        yellowWrongSymGeoLayer.Renderer = yellowRenderer;

                        //Add new layer to Arc Map
                        Utilities.MapDocumentFeatureLayers.AddLayerToArcMapPreProcessing(yellowWrongSymGeoLayer as ILayer, GSC_ProjectEditor.Properties.Resources.GroupLayerValidation);
                    }

                    #endregion

                    #region Manage undefined or not applicable movement

                    if (undefMovement.Count != 0)
                    {
                        //Get a query based on undefined order list
                        IDataset geolineFLDataset = geolineFL.FeatureClass as IDataset;
                        string buildUndefListQuery = GSC_ProjectEditor.Queries.BuildQueryFromStringList(objectidField, undefMovement, "Int", "OR", geolineFLDataset.Workspace);
                        //MessageBox.Show(buildUndefListQuery);
                        IFeatureLayer yellowNoMove = selectionFLD.CreateSelectionLayer(geolineMoveMissing, false, null, null);

                        //Set the def query afterward, or else it won't show up - ticket 6151
                        IFeatureLayerDefinition fifthDef = yellowNoMove as IFeatureLayerDefinition;
                        fifthDef.DefinitionExpression = buildUndefListQuery;

                        //Access other attributes of layer with geofeaturelayer
                        IGeoFeatureLayer yellowNoMoveGeoLayer = yellowNoMove as IGeoFeatureLayer;

                        //Get a renderer
                        List<int> yellowFillColor = new List<int>(new int[] { 255, 255, 115 });
                        IFeatureRenderer yellowRenderer = GSC_ProjectEditor.Symbols.GetSimpleLineRenderer(yellowFillColor, 2.0) as IFeatureRenderer;

                        //Set renderer into layer
                        yellowNoMoveGeoLayer.Renderer = yellowRenderer;

                        //Add new layer to Arc Map
                        Utilities.MapDocumentFeatureLayers.AddLayerToArcMapPreProcessing(yellowNoMoveGeoLayer as ILayer, GSC_ProjectEditor.Properties.Resources.GroupLayerValidation);
                    }

                    #endregion

                }
                else
                {
                    MessageBox.Show(Properties.Resources.Error_NoSpecificGeolines);
                }
                #endregion

            }
            catch (Exception validateMovementFaultExcept)
            {
                int lineNumber = GSC_ProjectEditor.Exceptions.LineNumber(validateMovementFaultExcept);
                MessageBox.Show("validateMovementFaultExcept (" + lineNumber.ToString() + "): " + validateMovementFaultExcept.Message);
            }
        }



        #endregion

        #region METHODS

        /// <summary>
        /// Will find any clicked radio button and invoke method that is stored within control tag.
        /// </summary>
        /// <param name="rbList">A list containing radio button, for iteration purposes</param>
        public void findRadioButton(List<RadioButton> rbList)
        {
            //Iterate through control list
            foreach (RadioButton rb in rbList)
            {
                //Find THE checked radio button
                if (rb.Checked == true)
                {
                    //Try to invoke associated method from radio button tag.
                    try
                    {
                        //Create a methodInfo object to mimic method that reside inside dw window class
                        MethodInfo mi = typeof(Dockablewindow_CreateEdit_QualityControl).GetMethod(rb.Tag.ToString());

                        //Create and cast a new delegate to call associated method
                        rbDelegate rbDel = (rbDelegate)Delegate.CreateDelegate(typeof(rbDelegate), null, mi);

                        //Invoke method
                        rbDel.Invoke();
                        
                    }

                    catch(Exception rbException)
                    {
                        MessageBox.Show(rbException.Message + "\n\n" + rbException.StackTrace.ToString());
                        MessageBox.Show(Properties.Resources.Warning_NotImplemented);
                    }

                }
            }

        }

        /// <summary>
        /// Based on report field type, it will parse current numerical value and try to set min and max values
        /// Needs to cast into float or integer.
        /// </summary>
        /// <param name="inReport">The report object</param>
        /// <param name="inValue">The value to look for min and max</param>
        /// <returns></returns>
        public QCNumDataReport FindQCNumMinMax(QCNumDataReport inReport, object inValue)
        {
            if (inReport.fieldType == qcNumFieldTypeDouble)
            {
                //Calculate min
                if ((Double)inValue < (Double)inReport.fieldMin)
                {
                    inReport.fieldMin = inValue;
                }

                //Calculate max
                if ((Double)inValue > (Double)inReport.fieldMax)
                {
                    inReport.fieldMax = inValue;
                }
            }
            else if (inReport.fieldType == qcNumFieldTypeSingle)
            {
                //Calculate min
                if ((Single)inValue < (Single)inReport.fieldMin)
                {
                    inReport.fieldMin = inValue;
                }

                //Calculate max
                if ((Single)inValue > (Single)inReport.fieldMax)
                {
                    inReport.fieldMax = inValue;
                }
            }
            else if (inReport.fieldType == qcNumFieldTypeSmallInt || inReport.fieldType == qcNumFieldTypeInteger)
            {
                //Calculate min
                if (Convert.ToInt32(inValue.ToString()) < Convert.ToInt32(inReport.fieldMin))
                {
                    inReport.fieldMin = inValue;
                }

                //Calculate max
                if (Convert.ToInt32(inValue.ToString()) > Convert.ToInt32(inReport.fieldMax))
                {
                    inReport.fieldMax = inValue;
                }
            }

            return inReport;
        }

        /// <summary>
        /// Will calculate internal stats coming from it's inner double list of values
        /// </summary>
        /// <param name="inNumDadaClass">The num class to calculate stats from</param>
        /// <returns></returns>
        public List<QCNumDataReport> CalculateStats(List<QCNumDataReport> inReportList)
        {
            //Variables
            List<QCNumDataReport> newReportList = new List<QCNumDataReport>();

            //Iterate through all report objects
            foreach (QCNumDataReport report in inReportList)
            {
                //Variables
                double reportAvg = 0.0;
                double reportStd = 0.0;

                //Get list of all values
                List<object> valueList = report.fieldValues;

                #region CALCULATE AVERAGE
                foreach (object value in valueList)
                {
                    if (report.fieldType == qcNumFieldTypeDouble)
                    {
                        reportAvg = reportAvg + (Double)value;
                    }
                    else if (report.fieldType == qcNumFieldTypeSingle)
                    {
                        reportAvg = reportAvg + (Single)value;
                    }
                    else if (report.fieldType == qcNumFieldTypeInteger || report.fieldType == qcNumFieldTypeSmallInt)
                    {
                        reportAvg = reportAvg + Convert.ToInt32(value.ToString());
                    }
                }

                reportAvg = reportAvg / (Double)report.fieldValues.Count;

                report.fieldAvg = Math.Round(reportAvg,2);
                #endregion

                #region CALCULATE STANDARD DEVIATION
                //Build the addition part of the formula
                foreach (object value2 in valueList)
                {
                    if (report.fieldType == qcNumFieldTypeDouble)
                    {
                        reportStd = reportStd + Math.Pow(((Double)value2 - report.fieldAvg),2);
                    }
                    else if (report.fieldType == qcNumFieldTypeSingle)
                    {
                        reportStd = reportStd + Math.Pow(((Single)value2 - report.fieldAvg), 2);
                    }
                    else if (report.fieldType == qcNumFieldTypeInteger || report.fieldType == qcNumFieldTypeSmallInt)
                    {
                        reportStd = reportStd + Math.Pow((Convert.ToInt32(value2.ToString()) - report.fieldAvg), 2);
                    }
                }

                //Finish the formula
                reportStd = Math.Sqrt(reportStd * (1.0 / (Double)report.fieldValues.Count));
                report.fieldStrd = Math.Round(reportStd,2);

                #endregion

                #region ROUND MIN AND MAX 

                try
                {
                    if (report.fieldType == qcNumFieldTypeDouble)
                    {
                        report.fieldMin = Math.Round((Double)report.fieldMin,4);
                        report.fieldMax = Math.Round((Double)report.fieldMax, 4);
                    }
                    else if (report.fieldType == qcNumFieldTypeSingle)
                    {
                        report.fieldMin = Math.Round((Single)report.fieldMin,4);
                        report.fieldMax = Math.Round((Single)report.fieldMax, 4);
                    }

                }
                catch (Exception)
                {

                }

                #endregion
                newReportList.Add(report);
            }

            return newReportList;
        }

        /// <summary>
        /// Will return a valid list of field indexes that are numeric, not domains, not subtypes and not relation keys.
        /// </summary>
        /// <param name="inFC">The feature class to retrieve the list from</param>
        /// <returns></returns>
        public List<int> GetValidNumericalFields(ITable inFC, List<string> banFieldList, bool isFeatureClass)
        {
            //Variables
            List<int> validFieldIndexList = new List<int>();
            SortedDictionary<string, int> subtypes = new SortedDictionary<string,int>();

            //Get dataset from feature class
            IDataset inFCDataset = inFC as IDataset;

            #region SUBTYPE MANAGEMENT
            if (isFeatureClass)
            {
                //Get subtype from feature if exists
                subtypes = GSC_ProjectEditor.Subtypes.GetSubtypeDico(inFCDataset.Name);

                if (subtypes.Count > 0)
                {
                    ISubtypes currentSub = inFC as ISubtypes;

                    if (!banFieldList.Contains(currentSub.SubtypeFieldName))
                    {
                        banFieldList.Add(currentSub.SubtypeFieldName);
                    }

                } 
            }

            #endregion

            #region FIELD MANAGEMENT

            Parallel.For(0, inFC.Fields.FieldCount, i =>
            {

                //Get field object
                IField currentField = inFC.Fields.get_Field(i);

                //Get domains from subtypes if exists
                if (subtypes.Count > 0)
                {
                    foreach (KeyValuePair<string, int> subDefinition in subtypes)
                    {
                        string currentSubtypeDomain = GSC_ProjectEditor.Domains.GetSubDomName(inFCDataset.Name, subDefinition.Value, currentField.Name);

                        if (currentSubtypeDomain != string.Empty && !banFieldList.Contains(currentField.Name))
                        {
                            banFieldList.Add(currentField.Name);
                            break;
                            
                        }

                    }
                }

                //Remove any ban field from list
                if (banFieldList.Contains(currentField.Name))
                {
                    if (validFieldIndexList.Contains(i))
                    {
                        validFieldIndexList.Remove(i);
                    }
                }

                #region Only treat num data type
                if (currentField.Name != GSC_ProjectEditor.Constants.DatabaseFields.ObjectID && !banFieldList.Contains(currentField.Name)) //No OID fields...
                {
                    if (currentField.Domain == null) //No Field with domains...
                    {
                        if (currentField.Type == esriFieldType.esriFieldTypeDouble)
                        {
                            validFieldIndexList.Add(i);
                        }
                        else if (currentField.Type == esriFieldType.esriFieldTypeInteger)
                        {
                            validFieldIndexList.Add(i);
                        }
                        else if (currentField.Type == esriFieldType.esriFieldTypeSingle)
                        {
                            validFieldIndexList.Add(i);
                        }
                        else if (currentField.Type == esriFieldType.esriFieldTypeSmallInteger)
                        {
                            validFieldIndexList.Add(i);
                        }
                        else
                        {
                            banFieldList.Add(currentField.Name);
                        }

                    }
                    else
                    {
                        banFieldList.Add(currentField.Name);
                    }
                }
                else
                {
                    
                }

                #endregion

            });


            #endregion

            return validFieldIndexList;
        }

        /// <summary>
        /// Will return user's chosen report type and file extension
        /// </summary>
        /// <returns></returns>
        public static Tuple<esriReportExportType, string> GetUserProjectReportType()
        {
            //Get user report type
            Dictionary<string, object> reportTypeDico = new Dictionary<string, object>();
            reportTypeDico["Adobe PDF"] = ".pdf";
            reportTypeDico["Web (HTML)"] = ".html";
            reportTypeDico["Rich Text Format (rtf)"] = ".rtf";
            reportTypeDico["Tag Image File Format (tiff)"] = ".tiff";
            reportTypeDico["Text File (txt)"] = ".pdf";
            reportTypeDico["Microsoft Excel"] = ".xls";
            
            string reportExtension = Form_Generic.ShowGenericComboboxForm("Report types", "Select desire output report type: ", null, reportTypeDico).ToString();

            //Get default report
            esriReportExportType reportType = esriReportExportType.esriReportExportPDF;

            //Parse
            if (reportExtension == ".pdf")
            {
                //Default
            }
            else if (reportExtension == ".html")
            {
                reportType = esriReportExportType.esriReportExportHTML;
            }
            else if (reportExtension == ".rtf")
            {
                reportType = esriReportExportType.esriReportExportRTF;
            }
            else if (reportExtension == ".tif" || reportExtension == ".tiff")
            {
                reportType = esriReportExportType.esriReportExportTIFF;
            }
            else if (reportExtension == ".txt")
            {
                reportType = esriReportExportType.esriReportExportTXT;
            }
            else if (reportExtension == ".xls")
            {
                reportType = esriReportExportType.esriReportExportXLS;
            }
            else
            {
                reportExtension = ".pdf";
            }

            //output
            Tuple<esriReportExportType, string> reportConfig = new Tuple<esriReportExportType, string>(reportType, reportExtension);

            return reportConfig;
        }

        /// <summary>
        /// Will removed existing layers from table of content
        /// </summary>
        /// <param name="layerList"></param>
        public void removeOldValidationLayer(List<string> layerList)
        {
            foreach(string layerNames in layerList)
            {
                Utilities.MapDocumentFeatureLayers.RemoveLayerFromArcMapPreProcessing(layerNames);
            }
        }

        /// <summary>
        /// Used for Contact vs Age QC Tool. Will output a list of no order geolines and a dictionary of must be flip geolines for the identity resulting left and right OID field.
        /// </summary>
        /// <param name="identityGeoline"> Input a feature class of the resulting identity process</param>
        /// <param name="leftOrderField"> field name to contain left order number, in geoline feature</param>
        /// <param name="rightOrderField"> field name to contain right order number, in geoline feature</param>
        /// <param name="polygonLayer"> A feature layer object from geopoly feature</param>
        public void parseGeolineLeftRight(IFeatureClass identityGeoline, string rightOrderField, string leftOrderField, IFeatureLayer polygonLayer, out List<string> undefList, out Dictionary<string, Dictionary<string, string>> flipRaw)
        {
            //Out params
            flipRaw = new Dictionary<string, Dictionary<string, string>>();
            undefList = new List<string>();

            //Create a GeoFeaturelayer to be able to make query with join fields
            IGeoFeatureLayer geopolyGFL = polygonLayer as IGeoFeatureLayer;

            //Create filters to query lines and polygons
            ISpatialFilter spatialQuery = new SpatialFilter();
            IQueryFilter simpleQuery = new QueryFilter();

            //Create an update cursor to fill in the two new fields
            IFeatureCursor geolineCursor = identityGeoline.Search(simpleQuery, true); //Pass the previous filter used to make a selection on geoline

            //Get some field indexes
            int rightOrderIndex = geolineCursor.Fields.FindField(rightOrderField);
            int leftOrderIndex = geolineCursor.Fields.FindField(leftOrderField);
            int rightPolyOIDIndex = geolineCursor.Fields.FindField(rightPolyOID);
            int leftPolyOIDIndex = geolineCursor.Fields.FindField(leftPolyOID);
            int geolineOIDIndex = geolineCursor.Fields.FindField(objectidField);

            //Iterate
            IFeature getGeoline = geolineCursor.NextFeature();

            while (getGeoline != null)
            {
                //Get right and left OID of polygons from geoline
                string currentRightPolyOID = getGeoline.get_Value(rightPolyOIDIndex).ToString();
                string currentLeftPolyOID = getGeoline.get_Value(leftPolyOIDIndex).ToString();
                string currentGeolineOID = getGeoline.get_Value(geolineOIDIndex).ToString();

                //Add new key to must be flip dictionary
                if (!flipRaw.ContainsKey(currentGeolineOID))
                {
                    flipRaw[currentGeolineOID] = new Dictionary<string, string>();
                }

                //Perform a spatial query on the polygons
                spatialQuery.Geometry = getGeoline.Shape;
                spatialQuery.GeometryField = polygonLayer.FeatureClass.ShapeFieldName; //Add shape field name of feature class querying against  (polygon)
                spatialQuery.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;//esriSpatialRelEnum.esriSpatialRelIntersects; //A simple intersection query

                //Iterate through intersected polygons
                IFeatureCursor geopolyCursor = geopolyGFL.SearchDisplayFeatures(spatialQuery as IQueryFilter, true);

                //Get some field indexes
                ILayerFields getLayerFields = geopolyGFL as ILayerFields; //Access this object to get index with a join inside feature layer
                string orderJoinName = tLegendGen + "." + tLegendGenOrder;
                string oidJoinName = geopolyFeature + "." + objectidField;
                int OIDIndex = getLayerFields.FindField(oidJoinName);
                int orderIndex = getLayerFields.FindField(orderJoinName);

                IFeature geopoly = geopolyCursor.NextFeature();
                while (geopoly != null)
                {
                    //Get field values
                    string geopolyOID = geopoly.get_Value(OIDIndex).ToString();
                    object geopolyOrderObj = geopoly.get_Value(orderIndex);
                    string geopolyOrder = geopoly.get_Value(orderIndex).ToString();

                    //Get order
                    if (geopolyOID == currentLeftPolyOID)
                    {
                        ////Update geoline with order
                        //getGeoline.set_Value(leftOrderIndex, geopolyOrder);
                        //geolineCursor.UpdateFeature(getGeoline);
                        flipRaw[currentGeolineOID]["Left"] = geopolyOrder;
                    }

                    if (geopolyOID == currentRightPolyOID)
                    {
                        ////Update geoline with order
                        //getGeoline.set_Value(rightOrderIndex, geopolyOrder);
                        //geolineCursor.UpdateFeature(getGeoline);
                        flipRaw[currentGeolineOID]["Right"] = geopolyOrder;
                    }

                    if (geopoly.get_Value(orderIndex) == DBNull.Value)
                    {
                        if (!undefList.Contains(currentGeolineOID))
                        {
                            undefList.Add(currentGeolineOID);
                        }
                    }

                    //Next polygon
                    geopoly = geopolyCursor.NextFeature();
                }

                //Release com cursor
                System.Runtime.InteropServices.Marshal.ReleaseComObject(geopolyCursor);

                //Next line
                getGeoline = geolineCursor.NextFeature();
            }

            //Release com cursor
            System.Runtime.InteropServices.Marshal.ReleaseComObject(geolineCursor);


        }

        /// <summary>
        /// Will return a dictionary with real values of OID that must be flipped
        /// </summary>
        public List<string> parseMustBeFlipDictionary(Dictionary<string, Dictionary<string, string>> mustBeFlipRaw)
        {
            //output
            List<string> mustBeFlip = new List<string>();

            foreach (KeyValuePair<string, Dictionary<string, string>> kv in mustBeFlipRaw)
            { 
                //Get value dico
                Dictionary<string, string> currentDico = kv.Value;

                if (currentDico["Left"][0] < currentDico["Right"][0])
                {
                    mustBeFlip.Add(kv.Key);
                }
            }

            return mustBeFlip;

        }

        /// <summary>
        /// Will return the true hangwall code based on a bearing calculation
        /// </summary>
        public void parseHangwallBearing(IGeometry inputGeom, string currentHangwall, string inOID, Dictionary<string,List<string>> hangwallListIn, out Dictionary<string,List<string>> hangwallListOut)
        {
            //Variables
            hangwallListOut = hangwallListIn;

            //Calculate bearing
            double getBearing = GSC_ProjectEditor.Geometry.CalculateBearing(inputGeom);
            //MessageBox.Show(getBearing.ToString());
            //Parse result to return right code
            if (getBearing > 337.5 && getBearing <= 22.5)
            {
                if (currentHangwall != geolineHgwallN)
                {
                    //Parse line that only needs a flip
                    if (currentHangwall == geolineHgwallS)
                    {
                        hangwallListOut[geolineMoveFlip].Add(inOID);
                    }
                    else
                    {
                        hangwallListOut[geolineHgwallN].Add(inOID);
                    }
                    
                }
            }

            if (getBearing > 22.5 && getBearing <= 67.5)
            {
                if (currentHangwall != geolineHgwallNE)
                {
                    //Parse line that only needs a flip
                    if (currentHangwall == geolineHgwallSW)
                    {
                        hangwallListOut[geolineMoveFlip].Add(inOID);
                    }
                    else
                    {
                        hangwallListOut[geolineHgwallNE].Add(inOID);
                    }

                }
            }

            if (getBearing > 67.5 && getBearing <= 112.5)
            {
                if (currentHangwall != geolineHgwallE)
                {
                    //Parse line that only needs a flip
                    if (currentHangwall == geolineHgwallW)
                    {
                        hangwallListOut[geolineMoveFlip].Add(inOID);
                    }
                    else
                    {
                        hangwallListOut[geolineHgwallE].Add(inOID);
                    }
                }
            }

            if (getBearing > 112.5 && getBearing <= 157.5)
            {
                if (currentHangwall != geolineHgwallSE)
                {

                    //Parse line that only needs a flip
                    if (currentHangwall == geolineHgwallNW)
                    {
                        hangwallListOut[geolineMoveFlip].Add(inOID);
                    }
                    else
                    {
                        hangwallListOut[geolineHgwallSE].Add(inOID);
                    }
                }
            }

            if (getBearing > 157.5 && getBearing <= 202.5)
            {
                if (currentHangwall != geolineHgwallS)
                {

                    //Parse line that only needs a flip
                    if (currentHangwall == geolineHgwallN)
                    {
                        hangwallListOut[geolineMoveFlip].Add(inOID);
                    }
                    else
                    {
                        hangwallListOut[geolineHgwallS].Add(inOID);
                    }
                }
            }

            if (getBearing > 202.5 && getBearing <= 247.5)
            {
                if (currentHangwall != geolineHgwallSW)
                {

                    //Parse line that only needs a flip
                    if (currentHangwall == geolineHgwallNE)
                    {
                        hangwallListOut[geolineMoveFlip].Add(inOID);
                    }
                    else
                    {
                        hangwallListOut[geolineHgwallSW].Add(inOID);
                    }
                }
            }

            if (getBearing > 247.5 && getBearing <= 295.5)
            {
                if (currentHangwall != geolineHgwallW)
                {

                    //Parse line that only needs a flip
                    if (currentHangwall == geolineHgwallE)
                    {
                        hangwallListOut[geolineMoveFlip].Add(inOID);
                    }
                    else
                    {
                        hangwallListOut[geolineHgwallW].Add(inOID);
                    }
                }
            }

            if (getBearing > 295.5 && getBearing <= 337.5)
            {
                if (currentHangwall != geolineHgwallNW)
                {
                    //Parse line that only needs a flip
                    if (currentHangwall == geolineHgwallSE)
                    {
                        hangwallListOut[geolineMoveFlip].Add(inOID);
                    }
                    else
                    {
                        hangwallListOut[geolineHgwallNW].Add(inOID);
                    }
                }
            }

        }

        /// <summary>
        /// Will return a validation layer name based on an input hangwall code
        /// </summary>
        /// <param name="inputCode"></param>
        /// <returns></returns>
        public string parseOutputLayerNames4Hangwalls(string inputCode, Dictionary<string, string> movementDomain)
        {
            //Variables
            string outputLayerName = "";

            //Main process
            //Parse result to return right code
            if (inputCode == geolineHgwallN)
            {
                outputLayerName = geolineMoveCode01;          
            }

            if (inputCode == geolineHgwallNE)
            {
                outputLayerName = geolineMoveCode02;              
            }
            if (inputCode == geolineHgwallE)
            {
                outputLayerName = geolineMoveCode03;              
            }
            if (inputCode == geolineHgwallSE)
            {
                outputLayerName = geolineMoveCode04;              
            }
            if (inputCode == geolineHgwallS)
            {
                outputLayerName = geolineMoveCode05;              
            }
            if (inputCode == geolineHgwallSW)
            {
                outputLayerName = geolineMoveCode06;              
            }
            if (inputCode == geolineHgwallW)
            {
                outputLayerName = geolineMoveCode07;              
            }
            if (inputCode == geolineHgwallNW)
            {
                outputLayerName = geolineMoveCode08;              
            }
            if (inputCode == geolineMoveFlip)
            {
                outputLayerName = geolineMoveFlip;
            }

            if (movementDomain.ContainsKey(inputCode))
            {
                outputLayerName = outputLayerName + movementDomain[inputCode] + " (" + inputCode + ")";
            }

            return outputLayerName;
        }
        #endregion

    }
}
