using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Globalization;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Desktop.AddIns;
using ESRI.ArcGIS.Geometry;

namespace GSC_ProjectEditor
{
    public partial class Form_Environment_UpgradeGSCBedrockGeodatabase : Form
    {
        #region Variables

        //Tables to updates
        private const string participantTable = GSC_ProjectEditor.Constants.Database.TParticipant;
        private const string personTable = GSC_ProjectEditor.Constants.Database.TPerson;
        private const string legenDescTable = GSC_ProjectEditor.Constants.Database.TLegendDescription;
        private const string studyIndexTable = GSC_ProjectEditor.Constants.Database.TStudyAreaIndex;
        private const string sourceTable = GSC_ProjectEditor.Constants.Database.TSource;
        private const string organisationTable = GSC_ProjectEditor.Constants.Database.TOrganisation;
        private const string legendGenTable = GSC_ProjectEditor.Constants.Database.TLegendGene;
        private const string projectTable = GSC_ProjectEditor.Constants.Database.TProject;
        private const string activityMTable = GSC_ProjectEditor.Constants.Database.TMActivity;
        private const string subactivityTable = GSC_ProjectEditor.Constants.Database.TSActivity;
        private const string metadataTable = GSC_ProjectEditor.Constants.Database.TMetadata;
        private const string geolineSymTable = GSC_ProjectEditor.Constants.Database.TGeolineSymbol;
        private const string geopointSymTable = GSC_ProjectEditor.Constants.Database.TGeopointSymbol;
        private const string legendTreeTable = GSC_ProjectEditor.Constants.Database.TLegendTree;
        private const string extentedAttrTable = GSC_ProjectEditor.Constants.Database.TExtenAttrb;
        private const string ganEarthmatTable = GSC_ProjectEditor.Constants.Database.gEarthMath;
        private const string ganMATable = GSC_ProjectEditor.Constants.Database.gMA;
        private const string ganMetaTable = GSC_ProjectEditor.Constants.Database.gMetadata;
        private const string ganMineTable = GSC_ProjectEditor.Constants.Database.gMineral;
        private const string ganSampleTable = GSC_ProjectEditor.Constants.Database.gSample;
        private const string ganStrucTable = GSC_ProjectEditor.Constants.Database.gStruc;
        private const string ganPhotoTable = GSC_ProjectEditor.Constants.Database.gPhoto;
        private const string tGeoEvent = GSC_ProjectEditor.Constants.Database.TGeoEvent;

        //Table legacy for 141003
        private const string subactivityTable_01 = GSC_ProjectEditor.Constants.Database.TSActivity_141003;
        private const string legendTreeTable_01 = GSC_ProjectEditor.Constants.Database.TLegendTree_141003;
        private const string legendTreeTable_02 = GSC_ProjectEditor.Constants.Database.TLegendTree_160411;
        private const string extentedAttrTable_01 = GSC_ProjectEditor.Constants.Database.TExtenAttrb_141003;
        private const string geolineSymTable_01 = GSC_ProjectEditor.Constants.Database.TGeolineSymbol_141003;
        private const string geopointSymTable_01 = GSC_ProjectEditor.Constants.Database.TGeopointSymbol_141003;
        private const string legendGenTable_01 = GSC_ProjectEditor.Constants.Database.TLegendGene_160411;
        private const string tGeoEvent_01 = GSC_ProjectEditor.Constants.Database.TGeoEvent_160915;
        private const string organisationTable_01 = GSC_ProjectEditor.Constants.Database.TOrganisation_160915;

        //Table field to update
        private const string geolineSymTable_SelectCode = GSC_ProjectEditor.Constants.DatabaseFields.MGeolineSelectCode;
        private const string geopointSymTable_SelectCode = GSC_ProjectEditor.Constants.DatabaseFields.TGeopointSelectCode;
        private const string geoline_ID = GSC_ProjectEditor.Constants.DatabaseFields.MGeolineID;
        private const string geopoint_ID = GSC_ProjectEditor.Constants.DatabaseFields.TGeopointID;
        private const string geopointLegendDescSymbol = GSC_ProjectEditor.Constants.DatabaseFields.TGeopointLegendDesc;
        private const string geopointLegendFGDCSymbol = GSC_ProjectEditor.Constants.DatabaseFields.TGeopointFGDC;
        private const string geolineLegendDescSymbol = GSC_ProjectEditor.Constants.DatabaseFields.MGeolineLegendDescription;
        private const string geolineLegendFGDCSymbol = GSC_ProjectEditor.Constants.DatabaseFields.MGeolineFGDC;
        private const string studyAreaIndexFC = GSC_ProjectEditor.Constants.DatabaseFields.TStudyAreaFC;
        private const string studyAreaIndexTable = GSC_ProjectEditor.Constants.DatabaseFields.TStudyAreaName;
        private const string participantID = GSC_ProjectEditor.Constants.DatabaseFields.ParticipantID;
        private const string participantPersonID = GSC_ProjectEditor.Constants.DatabaseFields.ParticipantPersonID;
        private const string participantMAID = GSC_ProjectEditor.Constants.DatabaseFields.ParticipantMActID;
        private const string participantSAID = GSC_ProjectEditor.Constants.DatabaseFields.ParticipantSActID;
        private const string participantRoleID = GSC_ProjectEditor.Constants.DatabaseFields.ParticipantRole;
        private const string legendGeneratorID = GSC_ProjectEditor.Constants.DatabaseFields.LegendLabelID;
        private const string legendGeneratorName = GSC_ProjectEditor.Constants.DatabaseFields.LegendLabelName;
        private const string legendGeneratorSymbolCode = GSC_ProjectEditor.Constants.DatabaseFields.LegendSymbol;
        private const string legendGeneratorSymbolType = GSC_ProjectEditor.Constants.DatabaseFields.LegendSymType;
        private const string legendGeneratorGISDisplay = GSC_ProjectEditor.Constants.DatabaseFields.LegendGISDisplay;
        private const string legendGeneratorSymbolTheme = GSC_ProjectEditor.Constants.DatabaseFields.LegendItemType;
        private const string personID = GSC_ProjectEditor.Constants.DatabaseFields.PersonID;
        private const string personAlias = GSC_ProjectEditor.Constants.DatabaseFields.PersonAlias;
        private const string projectID = GSC_ProjectEditor.Constants.DatabaseFields.ProjectID;
        private const string projectCode = GSC_ProjectEditor.Constants.DatabaseFields.ProjectCode;
        private const string mActivityProjectID = GSC_ProjectEditor.Constants.DatabaseFields.MainActProjectID;
        private const string legendGeneratorEon = GSC_ProjectEditor.Constants.DatabaseFields.LegendEon_160330;
        private const string legendGeneratorEra = GSC_ProjectEditor.Constants.DatabaseFields.LegendEra_160330;
        private const string legendGeneratorPeriod = GSC_ProjectEditor.Constants.DatabaseFields.LegendPeriod_160330;
        private const string geoEventName = GSC_ProjectEditor.Constants.DatabaseFields.TGeoEventName;
        private const string geoEventID = GSC_ProjectEditor.Constants.DatabaseFields.TGeoEventID;
        private const string geoEventMinAge = GSC_ProjectEditor.Constants.DatabaseFields.TGeoEventAgeMinTimescale;
        private const string geoEventMaxAge = GSC_ProjectEditor.Constants.DatabaseFields.TGeoEventAgeMaxTimescale;
        private const string geoEventMinAgeValue = GSC_ProjectEditor.Constants.DatabaseFields.TGeoEventAgeMinValue;
        private const string geoEventMaxAgeValue = GSC_ProjectEditor.Constants.DatabaseFields.TGeoEventAgeMaxValue;
        private const string geolineGeoEvent = GSC_ProjectEditor.Constants.DatabaseFields.FGeolineGeoEventID;
        private const string labelGeoEvent = GSC_ProjectEditor.Constants.DatabaseFields.FLabelGeoEventID;
        private const string labelID = GSC_ProjectEditor.Constants.DatabaseFields.FLabelID;
        private const string geolineMinAge = GSC_ProjectEditor.Constants.DatabaseFields.FGeolineMinAge_160101;
        private const string geolineMaxAge = GSC_ProjectEditor.Constants.DatabaseFields.FGeolineMaxAge_160101;
        private const string cartoFCTheme = GSC_ProjectEditor.Constants.DatabaseFields.FCartoPointTheme;
        private const string cartoFCLegendID = GSC_ProjectEditor.Constants.DatabaseFields.FCartoPointLegendID;

        //Features to update
        private const string geolineFC = GSC_ProjectEditor.Constants.Database.FGeoline;
        private const string geopointFC = GSC_ProjectEditor.Constants.Database.FGeopoint;
        private const string geopolyFC = GSC_ProjectEditor.Constants.Database.FGeopoly;
        private const string labelFC = GSC_ProjectEditor.Constants.Database.FLabel;
        private const string stationFC = GSC_ProjectEditor.Constants.Database.gFCStation;
        private const string studyAreaFC = GSC_ProjectEditor.Constants.Database.FStudyArea;
        private const string cartoFC = GSC_ProjectEditor.Constants.Database.FCartoPoint;
        private const string cgmFC = GSC_ProjectEditor.Constants.Database.FCGMIndex;
        private const string lineworkFC = GSC_ProjectEditor.Constants.Database.gFCLinework;
        private const string traversesFC = GSC_ProjectEditor.Constants.Database.gFCTraverses;

        //Feature legacy for 141003
        private const string labelFC_01 = GSC_ProjectEditor.Constants.Database.FLabel_141003;
        private const string cgmFC_01 = GSC_ProjectEditor.Constants.Database.FCGMIndex_160411;

        //Domains values
        private const string selectedSymbol = GSC_ProjectEditor.Constants.DatabaseDomainsValues.BoolYes;
        private const string fillSymbolType = GSC_ProjectEditor.Constants.DatabaseDomainsValues.SymTypeFill;
        private const string lineSymbolType = GSC_ProjectEditor.Constants.DatabaseDomainsValues.SymTypeLine;
        private const string pointSymbolType = GSC_ProjectEditor.Constants.DatabaseDomainsValues.SymTypePoint;
        private const string headerSymbolType = GSC_ProjectEditor.Constants.DatabaseDomainsValues.SymTypeHeader1;
        private const string symbolThemeHeaders = GSC_ProjectEditor.Constants.DatabaseDomainsValues.legendItemHeader;
        private const string symbolThemeGeopoint = GSC_ProjectEditor.Constants.DatabaseDomainsValues.legendItemGeopoint;
        private const string symbolThemeGeoline = GSC_ProjectEditor.Constants.DatabaseDomainsValues.legendItemGeoline;
        private const string symbolThemeMapUnits = GSC_ProjectEditor.Constants.DatabaseDomainsValues.legendItemMapUnit;

        //Folder
        private const string templateFolder = GSC_ProjectEditor.Constants.Folders.mxdFolder;

        //Folder legacy
        private const string templateFolder_01 = GSC_ProjectEditor.Constants.Folders.mxdFolder_160415;

        //Editor tracking
        private const string createNameField = GSC_ProjectEditor.Constants.DatabaseFields.ETCreatorID;
        private const string editorNameField = GSC_ProjectEditor.Constants.DatabaseFields.ETEditorID;
        private const string createDateField = GSC_ProjectEditor.Constants.DatabaseFields.ETCreateDate;
        private const string editorDateField = GSC_ProjectEditor.Constants.DatabaseFields.ETEditDate;

        //Other
        private const string oidField = GSC_ProjectEditor.Constants.DatabaseFields.ObjectID;
        private const string roleDomain = GSC_ProjectEditor.Constants.DatabaseDomains.ActivityRole;
        private const string partDomain = GSC_ProjectEditor.Constants.DatabaseDomains.participant;
        private const string orgAbbrev = GSC_ProjectEditor.Constants.DatabaseFields.OrganisationAbbr;
        private const string orgID = GSC_ProjectEditor.Constants.DatabaseFields.OrganisationID;
        private const string ageDesignatorDomain = GSC_ProjectEditor.Constants.DatabaseDomains.ageDesignator;
        public NumberStyles style = NumberStyles.Number; //Will be used to force parse for numbers

        public List<string> currentOldTableList { get; set; }

        #endregion

        public Form_Environment_UpgradeGSCBedrockGeodatabase()
        {
            //Set culture before init.
            Utilities.Culture.SetCulture();

            InitializeComponent();
        }

        #region MODEL
        public void MainUpdateProcess(string oldDBPath, string newBDPath)
        {
            //Get a list of all features to enable/disable the editor on
            List<string> etFeatureList = new List<string> { geolineFC, geopointFC, geopolyFC, labelFC, cartoFC };

            //Get workspace of old and new database
            IWorkspace newWorkspace = GSC_ProjectEditor.Workspace.AccessWorkspace(newBDPath);
            IWorkspace oldWorkspace = GSC_ProjectEditor.Workspace.AccessWorkspace(oldDBPath);

            //Disable editor tracking
            GSC_ProjectEditor.Workspace.DisableEditorTrackingFromWorkspace(newWorkspace, etFeatureList);

            //Get a list of table to update
            Dictionary<string, List<string>> tableToUpdate = GetTableToUpdateList();

            //Update and warn about missing domain values
            //Version 2.8 to 2.9
            Dictionary<string, List<string>> missingDomValues = new Dictionary<string, List<string>>(); //Will be used to update domains
            missingDomValues[GSC_ProjectEditor.Constants.DatabaseDomains.qualifLimitSID] = new List<string>() { "5064", "5065" };
            missingDomValues[GSC_ProjectEditor.Constants.DatabaseDomains.qualifFaultSID] = new List<string>() { "2010", "2011" };
            missingDomValues[GSC_ProjectEditor.Constants.DatabaseDomains.qualifOverprintSID] = new List<string>() { "6007" };
            missingDomValues[GSC_ProjectEditor.Constants.DatabaseDomains.qualifShearSID] = new List<string>() { "2058" };
            missingDomValues[GSC_ProjectEditor.Constants.DatabaseDomains.qualifContactSID] = new List<string>() { "1009"};
            missingDomValues[GSC_ProjectEditor.Constants.DatabaseDomains.qualifConstructSID] = new List<string>() { "5003", "5006" };
            //missingDomValues[GSC_ProjectEditor.Constants.DatabaseDomains.qualifLineamSID] = new List<string>() { "2075", "2076", "2077" };
            missingDomValues[GSC_ProjectEditor.Constants.DatabaseDomains.subsetPlanarSID] = new List<string>() { "1021", "1054" };
            missingDomValues[GSC_ProjectEditor.Constants.DatabaseDomains.subsetLinearSID] = new List<string>() { "2004"};
            missingDomValues[GSC_ProjectEditor.Constants.DatabaseDomains.strucGenerationSID] = new List<string>() { "99" };
            missingDomValues[GSC_ProjectEditor.Constants.DatabaseDomains.strucAttitudeLinear] = new List<string>() { "05", "10" };
            missingDomValues[GSC_ProjectEditor.Constants.DatabaseDomains.strucAttitudeLinear] = new List<string>() { "05", "10" };
            missingDomValues[GSC_ProjectEditor.Constants.DatabaseDomains.genUndefinedSID] = new List<string>() { "88" }; //Didn't find where the dom was used...

            //Will be used to detect feature values
            Dictionary<string, List<string>> missingGeolineFieldValues = new Dictionary<string, List<string>>();
            missingGeolineFieldValues[GSC_ProjectEditor.Constants.DatabaseFields.FGeolineQualif] = new List<string>() { "5064", "5065", "2010", "2011", "6007", "2058", "1009", "5003", "5006" };

            Dictionary<string, List<string>> missingGeopointFieldValues = new Dictionary<string, List<string>>();
            missingGeopointFieldValues[GSC_ProjectEditor.Constants.DatabaseFields.FGeopointSubset] = new List<string>() { "1021", "1054", "2004" };
            missingGeopointFieldValues[GSC_ProjectEditor.Constants.DatabaseFields.FGeopointStrucGene] = new List<string>() { "99" };
            
            //Will ve used to convert feature domain values
            string constraintQuery = GSC_ProjectEditor.Constants.DatabaseFields.FGeopointType + " = " + GSC_ProjectEditor.Constants.DatabaseDomainsValues.geopointLinear;
            Dictionary<string, List<Tuple<string, string, string>>> ConvertedGeopointFieldValues = new Dictionary<string,List<Tuple<string,string, string>>>();
            ConvertedGeopointFieldValues[GSC_ProjectEditor.Constants.DatabaseFields.FGeopointStrucAtt] = new List<Tuple<string, string, string>>() { new Tuple<string, string, string>("05", "99", constraintQuery), new Tuple<string, string, string>("10", "99", constraintQuery) };

            string constraintQueryGeoline = GSC_ProjectEditor.Constants.DatabaseFields.FGeolineSubtype + " = " + GSC_ProjectEditor.Constants.DatabaseDomainsValues.GeolineTrace;
            Dictionary<string, List<Tuple<string, string, string>>> ConvertedGeolineFieldValues = new Dictionary<string, List<Tuple<string, string, string>>>();
            ConvertedGeolineFieldValues[GSC_ProjectEditor.Constants.DatabaseFields.FGeolineQualif] = new List<Tuple<string, string, string>>() { new Tuple<string, string, string>("2075", "5036", constraintQueryGeoline), new Tuple<string, string, string>("2076", "5036", constraintQueryGeoline), new Tuple<string, string, string>("2077", "5036", constraintQueryGeoline) };

            //Will be used to convert feature subtype values
            string constraintQueryGeolineSubtype = GSC_ProjectEditor.Constants.DatabaseFields.FGeolineSubtype + " = " + GSC_ProjectEditor.Constants.DatabaseDomainsValues.GeolineStructureLineaments;
            Dictionary<string, List<Tuple<string, string, string>>> ConvertedGeolineSubtypeValues = new Dictionary<string, List<Tuple<string, string, string>>>();
            ConvertedGeolineSubtypeValues[GSC_ProjectEditor.Constants.DatabaseFields.FGeolineSubtype] = new List<Tuple<string, string, string>>() { new Tuple<string, string, string>("13", "17", constraintQueryGeolineSubtype) };

            //Update domains
            UpdateDomains(oldWorkspace, newWorkspace, missingDomValues);

            //Update P_PERSON id field and alias field
            //If the table gets updated, the new list of tables to update will miss the person table
            //Version 2.5 to 2.6
            Dictionary<string, int> dicoParticipantPersonID = new Dictionary<string, int>();
            UpdatePersonID(newWorkspace, oldWorkspace, tableToUpdate, out tableToUpdate, out dicoParticipantPersonID);

            //Update P_PARTICIPANT personID field if needed
            //If the table gets updates, the new list of table to update will miss the participant table
            //Version 2.5 to 2.6
            if (dicoParticipantPersonID.Count != 0)
            {
                UpdateParticipantPersonID(newWorkspace, oldWorkspace, tableToUpdate, out tableToUpdate, dicoParticipantPersonID);
            }

            //Update M_PROJECT id field and project code field
            //If the table gets updated, the new list of tables to update will miss the project table
            //Version 2.5 to 2.6
            Dictionary<string, int> dicoMactivityProjectID = new Dictionary<string, int>();
            UpdateProjectID(newWorkspace, oldWorkspace, tableToUpdate, out tableToUpdate, out dicoMactivityProjectID);

            //Update M_ACTIVITY projectID field if needed
            //If the table gets updates, the new list of table to update will miss the participant table
            //Version 2.5 to 2.6
            if (dicoMactivityProjectID.Count != 0)
            {
                UpdateMactivityProjectID(newWorkspace, oldWorkspace, tableToUpdate, out tableToUpdate, dicoMactivityProjectID);
            }

            //Update P_ORGANISATION without duplicating ids
            //The table will then be removed the update list 
            //Version 2.6 to 2.7
            UpdateOrganisationNoDuplicate(newWorkspace, oldWorkspace, tableToUpdate, out tableToUpdate);

            //Update tables
            List<string> oldTableList = new List<string>();
            UpdateTables(oldWorkspace, newWorkspace, tableToUpdate, out oldTableList);

            //Update P_ORGANISATION abbreviation field with id value if string.empty
            //Version 2.5 to 2.6
            if (oldTableList.Count != 0)
            {
                UpdateOrganisationAbbreviation(newWorkspace);
            }

            //Update features
            UpdateFeaturesPreProcess(oldWorkspace, newWorkspace);

            //Validate for any other information stored by the user but not coming from the Project Database Schema
            CheckForOtherData(newWorkspace, oldWorkspace);

            //Recalculate table names inside P_STUDY_AREA_INDEX
            UpdateStudyAreaIndex(newWorkspace);

            //Update participantID field inside P_PARTICIPANT table in case it's empty
            //Version 2.5 to 2.6
            UpdateEmptyParticipantID(newWorkspace);

            //Update P_LEGEND_GENERATOR with information contained inside SelectCode fields from symbol tables, if needed
            //Version 2.3 to 2.4
            UpdateLegendGeneratorSymbols(oldTableList, oldWorkspace, newWorkspace);

            //Update P_LEGEND_GENERATOR item type (theme) if needed
            ////Version 2.6 to 2.7
            UpdateLegendGeneratorSymbolTheme(tableToUpdate, oldWorkspace, newWorkspace);

            //Update GEO_EVENT table from P_LEGEND_GENERATOR Eon/Era/Period fields
            //Version 2.6 to 2.7
            UpdateGeoEventFromLegend(tableToUpdate, oldWorkspace, newWorkspace);

            //Update GEO_EVENT table from GEO_LINE feature class MinAge and MaxAge fields
            //Version 2.6 to 2.7
            UpdateGeoEventFromGeolineAge(oldWorkspace, newWorkspace);

            //Update workspace folder names
            //Version 2.6 to 2.7
            UpdateFolderNames();

            //Update CARTOGRAPHIC_POINT with legendItemIDs coming from P_LEGEND
            //Version 2.7 to 2.8
            UpdateCartographicPointLegendIds(newWorkspace);

            //Missing dom values 2.8 to 2.9
            WarnAboutMissingDomainValue(newWorkspace, geolineFC, missingGeolineFieldValues); //Remove missing dom values and warning user about lines and points that will be invalid
            WarnAboutMissingDomainValue(newWorkspace, geopointFC, missingGeopointFieldValues); //Remove missing dom values and warning user about lines and points that will be invalid

            //Convert subtype values 2.8 to 2.9
            ConvertMissingSubtypeValue(newWorkspace, geolineFC, ConvertedGeolineSubtypeValues);

            //Convert dom values 2.8 to 2.9
            ConvertMissingDomainValue(newWorkspace, geopointFC, ConvertedGeopointFieldValues);
            ConvertMissingDomainValue(newWorkspace, geolineFC, ConvertedGeolineFieldValues);

            //Removed missing dom values, that were added first to loading tables and featues could work
            CleanDomain(newWorkspace, missingDomValues);

            //Recalculate some new symbol from SYMBOL_GEOLINES 2.9 to 2.10
            RecalculateGeolineSymbols(newWorkspace, geolineFC, legendGenTable);

            //Enable editor tracking
            GSC_ProjectEditor.Workspace.EnabledEditorTrackingFromWorkspace(newWorkspace, etFeatureList, null, createDateField, null, editorDateField);

        }

        /// <summary>
        /// Will convert from a given list all geolines symbols into the new ones
        /// to match latest SYMBOL_GEOLINE update. Will also do the same thing in P_LEGEND
        /// From 2.9 to 2.10
        /// </summary>
        private void RecalculateGeolineSymbols(IWorkspace inWorkspace, string inFeatureClassName, string inTableName)
        {
            #region hardcoded values to change
            Dictionary<string, string> newGeolineSymbols = new Dictionary<string, string>();
            newGeolineSymbols["154004019999"] = "4.05.04.001";
            newGeolineSymbols["154004029999"] = "4.05.04.001";
            newGeolineSymbols["154004039999"] = "4.05.04.001";
            newGeolineSymbols["154004049999"] = "4.05.04.001";
            newGeolineSymbols["154005019999"] = "4.05.04.001";
            newGeolineSymbols["154005029999"] = "4.05.04.001";
            newGeolineSymbols["154005039999"] = "4.05.04.001";
            newGeolineSymbols["154005049999"] = "4.05.04.001";
            newGeolineSymbols["154007019999"] = "4.05.02.001";
            newGeolineSymbols["154007029999"] = "4.05.02.001";
            newGeolineSymbols["154007039999"] = "4.05.02.001";
            newGeolineSymbols["154007049999"] = "4.05.02.001";
            newGeolineSymbols["175029019999"] = "4.06.03.001";
            newGeolineSymbols["175029029999"] = "4.06.03.001";
            newGeolineSymbols["175029039999"] = "4.06.03.001";
            newGeolineSymbols["175029049999"] = "4.06.03.001";
            newGeolineSymbols["175030019999"] = "4.06.03.002";
            newGeolineSymbols["175030029999"] = "4.06.03.002";
            newGeolineSymbols["175030039999"] = "4.06.03.002";
            newGeolineSymbols["175030049999"] = "4.06.03.002";
            newGeolineSymbols["175031019999"] = "4.06.04.001";
            newGeolineSymbols["175031029999"] = "4.06.04.001";
            newGeolineSymbols["175031039999"] = "4.06.04.001";
            newGeolineSymbols["175031049999"] = "4.06.04.001";
            newGeolineSymbols["175032019999"] = "4.06.05.001";
            newGeolineSymbols["175032029999"] = "4.06.05.001";
            newGeolineSymbols["175032039999"] = "4.06.05.001";
            newGeolineSymbols["175032049999"] = "4.06.05.001";
            newGeolineSymbols["175033019999"] = "4.06.05.001";
            newGeolineSymbols["175033029999"] = "4.06.05.001";
            newGeolineSymbols["175033039999"] = "4.06.05.001";
            newGeolineSymbols["175033049999"] = "4.06.05.001";
            newGeolineSymbols["175034019999"] = "4.06.06.001";
            newGeolineSymbols["175034029999"] = "4.06.06.001";
            newGeolineSymbols["175034039999"] = "4.06.06.001";
            newGeolineSymbols["175034049999"] = "4.06.06.001";
            newGeolineSymbols["175035019999"] = "4.06.06.001";
            newGeolineSymbols["175035029999"] = "4.06.06.001";
            newGeolineSymbols["175035039999"] = "4.06.06.001";
            newGeolineSymbols["175035049999"] = "4.06.06.001";
            newGeolineSymbols["175037019999"] = "4.07.01.001";
            newGeolineSymbols["175037029999"] = "4.07.01.001";
            newGeolineSymbols["175037039999"] = "4.07.01.001";
            newGeolineSymbols["175037049999"] = "4.07.01.001";
            newGeolineSymbols["175038019999"] = "4.06.04.001";
            newGeolineSymbols["175038029999"] = "4.06.04.001";
            newGeolineSymbols["175038039999"] = "4.06.04.001";
            newGeolineSymbols["175038049999"] = "4.06.04.001";
            newGeolineSymbols["185050019999"] = "4.11.01.001";
            newGeolineSymbols["185050029999"] = "4.11.01.001";
            newGeolineSymbols["185050039999"] = "4.11.01.001";
            newGeolineSymbols["185050049999"] = "4.11.01.001";
            newGeolineSymbols["185051019999"] = "4.11.01.002";
            newGeolineSymbols["185051029999"] = "4.11.01.002";
            newGeolineSymbols["185051039999"] = "4.11.01.002";
            newGeolineSymbols["185051049999"] = "4.11.01.002";
            newGeolineSymbols["185052019999"] = "4.11.01.003";
            newGeolineSymbols["185052029999"] = "4.11.01.003";
            newGeolineSymbols["185052039999"] = "4.11.01.003";
            newGeolineSymbols["185052049999"] = "4.11.01.003";
            newGeolineSymbols["185053019999"] = "4.11.01.004";
            newGeolineSymbols["185053029999"] = "4.11.01.004";
            newGeolineSymbols["185053039999"] = "4.11.01.004";
            newGeolineSymbols["185053049999"] = "4.11.01.004";
            newGeolineSymbols["185054019999"] = "4.11.01.005";
            newGeolineSymbols["185054029999"] = "4.11.01.005";
            newGeolineSymbols["185054039999"] = "4.11.01.005";
            newGeolineSymbols["185054049999"] = "4.11.01.005";
            newGeolineSymbols["185055019999"] = "4.11.01.006";
            newGeolineSymbols["185055029999"] = "4.11.01.006";
            newGeolineSymbols["185055039999"] = "4.11.01.006";
            newGeolineSymbols["185055049999"] = "4.11.01.006";
            newGeolineSymbols["185056019999"] = "4.11.01.006";
            newGeolineSymbols["185056029999"] = "4.11.01.006";
            newGeolineSymbols["185056039999"] = "4.11.01.006";
            newGeolineSymbols["185056049999"] = "4.11.01.006";
            newGeolineSymbols["185057019999"] = "4.11.01.007";
            newGeolineSymbols["185057029999"] = "4.11.01.007";
            newGeolineSymbols["185057039999"] = "4.11.01.007";
            newGeolineSymbols["185057049999"] = "4.11.01.007";
            newGeolineSymbols["185058019999"] = "4.01.01.001";
            newGeolineSymbols["185058029999"] = "4.01.01.002";
            newGeolineSymbols["185058039999"] = "4.01.01.003";
            newGeolineSymbols["185058049999"] = "4.01.01.004";
            newGeolineSymbols["185059019999"] = "4.11.01.007";
            newGeolineSymbols["185059029999"] = "4.11.01.007";
            newGeolineSymbols["185059039999"] = "4.11.01.007";
            newGeolineSymbols["185059049999"] = "4.11.01.007";
            newGeolineSymbols["185060019999"] = "4.11.01.007";
            newGeolineSymbols["185060029999"] = "4.11.01.007";
            newGeolineSymbols["185060039999"] = "4.11.01.007";
            newGeolineSymbols["185060049999"] = "4.11.01.007";
            newGeolineSymbols["185061019999"] = "4.11.01.008";
            newGeolineSymbols["185061029999"] = "4.11.01.008";
            newGeolineSymbols["185061039999"] = "4.11.01.008";
            newGeolineSymbols["185061049999"] = "4.11.01.008";
            newGeolineSymbols["185062019999"] = "4.11.01.009";
            newGeolineSymbols["185062029999"] = "4.11.01.009";
            newGeolineSymbols["185062039999"] = "4.11.01.009";
            newGeolineSymbols["185062049999"] = "4.11.01.009";
            newGeolineSymbols["185063019999"] = "4.11.01.009";
            newGeolineSymbols["185063029999"] = "4.11.01.009";
            newGeolineSymbols["185063039999"] = "4.11.01.009";
            newGeolineSymbols["185063049999"] = "4.11.01.009";
            newGeolineSymbols["196001019999"] = "4.11.03.005";
            newGeolineSymbols["196001029999"] = "4.11.03.005";
            newGeolineSymbols["196001039999"] = "4.11.03.005";
            newGeolineSymbols["196001049999"] = "4.11.03.005";
            newGeolineSymbols["196002019999"] = "4.11.03.001";
            newGeolineSymbols["196002029999"] = "4.11.03.001";
            newGeolineSymbols["196002039999"] = "4.11.03.001";
            newGeolineSymbols["196002049999"] = "4.11.03.001";
            newGeolineSymbols["196003019999"] = "4.11.03.002";
            newGeolineSymbols["196003029999"] = "4.11.03.002";
            newGeolineSymbols["196003039999"] = "4.11.03.002";
            newGeolineSymbols["196003049999"] = "4.11.03.002";
            newGeolineSymbols["196004019999"] = "4.11.03.003";
            newGeolineSymbols["196004029999"] = "4.11.03.003";
            newGeolineSymbols["196004039999"] = "4.11.03.003";
            newGeolineSymbols["196004049999"] = "4.11.03.003";
            newGeolineSymbols["196005019999"] = "4.11.02.009";
            newGeolineSymbols["196005029999"] = "4.11.02.009";
            newGeolineSymbols["196005039999"] = "4.11.02.009";
            newGeolineSymbols["196005049999"] = "4.11.02.009";
            newGeolineSymbols["196006019999"] = "4.11.02.009";
            newGeolineSymbols["196006029999"] = "4.11.02.009";
            newGeolineSymbols["196006039999"] = "4.11.02.009";
            newGeolineSymbols["196006049999"] = "4.11.02.009";
            newGeolineSymbols["196008019999"] = "4.11.03.004";
            newGeolineSymbols["196008029999"] = "4.11.03.004";
            newGeolineSymbols["196008039999"] = "4.11.03.004";
            newGeolineSymbols["196008049999"] = "4.11.03.004";
            newGeolineSymbols["196009019999"] = "4.11.03.006";
            newGeolineSymbols["196009029999"] = "4.11.03.006";
            newGeolineSymbols["196009039999"] = "4.11.03.006";
            newGeolineSymbols["196009049999"] = "4.11.03.006";
            #endregion

            List<string> tableFieldValues = GSC_ProjectEditor.Tables.GetFieldValuesFromWorkspace(inWorkspace, inFeatureClassName, geoline_ID, null);
            foreach (KeyValuePair<string, string> lines in newGeolineSymbols)
            {
                if (tableFieldValues.Contains(lines.Key))
                {
                    GSC_ProjectEditor.Messages.ShowGenericErrorMessage("New GEOLINE symbol for item " + lines.Key + ". Converted value will be set as: " + lines.Value);

                    string whereQuery = geoline_ID + " = '" + lines.Key + "'";
                    string whereLegendQuery = legendGeneratorID + " = '" + lines.Key + "'";

                    GSC_ProjectEditor.Tables.UpdateFieldValueFromWorkspace(inWorkspace, inFeatureClassName, geolineLegendFGDCSymbol, whereQuery, lines.Value);
                    GSC_ProjectEditor.Tables.UpdateFieldValueFromWorkspace(inWorkspace, inTableName, legendGeneratorSymbolCode, whereLegendQuery, lines.Value);
                }
            }

        }

        /// <summary>
        /// Will convert missing subtypes into existing or new ones.
        /// </summary>
        /// <param name="newWorkspace"></param>
        /// <param name="geolineFC"></param>
        /// <param name="ConvertedGeolineSubtypeValues"></param>
        private void ConvertMissingSubtypeValue(IWorkspace newWorkspace, string tableName, Dictionary<string, List<Tuple<string, string, string>>> fieldValuesToConvert)
        {
            foreach (KeyValuePair<string, List<Tuple<string, string, string>>> invalidPair in fieldValuesToConvert)
            {
                List<string> tableFieldValues = GSC_ProjectEditor.Tables.GetFieldValuesFromWorkspace(newWorkspace, tableName, invalidPair.Key, null);

                foreach (Tuple<string, string, string> invalidValue in invalidPair.Value)
                {
                    if (tableFieldValues.Contains(invalidValue.Item1))
                    {
                        GSC_ProjectEditor.Messages.ShowGenericErrorMessage("New database schema doesn't contain value " + invalidValue.Item1 + " in associated domain of field '" + invalidPair.Key + "' from feature/table '" + tableName + ". Converted value will be set as: " + invalidValue.Item2);

                        string whereQuery = invalidPair.Key + " = " + invalidValue.Item1 ;

                        if (invalidValue.Item3 != string.Empty)
                        {
                            whereQuery = whereQuery + " AND " + invalidValue.Item3;
                        }
                        GSC_ProjectEditor.Tables.UpdateFieldValueFromWorkspace(newWorkspace, tableName, invalidPair.Key, whereQuery, invalidValue.Item2);
                    }
                }
            }
        }


        /// <summary>
        /// Since there was a new added field inside cartographic point to related them to the legend.
        /// Will need to fill in this new field with proper IDs. 
        /// </summary>
        /// <param name="oldWorkspace"></param>
        /// <param name="newWorkspace"></param>
        private void UpdateCartographicPointLegendIds(IWorkspace newWorkspace)
        {
            //Get a dictionnary of legendItemTypes and legendItemIDs 
            string queryCartoTheme = legendGeneratorSymbolTheme + " LIKE 'cartographicPoint%'";
            Dictionary<string, string> legendCartoRaw = GSC_ProjectEditor.Tables.GetUniqueDicoValuesFromWorkspace(newWorkspace, legendGenTable, legendGeneratorSymbolTheme, legendGeneratorID, queryCartoTheme);

            //Process dictionnary to keep only needed information
            Dictionary<string, string> legendCarto = new Dictionary<string, string>();
            foreach (KeyValuePair<string, string> cItems in legendCartoRaw)
            {
                legendCarto[cItems.Key.ToLower().Replace("cartographicpoint_", "")] = cItems.Value;
            }

            //Iterate through cartographic point with an update cursor
            ICursor cartoFCUpCursor = GSC_ProjectEditor.Tables.GetTableCursorFromWorkspace(newWorkspace, "Update", string.Empty, cartoFC);
            IRow cartoFCRows = cartoFCUpCursor.NextRow();
            int cartoFCLegendIDIndex = cartoFCUpCursor.Fields.FindField(cartoFCLegendID);
            int cartFCLegendThemeIndex = cartoFCUpCursor.Fields.FindField(cartoFCTheme);
            while (cartoFCRows != null)
            {
                //Get wanted theme related legendItemID
                string currentTheme = cartoFCRows.get_Value(cartFCLegendThemeIndex).ToString().ToLower();

                //Get current legend item id as a validation
                if (cartoFCRows.get_Value(cartoFCLegendIDIndex) == DBNull.Value)
                {
                    //Update table with proper legend item id if found inside dictionary
                    if (legendCarto.ContainsKey(currentTheme))
                    {
                        cartoFCRows.set_Value(cartoFCLegendIDIndex, legendCarto[currentTheme]);
                        cartoFCUpCursor.UpdateRow(cartoFCRows);
                    }
                }

                cartoFCRows = cartoFCUpCursor.NextRow();
            }

            GSC_ProjectEditor.ObjectManagement.ReleaseObject(cartoFCUpCursor);
        }

        /// <summary>
        /// Will rename some folders, if needed, from their legacy version to the newest ones.
        /// </summary>
        private void UpdateFolderNames()
        {
            //Get list of folder legacy names
            Dictionary<string, List<string>> folderLegacyNames = GetFolderToUpdate();

            //Get a list of workspace sub folders
            string workspaceFolderPath = GSC_ProjectEditor.Properties.Settings.Default.PROJECT_WORKSPACE_PATH;
            List<string> workspaceSubFolders = GSC_ProjectEditor.FolderAndFiles.GetListOfFoldersFromFolder(workspaceFolderPath, "*");

            //Iterate through workspace directories
            foreach (KeyValuePair<string, List<string>> folderLeg in folderLegacyNames)
            {
                foreach (string legacyNames in folderLeg.Value)
                {
                    foreach (string subFolders in workspaceSubFolders)
                    {
                        //Find an old name
                        if (subFolders.Contains(legacyNames))
                        {
                            //Rename
                            string subFolderPath = System.IO.Path.Combine(workspaceFolderPath, subFolders);
                            string newSubFolderPath = System.IO.Path.Combine(workspaceFolderPath, folderLeg.Key);
                            if (!System.IO.Directory.Exists(newSubFolderPath))
                            {
                                Directory.Move(subFolderPath, newSubFolderPath);
                            }

                        }
                    }
                }

            }

        }

        /// <summary>
        /// Will convert MinAge and MaxAge fields, if needed, from geoline feature class into
        /// geo_events table. Since they are text field, if it fails to convert the value
        /// will remain inside the event name instead
        /// </summary>
        /// <param name="oldWorkspace">The old workspace</param>
        /// <param name="newWorkspace">The new workspace</param>
        private void UpdateGeoEventFromGeolineAge(IWorkspace oldWorkspace, IWorkspace newWorkspace)
        {

            //Get old geoline table
            ITable oldGeolines = GSC_ProjectEditor.Tables.OpenTableFromWorkspace(oldWorkspace, geolineFC);

            //Find old fields
            int minAgeIndex = oldGeolines.Fields.FindField(geolineMinAge);
            int maxAgeIndex = oldGeolines.Fields.FindField(geolineMaxAge);
            if (minAgeIndex != -1 && maxAgeIndex != -1)
            {
                Tuple<string, string, string> geolineFieldList = new Tuple<string, string, string>(GSC_ProjectEditor.Constants.DatabaseFields.ObjectID,
                    geolineMinAge, geolineMaxAge);
                Dictionary<string, Tuple<string, string>> minMaxesCollection = GSC_ProjectEditor.Tables.GetUniqueDicoTripleFieldValuesFromWorkspace(oldWorkspace, geolineFC, geolineFieldList, null);

                //Condition to having something
                if (minMaxesCollection.Count > 0)
                {
                    ICursor geoEventCursor = GSC_ProjectEditor.Tables.GetTableCursor("Insert", null, tGeoEvent);
                    ITable geoEventTableObject = GSC_ProjectEditor.Tables.OpenTable(tGeoEvent);
                    int eventIDIndex = geoEventCursor.Fields.FindField(geoEventID);
                    int eventNameIndex = geoEventCursor.Fields.FindField(geoEventName);
                    int eventMinValueIndex = geoEventCursor.Fields.FindField(geoEventMinAgeValue);
                    int eventMaxValueIndex = geoEventCursor.Fields.FindField(geoEventMaxAgeValue);

                    //Control list
                    Dictionary<string, int> controlList = new Dictionary<string, int>();

                    foreach (KeyValuePair<string, Tuple<string, string>> minMaxItems in minMaxesCollection)
                    {
                        if (minMaxItems.Value.Item1 != string.Empty && minMaxItems.Value.Item2 != string.Empty)
                        {
                            //Get list of actual values
                            List<string> existingEvents = GSC_ProjectEditor.Tables.GetFieldValues(tGeoEvent, geoEventName, null);

                            //Calculate new name
                            string eventName = "Min (" + minMaxItems.Value.Item1.ToString() + "); Max (" + minMaxItems.Value.Item2.ToString() + ")";

                            if (!existingEvents.Contains(eventName))
                            {
                                #region NEW ROW
                                //Create the new row
                                IRowBuffer newEventBuffer = geoEventTableObject.CreateRowBuffer();

                                //Calculate ids
                                int eventID = geoEventTableObject.RowCount(new QueryFilter());

                                //fill buffer with values
                                Double outResult;
                                newEventBuffer.set_Value(eventIDIndex, eventID);
                                newEventBuffer.set_Value(eventNameIndex, eventName);

                                if (double.TryParse(minMaxItems.Value.Item1, out outResult))
                                {
                                    newEventBuffer.set_Value(eventMinValueIndex, minMaxItems.Value.Item1);
                                }

                                if (double.TryParse(minMaxItems.Value.Item2, out outResult))
                                {
                                    newEventBuffer.set_Value(eventMaxValueIndex, minMaxItems.Value.Item2);
                                }

                                geoEventCursor.InsertRow(newEventBuffer);
                                #endregion

                                existingEvents.Add(eventName); //Will prevent from adding same rows
                                controlList[minMaxItems.Key] = eventID; //Will be used to update geoline with the new id 
                            }

                        }
                    }

                    //Release
                    GSC_ProjectEditor.ObjectManagement.ReleaseObject(geoEventCursor);

                    //Update Geoline feature if needed
                    if (controlList.Count != 0)
                    {
                        //Iterate through new values
                        foreach (KeyValuePair<string, int> geolineEvents in controlList)
                        {
                            string geolineIDQuery = GSC_ProjectEditor.Constants.DatabaseFields.ObjectID + " = " + geolineEvents.Key;
                            GSC_ProjectEditor.Tables.UpdateFieldValue(geolineFC, geolineGeoEvent, geolineIDQuery, geolineEvents.Value);
                        }

                    }
                }

            }
        }

        /// <summary>
        /// Will add some rows inside GEO_EVENT table based on old legend age fields.
        /// If and only those field are present in the old workspace legend table.
        /// </summary>
        /// <param name="tableToUpdate">List of tables to update with old names</param>
        /// <param name="oldWorkspace">Old workspace</param>
        /// <param name="newWorkspace">New workspace</param>
        private void UpdateGeoEventFromLegend(Dictionary<string, List<string>> tableToUpdate, IWorkspace oldWorkspace, IWorkspace newWorkspace)
        {
            //Get legend generator table from old workspace from legacy list
            string oldLegend = String.Empty;
            foreach (string oldFieldNames in tableToUpdate[legendGenTable])
            {
                if (GSC_ProjectEditor.Workspace.GetNameExistsFromWorkspace(oldWorkspace, esriDatasetType.esriDTTable, oldFieldNames))
                {
                    oldLegend = oldFieldNames;
                }
            }

            //Verify if old fields exists
            ITable oldLegendTable = GSC_ProjectEditor.Tables.OpenTableFromWorkspace(oldWorkspace, oldLegend);
            List<string> oldLegendTableFields = GSC_ProjectEditor.Tables.GetFieldList(oldLegendTable, false);
            if (oldLegendTableFields.Contains(legendGeneratorEon) && oldLegendTableFields.Contains(legendGeneratorEra) && oldLegendTableFields.Contains(legendGeneratorPeriod))
            {
                //Build a list of all previous age names
                string geolineQuery = legendGeneratorSymbolType + " = '" + lineSymbolType + "'";
                Dictionary<string, string> oldTimescalesGeolines = oldTimeScaleList(oldLegendTable, geolineQuery);

                string labelQuery = legendGeneratorSymbolType + " = '" + fillSymbolType + "'";
                Dictionary<string, string> oldTimescalesLabels = oldTimeScaleList(oldLegendTable, labelQuery);

                //Get current list of timescales
                Dictionary<string, string> newTimeScaleDico = GSC_ProjectEditor.Domains.GetDomDicoFromWorkspace(newWorkspace, ageDesignatorDomain, "Description");
                Dictionary<string, string> newTimeScaleDicoLower = new Dictionary<string, string>();
                //Lower case timescale dico
                foreach (KeyValuePair<string, string> ages in newTimeScaleDico)
                {
                    newTimeScaleDicoLower[ages.Key.ToLower()] = ages.Value;
                }

                Dictionary<string, string> matchingGeolineGeoEvents = new Dictionary<string, string>();
                Dictionary<string, string> matchingLabelGeoEvents = new Dictionary<string, string>();

                //Update GEO_EVENT table if needed for geolines
                if (oldTimescalesGeolines.Count != 0)
                {
                    UpdateGeoEventTable(oldTimescalesGeolines, newTimeScaleDicoLower, out matchingGeolineGeoEvents);
                }

                //Update GEO_EVENT table if needed for labels
                if (oldTimescalesLabels.Count != 0)
                {
                    UpdateGeoEventTable(oldTimescalesLabels, newTimeScaleDicoLower, out matchingLabelGeoEvents);
                }

                //Update Geoline feature if needed
                if (matchingGeolineGeoEvents.Count != 0)
                {
                    //Iterate through new values
                    foreach (KeyValuePair<string, string> geolineEvents in matchingGeolineGeoEvents)
                    {
                        string geolineIDQuery = geoline_ID + " = '" + geolineEvents.Key + "'";
                        GSC_ProjectEditor.Tables.UpdateFieldValue(geolineFC, geolineGeoEvent, geolineIDQuery, geolineEvents.Value);
                    }

                }

                //Update label feature if needed
                if (matchingLabelGeoEvents.Count != 0)
                {
                    //Iterate through new values
                    foreach (KeyValuePair<string, string> labelEvents in matchingLabelGeoEvents)
                    {
                        string labelIDQuery = labelID + " = '" + labelEvents.Key + "'";
                        GSC_ProjectEditor.Tables.UpdateFieldValue(labelFC, labelGeoEvent, labelIDQuery, labelEvents.Value);
                    }
                }
            }

        }

        /// <summary>
        /// From a given dictionary of legend item ids and their related timescale, will update
        /// geo_event table if the new ids doesn't already exists inside it. The new geoEvent name
        /// will be equal to it's timescale new translated code.
        /// </summary>
        /// <param name="oldTimescales"></param>
        private void UpdateGeoEventTable(Dictionary<string, string> oldTimescales, Dictionary<string, string> newTimeScaleDico, out Dictionary<string, string> matchingItemGeoEvents)
        {
            #region Variables
            matchingItemGeoEvents = new Dictionary<string, string>();
            Dictionary<string, int> controlList = new Dictionary<string, int>();

            ITable eventTableObject = GSC_ProjectEditor.Tables.OpenTable(tGeoEvent);
            ICursor geoEventCursor = GSC_ProjectEditor.Tables.GetTableCursor("Insert", null, tGeoEvent);
            int eventIDIndex = geoEventCursor.Fields.FindField(geoEventID);
            int eventNameIndex = geoEventCursor.Fields.FindField(geoEventName);
            int eventMinIndex = geoEventCursor.Fields.FindField(geoEventMinAge);
            int eventMaxIndex = geoEventCursor.Fields.FindField(geoEventMaxAge);
            int eventMinValueIndex = geoEventCursor.Fields.FindField(geoEventMinAgeValue);
            int eventMaxValueIndex = geoEventCursor.Fields.FindField(geoEventMaxAgeValue);
            #endregion

            #region Iterate through dictionary
            foreach (KeyValuePair<string, string> timescales in oldTimescales)
            {
                //Special rule for phanerozoic timescale
                if (timescales.Value != GSC_ProjectEditor.Constants.DatabaseDomainsValues.eonPhanerozoic)
                {
                    //Get a translation
                    string newTime = TimeScaleTranslator(timescales.Value, newTimeScaleDico);

                    //Conditional to have a translation
                    if (newTime != string.Empty)
                    {
                        //If not already in table, add
                        string geoEventNameQuery = geoEventName + " = '" + timescales.Value + "'";
                        List<string> existingEvents = GSC_ProjectEditor.Tables.GetFieldValues(tGeoEvent, geoEventID, geoEventNameQuery);

                        if (existingEvents.Count == 0 && !controlList.ContainsKey(timescales.Value))
                        {
                            #region NEW ROW
                            //Create the new row
                            IRowBuffer newEventBuffer = eventTableObject.CreateRowBuffer();

                            //Calculate ids
                            int eventID = eventTableObject.RowCount(new QueryFilter());

                            //fill buffer with values
                            newEventBuffer.set_Value(eventIDIndex, eventID);
                            newEventBuffer.set_Value(eventNameIndex, timescales.Value);
                            newEventBuffer.set_Value(eventMinIndex, newTime);
                            newEventBuffer.set_Value(eventMaxIndex, newTime);

                            geoEventCursor.InsertRow(newEventBuffer);
                            #endregion
                            controlList[timescales.Value] = eventID;
                            matchingItemGeoEvents[timescales.Key] = eventID.ToString();
                        }
                        else if (existingEvents.Count != 0)
                        {

                            matchingItemGeoEvents[timescales.Key] = existingEvents[0];

                        }
                        else if (controlList.ContainsKey(timescales.Value))
                        {
                            matchingItemGeoEvents[timescales.Key] = controlList[timescales.Value].ToString();
                        }
                    }
                }
                else
                {
                    //Set min and max age instead of timescale
                    #region NEW ROW
                    //Create the new row
                    IRowBuffer newEventBuffer = eventTableObject.CreateRowBuffer();

                    //Calculate ids
                    int eventID = eventTableObject.RowCount(new QueryFilter());

                    //fill buffer with values
                    newEventBuffer.set_Value(eventIDIndex, eventID);
                    newEventBuffer.set_Value(eventNameIndex, timescales.Value);
                    newEventBuffer.set_Value(eventMaxValueIndex, 541);
                    newEventBuffer.set_Value(eventMinValueIndex, 0.0);

                    geoEventCursor.InsertRow(newEventBuffer);

                    matchingItemGeoEvents[timescales.Key] = eventID.ToString();
                    #endregion
                }
            }
            #endregion

            //Release
            GSC_ProjectEditor.ObjectManagement.ReleaseObject(geoEventCursor);
        }

        /// <summary>
        /// From a preset of rules, will translate an old time scale value to the domain code of the new ones.
        /// </summary>
        /// <param name="oldTime">A given old time value</param>
        /// <param name="newTimeScaleDico"> A dictionnary containing a timescale description in lower case and a value as a domain code.</param>
        /// <returns></returns>
        private string TimeScaleTranslator(string oldTime, Dictionary<string, string> newTimeScaleDico)
        {
            //Set to lower case
            oldTime = oldTime.ToLower();
            string newTime = string.Empty;

            //Get isntant match-up
            if (newTimeScaleDico.ContainsKey(oldTime))
            {
                newTime = newTimeScaleDico[oldTime];
            }
            else if (newTimeScaleDico.ContainsKey(oldTime.Replace("-", "")))
            {
                newTime = newTimeScaleDico[oldTime.Replace("-", "")];
            }
            else if (newTimeScaleDico.ContainsKey(oldTime + "e"))
            {
                newTime = newTimeScaleDico[oldTime + "e"];
            }

            return newTime;
        }

        /// <summary>
        /// From a given table and SQL query, will return a dictionnary of IDs as key and eron/era or period old timescale.
        /// Periods have precendence on era and era on eon.
        /// </summary>
        /// <param name="oldLegendTable">The old table to retrieve timescales from</param>
        /// <param name="query">The query to differentiate feature class time to build dictionary</param>
        /// <returns></returns>
        private Dictionary<string, string> oldTimeScaleList(ITable oldLegendTable, string query)
        {
            //Variables
            Dictionary<string, string> timeScalesDico = new Dictionary<string, string>();

            //Iterate through table
            IDataset oldLegendDataset = oldLegendTable as IDataset;
            ICursor tableCursor = GSC_ProjectEditor.Tables.GetTableCursorFromWorkspace(oldLegendDataset.Workspace, "Search", query, oldLegendDataset.Name);
            IRow tableRow = tableCursor.NextRow();
            int eonIndex = tableCursor.FindField(legendGeneratorEon);
            int eraIndex = tableCursor.FindField(legendGeneratorEra);
            int periodIndex = tableCursor.FindField(legendGeneratorPeriod);
            int idIndex = tableCursor.FindField(legendGeneratorID);
            while (tableRow != null)
            {
                string currentID = tableRow.get_Value(idIndex).ToString();
                string currentPeriod = tableRow.get_Value(periodIndex).ToString();
                string currentEra = tableRow.get_Value(eraIndex).ToString();
                string currnetEon = tableRow.get_Value(eonIndex).ToString();

                if (currentPeriod != string.Empty)
                {
                    timeScalesDico[currentID] = currentPeriod;
                }
                else if (currentEra != string.Empty)
                {
                    timeScalesDico[currentID] = currentEra;
                }
                else if (currnetEon != string.Empty)
                {
                    timeScalesDico[currentID] = currnetEon;
                }

                tableRow = tableCursor.NextRow();
            }
            GSC_ProjectEditor.ObjectManagement.ReleaseObject(tableCursor);

            return timeScalesDico;
        }

        /// <summary>
        /// Since this tables already contains values, but could use udpate, the code will parse
        /// existing ids and won't append them if they match new table
        /// </summary>
        /// <param name="newWorkspace">The new workspace</param>
        /// <param name="oldWorkspace">The old workspace to retrieve table from</param>
        /// <param name="tableToUpdate">The table list that needs to be updated</param>
        /// <param name="tableToUpdate_2">The new output list of tables that needs an append missing organisation table</param>
        private void UpdateOrganisationNoDuplicate(IWorkspace newWorkspace, IWorkspace oldWorkspace, Dictionary<string, List<string>> tableToUpdate, out Dictionary<string, List<string>> tableToUpdate_2)
        {
            //Init output
            tableToUpdate_2 = tableToUpdate;

            try
            {
                //Get organisation table from old workspace from legacy list
                string oldOrganisations = String.Empty;

                foreach (string oldFieldNames in tableToUpdate_2[organisationTable])
                {
                    if (GSC_ProjectEditor.Workspace.GetNameExistsFromWorkspace(oldWorkspace, esriDatasetType.esriDTTable, oldFieldNames))
                    {
                        oldOrganisations = oldFieldNames;
                    }
                }

                //Get a list of new ids
                List<string> organisationIDs = GSC_ProjectEditor.Tables.GetFieldValuesFromWorkspace(newWorkspace, organisationTable, orgID, null);

                //Get a search cursor with old organisation table
                ITable oldOrganisationTable = GSC_ProjectEditor.Tables.OpenTableFromWorkspace(oldWorkspace, oldOrganisations);
                int oldOrgIDIndex = oldOrganisationTable.FindField(orgID);

                //Get new table
                ITable newTable = GSC_ProjectEditor.Tables.OpenTableFromWorkspace(newWorkspace, organisationTable);

                //Iterate through old table
                ICursor oldCursor = GSC_ProjectEditor.Tables.GetTableCursorFromWorkspace(oldWorkspace, "Search", null, oldOrganisations);
                IRow currentRow = oldCursor.NextRow();

                //Get an insert cursor and iterate through organisation
                ICursor insertCursor = GSC_ProjectEditor.Tables.GetTableCursorFromWorkspace(newWorkspace, "Insert", null, organisationTable);

                while (currentRow != null)
                {
                    //Detect if current row org ID is empty and alias
                    if (!organisationIDs.Contains(currentRow.get_Value(oldOrgIDIndex)))
                    {
                        //Create a row buffer object (a template of all fields of table)
                        IRowBuffer inRowBuffer = newTable.CreateRowBuffer();

                        //Current project id that will be sent to the alias
                        string newCode = currentRow.get_Value(oldOrgIDIndex).ToString();

                        //Iterate through all field of source
                        for (int i = 1; i < inRowBuffer.Fields.FieldCount; i++) //Skip OID
                        {
                            //Get current old field name
                            string currentFieldName = inRowBuffer.Fields.get_Field(i).Name;

                            //Get old value
                            int oldFieldIndex = currentRow.Fields.FindField(currentFieldName);//Get new field index (could be different from old to new database)
                            object currentValue = currentRow.get_Value(oldFieldIndex);


                            inRowBuffer.set_Value(i, currentValue);

                        }

                        //Update
                        insertCursor.InsertRow(inRowBuffer);
                        insertCursor.Flush();

                    }

                    currentRow = oldCursor.NextRow();
                }

                //Release the cursor or else some lock could happen.
                System.Runtime.InteropServices.Marshal.ReleaseComObject(insertCursor);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(oldCursor);

                //Remove person table of list of table to append
                tableToUpdate_2.Remove(organisationTable);


            }
            catch (Exception UpdateOrganisationNoDuplicateException)
            {
                //Remove person table of list of table to append
                tableToUpdate_2.Remove(organisationTable);

                MessageBox.Show(UpdateOrganisationNoDuplicateException.StackTrace + "; " + UpdateOrganisationNoDuplicateException.Message);
            }
        }

        /// <summary>
        /// Will check if symbole theme field exists in old database, if yes, it'll update it with proper values from domain
        /// </summary>
        /// <param name="oldWorkspace"></param>
        /// <param name="newWorkspace"></param>
        private void UpdateLegendGeneratorSymbolTheme(Dictionary<string, List<string>> tableToUpdate, IWorkspace oldWorkspace, IWorkspace newWorkspace)
        {
            //Get organisation table from old workspace from legacy list
            string oldLegend = String.Empty;

            foreach (string oldFieldNames in tableToUpdate[legendGenTable])
            {
                if (GSC_ProjectEditor.Workspace.GetNameExistsFromWorkspace(oldWorkspace, esriDatasetType.esriDTTable, oldFieldNames))
                {
                    oldLegend = oldFieldNames;
                }
            }

            //Detect if new field exist inside old table
            ITable oldLegendTable = GSC_ProjectEditor.Tables.OpenTableFromWorkspace(oldWorkspace, oldLegend);

            if (oldLegendTable.FindField(legendGeneratorSymbolTheme) == -1)
            {
                ITable newLegend = GSC_ProjectEditor.Tables.OpenTable(legendGenTable);

                //Get some field indexes
                int idFieldIndex = newLegend.FindField(legendGeneratorID);
                int symTypeFieldIndex = newLegend.FindField(legendGeneratorSymbolType);
                int symThemeFieldIndex = newLegend.FindField(legendGeneratorSymbolTheme);

                //Get update cursor
                ICursor legendCursor = GSC_ProjectEditor.Tables.GetTableCursor("Update", null, legendGenTable);
                IRow legendRows = legendCursor.NextRow();
                while (legendRows != null)
                {
                    //Curent symbol type
                    string currentSymbolType = legendRows.get_Value(symTypeFieldIndex).ToString();

                    if (currentSymbolType.Contains(fillSymbolType))
                    {
                        legendRows.set_Value(symThemeFieldIndex, symbolThemeMapUnits);
                    }
                    else if (currentSymbolType.Contains(lineSymbolType))
                    {
                        legendRows.set_Value(symThemeFieldIndex, symbolThemeGeoline);
                    }
                    else if (currentSymbolType.Contains(pointSymbolType))
                    {
                        legendRows.set_Value(symThemeFieldIndex, symbolThemeGeopoint);
                    }
                    else if (currentSymbolType.Contains(headerSymbolType.ElementAt(0).ToString()))
                    {
                        legendRows.set_Value(symThemeFieldIndex, symbolThemeHeaders);
                    }
                    else
                    {
                        //Check with IDs instead
                        string currentSymbolID = legendRows.get_Value(idFieldIndex).ToString();
                        Guid outHeaderGuid;
                        if (currentSymbolID.Length == 12)
                        {
                            legendRows.set_Value(symThemeFieldIndex, symbolThemeGeoline);
                        }
                        else if (currentSymbolID.Length == 13)
                        {
                            legendRows.set_Value(symThemeFieldIndex, symbolThemeGeopoint);
                        }
                        else if (Guid.TryParse(currentSymbolID, out outHeaderGuid))
                        {
                            legendRows.set_Value(symThemeFieldIndex, symbolThemeHeaders);
                        }
                    }
                    legendCursor.UpdateRow(legendRows);
                    legendRows = legendCursor.NextRow();
                }
                GSC_ProjectEditor.ObjectManagement.ReleaseObject(legendCursor);

            }
        }

        /// <summary>
        /// Will update the abbreviation field with id value if empty
        /// </summary>
        /// <param name="oldWorkspace"></param>
        /// <param name="newWorkspace"></param>
        private void UpdateOrganisationAbbreviation(IWorkspace newWorkspace)
        {
            //Get an update cursor
            ICursor orgCursor = GSC_ProjectEditor.Tables.GetTableCursorFromWorkspace(newWorkspace, "Update", null, organisationTable);
            if (orgCursor != null)
            {
                IRow currentOrgRow = orgCursor.NextRow();
                int orgAbbreviationFieldIndex = orgCursor.Fields.FindField(orgAbbrev);
                int orgIDFieldIndex = orgCursor.Fields.FindField(orgID);

                while (currentOrgRow != null)
                {
                    //Get abbre value
                    string currentAbbreviation = currentOrgRow.get_Value(orgAbbreviationFieldIndex).ToString();

                    if (currentAbbreviation == string.Empty)
                    {
                        //Update with ID
                        currentOrgRow.set_Value(orgAbbreviationFieldIndex, currentOrgRow.get_Value(orgIDFieldIndex).ToString());
                        orgCursor.UpdateRow(currentOrgRow);
                    }
                    currentOrgRow = orgCursor.NextRow();
                }

                System.Runtime.InteropServices.Marshal.ReleaseComObject(orgCursor);
            }

        }

        /// <summary>
        /// Will update m_activity table with new project id as integer, if needed
        /// </summary>
        /// <param name="newWorkspace">The new workspace</param>
        /// <param name="oldWorkspace">The old workspace</param>
        /// <param name="tableToUpdate">The table list to update</param>
        /// <param name="tableToUpdate_2">The update table list to update</param>
        /// <param name="dicoMactivityProjectID">The dictionary of translation of old project ID string codes to integer codes</param>
        private void UpdateMactivityProjectID(IWorkspace newWorkspace, IWorkspace oldWorkspace, Dictionary<string, List<string>> tableToUpdateFromM, out Dictionary<string, List<string>> newTableToUpdateFromM, Dictionary<string, int> dicoMactivityProjectID)
        {
            //Init output
            newTableToUpdateFromM = tableToUpdateFromM;

            try
            {

                #region Update Empty alias and convert IDs

                //Get person table name from list
                string oldActivityTableName = String.Empty;

                foreach (string oldFieldNames in tableToUpdateFromM[activityMTable])
                {
                    if (GSC_ProjectEditor.Workspace.GetNameExistsFromWorkspace(oldWorkspace, esriDatasetType.esriDTTable, oldFieldNames))
                    {
                        oldActivityTableName = oldFieldNames;
                    }
                }

                //Find if personID field is an integer and if alias field exists
                ITable oldActivityTable = GSC_ProjectEditor.Tables.OpenTableFromWorkspace(oldWorkspace, oldActivityTableName);
                int oldProjectIDIndex = oldActivityTable.FindField(mActivityProjectID);
                IField oldProjectIDField = oldActivityTable.Fields.get_Field(oldProjectIDIndex);

                if (oldProjectIDField.Type == esriFieldType.esriFieldTypeString) //If field, needs to be converted to int
                {
                    //Get new table
                    ITable newTable = GSC_ProjectEditor.Tables.OpenTable(activityMTable);

                    //Iterate through old table
                    ICursor oldCursor = GSC_ProjectEditor.Tables.GetTableCursorFromWorkspace(oldWorkspace, "Search", null, oldActivityTableName);

                    //Get an insert cursor and iterate through participant
                    ICursor insertCursor = GSC_ProjectEditor.Tables.GetTableCursorFromWorkspace(newWorkspace, "Insert", null, activityMTable);

                    IRow currentRow = oldCursor.NextRow();
                    while (currentRow != null)
                    {
                        //Detect if current row person ID is empty and alias
                        if (currentRow.get_Value(oldProjectIDIndex) != null)
                        {
                            //Create a row buffer object (a template of all fields of table)
                            IRowBuffer inRowBuffer = newTable.CreateRowBuffer();

                            //Current person id that will be sent to the alias
                            string oldProjectID = currentRow.get_Value(oldProjectIDIndex).ToString();

                            //Iterate through all field of source
                            for (int i = 1; i < inRowBuffer.Fields.FieldCount; i++) //Skip OID
                            {
                                //Get current old field name
                                string currentFieldName = inRowBuffer.Fields.get_Field(i).Name;

                                //Get index of current field name inside the new workspace
                                object currentValue = DBNull.Value;
                                try
                                {
                                    int currentFieldInOldTableIndex = currentRow.Fields.FindField(currentFieldName);

                                    //Get value of the current field, in the old workspace
                                    currentValue = currentRow.get_Value(currentFieldInOldTableIndex);

                                    //Find personID or alias field and process their values, else append normally
                                    if (currentFieldName == mActivityProjectID)
                                    {
                                        //Retrieve new value from dictionary, based on old value that was just like the alias
                                        currentValue = dicoMactivityProjectID[currentValue.ToString()];

                                    }

                                }
                                catch (Exception)
                                {
                                    currentValue = DBNull.Value;
                                }

                                inRowBuffer.set_Value(i, currentValue);
                            }

                            //Update
                            insertCursor.InsertRow(inRowBuffer);


                        }

                        currentRow = oldCursor.NextRow();
                    }
                    insertCursor.Flush();
                    //Release the cursor or else some lock could happen.
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(insertCursor);
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(oldCursor);

                    //Remove person table of list of table to append
                    newTableToUpdateFromM.Remove(activityMTable);

                }
                else
                {
                    //Do a normal append.
                }

                #endregion

            }
            catch (Exception updatePersonIDException)
            {

                MessageBox.Show(updatePersonIDException.StackTrace + "; " + updatePersonIDException.Message);
            }
        }

        /// <summary>
        /// Will find if the project code field exists and that projectID is now an integer, New in version 2.6
        /// </summary>
        /// <param name="newWorkspace"></param>
        /// <param name="oldWorkspace"></param>
        /// <param name="tableToUpdate"></param>
        /// <param name="tableToUpdate_2"></param>
        private void UpdateProjectID(IWorkspace newWorkspace, IWorkspace oldWorkspace, Dictionary<string, List<string>> tablesToUpdate, out Dictionary<string, List<string>> newTablesToUpdate, out Dictionary<string, int> dicoMactivityProjectID)
        {
            //Init output
            newTablesToUpdate = tablesToUpdate;
            dicoMactivityProjectID = new Dictionary<string, int>();

            try
            {

                #region Update Empty alias and convert IDs

                //Get person table name from list
                string oldProjectTableName = String.Empty;

                foreach (string oldFieldNames in tablesToUpdate[projectTable])
                {
                    if (GSC_ProjectEditor.Workspace.GetNameExistsFromWorkspace(oldWorkspace, esriDatasetType.esriDTTable, oldFieldNames))
                    {
                        oldProjectTableName = oldFieldNames;
                    }
                }

                //Find if projectID field is an integer and if alias field exists
                ITable oldProjectTable = GSC_ProjectEditor.Tables.OpenTableFromWorkspace(oldWorkspace, oldProjectTableName);
                int oldProjectIDIndex = oldProjectTable.FindField(projectID);
                IField oldProjectIDField = oldProjectTable.Fields.get_Field(oldProjectIDIndex);

                if (oldProjectIDField.Type == esriFieldType.esriFieldTypeString) //If field, needs to be converted to int
                {
                    //Get new table
                    ITable newTable = GSC_ProjectEditor.Tables.OpenTable(projectTable);

                    //Iterate through old table
                    ICursor oldCursor = GSC_ProjectEditor.Tables.GetTableCursorFromWorkspace(oldWorkspace, "Search", null, oldProjectTableName);

                    //Get an insert cursor and iterate through participant
                    ICursor insertCursor = GSC_ProjectEditor.Tables.GetTableCursorFromWorkspace(newWorkspace, "Insert", null, projectTable);

                    //Start a dictionnary of conversion for participant table
                    dicoMactivityProjectID = new Dictionary<string, int>(); //{OldID (string alias): newID (int)}

                    IRow currentRow = oldCursor.NextRow();
                    while (currentRow != null)
                    {
                        //Detect if current row project ID is empty and alias
                        if (currentRow.get_Value(oldProjectIDIndex) != null)
                        {
                            //Create a row buffer object (a template of all fields of table)
                            IRowBuffer inRowBuffer = newTable.CreateRowBuffer();

                            //Current project id that will be sent to the alias
                            string newCode = currentRow.get_Value(oldProjectIDIndex).ToString();

                            //Iterate through all field of source
                            for (int i = 1; i < inRowBuffer.Fields.FieldCount; i++) //Skip OID
                            {
                                //Get current old field name
                                string currentFieldName = inRowBuffer.Fields.get_Field(i).Name;

                                //Get index of current field name inside the new workspace
                                object currentValue = DBNull.Value;
                                try
                                {
                                    int currentFieldInOldTableIndex = currentRow.Fields.FindField(currentFieldName);

                                    //Get value of the current field, in the old workspace
                                    currentValue = currentRow.get_Value(currentFieldInOldTableIndex);

                                    //Find projectID or code field and process their values, else append normally
                                    if (currentFieldName == projectID)
                                    {
                                        //Calculate new id
                                        currentValue = GSC_ProjectEditor.IDs.CalculateProjectID(oldProjectTableName, null);

                                        //Add ID and alias to dico
                                        dicoMactivityProjectID[newCode] = Convert.ToInt16(currentValue);
                                    }

                                }
                                catch (Exception)
                                {
                                    //The field didn't exist in the old table
                                    if (currentFieldName == projectCode)
                                    {
                                        currentValue = newCode;
                                    }
                                }

                                inRowBuffer.set_Value(i, currentValue);

                            }

                            //Update
                            insertCursor.InsertRow(inRowBuffer);
                            insertCursor.Flush();

                        }

                        currentRow = oldCursor.NextRow();
                    }

                    //Release the cursor or else some lock could happen.
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(insertCursor);
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(oldCursor);

                    //Remove person table of list of table to append
                    newTablesToUpdate.Remove(projectTable);
                }
                else
                {
                    //Do a normal append.
                }

                #endregion

            }
            catch (Exception updateProjectIDException)
            {

                MessageBox.Show(updateProjectIDException.StackTrace + "; " + updateProjectIDException.Message);
            }
        }

        /// <summary>
        /// Will find if the alias field exist and that personID is now an integer. New in version 2.6
        /// </summary>
        /// <param name="newWorkspace"></param>
        /// <param name="oldWorkspace"></param>
        public void UpdatePersonID(IWorkspace newWorkspace, IWorkspace oldWorkspace, Dictionary<string, List<string>> tablesToUpdate, out Dictionary<string, List<string>> newTablesToUpdate, out Dictionary<string, int> dicoParticipantPersonID)
        {
            //Init output
            newTablesToUpdate = tablesToUpdate;
            dicoParticipantPersonID = new Dictionary<string, int>();

            try
            {

                #region Update Empty alias and convert IDs

                //Get person table name from list
                string oldPersonTableName = String.Empty;

                foreach (string oldFieldNames in tablesToUpdate[personTable])
                {
                    if (GSC_ProjectEditor.Workspace.GetNameExistsFromWorkspace(oldWorkspace, esriDatasetType.esriDTTable, oldFieldNames))
                    {
                        oldPersonTableName = oldFieldNames;
                    }
                }

                //Find if personID field is an integer and if alias field exists
                ITable oldPersonTable = GSC_ProjectEditor.Tables.OpenTableFromWorkspace(oldWorkspace, oldPersonTableName);
                int oldPersonIDIndex = oldPersonTable.FindField(personID);
                IField oldPersonIDField = oldPersonTable.Fields.get_Field(oldPersonIDIndex);

                if (oldPersonIDField.Type == esriFieldType.esriFieldTypeString) //If field, needs to be converted to int
                {
                    //Get new table
                    ITable newTable = GSC_ProjectEditor.Tables.OpenTable(personTable);

                    //Iterate through old table
                    ICursor oldCursor = GSC_ProjectEditor.Tables.GetTableCursorFromWorkspace(oldWorkspace, "Search", null, oldPersonTableName);

                    //Get an insert cursor and iterate through participant
                    ICursor insertCursor = GSC_ProjectEditor.Tables.GetTableCursorFromWorkspace(newWorkspace, "Insert", null, personTable);

                    //Start a dictionnary of conversion for participant table
                    dicoParticipantPersonID = new Dictionary<string, int>(); //{OldID (string alias): newID (int)}

                    IRow currentRow = oldCursor.NextRow();
                    while (currentRow != null)
                    {
                        //Detect if current row person ID is empty and alias
                        if (currentRow.get_Value(oldPersonIDIndex) != null)
                        {
                            //Create a row buffer object (a template of all fields of table)
                            IRowBuffer inRowBuffer = newTable.CreateRowBuffer();

                            //Current person id that will be sent to the alias
                            string newAlias = currentRow.get_Value(oldPersonIDIndex).ToString();

                            //Iterate through all field of source
                            for (int i = 1; i < inRowBuffer.Fields.FieldCount; i++) //Skip OID
                            {
                                //Get current old field name
                                string currentFieldName = inRowBuffer.Fields.get_Field(i).Name;

                                //Get index of current field name inside the new workspace
                                object currentValue = DBNull.Value;
                                try
                                {
                                    int currentFieldInOldTableIndex = currentRow.Fields.FindField(currentFieldName);

                                    //Get value of the current field, in the old workspace
                                    currentValue = currentRow.get_Value(currentFieldInOldTableIndex);

                                    //Find personID or alias field and process their values, else append normally
                                    if (currentFieldName == personID)
                                    {
                                        //Calculate new id
                                        currentValue = GSC_ProjectEditor.IDs.CalculatePersonID(oldPersonTableName, null);

                                        //Add ID and alias to dico
                                        dicoParticipantPersonID[newAlias] = Convert.ToInt16(currentValue);

                                    }

                                }
                                catch (Exception)
                                {
                                    //The field didn't exist in the old table
                                    if (currentFieldName == personAlias)
                                    {
                                        currentValue = newAlias;
                                    }
                                }

                                inRowBuffer.set_Value(i, currentValue);
                            }

                            //Update
                            insertCursor.InsertRow(inRowBuffer);
                            insertCursor.Flush();

                        }

                        currentRow = oldCursor.NextRow();
                    }

                    //Release the cursor or else some lock could happen.
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(insertCursor);
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(oldCursor);

                    //Remove person table of list of table to append
                    newTablesToUpdate.Remove(personTable);

                }
                else
                {
                    //Do a normal append.
                }

                #endregion

            }
            catch (Exception updatePersonIDException)
            {

                MessageBox.Show(updatePersonIDException.StackTrace + "; " + updatePersonIDException.Message);
            }
        }

        /// <summary>
        /// Will update all features within new project db
        /// </summary>
        /// <param name="inputOldGDB">Old working pgdb</param>
        /// <param name="inputNewGDB">Current working pgdb</param>
        public void UpdateFeaturesPreProcess(IWorkspace oldWorkspace, IWorkspace newWorkspace)
        {
            try
            {
                #region Process database root
                //Get a list of all tables within old workspace
                List<string> oldFeatureList = GSC_ProjectEditor.FeatureClass.GetFeatureClassList(oldWorkspace, null);

                //Build list of tables to update Dictionary{NewestVersionTableName:[oldValue1, oldValue2, ...]} for db root
                Dictionary<string, List<string>> featureToUpdate = new Dictionary<string, List<string>>()
                {
                    {studyAreaFC, new List<string>{studyAreaFC}},
                    {cartoFC, new List<string>{cartoFC}},
                    {cgmFC, new List<string>{cgmFC_01, cgmFC}}
                };

                UpdateFeatures(featureToUpdate, oldFeatureList, oldWorkspace, newWorkspace);

                #endregion

                #region Process DB feature dataset field obs
                IFeatureDataset fdField = GSC_ProjectEditor.FeatureDataset.OpenFeatureDataSet(oldWorkspace, GSC_ProjectEditor.Constants.Database.FDField);
                List<string> fieldObsOldFeatureList = GSC_ProjectEditor.FeatureClass.GetFeatureClassList(oldWorkspace, fdField);

                //Build list of tables to update Dictionary{NewestVersionTableName:[oldValue1, oldValue2, ...]}
                Dictionary<string, List<string>> fieldObsToUpdate = new Dictionary<string, List<string>>()
                {
                    {stationFC, new List<string>{stationFC}},
                    {lineworkFC, new List<string>{lineworkFC}},
                    {traversesFC, new List<string>{traversesFC}}
                };

                UpdateFeatures(fieldObsToUpdate, fieldObsOldFeatureList, oldWorkspace, newWorkspace);

                #endregion

                #region Process for DB feature dataset GEO
                IFeatureDataset fdGEO = GSC_ProjectEditor.FeatureDataset.OpenFeatureDataSet(oldWorkspace, GSC_ProjectEditor.Constants.Database.FDGeo);
                List<string> geoOldFeatureList = GSC_ProjectEditor.FeatureClass.GetFeatureClassList(oldWorkspace, fdGEO);

                //Build list of tables to update Dictionary{NewestVersionTableName:[oldValue1, oldValue2, ...]}
                Dictionary<string, List<string>> geoToUpdate = new Dictionary<string, List<string>>()
                {
                    {geolineFC, new List<string>{geolineFC}},
                    {geopointFC, new List<string>{geopointFC}},
                    {geopolyFC, new List<string>{geopolyFC}},
                    {labelFC, new List<string>{labelFC, labelFC_01}}
                };

                UpdateFeatures(geoToUpdate, geoOldFeatureList, oldWorkspace, newWorkspace);

                #endregion
            }
            catch (Exception updateFeaturesPreProcessExcep)
            {
                MessageBox.Show(updateFeaturesPreProcessExcep.StackTrace.ToString());
            }


        }

        /// <summary>
        /// Will update all tables within new project db
        /// </summary>
        /// <param name="inputOldGDB">Old working pgdb</param>
        /// <param name="inputNewGDB">Current working pgdb</param>
        public void UpdateTables(IWorkspace oldWorkspace, IWorkspace newWorkspace, Dictionary<string, List<string>> tableToUpdate, out List<string> currentOldTableList)
        {
            //Get a list of all tables within workspace
            currentOldTableList = GSC_ProjectEditor.Tables.GetTableNameListFromWorkspace(oldWorkspace);

            //Get a list of feature or tables that needs a special field mapping
            Dictionary<string, Dictionary<string, List<string>>> fieldsToMap = FieldMapping();

            try
            {

                //Find matching tables, key is new table name, value is list a old names
                foreach (KeyValuePair<string, List<string>> oldTableNameList in tableToUpdate)
                {
                    //Iterate through all old version names
                    foreach (string oldNames in oldTableNameList.Value)
                    {
                        if (currentOldTableList.Contains(oldNames))
                        {
                            //Cast the input as table
                            ITable oldTable = GSC_ProjectEditor.Tables.OpenTableFromWorkspace(oldWorkspace, oldNames);

                            //Cast the output as a table
                            ITable newTable = GSC_ProjectEditor.Tables.OpenTableFromWorkspace(newWorkspace, oldTableNameList.Key);

                            //Append
                            if (fieldsToMap.ContainsKey(oldTableNameList.Key))
                            {
                                //Found field dico
                                Dictionary<string, List<string>> foundFieldDico = new Dictionary<string, List<string>>();

                                //Iterat through legacy field to find which one exists
                                for (int i = 0; i < oldTable.Fields.FieldCount; i++)
                                {
                                    string currentFieldName = oldTable.Fields.get_Field(i).Name;
                                    foreach (KeyValuePair<string, List<string>> newToOldPairs in fieldsToMap[oldTableNameList.Key])
                                    {
                                        //Find if old field name is within list of old field names that needs a field mapping
                                        if (newToOldPairs.Value.Contains(currentFieldName))
                                        {
                                            //Build appending field map dico
                                            foundFieldDico = new Dictionary<string, List<string>>() { { newToOldPairs.Key, new List<string>() { currentFieldName } } };

                                        }

                                    }
                                }

                                //Append
                                if (foundFieldDico.Count != 0)
                                {
                                    GSC_ProjectEditor.GeoProcessing.AppendDataWithFieldMap(oldTable, newTable, foundFieldDico);
                                }
                                else
                                {
                                    GSC_ProjectEditor.GeoProcessing.AppendData(oldTable, newTable);
                                }


                            }
                            else
                            {
                                GSC_ProjectEditor.GeoProcessing.AppendData(oldTable, newTable);
                            }

                        }
                    }

                }

            }
            catch (Exception updateTablesExcept)
            {
                MessageBox.Show(updateTablesExcept.StackTrace.ToString());
            }

        }

        /// <summary>
        /// Will update the domains from the old
        /// </summary>
        /// <param name="inputOldGDB">Old working pgdb</param>
        /// <param name="inputNewGDB">Current working pgdb</param>
        /// <param name="domainValuesToAddBeforeDelete">A dictionary that holds dom name and missing values. Those values will be temporaly added so feature can be added then removed to do some clean-up</param>
        public void UpdateDomains(IWorkspace inputOldGDB, IWorkspace inputNewGDB, Dictionary<string, List<string>> domainValuesToAddBeforeDelete)
        {
            IWorkspaceDomains2 newDomain2 = inputNewGDB as IWorkspaceDomains2;

            //Get a list of new version domains
            List<IDomain> listOfNewDomains = GSC_ProjectEditor.Domains.GetDomainList(inputNewGDB);

            //Get a list of old version domains
            List<IDomain> listOfOldDomains = GSC_ProjectEditor.Domains.GetDomainList(inputOldGDB);

            //Iterate through list and find match
            foreach (IDomain newDoms in listOfNewDomains)
            {
                int currentListIndex = listOfNewDomains.IndexOf(newDoms);
                foreach (IDomain oldDoms in listOfOldDomains)
                {
                    //Get only matchin dom names
                    if (oldDoms.Name == newDoms.Name)
                    {
                        //Get domain and associated coded values
                        ICodedValueDomain newDomCodes = newDoms as ICodedValueDomain;
                        ICodedValueDomain oldDomCodes = oldDoms as ICodedValueDomain;

                        //Get only matchin dom names if it's a PID one
                        if (newDoms.Name.Contains(GSC_ProjectEditor.Constants.DatabaseDomains.projectKeyword))
                        {
                            #region Append Project domaines

                            try
                            {
                                //Get a dico of old values
                                int[] numberOfCodes = Enumerable.Range(0, oldDomCodes.CodeCount).ToArray(); //Create an array of exact same lenght as number of values in domain.
                                Dictionary<object, string> oldValueDico = new Dictionary<object, string>();
                                foreach (int index in numberOfCodes)
                                {
                                    //Add code values description to list
                                    oldValueDico[oldDomCodes.get_Value(index)] = oldDomCodes.get_Name(index);

                                }

                                //Filter only code that are not in the new domain
                                int[] numberOfNewCodes = Enumerable.Range(0, newDomCodes.CodeCount).ToArray();
                                foreach (int index2 in numberOfNewCodes)
                                {

                                    if (oldValueDico.ContainsKey(newDomCodes.get_Value(index2).ToString()))
                                    {
                                        oldValueDico.Remove(newDomCodes.get_Value(index2).ToString());
                                    }
                                }

                                //Update domain with old code
                                if (oldValueDico.Count != 0)
                                {
                                    foreach (KeyValuePair<object, string> kv in oldValueDico)
                                    {
                                        //Special conversion for booleanTruth domain
                                        if (newDoms.Name == GSC_ProjectEditor.Constants.DatabaseDomains.BoolYesNo)
                                        {
                                            try
                                            {
                                                int newValue = Convert.ToInt16(kv.Key);
                                                newDomCodes.AddCode(newValue, kv.Value);
                                            }
                                            catch (Exception)
                                            {

                                            }

                                        }
                                        else
                                        {
                                            newDomCodes.AddCode(kv.Key, kv.Value);
                                        }

                                    }

                                }

                                //Update domain
                                newDomain2.AlterDomain(newDoms);
                            }
                            catch (Exception updateDomException)
                            {
                                MessageBox.Show(updateDomException.StackTrace);
                            }

                            #endregion
                        }

                        //Get matching domain with missing values
                        if (domainValuesToAddBeforeDelete.ContainsKey(newDoms.Name))
                        {
                            foreach (string missingValues in domainValuesToAddBeforeDelete[newDoms.Name])
                            {
                                newDomCodes.AddCode(missingValues, missingValues);
                            }

                            //Update domain
                            newDomain2.AlterDomain(newDoms);
                        }
                    }
                }

            }
        }

        /// <summary>
        /// Will update, from a dictionnary, given new workspace with old workspace feature class information.
        /// </summary>
        /// <param name="parsingDico">A dictionary that contains new feature class name as key and old names as value within a list</param>
        /// <param name="inputOldList">List of features from old workspace</param>
        /// <param name="oldWorkspace">Old workspace</param>
        /// <param name="newWorkspace">New workspace</param>
        public void UpdateFeatures(Dictionary<string, List<string>> parsingDico, List<string> inputOldList, IWorkspace oldWorkspace, IWorkspace newWorkspace)
        {
            //Get a list of feature or tables that needs a special field mapping
            Dictionary<string, Dictionary<string, List<string>>> fieldsToMap = FieldMapping();

            //Find matching tables
            foreach (KeyValuePair<string, List<string>> kvPairs in parsingDico)
            {
                //Iterate through all old version names
                foreach (string oldNames in kvPairs.Value)
                {
                    if (inputOldList.Contains(oldNames))
                    {
                        //Cast the input as table
                        IFeatureClass oldFeature = GSC_ProjectEditor.FeatureClass.OpenFeatureClassFromWorkspace(oldWorkspace, oldNames);

                        //Cast the output as a table
                        IFeatureClass newFeature = GSC_ProjectEditor.FeatureClass.OpenFeatureClassFromWorkspace(newWorkspace, kvPairs.Key);

                        //Append
                        if (fieldsToMap.ContainsKey(kvPairs.Key))
                        {
                            //Found field dico
                            Dictionary<string, List<string>> foundFieldDico = new Dictionary<string, List<string>>();

                            //Iterat through legacy field to find which one exists
                            for (int i = 0; i < oldFeature.Fields.FieldCount; i++)
                            {
                                string currentFieldName = oldFeature.Fields.get_Field(i).Name;
                                foreach (KeyValuePair<string, List<string>> oldPairs in fieldsToMap[kvPairs.Key])
                                {
                                    if (oldPairs.Value.Contains(currentFieldName))
                                    {
                                        //Build appending field map dico
                                        foundFieldDico = new Dictionary<string, List<string>>() { { oldPairs.Key, new List<string>() { currentFieldName } } };

                                    }
                                }

                            }

                            //Append
                            if (foundFieldDico.Count != 0)
                            {
                                GSC_ProjectEditor.GeoProcessing.AppendDataWithFieldMap(oldFeature, newFeature, foundFieldDico);
                            }
                            else
                            {
                                GSC_ProjectEditor.GeoProcessing.AppendData(oldFeature, newFeature);
                            }

                        }
                        else
                        {
                            GSC_ProjectEditor.GeoProcessing.AppendData(oldFeature, newFeature);
                        }

                    }
                }

            }
        }

        /// <summary>
        /// Will process any other data, not present in default project db schema, and copy them into another database beside the project database.
        /// </summary>
        /// <param name="newWorkspace">The new output workspace in which the copied data will be held.</param>
        /// <param name="oldWorkspace">The original workspace in which the different data could be found.</param>
        public void CheckForOtherData(IWorkspace newWorkspace, IWorkspace oldWorkspace)
        {
            //Variables
            List<IDataset> differenceDataList = new List<IDataset>(); //A list to contains any different data between the two workspace

            //Get list of current dataset inside the new and the old workspaces
            List<string> newDataList = GSC_ProjectEditor.Workspace.GetDatasetNameListFromWorkspace(newWorkspace);
            List<IDataset> oldDataList = GSC_ProjectEditor.Workspace.GetDatasetListFromWorkspace(oldWorkspace);

            foreach (IDataset data in oldDataList)
            {
                //Check for root difference
                if (!newDataList.Contains(data.BrowseName))
                {
                    differenceDataList.Add(data);
                }
            }

            try
            {
                //If any different data is found, process to copy those inside another database
                if (differenceDataList.Count != 0)
                {

                    //Copy all the database to the other file geodatabase.
                    foreach (IDataset diffData in differenceDataList)
                    {
                        //Copy
                        GSC_ProjectEditor.GeoProcessing.CopyDataset(diffData, newWorkspace); //NOTE THIS METHOD DOESN'T USE GP objects
                    }

                }
            }
            catch (Exception versionOnClickException)
            {

                MessageBox.Show(versionOnClickException.StackTrace + "; " + versionOnClickException.Message);
            }

        }

        /// <summary>
        /// Will update the tables names stored inside Study Area index table, if needed
        /// </summary>
        /// <param name="newWorkspace">The new workspace to calculate new table names</param>
        public void UpdateStudyAreaIndex(IWorkspace newWorkspace)
        {
            try
            {

                //Build list of tables to update Dictionary{NewestVersionTableName:[oldValue1, oldValue2, ...]}
                Dictionary<string, List<string>> tableToUpdate = new Dictionary<string, List<string>>()
                {
                    {subactivityTable, new List<string>{subactivityTable_01}},
                };

                //Find matching tables
                foreach (KeyValuePair<string, List<string>> kvPairs in tableToUpdate)
                {
                    //Iterate through all old version names
                    foreach (string oldNames in kvPairs.Value)
                    {
                        //Build a query to select proper old values 
                        string queryTableNames = studyAreaIndexTable + " = '" + oldNames + "'";

                        //Update
                        GSC_ProjectEditor.Tables.UpdateFieldValueFromWorkspace(newWorkspace, studyIndexTable, studyAreaIndexTable, queryTableNames, kvPairs.Key);
                    }

                }

                //Build list of tables to update Dictionary{NewestVersionTableName:[oldValue1, oldValue2, ...]}
                Dictionary<string, List<string>> featuresToUpdate = new Dictionary<string, List<string>>()
                {
                    {cgmFC, new List<string>{cgmFC_01}},
                };

                //Find matching tables
                foreach (KeyValuePair<string, List<string>> kvPairs in featuresToUpdate)
                {
                    //Iterate through all old version names
                    foreach (string oldNames in kvPairs.Value)
                    {
                        //Build a query to select proper old values 
                        string queryTableNames = studyAreaIndexFC + " = '" + oldNames + "'";

                        //Update
                        GSC_ProjectEditor.Tables.UpdateFieldValueFromWorkspace(newWorkspace, studyIndexTable, studyAreaIndexFC, queryTableNames, kvPairs.Key);
                    }

                }

            }
            catch (Exception updateTablesExcept)
            {
                MessageBox.Show(updateTablesExcept.StackTrace.ToString());
            }
        }

        /// <summary>
        /// Will find if all participant IDs have been entered in the participant table, else it will call a proper function to fill it in.
        /// </summary>
        /// <param name="newWorkspace">The new workspace to update participant table</param>
        public void UpdateEmptyParticipantID(IWorkspace newWorkspace)
        {
            try
            {
                //Retrieve dico for roles
                Dictionary<string, string> roleDico2 = GSC_ProjectEditor.Domains.GetDomDicoFromWorkspace(newWorkspace, roleDomain, "Code");

                #region Update Empty participant
                Form_ProjectMetadata_Roles newPartRoleForm = new Form_ProjectMetadata_Roles();

                //Get an update cursor and iterate through participant
                ICursor upCursor = GSC_ProjectEditor.Tables.GetTableCursorFromWorkspace(newWorkspace, "Update", null, participantTable);
                int participantIDIndex = upCursor.FindField(participantID);
                int participantRoleIndex = upCursor.FindField(participantRoleID);
                int participantMAIDIndex = upCursor.FindField(participantMAID);
                int participantSAIDIndex = upCursor.FindField(participantSAID);
                int participantPersonIDIndex = upCursor.FindField(participantPersonID);

                //Build a proper description for current participant -----------------------------
                IRow currentRow = upCursor.NextRow();
                bool messageBreaker = true;
                while (currentRow != null)
                {
                    //Detect if current row participant ID is empty
                    if (currentRow.get_Value(participantIDIndex) == null || currentRow.get_Value(participantIDIndex).ToString() == "" || currentRow.get_Value(participantIDIndex).ToString() == " ")
                    {
                        if (messageBreaker)
                        {
                            MessageBox.Show(Properties.Resources.Warning_ParticipantDomainCleanUp, Properties.Resources.Warning_BasicTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            messageBreaker = false;
                        }

                        string partUniqueName = "";


                        //Get a new ID and update domain with it
                        string newID = newPartRoleForm.GetNewIDAndUpdateDomainFromWorkspace(newWorkspace, currentRow.get_Value(participantPersonIDIndex).ToString(), false);

                        //Update current row with the new calculated value for participantID
                        currentRow.set_Value(participantIDIndex, newID);
                        upCursor.UpdateRow(currentRow);
                        upCursor.Flush();

                    }

                    currentRow = upCursor.NextRow();
                }

                //Release the cursor or else some lock could happen.
                System.Runtime.InteropServices.Marshal.ReleaseComObject(upCursor);
                #endregion
            }
            catch (Exception UpdateEmptyParticipantIDException)
            {

                MessageBox.Show(UpdateEmptyParticipantIDException.StackTrace + "; " + UpdateEmptyParticipantIDException.Message);
            }

        }

        /// <summary>
        /// Will update legend generator table, if needed, based on information from symbole tables for geoline and geopoint.
        /// </summary>
        public void UpdateLegendGeneratorSymbols(List<string> oldTableList, IWorkspace inputOldGDB, IWorkspace newWorkspace)
        {
            try
            {

                //Build list of tables to retrieve project information from item1 = old table name, item2 =  old Selected Code field, item3 = old id field, item4 = old leg. desc field, item5= oldsymbol field, item6= new table name
                List<Tuple<string, string, string, string, string, string>> parsingList = new List<Tuple<string, string, string, string, string, string>>
                    {
                        new Tuple<string, string, string, string, string, string>(geopointSymTable_01, geopointSymTable_SelectCode, geopoint_ID, geopointLegendDescSymbol, geopointLegendFGDCSymbol, geopointSymTable),
                        new Tuple<string, string, string, string, string, string>(geolineSymTable_01, geolineSymTable_SelectCode, geoline_ID, geolineLegendDescSymbol, geolineLegendFGDCSymbol, geolineSymTable),
                        new Tuple<string, string, string, string, string, string>(geopointSymTable, geopointSymTable_SelectCode, geopoint_ID, geopointLegendDescSymbol, geopointLegendFGDCSymbol, geopointSymTable),
                        new Tuple<string, string, string, string, string, string>(geolineSymTable, geolineSymTable_SelectCode, geoline_ID, geolineLegendDescSymbol, geolineLegendFGDCSymbol, geolineSymTable)
                    
                    };

                //Find matching tables
                foreach (Tuple<string, string, string, string, string, string> items in parsingList)
                {

                    if (oldTableList.Contains(items.Item1))
                    {
                        //Check wether the wanted deprecated fields are present or not in the old table
                        ITable currentTable = GSC_ProjectEditor.Tables.OpenTableFromWorkspace(inputOldGDB, items.Item1);
                        List<string> currentFields = GSC_ProjectEditor.Tables.GetFieldList(currentTable, false);
                        if (currentFields.Contains(items.Item2))
                        {
                            //Build a query to get all lines or points used in the old project, based on old project SelectCode field type
                            int selectSymbolIndex = currentTable.Fields.FindField(items.Item2);
                            IField selectSymbolField = currentTable.Fields.get_Field(selectSymbolIndex);
                            string selectedSymbolQuery = "";

                            //Prior to version 2.3 field were in string format, at version 2.3 they were integer (2015-02-19)
                            if (selectSymbolField.Type == esriFieldType.esriFieldTypeString)
                            {
                                selectedSymbolQuery = items.Item2 + " = '" + selectedSymbol + "'";
                            }
                            else if (selectSymbolField.Type == esriFieldType.esriFieldTypeSmallInteger)
                            {
                                selectedSymbolQuery = items.Item2 + " = " + selectedSymbol;
                            }

                            else
                            {
                                MessageBox.Show("Unknown field type in symbol tables when converting SelectCode field.");
                            }

                            //Get a list of old values 
                            Tuple<string, string, string> oldFieldNames = new Tuple<string, string, string>(items.Item3, items.Item4, items.Item5);
                            Tuple<string, string, string> ToFieldNames = new Tuple<string, string, string>(legendGeneratorID, legendGeneratorName, legendGeneratorSymbolCode);
                            List<Tuple<string, string, string>> oldIDList = GSC_ProjectEditor.Tables.GetUniqueTripleFieldValuesFromWorkspace(inputOldGDB, items.Item1, oldFieldNames, selectedSymbolQuery);

                            //Get a list of actuel values inside legend generator. Code won't append a symbol that already exists
                            List<string> currentProjectIDs = GSC_ProjectEditor.Tables.GetFieldValuesFromWorkspace(newWorkspace, legendGenTable, legendGeneratorID, null);

                            //Build a new row dictionnary if detected ID isn't inside current legend generator table
                            List<Dictionary<string, object>> rowsToAdd = new List<Dictionary<string, object>>();
                            foreach (Tuple<string, string, string> idListItems in oldIDList)
                            {
                                //Get current iteration geo id
                                string itemID = idListItems.Item1;

                                //Insert a new row if it doesn't exist inside table
                                if (!currentProjectIDs.Contains(itemID))
                                {
                                    Dictionary<string, object> newRow = new Dictionary<string, object>();
                                    newRow[ToFieldNames.Item1] = idListItems.Item1 as object; //Add current geoline or geopoint id to legend generator labelID field
                                    newRow[ToFieldNames.Item2] = idListItems.Item2 as object; //Add current legend description to legend generator name field
                                    newRow[ToFieldNames.Item3] = idListItems.Item3 as object; //Add current legend FGDC symbol code to legend generator symbol field
                                    if (items.Item6 == geolineSymTable)
                                    {
                                        newRow[legendGeneratorSymbolType] = lineSymbolType;
                                    }
                                    else if (items.Item6 == geopointSymTable)
                                    {
                                        newRow[legendGeneratorSymbolType] = pointSymbolType;
                                    }
                                    rowsToAdd.Add(newRow);
                                }
                            }

                            //Update legend generator table with project symbol coming from current table
                            GSC_ProjectEditor.Tables.AddRowsWithValues(legendGenTable, rowsToAdd);
                        }
                    }
                }
            }
            catch (Exception updateLegendGeneratorExcept)
            {
                MessageBox.Show(updateLegendGeneratorExcept.StackTrace.ToString());
            }
        }

        /// <summary>
        /// Will return a dictionary containing the parsing of current table names and legacy names
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, List<string>> GetTableToUpdateList()
        {
            //Build list of tables to update Dictionary{NewestVersionTableName:[oldValue1, oldValue2, ...]}
            Dictionary<string, List<string>> tableToUpdate = new Dictionary<string, List<string>>()
                {
                    {participantTable, new List<string>{participantTable}},
                    {personTable, new List<string>{personTable}},
                    {legenDescTable, new List<string>{legenDescTable}},
                    {studyIndexTable, new List<string>{studyIndexTable}},
                    {sourceTable, new List<string>{sourceTable}},
                    {organisationTable, new List<string>{organisationTable_01, organisationTable}},
                    {legendGenTable, new List<string>{legendGenTable_01, legendGenTable}},
                    {projectTable, new List<string>{projectTable}},
                    {activityMTable, new List<string>{activityMTable}},
                    {subactivityTable, new List<string>{subactivityTable_01, subactivityTable}},
                    {legendTreeTable, new List<string>{legendTreeTable_01, legendTreeTable_02, legendTreeTable}},
                    {extentedAttrTable, new List<string>{extentedAttrTable_01, extentedAttrTable}},
                    {ganEarthmatTable, new List<string>{ganEarthmatTable}},
                    {ganMATable, new List<string>{ganMATable}},
                    {ganMetaTable, new List<string>{ganMetaTable}},
                    {ganMineTable, new List<string>{ganMineTable}},
                    {ganSampleTable, new List<string>{ganSampleTable}},
                    {ganStrucTable, new List<string>{ganStrucTable}},
                    {ganPhotoTable, new List<string>{ganPhotoTable}},
                    {tGeoEvent, new List<string>{tGeoEvent_01, tGeoEvent}},
                };

            return tableToUpdate;
        }

        /// <summary>
        /// Will return a dictionary containing the parsing of current workspace folder names and legacy names
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, List<string>> GetFolderToUpdate()
        {
            //Build list of tables to update Dictionary{NewestVersionTableName:[oldValue1, oldValue2, ...]}
            Dictionary<string, List<string>> folderToUpdate = new Dictionary<string, List<string>>()
                {
                    {templateFolder, new List<string>{templateFolder_01}},
                };

            return folderToUpdate;
        }

        /// <summary>
        /// Will update table participant with new personID if needed, like the personID changed from alias type to int.
        /// </summary>
        /// <param name="translationDico">The dictionary that contains the new values to update</param>
        public void UpdateParticipantPersonID(IWorkspace newWorkspace, IWorkspace oldWorkspace, Dictionary<string, List<string>> tableToUpdateFromPart, out Dictionary<string, List<string>> newTableToUpdateFromPart, Dictionary<string, int> dicoParticipantPersonID)
        {
            //Init output
            newTableToUpdateFromPart = tableToUpdateFromPart;

            try
            {

                #region Update Empty alias and convert IDs

                //Get person table name from list
                string oldParticipantTableName = String.Empty;

                foreach (string oldFieldNames in tableToUpdateFromPart[participantTable])
                {
                    if (GSC_ProjectEditor.Workspace.GetNameExistsFromWorkspace(oldWorkspace, esriDatasetType.esriDTTable, oldFieldNames))
                    {
                        oldParticipantTableName = oldFieldNames;
                    }
                }

                //Find if personID field is an integer and if alias field exists
                ITable oldParticipantTable = GSC_ProjectEditor.Tables.OpenTableFromWorkspace(oldWorkspace, oldParticipantTableName);
                int oldPersonIDIndex = oldParticipantTable.FindField(participantPersonID);
                IField oldPersonIDField = oldParticipantTable.Fields.get_Field(oldPersonIDIndex);

                if (oldPersonIDField.Type == esriFieldType.esriFieldTypeString) //If field, needs to be converted to int
                {
                    //Get new table
                    ITable newTable = GSC_ProjectEditor.Tables.OpenTable(participantTable);

                    //Iterate through old table
                    ICursor oldCursor = GSC_ProjectEditor.Tables.GetTableCursorFromWorkspace(oldWorkspace, "Search", null, oldParticipantTableName);

                    //Get an insert cursor and iterate through participant
                    ICursor insertCursor = GSC_ProjectEditor.Tables.GetTableCursorFromWorkspace(newWorkspace, "Insert", null, participantTable);

                    IRow currentRow = oldCursor.NextRow();
                    while (currentRow != null)
                    {
                        //Detect if current row person ID is empty and alias
                        if (currentRow.get_Value(oldPersonIDIndex) != null)
                        {
                            //Create a row buffer object (a template of all fields of table)
                            IRowBuffer inRowBuffer = newTable.CreateRowBuffer();

                            //Current person id that will be sent to the alias
                            string oldPersonID = currentRow.get_Value(oldPersonIDIndex).ToString();

                            //Iterate through all field of source
                            for (int i = 1; i < inRowBuffer.Fields.FieldCount; i++) //Skip OID
                            {


                                //Get current old field name
                                string currentFieldName = inRowBuffer.Fields.get_Field(i).Name;

                                //Get index of current field name inside the new workspace
                                int currentFieldInOldTableIndex = currentRow.Fields.FindField(currentFieldName);
                                object currentValue = DBNull.Value;

                                try
                                {

                                    //Get value of the current field, in the old workspace
                                    currentValue = currentRow.get_Value(currentFieldInOldTableIndex);

                                    //Find personID or alias field and process their values, else append normally
                                    if (currentFieldName == participantPersonID)
                                    {
                                        //Retrieve new value from dictionary, based on old value that was just like the alias
                                        currentValue = dicoParticipantPersonID[currentValue.ToString()];

                                    }

                                }
                                catch (Exception)
                                {
                                    currentValue = DBNull.Value;
                                }

                                //Show warning in case something is wrong
                                if (currentFieldName == participantPersonID && currentValue == DBNull.Value)
                                {
                                    MessageBox.Show("Missing value with " + participantPersonID + ", check for value related to field " + participantID + " with value : " + currentRow.get_Value(currentFieldInOldTableIndex));
                                }

                                inRowBuffer.set_Value(i, currentValue);

                            }

                            //Update
                            insertCursor.InsertRow(inRowBuffer);

                        }

                        currentRow = oldCursor.NextRow();
                    }
                    insertCursor.Flush();
                    //Release the cursor or else some lock could happen.
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(insertCursor);
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(oldCursor);

                    //Remove person table of list of table to append
                    newTableToUpdateFromPart.Remove(participantTable);

                }
                else
                {
                    //Do a normal append.
                }

                #endregion

            }
            catch (Exception updatePersonIDException)
            {

                MessageBox.Show(updatePersonIDException.StackTrace + "; " + updatePersonIDException.Message);
            }
        }

        /// <summary>
        /// Will return a list of all the tables and features that needs a special field mapping. Key will be table or feature name, Value is a tuple that first item is new field, and second item is a list of all old possible field names
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, Dictionary<string, List<string>>> FieldMapping()
        {
            //Variables
            Dictionary<string, Dictionary<string, List<string>>> fieldMapDico = new Dictionary<string, Dictionary<string, List<string>>>();

            fieldMapDico[legendGenTable] = new Dictionary<string, List<string>>(){{GSC_ProjectEditor.Constants.DatabaseFields.LegendSymbol, new List<string> { GSC_ProjectEditor.Constants.DatabaseFields.LegendSymbol_151001 }},
            {GSC_ProjectEditor.Constants.DatabaseFields.LegendGISDisplay, new List<string> { GSC_ProjectEditor.Constants.DatabaseFields.LegendLabelName }}};
            fieldMapDico[geolineFC] = new Dictionary<string, List<string>>() { { GSC_ProjectEditor.Constants.DatabaseFields.FGeolineFGDC, new List<string> { GSC_ProjectEditor.Constants.DatabaseFields.FGeolineFGDC_151001 } } };
            fieldMapDico[geopointFC] = new Dictionary<string, List<string>>() { { GSC_ProjectEditor.Constants.DatabaseFields.FGeopointFGDC, new List<string> { GSC_ProjectEditor.Constants.DatabaseFields.FGeopointFGDC_151001, GSC_ProjectEditor.Constants.DatabaseFields.FGeopointFGDC_151015 } } };

            return fieldMapDico;

        }

        /// <summary>
        /// From new workspace, will warn user that some record will be deleted because their inner definition was removed from new schema
        /// </summary>
        /// <param name="newWorkspace">The new workspace to find invalid records</param>
        /// <param name="tableName">The table name to find invalid records</param>
        /// <param name="fieldValuesToDelete">A dictionnary of invalid field values</param>
        public void WarnAboutMissingDomainValue(IWorkspace newWorkspace, string tableName, Dictionary<string, List<string>> fieldValuesToDelete)
        { 
            foreach (KeyValuePair<string, List<string>> invalidPair in fieldValuesToDelete)
	        {
		        List<string> tableFieldValues = GSC_ProjectEditor.Tables.GetFieldValuesFromWorkspace(newWorkspace, tableName, invalidPair.Key, null);

                foreach (string invalidValue in invalidPair.Value)
                {
                    if (tableFieldValues.Contains(invalidValue))
                    {
                        GSC_ProjectEditor.Messages.ShowGenericErrorMessage("New database schema doesn't contain value " + invalidValue + " in associated domain of field '" + invalidPair.Key + "' from feature/table '" + tableName + ". Please chose another value or ask question to your committee.");
                    }
                }
	        }
            
        }

        /// <summary>
        /// From new workspace, will warn user that some record will be deleted because their inner definition was removed from new schema
        /// </summary>
        /// <param name="newWorkspace">The new workspace to find invalid records</param>
        /// <param name="tableName">The table name to find invalid records</param>
        /// <param name="fieldValuesToDelete">A dictionnary of invalid field values that can be converted</param>
        public void ConvertMissingDomainValue(IWorkspace newWorkspace, string tableName, Dictionary<string, List<Tuple<string, string, string>>> fieldValuesToConvert)
        {
            foreach (KeyValuePair<string, List<Tuple<string, string, string>>> invalidPair in fieldValuesToConvert)
            {
                List<string> tableFieldValues = GSC_ProjectEditor.Tables.GetFieldValuesFromWorkspace(newWorkspace, tableName, invalidPair.Key, null);

                foreach (Tuple<string, string, string> invalidValue in invalidPair.Value)
                {
                    if (tableFieldValues.Contains(invalidValue.Item1))
                    {
                        GSC_ProjectEditor.Messages.ShowGenericErrorMessage("New database schema doesn't contain value " + invalidValue.Item1 + " in associated domain of field '" + invalidPair.Key + "' from feature/table '" + tableName + ". Converted value will be set as: " + invalidValue.Item2);

                        string whereQuery = invalidPair.Key + " = '" + invalidValue.Item1 + "'";

                        if (invalidValue.Item3 != string.Empty)
                        {
                            whereQuery = whereQuery + " AND " + invalidValue.Item3;
                        }
                        GSC_ProjectEditor.Tables.UpdateFieldValueFromWorkspace(newWorkspace, tableName, invalidPair.Key, whereQuery, invalidValue.Item2);
                    }
                }
            }
        }

        /// <summary>
        /// Will clean up some domains that invalid values were added so loading tables and features could work.
        /// </summary>
        /// <param name="newWorkspace">The workspace to clean domains from</param>
        /// <param name="domainValuesToDelete">The dictionary of invalid domain values</param>
        public void CleanDomain(IWorkspace newWorkspace, Dictionary<string, List<string>> domainValuesToDelete)
        {
            IWorkspaceDomains2 newDoms = newWorkspace as IWorkspaceDomains2;

            //Get a list of new version domains
            List<IDomain> listOfNewDomains = GSC_ProjectEditor.Domains.GetDomainList(newWorkspace);

            //Iterate through list and find match
            foreach (IDomain doms in listOfNewDomains)
            {
                int currentListIndex = listOfNewDomains.IndexOf(doms);

                //Get domain and associated coded values
                ICodedValueDomain domCodes = doms as ICodedValueDomain;

                //Get matching domain with missing values
                if (domainValuesToDelete.ContainsKey(doms.Name))
                {
                    foreach (string invalidValues in domainValuesToDelete[doms.Name])
                    {
                        domCodes.DeleteCode(invalidValues);
                    }

                    //Update domain
                    newDoms.AlterDomain(doms);
                }
                    
            }
        }

        #endregion

        #region VIEW
        /// <summary>
        /// Will prompt a geodatabase dialog for user to select the database to upgrade
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_SelectDB_Click(object sender, EventArgs e)
        {
            //Prompt dialog to select proper database
            string getDBPath = GSC_ProjectEditor.Dialog.GetFGDBPrompt(GSC_ProjectEditor.ArcMap.Application.hWnd, Properties.Resources.Message_SelectOLDPGDB);

            if (getDBPath != "")
            {
                this.txtbox_inDBPath.Text = getDBPath;
            }
        }

        /// <summary>
        /// Will prompt an xml dialog for user to select the latest .xml version of the schema
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_PromptXML_Click(object sender, EventArgs e)
        {
            //Get custom xml file prompt (only available within this application)
            string getUserXMLPath = Dialog.GetXMLFilePrompt(this.Handle.ToInt32());

            //Fill textbox
            this.textBox_upgradedSchemaPath.Text = getUserXMLPath;
        }

        /// <summary>
        /// Will close the form.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Upgrade geodatabase
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Upgrade_Click(object sender, EventArgs e)
        {
            //Validate entries
            if (this.txtbox_inDBPath.Text != string.Empty && this.txtbox_DBName.Text != string.Empty && this.textBox_upgradedSchemaPath.Text != string.Empty)
            {
                this.Cursor = Cursors.WaitCursor;

                //Get spatial reference from workspace to upgrade
                IWorkspace workspaceToUpgrade = GSC_ProjectEditor.Workspace.AccessWorkspace(this.txtbox_inDBPath.Text);
                IFeatureClass geolineFC = GSC_ProjectEditor.FeatureClass.OpenFeatureClassFromWorkspace(workspaceToUpgrade, GSC_ProjectEditor.Constants.Database.FGeoline);
                ISpatialReference workspaceProjection = GSC_ProjectEditor.SpatialReferences.GetSpatialRef(geolineFC);

                //Create a new geodatabase
                Form_Environment_NewGeodatabase newGeodatabase = new Form_Environment_NewGeodatabase();
                newGeodatabase.processFinished += new Form_Environment_NewGeodatabase.thisIsTheEndEventHandler(newGeodatabase_processFinished);
                newGeodatabase.CreateGDB(this.txtbox_DBName.Text, workspaceProjection, this.textBox_upgradedSchemaPath.Text);
                
                
            }

        }

        /// <summary>
        /// Will be triggered when the new geodatabase has been created
        /// </summary>
        public void newGeodatabase_processFinished()
        {
            //Validate extension
            string newDBPathName = this.txtbox_DBName.Text.Split('.')[0] + ".gdb"; //Path could contain twice the .gdb extension
            MainUpdateProcess(this.txtbox_inDBPath.Text, newDBPathName);

            //End
            this.Cursor = Cursors.Default;
            this.Close();
            GSC_ProjectEditor.Messages.ShowEndOfProcess();

        }

        /// <summary>
        /// Will prompt for a save path for the new file geodatabase.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_select_DBPath_Click(object sender, EventArgs e)
        {
            //Get custom xml file prompt (only available within this application)
            string getUserFGDBPath = Dialog.GetFGDBSavePrompt(this.Handle.ToInt32(), "Select output path for new File Geodatabase");

            //Fill textbox
            this.txtbox_DBName.Text = getUserFGDBPath;
        }

        #endregion


    }
}
