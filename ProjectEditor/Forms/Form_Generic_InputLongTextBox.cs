using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Framework;

namespace GSC_ProjectEditor
{
    public partial class Form_Generic_InputLongTextBox : Form
    {
        #region Main Variables

        public delegate void pasteButtonEventHandler(object sender, EventArgs e); //A delegate for execution events
        public event pasteButtonEventHandler pasteButtonPushed; //This event is triggered when a new process is started with the execution button

        #endregion

        /// <summary>
        /// Init.
        /// </summary>
        public Form_Generic_InputLongTextBox()
        {

            //Set culture before init.
            Utilities.Culture.SetCulture();

            InitializeComponent();

            //Get shown event to retrieve received text from dock window.
            this.Shown += new EventHandler(FormLongText_Shown);
        }

        void FormLongText_Shown(object sender, EventArgs e)
        {
            this.textBox_LongText.Text = this.Tag as string;
        }

        /// <summary>
        /// Past button click event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_Paste_Click(object sender, EventArgs e)
        {
            //Call event of paste button pushed
            pasteButtonPushed(this.textBox_LongText as object, null);

            //Close form
            this.Close();

        }

        /// <summary>
        /// Clear button click event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_Clear_Click(object sender, EventArgs e)
        {
            this.textBox_LongText.Text = "";
        }
    }
}
