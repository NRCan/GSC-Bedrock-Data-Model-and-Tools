using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Linq;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Editor;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Display;

namespace GSC_ProjectEditor
{
    /// <summary>
    /// Designer class of the dockable window add-in. It contains user interfaces that
    /// make up the dockable window.
    /// </summary>
    public partial class DockableWindow_CreateEdit_GeopointTemplate : UserControl
    {
        #region Main Variables

        //Symbol tables
        private const string tGeopoint = GSC_ProjectEditor.Constants.Database.TGeopointSymbol;
        private const string tGeopointID = GSC_ProjectEditor.Constants.DatabaseFields.TGeopointID;
        private const string tGeopointSelectCode = GSC_ProjectEditor.Constants.DatabaseFields.TGeopointSelectCode;
        private const string tGeopointLegendDesc = GSC_ProjectEditor.Constants.DatabaseFields.TGeopointLegendDesc;
        private const string tGeopointFGDC = GSC_ProjectEditor.Constants.DatabaseFields.TGeopointFGDC;

        //FEATURE GEO_POINT
        private const string geopoints = GSC_ProjectEditor.Constants.Database.FGeopoint;
        private const string geopointID = GSC_ProjectEditor.Constants.DatabaseFields.FGeopointID;
        private const string geopointType = GSC_ProjectEditor.Constants.DatabaseFields.FGeopointType;
        private const string geopointSubset = GSC_ProjectEditor.Constants.DatabaseFields.FGeopointSubset;
        private const string geopointStrucAtt = GSC_ProjectEditor.Constants.DatabaseFields.FGeopointStrucAtt;
        private const string geopointStrucGene = GSC_ProjectEditor.Constants.DatabaseFields.FGeopointStrucGene;
        private const string geopointStrucYoung = GSC_ProjectEditor.Constants.DatabaseFields.FGeopointStrucYoung;
        private const string geopointStrucMethod = GSC_ProjectEditor.Constants.DatabaseFields.FGeopointStrucMethod;
        private const string geopointFGDC = GSC_ProjectEditor.Constants.DatabaseFields.FGeopointFGDC;
        private const string geopointAzim = GSC_ProjectEditor.Constants.DatabaseFields.FGeopointAzimuth;
        private const string geopointLayerName = GSC_ProjectEditor.Constants.Layers.geopoint;

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
        private const string pointSymbolType = GSC_ProjectEditor.Constants.DatabaseDomainsValues.SymTypePoint;

        //Other
        //private const string interGroupLayer = GSC_ProjectEditor.Constants.Layers.GroupInterpretation;
        private string dicoMain = "Main";
        Dictionary<string, string> DicoYesNo = null;
        //private string stylePath = null;//Utilities.globalVariables.Styles.MainStylePath;
        //private List<string> itemQueries = new List<string>(); //A list to contains all geoline specific query to add to selected geoline list box.
        private List<Tuple<string, string, string>> itemInformation = new List<Tuple<string, string, string>>(); //A list to contain geolineID description and symbol code

        #endregion

        public DockableWindow_CreateEdit_GeopointTemplate(object hook)
        {
            //Set culture before init.
            Utilities.Culture.SetCulture();
            
            InitializeComponent();
            this.Hook = hook;



            //if enabled is already open before init.
            if (this.Enabled)
            {
                DockableWindowAddSymbolPoint_EnabledChanged(this, null);
            }
            else if (!this.Enabled)
            {
                //Init controls if enabled.
                this.EnabledChanged += new EventHandler(DockableWindowAddSymbolPoint_EnabledChanged);
            }
        }


        /// <summary>
        /// Whenever dock window is enabled
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void DockableWindowAddSymbolPoint_EnabledChanged(object sender, EventArgs e)
        {
            try
            {

                #region dock window startup and control fill

                //Get a description dictionnary from Yes No Domain
                DicoYesNo = GSC_ProjectEditor.Domains.GetDomDico(domYesNo, "Description");

                //Fill in geopoint type combobox
                SortedDictionary<string, int> geopointTypeUniqueValues = GSC_ProjectEditor.Subtypes.GetSubtypeDico(geopoints);
                List<string> subNames = new List<string>();
                List<int> subCodes = new List<int>();

                foreach (KeyValuePair<string, int> subPair in geopointTypeUniqueValues)
                {
                    subNames.Add(subPair.Key);
                    subCodes.Add(subPair.Value);
                }

                this.cbox_GeopointType.DataSource = subNames;
                this.cbox_GeopointType.Tag = geopointTypeUniqueValues; //Add whole dictionnary to later access subtype codes
                this.cbox_GeopointType.SelectedIndex = -1;

                this.cbox_GeopointType.SelectedIndexChanged -= cbox_GeopointType_SelectedIndexChanged;
                this.cbox_GeopointType.SelectedIndexChanged += cbox_GeopointType_SelectedIndexChanged;

                //Init event handlers 
                Utilities.ProjectSymbols refreshPointEvent = new Utilities.ProjectSymbols();
                refreshPointEvent.refreshPointHasStarted += new Utilities.ProjectSymbols.refreshPointStyleEventHandler(refreshPointEvent_refreshPointHasStarted);
                refreshPointEvent.refreshPointHasEnded += new Utilities.ProjectSymbols.refreshPointStyleEventHandler(refreshPointEvent_refreshPointHasEnded);

                #endregion
            }
            catch (Exception enabledChanged)
            {
                MessageBox.Show(enabledChanged.Message);
            }
        }

        void refreshPointEvent_refreshPointHasEnded()
        {
            this.Cursor = Cursors.Default;
        }

        void refreshPointEvent_refreshPointHasStarted()
        {
            this.Cursor = Cursors.WaitCursor;
        }

        /// <summary>
        /// Whenever geopoint type combobox index changes, fill in other comboboxes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void cbox_GeopointType_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Fill in geoline qualifier combobox
            if (this.cbox_GeopointType.SelectedIndex != -1)
            {

                //Get selected subtype code
                string selectedSubtypeName = this.cbox_GeopointType.SelectedItem.ToString();
                SortedDictionary<string, int> selectedSubtypeDico = this.cbox_GeopointType.Tag as SortedDictionary<string, int>;
                int selectedSubtypeCode = selectedSubtypeDico[selectedSubtypeName];

                //Fill in subset combobox
                fill_SubsetCombobox(selectedSubtypeCode);

                //Fill in confidence combobox
                fill_AttitudeCombobox(selectedSubtypeCode);

                //Fill in attitude combobox
                fill_GenerationCombobox(selectedSubtypeCode);

                //Fill in generation combobox
                fill_YoungingCombobox(selectedSubtypeCode);

                //Fill in generation combobox
                fill_MethodCombobox(selectedSubtypeCode);

            }
        }

        /// <summary>
        /// Will fill the method combobox based on selected geopoint type
        /// </summary>
        /// <param name="methodCode"></param>
        private void fill_MethodCombobox(int methodCode)
        {
            //Get associated subset domain name
            string methodDomName = GSC_ProjectEditor.Domains.GetSubDomName(geopoints, methodCode, geopointStrucMethod);

            //Get a list of domain values
            Dictionary<string, string> methodDomDico = GSC_ProjectEditor.Domains.GetDomDico(methodDomName, "Code");
            List<string> methodDomValues = methodDomDico.Values.ToList();

            //Fill in combobox
            this.cbox_GeopointMethod.DataSource = methodDomValues;

            //Get default value
            string defaultValue = GSC_ProjectEditor.Subtypes.GetSubtypeFieldDefault(geopoints, geopointStrucMethod, methodCode);

            //Set default value into cbox
            if (defaultValue != "")
            {
                //Get default value index within list and combobox
                string textValue = methodDomDico[defaultValue];
                int textIndex = methodDomValues.IndexOf(textValue);
                this.cbox_GeopointMethod.SelectedIndex = textIndex;
            }

            //Add a dictionnary into tag
            this.cbox_GeopointMethod.Tag = methodDomDico;
        }

        /// <summary>
        /// Will fill the younging combobox based on selected geopoint type
        /// </summary>
        /// <param name="youngCode"></param>
        private void fill_YoungingCombobox(int youngCode)
        {
            //Get associated subset domain name
            string youngDomName = GSC_ProjectEditor.Domains.GetSubDomName(geopoints, youngCode, geopointStrucYoung);

            //Get a list of domain values
            Dictionary<string,string> youngDomDico = GSC_ProjectEditor.Domains.GetDomDico(youngDomName, "Code");
            List<string> youngDomValues = youngDomDico.Values.ToList();

            //Fill in combobox
            this.cbox_GeopointYounging.DataSource = youngDomValues;

            //Get default value
            string defaultValue = GSC_ProjectEditor.Subtypes.GetSubtypeFieldDefault(geopoints, geopointStrucYoung, youngCode);

            //Set default value into cbox
            if (defaultValue != "")
            {
                //Get default value index within list and combobox
                string textValue = youngDomDico[defaultValue];
                int textIndex = youngDomValues.IndexOf(textValue);
                this.cbox_GeopointYounging.SelectedIndex = textIndex;
            }

            //Add a dictionnary into tag
            this.cbox_GeopointYounging.Tag = youngDomDico;
        }

        /// <summary>
        /// Will fill the generation combobox based on selected geopoint type
        /// </summary>
        /// <param name="genCode"></param>
        private void fill_GenerationCombobox(int genCode)
        {
            //Get associated subset domain name
            string genDomName = GSC_ProjectEditor.Domains.GetSubDomName(geopoints, genCode, geopointStrucGene);

            //Get a list of domain values
            Dictionary<string, string> geneDomDico = GSC_ProjectEditor.Domains.GetDomDico(genDomName, "Code");
            List<string> geneDomValues = geneDomDico.Values.ToList();

            //Fill in combobox
            this.cbox_GeopointGeneration.DataSource = geneDomValues;

            //Get default value
            string defaultValue = GSC_ProjectEditor.Subtypes.GetSubtypeFieldDefault(geopoints, geopointStrucGene, genCode);

            //Set default value into cbox
            if (defaultValue != "")
            {
                //Get default value index within list and combobox
                string textValue = geneDomDico[defaultValue];
                int textIndex = geneDomValues.IndexOf(textValue);
                this.cbox_GeopointGeneration.SelectedIndex = textIndex;
            }

            //Add a dictionnary into tag
            this.cbox_GeopointGeneration.Tag = geneDomDico;
        }

        /// <summary>
        /// Will fill the attitude combobox based on selected geopoint type
        /// </summary>
        /// <param name="attitudeCode"></param>
        private void fill_AttitudeCombobox(int attitudeCode)
        {
            //Get associated subset domain name
            string attDomName = GSC_ProjectEditor.Domains.GetSubDomName(geopoints, attitudeCode, geopointStrucAtt);

            //Get a list of domain values
            Dictionary<string, string> attDomDico = GSC_ProjectEditor.Domains.GetDomDico(attDomName, "Code");
            List<string> attDomValues = attDomDico.Values.ToList();

            //Fill in combobox
            this.cbox_GeopointAttitude.DataSource = attDomValues;

            //Get default value
            string defaultValue = GSC_ProjectEditor.Subtypes.GetSubtypeFieldDefault(geopoints, geopointStrucAtt, attitudeCode);

            //Set default value into cbox
            if (defaultValue != "")
            {
                //Get default value index within list and combobox
                string textValue = attDomDico[defaultValue];
                int textIndex = attDomValues.IndexOf(textValue);
                this.cbox_GeopointAttitude.SelectedIndex = textIndex;
            }

            //Add a dictionnary into tag
            this.cbox_GeopointAttitude.Tag = attDomDico;
        }

        /// <summary>
        /// Will fill the subset combobox based on selectec geopoint type
        /// </summary>
        /// <param name="selectedSubtypeCode"></param>
        public void fill_SubsetCombobox(int selectedSubtypeCode)
        {
            //Get associated subset domain name
            string subsetDomName = GSC_ProjectEditor.Domains.GetSubDomName(geopoints, selectedSubtypeCode, geopointSubset);

            //Get a list of domain values
            Dictionary<string, string> subsetDomDico = GSC_ProjectEditor.Domains.GetDomDico(subsetDomName, "Code");
            List<string> subsetDomValues = subsetDomDico.Values.ToList();

            //Fill in combobox
            this.cbox_GeopointSubset.DataSource = subsetDomValues;

            //Get default value
            string defaultValue = GSC_ProjectEditor.Subtypes.GetSubtypeFieldDefault(geopoints, geopointSubset, selectedSubtypeCode);

            //Set default value into cbox
            if (defaultValue != "")
            {
                //Get default value index within list and combobox
                string textValue = subsetDomDico[defaultValue];
                int textIndex = subsetDomValues.IndexOf(textValue);
                this.cbox_GeopointSubset.SelectedIndex = textIndex;
            }

            //Add a dictionnary into tag
            this.cbox_GeopointSubset.Tag = subsetDomDico;
        }

        #region Addin init.
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
            public DockableWindow_CreateEdit_GeopointTemplate m_windowUI;

            public AddinImpl()
            {
            }

            protected override IntPtr OnCreateChild()
            {
                m_windowUI = new DockableWindow_CreateEdit_GeopointTemplate(this.Hook);
                return m_windowUI.Handle;
            }

            protected override void Dispose(bool disposing)
            {
                if (m_windowUI != null)
                    m_windowUI.Dispose(disposing);

                base.Dispose(disposing);
            }

        }

        #endregion

        /// <summary>
        /// When clicked, selected information within comboboxes will trigger a selection within table to get GEOPOINTID and add legend description in list box.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_AddGeopointList_Click(object sender, EventArgs e)
        {
            //Get all cbox values
            string valueType = this.cbox_GeopointType.SelectedItem.ToString();
            string valueSubset = this.cbox_GeopointSubset.SelectedItem.ToString();
            string valueAtt = this.cbox_GeopointAttitude.SelectedItem.ToString();
            string valueGen = this.cbox_GeopointGeneration.SelectedItem.ToString();
            string valueYoung = this.cbox_GeopointYounging.SelectedItem.ToString();
            string valueMethod = this.cbox_GeopointMethod.SelectedItem.ToString();

            if (this.cbox_GeopointType.SelectedItem == null || this.cbox_GeopointSubset.SelectedItem == null || this.cbox_GeopointAttitude.SelectedItem == null || this.cbox_GeopointGeneration.SelectedItem == null || this.cbox_GeopointYounging.SelectedItem == null || this.cbox_GeopointMethod.SelectedItem == null)
            {
                MessageBox.Show(Properties.Resources.Error_AddGeoline);
            }

            else
            {
                //Build a geopointID from selected values in comboboxes
                SortedDictionary<string, int> geopointTypeDico = this.cbox_GeopointType.Tag as SortedDictionary<string, int>;
                Dictionary<string, string> geopointSubDico = this.cbox_GeopointSubset.Tag as Dictionary<string, string>;
                Dictionary<string, string> geopointAttDico = this.cbox_GeopointAttitude.Tag as Dictionary<string, string>;
                Dictionary<string, string> geopointGenDico = this.cbox_GeopointGeneration.Tag as Dictionary<string, string>;
                Dictionary<string, string> geopointYoungDico = this.cbox_GeopointYounging.Tag as Dictionary<string, string>;
                Dictionary<string, string> geopointMethodDico = this.cbox_GeopointMethod.Tag as Dictionary<string, string>;

                //Reverse dictionnary key and values
                geopointSubDico = geopointSubDico.ToDictionary(kv => kv.Value, kv => kv.Key);
                geopointAttDico = geopointAttDico.ToDictionary(kv => kv.Value, kv => kv.Key);
                geopointGenDico = geopointGenDico.ToDictionary(kv => kv.Value, kv => kv.Key);
                geopointYoungDico = geopointYoungDico.ToDictionary(kv => kv.Value, kv => kv.Key);
                geopointMethodDico = geopointMethodDico.ToDictionary(kv => kv.Value, kv => kv.Key);

                string geopointTypeCode = geopointTypeDico[valueType].ToString();
                string geopointSubCode = geopointSubDico[valueSubset];
                string geopointAttCode = geopointAttDico[valueAtt];
                string geopointGenCode = geopointGenDico[valueGen];
                string geopointYoungCode = geopointYoungDico[valueYoung];
                string geopointMethodCode = geopointMethodDico[valueMethod];
                string newItemGeopointID = geopointTypeCode + geopointSubCode + geopointAttCode + geopointGenCode + geopointYoungCode + geopointMethodCode;

                //Build a new sql query for list tag object
                string newItemQuery = geopointID + " = '" + newItemGeopointID + "'";

                //Compile all info from cbbox and add legend description to list
                try
                {
                    List<Tuple<string, string, string>> geopointGenUniqueValues = GSC_ProjectEditor.Tables.GetUniqueTripleFieldValues(tGeopoint, new Tuple<string, string, string>(tGeopointID, tGeopointLegendDesc, tGeopointFGDC), newItemQuery);
                    this.lbox_GeopointSelected.Items.Add(geopointGenUniqueValues[0].Item2.ToString());

                    //Add new query to list and to tag of control
                    itemInformation.Add(geopointGenUniqueValues[0]);
                    this.lbox_GeopointSelected.Tag = itemInformation;
                }
                catch
                {
                    //MessageBox.Show(newItemQuery);
                    MessageBox.Show(Properties.Resources.Error_InvalidGeopointIDQuery);
                }

            }
        }

        /// <summary>
        /// Clears a list box selection from the list box itself.
        /// </summary>
        /// <param name="sencder"></param>
        /// <param name="e"></param>
        private void btn_ClearSelectedGeopoints_Click(object sender, EventArgs e)
        {
            while (this.lbox_GeopointSelected.SelectedItems.Count > 0)
            {

                //Remove from tag list, this step is important or else it unsync with current items in list box
                List<Tuple<string, string, string>> newTag = this.lbox_GeopointSelected.Tag as List<Tuple<string, string, string>>;
                newTag.RemoveAt(this.lbox_GeopointSelected.SelectedIndex);
                this.lbox_GeopointSelected.Tag = newTag;
                itemInformation = newTag;

                //Remove from list
                this.lbox_GeopointSelected.Items.Remove(this.lbox_GeopointSelected.SelectedItems[0]);


            }
        }

        /// <summary>
        /// From user list box of geopoints, will add a different feature template to edit session.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_AddGeopoints_Click(object sender, EventArgs e)
        {
            try
            {
                //Get current list of ids in legend table
                List<string> currentIDs = GSC_ProjectEditor.Tables.GetFieldValues(tLegendGen, tLegendGenID, null);

                //Update database table with user information
                foreach (string userSelectedPoint in this.lbox_GeopointSelected.Items)
                {
                    //Get current item index
                    int currentIndex = this.lbox_GeopointSelected.Items.IndexOf(userSelectedPoint);

                    //Get item query from control list tag
                    List<Tuple<string, string, string>> tagGeolines = this.lbox_GeopointSelected.Tag as List<Tuple<string, string, string>>;
                    string selectedPoint = tagGeolines[currentIndex].Item1;

                    //Insert new line in legend table, if needed
                    if (!currentIDs.Contains(selectedPoint))
                    {
                        Dictionary<string, object> newRowValue = new Dictionary<string, object>();
                        newRowValue[tLegendGenID] = selectedPoint;
                        newRowValue[tLegendGenName] = tagGeolines[currentIndex].Item2;
                        newRowValue[tLegendGenSym] = tagGeolines[currentIndex].Item3;
                        newRowValue[tLegendGenType] = pointSymbolType;
                        newRowValue[tLegendSymTheme] = GSC_ProjectEditor.Constants.DatabaseDomainsValues.legendItemGeopoint;
                        GSC_ProjectEditor.Tables.AddRowWithValues(tLegendGen, newRowValue);
                    } 

                }

                //Create and or update template
                Utilities.ProjectSymbols uSymbolPoint = new Utilities.ProjectSymbols();
                uSymbolPoint.CreatePointTemplate(ArcMap.Application.Document as IMxDocument);

            }
            catch (Exception btn_AddGeolines_Click_Error)
            {
                MessageBox.Show("btn_AddGeopoints_Click_Error: " + btn_AddGeolines_Click_Error.Message);
            }
        }

    }
}
