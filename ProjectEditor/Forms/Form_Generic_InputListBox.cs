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
    public partial class Form_Generic_InputListBox : Form
    {
        #region Main Variables

        public ListBox.SelectedObjectCollection returnValues { get; set; }

        #endregion

        public Form_Generic_InputListBox()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Close form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_Cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Get list box objects and close
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_OK_Click(object sender, EventArgs e)
        {
            returnValues = this.listBox_Generic.SelectedItems;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

    }
}
