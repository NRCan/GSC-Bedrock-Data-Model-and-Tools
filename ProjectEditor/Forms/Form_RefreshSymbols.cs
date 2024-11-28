using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GSC_ProjectEditor
{
    public partial class Form_RefreshSymbols : Form
    {

        #region Main Variables

        public delegate void refreshButtonEventHandler(object sender, EventArgs e); //A delegate for execution events
        public event refreshButtonEventHandler refreshButtonPushed; //This event is triggered when a new process is started with the execution button

        #endregion

        public Form_RefreshSymbols()
        {

            //Set culture before init.
            Utilities.Culture.SetCulture();

            //Init.
            InitializeComponent();

            //if enabled is already open before init.
            if (this.Enabled)
            {
                FormRefreshSymbols_EnabledChanged(this, null);
            }
            else if (!this.Enabled)
            {
                //Init controls if enabled.
                this.EnabledChanged += new EventHandler(FormRefreshSymbols_EnabledChanged);
            }


        }

        /// <summary>
        /// Will initialize the form is edit session is open
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void FormRefreshSymbols_EnabledChanged(object sender, EventArgs e)
        {
            //Set current list to last user choice
            updateListFromSettings();
        }

        /// <summary>
        /// Will update the checklistbox with user settings from last modification
        /// </summary>
        public void updateListFromSettings()
        {
            //For geopoints
            if (Properties.Settings.Default.refreshGeopoints)
            {
                this.checkedListBox_RefreshSymbols.SetItemChecked(0, true);
            }
            else
            {
                this.checkedListBox_RefreshSymbols.SetItemChecked(0, false);
            }

            //For geolines
            if (Properties.Settings.Default.refreshGeolines)
            {
                this.checkedListBox_RefreshSymbols.SetItemChecked(1, true);
            }
            else
            {
                this.checkedListBox_RefreshSymbols.SetItemChecked(1, false);
            }

            //For labels
            if (Properties.Settings.Default.refreshLabels)
            {
                this.checkedListBox_RefreshSymbols.SetItemChecked(2, true);
            }
            else
            {
                this.checkedListBox_RefreshSymbols.SetItemChecked(2, false);
            }

            //For map units
            if (Properties.Settings.Default.refreshMapUnits)
            {
                this.checkedListBox_RefreshSymbols.SetItemChecked(3, true);
            }
            else
            {
                this.checkedListBox_RefreshSymbols.SetItemChecked(3, false);
            }

        }

        /// <summary>
        /// Will call the close form method
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_Cancel_Click(object sender, EventArgs e)
        {
            CloseForm();
        }

        /// <summary>
        /// Will close the form
        /// </summary>
        public void CloseForm()
        {
            this.Close();
        }

        /// <summary>
        /// Will init. events and save user's choice in the setings
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_OK_Click(object sender, EventArgs e)
        {
            //Save choice in settings
            SaveChoice();

            //Trigger the proper events
            refreshButtonPushed(this.checkedListBox_RefreshSymbols as object, e);

            //Close the form
            CloseForm();

        }

        /// <summary>
        /// Will save the user's choice of refreshment in the internal settings
        /// </summary>
        public void SaveChoice()
        {
            //For geopoints
            if (this.checkedListBox_RefreshSymbols.GetItemCheckState(0) == CheckState.Checked)
            {
                Properties.Settings.Default.refreshGeopoints = true;
            }
            else
            {
                Properties.Settings.Default.refreshGeopoints = false;
            }

            //For geolines
            if (this.checkedListBox_RefreshSymbols.GetItemCheckState(1) == CheckState.Checked)
            {
                Properties.Settings.Default.refreshGeolines = true;
            }
            else
            {
                Properties.Settings.Default.refreshGeolines = false;
            }

            //For labels
            if (this.checkedListBox_RefreshSymbols.GetItemCheckState(2) == CheckState.Checked)
            {
                Properties.Settings.Default.refreshLabels = true;
            }
            else
            {
                Properties.Settings.Default.refreshLabels = false;
            }

            //For map units
            if (this.checkedListBox_RefreshSymbols.GetItemCheckState(3) == CheckState.Checked)
            {
                Properties.Settings.Default.refreshMapUnits = true;
            }
            else
            {
                Properties.Settings.Default.refreshMapUnits = false;
            }

            //Save
            Properties.Settings.Default.Save();

        }



    }
}
