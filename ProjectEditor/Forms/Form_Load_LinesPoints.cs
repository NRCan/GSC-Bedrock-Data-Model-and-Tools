using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Resources;
using System.Reflection;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;

namespace GSC_ProjectEditor
{
    public partial class Form_Load_LinesPoints : Form
    {
        #region Main Variables

        //FEATURE GEO_LINES
        private const string geoline = GSC_ProjectEditor.Constants.Database.FGeoline;
        private const string geolineSubtype = GSC_ProjectEditor.Constants.DatabaseFields.FGeolineSubtype;
        private const string geolineD1 = GSC_ProjectEditor.Constants.DatabaseFields.FGeolineQualif;
        private const string geolineD2 = GSC_ProjectEditor.Constants.DatabaseFields.FGeolineConf;
        private const string geolineD3 = GSC_ProjectEditor.Constants.DatabaseFields.FGeolineAtt;
        private const string geolineD4 = GSC_ProjectEditor.Constants.DatabaseFields.FGeolineGeneration;
        private const string geolineSym = GSC_ProjectEditor.Constants.DatabaseFields.FGeolineFGDC;
        private const string geolineID = GSC_ProjectEditor.Constants.DatabaseFields.FGeolineID;
        private const string geolineBound = GSC_ProjectEditor.Constants.DatabaseFields.FGeolineBoundary;
        private const string geolineSubContactCode = GSC_ProjectEditor.Constants.DatabaseSubtypes.FGeolineSubContact;
        private const string geolineSubThinUnitCode = GSC_ProjectEditor.Constants.DatabaseSubtypes.FGeolineSubThinUnit;
        private const string geolineSubUnitConstructCode = GSC_ProjectEditor.Constants.DatabaseSubtypes.FGeolineSubUnitConstruct;
        private const string geolineMovement = GSC_ProjectEditor.Constants.DatabaseFields.FGeolineMovement;
        private const string geolineWall = GSC_ProjectEditor.Constants.DatabaseFields.FGeolineHangwall;
        private const string geolineFoldTrend = GSC_ProjectEditor.Constants.DatabaseFields.FGeolineFoldTrend;
        private const string geolineArrow = GSC_ProjectEditor.Constants.DatabaseFields.FGeolineArrowDir;
    
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
        private const string legendTableItemType = GSC_ProjectEditor.Constants.DatabaseFields.LegendItemType;
        private const string legendTableItemTypeLine = GSC_ProjectEditor.Constants.DatabaseDomainsValues.legendItemGeoline;
        private const string legendTableItemTypePoint = GSC_ProjectEditor.Constants.DatabaseDomainsValues.legendItemGeopoint;

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

        //Project station feature class
        private const string pStation = GSC_ProjectEditor.Constants.Database.gFCStation;
        private const string pStationID = GSC_ProjectEditor.Constants.DatabaseFields.FStationID;

        //GANFELD STRUC shapefile
        private const string gStruc = GSC_ProjectEditor.Constants.GanfeldShapefiles.shpStruc;

        //DOMAINS
        private const string domYesNo = GSC_ProjectEditor.Constants.DatabaseDomains.BoolYesNo;
        private const string domYesValue = GSC_ProjectEditor.Constants.DatabaseDomainsValues.BoolYes;

        //OTHER
        string seperator = GSC_ProjectEditor.Constants.Seperator.textFileLineSep;
        List<string> restrictedFieldList = new List<string>() { geolineSubtype, geolineD1, geolineD2, geolineD3, geolineD4, geolineBound, geolineMovement, geolineWall, geolineFoldTrend, geolineArrow };
        List<string> restrictedPntFieldList = new List<string>() { geopointType, geopointSubset, geopointStrucAtt, geopointStrucGene, geopointStrucYoung, geopointStrucMethod, geopointStrucFlat, geopointStrucStrain, geopointStrucDip,geopointStrucSense };
        string structureGenerationDefaultException = "10"; //Will be used to process default values for exception regarding some structure type only, superseeding Ganfeld.

        #endregion

        public Form_Load_LinesPoints()
        {
            //Set culture
            Utilities.Culture.SetCulture();

            InitializeComponent();

        }

        /// <summary>
        /// This button will close the window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_CancelAddGEMData_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// This button will import the data
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_ImportGEMData_Click(object sender, EventArgs e)
        {

            //Parse data type and process them separately
            if(this.cbox_DataType.SelectedIndex == 0)
            {
                processGeolines();
            }
            else if (this.cbox_DataType.SelectedIndex == 1)
            {
                processGeopoints();
            }
            //else if (this.cbox_DataType.SelectedIndex == 2)
            //{
            //    //Special procedure for struc data coming from Ganfeld
            //    processGeopointsGanfeldShapefile(); 
            //}
            //else if (this.cbox_DataType.SelectedIndex == 3)
            //{
            //    //Special procedure for struc data coming from Ganfeld inside current project database
            //    processGeopointsGanfeldProjectTable(); 
            //}

            //Show end of process message
            GSC_ProjectEditor.Messages.ShowEndOfProcess();

            //Close form
            this.Close();


        }

        /// <summary>
        /// Will output a conversion dictionary. Keys will be ganfeld text value, Values will be project intented domain code
        /// </summary>
        /// <param name="settingTableName">Internal Main repo setting uploaded table name in which the conversion is present</param>
        /// <returns></returns>
        private Dictionary<string, string> BuildConversionDico(string mainResource)
        {
            //Variables
            Dictionary<string, string> outputConversionDico = new Dictionary<string, string>();

            //Get all lines from conversion textfile
            List<string> mainList = new List<string>(mainResource.Split(new[] { Environment.NewLine }, StringSplitOptions.None));

            //Iterate through lines and build the dictionary
            foreach (string line in mainList)
            {
                //Split line with separator
                string[] splitedLine = line.Split(seperator.ToArray());

                //Build dico, key= Ganfeld, value=dom code BedrockGDB
                outputConversionDico[splitedLine[3]] = splitedLine[1];
            }

            return outputConversionDico;

        }

        /// <summary>
        /// A button to browser for data (shape or Feature classes)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>    
        private void btn_DataBrowser_Click(object sender, EventArgs e)
        {
            //Prompt a folder dialog
            string shapeFolderPath = GSC_ProjectEditor.Dialog.GetFeatureShapePrompt(this.Handle.ToInt32());

            //Add path to textbox
            this.txtbox_DataPath.Text = shapeFolderPath;

            //Fill the field combobox
            if (this.txtbox_DataPath.Text != "")
            {
                fillFieldCombobox(this.txtbox_DataPath.Text);
            }
            else
            {
                //Reset combobox
                this.cbox_SelectIDField.Items.Clear();
            }
            

        }

        /// <summary>
        /// Will fill the field combobox with all input feature fields
        /// </summary>
        /// <param name="featurePath"></param>
        public void fillFieldCombobox(string featurePath)
        {
            //Reset combobox
            this.cbox_SelectIDField.Items.Clear();
            
            IFeatureClass getInputFC = GSC_ProjectEditor.FeatureClass.OpenFeatureClassFromString(featurePath);

            //Get a list of fields from within the feature
            List<string> getFieldList = GSC_ProjectEditor.FeatureClass.GetFieldList(getInputFC, true);

            //Add to combobox
            foreach (string field in getFieldList)
            {
                this.cbox_SelectIDField.Items.Add(field);
            }
        }

        /// <summary>
        /// Will process the new data for geolines
        /// </summary>
        public void processGeolines()
        {
            //Variables
            Dictionary<string, List<string>> fieldMapDico = new Dictionary<string, List<string>>();

            //Get input and output features
            IFeatureClass inFC = GSC_ProjectEditor.FeatureClass.OpenFeatureClassFromString(this.txtbox_DataPath.Text);
            IFeatureClass outFC = GSC_ProjectEditor.FeatureClass.OpenFeatureClass(geoline);

            //Validate input fields, detect if restricted field names are being used by input feature class
            List<string> inputFields = GSC_ProjectEditor.FeatureClass.GetFieldList(inFC, false);
            List<string> errorFields = new List<string>(); //Will be use if input feature class has some fields with restricted names

            //Validate for restricted field if user want's it
            if (checkBox_FieldValidation.Checked)
            {
                foreach (string fieldNames in restrictedFieldList)
                {
                    if (inputFields.Contains(fieldNames))
                    {
                        errorFields.Add(fieldNames);

                    }
                }
            }

            if (errorFields.Count == 0)
            {
                //Get some field names and build a field map dictionary
                string inGeolineID = this.cbox_SelectIDField.SelectedItem.ToString();
                fieldMapDico[geolineSubtype] = new List<string>(new string[] { inGeolineID, "0", "1" });
                fieldMapDico[geolineD1] = new List<string>(new string[] { inGeolineID, "2", "5" });
                fieldMapDico[geolineD2] = new List<string>(new string[] { inGeolineID, "6", "7" });
                fieldMapDico[geolineD3] = new List<string>(new string[] { inGeolineID, "8", "9" });
                fieldMapDico[geolineD4] = new List<string>(new string[] { inGeolineID, "10", "11" });
                fieldMapDico[geolineID] = new List<string>(new string[] { inGeolineID, "0", "12" });

                //Call an append method
                GSC_ProjectEditor.GeoProcessing.AppendDataWithFieldMap(inFC, outFC, fieldMapDico);

                //Set select code form
                processSymbolAndLegendTables(outFC, geolineSymbol, geolineSymbolGEOID);

                //Set IsBoundary field to schema default value based on subtype value
                ///Should not modify existing data, only the appended one
                processGeolineBoundary(inFC, outFC);

            }
            else
            {
                foreach (string probFields in errorFields)
                {
                    MessageBox.Show(Properties.Resources.Error_RestrictiedFieldName + " " + probFields);
                }
                
            }

            //Close form
            this.Close();

        }

        /// <summary>
        /// Will process the new data for geopoints
        /// </summary>
        public void processGeopoints()
        {

            //Variables
            Dictionary<string, List<string>> fieldMapDico = new Dictionary<string, List<string>>();

            //Get input and output features
            IFeatureClass inFC = GSC_ProjectEditor.FeatureClass.OpenFeatureClassFromString(this.txtbox_DataPath.Text);
            IFeatureClass outFC = GSC_ProjectEditor.FeatureClass.OpenFeatureClass(geopoints);

             //Validate input fields, detect if restricted field names are being used by input feature class
            List<string> inputFields = GSC_ProjectEditor.FeatureClass.GetFieldList(inFC, false);
            List<string> errorFields = new List<string>(); //Will be use if input feature class has some fields with restricted names

            //Validate for restricted field if user want's it
            if (checkBox_FieldValidation.Checked)
            {
                foreach (string fieldNames in restrictedPntFieldList)
                {
                    if (inputFields.Contains(fieldNames))
                    {
                        errorFields.Add(fieldNames);

                    }
                }
            }

            if (errorFields.Count == 0)
            {

                //Get some field names and build a field map dictionary
                string inGeolineID = this.cbox_SelectIDField.SelectedItem.ToString();
                fieldMapDico[geopointType] = new List<string>(new string[] { inGeolineID, "0", "0" });
                fieldMapDico[geopointSubset] = new List<string>(new string[] { inGeolineID, "1", "4" });
                fieldMapDico[geopointStrucAtt] = new List<string>(new string[] { inGeolineID, "5", "6" });
                fieldMapDico[geopointStrucGene] = new List<string>(new string[] { inGeolineID, "7", "8" });
                fieldMapDico[geopointStrucYoung] = new List<string>(new string[] { inGeolineID, "9", "10" });
                fieldMapDico[geopointStrucMethod] = new List<string>(new string[] { inGeolineID, "11", "12" });
                fieldMapDico[geopointID] = new List<string>(new string[] { inGeolineID, "0", "13" });

                //Call an append method
                GSC_ProjectEditor.GeoProcessing.AppendDataWithFieldMap(inFC, outFC, fieldMapDico);
            
                //Set select code form
                processSymbolAndLegendTables(outFC, geopointSymbol, geopointSymbolGEOID);
            }
            else
            {
                foreach (string probFields in errorFields)
                {
                    MessageBox.Show(Properties.Resources.Error_RestrictiedFieldName + " " + probFields);
                }

            }

            //Close form
            this.Close();

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
                

                //Get feature name
                IDataset inDS = inputFC as IDataset;
                IWorkspace inWorkspace = inDS.Workspace;
                string inName = inDS.BrowseName;
                string symType = string.Empty;
                string itemType = string.Empty;

                //Get a list of new ids from input feature class
                Dictionary<string, List<string>> uniqueValues = GSC_ProjectEditor.Tables.GetUniqueFieldValues(inName, outFieldName, null, true, fgdcSymbol);
                List<string> symbolDico = GSC_ProjectEditor.Tables.GetFieldValues(legendTable, legendTableID, null);

                //Update database 
                foreach (string userNewData in uniqueValues["Main"])
                {
                    if (userNewData!=string.Empty) //Some empty ids could be passed here
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
                                itemType = legendTableItemTypeLine;
	                        }
                            else 
	                        {
                                tupleFieldList = new Tuple<string, string, string>(geopointSymbolGEOID, fgdcSymbolPoint, geopointSymbolDesc);
                                symType = legendTableSymTypePoint;
                                itemType = legendTableItemTypePoint;
	                        }

                            Dictionary<string, Tuple<string, string>> tripleResults = GSC_ProjectEditor.Tables.GetUniqueDicoTripleFieldValues(outputSymbolTable, tupleFieldList, selectedQuery);

                            if (userNewData!=null)
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
                                newRowDico[legendTableItemType] = itemType;
                                GSC_ProjectEditor.Tables.AddRowWithValues(legendTable, newRowDico);

                            }

                            

                        }


                    }

                }

                

                //Release coms
                System.Runtime.InteropServices.Marshal.ReleaseComObject(inWorkspace);
            }
            catch (Exception processSymbolAndLegendTablesError)
            {
                MessageBox.Show("processSymbolAndLegendTablesError: " + processSymbolAndLegendTablesError.StackTrace);
            }

        }

        /// <summary>
        /// Manage selected input. Result will enable or disable checkbox for F_STRUC
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbox_DataType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.cbox_DataType.SelectedItem.ToString().ToLower().Contains("shapefile"))
            {
                //Disable ID field selection for incoming struc shapefiles
                this.cbox_SelectIDField.Enabled = false;
                this.label_SelectIDField.Enabled = false;

            }
            else if (this.cbox_DataType.SelectedItem.ToString().ToLower().Contains("geodatabase"))
            {
                //Disable ID field selection and data input for incoming struc table
                this.cbox_SelectIDField.Enabled = false;
                this.txtbox_DataPath.Enabled = false;
                this.label_SelectIDField.Enabled = false;
                this.label_Import.Enabled = false;
                this.btn_DataBrowser.Enabled = false;
            }
            else
            {
                //Enable everything
                this.cbox_SelectIDField.Enabled = true;
                this.txtbox_DataPath.Enabled = true;
                this.label_SelectIDField.Enabled = true;
                this.label_Import.Enabled = true;
                this.btn_DataBrowser.Enabled = true;
            }
        }

        /// <summary>
        /// Will update isBoundary field based on preset subtype value
        /// </summary>
        /// <param name="inFC">The input feature that was appended to geoline feature</param>
        /// <param name="outFC">Geoline feature class</param>
        public void processGeolineBoundary(IFeatureClass inFC, IFeatureClass outFC)
        {
            //Get other object from input feature class
            IDataset inDataset = inFC as IDataset;
            IWorkspace inWork = inDataset.Workspace;
            string inName = inDataset.Name;

            //Get other object from output feature class
            IDataset outDataset = outFC as IDataset;
            string outName = outDataset.Name;

            //Count number of feature inside input feature class
            int countInFC = GSC_ProjectEditor.Tables.GetRowCountFromWorkspace(inWork, inName, null);

            //Get a dictionary of subtypes
            SortedDictionary<string, int> geolineSubDico = GSC_ProjectEditor.Subtypes.GetSubtypeDico(outName);

            //Convert to a list with only subtype codes
            List<int> geolineSubCodes = geolineSubDico.Values.ToList();

            //Build a dictionary of default values based on subtype values for isBoundary field Key = subtype code value = isboundary default value
            Dictionary<int, string> defaultIsBoundaries = new Dictionary<int,string>();
            foreach (int subtypes in geolineSubCodes)
            {
                //Add to dictionary
                defaultIsBoundaries[subtypes] = GSC_ProjectEditor.Subtypes.GetSubtypeFieldDefault(outName, geolineBound, subtypes);
            }

            //Iterate through an update cursor and update isBoundary field
            ICursor upCursor = GSC_ProjectEditor.Tables.GetTableCursor("Update", null, outName);
            IRow upRow = upCursor.NextRow();
            int counter = 1;
            int subtypeFieldIndex = upCursor.FindField(geolineSubtype);
            int isBoundaryFieldIndex = upCursor.FindField(geolineBound);
            while (upRow!=null)
            {
                //Process only the appended data
                if (counter>= countInFC)
                {

                    //Update current row with default value
                    if (upRow.get_Value(subtypeFieldIndex) != null)
                    {
                        upRow.set_Value(isBoundaryFieldIndex, defaultIsBoundaries[Convert.ToInt16(upRow.get_Value(subtypeFieldIndex))]);
                        upCursor.UpdateRow(upRow);
                    }
                    
                }

                //Next iteration
                counter = counter + 1;
                upRow = upCursor.NextRow();
            }

            //Release the cursor or else some lock could happen.
            System.Runtime.InteropServices.Marshal.ReleaseComObject(upCursor);


        }

    }



}
