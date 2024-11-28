using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;

namespace GSC_ProjectEditor
{
    public partial class Form_Load_TranslateGanfeldPointStructure : Form
    {
        #region MAIN VARIABLE
        public List<string> extensionFilter = new List<string>() { ".shp", ".gdb", ".mdb", ".sqlite" };

        //Ganfeld structures
        private const string gFeatureDataSet = GSC_ProjectEditor.Constants.DatabaseGanfeld.fd;
        private const string gFCLinework = GSC_ProjectEditor.Constants.DatabaseGanfeld.fcLinework;
        private string gFCStation = GSC_ProjectEditor.Constants.DatabaseGanfeld.fcStation;
        private const string gFCTraverses = GSC_ProjectEditor.Constants.DatabaseGanfeld.fcTraverses;
        private string gTEarthmat = GSC_ProjectEditor.Constants.DatabaseGanfeld.gEarthMath;
        private const string gTMA = GSC_ProjectEditor.Constants.DatabaseGanfeld.gMA;
        private const string gTMineral = GSC_ProjectEditor.Constants.DatabaseGanfeld.gMineral;
        private const string gTMetadata = GSC_ProjectEditor.Constants.DatabaseGanfeld.gMetadata;
        private const string gTPhoto = GSC_ProjectEditor.Constants.DatabaseGanfeld.gPhoto;
        private const string gTSample = GSC_ProjectEditor.Constants.DatabaseGanfeld.gSample;
        private string gTStruc = GSC_ProjectEditor.Constants.DatabaseGanfeld.gStruc;
        private const string gTPFlow = GSC_ProjectEditor.Constants.DatabaseGanfeld.gPflow;
        private const string gTEnviron = GSC_ProjectEditor.Constants.DatabaseGanfeld.gEnviron;
        private const string gTSoilPro = GSC_ProjectEditor.Constants.DatabaseGanfeld.gSoilPro;

        //Ganfeld shapefile structures
        private const string shpLinework = GSC_ProjectEditor.Constants.GanfeldShapefiles.shpLinework;
        private const string shpStation = GSC_ProjectEditor.Constants.GanfeldShapefiles.shpStation;
        private const string shpTraverse = GSC_ProjectEditor.Constants.GanfeldShapefiles.shpTraverses;
        private const string shpEarthMat = GSC_ProjectEditor.Constants.GanfeldShapefiles.shpEarthMath;
        private const string shpMA = GSC_ProjectEditor.Constants.GanfeldShapefiles.shpMA;
        private const string dbfMetadata = GSC_ProjectEditor.Constants.GanfeldShapefiles.dbfMetadata;
        private const string shpMineral = GSC_ProjectEditor.Constants.GanfeldShapefiles.shpMineral;
        private const string shpPhoto = GSC_ProjectEditor.Constants.GanfeldShapefiles.shpPhoto;
        private const string shpSample = GSC_ProjectEditor.Constants.GanfeldShapefiles.shpSample;
        private const string shpStruc = GSC_ProjectEditor.Constants.GanfeldShapefiles.shpStruc;
        private const string shpEnviron = GSC_ProjectEditor.Constants.GanfeldShapefiles.shpEnviron;
        private const string shpPflow = GSC_ProjectEditor.Constants.GanfeldShapefiles.shpPFlow;
        private const string shpSoilPro = GSC_ProjectEditor.Constants.GanfeldShapefiles.shpSoilPro;

        //Database project structure
        private const string featureDataset = GSC_ProjectEditor.Constants.Database.FDGeo;
        private const string fcLinework = GSC_ProjectEditor.Constants.Database.gFCLinework;
        private const string fcStation = GSC_ProjectEditor.Constants.Database.gFCStation;
        private const string fcTraverse = GSC_ProjectEditor.Constants.Database.gFCTraverses;
        private const string tEarthMat = GSC_ProjectEditor.Constants.Database.gEarthMath;
        private const string tMA = GSC_ProjectEditor.Constants.Database.gMA;
        private const string tMetadata = GSC_ProjectEditor.Constants.Database.gMetadata;
        private const string tMineral = GSC_ProjectEditor.Constants.Database.gMineral;
        private const string tPhoto = GSC_ProjectEditor.Constants.Database.gPhoto;
        private const string tSample = GSC_ProjectEditor.Constants.Database.gSample;
        private const string tStruc = GSC_ProjectEditor.Constants.Database.gStruc;
        private const string tEnviron = GSC_ProjectEditor.Constants.Database.gEnviron;
        private const string tSoilPro = GSC_ProjectEditor.Constants.Database.gSoil;
        private const string tPflow = GSC_ProjectEditor.Constants.Database.gPFlow;

        //GANFELD F_STRUC table
        private string fStruc = GSC_ProjectEditor.Constants.DatabaseGanfeld.gStruc;
        private const string fStrucStatID = GSC_ProjectEditor.Constants.DatabaseGanfeldFields.stationID;
        private string fStrucEarthID = GSC_ProjectEditor.Constants.DatabaseGanfeldFields.earthmatID;
        private string fStrucID = GSC_ProjectEditor.Constants.DatabaseGanfeldFields.strucID;
        private string fStrucGeneration = GSC_ProjectEditor.Constants.DatabaseGanfeldFields.strucGeneration;
        private string fStrucMethod = GSC_ProjectEditor.Constants.DatabaseGanfeldFields.strucMethod;
        private string fStrucClass = GSC_ProjectEditor.Constants.DatabaseGanfeldFields.strucClass;
        private string fStrucYoung = GSC_ProjectEditor.Constants.DatabaseGanfeldFields.strucYounging;
        private string fStrucSubset = GSC_ProjectEditor.Constants.DatabaseGanfeldFields.strucDetail;
        private string fStrucFlat = GSC_ProjectEditor.Constants.DatabaseGanfeldFields.strucFlattening;
        private string fStrucStrain = GSC_ProjectEditor.Constants.DatabaseGanfeldFields.strucStrain;
        private string fStrucAtt = GSC_ProjectEditor.Constants.DatabaseGanfeldFields.strucAttitude;
        private string fStrucDip = GSC_ProjectEditor.Constants.DatabaseGanfeldFields.strucDip;
        private string fStrucRelated = GSC_ProjectEditor.Constants.DatabaseGanfeldFields.strucRelated;
        private string fStrucAzim = GSC_ProjectEditor.Constants.DatabaseGanfeldFields.strucAzim;
        private string fStrucNotes = GSC_ProjectEditor.Constants.DatabaseGanfeldFields.strucNotes;
        private string fStrucSense = GSC_ProjectEditor.Constants.DatabaseGanfeldFields.strucSense;
        private string fStrucType = GSC_ProjectEditor.Constants.DatabaseGanfeldFields.strucType;
        private string fStrucSymang = GSC_ProjectEditor.Constants.DatabaseGanfeldFields.strucSymAng;

        //GSC Field App F_STRUCTURE table
        public const string fieldAppLocatin = GSC_ProjectEditor.Constants.GSCFieldAPP.tLocation;
        public const string fieldAppStation = GSC_ProjectEditor.Constants.GSCFieldAPP.tStation;
        public const string fieldAppEarthMath = GSC_ProjectEditor.Constants.GSCFieldAPP.tEarthMath;
        public const string fieldAppStruc = GSC_ProjectEditor.Constants.GSCFieldAPP.tStruc;
        public const string fieldAppMetadata = GSC_ProjectEditor.Constants.GSCFieldAPP.tMetadata;

        private const string faStrucStatID = GSC_ProjectEditor.Constants.GSCFieldAPP.FieldStationID;
        private const string faStrucEarthID = GSC_ProjectEditor.Constants.GSCFieldAPP.FieldEarthMatID;
        private const string faStrucID = GSC_ProjectEditor.Constants.GSCFieldAPP.FieldStructureID;
        private const string faStrucGeneration = GSC_ProjectEditor.Constants.GSCFieldAPP.FieldStructureGeneration;
        private const string faStrucMethod = GSC_ProjectEditor.Constants.GSCFieldAPP.FieldStructureMethod;
        private const string faStrucClass = GSC_ProjectEditor.Constants.GSCFieldAPP.FieldStructureClass;
        private const string faStrucYoung = GSC_ProjectEditor.Constants.GSCFieldAPP.FieldStructureYoung;
        private const string faStrucSubset = GSC_ProjectEditor.Constants.GSCFieldAPP.FieldStructureDetail;
        private const string faStrucFlat = GSC_ProjectEditor.Constants.GSCFieldAPP.FieldStructureFlattening;
        private const string faStrucStrain = GSC_ProjectEditor.Constants.GSCFieldAPP.FieldStructureStrain;
        private const string faStrucAtt = GSC_ProjectEditor.Constants.GSCFieldAPP.FieldStructureAttitude;
        private const string faStrucDip = GSC_ProjectEditor.Constants.GSCFieldAPP.FieldStructureDip;
        private const string faStrucRelated = GSC_ProjectEditor.Constants.GSCFieldAPP.FieldStructureRelated;
        private const string faStrucAzim = GSC_ProjectEditor.Constants.GSCFieldAPP.FieldStructureAzimuth;
        private const string faStrucNotes = GSC_ProjectEditor.Constants.GSCFieldAPP.FieldStructureNotes;
        private const string faStrucSense = GSC_ProjectEditor.Constants.GSCFieldAPP.FieldStructureSense;
        private const string faStrucType = GSC_ProjectEditor.Constants.GSCFieldAPP.FieldStructureType;
        private const string faStrucSymang = GSC_ProjectEditor.Constants.GSCFieldAPP.FieldStrucSymAng;

        //GSC Field app other tables fields
        public const string faLocLongitude = GSC_ProjectEditor.Constants.GSCFieldAPP.FieldLocationLongitude;
        public const string faLocLatitude = GSC_ProjectEditor.Constants.GSCFieldAPP.FieldLocationlatitude;
        public const string faLocElevation = GSC_ProjectEditor.Constants.GSCFieldAPP.FieldLocationElevation;
        public const string faStationID = GSC_ProjectEditor.Constants.GSCFieldAPP.FieldStationID;
        public const string faLocationID = GSC_ProjectEditor.Constants.GSCFieldAPP.FieldLocationID;

        //Project F_STRUC Table
        private const string pStruc = GSC_ProjectEditor.Constants.Database.gStruc;

        //GANFELD STRUC shapefile
        private const string gStruc = GSC_ProjectEditor.Constants.GanfeldShapefiles.shpStruc;

        #region GEOPOINT
        //FEATURE GEO_POINT
        private const string geopoints = GSC_ProjectEditor.Constants.Database.FGeopoint;
        private const string geopointID = GSC_ProjectEditor.Constants.DatabaseFields.FGeopointID;
        private const string geopointType = GSC_ProjectEditor.Constants.DatabaseFields.FGeopointType;
        private const string geopointSubset = GSC_ProjectEditor.Constants.DatabaseFields.FGeopointSubset;
        private const string geopointStrucAtt = GSC_ProjectEditor.Constants.DatabaseFields.FGeopointStrucAtt;
        private const string geopointStrucGene = GSC_ProjectEditor.Constants.DatabaseFields.FGeopointStrucGene;
        private const string geopointStrucYoung = GSC_ProjectEditor.Constants.DatabaseFields.FGeopointStrucYoung;
        private const string geopointStrucMethod = GSC_ProjectEditor.Constants.DatabaseFields.FGeopointStrucMethod;
        private const string geopointStrucFlat = GSC_ProjectEditor.Constants.DatabaseFields.FGeopointFlat;
        private const string geopointStrucStrain = GSC_ProjectEditor.Constants.DatabaseFields.FGeopointStrain;
        private const string geopointStrucDip = GSC_ProjectEditor.Constants.DatabaseFields.FGeopointDipPlunge;
        private const string geopointStrucRelated = GSC_ProjectEditor.Constants.DatabaseFields.FGeopointRelatedStruc;
        private const string geopointStrucStrucID = GSC_ProjectEditor.Constants.DatabaseFields.FGeopointStrucID;
        private const string geopointStrucAzim = GSC_ProjectEditor.Constants.DatabaseFields.FGeopointAzimuth;
        private const string geopointStrucSense = GSC_ProjectEditor.Constants.DatabaseFields.FGeopointSenseEvid;
        private const string geopointStrucRemark = GSC_ProjectEditor.Constants.DatabaseFields.FGeopointRemark;

        //Feature GEO_POINT ATTITUDE DOM VALUES
        private const string attHoriUp = GSC_ProjectEditor.Constants.DatabaseDomainsValues.geopointAttHoriUp;
        private const string attInclinedUp = GSC_ProjectEditor.Constants.DatabaseDomainsValues.geopointAttInclinedUp;
        private const string attHoriOver = GSC_ProjectEditor.Constants.DatabaseDomainsValues.geopointAttHoriOverturned;
        private const string attInclinedLess180 = GSC_ProjectEditor.Constants.DatabaseDomainsValues.geopointAttInclinedOverLess180;
        private const string attInclinedOver180 = GSC_ProjectEditor.Constants.DatabaseDomainsValues.geopointAttInclinedOverHigher180;
        private const string attUndef = GSC_ProjectEditor.Constants.DatabaseDomainsValues.geopointAttUndef;
        //private const string attLineHori = GSC_ProjectEditor.Constants.DatabaseDomainsValues.geopointAttLinearHori;
        //private const string attLinePlunging = GSC_ProjectEditor.Constants.DatabaseDomainsValues.geopointAttLinearPlunging;
        private const string attVertical = GSC_ProjectEditor.Constants.DatabaseDomainsValues.geopointAttVerti;

        //Feature GEO_POINT Not applicable dom value
        private const string NA = GSC_ProjectEditor.Constants.DatabaseDomainsValues.notAppicable;
        #endregion

        #region Symbol tables
        private const string geopointSymbol = GSC_ProjectEditor.Constants.Database.TGeopointSymbol;
        private const string geolineSymbol = GSC_ProjectEditor.Constants.Database.TGeolineSymbol;
        private const string geopointSymbolGEOID = GSC_ProjectEditor.Constants.DatabaseFields.TGeopointID;
        private const string geolineSymbolGEOID = GSC_ProjectEditor.Constants.DatabaseFields.MGeolineID;
        private const string selectcodeField = GSC_ProjectEditor.Constants.DatabaseFields.MGeolineSelectCode;
        private const string fgdcSymbol = GSC_ProjectEditor.Constants.DatabaseFields.MGeolineFGDC;
        private const string fgdcSymbolPoint = GSC_ProjectEditor.Constants.DatabaseFields.TGeopointFGDC;
        private const string geolineSymbolDesc = GSC_ProjectEditor.Constants.DatabaseFields.MGeolineLegendDescription;
        private const string geopointSymbolDesc = GSC_ProjectEditor.Constants.DatabaseFields.TGeopointLegendDesc;
        #endregion

        #region Legend generator table
        private const string legendTable = GSC_ProjectEditor.Constants.Database.TLegendGene;
        private const string legendTableSymbol = GSC_ProjectEditor.Constants.DatabaseFields.LegendSymbol;
        private const string legendTableID = GSC_ProjectEditor.Constants.DatabaseFields.LegendLabelID;
        private const string legendTableSymType = GSC_ProjectEditor.Constants.DatabaseFields.LegendSymType;
        private const string legendTableName = GSC_ProjectEditor.Constants.DatabaseFields.LegendGISDisplay;
        private const string legendTableSymTypeLine = GSC_ProjectEditor.Constants.DatabaseDomainsValues.SymTypeLine;
        private const string legendTableSymTypePoint = GSC_ProjectEditor.Constants.DatabaseDomainsValues.SymTypePoint;
        private const string legendTableSymTheme = GSC_ProjectEditor.Constants.DatabaseFields.LegendItemType;
        #endregion

        //Project station feature class
        private const string pStation = GSC_ProjectEditor.Constants.Database.gFCStation;
        private const string pStationID = GSC_ProjectEditor.Constants.DatabaseFields.FStationID;

        //Other variables
        string seperator = GSC_ProjectEditor.Constants.Seperator.textFileLineSep;
        string structureGenerationDefaultException = "10"; //Will be used to process default values for exception regarding some structure type only, superseeding Ganfeld.
        bool haveGeometry = false;
        IWorkspace inWorkspace;

        #endregion
        public Form_Load_TranslateGanfeldPointStructure()
        {
            //Init culture
            Utilities.Culture.SetCulture();

            InitializeComponent();

            this.Shown += new EventHandler(FormTranslateFStrucToGEOPOINT_Shown);
        }

        #region EVENTS
        void FormTranslateFStrucToGEOPOINT_Shown(object sender, EventArgs e)
        {
            if (!GSC_ProjectEditor.Properties.Settings.Default.dwEnabling)
            {
                this.Close();
            }
        }

        private void btn_BrowseData_Click(object sender, EventArgs e)
        {
            //Open dialog
            string dataPath = GSC_ProjectEditor.Dialog.GetDataPrompt(this.Handle.ToInt32(), Properties.Resources.Message_DataPromptTitle);
            this.txtbox_DataPath.Text = dataPath;

            //Validate input data type
            bool didValidate = false;
            
            foreach (string validExtensions in extensionFilter)
            {
                if (dataPath.Contains(validExtensions))
                {
                    //Detect type of data, with or without geometry

                    if (validExtensions == ".shp")
                    {
                        haveGeometry = true;
                    }
                    else if (validExtensions == ".gdb" || validExtensions == ".mdb")
                    {
                        //Detect if it's a table or a feature class
                        haveGeometry = GSC_ProjectEditor.Datasets.GDBDatasetHasGeometry(dataPath);

                        //If it does have geometry, check for point
                        if (haveGeometry)
                        {
                            IFeatureClass inFeatureClass = GSC_ProjectEditor.FeatureClass.OpenFeatureClassFromStringFaster(this.txtbox_DataPath.Text);
                            if (inFeatureClass.ShapeType != esriGeometryType.esriGeometryPoint)
                            {
                                GSC_ProjectEditor.Messages.ShowGenericErrorMessage(Properties.Resources.Error_WrongGeometry);
                                break;
                            }

                            GSC_ProjectEditor.ObjectManagement.ReleaseObject(inFeatureClass);

                        }
                    }

                    didValidate = true;
                }
            }

            //If the code gets here, tell user that something was wrong
            if (!didValidate && !haveGeometry)
            {
                MessageBox.Show(Properties.Resources.Error_WrongFileType, Properties.Resources.Error_GenericTitle, MessageBoxButtons.RetryCancel, MessageBoxIcon.Error);
                this.btn_ImportData.Enabled = false;
            }
            else
            {
                this.txtbox_DataPath.Text = dataPath;
                ITable inTable = GSC_ProjectEditor.Tables.OpenTableFromStringFaster(dataPath);
                List<string> inFields = GSC_ProjectEditor.Tables.GetFieldList(inTable, false);

                //Find if other tables are also in there
                IDataset inTableDataset = inTable as IDataset;
                inWorkspace = inTableDataset.Workspace;
                string workspacePath = inWorkspace.PathName;

                if (inFields.Contains(GSC_ProjectEditor.Constants.GSCFieldAPP.FieldStructureName))
                {
                    this.radioButton_GSCFieldApp.Checked = true;

                    RemapFieldNames();
                    RemapTableNames();
                }
                else
                {
                    this.radioButton_Ganfeld.Checked = true;
                }

                //Get if needed tables exists
                if (!dataPath.Contains(".shp"))
                {
                    string stationPath = System.IO.Path.Combine(workspacePath, gFCStation);
                    string earthmatPath = System.IO.Path.Combine(workspacePath, gTEarthmat);
                    string locationPath = System.IO.Path.Combine(workspacePath, fieldAppLocatin);
                    if (GSC_ProjectEditor.Tables.OpenTableFromStringFaster(stationPath) != null && GSC_ProjectEditor.Tables.OpenTableFromStringFaster(earthmatPath) != null)
                    {
                        if (this.radioButton_GSCFieldApp.Checked)
                        {
                            if (GSC_ProjectEditor.Tables.OpenTableFromStringFaster(locationPath) == null)
                            {
                                MessageBox.Show("Mising tables", Properties.Resources.Error_GenericTitle, MessageBoxButtons.RetryCancel, MessageBoxIcon.Error);
                                this.btn_ImportData.Enabled = false;
                            }
                            else
                            {
                                this.btn_ImportData.Enabled = true;
                            }
                        }
                        else
                        {
                            this.btn_ImportData.Enabled = true;
                        }
                    }
                    else
                    {
                        MessageBox.Show("Mising tables", Properties.Resources.Error_GenericTitle, MessageBoxButtons.RetryCancel, MessageBoxIcon.Error);
                        this.btn_ImportData.Enabled = false;
                    }
                }
                else
                {
                    this.btn_ImportData.Enabled = true;
                }

            }

        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btn_ImportData_Click(object sender, EventArgs e)
        {
            Translate();
        }
        #endregion

        #region METHODS

        /// <summary>
        /// Will reset field name variables with the ones coming from GSC Field App.
        /// </summary>
        private void RemapFieldNames()
        {
            fStrucEarthID = faStrucEarthID;
            fStrucID = faStrucID;
            fStrucGeneration = faStrucGeneration;
            fStrucMethod = faStrucMethod;
            fStrucClass = faStrucClass;
            fStrucYoung = faStrucYoung;
            fStrucSubset = faStrucSubset;
            fStrucFlat = faStrucFlat;
            fStrucStrain = faStrucStrain;
            fStrucAtt = faStrucAtt;
            fStrucDip = faStrucDip;
            fStrucRelated = faStrucRelated;
            fStrucAzim = faStrucAzim;
            fStrucNotes = faStrucNotes;
            fStrucSense = faStrucSense;
            fStrucType = faStrucType;
            fStrucSymang = faStrucSymang;
        }

        private void RemapTableNames()
        {
            gTStruc = fStruc = fieldAppStruc;
            gTEarthmat = fieldAppEarthMath;
            gFCStation = fieldAppStation;

        }

        /// <summary>
        /// Will translate field structure to project database model.
        /// </summary>
        private void Translate()
        {
            //Remove any old validation text file if any exists
            string outputTextFilePath = GSC_ProjectEditor.Properties.Settings.Default.PROJECT_WORKSPACE_PATH + "\\" + GSC_ProjectEditor.Properties.Resources.FileName_FStrucValidation + ".txt";
            System.IO.File.Delete(outputTextFilePath);

            //Get input and output features, and also geometry if needed
            ITable inTable = GSC_ProjectEditor.Tables.OpenTableFromStringFaster(this.txtbox_DataPath.Text);
            Dictionary<string, IGeometry> StrucGeom = new Dictionary<string, IGeometry>();
            if (this.radioButton_Ganfeld.Checked)
            {
                //Make like if it is always from an outside databas, even though it might be not. It simplifies everything.
                if (System.IO.File.Exists(System.IO.Path.Combine(this.txtbox_DataPath.Text, pStation + ".shp")))
                {
                    StrucGeom = GSC_ProjectEditor.FeatureClass.GetGeometryDicoFrom(System.IO.Path.Combine(this.txtbox_DataPath.Text, pStation), true, null, pStationID);
                }
                else
                {
                    try
                    {
                        //Maybe station shapefile doesn't exists so try like a Field GDB
                        string fieldObsStation = System.IO.Path.Combine(this.txtbox_DataPath.Text.Split('.')[0] + ".gdb", Constants.Database.FDField, Constants.Database.FStation);
                        StrucGeom = GSC_ProjectEditor.FeatureClass.GetGeometryDicoFrom(fieldObsStation, true, null, pStationID);
                    }
                    catch (Exception)
                    {
                        //Maybe stationID field doesn't exists..if so it'll pop-up later as an error.
                       
                    }
                }

                
            }
            IFeatureClass outFC = GSC_ProjectEditor.FeatureClass.OpenFeatureClass(geopoints);

            Dictionary<string, List<string>> StrucFieldAppGeom = new Dictionary<string,List<string>>();
            if (this.radioButton_GSCFieldApp.Checked)
            {
                ISqlWorkspace insqlWorkspace = inWorkspace as ISqlWorkspace;
                string xyzID_query_select = "SELECT st." + fStrucID + ", l." + faLocLongitude + ", l." + faLocLatitude + ", l." + faLocElevation;
                string xyzID_query_from_join1 = " FROM " + gTStruc + " as st JOIN " + gTEarthmat + " on st." + fStrucEarthID + " = " + gTEarthmat + "." + fStrucEarthID;
                string xyzID_query_from_join2 = " JOIN " + gFCStation + " on " + gFCStation + "." + faStationID + " = " + gTEarthmat + "." + faStationID;
                string xyzID_query_from_join3 = " JOIN " + fieldAppLocatin + " as l on l."  +  faLocationID + " = " + gFCStation + "." + faLocationID;
                ICursor xyzIDCursor = insqlWorkspace.OpenQueryCursor(xyzID_query_select + xyzID_query_from_join1 + xyzID_query_from_join2 + xyzID_query_from_join3);
                IRow xyzid = xyzIDCursor.NextRow();
                int strucid_index = xyzIDCursor.FindField(fStrucID);
                int x_index = xyzIDCursor.FindField(faLocLongitude);
                int y_index = xyzIDCursor.FindField(faLocLatitude);
                int z_index = xyzIDCursor.FindField(faLocElevation);

                while (xyzid != null)
                {
                    StrucFieldAppGeom[xyzid.get_Value(strucid_index).ToString()] = new List<string>() { xyzid.get_Value(x_index).ToString(), xyzid.get_Value(y_index).ToString(), xyzid.get_Value(z_index).ToString() };
                    xyzid = xyzIDCursor.NextRow();
                }
                GSC_ProjectEditor.ObjectManagement.ReleaseObject(xyzIDCursor);
            }


            //Error handling variables
            bool foundErrors = false;
            List<IFeatureBuffer> bufferList = new List<IFeatureBuffer>(); //Will be used to go inside a second loop and insert buffers only if there is no error found
            List<String> strucIDList = new List<string>(); //Will be used to detect already appended strucs and prevent duplicate 

            //Validate if entered data is a F_STRUC data
            List<string> strucFieldList = GSC_ProjectEditor.Tables.GetFieldList(inTable, false);
            if (strucFieldList.Contains(fStrucEarthID) && strucFieldList.Contains(fStrucID))
            {
                #region Build some conversion dictionaries from internal settings
                Dictionary<string, string> strucSubsetPlanar;
                Dictionary<string, Dictionary<string, string>> strucSubsetPlanarRelated;
                BuildConversionDico(GSC_ProjectEditor.Properties.Resources.StrucSubsetPlanarSID, out strucSubsetPlanar, out strucSubsetPlanarRelated);

                Dictionary<string, string> strucSubsetLinear;
                Dictionary<string, Dictionary<string, string>> strucSubsetLinearRelated;
                BuildConversionDico(GSC_ProjectEditor.Properties.Resources.StrucSubsetLinearSID, out strucSubsetLinear, out strucSubsetLinearRelated);

                Dictionary<string, string> strucAttitude;
                Dictionary<string, Dictionary<string, string>> strucAttitudeRelated;
                BuildConversionDico(GSC_ProjectEditor.Properties.Resources.StrucAttitudeSID, out strucAttitude, out strucAttitudeRelated);

                Dictionary<string, string> strucGeneration;
                Dictionary<string, Dictionary<string, string>> strucGenerationRelated;
                BuildConversionDico(GSC_ProjectEditor.Properties.Resources.StrucGenerationSID, out strucGeneration, out strucGenerationRelated);

                Dictionary<string, string> strucYounging;
                Dictionary<string, Dictionary<string, string>> strucYoungingRelated;
                BuildConversionDico(GSC_ProjectEditor.Properties.Resources.StrucPlanarYoungingSID, out strucYounging, out strucYoungingRelated);

                Dictionary<string, string> strucMethod;
                Dictionary<string, Dictionary<string, string>> strucMethodRelated;
                BuildConversionDico(GSC_ProjectEditor.Properties.Resources.StrucMethodSID, out strucMethod, out strucMethodRelated);

                Dictionary<string, string> strucFlattening;
                Dictionary<string, Dictionary<string, string>> strucFlatteningRelated;
                BuildConversionDico(GSC_ProjectEditor.Properties.Resources.StrucFlatteningSID, out strucFlattening, out strucFlatteningRelated);

                Dictionary<string, string> strucStrain;
                Dictionary<string, Dictionary<string, string>> strucStrainRelated;
                BuildConversionDico(GSC_ProjectEditor.Properties.Resources.StrucStrainSID, out strucStrain, out strucStrainRelated);
                #endregion

                //Get an insert cursor for output data
                IFeatureCursor outCursor = outFC.Insert(true);

                //Iterate through input data and calculate new values
                ICursor inCursor = inTable.Search(null, true);

                #region Get some fields from input data
                int inClassIndex = inTable.FindField(fStrucClass);
                int outClassIndex = outFC.FindField(geopointType);

                int inTypeIndex = inTable.FindField(fStrucType);

                int inSubsetIndex = inTable.FindField(fStrucSubset);
                int outSubsetIndex = outFC.FindField(geopointSubset);

                int inAttIndex = inTable.FindField(fStrucAtt);
                int outAttIndex = outFC.FindField(geopointStrucAtt);

                int inGenerationIndex = inTable.FindField(fStrucGeneration);
                int outGenerationIndex = outFC.FindField(geopointStrucGene);

                int inMethodIndex = inTable.FindField(fStrucMethod);
                int outMethodIndex = outFC.FindField(geopointStrucMethod);

                int inYoungingIndex = inTable.FindField(fStrucYoung);
                int outYoungingIndex = outFC.FindField(geopointStrucYoung);

                int inFlatIndex = inTable.FindField(fStrucFlat);
                int outFlatIndex = outFC.FindField(geopointStrucFlat);

                int inStrainIndex = inTable.FindField(fStrucStrain);
                int outStrainIndex = outFC.FindField(geopointStrucStrain);

                int inDipIndex = inTable.FindField(fStrucDip);
                int outDipIndex = outFC.FindField(geopointStrucDip);

                int inStrucID = inTable.FindField(fStrucID);
                int outStrucID = outFC.FindField(geopointStrucStrucID);

                int inRelatedIndex = inTable.FindField(fStrucRelated);
                int outRelatedIndex = outFC.FindField(geopointStrucRelated);

                int inSymangIndex = inTable.FindField(fStrucSymang);
                int outAzimIndex = outFC.FindField(geopointStrucAzim);

                int inSenseIndex = inTable.FindField(fStrucSense);
                int outSenseIndex = outFC.FindField(geopointStrucSense);

                int inRemarkIndex = inTable.FindField(fStrucNotes);
                int outRemarkIndex = outFC.FindField(geopointStrucRemark);

                int outGeopointIDIndex = outFC.FindField(geopointID);

                int inStationIndex = inTable.FindField(fStrucStatID);

                #endregion

                IRow currentFeat = inCursor.NextRow();
                while (currentFeat != null)
                {
                    bool foundMissingStrucID = false;

                    //For current row, create a new buffer in which the new information will be inserted
                    //Build feature buffer
                    IFeatureBuffer outFeatBuff = outFC.CreateFeatureBuffer();

                    #region Get information and start build geopoitnID
                    //Get current information
                    string currentClass = currentFeat.get_Value(inClassIndex).ToString();
                    string currentGeneration = currentFeat.get_Value(inGenerationIndex).ToString();
                    string currentMethod = currentFeat.get_Value(inMethodIndex).ToString();
                    string currentYounging = currentFeat.get_Value(inYoungingIndex).ToString();
                    string currentSubset = currentFeat.get_Value(inSubsetIndex).ToString();
                    string currentFlat = currentFeat.get_Value(inFlatIndex).ToString();
                    string currentStrain = currentFeat.get_Value(inStrainIndex).ToString();
                    string currentAttitude = currentFeat.get_Value(inAttIndex).ToString();
                    string currentStucID = currentFeat.get_Value(inStrucID).ToString();
                    string currentRelated = currentFeat.get_Value(inRelatedIndex).ToString();
                    string currentSense = currentFeat.get_Value(inSenseIndex).ToString();
                    string currentRemark = currentFeat.get_Value(inRemarkIndex).ToString();
                    //string currentStatID = currentFeat.get_Value(inStationIndex).ToString();
                    string currentType = currentFeat.get_Value(inTypeIndex).ToString();

                    //Only process with a proper struc id
                    if (currentStucID == string.Empty || currentStucID == "" || currentStucID == DBNull.Value.ToString())
                    {
                        //Write to textfile
                        GSC_ProjectEditor.FolderAndFiles.WriteToTextFile(outputTextFilePath, "ID: " + currentStucID + ", missing StrucID");
                        foundErrors = true;
                        foundMissingStrucID = true;

                    }
                    else
                    {
                        strucIDList.Add(currentStucID);
                    }

                    int currentDip = 0;
                    if (currentFeat.get_Value(inDipIndex) != DBNull.Value)
                    {
                        currentDip = Convert.ToInt16(currentFeat.get_Value(inDipIndex));
                    }
                    int currentAzim = 0;
                    if (currentFeat.get_Value(inSymangIndex) != DBNull.Value)
                    {
                        currentAzim = Convert.ToInt16(currentFeat.get_Value(inSymangIndex));
                    }

                    //Start building GEOPOINTID
                    string buildGEOPOINTID = "";
                    string buildGEOPOINTID_Type = "";
                    string buildGEOPOINTID_Subset = "";
                    string buildGEOPOINTID_Attitude = "";
                    string buildGEOPOINTID_Generation = "";
                    string buildGEOPOINTID_Younging = "";
                    string buildGEOPOINTID_Method = "";

                    #endregion

                    #region Parse ganfeld value to project values

                    #region TYPE
                    if (currentClass == GSC_ProjectEditor.Constants.DatabaseGanfeldFieldValues.strucClassPlanar)
                    {
                        //Insert new values
                        outFeatBuff.set_Value(outClassIndex, GSC_ProjectEditor.Constants.DatabaseDomainsValues.geopointPlanar);

                        //Manage geopointID
                        buildGEOPOINTID_Type = GSC_ProjectEditor.Constants.DatabaseDomainsValues.geopointPlanar.ToString();
                    }
                    else if (currentClass == GSC_ProjectEditor.Constants.DatabaseGanfeldFieldValues.strucClassLinear)
                    {
                        //Insert new values
                        outFeatBuff.set_Value(outClassIndex, GSC_ProjectEditor.Constants.DatabaseDomainsValues.geopointLinear);

                        //Manage geopointID
                        buildGEOPOINTID_Type = GSC_ProjectEditor.Constants.DatabaseDomainsValues.geopointLinear.ToString();
                    }
                    #endregion

                    #region SUBSET PLANAR

                    if (currentClass == GSC_ProjectEditor.Constants.DatabaseGanfeldFieldValues.strucClassPlanar)
                    {
                        //If field value as any parents
                        if (strucSubsetPlanarRelated.Count !=0)
                        {
                            if (strucSubsetPlanarRelated.ContainsKey(currentSubset) && !strucSubsetPlanarRelated[currentSubset].ContainsKey(string.Empty))
                            {
                                outFeatBuff.set_Value(outSubsetIndex, strucSubsetPlanarRelated[currentSubset][currentType]);

                                buildGEOPOINTID_Subset = strucSubsetPlanarRelated[currentSubset][currentType]; 
                            }
                            else if (strucSubsetPlanarRelated.ContainsKey(currentSubset))
                            {
                                outFeatBuff.set_Value(outSubsetIndex, strucSubsetPlanarRelated[currentSubset][string.Empty]);

                                buildGEOPOINTID_Subset = strucSubsetPlanarRelated[currentSubset][string.Empty]; 
                            }

                        }
                        else
                        {
                            //Insert new values
                            outFeatBuff.set_Value(outSubsetIndex, strucSubsetPlanar[currentSubset]);

                            buildGEOPOINTID_Subset = strucSubsetPlanar[currentSubset];
                        }


                    }

                    #endregion

                    #region SUBSET LINEAR
                    if (currentClass == GSC_ProjectEditor.Constants.DatabaseGanfeldFieldValues.strucClassLinear)
                    {
                        //If field value as any parents
                        if (strucSubsetLinearRelated.Count != 0)
                        {
                            if (strucSubsetLinearRelated.ContainsKey(currentSubset) && strucSubsetLinearRelated[currentSubset].ContainsKey(currentType))
                            {
                                outFeatBuff.set_Value(outSubsetIndex, strucSubsetLinearRelated[currentSubset][currentType]);

                                buildGEOPOINTID_Subset = strucSubsetLinearRelated[currentSubset][currentType];
                            }

                        }
                        else
                        {
                            //Insert new values
                            if (strucSubsetLinear.ContainsKey(currentSubset))
                            {
                                outFeatBuff.set_Value(outSubsetIndex, strucSubsetLinear[currentSubset]);

                                buildGEOPOINTID_Subset = strucSubsetLinear[currentSubset];
                            }

                        }

                    }
                    #endregion

                    #region ATTITUDE
                    if (currentClass == GSC_ProjectEditor.Constants.DatabaseGanfeldFieldValues.strucClassPlanar && strucAttitude.ContainsKey(currentAttitude))
                    {
                        #region DROPPED Filter upright data
                        //if (currentAttitude.ToLower().Contains("upright"))
                        //{
                        //    #region Upright value calculation
                        //    //Calculate value based on DIP value
                        //    if (currentDip == 0 || currentDip == 360)
                        //    {
                        //        outFeatBuff.set_Value(outAttIndex, attHoriOver);
                        //        buildGEOPOINTID_Attitude = attHoriOver;
                        //    }
                        //    else if (currentDip == 90 || currentDip == 270)
                        //    {
                        //        outFeatBuff.set_Value(outAttIndex, attVertical);
                        //        buildGEOPOINTID_Attitude = attVertical;
                        //    }
                        //    else if (currentDip == 180)
                        //    {
                        //        outFeatBuff.set_Value(outAttIndex, attHoriOver);
                        //        buildGEOPOINTID_Attitude = attHoriOver;
                        //    }
                        //    else if ((currentDip > 0 && currentDip < 90) || (currentDip > 90 && currentDip < 180))
                        //    {
                        //        outFeatBuff.set_Value(outAttIndex, attInclinedLess180);
                        //        buildGEOPOINTID_Attitude = attInclinedLess180;
                        //    }
                        //    else if ((currentDip > 180 && currentDip < 270) || (currentDip > 270 && currentDip < 360))
                        //    {
                        //        outFeatBuff.set_Value(outAttIndex, attInclinedOver180);
                        //        buildGEOPOINTID_Attitude = attInclinedOver180;
                        //    }
                        //    #endregion
                        //}
                        //else
                        //{
                        //    //Insert new values
                        //    outFeatBuff.set_Value(outAttIndex, strucAttitude[currentAttitude]);
                        //    buildGEOPOINTID_Attitude = strucAttitude[currentAttitude];
                        //}
                        #endregion

                        //Insert new values
                        outFeatBuff.set_Value(outAttIndex, strucAttitude[currentAttitude]);
                        buildGEOPOINTID_Attitude = strucAttitude[currentAttitude];

                    }
                    else if (currentClass == GSC_ProjectEditor.Constants.DatabaseGanfeldFieldValues.strucClassLinear)
                    {
                        outFeatBuff.set_Value(outAttIndex, NA);
                        buildGEOPOINTID_Attitude = NA;
                    }
                    #endregion

                    #region GENERATION

                    //Get structure type younging exception
                    List<string> strucGenException = StrucGenerationException();

                    //Overwrite previous calculated id if needed
                    if (strucGenException.Contains(buildGEOPOINTID_Subset))
                    {
                        //Insert new values
                        outFeatBuff.set_Value(outGenerationIndex, structureGenerationDefaultException);
                        buildGEOPOINTID_Generation = structureGenerationDefaultException;
                    }
                    else
                    {
                        if (strucGeneration.ContainsKey(currentGeneration))
                        {
                            //Insert new values
                            outFeatBuff.set_Value(outGenerationIndex, strucGeneration[currentGeneration]);
                            buildGEOPOINTID_Generation = strucGeneration[currentGeneration];
                        }
                        else
                        {
                            MessageBox.Show(currentGeneration);
                        }
                    }

                    #endregion

                    #region YOUNGING

                    //Get structure type younging exception
                    List<string> strucException = StrucYoungingException();



                    //Overwrite previous calculated id if needed
                    if ((currentClass == GSC_ProjectEditor.Constants.DatabaseGanfeldFieldValues.strucClassPlanar && strucException.Contains(buildGEOPOINTID_Subset)) || currentClass == GSC_ProjectEditor.Constants.DatabaseGanfeldFieldValues.strucClassLinear)
                    {
                        //Insert new values
                        outFeatBuff.set_Value(outYoungingIndex, strucYounging["not applicable"]);
                        buildGEOPOINTID_Younging = strucYounging["not applicable"];
                    }
                    else
                    {
                        if (currentClass == GSC_ProjectEditor.Constants.DatabaseGanfeldFieldValues.strucClassPlanar && strucYounging.ContainsKey(currentYounging))
                        {
                            //Insert new values
                            outFeatBuff.set_Value(outYoungingIndex, strucYounging[currentYounging]);
                            buildGEOPOINTID_Younging = strucYounging[currentYounging];
                        }
                        else if (currentClass == GSC_ProjectEditor.Constants.DatabaseGanfeldFieldValues.strucClassLinear)
                        {
                            outFeatBuff.set_Value(outYoungingIndex, strucYounging["not applicable"]);
                            buildGEOPOINTID_Younging = strucYounging["not applicable"];
                        }
                        else if (currentClass == GSC_ProjectEditor.Constants.DatabaseGanfeldFieldValues.strucClassPlanar && !strucYounging.ContainsKey(currentYounging))
                        {
                            //In case something else was written inside this field, convert to no evidence...
                            outFeatBuff.set_Value(outYoungingIndex, strucYounging[""]);
                            buildGEOPOINTID_Younging = strucYounging[""];
                        }
                    }

                    #endregion

                    #region METHOD
                    if (strucMethod.ContainsKey(currentMethod))
                    {
                        //Insert new values
                        outFeatBuff.set_Value(outMethodIndex, strucMethod[currentMethod]);
                        buildGEOPOINTID_Method = strucMethod[currentMethod];
                    }
                    #endregion

                    #region FLATTENING
                    if (strucFlattening.ContainsKey(currentFlat))
                    {
                        //Insert new values
                        outFeatBuff.set_Value(outFlatIndex, strucFlattening[currentFlat]);
                    }
                    #endregion

                    #region STRAIN
                    if (strucStrain.ContainsKey(currentStrain))
                    {
                        //Insert new values
                        outFeatBuff.set_Value(outStrainIndex, strucStrain[currentStrain]);
                    }
                    #endregion

                    #endregion

                    #region Parse ganfeld field to project field (no conversion necessary)

                    //STRUC ID
                    outFeatBuff.set_Value(outStrucID, currentStucID);

                    //RELATED STRUC
                    if (currentRelated.Count() > 15)
                    {
                        outFeatBuff.set_Value(outRelatedIndex, currentRelated.Substring(0, 14));
                    }
                    else
                    {
                        outFeatBuff.set_Value(outRelatedIndex, currentRelated);
                    }

                    //AZIMUTH
                    outFeatBuff.set_Value(outAzimIndex, currentAzim);

                    //DIPPLUNGE
                    outFeatBuff.set_Value(outDipIndex, currentDip);

                    //SENSE_EVID
                    if (currentSense.Count() > 50)
                    {
                        outFeatBuff.set_Value(outSenseIndex, currentSense.Substring(0, 49));
                    }
                    else
                    {
                        outFeatBuff.set_Value(outSenseIndex, currentSense);
                    }

                    //REMARKS
                    outFeatBuff.set_Value(outRemarkIndex, currentRemark);

                    #endregion

                    #region GEOPOINTID

                    buildGEOPOINTID = buildGEOPOINTID_Type + buildGEOPOINTID_Subset + buildGEOPOINTID_Attitude + buildGEOPOINTID_Generation + buildGEOPOINTID_Younging + buildGEOPOINTID_Method;

                    if (buildGEOPOINTID.Count() == 13)
                    {
                        outFeatBuff.set_Value(outGeopointIDIndex, buildGEOPOINTID);
                    }
                    else
                    {
                        //error line to write into textfile
                        string buildErrorLine = "Empty field values for structure '" + currentStucID + "' in those fields: [";

                        if (buildGEOPOINTID_Type.Count() == 0)
                        {
                            buildErrorLine = buildErrorLine + ", " + fStrucClass;
                        }
                        if (buildGEOPOINTID_Subset.Count() == 0)
                        {
                            buildErrorLine = buildErrorLine + ", " + fStrucSubset;
                        }
                        if (buildGEOPOINTID_Attitude.Count() == 0)
                        {
                            buildErrorLine = buildErrorLine + ", " + fStrucAtt;
                        }
                        if (buildGEOPOINTID_Generation.Count() == 0)
                        {
                            buildErrorLine = buildErrorLine + ", " + fStrucGeneration;
                        }
                        if (buildGEOPOINTID_Younging.Count() == 0)
                        {
                            buildErrorLine = buildErrorLine + ", " + fStrucYoung;
                        }
                        if (buildGEOPOINTID_Method.Count() == 0)
                        {
                            buildErrorLine = buildErrorLine + ", " + fStrucMethod;
                        }

                        //Finish line
                        buildErrorLine = buildErrorLine + "]";
                        foundErrors = true;

                        //Write to textfile
                        GSC_ProjectEditor.FolderAndFiles.WriteToTextFile(outputTextFilePath, buildErrorLine);


                    }

                    #endregion

                    #region Manage geometry

                    //Variable
                    bool foundGeom = true; //For table, if no related geometry is found inside station feature
                    IGeometry currentGeom;

                    if (this.radioButton_Ganfeld.Checked)
                    {

                        try
                        {
                            currentGeom = StrucGeom[currentFeat.get_Value(inStationIndex).ToString()];
                        }
                        catch (Exception)
                        {

                            foundGeom = false;
                            currentGeom = null;
                        }

                        //Make Z Aware, or else throws an error, because output feature is 3D
                        //Insert geometry only if something was found, related to current struc
                        if (foundGeom && !foundErrors && !foundMissingStrucID)
                        {
                            GSC_ProjectEditor.Geometry.ApplyConstantZ(currentGeom, 0.0);

                            //Insert shape copy
                            outFeatBuff.Shape = currentGeom;
                            bufferList.Add(outFeatBuff);

                        }
                        
                    }
                    else if (this.radioButton_GSCFieldApp.Checked)
                    {
                        //Create point
                        IPoint currentPoint = new PointClass();

                        //Set spatial reference, the app always uses WGS84
                        currentPoint.SpatialReference = GSC_ProjectEditor.SpatialReferences.CreateGeographicCoordinateSystem(esriSRGeoCSType.esriSRGeoCS_WGS1984) as ISpatialReference;
                        try
                        {
                            currentPoint.X = Convert.ToDouble(StrucFieldAppGeom[currentStucID][0]);
                            currentPoint.Y = Convert.ToDouble(StrucFieldAppGeom[currentStucID][1]);
                            currentPoint.Z = Convert.ToDouble(StrucFieldAppGeom[currentStucID][2]);
                        }
                        catch (Exception)
                        {
                            currentPoint.X = 0.0;
                            currentPoint.Y = 0.0;
                            currentPoint.Z = 0.0;
                        }
                        GSC_ProjectEditor.Geometry.MakeZAware(currentPoint as IGeometry);

                        //Reproject to user spatial ref
                        currentPoint.Project(GSC_ProjectEditor.SpatialReferences.GetSpatialRef(outFC));

                        //Insert shape copy
                        outFeatBuff.Shape = currentPoint as IGeometry;
                        bufferList.Add(outFeatBuff);
                    }


                    currentFeat = inCursor.NextRow();

                    #endregion
                }

                //If any validation errors were found
                if (foundErrors)
                {

                    GSC_ProjectEditor.Messages.ShowGenericErrorMessage(GSC_ProjectEditor.Properties.Resources.Warning_FStrucValidationErrorFound + " " + outputTextFilePath);

                    //Flush
                    outCursor.Flush();

                    //Release coms
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(outCursor);
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(inCursor);


                }
                else
                {
                    //Get a list of actual geopoint strucIDs
                    List<string> actualStrucIDs = GSC_ProjectEditor.Tables.GetFieldValues(geopoints, geopointStrucStrucID, null);

                    foreach (IFeatureBuffer bufs in bufferList)
                    {
                        //Don't append if already in geopoint
                        int actualStrucIDFieldIndex = bufs.Fields.FindField(geopointStrucStrucID);
                        string currentStrucID = bufs.get_Value(actualStrucIDFieldIndex).ToString();
                        if (!actualStrucIDs.Contains(currentStrucID))
                        {
                            //Insert new feature
                            outCursor.InsertFeature(bufs);
                        }

                    }

                    //Flush
                    outCursor.Flush();

                    //Release coms
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(outCursor);
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(inCursor);

                    //Set select code form
                    processSymbolAndLegendTables(outFC, geopointSymbol, geopointSymbolGEOID);
                }

                GSC_ProjectEditor.Messages.ShowEndOfProcess();
                
            }
            else
            {
                MessageBox.Show(Properties.Resources.Error_MissingStructureIDFields, Properties.Resources.Error_GenericTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            this.Close();
        }

        /// <summary>
        /// Will output a conversion dictionary. Keys will be ganfeld text value, Values will be project intented domain code
        /// </summary>
        /// <param name="mainResource">The string that contains translated terms</param>
        /// <param name="outputConversionDico">A string string dictionary that will hold field value as key and it's associated project value in dict. value</param>
        /// <param name="outputRelatedConversionDico">A string string dictionary that will hold field value as key and it's associated parent term if any exists and project value as dict. value</param>
        /// <returns></returns>
        private void BuildConversionDico(string mainResource, out Dictionary<string, string> outputConversionDico, out Dictionary<string, Dictionary<string, string>> outputRelatedConversionDico)
        {
            //Variables
            outputConversionDico = new Dictionary<string, string>();
            outputRelatedConversionDico = new Dictionary<string, Dictionary<string, string>>();

            //Get all lines from conversion textfile
            List<string> mainList = new List<string>(mainResource.Split(new[] { Environment.NewLine }, StringSplitOptions.None));

            //Iterate through lines and build the dictionary
            foreach (string line in mainList)
            {
                //Split line with separator
                string[] splitedLine = line.Split(seperator.ToArray());

                //Build dico, key= Ganfeld, value=dom code BedrockGDB
                if (splitedLine.Length == 5)
                {
                    if (!outputRelatedConversionDico.ContainsKey(splitedLine[3]))
                    {
                        outputRelatedConversionDico[splitedLine[3]] = new Dictionary<string, string>(); 
                    }

                    outputRelatedConversionDico[splitedLine[3]][splitedLine[4]] = splitedLine[1];
                    
                }
                else
                {
                    outputConversionDico[splitedLine[3]] = splitedLine[1];
                }

 
            }

            //return outputConversionDico;

        }

        /// <summary>
        /// Will update the symbol tables (geoline_Symbols or geopoint_symbols) based on new appended data. The SelectCodeField will be update with a yes value.
        /// </summary>
        /// <param name="inputFC"></param>
        /// <param name="outputFC"></param>
        /// <param name="outFieldName"></param>
        public void processSymbolAndLegendTables(IFeatureClass inputFC, string outputSymbolTable, string outFieldName)
        {
            try
            {
                //Utilities.EditSession.Start();

                //Get feature name
                IDataset inDS = inputFC as IDataset;
                IWorkspace inWorkspace = inDS.Workspace;
                string inName = inDS.BrowseName;
                string symType = "";

                //Get a list of new ids from input feature class
                Dictionary<string, List<string>> uniqueValues = GSC_ProjectEditor.Tables.GetUniqueFieldValues(inName, outFieldName, null, true, fgdcSymbol);
                List<string> symbolDico = GSC_ProjectEditor.Tables.GetFieldValues(legendTable, legendTableID, null);

                //Update database 
                foreach (string userNewData in uniqueValues["Main"])
                {
                    if (userNewData != "") //Some empty ids could be passed here
                    {
                        //Detect if symbol isn't already in the project
                        if (!symbolDico.Contains(userNewData))
                        {
                            //Get proper information from symbol table based on given id
                            string selectedQuery = outFieldName + " = '" + userNewData + "'";
                            Tuple<string, string, string> tupleFieldList;

                            if (outputSymbolTable == geolineSymbol)
                            {
                                tupleFieldList = new Tuple<string, string, string>(geolineSymbolGEOID, fgdcSymbol, geolineSymbolDesc);
                                symType = legendTableSymTypeLine;
                            }
                            else
                            {
                                tupleFieldList = new Tuple<string, string, string>(geopointSymbolGEOID, fgdcSymbolPoint, geopointSymbolDesc);
                                symType = legendTableSymTypePoint;
                            }

                            Dictionary<string, Tuple<string, string>> tripleResults = GSC_ProjectEditor.Tables.GetUniqueDicoTripleFieldValues(outputSymbolTable, tupleFieldList, selectedQuery);

                            if (userNewData != null)
                            {
                                //Add row inside legend table
                                Dictionary<string, object> newRowDico = new Dictionary<string, object>();

                                if (tripleResults.Count != 0)
                                {
                                    newRowDico[legendTableID] = userNewData;
                                    newRowDico[legendTableSymbol] = tripleResults[userNewData].Item1;
                                    newRowDico[legendTableName] = tripleResults[userNewData].Item2;
                                    newRowDico[legendTableSymType] = symType;
                                }
                                else
                                {
                                    newRowDico[legendTableID] = userNewData;
                                    newRowDico[legendTableSymbol] = DBNull.Value;
                                    newRowDico[legendTableName] = Properties.Resources.Error_NotInScientificLanguageDictionary;
                                    newRowDico[legendTableSymType] = symType;

                                }
                                newRowDico[legendTableSymTheme] = GSC_ProjectEditor.Constants.DatabaseDomainsValues.legendItemGeopoint;
                                GSC_ProjectEditor.Tables.AddRowWithValues(legendTable, newRowDico);

                            }



                        }


                    }

                }

                //Utilities.EditSession.Stop();

                //Release coms
                System.Runtime.InteropServices.Marshal.ReleaseComObject(inWorkspace);
            }
            catch (Exception processSymbolAndLegendTablesError)
            {
                MessageBox.Show("processSymbolAndLegendTablesError: " + processSymbolAndLegendTablesError.StackTrace);
            }

        }

        /// <summary>
        /// Will return a list of structure domain code that needs to have younging field set to code "99 - Younging evidence not applicable"
        /// </summary>
        public List<string> StrucYoungingException()
        {
            //Variable
            List<string> strucYoungingException = new List<string>();

            strucYoungingException.Add("1013");
            strucYoungingException.Add("1015");
            strucYoungingException.Add("1024");
            strucYoungingException.Add("1025");
            strucYoungingException.Add("1027");
            strucYoungingException.Add("1030");
            strucYoungingException.Add("1033");
            strucYoungingException.Add("1036");
            strucYoungingException.Add("1039");
            strucYoungingException.Add("1045");
            strucYoungingException.Add("1048");
            strucYoungingException.Add("1049");
            strucYoungingException.Add("1050");
            strucYoungingException.Add("1051");
            strucYoungingException.Add("1052");
            strucYoungingException.Add("1053");

            return strucYoungingException;

        }

        /// <summary>
        /// Will return a list of structure domain code that needs to have generation field set to code "10 - Primary"
        /// </summary>
        public List<string> StrucGenerationException()
        {
            //Variable
            List<string> strucGenException = new List<string>();

            strucGenException.Add("1001");
            strucGenException.Add("1003");
            strucGenException.Add("1054");
            strucGenException.Add("2012");

            return strucGenException;

        }
        #endregion
    }
}
