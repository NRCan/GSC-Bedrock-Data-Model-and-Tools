namespace GSC_ProjectEditor
{
    partial class Dockablewindow_CreateEdit_LabelTemplate
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Dockablewindow_CreateEdit_LabelTemplate));
            this.grp_NewLabelTemplate = new System.Windows.Forms.GroupBox();
            this.txtbox_LabelDescription = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.checkBox_Overprint = new System.Windows.Forms.CheckBox();
            this.txtbox_LabelSymbol = new System.Windows.Forms.TextBox();
            this.pct_MapUnitColor = new System.Windows.Forms.PictureBox();
            this.label4 = new System.Windows.Forms.Label();
            this.btn_AddLabel = new System.Windows.Forms.Button();
            this.groupBox_Options = new System.Windows.Forms.GroupBox();
            this.label_AgePrefix = new System.Windows.Forms.Label();
            this.comboBox_AgePrefix = new System.Windows.Forms.ComboBox();
            this.grp_NewLabelTemplate.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pct_MapUnitColor)).BeginInit();
            this.groupBox_Options.SuspendLayout();
            this.SuspendLayout();
            // 
            // grp_NewLabelTemplate
            // 
            this.grp_NewLabelTemplate.BackColor = System.Drawing.SystemColors.Control;
            this.grp_NewLabelTemplate.Controls.Add(this.txtbox_LabelDescription);
            this.grp_NewLabelTemplate.Controls.Add(this.label1);
            resources.ApplyResources(this.grp_NewLabelTemplate, "grp_NewLabelTemplate");
            this.grp_NewLabelTemplate.Name = "grp_NewLabelTemplate";
            this.grp_NewLabelTemplate.TabStop = false;
            // 
            // txtbox_LabelDescription
            // 
            resources.ApplyResources(this.txtbox_LabelDescription, "txtbox_LabelDescription");
            this.txtbox_LabelDescription.Name = "txtbox_LabelDescription";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // checkBox_Overprint
            // 
            resources.ApplyResources(this.checkBox_Overprint, "checkBox_Overprint");
            this.checkBox_Overprint.Name = "checkBox_Overprint";
            this.checkBox_Overprint.UseVisualStyleBackColor = true;
            this.checkBox_Overprint.CheckedChanged += new System.EventHandler(this.checkBox_Overprint_CheckedChanged);
            // 
            // txtbox_LabelSymbol
            // 
            resources.ApplyResources(this.txtbox_LabelSymbol, "txtbox_LabelSymbol");
            this.txtbox_LabelSymbol.Name = "txtbox_LabelSymbol";
            // 
            // pct_MapUnitColor
            // 
            resources.ApplyResources(this.pct_MapUnitColor, "pct_MapUnitColor");
            this.pct_MapUnitColor.Name = "pct_MapUnitColor";
            this.pct_MapUnitColor.TabStop = false;
            this.pct_MapUnitColor.Click += new System.EventHandler(this.pct_MapUnitColor_Click);
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // btn_AddLabel
            // 
            resources.ApplyResources(this.btn_AddLabel, "btn_AddLabel");
            this.btn_AddLabel.Name = "btn_AddLabel";
            this.btn_AddLabel.TabStop = false;
            this.btn_AddLabel.UseVisualStyleBackColor = true;
            this.btn_AddLabel.Click += new System.EventHandler(this.btn_AddLabel_Click);
            // 
            // groupBox_Options
            // 
            this.groupBox_Options.Controls.Add(this.label_AgePrefix);
            this.groupBox_Options.Controls.Add(this.comboBox_AgePrefix);
            this.groupBox_Options.Controls.Add(this.txtbox_LabelSymbol);
            this.groupBox_Options.Controls.Add(this.checkBox_Overprint);
            this.groupBox_Options.Controls.Add(this.label4);
            this.groupBox_Options.Controls.Add(this.pct_MapUnitColor);
            resources.ApplyResources(this.groupBox_Options, "groupBox_Options");
            this.groupBox_Options.Name = "groupBox_Options";
            this.groupBox_Options.TabStop = false;
            // 
            // label_AgePrefix
            // 
            resources.ApplyResources(this.label_AgePrefix, "label_AgePrefix");
            this.label_AgePrefix.Name = "label_AgePrefix";
            // 
            // comboBox_AgePrefix
            // 
            this.comboBox_AgePrefix.FormattingEnabled = true;
            resources.ApplyResources(this.comboBox_AgePrefix, "comboBox_AgePrefix");
            this.comboBox_AgePrefix.Name = "comboBox_AgePrefix";
            this.comboBox_AgePrefix.SelectedIndexChanged += new System.EventHandler(this.comboBox_AgePrefix_SelectedIndexChanged);
            // 
            // dockablewindowAddLabel
            // 
            resources.ApplyResources(this, "$this");
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.groupBox_Options);
            this.Controls.Add(this.grp_NewLabelTemplate);
            this.Controls.Add(this.btn_AddLabel);
            this.DataBindings.Add(new System.Windows.Forms.Binding("Enabled", global::GSC_ProjectEditor.Properties.Settings.Default, "dwEnabling", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.Enabled = global::GSC_ProjectEditor.Properties.Settings.Default.dwEnabling;
            this.Name = "dockablewindowAddLabel";
            this.grp_NewLabelTemplate.ResumeLayout(false);
            this.grp_NewLabelTemplate.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pct_MapUnitColor)).EndInit();
            this.groupBox_Options.ResumeLayout(false);
            this.groupBox_Options.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox grp_NewLabelTemplate;
        private System.Windows.Forms.TextBox txtbox_LabelSymbol;
        private System.Windows.Forms.TextBox txtbox_LabelDescription;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btn_AddLabel;
        private System.Windows.Forms.PictureBox pct_MapUnitColor;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox checkBox_Overprint;
        private System.Windows.Forms.GroupBox groupBox_Options;
        private System.Windows.Forms.Label label_AgePrefix;
        private System.Windows.Forms.ComboBox comboBox_AgePrefix;

    }
}
