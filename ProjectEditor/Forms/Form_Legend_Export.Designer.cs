namespace GSC_ProjectEditor
{
    partial class Form_Legend_Export
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_Legend_Export));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.checkedListBox_Maps = new System.Windows.Forms.CheckedListBox();
            this.btn_Cancel = new System.Windows.Forms.Button();
            this.btn_ExportLegend = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.checkedListBox_Maps);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // checkedListBox_Maps
            // 
            this.checkedListBox_Maps.CheckOnClick = true;
            this.checkedListBox_Maps.FormattingEnabled = true;
            resources.ApplyResources(this.checkedListBox_Maps, "checkedListBox_Maps");
            this.checkedListBox_Maps.Name = "checkedListBox_Maps";
            this.checkedListBox_Maps.TabStop = false;
            // 
            // btn_Cancel
            // 
            this.btn_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.btn_Cancel, "btn_Cancel");
            this.btn_Cancel.Name = "btn_Cancel";
            this.btn_Cancel.UseVisualStyleBackColor = true;
            this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click_1);
            // 
            // btn_ExportLegend
            // 
            resources.ApplyResources(this.btn_ExportLegend, "btn_ExportLegend");
            this.btn_ExportLegend.Name = "btn_ExportLegend";
            this.btn_ExportLegend.UseVisualStyleBackColor = true;
            this.btn_ExportLegend.Click += new System.EventHandler(this.btn_ExportLegend_Click);
            // 
            // Form_Legend_Export
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btn_Cancel);
            this.Controls.Add(this.btn_ExportLegend);
            this.Controls.Add(this.groupBox1);
            this.Name = "Form_Legend_Export";
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckedListBox checkedListBox_Maps;
        private System.Windows.Forms.Button btn_Cancel;
        private System.Windows.Forms.Button btn_ExportLegend;
    }
}