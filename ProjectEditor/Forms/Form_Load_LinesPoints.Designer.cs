namespace GSC_ProjectEditor
{
    partial class Form_Load_LinesPoints
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_Load_LinesPoints));
            this.btn_CancelAddGEMData = new System.Windows.Forms.Button();
            this.btn_ImportGEMData = new System.Windows.Forms.Button();
            this.label_Import = new System.Windows.Forms.Label();
            this.btn_DataBrowser = new System.Windows.Forms.Button();
            this.txtbox_DataPath = new System.Windows.Forms.TextBox();
            this.cbox_DataType = new System.Windows.Forms.ComboBox();
            this.cbox_SelectIDField = new System.Windows.Forms.ComboBox();
            this.label_SelectDataType = new System.Windows.Forms.Label();
            this.label_SelectIDField = new System.Windows.Forms.Label();
            this.checkBox_FieldValidation = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // btn_CancelAddGEMData
            // 
            this.btn_CancelAddGEMData.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.btn_CancelAddGEMData, "btn_CancelAddGEMData");
            this.btn_CancelAddGEMData.Name = "btn_CancelAddGEMData";
            this.btn_CancelAddGEMData.TabStop = false;
            this.btn_CancelAddGEMData.UseVisualStyleBackColor = true;
            this.btn_CancelAddGEMData.Click += new System.EventHandler(this.btn_CancelAddGEMData_Click);
            // 
            // btn_ImportGEMData
            // 
            resources.ApplyResources(this.btn_ImportGEMData, "btn_ImportGEMData");
            this.btn_ImportGEMData.Name = "btn_ImportGEMData";
            this.btn_ImportGEMData.TabStop = false;
            this.btn_ImportGEMData.UseVisualStyleBackColor = true;
            this.btn_ImportGEMData.Click += new System.EventHandler(this.btn_ImportGEMData_Click);
            // 
            // label_Import
            // 
            resources.ApplyResources(this.label_Import, "label_Import");
            this.label_Import.Name = "label_Import";
            // 
            // btn_DataBrowser
            // 
            resources.ApplyResources(this.btn_DataBrowser, "btn_DataBrowser");
            this.btn_DataBrowser.Name = "btn_DataBrowser";
            this.btn_DataBrowser.TabStop = false;
            this.btn_DataBrowser.UseVisualStyleBackColor = true;
            this.btn_DataBrowser.Click += new System.EventHandler(this.btn_DataBrowser_Click);
            // 
            // txtbox_DataPath
            // 
            resources.ApplyResources(this.txtbox_DataPath, "txtbox_DataPath");
            this.txtbox_DataPath.Name = "txtbox_DataPath";
            // 
            // cbox_DataType
            // 
            this.cbox_DataType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbox_DataType.FormattingEnabled = true;
            this.cbox_DataType.Items.AddRange(new object[] {
            resources.GetString("cbox_DataType.Items"),
            resources.GetString("cbox_DataType.Items1")});
            resources.ApplyResources(this.cbox_DataType, "cbox_DataType");
            this.cbox_DataType.Name = "cbox_DataType";
            this.cbox_DataType.SelectedIndexChanged += new System.EventHandler(this.cbox_DataType_SelectedIndexChanged);
            // 
            // cbox_SelectIDField
            // 
            this.cbox_SelectIDField.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbox_SelectIDField.FormattingEnabled = true;
            resources.ApplyResources(this.cbox_SelectIDField, "cbox_SelectIDField");
            this.cbox_SelectIDField.Name = "cbox_SelectIDField";
            // 
            // label_SelectDataType
            // 
            resources.ApplyResources(this.label_SelectDataType, "label_SelectDataType");
            this.label_SelectDataType.Name = "label_SelectDataType";
            // 
            // label_SelectIDField
            // 
            resources.ApplyResources(this.label_SelectIDField, "label_SelectIDField");
            this.label_SelectIDField.Name = "label_SelectIDField";
            // 
            // checkBox_FieldValidation
            // 
            resources.ApplyResources(this.checkBox_FieldValidation, "checkBox_FieldValidation");
            this.checkBox_FieldValidation.Checked = true;
            this.checkBox_FieldValidation.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_FieldValidation.Name = "checkBox_FieldValidation";
            this.checkBox_FieldValidation.UseVisualStyleBackColor = true;
            // 
            // FormAppendProjectData
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.checkBox_FieldValidation);
            this.Controls.Add(this.label_SelectIDField);
            this.Controls.Add(this.label_SelectDataType);
            this.Controls.Add(this.cbox_SelectIDField);
            this.Controls.Add(this.cbox_DataType);
            this.Controls.Add(this.btn_CancelAddGEMData);
            this.Controls.Add(this.btn_ImportGEMData);
            this.Controls.Add(this.label_Import);
            this.Controls.Add(this.btn_DataBrowser);
            this.Controls.Add(this.txtbox_DataPath);
            this.DataBindings.Add(new System.Windows.Forms.Binding("Enabled", global::GSC_ProjectEditor.Properties.Settings.Default, "dwEnabling", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.Enabled = global::GSC_ProjectEditor.Properties.Settings.Default.dwEnabling;
            this.Name = "Form_Load_LinesPoints";
            this.ShowIcon = false;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btn_CancelAddGEMData;
        private System.Windows.Forms.Button btn_ImportGEMData;
        private System.Windows.Forms.Label label_Import;
        private System.Windows.Forms.Button btn_DataBrowser;
        private System.Windows.Forms.TextBox txtbox_DataPath;
        private System.Windows.Forms.ComboBox cbox_DataType;
        private System.Windows.Forms.ComboBox cbox_SelectIDField;
        private System.Windows.Forms.Label label_SelectDataType;
        private System.Windows.Forms.Label label_SelectIDField;
        private System.Windows.Forms.CheckBox checkBox_FieldValidation;

    }
}