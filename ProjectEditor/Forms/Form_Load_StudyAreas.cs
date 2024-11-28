using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ESRI.ArcGIS.Geodatabase;
using System.Globalization;
using ESRI.ArcGIS.Geometry;

namespace GSC_ProjectEditor
{
    public partial class Form_Load_StudyAreas : Form
    {
        #region Main Variables

        //Study Area table and feature
        private const string TArea = GSC_ProjectEditor.Constants.Database.TStudyAreaIndex;
        private const string FArea = GSC_ProjectEditor.Constants.Database.FStudyArea;
        private const string TAreaFC = GSC_ProjectEditor.Constants.DatabaseFields.TStudyAreaFC;
        private const string TAreaRowID = GSC_ProjectEditor.Constants.DatabaseFields.TStudyAreaRowID;
        private const string TAreaTableName = GSC_ProjectEditor.Constants.DatabaseFields.TStudyAreaName;
        private const string FAreaAbbr = GSC_ProjectEditor.Constants.DatabaseFields.FStudyAreaAbbr;
        private const string FAreaNorth = GSC_ProjectEditor.Constants.DatabaseFields.FStudyAreaNorth;
        private const string FAreaSouth = GSC_ProjectEditor.Constants.DatabaseFields.FStudyAreaSouth;
        private const string FAreaEast = GSC_ProjectEditor.Constants.DatabaseFields.FStudyAreaEast;
        private const string FAreaWest = GSC_ProjectEditor.Constants.DatabaseFields.FStudyAreaWest;
        private const string FAreaRemarks = GSC_ProjectEditor.Constants.DatabaseFields.FStudyAreaRemarks;
        private const string FAreaRelatedID = GSC_ProjectEditor.Constants.DatabaseFields.FStudyAreaRelatedID;

        //Sub Activity table
        private const string subActicityTable = GSC_ProjectEditor.Constants.Database.TSActivity;
        private const string subActicityName = GSC_ProjectEditor.Constants.DatabaseFields.SubActivityName;
        private const string subActicityID = GSC_ProjectEditor.Constants.DatabaseFields.SubActivityID;
        private const string subActivityAbbre = GSC_ProjectEditor.Constants.DatabaseFields.SubActivityAbbr;

        //Main Activity table
        private const string mainActTable = GSC_ProjectEditor.Constants.Database.TMActivity;
        private const string mainActID = GSC_ProjectEditor.Constants.DatabaseFields.MainActID;
        private const string mainActName = GSC_ProjectEditor.Constants.DatabaseFields.MainActivityName;

        //Project table
        private const string projectFieldID = GSC_ProjectEditor.Constants.DatabaseFields.ProjectID;
        private const string projectFieldName = GSC_ProjectEditor.Constants.DatabaseFields.ProjectName;
        private const string projectTable = GSC_ProjectEditor.Constants.Database.TProject;

        //CGM Map feature
        private const string FCGMFeature = GSC_ProjectEditor.Constants.Database.FCGMIndex;
        private const string FCGMName = GSC_ProjectEditor.Constants.DatabaseFields.FCGM_Name;
        private const string FCGMID = GSC_ProjectEditor.Constants.DatabaseFields.FCGM_ID;
        private const string FCGMRelatedID = GSC_ProjectEditor.Constants.DatabaseFields.FCGM_RelatedID;
        private const string FCGMNorth = GSC_ProjectEditor.Constants.DatabaseFields.FCGM_North;
        private const string FCGMSouth = GSC_ProjectEditor.Constants.DatabaseFields.FCGM_South;
        private const string FCGMEast = GSC_ProjectEditor.Constants.DatabaseFields.FCGM_East;
        private const string FCGMWest = GSC_ProjectEditor.Constants.DatabaseFields.FCGM_West;
        private const string FCGMRemarks = GSC_ProjectEditor.Constants.DatabaseFields.FCGM_Remarks;
        private const string FCGMAbstract = GSC_ProjectEditor.Constants.DatabaseFields.FCGM_Abstract;
        private const string FCGMAbbr = GSC_ProjectEditor.Constants.DatabaseFields.FCGM_ID;

        //Legend tree table
        private const string TTree = GSC_ProjectEditor.Constants.Database.TLegendTree;
        private const string TTreeCGMID = GSC_ProjectEditor.Constants.DatabaseFields.LegendTreeCGM;

        //Legend generator table
        private const string TLegend = GSC_ProjectEditor.Constants.Database.TLegendGene;

        //Source table
        private const string TSource = GSC_ProjectEditor.Constants.Database.TSource;
        private const string TSourceID = GSC_ProjectEditor.Constants.DatabaseFields.TSourceID;
        private const string TSourceName = GSC_ProjectEditor.Constants.DatabaseFields.TsourceName;
        private const string TSourceAbbr = GSC_ProjectEditor.Constants.DatabaseFields.TSourceAbbr;

        //Coordinate validation
        public Double parseResult; //Will be used to try parsing the text as an output double
        public NumberStyles style = NumberStyles.Number; //Will be used to force parse for numbers
        public CultureInfo culture = CultureInfo.InvariantCulture; //Will be used to force parse with an invariant culture (. and , will be accepted)

        //Other
        private const string objectID = GSC_ProjectEditor.Constants.DatabaseFields.ObjectID;
        public Dictionary<string, string> activityFieldList = new Dictionary<string, string>();
        private Dictionary<string, object> inFieldValues = new Dictionary<string, object>(); //Will be used to add info in legend generator table

        #endregion

        #region View Model

        /// <summary>
        /// A class that will keep unique ids and names of study areas
        /// Will be used as main study area combobox datasource
        /// </summary>
        public class UniqueIDs
        {
            public string Name { get; set; }
            public string Value { get; set; }
        }

        public Form_Load_StudyAreas()
        {
            //Init culture
            Utilities.Culture.SetCulture();

            InitializeComponent();

            //Initiate some custom controls.
            this.Shown += new EventHandler(FormStudyAreas_Shown);
        }

        void FormStudyAreas_Shown(object sender, EventArgs e)
        {
            if (GSC_ProjectEditor.Properties.Settings.Default.dwEnabling)
            {
                try
                {
                    fillAreaCombobox();

                }
                catch (Exception dwAddActException)
                {
                    MessageBox.Show(dwAddActException.Message);
                }
            }
            else
            {
                this.Close();
            }
        }

        /// <summary>
        /// Will fill the value combobox, based on user selected purpose
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbox_AreaPurpose_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Cast combobox
            ComboBox currentCombo = sender as ComboBox;

            if (currentCombo.SelectedIndex != -1)
            {
                //Clean combobox
                this.cbox_AreaPurposeValue.Items.Clear();
                this.cbox_AreaPurposeValue.Tag = null;

                //Init variables
                string itemID = "";
                string itemName = "";
                string itemTable = "";

                #region Fill values for Projects
                if (this.cbox_AreaPurpose.SelectedIndex == 0)
                {
                    itemID = projectFieldID;
                    itemName = projectFieldName;
                    itemTable = projectTable;
                }
                #endregion

                #region Fill values for main activities
                if (this.cbox_AreaPurpose.SelectedIndex == 1)
                {
                    itemID = mainActID;
                    itemName = mainActName;
                    itemTable = mainActTable;
                }
                #endregion

                #region Fill values for sub activities
                if (this.cbox_AreaPurpose.SelectedIndex == 2)
                {
                    itemID = subActicityID;
                    itemName = subActicityName;
                    itemTable = subActicityTable;
                }
                #endregion

                #region Fill values for sources
                if (this.cbox_AreaPurpose.SelectedIndex == 3)
                {
                    itemID = TSourceID;
                    itemName = TSourceAbbr;
                    itemTable = TSource;
                }
                #endregion

                //Tag purpose with selected table name
                this.cbox_AreaPurpose.Tag = itemTable;

                //Get a list of person from table
                Dictionary<string, List<string>> getDicoItem = GSC_ProjectEditor.Tables.GetUniqueFieldValues(itemTable, itemID, null, true, itemName);

                //Iterate through dico and build a list
                foreach (string getName in getDicoItem["Tag"])
                {
                    this.cbox_AreaPurposeValue.Items.Add(getName);
                }

                if (this.cbox_AreaPurposeValue.Items.Count == 0)
                {
                    this.cbox_AreaPurposeValue.Items.Add(Properties.Resources.Error_PurposeValue);
                }

                this.cbox_AreaPurposeValue.SelectedIndex = 0;
                this.cbox_AreaPurposeValue.Tag = getDicoItem;

            }



        }

        /// <summary>
        /// This button will erase all values in the form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_ClearAreaBoxes_Click(object sender, EventArgs e)
        {
            clearBoxes();
        }

        /// <summary>
        /// This button will pop a feature class dialog, for user to select desire feature
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_AreaImport_Click(object sender, EventArgs e)
        {
            this.txtbox_AreaImportPath.Text = GSC_ProjectEditor.Dialog.GetFeatureShapePrompt(this.Handle.ToInt32());
        }

        /// <summary>
        /// Will update or add a new study area feature within database. Index table will also be filled.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_AddArea_Click(object sender, EventArgs e)
        {
            //Validate if all needed fields have been filled
            if (this.cbox_selectArea.SelectedIndex != -1 && this.cbox_AreaPurpose.SelectedIndex != -1 && this.cbox_AreaPurposeValue.SelectedIndex != -1 && this.cbox_AreaPurposeValue.SelectedItem.ToString() != Properties.Resources.Error_PurposeValue && this.txtbox_AreaName.Text != "")
            {

                //Retrieve info from main area combobox
                UniqueIDs getCurrentArea = this.cbox_selectArea.SelectedItem as UniqueIDs;
                string getCurrentAreaName = getCurrentArea.Name;

                #region Set fields and features to updates
                Dictionary<string, string> outputNames = getNames(); //Will contains proper field names and feature names
                #endregion

                #region Process manual coordinate entry
                if (this.txtbox_AreaNorth.Text != "" && this.txtbox_AreaSouth.Text != "" && this.txtbox_AreaWest.Text != "" && this.txtbox_AreaEast.Text != "" && getCurrentAreaName.Contains(Properties.Resources.FindKeyWord_01))
                {
                    ManualCoordEntry(outputNames);
                }
                #endregion

                #region Process import feature entry
                if (this.txtbox_AreaImportPath.Text != null && this.txtbox_AreaImportPath.Text != "" && getCurrentAreaName.Contains(Properties.Resources.FindKeyWord_01))
                {
                    ImportFeatureEntry(outputNames);
                }
                #endregion

                #region Process update instead of adding new feature
                if (!getCurrentAreaName.Contains(Properties.Resources.FindKeyWord_01))
                {
                    UpdateFeature(outputNames);
                }
                #endregion

                //Reset interface
                clearBoxes();
                fillAreaCombobox();
            }
            else
            {
                MessageBox.Show(Properties.Resources.EmptyFields);
            }

        }

        /// <summary>
        /// Validate north coordinate
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtbox_AreaNorth_TextChanged(object sender, EventArgs e)
        {
            if (!Double.TryParse(this.txtbox_AreaNorth.Text.ToString(), style, culture, out parseResult) && this.txtbox_AreaNorth.Text != "")
            {
                MessageBox.Show(Properties.Resources.Error_InvalidCoord);
            }
        }

        /// <summary>
        /// Validate west coordinate
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtbox_AreaWest_TextChanged(object sender, EventArgs e)
        {
            if (!Double.TryParse(this.txtbox_AreaWest.Text.ToString(), style, culture, out parseResult) && this.txtbox_AreaWest.Text != "")
            {
                MessageBox.Show(Properties.Resources.Error_InvalidCoord);
            }
        }

        /// <summary>
        /// Validate east coordinate
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtbox_AreaEast_TextChanged(object sender, EventArgs e)
        {
            if (!Double.TryParse(this.txtbox_AreaEast.Text.ToString(), style, culture, out parseResult) && this.txtbox_AreaEast.Text != "")
            {
                MessageBox.Show(Properties.Resources.Error_InvalidCoord);
            }
        }

        /// <summary>
        /// Validate south coordinate
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtbox_AreaSouth_TextChanged(object sender, EventArgs e)
        {
            if (!Double.TryParse(this.txtbox_AreaSouth.Text.ToString(), style, culture, out parseResult) && this.txtbox_AreaSouth.Text != "")
            {
                MessageBox.Show(Properties.Resources.Error_InvalidCoord);
            }
        }

        /// <summary>
        /// Update all combobox and textbox with selected area informations
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbox_selectArea_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                //Cast current combobox
                ComboBox currentCbox = sender as ComboBox;

                if (currentCbox.SelectedIndex != -1 && currentCbox.Items.Count != 0 && currentCbox.Tag.ToString() != "")
                {
                    UniqueIDs currentUniqueIDS = this.cbox_selectArea.SelectedItem as UniqueIDs;
                    string currentAreaName = currentUniqueIDS.Name;

                    if (!currentAreaName.Contains(Properties.Resources.FindKeyWord_01) && currentAreaName != "")
                    {
                        //Get proper field and feature names
                        Dictionary<string, string> getNamesFromAreaType = getNames();

                        //Add current selected name
                        this.txtbox_AreaName.Text = currentAreaName;

                        //Get other information from combobox item value
                        UniqueIDs cboxItem = this.cbox_selectArea.SelectedItem as UniqueIDs;
                        string cboxItemValue = cboxItem.Value;

                        //Get information from areas
                        string getCurrentAreaQuery = objectID + " = " + cboxItemValue;

                        IFeatureCursor areaCursor = GSC_ProjectEditor.FeatureClass.GetFeatureCursor("Search", getCurrentAreaQuery, getNamesFromAreaType["Feature"]);
                        IRow areaRow = areaCursor.NextFeature();

                        //Get some field indexes
                        int areaRemarksIndex = areaCursor.FindField(getNamesFromAreaType["AreaRemarks"]);
                        int areaRelatedIndex = areaCursor.FindField(getNamesFromAreaType["FeatureRelatedID"]);

                        //Iterate through cursor
                        while (areaRow != null)
                        {
                            //Fill textbox
                            this.txtbox_AreaRemarks.Text = areaRow.get_Value(areaRemarksIndex) as String;

                            //Get purpose
                            string getRelatedID = areaRow.get_Value(areaRelatedIndex) as String;
                            string purposeQuery = TAreaRowID + " = '" + getRelatedID + "'";
                            string getPurposeRawName = GSC_ProjectEditor.Tables.GetUniqueFieldValues(TArea, TAreaTableName, purposeQuery, false, null)["Main"][0];
                            string getPurpose = "";

                            //Parse purpose
                            if (getPurposeRawName == TSource)
                            {
                                getPurpose = "Source Data";
                            }
                            else if (getPurposeRawName == mainActTable)
                            {
                                getPurpose = "Main Activity";
                            }
                            else if (getPurposeRawName == subActicityTable)
                            {
                                getPurpose = "Sub Activity";
                            }
                            else if (getPurposeRawName == projectTable)
                            {
                                getPurpose = "Project";
                            }

                            //Fill purpose comboboxes
                            this.cbox_AreaPurpose.SelectedItem = getPurpose;

                            //Get purpose value
                            if (this.cbox_AreaPurposeValue.Tag != null)
                            {
                                Dictionary<string, List<string>> getValueDico = this.cbox_AreaPurposeValue.Tag as Dictionary<string, List<string>>;
                                int valueIDIndex = getValueDico["Main"].IndexOf(getRelatedID);
                                string valueText = getValueDico["Tag"][valueIDIndex];
                                this.cbox_AreaPurposeValue.SelectedItem = valueText;
                            }


                            //Next iter.
                            areaRow = areaCursor.NextFeature();
                        }

                        //Release cursor
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(areaCursor);
                    }
                    else
                    {
                        //Clear textboxes
                        this.txtbox_AreaEast.Text = "";
                        this.txtbox_AreaImportPath.Text = "";
                        this.txtbox_AreaNorth.Text = "";
                        this.txtbox_AreaRemarks.Text = "";
                        this.txtbox_AreaSouth.Text = "";
                        this.txtbox_AreaWest.Text = "";
                        this.txtbox_AreaName.Text = "";

                        //Clear comboboxes
                        this.cbox_AreaPurpose.SelectedIndex = -1;
                        this.cbox_AreaPurposeValue.SelectedIndex = -1;
                    }


                }

            }
            catch (Exception cboxSelecAreaException)
            {
                string lineNumber = GSC_ProjectEditor.Exceptions.LineNumber(cboxSelecAreaException).ToString();
                MessageBox.Show("cboxSelecAreaException (" + lineNumber + "): " + cboxSelecAreaException.Message);
            }
        }

        /// <summary>
        /// Validate that entry does not exceed 25 characters
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtbox_AreaName_TextChanged(object sender, EventArgs e)
        {
            if (this.txtbox_AreaName.Text.Length > 25)
            {
                MessageBox.Show(Properties.Resources.Error_Name25Lenght, Properties.Resources.Error_GenericTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.txtbox_AreaName.Text = this.txtbox_AreaName.Text.Substring(0, 25);
            }


        }

        #endregion

        #region Model

        /// <summary>
        /// Will fill current are combobox with existing value from associated table
        /// </summary>
        public void fillAreaCombobox()
        {
            try
            {
                //Variables
                List<UniqueIDs> newUniqueIDList = new List<UniqueIDs>();
                Dictionary<string, string> tagDico = new Dictionary<string, string>();

                //this.cbox_selectArea.Items.Add(Properties.Resources.Message_AddNewArea);
                newUniqueIDList.Add(new UniqueIDs { Name = Properties.Resources.Message_AddNewArea, Value = "" });

                //Clear all possible values
                this.cbox_selectArea.DataSource = null;
                this.cbox_selectArea.Tag = "";

                //Get a list of areas from study area feature
                List<string> getDicoAreaOID = GSC_ProjectEditor.Tables.GetFieldValues(FArea, objectID, null);
                List<string> getDicoAreaAbbr = GSC_ProjectEditor.Tables.GetFieldValues(FArea, FAreaAbbr, null); //Use this method to prevent unsynch list between OID and abbr. because abbr might not be unique.

                //Iterate through dico and build a list
                foreach (string getAreaName in getDicoAreaAbbr)
                {
                    int areaNameIndex = getDicoAreaAbbr.IndexOf(getAreaName);
                    newUniqueIDList.Add(new UniqueIDs { Name = getAreaName, Value = getDicoAreaOID[areaNameIndex] });
                    tagDico[getAreaName] = FArea;

                }

                //Get a list of areas from cgm map index feature
                List<string> getDicoCGMAreaOID = GSC_ProjectEditor.Tables.GetFieldValues(FCGMFeature, objectID, null);
                List<string> getDicoCGMAreaAbbr = GSC_ProjectEditor.Tables.GetFieldValues(FCGMFeature, FCGMAbbr, null);

                foreach (string getCGMName in getDicoCGMAreaAbbr)
                {
                    int CGMNameIndex = getDicoCGMAreaAbbr.IndexOf(getCGMName);
                    newUniqueIDList.Add(new UniqueIDs { Name = getCGMName, Value = getDicoCGMAreaOID[CGMNameIndex] });
                    tagDico[getCGMName] = FCGMFeature;
                }



                //this.cbox_selectArea.Sorted = true;
                this.cbox_selectArea.DataSource = newUniqueIDList;
                this.cbox_selectArea.DisplayMember = "Name";
                this.cbox_selectArea.ValueMember = "Value";

                this.cbox_selectArea.SelectedIndex = 0;
                this.cbox_selectArea.Tag = tagDico;


            }
            catch (Exception fillAreaComboboxError)
            {
                int lineNumber = GSC_ProjectEditor.Exceptions.LineNumber(fillAreaComboboxError);
                MessageBox.Show("fillAreaComboboxError line (" + lineNumber.ToString() + "): " + fillAreaComboboxError.Message);
            }

        }

        /// <summary>
        /// Use to clear all values from dw controls.
        /// </summary>
        private void clearBoxes()
        {
            //Clear textboxes
            this.txtbox_AreaEast.Text = "";
            this.txtbox_AreaImportPath.Text = "";
            this.txtbox_AreaNorth.Text = "";
            this.txtbox_AreaRemarks.Text = "";
            this.txtbox_AreaSouth.Text = "";
            this.txtbox_AreaWest.Text = "";
            this.txtbox_AreaName.Text = "";

            //Clear comboboxes
            this.cbox_selectArea.SelectedIndex = 0;
            this.cbox_AreaPurpose.SelectedIndex = -1;
            this.cbox_AreaPurposeValue.SelectedIndex = -1;

        }

        /// <summary>
        /// Will return a dictionnary of all textboxes, comboboxes values from interface
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, List<object>> GetInterfaceValues()
        {
            //Variables
            Dictionary<string, List<object>> valueDico = new Dictionary<string, List<object>>();

            valueDico["cbox_SA"] = new List<object> { this.cbox_selectArea.SelectedItem.ToString(), this.cbox_selectArea.Tag };
            valueDico["cbox_Purpose"] = new List<object> { this.cbox_AreaPurpose.SelectedItem.ToString(), this.cbox_AreaPurpose.Tag };
            valueDico["cbox_PurposeValue"] = new List<object> { this.cbox_AreaPurposeValue.SelectedItem.ToString(), this.cbox_AreaPurposeValue.Tag };
            valueDico["tbox_Name"] = new List<object> { this.txtbox_AreaName.Text };
            valueDico["tbox_Remarks"] = new List<object> { this.txtbox_AreaRemarks.Text };
            valueDico["tbox_North"] = new List<object> { this.txtbox_AreaNorth.Text };
            valueDico["tbox_South"] = new List<object> { this.txtbox_AreaSouth.Text };
            valueDico["tbox_East"] = new List<object> { this.txtbox_AreaEast.Text };
            valueDico["tbox_West"] = new List<object> { this.txtbox_AreaWest.Text };
            valueDico["tbox_Import"] = new List<object> { this.txtbox_AreaImportPath.Text };

            return valueDico;
        }

        /// <summary>
        /// Will add a new feature based on a manual coordinate entry
        /// </summary>
        /// <param name="names">Pass the dictionnary containing names based on user selection</param>
        public void ManualCoordEntry(Dictionary<string, string> names)
        {

            #region Pre-processing and other
            //Variables
            string newGeomQuery = names["AreaAbbr"] + " = '" + this.txtbox_AreaName.Text + "'";
            //List<string> objectIDList = new List<string>(); //Will be used to update index table with added study area row numbers
            Dictionary<string, object> indexFieldValues = new Dictionary<string, object>(); //Will be used to update index table with all needed info.

            //Build dictionnary of field values
            Dictionary<string, object> inFieldAndValues = new Dictionary<string, object>();
            inFieldAndValues[names["AreaAbbr"]] = this.txtbox_AreaName.Text;
            inFieldAndValues[names["AreaRemarks"]] = this.txtbox_AreaRemarks.Text;

            //Get purpose value id from tag
            Dictionary<string, List<string>> valueTag = this.cbox_AreaPurposeValue.Tag as Dictionary<string, List<string>>;
            int valueIDIndex = valueTag["Tag"].IndexOf(this.cbox_AreaPurposeValue.SelectedItem.ToString());
            string valueText = valueTag["Main"][valueIDIndex];
            inFieldAndValues[names["FeatureRelatedID"]] = valueText;

            #endregion

            #region Main Process
            if (this.txtbox_AreaNorth.Text != "" && this.txtbox_AreaSouth.Text != "" && this.txtbox_AreaWest.Text != "" && this.txtbox_AreaEast.Text != "")
            {
                #region Build parameters
                //Parse coordinates as double values, set style and culture to make sure . and , are working.
                double getWest = Double.Parse(this.txtbox_AreaWest.Text, style, culture);
                double getEast = Double.Parse(this.txtbox_AreaEast.Text, style, culture);
                double getNorth = Double.Parse(this.txtbox_AreaNorth.Text, style, culture);
                double getSouth = Double.Parse(this.txtbox_AreaSouth.Text, style, culture);

                //Build list of coordinate in order to make poitns
                List<List<double>> coordinateList = new List<List<double>>();
                coordinateList.Add(new List<double> { getWest, getSouth, 0 });
                coordinateList.Add(new List<double> { getWest, getNorth, 0 });
                coordinateList.Add(new List<double> { getEast, getNorth, 0 });
                coordinateList.Add(new List<double> { getEast, getSouth, 0 });
                coordinateList.Add(new List<double> { getWest, getSouth, 0 });

                //Complete dictionnary of field values
                inFieldAndValues[names["AreaEast"]] = this.txtbox_AreaEast.Text;
                inFieldAndValues[names["AreaNorth"]] = this.txtbox_AreaNorth.Text;
                inFieldAndValues[names["AreaSouth"]] = this.txtbox_AreaSouth.Text;
                inFieldAndValues[names["AreaWest"]] = this.txtbox_AreaWest.Text;

                #endregion

                //Add new coordinates within feature
                GSC_ProjectEditor.FeatureClass.AddOnePolygonFromCoord(names["Feature"], coordinateList, inFieldAndValues);

                IFeatureCursor newAddedGeomCursor = GSC_ProjectEditor.FeatureClass.GetFeatureCursor("Search", newGeomQuery, names["Feature"]);
                int objIndexM = newAddedGeomCursor.FindField(objectID);
                IFeature newAddedRow = newAddedGeomCursor.NextFeature();
                while (newAddedRow != null)
                {
                    //Update object id list
                    //objectIDList.Add(newAddedRow.get_Value(objIndexM).ToString());
                    newAddedRow = newAddedGeomCursor.NextFeature();
                }

                //Release coms
                System.Runtime.InteropServices.Marshal.ReleaseComObject(newAddedGeomCursor);
            }
            #endregion

            FillIndexTable(names);

        }

        /// <summary>
        /// Will add a new feature based on an import feature from user
        /// </summary>
        /// <param name="names">Pass the dictionnary containing names based on user selection</param>
        public void ImportFeatureEntry(Dictionary<string, string> names)
        {
            #region Pre-processing and other
            //Variables
            string newGeomQuery = names["AreaAbbr"] + " = '" + this.txtbox_AreaName.Text + "'";
            //List<string> objectIDList = new List<string>(); //Will be used to update index table with added study area row numbers
            Dictionary<string, object> indexFieldValues = new Dictionary<string, object>(); //Will be used to update index table with all needed info.
            bool isLine = false; //If user has entered a line feature and it needs to be converted to a polygon, set this to true

            //Build dictionnary of field values
            Dictionary<string, object> inFieldAndValues = new Dictionary<string, object>();
            inFieldAndValues[names["AreaAbbr"]] = this.txtbox_AreaName.Text;
            inFieldAndValues[names["AreaRemarks"]] = this.txtbox_AreaRemarks.Text;

            //Get purpose value id from tag
            Dictionary<string, List<string>> valueTag = this.cbox_AreaPurposeValue.Tag as Dictionary<string, List<string>>;
            int valueIDIndex = valueTag["Tag"].IndexOf(this.cbox_AreaPurposeValue.SelectedItem.ToString());
            string valueText = valueTag["Main"][valueIDIndex];
            inFieldAndValues[names["FeatureRelatedID"]] = valueText;

            #endregion

            #region Process import feature entry
            UniqueIDs currentSelectedID = this.cbox_selectArea.SelectedItem as UniqueIDs;
            string currentSelection = currentSelectedID.Name;
            bool appended = false;
            if (this.txtbox_AreaImportPath.Text != null && this.txtbox_AreaImportPath.Text != "")
            {
                //Test for polygons only
                IFeatureClass importedFC = GSC_ProjectEditor.FeatureClass.OpenFeatureClassFromString(this.txtbox_AreaImportPath.Text);
                if (importedFC.ShapeType == esriGeometryType.esriGeometryPolygon)
                {
                    #region Append with full Arc Objects method (Instantanious)
                    //Build a list dictionnary of field values
                    List<Dictionary<string, object>> listDico = new List<Dictionary<string, object>>();
                    listDico.Add(inFieldAndValues);

                    //Append features from import
                    appended = GSC_ProjectEditor.FeatureClass.AppendPolygon(this.txtbox_AreaImportPath.Text, true, names["Feature"], listDico, null);

                    #endregion

                }
                else
                {
                    //Cast inputs as feature classes
                    IFeatureClass studyAreaFeature = GSC_ProjectEditor.FeatureClass.OpenFeatureClass(names["Feature"]);
                    GSC_ProjectEditor.FeatureClass.ConvertLineToPolygon(studyAreaFeature, null, importedFC.Search(null, true));

                    //Before finishing the process, redo the query or else next step query cursor won't work with the new added polygon
                    newGeomQuery = names["AreaAbbr"] + " = ''";
                    isLine = true;
                }

                //Continu if no errors were detected
                if (appended || isLine)
                {
                    //Complete attribute table
                    FinishAppend(names, newGeomQuery, isLine, inFieldAndValues);

                    //Reset interface
                    FillIndexTable(names);
                }
            }

            #endregion

        }

        /// <summary>
        /// Will fill the index table based on new added features within CGM or Study Area feature. This table links those features to the related tables (project, activities and sources)
        /// will Also fill the legend tree table if applicable
        /// </summary>
        /// <param name="names">Pass the dictionnary containing names based on user selection</param>
        public void FillIndexTable(Dictionary<string, string> names)
        {
            #region Fill in the index table, by linking new feature with wanted table

            //Build a dico with all needed field values for index table
            Dictionary<string, object> fielDico = new Dictionary<string, object>(); //Will be used to update index table with all needed info.
            fielDico[TAreaTableName] = this.cbox_AreaPurpose.Tag.ToString();
            Dictionary<string, List<string>> getSelectedValueIDList = this.cbox_AreaPurposeValue.Tag as Dictionary<string, List<string>>;
            int valueIDIndex = getSelectedValueIDList["Tag"].IndexOf(this.cbox_AreaPurposeValue.SelectedItem.ToString());
            string valueText = getSelectedValueIDList["Main"][valueIDIndex];
            fielDico[TAreaRowID] = valueText;
            fielDico[TAreaFC] = names["Feature"];
            GSC_ProjectEditor.Tables.AddRowWithValues(TArea, fielDico);

            #endregion
        }

        /// <summary>
        /// Will update an existing features with new users values. The user can only change the name and remarks of a study area.
        /// </summary>
        /// <param name="names">Pass the dictionary containing names based on user selection</param>
        public void UpdateFeature(Dictionary<string, string> names)
        {

            //Inform user of current process
            MessageBox.Show("The update will only occur on area name and remark field.");

            //Build dictionnary of field values
            Dictionary<string, object> inFieldAndValues = new Dictionary<string, object>();
            inFieldAndValues[names["AreaAbbr"]] = this.txtbox_AreaName.Text;
            inFieldAndValues[names["AreaRemarks"]] = this.txtbox_AreaRemarks.Text;

            //Get purpose value id from tag
            Dictionary<string, List<string>> valueTag = this.cbox_AreaPurposeValue.Tag as Dictionary<string, List<string>>;
            int relatedIDIndex = valueTag["Tag"].IndexOf(this.cbox_AreaPurposeValue.SelectedItem.ToString());
            string relatedIDText = valueTag["Main"][relatedIDIndex];

            //Build a query to select proper row and update it
            string upAreaQuery = names["FeatureRelatedID"] + " = '" + relatedIDText + "'";

            //Update feature
            GSC_ProjectEditor.Tables.UpdateMultipleFieldValue(names["Feature"], upAreaQuery, inFieldAndValues);

            //Update Legend Tree Table if feature is CGM
            if (names["Feature"] == FCGMFeature)
            {
                UniqueIDs studyAreaIDs = this.cbox_selectArea.SelectedItem as UniqueIDs;
                string oldMapID = studyAreaIDs.Name;
                string updateTreeTable = TTreeCGMID + " = '" + oldMapID + "'";
                GSC_ProjectEditor.Tables.UpdateFieldValue(TTree, TTreeCGMID, updateTreeTable, this.txtbox_AreaName.Text);
            }
        }

        /// <summary>
        /// Will get a dictionnary of names from either P_STUDY_AREA feature or CGM_INDEXMAP feature
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> getNames()
        {
            //Variables
            Dictionary<string, string> nameDico = new Dictionary<string, string>();

            try
            {
                //Get full name and full ids from area combobox
                UniqueIDs currentUniqueIDS = this.cbox_selectArea.SelectedItem as UniqueIDs;
                string currentAreaName = currentUniqueIDS.Name;
                Dictionary<string, string> currentAreaDico = this.cbox_selectArea.Tag as Dictionary<string, string>;

                string currentAreaType = "";

                if (currentAreaDico.ContainsKey(currentAreaName))
                {
                    currentAreaType = currentAreaDico[currentAreaName];
                }

                //Detect CGM maps
                if (currentAreaType == FCGMFeature)
                {
                    nameDico["AreaAbbr"] = FCGMID;
                    nameDico["AreaRemarks"] = FCGMRemarks;
                    nameDico["AreaEast"] = FCGMEast;
                    nameDico["AreaSouth"] = FCGMSouth;
                    nameDico["AreaNorth"] = FCGMNorth;
                    nameDico["AreaWest"] = FCGMWest;
                    nameDico["Feature"] = FCGMFeature;
                    nameDico["FeatureRelatedID"] = FCGMRelatedID;
                }

                //Detect other features to put in Study Area
                else
                {
                    if (this.cbox_AreaPurposeValue.SelectedIndex != -1 && this.cbox_AreaPurposeValue.SelectedItem.ToString().Contains(Properties.Resources.Keyword_BuildCGM))
                    {
                        nameDico["AreaAbbr"] = FCGMID;
                        nameDico["AreaRemarks"] = FCGMRemarks;
                        nameDico["AreaEast"] = FCGMEast;
                        nameDico["AreaSouth"] = FCGMSouth;
                        nameDico["AreaNorth"] = FCGMNorth;
                        nameDico["AreaWest"] = FCGMWest;
                        nameDico["Feature"] = FCGMFeature;
                        nameDico["FeatureRelatedID"] = FCGMRelatedID;
                    }
                    else
                    {
                        nameDico["AreaAbbr"] = FAreaAbbr;
                        nameDico["AreaRemarks"] = FAreaRemarks;
                        nameDico["AreaEast"] = FAreaEast;
                        nameDico["AreaSouth"] = FAreaSouth;
                        nameDico["AreaNorth"] = FAreaNorth;
                        nameDico["AreaWest"] = FAreaWest;
                        nameDico["Feature"] = FArea;
                        nameDico["FeatureRelatedID"] = FAreaRelatedID;
                    }
                }
            }

            catch (Exception cboxSelecAreaException)
            {
                string lineNumber = GSC_ProjectEditor.Exceptions.LineNumber(cboxSelecAreaException).ToString();
                MessageBox.Show("getNames (" + lineNumber + "): " + cboxSelecAreaException.Message);
            }

            return nameDico;
        }

        /// <summary>
        /// Will append imported polygon feature to another polygon feature
        /// </summary>
        public void FinishAppend(Dictionary<string, string> names, string newGeomQuery, bool isLine, Dictionary<string, object> inFieldAndValues)
        {
            //Calculate and fill coordinates fields from new features enveloppe
            IFeatureCursor newGeomCursor = GSC_ProjectEditor.FeatureClass.GetFeatureCursor("Update", newGeomQuery, names["Feature"]);
            int eastIndex = newGeomCursor.FindField(names["AreaEast"]);
            int westIndex = newGeomCursor.FindField(names["AreaWest"]);
            int northIndex = newGeomCursor.FindField(names["AreaNorth"]);
            int southIndex = newGeomCursor.FindField(names["AreaSouth"]);
            int objIndex = newGeomCursor.FindField(objectID);

            //For lines only, because it's already added when it's a polygon
            int abbrIndex = newGeomCursor.FindField(names["AreaAbbr"]);
            int remarkIndex = newGeomCursor.FindField(names["AreaRemarks"]);
            int relateIDIndex = newGeomCursor.FindField(names["FeatureRelatedID"]);

            IFeature geomRow = newGeomCursor.NextFeature();

            while (geomRow != null)
            {
                //Get enveloppe coordinates
                string XMin = geomRow.ShapeCopy.Envelope.XMin.ToString();
                string XMax = geomRow.ShapeCopy.Envelope.XMax.ToString();
                string YMin = geomRow.ShapeCopy.Envelope.YMin.ToString();
                string YMax = geomRow.ShapeCopy.Envelope.YMax.ToString();

                //Update coord fields
                geomRow.set_Value(eastIndex, XMin);
                geomRow.set_Value(westIndex, XMax);
                geomRow.set_Value(northIndex, YMax);
                geomRow.set_Value(southIndex, YMin);

                if (isLine)
                {
                    geomRow.set_Value(abbrIndex, inFieldAndValues[names["AreaAbbr"]]);
                    geomRow.set_Value(remarkIndex, inFieldAndValues[names["AreaRemarks"]]);
                    geomRow.set_Value(relateIDIndex, inFieldAndValues[names["FeatureRelatedID"]]);
                }

                newGeomCursor.UpdateFeature(geomRow);

                geomRow = newGeomCursor.NextFeature();
            }

            //Release coms
            System.Runtime.InteropServices.Marshal.ReleaseComObject(newGeomCursor);
        }

        #endregion
    }
}
