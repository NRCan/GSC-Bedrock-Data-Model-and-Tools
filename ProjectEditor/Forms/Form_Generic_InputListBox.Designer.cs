namespace GSC_ProjectEditor
{
    partial class Form_Generic_InputListBox
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.button_Cancel = new System.Windows.Forms.Button();
            this.button_OK = new System.Windows.Forms.Button();
            this.label_Generic = new System.Windows.Forms.Label();
            this.listBox_Generic = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // button_Cancel
            // 
            this.button_Cancel.Location = new System.Drawing.Point(222, 216);
            this.button_Cancel.Name = "button_Cancel";
            this.button_Cancel.Size = new System.Drawing.Size(75, 23);
            this.button_Cancel.TabIndex = 7;
            this.button_Cancel.Text = "Cancel";
            this.button_Cancel.UseVisualStyleBackColor = true;
            this.button_Cancel.Click += new System.EventHandler(this.button_Cancel_Click);
            // 
            // button_OK
            // 
            this.button_OK.Location = new System.Drawing.Point(15, 216);
            this.button_OK.Name = "button_OK";
            this.button_OK.Size = new System.Drawing.Size(75, 23);
            this.button_OK.TabIndex = 6;
            this.button_OK.Text = "Ok";
            this.button_OK.UseVisualStyleBackColor = true;
            this.button_OK.Click += new System.EventHandler(this.button_OK_Click);
            // 
            // label_Generic
            // 
            this.label_Generic.AutoSize = true;
            this.label_Generic.Location = new System.Drawing.Point(12, 9);
            this.label_Generic.Name = "label_Generic";
            this.label_Generic.Size = new System.Drawing.Size(55, 13);
            this.label_Generic.TabIndex = 4;
            this.label_Generic.Text = "Input Text";
            // 
            // listBox_Generic
            // 
            this.listBox_Generic.FormattingEnabled = true;
            this.listBox_Generic.HorizontalScrollbar = true;
            this.listBox_Generic.Location = new System.Drawing.Point(15, 25);
            this.listBox_Generic.Name = "listBox_Generic";
            this.listBox_Generic.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple;
            this.listBox_Generic.Size = new System.Drawing.Size(282, 173);
            this.listBox_Generic.TabIndex = 8;
            // 
            // FormGenericListBoxInput
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(313, 251);
            this.Controls.Add(this.listBox_Generic);
            this.Controls.Add(this.button_Cancel);
            this.Controls.Add(this.button_OK);
            this.Controls.Add(this.label_Generic);
            this.Name = "Form_Generic_InputListBox";
            this.Text = "Form_Generic_InputListBox";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button_Cancel;
        private System.Windows.Forms.Button button_OK;
        private System.Windows.Forms.Label label_Generic;
        private System.Windows.Forms.ListBox listBox_Generic;
    }
}