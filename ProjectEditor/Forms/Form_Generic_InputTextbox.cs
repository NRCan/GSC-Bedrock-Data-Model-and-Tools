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
    public partial class Form_Generic_InputTextbox : Form
    {
        #region Main Variables

        public string returnValue { get; set; }

        #endregion

        public Form_Generic_InputTextbox()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Will close the current form.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_Cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Will send the public result variable
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_OK_Click(object sender, EventArgs e)
        {
            returnValue = this.textBox_Generic.Text;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
