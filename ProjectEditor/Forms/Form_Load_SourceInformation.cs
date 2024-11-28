using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ESRI.ArcGIS.CatalogUI;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Catalog;

namespace GSC_ProjectEditor
{
    public partial class Form_Load_SourceInformation : Form
    {
        #region Main Variables

        //Source table
        private const string TSource = GSC_ProjectEditor.Constants.Database.TSource;
        private const string TSourceID = GSC_ProjectEditor.Constants.DatabaseFields.TSourceID;
        private const string TSourceName = GSC_ProjectEditor.Constants.DatabaseFields.TsourceName;
        private const string TSourceRemarks = GSC_ProjectEditor.Constants.DatabaseFields.TSourceRemarks;
        private const string TSourceDOI = GSC_ProjectEditor.Constants.DatabaseFields.TSourceDOI;
        private const string TSourceAbbr = GSC_ProjectEditor.Constants.DatabaseFields.TSourceAbbr;
        private const string TSourceFilePath = GSC_ProjectEditor.Constants.DatabaseFields.TSourceFilePath;
        private const string TSourceExtended = GSC_ProjectEditor.Constants.DatabaseFields.TSourceExtended;

        //Source domain
        private const string DSourceRef = GSC_ProjectEditor.Constants.DatabaseDomains.Source;

        //Others
        bool trigger = false;
        private IGxApplication m_pApp;

        #endregion

        /// <summary>
        /// A class that will keep source names and ids
        /// Will be used as datasource for source combobox
        /// </summary>
        public class UniqueSources
        {
            public string Abbr { get; set; }
            public string ID { get; set; }
        }

        public Form_Load_SourceInformation()
        {
            //Init culture
            Utilities.Culture.SetCulture();

            InitializeComponent();

            this.Shown += new EventHandler(FormSource_Shown);
        }

        void FormSource_Shown(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.dwEnabling)
            {
                //Fill project combobox
                fillSourceCombobox();

                trigger = true; //Will be used to prevent combobox index changed event to be triggered at init.
            }
            else
            {
                this.Close();
            }
        }

        /// <summary>
        /// Will fill the source combobox based on rows in source table
        /// </summary>
        public void fillSourceCombobox()
        {
            //Reset
            this.cbox_selectSource.DataSource = null;
            List<UniqueSources> newSourceList = new List<UniqueSources>();



            //Get environnement database path
            string envDBPath = Properties.Settings.Default.PROJECT_DATABASE_PATH;

            //Build a workspace
            IWorkspace getEnvWorkspace = GSC_ProjectEditor.Workspace.AccessWorkspace(envDBPath);

            //Retrieve a list of values from environment table
            List<string> sourceIDs = GSC_ProjectEditor.Tables.GetFieldValues(TSource, TSourceID, null);
            List<string> sourceAbbr = GSC_ProjectEditor.Tables.GetFieldValues(TSource, TSourceAbbr, null);

            if (sourceAbbr.Count != 0)
            {
                //Fill combobox
                foreach (string sources in sourceAbbr)
                {
                    //Get index
                    int currentIndex = sourceAbbr.IndexOf(sources);

                    //Add source to list
                    newSourceList.Add(new UniqueSources { Abbr = sources, ID = sourceIDs[currentIndex] });
                }


            }

            //Sort source list
            newSourceList.Sort((x, y) => x.Abbr.CompareTo(y.Abbr));

            //Add keywords "Add Datasource" at the beginning of the new list
            newSourceList.Insert(0, new UniqueSources { Abbr = Properties.Resources.Message_AddNewsSource, ID = "" });

            this.cbox_selectSource.DataSource = newSourceList;
            this.cbox_selectSource.DisplayMember = "Abbr";
            this.cbox_selectSource.ValueMember = "ID";
            this.cbox_selectSource.SelectedIndex = 0;
        }

        /// <summary>
        /// Will update textbox from interfaces based on user selected source from source table
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbox_selectSource_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Init. values
            ComboBox currentCbox = sender as ComboBox;
            UniqueSources currentSource = this.cbox_selectSource.SelectedItem as UniqueSources;

            //Fill interface boxes based on chosen database            
            if (currentCbox.SelectedIndex != -1 && !currentSource.Abbr.Contains(Properties.Resources.Message_AddNewsSource) && trigger == true)
            {
                #region Fill interface

                //Write source abbr name
                this.txtbox_SourceAbbrName.Text = currentSource.Abbr;

                //Get a cursor from source table
                string currentSourceID = currentSource.ID;
                string currentSourceQuery = TSourceID + " = '" + currentSourceID + "'";
                ICursor searchCursor = GSC_ProjectEditor.Tables.GetTableCursor("Search", currentSourceQuery, TSource);

                //Get some indexes 
                int sourceIDIndex = searchCursor.FindField(TSourceID);
                int sourceDOIIndex = searchCursor.FindField(TSourceDOI);
                int sourceAbbrIndex = searchCursor.FindField(TSourceAbbr);
                int sourceNameIndex = searchCursor.FindField(TSourceName);
                int sourceRemarksIndex = searchCursor.FindField(TSourceRemarks);
                int sourcePathIndex = searchCursor.FindField(TSourceFilePath);
                int sourceExtendedIndex = searchCursor.FindField(TSourceExtended);

                //Start cursor
                IRow currentRow = searchCursor.NextRow();
                while (currentRow != null)
                {
                    //Get some valid information
                    this.txtbox_SourceDOI.Text = currentRow.get_Value(sourceDOIIndex).ToString();
                    this.txtbox_SourceFullName.Text = currentRow.get_Value(sourceNameIndex).ToString();
                    this.txtBox_SourcePath.Text = currentRow.get_Value(sourcePathIndex).ToString();
                    this.txtbox_SourceRemarks.Text = currentRow.get_Value(sourceRemarksIndex).ToString();
                    this.txtbox_SourceExtension.Text = currentRow.get_Value(sourceExtendedIndex).ToString();

                    currentRow = searchCursor.NextRow();
                }

                //Release the cursor or else some lock could happen.
                System.Runtime.InteropServices.Marshal.ReleaseComObject(searchCursor);

                #endregion
            }
            else
            {

                //Empty textboxes
                this.txtbox_SourceDOI.Text = string.Empty;
                this.txtbox_SourceFullName.Text = string.Empty;
                this.txtBox_SourcePath.Text = string.Empty;
                this.txtbox_SourceRemarks.Text = string.Empty;
                this.txtbox_SourceAbbrName.Text = string.Empty;
                this.txtbox_SourceExtension.Text = string.Empty;

            }
        }

        /// <summary>
        /// Will prompt a dialog for user to select an existing raster datasets, to set as data source
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_SourcePrompt_Click(object sender, EventArgs e)
        {
            //Variables to show custom files within ESRI Arc Catalog
            m_pApp = (IGxApplication)GSC_ProjectEditor.ArcMap.Application;
            IGxCatalog pCat = null;
            IGxFileFilter pFileFilter = null;
            IEnumGxObject pSelection = null;
            IGxDialog pDlg = null;
            IGxObjectFilter pFilter = null;

            try
            {
                //Set catalog to show custom filters
                pDlg = new GxDialog();
                pCat = pDlg.InternalCatalog;
                pFileFilter = pCat.FileFilter;
                if (pFileFilter.FindFileType("pdf") < 0)
                {
                    pFileFilter.AddFileType("PDF", "Adobe PDF file", "");
                }
                if (pFileFilter.FindFileType("doc") < 0)
                {
                    pFileFilter.AddFileType("DOC", "Microsoft Work Document file", "");
                }
                if (pFileFilter.FindFileType("pdf") < 0)
                {
                    pFileFilter.AddFileType("DOCX", "Microsoft Work Document file", "");
                }
                pFilter = new CustomGxFilter();


                IGxObjectFilterCollection currentCollection = pDlg as IGxObjectFilterCollection;

                currentCollection.AddFilter(pFilter, false);
                currentCollection.AddFilter(new GxFilterBasicTypes(), false);
                currentCollection.AddFilter(new GxFilterFiles(), false);
                currentCollection.AddFilter(new GxFilterFileGeodatabases(), false);
                currentCollection.AddFilter(new GxFilterDatasets(), false);

                string sourcePath = "";

                //Open dialog and retrieve user's answer
                if (pDlg.DoModalOpen(this.Handle.ToInt32(), out pSelection) && pSelection != null)
                {
                    IGxObject currentObj = pSelection.Next();
                    sourcePath = currentObj.FullName;
                }

                //Add to textbox
                this.txtBox_SourcePath.Text = sourcePath;
            }
            catch (Exception customFilterExpcetion)
            {

                MessageBox.Show(customFilterExpcetion.Message + "; " + customFilterExpcetion.StackTrace);
            }


        }

        /// <summary>
        /// Will clear all values from interfaces when button is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_ClearSourceBoxes_Click(object sender, EventArgs e)
        {
            clearBoxes();
        }

        /// <summary>
        /// Will clean interfaces from any values
        /// </summary>
        public void clearBoxes()
        {

            //Clear textboxes
            this.txtbox_SourceDOI.Text = string.Empty;
            this.txtbox_SourceFullName.Text = string.Empty;
            this.txtBox_SourcePath.Text = string.Empty;
            this.txtbox_SourceRemarks.Text = string.Empty;
            this.txtbox_SourceAbbrName.Text = string.Empty;
            this.txtbox_SourceExtension.Text = string.Empty;
        }

        /// <summary>
        /// Will modify or add a new source when button is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_AddSource_Click(object sender, EventArgs e)
        {
            //Init. values
            UniqueSources currentSources = this.cbox_selectSource.SelectedItem as UniqueSources;

            //Main process           
            if (this.cbox_selectSource.SelectedIndex != -1 && this.txtbox_SourceFullName.Text != "" && this.txtbox_SourceAbbrName.Text != "")
            {
                #region Update source table
                //Init
                Dictionary<string, object> newFieldValues = new Dictionary<string, object>();

                //Get other informations
                newFieldValues[TSourceAbbr] = this.txtbox_SourceAbbrName.Text;
                newFieldValues[TSourceDOI] = this.txtbox_SourceDOI.Text;
                newFieldValues[TSourceName] = this.txtbox_SourceFullName.Text;
                newFieldValues[TSourceRemarks] = this.txtbox_SourceRemarks.Text;
                newFieldValues[TSourceExtended] = this.txtbox_SourceExtension.Text;

                if (this.txtBox_SourcePath.Text != "")
                {
                    newFieldValues[TSourceFilePath] = this.txtBox_SourcePath.Text;
                }

                //Detect if update or new row is needed
                if (currentSources.Abbr.Contains(Properties.Resources.Message_AddNewsSource))
                {

                    //Add other field to dico
                    int newSourceID = GSC_ProjectEditor.IDs.CalculateIDFromCount(TSource, null);

                    //Add new domain value, but validate id first, to prevent mistmatch between table and domain
                    Dictionary<string, string> domSourceDico = GSC_ProjectEditor.Domains.GetDomDico(DSourceRef, "Code");
                    while (domSourceDico.ContainsKey(newSourceID.ToString()))
                    {
                        newSourceID = newSourceID + 1;
                    }


                    //Add new domain value
                    GSC_ProjectEditor.Domains.AddDomainValue(DSourceRef, newSourceID.ToString(), this.txtbox_SourceAbbrName.Text);

                    //Add new row
                    newFieldValues[TSourceID] = newSourceID.ToString();
                    GSC_ProjectEditor.Tables.AddRowWithValues(TSource, newFieldValues);

                }
                else
                {

                    //update current row with some new values for empty fields
                    string currentSourceQuery = TSourceID + " = '" + currentSources.ID + "'";

                    //Update row
                    GSC_ProjectEditor.Tables.UpdateMultipleFieldValue(TSource, currentSourceQuery, newFieldValues);

                    //Update domain if needed
                    Dictionary<string, string> domSourceDico = GSC_ProjectEditor.Domains.GetDomDico(DSourceRef, "Code");
                    if (domSourceDico.ContainsKey(currentSources.ID))
                    {
                        if (domSourceDico[currentSources.ID] != this.txtbox_SourceAbbrName.Text)
                        {
                            //Update domain
                            GSC_ProjectEditor.Domains.UpdateDomainDescription(DSourceRef, currentSources.ID, this.txtbox_SourceAbbrName.Text);
                        }

                    }
                }
                #endregion

                clearBoxes();

                fillSourceCombobox();

                Properties.Settings.Default.refreshSource = true;
                Properties.Settings.Default.Save();
            }
            else
            {
                MessageBox.Show(Properties.Resources.EmptyFields);
            }
        }

        /// <summary>
        /// Open a bigger form for the user to have more space.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void txtbox_SourceRemarks_MouseDoubleClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            //Create a new form to enter more text
            Form_Generic_InputLongTextBox getNewForm = new Form_Generic_InputLongTextBox();

            //Sent current textbox text into new form tag
            getNewForm.Tag = this.txtbox_SourceRemarks.Text;

            //Show form
            getNewForm.Show();

            //Get any event coming from the form paste button
            getNewForm.pasteButtonPushed += new Form_Generic_InputLongTextBox.pasteButtonEventHandler(newForm_pasteButtonPushedForResume);
        }

        /// <summary>
        /// If there is any event that came from the paste button for the resume textbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void newForm_pasteButtonPushedForResume(object sender, EventArgs e)
        {
            //Cast incoming object
            TextBox resumeLongText = sender as TextBox;

            PasteButtonResume(resumeLongText, this.txtbox_SourceRemarks);
        }

        public void txtbox_SourceExtension_MouseDoubleClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            //Create a new form to enter more text
            Form_Generic_InputLongTextBox getExtendedForm = new Form_Generic_InputLongTextBox();

            //Sent current textbox text into new form tag
            getExtendedForm.Tag = this.txtbox_SourceExtension.Text;

            //Show form
            getExtendedForm.Show();

            //Get any event coming from the form paste button
            getExtendedForm.pasteButtonPushed += new Form_Generic_InputLongTextBox.pasteButtonEventHandler(getExtendedForm_pasteButtonPushed);
        }

        /// <summary>
        /// Will add information inside extended information textbox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void getExtendedForm_pasteButtonPushed(object sender, EventArgs e)
        {
            //Cast incoming object
            TextBox resumeLongText = sender as TextBox;

            PasteButtonResume(resumeLongText, this.txtbox_SourceExtension);
        }

        /// <summary>
        /// Will take text from an input text box and put inside another text box control.
        /// </summary>
        /// <param name="inputTextBox"></param>
        /// <param name="outputTextBox"></param>
        public void PasteButtonResume(TextBox inputTextBox, TextBox outputTextBox)
        {
            //Past text into interface
            outputTextBox.Text = inputTextBox.Text;
        }
    }
}
