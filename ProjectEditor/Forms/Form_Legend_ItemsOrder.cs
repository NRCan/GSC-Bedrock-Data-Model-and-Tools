using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ESRI.ArcGIS.Geodatabase;

namespace GSC_ProjectEditor
{
    public partial class Form_Legend_ItemsOrder : Form
    {
        #region Main Variables
        public object legendOrder { get; set; }

        //Legend generator table and fields
        private const string lTable = GSC_ProjectEditor.Constants.Database.TLegendGene;
        private const string lSymType = GSC_ProjectEditor.Constants.DatabaseFields.LegendSymType;
        private const string lLabelID = GSC_ProjectEditor.Constants.DatabaseFields.LegendLabelID;
        private const string lLabelName = GSC_ProjectEditor.Constants.DatabaseFields.LegendGISDisplay;
        private const string lOrder = GSC_ProjectEditor.Constants.DatabaseFields.LegendOrder;
        private const string lIndent = GSC_ProjectEditor.Constants.DatabaseFields.LegendIndentation;
        private const string lSymTheme = GSC_ProjectEditor.Constants.DatabaseFields.LegendItemType;
        private const string lOID = GSC_ProjectEditor.Constants.DatabaseFields.ObjectID;

        //DOMAINS
        private const string muPID = GSC_ProjectEditor.Constants.DatabaseDomains.MapUnit;
        private const string symTypeCodeFill = GSC_ProjectEditor.Constants.DatabaseDomainsValues.SymTypeFill;
        private const string symTypeCodeLine = GSC_ProjectEditor.Constants.DatabaseDomainsValues.SymTypeLine;
        private const string symTypeCodePoint = GSC_ProjectEditor.Constants.DatabaseDomainsValues.SymTypePoint;
        private string symTypeCodeHeader = GSC_ProjectEditor.Constants.DatabaseDomainsValues.SymTypeHeader1.Replace("1", "");
        private const string symTypeCodeHeader1 = GSC_ProjectEditor.Constants.DatabaseDomainsValues.SymTypeHeader1;
        private const string symTypeCodeHeader2 = GSC_ProjectEditor.Constants.DatabaseDomainsValues.SymTypeHeader2;
        private const string symTypeCodeHeader3 = GSC_ProjectEditor.Constants.DatabaseDomainsValues.SymTypeHeader3;

        //Values
        private Dictionary<string, string> geolineDictionary { get; set; }
        private Dictionary<string, string> geopointDictionary { get; set; }
        private Dictionary<string, string> mapUnitDictionary { get; set; }

        //Other
        private const string symTypeDefaultValue = GSC_ProjectEditor.Constants.FieldDefaults.LegendSymbolType;
        public static Random newRandGenerator = new Random(); //Will be used to add some temporary legend order for new headers.
        public string indentation = "\t"; //Will be used to indent items in the legend item order listbox
        public List<string> processedHeaders = new List<string>(); //A list to keep track of new added headers and prevent adding them twice.
        public int selectedItemIndex = -1;
        #endregion

        public Form_Legend_ItemsOrder()
        {
            //Set culture before init.
            Utilities.Culture.SetCulture();

            InitializeComponent();

            //Get shown event to retrieve received text from dock window.
            this.Shown += new EventHandler(FormExtendedListBox_Shown);

            this.listBox_LegendItems.DrawMode = DrawMode.OwnerDrawFixed;
            this.listBox_LegendItems.DrawItem += new DrawItemEventHandler(listBox_LegendItems_DrawItem);
            string formatedString = string.Format(Properties.Resources.ToolTip_LegendOrderListBox.ToString(), Environment.NewLine);
            this.toolTip_listBoxColors.SetToolTip(this.listBox_LegendItems, Properties.Resources.ToolTip_LegendOrderListBox);
        }

        /// <summary>
        /// Will modify item back colors based on the symbol type
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void listBox_LegendItems_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index != -1)
            {
                //Get some init values
                e.DrawBackground();
                Graphics g = e.Graphics;
                ListBox lb = (ListBox)sender;
                Rectangle newColoredRectangle = e.Bounds;
                newColoredRectangle.Width = 15;
                Font boldFont = e.Font;

                //Get item type
                LegendOrder currentLegendItem = this.listBox_LegendItems.Items[e.Index] as LegendOrder;
                string itemType = currentLegendItem.ItemType;

                //Parse type
                if (itemType.Contains(symTypeCodeHeader))
                {
                    //Set color
                    g.FillRectangle(new SolidBrush(Color.White), newColoredRectangle);

                    //Set font
                    boldFont = new Font(e.Font, FontStyle.Bold);


                }
                else if (itemType == symTypeCodeFill)
                {
                    //Set color (orange)
                    g.FillRectangle(new SolidBrush(Color.FromArgb(255, 255, 192, 0)), newColoredRectangle);
                }
                else if (itemType == symTypeCodeLine)
                {
                    //Set color (blue)
                    g.FillRectangle(new SolidBrush(Color.FromArgb(255, 83, 141, 213)), newColoredRectangle);
                }
                else if (itemType == symTypeCodePoint)
                {
                    //Set color (purple)
                    g.FillRectangle(new SolidBrush(Color.ForestGreen), newColoredRectangle);
                }

                g.DrawString(currentLegendItem.ItemDisplayName, boldFont, new SolidBrush(Color.Black), new PointF(e.Bounds.X + 15, e.Bounds.Y));
                e.DrawFocusRectangle();
            }
 

        }

        #region View Model
        /// <summary>
        /// A class that will keel items names and ids
        /// Will be used as datasource for the legend order listbox
        /// </summary>
        public class LegendOrder
        {
            public string ItemDisplayName { get; set; }
            public string ItemValueName { get; set; } //ID
            public string ItemParentName { get; set; } //Indentation
            public string ItemChildName { get; set; } //Identation
            public int ItemIdentationNumber { get; set; } //Indentation
            public string ItemType { get; set; } //Item type
            public bool ItemIsNew { get; set; } //for new headers
        }

        /// <summary>
        /// Fill the main list box with items coming from the original one
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void FormExtendedListBox_Shown(object sender, EventArgs e)
        {

            //Init random seed
            Random orderRandom = new Random((int)DateTime.Now.Ticks + 474);

            //Initialise some dictionary
            SortedDictionary<decimal, LegendOrder> legendOrders = new SortedDictionary<decimal, LegendOrder>();

            //Init a new listbox datasource
            List<LegendOrder> legendOrderToListBox = new List<LegendOrder>();

            //Iterate through legend generator table to get some information from
            ICursor legendCursor = GSC_ProjectEditor.Tables.GetTableCursor("Search", null, lTable);

            //Get some indexes before starting
            int symTypeIndex = legendCursor.Fields.FindField(lSymType);
            int symOrderIndex = legendCursor.Fields.FindField(lOrder);
            int itemIDIndex = legendCursor.Fields.FindField(lLabelID);
            int itemNameIndex = legendCursor.Fields.FindField(lLabelName);
            int indentIndex = legendCursor.Fields.FindField(lIndent);
            int oidIndex = legendCursor.Fields.FindField(lOID);
            try
            {
                #region Get usefull information from legend table
                IRow legendRow = legendCursor.NextRow();
                while (legendRow != null)
                {

                    //Get field informations
                    decimal symOrder = -1;
                    if (legendRow.get_Value(symOrderIndex) != DBNull.Value)
                    {
                        symOrder = Convert.ToDecimal(legendRow.get_Value(symOrderIndex));

                        //Get rid of old 999 values
                        if (symOrder == 999)
                        {
                            symOrder = GSC_ProjectEditor.IDs.GetRandomInt(orderRandom) + Convert.ToDecimal(legendRow.get_Value(oidIndex));
                        }

                        while (legendOrders.ContainsKey(symOrder))
                        {
                            symOrder++;
                        }
                    }
                    else
                    {
                        symOrder = Convert.ToDecimal(legendRow.get_Value(oidIndex)) + 10000;

                        while (legendOrders.ContainsKey(symOrder))
                        {
                            symOrder++;
                        }
                    }

                    int symIndent = 0;
                    if (legendRow.get_Value(indentIndex) != DBNull.Value)
                    {
                        symIndent = Convert.ToInt32(legendRow.get_Value(indentIndex));
                    }
                    LegendOrder newOrderItem = new LegendOrder();
                    newOrderItem.ItemType = legendRow.get_Value(symTypeIndex).ToString();
                    newOrderItem.ItemDisplayName = legendRow.get_Value(itemNameIndex).ToString();
                    newOrderItem.ItemValueName = legendRow.get_Value(itemIDIndex).ToString();
                    newOrderItem.ItemIdentationNumber = symIndent;

                    legendOrders[symOrder] = newOrderItem;

                    legendRow = legendCursor.NextRow();
                }

                //Delete com objects
                System.Runtime.InteropServices.Marshal.ReleaseComObject(legendCursor);
                #endregion

                #region Build the list

                //Iterate through a sorted list
                foreach (KeyValuePair<decimal, LegendOrder> item in legendOrders)
                {
                    //If item needs some indentation
                    if ((item.Key % 1) != 0)
                    {
                        //Get the number of decimals
                        int decimals = GSC_ProjectEditor.MathCustom.GetDecimalPlaces(item.Key);

                        //Add indent
                        if (decimals == 1)
                        {
                            item.Value.ItemDisplayName = indentation + item.Value.ItemDisplayName;
                        }
                        else if (decimals == 2)
                        {
                            item.Value.ItemDisplayName = indentation + indentation + item.Value.ItemDisplayName;
                        }
                        else if (decimals == 3)
                        {
                            item.Value.ItemDisplayName = indentation + indentation + indentation + item.Value.ItemDisplayName;
                        }
                        
                    }
                    //Add to list
                    //legendOrderToListBox.Add(new LegendDisplay { ItemDisplayName = item.Value.ItemDisplayName, ItemValueName = item.Value.ItemType });
                    item.Value.ItemIsNew = false;
                    legendOrderToListBox.Add(item.Value);

                }

                this.listBox_LegendItems.DataSource = legendOrderToListBox;
                this.listBox_LegendItems.DisplayMember = "ItemDisplayName";
                this.listBox_LegendItems.ValueMember = "ItemValueName";
                this.listBox_LegendItems.SelectedIndex = -1;
                

                #endregion

            }
            catch (Exception ex)
            {
                int lineNumber = GSC_ProjectEditor.Exceptions.LineNumber(ex);
                MessageBox.Show(ex.StackTrace + "; " + ex.Message);
            }
            
        }

        /// <summary>
        /// Will send selected items down one place
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_Down_Click(object sender, EventArgs e)
        {

            //Get current selected items from list
            ListBox.SelectedObjectCollection objectCollection = this.listBox_LegendItems.SelectedItems;

            //Transform object collection to list og legendOrder objects
            List<LegendOrder> selectedItems = new List<LegendOrder>();
            foreach (object obj in objectCollection)
            {
                selectedItems.Add(obj as LegendOrder);
            }

            //Cast full list from list box
            List<LegendOrder> listBoxSource = this.listBox_LegendItems.DataSource as List<LegendOrder>;

            //Get current index nad build list of new indices of selection
            ListBox.SelectedIndexCollection currentIndex = this.listBox_LegendItems.SelectedIndices;
            List<int> newIndices = Enumerable.Range(currentIndex[0] + 1, currentIndex.Count).ToList();

            //If anything is selected continue
            if (currentIndex.Count > 0 && currentIndex.Count != -1 && currentIndex[currentIndex.Count - 1] != listBoxSource.Count - 1)
            {
                //Remove selected items from list
                listBoxSource.RemoveRange(currentIndex[0], currentIndex.Count);

                //Insert selected items at the top of the list
                listBoxSource.InsertRange(currentIndex[0] + 1, selectedItems);

                //Re-initialise listbox control
                this.listBox_LegendItems.DataSource = null;
                this.listBox_LegendItems.DataSource = listBoxSource;
                this.listBox_LegendItems.DisplayMember = "ItemDisplayName";
                this.listBox_LegendItems.ValueMember = "ItemValueName";
                this.listBox_LegendItems.ClearSelected();
                foreach (int indices in newIndices)
                {
                    //Will select all re-inserted items
                    this.listBox_LegendItems.SelectedIndex = indices;
                }
            }
          
        }

        /// <summary>
        /// Will send selected items to the top of the list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_Top_Click(object sender, EventArgs e)
        {
            //Get current selected items from list
            ListBox.SelectedObjectCollection objectCollection = this.listBox_LegendItems.SelectedItems;

            //Transform object collection to list og legendOrder objects
            List<LegendOrder> selectedItems = new List<LegendOrder>();
            foreach (object obj in objectCollection)
            {
                selectedItems.Add(obj as LegendOrder);
            }

            //Cast full list from list box
            List<LegendOrder> listBoxSource = this.listBox_LegendItems.DataSource as List<LegendOrder>;

            //Get current index nad build list of new indices of selection
            ListBox.SelectedIndexCollection currentIndex = this.listBox_LegendItems.SelectedIndices;
            List<int> newIndices = Enumerable.Range(0, currentIndex.Count).ToList();

            //If anything is selected continue
            if (currentIndex.Count > 0 && currentIndex.Count != -1)
            {
                //Remove selected items from list
                listBoxSource.RemoveRange(currentIndex[0], currentIndex.Count);

                //Insert selected items at the top of the list
                listBoxSource.InsertRange(0, selectedItems);

                //Re-initialise listbox control
                this.listBox_LegendItems.DataSource = null;
                this.listBox_LegendItems.DataSource = listBoxSource;
                this.listBox_LegendItems.DisplayMember = "ItemDisplayName";
                this.listBox_LegendItems.ValueMember = "ItemValueName";
                this.listBox_LegendItems.ClearSelected();
                foreach (int indices in newIndices)
                {
                    //Will select all re-inserted items
                    this.listBox_LegendItems.SelectedIndex = indices;
                }
            }
        }

        /// <summary>
        /// Will send selected items up one place.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_Up_Click(object sender, EventArgs e)
        {
            //current selected items from list
            ListBox.SelectedObjectCollection objectCollection = this.listBox_LegendItems.SelectedItems;

            //Transform object collection to list og legendOrder objects
            List<LegendOrder> selectedItems = new List<LegendOrder>();
            foreach (object obj in objectCollection)
            {
                selectedItems.Add(obj as LegendOrder);
            }

            //Cast full list from list box
            List<LegendOrder> listBoxSource = this.listBox_LegendItems.DataSource as List<LegendOrder>;

            //Get current index nad build list of new indices of selection
            ListBox.SelectedIndexCollection currentIndex = this.listBox_LegendItems.SelectedIndices;
            List<int> newIndices = Enumerable.Range(currentIndex[0] - 1, currentIndex.Count).ToList();

            //If anything is selected continue and his not the last in the list
            if (currentIndex.Count > 0 && currentIndex.Count != -1 && currentIndex[0] != 0)
            {
                //Remove selected items from list
                listBoxSource.RemoveRange(currentIndex[0], currentIndex.Count);

                //Insert selected items at the top of the list
                listBoxSource.InsertRange(currentIndex[0] - 1, selectedItems);

                //Re-initialise listbox control
                this.listBox_LegendItems.DataSource = null;
                this.listBox_LegendItems.DataSource = listBoxSource;
                this.listBox_LegendItems.DisplayMember = "ItemDisplayName";
                this.listBox_LegendItems.ValueMember = "ItemValueName";
                this.listBox_LegendItems.ClearSelected();
                foreach (int indices in newIndices)
                {
                    //Will select all re-inserted items
                    this.listBox_LegendItems.SelectedIndex = indices;
                }
            }
        }

        /// <summary>
        /// Will send selected items to the bottom of the list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_Bottom_Click(object sender, EventArgs e)
        {
            //Get current selected items from list
            ListBox.SelectedObjectCollection objectCollection = this.listBox_LegendItems.SelectedItems;

            //Transform object collection to list og legendOrder objects
            List<LegendOrder> selectedItems = new List<LegendOrder>();
            foreach (object obj in objectCollection)
            {
                selectedItems.Add(obj as LegendOrder);
            }

            //Cast full list from list box
            List<LegendOrder> listBoxSource = this.listBox_LegendItems.DataSource as List<LegendOrder>;

            //Get current index nad build list of new indices of selection
            ListBox.SelectedIndexCollection currentIndex = this.listBox_LegendItems.SelectedIndices;
            List<int> newIndices = Enumerable.Range(listBoxSource.Count - currentIndex.Count, currentIndex.Count).ToList();

            //If anything is selected continue
            if (currentIndex.Count > 0 && currentIndex.Count != -1)
            {
                //Remove selected items from list
                listBoxSource.RemoveRange(currentIndex[0], currentIndex.Count);

                //Insert selected items at the top of the list
                listBoxSource.InsertRange(listBoxSource.Count, selectedItems);

                //Re-initialise listbox control
                this.listBox_LegendItems.DataSource = null;
                this.listBox_LegendItems.DataSource = listBoxSource;
                this.listBox_LegendItems.DisplayMember = "ItemDisplayName";
                this.listBox_LegendItems.ValueMember = "ItemValueName";
                this.listBox_LegendItems.ClearSelected();
                foreach (int indices in newIndices)
                {
                    //Will select all re-inserted items
                    this.listBox_LegendItems.SelectedIndex = indices;
                }
            }
        }

        /// <summary>
        /// Will remove one indentation from selected items
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_leftIndentation_Click(object sender, EventArgs e)
        {
            //Get current selected items from list
            ListBox.SelectedObjectCollection objectCollection = this.listBox_LegendItems.SelectedItems;

            //Transform object collection to list og legendOrder objects
            List<LegendOrder> selectedItems = new List<LegendOrder>();
            foreach (object obj in objectCollection)
            {
                selectedItems.Add(obj as LegendOrder);
            }

            //Cast full list from list box
            List<LegendOrder> listBoxSource = this.listBox_LegendItems.DataSource as List<LegendOrder>;

            //Get current index nad build list of new indices of selection
            ListBox.SelectedIndexCollection currentIndex = this.listBox_LegendItems.SelectedIndices;
            List<int> newIndices = Enumerable.Range(listBoxSource.Count - currentIndex.Count, currentIndex.Count).ToList();

            //If anything is selected continue
            if (currentIndex.Count > 0 && currentIndex.Count != -1)
            {
                foreach (LegendOrder items in selectedItems)
                {
                    //Block to 3 indentation max
                    if (items.ItemDisplayName.Contains(indentation))
                    {
                        string sourceString = items.ItemDisplayName;
                        string outputString = sourceString.Remove(sourceString.IndexOf(indentation), indentation.Length);
                        items.ItemDisplayName = outputString;
                    }

                }

                //Re-initialise listbox control
                this.listBox_LegendItems.DataSource = null;
                this.listBox_LegendItems.DataSource = listBoxSource;
                this.listBox_LegendItems.DisplayMember = "ItemDisplayName";
                this.listBox_LegendItems.ValueMember = "ItemValueName";

            }
        }

        /// <summary>
        /// Will add one indentation to selected items
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_rightIndentation_Click(object sender, EventArgs e)
        {
            //Get current selected items from list
            ListBox.SelectedObjectCollection objectCollection = this.listBox_LegendItems.SelectedItems;

            //Transform object collection to list og legendOrder objects
            List<LegendOrder> selectedItems = new List<LegendOrder>();
            foreach (object obj in objectCollection)
            {
                selectedItems.Add(obj as LegendOrder);
            }

            //Cast full list from list box
            List<LegendOrder> listBoxSource = this.listBox_LegendItems.DataSource as List<LegendOrder>;

            //Get current index nad build list of new indices of selection
            ListBox.SelectedIndexCollection currentIndex = this.listBox_LegendItems.SelectedIndices;
            List<int> newIndices = Enumerable.Range(listBoxSource.Count - currentIndex.Count, currentIndex.Count).ToList();

            //If anything is selected continue
            if (currentIndex.Count > 0 && currentIndex.Count != -1)
            {
                foreach (LegendOrder items in selectedItems)
                {
                    //Block to 3 indentation max
                    int countIndent = items.ItemDisplayName.Split(indentation.ToCharArray()).Length - 1;
                    if (countIndent < 3)
                    {
                        items.ItemDisplayName = indentation + items.ItemDisplayName;
                    }

                }

                //Re-initialise listbox control
                this.listBox_LegendItems.DataSource = null;
                this.listBox_LegendItems.DataSource = listBoxSource;
                this.listBox_LegendItems.DisplayMember = "ItemDisplayName";
                this.listBox_LegendItems.ValueMember = "ItemValueName";

            }
        }

        /// <summary>
        /// By clicking this button the current entered text for the header will be added to legend item list.
        /// If the header is being updated and is not a new one, display name will be changed inside legend table also.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_AddHeaderToList_Click(object sender, EventArgs e)
        {
            if (this.txtbox_Header.Text != "" || this.txtbox_Header.Tag != null) 
            {
                //Get user choice of header type
                string headerType = "";
                if (this.radioButton_H1.Checked)
                {
                    headerType = symTypeCodeHeader1;
                }
                else if (this.radioButton_H2.Checked)
                {
                    headerType = symTypeCodeHeader2;
                }
                else if (this.radioButton_H3.Checked)
                {
                    headerType = symTypeCodeHeader3;
                }

                //Access current legend item order list
                List<LegendOrder> currentOrderList = this.listBox_LegendItems.DataSource as List<LegendOrder>;

                //Init a new legend order object or get what is already inside the tag of the textbox (update wanted from user)
                LegendOrder newHeader = new LegendOrder();
                if (this.txtbox_Header.Tag !=null)
                {
                    newHeader = this.txtbox_Header.Tag as LegendOrder;
                }
                newHeader.ItemDisplayName = this.txtbox_Header.Text;
                
                newHeader.ItemType = headerType;

                //Update or new header value name
                if (newHeader.ItemValueName == null || newHeader.ItemIsNew)
                {
                    newHeader.ItemIsNew = true;
                    newHeader.ItemType = headerType;
                }
                else
                {
                    //Update display name inside legend
                    string upQuery = lLabelID + " = '" + newHeader.ItemValueName + "'";
                    GSC_ProjectEditor.Tables.UpdateFieldValue(lTable, lLabelName, upQuery, newHeader.ItemDisplayName);

                    //Update header type
                    GSC_ProjectEditor.Tables.UpdateFieldValue(lTable, lSymType, upQuery, newHeader.ItemType);

                    newHeader.ItemIsNew = false;
                }
                

                //Add to list, at the top if new else, update
                bool itemExists = false;
                foreach (LegendOrder lo in currentOrderList)
                {
                    if (lo.ItemValueName == newHeader.ItemValueName)
                    {
                        itemExists = true;

                    }
                }

                if (!itemExists)
                {
                    currentOrderList.Insert(0, newHeader);
                    newHeader.ItemIsNew = true;
                    newHeader.ItemType = headerType;
                    newHeader.ItemValueName = this.txtbox_Header.Text;
                }
                else
                {
                    newHeader.ItemIsNew = false;
                }
                

                //Reset list box with new list
                this.listBox_LegendItems.DataSource = null;
                this.listBox_LegendItems.DataSource = currentOrderList;
                this.listBox_LegendItems.DisplayMember = "ItemDisplayName";
                this.listBox_LegendItems.ValueMember = "ItemValueName";
                this.listBox_LegendItems.ClearSelected();
                this.txtbox_Header.Text = string.Empty;
                this.txtbox_Header.Tag = null;


            }
        }

        /// <summary>
        /// Will update the legend generator table with new order for all items within the listbox
        /// </summary>
        private void btn_ModifyItem_Click(object sender, EventArgs e)
        {

            //Variables
            Dictionary<string, List<object>> newHeaderDico = new Dictionary<string, List<object>>();
            Dictionary<string, double> currentIDList = new Dictionary<string, double>(); //Will be used to get a simple list of ids just as in listbox
            double lastOrder = 0; //Init.
            double order = 0; //Init.

            //Get listbox datasource
            List<LegendOrder> getCurrentOrderList = this.listBox_LegendItems.DataSource as List<LegendOrder>;

            #region Pre-processing
            //Get a list of item ids and order
            foreach (LegendOrder itemOrder in getCurrentOrderList)
            {
                //Calculate order
                int countIndent = itemOrder.ItemDisplayName.Split(indentation.ToCharArray()).Length - 1;
                if (countIndent == 1)
                {
                    order = lastOrder + 0.1;
                }
                else if (countIndent == 2)
                {
                    order = lastOrder + 0.01;
                }
                else if (countIndent == 3)
                {
                    order = lastOrder + 0.001;
                }
                else
                {
                    order = Math.Floor(lastOrder) + 1;
                }

                //Calculate id
                string id = itemOrder.ItemValueName;

                //Update list of ids and orders
                if (itemOrder.ItemIsNew && !processedHeaders.Contains(itemOrder.ItemDisplayName))
                {
                    //Calculate new id for new header
                    id = Guid.NewGuid().ToString();

                    //Get header type
                    string getHeaderType = itemOrder.ItemType;

                    //Add to dico
                    newHeaderDico[id] = new List<object> { order, getHeaderType, itemOrder.ItemDisplayName };

                    //Add to validation list
                    processedHeaders.Add(itemOrder.ItemDisplayName);

                    currentIDList[id] = order;

                }

                currentIDList[id] = order;

                //Update last order
                lastOrder = order;
            }

            #endregion

            //Add new rows for new headers
            AddHeaderFromDico(newHeaderDico);

            //Update existing items
            UpdateExistingOrder(currentIDList);

            GSC_ProjectEditor.Messages.ShowEndOfProcess();
        }

        #endregion

        #region MODEL

        /// <summary>
        /// Will add new rows within legend generator table for the new headers
        /// </summary>
        /// <param name="newHeaderDico">A complete dico with legendItemIds as key and order number as value</param>
        public void AddHeaderFromDico(Dictionary<string, List<object>> newHeaderDico)
        {
            if (newHeaderDico.Count != 0)
            {

                //Get table object
                ITable inWantedTable = GSC_ProjectEditor.Tables.OpenTable(lTable);

                //Create a row buffer object (a template of all fields of table)
                IRowBuffer inRowBuffer = inWantedTable.CreateRowBuffer();

                //Start a cursor to insert new row
                ICursor inCursor = inWantedTable.Insert(true);

                int itemAddIDIndex = inCursor.Fields.FindField(lLabelID);
                int itemAddOrderIndex = inCursor.Fields.FindField(lOrder);
                int itemAddSymType = inCursor.Fields.FindField(lSymType);
                int itemAddGeolName = inCursor.Fields.FindField(lLabelName);
                int itemAddSymThemeIndex = inCursor.Fields.FindField(lSymTheme);

                //Iterate through dictionnary
                foreach (KeyValuePair<string, List<object>> elements in newHeaderDico)
                {
                    //Get value as a list
                    List<object> elementList = elements.Value as List<object>;
                    Double decimalOrder = Convert.ToDouble(elementList[0]);

                    //Set field value
                    inRowBuffer.set_Value(itemAddIDIndex, elements.Key);
                    inRowBuffer.set_Value(itemAddOrderIndex, decimalOrder);
                    inRowBuffer.set_Value(itemAddSymType, elementList[1]);
                    inRowBuffer.set_Value(itemAddGeolName, elementList[2]);
                    inRowBuffer.set_Value(itemAddSymThemeIndex, GSC_ProjectEditor.Constants.DatabaseDomainsValues.legendItemHeader);

                    //Add the new row
                    inCursor.InsertRow(inRowBuffer);
                    inCursor.Flush();

                }

                //Release the cursor or else some lock could happen.
                System.Runtime.InteropServices.Marshal.ReleaseComObject(inCursor);
            }
        }

        /// <summary>
        /// Will update the legend order of all the items within legend_generator table
        /// </summary>
        /// <param name="currentIDList"></param>
        public void UpdateExistingOrder(Dictionary<string, double> currentIDList)
        {
            //Get a cursor for legend generator table 
            ICursor lgCursor = GSC_ProjectEditor.Tables.GetTableCursor("Update", null, lTable);

            //Get some indexe before starting the cursor
            int itemNameIndex = lgCursor.Fields.FindField(lLabelName);
            int itemIDIndex = lgCursor.Fields.FindField(lLabelID);
            int itemOrderIndex = lgCursor.Fields.FindField(lOrder);

            //Star the cursor
            int seed = 0;
            double order = 0;
            IRow lgRow = lgCursor.NextRow();
            while (lgRow != null)
            {
                //Get current item from dico
                if (currentIDList.ContainsKey(lgRow.get_Value(itemIDIndex).ToString()))
                {
                    //Get current order in list
                    order = currentIDList[lgRow.get_Value(itemIDIndex).ToString()];

                    //Set new value
                    lgRow.set_Value(itemOrderIndex, order);


                }

                //Update
                lgCursor.UpdateRow(lgRow);
                lgCursor.Flush();

                seed++;

                lgRow = lgCursor.NextRow();
            }

            //Release the cursor or else some lock could happen.
            System.Runtime.InteropServices.Marshal.ReleaseComObject(lgCursor);
        }

        #endregion

        /// <summary>
        /// Wait for any selection from user that would like to update header display name.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listBox_LegendItems_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.listBox_LegendItems.SelectedItem !=null)
            {
                //Make sure the selected item was not already selected, else clear selection
                if (this.listBox_LegendItems.SelectedIndex != -1)
                {

                    //keep selected index 
                    selectedItemIndex = this.listBox_LegendItems.SelectedIndex;

                    //Get item type
                    LegendOrder currentLegendItem = this.listBox_LegendItems.SelectedItem as LegendOrder;
                    string itemType = currentLegendItem.ItemType;

                    //Parse type
                    if (itemType.Contains(symTypeCodeHeader))
                    {
                        //Add back to the text box for headers
                        this.txtbox_Header.Text = currentLegendItem.ItemDisplayName;
                        this.txtbox_Header.Tag = currentLegendItem;

                        if (currentLegendItem.ItemType.Contains(symTypeCodeHeader1))
                        {
                            this.radioButton_H1.Checked = true;
                        }
                        if (currentLegendItem.ItemType.Contains(symTypeCodeHeader2))
                        {
                            this.radioButton_H2.Checked = true;
                        }
                        if (currentLegendItem.ItemType.Contains(symTypeCodeHeader3))
                        {
                            this.radioButton_H3.Checked = true;
                        }

                    }
                    else
                    {
                        //Clear
                        this.txtbox_Header.Text = string.Empty;
                        this.txtbox_Header.Tag = null;
                    }
                    

                }

            }
            
        }

    }
}
