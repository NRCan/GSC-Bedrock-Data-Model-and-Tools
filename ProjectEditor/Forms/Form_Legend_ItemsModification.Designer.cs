namespace GSC_ProjectEditor
{
    partial class Form_Legend_ItemsModification
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_Legend_ItemsModification));
            GSC_ProjectEditor.Properties.Settings settings1 = new GSC_ProjectEditor.Properties.Settings();
            GSC_ProjectEditor.Properties.Settings settings2 = new GSC_ProjectEditor.Properties.Settings();
            GSC_ProjectEditor.Properties.Settings settings3 = new GSC_ProjectEditor.Properties.Settings();
            this.label_MapUnitAnno = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox_Update = new System.Windows.Forms.GroupBox();
            this.btn_RemoveItem = new System.Windows.Forms.Button();
            this.btn_ModifyItem = new System.Windows.Forms.Button();
            this.txtBox_NewLabel = new System.Windows.Forms.TextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.checkBox_Overprint = new System.Windows.Forms.CheckBox();
            this.cbox_SelectItemType = new System.Windows.Forms.ComboBox();
            this.txtbox_MapUnitAnno = new System.Windows.Forms.TextBox();
            this.label_SelectItemType = new System.Windows.Forms.Label();
            this.txtBox_ArcGISDisplay = new System.Windows.Forms.TextBox();
            this.cbox_selectItem = new System.Windows.Forms.ComboBox();
            this.groupBox_ArcDisplay = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label_mapUnits = new System.Windows.Forms.Label();
            this.textBox_mapUnits = new System.Windows.Forms.TextBox();
            this.label_SelectLabel = new System.Windows.Forms.Label();
            this.tabctrl_MapUnit = new System.Windows.Forms.TabControl();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.groupBox_Symbol = new System.Windows.Forms.GroupBox();
            this.txtbox_MapUnitLegendSymbol = new System.Windows.Forms.TextBox();
            this.label_MapUnitLegendSymbol = new System.Windows.Forms.Label();
            this.pct_MapUnitColor = new System.Windows.Forms.PictureBox();
            this.groupBox_header = new System.Windows.Forms.GroupBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.radioButton_H1 = new System.Windows.Forms.RadioButton();
            this.radioButton_H2 = new System.Windows.Forms.RadioButton();
            this.radioButton_H3 = new System.Windows.Forms.RadioButton();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.grp_NewMapUnit = new System.Windows.Forms.GroupBox();
            this.txtbox_MapUnitName = new System.Windows.Forms.TextBox();
            this.label_MapUnitName = new System.Windows.Forms.Label();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.grp_GeolRank = new System.Windows.Forms.GroupBox();
            this.cbox_GeolRank = new System.Windows.Forms.ComboBox();
            this.label_geolrank = new System.Windows.Forms.Label();
            this.btn_MapUnitClearBoxes = new System.Windows.Forms.Button();
            this.button_AddItemType = new System.Windows.Forms.Button();
            this.groupBox_Update.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox_ArcDisplay.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.tabctrl_MapUnit.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.groupBox_Symbol.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pct_MapUnitColor)).BeginInit();
            this.groupBox_header.SuspendLayout();
            this.panel1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.grp_NewMapUnit.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.grp_GeolRank.SuspendLayout();
            this.SuspendLayout();
            // 
            // label_MapUnitAnno
            // 
            resources.ApplyResources(this.label_MapUnitAnno, "label_MapUnitAnno");
            this.label_MapUnitAnno.Name = "label_MapUnitAnno";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // groupBox_Update
            // 
            this.groupBox_Update.Controls.Add(this.btn_RemoveItem);
            this.groupBox_Update.Controls.Add(this.btn_ModifyItem);
            resources.ApplyResources(this.groupBox_Update, "groupBox_Update");
            this.groupBox_Update.Name = "groupBox_Update";
            this.groupBox_Update.TabStop = false;
            // 
            // btn_RemoveItem
            // 
            resources.ApplyResources(this.btn_RemoveItem, "btn_RemoveItem");
            this.btn_RemoveItem.Name = "btn_RemoveItem";
            this.btn_RemoveItem.TabStop = false;
            this.btn_RemoveItem.UseVisualStyleBackColor = true;
            this.btn_RemoveItem.Click += new System.EventHandler(this.btn_RemoveItem_Click);
            // 
            // btn_ModifyItem
            // 
            resources.ApplyResources(this.btn_ModifyItem, "btn_ModifyItem");
            this.btn_ModifyItem.Name = "btn_ModifyItem";
            this.btn_ModifyItem.TabStop = false;
            this.btn_ModifyItem.UseVisualStyleBackColor = true;
            this.btn_ModifyItem.Click += new System.EventHandler(this.btn_ModifyItem_Click);
            // 
            // txtBox_NewLabel
            // 
            resources.ApplyResources(this.txtBox_NewLabel, "txtBox_NewLabel");
            this.txtBox_NewLabel.Name = "txtBox_NewLabel";
            // 
            // groupBox3
            // 
            this.groupBox3.BackColor = System.Drawing.Color.Transparent;
            this.groupBox3.Controls.Add(this.checkBox_Overprint);
            this.groupBox3.Controls.Add(this.txtBox_NewLabel);
            this.groupBox3.Controls.Add(this.label2);
            settings1.Annick = false;
            settings1.Culture = new System.Globalization.CultureInfo("en");
            settings1.dwEnabling = true;
            settings1.Ganfeld_SelectedGeolcode = "";
            settings1.Ganfeld_SelectedYear = new System.DateTime(2014, 9, 4, 13, 19, 45, 0);
            settings1.ganfeldEditing = true;
            settings1.KeepCustomSymbols = false;
            settings1.ParticipantID = "Empty";
            settings1.RefreshButton = false;
            settings1.refreshGeolines = true;
            settings1.refreshGeopoints = true;
            settings1.refreshLabels = true;
            settings1.refreshMapUnits = true;
            settings1.SaveEditButton = false;
            settings1.SettingsKey = "";
            settings1.SourceID = "Empty";
            this.groupBox3.DataBindings.Add(new System.Windows.Forms.Binding("Enabled", settings1, "dwEnabling", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            resources.ApplyResources(this.groupBox3, "groupBox3");
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.TabStop = false;
            // 
            // checkBox_Overprint
            // 
            resources.ApplyResources(this.checkBox_Overprint, "checkBox_Overprint");
            this.checkBox_Overprint.Name = "checkBox_Overprint";
            this.checkBox_Overprint.UseVisualStyleBackColor = true;
            this.checkBox_Overprint.CheckedChanged += new System.EventHandler(this.checkBox_Overprint_CheckedChanged);
            // 
            // cbox_SelectItemType
            // 
            this.cbox_SelectItemType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbox_SelectItemType.FormattingEnabled = true;
            resources.ApplyResources(this.cbox_SelectItemType, "cbox_SelectItemType");
            this.cbox_SelectItemType.Name = "cbox_SelectItemType";
            this.cbox_SelectItemType.SelectedIndexChanged += new System.EventHandler(this.cbox_SelectItemType_SelectedIndexChanged);
            this.cbox_SelectItemType.SelectionChangeCommitted += new System.EventHandler(this.cbox_SelectItemType_SelectionChangeCommitted);
            // 
            // txtbox_MapUnitAnno
            // 
            resources.ApplyResources(this.txtbox_MapUnitAnno, "txtbox_MapUnitAnno");
            this.txtbox_MapUnitAnno.Name = "txtbox_MapUnitAnno";
            // 
            // label_SelectItemType
            // 
            resources.ApplyResources(this.label_SelectItemType, "label_SelectItemType");
            this.label_SelectItemType.Name = "label_SelectItemType";
            // 
            // txtBox_ArcGISDisplay
            // 
            resources.ApplyResources(this.txtBox_ArcGISDisplay, "txtBox_ArcGISDisplay");
            this.txtBox_ArcGISDisplay.Name = "txtBox_ArcGISDisplay";
            // 
            // cbox_selectItem
            // 
            this.cbox_selectItem.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbox_selectItem.DropDownWidth = 363;
            this.cbox_selectItem.FormattingEnabled = true;
            resources.ApplyResources(this.cbox_selectItem, "cbox_selectItem");
            this.cbox_selectItem.Name = "cbox_selectItem";
            this.cbox_selectItem.SelectedIndexChanged += new System.EventHandler(this.cbox_selectItem_SelectedIndexChanged);
            // 
            // groupBox_ArcDisplay
            // 
            this.groupBox_ArcDisplay.Controls.Add(this.txtBox_ArcGISDisplay);
            this.groupBox_ArcDisplay.Controls.Add(this.label1);
            resources.ApplyResources(this.groupBox_ArcDisplay, "groupBox_ArcDisplay");
            this.groupBox_ArcDisplay.Name = "groupBox_ArcDisplay";
            this.groupBox_ArcDisplay.TabStop = false;
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // groupBox2
            // 
            this.groupBox2.BackColor = System.Drawing.Color.Transparent;
            this.groupBox2.Controls.Add(this.label_mapUnits);
            this.groupBox2.Controls.Add(this.textBox_mapUnits);
            this.groupBox2.Controls.Add(this.label_MapUnitAnno);
            this.groupBox2.Controls.Add(this.txtbox_MapUnitAnno);
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // label_mapUnits
            // 
            resources.ApplyResources(this.label_mapUnits, "label_mapUnits");
            this.label_mapUnits.Name = "label_mapUnits";
            // 
            // textBox_mapUnits
            // 
            resources.ApplyResources(this.textBox_mapUnits, "textBox_mapUnits");
            this.textBox_mapUnits.Name = "textBox_mapUnits";
            // 
            // label_SelectLabel
            // 
            resources.ApplyResources(this.label_SelectLabel, "label_SelectLabel");
            this.label_SelectLabel.Name = "label_SelectLabel";
            // 
            // tabctrl_MapUnit
            // 
            this.tabctrl_MapUnit.Controls.Add(this.tabPage2);
            this.tabctrl_MapUnit.Controls.Add(this.tabPage1);
            this.tabctrl_MapUnit.Controls.Add(this.tabPage4);
            resources.ApplyResources(this.tabctrl_MapUnit, "tabctrl_MapUnit");
            this.tabctrl_MapUnit.Name = "tabctrl_MapUnit";
            this.tabctrl_MapUnit.SelectedIndex = 0;
            // 
            // tabPage2
            // 
            this.tabPage2.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage2.Controls.Add(this.groupBox_ArcDisplay);
            this.tabPage2.Controls.Add(this.groupBox_Symbol);
            this.tabPage2.Controls.Add(this.groupBox_header);
            resources.ApplyResources(this.tabPage2, "tabPage2");
            this.tabPage2.Name = "tabPage2";
            // 
            // groupBox_Symbol
            // 
            this.groupBox_Symbol.Controls.Add(this.txtbox_MapUnitLegendSymbol);
            this.groupBox_Symbol.Controls.Add(this.label_MapUnitLegendSymbol);
            this.groupBox_Symbol.Controls.Add(this.pct_MapUnitColor);
            resources.ApplyResources(this.groupBox_Symbol, "groupBox_Symbol");
            this.groupBox_Symbol.Name = "groupBox_Symbol";
            this.groupBox_Symbol.TabStop = false;
            // 
            // txtbox_MapUnitLegendSymbol
            // 
            resources.ApplyResources(this.txtbox_MapUnitLegendSymbol, "txtbox_MapUnitLegendSymbol");
            this.txtbox_MapUnitLegendSymbol.Name = "txtbox_MapUnitLegendSymbol";
            this.txtbox_MapUnitLegendSymbol.TextChanged += new System.EventHandler(this.txtbox_MapUnitLegendSymbol_TextChanged);
            // 
            // label_MapUnitLegendSymbol
            // 
            resources.ApplyResources(this.label_MapUnitLegendSymbol, "label_MapUnitLegendSymbol");
            this.label_MapUnitLegendSymbol.Name = "label_MapUnitLegendSymbol";
            // 
            // pct_MapUnitColor
            // 
            resources.ApplyResources(this.pct_MapUnitColor, "pct_MapUnitColor");
            this.pct_MapUnitColor.Name = "pct_MapUnitColor";
            this.pct_MapUnitColor.TabStop = false;
            this.pct_MapUnitColor.DoubleClick += new System.EventHandler(this.pct_MapUnitColor_Click);
            // 
            // groupBox_header
            // 
            this.groupBox_header.BackColor = System.Drawing.Color.Transparent;
            this.groupBox_header.Controls.Add(this.panel1);
            resources.ApplyResources(this.groupBox_header, "groupBox_header");
            this.groupBox_header.Name = "groupBox_header";
            this.groupBox_header.TabStop = false;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.radioButton_H1);
            this.panel1.Controls.Add(this.radioButton_H2);
            this.panel1.Controls.Add(this.radioButton_H3);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // radioButton_H1
            // 
            resources.ApplyResources(this.radioButton_H1, "radioButton_H1");
            this.radioButton_H1.Checked = true;
            this.radioButton_H1.Name = "radioButton_H1";
            this.radioButton_H1.TabStop = true;
            this.radioButton_H1.UseVisualStyleBackColor = true;
            // 
            // radioButton_H2
            // 
            resources.ApplyResources(this.radioButton_H2, "radioButton_H2");
            this.radioButton_H2.Name = "radioButton_H2";
            this.radioButton_H2.UseVisualStyleBackColor = true;
            // 
            // radioButton_H3
            // 
            resources.ApplyResources(this.radioButton_H3, "radioButton_H3");
            this.radioButton_H3.Name = "radioButton_H3";
            this.radioButton_H3.UseVisualStyleBackColor = true;
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage1.Controls.Add(this.grp_NewMapUnit);
            resources.ApplyResources(this.tabPage1, "tabPage1");
            this.tabPage1.Name = "tabPage1";
            // 
            // grp_NewMapUnit
            // 
            this.grp_NewMapUnit.BackColor = System.Drawing.Color.Transparent;
            this.grp_NewMapUnit.Controls.Add(this.txtbox_MapUnitName);
            this.grp_NewMapUnit.Controls.Add(this.label_MapUnitName);
            settings2.Annick = false;
            settings2.Culture = new System.Globalization.CultureInfo("en");
            settings2.dwEnabling = true;
            settings2.Ganfeld_SelectedGeolcode = "";
            settings2.Ganfeld_SelectedYear = new System.DateTime(2014, 9, 4, 13, 19, 45, 0);
            settings2.ganfeldEditing = true;
            settings2.KeepCustomSymbols = false;
            settings2.ParticipantID = "Empty";
            settings2.RefreshButton = false;
            settings2.refreshGeolines = true;
            settings2.refreshGeopoints = true;
            settings2.refreshLabels = true;
            settings2.refreshMapUnits = true;
            settings2.SaveEditButton = false;
            settings2.SettingsKey = "";
            settings2.SourceID = "Empty";
            this.grp_NewMapUnit.DataBindings.Add(new System.Windows.Forms.Binding("Enabled", settings2, "dwEnabling", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            resources.ApplyResources(this.grp_NewMapUnit, "grp_NewMapUnit");
            this.grp_NewMapUnit.Name = "grp_NewMapUnit";
            this.grp_NewMapUnit.TabStop = false;
            // 
            // txtbox_MapUnitName
            // 
            resources.ApplyResources(this.txtbox_MapUnitName, "txtbox_MapUnitName");
            this.txtbox_MapUnitName.Name = "txtbox_MapUnitName";
            // 
            // label_MapUnitName
            // 
            resources.ApplyResources(this.label_MapUnitName, "label_MapUnitName");
            this.label_MapUnitName.Name = "label_MapUnitName";
            // 
            // tabPage4
            // 
            this.tabPage4.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage4.Controls.Add(this.grp_GeolRank);
            this.tabPage4.Controls.Add(this.groupBox2);
            this.tabPage4.Controls.Add(this.groupBox3);
            resources.ApplyResources(this.tabPage4, "tabPage4");
            this.tabPage4.Name = "tabPage4";
            // 
            // grp_GeolRank
            // 
            this.grp_GeolRank.BackColor = System.Drawing.Color.Transparent;
            this.grp_GeolRank.Controls.Add(this.cbox_GeolRank);
            this.grp_GeolRank.Controls.Add(this.label_geolrank);
            settings3.Annick = false;
            settings3.Culture = new System.Globalization.CultureInfo("en");
            settings3.dwEnabling = true;
            settings3.Ganfeld_SelectedGeolcode = "";
            settings3.Ganfeld_SelectedYear = new System.DateTime(2014, 9, 4, 13, 19, 45, 0);
            settings3.ganfeldEditing = true;
            settings3.KeepCustomSymbols = false;
            settings3.ParticipantID = "Empty";
            settings3.RefreshButton = false;
            settings3.refreshGeolines = true;
            settings3.refreshGeopoints = true;
            settings3.refreshLabels = true;
            settings3.refreshMapUnits = true;
            settings3.SaveEditButton = false;
            settings3.SettingsKey = "";
            settings3.SourceID = "Empty";
            this.grp_GeolRank.DataBindings.Add(new System.Windows.Forms.Binding("Enabled", settings3, "dwEnabling", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            resources.ApplyResources(this.grp_GeolRank, "grp_GeolRank");
            this.grp_GeolRank.Name = "grp_GeolRank";
            this.grp_GeolRank.TabStop = false;
            // 
            // cbox_GeolRank
            // 
            this.cbox_GeolRank.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbox_GeolRank.FormattingEnabled = true;
            this.cbox_GeolRank.Items.AddRange(new object[] {
            resources.GetString("cbox_GeolRank.Items")});
            resources.ApplyResources(this.cbox_GeolRank, "cbox_GeolRank");
            this.cbox_GeolRank.Name = "cbox_GeolRank";
            // 
            // label_geolrank
            // 
            resources.ApplyResources(this.label_geolrank, "label_geolrank");
            this.label_geolrank.Name = "label_geolrank";
            // 
            // btn_MapUnitClearBoxes
            // 
            resources.ApplyResources(this.btn_MapUnitClearBoxes, "btn_MapUnitClearBoxes");
            this.btn_MapUnitClearBoxes.Name = "btn_MapUnitClearBoxes";
            this.btn_MapUnitClearBoxes.TabStop = false;
            this.btn_MapUnitClearBoxes.UseVisualStyleBackColor = true;
            this.btn_MapUnitClearBoxes.Click += new System.EventHandler(this.btn_MapUnitClearBoxes_Click);
            // 
            // button_AddItemType
            // 
            resources.ApplyResources(this.button_AddItemType, "button_AddItemType");
            this.button_AddItemType.Name = "button_AddItemType";
            this.button_AddItemType.UseVisualStyleBackColor = true;
            this.button_AddItemType.Click += new System.EventHandler(this.button_AddItemType_Click);
            // 
            // FormLegendItems
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.button_AddItemType);
            this.Controls.Add(this.groupBox_Update);
            this.Controls.Add(this.cbox_SelectItemType);
            this.Controls.Add(this.label_SelectItemType);
            this.Controls.Add(this.cbox_selectItem);
            this.Controls.Add(this.label_SelectLabel);
            this.Controls.Add(this.tabctrl_MapUnit);
            this.Controls.Add(this.btn_MapUnitClearBoxes);
            this.DataBindings.Add(new System.Windows.Forms.Binding("Enabled", global::GSC_ProjectEditor.Properties.Settings.Default, "dwEnabling", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.Enabled = global::GSC_ProjectEditor.Properties.Settings.Default.dwEnabling;
            this.Name = "Form_Legend_ItemsModification";
            this.TopMost = true;
            this.groupBox_Update.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox_ArcDisplay.ResumeLayout(false);
            this.groupBox_ArcDisplay.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.tabctrl_MapUnit.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.groupBox_Symbol.ResumeLayout(false);
            this.groupBox_Symbol.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pct_MapUnitColor)).EndInit();
            this.groupBox_header.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.tabPage1.ResumeLayout(false);
            this.grp_NewMapUnit.ResumeLayout(false);
            this.grp_NewMapUnit.PerformLayout();
            this.tabPage4.ResumeLayout(false);
            this.grp_GeolRank.ResumeLayout(false);
            this.grp_GeolRank.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label_MapUnitAnno;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox_Update;
        private System.Windows.Forms.Button btn_RemoveItem;
        private System.Windows.Forms.Button btn_ModifyItem;
        private System.Windows.Forms.TextBox txtBox_NewLabel;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.ComboBox cbox_SelectItemType;
        private System.Windows.Forms.TextBox txtbox_MapUnitAnno;
        private System.Windows.Forms.Label label_SelectItemType;
        private System.Windows.Forms.TextBox txtBox_ArcGISDisplay;
        private System.Windows.Forms.ComboBox cbox_selectItem;
        private System.Windows.Forms.GroupBox groupBox_ArcDisplay;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label_SelectLabel;
        private System.Windows.Forms.TabControl tabctrl_MapUnit;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.GroupBox grp_NewMapUnit;
        private System.Windows.Forms.TextBox txtbox_MapUnitName;
        private System.Windows.Forms.Label label_MapUnitName;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.GroupBox groupBox_Symbol;
        private System.Windows.Forms.TextBox txtbox_MapUnitLegendSymbol;
        private System.Windows.Forms.Label label_MapUnitLegendSymbol;
        private System.Windows.Forms.PictureBox pct_MapUnitColor;
        private System.Windows.Forms.GroupBox groupBox_header;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.RadioButton radioButton_H1;
        private System.Windows.Forms.RadioButton radioButton_H2;
        private System.Windows.Forms.RadioButton radioButton_H3;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.GroupBox grp_GeolRank;
        private System.Windows.Forms.ComboBox cbox_GeolRank;
        private System.Windows.Forms.Label label_geolrank;
        private System.Windows.Forms.Button btn_MapUnitClearBoxes;
        private System.Windows.Forms.Button button_AddItemType;
        private System.Windows.Forms.Label label_mapUnits;
        private System.Windows.Forms.TextBox textBox_mapUnits;
        private System.Windows.Forms.CheckBox checkBox_Overprint;
    }
}