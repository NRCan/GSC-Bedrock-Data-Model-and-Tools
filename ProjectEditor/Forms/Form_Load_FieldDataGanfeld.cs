using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geoprocessing;
using ESRI.ArcGIS.Geometry;

namespace GSC_ProjectEditor
{
    public partial class Form_Load_FieldDataGanfeld : Form
    {
        #region Main Variables

        //Ganfeld structures
        private const string gFeatureDataSet = GSC_ProjectEditor.Constants.DatabaseGanfeld.fd;
        private const string gFCLinework = GSC_ProjectEditor.Constants.DatabaseGanfeld.fcLinework;
        private const string gFCStation = GSC_ProjectEditor.Constants.DatabaseGanfeld.fcStation;
        private const string gFCTraverses = GSC_ProjectEditor.Constants.DatabaseGanfeld.fcTraverses;
        private const string gTEarthmat = GSC_ProjectEditor.Constants.DatabaseGanfeld.gEarthMath;
        private const string gTMA = GSC_ProjectEditor.Constants.DatabaseGanfeld.gMA;
        private const string gTMineral = GSC_ProjectEditor.Constants.DatabaseGanfeld.gMineral;
        private const string gTMetadata = GSC_ProjectEditor.Constants.DatabaseGanfeld.gMetadata;
        private const string gTPhoto = GSC_ProjectEditor.Constants.DatabaseGanfeld.gPhoto;
        private const string gTSample = GSC_ProjectEditor.Constants.DatabaseGanfeld.gSample;
        private const string gTStruc = GSC_ProjectEditor.Constants.DatabaseGanfeld.gStruc;
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
        private const string fStruc = GSC_ProjectEditor.Constants.DatabaseGanfeld.gStruc;
        private const string fStrucStatID = GSC_ProjectEditor.Constants.DatabaseGanfeldFields.stationID;
        private const string fStrucEarthID = GSC_ProjectEditor.Constants.DatabaseGanfeldFields.earthmatID;
        private const string fStrucID = GSC_ProjectEditor.Constants.DatabaseGanfeldFields.strucID;
        private const string fStrucGeneration = GSC_ProjectEditor.Constants.DatabaseGanfeldFields.strucGeneration;
        private const string fStrucMethod = GSC_ProjectEditor.Constants.DatabaseGanfeldFields.strucMethod;
        private const string fStrucClass = GSC_ProjectEditor.Constants.DatabaseGanfeldFields.strucClass;
        private const string fStrucYoung = GSC_ProjectEditor.Constants.DatabaseGanfeldFields.strucYounging;
        private const string fStrucSubset = GSC_ProjectEditor.Constants.DatabaseGanfeldFields.strucDetail;
        private const string fStrucFlat = GSC_ProjectEditor.Constants.DatabaseGanfeldFields.strucFlattening;
        private const string fStrucStrain = GSC_ProjectEditor.Constants.DatabaseGanfeldFields.strucStrain;
        private const string fStrucAtt = GSC_ProjectEditor.Constants.DatabaseGanfeldFields.strucAttitude;
        private const string fStrucDip = GSC_ProjectEditor.Constants.DatabaseGanfeldFields.strucDip;
        private const string fStrucRelated = GSC_ProjectEditor.Constants.DatabaseGanfeldFields.strucRelated;
        private const string fStrucAzim = GSC_ProjectEditor.Constants.DatabaseGanfeldFields.strucAzim;
        private const string fStrucNotes = GSC_ProjectEditor.Constants.DatabaseGanfeldFields.strucNotes;
        private const string fStrucSense = GSC_ProjectEditor.Constants.DatabaseGanfeldFields.strucSense;

        //Project F_STRUC Table
        private const string pStruc = GSC_ProjectEditor.Constants.Database.gStruc;

        //GANFELD STRUC shapefile
        private const string gStruc = GSC_ProjectEditor.Constants.GanfeldShapefiles.shpStruc;

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
        private const string attLineHori = GSC_ProjectEditor.Constants.DatabaseDomainsValues.geopointAttLinearHori;
        private const string attLinePlunging = GSC_ProjectEditor.Constants.DatabaseDomainsValues.geopointAttLinearPlunging;
        private const string attVertical = GSC_ProjectEditor.Constants.DatabaseDomainsValues.geopointAttVerti;

        //Feature GEO_POINT Not applicable dom value
        private const string NA = GSC_ProjectEditor.Constants.DatabaseDomainsValues.notAppicable;

        //Symbol tables
        private const string geopointSymbol = GSC_ProjectEditor.Constants.Database.TGeopointSymbol;
        private const string geolineSymbol = GSC_ProjectEditor.Constants.Database.TGeolineSymbol;
        private const string geopointSymbolGEOID = GSC_ProjectEditor.Constants.DatabaseFields.TGeopointID;
        private const string geolineSymbolGEOID = GSC_ProjectEditor.Constants.DatabaseFields.MGeolineID;
        private const string selectcodeField = GSC_ProjectEditor.Constants.DatabaseFields.MGeolineSelectCode;
        private const string fgdcSymbol = GSC_ProjectEditor.Constants.DatabaseFields.MGeolineFGDC;
        private const string fgdcSymbolPoint = GSC_ProjectEditor.Constants.DatabaseFields.TGeopointFGDC;
        private const string geolineSymbolDesc = GSC_ProjectEditor.Constants.DatabaseFields.MGeolineLegendDescription;
        private const string geopointSymbolDesc = GSC_ProjectEditor.Constants.DatabaseFields.TGeopointLegendDesc;

        //Legend generator table
        private const string legendTable = GSC_ProjectEditor.Constants.Database.TLegendGene;
        private const string legendTableSymbol = GSC_ProjectEditor.Constants.DatabaseFields.LegendSymbol;
        private const string legendTableID = GSC_ProjectEditor.Constants.DatabaseFields.LegendLabelID;
        private const string legendTableSymType = GSC_ProjectEditor.Constants.DatabaseFields.LegendSymType;
        private const string legendTableName = GSC_ProjectEditor.Constants.DatabaseFields.LegendGISDisplay;
        private const string legendTableSymTypeLine = GSC_ProjectEditor.Constants.DatabaseDomainsValues.SymTypeLine;
        private const string legendTableSymTypePoint = GSC_ProjectEditor.Constants.DatabaseDomainsValues.SymTypePoint;
        private const string legendTableSymTheme = GSC_ProjectEditor.Constants.DatabaseFields.LegendItemType;

        //Project station feature class
        private const string pStation = GSC_ProjectEditor.Constants.Database.gFCStation;
        private const string pStationID = GSC_ProjectEditor.Constants.DatabaseFields.FStationID;

        //Other variables
        string seperator = GSC_ProjectEditor.Constants.Seperator.textFileLineSep;
        private const string shapeExtension = "*.shp";
        private const string dbfExntension = "dbf";
        string structureGenerationDefaultException = "10"; //Will be used to process default values for exception regarding some structure type only, superseeding Ganfeld.

        #endregion

        public Form_Load_FieldDataGanfeld()
        {
            //Set culture
            Utilities.Culture.SetCulture();

            InitializeComponent();

            //Initiate some custom controls.
            this.Shown += new EventHandler(FormAddGanfeldData_Shown);
        }

        void FormAddGanfeldData_Shown(object sender, EventArgs e)
        {
            if (!GSC_ProjectEditor.Properties.Settings.Default.dwEnabling)
            {
                this.Close();
            }

        }

        /// <summary>
        /// Will prompt a folder dialog for user to select a folder containing ganfeld shapefiles
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_SelectGanShapeFolder_Click(object sender, EventArgs e)
        {
            //Prompt a folder dialog
            string shapeFolderPath = GSC_ProjectEditor.Dialog.GetESRIFolderPrompt(this.Handle.ToInt32(), Properties.Resources.Dialog_SelectGanfeldShapefileFolder);

            //Add path to textbox
            this.txtbox_GanShapeFolder.Text = shapeFolderPath;
        }

        /// <summary>
        /// Will prompt a workspace dialog to user to select a database containg ganfeld data.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_SelectGanDB_Click(object sender, EventArgs e)
        {
            //Prompt a database dialog
            string DBPath = GSC_ProjectEditor.Dialog.GetWorkspacePrompt(this.Handle.ToInt32(), Properties.Resources.Dialog_DescriptionGanfeldDB);

            //Add patht to textbox
            this.txtbox_GanDB.Text = DBPath;
        }

        /// <summary>
        /// Close this form.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_CancelAddGanfeldData_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// This button will add ganfeld data into the project database
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btn_ImportGanfeld_Click(object sender, System.EventArgs e)
        {
            //Variables
            bool isTable = false;
            string structureShapefilePath = string.Empty;

            //Get interfaces info
            string gdbPath = this.txtbox_GanDB.Text;
            string shapefilePath = this.txtbox_GanShapeFolder.Text;

            //Other variables
            string currentDBPath = GSC_ProjectEditor.Properties.Settings.Default.PROJECT_DATABASE_PATH;
            IWorkspace projectWorkspace = GSC_ProjectEditor.Workspace.AccessWorkspace(currentDBPath);

            //Trigger and breakers
            bool alreadyAppended = false; //Will be used to know if user already has appended data from a file geodatabase

            #region If user has entered a Field GDB
            if (gdbPath != "" && System.IO.Directory.Exists(gdbPath))
            {
                isTable = true;

                //Get the workspace for the input and output database 
                //TODO PARSE PERSONAL AND FILE DATABASE ACCESS HERE...
                IWorkspace inWork = GSC_ProjectEditor.Workspace.AccessWorkspace(gdbPath);

                if (inWork != null)
                {
                    //Get a dictionary of input and output, for feature classes and tables
                    Dictionary<string, string> featureClassDico = get_FeatureClass_GDB_DicoInOut();
                    Dictionary<string, string> tableDico = get_Table_GDB_DicoInOut();

                    //Get a list of current tables and features from in database
                    List<string> dataNames = GSC_ProjectEditor.Tables.GetTableNameListFromWorkspace(inWork);
                    List<string> dataPrjNames = GSC_ProjectEditor.Tables.GetTableNameListFromWorkspace(projectWorkspace);

                    //Iterate through fc dico
                    foreach (KeyValuePair<string, string> InOuts in featureClassDico)
                    {
                        //Cast in feature classes
                        IFeatureClass inFC = GSC_ProjectEditor.FeatureClass.OpenFeatureClassFromWorkspace(inWork, InOuts.Key);

                        //Cast out feature class
                        IFeatureClass outFC = GSC_ProjectEditor.FeatureClass.OpenFeatureClassFromWorkspace(projectWorkspace, InOuts.Value);

                        //Append
                        GSC_ProjectEditor.GeoProcessing.AppendData(inFC, outFC);
                    }

                    //Iterate through table dico
                    foreach (KeyValuePair<string, string> InOutsTables in tableDico)
                    {
                        //If table exists inside In database
                        if (dataNames.Contains(InOutsTables.Key) && dataPrjNames.Contains(InOutsTables.Value))
                        {

                            try
                            {
                                //Cast the input as table
                                ITable inTable = GSC_ProjectEditor.Tables.OpenTableFromWorkspace(inWork, InOutsTables.Key);

                                //Cast the output as a table
                                ITable outTable = GSC_ProjectEditor.Tables.OpenTableFromWorkspace(projectWorkspace, InOutsTables.Value);

                                //Append
                                GSC_ProjectEditor.GeoProcessing.AppendData(inTable, outTable);
                            }
                            catch (Exception missingTable)
                            {

                                MessageBox.Show(Properties.Resources.Error_MissingTable, Properties.Resources.Error_GenericTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }

                        }

                    }
                }

                alreadyAppended = true;

            }
            #endregion

            #region If user has entered a folder containing shapefiles
            if (shapefilePath != "" && alreadyAppended == false)
            {
                //Get a list of shapefiles from within given folder
                List<string> shapeList = GSC_ProjectEditor.FolderAndFiles.GetListOfFilesFromFolder(shapefilePath, shapeExtension);

                //Get metadata path from within given folder
                string metadataTable = dbfMetadata + "." + dbfExntension;
                List<string> dbfList = GSC_ProjectEditor.FolderAndFiles.GetListOfFilesFromFolder(shapefilePath, metadataTable);

                //Add dbf to shapelist
                shapeList.AddRange(dbfList);

                //Get a matching list of shapefile that will be transformed into tables
                Dictionary<string, string> tableShp_Dico = get_Table_GDB_SHP_InOut(shapeList);
                Dictionary<string, string> featureClassShp_Dico = get_FeatureClass_GDB_SHP_InOut(shapeList);

                //Get a list of current tables and features from in database
                List<string> dataPrjNames = GSC_ProjectEditor.Tables.GetTableNameListFromWorkspace(projectWorkspace);

                //Append features
                //Iterate through fc dico
                foreach (KeyValuePair<string, string> InOuts in featureClassShp_Dico)
                {

                    //Cast in feature classes
                    IFeatureClass inFC = GSC_ProjectEditor.FeatureClass.OpenFeatureClassFromString(InOuts.Value);

                    //Cast out feature class
                    IFeatureClass outFC = GSC_ProjectEditor.FeatureClass.OpenFeatureClassFromWorkspace(projectWorkspace, InOuts.Key);

                    //Append
                    GSC_ProjectEditor.GeoProcessing.AppendData(inFC, outFC);
                }

                //Iterate through table dico
                foreach (KeyValuePair<string, string> InOutsTables in tableShp_Dico)
                {
                    if (dataPrjNames.Contains(InOutsTables.Key))
                    {
                        //Cast the input as table
                        ITable inTable = GSC_ProjectEditor.Tables.OpenTableFromString(InOutsTables.Value);

                        //Cast the output as a table
                        ITable outTable = GSC_ProjectEditor.Tables.OpenTableFromWorkspace(projectWorkspace, InOutsTables.Key);

                        //Append
                        GSC_ProjectEditor.GeoProcessing.AppendData(inTable, outTable);

                        //Keep path to struc shapefile
                        if (InOutsTables.Value.Contains(shpStruc))
                        {
                            structureShapefilePath = InOutsTables.Value;
                        }

                    }

                }

            }

            #endregion

            #region Update Legend Generator Table
            string fieldTheme = GSC_ProjectEditor.Constants.DatabaseDomainsValues.legendItemField;
            string fieldThemeQuery = legendTableSymTheme + " ='" + fieldTheme + "'";

            List<string> legendFieldThemes = GSC_ProjectEditor.Tables.GetFieldValues(legendTable, legendTableSymTheme, fieldThemeQuery);

            if (legendFieldThemes.Count == 0)
            {
                Dictionary<string, string> themeDico = GSC_ProjectEditor.Domains.GetDomDico(GSC_ProjectEditor.Constants.DatabaseDomains.legendSymbolTheme, "Code");
                string themeDescription = themeDico[fieldTheme];
                AddToLegendTable(themeDescription, fieldTheme);
            }

            #endregion

            //Close form
            this.Close();

            GSC_ProjectEditor.Messages.ShowEndOfProcess();
        }

        /// <summary>
        /// Will build a dictionnary of matchup between input features and output features
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> get_FeatureClass_GDB_DicoInOut()
        {
            //Build a dictionary of input and output
            Dictionary<string, string> FC_GDBDico = new Dictionary<string, string>();

            FC_GDBDico[gFCLinework] = fcLinework;
            FC_GDBDico[gFCStation] = fcStation;
            FC_GDBDico[gFCTraverses] = fcTraverse;

            return FC_GDBDico;
        }

        /// <summary>
        /// Will build a dictionnary of matchup between input tables and output tables
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> get_Table_GDB_DicoInOut()
        {
            //Build a dictionary of input and output
            Dictionary<string, string> Table_GDBDico = new Dictionary<string, string>();

            Table_GDBDico[gTEarthmat] = tEarthMat;
            Table_GDBDico[gTMA] = tMA;
            Table_GDBDico[gTMineral] = tMineral;
            Table_GDBDico[gTMetadata] = tMetadata;
            Table_GDBDico[gTPhoto] = tPhoto;
            Table_GDBDico[gTSample] = tSample;
            Table_GDBDico[gTStruc] = tStruc;
            Table_GDBDico[gTEnviron] = tEnviron;
            Table_GDBDico[gTSoilPro] = tSoilPro;
            Table_GDBDico[gTPFlow] = tPflow;


            return Table_GDBDico;
        }

        /// <summary>
        /// Will build a dictionnary of matchup between input tables and output tables
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> get_Table_GDB_SHP_InOut(List<string> shapeList)
        {
            //Build a dictionary of input and output
            Dictionary<string, string> Table_GDBDico = new Dictionary<string, string>();

            //Iterate through input list of shapefiles
            foreach(string files in shapeList)
            {
                if (files.Contains(shpEarthMat))
                {
                    Table_GDBDico[gTEarthmat] = files;
                }

                if (files.Contains(shpMA))
                {
                    Table_GDBDico[gTMA] = files;
                }

                if (files.Contains(shpMineral))
                {
                    Table_GDBDico[gTMineral] = files;
                }

                if (files.Contains(dbfMetadata))
                {
                    Table_GDBDico[gTMetadata] = files;
                }

                if (files.Contains(shpPhoto))
                {
                    Table_GDBDico[gTPhoto] = files;
                }

                if (files.Contains(shpSample))
                {
                    Table_GDBDico[gTSample] = files;
                }

                if (files.Contains(shpStruc))
                {
                    Table_GDBDico[gTStruc] = files;
                }

                if (files.Contains(shpEnviron))
                {
                    Table_GDBDico[gTEnviron] = files;
                }

                if (files.Contains(shpPflow))
                {
                    Table_GDBDico[gTPFlow] = files;
                }

                if (files.Contains(shpSoilPro))
                {
                    Table_GDBDico[gTSoilPro] = files;
                }
            }

            return Table_GDBDico;
        }

        /// <summary>
        /// Will build a dictionnary of matchup between input features and output features
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> get_FeatureClass_GDB_SHP_InOut(List<string> shapeList)
        {
            //Build a dictionary of input and output
            Dictionary<string, string> FC_GDBDico = new Dictionary<string, string>();

            //Iterate through input list of shapefiles
            foreach (string files in shapeList)
            {
                if (files.Contains(shpLinework))
                {
                    FC_GDBDico[gFCLinework] = files;
                }

                if (files.Contains(shpStation))
                {
                    FC_GDBDico[gFCStation] = files;
                }

                if (files.Contains(shpTraverse))
                {
                    FC_GDBDico[gFCTraverses] = files;
                }
            }

            return FC_GDBDico;
        }

        /// <summary>
        /// Will add a new row inside legend generator table 
        /// </summary>
        /// <param name="inTheme"></param>
        /// <param name="newTheme"></param>
        private void AddToLegendTable(string themeName, string themeCode)
        {
            Dictionary<string, object> newRowDico = new Dictionary<string, object>();
            newRowDico[legendTableID] = Guid.NewGuid().ToString();
            newRowDico[legendTableSymbol] = GSC_ProjectEditor.Constants.Styles.InvalidPoint_FGDC;
            newRowDico[legendTableName] = themeName;
            newRowDico[legendTableSymType] = legendTableSymTypePoint;
            newRowDico[legendTableSymTheme] = themeCode;
            GSC_ProjectEditor.Tables.AddRowWithValues(legendTable, newRowDico);
        }
    }
}
