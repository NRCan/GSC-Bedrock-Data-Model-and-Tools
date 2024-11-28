namespace GSC_ProjectEditor
{
    partial class DockableWindow_Legend_ItemDescription
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DockableWindow_Legend_ItemDescription));
            this.label_Description = new System.Windows.Forms.Label();
            this.cbox_SelectItemType = new System.Windows.Forms.ComboBox();
            this.label_SelectItemType = new System.Windows.Forms.Label();
            this.txtbox_Description = new System.Windows.Forms.TextBox();
            this.cbox_selectLegendItem = new System.Windows.Forms.ComboBox();
            this.label_SelectLabel = new System.Windows.Forms.Label();
            this.btn_MapDescClearBoxes = new System.Windows.Forms.Button();
            this.btn_AddDescription = new System.Windows.Forms.Button();
            this.checkedListBox_Maps = new System.Windows.Forms.CheckedListBox();
            this.label_MapList = new System.Windows.Forms.Label();
            this.checkBox_AllMaps = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cbox_selectLegendDesc = new System.Windows.Forms.ComboBox();
            this.label_DescriptionCbox = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label_Description
            // 
            resources.ApplyResources(this.label_Description, "label_Description");
            this.label_Description.Name = "label_Description";
            // 
            // cbox_SelectItemType
            // 
            this.cbox_SelectItemType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbox_SelectItemType.FormattingEnabled = true;
            resources.ApplyResources(this.cbox_SelectItemType, "cbox_SelectItemType");
            this.cbox_SelectItemType.Name = "cbox_SelectItemType";
            this.cbox_SelectItemType.SelectedIndexChanged += new System.EventHandler(this.cbox_SelectItemType_SelectedIndexChanged);
            // 
            // label_SelectItemType
            // 
            resources.ApplyResources(this.label_SelectItemType, "label_SelectItemType");
            this.label_SelectItemType.Name = "label_SelectItemType";
            // 
            // txtbox_Description
            // 
            resources.ApplyResources(this.txtbox_Description, "txtbox_Description");
            this.txtbox_Description.Name = "txtbox_Description";
            this.txtbox_Description.DoubleClick += new System.EventHandler(this.txtbox_Description_DoubleClick);
            // 
            // cbox_selectLegendItem
            // 
            this.cbox_selectLegendItem.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbox_selectLegendItem.DropDownWidth = 363;
            this.cbox_selectLegendItem.FormattingEnabled = true;
            resources.ApplyResources(this.cbox_selectLegendItem, "cbox_selectLegendItem");
            this.cbox_selectLegendItem.Name = "cbox_selectLegendItem";
            this.cbox_selectLegendItem.SelectedIndexChanged += new System.EventHandler(this.cbox_selectLegendItem_SelectedIndexChanged);
            // 
            // label_SelectLabel
            // 
            resources.ApplyResources(this.label_SelectLabel, "label_SelectLabel");
            this.label_SelectLabel.Name = "label_SelectLabel";
            // 
            // btn_MapDescClearBoxes
            // 
            resources.ApplyResources(this.btn_MapDescClearBoxes, "btn_MapDescClearBoxes");
            this.btn_MapDescClearBoxes.Name = "btn_MapDescClearBoxes";
            this.btn_MapDescClearBoxes.TabStop = false;
            this.btn_MapDescClearBoxes.UseVisualStyleBackColor = true;
            this.btn_MapDescClearBoxes.Click += new System.EventHandler(this.btn_MapDescClearBoxes_Click);
            // 
            // btn_AddDescription
            // 
            resources.ApplyResources(this.btn_AddDescription, "btn_AddDescription");
            this.btn_AddDescription.Name = "btn_AddDescription";
            this.btn_AddDescription.TabStop = false;
            this.btn_AddDescription.UseVisualStyleBackColor = true;
            this.btn_AddDescription.Click += new System.EventHandler(this.btn_AddDescription_Click);
            // 
            // checkedListBox_Maps
            // 
            this.checkedListBox_Maps.CheckOnClick = true;
            this.checkedListBox_Maps.FormattingEnabled = true;
            resources.ApplyResources(this.checkedListBox_Maps, "checkedListBox_Maps");
            this.checkedListBox_Maps.Name = "checkedListBox_Maps";
            this.checkedListBox_Maps.Sorted = true;
            this.checkedListBox_Maps.TabStop = false;
            // 
            // label_MapList
            // 
            resources.ApplyResources(this.label_MapList, "label_MapList");
            this.label_MapList.Name = "label_MapList";
            // 
            // checkBox_AllMaps
            // 
            resources.ApplyResources(this.checkBox_AllMaps, "checkBox_AllMaps");
            this.checkBox_AllMaps.Checked = true;
            this.checkBox_AllMaps.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_AllMaps.Name = "checkBox_AllMaps";
            this.checkBox_AllMaps.TabStop = false;
            this.checkBox_AllMaps.UseVisualStyleBackColor = true;
            this.checkBox_AllMaps.CheckedChanged += new System.EventHandler(this.checkBox_AllMaps_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.checkBox_AllMaps);
            this.groupBox1.Controls.Add(this.label_MapList);
            this.groupBox1.Controls.Add(this.checkedListBox_Maps);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // cbox_selectLegendDesc
            // 
            this.cbox_selectLegendDesc.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbox_selectLegendDesc.DropDownWidth = 363;
            this.cbox_selectLegendDesc.FormattingEnabled = true;
            resources.ApplyResources(this.cbox_selectLegendDesc, "cbox_selectLegendDesc");
            this.cbox_selectLegendDesc.Name = "cbox_selectLegendDesc";
            this.cbox_selectLegendDesc.SelectionChangeCommitted += new System.EventHandler(this.cbox_selectDesc_SelectionChangeCommitted);
            // 
            // label_DescriptionCbox
            // 
            resources.ApplyResources(this.label_DescriptionCbox, "label_DescriptionCbox");
            this.label_DescriptionCbox.Name = "label_DescriptionCbox";
            // 
            // textBox1
            // 
            this.textBox1.BackColor = System.Drawing.SystemColors.Control;
            this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            resources.ApplyResources(this.textBox1, "textBox1");
            this.textBox1.ForeColor = System.Drawing.Color.Red;
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            // 
            // DockableWindowCGMDescription
            // 
            resources.ApplyResources(this, "$this");
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.cbox_SelectItemType);
            this.Controls.Add(this.label_SelectItemType);
            this.Controls.Add(this.cbox_selectLegendItem);
            this.Controls.Add(this.label_SelectLabel);
            this.Controls.Add(this.label_Description);
            this.Controls.Add(this.cbox_selectLegendDesc);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.txtbox_Description);
            this.Controls.Add(this.label_DescriptionCbox);
            this.Controls.Add(this.btn_AddDescription);
            this.Controls.Add(this.btn_MapDescClearBoxes);
            this.DataBindings.Add(new System.Windows.Forms.Binding("Enabled", global::GSC_ProjectEditor.Properties.Settings.Default, "dwEnabling", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.Enabled = global::GSC_ProjectEditor.Properties.Settings.Default.dwEnabling;
            this.Name = "DockableWindowCGMDescription";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label_Description;
        private System.Windows.Forms.ComboBox cbox_SelectItemType;
        private System.Windows.Forms.Label label_SelectItemType;
        private System.Windows.Forms.TextBox txtbox_Description;
        private System.Windows.Forms.ComboBox cbox_selectLegendItem;
        private System.Windows.Forms.Label label_SelectLabel;
        private System.Windows.Forms.Button btn_MapDescClearBoxes;
        private System.Windows.Forms.Button btn_AddDescription;
        private System.Windows.Forms.CheckedListBox checkedListBox_Maps;
        private System.Windows.Forms.Label label_MapList;
        private System.Windows.Forms.CheckBox checkBox_AllMaps;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox cbox_selectLegendDesc;
        private System.Windows.Forms.Label label_DescriptionCbox;
        protected internal System.Windows.Forms.TextBox textBox1;

    }
}
