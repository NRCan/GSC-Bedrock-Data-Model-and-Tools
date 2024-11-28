namespace GSC_ProjectEditor
{
    partial class Form_Load_TranslateGanfeldPointStructure
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_Load_TranslateGanfeldPointStructure));
            this.btn_BrowseData = new System.Windows.Forms.Button();
            this.btn_Cancel = new System.Windows.Forms.Button();
            this.btn_ImportData = new System.Windows.Forms.Button();
            this.label_SelectData = new System.Windows.Forms.Label();
            this.txtbox_DataPath = new System.Windows.Forms.TextBox();
            this.groupBox_DataType = new System.Windows.Forms.GroupBox();
            this.radioButton_Ganfeld = new System.Windows.Forms.RadioButton();
            this.radioButton_GSCFieldApp = new System.Windows.Forms.RadioButton();
            this.groupBox_DataType.SuspendLayout();
            this.SuspendLayout();
            // 
            // btn_BrowseData
            // 
            resources.ApplyResources(this.btn_BrowseData, "btn_BrowseData");
            this.btn_BrowseData.Name = "btn_BrowseData";
            this.btn_BrowseData.UseVisualStyleBackColor = true;
            this.btn_BrowseData.Click += new System.EventHandler(this.btn_BrowseData_Click);
            // 
            // btn_Cancel
            // 
            resources.ApplyResources(this.btn_Cancel, "btn_Cancel");
            this.btn_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btn_Cancel.Name = "btn_Cancel";
            this.btn_Cancel.UseVisualStyleBackColor = true;
            this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
            // 
            // btn_ImportData
            // 
            resources.ApplyResources(this.btn_ImportData, "btn_ImportData");
            this.btn_ImportData.Name = "btn_ImportData";
            this.btn_ImportData.UseVisualStyleBackColor = true;
            this.btn_ImportData.Click += new System.EventHandler(this.btn_ImportData_Click);
            // 
            // label_SelectData
            // 
            resources.ApplyResources(this.label_SelectData, "label_SelectData");
            this.label_SelectData.Name = "label_SelectData";
            // 
            // txtbox_DataPath
            // 
            resources.ApplyResources(this.txtbox_DataPath, "txtbox_DataPath");
            this.txtbox_DataPath.Name = "txtbox_DataPath";
            // 
            // groupBox_DataType
            // 
            resources.ApplyResources(this.groupBox_DataType, "groupBox_DataType");
            this.groupBox_DataType.Controls.Add(this.radioButton_Ganfeld);
            this.groupBox_DataType.Controls.Add(this.radioButton_GSCFieldApp);
            this.groupBox_DataType.Name = "groupBox_DataType";
            this.groupBox_DataType.TabStop = false;
            // 
            // radioButton_Ganfeld
            // 
            resources.ApplyResources(this.radioButton_Ganfeld, "radioButton_Ganfeld");
            this.radioButton_Ganfeld.Name = "radioButton_Ganfeld";
            this.radioButton_Ganfeld.UseVisualStyleBackColor = true;
            // 
            // radioButton_GSCFieldApp
            // 
            resources.ApplyResources(this.radioButton_GSCFieldApp, "radioButton_GSCFieldApp");
            this.radioButton_GSCFieldApp.Checked = true;
            this.radioButton_GSCFieldApp.Name = "radioButton_GSCFieldApp";
            this.radioButton_GSCFieldApp.TabStop = true;
            this.radioButton_GSCFieldApp.UseVisualStyleBackColor = true;
            // 
            // FormTranslateFStrucToGEOPOINT
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox_DataType);
            this.Controls.Add(this.btn_BrowseData);
            this.Controls.Add(this.btn_Cancel);
            this.Controls.Add(this.btn_ImportData);
            this.Controls.Add(this.label_SelectData);
            this.Controls.Add(this.txtbox_DataPath);
            this.Name = "Form_Load_TranslateGanfeldPointStructure";
            this.groupBox_DataType.ResumeLayout(false);
            this.groupBox_DataType.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btn_BrowseData;
        private System.Windows.Forms.Button btn_Cancel;
        private System.Windows.Forms.Button btn_ImportData;
        private System.Windows.Forms.Label label_SelectData;
        private System.Windows.Forms.TextBox txtbox_DataPath;
        private System.Windows.Forms.GroupBox groupBox_DataType;
        private System.Windows.Forms.RadioButton radioButton_Ganfeld;
        private System.Windows.Forms.RadioButton radioButton_GSCFieldApp;
    }
}