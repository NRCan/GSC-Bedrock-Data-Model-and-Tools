namespace GSC_ProjectEditor
{
    partial class Form_Load_CartographicPoints
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_Load_CartographicPoints));
            this.button_Add = new System.Windows.Forms.Button();
            this.button_Cancel = new System.Windows.Forms.Button();
            this.comboBox_Theme = new System.Windows.Forms.ComboBox();
            this.label_CartoTheme = new System.Windows.Forms.Label();
            this.tabControl_CartoPoint = new System.Windows.Forms.TabControl();
            this.tabPage_GIS = new System.Windows.Forms.TabPage();
            this.label_latitude = new System.Windows.Forms.Label();
            this.label_longitude = new System.Windows.Forms.Label();
            this.comboBox_latitude = new System.Windows.Forms.ComboBox();
            this.comboBox_longitude = new System.Windows.Forms.ComboBox();
            this.label_ZField = new System.Windows.Forms.Label();
            this.label_YField = new System.Windows.Forms.Label();
            this.label_XField = new System.Windows.Forms.Label();
            this.comboBox_ZField = new System.Windows.Forms.ComboBox();
            this.comboBox_YField = new System.Windows.Forms.ComboBox();
            this.comboBox_XField = new System.Windows.Forms.ComboBox();
            this.tabPage_CartoInfo = new System.Windows.Forms.TabPage();
            this.label_ScaleField = new System.Windows.Forms.Label();
            this.label_AngleField = new System.Windows.Forms.Label();
            this.label_SymbolField = new System.Windows.Forms.Label();
            this.comboBox_ScaleField = new System.Windows.Forms.ComboBox();
            this.comboBox_AngleField = new System.Windows.Forms.ComboBox();
            this.comboBox_SymbolField = new System.Windows.Forms.ComboBox();
            this.tabPage_Attributes = new System.Windows.Forms.TabPage();
            this.checkedListBox_Fields = new System.Windows.Forms.CheckedListBox();
            this.checkBox_KeepAllFields = new System.Windows.Forms.CheckBox();
            this.tabPage_Metadata = new System.Windows.Forms.TabPage();
            this.label_Source = new System.Windows.Forms.Label();
            this.comboBox_Source = new System.Windows.Forms.ComboBox();
            this.button_AddTheme = new System.Windows.Forms.Button();
            this.tabControl_CartoPoint.SuspendLayout();
            this.tabPage_GIS.SuspendLayout();
            this.tabPage_CartoInfo.SuspendLayout();
            this.tabPage_Attributes.SuspendLayout();
            this.tabPage_Metadata.SuspendLayout();
            this.SuspendLayout();
            // 
            // button_Add
            // 
            resources.ApplyResources(this.button_Add, "button_Add");
            this.button_Add.Name = "button_Add";
            this.button_Add.UseVisualStyleBackColor = true;
            this.button_Add.Click += new System.EventHandler(this.button_Load_Click);
            // 
            // button_Cancel
            // 
            resources.ApplyResources(this.button_Cancel, "button_Cancel");
            this.button_Cancel.Name = "button_Cancel";
            this.button_Cancel.UseVisualStyleBackColor = true;
            this.button_Cancel.Click += new System.EventHandler(this.button_Cancel_Click);
            // 
            // comboBox_Theme
            // 
            resources.ApplyResources(this.comboBox_Theme, "comboBox_Theme");
            this.comboBox_Theme.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_Theme.FormattingEnabled = true;
            this.comboBox_Theme.Name = "comboBox_Theme";
            // 
            // label_CartoTheme
            // 
            resources.ApplyResources(this.label_CartoTheme, "label_CartoTheme");
            this.label_CartoTheme.Name = "label_CartoTheme";
            // 
            // tabControl_CartoPoint
            // 
            resources.ApplyResources(this.tabControl_CartoPoint, "tabControl_CartoPoint");
            this.tabControl_CartoPoint.Controls.Add(this.tabPage_GIS);
            this.tabControl_CartoPoint.Controls.Add(this.tabPage_CartoInfo);
            this.tabControl_CartoPoint.Controls.Add(this.tabPage_Attributes);
            this.tabControl_CartoPoint.Controls.Add(this.tabPage_Metadata);
            this.tabControl_CartoPoint.Name = "tabControl_CartoPoint";
            this.tabControl_CartoPoint.SelectedIndex = 0;
            // 
            // tabPage_GIS
            // 
            resources.ApplyResources(this.tabPage_GIS, "tabPage_GIS");
            this.tabPage_GIS.Controls.Add(this.label_latitude);
            this.tabPage_GIS.Controls.Add(this.label_longitude);
            this.tabPage_GIS.Controls.Add(this.comboBox_latitude);
            this.tabPage_GIS.Controls.Add(this.comboBox_longitude);
            this.tabPage_GIS.Controls.Add(this.label_ZField);
            this.tabPage_GIS.Controls.Add(this.label_YField);
            this.tabPage_GIS.Controls.Add(this.label_XField);
            this.tabPage_GIS.Controls.Add(this.comboBox_ZField);
            this.tabPage_GIS.Controls.Add(this.comboBox_YField);
            this.tabPage_GIS.Controls.Add(this.comboBox_XField);
            this.tabPage_GIS.Name = "tabPage_GIS";
            this.tabPage_GIS.UseVisualStyleBackColor = true;
            // 
            // label_latitude
            // 
            resources.ApplyResources(this.label_latitude, "label_latitude");
            this.label_latitude.Name = "label_latitude";
            // 
            // label_longitude
            // 
            resources.ApplyResources(this.label_longitude, "label_longitude");
            this.label_longitude.Name = "label_longitude";
            // 
            // comboBox_latitude
            // 
            resources.ApplyResources(this.comboBox_latitude, "comboBox_latitude");
            this.comboBox_latitude.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.comboBox_latitude.FormattingEnabled = true;
            this.comboBox_latitude.Name = "comboBox_latitude";
            // 
            // comboBox_longitude
            // 
            resources.ApplyResources(this.comboBox_longitude, "comboBox_longitude");
            this.comboBox_longitude.FormattingEnabled = true;
            this.comboBox_longitude.Name = "comboBox_longitude";
            // 
            // label_ZField
            // 
            resources.ApplyResources(this.label_ZField, "label_ZField");
            this.label_ZField.Name = "label_ZField";
            // 
            // label_YField
            // 
            resources.ApplyResources(this.label_YField, "label_YField");
            this.label_YField.Name = "label_YField";
            // 
            // label_XField
            // 
            resources.ApplyResources(this.label_XField, "label_XField");
            this.label_XField.Name = "label_XField";
            // 
            // comboBox_ZField
            // 
            resources.ApplyResources(this.comboBox_ZField, "comboBox_ZField");
            this.comboBox_ZField.FormattingEnabled = true;
            this.comboBox_ZField.Name = "comboBox_ZField";
            // 
            // comboBox_YField
            // 
            resources.ApplyResources(this.comboBox_YField, "comboBox_YField");
            this.comboBox_YField.FormattingEnabled = true;
            this.comboBox_YField.Name = "comboBox_YField";
            // 
            // comboBox_XField
            // 
            resources.ApplyResources(this.comboBox_XField, "comboBox_XField");
            this.comboBox_XField.FormattingEnabled = true;
            this.comboBox_XField.Name = "comboBox_XField";
            // 
            // tabPage_CartoInfo
            // 
            resources.ApplyResources(this.tabPage_CartoInfo, "tabPage_CartoInfo");
            this.tabPage_CartoInfo.Controls.Add(this.label_ScaleField);
            this.tabPage_CartoInfo.Controls.Add(this.label_AngleField);
            this.tabPage_CartoInfo.Controls.Add(this.label_SymbolField);
            this.tabPage_CartoInfo.Controls.Add(this.comboBox_ScaleField);
            this.tabPage_CartoInfo.Controls.Add(this.comboBox_AngleField);
            this.tabPage_CartoInfo.Controls.Add(this.comboBox_SymbolField);
            this.tabPage_CartoInfo.Name = "tabPage_CartoInfo";
            this.tabPage_CartoInfo.UseVisualStyleBackColor = true;
            // 
            // label_ScaleField
            // 
            resources.ApplyResources(this.label_ScaleField, "label_ScaleField");
            this.label_ScaleField.Name = "label_ScaleField";
            // 
            // label_AngleField
            // 
            resources.ApplyResources(this.label_AngleField, "label_AngleField");
            this.label_AngleField.Name = "label_AngleField";
            // 
            // label_SymbolField
            // 
            resources.ApplyResources(this.label_SymbolField, "label_SymbolField");
            this.label_SymbolField.Name = "label_SymbolField";
            // 
            // comboBox_ScaleField
            // 
            resources.ApplyResources(this.comboBox_ScaleField, "comboBox_ScaleField");
            this.comboBox_ScaleField.FormattingEnabled = true;
            this.comboBox_ScaleField.Name = "comboBox_ScaleField";
            // 
            // comboBox_AngleField
            // 
            resources.ApplyResources(this.comboBox_AngleField, "comboBox_AngleField");
            this.comboBox_AngleField.FormattingEnabled = true;
            this.comboBox_AngleField.Name = "comboBox_AngleField";
            // 
            // comboBox_SymbolField
            // 
            resources.ApplyResources(this.comboBox_SymbolField, "comboBox_SymbolField");
            this.comboBox_SymbolField.FormattingEnabled = true;
            this.comboBox_SymbolField.Name = "comboBox_SymbolField";
            // 
            // tabPage_Attributes
            // 
            resources.ApplyResources(this.tabPage_Attributes, "tabPage_Attributes");
            this.tabPage_Attributes.Controls.Add(this.checkedListBox_Fields);
            this.tabPage_Attributes.Controls.Add(this.checkBox_KeepAllFields);
            this.tabPage_Attributes.Name = "tabPage_Attributes";
            this.tabPage_Attributes.UseVisualStyleBackColor = true;
            // 
            // checkedListBox_Fields
            // 
            resources.ApplyResources(this.checkedListBox_Fields, "checkedListBox_Fields");
            this.checkedListBox_Fields.CheckOnClick = true;
            this.checkedListBox_Fields.FormattingEnabled = true;
            this.checkedListBox_Fields.Name = "checkedListBox_Fields";
            // 
            // checkBox_KeepAllFields
            // 
            resources.ApplyResources(this.checkBox_KeepAllFields, "checkBox_KeepAllFields");
            this.checkBox_KeepAllFields.Checked = true;
            this.checkBox_KeepAllFields.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_KeepAllFields.Name = "checkBox_KeepAllFields";
            this.checkBox_KeepAllFields.UseVisualStyleBackColor = true;
            this.checkBox_KeepAllFields.CheckedChanged += new System.EventHandler(this.checkBox_KeepAllFields_CheckedChanged);
            // 
            // tabPage_Metadata
            // 
            resources.ApplyResources(this.tabPage_Metadata, "tabPage_Metadata");
            this.tabPage_Metadata.Controls.Add(this.label_Source);
            this.tabPage_Metadata.Controls.Add(this.comboBox_Source);
            this.tabPage_Metadata.Name = "tabPage_Metadata";
            this.tabPage_Metadata.UseVisualStyleBackColor = true;
            // 
            // label_Source
            // 
            resources.ApplyResources(this.label_Source, "label_Source");
            this.label_Source.Name = "label_Source";
            // 
            // comboBox_Source
            // 
            resources.ApplyResources(this.comboBox_Source, "comboBox_Source");
            this.comboBox_Source.FormattingEnabled = true;
            this.comboBox_Source.Name = "comboBox_Source";
            // 
            // button_AddTheme
            // 
            resources.ApplyResources(this.button_AddTheme, "button_AddTheme");
            this.button_AddTheme.Name = "button_AddTheme";
            this.button_AddTheme.UseVisualStyleBackColor = true;
            this.button_AddTheme.Click += new System.EventHandler(this.button_AddTheme_Click);
            // 
            // FormAddCartoPoint
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.button_AddTheme);
            this.Controls.Add(this.tabControl_CartoPoint);
            this.Controls.Add(this.label_CartoTheme);
            this.Controls.Add(this.comboBox_Theme);
            this.Controls.Add(this.button_Cancel);
            this.Controls.Add(this.button_Add);
            this.Name = "Form_Load_CartographicPoints";
            this.tabControl_CartoPoint.ResumeLayout(false);
            this.tabPage_GIS.ResumeLayout(false);
            this.tabPage_GIS.PerformLayout();
            this.tabPage_CartoInfo.ResumeLayout(false);
            this.tabPage_CartoInfo.PerformLayout();
            this.tabPage_Attributes.ResumeLayout(false);
            this.tabPage_Attributes.PerformLayout();
            this.tabPage_Metadata.ResumeLayout(false);
            this.tabPage_Metadata.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button_Add;
        private System.Windows.Forms.Button button_Cancel;
        private System.Windows.Forms.ComboBox comboBox_Theme;
        private System.Windows.Forms.Label label_CartoTheme;
        private System.Windows.Forms.TabControl tabControl_CartoPoint;
        private System.Windows.Forms.TabPage tabPage_GIS;
        private System.Windows.Forms.ComboBox comboBox_ZField;
        private System.Windows.Forms.ComboBox comboBox_YField;
        private System.Windows.Forms.ComboBox comboBox_XField;
        private System.Windows.Forms.TabPage tabPage_CartoInfo;
        private System.Windows.Forms.TabPage tabPage_Attributes;
        private System.Windows.Forms.CheckedListBox checkedListBox_Fields;
        private System.Windows.Forms.CheckBox checkBox_KeepAllFields;
        private System.Windows.Forms.TabPage tabPage_Metadata;
        private System.Windows.Forms.Label label_ZField;
        private System.Windows.Forms.Label label_YField;
        private System.Windows.Forms.Label label_XField;
        private System.Windows.Forms.Label label_ScaleField;
        private System.Windows.Forms.Label label_AngleField;
        private System.Windows.Forms.Label label_SymbolField;
        private System.Windows.Forms.ComboBox comboBox_ScaleField;
        private System.Windows.Forms.ComboBox comboBox_AngleField;
        private System.Windows.Forms.ComboBox comboBox_SymbolField;
        private System.Windows.Forms.Label label_Source;
        private System.Windows.Forms.ComboBox comboBox_Source;
        private System.Windows.Forms.Button button_AddTheme;
        private System.Windows.Forms.Label label_latitude;
        private System.Windows.Forms.Label label_longitude;
        private System.Windows.Forms.ComboBox comboBox_latitude;
        private System.Windows.Forms.ComboBox comboBox_longitude;
    }
}