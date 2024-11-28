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
    public partial class Form_View_CreateThematicLayers : Form
    {

        #region Main Variables

        public delegate void createButtonEventHandler(object sender, EventArgs e); //A delegate for execution events
        public event createButtonEventHandler createButtonPushed; //This event is triggered when a new process is started with the execution button

        #endregion

        public Form_View_CreateThematicLayers()
        {

            //Set culture before init.
            Utilities.Culture.SetCulture();

            //Init.
            InitializeComponent();
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
        /// Will init. events 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_OK_Click(object sender, EventArgs e)
        {

            //Trigger the proper events
            createButtonPushed(this.checkedListBox_ThemeLayers as object, e);

            //Close the form
            CloseForm();

        }

    }
}
