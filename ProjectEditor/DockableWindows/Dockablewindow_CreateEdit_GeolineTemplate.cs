using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Linq;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.DataSourcesFile;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Desktop.AddIns;
using ESRI.ArcGIS.Editor;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geometry;

namespace GSC_ProjectEditor
{
    /// <summary>
    /// Designer class of the dockable window add-in. It contains user interfaces that
    /// make up the dockable window.
    /// </summary>
    public partial class Dockablewindow_CreateEdit_GeolineTemplate : UserControl
    {

        #region Main Variables
        
        //M_GEOLINE_SYMBOL
        private const string mGeolineSymbol = GSC_ProjectEditor.Constants.Database.TGeolineSymbol;
        private const string mGeolineLegendDesc = GSC_ProjectEditor.Constants.DatabaseFields.MGeolineLegendDescription;
        private const string mSelectCode = GSC_ProjectEditor.Constants.DatabaseFields.MGeolineSelectCode;
        private const string mGeolineID = GSC_ProjectEditor.Constants.DatabaseFields.MGeolineID;
        private const string mGeolineFGDC = GSC_ProjectEditor.Constants.DatabaseFields.MGeolineFGDC;
        

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
        private const string geolineSubFaultCode = GSC_ProjectEditor.Constants.DatabaseSubtypes.FGeolineSubFault;

        //Feature layer for geolines
        private const string geolineLayer = GSC_ProjectEditor.Constants.Layers.geoline;

        //Table legend generator
        private const string tLegendGen = GSC_ProjectEditor.Constants.Database.TLegendGene;
        private const string tLegendGenID = GSC_ProjectEditor.Constants.DatabaseFields.LegendLabelID;
        private const string tLegendGenSym = GSC_ProjectEditor.Constants.DatabaseFields.LegendSymbol;
        private const string tLegendGenName = GSC_ProjectEditor.Constants.DatabaseFields.LegendGISDisplay;
        private const string tLegendGenType = GSC_ProjectEditor.Constants.DatabaseFields.LegendSymType;
        private const string tLegendSymTheme = GSC_ProjectEditor.Constants.DatabaseFields.LegendItemType;

        //DOMAINS
        private const string domYesNo = GSC_ProjectEditor.Constants.DatabaseDomains.BoolYesNo;
        private const string boolYesValue = GSC_ProjectEditor.Constants.DatabaseDomainsValues.BoolYes;
        private const string boolNoValue = GSC_ProjectEditor.Constants.DatabaseDomainsValues.BoolNo;
        private const string boundYesValue = GSC_ProjectEditor.Constants.DatabaseDomainsValues.BoundYes;
        private const string boundNoValue = GSC_ProjectEditor.Constants.DatabaseDomainsValues.BoundNo;
        private const string lineSymbolType = GSC_ProjectEditor.Constants.DatabaseDomainsValues.SymTypeLine;

        //Other
        //private const string interGroupLayer = GSC_ProjectEditor.Constants.Layers.GroupInterpretation;
        private string dicoMain = "Main";
        Dictionary<string, string> DicoYesNo = null;
        //private string stylePath = null;//Utilities.globalVariables.Styles.MainStylePath;
        //private List<string> itemIDs = new List<string>(); //A list to contains all geoline specific ids to add to selected geoline list box.
        private List<Tuple<string, string, string>> itemInformation = new List<Tuple<string,string,string>>(); //A list to contain geolineID description and symbol code

        #endregion

        /// <summary>
        /// Init. 
        /// Dw used to create geoline feature templates, from user requested selection, made with values within database.
        /// </summary>
        /// <param name="hook"></param>
        public Dockablewindow_CreateEdit_GeolineTemplate(object hook)
        {

            //Set culture before init.
            Utilities.Culture.SetCulture();

            InitializeComponent();
            this.Hook = hook;

            //Manage person list, if enabled is already open before init.
            if (this.Enabled)
            {
                dockablewindowSelectGeoline_EnabledChanged(this, null);
            }
            else if (!this.Enabled)
            {
                //Init controls if enabled.
                this.EnabledChanged += new EventHandler(dockablewindowSelectGeoline_EnabledChanged);
            }

        }

        void dockablewindowSelectGeoline_EnabledChanged(object sender, EventArgs e)
        {
            try
            {

                #region dock window startup and control fill

                //Get a description dictionnary from Yes No Domain
                DicoYesNo = GSC_ProjectEditor.Domains.GetDomDico(domYesNo, "Description");

                //Fill in geoline type combobox
                SortedDictionary<string, int> geolineTypeUniqueValues = GSC_ProjectEditor.Subtypes.GetSubtypeDico(geoline);
                List<string> subNames = new List<string>();
                List<int> subCodes = new List<int>();

                foreach (KeyValuePair<string, int> subPair in geolineTypeUniqueValues)
                {
                    subNames.Add(subPair.Key);
                    subCodes.Add(subPair.Value);
                }

                this.cbox_GeolineType.DataSource = subNames;
                this.cbox_GeolineType.Tag = geolineTypeUniqueValues; //Add whole dictionnary to later access subtype codes
                this.cbox_GeolineType.SelectedIndex = -1;

                this.cbox_GeolineType.SelectedIndexChanged -= cbox_GeolineType_SelectedIndexChanged;
                this.cbox_GeolineType.SelectedIndexChanged += cbox_GeolineType_SelectedIndexChanged;

                //Init event handlers 
                Utilities.ProjectSymbols uLineSymbols = new Utilities.ProjectSymbols();
                uLineSymbols.refreshLineHasStarted += new Utilities.ProjectSymbols.refreshLineStyleEventHandler(uLineSymbols_refreshLineHasStarted);
                uLineSymbols.refreshLineHasEnded += new Utilities.ProjectSymbols.refreshLineStyleEventHandler(uLineSymbols_refreshLineHasEnded);

                #endregion
            }
            catch (Exception enabledChanged)
            {
                MessageBox.Show(enabledChanged.Message);
            }

        }

        /// <summary>
        /// Ending refresh event
        /// Changes cursor back to default icon.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void uLineSymbols_refreshLineHasEnded()
        {
            this.Cursor = Cursors.Default;
        }

        /// <summary>
        /// Starting refresh event
        /// Changes cursor for a waiting icon.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void uLineSymbols_refreshLineHasStarted()
        {
            this.Cursor = Cursors.WaitCursor;
        }

        void cbox_GeolineType_SelectedIndexChanged(object sender, EventArgs e)
        {

            //Fill in geoline qualifier combobox
            if (this.cbox_GeolineType.SelectedIndex != -1)
            {

                //Get selected subtype code
                string selectedSubtypeName = this.cbox_GeolineType.SelectedItem.ToString();
                SortedDictionary<string, int> selectedSubtypeDico = this.cbox_GeolineType.Tag as SortedDictionary<string, int>;
                int selectedSubtypeCode = selectedSubtypeDico[selectedSubtypeName];

                //Fill in qualifier combobox
                fill_QualifierCombobox(selectedSubtypeCode);

                //Fill in confidence combobox
                fill_ConfidenceCombobox(selectedSubtypeCode);

                //Fill in attitude combobox
                fill_AttitudeCombobox(selectedSubtypeCode);

                //Fill in generation combobox
                fill_GenerationCombobox(selectedSubtypeCode);

            }


        }

        private void fill_AttitudeCombobox(int attitudeCode)
        {
            //Get associated qualifier domain name
            string attitudeDomName = GSC_ProjectEditor.Domains.GetSubDomName(geoline, attitudeCode, geolineD3);

            //Get a list of domain values
            Dictionary<string, string> attitudeDomDico = GSC_ProjectEditor.Domains.GetDomDico(attitudeDomName, "Code");
            List<string> attitudeDomValues = attitudeDomDico.Values.ToList();

            //Fill in combobox
            this.cbox_GeolineAttitude.DataSource = attitudeDomValues;

            //Get default value
            string defaultValue = GSC_ProjectEditor.Subtypes.GetSubtypeFieldDefault(geoline, geolineD3, attitudeCode);

            //Set default value into cbox
            if (defaultValue != "")
            {
                //Get default value index within list and combobox
                string textValue = attitudeDomDico[defaultValue];
                int textIndex = attitudeDomValues.IndexOf(textValue);
                this.cbox_GeolineAttitude.SelectedIndex = textIndex;
            }

            //Add a dictionnary into tag
            this.cbox_GeolineAttitude.Tag = attitudeDomDico;

        }

        private void fill_GenerationCombobox(int generationCode)
        {
            //Get associated qualifier domain name
            string generationDomName = GSC_ProjectEditor.Domains.GetSubDomName(geoline, generationCode, geolineD4);

            //Get a list of domain values
            Dictionary<string, string> generationDomDico = GSC_ProjectEditor.Domains.GetDomDico(generationDomName, "Code");
            List<string> generationDomValues = generationDomDico.Values.ToList();

            //Fill in combobox
            this.cbox_GeolineGeneration.DataSource = generationDomValues;

            //Get default value
            string defaultValue = GSC_ProjectEditor.Subtypes.GetSubtypeFieldDefault(geoline, geolineD4, generationCode);

            //Set default value into cbox
            if (defaultValue != "")
            {
                //Get default value index within list and combobox
                string textValue = generationDomDico[defaultValue];
                int textIndex = generationDomValues.IndexOf(textValue);
                this.cbox_GeolineGeneration.SelectedIndex = textIndex;
            }

            //Add a dictionnary into tag
            this.cbox_GeolineGeneration.Tag = generationDomDico;
        }

        private void fill_ConfidenceCombobox(int confCode)
        {
            //Get associated qualifier domain name
            string confidenceDomName = GSC_ProjectEditor.Domains.GetSubDomName(geoline, confCode, geolineD2);

            //Get a list of domain values
            Dictionary<string, string> confidenceDomDico = GSC_ProjectEditor.Domains.GetDomDico(confidenceDomName, "Code");
            List<string> confidenceDomValues = confidenceDomDico.Values.ToList();
 
            //Fill in combobox
            this.cbox_GeolineConfidence.DataSource = confidenceDomValues;

            //Get default value
            string defaultValue = GSC_ProjectEditor.Subtypes.GetSubtypeFieldDefault(geoline, geolineD2, confCode);

            //Set default value into cbox
            if (defaultValue != "")
            {
                //Get default value index within list and combobox
                string textValue = confidenceDomDico[defaultValue];
                int textIndex = confidenceDomValues.IndexOf(textValue);
                this.cbox_GeolineConfidence.SelectedIndex = textIndex;
            }

            //Add a dictionnary into tag
            this.cbox_GeolineConfidence.Tag = confidenceDomDico;
        }

        void fill_QualifierCombobox(int qualifCode)
        {
            //Get associated qualifier domain name
            string qualifierDomName = GSC_ProjectEditor.Domains.GetSubDomName(geoline, qualifCode, geolineD1);

            //Get a domain dictionary
            Dictionary<string, string> qualifDomDico = GSC_ProjectEditor.Domains.GetDomDico(qualifierDomName, "Code");
            List<string> qualifierDomValues = qualifDomDico.Values.ToList();

            //Fill in combobox
            this.cbox_GeolineQualifier.DataSource = qualifierDomValues;

            //Get default value
            string defaultValue = GSC_ProjectEditor.Subtypes.GetSubtypeFieldDefault(geoline, geolineD1, qualifCode);
            
            //Set default value into cbox
            if (defaultValue != "")
            {
                //Get default value index within list and combobox
                string textValue = qualifDomDico[defaultValue];
                int textIndex = qualifierDomValues.IndexOf(textValue);
                this.cbox_GeolineQualifier.SelectedIndex = textIndex; 
            }
            
            //Add a dictionnary into tag
            this.cbox_GeolineQualifier.Tag = qualifDomDico;

        }

        #region Addin init.
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
            public Dockablewindow_CreateEdit_GeolineTemplate m_windowUI;

            /// <summary>
            /// Init. dw.
            /// </summary>
            public AddinImpl()
            {
            }

            /// <summary>
            /// Will create dw.
            /// </summary>
            /// <returns></returns>
            protected override IntPtr OnCreateChild()
            {
                m_windowUI = new Dockablewindow_CreateEdit_GeolineTemplate(this.Hook);
                return m_windowUI.Handle;
            }

            /// <summary>
            /// Will dispose of dw.
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
        /// When clicked, selected information within comboboxes will trigger a selection within table to get GEOLINEID and add legend description in list box.
        /// Will read information in symbol table for initialisation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_AddGeolineList_Click(object sender, EventArgs e)
        {

            //Get all cbox values
            string valueType = this.cbox_GeolineType.SelectedItem.ToString();
            string valueQualif = this.cbox_GeolineQualifier.SelectedItem.ToString();
            string valueConf = this.cbox_GeolineConfidence.SelectedItem.ToString();
            string valueAtt = this.cbox_GeolineAttitude.SelectedItem.ToString();
            string valueGener = this.cbox_GeolineGeneration.SelectedItem.ToString();

            if (this.cbox_GeolineType.SelectedItem == null || this.cbox_GeolineQualifier.SelectedItem == null || this.cbox_GeolineConfidence.SelectedItem == null || this.cbox_GeolineAttitude.SelectedItem == null || this.cbox_GeolineGeneration.SelectedItem == null)
            {
                MessageBox.Show(Properties.Resources.Error_AddGeoline);
            }

            else
            {
                //Build a geolineID from selected values in comboboxes
                SortedDictionary<string, int> geolineTypeDico = this.cbox_GeolineType.Tag as SortedDictionary<string, int>;
                Dictionary<string, string> geolineQualifDico = this.cbox_GeolineQualifier.Tag as Dictionary<string, string>;
                Dictionary<string, string> geolineConfDico = this.cbox_GeolineConfidence.Tag as Dictionary<string, string>;
                Dictionary<string, string> geolineAttDico = this.cbox_GeolineAttitude.Tag as Dictionary<string, string>;
                Dictionary<string, string> geolineGeneDico = this.cbox_GeolineGeneration.Tag as Dictionary<string, string>;

                //Reverse dictionnary key and values
                geolineQualifDico = geolineQualifDico.ToDictionary(kv => kv.Value, kv => kv.Key);
                geolineConfDico = geolineConfDico.ToDictionary(kv => kv.Value, kv => kv.Key);
                geolineAttDico = geolineAttDico.ToDictionary(kv => kv.Value, kv => kv.Key);
                geolineGeneDico = geolineGeneDico.ToDictionary(kv => kv.Value, kv => kv.Key);

                string geolineTypeCode = geolineTypeDico[valueType].ToString();
                string geolineQualifCode = geolineQualifDico[valueQualif];
                string geolineConfCode = geolineConfDico[valueConf];
                string geolineAttCode = geolineAttDico[valueAtt];
                string geolineGeneCode = geolineGeneDico[valueGener];
                string newItemGeolineID = geolineTypeCode + geolineQualifCode + geolineConfCode + geolineAttCode + geolineGeneCode;

                //Build a new sql query for list tag object
                string newItemQuery = mGeolineID + " = '" + newItemGeolineID + "'";

                //Compile all info from cbbox and add legend description to list
                try
                {
                    List<Tuple<string, string, string>> geolineGenUniqueValues = GSC_ProjectEditor.Tables.GetUniqueTripleFieldValues(mGeolineSymbol, new Tuple<string, string, string>(mGeolineID, mGeolineLegendDesc, mGeolineFGDC), newItemQuery);
                    this.lbox_GeolineSelected.Items.Add(geolineGenUniqueValues[0].Item2.ToString());

                    //Add new query to list and to tag of control
                    itemInformation.Add(geolineGenUniqueValues[0]);
                    this.lbox_GeolineSelected.Tag = itemInformation;
                }
                catch
                {
                    MessageBox.Show(Properties.Resources.Error_InvalidGeolineIDQuery);
                }

            }


        }

        /// <summary>
        /// Clears a list box selection from the list box itself.
        /// </summary>
        /// <param name="sencder"></param>
        /// <param name="e"></param>
        private void btn_ClearSelectedGeoline_Click(object sencder, EventArgs e)
        {
            while(this.lbox_GeolineSelected.SelectedItems.Count > 0)
            {

                //Remove from tag list, this step is important or else it unsync with current items in list box
                List<Tuple<string, string, string>> newTag = this.lbox_GeolineSelected.Tag as List<Tuple<string, string, string>>;
                newTag.RemoveAt(this.lbox_GeolineSelected.SelectedIndex);
                this.lbox_GeolineSelected.Tag = newTag;
                itemInformation = newTag;

                //Remove from list
                this.lbox_GeolineSelected.Items.Remove(this.lbox_GeolineSelected.SelectedItems[0]); 

            }
        }

        /// <summary>
        /// From user list box of geolines, will add a different feature template to edit session.
        /// Will also add the selected lines to legend table.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_AddGeolines_Click(object sender, EventArgs e)
        {
            try
            {
                //Get current list of ids in legend table
                List<string> currentIDs = GSC_ProjectEditor.Tables.GetFieldValues(tLegendGen,tLegendGenID,null);

                //Update database table with user information
                foreach (string userSelectedLines in this.lbox_GeolineSelected.Items)
                {
                    //Get current item index
                    int currentIndex = this.lbox_GeolineSelected.Items.IndexOf(userSelectedLines);

                    //Get item query from control list tag
                    List<Tuple<string, string, string>> tagGeolines = this.lbox_GeolineSelected.Tag as List<Tuple<string, string, string>>;
                    string selectedLine = tagGeolines[currentIndex].Item1;

                    //Insert new line in legend table, if needed
                    if (!currentIDs.Contains(selectedLine))
	                {
                        Dictionary<string, object> newRowValue = new Dictionary<string,object>();
                        newRowValue[tLegendGenID] = selectedLine;
                        newRowValue[tLegendGenName] = tagGeolines[currentIndex].Item2;
                        newRowValue[tLegendGenSym] = tagGeolines[currentIndex].Item3;
                        newRowValue[tLegendGenType] = lineSymbolType;
                        newRowValue[tLegendSymTheme] = GSC_ProjectEditor.Constants.DatabaseDomainsValues.legendItemGeoline;
                        GSC_ProjectEditor.Tables.AddRowWithValues(tLegendGen, newRowValue);
	                } 
                    
                }

                //Create and or update template
                Utilities.ProjectSymbols uLineSymbols = new Utilities.ProjectSymbols();
                uLineSymbols.CreateLineTemplate(ArcMap.Application.Document as IMxDocument);

            }
            catch(Exception btn_AddGeolines_Click_Error)
            {
                MessageBox.Show("btn_AddGeolines_Click_Error: " + btn_AddGeolines_Click_Error.Message);
            }


        }



    }


}
