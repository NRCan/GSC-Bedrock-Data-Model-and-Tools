using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Editor;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.DisplayUI;

namespace GSC_ProjectEditor
{
    /// <summary>
    /// Designer class of the dockable window add-in. It contains user interfaces that
    /// make up the dockable window.
    /// </summary>
    public partial class Dockablewindow_CreateEdit_LabelTemplate : UserControl
    {
        #region Main Variables

        //P_LABELS
        private const string plabels = GSC_ProjectEditor.Constants.Database.FLabel;
        private const string plabelField = GSC_ProjectEditor.Constants.DatabaseFields.FLabelID;
        private const string plabelFieldAlias = GSC_ProjectEditor.Constants.DatabaseFields.FLabelIDAlias;
        private const string plabelLayerName = GSC_ProjectEditor.Constants.Layers.label;

        //DOMAINS
        private const string muPID = GSC_ProjectEditor.Constants.DatabaseDomains.MapUnit;
        private const string agePrefixPID = GSC_ProjectEditor.Constants.DatabaseDomains.ageDesignator;

        //Legend generator table and fields
        private const string lTable = GSC_ProjectEditor.Constants.Database.TLegendGene;
        private const string lSymbol = GSC_ProjectEditor.Constants.DatabaseFields.LegendSymbol;
        private const string lLabel = GSC_ProjectEditor.Constants.DatabaseFields.LegendLabelID;
        private const string lSymType = GSC_ProjectEditor.Constants.DatabaseFields.LegendSymType;
        private const string lSymTypeFill = GSC_ProjectEditor.Constants.DatabaseDomainsValues.SymTypeFill;
        private const string lSymName = GSC_ProjectEditor.Constants.DatabaseFields.LegendGISDisplay;
        private const string lSymMU = GSC_ProjectEditor.Constants.DatabaseFields.LegendMapUnit;
        private const string lSymTheme = GSC_ProjectEditor.Constants.DatabaseFields.LegendItemType;

        //Other
        private Dictionary<string, object> inFieldValues = new Dictionary<string, object>(); //Will be used to add info in legend generator table
        private const string overprintKeyWord = GSC_ProjectEditor.Constants.ValueKeywords.labelOverprint;

        //Delegates and events
        public delegate void newLabelEventHandler(object sender, EventArgs e); //A delegate for execution events
        public event newLabelEventHandler newLabelAdded; //This event is triggered when a new label has been added within database

        #endregion

        public Dockablewindow_CreateEdit_LabelTemplate(object hook)
        {
            //Set culture before init.
            Utilities.Culture.SetCulture();

            InitializeComponent();
            this.Hook = hook;

            //Manage person list, if enabled is already open before init.
            if (this.Enabled)
            {
                dockablewindowAddLabel_EnabledChanged(this, null);
            }
            else if (!this.Enabled)
            {
                //Init controls if enabled.
                this.EnabledChanged += new EventHandler(dockablewindowAddLabel_EnabledChanged);
            }

        }

        /// <summary>
        /// Whenever the form gets enabled.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        public void dockablewindowAddLabel_EnabledChanged(object sender, EventArgs e)
        {
            //Fill some comboboxes
            fillAgePrefixCombobox();
        }

        /// <summary>
        /// Will fill the age prefix combobox
        /// </summary>
        public void fillAgePrefixCombobox()
        {
            //Get a sorted dico of domain values
            if (GSC_ProjectEditor.Domains.IsDomExisting(agePrefixPID))
            {
                Dictionary<string, string> agePrefixUnsortedDico = GSC_ProjectEditor.Domains.GetDomDico(agePrefixPID, "Description");
                SortedDictionary<string, string> agePrefixSortedDico = new SortedDictionary<string, string>();
                foreach (KeyValuePair<string, string> kvUnsorted in agePrefixUnsortedDico)
                {
                    agePrefixSortedDico[kvUnsorted.Key] = kvUnsorted.Value;
                }

                //Get a list of age prefix for combobox
                List<AgePrefix> ageList = new List<AgePrefix>();
                ageList.Add(new AgePrefix { ageDisplay = string.Empty, ageValue = string.Empty }); //Some blank
                foreach (KeyValuePair<string, string> agePrefixes in agePrefixSortedDico)
                {
                    ageList.Add(new AgePrefix { ageDisplay = agePrefixes.Key, ageValue = agePrefixes.Value });
                }

                this.comboBox_AgePrefix.DataSource = ageList;
                this.comboBox_AgePrefix.DisplayMember = "ageDisplay";
                this.comboBox_AgePrefix.ValueMember = "ageValue";
            }
            else
            {
                this.comboBox_AgePrefix.Items.Add("Missing Age Designator Domain in Database");
            }

        }

        public class AgePrefix
        {
            public string ageDisplay { get; set; }
            public string ageValue { get; set; }
        }

        public int OverprintLevel { get; set; }

        #region Instantiate dock window.
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
            public Dockablewindow_CreateEdit_LabelTemplate m_windowUI;

            public AddinImpl()
            {
            }

            protected override IntPtr OnCreateChild()
            {
                m_windowUI = new Dockablewindow_CreateEdit_LabelTemplate(this.Hook);
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
        /// Show  all possible color codes from an html file //TEMP
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pct_MapUnitColor_Click(object sender, EventArgs e)
        {

            //Access style gallery storage
            string pathToStyle = GSC_ProjectEditor.Symbols.ValidateStyleFile2(ArcMap.Document);
            if (pathToStyle != string.Empty)
            {
                IStyleGalleryStorage styleGal;
                styleGal = ArcMap.Document.StyleGallery as IStyleGalleryStorage;

                //Remove all styles except geoline one
                List<string> userCurrentStyle = GSC_ProjectEditor.Symbols.RemoveAllStylesExceptGiven(styleGal, pathToStyle);

                //Parse wanted type of symbols
                ISimpleFillSymbol newFillSymbol = new SimpleFillSymbol();
                ISymbol wantedSymbolType = newFillSymbol as ISymbol;

                //Create a symbol selector 
                ISymbolSelector newDialog = new SymbolSelector();

                //Call the dialog with chosen symbol as parameter
                if (newDialog.AddSymbol(wantedSymbolType))
                {
                    //If user has selected a symbol
                    if (newDialog.SelectSymbol(0))
                    {

                        //Retreive user choice
                        ISymbol userSymbol = newDialog.GetSymbolAt(0);

                        //Find correct line symbol type
                        string symbolTypeName = "";
                        object correctLineSymbol = GSC_ProjectEditor.Symbols.GetPolygonSymbolType(userSymbol, out symbolTypeName);

                        if (correctLineSymbol != null)
                        {
                            this.txtbox_LabelSymbol.Text = Utilities.MapDocumentSymbol.GetMatchingPolygonCodeFromSymbol(correctLineSymbol, symbolTypeName, pathToStyle);
                        }

                    }
                }

                //Add back all the styles from user into the style gallery storage
                GSC_ProjectEditor.Symbols.AddStylesToStorage(styleGal, userCurrentStyle); 
            }



        }

        /// <summary>
        /// When button is clicked, add a map unit to domain and create feature template in edit session
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_AddLabel_Click(object sender, EventArgs e)
        {

            //Parse empty textbox that are needed
            if (this.txtbox_LabelDescription.Text != "")
            {
                #region Process new label name with overprints

                string labelDescText = this.txtbox_LabelDescription.Text;

                if (this.checkBox_Overprint.Checked)
                {
                    labelDescText = labelDescText + overprintKeyWord;

                    //Process overprint level
                    if (OverprintLevel > 1)
                    {
                        labelDescText = labelDescText + OverprintLevel.ToString();
                    }
                }


                #endregion


                #region Build a dictionnary of user values, related to fields within legend table

                //Empty dico
                inFieldValues.Clear();

                //Manage symbol color
                if (this.txtbox_LabelSymbol.Text == "")
                {
                    //User hasn't entered any value, push default
                    inFieldValues[lSymbol] = GSC_ProjectEditor.Constants.Symbol4Layers.mapUnitDefaultColor;
                }
                else
                {
                    //User has entered a value
                    inFieldValues[lSymbol] = this.txtbox_LabelSymbol.Text;
                }

               
                //Calculate ids for description and legend items
                Dictionary<string, string> dicoDom = GSC_ProjectEditor.Domains.GetDomDico(muPID, "Code");
                int newLabelID = GSC_ProjectEditor.IDs.CalculateIDFromCount(lTable, null); 

                while (dicoDom.ContainsKey(newLabelID.ToString()))
                {
                    newLabelID = newLabelID + 10;
                }
                

                //Add new ids to dictionnaries
                inFieldValues[lLabel] = newLabelID;
                inFieldValues[lSymType] = lSymTypeFill;
                inFieldValues[lSymName] = this.txtbox_LabelDescription.Text;
                inFieldValues[lSymMU] = this.txtbox_LabelDescription.Text;
                inFieldValues[lSymTheme] = GSC_ProjectEditor.Constants.DatabaseDomainsValues.legendItemMapUnit;

                #endregion

                //Add new row within table
                GSC_ProjectEditor.Tables.AddRowWithValues(lTable, inFieldValues);

                //Add values to domain
                try
                {
                    GSC_ProjectEditor.Domains.AddDomainValue(muPID, newLabelID.ToString(), labelDescText); //TEMP

                }
                catch
                {
                    MessageBox.Show("Dom prob.");
                }

                //Start event 
                try
                {
                    newLabelAdded(labelDescText, e);

                }
                catch
                {

                }

                //Create template for annotation
                Utilities.ProjectSymbols uStyles = new Utilities.ProjectSymbols();
                uStyles.CreateLabelTemplate(ArcMap.Application.Document as IMxDocument);

                //Clear all values from interface
                clearBoxes();


                
            }
            else
            {
                MessageBox.Show(Properties.Resources.Error_MapUnit);
            }

        }

        /// <summary>
        /// Use to clear all values from dw controls.
        /// </summary>
        private void clearBoxes()
        {
            //Clear textboxes
            this.txtbox_LabelDescription.Text = "";
            this.txtbox_LabelSymbol.Text = "";

            //Uncheck checkboxes
            this.checkBox_Overprint.Checked = false;

        }

        /// <summary>
        /// Whenever the selection changes, add the value to the textbox as a prefix
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBox_AgePrefix_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Get current selection
            if (this.comboBox_AgePrefix.SelectedIndex != -1)
            {

                //Get selected age value
                AgePrefix selectedAgePrefix = this.comboBox_AgePrefix.SelectedItem as AgePrefix;
                
                //Add to textbox
                this.txtbox_LabelDescription.Text = selectedAgePrefix.ageValue + this.txtbox_LabelDescription.Text;

            }
        }

        /// <summary>
        /// Whenever the overprint is checked get a level of overprint from user
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBox_Overprint_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkBox_Overprint.Checked)
            {
                Form_CreateEdit_CreateMapUnits_OverprintHierarchy newOverprintForm = new Form_CreateEdit_CreateMapUnits_OverprintHierarchy();

                //Show the form
                using (newOverprintForm)
                {
                    var formOutput = newOverprintForm.ShowDialog();
                    if (formOutput == DialogResult.OK)
                    {
                        OverprintLevel = newOverprintForm.returnLevel;
                    }

                }
            }
        }

    }
}
