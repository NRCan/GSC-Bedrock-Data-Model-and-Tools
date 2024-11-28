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
    public partial class Form_CreateEdit_CreateMapUnits_OverprintHierarchy : Form
    {
        #region Main Variable

        public int returnLevel { get; set; }

        #endregion

        public Form_CreateEdit_CreateMapUnits_OverprintHierarchy()
        {
            InitializeComponent();

            this.FormClosed += new FormClosedEventHandler(FormOverprintHierarchy_FormClosed);
        }

        //Whenever the form is being closed
        void FormOverprintHierarchy_FormClosed(object sender, FormClosedEventArgs e)
        {
            foreach (Control ctrl in this.groupBox_OPLevel.Controls)
            {
                //Check for radiobuttons
                if (ctrl is RadioButton)
                {
                    //Check for selected radiobuttons
                    RadioButton rb = ctrl as RadioButton;
                    if (rb.Checked)
                    {
                        returnLevel = Convert.ToInt16(rb.Tag);
                    }
                }
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }


    }
}
