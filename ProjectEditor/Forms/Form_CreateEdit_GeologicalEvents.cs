using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ESRI.ArcGIS.Geodatabase;

namespace GSC_ProjectEditor
{
    public partial class Form_CreateEdit_GeologicalEvents : Form
    {
        #region Main Variables

        //Geo event table
        public const string eventTable = GSC_ProjectEditor.Constants.Database.TGeoEvent;
        public const string eventTableID = GSC_ProjectEditor.Constants.DatabaseFields.TGeoEventID;
        public const string eventTableName = GSC_ProjectEditor.Constants.DatabaseFields.TGeoEventName;


        //Legend generator table
        public const string legendTable = GSC_ProjectEditor.Constants.Database.TLegendGene;
        public const string legendID = GSC_ProjectEditor.Constants.DatabaseFields.LegendLabelID;
        public const string legendDisplayGIS = GSC_ProjectEditor.Constants.DatabaseFields.LegendGISDisplay;
        public const string legendSymbolType = GSC_ProjectEditor.Constants.DatabaseFields.LegendSymType;
        public const string legendSymbolTypeFill = GSC_ProjectEditor.Constants.DatabaseDomainsValues.SymTypeFill;
        public const string legendSymbolTypeLine = GSC_ProjectEditor.Constants.DatabaseDomainsValues.SymTypeLine;

        //Feature classes
        public const string labelFC = GSC_ProjectEditor.Constants.Database.FLabel;
        public const string labelFCID = GSC_ProjectEditor.Constants.DatabaseFields.FLabelID;
        public const string geolineFC = GSC_ProjectEditor.Constants.Database.FGeoline;
        public const string geolineFCID = GSC_ProjectEditor.Constants.DatabaseFields.FGeolineID;

        //Feature layers
        public const string labelFL = GSC_ProjectEditor.Constants.Layers.label;
        public const string geolineFL = GSC_ProjectEditor.Constants.Layers.geoline;

        //Other
        public Random idRandom { get; set; }

        #endregion

        #region INIT
        public Form_CreateEdit_GeologicalEvents()
        {
            //Start a random
            idRandom = new Random((int)DateTime.Now.Ticks + 474);

            //Init culture
            Utilities.Culture.SetCulture();

            InitializeComponent();

            //Initiate some custom controls.
            this.Shown += new EventHandler(FormAddGeoEvent_Shown);
        }

        /// <summary>
        /// Whenever the form is shown this will initialize some components of the form.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void FormAddGeoEvent_Shown(object sender, EventArgs e)
        {
            FillEventNameCombobox();
            FillPrefixCombobox();
            FillTimeScaleCombobox();
            InitAgeItemDictionary();
            FillSourceCombobox();
        }

        /// <summary>
        /// Will fill the source combobox
        /// </summary>
        private void FillSourceCombobox()
        {
            //Make sure that nothing is already in the combobox
            this.comboBox_Source.DataSource = null;

            //Get a list of domain code and values
            Dictionary<string, string> sourceDico = GSC_ProjectEditor.Tables.GetUniqueDicoValues(GSC_ProjectEditor.Constants.Database.TSource, 
                GSC_ProjectEditor.Constants.DatabaseFields.TSourceID, 
                GSC_ProjectEditor.Constants.DatabaseFields.TSourceAbbr, null);

            //Build new class for display
            List<SourceDisplay> sourceToDisplay = new List<SourceDisplay>();

            foreach (KeyValuePair<string, string> sources in sourceDico)
            {
                sourceToDisplay.Add(new SourceDisplay { sourceID = sources.Key, sourceName = sources.Value });
            }

            //Set min age timescale
            this.comboBox_Source.DataSource = sourceToDisplay;
            this.comboBox_Source.DisplayMember = "sourceName";
            this.comboBox_Source.ValueMember = "sourceID";
            this.comboBox_Source.SelectedIndex = -1;
        }

        /// <summary>
        /// Will fill the two comboboxes for time scale values
        /// </summary>
        private void FillTimeScaleCombobox()
        {
            //Make sure that nothings is already in the combobox
            this.comboBox_TimescaleMax.DataSource = null;
            this.comboBox_TimescaleMin.DataSource = null;

            //Get a list of domain code and values
            Dictionary<string, string> timeScaleDom = GSC_ProjectEditor.Domains.GetDomDico(GSC_ProjectEditor.Constants.DatabaseDomains.ageDesignator, "Code");
        
            //Build new class for display
            List<TimeScaleDisplay> timeScaleSourceMin = new List<TimeScaleDisplay>();
            List<TimeScaleDisplay> timeScaleSourceMax = new List<TimeScaleDisplay>();
            foreach (KeyValuePair<string,string> timeScales in timeScaleDom)
            {
                timeScaleSourceMin.Add(new TimeScaleDisplay { TimeScaleID = timeScales.Key, TimeScaleName = timeScales.Value });
                timeScaleSourceMax.Add(new TimeScaleDisplay { TimeScaleID = timeScales.Key, TimeScaleName = timeScales.Value });
            }

            //Set min age timescale
            this.comboBox_TimescaleMin.DataSource = timeScaleSourceMin;
            this.comboBox_TimescaleMin.DisplayMember = "TimeScaleName";
            this.comboBox_TimescaleMin.ValueMember = "TimeScaleID";
            this.comboBox_TimescaleMin.SelectedIndex = -1;
        
            //Set max age timescale
            this.comboBox_TimescaleMax.DataSource = timeScaleSourceMax;
            this.comboBox_TimescaleMax.DisplayMember = "TimeScaleName";
            this.comboBox_TimescaleMax.ValueMember = "TimeScaleID";
            this.comboBox_TimescaleMax.SelectedIndex = -1;
        }

        /// <summary>
        /// Will fill the two comboboxes for age prefix names and values
        /// </summary>
        private void FillPrefixCombobox()
        {
            //Make sure that nothings is already in the combobox
            this.comboBox_PrefixMax.DataSource = null;
            this.comboBox_PrefixMin.DataSource = null;

            //Get a list of domain code and values
            Dictionary<string, string> prefixDom = GSC_ProjectEditor.Domains.GetDomDico(GSC_ProjectEditor.Constants.DatabaseDomains.agePrefix, "Code");

            //Build new class for display
            List<PrefixAgeDisplay> prefixSourceMin = new List<PrefixAgeDisplay>();
            List<PrefixAgeDisplay> prefixSourceMax = new List<PrefixAgeDisplay>();
            foreach (KeyValuePair<string, string> prefixes in prefixDom)
            {
                prefixSourceMin.Add(new PrefixAgeDisplay { PrefixAgeID = prefixes.Key, PrefixAgeName = prefixes.Value });
                prefixSourceMax.Add(new PrefixAgeDisplay { PrefixAgeID = prefixes.Key, PrefixAgeName = prefixes.Value });
            }

            //Set min age timescale
            this.comboBox_PrefixMin.DataSource = prefixSourceMin;
            this.comboBox_PrefixMin.DisplayMember = "PrefixAgeName";
            this.comboBox_PrefixMin.ValueMember = "PrefixAgeID";
            this.comboBox_PrefixMin.SelectedIndex = -1;

            //Set max age timescale
            this.comboBox_PrefixMax.DataSource = prefixSourceMax;
            this.comboBox_PrefixMax.DisplayMember = "PrefixAgeName";
            this.comboBox_PrefixMax.ValueMember = "PrefixAgeID";
            this.comboBox_PrefixMax.SelectedIndex = -1;
        }

        /// <summary>
        /// Will fill the combobox of geological event names from table
        /// </summary>
        private void FillEventNameCombobox()
        {
            //Make sure that nothings is already in the combobox
            this.comboBox_Event.DataSource = null;

            //Get a list of domain code and values
            Dictionary<string, string> eventDico = GSC_ProjectEditor.Tables.GetUniqueDicoValues(eventTable,eventTableID, eventTableName, null);

            //Build new class for display
            List<EventNameDisplay> eventToDisplay = new List<EventNameDisplay>();

            foreach (KeyValuePair<string, string> events in eventDico)
            {
                eventToDisplay.Add(new EventNameDisplay { eventID = events.Key, eventName = events.Value });
            }

            //Set min age timescale
            this.comboBox_Event.DataSource = eventToDisplay;
            this.comboBox_Event.DisplayMember = "eventName";
            this.comboBox_Event.ValueMember = "eventID";
            this.comboBox_Event.SelectedIndex = -1;
        }

        /// <summary>
        /// Class to be used to display in form combobox of event names
        /// </summary>
        public class EventNameDisplay
        {
            public string eventName { get; set; }
            public string eventID { get; set; }
        }

        /// <summary>
        /// Class to be used to display in form combobox of event linked legend items.
        /// </summary>
        public class EventLinkedItemsDisplay
        {
            public string linkedItemName { get; set; }
            public string linkedItemID { get; set; }
            public bool onScreenSelection { get; set; }
            public bool isLabel { get; set; } //Either label or geolines

        }

        /// <summary>
        /// Will be used to fill timescale combobox from DB domain
        /// </summary>
        public class TimeScaleDisplay
        {
            public string TimeScaleName { get; set; }
            public string TimeScaleID { get; set; }
            
        }

        /// <summary>
        /// Will be used to show sources
        /// </summary>
        public class SourceDisplay
        {
            public string sourceName { get; set; }
            public string sourceID { get; set; }
        }

        /// <summary>
        /// Will be used to fill age prefix combobox from DB domain
        /// </summary>
        public class PrefixAgeDisplay
        {
            public string PrefixAgeName { get; set; }
            public string PrefixAgeID { get; set; }
        }

        public Dictionary<string, string> AgeItemDictonary { get; set; } //Will be used to keep map unit and geolines ids and display names

        #endregion

        #region VIEW MODEL

        /// <summary>
        /// When clicked, will prompt user with a textbox to ask for a new name and
        /// will append it to the related combobox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_AddEvent_Click(object sender, EventArgs e)
        {
            //Ask user for a new name
            string formTitle = Properties.Resources.Message_EventNameAddTitle;
            string formLabel = Properties.Resources.Message_EventNameAdd;
            string newEventName = GSC_ProjectEditor.Form_Generic.ShowGenericTextboxForm(formTitle, formLabel, this.Icon, string.Empty);

            //Update combobox
            List<EventNameDisplay> currentEventNames = this.comboBox_Event.DataSource as List<EventNameDisplay>;
            currentEventNames.Add(new EventNameDisplay { eventID = string.Empty, eventName = newEventName});
            this.comboBox_Event.DataSource = null; //nullify before resetting to force update
            this.comboBox_Event.DataSource = currentEventNames;
            this.comboBox_Event.DisplayMember = "eventName";
            this.comboBox_Event.ValueMember = "eventID";
            this.comboBox_Event.SelectedIndex = this.comboBox_Event.Items.Count - 1;
        }

        /// <summary>
        /// Will prompt user with a list of map units and geolines from project
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_AddItem_Click(object sender, EventArgs e)
        {
            //Get a list of geolines and map units
            if (AgeItemDictonary==null)
            {
                InitAgeItemDictionary();
            }
            //Build a list of all items names and their values
            List<Tuple<string, object>> itemsToDisplay = new List<Tuple<string,object>>();
            foreach (KeyValuePair<string, string> items in AgeItemDictonary)
	        {
                itemsToDisplay.Add(new Tuple<string, object>(items.Value, items.Key));
	        }

            //Show selection form to user and retrieve selection
            string listBoxTitle = Properties.Resources.Message_AgeItemTitle;
            string listBoxMessage = Properties.Resources.Message_AgeItemMessage;
            List<Tuple<string, object>> selectedItems = GSC_ProjectEditor.Form_Generic.ShowGenericListBoxForm(listBoxTitle, listBoxMessage, this.Icon, itemsToDisplay);
        


            //Add selection to combobox
            this.comboBox_linkedItem.DataSource = null; //Reset
            List<EventLinkedItemsDisplay> cboxItems = new List<EventLinkedItemsDisplay>();
            foreach (Tuple<string, object> selectedItemsFromForm in selectedItems)
            {
                //Get feature class type
                bool itemIsLabel = false;
                if (selectedItemsFromForm.Item2.ToString().Length < 12)
                {
                    itemIsLabel = true;
                }

                cboxItems.Add(new EventLinkedItemsDisplay { linkedItemName = selectedItemsFromForm.Item1, 
                    linkedItemID = selectedItemsFromForm.Item2.ToString(), 
                    onScreenSelection = false, 
                    isLabel = itemIsLabel });
            }
            this.comboBox_linkedItem.DataSource = cboxItems;
            this.comboBox_linkedItem.DisplayMember = "linkedItemName";
            this.comboBox_linkedItem.ValueMember = "linkedItemID";
            if (cboxItems.Count != 0)
            {
                this.comboBox_linkedItem.SelectedIndex = 0;
            }
            
        }
        
        /// <summary>
        /// From a selected item in the combobox event, a user will be asked for a new name
        /// that then will be updated in the combobox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_RenameEvent_Click(object sender, EventArgs e)
        {
            //Get current item
            EventNameDisplay currentSelectedItem = this.comboBox_Event.SelectedItem as EventNameDisplay;
            int currentSelectedIndex = this.comboBox_Event.SelectedIndex;

            //Ask user for a new name
            string formTitle = Properties.Resources.Message_EventNameAddTitle;
            string formLabel = Properties.Resources.Message_EventNameAdd;
            string newEventName = GSC_ProjectEditor.Form_Generic.ShowGenericTextboxForm(formTitle, formLabel, this.Icon, currentSelectedItem.eventName);

            //Update combobox
            List<EventNameDisplay> currentEventNames = this.comboBox_Event.DataSource as List<EventNameDisplay>;
            int currentListIndex = currentEventNames.IndexOf(currentSelectedItem);
            EventNameDisplay modifiedEventName = currentEventNames[currentListIndex];
            modifiedEventName.eventName = newEventName;

            this.comboBox_Event.DataSource = null; //nullify before resetting to force update
            this.comboBox_Event.DataSource = currentEventNames;
            this.comboBox_Event.DisplayMember = "eventName";
            this.comboBox_Event.ValueMember = "eventID";
            this.comboBox_Event.SelectedIndex = currentSelectedIndex;
        }

        /// <summary>
        /// When the button is clicked, the form will close.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_Cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Will update the database with user's values from UI
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_AddModify_Click(object sender, EventArgs e)
        {
            if (this.comboBox_Event.SelectedIndex != -1)
	        {
                #region Variables

                bool isUpdate = false;

                #endregion

		        #region Gather all information from UI
                Dictionary<string, object> GeoEventValues = new Dictionary<string,object>();
                //Num values
                GeoEventValues[GSC_ProjectEditor.Constants.DatabaseFields.TGeoEventAgeMinValue] = this.numericUpDown_ValueMin.Value;
                GeoEventValues[GSC_ProjectEditor.Constants.DatabaseFields.TGeoEventAgeMaxValue] = this.numericUpDown_ValueMax.Value;
                GeoEventValues[GSC_ProjectEditor.Constants.DatabaseFields.TGeoEventAgeMinCertainty] = this.numericUpDown_CertaintyMin.Value;
                GeoEventValues[GSC_ProjectEditor.Constants.DatabaseFields.TGeoEventAgeMaxCertainty] = this.numericUpDown_CertaintyMax.Value;
            
                //Required
                EventNameDisplay currentEvent = this.comboBox_Event.SelectedItem as EventNameDisplay;
                GeoEventValues[eventTableName] = currentEvent.eventName;
                
                if (currentEvent.eventID == string.Empty)
	            {
		            //Get a new id
                    GeoEventValues[eventTableID] = GSC_ProjectEditor.IDs.GetRandomInt(idRandom);
	            }
                else
                {
                    GeoEventValues[eventTableID] = currentEvent.eventID;
                    isUpdate = true;
                }

                //Optionals
                if (this.comboBox_PrefixMin.SelectedIndex != -1)
                {
                    //Cast
                    PrefixAgeDisplay currentMinPrefix = this.comboBox_PrefixMin.SelectedItem as PrefixAgeDisplay;
                    GeoEventValues[GSC_ProjectEditor.Constants.DatabaseFields.TGeoEventAgeMinPrefix] = currentMinPrefix.PrefixAgeID;
                }
                if (this.comboBox_PrefixMax.SelectedIndex != -1)
                {
                    //Cast
                    PrefixAgeDisplay currentMaxPrefix = this.comboBox_PrefixMax.SelectedItem as PrefixAgeDisplay;
                    GeoEventValues[GSC_ProjectEditor.Constants.DatabaseFields.TGeoEventAgeMaxPrefix] = currentMaxPrefix.PrefixAgeID;
                }
                if (this.comboBox_TimescaleMin.SelectedIndex != -1)
                {
                    //Cast
                    TimeScaleDisplay currentMinTimeScale = this.comboBox_TimescaleMin.SelectedItem as TimeScaleDisplay;
                    GeoEventValues[GSC_ProjectEditor.Constants.DatabaseFields.TGeoEventAgeMinTimescale] = currentMinTimeScale.TimeScaleID;
                }
                if (this.comboBox_TimescaleMax.SelectedIndex != -1)
                {
                    //Cast
                    TimeScaleDisplay currentMaxTimeScale = this.comboBox_TimescaleMax.SelectedItem as TimeScaleDisplay;
                    GeoEventValues[GSC_ProjectEditor.Constants.DatabaseFields.TGeoEventAgeMaxTimescale] = currentMaxTimeScale.TimeScaleID;
                }

                if (this.comboBox_Source.SelectedIndex != -1)
                {
                    SourceDisplay currentSource = this.comboBox_Source.SelectedItem as SourceDisplay;
                    GeoEventValues[GSC_ProjectEditor.Constants.DatabaseFields.TGeoEventSourceID] = currentSource.sourceID;
                }

                #endregion

                #region Update or Add in GEO_EVENT table

                if (isUpdate)
                {
                    string currentEventQuery = eventTableID + " = " + GeoEventValues[eventTableID];
                    GSC_ProjectEditor.Tables.UpdateMultipleFieldValue(eventTable, currentEventQuery, GeoEventValues);
                }
                else
                {
                    GSC_ProjectEditor.Tables.AddRowWithValues(eventTable, GeoEventValues);
                }

                #endregion

                #region Update GEOLINE and/or LABELS
                //Get list of items to update
                ComboBox.ObjectCollection linkedItems = this.comboBox_linkedItem.Items;

                foreach (object links in linkedItems)
                {
                    //Cast
                    EventLinkedItemsDisplay linkInformation = links as EventLinkedItemsDisplay;

                    //Update
                    if (linkInformation.isLabel)
                    {
                        UpdateLabelWithEvent(linkInformation, GeoEventValues[eventTableID]);
                    }
                    else
                    {
                        UpdateGeolineWithEvent(linkInformation, GeoEventValues[eventTableID]);
                    }
                }


                #endregion

                this.Close();
                GSC_ProjectEditor.Messages.ShowEndOfProcess();
            }


        }

        /// <summary>
        /// Will fill the linked item combobox with elements selected on screen.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_OnScreenItemSelection_Click(object sender, EventArgs e)
        {
            //Get selected elements from label feature class
            List<IFeature> getSelected = Utilities.MapDocumentFeatureLayers.GetListOfAllSelectedFeatures();;

            //Validate if anything was found
            if (getSelected.Count != 0)
            {
                List<Tuple<string, object>> itemsToShow = new List<Tuple<string, object>>();
                Dictionary<string, bool> itemType = new Dictionary<string, bool>();
                //Filter list to get only labels and geolines
                List<IFeature> getFilteredSelection = new List<IFeature>();
                foreach (IFeature feat in getSelected)
                {
                    //Cast
                    ITable featTable = feat.Table;
                    IDataset featDataset = featTable as IDataset;
                    int IDIndex = 0;

                    //Build list to show in pop-up list box
                    if (featDataset.BrowseName == labelFC || featDataset.BrowseName == geolineFC)
                    {
                        getFilteredSelection.Add(feat);

                        //Get current display name
                        if (featDataset.BrowseName == labelFC)
                        {
                            IDIndex = feat.Fields.FindField(labelFCID);
                        }

                        if (featDataset.BrowseName == geolineFC)
                        {
                            IDIndex = feat.Fields.FindField(geolineFCID);
                        }
                        int currentOID = feat.OID;
                        string displayName = AgeItemDictonary[feat.get_Value(IDIndex).ToString()] + " (OID: " + currentOID.ToString() + ")";
                        
                        //Add to list
                        itemsToShow.Add(new Tuple<string, object>(displayName, currentOID));

                        //Add to dico 
                        if (feat.Shape.GeometryType == ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPoint)
	                    {
                            itemType[displayName] = true;
	                    }
                        else
                        {
                            itemType[displayName] = false;
                        }
                        
                    }
                }

                if (itemsToShow.Count != 0)
                {
                    //Add selection to pop-up list box
                    List<Tuple<string, object>> selectedPopupItems = GSC_ProjectEditor.Form_Generic.ShowGenericListBoxForm(Properties.Resources.Message_AgeItemTitle, Properties.Resources.Message_AgeItemMessage, this.Icon, itemsToShow);

                    //Add selected selection inside combobox
                    this.comboBox_linkedItem.DataSource = null; //Reset
                    List<EventLinkedItemsDisplay> cboxItems = new List<EventLinkedItemsDisplay>();
                    foreach (Tuple<string, object> selectedItemsFromForm in selectedPopupItems)
                    {
                        cboxItems.Add(new EventLinkedItemsDisplay { linkedItemName = selectedItemsFromForm.Item1, 
                            linkedItemID = selectedItemsFromForm.Item2.ToString(), onScreenSelection = true,
                            isLabel = itemType[selectedItemsFromForm.Item1]});
                    }
                    this.comboBox_linkedItem.DataSource = cboxItems;
                    this.comboBox_linkedItem.DisplayMember = "linkedItemName";
                    this.comboBox_linkedItem.ValueMember = "linkedItemID";
                    if (cboxItems.Count != 0)
                    {
                        this.comboBox_linkedItem.SelectedIndex = 0;
                    }
                }


            }
            else
            {
                GSC_ProjectEditor.Messages.ShowGenericErrorMessage(Properties.Resources.Error_NoSelectionFeature);
            }



        }

        /// <summary>
        /// Will initialize the ageItemDictionnary variable
        /// </summary>
        public void InitAgeItemDictionary()
        {
            AgeItemDictonary = null;
            //Get a list of geolines and map units
            string ageItemQuery = legendSymbolType + "= '" + legendSymbolTypeFill + "' OR " + legendSymbolType + " = '" + legendSymbolTypeLine + "'";
            AgeItemDictonary = GSC_ProjectEditor.Tables.GetUniqueDicoValues(legendTable, legendID, legendDisplayGIS, ageItemQuery);

        }

        /// <summary>
        /// Whenever the selection changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBox_Event_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Cast
            EventNameDisplay selectedEvent = this.comboBox_Event.SelectedItem as EventNameDisplay;

            ClearControls();

            //Conditions
            if (this.comboBox_Event.SelectedIndex != -1 && selectedEvent != null && selectedEvent.eventID != string.Empty && this.comboBox_Event.Items.Count != 0 && AgeItemDictonary!=null)
            {
                #region Fill linked item combobox
                //Get a list of all events linked to labels
                string eventLinkedLabelQuery = GSC_ProjectEditor.Constants.DatabaseFields.FLabelGeoEventID + " = " + selectedEvent.eventID;
                SortedDictionary<string, List<string>> linkedLabelEventsDico = GSC_ProjectEditor.Tables.GetAllDoubleUniqueFieldValues(labelFC,
                    labelFCID, GSC_ProjectEditor.Constants.DatabaseFields.ObjectID, eventLinkedLabelQuery);

                //Get a list of all events linked to geolines
                string eventLinkedGeolineQuery = GSC_ProjectEditor.Constants.DatabaseFields.FGeolineGeoEventID + " = " + selectedEvent.eventID;
                SortedDictionary<string, List<string>> linkedGeolineEventsDico = GSC_ProjectEditor.Tables.GetAllDoubleUniqueFieldValues(geolineFC,
                    geolineFCID, GSC_ProjectEditor.Constants.DatabaseFields.ObjectID, eventLinkedGeolineQuery);
            
                //Build new appended list
                Dictionary<string, List<string>> appendedList = new Dictionary<string,List<string>>();
                appendedList = appendedList.Concat(linkedLabelEventsDico).ToDictionary(x=>x.Key,x=>x.Value);
                appendedList = appendedList.Concat(linkedGeolineEventsDico).ToDictionary(x=>x.Key,x=>x.Value);

                //Build combobox list of linked items
                List<EventLinkedItemsDisplay> linkDisplayList = new List<EventLinkedItemsDisplay>();
                foreach (KeyValuePair<string, List<string>> kv in appendedList)
	            {
                    foreach (string items in kv.Value)
	                {
                        //Manage type
                        bool isItemLabel = false;
                        if (linkedLabelEventsDico.ContainsKey(kv.Key))
	                    {
		                    isItemLabel = true;
	                    }

                        //Build item display name
                        string itemDisplayName = AgeItemDictonary[kv.Key] + " (OID: " + items + ")";

                        //Add to list
		                linkDisplayList.Add(new EventLinkedItemsDisplay{linkedItemID = items, linkedItemName = itemDisplayName, isLabel = isItemLabel, onScreenSelection = true});
	                }
		            
	            }

                this.comboBox_linkedItem.DataSource = null;
                this.comboBox_linkedItem.DataSource = linkDisplayList;
                this.comboBox_linkedItem.DisplayMember = "linkedItemName";
                this.comboBox_linkedItem.ValueMember = "linkedItemID";
                if (linkDisplayList.Count != 0)
                {
                    this.comboBox_linkedItem.SelectedIndex = 0;  
                }
                else
                {
                    this.comboBox_linkedItem.SelectedIndex = -1;
                }
                

                #endregion

                #region Fill other controls

                FillControlsFromEventSelection(selectedEvent);

                #endregion
            }
        }

        /// <summary>
        /// Will fill all needed controls based on user event name selection
        /// </summary>
        /// <param name="selectedEvent"></param>
        private void FillControlsFromEventSelection(EventNameDisplay selectedEvent)
        {
            //Variables
            string eventID = selectedEvent.eventID;
            Dictionary<string, object> fieldValues = new Dictionary<string, object>();
            ITable eTable = GSC_ProjectEditor.Tables.OpenTable(eventTable);

            //Get a list of field indexes
            Dictionary<string, int> fieldIndexes = GetFieldIndexForControls(eTable);

            //Get a cursor and iterate through geo event table to retrieve wanted information
            string eventQuery = eventTableID + " =" + eventID;
            IQueryFilter eventFilter = new QueryFilter();
            eventFilter.WhereClause = eventQuery;
            
            ICursor eventCursor = eTable.Search(eventFilter, true);
            IRow eventRow = eventCursor.NextRow();
            while (eventRow != null)
            {

                foreach (KeyValuePair<string, int> fIndexes in fieldIndexes)
                {
                    fieldValues[fIndexes.Key] = eventRow.get_Value(fIndexes.Value); 
                }

                eventRow = eventCursor.NextRow();
            }

            GSC_ProjectEditor.ObjectManagement.ReleaseObject(eventCursor);

            #region Update prefix controls
            foreach (PrefixAgeDisplay preAge in this.comboBox_PrefixMin.Items)
            {
                if (preAge.PrefixAgeID == fieldValues[GSC_ProjectEditor.Constants.DatabaseFields.TGeoEventAgeMinPrefix].ToString())
                {
                    this.comboBox_PrefixMin.SelectedItem = preAge;
                }
            }
            foreach (PrefixAgeDisplay preAge in this.comboBox_PrefixMax.Items)
            {
                if (preAge.PrefixAgeID == fieldValues[GSC_ProjectEditor.Constants.DatabaseFields.TGeoEventAgeMaxPrefix].ToString())
                {
                    this.comboBox_PrefixMax.SelectedItem = preAge;
                }
            }
            #endregion

            #region Update timescale controls
            foreach (TimeScaleDisplay time in this.comboBox_TimescaleMin.Items)
            {
                if (time.TimeScaleID == fieldValues[GSC_ProjectEditor.Constants.DatabaseFields.TGeoEventAgeMinTimescale].ToString())
                {
                    this.comboBox_TimescaleMin.SelectedItem = time;
                }
            }
            foreach (TimeScaleDisplay time in this.comboBox_TimescaleMax.Items)
            {
                if (time.TimeScaleID == fieldValues[GSC_ProjectEditor.Constants.DatabaseFields.TGeoEventAgeMaxTimescale].ToString())
                {
                    this.comboBox_TimescaleMax.SelectedItem = time;
                }
            }
            #endregion

            #region Update source controls
            foreach (SourceDisplay source in this.comboBox_Source.Items)
            {
                if (source.sourceID == fieldValues[GSC_ProjectEditor.Constants.DatabaseFields.TGeoEventSourceID].ToString())
                {
                    this.comboBox_Source.SelectedItem = source;
                }
            }

            #endregion

            #region Update age value and certainty controls
            try
            {
                this.numericUpDown_ValueMin.Value = Convert.ToDecimal(fieldValues[GSC_ProjectEditor.Constants.DatabaseFields.TGeoEventAgeMinValue]);
            }
            catch (Exception)
            {

                this.numericUpDown_ValueMin.Value = Convert.ToDecimal(0.0);
            }

            try
            {
                this.numericUpDown_ValueMax.Value = Convert.ToDecimal(fieldValues[GSC_ProjectEditor.Constants.DatabaseFields.TGeoEventAgeMaxValue]);
            }
            catch (Exception)
            {

                this.numericUpDown_ValueMax.Value = Convert.ToDecimal(0.0);
            }
            try
            {
                this.numericUpDown_CertaintyMin.Value = Convert.ToDecimal(fieldValues[GSC_ProjectEditor.Constants.DatabaseFields.TGeoEventAgeMaxCertainty]);
            }
            catch (Exception)
            {

                this.numericUpDown_CertaintyMin.Value = Convert.ToDecimal(0.0);
            }
            try
            {
                this.numericUpDown_CertaintyMax.Value = Convert.ToDecimal(fieldValues[GSC_ProjectEditor.Constants.DatabaseFields.TGeoEventAgeMaxCertainty]);
            }
            catch (Exception)
            {

                this.numericUpDown_CertaintyMax.Value = Convert.ToDecimal(0.0);
            }
            
            #endregion

        }

        /// <summary>
        /// Will reset all the controls in the UI
        /// </summary>
        private void ClearControls()
        {

            this.comboBox_PrefixMin.SelectedIndex = -1;
            this.comboBox_PrefixMax.SelectedIndex = -1;
            this.comboBox_TimescaleMin.SelectedIndex = -1;
            this.comboBox_TimescaleMax.SelectedIndex = -1;
            this.comboBox_Source.SelectedIndex = -1;
            this.comboBox_linkedItem.DataSource = null;

            this.numericUpDown_ValueMin.Value = this.numericUpDown_ValueMin.Minimum;
            this.numericUpDown_ValueMax.Value = this.numericUpDown_ValueMax.Minimum;
            this.numericUpDown_CertaintyMin.Value = this.numericUpDown_CertaintyMin.Minimum;
            this.numericUpDown_CertaintyMax.Value = this.numericUpDown_CertaintyMax.Minimum;

        }

        #endregion

        #region MODEL

        /// <summary>
        /// Will update geoline feature class from the given item information
        /// </summary>
        /// <param name="inLinkedItem">The item to get information for update from</param>
        /// <param name="geoEventID">The geological event id to add inside the geoline feature class</param>
        private void UpdateGeolineWithEvent(EventLinkedItemsDisplay inLinkedItem, object geoEventID)
        {

            //Build selection query
            string geolineSelectionQuery = string.Empty;
            if (inLinkedItem.onScreenSelection)
            {
                geolineSelectionQuery = GSC_ProjectEditor.Constants.DatabaseFields.ObjectID + " = " + inLinkedItem.linkedItemID;
            }
            else
            {
                geolineSelectionQuery = GSC_ProjectEditor.Constants.DatabaseFields.FGeolineID + " = '" + inLinkedItem.linkedItemID + "'";
            }

            //Update
            GSC_ProjectEditor.Tables.UpdateFieldValue(geolineFC, GSC_ProjectEditor.Constants.DatabaseFields.FGeolineGeoEventID,
                geolineSelectionQuery, geoEventID);

        }

        /// <summary>
        /// Will update label feature class from the given item information
        /// </summary>
        /// <param name="inLinkedItem">The item to get information for update from</param>
        /// <param name="geoEventID">The geological event id to add inside the label feature class</param>
        private void UpdateLabelWithEvent(EventLinkedItemsDisplay inLinkedItem, object geoEventID)
        {
            //Build selection query
            string labelSelectionQuery = string.Empty;
            if (inLinkedItem.onScreenSelection)
            {
                labelSelectionQuery = GSC_ProjectEditor.Constants.DatabaseFields.ObjectID + " = " + inLinkedItem.linkedItemID;
            }
            else
            {
                labelSelectionQuery = GSC_ProjectEditor.Constants.DatabaseFields.FLabelID + " = '" + inLinkedItem.linkedItemID + "'";
            }

            //Update
            GSC_ProjectEditor.Tables.UpdateFieldValue(labelFC, GSC_ProjectEditor.Constants.DatabaseFields.FLabelGeoEventID,
                labelSelectionQuery, geoEventID);
        }

        /// <summary>
        /// Will return a dictionnary with geo event table field names and their respectives indexes inside the table
        /// </summary>
        /// <param name="inTable">The event table to retrieve the indexes from</param>
        /// <returns></returns>
        private Dictionary<string, int> GetFieldIndexForControls(ITable inTable)
        {
            //Variables
            Dictionary<string, int> fieldIndexes = new Dictionary<string, int>();
            List<string> fieldList = new List<string>();

            fieldList.Add(GSC_ProjectEditor.Constants.DatabaseFields.TGeoEventAgeMinValue);
            fieldList.Add(GSC_ProjectEditor.Constants.DatabaseFields.TGeoEventAgeMaxValue);
            fieldList.Add(GSC_ProjectEditor.Constants.DatabaseFields.TGeoEventAgeMinCertainty);
            fieldList.Add(GSC_ProjectEditor.Constants.DatabaseFields.TGeoEventAgeMaxCertainty);
            fieldList.Add(GSC_ProjectEditor.Constants.DatabaseFields.TGeoEventAgeMinPrefix);
            fieldList.Add(GSC_ProjectEditor.Constants.DatabaseFields.TGeoEventAgeMaxPrefix);
            fieldList.Add(GSC_ProjectEditor.Constants.DatabaseFields.TGeoEventAgeMinTimescale);
            fieldList.Add(GSC_ProjectEditor.Constants.DatabaseFields.TGeoEventAgeMaxTimescale);
            fieldList.Add(GSC_ProjectEditor.Constants.DatabaseFields.TGeoEventSourceID);

            foreach (string names in fieldList)
            {
                fieldIndexes[names] = inTable.Fields.FindField(names);
            }

            return fieldIndexes;
        }

        #endregion


    }
}
